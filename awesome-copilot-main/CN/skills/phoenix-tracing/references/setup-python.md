# Phoenix Tracing: Python设置

**在Python中使用`arize-phoenix-otel`.**设置Phoenix跟踪

# #元数据

|属性|值|| ---------- | ----------------------------------- |
|优先级| Critical -所有跟踪|都需要
|设置时间| <5 min |

##快速启动（3行）```python
from phoenix.otel import register
register(project_name="my-app", auto_instrument=True)
```
**连接到`http://localhost:6006`，自动仪表所有支持的库

# #安装```bash
pip install arize-phoenix-otel
```
**支持：** Python 3.10-3.13

# #配置

环境变量（推荐）```bash
export PHOENIX_API_KEY="your-api-key"  # Required for Phoenix Cloud
export PHOENIX_COLLECTOR_ENDPOINT="http://localhost:6006"  # Or Cloud URL
export PHOENIX_PROJECT_NAME="my-app"  # Optional
```
Python代码```python
from phoenix.otel import register

tracer_provider = register(
    project_name="my-app",              # Project name
    endpoint="http://localhost:6006",   # Phoenix endpoint
    auto_instrument=True,               # Auto-instrument supported libs
    batch=True,                         # Batch processing (default: True)
)
```
* *参数:* *

-`project_name`：项目名称（覆盖`PHOENIX_PROJECT_NAME`）
-`endpoint`: Phoenix URL（覆盖`PHOENIX_COLLECTOR_ENDPOINT`）
-`auto_instrument`：启用自动检测（默认：False）
-`batch`：使用BatchSpanProcessor（默认值：True，生产推荐）
—`protocol`：默认为`"http/protobuf"`或`"grpc"`# # Auto-Instrumentation

为你的框架安装工具：```bash
pip install openinference-instrumentation-openai      # OpenAI SDK
pip install openinference-instrumentation-langchain   # LangChain
pip install openinference-instrumentation-llama-index # LlamaIndex
# ... install others as needed
```
然后启用自动检测：```python
register(project_name="my-app", auto_instrument=True)
```
Phoenix自动发现和检测所有已安装的OpenInference包。

批量加工（生产）

默认开启。通过环境变量配置：```bash
export OTEL_BSP_SCHEDULE_DELAY=5000           # Batch every 5s
export OTEL_BSP_MAX_QUEUE_SIZE=2048           # Queue 2048 spans
export OTEL_BSP_MAX_EXPORT_BATCH_SIZE=512     # Send 512 spans/batch
```
* *链接:* *https://opentelemetry.io/docs/specs/otel/configuration/sdk-environment-variables/# #验证

1. 打开Phoenix UI:`http://localhost:6006`2. 导航到您的项目
3. 运行应用程序
4. 检查跟踪（出现在批处理延迟内）

# #故障排除

* *没有痕迹:* *

—验证`PHOENIX_COLLECTOR_ENDPOINT`是否与Phoenix服务器匹配
—“凤凰云”设置为“`PHOENIX_API_KEY`”
—确认已安装的仪表

* *缺失属性:* *

-检查跨度类型（见rules/ directory）
-验证属性名（见rules/ directory）

# #的例子```python
from phoenix.otel import register
from openai import OpenAI

# Enable tracing with auto-instrumentation
register(project_name="my-chatbot", auto_instrument=True)

# OpenAI automatically instrumented
client = OpenAI()
response = client.chat.completions.create(
    model="gpt-4",
    messages=[{"role": "user", "content": "Hello!"}]
)
```
## API参考

- [Python OTEL API文档]（https://arize-phoenix.readthedocs.io/projects/otel/en/latest/）
- Python客户端API文档（https://arize-phoenix.readthedocs.io/projects/client/en/latest/）