# GDPR参考-数据权利，问责制和治理

当你需要实现细节时加载这个文件：
用户权限端点，数据主体请求（DSR）工作流，
处理活动记录（RoPA），同意管理。

---

##用户权利实施（第15-22条）

每个权利都必须有一个经过测试的API端点或记录的后台流程
在系统运行之前。在**30个日历天内回复已核实的请求**。

|权利|文章|工程实现||---|---|---|
|访问权限| 15 |`GET /api/v1/me/data-export`-所有个人数据，JSON或CSV |
|纠正权| 16 |`PUT /api/v1/me/profile`-传播到所有下游存储|
|擦除| 17 |`DELETE /api/v1/me`-根据擦除检查表|擦除所有存储
|限制| 18 |用户记录上的`ProcessingRestricted`标志的权限；闸非必要处理|
|可移植性权利| 20 |与接入端点相同；结构化的、机器可读的（JSON） |
|反对权| 21 |合法权益处理的退出端点；荣誉立即b|
|自动化决策| 22 |公开人工审核路径+逻辑解释|

擦除检查表-必须覆盖所有商店

当调用`DELETE /api/v1/me`时，擦除管道必须擦洗：-主关系数据库（匿名或删除行）
-读取副本
-搜索索引（Elasticsearch， Azure Cognitive Search等）
-内存缓存（Redis, immemorycache）
-对象存储（S3， Azure Blob -个人资料图片，文档）
-电子邮件服务日志（Brevo， SendGrid -交付日志）
-分析平台（Mixpanel, Amplitude， GA4 -用户删除API）
—审计日志（匿名标识字段—不删除事件）
-备份（记录备份的TTL；接受备份自然过期）
- CDN边缘缓存（清除可能缓存的个人数据）
-第三方子处理器（触发其删除API或记录手动步骤）

数据导出格式（`GET /api/v1/me/data-export`）```json
{
  "exportedAt": "2025-03-30T10:00:00Z",
  "subject": {
    "id": "uuid",
    "email": "user@example.com",
    "createdAt": "2024-01-15T08:30:00Z"
  },
  "profile": { ... },
  "orders": [ ... ],
  "consents": [ ... ],
  "auditEvents": [ ... ]
}
```
-必须是机器可读的（JSON优先，CSV可接受）。
-绝对不能是PDF截图或HTML页面。
-必须包括该用户RoPA中列出的所有商店。

DSR跟踪器（后台）

实施**数据主体请求跟踪**，包括：
-传入请求日期
-请求类型（访问/更正/删除/可移植/限制/反对）
-验证状态（身份确认y/n）
-截止日期（收到日期+ 30天）
-指定处理程序
-完成日期和结果
——笔记

自动完成主要的仓库清理；记录第三方商店的手动步骤。

---

加工活动记录（RoPA）

在repo中作为版本控制的活文档（Markdown、YAML或JSON）进行维护。
更新引入处理活动的** **新特性。

每个处理活动的最小字段```yaml
- name: "User account management"
  purpose: "Create and manage user accounts for service access"
  legalBasis: "Contract (Art. 6(1)(b))"
  dataSubjects: ["Registered users"]
  personalDataCategories: ["Name", "Email", "Password hash", "IP address"]
  recipients: ["Internal engineering team", "Brevo (email delivery)"]
  retentionPeriod: "Account lifetime + 12 months"
  transfers:
    outside_eea: true
    safeguard: "Brevo — Standard Contractual Clauses (SCCs)"
  securityMeasures: ["TLS 1.3", "AES-256 at rest", "bcrypt password hashing"]
  dpia_required: false
```
法律依据选项（第6条）

|基础|何时使用||---|---|
|`Contract (6(1)(b))`|完成服务契约|所需的处理
|`Legitimate interest (6(1)(f))`|防欺诈、安全、分析（需要平衡测试）|
|`Consent (6(1)(a))`|营销，非必要的cookie，可选的分析|
税务记录，反洗钱|
|`Vital interest (6(1)(d))`|仅限紧急情况|
|`Public task (6(1)(e))`|公共权威|

---

##同意管理

# # #必须

-将同意存储为不可变的事件日志，而不是可变的布尔标志。
-记录：同意的内容、时间、隐私政策的版本、机制。
-加载分析/营销sdk **有条件地** -仅在获得同意后。
-提供一个撤销同意的机制，就像给予同意一样容易使用。

同意存储模式（最小）```sql
CREATE TABLE ConsentRecords (
    Id          UUID PRIMARY KEY,
    UserId      UUID NOT NULL,
    Purpose     VARCHAR(100) NOT NULL,   -- e.g. "marketing_emails", "analytics"
    Granted     BOOLEAN NOT NULL,
    PolicyVersion VARCHAR(20) NOT NULL,
    ConsentedAt TIMESTAMPTZ NOT NULL,
    IpAddressHash VARCHAR(64),           -- HMAC-SHA256 of anonymized IP
    UserAgent   VARCHAR(500)
);
```
###绝对不能

-绝对不能在同意复选框前打勾。
-绝对不能将营销同意与服务交付同意捆绑在一起。
-绝不能使服务准入以市场许可为条件。
-绝对不能使用深色图案（例如，“接受全部”突出，“拒绝”隐藏）。

---

子处理器管理

维护与每个新的SaaS工具或云服务一起更新的子处理器列表
这涉及到个人数据。

每个子处理器的最小字段数：

|字段|示例||---|---|
|名称| Brevo |
|服务|事务性邮件|
|传输的数据类别|邮箱地址、姓名、邮件内容|
|加工地点|欧盟（巴黎）|
| DPA签署| 2024-01-10 |
| DPA URL / reference | [link] |
| SCCs适用|N/A（基于欧盟）|

**必须**每年审查次级处理机清单，如有任何变化。
**绝对不能**允许数据流到一个新的子处理器之前，DPA被签署。

---

DPIA触发器（第35条）

在可能导致高风险的处理之前，DPIA是**强制性的**。诱因包括:-对个人有重大影响的系统和广泛的分析
-大规模处理特殊类别数据（健康、生物特征、种族出身、性取向、宗教）
-系统监控公众可进入的区域（闭路电视，位置跟踪）
-大规模处理儿童数据
-具有未知隐私影响的创新技术
—匹配或组合来自多个数据源的数据集

当有疑问时：无论如何都要执行DPIA。记录结果。