---
title: '07 · Putting It All Together'
description: 'Mirror the source capstone chapter that combines the GitHub Copilot CLI workflow end to end.'
authors:
  - GitHub Copilot Learning Hub Team
lastUpdated: 2026-03-20
---
！[第七章：把所有的东西放在一起]（/images/learning-hub/copilot-cli-for-beginners/07/chapter-header.png）

你所学到的一切在这里结合起来。在一次会议中完成从想法到合并PR的过程

在本章中，你将把你所学到的一切整合到完整的工作流程中。您将使用多代理协作构建功能，设置预提交钩子，在提交之前捕获安全问题，将Copilot集成到CI/CD管道中，并在单个终端会话中从功能想法到合并PR。这就是GitHub CopilotCLI成为真正的力量倍增器的地方。

>💡**注**：本章展示了如何将你所学的一切结合起来。**你不需要代理、技能或MCP来提高效率（尽管它们可能非常有帮助）。**核心工作流程-描述，计划，实施，测试，审查，发布-仅使用第00-03章的内置功能。

##🎯学习目标在本章结束时，你将能够：

—将agent、技能和MCP （Model Context Protocol，模型上下文协议）合并为统一的工作流
-使用多工具方法构建完整的功能
-设置基本自动化与挂钩
-为专业发展应用最佳实践

>⏱️**预计时间**:~75分钟（15分钟阅读+ 60分钟动手）

---

##🧩现实世界的类比：管弦乐队<img src="/images/learning-hub/copilot-cli-for-beginners/07/orchestra-analogy.png" alt="Orchestra Analogy - Unified Workflow" width="800"/>
交响乐团有很多部分：
- **字符串**提供基础（像你的核心工作流）
- **黄铜**增加力量（像具有专业知识的代理）
- **木管乐器**添加颜色（像技能扩展能力）
- **敲击**保持节奏（如MCP连接到外部系统）

单独来看，每个部分听起来都很有限。如果指挥得当，它们就能创造出宏伟的作品。

**这就是本章所教的！* *<br>*就像指挥管弦乐队一样，您将座席、技能和MCP编排到统一的工作流程中*

让我们从一个场景开始：修改代码、生成测试、审查代码并创建PR——所有这些都在一个会话中完成。

---

在一个会话中合并PR的想法每次在编辑器、终端、测试运行器和GitHub UI之间切换并丢失上下文，您可以将所有工具组合在一个终端会话中。我们将在下面的[集成模式]（#the- Integration -pattern-for-power-users）一节中分解这个模式。```bash
# Start Copilot in interactive mode
copilot

> I need to add a "list unread" command to the book app that shows only
> books where read is False. What files need to change?

# Copilot creates high-level plan...

# SWITCH TO PYTHON-REVIEWER AGENT
> /agent
# Select "python-reviewer"

> @samples/book-app-project/books.py Design a get_unread_books method.
> What is the best approach?

# Python-reviewer agent produces:
# - Method signature and return type
# - Filter implementation using list comprehension
# - Edge case handling for empty collections

# SWITCH TO PYTEST-HELPER AGENT
> /agent
# Select "pytest-helper"

> @samples/book-app-project/tests/test_books.py Design test cases for
> filtering unread books.

# Pytest-helper agent produces:
# - Test cases for empty collections
# - Test cases with mixed read/unread books
# - Test cases with all books read

# IMPLEMENT
> Add a get_unread_books method to BookCollection in books.py
> Add a "list unread" command option in book_app.py
> Update the help text in the show_help function

# TEST
> Generate comprehensive tests for the new feature

# Multiple tests are generated similar to the following:
# - Happy path (3 tests) — filters correctly, excludes read, includes unread
# - Edge cases (4 tests) — empty collection, all read, none read, single book
# - Parametrized (5 cases) — varying read/unread ratios via @pytest.mark.parametrize
# - Integration (4 tests) — interplay with mark_as_read, remove_book, add_book, and data integrity

# Review the changes
> /review

# If review passes, use /pr to operate on the pull request for the current branch
> /pr [view|create|fix|auto]

# Or ask naturally if you want Copilot to draft it from the terminal
> Create a pull request titled "Feature: Add list unread books command"
```
**传统方法：在编辑器、终端、测试运行器、文档和GitHub UI之间切换。每次转换都会导致上下文丢失和摩擦。

关键观点：你像建筑师一样指导专家。他们处理细节。你处理了幻象。

>💡**进一步**：对于像这样的大型多步骤计划，尝试`/fleet`让Copilot并行运行独立的子任务。请参阅[官方文档]（https://docs.github.com/copilot/concepts/agents/copilot-cli/fleet）了解详细信息。

---

#其他工作流程<img src="/images/learning-hub/copilot-cli-for-beginners/07/combined-workflows.png" alt="People assembling a colorful giant jigsaw puzzle with gears, representing how agents, skills, and MCP combine into unified workflows" width="800"/>
对于完成了第04-06章的高级用户，这些工作流展示了代理、技能和MCP如何提高你的效率。

集成模式

下面是把所有东西结合起来的心智模型：<img src="/images/learning-hub/copilot-cli-for-beginners/07/integration-pattern.png" alt="The Integration Pattern - A 4-phase workflow: Gather Context (MCP), Analyze and Plan (Agents), Execute (Skills + Manual), Complete (MCP)" width="800"/>
---

工作流程1:Bug调查和修复

真实世界的bug修复与完整的工具集成：```bash
copilot

# PHASE 1: Understand the bug from GitHub (MCP provides this)
> Get the details of issue #1

# Learn: "find_by_author doesn't work with partial names"

# PHASE 2: Research best practice (deep research with web + GitHub sources)
> /research Best practices for Python case-insensitive string matching

# PHASE 3: Find related code
> @samples/book-app-project/books.py Show me the find_by_author method

# PHASE 4: Get expert analysis
> /agent
# Select "python-reviewer"

> Analyze this method for issues with partial name matching

# Agent identifies: Method uses exact equality instead of substring matching

# PHASE 5: Fix with agent guidance
> Implement the fix using lowercase comparison and 'in' operator

# PHASE 6: Generate tests
> /agent
# Select "pytest-helper"

> Generate pytest tests for find_by_author with partial matches
> Include test cases: partial name, case variations, no matches

# PHASE 7: Commit and PR
> Generate a commit message for this fix

> Create a pull request linking to issue #1
```
---

工作流2：代码评审自动化（可选）

>💡**可选。**预提交钩子对团队来说很有用，但不是必须的。如果你刚刚开始，跳过这个。
>
>⚠️**性能说明**：该钩子为每个暂存文件调用`copilot -p`，每个文件需要几秒钟。对于大型提交，请考虑限制关键文件或使用`/review`手动运行审查。

git钩子是git在特定时刻自动运行的脚本，例如，在提交之前。您可以使用它对代码运行自动检查。以下是如何在提交上设置自动的Copilot审查：```bash
# Create a pre-commit hook
cat > .git/hooks/pre-commit << 'EOF'
#!/bin/bash

# Get staged files (Python files only)
STAGED=$(git diff --cached --name-only --diff-filter=ACM | grep -E '\.py$')

if [ -n "$STAGED" ]; then
  echo "Running Copilot review on staged files..."

  for file in $STAGED; do
    echo "Reviewing $file..."

    # Use timeout to prevent hanging (60 seconds per file)
    # --allow-all auto-approves file reads/writes so the hook can run unattended.
    # Only use this in automated scripts. In interactive sessions, let Copilot ask for permission.
    REVIEW=$(timeout 60 copilot --allow-all -p "Quick security review of @$file - critical issues only" 2>/dev/null)

    # Check if timeout occurred
    if [ $? -eq 124 ]; then
      echo "Warning: Review timed out for $file (skipping)"
      continue
    fi

    if echo "$REVIEW" | grep -qi "CRITICAL"; then
      echo "Critical issues found in $file:"
      echo "$REVIEW"
      exit 1
    fi
  done

  echo "Review passed"
fi
EOF

chmod +x .git/hooks/pre-commit
```
>⚠️**macOS用户**:macOS默认不包含`timeout`命令。用`brew install coreutils`安装它，或者用不带超时保护的简单调用替换`timeout 60`。

>📚**官方文档**:[使用hooks]（https://docs.github.com/copilot/how-tos/copilot-cli/use-hooks）和[hooks配置参考]（https://docs.github.com/copilot/reference/hooks-configuration）用于完整的hooks API。
>
>💡**内置替代方案**:Copilot CLI也有一个内置钩子系统（`copilot hooks`），可以在预提交等事件上自动运行。上面的手动git钩子给了你完全的控制权，而内置的系统更容易配置。请参阅上面的文档，以确定哪种方法适合您的工作流程。

现在，每次提交都会得到一个快速的安全审查：```bash
git add samples/book-app-project/books.py
git commit -m "Update book collection methods"

# Output:
# Running Copilot review on staged files...
# Reviewing samples/book-app-project/books.py...
# Critical issues found in samples/book-app-project/books.py:
# - Line 15: File path injection vulnerability in load_from_file
#
# Fix the issue and try again.
```
---

##工作流程3：登录到新的代码库

当加入一个新项目时，将上下文、代理和MCP结合起来，以快速提升：```bash
# Start Copilot in interactive mode
copilot

# PHASE 1: Get the big picture with context
> @samples/book-app-project/ Explain the high-level architecture of this codebase

# PHASE 2: Understand a specific flow
> @samples/book-app-project/book_app.py Walk me through what happens
> when a user runs "python book_app.py add"

# PHASE 3: Get expert analysis with an agent
> /agent
# Select "python-reviewer"

> @samples/book-app-project/books.py Are there any design issues,
> missing error handling, or improvements you would recommend?

# PHASE 4: Find something to work on (MCP provides GitHub access)
> List open issues labeled "good first issue"

# PHASE 5: Start contributing
> Pick the simplest open issue and outline a plan to fix it
```
这个工作流将`@`上下文、代理和MCP结合到一个单一的登录会话中，这正是本章前面提到的集成模式。

---

#最佳实践和自动化

使你的工作流程更有效的模式和习惯。

---

最佳实践

# # # 1。先从上下文开始，再分析

在要求分析之前一定要收集背景信息：```bash
# Good
> Get the details of issue #42
> /agent
# Select python-reviewer
> Analyze this issue

# Less effective
> /agent
# Select python-reviewer
> Fix login bug
# Agent doesn't have issue context
```
# # # 2。了解区别：代理，技能和自定义说明

每种工具都有一个最佳点：```bash
# Agents: Specialized personas you explicitly activate
> /agent
# Select python-reviewer
> Review this authentication code for security issues

# Skills: Modular capabilities that auto-activate when your prompt
# matches the skill's description (you must create them first — see Ch 05)
> Generate comprehensive tests for this code
# If you have a testing skill configured, it activates automatically

# Custom instructions (.github/copilot-instructions.md): Always-on
# guidance that applies to every session without switching or triggering
```
>💡**关键点**：代理和技能都可以分析和生成代码。真正的区别在于它们如何激活——代理是显式的（`/agent`），技能是自动的（提示匹配），并且自定义指令总是打开的。

# # # 3。保持会议的重点

使用`/rename`来标记会话（使其易于在历史中找到），并使用`/exit`来干净地结束会话：```bash
# Good: One feature per session
> /rename list-unread-feature
# Work on list unread
> /exit

copilot
> /rename export-csv-feature
# Work on CSV export
> /exit

# Less effective: Everything in one long session
```
# # # 4。使用Copilot使工作流可重用

而不是仅仅在wiki中记录工作流，直接将它们编码到您的repo中，Copilot可以使用它们：

- **自定义指令** (`.github/copilot-instructions.md`)：编码标准、架构规则和build/test/deploy步骤的始终在线指导。每个会话都自动遵循它们。
- **提示文件** (`.github/prompts/`)：可重用的、参数化的提示，您的团队可以共享类似于代码审查、组件生成或PR描述的模板。
- **定制代理** (`.github/agents/`)：编码专门的角色（例如，安全审稿人或文档作者），团队中的任何人都可以使用`/agent`激活。
- **自定义技能** (`.github/skills/`)：包一步一步的工作流程说明，自动激活时相关。

>💡**回报**：新团队成员免费获得您的工作流程-它们被内置到回购中，而不是锁定在某人的头脑中。

---奖励：生产模式

这些模式是可选的，但对于专业环境很有价值。

PR描述生成器```bash
# Generate comprehensive PR descriptions
BRANCH=$(git branch --show-current)
COMMITS=$(git log main..$BRANCH --oneline)

copilot -p "Generate a PR description for:
Branch: $BRANCH
Commits:
$COMMITS

Include: Summary, Changes Made, Testing Done, Screenshots Needed"
```
CI/CD集成

对于拥有现有CI/CD管道的团队，您可以使用GitHub Actions对每个拉取请求进行自动Copilot审查。这包括自动发布审查评论和过滤关键问题。

>📖**了解更多信息**：请参阅[CI/CD集成]（https://github.com/github/copilot-cli-for-beginners/blob/main/appendices/ci-cd-integration.md）以获取完整的GitHub Actions工作流、配置选项和故障排除提示。

---

#实践<img src="/images/learning-hub/copilot-cli-for-beginners/07/practice.png" alt="Warm desk setup with monitor showing code, lamp, coffee cup, and headphones ready for hands-on practice" width="800"/>
将完整的工作流程付诸实践。

---

##▶️自己试试

完成演示后，请尝试以下变体：

1. **端到端挑战**：选择一个小功能（例如，“列出未读书籍”或“导出到CSV”）。使用完整的工作流：
—规划为`/plan`-使用代理进行设计（python-reviewer, pytest-helper）
——实现
-生成测试
创建PR

2. **自动化挑战**：从代码审查自动化工作流中设置预提交钩子。使用故意的文件路径漏洞进行提交。它被阻塞了吗？

3. **您的生产工作流程**：为您做的常见任务设计自己的工作流程。把它作为清单写下来。哪些部分可以通过技能、代理或挂钩实现自动化？

**自检**：当您完成课程时，您可以向同事解释代理，技能和MCP如何协同工作-以及何时使用它们。

---

##📝作业主要挑战：端到端特性

实践示例介绍了如何构建“未读书籍列表”功能。现在在一个不同的功能上练习完整的工作流程：**按年份范围搜索书籍**：

1. 启动副驾驶并收集上下文：`@samples/book-app-project/books.py`2. 用`/plan Add a "search by year" command that lets users find books published between two years`规划
3. 在`BookCollection`中实现`find_by_year_range(start_year, end_year)`方法
4. 在`book_app.py`中添加一个`handle_search_year()`函数，该函数提示用户输入起始年和结束年
5. 生成测试：`@samples/book-app-project/books.py @samples/book-app-project/tests/test_books.py Generate tests for find_by_year_range() including edge cases like invalid years, reversed range, and no results.`6. 用`/review`复习
7. 更新README:`@samples/book-app-project/README.md Add documentation for the new "search by year" command.`8. 生成提交消息

记录你的工作流程。

**成功标准**：您已经使用Copilot CLI完成了从想法到提交的功能，包括计划，实现，测试，文档和审查。>💡**奖励**：如果你有从第04章设置代理，尝试创建和使用自定义代理。例如，用于检查实现的错误处理程序代理和用于README更新的文档编写器代理。<details>
<summary>💡 Hints (click to expand)</summary>
遵循本章开头的“创意到合并PR”（# Idea -to- Merged PR -in-one-session）示例的模式。关键步骤是：

1. 使用`@samples/book-app-project/books.py`收集上下文
2. 用`/plan Add a "search by year" command`规划
3. 实现方法和命令处理程序
4. 生成带有边缘情况的测试（无效输入、空结果、反向范围）
5. 用`/review`复习
6. 用`@samples/book-app-project/README.md`更新README
7. 使用`-p`生成提交消息

需要考虑的边缘情况：**
—如果用户输入“2000”和“1990”（反向范围）会怎样？
-如果没有符合这个范围的书怎么办？
-如果用户输入非数字输入怎么办？

**关键是练习完整的工作流程**从想法→环境→计划→实现→测试→文档→提交。</details>

---

<details>
<summary>🔧 <strong>Common Mistakes</strong> (click to expand)</summary>
|错误|发生了什么|修复||---------|--------------|-----|
|直接跳到实现|忽略以后修复成本很高的设计问题|首先使用`/plan`来考虑方法|
|组合：分析代理→执行技能→集成MCP |
总是运行`/review`或使用[预提交钩子](#workflow-2-code-review-automation-optional) |
|忘记与团队共享工作流|每个人都重新发明轮子|共享代理、技能和说明中的文档模式|</details>
---

#总结

##🔑关键要点

1. **集成>隔离**：组合工具以获得最大影响
2. **上下文优先**：在分析之前总是收集必要的上下文
3. **代理分析，技能执行**：为工作使用正确的工具
4. **自动重复**：钩子和脚本增加你的效率
5. 文档工作流：可共享的模式使整个团队受益

>📋**快速参考**：请参阅[GitHub CopilotCLI命令参考]（https://docs.github.com/en/copilot/reference/cli-command-reference）获得完整的命令和快捷方式列表。

---

##🎓课程完成！

恭喜你!你学到的:

b|章你学到了什么|---------|-------------------|
|00 | Copilot CLI安装和快速入门|
|1 |三种交互模式|
|2 |使用@语法进行上下文管理
|3 |开发工作流|
|0 04 |专业代理|
|0 05 |可扩展技能|
|0 06 |与MCP |外部连接
统一的生产工作流程

现在，您可以将GitHub CopilotCLI用作开发工作流程中的真正力量倍增器。

##➡️下一步是什么

你的学习不止于此：

1. **每天练习**：使用Copilot CLI进行实际工作
2. **构建自定义工具**：根据您的特定需求创建代理和技能
3. **分享知识**：帮助您的团队采用这些工作流
4. **保持更新**：关注GitHub Copilot更新的新功能

# # #资源

- [GitHub Copilot命令行文档]（https://docs.github.com/copilot/concepts/agents/about-copilot-cli）
- [MCP服务器注册表]（https://github.com/modelcontextprotocol/servers）
-[社区技能]（https://github.com/topics/copilot-skill）

---

* *好!现在去创造一些令人惊叹的东西吧