---
applyTo: "Src/**"
---

# Source Code Instructions

在 `Src/` 目录中工作时：

## 约定

- 遵循项目中现有的代码风格和命名约定
- 使用 C# 命名约定：公共成员使用 PascalCase，私有字段使用 _camelCase
- Namespace：`Philips.CT.Host.{Framework/ComponentName}.{SubComponent}` — 绝不使用 VS 默认值
- 所有 public 和 protected 成员都必须有 XML documentation comments
- 不要为未使用命名空间保留 `using` 语句
- 局部变量声明使用 `var`；例外：primitive types（`int`、`string`、`double` 等）
- 方法 ≤ 20 行；类 ≤ 200 行
- 新代码中每个方法的 cyclomatic complexity ≤ 6
- 构建必须产生**零 warnings**；warnings 按 errors 处理
- 不要保留注释掉的代码；不要写没有 justification 的 `TODO`
- 在变量名中包含单位：`positionInMm`、`timeoutInMs`、`angleInDeg`
- 不要用 `Thread.Sleep()` 做同步 — 使用 events 或 semaphores
- 所有 UI strings 来自 resource files；不要硬编码 UI strings 或 paths
- 避免 WPF code-behind；严格遵循 MVVM

## 测试

- 每个 public method 都必须有对应的 unit tests
- Unit test projects 与其组件共置：`Src/{ProjectDir}/Test/`
- BDD/functional module tests 位于 `Src/ModuleTests/` — 它们模拟所有 `ExtInf` 依赖
- Test method naming：`MethodName_Scenario_ExpectedResult`
- 使用 Arrange-Act-Assert pattern

## 架构

- 遵循现有分层架构：Interface (ExtInf) → Implementation (Src)
- 新功能放入合适的现有项目；只有必要时才创建新项目
- 尊重 `ExtInf`（contracts/interfaces）和 `Src`（implementations）之间的分离
- 跨仓库依赖必须只引用 `{repo}Inf.pkg` Interface NuGet — 绝不引用 `{repo}Impl.pkg`
- `Dependencies/Ref/` 保存所有解包后的依赖二进制文件；所有项目都从这里引用 assemblies，不直接从 Artifactory 引用

## 构建验证

任何代码更改后，使用以下命令验证：
```
Build\Verify-Baseline.cmd
```
这个规范脚本会自动解析 `Src/` 下的 `*Impl.sln`，并运行 `dotnet build` + `dotnet test --no-build`。完整本地质量门禁（build + coverage + ReSharper）请使用 `Build\Run-QualityGate.cmd -OutputDir <dir>`。
