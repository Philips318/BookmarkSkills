# Harness_Enhance — 统一 AI Development Harness（Copilot Instructions）

此 repository 是一个**合并后的 AI 开发 harness**，将两套互补工作成果
组合为一个面向 CT 医疗器械软件（IEC 62304 /
ISO 14971 / ISO 13485）的端到端自主软件交付系统：

- **执行引擎**（来自 *CT AI Dev Harness*）：Anthropic-style **generator + evaluator** harness —
  自主长运行循环、上下文隔离的 subagents、BDD + 黑盒 UI 测试、living QMS docs、3 个 human gates。
- **领域与合规智能**（来自 *CT `.github` asset library*）：丰富的医疗器械
  **risk / compliance / domain / quality-self-heal** skills — DFMEA、IFU、impact analysis、product-defect analysis、
  CT/DICOM/Spectral knowledge、TICS & CodeScene remediation，以及自包含 HTML 报告。

> **设计意图：** Execution Engine 决定*谁以什么顺序运行，以及如何评判工作*；Domain &
> Compliance layer 提供 CT software 的*“正确、安全、合规”含义*。完整资产到阶段映射见
> [`.github/ASSET_CATALOG.md`](./ASSET_CATALOG.md)。

## Workflow（统一）

Analyse → **Requirements Review + Safety Class** → Architect (ADR) **+ Impact Analysis** → Plan (Backlog + Gherkin)
→ **GATE 1** → Develop (BDD) ∥ Test-Design (black-box) → **GATE 2** → Evaluate (isolated) **+ Quality Self-Heal**
→ **Risk & Compliance (DFMEA / IFU / QMS)** → Demonstrate → **GATE 3** → Done。

用户只调用一次 **`@orchestrator`**。它会派生专门的 subagents，每个 subagent 位于其**自己的隔离上下文
窗口**，并管理三个人工门禁以及重试预算（最多 3×，共享）。

## 统一团队

### 执行角色（主 pipeline — 驱动自主循环）
| 智能体 | 角色 |
|-------|------|
| `@orchestrator` / `@orchestrator-parallel` | Traffic controller；派生 subagents，管理 gates & retries（parallel variant 会并发运行文件隔离任务） |
| `@analyst` | 摄取 PRD/telemetry → 结构化 requirements (FR/NFR) |
| `@sw-architect` | ADRs、组件图、interface contracts |
| `@product-owner` / `@product-owner-parallel` | 垂直切片 backlog + Gherkin specs（+ file-isolation metadata） |
| `@developer` | 在 ADR envelope 内进行 BDD outside-in implementation (Reqnroll) |
| `@test-designer` | 仅根据 spec 编写黑盒 FlaUI acceptance tests（从不查看 production code） |
| `@dev-evaluator` | 隔离 quality gate：运行 ALL tests，判断质量，验证已存储 tool outputs |
| `@feature-demonstrator` | 带 video evidence 的旁白式 feature showcase |
| `@docs-lookup`, `@explore` | API-doc lookup；只读 codebase Q&A |

### 领域与合规专家（咨询深度 — 为 CT-specific rigor 调用）
| 智能体 | 补充内容 |
|-------|------|
| `@requirements-analyst` | Requirements **review**、**traceability**、IEC 62304 **safety classification**、domain validation |
| `@architect` | Design proposals + 跨 modules/interfaces/data-flows 的 **impact-analysis** |
| `@reviewer` | 8-dimension review，含 **TICS/CodeScene conformance** + commit-readiness（补充 `@dev-evaluator`） |
| `@tester` | Additional unit-test generation (xUnit/AAA) + manual test-case design |
| `@doc-writer` | SDS、API docs、release notes、**DFMEA / IFU** drafts |
| `@cicd` | Local builds、test runs、pipeline configs |

> 主自主循环使用执行角色。specialists 由 orchestrator（或
> 直接）调用，用于在匹配阶段注入 CT domain depth、risk analysis、compliance documents 和 quality self-healing。

## Skills（31 — 按需加载）

- **Requirements & domain:** requirements, requirements-review, requirements-traceability, impact-analysis, domain-knowledge, document-reader, backlog-grooming, gherkin-spec-writing
- **Architecture:** architecture, system-design
- **Implementation:** csharp-development, ct-coding-standards, reqnroll-bdd, bdd-generator
- **Test:** test-generator, nunit-testing, ui-automation, flaui-winappdriver, ux-design
- **Review & quality self-heal:** code-review, csharp-code-review, code-quality, tics-standard, codescene-health
- **Risk & compliance:** dfmea-analysis, ifu-generator, productdefect-analysis, generate-3pp-dmr, iec62304-compliance, qms-documentation
- **Reporting:** doc-generator

## Always-on standards（instructions）

| Instruction | 触发范围 |
|-------------|----------|
| `tics-csharp` | `**/*.cs` — copyright headers, XML docs, exception logging, namespace consistency (TICS) |
| `codescene-csharp` | `**/*.cs` — complexity limits, anti primitive-obsession, bumpy-road/brain-method prevention |
| `source-code` | `Src/**` — project source conventions |
| `build-ci` | `Build/**`, `*.yml` — CI pipeline conventions |
| `harness-state` | `.harness/**` — state-file conventions |

## 关键规则（合并版）

- **每个会话一个任务。** 不要在一个上下文中尝试多个任务。
- **上下文隔离是关键机制。** `@test-designer` 在不查看 production code 的情况下编写 tests；
  `@dev-evaluator`/`@feature-demonstrator` 从不共享 developer 的上下文。**Domain/compliance skills 以
  只读评判标准注入，且不得破坏这种隔离**（例如，不要把 production
  code 提供给 test-designer）。
- **将生成与评估分离。** Agents 不能客观评判自己的工作。
- **构建前先验证。** 改代码前确认 baseline 为绿色（`Build\Verify-Baseline.cmd`）。
- **遵循 ADR。** 架构决策具有约束力。
- **只处理新增/修改代码。** 将 skill conventions 应用于新增/变更代码；不要改造 legacy。
- **安全优先。** 记录 IEC 62304 safety class (A/B/C) 及理由；任何 patient-data-path 或
  Class C impact 都无论 scope 如何一律升级。存疑时，上调一级。
- **不要编造 IDs。** RMM/complaint/requirement IDs 在真实流程分配前使用 `TBD`/`N/A`。
- **定义变更向下级联，绝不向上。** Gate rejections / mid-execution definition gaps 会从
  最高变更层重新进入，冻结已批准的 upstream，并重新呈现 combined gate（definition gap 不
  消耗 retry budget）。
- **orchestrator 拥有任务状态。** 只有它在所有 gates + CI 通过后将任务标记为 complete。

## State, Build & QMS

- **State store:** `.harness/` — backlogs (JSON schema)、requirements、architecture/adr + diagrams、specs
  (Gherkin)、eval_feedback、tool_outputs、demo_evidence、progress.md（append-only，single source of truth）。
- **Quality gates:** `Build/` batch scripts auto-resolve `Src\{repo}Impl.sln` (`Verify-Baseline.cmd`,
  `Run-QualityGate.cmd`, `Build-VerificationTests.cmd`, `Run-CombinedCoverage.cmd`)。
- **QMS（审计轨）：** `docs/qms/` living IEC 62304 docs（SwRS, SSDS, SDD, MVP, MVProcedure, MVReport,
  VerificationPlan/Procedure），带 `scripts/` md→docx export。
- **Reporting（管理轨）：** DFMEA / IFU / code-quality skills 输出**自包含 HTML** reports。

## Technology Stack（默认）

C# (.NET) · MSBuild/dotnet · Reqnroll (BDD) · NUnit + NSubstitute · FlaUI (UI) · ReSharper CLI · TICS · Coverity ·
Azure DevOps · NuGet/WiX · pandoc/python-docx (QMS export).

## 更多信息位置

- 完整 asset map 与 pipeline stages: [`.github/ASSET_CATALOG.md`](./ASSET_CATALOG.md)
- Overview & adoption guide: [`../README_Harness.md`](../README_Harness.md)
