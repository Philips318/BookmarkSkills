#生产：概述CI/CD评价与生产监控——互补的方法。

两种评估模式

|方面|CI/CD评估|生产监控|| ------ | ----------- | -------------------- |
| **当** |部署前|部署后，正在进行|
| **数据** |固定数据集|采样流量|
| **目标** |防止回归|检测漂移|
| **响应** |块部署|警报和分析|

##CI/CD评估```python
from phoenix.client import Client

client = Client()

# Fast, deterministic checks
ci_evaluators = [
    has_required_format,
    no_pii_leak,
    safety_check,
    regression_test_suite,
]

# Small but representative dataset (~100 examples)
client.experiments.run_experiment(dataset=ci_dataset, task=task, evaluators=ci_evaluators)
```
设定阈值：回归=0.95，安全性=1.0，格式=0.98。

##生产监控

# # # Python```python
from phoenix.client import Client
from datetime import datetime, timedelta

client = Client()

# Sample recent traces (last hour)
traces = client.traces.get_traces(
    project_identifier="my-app",
    start_time=datetime.now() - timedelta(hours=1),
    include_spans=True,
    limit=100,
)

# Run evaluators on sampled traffic
for trace in traces:
    results = run_evaluators_async(trace, production_evaluators)
    if any(r["score"] < 0.5 for r in results):
        alert_on_failure(trace, results)
```
# # #打印稿```typescript
import { getTraces } from "@arizeai/phoenix-client/traces";
import { getSpans } from "@arizeai/phoenix-client/spans";

// Sample recent traces (last hour)
const { traces } = await getTraces({
  project: { projectName: "my-app" },
  startTime: new Date(Date.now() - 60 * 60 * 1000),
  includeSpans: true,
  limit: 100,
});

// Or sample spans directly for evaluation
const { spans } = await getSpans({
  project: { projectName: "my-app" },
  startTime: new Date(Date.now() - 60 * 60 * 1000),
  limit: 100,
});

// Run evaluators on sampled traffic
for (const span of spans) {
  const results = await runEvaluators(span, productionEvaluators);
  if (results.some((r) => r.score < 0.5)) {
    await alertOnFailure(span, results);
  }
}
```
优先级：误差→负反馈→随机抽样。

##反馈循环```
Production finds failure → Error analysis → Add to CI dataset → Prevents future regression
```
