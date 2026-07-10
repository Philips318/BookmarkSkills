#评估器：自定义模板

设计法学硕士评委提示。

完成模板模式```python
TEMPLATE = """Evaluate faithfulness of the response to the context.

<context>{{context}}</context>
<response>{{output}}</response>

CRITERIA:
"faithful" = ALL claims supported by context
"unfaithful" = ANY claim NOT in context

EXAMPLES:
Context: "Price is $10" → Response: "It costs $10" → faithful
Context: "Price is $10" → Response: "About $15" → unfaithful

EDGE CASES:
- Empty context → cannot_evaluate
- "I don't know" when appropriate → faithful
- Partial faithfulness → unfaithful (strict)

Answer (faithful/unfaithful):"""
```
模板结构

1. 任务描述
2. 在XML标记中输入变量
3. 标准的定义
4. 示例（2-4例）
5. 边界情况
6. 输出格式

## XML标签```
<question>{{input}}</question>
<response>{{output}}</response>
<context>{{context}}</context>
<reference>{{reference}}</reference>
```
常见错误

|错误|修复|| ------- | --- |
模糊标准|精确定义每个标签|
|无示例|包括2-4例|
|格式模糊|指定精确输出|
|无边界情况|地址模糊|