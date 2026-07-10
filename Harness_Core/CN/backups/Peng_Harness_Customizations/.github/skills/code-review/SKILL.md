---
name: code-review
description: '用于 commit readiness 的全面 code review。分析 changed files、fix completeness、feature completeness、regression risk、test impact、coding standard compliance（TICS/CodeScene）、architecture conformance，并生成 commit logs。'
argument-hint: 'Provide the commit scope: branch name, changed files, ticket/issue ID, or a description of the change'
user-invocable: true
---

# Code Review

使用此 skill 在 commit 或提交 PR 前执行结构化、多维度 code review。它会产出可行动的 review report，覆盖八个 review dimensions。

## When to Use

- 你准备 commit code，并希望进行全面 self-review。
- 你在批准前 review 他人的 change。
- 你需要结构化 pre-commit 或 pre-PR quality gate。
- 你想为变更生成格式良好的 commit message。

## Preferred Inputs

提供以下一项或多项：

- Branch name 或 diff scope（例如 `git diff HEAD~1`）
- Changed files 列表
- Ticket/issue ID，或 bug fix / feature 描述
- Architectural context 或 design intent

如果未提供显式 input，此 skill 会通过 `git status` 和 `git diff` 检测 changed files。

## Review Dimensions

此 skill 沿 **eight dimensions** 审查，每个 dimension 都会在最终 report 中生成一个 section。

### Domain Accuracy（cross-cutting，应用于所有 dimensions）

对于任何触及 CT/DICOM/Spectral/ISP code 的变更，也要查阅 `domain-knowledge` skill：

- **dicom-patterns.md** — 验证正确的 VR types、tag IDs、上下文中的 transfer syntax、MONO1 vs MONO2 handling、SliceThickness vs SpacingBetweenSlices、RescaleSlope/Intercept usage
- **spectral-knowledge.md** — 验证 SBI version gates、MonoE keV range (40-200)、max 4 concurrent MonoE、non-HU RGB rule
- **safety-rules.md** — 如果缺少 safety controls，将任何触及 dose、patient ID、radiation interlock、spectral quantification 的代码升级为 Critical severity
- **ct-glossary.md** — 验证 correct units、value ranges 和 formal feature names 的使用

在问题表现出来的维度下报告 domain-accuracy issues（通常是 Regression Risk 或 Feature Completeness）。

### 1. Change Scope — Files & Impact Map

识别变更触及的所有 files、分类，并映射 dependency graph。

见 [review-checklist.md § Change Scope](./references/review-checklist.md#1-change-scope)。

### 2. Fix Completeness

对于 bug-fix commits：验证 root cause 已被解决、edge cases 被覆盖，并且 fix 不会留下 partial states。

见 [review-checklist.md § Fix Completeness](./references/review-checklist.md#2-fix-completeness)。

### 3. Feature Completeness

对于 feature commits：验证所有 acceptance criteria 被满足，UI/UX flows 完整，integration points 已接线。

见 [review-checklist.md § Feature Completeness](./references/review-checklist.md#3-feature-completeness)。

### 4. Regression Risk — New Issues

分析变更是否引入新风险：null-safety、thread-safety、resource leaks、behavioral side effects 或 API contract breakage。

见 [review-checklist.md § Regression Risk](./references/review-checklist.md#4-regression-risk)。

### 5. Test Impact — Affected Features

将变更映射到 affected functional areas，并推荐 focused testing targets。

见 [review-checklist.md § Test Impact](./references/review-checklist.md#5-test-impact)。

### 6. Coding Standards — TICS & CodeScene

对 changed files 运行组合 TICS + CodeScene preflight check。

见 [review-checklist.md § Coding Standards](./references/review-checklist.md#6-coding-standards)。

### 7. Architecture Conformance

验证变更遵循 project 的 layering、dependency 和 design rules。

见 [review-checklist.md § Architecture Conformance](./references/review-checklist.md#7-architecture-conformance) 和 [architecture-rules.md](./references/architecture-rules.md)。

**Architecture Drift sub-check（当 `artifacts/<feature>/03-implementation-log.md` 包含 "Post-implementation patches" 或 "Resolved post-implementation" section 时必需）：** 将 post-patch implementation 与 `02-architecture.md` 中记录的 interface contracts 和 component responsibilities 做 diff。当出现以下 drift signals 时标记为 Warning：

- Interface signature 中声明的 parameter 在 patch 后变成 **dead**（所有 implementation 中均未使用）— contract 不再匹配 design intent。
- Architecture 中声明的 field、property 或 DI dependency 被 implementation **silently dropped**。
- Architecture 中记录的 responsibility（例如 "Provider applies redaction"）在 patch 期间被**移动到不同 component**，且没有 ADR 或 implementation-log note。
- Post-patch 期间**新增** non-trivial behavior（例如 "force entry-assembly inclusion"），但 architecture document 未反映 — 要求同一 PR 中更新 architecture document。

此 sub-check 存在的原因是 mid-implementation Q-A/Q-B answer rounds 经常留下 architectural drift，而其他单一维度通常抓不到。Reviewer 是 drift 变成永久问题前最后一道防线。

### 8. Commit Log Generation

生成 conventional、结构良好的 commit message 来总结变更。

见 [review-checklist.md § Commit Log](./references/review-checklist.md#8-commit-log) 和 [commit-log-template.md](./references/commit-log-template.md)。

## Workflow

1. **Gather context**：通过 `git diff --name-only` 和 `git status` 识别 changed files。读取 diff content。
2. **Understand intent**：判断变更是 bug fix、feature、refactoring 还是 mixed。可使用提供的 ticket/issue description。
3. **Run each dimension**：按顺序遍历八个 review dimensions。对每个 dimension：
   a. 根据该 dimension 的 checklist 分析 changed code。
   b. 以 Pass / Warning / Fail 记录 findings，并提供具体 evidence（file、line、reason）。
   c. 分配 numeric score：Pass = 10，Warning = 5，Fail = 0。
4. **Coding standards deep-dive**：对于 Dimension 6，应用以下规则：
   - [TICS instructions](../../instructions/tics-csharp.instructions.md)
   - [CodeScene instructions](../../instructions/codescene-csharp.instructions.md)
   - [TICS common rules](../tics-standard/references/common-rules.md)（if available）
   - [CodeScene common issues](../codescene-health/references/common-issues.md)（if available）
5. **Generate commit log**：根据 review findings，按 template 产出 commit message。
6. **Calculate overall verdict**：从各 dimension verdicts 计算 overall status：
   - **Overall Pass**：所有 dimensions Pass（无 Fail、无 Warning）
   - **Overall Warning**：至少一个 Warning，且无 Fail
   - **Overall Fail**：至少一个 Fail
7. **Build HTML report**：用计算出的数据填充 [report template](./references/report-template.html)，包含：
   - 带 overall verdict badge（Pass / Warning / Fail）和 metadata pills 的 hero banner
   - **Root Cause 总结**（fix changes）：Root cause analysis paragraphs + one-liner summary
   - **Feature 交付总结**（feature changes）：每个 feature point 带 ✓/✗ 的 Feature delivery checklist
   - 核心发现 overview paragraph
   - 展示 dimension scores 的 8-axis polygon Radar chart SVG
   - 包含全部 8 dimensions 和 status badges 的 Summary table
   - 各维度详细结论 with card grids、diff snippets、finding blocks per dimension
   - 带 categories 和 impact 的 Changed files grid cards
   - 带 P0/P1/P2 的 Test plan section（ordered lists）
   - 带 copy button 的 Generated commit message block
   - Required actions before commit（Must-Fix / Should-Fix）
   - Residual risks list
8. **Save the HTML file**：写入 workspace root 下的 `Code Review Report/{project_or_branch}_review_report.html`（如需要则创建 `Code Review Report` 目录），除非用户明确指定其他位置。

## Default Priority Policy

- **Must-fix before commit**：Dimensions 2（Fix Completeness）、3（Feature Completeness）或 4（Regression Risk — Critical severity）中的任何 Fail finding。
- **Should-fix before commit**：Dimensions 4、6 或 7 中的任何 Warning。
- **Recommended**：Dimension 5（test recommendations）和 Dimension 8（commit log polish）中的 findings。

## Guardrails

- 除非用户明确要求，否则不要 auto-fix code。默认 mode 是 review-only。
- 不要修改 current change scope 之外的文件。
- 不要 suppress 或隐藏 findings 以生成 "clean" report。
- 标记任何不确定性 — 如果某个 dimension 无法完整评估（例如没有 test coverage data），请明确说明。
- 此 skill 是 pre-check。最终权威仍然是团队 CI pipeline、TICS server 和 CodeScene PR review。

## Expected Output

一个 **self-contained HTML report**（light-theme、no external dependencies），可在任意浏览器打开，包含：

Default save location：workspace root `Code Review Report/` directory，filename 为 `{project_or_branch}_review_report.html`，除非用户明确要求其他路径。

1. **Hero banner**：Gradient header with review scope、metadata pills 和 overall verdict badge
2. **Root Cause 总结**（仅 fix changes）：包含 root cause analysis paragraphs 和 highlighted one-liner summary box 的专用 section。feature changes 省略此 section。
3. **Feature 交付总结**（仅 feature changes）：包含 delivered features checklist（✓ completed / ✗ incomplete）和 summary note 的专用 section。fix changes 省略此 section。
4. **核心发现**：整体 review status 的 brief overview paragraph
5. **Summary Table**：# | 维度 | 状态 (Pass/Warning/Fail badge) | 结论
6. **维度雷达图**：SVG polygon，可视化全部 8 dimensions 的 scores
7. **各维度详细结论**：Per-dimension subsections，包含 card grids、diff code blocks 和 finding lists
8. **测试计划**：P0/P1/P2 ordered lists
9. **建议 Commit Message**：带 copy button 的 pre-formatted commit message
10. **提交前必须修复**：Must-Fix / Should-Fix action items
11. **Residual Risks**：无法本地验证的 items

Report 使用 [report template](./references/report-template.html)，且必须完全 self-contained（inline CSS、inline SVG、inline JS）。无 external resources。

### Scoring Convention

| Dimension Status | Numeric Score | 颜色 |
|-----------------|---------------|-------|
| 通过 | 10 | Green (#22c55e) |
| 警告 | 5 | Yellow (#eab308) |
| 失败 | 0 | Red (#ef4444) |
| N/A | — (excluded) | Gray (#94a3b8) |

## Related Skills

- [TICS Standard](../tics-standard/SKILL.md) — 用于 deep TICS rule analysis
- [CodeScene Code Health](../codescene-health/SKILL.md) — 用于 deep Code Health analysis

## Important Rules

- **Review-only by default** — 除非用户明确要求 auto-fix，否则不要修改任何 source files。
- 用实际 computed data 填充 HTML template — 不要输出带 placeholders 的 template。
- radar chart 必须是根据实际 dimension scores 计算出的正确 SVG polygon，不是 placeholder。
- 使用 inline SVG、CSS 和 JS — report 必须完全 self-contained。
- 如果某个 dimension 无法完整评估（例如无 test coverage data），标记为 N/A，并从 radar chart polygon 中排除。
- 即使用户没有特别要求，也始终在 report 中包含 generated commit message。
- **`Severity rationale:` 是每个 Warning 和 Fail finding 的 required field**（不是可选项，也不是惯例）。缺少一行解释 *why* severity 是 Warning 而不是 Fail（或 Fail 而不是 Warning）的 rationale，会被视为 incomplete review，并阻止 verdict 成为 final。此规则在 PipelineEnvironmentInfoLogger dogfood 后从 convention 升级为 requirement，因为 rationale field 将 Should-Fix loop time 降低了一个数量级 — 它让 Developer 无需重新询问 Reviewer 就能区分 `fix-now` 与 `defer`。

## References

- [Review checklist](./references/review-checklist.md)
- [Architecture rules](./references/architecture-rules.md)
- [Commit log template](./references/commit-log-template.md)
- [Report template](./references/report-template.html)

## Portability Note

此 skill 具备 **team-portable** 性。8 review dimensions、Pass/Warning/Fail scoring、HTML report template 和 commit-log convention 都是通用的，适用于任何使用 TICS + CodeScene 的 C# project。Domain-accuracy checks 委派给每个 team 按 product line 自定义的 `domain-knowledge` skill — 无需修改此文件。Team-specific architecture rules 应放入 [`references/architecture-rules.md`](./references/architecture-rules.md)（按 repo versioned）或 `/memories/repo/`。
