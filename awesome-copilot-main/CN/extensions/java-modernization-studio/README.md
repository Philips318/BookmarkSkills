# Java现代化工作室

一个交互式的GitHub Copilot画布，将[GitHub Copilot应用程序Java现代化](https://learn.microsoft.com/en-us/azure/developer/java/migration/migrate-github-copilot-app-modernization-for-java) CLI工作流变成一个可见的、可操纵的仪表板——评估遗留Java应用程序，驱动优先级修复计划，运行验证门，并调度微软预定义的任务，所有这些都基于repo的真实工件。

用于Java工具的Copilot App Modernization仍然是引擎。这个画布是它上面的座舱：它读取工作流产生的内容，并将每个步骤转换为代理驱动按钮。

它的作用- **概述** -一眼现代化状态（阶段，%完成，查找计数）扫描从回购。
- **Readiness (Environment Doctor)** -检查JDK、Maven（或`./mvnw`）、Git、Docker和Azure CLI的PATH，并在开始之前标记缺失的内容。
- **评估** -呈现结构化的发现从`.appmod/assessment.json`（堆栈总结，严重排序的发现，优势），每一个一键操作。
- **计划与进度** -呈现`plan.md`/`progress.md`作为现场检查清单（`- [ ]`/`- [x]`）。
- **验证** -在变更之前和之后运行工作流的质量闸门（CVE验证，测试生成）。
- **任务** -调度微软预定义的现代化任务（数据库的管理身份，秘密→密钥库，消息代理→服务总线，S3→Blob，缓存→Redis， Entra ID认证，以及更多）相关的检测堆栈。
- **摘要** -运行完成时表面`summary.md`。
- * * AutopIlot ** -一个可选的分阶段顺序，免手动循环，推进评估→修复→验证，并在代理取得进展时更新仪表板。按钮不会在画布上执行逻辑——它们向副驾驶代理发送接地提示（动作类型：`run_task`，`generate_plan`,`run_cve`,`generate_tests`,`fix_finding`）。代理人完成工作；画布反映了结果。

# #先决条件

- **GitHub Copilotapp**（画布主机）。
**GitHub Copilot应用程序现代化的Java**工具-这个画布驱动的底层工作流。
**JDK 17+**和**Maven**（或`./mvnw`包装器）为您正在现代化的Java项目。
- *可选：* **Azure CLI** (`az`)用于云准备和Azure迁移任务；**Docker**用于容器检查。

# #安装

这是一个在仓库中的画布扩展。将`java-modernization-studio/`文件夹复制到其中一个：

-`~/.copilot/extensions/`- **用户范围**（仅您），或
-`.github/extensions/`- **项目范围**（与你的repo团队共享）。然后重新加载扩展（或重新启动应用程序），以便Copilot发现它。不需要构建步骤- Copilot CLI自动解析`@github/copilot-sdk`。

# #使用

将画布指向Java存储库并让代理驱动它：```text
Open the Java Modernization Studio canvas for /path/to/my-java-app and run a readiness check.
```
画布从其`repoPath`输入解析目标repo，并返回到会话的工作目录。从那里:

1. **准备第一** -解决任何丢失的JDK/Maven/AzureCLI的医生标志。
2. **评估** -生成`.appmod/assessment.json`+一个优先的`plan.md`/`progress.md`。
3. **修复** -工作发现按严重程度排序（P0优先），使用任务按钮和“帮助我修复此问题”。
4. **验证-运行CVE和测试生成门。
5. **发布** -当工作真正完成时，写`summary.md`。

建议的代理指令```text
When a user modernizes a Java project with the Java Modernization Studio canvas:
1) Open the canvas pointed at the repo (repoPath) and run the Environment Doctor first.
2) Run an assessment; write findings to .appmod/assessment.json and a prioritized plan.md / progress.md.
   Start plan.md, progress.md, and summary.md with the exact first line <!-- appmod-cockpit --> so the
   canvas recognizes them as modernization artifacts.
3) Work findings in severity order (P0 first); run the validation gates (CVE scan, test generation)
   before and after code changes.
4) Keep plan.md / progress.md updated as - [ ] / - [x] checklists. Only write summary.md when the
   work is truly complete.
```
##如何保持接地气

画布呈现真实的状态，而不是虚构的状态：

-结构化的发现来自`.appmod/assessment.json`。
-Plan/progress/summary来自根`plan.md`/`progress.md`/`summary.md`。
为了避免将不相关的repo的`plan.md`误认为是现代化的输出，只有当`.appmod/`存在**或**文件的第一行是来源标记`<!-- appmod-cockpit -->`时，才会信任root markdown。
-堆栈检测（构建工具，Java版本，框架，容器）从`pom.xml`/ Gradle / Dockerfile中解析并驱动显示的任务。

代理可调用的操作

|动作|描述||---|---|
|`get_state`|返回从回购（评估、plan/progress、闸门、任务）扫描的当前现代化快照。|
重新扫描回购，并将一个新的快照推到打开的画布。|

# #发展grounding/parsing逻辑是纯粹的，并且独立于canvas运行时进行单元测试：```bash
node --test test/cockpit.test.mjs
```
# #许可证

麻省理工学院