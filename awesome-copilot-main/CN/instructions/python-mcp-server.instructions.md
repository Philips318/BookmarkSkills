---
description: 'Instructions for building Model Context Protocol (MCP) servers using the Python SDK'
applyTo: '**/*.py, **/pyproject.toml, **/requirements.txt'
---
# Python MCP服务器开发

# #指令-使用**uv**进行项目管理：`uv init mcp-server-demo`和`uv add "mcp[cli]"`—从“`mcp.server.fastmcp`:`from mcp.server.fastmcp import FastMCP`”导入FastMCP
-使用`@mcp.tool()`，`@mcp.resource()`和`@mcp.prompt()`装饰器进行注册
-类型提示是强制性的-它们用于模式生成和验证
-使用Pydantic模型、TypedDicts或数据类进行结构化输出
—当返回类型兼容时，工具会自动返回结构化输出
-对于演播室传输，使用`mcp.run()`或`mcp.run(transport="stdio")`—对于HTTP服务器，使用“`mcp.run(transport="streamable-http")`”或挂载到“Starlette/FastAPI”
—使用“tools/resources”中的“`Context`”参数访问MCP功能
—使用`await ctx.debug()`、`await ctx.info()`、`await ctx.warning()`、`await ctx.error()`发送日志
-使用`await ctx.report_progress(progress, total, message)`报告进度
—使用`await ctx.elicit(message, schema)`请求用户输入
-使用LLM采样`await ctx.session.create_message(messages, max_tokens)`—用`Icon(src="path", mimeType="image/png")`配置服务器、工具、资源、提示符的图标
-使用`Image`类自动图像处理：`return Image(data=bytes, format="png")`—使用URI模式定义资源模板：`@mcp.resource("greeting://{name}")`-通过接受部分值和返回建议来实现补全支持
-为共享资源的startup/shutdown使用寿命上下文管理器
-通过`ctx.request_context.lifespan_context`访问工具中的寿命上下文
—对于无状态HTTP服务器，在FastMCP初始化中设置`stateless_http=True`-启用JSON响应现代客户端：`json_response=True`-测试服务器：`uv run mcp dev server.py`（检查器）或`uv run mcp install server.py`（克劳德桌面）
—在Starlette中挂载多个不同路径的服务器：`Mount("/path", mcp.streamable_http_app())`—为浏览器客户端配置CORS: expose`Mcp-Session-Id`header
-当FastMCP不够时，使用低级服务器类来最大限度地控制最佳实践

-始终使用类型提示-它们驱动模式生成和验证
-为结构化工具输出返回Pydantic模型或TypedDicts
-将工具功能集中在单一职责上
-提供清晰的文档字符串-它们成为工具描述
—使用带有类型提示的描述性参数名
-使用Pydantic字段描述验证输入
-通过try-except块实现正确的错误处理
—I/O-bound操作使用异步函数
-清理生命周期上下文管理器中的资源
-登录到stderr以避免干扰stdio传输（当使用stdio时）
—使用环境变量进行配置
-在LLM集成前独立测试工具
—在公开文件系统或网络访问时，请考虑安全性
-对机器可读数据使用结构化输出
—同时提供内容数据和结构化数据，实现向后兼容##常见模式

基本服务器设置（stdio）```python
from mcp.server.fastmcp import FastMCP

mcp = FastMCP("My Server")

@mcp.tool()
def calculate(a: int, b: int, op: str) -> int:
    """Perform calculation"""
    if op == "add":
        return a + b
    return a - b

if __name__ == "__main__":
    mcp.run()  # stdio by default
```
### HTTP服务器```python
from mcp.server.fastmcp import FastMCP

mcp = FastMCP("My HTTP Server")

@mcp.tool()
def hello(name: str = "World") -> str:
    """Greet someone"""
    return f"Hello, {name}!"

if __name__ == "__main__":
    mcp.run(transport="streamable-http")
```
具有结构化输出的工具```python
from pydantic import BaseModel, Field

class WeatherData(BaseModel):
    temperature: float = Field(description="Temperature in Celsius")
    condition: str
    humidity: float

@mcp.tool()
def get_weather(city: str) -> WeatherData:
    """Get weather for a city"""
    return WeatherData(
        temperature=22.5,
        condition="sunny",
        humidity=65.0
    )
```
动态资源```python
@mcp.resource("users://{user_id}")
def get_user(user_id: str) -> str:
    """Get user profile data"""
    return f"User {user_id} profile data"
```
带有上下文的工具```python
from mcp.server.fastmcp import Context
from mcp.server.session import ServerSession

@mcp.tool()
async def process_data(
    data: str, 
    ctx: Context[ServerSession, None]
) -> str:
    """Process data with logging"""
    await ctx.info(f"Processing: {data}")
    await ctx.report_progress(0.5, 1.0, "Halfway done")
    return f"Processed: {data}"
```
工具与采样```python
from mcp.server.fastmcp import Context
from mcp.server.session import ServerSession
from mcp.types import SamplingMessage, TextContent

@mcp.tool()
async def summarize(
    text: str,
    ctx: Context[ServerSession, None]
) -> str:
    """Summarize text using LLM"""
    result = await ctx.session.create_message(
        messages=[SamplingMessage(
            role="user",
            content=TextContent(type="text", text=f"Summarize: {text}")
        )],
        max_tokens=100
    )
    return result.content.text if result.content.type == "text" else ""
```
寿命管理```python
from contextlib import asynccontextmanager
from dataclasses import dataclass
from mcp.server.fastmcp import FastMCP, Context

@dataclass
class AppContext:
    db: Database

@asynccontextmanager
async def app_lifespan(server: FastMCP):
    db = await Database.connect()
    try:
        yield AppContext(db=db)
    finally:
        await db.disconnect()

mcp = FastMCP("My App", lifespan=app_lifespan)

@mcp.tool()
def query(sql: str, ctx: Context) -> str:
    """Query database"""
    db = ctx.request_context.lifespan_context.db
    return db.execute(sql)
```
提示消息```python
from mcp.server.fastmcp.prompts import base

@mcp.prompt(title="Code Review")
def review_code(code: str) -> list[base.Message]:
    """Create code review prompt"""
    return [
        base.UserMessage("Review this code:"),
        base.UserMessage(code),
        base.AssistantMessage("I'll review the code for you.")
    ]
```
错误处理```python
@mcp.tool()
async def risky_operation(input: str) -> str:
    """Operation that might fail"""
    try:
        result = await perform_operation(input)
        return f"Success: {result}"
    except Exception as e:
        return f"Error: {str(e)}"
```
