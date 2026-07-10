#步骤9：收敛验证

子代理类型：`explore`；预算：3分钟。

# #输入

——`PrNumber`。
—从步骤7推送的`HeadOid`（用于独立的完整性检查）。
-循环是normal模式还是[single-iteration]
（orchestration.md#single-iteration-fallback）（在第一步决定）。

##返回合同```
{ converged, head_oid, latest_review_commit_oid, submitted_at,
  open_thread_count, open_threads_awaiting_reply, escalated_threads }
```
`converged`是单一真值源布尔值-`Converged: true`由`02-check-review-status.ps1`返回。

# #过程

运行状态检查，传递`-SingleIteration`步骤1：```pwsh
pwsh ./scripts/02-check-review-status.ps1 -PrNumber <n>
# single-iteration variant:
pwsh ./scripts/02-check-review-status.ps1 -PrNumber <n> -SingleIteration
```
然后运行一个独立的HEAD-vs-`LatestCopilotReview.commitOid`完整性检查** -从步骤7中记录的父节点的`HeadOid`应该
匹配`HEAD`和（在正常模式下）匹配最新的审查`commitOid`。

决定：返回或退出

状态检查后，父代理**必须**分支在`converged`上：```
if converged == true:
    run step 10 once (cleanup outdated)
    call task_complete with proof (HeadOid, LatestCopilotReview.commitOid, submittedAt)
    DONE — exit the loop
else:
    # non-converged = a fresh Copilot finding OR an unresolved human thread.
    # round = count of Copilot review submissions in the PR's history,
    #         read deterministically from the API (NOT a mental tally):
    #             pwsh ./scripts/09-review-round.ps1 -PrNumber <n>   -> {Round, RecapDue}
    if RecapDue == true:                        # Round is 10, 20, 30, ...
        RUN THE RECAP GATE (see "Round cap & recap gate" below) BEFORE looping:
            recap ALL prior rounds, then pick CONTINUE / REVERT-AND-SHIP / HAND-OFF.
            CONTINUE        -> fall through and start another round
            REVERT-AND-SHIP -> drop drifted commits, ship the in-scope result, exit
            HAND-OFF        -> escalate to the user with the recap, exit
    GO BACK TO STEP 1 — start another round
    (re-trigger via 01-request-review.ps1, wait via 02-wait,
     list via 03-list-threads, triage, fix, push, reply+resolve,
     re-check via this step)
```
一个非收敛的结果**永远**不**终端** -每一轮
处理对前一轮HEAD的公开评审反馈；
不管这是副驾驶的发现还是人类的评论
技能处理两者)。只有当没有new时循环才会终止
审查评论从任何来源**和**每个开放的线程-副驾驶
或者人—有来自代理**的回复（代理升级的线程）
回复给用户计数；它在`OpenThreadCount`中保持打开状态
显式移交，而不是循环工作)。但“永不终结”绝对不是
读作“无限”：机器人审查循环没有保证的固定点和
可能漂移到过度工程或振荡。没有脚本*强制* a
封顶或停止循环——封顶是父方拥有的理性决定
在[圆帽重盖门]（#round-cap-重盖门-断路器）
在下面。脚本所编写的是整数计数本身
([09-review-round.ps1（#round-cap- recap-gate-circuit-breaker）)，所以
盖特的触发是决定性的，而不是容易出错的心理统计
(而振荡——同样的发现在不同的回合中再次出现——是
早破
[04-triage.md] (04-triage.md#不一致的言论——break-oscillation-early))。`-SingleIteration`模式是**one**例外：根据定义，它
只运行一轮（触发路径不可用），并且`converged`的结果无论往哪个方向走都作为终端。

收敛语义`02-check-review-status.ps1`实现了pr状态保护和三个聚合分支（请参阅该脚本末尾的`Converged = if (...)`块以获取规范源代码）：**PR状态守卫（覆盖所有）** -如果`State != 'OPEN'`（关闭/合并），`Converged: false`不管所有其他标志。代理不能推送到非open PR；向用户显示状态变化并中止循环，而不是调用`task_complete`。
- **正常（副驾驶驱动）模式** -副驾驶审查存在或`CopilotPending: true`：`Converged: true`敌我识别`ReviewAtHead && NoNewComments && OpenThreadsAwaitingReply == 0`。
- **单迭代模式** (`-SingleIteration`通过，因为循环采取了[回退在第一步](orchestration.md# Single-iteration -fallback)))：`Converged: true`iff`OpenThreadsAwaitingReply == 0`。如果没有新的副驾驶审查，旧检查永远无法进行，所以它们被省略了。
- **没有副驾驶检查被观察到，也没有等待**（全新的pr没有发现，或者pr触发静默失败，脚本没有被`-SingleIteration`调用）`Converged: true`iff`OpenThreadsAwaitingReply == 0`。**不要相信这是“循环完成”之前的步骤1已经触发** -这只是意味着没有人类线程工作pending。父代理必须先运行`01-request-review.ps1`（per [step 1](01-request-review.md)）并重新检查；将全新的pr融合作为终端，将使整个回路短路。当升级到用户的线程保留时，`OpenThreadCount`可能是`> 0`打开——这是一个明确的人工交接，而不是一个循环故障。返回
升级的`thread_id`s列表，以便父程序可以将它们包含在
收敛性证明。

圆盖和复盖门（断路器）

没有脚本*强制*最大轮数上限或停止循环-一个硬数字
分不清“富有成效”的一轮和“飘忽不定”的一轮。而不是父母
代理运行**重述门**作为推理：默认**每10次停止
**（10,20,30，…）**，然后**循环回到步骤1，重述所有内容
并决定循环是否仍在为PR服务
原来的范围。脚本中的内容是计数，所以门的触发器是
决定论，而不是容易出错的心理统计。**圆**是** 1
执行[步骤1](01-request-review.md)** -一次副驾驶审查
在循环的顶端触发——这恰好产生了一名副驾驶
审查提交。[`09-review-round.ps1`] (../scripts/09-review-round.ps1)
直接从PR的API历史和报告中计算这些提交
节奏是否被击中：```pwsh
pwsh ./scripts/09-review-round.ps1 -PrNumber <n>
# {"PrNumber":<n>,...,"Round":20,"RecapInterval":10,"RecapDue":true}
```
在`RecapDue`上的非收敛分支和门的顶部运行它。
因为计数来源于历史，而不是记忆，所以它不能
漂移甚至跨越100+轮运行-确切的失败，这门存在
赶上。上限为**轮审查**（副驾驶审查提交），
不是子代理调用、工具调用或单独的修复编辑——所以是一轮
分流的五个线程仍然算作一个线程。节奏是`-RecapInterval`旋钮（默认为10）。脚本只报告触发器；
它永远不会决定判决。

这是因为无界的bot-review循环是故障模式
这个技能是为了生存而生的：一次真正的漂流可以打156发
最后几轮“修复”公关从未打算改变的东西
恢复他们自己早期的修复。这扇门挡住了那类漂泊者
提前，每10轮，而不是最后一次。回顾回顾（所有前几轮，而不仅仅是最近10轮）

1. **原PR范围** -issue/PR标题和PR的差异
基地。这是标尺；其他一切都与之相对比。
2. **每轮分类帐** -到目前为止每轮：副驾驶的发现，
处理（固定/拒绝/升级），以及结果
更改（文件+意图在一行）。
3. **整个历史的漂移信号**：
- **超出作用域** -不能追溯到     original issue/PR goal (new feature, adjacent refactor, polish
     the PR never promised).
-过度工程-防御层，抽象，或配置     added solely to satisfy bot nits, not the PR's actual goal.
- **方向错误** -修复后的回合必须撤销，工作     around, or re-fix (self-revert / oscillation across rounds).
- **属于单独的pr ** -一个合法的改进，即     nonetheless unrelated to this PR's stated change.
- **Scope/complexity增长** - diff大小或文件计数爬升     while the original goal was met rounds ago.
# # #裁决

|判决|何时|行动|| --- | --- | --- |
到目前为止，每一轮都追溯到最初的PR范围；无漂移信号；副驾驶仍在调查范围内的情况。|对于下一个10轮块，循环回到步骤1。|
一个或多个炮弹漂移（过度工程/方向错误/振荡），但范围内修复是合理的。|`git revert`（或删除）仅漂移的提交，保留范围内的提交，运行步骤6build/test，然后发布干净的结果。记录收敛证明中哪些回合被逆转。|
| **移交** |漂移与范围内工作纠缠在一起，正确的修复方法是重新设计，或者更改属于单独的PR。|停止循环，回复相关线程，并通过概述和建议升级给用户（单独的PR /重新设计）。**不要**一直循环。|触发是脚本化的，但判决是代理推理
故意。[`09-review-round.ps1`] (../scripts/09-review-round.ps1)
使“计数”是确定的（所以门不会被错过），但是
*选择哪一个判决*仍然是一个主观判断：没有数字可以告诉一个
从漂流而来的富有成效的一轮。回顾是廉价的(阅读
每轮提交+ PR基础差异)；跳过它的代价是
又一个失控的循环。- **相信`02-check-review-status.ps1`的`Converged`标志，不是你的
自己的re-derivation。**脚本强制执行所有三个条件
（正常模式）或简化条件（单次迭代）和
是规范的来源。
- **不要调用`task_complete`，直到`converged == true`。* *打印
证明(`HeadOid`,`LatestCopilotReview.commitOid`，`submittedAt`,`OpenThreadsAwaitingReply: 0`，升级列表
线程（如果`OpenThreadCount > 0`）在完成消息中。
- **`-SingleIteration`是粘性的退决定。* *如果
步骤1采取了回退，此循环中的每个步骤9都使用`-SingleIteration`;不要翻到一半。
- **PR州！= OPEN终止循环。**如果`State`是`CLOSED`或`MERGED`，`Converged`是脚本状态强制的`false`警卫。父代理不能推送到非open PR -表面
状态更改为用户并停止循环而不是
重试或调用`task_complete`。