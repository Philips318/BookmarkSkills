`Should -*`→`Should-*`完整断言映射

从经典的Pester v5断言到
纠缠v6`Should-*`断言。这里的每一个签名都来自于恶魔v6
命令参考。对于权威的、始终当前的参数和示例，
打开`https://pester.dev/docs/commands/<Name>`（例如`.../Should-Be`）。

下面使用的约定：
-`$x`=实际值（通过管道，或通过`-Actual`传递）。
—普通重命名是指：将`Should -Operator`更改为`Should-Operator`，并保留pipeline/arguments.其他的都被调用了。

---

相等/比较

|经典v5 | Pester v6 | Notes ||---|---|---|
|`$x \| Should -Be 1`|`$x \| Should-Be 1`|不区分大小写（`-eq`语义）。|
|`$x \| Should -Not -Be 1`|`$x \| Should-NotBe 1`|否定是它自己的命令。|
|`$x \| Should -BeExactly 'A'`|`$x \| Should-BeString 'A' -CaseSensitive`|`Should-Be`是**不区分**大小写；使用`Should-BeString -CaseSensitive`。|
|`$x \| Should -Not -BeExactly 'A'`|`$x \| Should-NotBeString 'A' -CaseSensitive`| |
|`$x \| Should -BeGreaterThan 1`|`$x \| Should-BeGreaterThan 1`|普通重命名。|
|`$x \| Should -BeGreaterOrEqual 1`|`$x \| Should-BeGreaterThanOrEqual 1`| **更名为** (`...ThanOrEqual`)。|
|`$x \| Should -BeLessThan 1`|`$x \| Should-BeLessThan 1`|普通重命名。|
|`$x \| Should -BeLessOrEqual 1`|`$x \| Should-BeLessThanOrEqual 1`| **更名为** (`...ThanOrEqual`)。|

没有否定的比较命令（例如，没有`Should-NotBeGreaterThan`）。反
在需要时使用相反运算符（`Should-BeLessThanOrEqual`）的逻辑。

---

##字符串（pattern / regex / whitespace）

|经典v5 | Pester v6 | Notes ||---|---|---|
|`$x \| Should -BeLike 'a*'`|`$x \| Should-BeLikeString 'a*'`|通配符匹配，默认不区分大小写。|
|`$x \| Should -BeLikeExactly 'a*'`|`$x \| Should-BeLikeString 'a*' -CaseSensitive`| |
|`$x \| Should -Not -BeLike 'a*'`|`$x \| Should-NotBeLikeString 'a*'`| |
|`$x \| Should -Match 're'`|`$x \| Should-MatchString 're'`| Regex匹配，默认不区分大小写。|
|`$x \| Should -MatchExactly 're'`|`$x \| Should-MatchString 're' -CaseSensitive`| |
|`$x \| Should -Not -Match 're'`|`$x \| Should-NotMatchString 're'`| |`Should-BeString`还提供`-IgnoreWhitespace`和`-TrimWhitespace`，加上
专用`Should-BeEmptyString`、`Should-NotBeEmptyString`和`Should-NotBeWhiteSpaceString`用于empty/whitespace检查(没有v5的对等物-它们
替换对字符串的`BeNullOrEmpty`样式检查)。

---

##布尔值/真实性

经典`BeTrue`/`BeFalse`是**truthy/falsy**检查。新的同名命令
**严格**（仅`$true`/`$false`）。故意选择:

|经典v5 | Pester v6（保留行为）| Pester v6（严格bool） ||---|---|---|
|`$x \| Should -BeTrue`|`$x \| Should-BeTruthy`|`$x \| Should-BeTrue`|
|`$x \| Should -BeFalse`|`$x \| Should-BeFalsy`|`$x \| Should-BeFalse`|`Should-BeFalsy`传递给`$false`、`0`、`''`、`$null`、`@()`。如果
Test是一个真正的布尔值，更喜欢严格的`Should-BeTrue`/`Should-BeFalse`。

---

## Null / empty`Should -BeNullOrEmpty`将多个检查合并为一个；V6将它们分开。选择的
实际值是什么：

| Intent | Classic v5 | Pester v6 ||---|---|---|
|的值为`$null`|`$x \| Should -BeNullOrEmpty`|`$x \| Should-BeNull`|
|空字符串|`'' \| Should -BeNullOrEmpty`|`'' \| Should-BeEmptyString`|
|空收集|`@() \| Should -BeNullOrEmpty`|`@() \| Should-BeCollection -Count 0`|
|任意假值|`$x \| Should -BeNullOrEmpty`|`$x \| Should-BeFalsy`|
| not null |`$x \| Should -Not -BeNullOrEmpty`|`$x \| Should-NotBeNull`|
|非空字符串|`$x \| Should -Not -BeNullOrEmpty`|`$x \| Should-NotBeEmptyString`|
|不是null/empty/whitespace|`$x \| Should -Not -BeNullOrEmpty`|`$x \| Should-NotBeWhiteSpaceString`|

当有疑问且类型混合时，`Should-BeFalsy`/`Should-NotBeNull`为
最接近的广义等价——但是特定于类型的断言提供了更好的消息。

---

# #类型

|经典v5 | Pester v6 | Notes ||---|---|---|
|`$x \| Should -BeOfType [int]`|`$x \| Should-HaveType ([int])`| |
|`$x \| Should -BeOfType 'System.Int32'`|`$x \| Should-HaveType ([System.Int32])`|新表单需要**类型文字**，而不是字符串类型名称。|
|`$x \| Should -Not -BeOfType [int]`|`$x \| Should-NotHaveType ([int])`| |

对于类型化集合，展开管道将`[int[]]`转换为`[object[]]`。使用`-Actual`保持实数：`Should-HaveType -Actual ([int[]](1,2)) -Expected ([int[]])`。

---

# #集合

|经典v5 | Pester v6 | Notes ||---|---|---|
|`$c \| Should -Be @(1,2,3)`|`$c \| Should-BeCollection @(1,2,3)`|`Should-Be`集合错误对数组使用`Should-BeCollection`。|
|`$c \| Should -HaveCount 3`|`$c \| Should-BeCollection -Count 3`| |
|`$c \| Should -Contain 2`|`$c \| Should-ContainCollection 2`|成员资格。按顺序传递一个项目或一个集合（`@(1, 2)`）以要求多个项目出现。|
|`$c \| Should -Not -Contain 2`|`$c \| Should-NotContainCollection 2`| |
|`$v \| Should -BeIn $c`|`$c \| Should-ContainCollection $v`|无`Should-BeIn`；操作数交换（实际变成集合）。|`Should-ContainCollection`检查实际文件中是否存在预期的项目
收集，以正确的顺序。传递单个项（`$c | Should-ContainCollection 2`）
或者一次需要几个的集合（`1, 2, 3 | Should-ContainCollection @(1, 2)`）。
对于精确的全集合相等，请使用`Should-BeCollection`。

新的组合子断言在v5中没有等价的，但在迁移中很方便：`$c | Should-All { $_ | Should-BeGreaterThan 0 }`和`$c | Should-Any { ... }`。

---

# #例外

|经典v5 | Pester v6 | Notes ||---|---|---|
|`{ ... } \| Should -Throw`|`{ ... } \| Should-Throw`|普通重命名。|
|`{ ... } \| Should -Throw 'msg'`|`{ ... } \| Should-Throw -ExceptionMessage 'msg'`|参数**从`-ExpectedMessage`改名为**。支持`-like`通配符。|
|`{ ... } \| Should -Throw -ErrorId 'X'`|`{ ... } \| Should-Throw -FullyQualifiedErrorId 'X'`|参数**重命名为**。|
|`{ ... } \| Should -Throw -ExceptionType ([T])`|`{ ... } \| Should-Throw -ExceptionType ([T])`|相同。|
|`{ ... } \| Should -Not -Throw`|`& { ... }; <no throw assertion needed>`|没有`Should-NotThrow`；不能抛出的脚本块只是运行。对其结果进行断言，或者保留经典的`Should -Not -Throw`。|`Should-Throw`添加`-AllowNonTerminatingError`并返回的错误记录
进一步断言：`$err = { throw 'boom' } | Should-Throw; $err.Exception.Message | Should-BeString '*boom*'`。

---

# #模拟

|经典v5 | Pester v6 | Notes ||---|---|---|
|`Should -Invoke Get-Thing`|`Should-Invoke Get-Thing`|普通重命名。|
|`Should -Invoke Get-Thing -Times 2 -Exactly`|`Should-Invoke Get-Thing -Times 2 -Exactly`|参数相同。|
|`Should -Invoke Get-Thing -ParameterFilter { ... }`|`Should-Invoke Get-Thing -ParameterFilter { ... }`|相同。|
|`Should -Not -Invoke Get-Thing`|`Should-NotInvoke Get-Thing`|单独命令。|
|`Should -InvokeVerifiable`|`Should-Invoke -Verifiable`|折叠成`-Verifiable`参数集。|`Assert-MockCalled`/`Assert-VerifiableMock`在v6中被完全移除-映射它们
到`Should-Invoke`/`Should-Invoke -Verifiable`。

---

命令元数据

|经典v5 | Pester v6 | Notes ||---|---|---|
|`Get-Command f \| Should -HaveParameter X -Mandatory`|`Get-Command f \| Should-HaveParameter X -Mandatory`|普通重命名。|
|`... \| Should -HaveParameter X -Type String`|`... \| Should-HaveParameter X -Type ([String])`|首选类型文字。|
|`... \| Should -HaveParameter X -DefaultValue 8`|`... \| Should-HaveParameter X -DefaultValue 8`|相同。|
|`... \| Should -Not -HaveParameter X`|`... \| Should-NotHaveParameter X`|单独命令。|

---

##没有`Should-*`等价（保持经典或重写）

这些v5操作符**没有** new断言。让它们保持经典的`Should -...`（两种语法共存），或者用PowerShell +一个新的断言重写：

|经典v5 |在v6 |的解决方案|---|---|
|`$p \| Should -Exist`|`Test-Path $p \| Should-BeTrue`|
|`$p \| Should -FileContentMatch 're'`|`(Get-Content $p -Raw) \| Should-MatchString 're'`|
|`$p \| Should -FileContentMatchExactly 're'`|`(Get-Content $p -Raw) \| Should-MatchString 're' -CaseSensitive`|
|`$p \| Should -FileContentMatchMultiline 're'`|`(Get-Content $p -Raw) \| Should-MatchString 're'`|
|`$p \| Should -FileContentMatchMultilineExactly 're'`|`(Get-Content $p -Raw) \| Should-MatchString 're' -CaseSensitive`|

(用`Get-Content -Raw`、`^`和`$`在正则表达式中匹配整个start/end文件，匹配多行操作符的行为。)

---

新的v6断言，没有v5对应的断言

迁移时值得去寻找的；它们经常取代尴尬的经典组合：

-`Should-BeSame`/`Should-NotBeSame`-引用相等（相同实例）。
-`Should-BeEquivalent`-递归，逐个属性的对象比较。
-`Should-BeHashtable`-断言hashtable/ordered字典形状、键和计数。
-`Should-BeBefore`/`Should-BeAfter`-`[datetime]`排序。
-`Should-BeFasterThan`/`Should-BeSlowerThan`-`[timespan]`/`[scriptblock]`定时。
-`Should-All`/`Should-Any`-运行过滤器或嵌套`Should-*`在每个项目。