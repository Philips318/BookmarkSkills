# Unified Harness — 资产目录与阶段映射

本目录将每个合并后的资产映射到统一 pipeline。**来源**图例：
🟦 **EXEC** = 来自 CT AI Dev Harness（执行引擎） · 🟪 **DOMAIN** = 来自 CT `.github` 资产库（领域/合规）。

合并后总计：**18 agents · 31 skills · 12 prompts · 5 instructions** + `.harness/` 状态、`Build/` 门禁、`docs/qms/` living QMS、`scripts/` QMS 导出。

## Pipeline 阶段 → 资产

| 阶段 | 门禁 | 主要 agent | 支持 skills | 专家 / 咨询 |
|-------|------|---------------|-------------------|-----------------------|
| **1. Analyse** | | 🟦 `@analyst` | 🟦 document-reader, backlog-grooming · 🟪 requirements, domain-knowledge | 🟪 `@requirements-analyst` |
| **2. 需求评审 + 安全等级** | | 🟪 `@requirements-analyst` | 🟪 requirements-review, requirements-traceability · 🟦 iec62304-compliance | |
| **3. 架构** | | 🟦 `@sw-architect` | 🟦 system-design · 🟪 architecture | 🟪 `@architect` |
| **4. 影响分析** | | 🟪 `@architect` | 🟪 impact-analysis | |
| **5. 计划（backlog + Gherkin）** | | 🟦 `@product-owner`(/`-parallel`) | 🟦 backlog-grooming, gherkin-spec-writing · 🟪 bdd-generator | |
| — | **⏸ GATE 1** — 需求 + 架构 + backlog + 风险（combined review） | 🟦 `@orchestrator` | | |
| **6. 开发（BDD）** | | 🟦 `@developer` | 🟦 csharp-development, ct-coding-standards, reqnroll-bdd | instructions: 🟪 tics-csharp, codescene-csharp（always-on） |
| **7. 测试设计（黑盒、隔离）** ∥ | | 🟦 `@test-designer` | 🟦 ui-automation, flaui-winappdriver, ux-design · 🟪 test-generator, nunit-testing | |
| — | **⏸ GATE 2** — 设计 + 测试设计 | 🟦 `@orchestrator` | | |
| **8. 评估（隔离）** | | 🟦 `@dev-evaluator` | 🟦 csharp-code-review · 🟪 code-review | 🟪 `@reviewer`（8 维 + TICS/CodeScene） |
| **9. 质量自愈** | | 🟪 `@reviewer` | 🟪 code-quality, tics-standard, codescene-health | |
| **10. 风险与合规** | | 🟪 `@doc-writer` | 🟪 dfmea-analysis, ifu-generator, productdefect-analysis, generate-3pp-dmr · 🟦 qms-documentation, iec62304-compliance | |
| **11. 文档（双轨）** | | 🟪 `@doc-writer` | 🟪 doc-generator（HTML） · 🟦 qms-documentation（md→docx） | |
| **12. 演示** | | 🟦 `@feature-demonstrator` | | |
| — | **⏸ GATE 3** — 功能 demo | 🟦 `@orchestrator` | | |
| **13. Build & CI** | | 🟪 `@cicd` | `Build/` gate scripts · TICS (TQI≥8.0) · Coverity | |

支持（任意阶段）：🟦 `@docs-lookup`（.NET API docs），🟦 `@explore`（只读 Q&A）。

## Agents (18)

**执行（🟦 12）：** orchestrator, orchestrator-parallel, analyst, sw-architect, product-owner, product-owner-parallel, developer, test-designer, dev-evaluator, feature-demonstrator, docs-lookup, explore
**领域专家（🟪 6）：** requirements-analyst, architect, reviewer, tester, doc-writer, cicd

## Skills (31)

**🟦 EXEC (14)：** backlog-grooming, csharp-code-review, csharp-development, ct-coding-standards, document-reader, flaui-winappdriver, gherkin-spec-writing, iec62304-compliance, nunit-testing, qms-documentation, reqnroll-bdd, system-design, ui-automation, ux-design
**🟪 DOMAIN (17)：** architecture, bdd-generator, code-quality, code-review, codescene-health, dfmea-analysis, doc-generator, domain-knowledge, generate-3pp-dmr, ifu-generator, impact-analysis, productdefect-analysis, requirements, requirements-review, requirements-traceability, test-generator, tics-standard

## Prompts (12)

**🟦 EXEC (2)：** orient, qms-export
**🟪 DOMAIN (10)：** full-pipeline, code-review, code-quality, tics-preflight, codescene-preflight, dfmea-analysis, ifu-generator, productdefect-analysis, generate-3pp-dmr, quick-test

## Instructions (5)

**🟦 EXEC（3，path-triggered）：** source-code (`Src/**`), build-ci (`Build/**`,`*.yml`), harness-state (`.harness/**`)
**🟪 DOMAIN（2，`**/*.cs`）：** tics-csharp, codescene-csharp

## 重叠说明（在 Phase 1–2 调优期间解决）

| 重叠 | 执行（🟦） | 领域（🟪） | 推荐组合方式 |
|---------|----------------|-------------|-------------------------|
| 需求 | analyst | requirements-analyst | analyst 摄取 → requirements-analyst 评审 + 安全等级 + 可追溯性 |
| 架构 | sw-architect | architect | sw-architect 拥有 ADR → architect 补充跨模块 impact-analysis |
| 评估 | dev-evaluator（隔离，运行测试） | reviewer（8 维 + TICS/CodeScene） | dev-evaluator = 客观门禁；reviewer = 标准/自愈检查 |
| 测试 | test-designer（黑盒 UI） | tester（unit/manual） | test-designer 负责验收；tester 负责 unit/manual 覆盖 |
| Code review skill | csharp-code-review | code-review, code-quality | 两者保留；code-quality 补充 HTML report |
| BDD | reqnroll-bdd, gherkin-spec-writing | bdd-generator | exec skills 负责 Reqnroll 编写；bdd-generator 负责 AC→Gherkin 转换 |
