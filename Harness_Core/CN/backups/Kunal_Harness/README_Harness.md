# CT AI Development Harness

这是一个结构化工作流，用于在 VS Code 中通过 **GitHub Copilot** 进行自主 AI 驱动的医疗器械软件开发。它实现了生成器 + 评估器模式，并包含 IEC 62304 合规、QMS 文档和多门禁质量控制。

灵感来自 [Anthropic 的 harness 研究](https://www.anthropic.com/engineering/harness-design-long-running-apps)。

## 快速开始

1. 在启用 GitHub Copilot 的 VS Code 中打开此仓库
2. 打开 Copilot Chat (`Ctrl+Shift+I`)
3. 使用你的功能请求、缺陷报告或改进想法调用 `@orchestrator`
4. 在三个门禁处批准：**combined review**（需求 + 架构 + backlog）、**per-task design & test-design review** 和 **feature demo**
5. harness 会自主实现、评估并演示每个任务
6. 只有当某个任务**失败 3×** 时，你才需要再次介入

### 示例

```
@orchestrator Add exponential backoff retry logic to the device communication layer.
              Should handle transient network failures with configurable max retries.
```

harness 将会：分诊 → 分析 → 设计架构 → 创建 backlog + Gherkin specs → **[你批准该包]** → 使用 BDD 实现每个任务 → 评估 → demo（如果是功能）→ **[你批准 demo]** → 报告。

## 架构

```
┌──────────────────────────────────────────────────────────────────┐
│                     @orchestrator                                  │
├──────────────────────────────────────────────────────────────────┤
│                                                                    │
│  User Request                                                      │
│       │                                                            │
│       ▼                                                            │
│  ┌─────────┐    ┌─────────┐                                       │
│  │ Triage  │───▶│  Route  │  (orchestrator handles directly)      │
│  └─────────┘    └─────────┘                                       │
│                      │                                             │
│                      ▼                                             │
│  ┌───────────────────────────────────────────┐                    │
│  │  SPAWN @analyst (own context)             │                    │
│  │  → clarifies with user if needed           │                    │
│  │  → reads PRD/telemetry → FR-XX/NFR-XX    │                    │
│  │  → updates docs/qms/SwRS.md              │                    │
│  └───────────────────────────────────────────┘                    │
│                      │                                             │
│                      ▼                                             │
│  ┌───────────────────────────────────────────┐                    │
│  │  SPAWN @sw-architect (own context)        │                    │
│  │  → ADRs, diagrams, interface contracts    │                    │
│  │  → updates docs/qms/SSDS.md              │                    │
│  └───────────────────────────────────────────┘                    │
│                      │                                             │
│                      ▼                                             │
│  ┌───────────────────────────────────────────┐                    │
│  │  SPAWN @product-owner (own context)       │                    │
│  │  → vertical slices + Gherkin specs        │                    │
│  └───────────────────────────────────────────┘                    │
│                      │                                             │
│      ⏸ GATE 1: USER REVIEWS COMBINED PACKAGE                      │
│        (requirements + architecture + backlog)                      │
│              │                                                      │
│       REJECT?─── YES → re-enter at highest changed layer,            │
│              │        cascade down, re-present GATE 1 (max 3)        │
│              NO                                                     │
│              │                                                      │
│                      ▼                                             │
│     ┌─────────────────────────────────────────────────┐           │
│     │  Per-Task Loop (dependency order):               │           │
│     │                                                   │           │
│     │  ┌─────────────────────────────────────────┐    │           │
│     │  │ SPAWN @developer (design) ∥             │    │           │
│     │  │       @test-designer (test design)      │    │           │
│     │  │ dev → docs/qms/SDD + MVP + MVProcedure  │    │           │
│     │  │ td  → docs/qms/VerificationPlan + Proc  │    │           │
│     │  │ (no code yet, own contexts)             │    │           │
│     │  └─────────────────────────────────────────┘    │           │
│     │                 │                                 │           │
│     │      ⏸ GATE 2: USER REVIEWS DESIGN +            │           │
│     │          TEST DESIGN                             │           │
│     │       REJECT?── YES → re-spawn dev/td in         │           │
│     │              │       rework mode, re-present     │           │
│     │              │       GATE 2 (own budget, max 3)  │           │
│     │              NO                                  │           │
│     │                 ▼                                 │           │
│     │  ┌─────────────────────────────────────────┐    │           │
│     │  │ SPAWN @developer (impl) ∥               │    │           │
│     │  │       @test-designer (test authoring)   │    │           │
│     │  │ (parallel, own contexts)                │    │           │
│     │  │ dev → BDD outside-in + module tests +   │    │           │
│     │  │       stored tool_outputs               │    │           │
│     │  │ td  → black-box FlaUI tests from spec   │    │           │
│     │  │       (Src/VerificationTests/, no src)  │    │           │
│     │  │ → update QMS only if impl deviates      │    │           │
│     │  └─────────────────────────────────────────┘    │           │
│     │                 │                                 │           │
│     │                 ▼                                 │           │
│     │  ┌─────────────────────────────────────────┐    │           │
│     │  │ SPAWN @dev-evaluator (own context)      │    │           │
│     │  │ → runs ALL tests + judges test quality  │    │           │
│     │  │ → verifies dev's stored tool_outputs    │    │           │
│     │  │ → updates docs/qms/MVReport.md          │    │           │
│     │  └─────────────────────────────────────────┘    │           │
│     │                 │                                 │           │
│     │          PASS? ─┤── NO → re-spawn per rework_   │           │
│     │                 │       target (developer and/or │           │
│     │                 │       test-designer; max 3,    │           │
│     │                 │       shared budget)           │           │
│     │                 │   DEFINITION GAP → pause task, │           │
│     │                 │   re-enter definition agents,  │           │
│     │                 │   re-present GATE 1, resume     │           │
│     │                 │   (does NOT consume retries)    │           │
│     │                YES                               │           │
│     │                 │                                 │           │
│     │    if feature:  ▼                                │           │
│     │  ┌─────────────────────────────────────────┐    │           │
│     │  │ SPAWN @feature-demonstrator             │    │           │
│     │  │ → narrated feature showcase             │    │           │
│     │  │ → video recording of app window         │    │           │
│     │  └─────────────────────────────────────────┘    │           │
│     │                 │                                 │           │
│     │          ⏸ GATE 3: USER APPROVES DEMO            │           │
│     │                 │                                 │           │
│     │                 ▼                                 │           │
│     │     Orchestrator marks task complete              │           │
│     └─────────────────────────────────────────────────┘           │
│                      │                                             │
│                      ▼                                             │
│                    Done                                             │
│                                                                    │
├──────────────────────────────────────────────────────────────────┤
│  State: .harness/ (backlogs, specs, requirements, architecture)   │
│  QMS:   docs/qms/ (SwRS, SSDS, SDD, MVP, MVProcedure, MVReport,  │
│         VerificationPlan, VerificationProcedure)                 │
└──────────────────────────────────────────────────────────────────┘
```

**关键设计机制：**每个 subagent 都在自己的**隔离上下文窗口**中运行。test-designer 只根据 spec 编写测试，从不查看生产代码；evaluator 和 demonstrator 也没有 developer 会话的记忆 — 这确保评估诚实且无偏。

### 独立使用

可以直接调用单个 agent 进行手动控制：

| 智能体 | 独立使用 |
|-------|---------------|
| `@analyst` | 摄取 PRD，并在不运行完整 pipeline 的情况下产出需求 |
| `@sw-architect` | 基于现有需求设计架构 |
| `@product-owner` | 从现有需求 + ADRs 创建 backlog |
| `@product-owner-parallel` | 同上，并额外生成用于并发执行的文件隔离元数据 |
| `@developer` | 从现有 backlog 实现特定任务 |
| `@test-designer` | 仅根据 spec 为某个任务编写黑盒 FlaUI 验收测试 |
| `@dev-evaluator` | 独立评审已完成的任务 |
| `@docs-lookup` | 从可信来源获取 .NET 库的 API 文档 |
| `@explore` | 快速只读代码库问答（仅面向用户） |

对于端到端运行，请调用 `@orchestrator`（顺序执行）或 `@orchestrator-parallel`（当 backlog 带有来自 `@product-owner-parallel` 的文件隔离元数据时并发运行独立任务）。

## 仓库结构

```
CT_AIDevHarness/
├── .github/
│   ├── copilot-instructions.md           # Always-loaded: workflow, rules, tech stack
│   ├── agents/
│   │   ├── orchestrator.agent.md         # End-to-end workflow controller (sequential)
│   │   ├── orchestrator-parallel.agent.md# Same workflow, runs independent tasks concurrently
│   │   ├── analyst.agent.md              # Requirements extraction
│   │   ├── sw-architect.agent.md         # Architecture & design
│   │   ├── product-owner.agent.md        # Backlog & Gherkin specs
│   │   ├── product-owner-parallel.agent.md # Backlog + file-isolation metadata for parallelism
│   │   ├── developer.agent.md            # BDD implementation
│   │   ├── test-designer.agent.md        # Black-box FlaUI acceptance tests (spec-only)
│   │   ├── dev-evaluator.agent.md        # Skeptical quality gate (runs all tests)
│   │   ├── feature-demonstrator.agent.md # Narrated feature showcase
│   │   ├── docs-lookup.agent.md          # API documentation lookup
│   │   └── explore.agent.md              # Read-only exploration
│   ├── instructions/
│   │   ├── source-code.instructions.md   # Auto-fires on Src/** edits
│   │   ├── build-ci.instructions.md      # Auto-fires on Build/** and *.yml
│   │   └── harness-state.instructions.md # Auto-fires on .harness/** edits
│   ├── skills/                           # Domain knowledge (loaded on demand)
│   │   ├── ct-coding-standards/          # Shared C# naming & rules
│   │   ├── csharp-development/           # Production code patterns
│   │   ├── csharp-code-review/           # Code review checklist
│   │   ├── nunit-testing/                # Unit test patterns
│   │   ├── reqnroll-bdd/                 # BDD step definitions
│   │   ├── gherkin-spec-writing/         # Feature file quality
│   │   ├── backlog-grooming/             # INVEST, vertical slicing
│   │   ├── system-design/                # ADRs, C4 diagrams
│   │   ├── qms-documentation/            # QMS living doc maintenance
│   │   ├── iec62304-compliance/          # Safety class & traceability
│   │   ├── document-reader/              # PRD/telemetry ingestion
│   │   ├── ux-design/                    # Design tokens & UI terminology
│   │   ├── ui-automation/                # Page object pattern
│   │   └── flaui-winappdriver/           # FlaUI specifics
│   ├── prompts/
│       ├── orient.prompt.md              # Session orientation checklist
│       └── qms-export.prompt.md          # Markdown → Word conversion (manual-only)
│   └── scripts/
│       ├── Extract-QmsSkeleton.py        # Distills PDLM .docx templates into canonical md skeletons (structure contract)
│       ├── Verify-QmsStructure.py        # Enforces docs/qms/*.md conformance to their skeletons
│       ├── Export-Qms.py                 # Pre-tested QMS md→docx export (verify + pandoc + mermaid + cover merge)
│       └── Export-Qms.bat                # Batch wrapper (uses repo venv Python)
├── Build/                                # Canonical quality-gate scripts (pure batch; auto-resolve *Impl.sln)
│   ├── _GateCommon.cmd                   # Shared batch helpers: resolve solution, filters, output dir
│   ├── Verify-Baseline.cmd               # build + test --no-build (developer baseline)
│   ├── Run-QualityGate.cmd               # build + instrumented test + ReSharper (developer full gate)
│   ├── Build-VerificationTests.cmd       # build + ReSharper on VerificationTests.sln (test-designer)
│   └── Run-CombinedCoverage.cmd          # combined dev + verification coverage (evaluator)
├── .harness/
│   ├── backlogs/                         # Task backlog JSONs
│   │   ├── backlog-schema.json           # Validation schema
│   │   └── _template.json               # New backlog template
│   ├── requirements/                     # Analyst output (FR-XX, NFR-XX)
│   ├── architecture/
│   │   ├── adr/                          # Architecture Decision Records
│   │   └── diagrams/                     # Mermaid component diagrams
│   ├── specs/                            # Gherkin feature files (by backlog)
│   ├── eval_feedback/                    # Evaluator verdict JSONs
│   ├── tool_outputs/                     # Developer-stored build/test/coverage outputs (per task)
│   ├── demo_evidence/                    # Demonstrator video recordings + logs
│   └── progress.md                       # Append-only session notes
├── .vscode/                              # Workspace settings (subagent invocation, etc.)
└── docs/
    ├── harness/
    │   └── project-map.md               # CT repository structure reference
    ├── qms/                             # Living QMS documents (IEC 62304)
    │   ├── SwRS.md                       # Software Requirements Specification
    │   ├── SSDS.md                       # Sub-System Design Specification
    │   ├── SDD.md                        # Software Design Document
    │   ├── MVP.md                        # Module Verification Plan
    │   ├── MVProcedure.md                # Module Verification Procedure
    │   ├── MVReport.md                   # Module Verification Report
    │   ├── VerificationPlan.md           # System Verification Plan (test-designer; skeleton pending)
    │   └── VerificationProcedure.md      # System Verification Procedure (test-designer; skeleton pending)
    └── qms-templates/                    # PDLM Word templates (reference docs)
        └── skeletons/                    # Generated md structure contracts (one per template)
```

**按需创建或提供（不随 harness 一起交付）：**

| 路径 | 来源 |
|------|--------|
| `Input PRD/`, `Input Telemetry/` | 你在调用 `@analyst` 前将输入工件放到这里 |
| `Src/`（包括 `Src/VerificationTests/`） | 目标仓库源代码 — 生产代码和 developer tests（由 `@developer` 创建），以及 `@test-designer` 在 `Src/VerificationTests/` 中创建的独立黑盒 FlaUI tests，执行期间创建 |
| `.harness/demo_runners/` | 由 `@feature-demonstrator` 按需创建，用于持久化 demo automation projects |

## 质量门禁

harness 在多个层级强制执行质量：

| 门禁 | 工具 | 时机 | 是否阻塞？ |
|------|------|------|-----------|
| 设计与测试设计评审 | `@developer`（设计）+ `@test-designer`（测试设计） | 每个任务，实现前 | 是（用户门禁） |
| 构建 + 测试 | `dotnet build/test` | 每个 developer 会话 | 是 |
| 代码检查 | `jb inspectcode` (ReSharper CLI) | 提交前 | 是（零错误） |
| 黑盒验收测试 | `@test-designer` 编写（`Src/VerificationTests/` 中的 C#/FlaUI） | 与 developer 并行 | 先编写，然后由 evaluator 运行 |
| 白盒评审 + 测试运行 | `@dev-evaluator`（运行全部测试，判断质量） | 实现后 | 是 |
| 功能展示 | `@feature-demonstrator`（旁白式 demo） | evaluator PASS 后 | 是（用户门禁） |
| 组合覆盖率 | Developer tests + verification tests | Evaluator 第 5 节 | 是（≥ 阈值） |
| 编码标准 | TICS (TIOBE) | CI pipeline | 是（TQI ≥ 8.0） |
| 安全扫描 | Coverity | CI pipeline | 是（无新增 High） |

## 设计原则

1. **上下文隔离是关键机制。** 每个 subagent 都获得全新上下文 — test-designer 在不查看生产代码的情况下编写测试，evaluator 和 demonstrator 也从不看到 developer 的推理。
2. **将生成与评估分离。** Agents 不能客观评判自己的工作。
3. **每个会话一个任务。** 聚焦可以避免上下文耗尽。
4. **构建前先验证。** 实现前始终确认基线为绿色。
5. **结构化工件桥接会话。** Backlogs、ADRs、progress notes 和 QMS docs 是共享记忆。
6. **只处理新增/修改代码。** Skills 将约定应用到新代码 — 不要改造遗留代码。
7. **orchestrator 拥有任务状态。** Developers 不标记任务完成；所有门禁通过后由 orchestrator 标记。
8. **定义变更向下级联，绝不向上。** 门禁拒绝和执行中出现的定义缺口会从发生变更的最高层重新进入（requirements → architecture → backlog），修订其下所有内容，冻结已批准的上游工件，并始终重新呈现 combined gate。定义缺口会暂停任务，但不消耗其重试预算。

## 应用于另一个仓库

要在另一个 CT 仓库上使用此 harness：

1. **将这些目录复制到目标 repo：**
   - `.github/`（copilot-instructions.md、agents/、instructions/、skills/、prompts/）
   - `.harness/`（backlogs/、requirements/、architecture/、specs/、eval_feedback/、demo_evidence/、progress.md）
   - `.vscode/`（subagent invocation setting）
   - `docs/harness/`
   - `docs/qms/`
   - `docs/qms-templates/`

2. **自定义：**
   - verification command 会自动解析到 `Src\{repo}Impl.sln` — 只需确保你的 `.sln` 遵循 `{repo}Impl.sln` 命名约定
   - 如果不是 C#/.NET，请更新 `copilot-instructions.md` technology stack
   - 按项目特定约定调整 `source-code.instructions.md`
   - 按你的 CI pipeline 变量调整 `build-ci.instructions.md`

3. **提供输入工件：**
   - 创建 `Input PRD/` 并将 PRD 文档放入其中
   - 创建 `Input Telemetry/` 并将 telemetry data 放入其中

4. **调用：**在 Copilot Chat 中使用你的请求调用 `@orchestrator`。

## 技术栈（默认）

| 关注点 | 工具 |
|---------|------|
| 语言 | C# (.NET) |
| 构建 | MSBuild / `dotnet` CLI |
| BDD | Reqnroll（SpecFlow 继任者） |
| 单元测试 | NUnit 3.x + NSubstitute |
| UI automation | FlaUI（WPF/WinForms） |
| CI | Azure DevOps Pipelines |
| 打包 | NuGet + WiX MSI |
| QMS compliance | IEC 62304（Class A/B/C） |
| Code quality | ReSharper CLI、TICS、Coverity |

## 参考

- [Effective Harnesses for Long-Running Agents](https://www.anthropic.com/engineering/effective-harnesses-for-long-running-agents) — Anthropic，2025 年 11 月
- [Harness Design for Long-Running Application Development](https://www.anthropic.com/engineering/harness-design-long-running-apps) — Anthropic，2026 年 3 月
- [celesteanders/harness](https://github.com/celesteanders/harness) — Claude Code 的参考实现
