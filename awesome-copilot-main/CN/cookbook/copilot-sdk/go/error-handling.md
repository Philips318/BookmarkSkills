#错误处理模式

在您的Copilot SDK应用程序中优雅地处理错误。

> **可运行示例：** [recipe/error-handling.go]（recipe/error-handling.go）
>
>“bash
>运行recipe/error-handling.go> ' ' '

示例场景

您需要处理各种错误情况，如连接失败、超时和无效响应。

基本错误处理```go
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
        log.Fatalf("Failed to start client: %v", err)
    }
    defer client.Stop()

    session, err := client.CreateSession(ctx, &copilot.SessionConfig{
    	OnPermissionRequest: copilot.PermissionHandler.ApproveAll,
        Model: "gpt-5.4",
    })
    if err != nil {
        log.Fatalf("Failed to create session: %v", err)
    }
    defer session.Disconnect()

    result, err := session.SendAndWait(ctx, copilot.MessageOptions{Prompt: "Hello!"})
    if err != nil {
        log.Printf("Failed to send message: %v", err)
        return
    }

    if result != nil {
        if d, ok := result.Data.(*copilot.AssistantMessageData); ok {
            fmt.Println(d.Content)
        }
    }
}
```
处理特定的错误类型```go
import (
    "context"
    "errors"
    "fmt"
    "os/exec"
    copilot "github.com/github/copilot-sdk/go"
)

func startClient(ctx context.Context) error {
    client := copilot.NewClient(nil)

    if err := client.Start(ctx); err != nil {
        var execErr *exec.Error
        if errors.As(err, &execErr) {
            return fmt.Errorf("Copilot CLI not found. Please install it first: %w", err)
        }
        if errors.Is(err, context.DeadlineExceeded) {
            return fmt.Errorf("Could not connect to Copilot CLI server: %w", err)
        }
        return fmt.Errorf("Unexpected error: %w", err)
    }

    return nil
}
```
##超时处理```go
import (
    "context"
    "errors"
    "fmt"
    "time"
    copilot "github.com/github/copilot-sdk/go"
)

func sendWithTimeout(session *copilot.Session) error {
    ctx, cancel := context.WithTimeout(context.Background(), 30*time.Second)
    defer cancel()

    result, err := session.SendAndWait(ctx, copilot.MessageOptions{Prompt: "Complex question..."})
    if err != nil {
        if errors.Is(err, context.DeadlineExceeded) {
            return fmt.Errorf("request timed out")
        }
        return err
    }

    if result != nil && result.Data.Content != nil {
        fmt.Println(*result.Data.Content)
    }
    return nil
}
```
##终止请求```go
func abortAfterDelay(ctx context.Context, session *copilot.Session) {
    // Start a request (non-blocking send)
    session.Send(ctx, copilot.MessageOptions{Prompt: "Write a very long story..."})

    // Abort it after some condition
    time.AfterFunc(5*time.Second, func() {
        if err := session.Abort(ctx); err != nil {
            log.Printf("Failed to abort: %v", err)
        }
        fmt.Println("Request aborted")
    })
}
```
##安全关机```go
import (
    "context"
    "fmt"
    "log"
    "os"
    "os/signal"
    "syscall"
    copilot "github.com/github/copilot-sdk/go"
)

func main() {
    ctx := context.Background()
    client := copilot.NewClient(nil)

    // Set up signal handling
    sigChan := make(chan os.Signal, 1)
    signal.Notify(sigChan, os.Interrupt, syscall.SIGTERM)

    go func() {
        <-sigChan
        fmt.Println("\nShutting down...")
        client.Stop()
        os.Exit(0)
    }()

    if err := client.Start(ctx); err != nil {
        log.Fatal(err)
    }

    // ... do work ...
}
```
延迟清理模式```go
func doWork() error {
    ctx := context.Background()
    client := copilot.NewClient(nil)

    if err := client.Start(ctx); err != nil {
        return fmt.Errorf("failed to start: %w", err)
    }
    defer client.Stop()

    session, err := client.CreateSession(ctx, &copilot.SessionConfig{
	OnPermissionRequest: copilot.PermissionHandler.ApproveAll,
	Model:               "gpt-5.4",
    })
    if err != nil {
        return fmt.Errorf("failed to create session: %w", err)
    }
    defer session.Disconnect()

    // ... do work ...

    return nil
}
```
最佳实践

1. **总是清理**：使用defer来确保`Stop()`被调用
2. **处理连接错误**:CLI可能未安装或未运行
3. **设置适当的超时时间**：对于长时间运行的请求使用`context.WithTimeout`4. **日志错误**：捕获错误细节以便调试
5. **包装错误**：使用`fmt.Errorf`和`%w`来保存错误链