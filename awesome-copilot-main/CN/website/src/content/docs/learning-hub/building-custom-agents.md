---
title: 'Building Custom Agents'
description: 'Learn how to create specialized GitHub Copilot agents with custom personas, tool integrations, and domain expertise.'
authors:
  - GitHub Copilot Learning Hub Team
lastUpdated: 2026-07-06
estimatedReadingTime: '10 minutes'
tags:
  - agents
  - customization
  - fundamentals
relatedArticles:
  - ./what-are-agents-skills-instructions.md
  - ./agents-and-subagents.md
  - ./creating-effective-skills.md
  - ./understanding-mcp-servers.md
prerequisites:
  - Basic understanding of GitHub Copilot chat
  - Familiarity with agents, skills, and instructions
---
定制代理是为GitHub Copilot提供重点人物、特定工具访问和领域专业知识的专门助手。与指令（被动应用）或技能（处理单个任务）不同，代理定义了一种完整的工作风格——它们塑造了Copilot的思维方式、使用的工具以及在整个会话中如何沟通。

本文向您展示了如何为团队的工作流设计、构建和部署有效的代理。

什么是客户代理？

定制代理是Markdown文件（`*.agent.md`），它通过以下方式配置GitHub Copilot：

- **角色：座席的专业知识、语气和工作方式
—**工具接入**：座席可使用的内置工具和MCP服务器
—**护栏**：座席遵循的边界和约定
- **模型偏好**：哪个AI模型为代理提供动力（可选但推荐）当用户在VS Code中选择自定义代理或通过Copilot编码代理将其分配给某个问题时，代理的配置将塑造整个交互。

* * * *要点:
—代理在对话中持续存在—他们保持自己的角色和上下文
—代理可以调用工具、运行命令、搜索代码库、与MCP服务器交互
—多个代理可以共存于一个存储库中，每个代理服务于不同的工作流
座席存储在`.github/agents/`中，并与整个团队共享

代理与其他自定义的区别

**代理与说明**：
-明确选择代理；指令自动应用于匹配的文件
-代理定义一个完整的角色；指令提供被动的背景信息
-使用代理进行交互式工作流程；使用编码标准的说明**代理vs技能**：
-代理是持久的人物角色；技能是单一任务的能力
—座席可以在对话中调用技能
-使用代理处理复杂的多步骤工作流程；将技能用于专注的、可重复的任务

##代理剖析

每个代理文件都有两个部分：YAML标题和Markdown指令。

### Frontmatter Fields```yaml
---
name: 'Security Reviewer'
description: 'Expert security auditor that reviews code for OWASP vulnerabilities, authentication flaws, and supply chain risks'
model: Claude Sonnet 4
tools: ['codebase', 'terminal', 'github']
---
```
**name**（推荐）：代理的可读显示名称。

**描述**（必需）：对代理工作的清晰总结。这将显示在代理选择器中，并帮助用户找到正确的代理。

**model**（推荐）：为座席提供动力的AI模型。根据任务的复杂性进行选择——使用更有能力的模型进行细致入微的推理。

**reasoningEffort** *(v1.0.66+)*：覆盖该代理的推理努力级别。接受的值为`low`、`medium`和`high`。这使您可以将特定的代理固定到cost/quality中，而不考虑用户的全局设置—例如，快速代码格式化代理可以使用`low`，而安全审查人员则使用`high`：```yaml
---
name: 'Security Reviewer'
description: 'Thorough security audit for OWASP vulnerabilities'
model: Claude Sonnet 4
reasoningEffort: high
tools: ['codebase', 'terminal', 'github']
---
```
**tools**（推荐）：代理可访问的一系列内置工具和MCP服务器。常用工具包括：

|工具|用途||------|---------|
|`codebase`|跨存储库|搜索和分析代码
|`terminal`|执行shell命令|
|`github`|与GitHub api交互（问题，pr等）|
|`fetch`|向外部api发出HTTP请求|
|`edit`|修改工作空间|中的文件

对于MCP服务器工具，通过服务器名称引用它们（例如，`postgres`,`docker`）。详细信息请参见[了解MCP服务器]（../understanding-mcp-servers/）。

###代理指令

在前置事项之后，写下定义代理行为的Markdown指令。清晰地组织它们：````markdown
---
name: 'API Design Reviewer'
description: 'Reviews API designs for consistency, RESTful patterns, and team conventions'
model: Claude Sonnet 4
tools: ['codebase', 'github']
---

# API Design Reviewer

You are an expert API designer who reviews endpoints, schemas, and contracts for consistency and best practices.

## Your Expertise

- RESTful API design patterns
- OpenAPI/Swagger specification
- Versioning strategies
- Error response standards
- Pagination and filtering patterns

## Review Checklist

When reviewing API changes:

1. **Naming**: Verify endpoints use plural nouns, consistent casing
2. **HTTP Methods**: Confirm correct verb usage (GET for reads, POST for creates)
3. **Status Codes**: Check appropriate codes (201 for creation, 404 for not found)
4. **Error Responses**: Ensure structured error objects with codes and messages
5. **Pagination**: Verify cursor-based pagination for list endpoints
6. **Versioning**: Confirm API version is specified in the path or header

## Output Format

Present findings as:
- 🔴 **Breaking**: Changes that break existing clients
- 🟡 **Warning**: Patterns that should be improved
- 🟢 **Good**: Patterns that follow our conventions
````
##设计模式

领域专家

创建对特定技术有深入了解的代理；```markdown
---
name: 'Terraform Expert'
description: 'Infrastructure-as-code specialist for Terraform on Azure with security-first defaults'
model: Claude Sonnet 4
tools: ['codebase', 'terminal']
---

You are an expert in Terraform and Azure infrastructure.

## Principles

- Security-first: always enable encryption, disable public access by default
- Use variables for all configurable values—never hardcode
- Apply consistent tagging strategy across all resources
- Follow Azure naming conventions: {env}-{project}-{resource-type}
- Include diagnostic settings for all resources that support them
```
###工作流自动化

创建执行多步骤流程的代理：```markdown
---
name: 'Release Manager'
description: 'Automates release preparation including changelog generation, version bumping, and tag creation'
model: Claude Sonnet 4
tools: ['codebase', 'terminal', 'github']
---

You are a release manager who automates the release process.

## Workflow

1. Analyze commits since last release using conventional commit format
2. Determine version bump (major/minor/patch) based on commit types
3. Generate changelog from commit messages
4. Update version in package.json / pyproject.toml
5. Create a release summary for the PR description

## Rules

- Never skip the changelog step
- Always verify the test suite passes before proceeding
- Ask for confirmation before creating tags or releases
```
质量门

创建执行标准的代理：

b> **内置`/security-review`**：在创建自定义安全审查代理之前，请注意GitHub CopilotCLI包含一个内置的`/security-review`命令（自v1.0.64以来所有用户都可以使用）。它对阶段性更改或指定文件执行以安全为重点的分析。自定义安全审查代理对于特定于领域的规则、团队约定以及与MCP工具（如Sentry或SAST平台）的深度集成仍然很有价值。```markdown
---
name: 'Accessibility Auditor'
description: 'Reviews UI components for WCAG 2.1 AA compliance and accessibility best practices'
model: Claude Sonnet 4
tools: ['codebase']
---

You are an accessibility expert who reviews UI components for WCAG compliance.

## Audit Areas

- Semantic HTML structure
- ARIA attributes and roles
- Keyboard navigation support
- Color contrast ratios (minimum 4.5:1 for text)
- Screen reader compatibility
- Focus management in dynamic content

## When Reviewing

- Check every interactive element has an accessible name
- Verify form inputs have associated labels
- Ensure images have meaningful alt text (or empty alt for decorative)
- Test that all functionality is keyboard-accessible
```
连接代理到MCP服务器

当通过MCP服务器连接到外部工具时，代理将变得更加强大。参考`tools`数组中的MCP工具：```yaml
---
name: 'Database Administrator'
description: 'Expert DBA for PostgreSQL performance tuning, query optimization, and schema design'
tools: ['codebase', 'terminal', 'postgres-mcp']
---
```
然后，代理可以查询数据库、分析查询计划并提出优化建议——所有这些都在对话中进行。详细设置请参见[了解MCP服务器]（../understanding-mcp-servers/）。

最佳实践

###编写有效的座席角色

- **具体说明专业知识**：“精通React 18+和TypeScript”胜过“前端开发人员”
- **定义工作方式**：代理人是否应该提出澄清性问题或做出假设？应该简洁还是彻底？
- **包括护栏**：代理不应该做什么？（“永远不要直接修改产品配置文件”）
- **提供示例**：显示您期望的输出格式（审阅注释，代码模式等）

选择正确的模式

|场景|推荐型号||----------|-------------------|
最苛刻的推理，安全审查|克劳德十四行诗5 *(v1.0.67+)* |
|复杂推理，分析|克劳德十四行诗4 |
|代码生成，重构| GPT-4.1 |
|代码专用任务，大上下文| kimi-k2.7-code *(v1.0.68+)* |
|快速分析，简单任务|克劳德俳句或gpt -4.1迷你|
|大型代码库理解|模型与更大的上下文窗口|

在存储库中组织代理```
.github/
└── agents/
    ├── security-reviewer.agent.md
    ├── api-designer.agent.md
    ├── terraform-expert.agent.md
    └── release-manager.agent.md
```
让代理集中注意力——每个文件一个角色。如果您发现一个代理试图做太多事情，请将其拆分为多个代理，或者将常见任务提取为代理可以调用的技能。

##常见问题

**问：如何选择海关代理？**

答：在VS Code中，打开副驾驶聊天并使用聊天面板顶部的代理选择器下拉。自定义代理与内置选项一起显示。您还可以按名称`@mention`代理。在Copilot CLI中，可以通过会话中的代理选择器发现自定义代理。使用**代理协调协议（ACP）与Copilot CLI集成的客户端**也可以列出可用的自定义代理，并通过`agent`会话配置选项（v1.0.40+）以编程方式在它们之间切换。这允许像Zed、Neovim插件和CI管道这样的工具通过ACP驱动Copilot来显示代理选择器和切换代理，而不需要斜杠命令。ACP客户端也接收代理的“实时计划”，因为它通过多步骤任务（v1.0.40+）工作，所以他们可以向用户显示实时进度，而无需等待每个回合完成。

**Q：座席可以使用技能吗？**

是的。代理可以根据用户的意图在会话期间发现和调用技能。技能扩展代理可以做什么，而不膨胀代理自己的指令。**Q：一个存储库应该有多少代理？**

答：最常见的工作流程从2-3个代理开始。随着模式的出现，添加更多。典型的团队有3-8个代理，涵盖代码审查、基础设施、测试和文档等领域。

**Q：我可以使用代理与副驾驶编码代理？**

是的。当你将一个问题分配给副驾驶时，你可以指定哪个代理应该处理它。代理的角色和工具访问应用于自治编码会话。详见[使用副驾驶编码代理]（../using-copilot-coding-agent/）。

**Q：代理应该包括代码示例吗？**

答：是的，在定义输出格式或编码模式时。显示您期望代理生成的内容——审查格式、代码结构、提交消息样式等。

##常见陷阱-❌**太宽泛**：“你是一个软件工程师”-没有重点或护栏
✅**代替**：定义具体的专业知识，审查标准和输出格式

-❌**未指定工具**：代理无法搜索代码或执行命令
✅**代替**：在frontmatter中声明代理需要的工具

-❌**与指令冲突**：代理显示“使用制表符”，但指令显示“使用空格”
✅**代替**：代理应该补充说明，而不是反驳说明

-❌**单片代理**：一个代理处理安全，测试，文档和部署
✅**代替**：创建专注的代理并让它们调用共享技能

##下一步- **Explore Repository Examples**：浏览[Agents Directory]（../../agents/）查看生产代理定义
- **连接外部工具**:[了解MCP服务器](../understanding-mcp-servers/) -让代理访问数据库，api等
- **自动编码代理**:[使用副驾驶编码代理](../using-copilot-coding-agent/) -在问题上自动运行代理
- **添加可重用任务**:[创建有效技能](../creating-effective-skills/) -构建任务代理可以发现和调用

---