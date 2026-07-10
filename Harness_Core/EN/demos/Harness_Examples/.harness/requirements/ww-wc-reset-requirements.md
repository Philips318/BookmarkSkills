---
created: "2026-07-02T00:00:00Z"
updated: "2026-07-02T00:00:00Z"
slug: ww-wc-reset
---

# WW/WC Reset to Default — Requirements

**Source artifacts:** Feature request description (inline, no external PRD file)

## User Story

As a **radiologist or CT technologist**, I want a **Reset Window/Level button** in the image display panel, so that I can instantly restore the DICOM-supplied default WW (Window Width) and WC (Window Center) values after interactive adjustments and avoid diagnostic errors caused by non-standard windowing settings.

---

## IEC 62304 Safety Classification

- **Assigned Class:** B
- **Justification:** The reset function directly affects the rendering of diagnostic CT images. Incorrect or non-default WW/WC settings can cause clinically significant features (e.g., lung nodules, haemorrhage) to be invisible or misinterpreted, contributing to missed or delayed diagnosis. Missed diagnosis constitutes non-serious-to-serious patient harm depending on the clinical context. The reset function itself is a safety mitigation — it restores known-good DICOM defaults — but it is part of the image-display pipeline which carries diagnostic impact. Per the IEC 62304 classification table, Class B applies to software where failure may contribute to non-serious injury.
- **OQ-01 resolved (2026-07-02):** Deployment context confirmed as **primary diagnostic workstation**. Class B is retained (user confirmed at Gate 1 approval).
- **OQ-02 resolved (2026-07-02):** Undo stack scope is **per-viewport** (not global application undo).
- **Patient-data path:** Yes. The function reads DICOM image display attributes (tags 0028,1050 and 0028,1051) from the loaded series dataset and writes the derived WW/WC values to the active display viewport state. It does not modify, store, or transmit patient data — it is read-only with respect to the DICOM dataset.

---

## Functional Requirements

### FR-01: Reset Button Presence and Placement

The image display panel **shall** include a visible "Reset Window/Level" button (or equivalent icon-button) located in or directly adjacent to the WW/WC controls area. The button **shall** be present whenever a CT series is loaded in the active viewport.

**Rationale:** Clinicians must be able to locate and invoke the reset action without navigating menus, reducing time-to-action during image review.

**Source:** Feature request — "Add a 'reset window/level to default' button to the image display panel"

**Verification method:** BDD / Manual — verify button is rendered and positioned correctly when a series is loaded; verify it is absent (or disabled) when no series is loaded.

---

### FR-02: Restore DICOM Default WW/WC

When the Reset Window/Level button is activated, the system **shall** set the active viewport's WW and WC to the values stored in DICOM tags (0028,1051) Window Width and (0028,1050) Window Center for the currently loaded series, using the **first value** (index 0) of each multi-valued attribute.

**Rationale:** DICOM tags 0028,1051 (VR: DS, VM: 1–n) and 0028,1050 (VR: DS, VM: 1–n) carry the scanner- or protocol-defined default window presets. Using the first value (index 0) corresponds to the primary preset, consistent with DICOM PS3.3 C.7.6.3.1.5. The restored WW value **must** be greater than 0.

**Source:** Feature request; DICOM PS3.3 §C.7.6.3.1.5; domain-knowledge WW/WC domain trigger

**Verification method:** Unit Test + BDD — inject a mock DICOM dataset with known WW/WC values; activate reset; assert viewport WW/WC equals injected values.

---

### FR-03: Multi-Preset Selection

When the loaded DICOM series contains **more than one** WW/WC preset (i.e., tags 0028,1051 and 0028,1050 have VM > 1), the system **shall** reset to the first preset (index 0) by default. The optional tag (0028,1055) Window Center & Width Explanation **shall** be used, when present, to display the name of the applied preset in the confirmation message.

**Rationale:** Multi-valued windowing attributes are common in CT (e.g., soft tissue, lung, bone presets). Defaulting to index 0 is predictable and consistent with DICOM convention. Showing the preset name improves operator confidence.

**Source:** Feature request; DICOM PS3.3 §C.7.6.3.1.5

**Verification method:** Unit Test — inject a multi-valued dataset; assert index-0 values are applied and preset name (from 0028,1055[0]) appears in the feedback message.

---

### FR-04: Fallback When DICOM Defaults Are Absent

When tags (0028,1051) or (0028,1050) are **absent** from the loaded series, the system **shall** apply a configurable software-defined fallback WW/WC pair (default: WW = 400, WC = 40, representing a standard soft-tissue window). The fallback values **shall** be configurable in application settings without requiring a software rebuild.

**Rationale:** Not all DICOM datasets include windowing attributes (they are Type 1C/optional in some IODs). A hard failure on absent tags is unacceptable in a diagnostic viewer. The fallback pair (WW 400, WC 40) is a well-established clinical starting point for CT soft tissue. WW must remain > 0.

**Source:** Feature request ("If no DICOM default exists, a sensible fallback is applied"); domain-knowledge WW > 0 rule

**Verification method:** Unit Test — inject a dataset with no windowing tags; activate reset; assert fallback values are applied. Integration test — verify fallback values are read from configuration, not hardcoded.

---

### FR-05: User Confirmation / Feedback Message

After the reset action completes, the system **shall** display a brief, non-blocking confirmation message (e.g., a transient tooltip or status-bar notification) indicating that the window/level has been reset. The message **shall** be visible for a minimum of 2 seconds and disappear automatically without requiring user dismissal. If the DICOM preset name is available (from tag 0028,1055[0]), the message **shall** include it (e.g., "Window/Level reset to 'Soft Tissue' (WW 400 / WC 40)").

**Rationale:** The user must receive immediate feedback that the action was executed, especially since the image appearance may already match the default (making the visual change subtle or invisible). A non-blocking message avoids interrupting the diagnostic workflow.

**Source:** Feature request — "shows a brief confirmation message to the user after the reset"

**Verification method:** BDD — activate reset; assert confirmation message appears within 500 ms; assert message auto-dismisses after ≥ 2 s.

---

### FR-06: Undo Support

The reset action **shall** be undoable via the standard application Undo function (e.g., Ctrl+Z or the Undo button in the toolbar). A single Undo shall restore the WW/WC values that were active immediately before the reset was invoked.

**Rationale:** Clinicians may inadvertently trigger the reset or may need to compare the adjusted view with the default. Supporting Undo maintains workflow continuity and reduces diagnostic disruption. If the application's Undo stack does not support WW/WC state, this requirement mandates that support is added as part of this feature.

**Source:** Feature request — "The reset action is undoable"

**Verification method:** BDD — set a non-default WW/WC; activate reset; invoke Undo; assert WW/WC returns to pre-reset values.

---

### FR-07: WW Validity Constraint Enforcement

The system **shall** reject any WW value ≤ 0 that arises from DICOM tags, fallback configuration, or the Undo stack. If such a value is encountered, the system **shall** log a warning, apply the configurable fallback WW/WC instead, and notify the user via the confirmation message that a fallback was used due to an invalid DICOM value.

**Rationale:** WW = 0 or negative is undefined in the DICOM standard and would produce a black or inverted image, potentially masking all image content. This is a patient-safety guard. Domain knowledge: WW > 0 is a mandatory domain rule.

**Source:** Domain-knowledge skill — WW/WC domain trigger ("WW > 0")

**Verification method:** Unit Test — inject a dataset with WW = 0 or WW = -100; assert fallback is applied and warning is logged.

---

## Non-Functional Requirements

### NFR-01: Response Time (Performance)

The reset action — from button activation to updated image rendering and confirmation message appearance — **shall** complete within **200 ms** measured on the minimum-specification target hardware. The user **shall** perceive the reset as instantaneous.

**Rationale:** Clinical workflow is time-sensitive. A perceptible delay on a simple display-parameter update degrades usability and confidence in the tool. 200 ms is below the human perception threshold for UI latency.

**Source:** Feature request — "user perceives as instant"

**Verification method:** Performance test — measure elapsed time between button activation and viewport repaint across 100 invocations; assert 95th percentile ≤ 200 ms.

---

### NFR-02: Accessibility (Usability)

The Reset Window/Level button **shall** be operable by keyboard alone, with a documented keyboard shortcut (default: **Ctrl+Shift+W** or application-standard equivalent). The button **shall** have a descriptive tooltip (e.g., "Reset Window/Level to DICOM default (Ctrl+Shift+W)") displayed on hover. The button **shall** be reachable via standard Tab-key navigation within the image display panel.

**Rationale:** Radiologists frequently operate CT viewers without a mouse (e.g., using a trackball, dictation microphone, or keyboard-only navigation). Accessibility compliance is also required for IEC 62304 Class B software in clinical environments.

**Source:** Feature request — "Accessibility (keyboard shortcut or tooltip)"

**Verification method:** Manual / BDD (FlaUI) — verify keyboard shortcut triggers reset; verify tooltip text; verify Tab navigation reaches the button.

---

### NFR-03: Security and PHI Handling

The reset function **shall not** log, transmit, or persist any patient-identifying information (PHI) as a result of the reset action. WW/WC values written to application state are display parameters only and do not constitute PHI. The configurable fallback values **shall** be stored in application configuration, not in any patient record.

**Rationale:** OWASP and HIPAA requirements prohibit unintended PHI disclosure. Display-state changes must not leak patient context to logs or telemetry beyond what is already captured by the application's existing audit trail.

**Source:** IEC 62304 Class B security obligation; OWASP Top 10 A02 (Cryptographic Failures / sensitive data exposure)

**Verification method:** Code review — inspect all code paths triggered by the reset; confirm no PHI fields are accessed or logged.

---

### NFR-04: Reliability and Degraded-Mode Behaviour

The reset function **shall** not throw an unhandled exception or crash the application under any supported DICOM dataset variant (missing tags, corrupt tag values, multi-frame series, empty series). If an error occurs internally during the reset, the system **shall** log the error at WARNING level, leave the current WW/WC unchanged, and inform the user that the reset could not be completed.

**Rationale:** Unhandled exceptions in a diagnostic viewer can interrupt clinical workflow and lose unsaved session state. The system must fail gracefully and informatively.

**Source:** IEC 62304 — no uncontrolled crashes; domain-knowledge — variety of DICOM dataset forms in clinical practice

**Verification method:** Unit Test — inject corrupted tag values (non-numeric DS strings, extreme values); assert no exception propagates; assert log entry is written.

---

### NFR-05: DICOM Standard Traceability (Maintainability)

All source code that reads DICOM tags (0028,1050), (0028,1051), and (0028,1055) **shall** reference the tag group/element numbers and the DICOM standard section (PS3.3 §C.7.6.3.1.5) in an inline XML documentation comment. No magic-number tag literals shall appear outside a dedicated DICOM constants class or file.

**Rationale:** Maintainability and audit traceability to the DICOM standard. Centralising tag literals enables validation that the correct tags are accessed and simplifies future standard updates.

**Source:** Feature request — "Traceability to DICOM standard tags"; source-code instructions — no magic numbers

**Verification method:** Code review — verify DICOM tag constants are in a dedicated constants file; verify XML doc comments reference standard section.

---

## Constraints

- **WPF/MVVM architecture:** The button and confirmation message must be implemented in XAML with a ViewModel command binding (`ICommand`). No code-behind in WPF views.
- **Namespace:** New classes must follow the `Philips.CT.Host.{ComponentName}.ImageDisplay` namespace pattern.
- **UI strings from resources:** Button label, tooltip text, and confirmation message text must be sourced from resource files (`.resx`), not hardcoded.
- **WW > 0:** The system must never allow a WW value of 0 or negative to reach the display pipeline.
- **DICOM tag access:** Tags must be accessed via the project's existing DICOM tag abstraction layer; direct DICOM dataset manipulation in the ViewModel is prohibited.
- **Zero build warnings:** Implementation must compile with zero warnings.
- **IEC 62304 Class B lifecycle:** Unit tests, integration tests, code review, and traceability documentation are mandatory before delivery.

---

## Out of Scope

- Editing or saving custom WW/WC presets to the DICOM dataset or to a user profile.
- Synchronising WW/WC reset across multiple linked viewports (single-viewport scope only unless architectural support already exists).
- Applying WW/WC to non-CT modalities (MR, PET) — this feature is CT-series-only.
- Automatic reset on series load (the reset is explicit/manual only).
- Remote/networked reset commands (e.g., via DICOM Softcopy Presentation State).

---

## Risks

| # | Risk | Likelihood | Impact | Mitigation |
|---|------|-----------|--------|------------|
| R-01 | DICOM dataset from legacy scanner has absent or zero WW/WC tags, causing fallback to be applied silently | Medium | Medium — unexpected image appearance if fallback differs from operator expectation | FR-04 (fallback) + FR-05 (confirmation message names the source); operators trained via IFU |
| R-02 | WW value read from tag is 0 or negative (malformed dataset) | Low | High — black/inverted image, missed diagnosis | FR-07 (explicit validation guard) |
| R-03 | Undo stack does not exist in current application; FR-06 requires new infrastructure | Medium | High — schedule/scope risk | Flag as open question OQ-02; escalate to architect before implementation |
| R-04 | Confirmation message obscures clinical image content in small-display configurations | Low | Low | Non-blocking status-bar placement preferred over overlay; UX review required |
| R-05 | Deployment on primary diagnostic workstation elevates safety class from B to C | Low | High — additional lifecycle obligations | OQ-01 tracks this; Class C work products required if confirmed |

---

## Open Questions

| # | Question | Owner | Blocking? |
|---|----------|-------|-----------|
| OQ-01 | Is this component deployed on a primary diagnostic workstation (IEC 62304 Class C territory) or a secondary review workstation (Class B)? This determines whether Class C lifecycle activities are required. | Product Owner / Regulatory | Yes — must be resolved before architecture ADR |
| OQ-02 | Does the application currently have an Undo/Redo stack for WW/WC display state? If not, FR-06 requires new infrastructure and the scope of this feature expands. | SW Architect | Yes — scope definition |
| OQ-03 | What is the application's existing DICOM tag abstraction layer? FR-05 and NFR-05 require all tag access to go through it. | Developer / Architect | No — can be resolved in design phase |
| OQ-04 | Which component currently owns the WW/WC viewport state? (ViewModel, ViewService, or image-rendering engine?) This determines where the reset command is implemented. | SW Architect | No — design-time question |
| OQ-05 | Is the keyboard shortcut Ctrl+Shift+W already bound to another action? Confirm or propose an alternative. | Product Owner | No |
