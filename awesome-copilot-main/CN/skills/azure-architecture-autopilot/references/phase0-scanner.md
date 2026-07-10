#阶段0：现有资源扫描器

该文件包含阶段0的详细说明。当用户请求分析现有Azure资源（路径B）时，读取并遵循此文件。

扫描结果被可视化为架构图，随后来自用户的自然语言修改请求被路由到阶段1。

> **🚨输出存储路径规则**：所有输出（scan JSON, diagram HTML, Bicep code）必须保存在当前工作目录(cwd)**下的**项目文件夹中。永远不要将它们保存在`~/.copilot/session-state/`中。会话状态目录是一个临时空间，可以在会话结束时删除。

---

步骤1:Azure登录+扫描范围选择

1-A：验证Azure登录```powershell
az account show 2>&1
```
—已登录→继续执行步骤1 ~ b
—未登录→执行命令`az login`### 1-B：订阅选择（支持多种选择）```powershell
az account list --output json
```
将订阅列表显示为`ask_user`选项。**可选择多个订阅：**```
ask_user({
  question: "Please select the Azure subscription(s) to analyze. (You can add more one at a time for multiple selections)",
  choices: [
    "sub-002 (Current default subscription) (Recommended)",
    "sub-001",
    "Analyze all subscriptions above"
  ]
})
```
—选中单个订阅→只扫描该订阅
-选中“分析全部”→扫描所有订阅
-如果用户需要额外订阅→再次使用ask_user添加更多订阅

1-C：扫描范围选择（支持多个RG选择）```
ask_user({
  question: "What scope of Azure resources would you like to analyze?",
  choices: [
    "Specify a particular resource group (Recommended)",
    "Select multiple resource groups",
    "All resource groups in the current subscription"
  ]
})
```
—**指定RG**→从RG列表中选择或手动输入
—**多个RGs**→重复ask_user，一次添加一个RGs。当用户说“够了”时停止。
或者，用户可以输入多个以逗号分隔的RGs（例如，`rg-prod, rg-dev, rg-network`）。
- **整个订阅**→`az group list`→扫描所有RGs（如果有很多资源可能需要时间警告）

**支持多个订阅+多个rg组合：**
-订阅A中的rg-prod +订阅B中的rg-network→同时扫描并显示在一个图中

---

##图表层次-显示多个Subscriptions/RGs**单订阅+单RG**：与以前相同（仅限VNet边界）
**多个RG（相同订阅）**：每个RG虚线边界
**多订阅**：订阅> RG的两级边界

在图JSON中传递层次信息：**添加`subscription`和`resourceGroup`字段到服务JSON:**```json
{
  "id": "foundry",
  "name": "foundry-xxx",
  "type": "ai_foundry",
  "subscription": "sub-002",
  "resourceGroup": "rg-prod",
  "details": [...]
}
```
**通过`--hierarchy`参数传递层次信息：**```
--hierarchy '[{"subscription":"sub-002","resourceGroups":["rg-prod","rg-dev"]},{"subscription":"sub-001","resourceGroups":["rg-network"]}]'
```
基于这些信息，图表脚本将：
-多个RG→用虚线边界表示每个RG为一个集群（标签：RG名称）
-多个订阅→在更大的订阅边界内嵌套RG边界
—VNet边界显示在VNet所属的RG内部

---

##步骤2：资源扫描

**🚨az CLI输出原则：**
—az命令行输出必须**保存到文件**中，然后使用`view`读取。直接终端输出可能被截断。
-每个PowerShell调用捆绑**不超过3个az命令**。捆绑太多可能会导致超时。
—使用`--query`JMESPath只提取需要的字段，减少输出大小。```powershell
# ✅ Correct approach — Save to file then read
az resource list -g "<RG>" --query "[].{name:name,type:type,kind:kind,location:location}" -o json | Set-Content -Path "$outDir/resources.json"

# ❌ Wrong approach — Direct terminal output (may be truncated)
az resource list -g "<RG>" -o json
```
### 2-A：列出所有资源+显示给用户```powershell
$outDir = "<project-name>/azure-scan"
New-Item -ItemType Directory -Path $outDir -Force | Out-Null

# Step 1: Basic resource list (name, type, kind, location)
az resource list -g "<RG>" --query "[].{name:name,type:type,kind:kind,location:location,id:id}" -o json | Set-Content "$outDir/resources.json"
```
**🚨读取resources.json后，必须立即向用户显示完整的资源列表表：**```
📋 rg-<RG> Resource List (N resources)

┌─────────────────────────┬──────────────────────────────────────────────┬─────────────────┐
│ Name                    │ Type                                         │ Location        │
├─────────────────────────┼──────────────────────────────────────────────┼─────────────────┤
│ my-storage              │ Microsoft.Storage/storageAccounts             │ koreacentral    │
│ my-keyvault             │ Microsoft.KeyVault/vaults                    │ koreacentral    │
│ ...                     │ ...                                          │ ...             │
└─────────────────────────┴──────────────────────────────────────────────┴─────────────────┘

⏳ Retrieving detailed information...
```
在进行详细查询之前，先显示这个表。不要让用户在不知道有什么资源存在的情况下等待。

2-B：动态详细查询-基于resources.json**根据在resources.json.**中找到的资源类型动态确定详细的查询命令

不要使用硬编码的命令列表。只执行resources.json中存在的类型的命令，从下面的映射表中选择。

**类型→详细查询命令映射：**

|输入resources.json|详细查询命令|输出文件||---|---|---|
|`Microsoft.Network/virtualNetworks`|`az network vnet list -g "<RG>" --query "[].{name:name,addressSpace:addressSpace.addressPrefixes,subnets:subnets[].{name:name,prefix:addressPrefix,pePolicy:privateEndpointNetworkPolicies}}" -o json`|`vnets.json`|
|`Microsoft.Network/privateEndpoints`|`az network private-endpoint list -g "<RG>" --query "[].{name:name,subnetId:subnet.id,targetId:privateLinkServiceConnections[0].privateLinkServiceId,groupIds:privateLinkServiceConnections[0].groupIds,state:provisioningState}" -o json`|`pe.json`|
|`Microsoft.Network/networkSecurityGroups`|`az network nsg list -g "<RG>" --query "[].{name:name,location:location,subnets:subnets[].id,nics:networkInterfaces[].id}" -o json`|`nsg.json`|
|`Microsoft.CognitiveServices/accounts`|`az cognitiveservices account list -g "<RG>" --query "[].{name:name,kind:kind,sku:sku.name,endpoint:properties.endpoint,publicAccess:properties.publicNetworkAccess,location:location}" -o json`|`cognitive.json`|
|`Microsoft.Search/searchServices`|`az search service list -g "<RG>" --query "[].{name:name,sku:sku.name,publicAccess:properties.publicNetworkAccess,semanticSearch:properties.semanticSearch,location:location}" -o json 2>$null`|`search.json`|
|`Microsoft.Compute/virtualMachines`|`az vm list -g "<RG>" --query "[].{name:name,size:hardwareProfile.vmSize,os:storageProfile.osDisk.osType,location:location,nicIds:networkProfile.networkInterfaces[].id}" -o json`|`vms.json`|
|`Microsoft.Storage/storageAccounts`|`az storage account list -g "<RG>" --query "[].{name:name,sku:sku.name,kind:kind,hns:properties.isHnsEnabled,publicAccess:properties.publicNetworkAccess,location:location}" -o json`|`storage.json`|
|`Microsoft.KeyVault/vaults`|`az keyvault list -g "<RG>" --query "[].{name:name,location:location}" -o json 2>$null`|`keyvault.json`|
|`Microsoft.ContainerService/managedClusters`|`az aks list -g "<RG>" --query "[].{name:name,kubernetesVersion:kubernetesVersion,sku:sku,agentPoolProfiles:agentPoolProfiles[].{name:name,count:count,vmSize:vmSize},networkProfile:networkProfile.networkPlugin,location:location}" -o json`|`aks.json`|
|`Microsoft.Web/sites`|`az webapp list -g "<RG>" --query "[].{name:name,kind:kind,sku:appServicePlan,state:state,defaultHostName:defaultHostName,httpsOnly:httpsOnly,location:location}" -o json`|`webapps.json`|
|`Microsoft.Web/serverFarms`|`az appservice plan list -g "<RG>" --query "[].{name:name,sku:sku.name,tier:sku.tier,kind:kind,location:location}" -o json`|`appservice-plans.json`|
|`Microsoft.DocumentDB/databaseAccounts`|`az cosmosdb list -g "<RG>" --query "[].{name:name,kind:kind,databaseAccountOfferType:databaseAccountOfferType,locations:locations[].locationName,publicAccess:publicNetworkAccess}" -o json`|`cosmosdb.json`|
|`Microsoft.Sql/servers`|`az sql server list -g "<RG>" --query "[].{name:name,fullyQualifiedDomainName:fullyQualifiedDomainName,publicAccess:publicNetworkAccess,location:location}" -o json`|`sql-servers.json`|
|`Microsoft.Databricks/workspaces`|`az databricks workspace list -g "<RG>" --query "[].{name:name,sku:sku.name,url:workspaceUrl,publicAccess:parameters.enableNoPublicIp.value,location:location}" -o json 2>$null`|`databricks.json`|
|`Microsoft.Synapse/workspaces`|`az synapse workspace list -g "<RG>" --query "[].{name:name,sqlAdminLogin:sqlAdministratorLogin,publicAccess:publicNetworkAccess,location:location}" -o json 2>$null`|`synapse.json`|
|`Microsoft.DataFactory/factories`|`az datafactory list -g "<RG>" --query "[].{name:name,publicAccess:publicNetworkAccess,location:location}" -o json 2>$null`|`adf.json`|
|`Microsoft.EventHub/namespaces`|`az eventhubs namespace list -g "<RG>" --query "[].{name:name,sku:sku.name,location:location}" -o json`|`eventhub.json`|
|`Microsoft.Cache/redis`|`az redis list -g "<RG>" --query "[].{name:name,sku:sku.name,port:port,sslPort:sslPort,publicAccess:publicNetworkAccess,location:location}" -o json`|`redis.json`|
|`Microsoft.ContainerRegistry/registries`|`az acr list -g "<RG>" --query "[].{name:name,sku:sku.name,adminUserEnabled:adminUserEnabled,publicAccess:publicNetworkAccess,location:location}" -o json`|`acr.json`|
|`Microsoft.MachineLearningServices/workspaces`|`az resource show --ids "<ID>" --query "{name:name,sku:sku,kind:kind,location:location,publicAccess:properties.publicNetworkAccess,hbiWorkspace:properties.hbiWorkspace,managedNetwork:properties.managedNetwork.isolationMode}" -o json`|`mlworkspace.json`|
|`Microsoft.Insights/components`|`az monitor app-insights component show -g "<RG>" --app "<NAME>" --query "{name:name,kind:kind,instrumentationKey:instrumentationKey,workspaceResourceId:workspaceResourceId,location:location}" -o json 2>$null`|`appinsights-<NAME>.json`|
|`Microsoft.OperationalInsights/workspaces`|`az monitor log-analytics workspace show -g "<RG>" -n "<NAME>" --query "{name:name,sku:sku.name,retentionInDays:retentionInDays,location:location}" -o json`|`log-analytics-<NAME>.json`|
|`Microsoft.Network/applicationGateways`|`az network application-gateway list -g "<RG>" --query "[].{name:name,sku:sku,location:location}" -o json`|`appgateway.json`|
|`Microsoft.Cdn/profiles`/`Microsoft.Network/frontDoors`|`az afd profile list -g "<RG>" --query "[].{name:name,sku:sku.name,location:location}" -o json 2>$null`|`frontdoor.json`|
|`Microsoft.Network/azureFirewalls`|`az network firewall list -g "<RG>" --query "[].{name:name,sku:sku,threatIntelMode:threatIntelMode,location:location}" -o json`|`firewall.json`|
|`Microsoft.Network/bastionHosts`|`az network bastion list -g "<RG>" --query "[].{name:name,sku:sku.name,location:location}" -o json`|`bastion.json`|**动态查询流程：**

1. 读`resources.json`2. 提取`type`字段的不同值
3. 只执行上面映射表中匹配类型的命令（跳过不存在的类型）。
4. 如果找到映射表中没有的类型→使用泛型查询：`az resource show --ids "<ID>" --query "{name:name,sku:sku,kind:kind,location:location,properties:properties}" -o json`5. 分批执行2-3个命令（不要一次全部运行）

### 2-C：模型部署查询（当存在认知服务时）```powershell
# Query model deployments for each Cognitive Services resource
az cognitiveservices account deployment list --name "<NAME>" -g "<RG>" --query "[].{name:name,model:properties.model.name,version:properties.model.version,sku:sku.name}" -o json | Set-Content "$outDir/<NAME>-deployments.json"
```
### 2-D：网卡+公网IP查询（虚拟机存在时）```powershell
az network nic list -g "<RG>" --query "[].{name:name,subnetId:ipConfigurations[0].subnet.id,privateIp:ipConfigurations[0].privateIPAddress,publicIpId:ipConfigurations[0].publicIPAddress.id}" -o json | Set-Content "$outDir/nics.json"
az network public-ip list -g "<RG>" --query "[].{name:name,ip:ipAddress,sku:sku.name}" -o json | Set-Content "$outDir/public-ips.json"
```
来自VNet：
-`addressSpace.addressPrefixes`→CIDR
-`subnets[].name`，`subnets[].addressPrefix`→子网信息
-`subnets[].privateEndpointNetworkPolicies`→PE策略

---

步骤3：推断资源之间的关系

自动推断扫描资源之间的关系（连接），为图构建连接JSON。

关系推理规则

**🚨如果没有足够的连接线，图表将变得毫无意义。推断出尽可能多的关系

####确认推断（直接从资源IDs/properties验证）

|关系类型|推理方法|连接类型||---|---|---|
| PE→服务|从PE的`privateLinkServiceId`|`private`|中提取服务ID
| PE→VNet |从PE的`subnet.id`|（表示为VNet边界）|中提取VNet
|`accounts/projects`|`api`|母资源
|虚拟机→网卡→子网|从网卡的`subnet.id`推断出VNet/Subnet| （VNet边界）|
| NSG→子网|检查NSG的`subnets[].id`|`network`|中已连接的子网
| NSG→网卡|检查NSG的`networkInterfaces[].id`|`network`|连接的虚拟机
|网卡→公共IP |从网卡`publicIPAddress.id`|（包括详细信息）|检查PIP
| Databricks→VNet |工作区的VNet注入配置| （VNet边界）|

####合理推断（同一RG内服务之间的公共模式）

|关系类型|推理条件|连接类型||---|---|---|
| Foundry→AI Search |两者存在于同一RG→Infer RAG连接|`api`（label：“RAG Search”）|
| Foundry→Storage |在同一个RG中存在→Infer data connection |`data`(label: " data ") |
| AI搜索→存储|两者存在于同一RG→推断索引连接|`data`(label: " indexing ") |
| Service→Key Vault | Key Vault存在于同一个RG→Infer secret management |`security`(label: "Secrets") |
| VM→Foundry/Search| VM + AI服务存在于同一RG→Infer API调用|`api`（标签：“API”）|
| DI→Foundry |文档智能+ Foundry存在于同一RG→推断OCR/extraction连接|`api`（标签：“OCR/Extract”） |
| ADF→存储| ADF +存储存在于同一RG→推断数据管道|`data`（标签：“ pipeline ”） |
| ADF→SQL | ADF + SQL存在于同一个RG→推断数据源|`data`(label: " source ") |
| Databricks→Storage |都存在于相同的RG→推断数据湖连接|`data`（标签：“数据湖”）|####推理后的用户确认

将推断的连接列表显示给用户并请求确认：```
> **⏳ Relationships between resources have been inferred** — Please verify if the following are correct.

Inferred connections:
- Foundry → AI Search (RAG Search)
- Foundry → Storage (Data)
- VM → Foundry (API Call)
- Document Intelligence → Foundry (OCR/Extract)

Does this look correct? Let me know if you'd like to add or remove any connections.
```
####无法推断的关系

可能存在无法使用上述规则推断的连接。用户可以自由地添加额外的连接。

模型部署查询（当Foundry资源存在时）```powershell
az cognitiveservices account deployment list --name "<FOUNDRY_NAME>" -g "<RG>" --query "[].{name:name,model:properties.model.name,version:properties.model.version,sku:sku.name}" -o json
```
将每个部署的模型名称、版本和SKU添加到Foundry节点的详细信息中。

---

##步骤4:services/connectionsJSON转换

将扫描结果转换为内置图表引擎的输入格式。

资源类型→图类型映射

| Azure资源类型|图类型||---|---|
|`Microsoft.CognitiveServices/accounts`（类型：AIServices） |`ai_foundry`|
|`Microsoft.CognitiveServices/accounts`（类型：OpenAI） |`openai`|
|`Microsoft.CognitiveServices/accounts`（类型：FormRecognizer） |`document_intelligence`|
|`Microsoft.CognitiveServices/accounts`（类型：TextAnalytics等）|`ai_foundry`（默认）|
|`Microsoft.CognitiveServices/accounts/projects`|`ai_foundry`|
|`Microsoft.Search/searchServices`|`search`|
|`Microsoft.Storage/storageAccounts`|`storage`|
|`Microsoft.KeyVault/vaults`|`keyvault`|
|`Microsoft.Databricks/workspaces`|`databricks`|
|`Microsoft.Sql/servers`|`sql_server`|
|`Microsoft.Sql/servers/databases`|`sql_database`|
|`Microsoft.DocumentDB/databaseAccounts`|`cosmos_db`|
|`Microsoft.Web/sites`|`app_service`|
|`Microsoft.ContainerService/managedClusters`|`aks`|
|`Microsoft.Web/sites`（类型：functionapp） |`function_app`|
|`Microsoft.Synapse/workspaces`|`synapse`|
|`Microsoft.Fabric/capacities`|`fabric`|
|`Microsoft.DataFactory/factories`|`adf`|
|`Microsoft.Compute/virtualMachines`|`vm`|
|`Microsoft.Network/privateEndpoints`|`pe`|
|`Microsoft.Network/virtualNetworks`|（表示为VNet边界-不包括在服务中）|
|`Microsoft.Network/networkSecurityGroups`|`nsg`|
|`Microsoft.Network/bastionHosts`|`bastion`|
|`Microsoft.OperationalInsights/workspaces`|`log_analytics`|
|`Microsoft.Insights/components`|`app_insights`|
|其他|`default`|

服务JSON构造规则```json
{
  "id": "resource name (lowercase, special characters removed)",
  "name": "actual resource name",
  "type": "determined from the mapping table above",
  "sku": "actual SKU (if available)",
  "private": true/false,  // true if a PE is connected
  "details": ["property1", "property2", ...]
}
```
**详细信息包括：**
-端点URL
-SKU/tierdetails
-种类（AIServices， OpenAI等）
-模型部署列表（Foundry）
-关键属性（isHnsEnabled， semanticSearch等）
——区域

VNet信息→`--vnet-info`参数

如果找到了VNet，通过`--vnet-info`将其显示在边界标签中：```
--vnet-info "10.0.0.0/16 | pe-subnet: 10.0.1.0/24 | <region>"
```
PE节点生成

如果发现PE，将每个PE单独添加为一个节点，并使用`private`类型连接到相应的服务：```json
{"id": "pe_<serviceId>", "name": "PE: <serviceName>", "type": "pe", "details": ["groupId: <groupId>", "<status>"]}
```
---

步骤5：图表生成+向用户展示

图表文件名：`<project-name>/00_arch_current.html`使用扫描的RG名称作为默认的项目名称：```
ask_user({
  question: "Please choose a project name. (This will be the folder name for scan results)",
  choices: ["<RG-name>", "azure-analysis"]
})
```
生成图表后，报告：```
## Current Azure Architecture

[Interactive Diagram — 00_arch_current.html]

Scanned Resources (N total):
[Summary table by resource type]

What would you like to change here?
- 🔧 Performance improvement ("it's slow", "increase throughput")
- 💰 Cost optimization ("reduce costs", "make it cheaper")
- 🔒 Security hardening ("add PE", "block public access")
- 🌐 Network changes ("separate VNet", "add Bastion")
- ➕ Add/remove resources ("add a VM", "delete this")
- 📊 Monitoring ("set up logs", "add alerts")
- 🤔 Diagnostics ("is this architecture OK?", "what's wrong?")
- Or just take the diagram and stop here
```
---

步骤6：修改对话→过渡到阶段1

当用户请求修改时，转换到阶段1 （phase1-advisor.md）。
这是路径B入口点，使用现有的扫描结果作为基线。

自然语言修改请求处理-澄清问题模式

问一些明确的问题，让用户模糊的要求更具体：

* * * *🔧性能

|用户请求|澄清问题示例||---|---|
|“慢”/“响应时间太长”|哪个服务慢？我们应该升级SKU还是改变区域？”|
|“我想增加吞吐量”|“我们应该增加哪个服务的吞吐量？”规模?增加DTU/RU?”|
|“人工智能搜索索引慢”|“我们应该添加分区吗？”把SKU升级到S2?”|

* * * *💰成本

|用户请求|澄清问题示例||---|---|
“我想降低成本”“我们应该降低哪种服务的成本？”SKU降级?清理未使用的资源？”|
“这个多少钱？”从MS文档中查找定价信息，并根据当前sku提供估计成本|
|“这是一个开发环境，所以要便宜”|“我们应该切换到Free/Basic层吗？”哪些服务?”|

* * * *🔒安全

|用户请求|澄清问题示例||---|---|
我们应该在没有pe的服务中添加pe吗？检查RBAC ?禁用publicNetworkAccess ?”|
|“我们是否应该对所有服务应用PE + publicNetworkAccess: Disabled ?”|
| “管理密钥” | “我们是否应该添加密钥库并将其与受管理身份连接？”|

* * * *🌐网络

|用户请求|澄清问题示例||---|---|
将PE“|”添加到哪个服务？我们应该立即将它们添加到所有服务中吗？”|
| “分隔VNet” | "我们应该分隔哪些子网？我们是否也应该加入核供应国集团？”|
| "Add Bastion“ | ”添加Azure Bastion供虚拟机访问。请指定子网CIDR。”|

**➕Add/Remove资源**

|用户请求|澄清问题示例||---|---|
| “添加虚拟机” | "多少？SKU什么?同样的联接吗?操作系统什么?”|
| "Add Fabric“ | ”什么SKU？管理邮箱是什么？”|
| “删除此” | "确定要删除[资源名]吗？连接的pe也将被移除。”|

* *📊Monitoring/Operations* *

|用户请求|澄清问题示例||---|---|
|“我们应该添加一个日志分析工作区并连接诊断设置吗？”|
|“设置警报”|“针对哪些指标？CPU ?出错率?响应时间?”|
|“将应用程序洞察”附加到哪个服务？应用程序服务吗?功能应用?”|

* *🔄Migration/Changes* *

|用户请求|澄清问题示例||---|---|
| “将区域” | "更改为哪个区域？我将核实该地区的所有服务是否可用。”|
| “切换SQL到Cosmos” | " Cosmos数据库API类型是什么？（SQL/MongoDB/Cassandra）我还可以提供数据迁移指南。|
| "Switch Foundry to Hub" | "Hub仅适用于MLtraining/open-source型号。让我来验证用例。”|

* *🤔Diagnostics/Questions* *

|用户请求|澄清问题示例||---|---|
b|“怎么了？”|分析当前配置（publicNetworkAccess打开，PE未连接，SKU不合适等）并提出改进建议
|“这个架构可以吗？”根据架构良好的框架（安全性、可靠性、性能、成本、操作）进行审查
|“PE连接是否正常？”|检查与`az network private-endpoint show`的连接状态，报告|
|不要过渡到第一阶段；提供00_arch_current.html路径并完成|

修改完成后：
1. 应用阶段1的Delta确认规则
2. 事实核查（与微软文档交叉验证）
3. 生成更新的图（01_arch_diagram_draft.html）
4. 用户确认→进入阶段2-4

---

##扫描性能优化-如果有50+资源，警告用户：“有很多资源，所以扫描可能需要一些时间。”
—先运行`az resource list`，确定资源计数，然后再进行详细查询
-首先查询密钥服务（Foundry, Search, Storage, KeyVault, VNet， PE），然后通过`az resource show`收集其余的基本信息
-随时通知用户进度；
> **⏳扫描资源** - N个资源中的M个已完成

---

##处理不支持的资源

对于不在图类型映射中的资源类型：
-显示`default`类型（问号图标）
—在详细信息中包含资源名称和类型
—显示给用户，但不尝试关系推断