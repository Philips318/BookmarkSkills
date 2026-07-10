#观察：跟踪设置

配置跟踪以捕获用于评估的数据。

##快速设置```python
# Python
from phoenix.otel import register

register(project_name="my-app", auto_instrument=True)
```

```typescript
// TypeScript
import { registerPhoenix } from "@arizeai/phoenix-otel";

registerPhoenix({ projectName: "my-app", autoInstrument: true });
```
基本属性

|属性|重要原因|| --------- | -------------- |
|`input.value`|用户请求|
|`output.value`|评估|的响应
|`retrieval.documents`|信实|的上下文
|`tool.name`，`tool.parameters`|代理评估|
|`llm.model_name`|按型号|跟踪

##自定义eval属性```python
span.set_attribute("metadata.client_type", "enterprise")
span.set_attribute("metadata.query_category", "billing")
```
##导出评估

### span （Python - DataFrame）```python
from phoenix.client import Client

# Client() works for local Phoenix (falls back to env vars or localhost:6006)
# For remote/cloud: Client(base_url="https://app.phoenix.arize.com", api_key="...")
client = Client()
spans_df = client.spans.get_spans_dataframe(
    project_identifier="my-app",  # NOT project_name= (deprecated)
    root_spans_only=True,
)

dataset = client.datasets.create_dataset(
    name="error-analysis-set",
    dataframe=spans_df[["input.value", "output.value"]],
    input_keys=["input.value"],
    output_keys=["output.value"],
)
```
### span （TypeScript）```typescript
import { getSpans } from "@arizeai/phoenix-client/spans";

const { spans } = await getSpans({
  project: { projectName: "my-app" },
  parentId: null, // root spans only
  limit: 100,
});
```
###跟踪（Python -结构化）

当你需要完整的跟踪树（例如，多回合对话，代理工作流）时，使用`get_traces`：```python
from datetime import datetime, timedelta

traces = client.traces.get_traces(
    project_identifier="my-app",
    start_time=datetime.now() - timedelta(hours=24),
    include_spans=True,  # includes all spans per trace
    limit=100,
)
# Each trace has: trace_id, start_time, end_time, spans (when include_spans=True)
```
### trace （TypeScript）```typescript
import { getTraces } from "@arizeai/phoenix-client/traces";

const { traces } = await getTraces({
  project: { projectName: "my-app" },
  startTime: new Date(Date.now() - 24 * 60 * 60 * 1000),
  includeSpans: true,
  limit: 100,
});
```
##将评估作为注释上传

# # # Python```python
from phoenix.evals import evaluate_dataframe
from phoenix.evals.utils import to_annotation_dataframe

# Run evaluations
results_df = evaluate_dataframe(dataframe=spans_df, evaluators=[my_eval])

# Format results for Phoenix annotations
annotations_df = to_annotation_dataframe(results_df)

# Upload to Phoenix
client.spans.log_span_annotations_dataframe(dataframe=annotations_df)
```
# # #打印稿```typescript
import { logSpanAnnotations } from "@arizeai/phoenix-client/spans";

await logSpanAnnotations({
  spanAnnotations: [
    {
      spanId: "abc123",
      name: "quality",
      label: "good",
      score: 0.95,
      annotatorKind: "LLM",
    },
  ],
});
```
在Phoenix UI中，注释与您的跟踪一起是可见的。

# #验证

必需的属性：`input.value`，`output.value`,`status_code`对于RAG:`retrieval.documents`代理商：`tool.name`、`tool.parameters`