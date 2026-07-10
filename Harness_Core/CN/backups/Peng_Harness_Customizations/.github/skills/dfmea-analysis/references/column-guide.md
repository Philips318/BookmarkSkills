# DFMEA Column Guide

Philips CT DFMEA template 中每一列的详细指南。

---

## Column Definitions

### A. Identification Columns

| # | 列 | 描述 | 示例 |
|---|--------|-------------|---------|
| C1 | **ID** | 唯一标识符。可以是顺序编号或 hierarchical（例如 `1`、`2`、`rib-1`、`6-1`）。Sub-items 使用 parent ID prefix。 | `1`, `rib-1` |
| C2 | **Item/Function** | 被分析的 subsystem 和 function。Format: `Subsystem/Function` | `Rib APP/Display rib labeling` |
| C3 | **Requirement** | 完整 SRS/SSRS requirement ID + text。 | `CT-NMP.SSRS.Console.7636 The rib application shall support to extract rib centerline with labeling` |
| C4 | **Classification of Higher Order Requirement** | Parent requirement 的 classification：`N/A`、`CTS`、`CTQ` 或 specific class。 | `N/A`, `CTS`, `CTQ` |

### B. Failure Analysis Columns

| # | 列 | 描述 | 示例 |
|---|--------|-------------|---------|
| C5 | **Potential Failure Mode** | 可能出错的内容。以 `FM01:`、`FM02:` 等作为前缀。每个 requirement 有多个 FMs 很常见。 | `FM01: The rib application fails to display the rib labeling` |
| C6 | **Local Effect of Failure** | subsystem 内部的 direct impact。描述 local 会发生什么。 | `The rib application does not provide rib application with rib centerline extraction` |
| C7 | **End Effect of Failure** | 对 end user 的影响。使用与 Severity level 匹配的 standard effect phrases。 | `convenience function inoperable` |
| C8 | **Rationale** | 对 user impact 的解释。描述 user 会经历什么，以及是否存在 workaround。 | `User needs to restart the rib application. User cannot directly get the rib labeling.` |
| C9 | **Potential Cause / Mechanism of Failure** | Technical root cause 或 failure mechanism。 | `The rib algorithm component crashes.` |

### C. Initial State — SOD Scoring

| # | 列 | 描述 | 取值 |
|---|--------|-------------|--------|
| C10 | **Severity (S1)** | end effect 的 severity。 | `S`, `8`, `5`, `3`, `1` |
| C11 | **Prevention Control** | 防止 cause 的 design controls。包含 design document references。 | `The rib application is designed as independent process. When crashed, it would not impact the scan process.` + `DHF369695 Architecture Rev H` |
| C12 | **Occurrence (O1)** | 在 prevention controls 下 cause 发生的 likelihood。 | `10`, `8`, `5`, `3`, `1` |
| C13 | **Criticality (S×O)** | Calculated: Severity × Occurrence。Severity = S 时写 `S × O`。 | `5`, `30`, `S×3` |
| C14 | **Classification** | 基于 criticality matrix 的 risk classification。 | `N/A`, `Consider CTQ`, `CTQ`, `RMM` |
| C15 | **Detection Control** | 检测 failure 的 test/verification。包含 test case IDs 和 verification document references。 | `software test` + `CaseID 57521` + `D001049716 Verification Record Rev A` |
| C16 | **Detection (D1)** | 在 detection controls 下的 detection likelihood。 | `10`, `8`, `5`, `3`, `1` |
| C17 | **RPN (S×O×D)** | Risk Priority Number = S × O × D。 | `15`, `90`, etc. |

### D. Risk Management Classification

| # | 列 | 描述 | 取值 |
|---|--------|-------------|--------|
| C18 | **RMM ID** | Risk Mitigation Measure identifier。由 RM process 分配。 | `CT-NM.RMM-P2-RMM.1104`, `N/A`, `TBD` |
| C19 | **RMM Severity of Harm** | risk management 中的 severity classification。 | `S1`, `S2`, `N/A` |
| C20 | **RMM Confirmed CTS Level** | Confirmed CTS（Critical To Safety）level。 | `CTS`, `No CTS`, `N/A` |

### E. Optimized State — After Mitigation

| # | 列 | 描述 |
|---|--------|-------------|
| C21 | **Severity (S2)** | mitigation 后重新评估的 severity（除非 design fundamentally changes，通常与 S1 相同）。 |
| C22 | **Prevention Action** | Additional prevention actions。如果 initial controls 已足够，写 `No further action needed`。 |
| C23 | **Occurrence (O2)** | additional prevention 后重新评估的 occurrence。 |
| C24 | **Criticality (S×O) Optimized** | Recalculated: S2 × O2。 |
| C25 | **Classification Optimized** | 重新评估的 risk classification。应与 initial 相同或更低。 |
| C26 | **Detection Action** | Additional detection actions。如果 initial detection 已足够，写 `No further action needed`。 |
| C27 | **Detection (D2)** | additional detection actions 后重新评估的 detection。 |
| C28 | **RPN (S×O×D) Optimized** | Recalculated RPN。应与 initial RPN 相同或更低。 |

### F. Metadata

| # | 列 | 描述 |
|---|--------|-------------|
| C29 | **Remarks** | Additional notes、open items、version references、review comments。 |

---

## Filling Guidelines

### ID Numbering
- 使用 sequential integers：`1`、`2`、`3`、...
- 对从 parent failure mode 派生的 sub-items，使用：`rib-1`、`rib-2` 或 `6-1`、`6-2`
- 当一个 requirement 有 lower-level requirement 来 mitigate failure mode 时，该 mitigation requirement 获得 sub-ID

### Item/Function Format
- Format: `Subsystem/Function` 或 `Subsystem/Sub-function`
- Examples:
  - `Rib APP/Display rib labeling`
  - `DE/Weighted Image`
  - `Spine Workflow/Detect Spine`
  - `MPR/Auto Orientation`

### Failure Mode Naming
- 在单个 requirement 内始终以 `FM01:`、`FM02:` 等作为前缀
- 要具体：描述 *what* fails，而不是 *why*
- Good: `FM01: The DE application fails to generate Weighted Image`
- Bad: `FM01: Software bug`

### Prevention Control Content
- 描述防止 failure 的 design decision 或 architecture
- 用 document number 和 revision 引用 design document（SDS、Architecture doc）
- Example: `The rib application is designed as independent process. When crashed, it would not impact the scan process.\n\nDHF369695 Incisive Host Post Processing Platform Architecture Rev H`

### Detection Control Content
- 描述 test type：`software test`、`code review`、`integration test`、`system test`
- 包含 test case IDs 和 verification record references
- Example: `software test\nCaseID 57521\nD001049716 Verification Record Rev A`

### When to Use "No further action needed"
- 在 Optimized State columns（C22、C26）中，当以下情况成立时写 `No further action needed`：
  - Initial state controls 已足够
  - criticality 已经是 Low（< 15）
  - additional mitigation 不实际或成本效益不足
