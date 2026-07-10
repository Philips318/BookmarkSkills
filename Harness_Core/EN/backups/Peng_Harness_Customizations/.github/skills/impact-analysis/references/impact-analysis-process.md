# Impact Analysis — Process Details

Companion reference for the `impact-analysis` skill. Generic, team-portable.

## The 4-Step Process (expanded)

### Step 1 — Identify Target Scope

Before searching the codebase, answer in writing:

| Question | Why it matters |
|----------|---------------|
| What contract changes? (signature, return type, exception set, behavior) | Determines whether change is binary-breaking or behavior-only |
| What is the visibility? (public / internal / private) | `public` API change has external blast radius |
| Does data shape change? (DTO, persisted schema, DICOM tag set, file format) | Persistence/serialization changes break old data |
| Does timing/threading change? (sync→async, new lock, new thread) | Thread-safety regressions are hardest to find |
| Does memory ownership change? (who allocates / who disposes) | Causes leaks or double-disposal across boundaries |

Record answers in the report's `Change Summary` section.

### Step 2 — Dependency Discovery

Run **all six** searches below, even if you think only one applies. Missing a category is the most common analysis failure.

```
1. Direct callers       : grep for ClassName / MethodName
2. Interface implementors : grep for ': ITargetInterface'
3. DLL consumers        : grep HintPath in *.csproj
4. Event subscribers    : grep '+= .*TargetEvent'
5. Configuration refs   : grep in *.config / *.json / appsettings*
6. Reflection / DI      : grep for typeof(Target) / Resolve<Target> / nameof(Target)
```

Tabulate each hit with file path + line number — do not summarize.

### Step 3 — Categorize Impact

For each hit:

| Level | Definition | Action |
|-------|-----------|--------|
| **Direct** | Code stops compiling or fails at runtime without change | Must modify; list specific change |
| **Indirect** | Compiles, but observable behavior may change | Add verification test; document expectation |
| **Potential** | No direct dependency, but shared infrastructure / data path | Review during PR; add monitoring if production |
| **None** | Hit is unrelated (false positive from search) | Drop from list with one-line reason |

### Step 4 — Risk Assessment

Score each dimension Low / Med / High; final risk = **highest** dimension (not average).

| Dimension | High triggers |
|-----------|---------------|
| Scope breadth | ≥ 6 modules touched |
| Cross-assembly | Cross-solution boundary |
| Patient data path | Direct read/write of PHI or diagnostic data |
| UI interaction | Changes input handling, not just display |
| Shared/published artifact | Writes to a binary that other solutions consume |
| Concurrency model | Changes locking, threading, or async boundaries |
| Persistence/serialization | Changes on-disk or wire format |

## Output Quality Checklist

Before declaring the impact analysis complete:

- [ ] Every "Direct" impact has a specific change description (not "needs update")
- [ ] Every cross-assembly impact flags rebuild order
- [ ] Patient data path impacts are escalated regardless of scope
- [ ] Overall Risk uses the **max** dimension, not average
- [ ] At least one mitigation per Med/High risk dimension
- [ ] If the requirement changes after analysis, the analysis is updated (not appended)

## Common Mistakes

1. **Only searching for class name** — misses interface, event, reflection usage
2. **Trusting the IDE "Find References"** — does not cross solution boundaries or string-based lookups
3. **Averaging risk scores** — one High dimension is enough; do not dilute with Lows
4. **Confusing "no compile error" with "no impact"** — behavioral change is still impact
5. **Skipping the "None" dropouts** — silent drops look like missed hits to reviewers
