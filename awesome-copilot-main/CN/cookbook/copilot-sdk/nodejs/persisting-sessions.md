#会话持久和恢复

跨应用程序重新启动保存和恢复会话会话。

示例场景

您希望用户能够在关闭并重新打开应用程序后继续对话。

> **可运行示例：** [recipe/persisting-sessions.ts]（recipe/persisting-sessions.ts）
>
>“bash
> CD配方&& NPM安装
> NPX TSXpersisting-sessions.ts> #或：NPM运行持久化会话
> ' ' '

###使用自定义ID创建会话```typescript
import { CopilotClient, approveAll } from "@github/copilot-sdk";

const client = new CopilotClient();
await client.start();

// Create session with a memorable ID
const session = await client.createSession({
    onPermissionRequest: approveAll,
    sessionId: "user-123-conversation",
    model: "gpt-5",
});

await session.sendAndWait({ prompt: "Let's discuss TypeScript generics" });

// Session ID is preserved
console.log(session.sessionId); // "user-123-conversation"

// Destroy session but keep data on disk
await session.destroy();
await client.stop();
```
###恢复会话```typescript
const client = new CopilotClient();
await client.start();

// Resume the previous session
const session = await client.resumeSession("user-123-conversation", { onPermissionRequest: approveAll });

// Previous context is restored
await session.sendAndWait({ prompt: "What were we discussing?" });
// AI remembers the TypeScript generics discussion

await session.destroy();
await client.stop();
```
列出可用的会话```typescript
const sessions = await client.listSessions();
console.log(sessions);
// [
//   { sessionId: "user-123-conversation", ... },
//   { sessionId: "user-456-conversation", ... },
// ]
```
永久删除会话```typescript
// Remove session and all its data from disk
await client.deleteSession("user-123-conversation");
```
获取会话历史记录

检索会话中的所有消息：```typescript
const messages = await session.getMessages();
for (const msg of messages) {
    console.log(`[${msg.type}]`, msg.data);
}
```
最佳实践

1. **使用有意义的会话ID **：在会话ID中包含用户ID或上下文
2. **处理丢失的会话**：在恢复之前检查会话是否存在
3. **清理旧会话**：定期删除不再需要的会话