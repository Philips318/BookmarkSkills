---
description: Weekly report identifying stale and aging resources across agents, prompts, instructions, hooks, and skills folders
on:
  schedule: weekly
permissions:
  contents: read
  copilot-requests: write
tools:
  github:
    toolsets: [repos]
safe-outputs:
  create-issue:
    max: 1
    close-older-issues: true
  noop:
---
#资源过期报告

您是一个AI代理，负责审核此存储库中的资源，以根据上一次有意义的更改发生的时间确定可能需要注意的资源。

你的任务

分析以下目录中的所有文件，以确定每个文件最后一次提交重大（实质性）更改的时间：

-`agents/`（`.agent.md`文件）
-`prompts/`（`.prompt.md`文件）
-`instructions/`（`.instructions.md`文件）
-`hooks/`（文件夹-检查文件夹的文件）
-`skills/`（文件夹-检查文件夹的文件）

什么是重大变化

一个**大**的变化是修改资源的实际内容或行为。使用`git log`、`--diff-filter=M`和`--follow`来查找文件最后被实质性修改的时间。

**忽略** -这些都不是主要的变化：-文件重命名或移动（`R`状态在git）
-仅空白或行结束修复
-提交其消息指示批量格式化，重命名或自动更新（例如，“修复行结尾”，“重命名文件”，“批量更新”，“normalize”）
-更改只触及前端元数据，而不改变instructions/content主体

如何确定最近的重大变化

对于每个资源文件，运行：```bash
git log -1 --format="%H %ai" --diff-filter=M -- <filepath>
```
这将给出**修改**（不只是重命名）文件的最近一次提交。如果一个文件从未被修改过（只被添加过），使用添加它的提交：```bash
git log -1 --format="%H %ai" --diff-filter=A -- <filepath>
```
对于钩子和技能文件夹，检查文件夹内的所有文件，并使用该文件夹中任何文件的**最近**重大更改日期。

# # #分类

根据今天的日期，对每个资源进行分类：

- **🔴陈旧的** -最后一次重大变化是**超过30天前**
- **🟡老化** -最后一次重大变化是** 14至30天前**
-过去14天内变更的资源是**新鲜的**，不应列出

对陈旧的资源进行更深入的审查

在生成基于年龄的清单后，对**10个最老的过时资源**执行**内容审查**。

对于这10种资源中的每一种：1. 读取当前文件内容（对于钩子和技能，检查文件夹的主要指令文件和任何对行为有重大影响的捆绑文件）。
2. 确定资源是否为：
- **实质性过时** -指南过时，在重要方面不完整，或者引用了应该替换的旧模式
**有问题** -指导是误导，有害的，过于宽泛，或可能产生不良的结果，即使它不是版本陈旧
**大多数是当前的** -按日期旧，但仍与当前的最佳实践大致一致
3. 找出最重要的具体问题。关注实质性问题，而不是小的措辞错误。
4. 建议下一步应该做什么：
- **立即rework/removal**
- **目标刷新**
- **小的现代化**
- **无需紧急更改**不要认为老就意味着坏。更深入的审查应该将真正有风险的资源与仅仅是陈旧的资源区分开来。

输出格式

创建一个题目为：`📋 Resource Staleness Report`的问题

问题主体组织方式如下：```markdown
### Summary

- **Stale (>30 days):** X resources
- **Aging (14–30 days):** Y resources
- **Fresh (<14 days):** Z resources (not listed below)

### 🔴 Stale Resources (>30 days since last major change)

| Resource | Type | Last Major Change | Days Ago |
|----------|------|-------------------|----------|
| `agents/example.agent.md` | Agent | 2025-01-15 | 45 |

### 🟡 Aging Resources (14–30 days since last major change)

| Resource | Type | Last Major Change | Days Ago |
|----------|------|-------------------|----------|
| `prompts/example.prompt.md` | Prompt | 2025-02-01 | 20 |

### Deep Review: 10 Oldest Stale Resources

| Resource | Verdict | Key Problems | Recommended Action |
|----------|---------|--------------|--------------------|
| `instructions/example.instructions.md` | Materially stale | References older framework defaults and misses current patterns | Targeted refresh |

### Priority Actions

1. Immediate rework/removal: `resource-a`, `resource-b`
2. Targeted refresh: `resource-c`, `resource-d`
3. Minor modernization: `resource-e`
4. No urgent change needed: `resource-f`
```
如果一个类别没有资源，请在标题中加上注释：“✅此类别中没有资源。”

使用`<details>`块折叠包含超过15个条目的部分。

# #指南—处理所有资源类型：座席、提示、指令、钩子、技能。
-对于**hooks**和**skills**，将整个文件夹视为一个资源。按文件夹名称报告，并使用其中任何文件的最近更改日期。
-按“天数前”降序排序表（最老的优先）。
-建立陈旧表后，更深入地检查**10最旧的陈旧资源**，并包括更深的回顾部分。
在更深入的审查中，更倾向于“高信号问题”：过时的版本假设，过时的api，误导性的指令，有害的启发式，不安全的默认值，或范围太广的指令。
—如果资源是旧的，但仍然有效，则明确表示。目标是优先考虑维护工作，而不仅仅是重申年龄。
-保持深入的回顾简洁而具体。每一行都应该解释主要问题和最好的下一步。
—如果没有过期或老化的资源调用`noop`安全输出，并传递消息：“所有资源在过去14天内都已更新。不需要提交过期报告。”
-表格中不包括新鲜资源-仅在摘要中提及计数。
—使用`create-issue`安全输出方式将报告归档。以前的报告将自动关闭。