---
name: suggest-awesome-github-copilot-skills
description: 'Suggest relevant GitHub Copilot skills from the awesome-copilot repository based on current repository context and chat history, avoiding duplicates with existing skills in this repository, and identifying outdated skills that need updates.'
---
建议Awesome GitHub CopilotSkills

分析当前存储库上下文，并从[GitHub awesome-copilot存储库]（https://github.com/github/awesome-copilot/blob/main/docs/README.skills.md）中建议相关的代理技能，这些技能在此存储库中尚未可用。Agent Skills是位于awesome-copilot存储库的[Skills]（https://github.com/github/awesome-copilot/tree/main/skills）文件夹中的自包含文件夹，每个文件夹包含一个带有说明和可选捆绑资产的`SKILL.md`文件。

# #过程1. **获取可用技能**：从[awesome-copilotREADME.skills.md]（https://github.com/github/awesome-copilot/blob/main/docs/README.skills.md）中提取技能列表和描述。必须使用`#fetch`工具。
2. **扫描本地技能**：发现现有的技能文件夹在`.github/skills/`文件夹
3. **提取描述**：从本地`SKILL.md`文件中读取前端内容，得到`name`和`description`4. **获取远程版本**：对于每个本地技能，使用原始GitHub url从awesome-copilot存储库中获取相应的`SKILL.md`（例如，`https://raw.githubusercontent.com/github/awesome-copilot/main/skills/<skill-name>/SKILL.md`）
5. **比较版本**：将本地技能内容与远程版本进行比较，以确定：
-最新的技能（完全匹配）
-过时的技能（内容不同）
-过时技能的主要差异（描述、说明、捆绑资产）
6. **分析上下文**：查看聊天记录、存储库文件和当前项目需求
7. **比较现有**：检查此存储库中已有的技能8. **匹配相关性**：将可用技能与确定的模式和需求进行比较
9. **当前选项**：显示相关技能的描述、基本原理和可用性状态，包括过时的技能
10. **验证**：确保建议的技能将增加现有技能尚未涵盖的价值
11. **输出**：提供结构化的表格，建议，描述和链接，既了不起的副驾驶技能和类似的本地技能    **AWAIT** user request to proceed with installation or updates of specific skills. DO NOT INSTALL OR UPDATE UNLESS DIRECTED TO DO SO.
12. **Download/Update资产**：对于请求的技能，自动：    - Download new skills to `.github/skills/` folder, preserving the folder structure
    - Update outdated skills by replacing with latest version from awesome-copilot
    - Download both `SKILL.md` and any bundled assets (scripts, templates, data files)
    - Do NOT adjust content of the files
    - Use `#fetch` tool to download assets, but may use `curl` using `#runInTerminal` tool to ensure all content is retrieved
    - Use `#todos` tool to track progress
##上下文分析标准

🔍**存储库模式**：
-使用的编程语言（.cs, .js, .py, .js）。ts,等等)。
-框架指标(ASP。. NET, React, Azure，Next.js等)
-项目类型（web应用程序，api，库，工具，基础设施）
-开发工作流程需求（测试，CI/CD，部署）
-基础设施和云提供商（Azure、AWS、GCP）

🗨️**聊天记录上下文**：
-最近的讨论和痛点
-特性请求或实现需求
-代码审查模式
-开发工作流程要求
-专门的任务需求（绘图、评估、部署）

##输出格式

在结构化表格中显示分析结果，比较出色的副驾驶技能和现有的存储库技能。

|棒极了-副驾驶技能|描述|捆绑资产|已安装|类似的本地技能|建议原理||-----------------------|-------------|----------------|-------------------|---------------------|---------------------|
| [gh-cli](https://github.com/github/awesome-copilot/tree/main/skills/gh-cli) | GitHub管理存储库和工作流的CLI技能|无|❌否|无|将增强GitHub工作流自动化能力|
| [aspire](https://github.com/github/awesome-copilot/tree/main/skills/aspire) |分布式应用开发的aspire技能| 9参考文件|✅是| aspire |已经涵盖了现有的aspire技能|
| [Terraform - AzureRM -set-diff-analyzer](https://github.com/github/awesome-copilot/tree/main/skills/terraform-azurerm-set-diff-analyzer) | Analyze Terraform AzureRM提供程序更改|参考文件|⚠️过时| Terraform - AzureRM -set-diff-analyzer |使用新的验证模式更新的说明-更新推荐|

本地技能发现过程

1. 列出`.github/skills/`目录下的所有文件夹
2. 对于每个文件夹，读取`SKILL.md`前面的内容以提取`name`和`description`3. 列出每个技能文件夹中的任何绑定资产
4. 建立现有技能及其能力的综合清单
5. 使用这个清单来避免重复版本比较过程

1. 对于每个本地技能文件夹，构建原始GitHub URL以获取远程`SKILL.md`：
—模式：`https://raw.githubusercontent.com/github/awesome-copilot/main/skills/<skill-name>/SKILL.md`2. 使用`#fetch`工具获取远程版本
3. 比较整个文件内容（包括标题和正文）
4. 确定具体的差异：
- **首页内容更改**（名称、描述）
- **指令更新**（指南、示例、最佳实践）
- **捆绑资产变更**（新增、移除或修改资产）
5. 记录过时技能的关键差异
6. 计算相似度以确定是否需要更新

技能结构要求根据Agent Skills规范，每个技能是一个文件夹，包含：
- **`SKILL.md`**：包含前端内容（`name`,`description`）和详细说明的主指令文件
- **可选的捆绑资产**：脚本、模板、参考数据和其他从`SKILL.md`引用的文件
- **文件夹命名**：小写带连字符（例如，`azure-deployment-preflight`）
—**名称匹配**:`SKILL.md`前内容中的`name`字段必须与文件夹名称匹配

前物质结构

技能在了不起的副驾驶使用这个前面的问题格式在`SKILL.md`：```markdown
---
name: 'skill-name'
description: 'Brief description of what this skill provides and when to use it'
---
```
# #要求-使用`fetch`工具从awesome-copilot存储库技能文档中获取内容
-使用`githubRepo`工具获取个人技能内容进行下载
—扫描本地文件系统“`.github/skills/`”目录下的已有技能
—从本地`SKILL.md`文件中读取YAML前端内容，提取名称和描述
—将本地技能与远程版本进行比较，发现过时的技能
-与此存储库中的现有技能进行比较，以避免重复
-关注当前技能库覆盖率的差距
-验证建议的技能是否符合存储库的目的和技术栈
-为每个建议提供清晰的理由
-包括链接到令人敬畏的副驾驶技能和类似的本地技能
-清楚地识别过时的技能，并指出具体的差异
-考虑捆绑资产的需求和兼容性
-不要在表格和表格之外提供任何额外的信息或背景分析##图标参考

-✅已安装和最新
-⚠️已安装但已过时（可更新）
-❌没有安装在回购

##更新处理

当发现过时的技能时：
1. 将它们包含在具有⚠️状态的输出表中
2. 在“建议理由”一栏中记录具体的差异
3. 根据所注意到的关键更改提供更新建议
4. 当用户请求更新时，用远程版本替换整个本地技能文件夹
5. 保留`.github/skills/`目录中的文件夹位置
6. 确保所有捆绑的资产与更新的`SKILL.md`一起下载