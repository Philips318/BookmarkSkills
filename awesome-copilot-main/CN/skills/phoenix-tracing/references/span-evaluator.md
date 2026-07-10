#评估者跨度

# #目的

EVALUATOR范围代表质量评估操作（答案相关性、可信度、幻觉检测）。

##必需属性

|属性|类型|描述|必选||-----------|------|-------------|----------|
|`openinference.span.kind`| String |必须为“EVALUATOR” |是|

##常用属性

|属性|类型|描述||-----------|------|-------------|
|`input.value`|字符串|正在评估的内容|
|`output.value`| String |评估结果（分数、标签、解释）|
|`metadata.evaluator_name`| String |求值器标识符|
|`metadata.score`|浮点|数字分数（0-1）|
|`metadata.label`|字符串|分类标签（relevant/irrelevant） |

示例：回答相关性```json
{
  "openinference.span.kind": "EVALUATOR",
  "input.value": "{\"question\": \"What is the capital of France?\", \"answer\": \"The capital of France is Paris.\"}",
  "input.mime_type": "application/json",
  "output.value": "0.95",
  "metadata.evaluator_name": "answer_relevance",
  "metadata.score": 0.95,
  "metadata.label": "relevant",
  "metadata.explanation": "Answer directly addresses the question with correct information"
}
```
示例：忠诚度检查```json
{
  "openinference.span.kind": "EVALUATOR",
  "input.value": "{\"context\": \"Paris is in France.\", \"answer\": \"Paris is the capital of France.\"}",
  "input.mime_type": "application/json",
  "output.value": "0.5",
  "metadata.evaluator_name": "faithfulness",
  "metadata.score": 0.5,
  "metadata.label": "partially_faithful",
  "metadata.explanation": "Answer makes unsupported claim about Paris being the capital"
}
```
