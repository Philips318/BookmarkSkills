---
name: TICS Preflight
description: "Run a pre-delivery TICS self-check on the current changed C# files. Use before handoff, PR, or commit when you want findings first instead of code edits."
argument-hint: "Optional: paste rule IDs, files, or scope to prioritize"
agent: "agent"
---

Review the current workspace changes for TICS readiness.

Use these repository assets as the source of policy and scope:

- [Repository instructions](../instructions/tics-csharp.instructions.md)
- [Common rules](../skills/tics-standard/references/common-rules.md)
- [Checked levels 1-7 summary](../skills/tics-standard/references/philips-csharp-checked-level1-7.md)
- [Rule placement decision table](../skills/tics-standard/references/which-file-to-edit.md)

Default behavior:

1. Inspect only the current changed C# files unless the user asks for a broader scope.
2. Prioritize Philips C# checked levels 1 through 7.
3. Treat levels 8 through 10 as low priority unless they are already reported on the touched code.
4. Default to review-only. Do not edit files unless the user explicitly asks to fix findings.
5. Run the narrowest validation available for the touched files.

Output format:

1. Findings first, ordered by severity and risk.
2. For each finding, cite the concrete file and the specific rule or rule family involved.
3. Then list residual risks or unverified areas.
4. If there are no findings, say so explicitly and mention any remaining validation gap.

Keep the scope tight, practical, and limited to delivery readiness.