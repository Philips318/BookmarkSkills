---
name: semantic-kernel
description: 'Create, update, refactor, explain, or review Semantic Kernel solutions using shared guidance plus language-specific references for .NET and Python.'
---
# Semantic Kernel

在处理基于Semantic Kernel的应用程序、插件、函数调用流或AI集成时使用此技能。

始终在最新的语义内核文档和示例中提供基础实现建议，而不是单独在内存中。

首先确定目标语言

在提出建议或更改代码之前选择语言工作流：1. 使用**。. NET**工作流，当存储库包含`.cs`，`.csproj`,`.sln`，或其他。. NET项目文件，或者当用户明确要求使用c#或。净的指导。遵循[references/dotnet.md] (references/dotnet.md)。
2. 当存储库包含`.py`，`pyproject.toml`,`requirements.txt`，或者用户明确要求Python指导时，使用**Python**工作流。遵循[references/python.md] (references/python.md)。
3. 如果存储库包含这两个生态系统，则匹配正在编辑的文件或用户声明的目标所使用的语言。
4. 如果语言不明确，请首先检查当前工作空间，然后选择最近的特定于语言的引用。

总是查阅实时文档

-先阅读语义内核概述：<https://learn.microsoft.com/semantic-kernel/overview/>-当前API界面的官方文档和示例。
-使用Microsoft Docs MCP工具获取最新的框架指南和示例。

##共享指导在任何语言中使用语义内核时：

-内核操作使用异步模式。
-遵循官方插件和函数调用模式。
-实现显式错误处理和日志记录。
偏好强类型、清晰抽象和可维护的组合模式。
-为Azure AI Foundry， Azure OpenAI， OpenAI和其他AI服务使用内置连接器，同时在适合任务的新项目中更倾向于使用Azure AI Foundry服务。
-使用内核的内存和上下文管理功能，当他们简化解决方案。
—当Azure身份验证合适时，使用`DefaultAzureCredential`。

# #工作流程1. 确定目标语言并读取匹配的参考文件。
2. 在做出实现选择之前，获取最新的官方文档和示例。
3. 应用此技能的共享语义内核指导。
4. 使用来自所选参考的特定于语言的包、存储库、示例路径和编码实践。
5. 当repo中的示例与当前文档不同时，解释差异并遵循当前支持的模式。

# #引用

——[。净参考](references/dotnet.md)
- [Python参考]（references/python.md）

##完成标准

—建议与目标语言匹配。
—包名、存储库路径和示例位置与所选的生态系统匹配。
-指南反映当前的语义内核文档，而不是陈旧的假设。