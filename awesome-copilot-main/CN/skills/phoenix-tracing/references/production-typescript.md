# Phoenix Tracing：制作指南（TypeScript）

**CRITICAL：为生产部署配置批处理、数据屏蔽和跨度过滤

# #元数据

|属性|值||-----------|-------|
|优先级|关键-生产就绪|
|影响|安全、性能|
设置时间| 5-15分钟|

批处理

**支持批量加工，提高生产效率。批处理通过分组而不是单独发送跨度来减少网络开销。```typescript
import { register } from "@arizeai/phoenix-otel";

const provider = register({
  projectName: "my-app",
  batch: true,  // Production default
});
```
###关机处理

**CRITICAL:**当进程退出时，如果span仍然在处理器中排队，则不能导出。调用`provider.shutdown()`在退出前显式刷新。```typescript
// Explicit shutdown to flush queued spans
const provider = register({
  projectName: "my-app",
  batch: true,
});

async function main() {
  await doWork();
  await provider.shutdown();  // Flush spans before exit
}

main().catch(async (error) => {
  console.error(error);
  await provider.shutdown();  // Flush on error too
  process.exit(1);
});
```
**优美的终止信号：**```typescript
// Graceful shutdown on SIGTERM
const provider = register({
  projectName: "my-server",
  batch: true,
});

process.on("SIGTERM", async () => {
  await provider.shutdown();
  process.exit(0);
});
```
---

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
* *打印稿TraceConfig: * *```typescript
import { register } from "@arizeai/phoenix-otel";
import { OpenAIInstrumentation } from "@arizeai/openinference-instrumentation-openai";

const traceConfig = {
  hideInputs: true,
  hideOutputs: true,
  hideInputMessages: true
};

const instrumentation = new OpenAIInstrumentation({ traceConfig });
```
**优先级：**代码>环境变量>默认值

---

## Span过滤

**抑制特定代码块：**```typescript
import { suppressTracing } from "@opentelemetry/core";
import { context } from "@opentelemetry/api";

await context.with(suppressTracing(context.active()), async () => {
  internalLogging(); // No spans generated
});
```
抽样:* * * *```bash
export OTEL_TRACES_SAMPLER="parentbased_traceidratio"
export OTEL_TRACES_SAMPLER_ARG="0.1"  # Sample 10%
```
---

##错误处理```typescript
import { SpanStatusCode } from "@opentelemetry/api";

try {
  result = await riskyOperation();
  span?.setStatus({ code: SpanStatusCode.OK });
} catch (e) {
  span?.recordException(e);
  span?.setStatus({ code: SpanStatusCode.ERROR });
  throw e;
}
```
---

##生产清单

-[]启用批处理
-[] **关闭处理：**在退出之前调用`provider.shutdown()`来刷新队列跨度
-[] **安全终止：** Flush跨越SIGTERM/SIGINT信号
-[]配置数据屏蔽（`HIDE_INPUTS`/`HIDE_OUTPUTS`if PII）
—[]健康checks/noisy路径的跨度过滤
-[]实现错误处理
-[]凤凰不可用时的优雅降级
-[]性能测试
-[]设置监控（检查了Phoenix UI）