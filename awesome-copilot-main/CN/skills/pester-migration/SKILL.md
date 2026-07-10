---
name: pester-migration
description: 'Experimental (preview) Pester migration skill for upgrading PowerShell Pester test suites across major versions — v3→v4, v4→v5, and v5→v6. The v5→v6 path tracks Pester 6, which is still a release candidate, so that guidance may change. Covers the Discovery/Run two-phase model, moving setup into BeforeAll, $PSScriptRoot vs $MyInvocation, mock changes (Assert-MockCalled → Should -Invoke, removed fall-through), Invoke-Pester parameters → PesterConfiguration, data-driven -ForEach/-TestCases, and the v6 breaking changes. Use when the user asks to upgrade, migrate, or modernize Pester tests, fix *.Tests.ps1 files that broke after bumping the Pester version, or convert legacy Should / Invoke-Pester syntax.'
---
#骚扰移民

> **实验/预览。**v5→v6**指南跟踪纠缠6，而它是一个版本
b>候选人，可能会改变；根据电流进行验证
>[发布说明]（https://github.com/pester/Pester/releases）。V3→v4和v4→v5涵盖稳定版本。

Pester是PowerShell的测试框架。测试文件以`*.Tests.ps1`结尾，并使用`Describe`/`Context`/`It`块与`Should`断言。此技能升级现有的
套件从一个主要的纠缠版本到下一个，并再次获得绿色。

心理模型：**每次主要跳跃都有不同的特征。**v3→v4**主要是一种语法
>重命名。**v4→v5**是一个*基本的运行时更改* （Discovery/Run分割），是最困难的
>。**v5→v6**很大程度上是向后兼容的——现在有一些以前不赞成的东西了
>。一次只迁移一个专业；永远不要跳过一个版本。详细的症状驱动指南在`references/`中-为您正在进行的跳跃加载指南。

# #引用

|参考|何时加载||---|---|
| [v3-to-v4.md](references/v3-to-v4.md) |`Should Be`→`Should -Be`,`Contain`→`FileContentMatch`,`Assert-VerifiableMocks`→`Assert-VerifiableMock`，数组断言边缘情况。|
| [v4-to-v5.md](references/v4-to-v5.md) |大的那个。Discovery/Run阶段，`BeforeAll`设置，`$PSScriptRoot`,`BeforeDiscovery`,`-ForEach`，模拟作用域，`Should -Throw`通配符，`Invoke-Pester`→`New-PesterConfiguration`。|
| [v5-to-v6.md](references/v5-to-v6.md) | PowerShell5.1/7.4+ only，按文件发现+运行，空`-ForEach`抛出，重复设置块抛出，名称`<...>`模板求值，`Assert-MockCalled`删除，模拟不再失败，代码覆盖跟踪器，遗留`Invoke-Pester`参数删除。|

规范来源：官方迁移指南https://pester.dev/docs/migrations/-此技能
镜子。当有疑问时，更喜欢网站。

##第0步-检测你在哪里，你要去哪里

找到已安装的版本和编写**测试**的版本。这些可能会有所不同。```powershell
# Installed Pester version(s) on this machine
Get-Module Pester -ListAvailable | Select-Object Name, Version, Path

# Version currently imported in the session
(Get-Module Pester).Version
```
用这些启发式方法从测试代码中分辨出源版本：

您可以在`*.Tests.ps1`/ build scripts中看到| Suite是为|编写的|---|---|
|`Should Be`/`Should Contain`（无破折号）| v3或更早版本→start at [v3-to-v4](references/v3-to-v4.md) |
|`$MyInvocation.MyCommand.Path`+。source在文件的**顶部**；`Describe`| v4→[v4-to-v5](references/v4-to-v5.md) |下的任意代码
|`Assert-MockCalled`,`Assert-VerifiableMock`，`Set-ItResult -Pending`| v4 /早期v5（这些**在v6**中删除）|
|`Invoke-Pester -Script … -OutputFile … -CodeCoverage …`（遗留参数）| v4调用→映射到配置|
|`BeforeAll { . $PSScriptRoot/… }`,`New-PesterConfiguration`，`Should -Invoke`|已经v5-style→[v5-to-v6](references/v5-to-v6.md) |

准备好后安装目标版本：```powershell
# Latest stable v5 — pin the major so this keeps installing v5 even after v6 goes GA
Install-Module Pester -MaximumVersion 5.99.99 -Force

# Pester 6 (currently a release candidate — needs -AllowPrerelease)
Install-Module Pester -AllowPrerelease -Force
```
在Windows PowerShell 5.1上，操作系统附带了一个微软签名的内置纠缠程序PowerShellGet
>不会用不同签名的新Pester覆盖-添加`-SkipPublisherCheck`到那里
>并排安装。在PowerShell 7+上不需要。看到
>https://pester.dev/docs/introduction/installation.##迁移流程

每次主要跳跃都运行这个循环。不要同时跳两个专业，先从v4跳到v5，再从v5跳到v6。

1. * *基线。**首先在**当前**版本上运行套件并记录pass/fail.已知-良好（或已知）的起点，因此您可以将迁移回归与
预先存在的失败。   ```powershell
   # Bare Invoke-Pester works on every major; exact parameters differ
   # (v3/v4: -Script/-OutputFile; v5+/v6: -Path/-Output).
   Invoke-Pester
   ```
2. **请阅读参考**，以便在编辑前了解完整的范围。
3. **按文件编辑文件。**应用机制变化（参见下面的每跳小抄）
引用)。保持变更小且可审查——一次一个文件或一个关注点。
4. **使用`Install-Module`切换版本**（步骤0），然后重新导入：' Remove-Module Pester；
导入-模块纠缠'（或开始一个新的会话）。
5. **运行并修复。**重新运行`-Output Detailed`；使用`-Output Diagnostic`（v4→v5）或读取
显式的v6错误消息来定位问题。将每个故障与**症状匹配→修复**
表中的引用。
6. 绿色，困难，承诺。**重新运行，直到结果匹配基线（或更好）。检查
困难，然后投入。在小的提交中迁移使得回归对分割来说很简单。

实际发生了什么变化（每次跳转的范围）

|跳跃|难度|自然||---|---|---|
|低|断言语法重命名（`Should -Be`）。很大程度上script-automatable。|
| v4→v5 | **高** |新增两相运行时。测试结构改变：设置必须移到`BeforeAll`，发现时间代码移到`BeforeDiscovery`，文件位置通过`$PSScriptRoot`。不是纯粹的“发现-取代”。|
| v5→v6 | Low-Medium |向后兼容的运行时；已弃用的特性现在抛出。大多数是小的、有针对性的修复。您的`Should -Be`断言保持不变。|

快速备忘单

v4→v5（最常见的修复）```powershell
# 1. Move file import into BeforeAll, use $PSScriptRoot (NOT $MyInvocation.MyCommand.Path)
# BEFORE
$here = Split-Path -Parent $MyInvocation.MyCommand.Path
. "$here\Get-Thing.ps1"
# AFTER
BeforeAll { . $PSScriptRoot/Get-Thing.ps1 }

# 2. Any code that DISCOVERS/generates tests must be in BeforeDiscovery
BeforeDiscovery { $cases = Get-Content $PSScriptRoot/cases.json | ConvertFrom-Json }

# 3. Should -Throw matches with -like wildcards, not .Contains
{ throw 'a long message' } | Should -Throw '*long*'

# 4. Invoke-Pester legacy params → New-PesterConfiguration (see reference for full map)
```
完整的细节、范围规则和参数→配置表：[references/v4-to-v5.md]（references/v4-to-v5.md）。

### v5→v6（最常见的修复）```powershell
# 1. Mock assertions: removed verbs — rename (old -> new):
#    Assert-MockCalled     -> Should -Invoke
#    Assert-VerifiableMock -> Should -InvokeVerifiable
Should -Invoke Get-Thing -Times 1 -Exactly
Should -InvokeVerifiable

# 2. Add a default mock — unmatched calls no longer run the real command
Mock Get-Thing { 'default' }
Mock Get-Thing -ParameterFilter { $Name -eq 'a' } -MockWith { 'a' }

# 3. Empty/$null -ForEach now throws; allow it only where empty is expected
Describe 'Optional' -ForEach $cases -AllowNullOrEmptyForEach { }

# 4. Combine duplicate BeforeAll/BeforeEach/AfterAll/AfterEach in the same block into one
```
包含症状和修复的完整中断更改列表：[references/v5-to-v6.md]（references/v5-to-v6.md）。

##安全规则

- **测试是规范。**迁移不能改变测试断言的内容——只能改变套件的形式
结构化和调用。如果测试以不同的方式启动passing/failing，而不是因为
文档化的破坏性变更，在接受之前进行调查。
- **自动迁移脚本产生误报。**社区脚本(链接在
参考资料)帮助了解`Should`语法和点源，但总是检查差异并重新运行
套件。永远不要批量编辑和未检查提交。
- **注意文件编码**当脚本替换超过`*.Tests.ps1`-保留原来的
编码（UTF-8 vs ASCII），这样您就不会混淆非ASCII测试名称。
- **在分支上工作，每个file/concern.提交**小的提交保持`git bisect`有用
迁移后的测试显示为红色。