---
title: 'Understanding MCP Servers'
description: 'Learn how Model Context Protocol servers extend GitHub Copilot with access to external tools, databases, and APIs.'
authors:
  - GitHub Copilot Learning Hub Team
lastUpdated: 2026-07-06
estimatedReadingTime: '8 minutes'
tags:
  - mcp
  - tools
  - fundamentals
relatedArticles:
  - ./building-custom-agents.md
  - ./what-are-agents-skills-instructions.md
prerequisites:
  - Basic understanding of GitHub Copilot agents
---
GitHub Copilot的内置工具——代码搜索、文件编辑、终端访问——涵盖了广泛的任务。但现实世界的工作流通常需要访问外部系统：数据库、云api、监控仪表板或内部服务。这就是MCP服务器的用武之地。

本文解释了什么是MCP，如何配置服务器，以及代理如何使用它们来完成需要上下文切换的任务。

什么是MCP？

模型上下文协议（MCP）是一个开放的标准，用于将人工智能助手连接到外部数据源和工具。MCP服务器是一个轻量级进程，它公开了Copilot可以在对话期间调用的功能（称为“工具”）。

将MCP服务器视为桥梁：```
GitHub Copilot  ←→  MCP Server  ←→  External System
                     (bridge)        (database, API, etc.)
```
* * * *关键特征:
- MCP是一个开放协议，不是特定于GitHub Copilot-它适用于所有AI工具
-服务器在本地机器上运行或在容器中运行
-每个服务器公开一个或多个具有定义输入和输出的工具
—座席和用户可以在通话过程中自然调用MCP工具

内置与MCP工具GitHub Copilot提供了几个始终可用的内置工具：

|内置工具|它做什么||--------------|--------------|
|`codebase`|跨存储库|搜索和分析代码
|`terminal`|在集成终端|中执行shell命令
|`edit`|创建并修改工作空间|中的文件
|`fetch`|向url |发出HTTP请求
|`search`|搜索工作空间文件|
|`github`|与GitHub api交互|

**MCP工具**通过外部功能扩展了这一点：

| MCP服务器示例|添加功能||-------------------|--------------|
| PostgreSQL服务器|查询数据库，检查模式，分析查询计划|
| Docker服务器|容器管理、日志检查、服务部署|
|守护服务器|获取错误报告，分析崩溃数据|
Figma服务器|读取设计令牌，组件规格|

配置MCP服务器

MCP服务器是按工作空间配置的。GitHub CopilotCLI从几个位置发现服务器定义（按顺序加载）：

|文件|作用域|注释||------|-------|-------|
|`.mcp.json`|存储库根| repo-shared配置首选|
|`.github/mcp.json`|存储库`.github/`文件夹|自动加载工作区配置（v1.0.61+） |
|`.vscode/mcp.json`|VS Code工作区|VS Code兼容工作区配置|
|`devcontainer.json`|开发容器|在容器|内运行时可用

b> **Security**：工作区MCP服务器只有在确认文件夹信任后才会被加载**。如果您没有明确信任某个文件夹，则在其配置文件中定义的服务器将不会启动-保护您免受不受信任存储库中的恶意MCP服务器配置的影响。

示例`.mcp.json`或`.vscode/mcp.json`：```json
{
  "servers": {
    "postgres": {
      "command": "npx",
      "args": ["-y", "@modelcontextprotocol/server-postgres"],
      "env": {
        "DATABASE_URL": "postgresql://user:pass@localhost:5432/mydb"
      }
    },
    "filesystem": {
      "command": "npx",
      "args": ["-y", "@modelcontextprotocol/server-filesystem", "./docs"]
    }
  }
}
```
从注册表安装MCP服务器GitHub CopilotCLI提供了一个基于注册表的安装流程，允许您通过引导配置浏览和安装MCP服务器——不需要手动编辑JSON。在v1.0.64+中，使用`/mcp registry`子命令来浏览可用的服务器：```
/mcp registry
```
选择器将列出注册中心的可用服务器。选择一个后，CLI提示输入任何所需的配置值（连接字符串、API密钥等），并自动将完成的条目写入持久MCP配置。

您也可以直接通过名称安装特定的服务器：```
/mcp install @modelcontextprotocol/server-postgres
```
推荐使用这种引导流程添加新的MCP服务器，特别是对于需要多个配置值的服务器。

###配置字段

**命令**：运行MCP服务器的可执行文件（如`npx`、`python`、`docker`）。

**args**：传递给命令的参数。大多数MCP服务器都是作为npm包分发的，可以使用`npx -y`运行。

**env**：传递给服务器进程的环境变量。将它们用于连接字符串、API密钥和配置——永远不要在JSON文件中硬编码秘密。

**type**（远程服务器）：远程MCP服务器的传输类型（`http`或`sse`）。这个字段现在可以省略了——当没有指定类型时，CLI默认为`http`，简化了远程服务器的配置。**deferTools** *（可选，v1.0.63+）*：当设置为`false`时，即使启用了工具搜索，服务器的工具也始终可用。默认情况下，工具搜索可以隐藏很少使用的MCP工具，以减少上下文噪声；在服务器上设置`deferTools: false`可以防止服务器上的工具被延迟，从而使这些工具永久地保留在工具列表中。

允许MCP服务器指令

默认情况下，Copilot CLI限制了MCP服务器指令注入系统提示符，以避免来自服务器的嘈杂或意外指令，您可能没有完全审查。您可以选择包含来自**所有**连接的带有`--allow-all-mcp-server-instructions`标志*的MCP服务器的指令*(v1.0.66+)*：```bash
copilot --allow-all-mcp-server-instructions
```
仅在您完全信任的服务器上使用此功能，因为它们的指示会影响Copilot在整个会话中的响应。对于大多数项目，默认行为就足够了——只有在特定服务器需要时才启用它（例如，由您控制其指令的内部工具）。

通过服务器rpc管理持久MCP配置

除了基于文件的配置之外，GitHub CopilotCLI还公开了服务器rpc，这些rpc允许MCP服务器和工具脚本在运行时管理持久的MCP服务器注册表。这就支持了程序化的设置——例如，一个注册服务器的安装程序脚本不需要您手工编辑JSON文件。

可用的rpc有：

| RPC |描述||-----|-------------|
|`mcp.config.list`|列出当前注册的所有持久MCP服务器|
|`mcp.config.add`|在持久配置|中添加新的MCP服务器
|`mcp.config.update`|更新已注册服务器|
|`mcp.config.remove`|从持久配置|中移除服务器

这对于需要将其MCP服务器作为install/uninstall流的一部分进行自注册或注销注册的插件和安装程序脚本特别有用，而不需要用户手动编辑配置文件。

常用MCP服务器配置

**PostgreSQL** -查询数据库和检查模式；```json
{
  "postgres": {
    "command": "npx",
    "args": ["-y", "@modelcontextprotocol/server-postgres"],
    "env": {
      "DATABASE_URL": "${input:databaseUrl}"
    }
  }
}
```
**GitHub** -扩展GitHub API访问：```json
{
  "github": {
    "command": "npx",
    "args": ["-y", "@modelcontextprotocol/server-github"],
    "env": {
      "GITHUB_TOKEN": "${input:githubToken}"
    }
  }
}
```
**文件系统** -控制对特定目录的访问：```json
{
  "filesystem": {
    "command": "npx",
    "args": ["-y", "@modelcontextprotocol/server-filesystem", "./data", "./config"]
  }
}
```
b> **安全提示**：敏感值使用`${input:variableName}`。VS Code将在运行时提示这些内容，而不是将它们存储在文件中。

# # #身份验证

某些MCP服务器连接受保护资源时需要进行认证。GitHub CopilotCLI支持几种身份验证方法：—**OAuth**: MCP服务器可以使用OAuth流与外部服务进行认证。CLI自动处理浏览器重定向和令牌存储。这在ACP（代理协调协议）模式下运行时也有效。
**`client_credentials`授权类型**：对于完全无头环境，没有浏览器可用，没有用户交互（如服务器到服务器自动化或CI管道），MCP服务器可以使用OAuth`client_credentials`授权类型进行身份验证。这允许机器对机器身份验证，而无需任何浏览器重定向或设备代码提示。
- **设备代码流(RFC 8628)**：当CLI运行在一个**headless或CI环境**，其中浏览器重定向是不可能的，它会自动回落到设备代码流。您将看到在另一台设备上输入URL和代码以完成身份验证。
- **`/mcp auth`**：如果令牌过期或您需要切换accoUnts，在会话中运行`/mcp auth`。这将为任何启用oauth的MCP服务器打开重新身份验证UI，并支持帐户切换。您可以在不重新启动会话的情况下重新验证。
- **Microsoft Entra ID (Azure AD)**：完全支持通过Microsoft Entra ID进行认证的MCP服务器。完成初始登录后，CLI将缓存身份验证，并且不会在后续连接中显示同意屏幕-每个会话验证一次，而不是每次服务器重新连接。
—**API密钥通过环境变量**：通过MCP服务器配置中的`env`字段传递秘密（参见上面的示例）。永远不要在`.mcp.json`中硬编码凭证。
- **`${input:variableName}`提示**:VS Code将在运行时提示这些值，使提交的文件保密。提示：如果您的MCP服务器使用OAuth动态客户端注册，但将其授权元数据托管在非标准URL（如一些企业服务器，如Atlassian Rovo）， Copilot CLI会自动处理此问题。

##代理如何使用MCP工具

当代理在其`tools`数组中声明MCP服务器时，Copilot可以在对话期间调用该服务器的功能：```yaml
---
name: 'Database Administrator'
description: 'Expert DBA for PostgreSQL performance tuning and schema design'
tools: ['codebase', 'terminal', 'postgres']
---
```
通过这种配置，代理可以：
—执行SQL查询，检查表结构
—分析查询执行计划
—根据实际数据提出索引优化建议
-将模式更改与实时数据库进行比较

###示例对话```
User: The users page is loading slowly. Can you figure out why?

Agent: Let me check the query that powers the users page.
[Searches codebase for user listing query]
[Runs EXPLAIN ANALYZE via postgres MCP server]

I found the issue. The query on user_profiles is doing a sequential scan
on 2.4M rows. Here's what I recommend:

CREATE INDEX idx_user_profiles_active ON user_profiles (is_active)
  WHERE is_active = true;

This should reduce the query time from ~3.2s to ~15ms based on the
current data distribution.
```
如果没有MCP服务器，代理将不得不猜测数据库结构和性能特征。有了它，代理就可以处理真实的数据。

MCP采样（LLM推理请求）

一些先进的MCP服务器可以从Copilot模型请求**LLM推理** - MCP规范中定义为*采样*的能力。这些服务器不仅可以接收来自人工智能的工具调用，还可以要求Copilot生成文本或做出决策，这是它们自己逻辑的一部分。

**工作原理**：
1. MCP服务器向副驾驶发送`sampling/createMessage`请求。
2. 副驾驶向用户显示一个评论提示，解释服务器正在请求什么。
3. 用户批准或拒绝请求。
4. 如果获得批准，Copilot生成响应并将其返回给服务器。这使得像MCP服务器这样的复杂模式能够协调多步骤推理，生成结构化输出或构建更复杂的人工智能管道，同时通过明确的批准步骤保持用户控制。

b> **注**：每次服务器请求推理时，采样都需要显式的用户批准。这是一个安全边界- MCP服务器不能在你不知情的情况下默默地消耗你的AI配额或泄露上下文。

查找MCP服务器

MCP生态系统正在迅速发展。以下是主要资源：

- **[官方MCP服务器](https://github.com/modelcontextprotocol/servers)**：常用服务（PostgreSQL, Slack，谷歌Drive等）的参考实现
- **[MCP规范](https://spec.modelcontextprotocol.io/)**：构建自己的服务器的协议规范
- **[了不起的MCP服务器](https://github.com/punkpeye/awesome-mcp-servers)**：社区策划的MCP服务器列表

建立你自己的MCP服务器如果您的团队拥有内部工具或专有api，则可以构建自定义MCP服务器。该协议支持三种主要的能力类型：

|容量|描述|示例||-----------|-------------|---------|
| **Tools** |功能AI可以调用|`query_database`，`deploy_service`|
| **资源** |数据人工智能可以读取|数据库模式，API文档|
| **提示** |预构建对话模板|常见故障处理流程|

MCP服务器sdk以[Python](https://github.com/modelcontextprotocol/python-sdk), [TypeScript]（https://github.com/modelcontextprotocol/typescript-sdk）和其他语言提供。浏览[Agents Directory]（../../agents/）以获取围绕MCP服务器专业知识构建的代理的示例。

故障排除MCP连接问题

当MCP服务器启动失败或失去连接时，Copilot CLI会显示一个带有可操作细节的警告，以帮助您快速诊断问题。**失败警告包括stderr输出** (v1.0.42+)：如果您的MCP服务器打印错误消息到stderr（例如，缺少环境变量，连接拒绝，导入错误），这些消息现在直接包含在CLI警告中。这意味着您通常无需手动运行服务器就能看到根本原因。

例如，由于没有设置`DATABASE_URL`而无法连接的PostgreSQL服务器将显示：```
⚠ MCP server "postgres" failed to start
  Error: connect ECONNREFUSED 127.0.0.1:5432
  stderr: Error: DATABASE_URL environment variable is required
```
**诊断`/mcp show`的连接问题**：运行`/mcp show`查看所有配置的MCP服务器的当前状态-哪些正在运行，哪些已经失败，以及它们的连接详细信息。当MCP服务器名称中包含空格时，故障警告还建议使用可直接运行的`/mcp show <name>`命令进行快速检查。```
/mcp show              # list all servers and their status
/mcp show postgres     # inspect a specific server
```
**使用`/mcp list`** (v1.0.69+)查看附加服务器：使用`/mcp list`查看当前连接到会话的MCP服务器及其状态。与`/mcp show`（显示所有已配置的服务器）不同，`/mcp list`专注于当前活动的内容，并且可以在代理工作时运行** -用于在不中断代理的情况下检查服务器状态。```
/mcp list              # show servers attached to this session
```
您还可以在代理工作时打开`/mcp`管理器，以便在回合中打开或关闭服务器。添加、编辑、删除和重新验证操作要等到回合结束，但启用或禁用服务器会立即生效。

**切换服务器打开和关闭** (v1.0.66+)：从`/mcp`列表视图，您可以**启用或禁用单个MCP服务器**无需编辑配置文件。在列表中选择一个服务器并切换它——禁用的服务器将不会在以后的会话中启动，并且它们的工具对代理也不可用。这对于暂时禁用导致减速或错误的服务器而不将其从配置中完全删除非常有用。

**常见原因及解决方法**：

| |可能原因|修复||---------|--------------|-----|
|缺少`npx`/`python`/命令|检查可执行文件是否安装在PATH中|
|更新配置中的`env`字段；检查`/mcp auth`|
|服务器启动后退出|服务器崩溃|检查警告中的stderr输出，查找根本原因|
|服务器阻塞|组织策略|联系管理员；切换到已批准的服务器|

最佳实践- **最小权限原则**：只给予MCP服务器所需的最小访问权限。对分析代理使用只读数据库连接。
**：使用`${input:variableName}`为API密钥和连接字符串，或从环境变量加载。
**记录您的服务器**：添加注释或自述文件，解释您的项目使用的MCP服务器及其原因。
- **版本控制小心**：提交`.mcp.json`或`.vscode/mcp.json`用于共享服务器配置，但使用`.gitignore`用于任何包含凭证的文件。
- **测试服务器连通性**：在代理工作流程中依赖MCP服务器之前，验证MCP服务器是否正确启动。使用`/mcp show`检查状态，并在任何失败警告中读取标准错误输出。
- **使用MCP allowlist（实验性）**：在高安全性环境中，`MCP_ALLOWLIST`功能标志允许您根据配置的注册表验证MCP服务器，阻止无法识别的服务器从装载。被allowlist策略阻止的MCP服务器**从`/mcp show`**隐藏，以避免混淆-只有允许的服务器出现在该视图中。对于需要严格控制允许使用哪些MCP服务器的企业环境，这是一个实验性特性。第三方MCP服务器的组织策略

GitHub组织可以强制执行一项政策，限制允许哪些第三方MCP服务器成员使用。当此策略激活时：

- Copilot CLI **强制**组织中的所有用户执行**策略。
-如果配置的MCP服务器被策略阻止，则会显示**警告，因此您可以在期望它们工作之前知道哪些服务器受到限制。

如果看到MCP服务器被阻止的警告，请与组织管理员联系以查明允许列表中有哪些服务器，或切换到已批准的替代服务器。

##常见问题

**问：MCP服务器运行在云中吗？**

答：不，MCP服务器通常作为子进程在您的机器上本地运行。它们在需要时自动启动，并在会话结束时停止。

**Q：我可以使用没有自定义代理的MCP服务器吗？**是的。在`.vscode/mcp.json`中配置后，MCP工具可在任何副驾驶聊天会话中使用。自定义代理简化了为工作流预先选择正确工具的过程。

**问：MCP服务器安全吗？**

答：MCP服务器以与用户帐户相同的权限运行。遵循最小特权原则：使用只读数据库连接，限定API令牌的范围，并在信任服务器代码之前检查它。

**问：我可以配置多少台MCP服务器？**

答：没有硬性限制，但每台服务器都是一个正在运行的进程。只配置您经常使用的服务器。大多数项目使用1-3台服务器。

**问：我正在使用Azure DevOps存储库。GitHub MCP服务器会干扰吗？**答:不是。Copilot CLI自动检测Azure DevOps存储库，并为这些会话禁用内置的GitHub MCP服务器。当你的项目托管在Azure DevOps上时，这可以防止无关的GitHub API调用。其他已配置的MCP服务器不受影响。

##下一步

- **构建代理**:[构建自定义代理](../building-custom-agents/) -创建利用MCP工具的代理
- **探索示例**：浏览[代理目录](../../agents/)，查找围绕MCP服务器集成构建的代理
- **协议深潜**:[MCP规范](https://spec.modelcontextprotocol.io/) -了解协议的细节，以建立自己的服务器

---