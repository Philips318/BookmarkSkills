---
name: CodeScene Code Health
description: 'Use when creating or modifying C# files monitored by CodeScene. Enforce function complexity limits, avoid primitive obsession, prevent bumpy road patterns, and maintain healthy module cohesion.'
applyTo: "**/*.cs"
---

# CodeScene Code Health Standards

## Scope

- Prefer the narrowest refactoring that resolves the reported code health issue.
- Default must-fix: any issue that causes the file's Code Health score to drop below Green (≥ 8.0).
- Declining Code Health alerts are high priority — never let existing code get worse.
- PR quality gate failures must be resolved before merge.

## Function-Level Constraints

- Keep cyclomatic complexity per function ≤ 9.
- Keep function body short. Prefer ≤ 20 logical statements; never exceed 50 lines without justification.
- Avoid Primitive Obsession: if a function takes more than 2 primitive parameters of the same logical domain, encapsulate them into a value object, record, or DTO.
- Avoid String Heavy Arguments: do not pass more than 2 raw strings to a function when they represent distinct domain concepts. Introduce typed wrappers.
- Functions must have a single level of abstraction. Do not mix high-level orchestration with low-level details.
- Do not create Brain Methods: a function should not combine high complexity + large size + deep nesting + central coupling.

## Implementation Constraints

- Avoid Bumpy Road: each function should contain at most one block of nested conditional logic (nesting ≥ 2). Extract additional blocks into their own methods.
- Limit nesting depth to 2 levels. Use guard clauses (early return) to flatten deeply nested code.
- Keep complex conditionals simple: if a branch condition uses more than 2 logical operators (&&/||), extract it into a named boolean method or variable.
- Do not duplicate assertion blocks in tests. Extract shared assertions into helper methods.

## Module-Level Constraints

- Keep module mean cyclomatic complexity ≤ 4.0 across all functions.
- Watch for Low Cohesion: a class should have one clear responsibility. If LCOM4 is high, consider splitting.
- Avoid God Class: a file should not combine many functions + large LoC + Brain Methods.
- Keep file size reasonable. Files exceeding 500 lines deserve review for split opportunities.

## Refactoring Patterns

- For Primitive Obsession / String Heavy: introduce `record struct` value objects or dedicated parameter objects.
- For Bumpy Road / Nested Complexity: use Extract Method to isolate each logical block.
- For Complex Method: apply guard clauses, strategy pattern, or polymorphism to replace large switch/if chains.
- For Brain Class: apply Single Responsibility Principle — split into focused collaborating classes.

## CodeScene Directives

- Only use `// @codescene(disable:"...")` for justified legacy code that cannot be refactored now.
- Always document the rationale on the same line when using a directive.
- Never use `disable-all` in new code.

## Validation

- After editing, verify that the change does not increase cyclomatic complexity or nesting depth.
- Call out anything that still needs a CodeScene PR review or full analysis rerun.
