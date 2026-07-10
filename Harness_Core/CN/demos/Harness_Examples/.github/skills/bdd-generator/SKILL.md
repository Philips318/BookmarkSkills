---
name: bdd-generator
description: '将 acceptance criteria 转换为 BDD Gherkin scenarios，并生成 SpecFlow step definitions。为 CT software department BDD workflow 产出 .feature files 和 C# step binding stubs。'
argument-hint: 'Provide acceptance criteria, a User Story, or a requirement description to convert into BDD scenarios'
user-invocable: true
---

# BDD Generator

使用此 skill 将 acceptance criteria 转换为使用 Gherkin syntax 和 SpecFlow step definitions 的可执行 BDD specifications，用于 .NET projects。

## When to Use

- requirements skill 已产出 structured AC 后 — 将它们转换为 `.feature` files。
- Three Amigos sessions 期间 — 生成 draft scenarios 供讨论。
- 为 existing features 增加 test coverage 时 — 从 code 反向推导 AC 并生成 Gherkin。
- 审查 BDD coverage 时 — 检查所有 AC 是否被 scenarios 覆盖。

## Integration with RequirementsAnalyst

此 skill 由 **RequirementsAnalyst agent 在 Workflow step 3** 调用。以下是 **hard contracts** — RA 的 Definition of Ready 依赖它们：

- **每个 AC 必须至少有一个 Scenario**，并带有匹配的 `@AC-n` tag。缺少 AC ⇒ DoR fails。
- **Coverage Matrix 是 mandatory** — RA 的 self-review（`requirements-review` skill）会读取它，它是 AC → Scenario traceability 的 source of truth。
- **`.feature` file path 固定**：`artifacts/<feature>/01-bdd-scenarios.feature`。RA 不写 step-definition stubs；它们稍后由 Tester（Stage 4）写入合适的 `specs/StepDefinitions/<module>/` folder。
- **Safety class tag 是必需的**：每个 Feature 必须携带 `@class-A`、`@class-B` 或 `@class-C`，并与 `01-requirements.md` 中的 Safety Classification 匹配。

## Preferred Inputs

提供以下一项或多项：

- **Acceptance Criteria** — Given/When/Then 或 plain text format。
- **User Story** — skill 会提取 AC 并生成 scenarios。
- **Existing .feature file** — 用于 coverage review 或 enhancement。
- **Module / class name** — 生成 step definitions 时作为上下文。

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

生成与 Gherkin steps 匹配的 step definition stubs：

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

将每个 AC 映射到对应 scenario(s)：

| AC | 场景 | 类型 | Covered |
|----|----------|------|---------|
| AC-1 | Scenario: ... | 正常路径 | ✓ |
| AC-1 | Scenario: ... | Boundary | ✓ |
| AC-2 | Scenario Outline: ... | Data-driven | ✓ |
| AC-3 | — | — | ✗ Missing |

## Scenario Design Rules

完整规则见 [bdd-guidelines.md](./references/bdd-guidelines.md)。关键原则：

1. **One behavior per scenario** — 不要在一个 scenario 中测试多件事。
2. **Independent** — scenario 不依赖另一个 scenario 的 state。
3. **Concrete values from `domain-knowledge`** — 不要用 "some value" / "valid input"。从 `.github/skills/domain-knowledge/references/` 取真实 constants（见下方 [Domain-Anchored Examples](#domain-anchored-examples)）。
4. **Declarative, not imperative** — 描述 WHAT，而不是 HOW（Given/When/Then 中不要写 UI clicks）。
5. **Background for shared setup** — 将 common Given steps 提取到 Background。
6. **Scenario Outline for data variations** — 使用 Examples table，而不是重复 scenarios。
7. **Tags for traceability** — 用 AC ID、module、safety class 打 tag。

## Domain-Anchored Examples

当 AC 涉及 CT/DICOM/Spectral concepts 时，Examples table values **必须来自 `domain-knowledge`**，不能编造数字。这保证 BDD scenarios 测试真实 domain envelope。

| 领域区域 | 参考文件 | Canonical values |
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

**Rule:** 如果 Scenario Outline 测试 numeric range，Examples table 必须包含上表中的 boundary value(s)，而不是任意 10 的倍数。

## Coverage Targets by Safety Class

Coverage intensity 由 `01-requirements.md` 中的 Safety Classification 驱动。RA 使用此表在声明 DoR met 前验证 `.feature` file。

| 安全等级 | 必需覆盖范围 |
|--------------|-------------------|
| **Class A** | 每个 AC 至少 1 个 happy-path Scenario + 每个 Feature 至少 1 个 error-path Scenario |
| **Class B** | Class A + 每个 numeric/enum AC 的 boundary values + 每个 major error path 至少 1 个 Scenario + stateful AC 的 state-transition Scenarios |
| **Class C** | Class B + **每个** error path 覆盖 + **每个** state transition 覆盖 + 每个适用 `safety-rules.md` red line 至少 1 个 Scenario（例如 "lossy compression rejected"、"mammography blocked"、"keV out of range rejected"）+ 适用时 negative authorization tests |

`.feature` deliverable 末尾的 Coverage Matrix table 必须说明**为什么** coverage 满足 safety class — 例如 class 为 C 时添加 `Class-C Required` column。

## Coverage Targets (functional dimensions)

除上述 safety-class minimums 外，scenarios 仍应覆盖：

- **Happy path**: 正常流程至少 1 个 scenario。
- **Boundary values**: numeric/string inputs 的 min、max 和 edge cases。
- **Error paths**: Invalid input、missing data、unauthorized access。
- **State transitions**: 如果 feature 涉及 state changes，覆盖每个 transition。
- **Equivalence classes**: 对相似 inputs 分组，并从每类测试一个。

## BDD for Non-Functional Requirements

`01-requirements.md` 中的 NFRs 也需要 executable scenarios。使用这些 patterns：

| NFR sub-category | 模式 | Example tag |
|------------------|---------|-------------|
| **Performance** | `Then ... within <N> ms` / `... within <N> seconds` | `@performance` |
| **Reliability** | Long-running scenario with repeated `When` step | `@reliability @long-running` |
| **Usability** | Manual scenario — step definitions throw `PendingStepException`, executed in usability sessions | `@manual @usability` |
| **Security** | Negative-authorization scenarios ("Given user without role X, When ..., Then access denied") | `@security` |
| **Regulatory** | Assert presence/format of audit log entries, DICOM tag values, regulatory labels | `@regulatory` |

NFR scenarios 放在同一个 `.feature` file 中，但应归组到 `# --- Non-Functional ---` comment 下以保持清晰。

## SpecFlow Project Conventions

- Feature files go in: `specs/Features/[ModuleName]/`
- Step definitions go in: `specs/StepDefinitions/[ModuleName]/`
- 使用 `ScenarioContext` 在 steps 间共享 state。
- 对 service dependencies 使用 dependency injection（BoDi）。
- Step definitions 不得包含 business logic — 委派给 page objects 或 service wrappers。

## Runner Selection (must match target test project)

Tester agent（Stage 4）会将 generated stubs 集成到现有 test project。选择与 production target 匹配的 runner：

| Production target | Test framework | NuGet packages | Stub attribute style |
|-------------------|----------------|----------------|----------------------|
| .NET 8 / .NET 6  | **xUnit** + FluentAssertions | `SpecFlow.xUnit`, `xunit`, `FluentAssertions` | `[Binding]`, no class attribute |
| .NET Framework 4.8 (existing MSTest project) | **MSTest** | `SpecFlow.MsTest`, `MSTest.TestFramework` | `[Binding]` |
| .NET Framework 4.8 (existing NUnit project) | **NUnit** | `SpecFlow.NUnit`, `NUnit` | `[Binding]` |

**Rule:** 生成 stubs 前始终检查 existing test project 的 `.csproj` 来检测 framework；不要在已使用某 framework 的 project 中引入第二个 framework。

## Recommended Step-Definition Stub Template

最小 `throw new PendingStepException()` 形式只适合 first drafts。**team-preferred** stub 使用 constructor injection + `ScenarioContext` + service wrapper，方便 Tester 填写：

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
- Constructor injection — 不使用 `[BeforeScenario]` field assignment。
- `ScenarioContext` 是 steps 间唯一 shared mutable state。
- Assertions 通过 FluentAssertions，以获得 readable failure messages。
- Step methods 中没有 business logic — 它们委派给 production module 中的 `IFeatureNameService`。
- Copyright header（per TICS）— 使用当前年份。
