# Which File To Edit

添加或修改 team-level TICS guidance 时使用此表。

| If you want to add... | Edit this file type | Location | Why this is the right place | Example in this repo |
|---|---|---|---|---|
| 每当编辑 C# files 时都应自动应用的稳定规则 | Repository instruction | `.github/instructions/*.instructions.md` | Instructions 通过 `applyTo` 自动附加，并塑造日常 code generation | [tics-csharp.instructions.md](../../instructions/tics-csharp.instructions.md) |
| 用于 review 和 repair 的 multi-step TICS workflow | Skill entry file | `.github/skills/<name>/SKILL.md` | Skill 是 on-demand workflow entry point，并告诉 agent 何时以及如何运行 | [SKILL.md](../SKILL.md) |
| 很长的 rule list、official summary、examples 或 rationale，会让 entry files 过重 | 参考文件 | `.github/skills/<name>/references/*.md` | Reference files 将详细知识移出 always-loaded entry files | [common-rules.md](./common-rules.md) |
| 员工可在 handoff 前通过 chat 运行的 manual pre-delivery self-check action | Prompt file | `.github/prompts/*.prompt.md` | Prompts 是单一用途、用户触发的 task templates | [tics-preflight.prompt.md](../../prompts/tics-preflight.prompt.md) |
| 可分享的 onboarding 或内部演示 explainer / visual aid | Asset file | `.github/skills/<name>/assets/*` | Assets 是团队引用的持久 supporting materials | [tics-customization-map.svg](../assets/tics-customization-map.svg) |
| 新的 language-specific always-on rule set | Another instruction file with a narrower `applyTo` | `.github/instructions/*.instructions.md` | 隔离语言或文件夹规则，使其只在相关场景 auto-load | Example: create `tics-xaml.instructions.md` for `**/*.xaml` |
| 应自动 block 或 rewrite behavior 的 deterministic enforcement | Hook file | `.github/hooks/*.json` | Hooks 用于 enforcement，而不是 guidance；仅在团队接受 hard automation 时使用 | 此 repo 尚未添加 |

## Quick Decisions

- 如果规则应在常规 C# 编辑期间自动生效，修改 repository instruction。
- 如果内容说明如何执行 TICS task，修改 skill。
- 如果内容是 detailed reference material，新增或更新 reference file。
- 如果内容是一键 chat action，供员工触发，新增 prompt。
- 如果内容是 diagram 或 communication aid，新增 asset。
- 如果内容必须 deterministic enforcement，仅在团队达成一致后引入 hook。

## Practical Rule Of Thumb

- 保持 `SKILL.md` 简短并聚焦 workflow。
- 保持 `.instructions.md` files 简短并聚焦 constraints。
- 将 long catalogs、examples 和 summaries 放入 `references/`。
- 将 reusable visuals 放入 `assets/`。
- 将 employee-triggered checklists 放入 `.github/prompts/`。
