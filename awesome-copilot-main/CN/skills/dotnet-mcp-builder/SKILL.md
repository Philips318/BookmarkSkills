---
name: dotnet-mcp-builder
description: 'Build Model Context Protocol (MCP) servers in C#/.NET against the current ModelContextProtocol 1.x NuGet packages. Especially helps with cases the model often gets wrong without guidance — stale preview versions (it tends to pick 0.3 or 0.4 preview), MCP Apps (interactive UI rendered in the host), elicitation URL mode, per-session HTTP wiring, OAuth and reverse-proxy deploy specifics, and debugging concrete MapMcp / STDIO / Streamable-HTTP errors. Also covers the routine work — STDIO and Streamable HTTP transports (SSE is deprecated), tools, prompts, resources, sampling, roots, completions, logging — and a basic .NET MCP client. Trigger when the user says or implies any .NET MCP server work: ModelContextProtocol, McpServerTool, MapMcp, WithStdioServerTransport, "MCP server in C#", "MCP tool in dotnet", "expose this as MCP", or names a primitive (prompt/resource/elicitation/MCP App) in a .NET context. Skip for MCP work in other languages.'
---
#构建MCP服务器。网

这项技能可以帮助你用c# /编写生产质量的MCP服务器和基本客户端。. NET针对**官方** [`ModelContextProtocol`](https://www.nuget.org/profiles/ModelContextProtocol) NuGet包，由微软和MCP项目维护。它的目标是**稳定的1。X **线和当前规范（2025-11-25）。

##当此技能获得维持时

这个。. NET MCP SDK在达到`1.0`之前有多年的预览包（`0.x-preview`）。在没有帮助的情况下，模型倾向于：
-固定一个过时的预览版本，不会编译当前的样本。
-错过最近的规范功能（引出URL模式，MCP应用程序，结构化内容块）。
-获得错误的HTTP传输细节（stateful/stateless，代理缓冲，OAuth布线）。
-忘记STDIOstdout/stderr陷阱。如果任务是其中之一，则加载匹配的引用并遵循它。如果真的是小事(例如：“重命名这个工具方法”)，您不需要阅读所有内容—下面的基本规则是最基本的。

30秒内完成心智模型

一个。. NET MCP服务器是一个普通的`Microsoft.Extensions.Hosting`（或`WebApplication`）应用程序，通过DI连接MCP服务器：```csharp
builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()      // OR .WithHttpTransport(...)
    .WithToolsFromAssembly()         // discover [McpServerToolType] classes
    .WithPrompts<MyPrompts>()        // optional
    .WithResources<MyResources>();   // optional
```
原语是用属性标记的类（`[McpServerToolType]`+`[McpServerTool]`,`[McpServerPromptType]`+`[McpServerPrompt]`,`[McpServerResourceType]`+`[McpServerResource]`）上的普通c#方法。从JSON-RPC绑定参数；SDK根据签名和`[Description]`属性构建JSON模式。

服务器到客户机特性（采样、激发、根、log/progress通知）是注入的`IMcpServer`上的方法。

决策树→加载哪些引用

如果您正在创建一个新项目或不确定当前包的版本，请始终加载`references/packages.md`。

|任务|加载||---|---|
|新增STDIO服务器|`references/transport-stdio.md`|
|新的HTTP（流）服务器|`references/transport-http.md`|
|Add/modify工具|`references/tool-primitive.md`|
|Add/modify提示|`references/prompt-primitive.md`|
|Add/modify资源|`references/resource-primitive.md`|
|在工具中向用户提问|`references/elicitation.md`|
|从工具|`references/sampling.md`|调用客户端的LLM
|读取用户的项目根目录|`references/roots.md`|
|返回交互式UI |`references/mcp-apps.md`|
b|参数补全，log/progress通知，过滤器，服务器指令|`references/server-features.md`|
|写a。. NET程序，**消耗**和MCP服务器|`references/client.md`|
| MCP检查器，内存测试，模拟，CI |`references/testing.md`|

对于多原语任务，一次加载几个。对于现有文件中的琐碎编辑，通常不需要任何编辑。

基本规则（始终适用；这些规则可以防止最频繁的中断）1. **固定当前的稳定包，而不是预览。**使用`ModelContextProtocol`/`ModelContextProtocol.AspNetCore`/`ModelContextProtocol.Core`最迟**1.x**。如果您发现自己正在编写`0.3-preview`或`0.4-preview`，请停止并检查NuGet预览api是否存在破坏性差异。
2. **STDIO服务器不能写入标准输出。**标准输出是JSON-RPC通道。首先配置`LogToStandardErrorThreshold = LogLevel.Trace`，不要从工具中配置`Console.WriteLine`。
3. **HTTP默认为有状态。**对于没有服务器发起的流量的水平扩展部署，设置`options.Stateless = true`。服务器到客户端特性（采样、激发、根、未请求的通知）需要有状态HTTP **或** STDIO -`Stateless = true`将在运行时破坏它们。
4. **不支持仅支持sse。**使用Streamable HTTP。只对必须支持的旧客户机启用遗留SSE (`EnableLegacySse = true`)，并调用它。
5. **总是`[Description]`工具和参数。**这是LLM在挑选和塑造电话时看到的。模糊的描述是工具不被使用的首要原因。
6. **每次添加原语时显示注册行。**一个新的`[McpServerPromptType]`类没有`.WithPrompts<...>()`（或`.WithPromptsFromAssembly()`）是不可见的。
7. **不要发明api。**如果你不确定一个方法是否存在，请说出来并检查[API参考](https://csharp.sdk.modelcontextprotocol.io/api/ModelContextProtocol.html) -错误的方法名称会导致静默失败。工作方式

- **做最小的，附加的改变。**向现有的工具类添加一个方法，而不是重新构建项目。
- **对于不重要的设置，执行`dotnet build`。**在用户看到它们之前捕获缺失的用法、属性错别字和TFM不匹配。
- **确认传输+。. NET版本+脚手架之前的原语**如果上下文还没有明确说明它们。默认为**。NET 10**用于新项目。

##当用户卡住时在猜测之前先看看这张清单：
1. **STDIO:**某些东西正在写入标准输出（记录器接收器，`Console.WriteLine`，库横幅）。
2. **HTTP 404:**路径不匹配-`app.MapMcp()`是根目录，`app.MapMcp("/mcp")`把它放在`/mcp`下。
3. **工具不出现：**类上缺少`[McpServerToolType]`，或者没有注册`.WithToolsFromAssembly()`/`.WithTools<T>()`。
4. **参数名称必须匹配JSON-RPC`arguments`键；复杂类型通过`System.Text.Json`绑定。
5. **Sampling/elicitation/roots失败：**传输是无状态HTTP，或者客户端没有发布该功能。

还困吗?将用户指向[`EverythingServer`]（https://github.com/modelcontextprotocol/csharp-sdk/tree/main/samples/EverythingServer）样例—它将练习每个特性。