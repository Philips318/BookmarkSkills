# FlowStudio MCP -动作模式：连接器

SharePoint、Outlook、Teams和Approvals连接器操作模式。

>所有示例都假设`"runAfter"`设置适当。
>将`<connectionName>`替换为`connectionReferences`中使用的**键**
>（如`shared_sharepointonline`，`shared_teams`）。这不是连接
> GUID——它是将操作链接到它的条目的逻辑引用名
>`connectionReferences`映射。

---

# # SharePoint

### SharePoint - Get项目```json
"Get_SP_Items": {
  "type": "OpenApiConnection",
  "runAfter": {},
  "inputs": {
    "host": {
      "apiId": "/providers/Microsoft.PowerApps/apis/shared_sharepointonline",
      "connectionName": "<connectionName>",
      "operationId": "GetItems"
    },
    "parameters": {
      "dataset": "https://mytenant.sharepoint.com/sites/mysite",
      "table": "MyList",
      "$filter": "Status eq 'Active'",
      "$top": 500
    }
  }
}
```
结果参考：`@outputs('Get_SP_Items')?['body/value']`> **带字符串插值的动态OData过滤器**：注入一个运行时值
使用`@{...}`语法将>直接放入`$filter`字符串中：
> ' ' '
> "$filter": “Title eq '@{outputs('ConfirmationCode')}'”
> ' ' '
注意双引号里面的单引号-正确的OData字符串字面值
>语法。避免单独的变量操作。

b> **大列表的分页**：默认情况下，GetItems在`$top`处停止。对auto-paginate
>之外，在操作上启用分页策略。在流定义中
>显示为：
> ' ' ' json
> "paginationPolicy": {"minimumItemCount": 10000}
> ' ' '
>将`minimumItemCount`设置为您期望的最大项数。连接器将
>保持获取页面，直到达到该计数或列表耗尽。没有这个,
>流在包含> 5000个条目的列表上静默返回一个上限结果。

---

### SharePoint - Get Item（单行ID）```json
"Get_SP_Item": {
  "type": "OpenApiConnection",
  "runAfter": {},
  "inputs": {
    "host": {
      "apiId": "/providers/Microsoft.PowerApps/apis/shared_sharepointonline",
      "connectionName": "<connectionName>",
      "operationId": "GetItem"
    },
    "parameters": {
      "dataset": "https://mytenant.sharepoint.com/sites/mysite",
      "table": "MyList",
      "id": "@triggerBody()?['ID']"
    }
  }
}
```
结果参考：`@body('Get_SP_Item')?['FieldName']`b>当您已经拥有ID时，使用`GetItem`（而不是带过滤器的`GetItems`）。
触发后重新抓取会给你当前行状态，而不是
在触发时间捕获的>快照-如果其他进程可能有，则很重要
>自流启动以来修改了项。

---

### SharePoint -创建项目```json
"Create_SP_Item": {
  "type": "OpenApiConnection",
  "runAfter": {},
  "inputs": {
    "host": {
      "apiId": "/providers/Microsoft.PowerApps/apis/shared_sharepointonline",
      "connectionName": "<connectionName>",
      "operationId": "PostItem"
    },
    "parameters": {
      "dataset": "https://mytenant.sharepoint.com/sites/mysite",
      "table": "MyList",
      "item/Title": "@variables('myTitle')",
      "item/Status": "Active"
    }
  }
}
```
---

### SharePoint - Update Item```json
"Update_SP_Item": {
  "type": "OpenApiConnection",
  "runAfter": {},
  "inputs": {
    "host": {
      "apiId": "/providers/Microsoft.PowerApps/apis/shared_sharepointonline",
      "connectionName": "<connectionName>",
      "operationId": "PatchItem"
    },
    "parameters": {
      "dataset": "https://mytenant.sharepoint.com/sites/mysite",
      "table": "MyList",
      "id": "@item()?['ID']",
      "item/Status": "Processed"
    }
  }
}
```
>`PatchItem`可以验证所需的SharePoint列，即使你没有
>更改这些字段。从触发器或
>先前的获取项操作，例如`item/Title`，并使用内部字段名。

---

### SharePoint -文件更新（创建或覆盖文档库）

如果文件已经存在，SharePoint的`CreateFile`失败。颠覆（创建或覆盖）
在没有事先存在检查的情况下，在**上同时使用`GetFileMetadataByPath`从`CreateFile`-如果创建失败，因为文件存在，元数据调用仍然
返回它的ID，`UpdateFile`可以覆盖它：```json
"Create_File": {
  "type": "OpenApiConnection",
  "inputs": {
    "host": { "apiId": "/providers/Microsoft.PowerApps/apis/shared_sharepointonline",
              "connectionName": "<connectionName>", "operationId": "CreateFile" },
    "parameters": {
      "dataset": "https://mytenant.sharepoint.com/sites/mysite",
      "folderPath": "/My Library/Subfolder",
      "name": "@{variables('filename')}",
      "body": "@outputs('Compose_File_Content')"
    }
  }
},
"Get_File_Metadata_By_Path": {
  "type": "OpenApiConnection",
  "runAfter": { "Create_File": ["Succeeded", "Failed"] },
  "inputs": {
    "host": { "apiId": "/providers/Microsoft.PowerApps/apis/shared_sharepointonline",
              "connectionName": "<connectionName>", "operationId": "GetFileMetadataByPath" },
    "parameters": {
      "dataset": "https://mytenant.sharepoint.com/sites/mysite",
      "path": "/My Library/Subfolder/@{variables('filename')}"
    }
  }
},
"Update_File": {
  "type": "OpenApiConnection",
  "runAfter": { "Get_File_Metadata_By_Path": ["Succeeded", "Skipped"] },
  "inputs": {
    "host": { "apiId": "/providers/Microsoft.PowerApps/apis/shared_sharepointonline",
              "connectionName": "<connectionName>", "operationId": "UpdateFile" },
    "parameters": {
      "dataset": "https://mytenant.sharepoint.com/sites/mysite",
      "id": "@outputs('Get_File_Metadata_By_Path')?['body/{Identifier}']",
      "body": "@outputs('Compose_File_Content')"
    }
  }
}
```
>如果`Create_File`成功，则`Get_File_Metadata_By_Path`为`Skipped`和`Update_File`>仍然触发（接受`Skipped`），无害地覆盖刚刚创建的文件。
>如果`Create_File`失败（文件存在），则元数据调用检索现有文件的ID
>和`Update_File`覆盖它。无论哪种方式，您都可以获得最新的内容。
>
> **文档库系统属性** -当迭代一个文件库结果时(例如：
>从`ListFolder`或`GetFilesV2`)，使用大括号属性名访问
> SharePoint的内置文件元数据。这些与列表字段名不同：
> ' ' '
> @item () ?['{Name}'] -文件名不带路径(例如：“report.csv”)
> @item () ?[' '{filenamewitheextension}'] -与大多数连接器中的{Name}相同
> @item () ?['{Identifier}'] -UpdateFile/DeleteFile使用的内部文件ID
> @item () ?['{FullPath}'] -完整的服务器相对路径
> @item () ?['{IsFolder}'] -布尔值，对文件夹为true条目
> ' ' '---

SharePoint - GetItemChanges列门

当SharePoint“项目修改”触发器触发时，它不会告诉你是哪一个
列变化。使用`GetItemChanges`获取每列更改标志，然后是gate
特定列上的下游逻辑：```json
"Get_Changes": {
  "type": "OpenApiConnection",
  "runAfter": {},
  "inputs": {
    "host": {
      "apiId": "/providers/Microsoft.PowerApps/apis/shared_sharepointonline",
      "connectionName": "<connectionName>",
      "operationId": "GetItemChanges"
    },
    "parameters": {
      "dataset": "https://mytenant.sharepoint.com/sites/mysite",
      "table": "<list-guid>",
      "id": "@triggerBody()?['ID']",
      "since": "@triggerBody()?['Modified']",
      "includeDrafts": false
    }
  }
}
```
门在特定的柱子上：```json
"expression": {
  "and": [{
    "equals": [
      "@body('Get_Changes')?['Column']?['hasChanged']",
      true
    ]
  }]
}
```
> **新项检测：**在第一次修改（1.0版本）时，
>`GetItemChanges`可能报告没有以前的版本。检查
>`@equals(triggerBody()?['OData__UIVersionString'], '1.0')`检测
>新创建的项，并跳过这些项的更改门逻辑。

---

### SharePoint - REST合并通过HttpRequest

用于标准不支持的交叉列表更新或高级操作
更新项目连接器（例如，更新不同站点中的列表），使用
SharePoint REST API通过`HttpRequest`操作：```json
"Update_Cross_List_Item": {
  "type": "OpenApiConnection",
  "runAfter": {},
  "inputs": {
    "host": {
      "apiId": "/providers/Microsoft.PowerApps/apis/shared_sharepointonline",
      "connectionName": "<connectionName>",
      "operationId": "HttpRequest"
    },
    "parameters": {
      "dataset": "https://mytenant.sharepoint.com/sites/target-site",
      "parameters/method": "POST",
      "parameters/uri": "/_api/web/lists(guid'<list-guid>')/items(@{variables('ItemId')})",
      "parameters/headers": {
        "Accept": "application/json;odata=nometadata",
        "Content-Type": "application/json;odata=nometadata",
        "X-HTTP-Method": "MERGE",
        "IF-MATCH": "*"
      },
      "parameters/body": "{ \"Title\": \"@{variables('NewTitle')}\", \"Status\": \"@{variables('NewStatus')}\" }"
    }
  }
}
```
> **键头：**
> -`X-HTTP-Method: MERGE`-告诉SharePoint做部分更新（PATCH语义）
> -`IF-MATCH: *`-覆盖不管当前ETag（无冲突检查）
>`HttpRequest`操作重用现有的SharePoint连接-没有额外的
需要>身份验证。当标准的Update Item连接器不能使用时，使用这个
>到达目标列表（不同的站点集合，或者您需要原始REST控制）。
>保持连接器特定的参数名称如下所示：
>`parameters/method`,`parameters/uri`,`parameters/headers`，和
>`parameters/body`。正文是一个JSON字符串，`parameters/uri`是相对的
到SharePoint`dataset`。

---

### SharePoint - File as JSON Database （Read + Parse）

使用SharePoint文档库JSON文件作为可查询的“数据库”
last-known-state记录。一个单独的过程（例如，Power BI数据流）维护
文件;流下载并过滤它以进行before/after比较。```json
"Get_File": {
  "type": "OpenApiConnection",
  "runAfter": {},
  "inputs": {
    "host": {
      "apiId": "/providers/Microsoft.PowerApps/apis/shared_sharepointonline",
      "connectionName": "<connectionName>",
      "operationId": "GetFileContent"
    },
    "parameters": {
      "dataset": "https://mytenant.sharepoint.com/sites/mysite",
      "id": "%252fShared%2bDocuments%252fdata.json",
      "inferContentType": false
    }
  }
},
"Parse_JSON_File": {
  "type": "Compose",
  "runAfter": { "Get_File": ["Succeeded"] },
  "inputs": "@json(decodeBase64(body('Get_File')?['$content']))"
},
"Find_Record": {
  "type": "Query",
  "runAfter": { "Parse_JSON_File": ["Succeeded"] },
  "inputs": {
    "from": "@outputs('Parse_JSON_File')",
    "where": "@equals(item()?['id'], variables('RecordId'))"
  }
}
```
b> **解码链：**`GetFileContent`返回base64编码的内容
>`body(...)?['$content']`。应用`decodeBase64()`然后`json()`得到a
>可用阵列。然后`Filter Array`充当WHERE子句。
>
> **何时使用：**当你需要一个轻量级的“before”快照来检测字段时
>从webhook负载（“after”状态）更改。比维护简单
>是一个完整的SharePoint列表镜像-可以很好地处理高达10K的记录。
>
> **文件路径编码：**在`id`参数中，SharePoint url对路径进行编码
>两次。空格变成`%2b`（加号），斜杠变成`%252f`。

---

## Excel Online

### Excel -运行Office脚本

为了节省时间，Office Script操作需要真实的工作簿和脚本标识符。
不要部署占位符`scriptId`值；`update_live_flow`在
甚至在测试运行存在之前进行动态操作验证。在可用时使用`describe_live_connector`或`get_live_dynamic_options`，或者
如果找不到工作簿和脚本，请向用户询问。如果是真的`scriptId`仍然无法解析，请用户添加Run脚本操作
进入设计器后，读取流定义并保留已解析的流
参数。

---

# #前景

### Outlook -发送电子邮件```json
"Send_Email": {
  "type": "OpenApiConnection",
  "runAfter": {},
  "inputs": {
    "host": {
      "apiId": "/providers/Microsoft.PowerApps/apis/shared_office365",
      "connectionName": "<connectionName>",
      "operationId": "SendEmailV2"
    },
    "parameters": {
      "emailMessage/To": "recipient@contoso.com",
      "emailMessage/Subject": "Automated notification",
      "emailMessage/Body": "<p>@{outputs('Compose_Message')}</p>",
      "emailMessage/IsHtml": true
    }
  }
}
```
---

### Outlook -获取电子邮件（从文件夹读取模板）```json
"Get_Email_Template": {
  "type": "OpenApiConnection",
  "runAfter": {},
  "inputs": {
    "host": {
      "apiId": "/providers/Microsoft.PowerApps/apis/shared_office365",
      "connectionName": "<connectionName>",
      "operationId": "GetEmailsV3"
    },
    "parameters": {
      "folderPath": "Id::<outlook-folder-id>",
      "fetchOnlyUnread": false,
      "includeAttachments": false,
      "top": 1,
      "importance": "Any",
      "fetchOnlyWithAttachment": false,
      "subjectFilter": "My Email Template Subject"
    }
  }
}
```
访问主题和主体：```
@first(outputs('Get_Email_Template')?['body/value'])?['subject']
@first(outputs('Get_Email_Template')?['body/value'])?['body']
```
> **Outlook-as- cms模式**：将模板电子邮件存储在专用的Outlook文件夹中。
>设置`fetchOnlyUnread: false`，以便模板在第一次使用后仍然存在。
>非技术用户可以通过编辑电子邮件来更新主题和正文
>不需要改变流量。将主题和主体直接传递到`SendEmailV2`。
>
>要获得Outlook中的文件夹ID：在web上，右键单击文件夹→打开
>新选项卡-文件夹GUID在URL中。在`folderPath`中加上`Id::`前缀。

---

# #团队

###团队-发布消息```json
"Post_Teams_Message": {
  "type": "OpenApiConnection",
  "runAfter": {},
  "inputs": {
    "host": {
      "apiId": "/providers/Microsoft.PowerApps/apis/shared_teams",
      "connectionName": "<connectionName>",
      "operationId": "PostMessageToConversation"
    },
    "parameters": {
      "poster": "Flow bot",
      "location": "Channel",
      "body/recipient": {
        "groupId": "<team-id>",
        "channelId": "<channel-id>"
      },
      "body/messageBody": "@outputs('Compose_Message')"
    }
  }
}
```
####变体：群聊（1:1或多人）

要发布到群组聊天而不是频道，请使用`"location": "Group chat"`线程ID作为接收者；```json
"Post_To_Group_Chat": {
  "type": "OpenApiConnection",
  "runAfter": {},
  "inputs": {
    "host": {
      "apiId": "/providers/Microsoft.PowerApps/apis/shared_teams",
      "connectionName": "<connectionName>",
      "operationId": "PostMessageToConversation"
    },
    "parameters": {
      "poster": "Flow bot",
      "location": "Group chat",
      "body/recipient": "19:<thread-hash>@thread.v2",
      "body/messageBody": "@outputs('Compose_Message')"
    }
  }
}
```
对于1:1（“与Flow bot聊天”），使用`"location": "Chat with Flow bot"`并设置`body/recipient`到用户的电子邮件地址。

> **Active-user gate:**在循环中发送通知时，检查接收方的
>在发布之前启用Azure AD帐户-避免交付失败
>工作人员:
> ' ' ' json
> "Check_User_Active": {
> "type": “OpenApiConnection”
> “输入”：{
> "host": {"apiId": "/providers/Microsoft.PowerApps/apis/shared_office365users"，
> "operationId": "UserProfile_V2"}，
> "parameters": {"id": "@{item()？”['邮件']}}
>}
>}
> ' ' '
然后闸门：`@equals(body('Check_User_Active')?['accountEnabled'], true)`---

##副驾驶工作室

###副驾驶工作室-调用代理

使用Copilot Studio连接器时，在运行
流。Draft/test代理可以存在于工作室画布中，但仍然不可用
或者通过流连接器端点失效。如果连接器操作因代理不可用或端点类型错误而失败，
发布代理，短暂等待传播，然后重新提交相同的流运行
在更改流定义之前。

---

# #批准

分割审批（创建→等待）

标准的“启动并等待批准”是一个单一的阻塞操作。
为了获得更多的控制（例如，在Teams中发布审批链接，或者添加超时）
作用域)，然后将其分成两个动作：`CreateAnApproval`（即发射即弃）`WaitForAnApproval`（网络钩子暂停）。```json
"Create_Approval": {
  "type": "OpenApiConnection",
  "runAfter": {},
  "inputs": {
    "host": {
      "apiId": "/providers/Microsoft.PowerApps/apis/shared_approvals",
      "connectionName": "<connectionName>",
      "operationId": "CreateAnApproval"
    },
    "parameters": {
      "approvalType": "CustomResponse/Result",
      "ApprovalCreationInput/title": "Review: @{variables('ItemTitle')}",
      "ApprovalCreationInput/assignedTo": "approver@contoso.com",
      "ApprovalCreationInput/details": "Please review and select an option.",
      "ApprovalCreationInput/responseOptions": ["Approve", "Reject", "Defer"],
      "ApprovalCreationInput/enableNotifications": true,
      "ApprovalCreationInput/enableReassignment": true
    }
  }
},
"Wait_For_Approval": {
  "type": "OpenApiConnectionWebhook",
  "runAfter": { "Create_Approval": ["Succeeded"] },
  "inputs": {
    "host": {
      "apiId": "/providers/Microsoft.PowerApps/apis/shared_approvals",
      "connectionName": "<connectionName>",
      "operationId": "WaitForAnApproval"
    },
    "parameters": {
      "approvalName": "@body('Create_Approval')?['name']"
    }
  }
}
```
> **`approvalType`选项：**
> -`"Approve/Reject - First to respond"`-二进制，第一个响应者获胜
> -`"Approve/Reject - Everyone must approve"`-要求所有受让人
> -`"CustomResponse/Result"`-定义自己的响应按钮
>
>在`Wait_For_Approval`之后，读取结果：
> ' ' '
> @body(“Wait_For_Approval”)?['outcome']→“批准”、“拒绝”或习惯
> @body (Wait_For_Approval) ?(“反应”)[0]?(“应答”)?[' displayName ']
> @body (Wait_For_Approval) ?(“反应”)[0]吗?(“评论”)
> ' ' '
>
分割模式允许您在创建和等待之间插入操作-例如，
>向Teams发布审批链接、启动超时范围或记录
>等待批准到跟踪列表。