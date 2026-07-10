# Orchestration Patterns

本 repo 认可的 agent 编排模式参考目录，以及要避免的反模式。添加会协调多个 personas 的新 slash command 之前，或引入会“包装”现有 personas 的新 persona 之前，请先阅读本文。

治理规则：**用户（或 slash command）是编排者。Personas 不调用其他 personas。** Skills 是 persona 工作流中的必经步骤。

---

## Endorsed patterns

### 1. Direct invocation（无编排）

单一 persona、单一视角、单一 artifact。默认且最便宜的选项。

```
user → code-reviewer → report → user
```

**Use when:** 工作是针对一个 artifact 的一个视角，并且你能用一句话描述它。

**Examples:**
- “Review this PR” → `code-reviewer`
- “Find security issues in `auth.ts`” → `security-auditor`
- “What tests are missing for the checkout flow?” → `test-engineer`

**Cost:** 一次 round trip。你应始终拿它作为比较编排模式的 baseline。

---

### 2. Single-persona slash command

用项目 skills 包装一个 persona 的 slash command。省去用户每次重新解释工作流的成本。

```
/review → code-reviewer (with code-review-and-quality skill) → report
```

**Use when:** 同一个 single-persona invocation 会带着相同设置反复发生。

**Examples in this repo:** `/review`、`/test`、`/code-simplify`。

**Cost:** 与 direct invocation 相同。slash command 只是保存下来的 prompt。

**Anti-signal:** 如果 slash command 的正文大多是“决定调用哪个 persona”，请删除它，让用户直接调用 persona。

---

### 3. Parallel fan-out with merge

多个 personas 并发处理同一输入，每个都产出独立报告。合并步骤（在 main agent 上下文中）把它们综合成单一决策。

```
                    ┌─→ code-reviewer    ─┐
/ship → fan out  ───┼─→ security-auditor ─┤→ merge → go/no-go + rollback
                    └─→ test-engineer    ─┘
```

**Use when:**
- Sub-tasks 真正独立（没有 shared mutable state，没有 ordering dependency）
- 每个 sub-agent 都受益于自己的 context window
- Merge step 小到可以留在 main context
- Wall-clock latency 很重要

**Examples in this repo:** `/ship`。

**Cost:** N 个并行 sub-agent contexts + 一个 merge turn。成本高于 direct invocation，但 wall-clock 更快，并且报告更好，因为每个 sub-agent 都保持聚焦于单一视角。

**采用此模式前的验证 checklist：**
- [ ] 我能在没有 ordering issues 的情况下同时运行所有 sub-agents 吗？
- [ ] 每个 persona 是否产出不同*类型*的发现，而不是从不同角度重复同一个发现？
- [ ] Merge step 是否能放进 main agent 剩余 context？
- [ ] 用户等待时间是否足够长，以至于 parallelism 真能被感知？

如果任一答案是 “no”，回退到 direct invocation 或 single-persona command。

---

### 4. Sequential pipeline as user-driven slash commands

用户按定义顺序运行 slash commands，在步骤之间携带 context（或 commit history）。没有 orchestrator agent — 用户就是 orchestrator。

```
user runs:  /spec  →  /plan  →  /build  →  /test  →  /review  →  /ship
```

**Use when:** 工作流有依赖关系（每一步都需要前一步的输出），并且步骤间的人类判断有价值。

**Examples in this repo:** 完整 DEFINE → PLAN → BUILD → VERIFY → REVIEW → SHIP 生命周期。

**Cost:** 每步一个 sub-agent context。编排层免费，因为没有 orchestrator agent。

**为什么不自动化它：** 一个 LLM “lifecycle orchestrator” 会 (a) 因为需要为 hand-off 做总结而丢失步骤间细微差别，(b) 跳过能早期捕获错误方向的人类 checkpoints，(c) 通过转述 turns 让 token 成本翻倍。

---

### 5. Research isolation（上下文保留）

当任务需要阅读大量材料，而这些材料不应污染 main context 时，spawn 一个 research sub-agent，只返回 digest。

```
main agent → research sub-agent (reads 50 files) → digest → main agent continues
```

**Use when:**
- Main session 需要保持聚焦在后续任务上
- 调查结果远小于其消耗的输入
- Main agent 之后有足够空间思考会提升决策质量

**Examples:** “Find every call site of this deprecated API across the monorepo,” “Summarize what these 30 ADRs say about caching.”

**Cost:** 一个隔离 sub-agent context。只要替代方案是把数百个文件加载到 main context 中，这就值得。

**在 Claude Code 上，使用内置 `Explore` subagent**，而不是定义自定义 research persona。`Explore` 运行在 Haiku 上，被拒绝 write/edit tools，并专为此模式设计。只有当 `Explore` 不合适时（例如需要模型无法自行推断的 domain-specific system prompt），才定义自定义 research subagent。

---

## Claude Code compatibility

此目录与运行环境无关，但大多数读者会在 Claude Code 上运行。下面说明每种模式如何映射到 Claude Code primitives，以及平台在哪里替我们强制执行规则。

### Where personas live

Plugin subagents 放在 plugin 根目录的 `agents/` 中。本 repo 是一个 plugin（`.claude-plugin/plugin.json`），因此启用 plugin 后会自动发现 `agents/code-reviewer.md`、`agents/security-auditor.md` 和 `agents/test-engineer.md`。无需路径配置。

### Subagents vs. Agent Teams

Claude Code 有两种 parallelism primitives。Pattern 3（parallel fan-out with merge）映射到 **subagents**。如果你需要彼此对话的 teammates，请改用 **Agent Teams**。

| | Subagents | Agent Teams |
|--|-----------|-------------|
| Coordination | Main agent fan out，sub-agents 只 report back | Teammates 彼此发消息，共享 task list |
| Context | 每个 subagent 有自己的 context window | 每个 teammate 有自己的 context window |
| When to use | 产出 reports 的独立任务 | 需要讨论的协作工作 |
| Status | 稳定 | 实验性 — 需要 `CLAUDE_CODE_EXPERIMENTAL_AGENT_TEAMS=1` |
| Cost | 较低 | 较高 — 每个 teammate 都是一个单独 Claude instance |

**本 repo 中的 personas 在两种模式下都可工作。** 作为 subagents spawn 时（例如由 `/ship` 调用），它们把 findings 报告给 main session。作为 teammates spawn 时（`Spawn a teammate using the security-auditor agent type…`），它们可以直接互相质疑发现。persona definition 相同；只有 spawning context 不同。

一个细节：persona frontmatter 中的 `skills` 和 `mcpServers` 字段在作为 subagent 运行时会被遵守，但**作为 teammate 运行时会被忽略** — teammates 从你的 project 和 user settings 加载 skills 和 MCP servers，就像普通 session 一样。如果某 persona 依赖特定 skill 或 MCP server，请在 session level 配置，使其在两种模式下都可用。

### Platform-enforced rules

此目录中的两条规则不仅是约定 — Claude Code 会强制它们：

- **“Subagents cannot spawn other subagents”**（文档原文）。Anti-pattern B（persona-calls-persona）和 Anti-pattern D（deep persona trees）在 Claude Code 上从结构上就无法存在。
- **“No nested teams”** — teammates 不能 spawn 自己的 teams。同样的反模式会在 team level 被阻止。

这意味着你可以采用此目录中的模式，而无需担心贡献者意外构建出反模式。它们会直接加载失败。

### Built-in subagents to know about

定义自定义 subagent 前，检查这些内置项是否已覆盖该角色：

| Built-in | Purpose |
|----------|---------|
| `Explore` | 只读 codebase search 和 analysis。用于 Pattern 5（research isolation）。 |
| `Plan` | Plan mode 期间的只读 research。 |
| `general-purpose` | 需要探索和修改的多步骤任务。 |

不要重新定义这些。将你的 specialist personas（code-reviewer、security-auditor、test-engineer）叠加在它们之上。

### Frontmatter restrictions for plugin agents

Plugin subagents **不支持** `hooks`、`mcpServers` 或 `permissionMode` frontmatter fields — 这些会被静默忽略。如果未来某个 persona 需要这些字段，用户必须改为把文件复制到 `.claude/agents/` 或 `~/.claude/agents/`。

在 plugin agents 中**可用**的字段是：`name`、`description`、`tools`、`disallowedTools`、`model`、`maxTurns`、`skills`、`memory`、`background`、`effort`、`isolation`、`color`、`initialPrompt`。如果你想优化成本，可以按 persona 使用 `model`（例如 `test-engineer` coverage scans 用 Haiku，`code-reviewer` 用 Sonnet，`security-auditor` 用 Opus）。

### Spawning multiple subagents in parallel

在 Claude Code 中，parallel fan-out（Pattern 3）需要在**同一个 assistant turn 中发出多个 Agent tool calls**。连续 turns 会串行执行。`/ship` 会明确指出这一点。任何新的 orchestrator command 都应同样说明。

---

## Worked example: Agent Teams for competing-hypothesis debugging

此示例说明何时应使用 **Agent Teams** 而不是 `/ship` 的 subagent fan-out。这两种模式从远处看很像 — 都 spawn 同样三个 personas — 但价值来自不同地方。

### The scenario

> *Checkout occasionally hangs for ~30 seconds before completing. It happens roughly once every 50 sessions. No errors in logs. Started after last week's release.*

可能的 root causes（彼此互斥，但都符合症状）：

1. 新 payment-confirmation flow 中的 race condition
2. 一个 auth check 偶尔落入缓慢的同步 network call
3. 某个 query 缺少 index，且随 cart size 扩展
4. flaky third-party API，SDK 在 timeout 前静默 retry

单个 agent 会选择第一个看起来合理的理论，然后停止调查。`/ship` 风格的 subagent fan-out 会让每个 persona 独立报告 — 但它们的报告不会彼此交汇，因此无法排除错误理论。

这正是 Agent Teams docs 所描述的场景：*“With multiple independent investigators actively trying to disprove each other, the theory that survives is much more likely to be the actual root cause.”*

### 为什么这*不是* `/ship` 的工作

| | `/ship` (subagents) | Agent Teams |
|--|--------------------|-------------|
| Sub-agents see | 同一个 diff，不同 lenses | 共享 task list，彼此消息 |
| Output | 三份独立 reports → 一次 merge | 对抗式辩论 → 共识 root cause |
| Right when | 你想对已知 artifact 得出 verdict | 你想在多个 hypotheses 中*找到* artifact |

`/ship` 是 verdict；Agent Teams 是 investigation。

### Setup（每个环境一次）

Agent Teams 是实验性的。在 `~/.claude/settings.json` 中：

```json
{
  "env": {
    "CLAUDE_CODE_EXPERIMENTAL_AGENT_TEAMS": "1"
  }
}
```

需要 Claude Code v2.1.32 或更高版本。本 repo 中的 personas 会自动被拾取 — 无需手写 team-config files。

### The trigger prompt

在 lead session 中用自然语言输入：

```
Users report checkout hangs for ~30 seconds intermittently after last
week's release. No errors in logs.

Create an agent team to debug this with competing hypotheses. Spawn
three teammates using the existing agent types:

  - code-reviewer  — investigate race conditions and blocking calls
                     in the checkout code path
  - security-auditor — investigate auth checks, session handling,
                       and any synchronous network calls added recently
  - test-engineer  — propose tests that would distinguish between the
                     hypotheses and check coverage gaps in checkout

Have them message each other directly to challenge each other's
theories. Update findings as consensus emerges. Only converge when
two teammates agree they can disprove the others'.
```

lead 会 spawn 三个 teammates，并引用现有 persona names。persona body 会作为附加 instructions **追加**到每个 teammate 的 system prompt（位于 lead 安装的 team-coordination instructions 之上）；上方 trigger prompt 会成为它们的任务。

### What happens

1. 每个 teammate 都在自己的 context window 中运行，从自己的 lens 探索 codebase。
2. Teammates 使用 `message` 直接互相发送 findings。lead 不需要转发。
3. 共享 task list 显示谁在调查什么 — 可随时通过 `Ctrl+T`（in-process mode）或 tmux pane（split mode）查看。
4. 当 `code-reviewer` 发现一个本应 sequential 的 `Promise.all` 时，它会 message `security-auditor` 确认 auth call 不是 race 的一部分。`security-auditor` 检查并回复 — 要么确认 race 是真正问题，要么给出反证。
5. `test-engineer` 为胜出的理论提出 focused integration test，团队用它在声明共识前验证。
6. lead 综合 converged finding 并呈现给你。

你可以通过 `Shift+Down` 切换到任意 teammate 并输入内容来中断 — 对重定向走错路的 investigator 很有用。

### When to clean up

当 investigation 找到 root cause 时，告诉 lead：

```
Clean up the team
```

始终通过 lead 清理，而不是 teammate（根据 docs：teammates 缺少完整 team context，无法清理）。

### Cost expectation

三个 Sonnet teammates 运行约 10–15 分钟 investigation，成本会明显高于 `/ship` 以 subagents 方式 spawn 同样三个 personas。其合理性在于*结论质量* — 对于错误修复代价很高的 production debugging，额外 tokens 很划算。例行 PR review 请坚持使用 `/ship`。

### 此场景中的反模式

不要把它重建为 fan out subagents 的 `/debug` slash command。Subagents 不能彼此消息 — 你会失去让该模式有效的对抗式辩论。如果某工作流经常出现，请把上方 trigger prompt 记录为 snippet，而不是把它包装进一个误用 subagents 的 slash command。

### 何时*不*使用 Agent Teams

- 对已知 diff 的 production-bound verdict → 使用 `/ship`（subagents）。
- 对一个 artifact 的一个专家视角 → direct persona invocation。
- Sequential lifecycle（spec → plan → build）→ user-driven slash commands（Pattern 4）。
- 读取量大但 digest 很小的 research → 内置 `Explore` subagent。

只有当 teammates **需要**相互质疑才能产出正确答案时，才使用 Agent Teams。

---

## Anti-patterns

### A. Router persona（“meta-orchestrator”）

一个职责是决定调用哪个其他 persona 的 persona。

```
/work → router-persona → "this needs a review" → code-reviewer → router (paraphrases) → user
```

**Why it fails:**
- 纯 routing layer，没有 domain value
- 增加两次 paraphrasing hops → 信息损失 + 约 2× token cost
- 用户已经知道自己想要 review；他们可以直接调用 `/review`
- 重复了 slash commands 和 `AGENTS.md` 中 intent mapping 已经做的工作

**What to do instead:** 添加或完善 slash commands。在 `AGENTS.md` 中记录 intent → command mapping。

---

### B. Persona that calls another persona

一个 `code-reviewer` 在看到 auth code 时内部调用 `security-auditor`。

**Why it fails:**
- Personas 被设计为产出单一视角；chain 它们会破坏这一点
- 调用方 persona 传递的 summary 会丢失被调用 persona 所需 context
- Failure modes 成倍增加（哪个 persona 的输出格式胜出？谁的规则适用？）
- 对用户隐藏成本

**What to do instead:** 让调用方 persona 在报告中*建议*后续 audit。用户或 slash command 运行第二轮 pass。

---

### C. Sequential orchestrator that paraphrases

一个 agent 代表用户依次调用 `/spec`、然后 `/plan`、然后 `/build` 等。

**Why it fails:**
- 丢失能捕获错误方向工作的 human checkpoints
- 每次 hand-off 都总结 context — 长 pipeline 中会累积 drift
- Token 成本翻倍：每一步都有 orchestrator turn + sub-agent turn
- 在最需要 judgment 的节点移除了 user agency

**What to do instead:** 保持用户为 orchestrator。在 `README.md` 中记录推荐顺序，让用户调用它。

---

### D. Deep persona trees

`/ship` 调用一个 `pre-ship-coordinator`，它调用 `quality-coordinator`，再调用 `code-reviewer`。

**Why it fails:**
- 每一层都增加 latency 和 tokens，却没有 decision value
- Debugging 变成多层调查
- Leaf personas 在多次 summarization steps 后丢失 context

**What to do instead:** 保持 orchestration depth 最多为 1（slash command → personas）。Merge 发生在 main agent 中。

---

## Decision flow

考虑新的 orchestrated workflow 时，走这个 flow：

```
Is the work one perspective on one artifact?
├── Yes → Direct invocation. Stop.
└── No  → Will the same composition repeat?
         ├── No  → Direct invocation, ad hoc. Stop.
         └── Yes → Are sub-tasks independent?
                  ├── No  → Sequential slash commands run by user (Pattern 4).
                  └── Yes → Parallel fan-out with merge (Pattern 3).
                           Validate against the checklist above.
                           If any check fails → fall back to single-persona command (Pattern 2).
```

---

## 何时向此目录添加新模式

只有在以下条件全部满足后，才添加新条目：

1. 你已经在真实工作中至少使用该模式两次
2. 你能指出本 repo 中展示该模式的具体 artifact
3. 你能解释为什么现有模式不适用
4. 你能描述它的 anti-pattern shadow（人们会错误构建出的东西）

过早的 catalog entries 会变成没人遵循的愿景文档。
