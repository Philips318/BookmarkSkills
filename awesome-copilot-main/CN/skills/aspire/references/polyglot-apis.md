# Polyglot api -完整参考

Aspire支持10+languages/runtimes.AppHost总是。但是编排的工作负载可以是任何语言。每种语言都有一个托管方法，该方法返回连接到依赖关系图中的资源。

---

托管模型差异

|型号|资源类型|运行方式|示例||---|---|---|---|
| **项目** |`ProjectResource`|。. NET项目参考，由SDK |`AddProject<T>()`|构建
| **容器** |`ContainerResource`|Docker/OCI映像|`AddContainer()`，`AddRedis()`,`AddPostgres()`|
| **可执行** |`ExecutableResource`|本机操作系统进程|`AddExecutable()`，所有`Add*App()`多语言方法|

所有多语言`Add*App()`方法都在底层创建`ExecutableResource`实例。它们不需要在AppHost端使用目标语言的SDK——只需要在开发机器上安装工作负载的运行时。

---

官方（微软维护）

# # #。. Net / c \#```csharp
builder.AddProject<Projects.MyApi>("api")
```
* *链接方法:* *
-`.WithHttpEndpoint(port?, targetPort?, name?)`-暴露HTTP端点
-`.WithHttpsEndpoint(port?, targetPort?, name?)`-暴露HTTPS端点
-`.WithEndpoint(port?, targetPort?, scheme?, name?)`-通用端点
-`.WithReference(resource)`-连线依赖（连接字符串或服务发现）
—`.WithReplicas(count)`—运行多个实例
-`.WithEnvironment(key, value)`-设置环境变量
-`.WithEnvironment(callback)`-通过回调设置env变量（延迟解析）
-`.WaitFor(resource)`-在依赖关系正常之前不要启动
-`.WithExternalHttpEndpoints()`-标记端点为外部可访问
-`.WithOtlpExporter()`-配置OpenTelemetry导出
-`.PublishAsDockerFile()`-覆盖Dockerfile的发布行为

# # # Python```csharp
// Standard Python script
builder.AddPythonApp("service", "../python-service", "main.py")

// Uvicorn ASGI server (FastAPI, Starlette, etc.)
builder.AddUvicornApp("fastapi", "../fastapi-app", "app:app")
```
* *`AddPythonApp(name, projectDirectory, scriptPath, args?)`* *

链接方法:
-`.WithHttpEndpoint(port?, targetPort?, name?)`-暴露HTTP
-`.WithVirtualEnvironment(path?)`-使用venv（默认：`.venv`）
-`.WithPipPackages(packages)`-启动时安装PIP包
-`.WithReference(resource)`-线依赖
-`.WithEnvironment(key, value)`- set env var
-`.WaitFor(resource)`-等待依赖项运行状况

* *`AddUvicornApp(name, projectDirectory, appModule, args?)`* *

链接方法:
-`.WithHttpEndpoint(port?, targetPort?, name?)`-暴露HTTP
-`.WithVirtualEnvironment(path?)`-使用venv
-`.WithReference(resource)`-线依赖
-`.WithEnvironment(key, value)`- set env var
-`.WaitFor(resource)`-等待依赖项运行状况

**Python服务发现：**环境变量被自动注入。使用`os.environ`读取：```python
import os
redis_conn = os.environ["ConnectionStrings__cache"]
api_url = os.environ["services__api__http__0"]
```
### JavaScript / TypeScript```csharp
// Generic JavaScript app (npm start)
builder.AddJavaScriptApp("frontend", "../web-app")

// Vite dev server
builder.AddViteApp("spa", "../vite-app")

// Node.js script
builder.AddNodeApp("worker", "server.js", "../node-worker")
```
* *`AddJavaScriptApp(name, workingDirectory)`* *

链接方法:
-`.WithHttpEndpoint(port?, targetPort?, name?)`-暴露HTTP
-`.WithNpmPackageInstallation()`-启动前运行`npm install`-`.WithReference(resource)`-线依赖
-`.WithEnvironment(key, value)`- set env var
-`.WaitFor(resource)`-等待依赖项运行状况

* *`AddViteApp(name, workingDirectory)`* *

链接方法（同`AddJavaScriptApp`plus）：
-`.WithNpmPackageInstallation()`-启动前运行`npm install`-`.WithHttpEndpoint(port?, targetPort?, name?)`- Vite默认为5173

* *`AddNodeApp(name, scriptPath, workingDirectory)`* *

链接方法:
-`.WithHttpEndpoint(port?, targetPort?, name?)`-暴露HTTP
-`.WithNpmPackageInstallation()`-启动前运行`npm install`-`.WithReference(resource)`-线依赖
-`.WithEnvironment(key, value)`- set env var

**JS/TS服务发现：**注入环境变量。使用`process.env`:```javascript
const redisUrl = process.env.ConnectionStrings__cache;
const apiUrl = process.env.services__api__http__0;
```
---

##社区（CommunityToolkit/Aspire）

所有社区集成都遵循相同的模式：在AppHost中安装NuGet包，然后使用`Add*App()`方法。

# # #去

* *包:* *`CommunityToolkit.Aspire.Hosting.Golang````csharp
builder.AddGolangApp("go-api", "../go-service")
    .WithHttpEndpoint(targetPort: 8080)
    .WithReference(redis)
    .WithEnvironment("LOG_LEVEL", "debug")
    .WaitFor(redis);
```
链接方法:
——`.WithHttpEndpoint(port?, targetPort?, name?)`——`.WithReference(resource)`——`.WithEnvironment(key, value)`——`.WaitFor(resource)`**Go服务发现：**标准环境变量通过`os.Getenv()`：```go
redisAddr := os.Getenv("ConnectionStrings__cache")
```
Java （Spring Boot）

* *包:* *`CommunityToolkit.Aspire.Hosting.Java````csharp
builder.AddSpringApp("spring-api", "../spring-service")
    .WithHttpEndpoint(targetPort: 8080)
    .WithReference(postgres)
    .WaitFor(postgres);
```
链接方法:
——`.WithHttpEndpoint(port?, targetPort?, name?)`——`.WithReference(resource)`——`.WithEnvironment(key, value)`——`.WaitFor(resource)`-`.WithMavenBuild()`-在开始之前运行Maven构建
-`.WithGradleBuild()`-在开始之前运行Gradle构建

**通过`System.getenv()`发现环境变量；```java
String dbConn = System.getenv("ConnectionStrings__db");
```
# # #生锈

* *包:* *`CommunityToolkit.Aspire.Hosting.Rust````csharp
builder.AddRustApp("rust-worker", "../rust-service")
    .WithHttpEndpoint(targetPort: 3000)
    .WithReference(redis)
    .WaitFor(redis);
```
链接方法:
——`.WithHttpEndpoint(port?, targetPort?, name?)`——`.WithReference(resource)`——`.WithEnvironment(key, value)`——`.WaitFor(resource)`-`.WithCargoBuild()`-启动前运行`cargo build`# # #包

* *包:* *`CommunityToolkit.Aspire.Hosting.Bun````csharp
builder.AddBunApp("bun-api", "../bun-service")
    .WithHttpEndpoint(targetPort: 3000)
    .WithReference(redis);
```
链接方法:
——`.WithHttpEndpoint(port?, targetPort?, name?)`——`.WithReference(resource)`——`.WithEnvironment(key, value)`——`.WaitFor(resource)`-`.WithBunPackageInstallation()`-启动前运行`bun install`# # # Deno

* *包:* *`CommunityToolkit.Aspire.Hosting.Deno````csharp
builder.AddDenoApp("deno-api", "../deno-service")
    .WithHttpEndpoint(targetPort: 8000)
    .WithReference(redis);
```
链接方法:
——`.WithHttpEndpoint(port?, targetPort?, name?)`——`.WithReference(resource)`——`.WithEnvironment(key, value)`——`.WaitFor(resource)`# # # PowerShell```csharp
builder.AddPowerShell("ps-script", "../scripts/process.ps1")
    .WithReference(storageAccount);
```
# # # Dapr

**包装：**`Aspire.Hosting.Dapr`（官方）```csharp
var dapr = builder.AddDapr();
var api = builder.AddProject<Projects.Api>("api")
    .WithDaprSidecar("api-sidecar");
```
---

完整的混合语言示例```csharp
var builder = DistributedApplication.CreateBuilder(args);

// Infrastructure
var redis = builder.AddRedis("cache");
var postgres = builder.AddPostgres("pg").AddDatabase("catalog");
var mongo = builder.AddMongoDB("mongo").AddDatabase("analytics");
var rabbit = builder.AddRabbitMQ("messaging");

// .NET API (primary)
var api = builder.AddProject<Projects.CatalogApi>("api")
    .WithReference(postgres)
    .WithReference(redis)
    .WithReference(rabbit)
    .WaitFor(postgres)
    .WaitFor(redis);

// Python ML service (FastAPI)
var ml = builder.AddUvicornApp("ml", "../ml-service", "app:app")
    .WithHttpEndpoint(targetPort: 8000)
    .WithVirtualEnvironment()
    .WithReference(redis)
    .WithReference(mongo)
    .WaitFor(redis);

// TypeScript frontend (Vite + React)
var web = builder.AddViteApp("web", "../frontend")
    .WithNpmPackageInstallation()
    .WithHttpEndpoint(targetPort: 5173)
    .WithReference(api);

// Go event processor
var processor = builder.AddGolangApp("processor", "../go-processor")
    .WithReference(rabbit)
    .WithReference(mongo)
    .WaitFor(rabbit);

// Java analytics service (Spring Boot)
var analytics = builder.AddSpringApp("analytics", "../spring-analytics")
    .WithHttpEndpoint(targetPort: 8080)
    .WithReference(mongo)
    .WithReference(rabbit)
    .WaitFor(mongo);

// Rust high-perf worker
var worker = builder.AddRustApp("worker", "../rust-worker")
    .WithReference(redis)
    .WithReference(rabbit)
    .WaitFor(redis);

builder.Build().Run();
```
这个AppHost启动跨5种语言的6个服务以及4个基础设施资源，所有这些都与自动服务发现连接在一起。