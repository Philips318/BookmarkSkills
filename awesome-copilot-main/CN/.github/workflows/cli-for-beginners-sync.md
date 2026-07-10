---
name: "CLI for Beginners Content Sync"
description: "Weekly check for updates to github/copilot-cli-for-beginners. Opens a PR to keep the Learning Hub mirror aligned when substantive upstream course changes are detected."
on:
  schedule: weekly
permissions:
  contents: read
  copilot-requests: write
tools:
  github:
    toolsets: [repos]
  cache-memory: true
safe-outputs:
  create-pull-request:
    labels: [automated-update, learning-hub, cli-for-beginners]
    title-prefix: "[bot] "
    base-branch: main
---
# CLI初学者内容同步

你是一个文档同步代理**awesome-副驾驶**学习中心。您的工作是检查自上次运行以来，上游源存储库[`github/copilot-cli-for-beginners`]（https://github.com/github/copilot-cli-for-beginners）是否收到了任何有意义的更新，如果有，则更新学习中心镜像，使其与上游课程保持一致。

##第1步-确定上游回购中的新内容

1. 读取`cache-memory`并查找名为`cli-for-beginners-sync-state.json`的文件。它可能包含：
-`last_synced_sha`-你在上一次运行中处理的最近的提交SHA
—`last_synced_at`—文件系统安全的时间戳，格式为`YYYY-MM-DD-HH-MM-SS`2. 使用GitHub工具从`github/copilot-cli-for-beginners`（默认分支）获取最近的提交：
-如果`last_synced_sha`存在，列表自SHA**以来提交**（一旦到达它就停止）。
—如果不存在缓存状态，列出**过去7天**的提交。3. 确定哪些文件在这些提交中发生了更改。关注:    - Markdown files (`*.md`) — course content, README, module descriptions
    - Supporting assets referenced by the course material, especially screenshots and GIFs
    - Any configuration or metadata files that materially affect the course content or navigation
4. 如果自上次同步以来没有发现任何提交，那么在这里停止并调用`noop`安全输出，并发出这样的消息：“自上次同步（`<last_synced_sha>`）以来，在`github/copilot-cli-for-beginners`中没有发现新的提交。”不需要采取行动。”然后用最新的SHA更新缓存。

##步骤2 -读取更改的上游内容

对于上游repo中更改的每个文件，使用GitHub工具从`github/copilot-cli-for-beginners`获取**当前文件内容**。密切注意：

-引入了新的部分、命令、标志或概念
-重命名或重组的部分
-已移除的弃用命令或工作流
-更新截图，gif，图片参考，或代码示例
-链接到新的官方文件或资源

##步骤3 -与本地学习中心的内容进行比较

阅读规范学习中心课程文件夹`website/src/content/docs/learning-hub/cli-for-beginners/`中的本地文件：```
website/src/content/docs/learning-hub/cli-for-beginners/
├── index.md
├── 00-quick-start.md
├── 01-setup-and-first-steps.md
├── 02-context-and-conversations.md
├── 03-development-workflows.md
├── 04-agents-and-custom-instructions.md
├── 05-skills.md
├── 06-mcp-servers.md
└── 07-putting-it-all-together.md
```
当上游更改触及屏幕截图，横幅或gif时，还检查`website/public/images/learning-hub/copilot-cli-for-beginners/`中的本地课程资产。

如果上游改变了航向结构或航行，你可能还需要检查：- `website/astro.config.mjs`
- `website/src/content/docs/learning-hub/index.md`
将上游更改映射到相关的本地文件。问问你自己:

-本地镜像是否缺少任何上游内容、结构、作业、示例或视觉效果？
现有的学习中心内容是否因为上游的变化而过时或不正确？
-本地路由重写，repo-link重写，或资产路径需要更新，镜像页面仍然在网站上工作？
Astro frontmatter字段（特别是`lastUpdated`）是否需要更新，因为镜像页面发生了变化？

如果本地内容已经与上游的更改完全一致—或者上游的更改不是实质性的（例如，只有CI配置、错字修复或内部工具更改）—就到此为止，调用`noop`安全输出，并给出简短的解释。仍然用最新的提交SHA更新缓存。

##步骤4 -更新学习中心文件

对于每个需要更新的本地文件：1. 编辑相关的本地文档、资源和支持导航文件，使网站保持上游课程的源忠实镜像。
-添加或更新缺失的概念，命令，标志，步骤，作业，演示和视觉效果
-纠正或删除过时的信息
-本地化新添加的截图或gif到`website/public/images/learning-hub/copilot-cli-for-beginners/`-碰撞的`lastUpdated`首页字段到今天的日期（`YYYY-MM-DD`），任何页面的镜像内容发生了变化

2. 保持“镜像优先”的方法：
尽可能地保留上游的措辞、标题、章节顺序、分配和整个章节流程
-不要总结、重新解释或“网站优化”课程，使其成为不同的学习体验
-只适应网站需要的内容：Astro frontmatter，路由安全的内部链接，GitHub repo链接，本地资产路径，以及展示所需的小HTML/CSS钩子    - Convert repo-root relative links that are invalid on the published website (for example `../.github/agents/`, `./.github/...`, or `.github/...`) into absolute links to `https://github.com/github/copilot-cli-for-beginners` (use `/tree/main/...` for directories and `/blob/main/...` for files)
3. 如果上游添加、删除或重命名主要部分或章节：
—在“`website/src/content/docs/learning-hub/cli-for-beginners/`”中创建、删除或重命名相应的markdown文件
-更新`website/astro.config.mjs`，如果侧边栏的章节列表必须改变
-更新`website/src/content/docs/learning-hub/index.md`只有当登陆页的课程条目必须改变

##步骤5 -更新同步状态缓存

在打开PR之前，使用以下命令将更新后的`cli-for-beginners-sync-state.json`写入`cache-memory`：```json
{
  "last_synced_sha": "<latest commit SHA from github/copilot-cli-for-beginners>",
  "last_synced_at": "<YYYY-MM-DD-HH-MM-SS>",
  "files_reviewed": ["<list of upstream files you compared>"],
  "files_updated": ["<list of local Learning Hub files you edited>"]
}
```
##第6步-打开拉请求

使用`create-pull-request`安全输出创建一个包含更改的pull请求。使用`main`作为与此工作流相关的所有工作的基本分支。公关机构必须包括：

1. **上游发生了什么变化** -`github/copilot-cli-for-beginners`中提交和文件更改的简明总结
2. **本地更新的内容** -列出您编辑的每个镜像学习中心文件或资产以及更改的内容
3. **源链接** -链接到相关的上游提交或文件
4. 注意，这个工作流的markdown主体可以直接在GitHub.com上编辑，而不需要重新编译

如果在分析之后没有什么需要更改的，那么就**不要**打开PR，而是调用`noop`安全输出。

# #指南-规范课程内容存在于`website/src/content/docs/learning-hub/cli-for-beginners/`；不要在其他地方重新创建遗留副本
-偏好课程文档和`website/public/images/learning-hub/copilot-cli-for-beginners/`内的更改
-仅在上游航向结构或导航确实需要时编辑`website/astro.config.mjs`或`website/src/content/docs/learning-hub/index.md`-保留现有的前缘田；如果确实需要，只更新`lastUpdated`和`description`-保持课程来源忠实；避免总结或解释性重写
-使用`main`作为这个工作流创建的任何分支或PR的基础分支
-不要自动合并；PR是供人审核的
-如果你不确定上游变更是否需要更新学习中心，那就创建PR吧——人工审阅者总是可以拒绝的
-总是调用`create-pull-request`或`noop`在你的运行结束，所以工作流程清楚地表明其结果