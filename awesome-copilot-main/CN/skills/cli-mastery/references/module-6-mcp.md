#模块6:MCP集成

什么是MCP？

-模型上下文协议-连接AI与外部工具的标准
-把它想象成“AI的USB端口”-插入任何兼容的工具
- GitHub MCP服务器是**内置**（搜索repos，问题，pr，行动）

##关键命令

|命令|它的作用||---------|-------------|
|`/mcp`| MCP服务器连接列表|
|`/mcp add <name> <command>`|新增MCP服务器|

##流行的MCP服务器

—`@modelcontextprotocol/server-postgres`—查询PostgreSQL数据库
-`@modelcontextprotocol/server-sqlite`-查询SQLite数据库
—`@modelcontextprotocol/server-filesystem`—使用权限访问本地文件
-`@modelcontextprotocol/server-memory`-持久知识图谱
-`@modelcontextprotocol/server-puppeteer`-浏览器自动化

# #配置

|级别|文件||-------|------|
|用户|`~/.copilot/mcp-config.json`|
|项目|`.github/mcp-config.json`|

##配置文件格式```json
{
  "mcpServers": {
    "my-server": {
      "command": "npx",
      "args": ["@modelcontextprotocol/server-postgres", "{{env.DATABASE_URL}}"],
      "env": { "NODE_ENV": "development" }
    }
  }
}
```
安全最佳实践

-永远不要把凭证直接放在配置文件中
—使用环境变量引用：`{{env.SECRET}}`-使用前检查MCP服务器源代码
-只连接实际需要的服务器