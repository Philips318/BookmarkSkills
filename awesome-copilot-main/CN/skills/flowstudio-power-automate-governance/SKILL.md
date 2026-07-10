---
name: flowstudio-power-automate-governance
description: >-
  Govern Power Automate flows and Power Apps at scale using the FlowStudio MCP
  cached store. Classify flows by business impact, detect orphaned resources,
  audit connector usage, enforce compliance standards, manage notification rules,
  and compute governance scores — all without Dataverse or the CoE Starter Kit.
  Load this skill when asked to: tag or classify flows, set business impact,
  assign ownership, detect orphans, audit connectors, check compliance, compute
  archive scores, manage notification rules, run a governance review, generate
  a compliance report, offboard a maker, or any task that involves writing
  governance metadata to flows. Requires a FlowStudio for Teams or MCP Pro+
  subscription — see https://mcp.flowstudio.app
---
#动力自动化治理与FlowStudio MCP

通过FlowStudio对Power automation流进行分类、标记和管理
MCP **缓存存储** -没有Dataverse，没有CoE Starter Kit，和
没有电力自动化门户。

该技能使用与`flowstudio-power-automate-monitoring`相同的`store_*`工具族，
但是有一个不同的*意图*：治理写元数据（`update_store_flow`）
并读取*审计和分类*结果。监控读取相同
用于*运行状况*结果的工具。不要试图记住哪种技能
“拥有”哪个工具——根据用户正在做的事情来选择。进行健康检查和
故障率仪表板，加载`flowstudio-power-automate-monitoring`代替。> **⚠️Pro+订阅需要。**此技能调用`store_*`工具
>仅适用于FlowStudio团队或MCP Pro+订阅者。
>
> **如果用户没有Pro+访问：**第一个`store_*`工具调用
>将返回一个403/404错误。当这种情况发生时：
> 1。停止调用存储工具
> 2。告诉用户治理功能需要Pro+订阅
> 3。将它们链接到https://mcp.flowstudio.app/pricing>
b> **发现：**通过元工具而不是`tools/list`-加载工具模式
>调用`tool_search`与`query: "skill:governance"`为规范包，
>或`query: "select:update_store_flow"`用于单个工具。这个技能包括
>工作流模式和字段语义——`tool_search`无法告诉你的事情。
>如果此文档与真正的API响应不一致，则API获胜。

---

关键：如何提取流量id`list_store_flows`以`<environmentId>.<flowId>`格式返回`id`。你必须离开
在第一个`.`**上获得所有其他工具的`environmentName`和`flowName`：```
id = "Default-<envGuid>.<flowGuid>"
environmentName = "Default-<envGuid>"    (everything before first ".")
flowName = "<flowGuid>"                  (everything after first ".")
```
另外：跳过没有`displayName`或有`state=Deleted`-的条目
这些是稀疏的记录或流，在Power automation中不再存在。
如果已删除的流具有`monitor=true`，建议禁用监控
（`update_store_flow`with`monitor=false`）来释放一个监视槽
（标准计划包括20个）。

---

写工具：`update_store_flow``update_store_flow`将治理元数据写入**Flow Studio缓存
只有** -它不修改流在电力自动化。这些字段是
不通过`get_live_flow`或PA门户可见。它们只存在于
Flow Studio存储和使用Flow Studio的扫描管道和
通知规则。这意味着:
-`ownerTeam`/`supportEmail`-设置Flow Studio考虑的
治理联系。不会更改实际的PA流所有者。
-`rule_notify_email`-设置谁接收流工作室failure/missing-run通知。不改变微软的内置流故障警报。
-`monitor`/`critical`/`businessImpact`- Flow Studio分类
只有。Power automation没有相应的字段。

合并语义-仅更新您提供的字段。返回完整的
更新的记录（与`get_store_flow`形状相同）。

所需参数：`environmentName`、`flowName`。所有其他字段都是可选的。

###可设置字段

|字段|类型|用途||---|---|---|
|`monitor`| bool |启用运行级扫描（标准计划：包含20个流）|
|`rule_notify_onfail`| bool |当|运行失败时，发送邮件通知
|`rule_notify_onmissingdays`| number | N天未运行流量时发送通知（0 = disabled） |
|`rule_notify_email`| string |逗号分隔的通知接收者|
|`description`| string |流做什么|
|`tags`|字符串|分类标签（也自动从描述`#hashtags`中提取）|
|`businessImpact`| string |低/中/高/紧急|
|`businessJustification`| string |为什么流存在，它自动化了什么过程|
|`businessValue`| string |业务价值语句|
|`ownerTeam`| string | Accountable team |
|`ownerBusinessUnit`| string |业务单位|
|`supportGroup`| string |支持升级组|
|`supportEmail`| string |支持联系邮箱|
|`critical`| bool |指定为业务关键|
|`tier`| string |标准或高级|
|`security`| string |安全分类或注释|**`security`:**`get_store_flow`上的`security`字段
>包含结构化JSON（例如`{"triggerRequestAuthenticationType":"All"}`）。
>编写像`"reviewed"`这样的普通字符串将覆盖此内容。标记a
>流作为安全审查，使用`tags`代替。

---

治理工作流

# # # 1。合规性详细审查

识别缺少所需治理元数据的流。```
1. Ask the user which compliance fields they require
2. list_store_flows
3. For each active flow: split id, call get_store_flow, check required fields
4. Report non-compliant flows with missing fields listed
5. For updates: ask for values, then update_store_flow(...provided fields)
```
常见合规字段：`description`，`businessImpact`，`businessJustification`,`ownerTeam`,`supportEmail`,`monitor``rule_notify_onfail``critical`。在标记之前询问用户的策略。

# # # 2。孤立资源检测

查找已删除或禁用Azure AD帐户拥有的流。```
1. list_store_makers
2. Filter where deleted=true AND ownerFlowCount > 0
3. list_store_flows → collect all flows
4. For each active flow: split id, get_store_flow, parse owners JSON
5. Match owner principalId against orphaned maker id
6. Reassign governance contact or stop/tag for decommission
```
`update_store_flow`不转移实际的PA所有权；使用管理中心
或者PowerShell。有些看起来孤立的流是系统生成的；标签
而不是在适当的时候重新分配。商店的覆盖范围只有新鲜的
最近一次扫描。

# # # 3。档案分数计算

计算每个流的不活动得分（0-7），以确定清理候选。```
1. list_store_flows
2. For each active flow: split id, get_store_flow
3. Add 1 point each: created≈modified, test/demo/temp/copy name, age >12mo,
   stopped/suspended, no owners, no recent runs, complexity.actions < 5
4. Score 5-7: recommend archive; 3-4: tag #archive-review; 0-2: active
5. For confirmed archive: set_live_flow_state(..., "Stopped") and append #archived
```
通过MCP存档意味着停止流并标记它。删除需要门户或
管理PowerShell。

# # # 4。连接器的审计

审计在被监视的流中使用了哪些连接器。对DLP有用
影响分析和高级许可证规划。```
1. list_store_flows(monitor=true)
2. For each active flow: split id, get_store_flow, parse connections JSON
3. Group by apiName; flag Premium tier, HTTP connectors, custom connectors
4. Report inventory to user
```
在可能的情况下，监测流程的范围；每个`get_store_flow`调用都需要花费时间。`list_store_connections`列出连接实例，而不是每个连接器的使用情况
流。DLP政策不公开；向用户询问连接器分类。

# # # 5。通知规则管理

为大规模的流配置监视和警报。```
Enable failure alerts on all critical flows:
1. list_store_flows(monitor=true)
2. For each active flow: split id, get_store_flow
3. If critical=true and rule_notify_onfail is false, update_store_flow(...,
   rule_notify_onfail=true, rule_notify_email="oncall@contoso.com")

Enable missing-run detection for scheduled flows:
1. list_store_flows(monitor=true)
2. For active Recurrence flows: get_store_flow
3. If rule_notify_onmissingdays is 0/missing, update_store_flow(...,
   rule_notify_onmissingdays=2)
```
在启用大容量`monitor=true`之前检查监控限制。如果没有流动`critical=true`，在配置警报之前将其报告为治理缺口。

# # # 6。分类和标记

按连接器类型、业务功能或风险级别对流进行批量分类。```
Auto-tag by connector:
1. list_store_flows
2. For each active flow: split id, get_store_flow, parse connections JSON
3. Map apiName values to tags (#sharepoint, #teams, #email, #custom-connector)
4. Read existing store tags, append new tags, update_store_flow(tags=...)
```
存储标签和描述标签是独立的系统。`tags=`覆盖
避免重写计算的`tier`，除非被要求。

# # # 7。制造商Offboarding

当员工离开时，确定他们的流程和应用程序，并重新分配
Flow Studio管理联系人和通知接收者。```
1. get_store_maker(makerKey="<departing-user-aad-oid>")
   → check ownerFlowCount, ownerAppCount, deleted status
2. list_store_flows → collect all flows
3. For each active flow: split id, get_store_flow, parse owners JSON
4. Flag flows whose owner principalId matches the departing user's OID
5. list_store_power_apps → filter ownerId
6. For kept flows: update ownerTeam/supportEmail/rule_notify_email; consider
   add_live_flow_to_solution before account deletion
7. For retired flows: set_live_flow_state(..., "Stopped") and tag #decommissioned
8. Report: flows reassigned, flows migrated to solutions, flows stopped,
   apps needing manual reassignment
```
这改变了Flow Studio治理联系，而不是实际的PA所有权。权力
应用程序所有权的变化是manual/admin-center工作。

# # # 8。安全审查

使用缓存存储数据检查流是否存在潜在的安全问题。```
1. list_store_flows(monitor=true)
2. For each active flow: split id, get_store_flow
3. Parse security/connections/referencedResources JSON; read sharingType top-level
4. Report findings; for reviewed flows append #security-reviewed tag
```
安全信号：`security.triggerRequestAuthenticationType`，`sharingType`，`connections`,`referencedResources`,`tier`。永远不要覆盖结构化的`security`领域;而是标记审查流。

# # # 9。环境治理

审核环境的合规性和扩展性。```
1. list_store_environments
   Skip entries without displayName (tenant-level metadata rows)
2. Flag:
   - Developer environments
   - Non-managed environments
   - Environments where service account lacks admin access (isAdmin=false)
3. list_store_flows → group by environmentName
4. list_store_connections → group by environmentName
```
# # # 10。治理仪表板

生成租户范围的治理摘要。```
Efficient metrics (list calls only):
1. total_flows = len(list_store_flows())
2. monitored = len(list_store_flows(monitor=true))
3. with_onfail = len(list_store_flows(rule_notify_onfail=true))
4. makers/apps/envs/conns = list_store_makers/list_store_power_apps/list_store_environments/list_store_connections
5. Compute monitoring %, notification %, orphan count, high-failure count

Detailed metrics (require get_store_flow per flow — expensive for large tenants):
- Compliance %: flows with businessImpact set / total active flows
- Undocumented count: flows without description
- Tier breakdown: group by tier field
```
---

##字段参考：`get_store_flow`治理中使用的字段

下面的所有字段都在`get_store_flow`响应中得到确认。
标记为`*`的字段也可以在`list_store_flows`上使用（更便宜）。

|字段|类型|治理使用||---|---|---|
|`displayName`* | string |归档评分（test/demo名称检测）|
|`state`* | string |存档评分，生命周期管理|
|`tier`| string | License审计（Standard vs Premium） |
|`monitor`* | bool |是否正在主动监控该流？|
|`critical`| bool |业务关键指定（可通过update_store_flow设置）|
|`businessImpact`| string |合规分类|
|`businessJustification`| string |合规认证|
|`ownerTeam`| string |所有权问责|
|`supportEmail`| string |升级联系人|
|`rule_notify_onfail`| bool |配置失败告警功能？|
|`rule_notify_onmissingdays`| number | SLA监控配置？|
|`rule_notify_email`| string |提醒收件人|
|`description`| string |文档完整性|
|`tags`| string |分类-`list_store_flows`只显示描述提取的标签；由`update_store_flow`编写的存储标记要求`get_store_flow`回读|
|`runPeriodTotal`* |号码|活动级别l |
|`runPeriodFailRate`* |编号|健康状态|
|`runLast`| ISO字符串|上次运行的时间戳|
|`scanned`| ISO字符串|数据新鲜度|
|`deleted`| bool |生命周期跟踪|
|`createdTime`* | ISO string | Archive score (age) |
|`lastModifiedTime`* | ISO字符串|存档评分（过期）|
|`owners`| JSON字符串|孤儿检测，所有权审计-使用JSON .loads（）解析|
|`connections`| JSON string | Connector audit, tier - parse with JSON .loads() |
|`complexity`| JSON字符串|存档分数（简单）-解析JSON .loads() |
|`security`| JSON字符串|认证类型audit - parse with JSON。加载（），包含`triggerRequestAuthenticationType`|
|`sharingType`| string |过度共享检测（顶层，非内部安全）|
|`referencedResources`| JSON字符串| URL审计-使用JSON .loads（）解析|---

相关技能

-`flowstudio-power-automate-monitoring`-运行状况检查、故障率、库存（只读）
-基础技能：连接设置，MCP助手，工具发现
-`flowstudio-power-automate-debug`-动作级深度诊断inputs/outputs-`flowstudio-power-automate-build`-构建和部署流定义