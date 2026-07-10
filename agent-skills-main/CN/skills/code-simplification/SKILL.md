---
name: code-simplification
description: 为清晰度简化代码。使用场景：在不改变行为的前提下重构代码以提升清晰度。使用场景：代码能工作，但比应有状态更难阅读、维护或扩展。使用场景：审查积累了不必要复杂度的代码。
---

# 代码简化

> 灵感来自 [Claude Code Simplifier plugin](https://github.com/anthropics/claude-plugins-official/blob/main/plugins/code-simplifier/agents/code-simplifier.md)。这里改编为一个与模型无关、由流程驱动的 skill，适用于任何 AI coding agent。

## 概述

通过降低复杂度来简化代码，同时精确保留行为。目标不是更少的行数，而是更易读、易理解、易修改和易调试的代码。每次简化都必须通过一个简单测试：“新团队成员理解这个版本是否会比原版更快？”

## 使用场景

- 功能已经工作且 tests pass，但实现感觉比需要的更沉重
- 代码审查中发现可读性或复杂度问题
- 遇到深层嵌套逻辑、长函数或不清晰命名
- 重构在时间压力下编写的代码
- 合并散落在多个文件中的相关逻辑
- 合并引入重复或不一致的变更之后

**不适用场景：**

- 代码已经干净可读，不要为了简化而简化
- 你还不理解代码做什么，先理解再简化
- 代码性能关键，而“更简单”的版本会显著更慢
- 你马上要完全重写该模块，简化即将丢弃的代码是在浪费精力

## 五项原则

### 1. 精确保留行为

不要改变代码做什么，只改变它如何表达。所有输入、输出、副作用、错误行为和边界情况都必须保持相同。如果你不确定某个简化是否保留行为，就不要做。

```
ASK BEFORE EVERY CHANGE:
→ Does this produce the same output for every input?
→ Does this maintain the same error behavior?
→ Does this preserve the same side effects and ordering?
→ Do all existing tests still pass without modification?
```

### 2. 遵循项目约定

简化意味着让代码更符合代码库，而不是强加外部偏好。简化前：

```
1. Read CLAUDE.md / project conventions
2. Study how neighboring code handles similar patterns
3. Match the project's style for:
   - Import ordering and module system
   - Function declaration style
   - Naming conventions
   - Error handling patterns
   - Type annotation depth
```

破坏项目一致性的简化不是简化，而是 churn。

### 3. 清晰优先于巧妙

当紧凑版本需要停下来思考才能解析时，显式代码优于紧凑代码。

```typescript
// UNCLEAR: Dense ternary chain
const label = isNew ? 'New' : isUpdated ? 'Updated' : isArchived ? 'Archived' : 'Active';

// CLEAR: Readable mapping
function getStatusLabel(item: Item): string {
  if (item.isNew) return 'New';
  if (item.isUpdated) return 'Updated';
  if (item.isArchived) return 'Archived';
  return 'Active';
}
```

```typescript
// UNCLEAR: Chained reduces with inline logic
const result = items.reduce((acc, item) => ({
  ...acc,
  [item.id]: { ...acc[item.id], count: (acc[item.id]?.count ?? 0) + 1 }
}), {});

// CLEAR: Named intermediate step
const countById = new Map<string, number>();
for (const item of items) {
  countById.set(item.id, (countById.get(item.id) ?? 0) + 1);
}
```

### 4. 保持平衡

简化有一种失败模式：过度简化。注意这些陷阱：

- **过度 inline**：移除给概念命名的 helper，会让调用点更难读
- **合并无关逻辑**：把两个简单函数合成一个复杂函数并不更简单
- **移除“非必要”抽象**：有些抽象是为了 extensibility 或 testability，而不是为了复杂度
- **以行数为优化目标**：更少行不是目标，更快理解才是目标

### 5. 聚焦已变更范围

默认简化最近修改的代码。除非明确要求扩大范围，否则避免顺手重构无关代码。无范围的简化会给 diff 制造噪音，并带来意外回归风险。

## 简化流程

### Step 1: 先理解再动手（Chesterton's Fence）

在修改或移除任何东西前，先理解它为什么存在。这就是 Chesterton's Fence：如果你看到路中间有一道栅栏却不知道它为什么在那里，不要拆掉它。先理解原因，再判断原因是否仍然成立。

```
BEFORE SIMPLIFYING, ANSWER:
- What is this code's responsibility?
- What calls it? What does it call?
- What are the edge cases and error paths?
- Are there tests that define the expected behavior?
- Why might it have been written this way? (Performance? Platform constraint? Historical reason?)
- Check git blame: what was the original context for this code?
```

如果你答不上来，就还没准备好简化。先多读上下文。

### Step 2: 识别简化机会

扫描这些模式。每一个都是具体信号，而不是模糊气味：

**结构复杂度：**

| Pattern | Signal | Simplification |
|---------|--------|----------------|
| Deep nesting (3+ levels) | 控制流难以跟随 | 将条件提取为 guard clauses 或 helper functions |
| Long functions (50+ lines) | 多个职责 | 拆成带描述性名称的 focused functions |
| Nested ternaries | 需要 mental stack 才能解析 | 替换为 if/else chains、switch 或 lookup objects |
| Boolean parameter flags | `doThing(true, false, true)` | 替换为 options objects 或 separate functions |
| Repeated conditionals | 同一个 `if` check 出现在多处 | 提取为命名良好的 predicate function |

**命名与可读性：**

| Pattern | Signal | Simplification |
|---------|--------|----------------|
| Generic names | `data`, `result`, `temp`, `val`, `item` | 改名以描述内容：`userProfile`, `validationErrors` |
| Abbreviated names | `usr`, `cfg`, `btn`, `evt` | 使用完整单词，除非缩写是通用的（`id`, `url`, `api`） |
| Misleading names | 名为 `get` 的函数却也 mutate state | 改名以反映真实行为 |
| Comments explaining "what" | `// increment counter` 位于 `count++` 上方 | 删除注释，代码已足够清晰 |
| Comments explaining "why" | `// Retry because the API is flaky under load` | 保留，这些注释承载代码无法表达的意图 |

**冗余：**

| Pattern | Signal | Simplification |
|---------|--------|----------------|
| Duplicated logic | 多处出现相同 5+ 行 | 提取为 shared function |
| Dead code | 不可达 branches、unused variables、commented-out blocks | 移除（确认确实 dead 后） |
| Unnecessary abstractions | wrapper 没有增加价值 | inline wrapper，直接调用 underlying function |
| Over-engineered patterns | factory-for-a-factory、strategy-with-one-strategy | 替换为简单直接方法 |
| Redundant type assertions | 对已经 inferred 的类型进行 casting | 移除 assertion |

### Step 3: 增量应用变更

一次只做一个简化。每次变更后运行测试。**将 refactoring changes 与 feature 或 bug fix changes 分开提交。** 同时重构和添加功能的 PR 是两个 PR，应拆分。

```
FOR EACH SIMPLIFICATION:
1. Make the change
2. Run the test suite
3. If tests pass → commit (or continue to next simplification)
4. If tests fail → revert and reconsider
```

避免把多个简化打包成一个未经测试的变更。如果出错，你需要知道是哪次简化造成的。

**The Rule of 500:** 如果一次重构会触及超过 500 行，应投入自动化（codemods、sed scripts、AST transforms），而不是手工修改。这个规模的手工编辑容易出错，也让审查很疲惫。

### Step 4: 验证结果

完成所有简化后，退一步评估整体：

```
COMPARE BEFORE AND AFTER:
- Is the simplified version genuinely easier to understand?
- Did you introduce any new patterns inconsistent with the codebase?
- Is the diff clean and reviewable?
- Would a teammate approve this change?
```

如果“简化”后的版本更难理解或审查，就 revert。并非每次简化尝试都会成功。

## 特定语言指导

### TypeScript / JavaScript

```typescript
// SIMPLIFY: Unnecessary async wrapper
// Before
async function getUser(id: string): Promise<User> {
  return await userService.findById(id);
}
// After
function getUser(id: string): Promise<User> {
  return userService.findById(id);
}

// SIMPLIFY: Verbose conditional assignment
// Before
let displayName: string;
if (user.nickname) {
  displayName = user.nickname;
} else {
  displayName = user.fullName;
}
// After
const displayName = user.nickname || user.fullName;

// SIMPLIFY: Manual array building
// Before
const activeUsers: User[] = [];
for (const user of users) {
  if (user.isActive) {
    activeUsers.push(user);
  }
}
// After
const activeUsers = users.filter((user) => user.isActive);

// SIMPLIFY: Redundant boolean return
// Before
function isValid(input: string): boolean {
  if (input.length > 0 && input.length < 100) {
    return true;
  }
  return false;
}
// After
function isValid(input: string): boolean {
  return input.length > 0 && input.length < 100;
}
```

### Python

```python
# SIMPLIFY: Verbose dictionary building
# Before
result = {}
for item in items:
    result[item.id] = item.name
# After
result = {item.id: item.name for item in items}

# SIMPLIFY: Nested conditionals with early return
# Before
def process(data):
    if data is not None:
        if data.is_valid():
            if data.has_permission():
                return do_work(data)
            else:
                raise PermissionError("No permission")
        else:
            raise ValueError("Invalid data")
    else:
        raise TypeError("Data is None")
# After
def process(data):
    if data is None:
        raise TypeError("Data is None")
    if not data.is_valid():
        raise ValueError("Invalid data")
    if not data.has_permission():
        raise PermissionError("No permission")
    return do_work(data)
```

### React / JSX

```tsx
// SIMPLIFY: Verbose conditional rendering
// Before
function UserBadge({ user }: Props) {
  if (user.isAdmin) {
    return <Badge variant="admin">Admin</Badge>;
  } else {
    return <Badge variant="default">User</Badge>;
  }
}
// After
function UserBadge({ user }: Props) {
  const variant = user.isAdmin ? 'admin' : 'default';
  const label = user.isAdmin ? 'Admin' : 'User';
  return <Badge variant={variant}>{label}</Badge>;
}

// SIMPLIFY: Prop drilling through intermediate components
// Before — consider whether context or composition solves this better.
// This is a judgment call — flag it, don't auto-refactor.
```

## Common Rationalizations

| Rationalization | Reality |
|---|---|
| “它能工作，没必要碰” | 难读的工作代码在出问题时也难修。现在简化能节省未来每次变更的时间。 |
| “更少行总是更简单” | 1 行 nested ternary 并不比 5 行 if/else 更简单。简单性关乎理解速度，而不是行数。 |
| “我顺手快速简化这段无关代码” | 无范围简化会制造 noisy diffs，并给你本不打算改的代码带来回归风险。保持聚焦。 |
| “类型让它自文档化了” | 类型记录结构，不记录意图。命名良好的函数比类型签名更能解释 *why*。 |
| “这个抽象以后可能有用” | 不要保留投机性抽象。如果现在没有使用，它就是没有价值的复杂度。移除它，需要时再加。 |
| “原作者一定有理由” | 也许。检查 git blame，应用 Chesterton's Fence。但积累复杂度往往没有理由，只是压力下迭代留下的残渣。 |
| “我一边加功能一边重构” | 将重构与功能工作分开。混合变更更难审查、回滚和理解历史。 |

## Red Flags

- 简化需要修改 tests 才能通过（你很可能改变了行为）
- “简化”后的代码比原来更长且更难跟随
- 按你的偏好重命名，而不是按项目约定
- 因为“让代码更干净”而移除错误处理
- 简化你没有完全理解的代码
- 把许多简化打包成一个大型、难审查的 commit
- 未被要求时重构当前任务范围外的代码

## Verification

完成一次简化后：

- [ ] 所有现有 tests 不经修改就通过
- [ ] Build 成功且没有新 warnings
- [ ] Linter/formatter 通过（没有 style regressions）
- [ ] 每项简化都是可审查、增量的变更
- [ ] Diff 干净，没有混入无关变更
- [ ] 简化后的代码遵循项目约定（已对照 CLAUDE.md 或等价文件）
- [ ] 没有移除或弱化错误处理
- [ ] 没有留下 dead code（unused imports、unreachable branches）
- [ ] 队友或 review agent 会批准该变更是净改进