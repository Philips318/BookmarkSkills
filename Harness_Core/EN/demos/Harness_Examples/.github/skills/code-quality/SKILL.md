---
name: code-quality
description: 'Assess code quality of a module or project across 8 dimensions: CodeScene health, TICS compliance, test coverage, architecture conformance, maintainability, security, performance patterns, and defect density. Outputs an interactive HTML report with radar charts, heatmaps, and per-file drill-down.'
argument-hint: 'Provide the module path(s), project name, or folder(s) to assess'
user-invocable: true
---

# Code Quality Assessment

Use this skill to produce a comprehensive, visual code quality report for one or more modules. The output is a self-contained HTML file with interactive charts, color-coded scores, and per-file drill-down tables.

## Scope vs `code-review`

| | `code-quality` (this skill) | `code-review` |
|---|----------------------------|---------------|
| **Unit of analysis** | A module / project / folder (many files) | A single commit / PR / change set |
| **Time horizon** | Baseline + trend over time | Point-in-time decision: commit or not |
| **Output** | Module quality dashboard with hotspot heatmap | Pass/Warning/Fail verdict per commit + commit message |
| **Triggered by** | Quarterly review, pre-refactoring baseline, hotspot investigation | Pre-commit / pre-PR |
| **Modifies code?** | Never — read-only assessment | Never by default; can suggest fixes when asked |
| **Overlap** | Both reuse `tics-standard` + `codescene-health` for raw findings | Same |

**Rule of thumb:** if the question is "is this change OK to commit?" use `code-review`; if the question is "how healthy is this module?" use `code-quality`.

## When to Use

- A module has high bug density and you want to understand root causes.
- You need a quality baseline before a major refactoring effort.
- You want to compare quality across modules or track trends.
- You need a visual report for stakeholders, tech leads, or quality reviews.
- You are preparing for a delivery and need a quality gate assessment.

## Preferred Inputs

Provide one or more of the following:

- Module path(s) or folder(s) to assess (e.g., `ThreeDSurview/Src/`)
- Project file(s) (`.csproj`)
- Scope filter (e.g., "only service layer", "only ViewModels")
- Known bug areas or hotspot files (optional, for focused analysis)

If no explicit path is provided, assess the module containing the currently open file.

## Assessment Dimensions

The report covers **8 dimensions**, each scored 0–10:

### D1. CodeScene Code Health
Function-level and module-level code smells. See [codescene-metrics.md](./references/codescene-metrics.md).

### D2. TICS Compliance
Philips C# Coding Standard 5.33 rule adherence. See [tics-metrics.md](./references/tics-metrics.md).

### D3. Test Quality
Coverage, test ratio, assertion density, test naming. See [assessment-checklist.md § Test Quality](./references/assessment-checklist.md#d3-test-quality).

### D4. Architecture Conformance
Layering, dependency direction, coupling metrics. See [assessment-checklist.md § Architecture](./references/assessment-checklist.md#d4-architecture-conformance).

### D5. Maintainability
Duplication, file size, complexity distribution, TODO/HACK density. See [assessment-checklist.md § Maintainability](./references/assessment-checklist.md#d5-maintainability).

### D6. Security
Input validation, credential handling, dependency vulnerabilities. See [assessment-checklist.md § Security](./references/assessment-checklist.md#d6-security).

### D7. Performance Patterns
Hot-path complexity, resource management, UI thread safety. See [assessment-checklist.md § Performance](./references/assessment-checklist.md#d7-performance-patterns).

### D8. Defect Density & Risk
Bug-proneness indicators, change frequency, complexity hotspots. See [assessment-checklist.md § Defect Density](./references/assessment-checklist.md#d8-defect-density--risk).

## Workflow

1. **Identify scope**: Resolve the target module path(s). List all `.cs` files recursively (exclude generated, designer, migrations).
2. **Collect file metrics**: For each file, compute:
   - Lines of code, class count, method count
   - Max cyclomatic complexity, mean complexity
   - Max nesting depth
   - Public API without XML docs
   - Primitive obsession indicators
   - Duplication patterns
3. **Score each dimension**: Apply the scoring rubric from [scoring-rubric.md](./references/scoring-rubric.md) to convert raw metrics into 0–10 scores.
4. **Identify hotspots**: Rank files by combined risk (low score + high complexity + large size).
5. **Generate HTML report**: Use the template from [report-template.html](./references/report-template.html) to produce the output.
6. **Save report**: Write to `Code Quality Report/{module-name}_quality_report.html` under the workspace root (create the `Code Quality Report` directory if needed), unless the user explicitly specifies another output path.

## Scoring Overview

| Score | Grade | Color | Meaning |
|-------|-------|-------|---------|
| 9–10  | A     | Green | Excellent — low risk, easy to maintain |
| 7–8.9 | B     | Blue  | Good — minor issues, maintainable |
| 5–6.9 | C     | Yellow | Fair — noticeable tech debt, moderate risk |
| 3–4.9 | D     | Orange | Poor — high bug risk, needs attention |
| 0–2.9 | F     | Red   | Critical — active quality crisis |

## Output Specification

The HTML report must include:

Default save location: workspace root `Code Quality Report/` directory, with filename `{module-name}_quality_report.html` unless the user explicitly requests another path.

1. **Executive Summary** — Overall score, grade, radar chart of 8 dimensions
2. **Dimension Detail Cards** — Each dimension with score, gauge, top findings
3. **Hotspot Heatmap** — Files ranked by risk, color-coded
4. **Per-File Table** — Sortable table: File | LoC | CC | Nesting | Score | Issues
5. **Top 10 Riskiest Files** — Drill-down with specific findings
6. **Trend Indicators** — If historical data available
7. **Recommendations** — Prioritized action items (P0/P1/P2)

## Guardrails

- This is a **read-only assessment** — do not modify any source files.
- Do not fabricate metrics. If a metric cannot be computed locally, mark it as "N/A — requires tooling" and explain.
- Clearly distinguish measured vs estimated values.
- For dimensions that need external tools (e.g., test coverage requires running tests), provide the best local estimate and note the gap.
- Report is a snapshot, not a replacement for CI/TICS/CodeScene pipelines.

## Related Skills

- [Code Review](../code-review/SKILL.md) — for per-commit review
- [TICS Standard](../tics-standard/SKILL.md) — for deep TICS rule analysis
- [CodeScene Code Health](../codescene-health/SKILL.md) — for code health fixes

## References

- [Assessment checklist](./references/assessment-checklist.md)
- [Scoring rubric](./references/scoring-rubric.md)
- [CodeScene metrics](./references/codescene-metrics.md)
- [TICS metrics](./references/tics-metrics.md)
- [Report template](./references/report-template.html)

## Portability Note

This skill is **team-portable**. The 8 quality dimensions, 0–10 scoring rubric, A–F grade scale, and HTML report template are generic and apply to any C# codebase. Domain-specific quality checks (e.g., DICOM safety rules contributing to D6 Security) delegate to the `domain-knowledge` skill which each team customizes. Team-specific scoring overrides, hotspot baselines, and module-quality history belong in `references/` (versioned) or `/memories/repo/` (per-workspace).
