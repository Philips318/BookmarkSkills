---
name: gdpr-compliant
description: 'Apply GDPR-compliant engineering practices across your codebase. Use this skill whenever you are designing APIs, writing data models, building authentication flows, implementing logging, handling user data, writing retention/deletion jobs, designing cloud infrastructure, or reviewing pull requests for privacy compliance. Trigger this skill for any task involving personal data, user accounts, cookies, analytics, emails, audit logs, encryption, pseudonymization, anonymization, data exports, breach response, CI/CD pipelines that process real data, or any question framed as "is this GDPR-compliant?". Inspired by CNIL developer guidance and GDPR Articles 5, 25, 32, 33, 35.'
---
# GDPR工程技能

为工程师、架构师、DevOps和技术主管提供可操作的GDPR参考。
受CNIL开发者指南和GDPR第5、25、32、33、35条的启发。

> **黄金法则：**少收。商店更少。少暴露。少保留。

要深入研究，请阅读`references/`中的参考文件：
-`references/data-rights.md`-用户权限端点，DSR工作流，RoPA
-`references/security.md`-加密，哈希，秘密，匿名化
-`references/operations.md`-云，CI/CD，事件响应，架构模式

---

# # 1。GDPR核心原则（第5条）

|原理|工程义务||---|---|
|合法性、公平性、透明度|为RoPA |中每一个处理活动提供法律依据
|目的限制|在没有新的法律依据的情况下，为目的A **收集的数据绝对不能**用于目的B
|数据最小化|只收集有文档化业务需求的字段|
|提供更新端点；将更正传播到下游存储|
在模式设计时定义TTL -不要在|之后
|在静态和传输中加密；限制和审计访问|
保持合规性的证据；RoPA准备好随时进行DPA检验

---

# # 2。隐私的设计和默认* *必须* *
-在创建时将`CreatedAt`，`RetentionExpiresAt`添加到每个保存个人数据的表中。
—所有可选数据采集默认为**关闭**。用户选择加入；他们从不选择退出默认设置。
-在建立高风险处理（生物识别，健康数据，大规模分析，系统监测）之前进行DPIA。
-用引入加工活动的每个新功能更新RoPA**。
-在数据流向每个子处理器之前，与它们签署一个DPA**。

* *不能* *
在没有法律依据的情况下发布新的数据收集功能。
-默认情况下启用分析、跟踪或遥测，无需明确同意。
-将个人资料储存在RoPA未列出的系统中。

---

# # 3。数据最小化* *必须* *
-将每个DTO/model字段映射到具体的业务需求。删除未归档的字段。
-使用不同的dto来创建、读取和更新-永远不要重用相同的对象。
-只返回呼叫者被授权看到的内容-使用响应投影。
-在边缘掩码敏感值：为卡号返回`****1234`，从不返回完整值。
-从默认list/search预测中排除敏感字段（出生年月、国民身份证、健康状况）。

* *不能* *
-记录完整的request/response正文，如果其中可能包含个人数据。
—在URL路径段或查询参数（CDN日志、浏览器历史记录）中包含个人数据。
—在没有明确法律依据的情况下收集`dateOfBirth`、国民身份证或健康数据。

---

# # 4。目的限制* *必须* *
-在代码注释和RoPA中记录每个处理活动的目的。
—数据二次再利用前，请先获取新的法律依据或进行兼容性分析。

* *不能* *
-未经明确同意，与广告网络分享为提供服务而收集的个人资料。
-在没有单独法律依据和用户通知的情况下，使用支持票证内容来训练ML模型。

---

# # 5。存储限制和保留

* *必须* *
-每个保存个人数据的表格**必须**有明确的保存期限。
-通过计划作业（Hangfire, cron）自动执行保留，而不是手动过程。
—当保留期到期时，对数据进行匿名化或删除—永远不要将过期的数据静默地留在生产中。

* *建议默认值* *

|数据类型|最大保留量||---|---|
|认证/审计日志| 12-24个月|
|会话/刷新令牌| 30-90天|
|邮件/通知日志| 6个月|
|未激活用户帐户|上次登录后12个月→通知→删除|
按税法要求（7-10年），最低|
|分析事件| 13个月|

* * * *
—添加`RetentionExpiresAt`列—compute at insert time。
—使用软删除（`DeletedAt`），并在擦除请求窗口（30天）后进行定时硬删除。

* *不能* *
-无限期保留个人资料，“以备日后使用”。

---

# # 6。API设计规则* *必须* *
-绝对不能在URL路径或查询参数中包含个人数据。
——`GET /users/{userId}`-验证返回或接受个人数据的所有端点。
-从JWT中提取代理用户的身份，而不是从请求体中提取。
-验证每个资源的所有权：`if (resource.OwnerId != currentUserId) return 403`。
-使用uuid或不透明标识符-绝不使用顺序整数作为公共资源id。

* * * *
—限速敏感端点（登录、数据导出、密码重置）。
—设置`Referrer-Policy: no-referrer`和显式的`CORS`allowlist。

* *不能* *
—返回API响应中的堆栈跟踪、内部路径或数据库错误。
—通过认证的api使用`Access-Control-Allow-Origin: *`。

---

# # 7。日志记录规则* *必须* *
—应用日志中的ip匿名化—mask最后八位（IPv4）或后80位（IPv6）。
——`192.168.1.xxx`-绝对不能记录：密码，令牌，会话id，凭据，卡号，国民id，健康数据。
-绝对不能在PII可能存在的地方记录完整的request/response主体。
—执行日志保留—在指定时间后自动清除。

* * * *
—日志**事件**不是数据：`"User {UserId} updated email"`不是`"Email changed from a@b.com to c@d.com"`。
—使用结构化日志（JSON），`userId`作为内部标识符，而不是电子邮件地址。
-将审计日志（敏感访问，管理操作）与应用程序日志分开-不同的保留和acl。

---

# # 8。错误处理* *必须* *
—返回通用错误消息—从不暴露堆栈跟踪、内部路径或DB错误。
——`"Column 'email' violates unique constraint on table 'users'"`——`"A user with this email address already exists."`-使用**问题详细信息(RFC 7807)**所有错误响应。
-记录完整的错误服务器端与相关ID；只向客户端返回关联ID。

* *不能* *
—在错误响应中包含文件路径、类名或行号。
-在错误信息中包含个人数据（例如，“找不到用户john@example.com”）。

---

# # 9。加密（摘要-详细信息请参见`references/security.md`）

|适用范围|最低标准||---|---|
|标准个人数据| AES-256disk/volume加密|
|敏感数据（健康、金融、生物识别）| AES-256 **列级** +通过KMS |进行信封加密
|在轨| TLS 1.2+（首选1.3）；HSTS强制b|
| hsm支持的KMS；每年轮换DEKs

**绝对不能**允许TLS1.0/1.1，空密码套件或硬编码加密密钥。

---

# # 10。密码哈希

* *必须* *
—推荐使用**Argon2id**或**bcrypt**（成本≥12）。不能使用MD5、SHA-1和SHA-256。
-每个密码使用唯一的盐。只存储哈希值。

* *不能* *
—任何形式的密码。以url方式传输密码。以明文形式存储重置令牌。

---

# # 11。管理的秘密* *必须* *
-将所有秘密存储在KMS中：Azure密钥库、AWS秘密管理器、GCP秘密管理器或HashiCorp密钥库。
-使用预提交钩子（`gitleaks`,`detect-secrets`）来防止秘密提交。
-在开发人员离职、年度计划或可疑的妥协时轮换秘密。

**`.gitignore`必须包含：**`.env`，`.env.*`,`*.pem`,`*.key`,`*.pfx`,`*.p12`,`secrets/`* *不能* *
—将秘密提交到源代码。将秘密存储为纯文本环境变量默认值。

---

# # 12。匿名化和假名化（摘要-见`references/security.md`）- **匿名化** =不可逆→不属于GDPR范围。用于擦除后保留的记录。
- **假名** =可逆与钥匙→仍然是个人数据，降低风险。
—删除用户时，对必须保留的记录（如财务、审计）进行匿名化处理，而不是直接删除。
—将假名密钥存储在KMS中，不要与假名数据存储在同一个数据库中。

**绝对不能**调用数据“匿名”，如果通过链接攻击重新识别是可能的。

---

# # 13。使用假数据进行测试

* *必须* *
-绝对不能在开发、登台或CI环境中使用生产个人数据。
-在没有首先擦洗PII的情况下，绝对不能将生产DB备份恢复到非生产。
-使用合成数据生成器：`Bogus`(。. NET)、`Faker`（JS/Python/Ruby）。
—使用`@example.com`为所有测试电子邮件地址。

---

# # 14。反模式

|反模式|正确方法||---|---|
|不透明的uuid作为公共标识符|
|记录完整的请求体|仅记录结构化事件元数据|
|“永远保持”模式|在设计时定义的TTL |
|dev/test|合成数据+洗涤管道|
|跨团队共享凭据|个人账户+ RBAC |
|硬编码秘密| KMS +秘密管理器|
|`Access-Control-Allow-Origin: *`on auth api |显式CORS allowlist |
|使用配置文件数据存储同意|专用同意存储|
| GET查询参数中的PII | POST主体或认证会话|
|顺序整数公共url id | uuid |
|准标识符的“匿名化”数据|应用k-匿名，测试链接阻力|
|混合EEA以外的备份区域|对备份任务|进行明确的区域锁定

---

# # 15。公关审查清单数据模型
-每个新的PII栏目都有一个记录在案的目的和保存期限。
—敏感字段（健康、金融、国民ID）使用列级加密。
-不允许连续整数pk作为面向公众的标识符。

# # # API
—URL路径或查询参数中没有PII。
-所有返回个人数据的终端都经过身份验证。
—所有权检查—用户不能访问其他用户的资源。
-对敏感端点进行速率限制。

# # #日志
-没有密码，令牌，或凭证记录。
—ip匿名化（最后八位掩码）。
-在PII可能存在的地方没有完整的request/response体记录。

# # #基础设施
—没有公有存储桶或公有ip数据库。
-新的云资源标记为`DataClassification`。
—新建的存储资源启用静态加密。
-数据存储的新地理区域符合eea或被SCCs覆盖。###秘密&CI/CD-源代码或提交的配置文件中没有秘密。
新秘密添加到KMS和秘密清单文件。
-CI/CD秘密隐藏在管道日志。

保留和擦除
-保留执行作业或策略覆盖新的数据存储或字段。
-擦除管道更新，以覆盖新的数据存储。

###用户权限和治理
-数据导出端点包括任何新的个人数据字段。
-引入新的加工活动时更新RoPA。
-新的子处理器有签名DPA和RoPA条目。
-当变更涉及高风险处理时触发DPIA。

---

> **黄金法则：**少收。商店更少。少暴露。少保留。
>
你不收集的每一个字节的个人数据都是你不能丢失的，
b>不能违约，也不能对此负责。

---*受CNIL开发者GDPR指南的启发，GDPR第5、25、32、33、35条
ENISA， OWASP和NIST工程最佳实践