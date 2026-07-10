---
name: azure-devops-cli
description: Manage Azure DevOps resources via CLI including projects, repos, pipelines, builds, pull requests, work items, artifacts, and service endpoints. Use when working with Azure DevOps, az commands, devops automation, CI/CD, or when user mentions Azure DevOps CLI.
---
# Azure DevOps CLI

使用带有Azure DevOps扩展的Azure CLI管理Azure DevOps资源。

**CLI版本：** 2.81.0（目前截至2025年）

# #先决条件```bash
# Install Azure CLI
brew install azure-cli  # macOS
curl -sL https://aka.ms/InstallAzureCLIDeb | sudo bash  # Linux

# Install Azure DevOps extension
az extension add --name azure-devops
```
# #身份验证```bash
# Login with PAT token
az devops login --organization https://dev.azure.com/{org} --token YOUR_PAT_TOKEN

# Set default organization and project (avoids repeating --org/--project)
# Note: Legacy URL https://{org}.visualstudio.com should be replaced with https://dev.azure.com/{org}
az devops configure --defaults organization=https://dev.azure.com/{org} project={project}

# List current configuration
az devops configure --list
```
命令行结构```
az devops          # Main DevOps commands
├── admin          # Administration (banner)
├── extension      # Extension management
├── project        # Team projects
├── security       # Security operations
│   ├── group      # Security groups
│   └── permission # Security permissions
├── service-endpoint # Service connections
├── team           # Teams
├── user           # Users
├── wiki           # Wikis
├── configure      # Set defaults
├── invoke         # Invoke REST API
├── login          # Authenticate
└── logout         # Clear credentials

az pipelines       # Azure Pipelines
├── agent          # Agents
├── build          # Builds
├── folder         # Pipeline folders
├── pool           # Agent pools
├── queue          # Agent queues
├── release        # Releases
├── runs           # Pipeline runs
├── variable       # Pipeline variables
└── variable-group # Variable groups

az boards          # Azure Boards
├── area           # Area paths
├── iteration      # Iterations
└── work-item      # Work items

az repos           # Azure Repos
├── import         # Git imports
├── policy         # Branch policies
├── pr             # Pull requests
└── ref            # Git references

az artifacts       # Azure Artifacts
└── universal      # Universal Packages
```
##参考文件

根据用户的任务，阅读相关的参考文件。每个文件都包含其域的完整命令语法和示例。

|文件|何时读取|覆盖||---|---|---|
|`references/repos-and-prs.md`|仓库、导入、pr （create/list/vote/reviewers/policies）、Git参考、分支策略|
管道，构建，发布，工件|管道CRUD，运行，构建，发布，工件download/upload|
工作项、sprint、区域路径|工作项（WIQL/create/update/relations）、区域路径、迭代、团队迭代|
|`references/variables-and-agents.md`|管道变量、座席池|管道变量、变量组、管道文件夹、座席pools/queues|
|项目、团队、用户、权限、wiki |项目、扩展、团队、用户、安全groups/permissions、服务端点、wiki、管理员|
|`references/advanced-usage.md`|输出格式，JMESPath查询|输出格式，JMESPath查询（基本+高级），全局参数，通用参数，Git别名|
|自动化脚本、最佳实践、错误处理|通用工作流、最佳实践、错误处理、脚本模式、Real-w世界的例子|
|`references/long-comments-on-windows.md`|长`--discussion`，`--description`，或`--content`值在Windows上失败|`cmd.exe`8191字符上限`az.cmd`， shell检测，和三个经过验证的解决方案（`azps.ps1`，原生`--file-path`，`az devops invoke --in-file`） |