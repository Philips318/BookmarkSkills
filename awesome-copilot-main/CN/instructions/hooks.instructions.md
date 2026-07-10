---
description: 'Portable guidance for authoring safe, fast, and clear hooks and reusable hook examples'
applyTo: '.github/hooks/**, hooks/**'
---
# Hook创作指南

钩子是在特定生命周期事件中运行的小的、确定性的命令或脚本。
一个出色的钩子只完成一个明确的任务，快速运行，并使其副作用明确。

##文件夹结构GitHub Copilot钩子位于存储库中的`.github/hooks/`中：```text
.github/
└── hooks/
    ├── block-dangerous-commands.json   ← hook config (which event, which script, options)
    └── scripts/
        ├── block-dangerous-commands.sh  ← Bash implementation
        └── block-dangerous-commands.ps1 ← PowerShell implementation (optional if Bash-only)
```
您可以有多个`.json`文件—每个文件都为一个或多个事件注册钩子。主机加载所有这些文件。

##配置文件

每个`.json`文件将事件映射到一个钩子项数组。

—**Command hooks** (`type: "command"`)：运行本地脚本。主机在标准输出上传递事件JSON，脚本通过退出代码和标准输出进行响应。

###配置示例```json
{
  "version": 1,
  "hooks": {
    "preToolUse": [
      {
        "matcher": "bash",
        "type": "command",
        "bash": "./.github/hooks/scripts/block-dangerous-commands.sh",
        "powershell": "./.github/hooks/scripts/block-dangerous-commands.ps1",
        "cwd": ".",
        "timeoutSec": 5,
        "env": {
          "BLOCK_MODE": "deny"
        }
      }
    ]
  }
}
```
配置字段

|字段|必选|它做什么|| ---- | ---- | ---- |
|`type`|是|`"command"`脚本|
|`matcher`| no |主机级过滤器-钩子只在工具名称匹配此值时触发（例如`"bash"`，`"powershell"`,`"edit"`,`"create"`）。本地验证工作在Copilot CLI v1.0.36；尚未在回购钩子示例中使用。|
|`bash`|一个或两个|在Unix /支持bash的主机上调用的命令行|
|`powershell`|一个或两个|在Windows / powershell主机上调用的命令行|
|`cwd`| no |工作目录，相对于repo根|
|`timeoutSec`| no |主机终止进程前的最大秒数（默认为30秒）
|`env`| no |传递给脚本|的额外进程环境变量

为什么匹配器很重要

如果没有匹配器，每个`preToolUse`钩子都会在每次工具调用时触发。你的脚本开始的样板文件如下：```bash
tool_name="$(printf '%s' "$payload" | jq -r '.toolName')"
[[ "$tool_name" != "bash" ]] && exit 0
```
有了匹配器，主机就会为您进行筛选——没有样板文件，也不会为不相关的工具生成进程。一旦功能稳定下来，这可能会成为标准模式。

如果钩子必须同时在CLI和云代理（或旧的CLI版本）上工作，那么即使在使用匹配器时，也要保留脚本内过滤作为后备。

###`env`-脚本的静态配置`env`是**标准主机字段**。其中的键是作者定义的变量——您选择名称和值。

它们作为进程环境变量到达，而不是在stdin JSON有效负载中。将它们用于不应该硬编码的静态配置：

|模式|示例|| ---- | ---- |
|模式标志|`"BLOCK_MODE": "deny"`-相同的脚本日志在一个repo，块在另一个|
|阈值|`"MAX_CHANGED_FILES": "20"`|
|路径|`"AUDIT_LOG_PATH": ".github/logs/hooks.log"`|
|特性开关|`"ENABLE_NOTIFICATIONS": "false"`|`bash`和`powershell`-何时提供一个或两个

主机选择与当前环境匹配的条目。它不会跑到两者，也不会从其中一个退回到另一个。

|情况|提供|| ---- | ---- |
|私有钩子，一个已知平台|只有该平台的入口|
|已发布的钩子声明跨平台支持|两个条目|
|单个跨平台运行时（Python, Node, pwsh） |通过两个条目公开相同的脚本|
| Bash-only依赖|`bash`仅|
|仅windows依赖|`powershell`仅|

通过两个条目使用Python的跨平台示例：```json
{
  "type": "command",
  "bash": "python3 ./.github/hooks/scripts/check.py",
  "powershell": "python .\\.github\\hooks\\scripts\\check.py"
}
```
##脚本契约

每个钩子脚本都遵循相同的基本契约：从标准输入读取JSON，执行工作，然后通过退出代码、标准输出和标准错误进行响应。

**重要**:`toolArgs`是一个**JSON字符串**，而不是一个嵌套对象。必须对其进行第二次解析才能访问其字段。

###读取stdin和响应- Bash和PowerShell

* * Bash * *:```bash
#!/usr/bin/env bash
set -euo pipefail
payload="$(cat)"
tool_name="$(printf '%s' "$payload" | jq -r '.toolName')"
tool_args="$(printf '%s' "$payload" | jq -r '.toolArgs')"
command="$(printf '%s' "$tool_args" | jq -r '.command // ""')"
```
* * PowerShell * *:```powershell
Set-StrictMode -Version Latest
$payload = [Console]::In.ReadToEnd() | ConvertFrom-Json
$toolArgs = $payload.toolArgs | ConvertFrom-Json
$command = $toolArgs.command
```
在`preToolUse`（PowerShell）中禁用：```powershell
@{ permissionDecision = 'deny'; permissionDecisionReason = 'Blocked by policy' } |
    ConvertTo-Json -Compress
exit 0
```
脚本接收到的内容

输入|它携带什么|| ---- | ---- |
|`stdin`|描述当前事件|的一个JSON有效负载
|进程环境|正常的环境变量加上配置|中`env`下定义的任何环境变量
|工作目录|`cwd`从配置，或主机默认|

###脚本如何响应

|通道|用途|| ---- | ---- |
| exit`0`|脚本成功-主机继续运行，除非stdout带有结构化的拒绝|
|非零exit | **阻塞触发动作**，发出钩子失败|信号
|`stdout`|结构化的机器可读输出—仅用于记录标准输出模式（如`preToolUse`）的事件|
|`stderr`|可读的日志诊断|

###退出代码和拒绝：全图

拒绝机制取决于事件：

|事件类型|允许|禁止/阻止|| ---- | ---- | ---- |
|`preToolUse`| exit`0`， empty或`{"permissionDecision":"allow"}`on stdout | **首选**:exit`0`+`{"permissionDecision":"deny","permissionDecisionReason":"..."}`on stdout -给主机一个显示的理由。**也适用于**：非零退出阻塞工具调用，但没有结构化的原因。|
|`userPromptSubmitted`| exit`0`|非零退出阻止提示符（此事件忽略stdout） |
|`agentStop`| exit`0`|非零exit阻断|动作
|其他事件（`sessionStart`、`sessionEnd`、`postToolUse`、`errorOccurred`） | exit`0`|非零退出信号失败；主机可以跳过该事件|的后续钩子**经验法则**：如果事件有结构化的标准输出模式（如`preToolUse`），使用它——它给出了一个清晰的理由，并且是官方文档的拒绝路径。对于没有结构化标准输出的事件，非零退出是实用的块机制——回购示例和学习中心文档证实了这一点，尽管官方GitHub参考没有明确记录“非零=块”作为合同保证。

例1：提交门阻塞提交，直到lint、类型和测试通过

**为什么此模式很重要**：拒绝原因包括实际的错误，所以代理看到什么是坏的，并在再次尝试之前修复它。这创造了一个自我纠正的反馈循环——这是钩子所能做的最强大的事情。

**事件**:`preToolUse`-在代理运行`git commit`之前触发

**配置** -`.github/hooks/commit-gate.json`：```json
{
  "version": 1,
  "hooks": {
    "preToolUse": [
      {
        "type": "command",
        "bash": "./.github/hooks/scripts/commit-gate.sh",
        "cwd": ".",
        "timeoutSec": 120
      }
    ]
  }
}
```
**脚本** -`.github/hooks/scripts/commit-gate.sh`：```bash
#!/usr/bin/env bash
set -euo pipefail

payload="$(cat)"
tool_name="$(printf '%s' "$payload" | jq -r '.toolName')"

# Only gate bash commands that are git commits
if [[ "$tool_name" != "bash" ]]; then exit 0; fi
command="$(printf '%s' "$payload" | jq -r '.toolArgs' | jq -r '.command // ""')"
if ! printf '%s' "$command" | grep -q "git commit"; then exit 0; fi

CWD="$(printf '%s' "$payload" | jq -r '.cwd')"
ERRORS=""

# 1. TypeScript type check
if [[ -f "$CWD/tsconfig.json" ]]; then
  TSC_OUT=$(cd "$CWD" && npx tsc --noEmit 2>&1) || ERRORS="${ERRORS}
=== TypeScript Errors ===
$(echo "$TSC_OUT" | head -30)"
fi

# 2. Lint
if [[ -f "$CWD/package.json" ]]; then
  HAS_LINT=$(jq -r '.scripts.lint // empty' "$CWD/package.json" 2>/dev/null)
  if [[ -n "$HAS_LINT" ]]; then
    LINT_OUT=$(cd "$CWD" && npm run lint --silent 2>&1) || ERRORS="${ERRORS}
=== Lint Errors ===
$(echo "$LINT_OUT" | tail -30)"
  fi

  # 3. Tests
  HAS_TEST=$(jq -r '.scripts.test // empty' "$CWD/package.json" 2>/dev/null)
  if [[ -n "$HAS_TEST" ]]; then
    TEST_OUT=$(cd "$CWD" && CI=true npm test -- --watchAll=false 2>&1) || ERRORS="${ERRORS}
=== Test Failures ===
$(echo "$TEST_OUT" | tail -30)"
  fi
fi

if [[ -n "$ERRORS" ]]; then
  jq -nc --arg reason "Cannot commit — fix these issues first:
$ERRORS" \
    '{permissionDecision:"deny",permissionDecisionReason:$reason}'
fi
exit 0
```
**运行时发生的事情：**

|场景| stdout | exit |主机动作|| ---- | ---- | ---- | ---- |
|所有检查通过|空|`0`|提交继续|
| Lint失败|`{"permissionDecision":"deny","permissionDecisionReason":"Cannot commit — fix these issues first:\n=== Lint Errors ===\n..."}`|`0`|阻塞提交；代理看到错误并修复它们|
| jq missing | empty |非零|钩子失败|

###示例2：文件编辑后自动格式化

为什么这种模式很重要：代理编写代码，然后格式化程序立即运行-不需要手动步骤。代理对该文件的下一次读取看到的是格式化后的版本。

事件**:`postToolUse`-在`edit`或`create`工具调用后触发

**Config** -`.github/hooks/format-on-save.json`：```json
{
  "version": 1,
  "hooks": {
    "postToolUse": [
      {
        "type": "command",
        "bash": "./.github/hooks/scripts/format-on-save.sh",
        "cwd": ".",
        "timeoutSec": 15
      }
    ]
  }
}
```
**脚本** -`.github/hooks/scripts/format-on-save.sh`：```bash
#!/usr/bin/env bash
set -euo pipefail

payload="$(cat)"
tool_name="$(printf '%s' "$payload" | jq -r '.toolName')"
result_type="$(printf '%s' "$payload" | jq -r '.toolResult.resultType // ""')"

# Only format after successful file writes
case "$tool_name" in
  edit|create) ;;
  *) exit 0 ;;
esac
[[ "$result_type" != "success" ]] && exit 0

file_path="$(printf '%s' "$payload" | jq -r '.toolArgs' | jq -r '.path // ""')"
[[ -z "$file_path" || ! -f "$file_path" ]] && exit 0

# Run the project's formatter — adapt to your stack
if command -v npx >/dev/null 2>&1 && [[ -f "package.json" ]]; then
  npx prettier --write "$file_path" 2>/dev/null || true
elif command -v dotnet >/dev/null 2>&1 && [[ "$file_path" == *.cs ]]; then
  dotnet format --include "$file_path" 2>/dev/null || true
fi
exit 0
```
**运行时发生的事情：**

|场景|钩子做什么|退出|| ---- | ---- | ---- |
|运行`prettier --write src/app.ts`|`0`|
|代理运行`bash ls`|跳过（不是文件写入工具）|`0`|
|静默跳过格式化|`0`|

###示例3：使用结构化deny阻止危险命令

**为什么这个模式很重要**：最简单的护栏-防止破坏性的shell命令在执行之前，有一个明确的原因，代理可以读取。

事件**:`preToolUse`-在任何工具调用之前触发

**Config** -`.github/hooks/block-dangerous.json`：```json
{
  "version": 1,
  "hooks": {
    "preToolUse": [
      {
        "type": "command",
        "bash": "./.github/hooks/scripts/block-dangerous.sh",
        "cwd": ".",
        "timeoutSec": 5,
        "env": {
          "BLOCK_MODE": "deny"
        }
      }
    ]
  }
}
```
**脚本** -`.github/hooks/scripts/block-dangerous.sh`：```bash
#!/usr/bin/env bash
set -euo pipefail

payload="$(cat)"
block_mode="${BLOCK_MODE:-log}"
tool_name="$(printf '%s' "$payload" | jq -r '.toolName')"

[[ "$tool_name" != "bash" ]] && exit 0

command="$(printf '%s' "$payload" | jq -r '.toolArgs' | jq -r '.command // ""')"

if printf '%s' "$command" | grep -qE 'rm -rf /|git reset --hard|git clean -fd|git push.*--force'; then
  # Truncate command to avoid leaking secrets in deny reason or logs
  short_cmd="$(printf '%.80s' "$command")"
  if [[ "$block_mode" == "deny" ]]; then
    jq -cn --arg reason "Destructive command blocked: ${short_cmd}..." \
      '{permissionDecision:"deny",permissionDecisionReason:$reason}'
  else
    echo "Would block: ${short_cmd}..." >&2
  fi
fi
exit 0
```
**运行时发生的事情：**

|场景| BLOCK_MODE | stdout | exit |主机动作|| ---- | ---- | ---- | ---- | ---- |
|安全命令| any | empty |`0`|继续|
|`git push --force`|`deny`|`{"permissionDecision":"deny",...}`|`0`| block with reason |
|`git push --force`|`log`| empty |`0`|继续（仅限日志）|

##事件类型

完整的hooks引用是权威的。**在编写钩子之前，总是检查最新的有效负载形状：

-[钩子配置参考]（https://docs.github.com/en/copilot/reference/hooks-configuration）
-[关于钩子]（https://docs.github.com/en/copilot/concepts/agents/cloud-agent/about-hooks）

|事件| stdout |典型使用|| ---- | ---- | ---- |
|`sessionStart`| **parsed** -将stdout中的`additionalContext`注入到会话|设置、验证、上下文注入、日志|
|`sessionEnd`|忽略|清理，总结|
|`userPromptSubmitted`|忽略|审计，提示阻塞|
|`preToolUse`| **解析** -`permissionDecision`，`modifiedArgs`/`updatedInput`，`additionalContext`|护栏，deny/block，参数修改|
|`postToolUse`|忽略|日志记录，格式化|
|`postToolUseFailure`| - |工具运行失败后恢复|
|`agentStop`| - |最终验证|
|`subagentStart`| - |子代理审计|
|`subagentStop`| - |子代理输出验证|
|`errorOccurred`|忽略|诊断，提醒|
|`preCompact`| - |预压实工作|
|`permissionRequest`| - |审批流程|

常见事件的有效负载模式

这些是来自hook参考的有效载荷形状。始终根据[官方参考]（https://docs.github.com/en/copilot/reference/hooks-configuration）验证最新的字段。

* *`sessionStart`* *```json
{
  "timestamp": 1704614400000,
  "cwd": "/path/to/project",
  "source": "new",
  "initialPrompt": "Create a new feature"
}
```
`source`是`"new"`，`"resume"`，或`"startup"`。如果提供的话，`initialPrompt`是用户的第一个提示符。

**`sessionStart`标准输出** -主机解析标准输出：```json
{
  "additionalContext": "Current branch: main. Deploy target: staging."
}
```
`additionalContext`被直接注入到会话会话中，允许钩子动态地提供特定于环境的上下文。

* *`sessionEnd`* *```json
{
  "timestamp": 1704618000000,
  "cwd": "/path/to/project",
  "reason": "complete"
}
```
`reason`是`"complete"`、`"error"`、`"abort"`、`"timeout"`或`"user_exit"`。

* *`userPromptSubmitted`* *```json
{
  "timestamp": 1704614500000,
  "cwd": "/path/to/project",
  "prompt": "Fix the authentication bug"
}
```
字段是`prompt`—用户提交的确切文本。

* *`preToolUse`* *```json
{
  "timestamp": 1704614600000,
  "cwd": "/path/to/project",
  "toolName": "bash",
  "toolArgs": "{\"command\":\"rm -rf dist\",\"description\":\"Clean build directory\"}"
}
```
`toolArgs`是一个JSON字符串，请再次解析它以访问它的字段。

**`preToolUse`标准输出** -主机解析标准输出：

|字段|它做什么|| ---- | ---- |
|`permissionDecision`|`"deny"`阻塞工具调用。`"allow"`和`"ask"`也接受；目前只处理`"deny"`。|
|`permissionDecisionReason`|显示给用户的可读原因|
|`modifiedArgs`或`updatedInput`|替换工具参数-用于代替原始|
|`additionalContext`|本回合注入代理上下文的文本|

* *`postToolUse`* *```json
{
  "timestamp": 1704614700000,
  "cwd": "/path/to/project",
  "toolName": "bash",
  "toolArgs": "{\"command\":\"npm test\"}",
  "toolResult": {
    "resultType": "success",
    "textResultForLlm": "All tests passed (15/15)"
  }
}
```
`resultType`是`"success"`，`"failure"`，或`"denied"`。

* *`errorOccurred`* *```json
{
  "timestamp": 1704614800000,
  "cwd": "/path/to/project",
  "error": {
    "message": "Network timeout",
    "name": "TimeoutError",
    "stack": "TimeoutError: Network timeout\n    at ..."
  }
}
```
* *`agentStop`* *```json
{
  "timestamp": 1704618000000,
  "cwd": "/path/to/project"
}
```
最小负载——用它来触发会话结束操作，比如运行`git diff --stat`或最终验证。

当钩子是错误的工具

|避免挂钩，更适合|| ---- | ---- |
|开放式推理或风格指导|说明、提示或代理|
|带有内存、重试或分支的长多步骤工作流|代理、脚本或工作流引擎|
|后台守护进程、监视程序、脱环或异步作业|专用自动化、服务或CI |
|重存储库范围的验证| CI、计划作业或专用自动化|

通用设计规则

b|规则b|为什么重要b|| ---- | ---- |
一个钩子，一个责任|小钩子更容易信任和调试|
|默认为**先观察** |阻塞或突变应该是一个明确的选择|
|保持钩子同步、有界和非交互|钩子在关键路径|中运行
使钩子确定性和幂等|重新运行不应该产生漂移|
|默认不改变分支、索引、工作树状态| git破坏行为属于高风险|
|将提示、工具参数和工具输出视为不可信和敏感的|输入可能是恶意的或私有的|
|编辑日志中的秘密、凭据、令牌和私有内容|日志的寿命通常比钩子运行的时间长|

##脚本创作规则-验证您实际使用的JSON字段
-引用shell变量，从不从原始输入构建命令
—保持stdout整洁，除非主机需要结构化输出
—使用严格模式：Bash`set -euo pipefail`、PowerShell`Set-StrictMode -Version Latest`-尽早检查依赖关系，并在缺少依赖关系时明确失败
—在执行过程中避免提示、隐藏安装或环境变化
-通过手动将有代表性的JSON有效负载导入测试脚本

选择最小可行的执行方法

1. **PowerShell 7**、**Node.js**或**Python**用于广泛可移植的钩子
2. **Bash**，其中Bash是一个明确的要求或安全假设
3. 当存储库已经依赖于现有的项目CLI时

不要仅仅为了实现一个普通的钩子而引入一个新的编译运行时。

包装一个可重用的钩子—打包配置、脚本和文档
-记录触发事件、目的、副作用、依赖关系和禁用路径
-解释钩子读什么，写什么，阻塞什么

# #反模式

-长时间运行的钩子、监视程序、后台守护进程或即发即弃异步工作
-当一个较窄的触发器可以触发时，对每个事件进行大量扫描
—关键路径下的隐藏网络呼叫或上传
-默认情况下Git状态（checkout, reset, clean, stash, stage, commit, push，或历史重写）的静默突变
-交互式提示或隐式审批步骤
-嘈杂标准输出、临时输出格式或混合machine/human输出
—记录原始提示、秘密、凭证或大型工具输出
-混合了不相关职责的单体钩子

# #可移植性

###GitHub Copilot: CLI、VS Code和Cloud Agent相同的`.github/hooks/*.json`配置、相同的有效负载模式和相同的脚本契约可以跨CLI、VS Code和云代理工作。事件名称接受camelCase （`preToolUse`）和PascalCase （`PreToolUse`）。工具参数的文档有效负载字段是`toolArgs`（一个JSON字符串）。

需要知道的一点是：云代理只从存储库的默认分支加载钩子。如果您的hooks.json只在一个特性分支上，那么云代理将看不到它。

克劳德代码

Claude Code使用了一个不同的钩子系统：

—“`~/.claude/settings.json`”和“`.claude/settings.json`”中的设置
-不同的事件名称和匹配器语法（regex，`if`条件）
出口2 =阻塞，出口1 =非阻塞错误（与GitHub Copilot不同）
- 5种钩子类型（命令、http、McP_tool、提示、代理）
-包括`FileChanged`，`CwdChanged`，`ConfigChange`在内的29多个赛事共享的最佳实践是相同的：保持钩子小、确定、对I/O显式，并严格控制副作用。