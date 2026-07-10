---
applyTo: ".harness/**"
---

# Harness State Files Instructions

When working with harness state files (backlogs, progress, feedback, requirements, architecture):

## Backlog Files (`.harness/backlogs/*.json`)

- Never remove or reorder tasks
- Only change `status` field: `pending` → `in_progress` → `complete`
- When changing status to `in_progress`, set `started_at` to current ISO 8601 timestamp
- When orchestrator marks `complete`, set `completed_at` to current ISO 8601 timestamp
- Always update the backlog-level `updated` timestamp on every modification
- Never modify `acceptance_criteria` after execution has started (discuss with user first)
- Use the schema in `.harness/backlogs/backlog-schema.json` for validation
- Copy `.harness/backlogs/_template.json` when starting a new backlog
- `priority` determines processing order across multiple backlogs (1 = highest)

## Requirements Documents (`.harness/requirements/`)

- Files are named `{slug}-requirements.md`
- Output from `@analyst` only — do not manually edit after analysis is complete
- `FR-XX` and `NFR-XX` IDs must remain stable — `@dev-evaluator` traces implementation back to these
- Include a YAML frontmatter block with `created` and `updated` ISO 8601 timestamps

## Architecture (`.harness/architecture/`)

- ADRs in `adr/` are named `ADR-{NNN}-{short-name}.md`
- Once `Status: Accepted`, ADRs are binding — changing them requires a new ADR that supersedes the old one
- Diagrams in `diagrams/` are Mermaid in `.md` files
- Each ADR must include a `Date:` field in its header (ISO 8601 date)

## Progress Notes (`.harness/progress.md`)

- Always append, never overwrite existing entries
- Use the role-appropriate template format at the bottom of the file
- **All agents must write to progress.md** — this is the single source of truth for pipeline state
- Orchestrator logs phase transitions and gate decisions (including the per-task design & test-design review gate)
- Planning agents (analyst, architect, product-owner) log artifacts produced and open questions
- Task agents (developer, evaluator, demonstrator) log task-scoped activity and outcomes
- Developer and test-designer run **twice per task** — first in design mode (DESIGN / TEST-DESIGN), then in implementation mode; log the mode in each entry so a re-spawn knows which phase it is resuming
- Include: date, agent name, status, what was produced/verified, next steps

## Shared Resources Under Parallelism

When tasks run concurrently (`@orchestrator-parallel`), a few files are touched by **every** task and cannot be partitioned by `files_likely_affected`. Writes to these are **serialized one task at a time** (task-ID order) — a task waits for the current writer to finish before applying its own append/edit:

- `.harness/progress.md` — append-only; serialize appends so no two agents interleave a write
- `Src/VerificationTests/VerificationTests.sln` — only the `<Project>` registration line is shared; each task's own `Src/VerificationTests/{slug}/Task{id}/` project files are private and never serialize
- The shared QMS documents (`docs/qms/SDD.md`, `MVP.md`, `MVProcedure.md`, `VerificationPlan.md`, `VerificationProcedure.md`) — see the qms-documentation skill

Only the **write operation** serializes; the content each task contributes is private to that task and the underlying design/implementation work continues in parallel.

## Eval Feedback (`.harness/eval_feedback/`)

- Files are named `{backlog-slug}_{task-id}.json`
- Output from `@dev-evaluator` only — do not manually edit
- Read them when fixing issues identified by the evaluator
- Each file includes an `evaluated_at` field (ISO 8601) set when the evaluation was performed

## Demo Evidence (`.harness/demo_evidence/`)

- Video recordings stored at `{backlog-slug}/task-{id}-{timestamp}/demo.mp4`
- Demo log JSON at `{backlog-slug}_{task-id}_demo.json`
- Output from `@feature-demonstrator` only — do not manually edit
- Demo logs do NOT contain pass/fail verdicts — the user is the gate
- Each log includes a `demonstrated_at` field (ISO 8601) set when the demo was executed
- Never capture full desktop — video records only the application window

## Tool Outputs (`.harness/tool_outputs/`)

- Files stored at `.harness/tool_outputs/{backlog-slug}_{task-id}/` (developer) and `.harness/tool_outputs/{backlog-slug}_{task-id}_verification/` (test-designer)
- Written by `@developer` and `@test-designer` during their Verify steps so `@dev-evaluator` can trust-but-verify without re-running everything. The two write to separate folders because they run in parallel.
- Developer folder contains: `build.log` (dotnet build), `tests.trx` (dotnet test results), `resharper.xml` (ReSharper CLI report), `coverage.xml` (dotCover report)
- Test-designer `_verification` folder contains: `build.log` and `resharper.xml` for `VerificationTests.sln` (test code is held to the same build-clean / zero-ReSharper-error bar as production code)
- `@dev-evaluator` writes its combined coverage report to `.harness/tool_outputs/{backlog-slug}_{task-id}_eval/coverage-combined.xml` (task-scoped so parallel evaluations never collide)
- `@dev-evaluator` reads these and re-runs a tool only when an output is missing, stale, borderline, or implausible
- These are runtime artefacts — do not hand-edit

## Verification Tests (`Src/VerificationTests/`)

- Independent C# FlaUI test projects authored by `@test-designer`
- Stored at `Src/VerificationTests/{slug}/Task{id}/` — one NUnit test project per task
- Solution: `Src/VerificationTests/VerificationTests.sln`
- The `@test-designer` writes these from the feature files + requirements ALONE, in parallel with the developer and without ever seeing the production code
- `@dev-evaluator` runs these tests (it does not author them) and judges their quality
- Always C# + FlaUI + NUnit — no PowerShell scripts
- All tests have `[Category("Verification")]` attribute
- Run against the live application in simulator mode (not mocks)
- Reusable in CI pipelines — combined coverage = developer tests + verification tests
- Do NOT store verification tests in `.harness/` — they are formal repo assets
