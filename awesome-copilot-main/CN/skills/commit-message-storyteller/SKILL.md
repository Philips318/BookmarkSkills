---
name: commit-message-storyteller
description: 'Analyzes git diffs or staged changes and generates narrative commit messages that explain WHY a change was made, not just what changed — following Conventional Commits format. Use when asked to "write a commit message", "generate a commit", "describe my changes", "what should I commit this as", "commit this", "summarize my diff", or "help me commit". Works with git diff output, staged files, or plain descriptions of changes.'
---
#提交消息故事讲述者

将原始的git差异和变更描述转换为遵循[常规提交]（https://www.conventionalcommits.org/）规范的清晰的、故事驱动的提交消息。您得到的不是“更新file.js”，而是传达意图、上下文和影响的消息。

何时使用此技能

-用户说“写提交消息”，“帮我提交”，或“生成提交”
-用户粘贴git diff或描述代码更改
-用户说“我应该提交什么？”或“总结我的差异”
-用户希望更好的提交历史为他们的团队或开源项目
-用户正在准备拉取请求，并希望有意义的提交消息

# #先决条件

至少准备好以下其中一项：
—`git diff`或`git diff --staged`输出
-描述你改变了什么以及为什么
-修改文件列表

##如何工作

步骤1：收集变更上下文询问用户（或从diff推断）：

1. **什么改变了** -文件，函数，逻辑受影响
2. **为什么改变**——bug修复，新特性，重构，性能，等等。
3. **Who/what触发了它** -发行号，用户请求，技术债务等。

如果用户提供了原始的`git diff`，则自动从diff中提取该上下文。

###步骤2：确定提交类型

使用以下指南将更改映射到常规提交类型：

|类型|当|时使用|------|----------|
|`feat`|新增特性或功能|
|`fix`|错误或错误行为被纠正|
|`refactor`|代码重组没有改变行为|
|`perf`|性能提升|
|`docs`|文档只更改|
|`style`|格式化，空白，缺少分号（没有逻辑更改）|
|`test`|添加或更新测试|
|`chore`|构建过程，依赖更新，配置更改|
|`ci`|CI/CD管道更改|
|`revert`|恢复之前提交的|

有关详细示例，请参见`references/conventional-commits-guide.md`。

###步骤3：写提交消息

遵循以下结构：```
<type>(<optional scope>): <short imperative summary>

<body — the story: why this change was made, what problem it solves>

<footer — issue refs, breaking change notices>
```
####各部件规则

**主题（第一行）：**
使用祈使语气：“add”，“fix”，“remove”，而不是“added”或“fixes”。
-最多72个字符
—末尾没有句号
—冒号后小写

正文（故事）：**
-解释“为什么”，而不是“什么”（差异已经显示了“什么”）
-描述在此更改之前存在的问题
-如果相关，请提及考虑的任何替代方案
—每行不超过100个字符
-用空行与主题分开

* *页脚:* *
—参考号：`Closes #123`、`Fixes #456`、`Refs #789`-标记打破更改：`BREAKING CHANGE: <description>`###步骤4：生成输出

在一个可复制的代码块中生成提交消息，然后用一行简单的英文解释您所讲的故事。

* *输出示例:* *```
fix(auth): prevent token refresh loop on expired sessions

When a user's session expired mid-request, the auth middleware was
triggering a token refresh, which itself failed validation and triggered
another refresh — causing an infinite retry loop that crashed the app.

This adds a recursion guard flag that aborts the refresh cycle if a
refresh is already in progress, returning a clean 401 instead.

Closes #312
```
> **故事：**会话到期时无声无限循环导致应用崩溃；这将提前停止循环并返回一个干净的错误。

---

##从一个Diff多次提交

如果diff包含逻辑上独立的更改，将它们分割成多个提交消息并告诉用户。使用这个启发式：

-不同的文件不相关的目的→可能单独提交
-相同的文件，但不同的关注点（例如，bug修复+重构）→建议拆分
-一切紧密耦合→一次提交是好的

---

##边缘情况

|情况|如何处理||-----------|---------------|
|用户不提供上下文，除了diff |从文件名和更改的符号|中推断类型和范围
|问：“这是一个逻辑更改，还是多个？”|
|自动添加`BREAKING CHANGE:`页脚
|用户说“保持简短”|省略主体，只写一个强有力的主题行|
|无发行号|完全省略页脚|

---

##快速参考```bash
# Get your staged diff to paste into Copilot
git diff --staged

# Or get the last uncommitted working tree changes
git diff
```
请参阅`references/conventional-commits-guide.md`了解类型示例和范围指南。