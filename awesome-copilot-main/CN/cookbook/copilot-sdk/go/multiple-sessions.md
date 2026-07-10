#使用多个会话

同时管理多个独立对话。

> **可运行示例：** [recipe/multiple-sessions.go]（recipe/multiple-sessions.go）
>
>“bash
>运行recipe/multiple-sessions.go> ' ' '

示例场景

您需要并行运行多个会话，每个会话都有自己的上下文和历史。

# #去```go
package main

import (
    "context"
    "fmt"
    "log"
    copilot "github.com/github/copilot-sdk/go"
)

func main() {
    ctx := context.Background()
    client := copilot.NewClient(nil)

    if err := client.Start(ctx); err != nil {
        log.Fatal(err)
    }
    defer client.Stop()

    // Create multiple independent sessions
    session1, err := client.CreateSession(ctx, &copilot.SessionConfig{
    	OnPermissionRequest: copilot.PermissionHandler.ApproveAll,
    	Model:               "gpt-5.4",
    })
    if err != nil {
        log.Fatal(err)
    }
    defer session1.Disconnect()

    session2, err := client.CreateSession(ctx, &copilot.SessionConfig{
    	OnPermissionRequest: copilot.PermissionHandler.ApproveAll,
    	Model:               "gpt-5.4",
    })
    if err != nil {
        log.Fatal(err)
    }
    defer session2.Disconnect()

    session3, err := client.CreateSession(ctx, &copilot.SessionConfig{
    	OnPermissionRequest: copilot.PermissionHandler.ApproveAll,
    	Model:               "claude-sonnet-4.6",
    })
    if err != nil {
        log.Fatal(err)
    }
    defer session3.Disconnect()

    // Each session maintains its own conversation history
    session1.Send(ctx, copilot.MessageOptions{Prompt: "You are helping with a Python project"})
    session2.Send(ctx, copilot.MessageOptions{Prompt: "You are helping with a TypeScript project"})
    session3.Send(ctx, copilot.MessageOptions{Prompt: "You are helping with a Go project"})

    // Follow-up messages stay in their respective contexts
    session1.Send(ctx, copilot.MessageOptions{Prompt: "How do I create a virtual environment?"})
    session2.Send(ctx, copilot.MessageOptions{Prompt: "How do I set up tsconfig?"})
    session3.Send(ctx, copilot.MessageOptions{Prompt: "How do I initialize a module?"})
}
```
##自定义会话id

使用自定义id更容易跟踪：```go
session, err := client.CreateSession(ctx, &copilot.SessionConfig{
	OnPermissionRequest: copilot.PermissionHandler.ApproveAll,
    SessionID: "user-123-chat",
    Model:     "gpt-5.4",
})
if err != nil {
    log.Fatal(err)
}

fmt.Println(session.SessionID) // "user-123-chat"
```
##列出会话```go
sessions, err := client.ListSessions(ctx, nil)
if err != nil {
    log.Fatal(err)
}

for _, sessionInfo := range sessions {
    fmt.Printf("Session: %s\n", sessionInfo.SessionID)
}
```
##删除会话```go
// Delete a specific session
if err := client.DeleteSession(ctx, "user-123-chat"); err != nil {
    log.Printf("Failed to delete session: %v", err)
}
```
##用例

- **多用户应用程序**：每个用户一个会话
- **多任务工作流**：不同任务的单独会话
- **A/B测试**：比较不同型号的响应