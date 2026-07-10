# Experiments: Python中的数据集

创建和管理评估数据集。

##创建数据集`create_dataset()`upserts：如果同名数据集已经存在，则就地更新；使用相同的输入重新运行是无操作的。```python
from phoenix.client import Client

client = Client()

# From examples
dataset = client.datasets.create_dataset(
    name="qa-test-v1",
    examples=[
        {
            "input": {"question": "What is 2+2?"},
            "output": {"answer": "4"},
            "metadata": {"category": "math"},
        },
    ],
)

# With stable example IDs for targeted updates across uploads
dataset = client.datasets.create_dataset(
    name="qa-test-v1",
    examples=[
        {
            "id": "q-001",                      # stable ID — server updates this row, not inserts
            "input": {"question": "What is 2+2?"},
            "output": {"answer": "4"},
            "metadata": {"category": "math"},
        },
    ],
)

# From DataFrame
dataset = client.datasets.create_dataset(
    dataframe=df,
    name="qa-test-v1",
    input_keys=["question"],
    output_keys=["answer"],
    metadata_keys=["category"],
    split_key="split",        # single split column (use this instead of deprecated split_keys)
    example_id_key="id",      # column containing stable example IDs
)
```
##从生产轨迹```python
spans_df = client.spans.get_spans_dataframe(project_identifier="my-app")

dataset = client.datasets.create_dataset(
    dataframe=spans_df[["input.value", "output.value"]],
    name="production-sample-v1",
    input_keys=["input.value"],
    output_keys=["output.value"],
)
```
##检索数据集```python
dataset = client.datasets.get_dataset(name="qa-test-v1")
df = dataset.to_dataframe()
```
##关键参数

| |参数说明|| --------- | ----------- |
|`input_keys`|任务输入|的列
|`output_keys`|预期输出|的列
|`metadata_keys`|附加上下文|
|`example_id_key`|具有稳定样例id的列；服务器更新匹配行，而不是插入|
|`split_key`|拆分分配的单列（取代已弃用的`split_keys`） |
|`split_keys`| **已弃用** -使用`split_key`（单数）代替|

##在实验中使用评估器

评价者作为实验评价者

将phoenix- evalals求值器作为`evaluators`参数直接传递给`run_experiment`：```python
from functools import partial
from phoenix.client import AsyncClient
from phoenix.evals import ClassificationEvaluator, LLM, bind_evaluator

# Define an LLM evaluator
refusal = ClassificationEvaluator(
    name="refusal",
    prompt_template="Is this a refusal?\nQuestion: {{query}}\nResponse: {{response}}",
    llm=LLM(provider="openai", model="gpt-4o"),
    choices={"refusal": 0, "answer": 1},
)

# Bind to map dataset columns to evaluator params
refusal_evaluator = bind_evaluator(refusal, {"query": "input.query", "response": "output"})

# Define experiment task
async def run_rag_task(input, rag_engine):
    return rag_engine.query(input["query"])

# Run experiment with the evaluator
experiment = await AsyncClient().experiments.run_experiment(
    dataset=ds,
    task=partial(run_rag_task, rag_engine=query_engine),
    experiment_name="baseline",
    evaluators=[refusal_evaluator],
    concurrency=10,
)
```
评估器作为任务（元评估）

使用LLM评估器作为实验任务来测试评估器本身
反对人工注释：```python
from phoenix.evals import create_evaluator

# The evaluator IS the task being tested
def run_refusal_eval(input, evaluator):
    result = evaluator.evaluate(input)
    return result[0]

# A simple heuristic checks judge vs human agreement
@create_evaluator(name="exact_match")
def exact_match(output, expected):
    return float(output["score"]) == float(expected["refusal_score"])

# Run: evaluator is the task, exact_match evaluates it
experiment = await AsyncClient().experiments.run_experiment(
    dataset=annotated_dataset,
    task=partial(run_refusal_eval, evaluator=refusal),
    experiment_name="judge-v1",
    evaluators=[exact_match],
    concurrency=10,
)
```
此模式允许您迭代评估器提示，直到它们与人类判断一致。
请参阅`tutorials/evals/evals-2/evals_2.0_rag_demo.ipynb`了解完整的工作示例。

最佳实践

- **默认为Upsert **：重新上传到相同的名称以更新原位；使用`example_id_key`，以便服务器针对特定的行，而不是将每次上传都视为新数据
- **版本控制**：当你想要一个干净的快照时，带有标签或新名称的版本（例如，`qa-test-v2`），而不仅仅是增量编辑
- **元数据**：跟踪来源，类别，难度
- **平衡**：确保不同类别的覆盖范围
- **避免`split_keys`**：传递`split_key`（单数）-`split_keys`已弃用，并发出`DeprecationWarning`