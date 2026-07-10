# FlowStudio MCP -触发器类型

复制-粘贴Power automation流定义的触发器定义。

---

# #复发

按计划跑步。```json
"Recurrence": {
  "type": "Recurrence",
  "recurrence": {
    "frequency": "Day",
    "interval": 1,
    "startTime": "2026-01-01T08:00:00Z",
    "timeZone": "AUS Eastern Standard Time"
  }
}
```
每周特定日期：```json
"Recurrence": {
  "type": "Recurrence",
  "recurrence": {
    "frequency": "Week",
    "interval": 1,
    "schedule": {
      "weekDays": ["Monday", "Tuesday", "Wednesday", "Thursday", "Friday"]
    },
    "startTime": "2026-01-05T09:00:00Z",
    "timeZone": "AUS Eastern Standard Time"
  }
}
```
常用的`timeZone`值：
-`"AUS Eastern Standard Time"`-Sydney/Melbourne（UTC+10/+11）
-`"UTC"`-世界时间
-`"E. Australia Standard Time"`-布里斯班（UTC+10无DST）
-`"New Zealand Standard Time"`-奥克兰（UTC+12/+13）
-洛杉矶（UTC-8/-7）
-`"GMT Standard Time"`-伦敦（UTC+0/+1）

---

## Manual （HTTP Request / Power Apps）

接收一个带有JSON主体的HTTP POST。```json
"manual": {
  "type": "Request",
  "kind": "Http",
  "inputs": {
    "schema": {
      "type": "object",
      "properties": {
        "name": { "type": "string" },
        "value": { "type": "integer" }
      },
      "required": ["name"]
    }
  }
}
```
访问值：`@triggerBody()?['name']`保存后可用的触发URL:`@listCallbackUrl()`####无模式变体（接受任意JSON）

当传入的有效负载结构未知或变化时，省略模式
在没有验证的情况下接受任何有效的JSON主体：```json
"manual": {
  "type": "Request",
  "kind": "Http",
  "inputs": {
    "schema": {}
  }
}
```
动态访问任何字段：`@triggerBody()?['anyField']`>将此用于外部webhooks （Stripe, GitHub， Employment Hero等）
>有效载荷形状可能会改变或未完整记录。流接受任何
> JSON不返回400的意外属性。

---

##手册（副驾驶工作室技能）

当流被Copilot Studio调用时，使用Skills触发器
代理工具。保持触发器模式显式，以便代理接收到可预测的
输入名称和类型。```json
"manual": {
  "type": "Request",
  "kind": "Skills",
  "inputs": {
    "schema": {
      "type": "object",
      "properties": {
        "itemId": { "type": "string" },
        "notes": { "type": "string" }
      },
      "required": ["itemId"]
    }
  },
  "metadata": {
    "operationMetadataId": "<stable-guid>"
  }
}
```
在部署了生产技能触发的流之后，调用`add_live_flow_to_solution`与目标`solutionId`；副驾驶工作室代理
工具发现期望流程能够感知解决方案。对于mcp驱动的测试，
使用具有相同操作和有效负载形状的临时HTTP副本，然后进行恢复
在操作验证后触发技能。

---

##自动（创建SharePoint项目）```json
"When_an_item_is_created": {
  "type": "OpenApiConnectionNotification",
  "inputs": {
    "host": {
      "apiId": "/providers/Microsoft.PowerApps/apis/shared_sharepointonline",
      "connectionName": "<connectionName>",
      "operationId": "OnNewItem"
    },
    "parameters": {
      "dataset": "https://mytenant.sharepoint.com/sites/mysite",
      "table": "MyList"
    },
    "subscribe": {
      "body": { "notificationUrl": "@listCallbackUrl()" },
      "queries": {
        "dataset": "https://mytenant.sharepoint.com/sites/mysite",
        "table": "MyList"
      }
    }
  }
}
```
访问触发数据：`@triggerBody()?['ID']`、`@triggerBody()?['Title']`等。

---

##自动（SharePoint项目修改）```json
"When_an_existing_item_is_modified": {
  "type": "OpenApiConnectionNotification",
  "inputs": {
    "host": {
      "apiId": "/providers/Microsoft.PowerApps/apis/shared_sharepointonline",
      "connectionName": "<connectionName>",
      "operationId": "OnUpdatedItem"
    },
    "parameters": {
      "dataset": "https://mytenant.sharepoint.com/sites/mysite",
      "table": "MyList"
    },
    "subscribe": {
      "body": { "notificationUrl": "@listCallbackUrl()" },
      "queries": {
        "dataset": "https://mytenant.sharepoint.com/sites/mysite",
        "table": "MyList"
      }
    }
  }
}
```
---

##自动（Outlook：当新邮件到达时）```json
"When_a_new_email_arrives": {
  "type": "OpenApiConnectionNotification",
  "inputs": {
    "host": {
      "apiId": "/providers/Microsoft.PowerApps/apis/shared_office365",
      "connectionName": "<connectionName>",
      "operationId": "OnNewEmail"
    },
    "parameters": {
      "folderId": "Inbox",
      "to": "monitored@contoso.com",
      "isHTML": true
    },
    "subscribe": {
      "body": { "notificationUrl": "@listCallbackUrl()" }
    }
  }
}
```
---

子流（被另一个流调用）```json
"manual": {
  "type": "Request",
  "kind": "Button",
  "inputs": {
    "schema": {
      "type": "object",
      "properties": {
        "items": {
          "type": "array",
          "items": { "type": "object" }
        }
      }
    }
  }
}
```
访问父方提供的数据：`@triggerBody()?['items']`要将数据返回给父节点，添加一个`Response`动作：```json
"Respond_to_Parent": {
  "type": "Response",
  "runAfter": { "Compose_Result": ["Succeeded"] },
  "inputs": {
    "statusCode": 200,
    "body": "@outputs('Compose_Result')"
  }
}
```
