#⚡代理工作流

[代理工作流]（https://github.github.com/gh-aw）是人工智能驱动的存储库自动化，在GitHub Actions中运行编码代理。它们使用自然语言指令在markdown中定义，通过内置护栏和安全优先设计实现事件触发和调度自动化。
###如何贡献

请参阅[CONTRIBUTING.md]（../CONTRIBUTING.md# addingagent -workflows）了解有关如何贡献新工作流、改进现有工作流和共享用例的指导方针。

如何使用代理工作流

* *包括:* *
-每个工作流是一个单一的`.md`文件与YAML前台和自然语言指令
—工作流程通过`gh aw compile`编译为`.lock.yml`GitHub Actions文件
-工作流遵循[GitHub代理工作流规范]（https://github.github.com/gh-aw）安装:* * * *
—安装`gh aw`命令行扩展：`gh extension install github/gh-aw`-将工作流`.md`文件复制到存储库的`.github/workflows/`目录
—使用`gh aw compile`编译生成`.lock.yml`文件
—同时提交`.md`和`.lock.yml`文件Activate/Use* *: * *
-工作流根据其配置的触发器（计划、事件、斜杠命令）自动运行
—使用`gh aw run <workflow>`触发手动运行
—监视器运行与`gh aw status`和`gh aw logs`**何时使用：**
-自动问题分类和标记
—生成每日状态报告
-自动维护文档
—定期执行代码质量检查
在issue和pr中响应斜杠命令
-编排多步骤存储库自动化

|名称|描述|触发|| ---- | ----------- | -------- |
|[每日问题报告](../workflows/daily-issues-report.md) |生成开放问题和最近活动的每日摘要，作为GitHub问题| schedule |
| [OSPO贡献者报告](../workflows/ospo-contributors-report.md) |跨组织存储库的每月贡献者活动度量。| schedule, workflow_dispatch
| [OSPO组织运行状况报告](../workflows/ospo-org-health.md) | GitHub组织的综合每周运行状况报告。显示陈旧的issues/PRs，合并时间分析，贡献者排行榜和需要人类关注的可操作项目。| schedule, workflow_dispatch
| [OSPO过期存储库报告](../workflows/ospo-stale-repos.md) |识别组织中不活跃的存储库，并生成存档建议报告。| schedule, workflow_dispatch
| [OSS发布遵从性检查器](../workflows/ospo-release-compliance-checker.md) |根据开源发布需求分析目标存储库，并发布详细的遵从性报告作为问题评论。b|问题，没有rkflow_dispatch |
|[相关性检查](../workflows/relevance-check.md) |斜杠命令评估一个问题或拉请求是否仍然与项目相关| slash_command，角色|
|[相关性摘要](../workflows/relevance-summary.md) |手动触发的工作流，将所有开放的问题和pr与/相关性检查响应汇总为单个问题| workflow_dispatch |
|[每周评论同步](../workflows/weekly-comment-sync.md) |每周工作流，查找过时的代码注释或README片段，进行纯文本同步更新，并在需要更改时打开草稿pull请求。| schedule, workflow_dispatch