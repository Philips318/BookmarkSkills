---
name: entra-agent-user
description: 'Create Agent Users in Microsoft Entra ID from Agent Identities, enabling AI agents to act as digital workers with user identity capabilities in Microsoft 365 and Azure environments.'
---
#技能：在Microsoft Entra中创建Agent用户

# #概述

**代理用户**是微软Entra ID中的一个专门的用户身份，它使人工智能代理能够充当数字工作者。它允许代理访问严格要求用户身份的api和服务（例如，Exchange邮箱、Teams、组织结构图），同时保持适当的安全边界。

代理用户接收带有`idtyp=user`的令牌，这与接收`idtyp=app`的常规代理身份不同。

---

# #先决条件

-具有Agent ID功能的Microsoft Entra租户
—根据**代理身份蓝图**创建的**代理身份**（类型为`ServiceIdentity`的业务主体）
-下列**权限之一：
-`AgentIdUser.ReadWrite.IdentityParentedBy`（最低权限）
——`AgentIdUser.ReadWrite.All`——`User.ReadWrite.All`—呼叫者必须至少具有**Agent ID Administrator**角色（在委托场景中）重要：`identityParentId`必须引用一个真正的代理身份（通过代理身份蓝图创建），而不是一个常规的应用程序服务主体。您可以通过检查服务主体是否具有`@odata.type: #microsoft.graph.agentIdentity`和`servicePrincipalType: ServiceIdentity`来进行验证。

---

# #架构```
Agent Identity Blueprint (application template)
    │
    ├── Agent Identity (service principal - ServiceIdentity)
    │       │
    │       └── Agent User (user - agentUser) ← 1:1 relationship
    │
    └── Agent Identity Blueprint Principal (service principal in tenant)
```
|组件|类型|令牌声明|目的||---|---|---|---|
|代理身份|服务主体|`idtyp=app`|Backend/API操作|
|座席用户|用户（`agentUser`） |`idtyp=user`|在M365中充当数字工作者|

---

##步骤1：验证代理身份是否存在

在创建代理用户之前，请确认代理标识为合适的`agentIdentity`类型：```http
GET https://graph.microsoft.com/beta/servicePrincipals/{agent-identity-id}
Authorization: Bearer <token>
```
验证响应包含：```json
{
  "@odata.type": "#microsoft.graph.agentIdentity",
  "servicePrincipalType": "ServiceIdentity",
  "agentIdentityBlueprintId": "<blueprint-id>"
}
```
# # # PowerShell```powershell
Connect-MgGraph -Scopes "Application.Read.All" -TenantId "<tenant>" -UseDeviceCode -NoWelcome
Invoke-MgGraphRequest -Method GET `
  -Uri "https://graph.microsoft.com/beta/servicePrincipals/<agent-identity-id>" | ConvertTo-Json -Depth 3
```
常见错误：**使用应用程序注册的`appId`或常规应用程序服务主体的`id`将失败。只有根据蓝图创建的代理身份才有效。

---

步骤2：创建代理用户

### HTTP请求```http
POST https://graph.microsoft.com/beta/users/microsoft.graph.agentUser
Content-Type: application/json
Authorization: Bearer <token>

{
  "accountEnabled": true,
  "displayName": "My Agent User",
  "mailNickname": "my-agent-user",
  "userPrincipalName": "my-agent-user@yourtenant.onmicrosoft.com",
  "identityParentId": "<agent-identity-object-id>"
}
```
必需的属性

|属性|类型|描述||---|---|---|
|`accountEnabled`|布尔值|`true`，启用帐户|
|`displayName`|字符串|人性化名称|
|`mailNickname`| String |邮件别名（无spaces/special字符）|
|`userPrincipalName`| String | UPN -在租户（`alias@verified-domain`） |中必须唯一
|`identityParentId`| String |父代理标识ID |

# # # PowerShell```powershell
Connect-MgGraph -Scopes "User.ReadWrite.All" -TenantId "<tenant>" -UseDeviceCode -NoWelcome

$body = @{
  accountEnabled    = $true
  displayName       = "My Agent User"
  mailNickname      = "my-agent-user"
  userPrincipalName = "my-agent-user@yourtenant.onmicrosoft.com"
  identityParentId  = "<agent-identity-object-id>"
} | ConvertTo-Json

Invoke-MgGraphRequest -Method POST `
  -Uri "https://graph.microsoft.com/beta/users/microsoft.graph.agentUser" `
  -Body $body -ContentType "application/json" | ConvertTo-Json -Depth 3
```
###关键提示

- **无密码** - agent用户不能设置密码。它们通过父代理身份的凭证进行身份验证。
- **1:1关系** -每个代理身份最多只能有一个代理用户。尝试创建第二个返回`400 Bad Request`。
—`userPrincipalName`不能重复。不要重用现有用户的UPN。

---

##步骤3：分配一个管理员（可选）

分配经理允许代理用户出现在组织结构图中（例如，团队）。```http
PUT https://graph.microsoft.com/beta/users/{agent-user-id}/manager/$ref
Content-Type: application/json
Authorization: Bearer <token>

{
  "@odata.id": "https://graph.microsoft.com/beta/users/{manager-user-id}"
}
```
# # # PowerShell```powershell
$managerBody = '{"@odata.id":"https://graph.microsoft.com/beta/users/<manager-user-id>"}'
Invoke-MgGraphRequest -Method PUT `
  -Uri "https://graph.microsoft.com/beta/users/<agent-user-id>/manager/`$ref" `
  -Body $managerBody -ContentType "application/json"
```
---

##步骤4：设置使用位置和分配许可证（可选）

代理用户需要许可证才能拥有邮箱、Teams状态等。首先必须设置使用位置。

###设置使用位置```http
PATCH https://graph.microsoft.com/beta/users/{agent-user-id}
Content-Type: application/json
Authorization: Bearer <token>

{
  "usageLocation": "US"
}
```
列出可用的许可证```http
GET https://graph.microsoft.com/beta/subscribedSkus?$select=skuPartNumber,skuId,consumedUnits,prepaidUnits
Authorization: Bearer <token>
```
需要`Organization.Read.All`权限。

###分配License```http
POST https://graph.microsoft.com/beta/users/{agent-user-id}/assignLicense
Content-Type: application/json
Authorization: Bearer <token>

{
  "addLicenses": [
    { "skuId": "<sku-id>" }
  ],
  "removeLicenses": []
}
```
PowerShell （all in one）```powershell
Connect-MgGraph -Scopes "User.ReadWrite.All","Organization.Read.All" -TenantId "<tenant>" -NoWelcome

# Set usage location
Invoke-MgGraphRequest -Method PATCH `
  -Uri "https://graph.microsoft.com/beta/users/<agent-user-id>" `
  -Body '{"usageLocation":"US"}' -ContentType "application/json"

# Assign license
$licenseBody = '{"addLicenses":[{"skuId":"<sku-id>"}],"removeLicenses":[]}'
Invoke-MgGraphRequest -Method POST `
  -Uri "https://graph.microsoft.com/beta/users/<agent-user-id>/assignLicense" `
  -Body $licenseBody -ContentType "application/json"
```
**提示：**您也可以通过**Entra管理中心**的“身份→用户→所有用户→选择代理用户→许可证和应用程序”来分配许可证。

---

##配置时间

|业务|估计时间||---|---|
|交换邮箱| 5-30分钟|
|团队可用性| 15分钟- 24小时|
|组织结构图/人员搜索|最多24-48小时|
| SharePoint / OneDrive | 5-30分钟|
|全局地址列表|不超过24小时|

---

## Agent User Capabilities

-✅添加到Microsoft Entra群组（包括动态群组）
-✅访问用户专用api （`idtyp=user`令牌）
-✅拥有邮箱，日历和联系人
-✅参与团队聊天和频道
-✅出现在组织结构图和人员搜索
-✅新增管理单位
-✅已分配的license

代理用户安全约束

-❌不能有密码、密钥或交互式登录
-❌不能被赋予特权管理员角色
-❌不能加入角色分配组
—❌默认权限类似guest用户
-❌不支持自定义角色分配

---

# #故障排除

|错误|原因|修复||---|---|---|
|`Agent user IdentityParent does not exist`|`identityParentId`指向不存在或非代理身份对象|验证ID是`agentIdentity`服务主体，而不是常规应用|
|`400 Bad Request`(identityParentId already linked) |代理身份已经有一个代理用户|每个代理身份只支持一个代理用户|
|`409 Conflict`上的UPN |`userPrincipalName`已经占用|使用唯一的UPN |
| License分配失败|使用位置未设置|分配License前设置`usageLocation`|

---

# #引用

-[代理身份]（https://learn.microsoft.com/en-us/entra/agent-id/identity-platform/agent-identities）
-[代理用户]（https://learn.microsoft.com/en-us/entra/agent-id/identity-platform/agent-users）
-[代理服务主体]（https://learn.microsoft.com/en-us/entra/agent-id/identity-platform/agent-service-principals）
-[创建代理身份蓝图]（https://learn.microsoft.com/en-us/entra/agent-id/identity-platform/create-blueprint）
-[创建代理身份]（https://learn.microsoft.com/en-us/entra/agent-id/identity-platform/create-delete-agent-identities）
—[agentUser资源类型（图API）]（https://learn.microsoft.com/en-us/graph/api/resources/agentuser?view=graph-rest-beta）
- [Create agentUser (Graph API)]（https://learn.microsoft.com/en-us/graph/api/agentuser-post?view=graph-rest-beta）