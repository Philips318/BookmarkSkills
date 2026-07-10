# Agent Personas

专门扮演单一角色、持有单一视角的专家 persona。每个 persona 都是一个 Markdown 文件，会被你的运行环境（Claude Code、Cursor、Copilot 等）作为 system prompt 使用。

| Persona | Role | Best for |
|---------|------|----------|
| [code-reviewer](../agents/code-reviewer.md) | Senior Staff Engineer | 合并前的五轴评审 |
| [security-auditor](../agents/security-auditor.md) | Security Engineer | 漏洞检测、OWASP 风格审计 |
| [test-engineer](../agents/test-engineer.md) | QA Engineer | 测试策略、覆盖率分析、Prove-It 模式 |
| [web-performance-auditor](../agents/web-performance-auditor.md) | Web Performance Engineer | Core Web Vitals 审计、加载/渲染/网络分析 |

## persona 如何关联 skills 和 commands

三层结构，每层都有清晰职责：

| Layer | What it is | Example | Composition role |
|-------|-----------|---------|------------------|
| **Skill** | 带步骤和退出标准的工作流 | `code-review-and-quality` | *how* — 从 persona 或 command 内部调用 |
| **Persona** | 带视角和输出格式的角色 | `code-reviewer` | *who* — 采用某个观点，产出报告 |
| **Command** | 面向用户的入口点 | `/review`, `/ship` | *when* — 组合 personas 和 skills |

用户（或 slash command）是编排者。**Personas 不调用其他 personas。** Skills 是 persona 工作流中的必经步骤。

## 何时使用哪个

### 直接调用 persona
当你想要针对当前变更获得一个视角，并且用户参与其中时，选择这种方式。

- “Review this PR” → 直接调用 `code-reviewer`
- “Are there security issues in `auth.ts`?” → 直接调用 `security-auditor`
- “What tests are missing for the checkout flow?” → 直接调用 `test-engineer`
- “Audit Core Web Vitals on the product page” → 直接调用 `web-performance-auditor`

### Slash command（背后是单个 persona）
当某个可重复工作流每次都需要重新解释时，选择这种方式。

- `/review` → 用项目的评审 skill 包装 `code-reviewer`
- `/test` → 用 TDD skill 包装 `test-engineer`
- `/webperf` → 包装 `web-performance-auditor`，用于面向性能的 Web 应用审计

### Slash command（编排器 — fan-out）
只有当**独立**调查可以并行运行，并产出由单个 agent 合并的报告时，才选择这种方式。

- `/ship` → 并行 fan out 到 `code-reviewer` + `security-auditor` + `test-engineer`，然后把它们的报告综合成 go/no-go 决策

这是本仓库认可的唯一编排模式。完整模式目录和反模式见 [references/orchestration-patterns.md](../references/orchestration-patterns.md)。

## 决策矩阵

```
Is the work a single perspective on a single artifact?
├── Yes → Direct persona invocation
└── No  → Are the sub-tasks independent (no shared mutable state, no ordering)?
         ├── Yes → Slash command with parallel fan-out (e.g. /ship)
         └── No  → Sequential slash commands run by the user (/spec → /plan → /build → /test → /review)
```

## 示例：有效的编排

`/ship` 是本仓库中规范的 fan-out 编排器：

```
/ship
  ├── (parallel) code-reviewer    → review report
  ├── (parallel) security-auditor → audit report
  └── (parallel) test-engineer    → coverage report
                  ↓
        merge phase (main agent)
                  ↓
        go/no-go decision + rollback plan
```

为什么它有效：
- 每个 sub-agent 都在同一个 diff 上工作，但产出**不同视角**
- 它们彼此没有依赖 → 真正的并行，节省实际等待时间
- 每个都在新的上下文窗口中运行 → 主会话保持清爽
- 合并步骤很小，而且受益于完整上下文，因此留在 main agent 中

## 示例：无效的编排（不要这样构建）

一个 `meta-orchestrator` persona，它的职责是“决定调用哪个其他 persona”：

```
/work-on-pr → meta-orchestrator
                  ↓ (decides "this needs a review")
              code-reviewer
                  ↓ (returns)
              meta-orchestrator (paraphrases result)
                  ↓
              user
```

为什么它会失败：
- 纯路由层，没有领域价值
- 增加两次转述跳转 → 信息损失 + 2× token 成本
- 用户已经知道自己想要评审；让他们直接调用 `/review`
- 重复了 slash commands 和 `AGENTS.md` intent-mapping 已经做的工作

## personas 规则

1. 一个 persona 是单一角色，且只有一种输出格式。如果你发现自己在添加第二个角色，就创建第二个 persona。
2. **Personas 不调用其他 personas。** 组合是 slash commands 或用户的职责。在 Claude Code 上，这也是硬性平台约束 — *"subagents cannot spawn other subagents"* — 因此规则会自动被强制执行。
3. persona 可以调用 skills（*how*）。
4. 每个 persona 文件都以一个 “Composition” 块结尾，说明它适合放在哪里。

## Claude Code 互操作

本仓库中的 personas 设计为无需修改即可作为 Claude Code subagents 和 Agent Teams teammates 使用：

- **作为 subagents：** 启用此 plugin 后会自动发现（无需路径配置）。使用 Agent tool，并传入 `subagent_type: code-reviewer`（或 `security-auditor`、`test-engineer`）。`/ship` 是规范示例。
- **作为 Agent Teams teammates**（实验性，需要 `CLAUDE_CODE_EXPERIMENTAL_AGENT_TEAMS=1`）：spawn teammate 时引用相同的 persona 名称。persona 正文会被**追加到** teammate 的 system prompt，作为附加指令（不是替换），因此你的 persona 文本会叠加在 lead 安装的团队协作指令之上（SendMessage、task-list tools 等）。

Subagents 只把结果报告回 main agent。Agent Teams 允许 teammates 彼此直接发消息。当报告足够时使用 subagents；当 sub-agents 需要互相质疑发现时（例如竞争假设调试）使用 Agent Teams。完整映射见 [references/orchestration-patterns.md](../references/orchestration-patterns.md)。

Plugin agents 不支持 `hooks`、`mcpServers` 或 `permissionMode` frontmatter — 这些字段会被静默忽略。在这里编写新 personas 时，不要依赖它们。

## 添加新的 persona

1. 创建 `agents/<role>.md`，使用与现有 personas 相同的 frontmatter 格式。
2. 定义角色、范围、输出格式和规则。
3. 在底部添加 **Composition** 块（Invoke directly when / Invoke via / Do not invoke from another persona）。
4. 把 persona 添加到本文件顶部的表格中。
5. 如果该 persona 启用了新的编排模式，请在 `references/orchestration-patterns.md` 中记录它，而不是在 persona 文件本身发明该模式。
