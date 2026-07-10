---
name: Relevance Summary
description: "Manually triggered workflow that summarizes all open issues and PRs with a /relevance-check response into a single issue"
on:
  workflow_dispatch:
engine:
  id: copilot
permissions:
  contents: read
  issues: read
  pull-requests: read
tools:
  github:
    toolsets: [default]
safe-outputs:
  create-issue:
    title-prefix: "[Relevance Summary] "
    labels: [report]
    close-older-issues: true
---
#相关性检查汇总报告

您是**${{github的报告生成器。**存储库。
您的工作是找到所有开放的问题和收到`/relevance-check`响应的pull请求，并编译一个摘要问题。

# #指令

# # # 1。查找相关项目

搜索这个存储库中所有**open** issue和pull requests。
对于每一个，阅读它的注释并查找包含“相关性评估”**部分的注释—这是`/relevance-check`斜杠命令的输出。

相关性检查响应包含以下标记：
-标题或加粗文本**“相关性评估：”**后面跟着：`Still Relevant`，`Likely Outdated`，或`Needs Discussion`之一
- **建议**部分，其中有：✅**保持开放**，🗄️**考虑关闭**，或💬**需要维护人员的输入**

# # # 2。提取信息对于每个具有相关性检查响应的问题或PR，摘录如下：
-issue/PR编号和标题
-是issue还是pull request
-相关性评估结论（仍然相关/可能过时/需要讨论）
-建议的措施（保持开放/考虑关闭/需要维护人员的输入）

# # # 3。创建摘要问题

创建一个单独的问题，用表格总结所有的发现。使用这个结构：```
### Relevance Check Summary

Summary of all open issues and pull requests that have been evaluated with `/relevance-check`.

**Generated:** YYYY-MM-DD

| # | Type | Title | Assessment | Recommendation |
|---|------|-------|------------|----------------|
| [#N](link) | Issue/PR | Brief title | Still Relevant / Likely Outdated / Needs Discussion | ✅ Keep open / 🗄️ Consider closing / 💬 Needs maintainer input |

### Statistics
- Total evaluated: N
- Still Relevant: N
- Likely Outdated: N
- Needs Discussion: N
```
# # # 4。指导方针

-如果没有开放的问题或pr有相关性检查响应，创建一个问题，说明没有找到项目。
-根据评估对表格进行排序：首先列出“可能过时”的项目（最具可操作性），然后是“需要讨论”，然后是“仍然相关”。
-在表格中保持标题简短-如果需要，截断到~60个字符。
—总是将issue/PR号码链接到它的URL。