构建MCP客户端。网

*消费* MCP服务器的简短参考。NET -用于测试服务器，构建代理线束，或将MCP连接到语义内核/ Microsoft.Extensions.AI管道中。

如果只是*运行*一个服务器，忽略这个文件。

# #包```bash
dotnet add package ModelContextProtocol.Core   # minimal: just client + transports
# or
dotnet add package ModelContextProtocol         # adds DI/hosting helpers
```
##通过STDIO连接（启动服务器进程）```csharp
using ModelContextProtocol.Client;

var transport = new StdioClientTransport(new StdioClientTransportOptions
{
    Command = "dotnet",
    Arguments = ["run", "--project", "../MyMcpServer"],
    EnvironmentVariables = new() { ["MY_API_KEY"] = "..." },
    ShutdownTimeout = TimeSpan.FromSeconds(10),
    StandardErrorLines = line => Console.Error.WriteLine($"[server] {line}")
});

await using var client = await McpClient.CreateAsync(transport);
```
`StandardErrorLines`是一个很好的调试辅助工具——您将看到服务器发生的日志。

##通过HTTP连接（可流）```csharp
using ModelContextProtocol.Client;

var transport = new HttpClientTransport(new HttpClientTransportOptions
{
    Endpoint = new Uri("https://my-server.example.com/mcp"),
    TransportMode = HttpTransportMode.StreamableHttp,
    ConnectionTimeout = TimeSpan.FromSeconds(30),
    AdditionalHeaders = new Dictionary<string, string>
    {
        ["Authorization"] = "Bearer ..."
    }
});

await using var client = await McpClient.CreateAsync(transport);
```
`TransportMode = AutoDetect`（默认值）首先尝试Streamable HTTP，然后退回到SSE—对于较旧的服务器很有用，但是对于新代码要使用`StreamableHttp`，因此失败很严重。

列出和调用工具```csharp
IList<McpClientTool> tools = await client.ListToolsAsync();

foreach (var t in tools)
    Console.WriteLine($"- {t.Name}: {t.Description}");

var echo = tools.First(t => t.Name == "Echo");
CallToolResult result = await echo.CallAsync(new Dictionary<string, object?>
{
    ["message"] = "hello"
});

if (result.IsError == true)
{
    var msg = result.Content.OfType<TextContentBlock>().FirstOrDefault()?.Text;
    Console.Error.WriteLine($"Tool failed: {msg}");
    return;
}

foreach (var block in result.Content)
{
    switch (block)
    {
        case TextContentBlock text:
            Console.WriteLine(text.Text);
            break;
        case ImageContentBlock image:
            File.WriteAllBytes("out.png", image.DecodedData.ToArray());
            break;
    }
}
```
列出提示和资源```csharp
IList<McpClientPrompt> prompts = await client.ListPromptsAsync();
GetPromptResult pr = await client.GetPromptAsync("code_review",
    new Dictionary<string, object?> { ["language"] = "csharp", ["code"] = "..." });

IList<McpClientResource> resources = await client.ListResourcesAsync();
ReadResourceResult rr = await client.ReadResourceAsync("config://app/settings");
```
订阅服务器通知```csharp
client.RegisterNotificationHandler(
    NotificationMethods.ToolListChangedNotification,
    async (notification, ct) =>
    {
        var updated = await client.ListToolsAsync(cancellationToken: ct);
        Console.WriteLine($"Tool list changed; now {updated.Count} tools.");
    });
```
处理服务器到客户端的请求（采样、获取、根）

如果您的服务器使用这些特性，您的客户端必须处理它们。在创建客户端时配置处理程序：```csharp
await using var client = await McpClient.CreateAsync(transport, new McpClientOptions
{
    Capabilities = new()
    {
        Sampling = new()
        {
            SamplingHandler = async (req, progress, ct) =>
            {
                // Route req.Messages to your IChatClient and return a CreateMessageResult.
                var response = await myChatClient.GetResponseAsync(/* convert */, ct);
                return new CreateMessageResult { /* fill in */ };
            }
        },
        Elicitation = new()
        {
            ElicitationHandler = async (req, ct) =>
            {
                // Show req.Message + req.RequestedSchema to the user; collect input.
                return new ElicitResult { Action = "accept", Content = collectedValues };
            }
        },
        Roots = new()
        {
            RootsHandler = async (req, ct) =>
            {
                return new ListRootsResult
                {
                    Roots = new[] { new Root { Uri = "file:///workspace", Name = "Workspace" } }
                };
            }
        }
    }
});
```
如果不提供处理程序，而服务器调用该特性，则调用失败，并显示“不支持方法”错误。

##使用MCP工具作为`IChatClient`功能工具

如果您将MCP插入`Microsoft.Extensions.AI`管道，则将工具暴露为`AIFunction`：```csharp
using Microsoft.Extensions.AI;

IList<McpClientTool> mcpTools = await client.ListToolsAsync();

var chatOptions = new ChatOptions
{
    Tools = mcpTools.Cast<AITool>().ToList()
};

var chatClient = new MyChatClient(...);   // any IChatClient
var response = await chatClient.GetResponseAsync(messages, chatOptions);
```
`McpClientTool`实现了`AIFunction`——函数调用中间件将调用正确的工具并自动将结果反馈给LLM。

##恢复会话（HTTP，有状态）```csharp
var transport = new HttpClientTransport(new HttpClientTransportOptions
{
    Endpoint = new Uri("https://my-server.example.com/mcp"),
    KnownSessionId = previousSessionId
});

await using var client = await McpClient.ResumeSessionAsync(transport, new ResumeClientSessionOptions
{
    ServerCapabilities = previousServerCapabilities,
    ServerInfo = previousServerInfo
});
```
对于存活时间较长的代理进程非常有用，这些代理进程可以在短暂的网络中断中幸存下来。