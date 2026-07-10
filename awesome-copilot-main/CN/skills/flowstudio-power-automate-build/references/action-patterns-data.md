# FlowStudio MCP -动作模式：数据转换

数组操作、HTTP调用、解析和数据转换模式。

>所有示例都假设`"runAfter"`设置适当。
>`<connectionName>`是`connectionReferences`中的**键**（例如`shared_sharepointonline`），而不是GUID。
GUID进入映射值的`connectionName`属性。

---

##数组操作

###选择（重塑/投影数组）

转换数组中的每个项，只保留所需的列或重命名它们。
避免在流的其余部分携带大型对象。```json
"Select_Needed_Columns": {
  "type": "Select",
  "runAfter": {},
  "inputs": {
    "from": "@outputs('HTTP_Get_Subscriptions')?['body/data']",
    "select": {
      "id":           "@item()?['id']",
      "status":       "@item()?['status']",
      "trial_end":    "@item()?['trial_end']",
      "cancel_at":    "@item()?['cancel_at']",
      "interval":     "@item()?['plan']?['interval']"
    }
  }
}
```
结果参考：`@body('Select_Needed_Columns')`-返回一个直接的重塑对象数组。>在循环或过滤之前使用Select，以减少有效负载大小并简化
>下游表达式。适用于任何数组- SP结果，HTTP响应，变量。
>
> * *小贴士:* *
> - **单到数组强制转换：**当API返回单个对象，但你需要
>选择（这需要一个数组），包装它：`@array(body('Get_Employee')?['data'])`。
>输出是一个单元素数组-通过`?[0]?['field']`访问结果。
> - **Null-normalize可选字段：**使用`@if(empty(item()?['field']), null, item()?['field'])`在每个可选字段上添加>，以规范化空字符串、缺失属性和空
>对象显式为`null`。确保一致的下游`@equals(..., @null)`检查。
**项目嵌套属性到平面字段：
> ' ' '
>“manager_name”:“@ if(空(项目()(“经理”)?['name']), null, item()?['manager']？['名字'])”
> ' ' '
>这允许直接与来自另一个源的平面模式进行字段级比较。

---### Filter Array （Query）

将数组筛选为符合条件的项。使用动作表单（而不是`filter()`）
表达式)用于复杂的多条件逻辑-它更清晰，更容易维护。```json
"Filter_Active_Subscriptions": {
  "type": "Query",
  "runAfter": {},
  "inputs": {
    "from": "@body('Select_Needed_Columns')",
    "where": "@and(or(equals(item().status, 'trialing'), equals(item().status, 'active')), equals(item().cancel_at, null))"
  }
}
```
结果参考：`@body('Filter_Active_Subscriptions')`- direct filtered array。

提示：在同一个源阵列上运行多个Filter Array操作来创建
>命名桶（例如活动，正在取消，完全取消），然后使用
>`coalesce(first(body('Filter_A')), first(body('Filter_B')), ...)`选择
>是没有任何循环的最高优先级匹配。

---

###创建CSV表（数组→字符串）

将对象数组转换为csv格式的字符串—没有连接器调用，没有代码。
在`Select`或`Filter Array`后面使用，用于导出数据或将其传递给文件写操作。```json
"Create_CSV": {
  "type": "Table",
  "runAfter": {},
  "inputs": {
    "from": "@body('Select_Output_Columns')",
    "format": "CSV"
  }
}
```
结果参考：`@body('Create_CSV')`-一个带有标题行+数据行的普通字符串。```json
// Custom column order / renamed headers:
"Create_CSV_Custom": {
  "type": "Table",
  "inputs": {
    "from": "@body('Select_Output_Columns')",
    "format": "CSV",
    "columns": [
      { "header": "Date",        "value": "@item()?['transactionDate']" },
      { "header": "Amount",      "value": "@item()?['amount']" },
      { "header": "Description", "value": "@item()?['description']" }
    ]
  }
}
```
>如果没有`columns`，头文件将取自源数组中的对象属性名。
>使用`columns`，可以显式地控制标题名称和列顺序。
>
>输出为原始字符串。使用`CreateFile`或`UpdateFile`将其写入文件
>（将`body`设置为`@body('Create_CSV')`），或者使用`SetVariable`存储在变量中。
>
>如果源数据来自Power BI的`ExecuteDatasetQuery`，列名将是
>包在方括号中（例如`[Amount]`）。写字前把它们剥掉：
>`@replace(replace(body('Create_CSV'),'[',''),']','')`---

### range() + Select for Array Generation`range(0, N)`产生一个整数序列`[0, 1, 2, …, N-1]`。把它穿过去
a选择操作以生成日期序列、索引网格或任何计算数组
没有循环：```json
// Generate 14 consecutive dates starting from a base date
"Generate_Date_Series": {
  "type": "Select",
  "inputs": {
    "from": "@range(0, 14)",
    "select": "@addDays(outputs('Base_Date'), item(), 'yyyy-MM-dd')"
  }
}
```
结果：`@body('Generate_Date_Series')`→`["2025-01-06", "2025-01-07", …, "2025-01-19"]`对于笛卡尔积，迭代`range(0, mul(rowCount, colCount))`并推导
索引为`div(item(), colCount)`和`mod(item(), colCount)`。

---

通过json（concat(join())）创建动态字典

当您需要在运行时进行O(1)键→值查找时，Power automation没有本机
使用Select + join + json从数组中创建一个字典类型：```json
"Build_Key_Value_Pairs": {
  "type": "Select",
  "inputs": {
    "from": "@body('Get_Lookup_Items')?['value']",
    "select": "@concat('\"', item()?['Key'], '\":\"', item()?['Value'], '\"')"
  }
},
"Assemble_Dictionary": {
  "type": "Compose",
  "inputs": "@json(concat('{', join(body('Build_Key_Value_Pairs'), ','), '}'))"
}
```
查找:`@outputs('Assemble_Dictionary')?['myKey']`>`json(concat('{', join(...), '}'))`模式适用于字符串值。为数字
>或布尔值，省略值部分周围的内部转义引号。
>密钥必须是唯一的，重复的密钥将静默覆盖之前的密钥。
>这取代了深度嵌套的`if(equals(key,'A'),'X', if(equals(key,'B'),'Y', ...))`链。

---

### union（）用于更改字段检测

当您需要查找几个字段中的*任意*更改的记录时，运行一个
每个字段`Filter Array`，结果`union()`。这避免了复杂的
多条件过滤器，并产生一个干净的重复数据删除集：```json
"Filter_Name_Changed": {
  "type": "Query",
  "inputs": { "from": "@body('Existing_Records')",
              "where": "@not(equals(item()?['name'], item()?['dest_name']))" }
},
"Filter_Status_Changed": {
  "type": "Query",
  "inputs": { "from": "@body('Existing_Records')",
              "where": "@not(equals(item()?['status'], item()?['dest_status']))" }
},
"All_Changed": {
  "type": "Compose",
  "inputs": "@union(body('Filter_Name_Changed'), body('Filter_Status_Changed'))"
}
```
参考：`@outputs('All_Changed')`-任何更改的行的重复数据删除数组。

>`union()`根据对象标识进行重复数据删除，因此在两个字段中都更改了一行
>出现一次。根据需要在`union()`中添加更多`Filter_*_Changed`输入：
>`@union(body('F1'), body('F2'), body('F3'))`---

文件-内容更改门

在对文件或blob运行代价高昂的处理之前，比较其当前内容
到存储的基线。如果没有任何更改，则完全跳过-使同步流
幂等且安全地重新运行或积极调度。```json
"Get_File_From_Source": { ... },
"Get_Stored_Baseline": { ... },
"Condition_File_Changed": {
  "type": "If",
  "expression": {
    "not": {
      "equals": [
        "@base64(body('Get_File_From_Source'))",
        "@body('Get_Stored_Baseline')"
      ]
    }
  },
  "actions": {
    "Update_Baseline": { "...": "overwrite stored copy with new content" },
    "Process_File":    { "...": "all expensive work goes here" }
  },
  "else": { "actions": {} }
}
```
将基线作为文件存储在SharePoint或blob存储中-`base64()`-编码
>实时内容之前比较，以便二进制和文本文件统一处理。
>在**处理之前写入新的基线**，以便在部分失败后重新运行
>不会再次重新处理同一个文件。

---

Set-Join for Sync（更新检测无嵌套循环）

当将源集合同步到目标时（例如API响应→SharePoint列表），
CSV→数据库)，避免嵌套的`Apply to each`循环来查找更改的记录。
相反，**投射平面键数组**并使用`contains()`执行集合操作
零嵌套循环，最后的循环只涉及改变的项目。

**Insert/update/delete同步配方：**1.`Select_Dest_Keys`从目标行。
2.`Filter_To_Insert`：键不在目标键中的源行。
3.`Filter_Already_Exists`：键在目标键中的源行。
4. 对于每个比较字段，运行`Filter_<Field>_Changed`；将它们与`union()`变成`Union_Changed`。
5. 从`Union_Changed`筛选`Select_Changed_Keys`，然后将目标行筛选为
在更新之前只使用这些键。
6.`Select_Source_Keys`，然后`Filter_To_Delete`目标行，其键为
不在源键中。

这将O（n x m）个嵌套循环更改为O（n + m）个集合操作，并有助于避免
Power automation的100k动作运行限制。

---

第一或空单行查找

在结果数组上使用`first()`提取一条没有循环的记录。
然后对输出进行空检查，以保护下游操作。```json
"Get_First_Match": {
  "type": "Compose",
  "runAfter": { "Get_SP_Items": ["Succeeded"] },
  "inputs": "@first(outputs('Get_SP_Items')?['body/value'])"
}
```
在Condition中，测试是否与**`@null`字面值**不匹配（而不是`empty()`）：```json
"Condition": {
  "type": "If",
  "expression": {
    "not": {
      "equals": [
        "@outputs('Get_First_Match')",
        "@null"
      ]
    }
  }
}
```
访问匹配行的字段：`@outputs('Get_First_Match')?['FieldName']`>当您只需要一个匹配记录时，使用这个而不是`Apply to each`。
>`first()`对空数组返回`null`；`empty()`表示arrays/strings；
>不是标量-在`first()`结果上使用它会导致运行时错误。

---

## HTTP &解析

### HTTP动作（外部API）```json
"Call_External_API": {
  "type": "Http",
  "runAfter": {},
  "inputs": {
    "method": "POST",
    "uri": "https://api.example.com/endpoint",
    "headers": {
      "Content-Type": "application/json",
      "Authorization": "Bearer @{variables('apiToken')}"
    },
    "body": {
      "data": "@outputs('Compose_Payload')"
    },
    "retryPolicy": {
      "type": "Fixed",
      "count": 3,
      "interval": "PT10S"
    }
  }
}
```
响应参考：`@outputs('Call_External_API')?['body']`####变体：ActiveDirectoryOAuth （Service-to-Service）

对于调用需要Azure AD客户端凭证的api（例如，Microsoft Graph），
使用内联OAuth代替Bearer令牌变量：```json
"Call_Graph_API": {
  "type": "Http",
  "runAfter": {},
  "inputs": {
    "method": "GET",
    "uri": "https://graph.microsoft.com/v1.0/users?$search=\"employeeId:@{variables('Code')}\"&$select=id,displayName",
    "headers": {
      "Content-Type": "application/json",
      "ConsistencyLevel": "eventual"
    },
    "authentication": {
      "type": "ActiveDirectoryOAuth",
      "authority": "https://login.microsoftonline.com",
      "tenant": "<tenant-id>",
      "audience": "https://graph.microsoft.com",
      "clientId": "<app-registration-id>",
      "secret": "@parameters('graphClientSecret')"
    }
  }
}
```
> **何时使用：**调用微软图形，Azure资源管理器，或任何
> Azure ad保护的API，来自没有高级连接器的流。
>`authentication`块处理整个OAuth客户端凭证流
>透明-不需要手动令牌获取步骤。
>
>`ConsistencyLevel: eventual`用于图`$search`查询。
>如果没有，`$search`返回400。
>
对于PATCH/PUT写入，相同的`authentication`块工作-只是更改
>`method`，加上`body`。
>
>⚠️**永远不要硬编码`secret`内联。**使用`@parameters('graphClientSecret')`>并在流的`parameters`块中声明它（类型为`securestring`）。这
>防止秘密出现在运行历史记录中或被读取
>`get_live_flow`。像这样声明参数：
> ' ' ' json
> "parameters": {
> "graphClientSecret": {"type": "securestring", "defaultValue": ""}
>}
> ' ' '
>然后通过流的连接或环境变量传递真实值
永远不要犯错误O源代码控制。---

HTTP响应（返回给调用者）

在http触发的流中使用，用于向调用方发送结构化回复。
必须在流超时之前运行（同步HTTP默认为2分钟）。```json
"Response": {
  "type": "Response",
  "runAfter": {},
  "inputs": {
    "statusCode": 200,
    "headers": {
      "Content-Type": "application/json"
    },
    "body": {
      "status": "success",
      "message": "@{outputs('Compose_Result')}"
    }
  }
}
```
b> **PowerApps /低代码调用模式**：总是返回`statusCode: 200`与一个
>`status`字段在主体（`"success"`/`"error"`）。PowerApps HTTP动作
>不能优雅地处理非2xx响应——调用者应该检查
>`body.status`而不是HTTP状态码。
>
>使用多个响应操作—每个分支一个—以便每个路径返回
>适当的消息。每次运行只执行一个。

---

子流调用（通过HTTP POST从父流到子流）

Power automation通过调用子流来支持父→子编排
HTTP直接触发URL。父进程发送一个HTTP POST并阻塞，直到
子节点返回一个`Response`动作。子流使用`manual`（请求）触发器。```json
// PARENT — call child flow and wait for its response
"Call_Child_Flow": {
  "type": "Http",
  "inputs": {
    "method": "POST",
    "uri": "https://prod-XX.australiasoutheast.logic.azure.com:443/workflows/<workflowId>/triggers/manual/paths/invoke?api-version=2016-06-01&sp=%2Ftriggers%2Fmanual%2Frun&sv=1.0&sig=<SAS>",
    "headers": { "Content-Type": "application/json" },
    "body": {
      "ID": "@triggerBody()?['ID']",
      "WeekEnd": "@triggerBody()?['WeekEnd']",
      "Payload": "@variables('dataArray')"
    },
    "retryPolicy": { "type": "none" }
  },
  "operationOptions": "DisableAsyncPattern",
  "runtimeConfiguration": {
    "contentTransfer": { "transferMode": "Chunked" }
  },
  "limit": { "timeout": "PT2H" }
}
```

```json
// CHILD — manual trigger receives the JSON body
// (trigger definition)
"manual": {
  "type": "Request",
  "kind": "Http",
  "inputs": {
    "schema": {
      "type": "object",
      "properties": {
        "ID": { "type": "string" },
        "WeekEnd": { "type": "string" },
        "Payload": { "type": "array" }
      }
    }
  }
}

// CHILD — return result to parent
"Response_Success": {
  "type": "Response",
  "inputs": {
    "statusCode": 200,
    "headers": { "Content-Type": "application/json" },
    "body": { "Result": "Success", "Count": "@length(variables('processed'))" }
  }
}
```
> **`retryPolicy: none`** -对父HTTP调用至关重要。没有它，一个孩子
>流超时触发重试，产生重复的子运行。
>
b> **`DisableAsyncPattern`** -防止父端将202 Accepted作为
>完成。父进程将阻塞，直到子进程发送它的`Response`。
>
> **`transferMode: Chunked`** -在将大数组（>100 KB）传递给子数组时启用；
>避免了请求大小限制。
>
> **`limit.timeout: PT2H`** -为长时间运行引发默认的2分钟HTTP超时
>孩子。最大是PT24H。
>
子流的触发器URL包含一个用于身份验证的SAS令牌（`sig=...`）
b>电话。从子流的触发器属性面板中复制它。URL变了
如果删除并重新创建触发器，则为>。

---

###解析JSON```json
"Parse_Response": {
  "type": "ParseJson",
  "runAfter": {},
  "inputs": {
    "content": "@outputs('Call_External_API')?['body']",
    "schema": {
      "type": "object",
      "properties": {
        "id": { "type": "integer" },
        "name": { "type": "string" },
        "items": {
          "type": "array",
          "items": { "type": "object" }
        }
      }
    }
  }
}
```
访问解析值：`@body('Parse_Response')?['name']`---

手动CSV→JSON（无Premium Action）

仅使用内置表达式将原始CSV字符串解析为对象数组。
避免高级的“Parse CSV”连接器操作。```json
"Delimiter": { "type": "Compose", "inputs": "," },
"Strip_Quotes": { "type": "Compose", "inputs": "@replace(body('Get_File_Content'), '\"', '')" },
"Detect_Line_Ending": {
  "type": "Compose",
  "inputs": "@if(equals(indexOf(outputs('Strip_Quotes'), decodeUriComponent('%0D%0A')), -1), if(equals(indexOf(outputs('Strip_Quotes'), decodeUriComponent('%0A')), -1), decodeUriComponent('%0D'), decodeUriComponent('%0A')), decodeUriComponent('%0D%0A'))"
},
"Headers": {
  "type": "Compose",
  "inputs": "@split(first(split(outputs('Strip_Quotes'), outputs('Detect_Line_Ending'))), outputs('Delimiter'))"
},
"Data_Rows": { "type": "Compose", "inputs": "@skip(split(outputs('Strip_Quotes'), outputs('Detect_Line_Ending')), 1)" },
"Select_CSV_Body": {
  "type": "Select",
  "inputs": {
    "from": "@outputs('Data_Rows')",
    "select": {
      "@{outputs('Headers')[0]}": "@split(item(), outputs('Delimiter'))[0]",
      "@{outputs('Headers')[1]}": "@split(item(), outputs('Delimiter'))[1]",
      "@{outputs('Headers')[2]}": "@split(item(), outputs('Delimiter'))[2]"
    }
  }
},
"Filter_Empty_Rows": {
  "type": "Query",
  "inputs": {
    "from": "@body('Select_CSV_Body')",
    "where": "@not(equals(item()?[outputs('Headers')[0]], null))"
  }
}
```
结果：`@body('Filter_Empty_Rows')`-以头名作为键的对象数组。

注意：`Detect_Line_Ending`处理CRLF/LF/CR.要求`Select`中的动态键`@{...}`插值。这个简单的模式不能安全地解析带引号的字段
带内嵌分隔符；对于这些，请使用专用解析器或自定义操作。

---

ConvertTimeZone（内置，无连接器）

转换时区之间的时间戳，无需API调用或连接器许可成本。
格式字符串`"g"`产生简短的地区日期+时间（`M/d/yyyy h:mm tt`）。```json
"Convert_to_Local_Time": {
  "type": "Expression",
  "kind": "ConvertTimeZone",
  "runAfter": {},
  "inputs": {
    "baseTime": "@{outputs('UTC_Timestamp')}",
    "sourceTimeZone": "UTC",
    "destinationTimeZone": "Taipei Standard Time",
    "formatString": "g"
  }
}
```
结果参考：`@body('Convert_to_Local_Time')`- **不是**`outputs()`，不像大多数操作。

常见的`formatString`值：`"g"`（短）、`"f"`（全）、`"yyyy-MM-dd"`、`"HH:mm"`常用时区字符串：`"UTC"`，`"AUS Eastern Standard Time"`,`"Taipei Standard Time"`，`"Singapore Standard Time"`,`"GMT Standard Time"`>这是`type: Expression, kind: ConvertTimeZone`-一个内置的逻辑应用程序操作，
>不是连接器。不需要连接参考。通过引用输出
>`body()`（不是`outputs()`），否则表达式返回null。