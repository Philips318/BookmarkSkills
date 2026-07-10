---
name: codeql
description: Comprehensive guide for setting up and configuring CodeQL code scanning via GitHub Actions workflows and the CodeQL CLI. This skill should be used when users need help with code scanning configuration, CodeQL workflow files, CodeQL CLI commands, SARIF output, security analysis setup, or troubleshooting CodeQL analysis.
---
# CodeQL代码扫描

该技能提供了配置和运行CodeQL代码扫描的过程指导—通过GitHub Actions工作流和独立的CodeQL CLI。

何时使用此技能

当请求涉及：

-创建或自定义`codeql.yml`GitHub Actions工作流
-选择默认设置和高级设置之间的代码扫描
-配置CodeQL语言矩阵、构建模式或查询套件
-本地运行CodeQL CLI （`codeql database create`,`database analyze`,`github upload-results`）
理解或解释来自CodeQL的SARIF输出
-排除CodeQL分析故障（构建模式，编译语言，运行程序需求）
-为单组件扫描设置CodeQL
-配置依赖缓存、自定义查询包或模型包

支持的语言

CodeQL支持以下语言标识符：|语言|标识符|备选项||---|---|---|
|C/C++ |`c-cpp`|`c`,`cpp`|
|`csharp`| - |
|转|`go`| - |
|Java/Kotlin|`java-kotlin`|`java`,`kotlin`|
|JavaScript/TypeScript|`javascript-typescript`|`javascript`,`typescript`|
| Python |`python`| - |
| Ruby |`ruby`| - |
|锈|`rust`| - |
| Swift |`swift`| - |
|GitHub Actions|`actions`| - |

>替代标识符等同于标准标识符（例如，`javascript`不排除TypeScript分析）。

核心工作流程-GitHub Actions步骤1：选择安装类型

- **默认设置** -启用从存储库设置→高级安全→CodeQL分析。最适合快速入门。对大多数语言使用`none`构建模式。
- **高级设置** -创建一个`.github/workflows/codeql.yml`文件，以完全控制触发器，构建模式，查询套件和矩阵策略。

要从默认设置切换到高级设置：首先禁用默认设置，然后提交工作流文件。步骤2：配置工作流触发器

定义扫描运行的时间：```yaml
on:
  push:
    branches: [main, protected]
  pull_request:
    branches: [main]
  schedule:
    - cron: '30 6 * * 1'  # Weekly Monday 6:30 UTC
```
-`push`-每次推送到指定分支时扫描结果显示在安全选项卡中`pull_request`扫描PR合并提交结果显示为PR检查注释
-`schedule`-定时扫描默认分支（默认分支上必须存在cron）
-`merge_group`-添加存储库是否使用合并队列

跳过仅文档pr的扫描：```yaml
on:
  pull_request:
    paths-ignore:
      - '**/*.md'
      - '**/*.txt'
```
>`paths-ignore`控制工作流是否运行，而不是分析哪些文件。

步骤3：配置权限

设置最小权限权限：```yaml
permissions:
  security-events: write   # Required to upload SARIF results
  contents: read            # Required to checkout code
  actions: read             # Required for private repos using codeql-action
```
步骤4：配置语言矩阵

使用矩阵策略并行分析每种语言：```yaml
jobs:
  analyze:
    name: Analyze (${{ matrix.language }})
    runs-on: ubuntu-latest
    strategy:
      fail-fast: false
      matrix:
        include:
          - language: javascript-typescript
            build-mode: none
          - language: python
            build-mode: none
```
对于编译语言，设置适当的`build-mode`：
-`none`-无需构建（支持C/C++， c#, Java, Rust）
-`autobuild`-自动构建检测
-`manual`-自定义构建命令（仅限高级设置）

>有关每种语言的详细自动构建行为和运行程序需求，请搜索`references/compiled-languages.md`。

步骤5：配置CodeQL初始化和分析```yaml
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

  - name: Perform CodeQL Analysis
    uses: github/codeql-action/analyze@v4
    with:
      category: "/language:${{ matrix.language }}"
```
**查询套件选项：**
-`security-extended`-默认安全查询加上额外的覆盖
-`security-and-quality`-安全性加上代码质量查询
-通过`packs:`输入自定义查询包（例如，`codeql/javascript-queries:AlertSuppression.ql`）

**依赖缓存：**在`init`操作上设置`dependency-caching: true`来缓存恢复的依赖。

**分析类别：**使用`category`区分单一的SARIF结果（例如，每个语言，每个组件）。

###步骤6：单线程配置

对于具有多个组件的单orepos，使用`category`参数分隔SARIF结果：```yaml
category: "/language:${{ matrix.language }}/component:frontend"
```
要将分析限制到特定目录，请使用CodeQL配置文件（`.github/codeql/codeql-config.yml`）：```yaml
paths:
  - apps/
  - services/
paths-ignore:
  - node_modules/
  - '**/test/**'
```
在工作流程中引用它：```yaml
- uses: github/codeql-action/init@v4
  with:
    config-file: .github/codeql/codeql-config.yml
```
###步骤7：手动构建步骤（编译语言）

如果`autobuild`失败或需要自定义构建命令：```yaml
- language: c-cpp
  build-mode: manual
```
然后在`init`和`analyze`之间添加显式构建步骤：```yaml
- if: matrix.build-mode == 'manual'
  name: Build
  run: |
    make bootstrap
    make release
```
核心工作流- CodeQL CLI

###步骤1：安装CodeQL CLI

下载CodeQL包（包括CLI +预编译查询）：```bash
# Download from https://github.com/github/codeql-action/releases
# Extract and add to PATH
export PATH="$HOME/codeql:$PATH"

# Verify installation
codeql resolve packs
codeql resolve languages
```
>始终使用CodeQL包，而不是独立的CLI下载。该包确保查询兼容性，并提供预编译查询以获得更好的性能。

步骤2：创建一个CodeQL数据库```bash
# Single language
codeql database create codeql-db \
  --language=javascript-typescript \
  --source-root=src

# Multiple languages (cluster mode)
codeql database create codeql-dbs \
  --db-cluster \
  --language=java,python \
  --command=./build.sh \
  --source-root=src
```
对于编译语言，通过`--command`提供构建命令。

步骤3：分析数据库```bash
codeql database analyze codeql-db \
  javascript-code-scanning.qls \
  --format=sarif-latest \
  --sarif-category=javascript \
  --output=results.sarif
```
常见查询套件：`<language>-code-scanning.qls`、`<language>-security-extended.qls`、`<language>-security-and-quality.qls`。

###步骤4：上传结果到GitHub```bash
codeql github upload-results \
  --repository=owner/repo \
  --ref=refs/heads/main \
  --commit=<commit-sha> \
  --sarif=results.sarif
```
需要具有`security-events: write`权限的`GITHUB_TOKEN`环境变量。

### CLI服务器模式

为了避免在运行多个命令时重复初始化JVM：```bash
codeql execute cli-server
```
>详细的CLI命令参考，请搜索“`references/cli-commands.md`”。

##警报管理

严重性级别

警报有两个严重性维度：
- **标准级别：**`Error`、`Warning`、`Note`- **安全级别：**`Critical`，`High`,`Medium`,`Low`（来自CVSS分数，优先显示）

###副驾驶自动修复GitHub CopilotAutofix自动为pull请求中的CodeQL警报生成修复建议-不需要订阅Copilot。在提交建议之前仔细审查建议。

pr中的警报分类

-警报显示为更改行上的检查注释
—对于`error`/`critical`/`high`级别告警，默认检查失败
—配置归并保护规则集，自定义阈值
-排除误报，并记录审计跟踪的原因

>详细的告警管理指导，请搜索“`references/alert-management.md`”。

自定义查询和包

使用自定义查询包```yaml
- uses: github/codeql-action/init@v4
  with:
    packs: |
      my-org/my-security-queries@1.0.0
      codeql/javascript-queries:AlertSuppression.ql
```
创建自定义查询包

使用CodeQL CLI创建和发布包：```bash
# Initialize a new pack
codeql pack init my-org/my-queries

# Install dependencies
codeql pack install

# Publish to GitHub Container Registry
codeql pack publish
```
CodeQL配置文件

对于高级查询和路径配置，创建`.github/codeql/codeql-config.yml`：```yaml
paths:
  - apps/
  - services/
paths-ignore:
  - '**/test/**'
  - node_modules/
queries:
  - uses: security-extended
packs:
  javascript-typescript:
    - my-org/my-custom-queries
```
##扫描码日志

总结指标

工作流日志包括关键指标：
-代码库中的代码行数** -提取前的基线
** -包括外部库和自动生成文件
- **提取errors/warnings** -提取过程中失败或产生警告的文件

调试日志

启用详细诊断：
- **GitHub Actions:**重新运行工作流程“启用调试日志”勾选
- **CodeQL命令行：**使用`--verbosity=progress++`和`--logdir=codeql-logs`# #故障排除

###常见问题

|解决方案||---|---|
验证`on:`触发匹配事件；检查`paths`/`branches`过滤器；确保在目标分支|上存在工作流
|`Resource not accessible`error |增加`security-events: write`和`contents: read`权限|
|自动构建失败|切换到`build-mode: manual`并添加显式构建命令|
|验证`--source-root`、构建命令和语言标识符|
|检查`/p:EmitCompilerGeneratedFiles=true`是否与`.sqlproj`或遗留项目冲突|
|从`none`切换到`autobuild`/`manual`；验证构建编译所有源|
|禁用并重新启用默认设置以切换到`autobuild`|
|在`init`动作|上验证`dependency-caching: true`|使用更大的流道；通过`paths`config减少分析范围；使用`build-mode: none`|
|确保令牌有`security-events: write`；检查10mb的文件大小限制|
| SARIF结果超出d限制|分割多个上传与不同的`--sarif-category`；减少查询范围|
|如果使用高级设置，则禁用默认设置，或删除旧的工作流文件|
|启用依赖缓存；使用`--threads=0`;减少查询套件范围|>全面的故障处理和详细的解决方案，请搜索`references/troubleshooting.md`。

硬件要求（自托管运行程序）

|代码大小| RAM | CPU ||---|---|---|
| Small (<100K LOC) | 8gb + | 2核|
| Medium (100K-1M LOC) | 16gb + | 4-8核|
|大（>1M LOC） | 64gb + | 8核|

所有规格：SSD硬盘，硬盘剩余空间≥14gb。

###操作版本控制

将CodeQL操作固定到特定的主要版本：```yaml
uses: github/codeql-action/init@v4      # Recommended
uses: github/codeql-action/autobuild@v4
uses: github/codeql-action/analyze@v4
```
为了获得最大的安全性，使用完整的提交SHA而不是版本标记。

##参考文件

有关详细文档，请根据需要加载以下参考文件：-`references/workflow-configuration.md`-完整的工作流触发器、运行器和配置选项
-搜索模式：`trigger`、`schedule`、`paths-ignore`、`db-location`、`model packs`、`alert severity`、`merge protection`、`concurrency`、`config file`-`references/cli-commands.md`-完整的CodeQL CLI命令参考
-搜索模式：`database create`、`database analyze`、`upload-results`、`resolve packs`、`cli-server`、`installation`、`CI integration`-`references/sarif-output.md`- SARIF v2.1.0对象模型，上传限制，第三方支持
-搜索模式：`sarifLog`、`result`、`location`、`region`、`codeFlow`、`fingerprint`、`suppression`、`upload limits`、`third-party`、`precision`、`security-severity`-`references/compiled-languages.md`-每种语言的构建模式和自动构建行为
-搜索类型：`C/C++`、`C#`、`Java`、`Go`、`Rust`、`Swift`、`autobuild`、`build-mode`、`hardware`、`dependency caching`-`references/troubleshooting.md`-全面的错误诊断和解决
-搜索模式：`no source code`、`out of disk`、`out of memory`、`403`、`C# compiler`、`analysis too long`、`Kotlin`、`extraction errors`、`debug logging`、`SARIF upload`、`SARIF limits`-`references/alert-management.md`-警报级别，分类，副驾驶自动修复和解散
-搜索类型：`severity`、`security severity`、`CVSS`、`Copilot Autofix`、`dismiss`、`triage`、`PR alerts`、`data flow`、`merge protection`、`REST API`