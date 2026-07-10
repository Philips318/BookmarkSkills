# ADR-001: WW/WC Reset to Default — Design Decision

**Date:** 2026-07-02
**Status:** Accepted
**Deciders:** [sw-architect, user (Gate 1 approval 2026-07-02)]
**Supersedes:** N/A

## Context

The CT image display application requires a "Reset Window/Level to Default" feature (FR-01 through FR-07) that restores DICOM-default Window Width (0028,1051) and Window Center (0028,1050) values for the active viewport. The application is a WPF desktop client following MVVM architecture, classified as IEC 62304 Class B because incorrect WW/WC rendering can mask clinically significant features, contributing to missed or delayed diagnosis.

This is a **greenfield** project — no `Src/` tree exists yet. This ADR defines the project layout, layered architecture, component responsibilities, interface contracts, and the binding constraints that `@developer` must follow.

Key forces driving this decision:

- **MVVM purity:** WPF views must have zero code-behind; all logic flows through ViewModel commands.
- **DICOM correctness:** WW must always be > 0; fallback values must be configurable without rebuild.
- **IEC 62304 Class B traceability:** All DICOM tag access must be through a dedicated abstraction; no magic numbers.
- **Undo requirement (FR-06):** The reset action must be undoable; this requires undo infrastructure.
- **Diagnostic workflow:** The reset must complete within 200 ms (NFR-01) and provide non-blocking user feedback.

## Decision

We will implement the WW/WC Reset feature using a **layered MVVM architecture** with clearly separated interface contracts (`ExtInf/`) and implementations (`Src/`), connected via constructor-injected dependencies. The reset action is exposed as an `IRelayCommand` on the ViewModel, the DICOM defaults live in the model layer, fallback values come from a configuration service, and user feedback is delivered through a status-bar notification service.

## Rationale

### 1. Command Binding Pattern — `RelayCommand` (IRelayCommand)

**Chosen:** `RelayCommand` implementing `IRelayCommand` (from CommunityToolkit.Mvvm or equivalent self-contained implementation).

**Why:** `RelayCommand` is the idiomatic WPF MVVM command pattern. It wraps an `Execute` delegate and a `CanExecute` predicate, supports `ICommand.CanExecuteChanged` for automatic button enable/disable, and requires no framework beyond the MVVM toolkit. Using `IRelayCommand` (which extends `ICommand` with `NotifyCanExecuteChanged()`) allows the ViewModel to explicitly re-evaluate the button's enabled state when the loaded series changes.

**Rejected alternatives:**
- `DelegateCommand` (Prism) — functionally equivalent but couples the project to the Prism framework, which is not otherwise required.
- Raw `ICommand` implementation — excessive boilerplate for a standard command pattern.

### 2. Default Value Storage — Model-Owned, On-Demand Access

**Chosen:** The `DicomImageModel` reads DICOM tags (0028,1050/1051) when the series is loaded and exposes them via a `DefaultWindowLevel` property (nullable `WindowLevel` value object). The ViewModel retrieves defaults from the model property — no separate caching.

**Why:** The model is the single source of truth for DICOM data. Caching defaults in the ViewModel would duplicate state and create synchronisation risk when the loaded series changes. The on-demand pattern keeps the data flow unidirectional: Model → ViewModel → View.

### 3. Fallback Strategy — `IWindowLevelDefaults` Configuration Service

**Chosen:** An `IWindowLevelDefaults` interface with a single property `FallbackWindowLevel` that returns the configurable fallback pair (default: WW = 400, WC = 40). The implementation reads from application settings (e.g., `appsettings.json` or `.config` file) — values are changeable without rebuild (FR-04).

**Why:** Separating fallback configuration into its own service adheres to the Single Responsibility Principle and makes it independently testable. The ViewModel logic becomes: "Use model defaults if valid; else use fallback service defaults." The fallback WW is validated (> 0) at the service level.

### 4. Confirmation Message — Status-Bar via `IStatusNotifier`

**Chosen:** Status-bar notification delivered through an `IStatusNotifier` interface. The implementation (`StatusBarNotifier`) sets a bound `StatusMessage` property and starts a `DispatcherTimer` for auto-dismiss after the configured display duration (minimum 2 seconds per FR-05).

**Why:** A status-bar message is non-blocking, does not obscure clinical image content (R-04), and is the standard feedback mechanism for low-severity confirmations in medical imaging applications. The `IStatusNotifier` abstraction maintains MVVM separation — the ViewModel calls `ShowTransientMessage(string, TimeSpan)` without knowing the UI implementation. Toast/InfoBar overlays were rejected because they risk obscuring the diagnostic image area (R-04).

### 5. Undo Support — Lightweight `IUndoService`

**Chosen:** A scoped `IUndoService` with `Push(IUndoableAction)` and `Undo()` / `Redo()` methods. The reset command pushes a `WindowLevelChangeAction` (implementing `IUndoableAction`) that captures the pre-reset WW/WC values. `Undo()` restores those values. The undo service is injected into `ImageDisplayViewModel`.

**Why:** FR-06 mandates undo. Per OQ-02, we assume no existing undo stack exists (greenfield). The `IUndoService` / `IUndoableAction` pattern is minimal, extensible to other display actions later, and keeps the undo concern out of the command itself (SRP). The undo stack depth is bounded (configurable, default 20) to prevent unbounded memory growth.

### 6. WW Validity Guard — Centralised in `WindowLevel` Value Object

**Chosen:** A `WindowLevel` record/struct with a factory method `WindowLevel.Create(double ww, double wc)` that throws `ArgumentOutOfRangeException` if WW ≤ 0. All code paths that produce a `WindowLevel` go through this factory. Invalid values from DICOM or the undo stack are caught at construction time (FR-07).

**Why:** Centralising the WW > 0 invariant in the value object means the validation cannot be bypassed, regardless of the call site (model, undo, fallback). This is a safety guard — per domain knowledge, WW ≤ 0 produces a black or inverted image.

### 7. DICOM Tag Access — Centralised `DicomDisplayTags` Constants

**Chosen:** A static class `DicomDisplayTags` in `Src/ImageDisplay/Constants/` holding named constants for tags (0028,1050), (0028,1051), and (0028,1055). Each constant has an XML doc comment referencing DICOM PS3.3 §C.7.6.3.1.5 (NFR-05). The `DicomImageModel` uses these constants — no magic-number tag literals anywhere else.

## Architectural Envelope (binding constraints for @developer)

### Project Structure (greenfield)

```
ExtInf/
  ImageDisplay/
    IImageDisplayViewModel.cs      — ViewModel contract
    IDicomImageModel.cs            — Model contract (DICOM data)
    IStatusNotifier.cs             — Transient message delivery
    IUndoService.cs                — Undo/Redo stack
    IUndoableAction.cs             — Single undoable action
    IWindowLevelDefaults.cs        — Fallback configuration
    WindowLevel.cs                 — Value object (shared between ExtInf and Src)

Src/
  ImageDisplay/
    ImageDisplayViewModel.cs       — ViewModel implementation
    Commands/
      ResetWindowLevelCommand.cs   — IRelayCommand for the reset action
    Models/
      DicomImageModel.cs           — IDicomImageModel implementation
      WindowLevelChangeAction.cs   — IUndoableAction for WW/WC undo
    Services/
      StatusBarNotifier.cs         — IStatusNotifier implementation
      WindowLevelUndoService.cs    — IUndoService implementation
      WindowLevelDefaults.cs       — IWindowLevelDefaults implementation
    Constants/
      DicomDisplayTags.cs          — DICOM tag constants (0028,1050/1051/1055)
    Resources/
      ImageDisplayStrings.resx     — Localised UI strings
    Views/
      ImageDisplayView.xaml        — WPF View (XAML only, zero code-behind)
    Test/
      ImageDisplayViewModelTests.cs
      ResetWindowLevelCommandTests.cs
      DicomImageModelTests.cs
      WindowLevelTests.cs
      WindowLevelUndoServiceTests.cs
      WindowLevelDefaultsTests.cs

  ModuleTests/
    WwWcReset/
      Features/
        WwWcReset.feature          — BDD Gherkin scenarios
      Steps/
        WwWcResetSteps.cs          — Reqnroll step definitions
```

### Namespace

All classes under `Philips.CT.Host.ImageDisplay` — sub-namespaces for `.Commands`, `.Models`, `.Services`, `.Constants`, `.Views`.

### Mandatory Types and Members

| Type | Location | Key Members |
|------|----------|-------------|
| `WindowLevel` | `ExtInf/ImageDisplay/` | `double Width` (> 0), `double Center`, `string? PresetName`, static `Create(double ww, double wc, string? name = null)` |
| `IImageDisplayViewModel` | `ExtInf/ImageDisplay/` | `double WindowWidth { get; set; }`, `double WindowCenter { get; set; }`, `IRelayCommand ResetWindowLevelCommand { get; }`, `bool IsSeriesLoaded { get; }` |
| `IDicomImageModel` | `ExtInf/ImageDisplay/` | `WindowLevel? DefaultWindowLevel { get; }`, `bool HasValidDefaultWindowLevel { get; }` |
| `IStatusNotifier` | `ExtInf/ImageDisplay/` | `void ShowTransientMessage(string message, TimeSpan duration)` |
| `IUndoService` | `ExtInf/ImageDisplay/` | `void Push(IUndoableAction action)`, `void Undo()`, `void Redo()`, `bool CanUndo { get; }`, `bool CanRedo { get; }` |
| `IUndoableAction` | `ExtInf/ImageDisplay/` | `void Execute()`, `void Undo()`, `string Description { get; }` |
| `IWindowLevelDefaults` | `ExtInf/ImageDisplay/` | `WindowLevel FallbackWindowLevel { get; }` |
| `ImageDisplayViewModel` | `Src/ImageDisplay/` | Implements `IImageDisplayViewModel`, `INotifyPropertyChanged`. Constructor-injected: `IDicomImageModel`, `IStatusNotifier`, `IUndoService`, `IWindowLevelDefaults` |
| `ResetWindowLevelCommand` | `Src/ImageDisplay/Commands/` | `IRelayCommand`. `CanExecute` → true only when `IsSeriesLoaded`. `Execute` → reads default from model (or fallback), validates WW > 0, pushes undo action, updates ViewModel properties, fires status notification. |
| `DicomDisplayTags` | `Src/ImageDisplay/Constants/` | `static readonly` fields for tags `WindowCenter` (0028,1050), `WindowWidth` (0028,1051), `WindowCenterWidthExplanation` (0028,1055). XML doc referencing PS3.3 §C.7.6.3.1.5. |

### Design Rules

1. **Zero code-behind** in `ImageDisplayView.xaml` — all button click handling via `{Binding ResetWindowLevelCommand}`.
2. **Constructor injection** for all ViewModel dependencies — no service locator, no `new` inside ViewModel.
3. **WW > 0 invariant** enforced by `WindowLevel.Create()` — throw `ArgumentOutOfRangeException` on violation.
4. **First preset (index 0)** when DICOM tags are multi-valued (FR-03).
5. **All UI strings from `ImageDisplayStrings.resx`** — button label, tooltip, confirmation message template.
6. **Keyboard shortcut `Ctrl+Shift+W`** bound via `InputBinding` / `KeyBinding` in XAML (NFR-02, subject to OQ-05 confirmation).
7. **Logging:** WARNING-level log on fallback activation (FR-04) and invalid WW rejection (FR-07). No PHI in logs (NFR-03).
8. **No unhandled exceptions** — the `ResetWindowLevelCommand.Execute` method wraps its body in a try/catch; errors logged at WARNING, user notified via `IStatusNotifier`, current WW/WC left unchanged (NFR-04).

### Design Notes per Task (for @product-owner backlog embedding)

| Task Area | `design_note` |
|-----------|--------------|
| **View (XAML)** | Add a `Button` with `Command="{Binding ResetWindowLevelCommand}"`, `ToolTip` from resource, and `KeyBinding` for Ctrl+Shift+W. Include `AutomationProperties.AutomationId="ResetWindowLevelButton"` for FlaUI testing. Status bar `TextBlock` bound to a `StatusMessage` property. |
| **ViewModel** | `ImageDisplayViewModel` : `IImageDisplayViewModel`, `INotifyPropertyChanged`. Construct with 4 injected dependencies. Expose `WindowWidth`, `WindowCenter`, `ResetWindowLevelCommand`, `IsSeriesLoaded`, `StatusMessage`. |
| **Command** | `ResetWindowLevelCommand` : `IRelayCommand`. Logic: read `IDicomImageModel.DefaultWindowLevel`; if null/invalid → use `IWindowLevelDefaults.FallbackWindowLevel`; validate WW > 0 via `WindowLevel.Create()`; push undo action; set ViewModel props; fire `IStatusNotifier`. |
| **Model** | `DicomImageModel` : `IDicomImageModel`. Parse tags (0028,1050/1051/1055) on series load using `DicomDisplayTags` constants. Expose `DefaultWindowLevel` (nullable). |
| **Undo** | `WindowLevelChangeAction` captures pre-reset WW/WC, restores on `Undo()`. `WindowLevelUndoService` manages a bounded stack (default depth 20). |
| **Fallback** | `WindowLevelDefaults` reads WW=400, WC=40 from configuration file. Validates WW > 0 on load. |
| **Status** | `StatusBarNotifier` sets a bound string property and starts a `DispatcherTimer` (2 s minimum) for auto-dismiss. |

## Consequences

### Positive

- **MVVM-clean:** The View has zero knowledge of DICOM, undo, or notification logic — all mediated by the ViewModel through interface contracts.
- **Testable:** Every component is interface-injected and can be unit-tested with NSubstitute mocks. The `WindowLevel` value object is independently testable for the WW > 0 invariant.
- **Safety guard is structural:** The WW > 0 constraint is enforced at the type level (`WindowLevel.Create()`), making it impossible for invalid values to propagate through the system without an explicit exception.
- **Extensible undo:** The `IUndoService` / `IUndoableAction` pattern extends naturally to other display actions (zoom, pan, rotation) in future features.
- **DICOM traceability:** All tag access is centralised in `DicomDisplayTags` with standard references, satisfying NFR-05 and simplifying audits.
- **IEC 62304 Class B compliance:** Clear separation of concerns, documented interfaces, and traceable design decisions support the required lifecycle documentation.

### Negative / Trade-offs

- **New undo infrastructure:** FR-06 requires an undo stack that does not yet exist. This adds scope (approximately 2 classes + interface) but is bounded and well-defined.
- **Interface proliferation:** Six new interfaces in `ExtInf/` for a single feature may seem heavy for the feature size, but this is consistent with the project's layered architecture pattern and enables independent testing.
- **DispatcherTimer for status dismiss:** Ties the `StatusBarNotifier` to WPF's `Dispatcher`. This is acceptable because it is an explicitly UI-layer component — the interface (`IStatusNotifier`) remains UI-framework-agnostic.

## Alternatives Considered

| Alternative | Description | Reason Rejected |
|-------------|-------------|-----------------|
| **DelegateCommand (Prism)** | Prism's command implementation with automatic CanExecute re-evaluation | Introduces a dependency on the Prism framework, which is not otherwise required by the project. `RelayCommand` provides equivalent functionality without the framework coupling. |
| **Cache defaults in ViewModel** | Store a copy of the DICOM default WW/WC in the ViewModel after first read | Duplicates state between Model and ViewModel, creating synchronisation risk when the loaded series changes. The Model should remain the single source of truth. |
| **Toast overlay for confirmation** | Show a floating toast notification over the image area | Risk of obscuring diagnostic image content (R-04). Status-bar placement avoids this risk and is the established pattern for non-critical confirmations in medical imaging applications. |
| **No undo (defer FR-06)** | Implement reset without undo; defer FR-06 to a future release | FR-06 is a stated requirement. Deferring would leave the feature incomplete and require re-work of the command architecture when undo is eventually added. |
| **ViewModel stores fallback constants** | Hardcode WW=400, WC=40 in the ViewModel | Violates FR-04 (configurable without rebuild) and mixes configuration concern into the ViewModel. |

## IEC 62304 Safety Class Impact

**Safety Class:** B (per requirements document classification)

The architecture supports Class B compliance through:

1. **Segregation:** The WW/WC reset logic is confined to the ImageDisplay module with well-defined interfaces. It does not affect acquisition, reconstruction, or DICOM storage pipelines.
2. **Validation guard:** The `WindowLevel` value object enforces WW > 0 at construction time, preventing the most critical safety hazard (black/inverted image from WW ≤ 0).
3. **Graceful degradation:** NFR-04 requires no unhandled exceptions. The command's try/catch + fallback pattern ensures the display is never left in an indeterminate state.
4. **Traceability:** Each interface in `ExtInf/` maps to a functional requirement. Each implementation in `Src/` can be traced back through the interface to the requirement.
5. **Testability:** All components are interface-injected and independently testable. Unit test coverage gates (≥ 80%) apply per the project's quality gate configuration.

**SAD Reference:** SAD §4.5 — Software Design, Image Display Sub-System (to be authored as part of the SSDS update).

## Open Questions Resolved by This ADR

| OQ | Resolution |
|----|-----------|
| OQ-02 | No existing undo stack assumed (greenfield). ADR defines a new `IUndoService` scoped to this module. |
| OQ-03 | DICOM tag access goes through `DicomDisplayTags` constants + model abstraction (`IDicomImageModel`). The tag abstraction layer is defined by this ADR. |
| OQ-04 | WW/WC viewport state is owned by `ImageDisplayViewModel` (properties `WindowWidth`, `WindowCenter`). The model holds the DICOM defaults; the ViewModel holds the active display state. |

## Open Questions Remaining

| OQ | Status |
|----|--------|
| OQ-01 | **Unresolved.** Class B vs Class C determination awaits Product Owner / Regulatory input. If Class C is confirmed, additional lifecycle activities (FMEA, formal verification) are required but the architecture does not change. |
| OQ-05 | **Unresolved.** Keyboard shortcut Ctrl+Shift+W needs confirmation from Product Owner. The XAML `KeyBinding` is trivially changeable. |
