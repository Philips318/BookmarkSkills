---
name: "orient"
description: "Session orientation checklist — run at the start of every developer or evaluator session to establish context before touching any code."
---

# Session Orientation Checklist

Complete ALL steps before implementing or evaluating anything. Do not skip steps even if you think you know the context.

## 1. Read Harness State

- [ ] Read `.harness/progress.md` — what was done last session, any blockers or unresolved decisions
- [ ] Read the active backlog file in `.harness/backlogs/` — which tasks are `pending`, `in_progress`, `complete`, or `blocked`

## 2. Identify and Understand Your Task

- [ ] Identify the task to work on (specified by orchestrator, or highest-priority `pending` with no unmet `depends_on`)
- [ ] Read the task's `spec_file` (Gherkin feature file) — understand exactly which scenarios must pass
- [ ] Read the task's `design_note` — understand the architectural constraints; these are binding, not suggestions
- [ ] Read the ADR file(s) referenced in `design_note` from `.harness/architecture/adr/`

## 3. Verify Baseline

Run the project verification command:

```
Build\Verify-Baseline.cmd
```

This canonical script auto-resolves the `*Impl.sln` under `Src/` and runs `dotnet build` + `dotnet test --no-build`.

- [ ] Build passes with no errors
- [ ] All existing tests pass (zero pre-existing regressions)

**If baseline fails:** fix the existing issue first, then proceed.

## 4. Confirm Readiness

- [ ] I know exactly which task I am implementing or evaluating
- [ ] I have read and understood the Gherkin spec
- [ ] I have read and understood the architectural guidance (design_note + ADRs)
- [ ] The baseline is green

**Only proceed after all checkboxes are satisfied.**
