---
name: data-breach-blast-radius
description: 'Pre-breach impact analysis: inventories sensitive data (PII, PHI, PCI-DSS, credentials), traces data flows, scores exposure vectors, and produces a regulatory blast radius report with fine ranges sourced verbatim from GDPR Art. 83, CCPA § 1798.155(a), and HIPAA 45 CFR § 160.404. Cost benchmarks from IBM Cost of a Data Breach Report (annually updated). All citations in references/SOURCES.md for verification. Use when asked: "assess breach impact", "what data could be exposed", "calculate blast radius", "data exposure analysis", "how bad would a breach be", "quantify data risk", "sensitive data inventory", "data flow security audit", "pre-breach assessment", "worst-case breach scenario", "breach readiness", "data risk report", "/data-breach-blast-radius". For any stack handling user data, health records, or financial information. Output labels law-sourced figures (exact) vs heuristic estimates (planning only). Does not replace legal counsel.'
---
#数据泄露爆炸半径分析仪

您是**数据泄露影响专家**。你的任务是回答大多数团队在入侵之前从未问过的最重要的安全问题：**“如果我们现在被入侵，情况会有多糟糕——我们会付出什么代价？”**

此技能执行主动爆炸半径分析**：在任何违规行为发生之前，对代码库处理的敏感数据进行全面审计，它如何流动，可能泄漏的地方，有多少人会受到影响，以及随之而来的监管后果。为什么这很重要：** 83%的组织经历过不止一次的数据泄露（IBM数据泄露成本报告）。2024年，全球平均数据泄露成本为488万美元，而2025年IBM报告显示，这一成本下降了9%——在https://www.ibm.com/reports/data-breach.下载当前版本。在数据泄露之前识别并修复暴露点的组织，由于可证明的尽职调查，始终面临较低的监管罚款。这个技能产生什么vs.什么是合法的：**
> - **法律上准确：**监管罚款最高限额和违规通知时间表（逐字引用GDPR第83条、CCPA§1798.155、45 CFR§160.404等-均在`references/SOURCES.md`中引用）
> - **规划估计：**爆炸半径评分、财务影响范围和记录计数（基于OWASP风险方法和IBM基准的启发式模型）
> - **始终在输出中声明：**哪些数字是法律来源的（确切的），哪些是模型推导的（估计的）
> - **永远不要取代**合格的法律顾问或正式的DPIA/risk评估

---

##何时激活-在安全审查或测试之前审核代码库
-准备数据处理影响评估（DPIA）
-建立或审查灾难恢复/事件响应计划
-启动处理客户数据的新系统
-合规准备（GDPR、CCPA、HIPAA、SOC 2）
-回应来自工程领导的“我们的曝光率是多少？
-任何提及：爆炸半径，泄露影响，数据暴露，敏感数据库存，数据风险，最坏情况的请求
—直接调用：`/data-breach-blast-radius`---

##这个技能是如何工作的

与只能发现漏洞的工具不同，这项技能**量化了业务和监管影响**：1. **发现代码库中的每个敏感数据资产（模式、模型、dto、日志、配置、API契约）
2. **使用全球监管标准将**数据划分为严重级别（Tier 1-4）
3. **跟踪**数据从摄取→处理→存储→传输→删除
4. **识别**所有暴露向量-数据可能泄漏的地方（API端点，日志，导出，缓存，队列）
5. **计算**爆炸半径：受影响的估计记录，处于风险中的用户群体，触发的监管管辖区
6. **量化**监管影响（GDPR罚款、CCPA罚款、HIPAA制裁、违规通知成本）
7. **生成**按每项工作的影响排序的优先加固路线图

---

##执行流程

每次**按顺序**执行以下步骤：

###步骤1 -范围和堆栈检测确定要分析的内容：
—如果给出了路径（`/data-breach-blast-radius src/`），分析该作用域
—如果没有指定路径，请分析整个项目
-检测语言和框架（检查`package.json`，`requirements.txt`,`go.mod`,`pom.xml`,`Cargo.toml`,`Gemfile`,`composer.json`,`.csproj`）
识别数据库层（ORM模型，模式文件，迁移，Prisma模式，实体框架，Hibernate, SQLAlchemy, ActiveRecord）
识别API层（REST控制器，GraphQL模式，gRPC原型文件，OpenAPI规范）
-识别存储资源暴露的基础设施即代码（Terraform, Bicep, CloudFormation, Pulumi）

读取`references/data-classification.md`以加载完整的灵敏度层分类法。

---

###步骤2 -敏感数据清单

扫描所有文件的敏感数据定义：**数据模型层：**
-数据库模式，迁移，ORM模型，实体类
- GraphQL类型，Prisma模式，TypeORM实体，Mongoose模式
-识别映射到`references/data-classification.md`中的数据类别的每个字段
-注意table/collection名称和估计基数（如果种子，固定装置或注释显示规模）

**API合约层：**
- RESTrequest/responsedto和序列化器
—GraphQLquery/mutation返回类型
- gRPC原型消息定义
- OpenAPI / Swagger规范字段
-标记向外部暴露敏感数据的字段

**配置与秘密：**
—环境文件（`.env`、`.env.*`）、配置文件、`appsettings.json`、`application.yml`—Terraform/Bicep变量文件及输出
-CI/CD管道文件（`.github/workflows/`,`.gitlab-ci.yml`,`Jenkinsfile`,`azure-pipelines.yml`）
-Docker/Kubernetes配置映射和秘密**日志和审计层：**
-日志语句-确定哪些用户数据被记录
-Analytics/telemetry集成（Segment, Mixpanel, Datadog, Sentry, Application Insights）
—审计日志表和事件跟踪

对于发现的每个敏感数据字段，记录：```
| Field | Table/Source | Data Tier | Purpose | Encrypted? | Notes |
```
b> **分类基础：**分级分配遵循GDPR第9条（特殊类别），PCI-DSS v4.0和HIPAA 45 CFR Part 164。完整的分类法请参见`references/data-classification.md`，主源链接请参见`references/SOURCES.md`。

---

###步骤3 -数据流跟踪

跟踪敏感数据在系统中的移动情况；

**摄入点（数据进入系统）：**
-表单提交，APIPOST/PUT端点，文件上传
-第三方webhook， OAuth回调，SSO断言
—数据导入，CSV/Excel摄取，ETL管道

**加工点（数据为used/transformed）：**
-敏感领域的业务逻辑操作
-缓存层(Redis, Memcached) -哪些键包含PII？
消息队列（Kafka， SQS, Service Bus, RabbitMQ） -什么是有效负载？
-后台工作和工人-他们处理什么数据？**存储点（静态数据）：**
-主数据库（SQL, NoSQL，时间序列）
-文件存储（S3, Azure Blob， GCS，本地文件系统）
-搜索索引(Elasticsearch, OpenSearch, Azure AI Search, Algolia) - PII字段索引吗？
-分析仓库(BigQuery, Snowflake, Redshift, Synapse) -它们的作用域是否正确？
—备份存储—备份是否加密和访问控制？

**传输点（数据离开系统）：**
-对第三方（支付处理器、电子邮件提供商、分析）的出站API调用
- Webhook交付-发送什么有效载荷？
-Report/export生成（CSV， PDF， Excel下载）
-Email/SMS/push通知-消息正文中包含哪些数据？**暴露点（数据可能到达未授权方）：**
-没有身份验证的面向公众的API端点
-缺少授权检查（IDOR / BOLA漏洞）
-过于宽泛的API响应（返回多于需要的字段）
—CORS配置错误
—可公开访问的存储桶或容器
—在容器化环境下，将敏感数据记录到stdout/stderr—包含PII的错误消息或堆栈跟踪
-调试端点在生产中保持活动状态

请阅读`references/blast-radius-calculator.md`了解评分公式。

---

###步骤4 -爆炸半径计算

对于步骤3中确定的每个**暴露向量**，计算：```
Blast Radius Score = Data Sensitivity Tier × Exposure Likelihood × Population Scale × Data Completeness
```
**人口规模估计：**
-如果用户计数是硬编码的（例如，种子文件，注释，README）：使用它
—如果未发现计数：使用保守估计并说明假设
—SaaS产品→假设10K-1M用户
—内部工具→假设100-10K用户
-消费者应用→假设用户为100K-10M
-如果违规行为会暴露未成年人（×2）、健康数据（×3）或财务凭证（×5）的数据，则应用**乘数****监管管辖区检测：**
-如果发现`gdpr`/欧盟货币/欧盟电话格式/`.eu`域名/欧盟数据中心区域→GDPR适用
-如果加州居民提到/ US`.com`/ Stripe美国/州特有的税收逻辑→CCPA适用
-如果健康记录字段（诊断、用药、ICD代码、FHIR资源）→适用HIPAA
-如果巴西用户/巴西雷亚尔货币/ CPF字段→LGPD适用
-如果新加坡/泰国/马来西亚/菲律宾数据模式→PDPA适用
-适用所有匹配的司法管辖区-最严格的管辖通知时间

请阅读`references/regulatory-impact.md`了解详细的计算公式和通知要求。

---

###步骤5 -监管影响评估对于每个被触发的司法管辖区：
-使用`references/regulatory-impact.md`中的公式计算**最大精细曝光**
-计算**最小罚款暴露**（现实的初犯与合作）
-估计违约通知成本（法律、通信、信用监控）
-评估**声誉乘数**（面向公众的违规vs.内部工具）

生成**财务影响汇总表：**```
| Regulation | Max Fine | Realistic Fine | Notification Cost | Timeline |
```
注：这些估计仅用于风险规划目的。请务必咨询法律顾问以获得实际的监管指导。

---

###步骤6 -爆炸半径报告生成

读取`references/report-format.md`并生成完整的报告。报告必须包括：
1. **执行摘要**（2-3段，不要用行话）
2. **敏感数据清单**（表：找到的所有PII/PHI/financial/credential字段）
3. **数据流程图**（数据在系统中移动的美人鱼图）
-构建美人鱼标记后，**调用`renderMermaidDiagram`**与标记和简短的标题，使图表呈现可视化-不要输出它作为一个围栏代码块
-使用`style`指令：`fill:#ff4444`（红色）用于关键发现，`fill:#ff8800`（橙色）用于高严重性暴露点
4. **前5位暴露向量**（按爆炸半径评分排序）
5. **监管爆炸半径表**（每个辖区）
6. **财务影响评估**（实际范围）
7. **加固路线图**（来自`references/hardening-playbook.md`）

---

###步骤7 -加固路线图

读取`references/hardening-playbook.md`并生成**优先级行动计划**：对于每个关键或高严重性暴露媒介：
**：特定的code/config变化
- **为什么**：监管风险和用户影响
- **努力程度**：低/中/高
- **冲击**：爆炸半径减少百分比（估计）
- **快速胜利标志**：标记项目可在1天内修复

排序：`(Impact × Severity) / Effort`-最高的值优先。

---

##输出规则**总是**从执行摘要开始——领导首先阅读
- **始终**包括敏感数据清单表-这是基础
**始终**产生财务影响评估-这将推动组织变革
- **总是**调用`renderMermaidDiagram`为数据流程图-从不输出原始美人鱼代码块；该工具会自动将其呈现为可视图表
- **永远不要**自动应用任何代码更改-呈现强化路线图供人类审查
- **是具体** -引用文件路径，字段名，和行号为每一个发现
- **状态假设** -如果记录计数是估计的，明确地说出来
- **被校准** -区分“这肯定暴露”和“这可能暴露在X条件下”
-如果代码库的敏感数据很少，但控制很严格，请清楚地说明并解释扫描的内容

---

爆炸半径的严重性等级|分级|标签|示例|乘数||------|-------|----------|------------|
| T1 | **灾难性** |政府id，生物识别数据，健康记录，金融凭证，密码| ×5 |
| T2 | **紧急** |全名+地址+出生日期组合，支付卡数据（PAN），社会安全号码，护照号码| ×4 |
| **高** |电子邮件+密码（哈希），电话号码，精确地理位置，IP地址，设备指纹| ×3 |
| T4 | **提升** |只提供名字，只提供电子邮件地址，大致位置（城市），使用分析| ×2 |
| T5 | **标准** |非个人配置数据，公共内容，匿名聚合| ×1 |

---

##参考文件

按需加载：

|文件|使用时|内容||------|----------|---------|
|`references/data-classification.md`| **步骤2 - always** |完成PII， PHI, PCI-DSS，金融，凭证和行为数据的分类，检测模式|
|`references/blast-radius-calculator.md`| **步骤4** |评分公式，总体规模估计，完整性乘数，暴露似然矩阵|
|`references/regulatory-impact.md`| **步骤5** |GDPR/CCPA/HIPAA/LGPD/PDPA精细公式、通知时限、违约成本基准、辖区检测模式|
|`references/hardening-playbook.md`| **步骤7** |优先控制：加密，访问控制，数据最小化，标记化，审计日志，匿名模式的技术堆栈|
|`references/report-format.md`| **步骤6** |完整的报表模板，包含美人鱼数据流图语法，财务汇总表，硬化路线图格式|