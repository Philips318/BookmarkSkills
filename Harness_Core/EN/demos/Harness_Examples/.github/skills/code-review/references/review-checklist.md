# Code Review Checklist

Detailed checklist for each of the eight review dimensions.

---

## 1. Change Scope

**Goal**: Understand what was changed and map the blast radius.

| Check | Details |
|-------|---------|
| List all changed files | Categorize by: source, test, config, build, resource |
| Identify file roles | Model / View / ViewModel / Service / Utility / Test / Config |
| Map dependencies | Which other modules consume or are consumed by the changed code? |
| Detect cross-project impact | Does the change affect shared libraries (e.g., `CT_SW_Common`, `DirectResultPipeline`, `MIPPP`)? |
| Detect mirrored files | Are there mirrored copies (per TICS rules) that also need updating? |
| Note new files | Any files added? Are they in the correct project/namespace/folder? |
| Note deleted files | Any files removed? Are all references cleaned up? |

---

## 2. Fix Completeness

**Goal**: For bug-fix commits, verify the root cause is actually resolved.

| Check | Details |
|-------|---------|
| Root cause identified | Is there a clear explanation of what caused the bug? |
| Root cause addressed | Does the code change directly fix the root cause, not just a symptom? |
| Edge cases covered | Does the fix handle boundary conditions, null inputs, concurrent access? |
| Error paths handled | Are exception/error paths properly handled after the fix? |
| No partial state | Does the fix leave the system in a consistent state in all scenarios? |
| Regression test | Is there a test (new or existing) that would catch a recurrence? |
| Related areas checked | Are similar patterns elsewhere in the codebase also affected? |

---

## 3. Feature Completeness

**Goal**: For feature commits, verify all requirements are implemented.

| Check | Details |
|-------|---------|
| Acceptance criteria met | Does the implementation satisfy all stated requirements? |
| UI/UX complete | Are all UI elements, bindings, and interactions wired up? |
| Data flow complete | Is the full data pipeline (input → processing → output) implemented? |
| Integration wired | Are all integration points (events, services, DI registrations) connected? |
| Error handling | Are user-facing error messages and fallback behaviors implemented? |
| Configuration | Are any new configuration items documented and defaulted? |
| Backward compatibility | Does the feature maintain backward compatibility where required? |

---

## 4. Regression Risk

**Goal**: Identify new risks introduced by the change.

| Check | Severity | Details |
|-------|----------|---------|
| Null safety | Critical | Any new nullable dereferences without null checks? |
| Thread safety | Critical | Any shared mutable state accessed without synchronization? |
| Resource leaks | Critical | Any new IDisposable objects not properly disposed? |
| Event subscription leaks | High | Any new event subscriptions without corresponding unsubscriptions? |
| API contract breakage | High | Any public/internal signature changes that break callers? |
| Behavioral side effects | High | Does the change alter behavior in unrelated code paths? |
| Performance regression | Medium | Any new O(n²) patterns, excessive allocations, or blocking calls on UI thread? |
| Configuration sensitivity | Medium | Any new hard-coded values that should be configurable? |
| Exception swallowing | Medium | Any new catch blocks that swallow exceptions without logging? |
| Magic numbers/strings | Low | Any new unexplained literal values? |

---

## 5. Test Impact

**Goal**: Map the change to affected features and recommend test focus.

| Check | Details |
|-------|---------|
| Directly affected features | List features whose code was directly modified |
| Indirectly affected features | List features that consume or depend on the modified code |
| Regression test scope | Which existing test suites should be rerun? |
| Manual test scenarios | What manual test scenarios are needed (especially for UI changes)? |
| Edge case test scenarios | What boundary/edge-case scenarios should be specifically tested? |
| Integration test needs | Are there cross-module integration scenarios to verify? |
| Performance test needs | Should performance/load testing be done for this change? |

**Output format**: A prioritized test plan with:
- P0 (must test before commit): Core scenarios directly affected
- P1 (should test before merge): Adjacent features and regression suite
- P2 (recommended): Broader integration and edge cases

---

## 6. Coding Standards

**Goal**: Verify compliance with TICS and CodeScene standards.

### TICS Checks (Philips C# Coding Standard 5.33)

| Rule Area | Check |
|-----------|-------|
| File header | Copyright header present (4@101) |
| XML docs | Public/internal types and members documented |
| Null safety | No unsafe nullable dereferences |
| Exception handling | No swallowed exceptions; sufficient logging context |
| Naming | Follows project naming conventions |
| Type structure | One top-level type per file; file named after type |
| Namespace | Follows existing project namespace pattern |
| Accessibility | Fields private by default; types internal by default |
| Disposal | IDisposable implemented when owning disposables |
| Events | Null-check before raising; paired subscribe/unsubscribe |

### CodeScene Checks

| Metric | Threshold | Check |
|--------|-----------|-------|
| Cyclomatic complexity | ≤ 9 per function | Any function exceeding limit? |
| Function length | ≤ 20 statements preferred | Any oversized functions? |
| Nesting depth | ≤ 2 levels | Any deeply nested blocks? |
| Primitive Obsession | ≤ 30% primitive params | Any functions with too many primitives? |
| String Heavy Arguments | ≤ 2 raw strings | Any functions with string overload? |
| Bumpy Road | ≤ 1 nested block per function | Any functions with multiple nested blocks? |
| Module mean complexity | ≤ 4.0 | File-level average complexity acceptable? |

---

## 7. Architecture Conformance

**Goal**: Verify the change respects the project's architectural rules.

See [architecture-rules.md](./architecture-rules.md) for the full rule set.

| Check | Details |
|-------|---------|
| Layer violations | Does the change introduce any upward or cross-layer dependencies? |
| Dependency direction | Do dependencies flow in the correct direction (abstraction → implementation)? |
| Circular dependencies | Does the change create any new circular references? |
| Separation of concerns | Is business logic kept out of UI/View layers? |
| DI/IoC compliance | Are dependencies injected, not resolved via Service Locator? |
| Interface segregation | Are new interfaces focused and not over-broad? |
| Shared library rules | Do changes to shared libraries maintain backward compatibility? |
| Project boundary | Are project-specific types kept within their project boundary? |

---

## 8. Commit Log

**Goal**: Generate a clear, conventional commit message.

See [commit-log-template.md](./commit-log-template.md) for the template and examples.

| Check | Details |
|-------|---------|
| Type prefix | Correct type (fix/feat/refactor/chore/docs/test/style) |
| Scope | Identifies the affected module/component |
| Subject line | ≤ 72 characters, imperative mood, no period |
| Body | Explains what and why (not how) |
| Footer | References ticket/issue ID if applicable |
| Breaking changes | BREAKING CHANGE footer if API changed |
