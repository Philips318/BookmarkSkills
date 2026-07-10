---
name: nunit-testing
description: 使用 NSubstitute mocking 的 NUnit 3.x unit test patterns：test class structure、AAA layout、data-driven tests（TestCase、TestCaseSource、Range）、lifecycle attributes 和 substitute verification。在 CT 项目中编写或审查非 BDD unit tests 时使用。
---

# NUnit Testing Skill

CT 项目中 unit tests（非 BDD）的 NUnit 3.x patterns。Mocking library：**NSubstitute**。

## Test Quality Rules

- Test code 遵循所有生产编码指南（命名、复杂度、零 warnings）
- **每个 test 一个 assert** — 单一失败能清楚指出失败内容
- 精确定义 test scope — unit（一个类）或 module（一个组件）；其他一切都 mocked
- Tests 只有一个失败原因；将 scenarios 拆分为单独 tests
- 不做排序假设 — 每个 test 都完全自包含且独立
- 不做环境假设 — unit tests 可在任何地方运行，不修改 files/registry/OS settings
- 绝不以 defect ID 命名 test；遵循标准命名约定
- 绝不通过 static fields 在 tests 间共享 mutable state

## 测试内容

- 有效输入的 expected behaviour
- Null、empty 和 missing values
- Boundary values：exact boundary、just inside、just outside
- Dependencies 的所有 documented exceptions
- Invalid call sequences（例如在 `Init` 前调用 `Execute`）
- Thread-safety：对 shared-state methods 的多次并发调用
- High-load 和 timing requirements（多次运行测量 min/max/mean）

## Project Setup

- Test projects 命名为 `[ProjectName].Tests`，位于 `Src/{ProjectDir}/Test/`
- Required NuGet packages：`Microsoft.NET.Test.Sdk`、`NUnit`、`NUnit3TestAdapter`、`NSubstitute`
- 运行测试：`dotnet test Src\{repo}Impl.sln --no-build`

## Test Structure

```csharp
[TestFixture]
public class CtDeviceControllerTests
{
    private CtDeviceController _sut;
    private IDeviceChannel _deviceChannel;

    [SetUp]
    public void SetUp()
    {
        _deviceChannel = Substitute.For<IDeviceChannel>();
        _sut = new CtDeviceController(_deviceChannel);
    }

    [TearDown]
    public void TearDown()
    {
        _sut?.Dispose();
    }

    [Test]
    public async Task GetMeasurementAsync_WhenDeviceConnected_ReturnsCurrentValue()
    {
        // Arrange
        _deviceChannel.ReadRegisterAsync(0x01, Arg.Any<CancellationToken>()).Returns(42.5);

        // Act
        double? result = await _sut.GetMeasurementAsync();

        // Assert
        Assert.That(result, Is.EqualTo(42.5).Within(0.001));
    }

    [Test]
    public async Task GetMeasurementAsync_WhenDeviceDisconnected_ReturnsNull()
    {
        // Arrange
        _deviceChannel.ReadRegisterAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Throws(new DeviceNotConnectedException());

        // Act
        double? result = await _sut.GetMeasurementAsync();

        // Assert
        Assert.That(result, Is.Null);
    }
}
```

## Lifecycle Attributes

| 属性 | 范围 | 用途 |
|-----------|-------|---------|
| `[SetUp]` | 每个测试 | 创建全新的 SUT 和 substitutes |
| `[TearDown]` | 每个测试 | Dispose resources |
| `[OneTimeSetUp]` | ?? fixture | 昂贵的共享 setup（例如 database seed） |
| `[OneTimeTearDown]` | ?? fixture | 对应 teardown |
| `[SetUpFixture]` | Assembly | Assembly-level setup/teardown |

仅对真正 immutable shared resources 使用 `[OneTimeSetUp]` — 创建 substitutes 和 SUT instances 时优先使用 `[SetUp]`。

## Test Naming Convention

`MethodName_Scenario_ExpectedResult`

示例：
- `GetMeasurementAsync_WhenDeviceConnected_ReturnsCurrentValue`
- `SendCommand_WithNegativeValue_ThrowsArgumentOutOfRangeException`
- `Connect_WhenAlreadyConnected_DoesNotThrow`

## Assertions — Constraint-Based（首选）

```csharp
Assert.That(result, Is.EqualTo(42.5).Within(0.001));       // float equality
Assert.That(list, Has.Count.EqualTo(3));                    // collection count
Assert.That(list, Contains.Item(expectedItem));             // collection membership
Assert.That(action, Throws.TypeOf<ArgumentException>());    // exceptions
Assert.That(result, Is.Not.Null.And.GreaterThan(0));        // compound
Assert.That(str, Does.StartWith("Error:"));                 // string
```

不要使用 legacy assertions：
```csharp
Assert.AreEqual(expected, actual);   // legacy â€” provides poor failure messages
Assert.IsTrue(result == expected);   // hides what the actual value was
```

## Data-Driven Tests

```csharp
// Inline data â€” use [TestCase]
[TestCase(0.0,  0.0)]
[TestCase(10.5, 10.5)]
[TestCase(-1.0, null, Description = "Negative value is invalid")]
public async Task GetMeasurementAsync_VariousInputs_ReturnsExpected(double deviceReading, double? expected)
{
    // ...
}

// Programmatic data â€” use [TestCaseSource]
private static IEnumerable<TestCaseData> BoundaryValues()
{
    yield return new TestCaseData(double.MinValue).SetDescription("Min boundary");
    yield return new TestCaseData(double.MaxValue).SetDescription("Max boundary");
}

[TestCaseSource(nameof(BoundaryValues))]
public void ProcessValue_BoundaryInputs_DoesNotThrow(double input) { ... }

// Sequential numeric range â€” use [Range]
[Test]
public void Calibrate_ValidRange_AlwaysSucceeds([Range(0, 100, 10)] int calibrationPoint) { ... }
```

## NSubstitute Patterns

```csharp
// Create substitute
var channel = Substitute.For<IDeviceChannel>();

// Configure return value
channel.ReadRegisterAsync(0x01, Arg.Any<CancellationToken>()).Returns(42.5);

// Configure exception
channel.ReadRegisterAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
    .Throws(new DeviceNotConnectedException());

// Configure async return
channel.ConnectAsync(Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);

// Verify a call was made
await channel.Received(1).SendCommandAsync(Arg.Is<byte[]>(b => b[0] == 0x02), Arg.Any<CancellationToken>());

// Verify a call was NOT made
channel.DidNotReceive().Disconnect();
```

始终使用 `Arg.Any<T>()` 或 `Arg.Is<T>(...)` — 绝不把原始 `null` 作为 matcher argument 传入（NSubstitute 会将其视为 ambiguous value match）。

## Test Organisation

```csharp
[Test, Category("Unit")]        // Pure unit test â€” no I/O, no real device
[Test, Category("Integration")] // Involves file system, network, or real hardware
[Test, Explicit]                // Must be run explicitly â€” not part of normal suite
[Test, Ignore("Reason")]        // Temporarily skipped â€” must include reason
```

CI pre-approval 中只运行 unit tests：`dotnet test --filter Category=Unit`
