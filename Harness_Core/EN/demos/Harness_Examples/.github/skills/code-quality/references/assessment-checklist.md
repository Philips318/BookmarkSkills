# Assessment Checklist

Detailed checklist for each of the 8 quality dimensions.

---

## D1. CodeScene Code Health

**Weight**: 15%

### Function-Level Checks

| Metric | Threshold | Score Impact |
|--------|-----------|-------------|
| Cyclomatic Complexity | ≤ 9 | -1 per function exceeding |
| Nesting Depth | ≤ 2 | -1 per function exceeding |
| Function Length | ≤ 50 lines | -0.5 per function exceeding |
| Primitive Obsession | ≤ 30% params | -0.5 per function exceeding |
| String Heavy Args | ≤ 39% params | -0.5 per function exceeding |
| Bumpy Road | ≤ 1 block/func | -0.5 per function exceeding |
| Brain Method | CC+size+nesting+coupling | -2 per occurrence |

### Module-Level Checks

| Metric | Threshold | Score Impact |
|--------|-----------|-------------|
| Mean CC | ≤ 4.0 | -1 per 0.5 over threshold |
| File LoC | ≤ 500 | -0.5 per file exceeding |
| LCOM4 (Low Cohesion) | Implementation-specific | -1 per flagged file |
| God Class | Large + Brain Methods | -2 per occurrence |

### Scoring

- Count violations across all files → compute violation density (violations / total functions)
- Density 0% = 10, ≤5% = 8, ≤15% = 6, ≤30% = 4, ≤50% = 2, >50% = 1

---

## D2. TICS Compliance

**Weight**: 15%

| Rule Area | Check | Score Impact |
|-----------|-------|-------------|
| File Header (4@101) | Copyright present | -0.5 per missing |
| XML Documentation | Public/internal types + members documented | -0.3 per missing |
| Null Safety | No unsafe dereferences | -1 per occurrence |
| Exception Handling | No swallowed exceptions | -1 per occurrence |
| Naming Conventions | Consistent naming | -0.3 per violation |
| Type Structure | One type per file | -0.3 per violation |
| Namespace Consistency | Follow project pattern | -0.3 per violation |
| Access Modifiers | Fields private by default | -0.3 per violation |
| IDisposable | Implemented when owning disposables | -1 per missing |
| Event Pairing | Subscribe/unsubscribe paired | -1 per missing |
| Floating Point | No == comparison | -0.5 per occurrence |
| Abstract Constructors | Protected constructor present | -0.3 per missing |

### Scoring

- Count weighted violations → compute compliance rate
- 100% = 10, ≥95% = 8, ≥85% = 6, ≥70% = 4, ≥50% = 2, <50% = 1

---

## D3. Test Quality

**Weight**: 15%

| Metric | Target | How to Assess |
|--------|--------|---------------|
| Line Coverage | ≥ 80% | Check coverage reports or estimate from test project existence |
| Branch Coverage | ≥ 70% | Estimate from test complexity |
| Test:Production Ratio | 1:1 to 1:3 | Count test files vs production files |
| Test File Existence | Every .cs has test | Map production → test files |
| AAA Pattern | Arrange/Act/Assert structure | Spot-check test methods |
| Test Naming | Descriptive names | Check test method naming convention |
| Edge Case Coverage | Null, empty, boundary | Check for parameterized/boundary tests |
| Assertion Density | ≥ 1 assert per test | Spot-check test methods |

### Scoring

- If no test project exists → score 0
- If test project exists but low ratio → score 2-4
- If test project exists with good ratio → score based on coverage estimate
- Full CI coverage data available → use actual percentages

---

## D4. Architecture Conformance

**Weight**: 12%

| Check | How to Assess | Score Impact |
|-------|---------------|-------------|
| Layer Violations | Scan `using` statements for upward deps | -2 per violation |
| Circular Dependencies | Check project references for cycles | -2 per cycle |
| Dependency on Abstractions | Count concrete vs interface deps | -1 if ratio < 30% |
| Service Locator Usage | Search for `Resolve<>`, `GetService` | -1 per non-root usage |
| UI Logic in Service | Business logic in code-behind | -1.5 per occurrence |
| Framework Types in Domain | Dispatcher/Brush in domain | -1 per occurrence |
| Interface Segregation | Check interface member count | -0.5 if > 10 members |
| Project Boundary | Specific types in shared libs | -1 per leaking type |

### Scoring

- Count weighted violations → normalize by module size
- 0 violations = 10, scale down based on density

---

## D5. Maintainability

**Weight**: 12%

| Metric | Target | How to Assess |
|--------|--------|---------------|
| Duplication Rate | ≤ 3% | Detect similar code blocks (≥ 6 lines) |
| File Size Distribution | 90% files ≤ 500 LoC | Count files by size bucket |
| Complexity Distribution | 90% functions CC ≤ 9 | Count functions by CC bucket |
| TODO/HACK Count | Track and trending down | Search for TODO, HACK, FIXME, WORKAROUND |
| Dead Code | No unreachable code | Look for unused methods/classes |
| Magic Numbers | Named constants preferred | Count raw numeric literals |
| Readability | Consistent formatting | Spot-check indentation, spacing |
| Fan-out | ≤ 20 dependencies per class | Count `using` + injected deps |

### Scoring

- Composite: weight each sub-metric equally
- Best-in-class module = 10, typical module = 6-7

---

## D6. Security

**Weight**: 10%

| Check | How to Assess | Severity |
|-------|---------------|----------|
| Input Validation | Check public API entry points | Critical |
| Hardcoded Credentials | Search for password, connectionString, apiKey | Critical |
| SQL Injection | Parameterized queries vs string concat | Critical |
| Path Traversal | File path construction safety | High |
| Sensitive Data Logging | PII in log statements | High |
| Dependency Vulnerabilities | Known CVEs in NuGet packages | High |
| Deserialization Safety | Untrusted data deserialization | Medium |
| Encryption Usage | Proper algorithm usage | Medium |
| Error Information Leakage | Stack traces exposed to user | Low |

### Scoring

- 0 Critical = base 8, each Critical → -3, each High → -1.5, each Medium → -0.5
- Cap at 0 minimum

---

## D7. Performance Patterns

**Weight**: 10%

| Check | How to Assess | Score Impact |
|-------|---------------|-------------|
| O(n²) in hot paths | Nested loops over collections | -2 per occurrence |
| Large Object Allocation | `new byte[large]` in loops | -1 per occurrence |
| UI Thread Blocking | Sync IO/compute on dispatcher | -2 per occurrence |
| Resource Leak | IDisposable not disposed | -1.5 per occurrence |
| Event Leak | Subscribe without unsubscribe | -1 per occurrence |
| String Concatenation | In loops (use StringBuilder) | -0.5 per occurrence |
| LINQ in Hot Path | Allocating LINQ in tight loops | -0.5 per occurrence |
| Async/Await Misuse | .Result, .Wait() blocking | -1 per occurrence |

### Scoring

- Count weighted violations → normalize by file count
- 0 violations = 10, scale down

---

## D8. Defect Density & Risk

**Weight**: 11%

| Metric | How to Assess | Score Impact |
|--------|---------------|-------------|
| Complexity Hotspots | Files with CC > 15 AND LoC > 300 | -2 per hotspot |
| Change Frequency | Files modified often (git log) | Risk multiplier |
| Bug-Fix Frequency | Commits with "fix" in message | Risk indicator |
| Error-Prone Patterns | Nested try/catch, deep nesting | -1 per pattern |
| Global Mutable State | Static mutable fields | -1.5 per occurrence |
| Implicit Dependencies | Hidden coupling via statics/singletons | -1 per occurrence |
| Code Age vs Churn | Old code frequently modified | Risk indicator |

### Scoring

- Combine hotspot count + pattern violations → risk score
- Enrich with git history if available (git log data is optional enhancement)

---

## Overall Score Calculation

```
Overall = D1×0.15 + D2×0.15 + D3×0.15 + D4×0.12 + D5×0.12 
        + D6×0.10 + D7×0.10 + D8×0.11
```

Round to one decimal place. Map to grade:

| Score | Grade | Action |
|-------|-------|--------|
| 9–10  | A     | Maintain — no action needed |
| 7–8.9 | B     | Monitor — address in normal sprint |
| 5–6.9 | C     | Plan — schedule improvement sprint |
| 3–4.9 | D     | Act — prioritize quality improvement |
| 0–2.9 | F     | Escalate — stop feature work, fix quality |
