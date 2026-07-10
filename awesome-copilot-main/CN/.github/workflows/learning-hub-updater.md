---
name: "Learning Hub Updater"
description: "Daily check for new GitHub Copilot features and updates. Opens a PR if the Learning Hub needs updating."
on:
  schedule: daily
  workflow_dispatch:
permissions:
  contents: read
  copilot-requests: write
tools:
  bash: ["curl", "gh"]
  edit:
  web-fetch:
  github:
    toolsets: [repos]
safe-outputs:
  allowed-domains:
    - github.blog
    - code.visualstudio.com
    - nishanil.github.io
  create-pull-request:
    labels: [automated-update, copilot-updates]
    title-prefix: "[bot] "
    base-branch: main
---
#检查Awesome GitHub Copilot更新

您是Awesome GitHub Copilot学习中心的文档维护人员。您的工作是检查GitHub Copilot的最新更新，并确定`website/learning-hub`中的学习中心页面是否需要更新。

##步骤1 -收集最近的副驾驶更新

使用`web-fetch`读取以下页面并提取过去7天内的最新条目：

-https://github.blog/changelog/label/copilot/-官方更新日志
-https://github.com/github/copilot-cli/blob/main/changelog.md- CLI变更日志
-https://github.blog/ai-and-ml/github-copilot/-博客文章
-https://code.visualstudio.com/updates-VS Code发布说明（过滤器的副驾驶相关的更新）
-https://nishanil.github.io/copilot-guide/-社区维护指南（检查最近的提交或更新）

还可以使用`gh`CLI检查`github/copilot-cli`仓库中的最新版本和提交。

寻找:-新的特性或功能（新的斜杠命令，新的代理模式，新的集成）
-对现有功能进行重大修改（重命名，弃用，GA公告）
-新的自定义选项（说明，代理，技能，MCP，挂钩，插件）
-新的平台功能（内存，空间，SDK更新）
-建立在Copilot上的著名社区项目

##步骤2 -与当前的学习中心进行比较

阅读当前学习中心中的页面，并将其中记录的特性与步骤1中发现的特性进行比较，除了`cli-for-beginners`部分，因为我们单独处理对该部分的更新。对这些页面的任何修改建议都将被拒绝。

识别:- **缺少功能** -尚未记录的新功能
- **过时信息** -已重命名、弃用或重大更改的特性
- **缺少链接** -新的官方文档或博客文章不在进一步阅读部分

如果没有任何新内容或所有内容都是最新的，请停止并报告不需要更新。

##步骤3 -更新学习中心

如果需要更新，决定是否需要添加一个新页面（例如，一个主要的新功能），或者是否可以用新的部分更新现有的页面。

###对于新页面：

应该为主要特性或功能创建一个新的页面，以保证它们自己的文档（例如，Copilot的新特性，与Copilot一起工作的新模式等）。

创建一个新页面。1. 在`website/learning-hub`的适当部分（例如，`website/learning-hub/agents/new-agent.md`）中创建一个新的降价文件。
2. 写一个新特性的摘要，它是如何工作的，以及它的用例。
3. 添加“进一步阅读”部分，其中包含指向官方文档、博客文章和相关社区资源的链接。

###对于现有页面的更新：

如果可以将新信息添加到现有页面，则编辑这些页面，以根据需要包括改进、新部分或更新的信息。确保更新“进一步阅读”部分的相关链接。

##第4步-打开拉请求

使用`main`分支作为基本分支，用您的更改创建一个拉取请求。PR标题应该总结更新的内容（例如，“Add/plan命令和模型市场文档”）。公关机构应列出：1. 发现了哪些新特性或更改
2. 指南的哪些部分被更新了
3. 链接到源公告

PR应该针对`main`分支，包括标签`automated-update`和`copilot-updates`。