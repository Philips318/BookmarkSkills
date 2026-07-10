#阶段4：部署代理

该文件包含阶段4的详细说明。当用户在阶段3（代码审查）完成后批准部署时，请阅读并遵循此文件。

---

**🚨🚨🚨第四阶段强制执行命令-绝不能跳过任何步骤🚨🚨🚨**

以下5个步骤必须严格按照**的顺序**执行。不得省略或跳过任何步骤。
即使用户使用“deploy it”、“go ahead”、“do it”等方式请求部署，也要始终按顺序从步骤1开始。```
Step 1: Verify prerequisites (az login, subscription, resource group)
    ↓
Step 2: What-if validation (az deployment group what-if) ← Must execute
    ↓
Step 3: Generate preview diagram (02_arch_diagram_preview.html) ← Must generate
    ↓
Step 4: Actual deployment after user final confirmation (az deployment group create)
    ↓
Step 5: Generate deployment result diagram (03_arch_diagram_result.html)
```
永远不要做以下事情
—直接执行`az deployment group create`，不带What-if
-跳过生成预览图（`02_arch_diagram_preview.html`）
-继续部署，不向用户显示What-if结果
—仅提供`az`命令供用户手动执行

---

步骤1：验证先决条件```powershell
# Verify az CLI installation and login
az account show 2>&1
```
如果未登录，请要求用户运行`az login`。
代理不能直接输入或存储凭据。

创建资源组：```powershell
az group create --name "<RG_NAME>" --location "<LOCATION>"  # Location confirmed in Phase 1
```
→确认成功后进行下一步

###步骤2：验证→假设验证-🚨必选

**请勿跳过此步骤。无论用户请求部署有多紧急，都要执行它

**步骤2-A：先运行Validate（快速预验证）**`what-if`可以在违反Azure策略、资源引用错误等情况下无限期挂起，而不会出现错误消息。
为了防止这种情况，**总是先运行`validate`**。Validate可以快速返回错误。```powershell
# validate — Quickly catches policy violations, schema errors, parameter issues
az deployment group validate `
  --resource-group "<RG_NAME>" `
  --parameters main.bicepparam
```
- **验证成功**→继续到步骤2-B（假设）
- **验证失败**→分析错误信息，修复二头肌，重新编译，重新验证
- Azure策略违规（`RequestDisallowedByPolicy`）→在Bicep中反映策略需求（例如，`azureADOnlyAuthentication: true`）
-架构错误→修复APIversion/properties—参数错误→修复参数文件

步骤2-B：运行What-if**

在验证通过后运行假设。

**选择参数传递方式：**
—所有`@secure()`参数均为默认值→使用`.bicepparam`—如果`@secure()`参数需要用户输入→使用`--template-file`+ JSON参数文件```powershell
# Method 1: Use .bicepparam (when all @secure() parameters have defaults)
az deployment group what-if `
  --resource-group "<RG_NAME>" `
  --parameters main.bicepparam

# Method 2: Use JSON parameter file (when @secure() parameters require user input)
az deployment group what-if `
  --resource-group "<RG_NAME>" `
  --template-file main.bicep `
  --parameters main.parameters.json `
  --parameters secureParam='value'
```
→总结假设结果并呈现给用户。

**⏱️如果执行方法和超时处理：**

What-if在Azure服务器端执行资源验证，因此可能需要一些时间，具体取决于service/region.**总是执行`initial_wait: 300`（5分钟）。**如5分钟内未完成，自动超时。```powershell
# Always set initial_wait: 300 when calling the powershell tool
# mode: "sync", initial_wait: 300
az deployment group what-if `
  --resource-group "<RG_NAME>" `
  --parameters main.bicepparam
```
** 5分钟内完成**→正常进行（总结结果→预览图→部署确认）

**未在5分钟内完成（超时）**→立即停止`stop_powershell`，并提供选择给用户：```
ask_user({
  question: "What-if validation did not complete within 5 minutes. The Azure server response is delayed. How would you like to proceed?",
  choices: [
    "Retry (Recommended)",
    "Skip What-if and deploy directly"
  ]
})
```
**如果选择“重试”：**重新执行与`initial_wait: 300`相同的命令。重试最多2次。
**如果选择“跳过假设，直接部署”：**
—根据第一阶段草图生成预览图
-告知用户风险：
> **⚠️没有进行假设验证的部署。**可能发生意外的资源变更。请在部署后在Azure门户中进行验证。

永远不要做以下事情
—执行时未设置`initial_wait`，导致无限期等待
-让代理任意决定“what-if是可选的”并跳过它
—自动切换到部署模式，不会超时询问用户
-因为“部署更快”之类的原因跳过假设

###步骤3：基于假设结果的预览图-🚨必选

**请勿跳过此步骤。总是在What-if成功时生成预览图从假设结果中使用要部署的实际资源（资源名称、类型、位置、计数）重新生成图。
保持第1阶段（`01_arch_diagram_draft.html`）的草稿原样，并生成预览为`02_arch_diagram_preview.html`。
草稿可以随时重新打开。```
## Architecture to Be Deployed (Based on What-if)

[Interactive diagram link — 02_arch_diagram_preview.html]
(Design draft: 01_arch_diagram_draft.html)

Resources to be created (N items):
[What-if results summary table]

Deploy these resources? (Yes/No)
```
待用户确认后，继续执行步骤4。**如果没有预览图，请勿继续部署

步骤4：实际部署

只有当用户审查了预览图和假设结果并批准了部署时才执行。
**使用与What-if.**相同的参数传递方法```powershell
$deployName = "deploy-$(Get-Date -Format 'yyyyMMdd-HHmmss')"

# Method 1: Use .bicepparam
az deployment group create `
  --resource-group "<RG_NAME>" `
  --parameters main.bicepparam `
  --name $deployName `
  2>&1 | Tee-Object -FilePath deployment.log

# Method 2: Use JSON parameter file
az deployment group create `
  --resource-group "<RG_NAME>" `
  --template-file main.bicep `
  --parameters main.parameters.json `
  --name $deployName `
  2>&1 | Tee-Object -FilePath deployment.log
```
在部署期间定期监控进度：```powershell
az deployment group show `
  --resource-group "<RG_NAME>" `
  --name "<DEPLOYMENT_NAME>" `
  --query "{status:properties.provisioningState, duration:properties.duration}" `
  -o table
```
###部署失败处理

当部署失败时，一些资源可能保持在“失败”状态。在这种状态下重新部署会导致类似`AccountIsNotSucceeded`的错误。

**⚠️删除资源为破坏性命令。在执行之前，一定要向用户解释情况并获得批准```
[Resource name] failed during deployment.
To redeploy, the failed resources must be deleted first.

Delete and redeploy? (Yes/No)
```
删除失败的资源，并在用户批准后重新部署。

**🔹软删除资源处理（防止重部署阻塞）：**

当资源组在部署失败后被删除时，认知服务（Foundry）、密钥库等保持在软删除状态。
使用相同的名称重新部署会导致`FlagMustBeSetForRestore`、`Conflict`错误。

**在重新部署前始终检查：**```powershell
# Check soft-deleted Cognitive Services
az cognitiveservices account list-deleted -o table

# Check soft-deleted Key Vault
az keyvault list-deleted -o table
```
**分辨率选项（为用户提供选择）：**```
ask_user({
  question: "Soft-deleted resources from a previous deployment were found. How would you like to handle this?",
  choices: [
    "Purge and redeploy (Recommended) - Clean delete then create new",
    "Redeploy in restore mode - Recover existing resources"
  ]
})
```
**注意-密钥库使用`enablePurgeProtection: true`:**
-无法清除（必须等待保留期到期）
-不能用相同的名称重新创建
解决方案：更改密钥库名称**并重新部署（例如，在`uniqueString()`seed中添加时间戳）
—向用户说明情况，指导用户修改名称

步骤5：部署完成——根据实际资源和报告生成图表

一旦部署完成，查询实际部署的资源并生成最终的架构图。

**步骤1：查询已部署资源**```powershell
az resource list --resource-group "<RG_NAME>" --output json
```
**步骤2：从实际资源生成图表**

从查询结果中提取资源名称、类型、sku和端点，并使用内置的图表引擎生成最终的图表。
注意文件名，避免覆盖之前的图表：
-`01_arch_diagram_draft.html`-设计稿（保留）
-`02_arch_diagram_preview.html`-如果预览（保留）
-`03_arch_diagram_result.html`-部署结果的最终版本

用实际部署的资源信息填充图的服务JSON：
-`name`：实际的资源名称（例如，`foundry-duru57kxgqzxs`）
—`sku`：实际SKU
-`details`：实际值，如端点，位置等。

**步骤3：报告**```
## Deployment Complete!

[Interactive architecture diagram — 03_arch_diagram_result.html]
(Design draft: 01_arch_diagram_draft.html | What-if preview: 02_arch_diagram_preview.html)

Created resources (N items):
[Dynamically extracted resource names, types, and endpoints from actual deployment results]

## Next Steps
1. Verify resources in Azure Portal
2. Check Private Endpoint connection status
3. Additional configuration guidance if needed

## Cleanup Command (If Needed)
az group delete --name <RG_NAME> --yes --no-wait
```
---

在部署后处理架构变更请求

**部署完成后，用户请求资源additions/changes/deletions时，不要直接跳转到Bicep/deployment.**
始终返回到阶段1并首先更新体系结构。

* *过程:* *

1. **确认用户意图** -首先询问他们是否想要添加到现有的部署架构中：   ```
   Would you like to add a VM to the currently deployed architecture?
   Current configuration: [Deployed services summary]
   ```
2. **返回阶段1 -应用Delta确认规则**
—使用已存在的部署结果（`03_arch_diagram_result.html`）作为当前状态基线
-验证新服务所需的字段（SKU，网络，区域可用性等）
-通过ask_user确认未确定的项目
-事实核查（MS文档获取+交叉验证）

3. **生成更新的架构图**
—将现有已部署资源+新资源合并为`04_arch_diagram_update_draft.html`-显示给用户并获得确认：   ```
   ## Updated Architecture

   [Interactive diagram — 04_arch_diagram_update_draft.html]
   (Previous deployment result: 03_arch_diagram_result.html)

   **Changes:**
   - Added: [New services list]
   - Removed: [Removed services list] (if any)

   Proceed with this configuration?
   ```
4. **确认后，按**顺序进行第2→3→4阶段
-增加新的资源模块到现有的二头肌
-审查→假设→部署（增量部署）

永远不要做以下事情
-当部署后请求更改时，直接跳转到生成Bicep而不更新架构图
—忽略现有部署状态，隔离创建新资源
—不与用户确认是否添加到现有架构中