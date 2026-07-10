# OpenCode 设置

本指南说明如何以非常接近 Claude Code 体验的方式在 OpenCode 中使用 Agent Skills（自动 skill 选择、生命周期驱动工作流，以及严格流程执行）。

## 概览

OpenCode 支持自定义 `/commands`，但没有像 Claude Code 那样的原生 plugin 系统或自动 skill routing。

因此，我们通过以下方式实现接近一致的体验：

- 强 system prompt（`AGENTS.md`）
- 内置 `skill` tool
- 从 `/skills` 目录进行一致的 skill discovery

这会创建一个**agent 驱动的工作流**，其中 skills 会被自动选择并执行。

虽然可以在 OpenCode 中重建 `/spec`、`/plan` 和其他 commands，但此集成刻意采用 agent 驱动方式：

- Skills 会根据 intent 自动选择
- Workflows 通过 `AGENTS.md` 强制执行
- 不需要手动调用 command

这更接近 Claude Code 的实际行为：skills 会自动触发，而不是手动触发。

---

## 安装

1. Clone 仓库：

```bash
git clone https://github.com/addyosmani/agent-skills.git
```

2. 在 OpenCode 中打开项目。

3. 确认你的 workspace 中存在以下文件：

- `AGENTS.md`（根目录）
- `skills/` 目录

不需要额外安装。

---

## 工作原理

### 1. Skill Discovery

所有 skills 都位于：

```
skills/<skill-name>/SKILL.md
```

OpenCode agents 会被指示（通过 `AGENTS.md`）：

- 检测 skill 何时适用
- 调用 `skill` tool
- 严格遵循 skill

### 2. 自动调用 Skill

agent 会评估每个请求，并将其映射到合适的 skill。

示例：

- “build a feature” → `incremental-implementation` + `test-driven-development`
- “design a system” → `spec-driven-development`
- “fix a bug” → `debugging-and-error-recovery`
- “review this code” → `code-review-and-quality`

用户**不需要**显式请求 skills。

### 3. Lifecycle Mapping（隐式 Commands）

开发生命周期被隐式编码为：

- DEFINE → `spec-driven-development`
- PLAN → `planning-and-task-breakdown`
- BUILD → `incremental-implementation` + `test-driven-development`
- VERIFY → `debugging-and-error-recovery`
- REVIEW → `code-review-and-quality`
- SHIP → `shipping-and-launch`

这取代了 `/spec`、`/plan` 等 slash commands。

---

## 使用示例

### 示例 1：Feature Development

User:
```
Add authentication to this app
```

Agent behavior:
- 检测到 feature work
- 调用 `spec-driven-development`
- 在写代码前产出 spec
- 进入 planning 和 implementation skills

---

### 示例 2：Bug Fix

User:
```
This endpoint is returning 500 errors
```

Agent behavior:
- 调用 `debugging-and-error-recovery`
- Reproduces → localizes → fixes → adds guards

---

### 示例 3：Code Review

User:
```
Review this PR
```

Agent behavior:
- 调用 `code-review-and-quality`
- 应用结构化 review（correctness、design、readability 等）

---

## Agent Expectations（关键）

为了让 OpenCode 正常工作，agent 必须遵循这些规则：

- 行动前始终检查是否有适用 skill
- 如果 skill 适用，就必须使用它
- 永不跳过必需工作流（spec、plan、test 等）
- 不要直接跳到实现

这些规则通过 `AGENTS.md` 强制执行。

---

## 限制

- 没有原生 slash commands（改由 intent mapping 处理）
- 没有 plugin 系统（改由 prompt + structure 处理）
- Skill invocation 依赖模型遵从性

尽管如此，该工作流在实践中非常接近 Claude Code。

---

## 推荐工作流

直接使用自然语言：

- “Design a feature”
- “Plan this change”
- “Implement this”
- “Fix this bug”
- “Review this”

agent 会自动选择并执行正确 skills。

---

## 总结

OpenCode 集成通过组合以下内容工作：

- 结构化 skills（本 repo）
- 强 agent rules（`AGENTS.md`）
- 基于推理的自动 skill invocation

其结果是一个**完全 agent 驱动、生产级的工程工作流**，不需要 plugins 或手动 commands。
