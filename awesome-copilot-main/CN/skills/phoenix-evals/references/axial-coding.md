#轴向编码

将开放式笔记分组到结构化的失败分类中。

# #过程

1. **收集** -收集开放的编码笔记
2. **模式** -用共同的主题分组笔记
3. **名称** -创建可操作的类别名称
4. **量化** -统计每个类别的失败

##示例分类```yaml
failure_taxonomy:
  content_quality:
    hallucination: [invented_facts, fictional_citations]
    incompleteness: [partial_answer, missing_key_info]
    inaccuracy: [wrong_numbers, wrong_dates]
  
  communication:
    tone_mismatch: [too_casual, too_formal]
    clarity: [ambiguous, jargon_heavy]
  
  context:
    user_context: [ignored_preferences, misunderstood_intent]
    retrieved_context: [ignored_documents, wrong_context]
  
  safety:
    missing_disclaimers: [legal, medical, financial]
```
##添加注释（Python）```python
from phoenix.client import Client

client = Client()
client.spans.add_span_annotation(
    span_id="abc123",
    annotation_name="failure_category",
    label="hallucination",
    explanation="invented a feature that doesn't exist",
    annotator_kind="HUMAN",
    sync=True,
)
```
##添加注释（TypeScript）```typescript
import { addSpanAnnotation } from "@arizeai/phoenix-client/spans";

await addSpanAnnotation({
  spanAnnotation: {
    spanId: "abc123",
    name: "failure_category",
    label: "hallucination",
    explanation: "invented a feature that doesn't exist",
    annotatorKind: "HUMAN",
  }
});
```
代理故障分类```yaml
agent_failures:
  planning: [wrong_plan, incomplete_plan]
  tool_selection: [wrong_tool, missed_tool, unnecessary_call]
  tool_execution: [wrong_parameters, type_error]
  state_management: [lost_context, stuck_in_loop]
  error_recovery: [no_fallback, wrong_fallback]
```
转移矩阵（agent）

显示状态之间发生故障的位置：```python
def build_transition_matrix(conversations, states):
    matrix = defaultdict(lambda: defaultdict(int))
    for conv in conversations:
        if conv["failed"]:
            last_success = find_last_success(conv)
            first_failure = find_first_failure(conv)
            matrix[last_success][first_failure] += 1
    return pd.DataFrame(matrix).fillna(0)
```
# #原则

- **MECE** -每个故障属于一个类别
- **可操作的** -类别建议修复
- **自下而上** -让分类从数据中产生