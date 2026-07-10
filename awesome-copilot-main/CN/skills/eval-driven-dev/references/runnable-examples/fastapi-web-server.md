示例：FastAPI / Web Server

**当应用程序是一个web服务器** (FastAPI, Flask, Starlette)，你需要执行完整的HTTP请求管道。

**方法**：使用`httpx.AsyncClient`和`ASGITransport`来运行ASGI应用程序。这是最快和最可靠的方法——没有子进程，没有端口管理。```python
# pixie_qa/run_app.py
import httpx
from pydantic import BaseModel
import pixie


class AppArgs(BaseModel):
    user_message: str


class AppRunnable(pixie.Runnable[AppArgs]):
    """Drives a FastAPI app via in-process ASGI transport."""

    _client: httpx.AsyncClient

    @classmethod
    def create(cls) -> "AppRunnable":
        return cls()

    async def setup(self) -> None:
        from myapp.main import app  # your FastAPI/Starlette app instance

        transport = httpx.ASGITransport(app=app)
        self._client = httpx.AsyncClient(transport=transport, base_url="http://test")

    async def run(self, args: AppArgs) -> None:
        await self._client.post("/chat", json={"message": args.user_message})

    async def teardown(self) -> None:
        await self._client.aclose()
```
asgittransport跳过生命周期事件`httpx.ASGITransport`不触发ASGI寿命事件（`startup`/`shutdown`）。如果应用程序在其生命周期内初始化资源（数据库连接，缓存，服务客户端），你必须在`setup()`中手动复制初始化：```python
async def setup(self) -> None:
    # Manually replicate what the app's lifespan does
    from myapp.db import get_connection, init_db, seed_data
    import myapp.main as app_module

    conn = get_connection()
    init_db(conn)
    seed_data(conn)
    app_module.db_conn = conn  # set the module-level global the app expects

    transport = httpx.ASGITransport(app=app_module.app)
    self._client = httpx.AsyncClient(transport=transport, base_url="http://test")

async def teardown(self) -> None:
    await self._client.aclose()
    # Clean up the manually-initialized resources
    import myapp.main as app_module
    if hasattr(app_module, "db_conn") and app_module.db_conn:
        app_module.db_conn.close()
```
共享可变状态的并发性

如果应用程序使用共享可变状态（内存SQLite，基于文件的数据库，全局缓存），添加一个信号量来序列化访问：```python
import asyncio

class AppRunnable(pixie.Runnable[AppArgs]):
    _client: httpx.AsyncClient
    _sem: asyncio.Semaphore

    @classmethod
    def create(cls) -> "AppRunnable":
        inst = cls()
        inst._sem = asyncio.Semaphore(1)
        return inst

    async def setup(self) -> None:
        from myapp.main import app
        transport = httpx.ASGITransport(app=app)
        self._client = httpx.AsyncClient(transport=transport, base_url="http://test")

    async def run(self, args: AppArgs) -> None:
        async with self._sem:
            await self._client.post("/chat", json={"message": args.user_message})

    async def teardown(self) -> None:
        await self._client.aclose()
```
只在需要的时候使用信号量——如果应用程序使用由唯一id （call_sid, session_id）键控的每个会话状态，并发调用自然是隔离的，不需要锁。

##备选方案：使用httpx的外部服务器

当应用程序无法直接导入时（复杂启动，`uvicorn.run()`在`__main__`），将其作为子进程启动，并使用HTTP访问它：```python
class AppRunnable(pixie.Runnable[AppArgs]):
    _client: httpx.AsyncClient

    @classmethod
    def create(cls) -> "AppRunnable":
        return cls()

    async def setup(self) -> None:
        # Assumes the server is already running (started via run-with-timeout.sh)
        self._client = httpx.AsyncClient(base_url="http://localhost:8000")

    async def run(self, args: AppArgs) -> None:
        await self._client.post("/chat", json={"message": args.user_message})

    async def teardown(self) -> None:
        await self._client.aclose()
```
在运行`pixie trace`或`pixie test`之前启动服务器：```bash
bash resources/run-with-timeout.sh 120 uv run python -m myapp.server
sleep 3  # wait for readiness
```
