---
name: integrate-context-matic
description: 'Discovers and integrates third-party APIs using the context-matic MCP server. Uses `fetch_api` to find available API SDKs, `ask` for integration guidance, `model_search` and `endpoint_search` for SDK details. Use when the user asks to integrate a third-party API, add an API client, implement features with an external API, or work with any third-party API or SDK.'
---
# API集成

当用户要求集成第三方API或实现涉及外部API或SDK的任何内容时，请遵循此工作流。不要依赖自己对可用api或其功能的了解—始终使用上下文相关的MCP服务器。

何时申请

当用户：
-要求集成第三方API
—需要为外部服务添加客户端或SDK
-依赖于外部API的请求实现
-提到一个特定的API（如PayPal， Twilio）和实现或集成

# #工作流程

# # # 1。确保指导方针和技能的存在

# # # # 1。检测项目的主要语言

在检查指导方针或技能之前，通过检查工作空间来确定项目的主要编程语言：

|文件/模式|语言||---|---|
|`*.csproj`,`*.sln`|`csharp`|
|`package.json`与`"typescript"`深度或`.ts`文件|`typescript`|
|`requirements.txt`,`pyproject.toml`,`*.py`|`python`|
|`go.mod`,`*.go`|`go`|
|`pom.xml`,`build.gradle`,`*.java`|`java`|
|`Gemfile`,`*.rb`|`ruby`|
|`composer.json`,`*.php`|`php`|

在需要`language`的所有后续步骤中使用检测到的语言。

# # # # 1 b。检查现有的指导方针和技能

通过在工作区中查找指导方针和技能，检查是否已经为该项目添加了指导方针和技能。-`{language}-conventions`是**add_skills**产生的技能。
-`{language}-security-guidelines.md`和`{language}-test-guidelines.md`是由**add_guidelines**生成的特定于语言的指南文件。`update-activity-workflow.md`是由**add_guidelines**生成的工作流指南文件（它不是特定于语言的）。
-独立检查。不要把一组的存在当作另一组已经存在的证据。
- **如果本项目缺少任何所需的指南文件：**调用**add_guidelines**。
- **如果项目语言缺少`{language}-conventions`:**调用**add_skills**。
- **如果所需的所有指南文件和`{language}-conventions`已经存在：**跳过此步骤，继续执行步骤2。

# # # 2。发现可用api

调用**fetch_api**来查找可用的api -总是从这里开始。—始终使用步骤1a中检测到的语言提供`language`参数。
-总是提供`key`参数：从用户的请求传递APIname/key（例如`"paypal"`，`"twilio"`）。
-如果用户没有提供APIname/key，询问他们想要集成哪个API，然后用该值调用`fetch_api`。
—工具只在精确匹配时返回匹配的API，或者在没有精确匹配时返回完整的API目录（名称、描述和`key`）。
—根据名称和描述识别与用户请求匹配的API。
-在继续之前为用户请求的API提取正确的`key`。此键将用于与该API相关的所有后续工具调用。**如果请求的API不在列表中
-通知用户API当前在此插件中不可用（上下文）并停止。
-请求用户指导如何进行API的集成。

# # # 3。获取集成指导

-提供`ask`:`language`，`key`（从步骤2），和你的`query`。
-将复杂的问题分解成更小的重点查询，以获得最佳结果；
- _“我如何认证？”＿
- _“我如何创建付款？”＿
- _“速率限制是什么？”＿

# # # 4。查找SDK模型和端点（根据需要）

这些工具只返回定义—它们不调用api或生成代码。- **model_search** -查找model/object定义。
-提供：`language`，`key`，以及`query`的精确或部分区分大小写的模型名称（例如`availableBalance`，`TransactionId`）。
- **endpoint_search** -查找端点方法的详细信息。
-提供：`language`，`key`，以及一个精确或部分区分大小写的方法名`query`（例如`createUser`，`get_account_balance`）。

# # # 5。记录的里程碑

只要在代码或基础架构中具体达到其中一个，调用update_activity**（使用适当的`milestone`） ** -不仅仅是提到或计划：

里程碑|何时通过||---|---|
|`sdk_setup`|项目中已安装SDK包（如`npm install`、`pip install`、`go get`已运行成功）。|
| API凭据被显式写入项目的运行时环境中（例如存在于`.env`文件，秘密管理器或配置文件中）**和**在实际代码中引用。|
|`first_call_made`|编写并执行的第一个API调用代码|
|`error_encountered`|开发人员报告错误、错误响应或调用|失败
|`error_resolved`|修复应用和API调用确认工作|

# #检查表[]项目的主要语言检测（步骤1a）
- []`add_guidelines`调用如果指南文件丢失，否则跳过
-[]如果`{language}-conventions`缺失，则调用`add_skills`，否则跳过
-使用正确的`language`和`key`（API名称）调用`fetch_api`[]正确的`key`为所请求的API识别（或通知用户，如果没有找到）
- []`update_activity`只在code/infrastructure中具体达到里程碑时调用-从不用于问题，搜索或工具查找
- []`update_activity`调用适当的`milestone`在每个集成里程碑
- []`ask`用于集成指导和代码示例
- []`model_search`/`endpoint_search`根据需要使用SDK详细信息
-[]每次代码修改后的项目编译

# #笔记- **API未找到**：如果API从`fetch_api`中丢失，不要猜测SDK使用情况-通知用户该API目前在此插件中不可用并停止。
**update_activity和fetch_api**:`fetch_api`是API发现，而不是集成-在它之前不要调用`update_activity`。