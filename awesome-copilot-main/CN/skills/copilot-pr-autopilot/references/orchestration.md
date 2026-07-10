#编排-父类拥有的循环控制

副驾驶公关审查循环的横切协议：时间盒，
子代理委托映射、单迭代回退以及
loop-wide笔记。每一步，包括父母拥有的第1、7步，
10 -有自己的`NN-*.md`合约文件和这个文件一起；这
文件只保存跨越整个循环的内容。

构建、测试和lint命令没有在这里指定。每一步
这需要它们遵守目标回购自身的约定`CONTRIBUTING.md`,`AGENTS.md`,`README`,`package.json`/`Makefile`/语言工具等)。发现并跟踪回购
现有的实践——永远不要发明构建命令。

时间盒和扩展协议

|概念|规则||---------|------|
|默认预算|每个子代理调用| 5分钟
|子代理必须返回|`status`∈{`complete`，`partial`,`blocked`} +`next_action`+`needs_extension_minutes`（无则为0）。总是在预算到期前总结进度——永远不要悄悄超支。|
|扩展|父扩展仅当`status: partial`和`next_action`是具体的；用`N = min(needs_extension_minutes, 10)`|发送`write_agent "continue for N min"`扩展上限（默认）|每步2个扩展；步骤6 （build/test）为慢速套件高达2倍。步骤2（等待）是单个有界子代理—参见[02-wait.md]（02-wait.md）—不是扩展驱动的。|
|父节点从不阻塞|步骤1（请求）、步骤7（提交+推送）、步骤8reply/resolve突变，并且`task_complete`决策保留在父节点|中

当达到上限并且工作仍然是`partial`时，父进程
缩小输入（在步骤4中批量更小/在步骤5中分割修复范围）
或者超越自己一步。

子代理委托映射> **循环：** one **round** =步骤1→2→3→4→5→6→7→8→9。在步骤9之后，如果`Converged: false`， **回到步骤1**进行另一轮。重复，直到步骤9返回`Converged: true`；然后运行步骤10一次并退出。**在每10轮，父运行[round-cap recap gate]（09-convergence.md#round-cap- recap-gate-circuit-breaker）在回圈之前** -断路器，重装所有之前的回合，并停止循环，如果它已经偏离了PR的原始范围。参见[09-convergence.md]（09-convergence.md）了解收敛定义、环出口/环回决策和重盖门。

每轮规范顺序：**请求→等待→列表→分类→修复→
构建→提交+推送→回复+解析（引用推送的SHA）→
收敛检查* *。Reply/resolve运行后推，所以回复可以引用
push commit SHA。

|步骤|所有者|合同||------|-------|----------|
| -请求检查|父| [01-request-review.md](01-request-review.md) |
| -等待审查|子代理（`general-purpose`， 20分钟）| [02-wait.md](02-wait.md) |
|子代理（`explore`， 5分钟）| [03-list-threads.md](03-list-threads.md) |
|子代理（`general-purpose`， 5分钟/≤5个线程）| [04-triage.md](04-triage.md) |
bbb5 -应用修复|子代理（`general-purpose`，并行最多5，每个5分钟）| [05-fix.md](05-fix.md) |
|子代理（`task`+`explore`， 10分钟）| [06-build-test.md](06-build-test.md) |
bbb7 - Commit + push | parent | [07-commit-push.md](07-commit-push.md) |
bbbb8 -回复（总是）+解决（有条件的）|子代理草案→父帖子| [08-reply-resolve.md](08-reply-resolve.md) |
bbb9 -收敛验证|子代理（`explore`, 3 min） | [09-convergence.md](09-convergence.md) |
|0 -清理过时（后收敛，一次）|父| [10-cleanup.md](10-cleanup.md) |

单迭代回退当`01-request-review.ps1`抛出，因为副驾驶代码审查没有
在repo /帐户上启用（GraphQL突变报告bot）
（不是有效的审阅者），代理就会退回到单次迭代
模式* *:

-跳过步骤2（无需等待副驾驶审查）。
-运行步骤3 - 8一次对任何审查线程已经存在
（人类，高级安全，其他机器人）。
—在步骤9中，将`-SingleIteration`传给`02-check-review-status.ps1`so
收敛性检查忽略了过时的检查
在没有新的副驾驶检查的情况下前进。`Converged: true`折叠成`OpenThreadsAwaitingReply == 0`。
-重复迭代只发生在人类之后发布新评论时-
在那个点重新运行技能。

单迭代模式是代理在触发后的决策
失败**，不是自动检测的状态-脚本不能可靠地判断
“副驾驶已禁用”源自“副驾驶已启用但尚未触发”源自
API状态。

收敛性证明在`task_complete`消息中打印收敛证明-证明，
不主张:

——`HeadOid`——`LatestCopilotReview.commitOid`——`submittedAt`——`OpenThreadsAwaitingReply: 0`-所有打开的`escalate-to-user`线程的列表`OpenThreadCount > 0`。

# #笔记- **重请求是一级请求。**`01-request-review.ps1`没有
当副驾驶已经检查时，静默跳过；它发出
相同的突变，并通过新的`copilot_work_started`事件进行验证
(脚本强制执行此操作—参见[api-quirks.md]（api-quirks.md）
GraphQL表面和静默陷阱)。
- **过时的线程仍然需要回复+解决。**他们出现在
PR UI在你明确关闭它们之前是无法解决的；第十步
是一个安全网，而不是主要机制。
- **重新打开/重新访问请求将线程重置到步骤4。**如果
被拒绝的搜索结果由用户（或后续用户）重新打开
副驾驶检查)，根据之前的理由将其拉回分类
作为输入，而不是重新运行整个循环。
- **中断后的恢复能力。**重启时，快照HEAD，
最新的副驾驶审查的`commit.oid`+`submittedAt`开放线程列表，以及任何未提交的本地更改。丢弃
缓存如果HEAD或打开的线程设置发生了变化，则分类/草稿。
- **本地构建补丁。**对于未提交local-build的项目
“git stash push -m local-build”——<paths>` before committing, `git stash pop` after. Note `-m ' must
来到`--`之前（参见[api-quirks.md](api-quirks.md)）