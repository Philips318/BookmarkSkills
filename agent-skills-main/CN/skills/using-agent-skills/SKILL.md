---
name: using-agent-skills
description: 发现并调用 agent skills。用于会话开始时，或需要发现哪个 skill 适用于当前任务时。这是治理所有其他 skills 如何被发现和调用的元技能。
---

# 使用 Agent Skills

## 概述

Agent Skills 是一组按开发阶段组织的工程工作流技能。每个 skill 都编码了一种资深工程师遵循的具体流程。这个元技能帮助你发现并应用适合当前任务的 skill。

## Skill 发现

任务到来时，识别开发阶段并应用对应 skill：

```
Task arrives
    │
    ├── Don't know what you want yet? ──────→ interview-me
    ├── Have a rough concept, need variants? → idea-refine
    ├── New project/feature/change? ──→ spec-driven-development
    ├── Have a spec, need tasks? ──────→ planning-and-task-breakdown
    ├── Implementing code? ────────────→ incremental-implementation
    │   ├── UI work? ─────────────────→ frontend-ui-engineering
    │   ├── API work? ────────────────→ api-and-interface-design
    │   ├── Need better context? ─────→ context-engineering
    │   ├── Need doc-verified code? ───→ source-driven-development
    │   └── Stakes high / unfamiliar code? ──→ doubt-driven-development
    ├── Writing/running tests? ────────→ test-driven-development
    │   └── Browser-based? ───────────→ browser-testing-with-devtools
    ├── Something broke? ──────────────→ debugging-and-error-recovery
    ├── Reviewing code? ───────────────→ code-review-and-quality
    │   ├── Too complex? ─────────────→ code-simplification
    │   ├── Security concerns? ───────→ security-and-hardening
    │   └── Performance concerns? ────→ performance-optimization
    ├── Committing/branching? ─────────→ git-workflow-and-versioning
    ├── CI/CD pipeline work? ──────────→ ci-cd-and-automation
    ├── Deprecating/migrating? ────────→ deprecation-and-migration
    ├── Writing docs/ADRs? ───────────→ documentation-and-adrs
    ├── Adding logs/metrics/alerts? ───→ observability-and-instrumentation
    └── Deploying/launching? ─────────→ shipping-and-launch
```

## 核心运行行为

这些行为始终适用，跨所有 skills。它们不可协商。

### 1. 暴露假设

实现任何非平凡内容前，明确说明你的假设：

```
ASSUMPTIONS I'M MAKING:
1. [assumption about requirements]
2. [assumption about architecture]
3. [assumption about scope]
→ Correct me now or I'll proceed with these.
```

不要默默填补模糊需求。最常见的失败模式是做出错误假设并 unchecked 地推进。尽早暴露不确定性，它比返工便宜。

### 2. 主动管理困惑

遇到不一致、冲突需求或不清晰 specification 时：

1. **STOP.** 不要猜着继续。
2. 命名具体困惑。
3. 展示取舍或提出澄清问题。
4. 等待解决后再继续。

**坏：** 静默选择一种解释并希望它正确。
**好：** “I see X in the spec but Y in the existing code. Which takes precedence?”

### 3. 必要时推回去

你不是 yes-machine。当某个方法存在明确问题时：

- 直接指出问题
- 解释具体 downside（可量化时量化，例如“this adds ~200ms latency”，而不是“this might be slower”）
- 提出替代方案
- 如果人类在充分知情后仍然坚持，接受他们的决定

Sycophancy 是失败模式。“Of course!” 后实现一个坏想法对谁都没帮助。诚实的技术分歧比虚假同意更有价值。

### 4. 强制简单

你的自然倾向是过度复杂化。主动抵抗它。

完成任何实现前，问：
- 这能用更少行完成吗？
- 这些抽象是否配得上它们的复杂度？
- Staff engineer 会不会看了说“为什么不直接……”？

如果你构建了 1000 行，而 100 行足够，那就是失败。偏爱无聊、明显的解决方案。聪明很昂贵。

### 5. 保持范围纪律

只触碰被要求触碰的内容。

不要：
- 删除你不理解的注释
- “清理”与任务正交的代码
- 顺带重构相邻系统
- 未经明确批准删除看似未使用的代码
- 因为“看起来有用”而添加 spec 中没有的功能

你的工作是外科式精准，而不是未经请求的翻新。

### 6. 验证，不要假设

每个 skill 都包含验证步骤。任务在验证通过前不算完成。“看起来对”永远不够，必须有证据（passing tests、build output、runtime data）。

每个 skill 的验证是局部检查。适用于*每个*变更的项目级标准是 Definition of Done：测试通过、无回归、运行时验证行为、文档已更新。见 `references/definition-of-done.md`。它补充每个任务的验收标准，而不是替代它们。

## 要避免的失败模式

这些微妙错误看起来像生产力，但会制造问题：

1. 不检查就做错误假设
2. 不管理自己的困惑，迷路时硬推进
3. 不暴露你注意到的不一致
4. 对非显而易见决策不展示取舍
5. 对明显有问题的方法 sycophantic（“Of course!”）
6. 让代码和 APIs 过度复杂
7. 修改与任务正交的代码或注释
8. 删除你没有完全理解的东西
9. 因为“很明显”而没有 spec 就构建
10. 因为“看起来对”而跳过验证

## Skill 规则

1. **开始工作前检查是否有适用 skill。** Skills 编码了防止常见错误的流程。

2. **Skills 是工作流，不是建议。** 按顺序遵循步骤。不要跳过验证步骤。

3. **多个 skills 可以同时适用。** 一个功能实现可能按顺序涉及 `idea-refine` → `spec-driven-development` → `planning-and-task-breakdown` → `incremental-implementation` → `test-driven-development` → `code-review-and-quality` → `code-simplification` → `shipping-and-launch`。

4. **拿不准时，从 spec 开始。** 如果任务非平凡且没有 spec，从 `spec-driven-development` 开始。

## 生命周期顺序

对于完整功能，典型 skill 顺序是：

```
1.  interview-me                → Extract what the user actually wants
2.  idea-refine                 → Refine vague ideas
3.  spec-driven-development     → Define what we're building
4.  planning-and-task-breakdown → Break into verifiable chunks
5.  context-engineering         → Load the right context
6.  source-driven-development   → Verify against official docs
7.  incremental-implementation  → Build slice by slice
8.  observability-and-instrumentation → Instrument as you build (runs parallel with 7-9, not after)
9.  doubt-driven-development    → Cross-examine non-trivial decisions in-flight
10. test-driven-development     → Prove each slice works
11. code-review-and-quality     → Review before merge
12. code-simplification         → Reduce unnecessary complexity while preserving behavior
13. git-workflow-and-versioning → Clean commit history
14. documentation-and-adrs      → Document the why, not just the what
15. deprecation-and-migration   → Retire old systems and move users safely when needed
16. shipping-and-launch         → Deploy safely
```

不是每个任务都需要每个 skill。一个 bug fix 可能只需要：`debugging-and-error-recovery` → `test-driven-development` → `code-review-and-quality`。

## 快速参考

| 阶段 | Skill | 一句话摘要 |
|-------|-------|-----------------|
| Define | interview-me | 在任何计划、spec 或代码存在前，暴露用户真正想要什么 |
| Define | idea-refine | 通过结构化发散与收敛思考打磨想法 |
| Define | spec-driven-development | 代码前的需求和验收标准 |
| Plan | planning-and-task-breakdown | 拆解成小而可验证的任务 |
| Build | incremental-implementation | 薄的垂直切片，每个切片扩展前先测试 |
| Build | source-driven-development | 实现前对照官方文档验证 |
| Build | doubt-driven-development | 对每个非平凡决策进行 fresh-context 对抗性审查 |
| Build | context-engineering | 在正确时间加载正确上下文 |
| Build | frontend-ui-engineering | 具备可访问性的生产质量 UI |
| Build | api-and-interface-design | 稳定接口和清晰契约 |
| Verify | test-driven-development | 先失败测试，再让它通过 |
| Verify | browser-testing-with-devtools | 使用 Chrome DevTools MCP 做运行时验证 |
| Verify | debugging-and-error-recovery | Reproduce → localize → fix → guard |
| Review | code-review-and-quality | 五轴审查和质量门禁 |
| Review | code-simplification | 在保持行为的同时减少不必要复杂度 |
| Review | security-and-hardening | OWASP 预防、输入验证、最小权限 |
| Review | performance-optimization | 先测量，只优化真正重要的内容 |
| Ship | git-workflow-and-versioning | 原子提交、干净历史 |
| Ship | ci-cd-and-automation | 每次变更的自动化质量门禁 |
| Ship | deprecation-and-migration | 移除旧系统并安全迁移用户 |
| Ship | documentation-and-adrs | 记录为什么，而不只是是什么 |
| Ship | observability-and-instrumentation | 结构化日志、RED metrics、traces、基于症状的告警 |
| Ship | shipping-and-launch | 预发布清单、监控、回滚计划 |
