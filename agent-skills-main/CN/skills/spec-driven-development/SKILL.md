---
name: spec-driven-development
description: 在编码前创建 specs。用于启动新项目、功能或重大变更且尚无 specification 时。用于需求不清晰、模糊，或只以粗略想法存在时。
---

# Spec 驱动开发

## 概述

在编写任何代码前，先写结构化 specification。Spec 是你和人类工程师之间共享的事实来源：它定义我们要构建什么、为什么构建，以及如何知道完成了。没有 spec 的代码就是猜测。

## 何时使用

- 启动新项目或功能
- 需求模糊或不完整
- 变更触及多个文件或模块
- 即将做架构决策
- 任务实现预计超过 30 分钟

**何时不使用：** 单行修复、typo 更正，或需求明确且自包含的变更。

## 有关卡的工作流

Spec-driven development 有四个阶段。在当前阶段获得验证前，不要进入下一阶段。

```
SPECIFY ──→ PLAN ──→ TASKS ──→ IMPLEMENT
   │          │        │          │
   ▼          ▼        ▼          ▼
 Human      Human    Human      Human
 reviews    reviews  reviews    reviews
```

### 阶段 1：Specify

从高层愿景开始。向人类提出澄清问题，直到需求具体。

**立即暴露假设。** 在写任何 spec 内容前，列出你的假设：

```
ASSUMPTIONS I'M MAKING:
1. This is a web application (not native mobile)
2. Authentication uses session-based cookies (not JWT)
3. The database is PostgreSQL (based on existing Prisma schema)
4. We're targeting modern browsers only (no IE11)
→ Correct me now or I'll proceed with these.
```

不要默默填补模糊需求。Spec 的全部目的，是在代码编写*之前*暴露误解。假设是最危险的误解形式。

**编写覆盖六个核心领域的 spec 文档：**

1. **Objective**：我们在构建什么，为什么？用户是谁？成功是什么样？

2. **Commands**：带 flags 的完整可执行命令，不只是工具名。
   ```
   Build: npm run build
   Test: npm test -- --coverage
   Lint: npm run lint --fix
   Dev: npm run dev
   ```

3. **Project Structure**：源码在哪里，测试放哪里，文档属于哪里。
   ```
   src/           → Application source code
   src/components → React components
   src/lib        → Shared utilities
   tests/         → Unit and integration tests
   e2e/           → End-to-end tests
   docs/          → Documentation
   ```

4. **Code Style**：一个真实代码片段比三段描述风格的文字更有用。包含命名约定、格式规则和好输出示例。

5. **Testing Strategy**：使用什么框架、测试放哪里、覆盖率期望、哪些关注点用哪些测试层级。

6. **Boundaries**：三层系统：
   - **Always do：** 提交前运行测试、遵循命名约定、验证输入
   - **Ask first：** 数据库 schema 变更、添加依赖、修改 CI config
   - **Never do：** 提交 secrets、编辑 vendor directories、未经批准移除失败测试

**Spec 模板：**

```markdown
# Spec: [Project/Feature Name]

## Objective
[What we're building and why. User stories or acceptance criteria.]

## Tech Stack
[Framework, language, key dependencies with versions]

## Commands
[Build, test, lint, dev — full commands]

## Project Structure
[Directory layout with descriptions]

## Code Style
[Example snippet + key conventions]

## Testing Strategy
[Framework, test locations, coverage requirements, test levels]

## Boundaries
- Always: [...]
- Ask first: [...]
- Never: [...]

## Success Criteria
[How we'll know this is done — specific, testable conditions]

## Open Questions
[Anything unresolved that needs human input]
```

**把指令重构为成功标准。** 收到模糊需求时，把它们翻译成具体条件：

```
REQUIREMENT: "Make the dashboard faster"

REFRAMED SUCCESS CRITERIA:
- Dashboard LCP < 2.5s on 4G connection
- Initial data load completes in < 500ms
- No layout shift during load (CLS < 0.1)
→ Are these the right targets?
```

这让你能围绕清晰目标循环、重试和解决问题，而不是猜测“更快”是什么意思。

### 阶段 2：Plan

有了已验证 spec 后，生成技术实现计划：

1. 识别主要组件及其依赖
2. 确定实现顺序（必须先构建什么）
3. 记录风险和缓解策略
4. 识别什么可并行，什么必须顺序执行
5. 定义阶段之间的验证检查点

> 遵循 `planning-and-task-breakdown` 来处理依赖图映射和垂直切片机制；它是权威来源。上面的 bullets 是轻量摘要；如果两者发生偏离，以 `planning-and-task-breakdown` 为准。
>
> **输出约定：** 按 `/plan` 命令约定，将计划保存到 `tasks/plan.md`，任务清单保存到 `tasks/todo.md`。如果 `tasks/` 不存在，请创建它。下游命令（`/build` 等）期望这些路径。

计划应该可审查：人类应该能读完后说“是，这就是正确方法”或“不，改 X”。

### 阶段 3：Tasks

把计划拆成离散、可实现的任务：

- 每个任务都应能在一次聚焦会话中完成
- 每个任务都有明确验收标准
- 每个任务都包含验证步骤（测试、构建、手动检查）
- 任务按依赖排序，而不是按感知重要性排序
- 没有任务应需要修改超过约 5 个文件

> 遵循 `planning-and-task-breakdown` 获取完整任务大小和依赖排序机制；它是权威来源。下面模板是轻量内联形式；如果两者发生偏离，以 `planning-and-task-breakdown` 为准。

**任务模板：**
```markdown
- [ ] Task: [Description]
  - Acceptance: [What must be true when done]
  - Verify: [How to confirm — test command, build, manual check]
  - Files: [Which files will be touched]
```

### 阶段 4：Implement

一次执行一个任务，遵循 `skills/incremental-implementation/SKILL.md`（`incremental-implementation`）和 `skills/test-driven-development/SKILL.md`（`test-driven-development`）。使用 `skills/context-engineering/SKILL.md`（`context-engineering`）在每一步加载正确的 spec sections 和 source files，而不是把整个 spec 都塞给 agent。

## 保持 Spec 活着

Spec 是活文档，不是一次性 artifact：

- **决策变化时更新**：如果你发现数据模型需要变化，先更新 spec，再实现。
- **范围变化时更新**：新增或删减的功能应反映在 spec 中。
- **提交 spec**：spec 应与代码一起进入版本控制。
- **在 PR 中引用 spec**：链接回每个 PR 实现的 spec section。

## 常见合理化借口

| 合理化借口 | 现实 |
|---|---|
| “这很简单，不需要 spec” | 简单任务不需要*长* spec，但仍需要验收标准。两行 spec 也可以。 |
| “我写完代码再写 spec” | 那是文档，不是 specification。Spec 的价值在于代码前迫使清晰。 |
| “Spec 会拖慢我们” | 15 分钟 spec 能避免数小时返工。15 分钟 waterfall 胜过 15 小时 debug。 |
| “需求反正会变” | 这就是 spec 是活文档的原因。过时 spec 仍好过没有 spec。 |
| “用户知道自己想要什么” | 即使清晰请求也有隐含假设。Spec 会暴露这些假设。 |

## 危险信号

- 没有任何书面需求就开始写代码
- 在澄清“done”是什么前问“should I just start building?”
- 实现任何 spec 或任务清单中没有提到的功能
- 做架构决策却不记录
- 因为“要构建什么很明显”而跳过 spec

## 验证

进入实现前，确认：

- [ ] Spec 覆盖全部六个核心领域
- [ ] 人类已审查并批准 spec
- [ ] 成功标准具体且可测试
- [ ] Boundaries（Always/Ask First/Never）已定义
- [ ] Spec 已保存到 repository 中的文件
