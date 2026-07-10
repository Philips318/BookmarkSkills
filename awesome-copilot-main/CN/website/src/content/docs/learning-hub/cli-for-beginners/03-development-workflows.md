---
title: '03 · Development Workflows'
description: 'Mirror the source development workflow chapter covering review, debugging, testing, and git support.'
authors:
  - GitHub Copilot Learning Hub Team
lastUpdated: 2026-07-03
---
！[第三章：开发工作流程]（/images/learning-hub/copilot-cli-for-beginners/03/chapter-header.png）

如果AI能够发现你甚至不知道该问的漏洞会怎样？**

在本章中，GitHub CopilotCLI将成为您的日常驱动程序。您将在每天已经依赖的工作流中使用它：测试、重构、调试和Git。

##🎯学习目标

在本章结束时，你将能够：

-运行全面的代码审查与Copilot CLI
安全地重构遗留代码
-调试AI辅助的问题
-自动生成测试
-集成Copilot CLI与您的git工作流

>⏱️**预计时间**:~60分钟（15分钟阅读+ 45分钟动手）

---

##🧩现实世界的类比：木匠的工作流程

木匠不仅知道如何使用工具，他们还有不同工作的“工作流程”：<img src="/images/learning-hub/copilot-cli-for-beginners/03/carpenter-workflow-steps.png" alt="Craftsman workshop showing three workflow lanes: Building Furniture (Measure, Cut, Assemble, Finish), Fixing Damage (Assess, Remove, Repair, Match), and Quality Check (Inspect, Test Joints, Check Alignment)" width="800"/>
类似地，开发人员有不同任务的工作流。GitHub CopilotCLI增强了这些工作流，使您在日常编码任务中更加高效。

---

五种工作流程<img src="/images/learning-hub/copilot-cli-for-beginners/03/five-workflows.png" alt="Five glowing neon icons representing code review, testing, debugging, refactoring, and git integration workflows" width="800"/>
下面的每个工作流都是独立的。选择那些符合你当前需求的，或者全部解决。

---

选择你自己的冒险

本章涵盖了开发人员通常使用的五个工作流。**然而，你不需要一次全部读完！**每个工作流都包含在下面的可折叠部分中。选择那些符合你需要的，最适合你当前项目的。你可以随时回来探索其他的。<img src="/images/learning-hub/copilot-cli-for-beginners/03/five-workflows-swimlane.png" alt="Five Development Workflows: Code Review, Refactoring, Debugging, Test Generation, and Git Integration shown as horizontal swimlanes" width="800"/>
b|我想…|跳到||---|---|
合并前检查代码| [Workflow -1: code Review](# Workflow -1-code- Review) |
清理杂乱的或遗留的代码| [Workflow 2: Refactoring](# Workflow -2- Refactoring) |
| [Workflow -3: Debugging](# Workflow -3- Debugging) |
为我的代码生成测试| [Workflow -4: Test Generation](# Workflow -4- Test - Generation) |
|编写更好的提交和pr | [Workflow 5: Git集成](# Workflow -5- Git - Integration) |
快速提示：在你计划或编码之前先研究](# Quick - Tip - Research -before- You - Plan -or- Code) |
|查看完整的端到端bug修复工作流程| [put It All Together](#put - It - All - Together -bug-fix-workflow) |

**选择下面的工作流来扩展它**并查看GitHub CopilotCLI如何在该区域增强您的开发过程。

---<a id="workflow-1-code-review"></a>
<details>
<summary><strong>Workflow 1: Code Review</strong> - Review files, use the /review agent, create severity checklists</summary>

<img src="/images/learning-hub/copilot-cli-for-beginners/03/code-review-swimlane-single.png" alt="Code review workflow: review, identify issues, prioritize, generate checklist." width="800"/>
基本回顾

本例使用`@`符号来引用文件，使Copilot CLI能够直接访问其内容以进行审查。```bash
copilot

> Review @samples/book-app-project/book_app.py for code quality
```

---

<details>
<summary>🎬 See it in action!</summary>
！[代码审查演示]（/images/learning-hub/copilot-cli-for-beginners/03/code-review-demo.gif）

*Demo输出不同。您的模型、工具和响应将与此处显示的有所不同</details>
---

输入验证审查

通过在提示中列出您关心的类别，要求Copilot CLI将其审查重点放在一个特定的关注点上（这里是输入验证）。```text
copilot

> Review @samples/book-app-project/utils.py for input validation issues. Check for: missing validation, error handling gaps, and edge cases
```
跨文件项目评审

使用`@`引用整个目录，让Copilot CLI一次扫描项目中的每个文件。```bash
copilot

> @samples/book-app-project/ Review this entire project. Create a markdown checklist of issues found, categorized by severity
```
交互式代码审查

使用多回合对话来深入操练。从一个广泛的回顾开始，然后问一些后续问题，而不是重新开始。```bash
copilot

> @samples/book-app-project/book_app.py Review this file for:
> - Input validation
> - Error handling
> - Code style and best practices

# Copilot CLI provides detailed review

> The user input handling - are there any edge cases I'm missing?

# Copilot CLI shows potential issues with empty strings, special characters

> Create a checklist of all issues found, prioritized by severity

# Copilot CLI generates prioritized action items
```
###检查清单模板

要求Copilot CLI以特定格式构建其输出（这里是一个可以粘贴到问题中的严重分类的降价清单）。```bash
copilot

> Review @samples/book-app-project/ and create a markdown checklist of issues found, categorized by:
> - Critical (data loss risks, crashes)
> - High (bugs, incorrect behavior)
> - Medium (performance, maintainability)
> - Low (style, minor improvements)
```
了解Git变更（对/review很重要）

在使用`/review`命令之前，您需要了解git中的两种类型的更改：

|更改类型|含义|如何看到||-------------|---------------|------------|
| **阶段性更改** |您为下一次提交标记的文件`git add`|`git diff --staged`|
| **未分级更改** |已修改但尚未添加的文件|`git diff`|```bash
# Quick reference
git status           # Shows both staged and unstaged
git add file.py      # Stage a file for commit
git diff             # Shows unstaged changes
git diff --staged    # Shows staged changes
```
###使用/review命令`/review`命令调用内置的代码审查代理**，该代理针对分析具有高信噪比输出的分级和非分级更改进行了优化。使用斜杠命令触发专门的内置代理，而不是编写自由格式的提示符。```bash
copilot

> /review
# Invokes the code-review agent on staged/unstaged changes
# Provides focused, actionable feedback

> /review Check for security issues in authentication
# Run review with specific focus area
```
>💡**提示**：当您有未完成的更改时，代码审查代理工作得最好。使用`git add`存放文件，以便进行更集中的审查。</details>

---

<a id="workflow-2-refactoring"></a>
<details>
<summary><strong>Workflow 2: Refactoring</strong> - Restructure code, separate concerns, improve error handling</summary>

<img src="/images/learning-hub/copilot-cli-for-beginners/03/refactoring-swimlane-single.png" alt="Refactoring workflow: assess code, plan changes, implement, verify behavior." width="800"/>
简单重构

> **先试试这个：**`@samples/book-app-project/book_app.py The command handling uses if/elif chains. Refactor it to use a dictionary dispatch pattern.`从简单的改进开始。在图书应用程序上尝试这些。每个提示使用`@`文件引用与特定的重构指令配对，因此Copilot CLI确切地知道要更改什么。```bash
copilot

> @samples/book-app-project/book_app.py The command handling uses if/elif chains. Refactor it to use a dictionary dispatch pattern.

> @samples/book-app-project/utils.py Add type hints to all functions

> @samples/book-app-project/book_app.py Extract the book display logic into utils.py for better separation of concerns
```
>💡**不熟悉重构？**在处理复杂的转换之前，先从简单的请求开始，比如添加类型提示或改进变量名。

---<details>
<summary>🎬 See it in action!</summary>
！[演示重构](/images/learning-hub/copilot-cli-for-beginners/03/refactor-demo.gif)

*Demo输出不同。您的模型、工具和响应将与此处显示的有所不同</details>
---

###分离关注点

在一个提示符中使用`@`引用多个文件，这样Copilot CLI就可以在它们之间移动代码，作为重构的一部分。```bash
copilot

> @samples/book-app-project/utils.py @samples/book-app-project/book_app.py
> The utils.py file has print statements mixed with logic. Refactor to separate display functions from data processing.
```
改进错误处理

提供两个相关的文件并描述横切关注点，以便Copilot CLI可以在两者之间提出一致的修复建议。```bash
copilot

> @samples/book-app-project/utils.py @samples/book-app-project/books.py
> These files have inconsistent error handling. Suggest a unified approach using custom exceptions.
```
###添加文档

使用详细的项目符号列表来指定每个文档字符串应该包含的内容。```bash
copilot

> @samples/book-app-project/books.py Add comprehensive docstrings to all methods:
> - Include parameter types and descriptions
> - Document return values
> - Note any exceptions raised
> - Add usage examples
```
使用测试进行安全重构

在多回合会话中链接两个相关请求。首先生成测试，然后用这些测试作为安全网进行重构。```bash
copilot

> @samples/book-app-project/books.py Before refactoring, generate tests for current behavior

# Get tests first

> Now refactor the BookCollection class to use a context manager for file operations

# Refactor with confidence - tests verify behavior is preserved
```

</details>

---

<a id="workflow-3-debugging"></a>
<details>
<summary><strong>Workflow 3: Debugging</strong> - Track down bugs, security audits, trace issues across files</summary>

<img src="/images/learning-hub/copilot-cli-for-beginners/03/debugging-swimlane-single.png" alt="Debugging workflow: understand error, locate root cause, fix, test." width="800"/>
简单调试

> **先试试这个：**`@samples/book-app-buggy/books_buggy.py Users report that searching for "The Hobbit" returns no results even though it's in the data. Debug why.`首先描述出了什么问题。这里有一些常见的调试模式，你可以尝试使用有bug的book应用程序。每个提示符都有一个`@`文件引用和一个清晰的症状描述，这样Copilot CLI就可以定位和诊断bug。```bash
copilot

# Pattern: "Expected X but got Y"
> @samples/book-app-buggy/books_buggy.py Users report that searching for "The Hobbit" returns no results even though it's in the data. Debug why.

# Pattern: "Unexpected behavior"
> @samples/book-app-buggy/book_app_buggy.py When I remove a book that doesn't exist, the app says it was removed. Help me find why.

# Pattern: "Wrong results"
> @samples/book-app-buggy/books_buggy.py When I mark one book as read, ALL books get marked. What's the bug?
```
>💡**调试提示**：描述*症状*（您看到的）和*期望*（应该发生的）。副驾驶命令行解决剩下的问题。

---<details>
<summary>🎬 See it in action!</summary>
！[修复Bug演示]（/images/learning-hub/copilot-cli-for-beginners/03/fix-bug-demo.gif）

*Demo输出不同。您的模型、工具和响应将与此处显示的有所不同</details>
---

“Bug侦探”- AI发现相关的Bug

这就是上下文感知调试的亮点所在。用有bug的book应用程序尝试这个场景。通过`@`提供整个文件，并只描述用户报告的症状。Copilot CLI将追踪根本原因，并可能发现附近的其他错误。```bash
copilot

> @samples/book-app-buggy/books_buggy.py
>
> Users report: "Finding books by author name doesn't work for partial names"
> Debug why this happens
```
** Copilot CLI的功能**：```
Root Cause: Line 80 uses exact match (==) instead of partial match (in).

Line 80: return [b for b in self.books if b.author == author]

The find_by_author function requires an exact match. Searching for "Tolkien"
won't find books by "J.R.R. Tolkien".

Fix: Change to case-insensitive partial match:
return [b for b in self.books if author.lower() in b.author.lower()]
```
**为什么这很重要**:Copilot CLI读取整个文件，理解你的bug报告的上下文，并给你一个明确的解释。

>💡**奖金**：因为Copilot CLI分析整个文件，它经常发现*其他*问题，你没有问。例如，在修复作者搜索时，Copilot CLI可能还会注意到`find_book_by_title`！

真实世界的安全侧边栏

虽然调试自己的代码很重要，但理解生产应用程序中的安全漏洞也很关键。试试这个例子：将Copilot CLI指向一个不熟悉的文件，并要求它审核安全问题。```bash
copilot

> @samples/buggy-code/python/user_service.py Find all security vulnerabilities in this Python user service
```
该文件演示了您在生产应用程序中会遇到的实际安全模式。

>💡**您将遇到的常见安全术语：**
> - **SQL注入**：将用户输入直接输入到数据库查询中，允许攻击者执行恶意命令
> - **参数化查询**：安全的替代方案-占位符（`?`）将用户数据与SQL命令分开
> - **竞争条件**：两个操作同时发生并且相互干扰
> - **XSS（跨站脚本）**：攻击者将恶意脚本注入网页

---

理解错误

将堆栈跟踪与`@`文件引用一起直接粘贴到提示符中，以便Copilot CLI可以将错误映射到源代码。```bash
copilot

> I'm getting this error:
> AttributeError: 'NoneType' object has no attribute 'title'
>     at show_books (book_app.py:19)
>
> @samples/book-app-project/book_app.py Explain why and how to fix it
```
使用测试用例进行调试

描述准确的输入和观察到的输出，给Copilot CLI一个具体的、可重复的测试用例来进行推理。```bash
copilot

> @samples/book-app-buggy/books_buggy.py The remove_book function has a bug. When I try to remove "Dune",
> it also removes "Dune Messiah". Debug this: explain the root cause and provide a fix.
```
通过代码跟踪问题

引用多个文件，并要求Copilot CLI跟踪它们之间的数据流，以找到问题的根源。```bash
copilot

> Users report that the book list numbering starts at 0 instead of 1.
> @samples/book-app-buggy/book_app_buggy.py @samples/book-app-buggy/books_buggy.py
> Trace through the list display flow and identify where the issue occurs
```
理解数据问题

在读取数据文件的代码旁边包含一个数据文件，以便Copilot CLI在建议改进错误处理时了解全局。```bash
copilot

> @samples/book-app-project/data.json @samples/book-app-project/books.py
> Sometimes the JSON file gets corrupted and the app crashes. How should we handle this gracefully?
```

</details>

---

<a id="workflow-4-test-generation"></a>
<details>
<summary><strong>Workflow 4: Test Generation</strong> - Generate comprehensive tests and edge cases automatically</summary>

<img src="/images/learning-hub/copilot-cli-for-beginners/03/test-gen-swimlane-single.png" alt="Test Generation workflow: analyze function, generate tests, include edge cases, run." width="800"/>
> **先试试这个：**`@samples/book-app-project/books.py Generate pytest tests for all functions including edge cases`“测试爆炸”——2次测试vs 15次以上测试

手动编写测试，开发人员通常创建2-3个基本测试：
-测试有效输入
-测试无效输入
-测试一个边缘情况

看看当您要求Copilot CLI生成全面测试时会发生什么！这个提示使用一个结构化的项目列表和一个`@`文件引用来指导Copilot CLI进行全面的测试覆盖：```bash
copilot

> @samples/book-app-project/books.py Generate comprehensive pytest tests. Include tests for:
> - Adding books
> - Removing books
> - Finding by title
> - Finding by author
> - Marking as read
> - Edge cases with empty data
```

---

<details>
<summary>🎬 See it in action!</summary>
！[测试生成演示]（/images/learning-hub/copilot-cli-for-beginners/03/test-gen-demo.gif）

*Demo输出不同。您的模型、工具和响应将与此处显示的有所不同</details>
---

**你得到什么**:15+综合测试，包括：```python
class TestBookCollection:
    # Happy path
    def test_add_book_creates_new_book(self):
        ...
    def test_list_books_returns_all_books(self):
        ...

    # Find operations
    def test_find_book_by_title_case_insensitive(self):
        ...
    def test_find_book_by_title_returns_none_when_not_found(self):
        ...
    def test_find_by_author_partial_match(self):
        ...
    def test_find_by_author_case_insensitive(self):
        ...

    # Edge cases
    def test_add_book_with_empty_title(self):
        ...
    def test_remove_nonexistent_book(self):
        ...
    def test_mark_as_read_nonexistent_book(self):
        ...

    # Data persistence
    def test_save_books_persists_to_json(self):
        ...
    def test_load_books_handles_missing_file(self):
        ...
    def test_load_books_handles_corrupted_json(self):
        ...

    # Special characters
    def test_add_book_with_unicode_characters(self):
        ...
    def test_find_by_author_with_special_characters(self):
        ...
```
**结果**：在30秒内，您将获得需要花费一个小时来思考和编写的边缘情况测试。

---

单元测试

以单个功能为目标，列举要测试的输入类别，以便Copilot CLI生成集中的、彻底的单元测试。```bash
copilot

> @samples/book-app-project/utils.py Generate comprehensive pytest tests for get_book_details covering:
> - Valid input
> - Empty strings
> - Invalid year formats
> - Very long titles
> - Special characters in author names
```
运行测试

问Copilot CLI一个简单的关于工具链的问题。它可以为您生成正确的shell命令。```bash
copilot

> How do I run the tests? Show me the pytest command.

# Copilot CLI responds:
# cd samples/book-app-project && python -m pytest tests/
# Or for verbose output: python -m pytest tests/ -v
# To see print statements: python -m pytest tests/ -s
```
针对特定场景进行测试

列出您想要涵盖的高级或棘手的场景，以便Copilot CLI超越快乐的道路。```bash
copilot

> @samples/book-app-project/books.py Generate tests for these scenarios:
> - Adding duplicate books (same title and author)
> - Removing a book by partial title match
> - Finding books when collection is empty
> - File permission errors during save
> - Concurrent access to the book collection
```
向现有文件添加测试

要求对单个功能进行“额外”测试，以便Copilot CLI生成新的用例来补充您已有的用例。```bash
copilot

> @samples/book-app-project/books.py
> Generate additional tests for the find_by_author function with edge cases:
> - Author name with hyphens (e.g., "Jean-Paul Sartre")
> - Author with multiple first names
> - Empty string as author
> - Author name with accented characters
```

</details>

---

<a id="workflow-5-git-integration"></a>
<details>
<summary><strong>Workflow 5: Git Integration</strong> - Commit messages, PR descriptions, /pr, /delegate, /diff, and /branch</summary>

<img src="/images/learning-hub/copilot-cli-for-beginners/03/git-integration-swimlane-single.png" alt="Git Integration workflow: stage changes, generate message, commit, create PR." width="800"/>
>💡**这个工作流假定基本的git熟悉度**（分段、提交、分支）。如果您对git不熟悉，请先尝试其他四个工作流。

生成提交消息

> **先试试这个：**`copilot -p "Generate a conventional commit message for: $(git diff --staged)"`-阶段一些变化，然后运行这个，看看Copilot CLI写你的提交消息。

本例使用`-p`内联提示标志和shell命令替换，将`git diff`输出直接管道到Copilot CLI中，以获得一次性提交消息。`$(...)`语法运行括号内的命令，并将其输出插入外部命令。```bash

# See what changed
git diff --staged

# Generate commit message using [Conventional Commit](https://github.com/github/copilot-cli-for-beginners/blob/main/GLOSSARY.md#conventional-commit) format
# (structured messages like "feat(books): add search" or "fix(data): handle empty input")
copilot -p "Generate a conventional commit message for: $(git diff --staged)"

# Output: "feat(books): add partial author name search
#
# - Update find_by_author to support partial matches
# - Add case-insensitive comparison
# - Improve user experience when searching authors"
```

---

<details>
<summary>🎬 See it in action!</summary>
！[Git集成演示]（/images/learning-hub/copilot-cli-for-beginners/03/git-integration-demo.gif）

*Demo输出不同。您的模型、工具和响应将与此处显示的有所不同</details>
---

解释变化

将`git show`的输出管道到`-p`提示符中，以获得最后一次提交的简单英文摘要。```bash
# What did this commit change?
copilot -p "Explain what this commit does: $(git show HEAD --stat)"
```
PR描述

将`git log`输出与结构化提示模板结合起来，自动生成完整的拉取请求描述。```bash
# Generate PR description from branch changes
copilot -p "Generate a pull request description for these changes:
$(git log main..HEAD --oneline)

Include:
- Summary of changes
- Why these changes were made
- Testing done
- Breaking changes? (yes/no)"
```
在当前分支的交互模式下使用/pr

如果你在Copilot CLI的交互模式下与分支一起工作，你可以使用`/pr`命令来处理拉请求。使用`/pr`查看一个PR，创建一个新的PR，修复一个现有的PR，或者让Copilot CLI根据分支状态自动决定。```bash
copilot

> /pr [view|create|fix|auto]
```
推前回顾

在`-p`提示符中使用`git diff main..HEAD`，以便对所有分支更改进行快速的预推送完整性检查。```bash
# Last check before pushing
copilot -p "Review these changes for issues before I push:
$(git diff main..HEAD)"
```
为后台任务使用/delegate`/delegate`命令将工作移交给GitHub上的Copilot编码代理。使用`/delegate`斜杠命令（或`&`快捷方式）将定义良好的任务卸载给后台代理。```bash
copilot

> /delegate Add input validation to the login form

# Or use the & prefix shortcut:
> & Fix the typo in the README header

# Copilot CLI:
# 1. Commits your changes to a new branch
# 2. Opens a draft pull request
# 3. Works in the background on GitHub
# 4. Requests your review when done
```
当你专注于其他工作时，这对于你想要完成的明确的任务来说是很好的。

###使用/diff检查会话更改`/diff`命令显示当前会话期间所做的所有更改。使用这个斜杠命令来查看在提交之前Copilot CLI修改的所有内容的视觉差异。它也可以在非git存储库的文件夹中工作。```bash
copilot

# After making some changes...
> /diff

# Shows a visual diff of all files modified in this session
# Great for reviewing before committing
```
使用/branch或/fork来分支你的会话

有时你想在不丢失原始对话的情况下探索解决问题的两种不同方法。`/branch`命令（也可以作为`/fork`使用）创建当前会话的副本，以便您可以尝试不同的方向，然后比较结果。```bash
copilot

> Fix the find_by_author function to support partial matches

# You want to try a different approach — branch first!
> /branch

# Now you're in a new session copy. Try your alternative approach:
> Fix find_by_author using a different regex-based strategy

# If you don't like the result, switch back to your original session using /session
```
>💡**`/branch`和`/fork`是相同的**：两个命令做相同的事情。添加`/branch`是为了更直观的名称。选择对你更有意义的方法。

>💡**何时进行分支**：当你不确定哪种方法更好，并希望保留两种选择时，分支是很好的选择。</details>
---

快速提示：在计划或编码之前进行研究

当您需要调查一个库、了解最佳实践或探索一个不熟悉的主题时，请在编写任何代码之前使用`/research`进行深入的研究调查：```bash
copilot

> /research What are the best Python libraries for validating user input in CLI apps?
```
Copilot搜索GitHub存储库和web资源，然后返回带有引用的摘要。当你准备开始一项新功能并希望首先做出明智的决定时，这是非常有用的。您可以使用`/share`共享结果。

>💡**提示**:`/research`工作良好**之前*`/plan`。研究方法，然后计划实施。

---

把它放在一起：Bug修复工作流程

下面是修复报告错误的完整工作流程：```bash

# 1. Understand the bug report
copilot

> Users report: 'Finding books by author name doesn't work for partial names'
> @samples/book-app-project/books.py Analyze and identify the likely cause

# 2. Debug the issue (continuing in same session)
> Based on the analysis, show me the find_by_author function and explain the issue

> Fix the find_by_author function to handle partial name matches

# 3. Generate tests for the fix
> @samples/book-app-project/books.py Generate pytest tests specifically for:
> - Full author name match
> - Partial author name match
> - Case-insensitive matching
> - Author name not found

# 4. Generate commit message
copilot -p "Generate commit message for: $(git diff --staged)"

# Output: "fix(books): support partial author name search"
```
Bug修复工作流程总结

|步骤|动作|副驾驶命令||------|--------|-----------------|
|了解bug |`> [describe bug] @relevant-file.py Analyze the likely cause`|
获得详细分析|`> Show me the function and explain the issue`|
| 3 |实现修复|`> Fix the [specific issue]`|
bbb40 |生成测试|`> Generate tests for [specific scenarios]`|
bbb5 |提交|`copilot -p "Generate commit message for: $(git diff --staged)"`|

---

#实践<img src="/images/learning-hub/copilot-cli-for-beginners/03/practice.png" alt="Warm desk setup with monitor showing code, lamp, coffee cup, and headphones ready for hands-on practice" width="800"/>
现在轮到您应用这些工作流了。

---

##▶️自己试试

完成演示后，请尝试以下变体：

1. **Bug detection Challenge**：让Copilot CLI调试`samples/book-app-buggy/books_buggy.py`中的`mark_as_read`函数。它是否解释了为什么该函数将所有书标记为已读，而不是只有一本？

2. **测试挑战**：在图书应用程序中生成`add_book`函数的测试。计算Copilot CLI包含的您不会想到的边缘情况。

3. **Commit Message Challenge**：对book app文件做任何小的改变，stage它（`git add .`），然后运行：   ```bash
   copilot -p "Generate a conventional commit message for: $(git diff --staged)"
   ```
这条信息是否比你快速写出来的内容更好？

**自检：当你能够解释为什么“调试这个bug”比“找到bug”更强大时，你就理解了开发工作流（上下文很重要！）。

---

##📝作业

主要挑战：重构、测试和发布

实际操作的示例集中在`find_book_by_title`和代码审查上。现在在`book-app-project`的不同功能上练习相同的工作流技能：1. **审查**：要求副驾驶CLI在`books.py`中审查`remove_book()`的边缘情况和潜在问题：`@samples/book-app-project/books.py Review the remove_book() function. What happens if the title partially matches another book (e.g., "Dune" vs "Dune Messiah")? Are there any edge cases not handled?`2. **重构**：要求Copilot CLI改进`remove_book()`，以处理不区分大小写的匹配等边缘情况，并在找不到书时返回有用的反馈
3. **Test**：专门为改进的`remove_book()`函数生成pytest测试，包括：
—删除存在的图书
-不区分大小写的标题匹配
-一本不存在的书会带来适当的反馈
-从空集合中移除
4. **Review**：执行更改并运行`/review`以检查是否存在任何遗留问题
5. **Commit**：生成常规提交消息：`copilot -p "Generate a conventional commit message for: $(git diff --staged)"`<details>
<summary>💡 Hints (click to expand)</summary>
**每个步骤的示例提示```bash
copilot

# Step 1: Review
> @samples/book-app-project/books.py Review the remove_book() function. What edge cases are not handled?

# Step 2: Refactor
> Improve remove_book() to use case-insensitive matching and return a clear message when the book isn't found. Show me the before and after code.

# Step 3: Test
> Generate pytest tests for the improved remove_book() function, including:
> - Removing a book that exists
> - Case-insensitive matching ("dune" should remove "Dune")
> - Book not found returns appropriate response
> - Removing from an empty collection

# Step 4: Review
> /review

# Step 5: Commit
> Generate a conventional commit message for this refactor
```
**提示：**改进`remove_book()`后，尝试询问Copilot CLI：“此文件中是否有其他功能可以从相同的改进中受益？”它可能建议对`find_book_by_title()`或`find_by_author()`进行类似的更改。</details>
###奖金挑战：创建一个应用程序与Copilot CLI

>💡**注意**：这个GitHub技能练习使用**Node.js**而不是Python。您将练习的GitHub CopilotCLI技术——创建问题、生成代码和从终端进行协作——适用于任何语言。

该练习向开发人员展示了如何在构建Node.js计算器应用程序时使用GitHub CopilotCLI来创建问题、生成代码和从终端进行协作。您将安装CLI，使用模板和代理，并练习迭代的命令行驱动开发。

##### <img src="/images/learning-hub/copilot-cli-for-beginners/03/github-skills-logo.png" width="28" align=“center“ />[开始”使用Copilot CLI创建应用程序”技能练习]（https://github.com/skills/create-applications-with-the-copilot-cli）

---<details>
<summary>🔧 <strong>Common Mistakes & Troubleshooting</strong> (click to expand)</summary>
常见错误

|错误|发生了什么|修复||---------|--------------|-----|
|使用模糊的提示，如“检查这段代码”|遗漏特定问题的一般反馈|要具体：“检查SQL注入、XSS和认证问题”|
|不使用`/review`进行代码审查|缺少优化的代码审查代理|使用`/review`，它针对高信噪比输出|进行了调优
|要求在没有上下文的情况下“查找bug”| Copilot CLI不知道你遇到了什么bug |描述症状：“用户报告X发生时Y”|
|测试可能使用错误的语法或断言库|指定：“使用Jest生成测试”或“使用pytest” |

# # #故障排除

**审查似乎不完整** -更具体地说明要寻找什么：```bash
copilot

# Instead of:
> Review @samples/book-app-project/book_app.py

# Try:
> Review @samples/book-app-project/book_app.py for input validation, error handling, and edge cases
```
**测试与我的框架不匹配** -指定框架：```bash
copilot

> @samples/book-app-project/books.py Generate tests using pytest (not unittest)
```
**重构改变行为** -要求Copilot CLI保留行为：```bash
copilot

> @samples/book-app-project/book_app.py Refactor command handling to use dictionary dispatch. IMPORTANT: Maintain identical external behavior - no breaking changes
```

</details>
---

#总结

##🔑关键要点<img src="/images/learning-hub/copilot-cli-for-beginners/03/specialized-workflows.png" alt="Specialized Workflows for Every Task: Code Review, Refactoring, Debugging, Testing, and Git Integration" width="800"/>
1. **代码审查**变得全面与特定的提示
2. 当你先生成测试时，重构**更安全
3. **调试**受益于显示副驾驶CLI的错误和代码
4. **测试生成**应包括边缘情况和错误场景
5. **Git集成**自动提交消息和PR描述

>📋**快速参考**：请参阅[GitHub CopilotCLI命令参考]（https://docs.github.com/en/copilot/reference/cli-command-reference）获得完整的命令和快捷方式列表。

---

##✅检查点：你已经掌握了要领

* *恭喜你!**你现在有所有的核心技能，是富有成效的GitHub CopilotCLI：

|技能|章节|你现在可以…||-------|---------|----------------|
|基本命令| Ch 01 | |使用交互模式、计划模式、编程模式（-p）和斜杠命令
|上下文| ch02 |与`@`的参考文件，管理会话，了解上下文windows |
审查代码，重构，调试，生成测试，集成git |

第04-06章涵盖了更多的功能，值得学习。

---

##推荐️建立你的个人工作流程

没有单一的“正确”方法来使用GitHub CopilotCLI。这里有一些建议，可以帮助你建立自己的模式：

>📚**官方文档**:[Copilot CLI最佳实践]（https://docs.github.com/copilot/how-tos/copilot-cli/cli-best-practices）从GitHub推荐的工作流程和提示。- **从`/plan`**开始。在执行之前完善计划——好的计划会带来更好的结果。
- **保存提示，工作良好。**当Copilot CLI出错时，注意哪里出错了。久而久之，这就变成了你的个人剧本。
- **自由实验。**有些开发人员喜欢冗长、详细的提示。其他人更喜欢简短的提示和后续内容。尝试不同的方法，注意什么感觉自然。

>💡**即将到来**：在第04章和第05章，您将学习如何将您的最佳实践编码为自定义说明和技能，Copilot CLI自动加载。

---

##➡️下一步是什么

其余章节涵盖了扩展Copilot CLI功能的其他功能：

b|章它涵盖了什么b|当你想要的时候b||---------|----------------|---------------------|
| Ch 04：代理|创建专门的AI角色|当你需要领域专家（前端，安全）|
当你经常重复相同的提示时，自动加载任务的说明
| ch06: MCP |连接外部服务|当你需要实时数据从GitHub，数据库|

**建议**：尝试一周的核心工作流程，当你有特殊需求时，再回到第04-06章。

---

##继续其他主题

在**[第04章：代理和自定义说明](../04-agents-and-custom-instructions/)**中，您将学习：

-使用内置代理（`/plan`,`/review`）
-使用`.agent.md`文件创建专门的代理（前端专家、安全审计员）
-多代理协作模式
-自定义项目标准的指导文件

---