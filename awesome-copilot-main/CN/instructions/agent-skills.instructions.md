---
description: 'Guidelines for creating high-quality Agent Skills for GitHub Copilot'
applyTo: '**/skills/**/SKILL.md'
---
#座席技能文件指南

创建有效和可移植的代理技能的说明，这些技能通过专门的功能、工作流和捆绑的资源增强GitHub Copilot。

什么是座席技能？

Agent Skills是包含指令和捆绑资源的独立文件夹，用于教授AI代理专门的功能。与自定义指令（定义编码标准）不同，技能支持特定于任务的工作流，它可以包括脚本、示例、模板和参考数据。

主要特征:
- **便携式**：跨VS Code， Copilot CLI，和Copilot编码代理
- **渐进式加载**：仅在与用户请求相关时加载
- **资源捆绑**：可以包括脚本，模板，示例以及说明
—**按需**：根据提示的相关性自动激活

目录结构

技能存储在特定的位置：|位置|范围|推荐||----------|-------|----------------|
|`.github/skills/<skill-name>/`|Project/repository|推荐用于项目技能|
|`.claude/skills/<skill-name>/`|Project/repository| Legacy，用于向后兼容|
|`~/.copilot/skills/<skill-name>/`|个人（用户范围）|推荐用于个人技能|
|`~/.agents/skills/<skill-name>/`|个人（用户范围）|可选支持的个人技能目录|
|`~/.claude/skills/<skill-name>/`|个人（用户范围）| Legacy，用于向后兼容|

每个技能**必须**有自己的子目录，至少包含一个`SKILL.md`文件。SKILL.md格式

### Frontmatter （Required）```yaml
---
name: webapp-testing
description: 'Toolkit for testing local web applications using Playwright. Use when asked to verify frontend functionality, debug UI behavior, capture browser screenshots, check for visual regressions, or view browser console logs. Supports Chrome, Firefox, and WebKit browsers.'
license: Complete terms in LICENSE.txt
---
```
|字段|必选|约束条件||-------|----------|-------------|
|`name`|是|小写，连字符代替空格，最多64个字符（例如`webapp-testing`） |
|`description`|是| 10-1024个字符，清晰的功能和用例，用单引号括起来|
|`license`|否|LICENSE.txt的引用（如`Complete terms in LICENSE.txt`）或SPDX标识符|

最佳实践描述

**CRITICAL**:`description`字段是自动发现技能的PRIMARY机制。副驾驶只读取`name`和`description`来决定是否加载技能。如果你的描述是模糊的，该技能将永远不会被激活。

**描述内容：**
1. **技能能做什么（能力）
2. **何时使用它（特定的触发器、场景、文件类型或用户请求）
3. **用户可能在提示中提到的关键字**

* *好描述:* *```yaml
description: 'Toolkit for testing local web applications using Playwright. Use when asked to verify frontend functionality, debug UI behavior, capture browser screenshots, check for visual regressions, or view browser console logs. Supports Chrome, Firefox, and WebKit browsers.'
```
* *可怜的描述:* *```yaml
description: 'Web testing helpers'
```
糟糕的描述之所以失败，是因为：
-没有特定的触发（副驾驶什么时候加载这个？）
-没有关键字（什么用户提示将匹配？）
-没有能力（它到底能做什么？）

### Body内容

身体包含详细的说明，副驾驶加载后的技能被激活。建议部分:

|章节|用途||---------|---------|
|`# Title`|简要概述此技能使|成为可能
|`## When to Use This Skill`|场景列表（加强描述触发器）|
|`## Prerequisites`|所需的工具、依赖项、环境设置（如果适用）|
|`## Step-by-Step Workflows`|可重复过程（构建、部署、设置）的编号步骤|
对非明显行为的主动警告（“不要因为Y而做X”）|
|`## Troubleshooting`|响应修复已知问题（“如果你看到X，尝试Y”）|
|`## References`|链接到捆绑文档或外部资源|

并非所有技能都需要每个部分。如果没有外部依赖项，则跳过`## Prerequisites`。如果该技能纯粹是建议的，则跳过`## Step-by-Step Workflows`。只要技能涉及外部工具、api或特定于平台的行为，就包括`## Gotchas`。

关于内容质量原则（包括什么和不包括什么），请参阅下面的[编写高影响技能]（# Writing - High-Impact - Skills）。

编写每个部分**`# Title`**——用一句话说明该技能支持什么。避免使用通用的措辞；具体说明领域。

**`## When to Use This Skill`** -加强描述触发器的具体场景的项目符号列表。这有助于副驾驶确认它加载了正确的技能。```markdown
## When to Use This Skill

- User asks to test a web application in a browser
- User needs to capture screenshots for visual regression testing
- User wants to debug frontend behavior with browser console logs
```
**`## Prerequisites`** -仅包括如果技能需要的工具，服务，或配置，副驾驶无法假设可用。列出准确的安装命令。```markdown
## Prerequisites

- [Playwright](https://playwright.dev/) installed: `npm install -D @playwright/test`
- At least one browser engine installed: `npx playwright install chromium`
```
**`## Step-by-Step Workflows`** -顺序重要的可重复过程（构建、部署、环境设置）的编号步骤。描述每个阶段要完成什么，而不是硬编码的文件路径或行号——步骤应该适应不同的项目结构。对于复杂的工作流（bbb50个步骤），分割成`references/`文件并链接到它们。```markdown
## Step-by-Step Workflows

### Deploy to Staging

1. Build the project: `npm run build`
2. Run pre-deploy validation: `npm run validate`
3. Deploy to staging: `npm run deploy -- --env staging`
4. Verify the health endpoint returns 200
```
**`## Gotchas`** -主动警告，防止错误。记录不明显的默认值、API怪癖、特定于版本的行为和常见陷阱。加粗关键约束，然后解释原因。```markdown
## Gotchas

- **Never** call `billing.charge()` without checking `user.hasPaymentMethod` first —
  the SDK throws an unrecoverable error instead of returning a failure.
- The `currency` field expects ISO 4217 codes, not display names.
  Copilot often writes "dollars" instead of "USD".
```
**`## Troubleshooting`** -针对已知问题的响应性修复，以症状表→解决方案对的形式呈现。每一行都应该是独立的和可操作的。```markdown
## Troubleshooting

| Issue | Solution |
|-------|----------|
| Plugin won't connect | Check servers are running (`npm run start:all`) |
| Browser blocks localhost | Allow local network access, or try a different browser |
| Tool execution times out | Ensure the plugin UI is open and shows "Connected" |
```
**`## References`** -链接到`references/`中的捆绑文档、外部文档或相关技能。对捆绑的文件使用相对路径。

##捆绑资源

技能可以包括Copilot按需访问的其他文件：

支持的资源类型

|文件夹|目的|加载到上下文？| . ||--------|---------|---------------------|---------------|
|`scripts/`|执行特定操作的可执行自动化|执行时|`helper.py`，`validate.sh`,`build.ts`|
|是的，当引用|`api_reference.md`，`schema.md`，`workflow_guide.md`|时
|`assets/`| **在输出中使用AS-IS**的静态文件（未被AI代理修改）|否|`logo.png`，`brand-template.pptx`,`custom-font.ttf`|
|`templates/`| **Startercode/scaffolds， AI代理修改**并建立在|之上是的，当引用|`viewer.html`（插入算法）时，`hello-world/`（扩展）|

目录结构示例```
.github/skills/my-skill/
├── SKILL.md              # Required: Main instructions
├── LICENSE.txt           # Recommended: License terms (Apache 2.0 typical)
├── scripts/              # Optional: Executable automation
│   ├── helper.py         # Python script
│   └── helper.ps1        # PowerShell script
├── references/           # Optional: Documentation loaded into context
│   ├── api_reference.md
│   ├── workflow-setup.md     # Detailed workflow (>5 steps)
│   └── workflow-deployment.md
├── assets/               # Optional: Static files used AS-IS in output
│   ├── baseline.png      # Reference image for comparison
│   └── report-template.html
└── templates/            # Optional: Starter code the AI agent modifies
    ├── scaffold.py       # Code scaffold the AI agent customizes
    └── config.template   # Config template the AI agent fills in
```
> **LICENSE.txt**：创建技能时，请从https://www.apache.org/licenses/LICENSE-2.0.txt下载Apache 2.0的license文件，并保存为`LICENSE.txt`。更新附录部分的版权年份和所有者。

资产vs模板：关键区别

**资产**是静态资源**在输出中未改变消耗**：
-嵌入到生成的文档中的`logo.png`—复制为输出格式的`report-template.html`-一个`custom-font.ttf`应用于文本渲染

**模板**是启动器code/scaffolds， ** AI代理会主动修改**；
- AI代理插入逻辑的`scaffold.py`-一个`config.template`， AI代理根据用户需求填写值
-一个`hello-world/`项目目录，AI代理扩展了新功能

**经验法则**：如果AI代理读取并构建文件内容→`templates/`。如果文件在输出中按原样使用，则→`assets/`。

引用SKILL.md中的资源使用相对路径来引用技能目录中的文件：```markdown
## Available Scripts

Run the [helper script](./scripts/helper.py) to automate common tasks.

See [API reference](./references/api_reference.md) for detailed documentation.

Use the [scaffold](./templates/scaffold.py) as a starting point.
```
渐进式加载架构

技能使用三级加载效率：

|等级|什么加载|当||-------|------------|------|
| 1。发现|`name`和`description`只|总是（轻量级元数据）|
| 2。指令|完整`SKILL.md`主体|当请求匹配描述|时
| 3。参考资料|脚本、示例、文档|仅当Copilot引用它们|时

这意味着:
-安装许多技能而不消耗上下文
-每个任务只加载相关内容
-资源不加载，直到明确需要

##内容指南

写作风格

-使用命令式语气：“运行”，“创建”，“配置”（而不是“你应该运行”）
-具体和可操作
—包含带参数的精确命令
-在有用的地方显示预期的输出
-保持各部分的重点和可浏览性

###脚本要求

当包含脚本时，首选跨平台语言：

|语言|用例||----------|----------|
|复杂自动化，数据处理|
PowerShell核心脚本|
|Node.js|基于javascript的工具|
|Bash/Shell|简单自动化任务|

最佳实践:
-包括help/usage文档（`--help`标志）
—以清晰的信息优雅地处理错误
—避免存储凭据或机密
—尽可能使用相对路径

何时捆绑脚本

包括脚本在你的技能：
-同样的代码会被代理反复重写
-确定性可靠性至关重要（例如，文件操作，API调用）
-复杂的逻辑受益于预先测试，而不是每次生成
—操作具有自包含的目的，可以独立发展
-可测试性问题-脚本可以进行单元测试和验证
—可预测行为优先于动态生成脚本支持进化：即使是简单的操作，当它们可能变得越来越复杂，需要跨调用的一致行为，或者需要未来的可扩展性时，也可以通过脚本实现。

安全考虑

-脚本依赖于现有的凭据助手（没有凭据存储）
-仅在破坏性操作中包含`--force`标志
—对用户进行不可逆操作前进行警告
-记录任何网络操作或外部呼叫

##写作高影响力技能

关注副驾驶不知道的事情不要包括Copilot已经从其训练数据中了解的信息-标准语言语法，通用库用法或记录良好的API行为。一项技能中的每一行都应该教给副驾驶一些否则会出错或完全错过的东西。如果这些信息是在官方文件的第一页，那就把它删掉。关注改变Copilot行为的内部约定、不明显的默认值、特定于版本的怪癖和特定于领域的工作流。

背景预算意识在发现过程中，所有技能描述共享可用上下文窗口的有限部分。你的描述与副驾驶注意到的其他所有已安装的技能相竞争。保持描述的简洁和关键字的密集-以最短的文本为目标，仍然传达什么，什么时候，和相关的关键字。冗长的描述不仅会浪费你的预算；它们降低了系统中其他技能的可见性。

陷阱是你的最高信号内容`## Gotchas`部分始终是任何技能中最有价值的部分——在错误发生之前进行主动警告。这与`## Troubleshooting`不同，后者在出现问题后提供响应式修复。把问题当成一个活生生的部分：每次副驾驶产生错误的结果，就添加一个问题。加粗键约束，然后解释原因（例如，“**永远**不要在没有先检查`Y`之前调用`X()`——SDK会抛出一个不可恢复的错误”）。

比起死板的步骤，更喜欢灵活的指导方针

只有在顺序真正重要的情况下，才对具体的、可重复的过程（构建、部署、环境设置）使用编号的步骤。对于开放式任务（调试、重构、代码审查），提供决策标准和参考信息——Copilot需要灵活性来适应用户的具体情况。```markdown
# ❌ Too rigid
1. Open the file at src/api/handlers.ts
2. Find the function named processOrder
3. Add a try-catch block around lines 45-60

# ✅ Flexible
When fixing error handling in API handlers:
- Ensure all database operations have proper error handling
- Use the project's ErrorHandler utility (see ./references/error-handling.md)
- Log errors with enough context to debug in production
```
###对于大型技能使用渐进披露

如果您的SKILL.md超过200行，请考虑将详细内容分成子目录。这减少了上下文消耗——Copilot最初只加载核心指令，并根据需要提取参考材料。```markdown
## Reference Files

- `references/api.md` — complete function signatures and return types
- `references/error-codes.md` — every error code this service can return
- `scripts/validate.sh` — run this after making changes to verify correctness

Read these files as needed for your current task. Do not read them all upfront.
```
##常见模式

参数表模式

文件参数明确：```markdown
| Parameter | Required | Default | Description |
|-----------|----------|---------|-------------|
| `--input` | Yes | - | Input file or URL to process |
| `--action` | Yes | - | Action to perform |
| `--verbose` | No | `false` | Enable verbose output |
```
工作流执行模式

当执行多步骤工作流时，创建一个TODO列表，其中每个步骤引用相关文档：```markdown
## TODO
- [ ] Step 1: Configure environment - see [workflow-setup.md](./references/workflow-setup.md#environment)
- [ ] Step 2: Build project - see [workflow-setup.md](./references/workflow-setup.md#build)
- [ ] Step 3: Deploy to staging - see [workflow-deployment.md](./references/workflow-deployment.md#staging)
- [ ] Step 4: Run validation - see [workflow-deployment.md](./references/workflow-deployment.md#validation)
- [ ] Step 5: Deploy to production - see [workflow-deployment.md](./references/workflow-deployment.md#production)
```
这确保了可追溯性，并允许在中断时恢复工作流。

验证检查表

在发布技能之前：- []`SKILL.md`与`name`和`description`有有效的正面关系
—[]`name`为小写+连字符，长度≤64个字符
- []`description`清楚地说明**什么**它做**，**何时**使用它，以及相关的**关键词**
- []`description`简洁且关键字密集（尊重上下文预算）
- [] Body关注副驾驶无法从训练数据中了解的信息
-[]主体包括何时使用，先决条件（如果适用）和核心说明
- []`## Gotchas`部分，如果技能涉及不明显的行为，API怪癖，或常见的陷阱
- []SKILL.md正文在500行以下（考虑在200行左右分割成`references/`， 500是硬最大值）
-[]大的工作流程（>5步）分成`references/`文件夹与SKILL.md清晰的链接
-[]脚本包括帮助文档和错误处理
-[]所有资源引用的相对路径
-[]没有硬编码的凭据或秘密##相关资源

-[座席技能规格]（https://agentskills.io/）
- [VS Code座席技能文档]（https://code.visualstudio.com/docs/copilot/customization/agent-skills）
-[参考技能库]（https://github.com/anthropics/skills）
-[出色的副驾驶技能]（https://github.com/github/awesome-copilot/blob/main/docs/README.skills.md）