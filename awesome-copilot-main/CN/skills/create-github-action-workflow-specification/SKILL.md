---
name: create-github-action-workflow-specification
description: 'Create a formal specification for an existing GitHub Actions CI/CD workflow, optimized for AI consumption and workflow maintenance.'
---
#创建GitHub Actions工作流规范

为GitHub Actions工作流创建一个全面的规范：`${input:WorkflowFile}`。

该规范作为工作流行为、需求和约束的规范。它必须与实现无关，关注工作流完成了什么，而不是如何实现。

## ai优化的需求

- **令牌效率**：使用简洁的语言而不牺牲清晰度
- **结构化数据**：利用表格，列表和图表密集的信息
- **语义清晰：始终如一地使用精确的术语
—**实现抽象**：避免特定的语法、命令或工具版本
- **可维护性**：随着工作流程的发展，设计易于更新

##规格模板

另存为：`/spec/spec-process-cicd-[workflow-name].md````md
---
title: CI/CD Workflow Specification - [Workflow Name]
version: 1.0
date_created: [YYYY-MM-DD]
last_updated: [YYYY-MM-DD]
owner: DevOps Team
tags: [process, cicd, github-actions, automation, [domain-specific-tags]]
---

## Workflow Overview

**Purpose**: [One sentence describing workflow's primary goal]
**Trigger Events**: [List trigger conditions]
**Target Environments**: [Environment scope]

## Execution Flow Diagram

```mermaid
图道明    A[Trigger Event] --> B[Job 1]
    B --> C[Job 2]
    C --> D[Job 3]
    D --> E[End]
    
    B --> F[Parallel Job]
    F --> D
    
    style A fill:#e1f5fe
    style E fill:#e8f5e8
```

## Jobs & Dependencies

| Job Name | Purpose | Dependencies | Execution Context |
|----------|---------|--------------|-------------------|
| job-1 | [Purpose] | [Prerequisites] | [Runner/Environment] |
| job-2 | [Purpose] | job-1 | [Runner/Environment] |

## Requirements Matrix

### Functional Requirements
| ID | Requirement | Priority | Acceptance Criteria |
|----|-------------|----------|-------------------|
| REQ-001 | [Requirement] | High | [Testable criteria] |
| REQ-002 | [Requirement] | Medium | [Testable criteria] |

### Security Requirements
| ID | Requirement | Implementation Constraint |
|----|-------------|---------------------------|
| SEC-001 | [Security requirement] | [Constraint description] |

### Performance Requirements
| ID | Metric | Target | Measurement Method |
|----|-------|--------|-------------------|
| PERF-001 | [Metric] | [Target value] | [How measured] |

## Input/Output Contracts

### Inputs

```yaml
#环境变量
ENV_VAR_1: string #目的：[description]
ENV_VAR_2: secret #目的：[description]

#存储库触发器
路径：[路径过滤器列表]
分支：[分支模式列表]```

### Outputs

```yaml
#作业输出
job_1_output: string #描述：[目的]
build_artifact：文件#描述：[内容类型]```

### Secrets & Variables

| Type | Name | Purpose | Scope |
|------|------|---------|-------|
| Secret | SECRET_1 | [Purpose] | Workflow |
| Variable | VAR_1 | [Purpose] | Repository |

## Execution Constraints

### Runtime Constraints

- **Timeout**: [Maximum execution time]
- **Concurrency**: [Parallel execution limits]
- **Resource Limits**: [Memory/CPU constraints]

### Environmental Constraints

- **Runner Requirements**: [OS/hardware needs]
- **Network Access**: [External connectivity needs]
- **Permissions**: [Required access levels]

## Error Handling Strategy

| Error Type | Response | Recovery Action |
|------------|----------|-----------------|
| Build Failure | [Response] | [Recovery steps] |
| Test Failure | [Response] | [Recovery steps] |
| Deployment Failure | [Response] | [Recovery steps] |

## Quality Gates

### Gate Definitions

| Gate | Criteria | Bypass Conditions |
|------|----------|-------------------|
| Code Quality | [Standards] | [When allowed] |
| Security Scan | [Thresholds] | [When allowed] |
| Test Coverage | [Percentage] | [When allowed] |

## Monitoring & Observability

### Key Metrics

- **Success Rate**: [Target percentage]
- **Execution Time**: [Target duration]
- **Resource Usage**: [Monitoring approach]

### Alerting

| Condition | Severity | Notification Target |
|-----------|----------|-------------------|
| [Condition] | [Level] | [Who/Where] |

## Integration Points

### External Systems

| System | Integration Type | Data Exchange | SLA Requirements |
|--------|------------------|---------------|------------------|
| [System] | [Type] | [Data format] | [Requirements] |

### Dependent Workflows

| Workflow | Relationship | Trigger Mechanism |
|----------|--------------|-------------------|
| [Workflow] | [Type] | [How triggered] |

## Compliance & Governance

### Audit Requirements

- **Execution Logs**: [Retention policy]
- **Approval Gates**: [Required approvals]
- **Change Control**: [Update process]

### Security Controls

- **Access Control**: [Permission model]
- **Secret Management**: [Rotation policy]
- **Vulnerability Scanning**: [Scan frequency]

## Edge Cases & Exceptions

### Scenario Matrix

| Scenario | Expected Behavior | Validation Method |
|----------|-------------------|-------------------|
| [Edge case] | [Behavior] | [How to verify] |

## Validation Criteria

### Workflow Validation

- **VLD-001**: [Validation rule]
- **VLD-002**: [Validation rule]

### Performance Benchmarks

- **PERF-001**: [Benchmark criteria]
- **PERF-002**: [Benchmark criteria]

## Change Management

### Update Process

1. **Specification Update**: Modify this document first
2. **Review & Approval**: [Approval process]
3. **Implementation**: Apply changes to workflow
4. **Testing**: [Validation approach]
5. **Deployment**: [Release process]

### Version History

| Version | Date | Changes | Author |
|---------|------|---------|--------|
| 1.0 | [Date] | Initial specification | [Author] |

## Related Specifications

- [Link to related workflow specs]
- [Link to infrastructure specs]
- [Link to deployment specs]

```
##分析说明

在分析工作流文件时：

1. **提取核心目的**：确定主要业务目标
2. **映射作业流程**：创建显示执行顺序的依赖关系图
3. **识别合同**：记录输入、输出和接口
4. **Capture Constraints**：提取超时、权限和限制
5. **定义质量门**：确定确认和批准点
6. **文档错误路径**：映射故障场景和恢复
7. **抽象实现**：关注行为，而不是语法

美人鱼图指南

流类型
—**顺序**:`A --> B --> C`- **并行**:`A --> B & A --> C; B --> D & C --> D`—**条件必选**:`A --> B{Decision}; B -->|Yes| C; B -->|No| D`# # #样式```mermaid
style TriggerNode fill:#e1f5fe
style SuccessNode fill:#e8f5e8
style FailureNode fill:#ffebee
style ProcessNode fill:#f3e5f5
```
复杂的工作流程
对于有5个以上作业的工作流，使用子图：```mermaid
graph TD
    subgraph "Build Phase"
        A[Lint] --> B[Test] --> C[Build]
    end
    subgraph "Deploy Phase"  
        D[Staging] --> E[Production]
    end
    C --> D
```
令牌优化策略

1. **使用表格**：结构化格式的密集信息
2. **缩写一致**：定义一次，贯穿始终
3. **要点：避免散文段落
4. **代码块：结构化数据高于叙事
5. **交叉引用**：链接而不是重复信息

专注于创建一个规范，它既可以作为文档，也可以作为工作流更新的模板。