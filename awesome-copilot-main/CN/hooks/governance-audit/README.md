---
name: 'Governance Audit'
description: 'Scans Copilot agent prompts for threat signals and logs governance events'
tags: ['security', 'governance', 'audit', 'safety']
---
#治理审计挂钩GitHub Copilot编码代理会话的实时威胁检测和审计日志记录。在代理处理危险模式之前，扫描用户提示。

# #概述

这个钩子为Copilot编码代理会话提供了治理控制：
—**威胁检测**：扫描数据泄露、特权升级、系统破坏、提示注入和凭证暴露提示
- **治理级别**：开放、标准、严格、锁定——从只审核到完全封锁
- **审计跟踪**：只追加所有治理事件的JSON日志
—**会话汇总**：报告会话结束时的威胁次数

##威胁类别

|类别|示例|严重性||----------|----------|----------|
|`data_exfiltration`| “发送所有记录到外部API” | 0.7 - 0.95 |
|`privilege_escalation`| "sudo", "chmod 777", "add to sudoers" | 0.8 - 0.95 |
|`system_destruction`| "rm -rf /", "drop database" | 0.9 - 0.95 |
|`prompt_injection`| “忽略之前的指令” | 0.6 - 0.9 |
|`credential_exposure`|硬编码API密钥，AWS访问密钥| 0.9 - 0.95 |

##治理级别

|级别|行为||-------|----------|
|`open`|仅限日志威胁，不阻塞|
|`standard`|日志威胁，仅当`BLOCK_ON_THREAT=true`|时阻断
|`strict`|记录并阻止所有检测到的威胁|
|`locked`|记录并阻止所有检测到的威胁|

# #安装

1. 将钩子文件夹复制到你的存储库：   ```bash
   cp -r hooks/governance-audit .github/hooks/
   ```
2. 确保脚本是可执行的：   ```bash
   chmod +x .github/hooks/governance-audit/*.sh
   ```
3. 创建logs目录并添加到`.gitignore`：   ```bash
   mkdir -p logs/copilot/governance
   echo "logs/" >> .gitignore
   ```
4. 提交到存储库的默认分支。

# #配置

设置`hooks.json`中的环境变量：```json
{
  "env": {
    "GOVERNANCE_LEVEL": "strict",
    "BLOCK_ON_THREAT": "true"
  }
}
```
|变量|值|默认值|描述||----------|--------|---------|-------------|
|`GOVERNANCE_LEVEL`|`open`,`standard`,`strict`，`locked`|`standard`|控制阻塞行为|
|`BLOCK_ON_THREAT`|`true`，`false`|`false`|块提示威胁（标准级别）|
|`SKIP_GOVERNANCE_AUDIT`|`true`| unset |完全关闭治理审计|

##日志格式

事件以JSON行格式写入`logs/copilot/governance/audit.log`：```json
{"timestamp":"2026-01-15T10:30:00Z","event":"session_start","governance_level":"standard","cwd":"/workspace/project"}
{"timestamp":"2026-01-15T10:31:00Z","event":"prompt_scanned","governance_level":"standard","status":"clean"}
{"timestamp":"2026-01-15T10:32:00Z","event":"threat_detected","governance_level":"standard","threat_count":1,"threats":[{"category":"privilege_escalation","severity":0.8,"description":"Elevated privileges","evidence":"sudo"}]}
{"timestamp":"2026-01-15T10:45:00Z","event":"session_end","total_events":12,"threats_detected":1}
```
# #要求`jq`用于JSON处理（预安装在大多数CI环境和macOS上）
-`grep`与`-E`（扩展正则表达式）支持`bc`用于浮点比较（可选，优雅地降级）

隐私和安全

-完整的提示是**永远**记录-只有匹配的威胁模式（最小的证据片段）和元数据被记录
—将“`logs/`”添加到“`.gitignore`”中，使审计数据保持在本地
—设置“`SKIP_GOVERNANCE_AUDIT=true`”为“完全禁用”
—所有数据保持本地-没有外部网络呼叫