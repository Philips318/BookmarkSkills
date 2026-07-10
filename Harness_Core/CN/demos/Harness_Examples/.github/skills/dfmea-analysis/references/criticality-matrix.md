# Criticality Matrix — Design FMEA

Criticality Matrix 根据 Severity × Occurrence（S×O）决定每个 failure mode 的 risk classification。

---

## Risk Classification Levels

| 等级 | 颜色 | 准则 | 需要措施 |
|-------|-------|----------|-----------------|
| **Safety** | 🔴 Red | Severity = S (any Occurrence) | Requirement/mitigation/design decision；Consider CTS。需要 additional mitigation efforts。Refer to Hazard List。 |
| **High** | 🟠 Orange | S×O ≥ 30（例如 8×5、8×8、8×10） | Requirement/mitigation/design decision 是 **CTQ**。需要 additional mitigation efforts / lower-level requirements。RMM required。 |
| **Medium** | 🟡 Yellow | S×O = 15–25（例如 5×5、8×3、5×3 with high RPN） | Requirement/mitigation/design decision；**Consider CTQ**。应考虑 classification 和 additional mitigation efforts。 |
| **Low** | 🟢 Green | S×O < 15（例如 5×1、3×3、1×any） | Non-critical requirement/mitigation/design decision。Failure 已被 acceptably controlled。 |

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

| S×O Value | 分类 |
|-----------|---------------|
| Sev = S | **Safety** (red) — regardless of O |
| ≥ 30 | **High** (orange) — CTQ / RMM required |
| 15 – 25 | **Medium** (yellow) — Consider CTQ |
| < 15 | **Low** (green) — Acceptable |

---

## Classification Decision Rules

### When Classification = `RMM`
- Criticality ≥ 30 **and** Severity is S or 8
- 必须分配 RMM ID（例如 `CT-NM.RMM-P2-RMM.xxxx`）
- 必须评估 RMM Severity of Harm（`S1` 或 `S2`）
- 必须确认 CTS level

### When Classification = `CTQ` or `Consider CTQ`
- Criticality 处于 medium range（15–25）
- Design decision 应按 quality-critical 处理
- 可能需要 lower-level requirement 来 mitigate

### When Classification = `N/A`
- Criticality < 15
- Risk 已被 current design acceptably controlled
- Optimized state 应确认 ≤ initial criticality

---

## Criticality Matrix — Optimized State

mitigation actions 后，Criticality Matrix 会重新计算。目标是：

1. **Reduce Occurrence** — 通过更好的 prevention controls、design changes 或 redundancy
2. **Reduce Detection** — 通过更好的 test coverage、monitoring 或 verification
3. **Severity typically does not change** — 除非 design fundamentally changes failure's impact

Optimized state 应显示：
- 没有 remaining **Safety** items without RMM
- 没有 remaining **High** items without explicit mitigation actions
- 所有 **RMM** items 都有 assigned RMM IDs 和 severity classification
