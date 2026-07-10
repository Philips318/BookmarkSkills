#需求评审协议

# #概述

这是`quality/REVIEW_REQUIREMENTS.md`的模板。剧本与需求管道输出一起生成这个文件。它提供了三种模式，用于在生成后交互式地审查需求。

##生成文件模板

剧本应该生成如下`quality/REVIEW_REQUIREMENTS.md`：

---```markdown
# Requirements Review Protocol: [Project Name]

## How to use

This protocol helps you review the generated requirements for completeness and accuracy. Run it with any AI model — the review is self-contained and reads from the files in `quality/`.

**Before starting:** Make sure `quality/REQUIREMENTS.md` exists (from the pipeline) and that you've read the Project Overview and Use Cases sections at the top.

### Choose a review mode

**Mode 1 — Self-guided review.** You pick which use cases to examine. Best when you already know which areas of the project need the most scrutiny.

**Mode 2 — Fully guided review.** The AI walks you through every use case in order, drilling into each linked requirement. Best for a thorough first review.

**Mode 3 — Cross-model audit.** A different AI model fact-checks the completeness report by verifying that every domain marked COVERED actually has requirements addressing the checklist item. Best run with a different model than the one that generated the requirements.

All three modes track progress in `quality/REFINEMENT_HINTS.md`.

---

## Mode 1: Self-guided review

Read `quality/REQUIREMENTS.md` and present the user with a numbered list of use cases:

```
REQUIREMENTS.md中的用例：
1. 用例1:[名称]（已审核）
2. []用例2:[名称]
3. []用例3:[名称]
…```

Check `quality/REFINEMENT_HINTS.md` for review progress — use cases marked `[x]` have already been reviewed. Present the list and ask the user which use case to examine.

When the user picks a use case:
1. Show the use case (actor, steps, postconditions, alternative paths)
2. List the linked REQ-NNN numbers
3. Ask: "Want to drill into any of these requirements, or does this use case look complete?"

When drilling into a requirement:
1. Show the full requirement (summary, user story, conditions of satisfaction, alternative paths)
2. Ask: "Does this capture the right behavior? Anything missing or wrong?"
3. Record feedback in REFINEMENT_HINTS.md under the use case heading

After reviewing a use case, mark it `[x]` in REFINEMENT_HINTS.md and return to the use case list.

Also offer: "Are there any cross-cutting concerns or requirements NOT linked to a use case that you'd like to review?"

---

## Mode 2: Fully guided review

Same as Mode 1, but instead of asking the user to pick, start at Use Case 1 and proceed sequentially.

For each use case:
1. Present the use case overview
2. Walk through each linked requirement one by one
3. For each requirement, ask: "Does this look right? Anything missing?"
4. Record any feedback in REFINEMENT_HINTS.md
5. Mark the use case as reviewed
6. Move to the next use case

After all use cases:
1. Present the Cross-Cutting Concerns section
2. Ask: "Any concerns about threading, null handling, errors, compatibility, or configuration composition?"
3. Ask: "Are there any requirements you expected to see that aren't here?"
4. Record feedback and present a summary of all hints collected

---

## Mode 3: Cross-model audit

Read `quality/COMPLETENESS_REPORT.md` and `quality/REQUIREMENTS.md`. For each domain in the completeness report:

1. Read the domain checklist item (from the report's domain coverage section)
2. Read each cited REQ-NNN
3. Verify: does this requirement actually address the domain checklist item?
4. If the citation is wrong (the requirement covers something else), flag it as a gap

Also check:
- Are there requirements that don't appear in any use case's Requirements list? If so, flag as potentially orphaned.
- Does every use case's alternative paths section have corresponding requirements for the error/edge cases it mentions?
- Do the cross-cutting concerns reference requirements that actually exist and address the stated concern?

Write findings to `quality/REFINEMENT_HINTS.md` under a `## Cross-Model Audit` heading:

```
跨模型审计
日期(日期):
型号：[型号名称]

已验证的域
-空处理：确认（REQ-054， REQ-055正确处理空语义）
-…

###发现漏洞
-切入点：COMPLETENESS_REPORT引用REQ-100， REQ-101，但这些是关于
漂亮的印刷，不是入门级合同。JsonStreamParser没有覆盖。
-…

孤立的需求
- REQ-NNN不链接到任何用例
-…```

Present findings to the user and ask which gaps should be addressed in a refinement pass.

---

## REFINEMENT_HINTS.md format

The review protocol creates and maintains this file:

```markdown
#改进提示

##回顾进度
- [x]用例1:[名称]-已审核，无问题
- [x]用例2:[名称]-已审核，见下面的反馈
-[]用例3:[name]
-[]用例4:[名称]
…

横切关注点
[]线程模型-尚未审查
-[]合同无效-尚未审核
[]错误哲学-还没有复习
-[]向后兼容性-尚未审查
-[]配置组成-尚未审查

# #反馈

用例2:[name]
- REQ-NNN:[关于缺失或错误的具体反馈]
-总则：[对这个用例的覆盖范围进行更广泛的观察]

跨模型审计
[如果模式3运行]

##附加提示
[来自用户的自由反馈，与特定用例无关]```

This file serves dual purpose: it tracks review progress (so the user can resume across sessions) AND accumulates feedback that the refinement pass reads.
```
