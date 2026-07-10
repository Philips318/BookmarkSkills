---
name: workiq-copilot
description: 'Guides the Copilot CLI on how to use the WorkIQ CLI/MCP server to query Microsoft 365 Copilot data (emails, meetings, docs, Teams, people) for live context, summaries, and recommendations.'
---
# WorkIQ副驾驶技能

# #概述

WorkIQ（公开预览版）允许副驾驶用自然语言查询微软365数据。它支持日程安排、文档、团队消息、电子邮件线程、后续跟踪、涉众总结等等。只要任务需要本地存储库之外的实时组织智能，就可以使用此技能。

支持的数据和示例提示

- **邮件** -“总结Sarah发来的关于预算的邮件。”
**会议** -“我这周有哪些会议？”
- **文件** -“查找有关第四季度规划的最新文件。”
- **团队** -“总结今天工程频道的消息。”
“谁在做Alpha项目？”

##获取访问权限1. **Copilot CLI插件（首选）**
——`copilot`——`/plugin marketplace add github/copilot-plugins`——`/plugin install workiq@copilot-plugins`—重新启动Copilot命令行。
2. **独立的CLI / MCP服务器**
—`npm install -g @microsoft/workiq`（或`npx -y @microsoft/workiq mcp`）。
—执行`workiq mcp`命令，公开MCP工具。
3. * *租户同意* *
-首次使用提示微软365管理同意（EULA +权限）。非管理员必须根据《租户管理员启用指南》联系租户管理员进行审批。

##飞行前检查清单

—执行命令`Get-Command workiq`，确保二进制文件可用。
-通过`workiq accept-eula`接受EULA一次。
—确认正确的租户（如果与默认的`common`不同，则为`-t <tenant-id>`）。
—根据提示在浏览器中完成设备登录。

核心工作流程1. **明确意图** -议程，行动项目，文件查找，人员搜索，风险总结等。
2. **制作精确的提示-包括时间框架，来源或主题（例如，“总结团队今天在#eng上的帖子”）。
3. **运行命令** -`workiq ask --question "<prompt>"`（如果需要，使用`-q`作为简写）。
4. **监控执行** -长答案可能流；等待响应完成后再发出其他请求。
5. **总结和编辑** -突出见解，注意conflicts/tasks，避免粘贴原始链接，除非需要。
6. **提供后续跟进-预留时间，起草笔记，更深入的询问等。

##命令参考

|命令|用途|| --------------------------------- | ------------------------------------------------------------- |
|`workiq --help`|显示全局选项。|
|`workiq version`|显示安装版本。|
|`workiq accept-eula`|接受许可（第一次使用）。|
|`workiq ask`|交互模式。|
提出一个特定的问题（如果愿意，可以使用`-q`简写）。|
|`workiq ask -t <tenant> -q "..."`|指定一个租户。|
|`workiq mcp`|启动MCP studio服务器（将WorkIQ工具公开给其他代理）。|

提示模式-日程安排：“我明天的日程安排是什么？”
-行动项目：“总结今天客户同步的后续行动。”
-文档：“列出关于Contoso FY26路线图的ppt。”
沟通：“我的经理是怎么说截止日期的？”
-洞察：“在过去三次会议中出现了哪些阻碍因素？”
-计划：“建议周二下午集中精力。”

##响应指南

保持摘要简洁（2-3句话），写明负载、优先级、障碍和可选的后续步骤。
—除非用户特别需要链接，否则一般使用meetings/documents。
-提及WorkIQ是否可以继续（例如，“如果需要，WorkIQ可以显示星期四”）。
-绘制WorkIQ的建议行动，以清除报价（阻止时间，发送跟进，请求记录，运行更深层次的查询）。

最佳实践-选择窄提示以减少噪音；如果需要，运行多个查询。
-在回应之前，将输出逻辑地结合起来（议程+冲突+行动项目）。
-尊重隐私：除非明确要求，否则不要公开与会者名单或机密片段。
-记录运行了哪些命令，以便将来的步骤可以引用它们（“询问WorkIQ议程+冲突”）。
—当另一个agent/workflow需要工具直接访问时，使用MCP模式（`workiq mcp`）。

# #故障排除- **缺少命令行** -通过npm安装或确保设置了PATH；如果不可用，通知用户。
- **Consent/autherrors** - admin授予权限或设备登录完成后重新运行命令。
**Long/incomplete输出** -重新运行细化范围或请求特定的数据片（每个day/project/person）。
- **命令挂起** -取消在终端上运行的命令（例如，用Ctrl+C）或重新启动Copilot CLI会话，然后重试；确保完成浏览器登录。

提供的后续行动

-块focus/overflow保持在建议的时间。
-草案reschedule/decline消息引用WorkIQ指南。
-要求重叠会议的录音或摘要。
-将操作项捕获到任务跟踪器中。
-运行额外的WorkIQ查询（按项目，利益相关者，时间范围）进行更深入的分析。