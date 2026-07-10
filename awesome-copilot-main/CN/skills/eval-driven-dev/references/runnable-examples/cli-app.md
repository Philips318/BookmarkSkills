示例：CLI应用程序

**当应用程序从命令行调用时**（例如，`python -m myapp`，一个带有argparse/click的CLI工具）。

**方法**：使用`asyncio.create_subprocess_exec`调用CLI并捕获输出。```python
# pixie_qa/run_app.py
import asyncio
import sys

from pydantic import BaseModel
import pixie


class AppArgs(BaseModel):
    query: str


class AppRunnable(pixie.Runnable[AppArgs]):
    """Drives a CLI application via subprocess."""

    @classmethod
    def create(cls) -> "AppRunnable":
        return cls()

    async def run(self, args: AppArgs) -> None:
        proc = await asyncio.create_subprocess_exec(
            sys.executable, "-m", "myapp", "--query", args.query,
            stdout=asyncio.subprocess.PIPE,
            stderr=asyncio.subprocess.PIPE,
        )
        stdout, stderr = await asyncio.wait_for(proc.communicate(), timeout=120)
        if proc.returncode != 0:
            raise RuntimeError(f"App failed (exit {proc.returncode}): {stderr.decode()}")
```
当CLI需要修补依赖项时

如果CLI从外部服务读取，在运行真正的CLI之前，创建一个包装器入口点来修补依赖：```python
# pixie_qa/patched_app.py
"""Entry point that patches external deps before running the real CLI."""
import myapp.config as config
config.redis_url = "mock://localhost"

from myapp.main import main
main()
```
然后将Runnable指向包装器：```python
async def run(self, args: AppArgs) -> None:
    proc = await asyncio.create_subprocess_exec(
        sys.executable, "-m", "pixie_qa.patched_app", "--query", args.query,
        stdout=asyncio.subprocess.PIPE,
        stderr=asyncio.subprocess.PIPE,
    )
    stdout, stderr = await asyncio.wait_for(proc.communicate(), timeout=120)
```
**注**：对于CLI应用程序，`wrap(purpose="input")`注入仅在应用程序运行在同一进程时有效。如果使用子流程，您可能需要通过环境变量或配置文件来传递测试数据。