---
name: csharp-nunit
description: 'Get best practices for NUnit unit testing, including data-driven tests'
---
单元最佳实践

你的目标是帮助我用NUnit编写有效的单元测试，包括标准和数据驱动的测试方法。

##项目设置

-使用单独的测试项目，命名约定为`[ProjectName].Tests`-参考Microsoft.NET.Test。Sdk、NUnit和NUnit3TestAdapter包
-创建与被测试类匹配的测试类（例如，`Calculator`对应`CalculatorTests`）
-使用。. NET SDK测试命令：`dotnet test`用于运行测试

##测试结构

-应用`[TestFixture]`属性到测试类
—测试方法使用`[Test]`属性
-遵循安排-行动-断言（AAA）模式
—使用模式`MethodName_Scenario_ExpectedBehavior`命名测试
-使用`[SetUp]`和`[TearDown]`进行每个测试的设置和拆除
-使用`[OneTimeSetUp]`和`[OneTimeTearDown]`为每类设置和拆除
-使用`[SetUpFixture]`进行装配级设置和拆卸

##标准测试-将测试集中在单个行为上
—避免在一种测试方法中测试多个行为
-使用表达意图的明确断言
—只包含验证测试用例所需的断言
-使测试独立和幂等（可以以任何顺序运行）
—避免测试相互依赖

数据驱动测试

-使用`[TestCase]`内联测试数据
-使用`[TestCaseSource]`编程生成的测试数据
—简单的参数组合使用`[Values]`-使用`[ValueSource]`属性或基于方法的数据源
-使用`[Random]`随机数字测试值
—使用`[Range]`作为顺序数字测试值
—多个参数合并时使用“`[Combinatorial]`”或“`[Pairwise]`”

# #断言-使用约束模型`Assert.That`（首选NUnit样式）
-使用约束，如`Is.EqualTo`，`Is.SameAs`,`Contains.Item`-使用`Assert.AreEqual`进行简单的值相等（经典风格）
—使用`CollectionAssert`进行集合比较
-使用`StringAssert`进行特定字符串的断言
—使用“`Assert.Throws<T>`”或“`Assert.ThrowsAsync<T>`”测试异常
-在断言中使用描述性消息以明确失败

##嘲笑和孤立

-考虑使用Moq或NSubstitute和NUnit
-模拟依赖关系以隔离被测单元
-使用接口来促进模拟
-考虑使用DI容器进行复杂的测试设置

##测试机构-按功能或组件分组测试
-使用类别与`[Category("CategoryName")]`-必要时使用`[Order]`控制测试执行顺序
—使用“`[Author("DeveloperName")]`”表示所有权
-使用`[Description]`提供额外的测试信息
-对于不应该自动运行的测试，请考虑使用`[Explicit]`—使用`[Ignore("Reason")]`临时跳过测试