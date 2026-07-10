---
name: backlog-grooming
description: Creating, reviewing, or refining backlog task definitions; writing acceptance criteria; applying INVEST principles; vertically slicing features; setting demo_required flags and coverage thresholds on backlog items.
---

# Backlog Grooming Skill

## INVEST Story Quality Criteria

| Criterion | Check |
|-----------|-------|
| **I**ndependent | Can this task be implemented without starting another simultaneously? |
| **N**egotiable | Are the implementation details open (bounded by ADR, not by the story)? |
| **V**aluable | Would an operator or the system behave observably differently after this task? |
| **E**stimable | Can a developer confidently complete this in one focused session? |
| **S**mall | Does it fit in a single developer session, not multiple days? |
| **T**estable | Does it have a `demo_scenario` (feature) or measurable `coverage_threshold` (infrastructure)? |

## Vertical Slice Validation

For every `type: feature` task, verify:

1. What does the operator **SEE** or **DO** differently after this task is complete?
2. Which architectural layers are exercised? (must be ≥ 2 layers)
3. Can this be demonstrated on a running application in < 2 minutes? (must be YES)
4. Does `demo_scenario` name an exact `Scenario:` from the feature file? (must match verbatim)

If any answer is wrong → **reject and reshape the task** before it enters the backlog.

## Infrastructure Task Validation

Infrastructure tasks must:
- Provide a written justification for why they cannot be a vertical slice
- Have `coverage_threshold: 90` or higher
- Not exceed 20% of total task count in the backlog

If infrastructure tasks exceed 20% of the backlog: the product owner is deferring integration. Push back and reshape.

## Task Size Check

Each task is implemented by `@developer` in **one context window**. An oversized task makes the developer run out of context and exit without a handover — losing all its work. Err on the side of smaller.

A well-sized task:
- Has 3–5 Gherkin scenarios in its feature file (treat 5 as a soft ceiling, not a target)
- Touches 2–5 files in `files_likely_affected`
- Has ≤ 2 direct `depends_on` entries
- Is completable in a single developer session (roughly 1–3 hours)

If a task has > 5 scenarios or > 5 files affected → split it. When genuinely unsure whether a task is too big, **split it** — two small demonstrable slices sequenced with `depends_on` are always safer than one oversized task.

When splitting:
- Each split task must still independently satisfy the Vertical Slicing Rule (user-visible, ≥ 2 layers, < 2 min demo, own `demo_scenario`).
- Do NOT pad a task with unrelated work "while we're in there." One task = one coherent behaviour.
- Sequence the parts with `depends_on` so each builds on the last.

## Dependency Validation

Check all `depends_on` entries:
- Does each referenced task ID exist in this backlog?
- Is there a circular dependency? (A → B → A)
- Are infrastructure tasks front-loaded such that all feature tasks that need them list them in `depends_on`?

## `design_note` Quality Check

Every task's `design_note` must:
- Name at least one specific ADR file (e.g. "See ADR-002 for IPositionPublisher pattern")
- State the architectural pattern or interface to use
- Not be a generic instruction like "follow good practices"

If `design_note` is missing or generic → return to `@sw-architect` for clarification before proceeding.
