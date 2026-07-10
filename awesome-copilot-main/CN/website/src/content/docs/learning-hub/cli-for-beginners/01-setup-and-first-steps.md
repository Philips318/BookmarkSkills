---
title: '01 · First Steps'
description: 'Experience your first GitHub Copilot CLI demos and learn the three main interaction modes.'
authors:
  - GitHub Copilot Learning Hub Team
lastUpdated: 2026-06-19
---
！[第01章：第一步]（/images/learning-hub/copilot-cli-for-beginners/01/chapter-header.png）

b> **观看AI立即发现bug，解释令人困惑的代码，并生成工作脚本。然后学习使用GitHub CopilotCLI的三种不同方法

这一章就是魔法开始的地方！您将亲身体验为什么开发人员将GitHub CopilotCLI描述为快速拨号的高级工程师。你将看到人工智能在几秒钟内发现安全漏洞，用简单的英语解释复杂的代码，并立即生成工作脚本。然后，您将掌握三种交互模式（交互，计划和编程），以便您确切地知道哪一个用于任何任务。

>⚠️**先决条件**：确保您已经完成**[第00章：快速入门](../00-quick-start/)**第一。在运行下面的演示之前，需要安装并验证GitHub CopilotCLI。

##🎯学习目标

在本章结束时，你将能够：-通过实际演示体验GitHub CopilotCLI提供的生产力提升
-为任何任务选择正确的模式（交互，计划或编程）
-使用斜杠命令来控制会话

>⏱️**预计时间**:~45分钟（15分钟阅读+ 30分钟动手）

---

#你的第一个副驾驶CLI体验<img src="/images/learning-hub/copilot-cli-for-beginners/01/first-copilot-experience.png" alt="Developer sitting at a desk with code on the monitor and glowing particles representing AI assistance" width="800"/>
直接跳进去，看看Copilot CLI能做什么。

---

##适应：你的第一个提示

在深入了解令人印象深刻的演示之前，让我们从一些简单的提示开始，您现在就可以尝试一下。**不需要代码库**！只需打开终端并启动Copilot CLI：```bash
copilot
```
试试这些初学者友好的提示：```
> Explain what a dataclass is in Python in simple terms

> Write a function that sorts a list of dictionaries by a specific key

> What's the difference between a list and a tuple in Python?

> Give me 5 best practices for writing clean Python code
```
不使用Python？没问题!只要问一些关于你选择的语言的问题。

注意它的自然感觉。就像问同事一样问问题。完成探索后，输入`/exit`以退出会话。

**关键洞察**:GitHub CopilotCLI是会话式的。您不需要特殊的语法就可以开始。用简单的英语问问题。

##看到它在行动

现在让我们看看为什么开发人员称之为“快速拨号上有一个高级工程师”。

>📖**阅读示例**：以`>`开头的行是您在交互式Copilot CLI会话中键入的提示。没有`>`前缀的行是您在终端中运行的shell命令。>💡**关于示例输出**：本课程中显示的示例输出是说明性的。因为Copilot CLI的回答每次都不同，所以你的结果在措辞、格式和细节上都会有所不同。关注返回信息的“类型”，而不是确切的文本。

###演示1：代码审查秒

本课程包括带有故意代码质量问题的示例文件。让我们回顾一下：```bash
# Clone the course repository if you're working locally and haven't already
git clone https://github.com/github/copilot-cli-for-beginners
cd copilot-cli-for-beginners

# Start Copilot
copilot
```
进入交互式会话后：```
> Review @samples/book-app-project/book_app.py for code quality issues and suggest improvements
```
>💡**`@`是什么？**`@`符号告诉副驾驶CLI读取文件。你会在第02章学到这些。现在，只需按照所示的方式复制命令。

---<details>
<summary>🎬 See it in action!</summary>
！[代码审查演示]（/images/learning-hub/copilot-cli-for-beginners/01/code-review-demo.gif）

*Demo输出不同。您的模型、工具和响应将与此处显示的有所不同</details>
---

**收获**：几秒钟内完成专业的代码审查。手动审查需要……嗯……比那还多！

---

演示2：解释令人困惑的代码

曾经盯着代码想知道它是做什么的吗？在你的副驾驶CLI会话中试试这个：```
> Explain what @samples/book-app-project/books.py does in simple terms
```

---

<details>
<summary>🎬 See it in action!</summary>
！[解释代码演示]（/images/learning-hub/copilot-cli-for-beginners/01/explain-code-demo.gif）

*Demo输出不同。您的模型、工具和响应将与此处显示的有所不同</details>
---

**发生了什么**:（你的输出会有所不同）Copilot CLI读取文件，理解代码，并用简单的英语解释它。```
This is a book collection management module using Python dataclasses.

Think of it like a digital bookshelf:
- Book is a dataclass - a simple way to store book information (title, author, year, read status)
- BookCollection manages the entire collection and handles saving/loading

Key components:

1. @dataclass decorator (line 8)
   - Automatically creates __init__, __repr__, etc.
   - Clean way to define data structures in Python

2. BookCollection class (line 16)
   - Maintains a list of Book objects
   - Handles persistence with JSON file I/O
   - load_books() reads from data.json
   - save_books() writes to data.json using asdict()

3. Book operations:
   - add_book() - creates and saves new books
   - find_book_by_title() - searches collection
   - mark_as_read() - updates read status
   - find_by_author() - filters by author name

Common pattern: Read from JSON → Work with Python objects → Write back to JSON
```
结论：像耐心的导师那样解释复杂的代码。

---

演示3：生成工作代码

需要一个你花15分钟在谷歌上搜索的功能吗？还在你的疗程中：```
> Write a Python function that takes a list of books and returns statistics: 
  total count, number read, number unread, oldest and newest book
```

---

<details>
<summary>🎬 See it in action!</summary>
！[生成代码演示]（/images/learning-hub/copilot-cli-for-beginners/01/generate-code-demo.gif）

*Demo输出不同。您的模型、工具和响应将与此处显示的有所不同</details>
---

**发生了什么**：一个完整的，在几秒钟内工作的函数，你可以复制粘贴运行。

当你完成探索后，退出这个环节。```
> /exit
```
**收获：即时满足，你会一直停留在一个连续的会话中。

---

#模式和命令<img src="/images/learning-hub/copilot-cli-for-beginners/01/modes-and-commands.png" alt="Futuristic control panel with glowing screens, dials, and equalizers representing Copilot CLI modes and commands" width="800"/>
你刚刚看到了Copilot CLI能做什么。现在让我们了解如何有效地使用这些功能。关键是知道在不同的情况下使用三种交互模式中的哪一种。

>💡**注**:Copilot CLI也有一个**自动驾驶**模式，它通过任务工作，而无需等待您的输入。它功能强大，但需要授予完全权限，并自主使用高级请求。本课程着重于以下三种模式。一旦你熟悉了基础知识，我们会教你自动驾驶仪。

---

##🧩现实世界的类比：外出就餐

想象一下使用GitHub CopilotCLI就像出去吃饭一样。从计划旅行到下订单，不同的情况需要不同的方法：

|模式|用餐类比|何时使用||------|----------------|-------------|
| **计划** | GPS路线到餐厅|复杂的任务-绘制路线，审查站点，同意计划，然后驾驶|
| **互动** |与服务员对话|探索迭代——提问、定制、获取实时反馈|
| **程序化** |免下车订购|快速，特定的任务-留在您的环境中，快速获得结果|

就像在外面吃饭一样，你自然会知道哪种方法是正确的。<img src="/images/learning-hub/copilot-cli-for-beginners/01/ordering-food-analogy.png" alt="Three Ways to Use GitHub Copilot CLI - Plan Mode (GPS route to restaurant), Interactive Mode (talking to waiter), Programmatic Mode (drive-through)" width="800"/>
*根据任务选择你的模式：计划先绘制出来，互动的来回协作，程序化的快速一次性结果*

我应该从哪个模式开始？

**从交互模式开始
-你可以尝试并提出后续问题
-通过对话自然地建立语境
-错误很容易纠正与`/clear`一旦你适应了，试试：
- **编程模式** (`copilot -p "<your prompt>"`)用于快速，一次性的问题
- **计划模式** (`/plan`)当你需要在编码之前更详细地计划事情

---

三种模式

模式1：互动模式（从这里开始）<img src="/images/learning-hub/copilot-cli-for-beginners/01/interactive-mode.png" alt="Interactive Mode - Like talking to a waiter who can answer questions and adjust the order" width="250"/>
**最适合**：探索、迭代、多回合对话。就像和服务员交谈一样，他可以回答问题，接受反馈，并随时调整订单。

启动交互式会话：```bash
copilot
```
正如您到目前为止看到的，您将看到一个提示符，您可以在其中自然地键入。要获取有关可用命令的帮助，只需键入：```
> /help
```
**关键洞察**：互动模式维持情境。每条信息都建立在前一条的基础上，就像真正的对话一样。

####交互模式示例```bash
copilot

> Review @samples/book-app-project/utils.py and suggest improvements

> Add type hints to all functions

> Make the error handling more robust

> /exit
```
注意每个提示是如何建立在前一个答案的基础上的。你们是在交谈，而不是每次都重新开始。

---

模式2：计划模式<img src="/images/learning-hub/copilot-cli-for-beginners/01/plan-mode.png" alt="Plan Mode - Like planning a route before a trip using GPS" width="250"/>
**最适合**：复杂的任务，你想在执行前检查方法。类似于在旅行前用GPS规划路线。

计划模式帮助您在编写任何代码之前创建一步一步的计划。使用`/plan`命令或按**Shift+Tab**进入计划模式：

>💡**提示**:**Shift+Tab**模式之间的循环：互动→计划→自动驾驶。在交互式会话期间，按下它可以随时切换模式，而无需键入命令。```bash
copilot

> /plan Add a "mark as read" command to the book app
```
**计划模式输出：**（您的输出可能不同）```
📋 Implementation Plan

Step 1: Update the command handler in book_app.py
  - Add new elif branch for "mark" command
  - Create handle_mark_as_read() function

Step 2: Implement the handler function
  - Prompt user for book title
  - Call collection.mark_as_read(title)
  - Display success/failure message

Step 3: Update help text
  - Add "mark" to available commands list
  - Document the command usage

Step 4: Test the flow
  - Add a book
  - Mark it as read
  - Verify status changes in list output

Proceed with implementation? [Y/n]
```
**关键洞察**：计划模式允许您在编写任何代码之前检查和修改方法。一旦计划完成，您甚至可以告诉Copilot CLI将其保存到文件中以供以后参考。例如，“将此计划保存到`mark_as_read_plan.md`”将创建一个包含计划详细信息的markdown文件。

>💡**想要更复杂的东西？**试试：`/plan Add search and filter capabilities to the book app`。计划模式从简单的功能扩展到完整的应用程序。>📚**自动驾驶模式**：你可能已经注意到Shift+Tab循环通过第三种模式称为**自动驾驶**。在自动驾驶模式下，Copilot会完成整个计划，而不会在每一步之后等待你的输入——就像把任务交给同事，然后说“完成后告诉我”。典型的工作流程是计划→接受→自动驾驶，这意味着你需要首先擅长写计划。熟悉交互式和计划模式，然后在准备好时查看[官方文档]（https://docs.github.com/copilot/concepts/agents/copilot-cli/autopilot）。

---

模式3：编程模式<img src="/images/learning-hub/copilot-cli-for-beginners/01/programmatic-mode.png" alt="Programmatic Mode - Like using a drive-through for a quick order" width="250"/>
**最适合**：自动化，脚本，CI/CD，单镜头命令。比如不用和服务员说话就可以在汽车餐厅快速点餐。

对于不需要交互的一次性命令，使用`-p`标志：```bash
# Generate code
copilot -p "Write a function that checks if a number is even or odd"

# Get quick help
copilot -p "How do I read a JSON file in Python?"
```
**关键洞察**：程序化模式让您快速回答并退出。没有对话，只是输入→输出。<details>
<summary>📚 <strong>Going Further: Using Programmatic Mode in Scripts</strong> (click to expand)</summary>
一旦你适应了，你可以在shell脚本中使用`-p`：```bash
#!/bin/bash

# Generate commit messages automatically
COMMIT_MSG=$(copilot -p "Generate a commit message for: $(git diff --staged)")
git commit -m "$COMMIT_MSG"

# Review a file
copilot --allow-all -p "Review @myfile.py for issues"
```
>⚠️**关于`--allow-all`**：此标志跳过所有权限提示，让Copilot CLI读取文件，运行命令，并访问url，而无需先询问。这对于编程模式（`-p`）是必要的，因为没有交互式会话来批准操作。只在您自己编写的提示符和您信任的目录中使用`--allow-all`。不要在不可信的输入或敏感目录中使用它。</details>
---

基本斜杠命令

这些命令在交互模式下工作。**从这六个**开始-它们涵盖了日常使用的90%：

|命令|功能|何时使用||---------|--------------|-------------|
|`/help`|显示所有可用的命令|忘记命令|
|`/clear`|清除对话并重新启动|切换主题|
|`/plan`|在编码之前计划好你的工作|对于更复杂的功能|
|`/research`|使用GitHub和web资源进行深入研究|当你需要在编码之前调查一个主题|
|`/model`|显示或切换AI模型|当需要更改AI模型|时
|`/exit`|结束会话|当您完成|

这就是开始！熟悉之后，您可以探索其他命令。

>📚**官方文档**:[CLI命令参考]（https://docs.github.com/copilot/reference/cli-command-reference）获取命令和标志的完整列表。<details>
<summary>📚 <strong>Additional Commands</strong> (click to expand)</summary>
>💡上面的基本命令涵盖了您在日常使用中要做的很多事情。当您准备探索更多内容时，此处提供参考。

代理环境

|命令|功能||---------|--------------|
|`/init`|为您的存储库|初始化副驾驶指令
|`/agent`|浏览并选择可用的代理|
|`/skills`|管理增强功能的技能|
|`/mcp`|管理MCP服务器配置|
打开一个交互式对话框，在一个地方浏览和编辑所有用户设置|

>💡技能在[第05章]（../05-skills/）中有详细介绍。MCP服务器在[第06章]（../06-mcp-servers/）中有介绍。

模型和子代理

|命令|功能||---------|--------------|
|`/model`|显示或切换AI模型|
|`/delegate`|将任务移交给GitHub上的副驾驶编码代理（云代理）|
|`/fleet`|将复杂任务拆分为并行子任务，以更快地完成|
|`/tasks`|查看后台子代理和分离的shell会话|

# # #代码

|命令|功能||---------|--------------|
|`/diff`|查看当前目录|中所做的更改
|`/pr`|对当前分支|的pull请求进行操作
|`/review`|运行代码审查代理来分析更改|
|`/research`|运行深入研究调查使用GitHub和web资源|
|`/terminal-setup`|启用多行输入支持（shift+enter和ctrl+enter） |

# # #权限

|命令|功能||---------|--------------|
|`/allow-all`|自动批准此会话的所有权限提示|
|`/add-dir <directory>`|添加目录到允许列表|
|`/list-dirs`|显示所有允许的目录|
|`/cwd`，`/cd [directory]`|查看或修改工作目录|

>⚠️**请谨慎使用**:`/allow-all`跳过确认提示。非常适合受信任的项目，但要小心不受信任的代码。

# # #会话

|命令|功能||---------|--------------|
|`/resume`|切换到另一个会话（可选指定会话ID） |
|`/rename`|重命名当前会话|
|`/context`|显示上下文窗口令牌使用和可视化|
|`/usage`|显示会话使用指标和统计信息|
|`/session`|显示会话信息和工作区摘要|
|`/compact`|总结会话以减少上下文使用|
|`/share`|导出会话为markdown文件或GitHub gist |

帮助和反馈

|命令|功能||---------|--------------|
|`/app`|直接从CLI打开GitHub应用程序（或浏览器回退）|
|`/help`|显示所有可用的命令|
|`/changelog`|显示CLI版本变更日志|
|`/feedback`|向GitHub提交反馈|
|`/theme`|查看或设置终端主题|

快速Shell命令

直接运行shell命令，不带AI，前缀为`!`：```bash
copilot

> !git status
# Runs git status directly, bypassing the AI

> !python -m pytest tests/
# Runs pytest directly
```
切换模型

Copilot CLI支持OpenAI、Anthropic、谷歌等多个AI模型。可用的模型取决于您的订阅级别和区域。使用`/model`查看选项并在它们之间切换：```bash
copilot
> /model

# Shows available models and lets you pick one. Select Sonnet 4.5.
```
>💡**提示**：一些模型比其他模型需要更多的“高级请求”。标记为**1x**的模型（如克劳德十四行诗4.5）是一个很好的默认值。他们既能干又有效率。乘数更高的模型使用额外请求配额的速度更快，所以把它们留到真正需要的时候再用。</details>
---

#实践<img src="/images/learning-hub/copilot-cli-for-beginners/01/practice.png" alt="Warm desk setup with monitor showing code, lamp, coffee cup, and headphones ready for hands-on practice" width="800"/>
是时候把你学到的东西付诸行动了。

---

##▶️自己试试

互动探索

启动Copilot并使用后续提示来迭代改进图书应用程序：```bash
copilot

> Review @samples/book-app-project/book_app.py - what could be improved?

> Refactor the if/elif chain into a more maintainable structure

> Add type hints to all the handler functions

> /exit
```
###计划一个特性

在编写任何代码之前，使用`/plan`让Copilot CLI绘制实现图：```bash
copilot

> /plan Add a search feature to the book app that can find books by title or author

# Review the plan
# Approve or modify
# Watch it implement step by step
```
###自动化与编程模式`-p`标志允许您直接从终端运行Copilot CLI，而无需进入交互模式。从存储库根目录复制并粘贴以下脚本到您的终端（不是在Copilot中），以查看book应用程序中的所有Python文件。```bash
# Review all Python files in the book app
for file in samples/book-app-project/*.py; do
  echo "Reviewing $file..."
  copilot --allow-all -p "Quick code quality review of @$file - critical issues only"
done
```
* * PowerShell (Windows): * *```powershell
# Review all Python files in the book app
Get-ChildItem samples/book-app-project/*.py | ForEach-Object {
  $relativePath = "samples/book-app-project/$($_.Name)";
  Write-Host "Reviewing $relativePath...";
  copilot --allow-all -p "Quick code quality review of @$relativePath - critical issues only" 
}
```
---

完成演示后，请尝试以下变体：

1. **互动挑战**：启动`copilot`并探索图书应用程序。询问`@samples/book-app-project/books.py`并连续3次要求改进。

2. **计划模式挑战**：运行`/plan Add rating and review features to the book app`。仔细阅读计划。这有意义吗？

3. **编程挑战**：运行`copilot --allow-all -p "List all functions in @samples/book-app-project/book_app.py and describe what each does"`。第一次尝试成功了吗？

---

##📝作业

主要挑战：改进图书应用工具

实际操作的示例侧重于审查和重构`book_app.py`。现在在另一个文件`utils.py`上练习相同的技巧：1. 启动交互式会话：`copilot`2. 要求副驾驶命令行总结文件：`@samples/book-app-project/utils.py What does each function in this file do?`3. 要求它添加输入验证：“将验证添加到`get_user_choice()`，以便它处理空输入和非数字项”
4. 要求它改进错误处理：“如果`get_book_details()`收到标题的空字符串会发生什么？”为此增加警卫。”
5. 请求一个文档字符串：“向`get_book_details()`添加一个包含参数描述和返回值的综合文档字符串”
6. 观察上下文如何在提示之间传递。每次改进都建立在上一次改进的基础上
7. 使用`/exit`退出

**成功标准**：您应该有一个改进的`utils.py`，具有输入验证、错误处理和文档字符串，所有这些都通过多回合对话构建。<details>
<summary>💡 Hints (click to expand)</summary>
**示例提示：**```bash
> @samples/book-app-project/utils.py What does each function in this file do?
> Add validation to get_user_choice() so it handles empty input and non-numeric entries
> What happens if get_book_details() receives an empty string for the title? Add guards for that.
> Add a comprehensive docstring to get_book_details() with parameter descriptions and return values
```
共同问题:* * * *
-如果副驾驶CLI询问澄清问题，请自然回答
-上下文向前推进，因此每个提示都建立在前一个提示的基础上
-使用`/clear`如果你想重新开始</details>
奖励挑战：比较模式

示例使用`/plan`作为搜索特性，`-p`用于批处理审查。现在在一个新任务上尝试这三种模式：向`BookCollection`类添加一个`list_by_year()`方法：

1. **交互式**:`copilot`→让它一步一步地设计和构建方法
2. * * * *计划:`/plan Add a list_by_year(start, end) method to BookCollection that filters books by publication year range`3. * * * *编程:`copilot --allow-all -p "@samples/book-app-project/books.py Add a list_by_year(start, end) method that returns books published between start and end year inclusive"`**反思**：哪种模式感觉最自然？你会在什么时候使用它们？

---<details>
<summary>🔧 <strong>Common Mistakes & Troubleshooting</strong> (click to expand)</summary>
常见错误

|错误|发生了什么|修复||---------|--------------|-----|
|输入`exit`而不是`/exit`| Copilot CLI将“exit”视为提示符，而不是命令|斜杠命令总是以`/`|开头
|使用`-p`进行多轮会话|每个`-p`呼叫都是隔离的，没有以前呼叫的内存|使用交互模式（`copilot`）进行基于上下文|的会话
|`$`或`!`| Shell在Copilot CLI看到特殊字符之前解释它们|用引号括起提示：`copilot -p "What does $HOME mean?"`|

# # #故障排除

**“型号不可用”** -您的订阅可能不包括所有型号。使用`/model`查看可用的内容。

“上下文太长”-你的对话已经使用了完整的上下文窗口。使用`/clear`重置或启动新会话。

**“速率限制已超过”** -等待几分钟后再试一次。考虑对有延迟的批处理操作使用编程模式。</details>
---

#总结

##🔑关键要点

1. **互动模式**是为了探索和迭代-上下文发扬。这就像和一个记得你说过的话的人交谈一样。
2. **计划模式**通常用于更复杂的任务。实施前审查。
3. **编程模式**为自动化模式。不需要交互。
4. **四个基本命令** (`/help`,`/clear`,`/plan`,`/exit`)涵盖了大多数日常使用。

>📋**快速参考**：请参阅[GitHub CopilotCLI命令参考]（https://docs.github.com/en/copilot/reference/cli-command-reference）以获得完整的命令和快捷方式列表。

---

##➡️下一步是什么

现在你了解了这三种模式，让我们学习如何给Copilot CLI上下文关于你的代码。

在**[第02章：语境和对话](../02-context-and-conversations/)**中，你将学到：—引用文件和目录的`@`语法
—使用`--resume`和`--continue`进行会话管理
-上下文管理如何使Copilot CLI真正强大

---