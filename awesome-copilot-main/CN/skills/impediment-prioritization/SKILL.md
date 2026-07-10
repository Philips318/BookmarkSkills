---
name: impediment-prioritization
description: 'Ranks any list of impediments and their countermeasures using a value-stream scoring model (ROI, Cost to Implement, Ease of Deployment, Risk Factor) and a fixed prioritization formula. Use when someone asks to prioritize, rank, sequence, or triage impediments, countermeasures, remediation items, risks, findings, gaps, action items, or backlog entries; or mentions value-stream prioritization, A3 / lean countermeasure ranking, ROI vs. effort scoring, or building a remediation / improvement backlog. Works with GHQR findings, audit results, retrospective action items, risk registers, architecture review gaps, or any free-form `{impediment, countermeasure}` list.'
license: MIT
metadata:
  author: ajenns
  version: "2.0.0"
  created: "2026-04-19"
  updated: "2026-04-21"
  framework: value-stream-prioritization
  domain: general
---
#障碍优先级技能

一种对障碍进行排序及其对策的领域不可知论技巧。适用于任何`{impediment, countermeasure}`列表- GHQR发现，审计结果，复古行动项，风险登记，架构审查差距等。

##何时激活

当用户：
-要求对障碍、差距、风险、发现或补救项目进行优先级排序、排序、排序或分类
-提供一系列障碍和建议的对策（或要求您针对一系列问题提出对策）
-对任何改进/补救积压询问“我们应该首先修复什么”
-提到价值流优先级，A3对策，roi / effort，或精益障碍排序

# #输入

可接受的输入：`{impediment, countermeasure}`对的列表。来源包括（非详尽的）：

|源|映射到障碍|映射到对策||--------|---------------------|-------------------------|
b| GHQR /健康检查结果|发现或差距（状态≠预期）|建议/期望值|
|审计结果|不符合|补救措施|
|回顾|“哪里出了问题”项目|商定的改进|
|风险登记|风险|缓解|
|架构审查|差距与目标状态|建议更改|
|用户自由格式列表|问题声明|建议修复|

* *规则:* *
-每个障碍一个对策。如果输入建议使用多种补救路径，则选择主要路径并在基本原理中注明替代路径—不要针对相同的障碍发出多行。
-在得分前重复折叠。
-如果源链接/引用是可用的，将其附加到对策。
—如果源上存在置信度，则将其表面显示为可选的`Confidence`列。

评分标准（1-10分）根据所有四个标准对每个障碍的对策进行评分。请参阅[references/scoring-rubric.md](./references/scoring-rubric.md)，了解跨多个领域（平台工程、安全性、SRE、应用程序开发、治理）的1 / 5 / 10级锚定示例。

|标准|分级|定义||-----------|-------|------------|
| **投资回报率(ROI)** | 1 =低，10 =高|该步骤和整个价值流的对策所带来的效率增益。不仅仅是财务方面的——重量吞吐量、周期时间的减少、缺陷的消除、用户/开发人员的体验，以及遵从性的提升。|
| **实施成本** | 1 =廉价，10 =非常昂贵|人力资本（工资+所需人员的时间）加上实施对策所需的任何采购、许可证或基础设施。|
| **易于部署** | 1 =极其困难，10 =非常容易|修复工作需要实际部署端到端的对策。反映了技术复杂性、变更管理负担和回滚风险。|
| **风险因素** | 1 =低风险，10 =极高风险|如果对策出错、停滞或延迟，对整体价值流的影响加权风险。|每个分数都必须附有一行理由。当分数是估计值而不是从明确的数据中得出时，用`(estimated)`标记基本原理。

# #公式```
Priority = ((ROI * (10 / Cost)) + (Ease * (10 / Risk))) / 2
```
—理论范围：**1→100**。典型积压的实际范围：~1→100。
-规模最小值`1`保证成本和风险永远不会为零（没有除以零）。
—高优先级= do first。
-边界检查：
- ROI=10，成本=1，易用性=10，风险=1→`((10*10)+(10*10))/2 = 100`- ROI=1，成本=10，易用性=1，风险=10→`((1*1)+(1*1))/2 = 1`一字不差地使用这个公式。不要重新称重、标准化或替代。

##方法（代理程序）1. **摄入**障碍列表。确认1:1的障碍-对抗映射；重复的崩溃。
2. **确认每个障碍的对策**。更喜欢有文档的领域最佳实践。如果有公共/权威链接，请引用。
3. **使用标题对所有四个标准进行评分。为每个标准写一行基本原理。
4. **使用公式计算**优先级。四舍五入到小数点后一位。
5. **按优先级降序排序**行。从1开始分配Rank。
6. **渲染**输出表（见下文）。
7. 用一个简短的“为什么要先行动”的段落列出前3个障碍。
8. **可选标签**：如果工作流需要所有权标志（例如，`[CSA Action Required]`vs.`[Customer Self-Service]`用于GHQR/PAK，或`[Owner: Team X]`/`[Self-Service]`用于内部待办事项），将它们包含在排名最高的项目上。如果没有要求，请跳过。

##输出模板```markdown
## Prioritized Impediments

**Scoring:** ROI (1 low → 10 high), Cost (1 cheap → 10 expensive), Ease (1 hard → 10 easy), Risk (1 low → 10 high).
**Formula:** `Priority = ((ROI * (10/Cost)) + (Ease * (10/Risk))) / 2`

| Rank | Impediment | Countermeasure | ROI | Cost | Ease | Risk | Priority | Rationale |
|------|------------|----------------|-----|------|------|------|----------|-----------|
| 1 | [gap] | [action + link] | [n] | [n] | [n] | [n] | [n.n] | ROI: …<br>Cost: …<br>Ease: …<br>Risk: … |

### Top 3 — Act First
1. **[Impediment]** — [why it wins on the formula + optional ownership tag]
2. …
3. …
```
工作示例（GitHub Enterprise采用）：**

b|等级b|障碍b|对策b|投资回报率|成本|缓解|风险|优先级|基本原理||------|------------|----------------|-----|------|------|------|----------|-----------|
| 1 | 2FA未在组织级别强制执行|强制执行组织范围的2FA ([docs](https://docs.github.com/en/organizations/keeping-your-organization-secure/setting-up-two-factor-authentication/requiring-two-factor-authentication-in-your-organization)) | 9 | 2 | 8 | 2 | 42.5 | ROI：删除广泛的凭证妥协类<br>cost: admin toggle +成员comms<br>Ease：单个组织设置，成员重新注册<br>risk：低-可以在宽限期|
| 2 |秘密扫描关闭|开启秘密扫描+推送保护org-wide ([docs](https://docs.github.com/en/code-security/secret-scanning/about-secret-scanning)) | 8 | 3 | 7 | 3 | 25.0 | ROI：捕获泄漏的信用premerge<br>cost：未捆绑的GHAS座（估计）<br>Ease: org-level default<br>Risk：推送保护可能会阻止合法提交；每个回购|阶段
b| |关键回购没有CODEOWNERS |将CODEOWNERS添加到前20个回购（[docs](https://docs.github.com/en/repositories/managing-your-repositorys-settings-and-features/customizing-your-repository/about-code-owners)） | 6 | 4 | 6 | 4 | 15.0 | ROI：目标审查覆盖率e<br>成本：团队定义所有者的时间（估计）<br>Ease：文件级更改，但需要所有者购买in<br>风险：审查瓶颈如果所有者不ndersized |**工作示例（一般回顾性行动项目）：**

|等级|阻碍|对策| ROI |成本|缓解|风险|优先级||------|------------|----------------|-----|------|------|------|----------|
| 1 |片状测试套件块每日部署|隔离前10个片状测试+添加重试策略bbb9 | 2 | 8 | 2 | 42.5 |
|支付服务没有随叫随到的运行手册|过去三次事件的运行手册草案| 7 | 3 | 8 | 2 | 31.7 |
| 3 |手动发布说明取2h/release|通过CI从常规提交生成| | 4 | 5 | 3 | 15.8 |

##假设&护栏-分数是根据标题和任何可用的来源/引用来估计的。用`(estimated)`明确地标记估计的基本原理。
永远不要捏造上下文（团队规模、预算、工具库存、组织约束）。如有需要，询问用户或按预估评分。
-最终排名是一个建议-在承诺执行计划之前，应该与负责任的团队/所有者一起审查。
-默认为只读-此技能不执行补救它生成下游消费的排序列表。

下游集成（可选）

由该技能生成的排名表是可交付的。将其连接到您的工作流所需的任何下游工件（Jira史诗、ADR、OKR待办事项、事件审查、健康检查报告等）。此技能不依赖于任何兄弟技能或外部模板。