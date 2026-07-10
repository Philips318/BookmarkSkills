---
name: requirements-review
description: 'Review and validate structured requirements packages for quality, completeness, consistency, and IEC 62304 compliance. Detects ambiguities, missing coverage, NFR gaps, and AC quality issues. Produces a review report with severity-ranked findings and improvement suggestions.'
argument-hint: 'Provide a requirements document (01-requirements.md) or feature name to review'
user-invocable: true
---

# Requirements Review

Use this skill to review a structured requirements package and assess its readiness for development.

## When to Use

- After the `requirements` skill has produced a requirements package — self-review before sharing.
- Before a Three Amigos session — ensure the draft is high-quality.
- When a peer or SE submits a requirements document for feedback.
- When checking if existing requirements meet IEC 62304 audit standards.

## Preferred Inputs

- **Requirements document** — `artifacts/<feature>/01-requirements.md` or equivalent.
- **BDD scenarios** — `artifacts/<feature>/01-bdd-scenarios.feature` for cross-checking.
- **Safety class** — A/B/C, to apply class-specific checks.

## Review Dimensions

Evaluate the requirements package across **8 dimensions**:

### 1. Clarity (ambiguity detection)

Scan for vague or subjective terms that must be quantified:

| Vague Term | Required Fix |
|-----------|-------------|
| "fast", "quickly" | Specify threshold: "< 200ms" |
| "user-friendly" | Define measurable criteria |
| "reliable" | Specify uptime %, MTBF, recovery behavior |
| "appropriate", "suitable" | Define exact conditions |
| "etc.", "and so on" | Enumerate explicitly |
| "should" (vs "shall") | Use "shall" for mandatory, "should" for recommended |
| "if possible" | Decide: mandatory or out of scope |

**Also check domain terminology** — cross-reference `domain-knowledge/ct-glossary.md`:

| Wrong | Right | Why |
|-------|-------|-----|
| "Hounsfield" | "HU" | Use the standard abbreviation |
| "kV" (when meaning energy) | "keV" | Tube voltage vs photon energy are different concepts |
| Marketing names ("AI Recon") | Formal names ("Precise Image") | IFU / documentation must use formal names |
| "window" (ambiguous) | "window width" or "window center" (WW / WC) | The two are different parameters |
| Generic "image" | "axial slice" / "MPR" / "MIP" | Different reconstructions, different APIs |

Flag domain-term misuse as Clarity findings of MEDIUM severity (or HIGH if it would change the AC's meaning).

### 2. Testability

For each AC, verify:
- [ ] Can be tested in isolation (no hidden dependencies)
- [ ] Has a clear pass/fail criterion
- [ ] Uses concrete values, not placeholders
- [ ] Expected result is observable and measurable

Flag any AC that requires subjective judgment to verify.

### 3. Completeness

Check coverage against the [requirements checklist](./references/requirements-review-checklist.md):
- [ ] Happy path covered
- [ ] Alternative flows covered
- [ ] Boundary conditions (min, max, edge) specified
- [ ] Error / exception paths defined
- [ ] Concurrency considerations (if applicable)
- [ ] Security considerations (if patient data involved)
- [ ] NFRs have measurable thresholds
- [ ] Verification methods defined for every AC

### 4. Consistency

- [ ] No contradictions between ACs
- [ ] AC numbering is sequential and complete
- [ ] User Story role matches the AC context
- [ ] NFRs do not conflict with functional requirements
- [ ] Terminology is used consistently (no synonym drift)

### 5. Traceability

- [ ] Every AC maps to at least one BDD scenario
- [ ] Every AC has a verification method
- [ ] User Story links to parent feature/epic (if known)
- [ ] Safety class justification is provided

### 6. NFR Quality

For each NFR:
- [ ] Has a numeric threshold or measurable criterion
- [ ] Specifies how to measure (tool, method, environment)
- [ ] Specifies acceptable deviation / tolerance

### 7. Medical Device Compliance (IEC 62304)

- [ ] Safety class (A/B/C) is stated with **explicit justification** quoting `domain-knowledge/safety-rules.md`
- [ ] Class B: Impact analysis section present (or `01-impact-analysis.md` exists)
- [ ] Class C: Patient safety risk scenarios documented in the Risks table
- [ ] No patient data (PII/PHI) in examples or test data
- [ ] Regulatory constraints explicitly listed in NFR § Regulatory
- [ ] If the feature involves any of the **safety red lines** below, the corresponding constraint is captured as an AC or explicit Out-of-Scope:

| Safety red line (from `safety-rules.md`) | Required AC or Out-of-Scope |
|-------------------------------------------|------------------------------|
| Lossy compression on diagnostic data | AC must reject lossy on diagnostic path |
| Mammography workflow | Explicit Out-of-Scope unless feature is mammography-certified |
| Spectral quantification used for diagnosis | AC must state intended use boundary |
| Pediatric without pediatric exam cards | AC must require pediatric protocol |
| Wrong-patient risk (PHI handling) | AC must specify patient-ID safeguards |
| Citrix degraded mode | Reliability NFR must specify degraded behavior |

Missing handling of an applicable red line is a HIGH-severity finding.

### 8. BDD Scenario Alignment

Cross-check BDD scenarios against ACs by **reading the Coverage Matrix at the end of `01-bdd-scenarios.feature`** (produced by the `bdd-generator` skill):

- [ ] Every AC has at least one scenario (matrix shows no missing row)
- [ ] No orphan scenarios (scenarios without matching AC)
- [ ] Scenario titles match AC titles
- [ ] Background steps are properly factored out
- [ ] Scenario Outlines used for data variations (not duplicated scenarios)
- [ ] Examples table values come from `domain-knowledge` (not made-up numbers)
- [ ] Coverage intensity matches Safety Class (Class C → every error path + every state transition + applicable safety-rules.md red lines)
- [ ] Every Feature carries a `@class-A` / `@class-B` / `@class-C` tag matching the Safety Classification

If the Coverage Matrix is missing from the `.feature` file, that is a HIGH finding — the `bdd-generator` skill requires it.

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

| Score | Verdict | Action |
|-------|---------|--------|
| 65-80 | **READY** | Proceed to Three Amigos / Architecture |
| 45-64 | **NEEDS_WORK** | Fix findings, then re-review |
| 0-44 | **INSUFFICIENT** | Major rework needed, return to stakeholder |

## Quality Rules

- Every finding must cite the specific AC or section.
- HIGH findings must be resolved before proceeding to development.
- MEDIUM findings should be resolved; acceptable if justified.
- LOW findings are suggestions for improvement.
- The review must be re-run after fixes to verify resolution.

## Portability Note

This skill is designed to be **team-portable**. The 8 review dimensions, vague-term table, verdict thresholds, and finding format are generic and apply to any IEC 62304 requirements package. The cross-references to `domain-knowledge` (CT glossary, safety rules) automatically pick up another team's domain skill when this skill is shared — no edits to this file are needed. Team-specific review preferences (additional dimensions, stricter thresholds) should be added as a companion file under `references/`, not patched into this SKILL.md.
