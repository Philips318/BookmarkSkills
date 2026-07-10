---
name: incremental-implementation
description: 以增量方式交付变更。用于实现任何触及多个文件的功能或变更。当你即将一次性编写大量代码，或任务感觉太大而无法一步落地时使用。
---

# 增量实现

## 概述

用薄的垂直切片构建：实现一小块、测试它、验证它，然后再扩展。避免一次性实现整个功能。每个增量都应让系统保持在可工作、可测试的状态。这是让大型功能可管理的执行纪律。

## 何时使用

- 实现任何多文件变更
- 从任务拆解中构建新功能
- 重构现有代码
- 任何时候你想在测试前编写超过约 100 行代码

**何时不使用：** 范围已经很小的单文件、单函数变更。

## 增量循环

```
┌──────────────────────────────────────┐
│                                      │
│   Implement ──→ Test ──→ Verify ──┐  │
│       ▲                           │  │
│       └───── Commit ◄─────────────┘  │
│              │                       │
│              ▼                       │
│          Next slice                  │
│                                      │
└──────────────────────────────────────┘
```

对每个切片：

1. **实现** 最小的完整功能片段
2. **测试**：运行测试套件（如果没有测试，就写一个）
3. **验证**：确认这个切片按预期工作（测试通过、构建成功、手动检查）
4. **提交**：用描述性消息保存进度（原子提交指导见 `git-workflow-and-versioning`）
5. **进入下一个切片**：继续推进，不要重新开始

## 切片策略

### 垂直切片（推荐）

构建一条穿过技术栈的完整路径：

```
Slice 1: Create a task (DB + API + basic UI)
    → Tests pass, user can create a task via the UI

Slice 2: List tasks (query + API + UI)
    → Tests pass, user can see their tasks

Slice 3: Edit a task (update + API + UI)
    → Tests pass, user can modify tasks

Slice 4: Delete a task (delete + API + UI + confirmation)
    → Tests pass, full CRUD complete
```

每个切片都交付可工作的端到端功能。

### 契约优先切片

当后端和前端需要并行开发时：

```
Slice 0: Define the API contract (types, interfaces, OpenAPI spec)
Slice 1a: Implement backend against the contract + API tests
Slice 1b: Implement frontend against mock data matching the contract
Slice 2: Integrate and test end-to-end
```

### 风险优先切片

先处理风险最高或最不确定的部分：

```
Slice 1: Prove the WebSocket connection works (highest risk)
Slice 2: Build real-time task updates on the proven connection
Slice 3: Add offline support and reconnection
```

如果 Slice 1 失败，你会在投入 Slice 2 和 3 之前发现。

## 实现规则

### 规则 0：简单优先

写任何代码前，先问：“能工作的最简单东西是什么？”

写完代码后，用这些检查审视它：
- 这能用更少行完成吗？
- 这些抽象是否配得上它们带来的复杂度？
- Staff engineer 看了会不会说“为什么不直接……”？
- 我是在为假设的未来需求构建，还是为当前任务构建？

```
SIMPLICITY CHECK:
✗ Generic EventBus with middleware pipeline for one notification
✓ Simple function call

✗ Abstract factory pattern for two similar components
✓ Two straightforward components with shared utilities

✗ Config-driven form builder for three forms
✓ Three form components
```

三行相似代码优于过早抽象。先实现天真但显然正确的版本。只有在测试证明正确后再优化。

### 规则 0.5：范围纪律

只触碰任务要求的内容。

不要：
- “顺手清理”与变更相邻的代码
- 重构你没有修改的文件中的 imports
- 删除你没有完全理解的注释
- 因为“看起来有用”而添加不在 spec 中的功能
- 在只是阅读的文件中现代化语法

如果你注意到任务范围外值得改进的东西，记录下来，不要修：

```
NOTICED BUT NOT TOUCHING:
- src/utils/format.ts has an unused import (unrelated to this task)
- The auth middleware could use better error messages (separate task)
→ Want me to create tasks for these?
```

### 规则 1：一次只做一件事

每个增量只改变一件逻辑上的事。不要混合关注点：

**坏：** 一个提交同时添加新组件、重构现有组件并更新构建配置。

**好：** 三个单独提交，每个对应一个变更。

### 规则 2：保持可编译

每个增量之后，项目必须能构建，现有测试必须通过。不要让代码库在切片之间处于破损状态。

### 规则 3：对未完成功能使用功能开关

如果功能还没准备好给用户使用，但你需要合并增量：

```typescript
// Feature flag for work-in-progress
const ENABLE_TASK_SHARING = process.env.FEATURE_TASK_SHARING === 'true';

if (ENABLE_TASK_SHARING) {
  // New sharing UI
}
```

这让你可以把小增量合并到主分支，而不暴露未完成工作。

### 规则 4：安全默认值

新代码应默认采取安全、保守行为：

```typescript
// Safe: disabled by default, opt-in
export function createTask(data: TaskInput, options?: { notify?: boolean }) {
  const shouldNotify = options?.notify ?? false;
  // ...
}
```

### 规则 5：易于回滚

每个增量都应能独立回滚：

- 添加式变更（新文件、新函数）容易回滚
- 对现有代码的修改应最小且聚焦
- 数据库迁移应有对应的 rollback migrations
- 避免在同一个提交里删除某物并替换它，请拆开

## 与 Agent 协作

指示 agent 增量实现时：

```
"Let's implement Task 3 from the plan.

Start with just the database schema change and the API endpoint.
Don't touch the UI yet — we'll do that in the next increment.

After implementing, run `npm test` and `npm run build` to verify
nothing is broken."
```

明确说明每个增量的范围，以及哪些不在范围内。

## 增量清单

每个增量后，验证：

- [ ] 变更只做一件事，并且完整地做好
- [ ] 所有现有测试仍然通过（`npm test`）
- [ ] 构建成功（`npm run build`）
- [ ] 类型检查通过（`npx tsc --noEmit`）
- [ ] Linting 通过（`npm run lint`）
- [ ] 新功能按预期工作
- [ ] 变更已用描述性消息提交

**注意：** 在某个变更可能影响验证命令时运行该命令。成功运行后，除非代码又发生变化，否则不要重复运行同一命令；在未变化代码上重跑不会增加信息。

## 常见合理化借口

| 合理化借口 | 现实 |
|---|---|
| “我最后一起测试” | Bug 会叠加。Slice 1 中的 bug 会让 Slice 2-5 都变错。每个切片都测试。 |
| “一次性做完更快” | 直到某处破了，而你找不到 500 行变更里哪一行导致时，它才不快。 |
| “这些变更太小，不值得分开提交” | 小提交是免费的。大提交隐藏 bug，并让回滚痛苦。 |
| “我之后再加功能开关” | 如果功能不完整，就不应该对用户可见。现在就加开关。 |
| “这个重构很小，可以一起带上” | 重构和功能混在一起，会让两者都更难审查和调试。拆开。 |
| “我再跑一次构建命令确认一下” | 成功运行后，除非代码变化，否则重复同一命令没有任何信息。后续编辑后再运行。 |

## 危险信号

- 写了超过 100 行代码还没运行测试
- 单个增量中有多个无关变更
- “让我也顺手加一下这个”的范围扩张
- 为了更快跳过测试/验证步骤
- 构建或测试在增量之间破损
- 大量未提交变更持续积累
- 在第三个用例真正需要前就构建抽象
- “既然来了”就触碰任务范围外的文件
- 为一次性操作创建新的 utility 文件
- 没有任何代码变更就连续运行同一个构建/测试命令两次

## 验证

完成任务的所有增量后：

- [ ] 每个增量都已单独测试并提交
- [ ] 完整测试套件通过
- [ ] 构建干净
- [ ] 功能按 spec 端到端工作
- [ ] 没有未提交变更

## 另请参阅

每个增量的验证是局部检查。在声明任务完成前，使用项目级 Definition of Done 作为最终关口，这是每个增量都要满足的固定标准，不论任务类型。见 `references/definition-of-done.md`。
