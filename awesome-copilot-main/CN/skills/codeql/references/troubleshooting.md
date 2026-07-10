# CodeQL故障排除参考

诊断和解决CodeQL分析错误、SARIF上传问题和常见配置问题的综合指南。

构建和分析错误

###“在构建过程中没有看到源代码”

**原因：** CodeQL提取器在创建数据库时没有找到任何源文件。

* *解决方案:* *
—检查`--source-root`是否指向正确的目录
—对于编译语言，确保build命令实际编译源文件
-检查`autobuild`正在检测正确的构建系统
—使用显式的构建命令从`autobuild`切换到`manual`—检查指定的语言是否与实际的源代码语言匹配

自动构建失败

**原因：**`autobuild`无法检测或运行项目构建系统。* *解决方案:* *
—切换到`build-mode: manual`并提供显式的构建命令
-确保所有构建依赖项都安装在运行器上
—对于C/C++：验证是否存在`gcc`、`make`、`cmake`或`msbuild`—对于c#：验证是否安装了`.NET SDK`或`MSBuild`—Java: verify已安装`gradle`或`maven`—检查失败的特定检测步骤的自动构建日志

c#编译器意外失败

**原因：** CodeQL跟踪器注入的编译器标志可能与项目配置冲突。

**详细信息：** CodeQL注入`/p:EmitCompilerGeneratedFiles=true`可能导致以下问题：
-遗产。。NET框架项目
-使用`.sqlproj`文件的项目

* *解决方案:* *
-添加`<EmitCompilerGeneratedFiles>false</EmitCompilerGeneratedFiles>`到有问题的项目文件
如果构建精度可以接受，使用c#的`build-mode: none`从CodeQL分析中排除有问题的项目

分析花费的时间太长**原因：**代码库大、查询复杂或资源不足。

* *解决方案:* *
-在精度可以接受的地方使用`build-mode: none`（明显更快）
—启用依赖项缓存：`dependency-caching: true`—在任务中设置“`timeout-minutes`”，防止工作流挂起
—通过命令行方式“`--threads=0`”使用所有可用的CPU内核
—缩小查询范围：使用`default`套件代替`security-and-quality`-对于自托管运行程序，确保硬件符合建议：
-小型（<100K LOC）： 8gb RAM， 2核
—中（100K-1M LOC）： 16gb RAM， 4-8核
—大型（>1M LOC）： 64gb内存，8核
-如果可用，配置更大的github托管运行程序
—在配置文件中使用`paths`限制分析目录

CodeQL扫描的行数少于预期

**原因：** Build命令没有编译所有的源文件，或者`build-mode: none`丢失了生成的代码。* *解决方案:* *
—从`none`切换到`autobuild`或`manual`构建模式
-确保build命令编译完整的代码库（而不仅仅是一个子集）
-检查代码扫描日志的提取指标：
-代码库中的代码行数（基线）
-提取的代码行数
-不包括自动生成文件的行
—验证语言检测包括所有期望的语言

在无构建模式下检测到Kotlin

**原因：**仓库使用`build-mode: none`（仅限Java），但也包含Kotlin代码。

* *解决方案:* *
-禁用默认设置并重新启用它（切换到`autobuild`）
-或者切换到高级设置`build-mode: autobuild`为`java-kotlin`Kotlin需要一个构建来分析`none`模式仅适用于Java

##权限和访问错误

###错误：403 “Resource not accessible by integration”

**原因：**`GITHUB_TOKEN`缺少所需的权限。* *解决方案:* *
-为工作流添加明确的权限：  ```yaml
  permissions:
    security-events: write
    contents: read
    actions: read
  ```
—对于Dependabot pr，请使用`pull_request_target`而不是`pull_request`-验证仓库是否启用了GitHub代码安全（用于私有仓库）

无法在私有存储库中启用CodeQL

**原因：** GitHub Code Security未启用。

**解决方案：**在存储库设置→高级安全中启用GitHub代码安全。

###错误：“GitHub Code Security或Advanced Security必须启用”

**原因：**试图在没有所需许可证的私有repo上使用代码扫描。

* *解决方案:* *
-为存储库启用GitHub Code Security
—请联系组织管理员开启高级安全

##配置错误

两个CodeQL工作流正在运行

**原因：**默认设置和已有的`codeql.yml`工作流都是激活的。* *解决方案:* *
-如果使用高级设置，禁用默认设置，或
-如果使用默认设置，删除旧的工作流文件
—检查存储库设置→高级安全的活动配置

有些语言没有被分析

**原因：**矩阵配置不包括所有语言。

* *解决方案:* *
—将缺失的语言添加到`matrix.include`数组中
-验证语言标识符是否正确（例如，`javascript-typescript`而不仅仅是`javascript`）
—检查每种语言是否有合适的`build-mode`不清楚是什么触发了工作流运行

* *解决方案:* *
—查看“存储库设置→高级安全”中的工具状态页面
-查看工作流运行日志，了解触发事件的详细信息
-查看工作流文件中的`on:`触发器

###错误：“is not a . ”. Ql文件。QLS文件、目录或查询包规范”

**原因：**工作流中的查询或包引用无效。* *解决方案:* *
—检查查询包名称和版本是否存在
—格式必须正确：`owner/pack-name@version`或`owner/pack-name:path/to/query.ql`—执行命令`codeql resolve packs`，验证可用包

##资源错误

### “Out of disk”或“Out of memory”

**原因：** Runner没有足够的资源进行分析。

* *解决方案:* *
-使用更大的github托管的运行程序（如果可用）
-对于自托管运行程序，增加RAM和磁盘（SSD≥14gb）
-使用`paths`配置减少分析范围
-每个作业分析更少的语言
—使用“`build-mode: none`”，减少资源占用

###数据库提取错误

**原因：**一些源文件无法被CodeQL提取器处理。

* *解决方案:* *
-检查工作流程日志中的提取指标，查看错误计数
—启用调试日志，以便进行详细的提取诊断
—验证源文件的语法是否有效
-确保所有构建依赖项可用

记录和调试###启用调试日志

获取更详细的诊断信息：

* *GitHub Actions: * *
1. 启用调试日志记录后重新运行工作流
2. 在工作流运行中，单击“重新运行作业”→“启用调试日志记录”

* * CodeQL CLI: * *```bash
codeql database create my-db \
  --language=javascript-typescript \
  --verbosity=progress++ \
  --logdir=codeql-logs
```
**详细级别：**`errors`，`warnings`,`progress`,`progress+`,`progress++`,`progress+++`代码扫描日志度量

工作流日志包括汇总指标：
-代码库中的代码行数** -提取前的基线
- **行代码在CodeQL数据库** -提取包括外部库
- **行不包括自动生成的文件** - net分析的代码
- **提取success/error/warning计数** -每个文件提取结果

私有注册表诊断

对于带有私有包注册表的`build-mode: none`：
-检查工作流日志中的“设置注册表代理”步骤
-查找`Credentials loaded for the following registries:`消息
-验证组织级私有注册表配置
-确保互联网访问是可用的依赖关系解决

## SARIF上传错误

SARIF文件太大

**限制：**最大10mb （gzip压缩）。* *解决方案:* *
-专注于最重要的查询套件（使用`default`而不是`security-and-quality`）
—通过配置减少查询的数量
-将分析拆分为多个具有单独SARIF上传的作业
—移除“`--sarif-add-file-contents`”标志

SARIF结果超出限制

GitHub强制限制SARIF数据对象：

|对象|最大|值|---|---|
|按文件运行| 20 |
|每次运行结果| 25,000 |
|每次运行规则| 25,000 |
|每次运行的工具扩展| 100 |
|每个结果的线程流位置| 10,000 |
|每个结果的位置| 1,000 |
|每个规则的标签| 20 |

* *解决方案:* *
—缩小查询范围，专注于影响较大的规则
-拆分分析跨多个SARIF上传与不同的`--sarif-category`-禁用产生许多结果的嘈杂查询

SARIF文件无效

* *解决方案:* *
-根据[Microsoft SARIF验证器]（https://sarifweb.azurewebsites.net/）进行验证
—确保`version`为`"2.1.0"`，并且`$schema`指向正确的模式
—验证所需属性（`runs`,`tool.driver`,`results`）是否存在

上传被拒绝：默认设置已启用

**原因：当默认设置激活时，无法上传codeql生成的SARIF。

* *解决方案:* *
-在通过CLI/API上传之前禁用默认设置
-或者切换到只使用默认设置（不手动上传）缺少认证令牌

* *解决方案:* *
—设置环境变量`GITHUB_TOKEN`，作用域为`security-events: write`-或使用`--github-auth-stdin`管道令牌
—对于GitHub Actions：令牌通过`${{ secrets.GITHUB_TOKEN }}`自动可用