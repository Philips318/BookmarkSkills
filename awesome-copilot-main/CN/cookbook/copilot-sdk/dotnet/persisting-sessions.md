#会话持久和恢复

跨应用程序重新启动保存和恢复会话会话。

示例场景

您希望用户能够在关闭并重新打开应用程序后继续对话。

> **可运行示例：** [recipe/persisting-sessions.cs]（recipe/persisting-sessions.cs）
>
>“bash
> CD食谱
运行persisting-sessions.cs> ' ' '

###使用自定义ID创建会话```csharp
using GitHub.Copilot;

await using var client = new CopilotClient();
await client.StartAsync();

// Create session with a memorable ID
var session = await client.CreateSessionAsync(new SessionConfig
{
    SessionId = "user-123-conversation",
    Model = "gpt-5",
    OnPermissionRequest = PermissionHandler.ApproveAll
});

await session.SendAsync(new MessageOptions { Prompt = "Let's discuss TypeScript generics" });

// Session ID is preserved
Console.WriteLine(session.SessionId); // "user-123-conversation"

// Destroy session but keep data on disk
await session.DisposeAsync();
await client.StopAsync();
```
###恢复会话```csharp
await using var client = new CopilotClient();
await client.StartAsync();

// Resume the previous session
var session = await client.ResumeSessionAsync("user-123-conversation", new ResumeSessionConfig { OnPermissionRequest = PermissionHandler.ApproveAll });

// Previous context is restored
await session.SendAsync(new MessageOptions { Prompt = "What were we discussing?" });

await session.DisposeAsync();
await client.StopAsync();
```
列出可用的会话```csharp
var sessions = await client.ListSessionsAsync();
foreach (var s in sessions)
{
    Console.WriteLine($"Session: {s.SessionId}");
}
```
永久删除会话```csharp
// Remove session and all its data from disk
await client.DeleteSessionAsync("user-123-conversation");
```
获取会话历史记录

从会话中检索所有事件：```csharp
using GitHub.Copilot; // UserMessageEvent, AssistantMessageEvent, etc. live in this namespace

var events = await session.GetEventsAsync();
foreach (var evt in events)
{
    switch (evt)
    {
        case UserMessageEvent user:
            Console.WriteLine($"[user] {user.Data.Content}");
            break;
        case AssistantMessageEvent assistant:
            Console.WriteLine($"[assistant] {assistant.Data.Content}");
            break;
        default:
            // Sessions can also contain other events (tool calls, tool results, system events).
            Console.WriteLine($"[{evt.GetType().Name}]");
            break;
    }
}
```
会话的事件流可能包括用户和助手消息之外的事件类型
>（例如工具调用、工具结果和系统事件）。处理你关心的人
>约并回落到默认情况，因此没有任何东西被静默丢弃。

最佳实践

1. **使用有意义的会话ID **：在会话ID中包含用户ID或上下文
2. **处理丢失的会话**：在恢复之前检查会话是否存在
3. **清理旧会话**：定期删除不再需要的会话