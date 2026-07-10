---
title: 'Agentic Workflows'
description: 'Learn what GitHub Agentic Workflows are, how to use community workflows from Awesome Copilot, and how to contribute your own.'
authors:
  - GitHub Copilot Learning Hub Team
lastUpdated: 2026-02-27
estimatedReadingTime: '7 minutes'
tags:
  - workflows
  - automation
  - github-actions
  - fundamentals
relatedArticles:
  - ./automating-with-hooks.md
  - ./using-copilot-coding-agent.md
prerequisites:
  - Basic understanding of GitHub Actions
  - Basic understanding of GitHub Copilot
---
代理工作流是由ai驱动的存储库自动化，在GitHub Actions中运行编码代理。它们用自然语言指令编写，可以让您自动执行问题分类、每日报告和合规性检查等任务——由时间表、事件或斜杠命令触发。

本文介绍了代理工作流是什么，如何安装和使用来自Awesome Copilot社区的工作流，以及如何贡献自己的工作流。

什么是代理工作流？

代理工作流是一个标记文件，它将YAML前端内容（触发器、权限、安全输出）与编码代理在运行时遵循的自然语言指令结合在一起。markdown文件是源代码：您使用`gh aw`CLI将其编译为`.lock.yml`工作流文件，GitHub Actions运行编译后的工作流，以执行自动遵循指令的Copilot编码代理。* * * *关键特征:
-在单个`.md`文件中定义-不需要YAML操作语法
—由调度、存储库事件或斜杠命令触发
-运行在GitHub Actions与副驾驶编码代理
—使用最小权限和安全输出来保证安全性
—通过`gh aw`命令行编译为`.lock.yml`文件

###一个工作流文件的解剖```markdown
---
name: "Daily Issues Report"
description: "Generates a daily summary of open issues"
on:
  schedule: daily on weekdays
permissions:
  contents: read
  issues: read
safe-outputs:
  create-issue:
    title-prefix: "[daily-report] "
    labels: [report]
---

## Daily Issues Report

Create a daily summary of open issues for the team.

## What to Include

- New issues opened in the last 24 hours
- Issues closed or resolved
- Stale issues that need attention
```
frontmatter声明工作流的触发器、权限和安全输出。主体包含代理遵循的自然语言指令。

何时使用代理工作流

|用例|示例||----------|---------|
|定期报告|每日问题摘要，每周组织运行状况检查|
|事件驱动自动化|分类新问题，检查PR相关性|
|斜杠命令|`/relevance-check`对一个问题或PR |
遵从性检查|许可证审计，发布准备审查|
|仓库维护|识别过时的仓库，跟踪贡献者的活动|

当您需要超越静态GitHub Actions所能完成的自治的、事件驱动的自动化（需要推理、总结或上下文感知决策的任务）时，代理工作流是理想的选择。

##使用来自Awesome Copilot的工作流

[Awesome Copilot工作流页面]（../../workflows/）托管了越来越多的社区贡献的工作流。下面是如何安装和使用它们。

# # #先决条件

安装`gh aw`CLI扩展：```bash
gh extension install github/gh-aw
```
安装工作流

1. **浏览**的[工作流集合](../../workflows/)，并找到一个适合您的需求
2. **复制工作流`.md`文件到存储库的`.github/workflows/`目录
3. **编译**工作流以生成Actions锁文件：```bash
gh aw compile
```
4. **同时提交`.md`源文件和生成的`.lock.yml`文件：```bash
git add .github/workflows/daily-issues-report.md
git add .github/workflows/daily-issues-report.lock.yml
git commit -m "Add daily issues report workflow"
```
###运行工作流

工作流根据其配置的触发器自动运行。您还可以：

—**手动触发**:`gh aw run <workflow>`—**监控运行**:`gh aw status`和`gh aw logs`—**本地验证**:`gh aw compile --validate --no-emit <workflow>.md`自定义工作流

由于工作流是简单的标记，定制它们很简单：

- **编辑主体中的指令**来调整代理的行为
- **改变触发器**在`on:`frontmatter控制，当它运行
- **调整权限**以匹配您的存储库的需求
- **修改安全输出**控制代理可以创建或更新的内容

编辑后，使用`gh aw compile`重新编译以重新生成锁文件。

贡献工作流

与社区共享您的工作流可以帮助其他人自动化他们的存储库。以下是如何贡献的方法。

步骤1：创建工作流文件在[Awesome Copilot存储库]（https://github.com/github/awesome-copilot）的`workflows/`目录中创建一个新的`.md`文件。使用一个描述性的、小写的、连字符的文件名：```
workflows/my-new-workflow.md
```
步骤2：添加Frontmatter

包括所需的标题字段：```yaml
---
name: "My New Workflow"
description: "A clear description of what this workflow does"
on:
  schedule: daily
permissions:
  contents: read
safe-outputs:
  create-issue:
    title-prefix: "[my-workflow] "
    labels: [automated]
---
```
* *必填字段* *:
-`name`-人类可读的工作流名称
-`description`-工作流程目的的简明总结

* * * *工作流字段:
-`on`-触发配置（计划、事件、斜杠命令）
-`permissions`- GitHub API范围（使用最小权限）
-`safe-outputs`-代理可以创建或修改的护栏

###步骤3：写清楚的说明

文件主体包含代理遵循的自然语言指令。具体而有条理：```markdown
## Task Overview

Describe the main goal clearly.

## Steps

1. First, gather the relevant data
2. Then, analyze and summarize
3. Finally, create the output (issue, comment, etc.)

## Output Format

Describe the expected format of the result.
```
步骤4：验证和测试```bash
# Validate the workflow compiles correctly
gh aw compile --validate --no-emit workflows/my-new-workflow.md
```
第五步：提交你的贡献

1. 分叉存储库并创建一个新的分支
2. 将您的工作流`.md`文件添加到`workflows/`目录
3. 执行`npm run build`命令更新README
4. 提交一个针对`main`分支的拉取请求

b> **重要：**只提交`.md`源文件。不要包含编译过的`.lock.yml`或`.yml`文件—CI将阻止它们。

工作流贡献指南

- **安全第一** -使用最小权限和安全输出代替直接写访问
- **清晰的指令** -在工作流主体中编写特定的，明确的自然语言
- **描述性名称** -使用带连字符的小写文件名（例如，`daily-issues-report.md`）
- **本地测试** -提交前用`gh aw compile --validate`验证
**文档的目的** -`description`字段应该清楚地说明工作流是做什么的，什么时候使用它

##了解更多- **官方文档**:[GitHub代理工作流](https://gh.io/gh-aw) -完整的规范和参考
- **浏览工作流**:[Awesome Copilot工作流](../../workflows/) -社区贡献的集合
- **投稿指南**:[CONTRIBUTING.md](https://github.com/github/awesome-copilot/blob/main/CONTRIBUTING.md#adding-agentic-workflows) -详细投稿指南
- **相关**:[自动化与Hooks](../automating-with-hooks/) -副驾驶代理会话的确定性自动化
- **相关**:[使用副驾驶编码代理](../using-copilot-coding-agent/) -代理的权力代理工作流

---