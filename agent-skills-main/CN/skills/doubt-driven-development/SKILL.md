---
name: doubt-driven-development
description: 让每个非平凡决策在成立前接受 fresh-context adversarial review。使用场景：正确性比速度更重要、在不熟悉代码中工作、风险较高（production、security-sensitive logic、irreversible operations），或任何时候，与其稍后调试一个自信输出，不如现在低成本验证它。
---

# 怀疑驱动开发

## 概述

自信的答案不等于正确答案。长 session 会积累 context，并悄悄把假设变成没人注意到的“事实”。Doubt-driven development 是这样一种纪律：在任何非平凡输出成立前，物化一个 fresh-context reviewer，并让它偏向于**证伪**，而不是批准。

这不是 `/review`。`/review` 是对完成产物的判定。这是一种进行中的姿态：非平凡决策要在 course-correction 仍然便宜时接受 cross-examination。

## 使用场景

一个决策在至少满足以下任一条件时是**非平凡**的：

- 它引入或修改 branching logic
- 它跨越 module 或 service boundary
- 它断言了 type system 或 compiler 无法验证的属性（thread safety、idempotence、ordering、invariants）
- 它的正确性依赖未来读者看不到的 context
- 它的 blast radius 不可逆（production deploy、data migration、public API change）

在以下情况应用该 skill：

- 即将在不确定性下做出架构决策
- 即将提交非平凡代码
- 即将声称一个非显然事实（“this is safe”、“this scales”、“this matches the spec”）
- 在你没有完全理解的代码中工作

**不适用场景：**

- 机械操作（renaming、formatting、file moves）
- 遵循清晰、无歧义的用户指令
- 阅读或总结现有代码
- 正确性显而易见的一行变更
- 纯 tooling operations（运行 tests、列出文件）
- 用户明确要求速度优先于验证

如果每个按键都怀疑，你什么也发不出去。该 skill 只适用于上面定义的非平凡决策。

## Loading Constraints

该 skill 为 **main-session orchestrator** 设计，其中下面 Step 3（DOUBT）可以 spawn fresh-context reviewer。

- **不要把这个 skill 添加到 persona 的 `skills:` frontmatter。** 遵循 Step 3 的 persona 会 spawn 另一个 persona，这正是 `references/orchestration-patterns.md` 明确禁止的 orchestration anti-pattern（“personas do not invoke other personas”）。
- **如果你发现自己正在 subagent context 中应用该 skill**（Claude Code 阻止 nested subagent spawn）：首选路径是向用户说明 doubt-driven 无法嵌套运行，让 main session 处理。仅作为最后手段，存在一个降级的自我质疑 fallback：把 ARTIFACT + CONTRACT 重写为 fresh self-prompt，用强硬的心理分隔与此前推理隔开，然后走 Steps 1–5。这**不是 fresh-context review**（你携带自己的 context），所以要标记结果是 degraded，并在用户可达时优先 escalation。

## 流程

应用该 skill 时复制这个 checklist：

```
Doubt cycle:
- [ ] Step 1: CLAIM — wrote the claim + why-it-matters
- [ ] Step 2: EXTRACT — isolated artifact + contract, stripped reasoning
- [ ] Step 3: DOUBT — invoked fresh-context reviewer with adversarial prompt
- [ ] Step 4: RECONCILE — classified every finding against the artifact text
- [ ] Step 5: STOP — met stop condition (trivial findings, 3 cycles, or user override)
```

### Step 1: CLAIM — Surface what stands

用两三行命名这个决策：

```
CLAIM: "The new caching layer is thread-safe under the
        read-heavy workload described in the spec."
WHY THIS MATTERS: a race here corrupts user data and is
                  hard to detect in QA.
```

如果你无法把 claim 写得这么紧凑，你拥有的是感觉，而不是决策。先把它说出来，再审查它。

### Step 2: EXTRACT — Smallest reviewable unit

Fresh-context reviewer 需要的是**artifact** 和 **contract**，而不是过程。

- Code：diff 或 function，而不是整个 file
- Decision：3–5 句 proposal，加上必须满足的 constraints
- Assertion：claim 加上据称支持它的 evidence（与 Step 1 CLAIM block 保持区分，后者是 orchestrator 待审查的 hypothesis）

去掉你的推理。如果你把结论交给 reviewer，得到的会是对结论的验证。这个单元必须足够小，让 reviewer 一次阅读就能 hold in mind。如果它是 500 行 PR，就先分解。

### Step 3: DOUBT — Invoke the fresh-context reviewer

Reviewer 的 prompt **必须是 adversarial**。框架决定答案。

```
Adversarial review. Find what is wrong with this artifact.
Assume the author is overconfident. Look for:
- Unstated assumptions
- Edge cases not handled
- Hidden coupling or shared state
- Ways the contract could be violated
- Existing conventions this might break
- Failure modes under unexpected input

Do NOT validate. Do NOT summarize. Find issues, or state
explicitly that you cannot find any after thorough examination.

ARTIFACT: <paste artifact>
CONTRACT: <paste contract>
```

**只传 ARTIFACT + CONTRACT。不要传 CLAIM。** 把你的结论交给 reviewer 会使它偏向同意。Reviewer 必须独立判断 artifact 是否满足 contract。

在 Claude Code 中，`agents/` 中的 role-based reviewers 默认以 isolated context 启动，可用于这里。参见 `agents/` 中的 roster 和每个 domain 的匹配方式。

**上面的 adversarial prompt 优先于 persona 的默认 response shape。** 像 `code-reviewer` 这样的 personas 被写成会输出带优缺点的平衡 verdict；doubt-driven 需要 issues-only output。逐字粘贴 adversarial prompt 到 invocation 中，让它覆盖 persona 默认行为。如果 persona 的 response shape 无法干净覆盖，则 fallback 到带 adversarial prompt 的 generic subagent。

#### Cross-model escalation

单模型 reviewer 与原作者共享盲点。一个更冷、不同架构的模型能捕获这些盲点。Doubt-driven 已经只在非平凡决策上 opt-in，所以在该范围内提供 cross-model 是该 skill 的价值之一，而不是可选摩擦。

**Interactive sessions: always offer. Never silently skip.**

**Step 1: Ask the user**

完成上面 Step 3 的 single-model review 后，但在 RECONCILE 前，暂停并询问：

> *"Single-model review complete. Want a cross-model second opinion? Options: Gemini CLI, Codex CLI, manual external review (you paste it elsewhere), or skip."*

这个问题在每个 interactive doubt cycle 中都是强制的，即使 artifact 看起来低风险。由用户，而不是 agent，决定成本是否值得。Agent 的职责是呈现选择。

**Step 2: If the user picks a CLI — verify, then invoke**

1. 检查工具是否在 PATH 中（`which gemini`, `which codex`）。
2. 在传入完整 prompt 前测试它是否可用（`gemini --version` 或等价命令）。陈旧或损坏的 binary 可能通过 `which`，但在真实输入时失败。
3. 与用户确认确切 invocation，包括所需 flags、auth 和 env vars（例如 API keys）。实现各不相同，绝不要假设。
4. 只传 ARTIFACT + CONTRACT + adversarial prompt。不要传 session context，不要传 CLAIM。
5. 注意 shell escaping。如果 artifact 包含 quotes、`$(...)` 或 backticks，优先使用 stdin（`echo … | gemini`）或 heredoc，而不是 inline `-p "…"`。不确定时，在运行前请用户确认 invocation。
6. 将输出带入 Step 4（RECONCILE）。

**绝不要把 artifact 插入 shell-quoted argument。** Code、markdown 和 review prompts 经常包含 backticks、`$(...)` 和 quote characters，它们会截断 prompt 或执行嵌入的 shell。将完整 prompt 写入文件，并通过 stdin 管道传入。

示例形态（根据已安装工具验证 flags，语法会因实现和版本而不同）：

```bash
# Write the adversarial prompt + ARTIFACT + CONTRACT to a temp file first.
# Then pipe via stdin so shell metacharacters in the artifact stay inert.

# Codex (read-only sandbox keeps the CLI from writing to your workspace):
codex exec --sandbox read-only -C <repo-path> - < /tmp/doubt-prompt.md

# Gemini ('--approval-mode plan' is read-only; '-p ""' triggers non-interactive
# mode and the prompt is read from stdin):
gemini --approval-mode plan -p "" < /tmp/doubt-prompt.md
```

Read-only sandbox 是关键细节：doubt artifact 本身可能包含指令（有意或无意的 prompt injection），否则 cross-model CLI 可能会对你的 workspace 执行它们。

**Step 3: If the CLI is unavailable or fails**

明确说明失败。提供选项：手动运行、尝试不同工具，或跳过。不要静默 fallback 到 single-model，用户应知道 cross-model 没有发生。

**Step 4: If the user skips**

在输出中确认跳过（*"Proceeding with single-model findings only"*），并继续 RECONCILE。跳过没问题，静默跳过不行。

**Non-interactive contexts**（CI、`/loop`、autonomous-loop、scheduled runs）：

- Cross-model 被**跳过**，且必须在输出中**声明**：*"Cross-model skipped: non-interactive context."*
- **没有明确用户授权时，绝不要调用 external CLI**，这是关键安全属性。

Cross-model 会增加成本、延迟和工具脆弱性。Agent 在每个 cycle 呈现选择；用户决定该 artifact 是否值得。

### Step 4: RECONCILE — Fold findings back

Reviewer 的输出是数据，不是 verdict。**你仍然是 orchestrator。** 根据每个 finding 重新阅读 artifact text，再分类。Rubber-stamping reviewer 与忽略 reviewer 是同一种失败模式。

对每个 finding，按这个**优先级顺序**分类（第一个匹配类别获胜）：

1. **Contract misread**：reviewer 标记某事，是因为你提供的 CONTRACT 不清楚或不完整。先修 contract，下一个 cycle 重新分类。
2. **Valid + actionable**：真实问题，需要修改 artifact。修改后重新循环。
3. **Valid trade-off**：问题真实，但修复成本高于接受成本。明确记录 trade-off，让用户看见它。
4. **Noise**：reviewer 标记了在其缺失 context 下看似问题、但实际正确的内容。记下它，继续，并追问：把那个 context 加入 contract 是否能避免这个 false flag？

Fresh reviewer 可能因为缺少 context 而出错。不要只因为它“fresh”就服从。

### Step 5: STOP — Bounded loop, not recursion

满足以下条件时停止：

- 下一轮只返回 trivial 或 already-considered findings，**或**
- 已完成 3 个 cycles（升级给用户，不要独自磨第四轮），**或**
- 用户明确说 “ship it”

如果 3 个 cycles 后 reviewer 仍然提出实质性问题，artifact 可能还没准备好。把这个信息呈现给用户。三轮未解决本身就是关于 artifact 的信息，而不是继续循环的理由。

如果因为 artifact 很大而“三轮显然不够”：artifact 太大，回到 Step 2 分解。不要提高上限。

## Common Rationalizations

| Rationalization | Reality |
|---|---|
| “我很有信心，跳过 doubt step” | 在新问题上，信心与正确性相关性很弱。确定感最强时，盲点最容易隐藏。 |
| “Spawn reviewer 太贵” | 调试生产环境中的错误 commit 更贵。这个检查有边界，bug 没有。 |
| “Reviewer 只会挑剔” | 只有在无范围时才会如此。把 prompt 限制为“会让 artifact 在 contract 下失败的问题”。 |
| “我会在最后用 `/review` 做 doubt” | `/review` 是最终 gate。Doubt-driven 在 course-correction 便宜时捕获错误方向。到 PR 阶段就太晚了。 |
| “如果每一步都怀疑，我永远发不出去” | 该 skill 只适用于非平凡决策，不是每个按键。重读 “When NOT to Use”。 |
| “两个意见总比一个好” | 当第二个 context 更少且产生噪音时，并非如此。Reconcile，不要 defer。 |
| “Reviewer 不同意，所以我错了” | Reviewer 缺少你的 context。分歧是信息，不是 verdict。重读 artifact、分类，然后决定。 |
| “Cross-model 总是更好” | Cross-model 能捕获单模型与自身共享的盲点，但会增加成本和工具脆弱性。在每个 interactive doubt cycle 中都要提供选择，由用户决定 artifact 是否值得。Agent 的职责是呈现选择，而不是 gate。 |
| “用户说过一次 yes，所以我可以继续调用 CLI” | 每次 invocation 都需要单独授权。Artifact、prompt 和 flags 会在调用之间变化。每次运行前重新确认确切命令。 |

## Red Flags

- 为一行 rename 或 formatting change spawn fresh-context reviewer
- 未重新阅读 artifact text 就把 reviewer output 当作权威
- 循环 >3 cycles 却未升级给用户
- 用 “is this good?” 而不是 “find issues” 提示 reviewer
- 在高风险决策上因为时间压力跳过 doubt
- 对未改变 artifact 反复 spawn fresh-context（你会得到同样 findings，这是在拖延）
- **Doubt theater（可检查信号）**：在 2 个或更多 cycles 中，reviewer 提出了实质性 findings，但 0 个 findings 被分类为 actionable。你是在验证，不是在怀疑。停下并升级。
- 提交之后才 doubt，那是 `/review`，不是 doubt-driven development
- 未与用户确认工具存在、已配置且接受该语法，就 hardcode external CLI invocation
- **在 interactive doubt cycle 中静默跳过 cross-model。** 即使不推荐，也必须让 offer 可见。跳过可以，静默跳过不行。
- External CLI 报错或缺失时静默 fallback，必须说明失败并让用户重定向
- 从 reviewer input 中剥离 contract
- 将 CLAIM 传给 reviewer（会偏向同意）

## Interaction with Other Skills

- **`code-review-and-quality` / `/review`**：互补。`/review` 是事后 PR verdict；doubt-driven 是进行中的 per-decision。
- **`source-driven-development`**：SDD 根据官方文档验证 *facts about frameworks*。Doubt-driven 验证 *your reasoning about the artifact*。SDD 检查 API 存在；doubt-driven 检查你是否在 contract 下正确使用它。
- **`test-driven-development`**：TDD 的 RED step 是具体化的怀疑。一个 failing test 就是一次 disproof attempt。当 TDD 适用时，该 failing test 就满足 behavioral claims 的 doubt step。
- **`debugging-and-error-recovery`**：当 reviewer 提出真实 failure mode 时，进入 debugging skill 来 localize 并修复。
- **Repo orchestration rules**（`references/orchestration-patterns.md`）：该 skill 从 main session 编排。Persona 调用另一个 persona 是 anti-pattern B，见上面的 Loading Constraints。

## Verification

应用 doubt-driven development 后：

- [ ] 每个非平凡决策（按上面定义）在成立前都明确命名为 CLAIM
- [ ] 每个非平凡 artifact 至少有一次 fresh-context review（TDD RED step 产生的 failing test 可满足 behavioral claims 的这一点，见 Interaction with Other Skills）
- [ ] Reviewer 收到 ARTIFACT + CONTRACT，而不是 CLAIM，也不是你的推理
- [ ] Reviewer prompt 是 adversarial（“find issues”），不是 validating（“is it good”）
- [ ] Findings 已依据 artifact text 分类（不是 rubber-stamped），使用优先级：contract misread / actionable / trade-off / noise
- [ ] 满足 stop condition（trivial findings、3 cycles 或 user override）
- [ ] 在 interactive mode 中，已**明确向用户提供** cross-model（无论 artifact 风险如何），并在输出中确认其响应
- [ ] 在 non-interactive mode 中，cross-model 已跳过，且已声明跳过
- [ ] 任何 external CLI invocation 都已先进行 PATH check、working-binary test、与用户确认语法，并获得明确运行授权