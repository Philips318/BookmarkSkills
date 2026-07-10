# DFMEA Column Guide

Detailed guide for each column in the Philips CT DFMEA template.

---

## Column Definitions

### A. Identification Columns

| # | Column | Description | Example |
|---|--------|-------------|---------|
| C1 | **ID** | Unique identifier. Sequential number or hierarchical (e.g., `1`, `2`, `rib-1`, `6-1`). Sub-items use parent ID prefix. | `1`, `rib-1` |
| C2 | **Item/Function** | Subsystem and function being analyzed. Format: `Subsystem/Function` | `Rib APP/Display rib labeling` |
| C3 | **Requirement** | Full SRS/SSRS requirement ID + text. | `CT-NMP.SSRS.Console.7636 The rib application shall support to extract rib centerline with labeling` |
| C4 | **Classification of Higher Order Requirement** | Classification from parent requirement: `N/A`, `CTS`, `CTQ`, or specific class. | `N/A`, `CTS`, `CTQ` |

### B. Failure Analysis Columns

| # | Column | Description | Example |
|---|--------|-------------|---------|
| C5 | **Potential Failure Mode** | What could go wrong. Prefix with `FM01:`, `FM02:`, etc. Multiple FMs per requirement are common. | `FM01: The rib application fails to display the rib labeling` |
| C6 | **Local Effect of Failure** | Direct impact within the subsystem. Describe what happens locally. | `The rib application does not provide rib application with rib centerline extraction` |
| C7 | **End Effect of Failure** | Impact on the end user. Use standard effect phrases matching the Severity level. | `convenience function inoperable` |
| C8 | **Rationale** | Explanation of user impact. Describes what the user experiences and what workaround exists. | `User needs to restart the rib application. User cannot directly get the rib labeling.` |
| C9 | **Potential Cause / Mechanism of Failure** | Technical root cause or failure mechanism. | `The rib algorithm component crashes.` |

### C. Initial State — SOD Scoring

| # | Column | Description | Values |
|---|--------|-------------|--------|
| C10 | **Severity (S1)** | Severity of the end effect. | `S`, `8`, `5`, `3`, `1` |
| C11 | **Prevention Control** | Design controls that prevent the cause. Include design document references. | `The rib application is designed as independent process. When crashed, it would not impact the scan process.` + `DHF369695 Architecture Rev H` |
| C12 | **Occurrence (O1)** | Likelihood of the cause occurring given prevention controls. | `10`, `8`, `5`, `3`, `1` |
| C13 | **Criticality (S×O)** | Calculated: Severity × Occurrence. For Severity = S, write `S × O`. | `5`, `30`, `S×3` |
| C14 | **Classification** | Risk classification based on criticality matrix. | `N/A`, `Consider CTQ`, `CTQ`, `RMM` |
| C15 | **Detection Control** | Test/verification that detects the failure. Include test case IDs and verification document references. | `software test` + `CaseID 57521` + `D001049716 Verification Record Rev A` |
| C16 | **Detection (D1)** | Likelihood of detection given detection controls. | `10`, `8`, `5`, `3`, `1` |
| C17 | **RPN (S×O×D)** | Risk Priority Number = S × O × D. | `15`, `90`, etc. |

### D. Risk Management Classification

| # | Column | Description | Values |
|---|--------|-------------|--------|
| C18 | **RMM ID** | Risk Mitigation Measure identifier. Assigned by RM process. | `CT-NM.RMM-P2-RMM.1104`, `N/A`, `TBD` |
| C19 | **RMM Severity of Harm** | Severity classification in risk management. | `S1`, `S2`, `N/A` |
| C20 | **RMM Confirmed CTS Level** | Confirmed CTS (Critical To Safety) level. | `CTS`, `No CTS`, `N/A` |

### E. Optimized State — After Mitigation

| # | Column | Description |
|---|--------|-------------|
| C21 | **Severity (S2)** | Re-assessed severity (usually same as S1 unless design fundamentally changes). |
| C22 | **Prevention Action** | Additional prevention actions. `No further action needed` if initial controls are sufficient. |
| C23 | **Occurrence (O2)** | Re-assessed occurrence after additional prevention. |
| C24 | **Criticality (S×O) Optimized** | Recalculated: S2 × O2. |
| C25 | **Classification Optimized** | Re-assessed risk classification. Should be same or lower than initial. |
| C26 | **Detection Action** | Additional detection actions. `No further action needed` if initial detection is sufficient. |
| C27 | **Detection (D2)** | Re-assessed detection after additional detection actions. |
| C28 | **RPN (S×O×D) Optimized** | Recalculated RPN. Should be same or lower than initial RPN. |

### F. Metadata

| # | Column | Description |
|---|--------|-------------|
| C29 | **Remarks** | Additional notes, open items, version references, review comments. |

---

## Filling Guidelines

### ID Numbering
- Use sequential integers: `1`, `2`, `3`, ...
- For sub-items derived from a parent failure mode, use: `rib-1`, `rib-2` or `6-1`, `6-2`
- When a requirement has a lower-level requirement that mitigates a failure mode, the mitigation requirement gets a sub-ID

### Item/Function Format
- Format: `Subsystem/Function` or `Subsystem/Sub-function`
- Examples:
  - `Rib APP/Display rib labeling`
  - `DE/Weighted Image`
  - `Spine Workflow/Detect Spine`
  - `MPR/Auto Orientation`

### Failure Mode Naming
- Always prefix with `FM01:`, `FM02:`, etc. within a single requirement
- Be specific: describe *what* fails, not *why*
- Good: `FM01: The DE application fails to generate Weighted Image`
- Bad: `FM01: Software bug`

### Prevention Control Content
- Describe the design decision or architecture that prevents the failure
- Reference the design document (SDS, Architecture doc) by document number and revision
- Example: `The rib application is designed as independent process. When crashed, it would not impact the scan process.\n\nDHF369695 Incisive Host Post Processing Platform Architecture Rev H`

### Detection Control Content
- Describe the test type: `software test`, `code review`, `integration test`, `system test`
- Include test case IDs and verification record references
- Example: `software test\nCaseID 57521\nD001049716 Verification Record Rev A`

### When to Use "No further action needed"
- In the Optimized State columns (C22, C26), write `No further action needed` when:
  - The initial state controls are sufficient
  - The criticality is already Low (< 15)
  - No additional mitigation is practical or cost-effective
