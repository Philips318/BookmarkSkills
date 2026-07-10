---
name: flowstudio-power-automate-build
description: >-
  Build, scaffold, and deploy Power Automate cloud flows using the FlowStudio
  MCP server. Your agent constructs flow definitions, wires connections, deploys,
  and tests — all via MCP without opening the portal.
  Load this skill when asked to: create a flow, build a new flow,
  deploy a flow definition, scaffold a Power Automate workflow, construct a flow
  JSON, update an existing flow's actions, patch a flow definition, add actions
  to a flow, wire up connections, or generate a workflow definition from scratch.
  Requires a FlowStudio MCP subscription — see https://mcp.flowstudio.app
---
#使用FlowStudio MCP构建和部署Power automation flow

构建和部署Power automation云流的分步指南
以编程方式通过FlowStudio MCP服务器。

**先决条件**:FlowStudio MCP服务器必须通过有效的JWT可访问。
有关连接设置，请参见`flowstudio-power-automate-mcp`技能。
订阅https://mcp.flowstudio.app工作流程:
1. 加载当前构建工具。
2. 检查是否存在流。
3. 解析连接引用。
4. 构建定义。
5. 部署。
6. 核实。
7. 测试。

---

##真理之源

b> **总是先呼叫`list_skills`/`tool_search`**确认可用的工具
>名称和参数模式。工具名称和参数可能在两者之间改变
>服务器版本。
>该技能包括响应形状、行为笔记和构建模式
工具模式无法告诉您的事情。如果这个文件不同意
>`tool_search`或真正的API响应，API胜出。

---

## Python Helper```python
import json, urllib.request

MCP_URL   = "https://mcp.flowstudio.app/mcp"
MCP_TOKEN = "<YOUR_JWT_TOKEN>"

def mcp(tool, **kwargs):
    payload = json.dumps({"jsonrpc": "2.0", "id": 1, "method": "tools/call",
                          "params": {"name": tool, "arguments": kwargs}}).encode()
    req = urllib.request.Request(MCP_URL, data=payload,
        headers={"x-api-key": MCP_TOKEN, "Content-Type": "application/json",
                 "User-Agent": "FlowStudio-MCP/1.0"})
    try:
        resp = urllib.request.urlopen(req, timeout=120)
    except urllib.error.HTTPError as e:
        body = e.read().decode("utf-8", errors="replace")
        raise RuntimeError(f"MCP HTTP {e.code}: {body[:200]}") from e
    raw = json.loads(resp.read())
    if "error" in raw:
        raise RuntimeError(f"MCP error: {json.dumps(raw['error'])}")
    return json.loads(raw["result"]["content"][0]["text"])

ENV = "<environment-id>"  # e.g. Default-xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx
```
---

# # 0。加载当前构建工具

对于一个全新的流，加载服务器的`create-flow`包。用于编辑
现有流量，负载`build-flow`。这使代理与MCP保持一致
在构造JSON之前获取服务器的当前模式。```python
schemas = mcp("tool_search", query="skill:create-flow")
# Includes list_live_environments, list_live_connections,
# describe_live_connector, get_live_dynamic_options, update_live_flow.
```
如果你需要一个包外的工具，显式加载它：```python
mcp("tool_search", query="select:get_live_dynamic_properties")
```
---

# # 1。安全检查：流程是否已经存在？

在构建之前一定要注意避免重复：```python
results = mcp("list_live_flows",
    environmentName=ENV,
    mode="owner",
    search="My New Flow",
    top=20)

# list_live_flows returns { "flows": [...], "mode": "...", ... }
matches = [f for f in results["flows"]
           if "My New Flow".lower() in f["displayName"].lower()]

if len(matches) > 0:
    # Flow exists — modify rather than create
    FLOW_ID = matches[0]["id"]   # plain UUID from list_live_flows
    print(f"Existing flow: {FLOW_ID}")
    defn = mcp("get_live_flow", environmentName=ENV, flowName=FLOW_ID)
else:
    print("Flow not found — building from scratch")
    FLOW_ID = None
```
对于非常大的环境，`list_live_flows`可能返回一个延续URL。
将其作为`continuationUrl`返回，并使用相同的`mode`检索下一个
批处理。仅当用户需要所有环境流和时才使用`mode="admin"`MCP身份具有管理员权限。

---

# # 2。获取连接参考

控件中的一个键，每个连接器操作都需要一个`connectionName`Flow的`connectionReferences`映射。该密钥链接到一个经过身份验证的连接
在环境中。

b> **必选**：你必须先调用`list_live_connections`-不要问
>用户用于连接名称或guid。API返回您需要的确切值。
>仅在API确认缺少所需连接时才提示用户。

### 2a -查找活动连接```python
conns = mcp("list_live_connections", environmentName=ENV)
active = [c for c in conns["connections"]
          if c["statuses"][0]["status"] == "Connected"]
conn_map = {c["connectorName"]: c["id"] for c in active}
```
对于已知的连接器，传递`search`以减少输出并准备粘贴`connectionReferenceTemplate`和`hostTemplate`值：```python
sp_conns = mcp("list_live_connections",
    environmentName=ENV,
    search="shared_sharepointonline")
```
### 2b -确定流需要哪些连接器

常用连接器API名称：SharePoint`shared_sharepointonline`、Outlook`shared_office365`，团队`shared_teams`，审批`shared_approvals`，
OneDrive`shared_onedriveforbusiness`, Excel`shared_excelonlinebusiness`，
数据规避`shared_commondataserviceforapps`，表单`shared_microsoftforms`。

不需要连接器的流（例如recursion + Compose + HTTP）可以
省略`connectionReferences`。

### 2c -如果连接丢失，引导用户```python
connectors_needed = ["shared_sharepointonline", "shared_office365"]  # adjust per flow
missing = [c for c in connectors_needed if c not in conn_map]
if missing:
    # STOP: connections require browser OAuth consent.
    # Ask the user to create the missing connector connections in the
    # selected environment, then re-run list_live_connections.
    raise Exception(f"Missing active connections: {missing}")
```
### 2d -构建connectionReferences块```python
connection_references = {}
host_templates = {}
for connector in connectors_needed:
    c = next(c for c in active if c["connectorName"] == connector)
    connection_references[connector] = c.get("connectionReferenceTemplate") or {
        "connectionName": c["id"],   # the connection id from list_live_connections
        "source": "Invoker",
        "id": f"/providers/Microsoft.PowerApps/apis/{connector}"
    }
    host_templates[connector] = c.get("hostTemplate") or {
        "connectionName": connector
    }
```
在步骤3 action JSON中，`inputs.host.connectionName`必须是映射键，例如`shared_teams`，不是GUID。GUID只属于`connectionReferences[connector].connectionName`价值。如果现有流使用
相同的连接器，也可以复制其`properties.connectionReferences`从`get_live_flow`。

---

# # 3。构建流定义

构造定义对象。看到[flow-schema.md] (references/flow-schema.md)
对于完整的模式和这些复制-粘贴模板的操作模式引用：
- [action-patterns-core.md](references/action-patterns-core.md) -变量，控制流，表达式
- [action-patterns-data.md](references/action-patterns-data.md) -数组转换，HTTP，解析
- [action-patterns-connectors.md](references/action-patterns-connectors.md) - SharePoint, Outlook, Teams, Approvals```python
definition = {
    "$schema": "https://schema.management.azure.com/providers/Microsoft.Logic/schemas/2016-06-01/workflowdefinition.json#",
    "contentVersion": "1.0.0.0",
    "triggers": { ... },   # see trigger-types.md / build-patterns.md
    "actions": { ... }     # see ACTION-PATTERNS-*.md / build-patterns.md
}
```
>参见[build-patterns.md]（references/build-patterns.md）获得完整的，即用型的
>流定义涵盖了复发+SharePoint+团队，HTTP触发器等。

###在猜测JSON之前发现连接器操作

对于连接器支持的triggers/actions，更倾向于使用活动连接器描述符
手写的形状。它可以返回撰写的提示，规范示例，变体
键、inputs/outputs和动态元数据指针。```python
# Search across connectors when you know the user's intent but not the API.
matches = mcp("describe_live_connector",
    environmentName=ENV,
    search="send email",
    top=5)

# Describe a specific operation before copying an exampleDefinition.
op = mcp("describe_live_connector",
    environmentName=ENV,
    connectorName="shared_office365",
    operationId="SendEmailV2")
print(op.get("hint"))
```
当一个操作有多个已授权的变体时，请向流请求该变体
需求:```python
teams_chat = mcp("describe_live_connector",
    environmentName=ENV,
    connectorName="shared_teams",
    operationId="PostMessageToConversation",
    variant="flowbot_chat")
```
当操作描述说参数有动态选项或动态选项时
属性，调用指定的下一个工具：```python
sp_op = mcp("describe_live_connector",
    environmentName=ENV,
    connectorName="shared_sharepointonline",
    operationId="GetItems")

sites = mcp("get_live_dynamic_options",
    environmentName=ENV,
    connectorName="shared_sharepointonline",
    connectionName=conn_map["shared_sharepointonline"],
    operationId="GetItems",
    parameterName="dataset",
    dynamicMetadata=sp_op["dynamicParameters"]["dataset"])

fields = mcp("get_live_dynamic_properties",
    environmentName=ENV,
    connectorName="shared_sharepointonline",
    connectionName=conn_map["shared_sharepointonline"],
    operationId="GetItems",
    parameterName="item",
    parameters={"dataset": "<site-url>", "table": "<list-id>"},
    dynamicMetadata=sp_op["dynamicProperties"]["item"])
```
对下拉id使用动态选项，如SharePointsites/lists和Teams
为schema/field形状使用动态属性，例如
SharePoint列表项列。

---

# # 4。部署（创建或更新）`update_live_flow`在一个工具中处理创建和更新。

###创建一个新流（没有现有流）

省略`flowName`-服务器生成一个新的GUID，并通过PUT创建：```python
definition["description"] = "Weekly SharePoint → Teams notification flow, built by agent"

result = mcp("update_live_flow",
    environmentName=ENV,
    # flowName omitted → creates a new flow
    definition=definition,
    connectionReferences=connection_references,
    displayName="Overdue Invoice Notifications"
)

if result.get("error") is not None:
    print("Create failed:", result["error"])
else:
    # Capture the new flow ID for subsequent steps
    FLOW_ID = result["created"]
    print(f"✅ Flow created: {FLOW_ID}")
```
更新现有流

提供`flowName`给PATCH：```python
definition["description"] = (
    "Updated by agent on " + __import__('datetime').datetime.utcnow().isoformat()
)

result = mcp("update_live_flow",
    environmentName=ENV,
    flowName=FLOW_ID,
    definition=definition,
    connectionReferences=connection_references,
    displayName="My Updated Flow"
)

if result.get("error") is not None:
    print("Update failed:", result["error"])
else:
    print("Update succeeded:", result)
```
>⚠️`update_live_flow`总是返回一个`error`键。
>`null`（Python`None`）表示成功-不要将键的存在视为失败。
>
>⚠️流描述存在于`definition["description"]`。当前服务器
>附加`#flowstudio-mcp`用于跟踪使用情况。不通过顶级
>`description`参数，除非`tool_search`在活动模式中显示一个。

常见的部署错误

|错误信息（包含）|原因|修复||---|---|---|
|`missing from connectionReferences`|一个动作的`host.connectionName`引用了一个在`connectionReferences`映射中不存在的键|确保`host.connectionName`使用`connectionReferences`中的**键**（例如`shared_teams`），而不是原始GUID |
|`ConnectionAuthorizationFailed`/ 403 |连接GUID属于其他用户或未授权|重新运行步骤2a，使用当前`x-api-key`用户|拥有的连接
|`InvalidTemplate`/`InvalidDefinition`|定义JSON语法错误|检查`runAfter`链、表达式语法和动作类型拼写|
|`ConnectionNotConfigured`|存在连接器操作，但连接GUID无效或过期|重新检查`list_live_connections`是否有新的GUID |

---

# # 5。验证部署```python
check = mcp("get_live_flow", environmentName=ENV, flowName=FLOW_ID)

# Confirm state
print("State:", check["properties"]["state"])  # Should be "Started"
# If state is "Stopped", use set_live_flow_state — NOT update_live_flow
# mcp("set_live_flow_state", environmentName=ENV, flowName=FLOW_ID, state="Started")

# Confirm the action we added is there
acts = check["properties"]["definition"]["actions"]
print("Actions:", list(acts.keys()))
```
---

# # 6。测试流程

> **必选**：在触发任何测试运行之前，**要求用户确认**。
运行流有真正的副作用——它可以发送电子邮件，发布团队消息，
>写SharePoint，开始审批，或调用外部api。解释一下
>流将在调用`trigger_live_flow`之前执行并等待显式批准
>或`resubmit_live_flow_run`。

更新的流（有先前的运行）-任何触发器类型

b> **先使用`resubmit_live_flow_run`。**它适用于每一个触发类型-
>复发，SharePoint，连接器webhook，按钮和HTTP。这回放
>原始触发器有效载荷。不要让用户手动触发
>流或等待下一次计划运行。```python
runs = mcp("get_live_flow_runs", environmentName=ENV, flowName=FLOW_ID, top=1)
if runs:
    # Works for Recurrence, SharePoint, connector triggers — not just HTTP
    result = mcp("resubmit_live_flow_run",
        environmentName=ENV, flowName=FLOW_ID, runName=runs[0]["name"])
    print(result)   # {"resubmitted": true, "triggerName": "..."}
```
http触发流-自定义测试负载

只有在需要发送不同的有效载荷时才使用`trigger_live_flow`比原来的运行。为了验证修复，`resubmit_live_flow_run`是
更好是因为它使用了导致故障的确切数据。```python
defn = mcp("get_live_flow", environmentName=ENV, flowName=FLOW_ID)
triggers = defn["properties"]["definition"]["triggers"]
manual = next(iter(triggers.values()))
print("Expected body:", manual.get("inputs", {}).get("schema"))

result = mcp("trigger_live_flow",
    environmentName=ENV, flowName=FLOW_ID,
    body={"name": "Test", "value": 1})
print(f"Status: {result['responseStatus']}")
```
全新的非http流（递归、连接器触发器等）

一个全新的递归流或连接器触发流没有先前的运行
没有HTTP端点可调用。这是唯一的场景，你
需要下面的临时HTTP触发器方法。**使用临时部署
首先是HTTP触发器，测试操作，然后切换到生产触发器

紧凑的食谱:```python
production_trigger = definition["triggers"]
definition["triggers"] = {
    "manual": {"type": "Request", "kind": "Http", "inputs": {"schema": {}}}
}

result = mcp("update_live_flow",
    environmentName=ENV,
    flowName=FLOW_ID,       # omit if creating new
    definition=definition,
    connectionReferences=connection_references,
    displayName="Overdue Invoice Notifications")
FLOW_ID = FLOW_ID or result["created"]

test = mcp("trigger_live_flow", environmentName=ENV, flowName=FLOW_ID,
           body={"sample": "payload"})
runs = mcp("get_live_flow_runs", environmentName=ENV, flowName=FLOW_ID, top=1)

if runs[0]["status"] == "Failed":
    err = mcp("get_live_flow_run_error",
        environmentName=ENV, flowName=FLOW_ID, runName=runs[0]["name"])
    raise Exception(err["failedActions"][-1])

definition["triggers"] = production_trigger
mcp("update_live_flow",
    environmentName=ENV,
    flowName=FLOW_ID,
    definition=definition,
    connectionReferences=connection_references)
```
触发器只是一个入口点；通过HTTP进行测试仍然使用
相同的动作。如果操作使用`triggerBody()`或`triggerOutputs()`，则传递a
代表`trigger_live_flow.body`形状的生产触发器
有效载荷。

---

# #陷阱

|错误|后果|预防||---|---|---|
|部署中缺少`connectionReferences`| 400“供应连接参考”|总是先调用`list_live_connections`|
|`"operationOptions"`Foreach缺失|并行执行，写时有竞争条件|总是添加`"Sequential"`|
|`union(old_data, new_data)`|旧值覆盖新的（第一胜）|使用`union(new_data, old_data)`|
|`split()`对潜在的空字符串|`InvalidTemplate`崩溃|与`coalesce(field, '')`|
|检查`result["error"]`是否存在|始终存在；正确错误是`!= null`|使用`result.get("error") is not None`|
|流量部署，但状态为“停止”|流量不会按计划运行|调用`set_live_flow_state`与`state: "Started"`-不要** **使用`update_live_flow`状态更改|
| Teams“Chat with Flow bot”接收者作为对象| 400`GraphUserDetailNotFound`|使用带尾分号的普通字符串（见下文）|
| Copilot Studio可能不会将其发现为代理工具|部署后，使用目标`solutionId`|调用`add_live_flow_to_solution`|Button/Skills触发器用于MCP测试| MCP不能直接发射p生产触发器|通过一个临时HTTP双胞胎测试相同的操作，然后将触发器交换回|
|连接器动作缺失`metadata.operationMetadataId`|Designer/run-onlyUI行为不一致|保留现有id；为新的连接器动作|添加稳定的guid
|占位符Excel`scriptId`|动态验证在节省时间时失败|在部署|之前解析真实的Office脚本ID
| SharePoint`PatchItem`省略必填字段|即使字段没有更改，保存也可能失败|回显未更改的必填字段，如`item/Title`|
| Copilot Studio连接器调用代理草案|连接器调用可能失败或遇到过时行为|在testing/resubmitting流|之前发布代理### Teams`PostMessageToConversation`-收件人格式`body/recipient`参数格式取决于`location`的值：

|位置|`body/recipient`格式|示例||---|---|---|
| **聊天流量bot** |普通的电子邮件字符串与**尾分号** |`"user@contoso.com;"`|
| **通道** |对象与`groupId`和`channelId`|`{"groupId": "...", "channelId": "..."}`|

b> **常见错误**：传递`{"to": "user@contoso.com"}`为“聊天与流量机器人”
>返回400`GraphUserDetailNotFound`错误。API需要一个普通字符串。

---

##参考文件

- [flow-schema.md](references/flow-schema.md) -全流定义JSON模式
- [trigger-types.md](references/trigger-types.md) -触发类型模板
- [action-patterns-core.md](references/action-patterns-core.md) -变量，控制流，表达式
- [action-patterns-data.md](references/action-patterns-data.md) -数组转换，HTTP，解析
- [action-patterns-connectors.md](references/action-patterns-connectors.md) - SharePoint, Outlook, Teams, Approvals
- [build-patterns.md](references/build-patterns.md) -完整的流程定义模板（recurrent +SP+Teams， HTTP触发器）

相关技能

-`flowstudio-power-automate-mcp`-核心连接设置和工具参考
—`flowstudio-power-automate-debug`—部署后调试失败流程