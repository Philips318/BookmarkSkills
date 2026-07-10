NuGet包和目标框架

三个官方包

所有包都位于[`ModelContextProtocol`NuGet配置文件]（https://www.nuget.org/profiles/ModelContextProtocol）下。官方的c# SDK库是[`modelcontextprotocol/csharp-sdk`](https://github.com/modelcontextprotocol/csharp-sdk)，由MCP项目和微软共同维护。

|包|使用时|带来||---|---|---|
| **`ModelContextProtocol`** |默认用于STDIO服务器和大多数项目|`Core`+`Microsoft.Extensions.Hosting`集成，属性发现（`AddMcpServer`，`WithToolsFromAssembly`等）|
| **`ModelContextProtocol.AspNetCore`** | HTTP（流）服务器托管在ASP。以上+`WithHttpTransport`和`MapMcp`|
| **`ModelContextProtocol.Core`** |纯客户端，自定义主机，低级场景，你不需要`Microsoft.Extensions.*`依赖|只是协议+传输+低级`McpServer.Create`/`McpClient.CreateAsync`|

**经验法则：**
-新的STDIO服务器→`ModelContextProtocol`+`Microsoft.Extensions.Hosting`。
-新的HTTP服务器→仅`ModelContextProtocol.AspNetCore`（它传递地拉入您需要的一切）。
-纯客户端app→`ModelContextProtocol.Core`（或`ModelContextProtocol`，如果你也想为客户端hosting/DI）。

# #版本

截至2026年，稳定线为**1。X **（撰写本文时`1.2.0`为当前值）。`0.x`行是预览版，有很大的差异——如果你发现文档或博客文章引用了`0.4`/`0.6`，就把它们当作过时的。查阅最新资料：```bash
dotnet search ModelContextProtocol --prerelease
```
##目标框架

SDK的目标是**`.NET 8.0`**和**`netstandard2.0`**。这意味着它运行：
-。净8 （lts）
-。网9
-。NET 10（当前LTS -建议用于新项目）
-。. NET Framework 4.6.2+通过netstandard2.0（罕见；仅适用于旧主机）

对于HTTP服务器，您需要一个支持ASP的TFM。. NET Core(所以。净8/9/10)。

项目设置命令

STDIO服务器```bash
dotnet new console -n MyMcpServer -f net10.0
cd MyMcpServer
dotnet add package ModelContextProtocol
dotnet add package Microsoft.Extensions.Hosting
```
HTTP（流）服务器```bash
dotnet new web -n MyMcpServer -f net10.0
cd MyMcpServer
dotnet add package ModelContextProtocol.AspNetCore
```
(`dotnet new web`给你一个最小的ASP。. NET Core项目——正是`MapMcp`所需要的。)

# # #客户```bash
dotnet new console -n MyMcpClient -f net10.0
cd MyMcpClient
dotnet add package ModelContextProtocol.Core
```
##可选，但通常很有用

| Package |为什么是||---|---|
|`Microsoft.Extensions.AI`|提供`IChatClient`、`ChatMessage`、`ChatRole`、`ChatOptions`——`AsSamplingChatClient()`和提示返回类型使用的抽象。|`DataContent`，`TextContent`等类型的传入，但值得了解。|
SDK为工具调用发出OTel跟踪和度量——如果用户有可观察性的故事，将它们连接起来。|`dnx`怎么样？

较新的Microsoft示例有时显示通过`dnx PackageName --version 1.2.3`启动服务器。这是一个有效的发布模型：将服务器作为NuGet包发布，让用户无需克隆即可运行它。它与服务器本身的构建方式是正交的——保持代码相同，只更改启动命令。