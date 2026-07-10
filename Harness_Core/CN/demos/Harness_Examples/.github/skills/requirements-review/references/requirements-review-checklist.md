## Requirements Review Checklist

### AC Coverage Matrix

对每个 requirement，验证 AC 在以下方面的 coverage：

| 类别 | 是否覆盖？ | Question to Ask |
|----------|----------|----------------|
| 正常路径 | ☐ | 正常/期望流程是否已完整描述？ |
| Alternative flow | ☐ | 是否存在其他达成目标的有效方式？ |
| 边界（最小） | ☐ | 在最小允许值时会发生什么？ |
| 边界（最大） | ☐ | 在最大允许值时会发生什么？ |
| Boundary (zero/empty) | ☐ | zero、null 或 empty input 会发生什么？ |
| Invalid input | ☐ | out-of-range 或 malformed input 会发生什么？ |
| Error recovery | ☐ | network failure、timeout 或 crash 时会发生什么？ |
| 并发 | ☐ | simultaneous access 时会发生什么？ |
| 安全性 | ☐ | unauthorized access 是否被处理？ |
| State transitions | ☐ | 所有 valid state changes 是否已覆盖？ |
| Undo / Cancel | ☐ | action 能否撤销？cancel 时会发生什么？ |
| 性能 | ☐ | 是否有 response time 或 throughput requirement？ |

### Ambiguity Detector — Flagged Terms

如果在 AC 或 NFR 中发现这些 words/phrases，会触发 automatic finding：

| 模式 | Why It's a Problem |
|---------|-------------------|
| "fast", "slow", "quickly" | 没有 measurable threshold |
| "user-friendly", "intuitive" | 主观 |
| "reliable", "robust" | 没有 failure/recovery spec |
| "appropriate", "suitable", "reasonable" | criteria 未定义 |
| "etc.", "and so on", "and more" | 枚举不完整 |
| "should" (without "shall") | obligation level 含糊 |
| "if possible", "ideally" | Optional or mandatory? |
| "some", "many", "few" | 未指定数量 |
| "normally", "usually", "typically" | abnormal cases 呢？ |
| "similar to", "like" | 需精确定义 |

### NFR Completeness Check

| NFR Category | Required Fields |
|-------------|----------------|
| 性能 | Metric + threshold + measurement method + load condition |
| 安全性 | Authentication method + authorization scope + audit requirement |
| 兼容性 | Protocol version + OS versions + hardware constraints |
| 可用性 | Interaction count + accessibility standard + localization scope |
| 可靠性 | MTBF + recovery time + data integrity guarantee |

### IEC 62304 Class-Specific Checks

| 检查项 | Class A | Class B | Class C |
|-------|---------|---------|---------|
| User Story | ✓ | ✓ | ✓ |
| AC (Given/When/Then) | ✓ | ✓ | ✓ |
| NFR with thresholds | ✓ | ✓ | ✓ |
| Verification Methods | ✓ | ✓ | ✓ |
| 影响分析 | — | ✓ | ✓ |
| Risk Assessment | — | — | ✓ |
| Patient Safety Scenarios | — | — | ✓ |
| DFMEA Input | — | — | ✓ |
