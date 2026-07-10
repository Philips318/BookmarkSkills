---
name: reqnroll-bdd
description: Reqnroll (SpecFlow successor) BDD step definitions: [Binding] class structure, step pattern matching, outside-in TDD workflow (Red/Green/Refactor), feature file location conventions, and vacuous pass prevention. Use when implementing or reviewing Reqnroll step definitions.
---

# Reqnroll BDD Skill

## Step Definition Class Structure

```csharp
using Reqnroll;
using NUnit.Framework;

[Binding]
public class CtDeviceMeasurementSteps
{
    private readonly ScenarioContext _scenarioContext;

    public CtDeviceMeasurementSteps(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
    }

    [Given("the CT device is connected")]
    public void GivenTheCtDeviceIsConnected()
    {
        var channel = new SimulatedDeviceChannel();
        var service = new CtDeviceService(channel);
        _scenarioContext["service"] = service;
    }

    [When("I request the current position")]
    public async Task WhenIRequestTheCurrentPosition()
    {
        var service = (ICtDeviceService)_scenarioContext["service"];
        var result = await service.GetPositionAsync();
        _scenarioContext["position"] = result;
    }

    [Then("the position should be {double} mm")]
    public void ThenThePositionShouldBeMm(double expected)
    {
        var actual = (double)_scenarioContext["position"];
        Assert.That(actual, Is.EqualTo(expected).Within(0.01));
    }
}
```

## Key Rules

- **One binding class per feature file**, named `{FeatureName}Steps.cs` in `Src/ModuleTests/StepDefinitions/`
- **Use ScenarioContext** for state between steps — never static fields (causes test isolation failures)
- **Use constructor injection** for infrastructure (DbContext, HTTP clients) — Reqnroll supports `[BeforeScenario]`-based DI
- **Step parameter types** must match Gherkin literals; use `[StepArgumentTransformation]` for custom types

## BDD Outside-In Workflow

1. Read the `.feature` file
2. Create step class with all bindings → `throw new PendingStepException()` initially
3. Run: confirm steps are **recognised and pending** (not unbound — unbound = wrong attribute text)
4. Implement **Given** steps (setup) → run → confirm Given steps pass
5. Implement **When** step (action) → run → confirm When step passes
6. Implement **Then** step (assertion) → run → confirm scenario is **GREEN**
7. Move to next scenario; repeat until all scenarios in the file are green

## Vacuous Pass Prevention

Every `Then` step must:
- Assert on a value returned by **production code**, not a hardcoded literal from a `Given` step
- Use specific matchers: `Assert.That(actual, Is.EqualTo(expected).Within(0.01))` not `Assert.IsTrue(true)`
- Call at least one public method on a production class (not on a mock or stub)

## Feature File Location

Feature files for Reqnroll must be in `Src/ModuleTests/Features/` to be picked up by the test runner.
The harness specs in `.harness/specs/` should be **symlinked or copied** to `Src/ModuleTests/Features/` during the developer workflow.

## Hooks

```csharp
[Binding]
public class ScenarioHooks
{
    [BeforeScenario]
    public void BeforeScenario() { /* shared setup across all features */ }

    [AfterScenario]
    public void AfterScenario() { /* cleanup — dispose resources */ }
}
```
