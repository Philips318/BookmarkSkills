---
name: requirements-traceability
description: 'Build and validate traceability chains from SRS requirements through AC, BDD scenarios, unit tests, and manual test cases. Detects coverage gaps where requirements lack corresponding tests. Produces a traceability matrix for IEC 62304 audit readiness.'
argument-hint: 'Provide a feature name, requirements doc path, and optionally test file paths to build the traceability chain'
user-invocable: true
---

# Requirements Traceability

Use this skill to build and validate end-to-end traceability from requirements to tests, ensuring every requirement is verifiable and every test traces back to a requirement.

## When to Use

- After requirements + BDD scenarios are produced — check for coverage gaps.
- After test generation (Stage 4) — validate full chain completeness.
- Before IEC 62304 audit — produce audit-ready traceability matrix.
- When adding tests to existing features — verify no requirement is orphaned.

## Preferred Inputs

- **Requirements document** — `artifacts/<feature>/01-requirements.md`
- **BDD scenarios** — `artifacts/<feature>/01-bdd-scenarios.feature`
- **Test report** — `artifacts/<feature>/04-test-report.md` (if available)
- **Test source files** — paths to `*Tests.cs` files (if available)

## Traceability Chain

```
SRS Requirement
  └── User Story
        └── AC-1
              ├── BDD Scenario (Gherkin)
              ├── Verification Method (requirements-level)
              ├── Unit Test (implementation-level)
              └── Manual Test Case (acceptance-level)
```

Each link in the chain must be bidirectional:
- **Forward**: Requirement → AC → Test (every requirement has tests)
- **Backward**: Test → AC → Requirement (every test traces to a requirement)

## Analysis Process

### Step 1: Extract Requirements

Parse `01-requirements.md` to build a list:

| ID | Title | Type |
|----|-------|------|
| AC-1 | [title] | Functional |
| AC-2 | [title] | Functional |
| NFR-1 | [title] | Non-Functional |

### Step 2: Map BDD Scenarios

Parse `01-bdd-scenarios.feature` and match @AC tags to requirements:

| AC | Scenario | Tag | Matched? |
|----|----------|-----|----------|
| AC-1 | [title] | @AC-1 | ✓ |
| AC-2 | — | — | ✗ Gap! |

### Step 3: Map Verification Methods

From `01-requirements.md` Verification Methods table:

| AC | Verification Method | Defined? |
|----|-------------------|----------|
| AC-1 | [steps] | ✓ |
| AC-2 | — | ✗ Gap! |

### Step 4: Map Unit Tests (if available)

Parse test files or `04-test-report.md`:

| AC | Unit Test Method | Covered? |
|----|-----------------|----------|
| AC-1 | FormatStageResult_Valid... | ✓ |
| AC-3 | FormatStageResult_Negative... | ✓ |

### Step 5: Map Manual Test Cases (if available)

| AC | Manual Test Case | Covered? |
|----|-----------------|----------|
| AC-1 | TC-01 | ✓ |
| AC-4 | TC-05 | ✓ |

## Output Structure

```markdown
# Traceability Matrix — [Feature Name]

## Forward Traceability (Requirement → Test)

| AC | BDD Scenario | Verification Method | Unit Test | Manual Test | Status |
|----|-------------|-------------------|-----------|-------------|--------|
| AC-1 | ✓ Scenario 1 | ✓ VM-1 | ✓ Test_Valid | ✓ TC-01 | FULL |
| AC-2 | ✓ Scenario 2 | ✓ VM-2 | ✗ Missing | ✓ TC-02 | PARTIAL |
| AC-3 | ✗ Missing | ✗ Missing | ✗ Missing | ✗ Missing | NONE |

## Backward Traceability (Test → Requirement)

| Test | Type | Traces to AC | Orphan? |
|------|------|-------------|---------|
| Test_Valid | Unit | AC-1 | No |
| Test_Boundary | Unit | AC-3 | No |
| Test_Legacy | Unit | — | ✗ Orphan! |

## Coverage Summary

| Metric | Count | Percentage |
|--------|-------|-----------|
| Total ACs | N | — |
| ACs with BDD scenario | N | ?% |
| ACs with verification method | N | ?% |
| ACs with unit test | N | ?% |
| ACs with manual test | N | ?% |
| Full coverage (all 4) | N | ?% |
| No coverage (0 of 4) | N | ?% |

## Gaps

### [GAP-1] AC-3 has no BDD scenario
**Severity:** HIGH — Every AC must have at least one Gherkin scenario.
**Action:** Add scenario to 01-bdd-scenarios.feature.

### [GAP-2] Test_Legacy is orphan (no AC)
**Severity:** LOW — Test exists but does not trace to any current requirement.
**Action:** Review if test covers an undocumented requirement or is obsolete.

## Audit Readiness

| IEC 62304 Check | Status |
|----------------|--------|
| Every requirement has verification method | ✓/✗ |
| Forward traceability complete | ✓/✗ |
| Backward traceability complete | ✓/✗ |
| No orphan tests | ✓/✗ |
| Coverage > 80% across all chains | ✓/✗ |

**Verdict: AUDIT_READY / GAPS_FOUND / NOT_READY**
```

## Quality Rules

- Every AC must have at least a BDD scenario AND a verification method (minimum bar).
- Orphan tests (no AC link) must be reviewed — they may indicate undocumented requirements.
- Coverage below 80% in any chain triggers GAPS_FOUND verdict.
- For IEC 62304 Class B/C, 100% forward traceability is mandatory.

## Required Chains by Safety Class

The minimum chain depth required for AUDIT_READY depends on the Safety Classification in `01-requirements.md` §12:

| Safety Class | BDD Scenario | Verification Method | Unit Test | Manual Test | Impact Analysis |
|--------------|:------------:|:-------------------:|:---------:|:-----------:|:---------------:|
| **A**        | Required     | Required            | Recommended | Optional  | Optional        |
| **B**        | Required     | Required            | Required  | Required    | **Required**    |
| **C**        | Required     | Required            | Required  | Required    | **Required** + every applicable `safety-rules.md` red line traced |

A Class C feature where any `safety-rules.md` red line applies but is **not** traced to an AC, BDD scenario, and test is automatically `NOT_READY`.

## Portability Note

This skill is **team-portable**. The traceability chain (Requirement → AC → BDD → VM → Test), forward/backward matrix, orphan-detection rules, and IEC 62304 audit checks are generic and apply to any medical-device project that follows the artifact layout. Safety-class triggers come from `domain-knowledge/safety-rules.md`, which each team customizes — no edits to this file are needed. Team-specific test-ID conventions, additional audit checklists, or regulatory mapping (e.g., FDA 510(k), EU MDR sections) belong in `references/` files or `/memories/repo/`.
