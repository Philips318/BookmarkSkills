# FlowStudio MCP -常见的电源自动化错误

调试时的错误代码、可能原因和建议修复的参考
通过FlowStudio MCP服务器实现自动化流。

---

表达式/模板错误

###`InvalidTemplate`-函数应用于Null

**完整消息模式**:`"Unable to process template language expressions... function 'split' expects its first argument 'text' to be of type string"`**根本原因**：表达式`@split(item()?['Name'], ' ')`收到空值。

* * * *诊断:
1. 注意错误消息中的操作名称
2. 对生成数组的操作调用`get_live_flow_run_action_outputs`3. 查找`Name`（或引用字段）为`null`的项

* *修复* *:```
Before: @split(item()?['Name'], ' ')
After:  @split(coalesce(item()?['Name'], ''), ' ')

Or guard the whole foreach body with a condition:
  expression: "@not(empty(item()?['Name']))"
```
---

###`InvalidTemplate`-表达式路径错误

**完整消息模式**:`"Unable to process template language expressions... 'triggerBody()?['FieldName']' is of type 'Null'"`**根本原因**：表达式中的字段名与实际的有效负载模式不匹配。

* * * *诊断:```python
# Check trigger output shape
mcp("get_live_flow_run_action_outputs",
    environmentName=ENV, flowName=FLOW_ID, runName=RUN_ID,
    actionName="<trigger-name>")
# Compare actual keys vs expression
```
**修正**：更新表达式使用正确的键名。常见的不匹配:
-`triggerBody()?['body']`vs`triggerBody()?['Body']`（区分大小写）
-`triggerBody()?['Subject']`vs`triggerOutputs()?['body/Subject']`---

###`InvalidTemplate`-类型不匹配

**完整消息模式**:`"... expected type 'Array' but got type 'Object'"`**根本原因**：传递一个对象，其中表达式期望一个数组（例如，单项HTTP响应与列表响应）。

* * * *:```
Before: @outputs('HTTP')?['body']
After:  @outputs('HTTP')?['body/value']    ← for OData list responses
        @createArray(outputs('HTTP')?['body'])  ← wrap single object in array
```
---

##连接/认证错误

# # #`ConnectionAuthorizationFailed`**完整消息**:`"The API connection ... is not authorized."`**根本原因**：流中引用的连接由不同的用户拥有user/service帐户，而不是使用JWT的帐户。

**诊断：检查`properties.connectionReferences`-`connectionName`GUID
标识所有者。无法通过API修复。

* * * *修复选择:
1. 打开Power automation设计器中的流程→重新验证连接
2. 使用您所持有令牌的服务帐户所拥有的连接
3. 与PA admin中的业务帐号共享连接

---

# # #`InvalidConnectionCredentials`**根本原因**：连接的底层OAuth令牌已过期或
用户的凭据已更改。

**修复**：所有者必须登录到Power automation并刷新连接。

---

## HTTP操作错误

###`ActionFailed`- HTTP4xx/5xx**完整消息模式**:`"An HTTP request to... failed with status code '400'"`* * * *诊断:```python
actions_out = mcp("get_live_flow_run_action_outputs", ..., actionName="HTTP_My_Call")
item = actions_out[0]   # first entry in the returned array
print(item["outputs"]["statusCode"])   # 400, 401, 403, 500...
print(item["outputs"]["body"])         # error details from target API
```
* * * *常见原因:
- 401 -缺少或过期的验证头
- 403 -拒绝对目标资源的访问权限
- 404 -错误的URL /资源被删除
- 400 -格式错误的JSON主体（检查构建主体的表达式）

---

###`ActionFailed`- HTTP超时

**根本原因**：目标端点在连接器超时时间内没有响应
（HTTP操作默认90秒）。

**修复**：为HTTP动作添加重试策略，或将有效载荷分割成较小的
批处理以减少每个请求的处理时间。

---

控制流错误

###`ActionSkipped`而不是运行

**根本原因**：不满足`runAfter`条件。例如，操作设置为
如果`Prev`失败或被跳过，`runAfter: { "Prev": ["Succeeded"] }`将不会运行。

**诊断**：检查上述操作的状态。故意跳过
（例如在假分支中）是有意的——意外的跳过是逻辑间隙。**修复**：添加`"Failed"`或`"Skipped"`到`runAfter`状态数组
行动也应该以这些结果为基础。

---

### Foreach运行在错误的顺序/竞争条件

**根本原因**:`Foreach`没有运行`operationOptions: "Sequential"`并行迭代，导致写冲突或未定义的顺序。

**修正**：添加`"operationOptions": "Sequential"`到Foreach动作。

---

在处理内部失败后，Foreach父失败

**症状**：内部操作有失败处理程序，但父操作`Foreach`仍然存在
显示`Failed`，并跳过`Response`等下游操作。

**根本原因**：处理过的子失败仍然可以将循环容器标记为
失败了。只接受`Succeeded`的下游`runAfter`将不会运行。

**诊断**：用`get_live_flow_run_error`检查父级，然后
检查失败迭代的子操作输出。**修复**：如果部分成功是可以接受的，允许下游join/response在`Succeeded`和`Failed`之后运行，并在
有效载荷。如果循环必须是全有或全无，则将有风险的内部工作包装在Scope和
在作用域边界处理success/failure。

---

##更新/部署错误

###`update_live_flow`返回No-Op

**现象**:`result["updated"]`列表为空或`result["created"]`列表为空。

**可能原因**：参数名称错误。所需的键是`definition`（对象），而不是`flowDefinition`或`body`。

---

###`update_live_flow`-`"Supply connectionReferences"`**根本原因**：定义中包含`OpenApiConnection`或`OpenApiConnectionWebhook`动作，但`connectionReferences`未通过。

**修复**：通过`get_live_flow`获取现有的连接引用并传递
作为`connectionReferences`参数。

---

##数据逻辑错误

###`union()`用null重写正确的记录**现象**：合并两个数组后，部分记录存在空字段
在其中一个源数组中。

**根本原因**:`union(old_data, new_data)`-`union()`优先，所以old_data
值覆盖new_data来匹配记录。

**修复**：交换参数顺序：`union(new_data, old_data)````
Before: @sort(union(outputs('Old_Array'), body('New_Array')), 'Date')
After:  @sort(union(body('New_Array'), outputs('Old_Array')), 'Date')
```
---

### Filter Array / Query中的Null级联

**现象**:lookup/filter步骤返回错误的记录或更高的表达式
在null时失败，即使过滤器动作本身成功。

**根本原因**：查找键为空或空。一种情况是`equals(item()?['Email'], outputs('Lookup_Email'))`可能会意外地匹配行
其中两边都为空，或者可以向下游传递空数组。

**诊断**：检查创建查找键和过滤器的操作
输出长度。在信任筛选结果之前，请确认密钥非空。

**修复**：在过滤器前添加非空保护，使比较值正常化
使用`trim()`/`toLower()`，当没有找到匹配时显式地进行分支。