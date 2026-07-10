---
name: mcp-cli
description: Interface for MCP (Model Context Protocol) servers via CLI. Use when you need to interact with external tools, APIs, or data sources through MCP servers, list available MCP servers/tools, or call MCP tools from command line.
---
# MCP-CLI

通过命令行访问MCP服务器。MCP支持与外部系统（如GitHub、文件系统、数据库和api）进行交互。

# #命令

|命令|输出|| ---------------------------------- | ------------------------------- |
|`mcp-cli`|列出所有服务器和工具名称|
|`mcp-cli <server>`|显示带|参数的工具
|`mcp-cli <server>/<tool>`|获取工具JSON模式|
|`mcp-cli <server>/<tool> '<json>'`|使用参数|调用工具
|`mcp-cli grep "<glob>"`|按名称搜索工具|

**添加`-d`以包含描述**（例如，`mcp-cli filesystem -d`）

# #工作流程

1. **发现**:`mcp-cli`→查看可用的服务器和工具
2. **探索**:`mcp-cli <server>`→查看带参数的工具
3. **Inspect**:`mcp-cli <server>/<tool>`→获取完整的JSON输入模式
4. **执行**:`mcp-cli <server>/<tool> '<json>'`→带参数运行

# #的例子```bash
# List all servers and tool names
mcp-cli

# See all tools with parameters
mcp-cli filesystem

# With descriptions (more verbose)
mcp-cli filesystem -d

# Get JSON schema for specific tool
mcp-cli filesystem/read_file

# Call the tool
mcp-cli filesystem/read_file '{"path": "./README.md"}'

# Search for tools
mcp-cli grep "*file*"

# JSON output for parsing
mcp-cli filesystem/read_file '{"path": "./README.md"}' --json

# Complex JSON with quotes (use heredoc or stdin)
mcp-cli server/tool <<EOF
{"content": "Text with 'quotes' inside"}
EOF

# Or pipe from a file/command
cat args.json | mcp-cli server/tool

# Find all TypeScript files and read the first one
mcp-cli filesystem/search_files '{"path": "src/", "pattern": "*.ts"}' --json | jq -r '.content[0].text' | head -1 | xargs -I {} sh -c 'mcp-cli filesystem/read_file "{\"path\": \"{}\"}"'
```
# #选项

|标志|目的|| ------------ | ------------------------- |
|`-j, --json`|脚本的JSON输出|
|`-r, --raw`|原始文本内容|
|`-d`|包含描述|

##退出码

-`0`：成功
-`1`：客户端错误（坏参数，缺少配置）
-`2`：服务器错误（工具失败）
—`3`：网络错误