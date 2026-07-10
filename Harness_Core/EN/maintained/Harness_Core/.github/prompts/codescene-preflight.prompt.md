---
name: CodeScene Preflight
description: 'Pre-delivery Code Health self-check. Review changed C# files for CodeScene issues before PR submission.'
agent: 'agent'
---

# CodeScene Preflight Check

Review the changed C# files in this workspace for potential CodeScene Code Health issues.

## Instructions

1. Identify all recently changed or staged `.cs` files.
2. For each file, check for:
   - Functions with cyclomatic complexity > 9
   - Nesting depth > 2 levels
   - Bumpy Road (multiple nested conditional blocks in one function)
   - Primitive Obsession (high ratio of primitive parameters)
   - String Heavy Arguments
   - Brain Methods (large + complex + deeply nested)
   - Overall module mean complexity > 4.0
3. Report findings in a table: File | Function | Issue | Severity | Suggested Fix.
4. Do NOT auto-fix. This is review-only.
5. Mark items that likely need `@codescene(disable:"...")` as residual risk with rationale.
6. Remind that CodeScene PR review is the final arbiter.

## References

- Use constraints from [codescene-csharp.instructions.md](../instructions/codescene-csharp.instructions.md)
- Use thresholds from [thresholds](../skills/codescene-health/references/thresholds.md)
- Use fix patterns from [refactoring-patterns](../skills/codescene-health/references/refactoring-patterns.md)
- Use common issues from [common-issues](../skills/codescene-health/references/common-issues.md)
