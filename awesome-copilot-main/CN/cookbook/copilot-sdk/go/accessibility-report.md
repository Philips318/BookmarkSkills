#生成无障碍报告

构建一个CLI工具，使用剧作家MCP服务器分析网页可访问性，并通过可选的测试生成生成详细的wcag兼容报告。

> **可运行示例：** [recipe/accessibility-report.go]（recipe/accessibility-report.go）
>
>“bash
>运行recipe/accessibility-report.go> ' ' '

示例场景

您想要审核网站的可访问性遵从性。该工具使用剧作家导航到URL，捕获可访问性快照，并生成包含WCAG标准（如地标、标题层次结构、焦点管理和触摸目标）的结构化报告。它还可以生成剧作家测试文件，以自动执行未来的可访问性检查。

# #先决条件```bash
go get github.com/github/copilot-sdk/go
```
您还需要为剧作家MCP服务器提供`npx`（已安装Node.js）。

# #使用```bash
go run accessibility-report.go
# Enter a URL when prompted
```
完整示例：accessibility-report.go```go
package main

import (
	"bufio"
	"context"
	"fmt"
	"log"
	"os"
	"strings"

	copilot "github.com/github/copilot-sdk/go"
)

func main() {
	ctx := context.Background()
	reader := bufio.NewReader(os.Stdin)

	fmt.Println("=== Accessibility Report Generator ===")
	fmt.Println()

	fmt.Print("Enter URL to analyze: ")
	url, _ := reader.ReadString('\n')
	url = strings.TrimSpace(url)

	if url == "" {
		fmt.Println("No URL provided. Exiting.")
		return
	}

	// Ensure URL has a scheme
	if !strings.HasPrefix(url, "http://") && !strings.HasPrefix(url, "https://") {
		url = "https://" + url
	}

	fmt.Printf("\nAnalyzing: %s\n", url)
	fmt.Println("Please wait...\n")

	// Create Copilot client with Playwright MCP server
	client := copilot.NewClient(nil)

	if err := client.Start(ctx); err != nil {
		log.Fatal(err)
	}
	defer client.Stop()

	session, err := client.CreateSession(ctx, &copilot.SessionConfig{
		OnPermissionRequest: copilot.PermissionHandler.ApproveAll,
		Model:     "claude-opus-4.6",
		Streaming: true,
		MCPServers: map[string]copilot.MCPServerConfig{
			"playwright": {
				"type":    "local",
				"command": "npx",
				"args":    []string{"@playwright/mcp@latest"},
				"tools":   []string{"*"},
			},
		},
	})
	if err != nil {
		log.Fatal(err)
	}
	defer session.Disconnect()

	// Set up streaming event handling
	done := make(chan struct{}, 1)

	session.On(func(event copilot.SessionEvent) {
		switch d := event.Data.(type) {
		case *copilot.AssistantMessageDeltaData:
			fmt.Print(d.DeltaContent)
		case *copilot.SessionIdleData:
			select {
			case done <- struct{}{}:
			default:
			}
		case *copilot.SessionErrorData:
			fmt.Printf("\nError: %s\n", d.Message)
			select {
			case done <- struct{}{}:
			default:
			}
		}
	})

	prompt := fmt.Sprintf(`
    Use the Playwright MCP server to analyze the accessibility of this webpage: %s
    
    Please:
    1. Navigate to the URL using playwright-browser_navigate
    2. Take an accessibility snapshot using playwright-browser_snapshot
    3. Analyze the snapshot and provide a detailed accessibility report
    
    Format the report with emoji indicators:
    - 📊 Accessibility Report header
    - ✅ What's Working Well (table with Category, Status, Details)
    - ⚠️ Issues Found (table with Severity, Issue, WCAG Criterion, Recommendation)
    - 📋 Stats Summary (links, headings, focusable elements, landmarks)
    - ⚙️ Priority Recommendations

    Use ✅ for pass, 🔴 for high severity issues, 🟡 for medium severity, ❌ for missing items.
    Include actual findings from the page analysis.
    `, url)

	if _, err := session.Send(ctx, copilot.MessageOptions{Prompt: prompt}); err != nil {
		log.Fatal(err)
	}
	<-done

	fmt.Println("\n\n=== Report Complete ===\n")

	// Prompt user for test generation
	fmt.Print("Would you like to generate Playwright accessibility tests? (y/n): ")
	generateTests, _ := reader.ReadString('\n')
	generateTests = strings.TrimSpace(strings.ToLower(generateTests))

	if generateTests == "y" || generateTests == "yes" {
		detectLanguagePrompt := `
        Analyze the current working directory to detect the primary programming language.
        Respond with ONLY the detected language name and a brief explanation.
        If no project is detected, suggest "TypeScript" as the default.
        `

		fmt.Println("\nDetecting project language...\n")
		select {
		case <-done:
		default:
		}
		if _, err := session.Send(ctx, copilot.MessageOptions{Prompt: detectLanguagePrompt}); err != nil {
			log.Fatal(err)
		}
		<-done

		fmt.Print("\n\nConfirm language for tests (or enter a different one): ")
		language, _ := reader.ReadString('\n')
		language = strings.TrimSpace(language)
		if language == "" {
			language = "TypeScript"
		}

		testGenerationPrompt := fmt.Sprintf(`
        Based on the accessibility report you just generated for %s,
        create Playwright accessibility tests in %s.
        
        Include tests for: lang attribute, title, heading hierarchy, alt text,
        landmarks, skip navigation, focus indicators, and touch targets.
        Use Playwright's accessibility testing features with helpful comments.
        Output the complete test file.
        `, url, language)

		fmt.Println("\nGenerating accessibility tests...\n")
		select {
		case <-done:
		default:
		}
		if _, err := session.Send(ctx, copilot.MessageOptions{Prompt: testGenerationPrompt}); err != nil {
			log.Fatal(err)
		}
		<-done

		fmt.Println("\n\n=== Tests Generated ===")
	}
}
```
##它是如何工作的

1. **剧作家MCP服务器**：配置运行`@playwright/mcp`的本地MCP服务器，提供浏览器自动化工具
2. **流输出**：使用`Streaming: true`和`AssistantMessageDeltaData`事件进行实时逐令牌输出
3. **可访问性快照**：剧作家的`browser_snapshot`工具捕获页面的完整可访问性树
4. **结构化报告**：提示工程师使用与表情符号严重性指标一致的wcag一致的报告格式
5. **测试生成**：可选地检测项目语言并生成剧作家可访问性测试

##关键概念

MCP服务器配置

该配方配置了一个与会话一起运行的本地MCP服务器：```go
session, err := client.CreateSession(ctx, &copilot.SessionConfig{
	OnPermissionRequest: copilot.PermissionHandler.ApproveAll,
    MCPServers: map[string]copilot.MCPServerConfig{
        "playwright": {
            "type":    "local",
            "command": "npx",
            "args":    []string{"@playwright/mcp@latest"},
            "tools":   []string{"*"},
        },
    },
})
```
这使模型可以访问剧作家浏览器工具，如`browser_navigate`、`browser_snapshot`和`browser_click`。

###流式处理事件

与`SendAndWait`不同，这个配方使用流作为实时输出：```go
session.On(func(event copilot.SessionEvent) {
    switch d := event.Data.(type) {
    case *copilot.AssistantMessageDeltaData:
        fmt.Print(d.DeltaContent)
    case *copilot.SessionIdleData:
        done <- struct{}{}
    }
})
```
##示例交互```
=== Accessibility Report Generator ===

Enter URL to analyze: github.com

Analyzing: https://github.com
Please wait...

📊 Accessibility Report: GitHub (github.com)

✅ What's Working Well
| Category | Status | Details |
|----------|--------|---------|
| Language | ✅ Pass | lang="en" properly set |
| Page Title | ✅ Pass | "GitHub" is recognizable |
| Heading Hierarchy | ✅ Pass | Proper H1/H2 structure |
| Images | ✅ Pass | All images have alt text |

⚠️ Issues Found
| Severity | Issue | WCAG Criterion | Recommendation |
|----------|-------|----------------|----------------|
| 🟡 Medium | Some links lack descriptive text | 2.4.4 | Add aria-label to icon-only links |

📋 Stats Summary
- Total Links: 47
- Total Headings: 8 (1× H1, proper hierarchy)
- Focusable Elements: 52
- Landmarks Found: banner ✅, navigation ✅, main ✅, footer ✅

=== Report Complete ===

Would you like to generate Playwright accessibility tests? (y/n): y

Detecting project language...
TypeScript detected (package.json found)

Confirm language for tests (or enter a different one): 

Generating accessibility tests...
[Generated test file output...]

=== Tests Generated ===
```
