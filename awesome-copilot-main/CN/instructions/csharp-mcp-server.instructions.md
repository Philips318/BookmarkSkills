---
description: 'Instructions for building Model Context Protocol (MCP) servers using the C# SDK'
applyTo: '**/*.cs, **/*.csproj'
---
c# MCP服务器开发

# #指令—大多数项目使用**ModelContextProtocol** NuGet包（预发布版）：`dotnet add package ModelContextProtocol --prerelease`—使用**ModelContextProtocol。基于http的MCP服务器
—使用**ModelContextProtocol。核心**最小依赖（仅客户端或低级服务器api）
-始终使用`LogToStandardErrorThreshold = LogLevel.Trace`配置日志错误，以避免干扰演播室传输
—对包含MCP工具的类使用`[McpServerToolType]`属性
-在方法上使用`[McpServerTool]`属性，将它们公开为工具
—使用`System.ComponentModel`中的`[Description]`属性来记录工具和参数
—在工具方法中支持依赖注入—将`McpServer`、`HttpClient`或其他服务作为参数注入
-使用`McpServer.AsSamplingChatClient()`从工具内向客户端发出采样请求
在类上使用`[McpServerPromptType]`，在方法上使用`[McpServerPrompt]`-对于工作室传输，在构建服务器时使用`WithStdioServerTransport()`—使用`WithToolsFromAssembly()`自动发现和注册呃，所有当前组装的工具
-工具方法可以同步也可以异步（返回`Task`或`Task<T>`）
-始终包括对工具和参数的全面描述，以帮助法学硕士理解其目的
—在异步工具中使用`CancellationToken`参数以获得适当的取消支持
—返回简单类型（string， int等）或可以序列化为JSON的复杂对象
对于细粒度控制，使用`McpServerOptions`和自定义处理程序，如`ListToolsHandler`和`CallToolHandler`—对于协议级错误使用`McpProtocolException`，并设置适当的`McpErrorCode`值
-使用来自同一SDK或任何兼容的MCP客户端的`McpClient`测试MCP服务器
-使用Microsoft.Extensions.Hosting构建项目，以进行适当的DI和生命周期管理最佳实践

-保持工具方法的重点和单一用途
—使用有意义的工具名称，清楚地表明其功能
-提供详细的描述，解释工具的功能、期望的参数和返回的结果
-验证输入参数，并抛出`McpProtocolException`与`McpErrorCode.InvalidParams`无效输入
-使用结构化日志来帮助调试，而不会污染标准输出
-使用`[McpServerToolType]`将相关工具组织成逻辑类
-在公开访问外部资源的工具时考虑安全隐患
—使用内置的DI容器来管理服务的生命周期和依赖关系
—正确处理错误，返回有意义的错误信息
-在与llm集成之前单独测试工具

##常见模式

基本服务器设置```csharp
var builder = Host.CreateApplicationBuilder(args);
builder.Logging.AddConsole(options => 
    options.LogToStandardErrorThreshold = LogLevel.Trace);
builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithToolsFromAssembly();
await builder.Build().RunAsync();
```
简单的工具```csharp
[McpServerToolType]
public static class MyTools
{
    [McpServerTool, Description("Description of what the tool does")]
    public static string ToolName(
        [Description("Parameter description")] string param) => 
        $"Result: {param}";
}
```
带有依赖注入的工具```csharp
[McpServerTool, Description("Fetches data from a URL")]
public static async Task<string> FetchData(
    HttpClient httpClient,
    [Description("The URL to fetch")] string url,
    CancellationToken cancellationToken) =>
    await httpClient.GetStringAsync(url, cancellationToken);
```
工具与采样```csharp
[McpServerTool, Description("Analyzes content using the client's LLM")]
public static async Task<string> Analyze(
    McpServer server,
    [Description("Content to analyze")] string content,
    CancellationToken cancellationToken)
{
    var messages = new ChatMessage[]
    {
        new(ChatRole.User, $"Analyze this: {content}")
    };
    return await server.AsSamplingChatClient()
        .GetResponseAsync(messages, cancellationToken: cancellationToken);
}
```
