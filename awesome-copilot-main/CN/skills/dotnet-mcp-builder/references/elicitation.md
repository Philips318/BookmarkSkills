#引出

触发允许工具通过客户端在执行过程中请求用户输入。法学硕士看不到这个问题；客户端将其直接呈现给用户。这将一次性工具调用转变为交互式流——收集确认、缺失参数、凭证（URL模式）等。

> **规格版本：** 2025-11-25。URL模式是较新的添加（最初2025-06-18只有表单模式）。

两种模式

|模式|做什么|何时使用||---|---|---|
| **表单（带内）** |服务器发送JSON Schema；客户端呈现表单；用户通过相同的MCP通道提交值。|确认，缺少参数，结构化选择。|
| **URL（带外）** |服务器发送URL；客户端在浏览器中打开；用户在此完成流程；服务器单独检查状态。oth，付款，任何MCP频道不能看到的东西。|

先决条件：有状态传输

触发要求服务器向客户端发送请求并等待响应。这只适用于：
- STDIO（总是）。
—有状态HTTP （`options.Stateless = false`）。

在无状态HTTP中，`ElicitAsync`将抛出—没有传输通道返回。

表单模式-完整的例子```csharp
using System.ComponentModel;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;

[McpServerToolType]
public class BookingTools
{
    [McpServerTool, Description("Books a meeting room. Asks the user for confirmation.")]
    public static async Task<string> BookRoom(
        IMcpServer server,
        [Description("Room name")] string room,
        [Description("Start time (ISO 8601)")] DateTime start,
        CancellationToken ct)
    {
        var elicit = await server.ElicitAsync(new ElicitRequestParams
        {
            Message = $"Confirm booking '{room}' at {start:HH:mm}?",
            RequestedSchema = new ElicitRequestParams.RequestSchema
            {
                Properties = new Dictionary<string, ElicitRequestParams.PrimitiveSchemaDefinition>
                {
                    ["confirm"] = new ElicitRequestParams.BooleanSchema
                    {
                        Description = "Confirm the booking",
                        Default = true
                    },
                    ["notes"] = new ElicitRequestParams.StringSchema
                    {
                        Description = "Optional notes for the booking"
                    }
                }
            }
        }, ct);

        if (elicit.Action != "accept")
            return "Booking cancelled by user.";

        var confirmed = elicit.Content?["confirm"].GetBoolean() ?? false;
        var notes     = elicit.Content?["notes"].GetString() ?? "";

        if (!confirmed)
            return "User declined to confirm.";

        // …perform the booking…
        return $"Booked '{room}' at {start:O}. Notes: {notes}";
    }
}
```
模式基本类型

你可以构建一个`RequestedSchema`：

|类型| c#类| Notes ||---|---|---|
| String |`StringSchema`|`Default`,`Description`。如果需要，可以在服务器端添加JSON-Schema验证。|
|数字|`NumberSchema`|用于整型和浮点数。|
|布尔值|`BooleanSchema`|呈现为复选框/切换。|
| Single-select enum (untitled) |`UntitledSingleSelectEnumSchema`|值列表；客户端呈现为dropdown/radio.|
| Single-select enum (title) |`TitledSingleSelectEnumSchema`|每个值有一个显示标题。|
|多选enum |`UntitledMultiSelectEnumSchema`/`TitledMultiSelectEnumSchema`|多选下拉/复选框组。|

每个都接受`Description`和`Default`。

响应形状`ElicitResult`:
—`Action`—`"accept"`、`"reject"`、`"cancel"`。一定要先检查这个。
—`Content`—`Dictionary<string, JsonElement>?`与用户提交的值。`null`如果用户rejected/cancelled.总是处理不可接受的路径：```csharp
if (elicit.Action == "cancel")
    return "User cancelled. No changes made.";
if (elicit.Action == "reject")
    return "User declined.";
// Action == "accept" → safe to read elicit.Content
```
## URL模式-完整示例

URL模式适用于用户必须在MCP通道之外完成某些操作的流——通常是OAuth。```csharp
[McpServerTool, Description("Connects the user's GitHub account.")]
public static async Task<string> ConnectGitHub(
    IMcpServer server,
    IOAuthService oauth,
    CancellationToken ct)
{
    var elicitationId = Guid.NewGuid().ToString();
    var authUrl = oauth.BuildAuthorizationUrl(state: elicitationId);

    var result = await server.ElicitAsync(new ElicitRequestParams
    {
        Mode = "url",
        ElicitationId = elicitationId,
        Url = authUrl,
        Message = "Please authorize access to GitHub in the browser window that just opened."
    }, ct);

    if (result.Action != "accept")
        return "Authorization cancelled.";

    // The user has come back. Look up the persisted token by elicitationId.
    var token = await oauth.GetTokenByStateAsync(elicitationId, ct);
    return token is not null ? "Connected." : "Authorization did not complete.";
}
```
# # #`UrlElicitationRequiredException`当一个工具在授权时被*阻塞*（而不是引导用户通过它），抛出`UrlElicitationRequiredException`。客户机将URL显示给用户，调用彻底失败。用于验证后重试模式：```csharp
if (!oauth.HasValidToken)
{
    var id = Guid.NewGuid().ToString();
    throw new UrlElicitationRequiredException(
        "Authorization required",
        new[]
        {
            new ElicitRequestParams
            {
                Mode = "url",
                ElicitationId = id,
                Url = oauth.BuildAuthorizationUrl(state: id),
                Message = "Sign in to continue."
            }
        });
}
```
##什么时候不要使用引子

LLM可以用自然语言询问琐碎的确认。**如果你可以在工具的文档字符串中表达“我应该做X吗？”，然后让LLM问，这比模态表单的摩擦要小。
- **法学硕士应该考虑的分支。**不要用表格代替法学硕士的判断——法学硕士不能决定的事情只能引出（用户秘密、实时同意、从只有用户知道的列表中选择）。
- **无状态部署。**不工作-见上面的先决条件。

客户端功能检查

不要盲目地调用`ElicitAsync`。检查:```csharp
if (server.ClientCapabilities?.Elicitation is null)
    return "This client doesn't support elicitation; please pass the value as an argument.";

var elicit = await server.ElicitAsync(...);
```
这在较老的客户端上可以很好地降级。