# Power Platform Architect Plugin
！(旗帜)(https://i.imgur.com/rIJLfiL.png)GitHub Copilot的插件，作为微软Power平台的高级解决方案架构师。给它业务需求、用例描述，甚至是原始的会议记录，它就会生成一个定制的技术体系结构，包括组件建议和可选的Mermaid.js图。

# #安装```bash
copilot plugin install power-platform-architect@awesome-copilot
```
# #演示
*点击下面的图片快速演示这个代理技能！*

[![链接](https://i.imgur.com/UnImFhl.png)] (https://youtu.be/tn4jEpZ6jiw)

包含的内容
# # #技能
|技能|描述|| --- | --- |
|`power-platform-architect`|根据业务需求生成功能强大的Power Platform体系结构|

##如何工作
该技能引导代理完成一个结构化的、多阶段的过程（尽管输出是无缝地呈现给用户的）：1. **需求分析** -扫描提供的材料，包括利益相关者、数据源、安全需求和功能要求。记录当前的（“现有的”）流程，并确定摩擦点。
2. **后续问题** -座席提出澄清性问题来填补空白（例如，“这是针对移动现场工作人员还是桌面后台用户？”，“是什么触发了这个过程？”）。如果用户不能回答，它会做出合理的假设。
3. **组件推荐** -只选择在解决方案中真正起作用的Power Platform组件，并解释每个组件所起的作用。它遵循一个内置的决策框架（例如，外部访问→Power Pages、数据存储→Dataverse、会话界面→Copilot Studio）。
4. 架构叙述——提供面向业务流程的架构建议，讲述数据如何流经系统的“故事”哪些组件处理每个步骤，哪些用户受众在每个点进行交互。
5. **架构图（可选）** -根据要求，生成可视化架构的Mermaid.js图，将其保存到`.md`文件，并引导用户到[mermaid.ai/live/edit]（https://mermaid.ai/live/edit）来渲染它。你所要做的就是：**给出一个问题陈述**！你甚至可以提供一份会议记录，其中描述了problem/need：

！(例子)(https://i.imgur.com/IH1JsPZ.jpeg)

该技能涵盖了完整的Power Platform生态系统：**Power Apps**（画布，模型驱动，代码应用程序），**Power Pages**, **Copilot Studio**, **Power automation **（云和桌面流），**AI Builder**, **Dataverse**, **Power BI**， **Connectors**和** gateway **。

##示例提示
*“回顾一下我们发现环节的文字记录，告诉我如何构建它。
- *“我应该为这个人力资源入职用例使用哪些电源平台组件？”*
- *“为连接到SQL并使用审批流程的Power Apps解决方案生成架构图。

示例输出架构图（在[mermaid]（https://mermaid.js.org/）中渲染）
！(例子)(https://i.imgur.com/eR1Og3W.png)

输出架构摘要```
Solution Architecture — End-to-End Process

1. Application Submission (Residents & Contractors → Power Pages)

Residents and solar contractors visit the Evergreen County Solar Permit Portal (Power Pages). The portal presents a
guided application form with required fields, document upload slots (site plan, electrical diagrams, signed
checklist), and fee acknowledgment. Built-in form validation prevents submission if mandatory fields are blank or
required attachments are missing — this is the first line of defense against incomplete applications.

For walk-in or mailed applications, Marcus's team enters the data directly into the Model-Driven App, which enforces
the same required-field rules.

All submitted applications land in Dataverse with a status of Submitted.

2. Automated Completeness Check (Power Automate + AI Builder)

Upon submission, a Power Automate cloud flow (automated trigger: new record created) fires immediately. It performs
a programmatic completeness check — verifying all required attachments are present, fee acknowledgment is recorded,
and applicant details are complete.

For uploaded documents, AI Builder's Document Processing model scans the site plan and signed forms to verify that
signature fields are not blank and key data areas are populated. This catches the subtle defects Marcus described —
"referenced but not included" attachments and illegible or unsigned documents.

 - If complete: The permit status advances to Under Review and the flow routes it to the assigned plan reviewer 
(Jim's team).
 - If incomplete: The status is set to Incomplete, and Power Automate sends an automated email notification to the 
applicant via the Outlook connector detailing exactly what's missing. The applicant can log back into the Power 
Pages portal to upload corrections. No staff time is consumed.

3. Plan Review & Approval (Jim's Team → Model-Driven App)

The assigned plan reviewer opens the permit in the Model-Driven App, which surfaces all applicant data, documents,
and the AI validation results in a single view. The reviewer evaluates the application and either:

 - Approves → Power Automate advances the status to Approved – Pending Inspection and notifies the applicant via 
email that their permit is approved and an inspection will be scheduled.
 - Requests Revisions → Status set to Revisions Requested, the applicant is emailed with specific feedback, and they
 resubmit through the portal.
 - Denies → Status set to Denied with documented reasoning; applicant is notified.

4. Inspection Scheduling & Field Work (Sarah's Team → Canvas App Mobile)

Once a permit reaches Approved – Pending Inspection, Marcus's team schedules an inspection date via the Model-Driven
App. The applicant is notified of the date through an automated email.

Before leaving the office, Sarah opens the Canvas App on her phone/tablet and reviews her day's inspection queue.
Each permit shows its live status — if a fee issue surfaced or the applicant requested a reschedule, Sarah sees it
immediately and can reroute to a ready site. No more wasted 40-minute drives.

On-site, Sarah uses the Canvas App to:

 - Complete a structured inspection checklist (roof mounts, junction boxes, conduit, serial plates)
 - Capture photos directly through the app — each photo is automatically linked to the permit record in Dataverse at
 the moment it's taken (no more manual matching from a memory card)
 - Record pass/fail results and notes

Results sync to Dataverse in real time. The moment Sarah submits, the office has the inspection outcome — not days
later.

5. Permit Issuance or Corrective Action (Power Automate)

When Sarah submits inspection results:

 - Pass → Power Automate sets status to Permit Issued, generates a confirmation, and notifies the applicant that 
their solar installation is approved.
 - Fail → Status set to Inspection Failed – Corrections Required, with Sarah's notes and photos attached. The 
applicant is notified of required corrections and can schedule a re-inspection through the portal.

6. Self-Service Status Tracking (Residents & Contractors → Power Pages)

At any point in this process, residents and contractors can log into the Power Pages portal and see their permit's
current status, what stage it's in, whether payment has been recorded, and what the next step is. This directly
addresses the top three phone questions Marcus cited:

 1. ✅ "Did you receive my check?" → Payment status visible on portal
 2. ✅ "What is the status of my permit?" → Live stage tracking
 3. ✅ "When is the inspector coming?" → Scheduled date displayed

7. Live Analytics & Audit Readiness (Elena & Jim → Power BI)

A Power BI dashboard connects directly to Dataverse and provides:

 - Time-to-Permit metrics (average, median, by stage)
 - Backlog heatmap — how many permits are sitting in each stage right now
 - Inspection throughput — inspections completed per day/week, pass/fail rates
 - "Green Energy" permit counts by quarter — the exact metric the County Auditor requests
 - Incomplete application rate trends — tracking whether the portal validation is reducing the 40% defect rate

Elena can answer Board and Auditor questions on demand with live data — no staff time diverted to manual counting.

-------------------------------------------------------------------------------------------------------------------

Summary

This architecture replaces the disconnected paper-based process with a single integrated data backbone (Dataverse)
that every stakeholder touches through the right interface for their role:

┌──────────────────────────┬─────────────────────┬──────────────────────────┐
│ Audience                 │ Interface           │ Purpose                  │
├──────────────────────────┼─────────────────────┼──────────────────────────┤
│ Residents & Contractors  │ Power Pages Portal  │ Submit, track, resubmit  │
├──────────────────────────┼─────────────────────┼──────────────────────────┤
│ Marcus (Intake)          │ Model-Driven App    │ Review, schedule, manage │
├──────────────────────────┼─────────────────────┼──────────────────────────┤
│ Jim (Planning/Review)    │ Model-Driven App    │ Approve/reject permits   │
├──────────────────────────┼─────────────────────┼──────────────────────────┤
│ Sarah (Field Inspectors) │ Canvas App (Mobile) │ Inspect, capture, submit │
├──────────────────────────┼─────────────────────┼──────────────────────────┤
│ Elena & Jim (Leadership) │ Power BI Dashboards │ Monitor, report, audit   │
└──────────────────────────┴─────────────────────┴──────────────────────────┘

The expected impact directly addresses Elena's four strategic needs and Jim's prediction: cut the backlog in half
without hiring a single new person.
```
# #源
由微软高级人工智能解决方案工程师[Tim Hanewich]（https://timh.ai）创建。

# #许可证
麻省理工学院