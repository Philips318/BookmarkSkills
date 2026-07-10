#pyproject.toml，后端，版本控制和类型包

##目录
1. [完成pyproject.toml- setuptools + setuptools_scm]（#1-complete-pyprojecttoml）
2. [hatchling (modern, zero-config)]（#2-hatchling-modern-zero-config）
3. [flit (minimal, version from`__version__`)]（#3-flit-minimal-version-from-__version__）
4. [诗歌（综合深度经理）]（#4-poetry-integrated- deep -manager）
5. [版本控制策略- PEP 440， semver，深度说明符]（#5-version - Strategy）
6. （#6-dynamic-version -with-setuptools_scm）
7. [setup.pyshim用于传统的可编辑安装]（#7-setup -shim）
8. [PEP 561类型的包（py.typed）]（#8-type -package- PEP -561）

---

# # 1。完成pyproject.toml### setuptools + setuptools_scm（推荐用于git标签版本控制）```toml
[build-system]
requires = ["setuptools>=68", "wheel", "setuptools_scm"]
build-backend = "setuptools.build_meta"

[project]
name = "your-package"
dynamic = ["version"]           # Version comes from git tags via setuptools_scm
description = "<your description> — <key feature 1>, <key feature 2>"
readme = "README.md"
requires-python = ">=3.10"
license = "MIT"                # PEP 639 SPDX expression (string, not {text = "MIT"})
license-files = ["LICENSE"]
authors = [
    {name = "Your Name", email = "you@example.com"},
]
maintainers = [
    {name = "Your Name", email = "you@example.com"},
]
keywords = [
    "python",
    # Add 10-15 specific keywords that describe your library — they affect PyPI discoverability
]
classifiers = [
    "Development Status :: 3 - Alpha",          # Change to 5 at stable release
    "Intended Audience :: Developers",
    "License :: OSI Approved :: MIT License",
    "Operating System :: OS Independent",
    "Programming Language :: Python :: 3",
    "Programming Language :: Python :: 3.10",
    "Programming Language :: Python :: 3.11",
    "Programming Language :: Python :: 3.12",
    "Programming Language :: Python :: 3.13",
    "Topic :: Software Development :: Libraries :: Python Modules",
    "Typing :: Typed",          # Add this when shipping py.typed
]
dependencies = [
    # List your runtime dependencies here. Keep them minimal.
    # Example: "httpx>=0.24", "pydantic>=2.0"
    # Leave empty if your library has no required runtime deps.
]

[project.optional-dependencies]
redis = [
    "redis>=4.2",               # Optional heavy backend
]
dev = [
    "pytest>=7.0",
    "pytest-asyncio>=0.21",
    "httpx>=0.24",
    "pytest-cov>=4.0",
    "ruff>=0.4",
    "black>=24.0",
    "isort>=5.13",
    "mypy>=1.0",
    "pre-commit>=3.0",
    "build",
    "twine",
]

[project.urls]
Homepage      = "https://github.com/yourusername/your-package"
Documentation = "https://github.com/yourusername/your-package#readme"
Repository    = "https://github.com/yourusername/your-package"
"Bug Tracker" = "https://github.com/yourusername/your-package/issues"
Changelog     = "https://github.com/yourusername/your-package/blob/master/CHANGELOG.md"

# --- Setuptools configuration ---
[tool.setuptools.packages.find]
include = ["your_package*"]   # flat layout
# For src/ layout, use:
# where = ["src"]

[tool.setuptools.package-data]
your_package = ["py.typed"]  # Ship the py.typed marker in the wheel

# --- setuptools_scm: version from git tags ---
[tool.setuptools_scm]
version_scheme = "post-release"
local_scheme   = "no-local-version"  # Prevents +local suffix breaking PyPI uploads

# --- Ruff (linting) ---
[tool.ruff]
target-version = "py310"
line-length    = 100

[tool.ruff.lint]
select = ["E", "F", "W", "I", "N", "UP", "B", "SIM", "C4", "PTH", "RUF"]
ignore = ["E501"]   # Line length enforced by formatter

[tool.ruff.lint.per-file-ignores]
"tests/*"   = ["S101", "ANN"]    # Allow assert and missing annotations in tests
"scripts/*" = ["T201"]           # Allow print in scripts

[tool.ruff.format]
quote-style = "double"

# --- Black (formatting) ---
[tool.black]
line-length    = 100
target-version = ["py310", "py311", "py312", "py313"]

# --- isort (import sorting) ---
[tool.isort]
profile     = "black"
line_length = 100

# --- mypy (static type checking) ---
[tool.mypy]
python_version         = "3.10"
warn_return_any        = true
warn_unused_configs    = true
warn_unused_ignores    = true
disallow_untyped_defs  = true
disallow_any_generics  = true
ignore_missing_imports = true
strict                 = false     # Set true for maximum strictness

[[tool.mypy.overrides]]
module = "tests.*"
disallow_untyped_defs = false     # Relaxed in tests

# --- pytest ---
[tool.pytest.ini_options]
asyncio_mode  = "auto"
testpaths     = ["tests"]
pythonpath    = ["."]          # For flat layout; remove for src/
python_files  = "test_*.py"
python_classes = "Test*"
python_functions = "test_*"
addopts       = "-v --tb=short --cov=your_package --cov-report=term-missing"

# --- Coverage ---
[tool.coverage.run]
source = ["your_package"]
omit   = ["tests/*"]

[tool.coverage.report]
fail_under   = 80
show_missing = true
exclude_lines = [
    "pragma: no cover",
    "def __repr__",
    "raise NotImplementedError",
    "if TYPE_CHECKING:",
    "@abstractmethod",
]
```
---

# # 2。孵化（现代，零配置）

最适合新的纯python项目，不需要C扩展。不需要`setup.py`。使用`hatch-vcs`用于git标签版本控制，或者省略它用于手动版本颠簸。```toml
[build-system]
requires = ["hatchling", "hatch-vcs"]     # hatch-vcs for git-tag versioning
build-backend = "hatchling.build"

[project]
name = "your-package"
dynamic = ["version"]            # Remove and add version = "1.0.0" for manual versioning
description = "One-line description"
readme = "README.md"
requires-python = ">=3.10"
license = "MIT"
license-files = ["LICENSE"]
authors = [{name = "Your Name", email = "you@example.com"}]
keywords = ["python"]
classifiers = [
    "Development Status :: 3 - Alpha",
    "Intended Audience :: Developers",
    "License :: OSI Approved :: MIT License",
    "Operating System :: OS Independent",
    "Programming Language :: Python :: 3",
    "Typing :: Typed",
]
dependencies = []

[project.optional-dependencies]
dev = ["pytest>=8.0", "pytest-cov>=5.0", "ruff>=0.6", "mypy>=1.10"]

[project.urls]
Homepage  = "https://github.com/yourusername/your-package"
Changelog = "https://github.com/yourusername/your-package/blob/master/CHANGELOG.md"

# --- Hatchling build config ---
[tool.hatch.build.targets.wheel]
packages = ["src/your_package"]    # src/ layout
# packages = ["your_package"]      # ← flat layout

[tool.hatch.version]
source = "vcs"                     # git-tag versioning via hatch-vcs

[tool.hatch.version.raw-options]
local_scheme = "no-local-version"

# ruff, mypy, pytest, coverage sections — same as setuptools template above
```
---

# # 3。flit（最小，版本从`__version__`）

最适合非常简单的单模块包。零配置。直接从`your_package/__init__.py`。对于`__version__`总是需要一个静态字符串。```toml
[build-system]
requires = ["flit_core>=3.9"]
build-backend = "flit_core.buildapi"

[project]
name = "your-package"
dynamic = ["version", "description"]  # Read from __init__.py __version__ and docstring
readme = "README.md"
requires-python = ">=3.10"
license = "MIT"
authors = [{name = "Your Name", email = "you@example.com"}]
classifiers = [
    "License :: OSI Approved :: MIT License",
    "Programming Language :: Python :: 3",
    "Typing :: Typed",
]
dependencies = []

[project.urls]
Homepage = "https://github.com/yourusername/your-package"

# flit reads __version__ from your_package/__init__.py automatically.
# Ensure __init__.py has: __version__ = "1.0.0"  (static string — flit does NOT support
# importlib.metadata for dynamic version discovery)
```
---

# # 4。诗歌（集成依赖+构建管理器）

最适合想要单一工具来管理深度、构建和发布的团队。诗歌v2 +
支持标准的`[project]`表。```toml
[build-system]
requires = ["poetry-core>=2.0"]
build-backend = "poetry.core.masonry.api"

[project]
name = "your-package"
version = "1.0.0"
description = "One-line description"
readme = "README.md"
requires-python = ">=3.10"
license = "MIT"
authors = [{name = "Your Name", email = "you@example.com"}]
classifiers = [
    "Programming Language :: Python :: 3",
    "Typing :: Typed",
]
dependencies = []   # poetry v2+ uses standard [project] table

[project.optional-dependencies]
dev = ["pytest>=8.0", "ruff>=0.6", "mypy>=1.10"]

# Optional: use [tool.poetry] only for poetry-specific features
[tool.poetry.group.dev.dependencies]
# Poetry-specific group syntax (alternative to [project.optional-dependencies])
pytest = ">=8.0"
```
---

# # 5。版本控制策略

PEP 440 -标准```
Canonical form:  N[.N]+[{a|b|rc}N][.postN][.devN]

Examples:
  1.0.0          Stable release
  1.0.0a1        Alpha (pre-release)
  1.0.0b2        Beta
  1.0.0rc1       Release candidate
  1.0.0.post1    Post-release (e.g., packaging fix only — no code change)
  1.0.0.dev1     Development snapshot (NOT for PyPI)
```
语义版本控制（SemVer）——对每个库都使用它```
MAJOR.MINOR.PATCH

MAJOR: Breaking API change (remove/rename public function/class/arg)
MINOR: New feature, fully backward-compatible
PATCH: Bug fix, no API change
```
|修改|什么东西碰了|例如||---|---|---|
|删除/重命名公共函数| MAJOR |`1.2.3 → 2.0.0`|
|新增公共功能| MINOR |`1.2.3 → 1.3.0`|
| PATCH |`1.2.3 → 1.2.4`|
|新的预发布|后缀|`2.0.0a1`，`2.0.0rc1`|

代码中的版本-从包元数据中读取```python
# your_package/__init__.py
from importlib.metadata import version, PackageNotFoundError

try:
    __version__ = version("your-package")
except PackageNotFoundError:
    __version__ = "0.0.0-dev"    # Fallback for uninstalled dev checkouts
```
在使用setuptools_scm时，不要硬编码`__version__ = "1.0.0"`-它在
第一个git标签。总是使用`importlib.metadata`。

版本说明符对依赖项的最佳实践```toml
# In [project] dependencies — for a LIBRARY:
"httpx>=0.24"            # Minimum version — PREFERRED for libraries
"httpx>=0.24,<1.0"       # Upper bound only when a known breaking change exists

# ONLY for applications (never for libraries):
"httpx==0.27.0"          # Pin exactly — breaks dep resolution in libraries

# NEVER do this in a library:
# "httpx~=0.24.0"        # Compatible release operator — too tight
# "httpx==0.27.*"        # Wildcard pin — fragile
```
---

# # 6。使用`setuptools_scm`进行动态版本控制`setuptools_scm`读取你的git标签并自动设置包版本-不再手动
在每次发布前编辑版本字符串。

###它是如何工作的```
git tag v1.0.0        →  package version = 1.0.0
git tag v1.1.0        →  package version = 1.1.0
(commits after tag)   →  version = 1.1.0.post1+g<hash>  (stripped for PyPI)
```
`local_scheme = "no-local-version"`去掉`+g<hash>`后缀，所以PyPI上传不会失败
“不允许本地版本标签”错误。

运行时访问版本```python
# your_package/__init__.py
from importlib.metadata import version, PackageNotFoundError

try:
    __version__ = version("your-package")
except PackageNotFoundError:
    __version__ = "0.0.0-dev"  # Fallback for uninstalled dev checkouts
```
不要在使用setuptools_scm时硬编码`__version__ = "1.0.0"`-它将在
第一个标记。

完整的发布流程（这就是它-不需要其他任何东西）```bash
git tag v1.2.0
git push origin master --tags
# GitHub Actions publish.yml triggers automatically
```
---

# # 7。`setup.py`垫片

一些较旧的工具和ide仍然期望使用`setup.py`。把它做成三条线的衬垫——全是真的
配置保持在`pyproject.toml`。```python
# setup.py — thin shim only. All config lives in pyproject.toml.
from setuptools import setup

setup()
```
不要复制`name`、`version`、`dependencies`或`pyproject.toml`的任何其他元数据`setup.py`。如果你在那里复制任何东西，它最终会漂移并引起混乱的冲突。

---

# # 8。类型化包（PEP 561）

正确声明的类型化包意味着mypy、pyright和ide会自动选择您的类型
提示，无需用户的任何额外配置。

步骤1：创建标记文件```bash
# The file must exist; its content doesn't matter — its presence is the signal.
touch your_package/py.typed
```
###步骤2：将其包含在车轮中

已经在上面的模板中：```toml
[tool.setuptools.package-data]
your_package = ["py.typed"]
```
步骤3：添加PyPI分类器```toml
classifiers = [
    ...
    "Typing :: Typed",
]
```
###步骤4：对所有公共函数进行类型注释```python
# Good — fully typed
def process(
    self,
    data: dict[str, object],
    *,
    timeout: int = 30,
) -> dict[str, object]:
    ...

# Bad — mypy will flag this, and IDEs give no completions to users
def process(self, data, timeout=30):
    ...
```
###步骤5：验证py。在轮子上打字```bash
python -m build
unzip -l dist/your_package-*.whl | grep py.typed
# Must show: your_package/py.typed
```
如果没有，请检查`[tool.setuptools.package-data]`配置。