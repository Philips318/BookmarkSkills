---
name: tics-standard
description: '根据 TICS 或 TIOBE code standard findings 审查并修复 C# code。适用于处理 TICS reports、Philips C# Coding Standard 5.33 checked rules、类似 20@480 或 7@107 的 rule IDs、准备代码交付，或检查 changed C# files 中的常见 TICS issues。'
argument-hint: 'Paste the TICS report, rule IDs, or changed C# files'
user-invocable: true
---

# TICS Standard

当此 repository 中的 C# code 变更需要满足 TICS 或 TIOBE code-standard checks 时，使用此 skill。

## When to Use

- 你有 TICS report，或 `20@480`、`7@105`、`7@107`、`3@109` 等 rule IDs。
- 你正在准备交付 C# 变更，并希望进行聚焦 pre-check。
- 你需要用于结构性 static-analysis findings 的 review-and-fix workflow。
- 你希望 agent 根据团队 common TICS patterns 检查 changed C# files。

## Preferred Inputs

尽可能提供以下任一项：

- TICS report excerpt
- Changed C# files
- Build 或 static-analysis log
- 被标记的具体 symbol、class 或 method

如果未提供 rule list，只检查 changed files，并优先处理 [common rules](./references/common-rules.md) 中记录的 common rules。

## Default Priority Policy

- 自动应用的 instruction file（`tics-csharp.instructions.md`）在日常编辑中将 checked levels 1 through 5 作为 must-fix。
- 当此 skill 用于 pre-delivery review 或 TICS report triage 时，覆盖完整 checked levels 1 through 7，作为 must-fix。
- 将 checked level 8 视为 best-effort：当 issue 位于 touched slice 且变更局部、成本低时修复。
- 将 checked levels 9 and 10 视为低优先级，除非用户明确要求，或 TICS/CI 已在 changed code 上报告它们。
- 如果 TICS 已经在 touched code 中报告 level 8 through 10 issue，即使优先级低也要修复。

### Rule 4@101 — Copyright Header（适用于 NEW 和 non-trivially MODIFIED files）

以下情况需要 Philips copyright header：

1. **每个新的 `.cs` file** — 不可协商。
2. **每个当前没有 header 且被 non-trivial 修改的 `.cs` file**（任一情况：新增 method、新增 field、新增 using、变更 type signature、> 5 added LOC）。修改者负责在同一变更中添加 header — 不要留给后续 PR。

这**不是** "new-files-only" rule。当没有 header 的 `.cs` file 被非平凡修改，并且变更后仍缺少 header 时，reviewers 必须标记 Should-Fix。此规则是在 PipelineEnvironmentInfoLogger dogfood 后补充的，当时 `App.xaml.cs` 被修改但没有 header，遗漏只在 code review 中暴露。

checked high-priority rule list 见 [priority summary](./references/philips-csharp-checked-level1-7.md)。

## Team Assets

- [Customization map](./assets/tics-customization-map.svg)
- [Customization map zh-CN](./assets/tics-customization-map.zh-CN.svg)
- [Asset role summary zh-CN](./assets/tics-assets-role-summary.zh-CN.svg)
- [Activation flow zh-CN](./assets/tics-activation-usage-flow.zh-CN.svg)
- [Rule placement decision table](./references/which-file-to-edit.md)
- [Priority summary](./references/philips-csharp-checked-level1-7.md)

## Related Skills

- [CodeScene Code Health](../codescene-health/SKILL.md) — 用于 Code Health findings（Primitive Obsession、Complex Method、Bumpy Road 等）

## Workflow

1. 从最窄的 failing rule、file 或 symbol 开始。
2. 只读取形成一个关于 reported violation 的可证伪假设所需的 local code。
3. 使用保持行为不变的最小变更修复 root cause。
4. 应用 [common rules](./references/common-rules.md) 中的 repository common TICS patterns，以及 [priority summary](./references/philips-csharp-checked-level1-7.md) 中的 priority policy。
5. 第一次 substantive edit 后，对 touched files 运行 focused validation。
6. 报告处理了哪些 rules、还有什么未验证，以及是否仍需要 TICS rerun。

## Guardrails

- 不要将 scope 扩大到无关 TICS findings。
- 除非明确要求，否则不要执行 broad namespace renames。
- 如果 CI paths 与 local paths 不同，在继续编辑前检查 source-root mismatch。
- 将 local fix 视为 pre-check。TICS 或 CI rerun 仍是 source of truth。

## Expected Output

- 针对 reported rule 的最小 code fix
- 对 touched files 的 focused validation
- 关于 residual risk 或 CI mismatch 的简短说明

## Portability Note

此 skill 在 Philips / TIOBE-TICS environments 内具备 **team-portable** 性。checked-level priority policy、rule-ID format（`20@480` 等）和 refactoring guidance 遵循 Philips C# Coding Standard，该标准在各业务单元中统一。Team-specific items — 实际 project rule overrides、custom suppressions、CI paths 到 local paths 的映射 — 应放在 [`references/`](./references/)（versioned）或 `/memories/repo/`（per-workspace）中，不要硬编码到此 SKILL.md。
