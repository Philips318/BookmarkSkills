# Repository-Specific Pitfalls

> **This file is a placeholder. Each team using the `impact-analysis` skill should customize it with their own repository's hard-won lessons.**
>
> skill 本身是 team-portable；下面 items 是应记录 facts 的**类型**，不是任何具体 codebase 的真实 facts。

## How to use this file

每当一次 impact analysis 漏掉问题并导致 regression，就**在这里加一行**，让下一位 analyst 能抓到它。

每个 entry 使用以下 format：

```
### [SHORT-TITLE]
- **Context:** what changed
- **What was missed:** the dependency the analyst did not find
- **Why it was missed:** the search pattern that should have surfaced it
- **Rule going forward:** the check to add to future analyses
```

## Generic example entries (replace with your own)

### [DLL-PUBLISH-CHAIN]
- **Context:** Changing the public API of a library that publishes its DLL to a shared `Bin/` folder consumed by another solution.
- **What was missed:** Consumer solution still loaded the old DLL because the publishing project was not rebuilt in the right configuration.
- **Why it was missed:** Source-code grep found no caller in consumer source — but `HintPath` in consumer `.csproj` pointed at the shared DLL.
- **Rule going forward:** Always run search pattern B (HintPath) AND verify rebuild order before declaring "no impact".

### [MIRRORED-PROJECTS]
- **Context:** A class exists in `<Module>_Dev` AND a parallel variant project. Modifying one without the other causes inconsistent behavior across product variants.
- **What was missed:** Search was scoped to the Dev project only.
- **Why it was missed:** Workspace-wide search was not run; analyst assumed the Dev project is the only owner.
- **Rule going forward:** When the changed project has a parallel/mirrored sibling, search both.

### [SHARED-RESOURCE-OWNERSHIP]
- **Context:** Two viewers manipulate the same in-memory dataset. Adding a write operation in one caused a stale read in the other.
- **What was missed:** Data ownership boundary was not documented; both viewers assumed they were the sole writer.
- **Why it was missed:** No dependency edge in the code — the coupling was through a shared singleton.
- **Rule going forward:** For data-shaping changes, also map "who else holds a reference" via field/property inspection of the shared type.

### [BUILD-CHAIN-ORDER]
- **Context:** Project A depends on Project B's published output, not B's source. Modifying B without rebuilding the publish configuration left A using stale binaries.
- **What was missed:** Standard solution build was sufficient locally but not for the publishing pipeline.
- **Why it was missed:** Build configuration matrix was not inspected.
- **Rule going forward:** For changes to projects with custom publish targets, document the required rebuild sequence in the analysis output.

## Related: VS Code Copilot repo memory

如果你的 workspace 使用 VS Code Copilot，additional repo-specific notes 可能位于 `/memories/repo/`（per-machine, per-workspace）。agent 会同时查阅此文件和 memory store。如果一个 recurring pitfall 只记录在 `/memories/repo/`，请考虑把它提升到这里，让整个 team 受益。
