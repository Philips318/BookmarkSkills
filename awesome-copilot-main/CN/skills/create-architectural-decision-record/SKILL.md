---
name: create-architectural-decision-record
description: 'Create an Architectural Decision Record (ADR) document for AI-optimized decision documentation.'
---
创建架构决策记录

为`${input:DecisionTitle}`创建ADR文档，使用针对AI消费和人类可读性优化的结构化格式。

# #输入

- **上下文**:`${input:Context}`- **决策**:`${input:Decision}`- **备选方案**:`${input:Alternatives}`- **利益相关者**:`${input:Stakeholders}`##输入验证
如果没有提供任何所需的输入，或者不能从会话历史中确定，请在继续生成ADR之前要求用户提供缺失的信息。

# #要求

-使用精确、明确的语言
-按照标准ADR格式处理前台事宜
-包括积极和消极的结果
-记录拒绝理由的备选方案
-供机器解析和人类参考的结构
-在多项目部分使用编码的项目符号（3-4个字母代码+ 3位数字）ADR必须保存在`/docs/adr/`目录中，使用命名约定：`adr-NNNN-[title-slug].md`，其中NNNN是下一个连续的4位数（例如，`adr-0001-database-selection.md`）。

所需的文档结构

文档文件必须遵循下面的模板，确保所有部分都正确填写。降价的正面内容应该按照下面的例子正确地组织：```md
---
title: "ADR-NNNN: [Decision Title]"
status: "Proposed"
date: "YYYY-MM-DD"
authors: "[Stakeholder Names/Roles]"
tags: ["architecture", "decision"]
supersedes: ""
superseded_by: ""
---

# ADR-NNNN: [Decision Title]

## Status

**Proposed** | Accepted | Rejected | Superseded | Deprecated

## Context

[Problem statement, technical constraints, business requirements, and environmental factors requiring this decision.]

## Decision

[Chosen solution with clear rationale for selection.]

## Consequences

### Positive

- **POS-001**: [Beneficial outcomes and advantages]
- **POS-002**: [Performance, maintainability, scalability improvements]
- **POS-003**: [Alignment with architectural principles]

### Negative

- **NEG-001**: [Trade-offs, limitations, drawbacks]
- **NEG-002**: [Technical debt or complexity introduced]
- **NEG-003**: [Risks and future challenges]

## Alternatives Considered

### [Alternative 1 Name]

- **ALT-001**: **Description**: [Brief technical description]
- **ALT-002**: **Rejection Reason**: [Why this option was not selected]

### [Alternative 2 Name]

- **ALT-003**: **Description**: [Brief technical description]
- **ALT-004**: **Rejection Reason**: [Why this option was not selected]

## Implementation Notes

- **IMP-001**: [Key implementation considerations]
- **IMP-002**: [Migration or rollout strategy if applicable]
- **IMP-003**: [Monitoring and success criteria]

## References

- **REF-001**: [Related ADRs]
- **REF-002**: [External documentation]
- **REF-003**: [Standards or frameworks referenced]
```
