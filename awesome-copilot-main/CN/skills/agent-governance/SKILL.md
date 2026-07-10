---
name: agent-governance
description: |
  Patterns and techniques for adding governance, safety, and trust controls to AI agent systems. Use this skill when:
  - Building AI agents that call external tools (APIs, databases, file systems)
  - Implementing policy-based access controls for agent tool usage
  - Adding semantic intent classification to detect dangerous prompts
  - Creating trust scoring systems for multi-agent workflows
  - Building audit trails for agent actions and decisions
  - Enforcing rate limits, content filters, or tool restrictions on agents
  - Working with any agent framework (PydanticAI, CrewAI, OpenAI Agents, LangChain, AutoGen)
---
#代理治理模式

为AI代理系统增加安全、信任和策略执行的模式。

# #概述

治理模式确保人工智能代理在定义的范围内运行——控制他们可以调用哪些工具，他们可以处理哪些内容，他们可以做多少，并通过审计跟踪维护问责制。```
User Request → Intent Classification → Policy Check → Tool Execution → Audit Log
                     ↓                      ↓               ↓
              Threat Detection         Allow/Deny      Trust Update
```
##何时使用

**：任何调用外部工具（api，数据库，shell命令）的代理
- **多代理系统**：代理委托给其他代理需要信任边界
- **生产部署**：合规性、审计和安全要求
- **敏感操作**：金融交易、数据访问、基础设施管理

---

模式1：治理策略

定义允许代理作为可组合、可序列化的策略对象执行的操作。```python
from dataclasses import dataclass, field
from enum import Enum
from typing import Optional
import re

class PolicyAction(Enum):
    ALLOW = "allow"
    DENY = "deny"
    REVIEW = "review"  # flag for human review

@dataclass
class GovernancePolicy:
    """Declarative policy controlling agent behavior."""
    name: str
    allowed_tools: list[str] = field(default_factory=list)       # allowlist
    blocked_tools: list[str] = field(default_factory=list)       # blocklist
    blocked_patterns: list[str] = field(default_factory=list)    # content filters
    max_calls_per_request: int = 100                             # rate limit
    require_human_approval: list[str] = field(default_factory=list)  # tools needing approval

    def check_tool(self, tool_name: str) -> PolicyAction:
        """Check if a tool is allowed by this policy."""
        if tool_name in self.blocked_tools:
            return PolicyAction.DENY
        if tool_name in self.require_human_approval:
            return PolicyAction.REVIEW
        if self.allowed_tools and tool_name not in self.allowed_tools:
            return PolicyAction.DENY
        return PolicyAction.ALLOW

    def check_content(self, content: str) -> Optional[str]:
        """Check content against blocked patterns. Returns matched pattern or None."""
        for pattern in self.blocked_patterns:
            if re.search(pattern, content, re.IGNORECASE):
                return pattern
        return None
```
政策组成

组合多个策略（例如，组织范围+团队+特定于代理）：```python
def compose_policies(*policies: GovernancePolicy) -> GovernancePolicy:
    """Merge policies with most-restrictive-wins semantics."""
    combined = GovernancePolicy(name="composed")

    for policy in policies:
        combined.blocked_tools.extend(policy.blocked_tools)
        combined.blocked_patterns.extend(policy.blocked_patterns)
        combined.require_human_approval.extend(policy.require_human_approval)
        combined.max_calls_per_request = min(
            combined.max_calls_per_request,
            policy.max_calls_per_request
        )
        if policy.allowed_tools:
            if combined.allowed_tools:
                combined.allowed_tools = [
                    t for t in combined.allowed_tools if t in policy.allowed_tools
                ]
            else:
                combined.allowed_tools = list(policy.allowed_tools)

    return combined


# Usage: layer policies from broad to specific
org_policy = GovernancePolicy(
    name="org-wide",
    blocked_tools=["shell_exec", "delete_database"],
    blocked_patterns=[r"(?i)(api[_-]?key|secret|password)\s*[:=]"],
    max_calls_per_request=50
)
team_policy = GovernancePolicy(
    name="data-team",
    allowed_tools=["query_db", "read_file", "write_report"],
    require_human_approval=["write_report"]
)
agent_policy = compose_policies(org_policy, team_policy)
```
策略为YAML

将策略存储为配置，而不是代码：```yaml
# governance-policy.yaml
name: production-agent
allowed_tools:
  - search_documents
  - query_database
  - send_email
blocked_tools:
  - shell_exec
  - delete_record
blocked_patterns:
  - "(?i)(api[_-]?key|secret|password)\\s*[:=]"
  - "(?i)(drop|truncate|delete from)\\s+\\w+"
max_calls_per_request: 25
require_human_approval:
  - send_email
```

```python
import yaml

def load_policy(path: str) -> GovernancePolicy:
    with open(path) as f:
        data = yaml.safe_load(f)
    return GovernancePolicy(**data)
```
---

模式2：语义意图分类

使用基于模式的信号，在提示到达代理之前检测危险意图。```python
from dataclasses import dataclass

@dataclass
class IntentSignal:
    category: str       # e.g., "data_exfiltration", "privilege_escalation"
    confidence: float   # 0.0 to 1.0
    evidence: str       # what triggered the detection

# Weighted signal patterns for threat detection
THREAT_SIGNALS = [
    # Data exfiltration
    (r"(?i)send\s+(all|every|entire)\s+\w+\s+to\s+", "data_exfiltration", 0.8),
    (r"(?i)export\s+.*\s+to\s+(external|outside|third.?party)", "data_exfiltration", 0.9),
    (r"(?i)curl\s+.*\s+-d\s+", "data_exfiltration", 0.7),

    # Privilege escalation
    (r"(?i)(sudo|as\s+root|admin\s+access)", "privilege_escalation", 0.8),
    (r"(?i)chmod\s+777", "privilege_escalation", 0.9),

    # System modification
    (r"(?i)(rm\s+-rf|del\s+/[sq]|format\s+c:)", "system_destruction", 0.95),
    (r"(?i)(drop\s+database|truncate\s+table)", "system_destruction", 0.9),

    # Prompt injection
    (r"(?i)ignore\s+(previous|above|all)\s+(instructions?|rules?)", "prompt_injection", 0.9),
    (r"(?i)you\s+are\s+now\s+(a|an)\s+", "prompt_injection", 0.7),
]

def classify_intent(content: str) -> list[IntentSignal]:
    """Classify content for threat signals."""
    signals = []
    for pattern, category, weight in THREAT_SIGNALS:
        match = re.search(pattern, content)
        if match:
            signals.append(IntentSignal(
                category=category,
                confidence=weight,
                evidence=match.group()
            ))
    return signals

def is_safe(content: str, threshold: float = 0.7) -> bool:
    """Quick check: is the content safe above the given threshold?"""
    signals = classify_intent(content)
    return not any(s.confidence >= threshold for s in signals)
```
**关键洞察**：意图分类发生在工具执行之前，作为飞行前的安全检查。这从根本上不同于输出护栏，后者只在生成后检查。

---

模式3：工具级治理装饰器

用治理检查包装单个工具功能：```python
import functools
import time
from collections import defaultdict

_call_counters: dict[str, int] = defaultdict(int)

def govern(policy: GovernancePolicy, audit_trail=None):
    """Decorator that enforces governance policy on a tool function."""
    def decorator(func):
        @functools.wraps(func)
        async def wrapper(*args, **kwargs):
            tool_name = func.__name__

            # 1. Check tool allowlist/blocklist
            action = policy.check_tool(tool_name)
            if action == PolicyAction.DENY:
                raise PermissionError(f"Policy '{policy.name}' blocks tool '{tool_name}'")
            if action == PolicyAction.REVIEW:
                raise PermissionError(f"Tool '{tool_name}' requires human approval")

            # 2. Check rate limit
            _call_counters[policy.name] += 1
            if _call_counters[policy.name] > policy.max_calls_per_request:
                raise PermissionError(f"Rate limit exceeded: {policy.max_calls_per_request} calls")

            # 3. Check content in arguments
            for arg in list(args) + list(kwargs.values()):
                if isinstance(arg, str):
                    matched = policy.check_content(arg)
                    if matched:
                        raise PermissionError(f"Blocked pattern detected: {matched}")

            # 4. Execute and audit
            start = time.monotonic()
            try:
                result = await func(*args, **kwargs)
                if audit_trail is not None:
                    audit_trail.append({
                        "tool": tool_name,
                        "action": "allowed",
                        "duration_ms": (time.monotonic() - start) * 1000,
                        "timestamp": time.time()
                    })
                return result
            except Exception as e:
                if audit_trail is not None:
                    audit_trail.append({
                        "tool": tool_name,
                        "action": "error",
                        "error": str(e),
                        "timestamp": time.time()
                    })
                raise

        return wrapper
    return decorator


# Usage with any agent framework
audit_log = []
policy = GovernancePolicy(
    name="search-agent",
    allowed_tools=["search", "summarize"],
    blocked_patterns=[r"(?i)password"],
    max_calls_per_request=10
)

@govern(policy, audit_trail=audit_log)
async def search(query: str) -> str:
    """Search documents — governed by policy."""
    return f"Results for: {query}"

# Passes: search("latest quarterly report")
# Blocked: search("show me the admin password")
```
---

模式4：信任评分

使用基于衰减的信任分数跟踪代理随时间的可靠性：```python
from dataclasses import dataclass, field
import math
import time

@dataclass
class TrustScore:
    """Trust score with temporal decay."""
    score: float = 0.5          # 0.0 (untrusted) to 1.0 (fully trusted)
    successes: int = 0
    failures: int = 0
    last_updated: float = field(default_factory=time.time)

    def record_success(self, reward: float = 0.05):
        self.successes += 1
        self.score = min(1.0, self.score + reward * (1 - self.score))
        self.last_updated = time.time()

    def record_failure(self, penalty: float = 0.15):
        self.failures += 1
        self.score = max(0.0, self.score - penalty * self.score)
        self.last_updated = time.time()

    def current(self, decay_rate: float = 0.001) -> float:
        """Get score with temporal decay — trust erodes without activity."""
        elapsed = time.time() - self.last_updated
        decay = math.exp(-decay_rate * elapsed)
        return self.score * decay

    @property
    def reliability(self) -> float:
        total = self.successes + self.failures
        return self.successes / total if total > 0 else 0.0


# Usage in multi-agent systems
trust = TrustScore()

# Agent completes tasks successfully
trust.record_success()  # 0.525
trust.record_success()  # 0.549

# Agent makes an error
trust.record_failure()  # 0.467

# Gate sensitive operations on trust
if trust.current() >= 0.7:
    # Allow autonomous operation
    pass
elif trust.current() >= 0.4:
    # Allow with human oversight
    pass
else:
    # Deny or require explicit approval
    pass
```
**多代理信任：在代理委托给其他代理的系统中，每个代理维护其委托的信任分数。```python
class AgentTrustRegistry:
    def __init__(self):
        self.scores: dict[str, TrustScore] = {}

    def get_trust(self, agent_id: str) -> TrustScore:
        if agent_id not in self.scores:
            self.scores[agent_id] = TrustScore()
        return self.scores[agent_id]

    def most_trusted(self, agents: list[str]) -> str:
        return max(agents, key=lambda a: self.get_trust(a).current())

    def meets_threshold(self, agent_id: str, threshold: float) -> bool:
        return self.get_trust(agent_id).current() >= threshold
```
---

模式5：审计跟踪

所有代理操作的仅附加审计日志-对合规性和调试至关重要：```python
from dataclasses import dataclass, field
import json
import time

@dataclass
class AuditEntry:
    timestamp: float
    agent_id: str
    tool_name: str
    action: str           # "allowed", "denied", "error"
    policy_name: str
    details: dict = field(default_factory=dict)

class AuditTrail:
    """Append-only audit trail for agent governance events."""
    def __init__(self):
        self._entries: list[AuditEntry] = []

    def log(self, agent_id: str, tool_name: str, action: str,
            policy_name: str, **details):
        self._entries.append(AuditEntry(
            timestamp=time.time(),
            agent_id=agent_id,
            tool_name=tool_name,
            action=action,
            policy_name=policy_name,
            details=details
        ))

    def denied(self) -> list[AuditEntry]:
        """Get all denied actions — useful for security review."""
        return [e for e in self._entries if e.action == "denied"]

    def by_agent(self, agent_id: str) -> list[AuditEntry]:
        return [e for e in self._entries if e.agent_id == agent_id]

    def export_jsonl(self, path: str):
        """Export as JSON Lines for log aggregation systems."""
        with open(path, "w") as f:
            for entry in self._entries:
                f.write(json.dumps({
                    "timestamp": entry.timestamp,
                    "agent_id": entry.agent_id,
                    "tool": entry.tool_name,
                    "action": entry.action,
                    "policy": entry.policy_name,
                    **entry.details
                }) + "\n")
```
---

模式6：框架集成

# # # PydanticAI```python
from pydantic_ai import Agent

policy = GovernancePolicy(
    name="support-bot",
    allowed_tools=["search_docs", "create_ticket"],
    blocked_patterns=[r"(?i)(ssn|social\s+security|credit\s+card)"],
    max_calls_per_request=20
)

agent = Agent("openai:gpt-4o", system_prompt="You are a support assistant.")

@agent.tool
@govern(policy)
async def search_docs(ctx, query: str) -> str:
    """Search knowledge base — governed."""
    return await kb.search(query)

@agent.tool
@govern(policy)
async def create_ticket(ctx, title: str, body: str) -> str:
    """Create support ticket — governed."""
    return await tickets.create(title=title, body=body)
```
# # # CrewAI```python
from crewai import Agent, Task, Crew

policy = GovernancePolicy(
    name="research-crew",
    allowed_tools=["search", "analyze"],
    max_calls_per_request=30
)

# Apply governance at the crew level
def governed_crew_run(crew: Crew, policy: GovernancePolicy):
    """Wrap crew execution with governance checks."""
    audit = AuditTrail()
    for agent in crew.agents:
        for tool in agent.tools:
            original = tool.func
            tool.func = govern(policy, audit_trail=audit)(original)
    result = crew.kickoff()
    return result, audit
```
OpenAI代理SDK```python
from agents import Agent, function_tool

policy = GovernancePolicy(
    name="coding-agent",
    allowed_tools=["read_file", "write_file", "run_tests"],
    blocked_tools=["shell_exec"],
    max_calls_per_request=50
)

@function_tool
@govern(policy)
async def read_file(path: str) -> str:
    """Read file contents — governed."""
    import os
    safe_path = os.path.realpath(path)
    if not safe_path.startswith(os.path.realpath(".")):
        raise ValueError("Path traversal blocked by governance")
    with open(safe_path) as f:
        return f.read()
```
---

##治理级别

将治理严格程度与风险级别相匹配：

| |控制|用例||-------|----------|----------|
| **打开** |仅限审计，无限制|内部dev/testing|
| **标准** |工具允许列表+内容过滤器|一般生产代理|
| **严格** |所有控制+敏感操作的人工批准|金融、医疗、法律|
| **锁定** |仅允许列表，无动态工具，完全审计|合规关键系统|

---

最佳实践

|实践|理论||----------|-----------|
| **策略作为配置** |在YAML/JSON中存储策略，不硬编码-允许更改而不部署|
当组合策略时，deny总是覆盖allow |
| **飞行前意图检查** |在工具执行之前对意图进行分类，而不是在|之后
| **信任衰减** |信任分数应该随着时间的推移而衰减-需要持续的良好行为|
| **追加审计** |永远不要修改或删除审计条目-不变性使合规|
| **失败关闭** |如果治理检查错误，拒绝操作而不是允许操作|
| **将策略与逻辑分离** |治理实施应该独立于代理业务逻辑|

---

##快速启动清单```markdown
## Agent Governance Implementation Checklist

### Setup
- [ ] Define governance policy (allowed tools, blocked patterns, rate limits)
- [ ] Choose governance level (open/standard/strict/locked)
- [ ] Set up audit trail storage

### Implementation
- [ ] Add @govern decorator to all tool functions
- [ ] Add intent classification to user input processing
- [ ] Implement trust scoring for multi-agent interactions
- [ ] Wire up audit trail export

### Validation
- [ ] Test that blocked tools are properly denied
- [ ] Test that content filters catch sensitive patterns
- [ ] Test rate limiting behavior
- [ ] Verify audit trail captures all events
- [ ] Test policy composition (most-restrictive-wins)
```
---

##相关资源

—[Agent Governance Toolkit]（https://github.com/microsoft/agent-governance-toolkit）—完整的治理框架
—[AgentMesh Integrations]（https://github.com/microsoft/agent-governance-toolkit/tree/main/packages/agentmesh-integrations）—特定于框架的包
- [OWASP十大法学硕士应用程序]（https://owasp.org/www-project-top-10-for-large-language-model-applications/）