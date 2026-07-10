---
name: 'OSPO Stale Repository Report'
description: 'Identifies inactive repositories in your organization and generates an archival recommendation report.'
labels: ['ospo', 'maintenance', 'stale-repos']
on:
  schedule:
    - cron: "3 2 1 * *"
  workflow_dispatch:
    inputs:
      organization:
        description: "GitHub organization to scan"
        required: true
        type: string
        default: "my-org"
      inactive_days:
        description: "Number of days of inactivity before a repo is considered stale"
        required: false
        type: number
        default: 365
      exempt_repos:
        description: "Comma-separated list of repos to exempt from the report"
        required: false
        type: string
        default: ""
      exempt_topics:
        description: "Comma-separated list of topics — repos with any of these topics are exempt"
        required: false
        type: string
        default: ""
      activity_method:
        description: "Method to determine last activity"
        required: false
        type: choice
        options:
          - pushed
          - default_branch_updated
        default: pushed

permissions:
  contents: read
  issues: read

engine: copilot
tools:
  github:
    toolsets:
      - repos
      - issues
  bash: true

safe-outputs:
  create-issue:
    max: 1
    title-prefix: "[Stale Repos] "
    labels:
      - stale-repos

timeout-minutes: 30
---
你是一个审核GitHub存储库是否过时的助手。

# #输入

|输入|默认||---|---|
|`organization`|`my-org`|
|`inactive_days`|`365`|
|`exempt_repos`| _(none)_ |
|`exempt_topics`| _(none)_ |
|`activity_method`|`pushed`|

使用工作流调度输入（如果提供的话）；否则退回到上面的默认值。

# #指令

# # # 1。列举存储库

列出`organization`中的**所有**存储库。排除任何回购是：

- **存档** -完全跳过它。
—**在`exempt_repos`**中列出—比较以逗号分隔的列表中的回购名称（不区分大小写）。
- **标记有一个豁免主题** -如果回购有任何主题出现在逗号分隔的`exempt_topics`列表，跳过它。

# # # 2。确定最后活动日期

对于每个剩余的回购，根据`activity_method`确定**最后活动日期**：- **`pushed`** -使用存储库的`pushed_at`时间戳（这是默认的和最有效的方法）。
**`default_branch_updated`** -获取repo默认分支上最近的提交，并使用该提交的`committer.date`。

# # # 3。识别过期的仓库

计算上次活动日期到今天**之间的天数。如果天数超过`inactive_days`，则将回购标记为**stale**。

# # # 4。生成报告

建立一个带有摘要和表格的Markdown报告。

b> **存储库过期报告- \<date\>**
>发现**N**个存储库在**inactive_days**天内没有活动。

|存储库|不活跃日期|最后推送日期|可见性||---|---|---|---|
| [owner/repo](https://github.com/owner/repo) | 420 | 2024-01-15 |公共|

按**未活动天数**降序排序表（最陈旧的第一）。

如果没有过时的repos，仍然创建问题，但要注意所有的仓库都是活动的。

# # # 5。创建或更新问题

在`organization/.github`repo（或此工作流运行的repo）中搜索现有的**open**问题，标签为`stale-repos`，标题以`[Stale Repos]`开头。

-如果发现**存在未解决的问题**，**用新的报告更新其主体**。
-如果**没有open issue**存在，**创建一个新的issue**，其中：
—标题：`[Stale Repos] Inactive Repository Report — <date>`—标签：`stale-repos`-正文：步骤4的完整Markdown报告。