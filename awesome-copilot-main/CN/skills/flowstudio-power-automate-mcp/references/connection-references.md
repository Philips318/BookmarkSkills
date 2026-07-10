# FlowStudio MCP -连接参考

连接引用将流的连接器操作连接到真正经过身份验证的操作
电源平台的连接。无论你什么时候打电话，都需要它们`update_live_flow`，使用连接器操作的定义。

---

流定义中的结构```json
{
  "properties": {
    "definition": { ... },
    "connectionReferences": {
      "shared_sharepointonline": {
        "connectionName": "shared-sharepointonl-eeeeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee",
        "id": "/providers/Microsoft.PowerApps/apis/shared_sharepointonline",
        "displayName": "SharePoint"
      },
      "shared_office365": {
        "connectionName": "shared-office365-xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx",
        "id": "/providers/Microsoft.PowerApps/apis/shared_office365",
        "displayName": "Office 365 Outlook"
      }
    }
  }
}
```
键是逻辑引用名（例如`shared_sharepointonline`）。
它们匹配每个动作的`host`块中的`connectionName`字段。

---

查找连接引用

首选方法：在目标环境中调用`list_live_connections`。使用`search`将结果缩小到您需要的连接器；较新的MCP服务器版本
返回粘贴准备好的模板。```python
matches = mcp("list_live_connections",
    environmentName=ENV,
    search="shared_sharepointonline")

conn = next(c for c in matches["connections"]
            if c.get("overallStatus") == "Connected"
            or c.get("statuses", [{}])[0].get("status") == "Connected")

conn_refs = {
    "shared_sharepointonline": conn.get("connectionReferenceTemplate") or {
        "connectionName": conn["id"],
        "id": "/providers/Microsoft.PowerApps/apis/shared_sharepointonline",
        "source": "Invoker"
    }
}
host = conn.get("hostTemplate") or {"connectionName": "shared_sharepointonline"}
```
使用`host`作为操作端`inputs.host`。使用`conn_refs`作为`update_live_flow(connectionReferences=conn_refs)`。

回退方法：从现有流复制。

在使用相同连接的任何现有流上调用`get_live_flow`然后复制`connectionReferences`块。连接器前缀后的GUID为
验证用户拥有的连接实例。```python
flow = mcp("get_live_flow", environmentName=ENV, flowName=EXISTING_FLOW_ID)
conn_refs = flow["properties"]["connectionReferences"]
# conn_refs["shared_sharepointonline"]["connectionName"]
# → "shared-sharepointonl-eeeeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"
```
>⚠️连接引用是**用户范围的**。是否拥有连接
通过另一个帐户>，`update_live_flow`将返回403
>`ConnectionAuthorizationFailed`。您必须使用属于的连接
>令牌在`x-api-key`报头中的帐户。

---

##将`connectionReferences`传递给`update_live_flow````python
result = mcp("update_live_flow",
    environmentName=ENV,
    flowName=FLOW_ID,
    definition=modified_definition,
    connectionReferences={
        "shared_sharepointonline": {
            "connectionName": "shared-sharepointonl-eeeeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee",
            "id": "/providers/Microsoft.PowerApps/apis/shared_sharepointonline"
        }
    }
)
```
只包括定义中实际使用的连接。

---

##公共连接器API id

|服务| API ID ||---|---|
| SharePoint Online |`/providers/Microsoft.PowerApps/apis/shared_sharepointonline`|
| Office 365 Outlook |`/providers/Microsoft.PowerApps/apis/shared_office365`|
|微软团队|`/providers/Microsoft.PowerApps/apis/shared_teams`|
OneDrive for Business |`/providers/Microsoft.PowerApps/apis/shared_onedriveforbusiness`|
Azure AD |`/providers/Microsoft.PowerApps/apis/shared_azuread`|
| HTTP与Azure AD |`/providers/Microsoft.PowerApps/apis/shared_webcontents`|
| SQL Server |`/providers/Microsoft.PowerApps/apis/shared_sql`|
| Dataverse |`/providers/Microsoft.PowerApps/apis/shared_commondataserviceforapps`|
| Azure Blob存储|`/providers/Microsoft.PowerApps/apis/shared_azureblob`|
|批准|`/providers/Microsoft.PowerApps/apis/shared_approvals`|
| Office 365用户|`/providers/Microsoft.PowerApps/apis/shared_office365users`|
|流量管理|`/providers/Microsoft.PowerApps/apis/shared_flowmanagement`|

---

团队自适应卡双连接要求

发送自适应卡片**和**发布后续消息的流程需要两个
独立团队连接：```json
"connectionReferences": {
  "shared_teams": {
    "connectionName": "shared-teams-xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx",
    "id": "/providers/Microsoft.PowerApps/apis/shared_teams"
  },
  "shared_teams_1": {
    "connectionName": "shared-teams-yyyyyyyy-yyyy-yyyy-yyyy-yyyyyyyyyyyy",
    "id": "/providers/Microsoft.PowerApps/apis/shared_teams"
  }
}
```
两者都可以指向相同的基础Teams帐户，但必须注册
作为两个不同的连接引用。webhook （`OpenApiConnectionWebhook`）
使用`shared_teams`，随后的消息操作使用`shared_teams_1`。