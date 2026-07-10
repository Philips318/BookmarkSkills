# Sessions （TypeScript）

通过使用会话id分组跟踪来跟踪多回合会话。**直接从`@arizeai/openinference-core`**使用`withSpan`-不需要包装器或自定义实用程序。

##核心概念

会话模式:* * * *
1. 在应用程序启动时生成唯一的`session.id`2. 导出SESSION_ID，在需要的地方导入`withSpan`3. 使用`withSpan`为每个交互使用`session.id`创建父CHAIN span
4. 所有子跨（LLM、TOOL、AGENT等）自动分组在父跨下
5. 在Phoenix中通过`session.id`查询跟踪以查看所有交互

##实现（最佳实践）

# # # 1。设置(instrumentation.ts)```typescript
import { register } from "@arizeai/phoenix-otel";
import { randomUUID } from "node:crypto";

// Initialize Phoenix
register({
  projectName: "your-app",
  url: process.env.PHOENIX_COLLECTOR_ENDPOINT || "http://localhost:6006",
  apiKey: process.env.PHOENIX_API_KEY,
  batch: true,
});

// Generate and export session ID
export const SESSION_ID = randomUUID();
```
# # # 2。用法（应用代码）```typescript
import { withSpan } from "@arizeai/openinference-core";
import { SESSION_ID } from "./instrumentation";

// Use withSpan directly - no wrapper needed
const handleInteraction = withSpan(
  async () => {
    const result = await agent.generate({ prompt: userInput });
    return result;
  },
  {
    name: "cli.interaction",
    kind: "CHAIN",
    attributes: { "session.id": SESSION_ID },
  }
);

// Call it
const result = await handleInteraction();
```
输入参数```typescript
const processQuery = withSpan(
  async (query: string) => {
    return await agent.generate({ prompt: query });
  },
  {
    name: "process.query",
    kind: "CHAIN",
    attributes: { "session.id": SESSION_ID },
  }
);

await processQuery("What is 2+2?");
```
##要点

###会话ID范围
- **CLI/DesktopApps**：在进程启动时生成一次
- **Web服务器**：生成每个用户会话（例如，在登录时，存储在会话存储中）
—**无状态api **：接受会话。Id作为来自客户端的参数

### Span层次```
cli.interaction (CHAIN) ← session.id here
├── ai.generateText (AGENT)
│   ├── ai.generateText.doGenerate (LLM)
│   └── ai.toolCall (TOOL)
└── ai.generateText.doGenerate (LLM)
```
`session.id`仅设置在**根跨度**上。子跨度根据跟踪层次结构自动分组。

查询会话```bash
# Get all traces for a session
npx @arizeai/phoenix-cli traces \
  --endpoint http://localhost:6006 \
  --project your-app \
  --format raw \
  --no-progress | \
  jq '.[] | select(.spans[0].attributes["session.id"] == "YOUR-SESSION-ID")'
```
# #依赖性```json
{
  "dependencies": {
    "@arizeai/openinference-core": "^2.0.5",
    "@arizeai/phoenix-otel": "^0.4.1"
  }
}
```
**注意：**`@opentelemetry/api`是不需要的-它只用于手动跨度管理。

为什么是这种模式？

1. **简单**：只导出SESSION_ID，直接使用withSpan -没有包装
2. **内置**:`withSpan`从`@arizeai/openinference-core`处理一切
3. 类型安全：保留函数签名和类型信息
4. **自动生命周期**：处理跨度创建，错误跟踪和清理
5. **框架无关**：适用于任何LLM框架（AI SDK， LangChain等）
6. **不需要额外的深度**：不需要`@opentelemetry/api`或自定义实用程序

##添加更多属性```typescript
import { withSpan } from "@arizeai/openinference-core";
import { SESSION_ID } from "./instrumentation";

const handleWithContext = withSpan(
  async (userInput: string) => {
    return await agent.generate({ prompt: userInput });
  },
  {
    name: "cli.interaction",
    kind: "CHAIN",
    attributes: {
      "session.id": SESSION_ID,
      "user.id": userId,              // Track user
      "metadata.environment": "prod",  // Custom metadata
    },
  }
);
```
反模式：不要创建包装器

❌**不要这样做：**```typescript
// Unnecessary wrapper
export function withSessionTracking(fn) {
  return withSpan(fn, { attributes: { "session.id": SESSION_ID } });
}
```
✅**这样做：**```typescript
// Use withSpan directly
import { withSpan } from "@arizeai/openinference-core";
import { SESSION_ID } from "./instrumentation";

const handler = withSpan(fn, {
  attributes: { "session.id": SESSION_ID }
});
```
可选方案：上下文API模式

对于web服务器或复杂的异步流，你需要通过中间件传播会话id，你可以使用Context API：```typescript
import { context } from "@opentelemetry/api";
import { setSession } from "@arizeai/openinference-core";

await context.with(
  setSession(context.active(), { sessionId: "user_123_conv_456" }),
  async () => {
    const response = await llm.invoke(prompt);
  }
);
```
**在以下情况下使用上下文API
-使用中间件链构建web服务器
—会话ID需要流经多个异步边界
你不能控制调用栈（例如，框架提供的处理程序）

**当：**时使用withSpan
-构建CLI应用程序或脚本
-控制函数调用点
-更简单、更显式的代码是首选

# #相关

—`fundamentals-universal-attributes.md`—其他通用属性(user。id、元数据)
-`span-chain.md`- CHAIN跨度规格
-`sessions-python.md`- Python会话跟踪模式