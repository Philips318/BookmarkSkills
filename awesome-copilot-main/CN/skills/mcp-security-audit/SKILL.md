---
name: mcp-security-audit
description: |
  Audit MCP (Model Context Protocol) server configurations for security issues. Use this skill when:
  - Reviewing .mcp.json files for security risks
  - Checking MCP server args for hardcoded secrets or shell injection patterns
  - Validating that MCP servers use pinned versions (not @latest)
  - Detecting unpinned dependencies in MCP server configurations
  - Auditing which MCP servers a project registers and whether they're on an approved list
  - Checking for environment variable usage vs. hardcoded credentials in MCP configs
  - Any request like "is my MCP config secure?", "audit my MCP servers", or "check .mcp.json"
  keywords: [mcp, security, audit, secrets, shell-injection, supply-chain, governance]
---
# MCP安全审计

审计MCP服务器配置中的安全问题——秘密暴露、shell注入、未固定的依赖关系和未批准的服务器。

# #概述

MCP服务器使代理能够直接使用工具访问外部系统。错误配置的`.mcp.json`可能会暴露凭据、允许shell注入或连接到不受信任的服务器。这一技能能够在问题进入生产阶段之前抓住它们。```
.mcp.json → Parse Servers → Check Each Server:
  1. Secrets in args/env?
  2. Shell injection patterns?
  3. Unpinned versions (@latest)?
  4. Dangerous commands (eval, bash -c)?
  5. Server on approved list?
→ Generate Report
```
##何时使用

-查看项目中任何`.mcp.json`文件
-将新的MCP服务器加入到项目中
-审计所有MCP服务器在一个单一的或插件市场
—MCP配置变更的预提交检查
—座席工具配置安全检查

---

审计检查1：硬编码的秘密

扫描MCP服务器参数和env值以获取硬编码凭据。```python
import json
import re
from pathlib import Path

SECRET_PATTERNS = [
    (r'(?i)(api[_-]?key|token|secret|password|credential)\s*[:=]\s*["\'][^"\']{8,}', "Hardcoded secret"),
    (r'(?i)Bearer\s+[A-Za-z0-9\-._~+/]+=*', "Hardcoded bearer token"),
    (r'(?i)(ghp_|gho_|ghu_|ghs_|ghr_)[A-Za-z0-9]{30,}', "GitHub token"),
    (r'sk-[A-Za-z0-9]{20,}', "OpenAI API key"),
    (r'AKIA[0-9A-Z]{16}', "AWS access key"),
    (r'-----BEGIN\s+(RSA\s+)?PRIVATE\s+KEY-----', "Private key"),
]

def check_secrets(mcp_config: dict) -> list[dict]:
    """Check for hardcoded secrets in MCP server configurations."""
    findings = []
    raw = json.dumps(mcp_config)
    for pattern, description in SECRET_PATTERNS:
        matches = re.findall(pattern, raw)
        if matches:
            findings.append({
                "severity": "CRITICAL",
                "check": "hardcoded-secret",
                "message": f"{description} found in MCP configuration",
                "evidence": f"Pattern matched: {pattern}",
                "fix": "Use environment variable references: ${ENV_VAR_NAME}"
            })
    return findings
```
**好的做法-使用env var引用：**```json
{
  "mcpServers": {
    "my-server": {
      "command": "node",
      "args": ["server.js"],
      "env": {
        "API_KEY": "${MY_API_KEY}",
        "DB_URL": "${DATABASE_URL}"
      }
    }
  }
}
```
**硬编码凭证：**```json
{
  "mcpServers": {
    "my-server": {
      "command": "node",
      "args": ["server.js", "--api-key", "sk-abc123realkey456"],
      "env": {
        "DB_URL": "postgresql://admin:password123@prod-db:5432/main"
      }
    }
  }
}
```
---

审计检查2:Shell注入模式

检测MCP服务器参数中的危险命令模式。```python
import json
import re

DANGEROUS_PATTERNS = [
    (r'\$\(', "Command substitution $(...)"),
    (r'`[^`]+`', "Backtick command substitution"),
    (r';\s*\w', "Command chaining with semicolon"),
    (r'\|\s*\w', "Pipe to another command"),
    (r'&&\s*\w', "Command chaining with &&"),
    (r'\|\|\s*\w', "Command chaining with ||"),
    (r'(?i)eval\s', "eval usage"),
    (r'(?i)bash\s+-c\s', "bash -c execution"),
    (r'(?i)sh\s+-c\s', "sh -c execution"),
    (r'>\s*/dev/tcp/', "TCP redirect (reverse shell pattern)"),
    (r'curl\s+.*\|\s*(ba)?sh', "curl pipe to shell"),
]

def check_shell_injection(server_config: dict) -> list[dict]:
    """Check MCP server args for shell injection risks."""
    findings = []
    args_text = json.dumps(server_config.get("args", []))
    for pattern, description in DANGEROUS_PATTERNS:
        if re.search(pattern, args_text):
            findings.append({
                "severity": "HIGH",
                "check": "shell-injection",
                "message": f"Dangerous pattern in MCP server args: {description}",
                "fix": "Use direct command execution, not shell interpolation"
            })
    return findings
```
---

审计检查3：未固定的依赖项

在其包引用中使用`@latest`标记MCP服务器。```python
def check_pinned_versions(server_config: dict) -> list[dict]:
    """Check that MCP server dependencies use pinned versions, not @latest."""
    findings = []
    args = server_config.get("args", [])
    for arg in args:
        if isinstance(arg, str):
            if "@latest" in arg:
                findings.append({
                    "severity": "MEDIUM",
                    "check": "unpinned-dependency",
                    "message": f"Unpinned dependency: {arg}",
                    "fix": f"Pin to specific version: {arg.replace('@latest', '@1.2.3')}"
                })
            # npx with unversioned package
            if arg.startswith("-y") or (not "@" in arg and not arg.startswith("-")):
                pass  # npx flag or plain arg, ok
    # Check if using npx without -y (interactive prompt in CI)
    command = server_config.get("command", "")
    if command == "npx" and "-y" not in args:
        findings.append({
            "severity": "LOW",
            "check": "npx-interactive",
            "message": "npx without -y flag may prompt interactively in CI",
            "fix": "Add -y flag: npx -y package-name"
        })
    return findings
```
**好的固定版本：**```json
{ "args": ["-y", "my-mcp-server@2.1.0"] }
```
**坏-未固定：**```json
{ "args": ["-y", "my-mcp-server@latest"] }
```
---

##审计检查4：完全审计运行程序

将所有检查合并为单个审计。```python
def audit_mcp_config(mcp_path: str) -> dict:
    """Run full security audit on an .mcp.json file."""
    path = Path(mcp_path)
    if not path.exists():
        return {"error": f"{mcp_path} not found"}

    config = json.loads(path.read_text(encoding="utf-8"))
    servers = config.get("mcpServers", {})
    results = {"file": str(path), "servers": {}, "summary": {}}
    total_findings = []

    # Run secrets check once on the whole config (not per-server)
    config_level_findings = check_secrets(config)
    total_findings.extend(config_level_findings)

    for name, server_config in servers.items():
        if not isinstance(server_config, dict):
            continue
        findings = []
        findings.extend(check_shell_injection(server_config))
        findings.extend(check_pinned_versions(server_config))
        results["servers"][name] = {
            "command": server_config.get("command", ""),
            "findings": findings,
        }
        total_findings.extend(findings)

    # Summary
    by_severity = {}
    for f in total_findings:
        sev = f["severity"]
        by_severity[sev] = by_severity.get(sev, 0) + 1

    results["summary"] = {
        "total_servers": len(servers),
        "total_findings": len(total_findings),
        "by_severity": by_severity,
        "passed": len(total_findings) == 0,
    }
    return results
```
* *用法:* *```python
results = audit_mcp_config(".mcp.json")
if not results["summary"]["passed"]:
    for server, data in results["servers"].items():
        for finding in data["findings"]:
            print(f"[{finding['severity']}] {server}: {finding['message']}")
            print(f"  Fix: {finding['fix']}")
```
---

##输出格式```
MCP Security Audit — .mcp.json
═══════════════════════════════
Servers scanned: 5
Findings: 3 (1 CRITICAL, 1 HIGH, 1 MEDIUM)

[CRITICAL] my-api-server: Hardcoded secret found in MCP configuration
  Fix: Use environment variable references: ${ENV_VAR_NAME}

[HIGH] data-processor: Dangerous pattern in MCP server args: bash -c execution
  Fix: Use direct command execution, not shell interpolation

[MEDIUM] analytics: Unpinned dependency: analytics-mcp@latest
  Fix: Pin to specific version: analytics-mcp@2.1.0
```
---

##相关资源

- [MCP规格]（https://modelcontextprotocol.io/）
-[代理治理工具包](https://github.com/microsoft/agent-governance-toolkit) -完整的治理框架与MCP信任代理
- [OWASP ASI-02：不安全的工具使用]（https://owasp.org/www-project-agentic-ai-threats/）