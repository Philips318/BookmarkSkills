# domain-knowledge — Changelog

跟踪对任何 `references/*.md` 的 material edits。每次添加 entry 时，都要 bump `SKILL.md` 中的 `domain-version` field（`YYYY.MM`）。

发布新版本时，应重新验证依赖 specific anchors 的 downstream skills：

- `bdd-generator` — Domain-Anchored Examples table values
- `test-generator` — Domain-Anchored Boundary Values table
- `requirements` / `requirements-review` — terminology and safety triggers
- `architecture` — Domain-Anchored Examples
- `code-review` — Domain Accuracy cross-cut
- `dfmea-analysis` — failure-mode seeds
- `ifu-generator` — canonical warning text
- `doc-generator` — terminology

---

## 2026.05 — Initial versioned release

Team-portable agent/skill rollout 的 baseline。References 已稳定：

- `ct-glossary.md` — formal CT/DICOM terminology、units、value ranges、8 patient positions
- `dicom-patterns.md` — 8 common DICOM pitfalls（Implicit VR + private tags、MONO1/MONO2、slice thickness vs spacing、RescaleSlope/Intercept、transfer syntax 等）
- `spectral-knowledge.md` — MonoE keV 40–200 range、max 4 concurrent results、iodine accuracy 5 mg/ml、SBI version gates、non-HU RGB rule、EFOV 500 mm limit
- `clinical-workflow.md` — 11-step scanner workflow、ISP module structure、user roles
- `safety-rules.md` — IEC 62304 A/B/C triggers；red lines：lossy compression on diagnostic、mammography、spectral quantification、pediatric exam cards、wrong-patient PHI、Citrix degraded mode

## Template for future entries

```
## YYYY.MM — One-line summary

### Added
- file.md — what was added and why

### Changed
- file.md — what changed; **breaking?** yes/no; downstream skills to re-validate: [list]

### Removed
- file.md — what was removed and the migration note
```

## Rules

- 对任何 material change（new anchor、changed value range、new safety red line）都要 bump `domain-version`。拼写修复不 bump。
- 如果 change 是 **breaking**（例如 keV range 从 40–200 变为 35–210），指出每个必须更新 anchor table 的 downstream skill。
- 最新 entry 放在最上方。
