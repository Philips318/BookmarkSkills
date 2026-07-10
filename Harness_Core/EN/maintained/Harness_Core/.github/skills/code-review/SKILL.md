---
name: code-review
description: 'Comprehensive code review for commit readiness. Analyzes changed files, fix completeness, feature completeness, regression risk, test impact, coding standard compliance (TICS/CodeScene), architecture conformance, and generates commit logs.'
argument-hint: 'Provide the commit scope: branch name, changed files, ticket/issue ID, or a description of the change'
user-invocable: true
---

# Code Review

Use this skill to perform a structured, multi-dimensional code review before committing or submitting a PR. It produces an actionable review report covering eight review dimensions.

## When to Use

- You are about to commit code and want a comprehensive self-review.
- You are reviewing someone else's change before approval.
- You need a structured pre-commit or pre-PR quality gate.
- You want to generate a well-formed commit message for the change.

## Preferred Inputs

Provide one or more of the following:

- The branch name or diff scope (e.g., `git diff HEAD~1`)
- The list of changed files
- The ticket/issue ID or description of the bug fix / feature
- The architectural context or design intent

If no explicit input is provided, the skill will detect changed files via `git status` and `git diff`.

## Review Dimensions

This skill reviews along **eight dimensions**, each producing a section in the final report.

### Domain Accuracy (cross-cutting, applied across all dimensions)

For any change touching CT/DICOM/Spectral/ISP code, also consult the `domain-knowledge` skill:

- **dicom-patterns.md** — verify correct VR types, tag IDs, transfer syntax for context, MONO1 vs MONO2 handling, SliceThickness vs SpacingBetweenSlices, RescaleSlope/Intercept usage
- **spectral-knowledge.md** — verify SBI version gates, MonoE keV range (40-200), max 4 concurrent MonoE, non-HU RGB rule
- **safety-rules.md** — escalate any code touching dose, patient ID, radiation interlock, spectral quantification to Critical severity if safety controls are missing
- **ct-glossary.md** — verify correct units, value ranges, and use of formal feature names

Report domain-accuracy issues under the dimension where they manifest (usually Regression Risk or Feature Completeness).

### 1. Change Scope — Files & Impact Map

Identify all files touched by the change, categorize them, and map the dependency graph.

See [review-checklist.md § Change Scope](./references/review-checklist.md#1-change-scope).

### 2. Fix Completeness

For bug-fix commits: verify the root cause is addressed, edge cases are covered, and the fix does not leave partial states.

See [review-checklist.md § Fix Completeness](./references/review-checklist.md#2-fix-completeness).

### 3. Feature Completeness

For feature commits: verify all acceptance criteria are met, UI/UX flows are complete, and integration points are wired.

See [review-checklist.md § Feature Completeness](./references/review-checklist.md#3-feature-completeness).

### 4. Regression Risk — New Issues

Analyze whether the change introduces new risks: null-safety, thread-safety, resource leaks, behavioral side effects, or API contract breakage.

See [review-checklist.md § Regression Risk](./references/review-checklist.md#4-regression-risk).

### 5. Test Impact — Affected Features

Map the change to affected functional areas and recommend focused testing targets.

See [review-checklist.md § Test Impact](./references/review-checklist.md#5-test-impact).

### 6. Coding Standards — TICS & CodeScene

Run a combined TICS + CodeScene preflight check on the changed files.

See [review-checklist.md § Coding Standards](./references/review-checklist.md#6-coding-standards).

### 7. Architecture Conformance

Verify the change follows the project's layering, dependency, and design rules.

See [review-checklist.md § Architecture Conformance](./references/review-checklist.md#7-architecture-conformance) and [architecture-rules.md](./references/architecture-rules.md).

**Architecture Drift sub-check (required when `artifacts/<feature>/03-implementation-log.md` contains a "Post-implementation patches" or "Resolved post-implementation" section):** diff the post-patch implementation against the interface contracts and component responsibilities documented in `02-architecture.md`. Flag as Warning when any of these drift signals appear:

- A parameter declared in an interface signature has become **dead** (unused in every implementation) after the patch — the contract no longer matches the design intent.
- A field, property, or DI dependency declared in the architecture is **silently dropped** by the implementation.
- A responsibility documented in the architecture (e.g., "Provider applies redaction") was **moved to a different component** during the patch without an ADR or implementation-log note.
- A non-trivial behavior was **added** during the post-patch (e.g., "force entry-assembly inclusion") that is not reflected in the architecture document — request an architecture document update in the same PR.

This sub-check exists because mid-implementation Q-A/Q-B answer rounds frequently leave architectural drift that no single dimension would otherwise catch. The Reviewer is the last line of defense before the drift becomes permanent.

### 8. Commit Log Generation

Generate a conventional, well-structured commit message summarizing the change.

See [review-checklist.md § Commit Log](./references/review-checklist.md#8-commit-log) and [commit-log-template.md](./references/commit-log-template.md).

## Workflow

1. **Gather context**: Identify changed files via `git diff --name-only` and `git status`. Read the diff content.
2. **Understand intent**: Determine whether the change is a bug fix, feature, refactoring, or mixed. Use the ticket/issue description if provided.
3. **Run each dimension**: Walk through all eight review dimensions sequentially. For each dimension:
   a. Analyze the changed code against the dimension's checklist.
   b. Record findings as Pass / Warning / Fail with concrete evidence (file, line, reason).
   c. Assign a numeric score: Pass = 10, Warning = 5, Fail = 0.
4. **Coding standards deep-dive**: For Dimension 6, apply rules from:
   - [TICS instructions](../../instructions/tics-csharp.instructions.md)
   - [CodeScene instructions](../../instructions/codescene-csharp.instructions.md)
   - [TICS common rules](../tics-standard/references/common-rules.md) (if available)
   - [CodeScene common issues](../codescene-health/references/common-issues.md) (if available)
5. **Generate commit log**: Based on the review findings, produce a commit message following the template.
6. **Calculate overall verdict**: Compute overall status from individual dimension verdicts:
   - **Overall Pass**: All dimensions Pass (no Fail, no Warning)
   - **Overall Warning**: At least one Warning, no Fail
   - **Overall Fail**: At least one Fail
7. **Build HTML report**: Fill the [report template](./references/report-template.html) with computed data, including:
   - Hero banner with overall verdict badge (Pass / Warning / Fail) and metadata pills
   - **Root Cause 总结** (for fix changes): Root cause analysis paragraphs + one-liner summary
   - **Feature 交付总结** (for feature changes): Feature delivery checklist with ✓/✗ per feature point
   - 核心发现 overview paragraph
   - Radar chart SVG with 8-axis polygon showing dimension scores
   - Summary table with all 8 dimensions and status badges
   - 各维度详细结论 with card grids, diff snippets, finding blocks per dimension
   - Changed files grid cards with categories and impact
   - Test plan section with P0/P1/P2 (ordered lists)
   - Generated commit message block with copy button
   - Required actions before commit (Must-Fix / Should-Fix)
   - Residual risks list
8. **Save the HTML file**: Write to `Code Review Report/{project_or_branch}_review_report.html` under the workspace root (create the `Code Review Report` directory if needed), unless the user explicitly specifies another location.

## Default Priority Policy

- **Must-fix before commit**: Any Fail finding in Dimensions 2 (Fix Completeness), 3 (Feature Completeness), or 4 (Regression Risk — Critical severity).
- **Should-fix before commit**: Any Warning in Dimensions 4, 6, or 7.
- **Recommended**: Findings in Dimension 5 (test recommendations) and Dimension 8 (commit log polish).

## Guardrails

- Do not auto-fix code unless the user explicitly requests it. Default mode is review-only.
- Do not modify files outside the current change scope.
- Do not suppress or hide findings to produce a "clean" report.
- Flag any uncertainty — if a dimension cannot be fully evaluated (e.g., no test coverage data), state this explicitly.
- This skill is a pre-check. The final authority remains the team's CI pipeline, TICS server, and CodeScene PR review.

## Expected Output

A **self-contained HTML report** (light-theme, no external dependencies) that can be opened in any browser, containing:

Default save location: workspace root `Code Review Report/` directory, with filename `{project_or_branch}_review_report.html` unless the user explicitly requests another path.

1. **Hero banner**: Gradient header with review scope, metadata pills, and overall verdict badge
2. **Root Cause 总结** (fix changes only): Dedicated section with root cause analysis paragraphs and a highlighted one-liner summary box. Omit this section for feature changes.
3. **Feature 交付总结** (feature changes only): Dedicated section with a checklist of delivered features (✓ completed / ✗ incomplete) and a summary note. Omit this section for fix changes.
4. **核心发现**: Brief overview paragraph of the overall review status
5. **Summary Table**: # | 维度 | 状态 (Pass/Warning/Fail badge) | 结论
6. **维度雷达图**: SVG polygon visualizing scores across all 8 dimensions
7. **各维度详细结论**: Per-dimension subsections with card grids, diff code blocks, and finding lists
8. **测试计划**: P0/P1/P2 ordered lists
9. **建议 Commit Message**: Pre-formatted commit message with copy button
10. **提交前必须修复**: Must-Fix / Should-Fix action items
11. **Residual Risks**: Items that cannot be verified locally

The report uses the [report template](./references/report-template.html) and must be fully self-contained (inline CSS, inline SVG, inline JS). No external resources.

### Scoring Convention

| Dimension Status | Numeric Score | Color |
|-----------------|---------------|-------|
| Pass | 10 | Green (#22c55e) |
| Warning | 5 | Yellow (#eab308) |
| Fail | 0 | Red (#ef4444) |
| N/A | — (excluded) | Gray (#94a3b8) |

## Related Skills

- [TICS Standard](../tics-standard/SKILL.md) — for deep TICS rule analysis
- [CodeScene Code Health](../codescene-health/SKILL.md) — for deep Code Health analysis

## Important Rules

- **Review-only by default** — do not modify any source files unless the user explicitly requests auto-fix.
- Fill the HTML template with actual computed data — do not output a template with placeholders.
- The radar chart must be a proper SVG polygon computed from actual dimension scores, not a placeholder.
- Use inline SVG, CSS, and JS — the report must be fully self-contained.
- If a dimension cannot be fully evaluated (e.g., no test coverage data available), mark it N/A and exclude from the radar chart polygon.
- Always include the generated commit message in the report even if the user did not specifically ask for one.
- **`Severity rationale:` is a required field on every Warning and Fail finding** (not optional, not a convention). A finding without a one-line rationale explaining *why* the severity is Warning rather than Fail (or Fail rather than Warning) is treated as an incomplete review and blocks the verdict from being final. This rule was promoted from convention to requirement after the PipelineEnvironmentInfoLogger dogfood demonstrated that the rationale field cut Should-Fix loop time by an order of magnitude — it lets the Developer triage `fix-now` vs `defer` without re-asking the Reviewer.

## References

- [Review checklist](./references/review-checklist.md)
- [Architecture rules](./references/architecture-rules.md)
- [Commit log template](./references/commit-log-template.md)
- [Report template](./references/report-template.html)

## Portability Note

This skill is **team-portable**. The 8 review dimensions, Pass/Warning/Fail scoring, HTML report template, and commit-log convention are generic and apply to any C# project using TICS + CodeScene. Domain-accuracy checks delegate to the `domain-knowledge` skill which each team customizes for their product line — no edits to this file are needed. Team-specific architecture rules belong in [`references/architecture-rules.md`](./references/architecture-rules.md) (versioned per repo) or `/memories/repo/`.
