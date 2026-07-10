# Harness_Enhance — Unified AI Development Harness (Copilot Instructions)

This repository is a **merged AI development harness** that combines two complementary bodies of work
into one end-to-end autonomous software delivery system for CT medical-device software (IEC 62304 /
ISO 14971 / ISO 13485):

- **Execution Engine** (from the *CT AI Dev Harness*): an Anthropic-style **generator + evaluator** harness —
  autonomous long-running loop, context-isolated subagents, BDD + black-box UI tests, living QMS docs, 3 human gates.
- **Domain & Compliance Intelligence** (from the *CT `.github` asset library*): rich medical-device
  **risk / compliance / domain / quality-self-heal** skills — DFMEA, IFU, impact analysis, product-defect analysis,
  CT/DICOM/Spectral knowledge, TICS & CodeScene remediation, and self-contained HTML reporting.

> **Design intent:** the Execution Engine decides *who runs in what order and how work is judged*; the Domain &
> Compliance layer supplies *what "correct, safe, and compliant" means* for CT software. See
> [`.github/ASSET_CATALOG.md`](./ASSET_CATALOG.md) for the full asset-to-stage mapping.

## Workflow (unified)

Analyse → **Requirements Review + Safety Class** → Architect (ADR) **+ Impact Analysis** → Plan (Backlog + Gherkin)
→ **GATE 1** → Develop (BDD) ∥ Test-Design (black-box) → **GATE 2** → Evaluate (isolated) **+ Quality Self-Heal**
→ **Risk & Compliance (DFMEA / IFU / QMS)** → Demonstrate → **GATE 3** → Done.

The user invokes **`@orchestrator`** once. It spawns specialised subagents, each in its **own isolated context
window**, and manages the three human gates and the retry budget (max 3×, shared).

## The Unified Team

### Execution roles (primary pipeline — drive the autonomous loop)
| Agent | Role |
|-------|------|
| `@orchestrator` / `@orchestrator-parallel` | Traffic controller; spawns subagents, manages gates & retries (parallel variant runs file-isolated tasks concurrently) |
| `@analyst` | Ingests PRD/telemetry → structured requirements (FR/NFR) |
| `@sw-architect` | ADRs, component diagrams, interface contracts |
| `@product-owner` / `@product-owner-parallel` | Vertically-sliced backlog + Gherkin specs (+ file-isolation metadata) |
| `@developer` | BDD outside-in implementation (Reqnroll) within the ADR envelope |
| `@test-designer` | Black-box FlaUI acceptance tests authored from the spec ALONE (never sees production code) |
| `@dev-evaluator` | Isolated quality gate: runs ALL tests, judges quality, verifies stored tool outputs |
| `@feature-demonstrator` | Narrated feature showcase with video evidence |
| `@docs-lookup`, `@explore` | API-doc lookup; read-only codebase Q&A |

### Domain & compliance specialists (advisory depth — invoked for CT-specific rigor)
| Agent | Adds |
|-------|------|
| `@requirements-analyst` | Requirements **review**, **traceability**, IEC 62304 **safety classification**, domain validation |
| `@architect` | Design proposals + **impact-analysis** across modules/interfaces/data-flows |
| `@reviewer` | 8-dimension review incl. **TICS/CodeScene conformance** + commit-readiness (complements `@dev-evaluator`) |
| `@tester` | Additional unit-test generation (xUnit/AAA) + manual test-case design |
| `@doc-writer` | SDS, API docs, release notes, **DFMEA / IFU** drafts |
| `@cicd` | Local builds, test runs, pipeline configs |

> The primary autonomous loop uses the execution roles. The specialists are invoked by the orchestrator (or
> directly) to inject CT domain depth, risk analysis, compliance documents, and quality self-healing at the
> matching stage.

## Skills (31 — loaded on demand)

- **Requirements & domain:** requirements, requirements-review, requirements-traceability, impact-analysis, domain-knowledge, document-reader, backlog-grooming, gherkin-spec-writing
- **Architecture:** architecture, system-design
- **Implementation:** csharp-development, ct-coding-standards, reqnroll-bdd, bdd-generator
- **Test:** test-generator, nunit-testing, ui-automation, flaui-winappdriver, ux-design
- **Review & quality self-heal:** code-review, csharp-code-review, code-quality, tics-standard, codescene-health
- **Risk & compliance:** dfmea-analysis, ifu-generator, productdefect-analysis, generate-3pp-dmr, iec62304-compliance, qms-documentation
- **Reporting:** doc-generator

## Always-on standards (instructions)

| Instruction | Fires on |
|-------------|----------|
| `tics-csharp` | `**/*.cs` — copyright headers, XML docs, exception logging, namespace consistency (TICS) |
| `codescene-csharp` | `**/*.cs` — complexity limits, anti primitive-obsession, bumpy-road/brain-method prevention |
| `source-code` | `Src/**` — project source conventions |
| `build-ci` | `Build/**`, `*.yml` — CI pipeline conventions |
| `harness-state` | `.harness/**` — state-file conventions |

## Critical Rules (merged)

- **One task per session.** Do not attempt multiple tasks in one context.
- **Context isolation is the key mechanism.** `@test-designer` authors tests without seeing production code;
  `@dev-evaluator`/`@feature-demonstrator` never share the developer's context. **Domain/compliance skills are
  injected as read-only judgment criteria and must NOT break this isolation** (e.g. do not feed production
  code into the test-designer).
- **Separate generation from evaluation.** Agents cannot objectively judge their own work.
- **Verify before building.** Confirm the baseline is green (`Build\Verify-Baseline.cmd`) before changing code.
- **Follow the ADR.** Architectural decisions are binding.
- **New/modified code only.** Apply skill conventions to new/changed code; do not retrofit legacy.
- **Safety first.** Record IEC 62304 safety class (A/B/C) with justification; any patient-data-path or
  Class C impact is escalated regardless of scope. When in doubt, escalate one level.
- **No fabricated IDs.** RMM/complaint/requirement IDs use `TBD`/`N/A` until assigned by the real process.
- **Definition changes cascade down, never up.** Gate rejections / mid-execution definition gaps re-enter at
  the highest changed layer, freeze approved upstream, and re-present the combined gate (a definition gap does
  NOT consume the retry budget).
- **The orchestrator owns task status.** Only it marks a task complete, after all gates + CI pass.

## State, Build & QMS

- **State store:** `.harness/` — backlogs (JSON schema), requirements, architecture/adr + diagrams, specs
  (Gherkin), eval_feedback, tool_outputs, demo_evidence, progress.md (append-only, single source of truth).
- **Quality gates:** `Build/` batch scripts auto-resolve `Src\{repo}Impl.sln` (`Verify-Baseline.cmd`,
  `Run-QualityGate.cmd`, `Build-VerificationTests.cmd`, `Run-CombinedCoverage.cmd`).
- **QMS (audit track):** `docs/qms/` living IEC 62304 docs (SwRS, SSDS, SDD, MVP, MVProcedure, MVReport,
  VerificationPlan/Procedure) with `scripts/` md→docx export.
- **Reporting (management track):** DFMEA / IFU / code-quality skills emit **self-contained HTML** reports.

## Technology Stack (default)

C# (.NET) · MSBuild/dotnet · Reqnroll (BDD) · NUnit + NSubstitute · FlaUI (UI) · ReSharper CLI · TICS · Coverity ·
Azure DevOps · NuGet/WiX · pandoc/python-docx (QMS export).

## Where to find more

- Full asset map & pipeline stages: [`.github/ASSET_CATALOG.md`](./ASSET_CATALOG.md)
- Overview & adoption guide: [`../README_Harness.md`](../README_Harness.md)
