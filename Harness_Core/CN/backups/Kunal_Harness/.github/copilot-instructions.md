# CT AI Development Harness — Copilot Instructions

此仓库使用 **Agent Harness** 模式进行自主 AI 驱动开发。
它实现了一个结构化工作流：Analyse → Architect → Plan (BDD) → Develop → Evaluate → Demonstrate → Done。

## Agents

此仓库使用一个**单一 orchestrator** (`@orchestrator`)，由它派生专门的 subagents，每个 subagent 都有自己隔离的上下文窗口。

| 智能体 | 角色 | 使用方式 |
|-------|------|---------------|
| `@orchestrator` | **Traffic controller** — 分诊、派生 subagents、管理门禁 | 用户直接调用 |
| `@analyst` | 澄清歧义，摄取输入工件（PRD、Word、PDF、Excel、telemetry）→ 结构化需求 | 由 orchestrator 派生（或独立运行） |
| `@sw-architect` | 定义 HOW：ADRs、组件图、接口契约 | 由 orchestrator 派生（或独立运行） |
| `@product-owner` | 创建垂直切片 backlog + Gherkin feature files | 由 orchestrator 派生（或独立运行） |
| `@developer` | 在架构边界内使用 BDD (Reqnroll) 实现一个任务；编写单元 + 模块测试；将工具输出存入 `.harness/tool_outputs/` | 由 orchestrator 派生（或独立运行） |
| `@test-designer` | 仅根据 feature files + requirements 编写黑盒 UI 验收测试（`Src/VerificationTests/` 下的 C#/FlaUI），与 developer 并行；绝不查看生产代码 | 由 orchestrator 派生（或独立运行） |
| `@dev-evaluator` | 质量门禁：运行全部测试（developer 的 + test-designer 的）、判断测试质量（覆盖率 + asserts）、验证 developer 存储的工具输出（仅在有疑问时重新运行）、评审代码质量、架构遵循情况、功能完整性和 demo 就绪度。不编写测试。 | 由 orchestrator 派生（或独立运行） |
| `@feature-demonstrator` | 旁白式功能展示：启动应用程序，通过视频录制带用户走查功能，并将控制权交给用户批准 | evaluator PASS 后由 orchestrator 派生 |
| `@docs-lookup` | 从可信来源获取 .NET 库的 API 文档 | 由 developer 或 architect 派生 |
| `@explore` | 快速只读代码库探索和问答 | 用户直接调用；agents 内联读取文件 |

### `@orchestrator` 的工作方式

用户使用自己的请求调用一次 `@orchestrator`。orchestrator 会：
1. 分诊工作（分类、优先级排序）
2. **派生 `@analyst`** → 与用户澄清歧义、读取输入工件、产出结构化需求
3. **派生 `@sw-architect`** → 产出 ADRs、图、接口契约
4. **派生 `@product-owner`** → 创建垂直切片 backlog + Gherkin specs
5. **[GATE：用户将需求 + 架构 + backlog 作为单个包评审]** → 如果拒绝，则从发生变更的最高层重新进入（requirements → architecture → backlog）并向下级联，然后始终重新呈现 combined gate（最多 3 次迭代）
6. 对每个任务（按依赖顺序）：
   - **并行派生 `@developer`（design）和 `@test-designer`（test design）** → developer 编写设计（`SDD`/`MVP`/`MVProcedure`）；test-designer 编写系统级验证方法（`VerificationPlan`/`VerificationProcedure`）— 尚不写代码（全新隔离上下文；互不重叠的 QMS docs）
   - **[GATE：用户评审设计 + 测试设计]** → 如果拒绝，则以 rework mode 重新派生被点名的 agent(s)，并重新呈现门禁（独立预算，最多 3 次迭代；不消耗实现重试预算）
   - **并行派生 `@developer`（impl）和 `@test-designer`（test authoring）** → developer 根据已批准设计自外向内用 BDD 实现（生产代码 + 单元/模块测试，存储工具输出）；test-designer 只根据 spec 编写黑盒 UI 验收测试（全新隔离上下文；互不重叠的文件）
   - 两者完成后，**派生 `@dev-evaluator`** → 运行所有测试、判断测试质量、验证存储的工具输出（仅在有疑问时重新运行）、评审代码质量、架构、完整性和 demo 就绪度（全新上下文，没有 developer/test-designer 记忆）
   - 如果 `type: feature`：**派生 `@feature-demonstrator`** → 针对运行中的应用进行功能展示（无源代码访问）→ **[GATE：用户批准 demo]**
   - 如果 FAIL：按 evaluator 的 `rework_target` 重新派生 — `@developer`（生产/开发测试缺陷）和/或 `@test-designer`（验证测试缺陷）— 然后重新评估（最多 3 次重试，共享预算）
   - 如果发出 `DEFINITION GAP` 信号（developer、test-designer 或 evaluator 发现 spec 本身错误）：暂停任务，按级联重新进入定义 agents，重新呈现 combined gate，然后恢复 — 不消耗重试预算
7. 报告完成

**上下文隔离是关键机制** — developer、test-designer、evaluator 和 demonstrator 各自在自己的上下文中工作。同一个 developer/test-designer 每个任务派生两次（design mode，然后 implementation mode），每次都有全新上下文，并从已批准的设计工件定向。test-designer 编写测试时从不查看生产代码；evaluator 和 demonstrator 也从不与 developer 共享上下文，从而确保评估诚实。

**用户在三个门禁处介入（combined review、per-task design & test-design review 和 feature demo），以及在任务失败 3× 时介入。**

## Project Map

完整文件夹结构参考见 [docs/harness/project-map.md](../docs/harness/project-map.md)。

## 会话协议

每个 agent 每次派生只在**一种 mode** 下工作。mode 决定生命周期 — 不存在单一线性的“coding session”。权威的逐步说明位于 `.github/agents/` 下每个 agent 的 prompt 中；下面的摘要说明每种 mode 适用的生命周期。

**通用第一步（每种 mode）：** Orient — 读取 `.harness/progress.md`、`.harness/backlogs/` 中的活动 backlog、任务的 feature 文件和相关 ADRs。

| 模式 | 派生为 | 生命周期（Orient 之后） |
|------|-----------|--------------------------|
| **Design** | `@developer` (DESIGN) | 基于 ADRs 编写设计 QMS docs（`SDD`/`MVP`/`MVProcedure`）。**不读源代码、不构建、不测试、不写代码** — 只做设计。向 progress.md 记录 DESIGN 条目。 |
| **Test-design** | `@test-designer` (TEST-DESIGN) | 仅根据 spec 编写系统级验证方法（`VerificationPlan`/`VerificationProcedure`）。不访问生产代码。记录 TEST-DESIGN 条目。 |
| **Implementation** | `@developer` (IMPL) | 验证基线 → BDD 自外向内：step definitions（Red）→ production code（Green）→ 在 ADR 架构内重构（feature files 不可变）。运行本地质量门禁、存储工具输出并记录条目。 |
| **Test-authoring** | `@test-designer` (AUTHORING) | 仅根据 feature files 在 `Src/VerificationTests/` 中编写黑盒 FlaUI/NUnit 验收测试 — 绝不读取生产代码。构建测试解决方案、存储工具输出并记录条目。 |
| **Evaluation** | `@dev-evaluator` | 运行全部测试、判断测试质量、验证存储的工具输出、评审代码/架构/完整性/demo 就绪度。不编写测试。将裁决写入 `.harness/eval_feedback/` 和 `docs/qms/MVReport.md`，并记录条目。 |

**本地质量门禁（仅 implementation 和 test-authoring modes）：**
```
Build\Run-QualityGate.cmd -OutputDir .harness\tool_outputs\{slug}_{task-id}
```
规范门禁脚本位于 `Build/` 下（它们会自动解析 `*Impl.sln`、内置 dotCover filters，并存储所有工具输出）：`Verify-Baseline.cmd`（developer baseline）、`Run-QualityGate.cmd`（developer full gate）、`Build-VerificationTests.cmd`（test-designer）、`Run-CombinedCoverage.cmd`（evaluator）。Agents 调用这些脚本，而不是手动拼装 `dotnet`/`jb`/`dotcover` 命令。所有测试都必须通过；ReSharper 零错误（warnings 为信息性）。

**更新状态（每种 mode）：** 完成前使用符合角色的模板追加到 `.harness/progress.md` — 它是 pipeline 状态的单一事实来源。只有 orchestrator 在所有门禁通过后（evaluator + demo + CI）将任务标记为 `complete`。

## 关键规则

- **每个会话一个任务。** 不要在单个上下文中尝试多个任务。
- **绝不从 backlog 文件中删除条目。** 只能将 `status` 从 `pending` 改为 `in_progress` 再改为 `complete`。
- **禁止占位实现。** 每个函数都必须完整实现。不要写 `TODO`、`throw new NotImplementedException()` 或 stub code。
- **构建前先验证。** 触碰新代码前始终确认现有功能可用。
- **遵循 ADR。** 架构决策是强制性的 — 不要偏离 `@sw-architect` 定义的模式。
- **只处理新增/修改代码。** 只将 skills 中的编码、设计和命名约定应用于新增或修改代码。除非任务明确要求，否则不要把约定改造到未触碰的现有代码上。
- **记录决策。** 如果你做出 ADR 未覆盖的架构选择，请记录到 progress.md，并标记给 `@sw-architect`。

## 验证命令

实现解决方案始终命名为 `Src\{repo}Impl.sln`，其中 `{repo}` 是仓库名（例如 `CT_Device` → `Src\CT_DeviceImpl.sln`）。`Build/` 门禁脚本会自动解析它；如需手动验证，请在 `Src/` 下找到实际 `.sln` 并使用它。

```
Build\Verify-Baseline.cmd
```

这会针对解析出的 `*Impl.sln` 运行 `dotnet build` + `dotnet test --no-build`。实现前后都必须通过验证。

## 技术栈

- **Language:** C# (.NET)
- **Build:** MSBuild / dotnet CLI
- **BDD Framework:** Reqnroll（SpecFlow 继任者）
- **Testing:** MSTest / NUnit（项目特定）+ Reqnroll scenarios
- **UI Automation:** FlaUI（WPF/WinForms）— 由 `@feature-demonstrator` 使用
- **CI:** Azure DevOps Pipelines
- **Packaging:** NuGet、MSI (WiX)
- **Document Processing:** pandoc、python-docx、pypdf、openpyxl（用于 analyst artifact ingestion 和 QMS doc generation）

## 更多信息位置

- **Project map:** `docs/harness/project-map.md`
- Backlog JSON schema: `.harness/backlogs/backlog-schema.json`
- Backlog template: `.harness/backlogs/_template.json`
