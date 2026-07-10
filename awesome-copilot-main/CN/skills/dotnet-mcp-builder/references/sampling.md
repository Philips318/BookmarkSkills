#抽样

抽样允许工具**通过客户端调用LLM **，而不是带来自己的模型。服务器说“为我总结”，客户端将请求路由到用户配置的任何模型（Claude、GPT、本地模型，任何东西）。成本和速率限制由客户端决定，而不是服务器。

何时使用采样

-该工具需要一个LLM步骤（总结，分类，起草，提取），你不想在服务器上ship/configure你自己的模型。
-你要尊重用户的型号选择、密钥和成本偏好。
-你正在构建一个“元”工具，将LLM工作作为其工作的一部分进行编排（例如多步骤代理）。

如果你已经有了一个确定性的算法，不要添加一个“为了味道”的抽样调用——它会增加延迟和成本。

先决条件：有状态传输与启发一样，采样需要服务器回调到客户机。STDIO总是工作；HTTP需要`options.Stateless = false`。

建议：`IChatClient`适配器

最干净的API将采样通道包装为`Microsoft.Extensions.AI.IChatClient`，因此您编写的代码看起来像普通的llm调用。NET:```csharp
using System.ComponentModel;
using Microsoft.Extensions.AI;
using ModelContextProtocol.Server;

[McpServerToolType]
public class SummaryTools
{
    [McpServerTool(Name = "SummarizeContent"), Description("Summarises arbitrary text using the client's LLM.")]
    public static async Task<string> Summarize(
        IMcpServer server,
        [Description("The text to summarize")] string text,
        CancellationToken cancellationToken)
    {
        ChatMessage[] messages =
        [
            new(ChatRole.User, "Briefly summarize the following content:"),
            new(ChatRole.User, text),
        ];

        var options = new ChatOptions
        {
            MaxOutputTokens = 256,
            Temperature = 0.3f,
        };

        var response = await server.AsSamplingChatClient()
            .GetResponseAsync(messages, options, cancellationToken);

        return $"Summary: {response}";
    }
}
```
为什么这很好：
—其余`IChatClient`API相同。. NET AI生态系统的使用。
-工作与`Microsoft.Extensions.AI`中间件（速率限制，重试，遥测，函数调用）。
-可以通过注入不同的`IChatClient`在测试中切换到直接提供程序。

低级：`SampleAsync`当你需要完全控制请求形状时：```csharp
using ModelContextProtocol.Protocol;

CreateMessageResult result = await server.SampleAsync(
    new CreateMessageRequestParams
    {
        Messages =
        [
            new SamplingMessage
            {
                Role = Role.User,
                Content = [new TextContentBlock { Text = "What is 2 + 2?" }]
            }
        ],
        MaxTokens = 100,
        Temperature = 0.0f,
        SystemPrompt = "You are a precise calculator.",
        // ModelPreferences, StopSequences, IncludeContext...
    },
    cancellationToken);

string answer = result.Content
    .OfType<TextContentBlock>()
    .FirstOrDefault()?.Text ?? string.Empty;
```
`ModelPreferences`让你提示模型选择（成本vs速度vs智能优先级）；客户端决定实际的模型。```csharp
ModelPreferences = new ModelPreferences
{
    Hints = [new ModelHint { Name = "claude" }],   // soft preference
    CostPriority = 0.2,        // 0..1
    SpeedPriority = 0.4,
    IntelligencePriority = 0.9,
}
```
# #`IncludeContext`采样请求可以要求客户端包含当前会话的上下文：```csharp
IncludeContext = ContextInclusion.ThisServer   // include this server's prior messages
// or AllServers, or None (default)
```
当您需要LLM考虑到目前为止在聊天中发生的事情而无需重新提供它时，它非常有用。

##功能检查

始终确认客户支持抽样-许多不支持：```csharp
if (server.ClientCapabilities?.Sampling is null)
    throw new McpException(
        "This client does not support sampling. " +
        "Configure a model in the host or use a different MCP client.");
```
##性能说明

抽样调用是网络往返（客户端→其提供者→返回）。预计100毫秒数秒。不要太紧。
-令牌成本由*用户*支付（他们的APIkey/quota）。保守地使用`MaxTokens`。
-取消传播：如果用户杀死工具调用，采样请求也被取消。

抽样vs.服务器端抽样

|采样（通过客户端）|直接LLM调用（服务器端）||---|---|
|使用用户型号+密钥|使用您的业务密钥|
|尊重用户的policy/quota|你对bill/track|的责任
|可以在用户将|锁定到随|发货的模型的任何主机上工作
|高延迟（额外跳）|低延迟，直接|
|无秘密管理|管理API密钥|

对于发送给许多用户的“智能”服务器，更倾向于抽样。对于内部企业服务器，如果你想要一致的行为，你已经为模型付费，直接是可以的。