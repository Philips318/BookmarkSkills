---
name: python-pypi-package-builder
description: 'End-to-end skill for building, testing, linting, versioning, and publishing a production-grade Python library to PyPI. Covers all four build backends (setuptools+setuptools_scm, hatchling, flit, poetry), PEP 440 versioning, semantic versioning, dynamic git-tag versioning, OOP/SOLID design, type hints (PEP 484/526/544/561), Trusted Publishing (OIDC), and the full PyPA packaging flow. Use for: creating Python packages, pip-installable SDKs, CLI tools, framework plugins, pyproject.toml setup, py.typed, setuptools_scm, semver, mypy, pre-commit, GitHub Actions CI/CD, or PyPI publishing.'
---
# Python PyPI包生成器技能

一个完整的，经过实战测试的指南，用于构建，测试，检查，版本控制，类型和
向PyPI发布一个生产级Python库——从首次提交到社区就绪
释放。

> **AI代理指令：**在编写一行代码或之前，请阅读整个文件
>创建任何文件。每个决策-布局，后端，版本策略，模式，CI -
b>在这里有一个决策规则。按照顺序遵循决策树。此技能适用于任何人
> Python包类型（实用工具、SDK、CLI、插件、数据库）。不要跳过部分。

---

##快速导航

在这个文件| |部分覆盖||---|---|
|(1。技能触发[#1-skill-trigger] |何时加载此技能|
|(2。包类型决定[#2-package-type-decision]确定你正在构建什么
|(3。（#3-folder-structure-decision） | / / / / / / / / / / /
|(4。（#4-build-backend-decision） | setuptools / hatchling / flit / poetry |
|(5。（#5- PyPA - package - Flow） |规范发布管道|
|(6。（#6-project-structure-templates） |每个选项的完整布局|
|(7。[版本控制策略](#7-version - Strategy) | PEP 440, semver，动态vs静态|

|参考文件|它涵盖什么||---|---|
|`references/pyproject-toml.md`|所有四个后端模板，`setuptools_scm`,`py.typed`，工具配置|
|`references/library-patterns.md`|OOP/SOLID，类型提示，核心类设计，工厂，协议，CLI |
|`references/testing-quality.md`|`conftest.py`，unit/backend/async测试，ruff/mypy/pre-commit|
|`references/ci-publishing.md`|`ci.yml`,`publish.yml`，可信发布，TestPyPI， CHANGELOG，发布清单|
自述文件，文档字符串，贡献，安全，反模式，主检查表
|`references/architecture-patterns.md`|后端系统（plugin/strategy），配置层，传输层，CLI，后端注入|
PEP 440, SemVer，预发布版，setuptools_scm deep-dive, flit static，决策引擎|
|`references/release-governance.md`|分支策略，分支保护，OIDC，标签作者验证，防止无效标签|
|`references/tooling-ruff.md`| Ruff-only设置（取代black/isort）， mypy配置，预提交，asyncio_mode=auto |

**脚手架脚本：**运行`python skills/python-pypi-package-builder/scripts/scaffold.py --name your-package-name`在一个命令中生成整个目录布局、存根文件和`pyproject.toml`。

---# # 1。技能触发

在用户想要的时候加载这个技能：

-创建、支撑或发布Python包或库到PyPI
-构建可pip安装的SDK、实用程序、CLI工具或框架扩展
-为Python项目设置`pyproject.toml`， linting, mypy， pre-commit或GitHub Actions-了解版本控制（`setuptools_scm`, PEP 440, semver，静态版本控制）
-了解PyPA规格：`py.typed`，`MANIFEST.in`,`RECORD`，分类器
-使用可信发布（OIDC）或API令牌发布到PyPI
-重构现有包以遵循现代Python打包标准
在Python库中添加类型提示、协议、abc或数据类
-在Python包中应用OOP/SOLID设计模式
-选择构建后端（setuptools, hatchling, flit, poetry）**也可以触发像这样的短语：**“构建Python SDK”，“发布我的库”，“设置PyPI CI”，
“创建一个pip包”，“如何发布到PyPI”，“pyproject.toml帮助”，“PEP 561输入”，
“setuptools_scm version”， “semver Python”， “PEP 440”， “git tag release”， “Trusted Publishing”。

---

# # 2。包装类型决定

在编写任何代码之前，确定用户正在构建什么。每种类型都有不同的模式。

决策表

|类型|核心模式|入口点|关键节点|样例包||---|---|---|---|---|
| **实用程序库** |纯函数+助手模块|只导入API |最小|`arrow`，`humanize`,`boltons`,`more-itertools`|
| **API客户端/ SDK** |类与方法，auth，重试逻辑|只导入API |`httpx`或`requests`|`boto3`，`stripe-python`,`openai`|
| **CLI工具** |命令功能+参数解析器|`[project.scripts]`或`[project.entry-points]`|`click`或`typer`|`black`、`ruff`、`httpie`、`rich`|
| **框架插件** |插件类，钩子注册|`[project.entry-points."framework.plugin"]`|框架深度|`pytest-*`，`django-*`,`flask-*`|
| **数据处理库** |类+函数管道|只导入API |可选：`numpy`、`pandas`|`pydantic`、`marshmallow`、`cerberus`|
| **混合/通用** |以上|变化|变化|许多真实世界的包|**决策原则：**询问用户是否不清楚。包可以组合类型（例如，SDK与CLI）
（入口点）——使用主要类型进行结构决策，并在上面添加次要类型模式。

有关每种类型的实现模式，请参见`references/library-patterns.md`。

包命名规则

—PyPI名称：全小写，连字符
—Python导入名称：下划线—`my_python_library`—启动前检查可用性：https://pypi.org/search/-避免隐藏流行的软件包（首先验证`pip install <name>`失败）

---

# # 3。文件夹结构决定

决策树```
Does the package have 5+ internal modules OR multiple contributors OR complex sub-packages?
├── YES → Use src/ layout
│         Reason: prevents accidental import of uninstalled code during development;
│         separates source from project root files; PyPA-recommended for large projects.
│
├── NO → Is it a single-module, focused package (e.g., one file + helpers)?
│         ├── YES → Use flat layout
│         └── NO (medium complexity) → Use flat layout, migrate to src/ if it grows
│
└── Is it multiple related packages under one namespace (e.g., myorg.http, myorg.db)?
          └── YES → Use namespace/monorepo layout
```
快速规则总结

|使用||---|---|
新项目，未知未来大小|`src/`布局（最安全的默认）|
|单用途，1-4模块|平面布局|
|大型库，许多贡献者|`src/`布局|
|一个repo中的多个包| Namespace / monorepo |
|旧单位迁移项目|保持单位不变；在下一个主要版本|时迁移到`src/`---

# # 4。构建后端决策

决策树```
Does the user need version derived automatically from git tags?
├── YES → Use setuptools + setuptools_scm
│         (git tag v1.0.0 → that IS your release workflow)
│
└── NO → Does the user want an all-in-one tool (deps + build + publish)?
          ├── YES → Use poetry (v2+ supports standard [project] table)
          │
          └── NO → Is the package pure Python with no C extensions?
                    ├── YES, minimal config preferred → Use flit
                    │   (zero config, auto-discovers version from __version__)
                    │
                    └── YES, modern & fast preferred → Use hatchling
                        (zero-config, plugin system, no setup.py needed)

Does the package have C/Cython/Fortran extensions?
└── YES → MUST use setuptools (only backend with full native extension support)
```
后端比较

|后端|版本源|配置| C扩展|最适合||---|---|---|---|---|
|`setuptools`+`setuptools_scm`| git标签（自动）|`pyproject.toml`+可选`setup.py`shim |是|项目与git标签发布；任何复杂性|
|`hatchling`| manual或plugin |`pyproject.toml`only | No |新的纯python项目；又快又现代
|`flit`|`__version__`in`__init__.py`|`pyproject.toml`only | No |非常简单的单模块封装|
|`poetry`|`pyproject.toml`field |`pyproject.toml`| No |团队想要整合深度管理|

有关所有四个完整的`pyproject.toml`模板，请参见`references/pyproject-toml.md`。

---

# # 5。PyPA包装流程

这是从源代码到用户安装的规范端到端流程。
**在发行前必须理解每一步```
1. SOURCE TREE
   Your code in version control (git)
   └── pyproject.toml describes metadata + build system

2. BUILD
   python -m build
   └── Produces two artifacts in dist/:
       ├── *.tar.gz   → source distribution (sdist)
       └── *.whl      → built distribution (wheel) — preferred by pip

3. VALIDATE
   twine check dist/*
   └── Checks metadata, README rendering, and PyPI compatibility

4. TEST PUBLISH (first release only)
   twine upload --repository testpypi dist/*
   └── Verify: pip install --index-url https://test.pypi.org/simple/ your-package

5. PUBLISH
   twine upload dist/*          ← manual fallback
   OR GitHub Actions publish.yml  ← recommended (Trusted Publishing / OIDC)

6. USER INSTALL
   pip install your-package
   pip install "your-package[extra]"
```
关键PyPA概念

概念|意思是什么|---|---|
| **sdist** |源分发-你的源+元数据；无轮可用时使用|
| **轮子whl)** |预构建二进制- pip直接提取到站点包；没有构建步骤|
| **PEP517/518** |通过`pyproject.toml [build-system]`表|标准构建系统接口
| **PEP 621** |标准`[project]`表在`pyproject.toml`；所有现代后端都支持它
| **PEP 639** |`license`键作为SPDX字符串（例如，`"MIT"`,`"Apache-2.0"`） -而不是`{text = "MIT"}`|
| **PEP 561** |`py.typed`空标记文件-告诉mypy/IDEs这个包发送类型信息|

有关完整的CI工作流和发布设置，请参见`references/ci-publishing.md`。

---

# # 6。项目结构模板

src/ Layout（新项目的默认设置）```
your-package/
├── src/
│   └── your_package/
│       ├── __init__.py           # Public API: __all__, __version__
│       ├── py.typed              # PEP 561 marker — EMPTY FILE
│       ├── core.py               # Primary implementation
│       ├── client.py             # (API client type) or remove
│       ├── cli.py                # (CLI type) click/typer commands, or remove
│       ├── config.py             # Settings / configuration dataclass
│       ├── exceptions.py         # Custom exception hierarchy
│       ├── models.py             # Data classes, Pydantic models, TypedDicts
│       ├── utils.py              # Internal helpers (prefix _utils if private)
│       ├── types.py              # Shared type aliases and TypeVars
│       └── backends/             # (Plugin pattern) — remove if not needed
│           ├── __init__.py       # Protocol / ABC interface definition
│           ├── memory.py         # Default zero-dep implementation
│           └── redis.py          # Optional heavy implementation
├── tests/
│   ├── __init__.py
│   ├── conftest.py               # Shared fixtures
│   ├── unit/
│   │   ├── __init__.py
│   │   ├── test_core.py
│   │   ├── test_config.py
│   │   └── test_models.py
│   ├── integration/
│   │   ├── __init__.py
│   │   └── test_backends.py
│   └── e2e/                      # Optional: end-to-end tests
│       └── __init__.py
├── docs/                         # Optional: mkdocs or sphinx
├── scripts/
│   └── scaffold.py
├── .github/
│   ├── workflows/
│   │   ├── ci.yml
│   │   └── publish.yml
│   └── ISSUE_TEMPLATE/
│       ├── bug_report.md
│       └── feature_request.md
├── .pre-commit-config.yaml
├── pyproject.toml
├── CHANGELOG.md
├── CONTRIBUTING.md
├── SECURITY.md
├── LICENSE
├── README.md
└── .gitignore
```
平面布局（小/集中的包装）```
your-package/
├── your_package/         # ← at root, not inside src/
│   ├── __init__.py
│   ├── py.typed
│   └── ... (same internal structure)
├── tests/
└── ... (same top-level files)
```
命名空间/单线程布局（多个相关包）```
your-org/
├── packages/
│   ├── your-org-core/
│   │   ├── src/your_org/core/
│   │   └── pyproject.toml
│   ├── your-org-http/
│   │   ├── src/your_org/http/
│   │   └── pyproject.toml
│   └── your-org-cli/
│       ├── src/your_org/cli/
│       └── pyproject.toml
├── .github/workflows/
└── README.md
```
每个子包都有自己的`pyproject.toml`。它们通过PEP 420共享`your_org`命名空间
隐式命名空间包（命名空间根目录中没有`__init__.py`）。

内部模块指南

|文件|用途|何时包含||---|---|---|
|`__init__.py`|公共API接口；再出口;`__version__`|总是|
|`py.typed`| PEP 561类型包标记（空）|总是|
|`core.py`|主类/主逻辑|总是|
|`config.py`|设置数据类或Pydantic模型|可配置时|
|`exceptions.py`|异常层次结构（`YourBaseError`→details） |总是|
|`models.py`|数据模型/ dto / TypedDicts |数据量大时|
|`utils.py`|内部帮助程序（不是公共API的一部分）|根据需要|
|`types.py`|共享`TypeVar`，`TypeAlias`，`Protocol`定义|复杂类型|时
|`cli.py`|命令行入口点（click/typer） |命令行类型只有|
|`backends/`|Plugin/strategy模式|可切换实现|
|`_compat.py`| Python版本兼容性shimshims |当3.9-3.13兼容需要|

---

# # 7。版本控制策略

PEP 440 -标准```
Canonical form:  N[.N]+[{a|b|rc}N][.postN][.devN]

Examples:
  1.0.0          Stable release
  1.0.0a1        Alpha (pre-release)
  1.0.0b2        Beta
  1.0.0rc1       Release candidate
  1.0.0.post1    Post-release (e.g., packaging fix only)
  1.0.0.dev1     Development snapshot (not for PyPI)
```
语义版本控制（推荐）```
MAJOR.MINOR.PATCH

MAJOR: Breaking API change (remove/rename public function/class/arg)
MINOR: New feature, fully backward-compatible
PATCH: Bug fix, no API change
```
使用setuptools_scm进行动态版本控制（推荐用于git-tag工作流）```bash
# How it works:
git tag v1.0.0          →  installed version = 1.0.0
git tag v1.1.0          →  installed version = 1.1.0
(commits after tag)     →  version = 1.1.0.post1  (suffix stripped for PyPI)

# In code — NEVER hardcode when using setuptools_scm:
from importlib.metadata import version, PackageNotFoundError
try:
    __version__ = version("your-package")
except PackageNotFoundError:
    __version__ = "0.0.0-dev"    # Fallback for uninstalled dev checkouts
```
所需的`pyproject.toml`配置：```toml
[tool.setuptools_scm]
version_scheme = "post-release"
local_scheme   = "no-local-version"   # Prevents +g<hash> from breaking PyPI uploads
```
**关键：**总是在每个CI检出步骤中设置`fetch-depth: 0`。没有完整的git历史记录，`setuptools_scm`找不到标记，构建版本会无声地退回到`0.0.0+dev`。

静态版本控制（flit，孵化手册，诗歌）```python
# your_package/__init__.py
__version__ = "1.0.0"    # Update this before every release
```
版本说明符对依赖项的最佳实践```toml
# In [project] dependencies:
"httpx>=0.24"            # Minimum version — PREFERRED for libraries
"httpx>=0.24,<1.0"       # Upper bound only when a known breaking change exists
"httpx==0.27.0"          # Pin exactly ONLY in applications, NOT libraries

# NEVER do this in a library — it breaks dependency resolution for users:
# "httpx~=0.24.0"        # Too tight
# "httpx==0.27.*"        # Fragile
```
版本变更→发布流程```bash
# 1. Update CHANGELOG.md — move [Unreleased] entries to [x.y.z] - YYYY-MM-DD
# 2. Commit the changelog
git add CHANGELOG.md
git commit -m "chore: prepare release vX.Y.Z"
# 3. Tag and push — this triggers publish.yml automatically
git tag vX.Y.Z
git push origin main --tags
# 4. Monitor GitHub Actions → verify on https://pypi.org/project/your-package/
```
有关所有四个后端的完整pyproject.toml模板，请参见`references/pyproject-toml.md`。

---

##下一步去哪里

在理解决策和结构之后：

1. **设置`pyproject.toml`**→`references/pyproject-toml.md`所有四个后端模板（setuptools+scm, hatchling, flit, poetry），完整的工具配置，`py.typed`设置，版本配置。

2. **编写库代码**→`references/library-patterns.md`OOP/SOLID原理，类型提示（PEP484/526/544/561），核心类设计，工厂函数，`__init__.py`，plugin/backend模式，CLI入口点。

3. **添加测试和代码质量**→`references/testing-quality.md``conftest.py`，unit/backend/async测试，参数化，ruff/mypy/pre-commit设置。

4. **设置CI/CD并发布**→`references/ci-publishing.md``ci.yml`,`publish.yml`with Trusted Publishing （OIDC，无API令牌），CHANGELOG格式
发布清单。5. **抛光community/OSS**→`references/community-docs.md`自述部分，文档字符串格式，贡献，安全，问题模板，反模式
表和主发布清单。

6. **设计后端，配置，传输，CLI**→`references/architecture-patterns.md`后端系统（plugin/strategy模式），设置数据类，HTTP传输层，
CLI用click/typer，后端注入规则。

7. **选择并实现版本控制策略**→`references/versioning-strategy.md`PEP 440规范表单，SemVer规则，预发布标识符，setuptools_scm
Flit静态版本控制，决策引擎（DEFAULT/BEGINNER/MINIMAL）。

8. **管理发布和保护发布管道**→`references/release-governance.md`分支策略、分支保护规则、OIDC可信发布设置、标签作者
CI验证、标签格式强制、完全受治理的`publish.yml`。9. **简化工具与Ruff**→`references/tooling-ruff.md`Ruff-only安装取代black/isort/flake8， mypy配置，预提交钩子，
Asyncio_mode =auto(删除@pytest.mark。Asyncio)，迁移指南。