# Microsoft Graph SDK for。网

当目标项目是用c#或其他语言编写时，请使用此参考。网络语言。

##权威来源

—SDK存储库：<https://github.com/microsoftgraph/msgraph-sdk-dotnet>—示例：<https://github.com/microsoftgraph/msgraph-training-dotnet>—SDK变更日志：<https://github.com/microsoftgraph/msgraph-sdk-dotnet/blob/main/CHANGELOG.md># #包```xml
<!-- Microsoft Graph SDK v5 (current) -->
<PackageReference Include="Microsoft.Graph" Version="5.*" />

<!-- Azure Identity for credential providers -->
<PackageReference Include="Azure.Identity" Version="1.*" />
```
通过命令行安装：```bash
dotnet add package Microsoft.Graph
dotnet add package Azure.Identity
```
##客户端设置

管理身份（azure托管的应用程序-首选）```csharp
using Azure.Identity;
using Microsoft.Graph;

var credential = new DefaultAzureCredential();
var graphClient = new GraphServiceClient(credential);
```
客户端凭证（app-only / daemon）```csharp
var credential = new ClientSecretCredential(
    tenantId: Environment.GetEnvironmentVariable("AZURE_TENANT_ID"),
    clientId: Environment.GetEnvironmentVariable("AZURE_CLIENT_ID"),
    clientSecret: Environment.GetEnvironmentVariable("AZURE_CLIENT_SECRET")
);
var graphClient = new GraphServiceClient(credential);
```
在生产环境中，首选`ClientCertificateCredential`而不是`ClientSecretCredential`。

On-Behalf-Of (OBO) -代理/ API作为登录用户```csharp
// incomingToken is the bearer token received from the caller
var credential = new OnBehalfOfCredential(
    tenantId: Environment.GetEnvironmentVariable("AZURE_TENANT_ID"),
    clientId: Environment.GetEnvironmentVariable("AZURE_CLIENT_ID"),
    clientSecret: Environment.GetEnvironmentVariable("AZURE_CLIENT_SECRET"),
    userAssertion: new UserAssertion(incomingToken)
);
var graphClient = new GraphServiceClient(credential);
```
###交互式（本地开发/ CLI）```csharp
var credential = new InteractiveBrowserCredential();
var graphClient = new GraphServiceClient(credential);
```
##常见呼叫模式

获取带有字段选择的资源```csharp
var user = await graphClient.Me.GetAsync(config =>
{
    config.QueryParameters.Select = ["displayName", "mail", "jobTitle"];
});
```
###列表过滤器和选择```csharp
var messages = await graphClient.Me.Messages.GetAsync(config =>
{
    config.QueryParameters.Filter = "isRead eq false";
    config.QueryParameters.Select = ["subject", "from", "receivedDateTime"];
    config.QueryParameters.Top = 25;
    config.QueryParameters.Orderby = ["receivedDateTime desc"];
});
```
###分页与PageIterator```csharp
var messages = await graphClient.Me.Messages.GetAsync();

var allMessages = new List<Message>();
var pageIterator = PageIterator<Message, MessageCollectionResponse>
    .CreatePageIterator(graphClient, messages, (msg) =>
    {
        allMessages.Add(msg);
        return true; // return false to stop early
    });

await pageIterator.IterateAsync();
```
###发送邮件```csharp
await graphClient.Me.SendMail.PostAsync(new SendMailPostRequestBody
{
    Message = new Message
    {
        Subject = "Hello from Graph",
        Body = new ItemBody { ContentType = BodyType.Text, Content = "Test message" },
        ToRecipients = [new Recipient { EmailAddress = new EmailAddress { Address = "user@contoso.com" } }]
    }
});
```
发布一个Teams频道消息```csharp
await graphClient.Teams[teamId].Channels[channelId].Messages.PostAsync(new ChatMessage
{
    Body = new ItemBody { ContentType = BodyType.Html, Content = "<b>Hello from Graph!</b>" }
});
```
批处理请求```csharp
using Microsoft.Graph.Models;

var batchRequestContent = new BatchRequestContentCollection(graphClient);

var meRequest = await batchRequestContent.AddBatchRequestStepAsync(
    graphClient.Me.ToGetRequestInformation());
var messagesRequest = await batchRequestContent.AddBatchRequestStepAsync(
    graphClient.Me.Messages.ToGetRequestInformation());

var batchResponse = await graphClient.Batch.PostAsync(batchRequestContent);

var me = await batchResponse.GetResponseByIdAsync<User>(meRequest);
var msgs = await batchResponse.GetResponseByIdAsync<MessageCollectionResponse>(messagesRequest);
```
##增量查询```csharp
// First sync — get all + deltaLink
var deltaResponse = await graphClient.Users.Delta.GetAsDeltaGetResponseAsync();
string? deltaLink = null;

var pageIterator = PageIterator<User, Microsoft.Graph.Users.Delta.DeltaGetResponse>
    .CreatePageIterator(graphClient, deltaResponse, (user) => { /* process */ return true; },
        (req) => { deltaLink = /* extract from response */; return req; });

await pageIterator.IterateAsync();
// Store deltaLink for next run

// Subsequent sync — only changes
// Use the stored deltaLink directly as the next request URL
```
节流/重试中间件

SDK包含默认启用的重试中间件。对于显式控制：```csharp
var handlers = GraphClientFactory.CreateDefaultHandlers();
// RetryHandler is included; configure max retries if needed
var httpClient = GraphClientFactory.Create(handlers);
var graphClient = new GraphServiceClient(httpClient, credential);
```
如果构建自定义重试逻辑，总是检查`Retry-After`-不要使用固定的指数回退。

依赖注入（ASP. js）. NET Core /。净工人)```csharp
// Program.cs
builder.Services.AddSingleton<GraphServiceClient>(_ =>
{
    var credential = new DefaultAzureCredential();
    return new GraphServiceClient(credential);
});
```
# #。NET-specific指导

-目标。NET 8+的新项目。
—始终使用`async`/`await`-所有Graph SDK调用都是异步的。
-将`GraphServiceClient`注册为单例（它在内部缓存令牌）。
-使用`ILogger`来记录图形异常-捕获`ODataError`以获取特定于图形的错误细节。
-对于ASP。使用OBO的。NET Core api，注入来自`IHttpContextAccessor`的传入令牌，并根据每个请求构造凭据（而不是作为单个凭据）。```csharp
// Catching Graph errors
try
{
    var user = await graphClient.Me.GetAsync();
}
catch (ODataError odataError)
{
    Console.WriteLine($"Graph error: {odataError.Error?.Code} - {odataError.Error?.Message}");
}
```
