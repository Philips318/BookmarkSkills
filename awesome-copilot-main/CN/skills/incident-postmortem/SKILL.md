---
name: incident-postmortem
description: 'Use when an outage, production incident, or significant service degradation has occurred and the team needs to write a structured blameless post-mortem. Triggers on phrases like "write a post-mortem", "incident review", "what went wrong", "outage report", "root cause analysis", or "RCA". Covers timeline reconstruction, contributing factor analysis, impact quantification, and action item generation with owners.'
---
#事故事后分析

指导团队在生产事件发生后撰写一份结构化的、无可指责的事后分析报告。输出是一份文件，它建立了共同的理解，确定了根本原因，而不是指责，并产生了具体的行动项目，以防止再次发生。

无可指责原则

失败的是系统，而不是人。我们的目标是了解事件是如何发生的，而不是谁造成的。避免说“X忘记了”、“Y应该知道”之类的话。使用“系统没有”，“进程缺乏”，“警报没有触发”。

##何时使用

-生产中断或服务降级已得到解决
-发生了一次重大的未遂事件（如果后来被发现就会成为事故）
—发生了面向用户的错误、数据丢失或违反SLA
-团队希望在上下文消失之前捕获学习**不适用于：**在分期、计划维护窗口或没有学习价值的事件中捕获的小错误。

##输入要求

在写验尸报告之前收集这些细节。询问丢失的东西：

###事件元数据
-事件标题（简短、描述性）
-检测日期和时间（带时区）
-解决的日期和时间
-严重程度/影响级别（P1-P4或同等级别）
-事故指挥官/随叫随到的船东

# # #的影响
—受影响的业务和系统
-对用户的影响（错误、速度慢、完全中断）
-估计受影响的用户数量
-数据丢失或损坏（yes/no，范围）
-SLA/SLO缺口（yes/no，缺口多少）###时间轴事件
重建的关键时刻：
-出现第一个症状
警报被触发（或被手动注意到）
-随叫随到页面/事件声明
-展开调查
-查明根本原因
-缓解措施
-确认全分辨率
-客户沟通（如有）

贡献因素
问团队：“是什么让事情变得更糟？”——而不是“谁失败了”。例子:
-警报阈值过高/警报未触发
- Runbook丢失或过期
- Deploy缺少回滚的特性标志
-监控没有涵盖这种故障模式
-随叫随到

# #过程

###步骤1 -收集元数据
如果用户没有提供完整的事件细节，请逐节询问。在你知道：标题，时间，严重程度，受影响的服务，以及至少一个粗略的时间表之前，不要开始写作。###步骤2 -重建时间线
与用户一起建立一个精确的时间轴。对于每个事件：
-准确时间（UTC优先）
-发生了什么（系统事件或人为行为）
-谁观察到它或采取了行动
-链接到日志/警报/ Slack消息（如果可用）

标志间隙：“我们不知道14:32到14:47之间发生了什么——值得查看日志。”

步骤3 -根本原因分析
反复使用“5个为什么”：```
Why did users see 500 errors?
→ The API pods were crash-looping.

Why were they crash-looping?
→ Memory limit was exceeded.

Why was the limit exceeded?
→ A new query was loading full result sets into memory.

Why wasn't this caught before deploy?
→ Load tests only covered the p50 case, not high-cardinality accounts.

Why did load tests only cover p50?
→ We had no test fixtures for large accounts.
```
当您到达可以修复的system/process间隙时停止。最后一个“为什么”应该指向一个行动项目。

区分:
**根本原因** -最深的系统缺口（一个或两个）
- **促成因素** -使情况变得更糟，但不是根本原因的条件

###步骤4 -影响量化
帮助用户精确：
-持续时间：从检测到解决（不是从症状开始到解决-将它们分开）
-峰值错误率与正常基线的对比
-受影响流量的百分比
-已知的收入/业务影响

###步骤5 -行动项目
对于每个根本原因和促成因素，至少产生一个行动项目：

| # |行动|所有者|到期日期|优先级||---|--------|-------|----------|----------|
| 1 |为帐户>添加负载测试fixture 10k记录| @eng-team | 2026-07-01 |高|
| 2 |内存告警阈值从90%降至75% | @平台| 2026-06-23 |高|
| 3 |为内存OOM pod添加runbook | @on-call-rotation | 2026-06-30 | Medium |

操作项必须有一个所有者（一个人，而不是一个团队）和一个截止日期。像“改善监控”这样模糊的行动是不可接受的——把它们分解成具体的可交付成果。

###第6步-编写文档
使用下面的模板制作完整的事后分析。保存到`docs/postmortems/YYYY-MM-DD-<slug>.md`。

##输出模板```markdown
# Post-Mortem: [Incident Title]

**Date:** YYYY-MM-DD  
**Severity:** P[1-4]  
**Duration:** X hours Y minutes (HH:MM UTC – HH:MM UTC)  
**Incident Commander:** @name  
**Status:** Resolved

---

## Summary

[2–3 sentences. What happened, what was the user impact, how was it resolved. Written for someone who wasn't involved.]

## Impact

| Dimension | Value |
|-----------|-------|
| Affected services | [list] |
| User-facing impact | [errors / degraded / full outage] |
| Users affected | [estimated number or %] |
| Peak error rate | [X% vs Y% baseline] |
| Data loss | [none / describe scope] |
| SLA breach | [yes/no — by how much] |

## Timeline

All times UTC.

| Time | Event |
|------|-------|
| HH:MM | [First symptom / alert fired] |
| HH:MM | [On-call paged] |
| HH:MM | [Incident declared] |
| HH:MM | [Root cause identified] |
| HH:MM | [Mitigation applied] |
| HH:MM | [Full resolution confirmed] |
| HH:MM | [Customer communication sent] |

## Root Cause

[1–2 paragraphs. The deepest systemic gap that, if fixed, would have prevented the incident. Written in blameless language. Reference the 5 Whys chain if helpful.]

## Contributing Factors

- [Factor 1 — condition that made the incident worse]
- [Factor 2]
- [Factor 3]

## What Went Well

- [Thing that worked — good alert, fast response, clear runbook]
- [Another positive]

## What Could Have Gone Better

- [Gap in process, tooling, or coverage — no blame language]
- [Another gap]

## Action Items

| # | Action | Owner | Due Date | Priority |
|---|--------|-------|----------|----------|
| 1 | [Specific deliverable] | @person | YYYY-MM-DD | High/Medium/Low |
| 2 | | | | |

## Lessons Learned

[Optional. 2–4 bullet points capturing non-obvious insights worth sharing with the broader team.]
```
常见错误

|错误|修复||---------|-----|
“Bob忘记检查配置”“|”“部署检查表不包括配置验证”“|”
根本原因是“人为错误”继续问为什么——人为错误总是一个症状
|没有所有者的行动项目|每个项目需要一个指定的个人，而不是一个团队|
|在写|之前检查日志、警报、Slack、PagerDuty
|指定：哪个服务，哪个度量，什么阈值，何时|
|几周后写的事后分析|在48-72小时内写，当时背景是新鲜的|