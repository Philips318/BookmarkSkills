---
name: webapp-testing
description: Toolkit for interacting with and testing local web applications using Playwright. Supports verifying frontend functionality, debugging UI behavior, capturing browser screenshots, and viewing browser logs.
---
# Web应用程序测试

该技能可以使用剧作家自动化对本地web应用程序进行全面的测试和调试。

如果可能的话，您应该使用剧作家MCP服务器来承担这项工作。如果MCP服务器不可用，可以在安装了剧作家的本地Node.js环境中运行代码。

何时使用此技能

在需要时使用此技能：

-在真实的浏览器中测试前端功能
-验证UI行为和交互
-调试web应用程序问题
-捕获屏幕截图用于文档或调试
—检查浏览器控制台日志
-验证表单提交和用户流
-检查跨视口的响应式设计

# #先决条件

—系统已安装的Node.js-本地运行的web应用程序（或可访问的URL）
剧作家将自动安装，如果不存在

##核心能力

# # # 1。浏览器自动化-导航到url
—单击按钮和链接
-填写表单字段
-选择下拉菜单
-处理对话框和警报

# # # 2。验证

-断言元素存在
-验证文本内容
-检查元素可见性
-验证url
-测试响应行为

# # # 3。调试

-截图
-查看控制台日志
-检查网络请求
-调试失败的测试

##用法示例

例1：基本导航测试```javascript
// Navigate to a page and verify title
await page.goto("http://localhost:3000");
const title = await page.title();
console.log("Page title:", title);
```
例2：表单交互```javascript
// Fill out and submit a form
await page.fill("#username", "testuser");
await page.fill("#password", "password123");
await page.click('button[type="submit"]');
await page.waitForURL("**/dashboard");
```
示例3：截图捕获```javascript
// Capture a screenshot for debugging
await page.screenshot({ path: "debug.png", fullPage: true });
```
# #指南

1. **始终验证应用程序正在运行** -在运行测试之前检查本地服务器是否可访问
2. **使用显式等待-等待元素或导航在交互之前完成
3. **捕获屏幕截图失败** -采取屏幕截图，以帮助调试问题
4. **清理资源** -完成后始终关闭浏览器
5. **优雅地处理超时** -为慢速操作设置合理的超时
6. **增量测试** -在复杂的流程之前从简单的交互开始
7. **明智地使用选择器** -首选经过数据测试或基于角色的选择器，而不是CSS类

##常见模式

模式：等待元素```javascript
await page.waitForSelector("#element-id", { state: "visible" });
```
### Pattern：检查元素是否存在```javascript
const exists = (await page.locator("#element-id").count()) > 0;
```
模式：获取控制台日志```javascript
page.on("console", (msg) => console.log("Browser log:", msg.text()));
```
###模式：处理错误```javascript
try {
  await page.click("#button");
} catch (error) {
  await page.screenshot({ path: "error.png" });
  throw error;
}
```
# #的局限性

—需要Node.js环境
-不能测试原生移动应用程序（使用React原生测试库代替）
-复杂的身份验证流程可能存在问题
-一些现代框架可能需要特定的配置

##辅助函数

[`test-helper.js`]（./assets/test-helper.js）中提供了一些辅助函数，用于简化常见任务，如等待元素、捕获屏幕截图和处理错误。您可以在测试中导入和使用这些函数，以提高可读性和可维护性。