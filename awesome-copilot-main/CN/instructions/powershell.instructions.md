---
applyTo: '**/*.ps1,**/*.psm1'
description: 'PowerShell cmdlet and scripting best practices based on Microsoft guidelines'
---
# PowerShell Cmdlet开发指南

本指南提供了特定于powershell的说明，以帮助GitHub Copilot生成习惯用法的、
安全且可维护的脚本。它与微软的PowerShell cmd开发指南保持一致。

命名约定

- **动名词形式：**
-使用授权的PowerShell动词（Get-Verb）
-使用单数名词
动词和名词都用PascalCase
—避免使用特殊字符和空格

—**参数名称：**
—使用PascalCase
—选择清晰、描述性的名称
-使用单数形式，除非总是复数形式
-遵循PowerShell标准名称

—**变量名称：**
—公共变量使用PascalCase
—私有变量使用camelCase
避免使用缩写
-使用有意义的名字—**别名避免：**
—使用完整的cmdlet名称
避免在脚本中使用别名（例如，使用`Get-ChildItem`而不是`gci`）
-记录任何自定义别名
—使用完整的参数名

###示例-命名约定```powershell
function Get-UserProfile {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory)]
        [string]$Username,

        [Parameter()]
        [ValidateSet('Basic', 'Detailed')]
        [string]$ProfileType = 'Basic'
    )

    process {
        $outputString = "Searching for: '$($Username)'"
        Write-Verbose -Message $outputString
        Write-Verbose -Message "Profile type: $ProfileType"
        # Logic here
    }
}
```
参数设计

- **标准参数：**
—使用通用的参数名称（`Path`,`Name`,`Force`）
-遵循内置的cmdlet约定
-对专门术语使用别名
-文件参数用途

—**参数名称：**
-使用单数形式，除非总是复数形式
—选择清晰、描述性的名称
—遵循PowerShell约定
—使用PascalCase格式

- **类型选择：**
—使用普通。网络类型
-实施适当的验证
-考虑ValidateSet有限的选项
-在可能的情况下启用选项卡补全

—**开关参数：**
- **ALWAYS**使用`[switch]`作为布尔标志，而不是`[bool]`- **永远不要**使用`[bool]$Parameter`或赋默认值
—省略时，将参数默认为`$false`-使用清晰的、面向行动的名字
-测试存在与`.IsPresent`-在参数属性中使用`$true`/`$false`（例如，`Mandatory = $true`）是可以接受的示例-参数设计```powershell
function Set-ResourceConfiguration {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory)]
        [string]$Name,

        [Parameter()]
        [ValidateSet('Dev', 'Test', 'Prod')]
        [string]$Environment = 'Dev',

        # ✔️ CORRECT: Use `[switch]` with no default value
        [Parameter()]
        [switch]$Force,

         # ❌ WRONG: Shows incorrect default assignment, however this is correct syntax (requires `[switch]` cast).
        [Parameter()]
        [switch]$Quiet = [switch]$true,

        [Parameter()]
        [ValidateNotNullOrEmpty()]
        [string[]]$Tags
    )

    process {
        # Use .IsPresent to check switch state
        if ($Quiet.IsPresent) {
            Write-Verbose "Quiet mode enabled"
        }
    }
}
```
##管道和输出

—**管路输入：**
—使用`ValueFromPipeline`作为直接对象输入
—使用`ValueFromPipelineByPropertyName`进行属性映射
-实现Begin/Process/End块管道处理
-文档管道输入要求

—**输出对象：**
-返回富对象，而不是格式化文本
-对结构化数据使用PSCustomObject
—避免使用Write-Host方式输出数据
—开启下游cmlet处理

—**管道流：**
—一次输出一个对象
—使用进程块进行流处理
—避免收集大型阵列
-启用即时处理

—**PassThru模式：**
-操作cmdlet默认不输出
—实现`-PassThru`切换对象返回
-用`-PassThru`返回modified/created对象
—使用verbose/warning进行状态更新

示例-管道和输出```powershell
function Update-ResourceStatus {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory, ValueFromPipeline, ValueFromPipelineByPropertyName)]
        [string]$Name,

        [Parameter(Mandatory)]
        [ValidateSet('Active', 'Inactive', 'Maintenance')]
        [string]$Status,

        [Parameter()]
        [switch]$PassThru
    )

    begin {
        Write-Verbose 'Starting resource status update process'
        $timestamp = Get-Date
    }

    process {
        # Process each resource individually
        Write-Verbose "Processing resource: $Name"

        $resource = [PSCustomObject]@{
            Name        = $Name
            Status      = $Status
            LastUpdated = $timestamp
            UpdatedBy   = "$($env:USERNAME)"
        }

        # Only output if PassThru is specified
        if ($PassThru.IsPresent) {
            Write-Output $resource
        }
    }

    end {
        Write-Verbose 'Resource status update process completed'
    }
}
```
错误处理和安全

- **ShouldProcess实现：**
—使用`[CmdletBinding(SupportsShouldProcess = $true)]`—设置合适的`ConfirmImpact`级别
-调用`$PSCmdlet.ShouldProcess()`作为关闭更改操作
—使用`$PSCmdlet.ShouldContinue()`进行其他确认

—**消息流：**
-`Write-Verbose`操作细节与`-Verbose`-`Write-Warning`用于警告条件
-`Write-Error`表示非终止错误
-`throw`用于终止错误
—除用户界面文本外，避免使用`Write-Host`- **错误处理模式：**
—使用try/catch块进行错误管理
-设置适当的ErrorAction首选项
—返回有意义的错误消息
-需要时使用ErrorVariable
-包括正确的终止和非终止错误处理
-在`[CmdletBinding()]`的高级功能中，优先使用`$PSCmdlet.WriteError()`而不是`Write-Error`-在`[CmdletBinding()]`的高级功能中，优先使用`$PSCmdlet.ThrowTerminatingError()`而不是`throw`-构造带有类别、目标和异常细节的ErrorRecord对象

- **非交互设计：**
-接受参数输入
—避免在脚本中使用`Read-Host`—支持自动化场景
-记录所有需要的输入

###示例-错误处理和安全```powershell
function Remove-CacheFiles {
    [CmdletBinding(SupportsShouldProcess, ConfirmImpact = 'High')]
    param(
        [Parameter(Mandatory)]
        [string]$Path
    )

    try {
        $files = Get-ChildItem -Path $Path -Filter "*.cache" -ErrorAction Stop
        
        # Demonstrates WhatIf support
        if ($PSCmdlet.ShouldProcess($Path, 'Remove cache files')) {
            $files | Remove-Item -Force -ErrorAction Stop
            Write-Verbose "Removed $($files.Count) cache files from $Path"
        }
    } catch {
        $errorRecord = [System.Management.Automation.ErrorRecord]::new(
            $_.Exception,
            'RemovalFailed',
            [System.Management.Automation.ErrorCategory]::NotSpecified,
            $Path
        )
        $PSCmdlet.WriteError($errorRecord)
    }
}
```
文档和样式

- **基于注释的帮助：**包括任何面向公众的函数或cmdlet的基于注释的帮助。在函数内部，添加一个`<# ... #>`帮助注释，至少为：
-`.SYNOPSIS`简要描述
-`.DESCRIPTION`详细说明
-`.EXAMPLE`节与实际使用
-`.PARAMETER`描述
-`.OUTPUTS`返回的输出类型
-`.NOTES`附加信息

—**格式一致：**
—遵循一致的PowerShell样式
-使用适当的缩进（建议4个空格）
-与语句在同一行的大括号
-新行上的结束大括号
-在管道操作符之后使用换行符
—PascalCase：函数名和参数名
—避免不必要的空白- **管道支持：**
-实现流水线函数的Begin/Process/End块
-在适当的地方使用ValueFromPipeline
-支持通过属性名进行管道输入
-返回正确的对象，而不是格式化的文本

- **避免使用别名：**使用完整的cmdlet名称和参数
-避免在脚本中使用别名（例如，使用Get-ChildItem代替gci）；别名可用于交互式shell。
—使用`Where-Object`，而不是`?`或`where`—使用`ForEach-Object`，而不是`%`-使用`Get-ChildItem`，而不是`ls`或`dir`---

完整示例：端到端Cmdlet模式```powershell
function Remove-UserAccount {
    [CmdletBinding(SupportsShouldProcess = $true, ConfirmImpact = 'High')]
    param(
        [Parameter(Mandatory, ValueFromPipeline)]
        [ValidateNotNullOrEmpty()]
        [string]$Username,

        [Parameter()]
        [switch]$Force
    )

    begin {
        Write-Verbose 'Starting user account removal process'
        $currentErrorActionValue = $ErrorActionPreference
        $ErrorActionPreference = 'Stop'
    }

    process {
        try {
            # Validation
            if (-not (Test-UserExists -Username $Username)) {
                $errorRecord = [System.Management.Automation.ErrorRecord]::new(
                    [System.Exception]::new("User account '$Username' not found"),
                    'UserNotFound',
                    [System.Management.Automation.ErrorCategory]::ObjectNotFound,
                    $Username
                )
                $PSCmdlet.WriteError($errorRecord)
                return
            }

            # ShouldProcess enables -WhatIf and -Confirm support
            if ($PSCmdlet.ShouldProcess($Username, "Remove user account")) {
                # ShouldContinue provides an additional confirmation prompt for high-impact operations
                # This prompt is bypassed when -Force is specified
                if ($Force -or $PSCmdlet.ShouldContinue("Are you sure you want to remove '$Username'?", "Confirm Removal")) {
                    Write-Verbose "Removing user account: $Username"
                    
                    # Main operation
                    Remove-ADUser -Identity $Username -ErrorAction Stop
                    Write-Warning "User account '$Username' has been removed"
                }
            }
        } catch [Microsoft.ActiveDirectory.Management.ADException] {
            $errorRecord = [System.Management.Automation.ErrorRecord]::new(
                $_.Exception,
                'ActiveDirectoryError',
                [System.Management.Automation.ErrorCategory]::NotSpecified,
                $Username
            )
            $PSCmdlet.ThrowTerminatingError($errorRecord)
        } catch {
            $errorRecord = [System.Management.Automation.ErrorRecord]::new(
                $_.Exception,
                'UnexpectedError',
                [System.Management.Automation.ErrorCategory]::NotSpecified,
                $Username
            )
            $PSCmdlet.ThrowTerminatingError($errorRecord)
        }
    }

    end {
        Write-Verbose 'User account removal process completed'
        # Set ErrorActionPreference back to the value it had
        $ErrorActionPreference = $currentErrorActionValue
    }
}
```
