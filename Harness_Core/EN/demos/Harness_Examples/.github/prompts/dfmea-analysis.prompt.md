---
name: DFMEA Analysis
description: 'Generate DFMEA analysis for CT medical device subsystems from SRS/SSRS requirements. Produces structured HTML report with failure modes, SOD scoring, risk classification, criticality matrix, and mitigation recommendations.'
argument-hint: 'Paste SRS/SSRS requirements, or specify subsystem name / requirements document path'
agent: 'agent'
---

Perform a Design FMEA analysis on the provided SRS/SSRS requirements and generate a **self-contained HTML report**.

Use these repository assets as the source of criteria, template, and domain knowledge:

- [SOD Criteria](../skills/dfmea-analysis/references/sod-criteria.md)
- [Criticality Matrix](../skills/dfmea-analysis/references/criticality-matrix.md)
- [DFMEA Column Guide](../skills/dfmea-analysis/references/column-guide.md)
- [Report Template](../skills/dfmea-analysis/references/report-template.html)
- DFMEA reference documents (existing DFMEAs for pattern reference): `C:\Work\Code\Git_Code\DFMEA\`

## Context

This is a **Philips CT medical device** DFMEA analysis. The product scope includes Console Software, CT Gantry, CT Bed, Image Reconstruction System, iWorkflow, Direct Result Pipeline, and related subsystems. The analysis must conform to IEC 62304 and ISO 14971 requirements.

## Workflow

1. **Parse requirements**: Extract requirement IDs, text, and subsystem/function context from the user's input.
2. **Identify Item/Function**: Map each requirement to its subsystem and functional area (e.g., `Rib APP/Display rib labeling`).
3. **Generate failure modes**: For each requirement, identify ≥ 2 potential failure modes:
   - Functional failure (feature does not work at all)
   - Secondary failure (incorrect output, degraded performance, data issue, etc.)
   - Use naming convention: `FM01:`, `FM02:`, etc.
4. **Analyze effects**: For each failure mode:
   - Local Effect: What happens within the subsystem
   - End Effect: Impact on end user — use standard SOD severity phrases
   - Rationale: Explain user impact and available workarounds
5. **Identify causes**: Technical root cause or mechanism for each failure mode.
6. **Score SOD (Initial State)**:
   - **Severity**: Based on end effect (S/8/5/3/1)
   - **Prevention Control**: Describe architectural controls, design decisions, safety mechanisms
   - **Occurrence**: Based on design maturity and similar failure history (10/8/5/3/1)
   - **Criticality**: Calculate S × O
   - **Classification**: Apply criticality matrix (N/A / Consider CTQ / CTQ / RMM)
   - **Detection Control**: Describe test/verification methods
   - **Detection**: Based on test maturity (10/8/5/3/1)
   - **RPN**: Calculate S × O × D
7. **Risk Management**: For items classified as RMM:
   - RMM ID: Mark as `TBD` (real IDs assigned by RM process)
   - RMM Severity of Harm: `S1` or `S2`
   - CTS Level: `CTS`, `No CTS`
8. **Score SOD (Optimized State)**: After proposing mitigations, re-score.
9. **Build HTML report** using the [report template](../skills/dfmea-analysis/references/report-template.html):
   - Fill all `{{PLACEHOLDER}}` values with actual computed data
   - Populate the executive summary with risk statistics
   - Fill the DFMEA analysis table with all entries and all columns
   - Populate the criticality matrix with failure mode counts per cell
   - Generate prioritized recommendations
   - Add review notes for items requiring expert confirmation
10. **Save the HTML file**: Write to `DFMEA/{subsystem}_dfmea_report.html` under the workspace root (create the `DFMEA` directory if needed), unless the user explicitly specifies another output path.

## Output

A single self-contained HTML file (light theme) with:
- Default save location: workspace root `DFMEA/` directory, with filename `{subsystem}_dfmea_report.html` unless the user explicitly requests another path.
- Hero banner with subsystem info and metadata
- Executive summary with risk distribution statistics (Safety/High/Medium/Low counts)
- SOD criteria reference tables (embedded for reviewer convenience)
- Criticality matrix visualization with failure mode distribution
- **Full DFMEA analysis table** — the main deliverable, with all 29 columns per the template
- Prioritized recommendations (Must-Address / Should-Address / Consider)
- Review notes and disclaimer

## Rules

- **This is a draft generator** — always state that results require DFMEA team review.
- Use conservative (higher) SOD scores when uncertain.
- Do **not** fabricate RMM IDs — use `TBD` and note that real IDs must be assigned by RM.
- Generate ≥ 2 failure modes per requirement.
- For Safety-classified items (Severity = S), always recommend RMM consideration.
- Fill the HTML report with actual analysis data — no placeholder values.
- The report must be fully self-contained (inline CSS, inline JS).
- Include the SOD criteria reference in the report so reviewers can verify ratings.
- Reference existing DFMEA patterns from the reference documents when available.
