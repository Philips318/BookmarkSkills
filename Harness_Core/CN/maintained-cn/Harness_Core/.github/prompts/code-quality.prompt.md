---
name: Code Quality Assessment
description: '从 8 个维度评估模块代码质量，并生成包含 radar charts、heatmaps 和优先级建议的交互式 HTML report。适用于 bug density 较高的模块或重大重构前。'
argument-hint: 'Module path(s) or folder(s) to assess, e.g. ThreeDSurview/Src/'
agent: 'agent'
---

对指定模块执行全面 code quality assessment，并生成 HTML report。

使用以下 repository assets 作为 metrics 和 rules 的来源：

- [Assessment checklist](../skills/code-quality/references/assessment-checklist.md)
- [Scoring rubric](../skills/code-quality/references/scoring-rubric.md)
- [CodeScene metrics](../skills/code-quality/references/codescene-metrics.md)
- [TICS metrics](../skills/code-quality/references/tics-metrics.md)
- [Report template](../skills/code-quality/references/report-template.html)
- [TICS instructions](../instructions/tics-csharp.instructions.md)
- [CodeScene instructions](../instructions/codescene-csharp.instructions.md)

## Workflow

1. **Resolve scope**：识别 module path。列出所有 `.cs` files（排除 `*.Designer.cs`、`*.g.cs`、`*.AssemblyInfo.cs`、`obj/`、`bin/`）。
2. **Collect metrics per file**：对每个文件读取并分析：
   - LoC、class count、method count
   - 每个 method：CC、nesting depth、parameter analysis
   - copyright header presence、XML doc coverage
   - using statements，用于 dependency analysis
   - pattern detection（try/catch、IDisposable、events、async）
3. **Score each of 8 dimensions**：应用 scoring rubric。
4. **Calculate overall score**：使用 checklist 中的 weighted formula。
5. **Identify hotspots**：按 combined risk 排序文件。
6. **Generate recommendations**：按 P0/P1/P2 确定优先级。
7. **Build HTML report**：用计算出的数据填充 template，包含：
   - Radar chart SVG with 8-axis polygon
   - Dimension score cards with gauges
   - File heatmap with color-coded risk
   - Sortable per-file table
   - Top 10 riskiest files with drill-down
   - Prioritized recommendations
8. **Save the HTML file**：写入 workspace root 下的 `Code Quality Report/{module}_quality_report.html`（如需要则创建 `Code Quality Report` 目录），除非用户明确指定其他输出路径。

## Important Rules

- **Read-only assessment** — 不要修改任何 source files。
- 分批处理文件以避免 token limits。如果模块超过 50 个文件，请策略性抽样：
  - 始终包含最大的 10 个文件
  - 始终包含方法最多的文件
  - 对剩余文件随机抽样
  - 在 report 中注明 sampling
- 对需要外部工具的 metrics（test coverage %），基于 test project structure 估算并注明 gap。
- 使用实际计算数据填充 HTML template — 不要输出带 placeholders 的 template。
- radar chart 必须是合格的 SVG polygon，不是 placeholder。
- 在 HTML 中使用 inline SVG — 无 external dependencies。report 必须完全 self-contained。

## Output

一个可在任意浏览器打开的 self-contained HTML file，包含：
- Default save location：workspace root `Code Quality Report/` directory，filename 为 `{module}_quality_report.html`，除非用户明确要求其他路径。
- Interactive sortable table
- Color-coded radar chart
- File risk heatmap
- Actionable recommendations
