# agent-skills 入门

agent-skills 适用于任何接受 Markdown instructions 的 AI coding agent。本指南介绍通用方法。工具特定设置请参见专门指南。

## Skills 如何工作

每个 skill 都是一个 Markdown 文件（`SKILL.md`），描述一个具体的工程工作流。当加载到 agent 的上下文中时，agent 会遵循该工作流，包括验证步骤、需要避免的反模式和退出标准。

**Skills 不是参考文档。** 它们是 agent 要遵循的逐步流程。

## 快速开始（任意 Agent）

### 1. Clone 仓库

```bash
git clone https://github.com/addyosmani/agent-skills.git
```

### 2. 选择一个 skill

浏览 `skills/` 目录。每个子目录都包含一个 `SKILL.md`，其中包括：
- **When to use** — 表明此 skill 适用的触发条件
- **Process** — 逐步工作流
- **Verification** — 如何确认工作已完成
- **Common rationalizations** — agent 可能用来跳过步骤的借口
- **Red flags** — skill 正在被违反的信号

### 3. 将 skill 加载到你的 agent 中

将相关 `SKILL.md` 内容复制到 agent 的 system prompt、rules file 或对话中。最常见的方式：

**System prompt:** 在会话开始时粘贴 skill 内容。

**Rules file:** 将 skill 内容添加到项目的 rules file（CLAUDE.md、.cursorrules 等）。

**Conversation:** 下达指令时引用该 skill：“Follow the test-driven-development process for this change.”

### 4. 使用 meta-skill 进行发现

从加载 `using-agent-skills` skill 开始。它包含一个 flowchart，会将任务类型映射到合适的 skill。

## 推荐设置

### Minimal（从这里开始）

将三个核心 skills 加载到你的 rules file：

1. **spec-driven-development** — 用于定义要构建什么
2. **test-driven-development** — 用于证明它能工作
3. **code-review-and-quality** — 用于在合并前验证质量

这三个覆盖了 AI 辅助开发中最关键的质量缺口。

### Full Lifecycle

如需全面覆盖，请按阶段加载 skills：

```
Starting a project:  spec-driven-development → planning-and-task-breakdown
During development:  incremental-implementation + test-driven-development
Before merge:        code-review-and-quality + security-and-hardening
Before deploy:       shipping-and-launch
```

### Context-Aware Loading

不要一次加载所有 skills — 这会浪费上下文。加载与当前任务相关的 skills：

- 处理 UI？加载 `frontend-ui-engineering`
- 调试？加载 `debugging-and-error-recovery`
- 设置 CI？加载 `ci-cd-and-automation`

## Skill Anatomy

每个 skill 都遵循相同结构：

```
YAML frontmatter (name, description)
├── Overview — What this skill does
├── When to Use — Triggers and conditions
├── Core Process — Step-by-step workflow
├── Examples — Code samples and patterns
├── Common Rationalizations — Excuses and rebuttals
├── Red Flags — Signs the skill is being violated
└── Verification — Exit criteria checklist
```

完整规范见 [skill-anatomy.md](skill-anatomy.md)。

## 使用 Agents

`agents/` 目录包含预配置的 agent personas：

| Agent | Purpose |
|-------|---------|
| `code-reviewer.md` | 五轴代码评审 |
| `test-engineer.md` | 测试策略和编写 |
| `security-auditor.md` | 漏洞检测 |
| `web-performance-auditor.md` | Core Web Vitals 与性能审计（通过 `/webperf`） |

当你需要专门评审时，加载一个 agent definition。例如，要求你的 coding agent “review this change using the code-reviewer agent persona”，并提供该 agent definition。

## 使用 Commands

`.claude/commands/` 目录包含 Claude Code 的 slash commands：

| Command | Skill Invoked |
|---------|---------------|
| `/spec` | spec-driven-development |
| `/plan` | planning-and-task-breakdown |
| `/build` | incremental-implementation + test-driven-development |
| `/build auto` | planning-and-task-breakdown → incremental-implementation + test-driven-development（整个 plan，一次批准） |
| `/test` | test-driven-development |
| `/review` | code-review-and-quality |
| `/code-simplify` | code-simplification |
| `/ship` | shipping-and-launch |
| `/webperf` | web-performance-auditor（专家 agent，仅 Web 应用） |

> **Note:** 当作为 Claude Code plugin 安装时，你可能会看到类似
> _"Default commands/ folder is ignored because the manifest sets 'commands'"_ 的警告。
> 这是预期行为。根目录 `commands/` 属于 Antigravity CLI，
> 并且有意与 `.claude/commands/` 分离。所有 Claude Code slash
> commands 都会从 `.claude/commands/` 正确加载；该警告只是外观问题。

## 使用 References

`references/` 目录包含补充 checklists：

| Reference | Use With |
|-----------|----------|
| `testing-patterns.md` | test-driven-development |
| `performance-checklist.md` | performance-optimization |
| `security-checklist.md` | security-and-hardening |
| `accessibility-checklist.md` | frontend-ui-engineering |

当你需要 skill 覆盖范围之外的详细模式时，加载一个 reference。

## Spec 和 task artifacts

`/spec` 和 `/plan` commands 会创建工作 artifacts（`SPEC.md`、`tasks/plan.md`、`tasks/todo.md`）。在工作进行中，把它们视为**活文档**：

- 开发期间将它们纳入版本控制，让人类和 agent 拥有共享的事实来源。
- 当范围或决策变化时更新它们。
- 如果你的 repo 不希望长期保留这些文件，请在合并前删除它们，或将该文件夹加入 `.gitignore` — 工作流不要求它们永久存在。

## Tips

1. **从 spec-driven-development 开始**，用于任何非平凡工作
2. **编写代码时始终加载 test-driven-development**
3. **不要跳过验证步骤** — 它们就是全部意义所在
4. **有选择地加载 skills** — 更多上下文不总是更好
5. **使用 agents 做评审** — 不同视角会捕获不同问题
