# MCP服务器-完整参考

Aspire公开了一个MCP（模型上下文协议）服务器**，允许AI编码助手查询和控制您正在运行的分布式应用程序，并搜索Aspire文档。这使得AI工具能够检查资源状态、读取日志、查看跟踪、重新启动服务和查找文档——所有这些都来自AI助手的上下文中。

参考:https://aspire.dev/get-started/configure-mcp/---

##设置：`aspire mcp init`配置MCP服务器最简单的方法是使用Aspire CLI：```bash
# Open a terminal in your project directory
aspire mcp init
```
该命令将引导您完成交互式设置：

1. **工作空间根目录** -提示工作空间根目录的路径（默认为当前目录）
2. **环境检测** -检测支持的AI环境（VS Code, Copilot CLI, Claude Code, OpenCode），并询问哪些配置
3. **剧作家MCP** -可选择提供配置剧作家MCP服务器与Aspire
4. **配置创建** -写入适当的配置文件（例如，`.vscode/mcp.json`）
5. **AGENTS.md** -如果一个AGENTS.md**还不存在，为AI代理创建一个`AGENTS.md`，其中包含特定于aspire的指令

b> **注：**`aspire mcp init`使用交互式提示（Spectre.Console）。它必须在真正的终端中运行——VS Code集成终端可能无法正确处理提示符。如果需要，请使用外部端子。

---

##了解配置当您运行`aspire mcp init`时，CLI将创建适合您检测到的环境的配置文件。VS Code（GitHub Copilot）

创建或更新`.vscode/mcp.json`：```json
{
  "servers": {
    "aspire": {
      "type": "stdio",
      "command": "aspire",
      "args": ["mcp", "start"]
    }
  }
}
```
## MCP工具

可用的工具取决于您的Aspire CLI版本。用`aspire --version`检查。

13.1+版本中可用的工具（稳定版）

####资源管理工具

这些工具需要运行AppHost （`aspire run`）。

|工具|描述|| ---------------------------- | ------------------------------------------------------------------------------------ |
|`list_resources`|列出所有资源，包括状态、健康状态、源、端点和命令|
|`list_console_logs`|列出资源|的控制台日志
|`list_structured_logs`|列出结构化日志，可选地按资源名称|进行过滤
|`list_traces`|列出分布式跟踪。跟踪可以使用可选的资源名参数|进行过滤
|`list_trace_structured_logs`|列出特定跟踪|的结构化日志
|`execute_resource_command`|执行资源命令（接受资源名和命令名）|

#### AppHost管理工具

|工具|描述|| ---------------- | ------------------------------------------------------------------------------------------- |
|`list_apphosts`|列出所有检测到的AppHost连接，显示工作目录作用域|的in/out|`select_apphost`|选择在多个运行|时使用哪个AppHost

####集成工具

这些工作无需运行AppHost。

|工具|描述|| ---------------------- | ----------------------------------------------------------------------------------------------------------------- |
|`list_integrations`|列出可用的Aspire托管集成（用于数据库、消息代理、云服务等的NuGet包）|
|`get_integration_docs`|获取特定Aspire托管集成包|的文档

13.2+新增工具（文档搜索）

> **版本门：**这些工具在[PR #14028]（https://github.com/dotnet/aspire/pull/14028）中添加，并在Aspire CLI **13.2**中发布。如果您使用的是13.1版本，这些工具将不会出现。要尽早获得它们，请更新到每日频道：`aspire update --self --channel daily`。

|工具|描述|| ------------- | ------------------------------------------------------------------------ |
|`list_docs`|列出来自aspire.dev |的所有可用文档
|`search_docs`|对已索引的aspire.dev文档|执行加权词法搜索
|`get_doc`|通过其段代码|检索特定文档

这些工具使用`llms.txt`规范索引aspire.dev内容，并提供加权词法搜索（标题10倍、摘要8倍、标题6倍、代码5倍、正文1倍）。它们可以在不运行AppHost的情况下工作。

###回退文档（13.1用户）

如果您使用的是Aspire CLI 13.1，并且没有`list_docs`/`search_docs`/`get_doc`，请使用**Context7**作为文档查询的后备。请参阅[SKILL.md文档研究部分]（../SKILL.md#1- research-aspire -documentation）了解详细信息。

---

从MCP中排除资源

通过标注资源，可以将资源和相关遥测数据排除在MCP结果之外：```csharp
var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.Api>("apiservice")
    .ExcludeFromMcp();  // Hidden from MCP tools

builder.AddProject<Projects.Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService);

builder.Build().Run();
```
---

支持AI助手`aspire mcp init`命令支持：

- [VS Code](https://code.visualstudio.com/docs/copilot/customization/mcp-servers) （GitHub Copilot）
-[副驾驶命令行]（https://docs.github.com/en/copilot/how-tos/use-copilot-agents/use-copilot-cli#add-an-mcp-server）
-[克劳德代码]（https://docs.claude.com/en/docs/claude-code/mcp）
——[OpenCode] (https://opencode.ai/docs/mcp-servers/)

MCP服务器使用STDIO传输协议，并且可以与支持该协议的其他代理编码环境一起工作。

---

##使用模式

在AI辅助下进行调试

一旦配置了MCP，你的人工智能助手可以：

1. **检查运行状态：**

-“列出我所有的Aspire资源及其状态”
-“数据库是否正常？”
-“API运行在哪个端口？”

2. * *读日志:* *

“给我看最近ML服务的日志”
“工人日志里有什么错误吗？”

3. * *观点痕迹:* *

“显示最后一个失败请求的跟踪”
“API→数据库调用的延迟是多少？”

4. 控制资源:* * * *-“重启API服务”
“在我调试队列的时候停止worker”

5. **搜索文档（13.2+）：**
“在Aspire文档中搜索Redis缓存”
-“如何配置服务发现？”
- _（需要CLI 13.2+）在13.1中，使用Context7或`list_integrations`/`get_integration_docs`来获取特定于集成的文档

---

##安全考虑

—MCP服务器只公开本地AppHost中的资源
-无需认证（仅限本地开发）
- STDIO传输仅适用于生成进程的AI工具
- **不要将MCP端点暴露给生产网络**

---

# #的局限性

-人工智能模型在数据处理上有限制。大数据字段（例如，堆栈跟踪）可能被截断。
-涉及大量遥测数据收集的请求可以通过省略旧项目来缩短。

---

# #故障排除如果遇到问题，请检查[在GitHub上打开MCP问题]（https://github.com/dotnet/aspire/issues?q=is%3Aissue+is%3Aopen+label%3Aarea-mcp）。

##参见Also

- [aspire MCP命令]（https://aspire.dev/reference/cli/commands/aspire-mcp/）
- [aspire MCP init命令]（https://aspire.dev/reference/cli/commands/aspire-mcp-init/）
- [aspire MCP启动命令]（https://aspire.dev/reference/cli/commands/aspire-mcp-start/）
- [GitHub Copilot在仪表盘]（https://aspire.dev/dashboard/copilot/）
-[我如何教AI阅读Aspire文档]