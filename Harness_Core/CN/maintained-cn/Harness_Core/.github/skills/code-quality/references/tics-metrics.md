# TICS Metrics Reference

质量评估中使用的 TICS / Philips C# Coding Standard metrics 快速 reference。

## Priority Levels for Assessment

| 优先级 | Rules | Assessment Approach |
|----------|-------|---------------------|
| Must-check (L1-5) | Copyright, null safety, exception handling, disposal, type structure | 统计 violations = 直接影响 score |
| Should-check (L6-7) | Naming, documentation, access modifiers | 统计 violations = 中等影响 |
| Best-effort (L8-10) | Style, advanced patterns | 记录但不重罚 |

## Key Checkpoints

### Copyright & Documentation
| 检查项 | 如何检测 | 影响 |
|-------|---------------|--------|
| Copyright header missing | First line doesn't match `/*Copyright` pattern | -0.5/file |
| Public type without `<summary>` | `public class/interface` without preceding `///` | -0.3/type |
| Public method without `<summary>` | `public` method without preceding `///` | -0.3/method |

### Safety
| 检查项 | 如何检测 | 影响 |
|-------|---------------|--------|
| Potential null dereference | Access member after assignment that could be null | -1.0/occurrence |
| Swallowed exception | `catch { }` or catch without log/throw | -1.0/occurrence |
| Missing IDisposable | Class holds IDisposable field without implementing IDisposable | -1.0/class |
| Event not unsubscribed | `+=` without corresponding `-=` in Dispose/cleanup | -1.0/occurrence |

### Structure
| 检查项 | 如何检测 | 影响 |
|-------|---------------|--------|
| Multiple types per file | More than one `class/struct/interface/enum` per file | -0.3/extra |
| Inconsistent namespace | Namespace doesn't match folder path | -0.3/file |
| Field not private | `public` or `internal` field (not const/readonly) | -0.3/field |
| `new` hiding member | `new` keyword on method/property | -0.5/occurrence |

### Advanced
| 检查项 | 如何检测 | 影响 |
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

使用 [scoring-rubric.md](./scoring-rubric.md) 中的表，将 compliance rate 映射到 D2 score。
