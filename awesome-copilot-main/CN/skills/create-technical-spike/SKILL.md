---
name: create-technical-spike
description: 'Create time-boxed technical spike documents for researching and resolving critical development decisions before implementation.'
---
#创建技术峰值文档

创建有时间限制的技术峰值文档，用于研究在开发开始之前必须回答的关键问题。每个尖峰都集中在一个具体的技术决策上，有明确的可交付成果和时间表。

文档结构

在`${input:FolderPath|docs/spikes}`目录中创建单独的文件。使用模式为每个文件命名：`[category]-[short-description]-spike.md`（例如，`api-copilot-integration-spike.md`,`performance-realtime-audio-spike.md`）。```md
---
title: "${input:SpikeTitle}"
category: "${input:Category|Technical}"
status: "🔴 Not Started"
priority: "${input:Priority|High}"
timebox: "${input:Timebox|1 week}"
created: [YYYY-MM-DD]
updated: [YYYY-MM-DD]
owner: "${input:Owner}"
tags: ["technical-spike", "${input:Category|technical}", "research"]
---

# ${input:SpikeTitle}

## Summary

**Spike Objective:** [Clear, specific question or decision that needs resolution]

**Why This Matters:** [Impact on development/architecture decisions]

**Timebox:** [How much time allocated to this spike]

**Decision Deadline:** [When this must be resolved to avoid blocking development]

## Research Question(s)

**Primary Question:** [Main technical question that needs answering]

**Secondary Questions:**

- [Related question 1]
- [Related question 2]
- [Related question 3]

## Investigation Plan

### Research Tasks

- [ ] [Specific research task 1]
- [ ] [Specific research task 2]
- [ ] [Specific research task 3]
- [ ] [Create proof of concept/prototype]
- [ ] [Document findings and recommendations]

### Success Criteria

**This spike is complete when:**

- [ ] [Specific criteria 1]
- [ ] [Specific criteria 2]
- [ ] [Clear recommendation documented]
- [ ] [Proof of concept completed (if applicable)]

## Technical Context

**Related Components:** [List system components affected by this decision]

**Dependencies:** [What other spikes or decisions depend on resolving this]

**Constraints:** [Known limitations or requirements that affect the solution]

## Research Findings

### Investigation Results

[Document research findings, test results, and evidence gathered]

### Prototype/Testing Notes

[Results from any prototypes, spikes, or technical experiments]

### External Resources

- [Link to relevant documentation]
- [Link to API references]
- [Link to community discussions]
- [Link to examples/tutorials]

## Decision

### Recommendation

[Clear recommendation based on research findings]

### Rationale

[Why this approach was chosen over alternatives]

### Implementation Notes

[Key considerations for implementation]

### Follow-up Actions

- [ ] [Action item 1]
- [ ] [Action item 2]
- [ ] [Update architecture documents]
- [ ] [Create implementation tasks]

## Status History

| Date   | Status         | Notes                      |
| ------ | -------------- | -------------------------- |
| [Date] | 🔴 Not Started | Spike created and scoped   |
| [Date] | 🟡 In Progress | Research commenced         |
| [Date] | 🟢 Complete    | [Resolution summary]       |

---

_Last updated: [Date] by [Name]_
```
##技术峰值分类

API集成

-第三方API的功能和限制
-集成模式和身份验证
—速率限制和性能特征

###建筑与设计

-系统架构决策
-设计模式适用性
-组件交互模型

性能和可扩展性

—性能要求和约束
-可扩展性瓶颈和解决方案
-资源利用模式

###平台和基础设施

-平台能力和限制
-基础设施要求
-部署和托管方面的考虑

安全性和合规性

—安全需求和实现
-遵从性约束
—认证授权方式

###用户体验

-用户交互模式
-无障碍要求
-界面设计决策

文件命名约定使用描述性的、烤肉串式的名字来表明类别和特定的未知：

* *API/Integration例子:* *- `api-copilot-chat-integration-spike.md`
- `api-azure-speech-realtime-spike.md`
- `api-vscode-extension-capabilities-spike.md`
* *性能示例:* *- `performance-audio-processing-latency-spike.md`
- `performance-extension-host-limitations-spike.md`
- `performance-webrtc-reliability-spike.md`
* *建筑例子:* *- `architecture-voice-pipeline-design-spike.md`
- `architecture-state-management-spike.md`
- `architecture-error-handling-strategy-spike.md`
AI代理的最佳实践

1. **每个问题：**每个文档集中在一个技术决策或研究问题

2. **限时研究：**定义每个峰值的具体时间限制和可交付成果

3. **基于证据的决策：**在标记完成之前需要具体的证据（测试、原型、文档）

4. **明确建议：**记录具体建议和实施理由

5. **依赖跟踪：**确定峰值如何相互关联并影响项目决策

6. **以结果为中心：**每个峰值必须产生一个可操作的决定或建议

##研究策略

阶段1：信息收集

1. **使用search/fetch工具搜索现有文档**
2. **分析代码库**现有的模式和约束
3. **研究外部资源** （api，库，示例）阶段2：验证和测试

1. **创建集中的原型**来测试特定的假设
2. **运行有针对性的实验**来验证假设
3. **用证据证明测试结果**

阶段3：决策和文档

1. **综合发现**为明确的建议
2. **开发团队文档实施指导**
3. **创建后续任务**以供实施

##工具使用

- **search/searchResults:**研究现有的解决方案和文档
—**fetch/githubRepo:**分析外部api、库和示例
-代码库：**了解现有的系统约束和模式
- **runTasks:**执行原型和验证测试
- **editFiles:**更新研究进展和发现
- **vscodeAPI:**测试VS Code扩展的能力和限制专注于有时间限制的研究，解决关键的技术决策，消除开发进程的障碍。