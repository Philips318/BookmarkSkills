# LLM跨度

表示对语言模型（OpenAI, Anthropic， local模型等）的调用。

##必需属性

|属性|类型|描述||-----------|------|-------------|
|`openinference.span.kind`| String |必须为“LLM”|
|`llm.model_name`| String |模型标识符（例如，“gpt-4”， “claude-3-5-sonnet-20241022”） |

##关键属性

|类别|属性|示例||----------|------------|---------|
| **Model** |`llm.model_name`,`llm.provider`| "gpt-4-turbo", "openai" |
| ** token ** |`llm.token_count.prompt`,`llm.token_count.completion`,`llm.token_count.total`| 25,8,33 |
| **成本** |`llm.cost.prompt`，`llm.cost.completion`,`llm.cost.total`| 0.0021, 0.0045, 0.0066 |
| **参数** |`llm.invocation_parameters`(JSON) |`{"temperature": 0.7, "max_tokens": 1024}`|
| **消息** |`llm.input_messages.{i}.*`，`llm.output_messages.{i}.*`|参见|下面的示例
| **Tools** |`llm.tools.{i}.tool.json_schema`|功能定义|

成本跟踪

核心属性:* * * *
-`llm.cost.prompt`-总投入成本（美元）
-`llm.cost.completion`-总产出成本（美元）
-`llm.cost.total`-总成本（美元）

**详细成本细目：**
-`llm.cost.prompt_details.{input,cache_read,cache_write,audio}`-投入成本成分
-`llm.cost.completion_details.{output,reasoning,audio}`-输出成本组件

# #消息

输入消息:* * * *
-`llm.input_messages.{i}.message.role`-“用户”、“助手”、“系统”、“工具”
-`llm.input_messages.{i}.message.content`-文本内容
-`llm.input_messages.{i}.message.contents.{j}`-多模式（文本+图像）
-`llm.input_messages.{i}.message.tool_calls`-工具调用

**输出消息：**与输入消息结构相同。

示例：Basic LLM Call```json
{
  "openinference.span.kind": "LLM",
  "llm.model_name": "claude-3-5-sonnet-20241022",
  "llm.invocation_parameters": "{\"temperature\": 0.7, \"max_tokens\": 1024}",
  "llm.input_messages.0.message.role": "system",
  "llm.input_messages.0.message.content": "You are a helpful assistant.",
  "llm.input_messages.1.message.role": "user",
  "llm.input_messages.1.message.content": "What is the capital of France?",
  "llm.output_messages.0.message.role": "assistant",
  "llm.output_messages.0.message.content": "The capital of France is Paris.",
  "llm.token_count.prompt": 25,
  "llm.token_count.completion": 8,
  "llm.token_count.total": 33
}
```
示例：带有工具调用的LLM```json
{
  "openinference.span.kind": "LLM",
  "llm.model_name": "gpt-4-turbo",
  "llm.input_messages.0.message.content": "What's the weather in SF?",
  "llm.output_messages.0.message.tool_calls.0.tool_call.function.name": "get_weather",
  "llm.output_messages.0.message.tool_calls.0.tool_call.function.arguments": "{\"location\": \"San Francisco\"}",
  "llm.tools.0.tool.json_schema": "{\"type\": \"function\", \"function\": {\"name\": \"get_weather\"}}"
}
```
##参见Also

- **仪器仪表：**`instrumentation-auto-python.md`，`instrumentation-manual-python.md`- **全规格：**https://github.com/Arize-ai/openinference/blob/main/spec/semantic_conventions.md