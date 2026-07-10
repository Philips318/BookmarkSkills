# 在 Antigravity CLI (agy) 中使用 agent-skills

`agent-skills` 包可以作为原生 plugin 安装到 Antigravity CLI（`agy`）中，让 agent 能够访问结构化工作流、personas 和自定义 slash commands。

## 设置

### 选项 1：原生 Plugin 安装（推荐）

Antigravity CLI 拥有一等 plugin 系统，可以注册 skills、agents 和自定义 commands。

**从远程仓库安装：**

```bash
agy plugin install https://github.com/addyosmani/agent-skills.git
```

**从本地 clone 安装：**

1. Clone 仓库：
   ```bash
   git clone https://github.com/addyosmani/agent-skills.git
   ```
2. 使用 `agy` 安装 plugin：
   ```bash
   agy plugin install /path/to/agent-skills
   ```

这会验证 plugin，并将其安装到你的全局 Antigravity 配置目录（`~/.gemini/antigravity-cli/plugins/agent-skills/`）。

### 选项 2：从 Gemini CLI 导入

如果你已经在旧版 Gemini CLI 安装位置下安装了 `agent-skills`，可以直接导入：
```bash
agy plugin import gemini
```

安装完成后，验证当前启用的 plugin：
```bash
agy plugin list
```

---

## Slash Commands

该 plugin 注册了 8 个自定义 slash commands：7 个生命周期 commands，以及 `/webperf` 专家审计：

| Command | What it does | Activated Skill |
|---------|--------------|-----------------|
| `/spec` | 写代码前编写结构化 spec | `spec-driven-development` |
| `/planning` | 将工作拆成小而可验证的任务 | `planning-and-task-breakdown` |
| `/build` | 以增量方式实现下一个任务 | `incremental-implementation` |
| `/test` | 运行 TDD 工作流 — red、green、refactor | `test-driven-development` |
| `/review` | 五轴代码评审 | `code-review-and-quality` |
| `/code-simplify` | 在不改变行为的前提下降低复杂度 | `code-simplification` |
| `/ship` | 通过并行 persona fan-out 执行发布前 checklist | `shipping-and-launch` |
| `/webperf` | 审计面向浏览器的应用，检查 Core Web Vitals 和性能问题 | `web-performance-auditor` |

每个 command 都会自动调用对应的 skill，并逐步引导 agent。

> **Note:** 使用 `/planning` 而不是 `/plan`，以避免与 Antigravity 内部的 plan 生成 command 冲突。

---

## Skills & Discovery

Antigravity 会自动发现 plugin 的 `skills/` 目录中的 skills。
* Antigravity 会按需将用户任务和意图匹配到相关 skills。
* 如果任务匹配某个 skill，agent 会加载该 skill，并在执行前请求你的许可。

---

## Verification & Validation

要验证你的本地 plugin 结构正确且包含所有 skills，请运行：
```bash
agy plugin validate /path/to/agent-skills
```

---

## 工作原理

### 1. 按需激活 Skill
Antigravity CLI 会自动发现已安装 plugin 的 `skills/` 目录中的 `SKILL.md` 文件。借助每个 skill frontmatter 中的触发描述，agent 会在检测到匹配的开发者意图时动态激活合适的工作流。

例如，当你要求 agent：
- **Design a new system** &rarr; 它会建议/激活 `spec-driven-development`。
- **Implement a feature** &rarr; 它会激活 `incremental-implementation` 和 `test-driven-development`。
- **Fix a bug** &rarr; 它会激活 `debugging-and-error-recovery`。

### 2. 专门的 Agent Personas
该 plugin 会从 `agents/` 目录注册可复用的 subagent 定义：
- `code-reviewer.md`
- `security-auditor.md`
- `test-engineer.md`

你可以在会话中直接调用这些 personas，也可以在使用 subagents 委派任务时调用它们。

---

## 配置与自定义

### 项目特定强制规则（`AGENTS.md`）
要强制严格遵守 skill（例如要求先写 spec 或 plan 再写代码），请将 `AGENTS.md` 复制或链接到工作区根目录。Antigravity CLI 会读取此文件，使 agent 的行为和规划阶段与你团队的约定保持一致。

### Sandbox Mode
如果你想以受限终端权限运行 skills 或 scripts（在运行第三方验证测试时更安全），请用以下方式启动 CLI：

```bash
agy --sandbox
```

---

## 使用提示

1. **保持 plugins 最新：** 你可以使用以下命令更新 CLI 或检查是否有新的 plugin 版本：
   ```bash
   agy update
   ```
2. **执行前评审：** 当 agents 使用这些 skills 执行复杂重构任务时，使用 `Ctrl+r` 进入 **Artifact Review** 屏幕，以便在代码提交前进行评审、编辑或批准。
3. **控制权限：** 只有在可信的本地项目中、且你希望绕过手动工具批准提示时，才使用 `--dangerously-skip-permissions` 标志。
