---
name: 'OSPO Contributors Report'
description: 'Monthly contributor activity metrics across an organization''s repositories.'
labels: ['ospo', 'reporting', 'contributors']
on:
  schedule:
    - cron: "3 2 1 * *"
  workflow_dispatch:
    inputs:
      organization:
        description: "GitHub organization to analyze (e.g. github)"
        required: false
        type: string
      repositories:
        description: "Comma-separated list of repos to analyze (e.g. owner/repo1,owner/repo2)"
        required: false
        type: string
      start_date:
        description: "Start date for the report period (YYYY-MM-DD)"
        required: false
        type: string
      end_date:
        description: "End date for the report period (YYYY-MM-DD)"
        required: false
        type: string
      sponsor_info:
        description: "Include GitHub Sponsors information for contributors"
        required: false
        type: boolean
        default: false

permissions:
  contents: read
  issues: read
  pull-requests: read

engine: copilot

tools:
  github:
    toolsets:
      - repos
      - issues
      - pull_requests
      - orgs
      - users
  bash: true

safe-outputs:
  create-issue:
    max: 1
    title-prefix: "[Contributors Report] "

timeout-minutes: 60
---
#贡献者报告

为指定的组织或存储库生成贡献者报告。

##步骤1：验证配置

检查工作流输入。必须提供`organization`或`repositories`。

—如果**两个**都为空，并且这是一个**计划运行**，默认分析拥有当前存储库的组织中的所有公共存储库。从`GITHUB_REPOSITORY`环境变量（`/`之前的部分）确定org。
-如果**两个**都为空，并且这是一个**手动调度**，则失败并显示一个明确的错误消息：“您必须提供一个组织或一个以逗号分隔的存储库列表。”
—如果**同时提供**，则优先选择`repositories`，忽略`organization`。

步骤2：确定日期范围—如果已有“`start_date`”和“`end_date`”，则直接使用。
—否则，默认为上一个日历月**。例如，如果今天是2025-03-15，则取值范围是2025-02-01 ~ 2025-02-28。
—根据需要使用bash计算日期。将它们存储为`START_DATE`和`END_DATE`。

##步骤3：枚举存储库

—如果输入的是`repositories`，将以逗号分隔的字符串拆分为一个列表。每个条目应该采用`owner/repo`格式。
-如果提供了`organization`输入（或从步骤1默认），列出所有**公共，非存档，非分支**库在组织中使用GitHub API。收集它们的`owner/repo`标识符。

##步骤4：从提交历史中收集贡献者

对于范围内的每个存储库：1. 使用GitHub API列出`START_DATE`和`END_DATE`之间的提交（在提交端点上使用`since`和`until`参数）。
2. 对于每个提交，提取**作者登录**（从提交对象上的`author.login`）。
3. **排除bot账户**：跳过用户名包含`[bot]`或`type`字段为`"Bot"`的贡献者。
4. 跟踪per-contributor:
-所有仓库的总提交数。
-他们贡献的repos集合。

使用bash聚合所有存储库中的贡献者数据并进行重复数据删除。

步骤5：确定新贡献者和老贡献者

对于在步骤4中找到的每个贡献者，检查他们在`START_DATE`**之前是否在任何作用域内存储库中有任何提交。

-如果贡献者在`START_DATE`**之前没有提交，将其标记为**新贡献者**。
-否则，将其标记为**返回贡献者**。##第六步：收集赞助商信息（可选）

如果输入的`sponsor_info`为`true`：

1. 对于每个贡献者，通过GitHub API查询用户的配置文件，检查他们是否有GitHub赞助商配置文件。
2. 如果用户启用了赞助，则将其赞助商URL记录为`https://github.com/sponsors/<username>`。
3. 如果不是，则将赞助商字段保留为空。

##步骤7：生成Markdown报告

建立一个降价报告，结构如下：

汇总表

|度量值|值||---|---|
|贡献者总数|计数|
|总贡献（提交）|计数|
|新增贡献者|计数|
|返回贡献者|计数|
新贡献者|百分比|

贡献者详细信息表

按提交计数降序排序贡献者。

| # |用户名|贡献计数|新贡献者|赞助商URL |提交||---|---|---|---|---|---|
| 1 | @用户名| 42 |是|[赞助商](url) |[视图]（提交-url） |

- **Username**列应该链接到贡献者的GitHub配置文件。
-如果`sponsor_info`为假或用户没有赞助商页面，**赞助商URL**列应该显示“N/A”。
**Commits**列应该链接到一个过滤的提交视图。

##步骤8：创建问题与报告

在**当前存储库**中创建一个问题：

- **标题：**`[Contributors Report] <ORG_OR_REPO_SCOPE> — START_DATE to END_DATE`- **正文：**步骤7的完整降价报告。
- **标签：**添加标签`contributors-report`，如果存在；如果没有，也不要失败。