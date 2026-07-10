#封装API引用

>从pixie源代码文档字符串自动生成。
>请勿手动编辑-运行`uv run python scripts/generate_skill_docs.py`。`pixie.wrap`-面向数据的观测API。

中的指定点观察数据值或可调用对象
处理管道。它的行为取决于活动模式：

- ** no -op**（禁用跟踪，没有eval注册表）：返回`data`不变。
—**跟踪** （`pixie trace`期间）：写入跟踪文件并发出一个
OTel事件（如果span是活动的，则通过span事件，或者通过OTel记录器）
否则)并原封不动地返回`data`(或包装一个可调用对象，以便
事件在调用时触发)。
- **Eval** （Eval注册表激活）：注入依赖数据`purpose="input"`，为`purpose="output"`/捕获output/state`purpose="state"`。

---

命令行命令|命令|描述|| ----------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------- |
|`pixie trace --runnable <filepath:ClassName> --input <kwargs.json> --output <file.jsonl>`|使用JSON文件中的kwargs运行Runnable一次，并编写跟踪文件。`--input`是一个**文件路径**（不是内联JSON）。|
|`pixie format --input <trace.jsonl> --output <dataset_entry.json>`|将跟踪文件转换为格式化的数据集条目模板。显示`input_data`、`eval_input`和`eval_output`（实际捕获的输出）。|
|`pixie trace filter <file.jsonl> --purpose input`|只打印符合给定目的的换行事件。每个匹配事件输出一行JSON。|

---

# #类

# # #`pixie.Runnable````python
class pixie.Runnable(Protocol[T]):
    @classmethod
    def create(cls) -> Runnable[Any]: ...
    async def setup(self) -> None: ...
    async def run(self, args: T) -> None: ...
    async def teardown(self) -> None: ...
```
评估工具使用的结构化可运行程序协议。`T`是a
字段与`input_data`键匹配的`pydantic.BaseModel`子类
在数据集JSON。

生命周期:

1. 构造并返回可运行实例的类方法。
2.`setup()`- **async**，在第一次`run()`调用之前**一次。
在这里初始化共享资源（例如，`TestClient`，数据库连接）。
可选-具有默认的无操作实现。
3.`run(args)`- **async**，对每个数据集条目**并发调用**
（最多4个并行条目）。`args`是一个经过验证的Pydantic模型
从`input_data`构建。调用应用程序的实际入口点。
4.`teardown()`- **async**，在最后一次`run()`调用之后** *一次。
释放在`setup()`中获得的任何资源。
可选-具有默认的无操作实现。`setup()`和`teardown()`有默认的无操作实现；
您只需要在需要共享资源时重写它们。

**并发**:`run()`通过`asyncio.gather`并发调用。你的
实现必须是并发安全的。如果它使用共享可变状态
（例如，一个SQLite连接，一个内存缓存，一个文件句柄），保护它`asyncio.Semaphore`或`asyncio.Lock`：```python
class AppRunnable(pixie.Runnable[AppArgs]):
    _sem: asyncio.Semaphore

    @classmethod
    def create(cls) -> "AppRunnable":
        inst = cls()
        inst._sem = asyncio.Semaphore(1)  # serialise DB access
        return inst

    async def run(self, args: AppArgs) -> None:
        async with self._sem:
            await call_app(args.message)
```
常见的并发缺陷：

**SQLite**：并发写不安全-使用`Semaphore(1)`或`aiosqlite`与WAL模式。
- **全局可变状态**：在`run()`中修改的模块级dicts/lists需要保护。
- **限速api **：增加一个信号量，避免429错误。

**导入分辨率**：项目根目录（其中`pixie test`/`pixie trace`. zip为`pixie test`. zip）
在加载可运行程序之前，自动添加到`sys.path`评价者。这意味着您的可运行程序可以使用普通的`import`语句来
参考项目模块（例如，`from app import service`）。

* * * *例子:```python
# pixie_qa/run_app.py
from pydantic import BaseModel
import pixie

class AppArgs(BaseModel):
    user_message: str

class AppRunnable(pixie.Runnable[AppArgs]):
    @classmethod
    def create(cls) -> "AppRunnable":
        return cls()

    async def run(self, args: AppArgs) -> None:
        from myapp import handle_request
        await handle_request(args.user_message)
```
Web服务器示例**（使用异步HTTP客户端）：```python
import httpx
from pydantic import BaseModel
import pixie

class AppArgs(BaseModel):
    user_message: str

class AppRunnable(pixie.Runnable[AppArgs]):
    _client: httpx.AsyncClient

    @classmethod
    def create(cls) -> "AppRunnable":
        return cls()

    async def setup(self) -> None:
        self._client = httpx.AsyncClient(base_url="http://localhost:8000")

    async def run(self, args: AppArgs) -> None:
        await self._client.post("/chat", json={"message": args.user_message})

    async def teardown(self) -> None:
        await self._client.aclose()
```
---

# #功能

# # #`pixie.wrap````python
pixie.wrap(data: 'T', *, purpose: "Literal['input', 'output', 'state']", name: 'str', description: 'str | None' = None) -> 'T'
```
观察在处理管道中的某个点上可调用的数据值或数据提供程序。`data`可以是一个普通值，也可以是一个产生值的可调用对象。
在这两种情况下，返回类型都是`T`—调用者返回的是完全相同的
与在无操作或跟踪模式下传递的类型相同。

在使用`purpose="input"`的eval模式下，返回值（或可调用值）为
替换为反序列化的注册表值。当`data`可调用时
返回的包装器忽略原始函数并返回注入的函数
每一次呼叫的价值；在所有其他模式中，返回的可调用对象封装
原始并添加跟踪或捕获行为。参数:
data：可调用的数据值或数据提供程序。
目的：对数据点进行分类：“输入”：来自外部依赖的数据（DB记录、API响应）—“输出”：向外部系统或用户输出的数据—“状态”：用于评估的中间状态（路由决策等）
name：此数据点的唯一标识符。的关键字
Eval注册表和跟踪日志。
description：可选的、人类可读的数据描述。

返回:
原始数据不变（跟踪/无操作模式），或者
注册表值（eval mode with purpose="input"）。当`data`是可调用的，返回值也是可调用的。

---

##错误类型

# # #`WrapRegistryMissError````python
WrapRegistryMissError(name: 'str') -> 'None'
```
当在eval注册表中找不到换行（purpose="input"）名称时引发。

# # #`WrapTypeMismatchError````python
WrapTypeMismatchError(name: 'str', expected_type: 'type', actual_type: 'type') -> 'None'
```
当反序列化的注册表值与预期类型不匹配时引发。

---

跟踪文件实用程序

包装日志条目和JSONL加载实用程序的Pydantic模型。`WrapLogEntry`是单个`wrap()`事件的类型化表示
记录在JSONL跟踪文件中。代码库中的多个位置加载
这些对象—`pixie trace filter`CLI、数据集加载器和
验证脚本——它们共享这个单一模型。

# # #`pixie.WrapLogEntry````python
pixie.WrapLogEntry(*, type: str = 'wrap', name: str, purpose: str, data: Any, description: str | None = None, trace_id: str | None = None, span_id: str | None = None) -> None
```
单个wrap（）事件被记录到json跟踪文件中。

属性:
类型：对于包装事件总是`"wrap"`。
name：换行点名称（匹配`wrap(name=...)`）。
用途：`"input"`，`"output"`，`"state"`之一。
data：序列化的数据（jsonpickle字符串）。
description：可选的人类可读描述。
trace_id: OTel跟踪ID（如果可用）。
span_id: OTel span ID（如果可用）。

# # #`pixie.load_wrap_log_entries````python
pixie.load_wrap_log_entries(jsonl_path: 'str | Path') -> 'list[WrapLogEntry]'
```
从JSONL文件加载所有包装日志条目。

跳过非换行行（例如`type=llm_span`）和畸形行。

参数:
jsonl_path: JSONL跟踪文件路径。

返回:
类：`WrapLogEntry`对象列表。

# # #`pixie.filter_by_purpose````python
pixie.filter_by_purpose(entries: 'list[WrapLogEntry]', purposes: 'set[str]') -> 'list[WrapLogEntry]'
```
按目的过滤包装日志条目。

参数:
entries：包装日志条目列表。
目的：要包含的目的值的集合。

返回:
过滤后的列表。