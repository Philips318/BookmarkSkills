# Service gottchas （Stable）

每个服务总结**非直观的必需属性**、**常见错误**和**PE映射**。
这里只包括近乎不可变的模式。不包括API版本、SKU列表和区域等动态值。

---

# # 1。必备属性（若省略则为部署失败或功能问题）

|服务|必选属性|结果如果省略|备注| . ||---------|------------------|-------------------|-------|
| ADLS Gen2 |`isHnsEnabled: true`|成为常规Blob存储。不能反转|`kind: 'StorageV2'`需要|
|存储帐户|名称中无特殊characters/hyphens|部署失败|仅小写+数字，3 ~ 24个字符| . | . | . | . | . | . | . | . | . | . |
| Foundry (AIServices) |`customSubDomainName: foundryName`|不能创建项目，创建后不能更改→必须删除并重新创建资源|全局唯一值|
| Foundry (AIServices) |`allowProjectManagement: true`|无法创建Foundry Project |`kind: 'AIServices'`|
| Foundry (AIServices) |`identity: { type: 'SystemAssigned' }`|创建项目失败| |
|铸造项目|必须与铸造资源|一起创建，不能从门户|`accounts/projects`|使用
|密钥库|`enableRbacAuthorization: true`|混合访问策略使用风险| |
|密钥库|`enablePurgeProtection: true`|生产端| |必选
| Fabric容量|`administration.members`必选|部署失败|管理员邮箱|
| PE子网|`privateEndpointNetworkPolicies: 'Disabled'`| PE部署失败| |
| PE DNS区域|`registrationEnabled: false`(VNet Link) |可能存在的DNS冲突||
| PE配置| 3-component set (PE + DNS Zone + VNet Link + Zone Group) | PE存在DNS解析失败| |---

# # 2。PE组名和DNS区域映射（关键服务）

下面的映射是稳定的，但是在添加新服务时要重新从`azure-dynamic-sources.md`中的PE DNS集成文档进行验证。

|服务| groupId |私有DNS区域||---------|---------|-----------------|
| Azure OpenAI / cognitivesservices |`account`|`privatelink.cognitiveservices.azure.com`|
|⚠️（Foundry/AIServicesadditional） |`account`|`privatelink.openai.azure.com`←**两个区域必须包含在“DNS区域组”中。如果省略** |，OpenAI API DNS解析失败
| Azure AI搜索|`searchService`|`privatelink.search.windows.net`|
| Storage (Blob/ADLS) |`blob`|`privatelink.blob.core.windows.net`|
| Storage (DFS/ADLSGen2) |`dfs`|`privatelink.dfs.core.windows.net`|
|密钥库|`vault`|`privatelink.vaultcore.azure.net`|
| Azure ML / AI Hub |`amlworkspace`|`privatelink.api.azureml.ms`|
|容器注册表|`registry`|`privatelink.azurecr.io`|
| Cosmos DB (SQL) |`Sql`|`privatelink.documents.azure.com`|
| Azure缓存Redis |`redisCache`|`privatelink.redis.cache.windows.net`|
|数据工厂|`dataFactory`|`privatelink.datafactory.azure.net`|
|`Gateway`|`privatelink.azure-api.net`|
|事件中心|`namespace`|`privatelink.servicebus.windows.net`|
|服务总线|`namespace`|`privatelink.servicebus.windows.net`|
|监视器（AMPLS） |⚠️复杂配置-见|⚠️需要多个DNS区域-见|注**：当`isHnsEnabled: true`时，**需要`blob`和`dfs`pe **。
>—仅使用`blob`PE， Blob API可以工作，但是数据湖操作（文件系统创建、目录操作、`abfss://`协议）将失败。
> - DFS PE: groupId`dfs`, DNS Zone`privatelink.dfs.core.windows.net`>
> **⚠️Azure Monitor私有链路（AMPLS）注意**:Azure Monitor不能配置单个PE +单个DNS Zone。它通过Azure监视器专用链接作用域（AMPLS）连接，并且需要所有**5个DNS区域**：
> -`privatelink.monitor.azure.com`> -`privatelink.oms.opinsights.azure.com`> -`privatelink.ods.opinsights.azure.com`> -`privatelink.agentsvc.azure-automation.net`> -`privatelink.blob.core.windows.net`（用于日志分析数据摄取）
>
这个映射是复杂的，并且可能会改变，所以在配置Monitor PE时总是获取并验证MS Docs：
>https://learn.microsoft.com/en-us/azure/azure-monitor/logs/private-link-configure---

# # 3。常见错误清单

|项目|❌错误示例|✅正确示例||------|---------------------|-------------------|
| ADLS Gen2 HNS |`isHnsEnabled`省略或`false`|`isHnsEnabled: true`|
| PE子网|未设置策略|`privateEndpointNetworkPolicies: 'Disabled'`|
| DNS区域组|只创建PE | PE + DNS区域+ VNet Link + DNS区域组|
|代工资源|`kind: 'OpenAI'`|`kind: 'AIServices'`+`allowProjectManagement: true`|
|铸造厂资源|`customSubDomainName`省略|`customSubDomainName: foundryName`-创建|后不能修改
|铸造厂项目|只有铸造厂存在，没有|项目必须创建一个集|
|密钥库auth |访问策略|`enableRbacAuthorization: true`|
|公网|未配置|`publicNetworkAccess: 'Disabled'`|
|存储名称|`st-my-storage`|`stmystorage`或`st${uniqueString(...)}`|
| API版本|从以前的conversation/error|复制
|区域|硬编码（`'eastus'`） |作为参数（`param location`） |传递
|敏感值|`.bicepparam`|`@secure()`+密钥库引用|

---

# # 4。服务关系决策规则

描述为默认选择规则，而不是绝对决定。Foundry vs Azure OpenAI vs AI Hub```
Default rules:
├─ AI/RAG workloads → Use Microsoft Foundry (kind: 'AIServices')
│   ├─ Create Foundry resource + Foundry Project as a set
│   └─ Model deployment is performed at the Foundry resource level (accounts/deployments)
│
├─ ML/open-source model training needed → Consider AI Hub (MachineLearningServices)
│   └─ Only when the user explicitly requests it or features not supported in Foundry are needed
│
└─ Standalone Azure OpenAI resource →
    Consider only when the user explicitly requests it or
    official documentation requires a separate resource
```
这些规则是反映当前MS建议的默认选择指南。
Azure产品关系可能会改变，所以如果不确定，请检查微软文档。

# # #监控```
Default rules:
├─ Foundry (AIServices) → Application Insights not required
└─ AI Hub (MachineLearningServices) → Application Insights + Log Analytics required
```
