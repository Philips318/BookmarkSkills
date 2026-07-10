#步骤2b：执行Runnable

有关完整的`Runnable`协议和`wrap()`API，请参见`wrap-api.md`。

**目标**：编写一个Runnable类，让eval控制像真正的用户那样调用应用程序。

---

##核心理念

Runnable是`pixie test`和`pixie trace`运行应用程序的方式。可以把它想象成一个真正用户的编程替身：它启动应用程序，向它发送请求，然后让应用程序做它自己的事情。eval harness为每个测试用例调用`run()`，传入用户的输入参数。应用程序通过其实际代码处理这些参数-真正的路由，真正的提示汇编，真正的LLM调用，真正的响应格式-并且通过步骤2a中的`wrap()`仪器观察发生了什么。**这意味着Runnable应该很简单。**它只是将应用程序的真正入口点连接到线束接口。如果你的Runnable变得复杂了——如果你正在构建自定义逻辑，重新实现应用程序行为，或者替换组件——那就有问题了。

##四个要求

# # # 1。运行实际的产品代码

Runnable调用应用程序的实际入口点——与实际用户触发的函数、类或端点相同。它不会重新实现、简化或替换应用程序的任何部分。

这包括法学硕士。应用程序的LLM调用必须经过真实的代码路径——不要模拟、伪造或替换应用程序组件。基于评估的测试的要点在于LLM输出是不确定的，因此您使用评估器（而不是断言）对它们进行评分。如果您用伪组件替换任何组件，您就消除了真实的行为，并且eval没有测量任何内容。**如果应用程序由于缺少环境变量或无法解决的配置而无法运行，请停止并要求用户修复环境设置。**不要通过模拟组件来绕过它。

# # # 2。用Pydantic BaseModel表示启动参数`run()`方法接收一个Pydantic`BaseModel`，该`BaseModel`的字段由数据集的`input_data`填充。用应用需要的字段定义一个子类：```python
from pydantic import BaseModel

class AppArgs(BaseModel):
    user_message: str
    # Add more fields as the app's entry point requires.
    # These map 1:1 to the dataset input_data keys.
```
**字段必须反映真实用户实际提供的内容。**阅读`pixie_qa/00-project-analysis.md`-“实际输入特性”部分描述了实际输入的复杂性，规模和多样性。将模型设计成能够接受现实主义水平的输入，而不是简化的玩具版本。

理解用户提供的参数和世界数据之间的界限：

- **用户提供的参数** （BaseModel上的字段）：真实用户类型或配置的内容-提示，查询，配置标志，url，模式定义。
**世界数据**（由步骤2a中的`wrap(purpose="input")`处理）：应用程序在执行期间从外部来源获取的内容-网页，数据库记录，API响应。这不是BaseModel的一部分。

|应用程序类型| BaseModel字段（用户提供）|世界数据（包装提供）|| -------------------- | ------------------------------------- | ------------------------------------------------------------------ |
b| Web scraper | URL +提示符+架构定义| HTML页面内容|
|研究代理|研究问题+范围约束|源文档，搜索结果|
|客户支持bot |客户的语音消息|来自CRM的客户配置文件，会话存储|的会话历史
|代码审查工具| PR URL +审查标准|实际差异，文件内容，CI结果|

如果一个字段最终保存了应用程序通常会自己获取的数据，那么它可能属于`wrap(purpose="input")`调用，而不是BaseModel。

# # # 3。是concurrency-safe

对于多个数据集条目（最多并行4个），`run()`被并发调用。如果应用程序使用共享可变状态——SQLite，基于文件的数据库，全局缓存——用`asyncio.Semaphore`保护访问：```python
import asyncio

class AppRunnable(pixie.Runnable[AppArgs]):
    _sem: asyncio.Semaphore

    @classmethod
    def create(cls) -> "AppRunnable":
        inst = cls()
        inst._sem = asyncio.Semaphore(1)
        return inst

    async def run(self, args: AppArgs) -> None:
        async with self._sem:
            await call_app(args.message)
```
只有当应用程序实际具有共享可变状态时才添加信号量。如果应用程序使用每个请求状态（由唯一id键控）或本质上是无状态的，那么并发调用自然是隔离的。

# # # 4。坚持Runnable界面```python
class AppRunnable(pixie.Runnable[AppArgs]):
    @classmethod
    def create(cls) -> "AppRunnable": ...     # construct instance
    async def setup(self) -> None: ...        # once, before first run()
    async def run(self, args: AppArgs) -> None: ...  # per dataset entry, concurrent
    async def teardown(self) -> None: ...     # once, after last run()
```
-`create()`- class方法，返回一个新实例。使用带引号的返回类型（`-> "AppRunnable"`）来避免前向引用错误。
-`setup()`-可选async；初始化共享资源（HTTP客户端、DB连接、服务器）。
-`run(args)`- async；每个数据集条目调用。调用应用程序的真正入口点。
-`teardown()`-可选async；从`setup()`清理资源。

最小示例```python
# pixie_qa/run_app.py
from pydantic import BaseModel
import pixie


class AppArgs(BaseModel):
    user_message: str


class AppRunnable(pixie.Runnable[AppArgs]):
    """Drives the application for tracing and evaluation."""

    @classmethod
    def create(cls) -> "AppRunnable":
        return cls()

    async def run(self, args: AppArgs) -> None:
        from myapp import handle_request
        await handle_request(args.user_message)
```
就是这样。Runnable导入应用程序的实际入口点并调用它。没有自定义逻辑，没有组件替换，没有聪明的变通方法。

特定于体系结构的示例

根据应用程序的运行方式，读取相应的示例文件：

|应用类型|入口点|示例文件|| ----------------------------------- | ----------------------- | ---------------------------------------------------------- |
| **独立函数**（无服务器）| Python函数|读取`references/runnable-examples/standalone-function.md`|
| **Web服务器** (FastAPI, Flask) |HTTP/WebSocket端点|读取`references/runnable-examples/fastapi-web-server.md`|
| **CLI应用** |命令行调用|读取`references/runnable-examples/cli-app.md`|

只读取与你的应用类型匹配的示例文件。

##文件放置

-将文件放在`pixie_qa/run_app.py`。
—数据集的`"runnable"`字段引用：`"pixie_qa/run_app.py:AppRunnable"`。
—项目根目录自动在`sys.path`上，所以使用普通导入（`from app import service`）。

##技术说明

不要在可运行文件中使用`from __future__ import annotations`-它会破坏Pydantic对嵌套模型的模型解析。在需要的地方使用带引号的返回类型。

---

# #输出`pixie_qa/run_app.py`- Runnable类。