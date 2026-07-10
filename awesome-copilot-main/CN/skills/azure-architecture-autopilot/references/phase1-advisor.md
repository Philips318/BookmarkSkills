#阶段1：架构顾问

该文件包含阶段1的详细说明。当从SKILL.md进入阶段1时，读取并遵循此文件。
用于路径A（新设计）和路径B（阶段0扫描后的修改）。

---

当从路径B进入时（经过现有资源分析）

在阶段0中扫描的当前架构图（00_arch_current.html）已经存在。
此时，跳过1-1中的项目name/service列表确认，直接进入修改对话：

1. “你想在这里换什么？”-用户的自然语言请求
2. 应用Delta确认规则-确认未确定的更改必需字段
3. 事实核查-与MS文档交叉验证
4. 生成更新的图（01_arch_diagram_draft.html）
5. 确认后进入第二阶段

---

**本阶段目标**：准确识别用户需求并共同完成架构。# # # 1 - 1。准备图表-收集所需信息

在绘制图表之前，向用户询问问题，直到确认以下所有项目。
**确认所有项目后才生成图表

**首先，确认项目名称：**

通过`ask_user`提供一个默认值作为选择。如果用户只按Enter键，则应用默认值；他们还可以键入自定义名称。
默认值是从用户的请求推断出来的（例如，RAG聊天机器人→`rag-chatbot`，数据平台→`data-platform`）。```
ask_user({
  question: "Please choose a project name. It will be used for the Bicep folder name, diagram path, and deployment name.",
  choices: ["<inferred-default>", "azure-project"]
})
```
项目名称用于Bicep输出文件夹名称、关系图保存路径、部署名称等。

**🔹平行预加载连同项目名称问题（必需）：**

当通过`ask_user`询问项目名称时，在等待用户响应的过程中会有空闲时间。
利用这段时间来预加载后续问题和二头肌生成所需的信息。

**与ask_user同时调用的工具：**```
// Call ask_user + the tools below simultaneously in a single response
[1] ask_user — Project name question

[2] view — Load reference files (pre-acquire Stable information)
    - references/service-gotchas.md
    - references/ai-data.md
    - references/azure-dynamic-sources.md
    - references/architecture-guidance-sources.md

[3] web_fetch — Pre-fetch architecture guidance (when workload type is identified)
    - Up to 2 targeted fetches based on decision rules in architecture-guidance-sources.md

[4] web_fetch — Fetch MS Docs for services mentioned by the user (pre-acquire Dynamic information)
    - e.g., Foundry → API version, model availability page
    - e.g., AI Search → SKU list page
    - Use URL patterns from azure-dynamic-sources.md
```
**好处**：当用户输入项目名称时，所有信息都被加载，
因此，在项目名称确定后，SKU/region问题可以立即以准确的选择呈现。
与顺序执行相比，等待时间大大减少。

* *注:* *
-预加载目标只是与项目名称无关的信息（不依赖于名称）
web_fetch只对用户初始请求中提到的服务执行（没有猜测）
- Azure CLI检查（`az account show`）在这一点上没有完成-在架构最终化时预加载

**🔹利用架构指导（调整问题深度）：**

从预加载期间获取的架构指导文档中提取**设计决策点**
并自然地将它们整合到后续的用户问题中。**目的**：不只是指定问题像SKU/region，
但是将官方建筑指南推荐的设计决策点反映到问题中。

**示例—请求“RAG聊天机器人”时：**
-获取基线Foundry聊天架构（A6）
-从文件中提取建议的设计决策点；
→网络隔离级别（完全私有还是混合？）
→认证方法（管理身份vs API密钥？）
→数据摄取策略（推索引vs拉索引？）
→监控范围（需要应用洞察？）
-在用户问题中自然包含这些要点

* *注:* *
从架构指南中提取的是“要问的问题”，而不是“答案”。
像SKU/APIversion/region这样的部署规格仍然只能通过`azure-dynamic-sources.md`来确定
—获取预算：最多2个文档。没有完全遍历**需要确认的项目：**
-[]项目名称（默认：`azure-project`）
-[]服务列表（使用哪些Azure服务）
- []SKU/tier-[]组网方式（私有端点使用）
-[]部署位置（地区）* *质疑原则:* *
—不要再次询问用户已经提到的信息
-不要询问图表中没有直接表示的详细实现细节（索引方法，查询量等）
不要一次问太多的问题；简明扼要地只问关键的未确定的问题
-对于具有明显默认值的项目（例如，PE启用），假设并确认。但是，必须始终与用户确认位置
- **当询问sku，型号或服务选项时，显示所有通过MS Docs验证的可用选项，并提供MS Docs URL。**这允许用户参考并做出自己的判断。不只显示部分选项或任意过滤它们

**🔹VM/ResourceSKU选择-区域可用性预检要求：**在向用户询问虚拟机或其他资源sku之前，您必须首先查询目标区域中实际可用的sku。
如果某个SKU由于特定区域的容量限制而阻塞，则会导致部署失败。

**虚拟机SKU校验方法：**```powershell
# Query only VM SKUs available without restrictions in the target region
az vm list-skus --location "<LOCATION>" --size Standard_D2 --resource-type virtualMachines `
  --query "[?restrictions==``[]``].name" -o tsv
```
* *原则:* *
-不要在选项中包含未经验证的sku
-不推荐从内存中输入“常用sku”-必须通过az cli或MS Docs进行验证
-在`ask_user`选项中只包括经过验证的sku
-即使是用户提供的sku，在继续之前也要验证可用性

**此原则不仅适用于虚拟机，还适用于所有受容量限制的资源（Fabric capacity等）

**🔹服务选项探索原则-禁止“从内存中列出”：**

当用户询问服务类别时（“有哪些Spark选项？”、“有哪些消息队列选项？”），或者当您需要为特定功能探索服务时：

**永远不要这样做：**
-直接从内存中获取2-3个服务的url并列出它们
明确表示“在Azure中，X有A和B”**必须这样做：**
1. **通过web_search探索完整的类别** -在类别级别搜索，如`"Azure managed Spark options site:learn.microsoft.com"`，首先发现存在哪些服务
2. **与v1作用域交叉检查** -无论搜索结果如何，检查v1作用域服务（Foundry, Fabric, AI search， ADLS Gen2等）是否属于相关类别。例如：“Spark”→Microsoft Fabric的数据工程工作负载也提供了Spark
3. **有针对性地获取发现的选项** -获取通过搜索找到的服务的MS Docs，以收集准确的比较信息
4. **向用户呈现所有选项** -在全面比较中呈现所有发现的选项，而不省略任何选项

**示例-当询问“哪些Spark实例可用？”：**```
Wrong approach: Fetch only Databricks URL + Synapse URL → Compare only 2
Correct approach: web_search("Azure managed Spark options") → Discover Databricks, Synapse, Fabric Spark, HDInsight
            → v1 scope check: Fabric is v1 scope and provides Spark → MUST include
            → Targeted fetch of each service's MS Docs → Present full comparison table
```
这一原则不仅适用于服务类别探索，还适用于用户请求“替代”、“其他选项”、“比较”等所有情况。

**🔹ask_user工具—必选用途：**

对于带有选项的问题，必须使用`ask_user`工具。它允许用户使用箭头键进行选择，方便，他们也可以键入自定义输入。ask_user使用规则：**
有2个或更多选项的问题**必须**使用ask_user（不要将其列为文本）
**`choices`必须作为字符串数组（`["A", "B"]`）传递** -作为字符串（`"A, B"`）传递将导致错误
—如果有推荐选项，请将其放在首位，并在末尾添加`(Recommended)`-在选择中包含参考信息-例如，`"Standard S1 - Recommended for production. Ref: https://..."`- **每次呼叫只有一个问题** -如果需要询问多个项目，依次调用ask_user
—选项最多为4个。如果有5个或更多，只包括3-4个最常见的（用户也可以自定义输入）。
-如果需要多个选择，将它们分成单独的问题**需要ask_user:**的项
—选择部署位置（地区）
-SKU/tier选择
-模型选择（聊天模型，嵌入模型等）
-组网方式选择
-订阅选择（第1阶段第2步）
-资源组选择（第1阶段第3步）
-任何其他需要用户选择的问题

* *用法示例:* *```
// Project name is free-form input so ask_user is not used (ask as text)
// SKU, region, etc. with defined choices use ask_user:

// 1. SKU question
ask_user({
  question: "Please select the SKU for AI Search. Ref: https://learn.microsoft.com/en-us/azure/search/search-sku-tier",
  choices: [
    "Standard S1 - Recommended for production (Recommended)",
    "Basic - For dev/test, up to 15 indexes",
    "Standard S2 - High-traffic production",
    "Free - Free trial, 50MB storage"
  ]
})

// 2. Region question (separate call — only 1 question per call)
ask_user({
  question: "Please select the Azure region for deployment. Ref: https://learn.microsoft.com/en-us/azure/ai-services/openai/concepts/models",
  choices: [
    "Korea Central - Korea region, supports most services (Recommended)",
    "East US - US East, supports all AI models",
    "Japan East - Japan East, close to Korea"
  ]
})
```
> **注**：以上示例中的SKU和区域值仅供说明。当实际请求时，通过web_fetch查询MS Docs，根据最新信息动态组合选择。不要硬编码。

**示例—用户输入不足时：**```
User: "I want to build a RAG chatbot. Using a GPT model in Foundry and AI Search."

→ Confirmed: Microsoft Foundry, Azure AI Search
→ Still undecided: Project name, specific model name, embedding model, networking (PE?), SKU, deployment location

The agent first confirms the project name via ask_user (default: rag-chatbot).
Then provides choices for each undecided item via the ask_user tool.
Include MS Docs URLs in the choices so the user can reference them directly.
```
**🚨🚨🚨[硬门]规格收集完成→图表生成所需🚨🚨🚨**

**填写所有确认项目后，您必须立即按顺序执行以下步骤。跳过任何一步都意味着第一阶段不完整

1. 根据确认的服务列表组成**services JSON + connections JSON**
2. 使用内置的图表引擎生成**`<project-name>/01_arch_diagram_draft.html`**
3. 通过`Start-Process`在浏览器中自动打开它
4. 以下面的**报告格式**向用户显示图表-这必须包括**详细的配置表**
5. 问用户：**“您想要更改或添加什么吗？”**
6. 如果用户没有更改，则继续进行阶段2转换（ask_user与下一步指导）**永远不要这样做：**
-❌没有生成图并询问“架构已确认”。我们进行下一步好吗？”
-❌将图表生成推迟到第二阶段或以后
-❌说“稍后我会创建图表”
-❌声明“架构确认”完全基于规格收集的完成
-❌生成图，但不显示配置表
-❌跳过“有什么要改变的吗？”的问题，直接跳到第二阶段

**验证条件**：如果未生成`01_arch_diagram_draft.html`文件，则不允许进入第二阶段。

**图表完成后的报告格式（所有部分都是强制性的）：**```
## Architecture Diagram

[Interactive diagram link — auto-opened in browser]

### Confirmed Configuration

| Service | Type | SKU/Tier | Details |
|---------|------|----------|---------|
| [Service name] | [Azure resource type] | [SKU] | [Key config: model, capacity, etc.] |
| ... | ... | ... | ... |

**Networking**: [VNet + Private Endpoint / Public / etc.]
**Location**: [confirmed region]
```
**显示报告后，立即使用`ask_user`，并选择：**```
ask_user({
  question: "The architecture diagram and configuration are ready. What would you like to do?",
  choices: [
    "Looks good — proceed to Bicep code generation (Recommended)",
    "I want to modify the architecture",
    "Add more services"
  ]
})
```
-如果“继续”→移动到第二阶段转换（收集subscription/RG信息）
-如果“修改”或“添加”→应用更改，重新生成图表，再次显示报告

**🚨配置表不可选。**用户需要在继续之前直观地验证已确认的内容。如果没有表，用户就无法验证体系结构。

# # # 1 - 2。交互式HTML图表生成

使用内置的图表引擎（技能中包含的Python脚本）来创建交互式HTML图表。
不需要`pip install`，因为脚本可以直接在`scripts/`文件夹中获得，不需要网络连接或安装包。
内置605+官方Azure图标。

**图表文件命名约定：**

所有关系图都在Bicep项目文件夹（`<project-name>/`）中生成。
它们被系统地管理，每个阶段都有编号的前缀，以前的阶段文件永远不会被覆盖。|阶段|文件名称|生成时||-------|-----------|----------------|
|一期设计稿|`01_arch_diagram_draft.html`|架构设计确认时|
|阶段4假设预览|`02_arch_diagram_preview.html`|假设验证后|
|第四阶段部署结果|`03_arch_diagram_result.html`|实际部署完成后|

**内置模块路径发现+ Python路径发现：**

**🚨Python路径+内置模块路径在第一阶段预加载期间被验证一次，并在所有后续的图表生成中重用。不要每次都重新发现```powershell
# ─── Step 1: Python Path Discovery ───
# ⚠️ Get-Command python may pick up the Windows Store alias, so filesystem discovery is done first
$PythonCmd = $null

# Priority 1: Direct discovery of actual installation path (most reliable)
$PythonExe = Get-ChildItem -Path "$env:LOCALAPPDATA\Programs\Python" -Filter "python.exe" -Recurse -ErrorAction SilentlyContinue |
  Where-Object { $_.FullName -notlike '*WindowsApps*' } |
  Select-Object -First 1 -ExpandProperty FullName
if ($PythonExe) { $PythonCmd = $PythonExe }

# Priority 2: Program Files discovery
if (-not $PythonCmd) {
  $PythonExe = Get-ChildItem -Path "$env:ProgramFiles\Python*", "$env:ProgramFiles(x86)\Python*" -Filter "python.exe" -Recurse -ErrorAction SilentlyContinue |
    Select-Object -First 1 -ExpandProperty FullName
  if ($PythonExe) { $PythonCmd = $PythonExe }
}

# Priority 3: Find in PATH (only if not a Windows Store alias)
if (-not $PythonCmd) {
  foreach ($cmd in @('python3', 'py')) {
    $found = Get-Command $cmd -ErrorAction SilentlyContinue
    if ($found -and $found.Source -notlike '*WindowsApps*') { $PythonCmd = $cmd; break }
  }
}

if (-not $PythonCmd) {
  Write-Host ""
  Write-Host "Python is not installed or not found in PATH." -ForegroundColor Red
  Write-Host ""
  Write-Host "Please install using one of the following methods:" -ForegroundColor Yellow
  Write-Host "  1. winget install Python.Python.3.12"
  Write-Host "  2. Download from https://www.python.org/downloads/"
  Write-Host "  3. Search for 'Python 3.12' in the Microsoft Store and install"
  Write-Host ""
  Write-Host "After installation, restart your terminal and try again."
  return
}

# ─── Step 2: Built-in Script Path Discovery (no pip install needed) ───
# Priority 1: Project local skill folder
$ScriptsDir = Get-ChildItem -Path ".github\skills\azure-architecture-autopilot" -Filter "cli.py" -Recurse -ErrorAction SilentlyContinue |
  Where-Object { $_.Directory.Name -eq 'scripts' } |
  Select-Object -First 1 -ExpandProperty DirectoryName
# Priority 2: Global skill folder
if (-not $ScriptsDir) {
  $ScriptsDir = Get-ChildItem -Path "$env:USERPROFILE\.copilot\skills\azure-architecture-autopilot" -Filter "cli.py" -Recurse -ErrorAction SilentlyContinue |
    Where-Object { $_.Directory.Name -eq 'scripts' } |
    Select-Object -First 1 -ExpandProperty DirectoryName
}

# ─── Step 3: Diagram Generation (CLI method — direct script execution) ───
$OutputFile = "<project-name>\01_arch_diagram_draft.html"

& $PythonCmd "$ScriptsDir\cli.py" `
  --services '<services_JSON>' `
  --connections '<connections_JSON>' `
  --title "Architecture Title" `
  --vnet-info "10.0.0.0/16 | pe-subnet: 10.0.1.0/24" `
  --output $OutputFile

# Automatically open in browser after generation
Start-Process $OutputFile
```
**Python API方法也可用（可选）：**

当JSON非常大时，您可以直接调用Python API以避免CLI参数长度限制。
将scripts文件夹添加到`sys.path`以导入内置模块：```python
import sys, os
# Add scripts folder to Python path (use built-in module without pip install)
scripts_dir = r"<absolute path to scripts folder>"  # $ScriptsDir value found in Step 2
sys.path.insert(0, scripts_dir)

from generator import generate_diagram

services = [...]   # services JSON
connections = [...] # connections JSON

html = generate_diagram(
    services=services,
    connections=connections,
    title="Architecture Title",
    vnet_info="10.0.0.0/16 | pe-subnet: 10.0.1.0/24",
    hierarchy=None  # Only used for multiple subscriptions/RGs
)

with open("<project-name>/01_arch_diagram_draft.html", "w", encoding="utf-8") as f:
    f.write(html)
```
**🔹CLI vs Python API选择标准：**

|场景|方法|原因||----------|--------|--------|
| 10个及以下业务| CLI (`python scripts/cli.py`) |简单快捷|
|超过10个服务或使用层次结构| Python API (sys。避免CLI参数长度限制|
|Multi-subscription/RG图| Python API +`hierarchy`参数|层次结构表示|

**支持的服务类型完整列表：**

可在技能的内置参考文件下的`references/`。
支持的服务类型值在下面的服务JSON格式部分中列出。

b> **图生成顺序**:(1)验证Python路径→(2)验证内置模块路径→(3)编写services/connectionsJSON→(4)执行。如果没有安装Python，请指导用户在编写JSON之前安装Python。这可以防止因为缺少Python而导致构建JSON失败的浪费。b> **🚨自动图表打开（无例外）**：当一个HTML文件是由内置的图表引擎生成，它**必须始终**在浏览器中打开，无论情况。无论何时（重新）生成关系图，都要执行`Start-Process`命令。图表生成和浏览器打开总是在一个PowerShell命令块中一起执行。
>
b> **当这个应用时（不只是这些，而是所有时候生成一个HTML图表）：**
> -一期设计稿（`01_arch_diagram_draft.html`）
> - Delta确认后的图表再生
> -第4阶段假设预览（`02_arch_diagram_preview.html`）
> -第4阶段部署结果（`03_arch_diagram_result.html`）
> -部署后的架构更改（`04_arch_diagram_update_draft.html`）
> -由于任何原因重新生成图表的任何其他情况

**services JSON格式：**

根据用户确认的服务列表动态组合。下面是JSON结构描述。```json
[
  {"id": "uniqueID", "name": "Service Display Name", "type": "iconType", "sku": "SKU", "private": true/false,
   "details": ["Detail line 1", "Detail line 2"]}
]
```
|字段|必选|类型|描述||-------|----------|------|-------------|
|`id`|是| string |唯一标识符（kebab-case） |
|`name`|是| string |图|上显示的显示名称
|`type`|是| string |服务类型（从下面的列表中选择）|
|`sku`| |字符串|SKU/tier信息|
|`private`| |布尔|私有端点已连接（默认：false） |
|`details`| | string[] |附加信息显示在侧栏|
|`subscription`| | string |订阅名（使用层次结构时需要）|
|`resourceGroup`| | string |资源组名（使用层次结构时需要）|

**服务类型-规范引用：**

>⚠️**CRITICAL**：始终使用下表中的**规范类型**。不要使用Azure ARM资源名（例如，`private_endpoints`,`storage_accounts`,`data_factories`）。生成器对常见变体进行规范化，但使用规范类型可确保正确的图标呈现、PE检测和颜色编码。

|类别|规范类型| Azure资源|图标||----------|---------------|----------------|------|
| **AI** |`ai_foundry`|Microsoft.CognitiveServices/accounts（类型：AIServices） | AI Foundry |
| |`openai`|Microsoft.CognitiveServices/accounts（类型：OpenAI） | Azure OpenAI |
| |`ai_hub`|铸造项目| AI工作室|
| |`search`|Microsoft.Search/searchServices|认知搜索|
| |`document_intelligence`|Microsoft.CognitiveServices/accounts（类型：FormRecognizer） |表单识别器|
| |`aml`|Microsoft.MachineLearningServices/workspaces|机器学习|
| **Data** |`fabric`|Microsoft.Fabric/capacities| Microsoft Fabric |
| |`adf`|Microsoft.DataFactory/factories|数据工厂|
| |`storage`|Microsoft.Storage/storageAccounts|存储帐户|
| |`adls`| ADLS Gen2 (Storage with HNS) |数据湖|
| |`cosmos_db`|Microsoft.DocumentDB/databaseAccounts| Cosmos DB |
| |`sql_database`|Microsoft.Sql/servers/databases| SQL数据库|
|`sql_server`|Microsoft.Sql/servers| SQL Server |
| |`databricks`|Microsoft.Databricks/workspaces|数据块|
| |`synapse`|Microsoft.Synapse/workspaces| Synapse Analytics |
| |`redis`|Microsoft.Cache/redis| Redis Cache |
| |`stream_analytics`|Microsoft.StreamAnalytics/streamingjobs|流分析|
|`postgresql`|Microsoft.DBforPostgreSQL/flexibleServers| PostgreSQL |
|`mysql`|Microsoft.DBforMySQL/flexibleServers| MySQL |
| **安全** |`keyvault`|Microsoft.KeyVault/vaults|密钥库|
| |`sentinel`|微软。SecurityInsights | Sentinel |
| **计算** |`appservice`|Microsoft.Web/sites|应用服务|
b| |`function_app`|Microsoft.Web/sites（类型：functionapp） |功能App |
| |`vm`|Microsoft.Compute/virtualMachines|虚拟机|
| |`aks`|Microsoft.ContainerService/managedClusters| AKS |
|`acr`|Microsoft.ContainerRegistry/registries|容器注册表|
| |`container_apps`|Microsoft.App/containerApps|容器应用|
| |`static_web_app`|Microsoft.Web/staticSites|静态Web应用|
| |`spring_apps`|Microsoft.AppPlatform/Spring| Spring Apps |
| **网络** |`pe`|Microsoft.Network/privateEndpoints|私有端点|
| |`vnet`|Microsoft.Network/virtualNetworks| VNet |
| |`nsg`|Microsoft.Network/networkSecurityGroups| NSG |
|`firewall`|Microsoft.Network/azureFirewalls|防火墙|
| |`bastion`|Microsoft.Network/bastionHosts|堡垒|
| |`app_gateway`|Microsoft.Network/applicationGateways|应用网关|
| |`front_door`|Microsoft.Cdn/profiles（前门）|前门|
| |`vpn`|Microsoft.Network/virtualNetworkGateways| VPN网关|
| |`load_balancer`|Microsoft.Network/loadBalancers|负载均衡|
| |`nat_gateway`|Microsoft.Network/natGateways| NAT网关|
| |`cdn`|Microsoft.Cdn/profiles| CDN |
| **IoT** |`iot_hub`|Microsoft.Devices/IotHubs|物联网集线器|
| |`digital_twins`|Microsoft.DigitalTwins/digitalTwinsInstances| Digital Twins |
| **集成** |`event_hub`|Microsoft.EventHub/namespaces|事件中心|
| |`event_grid`|Microsoft.EventGrid/topics|事件网格|
| |`apim`|Microsoft.ApiManagement/service| API管理|
| |`service_bus`|Microsoft.ServiceBus/namespaces|服务总线|
| |`logic_apps`|Microsoft.Logic/workflows|逻辑应用|
| **监控** |`log_analytics`|Microsoft.OperationalInsights/workspaces|日志分析|
| |`appinsights`|Microsoft.Insights/components| App Insights |
| |`monitor`| Azure Monitor | Monitor |
| **其他** |`jumpbox`，`user`，`devops`| - |特殊|**当使用私有端点时- PE节点需要添加：**

如果私有端点包含在体系结构中，则必须将PE节点添加到每个服务的服务JSON中，并且连接还必须包含PE链接，以便它们出现在图中。```json
// Add PE node corresponding to each service
{"id": "pe_serviceID", "name": "PE: ServiceName", "type": "pe", "details": ["groupId: correspondingGroupID"]}

// Add service → PE connection in connections
{"from": "serviceID", "to": "pe_serviceID", "label": "", "type": "private"}
```
**🚨🚨🚨PE连接和业务逻辑连接是分开的-两者必须包括🚨🚨🚨**

PE连接（`"type": "private"`）表示网络隔离。但是单独这样做并不能显示图中服务之间flow/API调用的实际**数据。

**必须包括两种类型的连接：**

1. 业务逻辑连接——服务之间的实际数据流（api、数据、安全类型）
2. **PE连接** -业务间的网络隔离↔PE（私有类型）```json
// ✅ Correct example — Function App → Foundry
// 1) Business logic: Function App calls Foundry for chat/embedding
{"from": "func_app", "to": "foundry", "label": "RAG Chat + Embedding", "type": "api"}
// 2) PE connection: Foundry's Private Endpoint
{"from": "foundry", "to": "pe_foundry", "label": "", "type": "private"}

// ❌ Wrong example — Only PE connection, no business logic connection
{"from": "foundry", "to": "pe_foundry", "label": "", "type": "private"}
// → No connection line between Function App and Foundry in the diagram, so the architecture flow is not visible
```
**永远不要这样做：**
—只创建PE连接，忽略业务逻辑连接
—将业务逻辑连接的`from`/`to`连接到PE节点（使用**实际服务ID**，而不是PE）
-假设“PE在那里，所以连接线会显示出来”

不同业务的PE组id不同。参考`references/service-gotchas.md`中的PE groupId & DNS Zone映射表。

b> **服务命名约定：必须使用最新的Azure官方名称。如果不确定名称，请与MS Docs验证。
>关于每个服务的资源类型和关键属性，请参考`references/ai-data.md`。

**connections JSON格式：**```json
[
  {"from": "serviceA_ID", "to": "serviceB_ID", "label": "Connection description", "type": "api|data|security|private"}
]
```
* *连接类型:* *

|类型|颜色|风格|使用||------|-------|-------|---------|
|`api`|蓝色|固体| API调用，查询|
|`data`|绿色|固体|数据流，索引|
|`security`|橙色|虚线|秘密，验证|
|`private`|紫色|虚线|私有端点连接|
|`network`|灰色|固体|网络路由|
|`default`|灰色|纯色|其他|

**🔹多语种原理：**
—服务中的`name`、服务中的`details`、连接中的`label`用**用户语言**书写
—示例：`"label": "RAG Search"`、`"label": "Data Ingestion"`官方Azure服务名称（Microsoft Foundry， AI Search等）始终为英文，无论语言如何**🔹VNet Node -不要添加到服务JSON:**
- VNet在图中自动显示为紫色虚线边界（当pe存在时）
-将单独的VNet节点添加到服务JSON中会导致与边界线重复的混淆
—VNet信息（CIDR，子网）通过侧栏VNet边界标签充分传达

向用户提供生成的HTML文件的完整路径。

# # # 1 - 3。通过对话完成建筑

架构是通过与用户的对话逐步确定的。当用户要求更改时，不要从头开始；相反，**只反映基于当前确认状态的请求更改，并重新生成图。

**⚠️达美航空确认规则-服务所需验证Addition/Change:**服务addition/change不是一个“简单的更新”——它是一个为该服务重新打开未确定的必填字段的事件。

* *过程:* *
1. Diff当前确认的状态+新请求
2. 确定新添加的服务所需的字段（请参阅`domain-packs`或MS Docs）
3. 从MS Docs中获取服务的区域availability/options4. 如果任何必填字段未确定，请先通过ask_user询问用户
5. **只有确认完成后才能重新生成图表**

**永远不要这样做：**
-完成图表更新，而所需字段仍未确定
-任意添加用户没有提到的sub-components/workloads（例如，自动添加OneLake和数据管道到Fabric请求）
-模糊地假设SKU/model像“F SKU”，没有确认

**不要重新询问已确认服务的设置。**仅确认新added/changed服务的未确定项目。

---**🚨🚨🚨[最优先原则]在设计阶段立即进行事实检查🚨🚨🚨**

**第一阶段的目的是确认一个“可行的架构”
**无论用户请求什么，在映射到图中之前，你必须通过web_fetch直接查询MS Docs来检查它是否真的可能

**设计方向与部署规范-独立的信息路径：**

|决策类型|参考路径|样例||--------------|----------------|----------|
设计方向（架构模式、最佳实践、服务组合）|`references/architecture-guidance-sources.md`→目标获取|“推荐的RAG结构是什么？”，“企业基线？”|
| **部署规格** （API版本，SKU，地区，型号，PE映射）|`references/azure-dynamic-sources.md`→MS Docs fetch |“API版本是什么？”，“这个型号在Korea Central可用吗？”|

- **设计方向来自架构指导，实际部署值来自动态来源。**不要把这两条路混在一起。
—不要用架构指导文档的内容来确定SKU/APIversion/region.- **不要为每个请求爬遍所有架构中心子文档。**执行基于触发器的目标获取最多2个相关文档。
—按题型划分的trigger/fetchbudget/decision规则参考`architecture-guidance-sources.md`。**此原则适用于所有请求，无一例外：**
—型号addition/change→在MS Docs中验证该型号是否存在，是否可以部署到目标区域
—服务addition/change→在MS Docs中验证该服务在目标区域是否可用
- SKU更改→在MS文档中验证SKU是否有效并支持所需的功能
-特性请求→在MS Docs中验证该特性是否被实际支持
—服务组合→在MS Docs中验证是否可以进行服务间集成
- **任何其他要求**→使用微软文档进行事实核查

**MS Docs验证结果：**
- **可能**→在图中反映
- **不可能**→立即向用户解释原因并建议可用的替代方案

**事实核查过程-需要交叉验证：**不要简单地查询一次，然后继续处理用户请求。
**必须始终使用其他MS文档pages/sources进行交叉验证

**GHCP环境约束**：子代理（explore/task/general-purpose）没有`web_fetch`/`web_search`工具。
因此，需要MS Docs查询的验证必须由主代理**直接执行**。```
[1st Verification] Main agent directly queries MS Docs via web_fetch (primary page)
    ↓
[2nd Verification] Main agent additionally fetches other/related MS Docs pages via web_fetch for cross-checking
    - e.g., Model availability → 1st: models page / 2nd: regional availability or pricing page
    - e.g., API version → 1st: Bicep reference page / 2nd: REST API reference page
    - Compare 1st and 2nd results and flag any discrepancies
    ↓
[Consolidate Results] If both verifications match, respond to the user
    - On discrepancy: Resolve with additional queries, or honestly inform the user about the uncertainty
```
**事实核查质量标准——要彻底，不要草率：**
-当获取一个MS Docs页面时，**检查所有相关的部分，标签和条件，没有遗漏**
-检查模型可用性时：检查**所有部署类型**，包括Global Standard， Standard, Provisioned， Data Zone等不要仅根据一种部署类型得出“不支持”的结论
—检查SKU时：**完全**检查该SKU支持的特性列表
—如果页面较大，请多次获取相关章节**，以确保准确性
—如果不确定，请查询其他页面。**永远不要根据猜测回答问题****永远不要这样做：**
—添加到图中，无需验证
-用“我将在生成二头肌时检查”或“它将在部署时验证”来推迟验证
-只依靠你的记忆并回答“它应该工作”- **必须直接查询MS文档**
-获取MS文档，但只阅读了一部分就急于得出结论
-基于单个查询完成- **必须与另一个源**交叉验证

**🚫子代理使用规则：**

** GHCP中的子代理=`task`工具：**
-`agent_type: "explore"`-只读任务，如代码库探索，文件搜索（**web_fetch/web_search不可用**）
-`agent_type: "task"`-命令执行，如az cli， bicep build
-`agent_type: "general-purpose"`-高级任务，如复杂的二头肌生成> **⚠️子代理工具约束**：所有子代理（explore/task/general-purpose）不能使用`web_fetch`或`web_search`。
>需要MS Docs查询的事实检查，API版本验证，模型可用性检查等。必须由主代理**直接执行**。

**前景vs背景决策标准：**
- **如果在进行下一步之前需要结果→`mode: "sync"`（默认）**
-例如，查询SKU列表，然后向用户提供选择，验证模型可用性，然后在图中反映
-在后台运行会让用户空闲等待结果
- **如果有其他独立的工作可以在等待结果时完成→`mode: "background"`**
-例如，同时web_fetch多个MS Docs页面进行交叉验证

**大多数事实检查应该在前台运行(`mode: "sync"`)**，因为没有结果就不能问下一个问题。**如何并行运行交叉验证：**```
// Execute 1st and 2nd verification simultaneously (main agent performs directly)
[Simultaneously] Directly query primary MS Docs page via web_fetch (1st)
[Simultaneously] Additionally query related MS Docs page via web_fetch (2nd)
// Compare both results to check for discrepancies
// e.g., Model availability → parallel fetch of models page + regional availability page
```
**永远不要这样做：**
-当需要结果时在后台运行，然后无所事事地等待
-将需要web_fetch/web_search的任务委托给子代理（主代理必须直接执行）
—尝试直接读取子代理内部的文件

---

**⚠️重要：在用户明确批准继续下一步之前，不要执行任何shell命令
但是，对于上述事实检查，MS Docs的web_fetch是例外允许的。

一旦架构得到确认（用户说图没有变化），询问用户是否继续下一步。

**🚨第二阶段过渡先决条件-在问这个问题之前必须满足以下所有条件：**1.`01_arch_diagram_draft.html`是使用内置的图表引擎**生成的
2. 该图表已在浏览器中**打开，并以报告格式**显示给用户**，并带有**配置表**
3. 用户被问到：“你想要更改或添加什么吗？”**和**回应**没有变化**，或修改已反映并**最终确认**

**如果不满足上述任何条件，请勿进入第2阶段。**
如果这个图还不存在，现在就生成它——按照第1-2节的步骤。
如果配置表没有显示，在询问更改之前立即显示它。

**遵循并行预加载原则，与ask_user同时执行`az account list`和`az group list`，提前准备subscription/RG选项```
// Call simultaneously in the same response:
[1] ask_user — "The architecture is confirmed! Shall we proceed to the next step?"
[2] powershell — az account show 2>&1              (pre-check login status)
[3] powershell — az account list --output json      (pre-prepare subscription choices)
[4] powershell — az group list --output json        (pre-prepare resource group choices)
```
Ask_user显示格式：```
The architecture is confirmed! Shall we proceed to the next step?

✅ Confirmed architecture: [summary]

The following steps will proceed:
1. [Bicep Code Generation] — AI automatically writes IaC code
2. [Code Review] — Automated security/best practice review
3. [Azure Deployment] — Actual resource creation (optional)

Shall we proceed? (If you'd like just the code without deployment, let me know)
```
用户批准后，按以下顺序收集信息。
**由于`az account show`+`az account list`+`az group list`在预加载时已经完成，所以subscription/RG选项可以立即呈现

**步骤1:Azure登录验证

从预加载中已经可以获得`az account show`结果。不需要额外的电话。

—已登录→转到步骤2
—未登录→引导用户：  ```
  Azure CLI login is required. Please run the following command in your terminal:
  az login
  Please let me know once completed.
  ```
**第二步：订阅选择**

从预加载中已经可以获得`az account list`结果。不需要额外的电话。

从查询结果中提供最多4个订阅，作为`ask_user`选项。
如果有5个或更多，包括3-4个最常用的订阅作为选项（用户也可以输入自定义输入）。
一旦用户选择，执行`az account set --subscription "<ID>"`。

**步骤3：资源组确认**

从预加载中已经可以获得`az group list`结果。不需要额外的电话。

从列表中提供多达4个现有资源组作为`ask_user`选项。
如果用户选择了一个现有的组，则按原样使用它；如果他们键入新名称作为自定义输入，则在阶段4部署期间创建它。**需要确认的项目：**
-[]服务列表和sku
-[]组网方式（私有端点使用）
-[]订阅ID（在步骤2中确认）
-[]资源组名称（在步骤3中确认）
-[]位置（与用户确认-通过MS Docs验证每个服务的区域可用性）

---

##🚨第一阶段完成清单-进入第二阶段前需要进行的验证

在离开第一阶段之前，请验证以下所有项目。如果有不完整的，不要进入第二阶段。

| # |项目|验证方法||---|------|---------------------|
| 1 |所需规格已确认|项目名称、服务、sku、区域、组网方式已确认|
b| |事实检查完成| MS Docs交叉验证已执行|
| **生成的图** |`01_arch_diagram_draft.html`文件已使用内置图引擎|生成
| 4 | **显示配置表** |以报告格式|向用户显示Service/Type/SKU/Details的详细表
bbb5 | **用户查看图表** |浏览器自动打开+报告格式+“有什么要改变的吗？”问题问|
bbbb6 |用户最终审批|用户确认没有更改，然后选择“继续下一步”|

**⚠️当第3-5项不完整时，不要问第6项。**流程必须为：图表→表格→询问变更→确认→下一步。

---

阶段2交接：二头肌生成代理一旦用户同意继续，读取`references/bicep-generator.md`指令并生成Bicep模板。
或者，这可以委托给单独的子代理。

**敏感信息处理原则（绝不违反）：**
—不要在聊天中询问虚拟机密码、API密钥或其他敏感值，也不要保存在参数文件中
—在代码审查过程中，如果在`main.bicepparam`明文中发现敏感值，请立即删除

**🔹用户输入的敏感值，如虚拟机密码-需要复杂度验证：**

当用户输入VM管理员密码或类似密码时，请在发送到Azure之前验证复杂度要求。
Azure虚拟机必须满足以下所有条件：
- 12个字符及以上
—至少包含大写字母、小写字母、数字、特殊字符中的3种**验证失败：**不要尝试部署；立即要求用户重新输入：
> **⚠️密码不符合Azure的复杂度要求。**必须包含大写字母+小写字母+数字+特殊字符中的至少3个字符。

**永远不要这样做：**
-警告“它可能不符合要求”，但仍然尝试部署- **必须阻止**
-发送到Azure没有进行复杂性验证，导致部署失败

**🚨`@secure()`参数与`.bicepparam`兼容原则：**

当`.bicepparam`文件具有`using './main.bicep'`指令时，其他`--parameters`标志不能与`az deployment group what-if/create`一起使用。
因此，`@secure()`参数处理遵循以下规则：

1. **`@secure()`参数必须有默认值** -使用Bicep函数，如`newGuid()`，`uniqueString()`   ```bicep
   @secure()
   param sqlAdminPassword string = newGuid()  // Auto-generated at deployment, store in Key Vault if needed
   ```
2. **如果有`@secure()`参数需要用户指定值：**
-不要使用`.bicepparam`文件；而是使用`--template-file`+`--parameters`组合
-或者生成一个单独的JSON参数文件（`main.parameters.json`）   ```powershell
   # When .bicepparam cannot be used — substitute with JSON parameter file
   az deployment group what-if `
     --template-file main.bicep `
     --parameters main.parameters.json `
     --parameters sqlAdminPassword='user-input-value'
   ```
3. **请勿在同一个部署命令中同时使用`.bicepparam`和`--parameters`   ```
   ❌ az deployment group create --parameters main.bicepparam --parameters key=value
   ✅ az deployment group create --parameters main.bicepparam
   ✅ az deployment group create --template-file main.bicep --parameters main.parameters.json --parameters key=value
   ```
* *决策标准:* *
—所有`@secure()`参数都有默认值（newGuid等）→可以使用`.bicepparam`—任何`@secure()`参数需要用户输入→使用JSON参数文件代替`.bicepparam`**当MS Docs获取失败时：**
如果web_fetch由于速率限制等原因失败，必须通知用户：  ```
  ⚠️ MS Docs API version lookup failed. Generating with the last known stable version.
  Verifying the actual latest version before deployment is recommended.
  Shall we continue?
  ```
-未经用户批准，不要悄悄进行硬编码版本

**前二头肌生成参考文件：**
-`references/service-gotchas.md`-必选属性，常见错误，PEgroupId/DNSZone映射
—`references/ai-data.md`—AI/Data业务配置指南（v1域）
-`references/azure-common-patterns.md`-PE/security/naming通用模式
-`references/azure-dynamic-sources.md`- MS Docs URL注册表（用于API版本获取）
—对于上述文件中未涉及的服务，请直接获取MS Docs来验证资源类型、属性和PE映射

* *输出结构:* *```
<project-name>/
├── main.bicep              # Main orchestration
├── main.bicepparam         # Parameters (environment-specific values)
└── modules/
    ├── network.bicep       # VNet, Subnet (including private endpoint subnet)
    ├── ai.bicep            # AI services (configured per user requirements)
    ├── storage.bicep       # ADLS Gen2 (isHnsEnabled: true)
    ├── fabric.bicep        # Microsoft Fabric (if needed)
    ├── keyvault.bicep      # Key Vault
    └── private-endpoints.bicep  # All PEs + DNS Zones
```
**肱二头肌强制原则：**
—参数化所有资源名称—`param openAiName string = 'oai-${uniqueString(resourceGroup().id)}'`私有服务必须有`publicNetworkAccess: 'Disabled'`—在pe-subnet中设置`privateEndpointNetworkPolicies: 'Disabled'`—私有DNS区域+ VNet Link + DNS区域组—三者必选
-当使用微软铸造厂，**铸造项目（`accounts/projects`）必须与**一起创建-没有它，门户是不可用的
ADLS Gen2必须有`isHnsEnabled: true`（省略这个会创建一个常规的Blob存储）
-在密钥库中存储秘密，通过`@secure()`参数引用
-添加英文注释，解释每个部分的目的

发电完成后立即过渡到第三阶段。

---

第3阶段交接：二头肌审查代理

按照`references/bicep-reviewer.md`指令进行复核。

**⚠️重点：不要只是目测和说“通过”。你必须运行`az bicep build`来验证实际的编译结果```powershell
az bicep build --file main.bicep 2>&1
```
1. 编译errors/warnings→修复
2. 检查清单→修复
3. 重新编译以确认
4. 报告结果（包括编译结果）

有关详细的检查清单和修复过程，请参见`references/bicep-reviewer.md`。

审查完成后，在过渡到阶段4之前向用户展示结果，并且**必须指导用户下一步

**🚨第三阶段完成后需要的报告格式：**```
## Bicep Code Review Complete

[Review result summary — bicep-reviewer.md Step 6 format]

---

**Next Step: Phase 4 (Azure Deployment)**

The review is complete. The following steps will proceed:
1. **What-if Validation** — Preview planned resources without making actual changes
2. **Preview Diagram** — Architecture visualization based on What-if results (02_arch_diagram_preview.html)
3. **Actual Deployment** — Create resources in Azure after user confirmation

Shall we proceed with deployment? (If you'd like just the code without deployment, let me know)
```
**永远不要这样做：**
-完成阶段3，只提供`az deployment group create`命令，没有进一步的指导
-直接部署而不需要验证，或者告诉用户自己运行命令
-跳过阶段4步骤（假设→预览图→部署）