---
applyTo: '**'
description: 'Comprehensive secure coding standards based on OWASP Top 10 2025, with 55+ anti-patterns, detection regex, framework-specific fixes for modern web and backend frameworks, and AI/LLM security guidance.'
---
#安全标准

web应用程序开发的综合安全规则。每个反模式都包括严重性分类、检测方法、OWASP 2025参考和纠正代码示例。

* *严重性级别:* *

- **CRITICAL** -可利用的漏洞。必须在合并前修复。
- **重要** -有重大风险。应该固定在同一个sprint中。
- **建议** -纵深防守改进。为将来的迭代做计划。

---

OWASP Top 10 - 2025快速参考

| # |类别|密钥缓解||---|----------|----------------|
| A01 |损坏访问控制|每个端点上的认证中间件，RBAC，所有权检查|
|安全头，在prod中没有调试，没有默认凭证|
| A03 |软件供应链故障*(NEW)* |`npm audit`， lockfile完整性，SBOM， SLSA来源|
|Argon2id/bcrypt用于密码，TLS无处不在，代码|中没有秘密
参数化查询，输入验证，没有原始HTML与用户输入|
|不安全设计|威胁建模，安全设计模式，滥用案例测试|
| A07 |认证失败|限速登录、安全会话管理、MFA |
| A08 |软件或数据完整性故障| CDN脚本的SRI，签名工件，无不安全反序列化|
| A09 |安全日志和告警故障|日志安全事件，日志中无PII，关联id，活动告警|
b| A10 b|错了异常条件的ing *(NEW)* |处理所有错误，在prod中没有堆栈跟踪，故障安全|---

注入反模式（I1-I8）

### 1: SQL注入通过字符串连接

—**严重性**：紧急
- **检测**:`\$\{.*\}.*(?:SELECT|INSERT|UPDATE|DELETE|FROM|WHERE)`- ** owasp **: a05```typescript
// BAD
const unsafeResult = await db.query(`SELECT * FROM users WHERE id = ${userId}`);

// GOOD — parameterized query
const safeResult = await db.query('SELECT * FROM users WHERE id = $1', [userId]);
```
### I2: NoSQL注入（MongoDB Operator注入）

—**严重性**：紧急
—**检测**:`\{\s*\$(?:gt|gte|lt|lte|ne|in|nin|regex|where|exists)`- ** owasp **: a05```typescript
// BAD — attacker sends { "password": { "$gt": "" } }
const user = await User.findOne({ username: req.body.username, password: req.body.password });

// GOOD — validate and cast input types
const username = String(req.body.username);
const password = String(req.body.password);
const user = await User.findOne({ username });
const valid = user && await verifyPassword(user.passwordHash, password);
```
### I3：命令注入（执行与用户输入）

—**严重性**：紧急
—**检测**:`(?:exec|execSync|execFile|execFileSync)\s*\(.*(?:req\.|params\.|query\.|body\.)`- ** owasp **: a05```typescript
// BAD — shell interpolation, sync call blocks the event loop
import { execFileSync } from 'node:child_process';
const unsafeOutput = execFileSync('sh', ['-c', `ls -la ${req.query.dir}`]);

// GOOD — async execFile, arguments array, no shell, bounded time/output
import { execFile } from 'node:child_process';
import { promisify } from 'node:util';
const pExecFile = promisify(execFile);

const dir = String(req.query.dir ?? '');
if (!dir || dir.startsWith('-')) throw new Error('Invalid directory');
const { stdout: safeOutput } = await pExecFile('ls', ['-la', '--', dir], {
  timeout: 5_000,      // fail fast on hung processes
  maxBuffer: 1 << 20,  // 1 MiB cap to prevent memory exhaustion
});

// BEST — allowlist validation on top of the async, bounded call above
const allowedDirs = ['/data', '/public'];
if (!allowedDirs.includes(dir)) throw new Error('Invalid directory');
```
在服务器处理程序中，首选异步`execFile`/`spawn`而不是`execFileSync`：同步变体阻塞了Node的事件循环，并可以放大DoS的影响。总是将`timeout`和`maxBuffer`传递给绑定执行。

### I4: XSS通过未经处理的HTML渲染

—**严重性**：紧急
- **检测**:`(?:v-html|\[innerHTML\]|dangerouslySetInner|bypassSecurityTrust)`- ** owasp **: a05

适用于所有前端框架。每个都有一个绕过默认XSS保护的API：

- **React**:`dangerouslySetInnerHTML`prop与原始用户内容
**Angular**:`[innerHTML]`绑定或`bypassSecurityTrustHtml`未消毒输入
**Vue**:`v-html`指令与用户控制的内容```typescript
// GOOD — sanitize with DOMPurify before rendering any raw HTML
import DOMPurify from 'dompurify';
const clean = DOMPurify.sanitize(userContent);

// BEST — use text interpolation when HTML is not needed
// React:   {userContent}
// Angular: {{ userContent }}
// Vue:     {{ userContent }}
```
### I5: SSRF通过用户控制的url

—**严重性**：紧急
—**检测**:`fetch\((?:req\.|params\.|query\.|body\.|url|href)`- ** owasp **: a01```typescript
// BAD
const data = await fetch(req.body.url);

// GOOD — scheme allowlist + hostname allowlist + DNS/IP validation (see TOCTOU note)
import { promises as dns } from 'node:dns';

function isPrivateIP(ip: string): boolean {
  // Normalize IPv4-mapped IPv6 (e.g., ::ffff:127.0.0.1 → 127.0.0.1)
  const normalized = ip.startsWith('::ffff:') ? ip.slice(7) : ip;
  // IPv4 private/reserved/loopback ranges
  if (/^(10\.|172\.(1[6-9]|2\d|3[01])\.|192\.168\.|127\.|0\.|169\.254\.)/.test(normalized)) return true;
  // IPv6 loopback, link-local (fe80::/10), and unique-local
  if (/^(::1|fe[89ab]|fc|fd)/i.test(normalized)) return true;
  return false;
}

const parsed = new URL(req.body.url);
if (parsed.protocol !== 'https:') throw new Error('Only HTTPS allowed');
const allowedHosts = ['api.example.com', 'cdn.example.com'];
if (!allowedHosts.includes(parsed.hostname)) throw new Error('Host not allowed');
// Resolve all A/AAAA records to prevent DNS rebinding via multiple IPs
const resolved = await dns.lookup(parsed.hostname, { all: true });
if (resolved.length === 0 || resolved.some(({ address }) => isPrivateIP(address))) {
  throw new Error('Private or reserved IPs not allowed');
}
// Note: for production, pin the resolved IP in the HTTP client to prevent
// TOCTOU rebinding between this check and fetch(). See undici Agent docs.
const data = await fetch(parsed.toString(), { redirect: 'error' });
```
### 16：文件操作中的路径遍历

—**严重性**：紧急
—**检测**:`(?:readFile|readFileSync|createReadStream|path\.join)\s*\(.*(?:req\.|params\.|query\.|body\.)`- ** owasp **: a01```typescript
// BAD
const file = fs.readFileSync(`/data/${req.params.filename}`);

// GOOD — resolve and validate within allowed directory
import path from 'path';
const basePath = '/data';
const filePath = path.resolve(basePath, req.params.filename);
if (!filePath.startsWith(basePath + path.sep)) throw new Error('Path traversal detected');
const file = fs.readFileSync(filePath);
```
### I7：模板注入

—**严重性**：紧急
—**检测**:`(?:render|compile|template)\s*\(.*(?:req\.|params\.|query\.|body\.)`- ** owasp **: a05```typescript
// BAD — user input as template source
const html = ejs.render(req.body.template, data);

// GOOD — predefined templates, user input only as data
const html = ejs.renderFile('./templates/page.ejs', { content: req.body.content });
```
XXE注入（XML外部实体）

—**严重性**：紧急
—**检测**:`(?:parseXml|DOMParser|xml2js|libxmljs).*(?:req\.|body\.|file)`- ** owasp **: a05```typescript
// GOOD — disable external entities in XML parser
import { XMLParser } from 'fast-xml-parser';
const parser = new XMLParser({
  allowBooleanAttributes: true,
  processEntities: false,
  htmlEntities: false,
});
const result = parser.parse(req.body.xml);
```
---

认证反模式（AU1-AU8）

### AU1: JWT算法混乱（alg:none）

—**严重性**：紧急
—**检测**:`jwt\.verify\((?![^)]*\balgorithms\b)[^)]*\)`- ** owasp **: a07```typescript
// BAD — accepts any algorithm including "none"
const decoded = jwt.verify(token, secret);

// GOOD — enforce specific algorithm
const decoded = jwt.verify(token, publicKey, { algorithms: ['RS256'] });
```
### AU2: JWT没有过期检查

—**严重性**：紧急
—**检测**:`jwt\.sign\((?![^)]*\b(?:expiresIn|exp)\b)[^)]*\)`- ** owasp **: a07```typescript
// BAD — token never expires
const token = jwt.sign({ userId: user.id }, secret);

// GOOD — short-lived token
const token = jwt.sign({ userId: user.id }, secret, { expiresIn: '15m' });
```
AU3: JWT存储在localStorage中

- **严重性**：重要
—**检测**:`localStorage\.setItem\(.*(?:token|jwt|auth|session)`- ** owasp **: a07```typescript
// BAD — accessible via XSS
localStorage.setItem('accessToken', token);

// GOOD — httpOnly cookie set by server
res.cookie('token', token, { httpOnly: true, secure: true, sameSite: 'strict' });
```
### AU4：明文/快速哈希密码（MD5/SHA-1/SHA-256）

—**严重性**：紧急
- **检测**:`(?:createHash|md5|sha1|sha256)\s*\(.*password`- ** owasp **: a04```typescript
// BAD — fast hash, no salt
const sha256Hash = crypto.createHash('sha256').update(password).digest('hex');

// GOOD — Argon2id (OWASP recommended)
import { hash as argon2Hash, argon2id } from 'argon2';
const hashed = await argon2Hash(password, { type: argon2id, memoryCost: 65536, timeCost: 3 });
```
AU5：登录时缺少暴力破解保护

—**严重性**：紧急
—**检测**:`(?:post|router\.post)\s*\(\s*['"]\/(?:login|signin|auth|register|reset)`- ** owasp **: a07```typescript
// BAD — no rate limiting
app.post('/api/auth/login', loginHandler);

// GOOD
import rateLimit from 'express-rate-limit';
const authLimiter = rateLimit({ windowMs: 15 * 60 * 1000, max: 5 });
app.post('/api/auth/login', authLimiter, loginHandler);
```
AU6：登录时缺少会话再生（会话固定）

- **严重性**：重要
—**检测**:`(?:session|req\.session)\s*\.\s*(?:userId|user|authenticated)\s*=`- ** owasp **: a07```typescript
// GOOD — regenerate session ID on successful login to prevent fixation
req.session.regenerate((err) => {
  if (err) return next(err);
  req.session.userId = user.id;
  req.session.save(next);
});
```
相关：在密码更改或提升时，也使该用户的所有其他活动会话无效（例如，通过碰撞`tokenVersion`列并拒绝具有过时版本的会话，或通过迭代会话存储并销毁与该用户相关的项）。

### AU7: OAuth没有状态参数

—**严重性**：紧急
- **检测**:`authorize\?(?![^\n#]*\bstate=)[^\n#]*`- ** owasp **: a07```typescript
// GOOD — include state parameter for CSRF protection
const state = crypto.randomBytes(32).toString('hex');
session.oauthState = state;
const authUrl = `https://provider.com/authorize?client_id=${clientId}&redirect_uri=${redirectUri}&state=${state}`;
```
### AU8：公共OAuth客户端缺少PKCE

- **严重性**：重要
—**检测**:`(?:authorization_code|code).*(?!.*code_challenge)`- ** owasp **: a07

对所有公共客户端（spa，移动）使用PKCE（代码交换证明密钥）和S256挑战方法。

---

授权反模式（AZ1-AZ6）

AZ1：在新端点上缺少认证中间件

—**严重性**：紧急
- **检测**:`(?:app|router)\.\w+\s*\(\s*['"]\/api\/(?:admin|users|settings)`- ** owasp **: a01```typescript
// BAD
router.delete('/api/users/:id', deleteUser);

// GOOD
router.delete('/api/users/:id', authenticate, authorize('admin'), deleteUser);
```
AZ2：仅客户端授权

—**严重性**：紧急
- **检测**：组件保护没有服务器端检查
- ** owasp **: a01

前端守卫仅用于用户体验。始终在服务器上进行验证。

### AZ3: IDOR（不安全的直接对象引用）

—**严重性**：紧急
- **检测**:`params\.(?:id|userId|orderId)`无归属检查
- ** owasp **: a01```typescript
// GOOD — verify ownership
router.get('/api/orders/:orderId', authenticate, async (req, res) => {
  const order = await Order.findById(req.params.orderId);
  if (!order || order.userId !== req.user.id) {
    return res.status(404).json({ error: 'Not found' });
  }
  res.json(order);
});
```
### AZ4：质量分配

—**严重性**：紧急
—**检测**:`(?:create|update|findOneAndUpdate)\s*\(\s*req\.body\s*\)`- ** owasp **: a01```typescript
// BAD
await User.findByIdAndUpdate(id, req.body);

// GOOD — explicitly pick allowed fields
const { name, email, avatar } = req.body;
await User.findByIdAndUpdate(id, { name, email, avatar });
```
### AZ5：通过角色参数进行权限升级

—**严重性**：紧急
—**检测**:`req\.body\.role|req\.body\.isAdmin|req\.body\.permissions`- ** owasp **: a01```typescript
// GOOD — ignore role from input
const { name, email, password } = req.body;
const user = await User.create({ name, email, password, role: 'user' });
```
### AZ6：缺少敏感操作的重新认证

- **严重性**：重要
- **检测**:`(?:delete|destroy|remove).*(?:account|user|organization)`不重新认证
- ** owasp **: a01

删除帐号、修改邮箱等敏感操作需要输入当前密码。

---

反模式（S1-S6）

硬编码API密钥/令牌

—**严重性**：紧急
- **检测**:`(?:password|secret|api_key|token|apiKey)\s*[:=]\s*['"][A-Za-z0-9+/=]{8,}['"]`- ** owasp **: a04```typescript
// BAD
const API_KEY = 'sk_live_abc123def456';

// GOOD
const API_KEY = process.env.API_KEY;
```
### s2：。env承诺Git

—**严重性**：紧急
- **检测**:`git ls-files .env`（应该返回空）
- ** owasp **: a04```gitignore
# .gitignore
.env
.env.local
.env.*.local
*.pem
*.key
```
S3：服务器机密暴露给客户端

—**严重性**：紧急
—**检测**:`NEXT_PUBLIC_.*(?:SECRET|PRIVATE|PASSWORD|KEY(?!.*PUBLIC))`- ** owasp **: a02```bash
# BAD
NEXT_PUBLIC_DATABASE_URL=postgresql://...

# GOOD
DATABASE_URL=postgresql://...
NEXT_PUBLIC_API_URL=https://api.example.com
```
Angular：不要把`environment.ts`文件中的秘密文件捆绑到客户端。

### S4：默认凭证配置

—**严重性**：紧急
- **检测**:`(?:admin|root|default|test).*(?:password|pass|pwd)\s*[:=]\s*['"](?:admin|root|password|1234|test)`- ** owasp **: a02

使用带有验证的环境变量（zod模式）。

### S5:CI/CD管道日志中的秘密

- **严重性**：重要
- **检测**:`(?:echo|console\.log|print).*(?:\$SECRET|\$TOKEN|\$PASSWORD|process\.env)`- ** owasp **: a09

在CI中使用隐藏的秘密。永远不要回显包含秘密的环境变量。

### S6：错误响应/堆栈跟踪中的敏感数据

- **严重性**：重要
- **检测**:`(?:stack|trace|query|sql).*(?:res\.json|res\.send|c\.JSON)`- ** owasp **: a10```typescript
// GOOD — generic error to client, details only in logs
app.use((err, req, res, _next) => {
  logger.error({ err, path: req.path, method: req.method });
  const isDev = process.env.NODE_ENV === 'development';
  res.status(500).json({
    error: 'Internal Server Error',
    ...(isDev && { message: err.message }),
  });
});
```
---

## header反模式（H1-H8）

### H1：缺少Content-Security-Policy

- **严重性**：重要
- **检测**：缺少`Content-Security-Policy`报头
- ** owasp **: a02

### H2: CSP不安全内联和不安全eval

- **严重性**：重要
- **检测**:`Content-Security-Policy.*(?:'unsafe-inline'|'unsafe-eval')`- ** owasp **: a02

使用基于nonce的CSP:`script-src 'self' 'nonce-{SERVER_GENERATED}'`### H3：缺少严格传输安全

- **严重性**：重要
- **检测**：缺少`Strict-Transport-Security`报头
- ** owasp **: a02

价值:`max-age=31536000; includeSubDomains; preload`### H4：缺少X-Content-Type-Options

- **严重性**：重要
- **检测**：不存在`X-Content-Type-Options: nosniff`- ** owasp **: a02

### H5：缺少x帧选项

- **严重性**：重要
- **检测**：缺少`X-Frame-Options`报头
- ** owasp **: a02

值:`DENY`。同时设置`Content-Security-Policy: frame-ancestors 'none'`。

### H6: Permissive referer - policy

—**严重性**：建议
- **检测**:`Referrer-Policy.*(?:unsafe-url|no-referrer-when-downgrade)`- ** owasp **: a02

用途:`strict-origin-when-cross-origin`### H7：缺少Permissions-Policy—**严重性**：建议
- **检测**：缺少`Permissions-Policy`报头
- ** owasp **: a02

价值:`camera=(), microphone=(), geolocation=(), payment=()`### H8：带有凭据的CORS通配符

—**严重性**：紧急
- **检测**:`(?:cors|Access-Control-Allow-Origin).*\*`- ** owasp **: a02```typescript
// GOOD
app.use(cors({
  origin: ['https://app.example.com', 'https://staging.example.com'],
  credentials: true,
}));
```
---

前端反模式（FE1-FE8）

FE1：未经处理的HTML渲染

—**严重性**：紧急
- **检测**：不含DOMPurify的`(?:innerHTML|v-html|dangerouslySetInner)`- ** owasp **: a05

在呈现用户控制的HTML之前，始终使用DOMPurify进行清理。看到预告。

FE2：基于用户输入的动态代码评估

—**严重性**：紧急
- **检测**:`eval\s*\(`- ** owasp **: a05

使用结构化数据解析器（JSON.parse）代替。

### FE3: postMessage没有来源验证

- **严重性**：重要
- **检测**:`addEventListener\s*\(\s*['"]message['"].*(?!.*origin)`- ** owasp **: a01```typescript
window.addEventListener('message', (event) => {
  if (event.origin !== 'https://trusted.example.com') return;
  processData(event.data);
});
```
FE4：原型污染

- **严重性**：重要
—**检测**:`(?:__proto__|constructor\.prototype|Object\.assign)\s*.*(?:req\.|body\.|query\.)`- ** owasp **: a05

在合并到对象之前，验证并过滤用户输入中的键。

### FE5：打开重定向

- **严重性**：重要
- **检测**:`(?:window\.location|location\.href|router\.push)\s*=\s*(?:req\.|params\.|query\.)`- ** owasp **: a01```typescript
// GOOD — relative paths only
const redirect = new URLSearchParams(window.location.search).get('redirect');
if (redirect?.startsWith('/') && !redirect.startsWith('//')) {
  window.location.href = redirect;
}
```
### FE6：敏感数据在localStorage

- **严重性**：重要
—**检测**:`localStorage\.setItem\(.*(?:token|session|credit|ssn|password)`- ** owasp **: a07

使用httpOnly cookie作为令牌。

### FE7：缺少CSRF令牌

- **严重性**：重要
- **检测**:POST/PUT/DELETE表单没有CSRF令牌或SameSite cookie
- ** owasp **: a01

使用双重提交cookie或同步器令牌。Next.js服务器动作有内置的CSRF通过起源头。

### FE8：仅客户端输入验证

- **严重性**：重要
- **检测**：仅在前端进行表单验证
- ** owasp **: a05

总是在服务器上验证。使用zod、joi或类验证器。

---

依赖反模式（D1-D5）

### D1：已知的脆弱依赖

—**严重性**：紧急
—**检测**:`npm audit --audit-level=high`退出非零
- ** owasp **: a03

### D2：锁文件不同步

- **严重性**：重要
—**检测**:`npm ci`失败
- ** owasp **: a08

### D3: typposquatting风险- **严重性**：重要
- **检测**：手动检查新的依赖项名称
- ** owasp **: a03

### D4: Postinstall脚本在新的依赖

- **严重性**：重要
- **检测**:`"postinstall"`在新的依赖的package.json- ** owasp **: a03

### D5：生产中的未固定版本

—**严重性**：建议
- **检测**:`":\s*["']\*["']|":\s*["']latest["']`- ** owasp **: a03

---

API反模式（AP1-AP6）

AP1：无速率限制的新端点

- **严重性**：重要
- ** owasp **: a05

AP2：没有深度限制的GraphQL

- **严重性**：重要
- **检测**:`new ApolloServer`无depth/complexity限制
- ** owasp **: a05```typescript
import depthLimit from 'graphql-depth-limit';
const server = new ApolloServer({
  schema,
  validationRules: [depthLimit(5)],
  introspection: process.env.NODE_ENV !== 'production',
});
```
### AP3：文件上传未经验证

- **严重性**：重要
- **检测**:`multer|formidable|busboy`，不做type/size检测
- ** owasp **: a05```typescript
const upload = multer({
  dest: 'uploads/',
  limits: { fileSize: 5 * 1024 * 1024 },
  fileFilter: (req, file, cb) => {
    const allowed = ['image/jpeg', 'image/png', 'image/webp'];
    cb(null, allowed.includes(file.mimetype));
  },
});
```
### AP4: Webhook没有签名验证

—**严重性**：紧急
- ** owasp **: a08

始终验证webhook签名（Stripe， GitHub HMAC等）。

AP5: API暴露内部信息

- **严重性**：重要
—**检测**:`(?:stack|trace|query|sql).*(?:res\.json|res\.send)`- ** owasp **: a10

### AP6：缺少请求体大小限制

- **严重性**：重要
- **检测**:`express\.json\(\)`不带`limit`- ** owasp **: a05```typescript
app.use(express.json({ limit: '100kb' }));
```
---

安全反模式（AI1-AI3）

### AI1：通过用户输入进行提示注入

—**严重性**：紧急
- **检测**：用户输入连接到LLM提示，不进行消毒
- **OWASP**: A05（注入）```typescript
// BAD — user input directly in prompt
const response = await llm.complete(`Summarize this: ${userInput}`);

// GOOD — structured input with system/user message separation
const response = await llm.complete({
  system: "You are a summarization assistant. Only summarize the provided text.",
  user: userInput,
});
```
### AI2：在SQL/Shell中使用的LLM输出未进行消毒

—**严重性**：紧急
- **检测**:LLM响应传递到`db.query()`，`exec()`，或模板字面量没有验证
- **OWASP**: A05（注入）

永远不要相信LLM输出是安全的。将其视为不受信任的用户输入-参数化查询，转义shell参数，在呈现前清理HTML。

### AI3: LLM响应缺少输出验证

- **严重性**：重要
- **检测**：在没有模式验证的情况下呈现或执行LLM响应
- **OWASP**: A08（软件或数据完整性故障）

在应用程序逻辑中使用之前，根据预期的模式（Zod、JSON模式）验证LLM输出。拒绝与预期结构不匹配的响应。

---

记录反模式（L1-L4）

### L1：安全事件未记录

- **严重性**：重要
- ** owasp **: a09日志：认证失败，拒绝访问，速率限制命中，输入验证失败，密码更改。

### L2：日志中的敏感数据

—**严重性**：紧急
- **检测**:`(?:log|logger)\.\w+\(.*(?:password|token|secret|ssn|credit)`- ** owasp **: a09```typescript
import pino from 'pino';
const logger = pino({ redact: ['req.headers.authorization', 'req.body.password'] });
```
### L3：缺少跟踪id

—**严重性**：建议
- ** owasp **: a09

L4：日志注入

- **严重性**：重要
—**检测**:`console\.log\(.*\+.*(?:req\.|user\.|body\.)`- ** owasp **: a09

使用结构化日志记录（JSON，自动转义）代替字符串连接。

---

框架特定：React /Next.js（RX1-RX4）

### RX1：未经授权的服务器操作

—**严重性**：紧急
—**检测**:`'use server'`功能，不进行`auth()`或会话检查
- ** owasp **: a01```typescript
'use server';
import { auth } from '@/auth';
export async function deleteUser(id: string) {
  const session = await auth();
  if (!session?.user || session.user.role !== 'admin') throw new Error('Unauthorized');
  await db.user.delete({ where: { id } });
}
```
### RX2：进程。客户端没有NEXT_PUBLIC_

- **严重性**：重要
- **检测**:`'use client'`文件访问`process.env`而不访问`NEXT_PUBLIC_`- ** owasp **: a02

RSC序列化泄露数据

- **严重性**：重要
- ** owasp **: a01

在将DB对象传递给客户端组件之前，只选择需要的字段。

RX4:middleware.ts不保护API路由

- **严重性**：重要
- **检测**:`config.matcher`不覆盖`/api/`- ** owasp **: a01

---

框架特定：Angular （NG1-NG3）

### NG1：通过用户输入绕过securitytrusthhtml

—**严重性**：紧急
- **检测**:`bypassSecurityTrust(?:Html|Script|Style|Url|ResourceUrl)`- ** owasp **: a05

在调用bypassSecurityTrust之前，使用DOMPurify进行消毒。

NG2：模板表达式注入

- **严重性**：重要
- ** owasp **: a05

不要对用户控制的模板使用JitCompilerFactory。

### NG3: HttpInterceptor不附加认证- **严重性**：重要
- ** owasp **: a07

使用集中式的`HttpInterceptorFn`作为认证令牌。

---

框架特定：Express （EX1-EX4）

### EX1：缺少helmet.js- **严重性**：重要
- ** owasp **: a02```typescript
import helmet from 'helmet';
app.use(helmet());
app.disable('x-powered-by');
```
### EX2:express.json（）没有身体尺寸限制

- **严重性**：重要
- ** owasp **: a05```typescript
app.use(express.json({ limit: '100kb' }));
```
### EX3: Cookie没有安全标志

- **严重性**：重要
- ** owasp **: a07```typescript
res.cookie('session', value, {
  httpOnly: true, secure: true, sameSite: 'strict', maxAge: 3600000, path: '/',
});
```
### EX4：错误处理器暴露堆栈跟踪

- **严重性**：重要
- ** owasp **: a10

只在开发模式下公开错误细节。

---

框架特定：Go （GO1-GO3）

### GO1:math/rand用于安全操作

—**严重性**：紧急
—**Detection**：导入安全相关文件中的`math/rand`- ** owasp **: a04

使用`crypto/rand`表示加密安全的随机值。

### GO2: TLS InsecureSkipVerify

—**严重性**：紧急
- **检测**:`InsecureSkipVerify:\s*true`- ** owasp **: a04

请使用系统CA池（默认）。

### GO3: SQL中的字符串插值

—**严重性**：紧急
- **检测**:`fmt\.Sprintf\s*\(.*(?:SELECT|INSERT|UPDATE|DELETE|FROM|WHERE)`- ** owasp **: a05```go
// GOOD — parameterized
db.Where("id = ?", userID).Find(&user)
```
---

##安全头模板

###helmet.js（Express）```typescript
import helmet from 'helmet';

app.use(helmet({
  contentSecurityPolicy: {
    directives: {
      defaultSrc: ["'self'"],
      scriptSrc: ["'self'"],
      styleSrc: ["'self'"],
      imgSrc: ["'self'", "data:", "https:"],
      fontSrc: ["'self'"],
      connectSrc: ["'self'"],
      frameAncestors: ["'none'"],
      objectSrc: ["'none'"],
      baseUri: ["'self'"],
      formAction: ["'self'"],
      upgradeInsecureRequests: [],
    },
  },
  hsts: { maxAge: 31536000, includeSubDomains: true, preload: true },
  frameguard: { action: 'deny' },
  referrerPolicy: { policy: 'strict-origin-when-cross-origin' },
  crossOriginOpenerPolicy: { policy: 'same-origin' },
  crossOriginResourcePolicy: { policy: 'same-origin' },
}));
app.disable('x-powered-by');
```
---

JWT验证检查表

1. 用预期算法验证签名-拒绝`alg: none`2. 强制算法：`algorithms: ['RS256']`或`['ES256']`3. 检查`exp`-拒绝过期令牌
4. 检查`iat`-拒绝过去发布的令牌
5. 选中`aud`-拒绝不打算用于此服务的令牌
6. 选中`iss`-拒绝来自未知发行者的令牌
7. 存储在httpOnly cookie -而不是localStorage
8. 使用短期访问令牌（15分钟）+刷新令牌轮换
9. 定期轮换签名密钥

---

##安全Cookie标志```
Set-Cookie: session=value; HttpOnly; Secure; SameSite=Strict; Path=/; Max-Age=3600
```
|标志|用途|何时使用||------|---------|-------------|
|`HttpOnly`|不能通过JavaScript访问（防止XSS令牌盗窃）|总是|
|`Secure`|只通过HTTPS发送|总是|
|`SameSite=Strict`|只发送同一站点请求（最强CSRF） |Auth/sessioncookies |
|`SameSite=Lax`|在顶级导航（中等CSRF）上发送|需要跨站点顶级导航（例如，OAuth返回）|
|`Path=/`|限制cookie范围|总是|
|`Max-Age`|显式过期（优先于`Expires`） |总是|

---

##安全检查表

###认证和会话
-[]密码哈希与Argon2id或bcrypt（成本>= 12）
-[]使用RS256/ES256签名的JWT，验证时强制执行算法
—[]访问令牌过期时间<= 15分钟
-[]刷新令牌：一次性使用，旋转，存储在httpOnly cookie
-[]登录、注册和密码重置速率限制
-[]认证后生成的会话
- [] MFA可用于特权帐户# # #授权
-[]每个API端点都有认证中间件
[]所有资源访问的所有权检查（防止IDOR）
-[]服务器端授权（前端保护仅限用户体验）
[]阻止大规模分配（明确的区域选择）
-[]敏感操作需要重新认证

输入和输出
-[]所有用户输入验证服务器端（zod/joi/class-validator）
-[]所有数据库操作的参数化查询
- HTML输出消毒（DOMPurify）时呈现用户内容
-[]错误响应不会暴露生产中的堆栈跟踪

# # #的秘密
-[]源代码中没有硬编码的秘密
- []`.env`文件在`.gitignore`[]服务器机密信息不公开给客户端（没有NEXT_PUBLIC_ on secrets）
-[]启动时校验的环境变量# # #标题
- [] Content-Security-Policy configured（优先选择基于非安全策略）
- [] Strict-Transport-Security带预加载
- [] X-Content-Type-Options: nosniff
- [] X-Frame-Options: DENY
- [] reference - policy: strict-origin-when-cross-origin
- [] permissions -限制未使用api的策略
- [] CORS仅限于已知来源

# # #依赖性
- []`npm audit`（或同等值）传入CI
-[]用`npm ci`提交并验证Lockfile
-[]新的依赖审查的typposquatting和postinstall脚本
-[]生产环境中没有通配符或“最新”版本

# # #日志
[]安全事件记录（认证失败，拒绝访问，速率限制）
-日志中没有敏感数据（密码，令牌，PII）
-[]关联id的结构化日志
-[]为异常模式配置的警报