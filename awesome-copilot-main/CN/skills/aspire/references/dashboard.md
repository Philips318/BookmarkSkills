# Dashboard -完成参考

Aspire Dashboard为分布式应用程序中的所有资源提供实时可观察性。它使用`aspire run`自动启动，也可以独立运行。

---

# #特性

资源视图

显示所有资源（项目，容器，可执行文件）：

- **名称**和**类型**（项目，容器，可执行文件）
- **状态** （Starting, Running, Stopped， FailedToStart等）
- **起始时间**和**正常运行时间**
- **端点** -每个暴露端点的可点击url
- **源** -项目路径、容器镜像或可执行路径
- **操作** -停止，开始，重新启动按钮

控制台日志

从所有资源聚合原始stdout/stderr：

—按资源名称过滤
-在日志中搜索
-自动滚动与暂停
-按资源颜色编码

结构化日志

应用程序级结构化日志（通过ILogger， OpenTelemetry）：—**可按资源、日志级别、分类、消息内容进行过滤
- **可扩展** -点击查看所有属性的完整日志条目
- **关联**与跟踪-单击跳转到相关的跟踪
—支持。. NET ILogger结构化日志属性
-支持任何语言的OpenTelemetry日志信号

分布式跟踪

跨所有服务的端到端请求跟踪：

-瀑布视图-显示完整的呼叫链与时间
- **Span详细信息** - HTTP方法、URL、状态码、持续时间
- **数据库跨度** - SQL查询，连接细节
-消息传递跨越-队列操作，主题发布
- **错误高亮显示** -失败的跨度显示为红色
-跨服务关联** -跟踪上下文自动为。net传播；其他语言手册

# # #指标

实时和历史指标：- **运行时指标** - CPU，内存，GC，线程池
- **HTTP指标** -请求率，错误率，延迟百分位数
- **自定义指标** -您的服务通过OpenTelemetry发出的任何指标
- **图表** -时间序列图为每个指标

GenAI Visualizer

对于使用AI/LLM集成的应用程序：

- **令牌使用** -提示令牌，完成令牌，每次请求总令牌
- **Prompt/completion对** -查看发送的确切提示和接收的响应
- **模型元数据** -哪个模型，温度，最大令牌
- **延迟** -每次AI呼叫的时间
-要求服务通过OpenTelemetry发出[GenAI语义约定]（https://opentelemetry.io/docs/specs/semconv/gen-ai/）

---

##仪表板URL

默认情况下，仪表板在自动分配的端口上运行。找到它:

—`aspire run`启动时的终端输出
—通过MCP:`list_resources`工具
-覆盖`--dashboard-port`：```bash
aspire run --dashboard-port 18888
```
---

独立仪表板

在没有AppHost的情况下运行仪表板——这对于已经发出OpenTelemetry的现有应用程序很有用：```bash
docker run --rm -d \
  -p 18888:18888 \
  -p 4317:18889 \
  mcr.microsoft.com/dotnet/aspire-dashboard:latest
```
|端口|用途|| ---------------- | ------------------------------------------------------------ |
|`18888`|仪表板web UI |
|`4317`→`18889`| OTLP gRPC接收器（标准OTel端口→仪表盘内部）|

配置你的服务

将OpenTelemetry导出器指向仪表板：```bash
# Environment variables for any language's OpenTelemetry SDK
OTEL_EXPORTER_OTLP_ENDPOINT=http://localhost:4317
OTEL_SERVICE_NAME=my-service
```
Docker撰写示例```yaml
services:
  dashboard:
    image: mcr.microsoft.com/dotnet/aspire-dashboard:latest
    ports:
      - "18888:18888"
      - "4317:18889"

  api:
    build: ./api
    environment:
      - OTEL_EXPORTER_OTLP_ENDPOINT=http://dashboard:18889
      - OTEL_SERVICE_NAME=api

  worker:
    build: ./worker
    environment:
      - OTEL_EXPORTER_OTLP_ENDPOINT=http://dashboard:18889
      - OTEL_SERVICE_NAME=worker
```
---

##仪表板配置

# # #身份验证

独立仪表板支持通过浏览器令牌进行身份验证：```bash
docker run --rm -d \
  -p 18888:18888 \
  -p 4317:18889 \
  -e DASHBOARD__FRONTEND__AUTHMODE=BrowserToken \
  -e DASHBOARD__FRONTEND__BROWSERTOKEN__TOKEN=my-secret-token \
  mcr.microsoft.com/dotnet/aspire-dashboard:latest
```
### OTLP配置```bash
# Accept OTLP over gRPC (default)
-e DASHBOARD__OTLP__GRPC__ENDPOINT=http://0.0.0.0:18889

# Accept OTLP over HTTP
-e DASHBOARD__OTLP__HTTP__ENDPOINT=http://0.0.0.0:18890

# Require API key for OTLP
-e DASHBOARD__OTLP__AUTHMODE=ApiKey
-e DASHBOARD__OTLP__PRIMARYAPIKEY=my-api-key
```
资源限制```bash
# Limit log entries retained
-e DASHBOARD__TELEMETRYLIMITS__MAXLOGCOUNT=10000

# Limit trace entries retained
-e DASHBOARD__TELEMETRYLIMITS__MAXTRACECOUNT=10000

# Limit metric data points
-e DASHBOARD__TELEMETRYLIMITS__MAXMETRICCOUNT=50000
```
---

副驾驶集成

仪表板与GitHub Copilot集成在VS Code中：

—询问资源状态
—以自然语言查询日志和跟踪
—MCP服务器（参见[MCP服务器](mcp-server.md)）提供网桥

---

# #非。NET服务遥测

非。. NET服务要出现在仪表板中，它们必须发出OpenTelemetry信号。当使用`.WithReference()`时，Aspire会自动注入OTLP端点env变量：

Python （OpenTelemetry SDK）```python
from opentelemetry import trace
from opentelemetry.exporter.otlp.proto.grpc.trace_exporter import OTLPSpanExporter
from opentelemetry.sdk.trace import TracerProvider
from opentelemetry.sdk.trace.export import BatchSpanProcessor
import os

# Aspire injects OTEL_EXPORTER_OTLP_ENDPOINT automatically
endpoint = os.environ.get("OTEL_EXPORTER_OTLP_ENDPOINT", "http://localhost:4317")

provider = TracerProvider()
provider.add_span_processor(BatchSpanProcessor(OTLPSpanExporter(endpoint=endpoint)))
trace.set_tracer_provider(provider)
```
### JavaScript （OpenTelemetry SDK）```javascript
const { NodeTracerProvider } = require("@opentelemetry/sdk-trace-node");
const { OTLPTraceExporter } = require("@opentelemetry/exporter-trace-otlp-grpc");

const provider = new NodeTracerProvider();
provider.addSpanProcessor(
  new BatchSpanProcessor(
    new OTLPTraceExporter({
      url: process.env.OTEL_EXPORTER_OTLP_ENDPOINT || "http://localhost:4317",
    })
  )
);
provider.register();
```
