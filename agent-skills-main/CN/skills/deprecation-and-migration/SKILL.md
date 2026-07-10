---
name: deprecation-and-migration
description: 管理 deprecation 和 migration。使用场景：移除旧系统、APIs 或功能。使用场景：将用户从一个实现迁移到另一个实现。使用场景：决定是否维护或 sunset 现有代码。
---

# 弃用与迁移

## 概述

代码是负债，不是资产。每一行代码都有持续维护成本，包括要修的 bug、要更新的依赖、要应用的安全补丁，以及新工程师的 onboarding 成本。Deprecation 是移除不再值得保留的代码的纪律，migration 是把用户安全地从旧系统迁到新系统的过程。

大多数工程组织擅长构建东西。很少有组织擅长移除东西。这个 skill 解决的就是这个缺口。

## 使用场景

- 用新系统、API 或 library 替换旧系统
- Sunset 不再需要的功能
- 合并重复实现
- 移除没人拥有但所有人都依赖的 dead code
- 规划新系统的生命周期（deprecation planning 从设计时开始）
- 决定是维护 legacy system 还是投入 migration

## 核心原则

### Code Is a Liability

每一行代码都有持续成本：它需要 tests、documentation、security patches、dependency updates，以及任何在附近工作的人都要承担的 mental overhead。代码的价值在于它提供的功能，而不是代码本身。当同样功能可以用更少代码、更低复杂度或更好抽象提供时，旧代码就应该离开。

### Hyrum's Law Makes Removal Hard

用户足够多时，每个可观察行为都会被依赖，包括 bugs、timing quirks 和未记录 side effects。这就是为什么 deprecation 需要主动 migration，而不仅仅是 announcement。用户不能“直接切换”，因为他们可能依赖 replacement 没有复刻的行为。

### Deprecation Planning Starts at Design Time

构建新东西时，问：“三年后我们如何移除它？” 带有清晰 interfaces、feature flags 和最小 surface area 的系统，比到处泄漏实现细节的系统更容易弃用。

## Deprecation Decision

弃用任何东西之前，回答这些问题：

```
1. Does this system still provide unique value?
   → If yes, maintain it. If no, proceed.

2. How many users/consumers depend on it?
   → Quantify the migration scope.

3. Does a replacement exist?
   → If no, build the replacement first. Don't deprecate without an alternative.

4. What's the migration cost for each consumer?
   → If trivially automated, do it. If manual and high-effort, weigh against maintenance cost.

5. What's the ongoing maintenance cost of NOT deprecating?
   → Security risk, engineer time, opportunity cost of complexity.
```

## Compulsory vs Advisory Deprecation

| Type | When to Use | Mechanism |
|------|-------------|-----------|
| **Advisory** | Migration 是可选的，旧系统稳定 | Warnings、documentation、nudges。用户按自己的时间线迁移。 |
| **Compulsory** | 旧系统有安全问题、阻塞进展，或维护成本不可持续 | 硬期限。旧系统将在 X 日期移除。提供 migration tooling。 |

**默认采用 advisory。** 只有当维护成本或风险足以证明强制迁移合理时，才使用 compulsory。Compulsory deprecation 要求提供 migration tooling、documentation 和 support，不能只是宣布一个 deadline。

## Migration Process

### Step 1: Build the Replacement

不要在没有可用替代方案时 deprecate。Replacement 必须：

- 覆盖旧系统的所有 critical use cases
- 有 documentation 和 migration guides
- 已在 production 中证明可行（不只是“理论上更好”）

### Step 2: Announce and Document

```markdown
## Deprecation Notice: OldService

**Status:** Deprecated as of 2025-03-01
**Replacement:** NewService (see migration guide below)
**Removal date:** Advisory — no hard deadline yet
**Reason:** OldService requires manual scaling and lacks observability.
            NewService handles both automatically.

### Migration Guide
1. Replace `import { client } from 'old-service'` with `import { client } from 'new-service'`
2. Update configuration (see examples below)
3. Run the migration verification script: `npx migrate-check`
```

### Step 3: Migrate Incrementally

一次迁移一个 consumer，而不是一次性迁移全部。对每个 consumer：

```
1. Identify all touchpoints with the deprecated system
2. Update to use the replacement
3. Verify behavior matches (tests, integration checks)
4. Remove references to the old system
5. Confirm no regressions
```

**The Churn Rule:** 如果你拥有正在弃用的基础设施，你就负责迁移你的用户，或提供无需 migration 的 backward-compatible updates。不要宣布 deprecation 后让用户自己摸索。

### Step 4: Remove the Old System

只有在所有 consumers 已迁移后：

```
1. Verify zero active usage (metrics, logs, dependency analysis)
2. Remove the code
3. Remove associated tests, documentation, and configuration
4. Remove the deprecation notices
5. Celebrate — removing code is an achievement
```

## Migration Patterns

### Strangler Pattern

新旧系统并行运行。逐步将流量从旧系统路由到新系统。当旧系统处理 0% 流量时，移除它。

```
Phase 1: New system handles 0%, old handles 100%
Phase 2: New system handles 10% (canary)
Phase 3: New system handles 50%
Phase 4: New system handles 100%, old system idle
Phase 5: Remove old system
```

### Adapter Pattern

创建一个 adapter，将旧 interface 的调用翻译到新 implementation。消费者继续使用旧 interface，同时你迁移 backend。

```typescript
// Adapter: old interface, new implementation
class LegacyTaskService implements OldTaskAPI {
  constructor(private newService: NewTaskService) {}

  // Old method signature, delegates to new implementation
  getTask(id: number): OldTask {
    const task = this.newService.findById(String(id));
    return this.toOldFormat(task);
  }
}
```

### Feature Flag Migration

使用 feature flags 一次把一个 consumer 从旧系统切到新系统：

```typescript
function getTaskService(userId: string): TaskService {
  if (featureFlags.isEnabled('new-task-service', { userId })) {
    return new NewTaskService();
  }
  return new LegacyTaskService();
}
```

## Zombie Code

Zombie code 是没人拥有但所有人都依赖的代码。它不再被主动维护，没有明确 owner，并积累安全漏洞和兼容性问题。迹象：

- 6+ 个月没有 commits，但仍有 active consumers
- 没有 assigned maintainer 或 team
- Failing tests 无人修复
- 依赖存在已知漏洞但无人更新
- Documentation 引用已经不存在的系统

**应对：** 要么分配 owner 并妥善维护，要么用具体 migration plan 弃用它。Zombie code 不能停留在 limbo 中，它要么获得投入，要么被移除。

## Common Rationalizations

| Rationalization | Reality |
|---|---|
| “它还能工作，为什么移除？” | 没人维护的工作代码会积累安全债和复杂度。维护成本会静默增长。 |
| “以后可能有人需要” | 如果以后需要，可以重建。保留未使用代码“以防万一”的成本高于重建。 |
| “迁移太贵了” | 将 migration cost 与未来 2-3 年的 ongoing maintenance cost 比较。长期看，migration 通常更便宜。 |
| “等新系统完成后再弃用” | Deprecation planning 从设计时开始。等新系统完成时，你会有新优先事项。现在就计划。 |
| “用户会自己迁移” | 他们不会。提供 tooling、documentation 和 incentives，或自己做迁移（The Churn Rule）。 |
| “我们可以无限期维护两个系统” | 两个做同一件事的系统意味着双倍维护、测试、文档和 onboarding 成本。 |

## Red Flags

- Deprecated systems 没有可用 replacement
- Deprecation announcements 没有 migration tooling 或 documentation
- “Soft” deprecation 多年 advisory 却没有进展
- Zombie code 没有 owner 但有 active consumers
- 新功能添加到 deprecated system（应投入 replacement）
- Deprecation 前没有测量当前 usage
- 未验证 zero active consumers 就移除代码

## Verification

完成 deprecation 后：

- [ ] Replacement 已在 production 中证明可行，并覆盖所有 critical use cases
- [ ] Migration guide 存在，且包含具体步骤和示例
- [ ] 所有 active consumers 已迁移（由 metrics/logs 验证）
- [ ] 旧代码、tests、documentation 和 configuration 已完全移除
- [ ] 代码库中不再有对 deprecated system 的 references
- [ ] Deprecation notices 已移除（它们已完成使命）