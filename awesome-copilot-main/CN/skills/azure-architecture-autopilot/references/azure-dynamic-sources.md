# Azure动态源注册表

这个文件只管理频繁变化的信息的源（url）。
实际值（API版本，SKU，区域等）此处不记录。
在生成Bicep之前，始终获取下面的url以验证最新信息。

---

# # 1。Bicep API版本（Always Must Fetch）

每个服务的MS Docs Bicep参考。在使用之前，请从这些url中验证最新的稳定apiVersion。

|服务| MS Docs URL ||---------|-------------|
| cognitiveesservices (Foundry/OpenAI) |https://learn.microsoft.com/en-us/azure/templates/microsoft.cognitiveservices/accounts|
| AI搜索|https://learn.microsoft.com/en-us/azure/templates/microsoft.search/searchservices|
|存储帐户|https://learn.microsoft.com/en-us/azure/templates/microsoft.storage/storageaccounts|
|密钥库|https://learn.microsoft.com/en-us/azure/templates/microsoft.keyvault/vaults|
|虚拟网络|https://learn.microsoft.com/en-us/azure/templates/microsoft.network/virtualnetworks|
|私有端点|https://learn.microsoft.com/en-us/azure/templates/microsoft.network/privateendpoints|
|私有DNS区域|https://learn.microsoft.com/en-us/azure/templates/microsoft.network/privatednszones|
|布料|https://learn.microsoft.com/en-us/azure/templates/microsoft.fabric/capacities|
|数据工厂|https://learn.microsoft.com/en-us/azure/templates/microsoft.datafactory/factories|
|应用洞察|https://learn.microsoft.com/en-us/azure/templates/microsoft.insights/components|
| ML工作空间（Hub） |https://learn.microsoft.com/en-us/azure/templates/microsoft.machinelearningservices/workspaces|

b> **也要始终验证子资源**:`accounts/projects`，`accounts/deployments`，`privateDnsZones/virtualNetworkLinks`等子资源可能具有与其父资源不同的API版本。按照父页面中的子资源链接进行验证。

不在上表中的服务

上面的表只包括v1作用域服务。对于其他服务，用这种格式构造URL并获取：```
https://learn.microsoft.com/en-us/azure/templates/microsoft.{provider}/{resourceType}
```
---

# # 2。模型可用性（当使用Foundry/OpenAI模型时需要）

验证模型名称在目标区域中是否可部署。不要依赖静态知识。

|验证方法| URL /命令||--------------------|---------------|
| MS Docs模型可用性|https://learn.microsoft.com/en-us/azure/ai-services/openai/concepts/models|
| Azure CLI（现有资源）|`az cognitiveservices account list-models --name "<NAME>" --resource-group "<RG>" -o table`|

>如果该型号在目标区域不可用→通知用户并建议可用的regions/alternative型号。未经用户同意，请勿替代。

---

# # 3。私有端点映射（添加新服务时）

PE groupId和DNS区域映射可以通过Azure更改。新增业务或需要验证时：

验证方法| URL ||--------------------|-----|
| PE DNS集成官方文档|https://learn.microsoft.com/en-us/azure/private-link/private-endpoint-dns|

>`service-gotchas.md`中的密钥服务映射是稳定的，但在添加新服务时总是从上面的URL重新验证。

---

# # 4。业务区域可用性

验证特定服务在特定区域是否可用：

验证方法| URL ||--------------------|-----|
| Azure按区域服务可用性|https://azure.microsoft.com/en-us/explore/global-infrastructure/products-by-region/|

---

# # 5。Azure更新（次要意识）

以下资料仅供参考。主要来源总是MS Docs官方文档。

|源| URL |目的||--------|-----|---------|
| Azure更新|https://azure.microsoft.com/en-us/updates/|服务变更感知|
| Azure新增功能|每服务文档新增页面|功能更改验证|

---

决策规则：何时获取？

|信息类型|必须获取？|基本原理|-----------------|-------------|-----------|
| API版本| **总是取** |频繁更改；设置错误会导致部署失败|
|型号可用性（名称，地区）| **总是获取** |因地区而异，经常更改|
| SKU列表| **始终获取** |可以更改每个服务|
|区域可用性| **始终获取** |每个服务区域支持频繁更改。始终验证用户指定的区域是否可用于服务|
| PE groupId和DNS区域|可以引用`service-gotchas.md`为v1密钥服务；**必须获取新的服务或复杂的配置（监视器等）** |密钥服务映射是稳定的，但new/complex服务是有风险的|
|必需的属性模式|首先参考文件|近乎不可变（isHnsEnabled等）|