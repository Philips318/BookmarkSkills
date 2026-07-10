---
name: security-and-hardening
description: 加固代码以抵御漏洞。用于处理用户输入、认证、数据存储或外部集成时。用于构建任何接受不可信数据、管理用户会话或与第三方服务交互的功能。
---

# 安全与加固

## 概述

面向 Web 应用的安全优先开发实践。把每个外部输入都视为敌对，把每个 secret 都视为神圣，把每个授权检查都视为强制要求。安全不是一个阶段，而是每一行触及用户数据、认证或外部系统的代码都必须遵守的约束。

## 何时使用

- 构建任何接受用户输入的内容
- 实现 authentication 或 authorization
- 存储或传输敏感数据
- 集成外部 API 或服务
- 添加文件上传、webhooks 或 callbacks
- 处理支付或 PII 数据

## 流程：先做威胁建模

没有威胁模型就外挂控制措施，本质上是在猜。在加固前，花五分钟像攻击者一样思考：

1. **映射信任边界。** 不可信数据在哪里进入系统？HTTP requests、form fields、file uploads、webhooks、third-party APIs、message queues，以及 **LLM output**。每个边界都是攻击面。
2. **命名资产。** 什么值得被偷或破坏？Credentials、PII、payment data、admin actions、money movement。
3. **对每个边界运行 STRIDE**：这是快速视角，不是仪式：

| 威胁 | 询问 | 典型缓解 |
|---|---|---|
| **S**poofing | 是否有人能冒充用户/服务？ | Authentication、signature verification |
| **T**ampering | 数据在传输或静止时能否被篡改？ | Integrity checks、parameterized queries、HTTPS |
| **R**epudiation | 某个动作之后能否被否认？ | Security events 的 audit logging |
| **I**nformation disclosure | 数据能否泄漏？ | Encryption、field allowlists、generic errors |
| **D**enial of service | 它能否被压垮？ | Rate limiting、input size caps、timeouts |
| **E**levation of privilege | 用户能否获得不该有的权限？ | Authorization checks、least privilege |

4. **把滥用案例写在用例旁边。** 对每个功能问“我会如何滥用它？”然后把它作为你的第一个测试。

如果你不能命名一个功能的 trust boundaries，就还没准备好保护它。这是 OWASP **A04: Insecure Design**，大多数 breach 始于设计，而不是代码。

## 三层边界系统

### 始终执行（无例外）

- 在系统边界（API routes、form handlers）**验证所有外部输入**
- **参数化所有数据库查询**：永远不要把用户输入拼接进 SQL
- **编码输出**以防 XSS（使用框架自动 escaping，不要绕过）
- 对所有外部通信**使用 HTTPS**
- 用 bcrypt/scrypt/argon2 **哈希 passwords**（永远不要存 plaintext）
- **设置安全 headers**（CSP、HSTS、X-Frame-Options、X-Content-Type-Options）
- 对 sessions 使用 **httpOnly、secure、sameSite cookies**
- 每次发布前运行 **`npm audit`**（或等价工具）

### 先询问（需要人类批准）

- 添加新的 authentication flows 或修改 auth logic
- 存储新的敏感数据类别（PII、payment info）
- 添加新的外部服务集成
- 修改 CORS 配置
- 添加文件上传处理器
- 修改 rate limiting 或 throttling
- 授予更高 permissions 或 roles

### 永远不要做

- **永远不要提交 secrets** 到版本控制（API keys、passwords、tokens）
- **永远不要记录敏感数据**（passwords、tokens、完整信用卡号）
- **永远不要信任客户端验证**作为安全边界
- **永远不要为了方便禁用 security headers**
- **永远不要对用户提供的数据使用 `eval()` 或 `innerHTML`**
- **永远不要把 sessions 存在客户端可访问存储中**（auth tokens 放 localStorage）
- **永远不要向用户暴露 stack traces** 或内部错误细节

## OWASP Top 10 预防模式

这些是预防模式，而不是排名。2021 排序见 `references/security-checklist.md` 中的速查表。

### Injection（SQL、NoSQL、OS Command）

```typescript
// BAD: SQL injection via string concatenation
const query = `SELECT * FROM users WHERE id = '${userId}'`;

// GOOD: Parameterized query
const user = await db.query('SELECT * FROM users WHERE id = $1', [userId]);

// GOOD: ORM with parameterized input
const user = await prisma.user.findUnique({ where: { id: userId } });
```

### Broken Authentication

```typescript
// Password hashing
import { hash, compare } from 'bcrypt';

const SALT_ROUNDS = 12;
const hashedPassword = await hash(plaintext, SALT_ROUNDS);
const isValid = await compare(plaintext, hashedPassword);

// Session management
app.use(session({
  secret: process.env.SESSION_SECRET,  // From environment, not code
  resave: false,
  saveUninitialized: false,
  cookie: {
    httpOnly: true,     // Not accessible via JavaScript
    secure: true,       // HTTPS only
    sameSite: 'lax',    // CSRF protection
    maxAge: 24 * 60 * 60 * 1000,  // 24 hours
  },
}));
```

### Cross-Site Scripting (XSS)

```typescript
// BAD: Rendering user input as HTML
element.innerHTML = userInput;

// GOOD: Use framework auto-escaping (React does this by default)
return <div>{userInput}</div>;

// If you MUST render HTML, sanitize first
import DOMPurify from 'dompurify';
const clean = DOMPurify.sanitize(userInput);
```

### Broken Access Control

```typescript
// Always check authorization, not just authentication
app.patch('/api/tasks/:id', authenticate, async (req, res) => {
  const task = await taskService.findById(req.params.id);

  // Check that the authenticated user owns this resource
  if (task.ownerId !== req.user.id) {
    return res.status(403).json({
      error: { code: 'FORBIDDEN', message: 'Not authorized to modify this task' }
    });
  }

  // Proceed with update
  const updated = await taskService.update(req.params.id, req.body);
  return res.json(updated);
});
```

### Security Misconfiguration

```typescript
// Security headers (use helmet for Express)
import helmet from 'helmet';
app.use(helmet());

// Content Security Policy
app.use(helmet.contentSecurityPolicy({
  directives: {
    defaultSrc: ["'self'"],
    scriptSrc: ["'self'"],
    styleSrc: ["'self'", "'unsafe-inline'"],  // Tighten if possible
    imgSrc: ["'self'", 'data:', 'https:'],
    connectSrc: ["'self'"],
  },
}));

// CORS — restrict to known origins
app.use(cors({
  origin: process.env.ALLOWED_ORIGINS?.split(',') || 'http://localhost:3000',
  credentials: true,
}));
```

### Sensitive Data Exposure

```typescript
// Never return sensitive fields in API responses
function sanitizeUser(user: UserRecord): PublicUser {
  const { passwordHash, resetToken, ...publicFields } = user;
  return publicFields;
}

// Use environment variables for secrets
const API_KEY = process.env.STRIPE_API_KEY;
if (!API_KEY) throw new Error('STRIPE_API_KEY not configured');
```

### Server-Side Request Forgery (SSRF)

任何时候，只要服务器获取了受用户影响的 URL，包括 webhooks、“import from URL”、image proxies、link previews，攻击者就能把它指向内部服务（cloud metadata、`localhost`、private IPs）。

```typescript
// BAD: fetch whatever the user gives you
await fetch(req.body.webhookUrl);

// GOOD: allowlist scheme + host, reject if ANY resolved IP is private, forbid redirects
import { lookup } from 'node:dns/promises';
import ipaddr from 'ipaddr.js';

const ALLOWED_HOSTS = new Set(['hooks.example.com']);

async function assertSafeUrl(raw: string): Promise<URL> {
  const url = new URL(raw);
  if (url.protocol !== 'https:') throw new Error('https only');
  if (!ALLOWED_HOSTS.has(url.hostname)) throw new Error('host not allowed');
  // Resolve ALL records; a single private/reserved address fails the check.
  const addrs = await lookup(url.hostname, { all: true });
  if (addrs.some((a) => ipaddr.parse(a.address).range() !== 'unicast')) {
    throw new Error('private/reserved IP');
  }
  return url;
}

await fetch(await assertSafeUrl(req.body.webhookUrl), { redirect: 'error' });
```

`range() !== 'unicast'` 检查覆盖 loopback、link-local `169.254.169.254`（cloud metadata，SSRF 的头号目标）、private，以及 IPv4 和 IPv6 的 unique-local ranges。

**注意：这仍然有 TOCTOU 缺口。** `fetch` 会在检查后再次解析 DNS，因此使用短 TTL 记录的攻击者可以在验证和连接之间 rebind 到内部 IP。对于高风险 surface，请解析一次并连接到 pinned IP，或在前面放一个 filtering agent（`request-filtering-agent` / `ssrf-req-filter`）。

## 输入验证模式

### 边界处的 Schema 验证

```typescript
import { z } from 'zod';

const CreateTaskSchema = z.object({
  title: z.string().min(1).max(200).trim(),
  description: z.string().max(2000).optional(),
  priority: z.enum(['low', 'medium', 'high']).default('medium'),
  dueDate: z.string().datetime().optional(),
});

// Validate at the route handler
app.post('/api/tasks', async (req, res) => {
  const result = CreateTaskSchema.safeParse(req.body);
  if (!result.success) {
    return res.status(422).json({
      error: {
        code: 'VALIDATION_ERROR',
        message: 'Invalid input',
        details: result.error.flatten(),
      },
    });
  }
  // result.data is now typed and validated
  const task = await taskService.create(result.data);
  return res.status(201).json(task);
});
```

### 文件上传安全

```typescript
// Restrict file types and sizes
const ALLOWED_TYPES = ['image/jpeg', 'image/png', 'image/webp'];
const MAX_SIZE = 5 * 1024 * 1024; // 5MB

function validateUpload(file: UploadedFile) {
  if (!ALLOWED_TYPES.includes(file.mimetype)) {
    throw new ValidationError('File type not allowed');
  }
  if (file.size > MAX_SIZE) {
    throw new ValidationError('File too large (max 5MB)');
  }
  // Don't trust the file extension — check magic bytes if critical
}
```

## 分诊 npm audit 结果

不是所有 audit 发现都需要立即行动。使用这个决策树：

```
npm audit reports a vulnerability
├── Severity: critical or high
│   ├── Is the vulnerable code reachable in your app?
│   │   ├── YES --> Fix immediately (update, patch, or replace the dependency)
│   │   └── NO (dev-only dep, unused code path) --> Fix soon, but not a blocker
│   └── Is a fix available?
│       ├── YES --> Update to the patched version
│       └── NO --> Check for workarounds, consider replacing the dependency, or add to allowlist with a review date
├── Severity: moderate
│   ├── Reachable in production? --> Fix in the next release cycle
│   └── Dev-only? --> Fix when convenient, track in backlog
└── Severity: low
    └── Track and fix during regular dependency updates
```

**关键问题：**
- 易受攻击的函数是否真的在你的代码路径中被调用？
- 依赖是 runtime dependency 还是 dev-only？
- 鉴于你的部署上下文，这个漏洞是否可利用（例如 client-only app 中的 server-side vulnerability）？

当你推迟修复时，记录原因并设置 review date。

### 供应链卫生

`npm audit` 会捕获已知 CVEs；它不会捕获恶意或 typosquatted package。还要：

- **提交 lockfile**，并在 CI 中用 `npm ci` 安装（不是 `npm install`）：可复现构建，没有静默版本漂移。
- **添加新依赖前审查它们**：维护情况、下载量，以及它们是否真正值得引入。每个依赖都是攻击面（OWASP **A06: Vulnerable Components**，**LLM03: Supply Chain**）。
- **警惕陌生 package 中的 `postinstall` scripts**：它们会在安装时运行任意代码。
- **注意 typosquats**：`cross-env` vs `crossenv`，`react-dom` vs `reactdom`。

## Rate Limiting

```typescript
import rateLimit from 'express-rate-limit';

// General API rate limit
app.use('/api/', rateLimit({
  windowMs: 15 * 60 * 1000, // 15 minutes
  max: 100,                   // 100 requests per window
  standardHeaders: true,
  legacyHeaders: false,
}));

// Stricter limit for auth endpoints
app.use('/api/auth/', rateLimit({
  windowMs: 15 * 60 * 1000,
  max: 10,  // 10 attempts per 15 minutes
}));
```

## Secrets 管理

```
.env files:
  ├── .env.example  → Committed (template with placeholder values)
  ├── .env          → NOT committed (contains real secrets)
  └── .env.local    → NOT committed (local overrides)

.gitignore must include:
  .env
  .env.local
  .env.*.local
  *.pem
  *.key
```

**提交前始终检查：**
```bash
# Check for accidentally staged secrets
git diff --cached | grep -i "password\|secret\|api_key\|token"
```

**如果 secret 曾经被提交，请 rotate 它。** 删除那一行或改写历史还不够；一旦它到达 remote，就假设它已经泄露。先撤销并重新签发 key，再从历史中清除它。

## 保护 AI / LLM 功能

如果你的应用调用 LLM，包括 chatbots、summarizers、agents、RAG，它就继承了新的攻击面。把它映射到 [OWASP Top 10 for LLM Applications (2025)](https://genai.owasp.org/llm-top-10/)：

- **把所有 model output 当作不可信输入（LLM05: Improper Output Handling）。** 永远不要把 LLM output 直接传给 `eval`、SQL、shell、`innerHTML` 或文件路径。像处理原始用户输入一样验证并编码它。
- **假设 prompts 可以被劫持（LLM01: Prompt Injection）。** Context window 中的不可信文本，包括用户消息、抓取的网页、PDF，都可能携带指令。System prompt 不是安全边界；在代码中强制权限，而不是在 prompt 中。
- **让 secrets 和其他用户数据远离 prompts（LLM02 / LLM07）。** Context 中的任何内容都可能被 echo 回来。不要把 API keys、跨租户数据或完整 system prompt 放到模型可能复述的位置。
- **约束 tool 和 agent 权限（LLM06: Excessive Agency）。** 将工具限定到最小权限，对破坏性或不可逆操作要求确认，并验证每个工具参数。
- **限制消耗（LLM10: Unbounded Consumption）。** 限制 tokens、请求速率和 loop/recursion depth，防止构造输入拉高成本或挂起系统。
- **隔离 retrieval 数据（LLM08: Vector and Embedding Weaknesses）。** 在 RAG 中，把 vector store 当作 trust boundary：按租户分区 embeddings，防止一个用户检索到另一个用户的数据，并在 indexing 前验证 documents，防止 poisoned content 引导回答。

```typescript
// BAD: trusting model output as a command or as markup
const sql = await llm.generate(`Write SQL for: ${userQuestion}`);
await db.query(sql);                                   // arbitrary query execution
container.innerHTML = await llm.reply(userMessage);   // stored XSS, via the model

// GOOD: model output is data — parse defensively, then validate, then encode
let intent;
try {
  intent = CommandSchema.parse(JSON.parse(await llm.replyJson(userMessage)));
} catch {
  throw new ValidationError('unexpected model output'); // JSON.parse or schema failed
}
await runAllowlistedAction(intent.action, intent.params);
container.textContent = await llm.reply(userMessage);
```

## 安全审查清单

```markdown
### Authentication
- [ ] Passwords hashed with bcrypt/scrypt/argon2 (salt rounds ≥ 12)
- [ ] Session tokens are httpOnly, secure, sameSite
- [ ] Login has rate limiting
- [ ] Password reset tokens expire

### Authorization
- [ ] Every endpoint checks user permissions
- [ ] Users can only access their own resources
- [ ] Admin actions require admin role verification

### Input
- [ ] All user input validated at the boundary
- [ ] SQL queries are parameterized
- [ ] HTML output is encoded/escaped
- [ ] Server-side URL fetches are allowlisted (no SSRF to internal services)

### Data
- [ ] No secrets in code or version control
- [ ] Sensitive fields excluded from API responses
- [ ] PII encrypted at rest (if applicable)

### Infrastructure
- [ ] Security headers configured (CSP, HSTS, etc.)
- [ ] CORS restricted to known origins
- [ ] Dependencies audited for vulnerabilities
- [ ] Error messages don't expose internals

### Supply Chain
- [ ] Lockfile committed; CI installs with `npm ci`
- [ ] New dependencies reviewed (maintenance, downloads, postinstall scripts)

### AI / LLM (if used)
- [ ] Model output treated as untrusted (no eval/SQL/innerHTML/shell)
- [ ] Secrets and other users' data kept out of prompts
- [ ] Tool/agent permissions scoped; destructive actions require confirmation
```
## 另请参阅

有关详细安全清单和提交前验证步骤，请参阅 `references/security-checklist.md`。

## 常见合理化借口

| 合理化借口 | 现实 |
|---|---|
| “这是内部工具，安全不重要” | 内部工具也会被攻破。攻击者会瞄准最薄弱环节。 |
| “我们之后再加安全” | 事后补安全难 10 倍。现在就加入。 |
| “没人会尝试利用这个” | 自动扫描器会找到它。Security by obscurity 不是安全。 |
| “框架会处理安全” | 框架提供工具，不提供保证。你仍然需要正确使用它们。 |
| “这只是原型” | 原型会变成生产。安全习惯从第一天开始。 |
| “这里做威胁建模太夸张” | 五分钟的“我会如何攻击它？”能防止没有控制措施能在之后补上的设计缺陷。 |
| “只是 LLM output，它只是文本” | 那段“文本”可以是 SQL 语句、script tag 或 shell command。像处理任何不可信输入一样处理它。 |

## 危险信号

- 用户输入直接传给数据库查询、shell commands 或 HTML rendering
- Secrets 出现在源码或提交历史中
- API endpoints 没有 authentication 或 authorization checks
- 缺少 CORS 配置或使用 wildcard (`*`) origins
- Authentication endpoints 没有 rate limiting
- Stack traces 或内部错误暴露给用户
- Dependencies 存在已知 critical vulnerabilities
- 服务器获取用户提供的 URLs 但没有 allowlist（SSRF）
- LLM/model output 被传入 query、DOM、shell 或 `eval`
- Secrets、PII 或完整 system prompt 被放入 LLM context window

## 验证

实现安全相关代码后：

- [ ] `npm audit` 没有 critical 或 high vulnerabilities
- [ ] 源码或 git history 中没有 secrets
- [ ] 所有用户输入都在系统边界验证
- [ ] 每个受保护 endpoint 都检查 authentication 和 authorization
- [ ] 响应中存在 security headers（用 browser DevTools 检查）
- [ ] Error responses 不暴露内部细节
- [ ] Auth endpoints 上启用 rate limiting
- [ ] Server-side URL fetches 通过 allowlist 验证（无 SSRF）
- [ ] LLM/model output 使用前已验证并编码（如果存在 AI 功能）
