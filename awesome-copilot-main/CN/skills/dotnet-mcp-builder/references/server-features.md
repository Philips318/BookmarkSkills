其他服务器特性（完成、日志、进度、过滤器）

除了核心原语之外的小型MCP服务器功能的快速参考。每个部分都是短的，当其中一个出现时加载这个文件。

参数补全

补全功能允许主机自动完成提示参数和资源模板参数。用户开始打字；客户端询问服务器“什么是有效值？”

通过低级处理程序实现（尚不存在高级属性）：```csharp
builder.Services.Configure<McpServerOptions>(options =>
{
    options.Capabilities ??= new();
    options.Capabilities.Completions ??= new();

    options.Capabilities.Completions.CompleteHandler = async (ctx, ct) =>
    {
        // ctx.Params.Ref tells us what they're completing (a prompt or resource).
        // ctx.Params.Argument has the partial value typed so far.
        var partial = ctx.Params.Argument.Value ?? "";
        var matches = MyDataSource
            .Where(x => x.StartsWith(partial, StringComparison.OrdinalIgnoreCase))
            .Take(100)
            .ToArray();

        return new CompleteResult
        {
            Completion = new()
            {
                Values = matches,
                HasMore = false,
                Total = matches.Length
            }
        };
    };
});
```
适用于：项目id、文件名、依赖于动态数据的枚举值。

# #日志

服务器可以发出日志消息，主机可以在其UI中显示这些消息（LLM有时可以看到）。使用通过DI注入的标准`ILogger<T>`—SDK会引导它通过。```csharp
public class WeatherTools
{
    private readonly ILogger<WeatherTools> _log;
    public WeatherTools(ILogger<WeatherTools> log) => _log = log;

    [McpServerTool, Description("…")]
    public string GetWeather(string city)
    {
        _log.LogInformation("Looking up weather for {City}", city);
        return "...";
    }
}
```
对于STDIO服务器，请记住：控制台日志**必须**到stderr (`LogToStandardErrorThreshold = LogLevel.Trace`) -否则它会破坏JSON-RPC流。[`transport-stdio.md`] (./transport-stdio.md)。

通过MCP通道发送日志（所以主机UI可以看到它，而不仅仅是你的容器日志）：```csharp
await server.SendNotificationAsync(
    NotificationMethods.LoggingMessageNotification,
    new LoggingMessageNotificationParams
    {
        Level = LoggingLevel.Info,
        Logger = "weather",
        Data = JsonSerializer.SerializeToElement(new { city, latency_ms = 123 })
    },
    ct);
```
客户端可能已经设置了`setLevel`过滤器—不要在它下面发送垃圾邮件。

##进度通知

对于长时间运行的工具，发送进度更新，以便主机可以显示带有文本的微调器：```csharp
[McpServerTool, Description("Processes a large dataset.")]
public static async Task<string> Process(
    IMcpServer server,
    RequestContext<CallToolRequestParams> ctx,
    string datasetId,
    CancellationToken ct)
{
    var progressToken = ctx.Params.Meta?.ProgressToken;

    for (int i = 0; i < 100; i++)
    {
        await Task.Delay(50, ct);

        if (progressToken is not null)
        {
            await server.SendNotificationAsync(
                NotificationMethods.ProgressNotification,
                new ProgressNotificationParams
                {
                    ProgressToken = progressToken,
                    Progress = i + 1,
                    Total = 100,
                    Message = $"Processing item {i + 1} of 100"
                },
                ct);
        }
    }

    return "Done.";
}
```
只有当客户端在请求元数据中传递了`progressToken`时才发送进度-否则主机不会侦听。

通知处理程序（服务器端）

服务器可以响应客户端发送的通知：```csharp
options.Capabilities ??= new();
options.Capabilities.NotificationHandlers ??= [];

options.Capabilities.NotificationHandlers[NotificationMethods.RootsListChangedNotification] =
    async (notification, ct) =>
    {
        // Refresh root cache, etc.
    };

options.Capabilities.NotificationHandlers[NotificationMethods.CancelledNotification] =
    async (notification, ct) =>
    {
        // The client cancelled a request; if you have side-effects in flight, abort them.
    };
```
过滤器/中间件

SDK支持包装工具调用的过滤器(想想ASP。. NET核心中间件（MCP）。将它们用于横切关注点：认证检查、遥测、速率限制、审计日志。```csharp
builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithToolsFromAssembly()
    .WithCallToolFilter(async (ctx, next) =>
    {
        var sw = Stopwatch.StartNew();
        try
        {
            return await next(ctx);
        }
        finally
        {
            sw.Stop();
            ctx.Server.Services?
                .GetRequiredService<ILogger<Program>>()
                .LogInformation("Tool {Tool} took {Ms}ms",
                    ctx.Params.Name, sw.ElapsedMilliseconds);
        }
    });
```
还有类似的`With*Filter`帮助程序用于资源、提示和其他功能—查看SDK API参考以获取当前的帮助程序集。

服务器指令（系统提示）

您可以在初始化时提供发送给客户端的指令。主机可以在LLM的系统提示符中包含它们。```csharp
builder.Services.AddMcpServer(options =>
{
    options.ServerInstructions =
        "Use the booking tools to schedule meetings. " +
        "Always confirm with the user before booking via elicitation.";
});
```
保持简短——这里的每个令牌都需要用户付出代价。

##功能广告

如果你想“不”宣传你碰巧有代码的功能，你可以静音它：```csharp
builder.Services.AddMcpServer(options =>
{
    options.Capabilities = new()
    {
        Tools = new(),       // advertise tools
        Prompts = new(),     // advertise prompts
        Resources = null,    // do NOT advertise resources, even if some are registered
        Logging = new()
    };
});
```
默认情况下，SDK会发布你注册的所有内容——通常是正确的行为。