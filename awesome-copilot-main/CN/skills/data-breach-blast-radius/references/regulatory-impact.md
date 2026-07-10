#监管影响参考

适用于所有主要全球数据保护法规的精细公式、违规通知时间表、成本基准和管辖权检测模式。

b> **免责声明：**此参考仅用于风险规划和开发人员教育。所有精确的估计都是基于公开可用的法律文本和`SOURCES.md`中引用的基准的近似值。向合格的法律顾问咨询您所在司法管辖区的实际监管指导。

> **验证这些数字：**本文件中的每个精细公式都来自法规的主要法律文本。请参阅`references/SOURCES.md`了解每个图的确切statute/articleURL。如果有任何数字看起来不对，请先检查SOURCES.md-如果它真的过期了，请打开PR。

---

管辖权检测模式

扫描这些信号的代码库，以确定哪些规则适用：GDPR （EU/EEA-通用数据保护条例）
* *触发信号:* *```
# Geographic signals
- Currency: EUR, GBP (for UK GDPR)
- Phone formats: +44, +49, +33, +31, +34, +39, +46, +47, +358, +45, +48
- Locale strings: 'de', 'fr', 'es', 'it', 'nl', 'pl', 'pt', 'sv', 'da', 'fi', 'nb', 'el'
- Country codes: DE, FR, ES, IT, NL, PL, BE, SE, AT, CH, DK, FI, NO, PT, GR, IE, HU, CZ, RO
- Cloud regions: eu-west-*, eu-central-*, northeurope, westeurope, francecentral, germanywestcentral
- Domain TLDs: .de, .fr, .es, .it, .nl, .pl, .eu, .uk, .ie, .at, .se, .dk, .fi, .be, .no, .pt

# Code signals
- GDPR-related comments or variable names: gdpr, dpa, data_protection, lawful_basis
- Consent management code: cookie_consent, gdpr_consent, marketing_opt_in
- Right to erasure endpoints: /delete-account, /forget-me, /data-deletion
- Data export endpoints: /export-data, /download-my-data, /dsar
- EU-specific third-party integrations: TrustArc, OneTrust, Cookiebot, Axeptio

# Config signals
- AWS S3 buckets with eu- prefix
- Azure storage accounts in European regions
- GCP storage in europe-* regions
```
**适用于：**处理EU/EEA居民个人数据的任何组织，无论该组织位于何处。

---

CCPA / CPRA（加州-消费者隐私权法案）
* *触发信号:* *```
# Geographic signals
- Country: US with state: CA, California
- Sales tax for California (CA sales tax logic)
- Phone format: +1 with 213, 310, 323, 408, 415, 424, 510, 530, 562, 619, 626, 650, 707, 714, 805, 818, 831, 858, 909, 916, 925, 949, 951

# Code signals
- CCPA-related comments: ccpa, california_privacy, do_not_sell, opt_out_of_sale
- Privacy preference center with California toggle
- Opt-out links: /do-not-sell, /privacy-choices, /opt-out
- GPC (Global Privacy Control) header handling

# Business signals
- Annual gross revenue > $25M (implied by scale signals in codebase)
- Comments/configs referencing California consumer data
```
**适用于：**满足以下任何一项的营利性企业：年总收入> $ 2500万，每年buys/sells/receives/shares个人数据100,000 +consumers/households，或从销售个人数据中获得50%以上的收入。

---

HIPAA（美国-健康保险流通与责任法案）
* *触发信号:* *```
# Field name signals (PHI — Protected Health Information)
- medical_record_number, mrn, patient_id, encounter_id
- diagnosis, icd_code, icd10, medication, prescription
- lab_result, test_result, radiology, pathology
- health_plan_id, insurance_id, claim_number
- fhir_, hl7_, dicom_

# Integration signals
- Epic, Cerner, Allscripts, eClinicalWorks API keys or webhooks
- FHIR API endpoints (/fhir/, /r4/, /stu3/)
- HL7 message parsing
- CMS (Centers for Medicare & Medicaid) API integration
- SNOMED, LOINC, ICD code lookups

# Config signals
- HIPAA compliance flags or BAA (Business Associate Agreement) references
- HIPAA-compliant hosting: AWS HIPAA BAA, Azure Healthcare APIs, GCP HIPAA
- Healthcare-specific cloud: Microsoft Cloud for Healthcare, Google Cloud Healthcare API
```
**适用于：**受保实体（医疗保健提供者、健康计划、票据交换所）及其业务伙伴（代表其处理PHI的供应商）。

---

LGPD（巴西- Lei general de proteo de Dados）
* *触发信号:* *```
# Geographic signals
- Currency: BRL, R$
- Phone format: +55
- Locale: pt-BR, pt_BR
- Country codes: BR, BRA, Brazil
- CPF field (Brazilian individual taxpayer registry): cpf, cpf_number
- CNPJ field (Brazilian company registry): cnpj
- CEP (Brazilian postal code): cep, codigo_postal (8 digits, XXXXX-XXX format)

# Code signals
- lgpd references in comments or variable names
- Brazilian payment integrations: PicPay, Nubank, Mercado Pago, PagSeguro, PIX
- Brazilian cloud regions: sa-east-1 (AWS São Paulo), brazilsouth (Azure)
```
**适用于：**在巴西对个人数据的任何处理，或在巴西进行的任何处理。

---

PDPA（多个亚洲司法管辖区）

####新加坡PDPA
**触发信号：**`+65`、`SGD`、`sg`locale、`.sg`TLD、`nric`field、`fin`（Foreign Identification Number）、`singpass`####泰国PDPA
**触发信号：**`+66`、`THB`、`th`区域设置、`.th`TLD、`thai_id`####马来西亚PDPA
**触发信号：**`+60`、`MYR`、`ms`区域设置、`.my`TLD、`my_kad`、`nric_malaysia`####菲律宾数据隐私法
**触发信号：**`+63`，`PHP`（货币），`ph`区域设置，`.ph`TLD,`phil_sys_number`####日本个人信息保护法
**触发信号：**`+81`、`JPY`、`ja`区域设置、`.jp`TLD、`my_number`（日本国民身份证）、`maruhi`（保密）

---

###其他规定（如果适用则标记）|监管|管辖|关键触发||-----------|-------------|-------------|
| PIPEDA / Law 25 |加拿大|`+1`+加拿大各省，`CAD`,`.ca`TLD， SIN域|
|澳大利亚隐私法案|澳大利亚|`+61`，`AUD`,`.au`TLD，`tfn`字段|
| POPIA |南非|`+27`，南非兰特`.za`TLD，`sa_id_number`|
| KVKK |土耳其|`+90`，`TRY`,`.tr`TLD |
| PDPB |印度（即将推出）|`+91`,`INR`，`aadhaar`字段-注：尚未生效|
| SOC 2 Type II |美国（安全标准，不是法律）|在代码库中提到，客户合同|
| PCI-DSS | Global（支付卡）|任意卡号/ CVV / PAN字段|

---

## GDPR罚款计算器**法律来源：** GDPR第83 -https://gdpr-info.eu/art-83-gdpr/条
**原文，第83.4条：**“……最高1000万欧元，或…占全球年营业额的2%。以高的为准。”
**原文，第83.5条：**“……最高2000万欧元，或…占全球年营业额的4%。以高的为准。”

最高罚款（第83条）```
Tier 1 violations (less severe — Art. 83.4):
  Maximum = max(€10,000,000, 2% of global annual turnover)
  [Note: 'higher' means the LARGER of the two — corrected from min() to max()]

Tier 2 violations (most severe — Art. 83.5 — core principles, data subject rights, cross-border transfers):
  Maximum = max(€20,000,000, 4% of global annual turnover)
```
风险规划的精细估计公式
当年度revenue/turnover未知时，使用以下保守估计：

|公司简介|预计年营业额|现实T1良好|现实T2良好||----------------|--------------------------|-------------------|-------------------|
| < 200万欧元|€2.5万- 10万欧元|€5万- 25万欧元|
b|小企业（10-50名员工）b| 200万- 1000万欧元b| 5万- 40万欧元b| 10万- 80万欧元b|
|中型企业（50-500名员工）| 1000万- 1亿欧元| 20万- 200万欧元| 50万- 400万欧元|
|大型企业（500 - 5000名员工）| 1亿- 10亿欧元| 200万- 2000万欧元| 500万- 4000万欧元|
|跨国公司| | 1000万欧元（上限为2%）| 2000万欧元（上限为4%）|

**校准的历史GDPR罚款（所有公开验证-链接在SOURCES.md）：**
- Meta: 12亿欧元（2023年）-跨境数据传输违规
亚马逊：7.46亿欧元（2021年）——违反cookie许可
- WhatsApp: 2.25亿欧元（2021年）-透明度违规
-谷歌：1.5亿欧元（法国，2022年）-饼干退出
H&M: 3530万欧元（2020年）——员工监督
-英国航空公司：2200万欧元（2020年）-安全漏洞（50万条记录）
-万豪：1840万欧元（2020年）-安全漏洞（3.39亿条记录）违反通知罚款增强：**不通知或延迟通知增加20-30%的基础罚款。

---

CCPA / CPRA罚款计算器

**法律来源：**加州民法典第1798.155(a)条（于2025年6月30日修订，Stats. 2025，第20章）-https://leginfo.legislature.ca.gov/faces/codes_displaySection.xhtml?lawCode=CIV&sectionNum=1798.155**私人诉讼权利来源：**加州民法典§1798.150 -https://leginfo.legislature.ca.gov/faces/codes_displaySection.xhtml?lawCode=CIV&sectionNum=1798.150```
Non-intentional violations: $2,500 per violation    [§ 1798.155(a)]
Intentional violations: $7,500 per violation         [§ 1798.155(a)]
Children's data violations: $7,500 per violation    [§ 1798.155(a) — intent not required for minors]
Private right of action: $100–$750 per consumer     [§ 1798.150]
```
###大规模缺口计算```
Max_CCPA_Fine = Records_affected × $7,500 (if intentional)
             = Records_affected × $2,500 (if unintentional)
```
**上限：**加州总检察长可以要求每名消费者赔偿2500美元，但根据私人诉讼权利提起的集体诉讼可以达到每名消费者100 - 750美元。

**私权（CCPA/CPRA独有）：**```
Civil_damages = max($100, min($750, actual_damages)) × affected_California_consumers
```
* *例子:* *
- 10万加州用户× 750美元= 7500万美元的最高私人诉讼权利
- 10万用户× 2500美元=最高CCPA罚款2.5亿美元（监管）

---

HIPAA精细计算器

**法律来源：** 45 CFR§160.404 -https://www.ecfr.gov/current/title-45/subtitle-A/subchapter-C/part-160/subpart-D/section-160.404**HHS执行页面：**https://www.hhs.gov/hipaa/for-professionals/compliance-enforcement/examples/all-cases/index.html**注：**金额为美国卫生与公众服务部2024年经通胀调整后的数字。每年更新-在上面的HHS链接验证。

HIPAA罚款按knowledge/culpability分级（45 CFR§160.404）：

|分级|罪责|每次违规最小|每次违规最大|年度上限|
|不知道| $137 | $68,928 | $2,067,813 |
| B |合理原因| $1,379 | $68,928 | $ 2,0067,813 |
|故意忽视，纠正| $13,785 | $68,928 | $2,067,813 |
|故意忽视，未纠正| $68,928 | $1,919,173 | $1,919,173 |

**违规计划：**暴露PHI的每个受影响患者记录= 1次违规。**违规通知费用：** HHS要求通知受影响的个人+ HHS。一个州超过500个人的违规行为需要媒体通知。超过500次的违规行为需要HHS年度报告。

**刑事处罚（司法部-严重案件）：**
-最高50,000元+ 1年监禁（简单违例）
-最高10万元+ 5年监禁（以虚假借口）
-最高25万美元+ 10年（意向为sell/use）

---

LGPD罚款计算器（巴西）

**法律来源：** Lei nº13.709/2018(LGPD) -第52条，I -https://www.planalto.gov.br/ccivil_03/_ato2015-2018/2018/lei/l13709.htm**ANPD（巴西DPA）：**https://www.gov.br/anpd/pt-br```
Maximum fine per violation = 2% of revenue in Brazil in the prior fiscal year  [Art. 52, I]
Hard cap = R$50,000,000 (≈ $10M USD) per violation                            [Art. 52, I]
```
违规期间可按日罚款。
**巴西DPA （ANPD）的执行始于2021年。**执法力度正在加大。

---

违约通知时间参考

**所有时间表均来自主要法律文本。**参见`SOURCES.md`的准确的article/sectionurl为每个规则。

在发现违规行为后，您必须多快通知监管机构和受影响的个人？

|监管|监管机构通知|个人通知|法律来源|备注||-----------|----------------------|------------------------|-------------|-------|
| GDPR | **发现后72小时** |如果是高风险，“不得无故延误”|第33条和第34条|即使细节不完整也必须通知|
|英国GDPR | **发现后72小时** |无不当延误|英国GDPR第33条|在英国脱欧后保留欧盟法律|
| CCPA / CPRA |“最合适的时间”（没有硬数字）|相同|呼叫文明。代码§1798.82 | CA AG如果> 500 CA居民|
| **发现后60天** | 60天（或更早）| 45 CFR§164.412 | HHS + 500+媒体在一个州|
| LGPD（巴西）| **2个工作日** （ANPD指引）|尽快| ANPD决议nº2/2022| ANPD自2021年起执行|
|新加坡PDPA | **强制违约3个日历日** |无不当延误| PDPA第26D条（2021年修订）|全球最严格的|之一
|澳大利亚隐私法案|尽快，不迟于**30天** |在可行的情况下尽快| 1988年隐私法案-新开发银行计划|通知数据-违反计划b|
| PIPEDA（加拿大）| **尽快可行** | **尽快可行** | PIPEDA s.10.1 | OPCC通知要求|
|日本APPI | ** 3-5个工作日** |及时| APPI第26条（2022年修订）|从之前的版本|收紧---

总违约成本估算模型

**基准来源：** IBM安全+波耐蒙研究所-“数据泄露成本报告”（每年更新）
* * URL: * *https://www.ibm.com/reports/data-breach以下数字来自**2024年版**（最后验证）。IBM 2025显示减少了9%—下载当前的PDF以获取更新的值。**[IBM 2024，第14页]**页的参考资料指的是2024版。

在生成财务影响评估部分时使用此模型：

直接成本```
1. Detection & containment: $1.1M average      [IBM 2024, p.14]
2. Post-breach response:     $1.2M average      [IBM 2024, p.14]
3. Lost business:            $1.5M average      [IBM 2024, p.14]
4. Notification costs:       records × $2–$8 per individual  [industry estimate]
5. Credit monitoring:        records × $5–$20/year if PII    [industry estimate]
6. Legal costs:              $200K–$3M depending on complexity [industry estimate]
7. Forensic investigation:   $50K–$500K                      [industry estimate]
8. PR/crisis communications: $100K–$500K                     [industry estimate]
```
监管成本```
9. Regulatory fines:         [see per-regulation formulas above — all sourced from law text]
10. Settlement costs:        $1M–$100M+ for class actions    [historic case data]
```
声誉倍增器
根据组织的公众知名度申请：```
B2C consumer app, consumer brand:     ×1.5 (high reputational damage)
B2B enterprise, low public profile:  ×1.1 (moderate reputational damage)
Healthcare or financial institution:  ×2.0 (trust erosion is severe)
Government or public sector:         ×1.8 (public accountability)
```
最终评估格式```
Minimum likely cost:   [conservative scenario, good response, small record count]
Probable cost:         [most likely scenario, average response]
Maximum exposure:      [worst case: maximum fines + class action + reputational]
```
