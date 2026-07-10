#来源和验证

本技能中的每个数字、公式和分类都来自可公开验证的主要来源。该文件的存在使得贡献者、审阅者和用户可以在信任输出之前独立地验证所有声明。

**如果您发现数字错误，过时或缺少引用-请针对此文件打开PR

---

数据分类标准

GDPR特殊类别（一级分类基础）
- **来源：**法规（EU）2016/679-第9条“特殊类别个人数据的处理”
—**URL:**https://gdpr-info.eu/art-9-gdpr/- **声明内容：**生物特征数据、健康数据、基因数据、racial/ethnic来源、政治观点、宗教信仰、性别life/orientation是需要明确同意的“特殊类别”。
- **我们的使用：**这些直接映射到一级在`data-classification.md`PCI-DSS数据分类
-来源：** PCI安全标准委员会- PCI DSS v4.0（2022年3月）
—**URL:**https://www.pcisecuritystandards.org/document_library/- **说明：**主账号（PAN），持卡人姓名，有效期，服务代码=持卡人数据。CVV =敏感的认证数据。两者都必须受到保护。
- **我们的用途：**映射到`data-classification.md`的第2层PCI-DSS

HIPAA保护的健康信息（PHI）定义
- **来源：45 CFR第160部分和第164部分（健康保险流通和责任法案）
—**URL:**https://www.hhs.gov/hipaa/for-professionals/privacy/special-topics/de-identification/index.html- **说明：**使健康数据“受保护”的18个HIPAA标识符-包括姓名，地理数据，日期，电话号码，电子邮件，ssn，医疗记录号码，健康计划id等。
- **我们的用途：**`data-classification.md`的第1层PHI字段

---

GDPR精细公式**来源：**法规（EU）2016/679-第83条“实施行政罚款的一般条件”
* * URL: * *https://gdpr-info.eu/art-83-gdpr/**确切的法律文本（第83.4条）：**
违反下列规定的，应当……被处以最高1000万欧元的行政罚款，或者在企业的情况下，被处以最高上一财政年度全球年营业额2%的罚款，以较高者为准……”

**确切的法律文本（第83.5条）：**
违反下列规定的，应当……被处以最高2000万欧元的行政罚款，或在企业的情况下，最高达上一财政年度全球年营业额的4%，以较高者为准……”

**我们的公式：**直接转录自第83.4条（第1级违规）和第83.5条（第2级违规）。没有补充解释。

**校准的历史罚款（所有公开验证）：**| Fine | Organization | Year |来源URL ||------|-------------|------|------------|
|€12亿| Meta（爱尔兰DPC） | 2023 |https://www.dataprotection.ie/en/news-media/press-releases/data-protection-commission-announces-decision-in-meta-ireland-inquiry|
|€7.46亿|亚马逊（卢森堡）| 2021 |https://iapp.org/news/a/amazon-hit-with-887m-fine-for-gdpr-violations/|
|€2.25亿| WhatsApp（爱尔兰DPC） | 2021 |https://www.dataprotection.ie/en/news-media/press-releases/data-protection-commission-announces-decision-in-whatsapp-inquiry|
|€1.5亿|谷歌（法国CNIL） | 2022 |https://www.cnil.fr/en/cookies-cnil-fines-google-150-million-euros-and-facebook-60-million-euros|
H&M（汉堡DPA） | 2020 |https://www.datenschutz-hamburg.de/news/detail/article/hamburgische-beauftragte-fuer-datenschutz-und-informationsfreiheit-verhaengt-bussgeld-gegen-hm.html|
|€2200万|英国航空公司（ICO） | 2020 |https://ico.org.uk/about-the-ico/media-centre/news-and-blogs/2020/10/ico-fines-british-airways-20m-for-data-breach-affecting-more-than-400-000-customers/|
|€1840万|万豪（ICO） | 2020 |https://ico.org.uk/about-the-ico/media-centre/news-and-blogs/2020/10/ico-fines-marriott-international-inc18-4million-for-failing-to-keep-customers-personal-data-secure/|

---

## CCPA / CPRA精细配方

**来源：**加州民法典第1798.155(a)条-加州消费者隐私法
* * URL: * *https://leginfo.legislature.ca.gov/faces/codes_displaySection.xhtml?lawCode=CIV&sectionNum=1798.155**注（截至2025年6月30日）：** Stats. 2025，第20章，第1节（AB 137）修订§1798.155。行政罚款金额现列于** (a)**款。在修改后的文本中，对`§ 1798.155(b)`的罚款金额的旧引用是不正确的。在上面的URL上验证是否有任何未来的更改。**确切的法定文本（§1798.155(a)修订）：**
b>“任何企业、服务提供商、承包商或其他违反本标题的人，每次违反不超过2500美元（2500美元）的行政罚款，或每次故意违反不超过7500美元（7500美元）的行政罚款……”

**私人诉讼权利来源：**加州民法典第1798.150条
* * URL: * *https://leginfo.legislature.ca.gov/faces/codes_displaySection.xhtml?lawCode=CIV&sectionNum=1798.150**确切的法定文本：**
>“任何消费者的未加密和未编辑的个人信息……受到未经授权的访问和泄露…我对……提起民事诉讼。每位消费者每次事故或实际损害赔偿的金额不低于一百美元（$100），不超过750美元（$750），以金额较高者为准……”我们的公式：**直接转录。每项违规罚款2500美元/ 7500美元，来自§1798.155(a)（于2025年6月30日修订）。$100 - $750私人诉讼权利逐字出自§1798.150。

---

HIPAA精细配方

来源：45 CFR§160.404 -民事罚款
* * URL: * *https://www.ecfr.gov/current/title-45/subtitle-A/subchapter-C/part-160/subpart-D/section-160.404**来源（HHS处罚等级解释）：** HHS民权办公室
* * URL: * *https://www.hhs.gov/hipaa/for-professionals/compliance-enforcement/agreements/index.html**卫生与公众服务部OCR处罚等级（当前通货膨胀调整后的2024年金额）：**
- A级（不知情）：每次违规137 - 68,928美元，年度上限为2,067,813美元
- B级（合理原因）：$1,379 - $68,928，年度上限$2,067,813
- C级（故意改正）：$13,785 - $68,928，年度上限$2,067,813
- D级（故意，未纠正）：68,928 - 1,919,173美元，每年上限1,919,173美元

**当前金额的URL:**https://www.hhs.gov/hipaa/for-professionals/compliance-enforcement/examples/all-cases/index.html**`regulatory-impact.md`中的美元金额与卫生与公众服务部经通货膨胀调整后的2024年罚款等级相符。卫生与公众服务部每年对这些数据进行调整。一定要对照当年的HHS OCR网站进行核实。

**刑事处罚来源：** 42 U.S.C.§1320d-6
* * URL: * *https://uscode.house.gov/view.xhtml?req=granuleid:USC-prelim-title42-section1320d-6---

## LGPD精细配方

**资料来源：**《保护总条例》(LGPD) -《条例》第52条
* * URL: * *https://www.planalto.gov.br/ccivil_03/_ato2015-2018/2018/lei/l13709.htm**准确文本（第52条第1款）：**巴西私人法人实体或集团在其上一财政年度的收入的2%罚款，每次违规不超过500,000,000雷亚尔（5000万雷亚尔）。

**我们的公式：**从第52条逐字逐句。

---

新加坡PDPA精细配方

**资料来源：**《2012年新加坡个人资料保护法》-第48J条
* * URL: * *https://sso.agc.gov.sg/Act/PDPA2012**最高罚款：**每次违规100万新元或新加坡年营业额的10%（如果营业额超过1000万新元）-根据2021年修正案，以较高者为准。

---

违反成本基准

**来源：** IBM安全-“数据泄露成本报告”（自2005年以来每年发布）
* * URL: * *https://www.ibm.com/reports/data-breach**出版商：** IBM安全+波耐蒙研究所
**方法（2024年版）：**对16年17个行业的604家组织进行调查，每次泄露涉及2,170-113,954条泄露记录。

**最新验证数据（IBM 2024版）：**
|度量值|值|源||--------|-------|--------|
全球平均总成本| $ 488万| IBM 2024， p.4 |
|每条记录的医疗保健成本| $408 | IBM 2024, p.12 |
每条记录的平均成本（所有行业）| $165 | IBM 2024, p.11 |
发现漏洞的平均时间| 194天| IBM 2024， p.15 |
控制泄漏的平均时间为73天
|违约成本溢价| 200天| + 102万美元| IBM 2024， p.16 | 4
|从AI/ML安全| -$ 222万| IBM 2024， p.20 |
IBM 2024年，p.21 |
|员工培训成本降低| - 25.8万美元| IBM 2024， p.21 |

**2025年更新：** IBM 2025年报告显示，全球平均收入从488万美元下降了9%。准确的2025年数字需要下载该报告的PDF。**技能维护人员：每年在新版本发布时更新此表

---

违约通知时间表|调控|时间线|来源||-----------|---------|--------|
| GDPR | 72小时| GDPR第33.1条-https://gdpr-info.eu/art-33-gdpr/|
英国GDPR | 72小时|英国GDPR第33条（保留欧盟法律）-https://ico.org.uk/for-organisations/report-a-breach/|
| HIPAA | 60天| 45 CFR§164.412 -https://www.ecfr.gov/current/title-45/subtitle-A/subchapter-C/part-164/subpart-D/section-164.412|
| CCPA |“最合适的时间”|加州文明。代码§1798.82 -https://leginfo.legislature.ca.gov/faces/codes_displaySection.xhtml?lawCode=CIV&sectionNum=1798.82|
|新加坡PDPA | 3个日历天| PDPA 26D -https://sso.agc.gov.sg/Act/PDPA2012|
|澳大利亚隐私法| 30天| 1988年隐私法，APP 1 + NDB计划-https://www.oaic.gov.au/privacy/notifiable-data-breaches|
| LGPD巴西| 2个工作日（ANPD指引）| ANPD决议CD/ANPDnº2/2022-https://www.gov.br/anpd/pt-br|
|日本APPI | 3-5个工作日（2022年修正案）|个人信息保护法第26条-https://www.ppc.go.jp/en/legal/|
PIPEDA加拿大|“尽快可行”| PIPEDA s.10.1 -https://www.priv.gc.ca/en/privacy-topics/privacy-laws-in-canada/the-personal-information-protection-and-electronic-documents-act-pipeda/|

---

##爆炸半径公式基础

评分公式结构改编自已建立的风险量化框架：

|组件|基于||-----------|---------|
| OWASP风险评级方法-https://owasp.org/www-community/OWASP_Risk_Rating_Methodology|
信息风险因子分析（FAIR）模型-https://www.fairinstitute.org/|
CVSS v4.0攻击规模度量-https://www.first.org/cvss/v4-0/|
GDPR陈述75,91（特殊类别增加风险等级）-https://gdpr-info.eu/recital-75-gdpr/|

**公式不是什么：**它不是一个法律认可的标准。它是一种基于公认风险框架的规划启发式方法，产生一个相对分数来比较暴露向量，而不是对违约成本的绝对预测。

---

什么是估计的，什么是准确的

|项目|状态|备注||------|--------|-------|
GDPR罚款最高限额（2000万欧元/营业额4%）| **准确** -从第83.5条逐字逐句，这是法律|
CCPA罚款（$2,500 / $7,500）| **准确** -从§1798.155(a)（修订于2025年6月30日）逐字逐句|这是法律|
| HIPAA分级金额| **与2024年相同** - HHS通胀调整后|每年更新|
|爆炸半径评分| **估计** -启发式规划工具|不是法律或保险数字|
|财务影响范围（$X - $Y） | **估计** - IBM基准+适用于人口的精细公式|不是预测|
|“可能的”罚款金额| **估计** -基于历史罚款模式|实际罚款因监管机构而异|
b|通知时间| **准确** -从法律逐字逐句|这些是严格的法律最后期限|