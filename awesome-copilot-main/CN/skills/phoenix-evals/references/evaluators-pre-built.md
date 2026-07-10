#评估器：预构建

仅用于探索。在生产前进行验证。

# # Python```python
from phoenix.evals import LLM
from phoenix.evals.metrics import FaithfulnessEvaluator

llm = LLM(provider="openai", model="gpt-4o")
faithfulness_eval = FaithfulnessEvaluator(llm=llm)
```
**注**:`HallucinationEvaluator`已弃用。请使用`FaithfulnessEvaluator`。
它使用得分为1.0 =忠实的“忠实”/“不忠实”标签。

# #打印稿```typescript
import { createHallucinationEvaluator } from "@arizeai/phoenix-evals";
import { openai } from "@ai-sdk/openai";

const hallucinationEval = createHallucinationEvaluator({ model: openai("gpt-4o") });
```
##可用（2.0版）

|评估器|类型|描述|| --------- | ---- | ----------- |
|`FaithfulnessEvaluator`| LLM |响应忠实于上下文吗？|
|`CorrectnessEvaluator`| LLM |响应是否正确？|
|`DocumentRelevanceEvaluator`| LLM |检索的文档是否相关？|
|`ToolSelectionEvaluator`| LLM |代理是否选择了正确的工具？|
|`ToolInvocationEvaluator`| LLM |代理是否正确调用工具？|
代理处理工具响应是否良好？|
|`MatchesRegex`|代码|输出是否匹配正则表达式模式？|
|`PrecisionRecallFScore`|代码|Precision/recall/F-score指标|
|`exact_match`|代码|精确字符串匹配|

遗留的评估器(`HallucinationEvaluator`,`QAEvaluator`,`RelevanceEvaluator`，`ToxicityEvaluator`,`SummarizationEvaluator`)在`phoenix.evals.legacy`中已弃用。

##何时使用

|情况|建议|| --------- | -------------- |
|勘探|查找踪迹查看|
|查找异常值|按分数排序|
|生产|先验证（>80%人类协议）|
|特定于域的|构建自定义|

##探索模式```python
from phoenix.evals import evaluate_dataframe

results_df = evaluate_dataframe(dataframe=traces, evaluators=[faithfulness_eval])

# Score columns contain dicts — extract numeric scores
scores = results_df["faithfulness_score"].apply(
    lambda x: x.get("score", 0.0) if isinstance(x, dict) else 0.0
)
low_scores = results_df[scores < 0.5]   # Review these
high_scores = results_df[scores > 0.9]  # Also sample
```
##需要验证```python
from sklearn.metrics import classification_report

print(classification_report(human_labels, evaluator_results["label"]))
# Target: >80% agreement
```
