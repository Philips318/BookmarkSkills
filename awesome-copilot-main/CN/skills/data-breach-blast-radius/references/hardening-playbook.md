#加固攻略

优先控制，以减少数据泄露爆炸半径。控制按影响类别组织，并包括特定于技术堆栈的实现模式。每个控制包括一个**爆炸半径减少估计**。

> **使用方法：**确定暴露向量后，将每个向量与下面的控件匹配。按`(Blast_Radius_Reduction × Severity) / Effort`对加固路线图进行排序。

---

控制优先级矩阵

|优先级|控制|爆炸半径减小|努力|类别||----------|---------|----------------------|--------|---------|
| P0 |修复IDOR/BOLA-添加所有权检查| 90%为受影响的向量|低|授权|
| P0 |从API响应中删除敏感字段| 85%受影响的字段| Low |数据最小化|
| P0 |撤销公共访问存储（S3/Blob） | 100%受影响的存储|低|访问控制|
| P0 |从code/logs| 100%删除受影响的秘密|低|秘密|明文凭证
| P1 | T1数据加字段级加密|加密字段80% | Medium |加密|
| P1 |Mask/tokenizePCI卡数据| 95%卡曝光|中等|标记化|
| P1 |从日志语句中删除PII | 70%的日志暴露| Medium |日志|
| P1 |对未认证的端点添加认证| 95%暴露的端点| Low |认证|
| P2 |执行数据访问审计日志记录| -50%检测时间|中等|监控|
| P2 |开启数据库活动监控| -60% de保护时间|中等|监控|
| P2 |敏感端点限速|数据采集减少60% |低|限速|
| P2 | T2敏感数据列级加密|加密列70% | Medium |加密|
| P3 |实现数据保留+自动删除|陈旧数据曝光减少40% |高|数据生命周期|
| P3 |从生产PII中分离分析存储| 60%用于分析漏洞|高|架构|
| P3 |假名化行为跟踪数据| 70%行为数据| Medium |假名化|---

## P0 -立即修复（< 1天）

# # # 1。修正授权：IDOR / BOLA

**修复的内容：**对象级别授权-用户可以通过更改ID访问其他用户的数据。

**代码中的检测模式：**```python
# VULNERABLE — no ownership check
@app.get("/api/orders/{order_id}")
def get_order(order_id: int):
    return db.query(Order).filter(Order.id == order_id).first()

# SECURE — ownership check
@app.get("/api/orders/{order_id}")
def get_order(order_id: int, current_user: User = Depends(get_current_user)):
    order = db.query(Order).filter(
        Order.id == order_id,
        Order.user_id == current_user.id  # ownership check
    ).first()
    if not order:
        raise HTTPException(status_code=404)
    return order
```

```typescript
// VULNERABLE
app.get('/api/users/:id/profile', authenticate, async (req, res) => {
  const user = await User.findById(req.params.id);
  res.json(user);
});

// SECURE
app.get('/api/users/:id/profile', authenticate, async (req, res) => {
  if (req.params.id !== req.user.id && !req.user.isAdmin) {
    return res.status(403).json({ error: 'Forbidden' });
  }
  const user = await User.findById(req.params.id);
  res.json(user);
});
```

```csharp
// VULNERABLE
[HttpGet("orders/{orderId}")]
public async Task<IActionResult> GetOrder(int orderId)
{
    var order = await _db.Orders.FindAsync(orderId);
    return Ok(order);
}

// SECURE
[HttpGet("orders/{orderId}")]
[Authorize]
public async Task<IActionResult> GetOrder(int orderId)
{
    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    var order = await _db.Orders
        .Where(o => o.Id == orderId && o.UserId == userId)
        .FirstOrDefaultAsync();
    if (order == null) return NotFound();
    return Ok(order);
}
```
---

# # # 2。从API响应中删除敏感字段

**过度抓取- api返回的数据比客户端需要的多。

* *模式:* *```typescript
// VULNERABLE — returns all fields including passwordHash, ssn
const user = await User.findById(id);
res.json(user);

// SECURE — explicit projection
const user = await User.findById(id).select('id name email createdAt');
res.json(user);
```

```python
# SECURE — Pydantic response model (FastAPI)
class UserPublicResponse(BaseModel):
    id: int
    name: str
    email: str
    # NOTE: password_hash, ssn, date_of_birth NOT included

@app.get("/api/users/{id}", response_model=UserPublicResponse)
def get_user(id: int):
    return db.query(User).filter(User.id == id).first()
```

```java
// SECURE — DTO with @JsonIgnore
public class UserResponse {
    public String id;
    public String name;
    public String email;
    // passwordHash, ssn not included in DTO
}
```
---

# # # 3。从代码中删除明文凭据

* *检测模式:* *```
# Patterns to search for in all files:
password\s*=\s*["'][^"']+["']
api_key\s*=\s*["'][^"']+["']
secret\s*=\s*["'][^"']+["']
token\s*=\s*["'][^"']+["']
connectionString\s*=\s*["'][^"']+["']
```
* *修复模式:* *```python
# VULNERABLE
DATABASE_URL = "postgresql://user:p@ssw0rd@prod-db.example.com/mydb"

# SECURE
import os
DATABASE_URL = os.environ.get("DATABASE_URL")
# In production: use Azure Key Vault, AWS Secrets Manager, or GCP Secret Manager
```
---

## P1 -修复本周

# # # 4。Tier 1数据的字段级加密

在**存储敏感字段之前**加密。加密密钥存在于KMS中，而不是数据库中。

**Python / SQLAlchemy + Azure密钥库：**```python
from azure.keyvault.secrets import SecretClient
from cryptography.fernet import Fernet

# Encrypt at write time
def encrypt_field(value: str, key: bytes) -> str:
    f = Fernet(key)
    return f.encrypt(value.encode()).decode()

# Decrypt at read time (only when authorized)
def decrypt_field(encrypted_value: str, key: bytes) -> str:
    f = Fernet(key)
    return f.decrypt(encrypted_value.encode()).decode()
```
**Node.js/ Prisma + AWS KMS:**```typescript
import { KMSClient, EncryptCommand, DecryptCommand } from "@aws-sdk/client-kms";

const kms = new KMSClient({ region: "us-east-1" });

async function encryptField(plaintext: string): Promise<string> {
  const { CiphertextBlob } = await kms.send(new EncryptCommand({
    KeyId: process.env.KMS_KEY_ARN,
    Plaintext: Buffer.from(plaintext),
  }));
  return Buffer.from(CiphertextBlob!).toString('base64');
}
```
** c# / EF Core + Azure密钥库：**```csharp
// Use Always Encrypted for SQL Server / Azure SQL
// Or manually encrypt with Azure Key Vault
services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString, sqlOptions =>
        sqlOptions.EnableSensitiveDataLogging(false)));

// In entity:
[Column(TypeName = "nvarchar(500)")]
public string EncryptedSsn { get; set; } // store Base64 ciphertext
```
**必须加密的字段（Tier 1）：**
- SSN /国民身份证号码
-护照号码
-完整的支付卡号码（更好：使用标记化，见下文）
-医疗记录数据/诊断
-生物识别模板

---

# # # 5。令牌化支付卡数据

**不要存储完整的卡号。**使用pci兼容的保险库代替。

* *建议提供者:* *
-条纹（通过Elements/PaymentIntents标记-你永远不会触摸卡号）
- Braintree / PayPal
——Adyen
平方

* *模式:* *```typescript
// CORRECT — use Stripe's tokenization
const paymentMethod = await stripe.paymentMethods.create({
  type: 'card',
  card: { token: cardToken }, // token from client-side Stripe.js
});
// Store: paymentMethod.id (token) — never the card number

// WRONG — never do this
const cardNumber = req.body.cardNumber; // Tier 2 PCI-DSS violation
await db.save({ userId, cardNumber });   // DO NOT store raw card data
```
---

# # # 6。从日志语句中删除PII

要搜索和修复的模式：**```python
# VULNERABLE
logger.info(f"User {user.email} logged in")
logger.debug(f"Payment by {user.full_name}, card ending {card_last4}")

# SECURE — log opaque identifiers, not PII
logger.info(f"User {user.id} authenticated", extra={"user_id": user.id})
logger.debug(f"Payment processed", extra={"user_id": user.id, "payment_id": payment_id})
```

```typescript
// VULNERABLE
console.log(`Processing order for ${user.email} at ${user.address}`);

// SECURE
logger.info('Processing order', { userId: user.id, orderId: order.id });
```
**安全的结构化日志字段：**
-内部用户ID （UUID/opaque）
-会话ID（如果是短暂的且没有外部共享）
-Transaction/correlationid
—错误码和错误类型
——时间戳
- HTTP状态码- Duration/latency
**不安全的结构化日志字段
-电邮地址
—IP地址（必须为掩码）—最后8位
-全名
-电话号码
-任何1-3级敏感字段

---

修复这个Sprint

# # # 7。实现数据访问审计日志

Tier 1和Tier 2数据的每个read/write必须记录到不可变的审计日志中。

**要记录什么：**```
{
  timestamp: ISO8601,
  actor_id: "user UUID",
  actor_role: "admin|user|service",
  action: "READ|WRITE|DELETE|EXPORT",
  resource_type: "User|HealthRecord|PaymentMethod",
  resource_id: "UUID of accessed record",
  fields_accessed: ["email", "phone"],  // NOT the values
  ip_address: "masked IP",
  result: "success|denied",
  correlation_id: "request trace ID"
}
```
**不记录审计日志中敏感字段的实际值

**分离：**将审计日志存储在**独立的**database/storage帐户中，该帐户具有比应用程序数据库更严格的访问控制。

---

# # # 8。速率限制敏感端点

防止自动批量数据收集，即使存在身份验证漏洞。```typescript
// Express + express-rate-limit
import rateLimit from 'express-rate-limit';

// Aggressive limit for data export endpoint
const exportLimiter = rateLimit({
  windowMs: 60 * 60 * 1000, // 1 hour
  max: 5, // max 5 exports per hour per IP
  message: 'Too many export requests'
});

// Standard limit for data lookup
const lookupLimiter = rateLimit({
  windowMs: 15 * 60 * 1000, // 15 minutes
  max: 100
});

app.get('/api/export', exportLimiter, authMiddleware, exportController);
app.get('/api/users/:id', lookupLimiter, authMiddleware, userController);
```
---

修复本季度

# # # 9。实现数据保留和自动删除

**每个有个人资料的表格必须有明确的保留政策```sql
-- Add retention column to all PII tables
ALTER TABLE users ADD COLUMN retention_expires_at TIMESTAMP;
ALTER TABLE health_records ADD COLUMN retention_expires_at TIMESTAMP;

-- Set retention at insert time
INSERT INTO users (email, retention_expires_at) 
VALUES ($1, NOW() + INTERVAL '7 years');

-- Scheduled job to hard-delete expired records (or anonymize)
DELETE FROM users 
WHERE retention_expires_at < NOW() 
AND deletion_notified_at IS NOT NULL; -- ensure user was notified
```
**Python计划清理：**```python
from apscheduler.schedulers.asyncio import AsyncIOScheduler

async def purge_expired_records():
    await db.execute(
        "DELETE FROM user_sessions WHERE expires_at < NOW()"
    )
    # Anonymize users (don't delete if financial records must be retained)
    await db.execute("""
        UPDATE users SET 
            email = CONCAT('deleted_', id, '@redacted.invalid'),
            phone = NULL,
            address = NULL,
            date_of_birth = NULL
        WHERE retention_expires_at < NOW() AND deleted_at IS NULL
    """)

scheduler = AsyncIOScheduler()
scheduler.add_job(purge_expired_records, 'cron', hour=2)  # 2 AM daily
scheduler.start()
```
---

# # # 10。假名化行为和分析数据

用假名令牌替换分析中的直接用户标识符。```python
import hashlib
import hmac

PSEUDONYM_SALT = os.environ.get("PSEUDONYM_SALT")  # stored in Key Vault

def pseudonymize_user_id(real_user_id: str) -> str:
    """
    One-way: analyst can track behavior across sessions 
    but cannot identify the real user without the salt.
    """
    return hmac.new(
        PSEUDONYM_SALT.encode(), 
        real_user_id.encode(), 
        hashlib.sha256
    ).hexdigest()

# In analytics event
analytics.track({
    "user_id": pseudonymize_user_id(user.id),  # NOT real user ID
    "event": "page_viewed",
    "page": request.path,
    "timestamp": datetime.utcnow().isoformat()
})
```
---

快速获胜清单（1天内完成）

-[]搜索所有硬编码的秘密文件→移动到env vars / Key Vault
-[]检查所有`SELECT *`查询→添加显式列列表，不包括敏感字段
—[]验证存储buckets/containers→禁止公众访问
-[]删除打印请求正文的`console.log`/`logger.debug`调用
-[]添加`HttpOnly; Secure; SameSite=Strict`到所有的会话cookie
-[]确认`/api/admin/*`路由是否需要检查admin角色
—[]确认密码重置令牌有效期小于15分钟
-[]检查500个错误响应在生产中不包括堆栈跟踪
-[]验证`.env`和secret文件是否在`.gitignore`中
-[]执行`git log --all --full-history -- "*.env"`命令，检查历史秘密提交

---

通过控制减小爆炸半径

在报告加固路线图时，使用以下估计：

|控制应用|爆炸半径减小|合理化||----------------|----------------------|---------------|
|修复所有IDOR漏洞| 80-90% |大多数违规场景利用授权漏洞|
| T1数据字段加密| 75-85% |没有KMS密钥|加密数据无效
|从日志中删除PII | 40-60% |日志访问通常比DB访问控制少|
|令牌化支付数据| 95%用于卡数据|标准PCI-DSS兼容性消除了卡数据范围|
|速率限制数据端点| 30-50% |限制自动采集攻击规模|
|数据保留强制| 20-40% |减少“数据湖”效应-更少的数据窃取|
|审计日志+异常检测| 0%预防，但检测时间-60% |违规被发现更快|
|分析数据的假名化| 60-70% |分析数据与身份分离|
|架构：将分析与PII分离| 50-70% |分析存储的破坏没有PII值|