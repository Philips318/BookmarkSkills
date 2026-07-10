#会话持久和恢复

跨应用程序重新启动保存和恢复会话会话。

示例场景

您希望用户能够在关闭并重新打开应用程序后继续对话。

> **可运行示例：** [recipe/persisting-sessions.go]（recipe/persisting-sessions.go）
>
>“bash
> CD食谱
>运行persisting-sessions.go> ' ' '

###使用自定义ID创建会话```go
package main

import (
    "context"
    "fmt"
    copilot "github.com/github/copilot-sdk/go"
)

func main() {
    ctx := context.Background()
    client := copilot.NewClient(nil)
    client.Start(ctx)
    defer client.Stop()

    // Create session with a memorable ID
    session, _ := client.CreateSession(ctx, &copilot.SessionConfig{
    	OnPermissionRequest: copilot.PermissionHandler.ApproveAll,
        SessionID: "user-123-conversation",
        Model:     "gpt-5.4",
    })

    session.SendAndWait(ctx, copilot.MessageOptions{Prompt: "Let's discuss TypeScript generics"})

    // Session ID is preserved
    fmt.Println(session.SessionID)

    // Disconnect session but keep data on disk
    session.Disconnect()
}
```
###恢复会话```go
ctx := context.Background()
client := copilot.NewClient(nil)
client.Start(ctx)
defer client.Stop()

// Resume the previous session
session, _ := client.ResumeSession(ctx, "user-123-conversation", &copilot.ResumeSessionConfig{OnPermissionRequest: copilot.PermissionHandler.ApproveAll})

// Previous context is restored
session.SendAndWait(ctx, copilot.MessageOptions{Prompt: "What were we discussing?"})

session.Disconnect()
```
列出可用的会话```go
sessions, _ := client.ListSessions(ctx, nil)
for _, s := range sessions {
    fmt.Println("Session:", s.SessionID)
}
```
永久删除会话```go
// Remove session and all its data from disk
client.DeleteSession(ctx, "user-123-conversation")
```
获取会话历史记录```go
messages, _ := session.GetMessages(ctx)
for _, msg := range messages {
    if d, ok := msg.Data.(*copilot.AssistantMessageData); ok {
        fmt.Printf("[assistant.message] %s\n", d.Content)
    }
}
```
最佳实践

1. **使用有意义的会话ID **：在会话ID中包含用户ID或上下文
2. **处理丢失的会话**：在恢复之前检查会话是否存在
3. **清理旧会话**：定期删除不再需要的会话