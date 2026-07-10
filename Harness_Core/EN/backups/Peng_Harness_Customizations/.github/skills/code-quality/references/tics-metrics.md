# TICS Metrics Reference

Quick reference for TICS / Philips C# Coding Standard metrics used in the quality assessment.

## Priority Levels for Assessment

| Priority | Rules | Assessment Approach |
|----------|-------|---------------------|
| Must-check (L1-5) | Copyright, null safety, exception handling, disposal, type structure | Count violations = direct score impact |
| Should-check (L6-7) | Naming, documentation, access modifiers | Count violations = moderate impact |
| Best-effort (L8-10) | Style, advanced patterns | Note but don't penalize heavily |

## Key Checkpoints

### Copyright & Documentation
| Check | How to Detect | Impact |
|-------|---------------|--------|
| Copyright header missing | First line doesn't match `/*Copyright` pattern | -0.5/file |
| Public type without `<summary>` | `public class/interface` without preceding `///` | -0.3/type |
| Public method without `<summary>` | `public` method without preceding `///` | -0.3/method |

### Safety
| Check | How to Detect | Impact |
|-------|---------------|--------|
| Potential null dereference | Access member after assignment that could be null | -1.0/occurrence |
| Swallowed exception | `catch { }` or catch without log/throw | -1.0/occurrence |
| Missing IDisposable | Class holds IDisposable field without implementing IDisposable | -1.0/class |
| Event not unsubscribed | `+=` without corresponding `-=` in Dispose/cleanup | -1.0/occurrence |

### Structure
| Check | How to Detect | Impact |
|-------|---------------|--------|
| Multiple types per file | More than one `class/struct/interface/enum` per file | -0.3/extra |
| Inconsistent namespace | Namespace doesn't match folder path | -0.3/file |
| Field not private | `public` or `internal` field (not const/readonly) | -0.3/field |
| `new` hiding member | `new` keyword on method/property | -0.5/occurrence |

### Advanced
| Check | How to Detect | Impact |
|-------|---------------|--------|
| Floating-point equality | `==` or `!=` with float/double | -0.5/occurrence |
| Lock on `this`/Type/string | `lock(this)`, `lock(typeof(...))`, `lock("...")` | -1.0/occurrence |
| Loop variable modified | `for` variable assigned inside loop body | -0.5/occurrence |
| Missing `GetHashCode` | `Equals` overridden without `GetHashCode` | -0.5/class |

## Compliance Rate Calculation

```
total_checkpoints = files × per_file_checks + types × per_type_checks + methods × per_method_checks
weighted_violations = sum(violation_count × impact_weight)
compliance_rate = (total_checkpoints - weighted_violations) / total_checkpoints × 100%
```

Map to D2 score using the table in [scoring-rubric.md](./scoring-rubric.md).
