---
name: python-mcp-server-generator
description: 'Generate a complete MCP server project in Python with tools, resources, and proper configuration'
---
#生成Python MCP服务器

用Python创建一个完整的模型上下文协议（MCP）服务器，规格如下：

# #要求

1. **项目结构**：使用uv创建一个具有适当结构的新Python项目
2. **依赖**：包含mcp[cli]包与uv
3. **传输类型**：选择stdio（用于本地）或streamable-http（用于远程）
4. **工具**：创建至少一个有适当类型提示的有用工具
5. **错误处理**：包括全面的错误处理和验证

##实现细节

###项目设置
—初始化为`uv init project-name`—添加MCP SDK:`uv add "mcp[cli]"`-创建主服务器文件（例如：`server.py`）
-为Python项目添加`.gitignore`—配置为直接执行`if __name__ == "__main__"`###服务器配置
-使用`mcp.server.fastmcp`中的`FastMCP`类
—设置服务器名称和可选指令
-选择传输：stdio（默认）或streamable-http
—HTTP：可选配置主机、端口和无状态模式

###工具实现
-在函数上使用`@mcp.tool()`装饰器
-总是包含类型提示-它们自动生成模式
-编写清晰的文档字符串-它们成为工具描述
-使用Pydantic模型或TypedDicts进行结构化输出
—支持I/O-bound任务的异步操作
—包括适当的错误处理

###Resource/Prompt设置（可选）
-添加资源与`@mcp.resource()`装饰器
—动态资源使用URI模板：`"resource://{param}"`-添加提示与`@mcp.prompt()`装饰器
—根据提示返回字符串或消息列表代码质量
-对所有函数参数和返回值使用类型提示
—为工具、资源和提示符编写文档字符串
-遵循PEP 8风格指南
—异步操作使用async/await-实现用于资源清理的上下文管理器
-为复杂逻辑添加内联注释

要考虑的示例工具类型
-数据处理和转换
-文件系统操作（读取，分析，搜索）
-外部API集成
-数据库查询
-文本分析或生成（带采样）
-系统信息检索
-数学或科学计算##配置选项
- **对于工作室服务器**：
-简单直接执行
-用`uv run mcp dev server.py`测试
-安装到Claude:`uv run mcp install server.py`- **对于HTTP服务器**：
—通过环境变量配置端口
—无状态扩展模式：`stateless_http=True`—JSON响应方式：`json_response=True`—浏览器客户端的CORS配置
-挂载到现有ASGI服务器（Starlette/FastAPI）

测试指南
—说明如何运行服务器：
- studio:`python server.py`或`uv run server.py`- HTTP:`python server.py`，然后连接到`http://localhost:PORT/mcp`-使用MCP检查器进行测试：`uv run mcp dev server.py`—安装到Claude Desktop:`uv run mcp install server.py`-包括示例工具调用
-添加故障排除提示需要考虑的附加功能
—日志、进度和通知的上下文使用情况
-人工智能工具的LLM采样
-交互式工作流程的用户输入引出
-共享资源（数据库，连接）的生命周期管理
- Pydantic模型的结构化输出
- UI显示图标
-图像处理与图像类
-完成支持更好的用户体验

最佳实践
-在任何地方使用类型提示-它们不是可选的
—尽可能返回结构化数据
-记录标准错误（或使用上下文日志），以避免标准输出污染
—合理清理资源
-尽早验证输入
—提供清晰的错误提示
-在LLM集成前独立测试工具

生成一个完整的、生产就绪的MCP服务器，具有类型安全、正确的错误处理和全面的文档。