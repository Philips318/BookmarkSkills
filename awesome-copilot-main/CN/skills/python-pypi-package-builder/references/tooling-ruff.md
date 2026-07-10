#工具- Ruff-Only设置和代码质量

##目录
1. [使用Ruff（代替black， issort, flake8）]（#1- Use - Only - Ruff -replace -black- issort -flake8）
2. [Ruff配置在pyproject.toml]（#2-ruff-configuration-in-pyprojecttoml）
3. (mypy配置)(# 3-mypy-configuration)
4. (导向配置)(# 4-pre-commit-configuration)
5. [pytest和Coverage配置]（#5-pytest和Coverage - Configuration）
6. [pyproject.toml中的开发依赖]（#6-dev-dependencies-in-pyprojecttoml）
7. [CI Lint Job - Ruff - Only]（#7-ci-lint-job- Ruff - Only）
8. [迁移指南-删除黑色和等距]（#8-migration-guide- remove -black- issort）

---

# # 1。只使用Ruff（代替黑色，issort, flake8）

**决定：**使用`ruff`作为单一检测和格式化工具。移除`black`和`isort`。

|旧的（避免）|新的（使用）|它做什么||---|---|---|
|`black`|`ruff format`|代码格式化|
|`isort`|`ruff check --select I`|导入排序|
|`flake8`|`ruff check`|样式和错误提示|
|`pyupgrade`|`ruff check --select UP`|语法升级到现代Python |
|`bandit`|`ruff check --select S`|安全检查|
|以上所有|`ruff`|一个工具，一个配置部分|

* *为什么拉夫?**
-比它所取代的工具快10 - 100倍（用Rust编写）。
-单一配置部分在`pyproject.toml`-没有`.flake8`，`.isort.cfg`，`pyproject.toml[tool.black]`蔓延。
-由Astral积极维护；遵循与它所取代的工具相同的规则。
-`ruff format`是黑色兼容-现有的黑色格式的代码通过没有更改。

---

# # 2。pyproject.toml中的Ruff配置```toml
[tool.ruff]
target-version = "py310"        # Minimum supported Python version
line-length    = 88             # black-compatible default
src            = ["src", "tests"]

[tool.ruff.lint]
select = [
    "E",   # pycodestyle errors
    "W",   # pycodestyle warnings
    "F",   # pyflakes
    "I",   # isort
    "B",   # flake8-bugbear (opinionated but very useful)
    "C4",  # flake8-comprehensions
    "UP",  # pyupgrade (modernise syntax)
    "SIM", # flake8-simplify
    "TCH", # flake8-type-checking (move imports to TYPE_CHECKING block)
    "ANN", # flake8-annotations (enforce type hints — remove if too strict)
    "S",   # flake8-bandit (security)
    "N",   # pep8-naming
]
ignore = [
    "ANN101",  # Missing type annotation for `self`
    "ANN102",  # Missing type annotation for `cls`
    "S101",    # Use of `assert` — necessary in tests
    "S603",    # subprocess without shell=True — often intentional
    "B008",    # Do not perform function calls in default arguments (false positives in FastAPI/Typer)
]

[tool.ruff.lint.isort]
known-first-party = ["your_package"]

[tool.ruff.lint.per-file-ignores]
"tests/**" = ["S101", "ANN", "D"]   # Allow assert and skip annotations/docstrings in tests

[tool.ruff.format]
quote-style              = "double"   # black-compatible
indent-style             = "space"
skip-magic-trailing-comma = false
line-ending              = "auto"
```
有用的ruff命令```bash
# Check for lint issues (no changes)
ruff check .

# Auto-fix fixable issues
ruff check --fix .

# Format code (replaces black)
ruff format .

# Check formatting without changing files (CI mode)
ruff format --check .

# Run both lint and format check in one command (for CI)
ruff check . && ruff format --check .
```
---

# # 3。mypy配置```toml
[tool.mypy]
python_version          = "3.10"
strict                  = true
warn_return_any         = true
warn_unused_ignores     = true
warn_redundant_casts    = true
disallow_untyped_defs   = true
disallow_incomplete_defs = true
check_untyped_defs      = true
no_implicit_optional    = true
show_error_codes        = true

# Ignore missing stubs for third-party packages that don't ship types
[[tool.mypy.overrides]]
module = ["redis.*", "pydantic_settings.*"]
ignore_missing_imports = true
```
###运行mypy -处理src和平面布局```bash
# src layout:
mypy src/your_package/

# flat layout:
mypy your_package/
```
在CI中，动态检测布局：```yaml
- name: Run mypy
  run: |
    if [ -d "src" ]; then
        mypy src/
    else
        mypy your_package/
    fi
```
---

# # 4。导向配置```yaml
# .pre-commit-config.yaml
repos:
  - repo: https://github.com/astral-sh/ruff-pre-commit
    rev: v0.4.4    # Pin to a specific release; update periodically with `pre-commit autoupdate`
    hooks:
      - id: ruff
        args: [--fix]       # Auto-fix what can be fixed
      - id: ruff-format     # Format (replaces black hook)

  - repo: https://github.com/pre-commit/mirrors-mypy
    rev: v1.10.0
    hooks:
      - id: mypy
        additional_dependencies:
          - types-requests
          - types-redis
          # Add stubs for any typed dependency used in your package

  - repo: https://github.com/pre-commit/pre-commit-hooks
    rev: v4.6.0
    hooks:
      - id: trailing-whitespace
      - id: end-of-file-fixer
      - id: check-toml
      - id: check-yaml
      - id: check-merge-conflict
      - id: check-added-large-files
        args: ["--maxkb=500"]
```
###❌拆下这些挂钩（用褶皱代替）```yaml
# DELETE or never add:
- repo: https://github.com/psf/black           # replaced by ruff-format
- repo: https://github.com/PyCQA/isort          # replaced by ruff lint I rules
- repo: https://github.com/PyCQA/flake8         # replaced by ruff check
- repo: https://github.com/PyCQA/autoflake      # replaced by ruff check F401
```
# # #设置```bash
pip install pre-commit
pre-commit install     # Installs git hook — runs on every commit
pre-commit run --all-files  # Run manually on all files
pre-commit autoupdate  # Update all hooks to latest pinned versions
```
---

# # 5。pytest和Coverage配置```toml
[tool.pytest.ini_options]
testpaths    = ["tests"]
addopts      = "-ra -q --strict-markers --cov=your_package --cov-report=term-missing"
asyncio_mode = "auto"    # Enables async tests without @pytest.mark.asyncio decorator

[tool.coverage.run]
source   = ["your_package"]
branch   = true
omit     = ["**/__main__.py", "**/cli.py"]  # omit entry points from coverage

[tool.coverage.report]
show_missing   = true
skip_covered   = false
fail_under     = 85        # Fail CI if coverage drops below 85%
exclude_lines  = [
    "pragma: no cover",
    "if TYPE_CHECKING:",
    "raise NotImplementedError",
    "@abstractmethod",
]
```
### asyncio_mode = "auto" -删除@pytest.mark.asyncio . mode

在`pyproject.toml`中设置`asyncio_mode = "auto"`， **不**添加`@pytest.mark.asyncio`测试函数。该装饰器是多余的，在现代pytest-asyncio中会引发警告。```python
# WRONG — the decorator is deprecated when asyncio_mode = "auto":
@pytest.mark.asyncio
async def test_async_operation():
    result = await my_async_func()
    assert result == expected

# CORRECT — just use async def:
async def test_async_operation():
    result = await my_async_func()
    assert result == expected
```
---

# # 6。pyproject.toml中的Dev依赖项

在名为`dev`的`[extras]`组中声明所有dev/test工具。```toml
[project.optional-dependencies]
dev = [
    "pytest>=8",
    "pytest-asyncio>=0.23",
    "pytest-cov>=5",
    "ruff>=0.4",
    "mypy>=1.10",
    "pre-commit>=3.7",
    "httpx>=0.27",       # If testing HTTP transport
    "respx>=0.21",       # If mocking httpx in tests
]
redis = [
    "redis>=5",
]
docs = [
    "mkdocs-material>=9",
    "mkdocstrings[python]>=0.25",
]
```
安装开发依赖项：```bash
pip install -e ".[dev]"
pip install -e ".[dev,redis]"   # Include optional extras
```
---

# # 7。CI Lint Job - Ruff Only

将单独的`black`、`isort`和`flake8`步骤替换为单个`ruff`步骤。```yaml
# .github/workflows/ci.yml  — lint job
lint:
  name: Lint & Type Check
  runs-on: ubuntu-latest
  steps:
    - uses: actions/checkout@v4

    - uses: actions/setup-python@v5
      with:
        python-version: "3.11"

    - name: Install dev dependencies
      run: pip install -e ".[dev]"

    # Single step: ruff replaces black + isort + flake8
    - name: ruff lint
      run: ruff check .

    - name: ruff format check
      run: ruff format --check .

    - name: mypy
      run: |
        if [ -d "src" ]; then
            mypy src/
        else
            mypy $(basename $(ls -d */))/ 2>/dev/null || mypy .
        fi
```
---

# # 8。迁移指南-去除黑色和黑色

如果您正在转换使用`black`和`isort`的现有项目：```bash
# 1. Remove black and isort from dev dependencies
pip uninstall black isort

# 2. Remove black and isort config sections from pyproject.toml
# [tool.black]  ← delete this section
# [tool.isort]  ← delete this section

# 3. Add ruff to dev dependencies (see Section 2 for config)

# 4. Run ruff format to confirm existing code is already compatible
ruff format --check .
# ruff format is black-compatible; output should be identical

# 5. Update .pre-commit-config.yaml (see Section 4)
# Remove black and isort hooks; add ruff and ruff-format hooks

# 6. Update CI (see Section 7)
# Remove black, isort, flake8 steps; add ruff check + ruff format --check

# 7. Reinstall pre-commit hooks
pre-commit uninstall
pre-commit install
pre-commit run --all-files   # Verify clean
```
