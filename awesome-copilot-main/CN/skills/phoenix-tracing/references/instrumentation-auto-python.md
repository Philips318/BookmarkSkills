# Phoenix Tracing: Auto-Instrumentation （Python）

**为LLM调用自动创建跨度而无需更改代码

# #概述

自动检测补丁支持库在运行时自动创建跨度。使用支持的框架（LangChain, LlamaIndex， OpenAI SDK等）。对于自定义逻辑，manual-instrumentation-python.md。

支持的框架

Python: * * * *

LLM sdk: OpenAI, Anthropic, Bedrock, Mistral, Vertex AI, Groq, Ollama
-框架：LangChain， LlamaIndex, DSPy, CrewAI, Instructor, Haystack
—安装路径：`pip install openinference-instrumentation-{name}`# #设置

**安装和启用：**```bash
pip install arize-phoenix-otel
pip install openinference-instrumentation-openai  # Add others as needed
```

```python
from phoenix.otel import register

register(project_name="my-app", auto_instrument=True)  # Discovers all installed instrumentors
```
* *的例子:* *```python
from phoenix.otel import register
from openai import OpenAI

register(project_name="my-app", auto_instrument=True)

client = OpenAI()
response = client.chat.completions.create(
    model="gpt-4",
    messages=[{"role": "user", "content": "Hello!"}]
)
```
跟踪出现在Phoenix UI中，模型，input/output，令牌，定时自动捕获。有关完整的属性模式，请参阅span kind文件。

**选择性仪器**（显式控制）：```python
from phoenix.otel import register
from openinference.instrumentation.openai import OpenAIInstrumentor

tracer_provider = register(project_name="my-app")  # No auto_instrument
OpenAIInstrumentor().instrument(tracer_provider=tracer_provider)
```
# #的局限性

自动检测不捕获：

-自定义业务逻辑
-内部函数调用

* *的例子:* *```python
def my_custom_workflow(query: str) -> str:
    preprocessed = preprocess(query)  # Not traced
    response = client.chat.completions.create(...)  # Traced (auto)
    postprocessed = postprocess(response)  # Not traced
    return postprocessed
```
**解决方案：**增加手动仪表：```python
@tracer.chain
def my_custom_workflow(query: str) -> str:
    preprocessed = preprocess(query)
    response = client.chat.completions.create(...)
    postprocessed = postprocess(response)
    return postprocessed
```
