---
title: '02 · Context and Conversations'
description: 'Learn how to give Copilot CLI richer context and build stronger multi-turn conversations.'
authors:
  - GitHub Copilot Learning Hub Team
lastUpdated: 2026-03-20
---
！[第二章：语境与对话]（/images/learning-hub/copilot-cli-for-beginners/02/chapter-header.png）

如果AI可以看到你的整个代码库，而不是一次一个文件，那会怎么样？**

在本章中，您将解锁GitHub CopilotCLI的真正功能：上下文。您将学习使用`@`语法来引用文件和目录，让Copilot CLI深入了解您的代码库。您将了解如何跨会议维护对话，几天后恢复工作，并了解跨文件分析如何捕获单文件审查完全遗漏的错误。

##🎯学习目标

在本章结束时，你将能够：

—使用`@`语法引用文件、目录和镜像
-使用`--resume`和`--continue`恢复以前的会话
-了解[上下文窗口]（https://github.com/github/copilot-cli-for-beginners/blob/main/GLOSSARY.md#context-window）是如何工作的
-写有效的多回合对话
—管理多项目工作流的目录权限>⏱️**预计时间**:~50分钟（20分钟阅读+ 30分钟动手）

---

##🧩现实世界的类比：与同事一起工作<img src="/images/learning-hub/copilot-cli-for-beginners/02/colleague-context-analogy.png" alt="Context Makes the Difference - Without vs With Context" width="800"/>
*就像你的同事一样，Copilot CLI不会读心术。提供更多信息有助于人类和副驾驶提供有针对性的支持！*

想象一下向同事解释一个bug：

> **没有上下文**：“图书应用程序不起作用。”

b> **带上下文**：“看看`books.py`，特别是`find_book_by_title`函数。它不会进行不区分大小写的匹配。”

为Copilot CLI提供上下文，使用`@`语法*将Copilot CLI指向特定的文件。

---

基本：基本背景<img src="/images/learning-hub/copilot-cli-for-beginners/02/essential-basic-context.png" alt="Glowing code blocks connected by light trails representing how context flows through Copilot CLI conversations" width="800"/>
本节涵盖了有效使用上下文所需的所有内容。首先掌握这些基础知识。

---

## @语法`@`符号在提示符中引用文件和目录。这就是你告诉副驾驶命令行“看看这个文件”的方式。

>💡**注**：本课程中的所有示例都使用此存储库中包含的`samples/`文件夹，因此您可以直接尝试每个命令。

现在试试（不需要设置）

您可以对计算机上的任何文件尝试此操作：```bash
copilot

# Point at any file you have
> Explain what @package.json does
> Summarize @README.md
> What's in @.gitignore and why?
```
>💡**手边没有项目？**创建一个快速测试文件：
>“bash
> echo "def greet(name)：返回'Hello ' + name" >test.py>副驾驶员
@test.py做什么？
> ' ' '

###基本模式

|模式|功能|示例使用||---------|--------------|-------------|
|`@file.py`|引用单个文件|`Review @samples/book-app-project/books.py`|
|`@folder/`|引用目录|`Review @samples/book-app-project/`|下的所有文件
|`@file1.py @file2.py`|引用多个文件|`Compare @samples/book-app-project/book_app.py @samples/book-app-project/books.py`|

引用单个文件```bash
copilot

> Explain what @samples/book-app-project/utils.py does
```

---

<details>
<summary>🎬 See it in action!</summary>
！[文件背景演示]（/images/learning-hub/copilot-cli-for-beginners/02/file-context-demo.gif）

*Demo输出不同。您的模型、工具和响应将与此处显示的有所不同</details>
---

###引用多个文件```bash
copilot

> Compare @samples/book-app-project/book_app.py and @samples/book-app-project/books.py for consistency
```
引用整个目录```bash
copilot

> Review all files in @samples/book-app-project/ for error handling
```
---

跨档案情报

这就是背景成为超级力量的地方。单文件分析是有用的。跨文件分析具有变革性。<img src="/images/learning-hub/copilot-cli-for-beginners/02/cross-file-intelligence.png" alt="Cross-File Intelligence - comparing single-file vs cross-file analysis showing how analyzing files together reveals bugs, data flow, and patterns invisible in isolation" width="800"/>
演示：发现跨越多个文件的bug```bash
copilot

> @samples/book-app-project/book_app.py @samples/book-app-project/books.py
>
> How do these files work together? What's the data flow?
```
>💡**高级选项**：对于以安全为重点的跨文件分析，请尝试Python安全示例：
>“bash
b> @samples/buggy-code/python/user_service.py@samples/buggy-code/python/payment_processor.py> >查找跨越两个文件的安全漏洞
> ' ' '

---<details>
<summary>🎬 See it in action!</summary>
！(多文件演示)(/images/learning-hub/copilot-cli-for-beginners/02/multi-file-demo.gif)

*Demo输出不同。您的模型、工具和响应将与此处显示的有所不同</details>
---

** Copilot CLI发现了什么**：```
Cross-Module Analysis
=====================

1. DATA FLOW PATTERN
   book_app.py creates BookCollection instance and calls methods
   books.py defines BookCollection class and manages data persistence

   Flow: book_app.py (UI) → books.py (business logic) → data.json (storage)

2. DUPLICATE DISPLAY FUNCTIONS
   book_app.py:9-21    show_books() function
   utils.py:28-36      print_books() function

   Impact: Two nearly identical functions doing the same thing. If you update
   one (like changing the format), you must remember to update the other.

3. INCONSISTENT ERROR HANDLING
   book_app.py handles ValueError from year conversion
   books.py silently returns None/False on errors

   Pattern: No unified approach to error handling across modules
```
**为什么这很重要**：单一文件审查会错过更大的画面。跨文件分析显示：
- **需要合并的重复代码**
- **数据流模式**显示组件如何交互
-影响可维护性的架构问题

---

演示：在60秒内理解一个代码库<img src="/images/learning-hub/copilot-cli-for-beginners/02/codebase-understanding.png" alt="Split-screen comparison showing manual code review taking 1 hour versus AI-assisted analysis taking 10 seconds" width="800" />
刚开始一个项目？使用Copilot CLI快速了解它。```bash
copilot

> @samples/book-app-project/
>
> In one paragraph, what does this app do and what are its biggest quality issues?
```
**你得到什么**：```
This is a CLI book collection manager that lets users add, list, remove, and
search books stored in a JSON file. The biggest quality issues are:

1. Duplicate display logic - show_books() and print_books() do the same thing
2. Inconsistent error handling - some errors raise exceptions, others return False
3. No input validation - year can be 0, empty strings accepted for title/author
4. Missing tests - no test coverage for critical functions like find_book_by_title

Priority fix: Consolidate duplicate display functions and add input validation.
```
**结果**：什么需要一个小时的代码读取压缩到10秒。你知道该把注意力集中在哪里。

---

##实际例子

示例1：带上下文的代码审查```bash
copilot

> @samples/book-app-project/books.py Review this file for potential bugs

# Copilot CLI now has the full file content and can give specific feedback:
# "Line 49: Case-sensitive comparison may miss books..."
# "Line 29: JSON decode errors are caught but data corruption isn't logged..."

> What about @samples/book-app-project/book_app.py?

# Now reviewing book_app.py, but still aware of books.py context
```
例2：理解代码库```bash
copilot

> @samples/book-app-project/books.py What does this module do?

# Copilot CLI reads books.py and understands the BookCollection class

> @samples/book-app-project/ Give me an overview of the code structure

# Copilot CLI scans the directory and summarizes

> How does the app save and load books?

# Copilot CLI can trace through the code it's already seen
```

<details>
<summary>🎬 See a multi-turn conversation in action!</summary>
！(多向演示)(/images/learning-hub/copilot-cli-for-beginners/02/multi-turn-demo.gif)

*Demo输出不同。您的模型、工具和响应将与此处显示的有所不同</details>
例3：多文件重构```bash
copilot

> @samples/book-app-project/book_app.py @samples/book-app-project/utils.py
> I see duplicate display functions: show_books() and print_books(). Help me consolidate these.

# Copilot CLI sees both files and can suggest how to merge the duplicate code
```
---

##会话管理

会话在您工作时自动保存。您可以恢复以前的会话，继续您离开的地方。

会话自动保存

每个对话都会自动保存。正常退出：```bash
copilot

> @samples/book-app-project/ Let's improve error handling across all modules

[... do some work ...]

> /exit
```
恢复最近的会话```bash
# Continue where you left off
copilot --continue
```
###恢复指定会话```bash
# Pick from a list of sessions interactively
copilot --resume

# Or resume a specific session by ID
copilot --resume abc123
```
>💡**如何找到会话ID？你不需要记住它们。运行不带ID的`copilot --resume`将显示一个交互式列表，其中包含您以前的会话、它们的名称、ID以及它们最后一次活动的时间。挑一个你想要的。
>
> **多终端怎么办？**每个终端窗口都有自己的会话和自己的上下文。如果你在三个终端上打开了Copilot CLI，那就是三个独立的会话。从任何终端运行`--resume`都可以浏览它们。`--continue`标志捕获最近关闭的会话，而不管它在哪个终端。
>
> **可以不重启会话切换吗？* *是的。在活动会话中使用`/resume`斜杠命令：
> ' ' '
> > /resume
> #显示要切换到的会话列表
> ' ' '

###组织你的会议

给会话起一个有意义的名字，以便以后找到它们：```bash
copilot

> /rename book-app-review
# Session renamed for easier identification
```
检查和管理上下文

当您添加文件和对话时，Copilot CLI的[上下文窗口]（https://github.com/github/copilot-cli-for-beginners/blob/main/GLOSSARY.md#context-window）填充。两个命令可以帮助您保持控制：```bash
copilot

> /context
Context usage: 45,000 / 128,000 tokens (35%)

> /clear
# Wipes context and starts fresh. Use when switching topics
```
>💡**何时使用`/clear`**：如果您一直在审查`books.py`，并希望切换到讨论`utils.py`，请先运行`/clear`。否则，来自旧主题的陈旧上下文可能会混淆响应。

---

从你离开的地方重新开始<img src="/images/learning-hub/copilot-cli-for-beginners/02/session-persistence-timeline.png" alt="Timeline showing how GitHub Copilot CLI sessions persist across days - start on Monday, resume on Wednesday with full context restored" width="800"/>
*会话自动保存当你退出。几天后恢复完整的上下文：文件，问题和进度都记住了

想象一下这个跨多天的工作流程：```bash
# Monday: Start book app review
copilot

> /rename book-app-review
> @samples/book-app-project/books.py
> Review and number all code quality issues

Quality Issues Found:
1. Duplicate display functions (book_app.py & utils.py) - MEDIUM
2. No input validation for empty strings - MEDIUM
3. Year can be 0 or negative - LOW
4. No type hints on all functions - LOW
5. Missing error logging - LOW

> Fix issue #1 (duplicate functions)
# Work on the fix...

> /exit
```

```bash
# Wednesday: Resume exactly where you left off
copilot --continue

> What issues remain unfixed from our book app review?

Remaining issues from our book-app-review session:
2. No input validation for empty strings - MEDIUM
3. Year can be 0 or negative - LOW
4. No type hints on all functions - LOW
5. Missing error logging - LOW

Issue #1 (duplicate functions) was fixed on Monday.

> Let's tackle issue #2 next
```
**是什么让它强大**：几天后，Copilot CLI记得：
-就是你在查的那份文件
-已编号的问题列表
-哪些问题你已经解决了
-你们谈话的语境

没有重新阐释。没有重新读取文件。继续工作。

---

**🎉你现在知道要领了！**`@`语法、会话管理（`--continue`/`--resume`/`/rename`）和上下文命令（`/context`/`/clear`）足以实现高生产率。下面的内容都是可选的。当你准备好了再回来。

---

#可选：更深入<img src="/images/learning-hub/copilot-cli-for-beginners/02/optional-going-deeper.png" alt="Abstract crystal cave in blue and purple tones representing deeper exploration of context concepts" width="800"/>
这些主题建立在上述要点之上。**选择你感兴趣的，或者直接跳到[练习]（# Practice）

b|我想了解……|跳到||---|---|
|通配符模式和高级会话命令|[附加的@模式和会话命令](# Additional -patterns) |
|[上下文感知对话](#context-aware- Conversations) |
|令牌限制和`/compact`|[理解上下文窗口](# Understanding - Context - Windows) |
如何选择正确的文件来引用|[选择引用什么](# chooting - What -to-reference) |
|分析截图和模型|[使用图像](# Working -with- Images) |<details>
<summary><strong>Additional @ Patterns & Session Commands</strong></summary>
<a id="additional-patterns"></a>
额外的@模式

对于高级用户，Copilot CLI支持通配符模式和图像引用：

b|模式b|它的作用b||---------|--------------|
|`@folder/*.py`| |文件夹下的所有.py文件
|`@**/test_*.py`|递归通配符：找到所有的测试文件|
|`@image.png`|用于UI审查的映像文件|```bash
copilot

> Find all TODO comments in @samples/book-app-project/**/*.py
```
查看会话信息```bash
copilot

> /session
# Shows current session details and workspace summary

> /usage
# Shows session metrics and statistics
```
分享你的会话```bash
copilot

> /share file ./my-session.md
# Exports session as a markdown file

> /share gist
# Creates a GitHub gist with the session
```

</details>

<details>
<summary><strong>Context-Aware Conversations</strong></summary>
<a id="context-aware-conversations"></a>
上下文感知对话

当你拥有基于彼此的多回合对话时，奇迹就会发生。

####示例：渐进增强```bash
copilot

> @samples/book-app-project/books.py Review the BookCollection class

Copilot CLI: "The class looks functional, but I notice:
1. Missing type hints on some methods
2. No validation for empty title/author
3. Could benefit from better error handling"

> Add type hints to all methods

Copilot CLI: "Here's the class with complete type hints..."
[Shows typed version]

> Now improve error handling

Copilot CLI: "Building on the typed version, here's improved error handling..."
[Adds validation and proper exceptions]

> Generate tests for this final version

Copilot CLI: "Based on the class with types and error handling..."
[Generates comprehensive tests]
```
注意每个提示是如何建立在前面工作的基础上的。这就是情境的力量。</details>

<details>
<summary><strong>Understanding Context Windows</strong></summary>
<a id="understanding-context-windows"></a>
了解上下文窗口

您已经从基本知识中了解了`/context`和`/clear`。下面是上下文窗口如何工作的更深层次的图片。

每个AI都有一个“上下文窗口”，即它一次可以考虑的文本数量。<img src="/images/learning-hub/copilot-cli-for-beginners/02/context-window-visualization.png" alt="Context Window Visualization" width="800"/>
上下文窗口就像一张桌子：它一次只能容纳这么多东西。文件、对话记录和系统提示都占用空间

####极限会发生什么```bash
copilot

> /context

Context usage: 45,000 / 128,000 tokens (35%)

# As you add more files and conversation, this grows

> @large-codebase/

Context usage: 120,000 / 128,000 tokens (94%)

# Warning: Approaching context limit

> @another-large-file.py

Context limit reached. Older context will be summarized.
```
####`/compact`命令

当你的上下文变满，但你不想失去对话，`/compact`总结你的历史，以释放令牌：```bash
copilot

> /compact
# Summarizes conversation history, freeing up context space
# Your key findings and decisions are preserved
```
####环境效率提示

|情况|行动|为什么||-----------|--------|-----|
|开始新主题|`/clear`|删除不相关的上下文|
|长对话|`/compact`|总结历史，释放代币|
|需要特定的文件|`@file.py`不是`@folder/`|只加载你需要的|
|启动新会话|新鲜128K上下文|
|多个主题|每个主题使用`/rename`|很容易恢复正确的会话|

####大型代码库的最佳实践

1. **具体**:`@samples/book-app-project/books.py`而不是`@samples/book-app-project/`2. **主题之间清晰**：切换焦点时使用`/clear`3. **使用`/compact`**：总结对话以释放上下文
4. **使用多个会话**：每个功能或主题一个会话</details>

<details>
<summary><strong>Choosing What to Reference</strong></summary>
<a id="choosing-what-to-reference"></a>
选择要引用的内容

当涉及到上下文时，并非所有文件都是相同的。以下是明智选择的方法：

####文件大小注意事项

|文件大小|近似[token](https://github.com/github/copilot-cli-for-beginners/blob/main/GLOSSARY.md#token) |策略||-----------|-------------------|----------|
|小（<100行）| ~500-1,500个标记|自由引用|
|中等（100-500行）| ~1,500-7,500个令牌|参考特定文件|
|大（500+行）| 7500 +令牌|有选择性，使用特定的文件|
|非常大（1000+行）| 15,000+标记|考虑拆分或瞄准部分|

* *具体的例子:* *
-图书应用程序的4个Python文件组合≈2,000-3,000个令牌
—一个典型的Python模块（200行）≈3000个token
一个Flask API文件（400行）≈6000个token
-您的package.json≈200-500个令牌
—短提示+响应≈500- 1500个token

>💡**快速估计代码：**将代码行乘以~15以获得近似令牌。请记住，这只是一个估计。

####包括什么和排除什么**高价值**（包括这些）：
-入口点（`book_app.py`,`main.py`,`app.py`）
-你问的具体文件
—目标文件直接导入的文件
—配置文件（`requirements.txt`,`pyproject.toml`）
-数据模型或数据类

**较低值**（考虑排除）：
-生成文件（编译输出，捆绑资产）
—节点模块或厂商目录
-大型数据文件或固定装置
-与你的问题无关的文件

####特异性谱```
Less specific ────────────────────────► More specific
@samples/book-app-project/                      @samples/book-app-project/books.py:47-52
     │                                       │
     └─ Scans everything                     └─ Just what you need
        (uses more context)                      (preserves context)
```
**何时走宽** (`@samples/book-app-project/`)：
-初始代码库探索
-在许多文件中查找模式
-架构评论

**何时去具体** (`@samples/book-app-project/books.py`)：
-调试特定的问题
-特定文件的代码审查
-询问单个功能

####实际示例：分阶段上下文加载```bash
copilot

# Step 1: Start with structure
> @package.json What frameworks does this project use?

# Step 2: Narrow based on answer
> @samples/book-app-project/ Show me the project structure

# Step 3: Focus on what matters
> @samples/book-app-project/books.py Review the BookCollection class

# Step 4: Add related files only as needed
> @samples/book-app-project/book_app.py @samples/book-app-project/books.py How does the CLI use the BookCollection?
```
这种分阶段的方法保持了上下文的重点和效率。</details>

<details>
<summary><strong>Working with Images</strong></summary>
<a id="working-with-images"></a>
###使用图像

您可以使用`@`语法在对话中包含图像，或者简单地从剪贴板**粘贴（Cmd+V / Ctrl+V）。Copilot CLI可以分析截图、模型和图表，以帮助进行UI调试、设计实现和错误分析。```bash
copilot

> @images/screenshot.png What is happening in this image?

> @images/mockup.png Write the HTML and CSS to match this design. Place it in a new file called index.html and put the CSS in styles.css.
```
>📖**了解更多**：请参阅[附加上下文功能]（https://github.com/github/copilot-cli-for-beginners/blob/main/appendices/additional-context.md#working-with-images）了解支持的格式，实际用例以及将图像与代码结合的提示。</details>
---

#实践<img src="/images/learning-hub/copilot-cli-for-beginners/02/practice.png" alt="Warm desk setup with monitor showing code, lamp, coffee cup, and headphones ready for hands-on practice" width="800"/>
是时候应用您的上下文和会话管理技能了。

---

##▶️自己试试

全面项目审查

本课程包括示例文件，您可以直接查看。启动副驾驶，运行如下提示：```bash
copilot

> @samples/book-app-project/ Give me a code quality review of this project

# Copilot CLI will identify issues like:
# - Duplicate display functions
# - Missing input validation
# - Inconsistent error handling
```
>💡**想尝试使用自己的文件吗？**创建一个小型Python项目（`mkdir -p my-project/src`），添加一些.py文件，然后使用`@my-project/src/`检查它们。如果您愿意，您可以要求副驾驶为您创建示例代码！

会话工作流```bash
copilot

> /rename book-app-review
> @samples/book-app-project/books.py Let's add input validation for empty titles

[Copilot CLI suggests validation approach]

> Implement that fix
> Now consolidate the duplicate display functions in @samples/book-app-project/
> /exit

# Later - resume where you left off
copilot --continue

> Generate tests for the changes we made
```
---

完成演示后，请尝试以下变体：

1. **跨文件挑战**：分析book_app.py和books.py如何协同工作：   ```bash
   copilot
   > @samples/book-app-project/book_app.py @samples/book-app-project/books.py
   > What's the relationship between these files? Are there any code smells?
   ```
2. **会话挑战：启动一个会话，用`/rename my-first-session`命名它，做一些事情，用`/exit`退出，然后运行`copilot --continue`。它还记得你在做什么吗？

3. **上下文挑战**：在会话中运行`/context`。您使用了多少代币？尝试`/compact`并再次检查。（参见深入了解`/compact`中的[理解上下文窗口]（# Understanding - Context - Windows）。）

**自检：当您能够解释为什么`@folder/`比单独打开每个文件更强大时，您就理解了上下文。

---

##📝作业

主要挑战：跟踪数据流

实际操作的示例侧重于代码质量审查和输入验证。现在在不同的任务中练习相同的上下文技能，跟踪数据在应用程序中的移动方式：1. 启动交互式会话：`copilot`2. 参考`books.py`和`book_app.py`一起：`@samples/book-app-project/books.py @samples/book-app-project/book_app.py Trace how a book goes from user input to being saved in data.json. What functions are involved at each step?`3. 引入数据文件以获得额外的上下文：`@samples/book-app-project/data.json What happens if this JSON file is missing or corrupted? Which functions would fail?`4. 要求跨文件改进：`@samples/book-app-project/books.py @samples/book-app-project/utils.py Suggest a consistent error-handling strategy that works across both files.`5. 重命名会话：`/rename data-flow-analysis`6. 使用`/exit`退出，然后继续使用`copilot --continue`，并询问有关数据流的后续问题

**成功标准**：您可以跨多个文件跟踪数据，恢复命名会话，并获得跨文件建议。<details>
<summary>💡 Hints (click to expand)</summary>
* *开始:* *```bash
cd /path/to/copilot-cli-for-beginners
copilot
> @samples/book-app-project/books.py @samples/book-app-project/book_app.py Trace how a book goes from user input to being saved in data.json.
> @samples/book-app-project/data.json What happens if this file is missing or corrupted?
> /rename data-flow-analysis
> /exit
```
然后继续输入：`copilot --continue`* *有用的命令:* *
—`@file.py`—引用单个文件
-`@folder/`-引用文件夹中的所有文件（注意后面的`/`）
-`/context`-检查你使用了多少上下文
-`/rename <name>`-命名您的会话，以便于恢复</details>
奖励挑战：情境限制

1. 使用`@samples/book-app-project/`一次引用所有图书应用程序文件
2. 询问关于不同文件的几个详细问题（`books.py`、`utils.py`、`book_app.py`、`data.json`）
3. 运行`/context`查看使用情况。它能多快被填满？
4. 练习使用`/compact`来收回空间，然后继续对话
5. 尝试使用更具体的文件引用（例如，`@samples/book-app-project/books.py`而不是整个文件夹），看看它如何影响上下文使用

---<details>
<summary>🔧 <strong>Common Mistakes & Troubleshooting</strong> (click to expand)</summary>
常见错误

|错误|发生了什么|修复||---------|--------------|-----|
|副驾驶命令行将“books.py”视为纯文本|使用`@samples/book-app-project/books.py`引用文件|
|使用`--continue`（最后一个会话）或`--resume`（选择一个会话）|
|使用`/add-dir /path/to/directory`授予访问|的权限
|切换主题时不使用`/clear`|旧的上下文混淆了对新主题的响应|在启动不同的任务|之前运行`/clear`# # #故障排除

**"File not found"错误** -确保你在正确的目录下：```bash
pwd  # Check current directory
ls   # List files

# Then start copilot and use relative paths
copilot

> Review @samples/book-app-project/books.py
```
**“权限被拒绝”** -将目录添加到允许列表中：```bash
copilot --add-dir /path/to/directory

# Or in a session:
> /add-dir /path/to/directory
```
**上下文填满得太快**：
-更具体的文件引用
—在不同主题之间使用`/clear`-跨多个会话拆分工作</details>
---

#总结

##🔑关键要点

1. **`@`语法**给出了Copilot CLI关于文件、目录和图像的上下文
2. 随着上下文的积累，多回合对话会相互建立
3. **会话自动保存**：使用`--continue`或`--resume`来捡起你离开的地方
4. **上下文窗口**有限制：使用`/context`，`/clear`和`/compact`来管理它们
5. **权限标志** (`--add-dir`,`--allow-all`)控制多目录访问。明智地使用它们！
6. **图像引用** (`@screenshot.png`)有助于直观地调试UI问题

>📚**官方文档**:[使用Copilot CLI]（https://docs.github.com/copilot/how-tos/copilot-cli/use-copilot-cli）关于上下文，会话和使用文件的完整参考。

>📋**快速参考**：请参阅[GitHub CopilotCLI命令参考]（https://docs.github.com/en/copilot/reference/cli-command-reference）获得完整的命令和快捷方式列表。

---

##➡️下一步是什么现在您可以为Copilot CLI提供上下文，让我们将其用于实际的开发任务。您刚刚学习的上下文技术（文件引用、跨文件分析和会话管理）是下一章中强大工作流的基础。

在**[第03章：开发工作流](../03-development-workflows/)**中，您将学习：

-代码审查工作流程
-重构模式
-调试协助
-测试生成
- Git集成

---