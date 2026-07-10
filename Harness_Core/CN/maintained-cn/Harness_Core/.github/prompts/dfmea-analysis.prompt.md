---
name: DFMEA Analysis
description: '根据 SRS/SSRS requirements 为 CT medical device subsystems 生成 DFMEA analysis。产出包含 failure modes、SOD scoring、risk classification、criticality matrix 和 mitigation recommendations 的结构化 HTML report。'
argument-hint: 'Paste SRS/SSRS requirements, or specify subsystem name / requirements document path'
agent: 'agent'
---

对提供的 SRS/SSRS requirements 执行 Design FMEA analysis，并生成一个 **self-contained HTML report**。

使用以下 repository assets 作为 criteria、template 和 domain knowledge 的来源：

- [SOD Criteria](../skills/dfmea-analysis/references/sod-criteria.md)
- [Criticality Matrix](../skills/dfmea-analysis/references/criticality-matrix.md)
- [DFMEA Column Guide](../skills/dfmea-analysis/references/column-guide.md)
- [Report Template](../skills/dfmea-analysis/references/report-template.html)
- DFMEA reference documents（用于 pattern reference 的 existing DFMEAs）：`C:\Work\Code\Git_Code\DFMEA\`

## Context

这是 **Philips CT medical device** DFMEA analysis。产品范围包括 Console Software、CT Gantry、CT Bed、Image Reconstruction System、iWorkflow、Direct Result Pipeline 以及相关 subsystems。分析必须符合 IEC 62304 和 ISO 14971 requirements。

## Workflow

1. **Parse requirements**：从用户输入中提取 requirement IDs、text 和 subsystem/function context。
2. **Identify Item/Function**：将每个 requirement 映射到 subsystem 和 functional area（例如 `Rib APP/Display rib labeling`）。
3. **Generate failure modes**：对每个 requirement 识别 ≥ 2 个 potential failure modes：
   - Functional failure（feature 完全不工作）
   - Secondary failure（incorrect output、degraded performance、data issue 等）
   - 使用命名约定：`FM01:`、`FM02:` 等。
4. **Analyze effects**：对每个 failure mode：
   - Local Effect：subsystem 内发生什么
   - End Effect：对 end user 的影响 — 使用标准 SOD severity phrases
   - Rationale：解释 user impact 和 available workarounds
5. **Identify causes**：每个 failure mode 的 technical root cause 或 mechanism。
6. **Score SOD (Initial State)**：
   - **Severity**：基于 end effect（S/8/5/3/1）
   - **Prevention Control**：描述 architectural controls、design decisions、safety mechanisms
   - **Occurrence**：基于 design maturity 和 similar failure history（10/8/5/3/1）
   - **Criticality**：计算 S × O
   - **Classification**：应用 criticality matrix（N/A / Consider CTQ / CTQ / RMM）
   - **Detection Control**：描述 test/verification methods
   - **Detection**：基于 test maturity（10/8/5/3/1）
   - **RPN**：计算 S × O × D
7. **Risk Management**：对分类为 RMM 的 items：
   - RMM ID：标记为 `TBD`（real IDs 由 RM process 分配）
   - RMM Severity of Harm：`S1` 或 `S2`
   - CTS Level：`CTS`、`No CTS`
8. **Score SOD (Optimized State)**：提出 mitigations 后重新评分。
9. **Build HTML report**，使用 [report template](../skills/dfmea-analysis/references/report-template.html)：
   - 用实际计算数据填充所有 `{{PLACEHOLDER}}` values
   - 用 risk statistics 填充 executive summary
   - 用所有 entries 和所有 columns 填充 DFMEA analysis table
   - 在 criticality matrix 中填入每个 cell 的 failure mode counts
   - 生成 prioritized recommendations
   - 添加需要 expert confirmation 的 review notes
10. **Save the HTML file**：写入 workspace root 下的 `DFMEA/{subsystem}_dfmea_report.html`（如需要则创建 `DFMEA` 目录），除非用户明确指定其他输出路径。

## Output

一个 self-contained HTML file（light theme），包含：
- Default save location：workspace root `DFMEA/` directory，filename 为 `{subsystem}_dfmea_report.html`，除非用户明确要求其他路径。
- Hero banner with subsystem info and metadata
- Executive summary with risk distribution statistics（Safety/High/Medium/Low counts）
- SOD criteria reference tables（嵌入以方便 reviewer 验证评分）
- Criticality matrix visualization with failure mode distribution
- **Full DFMEA analysis table** — 主交付物，包含 template 中全部 29 columns
- Prioritized recommendations（Must-Address / Should-Address / Consider）
- Review notes and disclaimer

## Rules

- **This is a draft generator** — 始终声明结果需要 DFMEA team review。
- 不确定时使用保守（更高）的 SOD scores。
- **不要编造 RMM IDs** — 使用 `TBD`，并注明 real IDs 必须由 RM 分配。
- 每个 requirement 生成 ≥ 2 个 failure modes。
- 对 Safety-classified items（Severity = S），始终建议考虑 RMM。
- 用实际 analysis data 填充 HTML report — 不要保留 placeholder values。
- report 必须完全 self-contained（inline CSS、inline JS）。
- 在 report 中包含 SOD criteria reference，方便 reviewers 验证 ratings。
- 可用时，参考 reference documents 中的 existing DFMEA patterns。
