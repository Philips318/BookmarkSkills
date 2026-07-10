---
description: 'Azure Verified Modules (AVM) and Bicep'
applyTo: '**/*.bicep, **/*.bicepparam'
---
# Azure验证模块（AVM）二头肌

# #概述

Azure验证模块（AVM）是遵循Azure最佳实践的预构建、测试和验证的Bicep模块。使用这些模块可以自信地创建、更新或查看Azure基础架构即代码（IaC）。

##发现模块

###二头肌公共注册

—搜索模块：`br/public:avm/res/{service}/{resource}:{version}`—浏览可用模块：`https://github.com/Azure/bicep-registry-modules/tree/main/avm/res`—例如：`br/public:avm/res/storage/storage-account:0.30.0`官方AVM指数

- **肱二头肌资源模块**:`https://raw.githubusercontent.com/Azure/Azure-Verified-Modules/refs/heads/main/docs/static/module-indexes/BicepResourceModules.csv`- **二头肌模式模块**:`https://raw.githubusercontent.com/Azure/Azure-Verified-Modules/refs/heads/main/docs/static/module-indexes/BicepPatternModules.csv`模块文档

- **GitHub存储库**:`https://github.com/Azure/bicep-registry-modules/tree/main/avm/res/{service}/{resource}`- **README**：每个模块包含全面的文档和示例

##模块使用

###从例子

1. 回顾模块README在`https://github.com/Azure/bicep-registry-modules/tree/main/avm/res/{service}/{resource}`2. 从模块文档中复制示例代码
3. 参考模块使用`br/public:avm/res/{service}/{resource}:{version}`4. 配置必需和可选参数

###示例用法```bicep
module storageAccount 'br/public:avm/res/storage/storage-account:0.30.0' = {
  name: 'storage-account-deployment'
  scope: resourceGroup()
  params: {
    name: storageAccountName
    location: location
    skuName: 'Standard_LRS'
    tags: tags
  }
}
```
AVM模块不可用

如果资源类型不存在AVM模块，请使用最新稳定API版本的本机Bicep资源声明。

命名约定

模块引用

—**资源模块**:`br/public:avm/res/{service}/{resource}:{version}`- **模式模块**:`br/public:avm/ptn/{pattern}:{version}`—例如：`br/public:avm/res/network/virtual-network:0.7.2`符号名称

-对所有名称（变量、参数、资源、模块）使用lowerCamelCase
-使用资源类型描述性名称（例如，`storageAccount`，而不是`storageAccountName`）
-避免在符号名称中使用“name”后缀，因为它们代表资源，而不是资源的名称
—避免使用后缀区分变量和参数

##版本管理

版本固定最佳实践

-始终固定到特定的模块版本：`:{version}`-使用语义版本控制（例如，`:0.30.0`）
-在升级前检查模块变更日志
—首先在非生产环境中进行版本升级测试

开发最佳实践模块发现和使用

-✅**总是**在创建原始资源之前检查现有的AVM模块
-✅**在实现**模块之前查看**文档和示例
-✅**Pin**模块版本明确
-✅**使用**类型从模块可用时（从模块导入类型）
-✅**优先选择** AVM模块而不是原始资源声明

代码结构

-✅**在文件顶部用`@sys.description()`装饰器声明**参数
-✅**指定“**`@minLength()`”和“`@maxLength()`”作为命名参数
-✅**谨慎使用**`@allowed()`装饰器以避免阻塞有效的部署
-✅**设置**默认值安全的测试环境（低成本sku）
-✅**在复杂表达式中使用**变量，而不是嵌入到资源属性中
-✅**利用**`loadJsonContent()`外部配置文件

资源引用-✅**使用**符号名作为引用（例如`storageAccount.id`），而不是`reference()`或`resourceId()`-✅**通过符号名创建**依赖，而不是显式的`dependsOn`-✅**使用**`existing`关键字访问其他资源中的属性
-✅**通过点表示法访问**模块输出（例如，`storageAccount.outputs.resourceId`）

资源命名

-✅**使用**`uniqueString()`与有意义的前缀唯一的名称
-✅**添加**前缀，因为一些资源不允许名称以数字开头
-✅**尊重**资源特定的命名约束（长度，字符）

###子资源

-✅**避免**子资源过多嵌套
-✅**使用**`parent`属性或嵌套，而不是手动构造名称

# # #安全-❌**永远**不要在输出中包含秘密或密钥
-✅**直接在输出中使用**资源属性（例如，`storageAccount.outputs.primaryBlobEndpoint`）
-✅**在可能的情况下启用**管理身份
-✅**启用网络隔离时，关闭**公共访问

# # #类型

-✅**从可用的模块中导入**类型：`import { deploymentType } from './module.bicep'`-✅**对于复杂的参数结构使用**自定义类型
-✅**利用**类型推断变量

# # #文档

-✅**包含**有用的`//`注释复杂的逻辑
-✅**使用**`@sys.description()`的所有参数，并明确说明
-✅**文件**非明显的设计决策

##验证要求

###构建验证（强制）

在对Bicep文件进行任何更改后，运行以下命令以确保所有文件构建成功：```shell
# Ensure Bicep CLI is up to date
az bicep upgrade

# Build and validate changed Bicep files
az bicep build --file main.bicep
```
###二头肌参数文件

-✅**总是**更新随`*.bicepparam`文件修改`*.bicep`文件
-✅**验证**参数文件是否匹配当前参数定义
-✅**在提交前用参数文件测试**部署

##工具集成

使用可用的工具

—**模式信息**：资源模式使用`azure_get_schema_for_Bicep`—**部署指导**：使用`azure_get_deployment_best_practices`工具
- **服务文档**：使用`microsoft.docs.mcp`用于Azure特定服务的指导

###GitHub Copilot使用肱二头肌时：

1. 在创建资源前检查是否存在AVM模块
2. 使用官方模块示例作为起点
3. 修改完成后运行`az bicep build`4. 更新随附的`.bicepparam`文件
5. 文档自定义或偏离示例

# #故障排除

###常见问题1. **模块版本**：始终在模块引用中指定准确的版本
2. **缺少依赖**：确保资源在依赖模块之前创建
3. **验证失败**：执行`az bicep build`命令识别syntax/type错误
4. **参数文件**：确保参数更改时`.bicepparam`文件更新

支持资源

- **AVM文档**:`https://azure.github.io/Azure-Verified-Modules/`- **肱二头肌注册**:`https://github.com/Azure/bicep-registry-modules`- **肱二头肌文档**:`https://learn.microsoft.com/azure/azure-resource-manager/bicep/`- **最佳实践**:`https://learn.microsoft.com/azure/azure-resource-manager/bicep/best-practices`合规性检查表

在提交任何二头肌代码之前：-[]可用的AVM模块
-[]模块版本固定
[]代码编译成功（`az bicep build`）
-[]随附`.bicepparam`文件更新
- []`@sys.description()`-[]用于引用的符号名
-[]输出无秘密
-[]根据需要输入imported/defined-[]对复杂逻辑添加注释
-[]遵循lowerCamelCase命名约定