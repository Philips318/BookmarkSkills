#生成无障碍报告

构建一个CLI工具，使用剧作家MCP服务器分析网页可访问性，并通过可选的测试生成生成详细的wcag兼容报告。

> **可运行示例：** [recipe/accessibility_report.py]（recipe/accessibility_report.py）
>
>“bash
CD recipe && PIP install -rrequirements.txt> pythonaccessibility_report.py> ' ' '

示例场景

您想要审核网站的可访问性遵从性。该工具使用剧作家导航到URL，捕获可访问性快照，并生成包含WCAG标准（如地标、标题层次结构、焦点管理和触摸目标）的结构化报告。它还可以生成剧作家测试文件，以自动执行未来的可访问性检查。

# #先决条件```bash
pip install github-copilot-sdk
```
您还需要为剧作家MCP服务器提供`npx`（已安装Node.js）。

# #使用```bash
python accessibility_report.py
# Enter a URL when prompted
```
完整示例：accessibility_report.py```python
#!/usr/bin/env python3

import asyncio
from copilot import (
    CopilotClient,
    SessionConfig,
    MessageOptions,
    SessionEvent,
    PermissionHandler,
)

# ============================================================================
# Main Application
# ============================================================================

async def main():
    print("=== Accessibility Report Generator ===\n")

    url = input("Enter URL to analyze: ").strip()

    if not url:
        print("No URL provided. Exiting.")
        return

    # Ensure URL has a scheme
    if not url.startswith("http://") and not url.startswith("https://"):
        url = "https://" + url

    print(f"\nAnalyzing: {url}")
    print("Please wait...\n")

    # Create Copilot client with Playwright MCP server
    client = CopilotClient()
    await client.start()

    session = await client.create_session(SessionConfig(
        model="claude-opus-4.6",
        streaming=True,
        mcp_servers={
            "playwright": {
                "type": "local",
                "command": "npx",
                "args": ["@playwright/mcp@latest"],
                "tools": ["*"],
            }
        },
        on_permission_request=PermissionHandler.approve_all))

    done = asyncio.Event()

    # Set up streaming event handling
    def handle_event(event: SessionEvent):
        if event.type.value == "assistant.message_delta":
            print(event.data.delta_content or "", end="", flush=True)
        elif event.type.value == "session.idle":
            done.set()
        elif event.type.value == "session.error":
            print(f"\nError: {event.data.message}")
            done.set()

    session.on(handle_event)

    prompt = f"""
    Use the Playwright MCP server to analyze the accessibility of this webpage: {url}
    
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
    """

    await session.send(MessageOptions(prompt=prompt))
    await done.wait()

    print("\n\n=== Report Complete ===\n")

    # Prompt user for test generation
    generate_tests = input(
        "Would you like to generate Playwright accessibility tests? (y/n): "
    ).strip().lower()

    if generate_tests in ("y", "yes"):
        done.clear()

        detect_language_prompt = """
        Analyze the current working directory to detect the primary programming language.
        Respond with ONLY the detected language name and a brief explanation.
        If no project is detected, suggest "TypeScript" as the default.
        """

        print("\nDetecting project language...\n")
        await session.send(MessageOptions(prompt=detect_language_prompt))
        await done.wait()

        language = input(
            "\n\nConfirm language for tests (or enter a different one): "
        ).strip()
        if not language:
            language = "TypeScript"

        done.clear()

        test_generation_prompt = f"""
        Based on the accessibility report you just generated for {url},
        create Playwright accessibility tests in {language}.
        
        Include tests for: lang attribute, title, heading hierarchy, alt text,
        landmarks, skip navigation, focus indicators, and touch targets.
        Use Playwright's accessibility testing features with helpful comments.
        Output the complete test file.
        """

        print("\nGenerating accessibility tests...\n")
        await session.send(MessageOptions(prompt=test_generation_prompt))
        await done.wait()

        print("\n\n=== Tests Generated ===")

    await session.destroy()
    await client.stop()

if __name__ == "__main__":
    asyncio.run(main())
```
##它是如何工作的

1. **剧作家MCP服务器**：配置运行`@playwright/mcp`的本地MCP服务器，提供浏览器自动化工具
2. **流输出**：使用`streaming=True`和`ASSISTANT_MESSAGE_DELTA`事件进行实时逐令牌输出
3. **可访问性快照**：剧作家的`browser_snapshot`工具捕获页面的完整可访问性树
4. **结构化报告**：提示工程师使用与表情符号严重性指标一致的wcag一致的报告格式
5. **测试生成**：可选地检测项目语言并生成剧作家可访问性测试

##关键概念

MCP服务器配置

该配方配置了一个与会话一起运行的本地MCP服务器：```python
session = await client.create_session(SessionConfig(
    mcp_servers={
        "playwright": {
            "type": "local",
            "command": "npx",
            "args": ["@playwright/mcp@latest"],
            "tools": ["*"],
        }
    },
        on_permission_request=PermissionHandler.approve_all))
```
这使模型可以访问剧作家浏览器工具，如`browser_navigate`、`browser_snapshot`和`browser_click`。

###流式处理事件

与`send_and_wait`不同，这个配方使用流作为实时输出：```python
def handle_event(event: SessionEvent):
    if event.type.value == "assistant.message_delta":
        print(event.data.delta_content or "", end="", flush=True)
    elif event.type.value == "session.idle":
        done.set()

session.on(handle_event)
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
