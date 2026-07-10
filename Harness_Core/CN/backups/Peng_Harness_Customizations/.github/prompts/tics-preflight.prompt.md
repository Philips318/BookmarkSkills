---
name: TICS Preflight
description: "对当前 changed C# files 运行交付前 TICS self-check。在 handoff、PR 或 commit 前使用，用于先获取 findings 而不是直接改代码。"
argument-hint: "Optional: paste rule IDs, files, or scope to prioritize"
agent: "agent"
---

审查当前 workspace changes 的 TICS readiness。

使用以下 repository assets 作为 policy 和 scope 的来源：

- [Repository instructions](../instructions/tics-csharp.instructions.md)
- [Common rules](../skills/tics-standard/references/common-rules.md)
- [Checked levels 1-7 summary](../skills/tics-standard/references/philips-csharp-checked-level1-7.md)
- [Rule placement decision table](../skills/tics-standard/references/which-file-to-edit.md)

Default behavior：

1. 除非用户要求更大范围，否则只检查当前 changed C# files。
2. 优先处理 Philips C# checked levels 1 through 7。
3. 除非 touched code 已经报告 levels 8 through 10，否则将其视为低优先级。
4. 默认 review-only。除非用户明确要求修复 findings，否则不要编辑文件。
5. 对 touched files 运行可用的最窄验证。

Output format：

1. Findings first，按 severity 和 risk 排序。
2. 对每个 finding，引用具体 file 和相关 rule 或 rule family。
3. 然后列出 residual risks 或 unverified areas。
4. 如果没有 findings，明确说明，并提及剩余 validation gap。

保持 scope 紧凑、实用，并限定在 delivery readiness。
