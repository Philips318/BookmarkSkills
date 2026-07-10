#基础

针对AI系统的特定应用程序测试。代码至上，法学硕士追求细微差别，人类追求真理。

##评估器类型

|类型|速度|成本|用例|| ---- | ----- | ---- | -------- |
| **代码** |快速|廉价| Regex， JSON，格式，精确匹配|
| **LLM** |中等|中等|主观质量，复杂标准|
| **人类** |慢|昂贵|接地真相，校准|

**决策：**代码优先→LLM仅在代码无法捕获标准时→人工校准。

##评分结构

|属性|必选|描述|| -------- | -------- | ----------- |
|`name`|是|评估器名称|
|`kind`|是的|`"code"`，`"llm"`,`"human"`|
|`score`| No* | 0-1数字|
|`label`| No* |`"pass"`,`"fail"`|
|`explanation`|否|基本原理|

*需要`score`或`label`中的一个。

二进制>李克特

使用pass/fail，而不是1-5个刻度。更清晰的标准，更容易校准。```python
# Multiple binary checks instead of one Likert scale
evaluators = [
    AnswersQuestion(),    # Yes/No
    UsesContext(),        # Yes/No
    NoHallucination(),    # Yes/No
]
```
##快速模式

代码评估器```python
from phoenix.evals import create_evaluator

@create_evaluator(name="has_citation", kind="code")
def has_citation(output: str) -> bool:
    return bool(re.search(r'\[\d+\]', output))
```
LLM评估器```python
from phoenix.evals import ClassificationEvaluator, LLM

evaluator = ClassificationEvaluator(
    name="helpfulness",
    prompt_template="...",
    llm=LLM(provider="openai", model="gpt-4o"),
    choices={"not_helpful": 0, "helpful": 1}
)
```
###运行实验```python
from phoenix.client.experiments import run_experiment

experiment = run_experiment(
    dataset=dataset,
    task=my_task,
    evaluators=[evaluator1, evaluator2],
)
print(experiment.aggregate_scores)
```
