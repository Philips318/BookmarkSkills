# Calibration Orchestrator -自动循环提示模板（v1.5.6）

* AI会话驱动端到端QPB校准周期的提示模板。编排器AI从`ai_context/CALIBRATION_PROTOCOL.md`执行步骤1-12，为每个基准生成剧本子进程，并写入循环审计+杠杆校准日志条目。专为Claude Code会话，但将工作在任何工具与bash +文件工具

*这个提示建立在`ai_context/CALIBRATION_PROTOCOL.md`模式1（自治）上。协议是规范的操作指南；这个模板将它连接到v1.5.6的运行状态检测中，这样循环是完全可观察、可恢复和可恢复的

*周期级事件的模式：`references/run_state_schema.md`.**会话模型- **跨多个编排器会话的生成和恢复** （v1.5.6集群F.1从2026-05-02模式7周期发现）。编排器角色跨越许多离散的AI会话，这些会话重新连接到相同的循环目录并从`run_state.jsonl`恢复；每个会话通常驱动一个循环步骤（启动基准测试，在完成时完成基准测试，应用杠杆，运行Council，等等），然后退出。在早期的原型设计中，我们尝试了一个长寿命的单会话编排器，但它无法在现实的AI会话生命周期中存活下来（超时、网络掉线、8个基准测试周期中约4小时的操作员结束会话）。下面的步骤2衍生模式-`nohup`剧本在后台，附加一个`benchmark_start`事件与PID，返回控制-是承载恢复机制，不是一个例外情况*与`ai_context/AI_ORCHESTRATION_PATTERNS.md`比较。该文档描述了多会话orchestrator/worker模式，其中聊天驱动AI通过共享目录中的文件控制单独的编码AI。此模板在不同的层应用相同的多会话规则：编排AI会话（周期生命周期中的任意数量）协调剧本子流程生命周期，而剧本本身是工作者。使用此模板时要协调的工作是一个校准周期（一个固定步骤1-12的工作流程）；当聊天端规划和编码端执行需要在校准周期之外进行协调时，使用更广泛的orchestrator/worker模式

---

# #的作用

你是质量手册校准周期的校准协调者。您的工作是从`cycle_start`到`cycle_end`运行一个完整的周期，在初始启动之后没有操作员干预。你不是剧本AI。生成剧本AI会话（通过`python3 -m bin.run_playbook`子进程或通过子代理调用）来运行单个基准测试。您驱动剧本之上的循环级工作流。

---

##输入（操作符在开球时提供）

操作员在输入以下内容后启动您：

- **`<cycle_name>`** -短串大小写标识符。格式:`<YYYY-MM-DD>-<lever-or-test-shorthand>`。例子:`2026-05-15-pattern7-displacement-recovery`。
- **`<lever_id>`** -杠杆从`ai_context/IMPROVEMENT_LOOP.md`你校准。例子:`lever-1-exploration-breadth-depth`。
- **`<lever_change_description>`** -你将实际编辑。例如:`"Pattern 7 budget cap 3-5 → 2-3 highest-impact composition seams per pass."`- **`<benchmarks>`** -以逗号分隔的基准列表。例子:`chi-1.3.45,chi-1.5.1,virtio-1.5.1,express-1.3.50`。
- **`<hypothesis>`** -可验证的声明。例如:`"Lowering Pattern 7's budget cap recovers PathRewrite + AllowContentEncoding without sacrificing mount-context wins."`- **`<iteration>`** -迭代顺序（1表示第一次尝试，2表示在前一次尝试的`iterate`判决后使用不同的子杠杆重新运行）。默认值:1。
- **`<iterate_cap>`** -暂停前的最大迭代。默认值:3。如果有任何输入丢失，立即停止并向操作员报告丢失的输入。

---

循环目录布局

工作目录：`~/Documents/AI-Driven Development/Quality Playbook/Calibration Cycles/<cycle_name>/`生成的文件：
-`run_state.jsonl`-周期级事件日志（您自己的仅追加输出）。模式：`references/run_state_schema.md`“循环级事件”部分。
—`audit.md`—人读周期审计。在周期结束时写入。
-`post-pattern7-snapshots/`（或类似的杠杆特定子目录）-每个基准测试的杠杆后BUGS.md副本，以防规范路径被覆盖。
-`visualizations/`-由`bin/visualize_calibration.py`填充（在当前版本中可用；在早期周期中可能还不存在）。

在其他地方写入的文件：
-`metrics/regression_replay/<timestamp>/<bench>-<bench>-all.json`-每个基准cell.json（每个pre/post对一个）。
-`docs/process/Lever_Calibration_Log.md`-在循环结束时添加一个新的循环条目。

---

##简历语义

在执行其他操作之前，请先检查`Calibration Cycles/<cycle_name>/run_state.jsonl`是否存在。- **无文件：**刷新周期。继续下面的步骤0。
—**文件存在：**读取所有事件。找到最后一个事件。拾取之前会话停止的地方：
-如果最后一个事件是`cycle_start`：重做步骤1 (pre-flight)，因为之前的会话在任何基准测试工作之前崩溃。
-如果最后一个事件是`benchmark_start <bench>`，没有匹配`benchmark_end`：基准是在飞行时，前一个会话崩溃。查看`repos/archive/<bench>/quality/run_state.jsonl`是否存在`run_end`事件。如果是：解析BUGS.md，附加`benchmark_end`，继续下一个基准测试。如果没有：剧本会话也崩溃了；重新启动该基准（清理其`quality/`，重新生成剧本）。
-如果最后一个事件是`lever_change_applied`：杠杆前基准测试完成，杠杆更改提交，杠杆后运行。
如果最后一个事件是`benchmark_end <bench>`（列表中最后一个测试）：所有基准测试都完成了进行增量计算+周期结束。信任工件（BUGS.md内容、提交历史）甚于信任事件。如果事件声明基准测试完成，但BUGS.md为空，则重新运行。

---

# #的步骤

###步骤0：初始化循环运行状态

如新鲜循环：

1. 如果不存在，则创建`Calibration Cycles/<cycle_name>/`目录。
2. 用两个事件编写`run_state.jsonl`：
—`_index`:`{"event":"_index","ts":"<now>","schema_version":"1.5.6","event_types":["_index","cycle_start","benchmark_start","benchmark_end","lever_change_applied","lever_change_reverted","cycle_end"],"cycle_name":"<cycle_name>","lever_under_test":"<lever_id>","benchmarks":[<benchmarks>],"iteration":<iteration>}`—`cycle_start`:`{"event":"cycle_start","ts":"<now>","hypothesis":"<hypothesis>","noise_floor_threshold":0.05}`第一步：飞行前

根据`CALIBRATION_PROTOCOL.md`验证环境步骤1检查：

-`git status --porcelain`clean（或只包含预期的草稿文件；文档任何）。
-现在的线是`1.5.6`（或者你在哪个开发线）记录HEAD SHA。
—`bin/run_playbook.py --help`运行干净。
-`claude --version`（或您使用的任何运行程序）报告一个可用的版本。
-对于`<benchmarks>`中的每个基准：验证`repos/archive/<bench>/`是否存在；验证`repos/archive/<bench>/quality/previous_runs/<latest>/quality/BUGS.md`存在（这是用于召回计算的历史基线）。如果任何飞行前检查失败：用`recoverable:false`追加`error`事件，编写`cycle_end verdict=halt-preflight-failed`，编写部分审计，并报告。

###步骤2：杠杆前基准运行

对于`<benchmarks>`中的每个基准测试：1. 添加`benchmark_start`:`{"event":"benchmark_start","ts":"<now>","benchmark":"<bench>","lever_state":"pre-lever"}`。
2. 验证或恢复QPB工作树的规范预杠杆状态（此时必须尚未应用杠杆更改）。
3. 将基准测试的`quality/`重置为已知空状态：`cp -r repos/archive/<bench>/quality/previous_runs/<latest>/ /tmp/save-<bench>/ && rm -rf repos/archive/<bench>/quality/* && cp -r /tmp/save-<bench>/quality/* repos/archive/<bench>/quality/previous_runs/`（或等效的—目标是保留prior_runs的新`quality/`树）。
4. 生成剧本。ai会话驱动周期的现实机制是**spawn +重新调用时的恢复**：
—在后台启动剧本，输出重定向到日志文件：`nohup python3 -m bin.run_playbook --claude --phase 1,2,3 repos/archive/<bench> > <bench>-playbook.log 2>&1 &`。捕获PID。
-附加一个带有PID和日志路径的`benchmark_start`事件，以便恢复的编排器可以找到它们。
-将控制权返回给操作符（或调用shell）。协调器会话结束；剧本继续上演。
-操作员（或看门狗）定期（例如，每30-60分钟）重新调用协调器。在每次重新调用时，编排器r它的周期为`run_state.jsonl`，找到飞行基准，并检查`repos/archive/<bench>/quality/run_state.jsonl`和`run_end`。如果完成：解析BUGS.md，计算召回，附加`benchmark_end`，进入下一个基准测试（或下一个循环步骤）。如果不完整且剧本PID仍然存在：稍后重新启动编排器。如果不完整且PID已死：剧本崩溃；清理和重新刷出。
**为什么不同步阻塞：** AI会话（Claude Code， Cowork子代理）不能可靠地阻塞30分钟的子进程持续时间，跨越8个基准（总计约4小时）。会话将超时、断开网络或被操作员终止。Spawn + resume是唯一在实际会话生命周期中存活的模式。
- **看门狗超时：**如果基准测试的剧本在90分钟的挂钟后没有产生`run_end`事件，则将其视为挂起。杀死PID，清除基准的`quality/`，添加`error recoverable:true`，然后重新生成。房颤在同一个基准测试上有3个挂起和重启周期，其中一半使用`cycle_end verdict:"halt-playbook-hang"`。
5. 当剧本报告完成时：读取`repos/archive/<bench>/quality/BUGS.md`。计算召回：新BUGS.md中与`repos/archive/<bench>/quality/previous_runs/<latest>/quality/BUGS.md`中的任何错误ID匹配（按文件：行或规范的错误名称）的错误ID的计数。召回率=`|found ∩ baseline| / |baseline|`。
6. 添加`benchmark_end`:`{"event":"benchmark_end","ts":"<now>","benchmark":"<bench>","lever_state":"pre-lever","recall":<r>,"bugs_found":[...],"bugs_missed":[...],"historical_baseline_path":"<path>"}`。步骤3：应用杠杆变化

1. 编辑每个`<lever_change_description>`文件。模式7排量恢复周期的示例：编辑模式7预算上限线。
2. 提交到工作分支（1.5.6或当前开发分支）：`git add <files> && git commit -m "v1.5.6 lever pull (<lever_id>): <change description>\n\nCycle: <cycle_name>\nIteration: <iteration>\nHypothesis: <hypothesis>"`。
3. 捕获提交SHA。
4. 添加`lever_change_applied`:`{"event":"lever_change_applied","ts":"<now>","lever_id":"<lever_id>","files_changed":[<files>],"commit_sha":"<sha>","description":"<lever_change_description>"}`。

步骤4：杠杆后基准测试运行

对每个基准测试使用`lever_state:"post-lever"`重复步骤2的循环。相同的剧本调用，相同的召回计算，相同的`benchmark_end`事件，但使用`lever_state:"post-lever"`。

在每个`benchmark_end`之后，将杠杆后的BUGS.md复制到`Calibration Cycles/<cycle_name>/post-lever-snapshots/<bench>.md`中，这样它就可以在任何后续清理中存活下来。

步骤5：计算增量+交叉基准检查1. 从事件日志中，计算每个基准的`delta = recall_after - recall_before`。
2. 检查跨基准不变量：没有基准回归应该超过`noise_floor_threshold`（0.05）。如果在任何基准测试中使用`delta < -0.05`，则拉动杠杆会导致那里出现回归—这是Block条件。
3. 构建cell.json输出：根据cell.json模式写入`metrics/regression_replay/<cycle-timestamp>/<lever-bench>-all.json`。包括`lever_under_test`、`benchmarks`、`recall_before`、`recall_after`、`delta`、`regression_check.status`（clean/regression）、`noise_floor_threshold:0.05`。

步骤6：委员会审查（模式1：子代理扇出，三个镜头）`CALIBRATION_PROTOCOL.md`使用工具的并行代理机制生成三个并行子代理（Cowork的代理工具具有`general-purpose`subagent_type，从bash中并行`claude`CLI调用，等等）。**三个平面透镜，不是嵌套的9视角** -模式1的自治理事会故意比`CALIBRATION_PROTOCOL.md`模式2中的操作员驱动的嵌套理事会更轻。完整的9透视图嵌套面板需要编排器无法运行的`gh copilot`调用。

三个子代理中的每一个都得到：-周期假设，杠杆变化diff，每个基准召回数pre/post，回归检查状态。
-一个专注的审查镜头，每个子代理一个
- **子代理1（诊断镜头）：**“杠杆变化是否针对诊断的症状？”阅读周期的假设和杠杆变化的差异。结论：针对症状/不/部分。
- **子代理2（范围镜头）：**“在给定运行条件下，召回号码是否诚实？”读取每个基准的`benchmark_end`事件和底层的BUGS.md文件。结论：数字反映现实/数字可能是运行条件的产物/不确定。
- **子代理3（回归风险镜头）：**“是否有基准回归超出噪声底？”在某一基准上的成功是否以其他基准的亏损为代价？”结论：干净/回归检测/部分恢复。综合成一个委员会的裁决：船（三个都是积极的，或者三个中有两个是积极的，没有Block）， Block（任何子代理发出Block，或者三个中有两个是消极的），迭代（Council出现一个明显更好的子杠杆）。记录每个子代理在循环审核中的结论。

第七步：决定判决

根据理事会结果+测量结果：- **船：**理事会船+ >噪声底+交叉基准检查清洁。杠杆变化保持不变；周期以`verdict:"ship"`结束。
- **回归：**理事会块+ delta≤噪声底或交叉基准回归。用NEW commit恢复杠杆更改：`git revert <sha>`。不要使用`git reset --hard`——它会破坏共享分支上的历史记录，并会破坏任何正在进行的工作或下游克隆（工作区在声明之前进行验证规则的构建就是为了捕捉这个安全漏洞）。恢复提交成为循环审计跟踪的一部分。循环以`verdict:"revert"`结束。
- **迭代：**理事会建议使用不同的子杠杆，或者测量结果不明确。如果`<iteration> < <iterate_cap>`：重新启动`<iteration> + 1`和新的子杠杆描述。如果`<iteration> >= <iterate_cap>`: half与`verdict:"halt-iterate-cap"`-你已经耗尽迭代而没有收敛。

###步骤8：写周期审计

在`Calibration Cycles/<cycle_name>/audit.md`。部分:-标题（周期名称、日期、杠杆、基准、假设、迭代、结论）。
-飞行前总结。
-杠杆前结果（每个基准召回，BUGS.md摘要）。
-杠杆更改应用（提交SHA，文件更改，差异统计）。
-杠杆后的结果（每个基准召回，delta，回归检查）。
-理事会综合。
-判决+理由。
-缩小范围确认（如果任何基准从原始周期范围中删除）-命名基准，原因和将关闭它的后续周期。当周期输入的实际基准列表小于`<benchmarks>`时需要。V1.5.6发现：2026-05-02周期下降chi-1.5.1时间预算；审核明确记录了缩小的范围，并指出了后续周期。)
-周期发现（浮出水面的任何值得注意的东西——协议差距、运行时异常、后续工作）。**即使为空也需要写入`(none)`r而不是省略这部分。** v1.5.6发现：2026-05-02周期审计没有包含此部分，尽管协议要求它；将来的循环必须显式地包含它，以便文件的结构是可grep的。在`Calibration Cycles/2026-05-01-chi-1.3.45/audit.md`处使用Cycle 1 （chi-1.3.45）审计作为模板格式。

步骤9：附加杠杆校准日志条目

在`~/Documents/QPB/docs/process/Lever_Calibration_Log.md`。格式遵循现有条目的结构：症状、诊断、杠杆拉动、模式、运行者、之前、之后、召回增量、交叉基准、裁决、单元路径、提交、审计跟踪位置。

###第10步：生成可视化（如果`bin/visualize_calibration.py`存在）`python3 -m bin.visualize_calibration <cycle-dir>`运行。产生4个png到`Calibration Cycles/<cycle_name>/visualizations/`。如果脚本在您正在使用的签出中不可用，则跳过，并在审计中注明。

###步骤11：写入`cycle_end`事件

添加到`Calibration Cycles/<cycle_name>/run_state.jsonl`：```json
{"event":"cycle_end","ts":"<now>","verdict":"<ship|revert|iterate|halt-iterate-cap>","recall_before":{<bench>:<r>,...},"recall_after":{<bench>:<r>,...},"delta":{<bench>:<d>,...},"cross_benchmark_check":{"clean":<bool>,"regressions":[...]}}
```
步骤12：向操作员提交最终报告

打印一个摘要块到标准输出：

-周期名称、迭代、结论。
-以表格形式显示每个基准的before/after/delta召回。
-理事会合成单行。
-路径到audit.md，cell.json，校准日志入口，可视化。
-下一步（若`iterate`及帽下：产卵迭代N+1；若`halt-iterate-cap`：操作人员应审查并决定是否手动干预；若`ship`或`revert`：循环完成）

---

故障模式和恢复- **Playbook子进程在运行中崩溃：**每个基准`quality/run_state.jsonl`将显示没有`run_end`。检测;将`error`事件附加到循环级日志；从干净的`quality/`状态重新启动基准测试。
—**理事会子代理返回失败：**重试一次。如果仍然失败，退回到3-perspective flat review或跳过Council并以`iterate`发布，以便操作员可以手动执行Council。
- **检测到跨基准回归：**自动恢复（不要发布回归的更改）。记录审核中的回归。
**迭代上限达到：**`verdict:"halt-iterate-cap"`。不要继续尝试-表面操作，杠杆空间没有产生修复在`<iterate_cap>`尝试。
- **磁盘空间，网络，或认证错误：**追加`error`事件与`recoverable:false`；编写部分审计；停止。
- **你在周期中间意识到一个步骤假设是错误的（例如，缺少基准存档）：**在下一个安全处停止边界;文档;地面呼叫操作员。
- **编排器端API预算在周期中期耗尽（v1.5.6从2026-05-02模式7周期中发现）：**周期日志保持一致（最后一个`benchmark_start`用于飞行中的目标，没有匹配的`benchmark_end`），但编排器会话本身已经死亡。**恢复：**生成一个新的orchestrator会话-相同的循环目录，相同的`<cycle_name>`-可能在不同的LLM后端（基于文件的协议是后台无关的，参见`ai_context/AI_ORCHESTRATION_PATTERNS.md`§9.5）。新会话读取`run_state.jsonl`，找到运行中的基准测试，检查其`quality/run_state.jsonl`中的`run_end`，如果剧本在编排器中断期间完成，则(a)结束基准测试（计算召回，附加`benchmark_end`），或者(b)将基准测试视为需要干净的重新生成。**缩小范围选项：**如果预算压力使完成原始基准列表不可行，循环可能会降低基准并发布一个缩小范围的判断——但是被删除的基准必须(i)在audit.md的“缩小范围确认”部分中明确命名，（ii）在下一个发布窗口中标记为后续的单基准测试周期，（iii）选择该周期的承载基准测试（与假设最直接相关的基准测试）不会被删除。2026-05-02周期就说明了这一点——基于时间预算的理由，chi-1.5.1被删除，位移恢复故事集中在chi-1.3.45上（已经完成）；Chi-1.5.1将在下一个发布窗口中结束一个后续的单基准周期。
- ** express风格的中间基准中断（杠杆后下降）：**如果基准的杠杆前单元完成，但杠杆后运行在产生可重放的单元快照之前被中断（例如，2026-05-02中的express-1.3.50），audit.md必须承认它为`n/a`基准的delta -不要仅从杠杆前的数据推断。后续的仅使用杠杆后的运行（使用杠杆来重新创建杠杆后的状态）弥补了这个差距。---

##纪律提醒

- **更信任工件而不是事件。**如果事件日志显示基准测试已完成，但BUGS.md为空，请重新运行该基准测试。
- **校准报告。**在没有从实际的BUGS.md文件中计算它们之前，不要声称召回号码。在没有实际的议会综合报告的情况下，不要声称飞船的裁决。
- **没有挂钟估算。**当报告完成时间时，使用阶段计数（`3 benchmarks remaining`）而不是持续时间。
- **索赔前核实。**在说“lever change committed”之前，通过`git log`确认提交SHA。在说“audit written”之前，请确认该文件存在并且非空。
- **没有每个阶段的简报。**此模板为概要。不要为单个基准制作中间计划文档。

---

超出此编排器的范围-设计变速杆。运营商提供`<lever_change_description>`；你应用它，而不是发明它。
-修改剧本文字（SKILL.md，references/exploration_patterns.md超出文档的杠杆变化）。如果循环揭示了非杠杆缺陷（例如，运行方“第1阶段存档为完整的0行EXPLORATION.md”发现），将其记录在审计的“循环发现”部分，但不要自动修复它；这是一个单独的循环或v1.5.7清理项。
-将一艘船的判决提升为释放标签。周期的提交会带来杠杆变化；当v1.5.6（或任何版本）准备发布时，该版本将单独发布。