#集成目录

Aspire在13个类别中有144多个集成。使用MCP工具获取实时的、最新的集成数据，而不是维护一个静态列表。

---

发现集成（MCP工具）

Aspire MCP服务器提供了两种用于集成发现的工具——它们可以在所有CLI版本（13.1+）上工作，并且不需要运行AppHost。

|工具|它做什么|何时使用|| ---------------------- | -------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------- |
|`list_integrations`|返回所有可用的Aspire托管集成及其NuGet包id |“哪些集成可用于数据库？”/“向我展示所有与redis相关的集成”|
|`get_integration_docs`|检索特定集成包的详细文档（设置、配置、代码示例）|“如何配置PostgreSQL?”/“给我看看`Aspire.Hosting.Redis`的文件”|

# # #工作流程

1. **浏览** -拨打`list_integrations`查看可用的内容。按类别或关键字过滤结果。
2. **Deep dive** -使用包ID（例如，`Aspire.Hosting.Redis`）和版本（例如，`9.0.0`）调用`get_integration_docs`以获得完整的安装说明。
3. **添加** -运行`aspire add <integration>`将主机包安装到AppHost中。

提示：这些工具返回与[官方集成库]（https://aspire.dev/integrations/gallery/）相同的数据。比起静态文档，更喜欢它们——集成是经常添加的。

---集成模式

每个集成都遵循两个包的模式：

—**Hosting package** (`Aspire.Hosting.*`)—将资源添加到AppHost中
- **客户端包** (`Aspire.*`) -配置客户端SDK在您的服务与健康检查，遥测和重试
- **社区工具包** (`CommunityToolkit.Aspire.*`) -社区维护的集成从[Aspire社区工具包]（https://github.com/CommunityToolkit/Aspire）```csharp
// === AppHost (hosting side) ===
var redis = builder.AddRedis("cache");  // Aspire.Hosting.Redis
var api = builder.AddProject<Projects.Api>("api")
    .WithReference(redis);

// === Service (client side) — in API's Program.cs ===
builder.AddRedisClient("cache");        // Aspire.StackExchange.Redis
// Automatically configures: connection string, health checks, OpenTelemetry, retries
```
---

##类别一目了然

使用`list_integrations`获取完整的活动列表。本摘要涵盖主要类别：

|类别|关键集成|托管包示例|| ------------------- | ------------------------------------------------------------------------------------- | ---------------------------------------- |
| **AI** | Azure OpenAI, OpenAI， GitHub模型，Ollama |`Aspire.Hosting.Azure.CognitiveServices`|
| **缓存** | Redis， Garnet, Valkey， Azure缓存Redis |`Aspire.Hosting.Redis`|
| **云/ Azure** |存储，Cosmos DB，服务总线，密钥库，事件中心，函数，SQL，信号r (25+) |`Aspire.Hosting.Azure.Storage`|
| **云/ AWS** | AWS SDK集成|`Aspire.Hosting.AWS`|
| **数据库** | PostgreSQL、SQL Server、MongoDB、MySQL、Oracle、Elasticsearch、Milvus、Qdrant、SQLite |`Aspire.Hosting.PostgreSQL`|
| **DevTools** |数据API Builder， Dev tunnel, Mailpit, k6, flag, Ngrok, Stripe |`Aspire.Hosting.DevTunnels`|
| RabbitMQ, Kafka， NATS, ActiveMQ, LavinMQ|`Aspire.Hosting.RabbitMQ`|
| **Observability** | OpenTelemetry（内置），Seq, OTel Collector |`Aspire.Hosting.Seq`|
| **Compute** | Docker Compose, Kubernetes |`Aspire.Hosting.Docker`|
| **反向代理** | YARP |`Aspire.Hosting.Yarp`|
| **安全** | Keycloak |`Aspire.Hosting.Keycloak`|
| **框架** | JavaScript， Python, Go, Java, Rust, Bun, Deno, Orleans， MAUI, Dapr, PowerShell |`Aspire.Hosting.Python`|有关多语言框架方法签名，请参见[polyglot api]（polyglot-apis.md）。

---