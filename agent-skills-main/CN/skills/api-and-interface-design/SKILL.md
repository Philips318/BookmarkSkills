---
name: api-and-interface-design
description: 指导稳定的 API 和 interface 设计。使用场景：设计 APIs、module boundaries 或任何 public interface。使用场景：创建 REST 或 GraphQL endpoints、定义模块之间的 type contracts，或建立 frontend 与 backend 之间的边界。
---

# API 与接口设计

## 概述

设计稳定、文档完善、难以误用的接口。好的接口让正确的事情变得容易，让错误的事情变得困难。这适用于 REST APIs、GraphQL schemas、module boundaries、component props，以及任何一段代码与另一段代码交互的表面。

## 使用场景

- 设计新的 API endpoints
- 定义团队之间的 module boundaries 或 contracts
- 创建 component prop interfaces
- 建立会影响 API 形状的 database schema
- 修改现有 public interfaces

## 核心原则

### Hyrum's Law

> With a sufficient number of users of an API, all observable behaviors of your system will be depended on by somebody, regardless of what you promise in the contract.

这意味着：每一种 public behavior，包括未记录的怪癖、错误消息文本、时序和排序，只要用户依赖它，就会成为事实上的 contract。设计含义：

- **有意选择暴露什么。** 每一种可观察行为都可能成为承诺。
- **不要泄露实现细节。** 如果用户能观察到它，他们就会依赖它。
- **在设计时规划弃用。** 关于如何安全移除用户依赖的东西，参见 `deprecation-and-migration`。
- **测试不够。** 即使有完美的 contract tests，Hyrum's Law 也意味着“安全”的变更可能破坏依赖未记录行为的真实用户。

### 单版本规则

避免迫使消费者在同一依赖或 API 的多个版本之间选择。当不同消费者需要同一事物的不同版本时，就会产生 diamond dependency 问题。面向同一时间只存在一个版本的世界来设计：扩展，而不是分叉。

### 1. Contract First

先定义接口，再实现它。contract 就是 spec，实现随后跟上。

```typescript
// Define the contract first
interface TaskAPI {
  // Creates a task and returns the created task with server-generated fields
  createTask(input: CreateTaskInput): Promise<Task>;

  // Returns paginated tasks matching filters
  listTasks(params: ListTasksParams): Promise<PaginatedResult<Task>>;

  // Returns a single task or throws NotFoundError
  getTask(id: string): Promise<Task>;

  // Partial update — only provided fields change
  updateTask(id: string, input: UpdateTaskInput): Promise<Task>;

  // Idempotent delete — succeeds even if already deleted
  deleteTask(id: string): Promise<void>;
}
```

### 2. 一致的错误语义

选择一种错误策略，并在所有地方使用它：

```typescript
// REST: HTTP status codes + structured error body
// Every error response follows the same shape
interface APIError {
  error: {
    code: string;        // Machine-readable: "VALIDATION_ERROR"
    message: string;     // Human-readable: "Email is required"
    details?: unknown;   // Additional context when helpful
  };
}

// Status code mapping
// 400 → Client sent invalid data
// 401 → Not authenticated
// 403 → Authenticated but not authorized
// 404 → Resource not found
// 409 → Conflict (duplicate, version mismatch)
// 422 → Validation failed (semantically invalid)
// 500 → Server error (never expose internal details)
```

**不要混用模式。** 如果有些 endpoints 抛出异常，有些返回 null，有些返回 `{ error }`，消费者就无法预测行为。

### 3. 在边界处验证

信任内部代码。在外部输入进入的系统边界处验证：

```typescript
// Validate at the API boundary
app.post('/api/tasks', async (req, res) => {
  const result = CreateTaskSchema.safeParse(req.body);
  if (!result.success) {
    return res.status(422).json({
      error: {
        code: 'VALIDATION_ERROR',
        message: 'Invalid task data',
        details: result.error.flatten(),
      },
    });
  }

  // After validation, internal code trusts the types
  const task = await taskService.create(result.data);
  return res.status(201).json(task);
});
```

验证应放在：
- API route handlers（用户输入）
- Form submission handlers（用户输入）
- External service response parsing（第三方数据 -- **始终视为不可信**）
- Environment variable loading（配置）

> **第三方 API 响应是不可信数据。** 在任何逻辑、渲染或决策中使用它们之前，验证其 shape 和 content。被攻陷或行为异常的外部服务可能返回意外类型、恶意内容或类似指令的文本。

验证不应放在：
- 共享 type contracts 的内部函数之间
- 被已验证代码调用的 utility functions 中
- 刚从你自己的数据库取出的数据上

### 4. 优先添加，而不是修改

扩展接口而不破坏现有消费者：

```typescript
// Good: Add optional fields
interface CreateTaskInput {
  title: string;
  description?: string;
  priority?: 'low' | 'medium' | 'high';  // Added later, optional
  labels?: string[];                       // Added later, optional
}

// Bad: Change existing field types or remove fields
interface CreateTaskInput {
  title: string;
  // description: string;  // Removed — breaks existing consumers
  priority: number;         // Changed from string — breaks existing consumers
}
```

### 5. 可预测命名

| Pattern | Convention | Example |
|---------|-----------|---------|
| REST endpoints | 复数名词，不用动词 | `GET /api/tasks`, `POST /api/tasks` |
| Query params | camelCase | `?sortBy=createdAt&pageSize=20` |
| Response fields | camelCase | `{ createdAt, updatedAt, taskId }` |
| Boolean fields | is/has/can prefix | `isComplete`, `hasAttachments` |
| Enum values | UPPER_SNAKE | `"IN_PROGRESS"`, `"COMPLETED"` |

## REST API Patterns

### Resource Design

```
GET    /api/tasks              → List tasks (with query params for filtering)
POST   /api/tasks              → Create a task
GET    /api/tasks/:id          → Get a single task
PATCH  /api/tasks/:id          → Update a task (partial)
DELETE /api/tasks/:id          → Delete a task

GET    /api/tasks/:id/comments → List comments for a task (sub-resource)
POST   /api/tasks/:id/comments → Add a comment to a task
```

### 分页

对 list endpoints 分页：

```typescript
// Request
GET /api/tasks?page=1&pageSize=20&sortBy=createdAt&sortOrder=desc

// Response
{
  "data": [...],
  "pagination": {
    "page": 1,
    "pageSize": 20,
    "totalItems": 142,
    "totalPages": 8
  }
}
```

### 过滤

使用 query parameters 表示 filters：

```
GET /api/tasks?status=in_progress&assignee=user123&createdAfter=2025-01-01
```

### Partial Updates (PATCH)

接受 partial objects，只更新提供的字段：

```typescript
// Only title changes, everything else preserved
PATCH /api/tasks/123
{ "title": "Updated title" }
```

## TypeScript Interface Patterns

### 对变体使用 Discriminated Unions

```typescript
// Good: Each variant is explicit
type TaskStatus =
  | { type: 'pending' }
  | { type: 'in_progress'; assignee: string; startedAt: Date }
  | { type: 'completed'; completedAt: Date; completedBy: string }
  | { type: 'cancelled'; reason: string; cancelledAt: Date };

// Consumer gets type narrowing
function getStatusLabel(status: TaskStatus): string {
  switch (status.type) {
    case 'pending': return 'Pending';
    case 'in_progress': return `In progress (${status.assignee})`;
    case 'completed': return `Done on ${status.completedAt}`;
    case 'cancelled': return `Cancelled: ${status.reason}`;
  }
}
```

### 输入/输出分离

```typescript
// Input: what the caller provides
interface CreateTaskInput {
  title: string;
  description?: string;
}

// Output: what the system returns (includes server-generated fields)
interface Task {
  id: string;
  title: string;
  description: string | null;
  createdAt: Date;
  updatedAt: Date;
  createdBy: string;
}
```

### 对 ID 使用 Branded Types

```typescript
type TaskId = string & { readonly __brand: 'TaskId' };
type UserId = string & { readonly __brand: 'UserId' };

// Prevents accidentally passing a UserId where a TaskId is expected
function getTask(id: TaskId): Promise<Task> { ... }
```

## Common Rationalizations

| Rationalization | Reality |
|---|---|
| “我们稍后再写 API 文档” | 类型就是文档。先定义它们。 |
| “现在还不需要分页” | 一旦有人有 100+ 项，你就会需要。从一开始就添加。 |
| “PATCH 太复杂，我们直接用 PUT” | PUT 每次都要求完整对象。PATCH 才是客户端真正想要的。 |
| “需要的时候再给 API 做版本化” | 没有版本化的 breaking changes 会破坏消费者。从一开始就为扩展设计。 |
| “没人用那个未记录行为” | Hyrum's Law：如果它可观察，就有人依赖它。把每个 public behavior 都当作承诺。 |
| “我们可以维护两个版本” | 多版本会成倍增加维护成本并制造 diamond dependency 问题。优先遵循 One-Version Rule。 |
| “Internal APIs 不需要 contracts” | 内部消费者仍然是消费者。contracts 防止耦合，并支持并行工作。 |

## Red Flags

- Endpoints 根据条件返回不同 shape
- Endpoints 之间错误格式不一致
- 验证散落在内部代码中，而不是集中在边界处
- 对现有字段做 breaking changes（类型变更、移除）
- List endpoints 没有分页
- REST URLs 中有动词（`/api/createTask`, `/api/getUsers`）
- 未经验证或清理就使用第三方 API 响应

## Verification

设计 API 后：

- [ ] 每个 endpoint 都有 typed input 和 output schemas
- [ ] Error responses 遵循单一一致格式
- [ ] 验证只发生在系统边界处
- [ ] List endpoints 支持分页
- [ ] 新字段是 additive 且 optional（向后兼容）
- [ ] 所有 endpoints 的命名遵循一致约定
- [ ] API documentation 或 types 与实现一起提交