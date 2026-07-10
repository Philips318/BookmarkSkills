---
name: foundry-agent-sync
description: "Create and synchronize prompt-based AI agents directly within Azure AI Foundry via REST API, from a local JSON manifest. Unlike scaffolding skills that only generate local code, this skill registers agents in the Foundry service itself — making them immediately available for invocation. Use when the user asks to create agents in Foundry, sync, deploy, register, or push agents to Foundry, update agent instructions, or scaffold the manifest and sync script for a new repository. Triggers: 'create agent in foundry', 'sync foundry agents', 'deploy agents to foundry', 'register agents in foundry', 'push agents', 'create foundry agent manifest', 'scaffold agent sync'."
---
# Foundry Agent同步

# #概述

通过代理服务REST API直接在Azure AI Foundry中创建和同步基于提示的AI代理。该技能在Foundry服务本身中注册代理——使它们可以通过Foundry门户或API立即调用、评估和管理。每个代理都是使用本地JSON清单文件中的定义，通过命名POST调用以幂等方式创建或更新的。

关键区别：**此技能在AI Foundry（服务器端）内创建代理。它不支撑本地代理代码或容器映像—为此，请使用`microsoft-foundry`技能的`create`子技能。

# #先决条件

用户必须具备：

1. 带有已部署模型的Azure AI Foundry项目（例如`gpt-5-4`）
2. Azure CLI （`az`）通过对Foundry项目的访问进行身份验证
3. 在Foundry项目资源上的**Azure AI User**角色（或更高）在继续之前收集这些值：

|值|如何获取||---|---|
| **代工项目端点** | Azure门户→AI代工项目→概述→端点，或`az resource show`|
| **订阅ID** |`az account show --query id -o tsv`|
| **模型部署名称** | Foundry项目中部署的模型名称（如`gpt-5-4`） |

## Manifest格式

清单是一个JSON数组，其中每个条目定义一个代理。在常见路径中查找它：`infra/foundry-agents.json`、`foundry-agents.json`或`.foundry/agents.json`。如果不存在，就脚手架一个。```json
[
  {
    "useCaseId": "alert-triage",
    "description": "Short description of what this agent does.",
    "baseInstruction": "You are an assistant that... <system prompt for the agent>"
  }
]
```
###字段参考

|字段|必选|描述||---|---|---|
|`useCaseId`|是|烤肉串大小写标识符；用于构建代理名称（`{prefix}-{useCaseId}`） |
|`description`|是|人类可读的描述，存储为代理元数据|
|`baseInstruction`|是|代理|的系统提示/基本指令

##同步脚本

PowerShell（交互式/ CI）

创建或定位同步脚本。规范路径是`infra/scripts/sync-foundry-agents.ps1`，但适合于repo布局。```powershell
param(
  [Parameter(Mandatory)]
  [string]$SubscriptionId,

  [Parameter(Mandatory)]
  [string]$ProjectEndpoint,

  [string]$ManifestPath = (Join-Path $PSScriptRoot '..\foundry-agents.json'),
  [string]$ModelName = 'gpt-5-4',
  [string]$AgentNamePrefix = 'myproject',
  [string]$ApiVersion = '2025-11-15-preview'
)

$ErrorActionPreference = 'Stop'

# Optional: append a common instruction suffix to every agent
$commonSuffix = ''

az account set --subscription $SubscriptionId | Out-Null
$accessToken = az account get-access-token --resource https://ai.azure.com/ --query accessToken -o tsv
if (-not $accessToken) { throw 'Failed to acquire Foundry access token.' }

$definitions = Get-Content -Raw -Path $ManifestPath | ConvertFrom-Json
$headers = @{ Authorization = "Bearer $accessToken" }
$results = @()

foreach ($def in $definitions) {
  $agentName = "$AgentNamePrefix-$($def.useCaseId)"
  $instructions = if ($commonSuffix) { "$($def.baseInstruction)`n`n$commonSuffix" } else { $def.baseInstruction }
  $body = @{
    definition  = @{ kind = 'prompt'; model = $ModelName; instructions = $instructions }
    description = $def.description
    metadata    = @{ useCaseId = $def.useCaseId; managedBy = 'foundry-agent-sync' }
  } | ConvertTo-Json -Depth 8

  $uri = "$($ProjectEndpoint.TrimEnd('/'))/agents/$agentName`?api-version=$ApiVersion"
  $resp = Invoke-RestMethod -Method Post -Uri $uri -Headers $headers -ContentType 'application/json' -Body $body
  $version = $resp.version ?? $resp.latest_version ?? $resp.id ?? 'unknown'
  Write-Host "Synced $agentName ($version)"
  $results += [pscustomobject]@{ name = $agentName; version = $version }
}

$results | Format-Table -AutoSize
```
Bash （Bicep部署脚本/ CI）

要通过`Microsoft.Resources/deploymentScripts`进行自动部署，请使用bash脚本：

1. 使用托管身份进行身份验证：`az login --identity --username "$CLIENT_ID"`2. 获取一个Foundry令牌：`az account get-access-token --resource https://ai.azure.com/`3. 从`FOUNDRY_AGENT_DEFINITIONS`环境变量（JSON字符串）迭代定义
4. 将每个代理发送到`{endpoint}/agents/{name}?api-version=2025-11-15-preview`二头肌集成（可选）

在基础设施部署期间自动运行同步。

1. **在编译时加载manifest**：   ```bicep
   var agentDefinitions = loadJsonContent('foundry-agents.json')
   ```
2. **在Foundry项目上使用**Azure AI User**角色创建一个用户分配的管理身份**。

3. **创建一个`Microsoft.Resources/deploymentScripts`**资源（类型`AzureCLI`）：
—使用被管理的身份
—通过`loadTextContent`加载bash同步脚本
-将项目端点、定义和模型作为环境变量传递

在`deployFoundryAgents`参数后面设置门，以便团队可以选择in/out.# #工作流程

###步骤1 -定位或脚手架清单

在repo中搜索`foundry-agents.json`。如果不存在，询问用户需要什么代理并创建清单。

###步骤2 -定位或支撑同步脚本

搜索`sync-foundry-agents.ps1`或`foundry-agent-sync.sh`。如果没有，使用上面的模板创建PowerShell脚本，适应：
-`$AgentNamePrefix`与项目名匹配
—`$ModelName`为用户部署的型号
-`$ManifestPath`到实际舱单位置

###步骤3—收集参数向用户询问：
-铸造项目终点
-订阅ID
—模型部署名称（默认为`gpt-5-4`）
-代理名称前缀（默认：以kebab-case表示的repo名称）

###步骤4 -运行同步

使用收集到的参数执行PowerShell脚本：```powershell
.\infra\scripts\sync-foundry-agents.ps1 `
  -SubscriptionId '<sub-id>' `
  -ProjectEndpoint '<endpoint>' `
  -ModelName '<model>' `
  -AgentNamePrefix '<prefix>'
```
###步骤5 -验证

通过列出已同步的代理来确认它们：```powershell
$token = az account get-access-token --resource https://ai.azure.com/ --query accessToken -o tsv
$endpoint = '<project-endpoint>'
Invoke-RestMethod -Uri "$endpoint/agents?api-version=2025-11-15-preview" `
  -Headers @{ Authorization = "Bearer $token" }
```
## REST API参考

|操作|方法| URL ||---|---|---|
|Create/updateagent | POST |`{projectEndpoint}/agents/{agentName}?api-version=2025-11-15-preview`|
|列表代理| GET |`{projectEndpoint}/agents?api-version=2025-11-15-preview`|
|获取代理|获取|`{projectEndpoint}/agents/{agentName}?api-version=2025-11-15-preview`|
|删除代理| Delete |`{projectEndpoint}/agents/{agentName}?api-version=2025-11-15-preview`|Create/Update有效载荷```json
{
  "definition": {
    "kind": "prompt",
    "model": "<deployed-model-name>",
    "instructions": "<system prompt>"
  },
  "description": "<agent description>",
  "metadata": {
    "useCaseId": "<use-case-id>",
    "managedBy": "foundry-agent-sync"
  }
}
```
# #故障排除

| |原因|修复||---|---|---|
|`401 Unauthorized`|令牌过期或受众错误|重新运行`az account get-access-token --resource https://ai.azure.com/`|
|`403 Forbidden`|缺少Azure AI用户角色|在Foundry项目范围|上分配角色
|`404 Not Found`|项目端点错误|验证端点包括`/api/projects/{projectName}`|
|模型未找到|模型未部署在|项目中首先在AI Foundry门户中部署模型|
| Manifest路径错误|检查`-ManifestPath`指向JSON文件|