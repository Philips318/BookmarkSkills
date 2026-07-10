#根

根是客户机向服务器发布的文件系统（或URI）位置，限定了允许服务器查看的范围。想想IDE中的“打开工作空间文件夹”——用户已经隐式地批准了服务器从这些地方读取数据。服务器在需要时提取列表。

当你使用根的时候

-构建scans/edits用户项目的工具。使用根来了解哪些目录在作用域中。
-以尊重用户开放工作空间的方式解析相对路径。
-限制文件访问广告根（深度防御）。

# #的先决条件

与sampling/elicitation相同：服务器到客户机请求→需要STDIO或有状态HTTP。另外，客户机必须宣传`roots`功能。

##从工具中读取根```csharp
using System.ComponentModel;
using System.Text;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;

[McpServerToolType]
public class WorkspaceTools
{
    [McpServerTool, Description("Lists the user's project roots.")]
    public static async Task<string> ListProjectRoots(
        IMcpServer server,
        CancellationToken cancellationToken)
    {
        if (server.ClientCapabilities?.Roots is null)
            return "Client does not support roots.";

        var result = await server.RequestRootsAsync(
            new ListRootsRequestParams(),
            cancellationToken);

        var sb = new StringBuilder();
        foreach (var root in result.Roots)
            sb.AppendLine($"- {root.Name ?? root.Uri}: {root.Uri}");

        return sb.ToString();
    }
}
```
`Root`有`Uri`（字符串，通常是`file://...`）和可选的`Name`（显示标签）。

对根的变化作出反应

当用户打开或关闭工作空间文件夹时，客户端发送`notifications/roots/list_changed`。订阅:```csharp
builder.Services.Configure<McpServerOptions>(options =>
{
    options.Capabilities ??= new();

    // The client tells us its roots changed; refresh whatever cache we have.
    options.Capabilities.NotificationHandlers ??= [];
    options.Capabilities.NotificationHandlers[NotificationMethods.RootsListChangedNotification] =
        async (notification, ct) =>
        {
            // Trigger your refresh — typically pull RequestRootsAsync again.
        };
});
```
一个有用的模式：缓存+刷新

根不会经常改变，但是在每次工具调用时重新获取是浪费的。每个会话缓存它们并在`roots/list_changed`上刷新：```csharp
public class RootsCache
{
    private IReadOnlyList<Root> _roots = Array.Empty<Root>();

    public IReadOnlyList<Root> Current => _roots;

    public async Task RefreshAsync(IMcpServer server, CancellationToken ct)
    {
        if (server.ClientCapabilities?.Roots is null) return;
        var result = await server.RequestRootsAsync(new ListRootsRequestParams(), ct);
        _roots = result.Roots;
    }
}
```
注册为单例（在有状态HTTP中按会话注册，在STDIO中自然是单例）。

根据根验证路径

深度防御：即使工具参数看起来像根目录下的路径，也要验证。```csharp
public static bool IsUnderAnyRoot(string absolutePath, IReadOnlyList<Root> roots)
{
    foreach (var root in roots)
    {
        if (!Uri.TryCreate(root.Uri, UriKind.Absolute, out var uri)) continue;
        if (!uri.IsFile) continue;
        var rootPath = Path.GetFullPath(uri.LocalPath);
        if (absolutePath.StartsWith(rootPath, StringComparison.OrdinalIgnoreCase))
            return true;
    }
    return false;
}
```
如果一个工具接收到广告根之外的路径，用明确的消息拒绝——不要默默地扩展作用域。