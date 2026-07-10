---
name: 'Secrets Scanner'
description: 'Scans files modified during a Copilot coding agent session for leaked secrets, credentials, and sensitive data'
tags: ['security', 'secrets', 'scanning', 'session-end']
---
#秘密扫描器钩子

扫描在GitHub Copilot编码代理会话期间修改的文件，在提交之前查找意外泄露的秘密、凭据、API密钥和其他敏感数据。

# #概述

人工智能编码代理快速生成和修改代码，这增加了硬编码秘密滑入代码库的风险。这个钩子作为一个安全网，在会话结束时扫描所有修改过的文件，查找20多种秘密模式，包括：

- **云凭证**:AWS访问密钥，GCP服务帐户密钥，Azure客户端机密
- **平台令牌**:GitHub PATs， npm令牌，Slack令牌，条纹键
—**私钥**:RSA、EC、OpenSSH、PGP、DSA私钥块
- **连接字符串**：数据库uri （PostgreSQL, MongoDB, MySQL, Redis， MSSQL）
- **通用秘密**:API密钥，密码，承载令牌，jwt
—**内部基础设施**：带端口的私有IP地址# #特性

- **两种扫描模式**:`warn`（仅日志）或`block`（exit非零以防止提交）
- **两个扫描范围**:`diff`（修改文件vs HEAD）或`staged`（仅git-stage文件）
—**智能过滤**：跳过二进制文件、锁文件和placeholder/example值
- **Allowlist支持**：排除已知的假阳性通过`SECRETS_ALLOWLIST`- **结构化日志**:JSON行输出与监控工具集成
- **编辑输出**：在日志中截断发现，以避免重新暴露秘密
- **零依赖**：仅使用标准Unix工具（`grep`,`file`,`git`）

# #安装

1. 将钩子文件夹复制到你的存储库：   ```bash
   cp -r hooks/secrets-scanner .github/hooks/
   ```
2. 确保脚本是可执行的：   ```bash
   chmod +x .github/hooks/secrets-scanner/scan-secrets.sh
   ```
3. 创建logs目录并将其添加到`.gitignore`：   ```bash
   mkdir -p logs/copilot/secrets
   echo "logs/" >> .gitignore
   ```
4. 将钩子配置提交到存储库的默认分支。

# #配置

钩子在`hooks.json`中被配置为在`sessionEnd`事件上运行：```json
{
  "version": 1,
  "hooks": {
    "sessionEnd": [
      {
        "type": "command",
        "bash": ".github/hooks/secrets-scanner/scan-secrets.sh",
        "cwd": ".",
        "env": {
          "SCAN_MODE": "warn",
          "SCAN_SCOPE": "diff"
        },
        "timeoutSec": 30
      }
    ]
  }
}
```
环境变量

|变量|值|默认值|描述||----------|--------|---------|-------------|
|`SCAN_MODE`|`warn`，`block`|`warn`|`warn`只记录结果；`block`退出非零，以防止自动提交|
|`SCAN_SCOPE`|`diff`，`staged`|`diff`|`diff`扫描未提交的更改vs HEAD；`staged`只扫描暂存文件|
|`SKIP_SECRETS_SCAN`|`true`| unset |完全禁用扫描器|
|`SECRETS_LOG_DIR`|路径|`logs/copilot/secrets`|扫描日志写入目录|
|`SECRETS_ALLOWLIST`|逗号分隔|取消|忽略模式（例如，`test_key_123,example.com`） |

##如何工作1. 当Copilot编码代理会话结束时，钩子执行
2. 使用`git diff`收集所有修改过的文件（尊重配置的作用域）
3. 过滤二进制文件和锁定文件
4. 逐行扫描每个文本文件，针对20多个regex模式查找已知的秘密格式
5. 跳过看起来像占位符的匹配（例如，包含`example`，`changeme`，`your_`的值）
6. 如果配置了，则根据allowlist检查匹配
7. 报告带有文件路径、行号、模式名称和严重性的发现
8. 为审计目的编写结构化JSON日志条目
9. 在`block`模式中，退出非零以通知代理在提交之前停止

##检测到秘密模式

|模式|级别|匹配||---------|----------|---------------|
|`AWS_ACCESS_KEY`|临界|`AKIAIOSFODNN7EXAMPLE`|
|`AWS_SECRET_KEY`|临界|`aws_secret_access_key = wJalr...`|
|`GCP_SERVICE_ACCOUNT`|临界|`"type": "service_account"`|
|`GCP_API_KEY`|高|`AIzaSyC...`|
|`AZURE_CLIENT_SECRET`|临界|`azure_client_secret = ...`|
|`GITHUB_PAT`|临界|`ghp_xxxxxxxxxxxx...`|
|`GITHUB_FINE_GRAINED_PAT`|临界|`github_pat_...`|
|`PRIVATE_KEY`|临界|`-----BEGIN RSA PRIVATE KEY-----`|
|`GENERIC_SECRET`|高|`api_key = "sk-..."`|
|`CONNECTION_STRING`|高|`postgresql://user:pass@host/db`|
|`SLACK_TOKEN`|高|`xoxb-...`|
|`STRIPE_SECRET_KEY`|临界|`sk_live_...`|
|`NPM_TOKEN`|高|`npm_...`|
|`JWT_TOKEN`| medium |`eyJhbGci...`|
|`INTERNAL_IP_PORT`| medium |`192.168.1.1:8080`|

请参阅`scan-secrets.sh`中的完整列表。

##输出示例

###清理扫描```
🔍 Scanning 5 modified file(s) for secrets...
✅ No secrets detected in 5 scanned file(s)
```
###检测到的结果（警告模式）```
🔍 Scanning 3 modified file(s) for secrets...

⚠️  Found 2 potential secret(s) in modified files:

  FILE                                     LINE   PATTERN                      SEVERITY
  ----                                     ----   -------                      --------
  src/config.ts                            12     GITHUB_PAT                   critical
  .env.local                               3      CONNECTION_STRING            high

💡 Review the findings above. Set SCAN_MODE=block to prevent commits with secrets.
```
###检测到的结果（块模式）```
🔍 Scanning 3 modified file(s) for secrets...

⚠️  Found 1 potential secret(s) in modified files:

  FILE                                     LINE   PATTERN                      SEVERITY
  ----                                     ----   -------                      --------
  lib/auth.py                              45     AWS_ACCESS_KEY               critical

🚫 Session blocked: resolve the findings above before committing.
   Set SCAN_MODE=warn to log without blocking, or add patterns to SECRETS_ALLOWLIST.
```
##日志格式

扫描事件以JSON行格式写入`logs/copilot/secrets/scan.log`：```json
{"timestamp":"2026-03-13T10:30:00Z","event":"secrets_found","mode":"warn","scope":"diff","files_scanned":3,"finding_count":2,"findings":[{"file":"src/config.ts","line":12,"pattern":"GITHUB_PAT","severity":"critical","match":"ghp_...xyz1"}]}
```

```json
{"timestamp":"2026-03-13T10:30:00Z","event":"scan_complete","mode":"warn","scope":"diff","status":"clean","files_scanned":5}
```
与其他钩子配对

这个钩子可以很好地与**Session Auto-Commit**钩子配对。当两者都安装时，命令它们使`secrets-scanner`首先运行：

1. 秘密扫描器在`sessionEnd`运行，捕获泄露的秘密
2. 自动提交在`sessionEnd`运行，只有在之前所有钩子都通过时才提交

设置`SCAN_MODE=block`以防止在检测到秘密时自动提交。

# #定制

- **添加自定义模式**：在`scan-secrets.sh`中编辑`PATTERNS`数组以添加项目特定的秘密格式
- **调整灵敏度**：更改严重性级别或删除产生误报的模式
- **Allowlist已知值**：使用`SECRETS_ALLOWLIST`测试夹具或已知的安全模式
—**更改日志位置**：设置`SECRETS_LOG_DIR`，将日志路由到您喜欢的目录

# #禁用

暂时禁用扫描仪：

—在hook环境中设置`SKIP_SECRETS_SCAN=true`-或者从`hooks.json`中删除`sessionEnd`条目

# #的局限性-基于模式的检测；不执行熵分析或上下文验证
-可能对测试装置或示例代码产生误报（使用allowlist来抑制这些）
-只扫描文本文件；未检测到二进制秘密（密钥库、DER格式的证书）
—要求执行环境中存在“`git`”