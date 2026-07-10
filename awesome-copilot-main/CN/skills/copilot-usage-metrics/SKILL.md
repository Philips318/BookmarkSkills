---
name: copilot-usage-metrics
description: Retrieve and display GitHub Copilot usage metrics for organizations and enterprises using the GitHub CLI and REST API.
---
#副驾驶使用指标

您是使用GitHub CLI （`gh`）检索和显示GitHub Copilot使用指标的技能。

何时使用此技能

当用户询问以下问题时使用此技能：
-副驾驶使用指标、采用率或统计数据
-有多少人在他们的组织或企业中使用Copilot
-副驾驶接受率、建议或聊天使用情况
-每个用户的副驾驶使用分解
-副驾驶使用的特定日期

如何使用这个技能

1. 确定用户想要**组织**还是**企业**级别的度量。
2. 如果没有提供，要求提供组织名称或企业名称。
3. 确定他们是想要**聚合**指标还是每个用户**指标。
4. 确定他们是否需要特定日期的指标（YYYY-MM-DD格式）或general/recent指标。
5. 从该技能的目录运行适当的脚本。

##可用脚本

组织度量-`get-org-metrics.sh <org> [day]`-获取组织的综合副驾驶使用指标。可选地通过YYYY-MM-DD格式的特定日期。
-`get-org-user-metrics.sh <org> [day]`-获取一个组织的每用户副驾驶使用指标。可选地通过特定的一天。

企业指标

-`get-enterprise-metrics.sh <enterprise> [day]`-获取企业的综合副驾驶使用指标。可选地通过特定的一天。
-`get-enterprise-user-metrics.sh <enterprise> [day]`-获取企业的每用户副驾驶使用指标。可选地通过特定的一天。

##格式化输出

当向用户显示结果时：
-总结关键指标：总活跃用户，接受率，总建议，总聊天互动
-使用表进行每个用户的细分
-如果比较多天，突出趋势
—指标数据从2025年10月10日开始可用，历史数据最长可访问1年

##重要事项-这些API端点需要**GitHub企业云**。
—用户必须具有适当的权限（企业所有者、计费经理或`manage_billing:copilot`/`read:enterprise`范围的令牌）。
-“副驾驶使用指标”策略必须在企业设置中启用。
—如果API返回403，建议用户检查token权限和企业策略设置。