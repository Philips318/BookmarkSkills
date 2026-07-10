秘密和凭证检测模式

在步骤3（秘密和暴露扫描）期间加载此文件。

---

##高可信度秘密模式

这些模式几乎总是暗示着一个真正的秘密：

API密钥和令牌```regex
# OpenAI
sk-[a-zA-Z0-9]{48}

# Anthropic
sk-ant-[a-zA-Z0-9\-_]{90,}

# AWS Access Key
AKIA[0-9A-Z]{16}

# AWS Secret Key (look for near AWS_ACCESS_KEY_ID assignment)
[0-9a-zA-Z/+]{40}

# GitHub Token
gh[pousr]_[a-zA-Z0-9]{36,}
github_pat_[a-zA-Z0-9]{82}

# Stripe
sk_live_[a-zA-Z0-9]{24,}
rk_live_[a-zA-Z0-9]{24,}

# Twilio Account SID
AC[a-z0-9]{32}
# Twilio API Key
SK[a-z0-9]{32}

# SendGrid
SG\.[a-zA-Z0-9\-_.]{66}

# Slack
xoxb-[0-9]+-[0-9]+-[a-zA-Z0-9]+
xoxp-[0-9]+-[0-9]+-[0-9]+-[a-zA-Z0-9]+
xapp-[0-9]+-[A-Z0-9]+-[0-9]+-[a-zA-Z0-9]+

# Google API Key
AIza[0-9A-Za-z\-_]{35}

# Google OAuth
[0-9]+-[0-9A-Za-z_]{32}\.apps\.googleusercontent\.com

# Cloudflare (near CF_API_TOKEN)
[a-zA-Z0-9_\-]{37}

# Mailgun
key-[a-zA-Z0-9]{32}

# Heroku
[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}
```
###私钥```regex
-----BEGIN (RSA |EC |OPENSSH |DSA |PGP )?PRIVATE KEY( BLOCK)?-----
-----BEGIN CERTIFICATE-----
```
数据库连接字符串```regex
# MongoDB
mongodb(\+srv)?:\/\/[^:]+:[^@]+@

# PostgreSQL / MySQL
(postgres|postgresql|mysql):\/\/[^:]+:[^@]+@

# Redis with password
redis:\/\/:[^@]+@

# Generic connection string with password
(connection[_-]?string|connstr|db[_-]?url).*password=
```
硬编码密码（变量名信号）```regex
# Variable names that suggest secrets
(password|passwd|pwd|secret|api_key|apikey|auth_token|access_token|private_key)
  \s*[=:]\s*["'][^"']{8,}["']
```
---

基于熵的检测

应用于赋值上下文中的字符串字面值> 20个字符。
高熵（香农熵> 4.5bits/char） +长度> 20 =可能的秘密。```
Calculate entropy: -sum(p * log2(p)) for each character frequency p
Threshold: > 4.5 bits/char AND > 20 chars AND assigned to a variable
```
要排除的常见误报：
- Lorem ipsum文本
-HTML/CSS内容
- base64编码的非敏感配置（但标志和说明）
-UUID/GUID（熵高但格式可识别）

---

不应该提交的文件

标记这些文件是否存在于repo根目录或被git跟踪：```
.env
.env.local
.env.production
.env.staging
*.pem
*.key
*.p12
*.pfx
id_rsa
id_ed25519
credentials.json
service-account.json
gcp-key.json
secrets.yaml
secrets.json
config/secrets.yml
```
如果没有秘密文件模式，也检查`.gitignore`-。Gitignore，标记它。

---

##CI/CD& IaC机密风险

###GitHub Actions-标记这些模式：```yaml
# Hardcoded values in env: blocks (should use ${{ secrets.NAME }})
env:
  API_KEY: "actual-value-here"   # VULNERABLE

# Printing secrets
- run: echo ${{ secrets.MY_SECRET }}   # leaks to logs
```
### Docker -标记这些：```dockerfile
# Secrets in ENV (persisted in image layers)
ENV AWS_SECRET_KEY=actual-value

# Secrets passed as build args (visible in image history)
ARG API_KEY=actual-value
```
### Terraform -标记这些：```hcl
# Hardcoded sensitive values (should use var or data source)
password = "hardcoded-password"
access_key = "AKIAIOSFODNN7EXAMPLE"
```
---

##安全模式（不标记）

这些是有意的占位符-识别和跳过：```
"your-api-key-here"
"<YOUR_API_KEY>"
"${API_KEY}"
"${process.env.API_KEY}"
"os.environ.get('API_KEY')"
"REPLACE_WITH_YOUR_KEY"
"xxx...xxx"
"sk-..." (in documentation/comments)
```
