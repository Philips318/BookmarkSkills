---
applyTo: "**.ts, **.js, package.json"
description: "This file provides guidance on building Node.js/TypeScript applications using GitHub Copilot SDK."
name: "GitHub Copilot SDK Node.js Instructions"
---
##核心原则

- SDK处于技术预览阶段，可能会有突破性的变化
—要求Node.js18.0及以上版本
-需要安装GitHub CopilotCLI，并放在PATH中
-使用TypeScript构建，用于类型安全
-使用async/await模式贯穿始终
提供完整的TypeScript类型定义

# #安装

总是通过npm/pnpm/yarn安装：```bash
npm install @github/copilot-sdk
# or
pnpm add @github/copilot-sdk
# or
yarn add @github/copilot-sdk
```
##客户端初始化

基本客户端设置```typescript
import { CopilotClient, approveAll } from "@github/copilot-sdk";

const client = new CopilotClient();
await client.start();
// Use client...
await client.stop();
```
客户端配置选项

当创建一个CopilotClient时，使用`CopilotClientOptions`：

-`cliPath`- CLI可执行文件的路径（默认为Path中的“copilot”）
-`cliArgs`-在sdk管理的标志（string[]）之前附加额外的参数
-`cliUrl`-已存在的CLI服务器的URL（例如localhost:8080）。当提供时，客户端不会生成进程
-`port`-服务器端口（随机默认为0）
-`useStdio`-使用stdio传输而不是TCP（默认值：true）
-`logLevel`-日志级别（默认为“debug”）
-`autoStart`-自动启动服务器（默认为true）
-`autoRestart`-崩溃时自动重启（默认为true）
—`cwd`—CLI进程的工作目录（默认：process.cwd()）
—`env`—CLI进程的环境变量，默认为process.env。

手动服务器控制

对于显式控制：```typescript
const client = new CopilotClient({ autoStart: false });
await client.start();
// Use client...
await client.stop();
```
当`stop()`耗时太长时，请使用`forceStop()`。

##会话管理

创建会话

使用`SessionConfig`进行配置：```typescript
const session = await client.createSession({
    onPermissionRequest: approveAll,
    model: "gpt-5",
    streaming: true,
    tools: [...],
    systemMessage: { ... },
    availableTools: ["tool1", "tool2"],
    excludedTools: ["tool3"],
    provider: { ... }
});
```
会话配置选项

-`sessionId`-自定义会话ID（字符串）
-`model`-模型名称（"gpt-5", “claude-sonnet-4.5”等）
-`tools`-自定义工具暴露于CLI （Tool[]）
-`systemMessage`-系统消息定制（SystemMessageConfig）
-`availableTools`-允许工具名称列表（字符串[]）
-`excludedTools`-工具名称列表（字符串[]）
-`provider`-自定义API提供程序配置（BYOK）
-`streaming`-启用流响应块（boolean）
-`mcpServers`- MCP服务器配置（MCPServerConfig[]）`customAgents`-自定义代理配置（CustomAgentConfig[]）
-`configDir`-配置目录覆盖（字符串）
-`skillDirectories`-技能目录（字符串[]）
-`disabledSkills`-禁用技能（字符串[]）
-`onPermissionRequest`-权限请求处理程序（PermissionHandler）

###恢复会话```typescript
const session = await client.resumeSession("session-id", {
  tools: [myNewTool],
  onPermissionRequest: approveAll,
});
```
会话操作

-`session.sessionId`-获取会话标识符（字符串）
-`await session.send({ prompt: "...", attachments: [...] })`-发送消息，返回Promise<string>-`await session.sendAndWait({ prompt: "..." }, timeout)`-发送并等待空闲，返回Promise<AssistantMessageEvent | null>
-`await session.abort()`-中止当前处理
-`await session.getMessages()`-获取所有events/messages，返回Promise<SessionEvent[]>-`await session.destroy()`-清理会话

##事件处理

事件订阅模式

总是使用async/await或Promises来等待会话事件：```typescript
await new Promise<void>((resolve) => {
  session.on((event) => {
    if (event.type === "assistant.message") {
      console.log(event.data.content);
    } else if (event.type === "session.idle") {
      resolve();
    }
  });

  session.send({ prompt: "..." });
});
```
###取消订阅事件`on()`方法返回一个取消订阅的函数：```typescript
const unsubscribe = session.on((event) => {
  // handler
});
// Later...
unsubscribe();
```
事件类型

使用带有类型保护的区分联合进行事件处理：```typescript
session.on((event) => {
  switch (event.type) {
    case "user.message":
      // Handle user message
      break;
    case "assistant.message":
      console.log(event.data.content);
      break;
    case "tool.executionStart":
      // Tool execution started
      break;
    case "tool.executionComplete":
      // Tool execution completed
      break;
    case "session.start":
      // Session started
      break;
    case "session.idle":
      // Session is idle (processing complete)
      break;
    case "session.error":
      console.error(`Error: ${event.data.message}`);
      break;
  }
});
```
##流式响应

###启用流

在SessionConfig中设置`streaming: true`：```typescript
const session = await client.createSession({
    onPermissionRequest: approveAll,
    model: "gpt-5",
    streaming: true,
});
```
处理流事件

处理增量事件和最终事件：```typescript
await new Promise<void>((resolve) => {
  session.on((event) => {
    switch (event.type) {
      case "assistant.message_delta":
        // Incremental text chunk
        process.stdout.write(event.data.deltaContent);
        break;
      case "assistant.reasoning_delta":
        // Incremental reasoning chunk (model-dependent)
        process.stdout.write(event.data.deltaContent);
        break;
      case "assistant.message":
        // Final complete message
        console.log("\n--- Final ---");
        console.log(event.data.content);
        break;
      case "assistant.reasoning":
        // Final reasoning content
        console.log("--- Reasoning ---");
        console.log(event.data.content);
        break;
      case "session.idle":
        resolve();
        break;
    }
  });

  session.send({ prompt: "Tell me a story" });
});
```
注意：无论流设置如何，最终事件（`assistant.message`,`assistant.reasoning`）总是被发送。

##自定义工具

使用defineTool定义工具

使用`defineTool`定义类型安全的工具：```typescript
import { defineTool } from "@github/copilot-sdk";

const session = await client.createSession({
    onPermissionRequest: approveAll,
    model: "gpt-5",
  tools: [
    defineTool({
      name: "lookup_issue",
      description: "Fetch issue details from tracker",
      parameters: {
        type: "object",
        properties: {
          id: { type: "string", description: "Issue ID" },
        },
        required: ["id"],
      },
      handler: async (args) => {
        const issue = await fetchIssue(args.id);
        return issue;
      },
    }),
  ],
});
```
###使用Zod作为参数

SDK支持Zod模式的参数：```typescript
import { z } from "zod";

const session = await client.createSession({
    onPermissionRequest: approveAll,
  tools: [
    defineTool({
      name: "get_weather",
      description: "Get weather for a location",
      parameters: z.object({
        location: z.string().describe("City name"),
        units: z.enum(["celsius", "fahrenheit"]).optional(),
      }),
      handler: async (args) => {
        return { temperature: 72, units: args.units || "fahrenheit" };
      },
    }),
  ],
});
```
工具返回类型

-返回任何json可序列化的值（自动包装）
-或者返回`ToolResultObject`以完全控制元数据：```typescript
{
    textResultForLlm: string;  // Result shown to LLM
    resultType: "success" | "failure";
    error?: string;  // Internal error (not shown to LLM)
    toolTelemetry?: Record<string, unknown>;
}
```
工具执行流

当Copilot调用一个工具时，客户端会自动：

1. 运行处理程序函数
2. 序列化返回值
3. 响应CLI

系统消息定制

附加模式（默认-保留护栏）```typescript
const session = await client.createSession({
    onPermissionRequest: approveAll,
    model: "gpt-5",
  systemMessage: {
    mode: "append",
    content: `
<workflow_rules>
- Always check for security vulnerabilities
- Suggest performance improvements when applicable
</workflow_rules>
`,
  },
});
```
替换模式（完全控制-移除护栏）```typescript
const session = await client.createSession({
    onPermissionRequest: approveAll,
    model: "gpt-5",
  systemMessage: {
    mode: "replace",
    content: "You are a helpful assistant.",
  },
});
```
##文件附件

将文件附加到消息：```typescript
await session.send({
  prompt: "Analyze this file",
  attachments: [
    {
      type: "file",
      path: "/path/to/file.ts",
      displayName: "My File",
    },
  ],
});
```
##消息传递模式

在消息选项中使用`mode`属性：

-`"enqueue"`-等待处理的消息队列
-`"immediate"`-立即处理消息```typescript
await session.send({
  prompt: "...",
  mode: "enqueue",
});
```
##多会话

会话是独立的，可以并发运行：```typescript
const session1 = await client.createSession({
    onPermissionRequest: approveAll,
    model: "gpt-5",
});
const session2 = await client.createSession({
    onPermissionRequest: approveAll,
    model: "claude-sonnet-4.5",
});

await Promise.all([
  session1.send({ prompt: "Hello from session 1" }),
  session2.send({ prompt: "Hello from session 2" }),
]);
```
自带钥匙（BYOK）

通过`provider`使用自定义API提供商：```typescript
const session = await client.createSession({
    onPermissionRequest: approveAll,
  provider: {
    type: "openai",
    baseUrl: "https://api.openai.com/v1",
    apiKey: "your-api-key",
  },
});
```
会话生命周期管理

列出会话```typescript
const sessions = await client.listSessions();
for (const metadata of sessions) {
  console.log(`${metadata.sessionId}: ${metadata.summary}`);
}
```
删除会话```typescript
await client.deleteSession(sessionId);
```
获取最后一个会话ID```typescript
const lastId = await client.getLastSessionId();
if (lastId) {
  const session = await client.resumeSession(lastId, { onPermissionRequest: approveAll });
}
```
检查连接状态```typescript
const state = client.getState();
// Returns: "disconnected" | "connecting" | "connected" | "error"
```
##错误处理

标准异常处理```typescript
try {
  const session = await client.createSession({ onPermissionRequest: approveAll });
  await session.send({ prompt: "Hello" });
} catch (error) {
  console.error(`Error: ${error.message}`);
}
```
会话错误事件

监视`session.error`事件类型以查看运行时错误：```typescript
session.on((event) => {
  if (event.type === "session.error") {
    console.error(`Session Error: ${event.data.message}`);
  }
});
```
##连接测试

使用ping来验证服务器连接：```typescript
const response = await client.ping("health check");
console.log(`Server responded at ${new Date(response.timestamp)}`);
```
##资源清理

自动清理与Try-Finally

总是在finally块中使用try-finally或cleanup：```typescript
const client = new CopilotClient();
try {
  await client.start();
  const session = await client.createSession({ onPermissionRequest: approveAll });
  try {
    // Use session...
  } finally {
    await session.destroy();
  }
} finally {
  await client.stop();
}
```
清理函数模式```typescript
async function withClient<T>(
  fn: (client: CopilotClient) => Promise<T>,
): Promise<T> {
  const client = new CopilotClient();
  try {
    await client.start();
    return await fn(client);
  } finally {
    await client.stop();
  }
}

async function withSession<T>(
  client: CopilotClient,
  fn: (session: CopilotSession) => Promise<T>,
): Promise<T> {
  const session = await client.createSession({ onPermissionRequest: approveAll });
  try {
    return await fn(session);
  } finally {
    await session.destroy();
  }
}

// Usage
await withClient(async (client) => {
  await withSession(client, async (session) => {
    await session.send({ prompt: "Hello!" });
  });
});
```
最佳实践

1. **总是使用try-finally**来清理资源
2. **使用Promises来等待会话。空闲事件
3. * *处理会话。Error **事件用于健壮的错误处理
4. **使用类型保护或switch语句**进行事件处理
5. **在交互场景中启用流**以获得更好的用户体验
6. **使用defineTool**定义类型安全的工具
7. **使用Zod模式**进行运行时参数验证
8. 当不再需要时，处理事件订阅
9. **使用systemMessage模式："append"**来保护安全护栏
10. **处理增量和最终事件**时，流是启用的
11. **利用TypeScript类型**来保证编译时的安全

##常见模式

简单查询-响应```typescript
import { CopilotClient, approveAll } from "@github/copilot-sdk";

const client = new CopilotClient();
try {
  await client.start();

  const session = await client.createSession({
    onPermissionRequest: approveAll,
    model: "gpt-5",
  });
  try {
    await new Promise<void>((resolve) => {
      session.on((event) => {
        if (event.type === "assistant.message") {
          console.log(event.data.content);
        } else if (event.type === "session.idle") {
          resolve();
        }
      });

      session.send({ prompt: "What is 2+2?" });
    });
  } finally {
    await session.destroy();
  }
} finally {
  await client.stop();
}
```
多回合对话```typescript
const session = await client.createSession({ onPermissionRequest: approveAll });

async function sendAndWait(prompt: string): Promise<void> {
  await new Promise<void>((resolve, reject) => {
    const unsubscribe = session.on((event) => {
      if (event.type === "assistant.message") {
        console.log(event.data.content);
      } else if (event.type === "session.idle") {
        unsubscribe();
        resolve();
      } else if (event.type === "session.error") {
        unsubscribe();
        reject(new Error(event.data.message));
      }
    });

    session.send({ prompt });
  });
}

await sendAndWait("What is the capital of France?");
await sendAndWait("What is its population?");
```
SendAndWait Helper```typescript
// Use built-in sendAndWait for simpler synchronous interaction
const response = await session.sendAndWait({ prompt: "What is 2+2?" }, 60000);

if (response) {
  console.log(response.data.content);
}
```
带有类型安全参数的工具```typescript
import { z } from "zod";
import { defineTool } from "@github/copilot-sdk";

interface UserInfo {
  id: string;
  name: string;
  email: string;
  role: string;
}

const session = await client.createSession({
    onPermissionRequest: approveAll,
  tools: [
    defineTool({
      name: "get_user",
      description: "Retrieve user information",
      parameters: z.object({
        userId: z.string().describe("User ID"),
      }),
      handler: async (args): Promise<UserInfo> => {
        return {
          id: args.userId,
          name: "John Doe",
          email: "john@example.com",
          role: "Developer",
        };
      },
    }),
  ],
});
```
###有进展的流媒体```typescript
let currentMessage = "";

const unsubscribe = session.on((event) => {
  if (event.type === "assistant.message_delta") {
    currentMessage += event.data.deltaContent;
    process.stdout.write(event.data.deltaContent);
  } else if (event.type === "assistant.message") {
    console.log("\n\n=== Complete ===");
    console.log(`Total length: ${event.data.content.length} chars`);
  } else if (event.type === "session.idle") {
    unsubscribe();
  }
});

await session.send({ prompt: "Write a long story" });
```
错误恢复```typescript
session.on((event) => {
  if (event.type === "session.error") {
    console.error("Session error:", event.data.message);
    // Optionally retry or handle error
  }
});

try {
  await session.send({ prompt: "risky operation" });
} catch (error) {
  // Handle send errors
  console.error("Failed to send:", error);
}
```
typescript特有的特性

类型推断```typescript
import type { SessionEvent, AssistantMessageEvent } from "@github/copilot-sdk";

session.on((event: SessionEvent) => {
  if (event.type === "assistant.message") {
    // TypeScript knows event is AssistantMessageEvent here
    const content: string = event.data.content;
  }
});
```
###通用Helper```typescript
async function waitForEvent<T extends SessionEvent["type"]>(
  session: CopilotSession,
  eventType: T,
): Promise<Extract<SessionEvent, { type: T }>> {
  return new Promise((resolve) => {
    const unsubscribe = session.on((event) => {
      if (event.type === eventType) {
        unsubscribe();
        resolve(event as Extract<SessionEvent, { type: T }>);
      }
    });
  });
}

// Usage
const message = await waitForEvent(session, "assistant.message");
console.log(message.data.content);
```
