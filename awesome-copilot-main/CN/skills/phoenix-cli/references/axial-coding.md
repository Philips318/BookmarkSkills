#轴向编码

将开放式的观察结果分组到结构化的故障分类中。轴向编码将注释、跟踪观察或开放编码输出转换为带有计数的命名类别，支持诸如评估设计和修复优先级等下游工作。它在[开放编码]（open-coding.md）之后工作得很好，但可以从任何一组开放式观察开始。

**每当用户有观察和需求结构时，都要做到这一点——例如，“我们有什么类型的故障”，“我应该为什么构建评估”，“我如何优先考虑修复”，“分组这些笔记”，“MECE故障”，或任何要求分类或计数的框架，这些框架基于真实的痕迹，而不是自上而下的发明。

编码注释标识符（重用open-coding值）重用开放编码中选择的编码注释标识符——下面的每个`annotate`调用都显式地传递`--identifier "$CODING_ANNOTATION_IDENTIFIER"`。在新的shell或新的代理调用中，将`CODING_ANNOTATION_IDENTIFIER`设置为相同的值（可从封装UI URL或通过列出`.px/coding/*.jsonl`恢复）；不要伪造新的身份证。请参阅[open-coding.md#coding-annotation-identifier-pick-this-first]（open-coding.md#coding-annotation-identifier-pick-this-first）了解基本原理和清理规则。

b> **工作流术语与服务器注释名称。**技能将此值称为**编码注释标识符**；用于UI过滤器的服务器注释NAME保持`coding_session_id`以保持数据兼容性。不要尝试重命名服务器端密钥。```bash
CODING_ANNOTATION_IDENTIFIER="coding-run:chatbot-context-loss-2026-05-06"
SLUG=$(echo -n "$CODING_ANNOTATION_IDENTIFIER" | sed 's/[^a-zA-Z0-9_-]/-/g')
NOTES_SIDECAR=".px/coding/${SLUG}.jsonl"
AXIAL_SIDECAR=".px/coding/${SLUG}-axial.jsonl"
```
##选择单位

[open-coding.md#选择分析单元]中的开放编码诊断（open-coding.md#选择分析单元）提交到一个单元（跟踪、范围或会话）。轴向编码在默认情况下继承该单元——如果开放编码在会话级别运行，轴向标签也会；trace和span也是一样。

**轴向标签可以存在于不同的层次，而不是通知它的注释** -这是一个功能，它可以在任何方向上工作：- *Trace→span*：一旦模式显示检索是一致的罪魁祸首，跟踪级别的注释“当询问有关返回时回答的shipping”可以在检索范围上产生一个跨度级别的注释。
*Trace→session*：一批描述单回合混乱的跟踪级注释可以产生会话级注释，一旦你看到模式是“代理不跟踪用户在回合中所陈述的上下文”。
- *Session→trace*：一个关于跨回合漂移的会话级记录，仔细阅读，可以归因于代理丢弃线程的特定回合；跟踪级注释可以命名该转弯。

无论您在哪个级别上编写轴向标签，都要在同一实体上编写匹配的`coding_session_id`UI-filter注释（参见下面的[UI-filter annotation](# UI-filter -annotation)），以便UI链接拾取它。

# #过程1. **设置编码标注标识符** -设置`CODING_ANNOTATION_IDENTIFIER`为开放编码使用的值，重新派生`SLUG`，`NOTES_SIDECAR`,`AXIAL_SIDECAR`（参见[编码标注标识符](#coding-annotation-identifier-reuse-the-open-coding-value)）
2. **收集** -从`$NOTES_SIDECAR`读取开放编码笔记（在开放编码提交的单元）；无服务器往返
3. **模式** -用共同的主题分组笔记
4. **名称** -创建可操作的类别名称
5. **属性** -决定每个类别所处的级别；轴向标签可以向上移动（跟踪→会话）或向下移动（跟踪→跨度）从源笔记的级别到模式实际涉及的级别
6. **记录** -`px {trace,span,session} annotate ... --name axial_coding_category --label <cat> --identifier "$CODING_ANNOTATION_IDENTIFIER"`，add/update一个JSONL sidecar行作为标签，然后编写匹配的`coding_session_id`UI-filter注释
7. **量化** -从`$AXIAL_SIDECAR`计算每个类别的失败

##示例分类```yaml
failure_taxonomy:
  content_quality:
    hallucination: [invented_facts, fictional_citations]
    incompleteness: [partial_answer, missing_key_info]
    inaccuracy: [wrong_numbers, wrong_dates]

  communication:
    tone_mismatch: [too_casual, too_formal]
    clarity: [ambiguous, jargon_heavy]

  context:
    user_context: [ignored_preferences, misunderstood_intent]
    retrieved_context: [ignored_documents, wrong_context]

  safety:
    missing_disclaimers: [legal, medical, financial]
```
# #阅读

# # # 1。收集-从侧斗上阅读这次运行的开放编码笔记

开放编码为`$NOTES_SIDECAR`（`.px/coding/${SLUG}.jsonl`）编写了一个JSONL行。直接读取它——不需要往返服务器。每一行都有`entity_kind`、`entity_id`、`note`、`identifier`和`ts`。如果同一个`(entity_kind, entity_id)`出现多次，则使用最新的`ts`作为当前注释。

* *文件丢失的行为。**缺少`$NOTES_SIDECAR`表示在此CWD中未运行此编码注释标识符的打开编码-请先停止并运行打开编码，不要将其视为零音符。

* *畸形。**每一行都是独立可解析的JSON。如果`jq`报告解析错误，手动修复或删除该行；不要编辑其他行。**本次运行之外的注释。**挎斗只装这名CWD写的笔记。要提取另一个审阅者或更早运行时编写的注释，可以通过`px {trace,span,session} list --include-notes`（将注释嵌入到行输出中）获取它们——工作流的侧车故意是按cwd按编码标识符进行的。

# # # 2。分组-综合类别

回顾上面收集的笔记文本。手动确定重复出现的主题和候选类别名称。针对MECE的报道：每个笔记应该只适合一个类别。

# # # 3。记录-写入轴向编码标签

使用`px {trace,span,session} annotate`为每个实体编写一个注释，在每次调用时显式地传递`--identifier "$CODING_ANNOTATION_IDENTIFIER"`，并在`$AXIAL_SIDECAR`中记录一行JSONL，以便下面的[Quantify]（#4-quantify—count-per-category-from-the-axial-sidecar）无需服务器往返即可计数。音阶可以与源音所在的位置不同-参见下面的[录音]（#录音）。# # # 4。量化-从轴向侧车计算每个类别

计数来自`$AXIAL_SIDECAR`（由[Record]（#3-record—write-axial-coding-labels）填充）。没有服务器查询，没有项目范围的历史记录混合在一起——侧车保存的正是这次运行所写的标签。按`axial_label`计算当前行；如果一个实体出现多次，请使用最新的`ts`。

与`$NOTES_SIDECAR`相同的文件缺失和格式错误规则：缺少轴向侧车意味着尚未写入标签（运行[Record](#3-record—write-axial-coding-labels)）；格式错误的行是行局部修复或删除，不要编辑相邻行。

# #记录

对**标签所属的级别使用匹配的注释命令-这可能与源注释所在的级别不同（参见[选择单元](# Choosing -the-unit)）。每个调用都携带`--identifier "$CODING_ANNOTATION_IDENTIFIER"`和`--format raw --no-progress`，并与`$AXIAL_SIDECAR`中的JSONL行配对。**轴向侧车JSONL线形（每`annotate`一个）：**```json
{"entity_kind":"trace","entity_id":"<trace-id>","annotation_name":"axial_coding_category","axial_label":"<label>","explanation":"<optional explanation>","identifier":"<original identifier value, unsanitized>","ts":"<ISO-8601 UTC>"}
```
字段:
—`entity_kind`—`"trace"`、`"span"`或`"session"`（匹配`annotate`子命令）
-`entity_id`-传递给`annotate`的实体参数`annotation_name`-轴向标签总是`"axial_coding_category"`（工作流的保留注释名称）
-`axial_label`-`--label`值，逐字输入；这就是[Quantify]（#4-quantify- count-per-category-from- axial sidecar）小组所做的
-`explanation`-可选，但当`annotate`调用使用`--explanation`时包括它
-`identifier`- **原始**`$CODING_ANNOTATION_IDENTIFIER`值，未经消毒；经过清理的表单只存在于文件名中
-`ts`- ISO-8601 UTC时间戳

如果在相同的编码注释标识符下修改同一实体的标签，要么替换该行，要么追加新行。当存在重复的`(entity_kind, entity_id, annotation_name)`行时，最新的`ts`是当前标签。这与`annotate --identifier`的服务器启动行为相匹配。

最小跟踪示例：```bash
px trace annotate <trace-id> \
  --name axial_coding_category \
  --label answered_off_topic \
  --explanation "asked about returns; answer covered shipping" \
  --annotator-kind HUMAN \
  --identifier "$CODING_ANNOTATION_IDENTIFIER" \
  --format raw --no-progress
```
然后使用上面的线形向`$AXIAL_SIDECAR`添加匹配的JSONL行。对于跨度或会话标签，请相应地更改`entity_kind`、`entity_id`和`px`子命令。

可接受的标志：`--name`、`--label`、`--score`、`--explanation`、`--annotator-kind`（`HUMAN`、`LLM`、`CODE`）、`--identifier`。没有`--sync`标志—CLI本身传递`sync=true`。

### UI-filter annotation

在与轴标签相同的级别上编写`coding_session_id`注释—请参阅[open-coding.md# UI -filter-annotation]（open-coding.md# UI -filter-annotation）了解Phoenix UI过滤器为什么需要基于名称的注释而不是简单的`--identifier`。如果开放编码已经在同一实体上编写了`coding_session_id`，则此调用将终止（幂等）。注释NAME`coding_session_id`不变；只有工作流的口头术语是“编码注释标识符”。```bash
# Same level as the axial label above
px trace annotate <trace-id> \
  --name coding_session_id \
  --label "$CODING_ANNOTATION_IDENTIFIER" \
  --identifier "$CODING_ANNOTATION_IDENTIFIER"
# or px span annotate / px session annotate at matching levels
```
记录纪律

轴向编码对您在开放编码期间记录的实体进行分类。使用`$NOTES_SIDECAR`作为候选实体的源，仅在读取注释文本和周围的trace/span/session上下文之后才写入标签。**不要**过滤`--status-code ERROR`-它只捕获Python引发的跨度，这排除了大多数失败模式（幻觉，错误音调，检索失败）。参见[open-coding.md]（open-coding.md#inspection）了解完整的推理。

**回退路径：** REST`POST /v1/{trace,span,session}_annotations`和`@arizeai/phoenix-client`的`addSpanAnnotation`/`addSessionAnnotation`（没有`addTraceAnnotation`导出-使用REST或`px trace annotate`）。GraphQL端点拒绝更改。

##结束

轴向编码完成后，与用户共享Phoenix UI链接。该链接指向由`coding_session_id`注释（`annotations['coding_session_id'].label == '<coding-annotation-id>'`）过滤的项目跟踪表。UI路由`/projects/:projectId`需要一个编码的GraphQL节点ID，而不是一个项目名称-通过`px project get`解析它：```bash
project_id=$(px project get "$PHOENIX_PROJECT" --format raw --no-progress | jq -r '.id')
encoded=$(python3 -c 'import urllib.parse, sys; print(urllib.parse.quote(sys.argv[1]))' \
  "annotations['coding_session_id'].label == '$CODING_ANNOTATION_IDENTIFIER'")
echo "Phoenix UI: $PHOENIX_HOST/projects/$project_id/traces?filterCondition=$encoded"
```
如果用户想要丢弃这次运行产生的所有内容（开放编码注释、轴向编码标签、服务器上的`coding_session_id`注释以及本地侧车），那么三个标识符绑定的删除处理服务器端，一个`rm`处理本地侧车。**运行前确认** -破坏性。每个`px <entity>-annotations delete`调用都需要`--all`授权无界扫描；`--identifier`只会变窄。如果还没有导出，先设置`PHOENIX_CLI_DANGEROUSLY_ENABLE_DELETES=true`：```bash
for kind in trace span session; do
  px "$kind-annotations" delete \
    --identifier "$CODING_ANNOTATION_IDENTIFIER" \
    --all -y \
    --format raw --no-progress
done
rm -f "$NOTES_SIDECAR" "$AXIAL_SIDECAR"
```
每个`px <entity>-annotations delete`调用一起删除注释、轴向编码标签和`coding_session_id`注释，因为它们共享底层注释表；`rm`清除局部侧车。

代理故障分类```yaml
agent_failures:
  planning: [wrong_plan, incomplete_plan]
  tool_selection: [wrong_tool, missed_tool, unnecessary_call]
  tool_execution: [wrong_parameters, type_error]
  state_management: [lost_context, stuck_in_loop]
  error_recovery: [no_fallback, wrong_fallback]
```
过渡矩阵- jq草图

要查找代理状态之间发生故障的位置，请在跟踪中的每个第一个错误范围之前确定最后一个非错误范围。注意：OTel在`status_code == "UNSET"`处留下大多数跨度，只有在代码明确匹配`!= "ERROR"`而不是`== "OK"`时才设置`"OK"`，因此矩阵适用于典型的OTel数据。```bash
px span list --format raw --no-progress | jq '
  group_by(.context.trace_id)
  | map(
      sort_by(.start_time)
      | { trace_id: .[0].context.trace_id,
          last_non_error: map(select(.status_code != "ERROR")) | last | .name,
          first_err:      map(select(.status_code == "ERROR")) | first | .name }
    )
  | [ .[] | select(.first_err != null) ]
  | group_by([.last_non_error, .first_err])
  | map({ transition: "\(.[0].last_non_error) → \(.[0].first_err)", count: length })
  | sort_by(-.count)
'
```
使用输出统计哪些状态到状态的转换最容易发生故障，并将它们添加到分类法中。

什么是好的分类

一个有用的分类是：
- **命名的原因**，而不是症状（"wrong_tool_selected“，而不是”bad_output"）
- **绑定到一个修复** -如果你不能命名一个修复，类别太模糊
- **以数据为基础** -从实际笔记文本中出现，而不是预先假设

# #原则- **每次运行一个编码注释标识符** -每个`annotate`调用和每个侧车行携带`$CODING_ANNOTATION_IDENTIFIER`，使用相同的值开放编码；永远不要在中途铸造新的id。
**显式传递`--identifier`** -每个`px`调用得到`--identifier "$CODING_ANNOTATION_IDENTIFIER"`；不要依赖遗传的嫉妒者。
** -收集和量化读取`$NOTES_SIDECAR`和`$AXIAL_SIDECAR`本地；记录写入服务器并更新sidecar。如果一个实体出现多次，则最新的`ts`获胜。
- **MECE** -每个故障属于一个类别。
- **可操作的** -类别建议修复。
- **自下而上** -让分类从数据中产生。
- **UI-filter注释总是成对的** -永远不要写`axial_coding_category`而不写匹配的`coding_session_id`注释；UI链接依赖于它。