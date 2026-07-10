---
name: domain-knowledge
description: '用于编写专业 medical device software requirements 的 CT/DICOM/Spectral domain knowledge base。覆盖 DICOM tags、CT geometry、Spectral CT workflows、clinical workflows、patient safety rules 和 common engineering pitfalls。'
argument-hint: 'Describe a requirement or feature involving CT imaging, DICOM, or Spectral CT — the skill provides relevant domain context'
user-invocable: true
domain-version: 2026.05
---

# CT Domain Knowledge

分析或结构化 CT medical device software requirements 时使用此 skill。它提供 domain-specific context，使 requirements 更 precise、更 professional。

> **Domain version:** `2026.05`。当此版本变化时（见 [CHANGELOG.md](./CHANGELOG.md)），依赖 specific anchors 的 downstream skills（`bdd-generator`、`test-generator`、`requirements`、`requirements-review`、`architecture`、`code-review`、`dfmea-analysis`、`ifu-generator`、`doc-generator`）应根据新内容重新验证。版本号遵循任何 `references/*.md` 发生 material edit 的 `YYYY.MM`。

## When to Use

- 编写涉及 **DICOM** tags、storage 或 network operations 的 requirements
- 编写涉及 **CT image display**（windowing、pixel mapping、orientation）的 requirements
- 编写涉及 **Spectral CT**（MonoE、VNC、Iodine Map、Effective-Z）的 requirements
- 编写涉及 **CT geometry**（FOV、pixel spacing、slice thickness、IPP/IOP）的 requirements
- 检查 requirement 的 **patient safety** implications
- 审查 requirements 的 **domain accuracy**（correct terminology、valid value ranges）

## Knowledge Domains

| 领域 | 参考文件 | 用于 |
|--------|---------------|---------|
| CT 术语 | [ct-glossary.md](./references/ct-glossary.md) | 正确缩写、单位、取值范围 |
| DICOM 模式 | [dicom-patterns.md](./references/dicom-patterns.md) | Tag 用法、VR 类型、transfer syntax、常见陷阱 |
| Spectral CT | [spectral-knowledge.md](./references/spectral-knowledge.md) | SBI、MonoE、VNC、result types、keV、material decomposition |
| Clinical Workflows | [clinical-workflow.md](./references/clinical-workflow.md) | Scanner workflow, viewing workflow, user roles |
| Safety Rules | [safety-rules.md](./references/safety-rules.md) | Patient safety, dose, data integrity, regulatory red lines |

## How to Apply

分析 requirement 时：

1. **Identify domain** — 哪个 knowledge area 相关？
2. **Check terminology** — 是否使用了正确 DICOM tags、units 和 terms？
3. **Validate value ranges** — numeric thresholds 是否 clinically realistic？
4. **Flag safety concerns** — feature 是否触及 patient data、dose 或 diagnostic accuracy？
5. **Add domain-specific AC** — 纳入来自 domain knowledge 的 boundary conditions（例如 WW must be > 0，keV range 40-200）
6. **Cross-reference** — 检查 requirement 是否符合 IFU/SSRS conventions

## Domain-Triggered Checks

当 requirement 提到以下内容时，自动应用对应 check：

| Trigger | Check to Apply |
|---------|---------------|
| "DICOM tag" or tag number | → Verify VR, VM, correct usage from dicom-patterns.md |
| "window" / "WW" / "WC" | → WW > 0, display pipeline rules, MONO1 vs MONO2 |
| "SUV" / "PET" | → Decay time correction, DICOM tag priority |
| "pixel spacing" / "FOV" | → PixelSpacing = FOV/Rows, FOV type consistency |
| "orientation" / "IOP" / "IPP" | → Patient position table, coordinate system (LPS) |
| "spectral" / "MonoE" / "VNC" | → SBI version gates, keV range, HU vs non-HU units |
| "iodine" / "calcium" / "Z eff" | → Material decomposition, result type availability |
| "slice" / "thickness" / "spacing" | → SliceThickness vs SpacingBetweenSlices distinction |
| "transfer syntax" / "compression" | → Lossless requirement for post-processing |
| "C-STORE" / "C-FIND" / "C-MOVE" | → Association/Presentation Context rules |
| "private tag" | → Explicit VR requirement, anonymization impact |
| "patient" / "dose" / "radiation" | → Safety classification escalation |
| "export" / "save" / "archive" | → DICOM conformance, non-HU RGB warning |
| "ISP" / "Portal" / "client-server" | → Multi-user constraints, version matching, lossy compression rules |
| "perfusion" / "CBV" / "CBF" | → Vessel definition, mask validation, traffic lights |
| "clipboard" / "screenshot" | → PHI leakage → clear clipboard warning |
