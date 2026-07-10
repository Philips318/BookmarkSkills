#使用多个会话

同时管理多个独立对话。

> **可运行示例：** [recipe/multiple-sessions.ts]（recipe/multiple-sessions.ts）
>
>“bash
> CD配方&& NPM安装
> NPX TSXmultiple-sessions.ts> #或：NPM运行多个会话
> ' ' '

示例场景

您需要并行运行多个会话，每个会话都有自己的上下文和历史。## Node.js

```typescript
import { CopilotClient, approveAll } from "@github/copilot-sdk";

const client = new CopilotClient();
await client.start();

// Create multiple independent sessions
const session1 = await client.createSession({
    onPermissionRequest: approveAll,
    model: "gpt-5",
});
const session2 = await client.createSession({
    onPermissionRequest: approveAll,
    model: "gpt-5",
});
const session3 = await client.createSession({
    onPermissionRequest: approveAll,
    model: "claude-sonnet-4.5",
});

// Each session maintains its own conversation history
await session1.sendAndWait({ prompt: "You are helping with a Python project" });
await session2.sendAndWait({ prompt: "You are helping with a TypeScript project" });
await session3.sendAndWait({ prompt: "You are helping with a Go project" });

// Follow-up messages stay in their respective contexts
await session1.sendAndWait({ prompt: "How do I create a virtual environment?" });
await session2.sendAndWait({ prompt: "How do I set up tsconfig?" });
await session3.sendAndWait({ prompt: "How do I initialize a module?" });

// Clean up all sessions
await session1.destroy();
await session2.destroy();
await session3.destroy();
await client.stop();
```
##自定义会话id

使用自定义id更容易跟踪：```typescript
const session = await client.createSession({
    onPermissionRequest: approveAll,
    sessionId: "user-123-chat",
    model: "gpt-5",
});

console.log(session.sessionId); // "user-123-chat"
```
##列出会话```typescript
const sessions = await client.listSessions();
console.log(sessions);
// [{ sessionId: "user-123-chat", ... }, ...]
```
##删除会话```typescript
// Delete a specific session
await client.deleteSession("user-123-chat");
```
##用例

- **多用户应用程序**：每个用户一个会话
- **多任务工作流**：不同任务的单独会话
- **A/B测试**：比较不同型号的响应