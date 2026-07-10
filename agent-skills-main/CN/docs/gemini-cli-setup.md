# 在 Gemini CLI 中使用 agent-skills

## 设置

### 选项 1：作为 Skills 安装（推荐）

Gemini CLI 拥有原生 skills 系统，会自动发现 `.gemini/skills/` 或 `.agents/skills/` 目录中的 `SKILL.md` 文件。每个 skill 会在匹配你的任务时按需激活。

**从 repo 安装：**

```bash
gemini skills install https://github.com/addyosmani/agent-skills.git --path skills
```

**或者从本地 clone 安装：**

```bash
git clone https://github.com/addyosmani/agent-skills.git
gemini skills install /path/to/agent-skills/skills/
```

**只安装到特定 workspace：**

```bash
gemini skills install /path/to/agent-skills/skills/ --scope workspace
```

安装到 workspace scope 的 skills 会进入 `.gemini/skills/`（或 `.agents/skills/`）。用户级 skills 会进入 `~/.gemini/skills/`。

安装后，用以下命令验证：

```
/skills list
```

Gemini CLI 会自动把 skill names 和 descriptions 注入 prompt。当它识别到匹配任务时，会先请求许可，再加载完整 instructions 来激活该 skill。

### 选项 2：GEMINI.md（持久上下文）

对于你希望始终作为持久项目上下文加载的 skills（而不是按需激活），请把它们加入项目的 `GEMINI.md`：

```bash
# Create GEMINI.md with core skills as persistent context
cat /path/to/agent-skills/skills/incremental-implementation/SKILL.md > GEMINI.md
echo -e "\n---\n" >> GEMINI.md
cat /path/to/agent-skills/skills/code-review-and-quality/SKILL.md >> GEMINI.md
```

你也可以通过从独立文件导入来模块化：

```markdown
# Project Instructions

@skills/test-driven-development/SKILL.md
@skills/incremental-implementation/SKILL.md
```

使用 `/memory show` 验证已加载上下文，修改后用 `/memory reload` 刷新。

> **Skills vs GEMINI.md:** Skills 是按需激活的专业知识，只在相关时激活，让上下文窗口保持干净。GEMINI.md 提供每个 prompt 都会加载的持久上下文。将 skills 用于阶段特定工作流，将 GEMINI.md 用于始终生效的项目约定。

## 推荐配置

### Always-On（GEMINI.md）

将这些作为每个会话的持久上下文：

- `incremental-implementation` — 以小而可验证的切片构建
- `code-review-and-quality` — 五轴评审

### On-Demand（Skills）

将这些安装为 skills，让它们只在相关时激活：

- `test-driven-development` — 在实现逻辑或修复 bug 时激活
- `spec-driven-development` — 在启动新项目或新 feature 时激活
- `frontend-ui-engineering` — 在构建 UI 时激活
- `security-and-hardening` — 在安全评审期间激活
- `performance-optimization` — 在性能工作期间激活

## 高级配置

### MCP 集成

此包中的许多 skills 会利用 [Model Context Protocol (MCP)](https://modelcontextprotocol.io/) tools 与环境交互。例如：

- `browser-testing-with-devtools` 使用 `chrome-devtools` MCP extension。
- `performance-optimization` 可以受益于性能相关 MCP tools。

要启用这些，请确保你已在 Gemini CLI 配置（`~/.gemini/config.json`）中安装相关 MCP extensions。

### Session Hooks

Gemini CLI 支持 session lifecycle hooks。你可以用它们在会话开始时自动注入上下文或运行验证 scripts。

要复刻其他工具中的 `agent-skills` 体验，可以配置一个 `SessionStart` hook，用来提醒可用 skills 或加载 meta-skill。

### 显式加载上下文

你可以在 prompt 中用 `@` 符号引用任何 skill，将其显式加载到当前会话：

```markdown
Use the @skills/test-driven-development/SKILL.md skill to implement this fix.
```

当你想确保遵循某个特定工作流，而不想等待自动发现时，这很有用。

## Slash Commands

该 repo 在 `.gemini/commands/` 下提供 8 个 slash commands：7 个生命周期 commands，以及 `/webperf` 专家审计。从项目根目录运行时，Gemini CLI 会自动发现它们。

| Command | What it does |
|---------|--------------|
| `/spec` | 写代码前编写结构化 spec |
| `/planning` | 将工作拆成小而可验证的任务 |
| `/build` | 以增量方式实现下一个任务 |
| `/test` | 运行 TDD 工作流 — red、green、refactor |
| `/review` | 五轴代码评审 |
| `/code-simplify` | 在不改变行为的前提下降低复杂度 |
| `/ship` | 通过并行 persona fan-out 执行发布前 checklist |
| `/webperf` | 审计面向浏览器的应用，检查 Core Web Vitals 和性能问题 |

每个 command 都会自动调用对应的 skill — 不需要手动加载 skill。

> **Note:** 使用 `/planning` 而不是 `/plan` — `/plan` 会与 Gemini CLI 内部 command 名称冲突。

## 使用提示

1. **优先使用 skills，而不是 GEMINI.md** — Skills 按需激活，并让上下文窗口保持聚焦。只有当你希望它们始终加载时，才把 skills 放进 GEMINI.md。
2. **Skill descriptions 很重要** — 每个 SKILL.md 都在 frontmatter 中有一个 `description` 字段，用来告诉 agents 何时激活它。本 repo 中的 descriptions 针对所有受支持工具（Claude Code、Gemini CLI 等）的自动发现做了优化，会清晰说明该 skill 做什么以及何时触发。
3. **使用 agents 做评审** — 请求结构化代码评审时，复制 `agents/code-reviewer.md` 内容。
4. **结合 references** — 处理测试或性能等具体质量领域时，引用 `references/` 中的 checklists。
