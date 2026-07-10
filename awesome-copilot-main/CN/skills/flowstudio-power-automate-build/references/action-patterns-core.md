# FlowStudio MCP -动作模式：核心

Power automation流定义的变量、控制流和表达式模式。

>所有示例都假设`"runAfter"`设置适当。
>将`<connectionName>`替换为`connectionReferences`映射中使用的**键**
>（例如`shared_teams`，`shared_office365`） -不是连接GUID。

---

##数据和变量

###撰写（存储一个值）```json
"Compose_My_Value": {
  "type": "Compose",
  "runAfter": {},
  "inputs": "@variables('myVar')"
}
```
参考:`@outputs('Compose_My_Value')`---

###初始化变量```json
"Init_Counter": {
  "type": "InitializeVariable",
  "runAfter": {},
  "inputs": {
    "variables": [{
      "name": "counter",
      "type": "Integer",
      "value": 0
    }]
  }
}
```
类型：`"Integer"`，`"Float"`,`"Boolean"`,`"String"`,`"Array"`,`"Object"`---

###设置变量```json
"Set_Counter": {
  "type": "SetVariable",
  "runAfter": {},
  "inputs": {
    "name": "counter",
    "value": "@add(variables('counter'), 1)"
  }
}
```
---

附加到数组变量```json
"Collect_Item": {
  "type": "AppendToArrayVariable",
  "runAfter": {},
  "inputs": {
    "name": "resultArray",
    "value": "@item()"
  }
}
```
---

###增量变量```json
"Increment_Counter": {
  "type": "IncrementVariable",
  "runAfter": {},
  "inputs": {
    "name": "counter",
    "value": 1
  }
}
```
>使用`IncrementVariable`（不是`SetVariable`和`add()`）作为循环内的计数器
>中，它是原子的，可以避免在变量的其他地方使用表达式错误
>相同的迭代。`value`可以是任何整数或表达式，例如`@mul(item()?['Interval'], 60)`>将Unix时间戳游标前进N分钟。

---

##控制流

###条件（If/Else）```json
"Check_Status": {
  "type": "If",
  "runAfter": {},
  "expression": {
    "and": [{ "equals": ["@item()?['Status']", "Active"] }]
  },
  "actions": {
    "Handle_Active": {
      "type": "Compose",
      "runAfter": {},
      "inputs": "Active user: @{item()?['Name']}"
    }
  },
  "else": {
    "actions": {
      "Handle_Inactive": {
        "type": "Compose",
        "runAfter": {},
        "inputs": "Inactive user"
      }
    }
  }
}
```
比较运算符：`equals`、`not`、`greater`、`greaterOrEquals`、`less`、`lessOrEquals`、`contains`逻辑：`and: [...]`，`or: [...]`---

# # #开关```json
"Route_By_Type": {
  "type": "Switch",
  "runAfter": {},
  "expression": "@triggerBody()?['type']",
  "cases": {
    "Case_Email": {
      "case": "email",
      "actions": { "Process_Email": { "type": "Compose", "runAfter": {}, "inputs": "email" } }
    },
    "Case_Teams": {
      "case": "teams",
      "actions": { "Process_Teams": { "type": "Compose", "runAfter": {}, "inputs": "teams" } }
    }
  },
  "default": {
    "actions": { "Unknown_Type": { "type": "Compose", "runAfter": {}, "inputs": "unknown" } }
  }
}
```
---

### Scope（分组/ Try-Catch）

将相关操作包装在作用域中以给它们一个共享名称，将它们折叠到
最重要的是，将它们的错误作为一个整体来处理。```json
"Scope_Get_Customer": {
  "type": "Scope",
  "runAfter": {},
  "actions": {
    "HTTP_Get_Customer": {
      "type": "Http",
      "runAfter": {},
      "inputs": {
        "method": "GET",
        "uri": "https://api.example.com/customers/@{variables('customerId')}"
      }
    },
    "Compose_Email": {
      "type": "Compose",
      "runAfter": { "HTTP_Get_Customer": ["Succeeded"] },
      "inputs": "@outputs('HTTP_Get_Customer')?['body/email']"
    }
  }
},
"Handle_Scope_Error": {
  "type": "Compose",
  "runAfter": { "Scope_Get_Customer": ["Failed", "TimedOut"] },
  "inputs": "Scope failed: @{result('Scope_Get_Customer')?[0]?['error']?['message']}"
}
```
>引用范围结果：`@result('Scope_Get_Customer')`返回一个操作数组
>的结果。在后续操作中使用`runAfter: {"MyScope": ["Failed", "TimedOut"]}`>创建不带Terminate的try/catch语义。

---

### Foreach （Sequential）```json
"Process_Each_Item": {
  "type": "Foreach",
  "runAfter": {},
  "foreach": "@outputs('Get_Items')?['body/value']",
  "operationOptions": "Sequential",
  "actions": {
    "Handle_Item": {
      "type": "Compose",
      "runAfter": {},
      "inputs": "@item()?['Title']"
    }
  }
}
```
>总是包含`"operationOptions": "Sequential"`，除非并行是有意的。

---

### Foreach （Parallel with Concurrency Limit）```json
"Process_Each_Item_Parallel": {
  "type": "Foreach",
  "runAfter": {},
  "foreach": "@body('Get_SP_Items')?['value']",
  "runtimeConfiguration": {
    "concurrency": {
      "repetitions": 20
    }
  },
  "actions": {
    "HTTP_Upsert": {
      "type": "Http",
      "runAfter": {},
      "inputs": {
        "method": "POST",
        "uri": "https://api.example.com/contacts/@{item()?['Email']}"
      }
    }
  }
}
```
>设置`repetitions`来控制同时处理的项数。
>实用值：`5–10`用于外部API调用（尊重速率限制），
>`20–50`用于internal/fast操作。
>将`runtimeConfiguration.concurrency`完全省略为平台默认值
>（目前为50）。不要同时使用`"operationOptions": "Sequential"`和并发。

---

###等待（延迟）```json
"Delay_10_Minutes": {
  "type": "Wait",
  "runAfter": {},
  "inputs": {
    "interval": {
      "count": 10,
      "unit": "Minute"
    }
  }
}
```
有效的`unit`值：`"Second"`、`"Minute"`、`"Hour"`、`"Day"`>使用Delay + refetch作为重复数据删除保护：等待任何竞争进程
>完成，然后在操作前重新读取记录。这避免了重复处理
>，当多个触发器或手动编辑可以在同一项上竞争时。

---

终止（成功或失败）```json
"Terminate_Success": {
  "type": "Terminate",
  "runAfter": {},
  "inputs": {
    "runStatus": "Succeeded"
  }
},
"Terminate_Failure": {
  "type": "Terminate",
  "runAfter": { "Risky_Action": ["Failed"] },
  "inputs": {
    "runStatus": "Failed",
    "runError": {
      "code": "StepFailed",
      "message": "@{outputs('Get_Error_Message')}"
    }
  }
}
```
---

### Do Until（循环）

重复一个动作块，直到退出条件为真。
在预先不知道迭代次数的情况下使用(例如对API进行分页，
遍历一个时间范围，轮询直到状态改变)。```json
"Do_Until_Done": {
  "type": "Until",
  "runAfter": {},
  "expression": "@greaterOrEquals(variables('cursor'), variables('endValue'))",
  "limit": {
    "count": 5000,
    "timeout": "PT5H"
  },
  "actions": {
    "Do_Work": {
      "type": "Compose",
      "runAfter": {},
      "inputs": "@variables('cursor')"
    },
    "Advance_Cursor": {
      "type": "IncrementVariable",
      "runAfter": { "Do_Work": ["Succeeded"] },
      "inputs": {
        "name": "cursor",
        "value": 1
      }
    }
  }
}
```
>始终显式设置`limit.count`和`limit.timeout`-平台默认值为
>低（60次迭代，1小时）。对于时间范围行走器，使用`limit.count: 5000`和
>`limit.timeout: "PT5H"`（ISO 8601持续时间）。
>
>在每次迭代之前计算退出条件。初始化游标
>变量，以便在第一次循环时可以正确地计算条件。

---

代理重试循环

当流调用AI或copilot类型的代理时，直到它到达终端
结果，保持循环状态显式：—初始化`agentStatus`、`attempt`、`finalPayload`等变量
在`Until`之前。
—在循环内部，调用代理，验证响应，更新状态，以及delay/retry仅当状态为非终端时使用。
-放置最终调度操作，如电子邮件，SharePoint更新或团队发布
循环后，重试不会重复副作用。
如果平台拒绝在`Until`中嵌套复杂的`Switch`，则保留
循环体进行简单的验证和状态更新，然后用`Switch`路由
在循环之后。

---

使用requesttid关联进行异步轮询

当API异步启动长时间运行的作业时（例如，Power BI数据集刷新），
报告生成，批量导出)，触发器调用返回请求ID。捕捉它
从**响应头**，然后轮询状态端点过滤的确切ID：```json
"Start_Job": {
  "type": "Http",
  "inputs": { "method": "POST", "uri": "https://api.example.com/jobs" }
},
"Capture_Request_ID": {
  "type": "Compose",
  "runAfter": { "Start_Job": ["Succeeded"] },
  "inputs": "@outputs('Start_Job')?['headers/X-Request-Id']"
},
"Initialize_Status": {
  "type": "InitializeVariable",
  "inputs": { "variables": [{ "name": "jobStatus", "type": "String", "value": "Running" }] }
},
"Poll_Until_Done": {
  "type": "Until",
  "expression": "@not(equals(variables('jobStatus'), 'Running'))",
  "limit": { "count": 60, "timeout": "PT30M" },
  "actions": {
    "Delay": { "type": "Wait", "inputs": { "interval": { "count": 20, "unit": "Second" } } },
    "Get_History": {
      "type": "Http",
      "runAfter": { "Delay": ["Succeeded"] },
      "inputs": { "method": "GET", "uri": "https://api.example.com/jobs/history" }
    },
    "Filter_This_Job": {
      "type": "Query",
      "runAfter": { "Get_History": ["Succeeded"] },
      "inputs": {
        "from": "@outputs('Get_History')?['body/items']",
        "where": "@equals(item()?['requestId'], outputs('Capture_Request_ID'))"
      }
    },
    "Set_Status": {
      "type": "SetVariable",
      "runAfter": { "Filter_This_Job": ["Succeeded"] },
      "inputs": {
        "name": "jobStatus",
        "value": "@first(body('Filter_This_Job'))?['status']"
      }
    }
  }
},
"Handle_Failure": {
  "type": "If",
  "runAfter": { "Poll_Until_Done": ["Succeeded"] },
  "expression": { "equals": ["@variables('jobStatus')", "Failed"] },
  "actions": { "Terminate_Failed": { "type": "Terminate", "inputs": { "runStatus": "Failed" } } },
  "else": { "actions": {} }
}
```
访问响应报头：`@outputs('Start_Job')?['headers/X-Request-Id']`b> **状态变量初始化**：设置之前的前哨值（`"Running"`,`"Unknown"`）
>循环。退出条件测试除哨兵之外的任何值。
这样，空的轮询结果（作业尚未在历史记录中）将保持变量不变
>和循环继续-它不会意外地在null时退出。
>
b> **过滤前提取**：总是`Filter Array`的历史到您的具体
>调用`first()`前的请求ID。历史端点返回所有作业；没有
>过滤，来自不同并发作业的状态可能会破坏您的轮询。

---

### runAfter Fallback（失败→替代操作）

当主操作失败时，路由到回退操作-没有条件块。
只需在备用服务器上设置`runAfter`，以接受来自主服务器的`["Failed"]`：```json
"HTTP_Get_Hi_Res": {
  "type": "Http",
  "runAfter": {},
  "inputs": { "method": "GET", "uri": "https://api.example.com/data?resolution=hi-res" }
},
"HTTP_Get_Low_Res": {
  "type": "Http",
  "runAfter": { "HTTP_Get_Hi_Res": ["Failed"] },
  "inputs": { "method": "GET", "uri": "https://api.example.com/data?resolution=low-res" }
}
```
下面的操作可以使用`runAfter`接受`["Succeeded", "Skipped"]`到
>处理任意路径-参见下面的风扇连接门。

---

扇入连接门（合并两个互斥分支）

当两个分支是互斥的（每次运行只有一个分支可以成功）时，使用单个分支
从**两个**分支接受`["Succeeded", "Skipped"]`的下游动作。
不管哪个分支跑过，门只会触发一次：```json
"Increment_Count": {
  "type": "IncrementVariable",
  "runAfter": {
    "Update_Hi_Res_Metadata":  ["Succeeded", "Skipped"],
    "Update_Low_Res_Metadata": ["Succeeded", "Skipped"]
  },
  "inputs": { "name": "LoopCount", "value": 1 }
}
```
>这避免了在每个分支中重复下游操作。关键观点：
无论哪个分支被跳过，都会报告`Skipped`—门接受该状态和
>开火一次。只有当两个分支真正互斥时才有效
>（例如一个是另一个的`runAfter: [...Failed]`）。

---

# #表达式

###通用表达式模式```
Null-safe field access:    @item()?['FieldName']
Null guard:                @coalesce(item()?['Name'], 'Unknown')
String format:             @{variables('firstName')} @{variables('lastName')}
Date today:                @utcNow()
Formatted date:            @formatDateTime(utcNow(), 'dd/MM/yyyy')
Add days:                  @addDays(utcNow(), 7)
Array length:              @length(variables('myArray'))
Filter array:              Use the "Filter array" action (no inline filter expression exists in PA)
Union (new wins):          @union(body('New_Data'), outputs('Old_Data'))
Sort:                      @sort(variables('myArray'), 'Date')
Unix timestamp → date:     @formatDateTime(addseconds('1970-1-1', triggerBody()?['created']), 'yyyy-MM-dd')
Date → Unix milliseconds:  @div(sub(ticks(startOfDay(item()?['Created'])), ticks(formatDateTime('1970-01-01Z','o'))), 10000)
Date → Unix seconds:       @div(sub(ticks(item()?['Start']), ticks('1970-01-01T00:00:00Z')), 10000000)
Unix seconds → datetime:   @addSeconds('1970-01-01T00:00:00Z', int(variables('Unix')))
Coalesce as no-else:       @coalesce(outputs('Optional_Step'), outputs('Default_Step'))
Flow elapsed minutes:      @div(float(sub(ticks(utcNow()), ticks(outputs('Flow_Start')))), 600000000)
HH:mm time string:         @formatDateTime(outputs('Local_Datetime'), 'HH:mm')
Response header:           @outputs('HTTP_Action')?['headers/X-Request-Id']
Array max (by field):      @reverse(sort(body('Select_Items'), 'Date'))[0]
Integer day span:          @int(split(dateDifference(outputs('Start'), outputs('End')), '.')[0])
ISO week number:           @div(add(dayofyear(addDays(subtractFromTime(date, sub(dayofweek(date),1), 'Day'), 3)), 6), 7)
Join errors to string:     @if(equals(length(variables('Errors')),0), null, concat(join(variables('Errors'),', '),' not found.'))
Normalize before compare:  @replace(coalesce(outputs('Value'),''),'_',' ')
Robust non-empty check:    @greater(length(trim(coalesce(string(outputs('Val')), ''))), 0)
```
不支持/有风险的表达式假设

Power automation表达式是工作流定义语言，而不是JavaScript。
这些模式通常看起来似乎合理，但并不像代理那样部署或表现
期望:

目标|避免|使用|代替||---|---|---|
构建一个内联对象|`createObject(...)`|一个包含JSON对象文字|的组合动作
|变换数组内联|`select(...)`在表达式|数据操作`Select`动作|
|在表达式内过滤数组|`filter(...)`|数据操作`Filter array`动作|
|使用计数器变量查找数组项索引|`indexOf(array, item)`| Foreach，或构建键控对象映射|

表达式中的换行符

b> **`\n`不会在Power automation表达式中产生换行符。**它是
>被视为文字反斜杠+`n`，将逐字显示或显示
>验证错误。

在需要换行符的地方使用`decodeUriComponent('%0a')`：```
Newline (LF):   decodeUriComponent('%0a')
CRLF:           decodeUriComponent('%0d%0a')
```
示例-通过`concat()`的多行团队或电子邮件正文：```json
"Compose_Message": {
  "type": "Compose",
  "inputs": "@concat('Hi ', outputs('Get_User')?['body/displayName'], ',', decodeUriComponent('%0a%0a'), 'Your report is ready.', decodeUriComponent('%0a'), '- The Team')"
}
```
示例-`join()`用换行符分隔：```json
"Compose_List": {
  "type": "Compose",
  "inputs": "@join(body('Select_Names'), decodeUriComponent('%0a'))"
}
```
这是在动态构建的字符串中嵌入换行符的唯一可靠方法
Power automation流定义中的>（根据Logic Apps运行时确认）。

---

###数组求和（XPath技巧）

Power automation没有原生的`sum()`函数。在XML上使用XPath：```json
"Prepare_For_Sum": {
  "type": "Compose",
  "runAfter": {},
  "inputs": { "root": { "numbers": "@body('Select_Amounts')" } }
},
"Sum": {
  "type": "Compose",
  "runAfter": { "Prepare_For_Sum": ["Succeeded"] },
  "inputs": "@xpath(xml(outputs('Prepare_For_Sum')), 'sum(/root/numbers)')"
}
```
`Select_Amounts`必须输出数字的平面数组（使用**Select**操作先提取单个数字字段）。结果是一个可以在条件或计算中直接使用的数字。

>这是在Power automation中聚合（sum/min/max）数组而不使用循环的唯一方法。