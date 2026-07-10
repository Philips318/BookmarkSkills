## CT Software Department Architecture Principles

### Layering Rules

```
┌─────────────────────────────┐
│  Presentation (Views/XAML)  │  ← Only UI rendering, no logic
├─────────────────────────────┤
│  ViewModel                  │  ← Commands, bindings, UI state
├─────────────────────────────┤
│  Application Services       │  ← Use cases, workflow orchestration
├─────────────────────────────┤
│  Domain Models              │  ← Business entities, value objects
├─────────────────────────────┤
│  Infrastructure             │  ← Repositories, DICOM, file I/O
└─────────────────────────────┘
```

**Dependency Rule:** Each layer may only depend on the layer directly below it. Never upward. Never skip layers.

### Forbidden Dependencies

| From | To | Why |
|------|----|-----|
| View | Service / Repository | Bypasses ViewModel |
| ViewModel | Repository (direct) | Must go through Service |
| Domain | Infrastructure | Domain is pure, no I/O |
| Any layer | Concrete class in another layer | Use interfaces |

### MVVM Rules for WPF Projects

1. **View (XAML)**
   - Data binding only — no `Click` handlers with business logic.
   - Use `Command` binding for user actions.
   - Code-behind limited to: visual-only logic (animations, focus management).

2. **ViewModel**
   - Implements `INotifyPropertyChanged`.
   - Exposes `ICommand` for user actions.
   - No direct references to UI types (`Window`, `Control`, `Brush`).
   - Injected dependencies via constructor.

3. **Model / Service**
   - Business logic lives here.
   - No knowledge of UI or ViewModel.
   - Testable in isolation (no UI thread dependency).

### Dependency Injection

- **New projects (.NET 8):** Use built-in `Microsoft.Extensions.DependencyInjection`.
- **Legacy projects (.NET 4.8):** Use Autofac (already in use).
- Register as interface → implementation.
- Prefer `Transient` unless state sharing is required.
- `Singleton` only for: configuration, logging, shared caches.

### Shared Component Guidelines

| Component | Package Type | Consumers |
|-----------|-------------|-----------|
| PPTCommon | Project reference / NuGet | All projects |
| CT_SW_Common | Project reference | MIA, MIC, VolumeEngine |
| DirectResultPipeline | NuGet | Service layer |

**Rules for shared components:**
- Breaking API changes require ADR + version bump + migration guide.
- All public APIs must have XML doc comments.
- Minimum 70% UT coverage for shared components.

### DICOM Considerations

- Wrap DICOM protocol behind `IDicomService` interface.
- No raw DICOM tag access in ViewModel or Service layer.
- DICOM parsing errors must be caught and logged, never crash the UI.
- Patient data must never appear in log files or AI prompts.

### Design Pattern Preferences

| Scenario | Preferred Pattern | Avoid |
|----------|------------------|-------|
| Multiple algorithms | Strategy | Switch/case blocks |
| Object creation | Factory / Builder | `new` in business logic |
| Event communication | Observer / EventAggregator | Direct coupling |
| State machines | State pattern | Nested if/else |
| Cross-cutting concerns | Decorator / Middleware | Base class inheritance |
| Configuration | Options pattern | Static config access |
