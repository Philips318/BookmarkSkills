---
name: flowstudio-power-automate-debug
description: >-
  Debug failing Power Automate cloud flows using the FlowStudio MCP server.
  The Graph API only shows top-level status codes. This skill gives your agent
  action-level inputs and outputs to find the actual root cause.
  Load this skill when asked to: debug a flow, investigate a failed run, why is
  this flow failing, inspect action outputs, find the root cause of a flow error,
  fix a broken Power Automate flow, diagnose a timeout, trace a DynamicOperationRequestFailure,
  check connector auth errors, read error details from a run, or troubleshoot
  expression failures. Requires a FlowStudio MCP subscription — see https://mcp.flowstudio.app
---
#电源自动调试与FlowStudio MCP

一步一步的诊断过程，用于调查故障的Power automation
云流通过FlowStudio MCP服务器。

> * * * *实际调试例子:[孩子流表达式错误](https://github.com/ninihen1/power-automate-mcp-skills/blob/main/examples/fix-expression-error.md) |
>[数据输入，不是流错误](https://github.com/ninihen1/power-automate-mcp-skills/blob/main/examples/data-not-flow.md) |
>[空值导致子流崩溃]（https://github.com/ninihen1/power-automate-mcp-skills/blob/main/examples/null-child-flow.md）

**先决条件**:FlowStudio MCP服务器必须通过有效的JWT可访问。
有关连接设置，请参见`flowstudio-power-automate-mcp`技能。
请订阅https://mcp.flowstudio.app---

##真理之源

b> **总是先呼叫`list_skills`/`tool_search`**确认可用的工具
>名称和参数模式。工具名称和参数可能在两者之间改变
>服务器版本。
>该技能包括响应形状、行为笔记和诊断模式
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

ENV = "<environment-id>"   # e.g. Default-xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx
```
---

##步骤1 -定位流程```python
result = mcp("list_live_flows", environmentName=ENV)
# Returns a wrapper object: {mode, flows, totalCount, error}
target = next(f for f in result["flows"] if "My Flow Name" in f["displayName"])
FLOW_ID = target["id"]   # plain UUID — use directly as flowName
print(FLOW_ID)
```
---

##步骤2 -找到失败的运行```python
runs = mcp("get_live_flow_runs", environmentName=ENV, flowName=FLOW_ID, top=5)
# Returns direct array (newest first):
# [{"name": "08584296068667933411438594643CU15",
#   "status": "Failed",
#   "startTime": "2026-02-25T06:13:38.6910688Z",
#   "endTime": "2026-02-25T06:15:24.1995008Z",
#   "triggerName": "manual",
#   "error": {"code": "ActionFailed", "message": "An action failed..."}},
#  {"name": "...", "status": "Succeeded", "error": null, ...}]

for r in runs:
    print(r["name"], r["status"], r["startTime"])

RUN_ID = next(r["name"] for r in runs if r["status"] == "Failed")
```
---

##步骤3 -获得顶层错误

b> **CRITICAL**:`get_live_flow_run_error`告诉你**哪个**操作失败了。
>`get_live_flow_run_action_outputs`告诉你**为什么**。你必须两个都打电话。
永远不要仅仅停留在错误上-错误代码如`ActionFailed`，
>`NotSpecified`和`InternalServerError`是通用包装器。实际的
>的根本原因（错误的字段，空值，HTTP 500正文，堆栈跟踪）是唯一的
>在动作的输入和输出中可见。```python
err = mcp("get_live_flow_run_error",
    environmentName=ENV, flowName=FLOW_ID, runName=RUN_ID)
# Returns:
# {
#   "runName": "08584296068667933411438594643CU15",
#   "failedActions": [
#     {"actionName": "Apply_to_each_prepare_workers", "status": "Failed",
#      "error": {"code": "ActionFailed", "message": "An action failed..."},
#      "startTime": "...", "endTime": "..."},
#     {"actionName": "HTTP_find_AD_User_by_Name", "status": "Failed",
#      "code": "NotSpecified", "startTime": "...", "endTime": "..."}
#   ],
#   "allActions": [
#     {"actionName": "Apply_to_each", "status": "Skipped"},
#     {"actionName": "Compose_WeekEnd", "status": "Succeeded"},
#     ...
#   ]
# }

# failedActions is ordered outer-to-inner. The ROOT cause is the LAST entry:
root = err["failedActions"][-1]
print(f"Root action: {root['actionName']} → code: {root.get('code')}")

# allActions shows every action's status — useful for spotting what was Skipped
# See common-errors.md to decode the error code.
```
---

步骤4 -检查失败动作的输入和输出

这是最重要的一步。**`get_live_flow_run_error`只给出
>你一个通用的错误代码。实际的错误细节- HTTP状态码，
>响应体、堆栈跟踪、空值——存在于操作的运行时
>输入和输出。**发生故障后立即检查
>标识**```python
# Get the root failing action's full inputs and outputs
root_action = err["failedActions"][-1]["actionName"]
detail = mcp("get_live_flow_run_action_outputs",
    environmentName=ENV,
    flowName=FLOW_ID,
    runName=RUN_ID,
    actionName=root_action)

if len(detail) > 1:
    print(f"{root_action} returned {len(detail)} repetitions; inspect iteration indexes")
out = detail[0] if detail else {}
print(f"Action: {out.get('actionName')}")
print(f"Status: {out.get('status')}")

# For HTTP actions, the real error is in outputs.body
if isinstance(out.get("outputs"), dict):
    status_code = out["outputs"].get("statusCode")
    body = out["outputs"].get("body", {})
    print(f"HTTP {status_code}")
    print(json.dumps(body, indent=2)[:500])

    # Error bodies are often nested JSON strings — parse them
    if isinstance(body, dict) and "error" in body:
        err_detail = body["error"]
        if isinstance(err_detail, str):
            err_detail = json.loads(err_detail)
        print(f"Error: {err_detail.get('message', err_detail)}")

# For expression errors, the error is in the error field
if out.get("error"):
    print(f"Error: {out['error']}")

# Also check inputs — they show what expression/URL/body was used
if out.get("inputs"):
    print(f"Inputs: {json.dumps(out['inputs'], indent=2)[:500]}")
```
动作输出显示了什么（错误代码没有显示）

|`get_live_flow_run_error`|错误代码`get_live_flow_run_action_outputs`显示||---|---|
|`ActionFailed`|嵌套操作实际失败及其HTTP响应|
|`NotSpecified`| HTTP状态码+实际错误|的响应体
|`InternalServerError`|服务器错误消息、堆栈跟踪或API错误JSON |
|`InvalidTemplate`|失败的精确表达式和null/wrong-type值|
|`BadRequest`|发送的请求体以及服务器拒绝它的原因|

### Foreach迭代

当`actionName`引用foreach中的操作时，输出工具可以
返回该动作的每次重复。每个项目可能包括`repetitionIndexes`与循环名称和从零开始的`itemIndex`。使用`iterationIndex`发现可疑项后检查一次迭代：```python
all_reps = mcp("get_live_flow_run_action_outputs",
    environmentName=ENV,
    flowName=FLOW_ID,
    runName=RUN_ID,
    actionName=root_action)

for rep in all_reps[:10]:
    print(rep.get("repetitionIndexes"), rep.get("status"), rep.get("error"))

one_rep = mcp("get_live_flow_run_action_outputs",
    environmentName=ENV,
    flowName=FLOW_ID,
    runName=RUN_ID,
    actionName=root_action,
    iterationIndex=3)
```
证据撰写书尾

对于不确定的连接器工作，请在危险操作之前添加`Compose_*_Request`在它之后是`Compose_*_Result`，两者都允许结果动作`Succeeded`和`Failed`。这为以后的调试提供了一个干净的负载快照
无需另一次部署。不包含秘密或长二进制有效载荷
在这些书挡里。

示例：HTTP操作返回500```
Error code: "InternalServerError" ← this tells you nothing

Action outputs reveal:
  HTTP 500
  body: {"error": "Cannot read properties of undefined (reading 'toLowerCase')
    at getClientParamsFromConnectionString (storage.js:20)"}
  ← THIS tells you the Azure Function crashed because a connection string is undefined
```
###示例：null表达式错误```
Error code: "BadRequest" ← generic

Action outputs reveal:
  inputs: "body('HTTP_GetTokenFromStore')?['token']?['access_token']"
  outputs: ""   ← empty string, the path resolved to null
  ← THIS tells you the response shape changed — token is at body.access_token, not body.token.access_token
```
---

##步骤5 -阅读流程定义```python
defn = mcp("get_live_flow", environmentName=ENV, flowName=FLOW_ID)
actions = defn["properties"]["definition"]["actions"]
print(list(actions.keys()))
```
在定义中找到失败的动作。检查它的`inputs`表达式
来理解它需要什么数据。

---

##第6步-从失败中走回来

当失败操作的输入引用上游操作时，检查这些操作
了。沿着链条往回走直到你找到
糟糕的数据:```python
# Inspect multiple actions leading up to the failure
for action_name in [root_action, "Compose_WeekEnd", "HTTP_Get_Data"]:
    result = mcp("get_live_flow_run_action_outputs",
        environmentName=ENV,
        flowName=FLOW_ID,
        runName=RUN_ID,
        actionName=action_name)
    out = result[0] if result else {}
    print(f"\n--- {action_name} ({out.get('status')}) ---")
    print(f"Inputs:  {json.dumps(out.get('inputs', ''), indent=2)[:300]}")
    print(f"Outputs: {json.dumps(out.get('outputs', ''), indent=2)[:300]}")
```
>⚠️数组处理操作的输出负载可能非常大。
>在打印前总是切片（例如`[:500]`）。

提示：当您不确定时，省略`actionName`以列出顶级操作
>哪个操作产生了坏数据。一旦你在foreach中选择了一个动作，
>传递`iterationIndex`以避免将每个重复都拉到上下文中。

---

##步骤7 -找出根本原因

表达式错误（例如`split`on null）
如果错误提到`InvalidTemplate`或函数名：
1. 在定义中找到动作
2. 检查它读取的上游action/expression3. **检查上游操作的输出**是否为空/缺失字段```python
# Example: action uses split(item()?['Name'], ' ')
# → null Name in the source data
result = mcp("get_live_flow_run_action_outputs", ..., actionName="Compose_Names")
if not result:
    print("No outputs returned for Compose_Names")
    names = []
else:
    names = result[0].get("outputs", {}).get("body") or []
nulls = [x for x in names if x.get("Name") is None]
print(f"{len(nulls)} records with null Name")
```
错误的字段路径
表达式`triggerBody()?['fieldName']`返回null→`fieldName`错误。
**检查触发器输出**查看实际字段名：```python
result = mcp("get_live_flow_run_action_outputs", ..., actionName="<trigger-action-name>")
print(json.dumps(result[0].get("outputs"), indent=2)[:500])
```
HTTP操作返回错误
错误代码说`InternalServerError`或`NotSpecified`- **总是检查
动作输出**来获取实际的HTTP状态和响应体：```python
result = mcp("get_live_flow_run_action_outputs", ..., actionName="HTTP_Get_Data")
out = result[0]
print(f"HTTP {out['outputs']['statusCode']}")
print(json.dumps(out['outputs']['body'], indent=2)[:500])
```
连接/验证失败
查找`ConnectionAuthorizationFailed`—连接所有者必须匹配
运行流的服务帐户。无法通过API修复；修复了PA设计器。

Outlook用户选择器失败（`DynamicListValuesUndefinedOrInvalid`）
Outlook操作如`GetEmailsV3`使用参数(`mailboxAddress`,`to`,`cc`，`from`)，其下拉列表由`builtInOperation:AadGraph.GetUsers`- which支持
在PA监听层被打破，总是返回`DynamicListValuesUndefinedOrInvalid`。当代理重建或
通过`update_live_flow`修改Outlook操作，并尝试解析用户
通过动态选项。**不要通过重试AadGraph**来修复它-切换到
而是`shared_office365users.SearchUserV2`（返回相同的AAD用户形状）。
使用`describe_live_connector`确认受影响的参数是否暴露
一个结构化的`fallback`，然后调用`get_live_dynamic_options`对`shared_office365users.SearchUserV2`，而不是损坏的AadGraph操作。
对于动态字段模式而不是下拉选项，使用
返回的元数据`describe_live_connector`。---

##步骤8 -应用修复

**对于expression/data问题**：```python
defn = mcp("get_live_flow", environmentName=ENV, flowName=FLOW_ID)
acts = defn["properties"]["definition"]["actions"]

# Example: fix split on potentially-null Name
acts["Compose_Names"]["inputs"] = \
    "@coalesce(item()?['Name'], 'Unknown')"

conn_refs = defn["properties"]["connectionReferences"]
result = mcp("update_live_flow",
    environmentName=ENV,
    flowName=FLOW_ID,
    definition=defn["properties"]["definition"],
    connectionReferences=conn_refs)

print(result.get("error"))  # None = success
```
>⚠️`update_live_flow`总是返回一个`error`键。
>`null`（Python中的`None`）表示成功。

---

##步骤9 -验证修复

b> **使用`resubmit_live_flow_run`测试任何流-不仅仅是HTTP触发器
>`resubmit_live_flow_run`使用其原始触发器重播以前的运行
>负载。这适用于**所有触发器类型**：复发，SharePoint
>“当一个项目被创建时”，连接器webhooks， Button触发器和HTTP
>触发器。您不需要要求用户手动触发流或
>等待下一次预定的运行。
>
>`resubmit`不可用的唯一情况是一个全新的流
>从来没有运行** -它没有先前的运行重播。```python
# Resubmit the failed run — works for ANY trigger type
resubmit = mcp("resubmit_live_flow_run",
    environmentName=ENV, flowName=FLOW_ID, runName=RUN_ID)
print(resubmit)   # {"resubmitted": true, "triggerName": "..."}

# Wait ~30 s then check
import time; time.sleep(30)
new_runs = mcp("get_live_flow_runs", environmentName=ENV, flowName=FLOW_ID, top=3)
print(new_runs[0]["status"])   # Succeeded = done
```
何时使用resubmit和trigger

|场景|使用|为何使用||---|---|---|
| **在任何流上测试修复** |`resubmit_live_flow_run`|重播导致失败的确切触发有效负载-验证|的最佳方式
|循环/预定流量|`resubmit_live_flow_run`|不能按需触发|
| SharePoint / connector trigger |`resubmit_live_flow_run`|需要创建实SP项|才能触发
| HTTP触发器与**自定义**测试负载|`trigger_live_flow`|当你需要发送不同的数据比原来运行|
|全新的流，从未运行|`trigger_live_flow`（仅HTTP） |没有先前的运行存在重新提交|

使用自定义有效负载测试http触发流

对于带有`Request`（HTTP）触发器的流，请在以下情况下使用`trigger_live_flow`需要发送一个与原始运行不同的有效载荷：```python
# First inspect what the trigger expects — read directly from the flow definition
defn = mcp("get_live_flow", environmentName=ENV, flowName=FLOW_ID)
triggers = defn["properties"]["definition"]["triggers"]
manual = next(iter(triggers.values()))   # usually the only trigger on HTTP flows
request_schema = manual.get("inputs", {}).get("schema")
print("Expected body schema:", request_schema)

# Response schemas live on Response action(s) in the actions block
for name, act in defn["properties"]["definition"]["actions"].items():
    if act.get("type") == "Response":
        print(f"Response {name}:", act.get("inputs", {}).get("schema"))

# Trigger with a test payload
result = mcp("trigger_live_flow",
    environmentName=ENV,
    flowName=FLOW_ID,
    body={"name": "Test User", "value": 42})
print(f"Status: {result['responseStatus']}, Body: {result.get('responseBody')}")
```
>`trigger_live_flow`自动处理aad认证的触发器。
>仅适用于触发类型为`Request`（HTTP）的流。

---

快速参考诊断决策树

|第一工具|然后总是调用|找什么||---|---|---|---|
|流程显示为失败的|`get_live_flow_run_error`|`get_live_flow_run_action_outputs`在失败的动作| HTTP状态+响应体在`outputs`|
|错误码为generic (`ActionFailed`,`NotSpecified`) | - |`get_live_flow_run_action_outputs`|`outputs.body`包含实际错误消息、堆栈跟踪或API错误|
| HTTP操作返回500 | - |`get_live_flow_run_action_outputs`|`outputs.statusCode`+`outputs.body`服务器错误详细信息|
|表达式崩溃| - |`get_live_flow_run_action_outputs`对先前的动作|输出体|中的空/错误类型字段
|流量从未启动|`get_live_flow`| - |检查`properties.state`= "Started" |
|操作返回错误数据|`get_live_flow_run_action_outputs`| - |实际输出体与预期|
修复应用，但仍然失败|`get_live_flow_runs`后，重新提交| - |新的运行`status`字段|

b> **规则：永远不要仅从错误代码进行诊断。* *`get_live_flow_run_error`>标识失败操作。`get_live_flow_run_action_outputs`揭示了
>实际原因。两者都要打电话。

---

##参考文件- [common-errors.md](references/common-errors.md) -错误代码，可能的原因，和修复
- [debug-workflow.md](references/debug-workflow.md) -复杂故障的全决策树

相关技能

-`flowstudio-power-automate-mcp`-基础技能：连接设置，MCP助手，工具发现
-`flowstudio-power-automate-build`-构建和部署新流