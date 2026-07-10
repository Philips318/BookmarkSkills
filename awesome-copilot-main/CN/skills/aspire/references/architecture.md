#建筑-深潜

本参考资料涵盖了Aspire的内部架构：DCP引擎、资源模型、服务发现、网络、遥测和事件系统。

---

开发者控制平面（DCP）

DCP是Aspire在`aspire run`模式下使用的运行时引擎。关键事实:

-用**Go**书写(非。净)
-公开一个与kubernetes兼容的API服务器**（仅在本地，而不是真正的K8s集群）
—资源生命周期管理：创建、启动、健康检查、停止、重启
-通过本地容器运行时运行容器（Docker, Podman, Rancher）
—像本地操作系统进程一样运行可执行文件
-通过自动端口分配的代理层处理网络
-为Aspire Dashboard的实时数据提供基础

DCP vs Kubernetes

| Aspect | DCP（本地开发）| Kubernetes（生产）||---|---|---|
| API | Kubernetes-compatible | Kubernetes Full API |
|作用域|单机|集群|
|本地代理，自动端口|业务网格，入口|
|存储|本地卷| pvc、云存储|
|用途|开发人员内环|生产部署|

与Kubernetes兼容的API意味着Aspire理解相同的资源抽象，但DCP不是Kubernetes发行版——它是一个轻量级的本地运行时。

---

##资源模型

Aspire中的一切都是**资源。资源模型是分层的：

类型层次结构```
IResource (interface)
└── Resource (abstract base)
    ├── ProjectResource          — .NET project reference
    ├── ContainerResource        — Docker/OCI container
    ├── ExecutableResource       — Native process (polyglot apps)
    ├── ParameterResource        — Config value or secret
    └── Infrastructure resources
        ├── RedisResource
        ├── PostgresServerResource
        ├── MongoDBServerResource
        ├── SqlServerResource
        ├── RabbitMQServerResource
        ├── KafkaServerResource
        └── ... (one per integration)
```
资源属性

每个资源都有：
—**名称**—AppHost内部的唯一标识
- **State** -生命周期状态（Starting, Running, FailedToStart, Stopped， Stopped等）
- **注释** -附加到资源的元数据
- **端点**资源暴露的网络端点
- **环境变量** -注入到process/container# # #注释

注解是附加在资源上的元数据包。常见的内置注释：

|注释|用途||---|---|
|`EndpointAnnotation`|定义HTTP/HTTPS/TCP端点|
|`EnvironmentCallbackAnnotation`|延迟env var分辨率|
|`HealthCheckAnnotation`|健康检查配置|
|`ContainerImageAnnotation`| Docker镜像细节|
|`VolumeAnnotation`|卷挂载配置|
|`CommandLineArgsCallbackAnnotation`| CLI动态参数|
|`ManifestPublishingCallbackAnnotation`|自定义发布行为|

资源生命周期状态```
NotStarted → Starting → Running → Stopping → Stopped
                 ↓                     ↓
          FailedToStart           RuntimeUnhealthy
                                       ↓
                                  Restarting → Running
```
DAG（有向无环图）

资源形成依赖关系图。Aspire按拓扑顺序启动资源：```
PostgreSQL ──→ API ──→ Frontend
Redis ────────↗
RabbitMQ ──→ Worker
```
1. PostgreSQL、Redis和RabbitMQ优先启动（无依赖）
2. PostgreSQL和Redis运行正常后启动API
3. 在API运行正常后启动前端
4. RabbitMQ运行正常后启动Worker`.WaitFor()`向依赖项边缘添加一个运行状况检查门。没有它，依赖会启动，但下游不会等待运行状况。

---

##服务发现

Aspire将环境变量注入到每个资源中，这样服务就可以找到彼此。不需要服务注册中心或DNS -这是纯粹的环境变量注入。

连接字符串

对于数据库、缓存和消息代理：```
ConnectionStrings__<resource-name>=<connection-string>
```
例子:```
ConnectionStrings__cache=localhost:6379
ConnectionStrings__catalog=Host=localhost;Port=5432;Database=catalog;Username=postgres;Password=...
ConnectionStrings__messaging=amqp://guest:guest@localhost:5672
```
服务端点

对于HTTP/HTTPS服务：```
services__<resource-name>__<scheme>__0=<url>
```
例子:```
services__api__http__0=http://localhost:5234
services__api__https__0=https://localhost:7234
services__ml__http__0=http://localhost:8000
```
###如何。WithReference()工作```csharp
var redis = builder.AddRedis("cache");
var api = builder.AddProject<Projects.Api>("api")
    .WithReference(redis);
```
这样做:
1. 将`ConnectionStrings__cache=localhost:<auto-port>`添加到API环境中
2. 在DAG中创建一个依赖边缘（API依赖于Redis）
3. 在API服务中，`builder.Configuration.GetConnectionString("cache")`返回连接字符串

跨语言服务发现

所有语言都使用相同的env var模式：

|语言|如何阅读||---|---|
| c# |`builder.Configuration.GetConnectionString("cache")`|
| Python |`os.environ["ConnectionStrings__cache"]`|
|`process.env.ConnectionStrings__cache`|
| |`os.Getenv("ConnectionStrings__cache")`|
| Java |`System.getenv("ConnectionStrings__cache")`|
|锈|`std::env::var("ConnectionStrings__cache")`|

---

# #网络

代理架构

在`aspire run`模式下，DCP为每个公开的端点运行一个反向代理：```
Browser → Proxy (auto-assigned port) → Actual Service (target port)
```
- **端口**（外部端口）-由DCP自动分配，除非被覆盖
- **targetPort** -你的服务实际监听的端口
—所有业务间流量都通过代理进行可观察性处理```csharp
// Let DCP auto-assign the external port, service listens on 8000
builder.AddPythonApp("ml", "../ml", "main.py")
    .WithHttpEndpoint(targetPort: 8000);

// Fix the external port to 3000
builder.AddViteApp("web", "../frontend")
    .WithHttpEndpoint(port: 3000, targetPort: 5173);
```
端点类型```csharp
// HTTP endpoint
.WithHttpEndpoint(port?, targetPort?, name?)

// HTTPS endpoint
.WithHttpsEndpoint(port?, targetPort?, name?)

// Generic endpoint (TCP, custom schemes)
.WithEndpoint(port?, targetPort?, scheme?, name?, isExternal?)

// Mark endpoints as externally accessible (for deployment)
.WithExternalHttpEndpoints()
```
---

遥测（OpenTelemetry）

Aspire自动配置OpenTelemetry。网络服务。非。. NET服务时，您手动配置OpenTelemetry，指向DCP收集器。

什么是自动配置的？网络服务)

- **分布式跟踪** - HTTPclient/server跨度，数据库跨度，消息传递跨度
- **指标** -运行指标，HTTP指标，自定义指标
-结构化日志** -与跟踪上下文相关的日志
- **出口商** - OTLP出口商指向Aspire仪表板

###配置非。网络服务

DCP公开OTLP端点。把这些嫉妒者放在你的办公室里。网络服务:```
OTEL_EXPORTER_OTLP_ENDPOINT=http://localhost:4317
OTEL_SERVICE_NAME=<your-service-name>
```
Aspire通过`.WithReference()`为仪表板收集器自动注入`OTEL_EXPORTER_OTLP_ENDPOINT`。

ServiceDefaults模式`ServiceDefaults`项目是一个共享配置库，它标准化了：
- OpenTelemetry设置（跟踪，度量，日志记录）
-运行状况检查端点（`/health`、`/alive`）
-弹性策略（重试，通过Polly断路）```csharp
// In each .NET service's Program.cs
builder.AddServiceDefaults();   // adds OTel, health checks, resilience
// ... other service config ...
app.MapDefaultEndpoints();      // maps /health and /alive
```
---

##健康检查

内置健康检查

每次集成都会在客户端自动添加运行状况检查：
—Redis:`PING`命令
—PostgreSQL:`SELECT 1`—MongoDB:`ping`命令
—RabbitMQ：连接检查
——等等。

WaitFor vs WithReference```csharp
// WithReference: wires connection string + creates dependency edge
// (downstream may start before dependency is healthy)
.WithReference(db)

// WaitFor: gates on health check — downstream won't start until healthy
.WaitFor(db)

// Typical pattern: both
.WithReference(db).WaitFor(db)
```
自定义健康检查```csharp
var api = builder.AddProject<Projects.Api>("api")
    .WithHealthCheck("ready", "/health/ready")
    .WithHealthCheck("live", "/health/live");
```
---

事件系统

AppHost支持生命周期事件来响应资源状态的变化：```csharp
builder.Eventing.Subscribe<ResourceReadyEvent>("api", (evt, ct) =>
{
    // Fires when "api" resource becomes healthy
    Console.WriteLine($"API is ready at {evt.Resource.Name}");
    return Task.CompletedTask;
});

builder.Eventing.Subscribe<BeforeResourceStartedEvent>("db", async (evt, ct) =>
{
    // Run database migrations before the DB resource is marked as started
    await RunMigrations();
});
```
可用事件

|事件|当||---|---|
|`BeforeResourceStartedEvent`|资源启动前|
|`ResourceReadyEvent`|资源状态正常，可用|
|`ResourceStateChangedEvent`|任意状态转换|
|`BeforeStartEvent`|在整个应用程序启动之前|
|`AfterEndpointsAllocatedEvent`|所有端口分配完成后为|

---

# #配置

# # #参数```csharp
// Plain parameter
var apiKey = builder.AddParameter("api-key");

// Secret parameter (prompted at run, not logged)
var dbPassword = builder.AddParameter("db-password", secret: true);

// Use in resources
var api = builder.AddProject<Projects.Api>("api")
    .WithEnvironment("API_KEY", apiKey);

var db = builder.AddPostgres("db", password: dbPassword);
```
配置源

参数解析自（按优先级顺序）：
1. 命令行参数
2. 环境变量
3. 用户秘密（`dotnet user-secrets`）
4.`appsettings.json`/`appsettings.{Environment}.json`5. 交互式提示（用于`aspire run`期间的秘密）