# AGENTS.md

本文件为 AI 编码代理（Claude Code、Cursor、Copilot、Antigravity 等）在本仓库中处理代码时提供指导。

## 仓库概览

这是面向高级软件工程师的 Claude.ai 和 Claude Code 技能集合。技能是打包的说明和脚本，用于扩展 Claude 以及你的编码代理能力。

## OpenCode 集成

OpenCode 使用由 `skill` 工具和本仓库 `/skills` 目录驱动的**技能驱动执行模型**。

### 核心规则

- 如果任务匹配某个技能，必须调用该技能
- 技能位于 `skills/<skill-name>/SKILL.md`
- 如果有适用技能，绝不要直接实现
- 始终严格遵循技能说明（不要只部分应用）

### 意图 → 技能映射

代理应自动将用户意图映射到技能：

- 功能 / 新功能 → `spec-driven-development`，然后 `incremental-implementation`、`test-driven-development`
- 规划 / 拆解 → `planning-and-task-breakdown`
- Bug / 失败 / 意外行为 → `debugging-and-error-recovery`
- 代码审查 → `code-review-and-quality`
- 重构 / 简化 → `code-simplification`
- API 或接口设计 → `api-and-interface-design`
- UI 工作 → `frontend-ui-engineering`

### 生命周期映射（隐式命令）

OpenCode 不支持 `/spec` 或 `/plan` 这样的斜杠命令。

因此，代理必须在内部遵循此生命周期：

- DEFINE → `spec-driven-development`
- PLAN → `planning-and-task-breakdown`
- BUILD → `incremental-implementation` + `test-driven-development`
- VERIFY → `debugging-and-error-recovery`
- REVIEW → `code-review-and-quality`
- SHIP → `shipping-and-launch`

### 执行模型

对每个请求：

1. 判断是否有任何技能适用（哪怕只有 1% 的可能性）
2. 使用 `skill` 工具调用相应技能
3. 严格遵循该技能工作流
4. 只有在必需步骤（spec、plan 等）完成后，才能继续实现

### 反合理化

以下想法是错误的，必须忽略：

- “这个太小了，不需要技能”
- “我可以很快直接实现”
- “我先收集上下文吧”

正确行为：

- 始终先检查并使用技能

这确保 OpenCode 的行为类似于 Claude Code，并具备完整的工作流强制执行。

## 编排：Personas、Skills 和 Commands

本仓库有三个可组合层。它们职责不同，不应混淆：

- **Skills** (`skills/<name>/SKILL.md`) — 带有步骤和退出标准的工作流。负责 *how*。当意图匹配时必须经过。
- **Personas** (`agents/<role>.md`) — 带有视角和输出格式的角色。负责 *who*。
- **Slash commands** (`.claude/commands/*.md`) — 面向用户的入口点。负责 *when*。这是编排层。

组合规则：**用户（或斜杠命令）是编排者。Personas 不调用其他 personas。** Persona 可以调用 skills。

本仓库认可的唯一多 persona 编排模式是**带合并步骤的并行 fan-out** — `/ship` 使用它并发运行 `code-reviewer`、`security-auditor` 和 `test-engineer`，并综合它们的报告。不要构建一个由 persona 决定调用哪些其他 persona 的 “router” persona；这是斜杠命令和意图映射的职责。

请参阅 [docs/agents.md](docs/agents.md) 了解决策矩阵，并参阅 [references/orchestration-patterns.md](references/orchestration-patterns.md) 了解完整模式目录。

**Claude Code 互操作：** `agents/` 中的 personas 可作为 Claude Code subagents（从此插件的 `agents/` 目录自动发现）以及 Agent Teams 队友（生成时按名称引用）使用。两个平台约束与我们的规则一致：subagents 不能生成其他 subagents，teams 不能嵌套。插件 agents 会静默忽略 `hooks`、`mcpServers` 和 `permissionMode` frontmatter 字段。

## 创建新技能

> **开始之前：** 运行 [CONTRIBUTING.md](CONTRIBUTING.md#before-proposing-a-new-skill) 中的预检，搜索目录，检查开放 PR（`gh pr list --state open`），确认想法符合 [docs/skill-anatomy.md](docs/skill-anatomy.md)，并在 PR 描述中说明缺口。多数新技能想法会与现有技能或开放 PR 重叠；优先扩展现有技能，而不是添加近似重复项。CONTRIBUTING.md 是此工作流的唯一事实来源。

本仓库中的技能以 Markdown 为先：每个技能位于 `skills/<kebab-case-name>/SKILL.md`，带有 YAML frontmatter（`name`、`description`），并遵循章节结构（Overview、When to Use、Process、Common Rationalizations、Red Flags、Verification）。只有当技能附带可运行辅助程序时才添加 `scripts/` 目录；多数技能仅为 Markdown，且没有按技能划分的 zip 包。

完整格式、命名约定、frontmatter 规则、支持文件阈值和写作原则，请参阅 [docs/skill-anatomy.md](docs/skill-anatomy.md)，它是技能结构的唯一事实来源。不要在此处复述该指南，请链接到它。