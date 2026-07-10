---
name: playwright-generate-test
description: 'Generate a Playwright test based on a scenario using Playwright MCP'
---
#使用剧作家MCP测试生成

您的目标是在完成所有规定的步骤后，根据所提供的场景生成一个剧作家测试。

##具体说明

给你一个剧本，你需要为它生成一个剧作家测试。如果用户没有提供场景，您将要求他们提供一个。
不要过早地生成测试代码，或者仅仅基于场景而没有完成所有规定的步骤。
—使用剧作家MCP提供的工具，逐条执行步骤。
-只有在所有步骤完成后，才会发出一个剧作家TypeScript测试，根据消息历史使用`@playwright/test`—将生成的测试文件保存在tests目录下
-执行测试文件并迭代直到测试通过