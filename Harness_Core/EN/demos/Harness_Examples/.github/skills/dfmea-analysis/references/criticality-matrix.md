# Criticality Matrix — Design FMEA

The Criticality Matrix determines the risk classification of each failure mode based on Severity × Occurrence (S×O).

---

## Risk Classification Levels

| Level | Color | Criteria | Action Required |
|-------|-------|----------|-----------------|
| **Safety** | 🔴 Red | Severity = S (any Occurrence) | Requirement/mitigation/design decision; Consider CTS. Additional mitigation efforts required. Refer to Hazard List. |
| **High** | 🟠 Orange | S×O ≥ 30 (e.g., 8×5, 8×8, 8×10) | Requirement/mitigation/design decision is **CTQ**. Additional mitigation efforts / lower-level requirements needed. RMM required. |
| **Medium** | 🟡 Yellow | S×O = 15–25 (e.g., 5×5, 8×3, 5×3 with high RPN) | Requirement/mitigation/design decision; **Consider CTQ**. Classification and additional mitigation efforts should be considered. |
| **Low** | 🟢 Green | S×O < 15 (e.g., 5×1, 3×3, 1×any) | Non-critical requirement/mitigation/design decision. Failure has been acceptably controlled. |

---

## Criticality Matrix — Initial State

```
              Occurrence
Sev    1      3      5      8      10
 S   │  S   │  S   │  S   │  S   │  S   │  ← Safety (always)
 8   │  8   │ 24   │ 40   │ 64   │ 80   │
 5   │  5   │ 15   │ 25   │ 40   │ 50   │
 3   │  3   │  9   │ 15   │ 24   │ 30   │
 1   │  1   │  3   │  5   │  8   │ 10   │
```

### Color coding:

| S×O Value | Classification |
|-----------|---------------|
| Sev = S | **Safety** (red) — regardless of O |
| ≥ 30 | **High** (orange) — CTQ / RMM required |
| 15 – 25 | **Medium** (yellow) — Consider CTQ |
| < 15 | **Low** (green) — Acceptable |

---

## Classification Decision Rules

### When Classification = `RMM`
- Criticality ≥ 30 **and** Severity is S or 8
- An RMM ID must be assigned (e.g., `CT-NM.RMM-P2-RMM.xxxx`)
- RMM Severity of Harm must be evaluated (`S1` or `S2`)
- CTS level must be confirmed

### When Classification = `CTQ` or `Consider CTQ`
- Criticality is in the medium range (15–25)
- Design decision should be treated as quality-critical
- Lower-level requirement may be needed to mitigate

### When Classification = `N/A`
- Criticality < 15
- Risk is acceptably controlled by current design
- Optimized state should confirm ≤ initial criticality

---

## Criticality Matrix — Optimized State

After mitigation actions, the Criticality Matrix is recalculated. The goal is:

1. **Reduce Occurrence** — through better prevention controls, design changes, or redundancy
2. **Reduce Detection** — through better test coverage, monitoring, or verification
3. **Severity typically does not change** — unless the design fundamentally changes the failure's impact

The optimized state should show:
- No remaining **Safety** items without RMM
- No remaining **High** items without explicit mitigation actions
- All **RMM** items have assigned RMM IDs and severity classification
