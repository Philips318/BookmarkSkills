#开放编码

在任何分类法存在之前，针对采样的轨迹、跨度或会话进行自由形式的笔记。在您选择了正确单元的样本之后（参见[选择分析单元](# Choosing -the-unit-of-analysis)），阅读每个样本，并写一篇简短的、具体的观察，看看哪里出了问题。这些原始记录提供[轴向编码](axial-coding.md)，在那里它们被分组到命名的故障类别中，并最终进入评估目标或修复优先级。

当用户想要在没有固定分类的情况下查看LLM流量时，可以使用这个方法，例如，“这个代理出了什么问题”，“我刚刚检测了我的应用程序，我从哪里开始”，“检查这些痕迹”，“聊天机器人总是失去上下文”，“模型犯了什么错误”，“帮助我理解这些对话”，或者任何需要在分类之前进行基础观察的框架。

选择分析单元正确的单位——跟踪、跨度或会话——取决于问题和系统。录音前要慎重挑选；这个选择决定了在整个过程中是调用`px trace`、`px span`还是`px session`，错误的默认值在运行过程中撤销代价很高。

这个单元是关于你正在调查的失效模式实际存在的地方。- **跟踪** -一个输入→一个调用图→一个输出。适合分类器、单次摘要器、无状态工具代理、单查询RAG。这里存在的故障模式：错误的答案，错误的输出，错过的检索，在一个请求中错误的工具选择。
- **Span** -跟踪内的一个操作。适用于隔离机械故障（触发异常、工具返回错误响应、输出格式错误）或您可以立即将其归因于特定组件。当跟踪作为一个整体是好的，但其中的一个部分是感兴趣的单位时，可以使用span。
- **会话** -一个序列的痕迹共享一个`session.id`。对于多回合对话代理，具有情景记忆的代理，任何失败模式是“轨迹”的东西：回合间上下文丢失，偏离用户声明的目标，代理忘记声明的偏好，重复的用户澄清规划设计。这些失败并不存在于任何单一的轨迹上；它们只存在于* *路径之间。###诊断-三个信号读取

1. * *用户框架。** *倾斜会话*：“对话”，“代理忘记”，“漂移”，“记忆”，“跨回合”，“用户必须重复自己”。*倾斜跟踪*：“此跟踪”，“此调用”，“响应错误”，“错误输出”。*倾斜跨度*：“异常”，“错误响应”，“畸形”，“检索失败”。

2. * *数据的形状。**在循环前探测。会话id位于`rootSpan.attributes["session.id"]`（它*不是*跟踪JSON上的顶级字段），并且对于没有会话连接的跟踪是`""`-过滤两者：   ```bash
   px trace list --limit 200 --format raw --no-progress \
     | jq '
       [ .[] | .rootSpan.attributes["session.id"] // empty | select(. != "") ]
       | { with_session: length,
           distinct_sessions: (group_by(.) | length),
           median_traces_per_session:
             (group_by(.) | map(length) | sort | .[length/2|floor] // 0) }
     '
   ```
会话未连接；痕迹就是纹路。`median_traces_per_session: 1`→单跟踪会话；仍然跟踪。`median_traces_per_session: 5+`→会话有意义；塞申斯似乎是正确的。

3. * *系统的类型。**打开一个最近的跟踪并检查根跨度的输入。单个用户消息→一个回合或一个镜头。一个消息*数组* (`[{role: user}, {role: assistant}, ...]`)→这是一个较长的对话中的一个回合；对话存在于会话级别。   ```bash
   px trace get <trace-id> --format raw \
     | jq '.rootSpan.attributes["input.value"] | (try fromjson catch .) | (type, length?)'
   ```
大声说出来，然后继续

在记录任何笔记之前明确说明单位：

问题：“聊天机器人总是失去语境”。数据：平均每个会话7个跟踪，消息数组输入。在**会话**级别进行录音；单圈观察将降至**trace**，机械故障将降至**span**。”

如果数据需要，该单位可以进行转移——一项追踪级别的调查显示，“特工从不记得早些时候的转弯”，应该转向会话。记录观察结果，然后重新对焦下一批。单位是一个起始假设，而不是契约。

##编码注释标识符（先选这个）这个工作流产生的每个工件——开放编码注释、轴向编码标签、本地sidecar文件和UI-filter注释——都被标记为一个编码注释标识符，因此运行是可查询的、可逆的和可作为一个单元查看的。在记录任何笔记之前，选择一个描述性的、唯一的标识符。格式建议:    coding-run:<short-topic>-<YYYY-MM-DD>
示例：`coding-run:chatbot-context-loss-2026-05-06`、`coding-run:agent-tool-misuse-q2`。描述性id对以后打开数据的人都有意义——比不透明的uid要好。`coding-run:`前缀是一种视觉约定；该值是工作流的编码注释标识符，而不是`px session`id。

b> **工作流术语与服务器注释名称。该技能将该值称为编码标注标识符**。用于UI过滤器的服务器端注释NAME保持不变—`coding_session_id`—以便与已写入的行保持数据兼容性。不要试图重命名它。

在每次`px`调用时显式地传递标识符。使用shell变量提高可读性是可以的，但是不要依赖于shell继承—许多代理工具在一个新的子shell中生成每个命令，因此`CODING_ANNOTATION_IDENTIFIER`可能不会传播。```bash
CODING_ANNOTATION_IDENTIFIER="coding-run:chatbot-context-loss-2026-05-06"
```
本地侧车位于`.px/coding/<sanitized-identifier>.jsonl`（相对于cwd，与`.px/docs`匹配）。清理规则：在使用文件名中的值之前，将任何不匹配`[a-zA-Z0-9_-]`的字符替换为`-`-冒号、斜杠和其他shell脆弱字符被规范化。对于`CODING_ANNOTATION_IDENTIFIER="coding-run:chatbot-context-loss-2026-05-06"`，侧车路径为`.px/coding/coding-run-chatbot-context-loss-2026-05-06.jsonl`。

验证这个运行是否已经开始-唯一性是一个**本地文件检查**，而不是服务器查询：```bash
SLUG=$(echo -n "$CODING_ANNOTATION_IDENTIFIER" | sed 's/[^a-zA-Z0-9_-]/-/g')
SIDECAR=".px/coding/${SLUG}.jsonl"
test ! -f "$SIDECAR" || { echo "Sidecar already exists at $SIDECAR — pick a new identifier or delete the file"; exit 1; }
mkdir -p .px/coding
```
如果`$SIDECAR`已经存在，则向`CODING_ANNOTATION_IDENTIFIER`追加消歧符（`-v2`、`-dustin`等），重新派生`SLUG`，并重新检查。代理线束可以在独立的调用中运行开放编码和轴向编码：每一步都从同一个文件`CODING_ANNOTATION_IDENTIFIER`和reads/writes重新派生`SLUG`。

# #过程1. **选择一个编码注释标识符** -选择一个描述性的值，并验证侧车文件还不存在（参见[编码注释标识符](#coding-annotation-identifier- Pick -this-first)）
2. **选择单元** -通过[选择分析单元]（# Choosing -the-unit-of-analysis）并提交跟踪、跨度或会话
3. **Inspect** -在选定的单元（trace / span / session）中获取一个实体
4. **读取** -输入，输出，异常，工具调用，检索上下文，以及（在会话级别）跨子跟踪的轨迹
5. **注** -写一个具体的句子来描述哪里出错了（如果正确的话可以跳过）
6. **记录** -`px {trace,span,session} add-note <id> --text "..." --identifier "$CODING_ANNOTATION_IDENTIFIER" --format raw --no-progress`，add/update一个JSONL sidecar行为备注，然后写匹配的[UI-filter annotation]（# UI-filter -annotation）
7. **迭代** -移动到下一个实体；重复，直到样品耗尽或达到饱和
8. **手动关闭** -轴向编码直接读取sidecar（不需要共享shell）；请参阅[wrap up]（#wrap -up）获取UI链接# #检验

使用`px`读取[选择单元]中提交的单元的上下文：

- **跟踪单元** -读取一个跟踪的输入→工具调用→检索上下文→作为一个故事输出。
- **Span单位** -读取一个操作的input/output和周围的上下文跨度。
- **会话单元** -按顺序读取跟踪序列；轨迹（转弯、检索、跨轨迹的工具调用模式）是数据，而不是任何单个轨迹的输入和输出。

**不要用`--status-code ERROR`过滤样品。** OTel的`status_code`仅在仪器捕获到引发的Python异常（网络故障，5xx，解析错误）时才翻转为`ERROR`。幻觉、错误的语气、检索错误和错误的工具选择都干净利落地完成，并以`OK`或`UNSET`的形式到达。`--status-code ERROR`对开放编码的抽样排除了该工作流存在的人口。```bash
# Sample recent traces — the unit of inspection in open coding
px trace list --limit 100 --format raw --no-progress | jq '
  .[] | {trace_id: .traceId, root: .rootSpan.name, status,
         input: .rootSpan.attributes["input.value"],
         output: .rootSpan.attributes["output.value"]}
'

# Trace-level context — all spans in one trace, ordered by start_time
px trace get <trace-id> --format raw | jq '
  .spans | sort_by(.start_time) | map({span_id: .context.span_id, name, status_code,
    input: .attributes["input.value"],
    output: .attributes["output.value"]})
'

# Drill to one span (px span get does not exist; filter via span list)
px span list --trace-id <trace-id> --format raw --no-progress \
  | jq '.[] | select(.context.span_id == "<span-id>")'

# Check existing notes on traces (default) or spans you are about to review
# Notes are stored as annotations with name="note"; use --include-notes (not --include-annotations)
px trace list --include-notes --limit 10 --format raw --no-progress | jq '
  .[] | select((.notes // []) | length > 0)
  | {trace_id: .traceId, notes: [.notes[] | .result.explanation]}
'
# Same shape on spans — swap px trace for px span and use .context.span_id
```
在编写脚本时，始终使用`--format raw --no-progress`管道通过`jq`。

##记录笔记

使用`add-note`命令，匹配在[选择单元]（# choose -the-unit-of-analysis）中提交的单元：`px trace add-note`、`px span add-note`或`px session add-note`。每个调用都带有显式的`--identifier "$CODING_ANNOTATION_IDENTIFIER"`和`--format raw --no-progress`。

传递`--identifier "$CODING_ANNOTATION_IDENTIFIER"`做两件事：
-用服务器上的编码注释标识符标记注释行，这样清理`px <entity>-annotations delete --identifier "$CODING_ANNOTATION_IDENTIFIER" --all`扫描就会删除这次运行产生的每个工件。
-在`(entity_id, name='note', identifier)`上调用**upsert** -在相同编码注释标识符内的同一实体上重新运行打开编码会覆盖先前的注释，而不是附加第二行。（如果没有`--identifier`，服务器会标记一个唯一的`px-{kind}-note:<uuid>`，并且每个调用都会追加。）在每个成功的`add-note`之后，在`$SIDECAR`中记录一行JSONL。sidecar是轴向编码所读取的内容——没有服务器往返。这是一个内容移交，而不是代码移交：保持它的可读性，直接检查它，并使用任何简单的工具是方便的。

**边车JSONL线形状（每个`add-note`一个）：**```json
{"entity_kind":"trace","entity_id":"<trace-id>","note":"<text>","identifier":"<original identifier value, unsanitized>","ts":"<ISO-8601 UTC>"}
```
字段:
—`entity_kind`—`"trace"`、`"span"`或`"session"`（匹配所使用的`add-note`子命令）
-`entity_id`-传递给`add-note`的实体参数（跟踪id， span id或会话id）
-`note`-`--text`值
-`identifier`- **原始**`$CODING_ANNOTATION_IDENTIFIER`值，未经消毒；经过清理的表单只存在于文件名中
-`ts`- ISO-8601 UTC时间戳（如`2026-05-08T17:14:09Z`）的本地追加

如果在相同编码注释标识符下修改同一实体的注释，请替换该行或追加新行。当存在重复的`(entity_kind, entity_id)`行时，最新的`ts`就是当前的注释。这与`add-note --identifier`的服务器启动行为相匹配。

最小跟踪示例：```bash
px trace add-note <trace-id> \
  --text "Asked about returns; final answer covered shipping policy instead" \
  --identifier "$CODING_ANNOTATION_IDENTIFIER" \
  --format raw --no-progress
```
然后使用上面的线形向`$SIDECAR`添加匹配的JSONL行。对于跨度或会话记录，请相应地更改`entity_kind`、`entity_id`和`px`子命令。

通过状态码（例如`px span list --status-code ERROR | xargs ... add-note "error"`）进行批量自动标记**不是开放编码** -开放编码是手动的，基于观察的，并且涵盖所有故障模式，而不仅仅是Python引发的范围。跳过按状态代码划分容量的快捷方式；与行走的痕迹相比，它产生的信息更少、更少。

### UI-filter annotation接收开放编码注释（或稍后的轴向编码标签）的每个实体还需要一个UI过滤器注释，以便Phoenix UI可以通过编码注释标识符进行过滤。Phoenix的UI过滤语言是基于名称的，而不是基于标识符的——没有用于按`identifier`进行过滤的UI原语，因此，注释的**name**是常量`coding_session_id`，其**label**是编码注释标识符值的注释才是封装UI链接实际过滤的内容。

注释NAME`coding_session_id`是服务器上的承载数据键，在这次重写中没有改变。该技能的工作流程术语为“编码标注标识符”；服务器键保持为`coding_session_id`，以便与已写入的行兼容。

每个被触摸的实体运行一次，与`add-note`一起运行（稍后当轴向编码标记不同的实体时再次运行）：```bash
px trace annotate <trace-id> \
  --name coding_session_id \
  --label "$CODING_ANNOTATION_IDENTIFIER" \
  --identifier "$CODING_ANNOTATION_IDENTIFIER"
# or px span annotate / px session annotate at matching levels
```
注释的`--identifier`与`$CODING_ANNOTATION_IDENTIFIER`匹配，因此[wrap-up DELETE]（# wrapped -up）在与注释和轴向编码标签相同的调用中清除它。

**回退写入路径（一行除外）：**

-`POST /v1/trace_notes`和`POST /v1/span_notes`和`POST /v1/session_notes`-接受一个`{data: {trace_id|span_id|session_id, note, identifier}}`每个请求；可选的`identifier`字段在非空时将在`(entity_id, name='note', identifier)`上启动。
-`@arizeai/phoenix-client``addTraceNote`、`addSpanNote`和`addSessionNote`包装相同的端点，并在注释对象上接受一个可选的`identifier`字段。
- GraphQL端点拒绝使用`"Only queries are permitted."`进行更改-通过`px {trace,span,session} add-note`或上面的REST端点进行写入。

什么是好的音符

|弱音|为什么弱|好音|为什么强|| -------------------- | ------------------------- | -------------------------------------------------------------------------- | ------------------------------------------- |
|“错误答案”|没有可观察到的细节|“说商店下午6点关门，但政策是晚上9点”|观察到的报价与正确值|
| “语气不好” |判断模糊| “企业支持票使用的名问候语” |上下文不匹配|
|“幻觉”|观察|之前的标签“引用了一个模式中不存在的产品特性（‘自动更新’）”|描述了编造的内容|
|“检索问题”|类别，而不是观察|“当问题是关于退货时检索到的关于发货的文档”|列出检索到的内容与需要的|
| “模型混淆” |不透明| “当用户用英语写” |可观察和可复制| "时，用西班牙语回答写下你看到的东西，而不是你认为它属于的类别——分类发生在[轴向编码]（axial-coding.md）中。像`TONE:`或`FACTUAL:`这样的短前缀是一种个人速记，而不是回购惯例。

# #饱和

当观察结果不再新鲜时，不要再写笔记了。信号:

**重复** -最后10-15个跟踪产生的注释描述了您已经看到的失败。
- **释义聚合** -你发现自己在写早期笔记的小变化。
- **跳过的次数超过注释** -最近的痕迹是正确的，不需要注释。

在饱和时，移动到[轴向编码]（axial-coding.md）来分组你所拥有的。继续过去的饱和会增加痕迹，但不会增加洞察力。您不需要注释每个跟踪-注释正确的跟踪会稀释信号。

##列出运行产生的结果本地侧车是本次运行的笔记的交接记录。直接检查。每一行是一个音符记录；如果同一实体出现多次，则使用最新的`ts`作为当前注释。缺少文件行为：缺少sidecar意味着该编码注释标识符尚未开始打开编码；把它当作零音符，而不是错误。格式错误的行是行局部的：修复或删除错误的行而不编辑相邻行。

##结束

运行完成后，与用户共享Phoenix UI链接。该链接通过每个注释旁边的`coding_session_id`注释过滤项目的跟踪页面。UI路由`/projects/:projectId`需要一个编码的GraphQL节点ID，而不是一个项目名称-通过`px project get`解析它：```bash
project_id=$(px project get "$PHOENIX_PROJECT" --format raw --no-progress | jq -r '.id')
encoded=$(python3 -c 'import urllib.parse, sys; print(urllib.parse.quote(sys.argv[1]))' \
  "annotations['coding_session_id'].label == '$CODING_ANNOTATION_IDENTIFIER'")
echo "Phoenix UI: $PHOENIX_HOST/projects/$project_id/traces?filterCondition=$encoded"
```
如果用户想要丢弃这次运行产生的所有内容，三个标识符绑定的删除处理服务器端，一个`rm`处理本地侧车。**运行**前与用户确认-这是破坏性的。每个调用都需要`--all`（或`--start-time`和`--end-time`）来授权扫描；`--identifier`进一步进行过滤，但从不单独授权。如果还没有导出，先设置`PHOENIX_CLI_DANGEROUSLY_ENABLE_DELETES=true`：```bash
for kind in trace span session; do
  px "$kind-annotations" delete \
    --identifier "$CODING_ANNOTATION_IDENTIFIER" \
    --all -y \
    --format raw --no-progress
done
rm -f "$SIDECAR" ".px/coding/${SLUG}-axial.jsonl"
```
每个`px <entity>-annotations delete`调用一次包含注释、结构化注释和`coding_session_id`注释，因为它们共享底层注释表。

# #原则- **每次运行一个编码注释标识符** -每个服务器工件和每个侧车行携带相同的`$CODING_ANNOTATION_IDENTIFIER`；永远不要创建每级id。
**显式传递`--identifier`** -每个`px`调用得到`--identifier "$CODING_ANNOTATION_IDENTIFIER"`；不要在利用派生的子shell中依赖继承的羡慕器。
**Sidecar是笔记的切换记录** -轴向编码从本地Sidecar读取，而不是从服务器读取；如果一个实体出现多次，则最新的`ts`获胜。
- **自由形式的过度结构化** -在开放编码期间不预先提交分类法；类别在轴向编码中出现。
- **具体优于一般** -引用或解释观察到的失败；模糊的标签（“糟糕的回应”）没有任何意义。
- **标签前的上下文** -检查输入，输出，并在写任何笔记之前检索上下文。
-在分类之前进行迭代-首先完成整个样本；不要在还冷的时候分组恳请。
- **Skip是有效的** -一个正确的跨度不需要注意；注释一切会稀释信号。
- **恢复是可选的** -结束的DELETE只有在明确的用户确认后才运行；默认路径打印UI链接并停止。