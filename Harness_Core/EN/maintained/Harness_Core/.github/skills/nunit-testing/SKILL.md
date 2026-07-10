---
name: nunit-testing
description: NUnit 3.x unit test patterns with NSubstitute mocking: test class structure, AAA layout, data-driven tests (TestCase, TestCaseSource, Range), lifecycle attributes, and substitute verification. Use when writing or reviewing non-BDD unit tests in the CT project.
---

# NUnit Testing Skill

NUnit 3.x patterns for unit tests (non-BDD) in the CT project. Mocking library: **NSubstitute**.

## Test Quality Rules

- Test code follows all production coding guidelines (naming, complexity, zero warnings)
- **One assert per test** — a single failure gives a clear indication of what failed
- Define test scope precisely — unit (one class) or module (one component); everything outside is mocked
- Tests have a single reason to fail; split scenarios into separate tests
- No ordering assumptions — each test is fully self-contained and independent
- No environment assumptions — unit tests run anywhere, modify no files/registry/OS settings
- Never name a test after a defect ID; follow the standard naming convention
- Never share mutable state between tests via static fields

## What to Test

- Expected behaviour for valid inputs
- Null, empty, and missing values
- Boundary values: exact boundary, just inside, just outside
- All documented exceptions from dependencies
- Invalid call sequences (e.g. calling `Execute` before `Init`)
- Thread-safety: multiple concurrent calls to shared-state methods
- High-load and timing requirements (measure min/max/mean over multiple runs)

## Project Setup

- Test projects named `[ProjectName].Tests`, located at `Src/{ProjectDir}/Test/`
- Required NuGet packages: `Microsoft.NET.Test.Sdk`, `NUnit`, `NUnit3TestAdapter`, `NSubstitute`
- Run tests with: `dotnet test Src\{repo}Impl.sln --no-build`

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

| Attribute | Scope | Use for |
|-----------|-------|---------|
| `[SetUp]` | Per test | Create fresh SUT and substitutes |
| `[TearDown]` | Per test | Dispose resources |
| `[OneTimeSetUp]` | Per fixture | Expensive shared setup (e.g. database seed) |
| `[OneTimeTearDown]` | Per fixture | Corresponding teardown |
| `[SetUpFixture]` | Assembly | Assembly-level setup/teardown |

Use `[OneTimeSetUp]` only for truly immutable shared resources — prefer `[SetUp]` for creating substitutes and SUT instances.

## Test Naming Convention

`MethodName_Scenario_ExpectedResult`

Examples:
- `GetMeasurementAsync_WhenDeviceConnected_ReturnsCurrentValue`
- `SendCommand_WithNegativeValue_ThrowsArgumentOutOfRangeException`
- `Connect_WhenAlreadyConnected_DoesNotThrow`

## Assertions â€” Constraint-Based (Preferred)

```csharp
Assert.That(result, Is.EqualTo(42.5).Within(0.001));       // float equality
Assert.That(list, Has.Count.EqualTo(3));                    // collection count
Assert.That(list, Contains.Item(expectedItem));             // collection membership
Assert.That(action, Throws.TypeOf<ArgumentException>());    // exceptions
Assert.That(result, Is.Not.Null.And.GreaterThan(0));        // compound
Assert.That(str, Does.StartWith("Error:"));                 // string
```

Do NOT use legacy assertions:
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

Always use `Arg.Any<T>()` or `Arg.Is<T>(...)` — never pass raw `null` as a matcher argument (NSubstitute treats it as an ambiguous value match).

## Test Organisation

```csharp
[Test, Category("Unit")]        // Pure unit test â€” no I/O, no real device
[Test, Category("Integration")] // Involves file system, network, or real hardware
[Test, Explicit]                // Must be run explicitly â€” not part of normal suite
[Test, Ignore("Reason")]        // Temporarily skipped â€” must include reason
```

Run only unit tests in CI pre-approval: `dotnet test --filter Category=Unit`
