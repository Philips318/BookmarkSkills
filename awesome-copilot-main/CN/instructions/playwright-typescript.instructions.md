---
description: 'Playwright test generation instructions'
applyTo: '**'
---
##测试编写指南

代码质量标准
**定位器**：优先考虑面向用户，基于角色的定位器（`getByRole`,`getByLabel`，`getByText`等）的弹性和可访问性。使用`test.step()`对交互进行分组，并改进测试的可读性和报告。
**断言**：使用自动重试web优先断言。这些断言以`await`关键字开始（例如，`await expect(locator).toHaveText()`）。避免使用`expect(locator).toBeVisible()`，除非专门测试可见性变化。
- **超时：依赖于剧作家的内置自动等待机制。避免硬编码等待或增加默认超时。
- **清晰度**：使用描述性测试和步骤标题，清楚地说明意图。只在解释复杂的逻辑或不明显的交互时添加注释。测试结构
—**导入**：从`import { test, expect } from '@playwright/test';`开始。
- **组织**：将某个特性的相关测试分组在`test.describe()`块下。
**Hooks**：使用`beforeEach`进行`describe`块中所有测试通用的设置操作（例如，导航到页面）。
—**标题**：遵循明确的命名约定，如`Feature - Specific action or scenario`。


###文件组织
—**存放位置**：存放所有测试文件在`tests/`目录下。
- **命名**：使用约定`<feature-or-page>.spec.ts`（例如，`login.spec.ts`,`search.spec.ts`）。
- **范围**：针对每个主要应用程序功能或页面一个测试文件。断言最佳实践
- **UI结构**：使用`toMatchAriaSnapshot`来验证组件的可访问性树结构。这提供了一个全面的和可访问的快照。
- **元素计数**：使用`toHaveCount`断言定位器找到的元素数量。
- **文本内容**:`toHaveText`用于精确文本匹配，`toContainText`用于部分匹配。
—**导航**：操作后使用`toHaveURL`验证页面URL。


示例测试结构```typescript
import { test, expect } from '@playwright/test';

test.describe('Movie Search Feature', () => {
  test.beforeEach(async ({ page }) => {
    // Navigate to the application before each test
    await page.goto('https://debs-obrien.github.io/playwright-movies-app');
  });

  test('Search for a movie by title', async ({ page }) => {
    await test.step('Activate and perform search', async () => {
      await page.getByRole('search').click();
      const searchInput = page.getByRole('textbox', { name: 'Search Input' });
      await searchInput.fill('Garfield');
      await searchInput.press('Enter');
    });

    await test.step('Verify search results', async () => {
      // Verify the accessibility tree of the search results
      await expect(page.getByRole('main')).toMatchAriaSnapshot(`
        - main:
          - heading "Garfield" [level=1]
          - heading "search results" [level=2]
          - list "movies":
            - listitem "movie":
              - link "poster of The Garfield Movie The Garfield Movie rating":
                - /url: /playwright-movies-app/movie?id=tt5779228&page=1
                - img "poster of The Garfield Movie"
                - heading "The Garfield Movie" [level=2]
      `);
    });
  });
});
```
测试执行策略

1. **初始运行**：使用`npx playwright test --project=chromium`执行测试
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