---
name: dotnet-upgrade
description: 'Ready-to-use prompts for comprehensive .NET framework upgrade analysis and execution'
---
#项目发现与评估
-名称：“工程项目分类分析”    prompt: "Identify all projects in the solution and classify them by type (`.NET Framework`, `.NET Core`, `.NET Standard`). Analyze each `.csproj` for its current `TargetFramework` and SDK usage."
-名称：“依赖兼容性审查”    prompt: "Review external and internal dependencies for framework compatibility. Determine the upgrade complexity based on dependency graph depth."
-名称：“遗留包检测”    prompt: "Identify legacy `packages.config` projects needing migration to `PackageReference` format."
#升级策略和排序
-名称：“项目升级订购”    prompt: "Recommend a project upgrade order from least to most dependent components. Suggest how to isolate class library upgrades before API or Azure Function migrations."
-名称：“增量策略规划”    prompt: "Propose an incremental upgrade strategy with rollback checkpoints. Evaluate the use of **Upgrade Assistant** or **manual upgrades** based on project structure."
-名称：“进度跟踪设置”    prompt: "Generate an upgrade checklist for tracking build, test, and deployment readiness across all projects."
#框架定位和代码调整
-名称：“目标框架选择”    prompt: "Suggest the correct `TargetFramework` for each project (e.g., `net8.0`). Review and update deprecated SDK or build configurations."
-名称：“代码现代化分析”    prompt: "Identify code patterns needing modernization (e.g., `WebHostBuilder` → `HostBuilder`). Suggest replacements for deprecated .NET APIs and third-party libraries."
-名称：“异步模式转换”    prompt: "Recommend conversion of synchronous calls to async where appropriate for improved performance and scalability."
# NuGet &依赖管理
-名称：“程式包相容性分析”    prompt: "Analyze outdated or incompatible NuGet packages and suggest compatible versions. Identify third-party libraries that lack .NET 8 support and provide migration paths."
-名称：“共享依赖策略”    prompt: "Recommend strategies for handling shared dependency upgrades across projects. Evaluate usage of legacy packages and suggest alternatives in Microsoft-supported namespaces."
-名称：“传递依赖审查”    prompt: "Review transitive dependencies and potential version conflicts after upgrade. Suggest resolution strategies for dependency conflicts."
#CI/CD&构建管道更新
-名称：“管道结构分析”    prompt: "Analyze YAML build definitions for SDK version pinning and recommend updates. Suggest modifications for `UseDotNet@2` and `NuGetToolInstaller` tasks."
-名称：“建设管道现代化”    prompt: "Generate updated build pipeline snippets for .NET 8 migration. Recommend validation builds on feature branches before merging to main."
-名称：“CI自动化增强”    prompt: "Identify opportunities to automate test and build verification in CI pipelines. Suggest strategies for continuous integration validation."
#测试和验证
-名称：“构建验证策略”    prompt: "Propose validation checks to ensure the upgraded solution builds and runs successfully. Recommend automated test execution for unit and integration suites post-upgrade."
-名称：“服务集成验证”    prompt: "Generate validation steps to verify logging, telemetry, and service connectivity. Suggest strategies for verifying backward compatibility and runtime behavior."
-名称：“部署就绪检查”    prompt: "Recommend UAT deployment verification steps before production rollout. Create comprehensive testing scenarios for upgraded components."
突发变化分析
-名称：“API弃用检测”    prompt: "Identify deprecated APIs or removed namespaces between target versions. Suggest automated scanning using `.NET Upgrade Assistant` and API Analyzer."
-名称：“API替代策略”    prompt: "Recommend replacement APIs or libraries for known breaking areas. Review configuration changes such as `Startup.cs` → `Program.cs` refactoring."
-名称：“回归测试焦点”    prompt: "Suggest regression testing scenarios focused on upgraded API endpoints or services. Create test plans for critical functionality validation."
#版本控制和提交策略
-名称：“分支策略规划”    prompt: "Recommend branching strategy for safe upgrade with rollback capability. Generate commit templates for partial and complete project upgrades."
-名称：“公关结构优化”    prompt: "Suggest best practices for creating structured PRs (`Upgrade to .NET [Version]`). Identify tagging strategies for PRs involving breaking changes."
-名称：“代码评审指引”    prompt: "Recommend peer review focus areas (build, test, and dependency validation). Create checklists for effective upgrade reviews."
#文件和沟通
-名称：“升级文档策略”    prompt: "Suggest how to document each project's framework change in the PR. Propose automated release note generation summarizing upgrades and test results."
-名称：“持份者沟通”    prompt: "Recommend communicating version upgrades and migration timelines to consumers. Generate documentation templates for dependency updates and validation results."
-名称：“进度跟踪系统”    prompt: "Suggest maintaining an upgrade summary dashboard or markdown checklist. Create templates for tracking upgrade progress across multiple projects."
#工具和自动化
-名称：“升级工具选择”    prompt: "Recommend when and how to use: `.NET Upgrade Assistant`, `dotnet list package --outdated`, `dotnet migrate`, and `graph.json` dependency visualization."
-名称：“分析脚本生成”    prompt: "Generate scripts or prompts for analyzing dependency graphs before upgrading. Propose AI-assisted prompts for Copilot to identify upgrade issues automatically."
-名称：“多存储库验证”    prompt: "Suggest how to validate automation output across multiple repositories. Create standardized validation workflows for enterprise-scale upgrades."
#最终验证和交付
-名称：“最终解决方案验证”    prompt: "Generate validation steps to confirm the final upgraded solution passes all validation checks. Suggest production deployment verification steps post-upgrade."
-名称：“部署就绪确认”    prompt: "Recommend generating final test results and build artifacts. Create a checklist summarizing completion across projects (builds/tests/deployment)."
-名称：“发布文件”    prompt: "Generate a release note summarizing framework changes and CI/CD updates. Create comprehensive upgrade summary documentation."

---
