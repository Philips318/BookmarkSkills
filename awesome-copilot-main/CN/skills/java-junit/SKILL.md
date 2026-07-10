---
name: java-junit
description: 'Get best practices for JUnit 5 unit testing, including data-driven tests'
---
# JUnit 5+最佳实践

你的目标是帮助我用JUnit 5编写有效的单元测试，包括标准和数据驱动的测试方法。

##项目设置

-使用标准的Maven或Gradle项目结构。
-将测试源代码放在`src/test/java`中。
—包括参数化测试的`junit-jupiter-api`、`junit-jupiter-engine`和`junit-jupiter-params`的依赖项。
—使用构建工具命令运行测试：`mvn test`或`gradle test`。

##测试结构

-测试类应该有一个`Test`后缀，例如，`Calculator`类的`CalculatorTest`。
-测试方法使用`@Test`。
-遵循安排-行动-断言（AAA）模式。
-使用描述性约定命名测试，如`methodName_should_expectedBehavior_when_scenario`。
-使用`@BeforeEach`和`@AfterEach`进行每个测试的设置和拆除。
-使用`@BeforeAll`和`@AfterAll`对每个类的设置和拆除（必须是静态方法）。
-使用`@DisplayName`为测试类和方法提供一个人类可读的名称。

##标准测试-将测试集中在单个行为上。
—避免在一种测试方法中测试多种情况。
-使测试独立和幂等（可以以任何顺序运行）。
—避免测试相互依赖。

数据驱动（参数化）测试

—使用`@ParameterizedTest`将方法标记为参数化测试。
-使用`@ValueSource`的简单文字值（字符串，整数等）。
-使用`@MethodSource`来引用提供测试参数的工厂方法，如`Stream`，`Collection`等。
—使用`@CsvSource`作为内联逗号分隔值。
—“`@CsvFileSource`”使用类路径中的CSV文件。
—使用`@EnumSource`使用枚举常量。

# #断言-使用`org.junit.jupiter.api.Assertions`的静态方法（例如，`assertEquals`,`assertTrue`,`assertNotNull`）。
对于更加流畅和可读的断言，可以考虑使用AssertJ （`assertThat(...).is...`）这样的库。
—使用“`assertThrows`”或“`assertDoesNotThrow`”进行异常测试。
—使用`assertAll`对相关断言进行分组，以确保在测试失败之前检查所有断言。
-在断言中使用描述性消息，以明确失败。

##嘲笑和孤立

-使用像Mockito这样的模拟框架来为依赖关系创建模拟对象。
-使用`@Mock`和`@InjectMocks`注解从Mockito简化模拟的创建和注入。
-使用接口来促进模拟。

##测试机构-使用包按功能或组件分组测试。
-使用`@Tag`对测试进行分类（例如，`@Tag("fast")`、`@Tag("integration")`）。
-必要时使用`@TestMethodOrder(MethodOrderer.OrderAnnotation.class)`和`@Order`控制测试执行顺序。
-使用`@Disabled`暂时跳过测试方法或类，并提供原因。
-使用`@Nested`在嵌套的内部类中分组测试，以获得更好的组织和结构。