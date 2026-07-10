---
name: dfmea-analysis
description: 'Generate DFMEA (Design Failure Mode and Effects Analysis) for CT medical device subsystems. Takes SRS/SSRS requirements as input and produces structured DFMEA analysis with Failure Modes, Effects, Causes, SOD scoring, Risk Classification, and Mitigation actions — delivered as a self-contained HTML report.'
argument-hint: 'Paste SRS/SSRS requirements text, or specify a requirements document path / subsystem name'
user-invocable: true
---

# DFMEA Analysis

Use this skill to perform Design Failure Mode and Effects Analysis (DFMEA) for CT medical device subsystems. Given SRS/SSRS requirement content, it generates DFMEA entries following the Philips CT DFMEA template format and delivers the results as an interactive HTML report.

## When to Use

- You have SRS/SSRS requirements and need to generate initial DFMEA entries for review.
- You are preparing for a DFMEA session and want draft analysis as a starting point.
- You need to identify potential failure modes, effects, and causes for a new feature or subsystem.
- You want to assess risk (SOD scoring) and determine whether Risk Mitigation Measures (RMM) are needed.
- You are extending an existing DFMEA with new requirements.

## Business Context

This skill is designed for **Philips CT medical device** development, covering subsystems including but not limited to:

- **Console Software** — Post-processing applications (MPR, 3D, DE, Rib, Spine, Endo, etc.)
- **CT Gantry** — Rotation, collimation, X-ray tube control
- **CT Bed (Table)** — Patient positioning, movement control
- **CT Image Reconstruction System** — Reconstruction algorithms, calibration
- **iWorkflow** — Scan planning, protocol management, patient workflow
- **Direct Result Pipeline** — Automated result generation, DICOM handling

The DFMEA process follows **IEC 62304** (Medical Device Software Lifecycle), **ISO 14971** (Risk Management for Medical Devices), and Philips internal DFMEA procedures.

## Related Skill: domain-knowledge

Before generating DFMEA entries, consult the `domain-knowledge` skill to identify domain-specific failure modes and safety red lines:

- **safety-rules.md** — IEC 62304 A/B/C classification triggers, radiation/data integrity/spectral/ISP safety red lines (use these as **seeds** for Failure Effects)
- **dicom-patterns.md** — 8 common DICOM pitfalls (Implicit VR + private tags, MONO1 vs MONO2, slice spacing vs thickness, etc.) — each is a known failure mode
- **spectral-knowledge.md** — spectral constraints (max 4 concurrent MonoE, SBI version gates, non-HU RGB rule) — violations are failure modes
- **ct-glossary.md** — correct units and value ranges; using wrong units (HU vs mg/ml) is a Failure Effect

This ensures DFMEA entries are grounded in known domain-specific failure modes rather than generic templates.

## Preferred Inputs

Provide one or more of the following:

- **SRS/SSRS requirement text**: One or more requirements with their IDs (e.g., `CT-NMP.SSRS.Console.7636 The rib application shall support to extract rib centerline with labeling`)
- **Subsystem / Feature name**: e.g., "Rib Application", "Spectral DE Workflow", "MPR Orientation Reset"
- **Existing DFMEA context**: If extending an existing DFMEA, provide the current entries or document reference
- **Architectural context**: Design documents, SDS references, component descriptions

If no explicit input is provided, the skill will prompt for the requirement content.

## DFMEA Column Structure

Each DFMEA entry follows the Philips CT DFMEA template with these columns:

### Identification

| Column | Description |
|--------|-------------|
| **ID** | Unique failure mode identifier (sequential or hierarchical, e.g., `1`, `2`, `rib-1`) |
| **Item/Function** | The subsystem/function being analyzed (e.g., `Rib APP/Display rib labeling`) |
| **Requirement** | The SRS/SSRS requirement ID and text |
| **Classification of Higher Order Requirement** | `N/A`, `CTS`, `CTQ`, or requirement class |

### Failure Analysis

| Column | Description |
|--------|-------------|
| **Potential Failure Mode** | What could go wrong (e.g., `FM01: The rib application fails to display the rib labeling`) |
| **Local Effect of Failure** | Direct local impact on the subsystem |
| **End Effect of Failure** | Impact on the end user / patient / system. Use standard effect phrases (see [End Effect phrases](./references/sod-criteria.md#standard-end-effect-phrases)) |
| **Rationale** | Explanation of the severity rating and user impact |
| **Potential Cause / Mechanism of Failure** | Root cause or mechanism (e.g., algorithm crash, data corruption, race condition) |

### Initial State (SOD Scoring)

| Column | Description |
|--------|-------------|
| **Severity (S1)** | Severity rating: `S`, `8`, `5`, `3`, `1` (see [SOD Criteria](./references/sod-criteria.md)) |
| **Prevention Control** | Design controls that prevent the failure (design rules, architecture decisions, redundancy) |
| **Occurrence (O1)** | Occurrence rating: `10`, `8`, `5`, `3`, `1` |
| **Criticality (S×O)** | Calculated: Severity × Occurrence |
| **Classification** | Risk classification: `N/A`, `Consider CTQ`, `CTQ`, `RMM` |
| **Detection Control** | How the failure is detected (software test, code review, integration test, etc.) |
| **Detection (D1)** | Detection rating: `10`, `8`, `5`, `3`, `1` |
| **RPN (S×O×D)** | Risk Priority Number = Severity × Occurrence × Detection |

### Risk Management

| Column | Description |
|--------|-------------|
| **RMM ID** | Risk Mitigation Measure ID (e.g., `CT-NM.RMM-P2-RMM.1104`) or `N/A` |
| **RMM Severity of Harm** | Severity of harm classification: `S1`, `S2`, or `N/A` |
| **RMM Confirmed CTS Level** | Confirmed CTS level: `CTS`, `No CTS`, or `N/A` |

### Optimized State (After Mitigation)

| Column | Description |
|--------|-------------|
| **Severity (S2)** | Re-assessed severity after mitigation |
| **Prevention Action** | Additional prevention actions taken |
| **Occurrence (O2)** | Re-assessed occurrence after mitigation |
| **Criticality (S×O) Optimized** | Recalculated criticality |
| **Classification Optimized** | Re-assessed risk classification |
| **Detection Action** | Additional detection actions taken |
| **Detection (D2)** | Re-assessed detection rating |
| **RPN (S×O×D) Optimized** | Recalculated RPN |

### Metadata

| Column | Description |
|--------|-------------|
| **Remarks** | Additional notes, references, version info |

## SOD Criteria

The Severity, Occurrence, and Detection ratings follow the Philips Design FMEA scale:

See [SOD Criteria](./references/sod-criteria.md) for the complete rating scales.

**Quick reference:**

| Severity | Level | Meaning |
|----------|-------|---------|
| S (Safety) | Safety-related failure, refer to hazard list |
| 8 | Critical | Loss of primary function (not safety) |
| 5 | Major | Convenience function inoperable |
| 3 | Minor | Slight user dissatisfaction |
| 1 | No effect | User probably won't notice |

| Occurrence | Level | Meaning |
|------------|-------|---------|
| 10 | Frequent | New technology, no history |
| 8 | Likely | New design, likely failure |
| 5 | Probable | Occasional failures in similar design |
| 3 | Remote | Isolated failures in identical design |
| 1 | Improbable | Eliminated through preventive control |

| Detection | Level | Meaning |
|-----------|-------|---------|
| 10 | Almost impossible | No test procedure capable |
| 8 | Remote | Uncertain procedure, limited experience |
| 5 | Moderate | Proven procedure, new usage |
| 3 | High | Physical testing, high confidence |
| 1 | Almost certain | Proven standards, prevents failure |

## Criticality Matrix

See [Criticality Matrix](./references/criticality-matrix.md) for the full matrix.

| Classification | Criticality (S×O) | Action Required |
|---------------|-------------------|-----------------|
| **Safety** | S × any O | Consider CTS; Additional mitigation efforts |
| **RMM** | ≥ 30 (e.g., S×10, 8×5, etc.) | Requirement/design decision is CTQ |
| **Consider CTQ** | 15–25 | Consider CTQ; Additional review |
| **N/A** | < 15 | Non-critical; Failure acceptably controlled |

## Workflow

1. **Parse requirements**: Extract requirement IDs, text, and subsystem context from the user's input.
2. **Identify Item/Function**: Determine the subsystem and function being analyzed.
3. **Generate failure modes**: For each requirement, identify potential failure modes:
   - Consider: functional failure, incorrect output, performance degradation, data corruption, timing issues, UI errors
   - Use the naming convention: `FM01:`, `FM02:`, etc.
4. **Analyze effects**: For each failure mode, determine:
   - Local effect (what happens in the subsystem)
   - End effect (what the user/patient experiences)
   - Use standard end effect phrases from the SOD criteria
5. **Identify causes**: Determine root causes and mechanisms:
   - Algorithm failures, data validation gaps, race conditions, resource exhaustion
   - Component crashes, communication failures, configuration errors
6. **Score SOD (Initial State)**: Apply severity, occurrence, and detection ratings based on:
   - Design architecture and controls already in place
   - Historical failure data from similar subsystems
   - Available test coverage
7. **Classify risk**: Based on criticality (S×O) and the criticality matrix:
   - Determine if RMM is required
   - Identify CTS/CTQ classifications
8. **Propose mitigations**: For items requiring action:
   - Prevention actions (design changes, additional controls)
   - Detection actions (additional tests, monitoring)
9. **Score SOD (Optimized State)**: Re-assess SOD after proposed mitigations.
10. **Build HTML report**: Generate the report using the [report template](./references/report-template.html):
    - Executive summary with risk statistics
    - SOD criteria reference section
    - Full DFMEA table with all columns
    - Criticality matrix visualization
    - Risk distribution charts
    - Actionable recommendations
11. **Save the HTML file**: Write to `DFMEA/{subsystem}_dfmea_report.html` under the workspace root (create the `DFMEA` directory if needed), unless the user explicitly specifies another output path.

## Analysis Guidelines

### Failure Mode Identification Patterns

For **software** subsystems, consider these common failure mode categories:

| Category | Example Failure Modes |
|----------|----------------------|
| **Functional** | Feature fails to execute, produces incorrect output, hangs/freezes |
| **Data** | Data corruption, data loss, incorrect data display |
| **Performance** | Slow response, timeout, resource exhaustion |
| **UI/UX** | Incorrect display, missing feedback, confusing interaction |
| **Integration** | Communication failure, protocol mismatch, version incompatibility |
| **Configuration** | Invalid settings, missing defaults, migration failure |
| **Concurrency** | Race condition, deadlock, data inconsistency |
| **Recovery** | Failure to recover after error, incomplete cleanup, state corruption |

### Standard End Effect Phrases

Use these standard phrases for the End Effect column (from the SOD severity scale):

- `Loss of primary function` (Severity = 8)
- `Temporary loss of primary function` (Severity = 8)
- `Degradation of primary function` (Severity = 8)
- `Function / Incorrect image or content` (Severity = S for safety)
- `convenience function inoperable` (Severity = 5)
- `temporary loss of convenience function` (Severity = 5)
- `Slight user dissatisfaction` (Severity = 3)
- `No discernible effect` (Severity = 1)

### Prevention Control Patterns

Common prevention controls for CT software:

- Independent process design (crash isolation)
- Input validation and boundary checking
- Error notification to user
- Fallback / manual workaround available
- Architecture review / design documents
- Standards compliance (IEC 62304, DICOM, etc.)
- Defensive coding (null checks, error handling)

### Detection Control Patterns

Common detection controls:

- `software test` — Unit / integration / system test
- `code review` — Peer review, architecture review
- `static analysis` — TICS, CodeScene, SonarQube
- `integration test` — Cross-module integration verification
- `system test` — End-to-end system verification
- `field monitoring` — Post-market surveillance data

## Important Rules

- **This is a draft generator** — output is intended as a starting point for DFMEA review sessions, not as a final DFMEA document.
- Always state that the results require expert review by the DFMEA team (architects, system engineers, quality engineers).
- Use conservative (higher) SOD scores when uncertain — it is safer to over-estimate risk.
- Do not fabricate RMM IDs — use `TBD` or `N/A` and note that real RMM IDs must be assigned by the risk management process.
- Include at least 2 failure modes per requirement (functional failure + most likely secondary failure).
- For Safety-classified items (Severity = S), always recommend RMM consideration.
- Fill the HTML report with actual analysis data — do not output a template with placeholders.
- The report must be fully self-contained (inline CSS, inline SVG, inline JS).

## Expected Output

A **self-contained HTML report** (light theme) following the [report template](./references/report-template.html), containing:

Default save location: workspace root `DFMEA/` directory, with filename `{subsystem}_dfmea_report.html` unless the user explicitly requests another path.

1. **Hero banner**: Subsystem name, requirement scope, date, analyst info
2. **Executive Summary**: Total requirements analyzed, failure modes identified, risk distribution (Safety/High/Medium/Low)
3. **SOD Criteria Reference**: Embedded severity, occurrence, and detection rating scales
4. **DFMEA Analysis Table**: Full table with all columns per the template — this is the main deliverable
5. **Criticality Matrix**: Visual matrix showing where each failure mode falls
6. **Risk Statistics**: Pie/bar charts showing risk distribution
7. **Recommendations**: Prioritized actions (Must-address / Should-address / Consider)
8. **Review Notes**: Items flagged for expert review, assumptions made, data gaps

## Related Skills

- [Architecture](../architecture/SKILL.md) — for design context and dependency analysis
- [Code Quality](../code-quality/SKILL.md) — for code-level risk assessment
- [Doc Generator](../doc-generator/SKILL.md) — for SDS and DFMEA/Impact Analysis documentation

## References

- [SOD Criteria](./references/sod-criteria.md)
- [Criticality Matrix](./references/criticality-matrix.md)
- [DFMEA Column Guide](./references/column-guide.md)
- [Report Template](./references/report-template.html)
- **Historical DFMEA reference documents** — auto-discover in this order:
  1. `DFMEA/` folder at the **workspace root** (most teams keep historical DFMEAs there)
  2. Any folder matching `**/DFMEA/` within the workspace (use `file_search`)
  3. Path recorded in `/memories/repo/dfmea-reference-path.md` (per-workspace override)
  4. If none found, proceed without historical context and note this in the report's "Review Notes" section

## Portability Note

This skill is **team-portable**. The DFMEA column structure, SOD scale, criticality matrix, and HTML report template follow the Philips CT DFMEA convention but are valid templates for any IEC 62304 / ISO 14971 risk analysis. Domain failure-mode seeds come from the separate `domain-knowledge` skill — another team's domain skill provides different seeds automatically. Team-specific items (actual RMM IDs, historical DFMEA file locations, project-specific failure patterns) must NOT be hardcoded here — store them in `references/` (versioned) or `/memories/repo/` (per-workspace).
