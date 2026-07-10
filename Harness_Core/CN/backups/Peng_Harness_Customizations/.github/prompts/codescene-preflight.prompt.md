---
name: CodeScene Preflight
description: '交付前 Code Health 自检。PR 提交前审查 changed C# files 中潜在的 CodeScene 问题。'
agent: 'agent'
---

# CodeScene Preflight Check

审查此 workspace 中已变更的 C# files，识别潜在 CodeScene Code Health 问题。

## Instructions

1. 识别所有最近变更或已 staged 的 `.cs` files。
2. 对每个文件检查：
   - Functions with cyclomatic complexity > 9
   - Nesting depth > 2 levels
   - Bumpy Road（一个 function 中有多个 nested conditional blocks）
   - Primitive Obsession（primitive parameters 比例高）
   - String Heavy Arguments
   - Brain Methods（大 + 复杂 + 深嵌套）
   - Overall module mean complexity > 4.0
3. 用表格报告 findings：File | Function | Issue | Severity | Suggested Fix。
4. 不要自动修复。这是 review-only。
5. 将可能需要 `@codescene(disable:"...")` 的事项标为 residual risk，并给出 rationale。
6. 提醒 CodeScene PR review 是最终裁定者。

## References

- 使用 [codescene-csharp.instructions.md](../instructions/codescene-csharp.instructions.md) 中的 constraints
- 使用 [thresholds](../skills/codescene-health/references/thresholds.md) 中的 thresholds
- 使用 [refactoring-patterns](../skills/codescene-health/references/refactoring-patterns.md) 中的 fix patterns
- 使用 [common-issues](../skills/codescene-health/references/common-issues.md) 中的 common issues
