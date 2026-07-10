---
name: nuget-manager
description: 'Manage NuGet packages in .NET projects/solutions. Use this skill when adding, removing, or updating NuGet package versions. It enforces using `dotnet` CLI for package management and provides strict procedures for direct file edits only when updating versions.'
---
# NuGet管理器

# #概述

该技能确保了跨NuGet包的一致和安全管理。网络项目。它优先使用`dotnet`CLI来维护项目的完整性，并为版本更新强制执行严格的验证和恢复工作流。

# #先决条件

-。. NET SDK安装(通常。. NET 8.0 SDK或更高版本，或与目标解决方案兼容的版本)。
-`dotnet`CLI在你的`PATH`上可用。
-`jq`（JSON处理器）或PowerShell（使用`dotnet package search`进行版本验证）。

核心规则

1.  **永远不要**直接编辑`.csproj`，`.props`，或`Directory.Packages.props`文件**添加**或**删除**包。始终使用`dotnet add package`和`dotnet remove package`命令。
2.  **直接编辑**只允许**更改现有软件包的**版本。
3.  **版本更新**必须遵循强制性的工作流程：    - Verify the target version exists on NuGet.
    - Determine if versions are managed per-project (`.csproj`) or centrally (`Directory.Packages.props`).
    - Update the version string in the appropriate file.
    - Immediately run `dotnet restore` to verify compatibility.
# #工作流程

添加一个包
使用`dotnet add [<PROJECT>] package <PACKAGE_NAME> [--version <VERSION>]`。
例如:`dotnet add src/MyProject/MyProject.csproj package Newtonsoft.Json`删除一个包
使用`dotnet remove [<PROJECT>] package <PACKAGE_NAME>`。
例如:`dotnet remove src/MyProject/MyProject.csproj package Newtonsoft.Json`更新包版本
更新版本时，请遵循以下步骤：

1.  **验证版本存在**：    Check if the version exists using the `dotnet package search` command with exact match and JSON formatting. 
    Using `jq`:
    `dotnet package search <PACKAGE_NAME> --exact-match --format json | jq -e '.searchResult[].packages[] | select(.version == "<VERSION>")'`
    Using PowerShell:
    `(dotnet package search <PACKAGE_NAME> --exact-match --format json | ConvertFrom-Json).searchResult.packages | Where-Object { $_.version -eq "<VERSION>" }`
2.  **确定版本管理**：    - Search for `Directory.Packages.props` in the solution root. If present, versions should be managed there via `<PackageVersion Include="Package.Name" Version="1.2.3" />`.
    - If absent, check individual `.csproj` files for `<PackageReference Include="Package.Name" Version="1.2.3" />`.
3.  * * * *应用变化:    Modify the identified file with the new version string.
4.  * * * *验证稳定:    Run `dotnet restore` on the project or solution. If errors occur, revert the change and investigate.
# #的例子

###用户：“添加Serilog到WebApi项目”
**动作**：执行`dotnet add src/WebApi/WebApi.csproj package Serilog`。

###用户：“更新Newtonsoft。Json到13.0.3在整个解决方案”
* *行动* *:
1. 验证13.0.3是否存在：`dotnet package search Newtonsoft.Json --exact-match --format json`（并解析输出以确认“13.0.3”是否存在）。
2. 找到它的定义位置（例如，`Directory.Packages.props`）。
3. 编辑该文件以更新版本。
4.`dotnet restore`运行。