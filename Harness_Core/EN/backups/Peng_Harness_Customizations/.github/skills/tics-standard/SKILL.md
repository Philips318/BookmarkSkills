---
name: tics-standard
description: 'Review and fix C# code against TICS or TIOBE code standard findings. Use when handling TICS reports, Philips C# Coding Standard 5.33 checked rules, rule IDs like 20@480 or 7@107, preparing code for delivery, or checking changed C# files for common TICS issues.'
argument-hint: 'Paste the TICS report, rule IDs, or changed C# files'
user-invocable: true
---

# TICS Standard

Use this skill when a change needs to satisfy TICS or TIOBE code-standard checks for C# code in this repository.

## When to Use

- You have a TICS report or rule IDs such as `20@480`, `7@105`, `7@107`, or `3@109`.
- You are preparing a C# change for delivery and want a focused pre-check.
- You need a review-and-fix workflow for structural static-analysis findings.
- You want the agent to inspect changed C# files against the team's common TICS patterns.

## Preferred Inputs

Provide one of the following when possible:

- A TICS report excerpt
- The changed C# files
- A build or static-analysis log
- A specific symbol, class, or method that was flagged

If no rule list is supplied, inspect only the changed files and prioritize the common rules documented in [common rules](./references/common-rules.md).

## Default Priority Policy

- The auto-applied instruction file (`tics-csharp.instructions.md`) enforces checked levels 1 through 5 as must-fix during daily editing.
- This skill covers the full checked levels 1 through 7 as must-fix when invoked for pre-delivery review or TICS report triage.
- Treat checked level 8 as best-effort: fix it when the issue is in the touched slice and the change is local and cheap.
- Treat checked levels 9 and 10 as low priority unless the user explicitly asks for them or TICS or CI already reports them on the changed code.
- If TICS already reports a level 8 through 10 issue in the touched code, fix it even if it is low priority.

### Rule 4@101 — Copyright Header (applies to NEW and non-trivially MODIFIED files)

The Philips copyright header is required on:

1. **Every new `.cs` file** — non-negotiable.
2. **Every modified `.cs` file that currently has no header**, when the modification is non-trivial (any of: new method, new field, new using, changed type signature, > 5 added LOC). The modifier is responsible for adding the header in the same change — do not leave it for a follow-up PR.

This is **not** a "new-files-only" rule. Reviewers must flag a Should-Fix when a header-less `.cs` file is modified non-trivially and the header is still missing after the change. Added after the PipelineEnvironmentInfoLogger dogfood, where `App.xaml.cs` was modified without a header and the omission only surfaced at code review.

See [priority summary](./references/philips-csharp-checked-level1-7.md) for the checked high-priority rule list.

## Team Assets

- [Customization map](./assets/tics-customization-map.svg)
- [Customization map zh-CN](./assets/tics-customization-map.zh-CN.svg)
- [Asset role summary zh-CN](./assets/tics-assets-role-summary.zh-CN.svg)
- [Activation flow zh-CN](./assets/tics-activation-usage-flow.zh-CN.svg)
- [Rule placement decision table](./references/which-file-to-edit.md)
- [Priority summary](./references/philips-csharp-checked-level1-7.md)

## Related Skills

- [CodeScene Code Health](../codescene-health/SKILL.md) — for Code Health findings (Primitive Obsession, Complex Method, Bumpy Road, etc.)

## Workflow

1. Start from the narrowest failing rule, file, or symbol.
2. Read only the local code needed to form one falsifiable hypothesis about the reported violation.
3. Fix the root cause with the smallest behavior-preserving change.
4. Apply the repository's common TICS patterns from [common rules](./references/common-rules.md) and the priority policy from [priority summary](./references/philips-csharp-checked-level1-7.md).
5. After the first substantive edit, run focused validation for the touched files.
6. Report which rules were addressed, what remains unverified, and whether a TICS rerun is still required.

## Guardrails

- Do not widen scope to unrelated TICS findings.
- Do not perform broad namespace renames unless explicitly requested.
- If CI paths and local paths differ, check for source-root mismatch before more edits.
- Treat the local fix as a pre-check. The rerun of TICS or CI remains the source of truth.

## Expected Output

- A minimal code fix for the reported rule
- Focused validation on touched files
- A short note on any residual risk or CI mismatch

## Portability Note

This skill is **team-portable** within Philips / TIOBE-TICS environments. The checked-level priority policy, rule-ID format (`20@480` etc.), and refactoring guidance follow the Philips C# Coding Standard, which is standardized across business units. Team-specific items — actual project rule overrides, custom suppressions, mapping of CI paths to local paths — belong in [`references/`](./references/) (versioned) or `/memories/repo/` (per-workspace), not in this SKILL.md.
