编写质量章程（文件1:QUALITY.md）

质量构成定义了这个特定项目的“质量”意味着什么，并使每个AI会话都明确、持久和继承这个标准。

# #模板```markdown
# Quality Constitution: [Project Name]

## Purpose

[2–3 paragraphs grounding quality in three principles:]

- **Deming** ("quality is built in, not inspected in") — Quality is built into context files
  and the quality playbook so every AI session inherits the same bar.
- **Juran** ("fitness for use") — Define fitness specifically for this project. Not "tests pass"
  but the actual real-world requirement. Example: "generates correct output that survives
  input schema changes without silently producing wrong results."
- **Crosby** ("quality is free") — Building a quality playbook upfront costs less than
  debugging problems found after deployment.

## Coverage Targets

| Subsystem | Target | Why |
|-----------|--------|-----|
| [Most fragile module] | 90–95% | [Real edge case or past bug] |
| [Core logic module] | 85–90% | [Concrete risk] |
| [I/O or integration layer] | 80% | [Explain] |
| [Configuration/utilities] | 75–80% | [Explain] |

The rationale column is essential. It must reference specific risks or past failures.
If you can't explain why a subsystem needs high coverage with a concrete example,
the target is arbitrary.

## Coverage Theater Prevention

[Define what constitutes a fake test for this project.]

Generic examples that apply to most projects:
- Asserting a function returned *something* without checking what
- Testing with synthetic data that lacks the quirks of real data
- Asserting an import succeeded
- Asserting mock returns what the mock was configured to return
- Calling a function and only asserting no exception was thrown

[Add project-specific examples based on what you learned during exploration.
For a data pipeline: "counting output records without checking their values."
For a web app: "checking HTTP 200 without checking the response body."
For a compiler: "checking output compiles without checking behavior."]

## Fitness-to-Purpose Scenarios

[5–10 scenarios. Every scenario must include a `[Req: tier — source]` tag linking it to its requirement source. Use the template below:]

### Scenario N: [Memorable Name]

**Requirement tag:** [Req: formal — Spec §X] *(or `user-confirmed` / `inferred` — see SKILL.md Phase 1, Step 1 for tier definitions)*

**What happened:** [The architectural vulnerability, edge case, or design decision.
Reference actual code — function names, file names, line numbers. Frame as "this architecture permits the following failure mode."]

**The requirement:** [What the code must do to prevent this failure.
Be specific enough that an AI can verify it.]

**How to verify:** [Concrete test or query that would fail if this regressed.
Include exact commands, test names, or assertions.]

---

[Repeat for each scenario]

## AI Session Quality Discipline

1. Read QUALITY.md before starting work.
2. Run the full test suite before marking any task complete.
3. Add tests for new functionality (not just happy path — include edge cases).
4. Update this file if new failure modes are discovered.
5. Output a Quality Compliance Checklist before ending a session.
6. Never remove a fitness-to-purpose scenario. Only add new ones.

## The Human Gate

[List things that require human judgment:]
- Output that "looks right" (requires domain knowledge)
- UX and responsiveness
- Documentation accuracy
- Security review of auth changes
- Backward compatibility decisions
```
##场景从何而来

场景来自两个来源——代码探索和领域知识——最好的场景将两者结合起来。

来源1：防御性代码模式（代码探索）

每一个防御模式都是过去失败或已知风险的证据：

1. **防御代码** -每个`if value is None: return`守卫都是一个场景。为什么需要它？
2. **归一化函数** -每个清理输入的函数都存在，因为原始输入会产生问题
3. **可以硬编码的配置** -如果一个值是从配置中读取的，而不是硬编码，有人知道这个值是不同的
4. **Git责备/提交消息** -“修复X丢失时的崩溃”→场景：X可能丢失
5. **注释解释“为什么”** -“我们使用哈希（id）而不是顺序索引，因为……”→关于该约束下正确性的场景

来源2：可能出错的地方（领域知识）不要把自己限制在代码已经防御的东西上。使用您对类似系统的了解来生成代码应该处理的实际故障场景。对于每个主要子系统，问：

“如果这个进程在运行中被终止了怎么办？”（状态机，文件I/O，批处理）
-“如果外部输入有细微的错误会发生什么？”（验证管道、API集成）
“如果以10倍的规模运行会发生什么？”（批处理、数据库、队列）
“如果两个操作重叠怎么办？”（并发、文件锁、共享状态）
什么东西会产生看起来正确但实际上是错误的输出？（随机性，统计操作，类型强制）这些都不是假设的——它们是发生在这种类型的每个系统中的事情。将它们写成**体系结构漏洞分析**：“由于`save_state()`缺乏原子重命名模式，在10,000条记录的批处理过程中，写入中途崩溃将留下一个损坏的状态文件——下一次运行得到JSONDecodeError，如果没有人工干预就无法恢复。在规模上（64批9240条记录），这种模式可能会无声地丢失1693多条记录，而没有任何标记它们丢失。”具体的数字和具体的后果使情景具有权威性和不可协商性。一个人工智能会话显示“记录可能会丢失”，将会推翻这个标准。AI会话读取具有量化影响的特定故障模式则不会。

###叙述的声音

每个场景的“发生了什么”必须读起来像架构漏洞分析，而不是抽象的规范。包括:-具体数量-“64批次308条记录”而不是“部分记录”
- **级联结果** -“级联通过所有后续管道步骤，需要重新处理4300条记录而不是308条”
- **检测难度** -“没有任何东西会将它们标记为缺失”或“只有统计验证才能捕获它”
- **代码的根本原因** -“`random.seed(index)`创建相关序列，因为顺序整数产生相关的随机流”

叙事的声音有一个关键的目的：它使标准不可协商。抽象的需求（“记录不应该丢失”）需要合理化。具有量化影响的特定故障模式（“在没有检测机制的情况下，批处理中期崩溃会无声地丢失1,693条记录”）则不会。将这些定义为“此架构允许以下故障”——基于实际代码，而不是捏造过去的事件。组合两个源

最强大的场景是将代码中的防御模式与有关其重要性的领域知识结合起来：

1. 找到防御代码：`save_state()`写入临时文件，然后重命名
2. 询问这可以防止什么故障：写中间崩溃留下损坏的状态文件
3. 将场景写成漏洞分析：“如果没有原子重命名模式，在写入过程中崩溃会使state.json完成50%。下一次运行得到JSONDecodeError，如果没有人工干预就无法恢复。”
4. 在代码中：“读取persistence.py行~340：验证临时文件+重命名模式”

“为什么”的要求

每一个覆盖目标，每一个质量关口，每一个标准都必须有一个“为什么”，引用一个特定的场景或风险。如果没有理由，未来的人工智能会议将优化速度并降低标准。坏：“核心逻辑：100%覆盖”
好：“核心逻辑：100%——因为`random.seed(index)`创建了产生77.5%偏差的相关序列，而不是50/50.。微妙的bug在这里产生看似合理但错误的输出。”只有统计验证才能抓住它们。”

“为什么”不是文件，而是防止侵蚀。

校准场景计数每个核心模块（被认为是最复杂或最脆弱的模块）的目标是2+场景。对于一个中等规模的项目，这通常会产生8-10个场景。对于小型项目来说，数量越少越好；更多的是复杂的。如果您发现的场景很少，这通常意味着您的探索不够深入，而不是项目很简单——请回头更仔细地阅读函数体。质量比数量更重要：一个精确捕获体系结构漏洞的场景比三个通用的“如果输入不好怎么办”的场景更有价值。

完成前自我批评

在起草完所有场景后，回顾每一个场景并问：1. **“AI会议是否会推翻这一标准？”**如果是，“为什么”还不够具体。添加数字、结果和检测难度。
2. **“‘发生了什么事’读起来像漏洞分析还是抽象规范？”**如果它读起来像规范，用特定的数量、级联结果和实际代码来重写它。
3. **“有没有我没看到的场景？”**想想不同的AI模型会标记什么。体系结构模型捕获数据流问题。边缘情况模型捕捉边界条件。你对什么视而不见？

##关键规则

每个场景的“如何验证”部分必须映射到功能测试文件中的至少一个自动化测试。如果场景不能自动化，请注意原因（可能需要Human Gate）——但大多数场景应该是可测试的。