# MCP应用程序（交互式UI）

[MCP Apps]（https://modelcontextprotocol.io/extensions/apps/overview）是官方的扩展，让一个工具返回一个交互式UI**呈现在一个沙盒iframe内的主机（克劳德，克劳德桌面，VS CodeCopilot，鹅，邮递员，MCPJam）。典型用例：图表、仪表板、多步骤表单、3D查看器、实时监视器、PDF/video查看器。

重要提示：截至2026年初，c# SDK不提供MCP应用程序的类型化便利层（在[csharp-sdk#1431]（https://github.com/modelcontextprotocol/csharp-sdk/issues/1431）中跟踪）。您可以手工实现该规范：提供`ui://`资源并在工具上发出正确的`_meta`。这并不难，只是没有键入。此页向您展示了该模式。

##它是如何工作的（简短版本）1. 在返回HTML包的`ui://`URI上注册一个资源。
2. 您注册一个**工具**，其定义包括指向该URI的`_meta.ui.resourceUri`。
3. 当LLM调用该工具时，主机获取UI资源并将其呈现在聊天框中的沙盒框架中。
4. HTML通过`postMessage`JSON-RPC与主机通信（使用包中的`@modelcontextprotocol/ext-apps`，或者手工滚动它）。
5. 该应用程序可以回调到您的MCP服务器（任何工具），更新模型上下文等。

完整的协议规范在[`@modelcontextprotocol/ext-apps`]（https://github.com/modelcontextprotocol/ext-apps）。

步骤1：提供UI资源

将HTML/JS/CSS打包成一个字符串（或从`wwwroot`加载）。以`ui://`URI提供它。```csharp
using System.ComponentModel;
using System.IO;
using System.Reflection;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;

[McpServerResourceType]
public static class ChartUiResource
{
    [McpServerResource(
        UriTemplate = "ui://charts/interactive",
        Name = "Interactive chart",
        MimeType = "text/html+skybridge")]   // see "MIME type" note below
    [Description("UI bundle for the interactive chart MCP App.")]
    public static TextResourceContents GetUi()
    {
        // Load a bundled HTML/JS file from embedded resources or wwwroot.
        var html = LoadEmbeddedString("MyMcpServer.AppUi.chart.html");

        return new TextResourceContents
        {
            Uri = "ui://charts/interactive",
            MimeType = "text/html+skybridge",
            Text = html
        };
    }

    private static string LoadEmbeddedString(string resourceName)
    {
        var asm = Assembly.GetExecutingAssembly();
        using var stream = asm.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException($"Missing embedded resource {resourceName}");
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}
```
**MIME类型注意：**规范使用`text/html+skybridge`的应用程序HTML，所以主机可以区分UI包从常规的`text/html`预览。使用它，即使普通的`text/html`今天可能在宽松的主机上工作。

##第二步：在工具上发出`_meta`c# SDK的`[McpServerTool]`今天没有在属性中公开`_meta`，所以通过较低级别的`Tool`定义来设置它。在启动时这样做一次：```csharp
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;
using System.Text.Json;
using System.Text.Json.Nodes;

builder.Services.Configure<McpServerOptions>(options =>
{
    options.Capabilities ??= new();
    options.Capabilities.Tools ??= new();

    // Define the tool manually so we can attach _meta.
    var visualizeTool = new Tool
    {
        Name = "visualize_data",
        Description = "Visualize the user's data as an interactive chart.",
        InputSchema = JsonDocument.Parse("""
            {
              "type": "object",
              "properties": {
                "datasetId": { "type": "string", "description": "Dataset to visualize." }
              },
              "required": ["datasetId"]
            }
            """).RootElement,
        Meta = new JsonObject
        {
            ["ui"] = new JsonObject
            {
                ["resourceUri"] = "ui://charts/interactive"
                // Optionally:
                // ["csp"] = new JsonObject { ["default-src"] = "'self' https://cdn.example.com" },
                // ["permissions"] = new JsonArray("clipboard-write")
            }
        }
    };

    // Implement the call handler that returns the data the UI will render.
    options.Capabilities.Tools.ToolCollection ??= new();
    options.Capabilities.Tools.ToolCollection.Add(McpServerTool.Create(
        async (CallToolRequestParams req, CancellationToken ct) =>
        {
            var args = req.Arguments ?? new();
            var datasetId = args["datasetId"]!.GetValue<string>();
            var data = await LoadDataset(datasetId, ct);
            return new CallToolResult
            {
                Content = [new TextContentBlock { Text = JsonSerializer.Serialize(data) }],
                StructuredContent = JsonSerializer.SerializeToNode(data)
            };
        },
        visualizeTool));
});
```
如果您不需要完整的结构化内容，该工具可以在文本块中返回JSON - UI在渲染后通过`app.callServerTool(...)`获取它。

向后兼容键

一些较老的主机期望使用`_meta["ui/resourceUri"]`而不是`_meta.ui.resourceUri`。为了安全，设置两者：```csharp
Meta = new JsonObject
{
    ["ui"] = new JsonObject { ["resourceUri"] = "ui://charts/interactive" },
    ["ui/resourceUri"] = "ui://charts/interactive"   // legacy
}
```
步骤3:HTML包

最小可行包：使用`@modelcontextprotocol/ext-apps`的香草JS。最简单的构建是一个独立的HTML文件。```html
<!doctype html>
<html>
  <head>
    <meta charset="utf-8" />
    <title>Chart</title>
    <style>body { font-family: system-ui; margin: 0; }</style>
  </head>
  <body>
    <div id="root">Loading…</div>
    <script type="module">
      import { App } from "https://esm.sh/@modelcontextprotocol/ext-apps@1";

      const app = new App();
      await app.connect();

      // Fetch the data we need from the server.
      const resp = await app.callServerTool({
        name: "visualize_data",
        arguments: { datasetId: "default" }
      });

      const data = JSON.parse(resp.content[0].text);
      document.getElementById("root").textContent =
        `Loaded ${data.points.length} data points.`;

      // Tell the model what just happened (becomes part of its context).
      await app.updateModelContext({
        content: [{ type: "text", text: "User opened the chart UI." }]
      });
    </script>
  </body>
</html>
```
**提示：**对于重要的ui，使用Vite （React/Vue/Svelte/Solid-任何[官方入门模板](https://github.com/modelcontextprotocol/ext-apps/tree/main/examples)）构建，并让构建发出单个内联HTML作为项目资源嵌入。

##项目布局

一个实用的布局的MCP应用程序。NET:```
MyMcpServer/
├── Program.cs
├── Tools/
│   └── VisualizeDataTool.cs       # (or registered via Configure as above)
├── Resources/
│   └── ChartUiResource.cs         # serves the ui:// resource
├── AppUi/
│   ├── chart.html                 # bundled UI (Embedded Resource)
│   └── package.json + src/...     # if you build with Vite, output to chart.html
└── MyMcpServer.csproj
```
在csproj中：```xml
<ItemGroup>
  <EmbeddedResource Include="AppUi\chart.html" />
</ItemGroup>
```
通过`Assembly.GetManifestResourceStream("MyMcpServer.AppUi.chart.html")`读取。

##本地测试

1. 运行MCP服务器（STDIO或HTTP）。
2. 使用支持MCP应用程序的主机- Claude Desktop或VS CodeCopilot Chat是最简单的。
3. 通过LLM触发工具。UI内联呈现。

对于纯ui迭代，[MCP Inspector]（https://github.com/modelcontextprotocol/inspector）显示资源内容，但不完全渲染应用程序；为此，将Claude Desktop指向您的开发服务器。

# #陷阱- ** MIME类型错误。**使用`text/html+skybridge`。普通的`text/html`可能仍然有效，但不是面向未来的。
- **CSP太紧或太松。**如果你的UI从CDN加载，在`Tool`定义的`Meta["ui"]["csp"]`中声明它（这在网络上序列化为`_meta.ui.csp`）。否则iframe沙盒会阻止它。
- **忘记了工具上的`Tool.Meta`。**如果没有包含`ui.resourceUri`条目的`Meta`属性，主机将把您的工具视为常规的文本返回工具。UI永远不会出现。
- **尝试在沙箱外使用浏览器api。**没有cookie，没有来自父节点的localStorage。使用`app.updateModelContext`和工具调用来表示状态。

# #能否经得住时间的考验当c# SDK发布它的类型化MCP应用程序帮助程序（issue [#1431](https://github.com/modelcontextprotocol/csharp-sdk/issues/1431)）时，你很可能能够用属性或流畅构建器替换手动`Configure`块。`ui://`资源的服务不变。将UI HTML作为嵌入的资源，这样迁移是机械的。