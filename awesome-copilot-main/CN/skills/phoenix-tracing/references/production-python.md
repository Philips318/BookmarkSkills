# Phoenix Tracing: Production Guide （Python）

**CRITICAL：为生产部署配置批处理、数据屏蔽和跨度过滤

# #元数据

|属性|值||-----------|-------|
|优先级|关键-生产就绪|
|影响|安全、性能|
设置时间| 5-15分钟|

批处理

**支持批量加工，提高生产效率。批处理通过分组而不是单独发送跨度来减少网络开销。

数据屏蔽（PII保护）

* *环境变量:* *```bash
export OPENINFERENCE_HIDE_INPUTS=true          # Hide input.value
export OPENINFERENCE_HIDE_OUTPUTS=true         # Hide output.value
export OPENINFERENCE_HIDE_INPUT_MESSAGES=true  # Hide LLM input messages
export OPENINFERENCE_HIDE_OUTPUT_MESSAGES=true # Hide LLM output messages
export OPENINFERENCE_HIDE_INPUT_IMAGES=true    # Hide image content
export OPENINFERENCE_HIDE_INPUT_TEXT=true      # Hide embedding text
export OPENINFERENCE_BASE64_IMAGE_MAX_LENGTH=10000  # Limit image size
```
Python TraceConfig: * * * *```python
from phoenix.otel import register
from openinference.instrumentation import TraceConfig

config = TraceConfig(
    hide_inputs=True,
    hide_outputs=True,
    hide_input_messages=True
)
register(trace_config=config)
```
**优先级：**代码>环境变量>默认值

---

## Span过滤

**抑制特定代码块：**```python
from phoenix.otel import suppress_tracing

with suppress_tracing():
    internal_logging()  # No spans generated
```
