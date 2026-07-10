---
name: dfmea-analysis
description: '为 CT medical device subsystems 生成 DFMEA（Design Failure Mode and Effects Analysis）。输入 SRS/SSRS requirements，产出包含 Failure Modes、Effects、Causes、SOD scoring、Risk Classification 和 Mitigation actions 的 structured DFMEA analysis，并以 self-contained HTML report 交付。'
argument-hint: 'Paste SRS/SSRS requirements text, or specify a requirements document path / subsystem name'
user-invocable: true
---

# DFMEA Analysis

使用此 skill 为 CT medical device subsystems 执行 Design Failure Mode and Effects Analysis（DFMEA）。给定 SRS/SSRS requirement content 后，它会按 Philips CT DFMEA template format 生成 DFMEA entries，并以 interactive HTML report 交付结果。

## When to Use

- 你有 SRS/SSRS requirements，需要生成 initial DFMEA entries 供 review。
- 准备 DFMEA session，希望 draft analysis 作为 starting point。
- 需要为 new feature 或 subsystem 识别 potential failure modes、effects 和 causes。
- 希望评估 risk（SOD scoring），并判断是否需要 Risk Mitigation Measures（RMM）。
- 正在用 new requirements 扩展 existing DFMEA。

## Business Context

此 skill 面向 **Philips CT medical device** development，覆盖但不限于以下 subsystems：

- **Console Software** — Post-processing applications（MPR、3D、DE、Rib、Spine、Endo 等）
- **CT Gantry** — Rotation、collimation、X-ray tube control
- **CT Bed (Table)** — Patient positioning、movement control
- **CT Image Reconstruction System** — Reconstruction algorithms、calibration
- **iWorkflow** — Scan planning、protocol management、patient workflow
- **Direct Result Pipeline** — Automated result generation、DICOM handling

DFMEA process 遵循 **IEC 62304**（Medical Device Software Lifecycle）、**ISO 14971**（Risk Management for Medical Devices）和 Philips internal DFMEA procedures。

## Related Skill: domain-knowledge

生成 DFMEA entries 前，查阅 `domain-knowledge` skill 以识别 domain-specific failure modes 和 safety red lines：

- **safety-rules.md** — IEC 62304 A/B/C classification triggers、radiation/data integrity/spectral/ISP safety red lines（把这些作为 Failure Effects 的 **seeds**）
- **dicom-patterns.md** — 8 个常见 DICOM pitfalls（Implicit VR + private tags、MONO1 vs MONO2、slice spacing vs thickness 等）— 每个都是 known failure mode
- **spectral-knowledge.md** — spectral constraints（max 4 concurrent MonoE、SBI version gates、non-HU RGB rule）— violations 是 failure modes
- **ct-glossary.md** — correct units 和 value ranges；使用 wrong units（HU vs mg/ml）是 Failure Effect

这确保 DFMEA entries 立足于 known domain-specific failure modes，而不是 generic templates。

## Preferred Inputs

提供以下一项或多项：

- **SRS/SSRS requirement text**: 一个或多个带 ID 的 requirements（例如 `CT-NMP.SSRS.Console.7636 The rib application shall support to extract rib centerline with labeling`）
- **Subsystem / Feature name**: 例如 "Rib Application"、"Spectral DE Workflow"、"MPR Orientation Reset"
- **Existing DFMEA context**: 如果扩展 existing DFMEA，提供 current entries 或 document reference
- **Architectural context**: Design documents、SDS references、component descriptions

如果没有明确 input，skill 会 prompt for requirement content。

## DFMEA Column Structure

每个 DFMEA entry 都遵循 Philips CT DFMEA template，包含以下 columns：

### Identification

| 列 | 描述 |
|--------|-------------|
| **ID** | Unique failure mode identifier（sequential or hierarchical，例如 `1`、`2`、`rib-1`） |
| **Item/Function** | 被分析的 subsystem/function（例如 `Rib APP/Display rib labeling`） |
| **Requirement** | SRS/SSRS requirement ID and text |
| **Classification of Higher Order Requirement** | `N/A`、`CTS`、`CTQ` 或 requirement class |

### Failure Analysis

| 列 | 描述 |
|--------|-------------|
| **Potential Failure Mode** | 可能出错的内容（例如 `FM01: The rib application fails to display the rib labeling`） |
| **Local Effect of Failure** | 对 subsystem 的 direct local impact |
| **End Effect of Failure** | 对 end user / patient / system 的影响。使用 standard effect phrases（见 [End Effect phrases](./references/sod-criteria.md#standard-end-effect-phrases)） |
| **Rationale** | 对 severity rating 和 user impact 的解释 |
| **Potential Cause / Mechanism of Failure** | Root cause 或 mechanism（例如 algorithm crash、data corruption、race condition） |

### Initial State (SOD Scoring)

| 列 | 描述 |
|--------|-------------|
| **Severity (S1)** | Severity rating: `S`、`8`、`5`、`3`、`1`（见 [SOD Criteria](./references/sod-criteria.md)） |
| **Prevention Control** | 防止 failure 的 design controls（design rules、architecture decisions、redundancy） |
| **Occurrence (O1)** | Occurrence rating: `10`、`8`、`5`、`3`、`1` |
| **Criticality (S×O)** | Calculated: Severity × Occurrence |
| **Classification** | Risk classification: `N/A`、`Consider CTQ`、`CTQ`、`RMM` |
| **Detection Control** | 如何检测 failure（software test、code review、integration test 等） |
| **Detection (D1)** | Detection rating: `10`、`8`、`5`、`3`、`1` |
| **RPN (S×O×D)** | Risk Priority Number = Severity × Occurrence × Detection |

### Risk Management

| 列 | 描述 |
|--------|-------------|
| **RMM ID** | Risk Mitigation Measure ID（例如 `CT-NM.RMM-P2-RMM.1104`）或 `N/A` |
| **RMM Severity of Harm** | Severity of harm classification: `S1`、`S2` 或 `N/A` |
| **RMM Confirmed CTS Level** | Confirmed CTS level: `CTS`、`No CTS` 或 `N/A` |

### Optimized State (After Mitigation)

| 列 | 描述 |
|--------|-------------|
| **Severity (S2)** | mitigation 后重新评估的 severity |
| **Prevention Action** | 采取的 additional prevention actions |
| **Occurrence (O2)** | mitigation 后重新评估的 occurrence |
| **Criticality (S×O) Optimized** | Recalculated criticality |
| **Classification Optimized** | 重新评估的 risk classification |
| **Detection Action** | 采取的 additional detection actions |
| **Detection (D2)** | mitigation 后重新评估的 detection rating |
| **RPN (S×O×D) Optimized** | Recalculated RPN |

### Metadata

| 列 | 描述 |
|--------|-------------|
| **Remarks** | Additional notes、references、version info |

## SOD Criteria

Severity、Occurrence 和 Detection ratings 遵循 Philips Design FMEA scale：

完整 rating scales 见 [SOD Criteria](./references/sod-criteria.md)。

**Quick reference:**

| 严重度 | 等级 | 含义 |
|----------|-------|---------|
| S (Safety) | Safety-related failure, refer to hazard list |
| 8 | 关键 | Loss of primary function (not safety) |
| 5 | 主要 | Convenience function inoperable |
| 3 | 次要 | Slight user dissatisfaction |
| 1 | 无影响 | User probably won't notice |

| Occurrence | 等级 | 含义 |
|------------|-------|---------|
| 10 | 频繁 | New technology, no history |
| 8 | 可能 | New design, likely failure |
| 5 | 很可能 | Occasional failures in similar design |
| 3 | 远程 | Isolated failures in identical design |
| 1 | 不太可能 | Eliminated through preventive control |

| Detection | 等级 | 含义 |
|-----------|-------|---------|
| 10 | 几乎不可能 | No test procedure capable |
| 8 | 远程 | Uncertain procedure, limited experience |
| 5 | 中等 | Proven procedure, new usage |
| 3 | 高 | Physical testing, high confidence |
| 1 | 几乎确定 | Proven standards, prevents failure |

## Criticality Matrix

完整 matrix 见 [Criticality Matrix](./references/criticality-matrix.md)。

| 分类 | Criticality (S×O) | 需要措施 |
|---------------|-------------------|-----------------|
| **Safety** | S × any O | Consider CTS; Additional mitigation efforts |
| **RMM** | ≥ 30 (e.g., S×10, 8×5, etc.) | Requirement/design decision is CTQ |
| **Consider CTQ** | 15–25 | Consider CTQ; Additional review |
| **N/A** | < 15 | Non-critical; Failure acceptably controlled |

## Workflow

1. **Parse requirements**: 从 user input 中提取 requirement IDs、text 和 subsystem context。
2. **Identify Item/Function**: 确定被分析的 subsystem 和 function。
3. **Generate failure modes**: 对每个 requirement，识别 potential failure modes：
   - Consider: functional failure、incorrect output、performance degradation、data corruption、timing issues、UI errors
   - 使用 naming convention：`FM01:`、`FM02:` 等。
4. **Analyze effects**: 对每个 failure mode，确定：
   - Local effect（subsystem 中发生什么）
   - End effect（user/patient 经历什么）
   - 使用来自 SOD criteria 的 standard end effect phrases
5. **Identify causes**: 确定 root causes 和 mechanisms：
   - Algorithm failures、data validation gaps、race conditions、resource exhaustion
   - Component crashes、communication failures、configuration errors
6. **Score SOD (Initial State)**: 基于以下内容应用 severity、occurrence 和 detection ratings：
   - 既有 design architecture 和 controls
   - 来自 similar subsystems 的 historical failure data
   - 可用 test coverage
7. **Classify risk**: 基于 criticality（S×O）和 criticality matrix：
   - 判断是否需要 RMM
   - 识别 CTS/CTQ classifications
8. **Propose mitigations**: 对需要 action 的 items：
   - Prevention actions（design changes、additional controls）
   - Detection actions（additional tests、monitoring）
9. **Score SOD (Optimized State)**: proposed mitigations 后重新评估 SOD。
10. **Build HTML report**: 使用 [report template](./references/report-template.html) 生成 report：
    - Executive summary with risk statistics
    - SOD criteria reference section
    - Full DFMEA table with all columns
    - Criticality matrix visualization
    - Risk distribution charts
    - Actionable recommendations
11. **Save the HTML file**: 写入 workspace root 下的 `DFMEA/{subsystem}_dfmea_report.html`（如需要创建 `DFMEA` directory），除非 user 明确指定其他 output path。

## Analysis Guidelines

### Failure Mode Identification Patterns

对 **software** subsystems，考虑这些 common failure mode categories：

| 类别 | Example Failure Modes |
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

End Effect column 使用这些 standard phrases（来自 SOD severity scale）：

- `Loss of primary function` (Severity = 8)
- `Temporary loss of primary function` (Severity = 8)
- `Degradation of primary function` (Severity = 8)
- `Function / Incorrect image or content` (Severity = S for safety)
- `convenience function inoperable` (Severity = 5)
- `temporary loss of convenience function` (Severity = 5)
- `Slight user dissatisfaction` (Severity = 3)
- `No discernible effect` (Severity = 1)

### Prevention Control Patterns

CT software 的 common prevention controls：

- Independent process design（crash isolation）
- Input validation and boundary checking
- Error notification to user
- Fallback / manual workaround available
- Architecture review / design documents
- Standards compliance（IEC 62304、DICOM 等）
- Defensive coding（null checks、error handling）

### Detection Control Patterns

Common detection controls：

- `software test` — Unit / integration / system test
- `code review` — Peer review、architecture review
- `static analysis` — TICS、CodeScene、SonarQube
- `integration test` — Cross-module integration verification
- `system test` — End-to-end system verification
- `field monitoring` — Post-market surveillance data

## Important Rules

- **This is a draft generator** — output intended as DFMEA review sessions 的 starting point，不是 final DFMEA document。
- 始终说明 results 需要 DFMEA team（architects、system engineers、quality engineers）进行 expert review。
- 不确定时使用 conservative（higher）SOD scores — over-estimate risk 更安全。
- 不要 fabricate RMM IDs — 使用 `TBD` 或 `N/A`，并说明真实 RMM IDs 必须由 risk management process 分配。
- 每个 requirement 至少包含 2 个 failure modes（functional failure + most likely secondary failure）。
- 对 Safety-classified items（Severity = S），始终 recommend RMM consideration。
- 用 actual analysis data 填充 HTML report — 不要输出带 placeholders 的 template。
- Report 必须 fully self-contained（inline CSS、inline SVG、inline JS）。

## Expected Output

一个遵循 [report template](./references/report-template.html) 的 **self-contained HTML report**（light theme），包含：

Default save location: workspace root `DFMEA/` directory，filename 为 `{subsystem}_dfmea_report.html`，除非 user 明确请求其他 path。

1. **Hero banner**: Subsystem name、requirement scope、date、analyst info
2. **Executive Summary**: Total requirements analyzed、failure modes identified、risk distribution（Safety/High/Medium/Low）
3. **SOD Criteria Reference**: Embedded severity、occurrence 和 detection rating scales
4. **DFMEA Analysis Table**: 按 template 包含 all columns 的 full table — 这是 main deliverable
5. **Criticality Matrix**: 显示每个 failure mode 落点的 visual matrix
6. **Risk Statistics**: 显示 risk distribution 的 pie/bar charts
7. **Recommendations**: Prioritized actions（Must-address / Should-address / Consider）
8. **Review Notes**: flagged for expert review 的 items、assumptions made、data gaps

## Related Skills

- [Architecture](../architecture/SKILL.md) — for design context and dependency analysis
- [Code Quality](../code-quality/SKILL.md) — for code-level risk assessment
- [Doc Generator](../doc-generator/SKILL.md) — for SDS and DFMEA/Impact Analysis documentation

## References

- [SOD Criteria](./references/sod-criteria.md)
- [Criticality Matrix](./references/criticality-matrix.md)
- [DFMEA Column Guide](./references/column-guide.md)
- [Report Template](./references/report-template.html)
- **Historical DFMEA reference documents** — 按以下顺序 auto-discover：
  1. workspace root 的 `DFMEA/` folder（多数 teams 将 historical DFMEAs 放在那里）
  2. workspace 内任何匹配 `**/DFMEA/` 的 folder（使用 `file_search`）
  3. `/memories/repo/dfmea-reference-path.md` 中记录的 path（per-workspace override）
  4. 如果都未找到，则不带 historical context 继续，并在 report 的 "Review Notes" section 中说明

## Portability Note

此 skill 是 **team-portable**。DFMEA column structure、SOD scale、criticality matrix 和 HTML report template 遵循 Philips CT DFMEA convention，但也可作为任何 IEC 62304 / ISO 14971 risk analysis 的 valid templates。Domain failure-mode seeds 来自单独的 `domain-knowledge` skill — 其他 team 的 domain skill 会自动提供不同 seeds。Team-specific items（actual RMM IDs、historical DFMEA file locations、project-specific failure patterns）不得 hardcode 在这里 — 存放到 `references/`（versioned）或 `/memories/repo/`（per-workspace）。
