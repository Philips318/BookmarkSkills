---
name: documentation-and-adrs
description: 记录决策和文档。使用场景：做出架构决策、修改 public APIs、发布功能，或需要记录未来工程师和 agents 理解代码库所需的上下文。
---

# 文档与 ADRs

## 概述

记录决策，而不只是记录代码。最有价值的文档捕获 *why*，也就是导致某个决策的上下文、约束和取舍。代码展示构建了 *what*；文档解释 *why it was built this way*，以及 *what alternatives were considered*。这些上下文对未来在代码库中工作的 humans 和 agents 都至关重要。

## 使用场景

- 做出重要架构决策
- 在竞争方案之间选择
- 添加或修改 public API
- 发布改变 user-facing behavior 的功能
- 让新团队成员（或 agents）onboard 项目
- 当你发现自己反复解释同一件事时

**不适用场景：** 不要记录显而易见的代码。不要添加只是复述代码已经表达内容的注释。不要为 throwaway prototypes 写文档。

## Architecture Decision Records (ADRs)

ADRs 捕获重要技术决策背后的推理。它们是你能写出的最高价值文档。

### 何时写 ADR

- 选择 framework、library 或 major dependency
- 设计 data model 或 database schema
- 选择 authentication strategy
- 决定 API architecture（REST vs. GraphQL vs. tRPC）
- 在 build tools、hosting platforms 或 infrastructure 之间选择
- 任何回滚代价高的决策

### ADR Template

将 ADRs 按顺序编号存储在 `docs/decisions/`：

```markdown
# ADR-001: Use PostgreSQL for primary database

## Status
Accepted | Superseded by ADR-XXX | Deprecated

## Date
2025-01-15

## Context
We need a primary database for the task management application. Key requirements:
- Relational data model (users, tasks, teams with relationships)
- ACID transactions for task state changes
- Support for full-text search on task content
- Managed hosting available (for small team, limited ops capacity)

## Decision
Use PostgreSQL with Prisma ORM.

## Alternatives Considered

### MongoDB
- Pros: Flexible schema, easy to start with
- Cons: Our data is inherently relational; would need to manage relationships manually
- Rejected: Relational data in a document store leads to complex joins or data duplication

### SQLite
- Pros: Zero configuration, embedded, fast for reads
- Cons: Limited concurrent write support, no managed hosting for production
- Rejected: Not suitable for multi-user web application in production

### MySQL
- Pros: Mature, widely supported
- Cons: PostgreSQL has better JSON support, full-text search, and ecosystem tooling
- Rejected: PostgreSQL is the better fit for our feature requirements

## Consequences
- Prisma provides type-safe database access and migration management
- We can use PostgreSQL's full-text search instead of adding Elasticsearch
- Team needs PostgreSQL knowledge (standard skill, low risk)
- Hosting on managed service (Supabase, Neon, or RDS)
```

### ADR Lifecycle

```
PROPOSED → ACCEPTED → (SUPERSEDED or DEPRECATED)
```

- **不要删除旧 ADRs。** 它们捕获历史上下文。
- 当决策变化时，写一份新的 ADR 引用并 supersede 旧 ADR。

## Inline Documentation

### 何时注释

注释 *why*，而不是 *what*：

```typescript
// BAD: Restates the code
// Increment counter by 1
counter += 1;

// GOOD: Explains non-obvious intent
// Rate limit uses a sliding window — reset counter at window boundary,
// not on a fixed schedule, to prevent burst attacks at window edges
if (now - windowStart > WINDOW_SIZE_MS) {
  counter = 0;
  windowStart = now;
}
```

### 何时不注释

```typescript
// Don't comment self-explanatory code
function calculateTotal(items: CartItem[]): number {
  return items.reduce((sum, item) => sum + item.price * item.quantity, 0);
}

// Don't leave TODO comments for things you should just do now
// TODO: add error handling  ← Just add it

// Don't leave commented-out code
// const oldImplementation = () => { ... }  ← Delete it, git has history
```

### 记录已知陷阱

```typescript
/**
 * IMPORTANT: This function must be called before the first render.
 * If called after hydration, it causes a flash of unstyled content
 * because the theme context isn't available during SSR.
 *
 * See ADR-003 for the full design rationale.
 */
export function initializeTheme(theme: Theme): void {
  // ...
}
```

## API Documentation

对于 public APIs（REST、GraphQL、library interfaces）：

### Inline with Types（TypeScript 首选）

```typescript
/**
 * Creates a new task.
 *
 * @param input - Task creation data (title required, description optional)
 * @returns The created task with server-generated ID and timestamps
 * @throws {ValidationError} If title is empty or exceeds 200 characters
 * @throws {AuthenticationError} If the user is not authenticated
 *
 * @example
 * const task = await createTask({ title: 'Buy groceries' });
 * console.log(task.id); // "task_abc123"
 */
export async function createTask(input: CreateTaskInput): Promise<Task> {
  // ...
}
```

### OpenAPI / Swagger for REST APIs

```yaml
paths:
  /api/tasks:
    post:
      summary: Create a task
      requestBody:
        required: true
        content:
          application/json:
            schema:
              $ref: '#/components/schemas/CreateTaskInput'
      responses:
        '201':
          description: Task created
          content:
            application/json:
              schema:
                $ref: '#/components/schemas/Task'
        '422':
          description: Validation error
```

## README Structure

每个项目都应有 README，覆盖：

```markdown
# Project Name

One-paragraph description of what this project does.

## Quick Start
1. Clone the repo
2. Install dependencies: `npm install`
3. Set up environment: `cp .env.example .env`
4. Run the dev server: `npm run dev`

## Commands
| Command | Description |
|---------|-------------|
| `npm run dev` | Start development server |
| `npm test` | Run tests |
| `npm run build` | Production build |
| `npm run lint` | Run linter |

## Architecture
Brief overview of the project structure and key design decisions.
Link to ADRs for details.

## Contributing
How to contribute, coding standards, PR process.
```

## Changelog Maintenance

对于已发布功能：

```markdown
# Changelog

## [1.2.0] - 2025-01-20
### Added
- Task sharing: users can share tasks with team members (#123)
- Email notifications for task assignments (#124)

### Fixed
- Duplicate tasks appearing when rapidly clicking create button (#125)

### Changed
- Task list now loads 50 items per page (was 20) for better UX (#126)
```

## Documentation for Agents

对 AI agent context 的特别考虑：

- **CLAUDE.md / rules files**：记录项目约定，让 agents 遵循它们
- **Spec files**：保持 specs 更新，让 agents 构建正确内容
- **ADRs**：帮助 agents 理解过去为什么做出某些决策（防止重新决策）
- **Inline gotchas**：防止 agents 掉入已知陷阱

## Common Rationalizations

| Rationalization | Reality |
|---|---|
| “代码是自文档化的” | 代码展示 what。它不展示 why、哪些替代方案被拒绝、适用哪些约束。 |
| “API 稳定后再写文档” | 写文档会让 API 更快稳定。文档是设计的第一道测试。 |
| “没人读文档” | Agents 会读。未来工程师会读。三个月后的你也会读。 |
| “ADRs 是负担” | 10 分钟的 ADR 可以避免六个月后围绕同一决策争论 2 小时。 |
| “注释会过时” | 关于 *why* 的注释稳定。关于 *what* 的注释会过时，所以你只写前者。 |

## Red Flags

- 架构决策没有书面 rationale
- Public APIs 没有 documentation 或 types
- README 没解释如何运行项目
- 用 commented-out code 代替删除
- TODO comments 已存在数周
- 有重要架构选择的项目没有 ADRs
- 文档复述代码，而不是解释意图

## Verification

完成文档后：

- [ ] 所有重要架构决策都有 ADRs
- [ ] README 覆盖 quick start、commands 和 architecture overview
- [ ] API functions 有 parameter 和 return type documentation
- [ ] Known gotchas 在相关位置 inline 记录
- [ ] 没有留下 commented-out code
- [ ] Rules files（CLAUDE.md 等）当前且准确