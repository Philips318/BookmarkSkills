#爆炸半径计算器

用于量化有多少人、记录和系统将受到正在分析的代码库中的数据泄露的影响的公式、评分矩阵和估计启发式。

---

##核心爆炸半径公式```
Blast Radius Score (BRS) = Tier_Weight × Exposure_Likelihood × Population_Scale × Completeness_Factor × Context_Multiplier
```
* *分数范围:* *
- 0-25: **低** -有限曝光，很少记录
- 26-50: **中等** -有意义的曝光，重点人群
- 51-75: **高** -显著暴露，广泛的监管后果
- 76-100: **临界** -灾难性暴露，需要立即采取行动

---

因素1：等级权重(T)

根据`data-classification.md`的数据分类层：

|分级|标签|权重||------|-------|--------|
| T1 |灾难性| 5.0 |
| T2 |紧急| 4.0 |
| T3 |高| 3.0 |
| T4 |升高| 2.0 |
| T5 |标准| 1.0 |

**规则：**当同一曝光向量中存在多个层时，使用**最高的**层权重。

**聚合提升：**如果来自不同层的3+字段一起暴露，添加+0.5至最高层权重（聚合攻击风险）。

---

因素2：暴露可能性(E)

在现实的入侵场景中，这个向量被利用的可能性有多大？

|可能性评分|标签|标准||-----------------|-------|---------|
| 1.0 | **某些** |数据今天是公开访问的（不需要授权）|
| 0.9 | **接近确定** |验证旁路是微不足道的（例如，顺序id上的IDOR， JWT验证失效）|
| 0.8 | **很有可能** |需要认证，但缺少此特定端点；或者大多数工程师可以访问的日志中泄露的数据|
| 0.7 | **可能** |需要授权，但访问范围过广（所有用户都可以看到所有数据）；缺少域级访问控制|
| 0.6 | **中等** |需要权限升级或与其他错误链接；内部系统具有广泛的开发人员访问|
| 0.5 | **可能** |需要大量攻击者的努力，但不需要深度防御；DB可从开发环境|访问
| 0.3 | **不可能** |多个安全控制到位；但是控件没有经过代码库审查|的验证
| 0.1 | **远程** |强大的深度防御：加密，字段屏蔽，适当的authz，速率1限流、异常检测均呈现|---

因素3：人口规模(P)

将受影响记录的估计数量标准化为0-1级。

估计记录计数

步骤1：在代码库中寻找显式信号```
# Strong signals (use these if found):
- README mentions user count ("serves 5M users")
- Seeder/fixture files with record counts
- Migration comments ("adding index for 50K users")
- Analytics dashboards or monitoring configs mentioning scale
- Infrastructure configs (DB instance size implies scale):
  - db.t3.micro → < 10K active users
  - db.r5.large → 10K–500K users
  - db.r5.4xlarge / Aurora Serverless → > 500K users

# Medium signals:
- App category (SaaS product → higher, internal tool → lower)
- Multi-tenant vs. single-tenant architecture
- Presence of sharding or partitioning in DB schema

# Weak signals:
- Tech stack alone (no reliable correlation to user count)
```
**步骤2：当没有发现信号时应用默认估计**

|应用类型|保守估计|典型估计||-----------------|----------------------|-----------------|
|企业内部工具| 100 - 1000 | 500 |
| B2B SaaS (small/startup) | 1000 - 10000 | 5000 |
| B2B SaaS（已建立）| 10000 - 100000 | 50000 |
| B2C应用（消费类创业）| 1 - 10万| 5万|
| B2C应用（成长期）| 10 - 100万| 50万|
| B2C应用（规模）| 100 - 1亿| 1000万|
|医疗保健系统| 1,000-100,000 | 20,000 |
|金融服务| 5000 - 50万| 5万|
|政府/公共部门| 10,000-10,000,000 | 1,000,000 |

**总是说明所使用的假设

人口规模得分(P)

|有风险记录|评分||----------------|-------|
| < 100 | 0.1 |
| 100 - 1000 | 0.2 |
| 1000 - 10000 | 0.3 |
|0 10000 - 50000 | 0.4 |
| 5 - 10万| 0.5 |
| 10 - 50万| 0.6 |
| 50 - 1,000,000 | 0.7 |
| 1m-10m | 0.8 |
|0米- 100米| 0.9 |
| > 100m | 1.0 |

---

因素4：完备性因素(C)

对于攻击者来说，暴露的数据是多少？

|因子|评分|描述||--------|-------|-------------|
| **完整个人资料** | 1.0 |完整的身份记录(姓名+电子邮件+电话+地址+敏感字段
| **Partial + Joinable** | 0.9 |部分数据，但其他表可以加入来完成；同样的漏洞给了攻击者连接键|
| **电子邮件+ PII** | 0.8 |电子邮件地址加1+敏感字段-足以针对网络钓鱼+利用|
| **仅敏感字段** | 0.7 |仅敏感字段（SSN, health, financial）没有联系信息-仍然非常严重|
| **只联系** | 0.5 |只发邮件/电话-允许垃圾邮件，网络钓鱼，但不会立即伤害|
| **碎片化的** | 0.3 |没有上下文的字段，如果没有额外的数据，则无法重新识别
| **匿名** | 0.1 |正确匿名-重新识别需要重要的外部数据链接|

---

因素5：情境乘数(M)将这些乘数应用于特定上下文的最终分数：

|上下文|乘数|原理||---------|-----------|-----------|
|儿童数据存在（COPPA / GDPR Art 8） | × 2.0 |全球最高法律风险|
|健康记录（HIPAA / GDPR特殊类别）| × 1.8 |特殊类别数据，民事+刑事暴露|
|生物识别数据（GDPR Art 9, BIPA in Illinois） | × 1.8 |不可变数据-在违反|后不能“改变”
|金融账户凭证| × 1.7 |直接金融盗窃可能|
|政府身份证（SSN，护照）| × 1.6 |身份盗窃持续数年|
|性取向/宗教/政治观点| × 1.6 | GDPR特殊类别，歧视风险|
|医疗保健提供商持有的数据| × 1.5 | HIPAA业务伙伴暴露|
|云区域内与用户管辖范围不匹配的数据| × 1.3 |跨境传输违规（GDPR第五章）|
|Backup/archive存储（经常被遗忘）| × 1.2 |泄露遏制中经常遗漏的备份|

---##爆炸半径分数计算示例

示例1：电子商务结帐系统

**暴露向量：** API端点`/api/users/{id}/payment-methods`-无所有权检查（IDOR）
—分级：T2（卡后4位+账单地址）= 4.0
-暴露可能性：0.9（连续id的IDOR，几乎确定被利用）
-人口规模：10万用户= 0.6
—完整性：部分配置文件+可连接到用户表= 0.9
—上下文乘数：支付数据= 1.7```
BRS = 4.0 × 0.9 × 0.6 × 0.9 × 1.7 = 3.30 (raw) → normalized to 66/100 → HIGH
```
例2：内部人力资源工具

**曝光向量：**员工表通过`/api/employees`对所有公司用户可见
- Tier: T2（工资+家庭住址+社会保障号）= 5.0（社会保障号为T1）
-暴露可能性：0.7（需要认证，但没有RBAC；任何员工都可以看到所有）
—人口规模：2000名员工= 0.3
—完整性：Full profile = 1.0
—上下文乘数：政府id (SSN) = 1.6```
BRS = 5.0 × 0.7 × 0.3 × 1.0 × 1.6 = 1.68 (raw) → normalized to 34/100 → MEDIUM
```
然而，**财务影响**超过了这里的得分，因为社会安全系数是1级。无论得分如何，标记为高。

---

##评分归一化

原始公式输出的范围通常为0-8。归一化为0-100：```
Normalized_BRS = min(100, (raw_BRS / 8.0) × 100)
```
---

##爆炸半径汇总表（每个暴露向量）

在报告时使用这种格式：```markdown
| # | Exposure Vector | Tier | Likelihood | Pop. at Risk | BRS | Severity | Jurisdiction |
|---|----------------|------|-----------|-------------|-----|----------|--------------|
| 1 | /api/users endpoint - SSN returned in response | T1 | 0.9 | 50K | 87 | CRITICAL | GDPR, CCPA |
| 2 | Logs contain plaintext emails | T3 | 0.6 | 50K | 45 | MEDIUM | GDPR |
| 3 | Redis cache stores full user objects | T2 | 0.5 | 50K | 38 | MEDIUM | GDPR, CCPA |
| 4 | S3 bucket - public read on user avatars | T4 | 1.0 | 50K | 28 | LOW | - |
```
---

总组织爆炸半径

对所有曝光向量打分后，计算：

**最大同时暴露（MSE）：**如果单个攻击者获得广泛的数据库访问（最坏的情况），可能受到影响的唯一个体的数量。这是监管报告中使用的数字。

**预期泄露暴露（EBE）：**基于最可能的攻击向量的典型暴露（最高可能性的发现，而不是最高影响的一个）。

**监管触发计数：**触发的不同监管制度的数量（每个制度都有自己的通知义务和精细公式）。```markdown
## Organizational Blast Radius Summary

| Metric | Value |
|--------|-------|
| Maximum records at risk | [number] |
| Users with Tier 1 data | [number] |
| Users with Tier 2 data | [number] |
| Users with Tier 3+ data | [number] |
| Regulations triggered | GDPR, CCPA, [others] |
| Worst-case BRS | [score] |
| Most likely attack vector | [description] |
| Time to detect (estimated) | [industry avg: 194 days if no SIEM] |
| Time to contain (estimated) | [industry avg: 73 days] |
```
---

违规成本基准（IBM Data - Verify Annual Edition）

当没有具体的成本数据可用时，使用这些。以下数字来自**IBM 2024版**。IBM每年在https://www.ibm.com/reports/data-breach发布一个新版本——2025年的报告显示全球平均成本下降了约9%。

| Metric |值（IBM 2024） ||--------|------------------|
每次违规的全球平均成本为488万美元
每条记录的平均成本（医疗保健）| $408 USD |
每条记录的平均成本（财务）$231 USD |
每条记录的平均成本（各行业平均值）165美元
识别漏洞的平均时间为194天
控制违约的平均时间为73天
|违规持续200天的成本溢价| +比平均|高出102万美元
大型数据泄露（超过100万记录）造成的损失为1300万至6500万美元
从事件响应计划中减少成本| - 23.2万美元|AI/ML安全部署成本降低| - 222万美元|
员工培训成本降低| - 25.8万美元|

来源：IBM《数据泄露成本报告2024》。将这些作为基准，而不是保证。在发布新版本时更新该表。