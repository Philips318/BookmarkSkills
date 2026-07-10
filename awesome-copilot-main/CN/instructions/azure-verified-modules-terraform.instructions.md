---
description: ' Azure Verified Modules (AVM) and Terraform'
applyTo: '**/*.terraform, **/*.tf, **/*.tfvars, **/*.tfstate, **/*.tflint.hcl, **/*.tf.json, **/*.tfvars.json'
---
# Azure验证模块（AVM） Terraform

# #概述

Azure验证模块（AVM）是预先构建、测试和验证的Terraform和Bicep模块，遵循Azure最佳实践。使用这些模块可以自信地创建、更新或查看Azure基础架构即代码（IaC）。GitHub Copilot代理的自定义指令

**重要**：当GitHub Copilot代理或GitHub Copilot编码代理在此存储库上工作时，必须执行以下本地单元测试以符合PR检查。未能运行这些测试将导致PR验证失败：```bash
./avm pre-commit
./avm tflint
./avm pr-check
```
这些命令必须在创建或更新任何拉取请求之前运行，以确保符合Azure Verified Modules标准，并防止CI/CD管道故障。
关于AVM进程的更多细节可以在[Azure验证模块贡献文档]（https://azure.github.io/Azure-Verified-Modules/contributing/terraform/testing/）中找到。

**未能运行这些测试将导致PR验证失败并阻止合并成功

##发现模块

Terraform注册表

—搜索“avm +资源名”
-通过“合作伙伴”标签筛选以查找官方AVM模块
—示例：搜索“avm存储帐户”→按合作伙伴过滤

官方AVM指数

> **注意：**以下链接总是指向主分支上最新版本的CSV文件。正如预期的那样，这意味着文件可能会随着时间的推移而更改。如果您需要某个时间点的版本，请考虑在URL中使用特定的发布标签。- **地形资源模块**:`https://raw.githubusercontent.com/Azure/Azure-Verified-Modules/refs/heads/main/docs/static/module-indexes/TerraformResourceModules.csv`- **地形模式模块**:`https://raw.githubusercontent.com/Azure/Azure-Verified-Modules/refs/heads/main/docs/static/module-indexes/TerraformPatternModules.csv`- **地形实用模块**:`https://raw.githubusercontent.com/Azure/Azure-Verified-Modules/refs/heads/main/docs/static/module-indexes/TerraformUtilityModules.csv`## Terraform模块用法

###从例子

1. 从模块文档中复制示例代码
2. 将`source = "../../"`替换为`source = "Azure/avm-res-{service}-{resource}/azurerm"`3. 添加`version = "~> 1.0"`（使用最新可用的）
4. 设置`enable_telemetry = true`###从头开始

1. 从模块文档中复制提供说明
2. 配置必需和可选的输入
3. 固定模块版本
4. 使遥测

###示例用法```hcl
module "storage_account" {
  source  = "Azure/avm-res-storage-storageaccount/azurerm"
  version = "~> 0.1"

  enable_telemetry    = true
  location            = "East US"
  name                = "mystorageaccount"
  resource_group_name = "my-rg"

  # Additional configuration...
}
```
命名约定

模块类型

—**资源模块**:`Azure/avm-res-{service}-{resource}/azurerm`—例如：`Azure/avm-res-storage-storageaccount/azurerm`- **模式模块**:`Azure/avm-ptn-{pattern}/azurerm`—例如：`Azure/avm-ptn-aks-enterprise/azurerm`- **实用模块**:`Azure/avm-utl-{utility}/azurerm`—例如：`Azure/avm-utl-regions/azurerm`服务命名

-使用烤肉串案例的服务和资源
-遵循Azure服务名称（例如，`storage-storageaccount`,`network-virtualnetwork`）

##版本管理

###检查可用版本

—终端：`https://registry.terraform.io/v1/modules/Azure/{module}/azurerm/versions`—例如：`https://registry.terraform.io/v1/modules/Azure/avm-res-storage-storageaccount/azurerm/versions`版本固定最佳实践

—使用悲观版本约束：`version = "~> 1.0"`-引脚到生产的特定版本：`version = "1.2.3"`-在升级前总是检查更新日志

##模块源

Terraform注册表

- **URL模式**:`https://registry.terraform.io/modules/Azure/{module}/azurerm/latest`- **示例**:`https://registry.terraform.io/modules/Azure/avm-res-storage-storageaccount/azurerm/latest`GitHub仓库

- **URL模式**:`https://github.com/Azure/terraform-azurerm-avm-{type}-{service}-{resource}`- * * * *例子:
—资源：`https://github.com/Azure/terraform-azurerm-avm-res-storage-storageaccount`—图案：`https://github.com/Azure/terraform-azurerm-avm-ptn-aks-enterprise`开发最佳实践

模块使用情况-✅**总是**引脚模块和提供商的版本
-✅**从模块文档中的官方示例开始**
-✅**实施前审核**所有输入和输出
-✅**使能**遥测：`enable_telemetry = true`-✅**使用** AVM实用模块的常见模式
-✅**遵循** AzureRM提供商的要求和约束

代码质量

-✅**总是**修改后运行`terraform fmt`-✅**总是**修改后运行`terraform validate`-✅**使用**有意义的变量名和描述
-✅**添加**适当的标签和元数据
-✅**文档**复杂配置

验证要求

在创建或更新任何拉取请求之前：```bash
# Format code
terraform fmt -recursive

# Validate syntax
terraform validate

# AVM-specific validation (MANDATORY)
./avm pre-commit
./avm tflint
./avm pr-check
```
##工具集成

使用可用的工具

—**部署指导**：使用`azure_get_deployment_best_practices`工具
- **服务文档**：使用`microsoft.docs.mcp`工具进行Azure特定服务的指导
—**模式信息**：使用`azure_get_schema_for_Bicep`作为Bicep资源

###GitHub Copilot当使用AVM存储库时：

1. 在创建新资源之前，始终检查现有模块
2. 使用官方的例子作为起点
3. 在提交之前运行所有验证测试
4. 记录任何自定义或与示例的偏差

##常见模式

资源组模块```hcl
module "resource_group" {
  source  = "Azure/avm-res-resources-resourcegroup/azurerm"
  version = "~> 0.1"

  enable_telemetry = true
  location         = var.location
  name            = var.resource_group_name
}
```
虚拟网络模块```hcl
module "virtual_network" {
  source  = "Azure/avm-res-network-virtualnetwork/azurerm"
  version = "~> 0.1"

  enable_telemetry    = true
  location            = module.resource_group.location
  name                = var.vnet_name
  resource_group_name = module.resource_group.name
  address_space       = ["10.0.0.0/16"]
}
```
# #故障排除

###常见问题

1. **版本冲突**：始终检查模块和提供商版本之间的兼容性
2. **缺失的依赖**：确保首先创建所有必需的资源
3. **验证失败**：在提交之前运行AVM验证工具
4. **文档**：始终参考最新的模块文档

支持资源

- **AVM文档**:`https://azure.github.io/Azure-Verified-Modules/`- **GitHub问题**：报告特定模块的GitHub存储库中的问题
- **社区**:Azure Terraform Provider GitHub讨论

合规性检查表

在提交任何avm相关代码之前：-[]固定模块版本
—[]启用遥测功能
-[]代码被格式化（`terraform fmt`）
-[]代码被验证（`terraform validate`）
- [] AVM预提交检查通过（`./avm pre-commit`）
- [] TFLint检查通过（`./avm tflint`）
- [] AVM PR检查通过（`./avm pr-check`）
-[]更新文档
-[]示例已经过测试并正常工作