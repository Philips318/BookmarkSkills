---
name: csharp-code-review
description: Reviewing C# code for correctness, security (OWASP Top 10), naming, complexity, thread safety, error handling, and IEC 62304 compliance. Use when evaluating a code submission or performing a white-box code review.
---

# C# Code Review Skill

> **Also load `ct-coding-standards` skill** — it defines the canonical naming, coding, events/delegates, and logging rules that the checklists below enforce.

## OWASP Top 10 (C# Context)

| Risk | What to Check |
|------|--------------|
| Injection | No string concatenation in SQL/command building — use parameterised queries or structured commands |
| Broken auth | No hardcoded credentials; secrets from configuration or secret store, never source code |
| Sensitive data | No PII or device calibration data logged at DEBUG level; no sensitive data in exception messages |
| XML/JSON parsing | Use validated deserialisation; never `XmlDocument.LoadXml(userInput)` without validation |
| Security misconfiguration | No debug endpoints, no `CORS *`, no verbose error detail exposed to callers |
| Vulnerable components | NuGet packages at current minor version with no known CVEs |
| Integrity failures | No unsigned assembly loading from user-controlled paths |
| Logging failures | Errors logged with full context; no `catch` blocks that silently swallow exceptions |
| SSRF | No user-controlled strings passed to `HttpClient` base URLs |

## Structure Review

- [ ] Classes have single responsibility — the class name describes exactly what it does
- [ ] No public methods that are not declared on an `ExtInf/` interface
- [ ] Methods are ≤ 20 lines; extract helpers if longer
- [ ] Cyclomatic complexity ≤ 6; flag methods with deep nesting (> 3 levels) or many branches
- [ ] No dead code, unreachable branches, or methods never called
- [ ] Build produces zero warnings — no `#pragma warning disable` without documented justification

## C# Idioms

- [ ] `IDisposable` implemented correctly where unmanaged resources are owned
- [ ] `using` declarations (C# 8+) or `using` statements for all `IDisposable` consumers
- [ ] No `async void` except event handlers; all async methods return `Task` or `Task<T>`
- [ ] `CancellationToken` accepted and propagated in all public async methods
- [ ] No `.Result` or `.Wait()` on tasks — deadlock risk on STA/dispatcher threads
- [ ] Null checks via `ArgumentNullException.ThrowIfNull()` on public method parameters
- [ ] No mutable public fields — use properties
- [ ] Thread safety: shared mutable state protected by `lock`, `SemaphoreSlim`, or `Interlocked`

## Documentation and Style

- [ ] All public and protected members have XML doc comments (`<summary>`, `<param>`, `<returns>`, `<exception>`)
- [ ] No commented-out code blocks
- [ ] No `TODO`, `FIXME`, or `HACK` in code
- [ ] No magic numbers — use named constants or configuration
- [ ] No single-letter variables except loop counters
- [ ] No abbreviations in names
- [ ] No unused `using` statements
- [ ] `var` used for non-primitive locals; primitive types (`int`, `string`, etc.) use predefined names
- [ ] `CompareOrdinal` used for string equality; `StringBuilder` used where strings are mutated repeatedly
- [ ] Variable names include units where applicable: `positionInMm`, `timeoutInMs`

## Architecture (Project-Specific)

- [ ] Production classes implement `ExtInf/` interfaces — none bypassed
- [ ] No `new` on service dependencies inside class bodies — constructor injection only
- [ ] No references from `Src/` production code to test assemblies
- [ ] Layering respected: UI → Application → Domain → Infrastructure (no upward references)
- [ ] New public types documented with their architectural layer and safety class (IEC 62304)

## Events and Threading

> Rules defined in `ct-coding-standards` skill.

- [ ] All registered events are unregistered on dispose
- [ ] Null-check before firing events: `handler?.Invoke(this, args)`
- [ ] No `Thread.Sleep()` for synchronisation
- [ ] Shared mutable state protected by `lock`, `SemaphoreSlim`, or `Interlocked`

## Logging

> Rules defined in `ct-coding-standards` skill.

- [ ] DEBUG for trace/diagnostics, EVENT for user actions, ERROR for recoverable, CRITICAL for unrecoverable at top level only
- [ ] No ePHI or device calibration secrets logged at any level
- [ ] No log statements inside tight loops
- [ ] Errors logged with full context — no silent swallowing in `catch` blocks

## Quality Gate Reports

Before issuing a review verdict, confirm the automated quality gates passed:

- [ ] **TICS**: TQI ≥ 8.0 on all measured characteristics — no characteristic in red
- [ ] **Coverity**: zero new High defects; any new Medium defects have a justification comment referencing the finding ID
- [ ] **ReSharper**: review the published report — flag any errors or warnings that the developer should address even though they are non-blocking
- [ ] No quality gate was suppressed or skipped without a documented waiver in the PR description

If a gate result is unavailable (pipeline not run, tool failure), the review verdict is **BLOCKED** until gates are confirmed.
