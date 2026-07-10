#通用属性

本文档涵盖了OpenInference中可用于任何跨度类型的属性。

# #概述

这些属性可以用于任何跨度类型，以提供额外的上下文、跟踪和元数据。## Input/Output
|属性|类型|描述|| ------------------ | ------ | ---------------------------------------------------- |
|`input.value`| String |输入操作（提示、查询、文档）|
|`input.mime_type`|字符串| MIME类型（如“text/plain”，“application/json”）|
|`output.value`| String |操作（响应、向量、结果）的输出|
|`output.mime_type`| String |输出|的MIME类型

为什么要捕获I/O？

**总是捕获input/output求值准备跨度：**
-凤凰评估器（可靠性，相关性，问答正确性）要求`input.value`和`output.value`Phoenix UI在跟踪视图中突出显示I/O，以便调试
-启用导出I/O，用于创建微调数据集
—为分析座席行为提供完整的上下文

* *示例属性:* *```json
{
  "openinference.span.kind": "CHAIN",
  "input.value": "What is the weather?",
  "input.mime_type": "text/plain",
  "output.value": "I don't have access to weather data.",
  "output.mime_type": "text/plain"
}
```
**参见特定语言的实现：**
—TypeScript:`instrumentation-manual-typescript.md`—Python:`instrumentation-manual-python.md`会话和用户跟踪

|属性|类型|描述|| ------------ | ------ | ---------------------------------------------- |
|`session.id`| String |分组相关跟踪的会话标识符|
|`user.id`| String |每用户分析的用户标识符|

* *的例子:* *```json
{
  "openinference.span.kind": "LLM",
  "session.id": "session_abc123",
  "user.id": "user_xyz789"
}
```
# #元数据

|属性|类型|描述|| ---------- | ------ | ------------------------------------------ |
|`metadata`| string |键值对的json序列化对象|

* *的例子:* *```json
{
  "openinference.span.kind": "LLM",
  "metadata": "{\"environment\": \"production\", \"model_version\": \"v2.1\", \"cost_center\": \"engineering\"}"
}
```
