# Definition of Done

一个常设的、项目范围的完成标准，每个变更都必须满足它才算完成。与 acceptance criteria 不同，acceptance criteria 会随任务变化并回答“我们是否构建了正确的东西？”，Definition of Done 每次都相同，并回答“这是否达到了我们的完成标准？”。在 `planning-and-task-breakdown`、`incremental-implementation` 和 `shipping-and-launch` 中将它作为最终 gate。

## Definition of Done vs. Acceptance Criteria

| | Acceptance Criteria | Definition of Done |
|---|---|---|
| Scope | 只针对一个 task 或 spec | 适用于每个 increment |
| Changes | 每项不同 | 固定且复用 |
| Answers | “Did we build *this thing*?” | “Is it *ready*?” |
| Owner | 规划 task 时定义 | 项目只定义一次 |
| Example | “User can reset password via email link” | “Tests pass, no regressions, docs updated” |

二者互为补充。一个 task 只有在**它的** acceptance criteria 被满足，且常设 Definition of Done 也被满足时，才算完成。跳过任一项都会留下看起来完成但实际上未完成的工作。

## The Standing Checklist

在声明任何变更完成前，都应用此 checklist。

### Correctness
- [ ] task 的所有 acceptance criteria 都已满足
- [ ] 代码能运行并按预期行为工作，经过运行时验证，而不只是编译或 typecheck
- [ ] 新行为有测试覆盖；没有该变更时测试会失败，有该变更时测试会通过
- [ ] 现有测试仍然通过；没有引入 regressions
- [ ] Edge cases 和 error paths 已处理，而不只是 happy path

### Quality
- [ ] 代码通过命名和结构表达意图；不需要 comments 来解释它*做什么*
- [ ] 没有重复的 business logic
- [ ] 没有遗留 dead code、debug output 或 commented-out blocks
- [ ] 变更范围限于任务；没有偷偷加入无关 refactors
- [ ] Linting 和 formatting 通过

这些条目背后的深度内容位于 `code-review-and-quality`（五轴评审）和 `code-simplification`（在不改变行为的前提下降低复杂度）。

### Integration
- [ ] 变更能与系统其余部分配合工作，而不只是在隔离状态下可用
- [ ] Database migrations、config changes 和 feature flags 已考虑
- [ ] 对任何 public interface 或 API change，都考虑了 backward compatibility

### Documentation
- [ ] Public interfaces、APIs 和用户可见行为已有文档
- [ ] 值得保留的 architectural decisions 已记录（见 `documentation-and-adrs`）
- [ ] 文档用不依赖时间的语言描述当前状态，而不是变更历史

### Ship-readiness
- [ ] 对任何 untrusted input、auth 或 data handling，已评审 security implications（见 `security-and-hardening`）
- [ ] 新 critical paths 已有 observability（logs、metrics、traces）（见 `observability-and-instrumentation`）
- [ ] 任何有风险的内容都有 rollback path（见 `shipping-and-launch`）
- [ ] 合并或部署前，人类已经评审并批准

## 如何应用

- **Per task**: 勾掉 task 前确认 Correctness 和 Quality sections。
- **Per feature**: 在认为 feature 完成前确认 Integration 和 Documentation。
- **Per release**: 完整 checklist 是最低标准；`shipping-and-launch` 会在其上添加部署特定 gates。

针对项目只定制一次此列表，然后不变地复用它。每个 sprint 都重新谈判的 Definition of Done，不是真正的 Definition of Done。

## Red Flags

- “It's done, I just haven't run it yet”: 未验证的工作不算完成。
- 用 “Tests pass” 作为 done 的同义词，同时跳过 docs、regressions 或 runtime verification。
- 根据 deadline pressure 应用不同标准。
- 把 acceptance criteria 当作全部标准，没有常设质量底线。
- 在需要 human review 的变更上，review 前就声明 “Done”。
