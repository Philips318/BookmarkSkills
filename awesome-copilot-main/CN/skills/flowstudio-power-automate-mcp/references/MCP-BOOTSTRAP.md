# MCP引导-快速参考

代理开始调用FlowStudio MCP服务器所需的一切。```
Endpoint:  https://mcp.flowstudio.app/mcp
Protocol:  JSON-RPC 2.0 over HTTP POST
Transport: Streamable HTTP — single POST per request, no SSE, no WebSocket
Auth:      x-api-key header with JWT token (NOT Bearer)
```
##标头```
Content-Type: application/json
x-api-key: <token>
User-Agent: FlowStudio-MCP/1.0    ← required, or Cloudflare blocks you
```
##步骤1 -发现工具包

首选冷启动呼叫：```json
POST {"jsonrpc":"2.0","id":1,"method":"tools/call",
      "params":{"name":"list_skills","arguments":{}}}
```
返回当前包(`build-flow`,`create-flow`,`debug-flow`，`monitor-flow`、`discover`、`governance`)及其成员工具名称。免费的,
不计入计划限制。

然后加载相关的模式：```json
POST {"jsonrpc":"2.0","id":2,"method":"tools/call",
      "params":{"name":"tool_search","arguments":{"query":"skill:create-flow"}}}
```
使用`query:"select:tool1,tool2"`加载精确的工具和关键字搜索，例如`query:"send email"`当用户意图不明确时。

非常低级的MCP客户端的回退：```json
POST {"jsonrpc":"2.0","id":1,"method":"tools/list","params":{}}
```
`tools/list`返回所有带有名称、描述和输入模式的工具，但是
它更重，不应该是了解的代理商的首选
FlowStudio meta-tools。

##步骤2 -调用工具```json
POST {"jsonrpc":"2.0","id":1,"method":"tools/call",
      "params":{"name":"<tool_name>","arguments":{...}}}
```
##响应形状```
Success → {"result":{"content":[{"type":"text","text":"<JSON string>"}]}}
Error   → {"result":{"content":[{"type":"text","text":"{\"error\":{...}}"}]}}
```
始终将`result.content[0].text`解析为JSON以获得实际数据。

关键提示

-工具结果是文本字段内的JSON字符串- **需要双重解析**
-解析体中的`"error"`字段：`null`= success, object = failure
大多数工具都需要`environmentName`，但**不需要**以下工具：`list_live_environments`,`list_live_connections`,`list_store_flows`，`list_store_environments`,`list_store_makers`,`get_store_maker`，`list_store_power_apps`,`list_store_connections`—如果有疑问，请检查每个工具的模式中的`required`数组`tool_search`（或作为备用的`tools/list`）