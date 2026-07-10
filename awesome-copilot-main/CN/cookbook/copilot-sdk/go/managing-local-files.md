#按元数据分组文件

使用Copilot可以根据元数据智能地组织文件夹中的文件。

> **可运行示例：** [recipe/managing-local-files.go]（recipe/managing-local-files.go）
>
>“bash
>运行recipe/managing-local-files.go> ' ' '

示例场景

您有一个包含许多文件的文件夹，并且希望根据元数据（如文件类型、创建日期、大小或其他属性）将它们组织到子文件夹中。Copilot可以分析文件并建议或执行分组策略。

##示例代码```go
package main

import (
    "context"
    "fmt"
    "log"
    "os"
    "path/filepath"
    copilot "github.com/github/copilot-sdk/go"
)

func main() {
    ctx := context.Background()

    // Create and start client
    client := copilot.NewClient(nil)
    if err := client.Start(ctx); err != nil {
        log.Fatal(err)
    }
    defer client.Stop()

    // Create session
    session, err := client.CreateSession(ctx, &copilot.SessionConfig{
    	OnPermissionRequest: copilot.PermissionHandler.ApproveAll,
        Model: "gpt-5.4",
    })
    if err != nil {
        log.Fatal(err)
    }
    defer session.Disconnect()

    // Event handler
    session.On(func(event copilot.SessionEvent) {
        switch d := event.Data.(type) {
        case *copilot.AssistantMessageData:
            fmt.Printf("\nCopilot: %s\n", d.Content)
        case *copilot.ToolExecutionStartData:
            fmt.Printf("  → Running: %s\n", d.ToolName)
        case *copilot.ToolExecutionCompleteData:
            fmt.Printf("  ✓ Completed (success=%v)\n", d.Success)
        }
    })

    // Ask Copilot to organize files
    homeDir, _ := os.UserHomeDir()
    targetFolder := filepath.Join(homeDir, "Downloads")

    prompt := fmt.Sprintf(`
Analyze the files in "%s" and organize them into subfolders.

1. First, list all files and their metadata
2. Preview grouping by file extension
3. Create appropriate subfolders (e.g., "images", "documents", "videos")
4. Move each file to its appropriate subfolder

Please confirm before moving any files.
`, targetFolder)

    _, err = session.SendAndWait(ctx, copilot.MessageOptions{Prompt: prompt})
    if err != nil {
        log.Fatal(err)
    }
}
```
分组策略

###通过文件扩展名```go
// Groups files like:
// images/   -> .jpg, .png, .gif
// documents/ -> .pdf, .docx, .txt
// videos/   -> .mp4, .avi, .mov
```
###按创建日期```go
// Groups files like:
// 2024-01/ -> files created in January 2024
// 2024-02/ -> files created in February 2024
```
###按文件大小```go
// Groups files like:
// tiny-under-1kb/
// small-under-1mb/
// medium-under-100mb/
// large-over-100mb/
```
##干式运行模式

为了安全起见，您可以要求Copilot只预览更改：```go
prompt := fmt.Sprintf(`
Analyze files in "%s" and show me how you would organize them
by file type. DO NOT move any files - just show me the plan.
`, targetFolder)

session.SendAndWait(ctx, copilot.MessageOptions{Prompt: prompt})
```
使用AI分析自定义分组

让Copilot根据文件内容确定最佳分组：```go
prompt := fmt.Sprintf(`
Look at the files in "%s" and suggest a logical organization.
Consider:
- File names and what they might contain
- File types and their typical uses
- Date patterns that might indicate projects or events

Propose folder names that are descriptive and useful.
`, targetFolder)

session.SendAndWait(ctx, copilot.MessageOptions{Prompt: prompt})
```
##安全考虑

1. **移动前确认**：要求副驾驶在执行移动前确认
2. **处理重复**：考虑如果存在同名的文件会发生什么
3. **保存原件**：考虑复制而不是移动重要文件