# Scoring Rubric

将 raw code metrics 转换为 0–10 dimension scores 的详细规则。

---

## General Principles

1. **Density-based scoring**：大多数 metrics 按 module size（files、functions、LoC）normalize，以便公平比较不同大小的 modules。
2. **Worst-file penalty**：如果任一单个文件 score 低于 2，则无论 average 如何，该 dimension score 最高 cap at 6。
3. **N/A handling**：如果 metric 无法测量，则从 dimension weight 中排除，并对剩余 metrics 重新 normalize。

---

## D1. CodeScene Code Health — Scoring Table

| Violation Density | 分数 | 等级 |
|-------------------|-------|-------|
| 0%                | 10    | A     |
| ≤ 2%              | 9     | A     |
| ≤ 5%              | 8     | B     |
| ≤ 10%             | 7     | B     |
| ≤ 15%             | 6     | C     |
| ≤ 25%             | 5     | C     |
| ≤ 35%             | 4     | D     |
| ≤ 50%             | 3     | D     |
| ≤ 70%             | 2     | F     |
| > 70%             | 1     | F     |

*Violation Density = functions with any CodeScene violation / total functions*

---

## D2. TICS Compliance — Scoring Table

| Compliance Rate | 分数 | 等级 |
|----------------|-------|-------|
| 100%           | 10    | A     |
| ≥ 97%          | 9     | A     |
| ≥ 95%          | 8     | B     |
| ≥ 90%          | 7     | B     |
| ≥ 85%          | 6     | C     |
| ≥ 75%          | 5     | C     |
| ≥ 65%          | 4     | D     |
| ≥ 50%          | 3     | D     |
| ≥ 30%          | 2     | F     |
| < 30%          | 1     | F     |

*Compliance Rate = (total checkpoints − weighted violations) / total checkpoints × 100%*

---

## D3. Test Quality — Scoring Table

| 条件 | 分数 |
|-----------|-------|
| No test project exists | 0 |
| Test project exists, 0 test files | 1 |
| Test ratio < 1:10 | 2 |
| Test ratio 1:5–1:10, no coverage data | 3 |
| Test ratio 1:3–1:5, basic patterns | 4 |
| Test ratio 1:2–1:3, AAA pattern used | 5 |
| Test ratio ~1:2, good naming, some edge cases | 6 |
| Test ratio ~1:1, good patterns | 7 |
| Coverage ≥ 70% confirmed, good patterns | 8 |
| Coverage ≥ 80%, mutation score ≥ 60% | 9 |
| Coverage ≥ 90%, comprehensive edge cases | 10 |

---

## D4. Architecture — Scoring Table

| Violation Count (weighted) | 分数 |
|---------------------------|-------|
| 0 | 10 |
| 1 | 8 |
| 2–3 | 6 |
| 4–6 | 4 |
| 7–10 | 2 |
| > 10 | 1 |

*Weight: layer violation = 2, circular dep = 2, other = 1*

---

## D5. Maintainability — Composite Scoring

Sub-scores averaged:

| Sub-metric | Good (10) | Acceptable (6) | Poor (2) |
|------------|-----------|----------------|----------|
| Duplication | ≤ 1% | ≤ 5% | > 10% |
| File size | 95% ≤ 500 LoC | 80% ≤ 500 LoC | < 60% ≤ 500 LoC |
| CC distribution | 95% ≤ 9 | 80% ≤ 9 | < 60% ≤ 9 |
| TODO density | 0 per kLoC | ≤ 2 per kLoC | > 5 per kLoC |
| 扇出 | all ≤ 15 | 90% ≤ 20 | < 70% ≤ 20 |

---

## D6. Security — Scoring Table

从 10 开始，每个 finding 扣分：

| 严重度 | 扣分 |
|----------|-----------|
| 关键 | −3 each |
| 高 | −1.5 each |
| 中 | −0.5 each |
| 低 | −0.2 each |

Minimum score: 0. 如果不存在 security-relevant code（pure DTO module），score N/A。

---

## D7. Performance — Scoring Table

从 10 开始，每个 finding 扣分：

| 模式 | 扣分 |
|---------|-----------|
| UI thread blocking | −2 each |
| O(n²) in hot path | −2 each |
| Resource leak | −1.5 each |
| Async misuse (.Result/.Wait) | −1 each |
| Event leak | −1 each |
| String concat in loop | −0.5 each |
| LINQ alloc in hot path | −0.5 each |

Minimum score: 0.

---

## D8. Defect Density — Scoring Table

| Hotspot Count (per 100 files) | Base Score |
|-------------------------------|------------|
| 0 | 10 |
| 1 | 8 |
| 2–3 | 6 |
| 4–6 | 4 |
| 7–10 | 2 |
| > 10 | 1 |

*Hotspot = file with CC > 15 AND LoC > 300, or file with > 3 error-prone patterns*

Additional modifiers:
- Global mutable state: −1 per occurrence（max −3）
- High churn + low health: 每个 file −1（if git history available）

---

## Grade Boundaries

| Overall Score | 等级 | Color Code | Hex |
|---------------|-------|------------|-----|
| 9.0 – 10.0 | A | 绿色 | #22c55e |
| 7.0 – 8.9 | B | 蓝色 | #3b82f6 |
| 5.0 – 6.9 | C | 黄色 | #eab308 |
| 3.0 – 4.9 | D | 橙色 | #f97316 |
| 0.0 – 2.9 | F | 红色 | #ef4444 |
