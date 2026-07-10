---
description: 通过并行 fan-out 让专家 personas 运行发布前检查清单，然后综合 go/no-go 决策
---

调用 agent-skills:shipping-and-launch 技能。

`/ship` 是一个 **fan-out orchestrator**。它针对当前更改并行运行三个专家 personas，然后将它们的报告合并为一个带回滚计划的 go/no-go 决策。personas 独立运行 — 没有共享状态、没有顺序依赖 — 这正是并行执行在这里安全且有用的原因。

## 阶段 A — 并行 fan-out

使用 Agent tool 并发生成三个 subagents。**在同一个 assistant turn 中发出全部三个 Agent tool calls，使它们并行执行** — 顺序调用会违背此命令的目的。

在 Claude Code 中，每次调用传入与 persona `name` 字段匹配的 `subagent_type`：

1. **`code-reviewer`** — 对 staged changes 或 recent commits 运行五轴审查（correctness、readability、architecture、security、performance）。输出标准审查模板。
2. **`security-auditor`** — 运行漏洞和 threat-model 检查。检查 OWASP Top 10、secrets handling、auth/authz、dependency CVEs。输出标准审计报告。
3. **`test-engineer`** — 分析更改的测试覆盖。识别 happy path、edge cases、error paths 和 concurrency scenarios 中的缺口。输出标准覆盖分析。

在没有 Agent tool 的其他 harness 中，按顺序调用每个 persona 的 system prompt，并把它们的输出视为并行返回 — 合并阶段仍然适用。

约束（来自 Claude Code 的 subagent model）：
- Subagents 不能生成其他 subagents — 不要让一个 persona 委托给另一个。
- 每个 subagent 都获得自己的 context window，并且只把自己的报告返回到主会话。
- 如果你需要队友之间相互交谈，而不只是回报，请使用 Claude Code Agent Teams，并将这些 personas 作为 teammate types 引用（参见 `references/orchestration-patterns.md`）。

**Persona 解析。** 如果你在 `.claude/agents/` 或 `~/.claude/agents/` 中定义了自己的 `code-reviewer`、`security-auditor` 或 `test-engineer`，它们会优先于此插件版本 — `/ship` 会自动采用你的自定义内容。这是有意设计的：插件 subagents 位于 Claude Code 作用域优先级表的底部，因此用户级定义按设计获胜。

## 阶段 B — 在主上下文中合并

三个报告全部返回后，主代理（不是 sub-persona）对它们进行综合：

1. **Code Quality** — 汇总 `code-reviewer` 的 Critical/Important findings，以及任何失败的 tests、lint 或 build output。合并审查者之间的重复项。
2. **Security** — 将任何 Critical/High `security-auditor` findings 提升为发布阻塞项。与 `code-reviewer` 的 security axis 交叉引用。
3. **Performance** — 从 `code-reviewer` 的 performance axis 提取；如适用，交叉检查 Core Web Vitals。
4. **Accessibility** — 验证 keyboard nav、screen reader support、contrast（三个 personas 未覆盖 — 在这里直接处理，或调用 accessibility checklist）。
5. **Infrastructure** — Env vars、migrations、monitoring、feature flags。直接验证。
6. **Documentation** — README、ADRs、changelog。直接验证。

## 阶段 C — 决策和回滚

生成单一输出：

```markdown
## Ship Decision: GO | NO-GO

### Blockers (must fix before ship)
- [Source persona: Critical finding + file:line]

### Recommended fixes (should fix before ship)
- [Source persona: Important finding + file:line]

### Acknowledged risks (shipping anyway)
- [Risk + mitigation]

### Rollback plan
- Trigger conditions: [what signals would prompt rollback]
- Rollback procedure: [exact steps]
- Recovery time objective: [target]

### Specialist reports (full)
- [code-reviewer report]
- [security-auditor report]
- [test-engineer report]
```

## 规则

1. 三个阶段 A personas 并行运行 — 绝不顺序运行。
2. Personas 不相互调用。主代理在阶段 B 合并。
3. 在任何 GO 决策之前，回滚计划都是必需的。
4. 如果任何 persona 返回 Critical finding，默认结论是 NO-GO，除非用户明确接受风险。
5. **只有在以下条件全部为真时才跳过 fan-out：** 更改触及 2 个或更少文件，diff 少于 50 行，并且不触及 auth、payments、data access 或 config/env。否则，默认进行 fan-out。`/ship` 面向生产发布更改 — 当 blast radius 非平凡时，即使 diff 看起来很小，也要运行并行审查。