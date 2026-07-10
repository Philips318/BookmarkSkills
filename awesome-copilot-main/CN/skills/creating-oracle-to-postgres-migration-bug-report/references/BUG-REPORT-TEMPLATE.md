# Bug报告模板

在创建Oracle-to-PostgreSQL迁移缺陷的bug报告时使用此模板。

##文件格式```
BUG_REPORT_<DescriptiveSlug>.md
```
模板结构```markdown
# Bug Report: <Title>

**Status:** ✅ RESOLVED | ⛔ UNRESOLVED | ⏳ IN PROGRESS
**Component:** <High-level component/endpoint and key method(s)>
**Test:** <Related automated test names>
**Severity:** Low | Medium | High | Critical

---

## Problem

<Observable incorrect behavior. State expected behavior (Oracle baseline)
versus actual behavior (PostgreSQL). Be specific and factual.>

## Scenario

<Ordered steps to reproduce the defect. Include:
1. Prerequisites and seed data
2. Exact operation or API call
3. Expected result (Oracle)
4. Actual result (PostgreSQL)>

## Root Cause

<Minimal, concrete technical cause. Reference the specific Oracle/PostgreSQL
behavioral difference (e.g., empty string vs NULL, type coercion strictness).>

## Solution

<Changes made or required. Be explicit about data access layer changes,
tracking flags, and any client code modifications. Note whether changes
are already applied or still needed.>

## Validation

<Bullet list of passing tests or manual checks that confirm the fix:
- Re-run reproduction steps on both Oracle and PostgreSQL
- Compare row/column outputs
- Check error handling parity>

## Files Modified

<Bullet list with relative file paths and short purpose for each change:
- `src/DataAccess/FooRepository.cs` — Added explicit NULL check for empty string parameter>

## Notes / Next Steps

<Follow-ups, environment caveats, risks, or dependencies on other fixes.>
```
##状态值

|状态|含义||--------|---------|
|✅已解决|缺陷已修复并验证|
|⛔未解决|缺陷尚未解决|
|⏳进行中|缺陷正在调查或修复中|

##样式规则

-保持措辞简洁和真实
-始终使用现在时或过去时
-对于步骤和验证，更喜欢使用项目符号和编号列表
明确地指出数据层的细微差别（跟踪、填充、约束）
-保持现有的runtime/language版本；避免投机性修复
-包括最小的SQL摘录和日志作为证据；省略敏感数据