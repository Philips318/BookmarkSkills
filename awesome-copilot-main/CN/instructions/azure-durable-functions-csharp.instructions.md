---
description: 'Guidelines and best practices for building Azure Durable Functions in C# using the isolated worker model'
applyTo: '**/*.cs, **/host.json, **/local.settings.json, **/*.csproj'
---
# Azure持久函数c#开发

##一般使用说明-对于新的持久功能项目，始终使用`Microsoft.Azure.Functions.Worker.Extensions.DurableTask`NuGet包的隔离工作模型。
-为编排器和活动上下文类型使用`Microsoft.DurableTask`命名空间（`TaskOrchestrationContext`,`TaskActivityContext`）。
-为了清晰起见，将编排器、活动、实体和客户端启动器功能分离到不同的类或文件中。
-永远不要将编排逻辑与活动逻辑混在一起-编排者相互协调；活动确实有效。
-始终使用`context.CreateReplaySafeLogger(nameof(OrchestratorName))`内部编排功能进行日志记录；永远不要在编排器中直接使用注入的`ILogger<T>`，因为它会记录每次重播。
-使用`async Task`或`async Task<T>`为所有协调器和活动方法-从不使用`async void`。
-将编排器代码视为**确定性和重放安全**：没有`DateTime.Now`，`Guid.NewGuid()`,`Random`，直接HTTP调用，或非确定性的I/O在编排器内。
-在编排器中使用`context.CurrentUtcDateTime`而不是`DateTime.UtcNow`项目结构-注册持久函数支持在`Program.cs`通过`builder.Services.AddDurableTaskClient()`和`builder.ConfigureFunctionsWorkerDefaults(x => x.UseDurableTask())`。
-将编排器、活动和实体组织到基于功能的文件夹中（例如，`/Orchestrations/OrderProcessing/`），而不是按功能类型组织。
-以后缀`Orchestrator`命名协调器（例如，`ProcessOrderOrchestrator`），以后缀`Activity`命名活动（例如，`ChargePaymentActivity`），以后缀`Entity`命名实体（例如，`CartEntity`）。
—传递给`CallActivityAsync`、`CallSubOrchestratorAsync`和`GetEntityStateAsync`的activity/orchestrator/entity名称使用常量或静态只读字符串，以防止输入错误。

##配置文件### local.settings.json
—始终包含`AzureWebJobsStorage`连接字符串用于本地开发—持久函数需要存储来维护业务流程状态。
—本地测试使用`"UseDevelopmentStorage=true"`或Azurite连接字符串—从不使用本地开发的生产存储帐户。
—在“local.settings.json”中设置“`FUNCTIONS_WORKER_RUNTIME`”为“`"dotnet-isolated"`”。
-对于Netherite或MSSQL存储提供商，包括特定于提供商的连接字符串（例如，`EventHubsConnection`为Netherite）。
-不要将`local.settings.json`提交到源代码管理-将其添加到`.gitignore`；使用带有占位符值的`local.settings.json.example`代替。
-如果需要，使用Azure密钥库本地通过`@Microsoft.KeyVault(...)`引用存储敏感值（存储键，事件中心连接字符串）。### host.json
—在`"extensions": { "durableTask": { ... } }`下配置持久函数特定的设置—不依赖于生产的默认值。
—将`"hubName"`设置为有意义的特定于环境的值（例如，`"MyAppProd"`,`"MyAppDev"`），以隔离共享相同存储帐户的环境中的任务中心。
—根据预期吞吐量和托管计划调整`"maxConcurrentActivityFunctions"`和`"maxConcurrentOrchestratorFunctions"`—默认值是保守的。
-在Premium/Dedicated计划上为长时间运行的编排启用扩展会话（`"extendedSessionsEnabled": true`）以减少重放开销。
—配置存储提供商：在大规模场景中使用`"storageProvider": { "type": "netherite" }`或`"mssql"`，而不是默认的Azure storage。
-适当设置`"maxQueuePollingInterval"`-较低的值会增加响应性，但会增加消耗计划上的存储事务成本。
-在`"logging": { "applicationInsights": { "samplingSettings": { ... } } }`下配置Application Insights采样率以控制遥测音量。

编排模式函数链接
-使用顺序的`await context.CallActivityAsync<T>(nameof(ActivityName), input)`调用分步工作流程，其中每个步骤取决于前一步的结果。
-在活动之间只传递可序列化的轻量级数据inputs/outputs-避免传递带有循环引用的整个域对象。

风扇输出/风扇输入
-使用`Task.WhenAll(tasks)`扇形后与多个`context.CallActivityAsync`调用聚合并行结果。
-限制在大型集合上展开时的并行度-使用批处理（例如，分区输入列表）来避免压倒下游服务或触及持久函数存储限制。
-优先使用`List<Task<T>>`而不是动态任务数组；在等待之前捕获所有任务以避免重播问题。异步HTTP API（人机交互/长时间运行）
-使用`client.ScheduleNewOrchestrationInstanceAsync`从HTTP触发启动器功能；返回`await client.CreateCheckStatusResponseAsync(req, instanceId)`以向调用者提供轮询url。
-将`context.WaitForExternalEvent<T>("EventName", timeout)`与`context.CreateTimer(deadline, CancellationToken)`结合使用，实现带有超时的approval/callback模式。
-总是处理超时竞赛：使用`Task.WhenAny(externalEventTask, timerTask)`，如果事件先到达，取消计时器。

监控/轮询模式
-使用`while`循环与`context.CreateTimer(context.CurrentUtcDateTime.Add(interval), CancellationToken.None)`轮询工作流，而不是单独的定时器触发函数。
—确保监控循环有明确的退出条件，避免无限循环永不终止。
—对于重复出现的永久工作流，使用`context.ContinueAsNew(input)`以新状态重新启动业务流程，并防止无限制的历史增长。永恒的管弦乐
-在编排器主体的末尾使用`context.ContinueAsNew(newInput)`，以清洁状态重新启动长期循环工作流程。
-在使用`isKeepRunning`模式时，在调用`ContinueAsNew`之前排除任何未决的外部事件。
—将“`ContinueAsNew`”与“`context.CreateTimer`”组合使用，实现周期性任务（如生成每日报表、刷新缓存）。

# # # Sub-Orchestrations
-使用`context.CallSubOrchestratorAsync<T>(nameof(SubOrchestrator), instanceId, input)`将复杂的工作流分解为可重用的子编排。
-当需要幂等或相关时，为子编排提供显式的`instanceId`。
-限制子业务流程嵌套深度以避免历史大小问题；尽可能使工作流扁平化。实体函数（有状态实体）
-使用基于类的语法定义实体，实现`TaskEntity<TState>`，用于类型化、封装状态管理。
-访问实体状态只能通过实体操作（`entity.State`）；永远不要直接读写实体存储。
-使用来自活动的`context.Entities.CallEntityAsync<T>`或来自编排器的`context.Entities.SignalEntityAsync`进行即发即弃的实体操作。
-当不需要返回值时，首选来自编排器的`SignalEntityAsync`而不是`CallEntityAsync`，以避免不必要的阻塞。
—对于需要分布式计数器、分布式锁、聚合器或per-user/per-session状态的场景，使用实体。
保持实体状态小且可序列化；避免存储在实体状态下无限制增长的大型blob或集合。

##活动函数-将活动功能集中在单个工作单元上-它们是执行I/O（数据库reads/writes， HTTP调用，队列发送）的唯一地方。
-通过构造函数DI注入服务（例如，`IRepository`,`IHttpClientFactory`）到包含活动函数的类中；不要在活动方法中使用`[FromServices]`。
-在可能的情况下使活动幂等-编排器可能在重试时多次调用相同的活动。
-使用`TaskActivityContext`参数类型为活动上下文；使用注入的`ILogger<T>`记录日志（不是重播安全的日志记录器-活动不会重播）。
-从活动中只返回可序列化的类型；避免返回带有导航属性的域实体。

错误处理和补偿-在编排器内的try/catch块中包装`context.CallActivityAsync`调用，以处理`TaskFailedException`，以实现优雅的错误处理和补偿。
-在捕获块中实现补偿事务（saga模式），当一个步骤在工作流程中失败时调用undo活动。
-使用`RetryPolicy`（通过`new TaskOptions(new RetryPolicy(maxRetries, firstRetryInterval))`）对活动调用进行自动重试，并在瞬态失败时退回。
—区分瞬态错误（重试）和业务错误（快速失败和补偿）—不重试验证或授权失败。
-总是通过持久功能管理API或客户端终止卡住的编排，如果他们进入一个错误状态，不能自我解决。

# #计时器-在协调器内部使用`context.CreateTimer(fireAt, CancellationToken)`进行持久延迟-从不使用`Task.Delay`或`Thread.Sleep`。
-总是通过传递和取消`CancellationTokenSource`来取消不再需要的计时器（例如，当外部事件在计时器触发之前到达时）。
-避免在消耗计划上的生产时间间隔过短（少于1分钟）；它们可能会导致过高的存储轮询成本。

##实例管理-当业务流程需要与业务实体关联时，使用有意义的、确定性的`instanceId`值（例如，`$"order-{orderId}"`）代替guid。
-在调度新实例之前，检查使用`client.GetInstanceMetadataAsync(instanceId)`的现有实例，以防止重复编排（单例模式）。
-使用`client.TerminateInstanceAsync`、`client.SuspendInstanceAsync`和`client.ResumeInstanceAsync`在管理api或管理功能中进行生命周期管理。
—定期清理completed/failed业务流程历史记录，使用`client.PurgeInstanceAsync`或批量清理来控制Task Hub存储增长。

# #可观测性-在编排器内使用`context.CreateReplaySafeLogger(nameof(Orchestrator))`进行所有日志记录，以防止重播期间重复的日志条目。
-记录来自编排器和启动器的每个日志语句中的`instanceId`，以实现端到端的可追溯性。
-使用与持久功能集成的应用程序洞察来跟踪编排生命周期事件、活动持续时间和故障。
-通过持久功能HTTP管理API端点（`/runtime/webhooks/durabletask/instances`）或持久功能监视器VS Code扩展监视业务流程运行状况。
—在“`host.json`”中设置“`durableTask.maxConcurrentOrchestratorFunctions`”和“`durableTask.maxConcurrentActivityFunctions`”，控制并发，防止资源耗尽。

存储和任务中心配置—在“`"extensions": { "durableTask": { "hubName": "MyTaskHub" } }`”下的“`host.json`”中配置任务中心名称，以隔离使用相同存储帐户的环境（dev/staging/prod）。
—每个环境使用单独的存储帐户或Task Hub名称，以避免跨环境干扰。
—对于高吞吐量场景，请使用**Netherite**或**MSSQL**存储提供商，而不是默认的Azure存储提供商，以提高性能并降低成本。
-避免将大的有效负载（>64KB）直接存储为编排inputs/outputs；将大数据存储在Blob Storage中，并传递引用（URL/ID）。

测试持久函数-使用`Microsoft.Azure.Functions.Worker.Extensions.DurableTask.Tests`NuGet包（如果可用）或手动模拟`TaskOrchestrationContext`用于单元测试编排器。
-作为常规方法隔离测试活动函数-为其依赖项（存储库，HTTP客户端）注入mock并对返回值进行断言。
-通过模拟`context.CallActivityAsync`、`context.CreateTimer`和`context.WaitForExternalEvent`来测试编排器逻辑，使用测试工具或手动模拟。
-避免测试持久功能运行时本身（事件溯源、重放）-将测试重点放在编排器和活动内部的业务逻辑上。
-使用与Azure或独立Azure存储帐户的集成测试来测试端到端工作流，包括启动器→编排器→活动→完成。
-在测试中使用确定性实例id（例如，`$"test-{Guid.NewGuid()}"`），以便通过`client.GetInstanceMetadataAsync`查询和验证业务流程状态。
-通过模拟`context.CreateTimer`立即启动并验证编排来测试超时场景或处理超时分支。
-通过强制活动失败（在模拟活动中抛出异常）和断言协调器调用补偿活动来测试compensation/error处理。
-在集成测试中使用`client.WaitForInstanceCompletionAsync`而不是轮询-它会阻塞，直到编排完成或超时。
—对于实体测试，在测试编排器中使用`context.Entities.SignalEntityAsync`，并在编排完成后通过`client.ReadEntityStateAsync`验证实体状态。现有代码审查指南

—如果在编排器中使用`DateTime.UtcNow`或`DateTime.Now`，将其标记并替换为`context.CurrentUtcDateTime`。
-如果在编排器中使用`Guid.NewGuid()`或`Random`，将其标记为不确定并将其移动到活动中。
-如果直接HTTP调用（`HttpClient.GetAsync`等）是在编排器内进行的，立即标记它们并将调用移动到活动函数中。
—如果在编排器内部使用了`Task.Delay`或`Thread.Sleep`，则替换为`context.CreateTimer`。
-如果在长时间循环中没有`ContinueAsNew`的情况下业务流程历史记录无限制地增长，建议添加`ContinueAsNew`来重置历史记录。
—如果实体状态存储大集合或blob数据，建议将大数据外部化到blob存储，只存储实体状态中的引用。
-如果活动函数不是幂等的，并且工作流没有retry/compensation逻辑，将其标记为可靠性风险。