---
applyTo: '*'
description: 'Quarkus and MCP Server with HTTP SSE transport development standards and instructions'
---
#夸克MCP服务器

使用Java 21、Quarkus和HTTP SSE传输构建MCP服务器。

# #栈

- Java 21与Quarkus框架
—MCP服务器扩展名：`mcp-server-sse`- CDI用于依赖注入
—MCP Endpoint:`http://localhost:8080/mcp/sse`##快速入门```bash
quarkus create app --no-code -x rest-client-jackson,qute,mcp-server-sse your-domain-mcp-server
```
# #结构

-使用标准的Java命名约定（PascalCase类，camelCase方法）
-组织在包：`model`，`repository`,`service`,`mcp`-使用记录类型的不可变数据模型
—不可变数据的状态管理必须由存储库层管理
—为公共方法添加Javadoc

## MCP工具

—必须是`@ApplicationScoped`CDI bean中的公共方法
—使用`@Tool(name="tool_name", description="clear description")`-永远不要返回`null`-而是返回错误消息
-始终验证参数并优雅地处理错误

# #架构

—关注点分离：MCP工具→服务层→存储库
-使用`@Inject`进行依赖注入
—使数据操作线程安全
—使用“`Optional<T>`”，避免空指针异常

##常见问题-不要把业务逻辑放在MCP工具中（使用服务层）
-不要从工具抛出异常（返回错误字符串）
-不要忘记验证输入参数
-测试边缘情况（null，空输入）