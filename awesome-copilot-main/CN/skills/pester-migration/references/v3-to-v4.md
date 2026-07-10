#迁移Pester v3到v4

这是最小的跳转—主要是断言语法重命名。许多套房只需要微小的改动，
有些人不需要。它在很大程度上是脚本自动化的，但总是检查差异并重新运行套件。

官方指南：https://pester.dev/docs/migrations/v3-to-v4提醒：如果你的目标是一个现代的纠缠者（5或6），v3→v4只是第一步。做吧，得到
>绿色，然后继续使用[v4-to-v5.md]（v4-to-v5.md）和[v5-to-v6.md]（v5-to-v6.md）。

---

##更改1 -虚线`Should`断言语法

V4引入了参数风格的`Should`语法。裸字形式仍然在v4中运行，但是现在
**在v5**中删除，所以现在转换可以节省以后的迁移。```powershell
# v3 (bareword)
It 'checks something' { 10 | Should Be 10 }

# v4+ (dashed)
It 'checks something' { 10 | Should -Be 10 }
```
重命名适用于所有操作符：`Be`，`BeExactly`,`Match`,`Throw`,`BeNullOrEmpty`，
→`-Be`、`-BeExactly`、`-Match`、`-Throw`、`-BeNullOrEmpty`、…

有一个著名的基于ast的转换器，`Update-PesterTest`(Chris Dent / Wojciech Sciesinski)，
它通过解析文件而不是对其进行正则化来安全地插入破折号：https://gist.github.com/indented-automation/aeb14825e39dd8849beee44f681fbab3，也可以复制
在官方v3→v4指南中。检查它的输出，特别是non-UTF-8/ASCII文件的输出
改变编码。

---

##改变2 -`Contain`→`FileContentMatch``Contain`断言被重命名为`FileContentMatch`（它测试文件**内容**）
旧名称对集合包含有歧义)。```powershell
# Should Contain      -> Should -FileContentMatch
# Should Not Contain  -> Should -Not -FileContentMatch
'app.config' | Should -FileContentMatch 'setting'
'app.config' | Should -Not -FileContentMatch 'secret'
```
一个简单的基于正则表达式的迁移脚本，来自官方指南（验证结果-它可以产生）
假阳性):```powershell
$content = Get-Content -Path $file -Encoding $encoding
$content = $content -replace 'Should\s+\-?Contain',        'Should -FileContentMatch'
$content = $content -replace 'Should\s+\-?Not\s*-?Contain', 'Should -Not -FileContentMatch'
$content = $content -replace 'Assert-VerifiableMocks',      'Assert-VerifiableMock'
$content | Set-Content -Path $file -Encoding $encoding
```
---

##更改3 -`Assert-VerifiableMocks`→`Assert-VerifiableMock`重命名了cmdlet（去掉了后面的`s`）。重命名所有事件。

在Pester 5这是*弃用*和在Pester 6它是*删除* -当你继续过去的v4，
>切换到`Should -InvokeVerifiable`。[v5-to-v6.md] (v5-to-v6.md)。

---

##更改4 -数组断言（注意边缘情况）`Should`在v4中获得了数组断言。这对大多数测试来说都是透明的，但也有边缘
在v3下通过的阵列测试在v4下失败的情况。如果与阵列相关的测试发生变化
结果，手动检查它，而不是强制它通过。背景:https://github.com/pester/Pester/issues/873.
当Pester从函数转向别名时，嘲讽也发生了微妙的变化；不需要
更改，但如果模拟命令行为看起来不正常，请参阅https://github.com/pester/Pester/issues/810和https://github.com/pester/Pester/issues/812.---

## v3→v4检查表

—[]套件优先运行在v3（基线）上。
-[]所有`Should <Operator>`转换为`Should -<Operator>`（首选AST转换器）。
- []`Should Contain`→`Should -FileContentMatch`（和`-Not`形式）。
- []`Assert-VerifiableMocks`→`Assert-VerifiableMock`。
-[]数组断言行为更改手动审查。
-[]任何脚本替换所保留的文件编码。
- [] v4套件绿色；diff审查;提交。