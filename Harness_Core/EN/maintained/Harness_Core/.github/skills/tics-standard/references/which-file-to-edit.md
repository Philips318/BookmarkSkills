# Which File To Edit

Use this table when adding or changing team-level TICS guidance.

| If you want to add... | Edit this file type | Location | Why this is the right place | Example in this repo |
|---|---|---|---|---|
| A stable rule that should automatically apply whenever C# files are edited | Repository instruction | `.github/instructions/*.instructions.md` | Instructions are auto-attached by `applyTo` and shape day-to-day code generation | [tics-csharp.instructions.md](../../instructions/tics-csharp.instructions.md) |
| A multi-step TICS workflow for review and repair | Skill entry file | `.github/skills/<name>/SKILL.md` | A skill is the on-demand workflow entry point and tells the agent when and how to run it | [SKILL.md](../SKILL.md) |
| A long rule list, official summary, examples, or rationale that would make the entry files too heavy | Reference file | `.github/skills/<name>/references/*.md` | Reference files keep detailed knowledge out of the always-loaded entry files | [common-rules.md](./common-rules.md) |
| A manual pre-delivery self-check action that employees can run in chat before handoff | Prompt file | `.github/prompts/*.prompt.md` | Prompts are single-purpose, user-invoked task templates | [tics-preflight.prompt.md](../../prompts/tics-preflight.prompt.md) |
| A shareable explainer or visual aid for onboarding or internal presentations | Asset file | `.github/skills/<name>/assets/*` | Assets are durable supporting materials referenced by the team | [tics-customization-map.svg](../assets/tics-customization-map.svg) |
| A new language-specific always-on rule set | Another instruction file with a narrower `applyTo` | `.github/instructions/*.instructions.md` | Keep language or folder rules isolated so only the relevant ones auto-load | Example: create `tics-xaml.instructions.md` for `**/*.xaml` |
| Deterministic enforcement that should block or rewrite behavior automatically | Hook file | `.github/hooks/*.json` | Hooks are for enforcement, not guidance; use only when the team accepts hard automation | Not added yet in this repo |

## Quick Decisions

- If the rule should be automatic during normal C# editing, change the repository instruction.
- If the content explains how to perform a TICS task, change the skill.
- If the content is detailed reference material, add or update a reference file.
- If the content is a one-click chat action for employees, add a prompt.
- If the content is a diagram or communication aid, add an asset.
- If the content must enforce behavior deterministically, introduce a hook only after team agreement.

## Practical Rule Of Thumb

- Keep `SKILL.md` short and workflow-focused.
- Keep `.instructions.md` files short and constraint-focused.
- Put long catalogs, examples, and summaries in `references/`.
- Put reusable visuals in `assets/`.
- Put employee-triggered checklists in `.github/prompts/`.