# CLI参考-完整的命令参考

Aspire CLI （`aspire`）是用于创建、运行和发布分布式应用程序的主要接口。它是跨平台的，独立安装(不耦合到。. NET CLI，尽管`dotnet`命令也可以使用)。

**测试对象：** Aspire CLI 13.1.0

---

# #安装```bash
# Linux / macOS
curl -sSL https://aspire.dev/install.sh | bash

# Windows PowerShell
irm https://aspire.dev/install.ps1 | iex

# Verify
aspire --version

# Update the CLI itself
aspire update --self
```
---

##全局选项

所有命令都支持这些选项：

|选项|描述|| --------------------- | ---------------------------------------------- |
|`-d, --debug`|打开控制台|的调试日志
|`--non-interactive`|禁用所有交互式提示符和旋转器|
|`--wait-for-debugger`|执行|前等待调试器挂载
|`-?, -h, --help`|显示帮助和使用信息|
|`--version`|显示版本信息|

---

##命令参考

# # #`aspire new`从模板创建新项目。```bash
aspire new [<template>] [options]

# Options:
#   -n, --name <name>        Project name
#   -o, --output <dir>       Output directory
#   -s, --source <source>    NuGet source for templates
#   -v, --version <version>  Version of templates to use
#   --channel <channel>      Channel (stable, daily)

# Examples:
aspire new aspire-starter
aspire new aspire-starter -n MyApp -o ./my-app
aspire new aspire-ts-cs-starter
aspire new aspire-py-starter
aspire new aspire-apphost-singlefile
```
可用的模板:

-`aspire-starter`- ASP。. NETCore/Blazorstarter + AppHost +测试
-`aspire-ts-cs-starter`- ASP。. NETCore/React+ AppHost
-`aspire-py-starter`-FastAPI/React+ AppHost
-`aspire-apphost-singlefile`-空单文件AppHost

# # #`aspire init`在现有项目或解决方案中初始化Aspire。```bash
aspire init [options]

# Options:
#   -s, --source <source>    NuGet source for templates
#   -v, --version <version>  Version of templates to use
#   --channel <channel>      Channel (stable, daily)

# Example:
cd my-existing-solution
aspire init
```
将AppHost和ServiceDefaults项目添加到现有解决方案中。交互式提示引导您选择要编排的项目。

# # #`aspire run`使用DCP（开发人员控制平面）在本地启动所有资源。```bash
aspire run [options] [-- <additional arguments>]

# Options:
#   --project <path>       Path to AppHost project file

# Examples:
aspire run
aspire run --project ./src/MyApp.AppHost
```
行为:

1. 构建AppHost项目
2. 启动DCP引擎
3. 按依赖顺序（DAG）创建资源
4. 等待对封闭资源进行运行状况检查
5. 在默认浏览器中打开仪表板
6. 将日志流到终端

按`Ctrl+C`，安全停止所有资源。

# # #`aspire add`向AppHost添加托管集成。```bash
aspire add [<integration>] [options]

# Options:
#   --project <path>         Target project file
#   -v, --version <version>  Version of integration to add
#   -s, --source <source>    NuGet source for integration

# Examples:
aspire add redis
aspire add postgresql
aspire add mongodb
```
###`aspire publish`（预览）

从AppHost资源模型生成部署清单。```bash
aspire publish [options] [-- <additional arguments>]

# Options:
#   --project <path>                   Path to AppHost project file
#   -o, --output-path <path>           Output directory (default: ./aspire-output)
#   --log-level <level>                Log level (trace, debug, information, warning, error, critical)
#   -e, --environment <env>            Environment (default: Production)
#   --include-exception-details        Include stack traces in pipeline logs

# Examples:
aspire publish
aspire publish --output-path ./deploy
aspire publish -e Staging
```
# # #`aspire config`管理Aspire配置设置。```bash
aspire config <subcommand>

# Subcommands:
#   get <key>              Get a configuration value
#   set <key> <value>      Set a configuration value
#   list                   List all configuration values
#   delete <key>           Delete a configuration value

# Examples:
aspire config list
aspire config set telemetry.enabled false
aspire config get telemetry.enabled
aspire config delete telemetry.enabled
```
# # #`aspire cache`管理磁盘缓存，用于CLI操作。```bash
aspire cache <subcommand>

# Subcommands:
#   clear                  Clear all cache entries

# Example:
aspire cache clear
```
###`aspire deploy`（预览）

将Aspire应用程序的内容部署到其定义的部署目标。```bash
aspire deploy [options] [-- <additional arguments>]

# Options:
#   --project <path>                   Path to AppHost project file
#   -o, --output-path <path>           Output path for deployment artifacts
#   --log-level <level>                Log level (trace, debug, information, warning, error, critical)
#   -e, --environment <env>            Environment (default: Production)
#   --include-exception-details        Include stack traces in pipeline logs
#   --clear-cache                      Clear deployment cache for current environment

# Example:
aspire deploy --project ./src/MyApp.AppHost
```
###`aspire do`（预览）

执行特定的管道步骤及其依赖项。```bash
aspire do <step> [options] [-- <additional arguments>]

# Options:
#   --project <path>                   Path to AppHost project file
#   -o, --output-path <path>           Output path for artifacts
#   --log-level <level>                Log level (trace, debug, information, warning, error, critical)
#   -e, --environment <env>            Environment (default: Production)
#   --include-exception-details        Include stack traces in pipeline logs

# Example:
aspire do build-images --project ./src/MyApp.AppHost
```
###`aspire update`（预览）

更新Aspire项目中的集成，或者更新CLI本身。```bash
aspire update [options]

# Options:
#   --project <path>       Path to AppHost project file
#   --self                 Update the Aspire CLI itself to the latest version
#   --channel <channel>    Channel to update to (stable, daily)

# Examples:
aspire update                          # Update project integrations
aspire update --self                   # Update the CLI itself
aspire update --self --channel daily   # Update CLI to daily build
```
# # #`aspire mcp`管理MCP（模型上下文协议）服务器。```bash
aspire mcp <subcommand>

# Subcommands:
#   init    Initialize MCP server configuration for detected agent environments
#   start   Start the MCP server
```
# # # #`aspire mcp init````bash
aspire mcp init

# Interactive — detects your AI environment and creates config files.
# Supported environments:
# - VS Code (GitHub Copilot)
# - Copilot CLI
# - Claude Code
# - OpenCode
```
为检测到的AI工具生成适当的配置文件。
详细信息请参见[MCP服务器]（mcp-server.md）。

# # # #`aspire mcp start````bash
aspire mcp start

# Starts the MCP server using STDIO transport.
# This is typically invoked by your AI tool, not run manually.
```
---

不存在的命令

以下命令在Aspire CLI 13.1中**无效**。使用替代品:

|无效命令|可选|| --------------- | -------------------------------------------------------------------- |
|`aspire build`|使用`dotnet build ./AppHost`|
|`aspire test`|使用`dotnet test ./Tests`|
|`aspire dev`|使用`aspire run`（包括文件观看）|`aspire new --help`用于模板，`aspire add`用于集成|

---

# #。. NET CLI等价`dotnet`CLI可以执行一些Aspire任务：

| Aspire CLI |. NET CLI等价|| --------------------------- | -------------------------------- |
|`aspire new aspire-starter`|`dotnet new aspire-starter`|
|`aspire run`|`dotnet run --project ./AppHost`|
|N/A|`dotnet build ./AppHost`|
|N/A|`dotnet test ./Tests`|

Aspire CLI通过`publish`、`deploy`、`add`、`mcp`、`config`、`cache`、`do`和`update`这些没有直接对应的`dotnet`命令来增加价值。