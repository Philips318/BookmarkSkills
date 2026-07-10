# STDIO传输

当服务器作为客户机（Claude Desktop、VS Code、MCP Inspector、一个自定义CLI）的子进程运行时，STDIO是正确的选择。客户端启动你的可执行文件；从标准输入读取JSON-RPC帧并将其写入标准输出。

何时选择STDIO

-本地优先服务器（文件系统访问，开发工具，CLI集成）。
-作为单个可执行文件或`dnx`-runnable NuGet包分发。
-你想要尽可能简单的部署故事（没有网络，没有授权）。
-你需要服务器到客户端的特性（采样，激发，根）- STDIO总是支持它们，不需要担心`Stateless`标志。

如果用户想要一个remote/multi-tenant服务器，使用[HTTP Streamable]（./transport-http.md）代替。

最小的服务器```bash
dotnet new console -n MyStdioServer -f net10.0
cd MyStdioServer
dotnet add package ModelContextProtocol
dotnet add package Microsoft.Extensions.Hosting
```

```csharp
// Program.cs
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;
using System.ComponentModel;

var builder = Host.CreateApplicationBuilder(args);

// CRITICAL: stdout is the JSON-RPC channel. Send all logs to stderr.
builder.Logging.AddConsole(o => o.LogToStandardErrorThreshold = LogLevel.Trace);

builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithToolsFromAssembly();

await builder.Build().RunAsync();

[McpServerToolType]
public static class EchoTool
{
    [McpServerTool, Description("Echoes the message back to the client.")]
    public static string Echo(string message) => $"hello {message}";
}
```
##stdout/stderrtrap

STDIO服务器中最常见的一个错误是编写标准输出，而不是JSON-RPC帧。然后，客户端将放弃连接并出现解析错误。

**无声地破坏STDIO的东西：**
-`Console.WriteLine(...)`在您的代码的任何地方。
-配置了默认控制台接收器（写入标准输出）的记录器。
如果附加了默认跟踪侦听器，则为`Trace.WriteLine(...)`。
-第三方库在启动时打印横幅。

* *防御清单:* *
1. 将日志配置为stderr **，然后再**其他内容（上面的代码片段就是这样做的）。
2. 不要对工具或启动代码吹毛求疵。使用`ILogger`注入到工具类中。
3. 如果依赖项有噪声，通过`ILogger`重定向它的日志，或者在启动时抑制它们。

##服务器标识

SDK在`initialize`响应中发送`serverInfo`（名称+版本）。默认情况下，它从程序集派生它们。覆盖:```csharp
builder.Services
    .AddMcpServer(options =>
    {
        options.ServerInfo = new()
        {
            Name = "my-stdio-server",
            Version = "1.0.0",
            Title = "My STDIO MCP Server"   // optional human-readable name
        };
    })
    .WithStdioServerTransport()
    .WithToolsFromAssembly();
```
##从客户端读取args/env客户端（例如Claude Desktop配置）通常使用参数和环境变量启动服务器。像阅读其他书籍一样阅读它们。NET应用程序:```csharp
string apiKey = Environment.GetEnvironmentVariable("MY_API_KEY")
    ?? throw new InvalidOperationException("MY_API_KEY not set");

string configPath = args.ElementAtOrDefault(0)
    ?? Path.Combine(Environment.CurrentDirectory, "config.json");
```
在README中记录期望的vars/args，以便用户知道在他们的客户机配置中应该放什么。

##连接到克劳德桌面

在`claude_desktop_config.json`:```json
{
  "mcpServers": {
    "my-server": {
      "command": "dotnet",
      "args": ["run", "--project", "C:/path/to/MyStdioServer"],
      "env": {
        "MY_API_KEY": "..."
      }
    }
  }
}
```
对于已发布的自包含可执行文件，将`command`/`args`替换为可执行路径。对于使用`dnx`的nuget分布式服务器：```json
"command": "dnx",
"args": ["MyMcpServer", "--version", "1.2.3"]
```
连接到VS Code（GitHub Copilot聊天）

在`.vscode/mcp.json`:```json
{
  "servers": {
    "my-server": {
      "type": "stdio",
      "command": "dotnet",
      "args": ["run", "--project", "${workspaceFolder}/src/MyMcpServer"]
    }
  }
}
```
##本地调试

最干净的工作流是[MCP Inspector](https://github.com/modelcontextprotocol/inspector)：```bash
npx @modelcontextprotocol/inspector dotnet run --project ./MyStdioServer
```
Inspector启动您的服务器，打开一个UI，并允许您以交互方式调用工具/列出资源/触发触发。请参阅[`testing.md`]（./testing.md）了解更多信息。

##安全关机`builder.Build().RunAsync()`已经处理SIGINT/SIGTERM.如果你有后台工作冲洗，使用`IHostApplicationLifetime`：```csharp
var host = builder.Build();
var lifetime = host.Services.GetRequiredService<IHostApplicationLifetime>();
lifetime.ApplicationStopping.Register(() =>
{
    // flush, close handles, etc. — keep it fast (<5s) so the client doesn't hang.
});
await host.RunAsync();
```
