---
name: requirements
description: 'Structure fuzzy one-line requirements into a complete requirements package: User Story, Acceptance Criteria (Given/When/Then), Non-Functional Requirements, constraints, risks, and BDD Gherkin scenarios. Designed for IEC 62304 medical device software.'
argument-hint: 'Provide a requirement description, module name, and optionally the IEC 62304 safety class (A/B/C)'
user-invocable: true
---

# Requirements Structuring

Use this skill to transform vague, one-line requirement descriptions into a structured, testable requirements package that SE, developers, and QA can directly work from.

## When to Use

- SE receives a brief requirement and needs to expand it into a full specification.
- Before a Three Amigos session — to prepare a structured draft for discussion.
- When checking an existing requirement for completeness and consistency.
- When generating BDD Gherkin scenarios from acceptance criteria.

## Preferred Inputs

Provide one or more of the following:

- **Requirement description** — even a single sentence is sufficient.
- **Module / component name** — e.g., "MIA Tissue Management", "Volume Rendering".
- **IEC 62304 safety class** — A / B / C. If unspecified, derive from `domain-knowledge/safety-rules.md` triggers; when in doubt escalate one level.
- **Existing AC or constraints** — if any partial specification already exists.

Before drafting, **always** consult `domain-knowledge` (ct-glossary, dicom-patterns, spectral-knowledge, clinical-workflow, safety-rules) to:
- validate terminology (HU/keV/formal names, not marketing)
- pull realistic boundary values for AC examples
- identify safety red lines that may convert into ACs or Out-of-Scope items

## Output Structure

The skill produces a requirements package matching the RequirementsAnalyst agent template. Sections **must appear in this order** so downstream agents can parse it:

1. Stakeholders (table: role / name / concern)
2. Glossary (feature-local terms, in addition to global `ct-glossary.md`)
3. User Story (`As a / I want / so that`)
4. Acceptance Criteria (AC-1, AC-2, … in Given/When/Then)
5. Non-Functional Requirements (all 5 sub-categories — see below)
6. Verification Methods (one row per AC: Precondition / Steps / Expected Result)
7. Constraints (Technical / Regulatory / Business)
8. Assumptions
9. Out of Scope
10. Open Questions (table: # / Question / Owner / Blocking?)
11. Risks (table: # / Risk / Likelihood / Impact / Mitigation)
12. Safety Classification (A / B / C) with **justification quoting `safety-rules.md`**
13. Skipped Artifacts (if any, with reason)

### 3. User Story

Format: `As a [role], I want [goal], so that [benefit]`. Use a concrete role (radiologist, technologist, service engineer, system), not generic "user".

### 4. Acceptance Criteria (AC)

Each AC uses Given/When/Then format:

```gherkin
AC-1: [Short title]
  Given [precondition]
  When [action]
  Then [expected outcome]
```

Cover:
- Normal flow (happy path)
- Alternative flows
- Error / exception flows
- Boundary conditions (use anchors from `domain-knowledge`, e.g., keV 40–200, iodine accuracy at 5 mg/ml, EFOV 500 mm)

See [requirements-checklist.md § Acceptance Criteria](./references/requirements-checklist.md#acceptance-criteria).

### 5. Non-Functional Requirements (NFR)

All five sub-categories must be present. Write `N/A — <one-line rationale>` when not applicable; do not leave blank.

- **Performance**: response time, throughput, memory limits
- **Security**: authentication, authorization, data protection, PHI handling
- **Usability**: accessibility, localization, clinical workflow fit
- **Reliability**: error recovery, data integrity, failover, degraded mode (e.g., Citrix)
- **Regulatory / Compliance**: IEC 62304 class, DICOM conformance statements, IFU obligations

### 6. Verification Methods

One row per AC. Requirement-level only (the Tester agent expands these into detailed test cases in Stage 4):

| AC | Precondition | Steps | Expected Result |
|----|--------------|-------|-----------------|
| AC-1 | [setup] | 1. step<br>2. step | [observable outcome] |

### 7–9. Constraints / Assumptions / Out of Scope

Standard sections. Out of Scope should explicitly call out any safety red lines that this feature does **not** cover (e.g., "mammography workflow is out of scope — see safety-rules.md").

### 10–11. Open Questions and Risks

Use tables. Open Questions must have an Owner and a Blocking? flag. Risks must have Likelihood and Impact rated.

### 12. Safety Classification

Format:
```
Safety Classification: [A | B | C]
Justification: [Quote the matching trigger from domain-knowledge/safety-rules.md]
```
When in doubt, escalate one level up. The justification must quote a specific safety-rules.md line, not paraphrase.

## Medical Device Compliance

- **Class A**: Standard output; impact analysis optional. If skipped, record under §13 Skipped Artifacts with reason.
- **Class B**: Companion `01-impact-analysis.md` is **mandatory** (see `impact-analysis` skill).
- **Class C**: Impact analysis + dedicated patient-safety risk scenarios in §11 Risks + all applicable `safety-rules.md` red lines reflected in ACs or Out of Scope.

## Integration with bdd-generator and Verification

The `bdd-generator` skill consumes this output and requires:
- Every AC has at least one BDD scenario (Coverage Matrix at end of `.feature` file)
- Feature carries a `@class-A` / `@class-B` / `@class-C` tag matching §12
- Boundary values in Scenario Outline `Examples` tables come from `domain-knowledge`

The `requirements-review` skill checks both the structure above and the integration contract.

## Quality Rules

- Every AC must be independently testable.
- Avoid vague terms: "fast", "user-friendly", "reliable" — quantify them (see `requirements-review` skill § Clarity table).
- Use formal CT/DICOM/Spectral terminology from `domain-knowledge/ct-glossary.md`, not marketing names or ad-hoc synonyms.
- Each NFR must have a measurable acceptance threshold (or `N/A` with rationale).
- If the input is too vague to produce quality output, fill the Open Questions table and ask the stakeholder before declaring the package ready.

## Portability Note

This skill is **team-portable**. The 13-section template, Given/When/Then format, NFR sub-categories, and Safety Classification process are generic and apply to any IEC 62304 medical-device project. Domain anchors and safety triggers come from the separate `domain-knowledge` skill, which each team customizes for their product line — no edits to this file are needed. Team-specific stakeholder lists, recurring acronyms, or product-line-specific AC patterns belong in `references/requirements-checklist.md` (versioned) or `/memories/repo/` (per-workspace).
