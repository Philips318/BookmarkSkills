#编写功能测试

这是最重要的可交付成果。Markdown文件是文档。功能测试文件是自动的安全网。使用项目约定命名：`test_functional.py`（Python/pytest）、`FunctionalSpec.scala`（Scala/ScalaTest）、`FunctionalTest.java`（Java/JUnit）、`functional.test.ts`（TypeScript/Jest）、`functional_test.go`（Go）等。

结构：三个测试组

使用测试框架提供的任何结构——类（Python/Java）、描述块（TypeScript/Jest）、特征（Scala）或子测试（Go）——将测试组织成三个逻辑组：```
Spec Requirements
    — One test per testable spec section
    — Each test's documentation cites the spec requirement

Fitness Scenarios
    — One test per QUALITY.md scenario (1:1 mapping)
    — Named to match: test_scenario_N_memorable_name (or equivalent convention)

Boundaries and Edge Cases
    — One test per defensive pattern from Step 5
    — Targets null guards, try/catch, normalization, fallbacks
```
测试计数启发式

**目标=（可测试的规范部分）+ （QUALITY.md场景）+（步骤5中的防御模式）**

例如：12个规范部分+ 10个场景+ 15个防御模式= 37个测试作为目标。

对于一个中等规模的项目（5-15个源文件），这通常会产生35-50个功能测试。显著的减少表明遗漏了需求或浅薄的探索。不要为了达到一个数字而填充物——每个测试都应该使用真实的项目代码并验证一个有意义的属性。

导入模式：匹配现有测试

在编写任何测试代码之前，阅读2-3个现有的测试文件，并确定它们是如何导入项目模块的。这是很关键的——项目处理导入的方式不同，出错意味着每个测试都因解析错误而失败。确定项目中使用的导入约定。无论现有测试使用什么模式，都要精确地复制它。不要猜测或发明不同的模式。

常用的语言模式：

**Python:**`sys.path.insert(0, "src/")`然后是裸导入；包导入（`from myproject.module import func`）；使用conftest.py路径操作的相对导入
- **Go:**同包测试（`package mypackage`）允许访问未导出的标识符；黑盒测试（`package mypackage_test`）测试只导出API；内部包可能需要特定的导入路径
**Java:**`import com.example.project.Module;`匹配包结构；测试源根必须镜像主源根
**TypeScript:**`import { func } from '../src/module'`带相对路径；`tsconfig.json`的路径别名（例如，`@/module`）
**Rust:**`use crate::module::function;`的单元测试在同一个crate；`use myproject::module::function;`用于`tests/`中的集成测试
**Scala:**`import com.example.project._`或`import com.example.project.{ClassA, ClassB}`；SBT布局在`src/test/scala/`中镜像`src/main/scala/`在编写测试之前创建测试设置每个测试框架都有一个共享设置机制。如果您的测试使用共享fixture或测试数据，则必须在编写测试之前创建安装文件。测试框架不会自动发现来自其他目录的fixture。

确定框架的设置机制（fixture、`@BeforeEach`、`beforeAll`、辅助函数、构建器模式等），并遵循项目现有测试中已经使用的约定。

**规则：所引用的每个fixture或测试助手都必须定义。**如果测试依赖于不存在的共享设置，则测试将在设置期间出错（而不是在断言期间失败）-产生看起来像通过的失败测试。**所有语言的首选方法：**编写内联创建自己的数据的测试。这消除了跨文件依赖。使用框架的临时目录支持和文字数据结构直接在每个测试函数中创建测试数据。

**编写完所有测试后，运行测试套件并检查安装错误。**无论框架如何分类，安装错误（未找到fixture，导入失败）都算作失败的测试。

没有占位符测试

每个测试都必须导入并调用实际的项目代码。如果测试主体是`pass`，或者它唯一的断言是`assert isinstance(errors, list)`，或者它检查的是像`assert hasattr(cls, 'validate')`这样的琐碎属性，那么删除它并编写一个真正的测试，或者完全删除它。不执行项目代码的测试比没有测试更糟糕——它会夸大数量，并产生错误的信心。如果您确实不能为防御模式编写有意义的测试（例如，它需要运行的服务器或外部服务），请在注释中将其标记为不可测试，而不是编写占位符。

先读后写：函数调用映射

在编写单个测试之前，构建一个函数调用映射。对于你计划测试的每个函数：1. **读取function/method签名** -不仅仅是名称，还有每个参数，其类型和默认值。
2. **阅读文档** - docstrings， Javadoc, TSDoc, ScalaDoc。它们通常指定返回类型、异常和边缘情况行为。
3. 读取一个调用它的现有测试——现有测试向您展示了确切的调用约定、fixture形状和断言模式。
4. **读取实际数据文件** -如果函数处理配置文件、模式文件或数据文件，则从项目中读取实际文件。您的测试装置必须与此形状完全匹配。

**常见的失败模式：**代理探索体系结构，从概念上理解函数的作用，然后用猜测的参数编写测试调用。测试失败是因为实际函数使用`(config, items_data, limit)`而不是`(items, seed, strategy)`。读取实际签名需要5秒，完全可以防止这种情况发生。**库版本意识：**检查项目的依赖清单（`requirements.txt`,`build.sbt`,`package.json`,`pom.xml`,`build.gradle`,`Cargo.toml`）来验证可用的版本。对可选依赖项使用测试框架的跳过机制（例如，`pytest.importorskip()`、`Assumptions.assumeTrue()`、`t.Skip()`、`#[ignore]`等）。

编写规范派生的测试

一节一节地浏览每个规范文档。对于每个部分，问：“这说明了什么可测试的需求？”然后编写一个测试。

每个测试应该：
1. **设置-加载夹具，创建测试数据，配置系统
2. **执行** -调用函数，运行管道，发出请求
3. **断言规范要求的特定属性

每个测试都应该包含一个可追溯性注释（通过文档字符串、显示名称或注释），引用它所验证的规范部分，例如，`[Req: formal — Design Doc §N] X should produce Y`。



什么是好的功能测试-可追踪** -测试名称，显示名称或文档注释说明它验证了哪个规范需求
- **Specific** -检查一个特定的属性，而不仅仅是“发生了什么事”
- **健壮** -使用真实数据（来自实际系统的夹具），而不是合成数据
- **交叉变量** -如果项目处理多种输入类型，测试所有输入类型
-在正确的层测试** -测试你关心的行为。如果需求是“无效数据不会产生错误的输出”，那么测试管道输出—不要仅仅测试模式验证器是否拒绝输入。

跨变量测试策略

如果项目处理多种输入类型，那么跨变体覆盖就是隐藏bug的地方。目标是大约30%的测试测试所有的变量——确切的百分比比确保每个横切属性都在所有的变量中测试重要得多。使用框架的参数化机制（例如，`@pytest.mark.parametrize`、`@ParameterizedTest`、`test.each`、表驱动测试、遍历用例）在所有变体上运行相同的断言逻辑。



如果参数化不合适，在单个测试中显式循环。

**哪些测试应该是交叉变量？**任何验证属性的测试，无论输入类型如何：实体标识，结构属性，所需链接，时间字段，特定于领域的语义。

**写完所有测试后，做一次跨变量审计。**将交叉变量测试数除以总数。如果低于30%，转换更多。

要避免的反模式

这些模式看起来像测试，但并不能捕捉到真正的bug：-只存在检查-找到一个正确的结果并不意味着所有的结果都是正确的。还要全面核对计数或核实。
- **仅存在断言** -断言一个值存在只证明存在，而不是正确性。断言实际值。
- **单变量测试** -测试一种输入类型，并希望其他工作。使用参数化。
- **Positive-only testing -你必须测试无效的输入不会产生坏的输出。
- **不完整的否定断言** -当测试拒绝时，断言所有结果都不存在，而不仅仅是一个。
-捕获异常而不是检查输出** -以特定方式测试代码崩溃并不是测试它是否正确处理输入。测试输出。

异常捕获反模式的细节```python
# WRONG: tests the validation mechanism
def test_bad_value_rejected(fixture):
    fixture.field = "invalid"  # Schema rejects this!
    with pytest.raises(ValidationError):
        process(fixture)
    # Tells you nothing about output

# RIGHT: tests the requirement
def test_bad_value_not_in_output(fixture):
    fixture.field = None  # Schema accepts None for Optional
    output = process(fixture)
    assert field_property not in output  # Bad data absent
    assert expected_type in output  # Rest still works
```
这种模式在每种语言中都是一样的：不要测试验证机制是否会拒绝错误的输入，而要测试当模式接受给定的边缘情况输入时系统是否会产生正确的输出。错误的方法测试实现（验证器）；正确的方法测试需求（输出）。



在选择突变值之前，始终检查步骤5b模式映射。

在正确的层进行测试

问：“说明书上说应该怎么做？”规范说的是“无效数据不应该出现在输出中”——而不是“验证层应该拒绝它”。测试规范，而不是实现。

**例外：**当规范明确要求特定机制时（例如，“必须在模式层快速失败”），测试该机制是合适的。但这种情况很少见。

适合目的场景测试对于QUALITY.md中的每个场景，编写一个测试。这是一个1:1的映射。每个测试应该包括一个引用场景的可追溯性注释，例如，`[Req: formal — QUALITY.md Scenario 1]`，并命名为与场景的容易记住的名称相匹配。



边界和负面测试

步骤5中的每个防御模式一个测试。每个测试应该包括一个引用防御模式的可追溯性注释，例如，`[Req: inferred — from function_name() guard] guards against X`。

对于每个边界测试：
1. 改变输入以触发防御代码路径（使用模式接受的值）
2. 处理突变的输入
3. 断言优美的处理——尽管有边缘情况输入，结果仍然有效



在选择突变值时使用步骤5b模式映射。每个突变都必须使用模式接受的值。系统方法:
- **缺少字段** -缺少可选字段？设置为空。
- **错误的类型** -字段得到不同的类型？使用模式有效的替代方案。
- **空值** -空列表？空字符串?空的东西吗?
- **边界值** -零，负，最大，第一，最后。
- **跨模块边界** -模块A产生不寻常但有效的输出- B处理它吗？

如果你发现了10个以上的防御模式，但只写了4个边界测试，那就回去写更多。目标是1:1的比例。