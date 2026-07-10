# Runnable示例：独立函数（无服务器）

**当应用程序是一个普通的Python函数或模块** -没有web框架，没有服务器，没有基础设施。

**方法**：直接从`run()`导入并调用函数。这是最简单的例子。```python
# pixie_qa/run_app.py
from pydantic import BaseModel
import pixie


class AppArgs(BaseModel):
    question: str


class AppRunnable(pixie.Runnable[AppArgs]):
    """Drives a standalone function for tracing and evaluation."""

    @classmethod
    def create(cls) -> "AppRunnable":
        return cls()

    async def run(self, args: AppArgs) -> None:
        from myapp.agent import answer_question
        await answer_question(args.question)
```
如果函数是同步的，用`asyncio.to_thread`包装它：```python
import asyncio

async def run(self, args: AppArgs) -> None:
    from myapp.agent import answer_question
    await asyncio.to_thread(answer_question, args.question)
```
如果函数依赖于外部服务（例如，矢量存储），那么在步骤2a中添加的`wrap(purpose="input")`调用将自动处理它——注册中心以eval模式注入测试数据。

何时使用`setup()`/`teardown()`大多数独立函数不需要生命周期方法。只有当函数需要共享资源时才使用它们（例如，预加载的嵌入模型，数据库连接）：```python
class AppRunnable(pixie.Runnable[AppArgs]):
    _model: SomeModel

    @classmethod
    def create(cls) -> "AppRunnable":
        return cls()

    async def setup(self) -> None:
        from myapp.models import load_model
        self._model = load_model()

    async def run(self, args: AppArgs) -> None:
        from myapp.agent import answer_question
        await answer_question(args.question, model=self._model)
```
