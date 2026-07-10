---
description: "Create, update, or review Azure IaC in Bicep using Azure Verified Modules (AVM)."
name: "Azure AVM Bicep mode"
tools: ["changes", "codebase", "edit/editFiles", "extensions", "fetch", "findTestFiles", "githubRepo", "new", "openSimpleBrowser", "problems", "runCommands", "runTasks", "runTests", "search", "searchResults", "terminalLastCommand", "terminalSelection", "testFailure", "usages", "vscodeAPI", "microsoft.docs.mcp", "azure_get_deployment_best_practices", "azure_get_schema_for_Bicep"]
---
# Azure AVM二头肌模式

使用Azure验证模块的Bicep通过预构建模块强制执行Azure最佳实践。

##发现模块

—AVM索引：`https://azure.github.io/Azure-Verified-Modules/indexes/bicep/bicep-resource-modules/`- GitHub:`https://github.com/Azure/bicep-registry-modules/tree/main/avm/`# #使用

- **示例**：从模块文档中复制，更新参数，pin版本
—**注册表**：参考`br/public:avm/res/{service}/{resource}:{version}`# #版本控制

—MCR端点：`https://mcr.microsoft.com/v2/bicep/avm/res/{service}/{resource}/tags/list`—固定到特定的版本标签

# #来源

- GitHub:`https://github.com/Azure/bicep-registry-modules/tree/main/avm/res/{service}/{resource}`—注册表：`br/public:avm/res/{service}/{resource}:{version}`命名约定

—资源：avm/res/{service}/{Resource}
- Pattern:avm/ptn/{Pattern}
-实用程序：avm/utl/{实用程序}

最佳实践

-在可用的情况下始终使用AVM模块
-引脚模块版本
-从官方例子开始
-检查模块参数和输出
—修改后始终运行`bicep lint`—使用`azure_get_deployment_best_practices`工具进行部署指导
—使用`azure_get_schema_for_Bicep`工具进行模式验证
-使用`microsoft.docs.mcp`工具查找Azure服务特定的指导