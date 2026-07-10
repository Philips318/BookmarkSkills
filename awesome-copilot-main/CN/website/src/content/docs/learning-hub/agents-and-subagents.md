---
title: 'Agents and Subagents'
description: 'Learn how delegated subagents differ from primary agents, when to use them, and how to launch them in VS Code and Copilot CLI.'
authors:
  - GitHub Copilot Learning Hub Team
lastUpdated: 2026-07-01
estimatedReadingTime: '9 minutes'
tags:
  - agents
  - subagents
  - orchestration
  - fundamentals
relatedArticles:
  - ./building-custom-agents.md
  - ./what-are-agents-skills-instructions.md
  - ./github-copilot-terminology-glossary.md
prerequisites:
  - Basic understanding of GitHub Copilot agents
---
我们熟悉代理（../what-are-agents-skills-instructions/），但是代理工作流还有另一个方面需要考虑，那就是子代理的角色。代理是您为会话或工作流选择的主要助手，而子代理是主代理为较窄的任务启动的临时工作者，通常是为了保持上下文整洁、并行化工作或应用一组更专门的指令。

当您从简单的聊天提示转移到编排的代理工作流时，这种区别更加重要。

##从心理模型开始

将主代理视为项目负责人，将子代理视为专注的贡献者：

|主题|代理|子代理||------|------|------|
|启动方式|由用户选择或为工作流配置|由另一个代理或编排器启动|
|生命周期|在主会话或会话中持续存在|临时的；仅对委托任务|存在
|上下文|承载更广泛的对话和目标|获得更窄的提示和它自己的孤立上下文|
|作用域|协调整个任务|执行一个重点工作|
|输出|直接与用户对话|向主代理报告，由主代理合成结果|

在实践中，主代理保持全局，而子代理吸收嘈杂的中间工作：研究、代码检查、专门的审查通过或独立的实现跟踪。

当工作转移到子代理时会发生什么变化子代理非常有用，因为它们不仅仅是“另一个选项卡中的同一个代理”。它们通常会在几个重要方面改变作品的形状：

- **上下文隔离**：子代理只获得与任务相关的提示，这减少了早期会话历史的干扰。
- **集中指令**：子代理可以使用更严格的角色，如计划者、实现者、审稿人或研究人员。
- **并行性**：多个子代理可以在任务不冲突的情况下同时工作。
- **控制合成**：父代理决定什么被带回主要对话。
- **可选模型选择**：子代理可以使用不同的AI模型来执行任务，所以当我们的主代理可能使用通才模型时，子代理可以配置为使用更专业的模型进行代码审查或研究。这种隔离是子代理在大型任务上优于单个整体代理的主要原因之一。

何时使用子代理

子代理在以下情况下工作得特别好：

-实施前的研究
-在不污染主线程的情况下比较多种方法
-运行并行评审透视图，例如正确性、安全性和体系结构
-将大型工作拆分为具有明确依赖关系的独立轨道
让一个协调代理专注于协调而不是直接执行
-比较不同模型的多种方法

如果所有工作都发生在一个小文件中，并且不需要分解，则可能不需要子代理。当委托减少上下文压力或让多个轨道独立运行时，好处就显现出来了。

在VS Code中启动子代理在VS Code中，子代理通常是由代理发起的。您通常描述较大的任务，而主代理决定何时委派一个重点关注的子任务。要实现这一点，代理需要访问子代理工具。

# # # 1。启用代理工具

在frontmatter中使用`agent`工具，以便主代理可以启动其他代理：```yaml
---
name: Feature Builder
tools: ['agent', 'read', 'search', 'edit']
agents: ['Planner', 'Implementer', 'Reviewer']
---
```
`agents`属性充当此协调器可以调用的工作代理的允许列表。

# # # 2。定义具有明确边界的工作代理

Worker代理通常对拾取器隐藏，并保留用于委托：```yaml
---
name: Planner
user-invocable: false
tools: ['read', 'search']
---
```
您还可以使用`disable-model-invocation: true`来防止将代理用作子代理，除非另一个协调器显式允许这样做。

# # # 3。提示孤立或并行工作

您并不总是需要说“运行子代理”，但是描述独立研究或并行跟踪的提示会使委托更容易。例如:```text
Analyze this feature in parallel:
1. Research existing code patterns
2. Propose an implementation plan
3. Review likely security risks
Then summarize the findings into one recommendation.
```
# # # 4。了解嵌套规则

默认情况下，子代理不会继续生成其他子代理。在VS Code中，递归委托由`chat.subagents.allowInvocationsFromSubagents`设置控制，默认情况下该设置是关闭的。

在Copilot CLI中启动子代理

在GitHub CopilotCLI中，最清晰的最终用户入口点是**`/fleet`**。Fleet充当一个协调器，分解一个更大的目标，启动多个后台子代理，尊重依赖关系，然后合成最终结果。```text
/fleet Update the auth docs, refactor the auth service, and add related tests.
```
对于非交互式执行：```bash
copilot -p "/fleet Update the auth docs, refactor the auth service, and add related tests." --no-ask-user
```
> **提示模式和repo钩子(v1.0.40+)**：当使用`copilot -p "..."`（提示模式）时，为了安全起见，默认禁用存储库钩子。如果你的`/fleet`工作流依赖于钩子（例如，编辑后的自动格式化或lint检查），在运行前通过设置`GITHUB_COPILOT_PROMPT_MODE_REPO_HOOKS=true`来选择。参见[使用Hooks自动化]（../automating-with-hooks/）了解详细信息。

重要的行为不同于单一的聊天回合：

-协调者首先计划工作项
-独立任务可以并行运行
每个子代理都有自己的上下文窗口
子代理共享相同的文件系统，因此应该避免重叠的写操作

这使得`/fleet`成为启动子代理的实用方法，即使您自己不编写自定义代理文件也是如此。

橡皮鸭剂在`/experimental`（v1.0.42+）中可用的橡皮鸭代理**应用了一种新颖的多模型模式：当您在gpt支持的会话中工作时，橡皮鸭代理在内部通过Claude路由某些请求，以提供第二个透视图。这个想法类似于橡皮鸭调试——与不同的“听众”讨论问题常常会暴露出你没有注意到的假设或盲点。

在v1.0.64+中，您可以直接从`/subagents`配置橡皮鸭代理（包括其补充模型策略）：```
/subagents          # open the subagents configuration panel
```
或者你仍然可以启用实验性功能，并从代理选择器中选择它：```
/experimental           # toggle experimental features
/agent                  # open the agent picker and select rubber-duck
```
互补模型策略允许你指定橡皮鸭代理应该自动从不同于你的主模型的家族中选择一个模型（例如，如果你在Claude上，它选择一个GPT模型，反之亦然）。这最大限度地提高了视角的多样性。

因为它作为子代理层运行，而不是取代主模型，所以当橡皮鸭分析在后台运行时，您可以保留当前会话模型和上下文。

> **注**：这是一个实验性功能，可能会更改。如果您觉得有用，可以通过`/feedback`提供反馈。

工作良好的编排模式

###协调者和工作者

一个代理拥有工作流，并将其委托给较小范围的专家，如计划者、实现者和审阅者。这使协调器保持轻量级，并使工作者提示更精确。

###多角度回顾为不同的视角（正确性、安全性、代码质量、架构）运行并行子代理，并在它们完成后合并结果。

研究，然后行动

使用一个子代理来收集事实，使用另一个子代理来实现这些事实。当您希望主线程不受探索性干扰时，此模式特别有用。

内置的**`/research`**命令自动使用这个orchestrator/subagent模型（v1.0.40+）：它生成一个编排器，将主题分解为研究线程，作为子代理并行运行它们，并将结果综合到结构化报告中。这意味着您可以获得比单轮查询更深入、更可靠的结果，而不必自己设置多代理模式。

您可以检查的存储库示例

这个存储库已经包含了一些与委托相关的有用语法示例：—[`agents/context7.agent.md`]（https://github.com/github/awesome-copilot/blob/main/agents/context7.agent.md）是VS Code风格的`handoffs`的具体例子。它定义了一个切换按钮，可以在研究完成后将工作传递给另一个代理。
—[`agents/rug-orchestrator.agent.md`]（https://github.com/github/awesome-copilot/blob/main/agents/rug-orchestrator.agent.md）是强协调器的例子。它启用`agent`工具并限制`agents: ['SWE', 'QA']`的委托。
- [`agents/gem-orchestrator.agent.md`]（https://github.com/github/awesome-copilot/blob/main/agents/gem-orchestrator.agent.md）显示了`user-invocable`和`disable-model-invocation`的调用控制，这在决定协调器是应该直接选择、可委派还是两者兼而有之时非常有用。
- [`agents/custom-agent-foundry.agent.md`]（https://github.com/github/awesome-copilot/blob/main/agents/custom-agent-foundry.agent.md）文档VS Code`handoffs`形状在其指导部分，这是有帮助的，如果你想要一个模板之前创建自己的协调器工作流。

重要的平台差异：切换并不普遍VS Code文档描述了子代理和`handoffs`frontmatter属性。[GitHub的自定义代理配置参考](https://docs.github.com/en/copilot/customizing-copilot/github-copilot-agents/configuration-reference-for-github-copilot-agents)，然而，注意到`handoffs`和`argument-hint`目前被忽略的副驾驶云代理在GitHub.com。

这意味着你应该考虑特定于产品的委派特性：

—**VS Code**：支持子代理概念，允许列表和面向切换的代理组合
- **Copilot CLI**：通过`/fleet`等命令暴露实际业务流程
- **GitHub.com编码代理/云代理**：支持自定义代理，但一些VS Code特定的前端内容被故意忽略

如果您跨界面共享代理文件，请记录这些差异，以便用户知道哪些行为是可移植的，哪些是特定于编辑器的。

##常见问题

**用户总是直接调用子代理吗？**否。大多数情况下，主代理在确定任务受益于上下文隔离或并行性时启动它们。

子代理可以使用不同的模型或工具集吗？**

是的，当被委托的工作人员是有自己前台事务的海关代理时。

**子代理总是并行的吗？**

否。当一个步骤依赖于另一个步骤时，它们可以顺序运行，或者当工作项独立时，它们可以并行运行。

**我可以控制同时运行多少个子代理吗？**

是。在v1.0.66+中，基于使用情况的计费用户可以直接从`/settings`配置子代理并发性和深度限制。并发限制控制并行运行的子代理的数量；深度限制控制深度委托可以链接多少层（防止失控的递归子代理树）。这些设置使您可以在复杂的编排任务期间对资源消耗进行可预测的控制。

##下一步-阅读[构建自定义代理]（../building-custom-agents/）来设计协调器和工作代理。
-重新访问[什么是代理，技能和指令](../what-are-agents-skills-instructions/)，以获得更广泛的自定义模型。
—在比较不同产品的术语时，请将[GitHub Copilot术语表]（../github-copilot-terminology-glossary/）放在附近。

---