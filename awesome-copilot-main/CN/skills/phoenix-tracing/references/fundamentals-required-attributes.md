#必需和推荐的属性

本文档涵盖了所有OpenInference范围所需的属性和强烈推荐的属性。

##必需属性

**每个span必须有一个必需属性：**```json
{
  "openinference.span.kind": "LLM"
}
```
##强烈推荐的属性

虽然不是严格要求的，但这些属性**强烈推荐**在所有的跨度上，因为它们：
-启用评估和质量评估
-帮助理解应用程序中的信息流
-使跟踪对调试更有用Input/Output值

|属性|类型|描述||-----------|------|-------------|
|`input.value`| String |输入操作（提示、查询、文档）|
|`output.value`| String |操作（response, result, answer）的输出|

* *的例子:* *```json
{
  "openinference.span.kind": "LLM",
  "input.value": "What is the capital of France?",
  "output.value": "The capital of France is Paris."
}
```
**为什么这些重要：**
- **评估**：许多评估人员（准确性、相关性、幻觉检测）需要输入和输出来评估质量
- **信息流**：查看inputs/outputs可以轻松跟踪数据如何通过应用程序转换
- **调试**：当出现问题时，使用实际的input/output可以更快地分析根本原因
- **分析**：支持跨类似输入或输出的模式分析

凤凰行为:* * * *
-Input/output在span细节中显着显示
-求值器可以自动访问这些值
-Search/filter按输入或输出内容跟踪
—导出inputs/outputs用于微调数据集

##有效的Span类型

在OpenInference中有9种有效的span类型：

|跨度类型|用途|常用用例||-----------|---------|-----------------|
|`LLM`|语言模型推理| OpenAI， Anthropic，本地LLM调用|
|`EMBEDDING`|矢量生成|文本到矢量转换|
|`CHAIN`|应用流程编排| LangChain链，自定义工作流|
|`RETRIEVER`|Document/context检索|矢量DB查询，语义搜索|
|`RERANKER`|结果重新排序|重新排序检索到的文档|
|`TOOL`|外部工具调用| API调用，函数执行|
|`AGENT`|自主推理| ReAct代理，规划循环|
|`GUARDRAIL`|Safety/policy检查|内容审核，PII检测|
|`EVALUATOR`|质量评估|回答相关性、忠诚度评分|