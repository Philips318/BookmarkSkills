---
description: 'Guidelines and best practices for building Azure Functions in C# using the isolated worker model'
applyTo: '**/*.cs, **/host.json, **/local.settings.json, **/*.csproj'
---
# Azure Functions c# Development

##一般使用说明-对于所有新的Azure Functions项目，始终使用隔离的worker模型（而不是遗留的进程内模型）。NET 6或更高版本。
—在`Program.cs`中使用`FunctionsApplication.CreateBuilder(args)`或`HostBuilder`进行主机设置和依赖注入。
-用`[Function("FunctionName")]`装饰函数方法，并使用强类型触发器和绑定属性。
保持函数方法的重点——每个函数应该只做一件事，并将业务逻辑委托给注入的服务。
永远不要将业务逻辑直接放在函数方法体中；将其提取到通过DI注册的可测试服务类中。
-使用通过构造函数注入的`ILogger<T>`，而不是作为函数参数传递的`ILogger`，以实现一致的结构化日志记录。
-所有I/O-bound操作总是使用`async/await`；不要用`.Result`或`.Wait()`阻塞。
—如果支持`CancellationToken`参数，则首选`CancellationToken`参数，以实现安全关机。

##项目结构和设置—使用“`Microsoft.Azure.Functions.Worker`”和“`Microsoft.Azure.Functions.Worker.Extensions.*`”NuGet软件包。
使用`builder.Services.Add*`扩展方法在`Program.cs`中注册服务，以实现干净的依赖注入。
-将相关功能按领域关注而不是触发器类型分组到单独的类中。
-门店配置在`local.settings.json`进行本地开发；为部署环境使用Azure应用程序配置或应用程序设置。
永远不要在代码中硬编码连接字符串或秘密；总是从`IConfiguration`或环境变量中读取。
—在已部署环境中的秘密使用App Settings中的Key Vault引用（`@Microsoft.KeyVault(SecretUri=...)`）。
-使用`Managed Identity`（`DefaultAzureCredential`）对Azure服务进行身份验证-尽可能避免使用密钥连接字符串。
—根据触发类型调整`host.json`：在主机级别配置`maxConcurrentCalls`、`batchSize`和重试策略。

# #触发器- **HttpTrigger**：使用`AuthorizationLevel.Function`或更高的生产端点；保留`AuthorizationLevel.Anonymous`仅用于具有明确理由的面向公众的api。使用ASP。. NET核心集成（`UseMiddleware`，`IActionResult`返回）。. NET Core集成模型。
- **TimerTrigger**：使用NCRONTAB表达式（`"0 */5 * * * *"`）的时间表；避免在生产环境中使用`RunOnStartup = true`，因为它会在每次冷启动时立即执行。
—**QueueTrigger / ServiceBusTrigger**：在`host.json`和Azure portal中配置`MaxConcurrentCalls`、死信策略和`MaxDeliveryCount`；直接处理`ServiceBusReceivedMessage`以获得高级消息控制（完整、放弃、死信）。
- **BlobTrigger**：优先选择基于事件网格的blob触发器（`Microsoft.Azure.Functions.Worker.Extensions.EventGrid`），而不是基于轮询的blob触发器，以降低延迟和降低存储事务成本。
- **EventHubTrigger**：设置`cardinality`为`many`进行批量处理；批处理模式使用`EventData[]`或`string[]`参数类型；总是检查点使用`EventHubTriggerAttribute`的内置检查点。
- **CosmosDBTrigger**：使用change feed触发器来处理Cosmos数据库更改的事件驱动处理；设置`LeaseContainerName`，并将租赁容器与数据容器分开管理。输入和输出绑定

-使用输入绑定以声明式方式读取数据，而不是直接在函数体中使用sdk，其中绑定覆盖了用例。
-对于多个输出绑定，定义一个自定义的返回类型，其属性带有适当的输出绑定属性注释（例如，`[QueueOutput]`,`[BlobOutput]`,`[HttpResult]`）。
-使用`[BlobInput]`和`[BlobOutput]`blobread/write；对于较大的blob，建议使用`Stream`而不是`byte[]`，以避免内存压力。
-使用`[CosmosDBInput]`点读取和简单查询；对于复杂的查询，通过DI注入`Managed Identity`。
-使用`[ServiceBusOutput]`的单消息发送；通过DI注入`ServiceBusSender`用于批处理或高级发送场景。
-避免将通过DI获得的SDK客户端与基于绑定的I/O混合使用，每个资源选择一种模式以保持一致性。

依赖注入和配置-使用`Azure.Extensions.AspNetCore.Configuration.Secrets`包中的`services.AddAzureClients()`将所有外部客户端（例如，`BlobServiceClient`,`ServiceBusClient`,`CosmosClient`）注册为单例。
—使用`IOptions<T>`或`IOptionsMonitor<T>`作为强类型配置节。
-避免在函数中使用`static`状态；所有共享状态都应该通过di注册的服务。
通过`IHttpClientFactory`注册`HttpClient`实例，管理连接池，避免套接字耗尽。

##错误处理和重试—在`host.json`中配置内置重试策略，使用`"retry"`与`fixedDelay`或`exponentialBackoff`策略进行触发级重试。
-对于代码级别的瞬态故障处理，使用`Microsoft.Extensions.Http.Resilience`或Polly v8 （`ResiliencePipeline`）具有重试，断路器和超时策略。
-总是捕获特定的异常，并在重新抛出或死信之前用结构化的上下文（例如，关联ID，输入标识符）记录它们。
-对所有重试失败的消息使用死信队列；永远不要在函数处理程序中静默地吞下异常。
对于HTTP触发器，返回适当的`IActionResult`类型（`BadRequestObjectResult`,`NotFoundObjectResult`），而不是为预期的错误条件抛出异常。

可观察性和日志记录-使用`ILogger<T>`与结构化日志属性：`_logger.LogInformation("Processing message {MessageId}", messageId)`。
-在`Program.cs`中通过`builder.Services.AddApplicationInsightsTelemetryWorkerService()`和`builder.Logging.AddApplicationInsights()`配置应用程序洞察。
-使用`TelemetryClient`进行自定义事件、度量和依赖项跟踪，而不是自动收集的内容。
—在`"logging"`下的`host.json`中设置适当的日志级别，避免生产中遥测成本过高。
—使用`System.Diagnostics`中的`Activity`和`ActivitySource`在功能和下游服务之间进行分布式跟踪上下文传播。
—避免在任何日志语句中记录敏感数据（PII、秘密、连接字符串）。

性能和可伸缩性保持函数启动时间最小化：将昂贵的初始化延迟到惰性加载的单例，而不是函数构造函数。
-将消费计划用于事件驱动的、不可预测的工作负载；对于低延迟、高吞吐量或vnet集成场景，请使用Premium或Dedicated方案。
-对于cpu密集型工作，卸载到后台`Task`或使用持久函数而不是阻塞函数宿主线程。
—批处理操作：在单个函数调用中处理`IEnumerable<EventData>`或`ServiceBusReceivedMessage[]`数组，而不是一次处理一条消息。
—根据主机规划和预期吞吐量设置`FUNCTIONS_WORKER_PROCESS_COUNT`和`maxConcurrentCalls`。
—在“应用设置”中启用`WEBSITE_RUN_FROM_PACKAGE=1`，直接从部署包中运行，加快冷启动速度。

# #安全-在处理之前始终验证和清理HTTP触发器输入；使用FluentValidation或Data Annotations。
—使用`AuthorizationLevel.Function`和存储在Key Vault中的功能键进行内部api到api调用。
-在http触发功能前集成Azure API管理（APIM），用于面向公众的API来处理认证、速率限制和路由。
-使用App Service网络特性（IP限制，私有端点）限制敏感功能的入站访问。
-永远不要记录包含PII或机密的请求体。

# #测试-使用标准xUnit/NUnit与模拟依赖关系独立于函数主机的单元测试服务类。
-使用`Azurite`（本地Azure存储模拟器）和`TestServer`或Azure功能核心工具集成测试功能。
-使用`Microsoft.Azure.Functions.Worker.Testing`帮助程序构建模拟`FunctionContext`实例。
-避免测试触发管道本身；将测试重点放在提取到服务中的业务逻辑上。

现有代码审查指南-如果项目使用遗留的进程内模型** (`FunctionsStartup`,`IWebJobsStartup`)，建议迁移到隔离的worker模型，并通过`dotnet-isolated-process-guide`提供迁移路径。
-如果在代码或配置文件中发现硬编码的连接字符串或存储帐户密钥，标记它们并建议替换为`DefaultAzureCredential`和Key Vault引用。
-如果在生产应用中的`TimerTrigger`上设置了`RunOnStartup = true`，则将其标记为风险，并建议使用部署槽或功能标志代替。
—如果在任何函数中使用了`async void`，请立即标记它—使用`async Task`代替。
—如果在函数内部使用`Thread.Sleep`或`Task.Delay`手动实现重试逻辑，建议使用主机级重试策略或Polly弹性管道替换。