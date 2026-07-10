#反模式

常见错误和修复。

|反模式|问题|修复|| ------------ | ------- | --- |
|通用指标|预构建分数与失败不匹配|从错误分析构建|
基于vibe的|无量化|用实验测量|
|忽略人类|未校准的LLM判断|验证>80%TPR/TNR|
过早自动化|假想问题的评估器|让观察到的故障驱动|
| 100%通过=无信号|保持能力在50-80% |
|相似性指标|BERTScore/ROUGE用于生成|仅用于检索|
|模型切换|希望模型工作得更好|错误分析先|
|当任务或判断是LLM调用|时，在`runExperiment`上设置`repetitions`（或增长数据集）

##量化变化```python
from phoenix.client import Client

client = Client()
baseline = client.experiments.run_experiment(dataset=dataset, task=old_prompt, evaluators=evaluators)
improved = client.experiments.run_experiment(dataset=dataset, task=new_prompt, evaluators=evaluators)
print(f"Improvement: {improved.pass_rate - baseline.pass_rate:+.1%}")
```
不要在生成时使用相似性```python
# BAD
score = bertscore(output, reference)

# GOOD
correct_facts = check_facts_against_source(output, context)
```
模型更改前的错误分析```python
# BAD
for model in models:
    results = test(model)

# GOOD
failures = analyze_errors(results)
# Then decide if model change is warranted
```
