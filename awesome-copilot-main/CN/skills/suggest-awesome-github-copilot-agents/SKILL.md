---
name: suggest-awesome-github-copilot-agents
description: 'Suggest relevant GitHub Copilot Custom Agents files from the awesome-copilot repository based on current repository context and chat history, avoiding duplicates with existing custom agents in this repository, and identifying outdated agents that need updates.'
---
建议使用Awesome GitHub Copilot自定义代理

分析当前存储库上下文，并从[GitHub awesome-copilot存储库]（https://github.com/github/awesome-copilot/blob/main/docs/README.agents.md）中建议相关的自定义代理文件，这些文件在此存储库中尚未可用。自定义代理文件位于awesome-copilot存储库的[agents]（https://github.com/github/awesome-copilot/tree/main/agents）文件夹中。

# #过程1. **获取可用的自定义代理**：从[awesome-copilotREADME.agents.md]（https://github.com/github/awesome-copilot/blob/main/docs/README.agents.md）提取自定义代理列表和描述。必须使用`fetch`工具。
2. **扫描本地自定义代理**：发现现有的自定义代理文件在`.github/agents/`文件夹
3. **提取描述**：从本地自定义代理文件中读取前端内容以获取描述
4. **获取远程版本**：对于每个本地代理，使用原始GitHub url（例如，`https://raw.githubusercontent.com/github/awesome-copilot/main/agents/<filename>`）从awesome-copilot存储库中获取相应的版本
5. **比较版本**：将本地代理内容与远程版本进行比较，以确定：
-最新的代理（完全匹配）
-过时的代理（内容不同）
-过时代理的主要区别（工具，描述，内容）
6. **分析上下文**：查看聊天记录、存储库文件和当前项目需求
7. **匹配相关性**：将可用的自定义代理与已识别的进行比较模式和需求
8. **Present Options**：显示相关的自定义代理及其描述、基本原理和可用性状态，包括过时的代理
9. **验证**：确保建议的代理将增加现有代理尚未涵盖的价值
10. **输出**：提供结构化表的建议，描述，并链接到这两个了不起的副驾驶自定义代理和类似的本地自定义代理    **AWAIT** user request to proceed with installation or updates of specific custom agents. DO NOT INSTALL OR UPDATE UNLESS DIRECTED TO DO SO.
11. **Download/UpdateAssets**：对于请求的代理，自动：    - Download new agents to `.github/agents/` folder
    - Update outdated agents by replacing with latest version from awesome-copilot
    - Do NOT adjust content of the files
    - Use `#fetch` tool to download assets, but may use `curl` using `#runInTerminal` tool to ensure all content is retrieved
    - Use `#todos` tool to track progress
##上下文分析标准

🔍**存储库模式**：

-使用的编程语言（.cs, .js, .py等）
-框架指标(ASP。. NET, React， Azure等)
-项目类型（web应用程序，api，库，工具）
-文档需求（README，规格，adr）

🗨️**聊天记录上下文**：

-最近的讨论和痛点
-特性请求或实现需求
-代码审查模式
-开发工作流程要求

##输出格式

在结构化的表格中显示分析结果，比较出色的copilot自定义代理与现有的存储库自定义代理。|棒极了-副驾驶自定义代理|描述|已经安装|类似的本地自定义代理|建议理由|| ------------------------------------------------------------------------------------------------------------------------------------------------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | ----------------- | ---------------------------------- | ------------------------------------------------------------- |
| [amplitude-experiment-implementation.agent.md](https://github.com/github/awesome-copilot/blob/main/agents/amplitude-experiment-implementation.agent.md) |此自定义代理使用Amplitude的MCP工具在Amplitude内部部署新的实验，实现无缝的变体测试能力和产品特性的推出|❌否|无|将增强产品|内的实验能力
| [launchdarkly-flag-cleanup.agent.md](https://github.com/github/awesome-copilot/blob/main/agents/launchdarkly-flag-cleanup.agent.md) |功能标志清理代理launchdark |✅是|launchdarkly-flag-cleanup.agent.md|已经覆盖了现有的launchdark自定义代理|
| [principal-software-engineer.agent.md](https://github.com/github/awesome-copilot/blob/main/agents/principal-software-engineer.agent.md) |提供负责人级别的软件工程指导，重点关注工程卓越性、技术领导力和务实实施。|⚠️过时|principal-software-engineer.agent.md| Tools配置不同：remoo使用`'web/fetch'`与本地`'fetch'`-更新推荐|本地代理发现进程

1. 列出`.github/agents/`目录下的所有`*.agent.md`文件
2. 对于每个发现的文件，读取前面的内容以提取`description`3. 建立现有代理商的全面库存
4. 使用这个清单来避免重复

版本比较过程

1. 对于每个本地代理文件，构造原始的GitHub URL来获取远程版本：
—模式：`https://raw.githubusercontent.com/github/awesome-copilot/main/agents/<filename>`2. 使用`fetch`工具获取远程版本
3. 比较整个文件内容（包括前端内容、工具数组和主体）
4. 确定具体的差异：
- **前端内容更改**（描述，工具）
- **Tools数组修改**（添加、删除或重命名Tools）
- **内容更新**（说明，示例，指南）
5. 记录过时代理的关键差异
6. 计算相似度以确定是否需要更新

# #要求-使用`githubRepo`工具从awesome-copilot存储库代理文件夹获取内容
—扫描本地文件系统中“`.github/agents/`”目录下存在的代理
—从本地代理文件中读取YAML前端内容，提取描述
—将本地代理与远端版本进行比较，发现过时的代理
—与此存储库中的现有代理进行比较，以避免重复
-关注当前代理库覆盖的差距
-验证建议的代理是否符合存储库的目的和标准
-为每个建议提供清晰的理由
-包括链接到令人敬畏的副驾驶代理和类似的本地代理
-清楚地识别过时的代理，并注明具体差异
-不要在表格和分析之外提供任何额外的信息或背景

##图标参考

-✅已安装和最新
-⚠️已安装但已过时（可更新）
-❌没有安装在回购##更新处理

当发现过时的代理时：
1. 将它们包含在具有⚠️状态的输出表中
2. 在“建议理由”一栏中记录具体的差异
3. 根据所注意到的关键更改提供更新建议
4. 当用户请求更新时，将整个本地文件替换为远程版本
5. 在`.github/agents/`目录中保留文件位置