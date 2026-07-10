---
name: csharp-mstest
description: 'Get best practices for MSTest 3.x/4.x unit testing, including modern assertion APIs and data-driven tests'
---
MSTest最佳实践（MSTest3.x/4.x）

你的目标是帮助我用现代的MSTest编写有效的单元测试，使用当前的api和最佳实践。

##项目设置

-使用单独的测试项目，命名约定为`[ProjectName].Tests`—参考MSTest 3。x+ NuGet包（包括分析器）
—考虑使用MSTest。用于简化项目设置的Sdk
-使用`dotnet test`运行测试

测试类结构

-测试类使用`[TestClass]`属性
- **默认密封测试类**性能和设计清晰度
-使用`[TestMethod]`作为测试方法（优先于`[DataTestMethod]`）
-遵循安排-行为-断言（AAA）模式
—使用模式`MethodName_Scenario_ExpectedBehavior`命名测试```csharp
[TestClass]
public sealed class CalculatorTests
{
    [TestMethod]
    public void Add_TwoPositiveNumbers_ReturnsSum()
    {
        // Arrange
        var calculator = new Calculator();

        // Act
        var result = calculator.Add(2, 3);

        // Assert
        Assert.AreEqual(5, result);
    }
}
```
测试生命周期

- **优先使用构造函数而不是`[TestInitialize]`** -启用`readonly`字段并遵循标准c#模式
—使用`[TestCleanup]`进行清理，即使测试失败也必须运行
-当需要异步设置时，将构造函数与async`[TestInitialize]`组合```csharp
[TestClass]
public sealed class ServiceTests
{
    private readonly MyService _service;  // readonly enabled by constructor

    public ServiceTests()
    {
        _service = new MyService();
    }

    [TestInitialize]
    public async Task InitAsync()
    {
        // Use for async initialization only
        await _service.WarmupAsync();
    }

    [TestCleanup]
    public void Cleanup() => _service.Reset();
}
```
执行命令

1. **程序集初始化** -`[AssemblyInitialize]`（每个测试程序集一次）
2. **类初始化** -`[ClassInitialize]`（每个测试类一次）
3. **测试初始化**（每个测试方法）：
1. 构造函数
2. 设置`TestContext`属性
3.`[TestInitialize]`4. **测试执行** -测试方法运行
5. **测试清理**（针对每个测试方法）：
1.`[TestCleanup]`2.`DisposeAsync`（如果实现）
3.`Dispose`（如果实现）
6. **类清理** -`[ClassCleanup]`（每个测试类一次）
7. **程序集清理** -`[AssemblyCleanup]`（每个测试程序集一次）

现代断言api

MSTest提供了三个断言类：`Assert`、`StringAssert`和`CollectionAssert`。

Assert类-核心断言```csharp
// Equality
Assert.AreEqual(expected, actual);
Assert.AreNotEqual(notExpected, actual);
Assert.AreSame(expectedObject, actualObject);      // Reference equality
Assert.AreNotSame(notExpectedObject, actualObject);

// Null checks
Assert.IsNull(value);
Assert.IsNotNull(value);

// Boolean
Assert.IsTrue(condition);
Assert.IsFalse(condition);

// Fail/Inconclusive
Assert.Fail("Test failed due to...");
Assert.Inconclusive("Test cannot be completed because...");
```
异常测试（优先于`[ExpectedException]`）```csharp
// Assert.Throws - matches TException or derived types
var ex = Assert.Throws<ArgumentException>(() => Method(null));
Assert.AreEqual("Value cannot be null.", ex.Message);

// Assert.ThrowsExactly - matches exact type only
var ex = Assert.ThrowsExactly<InvalidOperationException>(() => Method());

// Async versions
var ex = await Assert.ThrowsAsync<HttpRequestException>(async () => await client.GetAsync(url));
var ex = await Assert.ThrowsExactlyAsync<InvalidOperationException>(async () => await Method());
```
###集合断言（Assert类）```csharp
Assert.Contains(expectedItem, collection);
Assert.DoesNotContain(unexpectedItem, collection);
Assert.ContainsSingle(collection);  // exactly one element
Assert.HasCount(5, collection);
Assert.IsEmpty(collection);
Assert.IsNotEmpty(collection);
```
###字符串断言（断言类）```csharp
Assert.Contains("expected", actualString);
Assert.StartsWith("prefix", actualString);
Assert.EndsWith("suffix", actualString);
Assert.DoesNotStartWith("prefix", actualString);
Assert.DoesNotEndWith("suffix", actualString);
Assert.MatchesRegex(@"\d{3}-\d{4}", phoneNumber);
Assert.DoesNotMatchRegex(@"\d+", textOnly);
```
比较断言```csharp
Assert.IsGreaterThan(lowerBound, actual);
Assert.IsGreaterThanOrEqualTo(lowerBound, actual);
Assert.IsLessThan(upperBound, actual);
Assert.IsLessThanOrEqualTo(upperBound, actual);
Assert.IsInRange(actual, low, high);
Assert.IsPositive(number);
Assert.IsNegative(number);
```
###类型断言```csharp
// MSTest 3.x - uses out parameter
Assert.IsInstanceOfType<MyClass>(obj, out var typed);
typed.DoSomething();

// MSTest 4.x - returns typed result directly
var typed = Assert.IsInstanceOfType<MyClass>(obj);
typed.DoSomething();

Assert.IsNotInstanceOfType<WrongType>(obj);
```
# # #断言。那（MSTest 4.0+）```csharp
Assert.That(result.Count > 0);  // Auto-captures expression in failure message
```
StringAssert类

**注意：**优先使用`Assert`类的等效类（例如，`Assert.Contains("expected", actual)`优于`StringAssert.Contains(actual, "expected")`）。```csharp
StringAssert.Contains(actualString, "expected");
StringAssert.StartsWith(actualString, "prefix");
StringAssert.EndsWith(actualString, "suffix");
StringAssert.Matches(actualString, new Regex(@"\d{3}-\d{4}"));
StringAssert.DoesNotMatch(actualString, new Regex(@"\d+"));
```
CollectionAssert类

b> **注意：**在可用的情况下，首选`Assert`类等效（例如，`Assert.Contains`）。```csharp
// Containment
CollectionAssert.Contains(collection, expectedItem);
CollectionAssert.DoesNotContain(collection, unexpectedItem);

// Equality (same elements, same order)
CollectionAssert.AreEqual(expectedCollection, actualCollection);
CollectionAssert.AreNotEqual(unexpectedCollection, actualCollection);

// Equivalence (same elements, any order)
CollectionAssert.AreEquivalent(expectedCollection, actualCollection);
CollectionAssert.AreNotEquivalent(unexpectedCollection, actualCollection);

// Subset checks
CollectionAssert.IsSubsetOf(subset, superset);
CollectionAssert.IsNotSubsetOf(notSubset, collection);

// Element validation
CollectionAssert.AllItemsAreInstancesOfType(collection, typeof(MyClass));
CollectionAssert.AllItemsAreNotNull(collection);
CollectionAssert.AllItemsAreUnique(collection);
```
数据驱动测试

# # #我们```csharp
[TestMethod]
[DataRow(1, 2, 3)]
[DataRow(0, 0, 0, DisplayName = "Zeros")]
[DataRow(-1, 1, 0, IgnoreMessage = "Known issue #123")]  // MSTest 3.8+
public void Add_ReturnsSum(int a, int b, int expected)
{
    Assert.AreEqual(expected, Calculator.Add(a, b));
}
```
# # #动态数据

数据源可以返回以下任何类型：

-`IEnumerable<(T1, T2, ...)>`(ValueTuple) - **首选**，提供类型安全（MSTest 3.7+）
-`IEnumerable<Tuple<T1, T2, ...>>`-提供类型安全`IEnumerable<TestDataRow>`-提供类型安全以及对测试元数据（显示名称，类别）的控制
-`IEnumerable<object[]>`- **不受欢迎**，没有类型安全

**注：**创建新的测试数据方法时，优先选择`ValueTuple`或`TestDataRow`，而不是`IEnumerable<object[]>`。`object[]`方法不提供编译时类型检查，并且可能由于类型不匹配而导致运行时错误。```csharp
[TestMethod]
[DynamicData(nameof(TestData))]
public void DynamicTest(int a, int b, int expected)
{
    Assert.AreEqual(expected, Calculator.Add(a, b));
}

// ValueTuple - preferred (MSTest 3.7+)
public static IEnumerable<(int a, int b, int expected)> TestData =>
[
    (1, 2, 3),
    (0, 0, 0),
];

// TestDataRow - when you need custom display names or metadata
public static IEnumerable<TestDataRow<(int a, int b, int expected)>> TestDataWithMetadata =>
[
    new((1, 2, 3)) { DisplayName = "Positive numbers" },
    new((0, 0, 0)) { DisplayName = "Zeros" },
    new((-1, 1, 0)) { DisplayName = "Mixed signs", IgnoreMessage = "Known issue #123" },
];

// IEnumerable<object[]> - avoid for new code (no type safety)
public static IEnumerable<object[]> LegacyTestData =>
[
    [1, 2, 3],
    [0, 0, 0],
];
```
# #和TestContext`TestContext`类提供测试运行信息、取消支持和输出方法。
参见[TestContext文档]（https://learn.microsoft.com/dotnet/core/testing/unit-testing-mstest-writing-tests-testcontext）获得完整的参考。

###访问TestContext```csharp
// Property (MSTest suppresses CS8618 - don't use nullable or = null!)
public TestContext TestContext { get; set; }

// Constructor injection (MSTest 3.6+) - preferred for immutability
[TestClass]
public sealed class MyTests
{
    private readonly TestContext _testContext;

    public MyTests(TestContext testContext)
    {
        _testContext = testContext;
    }
}

// Static methods receive it as parameter
[ClassInitialize]
public static void ClassInit(TestContext context) { }

// Optional for cleanup methods (MSTest 3.6+)
[ClassCleanup]
public static void ClassCleanup(TestContext context) { }

[AssemblyCleanup]
public static void AssemblyCleanup(TestContext context) { }
```
取消令牌

总是使用`TestContext.CancellationToken`与`[Timeout]`协同消去：```csharp
[TestMethod]
[Timeout(5000)]
public async Task LongRunningTest()
{
    await _httpClient.GetAsync(url, TestContext.CancellationToken);
}
```
###测试运行属性```csharp
TestContext.TestName              // Current test method name
TestContext.TestDisplayName       // Display name (3.7+)
TestContext.CurrentTestOutcome    // Pass/Fail/InProgress
TestContext.TestData              // Parameterized test data (3.7+, in TestInitialize/Cleanup)
TestContext.TestException         // Exception if test failed (3.7+, in TestCleanup)
TestContext.DeploymentDirectory   // Directory with deployment items
```
输出和结果文件```csharp
// Write to test output (useful for debugging)
TestContext.WriteLine("Processing item {0}", itemId);

// Attach files to test results (logs, screenshots)
TestContext.AddResultFile(screenshotPath);

// Store/retrieve data across test methods
TestContext.Properties["SharedKey"] = computedValue;
```
##高级功能

###重试片状测试（MSTest 3.9+）```csharp
[TestMethod]
[Retry(3)]
public void FlakyTest() { }
```
条件执行（MSTest 3.10+）

跳过或运行基于OS或CI环境的测试：```csharp
// OS-specific tests
[TestMethod]
[OSCondition(OperatingSystems.Windows)]
public void WindowsOnlyTest() { }

[TestMethod]
[OSCondition(OperatingSystems.Linux | OperatingSystems.MacOS)]
public void UnixOnlyTest() { }

[TestMethod]
[OSCondition(ConditionMode.Exclude, OperatingSystems.Windows)]
public void SkipOnWindowsTest() { }

// CI environment tests
[TestMethod]
[CICondition]  // Runs only in CI (default: ConditionMode.Include)
public void CIOnlyTest() { }

[TestMethod]
[CICondition(ConditionMode.Exclude)]  // Skips in CI, runs locally
public void LocalOnlyTest() { }
```
# # #并行化```csharp
// Assembly level
[assembly: Parallelize(Workers = 4, Scope = ExecutionScope.MethodLevel)]

// Disable for specific class
[TestClass]
[DoNotParallelize]
public sealed class SequentialTests { }
```
工作项可追溯性（MSTest 3.8+）

将测试链接到工作项以实现测试报告中的可追溯性：```csharp
// Azure DevOps work items
[TestMethod]
[WorkItem(12345)]  // Links to work item #12345
public void Feature_Scenario_ExpectedBehavior() { }

// Multiple work items
[TestMethod]
[WorkItem(12345)]
[WorkItem(67890)]
public void Feature_CoversMultipleRequirements() { }

// GitHub issues (MSTest 3.8+)
[TestMethod]
[GitHubWorkItem("https://github.com/owner/repo/issues/42")]
public void BugFix_Issue42_IsResolved() { }
```
工作项关联出现在测试结果中，可用于：
跟踪测试覆盖需求
-将bug修复链接到回归测试
在CI/CD管道中生成可追溯性报告

要避免的常见错误```csharp
// ❌ Wrong argument order
Assert.AreEqual(actual, expected);
// ✅ Correct
Assert.AreEqual(expected, actual);

// ❌ Using ExpectedException (obsolete)
[ExpectedException(typeof(ArgumentException))]
// ✅ Use Assert.Throws
Assert.Throws<ArgumentException>(() => Method());

// ❌ Using LINQ Single() - unclear exception
var item = items.Single();
// ✅ Use ContainsSingle - better failure message
var item = Assert.ContainsSingle(items);

// ❌ Hard cast - unclear exception
var handler = (MyHandler)result;
// ✅ Type assertion - shows actual type on failure
var handler = Assert.IsInstanceOfType<MyHandler>(result);

// ❌ Ignoring cancellation token
await client.GetAsync(url, CancellationToken.None);
// ✅ Flow test cancellation
await client.GetAsync(url, TestContext.CancellationToken);

// ❌ Making TestContext nullable - leads to unnecessary null checks
public TestContext? TestContext { get; set; }
// ❌ Using null! - MSTest already suppresses CS8618 for this property
public TestContext TestContext { get; set; } = null!;
// ✅ Declare without nullable or initializer - MSTest handles the warning
public TestContext TestContext { get; set; }
```
##测试机构

-按功能或组件分组测试
—使用`[TestCategory("Category")]`过滤
-使用`[TestProperty("Name", "Value")]`自定义元数据（例如，`[TestProperty("Bug", "12345")]`）
—关键测试使用`[Priority(1)]`-启用相关的MSTest分析器（MSTEST0020为构造器首选）

##嘲笑和孤立

-使用Moq或NSubstitute代替mock依赖
-使用接口来促进模拟
-模拟依赖关系以隔离被测单元