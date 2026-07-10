#链条跨度

# #目的

CHAIN跨表示应用程序中的编排层（LangChain链、自定义工作流、应用程序入口点）。常用作根跨。

##必需属性

|属性|类型|描述|必选|| ------------------------- | ------ | --------------- | -------- |
|`openinference.span.kind`| String |必须为“CHAIN” |是|

##常用属性

CHAIN跨度通常使用[Universal Attributes](fundamentals-universal-attributes.md)：

-`input.value`-链的输入（用户查询，请求负载）
-`output.value`-链输出（最终响应）
-`input.mime_type`/`output.mime_type`-格式指示灯

示例：根链```json
{
  "openinference.span.kind": "CHAIN",
  "input.value": "{\"question\": \"What is the capital of France?\"}",
  "input.mime_type": "application/json",
  "output.value": "{\"answer\": \"The capital of France is Paris.\", \"sources\": [\"doc_123\"]}",
  "output.mime_type": "application/json",
  "session.id": "session_abc123",
  "user.id": "user_xyz789"
}
```
示例：嵌套子链```json
{
  "openinference.span.kind": "CHAIN",
  "input.value": "Summarize this document: ...",
  "output.value": "This document discusses..."
}
```
