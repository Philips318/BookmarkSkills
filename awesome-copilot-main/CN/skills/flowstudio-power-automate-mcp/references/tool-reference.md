# FlowStudio MCP -工具响应目录

响应形状和行为笔记为FlowStudio Power automation MCP服务器。

b> **对于工具名称和参数**：首选`list_skills`和`tool_search`。
>它们返回集中的、最新的模式，而无需一次加载每个MCP工具。
>仅在元工具不可用时使用`tools/list`作为低级回退。
>这个文档涵盖了工具模式没有告诉你的：**响应形状**
通过实际使用发现的>和**非明显行为**。

---

##真理之源

|优先级|源|覆盖||----------|--------|--------|
| 1 | **真实的API响应** |始终相信服务器实际返回的|
| **`list_skills`/`tool_search`** |工具名称、参数名称、类型、所需标志|
| 3 | **本文档** |响应形状，行为笔记，陷阱|

>如果本文档与`tool_search`、`tools/list`或实际API不一致
b>行为，API获胜。请相应地更新本文档。

---

##环境和租户发现

# # #`list_live_environments`响应：环境的直接数组。```json
[
  {
    "id": "Default-26e65220-5561-46ef-9783-ce5f20489241",
    "displayName": "FlowStudio (default)",
    "sku": "Production",
    "location": "australia",
    "state": "Enabled",
    "isDefault": true,
    "isAdmin": true,
    "isMember": true,
    "createdTime": "2023-08-18T00:41:05Z"
  }
]
```
b>在所有其他工具中使用`id`值作为`environmentName`。

# # #`list_store_environments`与`list_live_environments`形状相同，但从缓存读取（更快）。

---

##发现连接

# # #`list_live_connections`响应：包装对象与`connections`数组。```json
{
  "connections": [
    {
      "id": "shared-office365-9f9d2c8e-55f1-49c9-9f9c-1c45d1fbbdce",
      "displayName": "user@contoso.com",
      "connectorName": "shared_office365",
      "environment": "Default-26e65220-...",
      "createdBy": "User Name",
      "authenticatedUser": "user@contoso.com",
      "overallStatus": "Connected",
      "statuses": [{"status": "Connected"}],
      "createdTime": "2024-03-12T21:23:55.206815Z",
      "connectionReferenceTemplate": {
        "connectionName": "shared-office365-9f9d2c8e-55f1-49c9-9f9c-1c45d1fbbdce",
        "source": "Invoker",
        "id": "/providers/Microsoft.PowerApps/apis/shared_office365"
      },
      "hostTemplate": {
        "connectionName": "shared_office365"
      }
    }
  ],
  "totalCount": 56,
  "error": null
}
```
> **关键字段**:`id`是`connectionReferences`中使用的`connectionName`值。
>
> **关键字段**:`connectorName`映射到apiId：
>`"/providers/Microsoft.PowerApps/apis/" + connectorName`>
>按状态过滤：当存在时优先选择`overallStatus == "Connected"`；否则
>检查`statuses[0].status == "Connected"`。
>
>对于构建工作流，传递`environmentName`以避免使用来自的连接
>错误的环境。只有在有意清点连接时才省略它
>在所有环境。
>
>通过`search=<connector or account>`窄化输出接收
>`connectionReferenceTemplate`+`hostTemplate`可复制的值
>直接变成`update_live_flow`。

# # #`list_store_connections`来自缓存的相同连接数据。

---

##流程发现和列表

# # #`list_live_flows`响应：包装对象与`flows`数组。```json
{
  "mode": "owner",
  "flows": [
    {
      "id": "0757041a-8ef2-cf74-ef06-06881916f371",
      "displayName": "My Flow",
      "state": "Started",
      "triggerType": "Request",
      "triggerKind": "Http",
      "createdTime": "2023-08-18T01:18:17Z",
      "lastModifiedTime": "2023-08-18T12:47:42Z",
      "owners": "<aad-object-id>",
      "definitionAvailable": true
    }
  ],
  "totalCount": 100,
  "nextLink": null,
  "error": null
}
```
>通过`result["flows"]`访问。`id`是一个普通的UUID——直接作为`flowName`使用。
>
>`mode`表示使用的访问范围（`"owner"`或`"admin"`）。
>
>新版本服务器新增参数：
> -`search`：根据显示名称过滤服务器端。
> -`mode`:`owner`用于MCP身份拥有的流；`admin`表示所有流
>对管理帐户可见。
> -`timeoutSeconds`：使用`nextLink`返回部分结果，而不是等待
>在非常大的环境中。
> -`continuationUrl`：传递前一个`nextLink`以继续相同的查询。

# # #`list_store_flows`响应：**直接数组**（没有包装）。```json
[
  {
    "id": "3991358a-f603-e49d-b1ed-a9e4f72e2dcb.0757041a-8ef2-cf74-ef06-06881916f371",
    "displayName": "Admin | Sync Template v3 (Solutions)",
    "state": "Started",
    "triggerType": "OpenApiConnectionWebhook",
    "environmentName": "3991358a-f603-e49d-b1ed-a9e4f72e2dcb",
    "runPeriodTotal": 100,
    "createdTime": "2023-08-18T01:18:17Z",
    "lastModifiedTime": "2023-08-18T12:47:42Z"
  }
]
```
> **`id`格式**:`<environmentId>.<flowId>`——split在第一个`.`上提取流UUID：
>`flow_id = item["id"].split(".", 1)[1]`# # #`get_store_flow`响应：来自缓存的单流元数据（选定的字段）。```json
{
  "id": "<environmentId>.<flowId>",
  "displayName": "My Flow",
  "state": "Started",
  "triggerType": "Recurrence",
  "runPeriodTotal": 100,
  "runPeriodFailRate": 0.1,
  "runPeriodSuccessRate": 0.9,
  "runPeriodFails": 10,
  "runPeriodSuccess": 90,
  "runPeriodDurationAverage": 29410.8,
  "runPeriodDurationMax": 158900.0,
  "runError": "{\"code\": \"EACCES\", ...}",
  "description": "Flow description",
  "tier": "Premium",
  "complexity": "{...}",
  "actions": 42,
  "connections": ["sharepointonline", "office365"],
  "owners": ["user@contoso.com"],
  "createdBy": "user@contoso.com"
}
```
>`runPeriodDurationAverage`/`runPeriodDurationMax`的单位是**毫秒**（除以1000）。
>`runError`是一个**JSON字符串**——解析`json.loads()`。

---

流定义（Live API）

# # #`get_live_flow`响应：来自PA API的完整流程定义。```json
{
  "name": "<flow-guid>",
  "properties": {
    "displayName": "My Flow",
    "state": "Started",
    "definition": {
      "triggers": { "..." },
      "actions": { "..." },
      "parameters": { "..." }
    },
    "connectionReferences": { "..." }
  }
}
```
# # #`update_live_flow`**创建模式**：省略`flowName`——创建一个新流。需要`definition`和`displayName`。

**更新模式**：提供`flowName`-补丁现有流程。

回应:```json
{
  "created": false,
  "flowKey": "<environmentId>.<flowId>",
  "updated": ["definition", "connectionReferences"],
  "displayName": "My Flow",
  "state": "Started",
  "definition": { "...full definition..." },
  "error": null
}
```
>`error`**总是存在**，但可能是`null`。检查`result.get("error") is not None`。
>
>创建时：`created`是新的流GUID（字符串）。更新：`created`是`false`。
>
>服务器版本不同，必填字段也不同。使用`tool_search`with
>`select:update_live_flow`在创建或修补流之前；如果是描述
>是必需的，包括新的描述或来自的现有描述
>`get_live_flow`。
>
流描述是工作流定义的一部分（`definition.description`），
>不是当前模式中的顶级工具参数。

# # #`add_live_flow_to_solution`将非解决方案流迁移到解决方案中。如果已经在解决方案中，则返回错误。

在创建一个Copilot Studio skills触发的流程后使用它
可作为代理工具发现。传递`solutionId`作为目标解。如果
服务器支持省略`solutionId`，它使用环境的默认解决方案；
更喜欢为生产ALM提供明确的非托管解决方案。此工具仅更改解决方案成员关系。它不验证触发器
模式、发布Copilot Studio代理或证明流可由
代理。

---

连接器操作发现

# # #`describe_live_connector`描述connector/API及其操作。在创建连接器之前使用它
操作，而不是猜测操作JSON。

常见的模式:

|呼叫形状|使用||---|---|
|`search="send email"`无`connectorName`|跨连接器|的搜索操作
|`connectorName="shared_sharepointonline"`|一个连接器|的紧凑操作目录
|`operationId="GetItems"`|单个操作|的扩展模式
|`variant="flowbot_chat"`|一个操作变体|的授权示例

操作细节可以包括：
-`hint`：来自连接器提示表的编写指导。
—`exampleDefinition`：可复制action/trigger形状。
—动态元数据与`nextTool=get_live_dynamic_options`或`nextTool=get_live_dynamic_properties`。

# # #`get_live_dynamic_options`解析连接器参数的实时dropdown/list选项。用它来
从列表中选择的id，例如SharePointsites/lists， Teamsteams/channels，
或其他`x-ms-dynamic-list`/`x-ms-dynamic-values`参数。

传递由`describe_live_connector`返回的`dynamicMetadata`对象
来自`list_live_connections`的连接id，以及任何已解析的依赖项
参数。

# # #`get_live_dynamic_properties`解析连接器参数的实时schema/field属性。用它来
动态字段集，如SharePoint列表项列后的站点和列表
是已知的。

有用的参数:
—`parameters`：依赖值，例如' {"dataset": "<site-url>"，
“table”： “<list-id>”} "。
—`propertyName`：检查压缩响应后请求一个字段。
-`includeRaw`：仅在需要时包含原始连接器模式；它可以很大。

---

##运行历史和监控

# # #`get_live_flow_runs`响应：运行的直接数组（最新的优先）。```json
[{
  "name": "<run-id>",
  "status": "Succeeded|Failed|Running|Cancelled",
  "startTime": "2026-02-25T06:13:38Z",
  "endTime": "2026-02-25T06:14:02Z",
  "triggerName": "Recurrence",
  "error": null
}]
```
>`top`默认为**30**，并为更高的值自动分页。设置`top: 300`>， 24小时报道每5分钟一次的流量。
>
>运行ID字段为**`name`**，而不是`runName`。使用此值作为`runName`参数“>”。

# # #`get_live_flow_run_error`响应：运行失败的结构化错误分解。```json
{
  "runName": "08584296068667933411438594643CU15",
  "failedActions": [
    {
      "actionName": "Apply_to_each_prepare_workers",
      "status": "Failed",
      "error": {"code": "ActionFailed", "message": "An action failed."},
      "code": "ActionFailed",
      "startTime": "2026-02-25T06:13:52Z",
      "endTime": "2026-02-25T06:15:24Z"
    },
    {
      "actionName": "HTTP_find_AD_User_by_Name",
      "status": "Failed",
      "code": "NotSpecified",
      "startTime": "2026-02-25T06:14:01Z",
      "endTime": "2026-02-25T06:14:05Z"
    }
  ],
  "allActions": [
    {"actionName": "Apply_to_each", "status": "Skipped"},
    {"actionName": "Compose_WeekEnd", "status": "Succeeded"},
    {"actionName": "HTTP_find_AD_User_by_Name", "status": "Failed"}
  ]
}
```
>`failedActions`从外部到内部排序——**最后一个条目是根本原因**。
>使用`failedActions[-1]["actionName"]`作为诊断起点。

# # #`get_live_flow_run_action_outputs`Response：动作细节对象的数组。```json
[
  {
    "actionName": "Compose_WeekEnd_now",
    "status": "Succeeded",
    "startTime": "2026-02-25T06:13:52Z",
    "endTime": "2026-02-25T06:13:52Z",
    "error": null,
    "inputs": "Mon, 25 Feb 2026 06:13:52 GMT",
    "outputs": "Mon, 25 Feb 2026 06:13:52 GMT"
  }
]
```
> **`actionName`是可选的**：省略它将在运行中返回顶级操作。
>为特定的操作提供它。如果该操作在foreach中运行，则
>工具可以在迭代中返回该操作的每次重复；通过
>`iterationIndex`引脚到一个基于零的迭代。
>
>对于大容量数据操作，输出可能非常大（50 MB以上）。使用120秒+超时。

---

##运行控制

# # #`resubmit_live_flow_run`回应:`{ flowKey, resubmitted: true, runName, triggerName }`# # #`cancel_live_flow_run`取消一个`Running`流运行。

>不要取消等待自适应卡响应的运行——status`Running`当Teams卡等待用户输入时，>是正常的。

---

HTTP触发工具

# # #`get_live_flow_http_schema`弃用。选择`get_live_flow`并检查`Request`触发器`inputs.schema`加上任何直接来自定义的`Response`动作。

响应按键:```
flowKey            - Flow GUID
displayName        - Flow display name
triggerName        - Trigger action name (e.g. "manual")
triggerType        - Trigger type (e.g. "Request")
triggerKind        - Trigger kind (e.g. "Http")
requestMethod      - HTTP method (e.g. "POST")
relativePath       - Relative path configured on the trigger (if any)
requestSchema      - JSON schema the trigger expects as POST body
requestHeaders     - Headers the trigger expects
responseSchemas    - Array of JSON schemas defined on Response action(s)
responseSchemaCount - Number of Response actions that define output schemas
```
>请求体模式的格式是`requestSchema`（而不是`triggerSchema`）。

# # #`get_live_flow_trigger_url`弃用。当需要调用http触发时，建议使用`trigger_live_flow`流;它在内部获取当前回调URL。

返回http触发流的签名回调URL。反应包括`flowKey`,`triggerName`,`triggerType`,`triggerKind`,`triggerMethod`,`triggerUrl`。

# # #`trigger_live_flow`响应键：`flowKey`、`triggerName`、`triggerUrl`、`requiresAadAuth`、`authType`、`responseStatus``responseBody`。b> **仅适用于`Request`（HTTP）触发器。**返回一个错误
>等触发类型：`"only HTTP Request triggers can be invoked via this tool"`。
>`Button`-kind触发器返回`ListCallbackUrlOperationBlocked`。
>
>`responseStatus`+`responseBody`包含流的响应动作输出。
>自动处理aad认证的触发器。
>
b> **内容类型备注**：正文以`application/octet-stream`（raw）格式发送，
>不是`application/json`。具有`required`字段的触发器模式的流
>将使用`InvalidRequestContent`（400）拒绝请求，因为PA进行了验证
>`Content-Type`，然后根据模式进行解析。没有模式的流，或者
>流被设计为接受原始输入（例如解析body的baker模式流）
b>内部)，将工作良好。流接收base64编码的JSON
>`$content`与`$content-type: application/octet-stream`。

---

##流状态管理

# # #`set_live_flow_state`通过实时PA API启动或停止Power automation流程。**不**要求吗
Power Clarity工作区——适用于任何模拟账户可以访问的流程。
首先读取当前状态，只有在发生更改时才发出start/stop调用
实际需要。

参数：`environmentName`、`flowName`、`state`（`"Started"`|`"Stopped"`）—必选参数。

回应:```json
{
  "flowName": "6321ab25-7eb0-42df-b977-e97d34bcb272",
  "environmentName": "Default-26e65220-...",
  "requestedState": "Started",
  "actualState": "Started"
}
```
> **使用此工具** -而不是`update_live_flow`-启动或停止流。
>`update_live_flow`只改变displayName/definition；PA API忽略
>状态通过该端点。

# # #`set_store_flow_state`通过活动的PA API启动或停止流，并将更新后的状态保存回来
到Power Clarity缓存。与`set_live_flow_state`相同的参数，但需要
Power Clarity工作区。

响应（与`set_live_flow_state`形状不同）：```json
{
  "flowKey": "<environmentId>.<flowId>",
  "requestedState": "Stopped",
  "currentState": "Stopped",
  "flow": { /* full gFlows record, same shape as get_store_flow */ }
}
```
当您只需要切换状态时，首选`set_live_flow_state`-它是
>更简单，没有订阅要求。
>
b>当需要立即更新缓存时，使用`set_store_flow_state`>（无需等待下一个每日扫描），并希望完全更新
>治理记录返回到相同的调用中——对于工作流有用
>停止流并立即标记或检查它。

---

##存储工具—FlowStudio仅供团队使用

# # #`get_store_flow_summary`响应：聚合的运行统计信息。```json
{
  "totalRuns": 100,
  "failRuns": 10,
  "failRate": 0.1,
  "averageDurationSeconds": 29.4,
  "maxDurationSeconds": 158.9,
  "firstFailRunRemediation": "<hint or null>"
}
```
# # #`get_store_flow_runs`缓存最近N天的运行历史记录，包括持续时间和修复提示。

# # #`get_store_flow_errors`缓存的仅包含失败操作名称和补救提示的失败运行。

# # #`get_store_flow_trigger_url`从缓存触发URL（即时，没有PA API调用）。

# # #`update_store_flow`更新治理元数据（描述、标记、监视器标志、通知规则、业务影响）。`list_store_makers`/`get_store_maker`Maker（公民开发者）发现和细节。

# # #`list_store_power_apps`列出缓存中的所有Power Apps canvas应用。

---

##行为笔记

通过实际API使用发现的非明显行为。这些都是
工具模式无法告诉您。# # #`get_live_flow_run_action_outputs`- **`actionName`是可选的**：省略获得顶级动作，提供获得一个
行动。对于foreach循环中的操作，命名操作可能返回多个
重复;使用`iterationIndex`固定到一个迭代。
-大容量数据操作的输出可以是50mb +——总是使用120s+超时。

# # #`update_live_flow`-所需字段可能因服务器版本而异；与`tool_search`确认
（`select:update_live_flow`）在create/update.之前如果需要`description`打补丁时保留现有的描述。
-`error`键**始终存在**响应——`null`表示成功。
不要检查`if "error" in result`；检查`result.get("error") is not None`。
—创建时，`created`=新流GUID （string）。在更新时，`created`=`false`。
- **不能更改流状态。**只更新displayName， definition和
connectionReferences。使用`set_live_flow_state`到start/stop一个流。# # #`trigger_live_flow`- **仅适用于HTTP请求触发器。**返回递归错误，连接器，
以及其他触发类型。
-自动处理aad认证触发器（冒充的承载令牌）。

# # #`get_live_flow_runs`-`top`默认为**30**，如果值越大，会自动分页。
—“运行ID”字段为`name`，而不是`runName`。在其他工具中使用该值作为`runName`。
—运行按最新优先返回。`PostMessageToConversation`（通过`update_live_flow`）
- **"Chat with Flow bot"**:`body/recipient`=`"user@domain.com;"`（带末尾分号的字符串）。
- **“通道”**:`body/recipient`=`{"groupId": "...", "channelId": "..."}`（对象）。
-`poster`:`"Flow bot"`用于工作流机器人标识，`"User"`用于用户标识。# # #`list_live_connections`-对于构建工作流，传递`environmentName`；省略它的库存
跨环境的连接。
-使用`search=<connector/account>`获得较小的输出和粘贴准备`connectionReferenceTemplate`/`hostTemplate`值。
—`id`是`connectionReferences`中`connectionName`所需的值。
—`connectorName`映射到apiId:`"/providers/Microsoft.PowerApps/apis/" + connectorName`。