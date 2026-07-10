# Auto-Instrumentation （TypeScript）

自动为LLM调用创建范围，而无需更改代码。

支持的框架

**LLM sdk:** OpenAI
- **框架：** LangChain
—**安装：**`npm install @arizeai/openinference-instrumentation-{name}`# #设置

* * CommonJS(自动):* *```javascript
const { register } = require("@arizeai/phoenix-otel");
const OpenAI = require("openai");

register({ projectName: "my-app" });

const client = new OpenAI();
```
**ESM（需要手动）：**```typescript
import { register, registerInstrumentations } from "@arizeai/phoenix-otel";
import { OpenAIInstrumentation } from "@arizeai/openinference-instrumentation-openai";
import OpenAI from "openai";

register({ projectName: "my-app" });

const instrumentation = new OpenAIInstrumentation();
instrumentation.manuallyInstrument(OpenAI);
registerInstrumentations({ instrumentations: [instrumentation] });
```
**原因：** ESM导入在`register()`运行之前被提升。

# #的局限性

**什么自动仪表不捕获：**```typescript
async function myWorkflow(query: string): Promise<string> {
  const preprocessed = await preprocess(query);        // Not traced
  const response = await client.chat.completions.create(...);  // Traced (auto)
  const postprocessed = await postprocess(response);   // Not traced
  return postprocessed;
}
```
**解决方案：**为自定义逻辑添加手动仪表：```typescript
import { traceChain } from "@arizeai/openinference-core";

const myWorkflow = traceChain(
  async (query: string): Promise<string> => {
    const preprocessed = await preprocess(query);
    const response = await client.chat.completions.create(...);
    const postprocessed = await postprocess(response);
    return postprocessed;
  },
  { name: "my-workflow" }
);
```
##自动+手动组合```typescript
import { register } from "@arizeai/phoenix-otel";
import { traceChain } from "@arizeai/openinference-core";

register({ projectName: "my-app" });

const client = new OpenAI();

const workflow = traceChain(
  async (query: string) => {
    const preprocessed = await preprocess(query);
    const response = await client.chat.completions.create(...);  // Auto-instrumented
    return postprocess(response);
  },
  { name: "my-workflow" }
);
```
