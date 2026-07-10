#测试API参考

>从pixie源代码文档字符串自动生成。
>请勿手动编辑-运行`uv run python scripts/generate_skill_docs.py`。

小精灵。用于法学硕士应用程序的评估工具。公共API:-`Evaluation`为一个求值程序运行结果dataclass -`Evaluator`协议评估可调用-`evaluate`运行一个评估者对一个可评价的-`run_and_evaluate`评估跨越与pass/failMemoryTraceHandler -`assert_pass`批量评估标准——`assert_dataset_pass`加载数据集和运行assert_pass -`EvalAssertionError`提出当assert_pass失败-`capture_traces`上下文管理器内存跟踪捕获-`MemoryTraceHandler`InstrumentationHandler收集跨越-`ScoreThreshold`可配置的通过标准-`last_llm_call`/`root`-跟踪到可评估的助手-`DatasetEntryResult`-单个数据集条目的评估结果-`DatasetScorecard`-具有非统一评估器的每个数据集记分卡-`generate_dataset_scorecard_html`-将记分卡呈现为HTML -`save_dataset_scorecard`-将记分卡HTML写入磁盘预置的求值器（autoevals适配器）：-`AutoevalsAdapter`-任何自动值的通用包装器`Scorer`-`LevenshteinMatch`-编辑距离字符串相似性-`ExactMatch`-精确值比较-`NumericDiff`-规范化数字差异-`JSONDiff`-结构JSON比较-`ValidJSON`- JSON语法/模式验证-`ListContains`-列表重叠-`EmbeddingSimilarity`-嵌入余弦相似性-`Factuality`- LLM事实准确性检查-`ClosedQA`-闭本QA评估-`Battle`-头对头比较-`Humor`-幽默检测-`Security`-安全漏洞检查-`Sql`- SQL等价性-`Summary`-总结质量-`Translation`-翻译质量-`Possible`-可行性检查-`Moderation`-内容适度-`ContextRelevancy`- RAGAS上下文相关性-`Faithfulness`- RAGAS忠实性-`AnswerRelevancy`- RAGAS答案相关性-`AnswerCorrectness`- RAGAS答案正确性

数据集JSON格式数据集是一个JSON对象，具有以下顶级字段：```json
{
  "name": "customer-faq",
  "runnable": "pixie_qa/run_app.py:AppRunnable",
  "evaluators": ["Factuality"],
  "entries": [
    {
      "input_data": { "question": "Hello" },
      "description": "Basic greeting",
      "eval_input": [{ "name": "input", "value": "Hello" }],
      "expectation": "A friendly greeting that offers to help",
      "evaluators": ["...", "ClosedQA"]
    }
  ]
}
```
入口结构

所有字段在每个条目上都是顶级的（扁平结构-没有嵌套）：```
entry:
  ├── input_data    (required) — args for Runnable.run()
  ├── eval_input      (optional) — list of {"name": ..., "value": ...} objects (default: [])
  ├── description     (required) — human-readable label for the test case
  ├── expectation     (optional) — reference for comparison-based evaluators
  ├── eval_metadata   (optional) — extra per-entry data for custom evaluators
  └── evaluators      (optional) — evaluator names for THIS entry
```
字段引用—`runnable`（必选）：`filepath:ClassName`引用`Runnable`在评估期间驱动应用程序的子类。
-`evaluators`（数据集级别，可选）：默认的求值器名称-应用于
没有声明自己的`evaluators`的每个条目。
—`entries[].input_data`（必选）：作为参数传递给`Runnable.run()`的Kwargs
Pydantic模型。中使用的Pydantic模型的字段必须匹配`run(args: T)`。
—`entries[].description`（必选）：测试用例的可读标签。
—`entries[].eval_input`（可选，默认为`[]`）：`{"name": ..., "value": ...}`列表
对象。用于填充换行输入注册表-`wrap(purpose="input")`应用程序中的调用返回由`name`键值的注册表值。跑步者
在构建`Evaluable`时自动添加`input_data`。
—`entries[].expectation`（可选）：简明的期望描述
对于基于比较的评估者。应该描述一个正确的输出是什么样子
例如，**而不是**复制逐字输出。在跟踪中使用`pixie format`看到真实的输出形状，然后写一个更短的描述。
-`entries[].eval_metadata`（可选）：为自定义提供额外的每个条目数据
评估器——例如，预期的工具名称、布尔标志、阈值。访问
如`evaluable.eval_metadata`。
-`entries[].evaluators`（可选）：行级求值器覆盖。规则:
-省略→条目继承数据集级别`evaluators`。
-`["...", "ClosedQA"]`→数据集默认值** + ** ClosedQA。
-`["OnlyThis"]`（no`"..."`）→**only** OnlyThis，无默认值。##评估器名称解析

在数据集JSON中，求值器名称解析如下：

- **内置名称**（裸名称如`"Factuality"`，`"ExactMatch"`）是
自动解析为`pixie.{Name}`。
- **自定义评估**使用`filepath:callable_name`格式
(如`"pixie_qa/evaluators.py:my_evaluator"`)。
-自定义求值器引用指向模块级可调用对象-类
（自动实例化），工厂函数（如果0 -arg则调用），
求值器函数（按原样使用），或预实例化的可调用对象(例如；`create_llm_evaluator`结果（按原样使用）。

命令行命令

|命令|描述|| ------------------------------------------- | ------------------------------------- |
|`pixie test [path] [-v] [--no-open]`|对数据集文件|运行eval测试
|`pixie dataset create <name>`|创建新的空数据集|
|`pixie dataset list`|列出所有数据集|
|`pixie dataset save <name> [--select MODE]`|保存span到数据集|
|验证数据集JSON文件|
|生成分析和建议|

---

# #类型

# # #`Evaluable````python
class Evaluable(TestCase):
    eval_output: list[NamedData]      # wrap(purpose="output") + wrap(purpose="state") values
    # Inherited from TestCase:
    # eval_input: list[NamedData]     # from eval_input in dataset entry
    # expectation: JsonValue | _Unset # from expectation in dataset entry
    # eval_metadata: dict[str, JsonValue] | None  # from eval_metadata in dataset entry
    # description: str | None
```
评估人员的数据载体。用实际输出扩展`TestCase`。

-`eval_input`-`list[NamedData]`由条目的`eval_input`字段加上`input_data`（由跑者添加）填充。总是至少有一件物品。
—`eval_output`—`list[NamedData]`包含运行期间捕获的所有`wrap(purpose="output")`和`wrap(purpose="state")`值。每个条目都有`.name`（str）和`.value`（JsonValue）。使用`_get_output(evaluable, "name")`按名称查找。
-`eval_metadata`-`dict[str, JsonValue] | None`来自条目的`eval_metadata`字段
-`expected_output`-期望数据集文本（或`UNSET`，如果没有提供）属性:
eval_input：命名的输入数据项（来自dataset + input_data加上runner）。总是空。
eval_output：命名的输出数据项（来自运行期间的wrap调用）。
每个条目都有`.name`（str）和`.value`（JsonValue）。
包含所有`wrap(purpose="output")`和`wrap(purpose="state")`值。
eval_metadata：补充元数据（不存在时为`None`）。
expected_output：用于求值的expected/reference输出。
默认为`UNSET`（未提供）。可能是明确的
设置为`None`表示“没有预期的输出”。`wrap()`如何在测试时映射到`Evaluable`字段

当`pixie test`运行数据集条目时，应用程序中的`wrap()`调用填充求值器接收的`Evaluable`：

|`wrap()`调用在应用程序代码|可评估字段|类型|如何访问在评估|| ---------------------------------------- | ----------------- | ----------------- | ---------------------------------------------------- |
|`wrap(data, purpose="input", name="X")`|`eval_input`|`list[NamedData]`|从数据集条目|中的`eval_input`预填充
|`wrap(data, purpose="output", name="X")`|`eval_output`|`list[NamedData]`|`_get_output(evaluable, "X")`-参见|下面的帮助
|`wrap(data, purpose="state", name="X")`|`eval_output`|`list[NamedData]`|`_get_output(evaluable, "X")`-与输出|相同的列表
|（来自数据集条目`expectation`） |`expected_output`|`str \| None`|`evaluable.expected_output`|
|（来自数据集条目`eval_metadata`） |`eval_metadata`|`dict \| None`|`evaluable.eval_metadata`|

**关键洞察**:`purpose="output"`和`purpose="state"`换行值最终在`eval_output`中作为`NamedData`项结束。没有单独的`captured_output`或`captured_state`字典。使用下面的帮助函数根据换行名称查找值：```python
def _get_output(evaluable: Evaluable, name: str) -> Any:
    """Look up a wrap value by name from eval_output."""
    for item in evaluable.eval_output:
        if item.name == name:
            return item.value
    return None
```
**`eval_metadata`**用于将额外的每个条目数据传递给计算器，而不是输入数据或输出数据-例如，预期的工具名称，布尔标志，阈值。定义为条目上的顶级字段，作为`evaluable.eval_metadata`访问。

**完整的自定义评估器示例**（工具调用检查+数据集条目）：```python
from pixie import Evaluation, Evaluable

def _get_output(evaluable: Evaluable, name: str) -> Any:
    """Look up a wrap value by name from eval_output."""
    for item in evaluable.eval_output:
        if item.name == name:
            return item.value
    return None

def tool_call_check(evaluable: Evaluable, *, trace=None) -> Evaluation:
    expected = evaluable.eval_metadata.get("expected_tool") if evaluable.eval_metadata else None
    actual = _get_output(evaluable, "function_called")
    if expected is None:
        return Evaluation(score=1.0, reasoning="No expected_tool specified")
    match = str(actual) == str(expected)
    return Evaluation(
        score=1.0 if match else 0.0,
        reasoning=f"Expected {expected}, got {actual}",
    )
```
对应的数据集条目：```json
{
  "input_data": { "user_message": "I want to end this call" },
  "description": "User requests call end after failed verification",
  "eval_input": [{ "name": "user_input", "value": "I want to end this call" }],
  "expectation": "Agent should call endCall tool",
  "eval_metadata": {
    "expected_tool": "endCall",
    "expected_call_ended": true
  },
  "evaluators": ["...", "pixie_qa/evaluators.py:tool_call_check"]
}
```
# # #`Evaluation````python
Evaluation(score: 'float', reasoning: 'str', details: 'dict[str, Any]' = <factory>) -> None
```
单个评估器应用于单个测试用例的结果。

属性:
score：评价分数在0.0到1.0之间。
推理：人类可读的解释（必需）。
details：任意json序列化元数据。

# # #`ScoreThreshold````python
ScoreThreshold(threshold: 'float' = 0.5, pct: 'float' = 1.0) -> None
```
通过标准：输入的_pct_分数必须在所有评估器上得分>= _threshold_。

属性:
门槛：个人评估必须达到的最低分数。
pct：必须通过的测试用例输入（0.0-1.0）的一部分。

## Eval函数

# # #`pixie.run_and_evaluate````python
pixie.run_and_evaluate(evaluator: 'Callable[..., Any]', runnable: 'Callable[..., Any]', eval_input: 'Any', *, expected_output: 'Any' = <object object at 0x7788c2ad5c80>, from_trace: 'Callable[[list[ObservationNode]], Evaluable] | None' = None) -> 'Evaluation'
```
在捕获跟踪时运行_runnable(eval_input)_，然后求值。

结合`_run_and_capture`和`evaluate`的方便包装器。
可运行对象只被调用一次。

参数:
evaluator：一个可调用的求值器（同步或异步）。
runnable：要测试的应用程序功能。
eval*input：传递给\_runnable*的单个输入。
expected_output：可选的期望值，合并到
可评价的。
from_trace：可选调用，用于从中选择特定的span
用于评估的跟踪树。

返回:`Evaluation`结果。

提出了:
ValueError：如果在执行期间没有捕获跨度。

# # #`pixie.assert_pass````python
pixie.assert_pass(runnable: 'Callable[..., Any]', eval_inputs: 'list[Any]', evaluators: 'list[Callable[..., Any]]', *, evaluables: 'list[Evaluable] | None' = None, pass_criteria: 'Callable[[list[list[Evaluation]]], tuple[bool, str]] | None' = None, from_trace: 'Callable[[list[ObservationNode]], Evaluable] | None' = None) -> 'None'
```
针对多个输入的可运行项运行求值器。

对于每个输入，通过`_run_and_capture`运行一次可运行文件，
然后通过与每个求值器并发求值`asyncio.gather`。

结果矩阵的形状为`[eval_inputs][evaluators]`。
如果不满足通过条件，则引发：class:`EvalAssertionError`携带矩阵。

当提供`evaluables`时，行为取决于是否每个
项目已经填充了`eval_output`：

—**eval_output为None**—通过调用`runnable``run_and_evaluate`从跟踪产生输出，和
可求值中的`expected_output`被合并到结果中。
- **eval_output不是None** -直接使用可评估值
（不会为该项调用可运行项）。参数:
runnable：要测试的应用程序功能。
eval*inputs：输入列表，每个输入都传递给\_runnable*。
evaluators：可调用的评估器列表。
可评估的：`Evaluable`项的可选列表，每个输入一个。
当提供时，它们的`expected_output`被转发到`run_and_evaluate`。长度必须和
_eval_inputs_。
pass_criteria：接收结果矩阵，返回`(passed, message)`。默认为`ScoreThreshold()`。
from_trace：转发到的可选跨度选择器`run_and_evaluate`。

提出了:
EvalAssertionError：当不满足通过条件时。
ValueError：当_evaluable_length与_eval_inputs_不匹配时。

# # #`pixie.assert_dataset_pass````python
pixie.assert_dataset_pass(runnable: 'Callable[..., Any]', dataset_name: 'str', evaluators: 'list[Callable[..., Any]]', *, dataset_dir: 'str | None' = None, pass_criteria: 'Callable[[list[list[Evaluation]]], tuple[bool, str]] | None' = None, from_trace: 'Callable[[list[ObservationNode]], Evaluable] | None' = None) -> 'None'
```
按名称加载数据集，然后用它的项运行`assert_pass`。

这是一个方便的包装：

1. 从`DatasetStore`加载数据集。
2. 从每个项目中提取`eval_input`作为可运行输入。
3. 使用完整的`Evaluable`项（携带`expected_output`）
作为可评估的。
4. 委托给`assert_pass`。

参数:
runnable：要测试的应用程序功能。
dataset_name：要加载的数据集名称。
evaluators：可调用的评估器列表。
dataset_dir：覆盖数据集存储的目录。
当`None`时，从`PixieConfig.dataset_dir`读取。
pass_criteria：接收结果矩阵，返回`(passed, message)`。
from_trace：转发到的可选跨度选择器`assert_pass`。

提出了:
FileNotFoundError：如果不存在带有_dataset_name的数据集。
EvalAssertionError：当不满足通过条件时。

##跟踪助手

# # #`pixie.last_llm_call````python
pixie.last_llm_call(trace: 'list[ObservationNode]') -> 'Evaluable'
```
在跟踪树中查找带有最新`ended_at`的`LLMSpan`。

参数:
trace：跟踪树（根`ObservationNode`实例列表）。

返回:
封装最近结束的`LLMSpan`的`Evaluable`。

提出了:
ValueError：如果跟踪中不存在`LLMSpan`。

# # #`pixie.root````python
pixie.root(trace: 'list[ObservationNode]') -> 'Evaluable'
```
返回第一个根节点的跨度为`Evaluable`。

参数:
trace：跟踪树（根`ObservationNode`实例列表）。

返回:`Evaluable`包装了第一个根节点的跨度。

提出了:
ValueError：如果跟踪为空。

# # #`pixie.capture_traces````python
pixie.capture_traces() -> 'Generator[MemoryTraceHandler, None, None]'
```
安装`MemoryTraceHandler`并产生它的上下文管理器。

调用`init()`（如果已经初始化，则无操作），然后注册
通过`add_handler()`处理程序。在退出时，处理程序被删除并
将刷新传递队列，以便在`handler.spans`。