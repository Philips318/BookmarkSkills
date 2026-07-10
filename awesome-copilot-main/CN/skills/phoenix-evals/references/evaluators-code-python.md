# Evaluators: Python中的代码评估器

没有LLM的确定性评估器。快速、廉价、可复制。

##基本模式```python
import re
import json
from phoenix.evals import create_evaluator

@create_evaluator(name="has_citation", kind="code")
def has_citation(output: str) -> bool:
    return bool(re.search(r'\[\d+\]', output))

@create_evaluator(name="json_valid", kind="code")
def json_valid(output: str) -> bool:
    try:
        json.loads(output)
        return True
    except json.JSONDecodeError:
        return False
```
##参数绑定

| |参数说明|| --------- | ----------- |
|`output`|任务输出|
|`input`|示例输入|
|`expected`|预期输出|
|`metadata`|元数据示例|```python
@create_evaluator(name="matches_expected", kind="code")
def matches_expected(output: str, expected: dict) -> bool:
    return output.strip() == expected.get("answer", "").strip()
```
##常见模式

—**正则表达式**:`re.search(pattern, output)`- **JSON模式**:`jsonschema.validate()`- **关键词**:`keyword in output.lower()`- **长度**:`len(output.split())`- **相似性**:`editdistance.eval()`或Jaccard

##返回类型

|返回类型|结果|| ----------- | ------ |
|`bool`|`True`→score=1.0, label="True"；`False`→score=0.0, label="False" |
|`float`/`int`|直接用作`score`值|
|`str`（短，≤3个字）|用作`label`值|
|`str`（长，≥4个字）|用作`explanation`值|
|`dict`与`score`/`label`/`explanation`|直接映射到得分字段|
|`Score`对象|按原样使用

重要：代码vs法学硕士评估器`@create_evaluator`装饰器包装了一个普通的Python函数。

-`kind="code"`（默认）：用于不调用LLM的确定性求值器。
-`kind="llm"`：将评估器标记为基于LLM，但**你**必须实现LLM
在函数内部调用。装饰器不会为您调用LLM。

对于大多数基于llm的评估，更喜欢`ClassificationEvaluator`处理
LLM调用，结构化输出解析，并自动解释：```python
from phoenix.evals import ClassificationEvaluator, LLM

relevance = ClassificationEvaluator(
    name="relevance",
    prompt_template="Is this relevant?\n{{input}}\n{{output}}\nAnswer:",
    llm=LLM(provider="openai", model="gpt-4o"),
    choices={"relevant": 1.0, "irrelevant": 0.0},
)
```
# #预构建```python
from phoenix.client.experiments import create_evaluator
from phoenix.evals.metrics import MatchesRegex

date_format = MatchesRegex(pattern=r"\d{4}-\d{2}-\d{2}")


@create_evaluator(name="contains_any_keyword", kind="code")
def contains_any_keyword(output, expected):
    keywords = expected.get("keywords", [])
    return any(kw.lower() in str(output).lower() for kw in keywords)


@create_evaluator(name="json_parseable", kind="code")
def json_parseable(output):
    import json

    try:
        json.loads(output)
        return True
    except (json.JSONDecodeError, TypeError):
        return False
```
