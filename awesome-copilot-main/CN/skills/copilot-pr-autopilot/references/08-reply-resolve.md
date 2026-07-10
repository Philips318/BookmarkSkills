#步骤8：回复（总是）+解决（有条件）

子代理类型：`general-purpose`**起草**回复主体；这个
**父母**发布它们（突变由父母拥有）。年度预算
起草子代理：5分钟。

运行AFTER步骤7 (commit + push)，这样每个回复都可以引用
push commit SHA**。

# #输入

-步骤4的完整分类表- ' {thread_id, action，
理由}` per open thread (including `escalate-to-user”)。
—从步骤7中推送的`HeadOid`。
-从步骤5中修复每个线程的`summary`和`files_touched``fix`行)。

##返回合同

每个打开的线程一行：```
{ thread_id, action, reply_body }
```
式中`action`∈`fix`|`decline`|`escalate-to-user`。父
使用这些数据来驱动resolve/no-resolve决策(参见
程序)。

# #过程

1. **起草子代理**每个线程生成一个`reply_body`选择合适的模板（参见[#templates](#templates)）
根据分类`action`。引用步骤7中推入的SHA`fix`答道。对于`escalate-to-user`，说明配置和
人类合并所有者的开放性问题；不要承诺。
的决心。
2. **家长每次发帖回复**，选择是否解决：   ```pwsh
   pwsh ./scripts/08-reply-and-resolve.ps1 -ThreadId <id> -Body <text>
   ```
-`action ∈ { fix, decline }`→如上运行（解析发生）。
-`action == escalate-to-user`→**添加`-NoResolve`**所以线程     stays open for the human:

     ```pwsh
     pwsh ./scripts/08-reply-and-resolve.ps1 -ThreadId <id> -Body <text> -NoResolve
     ```
# #陷阱- **回复每一个开放的线程；只在循环拥有
配置** （`fix`或`decline`）。不回答而解决
没有记录为什么这个问题被认为已经解决了。
- **升级线程保持开放*与我们的回复*解释
处置。**它们是对人类合并的明确移交
所有者，而不是循环失败——这就是为什么第9步的收敛可以
成功使用`OpenThreadCount > 0`。
- **突变是父级所有的。**子代理仅汇票；它从来没有
职位。这样就保留了对父节点和
避免并发子代理之间的双post竞争。
- **引用推送的SHA，而不是本地提交。**第7步已录好`HeadOid`是唯一可以浏览到的SHA审查器。
- **回复下一轮的卫生事宜。**拒绝不这样做
引证推理会在下次副驾驶审查时再次提出。看到
[04-triage.md] (04-triage.md# reply-hygiene)。# #模板

按分诊动作挑选：

|分类动作|模板||---------------|----------|
|`fix`| [reply-fix.md](../templates/reply-fix.md) |
|`decline`| [reply-decline.md](../templates/reply-decline.md) |
| [reply-drift.md](../templates/reply-drift.md) | . | PR-description / comment漂移确认
| [reply-partial.md](../templates/reply-partial.md) |

对于`escalate-to-user`，没有模板—编写定制的回复
解释性格和开放性问题，然后发`-NoResolve`，线程保持打开。

##回复指导

回复必须做实际工作——它记录了未来的决定
维护人员和形状的下一次副驾驶审查将浮出水面。

具体（引用文件路径，提交sha，函数名）；
**直接**（当你有仓位时没有对冲），**短期** （2-4）
句子是典型的)。冗长的回复通常意味着应该进行一轮讨论
已经分手了。

反模式——不要使用-❌`"Thanks!"`/`"Good point."`无物质。
-❌`"Will fix later."`要么现在修复它，要么理性地下降
没有在任何地方跟踪的延迟修复会丢失。
-❌Resolve-without-reply。下一个评论者无法解释原因
线程关闭了。
-❌`"I disagree."`没有理由。说明实际的技术
分歧。