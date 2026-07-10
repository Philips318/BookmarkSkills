#测试和代码质量

##目录
1. (conftest.py) (# 1-conftestpy)
2. (单元测试)(# 2-unit-tests)
3. [后端单元测试]（#3-backend-unit-tests）
4. (运行测试)(# 4-running-tests)
5. 代码质量工具（#5-code-quality-tools）
6. (导向钩子)(# 6-pre-commit-hooks)

---## 1. `conftest.py`
使用`conftest.py`定义共享fixture。保持固定装置集中-每个关注一个固定装置。
对于异步测试，在`pyproject.toml`中使用`pytest-asyncio`和`asyncio_mode = "auto"`。```python
# tests/conftest.py
import pytest
from your_package.core import YourClient
from your_package.backends.memory import MemoryBackend


@pytest.fixture
def memory_backend() -> MemoryBackend:
    return MemoryBackend()


@pytest.fixture
def client(memory_backend: MemoryBackend) -> YourClient:
    return YourClient(
        api_key="test-key",
        backend=memory_backend,
    )
```
---

# # 2。单元测试

测试快乐路径和边缘情况（例如无效输入，错误条件）。```python
# tests/test_core.py
import pytest
from your_package import YourClient
from your_package.exceptions import YourPackageError


def test_client_creates_with_valid_key():
    client = YourClient(api_key="sk-test")
    assert client is not None


def test_client_raises_on_empty_key():
    with pytest.raises(ValueError, match="api_key"):
        YourClient(api_key="")


def test_client_raises_on_invalid_timeout():
    with pytest.raises(ValueError, match="timeout"):
        YourClient(api_key="sk-test", timeout=-1)


@pytest.mark.asyncio
async def test_process_returns_expected_result(client: YourClient):
    result = await client.process({"input": "value"})
    assert "output" in result


@pytest.mark.asyncio
async def test_process_raises_on_invalid_input(client: YourClient):
    with pytest.raises(YourPackageError):
        await client.process({})  # empty input should fail
```
---

# # 3。后端单元测试

独立测试每个后端，与库的其余部分隔离。这会导致失败
更容易诊断并确保您的抽象接口实际上正确实现。```python
# tests/test_backends.py
import pytest
from your_package.backends.memory import MemoryBackend


@pytest.mark.asyncio
async def test_set_and_get():
    backend = MemoryBackend()
    await backend.set("key1", "value1")
    result = await backend.get("key1")
    assert result == "value1"


@pytest.mark.asyncio
async def test_get_missing_key_returns_none():
    backend = MemoryBackend()
    result = await backend.get("nonexistent")
    assert result is None


@pytest.mark.asyncio
async def test_delete_removes_key():
    backend = MemoryBackend()
    await backend.set("key1", "value1")
    await backend.delete("key1")
    result = await backend.get("key1")
    assert result is None


@pytest.mark.asyncio
async def test_ttl_expires_entry():
    import asyncio
    backend = MemoryBackend()
    await backend.set("key1", "value1", ttl=1)
    await asyncio.sleep(1.1)
    result = await backend.get("key1")
    assert result is None


@pytest.mark.asyncio
async def test_different_keys_are_independent():
    backend = MemoryBackend()
    await backend.set("key1", "a")
    await backend.set("key2", "b")
    assert await backend.get("key1") == "a"
    assert await backend.get("key2") == "b"
    await backend.delete("key1")
    assert await backend.get("key2") == "b"
```
---

# # 4。运行测试```bash
pip install -e ".[dev]"
pytest                           # All tests
pytest --cov --cov-report=html   # With HTML coverage report (opens in browser)
pytest -k "test_middleware"      # Filter by name
pytest -x                        # Stop on first failure
pytest -v                        # Verbose output
```
`pyproject.toml`中的覆盖率配置强制执行一个最小阈值（`fail_under = 80`）。CI将
如果低于它就会失败，它会自动捕获覆盖回归。

---

# # 5。代码质量工具

Ruff （linting -取代flake8， pylint等）```bash
pip install ruff
ruff check .           # Check for issues
ruff check . --fix     # Auto-fix safe issues
```
Ruff非常快，并取代了大多数Python检查生态系统。在`pyproject.toml`—完整配置请参见`references/pyproject-toml.md`。

###黑色（格式化）```bash
pip install black
black .                # Format all files
black . --check        # CI mode — reports issues without modifying files
```
### isort（导入排序）```bash
pip install isort
isort .                # Sort imports
isort . --check-only   # CI mode
```
总是在`[tool.isort]`中设置`profile = "black"`-否则会发生黑色和排序冲突。

### mypy（静态类型检查）```bash
pip install mypy
mypy your_package/   # Type-check your package source only
```
常见的修复:

-`ignore_missing_imports = true`-忽略未类型化的第三方深度
-`from __future__ import annotations`-启用PEP 563延迟计算（Python 3.9 compat）
-`pip install types-redis`- redis库的类型存根

###同时运行```bash
ruff check . && black . --check && isort . --check-only && mypy your_package/
```
---

# # 6。导向钩子

预提交在每次提交之前自动运行所有质量工具，因此问题永远不会到达CI。
使用`pre-commit install`每个克隆安装一次。```yaml
# .pre-commit-config.yaml
repos:
  - repo: https://github.com/astral-sh/ruff-pre-commit
    rev: v0.4.4
    hooks:
      - id: ruff
        args: [--fix]
      - id: ruff-format

  - repo: https://github.com/psf/black
    rev: 24.4.2
    hooks:
      - id: black

  - repo: https://github.com/pycqa/isort
    rev: 5.13.2
    hooks:
      - id: isort

  - repo: https://github.com/pre-commit/mirrors-mypy
    rev: v1.10.0
    hooks:
      - id: mypy
        additional_dependencies: [types-redis]  # Add stubs for typed dependencies

  - repo: https://github.com/pre-commit/pre-commit-hooks
    rev: v4.6.0
    hooks:
      - id: trailing-whitespace
      - id: end-of-file-fixer
      - id: check-yaml
      - id: check-toml
      - id: check-merge-conflict
      - id: debug-statements
      - id: no-commit-to-branch
        args: [--branch, master, --branch, main]
```

```bash
pip install pre-commit
pre-commit install           # Install once per clone
pre-commit run --all-files   # Run all hooks manually (useful before the first install)
```
`no-commit-to-branch`钩子可以防止意外地直接提交到`main`/`master`。
这将绕过CI检查。总是在一个特性分支上工作。