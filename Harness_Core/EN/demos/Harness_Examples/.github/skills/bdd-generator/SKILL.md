---
name: bdd-generator
description: 'Convert acceptance criteria into BDD Gherkin scenarios and generate SpecFlow step definitions. Produces .feature files and C# step binding stubs for the CT software department BDD workflow.'
argument-hint: 'Provide acceptance criteria, a User Story, or a requirement description to convert into BDD scenarios'
user-invocable: true
---

# BDD Generator

Use this skill to convert acceptance criteria into executable BDD specifications using Gherkin syntax and SpecFlow step definitions for .NET projects.

## When to Use

- After the requirements skill has produced structured AC — convert them to `.feature` files.
- During Three Amigos sessions — generate draft scenarios for discussion.
- When adding test coverage to existing features — reverse-engineer AC from code and generate Gherkin.
- When reviewing BDD coverage — check if all AC are covered by scenarios.

## Integration with RequirementsAnalyst

This skill is invoked by the **RequirementsAnalyst agent at Workflow step 3**. The following are **hard contracts** — the RA's Definition of Ready depends on them:

- **Every AC must have at least one Scenario** tagged with the matching `@AC-n`. Missing AC ⇒ DoR fails.
- **The Coverage Matrix is mandatory** — it is read by the RA's self-review (`requirements-review` skill) and is the source of truth for AC → Scenario traceability.
- **The `.feature` file path is fixed**: `artifacts/<feature>/01-bdd-scenarios.feature`. Step-definition stubs are not written by the RA; they are produced later by the Tester (Stage 4) into the appropriate `specs/StepDefinitions/<module>/` folder.
- **Safety class tag is required**: every Feature must carry `@class-A`, `@class-B`, or `@class-C` matching the Safety Classification in `01-requirements.md`.

## Preferred Inputs

Provide one or more of the following:

- **Acceptance Criteria** — in Given/When/Then or plain text format.
- **User Story** — the skill will extract AC and generate scenarios.
- **Existing .feature file** — for coverage review or enhancement.
- **Module / class name** — for context when generating step definitions.

## Output Structure

### 1. Feature File (.feature)

```gherkin
@tag-module @tag-safety-class
Feature: [Feature name]
  As a [role]
  I want [goal]
  So that [benefit]

  Background:
    Given [common precondition shared by all scenarios]

  @AC-1
  Scenario: [AC-1 descriptive title]
    Given [precondition]
    When [action]
    Then [expected outcome]

  @AC-2
  Scenario Outline: [AC-2 with multiple data sets]
    Given [precondition with <parameter>]
    When [action with <input>]
    Then [expected outcome with <result>]

    Examples:
      | parameter | input | result |
      | value1    | in1   | out1   |
      | value2    | in2   | out2   |
```

### 2. Step Definitions (C# / SpecFlow)

Generate step definition stubs matching the Gherkin steps:

```csharp
[Binding]
public class FeatureNameSteps
{
    [Given(@"precondition")]
    public void GivenPrecondition()
    {
        // TODO: Arrange — set up test state
        throw new PendingStepException();
    }

    [When(@"action")]
    public void WhenAction()
    {
        // TODO: Act — execute the behavior under test
        throw new PendingStepException();
    }

    [Then(@"expected outcome")]
    public void ThenExpectedOutcome()
    {
        // TODO: Assert — verify the result
        throw new PendingStepException();
    }
}
```

### 3. Coverage Matrix

Map every AC to its corresponding scenario(s):

| AC | Scenario | Type | Covered |
|----|----------|------|---------|
| AC-1 | Scenario: ... | Happy path | ✓ |
| AC-1 | Scenario: ... | Boundary | ✓ |
| AC-2 | Scenario Outline: ... | Data-driven | ✓ |
| AC-3 | — | — | ✗ Missing |

## Scenario Design Rules

See [bdd-guidelines.md](./references/bdd-guidelines.md) for full rules. Key principles:

1. **One behavior per scenario** — do not test multiple things in one scenario.
2. **Independent** — no scenario depends on another scenario's state.
3. **Concrete values from `domain-knowledge`** — never "some value" / "valid input". Pull realistic constants from `.github/skills/domain-knowledge/references/` (see [Domain-Anchored Examples](#domain-anchored-examples) below).
4. **Declarative, not imperative** — describe WHAT, not HOW (no UI clicks in Given/When/Then).
5. **Background for shared setup** — extract common Given steps to Background.
6. **Scenario Outline for data variations** — use Examples table instead of duplicating scenarios.
7. **Tags for traceability** — tag with AC ID, module, safety class.

## Domain-Anchored Examples

When the AC involves CT/DICOM/Spectral concepts, the Examples table values **must come from `domain-knowledge`**, not made-up numbers. This guarantees that BDD scenarios test the real domain envelope.

| Domain area | Reference file | Canonical values |
|-------------|----------------|------------------|
| Spectral MonoE keV | `spectral-knowledge.md` | 40 / 70 / 100 / 200 (min, common, common, max) |
| Iodine quantification | `spectral-knowledge.md` | 0 / 5 / 15 mg/ml (zero, accuracy threshold, typical high) |
| Concurrent spectral results | `spectral-knowledge.md` | 1 / 4 / 5 (typical, max allowed, over-limit → reject) |
| SBI version compatibility | `spectral-knowledge.md` | 1.x reader vs 2.x data → reject; 2.x reader vs 1.x data → accept |
| Patient position (IOP) | `ct-glossary.md` (8 positions) | HFS / HFP / HFDL / HFDR / FFS / FFP / FFDL / FFDR |
| Window Width / Center | `ct-glossary.md` | WW 1 / 400 / 4096; WC -1000 / 40 / 3000 |
| Transfer syntax | `dicom-patterns.md` | ImplicitVRLittleEndian / ExplicitVRLittleEndian / JPEGLossless / JPEG2000 |
| Photometric Interpretation | `dicom-patterns.md` | MONOCHROME1 / MONOCHROME2 / RGB |
| Slice geometry | `dicom-patterns.md` | SliceThickness 0.625 / 1.0 / 5.0 mm; SpacingBetweenSlices may differ |
| EFOV spectral limit | `spectral-knowledge.md` | 500 mm (boundary: 499 accept, 501 reject) |
| Lossy compression on diagnostic path | `safety-rules.md` | must be rejected — always include a Class C scenario asserting rejection |

**Rule:** If a Scenario Outline tests a numeric range, the Examples table MUST include the boundary value(s) from the table above, not arbitrary multiples of 10.

## Coverage Targets by Safety Class

Coverage intensity is driven by the Safety Classification in `01-requirements.md`. The RA uses this table to validate the `.feature` file before declaring DoR met.

| Safety Class | Required Coverage |
|--------------|-------------------|
| **Class A** | At least 1 happy-path Scenario per AC + at least 1 error-path Scenario per Feature |
| **Class B** | Class A + boundary values for every numeric/enum AC + at least 1 Scenario per major error path + state-transition Scenarios for stateful AC |
| **Class C** | Class B + **every** error path covered + **every** state transition covered + at least 1 Scenario per `safety-rules.md` red line that applies (e.g. "lossy compression rejected", "mammography blocked", "keV out of range rejected") + negative authorization tests where applicable |

The Coverage Matrix table at the end of the `.feature` deliverable must show **why** the coverage meets the safety class — e.g. add a `Class-C Required` column when the class is C.

## Coverage Targets (functional dimensions)

In addition to the safety-class minimums above, scenarios should still span:

- **Happy path**: At least 1 scenario for the normal flow.
- **Boundary values**: Min, max, and edge cases for numeric/string inputs.
- **Error paths**: Invalid input, missing data, unauthorized access.
- **State transitions**: If the feature involves state changes, cover each transition.
- **Equivalence classes**: Group similar inputs, test one from each class.

## BDD for Non-Functional Requirements

NFRs from `01-requirements.md` also need executable scenarios. Use these patterns:

| NFR sub-category | Pattern | Example tag |
|------------------|---------|-------------|
| **Performance** | `Then ... within <N> ms` / `... within <N> seconds` | `@performance` |
| **Reliability** | Long-running scenario with repeated `When` step | `@reliability @long-running` |
| **Usability** | Manual scenario — step definitions throw `PendingStepException`, executed in usability sessions | `@manual @usability` |
| **Security** | Negative-authorization scenarios ("Given user without role X, When ..., Then access denied") | `@security` |
| **Regulatory** | Assert presence/format of audit log entries, DICOM tag values, regulatory labels | `@regulatory` |

NFR scenarios live in the same `.feature` file but should be grouped under a `# --- Non-Functional ---` comment for clarity.

## SpecFlow Project Conventions

- Feature files go in: `specs/Features/[ModuleName]/`
- Step definitions go in: `specs/StepDefinitions/[ModuleName]/`
- Use `ScenarioContext` for sharing state between steps.
- Use dependency injection (BoDi) for service dependencies.
- Step definitions must not contain business logic — delegate to page objects or service wrappers.

## Runner Selection (must match target test project)

The Tester agent (Stage 4) will integrate the generated stubs into an existing test project. Pick the runner that matches the production target:

| Production target | Test framework | NuGet packages | Stub attribute style |
|-------------------|----------------|----------------|----------------------|
| .NET 8 / .NET 6  | **xUnit** + FluentAssertions | `SpecFlow.xUnit`, `xunit`, `FluentAssertions` | `[Binding]`, no class attribute |
| .NET Framework 4.8 (existing MSTest project) | **MSTest** | `SpecFlow.MsTest`, `MSTest.TestFramework` | `[Binding]` |
| .NET Framework 4.8 (existing NUnit project) | **NUnit** | `SpecFlow.NUnit`, `NUnit` | `[Binding]` |

**Rule:** Always inspect the existing test project's `.csproj` to detect the framework before generating stubs; do not introduce a second framework into a project that already uses one.

## Recommended Step-Definition Stub Template

The minimal `throw new PendingStepException()` form is acceptable only for first drafts. The **team-preferred** stub uses constructor injection + `ScenarioContext` + a service wrapper, ready for the Tester to fill in:

```csharp
// Copyright (c) Koninklijke Philips N.V. <YEAR>. All rights reserved.
using System;
using BoDi;
using TechTalk.SpecFlow;
using FluentAssertions;

namespace MyModule.Specs.StepDefinitions
{
    [Binding]
    public sealed class FeatureNameSteps
    {
        private readonly ScenarioContext _scenarioContext;
        private readonly IFeatureNameService _service;     // injected via BoDi

        public FeatureNameSteps(ScenarioContext scenarioContext, IFeatureNameService service)
        {
            _scenarioContext = scenarioContext ?? throw new ArgumentNullException(nameof(scenarioContext));
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        [Given(@"the patient has an active scan session")]
        public void GivenPatientHasActiveScanSession()
        {
            var session = _service.CreateSession();
            _scenarioContext["session"] = session;
        }

        [When(@"the user sets WW to (\d+)")]
        public void WhenUserSetsWindowWidthTo(int windowWidth)
        {
            var session = (ScanSession)_scenarioContext["session"];
            _service.SetWindowWidth(session, windowWidth);
        }

        [Then(@"the system should accept the value")]
        public void ThenSystemShouldAcceptTheValue()
        {
            var session = (ScanSession)_scenarioContext["session"];
            session.LastError.Should().BeNull();
        }

        [Then(@"the system should reject with validation error")]
        public void ThenSystemShouldRejectWithValidationError()
        {
            var session = (ScanSession)_scenarioContext["session"];
            session.LastError.Should().NotBeNull();
        }
    }
}
```

Key points:
- Constructor injection — not `[BeforeScenario]` field assignment.
- `ScenarioContext` is the only shared mutable state between steps.
- Assertions go through FluentAssertions for readable failure messages.
- No business logic in step methods — they delegate to `IFeatureNameService`, which lives in the production module.
- Copyright header (per TICS) — use the current year.
