#项目规划和管理插件

用于软件项目计划、特性分解、史诗管理、实现计划和开发团队任务组织的工具和指南。

# #安装```bash
# Using Copilot CLI
copilot plugin install project-planning@awesome-copilot
```
包含的内容

###命令（斜杠命令）

|命令|描述||---------|-------------|
|`/project-planning:breakdown-feature-implementation`|提示创建详细的功能实现计划，遵循Epoch单线程结构。|
|`/project-planning:breakdown-feature-prd`|基于Epic为新特性创建产品需求文档（prd）的提示。|
|`/project-planning:breakdown-epic-arch`|基于产品需求文档为Epic创建高级技术架构的提示。|
|`/project-planning:breakdown-epic-pm`|为新史诗创建史诗产品需求文档（PRD）的提示。此PRD将用作生成技术架构规范的输入。|
为新特性、重构现有代码或升级包、设计、架构或基础设施创建新的实现计划文件。|
|`/project-planning:update-implementation-plan`|使用新的或更新需求更新现有的实现计划文件，以提供新功能、重构现有代码或升级包、设计、体系结构或基础设施。|
|`/project-planning:create-github-issues-feature-from-implementation-plan`| C使用feature_request.yml或chore_request.yml模板从实施计划阶段创建GitHub问题。|
|`/project-planning:create-technical-spike`|创建时间限制的技术峰值文档，用于在实现之前研究和解决关键的开发决策。|# # #代理

|代理|描述||-------|-------------|
创建可执行的执行计划的任务规划器-由microsoft/edge-ai|提供
|`task-researcher`|综合项目分析任务研究专家-由microsoft/edge-ai|为您带来
为新特性或重构现有代码生成实现计划。|
战略规划和架构助理，专注于在实施之前进行深思熟虑的分析。帮助开发人员理解代码库，澄清需求，并开发全面的实现策略。|
在Markdown中生成一个全面的产品需求文档（PRD），详细说明用户故事、验收标准、技术考虑和度量。可选地在用户确认后创建GitHub问题。|
为新特性或重构现有代码生成实现计划。|
系统的研究和验证技术艾尔·斯派克通过详尽的调查和控制实验记录了文件。|# #源

这个插件是[Awesome Copilot]（https://github.com/github/awesome-copilot）的一部分，这是一个社区驱动的GitHub Copilot扩展集合。

# #许可证

麻省理工学院