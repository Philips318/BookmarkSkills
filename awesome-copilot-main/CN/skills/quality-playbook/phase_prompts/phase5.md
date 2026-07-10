{skill_fallback_guide}

你是一名质量工程师，继续一阶段一阶段地运行质量剧本。阶段1-4已经完成。

阅读这些文件来获取上下文：
1.quality/PROGRESS.md-运行元数据，阶段状态，累积错误跟踪器
2.quality/BUGS.md-所有从代码审查和规范审核中确认的bug
3.quality/REQUIREMENTS.md派生需求
4.SKILL.md-阅读第5阶段部分（“第5阶段：审查后核对和关闭验证”）。还可以读取references/requirements_pipeline.md、references/review_protocols.md和references/spec_audit.md.。通过上面记录的回退列表解析SKILL.md和references/目录；不要假设任何单一的安装布局。

执行阶段5：协调+ TDD +结束。1. 运行每个references/requirements_pipeline.md.更新COMPLETENESS_REPORT.md的审查后对账。
2. 运行闭包验证：跟踪器中的每个BUG必须有回归测试或显式豁免。
3. 在quality/writeups/BUG-NNN.md为每一个确认的bug写bug报告。规范模板是SKILL.md的“Bug writeup generation”部分（通过上面的回退列表解析）—在编写之前请阅读该部分。使用这里列出的确切字段标题：**摘要，规范参考，代码，可观察结果，深度判断，修复，测试，相关问题**。每一篇书面报告都需要1-4、6、7节；第5节（深度判断）仅在直接代码的结果不明显时触发；第8节（相关问题）仅在存在相关bug时才包含。不要引入模板中没有的字段（没有“Minimal reproduction”作为顶级字段，没有“Patch path：”作为顶级字段）字段（分别属于Spec reference和test）。**强制水化步骤。**在编写writeup之前，重新打开quality/BUGS.md并找到要编写的bug的`### BUG-NNN:`条目。BUGS.md中每个已确认的bug都已经包含了您需要的内容—您的工作是将其复制到编写部分，而不是发明它。如果在BUGS.md中缺少一个字段，那么在PROGRESS.md中会出现一个调和错误，而不是一个要虚构的字段。使用这个字段图：

|BUGS.md字段|写作部分|如何使用|   |----------------------------|------------------------------|-------------------------------------------------------------------------------|
|总结|一个句子命名function/code路径和可观察到的故障。|
|主要需求|规格参考|`- Requirement: REQ-NNN`|
|规范基础|规范参考|`- Spec basis: <doc path + line range(s), semicolon-separated if multiple>`加上从引用行逐字复制的不超过15字的合同报价。|
代码|引用`file:line`并描述当前路径在那里的作用。|
|最小复制|可观察的结果|编织到结果段落中作为触发输入。|
|预期行为+实际行为|可观察结果|实际行为为可观察到的失败；期望值定义了差距。|
|回归测试|测试|`- Regression test: <function name>`-逐字从BUGS.md。|
|补丁（回归）|测试|`- Regression patch: <path>`-逐字从BUGS.md。|
| Patches (fix) | The fix + The test |如果存在修复补丁文件，请读取它并将统一的diff粘贴到“`diff; also list the patch path as `- fix patch:<path>` under The test. If no fix patch exists (confirmed-open bug), write the minimal concrete unified diff directly in The fix anyway — SKILL.md requires an inline diff in every writeup. In the no-patch case, omit the `Fix patch：”子弹中。|
|Red/greenlogs |测试|`- Red receipt: quality/results/BUG-NNN.red.log`和对应的绿色路径。|* *工作的例子。** BUG-004的BUGS.md条目为：       ### BUG-004: naive upstream timestamps crash ETA math
       - Source: Code Review
       - Severity: HIGH
       - Primary requirement: REQ-006
       - Location: bus_tracker.py:138-144
       - Spec basis: quality/REQUIREMENTS.md:163-172; quality/QUALITY.md:57-65
       - Minimal reproduction: Return a visit whose ExpectedArrivalTime is an ISO string
         without timezone information, such as 2026-04-21T12:00:00.
       - Expected behavior: The affected arrival degrades to unknown-time while the rest
         of the stop remains usable.
       - Actual behavior: datetime.fromisoformat() returns a naive datetime and
         subtracting it from datetime.now(timezone.utc) raises TypeError, aborting the
         stop/request path.
       - Regression test: quality.test_regression.TestPhase3Regressions.test_bug_004_fetch_stop_arrivals_degrades_naive_timestamps
       - Patches: quality/patches/BUG-004-regression-test.patch, quality/patches/BUG-004-fix.patch
水合的书写部分看起来像这样（草图-粘贴），真正的区别于
将补丁文件修改为“diff”，不要再编了)：       ## Summary
       fetch_stop_arrivals() crashes the whole stop/request path when an upstream visit
       carries a naive ExpectedArrivalTime, instead of degrading that arrival to
       unknown-time.

       ## Spec reference
       - Requirement: REQ-006
       - Spec basis: quality/REQUIREMENTS.md:163-172; quality/QUALITY.md:57-65
       - Behavioral contract quote: "degrade a bad per-arrival timestamp to unknown-time instead of aborting the whole response path"

       ## The code
       At bus_tracker.py:138-144, the parser calls datetime.fromisoformat(...) on
       ExpectedArrivalTime and subtracts the result from datetime.now(timezone.utc)…

       ## Observable consequence
       When the upstream visit returns ExpectedArrivalTime="2026-04-21T12:00:00"
       (no timezone), fromisoformat() returns a naive datetime, the subtraction
       raises TypeError, and the entire stop/request path aborts rather than the
       single affected arrival degrading to unknown-time.

       ## The fix
       ```diff
       <paste the real unified diff from quality/patches/BUG-004-fix.patch here>
       ```

       ## The test
       - Regression test: quality.test_regression.TestPhase3Regressions.test_bug_004_fetch_stop_arrivals_degrades_naive_timestamps
       - Regression patch: quality/patches/BUG-004-regression-test.patch
       - Fix patch: quality/patches/BUG-004-fix.patch
       - Red receipt: quality/results/BUG-004.red.log
       - Green receipt: quality/results/BUG-004.green.log
**确认清单（每次写入，在移动到下一个bug之前）。** (a)每
所需的部分已填充从BUGS.md或补丁文件复制的内容-
没有空的反引号，没有哨兵填充符，如“是一个被确认的代码错误”或
“受影响的实现存在于‘`" or "Patch path: ``". (b) The `’ ' diff
Fence包含来自实际修复补丁的至少一条`+`或`-`行。(c)
摘要命名一个真实的函数或代码路径，而不是BUG标识符。(d)没有
尖括号占位符（例如，`<...>`）保留在最后的写入中-它们是
来自工作示例和SKILL.md的教学标记是不可接受的
输出。
4. 运行TDD红绿循环：对于每个确认的bug，针对未打补丁的代码运行回归测试—>quality/results/BUG-NNN.red.log.如果存在修复补丁，则针对打补丁的代码运行回归测试—>quality/results/BUG-NNN.green.log.如果测试运行器不可用，则创建NOT_RUN打开的日志第一行。
5. 生成sidecar JSON:quality/results/tdd-results.json和quality/results/integration-results.json（schema_version "1.1"，规范字段：id， requirement, red_phase, green_phase, verdict, fix_patch_present, writeup_path）。
6. 如果存在机械验证工件，运行quality/mechanical/verify.sh并保存收据。
7. 运行终端门验证，写入PROGRESS.md。强制基数门（杠杆3，v1.5.2）

在完成此阶段之前，针对当前的回购状态运行基数调和门。通过与SKILL.md相同的回退列表找到`quality_gate.py`（在每个安装布局中，它与SKILL.md位于相同的目录中），然后将其作为脚本调用—`quality_gate.py`将`check_v1_5_2_cardinality_gate(repo_dir)`作为其标准传递的一部分运行：    python3 <resolved_quality_gate_path> .
其中`<resolved_quality_gate_path>`是在遍历记录的安装位置回退列表时的第一个目标，将`SKILL.md`替换为`quality_gate.py`（例如，`quality_gate.py`、`.claude/skills/quality-playbook/quality_gate.py`、`.github/skills/quality_gate.py`、`.cursor/skills/quality-playbook/quality_gate.py`、`.continue/skills/quality-playbook/quality_gate.py`、`.github/skills/quality-playbook/quality_gate.py`）。

如果gate输出包含任何以`cardinality gate:`开头的行，或者报告未覆盖的单元格、不正确的单元格id、缺少多单元格覆盖的整合理由，或者不正确的降级记录，请停止。修复BUGS.md条目或`compensation_grid_downgrades.json`文件。在这些故障线不再出现之前，不要进行完井。

对于每个模式标记的REQ，第5阶段合同是：
每个带有`"present": false`的网格单元出现在BUG的`Covers:`列表或降级记录中。
-每个`Covers:`条目使用标准单元格ID形式`REQ-N/cell-<item>-<site>`。
-每个具有≥2个`Covers:`条目的BUG都有一个非空的`Consolidation rationale:`行。
—每条降级记录有`cell_id`、`authority_ref`、`site_citation`、`reason_class`（在enum中）、`falsifiable_claim`（非空）。基数门阻塞了。它有意比第3阶段咨询自检更严格；咨询检查是为了尽早发现问题，但第5阶段是问题变得致命的时候。

在PROGRESS.md中标记第五阶段完成（使用复选框格式`- [x] Phase 5 - Reconciliation`-不要切换到表格）。

重要：如果任何writeup缺少一个非空的“`diff block or contains any of these sentinel phrases verbatim: "is a confirmed code bug in ``", "The affected implementation lives at ``", "Patch path: ``", "- Regression test: ``", "- Regression patch: `”，quality_gate.py将FAIL Phase 5。这两张支票是硬门。跳过上面的BUGS.md水合步骤并不是门强制的，但会产生读起来像未填充的存根的文章，并且无法通过人工审查—不要跳过它。