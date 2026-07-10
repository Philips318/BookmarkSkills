---
description: 'Instructions for building Model Context Protocol (MCP) servers using the TypeScript SDK'
applyTo: '**/*.ts, **/*.js, **/package.json'
---
# TypeScript MCP服务器开发

# #指令—使用**@modelcontextprotocol/sdk** npm包：`npm install @modelcontextprotocol/sdk`—从指定路径导入：`@modelcontextprotocol/sdk/server/mcp.js`、`@modelcontextprotocol/sdk/server/stdio.js`等。
-使用`McpServer`类实现具有自动协议处理的高级服务器
-使用`Server`类进行低级控制和手动请求处理程序
—对input/output模式验证使用** zd **:`npm install zod@3`-始终为工具、资源和提示提供`title`字段，以便更好地显示UI
-使用`registerTool()`、`registerResource()`和`registerPrompt()`方法（推荐使用旧api）
—使用zod:`{ inputSchema: { param: z.string() }, outputSchema: { result: z.string() } }`定义模式
—从工具返回`content`（用于显示）和`structuredContent`（用于结构化数据）
—对于HTTP服务器，请使用`StreamableHTTPServerTransport`与Express或类似框架
—对于本地集成，使用`StdioServerTransport`进行基于演播室的通信
为每个请求创建新的传输实例以防止请求ID冲突（无状态模式）
—对有状态服务器使用`sessionIdGenerator`会话管理
—启用DNS r本地服务器绑定保护：`enableDnsRebindingProtection: true`—配置CORS标头，并为基于浏览器的客户端公开`Mcp-Session-Id`—对于URI参数为`new ResourceTemplate('resource://{param}', { list: undefined })`的动态资源，使用`ResourceTemplate`-支持完成更好的用户体验使用`completable()`包装从`@modelcontextprotocol/sdk/server/completable.js`-使用`server.server.createMessage()`实现采样，以请求客户完成LLM
—使用`server.server.elicitInput()`在工具执行期间请求额外的用户输入
—启用批量更新的通知停用：`debouncedNotificationMethods: ['notifications/tools/list_changed']`-动态更新：调用`.enable()`，`.disable()`,`.update()`，或`.remove()`对注册项发出`listChanged`通知
—使用`@modelcontextprotocol/sdk/shared/metadataUtils.js`中的`getDisplayName()`作为UI显示名称
—使用MCP Inspector测试服务器：`npx @modelcontextprotocol/inspector`最佳实践

保持工具实现专注于单一职责
-提供清晰，描述性的标题和描述，以便LLM的理解
-为所有参数和返回值使用合适的TypeScript类型
-通过try-catch块实现全面的错误处理
—如果出现错误，在工具结果中返回`isError: true`—所有异步操作都使用async/await—关闭数据库连接，正确清理资源
—处理前验证输入参数
-使用结构化日志进行调试，而不会污染stdout/stderr-在公开文件系统或网络访问时考虑安全问题
-在传输关闭事件上执行适当的资源清理
—使用环境变量进行配置（端口、API密钥等）
-清楚地记录工具的功能和限制
—多客户端测试，确保兼容性

##常见模式基本服务器设置（HTTP）```typescript
import { McpServer } from '@modelcontextprotocol/sdk/server/mcp.js';
import { StreamableHTTPServerTransport } from '@modelcontextprotocol/sdk/server/streamableHttp.js';
import express from 'express';

const server = new McpServer({
    name: 'my-server',
    version: '1.0.0'
});

const app = express();
app.use(express.json());

app.post('/mcp', async (req, res) => {
    const transport = new StreamableHTTPServerTransport({
        sessionIdGenerator: undefined,
        enableJsonResponse: true
    });
    
    res.on('close', () => transport.close());
    
    await server.connect(transport);
    await transport.handleRequest(req, res, req.body);
});

app.listen(3000);
```
基本服务器设置（stdio）```typescript
import { McpServer } from '@modelcontextprotocol/sdk/server/mcp.js';
import { StdioServerTransport } from '@modelcontextprotocol/sdk/server/stdio.js';

const server = new McpServer({
    name: 'my-server',
    version: '1.0.0'
});

// ... register tools, resources, prompts ...

const transport = new StdioServerTransport();
await server.connect(transport);
```
简单的工具```typescript
import { z } from 'zod';

server.registerTool(
    'calculate',
    {
        title: 'Calculator',
        description: 'Perform basic calculations',
        inputSchema: { a: z.number(), b: z.number(), op: z.enum(['+', '-', '*', '/']) },
        outputSchema: { result: z.number() }
    },
    async ({ a, b, op }) => {
        const result = op === '+' ? a + b : op === '-' ? a - b : 
                      op === '*' ? a * b : a / b;
        const output = { result };
        return {
            content: [{ type: 'text', text: JSON.stringify(output) }],
            structuredContent: output
        };
    }
);
```
动态资源```typescript
import { ResourceTemplate } from '@modelcontextprotocol/sdk/server/mcp.js';

server.registerResource(
    'user',
    new ResourceTemplate('users://{userId}', { list: undefined }),
    {
        title: 'User Profile',
        description: 'Fetch user profile data'
    },
    async (uri, { userId }) => ({
        contents: [{
            uri: uri.href,
            text: `User ${userId} data here`
        }]
    })
);
```
工具与采样```typescript
server.registerTool(
    'summarize',
    {
        title: 'Text Summarizer',
        description: 'Summarize text using LLM',
        inputSchema: { text: z.string() },
        outputSchema: { summary: z.string() }
    },
    async ({ text }) => {
        const response = await server.server.createMessage({
            messages: [{
                role: 'user',
                content: { type: 'text', text: `Summarize: ${text}` }
            }],
            maxTokens: 500
        });
        
        const summary = response.content.type === 'text' ? 
            response.content.text : 'Unable to summarize';
        const output = { summary };
        return {
            content: [{ type: 'text', text: JSON.stringify(output) }],
            structuredContent: output
        };
    }
);
```
提示完成```typescript
import { completable } from '@modelcontextprotocol/sdk/server/completable.js';

server.registerPrompt(
    'review',
    {
        title: 'Code Review',
        description: 'Review code with specific focus',
        argsSchema: {
            language: completable(z.string(), value => 
                ['typescript', 'python', 'javascript', 'java']
                    .filter(l => l.startsWith(value))
            ),
            code: z.string()
        }
    },
    ({ language, code }) => ({
        messages: [{
            role: 'user',
            content: {
                type: 'text',
                text: `Review this ${language} code:\n\n${code}`
            }
        }]
    })
);
```
错误处理```typescript
server.registerTool(
    'risky-operation',
    {
        title: 'Risky Operation',
        description: 'An operation that might fail',
        inputSchema: { input: z.string() },
        outputSchema: { result: z.string() }
    },
    async ({ input }) => {
        try {
            const result = await performRiskyOperation(input);
            const output = { result };
            return {
                content: [{ type: 'text', text: JSON.stringify(output) }],
                structuredContent: output
            };
        } catch (err: unknown) {
            const error = err as Error;
            return {
                content: [{ type: 'text', text: `Error: ${error.message}` }],
                isError: true
            };
        }
    }
);
```
