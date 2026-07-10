---
name: requirements-traceability
description: '构建并验证从 SRS requirements 到 AC、BDD scenarios、unit tests 和 manual test cases 的 traceability chains。检测 requirements 缺少对应 tests 的 coverage gaps。产出用于 IEC 62304 audit readiness 的 traceability matrix。'
argument-hint: 'Provide a feature name, requirements doc path, and optionally test file paths to build the traceability chain'
user-invocable: true
---

# Requirements Traceability

使用此 skill 构建并验证从 requirements 到 tests 的端到端 traceability，确保每个 requirement 都可验证，并且每个 test 都能追溯回 requirement。

## When to Use

- requirements + BDD scenarios 产出后 — 检查 coverage gaps。
- test generation（Stage 4）之后 — 验证 full chain completeness。
- IEC 62304 audit 前 — 产出 audit-ready traceability matrix。
- 为 existing features 添加 tests 时 — 验证没有 requirement 成为 orphan。

## Preferred Inputs

- **Requirements document** — `artifacts/<feature>/01-requirements.md`
- **BDD scenarios** — `artifacts/<feature>/01-bdd-scenarios.feature`
- **Test report** — `artifacts/<feature>/04-test-report.md`（如可用）
- **Test source files** — `*Tests.cs` files 的路径（如可用）

## Traceability Chain

```
SRS Requirement
  └── User Story
        └── AC-1
              ├── BDD Scenario (Gherkin)
              ├── Verification Method (requirements-level)
              ├── Unit Test (implementation-level)
              └── Manual Test Case (acceptance-level)
```

chain 中每个 link 都必须是 bidirectional：
- **Forward**: Requirement → AC → Test（每个 requirement 都有 tests）
- **Backward**: Test → AC → Requirement（每个 test 都追溯到 requirement）

## Analysis Process

### Step 1: Extract Requirements

解析 `01-requirements.md` 并构建 list：

| ID | 标题 | 类型 |
|----|-------|------|
| AC-1 | [title] | 功能性 |
| AC-2 | [title] | 功能性 |
| NFR-1 | [title] | Non-Functional |

### Step 2: Map BDD Scenarios

解析 `01-bdd-scenarios.feature`，并将 @AC tags 匹配到 requirements：

| AC | 场景 | 标签 | Matched? |
|----|----------|-----|----------|
| AC-1 | [title] | @AC-1 | ✓ |
| AC-2 | — | — | ✗ Gap! |

### Step 3: Map Verification Methods

从 `01-requirements.md` 的 Verification Methods table 提取：

| AC | 验证方法 | Defined? |
|----|-------------------|----------|
| AC-1 | [steps] | ✓ |
| AC-2 | — | ✗ Gap! |

### Step 4: Map Unit Tests (if available)

解析 test files 或 `04-test-report.md`：

| AC | Unit Test Method | 是否覆盖？ |
|----|-----------------|----------|
| AC-1 | FormatStageResult_Valid... | ✓ |
| AC-3 | FormatStageResult_Negative... | ✓ |

### Step 5: Map Manual Test Cases (if available)

| AC | Manual Test Case | 是否覆盖？ |
|----|-----------------|----------|
| AC-1 | TC-01 | ✓ |
| AC-4 | TC-05 | ✓ |

## Output Structure

```markdown
# Traceability Matrix — [Feature Name]

## Forward Traceability (Requirement → Test)

| AC | BDD Scenario | Verification Method | Unit Test | Manual Test | Status |
|----|-------------|-------------------|-----------|-------------|--------|
| AC-1 | ✓ Scenario 1 | ✓ VM-1 | ✓ Test_Valid | ✓ TC-01 | FULL |
| AC-2 | ✓ Scenario 2 | ✓ VM-2 | ✗ Missing | ✓ TC-02 | PARTIAL |
| AC-3 | ✗ Missing | ✗ Missing | ✗ Missing | ✗ Missing | NONE |

## Backward Traceability (Test → Requirement)

| Test | Type | Traces to AC | Orphan? |
|------|------|-------------|---------|
| Test_Valid | Unit | AC-1 | No |
| Test_Boundary | Unit | AC-3 | No |
| Test_Legacy | Unit | — | ✗ Orphan! |

## Coverage Summary

| Metric | Count | Percentage |
|--------|-------|-----------|
| Total ACs | N | — |
| ACs with BDD scenario | N | ?% |
| ACs with verification method | N | ?% |
| ACs with unit test | N | ?% |
| ACs with manual test | N | ?% |
| Full coverage (all 4) | N | ?% |
| No coverage (0 of 4) | N | ?% |

## Gaps

### [GAP-1] AC-3 has no BDD scenario
**Severity:** HIGH — Every AC must have at least one Gherkin scenario.
**Action:** Add scenario to 01-bdd-scenarios.feature.

### [GAP-2] Test_Legacy is orphan (no AC)
**Severity:** LOW — Test exists but does not trace to any current requirement.
**Action:** Review if test covers an undocumented requirement or is obsolete.

## Audit Readiness

| IEC 62304 Check | Status |
|----------------|--------|
| Every requirement has verification method | ✓/✗ |
| Forward traceability complete | ✓/✗ |
| Backward traceability complete | ✓/✗ |
| No orphan tests | ✓/✗ |
| Coverage > 80% across all chains | ✓/✗ |

**Verdict: AUDIT_READY / GAPS_FOUND / NOT_READY**
```

## Quality Rules

- 每个 AC 至少必须有一个 BDD scenario 和一个 verification method（minimum bar）。
- Orphan tests（无 AC link）必须 review — 它们可能表明存在 undocumented requirements。
- 任一 chain coverage 低于 80% 会触发 GAPS_FOUND verdict。
- 对 IEC 62304 Class B/C，100% forward traceability 是 mandatory。

## Required Chains by Safety Class

AUDIT_READY 所需的 minimum chain depth 取决于 `01-requirements.md` §12 中的 Safety Classification：

| 安全等级 | BDD Scenario | 验证方法 | Unit Test | Manual Test | 影响分析 |
|--------------|:------------:|:-------------------:|:---------:|:-----------:|:---------------:|
| **A**        | 必需     | 必需            | Recommended | 可选  | 可选        |
| **B**        | 必需     | 必需            | 必需  | 必需    | **Required**    |
| **C**        | 必需     | 必需            | 必需  | 必需    | **Required** + every applicable `safety-rules.md` red line traced |

如果 Class C feature 适用任何 `safety-rules.md` red line，但它**没有**被 trace 到 AC、BDD scenario 和 test，则自动为 `NOT_READY`。

## Portability Note

此 skill 是 **team-portable**。traceability chain（Requirement → AC → BDD → VM → Test）、forward/backward matrix、orphan-detection rules 和 IEC 62304 audit checks 都是 generic，适用于任何遵循 artifact layout 的 medical-device project。Safety-class triggers 来自 `domain-knowledge/safety-rules.md`，每个 team 都可以自定义 — 无需编辑此文件。Team-specific test-ID conventions、additional audit checklists 或 regulatory mapping（例如 FDA 510(k)、EU MDR sections）应放在 `references/` files 或 `/memories/repo/` 中。
