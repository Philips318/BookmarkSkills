#生产：持续评估

能力vs回归评估和持续的反馈循环。

两种类型的评估

|类型|通过率目标|目的|更新|| ---- | ---------------- | ------- | ------ |
| **能力** | 50-80% |改进措施|增加更难的情况|
| **回归** | 95-100% |捕获破损|添加修复错误|

# #饱和

当能力评估达到95%通过率时，它们就饱和了。
1. 将传递的案例转移到回归套件
2. 向功能套件添加新的具有挑战性的用例

##反馈循环```
Production → Sample traffic → Run evaluators → Find failures
    ↑                                              ↓
Deploy  ←  Run CI evals  ←  Create test cases  ←  Error analysis
```
# #实现

建立一个持续的监控循环：

1. **定期采样最近的痕迹**（例如，每小时100条痕迹）
2. **运行评估器**采样痕迹
3. **记录结果**到Phoenix进行跟踪
4. **关于结果的队列**供人工审核
5. **根据重复出现的故障模式创建测试用例**

# # # Python```python
from phoenix.client import Client
from datetime import datetime, timedelta

client = Client()

# 1. Sample recent spans (includes full attributes for evaluation)
spans_df = client.spans.get_spans_dataframe(
    project_identifier="my-app",
    start_time=datetime.now() - timedelta(hours=1),
    root_spans_only=True,
    limit=100,
)

# 2. Run evaluators
from phoenix.evals import evaluate_dataframe

results_df = evaluate_dataframe(
    dataframe=spans_df,
    evaluators=[quality_eval, safety_eval],
)

# 3. Upload results as annotations
from phoenix.evals.utils import to_annotation_dataframe

annotations_df = to_annotation_dataframe(results_df)
client.spans.log_span_annotations_dataframe(dataframe=annotations_df)
```
# # #打印稿```typescript
import { getSpans } from "@arizeai/phoenix-client/spans";
import { logSpanAnnotations } from "@arizeai/phoenix-client/spans";

// 1. Sample recent spans
const { spans } = await getSpans({
  project: { projectName: "my-app" },
  startTime: new Date(Date.now() - 60 * 60 * 1000),
  parentId: null, // root spans only
  limit: 100,
});

// 2. Run evaluators (user-defined)
const results = await Promise.all(
  spans.map(async (span) => ({
    spanId: span.context.span_id,
    ...await runEvaluators(span, [qualityEval, safetyEval]),
  }))
);

// 3. Upload results as annotations
await logSpanAnnotations({
  spanAnnotations: results.map((r) => ({
    spanId: r.spanId,
    name: "quality",
    score: r.qualityScore,
    label: r.qualityLabel,
    annotatorKind: "LLM" as const,
  })),
});
```
对于跟踪级监控（例如，代理工作流），使用`get_traces`/`getTraces`来识别跟踪：```python
# Python: identify slow traces
traces = client.traces.get_traces(
    project_identifier="my-app",
    start_time=datetime.now() - timedelta(hours=1),
    sort="latency_ms",
    order="desc",
    limit=50,
)
```

```typescript
// TypeScript: identify slow traces
import { getTraces } from "@arizeai/phoenix-client/traces";

const { traces } = await getTraces({
  project: { projectName: "my-app" },
  startTime: new Date(Date.now() - 60 * 60 * 1000),
  limit: 50,
});
```
# #报警

|条件|严重性|动作|| --------- | -------- | ------ |
|回归< 98% |临界|页面oncall |
|容量下降|警告| Slack通知|
|能力> 95%为7d |信息|进度审查|

##关键原则

- **两个套件** -能力+回归始终
- **毕业案例** -将一致的传球移至回归
-跟踪趋势** -监控随着时间的推移，而不仅仅是快照