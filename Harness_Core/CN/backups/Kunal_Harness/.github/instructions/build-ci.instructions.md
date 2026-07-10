---
applyTo: "Build/**,*.yml"
---

# Build & CI Instructions

处理构建脚本、CI pipelines 或打包时：

## CI/CD

- Pipelines 是 Azure DevOps YAML
- PR pipeline：`Build/CI/PR_Pipeline.yml`
- Integration pipeline：`Build/CI/Integration_Pipeline.yml`
- 变量被拆分到 `Variables_*.yml` 文件中
- Pipeline templates 来自共享的 `CT_CICD` 仓库

## 编译脚本

- 编译脚本位于 `Build/Compile/` — 由开发团队拥有
- 不要把 CI 编排逻辑混入编译脚本

## 打包

- NuGet specs：`Build/Pkg/Nuget/`
- MSI packaging：`Build/Pkg/MSI/`（基于 WiX）
- 版本管理：`Build/Pkg/Version.Config`、`Build/Version/Main.Config`
- 每个 repo 产出 4 个包：`{repo}Inf.pkg`、`{repo}Impl.pkg`、`{repo}PostActions.pkg`、`{repo}Tests.pkg`
- 一个 MSI 由 3 个生产包组装而成：`{repo}Inf.pkg` + `{repo}Impl.pkg` + `{repo}PostActions.pkg`

## 安装后动作

- `Build/Actions/Install/` — MSI 安装后运行的脚本
- `Build/Actions/Uninstall/` — MSI 卸载前运行的脚本
- `Build/Actions/Test/` — 设置开发/测试环境的脚本

## 输出文件夹（构建后复制目标）

- `Output/OutInf/` — ExtInf 项目二进制文件
- `Output/OutImpl/` — Src 实现二进制文件
- `Output/OutCfg/` — 运行时配置文件
- `Output/OutRes/` — 运行时资源文件（图像、数据库）
- `Output/OutTests/` — 单元测试和模块测试二进制文件

## 版本控制

格式：`{MAJOR}.{MINOR}.{PATCH}.{BUILD}`

- **MAJOR** — 破坏性接口变更；必须手动更新
- **MINOR** — 增量接口变更（向后兼容）；必须手动更新
- **PATCH** — 实现变更；当源代码变化时按 nightly build 自动递增
- **BUILD** — 每次构建自动递增；当 PATCH 变化时重置为 0
- MAJOR.MINOR 在 Inf 和 Impl 版本之间共享
- 一个 repo 中的所有接口携带相同版本
- 任何接口源代码变化时都必须更新接口版本；如果不同步，构建会失败
- Implementation MAJOR.MINOR 必须始终匹配 Interface MAJOR.MINOR
- 版本配置文件：`Build/Version/Main.Config`（MAJOR/MINOR）、`Build/Pkg/Version.Config`
- `CommonVersion.cs` 由 nightly build 自动更新：`ExtInf/CommonVersion/CommonVersion.cs` 和 `Src/CommonVersion/CommonVersion.cs`

## 质量门禁

质量门禁分两个阶段运行：**提交前本地运行**（developer 责任）和 **CI pipeline 中运行**（自动强制执行）。

### 规范本地门禁脚本（`Build/`）

Agents 和 developers 调用这些已测试的包装器，而不是手动拼装 `dotnet`/`jb`/`dotcover` 命令行。每个脚本都会自动解析 `Src/` 下的 `*Impl.sln`，内置 dotCover filters（`+:Philips.CT.*;-:*.Test*`）和报告类型，将所有原始工具输出存入 `-OutputDir`，并在任何失败时返回非零退出码（含义：修复代码/测试，而不是修命令）：

| 脚本 | 用途 | 使用者 |
|---|---|---|
| `Build\Verify-Baseline.cmd` | 在 impl solution 上执行 `dotnet build` + `dotnet test --no-build` | developer baseline |
| `Build\Run-QualityGate.cmd -OutputDir <dir>` | build + instrumented test run（`tests.trx`、`coverage.xml`）+ ReSharper（`resharper.xml`） | developer full gate |
| `Build\Build-VerificationTests.cmd -OutputDir <dir>` | build + 对 `VerificationTests.sln` 运行 ReSharper | test-designer |
| `Build\Run-CombinedCoverage.cmd -OutputDir <dir>` | combined dev + verification coverage（`coverage-combined.xml`） | evaluator |

这些门禁脚本是自包含的 Windows batch (`.cmd`) 文件 — 不使用 PowerShell，因此运行时没有 execution-policy 或 publisher-trust 摩擦。它们共享 `Build/_GateCommon.cmd`（solution resolution、规范 dotCover filter、output-dir creation），并且是规范命令的单一事实来源。下面的原始命令仅作为参考并用于 CI pipeline 文档。

### 本地提交前（developer 提交评审前运行）

| 工具 | 命令 | 通过标准 |
|---|---|---|
| **ReSharper CLI** | `jb inspectcode Src\{repo}Impl.sln --output=report.xml` | 零错误；warnings 已记录 |
| **dotCover** | `dotnet dotcover test Src\{repo}Impl.sln --dcOutput=coverage.html --dcReportType=HTML --dcFilters="+:Philips.CT.*;-:*.Test*"` | 见下方覆盖率阈值 |

ReSharper CLI 和 dotCover 作为 dotnet tools 安装：
```
dotnet tool install JetBrains.ReSharper.GlobalTools
dotnet tool install JetBrains.dotCover.CommandLineTools
```

### 覆盖率阈值（按测试类型）

| 测试类型 | Filter | 最低覆盖率 | 命令 |
|-----------|--------|-----------------|----------|
| **Unit** | `--filter Category=Unit` | 80% statement | `dotnet dotcover test --dcFilters="+:Philips.CT.*;-:*.Test*" --filter Category=Unit` |
| **Module (BDD)** | `--filter Category=Module` | 70% statement | `dotnet dotcover test --dcFilters="+:Philips.CT.*;-:*.Test*" --filter Category=Module` |
| **Integration** | `--filter Category=Integration` | 60% statement | `dotnet dotcover test --dcFilters="+:Philips.CT.*;-:*.Test*" --filter Category=Integration` |
| **Verification** | `--filter Category=Verification` | 不是单独门禁 | 作为 combined coverage 的一部分运行 |
| **Combined** | All tests（dev + verification） | 80% statement | `dotnet dotcover test Src\{repo}Impl.sln Src\VerificationTests\VerificationTests.sln --dcFilters="+:Philips.CT.*;-:*.Test*"` |

覆盖率报告生成 HTML 和 XML。XML 报告（`--dcReportType=DetailedXML`）由 `@dev-evaluator` 用于逐类分析。Combined coverage 包括 developer tests（`Src/{repo}Impl.sln`）和 evaluator verification tests（`Src/VerificationTests/VerificationTests.sln`）。

### CI pipeline 门禁（提交时自动运行，阻塞合并）

| 工具 | 用途 | 阻塞阈值 |
|---|---|---|
| **TICS (TIOBE)** | 编码标准、可维护性、测试覆盖率 | 任何被测特征的 TQI < 8.0 都会阻塞批准 |
| **Coverity** | 静态安全和安全性缺陷分析 | 任何新增 **High** 缺陷阻塞批准；新增 **Medium** 需要记录 justification |
| **ReSharper CLI** (`jb inspectcode`) | 代码风格、命名和惯用法违规 | 报告发布到 pipeline；**不**阻塞批准（信息性） |

### Pipeline step order

1. `dotnet build` — 零 warnings
2. `dotnet test --filter Category=Unit` — 所有 unit tests 通过
3. `dotnet dotcover test` — coverage gate（combined ≥ 80%）
4. TICS analysis — TQI gate
5. Coverity scan — defect gate
6. ReSharper CLI — 仅报告
7. Package and publish artifacts

### 规则

- TICS 和 Coverity 结果是**证据工件** — `@dev-evaluator` 在发出 PASS verdict 前必须确认两个门禁均已通过
- 被 suppress 或跳过的质量门禁会被视为门禁失败 — 在 PR description 中记录任何 waiver，并获得 team lead 批准
- 生产代码中的 ReSharper suppressions（`// ReSharper disable`）需要 inline justification comment
- Coverity false-positive annotations（`/* coverity[...] */`）需要引用 finding ID 的 justification comment

## 约定

- 不要修改 pipeline templates — 它们位于共享的 CT_CICD repo 中
- 版本变更通过 `Build/Pkg/Version.Config` 或 `Build/Version/Main.Config`
- PowerShell scripts 必须通过 PSScriptAnalyzer
- Batch scripts 必须正确处理 error codes
