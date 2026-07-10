---
description: "Create, update, or review Azure IaC in Terraform using Azure Verified Modules (AVM)."
name: "Azure AVM Terraform mode"
tools: ["changes", "codebase", "edit/editFiles", "extensions", "fetch", "findTestFiles", "githubRepo", "new", "openSimpleBrowser", "problems", "runCommands", "runTasks", "runTests", "search", "searchResults", "terminalLastCommand", "terminalSelection", "testFailure", "usages", "vscodeAPI", "microsoft.docs.mcp", "azure_get_deployment_best_practices", "azure_get_schema_for_Bicep"]
---
# Azure AVM地形模式

使用Azure Verified Modules for Terraform通过预构建模块强制执行Azure最佳实践。

##发现模块

- Terraform注册表：搜索“avm”+资源，按合作伙伴标签过滤。
—AVM索引：`https://azure.github.io/Azure-Verified-Modules/indexes/terraform/tf-resource-modules/`# #使用

—**Examples**：复制example，将`source = "../../"`替换为`source = "Azure/avm-res-{service}-{resource}/azurerm"`，添加`version`，设置为`enable_telemetry`。
- **自定义**：复制提供指令，设置输入，引脚`version`。

# #版本控制

—终端：`https://registry.terraform.io/v1/modules/Azure/{module}/azurerm/versions`# #来源

—注册表：`https://registry.terraform.io/modules/Azure/{module}/azurerm/latest`- GitHub:`https://github.com/Azure/terraform-azurerm-avm-res-{service}-{resource}`命名约定

—资源：Azure/avm-res-{service}—{Resource}/azurerm
—模式：Azure/avm-ptn-{Pattern}/azurerm
—实用程序：Azure/avm-utl-{Utility}/azurerm

最佳实践-引脚模块和提供商版本
-从官方例子开始
-审查输入和输出
-启用遥测
—使用AVM实用模块
—遵循AzureRM提供商的要求
—修改后始终运行`terraform fmt`和`terraform validate`—使用`azure_get_deployment_best_practices`工具进行部署指导
-使用`microsoft.docs.mcp`工具查找Azure服务特定的指导GitHub Copilot代理的自定义说明

**重要**：当GitHub Copilot代理或GitHub Copilot编码代理在此存储库上工作时，必须执行以下本地单元测试以符合PR检查。未能运行这些测试将导致PR验证失败：```bash
./avm pre-commit
./avm tflint
./avm pr-check
```
这些命令必须在创建或更新任何拉取请求之前运行，以确保符合Azure Verified Modules标准，并防止CI/CD管道故障。
关于AVM进程的更多细节可以在[Azure验证模块贡献文档]（https://azure.github.io/Azure-Verified-Modules/contributing/terraform/testing/）中找到。