---
name: csharp-tunit
description: 'Get best practices for TUnit unit testing, including data-driven tests'
---
# unit最佳实践

您的目标是帮助我用TUnit编写有效的单元测试，包括标准和数据驱动的测试方法。

##项目设置

-使用单独的测试项目，命名约定为`[ProjectName].Tests`—参考TUnit包和TUnit。流畅断言的断言
-创建与被测试类匹配的测试类（例如，`Calculator`对应`CalculatorTests`）
-使用。. NET SDK测试命令：`dotnet test`用于运行测试
-单位要求。NET 8.0或更高版本

##测试结构-不需要测试类属性（如xUnit/NUnit）
-使用`[Test]`属性的测试方法（不像xUnit的`[Fact]`）
-遵循安排-行动-断言（AAA）模式
—使用模式`MethodName_Scenario_ExpectedBehavior`命名测试
-使用生命周期挂钩：`[Before(Test)]`用于安装，`[After(Test)]`用于拆卸
—使用`[Before(Class)]`和`[After(Class)]`作为类中测试之间的共享上下文
-使用`[Before(Assembly)]`和`[After(Assembly)]`跨测试类共享上下文
- unit支持高级生命周期钩子，如`[Before(TestSession)]`和`[After(TestSession)]`##标准测试

-将测试集中在单个行为上
—避免在一种测试方法中测试多个行为
-使用TUnit流畅的断言语法与`await Assert.That()`—只包含验证测试用例所需的断言
-使测试独立和幂等（可以以任何顺序运行）
避免测试相互依赖（如果需要，使用`[DependsOn]`属性）

数据驱动测试-使用`[Arguments]`属性内联测试数据（相当于xUnit的`[InlineData]`）
-使用`[MethodData]`作为基于方法的测试数据（相当于xUnit的`[MemberData]`）
-基于类的测试数据使用`[ClassData]`-通过实现`ITestDataSource`创建自定义数据源
—在数据驱动测试中使用有意义的参数名
—同一个测试方法可以使用多个`[Arguments]`属性

# #断言

—使用`await Assert.That(value).IsEqualTo(expected)`表示值相等
—参考相等使用`await Assert.That(value).IsSameReferenceAs(expected)`—布尔条件使用`await Assert.That(value).IsTrue()`或`await Assert.That(value).IsFalse()`—使用`await Assert.That(collection).Contains(item)`或`await Assert.That(collection).DoesNotContain(item)`作为集合
—使用`await Assert.That(value).Matches(pattern)`进行正则表达式模式匹配
—使用“`await Assert.That(action).Throws<TException>()`”或“`await Assert.That(asyncAction).ThrowsAsync<TException>()`”测试异常
-使用`.And`操作符链接断言：`await Assert.That(value).IsNotNull().And.IsEqualTo(expected)`—替代条件使用`.Or`操作符：`await Assert.That(value).IsEqualTo(1).Or.IsEqualTo(2)`-使用`.Within(tolerance)`的日期时间和数字比较与公差
-所有断言都是异步的，必须等待

##高级功能—使用“`[Repeat(n)]`”重复多次测试
—使用`[Retry(n)]`设置失败后自动重试
—使用`[ParallelLimit<T>]`控制并行执行限制
—使用`[Skip("reason")]`有条件跳过测试
—使用`[DependsOn(nameof(OtherTest))]`创建测试依赖项
—使用“`[Timeout(milliseconds)]`”设置测试超时时间
-通过扩展unit的基本属性创建自定义属性

##测试机构

-按功能或组件分组测试
-使用`[Category("CategoryName")]`进行测试分类
—使用`[DisplayName("Custom Test Name")]`作为自定义测试名
—考虑使用`TestContext`作为测试诊断和信息
-对特定于平台的测试使用自定义`[WindowsOnly]`等条件属性

性能和并行执行- unit默认并行运行测试（不像xUnit需要显式配置）
—使用`[NotInParallel]`禁用特定测试的并行执行
-使用`[ParallelLimit<T>]`与自定义限制类来控制并发
—默认情况下，同一类中的测试顺序运行
—负载测试场景使用`[Repeat(n)]`和`[ParallelLimit<T>]`##从xUnit迁移

—将`[Fact]`替换为`[Test]`—将“`[Theory]`”替换为“`[Test]`”，数据使用“`[Arguments]`”
—将`[InlineData]`替换为`[Arguments]`—将“`[MemberData]`”替换为“`[MethodData]`”
—将`Assert.Equal`替换为`await Assert.That(actual).IsEqualTo(expected)`—将“`Assert.True`”替换为“`await Assert.That(condition).IsTrue()`”
—将`Assert.Throws<T>`替换为`await Assert.That(action).Throws<T>()`—将constructor/IDisposable替换为`[Before(Test)]`/`[After(Test)]`—将“`IClassFixture<T>`”替换为“`[Before(Class)]`/`[After(Class)]`”

**为什么使用unit而不是xUnit？**TUnit提供了一种现代、快速和灵活的测试体验，它具有xUnit中没有的高级特性，比如异步断言、更精细的生命周期挂钩和改进的数据驱动测试功能。TUnit流畅的断言提供了更清晰和更具表现力的测试验证，使其特别适合复杂的测试。网络项目。