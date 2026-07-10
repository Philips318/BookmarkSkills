#CI/CD，发布和变更日志

##目录
1. (更新日志格式)(# 1-changelog-format)
2. [ci.yml- lint，类型检查，测试矩阵]（#2-ciyml）
3. [publish.yml-触发版本标签]（#3-publishyml）
4. [PyPI可信发布（无API令牌）]（#4- PyPI可信发布）
5. [手动发布回退]（#5-manual-publish-fallback）
6. (发布清单)(# 6-release-checklist)
7. (验证py。（#7-verify-pytype -ships-in- wheel）
8. [Semver更改类型指南]（#8-semver-change-type-guide）

---

# # 1。更新日志格式

保持`CHANGELOG.md`遵循[保持变更日志]（https://keepachangelog.com/）惯例。
每个PR都应该更新`[Unreleased]`部分。在释放之前，将这些条目移动到
带有日期的新版本部分。```markdown
# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

---

## [Unreleased]

### Added
- (in-progress features go here)

---

## [1.0.0] - 2026-04-02

### Added
- Initial stable release
- `YourMiddleware` with gradual, strict, and combined modes
- In-memory backend (no extra deps)
- Optional Redis backend (`pip install pkg[redis]`)
- Per-route override via `Depends(RouteThrottle(...))`
- `py.typed` marker — PEP 561 typed package
- GitHub Actions CI: lint, mypy, test matrix, Trusted Publishing

### Changed
### Fixed
### Removed

---

## [0.1.0] - 2026-03-01

### Added
- Initial project scaffold

[Unreleased]: https://github.com/you/your-package/compare/v1.0.0...HEAD
[1.0.0]: https://github.com/you/your-package/compare/v0.1.0...v1.0.0
[0.1.0]: https://github.com/you/your-package/releases/tag/v0.1.0
```
### Semver -什么碰撞什么

|修改类型| Bump |示例||---|---|---|
|打破API更改| MAJOR |`1.0.0 → 2.0.0`|
新特性，向后兼容| MINOR |`1.0.0 → 1.1.0`|
|补丁|`1.0.0 → 1.0.1`|

---## 2. `ci.yml`
运行在每一个推和拉请求。测试所有支持的Python版本。```yaml
# .github/workflows/ci.yml
name: CI

on:
  push:
    branches: [main, master]
  pull_request:
    branches: [main, master]

jobs:
  lint:
    name: Lint, Format & Type Check
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-python@v5
        with:
          python-version: "3.11"
      - name: Install dev dependencies
        run: pip install -e ".[dev]"
      - name: ruff lint
        run: ruff check .
      - name: ruff format check
        run: ruff format --check .
      - name: mypy
        run: |
          if [ -d "src" ]; then
              mypy src/
          else
              mypy {mod}/
          fi

  test:
    name: Test (Python ${{ matrix.python-version }})
    runs-on: ubuntu-latest
    strategy:
      matrix:
        python-version: ["3.10", "3.11", "3.12", "3.13"]

    steps:
      - uses: actions/checkout@v4
        with:
          fetch-depth: 0    # REQUIRED for setuptools_scm to read git tags

      - uses: actions/setup-python@v5
        with:
          python-version: ${{ matrix.python-version }}

      - name: Install dependencies
        run: pip install -e ".[dev]"

      - name: Run tests with coverage
        run: pytest --cov --cov-report=xml

      - name: Upload coverage
        uses: codecov/codecov-action@v4
        with:
          token: ${{ secrets.CODECOV_TOKEN }}
          fail_ci_if_error: false

  test-redis:
    name: Test Redis backend
    runs-on: ubuntu-latest
    services:
      redis:
        image: redis:7-alpine
        ports: ["6379:6379"]
    steps:
      - uses: actions/checkout@v4
        with:
          fetch-depth: 0

      - uses: actions/setup-python@v5
        with:
          python-version: "3.11"

      - name: Install with Redis extra
        run: pip install -e ".[dev,redis]"

      - name: Run Redis tests
        run: pytest tests/test_redis_backend.py -v
```
> **使用`setuptools_scm`时，总是在每个结帐步骤中添加`fetch-depth: 0`**。
>没有完整的git历史记录，`setuptools_scm`无法找到标签，并且构建失败
>检测错误。

---## 3. `publish.yml`
当您推送匹配`v*.*.*`的标记时自动触发。使用可信发布（OIDC）
存储库秘密中没有API令牌。```yaml
# .github/workflows/publish.yml
name: Publish to PyPI

on:
  push:
    tags:
      - "v*.*.*"

jobs:
  build:
    name: Build distribution
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
        with:
          fetch-depth: 0      # Critical for setuptools_scm

      - uses: actions/setup-python@v5
        with:
          python-version: "3.11"

      - name: Install build tools
        run: pip install build twine

      - name: Build package
        run: python -m build

      - name: Check distribution
        run: twine check dist/*

      - uses: actions/upload-artifact@v4
        with:
          name: dist
          path: dist/

  publish:
    name: Publish to PyPI
    needs: build
    runs-on: ubuntu-latest
    environment: pypi
    permissions:
      id-token: write     # Required for Trusted Publishing (OIDC)

    steps:
      - uses: actions/download-artifact@v4
        with:
          name: dist
          path: dist/

      - name: Publish to PyPI
        uses: pypa/gh-action-pypi-publish@release/v1
```
---

# # 4。可信出版

可信发布使用OpenID Connect (OIDC)，因此PyPI可以验证发布是否来自您的
特定的GitHub Actions工作流-不需要长期存在的API令牌，没有旋转负担。

一次性设置

1. 在https://pypi.org创建一个帐户
2. 转到**帐户→发布→添加一个新的挂起的发布者**
3. 填写:
- GitHub所有者（您的用户名或组织）
-存储库名称
—工作流程文件名：`publish.yml`—环境名称：`pypi`4. 在GitHub中创建`pypi`环境：
**repo→设置→环境→新环境→命名为`pypi`**

就是这样。下次推送`v*.*.*`标记时，工作流将自动进行身份验证。

---

# # 5。手动发布回退

如果CI还没有设置好或者你需要从你的机器上发布：```bash
pip install build twine

# Build wheel + sdist
python -m build

# Validate before uploading
twine check dist/*

# Upload to PyPI
twine upload dist/*

# OR test on TestPyPI first (recommended for first release)
twine upload --repository testpypi dist/*
pip install --index-url https://test.pypi.org/simple/ your-package
python -c "import your_package; print(your_package.__version__)"
```
---

# # 6。发布清单```
[ ] All tests pass on main/master
[ ] CHANGELOG.md updated — move [Unreleased] items to new version section with date
[ ] Update diff comparison links at bottom of CHANGELOG
[ ] git tag vX.Y.Z
[ ] git push origin master --tags
[ ] Monitor GitHub Actions publish.yml run
[ ] Verify on PyPI: pip install your-package==X.Y.Z
[ ] Test the installed version:
    python -c "import your_package; print(your_package.__version__)"
```
---

# # 7。验证py。船在轮子上打字

在每次构建之后，确认包含了键入的标记：```bash
python -m build
unzip -l dist/your_package-*.whl | grep py.typed
# Must print: your_package/py.typed
# If missing, check [tool.setuptools.package-data] in pyproject.toml
```
如果从转轮中缺少它，用户将无法获得类型信息，即使您的代码是
完全输入。这是一个无声的失败——在释放之前一定要确认。

---

# # 8。Semver更改类型指南

|修改|版本更新|示例||---|---|---|
|打破API更改（remove/rename公开代码）| MAJOR |`1.2.3 → 2.0.0`|
新功能，完全向后兼容| MINOR |`1.2.3 → 1.3.0`|
| PATCH |`1.2.3 → 1.2.4`|
|预发布|后缀|`2.0.0a1 → 2.0.0rc1 → 2.0.0`|
|发布后|`1.2.3 → 1.2.3.post1`|