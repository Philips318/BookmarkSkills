编译语言的# CodeQL构建模式

CodeQL如何处理编译语言分析的详细参考，包括构建模式、自动构建行为、运行程序需求和硬件规范。

##构建模式概述

CodeQL为编译语言提供了三种构建模式：

|模式|描述|何时使用||---|---|---|
|`none`|分析源代码而不构建。依赖关系是启发式推断的。|默认设置；快速扫描;类解释分析|
|`autobuild`|自动检测并运行构建系统。|当`none`产生不准确的结果；当Kotlin代码存在时|
|`manual`|用户提供显式构建命令。|复杂构建系统；自动构建失败;自定义构建需求|

# #C/C+ +

支持的构建模式`none`,`autobuild`,`manual`**默认设置模式：**`none`### No Build （`none`）
-通过源文件扩展名推断编译单元
-编译标志，包括通过检查代码库推断的路径
-不需要工作的构建命令

* *准确性考虑:* *
-如果代码严重依赖于自定义macros/defines而不是在现有的头文件中，可能不太准确
-当代码库有许多外部依赖时，可能会失去准确性* *改善准确性:* *
-将自定义macros/defines放在源文件包含的头文件中
-确保外部依赖（头）在系统包括目录或工作空间中可用
-在目标平台上运行提取（例如，Windows项目的Windows运行器）

# # #自动构建

Windows autodetection: * * * *
1. 在最接近根的`.sln`或`.vcxproj`上调用`MSBuild.exe`2. 如果有多个相同深度的文件，则尝试构建所有文件
3. 退回到构建脚本：`build.bat`、`build.cmd`、`build.exe`* *Linux/macOSautodetection: * *
1. 在根目录中查找构建系统
2. 如果没有找到，则在子目录中搜索唯一的构建系统
3. 运行相应的configure/build命令

**支持的构建系统：** MSBuild， Autoconf, Make, CMake, qmake, Meson, Waf, SCons, Linux Kbuild，构建脚本###运行器要求（C/C++）
**Ubuntu:**`gcc`编译器；可能需要`clang`或`msvc`。构建工具：`msbuild`，`make`,`cmake`,`bazel`。实用程序：`python`，`perl`,`lex`,`yacc`。
- **自动安装依赖：**设置`CODEQL_EXTRACTOR_CPP_AUTOINSTALL_DEPENDENCIES=true`（默认启用github托管，禁用自托管）。需要Ubuntu无密码`sudo apt-get`。
—**Windows: PATH中的**`powershell.exe`C \ # # #

支持的构建模式`none`,`autobuild`,`manual`**默认设置模式：**`none`### No Build （`none`）
-恢复依赖使用启发式从：`*.csproj`，`*.sln`,`nuget.config`,`packages.config`,`global.json`,`project.assets.json`—如果组织配置了私有NuGet源，则使用私有NuGet源
-生成额外的源文件的准确性：
-全局`using`指令（隐式`using`特性）
- ASP。. NET Core`.cshtml`→`.cs`转换* *准确性考虑:* *
-需要互联网接入或私人NuGet feed
同一个NuGet依赖的多个版本可能会导致问题（CodeQL选择较新的版本）
—多个。. NET框架版本可能会影响准确性
—类名冲突导致方法调用目标丢失

# # #自动构建

Windows autodetection: * * * *
1.`dotnet build`上的`.sln`或`.csproj`最接近根
2.`MSBuild.exe`对solution/project文件
3. 构建脚本：`build.bat`，`build.cmd`,`build.exe`* *Linux/macOSautodetection: * *
1.`dotnet build`上的`.sln`或`.csproj`最接近根
2.`MSbuild`对solution/project文件
3. 构建脚本：`build`，`build.sh`注入编译器标志（手动构建）

CodeQL跟踪器将这些标志注入到c#编译器调用中：

|标志|目的||---|---|
|`/p:MvcBuildViews=true`|预编译ASP. NET MVC视图用于安全分析|
|`/p:UseSharedCompilation=false`|关闭共享编译服务器（跟踪检查所需）|
|`/p:EmitCompilerGeneratedFiles=true`|将生成的源文件写入磁盘以进行提取|

>`/p:EmitCompilerGeneratedFiles=true`可能会导致遗留项目或`.sqlproj`文件出现问题。

###运行器要求（c#）
- * *。. NET Core:**。. NET SDK（适用于`dotnet`）
- * *。. NET Framework (Windows):** Microsoft Build Tools + NuGet CLI
- * *。. NET Framework (Linux/macOS):** Mono Runtime （`mono`,`msbuild`,`nuget`）
- **`build-mode: none`:**需要互联网访问或私有NuGet feed

# #去

支持的构建模式`autobuild`、`manual`（无`none`模式）

**默认设置模式：**`autobuild`# # #自动构建Autodetection序列:
1. 调用`make`、`ninja`、`./build`或`./build.sh`，直到一个成功并且`go list ./...`工作为止
2. 如果没有成功，则查找`go.mod`（`go get`）、`Gopkg.toml`（`dep ensure -v`）或`glide.yaml`（`glide install`）
3. 如果没有找到依赖项管理器，重新安排`GOPATH`的目录并使用`go get`4. 提取所有Go代码（类似于`go build ./...`）

**默认设置**自动检测`go.mod`并安装兼容的Go版本。

提取器选项

|环境变量|默认值|描述信息||---|---|---|
|`CODEQL_EXTRACTOR_GO_OPTION_EXTRACT_TESTS`|`false`|分析|包含`_test.go`文件
|`CODEQL_EXTRACTOR_GO_OPTION_EXTRACT_VENDOR_DIRS`|`false`|包含`vendor/`目录|## Java/Kotlin
支持的构建模式
- **Java:**`none`,`autobuild`,`manual`**Kotlin:**`autobuild`,`manual`（无`none`模式）

**默认设置模式：**
—仅Java:`none`- Kotlin或Java+Kotlin:`autobuild`如果使用`none`模式将Kotlin代码添加到repo中，禁用并重新启用默认设置以切换到`autobuild`。

###没有构建(`none`) -仅Java
-运行Gradle或Maven获取依赖信息（不是实际构建）
-查询每个根构建文件；在冲突上更倾向于使用较新的依赖版本
-如果配置，使用私有Maven注册表* *准确性考虑:* *
-无法查询依赖关系的构建脚本可能会导致不准确的猜测
-在正常构建过程中生成的代码将会丢失
-相同依赖的多个版本（CodeQL选择更新的）
-多个JDK版本- CodeQL使用最高发现；低版本文件可能会被部分分析
—类名冲突导致方法调用目标丢失

# # #自动构建

* * Autodetection序列:* *
1. 在根目录中搜索Gradle、Maven、Ant构建文件
2. 先运行（Gradle优先于Maven）
3. 否则，搜索构建脚本

**构建系统：** Gradle， Maven, Ant

###运行器需求（Java）
- JDK（适合项目的版本）
- Gradleand/orMaven
- Internet访问或私有工件存储库（用于`none`模式）

# #生锈

支持的构建模式`none`,`autobuild`,`manual`**默认设置模式：**`none`# #迅速支持的构建模式`autobuild`、`manual`（无`none`模式）

**默认设置模式：**`autobuild`**运行程序要求：** macOS运行程序仅限。不支持动作运行器控制器(ARC) -仅限Linux。

> macOS运行程序更昂贵；考虑只扫描构建步骤以优化成本。

多语言矩阵示例

混合构建模式```yaml
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
###条件手动构建步骤```yaml
steps:
  - name: Checkout
    uses: actions/checkout@v4

  - name: Initialize CodeQL
    uses: github/codeql-action/init@v4
    with:
      languages: ${{ matrix.language }}
      build-mode: ${{ matrix.build-mode }}

  - if: matrix.build-mode == 'manual'
    name: Build C/C++ code
    run: |
      make bootstrap
      make release

  - name: Perform CodeQL Analysis
    uses: github/codeql-action/analyze@v4
    with:
      category: "/language:${{ matrix.language }}"
```
特定于操作系统的运行程序```yaml
strategy:
  fail-fast: false
  matrix:
    include:
      - language: javascript-typescript
        build-mode: none
        runner: ubuntu-latest
      - language: swift
        build-mode: autobuild
        runner: macos-latest
      - language: csharp
        build-mode: autobuild
        runner: windows-latest

jobs:
  analyze:
    runs-on: ${{ matrix.runner }}
```
##硬件要求

推荐规格（自托管运行程序）

|代码库大小|代码行数|内存| CPU内核|磁盘||---|---|---|---|---|
|小型| < 100K | 8gb + | 2| SSD, |≥14gb
|中| 100K ~ 1M | 16gb + | 4 ~ 8 | SSD，≥14gb |
|大| > 1M | 64gb + | 8| SSD,≥14gb |

性能提示
—对于所有大小的代码库使用SSD存储
确保有足够的磁盘空间用于checkout + build + CodeQL数据
—“`--threads=0`”表示使用所有可用的CPU内核
—启用依赖缓存以减少分析时间
-考虑精度可以接受的`none`构建模式-比`autobuild`快得多

##依赖缓存

高级设置工作流```yaml
- uses: github/codeql-action/init@v4
  with:
    languages: java-kotlin
    dependency-caching: true
```
|值|行为||---|---|
|`false`/`none`/`off`|禁用（高级设置默认）|
|`restore`|仅恢复现有缓存|
|`store`|只存储新的缓存|
|`true`/`full`/`on`|恢复和存储缓存|

在github托管的运行程序上默认设置自动启用缓存。