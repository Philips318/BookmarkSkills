---
title: 'Automating with Hooks'
description: 'Learn how to use hooks to automate lifecycle events like formatting, linting, and governance checks during Copilot agent sessions.'
authors:
  - GitHub Copilot Learning Hub Team
lastUpdated: 2026-06-25
estimatedReadingTime: '8 minutes'
tags:
  - hooks
  - automation
  - fundamentals
relatedArticles:
  - ./building-custom-agents.md
  - ./what-are-agents-skills-instructions.md
prerequisites:
  - Basic understanding of GitHub Copilot agents
---
Hooks允许您在Copilot代理会话期间的关键时刻运行自动化脚本-当会话开始或结束时，当用户提交提示时，或者在代理使用工具之前和之后。它们是将Copilot的AI功能与团队现有工具（linters、格式化器、治理扫描器和通知系统）连接在一起的粘合剂。

本文解释了钩子是如何工作的，如何配置它们，以及常见自动化需求的实用模式。

什么是钩子？

Hooks是shell命令或脚本，在Copilot代理会话期间响应生命周期事件自动运行。它们在AI模型之外执行，它们是确定性的，可重复的，并且在你的完全控制之下。* * * *关键特征:
钩子作为shell命令在用户的机器上运行
—它们同步执行——代理等待它们完成
-它们可以阻止操作（例如，阻止失败的提交）
-它们在JSON文件中定义，存储在存储库中的`.github/hooks/*.json`-它们通过JSON输入接收详细的上下文，从而实现上下文感知自动化
-它们可以包含用于复杂逻辑的捆绑脚本

何时使用钩子与其他自定义

用例|最佳工具||----------|-----------|
| **Hook** |
|教副驾驶你的编码标准| **指令** |
|自动化多步骤工作流| **技能**或**代理** |
|扫描敏感数据提示| **Hook** |
|提交|前格式化代码**Hook** |
|为新代码| **技能** |生成测试

钩子对于必须可靠地发生的确定性自动化来说是理想的——你不想依赖AI记住去做的事情。

##钩子的解剖

这个存储库中的每个钩子都是一个文件夹，包含：```
hooks/
└── my-hook/
    ├── README.md          # Documentation with frontmatter
    ├── hooks.json         # Hook configuration
    └── scripts/           # Optional bundled scripts
        └── check.sh
```
注意：并非所有这些文件都是泛化钩子实现所必需的。在您自己的存储库中，钩子以JSON文件的形式存储在`.github/hooks/`中（例如，`.github/hooks/my-hook.json`）。出于文档目的，上面带有README.md的文件夹结构是特定于Awesome Copilot存储库的。### hooks.json
配置定义了哪些事件触发哪些命令：```json
{
  "version": 1,
  "hooks": {
    "postToolUse": [
      {
        "type": "command",
        "bash": "npx prettier --write .",
        "cwd": ".",
        "timeoutSec": 30
      }
    ]
  }
}
```
##钩子事件

钩子可以触发几个生命周期事件：

|事件|触发|常见用例||-------|---------------|------------------|
|`sessionStart`|代理会话开始或恢复|初始化环境，日志会话启动，验证项目状态|
|`sessionEnd`|代理会话完成或终止|清理临时文件，生成报告，发送通知|
|`userPromptSubmitted`|用户提交提示|日志请求进行审计和遵从性；直接处理请求而不调用LLM (v1.0.44+)；将`additionalContext`注入模型提示符（v1.0.65+） |
|`preToolUse`|代理使用任何工具（如`bash`，`edit`）之前| **批准或拒绝**工具的执行，阻止危险命令，强制执行安全策略|
|`postToolUse`|工具**成功**完成执行后|记录结果，跟踪使用情况，编辑后格式化代码|
|`postToolUseFailure`|工具调用**失败报错** |日志错误调试，发送失败提示，跟踪错误模式|
|`PermissionRequest`|命令行界面显示“**”权限提示“**”给用户|程序自动批准或拒绝权限请求，在CI/headless环境中启用自动审批|
|`agentStop`|主代理完成响应提示|运行finallinters/formatters，验证完整更改|
|`preCompact`|代理压缩上下文窗口前|保存快照、日志压缩事件、运行摘要脚本|
|`subagentStart`|子代理由主代理生成|向子代理的提示符注入额外的上下文，日志子代理启动|
|`subagentStop`|子代理完成后返回结果|审计子代理输出，日志子代理活动|
|`errorOccurred`|代理执行过程中发生错误|调试、发送通知、跟踪错误模式|关键洞察：`preToolUse`钩子是最强大的——它可以**批准或拒绝**单个工具的执行。这支持细粒度的安全策略，例如阻止特定的shell命令或要求对敏感文件操作进行批准。

### sessionStart additionalContext`sessionStart`钩子在其输出中支持`additionalContext`字段。当钩子脚本将包含`additionalContext`键的JSON写入标准输出时，该文本将在会话开始时直接注入会话。这使得钩子可以动态地提供特定于环境的上下文——比如当前的git分支、部署环境或团队加入说明——而不需要用户手动粘贴。

显示上下文的钩子脚本示例：```bash
#!/usr/bin/env bash
# Output JSON with additionalContext to inject into the session
cat <<EOF
{
  "additionalContext": "Current branch: $(git rev-parse --abbrev-ref HEAD). Open tickets: $(gh issue list --limit 3 --json number,title | jq -r '.[] | \"#\(.number) \(.title)\"' | tr '\n' '; ')"
}
EOF
```
userPromptSubmitted additionalContext （v1.0.65+）`userPromptSubmitted`钩子还支持`additionalContext`字段。当钩子返回`{"additionalContext": "..."}`时，该文本在模型处理用户消息之前被注入到面向模型的提示符中。这与`response`字段不同（它完全绕过模型）—这里模型仍然运行，但是预先添加了额外的上下文。

这对于每个提示丰富非常有用：添加应该影响模型对特定请求的响应的当前git diff、环境状态或动态指令。```bash
#!/usr/bin/env bash
# Inject the current git status into every prompt for context awareness
INPUT=$(cat)
BRANCH=$(git rev-parse --abbrev-ref HEAD 2>/dev/null || echo "unknown")
STAGED=$(git diff --cached --stat 2>/dev/null | tail -1)

cat <<EOF
{
  "additionalContext": "Active branch: $BRANCH. Staged changes: ${STAGED:-none}."
}
EOF
```
> **工作原理**：如果钩子将`{"additionalContext": "..."}`写入标准输出，并以代码`0`退出，则该文本将被添加到此轮的模型提示符中。该钩子还可以同时写入`additionalContext`和`response`——如果存在`response`，则获胜，并跳过模型调用。

扩展钩子合并

当多个IDE扩展（或扩展和`hooks.json`文件的混合）都定义钩子时，所有的钩子定义都是合并的，而不是最后一个覆盖其他的。这意味着您可以对来自不同源的钩子进行分层—项目的`.github/hooks/`文件、已安装的扩展名和个人设置文件—所有这些都将为相关事件触发。

跨平台事件名称兼容性钩子事件的名称可以写成**camelCase**（如`preToolUse`）或**PascalCase**（如`PreToolUse`）。两者都被接受，使得钩子配置文件在GitHub CopilotCLI、VS Code和Claude Code之间兼容，而无需修改。除了标准平面格式外，Hooks还支持Claude Code的嵌套`matcher`/`hooks`结构。

插件挂钩环境变量

当钩子在插件中定义时，钩子脚本会自动接收两个额外的环境变量：

|变量|描述||----------|-------------|
|`CLAUDE_PROJECT_DIR`|当前工程（工作）目录|的路径
|`CLAUDE_PLUGIN_DATA`|作用域为|插件的持久数据目录路径

你也可以直接在`hooks.json`配置的`bash`或`powershell`字段中使用这些模板变量：```json
{
  "version": 1,
  "hooks": {
    "sessionStart": [
      {
        "type": "command",
        "bash": "{{plugin_data_dir}}/scripts/init.sh --project {{project_dir}}",
        "timeoutSec": 10
      }
    ]
  }
}
```
这使得编写可在不同机器和项目之间移植的插件挂钩变得简单，而无需硬编码路径。

###事件配置

钩子支持两种类型：`"command"`用于运行本地shell脚本，`"http"`用于向URL发送JSON有效负载。

#### Shell命令钩子（`type: "command"`）```json
{
  "type": "command",
  "bash": "./scripts/my-check.sh",
  "powershell": "./scripts/my-check.ps1",
  "matcher": "^bash$",
  "cwd": ".",
  "timeoutSec": 10,
  "env": {
    "CUSTOM_VAR": "value"
  }
}
```
**type**:`"command"`用于基于shell的钩子。

**bash**：在Unix系统上执行的命令或脚本。可以内联或引用脚本文件。

**powershell**：在Windows系统上执行的命令或脚本。必须提供`bash`或`powershell`（或两者都提供）。

**matcher** *（可选）*：一个匹配工具名称的正则表达式。当出现该钩子时，该钩子只对名称与正则表达式完全匹配的工具触发。例如，`"^bash$"`确保钩子仅为`bash`工具运行，而不是为`edit`或其他工具运行。这对于希望针对特定工具的`preToolUse`和`postToolUse`钩子特别有用。b> **重要(v1.0.36+)**：在v1.0.36之前，`matcher`字段被静默地忽略了-为所有工具调用触发带有`matcher`的钩子，而不考虑正则表达式。升级到v1.0.36或更高版本后，只有名称与`matcher`正则表达式完全匹配的工具调用才会触发该钩子。检查使用`matcher`的任何现有`preToolUse`/`postToolUse`钩子，确保它们仍按预期发射。

b> **修复(v1.0.63+)**：一个bug导致`postToolUse`匹配器使用管道分离模式（例如，`"matcher": "Edit|Write"`）被无声地丢弃，因此针对多个工具的钩子被错误地触发所有工具调用。在v1.0.63中修复了这个问题-`postToolUse`匹配器现在可以正常工作了。如果依赖于在特定工具之后运行的格式化程序或筛选器，请升级到v1.0.63或更高版本，以确保它仅在需要时触发。

**cwd**：命令的工作目录（相对于存储库根目录）。**timeoutSec**：以秒为单位的最大执行时间（默认为30）。如果超过这个限制，钩子将被杀死。

**env**：与现有环境合并的附加环境变量。

#### HTTP钩子（`type: "http"`）

HTTP不再运行本地脚本，而是将JSON有效负载hook POST到配置的URL。这对于与webhook、通知系统或远程审计服务集成非常有用，而不需要在每台机器上安装本地脚本。```json
{
  "type": "http",
  "url": "https://your-server.example.com/hooks/copilot",
  "timeoutSec": 10
}
```
钩子发送一个HTTP POST请求，该请求带有命令钩子通过stdin接收的相同的JSON上下文（工具名称、工具输入、会话信息等）。如果服务器以非2xx状态响应，则该钩子被视为失败。

**url**：要POST JSON有效负载的url。

**timeoutSec**：等待HTTP响应的最大时间（默认为30秒）。

HTTP钩子是一种轻量级的方式，可以将钩子事件分散到集中的日志或治理服务，而无需将脚本分发到每个开发人员的机器上。### README.md
README提供了Awesome Copilot存储库的元数据和文档。虽然在您自己的实现中不需要，但它可以作为为您的团队记录它们的有用方法。```markdown
---
name: 'Auto Format'
description: 'Automatically formats code using project formatters before commits'
tags: ['formatting', 'code-quality']
---

# Auto Format

Runs your project's configured formatter (Prettier, Black, gofmt, etc.)
automatically before the agent commits changes.

## Setup

1. Ensure your formatter is installed and configured
2. Copy the hooks.json to your `.github/hooks/` directory
3. Adjust the formatter command for your project
```
##实际例子

在CI中使用PermissionRequest自动审批权限

当CLI向用户显示权限提示时，`PermissionRequest`钩子会触发—例如，当代理第一次想要运行shell命令时。不像`preToolUse`（它可以阻止特定的工具调用），`PermissionRequest`拦截权限批准UI本身，使其非常适合没有人可以点击“允许”的无头和CI环境。

b> **基于位置的持久性(v1.0.37+)**：默认情况下，权限批准现在按目录持久化-一旦您批准了给定工作目录的权限，该批准将延续到在同一目录中启动的未来会话。您不再需要每次都重新批准相同的工具。使用`PermissionRequest`钩子来自动化CI中的审批，并依赖于交互式本地会话的持久审批。当钩子脚本以代码`0`退出时，权限请求被**批准**。使用非零代码退出以**拒绝**它（用户仍然会看到提示）。```json
{
  "version": 1,
  "hooks": {
    "PermissionRequest": [
      {
        "type": "command",
        "bash": "./scripts/ci-permission-policy.sh",
        "cwd": ".",
        "timeoutSec": 5
      }
    ]
  }
}
```
在CI中运行时自动批准所有权限的策略脚本示例：```bash
#!/usr/bin/env bash
# scripts/ci-permission-policy.sh
# Auto-approve all permission requests in CI environments
if [ "${CI}" = "true" ]; then
  exit 0   # approve
fi
exit 1     # deny (let the user decide interactively)
```
> **安全提示**:`PermissionRequest`钩子使用时要小心。非ci环境中的全面自动审批取消了一项重要的安全检查。精确地限定自动审批逻辑的范围（例如，仅在CI中，仅针对特定的工具）。b> **提示模式安全性(v1.0.40+)**：当以**提示模式** (`copilot -p "..."`) - CI管道中常用的非交互模式运行CLI时，为了安全起见，默认**禁用**回购钩子。要在提示模式下选择回购钩子，在运行命令之前设置环境变量`GITHUB_COPILOT_PROMPT_MODE_REPO_HOOKS=true`：
>“bash
> GITHUB_COPILOT_PROMPT_MODE_REPO_HOOKS=true copilot -p “…”——no-ask-user
> ' ' '
>这是一个默认安全的更改：当用户在不熟悉的存储库中运行快速提示命令时，它可以防止不受信任的存储库钩子静默触发。同样，默认情况下，工作区MCP服务器在提示模式下是禁用的；选择加入`GITHUB_COPILOT_PROMPT_MODE_WORKSPACE_MCP=true`。扩展遵循混合模型（v1.0.41+）： **用户级扩展**（来自`~/.copilot/`）在提示模式下自动加载，但**项目级扩展和管理工具**在默认情况下是禁用的-选择`GITHUB_COPILOT_PROMPT_MODE_EXTENSIONS=true`来加载它们。使用posttoolusfailure处理工具故障`postToolUseFailure`钩子在工具调用失败并出现错误时触发——与`postToolUse`不同，后者只在成功时触发。使用它来记录错误，发送失败警报或实现重试逻辑：```json
{
  "version": 1,
  "hooks": {
    "postToolUseFailure": [
      {
        "type": "command",
        "bash": "./scripts/notify-tool-failure.sh",
        "cwd": ".",
        "timeoutSec": 10
      }
    ]
  }
}
```
钩子接收JSON输入，描述哪个工具失败以及错误消息。这种分离使您可以编写有针对性的故障处理逻辑，而无需向`postToolUse`钩子添加条件检查。

> **注**：在v1.0.15之前，`postToolUse`会为成功和失败的工具调用触发。如果您有处理故障的`postToolUse`钩子，请将该逻辑迁移到`postToolUseFailure`。

编辑后自动格式化

确保在代理编辑文件后格式化所有代码：```json
{
  "version": 1,
  "hooks": {
    "postToolUse": [
      {
        "type": "command",
        "bash": "npx prettier --write . && git add -A",
        "cwd": ".",
        "timeoutSec": 30
      }
    ]
  }
}
```
当代理完成时进行Lint检查

在代理完成响应后运行ESLint，如果有错误则阻塞：```json
{
  "version": 1,
  "hooks": {
    "agentStop": [
      {
        "type": "command",
        "bash": "npx eslint . --max-warnings 0",
        "cwd": ".",
        "timeoutSec": 60
      }
    ]
  }
}
```
如果lint命令以非零状态退出，则该操作被阻塞。

使用preToolUse进行安全控制

在危险命令执行之前阻止它们。使用`matcher`字段只针对`bash`工具，因此钩子不会为文件编辑或其他工具触发：```json
{
  "version": 1,
  "hooks": {
    "preToolUse": [
      {
        "type": "command",
        "matcher": "^bash$",
        "bash": "./scripts/security-check.sh",
        "cwd": ".",
        "timeoutSec": 15
      }
    ]
  }
}
```
`preToolUse`钩子接收JSON输入，其中包含被调用工具的详细信息。你的脚本可以用一个非零的代码来检查这个输入和退出，以**拒绝**工具的执行，或者用零来退出，以**批准**它。

使用preToolUse修改工具参数

除了approve/deny之外，`preToolUse`钩子还可以在工具参数传递给工具之前修改工具参数，并在代理的推理中注入额外的上下文。要做到这一点，从钩子脚本中写入JSON到stdout：```bash
#!/usr/bin/env bash
# scripts/sanitize-bash-args.sh
#
# Reads the proposed bash command from stdin, strips dangerous flags,
# and writes back the sanitized command as modifiedArgs.

INPUT=$(cat)
COMMAND=$(echo "$INPUT" | jq -r '.tool_input.command // empty')

# Strip the --no-sandbox flag if present
SAFE_COMMAND=$(echo "$COMMAND" | sed 's/--no-sandbox//g')

echo "{\"modifiedArgs\": {\"command\": \"$SAFE_COMMAND\"}, \"additionalContext\": \"Command was sanitized by security policy.\"}"
```
输出字段为：

|字段|描述||-------|-------------|
|`modifiedArgs`（或`updatedInput`） |替换工具参数。这些是用来代替原件的。|
|`additionalContext`|为这一回合注入代理上下文的文本—用于解释为什么进行了更改。|

这支持复杂的模式，如规范化文件路径、强制命名约定、添加所需的标志或显示策略上下文，而不会完全阻塞工具。

**注**:`modifiedArgs`和`updatedInput`都是可接受的替换参数字段名（为了跨工具兼容性）。

治理审计

扫描潜在安全威胁的用户提示并记录会话活动：```json
{
  "version": 1,
  "hooks": {
    "sessionStart": [
      {
        "type": "command",
        "bash": ".github/hooks/governance-audit/audit-session-start.sh",
        "cwd": ".",
        "timeoutSec": 5
      }
    ],
    "userPromptSubmitted": [
      {
        "type": "command",
        "bash": ".github/hooks/governance-audit/audit-prompt.sh",
        "cwd": ".",
        "env": {
          "GOVERNANCE_LEVEL": "standard",
          "BLOCK_ON_THREAT": "false"
        },
        "timeoutSec": 10
      }
    ],
    "sessionEnd": [
      {
        "type": "command",
        "bash": ".github/hooks/governance-audit/audit-session-end.sh",
        "cwd": ".",
        "timeoutSec": 5
      }
    ]
  }
}
```
此模式对于需要审核AI交互以确保遵从性的企业环境非常有用。

直接使用userPromptSubmitted处理请求（v1.0.44+）

从v1.0.44开始，`userPromptSubmitted`钩子可以做的不仅仅是记录或阻塞——它们可以**完全处理请求**，在不进行任何模型调用的情况下向用户返回响应。当钩子脚本将带有`response`字段的JSON对象写入stdout时，CLI将该文本传递给用户，并完全跳过LLM。

这是有用的：
- **FAQ bots**：在没有花费模型配额的情况下返回常见问题的罐装答案
- **策略实施**：用清晰、一致的消息拒绝超出范围的提示
- **重定向模式**：将用户定向到特定的资源或runbook```json
{
  "version": 1,
  "hooks": {
    "userPromptSubmitted": [
      {
        "type": "command",
        "bash": "./scripts/prompt-router.sh",
        "timeoutSec": 5
      }
    ]
  }
}
```
处理`/help`前缀而不调用模型的示例脚本：```bash
#!/usr/bin/env bash
# scripts/prompt-router.sh
# Return a direct response for known help queries — no LLM call needed.

INPUT=$(cat)
PROMPT=$(echo "$INPUT" | jq -r '.prompt // empty' | tr '[:upper:]' '[:lower:]')

if echo "$PROMPT" | grep -q "^/help\b"; then
  echo '{"response": "Available commands: /generate-tests, /review-pr, /explain-architecture. Type /help <command> for details."}'
  exit 0
fi

# No match — let the LLM handle it normally (exit 0 without writing a response)
exit 0
```
> **工作原理**：当钩子退出代码`0`**和**写入一个有效的`{"response": "..."}`JSON对象到stdout， CLI将该文本传递给用户并停止处理-没有模型调用。如果钩子以代码`0`退出，但没有写入任何内容（或没有写入`response`键），则CLI将正常运行并调用LLM。

> **多个钩子**：如果配置了多个`userPromptSubmitted`钩子，第一个返回`response`的将胜出；该事件的后续钩子将被跳过。

会话结束通知

当座席会话完成时，发送Slack或Teams通知：```json
{
  "version": 1,
  "hooks": {
    "sessionEnd": [
      {
        "type": "command",
        "bash": "curl -X POST \"$SLACK_WEBHOOK_URL\" -H 'Content-Type: application/json' -d '{\"text\": \"Copilot agent session completed\"}'",
        "cwd": ".",
        "env": {
          "SLACK_WEBHOOK_URL": "${input:slackWebhook}"
        },
        "timeoutSec": 5
      }
    ]
  }
}
```
通过HTTP钩子通知会话结束

使用HTTP钩子将会话活动发送到远程审计端点（不需要本地脚本）：```json
{
  "version": 1,
  "hooks": {
    "sessionEnd": [
      {
        "type": "http",
        "url": "https://audit.example.com/copilot-sessions",
        "timeoutSec": 5
      }
    ]
  }
}
```
CLI将会话上下文作为JSON发送到指定的URL。这对于集中式日志记录或遵从性服务来说是理想的，这些服务应该接收来自所有开发人员的事件，而不需要每个人都安装本地脚本。

向子代理注入上下文

当主代理生成子代理（例如，通过`task`工具）时，`subagentStart`钩子会被触发。使用它将额外的上下文（例如项目约定或安全指南）直接注入到子代理的提示符中：```json
{
  "version": 1,
  "hooks": {
    "subagentStart": [
      {
        "type": "command",
        "bash": "echo 'Follow the team coding standards in .github/instructions/ for all code changes.'",
        "cwd": ".",
        "timeoutSec": 5
      }
    ]
  }
}
```
这在子代理可能不会自动从父会话继承上下文的多代理工作流中特别有用。

插件钩子环境变量

当钩子在插件中定义时，Copilot CLI会自动注入两个额外的环境变量，以便脚本可以定位特定于项目和特定于插件的目录：

|变量|描述||----------|-------------|
|`CLAUDE_PROJECT_DIR`|工作项目目录|的绝对路径
|`CLAUDE_PLUGIN_DATA`|插件持久数据目录|的绝对路径

你也可以在钩子配置中引用这些路径作为模板变量：```json
{
  "version": 1,
  "hooks": {
    "postToolUse": [
      {
        "type": "command",
        "bash": "{{plugin_data_dir}}/scripts/format.sh {{project_dir}}",
        "timeoutSec": 30
      }
    ]
  }
}
```
这对于绑定脚本或数据文件的插件非常有用，因为无论插件安装在哪里，`{{plugin_data_dir}}`总是指向正确的安装位置。

##编写钩子脚本

对于复杂的逻辑，使用捆绑脚本代替内联bash命令：```bash
#!/usr/bin/env bash
# scripts/pre-commit-check.sh
set -euo pipefail

echo "Running pre-commit checks..."

# Format code
npx prettier --write .

# Run linter
npx eslint . --fix

# Run type checker
npx tsc --noEmit

# Stage any formatting changes
git add -A

echo "Pre-commit checks passed ✅"
```
**关于钩子脚本的提示**：
—使用`set -euo pipefail`来快速处理错误
保持脚本集中——每个脚本一个职责
—使脚本可执行：`chmod +x scripts/pre-commit-check.sh`—在将脚本添加到hooks.json之前，手动测试脚本
-使用合理的超时时间-格式化大型代码库可能需要30秒以上

最佳实践- **保持钩子快速**：钩子同步运行，所以缓慢的钩子延迟代理。设置严格的超时并优化脚本。
—**使用非零退出码来阻塞**：如果钩子以非零退出码退出，则触发动作被阻塞。将此用于必须通过的检查。
- **在hook文件夹中的捆绑脚本**：将相关脚本与hooks.json一起保存以实现可移植性。
- **文档设置要求**：如果钩子依赖于正在安装的工具（Prettier, ESLint），请在README中记录。
**先在本地测试**：在代理会话中依赖钩子脚本之前手动运行钩子脚本。
- **层钩子，不要重载**：使用多个钩子条目进行独立检查，而不是一个单一的脚本。

##常见问题

**Q：我把钩子配置文件放在哪里？**

答：有几个支持的位置，按优先顺序加载：- **储存库级别**（与团队共享）：`.github/hooks/*.json`在您的储存库-所有JSON文件在这个文件夹将自动加载
**Claude/Copilot项目设置**:`.claude/settings.json`和`.claude/settings.local.json`-这里定义的钩子应用于当前存储库，而不提交到`.github/`- **全局设置**:`settings.json`或`settings.local.json`（用户级CLI配置）
- **传统配置**:`config.json`（也支持）

对于每个人都应该使用的团队范围的钩子，`.github/hooks/`是推荐的位置，因为它是版本控制和自动共享的。

**Q：钩子可以访问用户的提示文本吗？**是的。对于`userPromptSubmitted`事件，提示内容可以通过JSON输入到钩子脚本中。从v1.0.44开始，这些钩子也可以通过将`{"response": "..."}`写入stdout来直接响应—CLI将该文本传递给用户并完全跳过LLM。其他钩子，如`preToolUse`和`postToolUse`，接收关于被调用工具的上下文。请参阅[GitHub Copilot钩子文档]（https://docs.github.com/en/copilot/concepts/agents/coding-agent/about-hooks）了解详细信息。

**Q：如果钩子超时会发生什么？**

A：钩子被终止，代理继续。为脚本设置适当的`timeoutSec`。

**Q：我可以有多个钩子为同一事件？**

是的。同一事件的钩子按照它们在数组中出现的顺序运行。如果任何钩子失败（非零退出），该事件的后续钩子可能会被跳过。

**Q：钩子与副驾驶编码代理一起工作吗？**是的。钩子对于编码代理来说特别有价值，因为它们为自主操作提供了确定性的保护。详见[使用副驾驶编码代理]（../using-copilot-coding-agent/）。

##下一步

- **探索示例**：浏览[Hooks目录]（../../hooks/）以获取准备使用的钩子配置
- **构建代理**:[构建自定义代理](../building-custom-agents/) -创建代理补充钩子
- **自动化进一步**:[使用副驾驶编码代理](../using-copilot-coding-agent/) -运行钩子在自主代理会话

---