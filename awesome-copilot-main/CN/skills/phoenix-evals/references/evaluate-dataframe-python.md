#使用evaluate_dataframe进行批计算（Python）

跨DataFrame运行求值器。core 2.0批量评估API。

##首选：async_evaluate_dataframe

对于批量评估（特别是使用LLM评估器），建议使用异步版本
为了获得更好的吞吐量：```python
from phoenix.evals import async_evaluate_dataframe

results_df = await async_evaluate_dataframe(
    dataframe=df,              # pandas DataFrame with columns matching evaluator params
    evaluators=[eval1, eval2], # List of evaluators
    concurrency=5,             # Max concurrent LLM calls (default 3)
    exit_on_error=False,       # Optional: stop on first error (default True)
    max_retries=3,             # Optional: retry failed LLM calls (default 10)
)
```
##同步版本```python
from phoenix.evals import evaluate_dataframe

results_df = evaluate_dataframe(
    dataframe=df,              # pandas DataFrame with columns matching evaluator params
    evaluators=[eval1, eval2], # List of evaluators
    exit_on_error=False,       # Optional: stop on first error (default True)
    max_retries=3,             # Optional: retry failed LLM calls (default 10)
)
```
## Result列格式`async_evaluate_dataframe`/`evaluate_dataframe`返回带有添加列的输入DataFrame的副本。
**结果列包含字典，而不是原始数字

对于每个名为`"foo"`的求值器，添加两列：

|字段|类型|内容|| ------ | ---- | -------- |
|`foo_score`|`dict`|`{"name": "foo", "score": 1.0, "label": "True", "explanation": "...", "metadata": {...}, "kind": "code", "direction": "maximize"}`|
|`foo_execution_details`|`dict`|`{"status": "success", "exceptions": [], "execution_seconds": 0.001}`|

分数字典中只出现非none字段。

提取数字分数```python
# WRONG — these will fail or produce unexpected results
score = results_df["relevance"].mean()                    # KeyError!
score = results_df["relevance_score"].mean()              # Tries to average dicts!

# RIGHT — extract the numeric score from each dict
scores = results_df["relevance_score"].apply(
    lambda x: x.get("score", 0.0) if isinstance(x, dict) else 0.0
)
mean_score = scores.mean()
```
提取标签```python
labels = results_df["relevance_score"].apply(
    lambda x: x.get("label", "") if isinstance(x, dict) else ""
)
```
提取解释（LLM评估者）```python
explanations = results_df["relevance_score"].apply(
    lambda x: x.get("explanation", "") if isinstance(x, dict) else ""
)
```
###发现失败```python
scores = results_df["relevance_score"].apply(
    lambda x: x.get("score", 0.0) if isinstance(x, dict) else 0.0
)
failed_mask = scores < 0.5
failures = results_df[failed_mask]
```
##输入映射

求值器接收每一行作为字典。列名必须与求值器的列名匹配
期望的参数名称。如果它们不匹配，使用`.bind()`或`bind_evaluator`：```python
from phoenix.evals import bind_evaluator, create_evaluator, async_evaluate_dataframe

@create_evaluator(name="check", kind="code")
def check(response: str) -> bool:
    return len(response.strip()) > 0

# Option 1: Use .bind() method on the evaluator
check.bind(input_mapping={"response": "answer"})
results_df = await async_evaluate_dataframe(dataframe=df, evaluators=[check])

# Option 2: Use bind_evaluator function
bound = bind_evaluator(evaluator=check, input_mapping={"response": "answer"})
results_df = await async_evaluate_dataframe(dataframe=df, evaluators=[bound])
```
或者简单地重命名列来匹配：```python
df = df.rename(columns={
    "attributes.input.value": "input",
    "attributes.output.value": "output",
})
```
##不要使用run_eval```python
# WRONG — legacy 1.0 API
from phoenix.evals import run_evals
results = run_evals(dataframe=df, evaluators=[eval1])
# Returns List[DataFrame] — one per evaluator

# RIGHT — current 2.0 API
from phoenix.evals import async_evaluate_dataframe
results_df = await async_evaluate_dataframe(dataframe=df, evaluators=[eval1])
# Returns single DataFrame with {name}_score dict columns
```
关键的不同点:`run_evals`返回一个dataframe列表（每个求值器一个）
-`async_evaluate_dataframe`返回一个合并了所有结果的**单个** DataFrame
—`async_evaluate_dataframe`使用`{name}_score`字典列格式`async_evaluate_dataframe`使用`bind_evaluator`作为输入映射（不是`input_mapping=`参数）