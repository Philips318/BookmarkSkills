#模型选择

首先进行误差分析，最后进行模型更改。

##决策树```
Performance Issue?
       │
       ▼
Error analysis suggests model problem?
    NO  → Fix prompts, retrieval, tools
    YES → Is it a capability gap?
          YES → Consider model change
          NO  → Fix the actual problem
```
判断模型选择

|原理|动作|| --------- | ------ |
|启动能力|使用gpt- 40先|
|优化后|测试便宜后，标准稳定|
相同的模型OK |法官做不同的任务|```python
# Start with capable model
judge = ClassificationEvaluator(
    llm=LLM(provider="openai", model="gpt-4o"),
    ...
)

# After validation, test cheaper
judge_cheap = ClassificationEvaluator(
    llm=LLM(provider="openai", model="gpt-4o-mini"),
    ...
)
# Compare TPR/TNR on same test set
```
##不要模仿商店```python
from phoenix.client import Client

client = Client()

# BAD
for model in ["gpt-4o", "claude-3", "gemini-pro"]:
    results = client.experiments.run_experiment(
        dataset=dataset,
        task=lambda input, _model=model: task(input, model=_model),
        evaluators=evaluators,
    )

# GOOD
failures = analyze_errors(results)
# "Ignores context" → Fix prompt
# "Can't do math" → Maybe try better model
```
当需要更改模型时

—提示优化后，故障仍然存在
-能力差距（推理、数学、代码）
-误差分析证实了模型的局限性