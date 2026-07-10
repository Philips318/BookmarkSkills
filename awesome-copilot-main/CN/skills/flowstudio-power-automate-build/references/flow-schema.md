# FlowStudio MCP -流定义模式`update_live_flow`期望的完整JSON结构（并由`get_live_flow`返回）。

---

##顶层形状```json
{
  "$schema": "https://schema.management.azure.com/providers/Microsoft.Logic/schemas/2016-06-01/workflowdefinition.json#",
  "contentVersion": "1.0.0.0",
  "parameters": {
    "$connections": {
      "defaultValue": {},
      "type": "Object"
    }
  },
  "triggers": {
    "<TriggerName>": { ... }
  },
  "actions": {
    "<ActionName>": { ... }
  },
  "outputs": {}
}
```
---

# #`triggers`每个流定义恰好有一个触发器。键名是任意的，但是
使用常规名称（例如`Recurrence`，`manual`,`When_a_new_email_arrives`）。

请参阅[trigger-types.md]（trigger-types.md）了解所有触发器模板。

---

# #`actions`由唯一操作名称键控的操作定义字典。
键名不能包含空格-使用下划线。

每个行动必须包括：
-`type`-动作类型标识符
-`runAfter`-上游动作名称映射→状态条件数组
-`inputs`-特定于动作的输入配置

参见[action-patterns-core.md](action-patterns-core.md), [action-patterns-data.md](action-patterns-data.md)，
[action-patterns-connectors.md]（action-patterns-connectors.md）用于模板。

###可选动作属性

除了所需的`type`、`runAfter`和`inputs`之外，操作还可以包括：

|属性|用途||---|---|
|`runtimeConfiguration`|分页、并发、安全数据、块传输|
|`operationOptions`|`"Sequential"`for Foreach,`"DisableAsyncPattern"`for HTTP |
|`limit`|超时覆盖（例如`{"timeout": "PT2H"}`） |
|`metadata`|设计器元数据，如`operationMetadataId`|

####设计者元数据

对于现有的连接器操作，请保留`metadata.operationMetadataId`编辑定义。对于新的连接器动作或Skills/HTTP响应动作，
添加一个稳定的GUID，并在更新期间保持稳定。不重新生成这些id吗
在每次部署中；设计师和一些专用表面使用它们来保持运动
身份一致。

####`runtimeConfiguration`变体

**分页** （SharePoint Get项目与大列表）：```json
"runtimeConfiguration": {
  "paginationPolicy": {
    "minimumItemCount": 5000
  }
}
```
如果没有这个，Get Items会在256个结果处静默封顶。设置`minimumItemCount`>到您期望的最大行数。任何超过256项的SharePoint列表都需要。

**并发**（并行Foreach）：```json
"runtimeConfiguration": {
  "concurrency": {
    "repetitions": 20
  }
}
```
**安全inputs/outputs**（运行历史中的掩码值）：```json
"runtimeConfiguration": {
  "secureData": {
    "properties": ["inputs", "outputs"]
  }
}
```
>用于处理凭据、令牌或PII的操作。遮罩值显示
>作为流运行历史UI和API响应中的`"<redacted>"`。

**块传输**（大HTTP有效负载）：```json
"runtimeConfiguration": {
  "contentTransfer": {
    "transferMode": "Chunked"
  }
}
```
>启用HTTP动作发送或接收体> 100kb（例如parent→child）
>流调用与大数组)。

---

##`runAfter`规则

分支中的第一个动作是`"runAfter": {}`（空-触发后运行）。

随后的动作声明它们的依赖关系：```json
"My_Action": {
  "runAfter": {
    "Previous_Action": ["Succeeded"]
  }
}
```
多个上游依赖项：```json
"runAfter": {
  "Action_A": ["Succeeded"],
  "Action_B": ["Succeeded", "Skipped"]
}
```
错误处理操作（当上游失败时运行）：```json
"Log_Error": {
  "runAfter": {
    "Risky_Action": ["Failed"]
  }
}
```
---

##`parameters`（流量输入参数）

可选的。在流程级别定义可重用的值：```json
"parameters": {
  "listName": {
    "type": "string",
    "defaultValue": "MyList"
  },
  "maxItems": {
    "type": "integer",
    "defaultValue": 100
  }
}
```
参考：表达式字符串中的`@parameters('listName')`。

---

# #`outputs`很少用于云流。保留为`{}`，除非调用流
作为子流，需要返回值。

对于返回数据的子流：```json
"outputs": {
  "resultData": {
    "type": "object",
    "value": "@outputs('Compose_Result')"
  }
}
```
---

作用域操作（在作用域块内）

为了错误处理或澄清而需要分组的操作：```json
"Scope_Main_Process": {
  "type": "Scope",
  "runAfter": {},
  "actions": {
    "Step_One": { ... },
    "Step_Two": { "runAfter": { "Step_One": ["Succeeded"] }, ... }
  }
}
```
---

##完整的最小示例```json
{
  "$schema": "https://schema.management.azure.com/providers/Microsoft.Logic/schemas/2016-06-01/workflowdefinition.json#",
  "contentVersion": "1.0.0.0",
  "triggers": {
    "Recurrence": {
      "type": "Recurrence",
      "recurrence": {
        "frequency": "Week",
        "interval": 1,
        "schedule": { "weekDays": ["Monday"] },
        "startTime": "2026-01-05T09:00:00Z",
        "timeZone": "AUS Eastern Standard Time"
      }
    }
  },
  "actions": {
    "Compose_Greeting": {
      "type": "Compose",
      "runAfter": {},
      "inputs": "Good Monday!"
    }
  },
  "outputs": {}
}
```
