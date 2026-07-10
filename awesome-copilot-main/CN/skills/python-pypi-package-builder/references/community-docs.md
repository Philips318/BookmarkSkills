#社区文档、PR检查表、反模式和发布检查表

##目录
1. [README.mdrequired sections]（#1-readmemd-required-sections）
2. [Docstrings -谷歌style]（#2-docstrings- Google -style）
3. (CONTRIBUTING.md模板)(# 3-contributingmd)
4. (SECURITY.md模板)(# 4-securitymd)
5. [GitHub Issue Templates]（#5-github-issue-templates）
6. (公关清单)(# 6-pr-checklist)
7. [要避免的反模式]（#7-反模式）
8. [主发布清单]（#8-master-release-checklist）

---

# # 1。`README.md`必填项

一个好的自述文件是最重要的文件。用户在30秒内决定是否
使用基于README的库。```markdown
# your-package

> One-line description — what it does and why it's useful.

[![PyPI version](https://badge.fury.io/py/your-package.svg)](https://pypi.org/project/your-package/)
[![Python Versions](https://img.shields.io/pypi/pyversions/your-package)](https://pypi.org/project/your-package/)
[![CI](https://github.com/you/your-package/actions/workflows/ci.yml/badge.svg)](https://github.com/you/your-package/actions/workflows/ci.yml)
[![Coverage](https://codecov.io/gh/you/your-package/branch/master/graph/badge.svg)](https://codecov.io/gh/you/your-package)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

## Installation

pip install your-package

# With Redis backend:
pip install "your-package[redis]"

## Quick Start

(A copy-paste working example — no setup required to run it)

from your_package import YourClient

client = YourClient(api_key="sk-...")
result = client.process({"input": "value"})
print(result)

## Features

- Feature 1
- Feature 2

## Configuration

| Parameter | Type | Default | Description |
|---|---|---|—--|
| api_key | str | required | Authentication credential |
| timeout | int | 30 | Request timeout in seconds |
| retries | int | 3 | Number of retry attempts |

## Backends

Brief comparison — in-memory vs Redis — and when to use each.

## Contributing

See [CONTRIBUTING.md](./CONTRIBUTING.md)

## Changelog

See [CHANGELOG.md](./CHANGELOG.md)

## License

MIT — see [LICENSE](./LICENSE)
```
---

# # 2。文档字符串-谷歌样式

为每个公共类、方法和函数使用google风格的文档字符串。ide显示这些
作为工具提示，mkdocs/sphinx可以从它们自动生成文档，并且它们传达意图
显然是对贡献者。```python
class YourClient:
    """
    Main client for <purpose>.

    Args:
        api_key: Authentication credential.
        timeout: Request timeout in seconds. Defaults to 30.
        retries: Number of retry attempts. Defaults to 3.

    Raises:
        ValueError: If api_key is empty or timeout is non-positive.

    Example:
        >>> from your_package import YourClient
        >>> client = YourClient(api_key="sk-...")
        >>> result = client.process({"input": "value"})
    """
```

---

## 3. `CONTRIBUTING.md`

```markdown
# Contributing to your-package

## Development Setup

git clone https://github.com/you/your-package
cd your-package
pip install -e ".[dev]"
pre-commit install

## Running Tests

pytest

## Running Linting

ruff check .
black . --check
mypy your_package/

## Submitting a PR

1. Fork the repository
2. Create a feature branch: `git checkout -b feat/your-feature`
3. Make changes with tests
4. Ensure CI passes: `pre-commit run --all-files && pytest`
5. Update `CHANGELOG.md` under `[Unreleased]`
6. Open a PR — use the PR template

## Commit Message Format (Conventional Commits)

- `feat: add Redis backend`
- `fix: correct retry behavior on timeout`
- `docs: update README quick start`
- `chore: bump ruff to 0.5`
- `test: add edge cases for memory backend`

## Reporting Bugs

Use the GitHub issue template. Include Python version, package version,
and a minimal reproducible example.
```

---

## 4. `SECURITY.md`

```markdown
# Security Policy

## Supported Versions

| Version | Supported |
|---|---|
| 1.x.x   | Yes       |
| < 1.0   | No        |

## Reporting a Vulnerability

Do NOT open a public GitHub issue for security vulnerabilities.

Report via: GitHub private security reporting (preferred)
or email: security@yourdomain.com

Include:
- Description of the vulnerability
- Steps to reproduce
- Potential impact
- Suggested fix (if any)

We aim to acknowledge within 48 hours and resolve within 14 days.
```
---

# # 5。GitHub发布模板### `.github/ISSUE_TEMPLATE/bug_report.md`

```markdown
---
name: Bug Report
about: Report a reproducible bug
labels: bug
---

**Python version:**
**Package version:**

**Describe the bug:**

**Minimal reproducible example:**
```python
#粘贴代码```

**Expected behavior:**

**Actual behavior:**
```

### `.github/ISSUE_TEMPLATE/feature_request.md`

```markdown
---
name: Feature Request
about: Suggest a new feature or enhancement
labels: enhancement
---

**Problem this would solve:**

**Proposed solution:**

**Alternatives considered:**
```
---

# # 6。公关的清单

所有项目必须在要求审查之前进行检查。CI必须是完全绿色的。

代码质量门```
[ ] ruff check . — zero errors
[ ] black . --check — zero formatting issues
[ ] isort . --check-only — imports sorted correctly
[ ] mypy your_package/ — zero type errors
[ ] pytest — all tests pass
[ ] Coverage >= 80% (enforced by fail_under in pyproject.toml)
[ ] All GitHub Actions workflows green
```
# # #结构```
[ ] pyproject.toml: name, dynamic/version, description, requires-python, license, authors,
    keywords (10+), classifiers, dependencies, all [project.urls] filled in
[ ] dynamic = ["version"] if using setuptools_scm
[ ] [tool.setuptools_scm] with local_scheme = "no-local-version"
[ ] setup.py shim present (if using setuptools_scm)
[ ] py.typed marker file exists in the package directory (empty file)
[ ] py.typed listed in [tool.setuptools.package-data]
[ ] "Typing :: Typed" classifier in pyproject.toml
[ ] __init__.py has __all__ listing all public symbols
[ ] __version__ via importlib.metadata (not hardcoded string)
```
# # #测试```
[ ] conftest.py has shared fixtures for client and backend
[ ] Core happy path tested
[ ] Error conditions and edge cases tested
[ ] Each backend tested independently in isolation
[ ] Redis backend tested in separate CI job with redis service (if applicable)
[ ] asyncio_mode = "auto" in pyproject.toml (for async tests)
[ ] fetch-depth: 0 in all CI checkout steps
```
###可选后端（如果适用）```
[ ] BaseBackend abstract class defines the interface
[ ] MemoryBackend works with zero extra deps
[ ] RedisBackend raises ImportError with clear pip install hint if redis not installed
[ ] Both backends unit-tested independently
[ ] redis extra declared in [project.optional-dependencies]
[ ] README shows both install paths (base and [redis])
```
###更改日志和文档```
[ ] CHANGELOG.md updated under [Unreleased]
[ ] README has: description, install, quick start, config table, badges, license
[ ] All public symbols have Google-style docstrings
[ ] CONTRIBUTING.md: dev setup, test/lint commands, PR instructions
[ ] SECURITY.md: supported versions, reporting process
[ ] .github/ISSUE_TEMPLATE/bug_report.md
[ ] .github/ISSUE_TEMPLATE/feature_request.md
```

### CI/CD
```
[ ] ci.yml: lint + mypy + test matrix (all supported Python versions)
[ ] ci.yml: separate job for Redis backend with redis service
[ ] publish.yml: triggered on v*.*.* tags, uses Trusted Publishing (OIDC)
[ ] fetch-depth: 0 in all workflow checkout steps
[ ] pypi environment created in GitHub repo Settings → Environments
[ ] No API tokens in repository secrets
```
---

# # 7。要避免的反模式

反模式|为什么不好|正确的方法||---|---|---|
|`__version__ = "1.0.0"`硬编码与setuptools_scm |失效后，第一个git标签|使用`importlib.metadata.version()`|
|在CI checkout中缺少`fetch-depth: 0`| setuptools_scm找不到标签→version =`0.0.0+dev`|将`fetch-depth: 0`添加到**每个** checkout步骤|
|`local_scheme`未设置|`+g<hash>`后缀破坏PyPI上传（本地版本拒绝）|`local_scheme = "no-local-version"`|
|缺少`py.typed`文件| ide和mymyy看不到输入的|在包根|中创建空`py.typed`|`py.typed`不在`package-data`|安装的轮子上缺少文件-无用|添加到`[tool.setuptools.package-data]`|
|在模块顶部导入可选的深度|`ImportError`在`import your_package`上为所有用户|惰性导入需要它的function/class|
|在`setup.py`中复制元数据|与`pyproject.toml`冲突；保持`setup.py`为3线垫片，仅|
|在覆盖配置中没有`fail_under`|覆盖回归不被注意|设置`fail_under = 80`|
| CI中没有mypy |类型错误静默累积|将mypy步骤添加到`ci.yml`|
| GitHub中的API令牌PyPI的秘密|安全风险，轮换负担|使用可信发布（OIDC） |
|直接提交到`main`/`master`|绕过CI检查|通过`no-commit-to-branch`预提交钩子|强制执行
|在CHANGELOG中缺少`[Unreleased]`部分|更改堆积并在发布时被遗忘|保持`[Unreleased]`更新每个PR |
|在库中固定精确的深度版本|打破了用户的依赖解析|只使用`>=`下界；避免`==`|
在`__init__.py`|用户可能会意外地导入内部助手|声明`__all__`与每个公共符号|
|`from your_package import *`测试|即使导入被破坏，测试也通过|始终使用显式导入|
|无`SECURITY.md`|无漏洞披露路径|添加响应时间为|的文件
|`Any`在类型提示中的任何地方|完全击败我|使用`object`来表示真正任意的值|
|`Union`返回类型|强制每个调用者编写`isinstance()`检查|返回具体类型；使用重载|
|`setup.cfg`+`pyproject.toml`两个活动|冲突和混淆贡献者|迁移到`pyproject.toml`|
|在未标记的提交上发布|版本号没有意义|总是在发布|之前标记
|未在所有支持的Python版本上测试|用户发现的破损，而不是您|在CI中进行矩阵测试|
|`license = {text = "MIT"}`（旧格式）|已弃用；PEP 639使用SPDX字符串|`license = "MIT"`|
|无问题模板| Bug报告不一致|添加`bug_report.md`+`feature_request.md`|---

# # 8。主发布清单

在按下发布标签之前，遍历每个项目。CI必须是完全绿色的。

代码质量```
[ ] ruff check . — zero errors
[ ] ruff format . --check — zero formatting issues
[ ] mypy src/your_package/ — zero type errors
[ ] pytest — all tests pass
[ ] Coverage >= 80% (fail_under enforced in pyproject.toml)
[ ] All GitHub Actions CI jobs green (lint + test matrix)
```
项目结构```
[ ] pyproject.toml — name, description, requires-python, license (SPDX string), authors,
    keywords (10+), classifiers (Python versions + Typing :: Typed), urls (all 5 fields)
[ ] dynamic = ["version"] set (if using setuptools_scm or hatch-vcs)
[ ] [tool.setuptools_scm] with local_scheme = "no-local-version"
[ ] setup.py shim present (if using setuptools_scm)
[ ] py.typed marker file exists (empty file in package root)
[ ] py.typed listed in [tool.setuptools.package-data]
[ ] "Typing :: Typed" classifier in pyproject.toml
[ ] __init__.py has __all__ listing all public symbols
[ ] __version__ reads from importlib.metadata (not hardcoded)
```
# # #测试```
[ ] conftest.py has shared fixtures for client and backend
[ ] Core happy path tested
[ ] Error conditions and edge cases tested
[ ] Each backend tested independently in isolation
[ ] asyncio_mode = "auto" in pyproject.toml (for async tests)
[ ] fetch-depth: 0 in all CI checkout steps
```
###修改elog和Docs```
[ ] CHANGELOG.md: [Unreleased] entries moved to [x.y.z] - YYYY-MM-DD
[ ] README has: description, install commands, quick start, config table, badges
[ ] All public symbols have Google-style docstrings
[ ] CONTRIBUTING.md: dev setup, test/lint commands, PR instructions
[ ] SECURITY.md: supported versions, reporting process with timeline
```
# # #版本控制```
[ ] All CI checks pass on the commit you plan to tag
[ ] CHANGELOG.md updated and committed
[ ] Git tag follows format v1.2.3 (semver, v prefix)
[ ] No stale local_scheme suffixes will appear in the built wheel name
```

### CI/CD
```
[ ] ci.yml: lint + mypy + test matrix (all supported Python versions)
[ ] publish.yml: triggered on v*.*.* tags, uses Trusted Publishing (OIDC)
[ ] pypi environment created in GitHub repo Settings → Environments
[ ] No API tokens stored in repository secrets
```
释放命令序列```bash
# 1. Run full local validation
ruff check . ; ruff format . --check ; mypy src/your_package/ ; pytest

# 2. Update CHANGELOG.md — move [Unreleased] to [x.y.z]
# 3. Commit the changelog
git add CHANGELOG.md
git commit -m "chore: prepare release vX.Y.Z"

# 4. Tag and push — this triggers publish.yml automatically
git tag vX.Y.Z
git push origin main --tags

# 5. Monitor: https://github.com/<you>/<pkg>/actions
# 6. Verify: https://pypi.org/project/your-package/
```
