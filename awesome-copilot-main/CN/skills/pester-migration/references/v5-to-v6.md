#迁移Pester v5→v6

Pester 6构建于v5之上，并且**很大程度上是向后兼容的**——大多数v5套件都运行于v6之上，而没有
更改后，现有的`Should -Be`断言继续工作。工作是修理一把
以前弃用的行为，现在抛出。这是一个低到中等努力的跳跃。

b> Pester 6目前是一个**候选版本**。安装与
>`Install-Module Pester -AllowPrerelease -Force`(在Windows上添加`-SkipPublisherCheck`b> PowerShell 5.1)。在最终发布之前，一些细节可能仍会发生变化-检查发布说明：
>https://github.com/pester/Pester/releases.官方指南：https://pester.dev/docs/migrations/v5-to-v6---

##快速升级清单

对于大多数套房来说，这是低风险的。浏览一下这些内容，然后仔细阅读其中的细节：1. 运行在**Windows PowerShell 5.1**或**PowerShell 7.4+**（旧的PS被删除）。
2. 检查任何可以为空的`-ForEach`/`-TestCases`-它现在**抛出**，除非你添加`-AllowNullOrEmptyForEach`。
3. 删除同一块中重复的`BeforeAll`/`BeforeEach`/`AfterAll`/`AfterEach`。
4. 将`Assert-MockCalled`/`Assert-VerifiableMock`替换为`Should -Invoke`/`Should -InvokeVerifiable`。
5. 添加一个默认的`Mock`，其中一些调用与`-ParameterFilter`不匹配- mock不再下降
接通真正的命令。
6. 如果使用v4样式的参数（`-Script`,`-OutputFile`，…）调用`Invoke-Pester`，请切换到`New-PesterConfiguration`。

---

破坏性更改（症状→修复）

仅限PowerShell 5.1和7.4+
PowerShell 3、4、6和early/unsupported7都被删除了（所有这些都不受Microsoft的支持）
让Pester把它的c#移到。NET 8 （net462 for Windows PowerShell 5.1）。- **症状：**在较旧的PowerShell上无法导入。
**修复：**更新您的机器和CI代理到Windows PowerShell 5.1或PowerShell 7.4+。

###发现和运行现在发生每个文件
在v5中，运行有两个全局阶段：发现**每个**文件，然后运行每个文件。在v6单元中
每个工作都是一个文件- Pester发现一个文件并运行它，然后再移动到下一个文件。这是
是什么使实验并行运行，和串行运行遵循相同的模型。- **症状：**一个文件依赖于*另一个*文件的发现时间副作用失败-例如a
在文件顶部导入的模块、全局变量、已更改的工作目录或
在另一个文件中定义的`-ForEach`数据。
- **修复：**使每个测试文件自包含。在`BeforeDiscovery`中进行发现时间设置，以及
在它自己的`BeforeAll`中导入文件所需的模块。对于每个文件都需要的设置，使用`Run.BeforeContainer`（config）或repo根目录下的`Pester.BeforeContainer.ps1`。```powershell
BeforeDiscovery {
    $cases = Get-Content "$PSScriptRoot/cases.json" | ConvertFrom-Json
}
Describe 'MyModule' {
    BeforeAll { Import-Module "$PSScriptRoot/MyModule.psm1" }
    It 'handles <Name>' -ForEach $cases { Invoke-Thing $Name | Should -Be 'ok' }
}
```
屏幕上的输出也会发生变化：一个`Running tests from N files.`横幅，每个文件结果，然后一个
单个总金额。旧的`Starting discovery in N files.`/`Discovery found X tests`框架是
去正常跑步了。并行运行程序（`Run.Parallel`）与少数对象保持相同的结果对象
边缘情况-参见https://pester.dev/docs/usage/result-object#parallel-runner-edge-cases.###空或`$null``-ForEach`抛出`-ForEach`（或`-TestCases`）给定`$null`或`@()`现在抛出而不是静默跳过。这
捕获将`-ForEach`指向未定义的变量的常见错误`BeforeDiscovery`，或者外部数据加载失败。

- * *症状:* *  ```
  Value can not be null or empty array. If this is expected, use -AllowNullOrEmptyForEach
  on this Describe, or set the Run.FailOnNullOrEmptyForEach configuration option to $false ...
  ```
- **修正：**当数据可以合法为空，允许它在特定的block/test：  ```powershell
  Describe 'Optional cases' -ForEach $cases -AllowNullOrEmptyForEach {
      It 'runs only when there is data' { }
  }
  ```
您*可以*使用`Run.FailOnNullOrEmptyForEach = $false`禁用整个运行的检查，但是
这又带来了无声的跳跃，它的目的是捕捉-宁愿修复数据或使用`-AllowNullOrEmptyForEach`，其中真正期望的是空。

###重复setup/teardown块抛出
每个块只能有一个`BeforeAll`/`BeforeEach`/`AfterAll`/`AfterEach`。两个一样的人
（一个常见的复制粘贴错误）在v5中被默默地允许；v6抛出。

- **现象：**`BeforeAll is already defined in this block. Each block can only have one BeforeAll.`- **修复：**合并成一个块：  ```powershell
  Describe 'd' {
      BeforeAll {
          $a = 1
          $b = 2   # was a second BeforeAll
      }
  }
  ```
###测试名将`<...>`模板计算为表达式
在v6中，`Describe`/`Context`/`It`名称中的每个`<...>`令牌的内容被求值为a
PowerShell表达式在测试的运行范围内（当前`-ForEach`项及其属性，范围内）
变量、算法、方法调用)。`<...>`以外的所有内容都保持文字形式。在v5中只有简单data/variable/property引用被替换。

- **现象：**一个名称在`<...>`中包含类似表达式的内容，用于渲染
现在计算。  ```powershell
  # v5 renders literally: "adds up to <($a + $b)>"; v6 evaluates: "adds up to 3"
  It 'adds up to <($a + $b)>' -ForEach @(@{ a = 1; b = 2 }) { }
  ```
- **修复（保留文字）：**用反勾号转义前导`<`：  ```powershell
  It 'adds up to `<($a + $b)`>' -ForEach @(@{ a = 1; b = 2 }) { }
  ```
删除`Assert-MockCalled`和`Assert-VerifiableMock`在v5中已弃用，在v6中已删除。

- **现象：**`The term 'Assert-MockCalled' is not recognized ...`- * *修复:* *  ```powershell
  # Assert-MockCalled     -> Should -Invoke
  # Assert-VerifiableMock -> Should -InvokeVerifiable
  Should -Invoke Get-Thing -Times 1 -Exactly
  Should -InvokeVerifiable
  ```
模拟不再通过真正的命令
在v5中，对不匹配任何`-ParameterFilter`模拟的模拟命令的调用会安静地运行
**real**命令。V6消除了这种隐式故障。

- * *症状:* *  ```
  No mock for command 'Get-Thing' matched the call: none of the parameter filters matched,
  and there is no default mock to fall back to. Add a default mock ...
  ```
- **修复：**为未过滤的调用添加默认模拟（没有`-ParameterFilter`），或扩大过滤器：  ```powershell
  Mock Get-Thing -MockWith { 'default' }                                # everything else
  Mock Get-Thing -ParameterFilter { $Name -eq 'a' } -MockWith { 'a' }   # special case
  ```
###`Set-ItResult -Pending`已移除`Pending`在v5中从未完全实现过，现在已经消失了。

- **现象：**`Parameter set cannot be resolved using the specified named parameters.`- **修复：**使用`-Inconclusive`或`-Skipped`，或标记测试`It … -Skip`：  ```powershell
  Set-ItResult -Inconclusive -Because 'not implemented yet'
  ```
代码覆盖在默认情况下使用Profiler跟踪器
Coverage不再在每个命令上设置断点；它使用分析器的跟踪器，这是很多
在大型代码库中速度更快。`CodeCoverage.UseBreakpoints`不再是实验性的，默认为`$false`。

- **症状：**覆盖数字与v5不同，您想要旧的行为。
- **修复：**`$config.CodeCoverage.UseBreakpoints = $true`。

###`CodeCoverage.OutputFormat = 'CoverageGutters'`删除
现在，所有覆盖率输出都相对于从`.git`找到的回购根（`Run.RepoRoot`）
目录)，所以普通`JaCoCo`已经与覆盖沟槽扩展。

—**现象：**将`OutputFormat`设置为`'CoverageGutters'`会抛出无效值错误。
**修复：**使用`JaCoCo`（默认）或`Cobertura`。`Invoke-Pester`遗留（v4）参数已删除
只有**简单**设置（`-Path`,`-Output`,`-Container`,`-Tag`，…）和**高级**设置
(`-Configuration`)依然存在。不再使用v4风格的参数集。- **症状：**调用类似“Invoke-Pester - script…- outputfile…- outputformat…- enableexit”
-CodeCoverage…'由于参数绑定错误而失败。
- **修复：**使用一个配置对象(完整参数→配置映射是在
“`Invoke-Pester`参数→`New-PesterConfiguration`”部分[v4-to-v5.md](v4-to-v5.md))：  ```powershell
  $config = New-PesterConfiguration
  $config.Run.Path = './tests'
  $config.Run.Exit = $true
  $config.TestResult.Enabled  = $true
  $config.TestResult.OutputPath = 'result.xml'
  $config.TestResult.OutputFormat = 'NUnitXml'
  $config.CodeCoverage.Enabled = $true
  $config.CodeCoverage.Path = './src'
  Invoke-Pester -Configuration $config
  ```
> v6配置方便：在`TestResult`或中设置**any**非默认选项
>`CodeCoverage`section自动启用该部分，因此您不再需要设置`.Enabled = $true`>单独设置，以便编写报告。

---

新的`Should-*`断言-可选，不是升级的一部分

V6增加了一个新的断言家族——`Should-Be`、`Should-Throw`、`Should-Invoke`，以及大约40多个
（注意连字符：`Should-Be`，一个不同的命令，而不是经典的`Should -Be`）—更清晰
失败的消息。**不需要修改现有的`Should -Be`断言来升级**；
他们继续工作。将任何`Should -Be`→`Should-Be`重写视为单独的、以后的工作，而不是
这是迁移的一部分。参考:https://pester.dev/docs/commands/Should-Be.如果您尝试使用它们：新的断言将从管道或`-Actual`获取实际值。这个
管道展开输入(因此`@(1)`变为`1`，`@()`变为`$null`，并且集合为
重新收集为`[object[]]`，丢失类型（如`[int[]]`）。当需要精确时，使用`-Actual`值或具体集合类型。

---

## v5→v6检查表-[]套件在v5（基线）上首先运行绿色。
-[]运行于Windows PowerShell 5.1或PowerShell 7.4+。
- []`-ForEach`/`-TestCases`，可以为空标记`-AllowNullOrEmptyForEach`（或数据固定）。
- []`Before*`/`After*`重复块组合。
- []`Assert-MockCalled`→`Should -Invoke`；`Assert-VerifiableMock`→`Should -InvokeVerifiable`。
-[]默认添加`Mock`的地方，调用可以错过每个`-ParameterFilter`。
- []`Set-ItResult -Pending`→`-Inconclusive`/`-Skipped`。
- []`<...>`test-name应该保持文字的模板被反斜杠转义。
-[]每个测试文件都是自包含的（不依赖于另一个文件的发现时间状态）。
- []`Invoke-Pester`遗留参数→`New-PesterConfiguration`。
-[]覆盖范围`OutputFormat`为`JaCoCo`/`Cobertura`；`UseBreakpoints`仅在需要旧号码时设置。
[] v6的套件绿色diff审查;提交。