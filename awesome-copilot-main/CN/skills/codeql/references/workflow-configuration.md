# CodeQL工作流配置参考

通过GitHub Actions工作流配置CodeQL分析的详细参考。这是对SKILL.md中的过程指导的补充。

##触发器配置

###推触发器

每次推送到指定分支时扫描：```yaml
on:
  push:
    branches: [main, protected]
```
-代码扫描是触发的每一个推到列出的分支
—扫描激活的目标分支上必须存在该工作流
—结果显示在存储库安全性选项卡中
-当推送结果映射到开放的PR时，警报也会显示为PR注释

拉请求触发器

扫描拉取请求的合并提交：```yaml
on:
  pull_request:
    branches: [main]
```
-扫描PR的合并提交（不是头部提交）以获得更准确的结果
-对于私有分支pr，在存储库设置中启用“从分支拉取请求运行工作流”
-结果显示为PR检查注释

###调度触发器

定期扫描默认分支：```yaml
on:
  schedule:
    - cron: '20 14 * * 1'  # Monday 14:20 UTC
```
—仅当默认分支上存在工作流文件时触发
-捕捉新发现的漏洞，即使没有积极的开发

合并组触发器

使用合并队列时必需：```yaml
on:
  push:
    branches: [main]
  pull_request:
    branches: [main]
  merge_group:
```
###路径过滤

控制何时根据更改的文件运行工作流：```yaml
on:
  pull_request:
    paths-ignore:
      - '**/*.md'
      - '**/*.txt'
      - 'docs/**'
```
或者使用`paths`仅在特定目录上触发：```yaml
on:
  pull_request:
    paths:
      - 'src/**'
      - 'apps/**'
```
b> **重要：**`paths-ignore`和`paths`控制工作流是否运行。当工作流运行时，它分析PR中所有更改的文件（包括`paths-ignore`匹配的文件），除非通过CodeQL配置文件的`paths-ignore`排除文件。

###工作流调度（手动触发）```yaml
on:
  workflow_dispatch:
    inputs:
      language:
        description: 'Language to analyze'
        required: true
        default: 'javascript-typescript'
```
##运行器和操作系统配置

### GitHub-Hosted Runners```yaml
jobs:
  analyze:
    runs-on: ubuntu-latest    # Also: windows-latest, macos-latest
```
-`ubuntu-latest`-最常见，推荐用于大多数语言
-`macos-latest`- Swift分析所需`windows-latest`-需要一些C/C++和c#项目使用MSBuild

###自托管跑者```yaml
jobs:
  analyze:
    runs-on: [self-hosted, ubuntu-latest]
```
对自托管跑者的要求：
—Git必须在PATH中
—建议使用SSD硬盘，磁盘空间≥14gb
—参见SKILL.md中的硬件要求表

###超时配置

防止工作流程挂起：```yaml
jobs:
  analyze:
    timeout-minutes: 120
```
语言和构建模式矩阵

标准矩阵模式```yaml
strategy:
  fail-fast: false
  matrix:
    include:
      - language: javascript-typescript
        build-mode: none
      - language: python
        build-mode: none
      - language: java-kotlin
        build-mode: none
      - language: c-cpp
        build-mode: autobuild
```
带有混合构建模式的多语言存储库```yaml
strategy:
  fail-fast: false
  matrix:
    include:
      - language: c-cpp
        build-mode: manual
      - language: csharp
        build-mode: autobuild
      - language: java-kotlin
        build-mode: none
```
###构建模式汇总

|语言|`none`|`autobuild`|`manual`|默认设置模式||---|:---:|:---:|:---:|---|
|C/C++ |✅|✅|✅|`none`|
| c# |✅|✅|✅|`none`|
| Go |❌|✅|✅|`autobuild`|
| Java |✅|✅|✅|`none`|
| Kotlin |❌|✅|✅|`autobuild`|
| Python |✅|❌|❌|`none`|
| Ruby |✅|❌|❌|`none`|
| Rust |✅|✅|✅|`none`|
| Swift |❌|✅|✅|`autobuild`|
|JavaScript/TypeScript|✅|❌|❌|`none`|
|GitHub Actions|✅|❌|❌|`none`|

CodeQL数据库位置

覆盖默认的数据库位置：```yaml
- uses: github/codeql-action/init@v4
  with:
    db-location: '${{ github.runner_temp }}/my_location'
```
—默认值：`${{ github.runner_temp }}/codeql_databases`—路径必须是可写的，且不存在或为空目录
-在自托管的跑步者，确保在跑步之间清理

##查询套件和包

内置查询套件```yaml
- uses: github/codeql-action/init@v4
  with:
    queries: security-extended
```
选项:
-（默认）-标准安全查询
-`security-extended`-额外的安全性查询，假阳性率略高
-`security-and-quality`-安全性加上代码质量查询

自定义查询包```yaml
- uses: github/codeql-action/init@v4
  with:
    packs: |
      codeql/javascript-queries:AlertSuppression.ql
      codeql/javascript-queries:~1.0.0
      my-org/my-custom-pack@1.2.3
```
模型包

扩展CodeQL覆盖自定义libraries/frameworks：```yaml
- uses: github/codeql-action/init@v4
  with:
    packs: my-org/my-model-pack
```
##分析类别

区分同一提交的多个分析：```yaml
- uses: github/codeql-action/analyze@v4
  with:
    category: "/language:${{ matrix.language }}"
```
单一类别模式```yaml
# Per language (default auto-generated pattern)
category: "/language:${{ matrix.language }}"

# Per component
category: "/language:${{ matrix.language }}/component:frontend"

# Per app in monorepo
category: "/language:javascript-typescript/app:blog"
```
在SARIF输出中，`category`值显示为`<run>.automationDetails.id`。

CodeQL配置文件

为高级路径创建`.github/codeql/codeql-config.yml`并查询配置：```yaml
name: "CodeQL Configuration"

# Directories to scan
paths:
  - apps/
  - services/
  - packages/

# Directories to exclude
paths-ignore:
  - node_modules/
  - '**/test/**'
  - '**/fixtures/**'
  - '**/*.test.ts'

# Additional queries
queries:
  - uses: security-extended
  - uses: security-and-quality

# Custom query packs
packs:
  javascript-typescript:
    - codeql/javascript-queries
  python:
    - codeql/python-queries
```
工作流程中的参考：```yaml
- uses: github/codeql-action/init@v4
  with:
    config-file: .github/codeql/codeql-config.yml
```
##依赖缓存

启用缓存以加快依赖项解析：```yaml
- uses: github/codeql-action/init@v4
  with:
    dependency-caching: true
```
价值观:
-`false`/`none`/`off`-禁用（默认为高级设置）
-`restore`只恢复现有缓存
-`store`-只存储新的缓存
-`true`/`full`/`on`-恢复和存储缓存

在github托管的运行程序上默认设置自动启用缓存。

警报级别和合并保护

使用存储库规则集根据代码扫描警报阻止pr：

—所需工具查找符合定义的严重性阈值的警报
-所需工具的分析仍在进行中
—未配置存储库所需的工具

通过存储库设置→规则→规则集→代码扫描进行配置。

##并发控制

防止重复的工作流运行：```yaml
concurrency:
  group: codeql-${{ github.ref }}
  cancel-in-progress: true
```
完整的工作流示例```yaml
name: "CodeQL Analysis"

on:
  push:
    branches: [main]
  pull_request:
    branches: [main]
  schedule:
    - cron: '30 6 * * 1'

permissions:
  security-events: write
  contents: read
  actions: read

concurrency:
  group: codeql-${{ github.ref }}
  cancel-in-progress: true

jobs:
  analyze:
    name: Analyze (${{ matrix.language }})
    runs-on: ${{ matrix.language == 'swift' && 'macos-latest' || 'ubuntu-latest' }}
    timeout-minutes: 120
    strategy:
      fail-fast: false
      matrix:
        include:
          - language: javascript-typescript
            build-mode: none
          - language: python
            build-mode: none

    steps:
      - name: Checkout repository
        uses: actions/checkout@v4

      - name: Initialize CodeQL
        uses: github/codeql-action/init@v4
        with:
          languages: ${{ matrix.language }}
          build-mode: ${{ matrix.build-mode }}
          queries: security-extended
          dependency-caching: true

      - if: matrix.build-mode == 'manual'
        name: Manual Build
        run: |
          echo 'Replace with actual build commands'
          exit 1

      - name: Perform CodeQL Analysis
        uses: github/codeql-action/analyze@v4
        with:
          category: "/language:${{ matrix.language }}"
```
