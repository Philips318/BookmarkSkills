#观察：抽样策略

如何有效地对生产痕迹进行抽样审查。

# #策略

# # # 1。以故障为中心（最高优先级）```python
errors = spans_df[spans_df["status_code"] == "ERROR"]
negative_feedback = spans_df[spans_df["feedback"] == "negative"]
```
# # # 2。离群值```python
long_responses = spans_df.nlargest(50, "response_length")
slow_responses = spans_df.nlargest(50, "latency_ms")
```
# # # 3。分层(覆盖率)```python
# Sample equally from each category
by_query_type = spans_df.groupby("metadata.query_type").apply(
    lambda x: x.sample(min(len(x), 20))
)
```
# # # 4。Metric-Guided```python
# Review traces flagged by automated evaluators
flagged = spans_df[eval_results["label"] == "hallucinated"]
borderline = spans_df[(eval_results["score"] > 0.3) & (eval_results["score"] < 0.7)]
```
##创建评审队列```python
def build_review_queue(spans_df, max_traces=100):
    queue = pd.concat([
        spans_df[spans_df["status_code"] == "ERROR"],
        spans_df[spans_df["feedback"] == "negative"],
        spans_df.nlargest(10, "response_length"),
        spans_df.sample(min(30, len(spans_df))),
    ]).drop_duplicates("span_id").head(max_traces)
    return queue
```
##样本大小指南

|用途|大小|| ------- | ---- |
初步勘探| 50-100 |
误差分析| 100+（直到饱和）|
|黄金数据集| 100-500 |
|判断校准| 100+每类|

**饱和：**停止时，新的痕迹显示相同的失败模式。

跟踪级采样

当您需要整个请求（每个跟踪的所有跨度）时，使用`get_traces`：```python
from phoenix.client import Client
from datetime import datetime, timedelta

client = Client()

# Recent traces with full span trees
traces = client.traces.get_traces(
    project_identifier="my-app",
    limit=100,
    include_spans=True,
)

# Time-windowed sampling (e.g., last hour)
traces = client.traces.get_traces(
    project_identifier="my-app",
    start_time=datetime.now() - timedelta(hours=1),
    limit=50,
    include_spans=True,
)

# Filter by session (multi-turn conversations)
traces = client.traces.get_traces(
    project_identifier="my-app",
    session_id="user-session-abc",
    include_spans=True,
)

# Sort by latency to find slowest requests
traces = client.traces.get_traces(
    project_identifier="my-app",
    sort="latency_ms",
    order="desc",
    limit=50,
)
```
