---
name: requirements-review
description: '审查并验证 structured requirements packages 的 quality、completeness、consistency 和 IEC 62304 compliance。检测 ambiguities、missing coverage、NFR gaps 和 AC quality issues。产出按 severity 排序 findings 和 improvement suggestions 的 review report。'
argument-hint: 'Provide a requirements document (01-requirements.md) or feature name to review'
user-invocable: true
---

# Requirements Review

使用此 skill 审查 structured requirements package，并评估其是否 ready for development。

## When to Use

- `requirements` skill 已产出 requirements package 后 — 分享前 self-review。
- Three Amigos session 前 — 确保 draft 高质量。
- Peer 或 SE 提交 requirements document 供反馈时。
- 检查 existing requirements 是否满足 IEC 62304 audit standards 时。

## Preferred Inputs

- **Requirements document** — `artifacts/<feature>/01-requirements.md` 或 equivalent。
- **BDD scenarios** — `artifacts/<feature>/01-bdd-scenarios.feature`，用于 cross-checking。
- **Safety class** — A/B/C，用于应用 class-specific checks。

## Review Dimensions

从 **8 dimensions** 评估 requirements package：

### 1. Clarity (ambiguity detection)

扫描必须量化的 vague 或 subjective terms：

| Vague Term | Required Fix |
|-----------|-------------|
| "fast", "quickly" | Specify threshold: "< 200ms" |
| "user-friendly" | Define measurable criteria |
| "reliable" | Specify uptime %, MTBF, recovery behavior |
| "appropriate", "suitable" | Define exact conditions |
| "etc.", "and so on" | Enumerate explicitly |
| "should" (vs "shall") | Use "shall" for mandatory, "should" for recommended |
| "if possible" | Decide: mandatory or out of scope |

**Also check domain terminology** — cross-reference `domain-knowledge/ct-glossary.md`：

| Wrong | Right | 原因 |
|-------|-------|-----|
| "Hounsfield" | "HU" | Use the standard abbreviation |
| "kV" (when meaning energy) | "keV" | Tube voltage vs photon energy are different concepts |
| Marketing names ("AI Recon") | Formal names ("Precise Image") | IFU / documentation must use formal names |
| "window" (ambiguous) | "window width" or "window center" (WW / WC) | The two are different parameters |
| Generic "image" | "axial slice" / "MPR" / "MIP" | Different reconstructions, different APIs |

将 domain-term misuse 标记为 Clarity findings，severity 为 MEDIUM（如果会改变 AC meaning，则为 HIGH）。

### 2. Testability

对每个 AC 验证：
- [ ] 可隔离测试（无 hidden dependencies）
- [ ] 有清晰 pass/fail criterion
- [ ] 使用 concrete values，而不是 placeholders
- [ ] Expected result observable and measurable

任何需要主观判断才能验证的 AC 都要标记。

### 3. Completeness

根据 [requirements checklist](./references/requirements-review-checklist.md) 检查 coverage：
- [ ] Happy path covered
- [ ] Alternative flows covered
- [ ] Boundary conditions（min、max、edge）specified
- [ ] Error / exception paths defined
- [ ] Concurrency considerations（if applicable）
- [ ] Security considerations（if patient data involved）
- [ ] NFRs have measurable thresholds
- [ ] Verification methods defined for every AC

### 4. Consistency

- [ ] ACs 之间没有 contradictions
- [ ] AC numbering sequential and complete
- [ ] User Story role matches the AC context
- [ ] NFRs 不与 functional requirements 冲突
- [ ] Terminology 一致使用（no synonym drift）

### 5. Traceability

- [ ] 每个 AC 映射到至少一个 BDD scenario
- [ ] 每个 AC 都有 verification method
- [ ] User Story 链接到 parent feature/epic（if known）
- [ ] Safety class justification 已提供

### 6. NFR Quality

对每个 NFR：
- [ ] 有 numeric threshold 或 measurable criterion
- [ ] 指定如何测量（tool、method、environment）
- [ ] 指定 acceptable deviation / tolerance

### 7. Medical Device Compliance (IEC 62304)

- [ ] Safety class（A/B/C）已说明，并**显式引用** `domain-knowledge/safety-rules.md` 给出 justification
- [ ] Class B：Impact analysis section present（或存在 `01-impact-analysis.md`）
- [ ] Class C：Risks table 中记录 patient safety risk scenarios
- [ ] Examples 或 test data 中没有 patient data（PII/PHI）
- [ ] Regulatory constraints 在 NFR § Regulatory 中显式列出
- [ ] 如果 feature 涉及以下任一 **safety red lines**，对应 constraint 已作为 AC 或 explicit Out-of-Scope 捕获：

| Safety red line (from `safety-rules.md`) | Required AC or Out-of-Scope |
|-------------------------------------------|------------------------------|
| Lossy compression on diagnostic data | AC must reject lossy on diagnostic path |
| Mammography workflow | Explicit Out-of-Scope unless feature is mammography-certified |
| Spectral quantification used for diagnosis | AC must state intended use boundary |
| Pediatric without pediatric exam cards | AC must require pediatric protocol |
| Wrong-patient risk (PHI handling) | AC must specify patient-ID safeguards |
| Citrix degraded mode | Reliability NFR must specify degraded behavior |

缺少 applicable red line 的处理属于 HIGH-severity finding。

### 8. BDD Scenario Alignment

通过**读取 `01-bdd-scenarios.feature` 末尾的 Coverage Matrix**（由 `bdd-generator` skill 产出）cross-check BDD scenarios against ACs：

- [ ] 每个 AC 至少一个 scenario（matrix 中没有 missing row）
- [ ] 没有 orphan scenarios（scenarios without matching AC）
- [ ] Scenario titles match AC titles
- [ ] Background steps properly factored out
- [ ] Scenario Outlines used for data variations（not duplicated scenarios）
- [ ] Examples table values come from `domain-knowledge`（not made-up numbers）
- [ ] Coverage intensity matches Safety Class（Class C → every error path + every state transition + applicable safety-rules.md red lines）
- [ ] 每个 Feature 都携带与 Safety Classification 匹配的 `@class-A` / `@class-B` / `@class-C` tag

如果 `.feature` file 缺少 Coverage Matrix，这是 HIGH finding — `bdd-generator` skill 要求它。

## Output Structure

### Review Report

```markdown
# Requirements Review — [Feature Name]

## Summary
| Dimension | Score | Findings |
|-----------|-------|----------|
| Clarity | ?/10 | N issues |
| Testability | ?/10 | N issues |
| Completeness | ?/10 | N issues |
| Consistency | ?/10 | N issues |
| Traceability | ?/10 | N issues |
| NFR Quality | ?/10 | N issues |
| IEC 62304 Compliance | ?/10 | N issues |
| BDD Alignment | ?/10 | N issues |

**Overall Score: ?/80**
**Verdict: READY / NEEDS_WORK / INSUFFICIENT**

## Findings

### [F-1] Severity: HIGH | Dimension: Clarity
**Location:** AC-3
**Issue:** Uses vague term "quickly" without threshold
**Suggestion:** Replace with "within 500ms measured at P95"

### [F-2] Severity: MEDIUM | Dimension: Completeness
...
```

### Verdict Thresholds

| 分数 | 结论 | 操作 |
|-------|---------|--------|
| 65-80 | **READY** | Proceed to Three Amigos / Architecture |
| 45-64 | **NEEDS_WORK** | Fix findings, then re-review |
| 0-44 | **INSUFFICIENT** | Major rework needed, return to stakeholder |

## Quality Rules

- 每个 finding 都必须引用具体 AC 或 section。
- HIGH findings 必须在进入 development 前解决。
- MEDIUM findings 应解决；如有 justification 可接受。
- LOW findings 是改进建议。
- 修复后必须 rerun review 以验证 resolution。

## Portability Note

此 skill 设计为 **team-portable**。8 review dimensions、vague-term table、verdict thresholds 和 finding format 都是通用的，适用于任何 IEC 62304 requirements package。对 `domain-knowledge`（CT glossary、safety rules）的 cross-references 在此 skill 共享时会自动使用其他团队的 domain skill — 无需修改此文件。Team-specific review preferences（additional dimensions、stricter thresholds）应作为 companion file 添加到 `references/` 下，不要 patch 到此 SKILL.md。
