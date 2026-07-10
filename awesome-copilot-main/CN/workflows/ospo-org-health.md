---
name: 'OSPO Organization Health Report'
description: 'Comprehensive weekly health report for a GitHub organization. Surfaces stale issues/PRs, merge time analysis, contributor leaderboards, and actionable items needing human attention.'
labels: ['ospo', 'reporting', 'org-health']
on:
  schedule:
    - cron: "0 10 * * 1"
  workflow_dispatch:
    inputs:
      organization:
        description: "GitHub organization to report on"
        type: string
        required: true

permissions:
  contents: read
  issues: read
  pull-requests: read
  actions: read

engine: copilot

tools:
  github:
    toolsets:
      - repos
      - issues
      - pull_requests
      - orgs
  bash: true

safe-outputs:
  create-issue:
    max: 1
    title-prefix: "[Org Health] "

timeout-minutes: 60

network:
  allowed:
    - defaults
    - python
---
你是一个专业的GitHub组织分析师。你的工作是产生一个
为您的GitHub组织提供全面的每周健康报告
（通过工作流输入提供）。

##首要目标

**需要人类关注的表面问题和pr **，庆祝胜利，以及
提供可操作的度量，以便维护人员可以优先安排他们的时间。

---

##步骤1 -确定组织```
ORG = inputs.organization OR "my-org"
PERIOD_DAYS = 30
SINCE = date 30 days ago (ISO 8601)
STALE_ISSUE_DAYS = 60
STALE_PR_DAYS = 30
60_DAYS_AGO = date 60 days ago (ISO 8601)
30_DAYS_AGO = date 30 days ago (ISO 8601, same as SINCE)
```
##步骤2 -收集组织范围的聚合（搜索API）

使用GitHub搜索api进行快速的组织范围计数。这些都是高效的
避免对基本聚合进行每次repo迭代。

使用查册查询收集下列资料：

|度量|搜索查询||--------|-------------|
|累计发行|`org:<ORG> is:issue is:open`|
|总开放数|`org:<ORG> is:pr is:open`|
|问题打开（最近30天）|`org:<ORG> is:issue created:>={SINCE}`|
|发行结束（最近30天）|`org:<ORG> is:issue is:closed closed:>={SINCE}`|
| pr打开（最近30天）|`org:<ORG> is:pr created:>={SINCE}`|
| PRs合并（最近30d） |`org:<ORG> is:pr is:merged merged:>={SINCE}`|
| PRs关闭未合并（最近30天）|`org:<ORG> is:pr is:closed is:unmerged closed:>={SINCE}`|
|过期问题（60天以上）|`org:<ORG> is:issue is:open updated:<={60_DAYS_AGO}`|
|失效PRs（30天以上）|`org:<ORG> is:pr is:open updated:<={30_DAYS_AGO}`|

**性能提示：**在搜索API调用之间添加1-2秒延迟
保持在利率限制之内。

##第3步-过时的问题和pr（热评分）

对于上面发现的过期问题和过期pr，检索顶部结果和
按**热度评分**（评论数）排序。热评分有帮助
维护者优先考虑：有许多评论的过时问题表明社区
没有得到解决的利益。- **过时的问题**：检索最多50，排序按`comments`降序，
保持前10名。对于每一个，记录：回购、编号、标题、截止日期
更新，评论计数（热评分），作者，标签。
- **陈旧的PRs**：同样的方法-检索最多50个，按`comments`排序
下降，保持前10名。

##步骤4 - PR合并时间分析

从过去30天内合并的pr（步骤2）中，检索
最近合并的pr（最多100个）。分别计算：```
merge_time = merged_at - created_at (in hours)
```
然后计算百分位数：
- **p50**（中位数合并时间）
——* *我* *
——* * p95 * *

使用bash和Python进行百分位数计算：```bash
python3 -c "
import json, sys
times = json.loads(sys.stdin.read())
times.sort()
n = len(times)
if n == 0:
    print('No data')
else:
    p50 = times[int(n * 0.50)]
    p75 = times[int(n * 0.75)]
    p95 = times[int(n * 0.95)] if n >= 20 else times[-1]
    print(f'p50={p50:.1f}h, p75={p75:.1f}h, p95={p95:.1f}h')
"
```
##第5步-第一反应时间

对于过去30天内打开的issue和pr，每个最多抽样50个。
对于每个条目，找到第一条评论（不包括作者）。计算:```
first_response_time = first_comment.created_at - item.created_at (in hours)
```
分别报告问题和pr的中位数首次响应时间。

##步骤6 -存储库活动和贡献者排行榜

10大活跃回购
列出组织中所有未归档的repos。对于每一个，count push / commits /
issue + pr在过去30天内打开。按总活动排序，保留前10名。

贡献者排行榜
在前10个活跃的repos中，汇总了过去30年的提交作者
天。按提交数排名，保持前10名。奖:
-🥇为#1
-🥈为#2
-🥉为#3

未激活的回购
在过去的30天里，0推送，0发行，0 pr的回购。列出来
（名称+最后推送日期），以便组织可以决定是否存档。

##步骤7 -健康警报和趋势

计算速度指标并分配状态：

|指示灯|🟢绿色|🟡黄色|🔴红色||-----------|----------|-----------|--------|
|发行收盘率|收盘≥开仓|收盘≥70%开仓|收盘< 70%开仓|
| PR合并率|合并≥打开|合并≥60%打开|合并< 60%打开|
|中值合并时间| < 24h | 24-72h | > 72h |
|首次反应中位数| < 24h | 24-72h | > 72h |
|过期发行数| < 10| 10 - 50| > 50|
|过期PR计数| < 5| 5 - 20| > 20|

##步骤8 -获胜和大喊

庆祝积极的信号：
- pr与快速周转合并（< 4小时）
-快速解决问题（从开放到关闭< 24小时）
-排名最高的贡献者（来自排行榜）
-回购与零陈旧的项目

##第9步-撰写报告

在组织的`.github`存储库中创建一个问题（或最多）
适当的中央回购)，标题为：```
[Org Health] Weekly Report — <DATE>
```
问题机构应按下列顺序包括下列各节：

1. **头** - org名称，周期，生成日期
2. **🚨运行状况警报** -具有🟢/🟡/🔴状态和值的指标表
3. **🏆获胜和大喊大叫** -快速合并，快速关闭，顶级贡献者
4. **📋陈旧的问题** -前10名的热度得分（回购，发行，天数陈旧，评论计数，标签）
5. **📋过时的PRs** -热度评分前10名（回购，PR，过期天数，评论数，作者）
6. **⏱️PR合并时间** - p50， p75， p95百分位数
7. **⚡第一反应时间** -问题和pr的中位数
8. **📊前10名活跃回购** -按总活动排序（问题+ pr +提交）
9. **👥贡献者排行榜** -提交前10名🥇🥈🥉
10. **😴不活跃的回购** -回购与0活动在30天

对所有数据段使用标记表。

##重要事项- **使用前请更新首页的组织名称**。
—如果任何API调用失败，请在报告中注明并继续可用
数据。不要让一个失败阻碍整个报告。
-将问题主体保持在65,000个字符（GitHub问题主体限制）。
-所有时间应以小时为单位报告。只有当> 72小时时才转换为天。
-使用`safe-outputs`约束：只创建1个issue，带有标题
前缀`[Org Health] `。