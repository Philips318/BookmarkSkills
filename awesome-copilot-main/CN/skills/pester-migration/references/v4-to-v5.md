#迁移Pester v4→v5

这是硬跳。V5引入了一个新的运行时，它将测试运行分为两个阶段
**发现**和**运行**——这改变了你必须*构建*测试的方式。它不是纯的
查找和替换。在编辑套件之前，请阅读整个文件。

官方指南：https://pester.dev/docs/migrations/v4-to-v5·
突破性的变化：https://pester.dev/docs/migrations/breaking-changes-in-v5---

##一个解释一切的概念：发现和运行

一次v5+的运行需要两次传递：

- **发现** - Pester从上到下执行每个`*.Tests.ps1`文件，但仅用于*查找*测试。
它调用`Describe`/`Context`脚本块来收集`It`s树，计算`It``-Name`字符串和`-TestCases`/`-ForEach`数据，并记录`BeforeAll`/`It`/等。脚本块
**不运行它们**。
- **运行** -然后，Pester以正确的范围执行记录的设置、测试和拆除。

使套件v5正确的两条规则1. 将所有测试代码放入`It`、`BeforeAll`、`BeforeEach`、`AfterAll`或`AfterEach`中。
2. **no**测试代码直接放在`Describe`/`Context`主体或在文件的顶部-
除非它的目的是构建测试，在这种情况下，它将放在`BeforeDiscovery`中。

位于`Describe`主体或文件顶层的松散代码在**Discovery**期间运行
在**运行**期间，结果通常**不**可用。这是大多数“它起作用”的根本原因
V4版本的bug是`$null`。

---

##修复1 -移动文件设置到`BeforeAll`并使用`$PSScriptRoot`经典的v4头在文件作用域中使用`$MyInvocation.MyCommand.Path`。在v5中，位置和变量都中断了。```powershell
# BEFORE (v4)
$here = Split-Path -Parent $MyInvocation.MyCommand.Path
$sut  = (Split-Path -Leaf $MyInvocation.MyCommand.Path).Replace('.Tests.', '.')
. "$here\$sut"

Describe 'Get-Cactus' {
    It 'Returns 🌵' { Get-Cactus | Should -Be '🌵' }
}
```

```powershell
# AFTER (v5+)
BeforeAll {
    # Do NOT use $MyInvocation.MyCommand.Path here.
    . $PSScriptRoot/Get-Cactus.ps1
    # or, by convention from the test file name:
    # . $PSCommandPath.Replace('.Tests.ps1', '.ps1')
}

Describe 'Get-Cactus' {
    It 'Returns 🌵' { Get-Cactus | Should -Be '🌵' }
}
```
为什么`$MyInvocation.MyCommand.Path`失败：它只在直接在
脚本的身体。在*任何*函数或脚本块中（`BeforeAll`是一个脚本块）`Path`是
空的。使用`$PSScriptRoot`（测试文件的目录）或`$PSCommandPath`（测试文件的满目录）
路径)。`string.Replace('.Tests.ps1','.ps1')`区分大小写，请保留`.Tests.ps1`套管精确。

>`$MyInvocation.MyCommand.Path`很好*在你的module/product代码* -只有改变
>影响测试文件头模式。看到
>https://pester.dev/docs/usage/importing-tested-functions#migrating-from-pester-v4.有一个社区迁移脚本为您执行BeforeAll包装（查看其输出）：https://gist.github.com/nohwnd/d488bd14ab4572f92ae77e208f476ada
---

##修复2 -生成测试`BeforeDiscovery`+`-ForEach`，而不是松散的`foreach`一个非常常见的v4模式从具有顶级`foreach`的数据构建测试。在v5中，数据经常
在发现时没有定义，因此没有生成测试，或者缺少每项变量`It`内部。```powershell
# BROKEN in v5: $files is set in BeforeAll (Run phase), but the foreach runs in Discovery
BeforeAll { $files = Get-ChildItem *.ps1 }
foreach ($file in $files) {
    Describe "$file is correct" {
        It 'has empty line at end' { }
    }
}
```
必须更改两件事：在`BeforeDiscovery`中构建数据（因此它在发现期间存在）
使用`-ForEach`/`-TestCases`将每项数据传递到测试中（因此`It`可以在运行期间看到它）：```powershell
BeforeDiscovery {
    $files = Get-ChildItem *.ps1            # runs during Discovery
}

Describe 'script <_> is correct' -ForEach $files {
    It 'has an empty line at the end' {
        # $_ is the current file here
    }
}
```
优先使用块上的`-ForEach`/`It`而不是手写的`foreach`；它既创建了副本，又
使当前项可用。在名称中使用`<_>`（或`<Name>`用于哈希表项）来
模板每个项目的标题。参考:https://pester.dev/docs/usage/data-driven-tests.---

##修复3 -`-Skip`和`-TestCases`在发现期间被评估

因为过滤器和数据是在Discovery期间解析的，所以在`BeforeAll`中计算的条件不是
可用。```powershell
# DOES NOT skip: $isSkipped is set in BeforeAll (Run), but -Skip is read in Discovery
Describe 'd' {
    BeforeAll { $isSkipped = Get-IsSkipped }
    It 'i' -Skip:$isSkipped { }
}
```
将便宜的跳过逻辑移到文件作用域（它在每个Discovery上运行），或者更好地将其基于静态
全局像`$IsWindows`：```powershell
$isSkipped = -not $IsWindows
Describe 'd' {
    It 'i' -Skip:$isSkipped { }
}
```
保持发现时代码的低成本——它在每次发现文件时都运行，这种情况可能经常发生。

---

##修复4 -变量不会从发现泄漏到测试

在发现过程中定义的变量在`BeforeAll/-Each`、`AfterAll/-Each`或`It`。如果您在生成测试时计算了某些内容，并且在运行时需要它，请将其附加到
测试通过`-ForEach`/`-TestCases`。(`TestDrive`是Run-only，同样不能在`-ForEach`)。

---

##修复5 -`Should -Throw`与`-like`通配符匹配

在v5中，`Should -Throw <message>`用`-like`匹配异常消息，而不是`.Contains()`。一个
用于匹配的子字符串现在需要通配符。```powershell
# v4: matched a substring
{ throw 'connection failed: timeout' } | Should -Throw 'timeout'
# v5+: use wildcards to match part of the message
{ throw 'connection failed: timeout' } | Should -Throw '*timeout*'
```
---

##修复6 -模拟：范围，调试和`InModuleScope`- **范围跟随位置。**在v5中，mock（及其调用计数）的作用域限定在您放置的位置
它们是当前的block/test，而不是整个`Describe`/`Context`。类中定义`Mock`与它应用的`BeforeAll`/`It`范围相同，并在该范围内断言计数。
**`Assert-VerifiableMocks`被移除。**使用`Should -InvokeVerifiable`。(`Assert-MockCalled``Assert-VerifiableMock`仍然存在，但在v5中被*弃用*，在v6中被**删除** -首选`Should -Invoke`/`Should -InvokeVerifiable`现在保存第二次迁移。看到
[v5-to-v6.md] (v5-to-v6.md)。)
- ** mock可调试。** v5不再重写模拟脚本块，因此您可以设置`-MockWith`和`-ParameterFilter`内部的断点。
**避免`InModuleScope`靠近`Describe`/`It`。**在发现期间加载模块（减速）
它向下)，并允许您测试内部而不是发布的表面。首选`-ModuleName`on`Mock`和`Should -Invoke`；如果必须使用`InModuleScope`，请将其保存在`It`内。看到  https://pester.dev/docs/usage/mocking.

```powershell
# Prefer this over wrapping the whole Describe in InModuleScope
Mock Get-Internal -ModuleName MyModule { 'mocked' }
Should -Invoke Get-Internal -ModuleName MyModule -Times 1 -Exactly
```
---

修复7 -`Invoke-Pester`参数→`New-PesterConfiguration``Invoke-Pester`的接口被彻底修改了。V5保留了一个**弃用的**兼容性集，因此v4调用
大多数仍然运行（带有警告），但您应该移动到**Simple**参数或
**高级**`-Configuration`对象。（v6完全删除了遗留集-现在迁移。）

**简单接口**（参数→配置属性）：

|简单参数|配置属性||---|---|
|`-Path`|`Run.Path`|
|`-ExcludePath`|`Run.ExcludePath`|
|`-Tag`|`Filter.Tag`|
|`-ExcludeTag`|`Filter.ExcludeTag`|
|`-FullNameFilter`|`Filter.FullName`|
|`-Output`|`Output.Verbosity`|
|`-CI`|`TestResult.Enabled`+`Run.Exit`（都是`$true`） |
|`-PassThru`|`Run.PassThru`|

**Legacy （v4）参数→config:**

| v4参数|配置属性||---|---|
|`-Script`|`Run.Path`（只有路径-没有哈希表）|
|`-EnableExit`|`Run.Exit`|
|`-TestName`|替换为`-FullNameFilter`/`Filter.FullName`|
|`-CodeCoverage`|`CodeCoverage.Path`(+`CodeCoverage.Enabled = $true`) |
|`-CodeCoverageOutputFile`|`CodeCoverage.OutputPath`|
|`-CodeCoverageOutputFileEncoding`|`CodeCoverage.OutputEncoding`|
|`-CodeCoverageOutputFileFormat`|`CodeCoverage.OutputFormat`|
|`-OutputFile`|`TestResult.OutputPath`(+`TestResult.Enabled = $true`|`-OutputFormat`|`TestResult.OutputFormat`|
|`-Show`/`-Output`|`Output.Verbosity`（见下面的映射）|
|`-PesterOption`，`-Strict`|被忽略/不可用|`-Show`值→`Output.Verbosity`:`All`/`Default`/`Detailed`→`Detailed`；`Fails`/`Normal`→`Normal`;`Diagnostic`→`Diagnostic`；`Minimal`→`Minimal``None`→`None`。```powershell
# BEFORE (v4 legacy)
Invoke-Pester -Script ./tests -CodeCoverage ./src/*.ps1 `
    -OutputFile result.xml -OutputFormat NUnitXml -EnableExit

# AFTER (v5 Advanced)
$config = New-PesterConfiguration
$config.Run.Path = './tests'
$config.Run.Exit = $true
$config.CodeCoverage.Enabled = $true
$config.CodeCoverage.Path = './src'
$config.TestResult.Enabled = $true
$config.TestResult.OutputPath = 'result.xml'
$config.TestResult.OutputFormat = 'NUnitXml'
Invoke-Pester -Configuration $config
```
`-Output Diagnostic`是您迁移时最好的朋友——它显示Discovery/Skip/Mock决策。

---

新的结果对象

v5的结果对象更加丰富，也是Pester内部使用的对象。以保持v4版本的CI
管道工作，用`ConvertTo-Pester4Result`转换。用于NUnit输出`ConvertTo-NUnitReport`，或者传递`-CI`以启用NUnit输出、代码覆盖和失败退出
代码在一个开关。每个测试的`-TestCases`/`-ForEach`项在测试对象的`Data`财产。

---

其他在v5中删除/更改的东西

- **PowerShell 2**不再支持。
- **旧的`Should Be`**（无破折号）被删除-转换为`Should -Be`（[v3-to-v4.md](v3-to-v4.md)）。
**小黄瓜**被移除-如果你需要，留在恶魔v4
-`-Output`/`-Show`简化为`None`，`Normal`,`Detailed`,`Diagnostic`。
-`-TestName`→`-FullNameFilter`；`-Script`→`-Path`（仅限路径）；`-PesterOption`移除。

---

## v4→v5检查表—[]套件在v4优先（基线）上运行绿色。
-[]文件导入移动到`BeforeAll`；`$MyInvocation.MyCommand.Path`替换为      `$PSScriptRoot`/`$PSCommandPath`.
-[]在`Describe`/`Context`正文或文件顶层没有松散的代码；移动测试生成代码      to `BeforeDiscovery`.
- []`foreach`-生成的测试转换为`-ForEach`；每项数据通过`-ForEach`/`-TestCases`传递。
- []`-Skip:`条件不依赖于`BeforeAll`变量。
—[]`Should -Throw`消息使用`-like`通配符（`*...*`）。
-[]在正确范围内定义的mock；`Assert-VerifiableMocks`→`Should -InvokeVerifiable`；      `InModuleScope` removed from around `Describe`/`It` in favor of `-ModuleName`.
—[]`Invoke-Pester`调用转换为简单参数或`New-PesterConfiguration`。
-`-Output Detailed`的v5套件绿色；diff审查;提交。