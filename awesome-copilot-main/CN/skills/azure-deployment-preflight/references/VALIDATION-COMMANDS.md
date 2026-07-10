#验证命令参考

此参考文档记录了用于Azure部署飞行前验证的所有命令。

Azure Developer CLI （azd）

azd provision—预览

无需部署即可预览azd项目的基础架构更改。```bash
azd provision --preview [options]
```
* *选择:* *
|选项|描述||--------|-------------|
|`--environment`，`-e`| |使用的环境名称
|`--no-prompt`|接受默认值，不提示|
|`--debug`|打开调试日志开关|
|`--cwd`|设置工作目录|

* *例子:* *```bash
# Preview with default environment
azd provision --preview

# Preview specific environment
azd provision --preview --environment dev

# Preview without prompts (CI/CD)
azd provision --preview --no-prompt
```
**输出：**显示将要创建、修改或删除的资源。

### azd auth login

对Azure进行azd操作的身份验证。```bash
azd auth login [options]
```
* *选择:* *
|选项|描述||--------|-------------|
|`--check-status`|不登录|检查登录状态
|`--use-device-code`|使用设备代码流程|
|`--tenant-id`|指定租户|
|`--client-id`|服务主体客户端ID |

### azd env list

列出可用的环境。```bash
azd env list
```
---

Azure CLI （az）

az部署组假设

预览资源组部署的更改。```bash
az deployment group what-if \
  --resource-group <rg-name> \
  --template-file <bicep-file> \
  [options]
```
* *必需的参数:* *
| |参数说明||-----------|-------------|
|`--resource-group`，`-g`|目标资源组名称|
|`--template-file`，`-f`| Bicep文件|的路径

* *可选参数:* *
| |参数说明||-----------|-------------|
|`--parameters`，`-p`|参数文件或内联值|
|`--validation-level`|`Provider`（默认）、`ProviderNoRbac`或`Template`|
|`--result-format`|`FullResourcePayloads`（默认）或`ResourceIdOnly`|
|`--no-pretty-print`|输出原始JSON，用于解析|
|`--name`、`-n`|部署名称|
|`--exclude-change-types`|从输出|中排除特定的更改类型

* *的验证级别:* *
|级别|描述|用例||-------|-------------|----------|
|`Provider`|完全验证与RBAC检查|默认，最彻底的|
|`ProviderNoRbac`|完全验证，只读权限|缺少部署权限|
|`Template`|仅静态语法验证|快速语法检查|

* *例子:* *```bash
# Basic what-if
az deployment group what-if \
  --resource-group my-rg \
  --template-file main.bicep

# With parameters and full validation
az deployment group what-if \
  --resource-group my-rg \
  --template-file main.bicep \
  --parameters main.bicepparam \
  --validation-level Provider

# Fallback without RBAC checks
az deployment group what-if \
  --resource-group my-rg \
  --template-file main.bicep \
  --validation-level ProviderNoRbac

# JSON output for parsing
az deployment group what-if \
  --resource-group my-rg \
  --template-file main.bicep \
  --no-pretty-print
```
### az部署子假设

预览订阅级部署的更改。```bash
az deployment sub what-if \
  --location <location> \
  --template-file <bicep-file> \
  [options]
```
* *必需的参数:* *
| |参数说明||-----------|-------------|
|`--location`，`-l`|部署元数据|的位置
|`--template-file`，`-f`| Bicep文件|的路径

* *例子:* *```bash
az deployment sub what-if \
  --location eastus \
  --template-file main.bicep \
  --parameters main.bicepparam \
  --validation-level Provider
```
### az部署mg假设

预览管理组部署的更改。```bash
az deployment mg what-if \
  --location <location> \
  --management-group-id <mg-id> \
  --template-file <bicep-file> \
  [options]
```
* *必需的参数:* *
| |参数说明||-----------|-------------|
|`--location`，`-l`|部署元数据|的位置
|`--management-group-id`，`-m`|目标器管理组ID |
|`--template-file`，`-f`| Bicep文件|的路径

### az部署租户假设

预览租户级部署的更改。```bash
az deployment tenant what-if \
  --location <location> \
  --template-file <bicep-file> \
  [options]
```
* *必需的参数:* *
| |参数说明||-----------|-------------|
|`--location`，`-l`|部署元数据|的位置
|`--template-file`，`-f`| Bicep文件|的路径

### az登录

验证到Azure CLI。```bash
az login [options]
```
* *选择:* *
|选项|描述||--------|-------------|
|`--tenant`、`-t`|租户ID或域|
|`--use-device-code`|使用设备代码流程|
|`--service-principal`|以业务主体|登录

### az帐户显示

显示当前订阅上下文。```bash
az account show
```
az组已存在

检查资源组是否存在。```bash
az group exists --name <rg-name>
```
---

##肱二头肌CLI

二头肌锻炼

编译Bicep到ARM JSON并验证语法。```bash
bicep build <bicep-file> [options]
```
* *选择:* *
|选项|描述||--------|-------------|
|`--stdout`|输出到stdout，而不是文件|
|`--outdir`|输出目录|
|`--outfile`|输出文件路径|
|`--no-restore`|跳过模块恢复|

* *例子:* *```bash
# Validate syntax (output to stdout, no file created)
bicep build main.bicep --stdout > /dev/null

# Build to specific directory
bicep build main.bicep --outdir ./build

# Validate multiple files
for f in *.bicep; do bicep build "$f" --stdout; done
```
**错误输出格式：**```
/path/to/file.bicep(22,51) : Error BCP064: Found unexpected tokens in interpolated expression.
/path/to/file.bicep(22,51) : Error BCP004: The string at this location is not terminated.
```
格式:`<file>(<line>,<column>) : <severity> <code>: <message>`###二头肌-版本

检查Bicep命令行版本。```bash
bicep --version
```
---

##参数文件检测

###二头肌参数（.bicepparam）

现代肱二头肌参数文件（推荐）：```bicep
using './main.bicep'

param location = 'eastus'
param environment = 'dev'
param tags = {
  environment: 'dev'
  project: 'myapp'
}
```
**检测模式：**`<template-name>.bicepparam`（.parameters.json）

传统的ARM参数文件：```json
{
  "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentParameters.json#",
  "contentVersion": "1.0.0.0",
  "parameters": {
    "location": { "value": "eastus" },
    "environment": { "value": "dev" }
  }
}
```
* *检测模式:* *
——`<template-name>.parameters.json`- `parameters.json`
——`parameters/<env>.json`###使用命令参数```bash
# Bicep parameters file
az deployment group what-if \
  --resource-group my-rg \
  --template-file main.bicep \
  --parameters main.bicepparam

# JSON parameters file
az deployment group what-if \
  --resource-group my-rg \
  --template-file main.bicep \
  --parameters @parameters.json

# Inline parameter overrides
az deployment group what-if \
  --resource-group my-rg \
  --template-file main.bicep \
  --parameters main.bicepparam \
  --parameters location=westus
```
---

确定部署范围

检查Bicep文件的`targetScope`声明：```bicep
// Resource Group (default if not specified)
targetScope = 'resourceGroup'

// Subscription
targetScope = 'subscription'

// Management Group
targetScope = 'managementGroup'

// Tenant
targetScope = 'tenant'
```
**范围到命令映射：**

| targetScope |命令|必选参数||-------------|---------|---------------------|
|`resourceGroup`|`az deployment group what-if`|`--resource-group`|
|`subscription`|`az deployment sub what-if`|`--location`|
|`managementGroup`|`az deployment mg what-if`|`--location`,`--management-group-id`|
|`tenant`|`az deployment tenant what-if`|`--location`|

---

##版本要求

|工具|最低版本|推荐版本|主要特性||------|-----------------|---------------------|--------------|
| Azure CLI | 2.14.0 | 2.76.0+ |`--validation-level`switch |
| Azure Developer CLI | 1.0.0 |最新|`--preview`flag |
|肱二头肌CLI | 0.4.0 |最新|最佳错误消息|

* *检查版本:* *```bash
az --version
azd version
bicep --version
```
