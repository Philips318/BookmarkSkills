#生成无障碍报告

构建一个CLI工具，使用剧作家MCP服务器分析网页可访问性，并通过可选的测试生成生成详细的wcag兼容报告。

> **可运行示例：** [recipe/accessibility-report.cs]（recipe/accessibility-report.cs）
>
>“bash
运行recipe/accessibility-report.cs> ' ' '

示例场景

您想要审核网站的可访问性遵从性。该工具使用剧作家导航到URL，捕获可访问性快照，并生成包含WCAG标准（如地标、标题层次结构、焦点管理和触摸目标）的结构化报告。它还可以生成剧作家测试文件，以自动执行未来的可访问性检查。

# #先决条件```bash
dotnet add package GitHub.Copilot.SDK
```
您还需要为剧作家MCP服务器提供`npx`（已安装Node.js）。

# #使用```bash
dotnet run recipe/accessibility-report.cs
# Enter a URL when prompted
```
完整示例：accessibility-report.cs```csharp
#:package GitHub.Copilot.SDK@*

using GitHub.Copilot;

// Create and start client
await using var client = new CopilotClient();
await client.StartAsync();

Console.WriteLine("=== Accessibility Report Generator ===");
Console.WriteLine();

Console.Write("Enter URL to analyze: ");
var url = Console.ReadLine()?.Trim();

if (string.IsNullOrWhiteSpace(url))
{
    Console.WriteLine("No URL provided. Exiting.");
    return;
}

// Ensure URL has a scheme
if (!url.StartsWith("http://") && !url.StartsWith("https://"))
{
    url = "https://" + url;
}

Console.WriteLine($"\nAnalyzing: {url}");
Console.WriteLine("Please wait...\n");

// Create a session with Playwright MCP server
await using var session = await client.CreateSessionAsync(new SessionConfig
{
    Model = "claude-opus-4.6",
    Streaming = true,
    OnPermissionRequest = PermissionHandler.ApproveAll,
    McpServers = new Dictionary<string, McpServerConfig>()
    {
        ["playwright"] =
        new McpStdioServerConfig
        {
            Command = "npx",
            Args = ["@playwright/mcp@latest"],
            Tools = ["*"]
        }
    },
});

// Wait for response using session.idle event
var done = new TaskCompletionSource();

session.On(evt =>
{
    switch (evt)
    {
        case AssistantMessageDeltaEvent delta:
            Console.Write(delta.Data.DeltaContent);
            break;
        case SessionIdleEvent:
            done.TrySetResult();
            break;
        case SessionErrorEvent error:
            Console.WriteLine($"\nError: {error.Data.Message}");
            done.TrySetResult();
            break;
    }
});

var prompt = $"""
    Use the Playwright MCP server to analyze the accessibility of this webpage: {url}
    
    Please:
    1. Navigate to the URL using playwright-browser_navigate
    2. Take an accessibility snapshot using playwright-browser_snapshot
    3. Analyze the snapshot and provide a detailed accessibility report
    
    Format the report EXACTLY like this structure with emoji indicators:

    📊 Accessibility Report: [Page Title] (domain.com)

    ✅ What's Working Well
    | Category | Status | Details |
    |----------|--------|---------|
    | Language | ✅ Pass | lang="en-US" properly set |
    | Page Title | ✅ Pass | "[Title]" is descriptive |
    | Heading Hierarchy | ✅ Pass | Single H1, proper H2/H3 structure |
    | Images | ✅ Pass | All X images have alt text |

    ⚠️ Issues Found
    | Severity | Issue | WCAG Criterion | Recommendation |
    |----------|-------|----------------|----------------|
    | 🔴 High | No <main> landmark | 1.3.1, 2.4.1 | Wrap main content in <main> element |
    | 🟡 Medium | Focus outlines disabled | 2.4.7 | Ensure visible :focus styles exist |

    📋 Stats Summary
    - Total Links: X
    - Total Headings: X
    - Focusable Elements: X
    - Landmarks Found: banner ✅, navigation ✅, main ❌, footer ✅

    ⚙️ Priority Recommendations
    ...

    Use ✅ for pass, 🔴 for high severity issues, 🟡 for medium severity, ❌ for missing items.
    Include actual findings from the page analysis - don't just copy the example.
    """;

await session.SendAsync(new MessageOptions { Prompt = prompt });
await done.Task;

Console.WriteLine("\n\n=== Report Complete ===\n");

// Prompt user for test generation
Console.Write("Would you like to generate Playwright accessibility tests? (y/n): ");
var generateTests = Console.ReadLine()?.Trim().ToLowerInvariant();

if (generateTests == "y" || generateTests == "yes")
{
    // Reset for next interaction
    done = new TaskCompletionSource();

    var detectLanguagePrompt = $"""
        Analyze the current working directory to detect the primary programming language used in this project.
        Respond with ONLY the detected language name and a brief explanation.
        If no project is detected, suggest "TypeScript" as the default for Playwright tests.
        """;

    Console.WriteLine("\nDetecting project language...\n");
    await session.SendAsync(new MessageOptions { Prompt = detectLanguagePrompt });
    await done.Task;

    Console.Write("\n\nConfirm language for tests (or enter a different one): ");
    var language = Console.ReadLine()?.Trim();

    if (string.IsNullOrWhiteSpace(language))
    {
        language = "TypeScript";
    }

    // Reset for test generation
    done = new TaskCompletionSource();

    var testGenerationPrompt = $"""
        Based on the accessibility report you just generated for {url}, create Playwright accessibility tests in {language}.
        
        The tests should:
        1. Verify all the accessibility checks from the report
        2. Test for the issues that were found (to ensure they get fixed)
        3. Include tests for landmarks, heading hierarchy, alt text, focus indicators, and more
        4. Use Playwright's accessibility testing features
        5. Include helpful comments explaining each test
        
        Output the complete test file that can be saved and run.
        """;

    Console.WriteLine("\nGenerating accessibility tests...\n");
    await session.SendAsync(new MessageOptions { Prompt = testGenerationPrompt });
    await done.Task;

    Console.WriteLine("\n\n=== Tests Generated ===");
}
```
##它是如何工作的

1. **剧作家MCP服务器**：配置一个运行`@playwright/mcp`的本地工作室MCP服务器（`McpStdioServerConfig`，通过`npx`启动），提供浏览器自动化工具
2. **流输出**：使用`Streaming = true`和`AssistantMessageDeltaEvent`进行实时token-by-token输出
3. 可访问性快照：剧作家的`browser_snapshot`工具捕获页面的完整可访问性树
4. **结构化报告**：提示工程师使用与表情符号严重性指标一致的wcag一致的报告格式
5. **测试生成**：可选地检测项目语言并生成剧作家可访问性测试

##关键概念

MCP服务器配置

该配方配置了一个与会话一起运行的本地studio MCP服务器（`McpStdioServerConfig`，通过`npx`启动）：```csharp
OnPermissionRequest = PermissionHandler.ApproveAll,
McpServers = new Dictionary<string, McpServerConfig>()
{
    ["playwright"] = new McpStdioServerConfig
    {
        Command = "npx",
        Args = ["@playwright/mcp@latest"],
        Tools = ["*"]
    }
}
```
这使模型可以访问剧作家浏览器工具，如`browser_navigate`、`browser_snapshot`和`browser_click`。

###流式处理事件

与`SendAndWaitAsync`不同，这个配方使用流作为实时输出：```csharp
session.On(evt =>
{
    switch (evt)
    {
        case AssistantMessageDeltaEvent delta:
            Console.Write(delta.Data.DeltaContent); // Token-by-token
            break;
        case SessionIdleEvent:
            done.TrySetResult(); // Model finished
            break;
    }
});
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
