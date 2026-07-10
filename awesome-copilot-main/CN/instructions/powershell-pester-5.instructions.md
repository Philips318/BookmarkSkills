---
applyTo: '**/*.Tests.ps1'
description: 'PowerShell Pester testing best practices based on Pester v5 conventions'
---
# PowerShell Pester v5测试指南

本指南提供了使用PowerShell Pester v5模块创建自动化测试的PowerShell特定说明。遵循[powershell.instructions.md]（./powershell.instructions.md）中的PowerShell cmdlet开发指南，了解一般的PowerShell脚本最佳实践。

文件命名和结构

- **文件约定：**使用`*.Tests.ps1`命名模式
**放置：**将测试文件放置在测试代码旁边或专用测试目录中
—**导入模式：**使用`BeforeAll { . $PSScriptRoot/FunctionName.ps1 }`导入测试函数
**将所有代码放入Pester块（`BeforeAll`,`Describe`,`Context`，`It`等）

测试结构层次结构```powershell
BeforeAll { # Import tested functions }
Describe 'FunctionName' {
    Context 'When condition' {
        BeforeAll { # Setup for context }
        It 'Should behavior' { # Individual test }
        AfterAll { # Cleanup for context }
    }
}
```
##核心关键词

- **`Describe`**：顶级分组，通常以被测试的功能命名
—**`Context`**：在描述中进行子分组，用于特定场景
- **`It`**：单独的测试用例，使用描述性名称
—**`Should`**：用于测试验证的断言关键字
**`BeforeAll/AfterAll`**：每块Setup/teardown一次
**`BeforeEach/AfterEach`**:Setup/teardownbefore/after各测试一次

## Setup和Teardown

- **`BeforeAll`**：在包含块开始时运行一次，用于昂贵的操作
**`BeforeEach`**：在每个`It`块之前运行，用于特定于测试的设置
**`AfterEach`**：在每个`It`之后运行，即使测试失败也保证运行
- **`AfterAll`**：在块结束时运行一次，用于清理
- **变量作用域**:`BeforeAll`变量可用于子块（只读），`BeforeEach/It/AfterEach`具有相同的作用域

断言（应该）**基本比较**:`-Be`，`-BeExactly`,`-Not -Be`- **系列**:`-Contain`、`-BeIn`、`-HaveCount`—**数字**:`-BeGreaterThan`、`-BeLessThan`、`-BeGreaterOrEqual`- **字符串**:`-Match`，`-Like`,`-BeNullOrEmpty`- **类型**:`-BeOfType`、`-BeTrue`、`-BeFalse`- **文件**:`-Exist`，`-FileContentMatch`—**例外**:`-Throw`、`-Not -Throw`# #嘲笑

—**`Mock CommandName { ScriptBlock }`**：替换命令行为
—**`-ParameterFilter`**：只在参数匹配条件时进行模拟
- **`-Verifiable`**：将mock标记为需要验证
- **`Should -Invoke`**：验证mock被调用的特定次数
- **`Should -InvokeVerifiable`**：验证所有可验证的模拟被调用
—**作用域**：模拟默认包含块作用域```powershell
Mock Get-Service { @{ Status = 'Running' } } -ParameterFilter { $Name -eq 'TestService' }
Should -Invoke Get-Service -Exactly 1 -ParameterFilter { $Name -eq 'TestService' }
```
测试用例（数据驱动测试）

使用`-TestCases`或`-ForEach`进行参数化测试：```powershell
It 'Should return <Expected> for <Input>' -TestCases @(
    @{ Input = 'value1'; Expected = 'result1' }
    @{ Input = 'value2'; Expected = 'result2' }
) {
    Get-Function $Input | Should -Be $Expected
}
```
数据驱动测试

—**`-ForEach`**：可在`Describe`、`Context`和`It`上使用，用于从数据生成多个测试
**`-TestCases`**:`-ForEach`在`It`块上的别名（向后兼容）
- **哈希表数据**：每个项定义测试中可用的变量（例如，`@{ Name = 'value'; Expected = 'result' }`）
- **数组数据**：使用`$_`变量为当前项
—**模板**：在测试名中使用`<variablename>`进行动态扩展```powershell
# Hashtable approach
It 'Returns <Expected> for <Name>' -ForEach @(
    @{ Name = 'test1'; Expected = 'result1' }
    @{ Name = 'test2'; Expected = 'result2' }
) { Get-Function $Name | Should -Be $Expected }

# Array approach
It 'Contains <_>' -ForEach 'item1', 'item2' { Get-Collection | Should -Contain $_ }
```
# #标签

—**适用于**:`Describe`、`Context`和`It`块
—**过滤**：使用`-TagFilter`和`-ExcludeTagFilter`与`Invoke-Pester`—**通配符**：标签支持`-like`通配符，灵活过滤```powershell
Describe 'Function' -Tag 'Unit' {
    It 'Should work' -Tag 'Fast', 'Stable' { }
    It 'Should be slow' -Tag 'Slow', 'Integration' { }
}

# Run only fast unit tests
Invoke-Pester -TagFilter 'Unit' -ExcludeTagFilter 'Slow'
```
# #跳过

—**`-Skip`**：可在`Describe`、`Context`和`It`上跳过测试
—**条件必选**：使用`-Skip:$condition`进行动态跳转
- **运行跳过**：在测试执行期间使用`Set-ItResult -Skipped`（setup/teardown仍然运行）
**结束测试体**:`Set-ItResult -Skipped`/`-Inconclusive`抛出内部结束`It`块，所以代码后不运行；末尾的`return`是不可访问的，不应该添加```powershell
It 'Should work on Windows' -Skip:(-not $IsWindows) { }
Context 'Integration tests' -Skip { }
```
##错误处理

—**Continue on Failure**：使用`Should.ErrorAction = 'Continue'`收集多个失败
- **临界时停止**：使用`-ErrorAction Stop`作为先决条件
—**测试异常**：使用`{ Code } | Should -Throw`进行异常测试

最佳实践

—**描述性名称**：使用清晰的测试描述来解释行为
- **AAA模式**：安排（设置），行动（执行），断言（验证）
- **独立测试**：每个测试应该是独立的
**避免使用别名**：使用完整的cmlet名称（`Where-Object`，而不是`?`）
- **单一职责**：尽可能每个测试一个断言
—**测试文件组织**：将相关测试分组到上下文块中。上下文块可以嵌套。

示例测试模式```powershell
BeforeAll {
    . $PSScriptRoot/Get-UserInfo.ps1
}

Describe 'Get-UserInfo' {
    Context 'When user exists' {
        BeforeAll {
            Mock Get-ADUser { @{ Name = 'TestUser'; Enabled = $true } }
        }

        It 'Should return user object' {
            $result = Get-UserInfo -Username 'TestUser'
            $result | Should -Not -BeNullOrEmpty
            $result.Name | Should -Be 'TestUser'
        }

        It 'Should call Get-ADUser once' {
            Get-UserInfo -Username 'TestUser'
            Should -Invoke Get-ADUser -Exactly 1
        }
    }

    Context 'When user does not exist' {
        BeforeAll {
            Mock Get-ADUser { throw "User not found" }
        }

        It 'Should throw exception' {
            { Get-UserInfo -Username 'NonExistent' } | Should -Throw "*not found*"
        }
    }
}
```
# #配置

当调用`Invoke-Pester`来控制执行行为时，配置是在**测试文件之外定义的。```powershell
# Create configuration (Pester 5.2+)
$config = New-PesterConfiguration
$config.Run.Path = './Tests'
$config.Output.Verbosity = 'Detailed'
$config.TestResult.Enabled = $true
$config.TestResult.OutputFormat = 'NUnitXml'
$config.Should.ErrorAction = 'Continue'
Invoke-Pester -Configuration $config
```
**关键部分**：运行（路径，退出），过滤器（标签，ExcludeTag），输出（冗长），测试结果（启用，输出格式），CodeCoverage（启用，路径），应该（ErrorAction），调试