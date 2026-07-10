# Agent Skills

**面向 AI 编码代理的生产级工程技能。**

Skills 编码了高级工程师构建软件时使用的工作流、质量门和最佳实践。这些技能被打包起来，让 AI agents 在开发的每个阶段都能一致地遵循它们。

<a href="https://trendshift.io/repositories/25200" target="_blank"><img src="https://trendshift.io/api/badge/repositories/25200" alt="addyosmani%2Fagent-skills | Trendshift" style="width: 250px; height: 55px;" width="250" height="55"/></a>

![Addy's Agent Skills](https://addyosmani.com/assets/images/addys-agent-skills.jpg)

```
  DEFINE          PLAN           BUILD          VERIFY         REVIEW          SHIP
 ┌──────┐      ┌──────┐      ┌──────┐      ┌──────┐      ┌──────┐      ┌──────┐
 │ Idea │ ───▶ │ Spec │ ───▶ │ Code │ ───▶ │ Test │ ───▶ │  QA  │ ───▶ │  Go  │
 │Refine│      │  PRD │      │ Impl │      │Debug │      │ Gate │      │ Live │
 └──────┘      └──────┘      └──────┘      └──────┘      └──────┘      └──────┘
  /spec          /plan          /build        /test         /review       /ship
```

---

## 命令

8 个映射到开发生命周期的斜杠命令。每个命令都会自动激活合适的技能。

| 你正在做什么 | 命令 | 关键原则 |
|-------------------|---------|---------------|
| 定义要构建什么 | `/spec` | 先写 spec 再写代码 |
| 规划如何构建 | `/plan` | 小型、原子化任务 |
| 增量构建 | `/build` | 一次一个切片 |
| 证明它能工作 | `/test` | 测试即证明 |
| 合并前审查 | `/review` | 改善代码健康度 |
| 审计 Web 性能 | `/webperf` | 优化前先测量 |
| 简化代码 | `/code-simplify` | 清晰优先于聪明 |
| 发布到生产 | `/ship` | 越快越安全 |

spec 已存在后想减少手动步骤？**`/build auto`** 会生成计划，并在一次批准后实现每个任务 — 你只批准一次计划，然后它自主运行。它移除的是任务*之间*的人为步进，而不是验证：每个任务仍然是测试驱动并单独提交，且会在失败或高风险步骤时暂停。

Skills 也会根据你正在做的事自动激活 — 设计 API 会触发 `api-and-interface-design`，构建 UI 会触发 `frontend-ui-engineering`，以此类推。

---

## 快速开始

**最快路径 — 任意 agent，一个命令。** 开放的 [skills CLI](https://github.com/vercel-labs/skills) 可安装到 70+ agents（Claude Code、Cursor、Codex、Copilot、Cline 等）：

```bash
npx skills add addyosmani/agent-skills            # install all 24 skills
npx skills add addyosmani/agent-skills --list     # browse before installing
```

或获取单个技能：

```bash
npx skills add addyosmani/agent-skills --skill code-review-and-quality   # five-axis review before merge
npx skills add addyosmani/agent-skills --skill interview-me              # requirements interrogation, one question at a time
npx skills add addyosmani/agent-skills --skill test-driven-development   # red-green-refactor, enforced
```

更喜欢原生集成？请在下方选择你的工具。

<details>
<summary><b>Claude Code（推荐）</b></summary>

**Marketplace 安装：**

```
/plugin marketplace add addyosmani/agent-skills
/plugin install agent-skills@addy-agent-skills
```

> **SSH errors?** marketplace 通过 SSH 克隆仓库。如果你没有在 GitHub 上设置 SSH keys，请[添加你的 SSH key](https://docs.github.com/en/authentication/connecting-to-github-with-ssh/adding-a-new-ssh-key-to-your-github-account)，或使用完整 HTTPS URL 强制 HTTPS 克隆：
> ```bash
> /plugin marketplace add https://github.com/addyosmani/agent-skills.git
> /plugin install agent-skills@addy-agent-skills
> ```

**本地 / 开发：**

```bash
git clone https://github.com/addyosmani/agent-skills.git
claude --plugin-dir /path/to/agent-skills
```

</details>

<details>
<summary><b>Cursor</b></summary>

将任意 `SKILL.md` 复制到 `.cursor/rules/`，或引用完整的 `skills/` 目录。参见 [docs/cursor-setup.md](docs/cursor-setup.md)。

</details>

<details>
<summary><b>Antigravity CLI</b></summary>

作为原生插件安装，以获得 skills、subagents 和 slash commands。参见 [docs/antigravity-setup.md](docs/antigravity-setup.md)。

**从仓库安装：**

```bash
agy plugin install https://github.com/addyosmani/agent-skills.git
```

**从本地 clone 安装：**

```bash
git clone https://github.com/addyosmani/agent-skills.git
agy plugin install ./agent-skills
```

</details>

<details>
<summary><b>Gemini CLI</b></summary>

作为原生 skills 安装以便自动发现，或添加到 `GEMINI.md` 作为持久上下文。参见 [docs/gemini-cli-setup.md](docs/gemini-cli-setup.md)。

**从仓库安装：**

```bash
gemini skills install https://github.com/addyosmani/agent-skills.git --path skills
```

**从本地 clone 安装：**

```bash
gemini skills install ./agent-skills/skills/
```

</details>

<details>
<summary><b>Windsurf</b></summary>

将技能内容添加到你的 Windsurf rules 配置。参见 [docs/windsurf-setup.md](docs/windsurf-setup.md)。

</details>

<details>
<summary><b>OpenCode</b></summary>

通过 AGENTS.md 和 `skill` 工具使用 agent-driven skill execution。

参见 [docs/opencode-setup.md](docs/opencode-setup.md)。

</details>

<details>
<summary><b>GitHub Copilot</b></summary>

使用 `agents/` 中的 agent definitions 作为 Copilot personas，并在 `.github/copilot-instructions.md` 中使用 skill content。参见 [docs/copilot-setup.md](docs/copilot-setup.md)。

</details>

<details>
  <summary><b>Kiro IDE & CLI </b></summary>
  Kiro 的 Skills 位于 ".kiro/skills/" 下，可存放在 Project 或 Global level。Kiro 也支持 Agents.md。参见 Kiro 文档：https://kiro.dev/docs/skills/
</details>

<details>
<summary><b>Codex</b></summary>

作为原生 Codex 插件安装（Codex CLI v0.122+）：

```bash
codex plugin marketplace add addyosmani/agent-skills
```

Codex 通过 `.codex-plugin/plugin.json` 直接读取根目录 `skills/`。安装后，在聊天中使用 `@` 调用 skills（例如 `@spec-driven-development`）。本地安装和故障排除请参见 [docs/codex-setup.md](docs/codex-setup.md)。

</details>

<details>
<summary><b>其他 Agents</b></summary>

Skills 是纯 Markdown — 它们适用于任何接受 system prompts 或 instruction files 的 agent。参见 [docs/getting-started.md](docs/getting-started.md)。

</details>



---

## 全部 24 个 Skills

上面的 commands 是入口点。这个包总共包含 24 个 skills — 23 个生命周期 skills 加上 `using-agent-skills` meta-skill。每个 skill 都是带步骤、验证门和反合理化表的结构化工作流。你也可以直接引用任意 skill。

### Meta - 发现适用的 skill

| Skill | 它做什么 | 使用时机 |
|-------|-------------|----------|
| [using-agent-skills](skills/using-agent-skills/SKILL.md) | 将传入工作映射到正确的 skill workflow，并定义共享操作规则 | 开始会话或决定哪个 skill 适用时 |

### Define - 明确要构建什么

| Skill | 它做什么 | 使用时机 |
|-------|-------------|----------|
| [interview-me](skills/interview-me/SKILL.md) | 一次一个问题的访谈，提取用户真正想要的内容，而不是他们以为自己应该想要的内容，直到约 95% 置信度 | 请求不够具体，或用户调用 "interview me" / "grill me" 时 |
| [idea-refine](skills/idea-refine/SKILL.md) | 结构化发散/收敛思考，将模糊想法转化为具体提案 | 你有一个需要探索的粗略概念时 |
| [spec-driven-development](skills/spec-driven-development/SKILL.md) | 在任何代码之前，编写覆盖目标、命令、结构、代码风格、测试和边界的 PRD | 开始新项目、功能或重大变更时 |

### Plan - 拆解工作

| Skill | 它做什么 | 使用时机 |
|-------|-------------|----------|
| [planning-and-task-breakdown](skills/planning-and-task-breakdown/SKILL.md) | 将 specs 分解为带验收标准和依赖顺序的小型可验证任务 | 你有 spec 并需要可实现单元时 |

### Build - 编写代码

| Skill | 它做什么 | 使用时机 |
|-------|-------------|----------|
| [incremental-implementation](skills/incremental-implementation/SKILL.md) | 薄垂直切片 - 实现、测试、验证、提交。feature flags、安全默认值、rollback-friendly changes | 任何触及多个文件的更改 |
| [test-driven-development](skills/test-driven-development/SKILL.md) | Red-Green-Refactor、test pyramid（80/15/5）、test sizes、DAMP over DRY、Beyonce Rule、browser testing | 实现逻辑、修复 bugs 或改变行为时 |
| [context-engineering](skills/context-engineering/SKILL.md) | 在正确时间向 agents 提供正确信息 - rules files、context packing、MCP integrations | 开始会话、切换任务，或输出质量下降时 |
| [source-driven-development](skills/source-driven-development/SKILL.md) | 将每个 framework 决策建立在官方文档上 - 验证、引用来源、标记未验证内容 | 你想为任何 framework 或 library 获得权威、带来源引用的代码时 |
| [doubt-driven-development](skills/doubt-driven-development/SKILL.md) | 对进行中的每个非平凡决策进行对抗式 fresh-context review - CLAIM → EXTRACT → DOUBT → RECONCILE → STOP，并可选进行用户授权的 cross-model escalation | 风险很高（生产、安全、不可逆）、在不熟悉代码中工作，或现在验证自信输出比以后调试更便宜时 |
| [frontend-ui-engineering](skills/frontend-ui-engineering/SKILL.md) | 组件架构、设计系统、状态管理、响应式设计、WCAG 2.1 AA accessibility | 构建或修改面向用户的 interfaces 时 |
| [api-and-interface-design](skills/api-and-interface-design/SKILL.md) | Contract-first design、Hyrum's Law、One-Version Rule、error semantics、boundary validation | 设计 APIs、module boundaries 或 public interfaces 时 |

### Verify - 证明它能工作

| Skill | 它做什么 | 使用时机 |
|-------|-------------|----------|
| [browser-testing-with-devtools](skills/browser-testing-with-devtools/SKILL.md) | Chrome DevTools MCP 用于实时 runtime data - DOM inspection、console logs、network traces、performance profiling | 构建或调试任何在浏览器中运行的内容时 |
| [debugging-and-error-recovery](skills/debugging-and-error-recovery/SKILL.md) | 五步 triage：reproduce、localize、reduce、fix、guard。Stop-the-line rule、safe fallbacks | tests fail、builds break，或行为意外时 |

### Review - 合并前质量门

| Skill | 它做什么 | 使用时机 |
|-------|-------------|----------|
| [code-review-and-quality](skills/code-review-and-quality/SKILL.md) | 五轴审查、change sizing（约 100 行）、severity labels（Nit/Optional/FYI）、review speed norms、splitting strategies | 合并任何更改之前 |
| [code-simplification](skills/code-simplification/SKILL.md) | Chesterton's Fence、Rule of 500，在保持精确行为的同时降低复杂度 | 代码能工作，但比应有状态更难阅读或维护时 |
| [security-and-hardening](skills/security-and-hardening/SKILL.md) | OWASP Top 10 prevention、auth patterns、secrets management、dependency auditing、three-tier boundary system | 处理 user input、auth、data storage 或 external integrations 时 |
| [performance-optimization](skills/performance-optimization/SKILL.md) | Measure-first approach - Core Web Vitals targets、profiling workflows、bundle analysis、anti-pattern detection | 存在性能要求，或你怀疑有回归时 |

### Ship - 自信部署

| Skill | 它做什么 | 使用时机 |
|-------|-------------|----------|
| [git-workflow-and-versioning](skills/git-workflow-and-versioning/SKILL.md) | Trunk-based development、atomic commits、change sizing（约 100 行）、commit-as-save-point pattern | 做任何代码更改时（始终） |
| [ci-cd-and-automation](skills/ci-cd-and-automation/SKILL.md) | Shift Left、Faster is Safer、feature flags、quality gate pipelines、failure feedback loops | 设置或修改 build and deploy pipelines 时 |
| [deprecation-and-migration](skills/deprecation-and-migration/SKILL.md) | Code-as-liability mindset、compulsory vs advisory deprecation、migration patterns、zombie code removal | 移除旧系统、迁移用户或下线功能时 |
| [documentation-and-adrs](skills/documentation-and-adrs/SKILL.md) | Architecture Decision Records、API docs、inline documentation standards - 记录 *why* | 做架构决策、改变 APIs 或发布功能时 |
| [observability-and-instrumentation](skills/observability-and-instrumentation/SKILL.md) | Structured logging、RED metrics、OpenTelemetry tracing、symptom-based alerting - 构建时即 instrumentation | 添加 telemetry，或发布任何会在生产中运行的内容时 |
| [shipping-and-launch](skills/shipping-and-launch/SKILL.md) | Pre-launch checklists、feature flag lifecycle、staged rollouts、rollback procedures、monitoring setup | 准备部署到生产时 |

---

## Agent Personas

用于目标审查的预配置专家 personas：

| Agent | Role | Perspective |
|-------|------|-------------|
| [code-reviewer](agents/code-reviewer.md) | Senior Staff Engineer | 按“staff engineer 会批准吗？”标准进行五轴代码审查 |
| [test-engineer](agents/test-engineer.md) | QA Specialist | 测试策略、覆盖分析和 Prove-It pattern |
| [security-auditor](agents/security-auditor.md) | Security Engineer | 漏洞检测、威胁建模、OWASP 评估 |
| [web-performance-auditor](agents/web-performance-auditor.md) | Web Performance Engineer | 使用 Quick/Deep 模式和 metric-honesty rule 进行 Core Web Vitals 审计；通过 `/webperf` 运行 |

请参阅 [docs/agents.md](docs/agents.md) 了解决策矩阵、编排规则，以及 personas 如何与 skills 和 slash commands 组合。

---

## 参考检查清单

skills 在需要时拉取的快速参考资料：

| Reference | 覆盖内容 |
|-----------|--------|
| [definition-of-done.md](references/definition-of-done.md) | 每个更改都要通过的项目级常设标准，并与每任务验收标准形成对照 |
| [testing-patterns.md](references/testing-patterns.md) | 测试结构、命名、mocking、React/API/E2E examples、anti-patterns |
| [security-checklist.md](references/security-checklist.md) | Pre-commit checks、auth、input validation、headers、CORS、OWASP Top 10 |
| [performance-checklist.md](references/performance-checklist.md) | Core Web Vitals targets、frontend/backend checklists、measurement commands |
| [accessibility-checklist.md](references/accessibility-checklist.md) | Keyboard nav、screen readers、visual design、ARIA、testing tools |
| [observability-checklist.md](references/observability-checklist.md) | On-call questions、structured logging、RED/USE metrics、tracing、symptom-based alerting、pre-launch gate |
| [orchestration-patterns.md](references/orchestration-patterns.md) | 被认可的 multi-persona orchestration patterns、anti-patterns，以及 “personas don't invoke personas” 规则 |

---

## Skills 如何工作

每个 skill 都遵循一致结构：

```
┌─────────────────────────────────────────────────┐
│  SKILL.md                                       │
│                                                 │
│  ┌─ Frontmatter ─────────────────────────────┐  │
│  │ name: lowercase-hyphen-name               │  │
│  │ description: Guides agents through [task].│  │
│  │              Use when…                    │  │
│  └───────────────────────────────────────────┘  │                                                                                                
│  Overview         → What this skill does        │
│  When to Use      → Triggering conditions       │
│  Process          → Step-by-step workflow       │
│  Rationalizations → Excuses + rebuttals         │
│  Red Flags        → Signs something's wrong     │
│  Verification     → Evidence requirements       │
└─────────────────────────────────────────────────┘
```

**关键设计选择：**

- **Process, not prose.** Skills 是 agents 要遵循的工作流，而不是供它们阅读的参考文档。每个 skill 都有步骤、检查点和退出标准。
- **Anti-rationalization.** 每个 skill 都包含一张表，列出 agents 用来跳过步骤的常见借口（例如 “I'll add tests later”）及有据可依的反驳。
- **Verification is non-negotiable.** 每个 skill 都以证据要求结束 - tests passing、build output、runtime data。"Seems right" 从来不够。
- **Progressive disclosure.** `SKILL.md` 是入口点。支持性 references 只在需要时加载，以保持 token usage 最小。

---

## 项目结构

```
agent-skills/
├── skills/                            # 24 skills (23 lifecycle + 1 meta)
│   ├── interview-me/                  #   Define
│   ├── idea-refine/                   #   Define
│   ├── spec-driven-development/       #   Define
│   ├── planning-and-task-breakdown/   #   Plan
│   ├── incremental-implementation/    #   Build
│   ├── context-engineering/           #   Build
│   ├── source-driven-development/     #   Build
│   ├── doubt-driven-development/      #   Build
│   ├── frontend-ui-engineering/       #   Build
│   ├── test-driven-development/       #   Build
│   ├── api-and-interface-design/      #   Build
│   ├── browser-testing-with-devtools/ #   Verify
│   ├── debugging-and-error-recovery/  #   Verify
│   ├── code-review-and-quality/       #   Review
│   ├── code-simplification/          #   Review
│   ├── security-and-hardening/        #   Review
│   ├── performance-optimization/      #   Review
│   ├── git-workflow-and-versioning/   #   Ship
│   ├── ci-cd-and-automation/          #   Ship
│   ├── deprecation-and-migration/     #   Ship
│   ├── documentation-and-adrs/        #   Ship
│   ├── observability-and-instrumentation/ # Ship
│   ├── shipping-and-launch/           #   Ship
│   └── using-agent-skills/            #   Meta: how to use this pack
├── agents/                            # 4 specialist personas
├── references/                        # 5 supplementary checklists
├── hooks/                             # Session lifecycle hooks
├── .claude/commands/                  # 8 slash commands (Claude Code)
├── .gemini/commands/                  # 8 slash commands (Gemini CLI)
├── commands/                          # 8 slash commands (Antigravity CLI)
├── plugin.json                        # Antigravity plugin manifest
└── docs/                              # Setup guides per tool
```

---

## 为什么选择 Agent Skills？

AI coding agents 默认走最短路径 - 这通常意味着跳过 specs、tests、security reviews，以及让软件可靠的实践。Agent Skills 为 agents 提供结构化工作流，强制执行高级工程师用于生产代码的同等纪律。

每个 skill 都编码了来之不易的工程判断：*什么时候*写 spec，*测试什么*，*如何*审查，以及*什么时候*发布。这些不是通用 prompts - 而是那种有主张、流程驱动的工作流，它们区分了生产质量工作和原型质量工作。

Skills 内置了来自 Google 工程文化的最佳实践 — 包括 [Software Engineering at Google](https://abseil.io/resources/swe-book) 和 Google 的 [engineering practices guide](https://google.github.io/eng-practices/) 中的概念。你会在 API design 中看到 Hyrum's Law，在 testing 中看到 Beyonce Rule 和 test pyramid，在 code review 中看到 change sizing 和 review speed norms，在 simplification 中看到 Chesterton's Fence，在 git workflow 中看到 trunk-based development，在 CI/CD 中看到 Shift Left 和 feature flags，以及一个把代码视为负债的专用 deprecation skill。这些不是抽象原则 — 它们被直接嵌入 agents 遵循的分步工作流中。