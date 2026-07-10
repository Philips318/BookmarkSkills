---
name: "Daily Issues Report"
description: "Generates a daily summary of open issues and recent activity as a GitHub issue"
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
##每日问题报告

为团队创建未解决问题的每日总结。

##要包含什么

-过去24小时内的新问题
-已关闭或已解决的问题
-需要关注的陈腐问题