---
name: batch-files
description: 'Expert-level Windows batch file (.bat/.cmd) skill for writing, debugging, and maintaining CMD scripts. Use when asked to "create a batch file", "write a .bat script", "automate a Windows task", "CMD scripting", "batch automation", "scheduled task script", "Windows shell script", or when working with .bat/.cmd files in the workspace. Covers cmd.exe syntax, environment variables, control flow, string processing, error handling, and integration with system tools.'
---
批处理文件

使用cmd.exe创建、编辑、调试和维护Windows批处理文件（.bat/.cmd）的综合技能。适用于CLI工具开发、系统管理自动化、定时任务、文件操作脚本、基于path的可执行脚本等。

何时使用此技能

—创建或编辑`.bat`或`.cmd`文件
-自动执行Windows任务（文件操作、部署、备份）
-为PATH上的`bin/`文件夹构建CLI工具
-编写计划任务脚本（SCHTASKS, task Scheduler）
-调试批处理脚本问题（变量扩展，错误级别，引用）
-集成批处理脚本与外部工具（curl, git,Node.js, Python）
-脚手架新的基于批的项目与结构化模板

# #先决条件—基于Windows nt的操作系统（Windows 7或更高版本）
- cmd.exe（内置）
—可选：在PATH下设置一个`bin/`目录，用于将脚本作为命令分发
-可选：pathxt配置为包含`.BAT;.CMD`（Windows默认）

##命令解释

Cmd.exe按顺序通过四个阶段处理每一行：1. **变量替换** -`%VAR%`标记被替换为环境变量值。`%0`-`%9`引用批处理参数。`%*`展开为所有参数。
2. **引号和转义** -插入`^`转义特殊字符（`& | < > ^`）。引号防止解释所包含的特殊字符。在批处理文件中，`%%`产生一个文字`%`。
3. **语法解析** -行被分成管道（`|`）、复合命令（`&`、`&&`、`||`）和括号组`( )`。
4. **重定向** -`>`覆盖，`>>`追加，`<`读取输入，`2>`重定向stderr，`2>&1`将stderr合并为stdout，`>NUL`丢弃输出。

# #变量

环境变量```bat
set _MY_VAR=Hello World
echo %_MY_VAR%
set _MY_VAR=
```
-不带参数的`set`列出所有变量
-`set _PREFIX`列出以`_PREFIX`开头的变量
-`=`周围没有空格-`set name = val`将变量`"name "`设置为`" val"`特殊变量

|变量|取值||----------|-------|
|`%CD%`|当前目录|
|`%DATE%`|系统日期（依赖于地区）|
|`%TIME%`|系统时间HH:MM:SS.mm |
|`%RANDOM%`|伪随机数0-32767 |
|`%ERRORLEVEL%`|最后一条命令的退出代码|
|`%USERNAME%`|当前用户名|
|`%USERPROFILE%`|当前用户配置文件路径|
|`%TEMP%`/`%TMP%`|临时文件目录|
|可执行扩展列表|
|`%COMSPEC%`| cmd.exe路径|

SETLOCAL / ENDLOCAL的作用域```bat
setlocal
set _LOCAL_VAR=scoped value
endlocal
REM _LOCAL_VAR is no longer defined here
```
从作用域块中返回一个值：```bat
endlocal & set _RESULT=%_LOCAL_VAR%
```
延迟扩展

括号内的变量在解析时展开。使用延迟展开进行运行时评估：```bat
setlocal EnableDelayedExpansion
set _COUNT=0
for /l %%i in (1,1,5) do (
    set /a _COUNT+=1
    echo !_COUNT!
)
endlocal
```
-`!VAR!`在执行时展开（延迟）
-`%VAR%`在解析时展开（立即）

##控制流

###条件执行```bat
if exist "output.txt" echo File found
if not defined _MY_VAR echo Variable not set
if "%_STATUS%"=="ready" (echo Go) else (echo Wait)
if %ERRORLEVEL% neq 0 echo Command failed
```
比较运算符：`equ`、`neq`、`lss`、`leq`、`gtr`、`geq`。使用`/i`进行不区分大小写的字符串比较。

复合命令```bat
command1 & command2        & REM Always run both
command1 && command2       & REM Run command2 only if command1 succeeds
command1 || command2       & REM Run command2 only if command1 fails
```
### FOR循环```bat
REM Iterate over a set of values
for %%i in (alpha beta gamma) do echo %%i

REM Numeric range: start, step, end
for /l %%i in (1,1,10) do echo %%i

REM Files in a directory
for %%f in (*.txt) do echo %%f

REM Recursive file search
for /r %%f in (*.log) do echo %%f

REM Directories only
for /d %%d in (*) do echo %%d

REM Parse command output
for /f "tokens=1,2 delims=:" %%a in ('ipconfig ^| findstr "IPv4"') do echo %%b

REM Parse file lines
for /f "usebackq tokens=*" %%a in ("data.txt") do echo %%a
```
### GOTO和标签```bat
goto :main_logic
:usage
echo Usage: %~nx0 [options]
exit /b 1

:main_logic
echo Running main logic...
goto :eof
```
`goto :eof`退出当前批处理或子例程。标签以`:`开头。

##命令行参数

|语法|取值||--------|-------|
|`%0`|被调用的脚本名称|
|`%1`-`%9`|位置参数|
|`%*`|所有参数（不受SHIFT影响）|
|`%~1`|带引号的参数1被删除|
|`%~f1`|参数1|的完整路径
|`%~d1`|参数1的驱动器号|
|`%~p1`|参数1 |的路径（不含驱动器）
|`%~n1`|参数1的文件名（无扩展名）|
|`%~x1`|参数1的扩展|
|`%~dp0`|批处理文件本身的驱动器和路径|
|`%~nx0`|批处理文件|的扩展名
|`%~z1`|参数1的文件大小|
|`%~$PATH:1`|搜索参数1 |的PATH

参数解析模式```bat
:parse_args
if "%~1"=="" goto :args_done
if /i "%~1"=="--help" goto :usage
if /i "%~1"=="--output" (
    set "_OUTPUT_DIR=%~2"
    shift
)
shift
goto :parse_args
:args_done
```
##字符串处理

# # #子字符串```bat
set _STR=Hello World
echo %_STR:~0,5%       & REM "Hello"
echo %_STR:~6%         & REM "World"
echo %_STR:~-5%        & REM "World"
echo %_STR:~0,-6%      & REM "Hello"
```
###搜索和替换```bat
set _STR=Hello World
echo %_STR:World=Earth%       & REM "Hello Earth"
echo %_STR:Hello=%            & REM " World" (remove "Hello")
```
子字符串包含测试```bat
if not "%_STR:World=%"=="%_STR%" echo Contains "World"
```
# #功能

函数使用标签、CALL和SETLOCAL/ENDLOCAL：```bat
@echo off
call :greet "Jane Doe"
echo Result: %_GREETING%
exit /b 0

:greet
setlocal
set "_MSG=Hello, %~1"
endlocal & set "_GREETING=%_MSG%"
exit /b 0
```
—`call :label args`调用函数`exit /b`从函数返回（不是脚本）
-使用`endlocal & set`技巧将值传递出作用域块

# #算术`set /a`执行32位有符号整数运算：```bat
set /a _RESULT=10 * 5 + 3
set /a _COUNTER+=1
set /a _REMAINDER=14 %% 3       & REM Use %% for modulo in batch files
set /a _BITS="255 & 0x0F"       & REM Bitwise AND
```
支持的操作符：`+ - * / %% ( )`和位`& | ^ ~ << >>`。

支持十六进制（`0xFF`）和八进制（`077`）文本。

##错误处理

错误级别约定

-`0`= success
-非零=失败（通常为`1`）```bat
mycommand.exe
if %ERRORLEVEL% neq 0 (
    echo ERROR: mycommand failed with code %ERRORLEVEL%
    exit /b %ERRORLEVEL%
)
```
快速失败模式```bat
command1 || (echo command1 failed & exit /b 1)
command2 || (echo command2 failed & exit /b 1)
```
###设置退出码```bat
exit /b 0        & REM Return success from a batch/function
exit /b 1        & REM Return failure
cmd /c "exit /b 42"   & REM Set ERRORLEVEL to 42 inline
```
基本命令参考

###文件操作

|命令|用途||---------|---------|
|`DIR`|列出目录内容|
|`COPY`|复制|文件
|`XCOPY`|带子目录的扩展副本（遗留）|
|`ROBOCOPY`|带重试、镜像、日志的健壮副本|
|`MOVE`|移动或重命名|文件
|`DEL`|删除|文件
|`REN`|重命名文件|
|`MD`/`MKDIR`|创建目录|
|`RD`/`RMDIR`|删除目录|
|`MKLINK`|创建符号链接或硬链接|
|`ATTRIB`|查看或设置文件属性|
|`TYPE`|打印文件内容|
|`MORE`|分页文件显示|
|`TREE`|显示目录结构|
|`REPLACE`|将目标文件替换为源|
|`COMPACT`|显示或设置NTFS压缩|
|`EXPAND`|从。cab文件中提取|
|`MAKECAB`|创建。cab档案|
|`TAR`|创建或提取tar存档|

文本搜索和处理

|命令|用途||---------|---------|
|`FIND`|搜索字面值字符串|
|`FINDSTR`|有限正则表达式搜索|
|按字母顺序排序|
|`CLIP`|复制管道输入到剪贴板|
|`FC`|比较两个文件|
|`COMP`|二进制文件比较|
|`CERTUTIL`|Encode/decodeBase64，计算哈希值|

###系统信息

|命令|用途||---------|---------|
|`SYSTEMINFO`|系统全配置|
|`HOSTNAME`|显示计算机名|
|`VER`| Windows版本|
|`WHOAMI`|当前用户和组信息|
|`TASKLIST`|运行进程列表|
|`TASKKILL`|终止进程|
|`WMIC`| WMI查询（驱动器、操作系统、内存）|
|`SC`|业务控制（查询、启动、停止）|
|`DRIVERQUERY`|列出已安装的驱动|
|`REG`|注册表操作（查询、添加、删除）|
|`SETX`|设置持久环境变量|

# # #网络

|命令|用途||---------|---------|
|`PING`|测试网络连通性|
|`IPCONFIG`| IP配置|
|`NSLOOKUP`| DNS查找|
|`NETSTAT`|网络连接和端口|
|`TRACERT`|到主机|的Trace路由
|`NET USE`|Map/disconnect网络驱动器|
|`NET USER`|管理用户|
|`NETSH`|网络配置实用程序|
|`ARP`| ARP缓存管理
|`ROUTE`|路由表管理|
|`CURL`| HTTP请求(Windows 10+
|`SSH`|安全shell (Windows 10+) |

###调度和自动化

|命令|用途||---------|---------|
|`SCHTASKS`|创建和管理定时任务|
|`TIMEOUT`|等待N秒（Vista+） |
|`START`|异步启动程序|
|`RUNAS`|以不同用户|运行
|`SHUTDOWN`|关闭或重启|
|`FORFILES`|按日期查找文件并执行命令|

Shell实用程序

|命令|用途||---------|---------|
|`WHERE`|在PATH |中找到可执行文件
|`DOSKEY`|创建命令宏|
|`CHOICE`|单键输入提示|
|`MODE`|配置控制台大小和端口|
|`SUBST`|映射文件夹到驱动器号|
|`CHCP`|获取或设置控制台代码页|
|`COLOR`|设置控制台颜色|
|`TITLE`|设置控制台窗口标题|
|`ASSOC`/`FTYPE`|文件类型关联|

Shell语法和表达式

###括号用于分组

括号将复合命令转换为单个单元，用于重定向或有条件执行：```bat
(echo Line 1 & echo Line 2) > output.txt
if exist "data.csv" (
    echo Processing...
    call :process "data.csv"
) else (
    echo No data found.
)
```
转义字符

插入符号`^`转义下一个字符：```bat
echo Total ^& Summary          & REM Outputs: Total & Summary
echo 100%% complete            & REM Outputs: 100% complete (in batch)
echo Line one^
Line two                       & REM Caret escapes the newline
```
在管道之后，需要使用三重插入符号：`echo x ^^^& y | findstr x`# # #通配符

-`*`匹配任何字符序列
-`?`匹配单个字符（或在无句点段结束时为零）```bat
dir *.txt           & REM All .txt files
ren *.jpeg *.jpg    & REM Bulk rename
```
###重定向汇总```bat
command > file.txt          & REM Overwrite stdout to file
command >> file.txt         & REM Append stdout to file
command 2> errors.log       & REM Redirect stderr
command > all.log 2>&1      & REM Merge stderr into stdout
command < input.txt         & REM Read stdin from file
command > NUL 2>&1          & REM Discard all output
```
编写生产质量的批处理文件

标准脚本结构```bat
@echo off
setlocal EnableDelayedExpansion

REM ============================================================
REM  Script: example.bat
REM  Purpose: Describe what this script does
REM ============================================================

call :main %*
exit /b %ERRORLEVEL%

:main
    call :parse_args %*
    if not defined _TARGET (
        echo ERROR: --target is required. 1>&2
        call :usage
        exit /b 1
    )
    echo Processing: %_TARGET%
    exit /b 0

:parse_args
    if "%~1"=="" exit /b 0
    if /i "%~1"=="--target" set "_TARGET=%~2" & shift
    if /i "%~1"=="--help"   call :usage & exit /b 0
    shift
    goto :parse_args

:usage
    echo Usage: %~nx0 --target ^<path^> [--help]
    echo.
    echo Options:
    echo   --target   Path to process (required)
    echo   --help     Show this help message
    exit /b 0
```
最佳实践1. **始终以`@echo off`和`setlocal`开始** -防止噪声输出和变量泄漏到调用者。
2. **在处理之前验证输入** -尽早检查所需的参数和文件是否存在。使用`if not defined`和`if not exist`。
3. **引用路径和变量** -使用`"%~1"`和`"%_MY_PATH%"`来安全地处理空格和特殊字符。
4. **使用`exit /b`而不是`exit`** -避免关闭父控制台窗口。
5. **返回有意义的退出代码** -`exit /b 0`表示成功，非零表示特定失败。
6. **使用`%~dp0`作为脚本相对路径** -确保脚本不管调用者的工作目录如何工作。
7. **首选`ROBOCOPY`而不是`XCOPY`** -更可靠，支持重试，镜像和日志记录。
8. **使用`EnableDelayedExpansion`修改循环或括号块内的变量
9. **将错误写入stderr** -`echo ERROR: message 1>&2`保持stdout清洁管道。
10. **使用`REM`进行评论Ts ** -`::`可能导致`FOR`循环体内部出现问题。安全考虑

- **不要在批处理文件中存储凭据** -使用环境变量，凭据存储或提示。
—**验证用户输入**—包含“`&`”、“`|`”、“`>`”的未加引号的变量可以注入命令。总是引用：`"%_USER_INPUT%"`。
- **使用`SETLOCAL`** -防止变量值泄漏到父进程。
** -在传递给`DEL`，`RD`或`ROBOCOPY`之前验证路径，以防止意外删除。
- **避免`SET /P`敏感输入** -输入是可见的，并存储在控制台历史记录。尽可能使用专用的凭证工具。

调试和故障排除

|技巧|如何||-----------|-----|
|删除`@echo off`或暂时使用`@echo on`在|段之间添加`PAUSE`|检查错误级别|`echo Exit code: %ERRORLEVEL%`|
|检查变量|`set _MY_`，列出所有以`_MY_`|开头的变量
|`( )`块内的变量没有更新？启用`!VAR!`语法|
在批处理文件中使用`%%i`，在命令行中使用`%i`|
| SET |`set name=value`中的空格，而不是`set name = value`|
|管道插入|管道后，使用`^^^`转义特殊字符|
在`if`块内使用`^(`和`^)`转义，或使用引号|
在批处理文件|中，对|`set /a r=14 %% 3`取模的双%

跨平台和扩展工具

当批处理脚本达到其极限时，这些工具扩展cmd.exe功能：

|工具|用途||------|---------|
| **Cygwin** | Windows上的全POSIX环境(grep, sed, awk, ssh
| **MSYS2** |轻量级Unix工具和包管理器（pacman） |
| **WSL** | Linux的Windows子系统-运行本地Linux二进制文件|
| **GnuWin32** |单个GNU实用程序作为本机Windows可执行程序|
| **PowerShell** |现代Windows脚本。. NET集成|

当您需要：快速启动、简单的文件操作、基于path的CLI工具或Task Scheduler集成时，可以使用批处理。考虑将PowerShell或wsdl用于复杂的数据处理、REST api或面向对象脚本。

## CMD快捷键

|快捷方式|动作||----------|--------|
|`Tab`|自动完成file/folder名称|
|`Up`/`Down`|导航命令历史|
|`F7`|显示命令历史弹出|
|`F3`|重复上次命令|
|`Esc`|清除电流线|
|`Ctrl+C`|取消执行|命令
|`Alt+F7`|清除命令历史|

##参考文件`references/`文件夹包含详细的文档：

|文件|内容||------|----------|
|`tools-and-resources.md`| Windows工具，实用程序，包管理器，终端|
|`batch-files-and-functions.md`|示例脚本、技术、最佳实践链接|
|`windows-commands.md`|综合A-Z Windows命令参考|
|`cygwin.md`| Cygwin用户指南和FAQ |
|`msys2.md`| MSYS2安装、包和环境|
| WSL设置、命令和文档|

资产模板`assets/`文件夹包含starter批处理文件模板数据，但作为文本文件：

|模板|用途||----------|---------|
|`executable.txt`|独立的CLI工具，支持参数解析|
|`library.txt`|具有可调用标签的可重用函数库
|`task.txt`|定时任务/自动化脚本|