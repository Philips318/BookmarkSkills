---
title: 'Installing and Using Plugins'
description: 'Learn how to find, install, and manage plugins that extend GitHub Copilot CLI with reusable agents, skills, hooks, and integrations.'
authors:
  - GitHub Copilot Learning Hub Team
lastUpdated: 2026-06-24
estimatedReadingTime: '8 minutes'
tags:
  - plugins
  - copilot-cli
  - fundamentals
relatedArticles:
  - ./building-custom-agents.md
  - ./creating-effective-skills.md
  - ./automating-with-hooks.md
prerequisites:
  - GitHub Copilot CLI installed
  - Basic understanding of agents, skills, and hooks
---
插件是可安装的包，它使用可重用的代理、技能、钩子和服务器扩展GitHub CopilotCLI，所有这些都捆绑在一个单元中，您可以使用一个命令安装。与手动复制代理文件和在每个项目中配置MCP服务器不同，插件允许您安装一组精心设计的功能并与团队共享。

本文解释了插件包含什么，如何查找和安装它们，以及如何管理插件库。

插件里面有什么？

一个插件包含以下一个或多个组件：

|组件|功能|文件位置||-----------|-------------|---------------|
| **海关代理** |专门的人工智能助手，拥有量身定制的专业知识|`agents/*.agent.md`|
| **技能** |离散可调用能力与捆绑资源|`skills/*/SKILL.md`|
| **钩子** |事件处理程序拦截代理行为|`hooks.json`或`hooks/`|
| **MCP服务器** |模型上下文外部工具协议集成|`.mcp.json`或`.github/mcp.json`|
| **LSP服务器** |语言服务器协议集成|`lsp.json`或`.github/lsp.json`|
|`extensions/`| **Extensions** |可通过插件市场安装的IDE扩展(v1.0.62+

一个插件可以包括所有这些，也可以只包括一个——例如，一个插件可以提供一个专门的代理，或者一个包含多个代理、技能、钩子和MCP服务器配置的完整开发工具包。

示例：插件是什么样子的

下面是一个典型插件的结构：```
my-plugin/
├── .github/
│   └── plugin/
│       └── plugin.json        # Plugin manifest (name, description, version)
├── agents/
│   ├── api-architect.agent.md
│   └── test-specialist.agent.md
├── skills/
│   └── database-migrations/
│       ├── SKILL.md
│       └── scripts/migrate.sh
├── hooks.json
└── README.md
```
`plugin.json`清单声明插件包含的内容：```json
{
  "name": "my-plugin",
  "description": "API development toolkit with specialized agents and migration skills",
  "version": "1.0.0",
  "agents": [
    "./agents/api-architect.md",
    "./agents/test-specialist.md"
  ],
  "skills": [
    "./skills/database-migrations/"
  ]
}
```
为什么要使用插件？

您可能会想：为什么不直接将代理文件手动复制到项目中呢？插件提供了几个优势：

|特性|手动配置|插件||---------|---------------------|--------|
| **范围** |单个存储库|任意项目|
| **共享** |手动copy/paste|`copilot plugin install`命令|
| **版本管理** | Git历史记录|市场版本|
b| **发现** |搜索知识库|市场浏览|
| **更新** |手动跟踪|`copilot plugin update`|

当你想要：

- **在整个团队中标准化** -每个人都安装相同的插件以获得一致的工具
- **共享领域专业知识** -将Rails专家，Kubernetes专家或安全审稿人打包为可安装单元
- **封装复杂的设置** -捆绑MCP服务器配置，否则需要手动设置
- **跨项目重用** -在每个项目中安装相同的功能，而无需复制文件

##查找插件插件收集在**市场** -注册表，你可以浏览和安装。Copilot CLI和VS Code都有两个默认注册的市场- **无需设置**：

- **`copilot-plugins`** -官方GitHub Copilot插件
- **`awesome-copilot`** -来自这个存储库的社区贡献插件

在Copilot CLI中浏览

列出您的注册市场：```bash
copilot plugin marketplace list
```
浏览特定市场中的插件：```bash
copilot plugin marketplace browse awesome-copilot
```
或者从交互式副驾驶会话中：```
/plugin marketplace browse awesome-copilot
```
提示：您也可以在此站点的[plugins Directory]（../../plugins/）上浏览插件，以在安装前查看描述、包含的代理和技能。

在VS Code中浏览

因为`awesome-copilot`是VS Code的默认市场，你可以在没有任何配置的情况下发现插件：

-打开**Extensions**搜索视图，键入**`@agentPlugins`**查看所有可用的插件
-或打开**命令面板** (`Ctrl+Shift+P`/`Cmd+Shift+P`)，并运行**聊天：插件**

增加更多的市场

从GitHub存储库注册其他市场：```bash
copilot plugin marketplace add anthropics/claude-code
```
或者从本地路径：```bash
copilot plugin marketplace add /path/to/local-marketplace
```
在团队中分享市场注册信息

要为在存储库中工作的每个人自动注册一个额外的市场，请在`.github/copilot-settings.json`（或`config.json`）中添加一个`extraKnownMarketplaces`条目：```json
{
  "extraKnownMarketplaces": [
    {
      "name": "my-org-plugins",
      "source": "my-org/internal-plugins"
    }
  ]
}
```
这样，团队成员就可以自动获得可用的`my-org-plugins`市场，而无需运行单独的`marketplace add`命令。这将取代旧的`marketplaces`设置，该设置在v1.0.16中被删除。

##安装插件

###从副驾驶CLI

通过名称和市场引用插件：```bash
copilot plugin install database-data-management@awesome-copilot
```
或者从交互式会话中：```
/plugin install database-data-management@awesome-copilot
```
> **弃用通知**：直接从GitHub存储库URL、原始URL或本地文件路径（例如`copilot plugin install github/awesome-copilot`）安装插件已弃用，并将在未来的版本中删除。使用基于市场的安装。

###来自VS Code通过扩展搜索视图中的`@agentPlugins`或通过命令调色板中的**Chat: Plugins**浏览到插件，然后单击**安装**。

##管理插件

一旦安装，插件管理与几个简单的命令：```bash
# List all installed plugins
copilot plugin list

# Update a plugin to the latest version
copilot plugin update my-plugin

# Refresh all marketplace catalogs (fetch the latest list of available plugins)
copilot plugin marketplace update

# Remove a plugin
copilot plugin uninstall my-plugin
```
###从本地目录加载插件

您可以直接从本地目录加载插件，而无需从市场安装它们，在启动Copilot时使用`--plugin-dir`标志：```bash
copilot --plugin-dir /path/to/my-plugin
```
以这种方式加载的插件出现在`/plugin list`中单独的**External Plugins**部分下，与市场安装的插件明显不同。这对于测试开发中的本地插件或加载未发布到任何市场的私有插件非常有用。

插件存储在哪里

- **市场插件**:`~/.copilot/installed-plugins/MARKETPLACE/PLUGIN-NAME/`**直接安装**:`~/.copilot/installed-plugins/_direct/PLUGIN-NAME/`插件在运行时是如何工作的

当你安装一个插件时，它的组件自动变为Copilot CLI可用：

**代理**出现在你的代理选择（使用`/agent`或代理下拉菜单）
- **技能**自动加载时，相关的当前任务
- **钩子**运行在配置的生命周期事件期间代理会话
- **MCP服务器**扩展代理可用的工具你不需要在安装后做任何额外的配置——插件的组件无缝地集成到你的工作流程中。插件安装后立即生效，不需要重新启动Copilot CLI。

##插件从这个存储库

这个存储库（`awesome-copilot`）既是单个资源的集合，也是插件市场。你可以用两种方式使用它：

###安装单独的插件

浏览[插件目录]（../../plugins/）并安装特定的插件：```bash
copilot plugin install context-engineering@awesome-copilot
copilot plugin install azure-cloud-development@awesome-copilot
copilot plugin install frontend-web-dev@awesome-copilot
```
每个插件都围绕一个特定的主题或技术捆绑了相关的代理和技能。

###使用单独的资源没有插件

如果你只需要一个代理或技能（而不是一个完整的插件），你仍然可以从这个repo直接复制单个文件到你的项目中：

—拷贝文件“`.agent.md`”到“`.github/agents/`”
-复制技能文件夹到`.github/skills/`复制一个钩子配置到`.github/hooks/`请参阅[使用副驾驶编码代理]（../using-copilot-coding-agent/）了解此方法的详细信息。

最佳实践- **开始与市场插件**之前建立自己的-可能已经有一个适合您的需求
- **保持插件的重点** -一个“Rails开发”的插件比一个“一切”的插件要好
- **定期检查更新** -运行`copilot plugin update`获得最新的改进
**检查你安装了什么** -插件在你的机器上运行代码，所以在安装之前检查不熟悉的插件
-发布一个内部插件，以确保每个团队成员都有相同的代理，技能和钩子
- **删除未使用的插件** -整理与`copilot plugin uninstall`，以保持您的环境清洁

##常见问题

**问：插件工作与编码代理GitHub.com答：插件特定于GitHub CopilotCLI和VS Code扩展（目前为Insiders）。对于GitHub.com上的编码代理，直接将代理，技能和钩子添加到您的存储库（如果您喜欢的话，可以通过插件！）。详见[使用副驾驶编码代理]（../using-copilot-coding-agent/）。

**问：我可以同时使用插件和存储库级别的配置吗？**

是的。插件组件与存储库的本地代理、技能和钩子合并。如果存在冲突，则优先考虑本地配置。

**问：如何创建自己的插件？**

答：用`plugin.json`清单和你的agents/skills/hooks.创建一个目录，参见[创建插件的GitHub文档]（https://docs.github.com/en/copilot/how-tos/copilot-cli/customize-copilot/plugins-creating）的一步一步的指导。

**问：我可以在我的组织内共享插件吗？**

是的。你可以在GitHub内部仓库中创建一个私有插件市场，然后让团队成员用`copilot plugin marketplace add org/internal-plugins`注册它。**问：如果我卸载插件会发生什么？**

答：插件的代理，技能和钩子从Copilot中删除，并且存储在磁盘上的任何缓存插件数据也被清理。任何已经使用这些工具完成的工作都不会受到影响——只有未来的会话才会失去访问权限。

##下一步

- **浏览插件**：探索[插件目录]（../../plugins/）可安装的插件包
- **创造技能**:[创造有效的技能](../creating-effective-skills/) -建立技能，可以包括在插件
- **构建代理**:[构建自定义代理](../building-custom-agents/) -创建代理包在插件
- **添加钩子**:[automated with Hooks](../automating-with-hooks/) -为插件自动化配置钩子

---