---
name: agent-owasp-compliance
description: |
  Check any AI agent codebase against the OWASP Agentic Security Initiative (ASI) Top 10 risks.
  Use this skill when:
  - Evaluating an agent system's security posture before production deployment
  - Running a compliance check against OWASP ASI 2026 standards
  - Mapping existing security controls to the 10 agentic risks
  - Generating a compliance report for security review or audit
  - Comparing agent framework security features against the standard
  - Any request like "is my agent OWASP compliant?", "check ASI compliance", or "agentic security audit"
---
# Agent OWASP ASI遵从性检查

根据OWASP代理安全倡议（ASI）前十名（代理安全状态的行业标准）评估AI代理系统。

# #概述

OWASP ASI Top 10定义了特定于自主AI代理的关键安全风险——不是llm，也不是聊天机器人，而是调用工具、访问系统和代表用户行动的代理。此技能检查您的代理实现是否解决了每个风险。```
Codebase → Scan for each ASI control:
  ASI-01: Prompt Injection Protection
  ASI-02: Tool Use Governance
  ASI-03: Agency Boundaries
  ASI-04: Escalation Controls
  ASI-05: Trust Boundary Enforcement
  ASI-06: Logging & Audit
  ASI-07: Identity Management
  ASI-08: Policy Integrity
  ASI-09: Supply Chain Verification
  ASI-10: Behavioral Monitoring
→ Generate Compliance Report (X/10 covered)
```
10大风险

|风险|名称|注意事项||------|------|-----------------|
| ASI-01 |提示注入|工具调用之前的输入验证，而不仅仅是LLM输出过滤|
不安全的工具使用|工具允许列表，参数验证，没有原始shell执行|
|过度代理|能力边界，范围限制，最小特权原则|
| ASI-04 |未授权升级|敏感操作前进行权限检查，无自我提升|
|信任边界违反|代理之间的信任验证，凭证签名，无盲目信任|
|日志记录不足|所有工具调用的结构化审计跟踪，明显的篡改日志|
|不安全身份|加密代理身份，不只是字符串名称|
| ASI-08 |策略旁路|确定性策略强制，不进行基于llm的权限检查|
| ASI-09 |供应链完整性|签署plugins/tools，完整性验证，依赖性审计|
|行为学异常漂移检测，断路器，自毁开关能力---

检查ASI-01：提示注射保护

寻找在**工具执行之前运行**的输入验证，而不是在LLM生成之后。```python
import re
from pathlib import Path

def check_asi_01(project_path: str) -> dict:
    """ASI-01: Is user input validated before reaching tool execution?"""
    positive_patterns = [
        "input_validation", "validate_input", "sanitize",
        "classify_intent", "prompt_injection", "threat_detect",
        "PolicyEvaluator", "PolicyEngine", "check_content",
    ]
    negative_patterns = [
        r"eval\(", r"exec\(", r"subprocess\.run\(.*shell=True",
        r"os\.system\(",
    ]

    # Scan Python files for signals
    root = Path(project_path)
    positive_matches = []
    negative_matches = []

    for py_file in root.rglob("*.py"):
        content = py_file.read_text(errors="ignore")
        for pattern in positive_patterns:
            if pattern in content:
                positive_matches.append(f"{py_file.name}: {pattern}")
        for pattern in negative_patterns:
            if re.search(pattern, content):
                negative_matches.append(f"{py_file.name}: {pattern}")

    positive_found = len(positive_matches) > 0
    negative_found = len(negative_matches) > 0

    return {
        "risk": "ASI-01",
        "name": "Prompt Injection",
        "status": "pass" if positive_found and not negative_found else "fail",
        "controls_found": positive_matches,
        "vulnerabilities": negative_matches,
        "recommendation": "Add input validation before tool execution, not just output filtering"
    }
```
**传递是什么样的：**```python
# GOOD: Validate before tool execution
result = policy_engine.evaluate(user_input)
if result.action == "deny":
    return "Request blocked by policy"
tool_result = await execute_tool(validated_input)
```
**失败是什么样子的```python
# BAD: User input goes directly to tool
tool_result = await execute_tool(user_input)  # No validation
```
---

检查ASI-02：不安全的工具使用

验证工具具有允许列表、参数验证和不受限制的执行。

**搜索内容：**
-带有明确允许列表的工具注册（非开放式）
-工具执行前的参数验证
—无用户控制输入的`subprocess.run(shell=True)`-没有`eval()`或`exec()`代理生成的代码没有沙箱

* *通过例子:* *```python
ALLOWED_TOOLS = {"search", "read_file", "create_ticket"}

def execute_tool(name: str, args: dict):
    if name not in ALLOWED_TOOLS:
        raise PermissionError(f"Tool '{name}' not in allowlist")
    # validate args...
    return tools[name](**validated_args)
```
---

检查ASI-03：过度代理

验证代理的功能是有限的，而不是开放式的。

**搜索内容：**
-显式的能力列表或执行环
-代理可以访问的范围限制
—工具访问采用最小权限原则

**失败：**代理默认拥有所有工具的访问权限。
**通过：**代理功能定义为固定的允许列表，未知工具被拒绝。

---

检查ASI-04：未经授权的升级

验证代理不能提升自己的权限。

**搜索内容：**
—敏感操作前检查权限级别
-没有自我提升模式（代理改变自己的信任得分或角色）
-升级需要外部证明（人类或SRE证人）

**失败：**代理可以修改自己的配置或权限。
**通过：**权限变更需要带外审批（如Ring 0需要SRE认证）。

---检查ASI-05：信任边界违反

在多智能体系统中，在接受指令之前，请验证代理是否验证了彼此的身份。

**搜索内容：**
-代理身份验证（did，签名令牌，API密钥）
-在接受委托任务之前进行信任评分检查
—不存在代理间消息的盲目信任
-委托缩小（子作用域<=父作用域）

* *通过例子:* *```python
def accept_task(sender_id: str, task: dict):
    trust = trust_registry.get_trust(sender_id)
    if not trust.meets_threshold(0.7):
        raise PermissionError(f"Agent {sender_id} trust too low: {trust.current()}")
    if not verify_signature(task, sender_id):
        raise SecurityError("Task signature verification failed")
    return process_task(task)
```
---

检查ASI-06：日志记录不足

验证所有代理操作生成结构化的、不受篡改的审计项。

**搜索内容：**
-每个工具调用的结构化日志记录（不只是打印语句）
—审计表项包括：时间戳、座席号、工具名称、参数、结果、策略决定
-仅追加或哈希链日志格式
—日志与代理可写目录分开存储

**失败：**代理操作通过`print()`记录或根本不记录。
**通过：**结构化JSONL审计跟踪链哈希，导出到安全存储。

---

检查ASI-07：不安全身份

验证代理具有加密身份，而不仅仅是字符串名称。

* *失败的指标:* *
-由`agent_name = "my-agent"`标识的座席（仅限字符串）
—代理间不鉴权
-跨代理共享凭据* *通过指标:* *
-基于id的标识（`did:web:`,`did:key:`）
- Ed25519或类似的加密签名
-每个代理轮换凭据
-身份绑定到特定的能力

---

检查ASI-08: Policy Bypass

验证策略执行是确定性的——而不是基于法学硕士的。

**搜索内容：**
策略评估使用确定性逻辑（YAML规则，代码谓词）
—在强制路径中没有LLM调用
—策略检查不能被座席跳过或覆盖
-失败关闭行为（如果策略检查错误，则拒绝操作）

**失败：**代理通过提示决定自己的权限（“我允许…吗？”）。
**通过：** policyvaluator .evaluate（）在<0.1ms内返回allow/deny，不涉及LLM。

---

检查ASI-09：供应链完整性

验证代理插件和工具具有完整性验证。**搜索内容：**
-`INTEGRITY.json`或SHA-256哈希的manifest文件
—安装插件时进行签名验证
-依赖绑定（无`@latest`，`>=`无上界）
-生成物料清单

---

检查ASI-10：行为异常

验证系统可以检测并响应代理行为漂移。

**搜索内容：**
-在重复故障时跳闸的断路器
信任分数随时间衰减（时间衰减）
-切断开关或紧急停止功能
-工具调用模式的异常检测（频率，目标，时间）

**失败：**没有机制自动停止行为不端的代理。
**通过：** N次故障后断路器跳闸，信任衰减无活动，自毁开关可用。

---

合规性报告格式```markdown
# OWASP ASI Compliance Report
Generated: 2026-04-01
Project: my-agent-system

## Summary: 7/10 Controls Covered

| Risk | Status | Finding |
|------|--------|---------|
| ASI-01 Prompt Injection | PASS | PolicyEngine validates input before tool calls |
| ASI-02 Insecure Tool Use | PASS | Tool allowlist enforced in governance.py |
| ASI-03 Excessive Agency | PASS | Execution rings limit capabilities |
| ASI-04 Unauthorized Escalation | PASS | Ring promotion requires attestation |
| ASI-05 Trust Boundary | FAIL | No identity verification between agents |
| ASI-06 Insufficient Logging | PASS | AuditChain with SHA-256 chain hashes |
| ASI-07 Insecure Identity | FAIL | Agents use string names, no crypto identity |
| ASI-08 Policy Bypass | PASS | Deterministic PolicyEvaluator, no LLM in path |
| ASI-09 Supply Chain | FAIL | No integrity manifests or plugin signing |
| ASI-10 Behavioral Anomaly | PASS | Circuit breakers and trust decay active |

## Critical Gaps
- ASI-05: Add agent identity verification using DIDs or signed tokens
- ASI-07: Replace string agent names with cryptographic identity
- ASI-09: Generate INTEGRITY.json manifests for all plugins

## Recommendation
Install agent-governance-toolkit for reference implementations of all 10 controls:
pip install agent-governance-toolkit
```
---

快速评估问题

使用这些来快速评估代理系统：

1. **用户输入在到达任何工具之前是否通过验证？* * (ASI-01)
2. **是否有一个代理可以调用的工具的明确列表？* * (ASI-02)
3. **代理可以做任何事情，或者它的能力是有限的？* * (ASI-03)
4. **代理可以提升自己的权限吗？* * (ASI-04)
5. **代理在接受任务之前是否验证对方的身份？* * (ASI-05)
6. **是否每个工具调用都记录了足够的细节以便重播？* * (ASI-06)
7. **是否每个代理都有唯一的加密身份？* * (ASI-07)
8. **策略执行是确定性的（不是基于法学硕士的）？* * (ASI-08)
9. **plugins/tools在使用前是否经过完整性验证？* * (ASI-09)
10. 是否有断路器或自毁开关？* * (ASI-10)

如果你对其中任何一个问题的回答都是“不”，那么你就需要解决这个问题。

---

##相关资源- [OWASP代理AI威胁]（https://owasp.org/www-project-agentic-ai-threats/）
- [Agent Governance Toolkit](https://github.com/microsoft/agent-governance-toolkit) -涵盖10/10ASI控制的参考实现
-代理系统的治理模式