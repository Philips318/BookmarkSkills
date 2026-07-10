---
name: Code Review
description: '带 HTML report 输出的全面 pre-commit code review。分析 change scope、fix/feature completeness、regression risk、test impact、TICS/CodeScene compliance、architecture conformance，并生成 commit logs。'
argument-hint: 'Optional: branch, ticket ID, or description of the change'
agent: 'agent'
---

对当前 workspace changes 执行全面 code review，并生成一个 **self-contained HTML report**。

使用以下 repository assets 作为 policy、rules 和 report format 的来源：

- [Review checklist](../skills/code-review/references/review-checklist.md)
- [Architecture rules](../skills/code-review/references/architecture-rules.md)
- [Commit log template](../skills/code-review/references/commit-log-template.md)
- [Report template](../skills/code-review/references/report-template.html)
- [TICS instructions](../instructions/tics-csharp.instructions.md)
- [CodeScene instructions](../instructions/codescene-csharp.instructions.md)
- [TICS common rules](../skills/tics-standard/references/common-rules.md)
- [CodeScene common issues](../skills/codescene-health/references/common-issues.md)

## Workflow

1. **Detect changes**：运行 `git diff --name-only` 和 `git status` 识别所有 changed files。如果指定了 branch 或 scope，则使用该范围。
2. **Read diffs**：读取每个 changed file 的实际 diff content。统计 additions 和 deletions。
3. **Classify change type**：判断这是 fix、feat、refactor、chore 还是 mixed change。
4. **Run all 8 review dimensions** from the review checklist：
   - §1 Change Scope — 列出并分类所有 files
   - §2 Fix Completeness — 验证 root cause 已被解决（适用于 fixes）
   - §3 Feature Completeness — 验证所有 requirements 已满足（适用于 features）
   - §4 Regression Risk — 检查新的 null/thread/resource/API risks
   - §5 Test Impact — 映射 affected features 并推荐 test plan
   - §6 Coding Standards — 对 changed `.cs` files 运行 TICS + CodeScene preflight
   - §7 Architecture Conformance — 检查 layering、dependencies、design rules
   - §8 Commit Log — 生成可直接使用的 commit message
5. **Score each dimension**：分配 Pass (10) / Warning (5) / Fail (0) / N/A。
6. **Calculate overall verdict**：
   - **Pass**：所有维度 Pass（无 Fail、无 Warning）
   - **Warning**：至少一个 Warning，且无 Fail
   - **Fail**：至少一个 Fail
7. **Build the HTML report**，使用 [report template](../skills/code-review/references/report-template.html)：
   - 用实际计算数据填充所有 `{{PLACEHOLDER}}` values
   - 根据 8 dimension scores 构建正确的 SVG radar chart polygon
   - **For fix changes**：用 root cause analysis paragraphs 和 one-liner summary 填充 "Root Cause 总结" section；省略 "Feature 交付总结" section
   - **For feature changes**：用 ✓/✗ checklist 填充 "Feature 交付总结" section；省略 "Root Cause 总结" section
   - 填充 summary table、per-dimension detailed conclusions、changed files cards
   - 在深色 `<pre>` blocks 中包含关键 diff snippets，并对 diff lines 着色
   - 生成 test plan、commit message、required actions 和 residual risks sections
   - 确保 report 完全 self-contained（inline CSS、inline SVG、inline JS）
8. **Save the HTML file**：写入 workspace root 下的 `Code Review Report/{project_or_branch}_review_report.html`（如需要则创建 `Code Review Report` 目录），除非用户明确指定其他位置。

## Output

一个可在任意浏览器打开的 self-contained HTML file（light theme），包含：
- Default save location：workspace root `Code Review Report/` directory，filename 为 `{project_or_branch}_review_report.html`，除非用户明确要求其他路径。
- Hero banner with gradient header、metadata pills 和 overall verdict badge
- **Root Cause 总结**（仅 fix changes）：Root cause analysis + one-liner summary box
- **Feature 交付总结**（仅 feature changes）：Delivered features checklist with ✓/✗ status
- 核心发现 overview paragraph
- Summary table with statuses and conclusions per dimension
- Radar chart visualizing 8 dimension scores
- 各维度详细结论 with card grids、diff code snippets 和 finding lists
- Prioritized test plan（P0 / P1 / P2 ordered lists）
- Generated commit message（with copy button）
- 提交前必须修复（Must-Fix / Should-Fix action items）
- Residual Risks

## Rules

- 默认 **review-only** — 除非用户明确要求，否则不要编辑 source files。
- 不要跳过 8 个维度中的任何一个。将不适用的维度标为 "N/A" 并说明原因。
- 要具体 — 引用 file names、line numbers 和具体 evidence。
- 用实际计算数据填充 HTML template — 不要输出带 placeholders 的 template。
- radar chart 必须是正确的 SVG polygon，不是 placeholder。
- 使用 inline SVG — 无 external dependencies。report 必须完全 self-contained。
- 明确标记不确定性。
