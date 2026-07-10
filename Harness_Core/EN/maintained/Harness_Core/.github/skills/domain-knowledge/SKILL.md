---
name: domain-knowledge
description: 'CT/DICOM/Spectral domain knowledge base for writing professional medical device software requirements. Covers DICOM tags, CT geometry, Spectral CT workflows, clinical workflows, patient safety rules, and common engineering pitfalls.'
argument-hint: 'Describe a requirement or feature involving CT imaging, DICOM, or Spectral CT — the skill provides relevant domain context'
user-invocable: true
domain-version: 2026.05
---

# CT Domain Knowledge

Use this skill when analyzing or structuring requirements for CT medical device software.
It provides domain-specific context that makes requirements more precise and professional.

> **Domain version:** `2026.05`. When this version changes (see [CHANGELOG.md](./CHANGELOG.md)), downstream skills that depend on specific anchors (`bdd-generator`, `test-generator`, `requirements`, `requirements-review`, `architecture`, `code-review`, `dfmea-analysis`, `ifu-generator`, `doc-generator`) should be re-validated against the new content. The version follows `YYYY.MM` of the last material edit to any `references/*.md`.

## When to Use

- Writing requirements involving **DICOM** tags, storage, or network operations
- Writing requirements involving **CT image display** (windowing, pixel mapping, orientation)
- Writing requirements involving **Spectral CT** (MonoE, VNC, Iodine Map, Effective-Z)
- Writing requirements involving **CT geometry** (FOV, pixel spacing, slice thickness, IPP/IOP)
- Checking **patient safety** implications of a requirement
- Reviewing requirements for **domain accuracy** (correct terminology, valid value ranges)

## Knowledge Domains

| Domain | Reference File | Use For |
|--------|---------------|---------|
| CT Terminology | [ct-glossary.md](./references/ct-glossary.md) | Correct abbreviations, units, value ranges |
| DICOM Patterns | [dicom-patterns.md](./references/dicom-patterns.md) | Tag usage, VR types, transfer syntax, common pitfalls |
| Spectral CT | [spectral-knowledge.md](./references/spectral-knowledge.md) | SBI, MonoE, VNC, result types, keV, material decomposition |
| Clinical Workflows | [clinical-workflow.md](./references/clinical-workflow.md) | Scanner workflow, viewing workflow, user roles |
| Safety Rules | [safety-rules.md](./references/safety-rules.md) | Patient safety, dose, data integrity, regulatory red lines |

## How to Apply

When analyzing a requirement:

1. **Identify domain** — which knowledge area is relevant?
2. **Check terminology** — are the correct DICOM tags, units, and terms used?
3. **Validate value ranges** — are numeric thresholds clinically realistic?
4. **Flag safety concerns** — does the feature touch patient data, dose, or diagnostic accuracy?
5. **Add domain-specific AC** — include boundary conditions from domain knowledge (e.g., WW must be > 0, keV range 40-200)
6. **Cross-reference** — check if the requirement aligns with IFU/SSRS conventions

## Domain-Triggered Checks

When a requirement mentions any of these, automatically apply the corresponding check:

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
