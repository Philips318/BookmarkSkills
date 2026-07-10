# Data Classification Taxonomy

用于识别代码库中敏感数据的全面分类法。应该列出匹配这些模式的每个字段、列、模型属性或变量，并为其分配适当的敏感层。

---

第1层-灾难性（暴露后造成不可逆转的伤害）

生物识别数据
**检测模式（字段名/列名）：**
-`fingerprint`,`thumbprint`,`retina_scan`,`iris_scan`,`face_id`,`facial_recognition`-`voice_print`,`voice_biometric`,`gait_analysis`,`dna_profile`,`genetic_data`-`biometric_template`,`biometric_hash`,`faceEmbedding`,`face_vector`**检测模式（数据值/格式）：**
- base64编码blobs > 512字节的生物识别命名字段
-名为`biometric_*`、`face_*`、`fingerprint_*`的表中的二进制列###政府发布的标识符
* *检测模式:* *
-`ssn`、`social_security_number`、`social_security`、`sin`（加拿大）、`nino`（英国）、`tfn`（澳大利亚）
-`passport_number`,`passport_no`,`passport_id`-`drivers_license`,`drivers_licence`,`dl_number`,`license_number`-`national_id`,`national_identification`,`id_number`,`id_card_number`-`tax_id`,`tin`,`ein`,`itin`,`vat_number`,`fiscal_code`-`aadhaar`、`pan_number`（印度）、`cpf`、`cnpj`（巴西）、`rut`（Chile/Colombia）
-`nric`、`fin`（新加坡）、`my_kad`（马来西亚）、`nik`（印度尼西亚）

**值的正则表达式模式```
SSN:          \b\d{3}-\d{2}-\d{4}\b
UK NINO:      \b[A-CEGHJ-PR-TW-Z]{2}\d{6}[A-D]\b
CPF (Brazil): \b\d{3}\.\d{3}\.\d{3}-\d{2}\b
Aadhaar:      \b\d{4}\s\d{4}\s\d{4}\b
```
健康和医疗数据（HIPAA下的PHI）
* *检测模式:* *
-`diagnosis`,`icd_code`,`icd10`,`icd11`,`snomed`,`loinc_code`-`medication`,`prescription`,`drug_name`,`dosage`,`treatment`-`medical_record_number`,`mrn`,`patient_id`,`encounter_id`-`lab_result`,`test_result`,`pathology`,`radiology`-`mental_health`,`psychiatric`,`therapy_notes`,`counseling`-`hiv_status`,`std_status`,`substance_abuse`,`addiction`-`insurance_id`,`insurance_member_id`,`health_plan_id`,`claim_number`—`fhir_resource`、`hl7_message`、`dicom_data`-`disability`,`handicap`,`chronic_condition`-`pregnancy`,`reproductive_health`,`fertility`###认证凭证
* *检测模式:* *
-`password`,`passwd`,`pwd`,`hashed_password`,`password_hash`,`password_digest`-`private_key`,`secret_key`,`api_key`,`api_secret`,`api_token`-`access_token`,`refresh_token`,`bearer_token`,`id_token`,`jwt_token`—`oauth_token`、`oauth_secret`、`oauth_access_token`-`mfa_secret`,`totp_secret`,`otp_secret`,`backup_codes`-`session_token`,`session_id`,`auth_token`—`client_secret`、`client_credential`-`private_key_pem`,`rsa_private`,`ecdsa_private`---

2级-关键（高度监管风险）支付卡数据（PCI-DSS）
* *检测模式:* *
-`card_number`,`pan`,`primary_account_number`,`credit_card`,`debit_card`-`cvv`,`cvc`,`cvv2`,`card_verification`,`security_code`-`card_expiry`,`expiration_date`,`exp_date`,`expiry_month`,`expiry_year`-`cardholder_name`,`card_holder`-`iban`,`bic`,`swift_code`,`routing_number`,`account_number`,`sort_code`-`bank_account`,`bank_details`,`wire_transfer`**值的正则表达式模式```
Visa:            \b4[0-9]{12}(?:[0-9]{3})?\b
Mastercard:      \b5[1-5][0-9]{14}\b
Amex:            \b3[47][0-9]{13}\b
Generic PAN:     \b[0-9]{13,19}\b (in a PAN-named field)
CVV:             \b[0-9]{3,4}\b (in a cvv-named field)
IBAN:            \b[A-Z]{2}\d{2}[A-Z0-9]{4}\d{7}([A-Z0-9]?){0,16}\b
```
身份组合（组合时重新识别的风险很高）
**组合在一起构成第2层：**
-全名+出生日期
-全名+地址（街道层）
-邮箱+出生日期+性别
—电话号码+地址

* *检测模式:* *
-`full_name`,`first_name`+`last_name`（作为单独的字段-注意两者都存在）
-`date_of_birth`,`dob`,`birth_date`,`birthdate`,`birthday`-`home_address`,`street_address`,`address_line1`,`postal_address`-`gender`,`sex`,`pronoun`（与其他标识符组合时）

---

3级-高（监管通知触发）

###联系信息
* *检测模式:* *
-`email`,`email_address`,`user_email`,`contact_email`,`primary_email`-`phone`,`phone_number`,`mobile`,`mobile_number`,`cell_phone`,`telephone`-`whatsapp_number`,`signal_number`* *正则表达式模式:* *```
Email:  \b[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Z|a-z]{2,}\b
Phone:  \+?[0-9\s\-\(\)]{7,20}  (in a phone-named field)
```
精确的位置数据
* *检测模式:* *
-`latitude`,`longitude`,`lat`,`lng`,`lat_lng`,`coordinates`,`geo_point`-`gps_location`,`precise_location`,`real_time_location`-`home_location`、`work_location`**注：**城市位置为Tier 4；街道或GPS坐标是第3层。

网络标识符
* *检测模式:* *
-`ip_address`,`ip`,`client_ip`,`remote_addr`,`x_forwarded_for`-`mac_address`,`device_mac`,`hardware_id`-`imei`,`imsi`,`device_id`,`advertising_id`,`idfa`,`gaid`认证工件
* *检测模式:* *
-`session_id`,`cookie_value`,`csrf_token`（如果长寿命和用户识别）
-`remember_me_token`,`persistent_session`---

##第4层-提升（与隐私相关）

部分个人标识符
* *检测模式:* *
-`first_name`,`last_name`,`display_name`,`username`（单独使用时）
-`profile_picture`,`avatar_url`-`city`,`state`,`country`,`region`,`zip_code`,`postal_code`-`time_zone`,`locale`,`language_preference`行为和分析数据
* *检测模式:* *
-`user_agent`,`browser`,`device_type`,`os`-`search_query`,`search_history`,`browsing_history`—`purchase_history`、`order_history`、`transaction_history`—`click_event`、`page_view`、`session_duration`-`preferences`,`interests`,`tags`,`segments`财务背景（非卡）
* *检测模式:* *
-`salary`,`income`,`net_worth`,`credit_score`,`credit_rating`-`account_balance`,`wallet_balance`,`subscription_tier`---

5级-标准（对隐私没有直接影响）

-系统配置值（非保密）
-面向公众的内容（博客文章，公众简介）
—匿名聚合统计
-非个人参考资料（产品目录、国家代码）
-没有外部暴露的内部系统标识符

---

AI分析的检测指南

框架特定模式

**Django / Python:**```python
# Sensitive fields typically appear in models.py
class User(models.Model):
    email = models.EmailField()           # Tier 3
    date_of_birth = models.DateField()    # Tier 2 (combined with name)
    ssn = models.CharField(max_length=11) # Tier 1
```
TypeScript / Prisma:**```prisma
model User {
  email       String    // Tier 3
  phoneNumber String?   // Tier 3
  dateOfBirth DateTime? // Tier 2 (when combined)
  cardNumber  String?   // Tier 2 PCI-DSS
}
```
**Java / Spring / JPA:**```java
@Entity
public class Patient {
    @Column(name = "diagnosis")  // Tier 1 PHI
    private String diagnosis;
    
    @Column(name = "ssn")        // Tier 1
    private String ssn;
}
```
c# / EF Core:**```csharp
public class UserProfile {
    public string Email { get; set; }        // Tier 3
    public string PassportNumber { get; set; } // Tier 1
    public DateTime DateOfBirth { get; set; }  // Tier 2
}
```
日志语句模式（高风险-经常被忽视）```python
# BAD — logs PII
logger.info(f"User {user.email} logged in from {request.remote_addr}")
logger.debug(f"Payment for card {card_number}")

# Look for these in logging calls:
# .info(), .debug(), .warn(), .error(), console.log(), System.out.println()
```
API响应泄漏（Serializer/DTO模式）```typescript
// Check if these fields are included in response objects
// even if not requested — over-fetching is a common exposure vector
{
  "id": "...",
  "email": "...",          // Tier 3
  "phone": "...",          // Tier 3 
  "dateOfBirth": "...",    // Tier 2 — should this be returned?
  "passwordHash": "...",   // Tier 1 — should NEVER be returned
  "ssn": "...",            // Tier 1 — should NEVER be returned
}
```
---

汇总风险评估

组合攻击-数据在组合时变得更加敏感：

|单独|合并|合并分级|风险||-------|--------------|---------------|------|
| Email (T3) |密码哈希（T1） | T1 |帐户接管|
| Name (T4) | DOB (T2) + Address (T2) | T2 |全身份重构|
| IP地址（T3） |时间戳+用户ID | T2 |行为分析|
|城市（T4） |购买历史（T4） | T3 |去匿名化风险|
|健康类别（T4） |名称+电子邮件| T1 |触发HIPAA的|

**规则：**总是组合评估字段，而不是单独评估。