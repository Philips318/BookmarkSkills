#内置求值器

>从pixie源代码文档字符串自动生成。
>请勿手动编辑-运行`uv run python scripts/generate_skill_docs.py`。

自动评估适配器-包装`autoevals`评分器的预制评估器。

这个模块提供：类：`AutoevalsAdapter`，它桥接
autoeval`Scorer`接口到pixie的`Evaluator`协议，并且
一组用于通用评估任务的工厂函数。

公共API（也都是从`pixie.evals`重新导出的）：

**核心适配器：** -:class:`AutoevalsAdapter`-任何自动eval的通用包装器`Scorer`。

**启发式评分器（不需要LLM）：** -:func:`LevenshteinMatch`-编辑距离字符串相似性。-:func:`ExactMatch`-精确值比较。-:func:`NumericDiff`-归一化数值差异。-:func:`JSONDiff`- JSON结构比较。-:func:`ValidJSON`- JSON语法/模式验证。-:func:`ListContains`-两个字符串列表之间的重叠。

**嵌入评分器：** -:func:`EmbeddingSimilarity`-余弦相似度通过嵌入。** llm作为裁判评分：** -:func:`Factuality`，：func:`ClosedQA`,:func:`Battle`，
：func:`Humor`,:func:`Security`,:func:`Sql`，
：func:`Summary`,:func:`Translation`,:func:`Possible`。

**审核：** -:func:`Moderation`- OpenAI内容审核检查。

**RAGAS参数：** -:func:`ContextRelevancy`，：func:`Faithfulness`，
: func:`AnswerRelevancy`, func:`AnswerCorrectness`。

评价者选择指南

根据**输出类型**和评估标准选择评估器：

|输出类型|评估器类别|示例|| -------------------------------------------- | ----------------------------------------------------------- | -------------------------------------- |
|确定性（标签，yes/no，固定格式）|启发式：`ExactMatch`，`JSONDiff`，`ValidJSON`|标签分类，JSON提取|
|带参考答案的开放式文本| LLM-as-judge:`Factuality`，`ClosedQA`，`AnswerCorrectness`|聊天机器人响应，问答，总结|
|文本期望context/grounding| RAG:`Faithfulness`，`ContextRelevancy`| RAG管道|
|文本与style/format要求|自定义通过`create_llm_evaluator`|语音友好的响应，音调检查|
|多方面的质量|多个评估者结合|事实+相关性+语气|
跟踪依赖质量（工具使用，路由）|代理评估器通过`create_agent_evaluator`|工具正确性，多步骤推理|

关键的规则:-对于开放式LLM文本，**永远**不要使用`ExactMatch`- LLM输出为
不确定的。
-`AnswerRelevancy`是**RAG-only** -需要`context`在跟踪。
不带它返回0.0。对于一般相关性，请使用`create_llm_evaluator`。
-不要使用比较评估器(`Factuality`,`ClosedQA`，`ExactMatch`)在没有`expected_output`的物品上产生
毫无意义的分数。

---

##评估参考

# # #`AnswerCorrectness````python
AnswerCorrectness(*, client: 'Any' = None) -> 'AutoevalsAdapter'
```
答案正确性评估器（RAGAS）。

判断`eval_output`与是否正确`expected_output`，结合事实相似度和语义
相似点。

**何时使用**:QA场景在RAG管道中，你有一个
参考答案并要综合正确得分。

**需要`expected_output`**：是的。
**需要`eval_metadata["context"]`**：可选（提高精度）。

参数:
client: OpenAI客户端实例。

# # #`AnswerRelevancy````python
AnswerRelevancy(*, client: 'Any' = None) -> 'AutoevalsAdapter'
```
答案相关性评估器（RAGAS）。

判断`eval_output`是否直接解决中的问题`eval_input`。

**何时只使用**:RAG管道-需要在
跟踪。不带它返回0.0。用于一般（非rag）响应
相关性，使用带有自定义提示符的`create_llm_evaluator`代替。

**要求`expected_output`**：不。
**需要`eval_metadata["context"]`**：是- **RAG管道仅**。

参数:
client: OpenAI客户端实例。

# # #`Battle````python
Battle(*, model: 'str | None' = None, client: 'Any' = None) -> 'AutoevalsAdapter'
```
头对头比较评估（法学硕士作为法官）。

使用LLM比较`eval_output`和`expected_output`根据`eval_input`的指令确定哪个更好。

**何时使用**:A/B测试场景，比较模型输出；
或者对不同的回答进行排序。

**需要`expected_output`**：是的。

参数:
model: LLM模型名称。
client: OpenAI客户端实例。

# # #`ClosedQA````python
ClosedQA(*, model: 'str | None' = None, client: 'Any' = None) -> 'AutoevalsAdapter'
```
闭卷式问答评价员（法学硕士作为裁判）。

使用LLM判断`eval_output`是否正确回答
问题是`eval_input`和`expected_output`的比较。(可选)
转发`eval_metadata["criteria"]`用于自定义分级标准。

**何时使用**：在QA场景中，答案应该与参考相匹配
例如，客户支持回答、知识库查询。

**要求`expected_output`**：是-不要在没有的物品上使用`expected_output`;产生无意义的分数。

参数:
model: LLM模型名称。
client: OpenAI客户端实例。

# # #`ContextRelevancy````python
ContextRelevancy(*, client: 'Any' = None) -> 'AutoevalsAdapter'
```
上下文相关性评估器（RAGAS）。

判断检索的上下文是否与查询相关。
将`eval_metadata["context"]`转发到底层计分器。

**何时使用**:RAG管道-评估检索质量。

**需要`expected_output`**：是的。
**需要`eval_metadata["context"]`**：是（仅限RAG管道）。

参数:
client: OpenAI客户端实例。

# # #`EmbeddingSimilarity````python
EmbeddingSimilarity(*, prefix: 'str | None' = None, model: 'str | None' = None, client: 'Any' = None) -> 'AutoevalsAdapter'
```
基于嵌入的语义相似度评估器。

计算`eval_output`的嵌入向量之间的余弦相似度
和`expected_output`。

**何时使用**：在准确的情况下比较两个文本的语义
措辞不重要。比Levenshtein的释义更有力
内容，但不像法学硕士那样细致入微。

**需要`expected_output`**：是的。

参数:
prefix：为域上下文添加的可选文本。
model：嵌入模型名称。
client: OpenAI客户端实例。

# # #`ExactMatch````python
ExactMatch() -> 'AutoevalsAdapter'
```
精确值比较求值器。

如果`eval_output`完全等于`expected_output`，返回1.0；
0.0。

**何时使用**：确定性的、结构化的输出(分类标签、yes/no答案，固定格式字符串)。**不要**用于开放式LLM
text - LLM输出是不确定的，所以精确匹配将几乎总是
失败。

**需要`expected_output`**：是的。

# # #`Factuality````python
Factuality(*, model: 'str | None' = None, client: 'Any' = None) -> 'AutoevalsAdapter'
```
事实准确性评估员（法学硕士作为法官）。

使用LLM判断`eval_output`是否与事实一致
使用`expected_output`给出`eval_input`上下文。

何时使用**：事实正确性重要的开放式文本
（聊天机器人回复，QA回答，总结）。优先于`ExactMatch`用于llm生成的文本。

**要求`expected_output`**：是-不要在没有的物品上使用`expected_output`;产生无意义的分数。

参数:
model: LLM模型名称。
client: OpenAI客户端实例。

# # #`Faithfulness````python
Faithfulness(*, client: 'Any' = None) -> 'AutoevalsAdapter'
```
忠诚评估（RAGAS）。

判断`eval_output`是否忠实于（即支持）
提供的上下文。前锋`eval_metadata["context"]`。

**何时使用**:RAG管道-确保答案不会
产生超出检索到的上下文所支持的幻觉。

**需要`expected_output`**：不需要。
**需要`eval_metadata["context"]`**：是（仅限RAG管道）。

参数:
client: OpenAI客户端实例。

# # #`Humor````python
Humor(*, model: 'str | None' = None, client: 'Any' = None) -> 'AutoevalsAdapter'
```
幽默质量评估员（法学硕士）。

用LLM来判断`eval_output`的幽默质量`expected_output`。

何时使用**：评价创意写作中的幽默，聊天机器人
个性，或娱乐应用。

**需要`expected_output`**：是的。

参数:
model: LLM模型名称。
client: OpenAI客户端实例。

# # #`JSONDiff````python
JSONDiff(*, string_scorer: 'Any' = None) -> 'AutoevalsAdapter'
```
结构JSON比较求值器。

递归比较两个JSON结构并产生相似性
得分。处理嵌套对象、数组和混合类型。

**何时使用**：结构化JSON输出中字段级比较
（例如，提取的数据、API响应模式、工具调用参数）。

**需要`expected_output`**：是。

参数:
string_scorer：字符串字段的可选成对记分器。

# # #`LevenshteinMatch````python
LevenshteinMatch() -> 'AutoevalsAdapter'
```
编辑距离字符串相似性评估器。

计算`eval_output`和之间的标准化Levenshtein距离`expected_output`。对于相同的字符串并递减返回1.0
随着编辑距离的增加得分。

**何时使用**：在小的情况下，确定或接近确定的输出
文本变化是可以接受的(例如，格式差异，次要的
拼写)。不适合开放式法学硕士文本-使用法学硕士作为裁判
评估者。

**需要`expected_output`**：是的。

# # #`ListContains````python
ListContains(*, pairwise_scorer: 'Any' = None, allow_extra_entities: 'bool' = False) -> 'AutoevalsAdapter'
```
列表重叠评估器。

检查`eval_output`是否包含来自的所有项`expected_output`。分数基于重叠比例。

**何时使用**：输出生成一个项目列表，其中完整性
事项（例如提取的实体、搜索结果、推荐）。

**需要`expected_output`**：是的。

参数:
pairwise_scorer：成对元素比较的可选评分器。
allow_extra_entities：如果为True，则输出中的多余项不会受到惩罚。

# # #`Moderation````python
Moderation(*, threshold: 'float | None' = None, client: 'Any' = None) -> 'AutoevalsAdapter'
```
内容审核评估员。

使用OpenAI适度API检查`eval_output`是否不安全
内容（仇恨言论、暴力、自残等）。

**何时使用**：任何需要考虑输出安全的应用程序
聊天机器人，内容生成，面向用户的人工智能。

**需要`expected_output`**：不需要。

参数:
threshold：自定义标记阈值。
client: OpenAI客户端实例。

# # #`NumericDiff````python
NumericDiff() -> 'AutoevalsAdapter'
```
规范化数值差分求值器。

计算`eval_output`和之间的规范化数字距离`expected_output`。对于相同的数字并递减返回1.0
分数随着差异的增加而增加。

**何时使用**：可以接受近似相等的数字输出
（如价格计算、分数、测量）。

**需要`expected_output`**：是的。

# # #`Possible````python
Possible(*, model: 'str | None' = None, client: 'Any' = None) -> 'AutoevalsAdapter'
```
可行性/合理性评估（法学硕士作为法官）。

使用LLM来判断`eval_output`是可信的还是
可行的反应。

**何时使用**：通用的质量检查，当你需要的时候
验证输出是合理的，没有特定的参考答案。

**需要`expected_output`**：不需要。

参数:
model: LLM模型名称。
client: OpenAI客户端实例。

# # #`Security````python
Security(*, model: 'str | None' = None, client: 'Any' = None) -> 'AutoevalsAdapter'
```
安全漏洞评估器（LLM-as-judge）。

使用LLM检查`eval_output`是否存在安全漏洞
基于`eval_input`中的指令。

**何时使用**：代码生成、SQL输出或任何场景
必须检查输出是否存在注入或漏洞风险。

**需要`expected_output`**：不需要。

参数:
model: LLM模型名称。
client: OpenAI客户端实例。

# # #`Sql````python
Sql(*, model: 'str | None' = None, client: 'Any' = None) -> 'AutoevalsAdapter'
```
SQL等价性评估器（LLM-as-judge）。

使用LLM判断`eval_output`SQL是否具有语义
相当于`expected_output`SQL。

**何时使用**：文本到SQL的应用程序在其中生成SQL
应该在功能上等同于引用查询。

**需要`expected_output`**：是的。

参数:
model: LLM模型名称。
client: OpenAI客户端实例。

# # #`Summary````python
Summary(*, model: 'str | None' = None, client: 'Any' = None) -> 'AutoevalsAdapter'
```
总结质量评估员（llm -judge）。

使用LLM来判断`eval_output`的质量作为总结
与`expected_output`中的参考摘要相比。

**何时使用**：总结任务中必须捕获的输出
来自原始材料的关键信息。

**需要`expected_output`**：是的。

参数:
model: LLM模型名称。
client: OpenAI客户端实例。

# # #`Translation````python
Translation(*, language: 'str | None' = None, model: 'str | None' = None, client: 'Any' = None) -> 'AutoevalsAdapter'
```
翻译质量评估员（法学硕士）。

使用LLM来判断`eval_output`的翻译质量
与目标语言中的`expected_output`相比。

**何时使用**：机器翻译或多语言输出场景。

**需要`expected_output`**：是的。

参数:
语言：目标语言（如`"Spanish"`）。
model: LLM模型名称。
client: OpenAI客户端实例。

# # #`ValidJSON````python
ValidJSON(*, schema: 'Any' = None) -> 'AutoevalsAdapter'
```
JSON语法和模式验证评估器。

如果`eval_output`是有效的JSON（并可选匹配），则返回1.0
提供的模式)，否则为0.0。

**何时使用**：输出必须是有效的JSON -可选的一致性
到一个特定的模式（例如，工具调用响应，结构化提取）。

**需要`expected_output`**：不需要。

参数:
schema：要验证的可选JSON模式。

---

自定义评估器：`create_llm_evaluator`从提示模板定制llm作为评判评估器的工厂。

用法::    from pixie import create_llm_evaluator

    concise_voice_style = create_llm_evaluator(
        name="ConciseVoiceStyle",
        prompt_template="""
        You are evaluating whether a voice agent response is concise and
        phone-friendly.

        User said: {eval_input}
        Agent responded: {eval_output}
        Expected behavior: {expectation}

        Score 1.0 if the response is concise (under 3 sentences), directly
        addresses the question, and uses conversational language suitable for
        a phone call. Score 0.0 if it's verbose, off-topic, or uses
        written-style formatting.
        """,
    )
# # #`create_llm_evaluator````python
create_llm_evaluator(name: 'str', prompt_template: 'str', *, model: 'str' = 'gpt-4o-mini', client: 'Any | None' = None) -> '_LLMEvaluator'
```
从提示模板创建自定义llm作为法官的评估器。

模板可以引用这些变量(从
:类:`~pixie.storage.evaluable.Evaluable`字段):

-`{eval_input}`-可求值的输入数据。单项目列表扩展
该项目的价值；多项列表扩展为JSON字典`name → value`对。
-`{eval_output}`-可求值的输出数据(与`eval_input`)。
-`{expectation}`-可评估对象的预期输出

参数:
name：评估者的显示名称（在记分卡中显示）。
一个带`{eval_input}`的字符串模板，`{eval_output}`，and/or`{expectation}`占位符。
model: OpenAI模型名称（默认为`gpt-4o-mini`）。
client：可选的预配置OpenAI客户端实例。

返回:
满足`Evaluator`协议的可调用求值器。

提出了:
ValueError：如果模板使用嵌套字段访问，如`{eval_input[key]}`（只支持顶级占位符）。

# # #`create_agent_evaluator````python
create_agent_evaluator(name: 'str', criteria: 'str') -> '_AgentEvaluator'
```
创建一个评估员，其评分延迟到编码代理。

在`pixie test`期间，代理评估器不会自动评分。
相反，它们提高`AgentEvaluationPending`并记录a`PendingEvaluation`带有评价标准。编码代理
（在步骤6的指导下）检查每个条目的跟踪和输出
对待完成的评估进行评分。

**何时使用**：需要全面审查的质量维度
LLM跟踪-工具调用正确性，多步推理质量，
路由决策——这是自动的法学硕士作为法官提示不能做到的
捕捉细微差别。

**何时不使用**：简单的文本质量检查(使用
而不是`create_llm_evaluator`)，确定性检查(使用启发式
评估者)，或者任何可以从输入+输出中打分的标准
孤独而没有痕迹的语境。参数:
name：评估者的显示名称（在记分卡中显示为⏳pending）。
标准：评估什么-代理的评分说明
将在审查结果时遵循。具体且可操作。

返回:
满足`Evaluator`协议的可调用求值器。它的`__call__`引发`AgentEvaluationPending`，而不是返回`Evaluation`。

例子:```python
from pixie import create_agent_evaluator

ResponseQuality = create_agent_evaluator(
    name="ResponseQuality",
    criteria="The response directly addresses the user's question with "
             "accurate, well-structured information. No hallucinations "
             "or off-topic content.",
)

ToolUsageCorrectness = create_agent_evaluator(
    name="ToolUsageCorrectness",
    criteria="The app called the correct tools in the right order based "
             "on the user's intent. No unnecessary or missed tool calls.",
)
```
