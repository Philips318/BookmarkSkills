# Dependency Discovery Patterns

`impact-analysis` skill 的 companion reference。用于查找 changed component 的所有 callers 和 consumers 的 generic、team-portable patterns。

## Search Pattern Cookbook

### A. Source-code references (same solution)

| 目标 | 模式 | Tool |
|------|---------|------|
| Find class usage | `\bClassName\b` (word boundary) | grep / regex search |
| Find method calls | `\.MethodName\(` | grep |
| Find interface implementations | `:\s*ITargetInterface\b` | grep regex |
| Find override / new | `override\s+ReturnType\s+MethodName` | grep |
| Find event handler attachments | `\+=\s*.*EventName` | grep |
| Find typeof / nameof | `typeof\(\s*Target\s*\)|nameof\(\s*Target\s*\)` | grep regex |
| Find DI registrations | `Register<.*Target.*>|RegisterType<.*Target.*>` | grep |

### B. Build-graph references (project / DLL level)

| 目标 | 模式 | Files to inspect |
|------|---------|------------------|
| Find project references | `<ProjectReference\s+Include="[^"]*TargetProject` | `*.csproj` |
| Find package references | `<PackageReference\s+Include="TargetPackage` | `*.csproj` |
| Find DLL HintPath references | `<HintPath>[^<]*TargetAssembly\.dll` | `*.csproj` |
| Find NuGet `packages.config` | `id="TargetPackage"` | `packages.config` |
| Find `Directory.Build.props` overrides | `<.*Target.*>` | `Directory.Build.*` |

### C. Configuration & runtime composition

| 目标 | 模式 | Files |
|------|---------|-------|
| Find config keys | `TargetSettingKey` | `*.config`, `appsettings*.json` |
| Find feature flags | `FeatureFlag.*Target|Target.*Enabled` | configs, code |
| Find logging categories | `Logger.*Target|<logger name="Target` | configs |

### D. Cross-solution discovery

当 change 跨 solution boundaries 时：

1. **List published artifacts** — changed project 会产出什么？（DLL name、NuGet package id）
2. **Search those artifact names** — 在 workspace 中每个 solution 搜索，而不是只搜当前 solution
3. **Check for mirrored / forked projects** — 许多 medical-device codebases 有并行 project families（例如 `<Module>_Dev` / `<Module>_<variant>` pairs）；改一个通常也需要 mirror 到另一个
4. **Check shared output folders** — 发布到 shared `Bin/`（而非 local `bin/`）的 projects 表明 consumers 引用 compiled output，而不是 source

### E. Indirect / dynamic dependencies (easy to miss)

这些用 class-name search 找不到，但**仍然会 break**：

- **Reflection** — `Type.GetType("Namespace.Class")` with string literal
- **Activator** — `Activator.CreateInstance(typeof(...))`
- **Serialization** — JSON/XML deserialization by type-name string
- **Resource lookups** — `Application.GetResource("...")` for XAML / images
- **Database column names** — schema migrations affecting persisted DTOs
- **DICOM tag handling** — adding/removing private tags breaks downstream parsers
- **Test fixtures** — test base classes, shared test data files
- **Code generators / T4 templates** — generated code depending on input shape

## Search Discipline

- **Run all six categories from `impact-analysis-process.md` Step 2**，即使其中某一类看起来已经足够。
- **Record file:line of every hit**，不要只记录 file。Reviewers 需要验证。
- **Categorize false positives explicitly**（Step 3 "None" with reason）；silent drops 看起来像漏查。
- **Sort results by impact level**（Direct → Indirect → Potential），不要按 file path 排序。

## When Search Cannot Find Something

如果 dependency 存在但没有 pattern 能暴露它（例如不同 repo 中 compiled-only consumers），analysis 必须：

1. 在 "Unverified Consumers" 下明确记录 suspected dependency
2. 推荐 verification owner（通常是 suspected consumer 的 maintainer）
3. 至少按 Med risk 处理
