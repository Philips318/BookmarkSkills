---
name: copilot-pr-autopilot
description: 'Copilot left 14 review comments on your PR — half are nits. Hours of fix → reply → resolve → re-request, and each round lands MORE comments. This skill runs loop engineering: auto-triggers Copilot Code Review via GraphQL (no @copilot mention), triages every open thread (Copilot, humans, advanced-security) with a fix / decline / escalate rubric, dispatches parallel fix sub-agents that obey the repo build/test/lint conventions, commits per iteration, replies+resolves citing the pushed SHA, then re-triggers until HEAD is reviewed with zero threads awaiting the agent''s reply (remaining open threads are explicit hand-offs to the human — escalated declines, design tradeoffs). You merge a clean PR; the bot runs it. Trigger phrases: "address copilot comments", "run a copilot review loop", "fix this PR", "iterate on copilot feedback". Repo-agnostic, gh CLI + PowerShell. Full autopilot needs repo Triage/Write; external PR authors get single-iteration mode plus manual re-trigger (UI 🔄 or substantive-commit push).'
---
#副驾驶公关自动驾驶

通过重复的Copilot代码驱动任何GitHub拉取请求
直到代理完成了它的工作——每一个副驾驶的发现都完成了
代理的回复(fix-acknowledgement, decline-with-rationale，
或者显式的升级到用户的切换)。剩余的打开线程，如果
任何一个，都是故意交给人类合并所有者的，他们是
不是循环失败。Repository-agnostic -适用于任何有
启用了Copilot Code Review，使用`gh`CLI从机器上运行
已安装并通过身份验证（参见前提条件）。

何时使用此技能

-用户要求“请求副驾驶审查”或“运行副驾驶审查循环”
在公共关系上
PR在功能上是完整的，用户想要一个最终的正确性通过
通过重复的自动复查。
-之前副驾驶对PR的审查留下了需要的开放线程
分类，修复，回复，解决。什么时候不要使用这个技能

PR还在主动设计中，等结构稳定了再走
否则，研究结果就会到处乱转。
-用户想要人类评论者的反馈，而不是副驾驶的。

# #先决条件-`gh`CLI安装并根据目标存储库进行身份验证。
Windows PowerShell 5.1+ (`powershell.exe`)或
PowerShell 7+ (`pwsh`)。两者都经过了测试。
-副驾驶代码审查是主要用例（`01-request-review.ps1`）
使用GraphQL`requestReviewsByLogin`触发副驾驶)。它是
**不是硬性要求** -如果`01-request-review.ps1`失败
因为回购/账户上没有启用副驾驶，代理可以启用
仍然驱动现有的审查线程（人力、高级安全等）。
通过将步骤3-8作为单个迭代运行一次来完成；只是
跳过触发器+等待。“副驾驶”没有自动检测功能
不可用”——代理在触发后做出决定
失败(脚本不能可靠地告诉“副驾驶禁用”从
“副驾驶启用，但尚未触发”仅从API状态)。

权限：谁可以运行完整的循环完整的多轮自动驾驶仪（步骤1→9→1）需要**Triage或Write**对目标仓库的权限，因为GitHub的唯一公共API用于添加副驾驶机器人作为审稿人（`requestReviewsByLogin`）对该权限进行了限制。根据此PR的提交历史中的公共REST + GraphQL表面进行验证-没有写权限的bot审阅者没有公共api路径。

你是…|什么有用||---|---|
| **Repo合作者与Triage / Write** |完整循环：`01`触发副驾驶，`02`等待，`04`-`08`Triage / fix / reply，循环回到`01`。不干涉。|
| **外部PR作者（无写权限）** |`01`将抛出一个明确的可操作错误。使用`-SingleIteration`模式：在一次传递中解决所有当前发现，然后单击UI🔄旁边的Copilot， **或**推动实质性提交（`synchronize`事件自动触发Copilot在大多数repos）。然后重新运行`02`进行验证。|

在单迭代模式下，循环的收敛布尔值为`Converged: true`iff`OpenThreadsAwaitingReply == 0`（代理方完成）。维护人员侧的再触发器然后驱动任何额外的子弹。每个脚本点源[scripts/_lib.ps1](scripts/_lib.ps1加载时运行`Assert-GhReady`：如果`gh`缺失或`gh auth status`如果失败，脚本将在任何工作之前停止**，并使用单个可操作操作
错误消息命名安装命令和`gh auth login`。这个
代理应该将该消息逐字呈现给用户，并停止
循环-不要重试或绕过它。

##分步工作流程

> **循环：**步骤1→2→3→4→5→6→7→8→9，然后**返回步骤1**如果`Converged: false`。重复1→9循环，直到步骤9返回`Converged: true`；然后运行步骤10一次，并调用`task_complete`。**在每10轮，父运行[round-cap recap gate]（references/09-convergence.md#round-cap- recap-gate-circuit-breaker）在回圈之前** -重播所有之前的回合，并停止，如果循环已经偏离了PR的原始范围。每一轮从步骤1到步骤9；步骤10是收敛后的一次性清理。父代理坐标；每个子代理步骤都在一个预算有限的新环境中运行。横切协议（时间盒、扩展、单迭代回退）：[orchestration.md]（references/orchestration.md）。1. **请求审查** _（父）_ -见[01-request-review.md]（references/01-request-review.md）
2. **等待审查** _（子代理，20分钟上限）_ -见[02-wait.md]（references/02-wait.md）
3. **列表+分类打开线程** _（子代理，5分钟）_ -参见[03-list-threads.md]（references/03-list-threads.md）
4. **分类** _（子代理，每≤5个线程5分钟）_ -参见[04-triage.md]（references/04-triage.md）
5. **修复** _（子代理，并行最多5,5分钟）_ -参见[05-fix.md]（references/05-fix.md）
6. **按repo约定构建+测试** _（子代理，10分钟）_ -参见[06-build-test.md]（references/06-build-test.md）
7. **Commit + push** _(parent)_ -参见[07-commit-push.md]（references/07-commit-push.md）
8. **Reply (always) + resolve (conditional)** _(sub-agent drafts, parent posts)_ - see [08-reply-resolve.md]（references/08-reply-resolve.md）
9. **收敛验证** _（子代理，3分钟）_ -参见[09-convergence.md]（references/09-convergence.md）
- **`Converged: false`→循环回到步骤1**进行另一轮（重新触发，等待，列表，分类，修复，推送，回复，重新检查）。每一轮讨论副驾驶对前一轮HEAD的发现；一旦副驾驶没有新消息要说，并且每个打开的线程都有代理的回复，循环就会终止。
- **`Converged: true`→退出循环**，运行步骤10一次，用证明调用`task_complete`。
- **每10轮（10,20,30…）→运行[圆盖盖门](references/09-convergence.md#round-cap- recap-gate-circuit-breaker)，然后再循环回去。**根据PR的原始范围回顾之前的所有回合，并选择一个结论：**CONTINUE**， ** restore - and - ship **（删除漂移提交，发布范围内的提交），或**HAND-OFF**（升级到用户）。这是阻止失控的机器人审查循环的断路器。
10. **清理过时的** _（父级，后收敛，一次）_ -参见[10-cleanup.md]（references/10-cleanup.md）收敛性由[scripts/02-check-review-status.ps1]（scripts/02-check-review-status.ps1）作为单个`Converged: true`布尔值计算。**不要**调用`task_complete`，直到它返回true；在完成消息中打印证明（`HeadOid`,`LatestCopilotReview.commitOid`,`submittedAt`）。

# #陷阱

捆绑的脚本强制执行硬正确性不变量（通过`copilot_work_started`事件id触发着陆，`Converged`需要head匹配+零等待+ at-HEAD审查，单迭代回退语义，pr状态保护）。相信他们——不要重新推导。下面的说明涵盖了剧本不能为你做的决定：- **回复每一个开放的线程；只有当循环拥有处置时才解析。**对于`fix`和`decline`线程，回复+解析。对于`escalate-to-user`线程，回复分析，但保留线程OPEN (`08-reply-and-resolve.ps1 -NoResolve`)，以便人工合并所有者可以对其进行操作。[08-reply-resolve.md] (references/08-reply-resolve.md)。
- **副驾驶线程是循环拥有的；Human / advanced-security / other-bot线程默认为`escalate-to-user`。**自动解决人工审查线程可以隐藏未解决的问题。参见[04-triage.md]（references/04-triage.md）的标题。
- **每轮只提交一次，而不是每个PR一次。**捆绑回合破坏了哪个发现驱动哪个更改的审计跟踪，并打破了`git bisect`。[07-commit-push.md] (references/07-commit-push.md)。
**Build/test/lint与repo自己的命令**（按其`CONTRIBUTING`/`AGENTS`/`README`/`package.json`/`Makefile`）推送修复之前。发现过程：[06-build-test.md]（references/06-build-test.md）。
- **当副驾驶时，用书面理由推回这一发现将对假设的边缘情况进行过度设计。自动接受每个建议会破坏设计——参见[04-triage.md]（references/04-triage.md）中的`decline`路径。
**脚本陷阱** （`gh api graphql -F`类型强制转换，`git stash push -m`位置解析，评论者突变的三个GraphQL陷阱）记录在[references/api-quirks.md]（references/api-quirks.md）中。修改脚本前请先阅读。# #故障排除

|问题|解决方案||-------|----------|
安装`gh`（Windows上的`winget install GitHub.cli`； macOS上的`brew install gh`； Linux上的软件包管理器；或从https://cli.github.com下载）。然后`gh auth login`。将消息显示给用户并停止循环-不要重试。|
|运行`gh auth login`。停止循环，直到用户完成验证。|
|在`synchronize`上推送一个实质性的（非空白的）提交-自动分配是最可靠的触发器。持续故障表明repo /帐户上可能未启用副驾驶代码审查（检查repo设置→代码和自动化→副驾驶或帐户级副驾驶Pro/Pro+）。|
等待~10分钟后无新审查|最近解雇或小困难抑制后的安静期。推送一个实质性的提交并重试。不要盲目地重新运行`01-request-review.ps1`-它报告`InFlight`，而副驾驶仍然是一个请求的审查。|
|过时但未解析的线程在打开列表|预期：未解决的状态是真相的来源。回复+解决他们像任何其他开放的线程。`10-cleanup-outdated.ps1`只是最后的安全网。|
|不确定是修复还是拒绝查找|参见[references/04-triage.md]（references/04-triage.md）。|
|需要“固定”，“拒绝”或“漂移”的回复措辞|请参阅[templates/]（templates/）下的模板- [reply-fix.md](templates/reply-fix.md), [reply-decline.md](templates/reply-decline.md), [reply-drift.md](templates/reply-drift.md), [reply-partial.md]（templates/reply-partial.md）。|# #引用- [references/orchestration.md](references/orchestration.md) -
横切回路控制：时间盒和扩展协议；
子代理委托映射、单迭代回退和循环范围
笔记。
-每步合约（每步一个`NN-*.md`）：
[references/01-request-review.md] (references/01-request-review.md) _(父)_,
[references/02-wait.md] (references/02-wait.md),
[references/03-list-threads.md] (references/03-list-threads.md),
[references/04-triage.md](references/04-triage.md)(包括
fix-vs-decline标题),
[references/05-fix.md] (references/05-fix.md),
[references/06-build-test.md] (references/06-build-test.md),
[references/07-commit-push.md] (references/07-commit-push.md) _(父)_,
[references/08-reply-resolve.md] (references/08-reply-resolve.md),
[references/09-convergence.md] (references/09-convergence.md)(包括
圆帽盖盖门)；
[references/10-cleanup.md] (references/10-cleanup.md) _(父)_。
- [references/api-quirks.md](references/api-quirks.md) -已验证
GitHub API行为、死角和GraphQL陷阱
评论家突变。
-模板（每个回复类型一个）：
[templates/reply-fix.md](templates/reply-fix.md) -已接受修复
模式;[templates/reply-decline.md] (templates/reply-decline.md)
declined-with-rationale模式;
[templates/reply-drift.md] (templates/reply-drift.md)
pr -描述/评论/测试计划漂移确认edgement;
[templates/reply-partial.md] (templates/reply-partial.md)
部分修复，延期跟进。横切回复指导
反模式存在于
[references/08-reply-resolve.md] (references/08-reply-resolve.md# reply-guidance)。
- [scripts/_lib.ps1](scripts/_lib.ps1) -共享助手(`Invoke-Gh`，`Invoke-GhGraphQL``Resolve-RepoCoords`);点来源的每一个
脚本。
- [scripts/01-request-review.ps1]（scripts/01-request-review.ps1）
触发副驾驶检查并通过`copilot_work_started`事件。
- [scripts/02-check-review-status.ps1](scripts/02-check-review-status.ps1) -
PR副驾驶审查状态的单次快照；发出`Converged: true`仅在三个条件都成立时。
- [scripts/03-list-open-threads.ps1]（scripts/03-list-open-threads.ps1）
**所有审稿人**未解决的PR审查线程(副驾驶，
人类，github-advanced-security，等等)。
- [scripts/08-reply-and-resolve.ps1](scripts/08-reply-and-resolve.ps1) -
在一个电话中回复并解决问题。
- [scripts/10-cleanup-outdated.ps1](scripts/10-cleanup-outdated.ps1) -
过时的副驾驶线程的安全网。