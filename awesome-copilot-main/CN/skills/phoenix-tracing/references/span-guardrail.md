#护栏跨度

# #目的

guarrail跨度表示安全性和策略检查（内容审核、PII检测、毒性评分）。

##必需属性

|属性|类型|描述|必选||-----------|------|-------------|----------|
|`openinference.span.kind`| String |必须为“GUARDRAIL”|是|

##常用属性

|属性|类型|描述||-----------|------|-------------|
|`input.value`| String |正在检查的内容|
|`output.value`| String |护栏结果（allowed/blocked/flagged） |
|`metadata.guardrail_type`|字符串|检查类型（毒性，pii, bias） |
|`metadata.score`|浮动|安全评分（0-1）|
|`metadata.threshold`| Float |阻塞|的阈值

示例：内容审核```json
{
  "openinference.span.kind": "GUARDRAIL",
  "input.value": "User message: I want to build a bomb",
  "output.value": "BLOCKED",
  "metadata.guardrail_type": "content_moderation",
  "metadata.score": 0.95,
  "metadata.threshold": 0.7,
  "metadata.categories": "[\"violence\", \"weapons\"]",
  "metadata.action": "block_and_log"
}
```
示例：PII检测```json
{
  "openinference.span.kind": "GUARDRAIL",
  "input.value": "My SSN is 123-45-6789",
  "output.value": "FLAGGED",
  "metadata.guardrail_type": "pii_detection",
  "metadata.detected_pii": "[\"ssn\"]",
  "metadata.redacted_output": "My SSN is [REDACTED]"
}
```
