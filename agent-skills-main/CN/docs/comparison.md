<!--
  This document is for developers evaluating the project. It is NOT a skill and
  is not meant to be loaded into an agent's context. It lives in docs/ so it
  stays out of the agent's working set.
-->

# agent-skills 如何比较

人们经常问 **agent-skills** 与另外两个流行的“面向 coding agents 的 skills”集合有什么关系：**Superpowers**（Jesse Vincent / obra）和 **Matt Pocock's skills**。三者都很好，拥有很多共同基因，也都值得借鉴。本页诚实地梳理它们在*形态*上的差异，帮助你选择适合自己工作方式的那个，或者从多个项目中借用内容。

> **TL;DR** - 它们优化的是不同场景。**agent-skills** 用 review personas 和反合理化护栏来组织*完整产品生命周期*（Define → Plan → Build → Verify → Review → Ship）。**Superpowers** 偏向使用 subagents 和 worktree 隔离来进行*自主、重推理*的运行。**Matt Pocock's skills** 是一个*锋利、个人化的 Claude Code 工具箱*，提炼自一位专家的日常工作流。抽象地说，没有哪一个是“最好”的，取决于你面前的工作。

---

## 一览

| | **agent-skills** | **Superpowers** | **Matt Pocock's skills** |
|---|---|---|---|
| **Core idea** | 将完整的高级工程生命周期编码为 skills | 建立在可组合 skills 之上的完整开发*方法论* | 一位专家的 `.claude` 工作流开源版 |
| **Organizing principle** | SDLC **阶段**（Define→Plan→Build→Verify→Review→Ship）加 meta-skill router | 有纪律的执行循环（brainstorm → plan → execute） | 聚焦 commands 的精选工具箱 |
| **Lifecycle coverage** | 覆盖广泛 — idea refinement、API/UI design、security、performance、CI/CD、deprecation、ADRs、launch | 深入核心构建循环（TDD、debugging、planning、review） | Planning + build + tooling + knowledge mgmt，带有明确主张 |
| **Entry points** | 与阶段 1:1 映射的 slash commands（`/spec` `/plan` `/build` `/test` `/review` `/code-simplify` `/ship`，外加 `/webperf`） | Commands 如 `/brainstorming`、`/execute-plan` | Slash commands 如 `/tdd`、`/grill-me`、`/diagnose`、`/grill-with-docs` |
| **Tooling reach** | 多工具：Claude Code、Cursor、Gemini CLI、Antigravity、OpenCode、Windsurf、Copilot | 多工具：Claude Code、Codex、Gemini CLI、OpenCode、Cursor、Copilot CLI、Factory Droid | Claude Code 优先（也可用于 Codex） |
| **Distinctive mechanisms** | 每个 skill 中的反合理化表 + Red Flags；带 `/ship` 并行 fan-out 的 review **personas**；参考 checklists | Subagent 驱动开发，带两阶段 review；git-worktree 隔离；skills-that-write-skills | “Grill me” 需求追问；严格 agent 级 TDD；pre-commit/git 护栏 |
| **Best for** | 通过每个阶段推动一个 feature，并在每一步保留人工 checkpoint | 长时间、自主、重推理或探索性工作 | 面向 TypeScript 风格项目的务实、经实战检验的日常循环 |

*（这些项目的采用数字在各类博客中说法差异很大；我们选择不写入它们，而不是重复未经验证的数据。）*

---

## 三个项目，各自的定位

### Superpowers - obra
一个建立在可组合 skills 之上的完整软件开发方法论。它押注于**自主性和前置推理**：写代码前进行苏格拉底式 brainstorming，使用 fresh subagents 执行任务并进行两阶段 review（先验证 spec compliance，再做 code quality），并通过 git worktrees 让并行工作保持隔离。它的 TDD 纪律很严格，会删除过早编写的代码，以守住 RED→GREEN→REFACTOR 这条线。如果你想交出一大块工作，回来时得到经过评审的结果，这就是为那种形态设计的。

**Repo:** <https://github.com/obra/superpowers>

### Matt Pocock's skills - mattpocock
Matt 将他日常使用的实际 `.claude` 目录开源了 — 一组紧凑、聚焦的 Claude Code skills。其中亮点是 `/tdd`（在 agent 层强制 red-green-refactor）和 `/grill-me`（在任何代码前追问你的需求）。它还涵盖 PRD 编写、issue 拆解、interface design、architecture passes、bug triage、pre-commit/git guardrails 和 knowledge management。它是个人化且有主张的，而且这种主张恰到好处：它反映了一位非常优秀的工程师实际交付的方式，而不是试图成为一个包罗万象的框架。

**Repo:** <https://github.com/mattpocock/skills> · related: <https://github.com/mattpocock/agent-rules-books>

### agent-skills - this project
agent-skills 将**整个产品生命周期**组织为 skills，并通过一个 meta-skill（`using-agent-skills`）把任务路由到正确的 skill。每个 skill 都包含 **Common Rationalizations** 表（agent 会用来跳过步骤的借口，以及逐条反驳）和 **Red Flags**。Slash commands 与生命周期阶段一一映射，而 `/ship` 会并行 fan out review **personas** — `code-reviewer`、`security-auditor`、`test-engineer`、`web-performance-auditor` — 然后将它们合并成 go/no-go。它刻意在每个阶段保留人工 checkpoint，并可运行在大多数主流 agent 工具中。

---

## 一次真实对比：Superpowers vs. agent-skills

Om Mishra 做了一次受控实验 — 同一个模型（Sonnet 4.6）、同一个 repo、Claude Code 中同一个 prompt，唯一变化是 skill framework — 并写成了这篇文章：

**["Superpowers vs Agent-Skills: Faster Shipping, Safer Reasoning"](https://www.linkedin.com/pulse/superpowers-vs-agent-skills-faster-shipping-safer-reasoning-om-mishra-dzakf/)** - Om Mishra

公平总结他的发现：

- **agent-skills** 更快进入代码（约 8 分钟 vs 约 12 分钟），并运行了**更多验证 passes**（7 次 vs 5 次，包括完整测试套件）。更广的验证捕获了一个*即时 feature 范围之外*的兼容性问题，而 feature-specific tests 没有发现。对于那项任务，他认为 agent-skills 在**验证深度**上更占优。
- **Superpowers** 投入了更多**前置架构推理**，他仍然更偏爱它作为自己演进生产系统和探索性工作的日常 driver，尤其是在没有既有模式可循时。
- Token efficiency 基本相同；二者都重新规划了一次。

这只是一个开发者的单任务实验，不是 benchmark，但它具体展示了核心权衡：**广泛而有纪律的验证 vs. 更重的前置推理。** 他自己的结论也很诚实：按任务选择工具。

---

## 何时选择哪个

- **选择 agent-skills**：当你想要一个**有引导的生命周期**，每个阶段都有人类 checkpoint，合并前有并行 review/security/perf passes，并且覆盖范围从构建循环延伸到 security、performance、CI/CD 和 launch。它也能跨最多 agent tools 使用。
- **选择 Superpowers**：当你想要**交出长时间的自主工作**并回来看到经过评审的结果，或者工作是探索性/架构性的，并受益于更重的前置推理和 subagent 隔离。
- **选择 Matt Pocock's skills**：当你想要一个**锋利、低仪式感的日常工具箱**，尤其是 requirement-grilling 和严格 TDD loop，用于 TypeScript 风格的 Claude Code 工作流。

你不必只能选一个，但组合时要谨慎。这些是 Markdown skills，不是 runtimes，所以挑选*单个* skills 通常很有效：把 Matt 的 `grill-me`、Superpowers 的 subagent isolation，或某个特定 checklist 拉进你的主设置里。

不奏效的是同时运行其中两个作为你的**活动 router**。堆叠 meta-skills 会争抢 command names（两个地方都定义 `/tdd`）、竞争 routing logic，并引入不同的 TDD 哲学，于是得到的是不可预测行为，而不是各取所长。选择一个 framework 作为 primary router，再从其他项目中按需借用。

---

## Sources

- Superpowers - <https://github.com/obra/superpowers>
- Matt Pocock's skills - <https://github.com/mattpocock/skills>
- Om Mishra, *Superpowers vs Agent-Skills* - <https://www.linkedin.com/pulse/superpowers-vs-agent-skills-faster-shipping-safer-reasoning-om-mishra-dzakf/>

*发现这里对其他项目的描述不准确？请 open an issue or PR - 我们宁愿公平，也不愿只说好听话。*
