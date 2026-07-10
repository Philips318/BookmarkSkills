---
title: 'GitHub Copilot Terminology Glossary'
description: 'A quick reference guide defining common GitHub Copilot and platform-specific terms.'
authors:
  - GitHub Copilot Learning Hub Team
lastUpdated: 2026-04-02
estimatedReadingTime: '8 minutes'
tags:
  - glossary
  - terminology
  - reference
relatedArticles:
  - ./what-are-agents-skills-instructions.md
  - ./copilot-configuration-basics.md
---
对GitHub Copilot定制不熟悉？本术语表定义了在探索Awesome GitHub Copilot生态系统中的代理、技能、指令和相关概念时可能遇到的常用术语。

在学习中心阅读文章或浏览存储库时，可以使用此页面作为快速参考。

---

##核心概念

# # #代理

一个专门的配置文件（`*.agent.md`），它定义了具有特定专业知识、工具和行为模式的GitHub Copilot角色或助手。在支持委托的产品中，代理通常是主要协调器或主要会话角色，而子代理则处理较窄的委托任务。

**何时使用**：用于从深度工具集成和持久会话上下文中受益的重复工作流程。

**了解更多**:[什么是代理，技能和说明]（../what-are-agents-skills-instructions/）

---

# # #副代理人由另一个代理或编排器启动的以任务为中心的临时代理。子代理通常获得更窄的提示，它自己的孤立上下文窗口，并将摘要返回给主代理，而不是停留在主会话中。

**何时使用**：用于孤立研究，并行分析，专门审查通过或委托实施步骤。

**了解更多**:[代理和子代理]（../agents-and-subagents/）

---

内置工具GitHub Copilot提供的本机功能，无需额外配置或MCP服务器。示例包括代码搜索、文件编辑、终端命令执行和web搜索。内置工具总是可用的，不需要安装。

相关术语：[Tools](# Tools), [MCP]（# MCP -model-context-protocol）

---

###聊天模式

**弃用术语** -该术语不再使用。使用[Agent]（# Agent）代替。以前，“聊天模式”是[Agent]（# Agent）的替代术语，它描述了如何将GitHub Copilotchat转换为特定于领域的助手。生态系统已经将“代理”作为首选术语进行了标准化。

* *看到* *(代理):(#代理)

---

# # #收集

**注**：集合是特定于Awesome GitHub Copilot存储库的概念，不是标准GitHub Copilot术语的一部分。

围绕特定主题或工作流程组织起来的相关技能、指示和代理的精心组合。集合在`collections/`目录下的YAML文件（`*.collection.yml`）中定义，并帮助用户一起发现相关的定制。

**示例**：“Awesome Copilot”集合捆绑了发现和生成GitHub Copilot自定义的元技能。

**了解更多**:[什么是代理，技能和说明]（../what-are-agents-skills-instructions/）

---

自定义代理(代理)(#代理)。术语“定制”强调这些是用户定义的配置，而不是GitHub Copilot的默认行为。任何人都可以创建自定义代理，并通过像Awesome GitHub Copilot这样的存储库共享。

---

自定义指令

看到(指令)(#指令)。术语“定制”强调这些是用户定义的规则，而不是GitHub Copilot的内置理解。自定义指令对于编写特定于团队的标准和体系结构决策特别有用。

---

##配置和元数据

###正面问题

放置在Markdown文件开头（在`---`分隔符之间）的YAML元数据，提供有关文件的结构化信息并控制其行为。在这个存储库中，前端内容通常包括`name`、`description`、`mode`、`model`、`tools`和`applyTo`等字段。最重要的是控制：
—**工具访问**：自定义可以使用哪些内置工具和MCP工具
- **模型选择**：哪个AI模型支持自定义
- **范围**：自定义适用的地方（例如，`applyTo`模式的指令）

**注**：并非所有字段在所有自定义类型中都是通用的。请参阅有关代理、技能或说明的特定文档，了解哪些字段适用于每种类型。

* * * *例子:```yaml
---
name: 'React Component Generator'
description: 'Generate modern React components with TypeScript'
mode: 'agent'
tools: ['codebase']
---
```
**用于**：技能，代理，说明和学习中心文章。

---

# # #切换VS Code自定义代理前端属性（`handoffs`），它定义了从一个代理到另一个代理的建议转换，通常带有预先填充的后续提示。交接对于诸如研究->实施或计划->审查等有指导的工作流程非常有用。

**重要**:GitHub的[自定义代理配置参考]（../building-custom-agents/#agent-configuration-reference）说`handoffs`目前被GitHub.com上的副驾驶云代理忽略，所以这个概念不能在每个副驾驶表面上移植。

**了解更多**:[代理和子代理](../agents-and-subagents/)，[建立自定义代理]（../building-custom-agents/）

---### AGENTS.md
一种新兴的行业标准文件格式，用于定义可移植的AI编码指令，可跨不同的AI编码工具（GitHub Copilot、Claude、Codex等）工作。`AGENTS.md`文件通常放在存储库根目录或`.github/`目录中，其中包含有关AI助手应如何与代码库交互的说明。

与特定于工具的定制文件（`.agent.md`、`.prompt.md`、`.instructions.md`）不同，`AGENTS.md`旨在提供一种标准化的、与平台无关的方式来定义可被多个工具使用的AI行为。

* * * *关键特征:
-跨平台兼容性的平台无关格式
-通常包含项目上下文、编码标准和架构指南
—位于存储库根目录或`.github/`目录

**了解更多**:[AGENTS.md规格]（https://agents.md/）

**相关术语**:[指令]（#指令），[前台事项]（#前台事项）

---

# # #指令一个配置文件（`*.instructions.md`），它提供持久的背景上下文和编码标准，GitHub Copilot在处理匹配文件时读取这些标准。说明包含风格指南、特定于框架的提示，以及帮助Copilot自动与您的工程实践保持一致的存储库规则。

**何时使用**：用于适用于许多会议的长期指导，如编码标准或遵从性要求。

**了解更多**:[什么是代理，技能和指令](../what-are-agents-skills-instructions/)，[定义自定义指令]（../defining-custom-instructions/）

---

技能和互动

# # #角色

为[Agent]（# Agent）定义的身份、语气和行为特征。精心设计的角色可以帮助GitHub Copilot对特定领域或专业领域做出一致且适当的响应。**示例**：“数据库性能专家”角色可能会优先考虑查询优化并使用特定于数据库的术语解释概念。

**相关术语**:[代理]（# Agent）

---

# # #提示

**已弃用** -提示符（`*.prompt.md`）是可重用的聊天模板，捕获特定的任务或工作流，使用GitHub Copilot聊天中的`/`命令调用。提示已经被[Skills]（#skill）所取代，后者提供了相同的斜杠命令调用以及代理发现、捆绑资产和跨平台可移植性。

如果您有现有的提示，请考虑将它们迁移到技能中。参考[创造有效技能]（../creating-effective-skills/）。

* *看到* *(技能):(#技能)

---

# # #技能一个自包含的文件夹，其中包含`SKILL.md`文件和可选的捆绑资产（参考文档、模板、脚本），这些资产打包了GitHub Copilot的可重用功能。技能遵循开放的[Agent Skills specification](https://agentskills.io/home)，可以由用户通过`/command`调用，也可以由代理自动发现和调用。

* * * *关键好处:
- **代理发现**：扩展的前台让代理自动发现和调用技能
- **捆绑资产**：参考文件、模板和脚本提供更丰富的上下文
- **跨平台**：可通过代理技能规范跨编码代理系统移植

**示例**：一个`/generate-tests`技能可能包括一个带有测试指令的`SKILL.md`、一个带有通用模式的`references/test-patterns.md`和一个`templates/test-template.ts`启动文件。

**何时使用**：用于标准化Copilot如何响应重复任务，特别是当捆绑资源提高质量时。**了解更多**:[什么是代理，技能和说明](../what-are-agents-skills-instructions/)，[创建有效技能]（../creating-effective-skills/）

---

平台与整合

MCP（模型上下文协议）

用于将AI助手（如GitHub Copilot）连接到外部数据源、工具和服务的标准化协议。MCP服务器充当桥梁，允许Copilot与api、数据库、文件系统和其他超出其内置功能的资源进行交互。

**示例**:MCP服务器可能提供对贵公司内部文档、AWS资源或特定数据库系统的访问。

**了解更多**:[模型上下文协议](https://modelcontextprotocol.io/) | [MCP规范](https://spec.modelcontextprotocol.io/) |[理解MCP服务器]（../understanding-mcp-servers/）

相关术语**:[工具](# Tools)，[内置工具]（# Built-in - Tool）

---

# # #钩在Copilot代理会话期间自动响应生命周期事件运行的shell命令或脚本。钩子以JSON文件的形式存储在`.github/hooks/`中，可以触发会话start/end、提示提交、before/after工具使用以及发生错误等事件。它们提供确定性的自动化——检查、格式化、治理扫描——而不依赖于人工智能记住去做这些事情。

**示例**:`postToolUse`钩子，在代理编辑文件后运行得更漂亮，或者`preToolUse`钩子，阻止危险的shell命令。

**何时使用**：用于必须可靠发生的确定性自动化，如格式化代码、运行编译器或审核遵从性提示。

**了解更多**:[自动化与挂钩]（../automating-with-hooks/）

相关术语：[代理](# Agent)，[编码代理]（# Coding - Agent）

---

编码代理自主的GitHub Copilot代理，可在云环境中处理问题，无需持续的人工指导。您将问题分配给Copilot，它会启动开发环境，实现解决方案，运行测试，并打开拉取请求以进行审查。

* * * *关键特征:
—运行在隔离的云环境中
-使用存储库的指令、代理、技能和钩子
-总是产生pr -它不能合并或部署
-通过PR评论支持迭代

**何时使用**：用于定义良好的任务，具有明确的可接受标准，可以自主完成。

**了解更多**:[使用副驾驶编码代理]（../using-copilot-coding-agent/）

相关术语：[Agent](# Agent), [Hook]（# Hook）

---

# # #插件一个可安装的包，通过一组捆绑的代理、技能、钩子、MCP服务器配置和LSP集成扩展GitHub CopilotCLI。插件提供了一种在项目和团队之间分发和共享自定义功能的方法，包括版本控制、发现和通过市场的一命令安装。

**示例**：安装`database-data-management@awesome-copilot`以在单个命令中获得数据库专家代理、迁移技能和模式验证挂钩。

**何时使用**：当您希望在多个项目或团队成员之间共享一组精心策划的Copilot功能时，或者当您希望安装社区贡献的工具而无需手动复制文件时。

**了解更多**:[安装和使用插件]（../installing-and-using-plugins/）

* *相关术语* *(代理):(#代理),(技能)(#技能),(钩)(#钩)

---

# # #工具GitHub Copilot可以调用这些功能来执行操作或检索信息。工具分为两类：

1. **内置工具**：原生功能，如`codebase`（代码搜索），`terminalCommand`（运行命令）和`web`（网页搜索）
2. **MCP工具**：由MCP服务器提供的外部集成（例如，数据库查询、云资源管理或API调用）

代理和技能可以指定他们需要或推荐哪些工具。

**前面内容示例**：```yaml
tools: ['codebase', 'terminalCommand', 'github']
```
相关术语：[MCP](# MCP -model-context-protocol), [Built-in - Tool](# Built-in - Tool), [Agent]（# Agent）

---

**你有想要添加的术语吗？**欢迎投稿！请参阅我们的[贡献指南]（https://github.com/github/awesome-copilot/blob/main/CONTRIBUTING.md）了解如何建议对本术语表进行补充。