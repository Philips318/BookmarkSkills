# FlowStudio Power自动化插件

给你的人工智能代理同样的可见性，你有在电力自动化门户。Graph API只返回顶层运行状态——代理看不到动作输入、循环迭代、嵌套故障或谁拥有流。Flow Studio MCP公开了这一切。

该插件包括涵盖整个生命周期的五项技能：连接、调试、构建、监控和管理Power automation云流。

需要[FlowStudio MCP]（https://mcp.flowstudio.app）订阅。

特工们今天看不到的东西

|您在门户中看到的|代理通过Graph API看到的|| ----------------------------------------- | -------------------------------- |
|动作输入输出|运行通过或失败（无详细信息）|
|循环迭代数据|无|
|子流失败|顶级错误码仅|
|流量运行状况和故障率|无|
|谁建立了一个流，它使用什么连接器|没有|

Flow Studio MCP填补了这些空白。

# #安装```bash
copilot plugin install flowstudio-power-automate@awesome-copilot
```
包含的内容

# # #技能

|技能|描述|| -------------------------------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------- |
基础技能-授权设置，可重用的MCP助手（Python +Node.js），通过`list_skills`/`tool_search`发现工具，超大响应处理。先加载。|
分步诊断工作流程-操作级输入和输出，而不仅仅是错误代码。识别跨嵌套子流和循环迭代的根本原因。|
从头开始构建和部署流定义——加载`create-flow`、发现连接器操作、解析动态options/properties、连接模板、部署，并通过重新提交进行测试。|
|`flowstudio-power-automate-monitoring`|缓存存储的流量运行状况—故故率、带有修复提示的运行历史、制造商库存、电源应用程序、环境和连接计数。|
|`flowstudio-power-automate-governance`|治理工作流——根据业务影响对流进行分类、检测孤立资源、审计连接器、管理通知规则、计算存档分数。|前三个技能调用实时的Power automation API。监视和治理技能从具有聚合统计信息和治理元数据的缓存每日快照中读取数据。

# #先决条件

- [FlowStudio MCP]（https://mcp.flowstudio.app）订阅
—MCP端点：`https://mcp.flowstudio.app/mcp`- API密钥（作为`x-api-key`头传递-而不是承载）

##开始

1. 安装插件
2. 在[mcp.flowstudio.app]（https://mcp.flowstudio.app）获取API密钥
3. 配置VS Code（`.vscode/mcp.json`）中的MCP连接：   ```json
   {
     "servers": {
       "flowstudio": {
         "type": "http",
         "url": "https://mcp.flowstudio.app/mcp",
         "headers": { "x-api-key": "<YOUR_TOKEN>" }
       }
     }
   }
   ```
4. 要求Copilot列出您的流、调试故障、构建新流、检查流运行状况或运行治理审查

# #源

这个插件是[Awesome Copilot]（https://github.com/github/awesome-copilot）的一部分，这是一个社区驱动的GitHub Copilot扩展集合。

技能来源：[ninihen1/power-automate-mcp-skills]（https://github.com/ninihen1/power-automate-mcp-skills）

# #许可证

麻省理工学院