#工具

工具是LLM可以调用的函数。在c# SDK中，它们是标记为`[McpServerToolType]`的类上的普通方法，每个方法标记为`[McpServerTool]`。SDK从方法签名和`[Description]`属性生成JSON Schema。

##工具解剖```csharp
using System.ComponentModel;
using ModelContextProtocol.Server;

[McpServerToolType]
public class WeatherTools
{
    // Static or instance — both work. Instance methods get DI for the containing class.
    [McpServerTool, Description("Returns the current weather for a city.")]
    public static string GetWeather(
        [Description("City name, e.g. 'Brussels'")] string city,
        [Description("Units: 'celsius' or 'fahrenheit'")] string units = "celsius")
    {
        return $"{city}: 18°{units[0]}";
    }
}
```
注册它（其中之一）：```csharp
.WithToolsFromAssembly()        // discovers all [McpServerToolType] in the calling assembly
.WithTools<WeatherTools>()      // explicit, single class
```
显示给LLM的工具名称是`GetWeather`（将PascalCase转换为snake_case是** *不是** *自动的—除非显式设置`Name`，否则所看到的就是所得到的）。

##属性选项```csharp
[McpServerTool(
    Name = "get_weather",                 // override the tool name
    Title = "Get current weather",        // human-readable display name
    Destructive = false,                  // hint: tool modifies state irreversibly
    Idempotent = true,                    // hint: same args ⇒ same result
    OpenWorld = true,                     // hint: interacts with external systems
    ReadOnly = true                       // hint: doesn't mutate any state
)]
[Description("Returns the current weather for a city.")]
public static string GetWeather(...) { ... }
```
行为提示（`Destructive`,`Idempotent`,`OpenWorld`,`ReadOnly`）是建议性的-客户使用它们来决定诸如自动批准之类的事情。它们不会改变运行时行为。

Async, cancel， DI```csharp
[McpServerTool, Description("Fetches the latest commits for a repo.")]
public async Task<IEnumerable<Commit>> GetCommits(
    string owner,
    string repo,
    IGitHubClient github,                         // injected from DI
    CancellationToken cancellationToken)          // injected by the SDK
{
    return await github.GetCommitsAsync(owner, repo, cancellationToken);
}
```
SDK识别并特殊处理这些参数类型——它们不会出现在工具模式中：
-`IMcpServer`/`McpServer`-当前服务器（用于`ElicitAsync`，`SampleAsync`,`RequestRootsAsync`，发送通知）。
-`CancellationToken`-从JSON-RPC请求传播。
-`RequestContext<CallToolRequestParams>`-如果需要的话，完整的请求上下文。
-`IServiceProvider`-请求作用域的服务提供者。
-任何可以从DI中解析的，SDK可以识别为非原始有效负载的内容。

其他所有内容都被视为JSON-RPC参数并进入模式。

##返回类型

SDK将您返回的任何内容序列化到适当的内容块中。实践指南:

|返回类型| LLM看到的||---|---|
|`string`|单一文本内容块。|
|将`int`、`bool`、`double`等字符串化为文本内容块。|
|任何DTO (record/class) |在文本内容块中序列化为JSON，加上支持它的客户端的结构化内容。|
|`IEnumerable<T>`的DTOs | JSON数组。|`ContentBlock`/`ImageContentBlock`/`AudioContentBlock`/`EmbeddedResourceBlock`|那一块，没动过。|
|`IEnumerable<ContentBlock>`|按顺序排列多个块。|
|全控-设置`Content`，`StructuredContent`,`IsError`。|

返回LLM可以处理的结构化数据```csharp
public record Forecast(string City, double TempC, string Conditions);

[McpServerTool, Description("Returns a 3-day forecast.")]
public static Forecast[] GetForecast(string city) =>
    new[]
    {
        new Forecast(city, 18.0, "sunny"),
        new Forecast(city, 16.5, "cloudy"),
        new Forecast(city, 14.2, "rain"),
    };
```
SDK将数组作为JSON文本块（对于较旧的客户机）和`structuredContent`（对于较新的客户机）发出，并从`Forecast`推断出输出模式。

返回图像/音频```csharp
[McpServerTool, Description("Generates a chart and returns it as a PNG.")]
public static ImageContentBlock RenderChart(string title)
{
    byte[] png = Renderer.Render(title);
    return ImageContentBlock.FromBytes(png, "image/png");
}

[McpServerTool, Description("Synthesises speech.")]
public static AudioContentBlock Speak(string text)
{
    byte[] wav = Tts.Synthesize(text);
    return AudioContentBlock.FromBytes(wav, "audio/wav");
}
```
混合内容块```csharp
[McpServerTool, Description("Returns the chart and a caption.")]
public static IEnumerable<ContentBlock> RenderAnnotatedChart(string title)
{
    byte[] png = Renderer.Render(title);
    return new ContentBlock[]
    {
        new TextContentBlock { Text = $"Chart for: {title}" },
        ImageContentBlock.FromBytes(png, "image/png"),
        new TextContentBlock { Text = "Generated at " + DateTime.UtcNow.ToString("u") }
    };
}
```
返回一个嵌入的资源

当工具结果*是*用户可能想要重用的文档时有用：```csharp
[McpServerTool, Description("Looks up a contract.")]
public static EmbeddedResourceBlock GetContract(string id)
{
    return new EmbeddedResourceBlock
    {
        Resource = new TextResourceContents
        {
            Uri = $"contracts://{id}",
            MimeType = "text/markdown",
            Text = LoadContract(id)
        }
    };
}
```
# #错误

工具可能产生两种类型的错误：

工具级错误（LLM可以读取并从中恢复）

抛出任何异常- SDK捕获它并返回带有`IsError = true`的`CallToolResult`和文本块中的异常消息：```csharp
[McpServerTool, Description("Divides a by b.")]
public static double Divide(double a, double b)
{
    if (b == 0)
        throw new ArgumentException("Cannot divide by zero.");
    return a / b;
}
```
你也可以显式地构建结果：```csharp
[McpServerTool, Description("…")]
public static CallToolResult Foo(...)
{
    return new CallToolResult
    {
        IsError = true,
        Content = [new TextContentBlock { Text = "Detailed error explanation for the LLM." }]
    };
}
```
协议级错误（在LLM看到结果之前，调用被拒绝）

使用`McpException`（或带有显式错误代码的`McpProtocolException`）来处理诸如坏参数之类的事情：```csharp
[McpServerTool, Description("…")]
public static string Process(string input)
{
    if (string.IsNullOrWhiteSpace(input))
        throw new McpProtocolException("Missing required input", McpErrorCode.InvalidParams);
    return $"Processed: {input}";
}
```
**启发式：**如果LLM应该用不同的参数*重试*，抛出一个常规异常，这样它就会得到一个工具错误。如果调用格式不正确，LLM无法修复，则抛出`McpProtocolException`。

通知客户端工具列表的变化

如果你的工具在运行时出现和消失（例如插件加载，用户登录），通知客户端：```csharp
await server.SendNotificationAsync(
    NotificationMethods.ToolListChangedNotification,
    new ToolListChangedNotificationParams(),
    cancellationToken);
```
需要有状态传输（STDIO或有状态HTTP）。

##常见陷阱

- **在课堂上忘记`[McpServerToolType]`。**方法级`[McpServerTool]`本身不会被`WithToolsFromAssembly`发现。
- **模糊的描述。**`[Description("Gets data")]`进行LLM猜测。用一句话描述这个工具做什么，什么时候调用它，以及它返回什么。
- **大载荷。**返回兆字节JSON的工具吃掉模型的上下文。修剪或分页。对于二进制blob，返回一个`EmbeddedResourceBlock`，以便主机可以决定如何呈现它。
- **隐藏错误。**将`"failed"`作为字符串返回给SDK看起来就像成功了。抛出异常或设置`IsError = true`。