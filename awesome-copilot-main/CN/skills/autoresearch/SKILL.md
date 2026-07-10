---
name: autoresearch
description: 'Autonomous iterative experimentation loop for any programming task. Guides the user through defining goals, measurable metrics, and scope constraints, then runs an autonomous loop of code changes, testing, measuring, and keeping/discarding results. Inspired by Karpathy''s autoresearch. USE FOR: autonomous improvement, iterative optimization, experiment loop, auto research, performance tuning, automated experimentation, hill climbing, try things automatically, optimize code, run experiments, autonomous coding loop. DO NOT USE FOR: one-shot tasks, simple bug fixes, code review, or tasks without a measurable metric.'
license: MIT
compatibility: Requires git. The project must be a git repository. Requires terminal access to run commands.
metadata:
  author: luiscantero
  inspired-by: https://github.com/karpathy/autoresearch
---
# Autoresearch：自主迭代实验

任何编程任务的自主实验循环。你定义了目标以及如何衡量它；代理自动迭代——修改代码、运行实验、测量结果，以及保留或丢弃更改——直到中断。

这项技能的灵感来自于[Karpathy的autoresearch](https://github.com/karpathy/autoresearch)，从机器学习训练推广到任何具有可测量结果的编程任务。

---

代理行为规则1. **DO**在启动循环之前交互式地引导用户完成设置阶段。
2. ** ** *在进行任何更改之前建立基线测量。
3. ** ** *在运行之前提交每个实验尝试（这样可以干净地恢复）。
4. ** ** *保持结果日志（TSV）跟踪每个实验。
5. **DO**恢复不改进度量的更改（git reset to last known good）。
6. **DO**一旦循环开始就自动运行——永远不要停下来问“我应该继续吗？”
7. **不要**修改用户标记为超出作用域的文件。
8. **不要**跳过测量步骤-每个实验都必须测量。
9. **不要**保留回归度量的更改，除非用户明确允许权衡。
10. ** *不要**安装新的依赖项或进行环境更改，除非用户批准。

---

阶段1：设置（交互式）在任何实验开始之前，与用户一起建立这些参数。
直接向用户询问每个项目。不要假设或跳过任何。

### 1.1定义目标

询问用户：

你想要改进或优化什么？**
>
示例：执行时间，内存使用，二进制大小，测试通过率，代码覆盖率，
API响应延迟、吞吐量、错误率、基准得分、构建时间、包大小、
100行代码，圈复杂度等等。

记录用户的回答作为目标。

定义度量

询问用户：我们如何衡量成功？产生度量的确切命令是什么？**
>
我需要：
> 1。**要运行的命令**（例如，`dotnet test`,`npm run benchmark`,`time ./build.sh`,`pytest --tb=short`）
> 2。**如何从输出中提取度量**（例如，正则表达式模式，特定行，JSON字段）
> 3。**方向**：越低越好还是越高越好？
>
>示例："输入`dotnet test --logger trx`，统计通过的测试项。越高越好。”
>示例：“执行`hyperfine './my-program'`，提取平均时间。越低越好。”

记录:
—`METRIC_COMMAND`：待执行的命令
-`METRIC_EXTRACTION`：如何从输出中提取数字度量
—`METRIC_DIRECTION`:`lower_is_better`或`higher_is_better`1.3定义作用域

询问用户：

> **我可以修改哪些文件或目录？**
>
>哪些文件是禁止的（只读的）？

记录:
—`IN_SCOPE_FILES`:files/dirs代理可以编辑
—`OUT_OF_SCOPE_FILES`:files/dirs，不可修改

1.4定义约束

询问用户：有什么约束是我应该尊重的吗？**
>
>例子:
> -每次实验的时间预算（例如，“每次运行应该少于2分钟”）
> -没有新的依赖项
> -必须保持所有现有的测试通过
> -不能更改公共API
> -必须保持向后兼容性
> -VRAM/memory限制
> -代码复杂性限制（更喜欢简单的解决方案）

记录为`CONSTRAINTS`。

定义实验预算（可选）

询问用户：

我应该做多少个实验，还是应该一直做下去直到你阻止我？**
>
>你可以说一个数字（例如，“尝试20个实验”）或“无限”（我会一直跑到你打断为止）。

记录为`MAX_EXPERIMENTS`（数字或`unlimited`）。

1.6简单性标准

告知用户默认的简单性策略：> **简化策略（默认）：**在其他条件相同的情况下，越简单越好。一个小小的改进
>增加了丑陋的复杂性是不值得的。在维护或改进的同时删除代码
这个指标是一个很好的结果。我将权衡复杂性成本与改进之间的关系
>级。这个政策对你有用吗，还是你想调整一下？

将任何调整记录为`SIMPLICITY_POLICY`。

### 1.7确认安装

将所有参数以清晰的表格形式汇总回用户：

| |值|| ------------------ | ---------------------------- |
|进球|…|
| Metric命令|…|
|公制萃取|…|
方向|越低越好/越高…|
|作用域内文件|…|
|超出作用域的文件|…|
|约束|…|
Max实验|…|
简单策略|…|

请用户确认。在确认之前不要继续。

---

阶段2：分支和基线

一旦用户确认：

1. **创建分支**：根据今天的日期提出一个标签（例如，`autoresearch/mar17`）。
创建分支：`git checkout -b autoresearch/<tag>`。

2. **读取作用域内文件**：读取作用域内的所有文件以构建当前状态的完整上下文。3. * *初始化的结果。tsv**：在repo根目录创建`results.tsv`，头行：   ```
   experiment	commit	metric	status	description
   ```
将`results.tsv`和`run.log`添加到`.git/info/exclude`中（如果还没有添加），这样它们就不会被跟踪，而不会修改任何被跟踪的文件。

4. **运行基线**：对当前未修改的代码执行度量命令。
记录结果为实验`0`，状态`baseline`在`results.tsv`。

5. **向用户报告基线**：
> Baseline established: **[metric_name] = [value]**
>开始自主实验循环。

---

阶段3：实验循环

连续运行这个循环。不要停下来询问用户。直到运行:
—`MAX_EXPERIMENTS`为“或”
—用户手动中断

###对于每个实验：```
LOOP:
  1. THINK   - Analyze previous results and the current code.
               Generate an experiment hypothesis.
               Consider: what worked, what didn't, what hasn't been tried.

  2. EDIT    - Modify the in-scope file(s) to implement the idea.
               Keep changes focused and minimal per experiment.

  3. COMMIT  - git add + git commit with a short descriptive message.
               Format: "experiment: <short description of what changed>"

  4. RUN     - Execute the metric command.
               Redirect output to run.log so it does not flood the context window.
               Use shell-appropriate redirection:
               - Bash/Zsh: `<command> > run.log 2>&1`
               - PowerShell: `<command> *> run.log`

  5. MEASURE - Extract the metric from run.log.
               If extraction fails (crash/error), read the last 50 lines
               of run.log for the error.

  6. DECIDE  - Compare metric to the current best:
               - IMPROVED: Keep the commit. Update the "best" baseline.
                 Log status = "keep".
               - SAME OR WORSE: Revert. `git reset --hard HEAD~1`.
                 Log status = "discard".
               - CRASH: Attempt a quick fix (typo, import, simple error).
                 Amend the experiment commit (`git commit --amend`) with the fix
                 and rerun. The experiment keeps its original number.
                 If unfixable after 2 attempts, revert the entire experiment
                 (`git reset --hard HEAD~1`) and log status = "crash".

  7. LOG     - Append a row to results.tsv:
               experiment_number  commit_hash  metric_value  status  description

  8. CONTINUE - Go to step 1.
```
实验策略

在产生实验想法时，遵循以下优先顺序：

1. **简单的参数调整，明显的低效率。
2. **根据结果**：如果一个方向显示出希望，就朝着这个方向进一步探索。
3. **停滞期后多样化**：如果前3-5次实验都失败了，那就尝试完全不同的方法。
4. **组合获胜者**：如果实验A和B各自独立提高，尝试组合它们。
5. **简化通过**：定期尝试删除code/complexity，看看度量是否成立。
6. **彻底的改变**：在用尽增量的想法后，尝试更大的架构改变。

处理约束- **时间预算**：如果运行超过预期持续时间的两倍，杀死它并将其视为崩溃。
- **现有测试**：如果约束要求测试通过，则运行它们before/after，如果它们中断，则恢复。
- **Memory/resources**：如果资源使用超过规定的限制，监控并恢复。

---

阶段4：报告

当循环结束时（达到预算或用户中断）：

1. **打印完整结果。Tsv **作为格式化的表。
2. * *总结* *:
-实验运行总数
-实验保留/丢弃/崩溃
-起始指标（基线）vs.最终指标
-改善百分比
- 3个最具影响力的变化
3. **显示保存实验的累积git日志**：`git log --oneline <start_commit>..HEAD`4. **建议下一步**：根据结果，建议人类研究人员下一步可能会尝试什么（对于自动化实验来说太risky/complex的想法）。

---

##快速参考

结果TSV格式tab分隔，5列：```
experiment	commit	metric	status	description
0	a1b2c3d	0.997900	baseline	unmodified code
1	b2c3d4e	0.993200	keep	increase learning rate to 0.04
2	c3d4e5f	1.005000	discard	switch to GeLU activation
3	d4e5f6g	0.000000	crash	double model width (OOM)
```
Git工作流

-所有实验都发生在`autoresearch/<tag>`分支上
-每个实验在运行前都要提交
—失败的实验用`git reset --hard HEAD~1`进行还原
-成功的实验推进分支
-`results.tsv`和`run.log`保持不被跟踪（添加到`.git/info/exclude`）

关键原则

1. **测量一切**：没有测量就没有实验。
2. **恢复失败**：分支只在改进时前进。
3. **保持自主：永远不要停下来问。如果遇到困难，要更努力地思考。
4. **保持简单**：复杂性是一种成本。权衡得失。
5. **记录一切**:TSV是研究日志。