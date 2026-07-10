---
name: flowstudio-power-automate-monitoring
description: >-
  Pro+ subscription required. Tenant-wide Power Automate monitoring using the
  FlowStudio MCP cached store: failure rates, run-health trends, maker/app
  inventory, inactive owners, and compliance/health reports. Use only for
  aggregated tenant views. For one environment, one flow, run control, or
  root-cause debugging, use flowstudio-power-automate-mcp, flowstudio-power-automate-debug, or the
  server monitor-flow bundle. Requires FlowStudio for Teams or MCP Pro+.
---
#电源自动监控与FlowStudio MCP

监视流运行状况、跟踪故障率并通过
FlowStudio MCP **缓存存储** -快速读取，没有PA API速率限制，和
丰富了治理元数据和补救提示。> **⚠️Pro+订阅需要。**这个技能调用`store_*`工具
>仅适用于FlowStudio团队或MCP Pro+订阅者。
>
> **如果用户没有Pro+访问：**第一个`store_*`工具调用
>将返回一个403/404错误。当这种情况发生时：
> 1。停止调用存储工具
> 2。告诉用户这个功能需要Pro+订阅
> 3。将它们链接到https://mcp.flowstudio.app/pricing> 4。如果他们的问题可以用实时工具(例如。列表涌进来
>一个环境”)，建议使用`flowstudio-power-automate-mcp`技能
>
发现：**通过`tool_search`而不是`tools/list`-加载工具模式
>调用`query: "select:list_store_flows,get_store_flow_summary"`>常用监控工具，或者用`query: "skill:governance"`加载全套
>(服务器的治理包也涵盖了大多数监视读取——这个技能
>和`flowstudio-power-automate-governance`共享底层工具族)。这个技能
>涵盖了响应形状、行为笔记和工作流模式等内容
>`tool_search`不能告诉你。如果本文档与真正的API不一致
>响应时，API获胜。---

##监控如何工作

Flow Studio每天为每个订阅者和缓存扫描Power automation API
结果。有两个层次：

- **所有流**获得元数据扫描：定义，连接，所有者，触发器
输入和汇总运行统计信息(`runPeriodTotal`、`runPeriodFailRate`、
等等)。环境、应用程序、连接和制造商也会被扫描。
- **监控流量** (`monitor: true`)额外获得每运行的细节：
包含状态、持续时间、失败操作名称和的单个运行记录
修复提示。这是`get_store_flow_runs`和`get_store_flow_summary`。

**数据新鲜度：**检查`get_store_flow`上的`scanned`字段以查看何时
上次扫描了一个流。如果过期，则扫描管道可能没有运行。

**启用监控：**通过`update_store_flow`或
Flow Studio for Teams应用程序
（[如何选择流](https://learn.flowstudio.app/teams-monitoring)）。**指定临界流：**使用`update_store_flow`和`critical=true`关于业务关键型流。这将启用治理技能的通知
规则管理，在关键流上自动配置故障警报。

---

# #工具

|工具|用途||---|---|
|`list_store_flows`|带故障率和监控过滤器|的列表流
|`get_store_flow`|完整缓存记录：运行状态，所有者，层，连接，定义(包括`triggerUrl`字段
|`get_store_flow_summary`|汇总运行统计：success/fail速率，avg/max持续时间|
|`get_store_flow_runs`|每次运行的历史记录，包括持续时间、状态、失败的操作、修复(过滤`status="Failed"`以获取仅错误视图
|`update_store_flow`|设置监视标志、通知规则、标签、治理元数据|
|`list_store_environments`|所有电源平台环境|
|`list_store_connections`|所有连接|
|`list_store_makers`|所有制作者（市民开发者）|
|`get_store_maker`|制造商详细信息：flow/app计数，许可证，帐户状态|
|`list_store_power_apps`|所有Power Apps canvas Apps |

对于start/stop，使用`monitor-flow`包中的`set_live_flow_state`> (`tool_search query: "select:set_live_flow_state"`) -缓存重新同步
b>下次扫描。前面的`set_store_flow_state`方便包装器是
>弃用。

---

## Store vs Live|问题|使用存储|使用Live ||---|---|---|
|有多少流量失败？|`list_store_flows`| - |
30天内的故障率是多少？|`get_store_flow_summary`| - |
|显示流|`get_store_flow_runs`（过滤器`status="Failed"`） | - |的错误历史
谁建立了这个流程？|`get_store_flow`→解析`owners`| - |
|阅读完整流定义|`get_store_flow`有它（JSON字符串）|`get_live_flow`（结构化）|
|检查动作inputs/outputs从运行| - |`get_live_flow_run_action_outputs`|
|重新提交运行失败| - |`resubmit_live_flow_run`|

>商店工具回答“发生了什么？”和“它有多健康？”
> Live工具回答“到底是哪里出了问题？”和“现在修复它”。

>如果`get_store_flow_runs`或`get_store_flow_summary`返回空结果，
>检查：(1)`monitor: true`在流上吗？(2)是`scanned`域
>最近?使用`get_store_flow`来验证两者。

---

##响应形状

# # #`list_store_flows`直接的数组。过滤器：`monitor`(bool),`rule_notify_onfail`(bool)，`rule_notify_onmissingdays`(bool)。```json
[
  {
    "id": "Default-<envGuid>.<flowGuid>",
    "displayName": "Stripe subscription updated",
    "state": "Started",
    "triggerType": "Request",
    "triggerUrl": "https://...",
    "tags": ["#operations", "#sensitive"],
    "environmentName": "Default-aaaaaaaa-...",
    "monitor": true,
    "runPeriodFailRate": 0.012,
    "runPeriodTotal": 82,
    "createdTime": "2025-06-24T01:20:53Z",
    "lastModifiedTime": "2025-06-24T03:51:03Z"
  }
]
```
>`id`格式：`Default-<envGuid>.<flowGuid>`。分割第一个`.`得到
>`environmentName`和`flowName`。
>
>`triggerUrl`、`tags`为可选参数。有些条目是稀疏的（只有`id`+）
>`monitor`) -跳过不包含`displayName`的条目。
>`list_store_flows`上的标签是自动从流的`description`中提取的
>字段（制造商标签，如`#operations`）。标签通过
>`update_store_flow(tags=...)`单独存储，仅在
>`get_store_flow`-它们不会出现在列表响应中。

# # #`get_store_flow`完整缓存记录。关键字段:

|类别|字段||---|---|
|`name`,`displayName`,`environmentName`,`state`,`triggerType`,`triggerKind`,`tier`,`sharingType`|
|运行stats |`runPeriodTotal`，`runPeriodFails`,`runPeriodSuccess`,`runPeriodFailRate`,`runPeriodSuccessRate`,`runPeriodDurationAverage`/`Max`/`Min`（毫秒），`runTotal`,`runFails`,`runFirst`,`runLast`,`runToday`|
|治理|`monitor`(bool),`rule_notify_onfail`(bool),`rule_notify_onmissingdays`(number),`rule_notify_email`(string),`log_notify_onfail`(ISO),`description`,`tags`|
|新鲜度|`scanned`(ISO),`nextScan`(ISO) |
|生命周期|`deleted`(bool),`deletedTime`(ISO
|`actions`,`connections`,`owners`,`complexity`,`definition`,`createdBy`,`security`,`triggers`,`referencedResources`，`runError`-都需要`json.loads()`来解析|

>持续时间字段（`runPeriodDurationAverage`,`Max`,`Min`）在
> * * * *毫秒。除以1000秒。
>
>`runError`以JSON字符串的形式包含最后一次运行错误。解析:
>`json.loads(record["runError"])`-当没有错误时返回`{}`。

# # #`get_store_flow_summary`在一个时间窗口内（默认：过去7天）汇总的统计信息。```json
{
  "flowKey": "Default-<envGuid>.<flowGuid>",
  "windowStart": null,
  "windowEnd": null,
  "totalRuns": 82,
  "successRuns": 81,
  "failRuns": 1,
  "successRate": 0.988,
  "failRate": 0.012,
  "averageDurationSeconds": 2.877,
  "maxDurationSeconds": 9.433,
  "firstFailRunRemediation": null,
  "firstFailRunUrl": null
}
```
>当窗口中不存在此流的运行数据时返回全零。
b>使用`startTime`和`endTime`（ISO 8601）参数更改窗口。

# # #`get_store_flow_runs`缓存运行记录的直接数组。参数：`startTime`，`endTime`，`status`(数组-传递`["Failed"]`为错误视图，`["Succeeded"]`，
或者全部省略)。

>当窗口中不存在运行数据时返回`[]`。

###触发URL

直接从`get_store_flow`（缓存）读取`triggerUrl`字段或`get_live_flow`(生活)。对于非http触发器，它是`null`。

###启动/停止流

使用`monitor-flow`服务器包中的`set_live_flow_state`。缓存
赶上下一个每日扫描；如果您需要更快地更新缓存，请致电`get_live_flow`状态改变后确认并让下一次扫描同步。

# # #`update_store_flow`更新治理元数据。只有提供的字段被更新（合并）。
返回完整更新的记录（与`get_store_flow`形状相同）。可设置字段：`monitor`(bool),`rule_notify_onfail`(bool)，`rule_notify_onmissingdays`（数字，0=禁用），`rule_notify_email`（逗号分隔），`description`,`tags`，`businessImpact`,`businessJustification`,`businessValue`，`ownerTeam`,`ownerBusinessUnit`,`supportGroup`,`supportEmail`，`critical`(bool),`tier`,`security`。

# # #`list_store_environments`直接的数组。```json
[
  {
    "id": "Default-aaaaaaaa-...",
    "displayName": "Flow Studio (default)",
    "sku": "Default",
    "type": "NotSpecified",
    "location": "australia",
    "isDefault": true,
    "isAdmin": true,
    "isManagedEnvironment": false,
    "createdTime": "2017-01-18T01:06:46Z"
  }
]
```
>`sku`取值：`Default`，`Production`,`Developer`,`Sandbox`,`Teams`。

# # #`list_store_connections`直接的数组。可以是非常大的（1500+项）。```json
[
  {
    "id": "<environmentId>.<connectionId>",
    "displayName": "user@contoso.com",
    "createdBy": "{\"id\":\"...\",\"displayName\":\"...\",\"email\":\"...\"}",
    "environmentName": "...",
    "statuses": "[{\"status\":\"Connected\"}]"
  }
]
```
>`createdBy`和`statuses`是**JSON字符串** -用`json.loads()`解析。

# # #`list_store_makers`直接的数组。```json
[
  {
    "id": "09dbe02f-...",
    "displayName": "Sample Maker",
    "mail": "maker@contoso.com",
    "deleted": false,
    "ownerFlowCount": 199,
    "ownerAppCount": 209,
    "userIsServicePrinciple": false
  }
]
```
>删除的maker有`deleted: true`，没有`displayName`/`mail`字段。

# # #`get_store_maker`全创记录。关键字段：`displayName`，`mail`,`userPrincipalName`，`ownerFlowCount`,`ownerAppCount`,`accountEnabled`,`deleted`,`country`，`firstFlow`,`firstFlowCreatedTime`,`lastFlowCreatedTime`，`firstPowerApp``lastPowerAppCreatedTime`,`licenses`（M365 sku的JSON字符串）。

# # #`list_store_power_apps`直接的数组。```json
[
  {
    "id": "<environmentId>.<appId>",
    "displayName": "My App",
    "environmentName": "...",
    "ownerId": "09dbe02f-...",
    "ownerName": "Catherine Han",
    "appType": "Canvas",
    "sharedUsersCount": 0,
    "createdTime": "2023-08-18T01:06:22Z",
    "lastModifiedTime": "2023-08-18T01:06:22Z",
    "lastPublishTime": "2023-08-18T01:06:22Z"
  }
]
```
---

##通用工作流

发现不健康的流量```
1. list_store_flows
2. Filter where runPeriodFailRate > 0.1 and runPeriodTotal >= 5
3. Sort by runPeriodFailRate descending
4. For each: get_store_flow for full detail
```
检查特定流的运行状况```
1. get_store_flow → check scanned (freshness), runPeriodFailRate, runPeriodTotal
2. get_store_flow_summary → aggregated stats with optional time window
3. get_store_flow_runs(status=["Failed"]) → per-run failure detail with remediation hints
4. If deeper diagnosis needed → switch to live tools:
   get_live_flow_runs → get_live_flow_run_action_outputs
```
###启用流监控```
1. update_store_flow with monitor=true
2. Optionally set rule_notify_onfail=true, rule_notify_email="user@domain.com"
3. Run data will appear after the next daily scan
```
每日健康检查```
1. list_store_flows
2. Flag flows with runPeriodFailRate > 0.2 and runPeriodTotal >= 3
3. Flag monitored flows with state="Stopped" (may indicate auto-suspension)
4. For critical failures → get_store_flow_runs(status=["Failed"]) for remediation hints
```
Maker审计```
1. list_store_makers
2. Identify deleted accounts still owning flows (deleted=true, ownerFlowCount > 0)
3. get_store_maker for full detail on specific users
```
# # #库存```
1. list_store_environments → environment count, SKUs, locations
2. list_store_flows → flow count by state, trigger type, fail rate
3. list_store_power_apps → app count, owners, sharing
4. list_store_connections → connection count per environment
```
---

相关技能

-`flowstudio-power-automate-mcp`-基础技能：连接设置，MCP助手，工具发现`flowstudio-power-automate-debug`-深度诊断与行动级inputs/outputs（live API）
-`flowstudio-power-automate-build`-构建和部署流定义
-`flowstudio-power-automate-governance`-治理元数据、标记、通知规则、CoE模式