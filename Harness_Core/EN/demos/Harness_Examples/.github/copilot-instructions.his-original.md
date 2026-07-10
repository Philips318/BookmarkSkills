# CT AI Development Harness — Copilot Instructions

This repository uses the **Agent Harness** pattern for autonomous AI-driven development.
It implements a structured workflow: Analyse → Architect → Plan (BDD) → Develop → Evaluate → Demonstrate → Done.

## Agents

This repository uses a **single orchestrator** (`@orchestrator`) that spawns specialized subagents, each with their own isolated context window.

| Agent | Role | How It's Used |
|-------|------|---------------|
| `@orchestrator` | **Traffic controller** — triages, spawns subagents, manages gates | User invokes directly |
| `@analyst` | Clarifies ambiguities, ingests input artifacts (PRD, Word, PDF, Excel, telemetry) → structured requirements | Spawned by orchestrator (or standalone) |
| `@sw-architect` | Defines the HOW: ADRs, component diagrams, interface contracts | Spawned by orchestrator (or standalone) |
| `@product-owner` | Creates vertically-sliced backlog + Gherkin feature files | Spawned by orchestrator (or standalone) |
| `@developer` | Implements one task with BDD (Reqnroll) within architectural envelope; writes unit + module tests; stores tool outputs in `.harness/tool_outputs/` | Spawned by orchestrator (or standalone) |
| `@test-designer` | Authors black-box UI acceptance tests (C#/FlaUI at `Src/VerificationTests/`) from the feature files + requirements ALONE, in parallel with the developer; never sees production code | Spawned by orchestrator (or standalone) |
| `@dev-evaluator` | Quality gate: runs ALL tests (developer's + test-designer's), judges test quality (coverage + asserts), verifies the developer's stored tool outputs (re-running only when in doubt), reviews code quality, architecture adherence, functional completeness, demo readiness. Authors no tests. | Spawned by orchestrator (or standalone) |
| `@feature-demonstrator` | Narrated feature showcase: launches application, walks user through the feature with video recording, hands control to user for approval | Spawned by orchestrator after evaluator PASS |
| `@docs-lookup` | Fetches API documentation for .NET libraries from trusted sources | Spawned by developer or architect |
| `@explore` | Fast read-only codebase exploration and Q&A | User invokes directly; agents read files inline |

### How `@orchestrator` works

The user invokes `@orchestrator` once with their request. The orchestrator:
1. Triages the work (categorises, prioritises)
2. **Spawns `@analyst`** → clarifies ambiguities with the user, reads input artifacts, produces structured requirements
3. **Spawns `@sw-architect`** → produces ADRs, diagrams, interface contracts
4. **Spawns `@product-owner`** → creates vertically-sliced backlog + Gherkin specs
5. **[GATE: user reviews requirements + architecture + backlog as a single package]** → if rejected, re-enters at the highest changed layer (requirements → architecture → backlog) and cascades downward, then always re-presents the combined gate (max 3 iterations)
6. For each task (in dependency order):
   - **Spawns `@developer` (design) and `@test-designer` (test design) in parallel** → the developer authors the design (`SDD`/`MVP`/`MVProcedure`); the test-designer authors the system-level verification approach (`VerificationPlan`/`VerificationProcedure`) — no code yet (fresh, isolated contexts; disjoint QMS docs)
   - **[GATE: user reviews the design + test design]** → if rejected, re-spawns the named agent(s) in rework mode and re-presents the gate (own budget, max 3 iterations; does NOT consume the implementation retry budget)
   - **Spawns `@developer` (impl) and `@test-designer` (test authoring) in parallel** → the developer implements with BDD outside-in against the approved design (production code + unit/module tests, storing tool outputs); the test-designer authors black-box UI acceptance tests from the spec alone (fresh, isolated contexts; disjoint files)
   - **Spawns `@dev-evaluator`** once both complete → runs all tests, judges test quality, verifies stored tool outputs (re-running only when in doubt), reviews code quality, architecture, completeness, demo readiness (fresh context, no developer/test-designer memory)
   - If `type: feature`: **Spawns `@feature-demonstrator`** → feature showcase against live application (no source code access) → **[GATE: user approves demo]**
   - If FAIL: re-spawns per the evaluator's `rework_target` — `@developer` (production/dev-test defects) and/or `@test-designer` (verification-test defects) — then re-evaluates (max 3 retries, shared budget)
   - If a `DEFINITION GAP` is signalled (developer, test-designer, or evaluator finds the spec itself wrong): pauses the task, re-enters the definition agents per the cascade, re-presents the combined gate, then resumes — does NOT consume the retry budget
7. Reports completion

**Context isolation is the key mechanism** — developer, test-designer, evaluator, and demonstrator each work in their own context. The same developer/test-designer are spawned twice per task (design mode, then implementation mode), each with a fresh context that orients from the approved design artifacts. The test-designer authors tests without ever seeing the production code, and the evaluator and demonstrator never share context with the developer, ensuring honest assessment.

**The user intervenes at three gates (combined review, per-task design & test-design review, and feature demo) and if a task fails 3×.**

## Project Map

See [docs/harness/project-map.md](../docs/harness/project-map.md) for the full folder structure reference.

## Session Protocol

Each agent works in **one mode per spawn**. The mode determines the lifecycle — there is no single linear "coding session". The authoritative, step-by-step instructions live in each agent's prompt under `.github/agents/`; the summary below shows which lifecycle applies to which mode.

**Common first step (every mode):** Orient — read `.harness/progress.md`, the active backlog in `.harness/backlogs/`, the task's feature file, and the relevant ADRs.

| Mode | Spawned as | Lifecycle (after Orient) |
|------|-----------|--------------------------|
| **Design** | `@developer` (DESIGN) | Author the design QMS docs (`SDD`/`MVP`/`MVProcedure`) against the ADRs. **No source reads, no build, no test, no code** — design only. Log a DESIGN entry to progress.md. |
| **Test-design** | `@test-designer` (TEST-DESIGN) | Author the system-level verification approach (`VerificationPlan`/`VerificationProcedure`) from the spec alone. No production-code access. Log a TEST-DESIGN entry. |
| **Implementation** | `@developer` (IMPL) | Verify baseline → BDD outside-in: step definitions (Red) → production code (Green) → refactor within the ADR architecture (feature files are immutable). Run local quality gates, store tool outputs, log an entry. |
| **Test-authoring** | `@test-designer` (AUTHORING) | Write black-box FlaUI/NUnit acceptance tests in `Src/VerificationTests/` from the feature files alone — never reads production code. Build the test solution, store tool outputs, log an entry. |
| **Evaluation** | `@dev-evaluator` | Run ALL tests, judge test quality, verify stored tool outputs, review code/architecture/completeness/demo readiness. Authors no tests. Write the verdict to `.harness/eval_feedback/` and `docs/qms/MVReport.md`, log an entry. |

**Local quality gates (implementation & test-authoring modes only):**
```
Build\Run-QualityGate.cmd -OutputDir .harness\tool_outputs\{slug}_{task-id}
```
The canonical gate scripts live under `Build/` (they auto-resolve the `*Impl.sln`, bake in the dotCover filters, and store all tool outputs): `Verify-Baseline.cmd` (developer baseline), `Run-QualityGate.cmd` (developer full gate), `Build-VerificationTests.cmd` (test-designer), `Run-CombinedCoverage.cmd` (evaluator). Agents call these instead of hand-assembling `dotnet`/`jb`/`dotcover` commands. All tests must pass; zero ReSharper errors (warnings are informational).

**Update state (every mode):** Append to `.harness/progress.md` using the role-appropriate template before completing — it is the single source of truth for pipeline state. Only the orchestrator marks a task `complete`, after all gates pass (evaluator + demo + CI).

## Critical Rules

- **One task per session.** Do not attempt multiple tasks in a single context.
- **Never remove items from backlog files.** Only change status from `pending` to `in_progress` to `complete`.
- **No placeholder implementations.** Every function must be fully implemented. No `TODO`, `throw new NotImplementedException()`, or stub code.
- **Verify before building.** Always confirm existing functionality works before touching new code.
- **Follow the ADR.** Architectural decisions are binding — do not deviate from patterns defined by `@sw-architect`.
- **New/modified code only.** Apply coding, design, and naming conventions from skills only to new or modified code. Do not retrofit conventions onto untouched existing code unless the task explicitly requires it.
- **Document decisions.** If you make an architectural choice not covered by an ADR, record it in progress.md and flag it for `@sw-architect`.

## Verification Command

The implementation solution is always named `Src\{repo}Impl.sln` where `{repo}` is the repository name (e.g. `CT_Device` → `Src\CT_DeviceImpl.sln`). The `Build/` gate scripts auto-resolve it; to verify manually, find the actual `.sln` under `Src/` and use it.

```
Build\Verify-Baseline.cmd
```

This runs `dotnet build` + `dotnet test --no-build` against the resolved `*Impl.sln`. The verification MUST pass before and after implementation.

## Technology Stack

- **Language:** C# (.NET)
- **Build:** MSBuild / dotnet CLI
- **BDD Framework:** Reqnroll (SpecFlow successor)
- **Testing:** MSTest / NUnit (project-specific) + Reqnroll scenarios
- **UI Automation:** FlaUI (WPF/WinForms) — used by `@feature-demonstrator`
- **CI:** Azure DevOps Pipelines
- **Packaging:** NuGet, MSI (WiX)
- **Document Processing:** pandoc, python-docx, pypdf, openpyxl (for analyst artifact ingestion and QMS doc generation)

## Where to Find More

- **Project map:** `docs/harness/project-map.md`
- Backlog JSON schema: `.harness/backlogs/backlog-schema.json`
- Backlog template: `.harness/backlogs/_template.json`
