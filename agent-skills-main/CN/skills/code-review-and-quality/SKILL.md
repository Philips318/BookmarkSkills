---
name: code-review-and-quality
description: 进行多轴代码审查。使用场景：合并任何变更前。使用场景：审查你自己、另一个 agent 或人类编写的代码。使用场景：需要在代码进入 main branch 前，从多个维度评估代码质量。
---

# 代码审查与质量

## 概述

带有 quality gates 的多维代码审查。每个变更在合并前都要经过审查，无一例外。审查覆盖五个轴：正确性、可读性、架构、安全性和性能。

**批准标准：** 当一个变更明确改善整体代码健康度时，就批准它，即使它并不完美。完美代码不存在，目标是持续改进。不要因为它不是你会写出的精确形式而阻止变更。如果它改善了代码库并遵循项目约定，就批准它。

## 使用场景

- 合并任何 PR 或变更之前
- 完成功能实现之后
- 当另一个 agent 或模型生成了你需要评估的代码时
- 重构现有代码时
- 任何 bug fix 之后（同时审查修复和 regression test）

## 五轴审查

每次审查都从这些维度评估代码：

### 1. 正确性

代码是否完成了它声称要完成的事情？

- 是否匹配 spec 或任务要求？
- 是否处理了边界情况（null、empty、boundary values）？
- 是否处理了错误路径（不只是 happy path）？
- 是否通过所有测试？这些测试真的测对了内容吗？
- 是否存在 off-by-one errors、race conditions 或 state inconsistencies？

### 2. 可读性与简单性

另一位工程师（或 agent）是否能在作者不解释的情况下理解这段代码？

- 命名是否具有描述性，并与项目约定一致？（没有缺少上下文的 `temp`、`data`、`result`）
- 控制流是否直观（避免 nested ternaries、deep callbacks）？
- 代码组织是否符合逻辑（相关代码归组，module boundaries 清晰）？
- 是否有应简化的“clever” tricks？
- **这件事能不能用更少的行数完成？**（1000 行能用 100 行完成的事，就是失败）
- **抽象是否配得上它的复杂度？**（不到第三个用例，不要泛化）
- 注释是否有助于澄清不明显的意图？（但不要注释显而易见的代码。）
- 是否存在 dead code artifacts：no-op variables（`_unused`）、backwards-compat shims 或 `// removed` comments？
- **新 conditional 是否硬塞进了无关 flow？** 这是设计气味，不是吹毛求疵。把逻辑推入自己的 helper、state 或 policy，而不是缠绕已有路径。
- **同一 shape 上是否反复出现 repeated conditionals？** 它们暗示缺少 model 或 dispatcher。“临时”分支通常会成为永久债务。

### 3. 架构

该变更是否适合系统设计？

- 是否遵循现有模式，还是引入新模式？如果是新模式，是否有理由？
- 是否维护了清晰的 module boundaries？
- 是否有应共享的代码重复？
- 依赖方向是否正确（没有 circular dependencies）？
- 抽象层级是否合适（不过度工程化，也不过度耦合）？
- **这个重构是在降低复杂度，还是只是移动复杂度？** 计算读者为了理解变更必须同时持有的概念数量。如果一个“更干净”的版本让这个数量不变，它就不更干净。优先选择让整个 branches、modes 或 layers 消失的重组，而不是重新集中同一套逻辑。优先删除抽象，而不是打磨抽象。
- **feature-specific logic 是否泄漏到了 shared 或 general-purpose module？** 把逻辑留在拥有该概念的层里，复用现有 canonical helper，而不是写一个近似重复，不要把架构漂移正常化。
- **type boundaries 是否明确？** 质疑无端的 `any`/`unknown`/optional/casts 和掩盖不清晰 invariant 的 silent fallbacks。让边界明确，通常会让周围控制流更简单。

### 4. 安全性

详细安全指导见 `security-and-hardening`。该变更是否引入漏洞？

- 用户输入是否已验证和清理？
- 密钥是否避免出现在代码、日志和版本控制中？
- 需要认证/授权的地方是否检查？
- SQL 查询是否参数化（没有字符串拼接）？
- 输出是否编码以防止 XSS？
- 依赖是否来自可信来源，且没有已知漏洞？
- 是否将外部来源的数据（APIs、logs、user content、config files）视为不可信？
- 外部数据流是否在系统边界处验证后，才用于逻辑或渲染？

### 5. 性能

详细 profiling 和优化见 `performance-optimization`。该变更是否引入性能问题？

- 是否有 N+1 query patterns？
- 是否有 unbounded loops 或 unconstrained data fetching？
- 是否有本应 async 的 synchronous operations？
- UI components 中是否有不必要的 re-renders？
- list endpoints 是否缺少 pagination？
- hot paths 中是否创建了 large objects？

## 结构性修复

当你标记结构性问题时，提出具体动作，而不只是指出问题。只说“这很复杂”的审查会让作者猜。使用命名的重组方式：

- **用 typed model 或 explicit dispatcher** 替换 conditionals 链。
- **折叠重复 branches** 为一个更清晰的 flow。
- **分离 orchestration 与 business logic**，让二者各自可读。
- **将 feature-specific logic** 从 shared module 移到拥有该概念的 package 中。
- **复用 canonical helper**，而不是定制一个近似重复。
- **让 type boundary 明确**，使 downstream branching 消失。
- **删除 pass-through wrapper**，如果它只增加间接性而没有澄清 API。
- **提取 helper，或拆分大文件** 为聚焦模块。

优先选择能移除移动部件的修复，而不是把同样复杂度散布到各处。

## 变更大小

小而聚焦的变更更容易审查、更快合并，也更安全。目标大小：

```
~100 lines changed   → Good. Reviewable in one sitting.
~300 lines changed   → Acceptable if it's a single logical change.
~1000 lines changed  → Too large. Split it.
```

**关注文件大小，而不只是 diff 大小。** 一个小 diff 仍可能把文件推过健康边界。单个文件约 1000 *total* lines（不同于上面的 ~1000 *changed*-lines 阈值）是常见的检查信号，不是硬上限。当一个变更明显增大已经很大的文件时，先问是否应先提取 helpers、subcomponents 或 modules，再继续堆叠。先拆解，再添加。

**什么算“一个变更”：** 一个自包含的修改，解决一件事，包含相关 tests，并在提交后保持系统可用。它是功能的一部分，而不是整个功能。

**变更过大时的拆分策略：**

| Strategy | How | When |
|----------|-----|------|
| **Stack** | 提交一个小变更，再基于它开始下一个 | 顺序依赖 |
| **By file group** | 为需要不同 reviewers 的文件组拆分变更 | Cross-cutting concerns |
| **Horizontal** | 先创建 shared code/stubs，再做 consumers | Layered architecture |
| **Vertical** | 将功能拆成更小的 full-stack slices | Feature work |

**大变更何时可接受：** 完整文件删除，以及 automated refactoring，其中 reviewer 只需验证意图，而不是逐行检查。

**将重构与功能工作分开。** 同时重构现有代码并添加新行为的变更是两个变更，应分开提交。小清理（变量重命名）可由 reviewer 酌情允许。

## 变更描述

每个变更都需要一个能在版本控制历史中独立成立的描述。

**第一行：** 简短、祈使句、独立可读。“Delete the FizzBuzz RPC” 而不是 “Deleting the FizzBuzz RPC”。它必须足够有信息量，让搜索历史的人无需读 diff 就能理解变更。

**正文：** 说明改了什么以及为什么。包含代码本身看不出的上下文、决策和推理。必要时链接 bug numbers、benchmark results 或 design docs。当方法有不足时也要承认。

**反模式：** “Fix bug”、“Fix build”、“Add patch”、“Moving code from A to B”、“Phase 1”、“Add convenience functions”。

## 审查流程

### Step 1: 理解上下文

看代码之前，先理解意图：

```
- What is this change trying to accomplish?
- What spec or task does it implement?
- What is the expected behavior change?
```

### Step 2: 先审查测试

测试揭示意图和覆盖范围：

```
- Do tests exist for the change?
- Do they test behavior (not implementation details)?
- Are edge cases covered?
- Do tests have descriptive names?
- Would the tests catch a regression if the code changed?
```

### Step 3: 审查实现

带着五个轴逐步阅读代码：

```
For each file changed:
1. Correctness: Does this code do what the test says it should?
2. Readability: Can I understand this without help?
3. Architecture: Does this fit the system?
4. Security: Any vulnerabilities?
5. Performance: Any bottlenecks?
```

### Step 4: 分类发现

给每条评论标记严重性，让作者知道哪些是必须处理，哪些是可选：

| Prefix | Meaning | Author Action |
|--------|---------|---------------|
| *(no prefix)* | 必需变更 | 合并前必须处理 |
| **Critical:** | 阻止合并 | 安全漏洞、数据丢失、功能损坏 |
| **Nit:** | 轻微、可选 | 作者可以忽略，格式、风格偏好 |
| **Optional:** / **Consider:** | 建议 | 值得考虑，但非必需 |
| **FYI** | 仅供信息 | 无需行动，未来上下文 |

这可以防止作者把所有反馈都当成强制项，并把时间浪费在可选建议上。

**先讲重要的。** 按杠杆排序 findings：正确性和安全性优先，然后是结构性回退和错过的简化机会，最后才是其他内容。不要把真实问题埋在外观 nit 下面。少量高置信度评论胜过长清单。如果你有一个结构性问题和十个 nits，那么结构性问题就是这次审查。

### Step 5: 验证验证故事

检查作者的 verification story：

```
- What tests were run?
- Did the build pass?
- Was the change tested manually?
- Are there screenshots for UI changes?
- Is there a before/after comparison?
```

## Multi-Model Review Pattern

对不同审查视角使用不同模型：

```
Model A writes the code
    │
    ▼
Model B reviews for correctness and architecture
    │
    ▼
Model A addresses the feedback
    │
    ▼
Human makes the final call
```

这能捕获单个模型可能漏掉的问题。不同模型有不同盲点。

**审查 agent 的示例 prompt：**
```
Review this code change for correctness, security, and adherence to
our project conventions. The spec says [X]. The change should [Y].
Flag any issues as Critical, Required, Optional, or Nit.
```

## Dead Code Hygiene

任何重构或实现变更之后，检查 orphaned code：

1. 识别现在不可达或未使用的代码
2. 明确列出它
3. **删除前先询问：** “Should I remove these now-unused elements: [list]?”

不要留下 dead code，它会迷惑未来读者和 agents。但不要默默删除你不确定的东西。有疑问时，先问。

```
DEAD CODE IDENTIFIED:
- formatLegacyDate() in src/utils/date.ts — replaced by formatDate()
- OldTaskCard component in src/components/ — replaced by TaskCard
- LEGACY_API_URL constant in src/config.ts — no remaining references
→ Safe to remove these?
```

## 审查速度

缓慢审查会阻塞整个团队。切换上下文来审查的成本，小于让其他人等待所施加的成本。

- **一个工作日内响应**，这是最大值，不是目标
- **理想节奏：** 审查请求到达后尽快响应，除非正在深度专注编码。典型变更应能在一天内完成多轮审查
- **优先快速给出单次响应**，而不是快速最终批准。快速反馈能减少挫败，即使需要多轮
- **大变更：** 要求作者拆分，而不是审查一个巨大的 changeset

## 处理分歧

解决审查争议时，应用这个优先级：

1. **技术事实和数据** 高于意见和偏好
2. **Style guides** 是风格问题的绝对权威
3. **Software design** 必须用工程原则评估，而不是个人偏好
4. **Codebase consistency** 在不降低整体健康度时是可接受的

**不要接受“我稍后清理”。** 经验表明，推迟的清理很少发生。除非是真正紧急情况，否则要求提交前清理。如果周边问题无法在这个变更中处理，要求创建一个 bug 并自分配。

## 审查中的诚实

审查代码时，无论代码由你、另一个 agent 还是人类编写：

- **不要 rubber-stamp。** 没有审查证据的 “LGTM” 对任何人都没帮助。
- **不要弱化真实问题。** 如果是会打到生产环境的 bug，却说 “This might be a minor concern”，这是不诚实的。
- **尽可能量化问题。** “This N+1 query will add ~50ms per item in the list” 比 “this could be slow” 更好。
- **对明显有问题的方法提出反对。** Sycophancy 是审查中的失败模式。如果实现有问题，直接说明并提出替代方案。
- **优雅接受 override。** 如果作者掌握完整上下文并不同意，尊重其判断。评论代码，不评论人，把个人批评重构为对代码本身的关注。

## Dependency Discipline

代码审查的一部分是 dependency review：

**添加任何依赖前：**
1. 现有 stack 是否能解决？（通常可以。）
2. 依赖有多大？（检查 bundle impact。）
3. 是否活跃维护？（检查 last commit、open issues。）
4. 是否有已知漏洞？（`npm audit`）
5. license 是什么？（必须与项目兼容。）

**规则：** 优先使用 standard library 和现有 utilities，而不是新依赖。每个依赖都是负担。

## The Review Checklist

```markdown
## Review: [PR/Change title]

### Context
- [ ] I understand what this change does and why

### Correctness
- [ ] Change matches spec/task requirements
- [ ] Edge cases handled
- [ ] Error paths handled
- [ ] Tests cover the change adequately

### Readability
- [ ] Names are clear and consistent
- [ ] Logic is straightforward
- [ ] No unnecessary complexity

### Architecture
- [ ] Follows existing patterns
- [ ] No unnecessary coupling or dependencies
- [ ] Appropriate abstraction level
- [ ] Refactors reduce complexity rather than relocate it
- [ ] No feature logic in shared modules; file stays within a healthy size

### Security
- [ ] No secrets in code
- [ ] Input validated at boundaries
- [ ] No injection vulnerabilities
- [ ] Auth checks in place
- [ ] External data sources treated as untrusted

### Performance
- [ ] No N+1 patterns
- [ ] No unbounded operations
- [ ] Pagination on list endpoints

### Verification
- [ ] Tests pass
- [ ] Build succeeds
- [ ] Manual verification done (if applicable)

### Verdict
- [ ] **Approve** — Ready to merge
- [ ] **Request changes** — Issues must be addressed
```
## See Also

- 详细安全审查指导见 `references/security-checklist.md`
- 性能审查检查项见 `references/performance-checklist.md`

## Common Rationalizations

| Rationalization | Reality |
|---|---|
| “它能工作，就够了” | 可读性差、不安全或架构错误的工作代码会创造复利债务。 |
| “这是我写的，所以我知道它是对的” | 作者会看不见自己的假设。每个变更都能从另一双眼睛中受益。 |
| “我们稍后清理” | 稍后不会到来。审查是 quality gate，用起来。要求合并前清理，而不是之后。 |
| “AI-generated code 应该没问题” | AI code 需要更多审查，而不是更少。它自信且看似合理，即使错误时也是如此。 |
| “测试通过了，所以很好” | 测试是必要但不充分的。它们无法捕获架构问题、安全问题或可读性问题。 |
| “这个重构让它更干净” | 移动复杂度不是降低复杂度。如果读者仍需持有同样数量的概念，结构并没有改善。寻找让 branches 消失的版本。 |
| “这只是对这个文件的小添加” | 小 diff 仍可能把文件推过健康大小，并把 branches 硬接到无关 flows。评判结果结构，而不是 diff 大小。 |

## Red Flags

- PRs 未经任何审查就合并
- 审查只检查测试是否通过（忽略其他轴）
- “LGTM” 没有实际审查证据
- 安全敏感变更没有 security-focused review
- 大 PR “太大而无法正确审查”（拆分它们）
- Bug fix PR 没有 regression tests
- Review comments 没有 severity labels，导致不清楚哪些必需、哪些可选
- 接受 “I'll fix it later”，它不会发生
- 重构只是移动代码，而没有减少读者必须持有的概念数量
- 变更扩大了已经很大的文件，而不是拆解它
- 新 conditionals 散布进无关 code paths（缺少抽象）
- 定制 helper 重复了现有 canonical helper，或 feature logic 被放进 shared module

## Verification

审查完成后：

- [ ] 所有 Critical issues 都已解决
- [ ] 所有 Required（无前缀）变更都已解决，或有理由地明确推迟
- [ ] Tests pass
- [ ] Build succeeds
- [ ] Verification story 已记录（改了什么，如何验证）

**Presumptive blockers:** 针对这些情况提出并建议更简单的设计；只有当变更主动让结构变差时才升级为 Required：重构把复杂度转移而不是减少；变更把文件推过大小边界且没有拆解；feature logic 加入 shared module；近似重复现有 canonical helper；silent fallback 掩盖不清楚的 invariant。