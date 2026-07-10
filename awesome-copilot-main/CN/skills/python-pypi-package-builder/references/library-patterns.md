#库核心模式，OOP/SOLID和类型提示

##目录
1. [OOP & SOLID原则]（#1-oop—SOLID - Principles）
2. [类型提示最佳实践]（#2- Type -hint - Best - Practices）
3. [核心课程设计]（#3-core-class-design）
4. [工厂/建造者模式]（#4-factory- Builder - Pattern）
5. (配置模式)(# 5-configuration-pattern)
6. [`__init__.py`-explicit public API]（#6-__init__py——explicit-public- API）
7. [可选后端（插件模式）]（#7- Optional -backend - Plugin - Pattern）

---

# # 1。OOP和SOLID原则

应用这些原则来生成可维护的、可测试的、可扩展的包。
不要过度工程化——应用解决实际问题的原则，而不是所有的原则
在一次。

单一责任原则

每个class/module应该有一个改变的理由。```python
# BAD: one class handles data, validation, AND persistence
class UserManager:
    def validate(self, user): ...
    def save_to_db(self, user): ...
    def send_email(self, user): ...

# GOOD: split responsibilities
class UserValidator:
    def validate(self, user: User) -> None: ...

class UserRepository:
    def save(self, user: User) -> None: ...

class UserNotifier:
    def notify(self, user: User) -> None: ...
```
### 0 -Open/Closed原理

打开扩展，关闭修改。使用协议或abc作为扩展点。```python
from abc import ABC, abstractmethod

class StorageBackend(ABC):
    """Define the interface once; never modify it for new implementations."""
    @abstractmethod
    def get(self, key: str) -> str | None: ...
    @abstractmethod
    def set(self, key: str, value: str) -> None: ...

class MemoryBackend(StorageBackend):    # Extend by subclassing
    ...

class RedisBackend(StorageBackend):     # Add new impl without touching StorageBackend
    ...
```
### L - Liskov代换原理

子类必须是基类的替代品。永远不要在子类中缩小契约。```python
class BaseProcessor:
    def process(self, data: dict) -> dict: ...

# BAD: raises TypeError for valid dicts — breaks substitutability
class StrictProcessor(BaseProcessor):
    def process(self, data: dict) -> dict:
        if not data:
            raise TypeError("Must have data")  # Base never raised this

# GOOD: accept what base accepts, fulfill the same contract
class StrictProcessor(BaseProcessor):
    def process(self, data: dict) -> dict:
        if not data:
            return {}   # Graceful — same return type, no new exceptions
```
接口隔离原则

比起大型的单片abc，更喜欢小型的、集中的协议。```python
# BAD: forces all implementers to handle read+write+delete+list
class BigStorage(ABC):
    @abstractmethod
    def read(self): ...
    @abstractmethod
    def write(self): ...
    @abstractmethod
    def delete(self): ...
    @abstractmethod
    def list_all(self): ...   # Not every backend needs this

# GOOD: separate protocols — clients depend only on what they need
from typing import Protocol

class Readable(Protocol):
    def read(self, key: str) -> str | None: ...

class Writable(Protocol):
    def write(self, key: str, value: str) -> None: ...

class Deletable(Protocol):
    def delete(self, key: str) -> None: ...
```
### D -依赖倒置原则

高级模块依赖于抽象（protocols/ABCs），而不是具体的实现。
通过`__init__`（构造函数注入）传入依赖项。```python
# BAD: high-level class creates its own dependency
class ApiClient:
    def __init__(self) -> None:
        self._cache = RedisCache()   # Tightly coupled to Redis

# GOOD: depend on the abstraction; inject the concrete at call site
class ApiClient:
    def __init__(self, cache: CacheBackend) -> None:  # CacheBackend is a Protocol
        self._cache = cache

# User code (or tests):
client = ApiClient(cache=RedisCache())    # Real
client = ApiClient(cache=MemoryCache())  # Test
```
组合优于继承

比起深层继承链，更倾向于委托给包含对象。```python
# Prefer this (composition):
class YourClient:
    def __init__(self, backend: StorageBackend, http: HttpTransport) -> None:
        self._backend = backend
        self._http = http

# Avoid this (deep inheritance):
class YourClient(BaseClient, CacheMixin, RetryMixin, LoggingMixin):
    ...    # Fragile, hard to test, MRO confusion
```
异常层次结构

总是为你的包定义一个基异常；把细节放在下面。```python
# your_package/exceptions.py
class YourPackageError(Exception):
    """Base exception — catch this to catch any package error."""

class ConfigurationError(YourPackageError):
    """Raised when package is misconfigured."""

class AuthenticationError(YourPackageError):
    """Raised on auth failure."""

class RateLimitError(YourPackageError):
    """Raised when rate limit is exceeded."""
    def __init__(self, retry_after: int) -> None:
        self.retry_after = retry_after
        super().__init__(f"Rate limited. Retry after {retry_after}s.")
```
---

# # 2。类型提示最佳实践

遵循PEP 484（类型提示），PEP 526（变量注释），PEP 544（协议），
PEP 561（类型化包）。对于高质量的库来说，这些都不是可选的。```python
from __future__ import annotations    # Enables PEP 563 deferred evaluation — always add this

# For ARGUMENTS: prefer abstract / protocol types (more flexible for callers)
from collections.abc import Iterable, Mapping, Sequence, Callable

def process_items(items: Iterable[str]) -> list[int]: ...   # ✓ Accepts any iterable
def process_items(items: list[str]) -> list[int]: ...       # ✗ Too restrictive

# For RETURN TYPES: prefer concrete types (callers know exactly what they get)
def get_names() -> list[str]: ...                           # ✓ Concrete
def get_names() -> Iterable[str]: ...                       # ✗ Caller can't index it

# Use X | Y syntax (Python 3.10+), not Union[X, Y] or Optional[X]
def find(key: str) -> str | None: ...                       # ✓ Modern
def find(key: str) -> Optional[str]: ...                    # ✗ Old style

# None should be LAST in unions
def get(key: str) -> str | int | None: ...                  # ✓

# Avoid Any — it disables type checking entirely
def process(data: Any) -> Any: ...                          # ✗ Loses all safety
def process(data: dict[str, object]) -> dict[str, object]:  # ✓

# Use object instead of Any when a param accepts literally anything
def log(value: object) -> None: ...                         # ✓

# Avoid Union return types — they require isinstance() checks at every call site
def get_value() -> str | int: ...                           # ✗ Forces callers to branch
```
协议vs abc```python
from typing import Protocol, runtime_checkable
from abc import ABC, abstractmethod

# Use Protocol when you don't control the implementer classes (duck typing)
@runtime_checkable    # Makes isinstance() checks work at runtime
class Serializable(Protocol):
    def to_dict(self) -> dict[str, object]: ...

# Use ABC when you control the class hierarchy and want default implementations
class BaseBackend(ABC):
    @abstractmethod
    async def get(self, key: str) -> str | None: ...

    def get_or_default(self, key: str, default: str) -> str:
        result = self.get(key)
        return result if result is not None else default
```
TypeVar和泛型```python
from typing import TypeVar, Generic

T = TypeVar("T")
T_co = TypeVar("T_co", covariant=True)   # For read-only containers

class Repository(Generic[T]):
    """Type-safe generic repository."""
    def __init__(self, model_class: type[T]) -> None:
        self._store: list[T] = []

    def add(self, item: T) -> None:
        self._store.append(item)

    def get_all(self) -> list[T]:
        return list(self._store)
```
###数据类用于数据容器```python
from dataclasses import dataclass, field

@dataclass(frozen=True)   # frozen=True → immutable, hashable (good for configs/keys)
class Config:
    api_key: str
    timeout: int = 30
    headers: dict[str, str] = field(default_factory=dict)

    def __post_init__(self) -> None:
        if not self.api_key:
            raise ValueError("api_key must not be empty")
```
### TYPE_CHECKING guard（避免循环导入）```python
from __future__ import annotations
from typing import TYPE_CHECKING

if TYPE_CHECKING:
    from your_package.models import HeavyModel    # Only imported during type checking

def process(model: "HeavyModel") -> None:
    ...
```
###多个签名过载```python
from typing import overload

@overload
def get(key: str, default: None = ...) -> str | None: ...
@overload
def get(key: str, default: str) -> str: ...
def get(key: str, default: str | None = None) -> str | None:
    ...    # Single implementation handles both
```
---

# # 3。核心课程设计

你的库的主类应该有一个清晰的，最小的`__init__`，所有的默认值都是合理的
参数，并对无效输入提前引发`TypeError`/`ValueError`。这样可以避免混淆
错误发生在调用时而不是构造时。```python
# your_package/core.py
from __future__ import annotations

from your_package.exceptions import YourPackageError


class YourClient:
    """
    Main entry point for <your purpose>.

    Args:
        api_key: Required authentication credential.
        timeout: Request timeout in seconds. Defaults to 30.
        retries: Number of retry attempts. Defaults to 3.

    Raises:
        ValueError: If api_key is empty or timeout is non-positive.

    Example:
        >>> from your_package import YourClient
        >>> client = YourClient(api_key="sk-...")
        >>> result = client.process(data)
    """

    def __init__(
        self,
        api_key: str,
        timeout: int = 30,
        retries: int = 3,
    ) -> None:
        if not api_key:
            raise ValueError("api_key must not be empty")
        if timeout <= 0:
            raise ValueError("timeout must be positive")
        self._api_key = api_key
        self.timeout = timeout
        self.retries = retries

    def process(self, data: dict) -> dict:
        """
        Process data and return results.

        Args:
            data: Input dictionary to process.

        Returns:
            Processed result as a dictionary.

        Raises:
            YourPackageError: If processing fails.
        """
        ...
```
设计规则

-接受`__init__`中的所有配置，而不是分散在方法调用中。
-在施工时进行验证-快速失败并给出明确的信息。
—保持`__init__`签名稳定。用默认值添加新的**仅限关键字的**参数是倒退的
兼容的。删除或重新排序位置参数是一个破坏性的变化。

---

# # 4。工厂/建造商模式

当用户需要创建预配置的实例时，使用工厂函数。这样可以避免混乱`__init__`带有十几个关键字参数，并保持常见情况的简单性。```python
# your_package/factory.py
from __future__ import annotations

from your_package.core import YourClient
from your_package.backends.memory import MemoryBackend


def create_client(
    api_key: str,
    *,
    timeout: int = 30,
    retries: int = 3,
    backend: str = "memory",
    backend_url: str | None = None,
) -> YourClient:
    """
    Factory that returns a configured YourClient.

    Args:
        api_key: Required API key.
        timeout: Request timeout in seconds.
        retries: Number of retry attempts.
        backend: Storage backend type. One of 'memory' or 'redis'.
        backend_url: Connection URL for the chosen backend.

    Example:
        >>> client = create_client(api_key="sk-...", backend="redis", backend_url="redis://localhost")
    """
    if backend == "redis":
        from your_package.backends.redis import RedisBackend
        _backend = RedisBackend(url=backend_url or "redis://localhost:6379")
    else:
        _backend = MemoryBackend()

    return YourClient(api_key=api_key, timeout=timeout, retries=retries, backend=_backend)
```
**为什么是工厂而不是类方法？**都可以。独立的工厂函数更容易
在测试中模拟，避免将工厂逻辑耦合到类本身。

---

# # 5。配置模式

使用一个数据类（或Pydantic`BaseModel`）来保存配置。这给了你免费的验证，
有用的错误消息，以及记录每个选项的单一位置。```python
# your_package/config.py
from __future__ import annotations
from dataclasses import dataclass, field


@dataclass
class YourSettings:
    """
    Configuration for YourClient.

    Attributes:
        timeout: HTTP timeout in seconds.
        retries: Number of retry attempts on transient errors.
        base_url: Base API URL.
    """
    timeout: int = 30
    retries: int = 3
    base_url: str = "https://api.example.com"
    extra_headers: dict[str, str] = field(default_factory=dict)

    def __post_init__(self) -> None:
        if self.timeout <= 0:
            raise ValueError("timeout must be positive")
        if self.retries < 0:
            raise ValueError("retries must be non-negative")
```
如果需要加载环境变量，请使用`pydantic-settings`作为可选的依赖项
在`[project.optional-dependencies]`中声明它，而不是作为必需的深度。

---

# # 6。`__init__.py`-显式公共API

定义良好的`__all__`不仅仅是样式—它告诉用户（和ide）什么是您的代码的一部分
公共API，并防止意外导入内部帮助器作为契约的一部分。```python
# your_package/__init__.py
"""your-package: <one-line description>."""

from importlib.metadata import version, PackageNotFoundError

try:
    __version__ = version("your-package")
except PackageNotFoundError:
    __version__ = "0.0.0-dev"

from your_package.core import YourClient
from your_package.config import YourSettings
from your_package.exceptions import YourPackageError

__all__ = [
    "YourClient",
    "YourSettings",
    "YourPackageError",
    "__version__",
]
```
规则:
-只导出用户应该使用的内容。内部帮助程序位于`_utils.py`或子模块中。
—将导入保持在`__init__.py`的顶层浅层—避免导入重的可选深度
（如`redis`）在模块级。在需要它们的类或函数中惰性地导入它们。
-`__version__`始终是公共API的一部分-它使`your_package.__version__`为
调试。

---

# # 7。可选后端（插件模式）

这种模式可以让你的包在内存后端上开箱即用（不需要额外的深度），
同时允许高级用户插入Redis、数据库或任何自定义存储。

抽象基类——定义接口```python
# your_package/backends/__init__.py
from abc import ABC, abstractmethod


class BaseBackend(ABC):
    """Abstract storage backend interface.

    Implement this to add a custom backend (database, cache, etc.).
    """

    @abstractmethod
    async def get(self, key: str) -> str | None:
        """Retrieve a value by key. Returns None if not found."""
        ...

    @abstractmethod
    async def set(self, key: str, value: str, ttl: int | None = None) -> None:
        """Store a value. Optional TTL in seconds."""
        ...

    @abstractmethod
    async def delete(self, key: str) -> None:
        """Delete a key."""
        ...
```
内存后端-零额外深度```python
# your_package/backends/memory.py
from __future__ import annotations

import asyncio
import time
from your_package.backends import BaseBackend


class MemoryBackend(BaseBackend):
    """Thread-safe in-memory backend. Works out of the box — no extra dependencies."""

    def __init__(self) -> None:
        self._store: dict[str, tuple[str, float | None]] = {}
        self._lock = asyncio.Lock()

    async def get(self, key: str) -> str | None:
        async with self._lock:
            entry = self._store.get(key)
            if entry is None:
                return None
            value, expires_at = entry
            if expires_at is not None and time.time() > expires_at:
                del self._store[key]
                return None
            return value

    async def set(self, key: str, value: str, ttl: int | None = None) -> None:
        async with self._lock:
            expires_at = time.time() + ttl if ttl is not None else None
            self._store[key] = (value, expires_at)

    async def delete(self, key: str) -> None:
        async with self._lock:
            self._store.pop(key, None)
```
Redis后端-如果没有安装，会抛出clear importterror

关键设计：在`__init__`中惰性地导入`redis`，而不是在模块级别。这种方式,
即使没有安装`redis`，`import your_package`也不会失败。```python
# your_package/backends/redis.py
from __future__ import annotations
from your_package.backends import BaseBackend

try:
    import redis.asyncio as aioredis
except ImportError as exc:
    raise ImportError(
        "Redis backend requires the redis extra:\n"
        "  pip install your-package[redis]"
    ) from exc


class RedisBackend(BaseBackend):
    """Redis-backed storage for distributed/multi-process deployments."""

    def __init__(self, url: str = "redis://localhost:6379") -> None:
        self._client = aioredis.from_url(url, decode_responses=True)

    async def get(self, key: str) -> str | None:
        return await self._client.get(key)

    async def set(self, key: str, value: str, ttl: int | None = None) -> None:
        await self._client.set(key, value, ex=ttl)

    async def delete(self, key: str) -> None:
        await self._client.delete(key)
```
5.4用户如何选择后端```python
# Default: in-memory, no extra deps needed
from your_package import YourClient
client = YourClient(api_key="sk-...")

# Redis: pip install your-package[redis]
from your_package.backends.redis import RedisBackend
client = YourClient(api_key="sk-...", backend=RedisBackend(url="redis://localhost:6379"))
```
