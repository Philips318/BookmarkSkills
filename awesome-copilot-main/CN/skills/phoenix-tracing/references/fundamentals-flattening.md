#扁平化约定

OpenInference将嵌套数据结构扁平化为点符号属性，以实现数据库兼容性、opentelementetry兼容性和简单查询。

##扁平化规则

**对象→点表示法**```javascript
{ llm: { model_name: "gpt-4", token_count: { prompt: 10, completion: 20 } } }
// becomes
{ "llm.model_name": "gpt-4", "llm.token_count.prompt": 10, "llm.token_count.completion": 20 }
```
**数组→零索引符号**```javascript
{ llm: { input_messages: [{ role: "user", content: "Hi" }] } }
// becomes
{ "llm.input_messages.0.message.role": "user", "llm.input_messages.0.message.content": "Hi" }
```
消息约定：需要`.message.`段**```
llm.input_messages.{index}.message.{field}
llm.input_messages.0.message.tool_calls.0.tool_call.function.name
```
##完整示例```javascript
// Original
{
  openinference: { span: { kind: "LLM" } },
  llm: {
    model_name: "claude-3-5-sonnet-20241022",
    invocation_parameters: { temperature: 0.7, max_tokens: 1000 },
    input_messages: [{ role: "user", content: "Tell me a joke" }],
    output_messages: [{ role: "assistant", content: "Why did the chicken cross the road?" }],
    token_count: { prompt: 5, completion: 10, total: 15 }
  }
}

// Flattened (stored in Phoenix spans.attributes JSONB)
{
  "openinference.span.kind": "LLM",
  "llm.model_name": "claude-3-5-sonnet-20241022",
  "llm.invocation_parameters": "{\"temperature\": 0.7, \"max_tokens\": 1000}",
  "llm.input_messages.0.message.role": "user",
  "llm.input_messages.0.message.content": "Tell me a joke",
  "llm.output_messages.0.message.role": "assistant",
  "llm.output_messages.0.message.content": "Why did the chicken cross the road?",
  "llm.token_count.prompt": 5,
  "llm.token_count.completion": 10,
  "llm.token_count.total": 15
}
```
