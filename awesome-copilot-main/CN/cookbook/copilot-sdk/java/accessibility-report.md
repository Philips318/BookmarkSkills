#生成无障碍报告

构建一个CLI工具，使用剧作家MCP服务器分析网页可访问性，并通过可选的测试生成生成详细的wcag兼容报告。

> **可运行示例：** [recipe/AccessibilityReport.java]（recipe/AccessibilityReport.java）
>
>“bash
> jbangrecipe/AccessibilityReport.java> ' ' '

示例场景

您想要审核网站的可访问性遵从性。该工具使用剧作家导航到URL，捕获可访问性快照，并生成包含WCAG标准（如地标、标题层次结构、焦点管理和触摸目标）的结构化报告。它还可以生成剧作家测试文件，以自动执行未来的可访问性检查。

# #先决条件

为剧作家MCP服务器安装[JBang]（https://www.jbang.dev/）并确保`npx`可用（Node.js安装）：```bash
# macOS (using Homebrew)
brew install jbangdev/tap/jbang

# Verify npx is available (needed for Playwright MCP)
npx --version
```
# #使用```bash
jbang recipe/AccessibilityReport.java
# Enter a URL when prompted
```
完整示例：AccessibilityReport.java```java
///usr/bin/env jbang "$0" "$@" ; exit $?
//DEPS com.github:copilot-sdk-java:0.2.1-java.1

import com.github.copilot.sdk.*;
import com.github.copilot.sdk.events.*;
import com.github.copilot.sdk.json.*;
import java.io.*;
import java.util.*;
import java.util.concurrent.*;

public class AccessibilityReport {
    public static void main(String[] args) throws Exception {
        System.out.println("=== Accessibility Report Generator ===\n");

        var reader = new BufferedReader(new InputStreamReader(System.in));

        System.out.print("Enter URL to analyze: ");
        String url = reader.readLine().trim();
        if (url.isEmpty()) {
            System.out.println("No URL provided. Exiting.");
            return;
        }
        if (!url.startsWith("http://") && !url.startsWith("https://")) {
            url = "https://" + url;
        }

        System.out.printf("%nAnalyzing: %s%n", url);
        System.out.println("Please wait...\n");

        try (var client = new CopilotClient()) {
            client.start().get();

            // Configure Playwright MCP server for browser automation
            Map<String, Object> mcpConfig = Map.of(
                "type", "local",
                "command", "npx",
                "args", List.of("@playwright/mcp@latest"),
                "tools", List.of("*")
            );

            var session = client.createSession(
                new SessionConfig()
                    .setOnPermissionRequest(PermissionHandler.APPROVE_ALL)
                    .setModel("claude-opus-4.6")
                    .setStreaming(true)
                    .setMcpServers(Map.of("playwright", mcpConfig))
            ).get();

            // Stream output token-by-token
            var idleLatch = new CountDownLatch(1);

            session.on(AssistantMessageDeltaEvent.class,
                ev -> System.out.print(ev.getData().deltaContent()));

            session.on(SessionIdleEvent.class,
                ev -> idleLatch.countDown());

            session.on(SessionErrorEvent.class, ev -> {
                System.err.printf("%nError: %s%n", ev.getData().message());
                idleLatch.countDown();
            });

            String prompt = """
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
                """.formatted(url);

            session.send(new MessageOptions().setPrompt(prompt));
            idleLatch.await();

            System.out.println("\n\n=== Report Complete ===\n");

            // Prompt user for test generation
            System.out.print("Would you like to generate Playwright accessibility tests? (y/n): ");
            String generateTests = reader.readLine().trim();

            if (generateTests.equalsIgnoreCase("y") || generateTests.equalsIgnoreCase("yes")) {
                var testLatch = new CountDownLatch(1);

                session.on(SessionIdleEvent.class,
                    ev -> testLatch.countDown());

                String testPrompt = """
                    Based on the accessibility report you just generated for %s,
                    create Playwright accessibility tests in Java.

                    Include tests for: lang attribute, title, heading hierarchy, alt text,
                    landmarks, skip navigation, focus indicators, and touch targets.
                    Use Playwright's accessibility testing features with helpful comments.
                    Output the complete test file.
                    """.formatted(url);

                System.out.println("\nGenerating accessibility tests...\n");
                session.send(new MessageOptions().setPrompt(testPrompt));
                testLatch.await();

                System.out.println("\n\n=== Tests Generated ===");
            }

            session.close();
        }
    }
}
```
##它是如何工作的

1. **剧作家MCP服务器**：配置运行`@playwright/mcp`的本地MCP服务器，提供浏览器自动化工具
2. **流输出**：使用`streaming: true`和`AssistantMessageDeltaEvent`进行实时逐令牌输出
3. **可访问性快照**：剧作家的`browser_snapshot`工具捕获页面的完整可访问性树
4. **结构化报告**：提示工程师使用与表情符号严重性指标一致的wcag一致的报告格式
5. **测试生成**：根据分析可选择生成剧作家可访问性测试

##关键概念

MCP服务器配置

该配方配置了一个与会话一起运行的本地MCP服务器：```java
Map<String, Object> mcpConfig = Map.of(
    "type", "local",
    "command", "npx",
    "args", List.of("@playwright/mcp@latest"),
    "tools", List.of("*")
);

var session = client.createSession(
    new SessionConfig()
        .setOnPermissionRequest(PermissionHandler.APPROVE_ALL)
        .setMcpServers(Map.of("playwright", mcpConfig))
).get();
```
这使模型可以访问剧作家浏览器工具，如`browser_navigate`、`browser_snapshot`和`browser_click`。

###流式处理事件

与`sendAndWait`不同，这个配方使用流作为实时输出：```java
session.on(AssistantMessageDeltaEvent.class,
    ev -> System.out.print(ev.getData().deltaContent()));

session.on(SessionIdleEvent.class,
    ev -> idleLatch.countDown());
```
`CountDownLatch`将主线程与异步事件流同步—当会话变为空闲时，锁存器释放，程序继续执行。

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

Generating accessibility tests...
[Generated test file output...]

=== Tests Generated ===
```
