---
name: backlog-grooming
description: 创建、审查或细化 backlog 任务定义；编写 acceptance criteria；应用 INVEST 原则；垂直切分功能；为 backlog items 设置 demo_required 标志和 coverage thresholds。
---

# Backlog Grooming Skill

## INVEST Story 质量标准

| 标准 | 检查 |
|-----------|-------|
| **I**ndependent | 这个任务能否在不同时启动另一个任务的情况下实现？ |
| **N**egotiable | 实现细节是否开放（受 ADR 约束，而不是受 story 约束）？ |
| **V**aluable | 任务完成后，operator 或系统是否会表现出可观察的不同行为？ |
| **E**stimable | developer 能否有信心在一个聚焦会话中完成它？ |
| **S**mall | 它是否适合一个 developer 会话，而不是多天工作？ |
| **T**estable | 它是否有 `demo_scenario`（feature）或可度量的 `coverage_threshold`（infrastructure）？ |

## Vertical Slice 验证

对每个 `type: feature` 任务，验证：

1. 任务完成后，operator 会**看到**或**做**什么不同的事？
2. 会触达哪些架构层？（必须 ≥ 2 层）
3. 能否在运行中的应用上于 < 2 分钟内演示？（必须为 YES）
4. `demo_scenario` 是否命名了 feature 文件中精确的 `Scenario:`？（必须逐字匹配）

如果任何答案不正确 → 在任务进入 backlog 前**拒绝并重塑任务**。

## Infrastructure Task 验证

Infrastructure tasks 必须：
- 提供书面 justification，说明为什么它不能成为 vertical slice
- 具有 `coverage_threshold: 90` 或更高
- 不超过 backlog 中任务总数的 20%

如果 infrastructure tasks 超过 backlog 的 20%：product owner 正在推迟集成。需要 push back 并重塑。

## 任务大小检查

每个任务由 `@developer` 在**一个上下文窗口**中实现。过大的任务会让 developer 耗尽上下文并在没有 handover 的情况下退出 — 失去全部工作。宁可更小。

大小合适的任务：
- 其 feature 文件中有 3–5 个 Gherkin scenarios（把 5 视为软上限，而不是目标）
- 触碰 `files_likely_affected` 中 2–5 个文件
- 有 ≤ 2 个直接 `depends_on` 条目
- 可在一个 developer 会话中完成（约 1–3 小时）

如果一个任务有 > 5 个 scenarios 或 > 5 个受影响文件 → 拆分它。当确实不确定任务是否太大时，**拆分它** — 两个带 `depends_on` 串接的小型可演示 slice 永远比一个过大的任务更安全。

拆分时：
- 每个拆出的任务仍必须独立满足 Vertical Slicing Rule（用户可见、≥ 2 层、< 2 min demo、有自己的 `demo_scenario`）。
- 不要用无关工作填充任务，比如“既然已经改到这里了”。一个任务 = 一个连贯行为。
- 用 `depends_on` 排列各部分，使每部分建立在上一部分之上。

## Dependency 验证

检查所有 `depends_on` 条目：
- 每个引用的 task ID 是否存在于此 backlog 中？
- 是否存在循环依赖？（A → B → A）
- Infrastructure tasks 是否前置，使所有需要它们的 feature tasks 都在 `depends_on` 中列出它们？

## `design_note` 质量检查

每个任务的 `design_note` 必须：
- 至少命名一个具体 ADR 文件（例如 “See ADR-002 for IPositionPublisher pattern”）
- 说明要使用的架构模式或接口
- 不能是 “follow good practices” 这类泛泛指令

如果 `design_note` 缺失或泛泛而谈 → 继续前返回 `@sw-architect` 澄清。
