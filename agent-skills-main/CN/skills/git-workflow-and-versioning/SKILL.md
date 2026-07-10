---
name: git-workflow-and-versioning
description: 组织 git 工作流实践。用于进行任何代码变更时。用于提交、创建分支、解决冲突，或需要组织多个并行工作流时。用于发布版本、选择语义化版本提升、打标签或编写 changelog 时。
---

# Git 工作流和版本管理

## 概述

Git 是你的安全网。把提交当作保存点，把分支当作沙盒，把历史当作文档。随着 AI agent 高速生成代码，严谨的版本控制就是让变更保持可管理、可审查、可回滚的机制。

## 何时使用

始终使用。每一次代码变更都经过 git。

## 核心原则

### 基于主干的开发（推荐）

保持 `main` 始终可部署。在短生命周期的功能分支中工作，并在 1-3 天内合并回主干。长期存在的开发分支是隐藏成本：它们会分叉、制造合并冲突并延迟集成。DORA 研究持续表明，基于主干的开发与高绩效工程团队相关。

```
main ──●──●──●──●──●──●──●──●──●──  (always deployable)
        ╲      ╱  ╲    ╱
         ●──●─╱    ●──╱    ← short-lived feature branches (1-3 days)
```

这是推荐的默认方式。使用 gitflow 或长期分支的团队也可以把这些原则（原子提交、小变更、描述性消息）适配到自己的分支模型中。提交纪律比分支策略本身更重要。

- **Dev 分支是成本。** 分支每多活一天，就会积累合并风险。
- **Release 分支可以接受。** 当你需要在 main 继续前进的同时稳定一个发布版本。
- **功能开关优于长期分支。** 更偏向把未完成工作隐藏在开关后部署，而不是让它在分支上停留数周。

### 1. 早提交，常提交

每个成功的增量都应有自己的提交。不要积累大量未提交变更。

```
Work pattern:
  Implement slice → Test → Verify → Commit → Next slice

Not this:
  Implement everything → Hope it works → Giant commit
```

提交是保存点。如果下一次变更破坏了什么，你可以立即回到上一个已知良好状态。

### 2. 原子提交

每个提交只做一件逻辑上的事：

```
# Good: Each commit is self-contained
git log --oneline
a1b2c3d Add task creation endpoint with validation
d4e5f6g Add task creation form component
h7i8j9k Connect form to API and add loading state
m1n2o3p Add task creation tests (unit + integration)

# Bad: Everything mixed together
git log --oneline
x1y2z3a Add task feature, fix sidebar, update deps, refactor utils
```

### 3. 描述性消息

提交消息解释的是*为什么*，不只是*做了什么*：

```
# Good: Explains intent
feat: add email validation to registration endpoint

Prevents invalid email formats from reaching the database.
Uses Zod schema validation at the route handler level,
consistent with existing validation patterns in auth.ts.

# Bad: Describes what's obvious from the diff
update auth.ts
```

**格式：**
```
<type>: <short description>

<optional body explaining why, not what>
```

**类型：**
- `feat` — 新功能
- `fix` — Bug 修复
- `refactor` — 既不修复 bug 也不添加功能的代码变更
- `test` — 添加或更新测试
- `docs` — 仅文档
- `chore` — 工具、依赖、配置

### 4. 分离关注点

不要把格式化变更和行为变更混在一起。不要把重构和功能混在一起。每种变更都应该是单独的提交，理想情况下也是单独的 PR：

```
# Good: Separate concerns
git commit -m "refactor: extract validation logic to shared utility"
git commit -m "feat: add phone number validation to registration"

# Bad: Mixed concerns
git commit -m "refactor validation and add phone number field"
```

**将重构与功能工作分开。** 重构变更和功能变更是两种不同的变更，请分别提交。这样每个变更更容易审查、回滚，也更容易在历史中理解。小清理（例如重命名变量）可以由评审者酌情包含在功能提交中。

### 5. 控制变更大小

目标是每个提交/PR 约 100 行。超过约 1000 行的变更应拆分。如何拆分大型变更，请参阅 `code-review-and-quality` 中的拆分策略。

```
~100 lines  → Easy to review, easy to revert
~300 lines  → Acceptable for a single logical change
~1000 lines → Split into smaller changes
```

## 分支策略

### 功能分支

```
main (always deployable)
  │
  ├── feature/task-creation    ← One feature per branch
  ├── feature/user-settings    ← Parallel work
  └── fix/duplicate-tasks      ← Bug fixes
```

- 从 `main`（或团队默认分支）创建分支
- 保持分支短生命周期（1-3 天内合并）：长期分支是隐藏成本
- 合并后删除分支
- 对未完成功能，优先使用功能开关，而不是长期分支

### 分支命名

```
feature/<short-description>   → feature/task-creation
fix/<short-description>       → fix/duplicate-tasks
chore/<short-description>     → chore/update-deps
refactor/<short-description>  → refactor/auth-module
```

## 使用 Worktree

对于并行 AI agent 工作，使用 git worktree 同时运行多个分支：

```bash
# Create a worktree for a feature branch
git worktree add ../project-feature-a feature/task-creation
git worktree add ../project-feature-b feature/user-settings

# Each worktree is a separate directory with its own branch
# Agents can work in parallel without interfering
ls ../
  project/              ← main branch
  project-feature-a/    ← task-creation branch
  project-feature-b/    ← user-settings branch

# When done, merge and clean up
git worktree remove ../project-feature-a
```

好处：
- 多个 agent 可以同时处理不同功能
- 不需要切换分支（每个目录都有自己的分支）
- 如果某个实验失败，删除 worktree 即可，不会丢失其他内容
- 变更会一直隔离，直到显式合并

## 保存点模式

```
Agent starts work
    │
    ├── Makes a change
    │   ├── Test passes? → Commit → Continue
    │   └── Test fails? → Revert to last commit → Investigate
    │
    ├── Makes another change
    │   ├── Test passes? → Commit → Continue
    │   └── Test fails? → Revert to last commit → Investigate
    │
    └── Feature complete → All commits form a clean history
```

这个模式意味着你最多只会丢失一个增量的工作。如果 agent 偏离轨道，`git reset --hard HEAD` 会把你带回上一个成功状态。

## 变更摘要

任何修改之后，都提供结构化摘要。这会让审查更容易，记录范围纪律，并暴露意外变更：

```
CHANGES MADE:
- src/routes/tasks.ts: Added validation middleware to POST endpoint
- src/lib/validation.ts: Added TaskCreateSchema using Zod

THINGS I DIDN'T TOUCH (intentionally):
- src/routes/auth.ts: Has similar validation gap but out of scope
- src/middleware/error.ts: Error format could be improved (separate task)

POTENTIAL CONCERNS:
- The Zod schema is strict — rejects extra fields. Confirm this is desired.
- Added zod as a dependency (72KB gzipped) — already in package.json
```

这种模式会及早捕捉错误假设，并给评审者一张清晰的变更地图。“未触碰”的部分尤其重要，它表明你遵守了范围纪律，没有进行未经请求的翻新。

## 提交前卫生

每次提交前：

```bash
# 1. Check what you're about to commit
git diff --staged

# 2. Ensure no secrets
git diff --staged | grep -i "password\|secret\|api_key\|token"

# 3. Run tests
npm test

# 4. Run linting
npm run lint

# 5. Run type checking
npx tsc --noEmit
```

用 git hook 自动化：

```json
// package.json (using lint-staged + husky)
{
  "lint-staged": {
    "*.{ts,tsx}": ["eslint --fix", "prettier --write"],
    "*.{json,md}": ["prettier --write"]
  }
}
```

## 处理生成文件

- **只在项目期望时提交生成文件**，例如 `package-lock.json`、Prisma migrations
- **不要提交** 构建输出（`dist/`、`.next/`）、环境文件（`.env`）或 IDE 配置（除非共享的 `.vscode/settings.json`）
- **准备 `.gitignore`**，覆盖：`node_modules/`、`dist/`、`.env`、`.env.local`、`*.pem`

## 用 Git 调试

```bash
# Find which commit introduced a bug
git bisect start
git bisect bad HEAD
git bisect good <known-good-commit>
# Git checkouts midpoints; run your test at each to narrow down

# View what changed recently
git log --oneline -20
git diff HEAD~5..HEAD -- src/

# Find who last changed a specific line
git blame src/services/task.ts

# Search commit messages for a keyword
git log --grep="validation" --oneline
```

## 发布与版本管理

提交是*你*跟踪变更的方式；**版本**是你的*消费者*跟踪变更的方式。一旦有任何其他人依赖你的代码，另一个团队、已发布的软件包、已部署的客户端都算，“main 上最新版本”就不再足以回答“我运行的是什么，升级安全吗？”版本号和 changelog 就是回答这个问题的契约。

### 语义化版本

对于任何有消费者的东西，使用 `MAJOR.MINOR.PATCH` 版本，并让数字携带含义：

```
  MAJOR  breaking change — consumers must change their code to upgrade
  MINOR  new functionality, backward-compatible — safe to upgrade
  PATCH  bug fix, backward-compatible — safe to upgrade
```

数字是一种承诺，所以代码要与之匹配。一个改变了消费者依赖行为的“patch”，本质上是披着伪装的 major 变更（Hyrum's Law，请参阅 `api-and-interface-design` 技能）。当不确定某个变更是否破坏兼容时，假设它是破坏性的；意外的 major 比破坏消费者要便宜得多。

### 给发布打标签，并让标签成为事实来源

发布是历史中不可变的点，而不是移动的分支。给它打标签，让它始终可以复现：

```bash
git tag -a v1.4.0 -m "Release 1.4.0"
git push origin v1.4.0
```

从标签推导版本，而不是在分散的文件中手动编辑版本号，这样 artifact、tag 和 changelog 永远不会互相不一致。

### 保持面向人的 changelog

changelog 不是 `git log`。它是面向消费者精心整理的“发生了什么、我是否需要关心？”的答案，按 `Added / Changed / Fixed / Deprecated / Removed / Security` 分组，最新版本在最上方，每条都围绕用户影响表述，而不是内部机制。

```markdown
## [1.4.0] - 2025-06-12
### Added
- Bulk task import via CSV
### Fixed
- Timezone drift in recurring task due dates
### Deprecated
- `GET /v1/tasks/all` — use the paginated `GET /v1/tasks` (removal in 2.0)
```

在做出变更的同一次变更里写 changelog 条目，趁影响还新鲜，而不是到发布时间再从提交考古中重建。破坏性变更需要迁移说明和弃用窗口（遵循 `deprecation-and-migration` 技能）；真正发布版本是 `shipping-and-launch` 技能的职责。本节是供它使用的版本契约。

## 常见合理化借口

| 合理化借口 | 现实 |
|---|---|
| “功能完成后我再提交” | 一个巨型提交无法审查、调试或回滚。每个切片都要提交。 |
| “提交消息不重要” | 消息就是文档。未来的你（以及未来的 agent）需要理解发生了什么以及为什么。 |
| “之后我会全部 squash” | Squash 会摧毁开发叙事。最好从一开始就保持干净的增量提交。 |
| “分支增加开销” | 短生命周期分支几乎免费，并能防止冲突工作互相碰撞。长期分支才是问题：1-3 天内合并。 |
| “之后我会拆分这个变更” | 大变更更难审查、部署风险更高，也更难回滚。在提交前拆分，而不是之后。 |
| “我不需要 .gitignore” | 直到包含生产密钥的 `.env` 被提交。立即设置它。 |
| “只是个小修复，提升 patch 版本就行” | 检查消费者能观察到什么。他们依赖的行为发生变化就是 major，无论 diff 多小。 |
| “changelog 就是提交日志” | 提交是给你看的；changelog 是给消费者看的，要按影响精心整理。直接从原始提交生成会埋没重要信息。 |
| “我们发布时间再写 changelog” | 到那时影响只能靠记忆重建，而且一半会缺失。随变更一起写条目。 |

## 危险信号

- 大量未提交变更持续积累
- 提交消息像 “fix”、“update”、“misc”
- 格式化变更与行为变更混在一起
- 项目中没有 `.gitignore`
- 提交 `node_modules/`、`.env` 或构建产物
- 长期分支与 main 显著分叉
- 强推到共享分支
- 破坏性变更以 minor 或 patch 版本发布
- 发布没有 tag，或手动编辑的版本号与 tag 不同步
- 面向用户的发布没有 changelog 条目，或 changelog 只是倾倒提交消息

## 验证

对每个提交：

- [ ] 提交只做一件逻辑上的事
- [ ] 消息解释了为什么，并遵循类型约定
- [ ] 提交前测试通过
- [ ] diff 中没有 secrets
- [ ] 没有把纯格式化变更与行为变更混在一起
- [ ] `.gitignore` 覆盖标准排除项

对每个发布（任何有消费者的内容）：

- [ ] 版本提升与变更匹配：破坏性 → major，新增兼容功能 → minor，修复 → patch
- [ ] 发布已打标签，版本来自 tag，而不是手动编辑到与 tag 不同步
- [ ] changelog 中有为该版本按影响分组、面向人的精心整理条目
