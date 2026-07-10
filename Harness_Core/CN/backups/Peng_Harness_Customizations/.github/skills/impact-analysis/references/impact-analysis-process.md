# Impact Analysis — Process Details

`impact-analysis` skill 的 companion reference。Generic、team-portable。

## The 4-Step Process (expanded)

### Step 1 — Identify Target Scope

搜索 codebase 前，先以书面回答：

| 问题 | 重要性 |
|----------|---------------|
| 哪些 contract 发生变化？（signature、return type、exception set、behavior） | 决定该变更是 binary-breaking 还是仅行为变化 |
| visibility 是什么？（public / internal / private） | `public` API 变更具有外部影响范围 |
| data shape 是否变化？（DTO、persisted schema、DICOM tag set、file format） | 持久化/序列化变化会破坏旧数据 |
| timing/threading 是否变化？（sync→async、新 lock、新 thread） | thread-safety 回归最难发现 |
| memory ownership 是否变化？（谁分配 / 谁释放） | 会跨边界导致泄漏或重复释放 |

在 report 的 `Change Summary` section 中记录答案。

### Step 2 — Dependency Discovery

运行下面 **all six** searches，即使你认为只有一个适用。漏掉某类是 analysis failure 最常见原因。

```
1. Direct callers       : grep for ClassName / MethodName
2. Interface implementors : grep for ': ITargetInterface'
3. DLL consumers        : grep HintPath in *.csproj
4. Event subscribers    : grep '+= .*TargetEvent'
5. Configuration refs   : grep in *.config / *.json / appsettings*
6. Reflection / DI      : grep for typeof(Target) / Resolve<Target> / nameof(Target)
```

用 file path + line number tabulate 每个 hit — 不要只总结。

### Step 3 — Categorize Impact

对每个 hit：

| 等级 | Definition | 操作 |
|-------|-----------|--------|
| **Direct** | Code stops compiling or fails at runtime without change | Must modify; list specific change |
| **Indirect** | Compiles, but observable behavior may change | Add verification test; document expectation |
| **Potential** | No direct dependency, but shared infrastructure / data path | Review during PR; add monitoring if production |
| **None** | Hit is unrelated (false positive from search) | Drop from list with one-line reason |

### Step 4 — Risk Assessment

每个 dimension 评分 Low / Med / High；final risk = **highest** dimension（not average）。

| Dimension | High triggers |
|-----------|---------------|
| Scope breadth | ≥ 6 modules touched |
| Cross-assembly | Cross-solution boundary |
| 患者数据路径 | Direct read/write of PHI or diagnostic data |
| UI interaction | Changes input handling, not just display |
| Shared/published artifact | Writes to a binary that other solutions consume |
| Concurrency model | Changes locking, threading, or async boundaries |
| Persistence/serialization | Changes on-disk or wire format |

## Output Quality Checklist

声明 impact analysis complete 前：

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
