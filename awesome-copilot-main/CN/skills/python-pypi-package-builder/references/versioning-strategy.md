版本控制策略- PEP 440， SemVer和决策引擎

##目录
1. [PEP 440- The Standard]（#1-pep-440- The - Standard）
2. [语义版本控制（SemVer）]（#2- Semantic -version - SemVer）
3. (预发布标识符)(# 3-pre-release-identifiers)
4. [版本决策引擎]（#4-version - Decision - Engine）
5. [动态版本控制-setuptools_scm（推荐）]（#5-动态版本控制-setuptools_scm- Recommended）
6. [hatch- with-hatch-vcs- Plugin]（#6-hatch -with-hatch-vcs- Plugin）
7. [Static - Versioning -flit]（#7-static-versioning- flit）
8. [静态版本控制-孵化手册]（#8-static-versioning- hatch -manual）
9. （#9-do-not-hardcode-version-except-flit）
10. [依赖版本说明符]（#10-dependency-version-specifiers）
11. [PyPA发布命令]（#11-pypa-release-commands）

---

# # 1。PEP 440 -标准所有Python包版本必须符合[PEP 440]（https://peps.python.org/pep-0440/）。
不兼容的版本（例如，`1.0-beta`,`2023.1.1.dev`）将被PyPI拒绝。```
Canonical form:  N[.N]+[{a|b|rc}N][.postN][.devN]

1.0.0            Stable release
1.0.0a1          Alpha pre-release
1.0.0b2          Beta pre-release
1.0.0rc1         Release candidate
1.0.0.post1      Post-release (packaging fix; same codebase)
1.0.0.dev1       Development snapshot — DO NOT upload to PyPI
2.0.0            Major release (breaking changes)
```
###纪元前缀（罕见）```
1!1.0.0          Epoch 1; used when you need to skip ahead of an old scheme
```
只将epoch作为最后的手段来修复损坏的版本序列。

---

# # 2。语义版本控制（SemVer）

SemVer干净地映射到PEP 440。总是使用`MAJOR.MINOR.PATCH`：```
MAJOR  Increment when you make incompatible API changes (rename, remove, break)
MINOR  Increment when you add functionality backward-compatibly (new features)
PATCH  Increment when you make backward-compatible bug fixes

Examples:
  1.0.0 → 1.0.1   Bug fix, no API change
  1.0.0 → 1.1.0   New method added; existing API intact
  1.0.0 → 2.0.0   Public method renamed or removed
```
什么是突破性的改变？

b|改变b|打破？||---|---|
|重命名公共函数| YES -`MAJOR`|
|删除参数|是-`MAJOR`|
|添加所需参数|是-`MAJOR`|
|添加可选参数，默认为| NO -`MINOR`|
添加一个新的function/class| NO -`MINOR`|
|修复| NO -`PATCH`|
|更新依赖下限| NO（通常）-`PATCH`|
|更新依赖上限（打破）| YES -`MAJOR`|

---

# # 3。预发布标识符

在稳定发布之前，使用预发布版本来获得用户反馈。
pip默认**不**安装预发行版（`pip install pkg`跳过它们）。
用户必须选择：`pip install "pkg==2.0.0a1"`或`pip install --pre pkg`。```
1.0.0a1    Alpha-1: very early; expect bugs; API may change
1.0.0b1    Beta-1:  feature-complete; API stabilising; seek broader feedback
1.0.0rc1   Release candidate: code-frozen; final testing before stable
1.0.0      Stable: ready for production
```
增量规则```
Start:       1.0.0a1
More alphas: 1.0.0a2, 1.0.0a3
Move to beta: 1.0.0b1  (reset counter)
Move to RC:  1.0.0rc1
Stable:      1.0.0
```
---

# # 4。版本决策引擎

在编写任何代码之前，使用此决策树选择正确的版本控制策略。```
Is the project using git and tagging releases with version tags?
├── YES → setuptools + setuptools_scm  (DEFAULT — best for most projects)
│         Git tag v1.0.0 becomes the installed version automatically.
│         Zero manual version bumping.
│
└── NO — Is the project a simple, single-module library with infrequent releases?
          ├── YES → flit
          │         Set __version__ = "1.0.0" in __init__.py.
          │         Update manually before each release.
          │
          └── NO — Does the team want an integrated build + dep management tool?
                    ├── YES → poetry
                    │         Manage version in [tool.poetry] version field.
                    │
                    └── NO → hatchling (modern, fast, pure-Python)
                              Use hatch-vcs plugin for dynamic versioning
                              or set version manually in [project].

Does the package have C/Cython/Fortran extensions?
└── YES (always) → setuptools (only backend with native extension support)
```
汇总表

|后端|版本源|最适合||---|---|---|
|`setuptools`+`setuptools_scm`| Git标签-全自动|默认为新项目|
|`hatchling`+`hatch-vcs`| Git标签-自动通过插件|孵化用户|
|`flit`|`__version__`in`__init__.py`|非常简单，最小配置|
|`poetry`|`[tool.poetry] version`field |集成深度+构建管理|
|`hatchling`manual |`[project] version`field |一次性静态版本控制|

---

# # 5。动态版本控制- setuptools_scm（推荐）`setuptools_scm`读取当前的git标签，并在构建时计算版本。
没有单独的`__version__`更新步骤-只是标记和推送。

###`pyproject.toml`配置```toml
[build-system]
requires      = ["setuptools>=70", "setuptools_scm>=8"]
build-backend = "setuptools.backends.legacy:build"

[project]
name    = "your-package"
dynamic = ["version"]

[tool.setuptools_scm]
version_scheme = "post-release"
local_scheme   = "no-local-version"   # Prevents +g<hash> from breaking PyPI
```
`__init__.py`-正确的版本访问```python
# your_package/__init__.py
from importlib.metadata import version, PackageNotFoundError

try:
    __version__ = version("your-package")
except PackageNotFoundError:
    # Package is not installed (running from a source checkout without pip install -e .)
    __version__ = "0.0.0.dev0"

__all__ = ["__version__"]
```
如何计算版本```
git tag v1.0.0            →  installed_version = "1.0.0"
3 commits after v1.0.0    →  installed_version = "1.0.0.post3+g<hash>"  (dev only)
git tag v1.1.0            →  installed_version = "1.1.0"
```
使用`local_scheme = "no-local-version"`，`+g<hash>`后缀将为PyPI删除
上传，同时仍然是可见的本地。

关键的CI需求```yaml
- uses: actions/checkout@v4
  with:
    fetch-depth: 0    # REQUIRED — without this, git has no tag history
                      # setuptools_scm falls back to 0.0.0+d<date> silently
```
**安装或构建包的每个CI作业必须具有`fetch-depth: 0`。

调试版本问题```bash
# Check what version setuptools_scm would produce right now:
python -m setuptools_scm

# If you see 0.0.0+d... it means:
# 1. No tags reachable from HEAD, OR
# 2. fetch-depth: 0 was not set in CI
```
---

# # 6。孵化与孵化vcs插件

对于已经使用hatchling的团队来说，setuptools_scm的替代方案。```toml
[build-system]
requires      = ["hatchling", "hatch-vcs"]
build-backend = "hatchling.build"

[project]
name    = "your-package"
dynamic = ["version"]

[tool.hatch.version]
source = "vcs"

[tool.hatch.build.hooks.vcs]
version-file = "src/your_package/_version.py"
```
访问版本的方式与setuptools_scm相同：```python
from importlib.metadata import version, PackageNotFoundError
try:
    __version__ = version("your-package")
except PackageNotFoundError:
    __version__ = "0.0.0.dev0"
```
---

# # 7。静态版本控制-切换

只对简单的单模块包使用flit，在这种情况下，手动升级版本是可以接受的。### `pyproject.toml`

```toml
[build-system]
requires      = ["flit_core>=3.9"]
build-backend = "flit_core.buildapi"

[project]
name    = "your-package"
dynamic = ["version", "description"]
```

### `__init__.py`

```python
"""your-package — a focused, single-purpose utility."""
__version__ = "1.2.0"   # flit reads this; update manually before each release
```
**flit异常：**这是硬编码`__version__`正确的唯一情况。
Flit通过导入`__init__.py`并读取`__version__`来发现版本。

###释放flit流```bash
# 1. Bump __version__ in __init__.py
# 2. Update CHANGELOG.md
# 3. Commit
git add src/your_package/__init__.py CHANGELOG.md
git commit -m "chore: release v1.2.0"
# 4. Tag (flit can also publish directly)
git tag v1.2.0
git push origin v1.2.0
# 5. Build and publish
flit publish
# OR
python -m build && twine upload dist/*
```
---

# # 8。静态版本控制-孵化手册```toml
[build-system]
requires      = ["hatchling"]
build-backend = "hatchling.build"

[project]
name    = "your-package"
version = "1.0.0"   # Manual; update before each release
```
在每次发布之前更新`pyproject.toml`中的`version`。不需要`__version__`（像往常一样通过`importlib.metadata.version()`访问）。

---

# # 9。不要硬编码版本（除了flit）

当**不**使用flit时，在`__init__.py`中硬编码`__version__`会创建双源
随时间发散的真相。```python
# BAD — when using setuptools_scm, hatchling, or poetry:
__version__ = "1.0.0"    # gets stale; diverges from the installed package version

# GOOD — works for all backends except flit:
from importlib.metadata import version, PackageNotFoundError
try:
    __version__ = version("your-package")
except PackageNotFoundError:
    __version__ = "0.0.0.dev0"
```
---

# # 10。依赖版本说明

选择正确的说明符样式以避免毒害用户环境。```toml
# [project] dependencies — library best practices:

"httpx>=0.24"            # Minimum only — PREFERRED; lets users upgrade freely
"httpx>=0.24,<2.0"       # Upper bound only when a known breaking change exists in next major
"requests>=2.28,<3.0"    # Acceptable for well-known major-version breaks

# Application / CLI (pinning is fine):
"httpx==0.27.2"          # Lock exact version for reproducible deploys

# NEVER in a library:
# "httpx~=0.24.0"        # Too tight; blocks minor upgrades
# "httpx==0.27.*"        # Not valid PEP 440
# "httpx"                # No constraint; fragile against future breakage
```
---

# # 11。PyPA释放命令

从代码到用户安装的规范顺序。```bash
# Step 1: Tag the release (triggers CI publish.yml automatically if configured)
git tag -a v1.2.3 -m "Release v1.2.3"
git push origin v1.2.3

# Step 2 (manual fallback only): Build locally
python -m build
# Produces:
#   dist/your_package-1.2.3.tar.gz   (sdist)
#   dist/your_package-1.2.3-py3-none-any.whl  (wheel)

# Step 3: Validate
twine check dist/*

# Step 4: Test on TestPyPI first (first release or major change)
twine upload --repository testpypi dist/*
pip install --index-url https://test.pypi.org/simple/ --extra-index-url https://pypi.org/simple/ your-package==1.2.3

# Step 5: Publish to production PyPI
twine upload dist/*
# OR via GitHub Actions (recommended):
# push the tag → publish.yml runs → pypa/gh-action-pypi-publish handles upload via OIDC

# Step 6: Verify
pip install your-package==1.2.3
python -c "import your_package; print(your_package.__version__)"
```
