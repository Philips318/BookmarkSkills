---
applyTo: "**.py, pyproject.toml, setup.py"
description: "This file provides guidance on building Python applications using GitHub Copilot SDK."
name: "GitHub Copilot SDK Python Instructions"
---
##核心原则

- SDK处于技术预览阶段，可能会有突破性的变化
-需要Python 3.9或更高版本
—需要安装GitHub CopilotCLI，并放在PATH中
-使用async/await模式（asyncio）
-支持异步上下文管理器和手动生命周期管理
-为更好的IDE支持提供类型提示

# #安装

总是通过pip安装：```bash
pip install github-copilot-sdk
# or with poetry
poetry add github-copilot-sdk
# or with uv
uv add github-copilot-sdk
```
##客户端初始化

基本客户端设置```python
from copilot import CopilotClient, PermissionHandler
import asyncio

async def main():
    async with CopilotClient() as client:
        # Use client...
        pass

asyncio.run(main())
```
客户端配置选项

当创建一个CopilotClient时，使用一个带有这些键的字典：

-`cli_path`-命令行可执行路径（默认为“copilot”from Path或COPILOT_CLI_PATH env var）
—`cli_url`—已存在的CLI服务器的URL（例如localhost:8080）。当提供时，客户端不会生成进程
-`port`-服务器端口（随机默认为0）
-`use_stdio`-使用stdio传输而不是TCP（默认值：True）
-`log_level`-日志级别（默认为“info”）
-`auto_start`-自动启动服务器（默认为True）
-`auto_restart`-崩溃时自动重启（默认为True）
—`cwd`—CLI进程工作目录（默认：os.getcwd()）
—`env`—CLI进程的环境变量（dict）

手动服务器控制

对于显式控制：```python
from copilot import CopilotClient
import asyncio

async def main():
    client = CopilotClient({"auto_start": False})
    await client.start()
    # Use client...
    await client.stop()

asyncio.run(main())
```
当`stop()`耗时太长时，请使用`force_stop()`。

##会话管理

创建会话

在SessionConfig中使用字典：```python
session = await client.create_session({
    "on_permission_request": PermissionHandler.approve_all,
    "model": "gpt-5",
    "streaming": True,
    "tools": [...],
    "system_message": { ... },
    "available_tools": ["tool1", "tool2"],
    "excluded_tools": ["tool3"],
    "provider": { ... }
})
```
会话配置选项

-`session_id`-自定义会话ID （str）
-`model`-模型名称（"gpt-5", “claude-sonnet-4.5”等）
-`tools`-自定义工具暴露给CLI （list[Tool]）
-`system_message`-系统消息定制（dict）
-`available_tools`-允许列表的工具名称（list[str]）
-`excluded_tools`-工具名称阻塞列表（list[str]）
-`provider`-自定义API提供程序配置（BYOK）
-`streaming`-启用流响应块（bool）
-`mcp_servers`- MCP服务器配置（list）`custom_agents`-自定义代理配置（列表）
-`config_dir`配置目录覆盖（str）`skill_directories`-技能目录（list[str]）
-`disabled_skills`-禁用技能（list[str]）
-`on_permission_request`-权限请求处理程序（可调用）

###恢复会话```python
session = await client.resume_session("session-id", {
    "on_permission_request": PermissionHandler.approve_all,
    "tools": [my_new_tool]
})
```
会话操作

-`session.session_id`-获取会话标识符（str）`await session.send({"prompt": "...", "attachments": [...]})`-发送消息，返回str（消息ID）
—`await session.send_and_wait({"prompt": "..."}, timeout=60.0)`—发送并等待空闲，返回SessionEvent |无
-`await session.abort()`-中止当前处理
-获取所有events/messages，返回列表[SessionEvent]
-`await session.destroy()`-清理会话

##事件处理

事件订阅模式

总是使用asyncio事件或futures来等待会话事件：```python
import asyncio

done = asyncio.Event()

def handler(event):
    if event.type == "assistant.message":
        print(event.data.content)
    elif event.type == "session.idle":
        done.set()

session.on(handler)
await session.send({"prompt": "..."})
await done.wait()
```
###取消订阅事件`on()`方法返回一个取消订阅的函数：```python
unsubscribe = session.on(lambda event: print(event.type))
# Later...
unsubscribe()
```
事件类型

使用属性访问事件类型检查：```python
def handler(event):
    if event.type == "user.message":
        # Handle user message
        pass
    elif event.type == "assistant.message":
        print(event.data.content)
    elif event.type == "tool.executionStart":
        # Tool execution started
        pass
    elif event.type == "tool.executionComplete":
        # Tool execution completed
        pass
    elif event.type == "session.start":
        # Session started
        pass
    elif event.type == "session.idle":
        # Session is idle (processing complete)
        pass
    elif event.type == "session.error":
        print(f"Error: {event.data.message}")

session.on(handler)
```
##流式响应

###启用流

在SessionConfig中设置`streaming: True`：```python
session = await client.create_session({
    "on_permission_request": PermissionHandler.approve_all,
    "model": "gpt-5",
    "streaming": True
})
```
处理流事件

处理增量事件和最终事件：```python
import asyncio

done = asyncio.Event()

def handler(event):
    if event.type == "assistant.message.delta":
        # Incremental text chunk
        print(event.data.delta_content, end="", flush=True)
    elif event.type == "assistant.reasoning.delta":
        # Incremental reasoning chunk (model-dependent)
        print(event.data.delta_content, end="", flush=True)
    elif event.type == "assistant.message":
        # Final complete message
        print("\n--- Final ---")
        print(event.data.content)
    elif event.type == "assistant.reasoning":
        # Final reasoning content
        print("--- Reasoning ---")
        print(event.data.content)
    elif event.type == "session.idle":
        done.set()

session.on(handler)
await session.send({"prompt": "Tell me a story"})
await done.wait()
```
注意：无论流设置如何，最终事件（`assistant.message`,`assistant.reasoning`）总是被发送。

##自定义工具

使用define_tool定义工具

使用`define_tool`作为工具定义：```python
from copilot import define_tool

async def fetch_issue(issue_id: str):
    # Fetch issue from tracker
    return {"id": issue_id, "status": "open"}

session = await client.create_session({
    "on_permission_request": PermissionHandler.approve_all,
    "model": "gpt-5",
    "tools": [
        define_tool(
            name="lookup_issue",
            description="Fetch issue details from tracker",
            parameters={
                "type": "object",
                "properties": {
                    "id": {"type": "string", "description": "Issue ID"}
                },
                "required": ["id"]
            },
            handler=lambda args, inv: fetch_issue(args["id"])
        )
    ]
})
```
使用Pydantic作为参数

SDK可以很好地与Pydantic模型一起工作：```python
from pydantic import BaseModel, Field

class WeatherArgs(BaseModel):
    location: str = Field(description="City name")
    units: str = Field(default="fahrenheit", description="Temperature units")

async def get_weather(args: WeatherArgs, inv):
    return {"temperature": 72, "units": args.units}

session = await client.create_session({
    "on_permission_request": PermissionHandler.approve_all,
    "tools": [
        define_tool(
            name="get_weather",
            description="Get weather for a location",
            parameters=WeatherArgs.model_json_schema(),
            handler=lambda args, inv: get_weather(WeatherArgs(**args), inv)
        )
    ]
})
```
工具返回类型

-返回任何json可序列化的值（自动包装）
-或返回ToolResult字典的完全控制：```python
{
    "text_result_for_llm": str,  # Result shown to LLM
    "result_type": "success" | "failure",
    "error": str,  # Optional: Internal error (not shown to LLM)
    "tool_telemetry": dict  # Optional: Telemetry data
}
```
工具处理程序签名

工具处理程序接收两个参数：

-`args`(dict) - LLM传递的工具参数
—`invocation`（ToolInvocation）—关于调用的元数据
-`invocation.session_id`-会话ID
-`invocation.tool_call_id`-工具呼叫ID
—`invocation.tool_name`—工具名称
—`invocation.arguments`—与参数args相同

工具执行流

当Copilot调用一个工具时，客户端会自动：

1. 运行处理程序函数
2. 序列化返回值
3. 响应CLI

系统消息定制

附加模式（默认-保留护栏）```python
session = await client.create_session({
    "on_permission_request": PermissionHandler.approve_all,
    "model": "gpt-5",
    "system_message": {
        "mode": "append",
        "content": """
<workflow_rules>
- Always check for security vulnerabilities
- Suggest performance improvements when applicable
</workflow_rules>
"""
    }
})
```
替换模式（完全控制-移除护栏）```python
session = await client.create_session({
    "on_permission_request": PermissionHandler.approve_all,
    "model": "gpt-5",
    "system_message": {
        "mode": "replace",
        "content": "You are a helpful assistant."
    }
})
```
##文件附件

将文件附加到消息：```python
await session.send({
    "prompt": "Analyze this file",
    "attachments": [
        {
            "type": "file",
            "path": "/path/to/file.py",
            "display_name": "My File"
        }
    ]
})
```
##消息传递模式

在消息选项中使用`mode`键：

-`"enqueue"`-等待处理的消息队列
-`"immediate"`-立即处理消息```python
await session.send({
    "prompt": "...",
    "mode": "enqueue"
})
```
##多会话

会话是独立的，可以并发运行：```python
session1 = await client.create_session({
    "on_permission_request": PermissionHandler.approve_all,
    "model": "gpt-5",
})
session2 = await client.create_session({
    "on_permission_request": PermissionHandler.approve_all,
    "model": "claude-sonnet-4.5",
})

await asyncio.gather(
    session1.send({"prompt": "Hello from session 1"}),
    session2.send({"prompt": "Hello from session 2"})
)
```
自带钥匙（BYOK）

通过`provider`使用自定义API提供商：```python
session = await client.create_session({
    "on_permission_request": PermissionHandler.approve_all,
    "provider": {
        "type": "openai",
        "base_url": "https://api.openai.com/v1",
        "api_key": "your-api-key"
    }
})
```
会话生命周期管理

列出会话```python
sessions = await client.list_sessions()
for metadata in sessions:
    print(f"{metadata.session_id}: {metadata.summary}")
```
删除会话```python
await client.delete_session(session_id)
```
获取最后一个会话ID```python
last_id = await client.get_last_session_id()
if last_id:
    session = await client.resume_session(last_id, on_permission_request=PermissionHandler.approve_all)
```
检查连接状态```python
state = client.get_state()
# Returns: "disconnected" | "connecting" | "connected" | "error"
```
##错误处理

标准异常处理```python
try:
    session = await client.create_session(on_permission_request=PermissionHandler.approve_all)
    await session.send({"prompt": "Hello"})
except Exception as e:
    print(f"Error: {e}")
```
会话错误事件

监视`session.error`事件类型以查看运行时错误：```python
def handler(event):
    if event.type == "session.error":
        print(f"Session Error: {event.data.message}")

session.on(handler)
```
##连接测试

使用ping来验证服务器连接：```python
response = await client.ping("health check")
print(f"Server responded at {response['timestamp']}")
```
##资源清理

使用上下文管理器自动清理

总是使用异步上下文管理器进行自动清理：```python
async with CopilotClient() as client:
    async with await client.create_session(on_permission_request=PermissionHandler.approve_all) as session:
        # Use session...
        await session.send({"prompt": "Hello"})
    # Session automatically destroyed
# Client automatically stopped
```
使用Try-Finally手动清理```python
client = CopilotClient()
try:
    await client.start()
    session = await client.create_session(on_permission_request=PermissionHandler.approve_all)
    try:
        # Use session...
        pass
    finally:
        await session.destroy()
finally:
    await client.stop()
```
最佳实践

1. **始终使用异步上下文管理器** (`async with`)进行自动清理
2. * *使用asyncio。Event或asyncio。未来**等待会话。空闲事件
3. * *处理会话。Error **事件用于健壮的错误处理
4. **使用if/elif链**进行事件类型检查
5. **在交互场景中启用流**以获得更好的用户体验
6. **使用define_tool**来定义工具
7. **使用Pydantic模型进行类型安全的参数验证
8. 当不再需要时，处理事件订阅
9. **使用system_message with mode: "append"**来保存安全护栏
10. **处理增量和最终事件**时，流是启用的
11. **使用类型提示**以获得更好的IDE支持和代码清晰度

##常见模式

简单查询-响应```python
from copilot import CopilotClient, PermissionHandler
import asyncio

async def main():
    async with CopilotClient() as client:
        async with await client.create_session({
            "on_permission_request": PermissionHandler.approve_all,
            "model": "gpt-5",
        }) as session:
            done = asyncio.Event()

            def handler(event):
                if event.type == "assistant.message":
                    print(event.data.content)
                elif event.type == "session.idle":
                    done.set()

            session.on(handler)
            await session.send({"prompt": "What is 2+2?"})
            await done.wait()

asyncio.run(main())
```
多回合对话```python
async def send_and_wait(session, prompt: str):
    done = asyncio.Event()
    result = []

    def handler(event):
        if event.type == "assistant.message":
            result.append(event.data.content)
            print(event.data.content)
        elif event.type == "session.idle":
            done.set()
        elif event.type == "session.error":
            result.append(None)
            done.set()

    unsubscribe = session.on(handler)
    await session.send({"prompt": prompt})
    await done.wait()
    unsubscribe()

    return result[0] if result else None

async with await client.create_session(on_permission_request=PermissionHandler.approve_all) as session:
    await send_and_wait(session, "What is the capital of France?")
    await send_and_wait(session, "What is its population?")
```
SendAndWait Helper```python
# Use built-in send_and_wait for simpler synchronous interaction
async with await client.create_session(on_permission_request=PermissionHandler.approve_all) as session:
    response = await session.send_and_wait(
        {"prompt": "What is 2+2?"},
        timeout=60.0
    )

    if response and response.type == "assistant.message":
        print(response.data.content)
```
带有数据类返回类型的工具```python
from dataclasses import dataclass, asdict
from copilot import define_tool

@dataclass
class UserInfo:
    id: str
    name: str
    email: str
    role: str

async def get_user(args, inv) -> dict:
    user = UserInfo(
        id=args["user_id"],
        name="John Doe",
        email="john@example.com",
        role="Developer"
    )
    return asdict(user)

session = await client.create_session({
    "on_permission_request": PermissionHandler.approve_all,
    "tools": [
        define_tool(
            name="get_user",
            description="Retrieve user information",
            parameters={
                "type": "object",
                "properties": {
                    "user_id": {"type": "string", "description": "User ID"}
                },
                "required": ["user_id"]
            },
            handler=get_user
        )
    ]
})
```
###有进展的流媒体```python
import asyncio

current_message = []
done = asyncio.Event()

def handler(event):
    if event.type == "assistant.message.delta":
        current_message.append(event.data.delta_content)
        print(event.data.delta_content, end="", flush=True)
    elif event.type == "assistant.message":
        print(f"\n\n=== Complete ===")
        print(f"Total length: {len(event.data.content)} chars")
    elif event.type == "session.idle":
        done.set()

unsubscribe = session.on(handler)
await session.send({"prompt": "Write a long story"})
await done.wait()
unsubscribe()
```
错误恢复```python
def handler(event):
    if event.type == "session.error":
        print(f"Session error: {event.data.message}")
        # Optionally retry or handle error

session.on(handler)

try:
    await session.send({"prompt": "risky operation"})
except Exception as e:
    # Handle send errors
    print(f"Failed to send: {e}")
```
使用TypedDict实现类型安全```python
from typing import TypedDict, List

class MessageOptions(TypedDict, total=False):
    prompt: str
    attachments: List[dict]
    mode: str

class SessionConfig(TypedDict, total=False):
    model: str
    streaming: bool
    tools: List

# Usage with type hints
options: MessageOptions = {
    "prompt": "Hello",
    "mode": "enqueue"
}
await session.send(options)

config: SessionConfig = {
    "on_permission_request": PermissionHandler.approve_all,
    "model": "gpt-5",
    "streaming": True
}
session = await client.create_session(config)
```
流的异步生成器```python
from typing import AsyncGenerator

async def stream_response(session, prompt: str) -> AsyncGenerator[str, None]:
    """Stream response chunks as an async generator."""
    queue = asyncio.Queue()
    done = asyncio.Event()

    def handler(event):
        if event.type == "assistant.message.delta":
            queue.put_nowait(event.data.delta_content)
        elif event.type == "session.idle":
            done.set()

    unsubscribe = session.on(handler)
    await session.send({"prompt": prompt})

    while not done.is_set():
        try:
            chunk = await asyncio.wait_for(queue.get(), timeout=0.1)
            yield chunk
        except asyncio.TimeoutError:
            continue

    # Drain remaining items
    while not queue.empty():
        yield queue.get_nowait()

    unsubscribe()

# Usage
async for chunk in stream_response(session, "Tell me a story"):
    print(chunk, end="", flush=True)
```
工具的装饰器模式```python
from typing import Callable, Any
from copilot import define_tool

def copilot_tool(
    name: str,
    description: str,
    parameters: dict
) -> Callable:
    """Decorator to convert a function into a Copilot tool."""
    def decorator(func: Callable) -> Any:
        return define_tool(
            name=name,
            description=description,
            parameters=parameters,
            handler=lambda args, inv: func(**args)
        )
    return decorator

@copilot_tool(
    name="calculate",
    description="Perform a calculation",
    parameters={
        "type": "object",
        "properties": {
            "expression": {"type": "string", "description": "Math expression"}
        },
        "required": ["expression"]
    }
)
def calculate(expression: str) -> float:
    return eval(expression)

session = await client.create_session({
    "on_permission_request": PermissionHandler.approve_all,
    "tools": [calculate]})
```
python特有的特性

Async上下文管理器协议

SDK实现`__aenter__`和`__aexit__`：```python
class CopilotClient:
    async def __aenter__(self):
        await self.start()
        return self

    async def __aexit__(self, exc_type, exc_val, exc_tb):
        await self.stop()
        return False

class CopilotSession:
    async def __aenter__(self):
        return self

    async def __aexit__(self, exc_type, exc_val, exc_tb):
        await self.destroy()
        return False
```
数据类支持

事件数据作为属性可用：```python
def handler(event):
    # Access event attributes directly
    print(event.type)
    print(event.data.content)  # For assistant.message
    print(event.data.delta_content)  # For assistant.message.delta
```
