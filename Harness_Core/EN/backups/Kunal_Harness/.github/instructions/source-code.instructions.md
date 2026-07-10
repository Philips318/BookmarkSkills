---
applyTo: "Src/**"
---

# Source Code Instructions

When working in the `Src/` directory:

## Conventions

- Follow existing code style and naming conventions in the project
- Use C# naming conventions: PascalCase for public members, _camelCase for private fields
- Namespace: `Philips.CT.Host.{Framework/ComponentName}.{SubComponent}` — never the VS default
- All public and protected members must have XML documentation comments
- No `using` statements for unused namespaces
- Use `var` for local variable declarations; exception: primitive types (`int`, `string`, `double`, etc.)
- Methods ≤ 20 lines; classes ≤ 200 lines
- Cyclomatic complexity ≤ 6 per method for new code
- Build must produce **zero warnings**; warnings are treated as errors
- No commented-out code; no `TODO` without justification
- Include variable units in names: `positionInMm`, `timeoutInMs`, `angleInDeg`
- No `Thread.Sleep()` for synchronisation — use events or semaphores
- All UI strings from resource files; no hardcoded UI strings or paths
- Avoid code-behind in WPF; follow MVVM strictly

## Testing

- Every public method must have corresponding unit tests
- Unit test projects are co-located with their component: `Src/{ProjectDir}/Test/`
- BDD/functional module tests live in `Src/ModuleTests/` — these mock all `ExtInf` dependencies
- Test method naming: `MethodName_Scenario_ExpectedResult`
- Use Arrange-Act-Assert pattern

## Architecture

- Follow the existing layered architecture: Interface (ExtInf) → Implementation (Src)
- New functionality goes in the appropriate existing project; create new projects only when necessary
- Respect the separation between `ExtInf` (contracts/interfaces) and `Src` (implementations)
- Cross-repository dependencies must reference the `{repo}Inf.pkg` Interface NuGet only — never the `{repo}Impl.pkg`
- `Dependencies/Ref/` holds all unpacked dependency binaries; all projects reference assemblies from here, not from Artifactory directly

## Build Verification

After any code change, verify with:
```
Build\Verify-Baseline.cmd
```
This canonical script auto-resolves the `*Impl.sln` under `Src/` and runs `dotnet build` + `dotnet test --no-build`. For the full local quality gate (build + coverage + ReSharper) use `Build\Run-QualityGate.cmd -OutputDir <dir>`.
