---
name: brag-sheet
description: >
  Turn vague "what did I do?" into evidence-backed impact statements for performance
  reviews, self-reviews, promotion packets, and weekly updates. Uniquely mines Copilot
  CLI session logs to reconstruct forgotten work, plus git commits and GitHub PRs.
  Enforces a 3-part impact contract (action → result → evidence). Works standalone
  with zero dependencies. Trigger for: "brag", "log work", "what did I do",
  "backfill my work history", "performance review", "self-review", "self assessment",
  "write impact statement", "review prep", "promo packet", "promotion case",
  "weekly update", "status report", "accomplishments", "what did I ship",
  "I forgot to log my work", "summarize my work", "track my wins",
  "what should I highlight", "end of half", "career growth", "work journal",
  or any request to document, summarize, or organize work accomplishments.
license: MIT
compatibility: 'Cross-platform (Windows, macOS, Linux). Works with any GitHub Copilot CLI session. Optional: git, gh CLI.'
metadata:
  version: "1.1"
argument-hint: 'Optional: time range ("last 2 weeks", "this half"), category ("infrastructure"), "backfill", or "review prep"'
---
#吹牛表-工作影响作家

将工程工作转化为有证据支持的影响声明，用于绩效评估、自我评估、促销包和每周更新。独特地挖掘Copilot CLI会话日志，git历史记录和pr来重建被遗忘的工作。

用于：“吹牛”、“记录工作”、“我做了什么”、“回填”、“绩效评估”、“自我评估”、“促销包”、“每周更新”、“状态报告”、“撰写影响声明”、“我发布了什么”、“我忘记记录工作”、“审查准备”、“成就”
不要用于：项目管理，冲刺计划，时间跟踪，票证创建

##快速入门

|用户想要…|模式|输出||---------------|------|--------|
|记录一个完成| **捕获** | 1影响第一项|
“我上周做了什么？”| **回填** |按周分组的条目，从git/PRs/sessions|挖掘
|准备审查或推广| **审查包** |按影响主题分组的参赛作品+ STAR叙事|

代理行为规则1. **DO**在扫描源前确认时间范围和范围。不要假设“上周”——要问。
2. **DO**在选择工作流之前检查哪些工具可用（`save_to_brag_sheet`,`git`,`gh`）。
3. **DO**始终包括所有三个部分：行动→结果→证据。如果没有证据，就写`(evidence needed)`——永远不要悄悄省略。
4. **DO**在保存之前向用户显示起草的条目。未经确认绝不自动保存。
5. **DO**组相关提交到单个条目。同一特性上的10次提交=一个条目。
6. **DO**保留用户的声音。重新构思影响，但不要虚构成就或扩大范围。
7. **不要**捏造指标、团队规模或影响数字。如果用户没有提供一个数字，就不要发明一个。
8. **不要**为用户只口头描述而不验证的工作写条目。问：“这艘船吗？”有没有我可以参考的公关或文件？”9. **不要**在扫描完成之前跳过回填扫描步骤或草稿条目。
10. **不要**用琐碎的条目填充弱周期。诚实的差距比夸大的绒毛要好。##输入格式

每个条目都使用影响优先帧，包含三个必要部分：```
Did [action] → [result/impact] → [evidence]
```
**不要输出条目，除非它包含所有三个部分。**如果缺少证据，请要求提供或标记为“（需要证据）”。

# # #反模式

|❌不要|✅要||---------|--------------|
固定令牌刷新竞争条件→消除401影响12%的API调用→PR #247" |
“|”在Grafana中构建延迟仪表板→随叫随到检测P95峰值<2分钟→部署到prod“|”
创造一个指标：“节省40%的工作时间”问：“你有一个粗略的估计，还是我应该保持这个定性？”|
|每次提交一个表项|组相关的提交到一个表项中，具有最高的影响帧|
被动语态：“管道得到了改进”|主动语态：“构建了CI矩阵→在发布之前捕获了仅限windows的错误”|
|列出使用的技术|说明结果：“将4个服务迁移到IaC→部署时间45min→8min”|
|标记`(evidence needed)`，供用户填写|

证据阶梯

并不是每个条目都需要度量。使用最有力的证据：

|强度|证据类型|示例||----------|--------------|---------|
|🥇最佳|量化度量|“将P95延迟从800ms减少到120ms”|
|🥈强大的| PR，提交，或文档链接|“PR #312，设计文档在wiki”|
|🥉良好|可观察到的结果|“解锁X团队”，“解决Sev2事件Y”|
|✅可接受的|定性+上下文|“减少随叫随到轮换的辛劳-参见更新的手册”|
|⚠️弱|活动仅|“工作在验证”-重新帧或标记`(evidence needed)`|

永远不要发明一个指标来填补空白。有背景的定性证据胜过捏造的数字。

# #类别

| ID |表情符号|用于||----|-------|---------|
|`pr`|🚀|合并pr，发布特性|
|`bugfix`|🐛|错误修复，事件补丁|
|`infrastructure`| <s:1>️|基础设施、部署、迁移|
|`investigation`|🔍|根本原因分析，调试|
|`collaboration`|🤝|评论，指导，设计讨论|
|`tooling`|🔧|开发工具、脚本、自动化|
|`oncall`|🚨|事件响应，随叫随到赢得|
|`design`|📐|设计文档，架构决策|
|`documentation`|📝|文档，手册，指南|

如何帮助用户

遵循这个决策树：

1. **如果已有`save_to_brag_sheet`工具**→直接使用扩展工具（`save_to_brag_sheet`,`review_brag_sheet`,`generate_work_log`）。除非确认这些工具可用，否则不要引用或尝试调用它们。

2. **如果git或gh CLI可用**→从提交和pr中回填（参见下面的回填部分）

3. **否则**→引导性访谈：“你做了什么？”，“谁受益了？”，“有什么证据？”对于每个条目，请浏览：**What**（可交付内容）→**Why**（谁受益）→**Evidence** （PR，指标，链接）。输出格式化的标记，用户可以粘贴到审查文档中。

##回填工作流

当用户询问“我上周做了什么”或“回填我的历史记录”时：

**按顺序执行这些步骤。在扫描完成之前不要草稿。**

###步骤1：扫描可用资源

检查可用的资源，然后挖掘每个资源：```bash
git --version 2>/dev/null         # for commit mining
gh --version 2>/dev/null          # for PR mining
ls ~/.copilot/session-state/ 2>/dev/null  # Copilot session logs
```
**Git提交** -用户在当前仓库中最近的提交：```bash
git log --author="$(git config user.email)" --since="2 weeks ago" \
  --pretty=format:'%h|%ad|%s' --date=short --no-merges
```
**PR历史** -跨repos合并PR：```bash
gh pr list --author @me --state merged --limit 20 \
  --json number,title,repository,mergedAt
```
**副驾驶会话记录**（该技能独有）：
—路径：`~/.copilot/session-state/<session-id>/workspace.yaml`—读取字段：`summary`，`cwd`,`repository`,`branch`-跳过没有`summary`字段的会话
—注意：并非所有机器上都存在此目录

如果这些资料都不具备，那就回到指导式面试。

步骤2：将相关工作分组

将相关信号集中到一个条目中：
-相同的PR +其提交→1项
-在3天内多次提交同一个file/feature→1个条目
副驾驶会话引用相同的repo +分支→合并到PR条目（如果存在的话）

###步骤3：起草参赛作品

为每个组编写影响优先条目。指定类别。

###步骤4：呈现并完善

向用户显示所有起草的条目。根据反馈进行调整。

步骤5：输出

按周分组的降价格式：```markdown
## Week of 2025-04-14

### 🚀 PRs & Features
- **Migrated auth service to managed identity** → eliminated 3 secret rotation incidents/quarter → PR #312

### 🏗️ Infrastructure
- **Built CI pipeline for copilot-brag-sheet** → 107 tests across 3 OSes × 3 Node versions → shipped v1.0.0
```
绩效评估准备

当用户准备进行绩效考核（Connect， annual review等）时：

# # #结构

1. **收集** -从工作日志中收集条目（或使用上述工作流进行回填）
2. **选择** -选择前3-5个影响最大的项目
3. **将**条目改写为三部分：
- **我做了什么** -具体的行动
- **为什么重要** -谁受益，什么改变了
- **证明** - PR号码，度量delta，仪表板链接，客户结果
4. **按影响主题组织**（不按时间顺序）：
-交付成果/卓越运营
-客户/团队影响
-协作/指导/领导
-成长/学习
5. **询问差距** -如果证据缺失，提示用户：“什么指标改变了？”，“谁被解锁了？”，“PR或事件ID是什么？”

强条目vs弱条目

|✅强|❌弱||----------|--------|
结果优先，量化|活动列表（“在X上工作”）|
|绑定customer/team影响|没有受益人提到|
|包括证据（PR，度量）|无可测量结果|
|表示所有权或领导|纯粹的任务完成|

叙述格式

对于较长的叙述部分，使用STAR: **S* situation→**T**ask→**A* action→**R** result。

对于使用Connect预设的微软员工，围绕核心优先事项构建条目：交付结果、客户至上、团队合作和成长心态。

##输出合同

在完成之前，请确保：
1. 每个条目都有动作→结果→证据（如果缺少则标记为`(evidence needed)`）
2. 没有捏造的指标-只有用户提供或源验证的数据
3. 保存前显示给用户的条目
4. 明确规定的时间范围
5. 输出是带有已分配类别的可粘贴标记

# #陷阱当前版本中没有最近的提交
用户可以跨多个仓库工作。在结束之前，没有什么需要回填的：
1. 询问他们是否想扫描不同的回购或分支
2. 查看`gh pr list --author @me --state merged`的交叉回购pr
3. 回到指导性面试——并不是所有有影响力的工作都会留下git的痕迹（设计文档、事件响应、指导）。

回顾周期与git历史记录不匹配
绩效评估通常持续6-12个月。显式设置日期范围：```bash
git log --author="$(git config user.name)" --since="2024-07-01" --until="2025-01-01" --oneline
```
PR历史记录（`gh pr list --state merged`）在长时间范围内比提交日志更可靠。

用户无法量化影响
并不是每个条目都需要一个数字。参见上面的证据阶梯。可接受的证据包括PR链接、“畅通无阻的X团队”或带有上下文的定性结果。永远不要发明一个指标来填补空白。

副驾驶会话目录不存在`~/.copilot/session-state/`仅在用户运行过Copilot CLI会话时存在。不要出错-静默跳过并注意：“没有找到副驾驶会话记录；只扫描git和pr。”

“吹牛”可能有别的意思
用户可能会说“向我的团队炫耀一下这个特性”（发布公告，而不是工作条目）。如果模棱两可，请确认意图。

结对编程或合著提交
如果多个作者出现在相同的提交中，问：“我应该把它归为你的工作，共享的工作，还是跳过它？”

自动会话跟踪（可选）自动后台跟踪每一个Copilot CLI会话（文件编辑，pr创建，git动作），安装[Copilot - bragg -sheet]（https://github.com/microsoft/copilot-brag-sheet）扩展。它将`save_to_brag_sheet`、`review_brag_sheet`和`generate_work_log`工具添加到每个会话。