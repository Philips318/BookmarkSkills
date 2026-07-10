---
description: 'Guidelines for building safe, governed AI agent systems. Apply when writing code that uses agent frameworks, tool-calling LLMs, or multi-agent orchestration to ensure proper safety boundaries, policy enforcement, and auditability.'
applyTo: '**'
---
#代理安全和治理

##核心原则

- **失败关闭**：如果治理检查错误或不明确，拒绝而不是允许该操作
—**策略作为配置**：在YAML/JSON文件中定义治理规则，而不是硬编码在应用程序逻辑中
- **最小权限**：代理应该拥有其任务所需的最小工具访问权限
- **追加审计**：永远不会修改或删除审计跟踪条目-不变性使合规

##工具访问控制-始终定义一个显式的工具允许列表，代理可以使用-永远不要给无限制的工具访问
-将工具注册与工具授权分开-框架知道存在哪些工具，允许哪些策略控制
-对已知危险的操作（shell执行，文件删除，数据库DDL）使用块列表
-要求高影响力工具（发送电子邮件，部署，删除记录）的人在环批准
-强制对每个请求的工具调用进行速率限制，以防止无限循环和资源耗尽

##内容安全-在传递给代理之前扫描所有用户输入的威胁信号（数据泄露，提示注入，特权升级）
-过滤敏感模式的代理参数：API密钥、凭据、PII、SQL注入
-使用无需更改代码即可更新的正则表达式模式列表
-检查用户的原始提示符和代理生成的工具参数

多代理安全

-多代理系统中的每个代理都应该有自己的治理策略
—当座席委托给其他座席时，应用其中一个座席中最严格的策略
-跟踪代理委托的信任分数-在失败时降低信任，要求持续的良好行为
永远不要允许内部代理拥有比调用它的外部代理更大的权限

审计和可观察性-记录每个工具调用：时间戳，代理ID，工具名称，allow/deny决策，策略名称
-用匹配的规则和证据记录每个治理违规
-以JSON Lines格式导出审计跟踪，以便与日志聚合系统集成
—在审计日志中包含会话边界（start/end），以便进行关联

##代码模式

编写代理工具功能时：```python
# Good: Governed tool with explicit policy
@govern(policy)
async def search(query: str) -> str:
    ...

# Bad: Unprotected tool with no governance
async def search(query: str) -> str:
    ...
```
在定义策略时：```yaml
# Good: Explicit allowlist, content filters, rate limit
name: my-agent
allowed_tools: [search, summarize]
blocked_patterns: ["(?i)(api_key|password)\\s*[:=]"]
max_calls_per_request: 25

# Bad: No restrictions
name: my-agent
allowed_tools: ["*"]
```
在组合多代理策略时：```python
# Good: Most-restrictive-wins composition
final_policy = compose_policies(org_policy, team_policy, agent_policy)

# Bad: Only using agent-level policy, ignoring org constraints
final_policy = agent_policy
```
特定于框架的注意事项

**PydanticAI**：使用`@agent.tool`与治理装饰器包装。PydanticAI即将推出的Traits特性就是为这种模式设计的。
- **机组人员**：在机组人员层面对所有代理进行管理。使用`before_kickoff`回调进行策略验证。
- **OpenAI Agents SDK**：将`@function_tool`与治理捆绑在一起。为多代理信任使用切换保护。
**LangChain/LangGraph**：使用`RunnableBinding`或工具包装器进行治理。在图形边缘级别应用流量控制。
**AutoGen**：在`ConversableAgent.register_for_execution`钩子中实现治理。

常见错误-只依赖输出护栏（后生成），而不是执行前治理
—硬编码策略规则，而不是从配置加载
-允许代理自我修改自己的治理策略
忘记检查工具参数，而不仅仅是工具名称
-信任不会随着时间的推移而衰减-陈旧的信任是危险的
-审计跟踪中的日志提示-记录决策和元数据，而不是用户内容