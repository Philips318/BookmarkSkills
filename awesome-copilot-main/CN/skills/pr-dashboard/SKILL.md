---
name: pr-dashboard
description: 'Open a GitHub PR dashboard in the browser. Use when the user asks to see their pull requests, open the PR dashboard, show PRs for a date range, or check PR status. Trigger phrases include "show my PRs", "open PR dashboard", "pull request dashboard".'
---
# PR仪表盘

在浏览器中为给定的日期范围和角色过滤器生成并打开GitHub PR仪表板。

**前提条件：** GitHub CLI （`gh`）必须安装并认证（`gh auth login`）。

##该怎么做

找到与此技能绑定的CLI脚本并运行它：```bash
SKILL_SCRIPT=$(find ~/.copilot -name "pr-dashboard-cli.mjs" -path "*/pr-dashboard/scripts/*" 2>/dev/null | head -1)
node "$SKILL_SCRIPT" "<query>" "<role>"
```
—`<query>`：用户指定的日期范围（默认为`last 7 days`）
-`<role>`:`Authored by me`,`Requested reviews`,`Assigned to me`，`All`之一（默认：`Authored by me`）

解析用户的请求

从用户消息中提取日期范围和角色。例子:

|用户输入|查询|角色||---|---|---|
|显示我的PRs |`last 7 days`|`Authored by me`|
|显示我最近两周的pr |`last 2 weeks`|`Authored by me`|
| PR仪表板本月评论|`this month`|`Requested reviews`|
| PR仪表板2026年3月分配|`march 2026`|`Assigned to me`|
|显示最近30天的所有pr |`last 30 days`|`All`|

**角色关键字映射：**
“my PRs”，“authored”，“I written”→`Authored by me`- "reviews", "review requested", “reviewing”→`Requested reviews`-“assigned”→`Assigned to me`- "all", “ involved me”→`All`支持的日期范围格式

脚本理解自然语言——按原样传递它：
-`last 7 days`,`last 2 weeks`,`last 30 days`-`this week`,`last week`,`this month`,`last month`-`march 2026`、`feb 2025`——`2026-01-01 - 2026-03-31`-`2025`（全年）

##运行后

告诉用户正在浏览器中打开仪表板。该脚本将进度输出到标准输出。如果退出时显示错误，则显示错误输出，并建议他们运行`gh auth login`（如果是验证问题）。