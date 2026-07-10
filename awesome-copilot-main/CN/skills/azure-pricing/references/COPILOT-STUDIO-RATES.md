#副驾驶工作室-计费率和估计

>来源：[计费费率和管理]（https://learn.microsoft.com/en-us/microsoft-copilot-studio/requirements-messages-management）
> Estimator: [Microsoft代理使用估计器]（https://microsoft.github.io/copilot-studio-estimator/）
>许可指南：[Copilot Studio许可指南]（https://go.microsoft.com/fwlink/?linkid=2320995）

副驾驶的信用率

**1副驾驶积分= $0.01 USD**

计费费率（缓存快照-最后更新于2026年3月）

**重要：总是喜欢从下面的源url获取实时费率。只有当web获取不可用时，才可以使用这个表

|特性|速率|单位||---|---|---|
|经典回答|每个回答|
|生成式应答|每个应答| 2 |
每个动作（触发、深度推理、主题转换、计算机使用）|
|租户图接地| 10 |每条消息|
|代理流程操作|每100个流程操作| 13 |
|文本和人工智能工具（基本）||每10个回复|
|文本和人工智能工具（标准）|每10个回复| 15 |
|文本和人工智能工具（高级）|每10个回复| 100 |
|内容处理工具| 8 |每页|

# # #笔记- **经典答案**：预定义的，手动撰写的回答。静态——除非开发者更新，否则不要改变。
- **生成答案**：使用AI模型（gpt）动态生成。根据上下文和知识来源进行调整。
- **租户图接地**：基于租户范围的Microsoft graph的RAG，包括通过连接器的外部数据。每个代理可选。
- **代理行动**：步骤像触发器，深度推理，主题转换可见的活动图。包括计算机使用代理。
- **文本和人工智能工具**：嵌入在代理中的提示工具。基于底层语言模型的三个层（basic/standard/premium）。
—**代理流程动作**：预定义的流程动作序列，每一步执行时不需要代理reasoning/orchestration。

推理模型计费

当使用推理能力模型时：```
Total cost = feature rate for operation + text & gen AI tools (premium) per 10 responses
```
示例：使用推理模型的生成式答案花费**2学分**（生成式答案）**+ 10学分**（每个回答的溢价，从100/10按比例计算）。

##估算公式

# # #输入

| |参数说明||---|---|
|`users`|终端用户数量|
|`interactions_per_month`|每个用户每月平均互动|
|`knowledge_pct`|知识来源回复的百分比（0-100）|
|`tenant_graph_pct`|知识响应中，%使用租户图接地（0-100）|
|`tool_prompt`|每个会话的平均提示工具调用|
|`tool_agent_flow`|每会话平均座席流呼叫数|
|`tool_computer_use`|每个会话的平均计算机使用呼叫数|
|`tool_custom_connector`|每个会话的平均自定义连接器调用|
|`tool_mcp`|每个会话平均MCP（模型上下文协议）调用|
|`tool_rest_api`|每个会话的平均REST API调用|
|`prompts_basic`|每个会话的平均基本AI提示使用|
|`prompts_standard`|每个会话的平均标准AI提示使用|
|`prompts_premium`|每次会话的平均高级AI提示使用|

# # #计算```
total_sessions = users × interactions_per_month

── Knowledge Credits ──
tenant_graph_credits    = total_sessions × (knowledge_pct/100) × (tenant_graph_pct/100) × 10
generative_answer_credits = total_sessions × (knowledge_pct/100) × (1 - tenant_graph_pct/100) × 2
classic_answer_credits  = total_sessions × (1 - knowledge_pct/100) × 1

── Agent Tools Credits ──
tool_calls = total_sessions × (prompt + computer_use + custom_connector + mcp + rest_api)
tool_credits = tool_calls × 5

── Agent Flow Credits ──
flow_calls = total_sessions × tool_agent_flow
flow_credits = ceil(flow_calls / 100) × 13

── Prompt Modifier Credits ──
basic_credits    = ceil(total_sessions × prompts_basic / 10) × 1
standard_credits = ceil(total_sessions × prompts_standard / 10) × 15
premium_credits  = ceil(total_sessions × prompts_premium / 10) × 100

── Total ──
total_credits = knowledge + tools + flows + prompts
cost_usd = total_credits × 0.01
```
##计费示例（来自Microsoft Docs）

客户支持代理

-每个会话4个经典答案+ 2个生成答案
- 900customers/day- **每日**:`[(4×1) + (2×2)] × 900 = 7,200 credits`- * *月(30 d) * *: 2160美元~ 216000学分= * * ~ * *

销售业绩代理（租户图接地）

-每个会话4个生成式答案+ 4个基于租户图的响应
- 100个未授权用户
- **每日**:`[(4×2) + (4×10)] × 100 = 4,800 credits`- * *月(30 d) * *: 1440美元~ 144000学分= * * ~ * *

订单处理代理

-每次触发4次动作调用（自主）
- **每个触发器**:`4 × 5 = 20 credits`员工与客户代理类型

|代理类型|包含在M365副驾驶？||---|---|
|面向员工（BtoE） |当用户拥有Microsoft 365 Copilot许可证|时，经典答案、生成答案和租户图基础都是零成本的
|Customer/partner-facing|所有使用都正常计费|

##过度执法

-以预付容量的125%触发
-自定义代理被禁用（正在进行的对话继续）
—发送给租户admin的邮件通知
-解决方案：重新分配容量，购买更多，或启用按需付费

##实时源url

要获取最新费率，请从以下页面获取内容：

-[计费费率及管理]（https://learn.microsoft.com/en-us/microsoft-copilot-studio/requirements-messages-management）
-[副驾驶工作室许可]（https://learn.microsoft.com/en-us/microsoft-copilot-studio/billing-licensing）
- [Copilot Studio授权指南（PDF）]（https://go.microsoft.com/fwlink/?linkid=2320995）