#资源

资源是由URI标识的服务器公开的“事物”。主持人将它们列出来，以便用户可以选择将哪些附加到对话中；工具和提示符也可以通过`EmbeddedResourceBlock`引用它们。想想文件、数据库行、API对象、设置——任何可寻址的东西。

两种口味:
—**静态资源**—固定URI （`config://app/settings`）。对单身人士很有用。
- **资源模板** -带有占位符的URI （`docs://articles/{id}`）。主机（或LLM）替代参数；你的方法接收它们。

##静态资源```csharp
using System.ComponentModel;
using System.Text.Json;
using ModelContextProtocol.Server;

[McpServerResourceType]
public class AppResources
{
    [McpServerResource(
        UriTemplate = "config://app/settings",
        Name = "App Settings",
        MimeType = "application/json")]
    [Description("Returns application configuration settings.")]
    public static string GetSettings() =>
        JsonSerializer.Serialize(new { theme = "dark", language = "en" });
}
```
注册:```csharp
.WithResources<AppResources>()
// or
.WithResourcesFromAssembly()
```
模板化资源`UriTemplate`中的占位符按名称映射到方法参数。任何不是占位符的东西都遵循与工具（`IMcpServer`、`CancellationToken`、服务）相同的DI规则。```csharp
[McpServerResourceType]
public class DocumentResources
{
    [McpServerResource(
        UriTemplate = "docs://articles/{id}",
        Name = "Article",
        MimeType = "text/markdown")]
    [Description("Returns an article by its ID.")]
    public static ResourceContents GetArticle(string id)
    {
        string? content = LoadArticle(id);
        if (content is null)
            throw new McpException($"Article not found: {id}");

        return new TextResourceContents
        {
            Uri = $"docs://articles/{id}",
            MimeType = "text/markdown",
            Text = content
        };
    }
}
```
##返回类型

|返回|结果||---|---|
用来自模板的URI和声明的`MimeType`包装在`TextResourceContents`中。|
|`byte[]`|包裹在`BlobResourceContents`。|
|`TextResourceContents`|按设置返回`Uri`、`MimeType`、`Text`。|
|`BlobResourceContents`|按原样返回`BlobResourceContents.FromBytes(...)`。|
|`IEnumerable<ResourceContents>`|多部分资源。|

二进制资源```csharp
[McpServerResource(
    UriTemplate = "images://photos/{id}",
    Name = "Photo",
    MimeType = "image/png")]
public static BlobResourceContents GetPhoto(int id)
{
    byte[] data = LoadPhoto(id);
    return BlobResourceContents.FromBytes(data, $"images://photos/{id}", "image/png");
}
```
###指向文件系统

常见的模式是从磁盘公开文件。小心路径遍历——永远不要完全信任URI。```csharp
[McpServerResource(
    UriTemplate = "file://workspace/{*relativePath}",
    Name = "Workspace file")]
public static TextResourceContents ReadFile(string relativePath, IOptions<WorkspaceOptions> opts)
{
    var root = opts.Value.RootPath;
    var fullPath = Path.GetFullPath(Path.Combine(root, relativePath));
    if (!fullPath.StartsWith(root, StringComparison.Ordinal))
        throw new McpException("Path traversal blocked.");

    return new TextResourceContents
    {
        Uri = $"file://workspace/{relativePath.Replace("\\", "/")}",
        MimeType = "text/plain",
        Text = File.ReadAllText(fullPath)
    };
}
```
列出动态资源

基于属性的发现涵盖了常见情况（每个模板一个方法）。当你需要列举不适合模板的资源时——比如，“列出工作区中的每个文件”——在`McpServerOptions.Capabilities.Resources`中实现一个低级处理程序：```csharp
builder.Services.Configure<McpServerOptions>(options =>
{
    options.Capabilities ??= new();
    options.Capabilities.Resources ??= new();

    options.Capabilities.Resources.ListResourcesHandler = (ctx, ct) =>
    {
        var resources = Directory
            .EnumerateFiles(WorkspaceRoot, "*.*", SearchOption.AllDirectories)
            .Select(path => new Resource
            {
                Uri = "file://workspace/" + Path.GetRelativePath(WorkspaceRoot, path).Replace('\\', '/'),
                Name = Path.GetFileName(path),
                MimeType = "text/plain"
            })
            .ToList();

        return ValueTask.FromResult(new ListResourcesResult { Resources = resources });
    };
});
```
您可以混合使用基于属性的和基于处理程序的——SDK将两者合并。

资源订阅（服务器推送更新）

如果客户端订阅的资源发生了变化，推送通知：```csharp
await server.SendNotificationAsync(
    NotificationMethods.ResourceUpdatedNotification,
    new ResourceUpdatedNotificationParams { Uri = "docs://articles/42" },
    cancellationToken);
```
批发清单更改：```csharp
await server.SendNotificationAsync(
    NotificationMethods.ResourceListChangedNotification,
    new ResourceListChangedNotificationParams(),
    cancellationToken);
```
两者都需要有状态传输。

##从客户端读取资源```csharp
ReadResourceResult result = await client.ReadResourceAsync("config://app/settings");
foreach (var content in result.Contents)
{
    if (content is TextResourceContents text)
        Console.WriteLine($"[{text.MimeType}] {text.Text}");
    else if (content is BlobResourceContents blob)
        File.WriteAllBytes("out.bin", blob.DecodedData.ToArray());
}
```
资源vs工具——何时选择哪一个

- **资源：**用户（或LLM）想要*附加上下文*到对话。只读，可寻址，可列出。主机控制when/whether来加载它。非常适合文档、配置、模式。
- **工具：**法学硕士想要*做一些事情*（可能包括读取数据）。副作用、操作、不适合URI的参数。

如果您有LLM可能想要“搜索”的内容，那么同时公开：`search_articles`工具和`docs://articles/{id}`资源模板。该工具返回一个uri列表；主机通过资源获取内容。