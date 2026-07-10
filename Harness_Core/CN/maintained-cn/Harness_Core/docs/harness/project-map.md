# CT 仓库结构

| 路径 | 用途 |
|------|---------|
| `ExtInf/` | 外部接口定义（受治理的托管 API） |
| `ExtInf/{repo}Inf.sln` | 构建所有接口项目的解决方案 |
| `Src/{ProjectDir}/Src/` | 每个组件的生产源代码和 `.csproj` |
| `Src/{ProjectDir}/Test/` | 与被测组件共置的单元测试项目 |
| `Src/{ProjectDir}/Cfg/` | 运行时所需的配置资产（XML、JSON） |
| `Src/{ProjectDir}/Res/` | 运行时所需的资源资产（图像、数据库） |
| `Src/ModuleTests/` | 功能/BDD 模块测试 — 模拟所有 ExtInf 依赖 |
| `Src/ModuleTests/Features/` | Reqnroll `.feature` 文件 |
| `Src/ModuleTests/StepDefinitions/` | Reqnroll 步骤绑定类 |
| `Src/{repo}Impl.sln` | 构建所有实现项目和单元测试项目的单一解决方案 |
| `Build/CI/` | Azure DevOps pipeline YAML（由 DevOps 拥有） |
| `Build/Compile/` | 编译脚本（由开发团队拥有） |
| `Build/Pkg/Nuget/` | NuGet 打包脚本 — Inf、Impl、PostActions、Test packages |
| `Build/Pkg/MSI/` | WiX MSI 打包脚本 |
| `Build/Actions/Install/` | 安装后动作脚本 |
| `Build/Actions/Uninstall/` | 卸载前清理脚本 |
| `Output/OutInf/` | 编译后的 ExtInf 二进制文件（构建后复制目标） |
| `Output/OutImpl/` | 编译后的 Src 实现二进制文件 |
| `Output/OutCfg/` | 运行时配置资产 |
| `Output/OutRes/` | 运行时资源资产 |
| `Output/OutTests/` | 编译后的单元测试和模块测试二进制文件 |
| `Dependencies/Ref/` | 解包后的 NuGet 依赖二进制文件（从 CT_CompRegistry 填充） |
| `Export/` | 最终包：`{repo}Inf.pkg`、`{repo}Impl.pkg`、`{repo}PostActions.pkg`、`{repo}Tests.pkg`、`{repo}.msi` |
