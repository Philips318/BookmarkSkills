#错误处理模式

在您的Copilot SDK应用程序中优雅地处理错误。

> **可运行示例：** [recipe/error_handling.py]（recipe/error_handling.py）
>
>“bash
CD recipe && PIP install -rrequirements.txt> pythonerror_handling.py> ' ' '

示例场景

您需要处理各种错误情况，如连接失败、超时和无效响应。

##基本尝试-例外```python
import asyncio
from copilot import CopilotClient, SessionConfig, MessageOptions, PermissionHandler

async def main():
    client = CopilotClient()

    try:
        await client.start()
        session = await client.create_session(SessionConfig(model="gpt-5",
        on_permission_request=PermissionHandler.approve_all))

        response = await session.send_and_wait(MessageOptions(prompt="Hello!"))

        if response:
            print(response.data.content)

        await session.destroy()
    except Exception as e:
        print(f"Error: {e}")
    finally:
        await client.stop()

if __name__ == "__main__":
    asyncio.run(main())
```
处理特定的错误类型```python
try:
    await client.start()
except FileNotFoundError:
    print("Copilot CLI not found. Please install it first.")
except ConnectionError:
    print("Could not connect to Copilot CLI server.")
except Exception as e:
    print(f"Unexpected error: {e}")
```
##超时处理```python
session = await client.create_session(SessionConfig(model="gpt-5",
        on_permission_request=PermissionHandler.approve_all))

try:
    # send_and_wait accepts an optional timeout in seconds
    response = await session.send_and_wait(
        MessageOptions(prompt="Complex question..."),
        timeout=30.0
    )
    print("Response received")
except TimeoutError:
    print("Request timed out")
```
##终止请求```python
session = await client.create_session(SessionConfig(model="gpt-5",
        on_permission_request=PermissionHandler.approve_all))

# Start a request (non-blocking send)
await session.send(MessageOptions(prompt="Write a very long story..."))

# Abort it after some condition
await asyncio.sleep(5)
await session.abort()
print("Request aborted")
```
##安全关机```python
import signal
import sys

def signal_handler(sig, frame):
    print("\nShutting down...")
    try:
        loop = asyncio.get_running_loop()
        loop.create_task(client.stop())
    except RuntimeError:
        asyncio.run(client.stop())
    sys.exit(0)

signal.signal(signal.SIGINT, signal_handler)
```
最佳实践

1. **总是清理**：使用try-finally来确保`await client.stop()`被调用
2. **处理连接错误**:CLI可能未安装或未运行
3. **设置适当的超时时间**：使用`send_and_wait()`的`timeout`参数
4. **日志错误**：捕获错误细节以便调试