# Assessment Checklist

每个 8 quality dimensions 的详细 checklist。

---

## D1. CodeScene Code Health

**Weight**: 15%

### Function-Level Checks

| 指标 | 阈值 | 分数影响 |
|--------|-----------|-------------|
| 圈复杂度 | ≤ 9 | 每个超限 function -1 |
| 嵌套深度 | ≤ 2 | 每个超限 function -1 |
| Function Length | ≤ 50 lines | 每个超限 function -0.5 |
| 基本类型偏执 | ≤ 30% params | 每个超限 function -0.5 |
| 字符串参数过多 | ≤ 39% params | 每个超限 function -0.5 |
| ?????Bumpy Road? | ≤ 1 block/func | 每个超限 function -0.5 |
| ?????Brain Method? | CC+size+nesting+coupling | 每次出现 -2 |

### Module-Level Checks

| 指标 | 阈值 | 分数影响 |
|--------|-----------|-------------|
| 平均圈复杂度 | ≤ 4.0 | 每超过 threshold 0.5 扣 1 |
| 文件代码行数 | ≤ 500 | 每个超限 file -0.5 |
| LCOM4 (Low Cohesion) | Implementation-specific | 每个 flagged file -1 |
| God Class | Large + Brain Methods | 每次出现 -2 |

### Scoring

- 统计所有 files 的 violations → 计算 violation density（violations / total functions）
- Density 0% = 10, ≤5% = 8, ≤15% = 6, ≤30% = 4, ≤50% = 2, >50% = 1

---

## D2. TICS Compliance

**Weight**: 15%

| 规则领域 | 检查项 | 分数影响 |
|-----------|-------|-------------|
| File Header (4@101) | Copyright present | 每个 missing -0.5 |
| XML Documentation | Public/internal types + members documented | 每个 missing -0.3 |
| Null Safety | No unsafe dereferences | 每次 occurrence -1 |
| Exception Handling | No swallowed exceptions | 每次 occurrence -1 |
| Naming Conventions | Consistent naming | 每个 violation -0.3 |
| Type Structure | One type per file | 每个 violation -0.3 |
| Namespace Consistency | Follow project pattern | 每个 violation -0.3 |
| Access Modifiers | Fields private by default | 每个 violation -0.3 |
| IDisposable | Implemented when owning disposables | 每个 missing -1 |
| Event Pairing | Subscribe/unsubscribe paired | 每个 missing -1 |
| Floating Point | No == comparison | 每次 occurrence -0.5 |
| Abstract Constructors | Protected constructor present | 每个 missing -0.3 |

### Scoring

- 统计 weighted violations → 计算 compliance rate
- 100% = 10, ≥95% = 8, ≥85% = 6, ≥70% = 4, ≥50% = 2, <50% = 1

---

## D3. Test Quality

**Weight**: 15%

| 指标 | 目标 | 如何评估 |
|--------|--------|---------------|
| Line Coverage | ≥ 80% | 检查 coverage reports，或根据 test project existence 估算 |
| Branch Coverage | ≥ 70% | 根据 test complexity 估算 |
| Test:Production Ratio | 1:1 to 1:3 | 统计 test files vs production files |
| Test File Existence | Every .cs has test | 映射 production → test files |
| AAA Pattern | Arrange/Act/Assert structure | 抽查 test methods |
| Test Naming | Descriptive names | 检查 test method naming convention |
| Edge Case Coverage | Null, empty, boundary | 检查 parameterized/boundary tests |
| Assertion Density | ≥ 1 assert per test | 抽查 test methods |

### Scoring

- 如果 no test project exists → score 0
- 如果 test project exists 但 ratio 低 → score 2-4
- 如果 test project exists 且 ratio 良好 → 基于 coverage estimate 评分
- 如果有完整 CI coverage data → 使用 actual percentages

---

## D4. Architecture Conformance

**Weight**: 12%

| 检查项 | 如何评估 | 分数影响 |
|-------|---------------|-------------|
| Layer Violations | 扫描 `using` statements 中的 upward deps | 每个 violation -2 |
| Circular Dependencies | 检查 project references 是否有 cycles | 每个 cycle -2 |
| Dependency on Abstractions | 统计 concrete vs interface deps | ratio < 30% 时 -1 |
| Service Locator Usage | 搜索 `Resolve<>`, `GetService` | 每个 non-root usage -1 |
| UI Logic in Service | Business logic in code-behind | 每次 occurrence -1.5 |
| Framework Types in Domain | Dispatcher/Brush in domain | 每次 occurrence -1 |
| Interface Segregation | 检查 interface member count | > 10 members 时 -0.5 |
| Project Boundary | shared libs 中出现 specific types | 每个 leaking type -1 |

### Scoring

- 统计 weighted violations → 按 module size normalize
- 0 violations = 10，根据 density 下调

---

## D5. Maintainability

**Weight**: 12%

| 指标 | 目标 | 如何评估 |
|--------|--------|---------------|
| Duplication Rate | ≤ 3% | 检测相似 code blocks（≥ 6 lines） |
| File Size Distribution | 90% files ≤ 500 LoC | 按 size bucket 统计 files |
| Complexity Distribution | 90% functions CC ≤ 9 | 按 CC bucket 统计 functions |
| TODO/HACK Count | Track and trending down | 搜索 TODO、HACK、FIXME、WORKAROUND |
| Dead Code | No unreachable code | 查找 unused methods/classes |
| Magic Numbers | Named constants preferred | 统计 raw numeric literals |
| Readability | Consistent formatting | 抽查 indentation、spacing |
| 扇出 | ≤ 20 dependencies per class | 统计 `using` + injected deps |

### Scoring

- Composite：每个 sub-metric equal weight
- Best-in-class module = 10，typical module = 6-7

---

## D6. Security

**Weight**: 10%

| 检查项 | 如何评估 | 严重度 |
|-------|---------------|----------|
| Input Validation | 检查 public API entry points | 关键 |
| Hardcoded Credentials | 搜索 password、connectionString、apiKey | 关键 |
| SQL Injection | Parameterized queries vs string concat | 关键 |
| Path Traversal | File path construction safety | 高 |
| Sensitive Data Logging | log statements 中的 PII | 高 |
| Dependency Vulnerabilities | NuGet packages 中 known CVEs | 高 |
| Deserialization Safety | Untrusted data deserialization | 中 |
| Encryption Usage | Proper algorithm usage | 中 |
| Error Information Leakage | Stack traces exposed to user | 低 |

### Scoring

- 0 Critical = base 8，每个 Critical → -3，每个 High → -1.5，每个 Medium → -0.5
- 最低 cap 为 0

---

## D7. Performance Patterns

**Weight**: 10%

| 检查项 | 如何评估 | 分数影响 |
|-------|---------------|-------------|
| O(n²) in hot paths | Nested loops over collections | 每次 occurrence -2 |
| Large Object Allocation | `new byte[large]` in loops | 每次 occurrence -1 |
| UI Thread Blocking | Sync IO/compute on dispatcher | 每次 occurrence -2 |
| Resource Leak | IDisposable not disposed | 每次 occurrence -1.5 |
| Event Leak | Subscribe without unsubscribe | 每次 occurrence -1 |
| String Concatenation | In loops (use StringBuilder) | 每次 occurrence -0.5 |
| LINQ in Hot Path | Allocating LINQ in tight loops | 每次 occurrence -0.5 |
| Async/Await Misuse | .Result, .Wait() blocking | 每次 occurrence -1 |

### Scoring

- 统计 weighted violations → 按 file count normalize
- 0 violations = 10，按情况下调

---

## D8. Defect Density & Risk

**Weight**: 11%

| 指标 | 如何评估 | 分数影响 |
|--------|---------------|-------------|
| Complexity Hotspots | Files with CC > 15 AND LoC > 300 | 每个 hotspot -2 |
| Change Frequency | Files modified often (git log) | Risk multiplier |
| Bug-Fix Frequency | Commits with "fix" in message | 风险指标 |
| Error-Prone Patterns | Nested try/catch, deep nesting | 每个 pattern -1 |
| Global Mutable State | Static mutable fields | 每次 occurrence -1.5 |
| Implicit Dependencies | Hidden coupling via statics/singletons | 每次 occurrence -1 |
| Code Age vs Churn | Old code frequently modified | 风险指标 |

### Scoring

- 组合 hotspot count + pattern violations → risk score
- 如果可用，加入 git history（git log data 是 optional enhancement）

---

## Overall Score Calculation

```
Overall = D1×0.15 + D2×0.15 + D3×0.15 + D4×0.12 + D5×0.12 
        + D6×0.10 + D7×0.10 + D8×0.11
```

四舍五入到一位小数。映射到 grade：

| 分数 | 等级 | 操作 |
|-------|-------|--------|
| 9–10  | A     | Maintain — 无需 action |
| 7–8.9 | B     | Monitor — 在正常 sprint 中处理 |
| 5–6.9 | C     | Plan — 安排 improvement sprint |
| 3–4.9 | D     | Act — 优先处理 quality improvement |
| 0–2.9 | F     | Escalate — 停止 feature work，修复质量 |
