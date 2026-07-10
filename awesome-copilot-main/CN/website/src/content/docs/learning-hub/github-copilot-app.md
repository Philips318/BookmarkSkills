---
title: 'Getting Started with the GitHub Copilot app'
description: 'Learn about the GitHub Copilot app, a desktop experience built for agent-native development. Understand its key features and who it''s for.'
authors:
  - GitHub Copilot Learning Hub Team
lastUpdated: 2026-06-17
estimatedReadingTime: '8 minutes'
tags:
  - copilot-app
  - desktop
  - agents
  - parallel-work
relatedArticles:
  - ./using-automations-in-copilot-app.md
  - ./using-copilot-coding-agent.md
  - ./agentic-workflows.md
  - ./what-are-agents-skills-instructions.md
prerequisites:
  - Understanding of GitHub Copilot agents
  - Copilot Pro, Pro+, Business, or Enterprise plan
---
GitHub Copilot应用程序是为代理原生开发而从头构建的桌面体验。随着代理成为开发工作流程的中心部分，您需要一个地方，可以看到多个代理并行工作，检查它们的进度，并在需要时进行控制，所有这些都不会在窗口之间切换上下文或丢失正在运行的跟踪。

本指南涵盖了什么是Copilot应用程序，它的主要功能，以及如何开始。

什么是GitHub Copilot应用程序？

Copilot应用程序是一个独立的桌面应用程序，作为代理开发的控制中心。与通过GitHub.com的拉请求、问题和CLI窗口管理代理不同，Copilot应用程序将所有内容整合到一个统一的界面中。把它想象成一个指挥中心，你可以：
-看到你所有的活跃的工作一目了然
-启动多个代理同时处理不同的任务
-实时检查每个代理正在做什么
-在任务中重定向代理或批准其更改
-让代理处理自动化（如合并pr），而你专注于其他地方

与现有的Copilot体验的关键区别在于，该应用程序是专门为并行代理工作而构建的。它可以自动处理管理多个孤立环境、分支和工作树的复杂性，因此您不必这样做。

##主要特性

我的工作视图

副驾驶应用程序的中心枢纽是**我的工作**视图。这个仪表板显示：- **活动会话**：在一个任务上工作的每个代理都有自己的隔离会话
- **Issues and pr **：来自连接存储库的工作项收件箱
- **后台自动化**：在后台运行的任务，如Agent Merge处理你的pull请求
- **总体状态**：快速概述正在进行的内容，已完成的内容以及阻止的内容

而不是检查GitHub，您的CLI和VS Code更新，一切都在一个地方。

# # #自动化

Copilot应用程序包括内置的自动化功能，可以使用相同的代理技术为您运行计划任务。您可以使用现成的模板，也可以创建自己的模板。

自动化在存储库的上下文中运行，因此它们可以访问问题、提取请求和代码。您还可以选择它们是作为计划、交互式会话还是自动运行。

隔离工作树用于并行工作Copilot应用程序创建的每个会话都运行在它自己的git工作树中——一个真实的、独立的分支副本。这对于并行代理工作至关重要：

—多个代理可以同时处理不同的任务，而不会相互踩踏
-每个代理都有自己的分支，自己的环境和自己的变化
-不需要手动处理分支或清理-应用程序处理这一切
-您可以从任何设备上的任何工作树上拾取会话

这使得分派多个代理并信任它们不会相互干扰变得容易。

# # #画布

**画布**是您和代理协作的交互式工作界面。而不是长聊天线程，画布显示实际工作：画布可能显示一个计划，一个pull request diff，一个终端输出，或者一个实时的浏览器会话
-代理在工作时更新画布，您可以在同一表面上编辑，批准或重定向更改
-这可以很容易地看到代理正在做什么，并在需要时介入

有关使用`/create-canvas`构建画布的实践指南，请参阅[使用画布扩展]（../working-with-canvas-extensions/）。

代理合并

**代理合并**是一个功能，可以携带您的拉请求通过整个工作流程：

—监视CI/CD管道并等待检查通过
-处理失败的测试或检查错误
跟踪所需的审核人员并等待批准
—当满足所有条件时，可以自动合并您可以控制自动化级别——决定Agent Merge是否应该只运行CI、处理反馈，还是一直进行合并。这是一种让Copilot处理审查和合并过程中繁琐部分的方法。

##谁是副驾驶应用程序？

Copilot应用程序并不是现有Copilot体验的替代品——它是工具箱中的另一个工具。以下是它最适合的人群：

想要指导多个代理的开发者

如果你经常使用代理，并且需要管理并行工作，那么Copilot应用程序可以为你提供一个专门的控制中心。而不是检查多个窗口，你看到所有的东西在一个地方。

非开发人员角色的团队成员与VS Code或CLI等以开发人员为中心的体验相比，Copilot应用程序具有更易于访问、桌面优先的界面。这使得它吸引了业务分析师、产品经理和其他技术团队成员，他们希望与代理一起工作，但发现传统的开发人员工具势不可挡。

利用并行代理工作的团队

该应用程序的工作树架构可以很自然地将多个代理分配到不同的任务上，而无需协调。如果你的团队经常有代理同时处理多个项目，那么这个应用程序就是为这个工作流程而构建的。

喜欢图形界面的开发者

虽然CLI功能强大，但一些开发人员更喜欢使用可视化界面来完成常见任务。Copilot应用程序提供了gui优先的体验，同时还提供了代理、钩子、技能和自定义说明的所有功能。与其他副驾驶经验的比较

体验|最适合|力量||------------|----------|----------|
| **Copilot CLI** |终端中的开发人员|原始功能，可编写脚本，始终在您的shell |中可用
| **VS Code扩展** |编码和实时人工智能辅助|与您的编辑器集成，即时反馈|
| **GitHub.com** |代码审查和公关管理|中央枢纽协作，始终可访问的web |
| **Copilot App** |指导并行代理，可视化工作流|代理开发控制中心，多代理管理|

Copilot应用程序补充了这些体验——您仍然可以使用VS Code进行编码，使用CLI进行自动化，使用GitHub.com进行协作。Copilot应用填补了一个特定的空白：用一个统一的界面并行管理多个代理。

##开始

# # #要求

要使用GitHub Copilot应用程序，您需要：- A **GitHub CopilotPro, Pro+，商务，企业计划**
—兼容的操作系统（macOS、Windows或Linux）
-连接GitHub存储库

# # #的安装

1. 访问[GitHub Copilotapp]（https://github.com/features/ai/github-app）并为您的平台下载安装程序
2. 安装并启动应用程序
3. 使用您的GitHub帐户进行身份验证
4. 连接您的存储库

创建你的第一个会话

安装后，您可以通过以下方式创建会话：

1. **从一个问题**：分配一个GitHub问题到Copilot，应用程序将创建一个会话来解决它
2. **从提示**：打开副驾驶应用程序，并描述你想做什么（例如，“修复登录错误”或“添加暗模式支持”）
3. **从你的收件箱**：应用程序同步你的GitHub收件箱-点击一个问题，并开始一个会话

每个会话在自己的工作树中运行，具有自己的隔离环境。您可以并行运行多个会话。从带有深度链接的终端启动会话GitHub Copilot应用程序支持URL深度链接。当你想要打开应用程序或直接从终端工作流开始会话时，这很有用。

支持方案:

-`ghapp://`（规范）
——`github-app://`——`gh://`在下面的示例中，用您的存储库替换`owner/repo`。

####打开一个新的会话

使用`session/new`路由：```bash
# Basic new session
open "ghapp://session/new?repo=owner/repo"

# Start from a branch
open "ghapp://session/new?repo=owner/repo&branch=main"

# Start from a pull request
open "ghapp://session/new?repo=owner/repo&pr=1234"

# Start with a kickoff prompt
open "ghapp://session/new?repo=owner/repo&prompt=fix%20the%20flaky%20test"

# Set the initial session mode
open "ghapp://session/new?repo=owner/repo&mode=plan"
```
`session/new`支持:

-`repo`（**required**，格式`owner/repo`）
—`pr`（整数，与`branch`互斥）
—`branch`（与`pr`互斥）
-`prompt`（url编码文本）
-`mode`（`plan`、`interactive`、`autopilot`）

####其他有用的深层链接

-`ghapp://repo/owner/repo`-打开（或克隆）一个repo到项目
-`ghapp://clone/owner/repo`-克隆一个repo
-`ghapp://sessions/<sessionId>`—打开已存在的会话
-`ghapp://chats`-打开聊天
-`ghapp://mywork`-打开My Work视图
-`ghapp://recent`-打开最近的工作区
-`ghapp://workflows`-开放自动化
-`ghapp://owner/repo/issues/123`-打开issue
-`ghapp://owner/repo/pull/456`-打开拉取请求

####重要限制

-深度链接以仓库为中心，期望`owner/repo`。
-没有直接打开任意本地文件夹的深层链接。
-对于本地文件夹，使用应用程序的**添加本地文件夹**流程；如果该文件夹已经是带有`github.com`远程的Git存储库，那么将该远程解析为`owner/repo`并使用`session/new`。了解会话工作流

下面是创建会话时发生的情况：```
1. You describe the work or assign an issue
          ↓
2. Copilot app creates an isolated worktree
          ↓
3. The agent reads your issue, instructions, and codebase
          ↓
4. It plans and implements a solution
          ↓
5. You can monitor progress in the My Work view
          ↓
6. You can redirect the agent or let it finish
          ↓
7. Changes are ready for review (either a PR or approval)
```
连接存储库

让Copilot访问您的存储库：

1. 在Copilot应用程序中，打开**设置**→**连接存储库**
2. 点击**添加存储库**并从您的GitHub帐户中选择存储库
3. 授予必要的权限
4. 应用程序现在可以访问你的代码、问题和pull请求

##使用自定义配置的副驾驶应用程序

副驾驶的应用程序尊重所有现有的GitHub Copilot自定义：

- **海关代理** （`.agent.md`文件中的`.github/agents/`）
- **技能** （`.github/skills/`专业任务指导）
- **说明书**（编码标准为`.github/instructions/`）
- **Hooks** （`.github/hooks/`中的自动检查和格式化）
- **安装步骤** （`.github/copilot-setup-steps.yml`）

如果您还没有设置自定义代理，技能或说明，请参阅[副驾驶配置基础知识]（../copilot-configuration-basics/）开始。

##通用工作流

###并行Bug修复创建多个会话来同时修复不同的bug：

1. 打开副驾驶应用程序
2. 为“修复登录超时问题”创建会话
3. 同时，为“修复暗模式按钮样式”创建另一个会话
4. 在My Work视图中监视两者
5. 独立审查和合并每个PR

并行特性开发

在不同的sprint中为代理分配多个特征：

1. 连接问题跟踪器
2. 让Copilot从你的待办事项列表中提取功能
3. 为每个特性创建一个会话
4. 每个代理在自己的工作树中独立工作
5. pr着陆时不会互相干扰

自动PR合并与代理合并

启用Agent Merge以自动执行日常PR工作流程：1. 在Copilot应用程序设置中配置Agent Merge
2. 指定要启用哪些自动化（运行CI、地址反馈、合并）
3. 创建一个会话来实现一个特性
4. 创建PR后，Agent Merge将对其进行监视
5. 它运行CI，等待审查，处理反馈，并在准备好时进行合并

##下一步

- **设置您的存储库**:[Copilot配置基础知识](../copilot-configuration-basics/) -添加自定义代理，技能和说明
- **理解代理技能**:[创建有效技能](../creating-effective-skills/) -构建可重用的任务指南
- **自动化与挂钩**:[自动化与挂钩](../automating-with-hooks/) -添加护栏自动工作

---