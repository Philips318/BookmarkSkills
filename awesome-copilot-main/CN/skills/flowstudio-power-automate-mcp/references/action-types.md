# FlowStudio MCP -动作类型参考

用于识别`get_live_flow`返回的操作类型的紧凑查找。
使用它来阅读和理解现有的流定义。

>有关完整的复制-粘贴构造模式，请参阅`flowstudio-power-automate-build`技能。

---

如何阅读流定义

每个动作都有`"type"`、`"runAfter"`和`"inputs"`。`runAfter`对象
声明依赖关系：`{"Previous": ["Succeeded"]}`。有效的状态:`Succeeded`,`Failed`,`Skipped`,`TimedOut`。

---

动作类型快速参考

|类型|用途| |巡检关键字段输出参考||---|---|---|---|
|`Compose`|Store/transforma值|`inputs`（任意表达式）|`outputs('Name')`|
|`InitializeVariable`|声明一个变量|`inputs.variables[].{name, type, value}`|`variables('name')`|
|`SetVariable`|更新变量|`inputs.{name, value}`|`variables('name')`|
|`IncrementVariable`|增加一个数值变量|`inputs.{name, value}`|`variables('name')`|
|`AppendToArrayVariable`|推入数组变量|`inputs.{name, value}`|`variables('name')`|
|`If`|条件必选分支|`expression.and/or`、`actions`、`else.actions`| - |
|`Switch`|多路分支|`expression`，`cases.{case, actions}`,`default`| - |
|`Foreach`|循环数组|`foreach`，`actions`,`operationOptions`|`item()`/`items('Name')`|
|`Until`|循环直到条件|`expression`，`limit.{count, timeout}`,`actions`| - |
|`Wait`|延迟|`inputs.interval.{count, unit}`| - |
|`Scope`| Group / try-catch |`actions`（嵌套动作图）|`result('Name')`|
|`Terminate`|结束执行|`inputs.{runStatus, runError}`| - |
|`OpenApiConnection`|连接器呼叫（SP, Outlook, Teams…）|`inputs.host.{apiId, connectionName, operationId}`,`inputs.parameters`|`outputs('Name')?['body/...']`|
|`OpenApiConnectionWebhook`| Webhook wait (approval, adaptive cards) |同上|`body('Name')?['...']`|
|`Http`|外部HTTP调用|`inputs.{method, uri, headers, body}`|`outputs('Name')?['body']`|
|`Response`|返回HTTP调用者|`inputs.{statusCode, headers, body}`| - |
|`Query`|过滤阵列|`inputs.{from, where}`|`body('Name')`（过滤阵列）|
|`Select`|Reshape/project阵列|`inputs.{from, select}`|`body('Name')`（投影阵列）|
|`Table`|数组→CSV/HTMLstring |`inputs.{from, format, columns}`|`body('Name')`(string) |
|`ParseJson`|用模式|`inputs.{content, schema}`|`body('Name')?['field']`|解析JSON
|`Expression`|内置函数（如ConvertTimeZone） |`kind`,`inputs`|`body('Name')`|---

##连接器标识

当您看到`type: OpenApiConnection`时，识别来自`host.apiId`的连接器：

| apiId后缀|连接器||---|---|
|`shared_sharepointonline`| SharePoint |
|`shared_office365`| Outlook / Office 365 |
微软团队|
|`shared_approvals`|批准|
|`shared_office365users`| Office 365用户|
|`shared_flowmanagement`|流量管理|`operationId`告诉你具体的操作(例如`GetItems`，`SendEmailV2`，`PostMessageToConversation`)。`connectionName`映射到中的GUID`properties.connectionReferences`。

---

常用表达（阅读小抄）

|表达式|含义||---|---|
|`@outputs('X')?['body/value']`|连接器动作X |的数组结果
|`@body('X')`|操作X （Query, Select, ParseJson）的直接主体|
|`@item()?['Field']`|当前循环项的字段|
|`@triggerBody()?['Field']`|触发有效载荷域|
|`@variables('name')`|变量值|
|`@coalesce(a, b)`|第一个非空a， b |
|`@first(array)`|第一个元素（如果为空则为null） |
|`@length(array)`|阵列计数|
|`@empty(value)`|如果null/emptystring/empty数组|为True
|`@union(a, b)`|合并数组- **先赢**在重复|
|`@result('Scope')`|作用域内操作结果的数组|