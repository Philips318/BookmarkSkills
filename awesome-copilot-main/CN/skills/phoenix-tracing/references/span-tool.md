#工具跨度

# #目的

TOOL跨表示外部工具或函数调用（API调用、数据库查询、计算器、自定义函数）。

##必需属性

|属性|类型|描述|必选|| ------------------------- | ------ | ------------------ | ----------- |
|`openinference.span.kind`| String |必须为“TOOL” |是|
|`tool.name`|字符串|Tool/function名称|推荐|

##属性引用

工具执行属性

|属性|类型|描述|| ------------------ | ------------- | ------------------------------------------ |
|`tool.name`|字符串|Tool/function名称|
|`tool.description`| String | Toolpurpose/description|
|`tool.parameters`|字符串（JSON） |定义工具参数的JSON模式|
|`input.value`| String (JSON) |传递给工具|的实际输入值
|`output.value`| String | Tooloutput/result|
|`output.mime_type`| String |结果内容类型（例如“application/json”）|

# #的例子

API调用工具```json
{
  "openinference.span.kind": "TOOL",
  "tool.name": "get_weather",
  "tool.description": "Fetches current weather for a location",
  "tool.parameters": "{\"type\": \"object\", \"properties\": {\"location\": {\"type\": \"string\"}, \"units\": {\"type\": \"string\", \"enum\": [\"celsius\", \"fahrenheit\"]}}, \"required\": [\"location\"]}",
  "input.value": "{\"location\": \"San Francisco\", \"units\": \"celsius\"}",
  "output.value": "{\"temperature\": 18, \"conditions\": \"partly cloudy\"}"
}
```
计算器工具```json
{
  "openinference.span.kind": "TOOL",
  "tool.name": "calculator",
  "tool.description": "Performs mathematical calculations",
  "tool.parameters": "{\"type\": \"object\", \"properties\": {\"expression\": {\"type\": \"string\", \"description\": \"Math expression to evaluate\"}}, \"required\": [\"expression\"]}",
  "input.value": "{\"expression\": \"2 + 2\"}",
  "output.value": "4"
}
```
数据库查询工具```json
{
  "openinference.span.kind": "TOOL",
  "tool.name": "sql_query",
  "tool.description": "Executes SQL query on user database",
  "tool.parameters": "{\"type\": \"object\", \"properties\": {\"query\": {\"type\": \"string\", \"description\": \"SQL query to execute\"}}, \"required\": [\"query\"]}",
  "input.value": "{\"query\": \"SELECT * FROM users WHERE id = 123\"}",
  "output.value": "[{\"id\": 123, \"name\": \"Alice\", \"email\": \"alice@example.com\"}]",
  "output.mime_type": "application/json"
}
```
