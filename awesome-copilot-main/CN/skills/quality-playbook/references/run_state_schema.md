#运行状态模式（v1.5.6）

*`quality/run_state.jsonl`，`quality/PROGRESS.md`和`Calibration Cycles/<cycle>/run_state.jsonl`的权威模式。剧本AI通过文件工具层直接写入这些文件；编排人工智能读取它们以驱动多基准校准周期

*伴随：`docs/design/QPB_v1.5.5_Design.md`（“设计-运行状态事件分类法”部分）

---

##文件位置和所有权

-`<benchmark>/quality/run_state.jsonl`-每次运行的事件日志。扩展。由执行剧本的AI编写。
-`<benchmark>/quality/PROGRESS.md`-运行状态。每个事件由AI自动重写。
-`Calibration Cycles/<cycle>/run_state.jsonl`-周期级事件日志。扩展。由编曲AI编写。

这三个都位于用户拥有的绑定安装的工作空间中。AI通过Edit/Write文件工具进行写入，而不是通过shell重定向或`tee`（在某些沙盒运行时通过不同的UID层进行路由）。

---

模式版本控制每个`run_state.jsonl`以记录`schema_version`的`_index`事件打开。当前版本：`"1.5.6"`。模式碰撞保持向后兼容性——较旧的文件仍然可以被较新的解析器读取。破坏模式的更改会影响主号。

---

##必填字段（每个事件）

每个事件对象必须具备：

-`ts`- ISO 8601 UTC时间戳，后缀为`Z`（例如`"2026-05-15T14:32:01Z"`）。亚秒精度是允许的，但不是必需的。
-`event`- string事件类型名称。必须匹配`_index.event_types`中列出的名称之一。

事件可以根据下面的类型规范有额外的字段。读取器可以容忍未知字段（向前兼容）。

---

##运行事件（`<benchmark>/quality/run_state.jsonl`）

# # #`_index`总是在第一行。记录模式元数据。

|字段|类型|必选|备注||---|---|---|---|
|`event`| string |是|总是`"_index"`|
|`ts`| string | yes | ISO 8601 UTC |
|`schema_version`| string | yes |`"1.5.6"`|
|`event_types`|字符串的数组|是|每个事件类型这个文件使用|
|`benchmark`| string | yes |例如`"chi-1.3.45"`，`"virtio-1.5.1"`|
|`lever_state`| string | yes |例如`"pre-pattern7"`，`"post-pattern7"`,`"baseline"`|
|`started_at`| string | yes | ISO 8601 UTC，等于此事件的`ts`|

# # #`run_start`标志着剧本的开始。

|字段|类型|必选|备注||---|---|---|---|
|`event`| string | yes |`"run_start"`|
|`ts`| string | yes | |
|`runner`| string | yes |`"claude"`,`"codex"`,`"copilot"`，`"cursor"`|之一
|`playbook_version`| string | yes |例如`"1.5.6-pre"`，`"1.5.6"`（匹配`bin.benchmark_lib.RELEASE_VERSION`） |
|`target_path`| string | yes |基准测试目标|的相对路径

# # #`phase_start`标志着六个剧本阶段之一的开始。

|字段|类型|必选|备注||---|---|---|---|
|`event`| string | yes |`"phase_start"`|
|`ts`| string | yes | |
|`phase`| integer | yes | 1,2,3,4,5或6 |

# # #`pattern_walked`只有第一阶段。记录了七个勘探模式中的一个。

|字段|类型|必选|备注||---|---|---|---|
|`event`| string | yes |`"pattern_walked"`|
|`ts`| string | yes | |
|`phase`| integer |是|总是1 |
|`pattern`| integer | yes | 1 ~ 7 |
|`findings_count`| integer | yes |此模式产生的发现数|
|`duration_seconds`| number |可选|此模式行走的挂钟|

###`pass_started`/`pass_ended`只有第四阶段。记录四个技能派生通道之一的start/end。

|字段|类型|必选|备注||---|---|---|---|
|`event`| string |是|`"pass_started"`或`"pass_ended"`|
|`ts`| string | yes | |
|`phase`| integer | yes | Always 4|
|`pass`| string | yes |`"A"`,`"B"`,`"C"`，`"D"`|之一
|`output_artifact`| string |可选|传递工件的相对路径（在`pass_ended`上）|

# # #`finding_logged`记录在当前阶段记录的发现（技能差异、代码错误等）。

|字段|类型|必选|备注||---|---|---|---|
|`event`| string | yes |`"finding_logged"`|
|`ts`| string | yes | |
|`phase`| integer |是| 1-6 |
|`finding_id`| string | yes |例如`"BUG-007"`，`"REQ-042"`|
|`category`| string | yes |例如`"code-bug"`，`"skill-divergence"`,`"missing-citation"`,`"prose-to-code-mismatch"`|

# # #`artifact_written`记录工件文件为produced/updated.|字段|类型|必选|备注||---|---|---|---|
|`event`| string | yes |`"artifact_written"`|
|`ts`| string | yes | |
|`relative_path`| string | yes |相对于基准目标的路径（例如`"quality/EXPLORATION.md"`） |
|`byte_size`| integer |可选|写时间|的文件大小
|`line_count`| integer |可选|行数|

# # #`gate_check`记录单个质量门检查的结果。

|字段|类型|必选|备注||---|---|---|---|
|`event`| string | yes |`"gate_check"`|
|`ts`| string | yes | |
|`gate_name`| string | yes |来自`quality_gate.py`|的标识符
|`verdict`| string | yes |`"pass"`,`"fail"`,`"warn"`，`"skip"`|之一
|`reason`| string |可选|人类可读解释|

# # #`phase_end`标志一个阶段的结束。在编写之前，对阶段的预期工件进行交叉验证（参见下面的“交叉验证规则”）。

|字段|类型|必选|备注||---|---|---|---|
|`event`| string | yes |`"phase_end"`|
|`ts`| string | yes | |
|`phase`| integer |是| 1-6 |
|`key_counts`|对象|是|阶段特定计数（见下文）|
|`artifacts_produced`|字符串数组| yes |此阶段产生的工件的相对路径|
|`duration_seconds`| number |可选|全相位挂钟|

每相`key_counts`：

-第一期：`{"findings_total": N, "patterns_walked": M}`（整个第一期的M应为7）
—阶段2:`{"findings_promoted": N, "findings_dropped": M}`—阶段3:`{"bugs_identified": N, "bug_writeups": M}`-第4阶段：`{"req_count": N, "uc_count": M, "passes_complete": K}`（K应为4）
—阶段5:`{"gate_checks_total": N, "gate_failures": M}`—阶段6:`{"bugs_md_count": N, "gate_verdict": "pass|fail|partial"}`# # #`error`记录运行过程中的错误。

|字段|类型|必选|备注||---|---|---|---|
|`event`| string | yes |`"error"`|
|`ts`| string | yes | |
|`phase`| integer |可选|如果错误是相位作用域|
|`message`| string |是|人类可读的描述|
|`recoverable`|布尔|是|如果为true，运行将重试受影响的阶段；如果为false，则运行中止|

# # #`documentation_state`v1.5.6 +。记录阶段1条目的文档可用性状态。目前唯一发出的状态是`"code_only"`，这表明`reference_docs/`和`reference_docs/cite/`没有携带可识别的明文内容（`.md`或`.txt`），并且阶段1以纯代码模式进行（参见`references/code-only-mode.md`）。`"with_docs"`值为将来的显式发射保留；今天缺少`documentation_state`事件意味着存在文档。

|字段|类型|必选|备注||---|---|---|---|
|`event`| string | yes |`"documentation_state"`|
|`ts`| string | yes | |
|`state`| string | yes |当前`"code_only"`。未来的值可能包括`"with_docs"`。|
|`reason`|字符串|是|自由格式（例如`"reference_docs/ empty"`） |

当发出`documentation_state state="code_only"`时，剧本还在`quality/EXPLORATION.md`中添加了一个“Documentation status: code-only mode”部分，并在`quality/PROGRESS.md`中添加了一个“Documentation state: code_only”行，这样任何阅读这两个工件的人都可以看到降级。添加`documentation_state`事件的新运行必须将其包含在`_index.event_types`列表中。

# # #`aborted_missing_docs`v1.5.6 +。记录运行在阶段1项终止，因为设置了`--require-docs`，而`reference_docs/`为空。对于相同的第一阶段条目，`documentation_state state="code_only"`是互斥的——`--require-docs`是opt-IN中止路径；该标志的缺失保留了记录的仅代码模式降级。在此事件之后，运行程序返回非零，而不调用任何LLM工作，因此不记录`phase_start phase=1`。

|字段|类型|必选|备注||---|---|---|---|
|`event`| string | yes |`"aborted_missing_docs"`|
|`ts`| string | yes | |
|`reason`|字符串|是|自由形式（例如`"reference_docs/ empty and --require-docs set"`） |

当发出`aborted_missing_docs`时，剧本还将`ERROR: aborted_missing_docs — <reason>`块写入`quality/PROGRESS.md`，这样在不读取JSONL的情况下就可以看到abort。将`--require-docs`传递给空`reference_docs/`的新运行必须在`_index.event_types`列表中包含`aborted_missing_docs`。

# # #`run_end`标志着剧本运行的结束。

|字段|类型|必选|备注||---|---|---|---|
|`event`| string | yes |`"run_end"`|
|`ts`| string | yes | |
|`status`| string | yes |`"success"`,`"aborted"`，`"failed"`|之一
|`total_findings`| integer |可选|所有阶段之和|
|`final_verdict`| string |可选|阶段6门判定|

---

##循环级事件（`Calibration Cycles/<cycle>/run_state.jsonl`）

###`_index`（循环级）

|字段|类型|必选|备注||---|---|---|---|
|`event`| string | yes |`"_index"`|
|`ts`| string | yes | |
|`schema_version`| string | yes |`"1.5.6"`|
|`event_types`|字符串|是| |的数组
|`cycle_name`| string | yes |例如`"2026-05-15-pattern7-displacement-recovery"`|
|`lever_under_test`| string | yes |例如`"lever-1-exploration-breadth-depth"`|
|`benchmarks`|数组的字符串|是|周期的固定基准列表|
|`iteration`| integer | yes |迭代序号（1、2或3 -参见iterate-cap） |

# # #`cycle_start`|字段|类型|必选|备注||---|---|---|---|
|`event`| string | yes |`"cycle_start"`|
|`ts`| string | yes | |
|`hypothesis`| string | yes |循环的可测试假设|
|`noise_floor_threshold`| number | yes |回忆低于此值的delta被视为噪声（默认为0.05）|

# # #`benchmark_start`|字段|类型|必选|备注||---|---|---|---|
|`event`| string | yes |`"benchmark_start"`|
|`ts`| string | yes | |
|`benchmark`| string | yes | |
|`lever_state`| string |是|`"pre-lever"`或`"post-lever"`|

# # #`lever_change_applied`|字段|类型|必选|备注||---|---|---|---|
|`event`| string | yes |`"lever_change_applied"`|
|`ts`| string | yes | |
|`lever_id`| string | yes |例如`"lever-1-exploration-breadth-depth"`|
|`files_changed`| string数组| yes |相对于QPB repo root |的路径
|`commit_sha`| string | yes |在实施分支|上提交SHA
|`description`| string | yes |变化是什么（例如`"Pattern 7 budget cap 3-5 → 2-3"`） |

# # #`lever_change_reverted`|字段|类型|必选|备注||---|---|---|---|
|`event`| string | yes |`"lever_change_reverted"`|
|`ts`| string | yes | |
|`files_changed`|字符串|是| |的数组
|`commit_sha`| string |可选|Null/absent如果还原为未提交|

# # #`benchmark_end`|字段|类型|必选|备注||---|---|---|---|
|`event`| string | yes |`"benchmark_end"`|
|`ts`| string | yes | |
|`benchmark`| string | yes | |
|`lever_state`| string | yes | |
|`recall`| number | yes | 0.0-1.0 |
|`bugs_found`|字符串数组|是|运行|发现的错误id
|`bugs_missed`|字符串数组| yes |基线中的Bug id错过了本次运行|
|`historical_baseline_path`| string | yes |用于召回计算的基线BUGS.md的路径|

# # #`cycle_end`|字段|类型|必选|备注||---|---|---|---|
|`event`| string | yes |`"cycle_end"`|
|`ts`| string | yes | |
|`verdict`| string | yes |`"ship"`,`"revert"`,`"iterate"`，`"halt-iterate-cap"`|之一
|`recall_before`|对象|是|每基准召回之前的杠杆变化|
|`recall_after`|对象|是|每基准召回后的杠杆变化|
|`delta`|对象|是|每个基准delta (recall_after - recall_before) |
|`cross_benchmark_check`|对象|是|`{"clean": bool, "regressions": [list of bench/bug pairs that regressed]}`|

---

交叉验证规则（按`phase_end`）

AI在附加`phase_end`事件之前验证这些条件。如果任何检查失败，AI将用`recoverable: true`追加一个`error`事件，并重新运行失败阶段。

|阶段|所需条件||---|---|
|`quality/EXPLORATION.md`存在，≥120行（与`bin/run_playbook.check_phase_gate`中的第二阶段启动门对齐），包含至少一个查找部分（regex`^##\s+(Finding\|Open Exploration Findings\|\d+\.)`-接受`## Finding ...`， skill规定的精确标题`## Open Exploration Findings`，以及编号的`## N.`标题）|
在`quality/`下，所有九个固定名称的生成契约工件都是非空的：`REQUIREMENTS.md`、`QUALITY.md`、`CONTRACTS.md`、`COVERAGE_MATRIX.md`、`COMPLETENESS_REPORT.md`、`RUN_CODE_REVIEW.md`、`RUN_INTEGRATION_TESTS.md`、`RUN_SPEC_AUDIT.md`、`RUN_TDD_TESTS.md`。加上至少一个非空的`quality/test_functional.<ext>`（扩展名因主要语言而异）。在v1.5.6之前，这一行描述了v1.5.5设计的分类模型（`EXPLORATION_MERGED.md`/`triage.md`）；所提供的SKILL.md/orchestrator_protocol.md/代理文件从未采用该映射，它们总是将阶段2记录为Generate。|
| 3 |`quality/code_reviews/`目录下至少包含一个review文件。如果`quality/BUGS.md`有任何`### BUG-`标题，则`quality/patches/`至少包含一个`BUG-*-regression-test.patch`文件。在v1.5.6之前，这一行检查了`quality/RUN_CODE_REVIEW.md`（a阶段2）不是第3阶段的审查结果)-与第2阶段行相同的v1.5.5-design / shipped-Generate漂移类。集群B和解。|
| 4 |`quality/spec_audits/`目录至少包含一个`*-triage.md`文件和至少一个`*-auditor-*.md`文件（根据orchestrator_protocol.md命名约定）。当两个名称模式都不匹配时，验证器退回到较弱的“≥2个文件”检查-旧的引导运行时任意`.md`名称仍然通过；阶段6的闸门加强了更深层次的一致性。在v1.5.6之前，这一行检查了`quality/REQUIREMENTS.md`+`COVERAGE_MATRIX.md`（阶段2输出）-相同的v1.5.5设计漂移类。集群B和解。|
| 5 |如果`quality/BUGS.md`已经确认了`### BUG-`条目：`quality/results/tdd-results.json`存在非空；对于每一个确认的bug，存在`quality/writeups/BUG-NNN.md`和`quality/results/BUG-NNN.red.log`。由于没有确认的bug，这一行就满足了。在v1.5.6之前，这一行检查了`quality/results/quality-gate.log`（第6阶段输出）—相同的v1.5.5设计漂移类。集群B和解。|
| 6 |`quality/results/quality-gate.log`存在非空且`quality/PROGRESS.md`包含一个`Terminal Gate Verification`部分（阶段6运行脚本验证门直至完成的编排协议标记）。在v1.5.6之前，这一行检查了`quality/BUGS.md`+`quality/INDEX.md`-BUGS.md是第三阶段的输出，INDEX.md从未在发货合同中采用。相同的v1.5.5设计漂移类。集群B和解。|`run_end`事件还需要：日志中出现的所有6个`phase_end`事件；最终的BUGS.md计数与`phase_end phase=6 key_counts.bugs_md_count`匹配。

---

##简历语义

当AI会话在运行目录上启动时：

1. 如果`quality/run_state.jsonl`不存在：fresh run。写`_index`+`run_start`+`phase_start phase=1`。
2. 如果存在：读取所有事件。查找最后一个`phase_start`，后面没有匹配的`phase_end`。称之为“进行中阶段”。
3. 验证正在进行阶段的预期工件（根据上面的交叉验证规则）：
—如果工件完成：添加缺失的`phase_end`事件并进入下一阶段。注意：这是“会话在中期崩溃，但工作已经完成”的恢复路径。
-如果工件不完整：从头开始重新运行该阶段。先前的会话留下了无法安全恢复的部分状态。
4. 如果所有6个`phase_end`事件都存在，但没有`run_end`：添加`run_end status=success`并完成。策略是“信任工件多于信任事件”。如果事件声称阶段4已完成，但`REQUIREMENTS.md`不存在，AI将重新运行阶段4。如果事件在中途停止，但工件已经完成，AI就会赶上事件。

---

##PROGRESS.md格式

在每个事件上自动重写。减价。```markdown
# QPB Run Progress

**Started:** 2026-05-15T14:32:01Z  **Benchmark:** chi-1.5.1  **Lever:** post-pattern7
**Runner:** claude  **Playbook version:** 1.5.6

## Phases

- [x] Phase 1 — Explore (10:10, 12 findings, patterns 1-7 walked)
- [x] Phase 2 — Generate (0:42, 9 artifacts produced)
- [x] Phase 3 — Code Review (15:31, 6 bugs identified)
- [x] Phase 4 — Spec Audit (3 auditors, 1 triage)
- [ ] Phase 5 — Reconciliation *(in progress, started 14:58:31Z)*
- [ ] Phase 6 — Verify

## Recent events (last 10)

- 2026-05-15T14:58:31Z — phase_start phase=5
- 2026-05-15T14:58:30Z — phase_end phase=4 passes=[A,B,C,D] req_count=89
- 2026-05-15T14:42:11Z — phase_end phase=1 findings=12

## Artifacts produced

- quality/EXPLORATION.md (12,034 bytes)
- quality/REQUIREMENTS.md (28,891 bytes)
- quality/COVERAGE_MATRIX.md (3,022 bytes)
```
章节（标题、阶段检查表、最近的事件、产生的工件）是必需的。阶段检查表使用`[x]`表示已完成的阶段（带有汇总统计），`[ ]`表示未完成的阶段，并使用开始时间显式地标记正在进行的阶段。最近事件以人类可读的形式显示`run_state.jsonl`的最后10个事件行。生成的工件显示了本次运行以字节大小写入的文件。

---

格式不变量（由`bin/run_state_lib.py`验证器强制执行）1.`_index`是第一行。
2. 每行都是有效的JSON（每行一个对象）。
3. 每个事件都有`ts`和`event`字段。
4. 每个`event`值都出现在`_index.event_types`中。
5. 仅追加：事件被添加，不被编辑。编辑先前的事件违反了模式。
6. 给定阶段的`phase_start`和`phase_end`事件每次运行最多出现一次（没有乱序或重复的阶段标记）。
7.`run_start`为第二行（在`_index`之后）；`run_end`是运行完成后的最后一行。

验证器是只读检查。他们把违规行为当作调查结果来处理；它们不会自动更正。