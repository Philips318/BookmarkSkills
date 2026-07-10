CodeQL CLI命令参考

CodeQL CLI的详细参考-安装、数据库创建、分析、SARIF上传和CI集成。

# #安装

下载CodeQL Bundle

始终从以下地址下载CodeQL包（CLI +预编译查询）：**https://github.com/github/codeql-action/releases**
该包包括：
—CodeQL CLI产品
-兼容查询和库从`github/codeql`—预编译查询计划，加快分析速度

特定于平台的包

|平台|文件||---|---|
|所有平台|`codeql-bundle.tar.zst`|
| Linux |`codeql-bundle-linux64.tar.zst`|
| macOS |`codeql-bundle-osx64.tar.zst`|
| Windows |`codeql-bundle-win64.tar.zst`|

>`.tar.gz`变体也可用于没有Zstandard支持的系统。

# # #设置```bash
# Extract the bundle
tar xf codeql-bundle-linux64.tar.zst

# Add to PATH
export PATH="$HOME/codeql:$PATH"

# Verify installation
codeql resolve packs
codeql resolve languages
```
`codeql resolve packs`应该列出所有受支持语言的可用查询包。如果包丢失，请验证您下载了包（不是独立的CLI）。

CI系统设置

确保在每个CI服务器上都可以使用完整的CodeQL包内容：
-从中心位置复制并在每个服务器上提取，或者
-每次运行时使用GitHub REST API动态下载包

##核心命令

# # #`codeql database create`从源代码创建一个CodeQL数据库。```bash
# Basic usage (interpreted language)
codeql database create <output-dir> \
  --language=<language> \
  --source-root=<source-dir>

# Compiled language with build command
codeql database create <output-dir> \
  --language=java-kotlin \
  --command='./gradlew build' \
  --source-root=.

# Multiple languages (cluster mode)
codeql database create <output-dir> \
  --db-cluster \
  --language=java,python,javascript-typescript \
  --command='./build.sh' \
  --source-root=.
```
* *关键标志:* *

|标志位|描述||---|---|
|`--language=<lang>`|要提取的语言（必选）。使用CodeQL语言标识符。|
|`--source-root=<dir>`|源代码根目录（默认为当前目录）|
|`--command=<cmd>`|编译语言构建命令|
|`--db-cluster`|一次创建多种语言的数据库|
|`--overwrite`|覆盖现有数据库目录|
|`--threads=<n>`|用于提取的线程数（默认为1；所有可用内核使用0）|
|`--ram=<mb>`|提取|的内存限制（MB）

# # #`codeql database analyze`对CodeQL数据库运行查询并生成SARIF输出。```bash
codeql database analyze <database-dir> \
  <query-suite-or-pack> \
  --format=sarif-latest \
  --sarif-category=<category> \
  --output=<output-file>
```
* *关键标志:* *

|标志位|描述||---|---|
|`--format=sarif-latest`|输出格式(当前SARIF v2.1.0使用`sarif-latest`|`--sarif-category=<cat>`| SARIF结果的类别标记（对于多语言回购很重要）|
|`--output=<file>`| SARIF结果的输出文件路径|
|`--threads=<n>`|用于分析的线程数|
|`--ram=<mb>`|内存限制，单位为MB
|`--sarif-add-file-contents`|在SARIF输出|中包含源文件内容
|`--ungroup-results`|禁用结果分组（每个事件单独报告）|
|`--no-download`|跳过下载查询包（只使用本地可用的包）|

**通用查询套件：**

| Suite |描述||---|---|
|`<lang>-code-scanning.qls`|标准代码扫描查询|
|`<lang>-security-extended.qls`|扩展安全查询|
|`<lang>-security-and-quality.qls`|安全+代码质量查询|

* *例子:* *```bash
# JavaScript analysis with extended security
codeql database analyze codeql-db/javascript-typescript \
  javascript-typescript-security-extended.qls \
  --format=sarif-latest \
  --sarif-category=javascript \
  --output=js-results.sarif

# Java analysis with all available threads
codeql database analyze codeql-db/java-kotlin \
  java-kotlin-code-scanning.qls \
  --format=sarif-latest \
  --sarif-category=java \
  --output=java-results.sarif \
  --threads=0

# Include file contents in SARIF
codeql database analyze codeql-db \
  javascript-typescript-code-scanning.qls \
  --format=sarif-latest \
  --output=results.sarif \
  --sarif-add-file-contents
```
# # #`codeql github upload-results`上传SARIF结果到GitHub代码扫描。```bash
codeql github upload-results \
  --repository=<owner/repo> \
  --ref=<git-ref> \
  --commit=<commit-sha> \
  --sarif=<sarif-file>
```
* *关键标志:* *

|标志位|描述||---|---|
|`--repository=<owner/repo>`|目标GitHub存储库|
|`--ref=<ref>`| Git ref（例如，`refs/heads/main`,`refs/pull/42/head`） |
|`--commit=<sha>`|全提交SHA |
|`--sarif=<file>`| SARIF文件|的路径
|`--github-url=<url>`| GitHub实例URL(用于GHES；默认为github.com
|`--github-auth-stdin`|从stdin读取认证令牌，而不是`GITHUB_TOKEN`env var |

**认证：**使用具有`security-events: write`作用域的令牌设置`GITHUB_TOKEN`环境变量，或者使用`--github-auth-stdin`。

# # #`codeql resolve packs`列出可用的查询包：```bash
codeql resolve packs
```
用于验证安装和诊断丢失的包。从CLI v2.19.0开始可用（早期版本：使用`codeql resolve qlpacks`）。

# # #`codeql resolve languages`支持的语言列表：```bash
codeql resolve languages
```
显示当前安装中可用的语言提取器。

# # #`codeql database bundle`创建一个可重新定位的CodeQL数据库存档，用于共享或故障排除：```bash
codeql database bundle <database-dir> \
  --output=<archive-file>
```
用于与团队成员或GitHub支持共享数据库。

## CLI服务器模式

# # #`codeql execute cli-server`运行持久化服务器以避免在执行多个命令时重复初始化JVM：```bash
codeql execute cli-server [options]
```
* *关键标志:* *

|标志位|描述||---|---|
|`-v, --verbose`|增加进度消息|
|`-q, --quiet`|减少进度消息|
|`--verbosity=<level>`|设置详细度：`errors`，`warnings`,`progress`,`progress+`,`progress++`,`progress+++`|
|`--logdir=<dir>`|将详细日志写入目录|
|`--common-caches=<dir>`|持久缓存数据的位置（默认：`~/.codeql`） |
|`-J=<opt>`|将选项传递给JVM |

服务器通过stdin接受命令并返回结果，使JVM在命令之间保持温暖。主要用于运行多个顺序CodeQL命令的CI环境。

CI集成模式

完成CI脚本示例```bash
#!/bin/bash
set -euo pipefail

REPO="my-org/my-repo"
REF="refs/heads/main"
COMMIT=$(git rev-parse HEAD)
LANGUAGES=("javascript-typescript" "python")

# Create databases for all languages
codeql database create codeql-dbs \
  --db-cluster \
  --source-root=. \
  --language=$(IFS=,; echo "${LANGUAGES[*]}")

# Analyze each language and upload results
for lang in "${LANGUAGES[@]}"; do
  echo "Analyzing $lang..."

  codeql database analyze "codeql-dbs/$lang" \
    "${lang}-security-extended.qls" \
    --format=sarif-latest \
    --sarif-category="$lang" \
    --output="${lang}-results.sarif" \
    --threads=0

  codeql github upload-results \
    --repository="$REPO" \
    --ref="$REF" \
    --commit="$COMMIT" \
    --sarif="${lang}-results.sarif"

  echo "$lang analysis uploaded."
done
```
外部CI系统

对于GitHub Actions以外的CI系统：
1. 在CI运行器上安装CodeQL包
2. 使用适当的构建命令运行`codeql database create`3. 执行`codeql database analyze`命令生成SARIF
4. 运行`codeql github upload-results`将结果推送到GitHub
5. 使用`security-events: write`权限设置`GITHUB_TOKEN`##环境变量

|变量|用途||---|---|
|`GITHUB_TOKEN`|`github upload-results`|鉴权
|`CODEQL_EXTRACTOR_<LANG>_OPTION_<KEY>`|提取器配置（例如，`CODEQL_EXTRACTOR_GO_OPTION_EXTRACT_TESTS=true`） |
自动安装C/C++在Ubuntu |上建立依赖
|`CODEQL_RAM`|覆盖分析的默认RAM分配|
|`CODEQL_THREADS`|覆盖默认线程数|