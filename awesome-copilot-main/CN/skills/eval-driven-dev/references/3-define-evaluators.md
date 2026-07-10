#步骤3：定义评估器

**为什么此步骤**：随着应用程序的仪器化（步骤2），现在你可以将每个评估标准映射到一个具体的评估器-在需要的地方实现自定义的-所以数据集（步骤4）可以通过名称引用它们。

---

# # 3。将标准映射到评估者

**步骤1c中的每个评估标准-包括用户在提示符中指定的任何尺寸-必须有相应的评估器。**如果用户要求“事实性、完整性和偏见”，你需要三个评估者（或者一个涵盖这三个方面的多标准评估者）。不要静默地删除任何请求的维度。优先考虑衡量`pixie_qa/00-project-analysis.md`中确定的**难题/故障模式**的评估人员——这些评估人员比一般的质量评估人员更有价值。

对于每个评估标准，使用以下决策顺序选择一个评估器：1. **内置评估器** -如果标准评估器符合标准（事实正确性→`Factuality`，精确匹配→`ExactMatch`， RAG可靠性→`Faithfulness`）。有关完整目录，请参阅`evaluators.md`。
2. **代理评估器** (`create_agent_evaluator`) - **所有语义，定性和应用程序特定标准的默认值**。代理评估器在步骤6中由您（编码代理）评分，在步骤6中，您将全面地检查每个条目的跟踪和输出。这比诸如“提取是否准确地捕获了源内容？”、“是否存在幻觉值？”或“应用是否优雅地处理了嘈杂的输入？”等标准的自动评分要有效得多。
3. **手动自定义评估器** -仅用于**机械，确定性检查**，其中编程函数是绝对正确的：字段存在，regex模式匹配，JSON模式验证，数字阈值，类型检查。**不要使用手动自定义评估语义质量的评估器——如果检查需要判断内容是否正确、相关或完整，请使用代理评估器。**区分结构标准和语义标准**：对于每一个标准，问：“这可以用一个简单的编程规则来检查，并且总是给出正确的答案吗？”是= >手动自定义评估器。否= >代理评估器。大多数特定于应用程序的质量标准是语义的，而不是结构的。

对于开放式LLM文本，**永远不要**使用`ExactMatch`- LLM输出是不确定的。`AnswerRelevancy`是**RAG-only** -它需要在跟踪中使用`context`值。不带它返回0.0。对于一般相关性，使用具有明确标准的代理评估器。

# # 3 b。实现自定义评估器

如果任何标准需要自定义评估器，现在就实现它。将自定义求值器放置在`pixie_qa/evaluators.py`中（如果有许多子模块，则放置在子模块中）。

代理评估器(`create_agent_evaluator`) -默认值对所有语义、定性和基于判断的标准使用代理评估器。这些由您（编码代理）在步骤5d中分级，在步骤5d中，您用完整的上下文检查每个条目的跟踪和输出——在准确性、完整性、幻觉检测或错误处理等质量维度上，这比任何自动化方法都要有效得多。```python
from pixie import create_agent_evaluator

extraction_accuracy = create_agent_evaluator(
    name="ExtractionAccuracy",
    criteria="The extracted data accurately reflects the source content. All fields "
             "contain correct values from the source — no hallucinated, fabricated, or "
             "placeholder values. Compare the final_answer against the fetched_content "
             "and parsed_content to verify every claimed fact.",
)

noise_handling = create_agent_evaluator(
    name="NoiseHandling",
    criteria="The app correctly ignored navigation chrome, boilerplate, ads, and other "
             "non-content elements from the source. The extracted data contains only "
             "information relevant to the user's prompt, not noise from the page structure.",
)

schema_compliance = create_agent_evaluator(
    name="SchemaCompliance",
    criteria="The output contains all fields requested in the prompt with appropriate "
             "types and non-trivial values. Missing fields, null values for required data, "
             "or fields with generic placeholder text indicate failure.",
)
```
通过`filepath:callable_name`引用数据集中的代理评估器（例如，`"pixie_qa/evaluators.py:extraction_accuracy"`）。

在`pixie test`期间，代理评估器在控制台中显示为`⏳`。它们在步骤5d中被分级。

**编写有效标准**:`criteria`字符串是您将在步骤5d中遵循的评分标准。制定具体的、可操作的计划：

- **坏**：“检查输出是否良好”-太模糊而无法持续评分
- **坏**：“回应应该准确”-没有说要比较什么
- **好**：“将提取的字段与源比较HTML/document.每个字段必须在源中有相应的通道。标记任何值不能追溯到源内容的字段。”
- **好**：“应用程序应该保留源文档的结构层次结构。如果源具有sections/subsections，则提取应该反映嵌套，而不是将所有内容平摊到单个级别。”手动自定义评估器-仅用于机械检查

只有当一个简单的函数确定地给出正确答案时，才使用手动自定义的求值器进行确定性的、程序性的检查。示例：字段存在、正则表达式匹配、JSON模式验证、数字范围检查、类型验证。

**不要使用手动自定义的语义质量评估器。**如果检查需要判断内容是否正确、相关、完整或写得好，请使用代理评估器。试金石：“正则表达式、字符串匹配或比较操作符能否完美地实现此检查？”如果不是，那就是语义问题——使用代理评估器。

自定义求值器可以是同步或异步函数。将它们赋值给`pixie_qa/evaluators.py`中的模块级变量：```python
from pixie import Evaluation, Evaluable

def my_evaluator(evaluable: Evaluable, *, trace=None) -> Evaluation:
    score = 1.0 if "expected pattern" in str(evaluable.eval_output) else 0.0
    return Evaluation(score=score, reasoning="...")
```
数据集中`filepath:callable_name`的引用：`"pixie_qa/evaluators.py:my_evaluator"`。

**访问`eval_metadata`和捕获的数据**：自定义求值器通过`Evaluable`字段访问每个条目的元数据和`wrap()`输出：

-`evaluable.eval_metadata`-从条目的`eval_metadata`字段（例如，`{"expected_tool": "endCall"}`）中查找
—`evaluable.eval_output`—`list[NamedData]`包含所有`wrap(purpose="output")`和`wrap(purpose="state")`值。每个条目都有`.name`（str）和`.value`（JsonValue）。使用下面的帮助器按名称查找。```python
def _get_output(evaluable: Evaluable, name: str) -> Any:
    """Look up a wrap value by name from eval_output."""
    for item in evaluable.eval_output:
        if item.name == name:
            return item.value
    return None

def call_ended_check(evaluable: Evaluable, *, trace=None) -> Evaluation:
    expected = evaluable.eval_metadata.get("expected_call_ended") if evaluable.eval_metadata else None
    actual = _get_output(evaluable, "call_ended")
    if expected is None:
        return Evaluation(score=1.0, reasoning="No expected_call_ended in eval_metadata")
    match = bool(actual) == bool(expected)
    return Evaluation(
        score=1.0 if match else 0.0,
        reasoning=f"Expected call_ended={expected}, got {actual}",
    )
```
ValidJSON和字符串期望冲突`ValidJSON`在出现时将数据集条目的`expectation`字段视为JSON Schema。如果您的条目使用**string**期望（例如，对于`Factuality`），将`ValidJSON`添加为数据集级别的默认求值器将导致失败-它无法将普通字符串验证为JSON模式。要么只对具有object/boolean期望的条目应用`ValidJSON`，要么在数据集依赖于字符串期望时忽略它。

# # 3 c。生成评估器映射工件

将标准到求值器的映射编写到`pixie_qa/03-evaluator-mapping.md`。这个工件在评估标准（步骤1c）和数据集（步骤4）之间架起桥梁。

**CRITICAL**：使用`evaluators.md`引用中出现的准确的求值器名称——内置求值器使用它们的短名称（例如，`Factuality`,`ClosedQA`），而自定义求值器使用`filepath:callable_name`格式（例如，`pixie_qa/evaluators.py:ConciseVoiceStyle`）。

# # #模板```markdown
# Evaluator Mapping

## Built-in evaluators used

| Evaluator name | Criterion it covers | Applies to                 |
| -------------- | ------------------- | -------------------------- |
| Factuality     | Factual accuracy    | All items                  |
| ClosedQA       | Answer correctness  | Items with expected_output |

## Agent evaluators

| Evaluator name                             | Criterion it covers          | Applies to | Source file            |
| ------------------------------------------ | ---------------------------- | ---------- | ---------------------- |
| pixie_qa/evaluators.py:extraction_accuracy | Content accuracy vs source   | All items  | pixie_qa/evaluators.py |
| pixie_qa/evaluators.py:noise_handling      | Navigation/boilerplate noise | All items  | pixie_qa/evaluators.py |

## Manual custom evaluators (mechanical checks only)

| Evaluator name                                 | Criterion it covers  | Applies to | Source file            |
| ---------------------------------------------- | -------------------- | ---------- | ---------------------- |
| pixie_qa/evaluators.py:required_fields_present | Required field check | All items  | pixie_qa/evaluators.py |

## Applicability summary

- **Dataset-level defaults** (apply to all items): Factuality, pixie_qa/evaluators.py:extraction_accuracy
- **Item-specific** (apply to subset): ClosedQA (only items with expected_output)
```
# #输出

-`pixie_qa/evaluators.py`中的自定义求值器实现（如果需要任何自定义求值器）
-`pixie_qa/03-evaluator-mapping.md`-标准到求值器的映射

---

b> **评估器选择指南**：参见`evaluators.md`获得完整的内置评估器目录和`create_agent_evaluator`参考。
>
b> **如果在实现求值器时遇到意外错误**（导入失败、API不匹配），请先读取`evaluators.md`以获取权威的求值器引用，然后读取`wrap-api.md`以获取API详细信息，然后再猜测修复方法。