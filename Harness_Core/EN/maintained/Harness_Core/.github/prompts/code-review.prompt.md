---
name: Code Review
description: 'Comprehensive pre-commit code review with HTML report output. Analyzes change scope, fix/feature completeness, regression risk, test impact, TICS/CodeScene compliance, architecture conformance, and generates commit logs.'
argument-hint: 'Optional: branch, ticket ID, or description of the change'
agent: 'agent'
---

Perform a comprehensive code review on the current workspace changes and generate a **self-contained HTML report**.

Use these repository assets as the source of policy, rules, and report format:

- [Review checklist](../skills/code-review/references/review-checklist.md)
- [Architecture rules](../skills/code-review/references/architecture-rules.md)
- [Commit log template](../skills/code-review/references/commit-log-template.md)
- [Report template](../skills/code-review/references/report-template.html)
- [TICS instructions](../instructions/tics-csharp.instructions.md)
- [CodeScene instructions](../instructions/codescene-csharp.instructions.md)
- [TICS common rules](../skills/tics-standard/references/common-rules.md)
- [CodeScene common issues](../skills/codescene-health/references/common-issues.md)

## Workflow

1. **Detect changes**: Run `git diff --name-only` and `git status` to identify all changed files. If a branch or scope is specified, use that.
2. **Read diffs**: Read the actual diff content for each changed file. Count additions and deletions.
3. **Classify change type**: Determine if this is a fix, feat, refactor, chore, or mixed change.
4. **Run all 8 review dimensions** from the review checklist:
   - §1 Change Scope — list and categorize all files
   - §2 Fix Completeness — verify root cause is addressed (for fixes)
   - §3 Feature Completeness — verify all requirements met (for features)
   - §4 Regression Risk — check for new null/thread/resource/API risks
   - §5 Test Impact — map affected features and recommend test plan
   - §6 Coding Standards — run TICS + CodeScene preflight on changed `.cs` files
   - §7 Architecture Conformance — check layering, dependencies, design rules
   - §8 Commit Log — generate a ready-to-use commit message
5. **Score each dimension**: Assign Pass (10) / Warning (5) / Fail (0) / N/A.
6. **Calculate overall verdict**:
   - **Pass**: All dimensions Pass (no Fail, no Warning)
   - **Warning**: At least one Warning, no Fail
   - **Fail**: At least one Fail
7. **Build the HTML report** using the [report template](../skills/code-review/references/report-template.html):
   - Fill all `{{PLACEHOLDER}}` values with actual computed data
   - Build a proper SVG radar chart polygon from the 8 dimension scores
   - **For fix changes**: Fill the "Root Cause 总结" section with root cause analysis paragraphs and one-liner summary; omit "Feature 交付总结" section
   - **For feature changes**: Fill the "Feature 交付总结" section with a ✓/✗ checklist of delivered features; omit "Root Cause 总结" section
   - Populate the summary table, per-dimension detailed conclusions, changed files cards
   - Include key diff snippets in dark-themed `<pre>` blocks with colored diff lines
   - Generate the test plan, commit message, required actions, and residual risks sections
   - Ensure the report is fully self-contained (inline CSS, inline SVG, inline JS)
8. **Save the HTML file**: Write to `Code Review Report/{project_or_branch}_review_report.html` under the workspace root (create the `Code Review Report` directory if needed), unless the user explicitly specifies another location.

## Output

A single self-contained HTML file (light theme) that can be opened in any browser, with:
- Default save location: workspace root `Code Review Report/` directory, with filename `{project_or_branch}_review_report.html` unless the user explicitly requests another path.
- Hero banner with gradient header, metadata pills, and overall verdict badge
- **Root Cause 总结** (fix changes only): Root cause analysis + one-liner summary box
- **Feature 交付总结** (feature changes only): Delivered features checklist with ✓/✗ status
- 核心发现 overview paragraph
- Summary table with statuses and conclusions per dimension
- Radar chart visualizing 8 dimension scores
- 各维度详细结论 with card grids, diff code snippets, and finding lists
- Prioritized test plan (P0 / P1 / P2 ordered lists)
- Generated commit message (with copy button)
- 提交前必须修复 (Must-Fix / Should-Fix action items)
- Residual Risks

## Rules

- Default to **review-only** — do not edit source files unless explicitly asked.
- Do not skip any of the 8 dimensions. Mark inapplicable dimensions as "N/A" with reason.
- Be specific — cite file names, line numbers, and concrete evidence.
- Fill the HTML template with actual computed data — do not output a template with placeholders.
- The radar chart must be a proper SVG polygon, not a placeholder.
- Use inline SVG in the HTML — no external dependencies. The report must be fully self-contained.
- Flag uncertainty explicitly.
