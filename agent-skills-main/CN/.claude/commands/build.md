---
description: 增量实现任务 — 构建、测试、验证、提交。添加 "auto" 可在一次批准后运行整个计划。
---

同时调用 agent-skills:incremental-implementation 技能和 agent-skills:test-driven-development 技能。

## 模式

- **`/build`** — 实现*下一个*待处理任务，然后停止（谨慎地一次一个切片）。
- **`/build auto`** — 如有需要先生成计划，获得一次批准，然后不在任务之间停顿地实现*每个*任务。

`$ARGUMENTS` 选择模式。将 `auto`（规范形式）或 `all` 视为自主模式；其他任何值（或空值）都是默认的单任务模式。注意：自主模式并不会让*每个任务*更快 — 它运行相同的测试驱动循环 — 它只移除了任务*之间*的人为步进。

## 默认：一个任务

从计划中选择下一个待处理任务。然后：

1. 阅读该任务的验收标准
2. 加载相关上下文（现有代码、模式、类型）
3. 为预期行为编写一个失败测试（RED）
4. 实现通过测试所需的最少代码（GREEN）
5. 运行完整测试套件以检查回归
6. 运行构建以验证编译
7. 使用描述性消息提交
8. 将任务标记为完成并停止

## 自主：整个计划（`/build auto`）

在已有 spec 且你想把 plan + build 合并为一次运行时使用此模式。它移除的是任务之间的手动步进 — **不是**验证。每个任务仍然必须获得通过的测试和自己的提交。

1. **要求有 spec。** 只在已知路径查找 spec：仓库根目录的 `SPEC.md`、`docs/SPEC.md`，或 `spec/` 下的文件。README 或任意文档**不**算。如果不存在，停止并告诉用户先运行 `/spec` — 不要编造需求。
2. **建立干净基线。** 运行 `git status --porcelain`。如果在预期规划产物（`SPEC.md`、`docs/SPEC.md`、`spec/*`、`tasks/plan.md`、`tasks/todo.md`）之外存在未提交更改，停止并要求用户提交、stash，或确认如何处理它们。自主的逐任务提交绝不能吸收无关的本地工作，否则干净回滚保证会被破坏。
3. **如有需要则计划。** 如果没有 `tasks/plan.md`，调用 agent-skills:planning-and-task-breakdown 来生成一个。
4. **单一检查点。** 展示完整计划，并等待明确肯定（例如 "approve"、"go"、"yes"）。将模糊回应（"looks reasonable"、"I guess"）视为**未**批准。这是唯一的人类关卡 — 批准后即自主运行。如果你生成了 `tasks/plan.md`，现在把它作为一个准备性提交单独提交，避免它混入第一个任务的提交。
5. **按依赖顺序执行每个任务。** 使用每个任务声明的依赖；如果依赖不明确，则按计划列出的顺序执行。对每个任务运行上面的完整默认循环（RED → GREEN → regression → build → commit → mark complete）。只暂存该任务触及的文件及其任务状态更新 — 绝不要盲目执行 `git add -A` — 并且每个任务做一次提交，这样任意点都能干净回滚。
6. **停止并询问用户**（不要硬推过去）当：
   - 测试无法通过，或构建失败且没有明显修复方法 → 遵循 agent-skills:debugging-and-error-recovery
   - spec 有歧义，或任务需要 spec 未覆盖的决策
   - 任务高风险或不可逆 — auth/permission 更改、破坏性数据迁移、payments、deletions、deploys、任何触及 secrets 的内容，**或任何你无法用 `git revert` 撤销的内容** → 遵循 agent-skills:doubt-driven-development，并在继续前获得明确确认

   用户解决阻塞后，重新调用 `/build auto` — 它会从下一个待处理任务恢复。
7. **结束时总结：** 已完成的任务、已添加的测试、已创建的提交，以及任何跳过、标记或留给用户的事项。

如果任何步骤失败，请遵循 agent-skills:debugging-and-error-recovery 技能。