# CT AI Development Harness

A structured workflow for autonomous AI-driven medical device software development using **GitHub Copilot** in VS Code. Implements the generator + evaluator pattern with IEC 62304 compliance, QMS documentation, and multi-gate quality control.

Inspired by [Anthropic's harness research](https://www.anthropic.com/engineering/harness-design-long-running-apps).

## Quick Start

1. Open this repository in VS Code with GitHub Copilot enabled
2. Open Copilot Chat (`Ctrl+Shift+I`)
3. Invoke `@orchestrator` with your feature request, bug report, or improvement idea
4. Approve at three gates: **combined review** (requirements + architecture + backlog), **per-task design & test-design review**, and **feature demo**
5. The harness implements, evaluates, and demonstrates each task autonomously
6. You only intervene again if a task **fails 3×**

### Example

```
@orchestrator Add exponential backoff retry logic to the device communication layer.
              Should handle transient network failures with configurable max retries.
```

The harness will: triage → analyse → design architecture → create backlog + Gherkin specs → **[you approve the package]** → implement each task with BDD → evaluate → demo (if feature) → **[you approve demo]** → report.

## Architecture

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

**Key design mechanism:** Each subagent runs in its **own isolated context window**. The test-designer authors its tests from the spec alone without ever seeing the production code, and the evaluator and demonstrator have no memory of the developer's session — this ensures honest, unbiased assessment.

### Standalone Use

Individual agents can be invoked directly for manual control:

| Agent | Standalone Use |
|-------|---------------|
| `@analyst` | Ingest a PRD and produce requirements without running the full pipeline |
| `@sw-architect` | Design architecture for existing requirements |
| `@product-owner` | Create a backlog from existing requirements + ADRs |
| `@product-owner-parallel` | Same, plus file-isolation metadata for concurrent execution |
| `@developer` | Implement a specific task from an existing backlog |
| `@test-designer` | Author black-box FlaUI acceptance tests for a task from the spec alone |
| `@dev-evaluator` | Review a completed task independently |
| `@docs-lookup` | Fetch API documentation for .NET libraries from trusted sources |
| `@explore` | Fast read-only codebase Q&A (user-facing only) |

For end-to-end runs, invoke `@orchestrator` (sequential) or `@orchestrator-parallel` (runs independent tasks concurrently when the backlog carries file-isolation metadata from `@product-owner-parallel`).

## Repository Structure

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

**Created or provided as needed (not shipped with the harness):**

| Path | Origin |
|------|--------|
| `Input PRD/`, `Input Telemetry/` | You drop input artifacts here before invoking `@analyst` |
| `Src/` (incl. `Src/VerificationTests/`) | The target repository's source — production code and developer tests (by `@developer`), plus the `@test-designer`'s independent black-box FlaUI tests in `Src/VerificationTests/`, created during execution |
| `.harness/demo_runners/` | Created on demand by `@feature-demonstrator` to persist demo automation projects |

## Quality Gates

The harness enforces quality at multiple levels:

| Gate | Tool | When | Blocking? |
|------|------|------|-----------|
| Design & test-design review | `@developer` (design) + `@test-designer` (test design) | Per task, before implementation | Yes (user gate) |
| Build + tests | `dotnet build/test` | Every developer session | Yes |
| Code inspection | `jb inspectcode` (ReSharper CLI) | Pre-submit | Yes (zero errors) |
| Black-box acceptance tests | `@test-designer` authors (C#/FlaUI in `Src/VerificationTests/`) | In parallel with developer | Authored, then run by evaluator |
| White-box review + test run | `@dev-evaluator` (runs ALL tests, judges quality) | After implementation | Yes |
| Feature showcase | `@feature-demonstrator` (narrated demo) | After evaluator PASS | Yes (user gate) |
| Combined coverage | Developer tests + verification tests | Evaluator Section 5 | Yes (≥ threshold) |
| Coding standards | TICS (TIOBE) | CI pipeline | Yes (TQI ≥ 8.0) |
| Security scan | Coverity | CI pipeline | Yes (no new High) |

## Design Principles

1. **Context isolation is the key mechanism.** Each subagent gets a fresh context — the test-designer authors tests without seeing production code, and evaluators and demonstrators never see the developer's reasoning.
2. **Separate generation from evaluation.** Agents cannot objectively judge their own work.
3. **One task per session.** Focus prevents context exhaustion.
4. **Verify before building.** Always confirm the baseline is green before implementing.
5. **Structured artifacts bridge sessions.** Backlogs, ADRs, progress notes, and QMS docs are the shared memory.
6. **New/modified code only.** Skills apply conventions to new code — don't retrofit legacy.
7. **The orchestrator owns task status.** Developers don't mark tasks complete; the orchestrator does after all gates pass.
8. **Definition changes cascade down, never up.** Gate rejections and mid-execution definition gaps re-enter at the highest changed layer (requirements → architecture → backlog), revise everything below, freeze approved upstream artifacts, and always re-present the combined gate. A definition gap pauses the task without consuming its retry budget.

## Applying to Another Repository

To use this harness on another CT repository:

1. **Copy these directories into the target repo:**
   - `.github/` (copilot-instructions.md, agents/, instructions/, skills/, prompts/)
   - `.harness/` (backlogs/, requirements/, architecture/, specs/, eval_feedback/, demo_evidence/, progress.md)
   - `.vscode/` (subagent invocation setting)
   - `docs/harness/`
   - `docs/qms/`
   - `docs/qms-templates/`

2. **Customize:**
   - The verification command auto-resolves to `Src\{repo}Impl.sln` — just ensure your `.sln` follows the `{repo}Impl.sln` naming convention
   - Update `copilot-instructions.md` technology stack if not C#/.NET
   - Adjust `source-code.instructions.md` for project-specific conventions
   - Adjust `build-ci.instructions.md` for your CI pipeline variables

3. **Provide input artifacts:**
   - Create `Input PRD/` and drop PRD documents there
   - Create `Input Telemetry/` and drop telemetry data there

4. **Invoke:** `@orchestrator` in Copilot Chat with your request.

## Technology Stack (Default)

| Concern | Tool |
|---------|------|
| Language | C# (.NET) |
| Build | MSBuild / `dotnet` CLI |
| BDD | Reqnroll (SpecFlow successor) |
| Unit tests | NUnit 3.x + NSubstitute |
| UI automation | FlaUI (WPF/WinForms) |
| CI | Azure DevOps Pipelines |
| Packaging | NuGet + WiX MSI |
| QMS compliance | IEC 62304 (Class A/B/C) |
| Code quality | ReSharper CLI, TICS, Coverity |

## References

- [Effective Harnesses for Long-Running Agents](https://www.anthropic.com/engineering/effective-harnesses-for-long-running-agents) — Anthropic, Nov 2025
- [Harness Design for Long-Running Application Development](https://www.anthropic.com/engineering/harness-design-long-running-apps) — Anthropic, Mar 2026
- [celesteanders/harness](https://github.com/celesteanders/harness) — Reference implementation for Claude Code
