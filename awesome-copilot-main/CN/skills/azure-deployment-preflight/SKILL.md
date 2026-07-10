---
name: azure-deployment-preflight
description: 'Performs comprehensive preflight validation of Bicep deployments to Azure, including template syntax validation, what-if analysis, and permission checks. Use this skill before any deployment to Azure to preview changes, identify potential issues, and ensure the deployment will succeed. Activate when users mention deploying to Azure, validating Bicep files, checking deployment permissions, previewing infrastructure changes, running what-if, or preparing for azd provision.'
---
# Azure部署飞行前验证

此技能在执行前验证Bicep部署，支持Azure CLI （`az`）和Azure Developer CLI （`azd`）工作流。

何时使用此技能

-在将基础设施部署到Azure之前
-在准备或审查肱二头肌文件时
预览一次部署会有什么变化
—验证权限是否满足部署要求
—执行`azd up`、`azd provision`、`az deployment`命令前

验证过程

按顺序执行以下步骤。即使前一个步骤失败，也要继续执行下一个步骤——在最终报告中捕获所有问题。

步骤1：检测项目类型

通过检查项目指标确定部署流程：

1. **检查azd项目**：在项目根目录中查找`azure.yaml`-如果发现→使用**azd工作流**
-如果没有找到→使用**az CLI工作流**2. **查找Bicep文件**：查找所有要验证的`.bicep`文件
—对于azd项目：先检查“`infra/`”目录，再检查“project root”目录
—单机：使用用户指定的文件或搜索常用位置（`infra/`,`deploy/`, project root）

3. **自动检测参数文件**：对于每个Bicep文件，查找匹配的参数文件：
-`<filename>.bicepparam`（肱二头肌参数-首选）
-`<filename>.parameters.json`（JSON参数）
—“`parameters.json`”或“`parameters/<env>.json`”在同一目录下

步骤2：验证肱二头肌语法

在尝试部署验证之前，运行Bicep CLI检查模板语法：```bash
bicep build <bicep-file> --stdout
```
**捕获内容：**
—line/column数字语法错误
-警告信息
-构建success/failure状态

**如果未安装Bicep命令行：**
—注意报告中的问题
-继续步骤3 （Azure将在假设期间验证语法）

###步骤3：运行飞行前验证

根据步骤1中检测到的项目类型选择适当的验证。

####对于azd项目（azure.yaml存在）

使用`azd provision --preview`验证部署：```bash
azd provision --preview
```
如果指定了一个环境或存在多个环境：```bash
azd provision --preview --environment <env-name>
```
####独立二头肌（无azure.yaml）

从Bicep文件的`targetScope`声明中确定部署范围：

|目标范围|命令||--------------|---------|
|`resourceGroup`（默认）|`az deployment group what-if`|
|`subscription`|`az deployment sub what-if`|
|`managementGroup`|`az deployment mg what-if`|
|`tenant`|`az deployment tenant what-if`|

**首先运行提供者验证级别：**```bash
# Resource Group scope (most common)
az deployment group what-if \
  --resource-group <rg-name> \
  --template-file <bicep-file> \
  --parameters <param-file> \
  --validation-level Provider

# Subscription scope
az deployment sub what-if \
  --location <location> \
  --template-file <bicep-file> \
  --parameters <param-file> \
  --validation-level Provider

# Management Group scope
az deployment mg what-if \
  --location <location> \
  --management-group-id <mg-id> \
  --template-file <bicep-file> \
  --parameters <param-file> \
  --validation-level Provider

# Tenant scope
az deployment tenant what-if \
  --location <location> \
  --template-file <bicep-file> \
  --parameters <param-file> \
  --validation-level Provider
```
* *回退策略:* *

如果`--validation-level Provider`因权限错误（RBAC）失败，请重试`ProviderNoRbac`：```bash
az deployment group what-if \
  --resource-group <rg-name> \
  --template-file <bicep-file> \
  --validation-level ProviderNoRbac
```
注意报告中的回退—用户可能缺乏完全部署权限。

###步骤4：捕获假设结果

解析what-if输出以对资源更改进行分类：

|修改类型|符号|含义||-------------|--------|---------|
|创建|`+`|将创建新的资源|
|删除|`-`|资源将被删除|
|修改|`~`|资源属性将改变|
| NoChange |`=`|资源不变|
|忽略|`*`|未分析资源（已达到限制）|
|部署|`!`|将部署资源（变化未知）|

对于已修改的资源，捕获特定的属性更改。

###步骤5：生成报告

在**项目根**中创建一个Markdown报告文件，命名为：- `preflight-report.md`
使用来自[references/REPORT-TEMPLATE.md]（references/REPORT-TEMPLATE.md）的模板结构。

* *报告部分:* *
1. **摘要** -总体状态，时间戳，文件验证，目标范围
2. **执行的工具** -运行的命令，版本，使用的验证级别
3. **问题** -所有错误和警告的严重性和补救措施
4. **如果结果** -资源到create/modify/delete/unchanged5. **建议** -可执行的后续步骤

##所需信息

在运行验证之前，收集：

|信息| |必需的|获取方式|-------------|--------------|---------------|
|资源组|`az deployment group`|询问用户或查看已有的`.azure/`config |
|订阅|所有部署|`az account show`或询问用户|
|位置|Sub/MG/Tenant范围|询问用户或使用配置|中的默认值
|环境| azd项目|`azd env list`或询问用户|

如果缺少所需的信息，在继续之前提示用户。

##错误处理

参见[references/ERROR-HANDLING.md]（references/ERROR-HANDLING.md）获得详细的错误处理指导。

**关键原则：**即使发生错误也要继续验证。在最终报告中记录所有问题。

|错误类型|操作||------------|--------|
|未登录|报告中备注，建议使用`az login`或`azd auth login`|
|权限被拒绝|退回到`ProviderNoRbac`，在|报告中说明
|二头肌语法错误|包括所有错误，继续其他文件|
|工具未安装|在报告中注意，跳过验证步骤|
|资源组未找到|报告中备注，建议创建|

##工具要求

该技能使用以下工具：

- **Azure CLI** (`az`) -版本2.76.0+推荐用于`--validation-level`- **Azure Developer CLI** (`azd`) -用于`azure.yaml`的项目
- **Bicep CLI** (`bicep`) -用于语法验证
- **Azure MCP工具** -用于文档查找和最佳实践

启动前检查工具可用性：```bash
az --version
azd version
bicep --version
```
##示例工作流

1. 用户：“在运行前验证我的Bicep部署”
2. 代理检测到`azure.yaml`→azd项目
3. 代理找到`infra/main.bicep`和`infra/main.bicepparam`4. 代理运行`bicep build infra/main.bicep --stdout`5. 代理运行`azd provision --preview`6. Agent在项目根目录下生成`preflight-report.md`7. 代理向用户总结调查结果

##参考文档

-[验证命令参考]（references/VALIDATION-COMMANDS.md）
—[报表模板]（references/REPORT-TEMPLATE.md）
-[错误处理指南]（references/ERROR-HANDLING.md）