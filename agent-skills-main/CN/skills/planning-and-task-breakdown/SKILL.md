---
name: planning-and-task-breakdown
description: 将工作拆解成有序任务。当你已有 spec 或明确需求，并需要拆解成可实现任务时使用。当任务感觉太大而无法开始、需要估算范围，或可以并行工作时使用。
---

# 计划与任务拆解

## 概述

把工作分解成小而可验证的任务，并带有明确验收标准。好的任务拆解，是 agent 可靠完成工作和产出一团乱麻之间的区别。每个任务都应足够小，可以在一次聚焦会话中实现、测试和验证。

## 何时使用

- 你有 spec，需要拆解成可实现单元
- 任务感觉太大或太模糊，无法开始
- 工作需要在多个 agent 或会话间并行化
- 需要向人类沟通范围
- 实现顺序不明显

**何时不使用：** 范围明显的单文件变更，或 spec 已经包含定义良好的任务。

## 计划流程

### 步骤 1：进入计划模式

写任何代码前，以只读模式工作：

- 阅读 spec 和相关代码库部分
- 识别现有模式和约定
- 映射组件之间的依赖
- 记录风险和未知项

**计划期间不要写代码。** 输出是一份保存到 `tasks/plan.md` 的计划文档，以及一份保存到 `tasks/todo.md` 的任务清单，而不是实现。

### 步骤 2：识别依赖图

映射什么依赖什么：

```
Database schema
    │
    ├── API models/types
    │       │
    │       ├── API endpoints
    │       │       │
    │       │       └── Frontend API client
    │       │               │
    │       │               └── UI components
    │       │
    │       └── Validation logic
    │
    └── Seed data / migrations
```

实现顺序沿依赖图自底向上：先构建基础。

### 步骤 3：垂直切片

不要先构建所有数据库，再构建所有 API，再构建所有 UI。一次构建一条完整功能路径：

**坏（水平切片）：**
```
Task 1: Build entire database schema
Task 2: Build all API endpoints
Task 3: Build all UI components
Task 4: Connect everything
```

**好（垂直切片）：**
```
Task 1: User can create an account (schema + API + UI for registration)
Task 2: User can log in (auth schema + API + UI for login)
Task 3: User can create a task (task schema + API + UI for creation)
Task 4: User can view task list (query + API + UI for list view)
```

每个垂直切片都交付可工作、可测试的功能。

### 步骤 4：编写任务

每个任务遵循这个结构：

```markdown
## Task [N]: [Short descriptive title]

**Description:** One paragraph explaining what this task accomplishes.

**Acceptance criteria:**
- [ ] [Specific, testable condition]
- [ ] [Specific, testable condition]

**Verification:**
- [ ] Tests pass: `npm test -- --grep "feature-name"`
- [ ] Build succeeds: `npm run build`
- [ ] Manual check: [description of what to verify]

**Dependencies:** [Task numbers this depends on, or "None"]

**Files likely touched:**
- `src/path/to/file.ts`
- `tests/path/to/test.ts`

**Estimated scope:** [Small: 1-2 files | Medium: 3-5 files | Large: 5+ files]
```

### 步骤 5：排序和检查点

安排任务时确保：

1. 依赖已满足（先构建基础）
2. 每个任务都让系统保持可工作状态
3. 每 2-3 个任务后有验证检查点
4. 高风险任务靠前（快速失败）

添加明确检查点：

```markdown
## Checkpoint: After Tasks 1-3
- [ ] All tests pass
- [ ] Application builds without errors
- [ ] Core user flow works end-to-end
- [ ] Review with human before proceeding
```

## 任务大小指南

| 大小 | 文件数 | 范围 | 示例 |
|------|-------|-------|---------|
| **XS** | 1 | 单函数或配置变更 | 添加一个验证规则 |
| **S** | 1-2 | 一个组件或 endpoint | 添加一个新的 API endpoint |
| **M** | 3-5 | 一个功能切片 | 用户注册流程 |
| **L** | 5-8 | 多组件功能 | 带 filtering 和 pagination 的搜索 |
| **XL** | 8+ | **太大，需要进一步拆解** | — |

如果任务是 L 或更大，应拆成更小任务。Agent 在 S 和 M 任务上表现最好。

**何时进一步拆解任务：**
- 需要超过一次聚焦会话（大约 2+ 小时 agent 工作）
- 你无法用 3 个或更少 bullet 描述验收标准
- 它触及两个或更多独立子系统（例如 auth 和 billing）
- 你发现任务标题里在写 “and”（这是两个任务的信号）

## 输出文件

- **计划文档：** 将实现计划保存到 `tasks/plan.md`。
- **任务清单：** 将 checklist 风格任务列表保存到 `tasks/todo.md`。

如果 `tasks/` 目录不存在，请创建它。这些路径是 `/build` 命令和其他下游工具期望的约定。

## 计划文档模板

```markdown
# Implementation Plan: [Feature/Project Name]

## Overview
[One paragraph summary of what we're building]

## Architecture Decisions
- [Key decision 1 and rationale]
- [Key decision 2 and rationale]

## Task List

### Phase 1: Foundation
- [ ] Task 1: ...
- [ ] Task 2: ...

### Checkpoint: Foundation
- [ ] Tests pass, builds clean

### Phase 2: Core Features
- [ ] Task 3: ...
- [ ] Task 4: ...

### Checkpoint: Core Features
- [ ] End-to-end flow works

### Phase 3: Polish
- [ ] Task 5: ...
- [ ] Task 6: ...

### Checkpoint: Complete
- [ ] All acceptance criteria met
- [ ] Ready for review

## Risks and Mitigations
| Risk | Impact | Mitigation |
|------|--------|------------|
| [Risk] | [High/Med/Low] | [Strategy] |

## Open Questions
- [Question needing human input]
```

## 并行化机会

当有多个 agent 或会话可用时：

- **可以安全并行：** 独立功能切片、已实现功能的测试、文档
- **必须顺序执行：** 数据库迁移、共享状态变更、依赖链
- **需要协调：** 共享 API contract 的功能（先定义 contract，再并行）

## 常见合理化借口

| 合理化借口 | 现实 |
|---|---|
| “我边做边想” | 这就是你得到一团乱麻和返工的方式。10 分钟计划能节省数小时。 |
| “任务很明显” | 还是写下来。显式任务会暴露隐藏依赖和遗漏的边界情况。 |
| “计划是开销” | 计划就是任务。没有计划的实现只是打字。 |
| “我能全记在脑子里” | Context window 是有限的。书面计划能跨会话和 compaction 存活。 |

## 危险信号

- 没有书面任务清单就开始实现
- 任务写成“implement the feature”，没有验收标准
- 计划中没有验证步骤
- 所有任务都是 XL 大小
- 任务之间没有检查点
- 没有考虑依赖顺序

## 验证

开始实现前，确认：

- [ ] 每个任务都有验收标准
- [ ] 每个任务都有验证步骤
- [ ] 任务依赖已识别并按正确顺序排列
- [ ] 没有任务触碰超过约 5 个文件
- [ ] 主要阶段之间有检查点
- [ ] 人类已审查并批准计划

## 另请参阅

验收标准是任务级的，回答“我们是否构建了正确的东西？”。它们位于项目级 Definition of Done 之上，后者是每个任务计为完成前必须满足的固定标准。见 `references/definition-of-done.md`。
