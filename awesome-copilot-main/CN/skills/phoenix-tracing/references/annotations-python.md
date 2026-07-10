# Python SDK注释模式

使用Python客户端向跨度、跟踪、文档和会话添加反馈。

##客户端设置```python
from phoenix.client import Client
client = Client()  # Default: http://localhost:6006
```
## Span注释

为单个跨度添加反馈：```python
client.spans.add_span_annotation(
    span_id="abc123",
    annotation_name="quality",
    annotator_kind="HUMAN",
    label="high_quality",
    score=0.95,
    explanation="Accurate and well-formatted",
    metadata={"reviewer": "alice"},
    sync=True
)
```
##文档注释

在retriver范围内对单个文档进行评级：```python
client.spans.add_document_annotation(
    span_id="retriever_span",
    document_position=0,  # 0-based index
    annotation_name="relevance",
    annotator_kind="LLM",
    label="relevant",
    score=0.95
)
```
##跟踪注释

整个轨迹反馈：```python
client.traces.add_trace_annotation(
    trace_id="trace_abc",
    annotation_name="correctness",
    annotator_kind="HUMAN",
    label="correct",
    score=1.0
)
```
## Span笔记

注释是针对自由格式文本的一种特殊类型的注释——对开放编码很有用，在开放编码中，审阅者在任何标题存在之前都会对一个跨度留下定性观察。之后，这些音符可以聚合并提炼成结构化的标签或分数。

注释只能追加：每次调用都会自动生成一个uidv4标识符，因此多个注释自然会在同一span上累积。结构化注释的关键字是`(name, span_id, identifier)`——通过提供不同的标识符（例如，每个审阅者一个），你可以在一个跨度上有许多同名的注释；写入相同的`(name, span_id, identifier)`将覆盖现有条目。```python
client.spans.add_span_note(
    span_id="abc123def456",
    note="Unexpected token in response, needs review",
)
```
##会话注释

多回合对话反馈：```python
client.sessions.add_session_annotation(
    session_id="session_xyz",
    annotation_name="user_satisfaction",
    annotator_kind="HUMAN",
    label="satisfied",
    score=0.85
)
```
## RAG管道示例```python
from phoenix.client import Client
from phoenix.client.resources.spans import SpanDocumentAnnotationData

client = Client()

# Document relevance (batch)
client.spans.log_document_annotations(
    document_annotations=[
        SpanDocumentAnnotationData(
            name="relevance", span_id="retriever_span", document_position=i,
            annotator_kind="LLM", result={"label": label, "score": score}
        )
        for i, (label, score) in enumerate([
            ("relevant", 0.95), ("relevant", 0.80), ("irrelevant", 0.10)
        ])
    ]
)

# LLM response quality
client.spans.add_span_annotation(
    span_id="llm_span",
    annotation_name="faithfulness",
    annotator_kind="LLM",
    label="faithful",
    score=0.90
)

# Overall trace quality
client.traces.add_trace_annotation(
    trace_id="trace_123",
    annotation_name="correctness",
    annotator_kind="HUMAN",
    label="correct",
    score=1.0
)
```
## API参考

- [Python客户端API]（https://arize-phoenix.readthedocs.io/projects/client/en/latest/）