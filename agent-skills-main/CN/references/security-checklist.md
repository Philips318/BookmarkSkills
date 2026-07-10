# Security Checklist

Web 应用安全的快速参考。与 `security-and-hardening` skill 搭配使用。

## Table of Contents

- [Threat Modeling (Start Here)](#threat-modeling-start-here)
- [Pre-Commit Checks](#pre-commit-checks)
- [Authentication](#authentication)
- [Authorization](#authorization)
- [Input Validation](#input-validation)
- [Security Headers](#security-headers)
- [CORS Configuration](#cors-configuration)
- [Data Protection](#data-protection)
- [Dependency Security](#dependency-security)
- [AI / LLM Security](#ai--llm-security)
- [Error Handling](#error-handling)
- [OWASP Top 10 Quick Reference](#owasp-top-10-quick-reference)
- [OWASP Top 10 for LLMs Quick Reference](#owasp-top-10-for-llms-quick-reference)

## Threat Modeling (Start Here)

在伸手拿 controls 前，先花五分钟像攻击者一样思考：

- [ ] Trust boundaries 已映射（requests、uploads、webhooks、third-party APIs、LLM output）
- [ ] Assets 已命名（credentials、PII、payment data、admin actions、money movement）
- [ ] 每个 boundary 已运行 STRIDE（Spoofing、Tampering、Repudiation、Info disclosure、DoS、Elevation）
- [ ] Abuse cases 写在 use cases 旁边（“how would I misuse this?”）

## Pre-Commit Checks

- [ ] 代码中没有 secrets（`git diff --cached | grep -i "password\|secret\|api_key\|token"`）
- [ ] `.gitignore` 覆盖：`.env`、`.env.local`、`*.pem`、`*.key`
- [ ] `.env.example` 使用 placeholder values（不是真实 secrets）

## Authentication

- [ ] Passwords 使用 bcrypt（≥12 rounds）、scrypt 或 argon2 hash
- [ ] Session cookies: `httpOnly`、`secure`、`sameSite: 'lax'`
- [ ] Session expiration 已配置（合理 max-age）
- [ ] Login endpoint 有 rate limiting（每 15 分钟 ≤10 次尝试）
- [ ] Password reset tokens：有时间限制（≤1 小时）、一次性使用
- [ ] 重复失败后 account lockout（可选，带 notification）
- [ ] 敏感操作支持 MFA（可选但推荐）

## Authorization

- [ ] 每个 protected endpoint 都检查 authentication
- [ ] 每次 resource access 都检查 ownership/role（防止 IDOR）
- [ ] Admin endpoints 要求验证 admin role
- [ ] API keys scope 到最小必要权限
- [ ] JWT tokens 已验证（signature、expiration、issuer）

## Input Validation

- [ ] 所有 user input 都在 system boundaries 处验证（API routes、form handlers）
- [ ] Validation 使用 allowlists（不是 denylists）
- [ ] String lengths 有约束（min/max）
- [ ] Numeric ranges 已验证
- [ ] Email、URL 和 date formats 使用合适 libraries 验证
- [ ] File uploads：限制 type，限制 size，验证 content
- [ ] SQL queries 参数化（无 string concatenation）
- [ ] HTML output 已编码（使用 framework auto-escaping）
- [ ] Redirect 前验证 URLs（防止 open redirect）
- [ ] Server-side URL fetches 使用 allowlist；阻止 private/reserved IPs（防止 SSRF）

## Security Headers

```
Content-Security-Policy: default-src 'self'; script-src 'self'
Strict-Transport-Security: max-age=31536000; includeSubDomains
X-Content-Type-Options: nosniff
X-Frame-Options: DENY
X-XSS-Protection: 0  (disabled, rely on CSP)
Referrer-Policy: strict-origin-when-cross-origin
Permissions-Policy: camera=(), microphone=(), geolocation=()
```

## CORS Configuration

```typescript
// Restrictive (recommended)
cors({
  origin: ['https://yourdomain.com', 'https://app.yourdomain.com'],
  credentials: true,
  methods: ['GET', 'POST', 'PUT', 'PATCH', 'DELETE'],
  allowedHeaders: ['Content-Type', 'Authorization'],
})

// NEVER use in production:
cors({ origin: '*' })  // Allows any origin
```

## Data Protection

- [ ] API responses 中排除 sensitive fields（`passwordHash`、`resetToken` 等）
- [ ] 不记录 sensitive data（passwords、tokens、完整 CC numbers）
- [ ] PII 静态加密（如法规要求）
- [ ] 所有 external communication 使用 HTTPS
- [ ] Database backups 已加密

## Dependency Security

```bash
# Audit dependencies
npm audit

# Fix automatically where possible
npm audit fix

# Check for critical vulnerabilities
npm audit --audit-level=critical

# Keep dependencies updated
npx npm-check-updates
```

**Supply-chain hygiene**（`npm audit` 捕获不到 malicious packages）：
- [ ] Lockfile 已提交；CI 使用 `npm ci` 安装（不是 `npm install`）
- [ ] 新 dependencies 已评审（maintenance、downloads、`postinstall` scripts）
- [ ] 没有 typosquats（`cross-env` vs `crossenv`，`react-dom` vs `reactdom`）

## AI / LLM Security

对于任何调用 LLM 的 feature（chatbots、summarizers、agents、RAG）：

- [ ] Model output 被视为不可信 — 绝不进入 `eval`/SQL/shell/`innerHTML`/file paths
- [ ] 假定存在 prompt injection；permissions 在代码中强制执行，而不是在 system prompt 中
- [ ] Secrets、cross-tenant data 和完整 system prompts 不进入 context window
- [ ] Tool/agent permissions 有 scope；destructive 或 irreversible actions 需要确认
- [ ] 设置 token、rate 和 recursion/loop limits（限制消耗）

## Error Handling

```typescript
// Production: generic error, no internals
res.status(500).json({
  error: { code: 'INTERNAL_ERROR', message: 'Something went wrong' }
});

// NEVER in production:
res.status(500).json({
  error: err.message,
  stack: err.stack,         // Exposes internals
  query: err.sql,           // Exposes database details
});
```

## OWASP Top 10 Quick Reference

| # | Vulnerability | Prevention |
|---|---|---|
| 1 | Broken Access Control | 每个 endpoint 做 auth checks，验证 ownership |
| 2 | Cryptographic Failures | HTTPS、strong hashing、代码中无 secrets |
| 3 | Injection | Parameterized queries、input validation |
| 4 | Insecure Design | Threat modeling、spec-driven development |
| 5 | Security Misconfiguration | Security headers、minimal permissions、audit deps |
| 6 | Vulnerable Components | `npm audit`、保持 deps updated、minimal deps |
| 7 | Auth Failures | Strong passwords、rate limiting、session management |
| 8 | Data Integrity Failures | 验证 updates/dependencies，signed artifacts |
| 9 | Logging Failures | 记录 security events，不记录 secrets |
| 10 | SSRF | 验证/allowlist URLs，限制 outbound requests |

## OWASP Top 10 for LLMs Quick Reference

适用于带 LLM features 的 apps。见 [OWASP GenAI Security Project](https://genai.owasp.org/llm-top-10/)。

| ID | Risk | Prevention |
|---|---|---|
| LLM01 | Prompt Injection | 不要把 system prompt 当边界信任；在代码中强制 permissions |
| LLM02 | Sensitive Information Disclosure | 让 secrets/PII 离开 prompts；过滤 outputs |
| LLM03 | Supply Chain | 像审查任何 dependency 一样审查 models、datasets 和 plugins |
| LLM04 | Data and Model Poisoning | 使用可信 model sources，验证 integrity；审查 fine-tuning 和 RAG data |
| LLM05 | Improper Output Handling | 将 model output 视为不可信；validate、parameterize、encode |
| LLM06 | Excessive Agency | 限定 tool permissions；确认 destructive actions |
| LLM07 | System Prompt Leakage | 假定 system prompt 可能泄漏；不要在其中放 secrets |
| LLM08 | Vector and Embedding Weaknesses | 按 tenant 分区 RAG embeddings；indexing 前验证 documents |
| LLM09 | Misinformation | 用 citations grounding answers；验证 critical claims；保留 human in the loop |
| LLM10 | Unbounded Consumption | 限制 tokens、request rate 和 loop/recursion depth |
