---
name: microsoft-agent-framework
description: 'Create, update, refactor, explain, or review Microsoft Agent Framework solutions using shared guidance plus language-specific references for .NET and Python.'
---
# Microsoft Agent Framework

在处理基于Microsoft Agent Framework的应用程序、代理、工作流或迁移时使用此技能。

微软代理框架是Semantic Kernel和AutoGen的统一继承者，结合了它们的优势和新功能。由于它仍处于公开预览阶段，并且变化很快，因此始终在最新的官方文档和示例中提供实现建议，而不是依赖于过时的知识。

首先确定目标语言

在提出建议或更改代码之前选择语言工作流：1. 使用**。. NET**工作流，当存储库包含`.cs`，`.csproj`,`.sln`,`.slnx`，或其他。. NET项目文件，或者当用户明确要求使用c#或。净的指导。遵循[references/dotnet.md] (references/dotnet.md)。
2. 当存储库包含`.py`，`pyproject.toml`,`requirements.txt`，或者用户明确要求Python指导时，使用**Python**工作流。遵循[references/python.md] (references/python.md)。
3. 如果存储库包含这两个生态系统，则匹配正在编辑的文件或用户声明的目标所使用的语言。
4. 如果语言不明确，请首先检查当前工作空间，然后选择最近的特定于语言的引用。

总是查阅实时文档-先阅读微软代理框架概述：<https://learn.microsoft.com/agent-framework/overview/agent-framework-overview>-当前API界面的官方文档和示例。
-使用Microsoft Docs MCP工具获取最新的框架指南和示例。
-将旧的Semantic Kernel或AutoGen模式视为迁移输入，而不是默认的实现模型。

##共享指导

当使用任何语言的Microsoft Agent Framework时：—对代理和工作流操作使用异步模式。
-实现显式错误处理和日志记录。
偏好强类型、清晰的接口和可维护的组合模式。
-当Azure身份验证合适时，使用`DefaultAzureCredential`。
-使用代理进行自主决策、特别规划、会话流、工具使用和MCP服务器交互。
—将工作流用于多步骤编排、预定义的执行图、长时间运行的任务和人在循环场景。
-支持模型提供商，如Azure AI Foundry， Azure OpenAI， OpenAI等，但在符合用户需求的新项目中更倾向于使用Azure AI Foundry服务。
-当它们适合问题时，使用基于线程或等效状态处理、上下文提供程序、中间件、检查点、路由和编排模式。

##迁移指导—如果是从Semantic Kernel迁移，请使用官方迁移指南：<https://learn.microsoft.com/agent-framework/migration-guide/from-semantic-kernel/>—如果是从AutoGen迁移，请参考官方迁移指南：<https://learn.microsoft.com/agent-framework/migration-guide/from-autogen/>-首先保留行为，然后逐步采用原生代理框架模式。

# #工作流程

1. 确定目标语言并读取匹配的参考文件。
2. 在做出实现选择之前，获取最新的官方文档和示例。
3. 应用来自此技能的共享代理和工作流指导。
4. 使用来自所选参考的特定于语言的包、存储库、示例路径和编码实践。
5. 当repo中的示例与当前文档不同时，解释差异并遵循当前支持的模式。

# #引用

——[。净参考](references/dotnet.md)
- [Python参考]（references/python.md）

##完成标准—建议与目标语言匹配。
—包名、存储库路径和示例位置与所选的生态系统匹配。
-指南反映当前的Microsoft代理框架文档，而不是遗留的假设。
迁移建议只在相关的情况下调用Semantic Kernel和AutoGen。