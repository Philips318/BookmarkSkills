# CT Repository Structure

| Path | Purpose |
|------|---------|
| `ExtInf/` | External interface definitions (governed managed API) |
| `ExtInf/{repo}Inf.sln` | Solution building all interface projects |
| `Src/{ProjectDir}/Src/` | Production source and `.csproj` for each component |
| `Src/{ProjectDir}/Test/` | Unit test project co-located with the component it tests |
| `Src/{ProjectDir}/Cfg/` | Configuration assets (XML, JSON) needed at runtime |
| `Src/{ProjectDir}/Res/` | Resource assets (images, databases) needed at runtime |
| `Src/ModuleTests/` | Functional/BDD module tests — mocks all ExtInf dependencies |
| `Src/ModuleTests/Features/` | Reqnroll `.feature` files |
| `Src/ModuleTests/StepDefinitions/` | Reqnroll step binding classes |
| `Src/{repo}Impl.sln` | Single solution building all implementation + unit test projects |
| `Build/CI/` | Azure DevOps pipeline YAML (owned by DevOps) |
| `Build/Compile/` | Compile scripts (owned by dev team) |
| `Build/Pkg/Nuget/` | NuGet packaging scripts — Inf, Impl, PostActions, Test packages |
| `Build/Pkg/MSI/` | WiX MSI packaging scripts |
| `Build/Actions/Install/` | Post-install action scripts |
| `Build/Actions/Uninstall/` | Pre-uninstall cleanup scripts |
| `Output/OutInf/` | Compiled ExtInf binaries (post-build copy target) |
| `Output/OutImpl/` | Compiled Src implementation binaries |
| `Output/OutCfg/` | Runtime configuration assets |
| `Output/OutRes/` | Runtime resource assets |
| `Output/OutTests/` | Compiled unit test and module test binaries |
| `Dependencies/Ref/` | Unpacked NuGet dependency binaries (populated from CT_CompRegistry) |
| `Export/` | Final packages: `{repo}Inf.pkg`, `{repo}Impl.pkg`, `{repo}PostActions.pkg`, `{repo}Tests.pkg`, `{repo}.msi` |
