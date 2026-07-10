---
applyTo: "**.go, go.mod"
description: "This file provides guidance on building Go applications using GitHub Copilot SDK."
name: "GitHub Copilot SDK Go Instructions"
---
##核心原则

- SDK处于技术预览阶段，可能会有突破性的变化
—要求Go 1.21及以上版本
—需要安装GitHub CopilotCLI，并放在PATH中
-使用线程程序和通道进行并发操作
-标准库之外没有外部依赖

# #安装

总是通过Go模块安装：```bash
go get github.com/github/copilot-sdk/go
```
##客户端初始化

基本客户端设置```go
import "github.com/github/copilot-sdk/go"

client := copilot.NewClient(nil)
if err := client.Start(); err != nil {
    log.Fatal(err)
}
defer client.Stop()
```
客户端配置选项

当创建一个CopilotClient时，使用`ClientOptions`：

-`CLIPath`- CLI可执行文件的路径（默认为Path中的“copilot”）
-`CLIUrl`-已存在的CLI服务器的URL（例如localhost:8080）。当提供时，客户端不会生成进程
-`Port`-服务器端口（随机默认为0）
-`UseStdio`-使用stdio传输而不是TCP（默认值：true）
-`LogLevel`-日志级别（默认为“info”）
-`AutoStart`-自动启动服务器（默认值：true，使用指针：`boolPtr(true)`）
-`AutoRestart`-崩溃时自动重启（默认：true，使用指针：`boolPtr(true)`）
—`Cwd`—CLI进程的工作目录
—`Env`—CLI进程环境变量（[]string）

手动服务器控制

对于显式控制：```go
autoStart := false
client := copilot.NewClient(&copilot.ClientOptions{AutoStart: &autoStart})
if err := client.Start(); err != nil {
    log.Fatal(err)
}
// Use client...
client.Stop()
```
当`Stop()`耗时太长时，请使用`ForceStop()`。

##会话管理

创建会话

使用`SessionConfig`进行配置：```go
session, err := client.CreateSession(&copilot.SessionConfig{
	OnPermissionRequest: copilot.PermissionHandler.ApproveAll,
    Model: "gpt-5",
    Streaming: true,
    Tools: []copilot.Tool{...},
    SystemMessage: &copilot.SystemMessageConfig{ ... },
    AvailableTools: []string{"tool1", "tool2"},
    ExcludedTools: []string{"tool3"},
    Provider: &copilot.ProviderConfig{ ... },
})
if err != nil {
    log.Fatal(err)
}
```
会话配置选项

-`SessionID`—自定义会话ID
-`Model`-模型名称（"gpt-5", “claude-sonnet-4.5”等）
-`Tools`-自定义工具暴露于CLI （[]Tool）`SystemMessage`-系统消息定制（\*SystemMessageConfig）
-`AvailableTools`-允许工具名称列表（[]string）
-`ExcludedTools`-工具名称黑名单（[]string）
自定义API提供程序配置（BYOK） （\*ProviderConfig）
-`Streaming`-启用流响应块（bool）
—`MCPServers`—MCP服务器配置
-`CustomAgents`-自定义座席配置
-`ConfigDir`-配置目录覆盖
-`SkillDirectories`-技能目录（[]string）
-`DisabledSkills`-禁用技能（[]string）

###恢复会话```go
session, err := client.ResumeSession("session-id", &copilot.ResumeSessionConfig{OnPermissionRequest: copilot.PermissionHandler.ApproveAll})
// Or with options:
session, err := client.ResumeSessionWithOptions("session-id", &copilot.ResumeSessionConfig{ ... })
```
会话操作

-`session.SessionID`-获取会话标识符（字符串）`session.Send(copilot.MessageOptions{Prompt: "...", Attachments: []copilot.Attachment{...}})`-发送消息，返回（messageID字符串，错误）`session.SendAndWait(options, timeout)`-发送并等待空闲，返回（\*SessionEvent，错误）
-`session.Abort()`-中止当前处理，返回错误
-获取所有events/messages，返回（[]SessionEvent, error）
-`session.Destroy()`-清理会话，返回错误

##事件处理

事件订阅模式

总是使用通道或done信号来等待会话事件：```go
done := make(chan struct{})

unsubscribe := session.On(func(evt copilot.SessionEvent) {
    switch evt.Type {
    case copilot.AssistantMessage:
        fmt.Println(*evt.Data.Content)
    case copilot.SessionIdle:
        close(done)
    }
})
defer unsubscribe()

session.Send(copilot.MessageOptions{Prompt: "..."})
<-done
```
###取消订阅事件`On()`方法返回一个取消订阅的函数：```go
unsubscribe := session.On(func(evt copilot.SessionEvent) {
    // handler
})
// Later...
unsubscribe()
```
事件类型

使用类型开关进行事件处理：```go
session.On(func(evt copilot.SessionEvent) {
    switch evt.Type {
    case copilot.UserMessage:
        // Handle user message
    case copilot.AssistantMessage:
        if evt.Data.Content != nil {
            fmt.Println(*evt.Data.Content)
        }
    case copilot.ToolExecutionStart:
        // Tool execution started
    case copilot.ToolExecutionComplete:
        // Tool execution completed
    case copilot.SessionStart:
        // Session started
    case copilot.SessionIdle:
        // Session is idle (processing complete)
    case copilot.SessionError:
        if evt.Data.Message != nil {
            fmt.Println("Error:", *evt.Data.Message)
        }
    }
})
```
##流式响应

###启用流

在SessionConfig中设置`Streaming: true`：```go
session, err := client.CreateSession(&copilot.SessionConfig{
	OnPermissionRequest: copilot.PermissionHandler.ApproveAll,
    Model: "gpt-5",
    Streaming: true,
})
```
处理流事件

处理增量事件和最终事件：```go
done := make(chan struct{})

session.On(func(evt copilot.SessionEvent) {
    switch evt.Type {
    case copilot.AssistantMessageDelta:
        // Incremental text chunk
        if evt.Data.DeltaContent != nil {
            fmt.Print(*evt.Data.DeltaContent)
        }
    case copilot.AssistantReasoningDelta:
        // Incremental reasoning chunk (model-dependent)
        if evt.Data.DeltaContent != nil {
            fmt.Print(*evt.Data.DeltaContent)
        }
    case copilot.AssistantMessage:
        // Final complete message
        fmt.Println("\n--- Final ---")
        if evt.Data.Content != nil {
            fmt.Println(*evt.Data.Content)
        }
    case copilot.AssistantReasoning:
        // Final reasoning content
        fmt.Println("--- Reasoning ---")
        if evt.Data.Content != nil {
            fmt.Println(*evt.Data.Content)
        }
    case copilot.SessionIdle:
        close(done)
    }
})

session.Send(copilot.MessageOptions{Prompt: "Tell me a story"})
<-done
```
注意：无论流设置如何，最终事件（`AssistantMessage`,`AssistantReasoning`）总是被发送。

##自定义工具

定义工具```go
session, err := client.CreateSession(&copilot.SessionConfig{
	OnPermissionRequest: copilot.PermissionHandler.ApproveAll,
    Model: "gpt-5",
    Tools: []copilot.Tool{
        {
            Name:        "lookup_issue",
            Description: "Fetch issue details from tracker",
            Parameters: map[string]interface{}{
                "type": "object",
                "properties": map[string]interface{}{
                    "id": map[string]interface{}{
                        "type":        "string",
                        "description": "Issue ID",
                    },
                },
                "required": []string{"id"},
            },
            Handler: func(inv copilot.ToolInvocation) (copilot.ToolResult, error) {
                args := inv.Arguments.(map[string]interface{})
                issueID := args["id"].(string)

                issue, err := fetchIssue(issueID)
                if err != nil {
                    return copilot.ToolResult{}, err
                }

                return copilot.ToolResult{
                    TextResultForLLM: fmt.Sprintf("Issue: %v", issue),
                    ResultType:       "success",
                    ToolTelemetry:    map[string]interface{}{},
                }, nil
            },
        },
    },
})
```
工具返回类型

-返回带有字段的`ToolResult`结构体：
-`TextResultForLLM`(string) - LLM的结果文本
-`ResultType`(string) -“成功”或“失败”
-`Error`（字符串，可选）-内部错误消息（不显示给LLM）
-`ToolTelemetry`(map[string]interface{}) -遥测数据

工具执行流

当Copilot调用一个工具时，客户端会自动：

1. 运行处理程序函数
2. 返回ToolResult
3. 响应CLI

系统消息定制

附加模式（默认-保留护栏）```go
session, err := client.CreateSession(&copilot.SessionConfig{
	OnPermissionRequest: copilot.PermissionHandler.ApproveAll,
    Model: "gpt-5",
    SystemMessage: &copilot.SystemMessageConfig{
        Mode: "append",
        Content: `
<workflow_rules>
- Always check for security vulnerabilities
- Suggest performance improvements when applicable
</workflow_rules>
`,
    },
})
```
替换模式（完全控制-移除护栏）```go
session, err := client.CreateSession(&copilot.SessionConfig{
	OnPermissionRequest: copilot.PermissionHandler.ApproveAll,
    Model: "gpt-5",
    SystemMessage: &copilot.SystemMessageConfig{
        Mode:    "replace",
        Content: "You are a helpful assistant.",
    },
})
```
##文件附件

使用`Attachment`将文件附加到消息：```go
messageID, err := session.Send(copilot.MessageOptions{
    Prompt: "Analyze this file",
    Attachments: []copilot.Attachment{
        {
            Type:        "file",
            Path:        "/path/to/file.go",
            DisplayName: "My File",
        },
    },
})
```
##消息传递模式

在`MessageOptions`中使用`Mode`字段：

-`"enqueue"`-等待处理的队列消息
-`"immediate"`-立即处理消息```go
session.Send(copilot.MessageOptions{
    Prompt: "...",
    Mode:   "enqueue",
})
```
##多会话

会话是独立的，可以并发运行：```go
session1, _ := client.CreateSession(&copilot.SessionConfig{
	OnPermissionRequest: copilot.PermissionHandler.ApproveAll,
	Model:               "gpt-5",
})
session2, _ := client.CreateSession(&copilot.SessionConfig{
	OnPermissionRequest: copilot.PermissionHandler.ApproveAll,
	Model:               "claude-sonnet-4.5",
})

session1.Send(copilot.MessageOptions{Prompt: "Hello from session 1"})
session2.Send(copilot.MessageOptions{Prompt: "Hello from session 2"})
```
自带钥匙（BYOK）

通过配置`ProviderConfig`来使用自定义API提供程序：```go
session, err := client.CreateSession(&copilot.SessionConfig{
	OnPermissionRequest: copilot.PermissionHandler.ApproveAll,
    Provider: &copilot.ProviderConfig{
        Type:    "openai",
        BaseURL: "https://api.openai.com/v1",
        APIKey:  "your-api-key",
    },
})
```
会话生命周期管理

检查连接状态```go
state := client.GetState()
// Returns: "disconnected", "connecting", "connected", or "error"
```
##错误处理

标准异常处理```go
session, err := client.CreateSession(&copilot.SessionConfig{
	OnPermissionRequest: copilot.PermissionHandler.ApproveAll,
})
if err != nil {
    log.Fatalf("Failed to create session: %v", err)
}

_, err = session.Send(copilot.MessageOptions{Prompt: "Hello"})
if err != nil {
    log.Printf("Failed to send: %v", err)
}
```
会话错误事件

监视`SessionError`类型以查看运行时错误：```go
session.On(func(evt copilot.SessionEvent) {
    if evt.Type == copilot.SessionError {
        if evt.Data.Message != nil {
            fmt.Fprintf(os.Stderr, "Session Error: %s\n", *evt.Data.Message)
        }
    }
})
```
##连接测试

使用`Ping`验证服务器连通性：```go
resp, err := client.Ping("test message")
if err != nil {
    log.Printf("Server unreachable: %v", err)
} else {
    log.Printf("Server responded at %d", resp.Timestamp)
}
```
##资源清理

使用Defer进行清理

总是使用`defer`进行清理：```go
client := copilot.NewClient(nil)
if err := client.Start(); err != nil {
    log.Fatal(err)
}
defer client.Stop()

session, err := client.CreateSession(&copilot.SessionConfig{OnPermissionRequest: copilot.PermissionHandler.ApproveAll})
if err != nil {
    log.Fatal(err)
}
defer session.Destroy()
```
###手动清理

如果不使用defer：```go
client := copilot.NewClient(nil)
err := client.Start()
if err != nil {
    log.Fatal(err)
}

session, err := client.CreateSession(&copilot.SessionConfig{OnPermissionRequest: copilot.PermissionHandler.ApproveAll})
if err != nil {
    client.Stop()
    log.Fatal(err)
}

// Use session...

session.Destroy()
errors := client.Stop()
for _, err := range errors {
    log.Printf("Cleanup error: %v", err)
}
```
最佳实践

1. **总是使用`defer`**来清理客户端和会话
2. **使用通道**等待SessionIdle事件
3. **处理SessionError**事件健壮的错误处理
4. **使用类型开关**处理事件
5. **在交互场景中启用流**以获得更好的用户体验
6. **提供描述性工具名称和描述，以便更好地理解模型
7. **不再需要时调用取消订阅函数**
8. **使用SystemMessageConfig with Mode: "append"**来保护安全护栏
9. **处理增量和最终事件**时，流是启用的
10. **检查事件数据中的空指针** （Content， Message等都是指针）

##常见模式

简单查询-响应```go
client := copilot.NewClient(nil)
if err := client.Start(); err != nil {
    log.Fatal(err)
}
defer client.Stop()

session, err := client.CreateSession(&copilot.SessionConfig{
	OnPermissionRequest: copilot.PermissionHandler.ApproveAll,
	Model:               "gpt-5",
})
if err != nil {
    log.Fatal(err)
}
defer session.Destroy()

done := make(chan struct{})

session.On(func(evt copilot.SessionEvent) {
    if evt.Type == copilot.AssistantMessage && evt.Data.Content != nil {
        fmt.Println(*evt.Data.Content)
    } else if evt.Type == copilot.SessionIdle {
        close(done)
    }
})

session.Send(copilot.MessageOptions{Prompt: "What is 2+2?"})
<-done
```
多回合对话```go
session, _ := client.CreateSession(&copilot.SessionConfig{OnPermissionRequest: copilot.PermissionHandler.ApproveAll})
defer session.Destroy()

sendAndWait := func(prompt string) error {
    done := make(chan struct{})
    var eventErr error

    unsubscribe := session.On(func(evt copilot.SessionEvent) {
        switch evt.Type {
        case copilot.AssistantMessage:
            if evt.Data.Content != nil {
                fmt.Println(*evt.Data.Content)
            }
        case copilot.SessionIdle:
            close(done)
        case copilot.SessionError:
            if evt.Data.Message != nil {
                eventErr = fmt.Errorf(*evt.Data.Message)
            }
        }
    })
    defer unsubscribe()

    if _, err := session.Send(copilot.MessageOptions{Prompt: prompt}); err != nil {
        return err
    }
    <-done
    return eventErr
}

sendAndWait("What is the capital of France?")
sendAndWait("What is its population?")
```
SendAndWait Helper```go
// Use built-in SendAndWait for simpler synchronous interaction
response, err := session.SendAndWait(copilot.MessageOptions{
    Prompt: "What is 2+2?",
}, 0) // 0 uses default 60s timeout

if err != nil {
    log.Printf("Error: %v", err)
}
if response != nil && response.Data.Content != nil {
    fmt.Println(*response.Data.Content)
}
```
带有Struct返回类型的工具```go
type UserInfo struct {
    ID    string `json:"id"`
    Name  string `json:"name"`
    Email string `json:"email"`
    Role  string `json:"role"`
}

session, _ := client.CreateSession(&copilot.SessionConfig{
	OnPermissionRequest: copilot.PermissionHandler.ApproveAll,
    Tools: []copilot.Tool{
        {
            Name:        "get_user",
            Description: "Retrieve user information",
            Parameters: map[string]interface{}{
                "type": "object",
                "properties": map[string]interface{}{
                    "user_id": map[string]interface{}{
                        "type":        "string",
                        "description": "User ID",
                    },
                },
                "required": []string{"user_id"},
            },
            Handler: func(inv copilot.ToolInvocation) (copilot.ToolResult, error) {
                args := inv.Arguments.(map[string]interface{})
                userID := args["user_id"].(string)

                user := UserInfo{
                    ID:    userID,
                    Name:  "John Doe",
                    Email: "john@example.com",
                    Role:  "Developer",
                }

                jsonBytes, _ := json.Marshal(user)
                return copilot.ToolResult{
                    TextResultForLLM: string(jsonBytes),
                    ResultType:       "success",
                    ToolTelemetry:    map[string]interface{}{},
                }, nil
            },
        },
    },
})
```
