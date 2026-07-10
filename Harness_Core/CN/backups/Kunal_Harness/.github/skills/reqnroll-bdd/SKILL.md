---
name: reqnroll-bdd
description: Reqnroll（SpecFlow 继任者）BDD step definitions：[Binding] class structure、step pattern matching、outside-in TDD workflow（Red/Green/Refactor）、feature file location conventions 和 vacuous pass prevention。实现或审查 Reqnroll step definitions 时使用。
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

## 关键规则

- **每个 feature file 一个 binding class**，命名为 `Src/ModuleTests/StepDefinitions/` 中的 `{FeatureName}Steps.cs`
- **使用 ScenarioContext** 在 steps 之间传递状态 — 绝不使用 static fields（会导致 test isolation failures）
- **使用 constructor injection** 注入 infrastructure（DbContext、HTTP clients）— Reqnroll 支持基于 `[BeforeScenario]` 的 DI
- **Step parameter types** 必须匹配 Gherkin literals；自定义类型使用 `[StepArgumentTransformation]`

## BDD Outside-In Workflow

1. 读取 `.feature` 文件
2. 创建包含所有 bindings 的 step class → 初始时 `throw new PendingStepException()`
3. 运行：确认 steps 被**识别且 pending**（不是 unbound — unbound = attribute text 错误）
4. 实现 **Given** steps（setup）→ 运行 → 确认 Given steps 通过
5. 实现 **When** step（action）→ 运行 → 确认 When step 通过
6. 实现 **Then** step（assertion）→ 运行 → 确认 scenario 为 **GREEN**
7. 转到下一个 scenario；重复，直到文件中所有 scenarios 都为 green

## Vacuous Pass Prevention

每个 `Then` step 必须：
- Assert 一个由**生产代码**返回的值，而不是来自 `Given` step 的 hardcoded literal
- 使用具体 matchers：`Assert.That(actual, Is.EqualTo(expected).Within(0.01))`，不要用 `Assert.IsTrue(true)`
- 至少调用一个 production class 上的 public method（不是 mock 或 stub 上的方法）

## Feature File Location

Reqnroll 的 feature files 必须位于 `Src/ModuleTests/Features/`，测试运行器才能拾取它们。
Developer workflow 期间，`.harness/specs/` 中的 harness specs 应被 **symlinked or copied** 到 `Src/ModuleTests/Features/`。

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
