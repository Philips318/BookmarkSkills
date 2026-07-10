---
name: ct-coding-standards
description: Canonical CT project C# coding standards shared by all skills: naming conventions, core coding rules, events/delegates, and logging levels. Load this skill whenever writing or reviewing C# code in the CT project.
---

# CT Coding Standards

Shared reference for all CT C# code. Referenced by `csharp-development` and `csharp-code-review` skills.

## Naming Conventions

| Element | Convention | Example |
|---------|-----------|----------|
| Class / Struct | PascalCase | `CtDeviceController` |
| Public method | PascalCase | `GetPositionAsync()` |
| Private field | `_camelCase` | `_deviceChannel` |
| Property | PascalCase | `CurrentPosition` |
| Interface | `I` + PascalCase | `IDeviceChannel` |
| Constant | PascalCase | `MaxPositionMm` |
| Local variable | camelCase | `positionValue` |
| Async method | suffix `Async` | `GetPositionAsync()` |

- Include units in variable names: `positionInMm`, `timeoutInMs`, `angleInDeg`
- Prefix `bool` members and methods with `is`, `has`, or `can`: `isConnected`, `hasData`
- No single-character names except loop counters (`i`, `j`, `k`)
- No abbreviations — use full descriptive words
- File name must match class name (PascalCase)

## Core Coding Rules

- Use `var` for local variable declarations; exception: primitive types (`int`, `string`, `double`, etc.) — use the predefined type name
- No unused `using` statements
- No magic numbers — use named constants or configuration values
- Methods ≤ 20 lines; classes ≤ 200 lines
- Cyclomatic complexity ≤ 6 per method for new code
- No public mutable fields — use properties
- Mark classes `sealed` unless inheritance is explicitly designed
- Constructors do construction only — no business logic
- `CompareOrdinal` for string equality comparisons; `StringBuilder` when a string is modified repeatedly
- All UI strings from resource files — no hardcoded strings or paths
- Avoid code-behind in WPF; use MVVM strictly
- Build must produce zero warnings; warnings are errors
- No commented-out code; no `TODO`, `FIXME`, or `HACK` in code

## Events and Delegates

- All registered events must be unregistered when the subscriber is disposed
- Null-check before firing an event: `handler?.Invoke(this, args)`
- Event handlers must not be public — no external direct invocation
- If an event arrives on a non-UI thread and updates the UI, marshal via `Dispatcher.Invoke`
- No `Thread.Sleep()` for synchronisation — use events or semaphores

## Logging

- DEBUG: method entry/exit, internal state traces
- EVENT: user-initiated actions
- ERROR: recoverable errors — logged by the class that handles them
- CRITICAL: unrecoverable errors — logged once at the top level only
- Never log ePHI (patient data) or device calibration secrets at any level
- No log statements inside tight loops
