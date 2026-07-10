#三准则理事会审核协议（文件5）

这是一个静态分析协议——AI模型读取代码并将其与规范进行比较。不执行任何代码。它捕获了与测试不同的一类问题：规范代码分歧、未记录的特性、虚幻规范和缺失的实现。

为什么是三个模型？

不同的人工智能模型有不同的盲点——它们对不同的事情有信心，也会错过不同的事情。交叉引用三个独立的评审可以捕获任何单个模型都可能忽略的缺陷。

# #模板```markdown
# Spec Audit Protocol: [Project Name]

## The Definitive Audit Prompt

Give this prompt identically to three independent AI tools (e.g., Claude, GPT, Gemini).

---

**Context files to read:**
1. [List all spec/intent documents with paths]
2. [Architecture docs]
3. [Design decision records]

**Task:** Act as the Tester. Read the actual code in [source directories] and compare it against the specifications listed above.

**Requirement confidence tiers:**
Requirements are tagged with `[Req: tier — source]`. Weight your findings by tier:
- **formal** — written by humans in a spec document. Authoritative. Divergence is a real finding.
- **user-confirmed** — stated by the user but not in a formal doc. Treat as authoritative unless contradicted by other evidence.
- **inferred** — deduced from code behavior. Lower confidence. Report divergence as NEEDS REVIEW, not as a definitive defect.

**Rules:**
- ONLY list defects. Do not summarize what matches.
- For EVERY defect, cite specific file and line number(s).
  If you cannot cite a line number, do not include the finding.
- Before claiming missing, grep the codebase.
- Before claiming exists, read the actual function body.
- Classify each finding: MISSING / DIVERGENT / UNDOCUMENTED / PHANTOM
- For findings against inferred requirements, add: NEEDS REVIEW

**Defect classifications:**
- **MISSING** — Spec requires it, code doesn't implement it
- **DIVERGENT** — Both spec and code address it, but they disagree
- **UNDOCUMENTED** — Code does it, spec doesn't mention it
- **PHANTOM** — Spec describes it, but it's actually implemented differently than described

**Project-specific scrutiny areas:**

[5–10 specific questions that force the auditor to read the most critical code. Target:]

1. [The most fragile module — force the auditor to read specific functions]
2. [External data handling — validation, normalization, error recovery]
3. [Assumptions that might not hold — field presence, value ranges, format consistency]
4. [Features that cross module boundaries]
5. [The gap between documentation and implementation]
6. [Specific edge cases from the QUALITY.md scenarios]

**Output format:**

### [filename.ext]
- **Line NNN:** [MISSING / DIVERGENT / UNDOCUMENTED / PHANTOM] [Req: tier — source] Description.
  Spec says: [quote or reference]. Code does: [what actually happens].
  *(Include the `[Req: tier — source]` tag so findings can be traced back to their requirement and confidence level.)*

---

## Pre-audit docs validation (required triage section)

The triage report must include a `## Pre-audit docs validation` section regardless of whether `reference_docs/` exists. This section documents what the auditors used as their factual baseline.

**If `reference_docs/` exists:** Spot-check the gathered docs for factual accuracy before running the audit. Stale or incorrect docs can skew audit confidence — a model that reads "the library handles X by doing Y" in the docs will rate a divergent finding higher even if the docs are wrong.

**Quick validation procedure (5 minutes max):**
1. Pick 2–3 factual claims from `reference_docs/` that describe specific runtime behavior (e.g., "invalid input raises ValueError", "field X defaults to Y", "format Z is not supported").
2. Grep the source code for the cited behavior. Does the code match the docs?
3. If any claim is wrong, note it in the triage header: "reference_docs/ contains N known inaccuracies: [list]. Findings that rely on these claims are downgraded to NEEDS REVIEW."

**Spot-check claims about code contents must extract, not assert.** When the spec audit prompt or pre-validation includes claims like "function X handles constant Y at line Z," the triage must read the cited lines and report what they actually contain. Do not confirm a claim by checking that the function exists or that the constant is defined somewhere — confirm it by showing the exact text at the cited lines. Format each spot-check result as:

```
声明：“vring_transport_features（）保留了3527行的VIRTIO_F_RING_RESET”
实际行3527:`default:`结果：CLAIM IS FALSE -行3527是默认分支，而不是RING_RESET case标签```

Spot-check claims derived from generated requirements or gathered docs (rather than from the code) are **hypotheses to test**, not facts to confirm. This rule prevents the contamination chain observed in v1.3.17 where a false spot-check claim was accepted as "accurate" without reading the actual lines, causing three auditors to inherit a hallucinated code-presence claim.

**If `reference_docs/` does not exist:** State this explicitly: "No supplemental docs provided. Auditors relied on in-repo specs and code only." This confirms the absence is intentional, not an oversight.

This section fires in every triage, not just when docs are present. In v1.3.5 cross-repo testing, it only fired in 1/8 repos because it was conditional — making it required ensures the audit trail always documents the factual baseline.

## Running the Audit

1. Give the identical prompt to three AI tools
2. Each auditor works independently — no cross-contamination
3. Collect all three reports

## Triage Process

After all three models report, merge findings.

**Log the effective council size.** If a model did not return a usable report (timeout, empty output, refusal), record this in the triage header:

```
##理事会现状
-型号A：新收到的报告（YYYY-MM-DD）
-型号B：新收到报告（YYYY-MM-DD）
-型号C: TIMEOUT -无可用报告。生效理事会：2/3.```

When the effective council is 2/3, downgrade the confidence tier: "All three" becomes impossible, "Two of three" becomes the ceiling. When the effective council is 1/3, all findings are "Needs verification" regardless of how confident that single model is. Do not silently substitute stale reports from prior runs — if a model didn't produce a fresh report for this run, it didn't participate.

| Confidence | Found By | Action |
|------------|----------|--------|
| Highest | All three | Almost certainly real — fix or update spec |
| High | Two of three | Likely real — verify and fix |
| Needs verification | One only | Could be real or hallucinated — deploy verification probe |

**When the effective council is 2/3 or less:** Distinguish single-auditor findings from multi-auditor findings explicitly in the triage. With a 2/3 council, a finding from both present auditors has "High" confidence. A finding from only one present auditor has "Needs verification" — it cannot be promoted to confirmed BUG without a verification probe, because the missing auditor might have contradicted it. Do not treat all findings as equivalent just because the council is incomplete.

In the triage summary table, add a column for auditor agreement: "2/2 present", "1/2 present", etc. This makes the confidence tier visible and auditable.

**Incomplete council gate for enumeration/dispatch checks.** If the effective council is less than 3/3 and the run includes whitelist/enumeration/dispatch-function checks (claims about which constants a function handles), the audit may not conclude "no confirmed defects" for those checks without executed mechanical proof. Check whether `quality/mechanical/<function>_cases.txt` exists for each relevant function. If it does and shows the constant is present, the claim is confirmed. If it does and shows the constant is absent, the claim is false regardless of what any auditor wrote. If no mechanical artifact exists, generate one before closing the enumeration check. This rule exists because v1.3.18 had an effective council of 1/3, and the single model's triage fabricated line contents for enumeration claims — a mechanical artifact would have caught the contradiction.

### The Verification Probe

When models disagree on factual claims, deploy a read-only probe: give one model the disputed claim and ask it to read the code and report ground truth. Never resolve factual disputes by majority vote — the majority can be wrong about what code actually does.

**Verification probes must produce executable evidence.** Prose reasoning is not sufficient for either confirmations or rejections. Every verification probe must produce a test assertion that mechanically proves the determination:

**For rejections** (finding is false positive): Write an assertion that PASSES, proving the auditor's claim is wrong:
```python
#拒绝证明：函数X在第247行检查null
在source_of（“X“）中断言”if (ptr == NULL)”， “X在第247行检查为空”```
If you cannot write a passing assertion, **do not reject the finding**. The inability to produce mechanical proof is itself evidence that the finding may be real.

**For confirmations** (finding is a real bug): Write an assertion that FAILS (expected-failure), proving the bug exists:
```python
#确认证明：RING_RESET不是白名单中的case标签
assert "case VIRTIO_F_RING_RESET:" in source_of("vring_transport_features"), \    "RING_RESET should be in the switch but is not — cleared by default at line 3527"
```

**Every assertion must cite an exact line number** for the evidence it references. Not "lines 3527-3528" but "line 3527: `default:`" — showing what the line actually contains.

**Why this rule exists:** In v1.3.16 virtio testing, the triage received a correct minority finding that VIRTIO_F_RING_RESET was missing from a switch/case whitelist. The triage performed a verification probe that claimed lines 3527-3528 "explicitly preserve VIRTIO_F_RING_RESET" — but those lines contained the `default:` branch. The probe hallucinated compliance. Had it been required to write `assert "case VIRTIO_F_RING_RESET:" in source`, the assertion would have failed, exposing the hallucination. Requiring executable evidence makes hallucinated rejections self-defeating.

### Categorize Each Confirmed Finding

- **Spec bug** — Spec is wrong, code is fine → update spec
- **Design decision** — Human judgment needed → discuss and decide
- **Real code bug** — Fix in small batches by subsystem
- **Documentation gap** — Feature exists but undocumented → update docs
- **Missing test** — Code is correct but no test verifies it → add to the functional test file
- **Inferred requirement wrong** — The inferred requirement doesn't match actual intent → remove or correct it in QUALITY.md

That last category is the bridge between the spec audit and the test suite. Every confirmed finding not already covered by a test should become one.

### Legacy and historical scripts

Scripts documented as "historical," "deprecated," or "not part of current workflow" are sometimes downgraded during triage on the theory that they don't affect current operations. This is correct when the script genuinely never runs. But if the script's bug has already materialized in canonical artifacts — duplicate entries in a published file, stale data in a checked-in cache, incorrect mappings that downstream tools consume — the bug is not historical. It's a live defect in the repository's published state.

**Rule: If a legacy script's bug is already visible in canonical artifacts, promote it to confirmed BUG regardless of the script's status.** The script may be historical, but the damage it left behind is current. The regression test should target the artifact (the duplicate entry, the stale mapping), not the script — because the artifact is what users encounter.

This rule exists because v1.3.5 bootstrap runs on QPB found duplicate changelog entries and stale cache mappings produced by a "historical" script. Both triages downgraded the findings because the script was historical. But the duplicate entries were already in the published library, visible to every user.

### Cross-artifact consistency check

After triage, compare the spec audit findings against the code review findings from `quality/code_reviews/`. If the code review and spec audit disagree on the same factual claim (one says a bug is real, the other calls it a false positive), flag the disagreement and deploy a verification probe. The code review and spec audit use different methods (structural reading vs. spec comparison), so disagreements are informative, not errors. But a factual contradiction about what the code actually does needs to be resolved before either report is trusted.

## Detecting partial sessions and carried-over artifacts

### Partial session detection

A session that terminates early (timeout, context exhaustion, crash) may generate scaffolding (directory structure, empty templates) without producing the actual review or audit content. The retry mechanism in the run script can regenerate scaffolding but cannot recover the analytical work.

**After any session completes, check for partial results:**
1. If `quality/code_reviews/` exists but contains no `.md` files with actual findings (or only contains template headers with no BUG/VIOLATED/INCONSISTENT entries), the code review did not run. Mark this as FAILED in PROGRESS.md, not as "complete with no findings."
2. If `quality/spec_audits/` exists but contains no triage summary, the spec audit did not run.
3. If `quality/test_regression.*` exists but contains only imports and no test functions, regression tests were not written.

A partial session is not a "clean run with no findings" — it's a failed run that needs to be re-executed. PROGRESS.md should record this clearly: "Phase 6: FAILED — code review session terminated before producing findings. Re-run required."

### Provenance headers on carried-over artifacts

When a new playbook run finds existing artifacts from a previous run (after archiving), or when artifacts survive from a failed session, they must carry provenance headers so readers know their origin.

**If any artifact was NOT generated fresh in the current run**, add a provenance header:

```markdown
<!-- PROVENANCE: This file was carried over from a previous run ([date]).
     It was NOT regenerated by the current v1.3.5 run.
     Treat findings as potentially stale — verify against current source before acting. -->
```

This prevents the failure mode observed in v1.3.4 where express and zod silently preserved v1.3.3 code reviews and spec audits without marking them as archival. Users reading those artifacts assumed they were fresh v1.3.4 results.

## Fix Execution Rules

- Group fixes by subsystem, not by defect number
- Never one mega-prompt for all fixes
- Each batch: implement, test, have all three reviewers verify the diff
- At least two auditors must confirm fixes pass before marking complete

## Output

Save audit reports to `quality/spec_audits/YYYY-MM-DD-[model].md`
Save triage summary to `quality/spec_audits/YYYY-MM-DD-triage.md`
```
四道护栏（对所有审计师都至关重要）

一些模型在没有检查代码的情况下自信地声称功能缺失。这四个规则嵌入到审计提示中，通过减少模糊和幻觉的发现，大大提高了输出质量：

1. **强制行号** -如果你不能引用行号，不包括发现。这消除了模糊的要求。
2. **在声明缺失之前搜索** -在声明某个特性缺失之前，搜索代码库。它可能在另一个文件中。
3. **阅读函数体，而不仅仅是签名** -不要根据函数的名称假设函数工作正确。
4. **对缺陷类型进行分类** -强制结构化思维（MISSING/DIVERGENT/UNDOCUMENTED/PHANTOM），而不是模糊的“这看起来不对”。

这些护栏已经嵌入在上面的模板中。对于那些倾向于自信但未经证实的说法的模特来说，它们最重要。

型号选择说明不同的模型具有不同的审计强度。在实践中:

- **以体系结构为中心的模型**（例如Claude）倾向于用最少的误报找到最多的问题，擅长于无声数据丢失、跨功能数据流和状态机错误。
- **关注边缘情况的模型**（例如，基于gpt的工具）倾向于捕捉其他模型遗漏的边界条件（零长度输入，文件冲突，差一错误），并作为有效的验证交叉检查器。
**需要结构的模型（例如，一些Gemini变体）可能在开放式审计提示上表现不佳，但对上述四道护栏的反应却非常明显。

excel的具体模型会随着时间的推移而改变。原则是：使用不同强度的多个模型，并始终包括四个护栏。

最小模型能力审计协议要求读取函数体，引用行号，在声明缺失之前进行检查，并对缺陷类型进行分类。轻量级或速度优化的模型（俳句级，gpt - 40 -mini级）不适合作为审计器。他们倾向于略读而不是阅读，跳过grep步骤，并在代码库中生成肤浅或空洞的报告（“未发现缺陷”），而更强大的模型发现了真正的bug。为所有三个审计员位置使用具有较强代码阅读能力的模型。一个软弱的审计员不仅会错过调查结果，还会把委员会从三个独立的角度减少到两个。

编写审查区域的技巧

审查区域是提示中最重要的部分。像“检查代码是否符合规范”这样的通用问题会产生通用的答案。命名函数、文件和边缘情况的特定问题产生特定的结果。良好的审查领域：
读取`pipeline.py`第45-120行中的`process_input()`。规范说它应该通过替换默认值来处理缺失的字段。不是吗?哪些字段有默认值，哪些字段静默地产生空值？”
-“架构文档说模块A将经过验证的数据传递给模块b。是否存在未经验证的数据到达模块B的路径？”

不良审查领域：
-“检查代码是否正确”
-“查找bug”
-“验证实现是否符合规范”