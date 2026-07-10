# Runnable Recipe Examples

这个文件夹包含每个食谱的独立的、可执行的c#示例。这些是[基于文件的应用程序](https://learn.microsoft.com/dotnet/core/sdk/file-based-apps)，可以直接使用`dotnet run`运行。

# #先决条件

-。NET 10.0或更高版本
-GitHub CopilotSDK包（自动引用）

##运行示例

每个`.cs`文件都是一个完整的、可运行的程序。简单的使用方法:```bash
dotnet run <filename>.cs
```
可用的食谱

|配方|命令|描述|| -------------------- | ------------------------------------ | ------------------------------------------ |
|错误处理|`dotnet run error-handling.cs`|演示错误处理模式|
|多会话|`dotnet run multiple-sessions.cs`|管理多个独立会话|
|本地文件管理⚠️|`dotnet run managing-local-files.cs`|使用AI分组|对文件进行组织
| PR可视化ℹ️|`dotnet run pr-visualization.cs`|生成PR年龄图表|
|持久化会话|`dotnet run persisting-sessions.cs`|跨重启保存和恢复会话|
|可访问性报告ℹ️|`dotnet run accessibility-report.cs`|分析网页可访问性|
|拉尔夫循环⚠️|`dotnet run ralph-loop.cs`|自主开发循环|

带参数的例子

**PR可视化与特定的回购：**```bash
dotnet run pr-visualization.cs -- --repo github/copilot-sdk
```
**管理本地文件（编辑文件以更改目标文件夹）：**```bash
# Edit the targetFolder variable in managing-local-files.cs first
dotnet run managing-local-files.cs
```
##安全和先决条件

有些配方有副作用或外部依赖。展开安全测试模式和先决条件的每个部分。<details>
<summary><strong>⚠️ Managing Local Files</strong> — Modifies your filesystem</summary>
在真正的目录上运行之前，先在一个副本上进行测试。
从该配方目录运行这些代码片段，以便在切换到临时文件夹之前捕获配方路径。

* * PowerShell: * *```powershell
$recipeDir = (Get-Location).Path
$tempDir = New-Item -ItemType Directory -Path ([IO.Path]::Combine([IO.Path]::GetTempPath(), "copilot-test-files"))
@("document1.txt", "image1.png", "data.json") | ForEach-Object { 
    New-Item -Path "$tempDir/$_" -ItemType File
}
cd $tempDir
dotnet run "$recipeDir/managing-local-files.cs"
# Inspect results, then clean up
Remove-Item $tempDir -Recurse
```
Bash: * * * *```bash
recipeDir=$(pwd)
tempDir=$(mktemp -d)
touch "$tempDir"/{document1.txt,image1.png,data.json}
cd "$tempDir"
dotnet run "$recipeDir/managing-local-files.cs"
# Inspect results, then clean up
rm -rf "$tempDir"
```
在运行之前，编辑`.cs`文件中的`targetFolder`变量，以指向测试目录。</details>

<details>
<summary><strong>⚠️ Ralph Loop</strong> — Creates git commits and modifies files</summary>
总是首先在一个独立的git存储库中运行它来验证它的行为。
从该配方目录运行这些代码片段，以便在切换到临时存储库之前捕获配方路径。

* * PowerShell: * *```powershell
$recipeDir = (Get-Location).Path
$tempDir = New-Item -ItemType Directory -Path ([IO.Path]::Combine([IO.Path]::GetTempPath(), "copilot-test-repo"))
cd $tempDir
git init
git config user.email "test@example.com"
git config user.name "Test User"

# Create a PROMPT_task.md for the recipe to work with
"# Task`nCreate a simple README" | Out-File PROMPT_task.md
dotnet run "$recipeDir/ralph-loop.cs"

# Review commits and changes
git log --oneline
git diff

# Clean up
cd ..
Remove-Item $tempDir -Recurse
```
Bash: * * * *```bash
recipeDir=$(pwd)
tempDir=$(mktemp -d)
cd "$tempDir"
git init
git config user.email "test@example.com"
git config user.name "Test User"

# Create a PROMPT_task.md for the recipe to work with
echo -e "# Task\nCreate a simple README" > PROMPT_task.md
dotnet run "$recipeDir/ralph-loop.cs"

# Review commits and changes
git log --oneline
git diff

# Clean up
cd ..
rm -rf "$tempDir"
```
该配方需要一个至少包含一个`PROMPT_*.md`文件的git存储库，并将在无限循环中运行，直到手动停止。</details>

<details>
<summary><strong>ℹ️ Accessibility Report</strong> — Requires Playwright MCP</summary>
这个配方需要剧作家MCP安装和可用：```bash
npm install -g @playwright/mcp
```
或者让Node Package Manager按需安装。该配方将尝试自动启动`npx @playwright/mcp`。正常运行食谱：```bash
dotnet run accessibility-report.cs
```
该配方将提示您输入URL以分析并生成可访问性报告。</details>

<details>
<summary><strong>ℹ️ PR Visualization</strong> — Requires GitHub API access</summary>
这个食谱需要：

-访问GitHub存储库（公共或私有，具有适当的凭据）
—`gh`命令行工具已安装并认证：https://cli.github.com/使用repository参数运行：```bash
dotnet run pr-visualization.cs -- --repo owner/repo-name
```
例子:```bash
dotnet run pr-visualization.cs -- --repo github/copilot-sdk
```
**注意：** GitHub API请求是速率限制的。大型存储库或频繁运行可能会达到速率限制。详细信息请参见[GitHub API速率限制]（https://docs.github.com/rest/overview/rate-limits-for-the-rest-api）。</details>
基于文件的应用程序

这些例子使用。. NET基于文件的应用程序特性，它允许单文件c#程序：

—不带项目文件运行
—自动引用常用包
-支持顶级语句

学习资源

——[。. NET文件应用程序文档]（https://learn.microsoft.com/en-us/dotnet/core/sdk/file-based-apps）
- [GitHub CopilotSDK文档]（https://github.com/github/copilot-sdk/blob/main/dotnet/README.md）
-[家长食谱]（../README.md）