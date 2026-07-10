---
name: pester-should-migration
description: 'Experimental (preview) Pester skill for migrating classic Should -Be (v5) assertion syntax to the new Should-* (v6) assertions (note the hyphen, no space), e.g. `Should -Be` -> `Should-Be`, `Should -Not -Be` -> `Should-NotBe`. Tracks Pester 6, which is still a release candidate, so this guidance may change; verified against Pester 6.0.0-rc2. Use when converting Pester v5 assertions to Pester v6 Should-* operators, modernizing a Pester test suite, or when a user asks to migrate, convert, or rewrite `Should -...` calls in .Tests.ps1 / PowerShell files.'
argument-hint: "File, folder, or test suite to migrate"
---
#纠缠`Should -*`→`Should-*`迁移

将经典的Pester v5断言（`Should -Be`，空格然后参数）转换为
新的Pester v6`Should-*`断言（`Should-Be`，连字符，无空格）。

> **状态：实验/预览。**针对Pester 6.0.0-rc2进行验证。经典的
>`Should -Be`风格在v6中仍然有效，所以要逐步迁移并保持套件绿色。

> **同伴技能。**该技能涵盖了*可选的*移动到新的`Should-*`操作符。
要跨主要的Pester版本（v3→v4→v5→v6—运行时、模拟和配置）升级套件，
b>使用单独的“骚扰-迁移”技能。在v6中，经典的`Should -Be`继续工作，所以
>采用`Should-*`是独立于任何版本的碰撞。

##何时使用

-将Pester套件现代化到v6`Should-*`断言。
—用户请求迁移/转换/重写`Should -...`调用。
您希望从新断言中获得更清晰、类型感知的失败消息。##先了解这一点

- **两种语法在Pester v6中并肩工作。**迁移是可选的，可以
一次只做一个测试（或一个文件）。如果你留下一些经典的东西，什么都不会坏。
- **需要纠缠v6+。** v5版本不存在`Should-*`命令。
—**否定是一个单独的命令**，而不是`-Not`开关：`Should -Not -Be`→`Should-NotBe`。在新的断言中没有`-Not`参数。
- **实际值仍然来自管道** (`$x | Should-Be 1`)或来自`-Actual`(`Should-Be -Actual $x -Expected 1`)。`-Because`保持不变。
- **大多数重命名是机械的，但有几个有你必须检查的行为变化
手动-参见[陷阱]（#step-3- check-the-behavior -陷阱-do-not-skip）。

# #过程

###步骤1 -找到经典断言

在目标中搜索经典的空格分隔语法(告诉是`Should -`，
或`Should`接`-Not`)：```
Should -          # any classic operator
Should -Not -     # negated classic operator
Assert-MockCalled # also removed in v6 -> Should-Invoke
```
将范围限制为PowerShell测试文件（`*.Tests.ps1`,`*.ps1`）。

###步骤2 -应用映射

最常用的转换（完整列表见[references/assertion-map.md](references/assertion-map.md)）：

|经典（v5） |新（v6） ||---|---|
|`$x \| Should -Be 1`|`$x \| Should-Be 1`|
|`$x \| Should -Not -Be 1`|`$x \| Should-NotBe 1`|
|`$x \| Should -BeExactly 'A'`|`$x \| Should-BeString 'A' -CaseSensitive`|
|`$x \| Should -BeGreaterOrEqual 2`|`$x \| Should-BeGreaterThanOrEqual 2`|
|`$x \| Should -BeLessOrEqual 2`|`$x \| Should-BeLessThanOrEqual 2`|
|`$x \| Should -BeLike 'a*'`|`$x \| Should-BeLikeString 'a*'`|
|`$x \| Should -Match 're'`|`$x \| Should-MatchString 're'`|
|`$x \| Should -BeOfType [int]`|`$x \| Should-HaveType ([int])`|
|`$x \| Should -BeNullOrEmpty`|视情况而定-参见gotchas（没有单一等效）|
|`$c \| Should -HaveCount 3`|`$c \| Should-BeCollection -Count 3`|
|`$c \| Should -Contain 2`|`$c \| Should-ContainCollection 2`|
|`{ ... } \| Should -Throw 'msg'`|`{ ... } \| Should-Throw -ExceptionMessage 'msg'`|
|`Should -Invoke Get-Thing`|`Should-Invoke Get-Thing`|
|`Should -InvokeVerifiable`|`Should-Invoke -Verifiable`|

###步骤3 -检查行为陷阱（不要跳过）

这些**不**通过简单的重命名进行翻译。在转换之前请阅读：1. * *敏感性。**经典的`Should -Be`对字符串不区分大小写；所以`Should-Be`。但是经典的`Should -BeExactly`（区分大小写）有**no** plain
等效-使用`Should-BeString -CaseSensitive`。`$x \| Should-NotBe 1`永远不会
区分大小写)。`BeLikeExactly`→`Should-BeLikeString -CaseSensitive`的模式相同`MatchExactly`→`Should-MatchString -CaseSensitive`。
2. **真相vs真实。**经典`Should -BeTrue`/`-BeFalse`接受任何*真相* /
* false * value （`1`,`'x'`,`0`,`''`,`$null`,`@()`）。新的`Should-BeTrue`/`Should-BeFalse`是**严格的**（确切地说是`$true`/`$false`）。保存旧的
松散行为使用`Should-BeTruthy`/`Should-BeFalsy`。只使用严格的
当值确实是布尔值时。
3. **`BeNullOrEmpty`没有单一的等价物。**按意图选择：`$null`→`Should-BeNull`;空字符串→`Should-BeEmptyString`；空集合→`Should-BeCollection -Count 0`;宽“假”→`Should-BeFalsy`。的否定`Should -Not -BeNullOrEmpty`类似地拆分为`Should-NotBeNull`/`Should-NotBeEmptyString`/`Should-NotBeWhiteSpaceString`。
4. * *馆藏ns。**经典`Should -Be`也比较数组；新的`Should-Be`是
如果`-Expected`是一个集合(“您提供了一个
收集到“预期参数”)。使用`Should-BeCollection`比较数组。`Should -Contain`（单项会员）→`Should-ContainCollection`。新
Command也接受一个期望项目的集合，并检查它们是否都存在。
按正确的顺序（`1, 2, 3 | Should-ContainCollection @(1, 2)`）。确切的,
全集合相等使用`Should-BeCollection`代替。
5. * *管道展开。**管道展开输入：值断言看到`@(1)`将`1`和`@()`重新收集为`$null`，并将类型化集合（`[int[]]`）重新收集为`[object[]]`。当确切的值或具体的集合类型很重要时(例如：`Should-HaveType`)，用`-Actual`而不是管道传递。
6. **不等同于`Should-*`。**`Should -Exist`和`Should -FileContentMatch*`家庭没有新的对应物。要么保留经典的asse咖啡重写，或者重写
PowerShell:`Test-Path $p | Should-BeTrue`,`(Get-Content $p -Raw) | Should-MatchString 're'`。
7. * *`Should -BeIn`方向。** No`Should-BeIn`。反转操作数：`$value | Should -BeIn $collection`→`$collection | Should-ContainCollection $value`（注意actual/expected交换），或者保持经典形式。###步骤4 -验证

运行套件并确认它仍然是绿色的-新消息不同，但通过
必须停留通行证：```powershell
Invoke-Pester -Path ./tests
```
如果转换的断言最近失败，请重新检查上面的陷阱（最常见的是#2）truthy/falsy， #3为空或为空，或#4为集合)。

###步骤5 -（可选）执行新样式

一旦一个套件被完全迁移，关闭经典语法，这样它就不会爬回来了：```powershell
$config = New-PesterConfiguration
$config.Should.DisableV5 = $true
```
有了这个集合，任何剩余的`Should -Be`都会抛出并指向`Should-Be`形式。

# #输出

总结改变了什么：被触摸的文件，被转换的断言的计数，任何经典
有意留下断言（例如`Should -Exist`），以及任何需要的转换
人工决策（truthy/falsy， null或empty，集合语义）。

# #参考

- [references/assertion-map.md](references/assertion-map.md) -完全逐个操作符
表中有before/after示例和变通方法。
—实时命令参考：`https://pester.dev/docs/commands/Should-Be`(swap in any .`Should-*`名称)获取准确的参数和示例。
-概念：`https://pester.dev/docs/assertions/should-command`（值与集合）
断言、管道与`-Actual`)。
- v5→v6升级指南：`https://pester.dev/docs/migrations/v5-to-v6`。