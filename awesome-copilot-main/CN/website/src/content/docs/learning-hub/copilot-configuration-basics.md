---
title: 'Copilot Configuration Basics'
description: 'Learn how to configure GitHub Copilot at user, workspace, and repository levels to optimize your AI-assisted development experience.'
authors:
  - GitHub Copilot Learning Hub Team
lastUpdated: 2026-07-07
estimatedReadingTime: '10 minutes'
tags:
  - configuration
  - setup
  - fundamentals
relatedArticles:
  - ./what-are-agents-skills-instructions.md
  - ./understanding-copilot-context.md
prerequisites:
  - Basic familiarity with GitHub Copilot
---
GitHub Copilot提供了广泛的配置选项，允许您根据个人偏好、项目需求和团队标准定制其行为。理解这些配置层可以帮助您最大限度地提高生产力，同时保持团队之间的一致性。本文解释了配置层次结构、关键设置以及如何设置使整个团队受益的存储库级别自定义。

##配置级别GitHub Copilot使用分层配置系统，其中不同级别的设置可以相互覆盖。理解这个层次结构可以帮助您在正确的级别应用正确的配置。

###用户设置

用户设置全局应用于所有项目，并代表您的个人偏好。它们存储在IDE的用户配置中，并随IDE配置文件一起传输。**普通用户级设置**：
-Enable/disable内联建议全局
—提交消息样式首选项
-默认语言首选项

**何时使用**：用于个人偏好，应该适用于您工作的任何地方，如键盘快捷键或您是否喜欢内联建议与聊天。

存储库设置

存储库设置存在于代码库中（通常在`.github/`中，尽管有些编辑器允许自定义Copilot将使用的路径），并与项目中的每个人共享。它们提供最高级别的自定义，并覆盖用户和工作空间设置。

**常见的库级自定义**：
-编码约定的自定义说明
-通用任务的可重用技能
-项目工作流程的专门代理
-专业领域的客户代理**何时使用**：用于存储库范围内的标准，特定于项目的最佳实践，以及应该进行版本控制和共享的可重用自定义。

组织设置（仅限GitHub.com）

组织设置允许管理员跨组织内的所有存储库强制执行Copilot策略。这些设置可以包括定义自定义代理、创建全局应用指令、启用或禁用Copilot、管理账单以及设置使用限制。这些策略可能不会在IDE中强制执行，这取决于IDE对组织级别设置的支持，但将适用于GitHub.com上的Copilot使用。

**何时使用**：用于实施组织范围的策略，确保合规性，并跨多个存储库提供共享资源。

配置优先级当多个配置级别定义相同的设置时，GitHub Copilot按以下顺序应用它们（优先级最高）：

1. **机构设置**（如适用）
1. **存储库设置** （`.github/`）
1. **用户设置** （IDE全局首选项）

**示例**：如果您的用户设置为`.test.ts`文件禁用了Copilot，但存储库设置为测试文件启用了自定义指令，则存储库设置优先，并且Copilot在应用自定义指令时保持活跃。

密钥配置选项

这些设置控制GitHub Copilot在所有ide中的核心行为：

内联建议

控制是否在您键入时自动建议Copilot完成代码。

* * * *VS Code例子:```json
{
  "github.copilot.enable": {
    "*": true,
    "plaintext": false,
    "markdown": false
  }
}
```
**为什么重要**：有些开发者更喜欢显式调用Copilot，而不是看到自动建议。您还可以仅为特定语言启用它。

###聊天可用性

控制访问GitHub Copilot聊天在您的IDE。

* * * *VS Code例子:```json
{
  "github.copilot.chat.enabled": true
}
```
**为什么重要**:Chat提供了一个会话界面，用于提问和获得解释，补充内联建议。

建议触发行为

配置Copilot生成建议的方式和时间。

* * * *VS Code例子:```json
{
  "editor.inlineSuggest.enabled": true,
  "github.copilot.editor.enableAutoCompletions": true
}
```
**为什么重要**：控制建议是自动出现还是只有在明确要求时才出现，平衡有益和潜在的干扰。

特定于语言的设置

为特定编程语言启用或禁用Copilot。

* * * *VS Code例子:```json
{
  "github.copilot.enable": {
    "typescript": true,
    "javascript": true,
    "python": true,
    "markdown": false
  }
}
```
**为什么重要**：您可能希望Copilot为代码文件激活，但不为文档或配置文件激活。

排除的文件和目录

防止Copilot访问特定的文件或目录。

* * * *VS Code例子:```json
{
  "github.copilot.advanced": {
    "debug.filterLogCategories": [],
    "excludedFiles": [
      "**/secrets/**",
      "**/*.env",
      "**/node_modules/**"
    ]
  }
}
```
**为什么重要**：从Copilot的上下文中排除敏感文件、生成的代码或依赖项，以提高建议的相关性并保护机密信息。

##存储库级配置

存储库中的`.github/`目录支持团队范围的定制，这些定制受版本控制，并在所有贡献者之间共享。

目录结构

一个组织良好的Copilot配置目录是这样的：```
.github/
├── agents/
│   ├── terraform-expert.agent.md
│   └── api-reviewer.agent.md
├── skills/
│   ├── generate-tests/
│   │   └── SKILL.md
│   └── refactor-component/
│       └── SKILL.md
└── instructions/
    ├── typescript-conventions.instructions.md
    └── api-design.instructions.md
```
支持Monorepo

在具有多个包或服务的单节点环境中，GitHub CopilotCLI可以在从工作目录到git存储库根目录的每个目录级别发现自定义。这意味着每个包或服务都可以拥有自己的`.github/`文件夹，其中包含专门的代理、指令、技能和MCP服务器，同时仍然继承来自父目录的配置。```
my-monorepo/
├── .github/
│   └── instructions/
│       └── shared-conventions.instructions.md   ← applies everywhere
├── packages/
│   ├── api/
│   │   └── .github/
│   │       └── agents/
│   │           └── api-expert.agent.md           ← applies in packages/api/
│   └── web/
│       └── .github/
│           └── instructions/
│               └── react-conventions.instructions.md  ← applies in packages/web/
```
当您在`packages/api/`内部工作时，Copilot从`packages/api/.github/`加载配置，然后从`packages/.github/`（如果存在）加载配置，然后从根`.github/`加载配置。这种分层发现确保了无论您在存储库中的哪个位置工作，正确的上下文都是活动的。

个人技能目录

除了存储库级别的技能，GitHub CopilotCLI还支持`~/.copilot/skills/`和`~/.agents/skills/`的**个人技能目录**。您放置在任何位置的技能都会在您的所有项目中自动发现，这使得它们非常适合于个人工作流和不特定于项目的可重用实用程序。```
~/.agents/
└── skills/
    ├── my-review-style/
    │   └── SKILL.md     ← available in all sessions
    └── cleanup-todos/
        └── SKILL.md
```
`~/.agents/skills/`路径与VS CodeGitHub Copilot为Azure扩展的默认技能发现路径对齐，而`~/.copilot/skills/`匹配Copilot CLI配置目录。两者都支持个人技能。

自定义代理

代理是特定工作流程的专门助手。将代理定义文件放在`.github/agents/`中。

**代理示例** (`terraform-expert.agent.md`)：```markdown
---
description: 'Terraform infrastructure-as-code specialist'
tools: ['filesystem', 'terminal']
name: 'Terraform Expert'
---

You are an expert in Terraform and cloud infrastructure.
Guide users through creating, reviewing, and deploying infrastructure code.
```
**何时使用**：为基础设施管理、API设计或安全审查等领域特定任务创建代理。

可重复使用的技能

技能是包含可重用功能的自包含文件夹。将它们存储在`.github/skills/`中。

**示例技能** (`generate-tests/SKILL.md`)：```markdown
---
name: generate-tests
description: 'Generate comprehensive unit tests for a component, covering happy path, edge cases, and error conditions'
---

# generate-tests

Generate unit tests for the selected code that:
- Cover all public methods and edge cases
- Use our testing conventions from @testing-utils.ts
- Include descriptive test names

See [references/test-patterns.md](references/test-patterns.md) for standard patterns.
```
技能还可以在文件夹中捆绑参考文件、模板和脚本，为AI提供比单个文件更丰富的上下文。与旧的提示格式不同，代理可以自动发现和调用技能。

**动态技能检索** (v1.0.66+)：默认情况下，Copilot CLI使用基于嵌入的检索，自动为每个提示显示最相关的技能。您可以使用`--dynamic-retrieval`标志或`dynamicRetrieval`配置设置来切换此行为。禁用基于嵌入的检索（例如，强制加载所有已配置的技能）：```bash
copilot --dynamic-retrieval skills=off
```
一旦保存到您的配置中，此设置将在各个会话之间持续存在。

**何时使用**：对于团队定期执行的重复性任务，如生成测试、创建文档或重构模式。

指令文件

指令提供在特定文件或目录中工作时自动应用的持久上下文。将它们存储在`.github/instructions/`中。

**示例指令** (`typescript-conventions.instructions.md`)：```markdown
---
description: 'TypeScript coding conventions for this project'
applyTo: '**.ts, **.tsx'
---

When writing TypeScript code:
- Use strict type checking
- Prefer interfaces over type aliases for object types
- Always handle null/undefined with optional chaining
- Use async/await instead of raw promises
```
**何时使用**：对于应该影响所有建议的项目范围的编码标准、架构模式或特定于技术的约定。

设置团队配置

按照以下步骤建立有效的团队副驾驶配置：

# # # 1。创建配置结构

首先在存储库根目录中创建`.github/`目录：```bash
mkdir -p .github/{agents,skills,instructions}
```
# # # 2。记录你的约定

创建能够捕获团队编码标准的说明：```markdown
<!-- .github/instructions/team-conventions.instructions.md -->
---
description: 'Team coding conventions and best practices'
applyTo: '**'
---

Our team follows these practices:
- Write self-documenting code with clear names
- Add comments only for complex logic
- Prefer composition over inheritance
- Keep functions small and focused
```
# # # 3。构建可重用技能

识别重复性任务并为其创造技能：```markdown
<!-- .github/skills/add-error-handling/SKILL.md -->
---
name: add-error-handling
description: 'Add comprehensive error handling to existing code following team patterns'
---

# add-error-handling

Add error handling to the selected code:
- Catch and handle potential errors
- Log errors with context
- Provide meaningful error messages
- Follow our error handling patterns from @error-utils.ts
```
# # # 4。版本控制最佳实践

- **提交所有`.github/`文件**到您的存储库
- **在添加或更新自定义时使用描述性提交消息**
**审查变更**以确保其符合团队标准
- **文档**每个定制清晰的描述和示例

# # # 5。加入新团队成员

让副驾驶配置成为你入职过程的一部分：

1. 将新成员指向`.github/`目录
2. 解释存在哪些代理和技能以及何时使用它们
3. 鼓励探索和贡献
4. 在您的项目README中包含示例用法

特定于ide的配置

虽然存储库级别的自定义可以在所有ide中工作，但您可能还需要特定于ide的设置：### VS Code
设置文件：`.vscode/settings.json`或全局用户设置```json
{
  "github.copilot.enable": {
    "*": true
  },
  "github.copilot.chat.enabled": true,
  "editor.inlineSuggest.enabled": true
}
```
Visual Studio

设置路径：Tools→Options→GitHub Copilot-配置内联建议
-设置键盘快捷键
-管理特定于语言的启用

### JetBrains ide

设置：文件→设置→工具→GitHub Copilot-Enable/disable用于指定文件类型
-配置建议行为
-自定义键盘快捷键GitHub Copilot命令行

配置文件：`~/.copilot-cli/config.json````json
{
  "editor": "vim",
  "suggestions": true
}
```
CLI设置使用**camelCase**命名。最近版本中添加的关键设置：

|设置|描述||---------|-------------|
|`includeCoAuthoredBy`|在提交中包含共同编写的预告片|
|`effortLevel`|默认推理努力级别（`low`,`medium`,`high`） |
|`autoUpdatesChannel`|更新通道（`stable`,`preview`） |
|`statusLine`|在终端界面显示状态行|
|`include_gitignored`|在`@`文件搜索中包含被忽略的文件|
|`extension_mode`|控件可扩展性（代理工具和插件）|
|`continueOnAutoMode`|在限速时自动切换到auto模式，而不是暂停|
|`proxy`|所有出站CLI请求的HTTP(S)代理URL（例如，`http://proxy.example.com:8080`） （v1.0.64+） |
|`sessionLimits`|限制信用或回合使用会话；限制适用于当前会话，并在`/clear`(v1.0.66+) |上重置
|`stayInAutopilot`|自动驾驶任务完成后，保持CLI处于自动驾驶模式，而不是返回到交互模式（v1.0.69+） |b> **注**：旧的snake_case名称（例如，`include_gitignored`,`auto_updates_channel`）仍然被接受向后兼容，但camelCase现在是首选格式。

除了主配置文件之外，GitHub CopilotCLI还读取两个可选的每个项目文件，用于特定于存储库的覆盖：

-`.claude/settings.json`-已提交工程设置
-`.claude/settings.local.json`-本地覆盖（添加到`.gitignore`进行个人调整）

这些文件遵循与`config.json`相同的格式，并在全局配置之后加载，因此它们可以在不触及`.github/`的情况下为每个存储库定制CLI行为（包括钩子定义）。b> **重要(v1.0.36+)**：放置在`~/.claude/`（Claude Code用户目录）中的自定义代理，技能和命令**不再由GitHub CopilotCLI加载**。配置时只读取`~/.claude/settings.json`。如果您以前在`~/.claude/`中存储了个人代理或技能，请将它们移动到受支持的位置：`~/.copilot/agents/`用于用户级代理，`~/.copilot/skills/`或`~/.agents/skills/`用于个人技能，或者`.github/agents/`和`.github/skills/`用于项目级定制。

模型选择器

模型选择器以全屏视图打开，并进行内联推理调整。使用**←/→**箭头键直接从选择器更改推理努力级别（`low`,`medium`,`high`），而无需离开会话。当前的推理工作级别也显示在模型标题中（例如，`claude-sonnet-4.6 (high)`），因此您始终知道哪个级别是活动的。**自动模式和服务器端模型路由** (v1.0.43+)：当您选择**Auto**作为您的模型时，CLI使用服务器端模型路由进行实时模型选择。Auto模式不是在会话开始时锁定单个模型，而是对每个请求进行评估，并动态地将其路由到最合适的模型。这意味着简单的问题可以由一个更快的模型来处理，而复杂的推理任务可以自动升级——而不需要你手动切换模型。**模型族别名** (v1.0.64+)：您可以在模型设置中使用短族别名，而不是键入完整的模型名：`opus`、`sonnet`、`haiku`（Anthropic）和`gpt`、`gemini`（Google/OpenAI）。CLI将别名解析为该系列中最新的可用模型。这在脚本或配置文件中特别有用，因为您希望在不硬编码版本字符串的情况下跟踪一个家族中的最佳模型。

### CLI会话命令`/settings`命令（v1.0.61+）打开一个交互式对话框，在一个地方浏览和编辑所有用户设置。使用它可以发现可用的设置、切换选项和更新值，而无需手动编辑配置文件：```
/settings
```
设置对话框支持按名称筛选设置的搜索类型。更改立即生效。GitHub CopilotCLI有两个用于管理会话状态的命令，具有不同的行为：

|命令|行为||---------|-----------|
|`/new [prompt]`|启动一个新的会话，同时保持当前会话的后台。您可以切换回后台会话。|
|`/clear [prompt]`|完全放弃当前会话并开始一个新的会话。后台会话不受影响。在项目中配置的MCP服务器保留在新会话中。|

这两个命令都接受一个可选的prompt参数，用一个打开消息作为新会话的种子，例如`/new Add error handling to the login flow`。`/session rename`命令重命名当前会话。当调用**而不带名称参数**时，它会根据会话历史自动生成会话名称：```
/session rename               # auto-generate a name from conversation history
/session rename "My feature"  # set a specific name
```
自动生成的名称可以帮助您在多个后台会话之间切换时快速找到会话。

您还可以在启动时使用`--name`标志命名会话，然后通过名称恢复会话：```bash
copilot --name "auth-refactor"          # start a session with a given name
copilot --resume="auth-refactor"        # resume that session by name
```
`/session delete`命令删除不再需要的会话：```
/session delete              # delete the current session
/session delete <id>         # delete a session by ID
/session delete-all          # delete all sessions
```
您还可以在会话选择器（`--resume`）中按**x**以将突出显示的会话直接从列表中删除。

在会话选择器中，按**`s`**循环排序顺序：相关性、上次使用、创建或名称。选择器还显示每个会话的分支名称和idle/in-use状态。`/rewind`命令打开一个时间轴选择器，允许您将对话回滚到历史上的任何较早的点，恢复对话和在该点之后所做的任何文件更改。你也可以通过按**双esc **来触发它：```
/rewind
```
当您希望从对话中的不同点进行分支时，可以使用`/rewind`，而不仅仅是撤消最近的回合。`/undo`命令恢复最后一个回合——包括代理所做的任何文件更改——允许您在不手动撤消编辑的情况下进行航向纠正：```
/undo
```
当代理的最后一个响应指向不需要的方向时，使用`/undo`，并且您希望从那时开始尝试不同的方法。`/fork`命令（v1.0.45+）将当前会话复制到一个新的独立会话，该会话从相同的会话状态开始。原始会话保持不变-您可以随时切换回它。当您希望同时探索解决问题的两种不同方法时，这非常有用。在v1.0.64+中，`/branch`可以作为`/fork`的别名（与Claude Code的命令命名相匹配）：```
/fork                    # fork with an auto-generated name
/fork "my-experiment"    # fork with a custom name (v1.0.47+)
/branch                  # alias for /fork (v1.0.64+)
```
分叉后，新会话立即激活。两个会话共享相同的历史，直到分叉点，但从那一刻起独立地积累变化。在不放弃当前工作会话的情况下，使用`/fork`进行有风险的重构实验。从v1.0.47开始，分叉的会话在会话对话框中显示它们的原始会话名称，从而很容易跟踪分叉来自哪个会话。`/cd`命令改变当前会话的工作目录。从v1.0.65开始，当您恢复会话时，工作目录**仍然存在-如果您重新启动CLI并恢复，您将自动返回到相同的目录。更改目录还会触发在新位置发现自定义代理，因此切换到另一个项目无需重新启动即可加载其代理：```
/cd ~/projects/my-other-repo
```
当您有多个后台会话，每个会话侧重于不同的项目目录时，这很有用。`/worktree`命令（v1.0.61+，别名`/move`）创建一个新的git工作树并切换到其中，移动任何未提交的更改。这可以让你在不离开当前终端会话的情况下开始并行分支的工作：```
/worktree my-feature-branch
```
在v1.0.66+中，您可以将任务描述传递给`/worktree`，以从任务中命名分支，并立即将任务作为新工作树中的第一个提示符运行-所有这些都是一步完成的：```
/worktree fix the login redirect
```
这将根据您的任务描述创建一个分支，并立即开始处理该分支，从而可以轻松地启动并行工作，而无需停下来考虑分支名称。

命令运行后，会话位于新工作树中。当您希望并行处理第二个任务而不隐藏更改或打开新终端时，可以使用此选项。在v1.0.64+中，您还可以在启动时使用实验性的`--worktree`标志（`copilot -w [name]`），以便在会话开始之前在`<repo>.worktrees/`下创建或重用工作树。`/every`命令（自v1.0.64以来也可以作为`/loop`使用）安排一个循环提示以指定的间隔自动运行。配套的`/after`命令在指定的延迟之后运行一次提示符。两者都适用于自定节奏的自动化——轮询结果、定期总结进度或触发计时器上的其他斜杠命令：```
/every 5m Check if there are any new test failures and summarize them
/loop 30s Check if the build is done
/after 2h /compact                        # compact the session after 2 hours
/every 1d /chronicle standup              # daily standup report via /chronicle
```
间隔可以指定为秒（`s`）、分钟（`m`）或小时（`h`），这两个命令都可以调用其他斜杠命令作为它们的有效负载。要查看和管理所有预定的提示，请使用不带参数的`/every`-它将打开计划管理器。要取消正在运行的计划，请使用`/every stop`或**Ctrl+C**。

b> **实验**:`/every`，`/loop`和`/after`是实验特征集的一部分。它们出现在`/experimental`斜杠命令列表中—如果它们在当前会话中还不可见，则启用实验性特性。

b> **注**：预定的提示在当前会话的后台运行，并使用您的活动模型。它们共享会话上下文窗口，因此具有长响应的非常频繁的调度可能会快速消耗上下文。如果需要考虑上下文使用，请使用`/compact`。`/pr auto`命令*(v1.0.66+)*启动一个自定节奏的自动化循环，将当前的拉取请求驱动到CI绿色。它不是连续运行，而是每次运行修复一个失败的项目，并围绕CI检查进行调整，以避免冗余工作：```
/pr auto            # start fixing the current PR until CI passes
/pr automerge       # continue until the PR is fully merged
```
当您的PR包含失败的测试或检查错误时，`/pr auto`是理想的—让它一次处理一个失败，而您则专注于其他事情。`/pr automerge`进一步扩展了这一点：它将一直持续到所有CI检查通过，所需的评审得到批准，并且PR成功合并。可以从`/loop`或`/every`监视和停止这两个命令，它们将正在运行的自动化注册为可调度的循环任务。`/share html`命令将当前会话（包括会话历史和任何研究报告）导出为一个自包含的交互式HTML文件。```
/share html
```
导出的文件包含在没有网络连接的情况下查看会话所需的所有内容，可以与队友共享或存储以供以后参考。这是对`/share`（通过URL共享）的补充，适用于首选脱机或附加格式的情况。`/chronicle`命令打开代理在当前会话中所做的所有事情的交互式时间轴。它按时间顺序显示文件更改，工具调用和对话，让您一目了然地查看会话的完整弧线。```
/chronicle
```
Chronicle跟踪在会话期间创建、修改或删除了哪些文件，以及导致这些更改的对话。使用它来检查`/rewind`之前发生的事情，审计代理更改的内容，或者与团队成员共享会话活动的摘要。`/chronicle skills review`子命令*(v1.0.66+)*为提议的技能修改草案打开一个交互式审查流。当代理在会议期间建议增加或修改技能时，您可以单独审查每个草案并选择接受，拒绝或推迟：```
/chronicle skills review
```
这使您能够控制技能演变—代理可以在发现可重用模式时提出技能改进，但在您明确批准每个更改之前，什么都不会应用。

**注**：会话历史、文件跟踪和`/chronicle`命令以前是实验性的功能。从v1.0.40开始，所有用户都可以使用它们，而无需启用实验模式。`/diagnose`命令（v1.0.64+）分析当前会话的日志并显示诊断信息，以帮助排除意外行为、性能问题或错误：```
/diagnose
```
当会话行为异常时使用`/diagnose`—它检查会话日志并报告发现的内容，从而更容易与支持人员共享诊断信息或了解内部发生的情况。

**排队消息的键盘快捷键**：使用**Ctrl+Q**或**Ctrl+Enter**将消息排队（在座席仍在工作时发送消息）。**Ctrl+D**不再排队消息-它现在有其默认的终端行为。如果你对按Ctrl+D排队有肌肉记忆，那就切换到按Ctrl+Q。

**后台运行任务**：按**Ctrl+X→B**将当前运行的任务或shell命令移至后台。任务继续执行，同时您可以键入新消息或查看先前的输出。这对于希望在等待结果时与代理交互的长时间运行命令非常有用。**正常模式下的Shell命令历史** (v1.0.65+): **↑/↓**箭头键和**Ctrl+R**反向搜索现在包括过去的Shell命令（使用`!`运行的命令），而您处于正常（非Shell）输入模式。以前，您必须输入`!`才能进入shell模式。现在，您可以召回并重新运行shell命令，而无需首先切换模式——这对于快速重复会话早期的构建、测试或诊断命令非常有用。

**内联图像渲染** (v1.0.64+)：如果终端支持，CLI可以在终端中内联显示图像。如果MCP工具、代理或附件返回图像，它将直接呈现在会话时间轴中，而不是显示为文件路径或URL。这适用于具有图像协议支持的终端（如iTerm2、Kitty、Wezterm和具有适当配置的tmux）。`/ask`命令允许您在不影响对话历史记录的情况下提出一个快速问题。当前会话上下文被保留，因此您可以将其用于一次性查找，而不会使正在进行的任务脱轨。响应呈现为完整的标记，包括表格和格式化的链接：```
/ask What does the `retry` utility in src/utils do?
```
`/env`命令在一个视图中显示所有加载的环境细节—指令、MCP服务器、技能、代理和插件。使用它来验证当前会话是否激活了正确的资源：```
/env
```
`/context`命令显示当前会话上下文窗口使用情况的可视化-消耗了多少令牌以及剩余多少剩余空间：```
/context
```
`/usage`命令显示会话指标，例如消耗的令牌数量、进行的API调用和当前会话的任何配额信息。在v1.0.64+中，当您在会话中使用多个模型时，`/usage`还显示每个模型的令牌总数：```
/usage
```
`/compact`命令总结会话历史，以在保留会话线程的同时释放上下文窗口空间。当你的上下文已经满了，但你不想开始一个新的会话时使用它：```
/compact
```
> **注**：技能在`/compact`后仍然加载并有效。在压缩后不需要重新调用它们。

b> **ACP会话(v1.0.39+)**:`/compact`,`/context`，`/usage`和`/env`命令现在在ACP（代理协调协议）会话中可用，允许远程ACP客户端从他们自己的自动化工作流中显示会话细节和管理上下文。`/statusline`命令（使用`/footer`作为别名）允许您控制终端状态栏中出现的项目。您可以显示或隐藏单个指标，如工作目录、当前分支、工作级别、上下文窗口使用情况、配额和**活动帐户用户名** (v1.0.43+)。**changes**开关显示会话的added/removed行数——在跟踪正在进行的编辑的范围时非常有用。在v1.0.65+中，还有一个可选的**CI检查状态**指示器，显示当前分支的CI检查的passing/running/failing状态-从`/statusline`菜单启用它：```
/statusline             # show the statusline configuration menu
```
切换**用户名**指示器显示哪个GitHub帐户目前在页脚中处于活动状态-当您使用多个帐户或在个人和组织上下文之间切换时很有用。`/keep-alive`命令可以防止系统在Copilot CLI处于激活状态时处于休眠状态。这在笔记本电脑或具有侵略性睡眠设置的机器上长时间运行的代理会话期间非常有用：```
/keep-alive             # toggle keep-alive on or off
```
**注**:`/keep-alive`以前是一个实验性功能。从v1.0.36开始，无需启用实验模式即可使用。`/allow-all`命令（也可以作为`/yolo`访问）启用自动驾驶模式，在这种模式下，代理运行所有工具而不需要请求确认。它现在支持`on`、`off`和`show`子命令：```
/allow-all on     # enable allow-all mode
/allow-all off    # disable allow-all mode
/allow-all show   # check current allow-all status
```
**注**:`/allow-all on`权限在`/clear`启动新会话后仍然存在，因此您不需要每次都重新启用它。

b> **ACP客户端(v1.0.39+)**: ACP客户端还可以通过会话配置以编程方式切换allow-all模式，而无需发出斜杠命令。这对于通过ACP协议驱动Copilot CLI的自动化管道非常有用。`/autopilot`命令（v1.0.45+）是一个快速的会话切换，可以在**交互模式**（代理在使用工具之前暂停请求确认）和**自动驾驶模式**（自动运行）之间切换。与`/allow-all`不同，`/allow-all`专门控制是否需要工具权限，`/autopilot`切换整体代理模式：```
/autopilot        # toggle between interactive and autopilot modes
```
当您希望在会话中在监督操作和非监督操作之间切换，而不需要输入完整的`/allow-all on`或`/allow-all off`命令时，可以使用`/autopilot`。

b> **增强的自动驾驶仪(v1.0.64+)**：当自动驾驶模式是活跃的-包括启动时`--autopilot`启动时或在自动延续转弯期间-代理自动处理引出对话框，`ask_user`提示，采样请求和权限提示，而不将它们作为交互对话框呈现。这意味着长时间运行的自动会话可以端到端进行，而无需手动确认步骤。b> **自动允许所有模式(v1.0.69+)**：除了标准的允许所有模式（批准所有内容）之外，CLI现在支持**自动允许所有模式，该模式使用LLM判断来评估每个工具请求。启用后，法官会自动批准它评估为可接受的请求，并且只要求您对它认为有风险的请求进行手动确认。这给了你一个介于完全自动驾驶和完全监督操作之间的中间地带——大多数常规操作会自动进行，而不寻常或潜在危险的操作仍然会浮出水面供你审查。从v1.0.69-3开始，该模式需要启用实验性特性—使用`/experimental on`或使用`--experimental`启动CLI—然后使用`/allow-all auto`激活它。以前的`AUTO_APPROVAL`环境变量方法已被删除，以支持实验模式。b> **只读`gh`CLI命令(v1.0.46+)**：只读`gh`命令-如`gh issue list`，`gh pr view`,`gh run status`，以及其他不写入GitHub的命令- **自动批准**，不需要权限提示。只有写到GitHub的命令（如创建问题，合并pr）仍然需要明确的批准。这减少了你频繁检查问题或PR状态的探索性会议中的摩擦。`--effort`标志（`--reasoning-effort`的简写）控制模型对请求应用多少计算推理：```bash
gh copilot --effort high "Refactor the authentication module"
```
接受的值为`low`、`medium`和`high`。您还可以通过`effortLevel`配置设置设置默认值。

### CLI启动标志`-C <directory>`标志在启动前更改工作目录，类似于`git -C`（v1.0.42+）。这对于需要在特定项目目录中启动Copilot CLI的脚本或别名很有用，而不需要单独的`cd`：```bash
copilot -C ~/projects/my-repo          # start in a different directory
copilot -C ~/projects/my-repo -p "..."  # combine with prompt mode
```
`--mode`标志（以及它的别名`--autopilot`和`--plan`）允许您直接以特定代理模式启动CLI，而无需等待交互式会话启动：```bash
copilot --mode agent    # start in agent mode (autonomous tool use)
copilot --autopilot     # alias for --mode autopilot (allow-all)
copilot --plan          # start in plan mode (propose without executing)
```
这在脚本或CI管道中非常有用，因为您希望CLI在没有交互式提示的情况下立即以特定模式开始工作。`--max-autopilot-continues`标志控制副驾驶在暂停确认之前可以自动继续进入自动驾驶模式的次数。默认值是5：```bash
copilot --autopilot --max-autopilot-continues 10 "Refactor the authentication module"
```
对于长时间运行的任务设置更高的值，对于需要更频繁检查点的任务设置更低的值。将其设置为`0`将完全禁用自动延续。`--attachment`标志（在提示模式下可用，`-p`）允许您在非交互模式下将文件（图像或本机文档）附加到初始提示符：```bash
copilot -p "Summarize the architecture shown in these diagrams" \
  --attachment arch-overview.png \
  --attachment data-flow.pdf
```
这在自动化管道中非常有用，当您希望在没有交互式文件选择的情况下将可视化或文档上下文（屏幕截图、设计规范、PDF报告）传递给模型时。可以指定多个`--attachment`标志来一次包含多个文件。`COPILOT_HOME`环境变量用于设置Copilot CLI配置目录。它是`--config-dir`标志的首选替代品，已弃用：```bash
# Preferred — set via environment variable
export COPILOT_HOME=~/.my-copilot-config
copilot

# Deprecated — use COPILOT_HOME instead
copilot --config-dir ~/.my-copilot-config
```
在shell配置文件中设置`COPILOT_HOME`，以便在所有会话中使用自定义配置目录。这在为不同的项目或团队运行多个Copilot配置时特别有用。

Shell完成`copilot completion`子命令为子命令、标志和已知选项值生成静态shell完成脚本。安装后，按Tab键自动完成终端中的Copilot CLI命令。```bash
# Bash — add to ~/.bashrc
eval "$(copilot completion bash)"

# Zsh — add to ~/.zshrc
eval "$(copilot completion zsh)"

# Fish — add to ~/.config/fish/config.fish
copilot completion fish | source
```
或者将脚本写入文件并从shell配置文件中获取：```bash
copilot completion bash > ~/.copilot-completion.bash
echo 'source ~/.copilot-completion.bash' >> ~/.bashrc
```
提示**：在添加完成脚本后重新加载shell （`source ~/.bashrc`或打开一个新终端）以使更改生效。

##常见问题

**问：如何为特定文件禁用副驾驶？**

答：使用`excludedFiles`设置在您的IDE配置或创建一个工作区设置，禁用Copilot的特定模式：```json
{
  "github.copilot.advanced": {
    "excludedFiles": [
      "**/secrets/**",
      "**/*.env",
      "**/test/fixtures/**"
    ]
  }
}
```
**问：我可以为每个项目设置不同的设置吗？**

答:是的!使用工作区设置（`.vscode/settings.json`）来实现不需要共享的特定于项目的首选项，或者使用存储库设置（例如，`.github/agents/`、`.github/skills/`、`.github/instructions/`和`.github/copilot-instructions.md`中的文件）来实现应该受版本控制的团队范围的自定义。

**Q：团队设置如何覆盖个人设置？**

答：存储库级的Copilot配置（如`.github/agents/`、`.github/skills/`、`.github/instructions/`和`.github/copilot-instructions.md`）具有最高的优先级，其次是工作空间设置，然后是用户设置。这意味着即使您的个人设置不同，团队定义的指示和代理也将适用，从而确保整个团队的一致性。

**问：我应该把适用于所有项目的自定义放在哪里？**答：在您的IDE中使用用户级设置来设置应该适用于任何地方的个人偏好。对于特定于技术或框架的定制（如React约定），请考虑在awesome-copilot-hub存储库中创建一个集合，以便在多个项目中引用。

##下一步

现在，您了解副驾驶配置，探索如何创建强大的自定义：

- **[什么是座席、技能和说明](../what-are-agents-skills-instructions/)** -了解可配置的自定义类型
- **[了解副驾驶上下文](../understanding-copilot-context/)** -了解配置如何影响上下文使用
- **[定义自定义指令](../defining-custom-instructions/)** -为您的项目创建持久的上下文
- **[创建有效的技能](../creating-effective-skills/)** -构建可重用的任务文件夹与捆绑的资产
- **[建立海关代理](../building-custom-agents/)** -开发专业助理