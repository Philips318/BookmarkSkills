# Manual Instrumentation （Python）

使用装饰器或上下文管理器添加自定义跨度，以实现细粒度跟踪控制。

# #设置```bash
pip install arize-phoenix-otel
```

```python
from phoenix.otel import register
tracer_provider = register(project_name="my-app")
tracer = tracer_provider.get_tracer(__name__)
```
##快速参考

| Span Kind | Decorator |用例||-----------|-----------|----------|
| CHAIN |`@tracer.chain`|业务流程、工作流、管道|
| retriver |`@tracer.retriever`|矢量搜索，文档检索|
| TOOL |`@tracer.tool`|外部API调用，函数执行|
| AGENT |`@tracer.agent`|多步推理，规划|
| LLM |`@tracer.llm`| LLM API调用（仅手动）|
|嵌入|`@tracer.embedding`|嵌入生成|
| RERANKER |`@tracer.reranker`|文档重新排序|
|护栏|`@tracer.guardrail`|安全检查、内容审核|
| EVALUATOR |`@tracer.evaluator`| LLM评估，质量检查|

装饰方法（推荐）

**全功能仪表，自动I/O捕获```python
@tracer.chain
def rag_pipeline(query: str) -> str:
    docs = retrieve_documents(query)
    ranked = rerank(docs, query)
    return generate_response(ranked, query)

@tracer.retriever
def retrieve_documents(query: str) -> list[dict]:
    results = vector_db.search(query, top_k=5)
    return [{"content": doc.text, "score": doc.score} for doc in results]

@tracer.tool
def get_weather(city: str) -> str:
    response = requests.get(f"https://api.weather.com/{city}")
    return response.json()["weather"]
```
**自定义跨度名称：**```python
@tracer.chain(name="rag-pipeline-v2")
def my_workflow(query: str) -> str:
    return process(query)
```
上下文管理器方法

**用途：**部分功能检测，自定义属性，动态控制```python
from opentelemetry.trace import Status, StatusCode
import json

def retrieve_with_metadata(query: str):
    with tracer.start_as_current_span(
        "vector_search",
        openinference_span_kind="retriever"
    ) as span:
        span.set_attribute("input.value", query)

        results = vector_db.search(query, top_k=5)

        documents = [
            {
                "document.id": doc.id,
                "document.content": doc.text,
                "document.score": doc.score
            }
            for doc in results
        ]
        span.set_attribute("retrieval.documents", json.dumps(documents))
        span.set_status(Status(StatusCode.OK))

        return documents
```
##捕获Input/Output**总是捕获I/O的求值准备跨度

自动捕获I/O（装饰器）

装饰器自动捕获输入参数和返回值：```python  theme={null}
@tracer.chain
def handle_query(user_input: str) -> str:
    result = agent.generate(user_input)
    return result.text

# Automatically captures:
# - input.value: user_input
# - output.value: result.text
# - input.mime_type / output.mime_type: auto-detected
```
###手动I/O捕获（上下文管理器）

使用`set_input()`和`set_output()`进行简单的I/O捕获：```python  theme={null}
from opentelemetry.trace import Status, StatusCode

def handle_query(user_input: str) -> str:
    with tracer.start_as_current_span(
        "query.handler",
        openinference_span_kind="chain"
    ) as span:
        span.set_input(user_input)

        result = agent.generate(user_input)

        span.set_output(result.text)
        span.set_status(Status(StatusCode.OK))

        return result.text
```
**捕获的内容：**```json
{
  "input.value": "What is 2+2?",
  "input.mime_type": "text/plain",
  "output.value": "2+2 equals 4.",
  "output.mime_type": "text/plain"
}
```
**为什么重要：**
- Phoenix评估器需要`input.value`和`output.value`Phoenix UI突出显示I/O以便调试
—启用数据导出功能，用于微调数据集

使用额外的元数据自定义I/O使用`set_attribute()`作为I/O旁边的自定义属性：```python  theme={null}
def process_query(query: str):
    with tracer.start_as_current_span(
        "query.process",
        openinference_span_kind="chain"
    ) as span:
        # Standard I/O
        span.set_input(query)

        # Custom metadata
        span.set_attribute("input.length", len(query))

        result = llm.generate(query)

        # Standard output
        span.set_output(result.text)

        # Custom metadata
        span.set_attribute("output.tokens", result.usage.total_tokens)
        span.set_status(Status(StatusCode.OK))

        return result
```
##参见Also

- **Span属性：**`span-chain.md`、`span-retriever.md`、`span-tool.md`、`span-llm.md`、`span-agent.md`、`span-embedding.md`、`span-reranker.md`、`span-guardrail.md`、`span-evaluator.md`**自动检测：**`instrumentation-auto-python.md`用于框架集成
**API文档：**https://docs.arize.com/phoenix/tracing/manual-instrumentation