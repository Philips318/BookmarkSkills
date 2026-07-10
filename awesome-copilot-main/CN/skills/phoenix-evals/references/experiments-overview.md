#实验：概述

用数据集、任务和评估器对人工智能系统进行系统测试。

# #结构```
DATASET     → Examples: {input, expected_output, metadata}
TASK        → function(input) → output
EVALUATORS  → (input, output, expected) → score
EXPERIMENT  → Run task on all examples, score results
```
##基本用法```python
from phoenix.client import Client

client = Client()
experiment = client.experiments.run_experiment(
    dataset=my_dataset,
    task=my_task,
    evaluators=[accuracy, faithfulness],
    experiment_name="improved-retrieval-v2",
)

print(experiment.aggregate_scores)
# {'accuracy': 0.85, 'faithfulness': 0.92}
```
# #工作流程

1. **创建数据集** -从痕迹，合成数据，或手动策展
2. **定义任务** -要测试的函数（您的LLM管道）
3. **选择评估器** -代码and/or基于llm
4. **运行实验** -执行并得分
5. **分析和迭代** -审查，修改任务，重新运行

##演习

完全执行前的测试设置：```python
experiment = client.experiments.run_experiment(
    dataset=dataset,
    task=task,
    evaluators=evaluators,
    dry_run=3,
)  # Just 3 examples
```
##异步使用

当您的任务或评估器进行网络调用并且需要更高的吞吐量时，请使用`AsyncClient`：```python
from phoenix.client import AsyncClient

client = AsyncClient()
experiment = await client.experiments.run_experiment(
    dataset=my_dataset,
    task=my_async_task,
    evaluators=[accuracy, faithfulness],
    experiment_name="improved-retrieval-v2",
)
```
最佳实践

- **有意义的名称**:`"improved-retrieval-v2-2024-01-15"`而不是`"test"`- **版本数据集**：不修改已有
- **多个评估者**：结合观点