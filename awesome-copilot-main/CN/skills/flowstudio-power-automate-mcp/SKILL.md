---
name: flowstudio-power-automate-mcp
description: >-
  Foundation skill for Power Automate via FlowStudio MCP — auth setup, the
  reusable MCP helper (Python + Node.js), tool discovery via `list_skills` /
  `tool_search`, and oversized-response handling. Load this skill first when
  connecting an agent to Power Automate. For specialized workflows, load
  `flowstudio-power-automate-build`, `flowstudio-power-automate-debug`, `flowstudio-power-automate-monitoring`
  (Pro+), or `flowstudio-power-automate-governance` (Pro+) — each contains the workflow
  narrative, this skill provides the plumbing they all rely on. Requires a
  FlowStudio MCP subscription or compatible server — see https://mcp.flowstudio.app
---
#权力自动化通过FlowStudio MCP -基础

这个技能是“管道层”。它给了人工智能代理一个可靠的方法
与FlowStudio MCP服务器交谈，发现可用的工具，并处理
答案很清晰。实际的工作流叙述分为四个方面
这些技能都建立在这个基础上。

> * * * *实际调试例子:[孩子流表达式错误](https://github.com/ninihen1/power-automate-mcp-skills/blob/main/examples/fix-expression-error.md) |
>[数据输入，不是流错误](https://github.com/ninihen1/power-automate-mcp-skills/blob/main/examples/data-not-flow.md) |
>[空值导致子流崩溃]（https://github.com/ninihen1/power-automate-mcp-skills/blob/main/examples/null-child-flow.md）

b> **要求：** A [FlowStudio](https://mcp.flowstudio.app) MCP订阅(或
>兼容Power automation MCP服务器)。您需要：
> - MCP端点：`https://mcp.flowstudio.app/mcp`（所有订阅者相同）
> - API密钥/ JWT令牌（`x-api-key`报头-非承载）
> -电源平台环境名称（例如`Default-<tenant-guid>`）

---

什么时候使用什么技能技能是按用例意图组织的，而不是按它们调用的工具组织的。
多种技能重用相同的底层工具——根据用户是什么来选择
努力完成。

|加载此技能||---|---|
| **`flowstudio-power-automate-build`** |创建或更改流（构建新流、修改现有流、修复错误、部署流）
|诊断流失败的原因（运行失败的根本原因分析）| **`flowstudio-power-automate-debug`** |
|参见租户流量运行状况、故障率、资产库存| **`flowstudio-power-automate-monitoring`** *(Pro+)* |
|标记、审计、分类、评分、脱板流| **`flowstudio-power-automate-governance`** *(Pro+)* |
|只是连接，设置授权，编写助手，解析响应|这个技能（基础）|

同样的工具，不同的镜头。**`flowstudio-power-automate-build`和`flowstudio-power-automate-debug`两者都调用`update_live_flow`、`get_live_flow`和运行错误工具——它们
不同的是“方向”（向前与向后）和“意图”（撰写与诊断）。`flowstudio-power-automate-monitoring`和`flowstudio-power-automate-governance`都调用Store
工具——它们在“受众”（操作vs遵从）和“结果”（阅读）方面有所不同
运行状况vs写入元数据)。不要试图记住“哪个工具属于哪个工具”
技能”;根据用户正在做的事情来选择技能。

---

##真理之源|优先级|源|覆盖||----------|--------|--------|
| 1 | **真实的API响应** |始终相信服务器实际返回的|
| **`tool_search`/`list_skills`** |权威工具模式、参数名称、类型、所需标志|
| **技能文档和参考文件** |工作流程叙述，响应形状，非明显行为|

如果文档与真正的API响应不一致，则API获胜。工具模式
在此技能（或任何其他）可能会延迟服务器-调用`tool_search`来确认
在调用最近未使用的工具之前，先获取当前形状。

---

代理如何发现工具

FlowStudio MCP服务器（v1.1.5+）暴露了两个**不可计费的**元工具
让代理只加载与当前任务相关的工具。使用这些
首选`tools/list`（一次加载所有30多个模式）或猜测
工具名称。

|元工具|何时调用||---|---|
冷启动-查看可用的包（`build-flow`,`create-flow`,`debug-flow`,`monitor-flow`,`discover`,`governance`）并选择一个|
|`tool_search`with`query: "skill:<name>"`|为一个bundle加载完整的模式集（例如`skill:debug-flow`） |
|`tool_search`with`query: "select:tool1,tool2"`|按名称加载特定的工具（例如，当跨包链接时）|
|`tool_search`with`query: "<keywords>"`|当用户请求不明确时（例如`"cancel run"`），自由文本搜索|

服务器的`tool_search`包故意比这更窄
技能家族** -它们是最可能需要的工具的入门包
意图。工作流技能（例如`flowstudio-power-automate-debug`）可以拉一个包和
然后，随着工作流程的进展，再次调用`tool_search`获取其他工具。```python
# Cold start — pick a bundle by intent
skills = mcp("list_skills", {})
# [{"name": "debug-flow", "description": "Investigate why a flow is failing...",
#   "tools": ["get_live_flow_runs", "get_live_flow_run_error", ...]}, ...]

# Load schemas for the bundle
debug_tools = mcp("tool_search", {"query": "skill:debug-flow"})
```
当前常用包：

| Bundle | |时使用|---|---|
|`create-flow`|创建全新流；包括environment/connection发现、连接器描述、动态选项和`update_live_flow`|
|`build-flow`|读取或修改现有流定义|
|`debug-flow`|调查失败的运行和操作级别inputs/outputs|
|`monitor-flow`|Starting/stopping，触发、取消或重新提交运行|
|`discover`|枚举环境、流和连接|
|`governance`| Pro+缓存存储标记，制造商审计和元数据更新|

---

推荐语言：Python或Node.js本技能族中的所有示例都使用`urllib.request`**的**Python
（stdlib -不需要`pip install`）。**Node.js**是一个同样有效的选择：`fetch`是Node 18+内置的，JSON处理是本地的，而async/await清晰地映射到MCP工具调用的请求-响应模式上
对于已经在JavaScript/TypeScript堆栈中工作的团队来说，这是一个自然的选择。

|语言|判决|笔记||---|---|---|
| **Python** |推荐|干净的JSON处理，没有转义问题，所有技能示例使用它|
| **Node.js(≥18)** |推荐|原生`fetch`+`JSON.stringify`/`JSON.parse`；没有额外的包裹|
| PowerShell |避免流操作|`ConvertTo-Json -Depth`静默截断嵌套定义；引用和转义会破坏复杂的有效载荷。对于快速连接烟雾测试是可以接受的，但对于构建或更新流则不适用。|
| cURL / Bash |可能但脆弱| shell转义嵌套JSON容易出错；没有本地JSON解析器|

> * * TL;DR -使用下面的Core MCP Helper （Python或Node.js）。**双手柄
> JSON-RPC框架、验证和响应解析在单个可重用函数中。

---

Core MCP Helper （Python）

在所有后续操作中使用此帮助器：```python
import json, urllib.request

TOKEN = "<YOUR_JWT_TOKEN>"
MCP   = "https://mcp.flowstudio.app/mcp"

def mcp(tool, args, cid=1):
    payload = {"jsonrpc": "2.0", "method": "tools/call", "id": cid,
               "params": {"name": tool, "arguments": args}}
    req = urllib.request.Request(MCP, data=json.dumps(payload).encode(),
        headers={"x-api-key": TOKEN, "Content-Type": "application/json",
                 "User-Agent": "FlowStudio-MCP/1.0"})
    try:
        resp = urllib.request.urlopen(req, timeout=120)
    except urllib.error.HTTPError as e:
        body = e.read().decode("utf-8", errors="replace")
        raise RuntimeError(f"MCP HTTP {e.code}: {body[:200]}") from e
    raw = json.loads(resp.read())
    if "error" in raw:
        raise RuntimeError(f"MCP error: {json.dumps(raw['error'])}")
    text = raw["result"]["content"][0]["text"]
    return json.loads(text)
```
> **常见验证错误：**
> - HTTP401/403→令牌缺失、过期或格式错误。从[mcp.flowstudio.app]（https://mcp.flowstudio.app）获得一个新的JWT。
> - HTTP 400→畸形的JSON-RPC有效负载。检查`Content-Type: application/json`和车身结构。
> -`MCP error: {"code": -32602, ...}`→工具参数错误或缺失。用`select:<toolname>`调用`tool_search`以确认模式。

---

Core MCP Helper （Node.js）Node.js18+的等效帮助器（内置`fetch`-不需要包）：```js
const TOKEN = "<YOUR_JWT_TOKEN>";
const MCP   = "https://mcp.flowstudio.app/mcp";

async function mcp(tool, args, cid = 1) {
  const payload = {
    jsonrpc: "2.0",
    method: "tools/call",
    id: cid,
    params: { name: tool, arguments: args },
  };
  const res = await fetch(MCP, {
    method: "POST",
    headers: {
      "x-api-key": TOKEN,
      "Content-Type": "application/json",
      "User-Agent": "FlowStudio-MCP/1.0",
    },
    body: JSON.stringify(payload),
  });
  if (!res.ok) {
    const body = await res.text();
    throw new Error(`MCP HTTP ${res.status}: ${body.slice(0, 200)}`);
  }
  const raw = await res.json();
  if (raw.error) throw new Error(`MCP error: ${JSON.stringify(raw.error)}`);
  return JSON.parse(raw.result.content[0].text);
}
```
>需要Node.js18+。对于旧的Node，将`fetch`替换为`https.request`或者安装`node-fetch`。

---

##验证连接

一个3行烟雾测试，确认令牌，端点和helper都工作：```python
skills = mcp("list_skills", {})
print(f"Connected — {len(skills)} skill bundles available:",
      [s["name"] for s in skills])
```
预期的输出:```text
Connected — 6 skill bundles available: ['build-flow', 'create-flow', 'debug-flow', 'monitor-flow', 'discover', 'governance']
```
如果失败，请参阅上面的常见验证错误说明。如果成功，手
转向匹配用户意图的工作流技能。

---

##处理超大响应

一些MCP工具响应大到足以溢出代理的上下文窗口：

|工具|典型尺寸|原因||---|---|---|
|`describe_live_connector`| 100- 600kb |连接器|的全Swagger规格
|`get_live_dynamic_properties`| 50- 500kb |动态连接器字段模式，如SharePoint列表列|
|`get_live_flow_run_action_outputs`（无`actionName`） | 50 KB -数MB |顶层动作输出；对于foreach中的操作，每次重复都可以返回|
|`get_live_flow`（大流量）| 50-500 KB |深嵌套分支|
|`list_live_flows`（大租户）| 50- 200kb |数百条流记录|

###当线束溢出到文件

Agent安全带（Claude Code，VS CodeCopilot等）保存超大响应
到临时文件（例如`tool-results/mcp-flowstudio-describe_live_connector-NNNN.txt`）
并返回路径而不是内联JSON。该文件是**双包装** -
外部MCP信封加上内部json转义的有效载荷：```text
[{"type":"text","text":"<JSON-escaped payload>"}]
```
两次解析来获得可用的对象：```python
import json
with open(path) as f:
    raw = json.loads(f.read())
payload = json.loads(raw[0]["text"])
```

```powershell
$payload = ((Get-Content $path -Raw | ConvertFrom-Json)[0].text) | ConvertFrom-Json
```
经验法则

1. **提取，不要回声。**拉出您需要的特定字段（一个`operationId`，一个动作的输出），并在推理之前丢弃其余字段。
2. **总是将`actionName`传递给`get_live_flow_run_action_outputs`。**省略它获取所有顶层动作。对于foreach中的操作，传递`actionName`而不传递`iterationIndex`可以返回该操作的每次重复。
3. **在会话中重用溢出文件。**重新获取相同的连接器swagger花费30秒以上，并产生另一个溢出缓存路径。
4. **不要直接使用泄漏文件获取JSON键。**字符串在文件（`\"OperationId\":`）内进行json转义，因此`"OperationId":`的普通grep将不匹配。先解析，再过滤。
5. **向用户总结工具输出。**返回`name + state + trigger`流列表和`actionName + status + code`运行错误-不是原始JSON，除非被要求。```python
# Good — drill into one operation in a connector swagger
conn = mcp("describe_live_connector", {"environmentName": ENV, "connectorName": "shared_sharepointonline"})
op = conn["properties"]["swagger"]["paths"]["/datasets/{dataset}/tables/{table}/items"]["get"]
print(op["operationId"], "—", op.get("summary"))

# Bad — keeping the whole 500 KB swagger in context
print(json.dumps(conn, indent=2))   # don't do this
```
---

##认证和连接说明

|字段|值||---|---|
|授权头|`x-api-key: <JWT>`- **不是**`Authorization: Bearer`|
|令牌格式|普通JWT -不剥离，更改或前缀|
|超时时间|`get_live_flow_run_action_outputs`（大输出）使用≥120s
|环境名称|`Default-<tenant-guid>`（通过`list_live_environments`或`list_live_flows`响应找到）|

---

##参考文件

- [MCP-BOOTSTRAP.md](references/MCP-BOOTSTRAP.md) - endpoint, auth，request/response格式（先读这个）
- [tool-reference.md](references/tool-reference.md) -响应形状和行为笔记（参数在`tool_search`）
- [action-types.md](references/action-types.md) - Power自动操作类型模式
- [connection-references.md](references/connection-references.md) -连接器参考指南