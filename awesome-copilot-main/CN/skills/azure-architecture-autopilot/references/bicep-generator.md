#二头肌生成器代理

从阶段1接收最终的架构规范，并生成可部署的Bicep模板。

##第0步：验证最新规格（在生成二头肌之前需要）

不要在Bicep代码中硬编码API版本。
总是获取你打算使用的服务的MS Docs Bicep参考，并在使用它之前确认最新的稳定apiVersion。

验证步骤
1. 确定要使用的服务列表
2. 获取每个服务的MS Docs URL（使用web_fetch工具）
3. 从页面确认最新的稳定API版本
4. 用那个版本写肱二头肌

模型部署可用性检查（当使用Foundry/OpenAI模型时需要）

在生成Bicep**之前，验证用户指定的模型名在目标区域**中是可部署的。
模型的可用性因地区而异，并且经常变化——不要依赖于静态知识。**验证方法（按优先级排序）：**
1. 查看MS Docs型号可用性页面：https://learn.microsoft.com/en-us/azure/ai-services/openai/concepts/models2. 或者直接通过Azure CLI查询：   ```powershell
   az cognitiveservices account list-models --name "<FOUNDRY_NAME>" --resource-group "<RG_NAME>" -o table
   ```
（当Foundry资源已经存在时）

**如果目标地区没有该型号：**
-通知用户并建议可用的区域或替代模型
—未经用户同意，请勿更换其他型号或地区

每个服务的MS Docs url

完整的URL注册表在`references/azure-dynamic-sources.md`中。获取时引用这个文件。
参考文件位于`.github/skills/azure-architecture-autopilot/`路径下。

b> **重要**：使用web_fetch直接从URL获取，以确认最新的稳定apiVersion。不要盲目地使用参考文件或以前对话中的硬编码版本。

b> **也要始终验证子资源**：从父资源页面检查子资源（accounts/projects,accounts/deployments,privateDnsZones/virtualNetworkLinks，privateEndpoints/privateDnsZoneGroups等）的API版本。父API和子API版本可能不同。b> **发生errors/warnings时同样适用**：如果在假设或部署期间发生API版本相关错误，请不要将错误消息中的版本视为“最新版本”，而直接应用它。在进行更正之前，总是重新获取MS Docs URL以确认实际的最新稳定版本。

---

信息参考原则（稳定vs动态）

###总是取（动态）
- API版本→从`azure-dynamic-sources.md`的url获取
—型号可用性（名称、版本、地区）→获取
- SKUlist/pricing→Fetch
—区域可用性→提取

参考优先（稳定）
-所需的属性模式（`isHnsEnabled`，`allowProjectManagement`等）→`service-gotchas.md`- PE groupId和DNS区域映射（主要业务）→`service-gotchas.md`-PE/security/naming通用模式→`azure-common-patterns.md`-AI/Data业务配置指南→`ai-data.md`>如果不确定稳定信息，请与MS文档重新验证。但是没有必要每次都取回。

---

未知服务回退工作流

当用户请求v1范围（`ai-data.md`）未涵盖的服务时：

1. **通知用户**：“此服务不在v1默认范围内。它将通过参考微软文档在最大程度上生成。”
2. **获取API版本**：以`https://learn.microsoft.com/en-us/azure/templates/microsoft.{provider}/{resourceType}`格式构造URL并获取
3. **识别资源type/required属性**：从获取的文档中确认资源类型和所需属性
4. **验证PE映射**：获取`https://learn.microsoft.com/en-us/azure/private-link/private-endpoint-dns`以确认groupId/DNSZone
5. **应用通用模式**：从`azure-common-patterns.md`应用security/network/naming模式
6. **Write Bicep**：根据以上信息生成模块
7. **交给审稿人**：用`az bicep build`验证编译

##输入信息以下资料必须在第一阶段完成后定稿：```
- services: [Service list + SKU]
- networking: Whether private_endpoint is used
- resource_group: Resource group name
- location: Deployment location (confirmed with user in Phase 1)
- subscription_id: Azure subscription ID
```
##输出文件结构```
<project-name>/
├── main.bicep              # Main orchestration — module calls and parameter passing
├── main.bicepparam         # Parameter file — environment-specific values, excluding sensitive info
└── modules/
    ├── network.bicep           # VNet, Subnet (including pe-subnet)
    ├── ai.bicep                # AI services (configured per user requirements)
    ├── storage.bicep           # ADLS Gen2 (isHnsEnabled: true required)
    ├── fabric.bicep            # Microsoft Fabric Capacity (only when needed)
    ├── keyvault.bicep          # Key Vault
    ├── monitoring.bicep        # Application Insights, Log Analytics (only needed for Hub-based configurations)
    └── private-endpoints.bicep # All PEs + Private DNS Zones + VNet Links + DNS Zone Groups
```
##模块职责

# # #`network.bicep`- VNet - CIDR作为参数接收（避免与客户环境中的现有地址空间冲突）
- pe-subnet -`privateEndpointNetworkPolicies: 'Disabled'`必选参数
-根据需要通过参数处理额外的子网

# # #`ai.bicep`- **微软Foundry资源** (`Microsoft.CognitiveServices/accounts`,`kind: 'AIServices'`) -顶级AI资源
—必选参数`customSubDomainName: foundryName`—**创建完成后不能修改。如果省略，则必须删除资源并重新创建**
-`identity: { type: 'SystemAssigned' }`-`allowProjectManagement: true`—模型部署（`Microsoft.CognitiveServices/accounts/deployments`）—在Foundry资源级别执行
- **⚠️Foundry Project** (`Microsoft.CognitiveServices/accounts/projects`) - **必须创建为子资源**
-资源类型：`Microsoft.CognitiveServices/accounts/projects`（永远不要创建一个独立的`accounts`资源）
-在二头肌中使用`parent: foundryAccount`-错误示例：创建一个项目作为一个单独的`kind: 'AIServices'`帐户→在门户中无法识别
—正确示例：    ```bicep
    resource foundryProject 'Microsoft.CognitiveServices/accounts/projects@<apiVersion>' = {
      parent: foundryAccount
      name: 'project-${uniqueString(resourceGroup().id)}'
      location: location
      kind: 'AIServices'
      properties: {}
    }
    ```
- **Azure AI搜索** -语义排名，矢量搜索配置
-只有当用户显式请求或需要MLtraining/open-source模型时，才应该考虑基于hub的（`Microsoft.MachineLearningServices/workspaces`）。对于标准的AI/RAG工作负载，Foundry （AIServices）是默认选择

**⛔CognitiveServices禁止属性：**
-`apiProperties.statisticsEnabled`-该属性不存在。永远不要使用它。导致部署时出现`ApiPropertiesInvalid`错误
-`apiProperties.qnaAzureSearchEndpointId`-仅QnA Maker。不与铸造一起使用
—不要随意添加未经验证的属性到`properties.apiProperties`# # #`storage.bicep`- ADLS Gen2:`isHnsEnabled: true`←**永远不要省略这个**
-容器：生的、加工的、管理的（或按要求）
-`allowBlobPublicAccess: false`,`minimumTlsVersion: 'TLS1_2'`# # #`keyvault.bicep`-`enableRbacAuthorization: true`（不使用访问策略模型）
-`enableSoftDelete: true`、`softDeleteRetentionInDays: 90`——`enablePurgeProtection: true`# # #`monitoring.bicep`-日志分析工作区
-应用程序洞察（只需要基于hub的配置-不需要Foundry AIServices）# # #`private-endpoints.bicep`-每项服务三件套：
1.`Microsoft.Network/privateEndpoints`（放置在pe-subnet中）
2.`Microsoft.Network/privateDnsZones`+ VNet Link （`registrationEnabled: false`）  3. `Microsoft.Network/privateEndpoints/privateDnsZoneGroups`
—各服务DNS区域对应关系请参考`references/service-gotchas.md`**⚠️Foundry/AIServicesPE DNS规则：**
—PE组id:`account`—DNS区域组必须包含**2个区域**：
1.`privatelink.cognitiveservices.azure.com`2.`privatelink.openai.azure.com`—只包含一个会导致OpenAI API调用DNS解析失败→连接错误

**⚠️ADLS Gen2 (isHnsEnabled: true) PE规则：**
-需要2个pe：
1.`blob`→`privatelink.blob.core.windows.net`2.`dfs`→`privatelink.dfs.core.windows.net`-没有DFS PE，数据湖操作（文件系统创建，目录操作）将失败

###`rbac.bicep`（或inline in main.bicep）

**⚠️RBAC角色分配-永远不要忽略**

**任何具有托管身份（`identity.type: 'SystemAssigned'`）的服务必须创建RBAC角色分配
没有角色分配的身份会导致服务间身份验证失败。
这不是可选的-这是一个**强制性的项目**。
遗漏将在第3阶段评审中报告为CRITICAL。

-所需的RBAC映射：|源服务|目标服务|角色|角色定义ID ||------------|-----------|------|-------------------|
|铸造|存储|`Storage Blob Data Contributor`|`ba92f5b4-2d11-453d-a403-e96b0029c9fe`|
|铸造|人工智能搜索|`Search Index Data Contributor`|`8ebe5a00-799e-43f5-93ac-243d3dce84a7`|
|铸造|人工智能搜索|`Search Service Contributor`|`7ca78c08-252a-4471-8644-bb5ff32d4ba0`|
|应用服务|密钥库|`Key Vault Secrets User`|`4633458b-17de-408a-b874-0445c86b69e6`|
| AKS (kubeletiidentity) | ACR |`AcrPull`|`7f951dda-4ed3-4680-a7ca-43fe172d538d`|
|数据工厂|存储|`Storage Blob Data Contributor`|`ba92f5b4-2d11-453d-a403-e96b0029c9fe`|
|数据工厂|密钥库|`Key Vault Secrets User`|`4633458b-17de-408a-b874-0445c86b69e6`|
|数据块|存储|`Storage Blob Data Contributor`|`ba92f5b4-2d11-453d-a403-e96b0029c9fe`|

b> **AKS特殊规则**:AKS使用`identityProfile.kubeletidentity.objectId`，而不是`identity.principalId`。```bicep
// RBAC Example — Foundry → Storage Blob Data Contributor
resource foundryStorageRole 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(storageAccount.id, foundry.id, 'ba92f5b4-2d11-453d-a403-e96b0029c9fe')
  scope: storageAccount
  properties: {
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', 'ba92f5b4-2d11-453d-a403-e96b0029c9fe')
    principalId: foundry.identity.principalId
    principalType: 'ServicePrincipal'
  }
}
```
SQL Server规则
—**密码管理**：在main中声明`@secure() param sqlAdminPassword string`。并将其传递给模块
—不要在模块内部生成`newGuid()`—重新部署时密码会改变
-作为密钥库秘密存储，以便在部署后检索
—**认证方式**：默认为`administrators.azureADOnlyAuthentication: true`-许多组织策略（MCAPS等）阻止独立的SQL身份验证
—AAD-only认证+ Managed Identity是最安全的配置

网络秘密处理
—**VPN网关共享密钥**:`@secure() param vpnSharedKey string`—`@secure()`为必选
-永远不要在`.bicepparam`中包含明文VPN密钥-在部署时提供或使用密钥库参考
—此规则与SQL密码相同
—**适用于**:VPN共享密钥、ExpressRoute授权密钥、Wi-Fi PSK等所有网络秘密
-模块参数还必须包含`@secure()`装饰器###⚠️网络隔离一致性规则
—配置`publicNetworkAccess: 'Disabled'`时，**必须**创建该服务对应的PE
—在没有PE的情况下，将publicNetworkAccess设置为Disabled会导致服务不可达→部署后不可用
-第3阶段审查员必须将这种不一致报告为“CRITICAL”
—当发现不一致时：添加PE模块或将publicNetworkAccess恢复为Enabled

强制编码原则

命名约定```bicep
// Use uniqueString to prevent naming collisions — always required
param foundryName string = 'foundry-${uniqueString(resourceGroup().id)}'
param searchName string = 'srch-${uniqueString(resourceGroup().id)}'
param storageName string = 'st${uniqueString(resourceGroup().id)}'  // No special characters allowed
param keyVaultName string = 'kv-${uniqueString(resourceGroup().id)}'
```
b> **⚠️需要`customSubDomainName`（Foundry， Cognitive Services等）的资源必须包括`uniqueString()`.**
>静态字符串（例如，`'my-rag-chatbot'`）可能已经被另一个租户使用，从而导致部署失败。
>同样适用于Foundry项目名称-`'project-${uniqueString(resourceGroup().id)}'`###网络隔离```bicep
// Required for all services when using Private Endpoints
publicNetworkAccess: 'Disabled'
networkAcls: {
  defaultAction: 'Deny'
  ipRules: []
  virtualNetworkRules: []
}
```
依赖管理```bicep
// Use implicit dependencies via resource references instead of explicit dependsOn
resource aiProject '...' = {
  properties: {
    hubResourceId: aiHub.id  // Reference to aiHub → aiHub is automatically deployed first
  }
}
```
# # #安全```bicep
// Use Key Vault references for sensitive values — never store plaintext in parameter files
@secure()
param adminPassword string  // Do not put plaintext values in main.bicepparam
```
###代码注释```bicep
// Microsoft Foundry resource — kind: 'AIServices'
// customSubDomainName: Required, globally unique. Cannot be changed after creation — if omitted, resource must be deleted and recreated
// allowProjectManagement: true is required or Foundry Project creation will fail
// Replace apiVersion with the latest version fetched in Step 0
resource foundry 'Microsoft.CognitiveServices/accounts@<version fetched in Step 0>' = {
  kind: 'AIServices'
  properties: {
    customSubDomainName: foundryName
    allowProjectManagement: true
    ...
  }
}
```
⚠️二头肌代码质量验证（生成后需要）

**模块声明验证：**
—检查每个模块块中的`name:`属性没有重复
—正确示例：`name: 'deploy-sql'`—错误示例：`name: 'name: 'deploy-sql'`（重复名称：→编译错误）

**重复属性预防：**
—如果相同的属性名在单个资源块中出现多次，则会导致编译错误
-在VPN网关（`gatewayType`），防火墙，AKS等复杂资源中尤其常见。
—在“`az bicep build`”输出中查看是否为“`BCP025: The property "xxx" is declared multiple times`”

**`az bicep build`必须运行：**
—生成所有Bicep文件后，始终运行`az bicep build --file main.bicep`-修复错误并重新编译
—在MS Docs中验证API版本后，可以忽略警告（BCP081等）

# #主要。二头肌基础结构```bicep
// ============================================================
// Azure [Project Name] Infrastructure — main.bicep
// Generated: [Date]
// ============================================================

targetScope = 'resourceGroup'

// ── Common Parameters ─────────────────────────────────────
param location string   // Location confirmed in Phase 1 — do not hardcode
param projectPrefix string
param vnetAddressPrefix string    // ← Confirm with user. Prevent conflicts with existing networks
param peSubnetPrefix string       // ← PE-dedicated subnet CIDR within the VNet

// ── Network ───────────────────────────────────────────────
module network './modules/network.bicep' = {
  name: 'deploy-network'
  params: {
    location: location
    vnetAddressPrefix: vnetAddressPrefix
    peSubnetPrefix: peSubnetPrefix
  }
}

// ── AI/Data Services ──────────────────────────────────────
module ai './modules/ai.bicep' = {
  name: 'deploy-ai'
  params: {
    location: location
    // Add separate params if regions differ per service — verify available regions in MS Docs
  }
  dependsOn: [network]
}

// ── Storage ───────────────────────────────────────────────
module storage './modules/storage.bicep' = {
  name: 'deploy-storage'
  params: {
    location: location
  }
}

// ── Key Vault ─────────────────────────────────────────────
module keyVault './modules/keyvault.bicep' = {
  name: 'deploy-keyvault'
  params: {
    location: location
  }
}

// ── Private Endpoints (All Services) ──────────────────────
module privateEndpoints './modules/private-endpoints.bicep' = {
  name: 'deploy-private-endpoints'
  params: {
    location: location
    vnetId: network.outputs.vnetId
    peSubnetId: network.outputs.peSubnetId
    foundryId: ai.outputs.foundryId
    searchId: ai.outputs.searchId
    storageId: storage.outputs.storageId
    keyVaultId: keyVault.outputs.keyVaultId
  }
}

// ── Outputs ───────────────────────────────────────────────
output vnetId string = network.outputs.vnetId
output foundryEndpoint string = ai.outputs.foundryEndpoint
output searchEndpoint string = ai.outputs.searchEndpoint
```
# #主要。双元基结构```bicep
using './main.bicep'

param location = '<Location confirmed in Phase 1>'
param projectPrefix = '<Project prefix>'
// Do not put sensitive values here — use Key Vault references
// Set regions after verifying per-service availability in MS Docs
```
### @secure（）参数处理

当`.bicepparam`文件包含`using`指令时，其他`--parameters`标志不能与`az deployment`一起使用。
因此，`@secure()`参数必须遵循以下规则：

—**尽量设置默认值**:`@secure() param password string = newGuid()`- **如果@secure（）参数需要用户输入**：生成一个JSON参数文件（`main.parameters.json`）旁边而不是使用`.bicepparam`—**千万不要这样做**：生成同时使用`.bicepparam`和`--parameters key=value`的命令

##常见错误清单

完整的清单在`references/service-gotchas.md`。主要总结:

|项目|❌错误|✅正确||------|--------|----------|
| ADLS Gen2 |`isHnsEnabled`省略|`isHnsEnabled: true`|
| PE子网|未设置策略|`privateEndpointNetworkPolicies: 'Disabled'`|
| PE配置| PE只创建| PE + DNS区域+ VNet Link + DNS区域组|
|铸造|`kind: 'OpenAI'`|`kind: 'AIServices'`+`allowProjectManagement: true`|
| Foundry |`customSubDomainName`省略|`customSubDomainName: foundryName`-创建|后不能修改
|铸造项目|未创建|必须始终与铸造资源|创建一个集合
|用于标准AI |仅在用户明确请求或ML/open-source型号需要|时使用
|公网|未配置|`publicNetworkAccess: 'Disabled'`|
|存储名称|包含连字符|只能包含小写+数字，`uniqueString()`推荐使用|
| API版本|从之前的值|拷贝从MS Docs (Dynamic) |获取
|区域|硬编码|参数+ MS Docs (Dynamic) |验证可用性

##一代完成后肱二头肌生成完成后：
1. 向用户提供生成的文件列表和每个文件的角色的摘要报告
2. 立即转入第三阶段（二头肌审查员）
3. 审稿人按照`references/bicep-reviewer.md`指导方针进行自动审查和更正