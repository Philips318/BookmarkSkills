# Domain Pack:AI/Data（v1）

专门用于AzureAI/Data工作负载的服务配置指南。
v1范围：代工，AI搜索，ADLS Gen2，密钥库，织物，ADF,VNet/PE.要求properties/common错误→`service-gotchas.md`>动态信息（API版本、SKU、地区）→`azure-dynamic-sources.md`>常用模式（PE、安全性、命名）→`azure-common-patterns.md`---

# # 1。Microsoft Foundry （cognitiveeservices）

资源层次结构```
Microsoft.CognitiveServices/accounts (kind: 'AIServices')
├── /projects          — Foundry Project (required for portal access)
└── /deployments       — Model deployments (GPT-4o, embedding, etc.)
```
二头肌核心结构```bicep
// Foundry resource
resource foundry 'Microsoft.CognitiveServices/accounts@<fetch>' = {
  name: foundryName
  location: location
  kind: 'AIServices'
  sku: { name: '<confirm with user>' }               // ← SKU confirmed after MS Docs check in Phase 1
  identity: { type: 'SystemAssigned' }
  properties: {
    customSubDomainName: foundryName  // ← Required, globally unique. Cannot change after creation — must delete and recreate if omitted
    allowProjectManagement: true
    publicNetworkAccess: 'Disabled'
    networkAcls: { defaultAction: 'Deny' }
  }
}

// Foundry Project — Must be created as a set with Foundry
resource project 'Microsoft.CognitiveServices/accounts/projects@<fetch>' = {
  parent: foundry
  name: '${foundryName}-project'
  location: location
  sku: { name: '<same as parent>' }
  kind: 'AIServices'
  identity: { type: 'SystemAssigned' }
  properties: {}
}

// Model deployment — At Foundry resource level
resource deployment 'Microsoft.CognitiveServices/accounts/deployments@<fetch>' = {
  parent: foundry
  name: '<model-name>'                              // ← Confirmed with user in Phase 1
  sku: {
    name: '<deployment-type>'                        // ← GlobalStandard, Standard, etc. — MS Docs fetch
    capacity: <confirm with user>                    // ← Capacity units — verify available range from MS Docs
  }
  properties: {
    model: {
      format: 'OpenAI'
      name: '<model-name>'                           // ← Must verify availability (fetch)
      version: '<fetch>'                             // ← Version also fetched
    }
  }
}
```
>`@<fetch>`：从`azure-dynamic-sources.md`中的url验证API版本。
>型号name/version/deploymenttype/capacity：所有动态-在第1阶段获取MS文档后与用户确认。

---

# # 2。Azure人工智能搜索

二头肌核心结构```bicep
resource search 'Microsoft.Search/searchServices@<fetch>' = {
  name: searchName
  location: location
  sku: { name: '<confirm with user>' }
  identity: { type: 'SystemAssigned' }
  properties: {
    hostingMode: 'default'
    publicNetworkAccess: 'disabled'
    semanticSearch: '<confirm with user>'    // disabled | free | standard — verify in MS Docs
  }
}
```
设计注意事项

- PE支持：基本SKU或更高版本（在MS文档中验证最新约束）
-语义排名：通过`semanticSearch`属性激活(`disabled`|`free`|`standard`) -验证每个sku在MS文档中的支持
-矢量搜索：支持付费sku（在MS Docs中验证）
-通常与Foundry一起用于RAG配置

---

# # 3。ADLS Gen2（储存帐户）

二头肌核心结构```bicep
resource storage 'Microsoft.Storage/storageAccounts@<fetch>' = {
  name: storageName        // Lowercase+numbers only, no hyphens
  location: location
  kind: 'StorageV2'
  sku: { name: 'Standard_LRS' }
  properties: {
    isHnsEnabled: true                 // ← Never omit this
    accessTier: 'Hot'
    allowBlobPublicAccess: false
    minimumTlsVersion: 'TLS1_2'
    publicNetworkAccess: 'Disabled'
    networkAcls: { defaultAction: 'Deny' }
  }
}

// Container
resource container 'Microsoft.Storage/storageAccounts/blobServices/containers@<fetch>' = {
  name: '${storage.name}/default/raw'
}
```
设计注意事项

—`isHnsEnabled`创建后不能修改→如果省略，必须重新创建资源
PE：可能同时需要`blob`和`dfs`PE，具体取决于用例
—常用容器：`raw`、`processed`、`curated`---

# # 4。微软织物

二头肌核心结构```bicep
resource fabric 'Microsoft.Fabric/capacities@<fetch>' = {
  name: fabricName
  location: location
  sku: { name: '<confirm with user>', tier: 'Fabric' }
  properties: {
    administration: {
      members: [ '<admin-email>' ]    // ← Required, deployment fails without it
    }
  }
}
```
设计注意事项

-只有容量可以通过二头肌提供
-工作区、湖屋、仓库等必须在门户中手动创建
-与用户确认管理邮件（`ask_user`）

在阶段1添加时需要的确认项

当Fabric在会话中添加时，在更新图之前必须通过ask_user确认以下项目：

- [] **SKU/Capacity**: F2, F4, F8，…-从MS文档中获取可用sku后提供选择
-[] **管理。members**: Admin电子邮件—没有它部署将失败

>不要随意包含用户没有指定的子工作负载（OneLake、数据管道、仓库等）。只有容量可以通过Bicep进行配置。

---

# # 5。Azure数据工厂

二头肌核心结构```bicep
resource adf 'Microsoft.DataFactory/factories@<fetch>' = {
  name: adfName
  location: location
  identity: { type: 'SystemAssigned' }
  properties: {
    publicNetworkAccess: 'Disabled'
  }
}
```
设计注意事项

-自托管集成运行时需要在Bicep外部手动设置
—主要用于本地数据摄取场景
—PE groupId:`dataFactory`---

# # 6。反洗钱/人工智能中心（机器学习服务）

###何时使用```
Decision Rule:
├─ General AI/RAG → Use Foundry (AIServices)
└─ ML training, open-source models needed → Consider AI Hub
    └─ Only when the user explicitly requests it
```
二头肌核心结构```bicep
resource hub 'Microsoft.MachineLearningServices/workspaces@<fetch>' = {
  name: hubName
  location: location
  kind: 'Hub'
  sku: { name: '<confirm with user>', tier: '<confirm with user>' }  // e.g., Basic/Basic — verify available SKUs in MS Docs
  identity: { type: 'SystemAssigned' }
  properties: {
    friendlyName: hubName
    storageAccount: storage.id
    keyVault: keyVault.id
    applicationInsights: appInsights.id    // Required for Hub
    publicNetworkAccess: 'Disabled'
  }
}
```
AI Hub Dependencies

使用Hub时需要的其他资源：
—存储帐户
-密钥库
-应用程序洞察+日志分析工作区
-容器注册表（可选）

---

# # 7。常见的AI/Data架构组合

聊天机器人```
Foundry (AIServices) + Project
├── <chat-model> (chat)              — Confirmed after availability check in Phase 1
├── <embedding-model> (embedding)    — Confirmed after availability check in Phase 1
├── AI Search (vector + semantic)
├── ADLS Gen2 (document store)
└── Key Vault (secrets)
+ Full VNet/PE configuration
```
###数据平台```
Fabric Capacity (analytics)
├── ADLS Gen2 (data lake)
├── ADF (ingestion)
└── Key Vault (secrets)
+ VNet/PE configuration
```
