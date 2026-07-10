---
description: 'TypeScript patterns for Azure Functions'
applyTo: '**/*.ts, **/*.js, **/*.json'
---
代码生成指南
为Node.js生成现代TypeScript代码
—异步代码使用`async/await`-只要可能，使用Node.jsv22 LTS内置模块，而不是外部包
-总是使用Node.js异步函数，像`node:fs/promises`而不是`fs`，以避免阻塞事件循环
-在向项目添加任何额外的依赖项之前询问
- API是使用Azure Functions使用`@azure/functions@4`包构建的。
—每个端点应该有自己的函数文件，并使用以下命名约定：`src/functions/<resource-name>-<http-verb>.ts`-当对API进行更改时，请确保相应地更新OpenAPI架构（如果存在）和`README.md`文件。