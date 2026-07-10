---
name: code-quality
description: '从 8 个维度评估 module 或 project 的 code quality：CodeScene health、TICS compliance、test coverage、architecture conformance、maintainability、security、performance patterns 和 defect density。输出包含 radar charts、heatmaps 和 per-file drill-down 的交互式 HTML report。'
argument-hint: 'Provide the module path(s), project name, or folder(s) to assess'
user-invocable: true
---

# Code Quality Assessment

使用此 skill 为一个或多个 modules 生成全面、可视化的 code quality report。输出是 self-contained HTML file，包含 interactive charts、color-coded scores 和 per-file drill-down tables。

## Scope vs `code-review`

| | `code-quality` (this skill) | `code-review` |
|---|----------------------------|---------------|
| **Unit of analysis** | A module / project / folder（many files） | A single commit / PR / change set |
| **Time horizon** | Baseline + trend over time | Point-in-time decision: commit or not |
| **Output** | Module quality dashboard with hotspot heatmap | Pass/Warning/Fail verdict per commit + commit message |
| **Triggered by** | Quarterly review, pre-refactoring baseline, hotspot investigation | Pre-commit / pre-PR |
| **Modifies code?** | Never — read-only assessment | Never by default; can suggest fixes when asked |
| **Overlap** | Both reuse `tics-standard` + `codescene-health` for raw findings | Same |

**Rule of thumb:** 如果问题是 "is this change OK to commit?"，使用 `code-review`；如果问题是 "how healthy is this module?"，使用 `code-quality`。

## When to Use

- 模块 bug density 较高，你想理解 root causes。
- 在重大 refactoring effort 前需要 quality baseline。
- 想比较多个 modules 的质量或跟踪 trends。
- 需要面向 stakeholders、tech leads 或 quality reviews 的 visual report。
- 正在准备 delivery，需要 quality gate assessment。

## Preferred Inputs

提供以下一项或多项：

- 要评估的 module path(s) 或 folder(s)（例如 `ThreeDSurview/Src/`）
- Project file(s)（`.csproj`）
- Scope filter（例如 "only service layer"、"only ViewModels"）
- 已知 bug areas 或 hotspot files（可选，用于 focused analysis）

如果未提供显式 path，则评估当前打开文件所在 module。

## Assessment Dimensions

报告覆盖 **8 dimensions**，每项按 0–10 打分：

### D1. CodeScene Code Health
Function-level 和 module-level code smells。见 [codescene-metrics.md](./references/codescene-metrics.md)。

### D2. TICS Compliance
Philips C# Coding Standard 5.33 rule adherence。见 [tics-metrics.md](./references/tics-metrics.md)。

### D3. Test Quality
Coverage、test ratio、assertion density、test naming。见 [assessment-checklist.md § Test Quality](./references/assessment-checklist.md#d3-test-quality)。

### D4. Architecture Conformance
Layering、dependency direction、coupling metrics。见 [assessment-checklist.md § Architecture](./references/assessment-checklist.md#d4-architecture-conformance)。

### D5. Maintainability
Duplication、file size、complexity distribution、TODO/HACK density。见 [assessment-checklist.md § Maintainability](./references/assessment-checklist.md#d5-maintainability)。

### D6. Security
Input validation、credential handling、dependency vulnerabilities。见 [assessment-checklist.md § Security](./references/assessment-checklist.md#d6-security)。

### D7. Performance Patterns
Hot-path complexity、resource management、UI thread safety。见 [assessment-checklist.md § Performance](./references/assessment-checklist.md#d7-performance-patterns)。

### D8. Defect Density & Risk
Bug-proneness indicators、change frequency、complexity hotspots。见 [assessment-checklist.md § Defect Density](./references/assessment-checklist.md#d8-defect-density--risk)。

## Workflow

1. **Identify scope**：解析 target module path(s)。递归列出所有 `.cs` files（排除 generated、designer、migrations）。
2. **Collect file metrics**：对每个文件计算：
   - Lines of code、class count、method count
   - Max cyclomatic complexity、mean complexity
   - Max nesting depth
   - Public API without XML docs
   - Primitive obsession indicators
   - Duplication patterns
3. **Score each dimension**：应用 [scoring-rubric.md](./references/scoring-rubric.md) 中的 scoring rubric，将 raw metrics 转换为 0–10 scores。
4. **Identify hotspots**：按 combined risk（low score + high complexity + large size）排序 files。
5. **Generate HTML report**：使用 [report-template.html](./references/report-template.html) 的 template 生成输出。
6. **Save report**：写入 workspace root 下的 `Code Quality Report/{module-name}_quality_report.html`（如需要则创建 `Code Quality Report` 目录），除非用户明确指定其他 output path。

## Scoring Overview

| 分数 | 等级 | 颜色 | 含义 |
|-------|-------|-------|---------|
| 9–10  | A     | 绿色 | Excellent — 低风险，易维护 |
| 7–8.9 | B     | 蓝色  | Good — 轻微问题，可维护 |
| 5–6.9 | C     | 黄色 | Fair — 明显 tech debt，中等风险 |
| 3–4.9 | D     | 橙色 | Poor — 高 bug 风险，需要关注 |
| 0–2.9 | F     | 红色   | Critical — 活跃质量危机 |

## Output Specification

HTML report 必须包含：

Default save location：workspace root `Code Quality Report/` directory，filename 为 `{module-name}_quality_report.html`，除非用户明确要求其他路径。

1. **Executive Summary** — Overall score、grade、8 dimensions radar chart
2. **Dimension Detail Cards** — 每个 dimension 的 score、gauge、top findings
3. **Hotspot Heatmap** — 按 risk 排名并 color-coded 的 files
4. **Per-File Table** — Sortable table: File | LoC | CC | Nesting | Score | Issues
5. **Top 10 Riskiest Files** — 带具体 findings 的 drill-down
6. **Trend Indicators** — 如有 historical data
7. **Recommendations** — Prioritized action items（P0/P1/P2）

## Guardrails

- 这是 **read-only assessment** — 不要修改任何 source files。
- 不要编造 metrics。如果某个 metric 无法本地计算，标记为 "N/A — requires tooling" 并解释。
- 清晰区分 measured 与 estimated values。
- 对需要外部工具的 dimensions（例如 test coverage 需要运行 tests），提供最佳 local estimate 并注明 gap。
- Report 是 snapshot，不能替代 CI/TICS/CodeScene pipelines。

## Related Skills

- [Code Review](../code-review/SKILL.md) — 用于 per-commit review
- [TICS Standard](../tics-standard/SKILL.md) — 用于 deep TICS rule analysis
- [CodeScene Code Health](../codescene-health/SKILL.md) — 用于 code health fixes

## References

- [Assessment checklist](./references/assessment-checklist.md)
- [Scoring rubric](./references/scoring-rubric.md)
- [CodeScene metrics](./references/codescene-metrics.md)
- [TICS metrics](./references/tics-metrics.md)
- [Report template](./references/report-template.html)

## Portability Note

此 skill 具备 **team-portable** 性。8 quality dimensions、0–10 scoring rubric、A–F grade scale 和 HTML report template 都是通用的，适用于任何 C# codebase。Domain-specific quality checks（例如影响 D6 Security 的 DICOM safety rules）委派给每个团队自定义的 `domain-knowledge` skill。Team-specific scoring overrides、hotspot baselines 和 module-quality history 应放在 `references/`（versioned）或 `/memories/repo/`（per-workspace）中。
