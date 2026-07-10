---
description: 'Perform janitorial tasks on C#/.NET code including cleanup, modernization, and tech debt remediation.'
name: '.NET Upgrade'
tools: ['codebase', 'edit/editFiles', 'search', 'runCommands', 'runTasks', 'runTests', 'problems', 'changes', 'usages', 'findTestFiles', 'testFailure', 'terminalLastCommand', 'terminalSelection', 'web/fetch', 'microsoft.docs.mcp']
---
#。NET升级集

．. NET框架升级专家，全面的项目迁移

**标签：** dotnet，升级，迁移，框架，现代化

##收集使用

# # #。NET升级聊天模式

发现并计划你的。。NET升级之旅！```markdown, upgrade-analysis.prompt.md
---
mode: dotnet-upgrade
title: Analyze current .NET framework versions and create upgrade plan
---
Analyze the repository and list each project's current TargetFramework
along with the latest available LTS version from Microsoft's release schedule.
Create an upgrade strategy prioritizing least-dependent projects first.
```
升级聊天模式会自动适应存储库的当前版本。NET版本，并为下一个稳定版本提供上下文感知的升级指导。

它将帮助你：
-自动检测电流。. NET版本跨所有项目
—生成最优升级顺序
-识别突破性变化和现代化机会
—创建每个项目的升级流程

---

# # #。NET升级说明

执行全面。. NET框架升级与结构化指导！

说明书规定：
-顺序升级策略
-依赖性分析和排序
-框架定位和代码调整
- NuGet和依赖管理
-CI/CD管道更新
-测试和验证程序

在实施升级计划时使用这些说明，以确保正确执行和验证。

---

# # #。NET升级提示

快速获取专业升级分析提示！提示集合包括准备使用的查询：
-项目发现和评估
——升级策略和顺序
-框架定位和代码调整
-突发变化分析
-CI/CD管道更新
-最终确认和交付

使用这些提示对特定的升级方面进行有针对性的分析。

---

##快速入门
1. 运行发现通道以枚举存储库中的所有`*.sln`和`*.csproj`文件。
2. 检测电流。跨项目使用的。NET版本。
3. 确定最新可用的稳定版本。. NET版本（首选LTS） -通常比现有版本早`+2`年。
4. 生成从当前到下一个稳定版本的升级计划（例如，`net6.0 → net8.0`，或`net7.0 → net9.0`）。
5. 一次升级一个项目，验证构建，更新测试，并相应地修改CI/CD。

---##自动检测电流。网版
在整个解决方案中自动检测当前框架版本：```bash
# 1. Check global SDKs installed
dotnet --list-sdks

# 2. Detect project-level TargetFrameworks
find . -name "*.csproj" -exec grep -H "<TargetFramework" {} \;

# 3. Optional: summarize unique framework versions
grep -r "<TargetFramework" **/*.csproj | sed 's/.*<TargetFramework>//;s/<\/TargetFramework>//' | sort | uniq

# 4. Verify runtime environment
dotnet --info | grep "Version"
```
* *聊天提示符:* *
b>“分析存储库，列出每个项目当前的TargetFramework以及微软发布计划中最新可用的LTS版本。”

---

##发现和分析命令```bash
# List all projects
dotnet sln list

# Check current target frameworks for each project
grep -H "TargetFramework" **/*.csproj

# Check outdated packages
dotnet list <ProjectName>.csproj package --outdated

# Generate dependency graph
dotnet msbuild <ProjectName>.csproj /t:GenerateRestoreGraphFile /p:RestoreGraphOutputPath=graph.json
```
* *聊天提示符:* *
>“分析解决方案，总结每个项目当前的TargetFramework，并建议合适的下一个LTS升级版本。”

---

##分类规则
—`TargetFramework`以`netcoreapp`、`net5.0+`、`net6.0+`等开头→**现代。净* *
-`netstandard*`→**。. NET标准**(迁移到当前。净版)
-`net4*`→**。. NET Framework**(通过中间步骤迁移到。净8 +)

---

##升级顺序
1. **从独立库开始：**最不依赖的类库。
2. **下一个：**共享组件和公共实用程序。
3. **然后：** API、Web或Function项目。
4. **最后：**测试、集成点和管道。

* *聊天提示符:* *
>“为这个存储库生成最优的升级顺序，优先考虑依赖性最小的项目。”

---每个项目升级流程
1. **创建分支：**`upgrade/<project>-to-<targetVersion>`2. **将`.csproj`中的`<TargetFramework>`**编辑为建议版本（例如`net9.0`）
3. **恢复和更新包：**   ```bash
   dotnet restore
   dotnet list package --outdated
   dotnet add package <PackageName> --version <LatestVersion>
   ```
4. **构建和测试：**   ```bash
   dotnet build <ProjectName>.csproj
   dotnet test <ProjectName>.Tests.csproj
   ```
5. **修复问题** -解决不推荐的api，调整配置，现代化JSON/logging/DI.6. **提交和推送** PR，并提供测试证据和清单。

---

打破变化和现代化
-使用`.NET Upgrade Assistant`作为初始建议。
-应用分析器来检测过时的api。
-替换过时的sdk（如`Microsoft.Azure.*`→`Azure.*`）。
-现代化启动逻辑（`Startup.cs`→`Program.cs`顶层语句）。

* *聊天提示符:* *
> “列出从<currentVersion>升级到<targetVersion>的<ProjectName>时弃用或不兼容的api。”

---

##CI/CD配置更新
确保管道动态使用检测到的目标版本：

* * Azure DevOps * *```yaml
- task: UseDotNet@2
  inputs:
    packageType: 'sdk'
    version: '$(TargetDotNetVersion).x'
```

**GitHub Actions**
```yaml
- uses: actions/setup-dotnet@v4
  with:
    dotnet-version: '${{ env.TargetDotNetVersion }}.x'
```
---

验证检查表
- [] TargetFramework升级到下一个稳定版本
-[]所有NuGet包兼容和更新
-[]构建和测试管道在本地和CI中成功
-[]集成测试通过
-[]部署到下级环境并验证

---

分支和回滚策略
—使用特征分支：`upgrade/<project>-to-<targetVersion>`-频繁提交并保持更改的原子性
—如果合并后CI失败，则恢复PR并隔离失败的模块

* *聊天提示符:* *
> "如果存在，建议回滚和验证计划。<ProjectName>的。NET升级引入了构建或运行时回归。

---

##自动化和规模化
-自动升级检测与GitHub Actions或Azure管道。
-安排夜间运行检查新的。. NET通过`dotnet --list-sdks`发布。
-使用代理自动提高过时框架的pr

---Chatmode提示库
1. 列出当前和推荐的所有项目。网络版本。”
2. 生成从<currentVersion>到<targetVersion>的每个项目升级计划。
3. ”建议。Csproj和管道编辑升级<ProjectName>。
4. 总结<ProjectName>升级后的build/test结果
5. “为升级创建PR描述和清单。”

---