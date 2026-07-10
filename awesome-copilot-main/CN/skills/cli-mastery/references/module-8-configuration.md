模块8：配置

##密钥文件

|文件|用途||------|---------|
|`~/.copilot/config.json`|主要设置（模型，主题，日志，实验标志）|
|`~/.copilot/mcp-config.json`| MCP服务器|
|`~/.copilot/lsp-config.json`|语言服务器(用户级
|`.github/lsp.json`|语言服务器（repo级）|
|`~/.copilot/copilot-instructions.md`|全局自定义指令|
|回购级自定义指令|

##环境变量

|变量|用途||----------|---------|
|`EDITOR`|`Ctrl+G`的文本编辑器（外部编辑器中的编辑提示）|
|`COPILOT_LOG_LEVEL`|日志详细信息（error/warn/info/debug/trace） |
|`GH_TOKEN`/`GITHUB_TOKEN`| GitHub认证令牌（按顺序检查）|
|`COPILOT_CUSTOM_INSTRUCTIONS_DIRS`|自定义指令|的附加目录

##权限模型

—默认：编辑、创建、shell命令需要确认
—`/allow-all`或`--yolo`：跳过对会话的所有确认
—`/reset-allowed-tools`：重新启用确认
-目录允许列表，工具批准门，MCP服务器信任

##日志级别

错误，警告，信息，调试，跟踪（`COPILOT_LOG_LEVEL=debug copilot`）

使用debug/trace处理：MCP连接问题、工具故障、意外行为、bug报告