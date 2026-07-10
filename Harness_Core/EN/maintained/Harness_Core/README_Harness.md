# Harness_Enhance — Unified End-to-End AI Delivery Harness (Distributable)

A ready-to-use **AI development harness** for CT medical-device software (IEC 62304 / ISO 14971 / ISO 13485),
for VS Code + GitHub Copilot. It merges two complementary approaches into one autonomous, trustworthy, and
compliant end-to-end delivery system:

| Half | Strength |
|------|----------|
| **Execution Engine** | Autonomous long-running loop, context-isolated generator+evaluator, BDD + black-box UI tests, living QMS, 3 human gates, parallel execution |
| **Domain & Compliance Intelligence** | DFMEA / IFU / impact / product-defect analysis, CT·DICOM·Spectral knowledge, TICS & CodeScene remediation, self-contained HTML reporting |

> This is a **clean distributable template** — it contains the harness assets only, with no example project.
> A fresh `.harness/` state store and empty QMS skeletons are included so your first run starts clean.

## What's inside

```
Harness_Enhance/
├─ .github/
│  ├─ copilot-instructions.md   # Unified always-loaded core
│  ├─ ASSET_CATALOG.md          # Full asset → pipeline-stage map
│  ├─ agents/       (18)         # 12 execution + 6 domain specialists
│  ├─ skills/       (31)         # engineering/test/UI + domain/risk/compliance
│  ├─ prompts/      (12)         # slash-command entry points
│  ├─ instructions/ (5)          # path-triggered + always-on TICS/CodeScene
│  └─ scripts/                   # QMS md→docx export (Python)
├─ Build/                        # Batch quality-gate scripts (auto-resolve *Impl.sln)
├─ .harness/                     # State store (empty: backlogs schema + template, empty stage dirs, fresh progress.md)
├─ docs/
│  ├─ harness/                   # Project-map reference
│  ├─ qms/                       # Empty IEC 62304 QMS skeletons (SwRS, SSDS, SDD, MVP, MVProcedure, MVReport)
│  └─ qms-templates/             # PDLM templates + generated skeletons
└─ .vscode/                      # Subagent invocation settings
```

Asset totals: **18 agents · 31 skills · 12 prompts · 5 instructions.**

## Quick start

1. Open this folder in VS Code with GitHub Copilot (agent mode) enabled.
2. Open Copilot Chat and invoke **`@orchestrator`** with your feature / bug / improvement request.
3. Approve at the **3 human gates**: (1) requirements + architecture + backlog + risk, (2) design + test design, (3) feature demo.
4. The harness implements, evaluates (isolated), self-heals quality, generates risk/compliance docs, and
   demonstrates each task autonomously — you only re-intervene if a task fails 3×.

For one-off tasks, use the slash prompts (e.g. `/full-pipeline`, `/dfmea-analysis`, `/code-quality`, `/tics-preflight`).

## Unified pipeline

```
Analyse → Requirements Review + Safety Class → Architect (ADR) + Impact Analysis → Plan (Backlog + Gherkin)
  → ⏸ GATE 1
  → Develop (BDD) ∥ Test-Design (black-box, isolated)
  → ⏸ GATE 2
  → Evaluate (isolated) + Quality Self-Heal (TICS/CodeScene)
  → Risk & Compliance (DFMEA / IFU / QMS) → Documentation (HTML + md→docx)
  → Demonstrate → ⏸ GATE 3 → Done
```

See [`.github/ASSET_CATALOG.md`](./.github/ASSET_CATALOG.md) for the full stage-by-stage asset map.

## Deploy to a project

The harness lives at the **root** of the repository it operates on. Two ways to use it:

- **Try it standalone:** open this folder directly in VS Code — everything is already at the root.
- **Apply to a target repo:** copy `.github/`, `.harness/`, `Build/`, `docs/`, and `.vscode/` into the target
  repository's root (the harness operates on that repo's `Src/`).

Then **Reload Window** so Copilot re-scans the customizations. In Chat, `@` should list the agents and `/`
the prompts.

## Customize

- Ensure your solution follows the `Src\{repo}Impl.sln` naming convention (the `Build/` gate scripts auto-resolve it).
- If not C#/.NET, update the technology stack in `.github/copilot-instructions.md` and the
  `source-code` / `build-ci` instructions.
- Record project-specific facts in `.harness/` or `/memories/repo/` — keep the skills team-portable
  (do not hardcode project specifics into skills).

## Prerequisites (for full end-to-end runs)

.NET SDK / `dotnet` CLI · ReSharper CLI (`jb`) for inspection · `dotnet dotcover` for coverage ·
(optional) `ffmpeg` for demo video · Python (for QMS md→docx export).

## Design principles

1. **Context isolation is the key mechanism** (blind test-design, isolated evaluation) — domain skills inject as read-only criteria and must not break it.
2. **Separate generation from evaluation.** 3. **One task per session; the orchestrator owns status.**
4. **Verify before building.** 5. **Follow the ADR; new/modified code only.** 6. **Safety first** (IEC 62304 class recorded with justification).
7. **Definition changes cascade down, never up.** 8. **Dual-track documentation** — QMS (audit) + HTML reports (management).
