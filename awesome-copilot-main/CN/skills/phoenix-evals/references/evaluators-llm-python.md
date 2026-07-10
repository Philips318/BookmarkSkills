# Evaluators: Python中的LLM评估器

LLM评估人员使用语言模型来判断输出。当标准是主观的时候使用。

##快速入门```python
from phoenix.evals import ClassificationEvaluator, LLM

llm = LLM(provider="openai", model="gpt-4o")

HELPFULNESS_TEMPLATE = """Rate how helpful the response is.

<question>{{input}}</question>
<response>{{output}}</response>

"helpful" means directly addresses the question.
"not_helpful" means does not address the question.

Your answer (helpful/not_helpful):"""

helpfulness = ClassificationEvaluator(
    name="helpfulness",
    prompt_template=HELPFULNESS_TEMPLATE,
    llm=llm,
    choices={"not_helpful": 0, "helpful": 1}
)
```
##模板变量

为了清晰起见，使用XML标签来包装变量：

|变量| XML标签|| -------- | ------- |
|`{{input}}`|`<question>{{input}}</question>`|
|`{{output}}`|`<response>{{output}}</response>`|
|`{{reference}}`|`<reference>{{reference}}</reference>`|
|`{{context}}`|`<context>{{context}}</context>`|

## create_classifier （Factory）

返回一个`ClassificationEvaluator`的速记工厂。更喜欢直接`ClassificationEvaluator`实例化更多parameters/customization：```python
from phoenix.evals import create_classifier, LLM

relevance = create_classifier(
    name="relevance",
    prompt_template="""Is this response relevant to the question?
<question>{{input}}</question>
<response>{{output}}</response>
Answer (relevant/irrelevant):""",
    llm=LLM(provider="openai", model="gpt-4o"),
    choices={"relevant": 1.0, "irrelevant": 0.0},
)
```
##输入映射

列名必须与模板变量匹配。重命名列或使用`bind_evaluator`：```python
# Option 1: Rename columns to match template variables
df = df.rename(columns={"user_query": "input", "ai_response": "output"})

# Option 2: Use bind_evaluator
from phoenix.evals import bind_evaluator

bound = bind_evaluator(
    evaluator=helpfulness,
    input_mapping={"input": "user_query", "output": "ai_response"},
)
```
# #运行```python
from phoenix.evals import evaluate_dataframe

results_df = evaluate_dataframe(dataframe=df, evaluators=[helpfulness])
```
最佳实践

1. **具体** -准确定义pass/fail的含义
2. **包括示例** -显示每个标签的具体案例
3. **默认的解释** -`ClassificationEvaluator`自动包含解释
4. **学习内置提示** -参见
例如`phoenix.evals.__generated__.classification_evaluator_configs`结构良好的评估提示（可靠性、正确性、文档相关性等）