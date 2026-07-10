---
name: scoutqa-test
description: 'This skill should be used when the user asks to "test this website", "run exploratory testing", "check for accessibility issues", "verify the login flow works", "find bugs on this page", or requests automated QA testing. Triggers on web application testing scenarios including smoke tests, accessibility audits, e-commerce flows, and user flow validation using ScoutQA CLI. Use this skill proactively after implementing web application features to verify they work correctly.'
---
# ScoutQA测试技能

使用`scoutqa`CLI对web应用程序执行基于ai的探索性测试。

**将ScoutQA视为一个智能测试伙伴**，它可以自主探索、发现问题并验证功能。将测试委托给多个并行的ScoutQA执行，以在节省时间的同时最大化覆盖率。

何时使用此技能

在以下两种情况下使用此技能：

1. **用户请求测试** -当用户明确要求测试网站或验证功能
2. **主动验证** -在实现web功能后，自动运行测试以验证实现是否正确工作

**主动使用示例：**—完成登录表单后→测试认证流程
-添加表单验证后→验证验证规则和错误处理
-建立结帐流程后→测试端到端购买流程
-修复bug后→验证修复工作，没有破坏其他功能

**最佳实践**：当你完成一个web特性的实现时，在后台主动启动一个ScoutQA测试，以验证它是否有效，同时你继续执行其他任务。

##运行测试

测试工作流程

复制这张清单并跟踪你的进度：

测试进度:

-[]写有明确期望的具体测试提示
-[]在后台执行scoutqa命令
-[]通知用户执行ID和浏览器URL
-[]提取并分析结果

**步骤1：编写具体的测试提示符**

参考下面的“有效提示”部分。

**第二步：执行scoutqa命令****重要**：使用Bash工具的timeout参数（5000ms = 5秒）来捕获执行细节：

在调用Bash工具时，将`timeout: 5000`设置为参数：

这是Bash工具在Claude Code中的内置超时参数（不是Unix的`timeout`命令）
—5秒后，Bash工具返回带有任务ID的控制权，进程继续在后台运行
-这与Unix的`timeout`不同，后者杀死进程-这里的进程继续运行
-前5秒从ScoutQA的输出中捕获执行ID和浏览器URL
-测试继续在ScoutQA的基础设施上远程运行，并执行后台任务```bash
scoutqa --url "https://example.com" --prompt "Your test instructions"
```
在最初的几秒钟内，命令将输出：

- **执行ID**（例如：`019b831d-xxx`）
- **浏览器网址**（例如：`https://app.scoutqa.ai/t/019b831d-xxx`）
-显示测试进度的初始工具调用

在5秒超时之后，Bash工具返回一个任务ID，命令继续在后台运行。您可以在测试运行时处理其他任务。超时只是为了捕获初始输出（执行ID和浏览器URL）—测试将在本地作为后台任务运行，并在ScoutQA的基础架构上远程运行。

**第三步：通知用户执行ID和浏览器URL**

在Bash工具返回任务ID后（在前5秒内捕获了执行细节），通知用户：

- ScoutQA执行ID和浏览器URL，以便他们可以在浏览器中监控进度
—如果需要稍后查看本地命令的回显，则为后台任务ID当您继续其他工作时，测试将继续在后台运行。

**步骤4：提取并分析结果**

完整的格式见下面的“呈现结果”部分。

###命令选项

-`--url`（必选）：要测试的网站URL（支持`localhost`/`127.0.0.1`）
-`--prompt`（必选）：自然语言测试说明
—`--project-id`（可选）：关联项目进行跟踪
—`-v, --verbose`（可选）：显示所有工具调用，包括内部调用

本地测试支持

ScoutQA支持自动测试`localhost`和`127.0.0.1`url——不需要手动设置。```bash
# Seamlessly test a locally running app when you're developing your app
scoutqa --url "http://localhost:3000" --prompt "Test the registration form"
```
###何时使用每个命令

**开始新的测试？**→使用`scoutqa --url --prompt`**验证已知问题？**→使用`scoutqa issue-verify --issue-id <id>`**从执行中找到问题id ？**→使用`scoutqa list-issues --execution-id <id>`**代理需要更多的上下文？**→使用`scoutqa send-message`（参见“跟踪卡住的执行”）

写有效的提示

关注要探索和验证的内容，而不是规定的步骤。ScoutQA自主决定如何测试。

**示例：用户注册流程**```bash
scoutqa --url "https://example.com" --prompt "
Explore the user registration flow. Test form validation edge cases,
verify error handling, and check accessibility compliance.
"
```
**示例：电子商务结账**```bash
scoutqa --url "https://shop.example.com" --prompt "
Test the checkout flow. Verify pricing calculations, cart persistence,
payment options, and mobile responsiveness.
"
```
**示例：运行全面覆盖的并行测试**

通过在一条消息中调用多个Bash工具并行启动多个测试，每个Bash工具的`timeout`参数设置为`5000`（毫秒）：```bash
# Test 1: Authentication & security
scoutqa --url "https://app.example.com" --prompt "
Explore authentication: login/logout, session handling, password reset,
and security edge cases.
"

# Test 2: Core features (runs in parallel)
scoutqa --url "https://app.example.com" --prompt "
Test dashboard and main user workflows. Verify data loading,
CRUD operations, and search functionality.
"

# Test 3: Accessibility (runs in parallel)
scoutqa --url "https://app.example.com" --prompt "
Conduct accessibility audit: WCAG compliance, keyboard navigation,
screen reader support, color contrast.
"
```
**实现**：用三个Bash工具调用发送单个消息。对于每个Bash工具调用，将`timeout`参数设置为`5000`毫秒。5秒后，每个Bash调用返回一个任务ID，而进程继续在后台运行。这将捕获初始输出中每个测试的执行ID和浏览器URL，然后这三个测试继续并行运行（在ScoutQA的基础架构上作为本地和远程后台任务）。

* *重要指南:* *-描述**要测试什么**，而不是**如何测试** （ScoutQA会找出步骤）
-关注目标、边缘情况和关注点
-对不同的测试区域执行多个并行执行
-信任ScoutQA能够自主探索和发现问题
-在调用scoutqa命令时，总是将Bash工具的`timeout`参数设置为`5000`毫秒（这将在5秒后返回控制，而进程仍在后台继续）
—对于并行测试，在一条消息中调用多个Bash工具
—请记住：Bash工具超时≠Unix超时命令（Bash超时在后台继续进程，Unix超时终止进程）

常见测试场景

**部署后冒烟试验：**```bash
scoutqa --url "$URL" --prompt "
Smoke test: verify critical functionality works after deployment.
Check homepage, navigation, login/logout, and key user flows.
"
```
* *易访问性审计:* *```bash
scoutqa --url "$URL" --prompt "
Audit accessibility: WCAG 2.1 AA compliance, keyboard navigation,
screen reader support, color contrast, and semantic HTML.
"
```
电子商务测试:* * * *```bash
scoutqa --url "$URL" --prompt "
Explore e-commerce functionality: product search/filtering,
cart operations, checkout flow, and pricing calculations.
"
```
* * SaaS应用程序:* *```bash
scoutqa --url "$URL" --prompt "
Test SaaS app: authentication, dashboard, CRUD operations,
permissions, and data integrity.
"
```
* *表单验证:* *```bash
scoutqa --url "$URL" --prompt "
Test form validation: edge cases, error handling, required fields,
format validation, and successful submission.
"
```
* *移动响应:* *```bash
scoutqa --url "$URL" --prompt "
Check mobile experience: responsive layout, navigation,
touch interactions, and viewport behavior.
"
```
**已知问题的验证：**```bash
# First, find issue IDs from a previous execution
scoutqa list-issues --execution-id <executionId>

# Then verify the issue (creates a new verification execution automatically)
scoutqa issue-verify --issue-id <issueId>
```
`issue-verify`命令将：

1. 为问题创建一个验证执行
2. 显示执行ID和浏览器URL
3. 实时传输代理的验证进度
4. 显示带有结果链接的完成摘要

**功能验证（实现后）：**```bash
scoutqa --url "$URL" --prompt "
Verify the new [feature name] works correctly. Test core functionality,
edge cases, error handling, and integration with existing features.
"
```
**示例：在编写功能后进行主动测试**

实现用户注册表单后，自动验证它是否有效：```bash
scoutqa --url "http://localhost:3000/register" --prompt "
Test the newly implemented registration form. Verify:
- Form validation (email format, password strength, required fields)
- Error messages display correctly
- Successful registration flow
- Edge cases (duplicate emails, special characters, etc.)
"
```
当实现在上下文中是新鲜的时候，这可以立即捕获问题。

##清单问题

使用`scoutqa list-issues`浏览在前一次执行中发现的问题。这对于查找与`issue-verify`一起使用的问题id很有用。```bash
scoutqa list-issues --execution-id <executionId>
```
* *选择:* *

—`--execution-id`（必选）：执行ID（来自`/t/<executionId>`URL或CLI输出）

* *输出示例:* *```
Showing 3 issues:

🔴 019c-abc1
   Login button unresponsive on mobile
   Severity: critical | Category: usability | Status: open

🟠 019c-abc2
   Missing form validation on email field
   Severity: high | Category: functional | Status: open

🟡 019c-abc3
   Color contrast insufficient on footer links
   Severity: medium | Category: accessibility | Status: resolved
```
##展示结果

立即呈现（开始测试后）

在运行scoutqa命令之后，向用户显示执行细节：```markdown
**ScoutQA Test Started**

Execution ID: `019b831d-xxx`
View Live: https://app.scoutqa.ai/t/019b831d-xxx

The test is running remotely. You can view real-time progress in your browser at the link above while I continue with other tasks.
```
最终结果（完成后）

当执行完成时，使用以下格式来显示结果：```markdown
**ScoutQA Test Results**

Execution ID: `ex_abc123`

**Issues Found:**

[High] Accessibility: Missing alt text on logo image

- Impact: Screen readers cannot describe the logo
- Location: Header navigation

[Medium] Usability: Submit button not visible on mobile viewport

- Impact: Users cannot complete form on mobile devices
- Location: Contact form, bottom of page

[Low] Functional: Search returns no results for valid queries

- Impact: Search feature appears broken
- Location: Main search bar

**Summary:** Found 3 issues across accessibility, usability, and functional categories. See full interactive report with screenshots at the URL above.
```
总是包括:

- **执行ID**（如`ex_abc123`）作为参考
**发现的问题，包括严重性、类别（可访问性、可用性、功能）、影响和位置

跟踪卡住的执行

如果远程代理卡住或需要澄清，请使用`send-message`继续：```bash
# Example: Agent is stuck at login, user provides credentials
scoutqa send-message --execution-id ex_abc123 --prompt "
Use these test credentials: username: testuser@example.com, password: TestPass123
"

# Example: Agent asks which flow to test next
scoutqa send-message --execution-id ex_abc123 --prompt "
Focus on the checkout flow next, skip the wishlist feature
"
```
检查测试结果

ScoutQA测试在ScoutQA的基础设施上远程运行。在用一个短暂的超时开始一个测试以捕获执行ID后：

1. 测试继续远程运行（而不是在后台本地运行）。
2. 你可以马上继续其他工作
3. 要稍后检查结果，请访问测试开始时提供的浏览器URL
4. 或者，使用`scoutqa get-execution --execution-id <id>`通过CLI获取结果

**最佳实践**：通过将Bash工具的`timeout`参数设置为`5000`毫秒开始测试。5秒后，Bash工具返回带有任务ID和执行细节（执行ID和浏览器URL）的控件，同时测试继续在后台运行。然后，您可以继续其他工作，并在需要时在ScoutQA的网站上或通过CLI检查结果。

# #故障排除

|问题|解决方案|| ------------------------------ | ----------------------------------------------------------- |
|`command not found: scoutqa`|安装命令行：`npm i -g @scoutqa/cli@latest`|
|授权过期/未授权|执行`scoutqa auth login`|
|测试挂起或需要输入|使用`scoutqa send-message --execution-id`|
|检查测试结果|访问浏览器URL或`scoutqa get-execution --execution-id`|
|需要issue ID验证|执行`scoutqa list-issues --execution-id <id>`|