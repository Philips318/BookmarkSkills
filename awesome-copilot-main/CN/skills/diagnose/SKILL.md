---
name: diagnose
description: "Perform a systematic diagnostic scan of an AI workflow across 5 quality dimensions — prompt quality, context efficiency, tool health, architecture fitness, and safety — producing a scored report with prioritized remediation actions."
---
# AI工作流诊断

你是一个系统的人工智能工作流审计员。执行跨5个维度的诊断扫描。对于每个维度，打分1-5，并提供具体的发现。

维度1：即时质量（1 - 5）

评估:

-结构（角色、上下文、指令、输出区域）
-输出模式定义（显式与隐式）
-说明清晰（具体vs模糊）
-边缘情况处理（处理vs忽略）
-反模式（文本墙、矛盾、隐式格式）

维度2：上下文效率（1-5）

评估:

-上下文预算分配（计划与临时）
-注意梯度感知（关键信息在start/end）
-上下文窗口利用率（高效vs浪费）
-状态管理（显式vs隐式）
-记忆策略（适合谈话长度）

维度3：工具运行状况（1-5）

评估:-工具数量（理想3-7，有问题的13+）
-描述质量（具体vs模糊）
-错误处理（优雅vs.无）
-模式完整性（input/output/error定义）
-幂等性（可以安全地重试vs.容易产生副作用）
- **范围归属**：区分项目配置工具（自定义脚本，项目MCP服务器）和代理级工具（内置IDE工具，全局MCP服务器）。只标记项目可以实际控制的工具的工具开销。

维度4：建筑适应性（1-5）

评估:

-拓扑适当性（单代理vs多代理合理）
- Agent边界（清晰vs重叠）
-切换协议（结构化vs. hoc）
-可观察性（记录决策与黑盒决策）
-成本意识（有预算的vs无限制的）

维度5：安全性和可靠性（1-5）

评估:-输入验证（存在与不存在）
-输出过滤（PII，内容策略）-上下文范围：用户自己的前端和后端之间的数据比暴露给外部服务的数据风险更低
-成本控制（上限与无上限）
-错误恢复（回退vs崩溃）
-评估策略(黄金测试vs。“似乎管用”)

诊断报告格式```text
╔══════════════════════════════════════╗
║          WORKFLOW DIAGNOSTIC        ║
╠══════════════════════════════════════╣
║ Prompt Quality      ████░  4/5      ║
║ Context Efficiency   ███░░  3/5      ║
║ Tool Health          ██░░░  2/5      ║
║ Architecture         ████░  4/5      ║
║ Safety & Reliability ██░░░  2/5      ║
╠══════════════════════════════════════╣
║ Overall Score:       15/25           ║
╚══════════════════════════════════════╝

CRITICAL FINDINGS:
1. [Most severe issue — immediate action needed]
2. [Second most severe]
3. [Third]

RECOMMENDED ACTIONS:
1. [Specific remediation for finding #1]
2. [Specific remediation for finding #2]
3. [Specific remediation for finding #3]
```
评分指南

|评分|含义|建议动作||-------|------------------------|-------------------------------------------|
b| 5 b|生产-优秀b|不需要采取行动b|
| 4 |好，有小差距|完善提示清晰或输出模式|
| 3 |功能正常但存在风险|增加错误处理或降低复杂性|
|2 |重大问题|立即关注-添加retries/guards|
| 1 |损坏或丢失|重建与清晰的结构|

# #使用

当你想调用此技能时：

-在工作流投入生产之前发现隐藏的问题
-审核现有代理的质量和可靠性
-制定优先的补救计划，包括具体的后续步骤
—在重大更改后检查工作流的运行状况

提供工作流描述、提示文本、工具列表或代理配置作为上下文。你提供的细节越多，结果就越精确。