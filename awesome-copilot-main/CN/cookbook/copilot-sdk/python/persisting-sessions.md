#会话持久和恢复

跨应用程序重新启动保存和恢复会话会话。

示例场景

您希望用户能够在关闭并重新打开应用程序后继续对话。

> **可运行示例：** [recipe/persisting_sessions.py]（recipe/persisting_sessions.py）
>
>“bash
CD recipe && PIP install -rrequirements.txt> pythonpersisting_sessions.py> ' ' '

###使用自定义ID创建会话```python
import asyncio
from copilot import CopilotClient, SessionConfig, MessageOptions, PermissionHandler

async def main():
    client = CopilotClient()
    await client.start()

    # Create session with a memorable ID
    session = await client.create_session(SessionConfig(
        session_id="user-123-conversation",
        model="gpt-5",
        on_permission_request=PermissionHandler.approve_all))

    await session.send_and_wait(MessageOptions(prompt="Let's discuss TypeScript generics"))

    # Session ID is preserved
    print(session.session_id)  # "user-123-conversation"

    # Destroy session but keep data on disk
    await session.destroy()
    await client.stop()

if __name__ == "__main__":
    asyncio.run(main())
```
###恢复会话```python
client = CopilotClient()
await client.start()

# Resume the previous session
session = await client.resume_session("user-123-conversation", on_permission_request=PermissionHandler.approve_all)

# Previous context is restored
await session.send_and_wait(MessageOptions(prompt="What were we discussing?"))

await session.destroy()
await client.stop()
```
列出可用的会话```python
sessions = await client.list_sessions()
for s in sessions:
    print("Session:", s.session_id)
```
永久删除会话```python
# Remove session and all its data from disk
await client.delete_session("user-123-conversation")
```
获取会话历史记录```python
messages = await session.get_messages()
for msg in messages:
    print(f"[{msg.type}] {msg.data.content}")
```
最佳实践

1. **使用有意义的会话ID **：在会话ID中包含用户ID或上下文
2. **处理丢失的会话**：在恢复之前检查会话是否存在
3. **清理旧会话**：定期删除不再需要的会话