---
name: csharp-xunit
description: 'Get best practices for XUnit unit testing, including data-driven tests'
---
# XUnit最佳实践

您的目标是帮助我用XUnit编写有效的单元测试，包括标准和数据驱动的测试方法。

##项目设置

-使用单独的测试项目，命名约定为`[ProjectName].Tests`-参考Microsoft.NET.Test。Sdk、xunit和xunit.runner.visualstudio包
-创建与被测试类匹配的测试类（例如，`Calculator`对应`CalculatorTests`）
-使用。. NET SDK测试命令：`dotnet test`用于运行测试

##测试结构

-不需要测试类属性（不像MSTest/NUnit）
-使用带有`[Fact]`属性的基于事实的测试进行简单测试
-遵循安排-行动-断言（AAA）模式
—使用模式`MethodName_Scenario_ExpectedBehavior`命名测试
-使用构造器安装和`IDisposable.Dispose()`拆除
-使用`IClassFixture<T>`作为类中测试之间的共享上下文
-使用`ICollectionFixture<T>`用于多个测试类之间的共享上下文

##标准测试-将测试集中在单个行为上
—避免在一种测试方法中测试多个行为
-使用表达意图的明确断言
—只包含验证测试用例所需的断言
-使测试独立和幂等（可以以任何顺序运行）
—避免测试相互依赖

数据驱动测试

—结合数据源属性使用`[Theory]`-内联测试数据使用`[InlineData]`-基于方法的测试数据使用`[MemberData]`-基于类的测试数据使用`[ClassData]`—通过`DataAttribute`创建自定义数据属性
—在数据驱动测试中使用有意义的参数名

# #断言—使用`Assert.Equal`表示值相等
—参考相等使用`Assert.Same`—布尔条件使用`Assert.True`/`Assert.False`—使用`Assert.Contains`/`Assert.DoesNotContain`作为集合
-使用`Assert.Matches`/`Assert.DoesNotMatch`进行正则表达式模式匹配
—使用“`Assert.Throws<T>`”或“`await Assert.ThrowsAsync<T>`”测试异常
-使用流畅的断言库以获得更可读的断言

##嘲笑和孤立

-考虑使用Moq或NSubstitute和XUnit
-模拟依赖关系以隔离被测单元
-使用接口来促进模拟
-考虑使用DI容器进行复杂的测试设置

##测试机构

-按功能或组件分组测试
-使用`[Trait("Category", "CategoryName")]`进行分类
-使用集合fixture对具有共享依赖关系的测试进行分组
—考虑输出帮助器（`ITestOutputHelper`）进行测试诊断
—在fact/theory属性中有条件跳过`Skip = "reason"`的测试