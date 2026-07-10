---
title: '06 · Connect to GitHub, Databases & APIs'
description: 'Mirror the source chapter on MCP servers and external integrations for GitHub Copilot CLI.'
authors:
  - GitHub Copilot Learning Hub Team
lastUpdated: 2026-06-19
---
！[第六章：MCP服务器]（/images/learning-hub/copilot-cli-for-beginners/06/chapter-header.png）

如果Copilot可以读取你的GitHub问题，检查你的数据库，并创建pr…都是航站楼的吗？**

到目前为止，Copilot只能处理你直接给它的东西：你用`@`引用的文件、对话历史记录和它自己的训练数据。但是，如果它可以自己检查您的GitHub存储库，浏览您的项目文件，或查找库的最新文档呢？

这就是MCP（模型上下文协议）所做的。这是一种将Copilot与外部服务连接起来的方式，这样它就可以访问实时的、真实的数据。副驾驶连接的每个服务都被称为“MCP服务器”。在本章中，你将建立一些这样的连接，看看它们是如何使Copilot变得更加有用的。>💡**已经熟悉MCP了？**[跳到快速开始]（#-use-the-built-in-github-mcp）确认它正在工作并开始配置服务器。

##🎯学习目标

在本章结束时，你将能够：

-了解MCP是什么以及为什么它很重要
—使用`/mcp`命令管理MCP服务器
-为GitHub、文件系统和文档配置MCP服务器
-在图书应用程序项目中使用mcp支持的工作流
-知道何时以及如何构建自定义MCP服务器（可选）

>⏱️**预计时间**:~50分钟（15分钟阅读+ 35分钟动手）

---

##🧩现实世界的类比：浏览器扩展<img src="/images/learning-hub/copilot-cli-for-beginners/06/browser-extensions-analogy.png" alt="MCP Servers are like Browser Extensions" width="800"/>
把MCP服务器想象成浏览器扩展。你的浏览器本身可以显示网页，但扩展连接到额外的服务：

|浏览器扩展|连接| MCP等效||-------------------|---------------------|----------------|
|密码管理器|您的密码库| **GitHub MCP**→您的repos，问题，PRs |
|语法|编写分析服务| **上下文7 MCP**→库文档|
|文件管理器|云存储| **文件系统MCP**→本地项目文件|

没有扩展，您的浏览器仍然有用，但有了它们，它就变得强大了。MCP服务器为Copilot做同样的事情。他们将它连接到真实的、实时的数据源，这样它就可以读取你的GitHub问题，探索你的文件系统，获取最新的文档，等等。

***MCP服务器将Copilot连接到外部世界：GitHub、存储库、文档等等

>💡**关键见解**：没有MCP， Copilot只能看到你显式与`@`共享的文件。有了MCP，它可以主动探索你的项目，检查你的GitHub仓库，并查找文档，所有这些都是自动的。

---<img src="/images/learning-hub/copilot-cli-for-beginners/06/quick-start-mcp.png" alt="Power cable connecting with bright electrical spark surrounded by floating tech icons representing MCP server connections" width="800"/>
快速入门：30秒内完成MCP

##开始使用内置GitHub MCP服务器
在配置任何东西之前，让我们现在看看MCP的作用。
默认情况下包含GitHub MCP服务器。试试这个:```bash
copilot
> List the recent commits in this repository
```
如果Copilot返回真实的提交数据，那么您就看到了MCP的作用。这是GitHub MCP服务器代表你向GitHub伸出援手。但GitHub只是*一个*服务器。本章向您展示如何添加更多（文件系统访问，最新文档等），以便Copilot可以做得更多。

---`/mcp show`命令

使用`/mcp show`查看配置了哪些MCP服务器以及它们是否启用：```bash
copilot

> /mcp show

MCP Servers:
✓ github (enabled) - GitHub integration
✓ filesystem (enabled) - File system access
```
>💡**只看到GitHub服务器？这是意料之中的！如果你还没有添加任何额外的MCP服务器，GitHub是唯一列出的。您将在下一节中添加更多内容。

>📚**想查看所有`/mcp`命令？**还有其他命令用于添加、编辑、启用和删除服务器。请参阅本章末尾的[完整命令参考]（#-additional-mcp-commands）。<details>
<summary>🎬 See it in action!</summary>
！[MCP状态演示]（/images/learning-hub/copilot-cli-for-beginners/06/mcp-status-demo.gif）

*Demo输出不同。您的模型、工具和响应将与此处显示的有所不同</details>
---

MCP有什么变化？

以下是MCP在实践中的不同之处：

没有MCP的* *:* *```bash
> What's in GitHub issue #42?

"I don't have access to GitHub. You'll need to copy and paste the issue content."
```
与MCP * *: * *```bash
> What's in GitHub issue #42 of this repository?

Issue #42: Login fails with special characters
Status: Open
Labels: bug, priority-high
Description: Users report that passwords containing...
```
MCP使副驾驶意识到您的实际开发环境。

>📚**官方文档**:[关于MCP]（https://docs.github.com/copilot/concepts/context/mcp）深入了解MCP如何与GitHub Copilot一起工作。

---

#配置MCP服务器<img src="/images/learning-hub/copilot-cli-for-beginners/06/configuring-mcp-servers.png" alt="Hands adjusting knobs and sliders on a professional audio mixing board representing MCP server configuration" width="800"/>
现在您已经看到了运行中的MCP，让我们设置其他服务器。本节介绍配置文件格式以及如何添加新服务器。

---

MCP配置文件

MCP服务器可以在`~/.copilot/mcp-config.json`中的用户级别进行配置，这适用于各个项目，也可以在`.mcp.json`中的项目级别进行配置，或者在工作空间配置文件`.github/mcp.json`中进行配置。`.github/mcp.json`与`.mcp.json`一起自动加载。如果您使用的是`/mcp search`，那么CLI将创建或更新您的用户级`~/.copilot/mcp-config.json`，但是当您希望自定义或共享项目级MCP配置时，理解JSON格式非常有用。

>⚠️**注**：不再支持`.vscode/mcp.json`作为MCP配置源。如果已有`.vscode/mcp.json`，请将其迁移到项目根目录中的`.mcp.json`。如果CLI检测到旧的配置文件，它将显示一个迁移提示。```json
{
  "mcpServers": {
    "server-name": {
      "type": "local",
      "command": "npx",
      "args": ["@package/server-name"],
      "tools": ["*"]
    }
  }
}
```
*大多数MCP服务器是作为npm包分发的，通过`npx`命令运行<details>
<summary>💡 <strong>New to JSON?</strong> Click here to learn what each field means</summary>
|字段|含义||-------|---------------|
|`"mcpServers"`|所有MCP服务器配置的容器|
|`"server-name"`|您选择的名称（例如，“github”，“filesystem”）
|`"type": "local"`|服务器在您的机器|上运行
|`"command": "npx"`|要运行的程序（npx运行npm包）|
|`"args": [...]`|传递给|命令的参数
|`"tools": ["*"]`|允许来自该服务器|的所有工具

重要的JSON规则：**
对字符串使用双引号`"`（不是单引号）
—最后一项后面不能有逗号
-文件必须是有效的JSON（如果不确定，使用[JSON验证器](https://jsonlint.com/)）</details>
---

添加MCP服务器

GitHub MCP服务器是内置的，不需要设置。以下是您可以添加的其他服务器。**选择您感兴趣的，或按顺序工作

b|我想…|跳到||---|---|
让Copilot浏览我的项目文件| [Filesystem Server](# filessystem - Server) |
获取最新的库文档| [Context7 Server](# Context7 - Server -documentation) |
探索可选的附加功能（自定义服务器，web_fetch） | [Beyond the Basics](# Beyond -the- Basics) |<details>
<summary><strong>Filesystem Server</strong> - Let Copilot explore your project files</summary>
<a id="filesystem-server"></a>
文件系统服务器```json
{
  "mcpServers": {
    "filesystem": {
      "type": "local",
      "command": "npx",
      "args": ["-y", "@modelcontextprotocol/server-filesystem", "."],
      "tools": ["*"]
    }
  }
}
```
>💡**`.`路径**:`.`表示“当前目录”。Copilot可以访问相对于你启动它的位置的文件。在代码空间中，这是您的工作空间根目录。如果愿意，还可以使用像`/workspaces/copilot-cli-for-beginners`这样的绝对路径。

将此添加到您的`~/.copilot/mcp-config.json`并重新启动副驾驶。</details>

<details>
<summary><strong>Context7 Server</strong> - Get up-to-date library docs</summary>
<a id="context7-server-documentation"></a>
### Context7服务器（文档）

Context7使Copilot能够访问流行框架和库的最新文档。它不依赖可能过时的训练数据，而是获取当前的实际文档。```json
{
  "mcpServers": {
    "context7": {
      "type": "local",
      "command": "npx",
      "args": ["-y", "@upstash/context7-mcp"],
      "tools": ["*"]
    }
  }
}
```
-✅**不需要API密钥**
-✅**不需要帐户**
-✅**您的代码保持本地**

将此添加到您的`~/.copilot/mcp-config.json`并重新启动副驾驶。</details>

<details>
<summary><strong>Beyond the Basics</strong> - Custom servers and web access (optional)</summary>
<a id="beyond-the-basics"></a>
当您对上述核心服务器感到满意时，这些都是可选的附加功能。

微软学习MCP服务器

到目前为止，您看到的每个MCP服务器（文件系统、Context7）都在您的机器上本地运行。但MCP服务器也可以远程运行，这意味着你只需将Copilot CLI指向一个URL，它就可以处理其余的事情。没有`npx`或`python`，没有本地进程，没有需要安装的依赖项。

[Microsoft Learn MCP Server]（https://github.com/microsoftdocs/mcp）就是一个很好的例子。它使Copilot CLI可以直接访问微软官方文档（Azure、Microsoft Foundry和其他人工智能主题）。. NET、Microsoft 365等)，因此它可以搜索文档、获取完整页面并找到官方代码示例，而不是依赖于模型的训练数据。

-✅**不需要API密钥**
-✅**不需要帐户**
-✅**无需本地安装**

**快速安装`/plugin install`:**而不是手动编辑JSON配置文件，你可以在一个命令中安装它：```bash
copilot

> /plugin install microsoftdocs/mcp
```
这会自动添加服务器及其关联的代理技能。安装的技能包括：

- **microsoft-docs**：概念、教程和事实查找
- **microsoft-code-reference**: API查找，代码示例和故障排除
- ** Microsoft -skill-creator**：用于生成有关Microsoft技术的自定义技能的元技能

* *用法:* *```bash
copilot

> What's the recommended way to deploy a Python app to Azure App Service? Search Microsoft Learn.
```
📚了解更多：[Microsoft Learn MCP Server概述]（https://learn.microsoft.com/training/support/mcp-get-started）

###使用`web_fetch`访问Web

Copilot CLI包括一个内置的`web_fetch`工具，可以从任何URL获取内容。这对于在不离开终端的情况下导入readme、API文档或发行说明非常有用。不需要MCP服务器。

您可以通过`~/.copilot/config.json`（一般的Copilot设置）控制哪些url是可访问的，它与`~/.copilot/mcp-config.json`（MCP服务器定义）是分开的。```json
{
  "permissions": {
    "allowedUrls": [
      "https://api.github.com/**",
      "https://docs.github.com/**",
      "https://*.npmjs.org/**"
    ],
    "blockedUrls": [
      "http://**"
    ]
  }
}
```
* *用法:* *```bash
copilot

> Fetch and summarize the README from https://github.com/facebook/react
```
构建自定义MCP服务器

想要将Copilot连接到您自己的api、数据库或内部工具？您可以在Python中构建自定义MCP服务器。这是完全可选的，因为预构建的服务器（GitHub、filesystem、Context7）涵盖了大多数用例。

📖请参阅[自定义MCP服务器指南](https://github.com/github/copilot-cli-for-beginners/blob/main/06-mcp-servers/mcp-custom-server.md)，以book应用程序为例进行完整的操作。

📚欲了解更多背景知识，请参阅[MCP初学者课程]（https://github.com/microsoft/mcp-for-beginners）。</details>

<a id="complete-configuration-file"></a>
完成配置文件

下面是一个包含文件系统和Context7服务器的完整`mcp-config.json`：

>💡**注：** GitHub MCP是内置的。您不需要将其添加到配置文件中。```json
{
  "mcpServers": {
    "filesystem": {
      "type": "local",
      "command": "npx",
      "args": ["-y", "@modelcontextprotocol/server-filesystem", "."],
      "tools": ["*"]
    },
    "context7": {
      "type": "local",
      "command": "npx",
      "args": ["-y", "@upstash/context7-mcp"],
      "tools": ["*"]
    }
  }
}
```
将其保存为`~/.copilot/mcp-config.json`用于全局访问，或保存为项目根目录中的`.mcp.json`用于特定于项目的配置。

---

#使用MCP服务器

现在您已经配置了MCP服务器，让我们看看它们可以做什么。<img src="/images/learning-hub/copilot-cli-for-beginners/06/using-mcp-servers.png" alt="Using MCP Servers - Hub-and-spoke diagram showing a Developer CLI connected to GitHub, Filesystem, Context7, and Custom/Web Fetch servers" width="800" />
---

##服务器使用示例

**选择一个服务器进行探索，或按顺序完成它们

我想试试……|跳到||---|---|
GitHub repos， issue和PRs | [GitHub Server](# GitHub - Server -built-in
|[文件系统服务器使用情况](# filessystem - Server - Usage
|库文档查找| [Context7 - Server - Usage](# Context7 - Server - Usage
自定义服务器，Microsoft Learn MCP和web_fetch用法| [Beyond the Basics usage](# Beyond - Basics -usage) |<details>
<summary><strong>GitHub Server (Built-in)</strong> - Access repos, issues, PRs, and more</summary>
<a id="github-server-built-in"></a>
GitHub服务器（内置）

GitHub MCP服务器是**内置的**。如果你登录了Copilot（你在初始设置时登录过），它已经可以工作了。不需要配置！

>💡**不工作？**运行`/login`与GitHub重新认证。<details>
<summary><strong>Authentication in Dev Containers</strong></summary>
—**GitHub Codespaces**（推荐）：自动认证。`gh`CLI继承您的cospace令牌。不需要任何行动。
—**本地开发容器(Docker)**：待容器启动后执行`gh auth login`命令，重启Copilot。

* *故障排除认证:* *```bash
# Check if you're authenticated
gh auth status

# If not, log in
gh auth login

# Verify GitHub MCP is connected
copilot
> /mcp show
```

</details>
|特性|示例||---------|----------|
| **库信息** |查看提交、分支、贡献者|
| **议题** |议题列表、创建、搜索和评论|
| **Pull requests** |查看pr， diffs，创建pr，查看状态|
| **代码搜索** |跨存储库搜索代码|
| **Actions** |查询工作流运行和状态|```bash
copilot

# See recent activity in this repo
> List the last 5 commits in this repository

Recent commits:
1. abc1234 - Update chapter 05 skills examples (2 days ago)
2. def5678 - Add book app test fixtures (3 days ago)
3. ghi9012 - Fix typo in chapter 03 README (4 days ago)
...

# Explore the repo structure
> What branches exist in this repository?

Branches:
- main (default)
- chapter6 (current)

# Search for code patterns across the repo
> Search this repository for files that import pytest

Found 1 file:
- samples/book-app-project/tests/test_books.py
```
>💡**在你自己的分叉上工作？**如果您分叉本课程回购，您也可以尝试编写操作，如创建问题和拉请求。我们将在下面的练习中进行练习。

>⚠️**没有看到结果？** GitHub MCP操作在仓库的远程（在github.com上），而不仅仅是本地文件。确保您的repo有一个远程：运行`git remote -v`检查。</details>

<details>
<summary><strong>Filesystem Server</strong> - Browse and analyze project files</summary>
<a id="filesystem-server-usage"></a>
文件系统服务器

配置后，文件系统MCP提供了Copilot可以自动使用的工具：```bash
copilot

> How many Python files are in the book-app-project directory?

Found 3 Python files in samples/book-app-project/:
- book_app.py
- books.py
- utils.py

> What's the total size of the data.json file?

samples/book-app-project/data.json: 2.4 KB

> Find all functions that don't have type hints in the book app

Found 2 functions without type hints:
- samples/book-app-project/utils.py:10 - get_user_choice()
- samples/book-app-project/utils.py:14 - get_book_details()
```

</details>

<details>
<summary><strong>Context7 Server</strong> - Look up library documentation</summary>
<a id="context7-server-usage"></a>
### Context7服务器```bash
copilot

> What are the best practices for using pytest fixtures?

From pytest Documentation:

Fixtures - Use fixtures to provide a fixed baseline for tests:

    import pytest

    @pytest.fixture
    def sample_books():
        return [
            {"title": "1984", "author": "George Orwell", "year": 1949},
            {"title": "Dune", "author": "Frank Herbert", "year": 1965},
        ]

    def test_find_by_author(sample_books):
        # fixture is automatically passed as argument
        results = [b for b in sample_books if "Orwell" in b["author"]]
        assert len(results) == 1

Best practices:
- Use fixtures instead of setup/teardown methods
- Use tmp_path fixture for temporary files
- Use monkeypatch for modifying environment
- Scope fixtures appropriately (function, class, module, session)

> How can I apply this to the book app's test file?

# Copilot now knows the official pytest patterns
# and can apply them to samples/book-app-project/tests/test_books.py
```

</details>

<details>
<summary><strong>Beyond the Basics</strong> - Custom server and web_fetch usage</summary>
<a id="beyond-the-basics-usage"></a>
超越基础

**自定义MCP服务器**：如果你从[自定义MCP服务器指南]（https://github.com/github/copilot-cli-for-beginners/blob/main/06-mcp-servers/mcp-custom-server.md）建立图书查找服务器，你可以直接查询你的藏书：```bash
copilot

> Look up information about "1984" using the book lookup server. Search for books by George Orwell
```
**Microsoft Learn MCP**：如果您安装了[Microsoft Learn MCP服务器](# Microsoft - Learn - MCP -server)，您可以直接查找微软官方文档：```bash
copilot

> How do I configure managed identity for an Azure Function? Search Microsoft Learn.
```
**Web Fetch**：使用内置的`web_fetch`工具从任何URL提取内容：```bash
copilot

> Fetch and summarize the README from https://github.com/facebook/react
```

</details>
---

多服务器工作流

这些工作流显示了为什么开发人员说“我再也不想没有它工作了”。每个示例在单个会话中组合多个MCP服务器。<img src="/images/learning-hub/copilot-cli-for-beginners/06/issue-to-pr-workflow.png" alt="Issue to PR Workflow using MCP - Shows the complete flow from getting a GitHub issue through creating a pull request" width="800"/>
*完整的MCP工作流程：GitHub MCP检索repo数据，Filesystem MCP查找代码，Context7 MCP提供最佳实践，Copilot处理分析*

下面的每个示例都是独立的。选一本你感兴趣的，或者全部读一遍

b|我想看……|跳到||---|---|
|[多服务器探索](# Multi-Server - Exploration) |
|在一个会话中从issue到PR | [issue-to- PR工作流](#issue-to- PR - Workflow) |
|快速项目健康检查|[健康仪表盘](#health- Dashboard) |<details>
<summary><strong>Multi-Server Exploration</strong> - Combine filesystem, GitHub, and Context7 in one session</summary>
<a id="multi-server-exploration"></a>
####使用多个MCP服务器探索图书应用程序```bash
copilot

# Step 1: Use filesystem MCP to explore the book app
> List all Python files in samples/book-app-project/ and summarize
> what each file does

Found 3 Python files:
- book_app.py: CLI entry point with command routing (list, add, remove, find)
- books.py: BookCollection class with data persistence via JSON
- utils.py: Helper functions for user input and display

# Step 2: Use GitHub MCP to check recent changes
> What were the last 3 commits that touched files in samples/book-app-project/?

Recent commits affecting book app:
1. abc1234 - Add test fixtures for BookCollection (2 days ago)
2. def5678 - Add find_by_author method (5 days ago)
3. ghi9012 - Initial book app setup (1 week ago)

# Step 3: Use Context7 MCP for best practices
> What are Python best practices for JSON data persistence?

From Python Documentation:
- Use context managers (with statements) for file I/O
- Handle JSONDecodeError for corrupted files
- Use dataclasses for structured data
- Consider atomic writes to prevent data corruption

# Step 4: Synthesize a recommendation
> Based on the book app code and these best practices,
> what improvements would you suggest?

Suggestions:
1. Add input validation in add_book() for empty strings and invalid years
2. Consider atomic writes in save_books() to prevent data corruption
3. Add type hints to utils.py functions (get_user_choice, get_book_details)
```

<details>
<summary>🎬 See the MCP workflow in action!</summary>
！[MCP工作流程演示]（/images/learning-hub/copilot-cli-for-beginners/06/mcp-workflow-demo.gif）

*Demo输出不同。您的模型、工具和响应将与此处显示的有所不同</details>
**结果**：代码探索→历史回顾→最佳实践查找→改进计划。**所有从一个终端会话，使用三个MCP服务器在一起</details>

<details>
<summary><strong>Issue-to-PR Workflow</strong> - Go from a GitHub issue to a pull request without leaving the terminal</summary>
<a id="issue-to-pr-workflow"></a>
#### Issue-to-PR工作流（在你自己的Repo上）

这在你自己的分支或有写访问权限的存储库上效果最好：

>💡**如果你现在不能尝试，不要担心。**如果你使用的是只读克隆，你会在作业中练习这个。现在，只要通读一遍就能理解流程。```bash
copilot

> Get the details of GitHub issue #1

Issue #1: Add input validation for book year
Status: Open
Description: The add_book function accepts any year value...

> @samples/book-app-project/books.py Fix the issue described in issue #1

[Copilot implements year validation in add_book()]

> Run the tests to make sure the fix works

All 8 tests passed ✓

> Create a pull request titled "Add year validation to book app"

✓ Created PR #2: Add year validation to book app
```
* *零复制粘贴。零上下文切换。一个终端会话</details>

<details>
<summary><strong>Health Dashboard</strong> - Get a quick project health check using multiple servers</summary>
<a id="health-dashboard"></a>
####图书应用程序健康仪表板```bash
copilot

> Give me a health report for the book app project:
> 1. List all functions across the Python files in samples/book-app-project/
> 2. Check which functions have type hints and which don't
> 3. Show what tests exist in samples/book-app-project/tests/
> 4. Check the recent commit history for this directory

Book App Health Report
======================

📊 Functions Found:
- books.py: 8 methods in BookCollection (all have type hints ✓)
- book_app.py: 6 functions (4 have type hints, 2 missing)
- utils.py: 3 functions (1 has type hints, 2 missing)

🧪 Test Coverage:
- test_books.py: 8 test functions covering BookCollection
- Missing: no tests for book_app.py CLI functions
- Missing: no tests for utils.py helper functions

📝 Recent Activity:
- 3 commits in the last week
- Most recent: added test fixtures

Recommendations:
- Add type hints to utils.py functions
- Add tests for book_app.py CLI handlers
- All files well-sized (<100 lines) - good structure!
```
**结果**：多个数据源在几秒钟内聚合。手动地，这意味着运行grep、计算行数、检查git日志和浏览测试文件。轻松15分钟以上的工作。</details>
---

#实践<img src="/images/learning-hub/copilot-cli-for-beginners/06/practice.png" alt="Warm desk setup with monitor showing code, lamp, coffee cup, and headphones ready for hands-on practice" width="800"/>
**🎉你现在知道要领了！**您了解MCP，您已经看到了如何配置服务器，您已经看到了实际的工作流程。现在是时候自己试试了。

---

##▶️自己试试

现在轮到你了！完成这些练习，练习使用MCP服务器与图书应用程序项目。

练习1：检查你的MCP状态

首先查看可用的MCP服务器：```bash
copilot

> /mcp show
```
您应该看到GitHub服务器被列为已启用。如果不是，请执行`/login`命令进行身份验证。

---

###练习2：使用文件系统MCP探索Book应用程序

如果你已经配置了文件系统服务器，使用它来探索图书应用程序：```bash
copilot

> How many Python files are in samples/book-app-project/?
> What functions are defined in each file?
```
**预期结果**：副驾驶列出`book_app.py`、`books.py`、`utils.py`的功能。

>💡**还没有配置文件系统MCP ？**从上面的[Complete Configuration]（# Complete - Configuration -file）部分创建配置文件。然后重启副驾驶。

---

###练习3：使用GitHub MCP查询存储库历史

使用内置的GitHub MCP来探索这个课程存储库：```bash
copilot

> List the last 5 commits in this repository

> What branches exist in this repository?
```
**预期结果**:Copilot显示最近的提交消息和分支名称从GitHub远程。

>⚠️**在一个代码空间？**自动工作。身份验证是继承的。如果您使用的是本地克隆，请确保`gh auth status`显示您已登录。

---

练习4：组合多个MCP服务器

现在将文件系统和GitHub MCP合并到一个会话中：```bash
copilot

> Read samples/book-app-project/data.json and tell me what books are
> in the collection. Then check the recent commits to see when this
> file was last modified.
```
**预期结果**:Copilot读取JSON文件（文件系统MCP），列出5本书包括“霍比特人”，“1984”，“沙丘”，“杀死一只知更鸟”，“神秘的书”，然后查询GitHub提交历史。

**自检**：当你能解释为什么“检查我的repo的提交历史”比手动运行`git log`并将输出粘贴到提示符中更好时，你就理解了MCP。

---

##📝作业

主要挑战：Book App MCP探索

在图书应用程序项目中练习一起使用MCP服务器。完成这些步骤在一个单一的副驾驶会话：1. **验证MCP是否工作**：执行`/mcp show`命令，确认至少启用了GitHub服务器
2. **设置文件系统MCP**（如果尚未完成）：使用文件系统服务器配置创建`~/.copilot/mcp-config.json`3. **探索代码**：要求Copilot使用文件系统服务器来：
—列出“`samples/book-app-project/books.py`”中的所有功能
-检查`samples/book-app-project/utils.py`中哪些函数缺少类型提示
读取`samples/book-app-project/data.json`并识别任何数据质量问题（提示：查看最后一个条目）
4. **检查存储库活动**：要求Copilot使用GitHub MCP来：
-列出最近提交的文件在`samples/book-app-project/`-检查是否有任何未解决的问题或撤回请求
5. **合并服务器**：在一个提示中，要求副驾驶：
—在`samples/book-app-project/tests/test_books.py`读取测试文件
—将测试的函数与`books.py`中的所有函数进行比较
-总结测试覆盖率缺失的内容**成功标准**：您可以在单个Copilot会话中无缝地结合文件系统和GitHub MCP数据，并且您可以解释每个MCP服务器对响应的贡献。<details>
<summary>💡 Hints (click to expand)</summary>
**步骤1：验证MCP**```bash
copilot
> /mcp show
# Should show "github" as enabled
# If not, run: /login
```
**步骤2：创建配置文件**

使用上面[Complete Configuration]（# Complete - Configuration -file）一节中的JSON，并将其保存为`~/.copilot/mcp-config.json`。

**步骤3：数据质量问题寻找**`data.json`的最后一本书是：```json
{
  "title": "Mysterious Book",
  "author": "",
  "year": 0,
  "read": false
}
```
作者为空，年份为0。这就是数据质量问题！

步骤5：测试覆盖率比较`test_books.py`中的测试包括：`add_book`、`mark_as_read`、`remove_book`、`get_unread_books`和`find_book_by_title`。像`load_books`、`save_books`和`list_books`这样的函数没有直接测试。`book_app.py`中的CLI函数和`utils.py`中的帮助程序根本没有测试。

**如果MCP不工作：**编辑配置文件后重新启动副驾驶。</details>
奖励挑战：建造一个自定义MCP服务器

准备好更深入了吗？遵循[自定义MCP服务器指南]（https://github.com/github/copilot-cli-for-beginners/blob/main/06-mcp-servers/mcp-custom-server.md）在Python中构建自己的MCP服务器，连接到任何API。

---<details>
<summary>🔧 <strong>Common Mistakes & Troubleshooting</strong> (click to expand)</summary>
常见错误

|错误|发生了什么|修复||---------|--------------|-----|
|不知道GitHub MCP是内置的|试图手动install/configure| GitHub MCP默认包含。试试：“List the recent commits in this repo” |
|找不到或编辑MCP设置|用户级配置在`~/.copilot/mcp-config.json`，项目级是`.mcp.json`或`.github/mcp.json`|
|配置文件JSON无效| MCP服务器加载失败|使用`/mcp show`检查配置；验证JSON语法|
|忘记认证MCP服务器|“认证失败”错误|部分MCP需要单独认证。检查每个服务器的需求|

# # #故障排除

**“MCP服务器未找到”** -检查：
1. 存在npm包：`npm view @modelcontextprotocol/server-github`2. 您的配置是有效的JSON
3. 服务器名称与您的配置匹配

使用`/mcp show`查看当前配置。

**“GitHub认证失败”** -内置GitHub MCP使用您的`/login`凭据。试一试:```bash
copilot
> /login
```
这将使用GitHub重新验证您。如果问题仍然存在，请检查您的GitHub帐户是否具有您正在访问的存储库的必要权限。

**"MCP server failed to start"** -查看服务器日志：```bash
# Run the server command manually to see errors
npx -y @modelcontextprotocol/server-github
```
**MCP工具不可用** -确保服务器已启用：```bash
copilot

> /mcp show
# Check if server is listed and enabled
```
如果服务器被禁用，请参阅下面的[additional`/mcp`命令]（#-additional-mcp-commands）了解如何重新启用它。</details>

---

<details>
<summary>📚 <strong>Additional <code>/mcp</code> Commands</strong> (click to expand)</summary>
<a id="-additional-mcp-commands"></a>
除了`/mcp show`，还有其他几个用于管理MCP服务器的命令：

|命令|功能||---------|--------------|
|`/mcp show`|显示所有已配置的MCP服务器及其状态|
|`/mcp add`|添加新服务器|的交互式设置
|`/mcp edit <server-name>`|编辑现有服务器配置|
|`/mcp enable <server-name>`|启用已禁用的服务器|
|`/mcp disable <server-name>`|暂时禁用服务器|
|`/mcp delete <server-name>`|永久移除服务器|

对于本课程的大部分内容，`/mcp show`是您所需要的。随着时间的推移，当您管理更多服务器时，其他命令将变得有用。</details>
---

#总结

##🔑关键要点

1. **MCP**将Copilot连接到外部服务（GitHub，文件系统，文档）
2. **GitHub MCP是内置的** -不需要配置，只是`/login`3. **文件系统和上下文7**通过`~/.copilot/mcp-config.json`配置
4. **多服务器工作流**将来自多个来源的数据合并到一个会话中
5. **使用`/mcp show`检查服务器状态**（可用于管理服务器的附加命令）
6. **自定义服务器**让你连接任何API（可选，在附录指南中涵盖）

>📋**快速参考**：请参阅[GitHub CopilotCLI命令参考]（https://docs.github.com/en/copilot/reference/cli-command-reference）获得完整的命令和快捷方式列表。

---

##➡️下一步是什么

现在您拥有了所有的构建块：模式、上下文、工作流、代理、技能和MCP。是时候把它们放在一起了。

在**[Chapter 07: put It All Together](../07-putting-it-all-together/)**中，你将学到：—将座席、技能和MCP统一在工作流中
-完成功能开发，从想法到合并的PR
-带有挂钩的自动化
-团队环境的最佳实践

---