---
name: breakdown-plan
description: 'Issue Planning and Automation prompt that generates comprehensive project plans with Epic > Feature > Story/Enabler > Test hierarchy, dependencies, priorities, and automated tracking.'
---
# GitHub问题计划和项目自动化提示

# #目标

担任高级项目经理和DevOps专家，具有敏捷方法和GitHub项目管理方面的专业知识。你的任务是获取完整的特性工件（PRD、UX设计、技术分解、测试计划），并生成一个全面的GitHub项目计划，包括自动问题创建、依赖链接、优先级分配和看板式跟踪。

GitHub项目管理最佳实践

敏捷工作项层次结构- **Epic**：跨越多个功能的大型业务能力（里程碑级别）
- **功能**：在史诗中可交付的面向用户的功能
- **故事**：以用户为中心，独立交付价值的需求
- **Enabler**：支持故事的技术基础设施或架构工作
- **测试**：验证故事和推动者的质量保证工作
- **任务**:stories/enablers的实现级工作分解

项目管理原则

- **投资标准**：独立、可协商、有价值、可评估、小型、可测试
- **就绪定义**：在工作开始前明确验收标准
- **完成的定义**：质量门和完成标准
- **依赖管理**：清除阻塞关系和关键路径识别
- **基于价值的优先排序**：用于决策的业务价值与工作量矩阵

##输入要求在使用此提示之前，请确保您拥有完整的测试工作流工件：

核心功能文档

1. **特性PRD**:`/docs/ways-of-work/plan/{epic-name}/{feature-name}.md`2. **技术故障**:`/docs/ways-of-work/plan/{epic-name}/{feature-name}/technical-breakdown.md`3. **实施方案**:`/docs/ways-of-work/plan/{epic-name}/{feature-name}/implementation-plan.md`相关规划提示

- **测试计划**：使用`plan-test`提示进行全面的测试策略、质量保证计划和测试问题创建
- **架构规划**：使用`plan-epic-arch`提示符进行系统架构和技术设计
- **功能规划**：使用`plan-feature-prd`提示符进行详细的功能需求和规格说明

##输出格式

创建两个主要交付物：

1. **项目计划**:`/docs/ways-of-work/plan/{epic-name}/{feature-name}/project-plan.md`2. **问题创建清单**:`/docs/ways-of-work/plan/{epic-name}/{feature-name}/issues-checklist.md`项目计划结构

# # # # 1。项目概述- **特性概述**：简要描述和业务价值
- **成功标准**：可衡量的结果和关键绩效指标
- **关键里程碑**：主要可交付成果的细分，没有时间表
- **风险评估**：潜在的阻碍因素和缓解策略

# # # # 2。工作项目层次结构```mermaid
graph TD
    A[Epic: {Epic Name}] --> B[Feature: {Feature Name}]
    B --> C[Story 1: {User Story}]
    B --> D[Story 2: {User Story}]
    B --> E[Enabler 1: {Technical Work}]
    B --> F[Enabler 2: {Infrastructure}]

    C --> G[Task: Frontend Implementation]
    C --> H[Task: API Integration]
    C --> I[Test: E2E Scenarios]

    D --> J[Task: Component Development]
    D --> K[Task: State Management]
    D --> L[Test: Unit Tests]

    E --> M[Task: Database Schema]
    E --> N[Task: Migration Scripts]

    F --> O[Task: CI/CD Pipeline]
    F --> P[Task: Monitoring Setup]
```
# # # # 3。GitHub问题分解

#####史诗发行模板```markdown
# Epic: {Epic Name}

## Epic Description

{Epic summary from PRD}

## Business Value

- **Primary Goal**: {Main business objective}
- **Success Metrics**: {KPIs and measurable outcomes}
- **User Impact**: {How users will benefit}

## Epic Acceptance Criteria

- [ ] {High-level requirement 1}
- [ ] {High-level requirement 2}
- [ ] {High-level requirement 3}

## Features in this Epic

- [ ] #{feature-issue-number} - {Feature Name}

## Definition of Done

- [ ] All feature stories completed
- [ ] End-to-end testing passed
- [ ] Performance benchmarks met
- [ ] Documentation updated
- [ ] User acceptance testing completed

## Labels

`epic`, `{priority-level}`, `{value-tier}`

## Milestone

{Release version/date}

## Estimate

{Epic-level t-shirt size: XS, S, M, L, XL, XXL}
```
#####特性发布模板```markdown
# Feature: {Feature Name}

## Feature Description

{Feature summary from PRD}

## User Stories in this Feature

- [ ] #{story-issue-number} - {User Story Title}
- [ ] #{story-issue-number} - {User Story Title}

## Technical Enablers

- [ ] #{enabler-issue-number} - {Enabler Title}
- [ ] #{enabler-issue-number} - {Enabler Title}

## Dependencies

**Blocks**: {List of issues this feature blocks}
**Blocked by**: {List of issues blocking this feature}

## Acceptance Criteria

- [ ] {Feature-level requirement 1}
- [ ] {Feature-level requirement 2}

## Definition of Done

- [ ] All user stories delivered
- [ ] Technical enablers completed
- [ ] Integration testing passed
- [ ] UX review approved
- [ ] Performance testing completed

## Labels

`feature`, `{priority-level}`, `{value-tier}`, `{component-name}`

## Epic

#{epic-issue-number}

## Estimate

{Story points or t-shirt size}
```
#####用户故事发布模板```markdown
# User Story: {Story Title}

## Story Statement

As a **{user type}**, I want **{goal}** so that **{benefit}**.

## Acceptance Criteria

- [ ] {Specific testable requirement 1}
- [ ] {Specific testable requirement 2}
- [ ] {Specific testable requirement 3}

## Technical Tasks

- [ ] #{task-issue-number} - {Implementation task}
- [ ] #{task-issue-number} - {Integration task}

## Testing Requirements

- [ ] #{test-issue-number} - {Test implementation}

## Dependencies

**Blocked by**: {Dependencies that must be completed first}

## Definition of Done

- [ ] Acceptance criteria met
- [ ] Code review approved
- [ ] Unit tests written and passing
- [ ] Integration tests passing
- [ ] UX design implemented
- [ ] Accessibility requirements met

## Labels

`user-story`, `{priority-level}`, `frontend/backend/fullstack`, `{component-name}`

## Feature

#{feature-issue-number}

## Estimate

{Story points: 1, 2, 3, 5, 8}
```
#####技术启用项问题模板```markdown
# Technical Enabler: {Enabler Title}

## Enabler Description

{Technical work required to support user stories}

## Technical Requirements

- [ ] {Technical requirement 1}
- [ ] {Technical requirement 2}

## Implementation Tasks

- [ ] #{task-issue-number} - {Implementation detail}
- [ ] #{task-issue-number} - {Infrastructure setup}

## User Stories Enabled

This enabler supports:

- #{story-issue-number} - {Story title}
- #{story-issue-number} - {Story title}

## Acceptance Criteria

- [ ] {Technical validation 1}
- [ ] {Technical validation 2}
- [ ] Performance benchmarks met

## Definition of Done

- [ ] Implementation completed
- [ ] Unit tests written
- [ ] Integration tests passing
- [ ] Documentation updated
- [ ] Code review approved

## Labels

`enabler`, `{priority-level}`, `infrastructure/api/database`, `{component-name}`

## Feature

#{feature-issue-number}

## Estimate

{Story points or effort estimate}
```
# # # # 4。优先级和价值矩阵

|优先级|值|标准|标签|| -------- | ------ | ------------------------------- | --------------------------------- |
| P0 |高|关键路径，阻断释放|`priority-critical`，`value-high`|
| P1 |高|核心功能，面向用户|`priority-high`，`value-high`|
| P1 | Medium |核心功能，内部|`priority-high`，`value-medium`|
| P2 |中等|重要但不阻塞|`priority-medium`，`value-medium`|
| P3 |低|很好，技术债|`priority-low``value-low`|

# # # # 5。评估指南

#####故事点刻度（斐波那契）

- **1点**：简单更改，<4小时
- **2分**：小功能，<1天
- **3分**：中等特性，1-2天
- **5分**：大功能，3-5天
- **8分**：复杂功能，1-2周
- **13+分**：史诗级的工作，需要分解

##### t恤尺寸（Epics/Features）**XS**: 1-2个故事点
- **S**：总共3-8个故事点
- **M**: 8-20个故事点
**L**：总共20-40个故事点
**XL**：总计40+故事点（考虑分解）

# # # # 6。依赖关系管理```mermaid
graph LR
    A[Epic Planning] --> B[Feature Definition]
    B --> C[Enabler Implementation]
    C --> D[Story Development]
    D --> E[Testing Execution]
    E --> F[Feature Delivery]

    G[Infrastructure Setup] --> C
    H[API Design] --> D
    I[Database Schema] --> C
    J[Authentication] --> D
```
#####依赖类型

- **Blocks**：在此完成之前无法继续的工作
- **相关**：共享上下文但不阻塞的工作
- **先决条件**：需要的基础设施或安装工作
- **并行**：工作可以同时进行

# # # # 7。Sprint计划模板

##### Sprint容量规划

- **团队速度**:{每个sprint的平均故事点}
- **冲刺时间**:{建议进行2周的冲刺}
- **缓冲区分配**:20%用于意外工作和bug修复
- **焦点系数**：计划工作总时间的70-80%

#####冲刺目标定义```markdown
## Sprint {N} Goal

**Primary Objective**: {Main deliverable for this sprint}

**Stories in Sprint**:

- #{issue} - {Story title} ({points} pts)
- #{issue} - {Story title} ({points} pts)

**Total Commitment**: {points} story points
**Success Criteria**: {Measurable outcomes}
```
# # # # 8。GitHub项目板配置

#####柱状结构（看板）

1. **待办事项**：优先排序并准备进行规划
2. **冲刺准备**：详细和估计，准备开发
3. **进行中**：目前正在制作中
4. **审查中**：代码审查、测试或涉众审查
5. **测试**:QA验证和验收测试
6. **完成**：完成并接受

#####自定义字段配置

—**优先级**:P0、P1、P2、P3
—**取值**：高、中、低
-组件：前端，后端，基础设施，测试
- **估计**：故事点或t恤大小
- **Sprint**：当前的Sprint分配
- **受让人**：负责的团队成员
—**Epic**：父史诗引用

# # # # 9。自动化和GitHub Actions#####自动问题创建```yaml
name: Create Feature Issues

on:
  workflow_dispatch:
    inputs:
      feature_name:
        description: 'Feature name'
        required: true
      epic_issue:
        description: 'Epic issue number'
        required: true

jobs:
  create-issues:
    runs-on: ubuntu-latest
    steps:
      - name: Create Feature Issue
        uses: actions/github-script@v7
        with:
          script: |
            const { data: epic } = await github.rest.issues.get({
              owner: context.repo.owner,
              repo: context.repo.repo,
              issue_number: ${{ github.event.inputs.epic_issue }}
            });

            const featureIssue = await github.rest.issues.create({
              owner: context.repo.owner,
              repo: context.repo.repo,
              title: `Feature: ${{ github.event.inputs.feature_name }}`,
              body: `# Feature: ${{ github.event.inputs.feature_name }}\n\n...`,
              labels: ['feature', 'priority-medium'],
              milestone: epic.data.milestone?.number
            });
```
#####自动状态更新```yaml
name: Update Issue Status

on:
  pull_request:
    types: [opened, closed]

jobs:
  update-status:
    runs-on: ubuntu-latest
    steps:
      - name: Move to In Review
        if: github.event.action == 'opened'
        uses: actions/github-script@v7
        # Move related issues to "In Review" column

      - name: Move to Done
        if: github.event.action == 'closed' && github.event.pull_request.merged
        uses: actions/github-script@v7
        # Move related issues to "Done" column
```
###问题创建清单

####创建前准备

-[] **功能工件完成**:PRD， UX设计，技术分解，测试计划
-[] **史诗存在**：父史诗问题已创建，并有适当的标签和里程碑
-[] **项目板配置**：列，自定义字段和自动化规则设置
-[] **团队能力评估**：完成Sprint计划和资源分配

####史诗级别问题

-[] **史诗级的issue创建**，有全面的描述和验收标准
-[] **史诗里程碑创建**与目标发布日期
-[] **应用的史诗标签**:`epic`，优先级，值和团队标签
[] **史诗被添加到项目板**的相应栏

####功能级别问题-[] **功能问题导致**链接到父史诗
-[] **识别并记录功能依赖关系
-[] **使用t恤尺寸完成特征估计
-[]定义了可测量结果的特性验收标准

####Story/Enabler级别`/docs/ways-of-work/plan/{epic-name}/{feature-name}/issues-checklist.md`中记录的问题

-[] **根据INVEST标准创建的用户故事
-[] **确定技术支持因素并确定优先级
-[] **故事点估计分配**使用斐波那契规模
-[]故事和enablers之间的依赖关系映射
-[] **详细的验收标准**和可测试要求

##成功指标

项目管理关键绩效指标- **Sprint可预见性**:>每个Sprint完成80%的承诺工作
- **周期时间**：从“进行中”到“完成”的平均时间<5个工作日
- **交货期**：从“待定”到“完成”的平均时间<2周
- **缺陷逃避率**:<5%的故事需要发布后修复
- **团队速度**：在sprint中一致地交付故事点

流程效率指标

- **问题创建时间**:<1小时创建完整的功能分解
—**依赖项解析**:<24小时解析阻塞依赖项
- **状态更新精度**:>95%自动状态转换工作正常
- **文档完整性**:100%的问题都需要模板字段
- **跨团队协作**：外部依赖解决少于2个工作日

项目交付度量- **完成合规性定义**:100%完成的故事符合国防部标准
- **验收标准覆盖率**:100%的验收标准已验证
- **Sprint目标完成情况**:>90%的Sprint目标成功交付
- **利益相关者满意度**:>90%的利益相关者对完成的功能的认可
- **计划准确性**：预计交货时间与实际交货时间差异<10%

这种全面的GitHub项目管理方法确保了从史诗级计划到单个实施任务的完整可追溯性，并对所有团队成员进行了自动跟踪和明确的问责制。