#提示

提示符是可重用的、参数化的消息模板，用户（而不是LLM）通常从列表中挑选——想想聊天客户端的“斜杠命令”。服务器定义它们；主机将它们呈现为菜单。

##解析提示符```csharp
using System.ComponentModel;
using Microsoft.Extensions.AI;
using ModelContextProtocol.Server;

[McpServerPromptType]
public class CodePrompts
{
    [McpServerPrompt, Description("Generates a code review prompt.")]
    public static IEnumerable<ChatMessage> CodeReview(
        [Description("The programming language")] string language,
        [Description("The code to review")] string code) =>
        [
            new(ChatRole.User,
                $"Please review the following {language} code:\n\n```{language}\n{code}\n```"),
            new(ChatRole.Assistant,
                "I'll review the code for correctness, style, and potential improvements.")
        ];
}
```
注册:```csharp
.WithPrompts<CodePrompts>()
// or
.WithPromptsFromAssembly()
```
##返回类型

|返回类型|结果||---|---|
|`ChatMessage`|单条消息。|
|`IEnumerable<ChatMessage>`|会话种子。|
|`PromptMessage`/`IEnumerable<PromptMessage>`|低级-当您需要完全控制内容块（嵌入式资源，每个消息的多个类型块）时使用。|
|完全控制-设置`Messages`和`Description`。|`ChatMessage`/`ChatRole`来自`Microsoft.Extensions.AI`。它们是高级的形状，你应该在90%的时间里使用它们。只有在需要嵌入资源或细粒度内容类型时，才下拉到`PromptMessage`/`ContentBlock`。

# #参数

当用户选择提示符时，每个参数（在SDK去掉的特殊参数之后——`IMcpServer`、`CancellationToken`等）都成为提示符参数。使用`[Description]`来解释用户应该提供什么。

要将参数标记为可选，请给它一个默认值：```csharp
[McpServerPrompt, Description("…")]
public static ChatMessage Greeting(
    [Description("Their preferred greeting style")] string style = "casual")
    => new(ChatRole.User, $"Greet me in a {style} style.");
```
图像和文件内容

对于包含图像的提示：```csharp
[McpServerPrompt, Description("Asks the model to analyze an image.")]
public static IEnumerable<ChatMessage> AnalyzeImage(
    [Description("Instructions for the analysis")] string instructions)
{
    byte[] imageBytes = LoadSampleImage();
    return new[]
    {
        new ChatMessage(ChatRole.User, new AIContent[]
        {
            new TextContent($"Please analyze this image: {instructions}"),
            new DataContent(imageBytes, "image/png")
        })
    };
}
```
对于嵌入的文本资源（例如，用用户选择的文档播种对话）：```csharp
[McpServerPrompt, Description("Reviews a referenced document.")]
public static IEnumerable<PromptMessage> ReviewDocument(
    [Description("The document ID to review")] string documentId)
{
    string content = LoadDocument(documentId);
    return new[]
    {
        new PromptMessage
        {
            Role = Role.User,
            Content = new TextContentBlock { Text = "Please review the following document:" }
        },
        new PromptMessage
        {
            Role = Role.User,
            Content = new EmbeddedResourceBlock
            {
                Resource = new TextResourceContents
                {
                    Uri = $"docs://documents/{documentId}",
                    MimeType = "text/plain",
                    Text = content
                }
            }
        }
    };
}
```
##异步提示

当你需要查找数据来构建消息时，提示可以是异步的：```csharp
[McpServerPrompt, Description("Drafts a release-notes prompt.")]
public static async Task<IEnumerable<ChatMessage>> ReleaseNotes(
    string repo,
    string fromTag,
    string toTag,
    IGitHubClient github,
    CancellationToken ct)
{
    var commits = await github.GetCommitsBetweenAsync(repo, fromTag, toTag, ct);
    var summary = string.Join("\n", commits.Select(c => $"- {c.Message}"));
    return new[]
    {
        new ChatMessage(ChatRole.User,
            $"Draft release notes for {repo} {fromTag}→{toTag} from these commits:\n{summary}")
    };
}
```
##通知客户端提示更改```csharp
await server.SendNotificationAsync(
    NotificationMethods.PromptListChangedNotification,
    new PromptListChangedNotificationParams(),
    cancellationToken);
```
何时使用提示与工具

- **提示：**用户*从菜单中触发它，提供任何所需的参数。输出是消息，而不是数据。适用于“/ summary”，“/code-review”，“/draft-email”。
**工具：** LLM*触发它（通常没有明确的用户操作）来获取或更改数据。适用于“get_weather”和“create_issue”。

如果两者都适用（用户想要一个触发LLM可以调用的相同逻辑的斜杠命令），则公开两者—相同的DTO/service可以返回两者。