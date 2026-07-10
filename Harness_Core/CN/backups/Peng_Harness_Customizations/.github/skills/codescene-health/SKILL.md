---
name: codescene-health
description: '根据 CodeScene Code Health findings 审查并修复 C# code。适用于处理 CodeScene reports、code health scores、PR quality gate failures，或 Primitive Obsession、Complex Method、Bumpy Road、Brain Method、String Heavy Arguments、nested complexity、cyclomatic complexity 等问题。'
argument-hint: 'Paste the CodeScene finding, code health report, or changed C# files'
user-invocable: true
---

# CodeScene Code Health

当此 repository 中的 C# code 变更需要满足 CodeScene Code Health checks 时，使用此 skill。

## When to Use

- 你有 CodeScene code health report 或 PR quality gate failure。
- 你看到 "Primitive Obsession"、"Complex Method"、"Bumpy Road Ahead"、"Brain Method"、"String Heavy Function Arguments" 或 "Nested Complexity" 等 findings。
- 你想提升文件的 Code Health score（target: Green ≥ 8.0）。
- 你正在准备 C# 变更，并希望防止 code health decline。

## Preferred Inputs

尽可能提供以下任一项：

- CodeScene finding 或 PR review excerpt
- Changed C# files 或具体 function names
- Code health score report
- 具体 code smell name 和 affected function

如果未提供具体 finding，请检查 changed files 中 [common issues](./references/common-issues.md) 记录的 common issues。

## Default Priority Policy

- **Critical (must-fix)**：任何导致文件从 Green 降为 Yellow/Red，或触发 "Declining Code Health" alert 的 finding。
- **High**：Complex Method (CC > 9)、Brain Method、Bumpy Road with > 1 block。
- **Medium**：Primitive Obsession (> 30%)、String Heavy Arguments (> 39%)、Overall Complexity (mean > 4)。
- **Low**：Module-level smells（Low Cohesion、Large File），除非该文件是 hotspot。

所有默认 threshold values 见 [thresholds reference](./references/thresholds.md)。

## Workflow

1. 识别具体 code smell 和 affected function/module。
2. 读取 local code 以理解 root cause — 它是真的复杂，还是结构不佳？
3. 应用 [refactoring patterns](./references/refactoring-patterns.md) 中合适的 refactoring pattern。
4. 验证修复降低了 complexity/nesting 且不改变行为。
5. 如果代码当前确实无法重构，建议带 documented rationale 的 `@codescene(disable:"...")` directive。
6. 报告修复内容、新估算 complexity，以及是否需要 CodeScene rerun。

## Guardrails

- 除非明确要求，否则不要更改 public API signatures。
- 不要为了 metric gaming 而抽取 methods — 每个 extracted method 都必须有清晰 single responsibility。
- 不要在新代码中使用 `@codescene(disable-all)`。
- 不要重构 reported finding 之外的无关代码。
- 将 local fix 视为 pre-check。CodeScene PR review 仍是 source of truth。

## Expected Output

- 解决 reported code smell 的最小 refactoring
- 适用时提供 before/after complexity metrics
- 关于 residual risk 或需要 CodeScene rerun 项的简短说明

## Team Assets

- [Common issues reference](./references/common-issues.md)
- [Thresholds reference](./references/thresholds.md)
- [Refactoring patterns](./references/refactoring-patterns.md)

## Portability Note

此 skill 具备 **team-portable** 性。code-smell catalog（Primitive Obsession、Complex Method、Bumpy Road、Brain Method 等）、refactoring patterns 和 priority policy 是适用于任何 C# codebase 的通用 CodeScene concepts。Team-specific thresholds、project-specific refactoring exemplars 或 repository hotspots 应添加到已有 `references/` files（versioned），或记录在 `/memories/repo/`（per-workspace）中 — 不要硬编码到此 SKILL.md。
