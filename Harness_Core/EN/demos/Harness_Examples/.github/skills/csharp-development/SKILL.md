---
name: csharp-development
description: Writing new C# production code: naming conventions, XML documentation, async/await patterns, layered architecture rules, and interface-first design. Use when implementing features or classes in the Src/ directory.
---

# C# Development Skill

> **Also load `ct-coding-standards` skill** — it defines naming conventions, core coding rules, events/delegates, and logging levels that apply to all CT C# code.

## Naming Conventions

> See `ct-coding-standards` skill.

## XML Documentation

All public members must have XML doc comments:

```csharp
/// <summary>
/// Returns the current device measurement value in millimetres.
/// </summary>
/// <param name="token">Cancellation token.</param>
/// <returns>Position in mm, or <c>null</c> if device is not connected.</returns>
/// <exception cref="DeviceCommunicationException">Thrown when the device returns a malformed response.</exception>
public async Task<double?> GetPositionAsync(CancellationToken token = default) { ... }
```

## Coding Rules

> Base rules in `ct-coding-standards` skill. Additional implementation patterns:

- Implement `IDisposable` for classes owning unmanaged resources; use `using` declarations for consumers
- No `catch (Exception)` without either rethrowing or logging with full context

## Async Patterns

- Async methods return `Task` or `Task<T>` — never `async void` (except event handlers)
- Always accept `CancellationToken` in public async methods and propagate it
- Never use `.Result` or `.Wait()` — use `await` (deadlock risk on STA threads, e.g. WPF dispatcher)
- Do not `await` in constructors — use factory methods or lazy initialisation

## Events and Delegates

> See `ct-coding-standards` skill.

## Logging

> See `ct-coding-standards` skill.

## Architecture Rules (Project-Specific)

- Production classes implement interfaces from `ExtInf/` — never bypass them
- No `new` on service dependencies inside class bodies — use constructor injection
- New classes in `Src/` must reside in the namespace matching their folder path
- Do not reference test assemblies from production code
- Dependencies flow downward only: UI → Application → Domain → Infrastructure
- Cross-repository dependencies must reference `{repo}Inf.pkg` Interface NuGet only — never `{repo}Impl.pkg`
- All dependency assemblies are referenced from `Dependencies/Ref/` — never from Artifactory or local NuGet cache directly
- Unit tests co-locate with their component in `Src/{ProjectDir}/Test/`
