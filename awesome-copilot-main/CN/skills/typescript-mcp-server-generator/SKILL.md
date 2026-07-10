---
name: typescript-mcp-server-generator
description: 'Generate a complete MCP server project in TypeScript with tools, resources, and proper configuration'
---
#生成TypeScript MCP Server

用TypeScript创建一个完整的模型上下文协议（Model Context Protocol， MCP）服务器，规范如下：

# #要求

1. **项目结构**：创建一个具有适当目录结构的新TypeScript/Node.js项目
2. **NPM包**：包括@modelcontextprotocol/sdk， zod@3和express（用于HTTP）或stdio支持
3. **TypeScript配置**：正确的tsconfig.json与ES模块支持
4. **服务器类型**：选择HTTP（可流式HTTP传输）或基于视频的服务器
5. **工具**：创建至少一个具有适当模式验证的有用工具
6. **错误处理**：包括全面的错误处理和验证

##实现细节###项目设置
—初始化`npm init`，创建package.json—安装依赖项：`@modelcontextprotocol/sdk`、`zod@3`和特定于传输的软件包
在TypeScript中配置ES模块：package.json中的`"type": "module"`—添加开发依赖项：`tsx`或`ts-node`用于开发
—创建适当的。gitignore文件

###服务器配置
-使用`McpServer`类进行高级实现
—设置服务器名称和版本
-选择合适的传输（StreamableHTTPServerTransport或StdioServerTransport）
-对于HTTP：设置Express与适当的中间件和错误处理
-对于stdio：直接使用StdioServerTransport###工具实现
—使用带有描述性名称的`registerTool()`方法
-使用zod定义模式，用于输入和输出验证
—提供清晰的`title`和`description`字段
—返回结果中的`content`和`structuredContent`-使用try-catch块实现适当的错误处理
—支持异步操作

###Resource/Prompt设置（可选）
-添加资源使用`registerResource()`与ResourceTemplate的动态uri
-使用带有参数模式的`registerPrompt()`添加提示
-考虑为更好的用户体验添加补全支持

代码质量
-使用TypeScript来保证类型安全
—始终遵循async/await模式
-对运输关闭事件进行适当的清理
—使用环境变量进行配置
-为复杂逻辑添加内联注释
-结构代码与清晰的关注点分离要考虑的示例工具类型
-数据处理和转换
-外部API集成
-文件系统操作（读取，搜索，分析）
-数据库查询
-文本分析或总结（抽样）
-系统信息检索

##配置选项
- **对于HTTP服务器**：
—通过环境变量配置端口
-浏览器客户端的CORS设置
-会话管理（无状态vs有状态）
—本地服务器DNS重绑定保护

- **对于工作室服务器**：
-正确处理stdin/stdout—基于环境的配置
-过程生命周期管理

测试指南
-说明如何运行服务器（`npm start`或`npx tsx server.ts`）
—提供MCP Inspector命令：`npx @modelcontextprotocol/inspector`—HTTP服务器：连接URL:`http://localhost:PORT/mcp`-包括示例工具调用
—增加常见问题处理提示需要考虑的附加功能
- llm驱动工具的采样支持
-交互式工作流程的用户输入引出
-动态工具注册与enable/disable能力
-批量更新的通知退出
—资源链接，提供高效的数据引用

生成一个完整的、生产就绪的MCP服务器，并提供全面的文档、类型安全和错误处理。