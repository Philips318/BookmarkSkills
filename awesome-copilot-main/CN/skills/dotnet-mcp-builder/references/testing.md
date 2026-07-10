#测试和本地调试

三个工作流程：使用MCP检查器进行交互测试、进程内集成测试和ci友好单元测试。

## MCP检查器（交互式）

[MCP Inspector]（https://github.com/modelcontextprotocol/inspector）是手动测试服务器的首选工具。它启动服务器，通过STDIO或HTTP连接，并为您提供list/call工具的UI，查看资源、触发触发、查看日志和检查原始JSON-RPC帧。

# # #的头```bash
npx @modelcontextprotocol/inspector dotnet run --project ./MyMcpServer
```
在`--`后传递env或参数：```bash
npx @modelcontextprotocol/inspector \
  dotnet run --project ./MyMcpServer -- \
  --some-flag value
```
# # # HTTP

正常启动服务器（`dotnet run`），然后在检查器中选择“Streamable HTTP”并输入URL（例如`http://localhost:3001`）。

###使用它

-验证工具描述是否清晰（检查器呈现它们，就像LLM会使用它们一样）。
-在没有真正的法学硕士学位的情况下完成启发流程。
-在提交bug报告时捕获准确的JSON-RPC有效负载。

进程内集成测试（推荐）

最干净的测试设置使用`InMemoryTransport`（或较低级别的`StreamServerTransport`/`StreamClientTransport`）在同一进程中将真实服务器和真实客户机连接在一起。没有子进程，没有网络。```csharp
using System.IO.Pipelines;
using ModelContextProtocol;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;
using Xunit;

public class WeatherToolsTests
{
    [Fact]
    public async Task GetWeather_returns_text()
    {
        var clientToServer = new Pipe();
        var serverToClient = new Pipe();

        await using var server = McpServer.Create(
            new StreamServerTransport(
                clientToServer.Reader.AsStream(),
                serverToClient.Writer.AsStream()),
            new McpServerOptions
            {
                ToolCollection =
                [
                    McpServerTool.Create(
                        (string city) => $"{city}: 18°C",
                        new() { Name = "GetWeather" })
                ]
            });

        var serverTask = server.RunAsync();

        await using var client = await McpClient.CreateAsync(
            new StreamClientTransport(
                clientToServer.Writer.AsStream(),
                serverToClient.Reader.AsStream()));

        var tools = await client.ListToolsAsync();
        var tool = tools.Single(t => t.Name == "GetWeather");

        var result = await tool.CallAsync(new Dictionary<string, object?>
        {
            ["city"] = "Brussels"
        });

        Assert.False(result.IsError);
        var text = result.Content.OfType<TextContentBlock>().Single().Text;
        Assert.Equal("Brussels: 18°C", text);
    }
}
```
这种风格允许你断言*暴露的*行为（真正的客户端看到的），而不是内部细节。

使用sampling/elicitation/roots的测试工具

注入MCP服务器，但提供模拟客户机功能。使用上面的内存模式，在客户端上注册处理程序：```csharp
await using var client = await McpClient.CreateAsync(clientTransport, new McpClientOptions
{
    Capabilities = new()
    {
        Sampling = new()
        {
            SamplingHandler = (req, progress, ct) =>
                Task.FromResult(new CreateMessageResult
                {
                    Content = [new TextContentBlock { Text = "MOCK SUMMARY" }]
                })
        },
        Elicitation = new()
        {
            ElicitationHandler = (req, ct) =>
                Task.FromResult(new ElicitResult
                {
                    Action = "accept",
                    Content = JsonSerializer.SerializeToNode(new { confirm = true })
                                .AsObject().ToDictionary(kv => kv.Key, kv => JsonDocument.Parse(kv.Value!.ToJsonString()).RootElement)
                })
        }
    }
});
```
现在工具的`server.SampleAsync`/`server.ElicitAsync`调用命中确定性模拟。

在DI层进行单元测试

对于没有mcp特定行为的纯逻辑，只需测试类：```csharp
[Fact]
public void Echo_prepends_hello()
{
    Assert.Equal("hello world", EchoTool.Echo("world"));
}
```
`[McpServerTool]`属性不会影响MCP连接之外的运行时行为——你的方法只是方法。

##在开发期间从Claude Desktop /VS Code运行

对于端到端的“感觉像真实的东西”测试：

1. 运行`dotnet publish -c Release`（或者只是`dotnet build`，并使用`dotnet run`）。
2. 点克劳德桌面/VS Code在二进制或`dotnet run --project ...`。有关配置片段，请参见[`transport-stdio.md`]（./transport-stdio.md）。
3. 重启主机。
4. 从聊天中触发工具。

在迭代时，设置`dotnet watch run --project ...`，以便服务器在编辑时重新启动；主机通常在下一次工具调用时重新连接。

# # CI

一个典型的CI管道：```yaml
- run: dotnet restore
- run: dotnet build --no-restore
- run: dotnet test --no-build --logger "trx;LogFileName=test-results.trx"
```
没有什么MCP-specific。内存中的传输测试在`dotnet test`运行的任何地方运行——没有Node，没有Docker。

常见的诊断技巧

- **“工具没有显示”：**在快速测试中调用`client.ListToolsAsync()`并转储名称。如果您的工具不在那里，则注册错误。
- **“LLM一直滥用工具”：**打开检查器并查看LLM看到的schema/description。大多数“模型是愚蠢的”问题实际上都忽略了`[Description]`。
**“Sampling/elicitation抛出‘method not supported’”：**客户端不通告该功能。要么是针对不支持它的主机进行测试（Inspector两者都支持），要么是您的内存客户端缺少处理程序。
- **"HTTP返回404为/":**检查`app.MapMcp()`被称为*和*你击中正确的路径。`MapMcp("/mcp")`表示URL是`http://host/mcp`，而不是`http://host/`。