#错误分析

在构建评估器之前检查跟踪以发现故障模式。

# #过程

1. **采样** - 100+跟踪（误差，负反馈，随机）
2. **开放代码** -为每个跟踪编写自由格式的注释
3. **轴向代码** -将注释按故障类别分组
4. **量化** -统计每个类别的失败
5. **优先级** -按频率×严重程度排序

##采样痕迹

Span-level采样（Python - DataFrame）```python
from phoenix.client import Client

# Client() works for local Phoenix (falls back to env vars or localhost:6006)
# For remote/cloud: Client(base_url="https://app.phoenix.arize.com", api_key="...")
client = Client()
spans_df = client.spans.get_spans_dataframe(project_identifier="my-app")

# Build representative sample
sample = pd.concat([
    spans_df[spans_df["status_code"] == "ERROR"].sample(30),
    spans_df[spans_df["feedback"] == "negative"].sample(30),
    spans_df.sample(40),
]).drop_duplicates("span_id").head(100)
```
### Span-level采样（TypeScript）```typescript
import { getSpans } from "@arizeai/phoenix-client/spans";

const { spans: errors } = await getSpans({
  project: { projectName: "my-app" },
  statusCode: "ERROR",
  limit: 30,
});
const { spans: allSpans } = await getSpans({
  project: { projectName: "my-app" },
  limit: 70,
});
const sample = [...errors, ...allSpans.sort(() => Math.random() - 0.5).slice(0, 40)];
const unique = [...new Map(sample.map((s) => [s.context.span_id, s])).values()].slice(0, 100);
```
跟踪级采样（Python）

当错误跨越多个跨度（例如，代理工作流）时，采样整个跟踪：```python
from datetime import datetime, timedelta

traces = client.traces.get_traces(
    project_identifier="my-app",
    start_time=datetime.now() - timedelta(hours=24),
    include_spans=True,
    sort="latency_ms",
    order="desc",
    limit=100,
)
# Each trace has: trace_id, start_time, end_time, spans
```
跟踪级采样（TypeScript）```typescript
import { getTraces } from "@arizeai/phoenix-client/traces";

const { traces } = await getTraces({
  project: { projectName: "my-app" },
  startTime: new Date(Date.now() - 24 * 60 * 60 * 1000),
  includeSpans: true,
  limit: 100,
});
```
添加注释（Python）```python
client.spans.add_span_note(
    span_id="abc123",
    note="wrong timezone - said 3pm EST but user is PST"
)
```
##添加注释（TypeScript）```typescript
import { addSpanNote } from "@arizeai/phoenix-client/spans";

await addSpanNote({
  spanNote: {
    spanId: "abc123",
    note: "wrong timezone - said 3pm EST but user is PST"
  }
});
```
##注意事项

|类型|样例|| ---- | -------- |
事实错误日期、价格、编造的特征错误
|缺少信息|没有回答问题，省略细节|
| Tone issue | Toocasual/formalfor context |
|工具发出|错误的工具，错误的参数|
|检索|文档错误，缺少相关文档|

##好的笔记```
BAD:  "Response is bad"
GOOD: "Response says ships in 2 days but policy is 5-7 days"
```
分组为类别```python
categories = {
    "factual_inaccuracy": ["wrong shipping time", "incorrect price"],
    "hallucination": ["made up a discount", "invented feature"],
    "tone_mismatch": ["informal for enterprise client"],
}
# Priority = Frequency × Severity
```
##检索现有注解

# # # Python```python
# From a spans DataFrame
annotations_df = client.spans.get_span_annotations_dataframe(
    spans_dataframe=sample,
    project_identifier="my-app",
    include_annotation_names=["quality", "correctness"],
)
# annotations_df has: span_id (index), name, label, score, explanation

# Or from specific span IDs
annotations_df = client.spans.get_span_annotations_dataframe(
    span_ids=["span-id-1", "span-id-2"],
    project_identifier="my-app",
)
```
# # #打印稿```typescript
import { getSpanAnnotations } from "@arizeai/phoenix-client/spans";

const { annotations } = await getSpanAnnotations({
  project: { projectName: "my-app" },
  spanIds: ["span-id-1", "span-id-2"],
  includeAnnotationNames: ["quality", "correctness"],
});

for (const ann of annotations) {
  console.log(`${ann.span_id}: ${ann.name} = ${ann.result?.label} (${ann.result?.score})`);
}
```
# #饱和

当新的跟踪显示没有新的故障模式时停止。最小：100道。