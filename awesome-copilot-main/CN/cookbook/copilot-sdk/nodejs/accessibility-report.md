#生成无障碍报告

构建一个CLI工具，使用剧作家MCP服务器分析网页可访问性，并通过可选的测试生成生成详细的wcag兼容报告。

> **可运行示例：** [recipe/accessibility-report.ts]（recipe/accessibility-report.ts）
>
>“bash
> CD配方&& NPM安装
> NPX TSXaccessibility-report.ts> #或：NPM运行可访问性报告
> ' ' '

示例场景

您想要审核网站的可访问性遵从性。该工具使用剧作家导航到URL，捕获可访问性快照，并生成包含WCAG标准（如地标、标题层次结构、焦点管理和触摸目标）的结构化报告。它还可以生成剧作家测试文件，以自动执行未来的可访问性检查。

# #先决条件```bash
npm install @github/copilot-sdk
npm install -D typescript tsx @types/node
```
您还需要为剧作家MCP服务器提供`npx`（已安装Node.js）。

# #使用```bash
npx tsx accessibility-report.ts
# Enter a URL when prompted
```
完整示例：accessibility-report.ts```typescript
#!/usr/bin/env npx tsx

import { CopilotClient, approveAll } from "@github/copilot-sdk";
import * as readline from "node:readline";

// ============================================================================
// Main Application
// ============================================================================

async function main() {
    console.log("=== Accessibility Report Generator ===\n");

    const rl = readline.createInterface({
        input: process.stdin,
        output: process.stdout,
    });

    const askQuestion = (query: string): Promise<string> =>
        new Promise((resolve) => rl.question(query, (answer) => resolve(answer.trim())));

    let url = await askQuestion("Enter URL to analyze: ");

    if (!url) {
        console.log("No URL provided. Exiting.");
        rl.close();
        return;
    }

    // Ensure URL has a scheme
    if (!url.startsWith("http://") && !url.startsWith("https://")) {
        url = "https://" + url;
    }

    console.log(`\nAnalyzing: ${url}`);
    console.log("Please wait...\n");

    // Create Copilot client with Playwright MCP server
    const client = new CopilotClient();

    const session = await client.createSession({
        onPermissionRequest: approveAll,
        model: "claude-opus-4.6",
        streaming: true,
        mcpServers: {
            playwright: {
                type: "local",
                command: "npx",
                args: ["@playwright/mcp@latest"],
                tools: ["*"],
            },
        },
    });

    // Set up streaming event handling
    let idleResolve: (() => void) | null = null;

    session.on((event) => {
        if (event.type === "assistant.message_delta") {
            process.stdout.write(event.data.deltaContent ?? "");
        } else if (event.type === "session.idle") {
            idleResolve?.();
        } else if (event.type === "session.error") {
            console.error(`\nError: ${event.data.message}`);
            idleResolve?.();
        }
    });

    const waitForIdle = (): Promise<void> =>
        new Promise((resolve) => {
            idleResolve = resolve;
        });

    const prompt = `
    Use the Playwright MCP server to analyze the accessibility of this webpage: ${url}
    
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
    `;

    let idle = waitForIdle();
    await session.send({ prompt });
    await idle;

    console.log("\n\n=== Report Complete ===\n");

    // Prompt user for test generation
    const generateTests = await askQuestion(
        "Would you like to generate Playwright accessibility tests? (y/n): "
    );

    if (generateTests.toLowerCase() === "y" || generateTests.toLowerCase() === "yes") {
        const detectLanguagePrompt = `
        Analyze the current working directory to detect the primary programming language.
        Respond with ONLY the detected language name and a brief explanation.
        If no project is detected, suggest "TypeScript" as the default.
        `;

        console.log("\nDetecting project language...\n");
        idle = waitForIdle();
        await session.send({ prompt: detectLanguagePrompt });
        await idle;

        let language = await askQuestion("\n\nConfirm language for tests (or enter a different one): ");
        if (!language) language = "TypeScript";

        const testGenerationPrompt = `
        Based on the accessibility report you just generated for ${url},
        create Playwright accessibility tests in ${language}.
        
        Include tests for: lang attribute, title, heading hierarchy, alt text,
        landmarks, skip navigation, focus indicators, and touch targets.
        Use Playwright's accessibility testing features with helpful comments.
        Output the complete test file.
        `;

        console.log("\nGenerating accessibility tests...\n");
        idle = waitForIdle();
        await session.send({ prompt: testGenerationPrompt });
        await idle;

        console.log("\n\n=== Tests Generated ===");
    }

    rl.close();
    await session.destroy();
    await client.stop();
}

main().catch(console.error);
```
##它是如何工作的

1. **剧作家MCP服务器**：配置运行`@playwright/mcp`的本地MCP服务器，提供浏览器自动化工具
2. **流输出**：使用`streaming: true`和`assistant.message_delta`事件进行实时逐令牌输出
3. **可访问性快照**：剧作家的`browser_snapshot`工具捕获页面的完整可访问性树
4. **结构化报告**：提示工程师使用与表情符号严重性指标一致的wcag一致的报告格式
5. **测试生成**：可选地检测项目语言并生成剧作家可访问性测试

##关键概念

MCP服务器配置

该配方配置了一个与会话一起运行的本地MCP服务器：```typescript
const session = await client.createSession({
    onPermissionRequest: approveAll,
    mcpServers: {
        playwright: {
            type: "local",
            command: "npx",
            args: ["@playwright/mcp@latest"],
            tools: ["*"],
        },
    },
});
```
这使模型可以访问剧作家浏览器工具，如`browser_navigate`、`browser_snapshot`和`browser_click`。

###流式处理事件

与`sendAndWait`不同，这个配方使用流作为实时输出：```typescript
session.on((event) => {
    if (event.type === "assistant.message_delta") {
        process.stdout.write(event.data.deltaContent ?? "");
    } else if (event.type === "session.idle") {
        idleResolve?.();
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
