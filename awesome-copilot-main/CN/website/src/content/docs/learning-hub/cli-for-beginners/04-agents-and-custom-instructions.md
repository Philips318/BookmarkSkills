---
title: '04 · Create Specialized AI Assistants'
description: 'Mirror the source chapter on custom agents and custom instructions for GitHub Copilot CLI.'
authors:
  - GitHub Copilot Learning Hub Team
lastUpdated: 2026-06-19
---
！[第04章：代理人及海关指示]（/images/learning-hub/copilot-cli-for-beginners/04/chapter-header.png）

b> **如果你可以雇佣一个Python代码审查员、测试专家和安全审查员……所有在一个工具？**

在第03章中，你掌握了基本的工作流程：代码审查、重构、调试、测试生成和git集成。这些使您在使用GitHub CopilotCLI时非常高效。现在，让我们更进一步。

到目前为止，您一直在使用Copilot CLI作为通用助手。代理允许您赋予它一个具有内置标准的特定角色，比如强制执行类型提示和PEP 8的代码审查器，或者编写pytest用例的测试助手。您将看到，当由具有目标指令的代理处理相同的提示时，如何获得明显更好的结果。

##🎯学习目标

在本章结束时，你将能够：-使用内置代理：计划（`/plan`），代码审查（`/review`），并了解自动代理（探索，任务）
-使用代理文件创建专门的代理（`.agent.md`）
—使用代理完成特定于域的任务
—使用`/agent`和`--agent`切换代理
为项目特定标准编写自定义指令文件

>⏱️**预计时间**:~55分钟（20分钟阅读+ 35分钟动手）

---

##🧩现实世界的类比：招聘专家

当你需要帮忙收拾房子时，你不会叫一个“一般帮手”。你打电话给专家：

|问题|专家|为什么||---------|------------|-----|
|管道漏水|水管工|了解管道规范，有专业工具|
电工|了解安全要求，直至规范|
|新屋顶|屋顶工人|了解材料，当地天气考虑|

代理也是这样工作的。不要使用通用的人工智能，而是使用专注于特定任务并知道正确流程的代理。设置一次指令，然后在需要代码审查、测试、安全性和文档时重用它们。<img src="/images/learning-hub/copilot-cli-for-beginners/04/hiring-specialists-analogy.png" alt="Hiring Specialists Analogy - Just as you call specialized tradespeople for house repairs, AI agents are specialized for specific tasks like code review, testing, security, and documentation" width="800" />
---

#使用代理

立即开始使用内置和自定义代理。

---

## *代理新手？*从这里开始！
从未使用或制作代理？这是你开始学习这门课程所需要知道的。

1. **现在试试*内置的*代理：**   ```bash
   copilot
   > /plan Add input validation for book year in the book app
   ```
这将调用Plan代理来创建一步一步的实现计划。

2. **定义代理的指令很简单，请查看我们提供的[python-reviewer.agent.md]（https://github.com/github/copilot-cli-for-beginners/blob/main/.github/agents/python-reviewer.agent.md）文件以查看模式。

3. **了解核心概念：**代理就像咨询专家而不是通才。“前端代理”将自动关注可访问性和组件模式，您不必提醒它，因为它已经在代理的指令中指定了。


内置代理

**你已经在第03章开发流程中使用了一些内置代理！**<br>`/plan`和`/review`实际上是内置代理。现在你知道发生了什么了吧。以下是完整的清单：

|代理|如何调用|它做什么||-------|---------------|--------------|
| **Plan** |`/plan`或`Shift+Tab`（循环模式）|在编码|之前创建分步执行计划
| **代码审查** |`/review`|审查staged/unstaged更改，并提供有针对性的、可操作的反馈|
| **Init** |`/init`|生成项目配置文件（指令、代理）|
| **探索** | *自动* |内部使用，当你要求副驾驶探索或分析代码库|
| **任务** | *自动* |执行命令，如测试，构建，检查和依赖安装|<br>
**内置代理在运行** -调用计划，代码审查，探索和任务的例子```bash
copilot

# Invoke the Plan agent to create an implementation plan
> /plan Add input validation for book year in the book app

# Invoke the Code-review agent on your changes
> /review

# Explore and Task agents are invoked automatically when relevant:
> Run the test suite        # Uses Task agent

> Explore how book data is loaded    # Uses Explore agent
```
那么任务代理呢？它在幕后工作，管理和跟踪正在发生的事情，并以干净清晰的格式报告。

b|结果b|你看到的b||---------|--------------|
|✅**Success** |简要总结（例如，“所有247个测试都通过了”，“构建成功”）|
|❌**Failure** |完整输出，包括堆栈跟踪、编译错误和详细日志|


>📚**官方文档**:[GitHub CopilotCLI Agents]（https://docs.github.com/copilot/how-tos/use-copilot-agents/use-copilot-cli#use-custom-agents）

---

#添加代理到Copilot CLI

您可以简单地定义您自己的代理作为您的工作流程的一部分！定义一次，然后指导！<img src="/images/learning-hub/copilot-cli-for-beginners/04/using-agents.png" alt="Four colorful AI robots standing together, each with different tools representing specialized agent capabilities" width="800"/>
##🗂️添加代理

代理文件是扩展名为`.agent.md`的markdown文件。它们有两部分：YAML前端内容（元数据）和标记指令。

>💡**新的YAML frontmatter？**它是文件顶部的一小块设置，由`---`标记包围。YAML就是`key: value`对。文件的其余部分是常规的降价。

这是一个最小代理：```markdown
---
name: my-reviewer
description: Code reviewer focused on bugs and security issues
---

# Code Reviewer

You are a code reviewer focused on finding bugs and security issues.

When reviewing code, always check for:
- SQL injection vulnerabilities
- Missing error handling
- Hardcoded secrets
```
>💡**必填与可选**：必填`description`字段。其他字段，如`name`、`tools`和`model`是可选的。

在哪里放置代理文件

|位置|范围|最适合||----------|-------|----------|
|`.github/agents/`|特定于项目的|具有项目约定的团队共享代理|
|`~/.copilot/agents/`|全球（所有项目）|您在任何地方使用的个人代理|

**本项目包括[.github/agents/]（https://github.com/github/copilot-cli-for-beginners/tree/main/.github/agents/）文件夹中的示例代理文件**。您可以编写自己的，或者自定义已经提供的。<details>
<summary>📂 See the sample agents in this course</summary>
|文件|描述| . ||------|-------------|
|`hello-world.agent.md`|最小的例子-从这里|开始
|`python-reviewer.agent.md`| Python代码质量审稿人|
|`pytest-helper.agent.md`| Pytest测试专家|```bash
# Or copy one to your personal agents folder (available in every project)
cp .github/agents/python-reviewer.agent.md ~/.copilot/agents/
```
有关更多社区代理，请参见[github/awesome-copilot]（https://github.com/github/awesome-copilot）</details>
##🚀使用自定义代理的两种方式

###交互模式
在交互模式中，使用`/agent`列出代理，并选择要开始使用的代理。
选择一个座席继续对话。```bash
copilot
> /agent
```
若要切换到不同的代理，或返回到默认模式，请再次使用`/agent`命令。

编程模式

直接启动与代理的新会话。```bash
copilot --agent python-reviewer
> Review @samples/book-app-project/books.py
```
>💡**切换代理**：您可以随时通过再次使用`/agent`或`--agent`切换到不同的代理。要回到标准的Copilot CLI体验，使用`/agent`并选择**no agent**。

---

#深入探员<img src="/images/learning-hub/copilot-cli-for-beginners/04/creating-custom-agents.png" alt="Robot being assembled on a workbench surrounded by components and tools representing custom agent creation" width="800"/>
>💡**可选。**内置代理（`/plan`,`/review`）对于大多数工作流来说足够强大。当您需要在整个工作中始终应用的专业知识时，创建自定义代理。

下面的每个主题都是独立的。选择你感兴趣的——你不需要一次读完它们

b|我想…|跳到||---|---|
看看为什么代理打败了通用提示|[专家vs通用]（# Specialist -vs-generic-看到区别）|
| [Working with Multiple agents](# Working with- Multiple agents) |
|组织、命名和共享代理|[组织和共享代理](# Organizing—Sharing -agents) |
|设置始终在线的项目上下文|[配置您的项目为副驾驶](# configuration - Your -project-for- Copilot) |
|查找YAML属性和工具|[代理文件参考](# Agent - File - Reference) |

选择下面的场景展开它。

---<a id="specialist-vs-generic-see-the-difference"></a>
<details>
<summary><strong>Specialist vs Generic: See the Difference</strong> - Why agents produce better output than generic prompts</summary>
专家vs通用：看看区别

这是特工们证明自己价值的地方。注意区别：

没有代理（普通副驾驶）```bash
copilot

> Add a function to search books by year range in the book app
```
* *通用输出* *:```python
def search_by_year_range(books, start_year, end_year):
    results = []
    for book in books:
        if book['year'] >= start_year and book['year'] <= end_year:
            results.append(book)
    return results
```
基本的。作品。但是错过了很多。

---

使用Python审阅器代理```bash
copilot

> /agent
# Select "python-reviewer"

> Add a function to search books by year range in the book app
```
输出* *:* *专家```python
from typing import List, Dict, Any

def search_by_year_range(
    books: List[Dict[str, Any]],
    start_year: int,
    end_year: int
) -> List[Dict[str, Any]]:
    """
    Search for books within a specified year range.

    Args:
        books: List of book dictionaries
        start_year: Starting year (inclusive)
        end_year: Ending year (inclusive)

    Returns:
        List of books published within the year range

    Raises:
        ValueError: If start_year is greater than end_year
        TypeError: If year values are not integers
    """
    if not isinstance(start_year, int) or not isinstance(end_year, int):
        raise TypeError("Year values must be integers")

    if start_year > end_year:
        raise ValueError(f"Start year ({start_year}) cannot be greater than end year ({end_year})")

    return [
        book for book in books
        if isinstance(book.get('year'), int)
        and start_year <= book['year'] <= end_year
    ]
```
** python-reviewer代理自动包含的内容：
-✅对所有参数和返回值进行提示
-✅综合文档字符串与Args/Returns/Raises-✅输入验证与正确的错误处理
-✅列表理解，更好的性能
-✅边缘情况处理（missing/invalid年值）
-✅PEP 8兼容的格式
-✅防御性编程实践

区别：同样的提示，显著更好的输出。探员带来了你都忘了要的专业知识。</details>

---

<a id="working-with-multiple-agents"></a>
<details>
<summary><strong>Working with Multiple Agents</strong> - Combine specialists, switch mid-session, agent-as-tools</summary>
##使用多个代理

真正的力量来自于专家们在一个特性上的合作。

示例：构建一个简单的功能```bash
copilot

> I want to add a "search by year range" feature to the book app

# Use python-reviewer for design
> /agent
# Select "python-reviewer"

> @samples/book-app-project/books.py Design a find_by_year_range method. What's the best approach?

# Switch to pytest-helper for test design
> /agent
# Select "pytest-helper"

> @samples/book-app-project/tests/test_books.py Design test cases for a find_by_year_range method.
> What edge cases should we cover?

# Synthesize both designs
> Create an implementation plan that includes the method implementation and comprehensive tests.
```
关键观点：你是架构师指导专家。他们负责细节，你负责愿景。<details>
<summary>🎬 See it in action!</summary>
！[Python审查器演示]（/images/learning-hub/copilot-cli-for-beginners/04/python-reviewer-demo.gif）

*演示输出不同-您的模型，工具和响应将与此处显示的不同</details>
代理作为工具

配置好代理后，Copilot还可以在执行复杂任务时将其作为工具调用。如果你要求全栈功能，副驾驶可能会自动将某些部件委托给适当的专业代理。</details>

---

<a id="organizing--sharing-agents"></a>
<details>
<summary><strong>Organizing & Sharing Agents</strong> - Naming, file placement, instruction files, and team sharing</summary>
组织和共享代理

命名你的代理

在创建代理文件时，名称很重要。它是您将在`/agent`或`--agent`之后键入的内容，以及您的队友将在代理列表中看到的内容。

|✅好名字|❌避免||--------------|----------|
|`frontend`|`my-agent`|
|`backend-api`|`agent1`|
|`security-reviewer`|`helper`|
|`react-specialist`|`code`|
|`python-backend`|`assistant`|

* *命名约定:* *
—小写+连字符：`my-agent-name.agent.md`—包含域：`frontend`、`backend`、`devops`、`security`-需要时要具体：`react-typescript`vs`frontend`---

与你的团队分享

将代理文件放在`.github/agents/`中，它们是版本控制的。推送到你的仓库，每个团队成员都会自动获得它们。但是代理只是Copilot从项目中读取的一种文件类型。它还支持自动应用于每个会话的指令文件，而不需要运行`/agent`。

可以这样想：代理是你所呼叫的专家，而指令文件是始终有效的团队规则。

###放文件的地方您已经知道了两个主要位置（参见上面的[将代理文件放在哪里](# Where -to-put agent-files)）。使用这个决策树来选择：<img src="/images/learning-hub/copilot-cli-for-beginners/04/agent-file-placement-decision-tree.png" alt="Decision tree for where to put agent files: experimenting → current folder, team use → .github/agents/, everywhere → ~/.copilot/agents/" width="800"/>
**开始简单：**创建一个单一的`*.agent.md`文件在您的项目文件夹。一旦你对它满意，就把它搬到一个固定的地方。

除了代理文件，Copilot还自动读取**项目级指令文件**，不需要`/agent`。参见下面的`AGENTS.md`，`.instructions.md`和`/init`的[配置你的项目为副驾驶]（# configur-your-project-forcopilot）。</details>

---

<a id="configuring-your-project-for-copilot"></a>
<details>
<summary><strong>Configuring Your Project for Copilot</strong> - AGENTS.md, instruction files, and /init setup</summary>
##配置您的项目的副驾驶

代理是您根据需要调用的专家。**项目配置文件**是不同的：Copilot在每次会话中都会自动读取它们，以了解项目的约定、技术堆栈和规则。没有人需要运行`/agent`；上下文对于在repo中工作的每个人都是活跃的。

###快速设置/init

最快的入门方法是让Copilot为您生成配置文件：```bash
copilot
> /init
```
Copilot将扫描您的项目并创建定制的指令文件。之后你可以编辑它们。

指令文件格式

|文件|作用域|注释||------|-------|-------|
|`AGENTS.md`|项目根或嵌套| **跨平台标准** -与Copilot和其他AI助手|一起工作
|`.github/copilot-instructions.md`|项目|GitHub Copilot特定|
|`.github/instructions/*.instructions.md`|项目|颗粒状，特定于主题的说明|
|`~/.copilot/instructions/**/*.instructions.md`|用户（所有项目）|个人说明适用于任何地方，在所有的回购|
|`CLAUDE.md`、`GEMINI.md`|项目根|兼容性支持|

>🎯**刚刚开始？**使用`AGENTS.md`作为项目说明。您可以在以后根据需要探索其他格式。### AGENTS.md
建议使用`AGENTS.md`格式。这是一个开放标准（https://agents.md/），适用于Copilot和其他AI编码工具。将它放在存储库根目录中，Copilot将自动读取它。这个项目自己的[AGENTS.md]（https://github.com/github/copilot-cli-for-beginners/blob/main/AGENTS.md）就是一个工作示例。

典型的`AGENTS.md`描述了您的项目上下文、代码风格、安全需求和测试标准。按照示例文件中的模式编写您自己的代码。

自定义指令文件（.instructions.md）

对于需要更细粒度控制的团队，将指令拆分为特定于主题的文件。每个文件涵盖一个关注点，并自动应用：```
.github/
└── instructions/
    ├── python-standards.instructions.md
    ├── security-checklist.instructions.md
    └── api-design.instructions.md
```
>💡**注**：指令文件适用于任何语言。这个例子使用Python来匹配我们的课程项目，但你可以为TypeScript、Go、Rust或你的团队使用的任何技术创建类似的文件。

**查找社区指令文件**：浏览[github/awesome-copilot]（https://github.com/github/awesome-copilot）的预先制作的指令文件覆盖。. NET、Angular、Azure、Python、Docker以及更多的技术。

禁用自定义指令

如果您需要Copilot忽略所有特定于项目的配置（用于调试或比较行为）：```bash
copilot --no-custom-instructions
```

</details>

---

<a id="agent-file-reference"></a>
<details>
<summary><strong>Agent File Reference</strong> - YAML properties, tool aliases, and complete examples</summary>
代理文件参考

一个更完整的例子

您已经看到了上面的[最小代理格式]（#-add-your-agents）。下面是一个更全面的代理，它使用`tools`属性。创建`~/.copilot/agents/python-reviewer.agent.md`:```markdown
---
name: python-reviewer
description: Python code quality specialist for reviewing Python projects
tools: ["read", "edit", "search", "execute"]
---

# Python Code Reviewer

You are a Python specialist focused on code quality and best practices.

**Your focus areas:**
- Code quality (PEP 8, type hints, docstrings)
- Performance optimization (list comprehensions, generators)
- Error handling (proper exception handling)
- Maintainability (DRY principles, clear naming)

**Code style requirements:**
- Use Python 3.10+ features (dataclasses, type hints, pattern matching)
- Follow PEP 8 naming conventions
- Use context managers for file I/O
- All functions must have type hints and docstrings

**When reviewing code, always check:**
- Missing type hints on function signatures
- Mutable default arguments
- Proper error handling (no bare except)
- Input validation completeness
```
### YAML属性

|属性|必选|描述||----------|----------|-------------|
|`name`|否|显示名称，默认为filename
|`description`| **是** |代理所做的-帮助副驾驶了解何时建议|
|`tools`|否|允许使用的工具列表（省略=所有可用工具）。请参阅下面的工具别名。|
|`target`|否|仅限`vscode`或`github-copilot`|

###工具别名

在`tools`列表中使用这些名称：
-`read`-读取文件内容
-`edit`-编辑文件
-搜索文件（grep/glob）
-`execute`-运行shell命令（也可以是：`shell`，`Bash`）
-`agent`-调用其他自定义代理

>📖**官方文档**:[海关代理配置]（https://docs.github.com/copilot/reference/custom-agents-configuration）
>
>⚠️**VS CodeOnly**:`model`属性（用于选择AI模型）在VS Code中工作，但在GitHub CopilotCLI中不支持。您可以安全地将它包含在跨平台代理文件中。GitHub CopilotCLI将忽略它。

更多代理模板>💡**初学者注意事项**：下面的示例是模板。**用您的项目使用的任何特定技术替换。重要的是代理的结构，而不是具体提到的技术。

本项目包括[.github/agents/]（https://github.com/github/copilot-cli-for-beginners/tree/main/.github/agents/）文件夹中的工作示例：
- [hello-world.agent.md](https://github.com/github/copilot-cli-for-beginners/blob/main/.github/agents/hello-world.agent.md) -最小的例子，从这里开始
- [python-reviewer.agent.md](https://github.com/github/copilot-cli-for-beginners/blob/main/.github/agents/python-reviewer.agent.md) - Python代码质量审查员
- [pytest-helper.agent.md](https://github.com/github/copilot-cli-for-beginners/blob/main/.github/agents/pytest-helper.agent.md) - Pytest测试专家

关于社区代理，请参见[github/awesome-copilot]（https://github.com/github/awesome-copilot）。</details>
---

#实践<img src="/images/learning-hub/copilot-cli-for-beginners/04/practice.png" alt="Warm desk setup with monitor showing code, lamp, coffee cup, and headphones ready for hands-on practice" width="800"/>
创建您自己的代理并查看它们的运行情况。

---

##▶️自己试试```bash

# Create the agents directory (if it doesn't exist)
mkdir -p .github/agents

# Create a code reviewer agent
cat > .github/agents/reviewer.agent.md << 'EOF'
---
name: reviewer
description: Senior code reviewer focused on security and best practices
---

# Code Reviewer Agent

You are a senior code reviewer focused on code quality.

**Review priorities:**
1. Security vulnerabilities
2. Performance issues
3. Maintainability concerns
4. Best practice violations

**Output format:**
Provide issues as a numbered list with severity tags:
[CRITICAL], [HIGH], [MEDIUM], [LOW]
EOF

# Create a documentation agent
cat > .github/agents/documentor.agent.md << 'EOF'
---
name: documentor
description: Technical writer for clear and complete documentation
---

# Documentation Agent

You are a technical writer who creates clear documentation.

**Documentation standards:**
- Start with a one-sentence summary
- Include usage examples
- Document parameters and return values
- Note any gotchas or limitations
EOF

# Now use them
copilot --agent reviewer
> Review @samples/book-app-project/books.py

# Or switch agents
copilot
> /agent
# Select "documentor"
> Document @samples/book-app-project/books.py
```
---

##📝作业

主要挑战：建立一个专门的特工团队

实际示例创建了`reviewer`和`documentor`代理。现在练习为不同的任务创建和使用代理——改进图书应用程序中的数据验证：

1. 创建3个针对图书应用程序的代理文件（`.agent.md`），每个代理一个，放在`.github/agents/`中
2. 你的代理:
**data-validator**：检查`data.json`是否缺少或不正确的数据（空作者，年份=0，缺少字段）
- **error-handler**：检查Python代码中不一致的错误处理，并建议统一的方法
- **doc-writer**：生成或更新文档字符串和README内容
3. 使用图书应用程序上的每个代理：
-`data-validator`→审计`@samples/book-app-project/data.json`-`error-handler`→回顾`@samples/book-app-project/books.py`和`@samples/book-app-project/utils.py`-`doc-writer`→添加文档字符串到`@samples/book-app-project/books.py`4. 协作：使用`error-handler`来识别错误处理的差距，然后使用`doc-writer`来记录改进的方法**成功标准**：你有3个工作代理，产生一致的，高质量的输出，你可以用`/agent`在它们之间切换。<details>
<summary>💡 Hints (click to expand)</summary>
**Starter模板**：在`.github/agents/`中为每个代理创建一个文件；`data-validator.agent.md`:```markdown
---
description: Analyzes JSON data files for missing or malformed entries
---

You analyze JSON data files for missing or malformed entries.

**Focus areas:**
- Empty or missing author fields
- Invalid years (year=0, future years, negative years)
- Missing required fields (title, author, year, read)
- Duplicate entries
```
`error-handler.agent.md`:```markdown
---
description: Reviews Python code for error handling consistency
---

You review Python code for error handling consistency.

**Standards:**
- No bare except clauses
- Use custom exceptions where appropriate
- All file operations use context managers
- Consistent return types for success/failure
```
`doc-writer.agent.md`:```markdown
---
description: Technical writer for clear Python documentation
---

You are a technical writer who creates clear Python documentation.

**Standards:**
- Google-style docstrings
- Include parameter types and return values
- Add usage examples for public methods
- Note any exceptions raised
```
**测试您的代理：**

>💡**注意：**你应该已经有`samples/book-app-project/data.json`在你的本地副本的这个回购。如果没有，请从源代码库下载原始版本：
> [data.json] (https://github.com/github/copilot-cli-for-beginners/blob/main/samples/book-app-project/data.json)```bash
copilot
> /agent
# Select "data-validator" from the list
> @samples/book-app-project/data.json Check for books with empty author fields or invalid years
```
**提示：** YAML前端内容中的`description`字段是代理工作所必需的。</details>
奖励挑战：指令库

您已经构建了可以按需调用的代理。现在试试另一边：Copilot在每次会话中自动读取的**指令文件，不需要`/agent`。

创建一个包含至少3个指令文件的`.github/instructions/`文件夹：
-`python-style.instructions.md`用于强制PEP 8和类型提示约定
-`test-standards.instructions.md`用于在测试文件中强制pytest约定
-`data-quality.instructions.md`用于验证JSON数据条目

测试每个指令文件上的书应用程序代码。

---<details>
<summary>🔧 <strong>Common Mistakes & Troubleshooting</strong> (click to expand)</summary>
常见错误

|错误|发生了什么|修复||---------|--------------|-----|
|代理前文中缺少`description`|代理无法加载或无法发现|总是在YAML前文|中包含`description:`|代理的文件位置错误|当您尝试使用它时找不到代理|放置在`~/.copilot/agents/`（个人）或`.github/agents/`（项目）|
|使用`.md`代替`.agent.md`|文件可能无法识别为代理|将文件命名为`python-reviewer.agent.md`|
|过长的代理提示|可能达到30,000个字符的限制|保持代理定义集中；使用技能的详细说明|

# # #故障排除

**未找到代理** -检查代理文件是否存在于以下位置之一：
——`~/.copilot/agents/`——`.github/agents/`列出可用的代理：```bash
copilot
> /agent
# Shows all available agents
```
**代理不遵循说明** -在提示中明确并添加更多细节到代理定义：
—特定的frameworks/libraries，带有版本号
-团队约定
-示例代码模式

**自定义指令不加载** -在项目中运行`/init`来设置项目特定的指令：```bash
copilot
> /init
```
或者检查它们是否被禁用：```bash
# Don't use --no-custom-instructions if you want them loaded
copilot  # This loads custom instructions by default
```

</details>
---

#总结

##🔑关键要点

1. **内置代理**:`/plan`和`/review`直接调用；探索和任务自动工作
2. **自定义代理**是在`.agent.md`文件中定义的专家
3. **好的代理**有明确的专业知识、标准和输出格式
4. **多代理协作**结合专业知识解决复杂问题
5. **指令文件** (`.instructions.md`)编码团队标准自动应用
6. **一致的输出**来自定义良好的代理指令

>📋**快速参考**：请参阅[GitHub CopilotCLI命令参考]（https://docs.github.com/en/copilot/reference/cli-command-reference）以获得完整的命令和快捷方式列表。

---

##➡️下一步是什么

代理改变了Copilot在代码中处理和采取目标操作的方式。接下来，你将学习**技能** -改变它遵循的步骤。想知道特工和技能有什么不同吗？第05章直接讨论了这个问题。在**[第05章：技能系统](../05-skills/)**中，你将学习：

如何从你的提示中自动触发技能（不需要斜杠命令）
-建立社区技能
-创建自定义技能与SKILL.md文件
—座席、技能、MCP的区别
-何时使用每一个

---