# TypeScript设置

使用`@arizeai/phoenix-otel`在TypeScript/JavaScript中设置Phoenix跟踪。

# #元数据

|属性|值||-----------|-------|
|优先级| Critical -所有跟踪|都需要
|设置时间| <5 min |

##快速入门```bash
npm install @arizeai/phoenix-otel
```

```typescript
import { register } from "@arizeai/phoenix-otel";
register({ projectName: "my-app" });
```
默认连接到`http://localhost:6006`。

# #配置```typescript
import { register } from "@arizeai/phoenix-otel";

register({
  projectName: "my-app",
  url: "http://localhost:6006",
  apiKey: process.env.PHOENIX_API_KEY,
  batch: true
});
```
* *环境变量:* *```bash
export PHOENIX_API_KEY="your-api-key"
export PHOENIX_COLLECTOR_ENDPOINT="http://localhost:6006"
export PHOENIX_PROJECT_NAME="my-app"
```
ESM vs CommonJS

* * CommonJS(自动):* *```javascript
const { register } = require("@arizeai/phoenix-otel");
register({ projectName: "my-app" });

const OpenAI = require("openai");
```
**ESM（需要手动仪表）：**```typescript
import { register, registerInstrumentations } from "@arizeai/phoenix-otel";
import { OpenAIInstrumentation } from "@arizeai/openinference-instrumentation-openai";
import OpenAI from "openai";

register({ projectName: "my-app" });

const instrumentation = new OpenAIInstrumentation();
instrumentation.manuallyInstrument(OpenAI);
registerInstrumentations({ instrumentations: [instrumentation] });
```
**原因：** ESM导入是提升的，所以需要`manuallyInstrument()`。

框架集成

**Next.js（应用路由器）：**```typescript
// instrumentation.ts
export async function register() {
  if (process.env.NEXT_RUNTIME === "nodejs") {
    const { register } = await import("@arizeai/phoenix-otel");
    register({ projectName: "my-nextjs-app" });
  }
}
```
* *Express.js: * *```typescript
import { register } from "@arizeai/phoenix-otel";

register({ projectName: "my-express-app" });

const app = express();
```
##退出前的冲洗跨度

**CRITICAL:**当进程退出时，如果span仍然在处理器中排队，则不能导出。调用`provider.shutdown()`在退出前显式刷新。

标准模式:* * * *```typescript
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
* *选择:* *```typescript
// Use batch: false for immediate export (no shutdown needed)
register({
  projectName: "my-app",
  batch: false,
});
```
有关包括优美终止的生产模式，请参阅`production-typescript.md`。

# #验证

1. 打开Phoenix UI:`http://localhost:6006`2. 运行应用程序
3. 检查项目中的跟踪

**启用诊断日志：**```typescript
import { DiagLogLevel, register } from "@arizeai/phoenix-otel";

register({
  projectName: "my-app",
  diagLogLevel: DiagLogLevel.DEBUG,
});
```
# #故障排除

* *没有痕迹:* *
—验证`PHOENIX_COLLECTOR_ENDPOINT`是否正确
—“凤凰云”设置为“`PHOENIX_API_KEY`”
—ESM：确保调用了`manuallyInstrument()`- **使用`batch: true`:**在退出之前调用`provider.shutdown()`来刷新队列跨度（参见刷新跨度部分）

* *失踪的痕迹:* *
—使用`batch: true`：在进程退出之前调用`await provider.shutdown()`来刷新队列跨度
-可选：设置`batch: false`立即导出（不需要关机）

* *缺失属性:* *
-检查仪器是否注册（ESM需要手动设置）
—参见`instrumentation-auto-typescript.md`##参见Also

- **自动仪表：**`instrumentation-auto-typescript.md`- **手动仪表：**`instrumentation-manual-typescript.md`**API文档：**https://arize-ai.github.io/phoenix/