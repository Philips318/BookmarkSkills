# Harness_Enhance — Unified End-to-End AI Delivery Harness (with worked example)

A ready-to-use **AI development harness** for CT medical-device software (IEC 62304 / ISO 14971 / ISO 13485),
for VS Code + GitHub Copilot. It merges two complementary approaches into one autonomous, trustworthy, and
compliant end-to-end delivery system:

| Half | Strength |
|------|----------|
| **Execution Engine** | Autonomous long-running loop, context-isolated generator+evaluator, BDD + black-box UI tests, living QMS, 3 human gates, parallel execution |
| **Domain & Compliance Intelligence** | DFMEA / IFU / impact / product-defect analysis, CT·DICOM·Spectral knowledge, TICS & CodeScene remediation, self-contained HTML reporting |

> This package **includes a worked example** — the `WW/WC Reset` feature — so you can see exactly what a full
> end-to-end run produces. To start your own work from a clean slate, see **"Reset to a clean slate"** below.

## What's inside

```
Harness_Enhance/
├─ .github/                      # The harness (18 agents · 31 skills · 12 prompts · 5 instructions)
│  ├─ copilot-instructions.md    #   unified always-loaded core
│  ├─ ASSET_CATALOG.md           #   asset → pipeline-stage map
│  ├─ agents/ skills/ prompts/ instructions/ scripts/
├─ Build/                        # Batch quality-gate scripts (auto-resolve *Impl.sln)
├─ .harness/                     # State store — POPULATED by the example run:
│  │                             #   backlogs/ww-wc-reset.json, requirements/, architecture/adr+diagrams/,
│  │                             #   specs/ (Gherkin), eval_feedback/, tool_outputs/, demo_evidence/, progress.md
├─ docs/
│  ├─ harness/                   # Project-map reference
│  ├─ qms/                       # Living IEC 62304 QMS docs — POPULATED by the example
│  └─ qms-templates/skeletons/   # Clean skeletons (use these to reset qms/)
├─ ExtInf/  Src/                 # The worked EXAMPLE: WW/WC Reset production code, tests, WPF host
└─ .vscode/                      # Subagent invocation settings
```

Asset totals: **18 agents · 31 skills · 12 prompts · 5 instructions.**

### The worked example — WW/WC Reset

A greenfield WPF/MVVM CT image-display feature (reset Window Width/Center to DICOM defaults, IEC 62304 Class B),
built end-to-end by the harness across 4 tasks / 17 BDD scenarios, ending at **85 unit + 17 module tests green**.
Explore it to learn the flow:

- Requirements: `.harness/requirements/ww-wc-reset-requirements.md`
- Architecture: `.harness/architecture/adr/ADR-001-ww-wc-reset-design.md` (+ diagrams)
- Backlog + specs: `.harness/backlogs/ww-wc-reset.json`, `.harness/specs/ww-wc-reset/`
- Evaluator verdicts: `.harness/eval_feedback/`
- QMS: `docs/qms/` (SwRS … MVReport)
- Session narrative (every stage + gate): `.harness/progress.md`
- Code + tests + WPF host: `ExtInf/ImageDisplay/`, `Src/`

## Quick start (your own feature)

1. Open this folder in VS Code with GitHub Copilot (agent mode) enabled.
2. Open Copilot Chat and invoke **`@orchestrator`** with your feature / bug / improvement request.
3. Approve at the **3 human gates**: (1) requirements + architecture + backlog + risk, (2) design + test design, (3) feature demo.
4. The harness implements, evaluates (isolated), self-heals quality, generates risk/compliance docs, and
   demonstrates each task autonomously — you only re-intervene if a task fails 3×.

For one-off tasks, use the slash prompts (e.g. `/full-pipeline`, `/dfmea-analysis`, `/code-quality`, `/tics-preflight`).

## Reset to a clean slate

To remove the example and start fresh:

```powershell
# from the harness root
Remove-Item Src, ExtInf -Recurse -Force
Get-ChildItem .harness\requirements, .harness\architecture\adr, .harness\architecture\diagrams, `
  .harness\specs, .harness\eval_feedback, .harness\tool_outputs, .harness\demo_evidence -Recurse -File |
  Where-Object { $_.Name -ne 'README.md' } | Remove-Item -Force
Remove-Item .harness\backlogs\ww-wc-reset.json -Force -EA SilentlyContinue
Get-ChildItem docs\qms-templates\skeletons\*.skeleton.md | ForEach-Object {
  Copy-Item $_.FullName ("docs\qms\" + ($_.Name -replace '\.skeleton\.md$','.md')) -Force }
```

(Then clear the session entries in `.harness/progress.md`, keeping the Templates section.)
A pre-cleaned template with no example is also available as **`Harness_Enhance_Dist`**.

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

The harness lives at the **root** of the repository it operates on. Either open this folder directly, or copy
`.github/`, `.harness/`, `Build/`, `docs/`, and `.vscode/` into a target repository's root (it operates on that
repo's `Src/`). Then **Reload Window** so Copilot re-scans the customizations.

## Customize

- Ensure your solution follows the `Src\{repo}Impl.sln` naming convention (the `Build/` gate scripts auto-resolve it).
- If not C#/.NET, update the technology stack in `.github/copilot-instructions.md` and the `source-code` / `build-ci` instructions.
- Record project-specific facts in `.harness/` or `/memories/repo/` — keep skills team-portable.

## Prerequisites (for full end-to-end runs)

.NET SDK / `dotnet` CLI · ReSharper CLI (`jb`) · `dotnet dotcover` (coverage) · (optional) `ffmpeg` (demo video) ·
Python (QMS md→docx export).
