# Experiments：在Python中运行实验

使用`run_experiment`执行实验。

##基本用法```python
from phoenix.client import Client
from phoenix.client.experiments import run_experiment

client = Client()
dataset = client.datasets.get_dataset(name="qa-test-v1")

def my_task(example):
    return call_llm(example.input["question"])

def exact_match(output, expected):
    return 1.0 if output.strip().lower() == expected["answer"].strip().lower() else 0.0

experiment = run_experiment(
    dataset=dataset,
    task=my_task,
    evaluators=[exact_match],
    experiment_name="qa-experiment-v1",
)
```
##任务函数```python
# Basic task
def task(example):
    return call_llm(example.input["question"])

# With context (RAG)
def rag_task(example):
    return call_llm(f"Context: {example.input['context']}\nQ: {example.input['question']}")
```
##评估器参数

|参数|接入|| --------- | ------ |
|`output`|任务输出|
|`expected`|示例期望输出|
|`input`|示例输入|
|`metadata`|元数据示例|

# #选项```python
experiment = run_experiment(
    dataset=dataset,
    task=my_task,
    evaluators=evaluators,
    experiment_name="my-experiment",
    dry_run=3,       # Test with 3 examples
    repetitions=3,   # Run each example 3 times
)
```
# #结果```python
print(experiment.aggregate_scores)
# {'accuracy': 0.85, 'faithfulness': 0.92}

for run in experiment.runs:
    print(run.output, run.scores)
```
# #稳定

当任务或评估器是不确定的（LLM调用、工具使用、流输出、LLM作为裁判）时，单次运行的分数是嘈杂的。在一个小数据集上，每次运行的噪声可能会淹没提示变化的信号。

重复的平均可以让你报告的分数反映提示而不是采样噪声：```python
run_experiment(
    # ...
    repetitions=3,
)
```
需要考虑的事项：

-当任务或评估器是LLM调用并且数据集很小时，需要进行重复。
-当每个样本的成本较低并且你主要想要结算分数时更喜欢重复当您还需要覆盖更多行为时，更喜欢增长数据集。
-当任务和评估器都是确定的时跳过重复（例如，与基础真理进行字符串比较）-一次运行就是答案。

考虑在以下情况下增加稳定性：

-重复运行相同的实验漂移的方式感觉比你试图测量的差异更大。
-提示式更改以不跟踪输出实际变化的方式翻转示例标签。
-法官对同一输出的推理从一次运行到下一次运行的读数不同。

重复也是`repetitions=1`（默认）默默地依赖的—不要相信基于单个10个示例运行的调优决策。##稍后添加评估```python
from phoenix.client.experiments import evaluate_experiment

evaluate_experiment(experiment=experiment, evaluators=[new_evaluator])
```
