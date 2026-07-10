# Architecture Rules

Rules for verifying architectural conformance during code review.

---

## Layering Model

The typical project layering (top → bottom):

```
┌─────────────────────────┐
│   UI / Views / XAML      │  ← Presentation layer
├─────────────────────────┤
│   ViewModels / Commands  │  ← Presentation logic
├─────────────────────────┤
│   Services / Workflows   │  ← Application logic
├─────────────────────────┤
│   Domain / Models        │  ← Business domain
├─────────────────────────┤
│   Infrastructure / IO    │  ← Data access, file IO, network
├─────────────────────────┤
│   Common / Utilities     │  ← Cross-cutting shared code
└─────────────────────────┘
```

**Rule**: Dependencies must flow downward. Upper layers may depend on lower layers, never the reverse.

---

## Dependency Rules

### DR-1: No Upward Dependencies
A lower-layer module must not reference a higher-layer module. For example, a Service must not reference a ViewModel.

### DR-2: No Circular Dependencies
Project A → Project B → Project A is forbidden. If detected, introduce an abstraction (interface) in a shared lower layer.

### DR-3: Depend on Abstractions
High-level modules should depend on interfaces or abstract classes, not concrete implementations. This is especially important for cross-project boundaries.

### DR-4: Shared Library Stability
Shared libraries (`CT_SW_Common`, `DirectResultPipeline`, `MIPPP`) must maintain backward-compatible public APIs. Breaking changes require explicit coordination.

### DR-5: Project Boundary Enforcement
Types specific to one application (e.g., `iWorkflow`, `ThreeDSurview`) must not leak into shared libraries.

---

## Design Rules

### DES-1: Separation of Concerns
- No business logic in View/XAML code-behind.
- No UI framework types (e.g., `Dispatcher`, `Brush`, `Visibility`) in Service or Domain layers.
- No file I/O or network calls in ViewModel constructors.

### DES-2: Dependency Injection
- Prefer constructor injection over Service Locator / `Resolve<T>()` patterns.
- Register new services in the composition root, not scattered through the codebase.
- Avoid injecting the DI container itself.

### DES-3: Interface Segregation
- New interfaces should be small and focused on a single role.
- Do not add methods to existing interfaces if they serve a different purpose — create a new interface.

### DES-4: Event and Messaging
- Prefer typed events or a message bus over tight coupling between modules.
- Always pair event subscribe with unsubscribe.
- Do not use static events for cross-module communication unless no alternative exists.

### DES-5: Configuration Management
- No hard-coded connection strings, paths, or magic values.
- Configuration should flow from config files or DI, not be embedded in business logic.

### DES-6: Error Handling Strategy
- Domain exceptions should be caught and translated at layer boundaries.
- UI layer should present user-friendly messages; service layer should log technical details.
- Do not let infrastructure exceptions (e.g., `SqlException`) propagate to UI.

---

## Cross-Project Rules

### CP-1: Mirrored Files
When behavior exists in mirrored folders (e.g., `_Dev` / `_Plot` pairs), update both copies in the same commit.

### CP-2: Shared DLL Output
Projects that publish DLLs to shared output folders (e.g., `MIPPP\Bin`) must be rebuilt in the correct configuration before dependent projects are validated.

### CP-3: NuGet Package Consistency
When updating a NuGet package in one project, check whether the same package is used in sibling projects and align versions to avoid runtime conflicts.

---

## Review Verdicts

| Verdict | Criteria |
|---------|----------|
| **Pass** | No architecture rule violated |
| **Warning** | Minor deviation that does not break the architecture (e.g., a convenience shortcut) |
| **Fail** | Clear violation of layering, dependency direction, or project boundary rules |
