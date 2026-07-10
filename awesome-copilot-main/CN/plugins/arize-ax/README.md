# AX插件

为LLM的可观察性、评估和优化掌握AX平台技能。包括跟踪导出、仪器仪表、数据集、实验、评估器、AI提供程序集成、注释、提示优化和到Arize UI的深度链接。

# #安装```bash
# Using Copilot CLI
copilot plugin install arize-ax@awesome-copilot
```
包含的内容

# # #技能

|技能|描述||-------|-------------|
|`arize-trace`|导出和分析使用ax CLI调试LLM应用程序的跟踪和跨度。|
|`arize-instrumentation`|使用两阶段代理辅助工作流向应用程序添加ajax跟踪。|
|`arize-dataset`|使用ax CLI创建、管理和查询版本化的评估数据集。|
|`arize-experiment`|针对数据集运行实验，并使用ax CLI比较结果。|
创建并运行llm作为评判评估器，为跨度和实验自动评分。|
|`arize-ai-provider-integration`|存储和管理LLM提供程序凭据，以便与评估器一起使用。|
|`arize-annotation`|创建注释配置并批量应用人工反馈标签到跨度。|
使用生产跟踪数据、评估和注释优化LLM提示。|
生成到alize UI的深度链接，用于跟踪、跨度、会话、数据集等。|