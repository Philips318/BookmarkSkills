---
description: 'Playwright .NET test generation instructions'
applyTo: '**'
---
#剧作家。NET测试生成指令

##测试编写指南

代码质量标准

**定位器**：优先考虑面向用户，基于角色的定位器（`GetByRole`,`GetByLabel`，`GetByText`等）的弹性和可访问性。使用`await Test.StepAsync()`对交互进行分组，并改进测试的可读性和报告。
**断言**：使用自动重试web优先断言。这些断言使用来自剧作家断言的`Expect()`（例如，`await Expect(locator).ToHaveTextAsync()`）。避免检查可见性，除非专门测试可见性变化。
- **超时：依赖于剧作家的内置自动等待机制。避免硬编码等待或增加默认超时。
- **清晰度**：使用描述性测试和步骤标题，清楚地说明意图。只在解释复杂的逻辑或不明显的交互时添加注释。

测试结构- **用法**：以`using Microsoft.Playwright;`开头，MSTest以`using Microsoft.Playwright.Xunit;`或`using Microsoft.Playwright.NUnit;`或`using Microsoft.Playwright.MSTest;`开头。
- **组织**：创建从`PageTest`继承的测试类（可在NUnit， xUnit和MSTest包中使用）或使用`IClassFixture<PlaywrightFixture>`为xUnit定制fixture。将某一特性的相关测试分组到同一测试类中。
- **Setup**：使用`[SetUp]`（NUnit）、`[TestInitialize]`（MSTest）或构造函数初始化（xUnit）进行所有测试共同的设置操作（例如，导航到页面）。
- **标题**：使用适当的测试属性（`[Test]`用于NUnit，`[Fact]`用于xUnit，`[TestMethod]`用于MSTest）和描述性方法名称，遵循c#命名约定（例如，`SearchForMovieByTitle`）。

###文件组织- **位置**：将所有测试文件存储在`Tests/`目录或按特性组织。
- **命名**：使用约定`<FeatureOrPage>Tests.cs`（例如，`LoginTests.cs`,`SearchTests.cs`）。
- **范围**：针对每个主要应用程序功能或页面一个测试类。

断言最佳实践

- **UI结构**：使用`ToMatchAriaSnapshotAsync`来验证组件的可访问性树结构。这提供了一个全面的和可访问的快照。
- **元素计数**：使用`ToHaveCountAsync`断言定位器找到的元素数量。
- **文本内容**:`ToHaveTextAsync`用于精确文本匹配，`ToContainTextAsync`用于部分匹配。
—**导航**：操作后使用`ToHaveURLAsync`验证页面URL。

示例测试结构```csharp
using Microsoft.Playwright;
using Microsoft.Playwright.Xunit;
using static Microsoft.Playwright.Assertions;

namespace PlaywrightTests;

public class MovieSearchTests : PageTest
{
    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        // Navigate to the application before each test
        await Page.GotoAsync("https://debs-obrien.github.io/playwright-movies-app");
    }

    [Fact]
    public async Task SearchForMovieByTitle()
    {
        await Test.StepAsync("Activate and perform search", async () =>
        {
            await Page.GetByRole(AriaRole.Search).ClickAsync();
            var searchInput = Page.GetByRole(AriaRole.Textbox, new() { Name = "Search Input" });
            await searchInput.FillAsync("Garfield");
            await searchInput.PressAsync("Enter");
        });

        await Test.StepAsync("Verify search results", async () =>
        {
            // Verify the accessibility tree of the search results
            await Expect(Page.GetByRole(AriaRole.Main)).ToMatchAriaSnapshotAsync(@"
                - main:
                  - heading ""Garfield"" [level=1]
                  - heading ""search results"" [level=2]
                  - list ""movies"":
                    - listitem ""movie"":
                      - link ""poster of The Garfield Movie The Garfield Movie rating"":
                        - /url: /playwright-movies-app/movie?id=tt5779228&page=1
                        - img ""poster of The Garfield Movie""
                        - heading ""The Garfield Movie"" [level=2]
            ");
        });
    }
}
```
测试执行策略

1. **初始运行**：使用`dotnet test`或在IDE中使用测试运行器执行测试
2. **调试故障**：分析测试故障并找出根本原因
3. 迭代：根据需要细化定位器、断言或测试逻辑
4. **验证**：确保测试始终通过并涵盖预期的功能
5. **报告**：对测试结果和发现的问题提供反馈

质量检查表

在完成测试之前，请确保：

-[]所有定位器都是可访问的和特定的，并避免严格的模式违规
—[]测试逻辑分组，结构清晰
-[]断言是有意义的，反映了用户的期望
—[]测试遵循一致的命名约定
-[]代码格式和注释正确