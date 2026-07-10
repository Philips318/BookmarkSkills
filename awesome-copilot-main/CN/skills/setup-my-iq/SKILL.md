---
name: setup-my-iq
description: |
  Create, set up, or update the personal context portfolio: structured markdown files describing
  who you are, how you work, your teams, and your tool/ADO configuration. Runs the interview
  workflow for first-time setup and targeted edits for updates.

  Trigger this skill when the user asks to: set up their context, create or update their context
  portfolio, "create my IQ", "set up my IQ", edit their profile, add/remove a stakeholder,
  update ADO config, change team info, update pillars, or set up any plugin configuration.
  Trigger when another skill fails to find context (missing files or TODO markers) and needs
  context populated. Also trigger when the user mentions a context change in passing
  (e.g., "my manager changed", "we added someone to the team") to offer a context file update.

  Do NOT trigger for read-only questions like "who's on my team?" or "what's my ADO config?".
  Those are answered directly from the context files referenced in the loaded custom
  instructions; no skill is needed.
---
#设置我的智商：创建和更新上下文组合

创建并维护个人上下文组合：一组结构化的降价
这些文件代表了你是谁，你是如何工作的，以及什么对你来说很重要。的文件
可以存在磁盘上的任何地方（OneDrive文件夹，git repo，本地目录等）。
将它们与代理联系在一起的是用户自定义指令中的指针
文件（`AGENTS.md`、`copilot-instructions.md`、`CLAUDE.md`或任何`*.instructions.md`）
主机加载到会话中的数据)。

##这个技能是如何工作的

调用时，确定当前状态并采取正确的操作。

发现现有的上下文扫描会话以查找对该技能创建的文件的引用。主持人有
已经将所有相关的自定义指令文件合并到会话中：`AGENTS.md`（工作区或用户范围），`copilot-instructions.md`，`CLAUDE.md`、任何`*.instructions.md`文件，以及类似的特定于主机的等效文件。
指针来自哪个文件并不重要，只要查找文件名即可：- `identity.md`
- `role-and-responsibilities.md`
- `team.md`
- `tools-systems-and-config.md`
- `communication-style.md`
- `preferences-and-constraints.md`
对于每一个，在任何地方找到一个`@<absolute-path>`（或等效的路径引用）
加载的指令文本。这是文件的位置。不要依赖标签
或者路径旁边的变量名：用户可以对指针进行不同的命名
（`identityProfile`、`me`、`who-i-am`等）。匹配最后的文件名
在小路上。

对于每个文件名，分类如下：

- **未引用** -在会话中不加载此文件名的路径。
- **引用，文件丢失** -加载路径，但文件在磁盘上不存在。
- **PRESENT** -加载路径，文件存在。阅读它来检查占位符。

如果这六个文件名处于混合状态，则按以下优先级顺序处理它们：
首先通过面试创建任何未被引用的文件，然后填写引用。
文件缺少条目，然后修复PRESENT文件中的不完整字段。总结
在开始之前将组合状态告知用户，以便他们知道将会发生什么。具体的例子。假设`identity.md`和`team.md`是PRESENT和
完成后，`tools-systems-and-config.md`是PRESENT，但有TODO占位符
在ADO部分，`role-and-responsibilities.md`和`communication-style.md`被引用，但文件在磁盘上不存在，并且`preferences-and-constraints.md`未被引用。技能应该告诉
用户：“你的身份和团队都完成了。Tools-config有缺陷。角色和
沟通风格被引用但缺失。并没有设置Preferences
所有人。我会先面试你的喜好，然后创建角色和
沟通风格，然后填补工具配置中的空白。好吗?”

# # # 1。没有引用上下文文件
在加载的指令中没有任何期望的文件名出现。
将此视为第一次设置。运行**First-Time Setup**。# # # 2。一些文件被引用但在磁盘上丢失
用户先前开始安装但没有完成，或者路径没有完成
他们身后是文件。**在他们离开的地方继续简历：**
-列出哪些文件存在，哪些文件缺失。
-告诉用户：“你有{现有文件}。”仍然需要：{丢失的文件}。想要
接着你离开的地方？”
-在选择下一个要面试的文件时，优先顺序
*发现现有的上下文*仍然适用。在平局中，使用这个
默认顺序为：identity -> role -> team -> tools -> communication-style
- > preferences-and-constraints。

# # # 3。文件存在，但有不完整的字段

文件存在，但有些文件包含未填充的占位符值。* *填补空白:* *

-扫描所有引用的文件，以查找这些不完整字段模式：
-`<!-- TODO -->`-显式占位符
-使用格式为`<!-- ... -->`的HTML注释作为替代值    (e.g., `<!-- your name -->`, `<!-- org name -->`, `<!-- manager name -->`)
-任何仅以HTML注释为内容的表格单元格或字段
-列出空白：“我在{文件}中发现不完整的字段。想填一下吗？”
-对于每个空缺，问一个特定领域的问题（而不是整个面试）。
-用用户的答案更新文件。

# # # 4。文件已完成，用户想要更新
用户要求更改某些内容，或者顺便提到上下文更改
例如，“我的经理换了”，“我们雇了一个新员工”，“我搬到另一个地方了。
团队”)。* *有针对性的更新:* *
-如果用户在另一个任务中提到了变化，提供：“它
听起来你的{文件}可能需要更新。要我现在修吗？”
-如果用户明确要求，直接进行。
—从*发现现有上下文*解析的路径中读取文件。
-进行编辑并与用户确认。# # # 5。文件已完成，用户想要完全刷新
用户想从头开始重做一个文件。重新播放一下面试
按照与首次设置相同的步骤创建特定文件。

---

##首次设置

这个技能中的所有`assets/...`路径都相对于这个SKILL.md路径
目录中。使用文件读取工具从这些文件中加载绑定的模板
路径。如果由于某种原因模板不可读，则退回到该字段
结构，如本文件底部的示例演练所示。

# # # 1。选择上下文目录

问：“你的上下文文件应该放在哪里？”任何持久的工作：一个同步
云文件夹（OneDrive, Dropbox, iCloud），本地目录或git repo。只是
选择一条你以后还能找到的路。”—如果目录不存在，请创建该目录。
—为后续步骤保存路径。
—在macOS或Linux上，建议使用“`~/my-iq-context`”或“`~/OneDrive/my-iq-context`”
默认值。用`$HOME`代替`%USERPROFILE%`在任意路径中的技能
以后产生的。

# # # 2。开始面试

流程介绍：

“我要面试你，并制作你的个人背景作品集：一套
你是谁，你的工作方式，以及什么是重要的
给你。任何AI技能，代理或插件都可以读取这些文件并立即
了解它是如何工作的。

我们一次看一个文件。我会问你问题，起草文件，然后
我要你告诉我哪里做错了。你可以跳过任何你不想要的文件
现在。准备好开始了吗？”

# # # 3。从可用的数据源预填充事实字段

**在问任何面试问题之前，这一步是必须的。**不要跳过
它，即使你期望它不返回任何东西。在询问用户任何事情之前，尝试从任何事情中收集事实数据
环境提供的连接源。用户不应该这样做
输入连接的系统已经知道的信息。

**查找内容（每个文件）：**

|文件|事实字段预填充||------|---------------------------|
|`identity.md`|名称，role/title，组织，团队，经理|
|`role-and-responsibilities.md`|支持的团队，节奏（从日历），报告行|
直接下属，经常性合作者，组织结构图数据|
|`tools-systems-and-config.md`| ADOorg/project/team/area路径，repos，工具在使用|
|`communication-style.md`|从mail/docs写入示例（先征得许可）|
没有预填充（这些是主观的界限）|

**如何执行：**1. 检查您的环境中有哪些数据源可用(工作配置文件、
目录、日历、ADO、邮件等)。仅使用已授权的资源。
2. 查询您要创建的文件的相关事实字段
面试。
3. 以提议的形式呈现发现：“我从你的工作中提取了一些信息
概要文件。这是我发现的——在我们继续之前确认或纠正。”
4. 将每个值标记为自动填充，以便用户知道来自系统的内容
和你要求他们提供的。
5. 只有在预填建议得到确认后（或者如果没有来源）
（可用），接着问剩下的问题。

**如果没有可用的数据源：**明确声明（“I don't have .”）
在这种环境下访问工作配置文件或目录，所以我会问你
直接”)，然后继续回答面试问题。不要默默地跳过这个
步骤。参见*Pre-fill中可用数据的完整规则和约束
根据《面试规则》的规定，在问问题之前。

# # # 4。identity.md的面试

从这个技能包中的`assets/templates/identity.md`加载模板
文件结构。

**首先：应用步骤3（预填充）。**查询姓名、角色、
组织、团队和经理。提出任何发现以供确认。然后进行
对于以下未包含在预填写中的字段的问题。

要问的开放式问题（这些问题总是需要用户）：

如果你必须在晚宴上向某人解释你实际做了什么，你会说什么？
-人们来找你干什么？

只有在预填写没有涵盖的情况下才问事实性问题：

-你的名字和目前的角色是什么？
-你在哪个组织或公司工作？
-你是哪一队的？
-你的经理是谁？一旦你有了足够的草稿（超过这个点就不要再问了），起草
文件。附上面试官的反应表（见下面的面试规则）。修改,
然后写入上下文目录。

过渡：“这就是身份。接下来是角色和责任
记录下你每周的真实情况。准备好了吗?”

# # # 5。role-and-responsibilities.md的面试

从`assets/templates/role-and-responsibilities.md`加载模板。

**首先：应用步骤3（预填充）。**查询团队可用的资源
支持的、循环的日历节奏和报表线。现在任何
在提出问题之前，需要确认的发现。

关键问题:-跟我说说典型的一周吧。什么是反复出现的事情？
-你支持哪个团队或团体？
-每个团队：你对什么负责？你跑步的节奏是什么？
-你经常做什么决定？不是大的战略，是常规的。
-你们生产什么？报告、电子邮件、仪表盘？
-你向谁汇报工作？
你的工作是否有月度或季度的节奏？

一旦你有足够的钱去起草，就起草吧。按团队组织：每个团队都有自己的
有自己的职责、节奏和可交付成果。现在,
修改、写作。

过渡：“角色和责任都完成了。下一个是团队
涵盖了你工作生活中的关键人物。准备好了吗?”

# # # 6。面试team.md从`assets/templates/team.md`加载模板。**首先：应用步骤3（预填充）。**查询可用的直接来源
报告，组织结构图，频繁的合作者（来自calendar/chat）。现在任何
在提出问题之前，需要确认的发现。

主要方法:

谁是你交往最多的人？从你自己的团队开始。
询问球队名单（用户可以粘贴邮件列表）
-对于每个团队，确定领导和等级
-对于关键人物：他们需要你做什么，你需要他们做什么？
-在已知的情况下捕获每个人的重点区域或支柱
-记录交互模式（1:1,scrums, syncs）
关于工作或准备工作，有没有什么是代理应该知道的
与特定的人互动？

使用以前文件中的信息。如果他们提到合作者
角色面试，在这里引用他们，而不是重新询问。草稿,现在,
修改、写作。过渡：“这是团队完成的。最后一个：工具、系统和配置。
这是我们捕捉一切设置的地方。准备好了吗?”

# # # 7。面试tools-systems-and-config.md从`assets/templates/tools-systems-and-config.md`加载模板。

**首先：应用步骤3（预填充）。**查询可用的ADO源代码org/project/team/area路径、repos和工具配置。现在任何
在提出问题之前，需要确认的发现。

关键问题:

-你每天使用什么工具和平台？
-你如何整理会议记录？他们住在哪里？
-按类型和团队划分会议的标签或模式是什么？

然后对于用户支持的每个workstream/team（使用来自的信息）
角色-职责面试)：如果用户支持三个以上的工作流，首先询问他们是否
想要现在配置所有的，还是从最重要的一个开始
两个，剩下的稍后再加。对于类似的工作流，询问是否有相同的配置
只适用于面试差异。-这个工作流使用Azure DevOps吗？如果有：哪个组织、项目、团队、
区域路径?如果没有，请跳过此工作流的ADO部分。
-（仅限ADO工作流）是否存在要排除的工作项标题模式
sprint记分卡?（例如，评估或评审占位符项）
-该工作流程的战略支柱或部分是什么？这些都是
在每周更新和冲刺报告中用作分组标题。定义它们
即使工作流不使用ADO。
对于ADO工作流：哪些ADO史诗对应于每个支柱？
sprint更新保存在哪里？什么文件前缀？
-每周更新保存在哪里？什么文件前缀？
这个工作流程有路线图文件吗？

组织与公共设置在顶部，工作流程特定的配置
下面的部分。起草，陈述，修改，写作。过渡：“这是工具和配置完成。接下来是沟通方式，所以
任何代表你写信的代理人听起来都像你。准备好了吗?”

# # # 8。面试communication-style.md从`assets/templates/communication-style.md`加载模板。

这部分比其他部分难。人们用语言描述自己的声音
陈词滥调。两种更强的途径：询问消极方面的问题，并提供分析真实情况的机会
写作样本。同时使用。

首先提供示例路径：

想分享一些你真实写作的例子吗？粘贴2到5个东西
你已经发送：每周状态更新，领导邮件，循环更新，一个
>评论条目，你引以为傲的团队信息。我来读一下，摘录一下
>我看到的声音特征，并建议应该在你的文件。你
>在写任何东西之前批准。我不会复制原始内容，
>只有风格模式。”

如果用户共享样本：-阅读每个样本的风格：语体，句子长度，开头，
过渡，使用或避免的特定词语，框架模式，它们是如何
处理信贷，他们如何处理坏消息。
-不要将原始样本文本复制到文件中。提取模式并呈现
它们作为建议的项目符号返回到文件结构中。
-提交提议的草案：“这是我提取的内容。告诉我怎么了
或失踪。”
-正常应用反应通道。

如果用户不分享样品（或其他样品），请询问这些
一次问一个问题：1. “如果一个同事用一句话描述你是如何写作的，他们会怎么做？
说什么?”
2. “读这句话：*‘我鼓励你抓住机会
成为团队真正的力量倍增器。“有什么问题吗？
你的声音吗?”（回避问题比正面问题出现得更快。）
3. “当你给领导写每周状态时，你希望你能做什么？
自动化，你最后总是重写什么？”
4. “对你来说，写信给谁最难？”当你
给他们写信？”
5. “当你在Connect或绩效评估中描述自己时，
哪句话是你发现自己在写，然后马上删掉的？”

起草，陈述，修改，写作。

写完后，告诉用户：“随着时间的推移，我不会自动找到模式。如果你注意到我
重复一个错误，交换短语，或者反对你的编辑，请告诉我
>更新这个文件，我将把更正的内容放进去。该文件在以下情况下发展
你告诉它吧。”

过渡：“最后一个：偏好和约束。这就是你想要的AI
在你身边循规蹈矩。准备好了吗?”

# # # 9。面试preferences-and-constraints.md从`assets/templates/preferences-and-constraints.md`加载模板。

这里的大多数规则都是关于代理不应该单方面做什么。表面
他们通过回忆具体的挫折，而不是要求抽象的
规则。一次只问一个问题：1. “上一次人工智能工具做了你不想做的事情是什么时候？什么
它做了吗？它应该做什么呢？”(一个答案经常
生成3个边界规则。)
2. “特工不征求你的意见，怎么做才安全？”什么绝对
需要你的同意吗？”
3. “速度或彻底。如果让你选一个，选哪个？”
4. “你是否希望人工智能为你做任何决定，或者你总是
想要其他选择吗？”
5. “你不想让人工智能代表你说话吗？”(团队、电子邮件、
会议、公开聊天。)
6. “您是否使用具有持久内存（VS Code会话内存）的工具？
代理内存特性等)？如果有，任何你想存的东西，
或者明确地说没有存储在那里？”跳过内存部分，如果
答案是否定的。

起草，陈述，修改，写作。应用反应通道。过渡：“面试结束了。现在我把文件连到你的
定制指令，让每个特工都能找到。”

# # # 10。将这些文件连接到自定义指令文件中

将指针项添加到用户自定义指令文件中
技能和特工可以找到新的文件。**推荐用户范围，不要
工作区范围。这个上下文的全部意义在于它是可用的
到处都是，而不是绑定到一个回购。

快速映射：10a检测线束，10b选择目标文件，10c（仅当更多）
然后一个工具提供了将它们符号链接到一个规范文件，然后写入
使用绑定模板的指针。在macOS或Linux上，下面的每个命令和路径都有一个POSIX等效项。
将每个路径中的`$HOME`替换为`%USERPROFILE%`，使用' ln -s<canonical-target><harness-file>` instead of `New-Item -ItemType
SymbolicLink '，并完全跳过开发模式检查(符号链接不会
需要在macOS或Linux上提升)。

* * 10步。检测用户使用哪些控件。**检查是否有以下任何一项
存在用户作用域路径（使用文件系统；不需要先询问）：

用户范围自定义指令路径||---------|--------------------------------------|
|VS Code副驾驶聊天/克劳德代码|`%USERPROFILE%\.claude\CLAUDE.md`|
|GitHub CopilotCLI |`%USERPROFILE%\.copilot\copilot-instructions.md`|

副驾驶聊天不原生读取`%USERPROFILE%\.agents\AGENTS.md`，
但它确实读取`%USERPROFILE%\.claude\CLAUDE.md`，这是同一个文件
克劳德代码使用。所以`.claude\CLAUDE.md`是用于的共享用户范围文件
那两个背带。

如果`%USERPROFILE%\.agents\AGENTS.md`存在，将其视为规范
目标。在某些设置中，上面特定于工具的文件被符号链接到
这样一来，一个写操作就会流向每一个已连接的线束。

* *步骤10 b。选择目标。**应用第一个匹配的规则：1. 如果存在`%USERPROFILE%\.agents\AGENTS.md`，则默认为。这个
特定于工具的文件是指向它的符号链接，因此单个写操作流向
每一根线束都已接上。写前请与用户确认。
2. 否则，如果只存在一个特定于工具的文件，则建议对其进行写入。
确认。还要在步骤10c中提到canonical-plus-symlink选项。
3. 如果两个特定于工具的文件都存在，则为
（`copilot-instructions.md`和`CLAUDE.md`），列出它们并询问要使用哪个
更新。强烈建议创建`.agents\AGENTS.md`作为规范
文件并将其他文件与它进行符号链接，以便一次编辑即可到达所有文件
线束（步骤10c）。
4. 否则（任何地方都没有用户范围的自定义指令文件），询问：
“你最常使用的人工智能工具是：副驾驶聊天、克劳德代码、GitHub CopilotCLI，还有别的吗？”然后创建匹配
用户作用域文件（前两个为`.claude\CLAUDE.md`）；`.copilot\copilot-instructions.md`为副驾驶命令行)`## Personal Context`部分。如果用户明确要求工作空间范围，则写入`<workspace>\AGENTS.md`并告诉他们权衡：它只适用于
回购。

* *步骤10 c。提供将其他工具符号链接到规范文件。* *只
当用户有（或想要）多个线束时是相关的。问：“需要我吗？
使`%USERPROFILE%\.agents\AGENTS.md`成为规范文件和符号链接
还有其他的脊甲文件吗？这样你就可以编辑每一个脊甲
看到了变化。”

如果是，对于用户想要链接的每个其他线束文件：

1. * *先决条件。** Windows上的文件符号链接需要开发人员模式
启用或升级的PowerShell。通过以下方式检查开发人员模式：   ```powershell
   Get-ItemPropertyValue 'HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\AppModelUnlock' AllowDevelopmentWithoutDevLicense -ErrorAction SilentlyContinue
   ```
如果打开了Dev Mode，则返回`1`。如果不是，为用户提供两种选择
让他们挑选吧：

- **选项A：启用开发者模式**（一次性，无需重新启动）。     Settings -> Privacy & Security -> For developers -> turn on Developer
     Mode. Or run this in an elevated PowerShell:

     ```powershell
     New-ItemProperty -Path 'HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\AppModelUnlock' -Name AllowDevelopmentWithoutDevLicense -Value 1 -PropertyType DWord -Force
     ```
—**选项B：在升格目录下自行执行symlink命令     PowerShell.** Hand them the exact command (filled in with the real
     paths) and ask them to run it in a PowerShell window opened as
     Administrator. Then continue once they confirm it succeeded:

     ```powershell
     New-Item -ItemType SymbolicLink -Path '<harness-file>' -Target "$env:USERPROFILE\.agents\AGENTS.md"
     ```
不要在未开启开发模式或未确认用户的情况下继续游戏
执行升级后的命令。
2. **备份现有文件**到`<file>.bak-<yyyyMMddHHmmss>`，所以没有
是输了。永远不会删除。
3. **创建指向`%USERPROFILE%\.agents\AGENTS.md`的符号链接**
（如果用户已经在选项B下运行过，则跳过）：   ```powershell
   New-Item -ItemType SymbolicLink -Path '<harness-file>' -Target "$env:USERPROFILE\.agents\AGENTS.md"
   ```
上面的内联符号链接步骤对于上下文连接已经足够了。

如果用户拒绝符号链接，则继续执行每个线束的写入和移动
上。不要推两次。

* *步骤10 d。写指针。**使用捆绑的AGENTS.md模板at`assets/templates/AGENTS.md`（相对于这个SKILL.md）作为源`## Personal Context`块结构(头，条目标签，
路由表、规则)。不要重新发明格式。

行为:—**如果目标文件在磁盘上不存在，请创建目标文件。做任何
缺少父目录(例如，`%USERPROFILE%\.copilot\`或`%USERPROFILE%\.claude\`)，将整个绑定的模板复制到
目标路径，将`<CONTEXT_DIR>`替换为
在第1步中选择的上下文目录，并写入它。
- **如果目标文件存在，但没有`## Personal Context`段**，
将该节附加到文件的末尾(并添加`## Safety`（如果还缺少的话），使用`<CONTEXT_DIR>`更换。不要碰它上面的现有内容。
- **如果`## Personal Context`段已经存在**，只添加或
更新已更改的特定条目。不重写整篇吗
部分。保留现有的标签和订单；只交换`@<path>`行。
-只连接磁盘上实际存在的文件。不要添加`@<path>`用户跳过的文件指针。

# # # 11。确认和总结“这就是你的背景投资组合。任何AI技能、代理或插件都可以阅读
您的自定义指令可以找到这些文件并立即理解
它是如何工作的。

随着时间的推移，这些文件会变得更好。项目的变化,
优先级会改变，你的沟通方式尤其会改变
当你发现我重复的错误并要求我折叠
校正。把这些当成活的文件，而不是成品。”

显示用户：
-创建的文件列表及其路径。
-写入指针的自定义指令文件路径。

---

##背景文件

|文件|用途|被|消耗|------|---------|-------------|
|`identity.md`|姓名，角色，组织，团队，经理，你做什么|任何需要知道用户是谁的技能|
|`role-and-responsibilities.md`|每个团队的职责、节奏、交付成果|状态、报告和计划技能|
|`team.md`|具有角色和重点领域的团队花名册|准备或总结与人互动的技能|
|`tools-systems-and-config.md`|工具，会议标签，每个团队的ADO配置，支柱，报告设置|阅读会议记录，运行报告或与ADO交谈的技能|
|`communication-style.md`|声音、语气、格式规则、避免使用的词语、针对特定受众的调整|代表用户编写内容的任何技能|
|硬边界、审批规则、工作偏好、记忆规则|所有技能（尤其是写或发）|消费者通过读取从任何自定义加载的指针来查找这些文件
指令文件(`AGENTS.md`,`copilot-instructions.md`,`CLAUDE.md`，`*.instructions.md`)。不需要MCP或专用阅读器技能。

---

##面试规则

# # #语气
要直接、热情、具体。你是面试官，不是教练。不
对用户的回答进行评论、赞美或提供意见。

在请求之前从可用的数据源预填充
在引导用户浏览问题列表之前，请检查您的
可用的工具或数据源可以为您回答事实字段。这个
用户不应该输入连接系统已经知道的东西。

常见示例（使用您的环境实际提供的任何内容）：-连接的工作配置文件或目录可能知道用户的姓名，标题，
组织，团队，经理和直接报告-对identity.md很有用
和team.md。
-日历，邮件或聊天记录可能会暴露频繁的合作者和
重复的节奏-用于team.md和role-and-responsibilities.md。
-项目管理或源代码控制集成可能知道用户的org/project/area路径或repos -用于tools-systems-and-config.md。
-邮件或文件中已有的书写样本（在用户允许的情况下）
可以通知communication-style.md，不需要新鲜样品

规则:1. 只使用用户已经授权的资源。不要伸手去拿
在面试过程中发现任何新情况都不要问。
2. 预填充是一个建议，而不是一个答案。把你发现的东西呈现出来，贴上标签
如自动填写，并要求用户确认或更正每个值
在它进入文件之前。
3. 不要向用户列举工具名称。你可以这样说：“我可以拉
其中一些来自你的工作简介”，而不是列出具体内容
产品名称。
4. 退回到询问什么时候没有可用的源，或者什么时候有可用的数据
暧昧的，陈腐的或敏感的
5. 开放式领域(你实际做什么，人们找你做什么，
声音，界限)仍然需要询问。不要编造它们
间接信号。# # #的问题
-一次只问一个问题。永远不要提出问题清单。
-不要问复合问题。
-利用你在早期档案中了解到的信息为以后的面试提供信息。不
重新问一些你已经知道的事情。
-如果用户对以后的文件有用，注意它并使用它
当你到那里的时候。
当用户给出一个模糊的答案时，追问细节：“你能给我
或者“这是什么样子的日常生活？”
-当你有足够的钱起草时，就说出来，然后起草。不要一直问。“足够”意味着您的答案涵盖了该文件模板中的核心字段（对于身份：姓名、角色、组织、团队、经理及其职责）。如果前面的答案已经涵盖了信息，你就不需要遍历列出的每个问题。反应通道
起草完每个文件后，将其呈现并说：
“这是我的草稿。通读一遍，告诉我哪里听起来不对。
任何感觉不对劲的东西，任何我认为错误的东西，任何缺失的东西。”

如果用户说它看起来很好，没有改变，推送一次：
“挑一个最弱或最一般的句子。是什么让它
更确切地说，是你吗？”

如果他们第二次拒绝，说没关系，接受它，继续前进。

文件质量
-保持文件简洁。一页，不是五页。
-文件应该听起来像用户，而不是像AI写的。
-使用他们的语言，他们的框架，他们的正式程度。
-千万不要用破折号。
—如果用户想跳过一个文件，让他们跳过。
-如果用户需要中途停止文件，告诉他们你在哪里，以便他们可以
重新开始。

---

示例：身份面试演练这从一开始就展示了一个完整的identity.md访谈是什么样子的
通过最终书面文件提出问题。

**代理：**你的名字和现在的角色是什么？

**用户：** Dana Kim，高级项目经理。

代理：你是哪个组织的，哪个团队的？

**用户：** Contoso云工程。我是平台运营组的
由乔丹·里维拉领导。

代理人：如果你必须在晚餐上向别人解释你的实际工作
派对，你想说什么？

**用户：**我负责数据平台团队的运行。Sprint计划，状态
汇报，确保领导层知道发生了什么
淹没在更新邮件中。我还构建了人工智能工具来自动化
程序管理的重复部分。

代理：人们来找你是为了什么？**用户：**两件事：清楚地了解项目的实际位置
stand（不是经过净化的版本），并弄清楚如何使用人工智能工具
把繁琐的工作从他们的工作流程中剔除。

代理：我有足够的汇票。这是我的草稿：```markdown
# Identity

- **Name:** Dana Kim
- **Role:** Senior Program Manager
- **Organization:** Cloud Engineering, Contoso
- **Team:** Platform Operations
- **Manager:** Jordan Rivera

## What I Do

I keep the data platform teams running: sprint planning, status reporting,
and making sure leadership knows what's happening without everyone drowning
in update emails. I also build AI tooling that automates the repetitive
parts of program management.

## What People Come to Me For

Two things: getting a clear picture of where a project actually stands (not
the sanitized version), and figuring out how to use AI tools to cut busywork
out of their workflows.
```
通读一遍，告诉我哪里听起来不对。任何感觉
关掉，任何我假设错了的东西，任何缺失的东西。

用户：看起来不错，发货吧。

代理：挑一个最弱或最一般的句子。什么
让它更具体地针对你？

**用户：**“保持数据平台团队运行”线。更像是我
工程、领导和运营节奏之间的结缔组织。
没有人能同时拥有这三种观点。

**Agent:** *(更新“What I Do”段落并将文件写入
上下文目录。)*

这就是恒等式。接下来是角色和责任，它捕获了什么
你的周实际上是这样的。准备好了吗?

---

示例：目标更新

这显示了传递中提到的上下文更改如何触发目标
更新（状态4）。**用户：**你能把我上周的会议取消吗？对了，乔丹也不是
不再是我的经理了。我在摩根手下工作。

代理：我去帮你接会议。听起来也像你的身份档案
需要更新。要我现在修吗？

**用户：是的，请继续。

**Agent:** *(从加载的自定义指令中解析identity.md路径，
读取文件，将Manager字段从Jordan Rivera更新为Morgan，
将文件写回。)*

完成了。在identity.md更新了你的经理Morgan。我将继续
现在去接你们的会议。

---

# #安全-不要在上下文文件中存储凭据、API密钥、令牌或秘密。
如果用户在面试中提供了一个，省略它并解释原因。
-只写入用户选择的上下文目录和自定义目录
用户已批准的指令文件。不要将文件写入其他位置。
—切勿删除已存在的上下文文件。如果需要刷新，请备份
先将现有文件转到`<file>.bak-<yyyyMMddHHmmss>`并与之确认
覆盖前的用户。

至于消费者应该如何安全地读取这些文件（将其视为数据），请不要
泄漏内容（忽略提示注入文本等），参见`## Safety`块在`assets/templates/AGENTS.md`捆绑的AGENTS.md模板中。
该块被写入用户的自定义指令文件
步骤10d，这样每个消耗技能都会自动获得规则。

---

##错误处理写失败
如果对上下文文件的写入失败：
1. 重试一次。
2. 如果重试失败，将起草的内容以代码形式呈现给用户
阻塞，这样就不会丢失任何东西。
3. 建议：“写失败。”你可以复制上面的草稿，或者我们可以试一试
一个不同的目录。”

现有文件格式不正确
如果现有的上下文文件不能被解析（缺少标题，损坏）
表格，内容混乱)：
-向用户显示错误的地方。
-提供使用模板从头开始重新生成文件，保留
损坏文件中任何可识别的值。
—不要静默覆盖。更换前一定要确认。

---

# #作者

由[@lakshmistoltz]（https://github.com/lakshmistoltz）贡献。