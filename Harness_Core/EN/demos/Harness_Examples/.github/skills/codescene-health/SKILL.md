---
name: codescene-health
description: 'Review and fix C# code against CodeScene Code Health findings. Use when handling CodeScene reports, code health scores, PR quality gate failures, or issues like Primitive Obsession, Complex Method, Bumpy Road, Brain Method, String Heavy Arguments, nested complexity, or cyclomatic complexity.'
argument-hint: 'Paste the CodeScene finding, code health report, or changed C# files'
user-invocable: true
---

# CodeScene Code Health

Use this skill when a change needs to satisfy CodeScene Code Health checks for C# code in this repository.

## When to Use

- You have a CodeScene code health report or PR quality gate failure.
- You see findings like "Primitive Obsession", "Complex Method", "Bumpy Road Ahead", "Brain Method", "String Heavy Function Arguments", or "Nested Complexity".
- You want to improve a file's Code Health score (target: Green ≥ 8.0).
- You are preparing a C# change and want to prevent code health decline.

## Preferred Inputs

Provide one of the following when possible:

- A CodeScene finding or PR review excerpt
- The changed C# files or specific function names
- A code health score report
- A specific code smell name and affected function

If no specific finding is supplied, inspect the changed files for the common issues documented in [common issues](./references/common-issues.md).

## Default Priority Policy

- **Critical (must-fix)**: Any finding that causes the file to drop from Green to Yellow/Red, or triggers a "Declining Code Health" alert.
- **High**: Complex Method (CC > 9), Brain Method, Bumpy Road with > 1 block.
- **Medium**: Primitive Obsession (> 30%), String Heavy Arguments (> 39%), Overall Complexity (mean > 4).
- **Low**: Module-level smells (Low Cohesion, Large File) unless the file is a hotspot.

See [thresholds reference](./references/thresholds.md) for all default threshold values.

## Workflow

1. Identify the specific code smell and affected function/module.
2. Read the local code to understand the root cause — is it genuinely complex or poorly structured?
3. Apply the appropriate refactoring pattern from [refactoring patterns](./references/refactoring-patterns.md).
4. Verify that the fix reduces complexity/nesting without changing behavior.
5. If the code truly cannot be refactored now, suggest a `@codescene(disable:"...")` directive with documented rationale.
6. Report what was fixed, new estimated complexity, and whether a CodeScene rerun is needed.

## Guardrails

- Do not change public API signatures unless explicitly requested.
- Do not extract methods purely for metric gaming — each extracted method must have a clear single responsibility.
- Do not use `@codescene(disable-all)` in new code.
- Do not refactor unrelated code outside the reported finding.
- Treat the local fix as a pre-check. The CodeScene PR review remains the source of truth.

## Expected Output

- A minimal refactoring that resolves the reported code smell
- Before/after complexity metrics where applicable
- A short note on any residual risk or items needing CodeScene rerun

## Team Assets

- [Common issues reference](./references/common-issues.md)
- [Thresholds reference](./references/thresholds.md)
- [Refactoring patterns](./references/refactoring-patterns.md)

## Portability Note

This skill is **team-portable**. The code-smell catalog (Primitive Obsession, Complex Method, Bumpy Road, Brain Method, etc.), refactoring patterns, and priority policy are generic CodeScene concepts that apply to any C# codebase. Team-specific thresholds, project-specific refactoring exemplars, or repository hotspots should be added to the existing `references/` files (versioned) or recorded in `/memories/repo/` (per-workspace) — do not hardcode them into this SKILL.md.
