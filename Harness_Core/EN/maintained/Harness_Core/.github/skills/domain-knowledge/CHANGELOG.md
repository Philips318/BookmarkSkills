# domain-knowledge — Changelog

Track material edits to any `references/*.md`. Bump the `domain-version` field in `SKILL.md` (`YYYY.MM`) whenever an entry is added here.

When a new version is published, downstream skills that depend on specific anchors should be re-validated:

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

Baseline for the team-portable agent/skill rollout. References stabilized:

- `ct-glossary.md` — formal CT/DICOM terminology, units, value ranges, 8 patient positions
- `dicom-patterns.md` — 8 common DICOM pitfalls (Implicit VR + private tags, MONO1/MONO2, slice thickness vs spacing, RescaleSlope/Intercept, transfer syntax, etc.)
- `spectral-knowledge.md` — MonoE keV 40–200 range, max 4 concurrent results, iodine accuracy 5 mg/ml, SBI version gates, non-HU RGB rule, EFOV 500 mm limit
- `clinical-workflow.md` — 11-step scanner workflow, ISP module structure, user roles
- `safety-rules.md` — IEC 62304 A/B/C triggers; red lines: lossy compression on diagnostic, mammography, spectral quantification, pediatric exam cards, wrong-patient PHI, Citrix degraded mode

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

- Bump `domain-version` for any material change (new anchor, changed value range, new safety red line). Spelling fixes do not bump.
- If a change is **breaking** (e.g., keV range changes from 40–200 to 35–210), call out every downstream skill that must update its anchor table.
- Keep the latest entry at the top.
