---
name: Code Quality Assessment
description: 'Assess module code quality across 8 dimensions and generate an interactive HTML report with radar charts, heatmaps, and prioritized recommendations. Use for modules with high bug density or before major refactoring.'
argument-hint: 'Module path(s) or folder(s) to assess, e.g. ThreeDSurview/Src/'
agent: 'agent'
---

Perform a comprehensive code quality assessment on the specified module and generate an HTML report.

Use these repository assets as the source of metrics and rules:

- [Assessment checklist](../skills/code-quality/references/assessment-checklist.md)
- [Scoring rubric](../skills/code-quality/references/scoring-rubric.md)
- [CodeScene metrics](../skills/code-quality/references/codescene-metrics.md)
- [TICS metrics](../skills/code-quality/references/tics-metrics.md)
- [Report template](../skills/code-quality/references/report-template.html)
- [TICS instructions](../instructions/tics-csharp.instructions.md)
- [CodeScene instructions](../instructions/codescene-csharp.instructions.md)

## Workflow

1. **Resolve scope**: Identify the module path. List all `.cs` files (exclude `*.Designer.cs`, `*.g.cs`, `*.AssemblyInfo.cs`, `obj/`, `bin/`).
2. **Collect metrics per file**: For each file, read and analyze:
   - LoC, class count, method count
   - Per-method: CC, nesting depth, parameter analysis
   - Copyright header presence, XML doc coverage
   - Using statements for dependency analysis
   - Pattern detection (try/catch, IDisposable, events, async)
3. **Score each of 8 dimensions**: Apply the scoring rubric.
4. **Calculate overall score**: Use the weighted formula from the checklist.
5. **Identify hotspots**: Rank files by combined risk.
6. **Generate recommendations**: Prioritize as P0/P1/P2.
7. **Build HTML report**: Fill the template with computed data, including:
   - Radar chart SVG with 8-axis polygon
   - Dimension score cards with gauges
   - File heatmap with color-coded risk
   - Sortable per-file table
   - Top 10 riskiest files with drill-down
   - Prioritized recommendations
8. **Save the HTML file**: Write to `Code Quality Report/{module}_quality_report.html` under the workspace root (create the `Code Quality Report` directory if needed), unless the user explicitly specifies another output path.

## Important Rules

- **Read-only assessment** — do not modify any source files.
- Process files in batches to avoid token limits. If the module has > 50 files, sample strategically:
  - Always include the largest 10 files
  - Always include files with the most methods
  - Random sample the rest
  - Note the sampling in the report
- For metrics that need external tools (test coverage %), estimate from test project structure and note the gap.
- Fill the HTML template with actual computed data — do not output a template with placeholders.
- The radar chart must be a proper SVG polygon, not a placeholder.
- Use inline SVG in the HTML — no external dependencies. The report must be fully self-contained.

## Output

A single self-contained HTML file that can be opened in any browser, with:
- Default save location: workspace root `Code Quality Report/` directory, with filename `{module}_quality_report.html` unless the user explicitly requests another path.
- Interactive sortable table
- Color-coded radar chart
- File risk heatmap
- Actionable recommendations
