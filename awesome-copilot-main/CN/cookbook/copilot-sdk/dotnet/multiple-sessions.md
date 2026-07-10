#使用多个会话

同时管理多个独立对话。

> **可运行示例：** [recipe/multiple-sessions.cs]（recipe/multiple-sessions.cs）
>
>“bash
运行recipe/multiple-sessions.cs> ' ' '

示例场景

您需要并行运行多个会话，每个会话都有自己的上下文和历史。

## c #```csharp
using GitHub.Copilot;

await using var client = new CopilotClient();
await client.StartAsync();

// Create multiple independent sessions
var session1 = await client.CreateSessionAsync(new SessionConfig
{
    Model = "gpt-5",
    OnPermissionRequest = PermissionHandler.ApproveAll
});
var session2 = await client.CreateSessionAsync(new SessionConfig
{
    Model = "gpt-5",
    OnPermissionRequest = PermissionHandler.ApproveAll
});
var session3 = await client.CreateSessionAsync(new SessionConfig
{
    Model = "claude-sonnet-4.5",
    OnPermissionRequest = PermissionHandler.ApproveAll
});

// Each session maintains its own conversation history
await session1.SendAsync(new MessageOptions { Prompt = "You are helping with a Python project" });
await session2.SendAsync(new MessageOptions { Prompt = "You are helping with a TypeScript project" });
await session3.SendAsync(new MessageOptions { Prompt = "You are helping with a Go project" });

// Follow-up messages stay in their respective contexts
await session1.SendAsync(new MessageOptions { Prompt = "How do I create a virtual environment?" });
await session2.SendAsync(new MessageOptions { Prompt = "How do I set up tsconfig?" });
await session3.SendAsync(new MessageOptions { Prompt = "How do I initialize a module?" });

// Clean up all sessions
await session1.DisposeAsync();
await session2.DisposeAsync();
await session3.DisposeAsync();
```
##自定义会话id

使用自定义id更容易跟踪：```csharp
var session = await client.CreateSessionAsync(new SessionConfig
{
    SessionId = "user-123-chat",
    Model = "gpt-5",
    OnPermissionRequest = PermissionHandler.ApproveAll
});

Console.WriteLine(session.SessionId); // "user-123-chat"
```
##列出会话```csharp
var sessions = await client.ListSessionsAsync();
foreach (var sessionInfo in sessions)
{
    Console.WriteLine($"Session: {sessionInfo.SessionId}");
}
```
##删除会话```csharp
// Delete a specific session
await client.DeleteSessionAsync("user-123-chat");
```
##用例

- **多用户应用程序**：每个用户一个会话
- **多任务工作流**：不同任务的单独会话
- **A/B测试**：比较不同型号的响应