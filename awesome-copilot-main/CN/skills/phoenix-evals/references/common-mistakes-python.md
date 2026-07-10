#常见错误（Python）

llm经常从训练数据中错误地生成模式。

遗留模型类```python
# WRONG
from phoenix.evals import OpenAIModel, AnthropicModel
model = OpenAIModel(model="gpt-4")

# RIGHT
from phoenix.evals import LLM
llm = LLM(provider="openai", model="gpt-4o")
```
**为什么**:`OpenAIModel`，`AnthropicModel`等是`phoenix.evals.legacy`中的传统1.0包装器。`LLM`类与提供者无关，是当前的2.0 API。

##使用run_evals代替evaluate_dataframe```python
# WRONG — legacy 1.0 API
from phoenix.evals import run_evals
results = run_evals(dataframe=df, evaluators=[eval1], provide_explanation=True)
# Returns list of DataFrames

# RIGHT — current 2.0 API
from phoenix.evals import evaluate_dataframe
results_df = evaluate_dataframe(dataframe=df, evaluators=[eval1])
# Returns single DataFrame with {name}_score dict columns
```
**为什么**:`run_evals`是旧的1.0批处理函数。`evaluate_dataframe`是电流
函数使用不同的返回格式。

错误的结果列名```python
# WRONG — column doesn't exist
score = results_df["relevance"].mean()

# WRONG — column exists but contains dicts, not numbers
score = results_df["relevance_score"].mean()

# RIGHT — extract numeric score from dict
scores = results_df["relevance_score"].apply(
    lambda x: x.get("score", 0.0) if isinstance(x, dict) else 0.0
)
score = scores.mean()
```
**为什么**:`evaluate_dataframe`返回名为`{name}_score`的列，其中包含Score字典
像`{"name": "...", "score": 1.0, "label": "...", "explanation": "..."}`。

##已弃用project_name参数```python
# WRONG
df = client.spans.get_spans_dataframe(project_name="my-project")

# RIGHT
df = client.spans.get_spans_dataframe(project_identifier="my-project")
```
**为什么**：不支持`project_name`，而支持`project_identifier`接受项目id。

错误的客户端构造函数```python
# WRONG
client = Client(endpoint="https://app.phoenix.arize.com")
client = Client(url="https://app.phoenix.arize.com")

# RIGHT — for remote/cloud Phoenix
client = Client(base_url="https://app.phoenix.arize.com", api_key="...")

# ALSO RIGHT — for local Phoenix (falls back to env vars or localhost:6006)
client = Client()
```
**原因**：参数为`base_url`，而不是`endpoint`或`url`。对于本地实例，
不带参数的`Client()`工作得很好。对于远程实例，需要`base_url`和`api_key`。

过于激进的时间过滤器```python
# WRONG — often returns zero spans
from datetime import datetime, timedelta
df = client.spans.get_spans_dataframe(
    project_identifier="my-project",
    start_time=datetime.now() - timedelta(hours=1),
)

# RIGHT — use limit to control result size instead
df = client.spans.get_spans_dataframe(
    project_identifier="my-project",
    limit=50,
)
```
**原因**：痕迹可能来自任何时间段。一个1小时的窗口经常返回
什么都没有。使用`limit=`来控制结果大小。

##没有适当地过滤```python
# WRONG — fetches all spans including internal LLM calls, retrievers, etc.
df = client.spans.get_spans_dataframe(project_identifier="my-project")

# RIGHT for end-to-end evaluation — filter to top-level spans
df = client.spans.get_spans_dataframe(
    project_identifier="my-project",
    root_spans_only=True,
)

# RIGHT for RAG evaluation — fetch child spans for retriever/LLM metrics
all_spans = client.spans.get_spans_dataframe(
    project_identifier="my-project",
)
retriever_spans = all_spans[all_spans["span_kind"] == "RETRIEVER"]
llm_spans = all_spans[all_spans["span_kind"] == "LLM"]
```
**为什么**：对于端到端评估（例如，整体回答质量），使用`root_spans_only=True`。
对于RAG系统，您通常需要单独的子跨度-检索跨度
文档相关性和法学硕士涵盖了忠诚。选择正确的跨度级别
为您的评估目标。

假设Span输出是纯文本```python
# WRONG — output may be JSON, not plain text
df["output"] = df["attributes.output.value"]

# RIGHT — parse JSON and extract the answer field
import json

def extract_answer(output_value):
    if not isinstance(output_value, str):
        return str(output_value) if output_value is not None else ""
    try:
        parsed = json.loads(output_value)
        if isinstance(parsed, dict):
            for key in ("answer", "result", "output", "response"):
                if key in parsed:
                    return str(parsed[key])
    except (json.JSONDecodeError, TypeError):
        pass
    return output_value

df["output"] = df["attributes.output.value"].apply(extract_answer)
```
**为什么**:LangChain和其他框架经常从根跨输出结构化JSON，
像`{"context": "...", "question": "...", "answer": "..."}`。评估者需要
实际的答案文本，而不是原始的JSON。

##使用@create_evaluator进行基于llm的评估```python
# WRONG — @create_evaluator doesn't call an LLM
@create_evaluator(name="relevance", kind="llm")
def relevance(input: str, output: str) -> str:
    pass  # No LLM is involved

# RIGHT — use ClassificationEvaluator for LLM-based evaluation
from phoenix.evals import ClassificationEvaluator, LLM

relevance = ClassificationEvaluator(
    name="relevance",
    prompt_template="Is this relevant?\n{{input}}\n{{output}}\nAnswer:",
    llm=LLM(provider="openai", model="gpt-4o"),
    choices={"relevant": 1.0, "irrelevant": 0.0},
)
```
**为什么**:`@create_evaluator`包装了一个普通的Python函数。设置`kind="llm"`将其标记为基于LLM，但您必须自己实现LLM调用。
对于基于llm的评估，建议使用`ClassificationEvaluator`LLM调用，结构化输出解析，并自动解释。

##使用llm_classifier代替ClassificationEvaluator```python
# WRONG — legacy 1.0 API
from phoenix.evals import llm_classify
results = llm_classify(
    dataframe=df,
    template=template_str,
    model=model,
    rails=["relevant", "irrelevant"],
)

# RIGHT — current 2.0 API
from phoenix.evals import ClassificationEvaluator, async_evaluate_dataframe, LLM

classifier = ClassificationEvaluator(
    name="relevance",
    prompt_template=template_str,
    llm=LLM(provider="openai", model="gpt-4o"),
    choices={"relevant": 1.0, "irrelevant": 0.0},
)
results_df = await async_evaluate_dataframe(dataframe=df, evaluators=[classifier])
```
**为什么**:`llm_classify`是旧的1.0函数。当前的模式是创建
一个带有`ClassificationEvaluator`的求值器，并使用`async_evaluate_dataframe()`运行它。

使用HallucinationEvaluator```python
# WRONG — deprecated
from phoenix.evals import HallucinationEvaluator
eval = HallucinationEvaluator(model)

# RIGHT — use FaithfulnessEvaluator
from phoenix.evals.metrics import FaithfulnessEvaluator
from phoenix.evals import LLM
eval = FaithfulnessEvaluator(llm=LLM(provider="openai", model="gpt-4o"))
```
**为什么**:`HallucinationEvaluator`不支持使用。`FaithfulnessEvaluator`是它的替代品，
使用分数最大化的“忠实”/“不忠实”标签（1.0 =忠实）。