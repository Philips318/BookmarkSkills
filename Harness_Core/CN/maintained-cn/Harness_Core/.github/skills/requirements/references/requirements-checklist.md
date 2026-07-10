- Every AC must map to at least one Gherkin scenario.
- Gherkin scenarios must be independently executable.
- No scenario should depend on another scenario's side effects.
- Use concrete values in examples, not placeholders.

## Acceptance Criteria

每个 AC 必须满足：

1. **Atomic** — 只测试一个 behavior。
2. **Independent** — 不依赖其他 AC。
3. **Unambiguous** — 只能有一种解释。
4. **Measurable** — 能客观判断 pass/fail。
5. **Traceable** — 链接回 parent User Story。

### Coverage Matrix

对每个 requirement，验证 AC 在以下方面的 coverage：

| 类别 | 是否覆盖？ | 示例 |
|----------|----------|---------|
| 正常路径 | ☐ | Normal user flow |
| Alternative flow | ☐ | Different valid input |
| 边界（最小） | ☐ | Minimum allowed value |
| 边界（最大） | ☐ | Maximum allowed value |
| Invalid input | ☐ | Out-of-range, null, empty |
| Error recovery | ☐ | Network failure, timeout |
| 并发 | ☐ | Simultaneous access |
| 安全性 | ☐ | Unauthorized access attempt |

## NFR Checklist

| 类别 | 问题 | 适用对象 |
|----------|----------|-------------|
| 性能 | Response time < ? ms | 全部 |
| 性能 | Memory usage < ? MB | 图像处理 |
| 安全性 | 是否需要 authentication？ | patient data |
| 安全性 | 是否需要 audit logging？ | Class B/C |
| 兼容性 | DICOM version? | Imaging modules |
| 兼容性 | Min OS version? | All |
| 可用性 | Keyboard navigation? | Clinical workflow |
| 可靠性 | Auto-save / recovery? | Data entry |

## Completeness Score Rubric

- **90-100**：Ready for Three Amigos，几乎不需要讨论。
- **70-89**：Good draft，需要对 flagged areas 做 targeted discussion。
- **50-69**：存在明显 gaps，继续前需要 stakeholder clarification。
- **Below 50**：Requirement 过于模糊，带具体问题返回 stakeholder。
