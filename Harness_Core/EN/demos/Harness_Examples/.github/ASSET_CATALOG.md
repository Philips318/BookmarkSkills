# Unified Harness — Asset Catalog & Stage Map

This catalog maps every merged asset to the unified pipeline. **Source** legend:
🟦 **EXEC** = from the CT AI Dev Harness (execution engine) · 🟪 **DOMAIN** = from the CT `.github` asset library (domain/compliance).

Totals after merge: **18 agents · 31 skills · 12 prompts · 5 instructions** + `.harness/` state, `Build/` gates, `docs/qms/` living QMS, `scripts/` QMS export.

## Pipeline stages → assets

| Stage | Gate | Primary agent | Supporting skills | Specialist / advisory |
|-------|------|---------------|-------------------|-----------------------|
| **1. Analyse** | | 🟦 `@analyst` | 🟦 document-reader, backlog-grooming · 🟪 requirements, domain-knowledge | 🟪 `@requirements-analyst` |
| **2. Requirements review + safety class** | | 🟪 `@requirements-analyst` | 🟪 requirements-review, requirements-traceability · 🟦 iec62304-compliance | |
| **3. Architecture** | | 🟦 `@sw-architect` | 🟦 system-design · 🟪 architecture | 🟪 `@architect` |
| **4. Impact analysis** | | 🟪 `@architect` | 🟪 impact-analysis | |
| **5. Plan (backlog + Gherkin)** | | 🟦 `@product-owner`(/`-parallel`) | 🟦 backlog-grooming, gherkin-spec-writing · 🟪 bdd-generator | |
| — | **⏸ GATE 1** — requirements + architecture + backlog + risk (combined review) | 🟦 `@orchestrator` | | |
| **6. Develop (BDD)** | | 🟦 `@developer` | 🟦 csharp-development, ct-coding-standards, reqnroll-bdd | instructions: 🟪 tics-csharp, codescene-csharp (always-on) |
| **7. Test-design (black-box, isolated)** ∥ | | 🟦 `@test-designer` | 🟦 ui-automation, flaui-winappdriver, ux-design · 🟪 test-generator, nunit-testing | |
| — | **⏸ GATE 2** — design + test design | 🟦 `@orchestrator` | | |
| **8. Evaluate (isolated)** | | 🟦 `@dev-evaluator` | 🟦 csharp-code-review · 🟪 code-review | 🟪 `@reviewer` (8-dim + TICS/CodeScene) |
| **9. Quality self-heal** | | 🟪 `@reviewer` | 🟪 code-quality, tics-standard, codescene-health | |
| **10. Risk & compliance** | | 🟪 `@doc-writer` | 🟪 dfmea-analysis, ifu-generator, productdefect-analysis, generate-3pp-dmr · 🟦 qms-documentation, iec62304-compliance | |
| **11. Documentation (dual-track)** | | 🟪 `@doc-writer` | 🟪 doc-generator (HTML) · 🟦 qms-documentation (md→docx) | |
| **12. Demonstrate** | | 🟦 `@feature-demonstrator` | | |
| — | **⏸ GATE 3** — feature demo | 🟦 `@orchestrator` | | |
| **13. Build & CI** | | 🟪 `@cicd` | `Build/` gate scripts · TICS (TQI≥8.0) · Coverity | |

Support (any stage): 🟦 `@docs-lookup` (.NET API docs), 🟦 `@explore` (read-only Q&A).

## Agents (18)

**Execution (🟦 12):** orchestrator, orchestrator-parallel, analyst, sw-architect, product-owner, product-owner-parallel, developer, test-designer, dev-evaluator, feature-demonstrator, docs-lookup, explore
**Domain specialists (🟪 6):** requirements-analyst, architect, reviewer, tester, doc-writer, cicd

## Skills (31)

**🟦 EXEC (14):** backlog-grooming, csharp-code-review, csharp-development, ct-coding-standards, document-reader, flaui-winappdriver, gherkin-spec-writing, iec62304-compliance, nunit-testing, qms-documentation, reqnroll-bdd, system-design, ui-automation, ux-design
**🟪 DOMAIN (17):** architecture, bdd-generator, code-quality, code-review, codescene-health, dfmea-analysis, doc-generator, domain-knowledge, generate-3pp-dmr, ifu-generator, impact-analysis, productdefect-analysis, requirements, requirements-review, requirements-traceability, test-generator, tics-standard

## Prompts (12)

**🟦 EXEC (2):** orient, qms-export
**🟪 DOMAIN (10):** full-pipeline, code-review, code-quality, tics-preflight, codescene-preflight, dfmea-analysis, ifu-generator, productdefect-analysis, generate-3pp-dmr, quick-test

## Instructions (5)

**🟦 EXEC (3, path-triggered):** source-code (`Src/**`), build-ci (`Build/**`,`*.yml`), harness-state (`.harness/**`)
**🟪 DOMAIN (2, `**/*.cs`):** tics-csharp, codescene-csharp

## Overlap notes (to resolve during Phase 1–2 tuning)

| Overlap | Execution (🟦) | Domain (🟪) | Recommended composition |
|---------|----------------|-------------|-------------------------|
| Requirements | analyst | requirements-analyst | analyst ingests → requirements-analyst reviews + safety class + traceability |
| Architecture | sw-architect | architect | sw-architect owns ADR → architect adds cross-module impact-analysis |
| Evaluation | dev-evaluator (isolated, runs tests) | reviewer (8-dim + TICS/CodeScene) | dev-evaluator = objective gate; reviewer = standards/self-heal pass |
| Testing | test-designer (black-box UI) | tester (unit/manual) | test-designer for acceptance; tester for unit/manual coverage |
| Code review skill | csharp-code-review | code-review, code-quality | keep both; code-quality adds HTML report |
| BDD | reqnroll-bdd, gherkin-spec-writing | bdd-generator | exec skills for Reqnroll authoring; bdd-generator for AC→Gherkin conversion |
