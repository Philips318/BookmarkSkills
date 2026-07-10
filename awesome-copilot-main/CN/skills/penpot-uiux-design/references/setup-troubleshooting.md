# Penpot MCP服务器设置和故障排除

完整的指南安装，配置，并排除故障的Penpot MCP服务器。

##架构概述

Penpot MCP集成需要**三个组件**一起工作：```
┌─────────────────┐     ┌─────────────────┐     ┌─────────────────┐
│   MCP Client    │────▶│   MCP Server    │◀───▶│  Penpot Plugin  │
│ (VS Code/Claude)│     │  (port 4401)    │     │ (in browser)    │
└─────────────────┘     └────────┬────────┘     └────────┬────────┘
                                 │                       │
                                 │    WebSocket          │
                                 │    (port 4402)        │
                                 └───────────────────────┘
```
1. **MCP服务器** -将工具暴露给AI客户端（端口4401的HTTP）
2. **插件服务器** -提供Penpot插件文件（端口4400的HTTP）
3. **Penpot MCP插件** -在Penpot浏览器内运行，执行设计命令

# #先决条件

- **Node.jsv22+** -[下载]（https://nodejs.org/）
- **Git** -用于克隆存储库
- **现代浏览器** - Chrome、Firefox或基于Chrome的浏览器

验证Node.js安装：```bash
node --version  # Should be v22.x or higher
npm --version
npx --version
```
# #安装

###步骤1：克隆和安装```bash
# Clone the repository
git clone https://github.com/penpot/penpot-mcp.git
cd penpot-mcp

# Install dependencies
npm install
```
###步骤2：构建和启动服务器```bash
# Build all components and start servers
npm run bootstrap
```
这个命令:

-安装所有组件的依赖项
-构建MCP服务器和插件
启动两个服务器（MCP在4401，插件在4400）

预期输出:* * * *```txt
MCP Server listening on http://localhost:4401
Plugin server listening on http://localhost:4400
WebSocket server listening on port 4402
```
###步骤3：加载插件在Penpot

1. 在浏览器中打开[Penpot]（https://design.penpot.app/）
2. 打开或创建设计文件
3. 进入**插件**菜单（或按插件图标）
4. 点击**从URL**加载插件
5. 输入:`http://localhost:4400/manifest.json`6. 插件UI将出现-点击**“连接到MCP服务器”**
7. 状态应该变为“已连接到MCP服务器”**

> **重要**：在使用MCP工具时保持插件UI打开。关闭它将断开服务器连接。

步骤4：配置您的MCP客户端

####VS CodewithGitHub Copilot添加到你的VS Code`settings.json`：```json
{
  "mcp": {
    "servers": {
      "penpot": {
        "url": "http://localhost:4401/sse"
      }
    }
  }
}
```
或者使用HTTP端点：```json
{
  "mcp": {
    "servers": {
      "penpot": {
        "url": "http://localhost:4401/mcp"
      }
    }
  }
}
```
####克劳德桌面

Claude Desktop需要`mcp-remote`代理（仅限无线传输）：

1. 安装代理：   ```bash
   npm install -g mcp-remote
   ```
2. 编辑Claude Desktop配置：
—**macOS**:`~/Library/Application Support/Claude/claude_desktop_config.json`—**Windows**:`%APPDATA%/Claude/claude_desktop_config.json`—**Linux**:`~/.config/Claude/claude_desktop_config.json`3. 添加Penpot服务器：   ```json
   {
     "mcpServers": {
       "penpot": {
         "command": "npx",
         "args": ["-y", "mcp-remote", "http://localhost:4401/sse", "--allow-http"]
       }
     }
   }
   ```
4. **完全退出**克劳德桌面（文件→退出，不只是关闭窗口）和重新启动

#### Claude Code （CLI）```bash
claude mcp add penpot -t http http://localhost:4401/mcp
```
# #故障排除

###连接问题

#### “插件无法连接到MCP服务器”

**症状**：即使单击连接，插件也显示“未连接”

* * * *解决方案:

1. 验证服务器正在运行：   ```bash
   # Check if ports are in use
   lsof -i :4401  # MCP server
   lsof -i :4402  # WebSocket
   lsof -i :4400  # Plugin server
   ```
2. 重新启动服务器：   ```bash
   # In the penpot-mcp directory
   npm run start:all
   ```
3. 检查浏览器控制台（F12）是否有WebSocket错误

####浏览器阻止本地连接

**症状**：浏览器拒绝从Penpot连接到本地主机

**原因**：铬142+强制私有网络访问（PNA）限制

* * * *解决方案:

1. **Chrome/Chromium**：当提示时，允许访问本地网络
2. **Brave**：为Penpot网站禁用Shield：
-点击地址栏中的勇敢之盾图标
-为这个网站关闭屏蔽
3. **试试Firefox**: Firefox并没有严格执行这些限制

#### “WebSocket连接失败”

* * * *解决方案:

1. 检查防火墙设置-允许端口4400,4401,4402
2. 如果VPN处于激活状态，请关闭VPN
3. 检查使用相同端口的冲突应用程序

MCP客户端问题

####VS Code/Claude中没有出现的工具

1. * *验证端点* *:   ```bash
   # Test the SSE endpoint
   curl http://localhost:4401/sse
   
   # Test the MCP endpoint
   curl http://localhost:4401/mcp
   ```
2. **检查配置语法** - JSON必须有效
3. **完全重启MCP客户端**
4. **检查MCP服务器日志**：   ```bash
   # Logs are in mcp-server/logs/
   tail -f mcp-server/logs/mcp-server.log
   ```
#### “工具执行超时”

**原因**：插件断开连接或操作时间过长

* * * *解决方案:

1. 确保插件UI在Penpot中仍然打开
2. 验证插件显示“已连接”状态
3. 尝试重新连接：单击“断开连接”，然后在插件中连接

插件问题

#### “插件加载失败”

1. 验证插件服务器是否在端口4400上运行
2. 尝试在浏览器中直接访问`http://localhost:4400/manifest.json`3. 清除浏览器缓存并重新加载Penpot
4. 删除并重新添加插件

#### “找不到笔筒对象”

**原因**：插件未正确初始化或设计文件未打开

* * * *解决方案:

1. 确保打开了一个设计文件（而不仅仅是仪表板）。
2. 打开文件后等待几秒钟再连接
3. 刷新Penpot并重新加载插件

###服务器问题

####端口已被使用```bash
# Find process using the port
lsof -i :4401

# Kill the process if needed
kill -9 <PID>
```
或者通过环境变量配置不同的端口：```bash
PENPOT_MCP_SERVER_PORT=4501 npm run start:all
```
####服务器启动时崩溃

1. 检查Node.js版本（必须是v22+）
2. 删除`node_modules`并重新安装：   ```bash
   rm -rf node_modules
   npm install
   npm run bootstrap
   ```
##配置参考

环境变量

|变量|默认值|描述||----------|---------|-------------|
|`PENPOT_MCP_SERVER_PORT`| 4401 |HTTP/SSE服务器端口|
|`PENPOT_MCP_WEBSOCKET_PORT`| 4402 | WebSocket服务器端口|
|`PENPOT_MCP_SERVER_LISTEN_ADDRESS`| localhost |服务器绑定地址|
|`PENPOT_MCP_LOG_LEVEL`| info |日志级别（trace/debug/info/warn/error） |
|`PENPOT_MCP_LOG_DIR`| logs |日志文件目录|
|`PENPOT_MCP_REMOTE_MODE`| false |启用远程模式（禁用文件系统访问）|

示例：自定义配置```bash
# Run on different ports with debug logging
PENPOT_MCP_SERVER_PORT=5000 \
PENPOT_MCP_WEBSOCKET_PORT=5001 \
PENPOT_MCP_LOG_LEVEL=debug \
npm run start:all
```
##验证安装

运行以下检查表确认一切正常：

1. * *服务器运行* *:   ```bash
   curl -s http://localhost:4401/sse | head -1
   # Should return SSE stream headers
   ```
2. **插件连接**：插件界面显示“连接到MCP服务器”

3. **可用工具**：在您的MCP客户端中，验证这些工具是否出现：
——`mcp__penpot__execute_code`——`mcp__penpot__export_shape`——`mcp__penpot__import_image`——`mcp__penpot__penpot_api_info`4. **测试执行**：让你的人工智能助手运行一个简单的命令：
> “使用笔筒获取当前页面名称”

##寻求帮助

- **GitHub问题**:[penpot/penpot-mcp/issues]（https://github.com/penpot/penpot-mcp/issues）
- **GitHub讨论**:[penpot/penpot-mcp/discussions]（https://github.com/penpot/penpot-mcp/discussions）
- **笔盆社区**:[Community . Penpot .app]（https://community.penpot.app/）