#审查方案（文件3和4）

##文件3：代码审查协议（`RUN_CODE_REVIEW.md`）

# # #模板```markdown
# Code Review Protocol: [Project Name]

## Bootstrap (Read First)

Before reviewing, read these files for context:
1. `quality/QUALITY.md` — Quality constitution and fitness-to-purpose scenarios
2. `quality/REQUIREMENTS.md` — Testable requirements derived during playbook generation
3. [Main architectural doc]
4. [Key design decisions doc]
5. [Any other essential context]

## Pass 1: Structural Review

Read the code and report anything that looks wrong. No requirements, no focus areas — use your own knowledge of code correctness. Look for: race conditions, null pointer hazards, resource leaks, off-by-one errors, type mismatches, error handling gaps, and any code that looks suspicious.

### Guardrails

- **Line numbers are mandatory.** If you cannot cite a specific line, do not include the finding.
- **Read function bodies, not just signatures.** Don't assume a function works correctly based on its name.
- **If unsure whether something is a bug or intentional**, flag it as a QUESTION rather than a BUG.
- **Grep before claiming missing.** If you think a feature is absent, search the codebase. If found in a different file, that's a location defect, not a missing feature.
- **Do NOT suggest style changes, refactors, or improvements.** Only flag things that are incorrect or could cause failures.

### Output

For each file reviewed:

#### filename.ext
- **Line NNN:** [BUG / QUESTION / INCOMPLETE] Description. Expected vs. actual. Why it matters.

## Pass 2: Requirement Verification

Read `quality/REQUIREMENTS.md`. For each requirement, check whether the code satisfies it. This is a pure verification pass — your only job is "does the code satisfy this requirement?"

Do NOT also do a general code review. Do NOT look for other bugs. Do NOT evaluate code quality. Just check each requirement.

For each requirement, report one of:
- **SATISFIED**: The code implements this requirement. Quote the specific code.
- **VIOLATED**: The code does NOT satisfy this requirement. Explain what the code does vs. what the requirement says. Quote the code.
- **PARTIALLY SATISFIED**: Some aspects implemented, others missing. Explain both.
- **NOT ASSESSABLE**: Can't be checked from the files under review.

### Output

For each requirement:

#### REQ-N: [requirement text]
**Status**: SATISFIED / VIOLATED / PARTIALLY SATISFIED / NOT ASSESSABLE
**Evidence**: [file:line] — [code quote]
**Analysis**: [explanation]
[If VIOLATED] **Severity**: [impact description]

## Pass 3: Cross-Requirement Consistency

Compare pairs of requirements from `quality/REQUIREMENTS.md` that reference the same field, constant, range, or security policy. For each pair, check whether their constraints are mutually consistent.

What to look for:
- **Numeric range vs bit width**: If one requirement says the valid range is [0, N) and another says the field is M bits wide, does N = 2^M?
- **Security policy propagation**: If one requirement says a CA file is configured, do all requirements about connections that should use it actually reference using it?
- **Validation bounds vs encoding limits**: Does a validation check in one file agree with the storage capacity in another?
- **Lifecycle consistency**: If a resource is created by one requirement's code, is it cleaned up by another's?

For each pair that shares a concept, verify consistency against the actual code.

### Output

For each shared concept:

#### Shared Concept: [name]
**Requirements**: REQ-X, REQ-Y
**What REQ-X claims**: [summary]
**What REQ-Y claims**: [summary]
**Consistency**: CONSISTENT / INCONSISTENT
**Code evidence**: [quotes from both locations]
**Analysis**: [explanation]
[If INCONSISTENT] **Impact**: [what happens when the contradiction is triggered]

## Combined Summary

| Source | Finding | Severity | Status |
|--------|---------|----------|--------|
| Pass 1 | [structural finding] | [severity] | BUG / QUESTION |
| Pass 2, REQ-N | [requirement violation] | [severity] | VIOLATED |
| Pass 3, REQ-X vs REQ-Y | [consistency issue] | [severity] | INCONSISTENT |

- Total findings by pass and severity
- Overall assessment: SHIP / FIX BEFORE MERGE / BLOCK
```
执行要求

**所有三个通行证都是强制性的。**不要将考试合并为一次审查。由于使用了不同的镜头，每一遍都会产生不同的结果：

- **通过1**发现结构性错误（竞争条件，null危险，资源泄漏）
**通过2**发现需求违反（缺失行为，规范偏差）
- **通过3**发现跨需求矛盾（不一致的范围，冲突的保证）

**在输出文件中将每个通道写入一个明确标记的部分**。使用标题`## Pass 1: Structural Review`、`## Pass 2: Requirement Verification`、`## Pass 3: Cross-Requirement Consistency`和`## Combined Summary`。**如果通过没有发现，解释原因。**不要只写“没有发现”。写下你检查的内容以及为什么没有错。例如：“审查了lib/response.js中的12个函数，以查找null危险、资源泄漏和错误处理差距。没有确认的bug——所有的错误路径要么抛出要么返回定义良好的默认值。”没有发现也没有解释的传球是没有完成的传球。

**大型代码库的范围：**如果项目有超过50个需求，步骤2不需要针对每个文件验证每个需求。相反，将第2步的重点放在与被评审的文件最相关的需求上——检查引用那些文件的需求或者管理那些文件实现的行为域的需求。目标是审查文件的深度，而不是所有需求的广度。**完成前自检：**写完三通及综合总结后，验证：(1)所有三个通过部分都存在于输出中，(2)通过2引用带有SATISFIED/VIOLATED结论的特定REQ-NNN数字，(3)通过3确定需求之间至少有一个共享概念（即使一致），(4)每个BUG发现在`quality/test_regression.*`中都有相应的回归测试（参见下面的阶段2），(5)每个回归测试练习在发现中引用的实际代码路径（参见下面的测试-查找对齐检查）。如果任何检查失败，返回并完成缺失的工作。

当文档可用时，采取对抗立场如果剧本是与补充文档（reference_docs/，社区文档，用户指南，API参考）一起生成的，代码审查必须使用该文档来“对抗”代码，而不是为其辩护。文档告诉您代码应该做什么。你的工作是找到它不存在的地方。

**不要让文档解释成为代码缺陷的借口。**如果文档说“库优雅地处理X”，但代码没有检查X，那就是一个bug——文档使它“更”是个bug，而不是更少。对意图更深入的理解应该会让你对代码更“强硬”，而不是更“软弱”。这里提到的失败模式是：当模型可以访问文档时，它们会构建一个更丰富的软件心理模型，并且对与该模型近似匹配的代码变得更“宽容”。文档使模型有理由相信代码可以工作，从而抑制了检测。通过将文件视为控方的证据来解决这个问题——它定义了代码所承诺的内容，而你的工作就是找到未兑现的承诺。

###测试查找对齐检查

对于每个声称重现特定发现的回归测试，验证测试实际上执行了引用的代码路径。针对不同的功能、不同的分支或不同的故障模式而不是它声称要重现的发现的测试比没有测试更糟糕——它会产生错误的信心。**验证程序：**每个回归测试：
1. 读取结果：注意特定的文件、行号、函数和故障条件
2. 阅读测试：确定它调用了哪个函数以及它断言了什么条件
3. 确认对齐：测试必须调用查找结果中引用的函数，触发查找结果所描述的特定条件，并断言查找结果认为是错误的行为

如果测试没有执行引用的代码路径，要么修复测试，要么将发现标记为UNCONFIRMED。不要发布由于与发现无关的原因而通过或失败的回归测试。

###关闭命令每个确认的BUG发现都必须在`quality/test_regression.*`中产生一个回归测试。测试必须是用项目语言编写的可执行源文件——不是Markdown文件，不是散文文档，不是描述测试将做什么的注释块。如果项目使用Java，则编写`.java`文件。如果是Python，则使用`.py`文件。测试必须可以编译（或解析），并且可以由项目的测试框架运行。

**没有语言豁免。**如果担心在修复之前引入失败测试，请使用该语言的预期失败机制。该保护必须是框架**最早的语法保护——习惯的修饰符或注释，否则是测试体中的第一个可执行行：**Python (pytest):**`@pytest.mark.xfail(strict=True, reason="BUG-NNN: [description]")`-`def test_...():`之上的装饰器。当出现错误时：XFAIL（预期）。当错误被修复但标记未被移除：XPASS→严格模式失败时，应该移除信号保护。
**Python (unittest):**`@unittest.expectedFailure`-测试方法上面的装饰器。
- **Go:**`t.Skip("BUG-NNN: [description] — unskip after applying quality/patches/BUG-NNN-fix.patch")`-测试函数内的第一行。注意：Go的`t.Skip`隐藏了测试（报告SKIP，而不是FAIL），它比Python的xfail弱。
**Rust:**`#[ignore]`属性的测试函数-标准的“不运行在默认套件”机制。`#[should_panic]`仅用于恐慌型bug。
**Java (JUnit 5):**`@Disabled("BUG-NNN: [description]")`上面标注的测试方法。
**TypeScript/JavaScript（开玩笑）：**`test.failing("BUG-NNN: [description]", () => { ... })`- **TypeScript/JavaScript(Vitest):**`test.fails("BUG-NNN: [description]", () => { ... })`**JavaScript (Mocha):**`it.skip("BUG-NNN: [description]", () => { ... })`或`this.skip()`在测试体中的条件跳过。每个守卫都必须引用错误ID （bug - nnn格式）和修复补丁路径，以便遇到跳过测试的人知道如何解决它。

这些模式确保每个bug都有一个可执行的测试，可以在修复完成时启用，而不会用预期的失败污染CI。**TDDred/green与skip guard交互。**在TDD验证期间，红色和绿色相位必须暂时绕过跳过保护：
- **红色阶段（从未跳过）：**删除或禁用防护，运行未打补丁的代码。必须失败。记录结果后重新启用保护。**红色阶段是强制性的每一个确认的错误，即使没有修复补丁存在。**记录`verdict: "confirmed open"`与`red_phase: "fail"`和`green_phase: "skipped"`。不要使用`verdict: "skipped"`-该值已弃用。
- **绿色阶段：**删除或禁用保护，应用修复补丁，运行。必须通过。如果修复将恢复，则重新启用保护。如果没有修复补丁，记录“`green_phase: "skipped"`”。
- **成功后红色→绿色：**在`quality/writeups/BUG-NNN.md`生成每个Bug的writeup（参见SKILL.md文件7，“Bug writeup generation”）。记录“`tdd-results.json`”中的路径为“`writeup_path`”。在编写`tdd-results.json`之后，重新打开它并验证所有必需的字段、枚举值和没有额外的未记录的根键参见SKILL.md写后验证步骤)。两个sidecar JSON文件必须使用`schema_version: "1.1"`。
- **在TDD周期后：** Guard保留在提交的回归测试文件中，只有当修复被永久合并时才会删除。唯一可以接受的例外情况是当回归测试确实无法编写时——例如，bug需要多线程计时，但无法可靠地重现，或者需要测试环境中不可用的外部服务。在这种情况下，在合并摘要中写一个明确的豁免说明，解释为什么，并包括一个最小的代码草图，显示如果可能的话，您将测试什么。

没有可执行的回归测试或明确的豁免说明的结果是不完整的。合并的摘要不能包括未解决的发现——每个BUG都必须有闭包。

回归测试语义约定

所有回归测试必须断言期望的正确行为，并在当前代码上标记为预期失败。不要编写断言当前故障行为并通过的测试。区别很重要：- **正确：**测试说“这个输入应该产生X”→测试失败，因为有bug的代码产生Y→标记`xfail`/`@Disabled`/`t.Skip`→当bug被修复，测试通过，跳过标记被删除。
- **错误：**测试说“这个输入产生Y（有bug的输出）”→测试通过有bug的代码→当bug修复后，测试无声地失败→过时的测试现在断言错误的行为。`xfail(strict=True)`模式（Python/pytest）是黄金标准：如果存在错误（预期），它就会失败，如果修复了错误，但没有删除`xfail`标记（严格），它也会失败。其他语言应该近似地使用skip + reason。

审查后关闭验证

在编写所有回归测试和组合摘要之后，运行此检查表：1. **计算合并摘要中的bug。**这是预期的计数。
2. **计数测试功能在`quality/test_regression.*`。**这应该等于或超过BUG数（有些BUG可能需要多次测试）。
3. **对于摘要**中的每个BUG行，验证它是否有：
-引用测试函数名OR的`REGRESSION TEST:`行
-`EXEMPTION:`行解释为什么没有编写测试
4. 如果任何BUG两者都不具备，在宣布审查完成之前，回去编写测试或豁免。

这份核对表是结束任务的执行机制。如果没有它，任务就会变得不切实际——代理在通过摘要中完整地记录bug，但跳过回归测试，继续前进。

规范审计后回归测试闭包要求适用于规范审计确认的代码错误，而不仅仅是代码审查错误。在规范审核对发现进行分类之后，每一个被归类为“真正的代码bug”的发现都必须进行回归测试——使用与代码审查回归测试相同的约定（可执行源文件、预期失败标记、测试发现对齐）。

**为什么这是一个单独的步骤：**代码审查回归测试是在代码审查之后立即编写的，在规范审计运行之前。这意味着规范审计错误是系统孤立的——它们出现在分类报告中，但从未进入回归测试文件。v1.3.4在8个repos上运行，规范审计bug占所有发现的30%，而且只有1 / 8的repos （httpx）为它们编写了回归测试。* *程序:* *
1. 在规范审计分类之后，阅读分类摘要，找出归类为“真正的代码错误”的发现。
2. 对于每一个，使用与代码审查回归测试相同的格式，在`quality/test_regression.*`中编写一个回归测试。使用规格审核报告作为源引用：`[BUG from spec_audits/YYYY-MM-DD-triage.md]`。
3. 运行测试以确认它失败（预期）或通过（需要调查）。
4. 使用测试参考更新PROGRESS.md中的累积BUG跟踪器。

如果规范审计没有产生已确认的代码错误，则跳过此步骤—但是在PROGRESS.md中记录这一点，这样审计跟踪就完成了。

规范审计反转后的清理当规范审计推翻代码审查发现（将BUG分类为设计选择或误报）时，必须删除相应的回归测试，或者将其移到记录故意行为的单独文件（`quality/design_behavior_tests.*`）中。一个指向文档化正确行为的失败测试比没有测试更糟糕——它会制造噪音并侵蚀回归套件中的信任。

在规格审核分类之后，检查：`quality/test_regression.*`中的任何测试是否对应于重新分类为非缺陷的发现？如果是，将其从回归文件中删除。

为什么用三个通道代替焦点区域

之前的实验（QPB NSQ基准）表明，焦点区域并不能可靠地改善AI代码审查。一个通用的“审查漏洞”提示得分为65.5%，而一个有7个命名焦点区域的剧本得分为48.3%——焦点区域缩小了模型的注意力，抑制了检测。三道流程之所以有效，是因为每道流程只做一件事，没有交叉污染：
- **通过1**让模型做它已经擅长的事情（结构审查，~65%的缺陷）
**通过2**捕获结构审查遗漏的个别需求违反（缺失bug，规范偏差）
** 3**捕获单独正确的代码片段之间的矛盾（跨文件算术错误，安全策略缺口）

在NSQ代码库上的实验表明，这个管道发现了3个缺陷中的2个，这些缺陷在所有结构审查条件下都是不可见的——对特定的错误一无所知。发现的缺陷是跨文件数字不匹配（验证绑定与位字段宽度）和安全设计缺口（配置的CA未传播到出站身份验证客户端）。

阶段2：对已确认的bug进行回归测试在代码审查产生发现之后，编写回归测试来重现每个发现的BUG。这将审查从“这里是潜在的bug”转变为“这里是通过失败测试证明的bug”。

**为什么这很重要：**没有复制器的代码审查发现是一种意见。测试失败的结果是事实。跨多个代码库（Go、Rust、Python），根据代码审查结果编写的回归测试已经以很高的速度确认了错误——包括数据竞争、跨租户数据泄漏、状态机违规和静默上下文丢失。回归测试还可以作为修复错误的验收标准：当测试通过时，错误就被修复了。

**如何生成回归测试：**1. **对于每个BUG发现**，编写一个测试：
-目标从发现的确切代码路径和行号
-当前实现失败，确认bug存在
—使用mocking/monkeypatching与外部服务隔离
-在测试文档字符串中包含可追溯性的发现描述

2. **使用项目语言命名测试文件**`quality/test_regression.*`：
—Python:`quality/test_regression.py`- Go:`quality/regression_test.go`（或在相关软件包的测试目录下）
- Rust:`quality/regression_tests.rs`或相关crate中的`tests/regression_*.rs`文件
—Java:`quality/RegressionTest.java`- TypeScript:`quality/regression.test.ts`3. **每个测试应记录其来源：**   ```
   # Python example
   def test_webhook_signature_raises_on_malformed_input():
       """[BUG from 2026-03-26-reviewer.md, line 47]
       Webhook signature verification raises instead of returning False
       on malformed signatures, risking 500 instead of clean 401."""

   // Go example
   func TestRestart_DataRace_DirectFieldAccess(t *testing.T) {
       // BUG from 2026-03-26-claude.md, line 3707
       // Restart() writes mutex-protected fields without acquiring the lock
   }
   ```
4. **运行测试并将结果**作为确认表报告：   ```
   | Finding | Test | Result | Confirmed? |
   |---------|------|--------|------------|
   | Webhook signature raises on malformed input | test_webhook_signature_... | FAILED (expected) | YES — bug confirmed |
   | Queued messages deleted before processing | test_message_queue_... | FAILED (expected) | YES — bug confirmed |
   | Thread active check fails open | test_is_thread_active_... | PASSED (unexpected) | NO — needs investigation |
   ```
5. **如果测试意外通过**，调查——要么发现是假阳性，要么测试没有执行正确的代码路径。报告为需求调查，而不是确认的bug。

* *特定于语言的小贴士:* *

- **Go:**使用`go test -race`确认数据争用结果。比赛探测器是决定性的——如果它被触发，比赛就是真实的。
**Rust:**在特定的错误条件下使用`#[should_panic]`或assert。对于原子性错误，在注入失败后断言清理状态。
- **Python:**使用`monkeypatch`或`unittest.mock.patch`来隔离外部依赖。使用`pytest.raises`来处理异常路径错误。
- **Java:**使用Mockito或类似的隔离依赖。使用`assertThrows`来处理异常路径错误。

**保存回归测试输出**与代码审查：如果审查在`quality/code_reviews/2026-03-26-reviewer.md`，回归测试在`quality/test_regression.*`中，确认结果在审查文件中作为附录或在`quality/results/`中。为什么这些护栏很重要

这四个护栏通常通过减少模糊和幻觉的发现来提高AI代码审查的质量：

1. **行号**迫使模型实际定位问题，而不仅仅是描述一般问题
2. **读取函数体**避免了基于函数名假设函数工作的常见错误
3. **QUESTION vs BUG**减少浪费人力时间的误报
4. **在声称丢失之前进行检查**可以防止最常见的AI审查幻觉：声称某些东西不存在，但它在不同的文件中

“不改变风格”的规则使评审集中在正确性上。风格建议淡化了信号，浪费了审查时间。

---

文件4：集成测试协议（`RUN_INTEGRATION_TESTS.md`）

# # #模板```markdown
# Integration Test Protocol: [Project Name]

## Working Directory

All commands in this protocol use **relative paths from the project root.** Run everything from the directory containing this file's parent (the project root). Do not `cd` to an absolute path or a parent directory — if a command starts with `cd /some/absolute/path`, it's wrong. Use `./scripts/`, `./pipelines/`, `./quality/`, etc.

## Safety Constraints

[If this protocol runs with elevated permissions:]
- DO NOT modify source code
- DO NOT delete files
- ONLY create files in the test results directory
- If something fails, record it and move on — DO NOT fix it

## Pre-Flight Check

Before running integration tests, verify:
- [ ] [Dependencies installed — specific command]
- [ ] [API keys / external services available — specific checks]
- [ ] [Test fixtures exist — specific paths]
- [ ] [Clean state — specific cleanup if needed]

## Test Matrix

| Check | Method | Pass Criteria |
|-------|--------|---------------|
| [Happy path flow] | [Specific command or test] | [Specific expected result] |
| [Variant A end-to-end] | [Command] | [Expected result] |
| [Variant B end-to-end] | [Command] | [Expected result] |
| [Output correctness] | [Specific assertion] | [Expected property] |
| [Component boundary A→B] | [Command] | [Expected result] |

### Design Principles for Integration Checks

- **Happy path** — Does the primary flow work from input to output?
- **Cross-variant consistency** — Does each variant produce correct output?
- **Output correctness** — Don't just check "output exists" — verify specific properties
- **Component boundaries** — Does Module A's output correctly feed Module B?

## Automated Integration Tests

Where possible, encode checks as automated tests:

```bash
[测试运行器][集成测试文件]——verbose```

## Manual Verification Steps

[Any checks requiring external systems, human judgment, or manual inspection]

## Execution UX (How to Present When Running This Protocol)

When an AI agent runs this protocol, it should communicate in three phases so the user can follow along without reading raw output:

### Phase 1: The Plan

Before running anything, show the user what's about to happen:

```
集成测试计划

**飞行前：**检查依赖关系，API密钥和环境
要运行的测试：**

| # |测试|它检查什么|测试时间||---|------|---------------|-----------|
| 1 |[测试名称]|[一行描述]| ~30s |
| 2 |[测试名称]|[一行描述]| ~2m |
|……| | | |

**总计：** N次测试，估计M分钟```

This gives the user a chance to say "skip test 4" or "actually, don't run the live API tests" before anything starts.

### Phase 2: Progress

As each test runs, report a one-line status update. Keep it compact — the user wants a heartbeat, not a log dump:

```
测试1：表情评估- PASS （0.3s）
测试2：模式验证-通过（0.1s）
⧗测试3：活管道（Gemini，实时）…运行```

Use `✓` for pass, `✗` for fail, `⧗` for in-progress. If a test fails, show one line of context (the error message or assertion that failed), not the full stack trace. The user can ask for details if they want them.

### Phase 3: Results

After all tests complete, show a summary table and a recommendation:

```
# #结果

| # |测试|结果|时间|备注||---|------|--------|------|-------|
|表达评估|✓PASS | 0.3s | |
|模式验证|✓PASS | 0.1s | |
| | Live pipeline (Gemini) |·FAIL | 45秒| 8个单位后限速|
|……| | | | |

**通过：**7/8| **失败：**1/8**建议：**在合并前修复-速率限制处理需要调查。```

Then save the detailed results to `quality/results/YYYY-MM-DD-integration.md`.

## Reporting (Saved to File)

Save results to `quality/results/YYYY-MM-DD-integration.md`

### Summary Table
| Check | Result | Notes |
|-------|--------|-------|
| ... | PASS/FAIL | ... |

### Detailed Findings
[Specific failures, unexpected behavior, performance observations]

### Recommendation
[SHIP / FIX BEFORE MERGE / BLOCK]
```
编写良好集成检查的技巧

-每个检查应该执行一个真正的端到端流程，而不仅仅是调用单个函数
-通过标准必须是具体的和可验证的-不是“看起来正确”，而是“输出恰好包含N条具有属性X的记录”
-包括相关的时间预期（特别是batch/pipeline项目）
-如果项目有多种执行模式（批处理与实时，不同的供应商），测试每种组合

针对外部服务的实时执行

集成测试必须检验项目实际的外部依赖关系——api、数据库、服务、文件系统。只测试本地验证和配置解析的协议不是集成测试协议；它是一个伪装的单元测试套件。在探索过程中，确定：
- **项目调用的外部API ** -查找API键。env文件、环境变量引用、provider/client抽象、HTTP客户机配置
- **执行模式** -批处理vs实时，同步vs异步，不同的提供程序后端
- **现有的集成测试运行器** -已经执行端到端流程的脚本或测试文件

然后将测试矩阵设计为**提供者×管道×模式**网格。例如，如果项目支持3个API提供者和3个具有批处理和实时模式的管道，协议应该跨矩阵运行实际执行，而不仅仅是本地验证配置。

**结构为并行运行。**组运行，以便每个提供商最多同时执行一个运行（以避免速率限制）。使用后台进程和`wait`进行组内并发执行。**定义每个管道的质量检查。**每个管道产生不同的输出与不同的正确性标准。协议必须指定要检查的字段和每个管道可接受的值——而不仅仅是“输出存在”。

**包括运行后验证清单。**对于每次运行，验证：日志文件存在完成消息，清单显示终端状态，验证的输出文件存在并包含可解析的数据，示例记录已填充预期字段，并且任何现有的自动质量检查脚本通过。

**飞行前必须检查API密钥。**如果钥匙丢失，请停下来询问—不要悄悄跳过现场测试。我们的目标是在实际条件下运行该协议来测试整个系统，捕捉本地测试可能遗漏的问题：特定于提供者的响应格式差异、超时行为、速率限制和真实LLM响应的输出正确性。

并行性和速率限制意识

顺序集成会浪费时间。组运行，以便独立运行并发执行，具有以下约束：

- **每个外部提供商最多同时运行一次**以避免速率限制
- **使用后台进程和`wait`**在组内并发执行
—**在会话开始时生成一个共享的时间戳**，以保持运行目录命名的一致性

一个有3个管道和3个提供者（9+运行）的项目分组示例：```
Group 1 (parallel): Pipeline_A × Provider_1 | Pipeline_B × Provider_2 | Pipeline_C × Provider_3
Group 2 (parallel): Pipeline_A × Provider_2 | Pipeline_B × Provider_3 | Pipeline_C × Provider_1
Group 3 (parallel): Pipeline_A × Provider_3 | Pipeline_B × Provider_1 | Pipeline_C × Provider_2
```
此模式最大限度地提高了吞吐量，同时不会因为并发请求而碰到相同的提供程序。根据项目的实际管道和提供者数量调整分组。

在生成的协议中，包括实际的bash命令，其中`&`用于后台执行，`wait`用于组间执行。不要仅仅描述并行性——用脚本来描述它。

从代码中派生质量门

通用的pass/fail标准（“所有单元验证”）忽略了特定于领域的正确性问题。从代码本身派生特定于管道的质量检查：

1. **读取验证规则。**如果项目验证输出（模式验证器、断言函数、业务规则检查），这些规则定义“正确”是什么样子的。将它们转换为质量门：“字段X必须满足所有输出记录的条件Y。”2. **读取模式枚举。**如果模式定义了枚举字段（例如，`outcome: ["fell_in_water", "reached_ship"]`），质量门是：“所有输出必须使用这个集合中的值，并且分布应该是非退化的（不是100%一个值）。”

3. **读取生成逻辑。**如果项目生成测试数据（项目文件、种子数据、排列策略），了解应该出现哪些变体。如果有3种人格类型，质量门槛是：“所有3种类型都必须出现在输出中，并有足够的样本量。”

4. **阅读现有的质量检查。**搜索已经验证输出质量的脚本或函数（例如，`integration_checks.py`，运行后调用的验证函数）。直接从协议中引用或调用它们。对于项目中的每个管道，集成协议应该有一个专门的“质量检查”部分，其中列出了2-4个特定的检查，这些检查具有从上述探索中得到的期望值。不要使用像“output exists”这样的通用检查——每个检查都必须引用一个特定的字段和可接受的值范围。

字段参考表（写质量门之前需要）**为什么存在：** AI模型自信地写出错误的字段名，即使他们已经读了模式。这是因为模型在探索期间读取模式，然后在数小时（或数千个令牌）之后从内存中写入协议。内存漂移：`document_id`变为`doc_id`，`sentiment_score`变为`sentiment`，`float 0-1`变为`int 0-100`。协议看起来很权威，但字段名是幻觉。当有人根据真实数据运行质量闸门时，他们失败了——用户对整个生成的剧本失去了信任。

**修复是程序性的，而不是指导性的。**不要只是告诉自己“以后再交叉检查”——先建立参考表，然后通过复制它来编写质量检验表。

在编写任何引用输出字段名的质量门之前，通过重新读取每个模式文件来构建一个字段引用表：```
## Field Reference Table (built from schemas, not memory)

### Pipeline: WeatherForecast
Schema: pipelines/WeatherForecast/schemas/analyze.json
| Field | Type | Constraints |
|-------|------|-------------|
| region_name | string | — |
| temperature | number | min: -50, max: 60 |
| condition | string | enum: ["sunny", "cloudy", "rain", "snow"] |

### Pipeline: SentimentAnalysis
Schema: pipelines/SentimentAnalysis/schemas/evaluate.json
| Field | Type | Constraints |
|-------|------|-------------|
| document_id | string | — |
| sentiment_score | number | min: -1.0, max: 1.0 |
| classification | string | enum: ["positive", "negative", "neutral"] |
...
```
* *过程:* *
1. **在写入每个表行之前，立即重新读取每个模式文件。**不要从内存中写入任何行。读的文件和表的行必须相邻——读文件，写行，读下一个文件，写下一行。如果您在对话的前面读取了所有模式，那就不算数了——您必须在这里再次读取它们，因为您对字段名的记忆会漂移到数千个令牌上。
2. **从文件内容中逐字复制字段名。**不要重新输入。`document_id`不是`doc_id`。`sentiment_score`不是`sentiment`。`classification`不是`category`。即使是很小的差异也会打破质量门槛。
3. **包括模式中的所有字段，而不仅仅是那些你认为重要的字段。**如果模式有8个必填字段，则表有8行。如果编写的行数少于模式的字段数，则跳过字段。
4. 中的字段名来编写质量检验关完成表。
5. 写完后，计数字段：如果质量闸门提到了一个不在表中的字段，那就是你产生了幻觉。删除它。这个表是一个中间工件——将它包含在协议本身中（作为参考部分），以便将来的协议用户可以验证字段的准确性。关键是要将其创建为产生模式读取证据的具体步骤，而不是因为“已经知道”字段而跳过它。

校准刻度

每次集成测试运行的units/records/iterations的数量很重要：**太少（1-3）：**快速和便宜，但错过并发错误，分布检查失败（不能验证“25-75%的比例”与2条记录），fan-out/expansion逻辑未经实际规模测试。
- **太多（100+）：**测试协议昂贵且缓慢。适用于生产，但不适用于质量验证。
- **正确范围：**足够有意义地锻炼系统。指南:
-如果项目有chunking/batching逻辑，使用至少跨越2块的计数（例如，如果chunk_size=10，使用15-30个单位）
-如果项目有分布检查，使用至少5 - 10倍的类别数量（例如，3个结果类型→至少15个单位）
-如果项目有fan-out/expansion，则使用产生非平凡子数目的计数在项目中查找`chunk_size`、`batch_size`或类似的配置进行校准。当有疑问时，10-30条记录通常是集成测试的正确范围——足以在不消耗API预算的情况下发现真正的问题。

技能和llm自动化工具的集成测试

当测试项目是人工智能技能、包装LLM调用的CLI工具，或其主要执行路径涉及调用人工智能模型的任何软件时，集成测试协议必须包括**LLM自动集成测试**——通过命令行人工智能代理端到端运行工具并从结构上验证输出的测试。这与标准的集成测试不同，因为被测系统没有要调用的确定性API。“集成”是：将技能安装到测试仓库中，通过CLI代理（GitHub CopilotCLI, Claude Code，或类似的）调用它，并验证输出工件满足结构和内容预期。

**为什么这很重要：**技能和LLM工具不能通过直接调用函数来测试——它们的执行路径要经过一个解释指令、读取文件和产生工件的AI代理。测试技能是否有效的唯一方法就是运行它。手动执行对于开发来说是好的，但是质量手册应该将测试编码为可重复的协议。

**skill/LLM集成测试协议结构：**```markdown
## Skill Integration Test Protocol

### Prerequisites
- CLI agent installed and configured (e.g., `gh copilot`, `claude`, `npx @anthropic-ai/claude-code`)
- Test repo prepared with skill installed at `.github/skills/SKILL.md` (or equivalent)
- Clean `quality/` directory (no artifacts from prior runs)
- Optional: `reference_docs/` folder for with-docs comparison runs

### Test Matrix

| Test | Method | Pass Criteria |
|------|--------|---------------|
| Full execution | Run skill via CLI with "execute" prompt | All expected artifacts exist in `quality/` |
| PROGRESS.md completeness | Read `quality/PROGRESS.md` | All phases checked complete, BUG tracker populated |
| Artifact structural check | Verify each expected file | Files are non-empty, contain expected sections |
| BUG tracker closure | Count BUG entries vs regression tests | Every BUG has a test reference or exemption |
| Baseline vs with-docs (optional) | Run twice: without and with reference_docs/ | With-docs run produces >= baseline requirement count |

### Execution

```bash
#将技能安装到测试仓库
Cp -rpath/to/skill/.githubtest-repo/.github#通过CLI代理运行（调整命令到您的代理）
cd test-repo
读取.github/skills/SKILL.md及其参考文件。执行这个项目的质量剧本。”＼    --model gpt-5.4 --yolo > quality_run.output.txt 2>&1
```

### Structural Verification (automated)

After the run, verify output structurally:

```bash
需要的工件存在并且是非空的
对于f在quality/QUALITY.mdquality/REQUIREMENTS.mdquality/CONTRACTS.md\         quality/COVERAGE_MATRIX.md quality/COMPLETENESS_REPORT.md \
         quality/PROGRESS.md quality/RUN_CODE_REVIEW.md \
         quality/RUN_INTEGRATION_TESTS.md quality/RUN_SPEC_AUDIT.md; do
    [ -s "$f" ] || echo "FAIL: $f missing or empty"
完成

#功能测试文件存在（语言名称合适）quality/test_functional.*quality/FunctionalSpec.*quality/functional.test.* 2>/dev/null\    || echo "FAIL: no functional test file"
＃PROGRESS.md已检查所有阶段
Grep -c ' \[x\] 'quality/PROGRESS.md＃应该等于总相位计数

BUG跟踪器有条目（如果发现了BUG）
Grep -c '^| [0-9]'quality/PROGRESS.md代码审查和规范审计产生了实质性的文件
找到quality/code_reviews-name“*”。Md " -size +500c | wc - l＃应该是>= 1
找到quality/spec_audits-name "*triage*" -size +500c | wc -l ＃应该是>= 1```
```
**基线与带文档的比较模式：**在同一个repo上运行两次技能-一次没有补充文档，一次带有包含项目历史的`reference_docs/`文件夹。比较：需求计数、场景计数、bug计数和管道完成情况。with-docs运行应该会产生相同或更多的需求和相同或更多的bug。如果基线在bug检测上优于with-docs运行，这是关于文档质量的发现，而不是技能失败。

**何时生成此协议：**在`RUN_INTEGRATION_TESTS.md`中生成技能集成测试部分，无论何时被分析的项目是技能，包装AI调用的CLI工具或构建AI驱动工具的框架。查找：`SKILL.md`文件、提示模板、LLM客户端配置、代理编排代码或对代码库中AI模型的引用。

运行后验证深度没有错误完成的运行可能仍然是错误的。对于每个集成测试运行，在多个级别进行验证：

1. **进程级别：**进程是否干净退出？检查日志文件以查看完成消息，而不仅仅是退出代码。
2. **状态级：**运行是否处于终端状态？检查runmanifest/status文件的“complete”（没有卡在“running”或“submitted”中）。
3. **数据级别：**输出数据是否存在并正确解析？读取实际的输出文件，验证它们包含有效的JSON/CSV/etc.4. **内容级：**输出记录是否有预期的字段填充合理的值？阅读2-3个示例记录并检查关键字段。
5. **质量级别：**特定管道的质量检验关是否通过？运行任何现有的质量检查脚本。
6. ** ui级别（如果适用）：**如果项目有dashboard/TUI/UI，请验证运行在那里是否正确。在生成的协议的运行后检查表中包括所有适用的级别。常见的故障是在level 2（进程完成）停止而没有检查level 3-5。