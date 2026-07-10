# Dependency Discovery Patterns

Companion reference for the `impact-analysis` skill. Generic, team-portable patterns for finding all callers and consumers of a changed component.

## Search Pattern Cookbook

### A. Source-code references (same solution)

| Goal | Pattern | Tool |
|------|---------|------|
| Find class usage | `\bClassName\b` (word boundary) | grep / regex search |
| Find method calls | `\.MethodName\(` | grep |
| Find interface implementations | `:\s*ITargetInterface\b` | grep regex |
| Find override / new | `override\s+ReturnType\s+MethodName` | grep |
| Find event handler attachments | `\+=\s*.*EventName` | grep |
| Find typeof / nameof | `typeof\(\s*Target\s*\)|nameof\(\s*Target\s*\)` | grep regex |
| Find DI registrations | `Register<.*Target.*>|RegisterType<.*Target.*>` | grep |

### B. Build-graph references (project / DLL level)

| Goal | Pattern | Files to inspect |
|------|---------|------------------|
| Find project references | `<ProjectReference\s+Include="[^"]*TargetProject` | `*.csproj` |
| Find package references | `<PackageReference\s+Include="TargetPackage` | `*.csproj` |
| Find DLL HintPath references | `<HintPath>[^<]*TargetAssembly\.dll` | `*.csproj` |
| Find NuGet `packages.config` | `id="TargetPackage"` | `packages.config` |
| Find `Directory.Build.props` overrides | `<.*Target.*>` | `Directory.Build.*` |

### C. Configuration & runtime composition

| Goal | Pattern | Files |
|------|---------|-------|
| Find config keys | `TargetSettingKey` | `*.config`, `appsettings*.json` |
| Find feature flags | `FeatureFlag.*Target|Target.*Enabled` | configs, code |
| Find logging categories | `Logger.*Target|<logger name="Target` | configs |

### D. Cross-solution discovery

When the change crosses solution boundaries:

1. **List published artifacts** — what does the changed project produce? (DLL name, NuGet package id)
2. **Search those artifact names** across every solution in the workspace, not just the current one
3. **Check for mirrored / forked projects** — many medical-device codebases have parallel project families (e.g., `<Module>_Dev` / `<Module>_<variant>` pairs); a change to one often needs mirroring in the other
4. **Check shared output folders** — projects that publish to a shared `Bin/` (instead of local `bin/`) imply consumers reference compiled output, not source

### E. Indirect / dynamic dependencies (easy to miss)

These find no hit with class-name search but **still break**:

- **Reflection** — `Type.GetType("Namespace.Class")` with string literal
- **Activator** — `Activator.CreateInstance(typeof(...))`
- **Serialization** — JSON/XML deserialization by type-name string
- **Resource lookups** — `Application.GetResource("...")` for XAML / images
- **Database column names** — schema migrations affecting persisted DTOs
- **DICOM tag handling** — adding/removing private tags breaks downstream parsers
- **Test fixtures** — test base classes, shared test data files
- **Code generators / T4 templates** — generated code depending on input shape

## Search Discipline

- **Run all six categories from `impact-analysis-process.md` Step 2**, even when one feels sufficient.
- **Record file:line of every hit**, not just the file. Reviewers need to verify.
- **Categorize false positives explicitly** (Step 3 "None" with reason); silent drops look suspicious.
- **Sort results by impact level** (Direct → Indirect → Potential), not by file path.

## When Search Cannot Find Something

If a dependency exists but no pattern surfaces it (e.g., compiled-only consumers in a different repo), the analysis must:

1. Document the suspected dependency explicitly under "Unverified Consumers"
2. Recommend a verification owner (typically the maintainer of the suspected consumer)
3. Treat it as Med risk at minimum
