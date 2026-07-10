# Streamable HTTP transport （ASP. net）净核心)

可流HTTP是现代远程传输。单个端点通过HTTP POST接受JSON-RPC，并且（可选地）在服务器有多个消息要发送时将响应作为服务器发送的事件流返回。

> **SSE-only已弃用。**旧的“HTTP+SSE”传输（单独的POST端点+ GET SSE端点）从新客户端消失。使用可流式HTTP。如果必须支持已知的旧客户机，则只启用遗留SSE (`EnableLegacySse = true`)，并记录原因。

何时选择HTTP

—多租户或远程托管服务器。
-通过OAuth / API网关进行验证。
-水平扩展部署（使用`Stateless = true`）。
-容器，Azure容器应用程序，Kubernetes等

对于本地单用户场景，[STDIO]（./transport-stdio.md）更简单。

最小的服务器```bash
dotnet new web -n MyHttpServer -f net10.0
cd MyHttpServer
dotnet add package ModelContextProtocol.AspNetCore
```

```csharp
// Program.cs
using ModelContextProtocol.Server;
using System.ComponentModel;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddMcpServer()
    .WithHttpTransport(options =>
    {
        // Stateless = true: each request is independent, no Mcp-Session-Id tracking.
        // Required for horizontal scaling without sticky sessions.
        // Disables server-to-client features (sampling, elicitation, roots, unsolicited notifications).
        options.Stateless = true;
    })
    .WithToolsFromAssembly();

var app = builder.Build();

app.MapMcp();              // mounts the MCP endpoints at "/"
// app.MapMcp("/mcp");     // or under a path prefix

app.Run("http://localhost:3001");

[McpServerToolType]
public static class EchoTool
{
    [McpServerTool, Description("Echoes the message back to the client.")]
    public static string Echo(string message) => $"hello {message}";
}
```
无状态vs有状态——最重要的决定

|模式|`options.Stateless`|行为| |时使用|---|---|---|---|
| **无状态** |`true`|无`Mcp-Session-Id`。每个POST都是独立的。|水平扩展，简单的工具服务器，没有服务器发起的流量。|
| **有状态** |`false`（默认）|服务器分配和跟踪`Mcp-Session-Id`。长寿的会话。|您需要启发、采样、根、日志通知或任何从服务器推送到客户机的内容。在负载平衡器上需要会话关联。|

**规则：**如果用户想要`ElicitAsync`，`SampleAsync`，`RequestRootsAsync`中的任何一个，或者推送log/notification消息，**不要**设置`Stateless = true`。调用将在运行时失败，因为没有传输来交付它们。

##端点形状`MapMcp(pattern = "")`在`pattern`创建一个路由组并映射：
- **POST** -接受JSON- rpcrequests/responses/notifications.返回JSON响应或SSE流取决于`Accept`报头以及是否需要多个消息流。
- **GET** -由有状态会话用于服务器到客户端的SSE通道。
- **DELETE**终止有状态会话。

默认模式是根（`/`）。将MCP放在`/mcp/v1`下：```csharp
app.MapMcp("/mcp/v1");
```
在客户端匹配它（`Endpoint = new Uri("https://host/mcp/v1")`）。

每会话配置（HttpContext访问）

当你需要改变每个HTTP请求（auth, tenant, headers）的服务器行为时，使用`ConfigureSessionOptions`回调：```csharp
builder.Services
    .AddMcpServer()
    .WithHttpTransport(options =>
    {
        options.ConfigureSessionOptions = async (httpContext, mcpOptions, ct) =>
        {
            var tenantId = httpContext.Request.Headers["X-Tenant"].ToString();
            mcpOptions.ServerInstructions = $"Tenant: {tenantId}";
            // mutate any McpServerOptions fields per-session
        };
    });
```
在工具内部，如果注册了`AddHttpContextAccessor()`，还可以注入`IHttpContextAccessor`。请参阅[`AspNetCoreMcpPerSessionTools`示例]（https://github.com/modelcontextprotocol/csharp-sdk/tree/main/samples/AspNetCoreMcpPerSessionTools）。

# #身份验证

MCP端点只是一个ASP。. NET核心端点-应用标准中间件：```csharp
builder.Services
    .AddAuthentication("Bearer")
    .AddJwtBearer(/* configure */);
builder.Services.AddAuthorization();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapMcp().RequireAuthorization();   // protect the endpoint
```
对于以*MCP服务器*为资源服务器的OAuth流，请遵循[MCP授权规范]（https://modelcontextprotocol.io/specification/2025-06-18/basic/authorization）。[`ProtectedMcpServer`样例]（https://github.com/modelcontextprotocol/csharp-sdk/tree/main/samples/ProtectedMcpServer）显示了一个带有发现端点的工作设置。

对于机器对机器，API密钥中间件很好：```csharp
app.Use(async (ctx, next) =>
{
    if (ctx.Request.Headers["X-Api-Key"] != Configuration["ApiKey"])
    {
        ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
        return;
    }
    await next();
});
```
CORS（当客户端在浏览器中时）```csharp
builder.Services.AddCors(o => o.AddDefaultPolicy(p =>
    p.WithOrigins("https://my-host.example.com")
     .AllowAnyHeader()
     .AllowAnyMethod()
     .AllowCredentials()));
// ...
app.UseCors();
app.MapMcp();
```
健康检查和可观察性

添加标准ASP。. NET Core探针；MCP端点不应该是活动性检查。```csharp
builder.Services.AddHealthChecks();
// ...
app.MapHealthChecks("/healthz");
```
SDK发出OpenTelemetry跟踪（每个工具调用`Activity`）和度量。如果用户有OTel管道，则将它们连接起来：```csharp
builder.Services
    .AddOpenTelemetry()
    .WithTracing(t => t.AddSource("ModelContextProtocol").AddOtlpExporter())
    .WithMetrics(m => m.AddMeter("ModelContextProtocol").AddOtlpExporter());
```
##部署说明

- **正常集装箱化。**没有特殊的mcp特定Dockerfile -它只是一个ASP。. NET Core应用。
- **在反向代理** （nginx， Azure前门，AWS ALB）后面，确保SSE缓冲为MCP路径**禁用**。nginx:`proxy_buffering off;`。如果没有这个，流响应将被批处理成一个缓慢的blob。
- * *超时。**客户端可能长时间保持SSE连接打开。为有状态部署设置较高的代理空闲超时（例如5分钟以上）；对于无状态来说不那么关键。
- **Azure容器应用程序/应用服务**开箱即用；两者都支持长期HTTP响应。

启用遗留SSE（仅兼容）```csharp
builder.Services
    .AddMcpServer()
    .WithHttpTransport(options =>
    {
        options.EnableLegacySse = true;
#pragma warning disable MCP9004
        options.Stateless = false; // SSE requires stateful mode
#pragma warning restore MCP9004
    })
    .WithToolsFromAssembly();
```
只有当用户有一个文档化的、没有迁移的客户端时才这样做。新的部署不应该启用它。