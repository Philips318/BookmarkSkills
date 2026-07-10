# Repository-Specific Pitfalls

> **This file is a placeholder. Each team using the `impact-analysis` skill should customize it with their own repository's hard-won lessons.**
>
> The skill itself is team-portable; the items below are the **kind** of facts to record, not the actual facts of any specific codebase.

## How to use this file

Whenever an impact analysis misses something and causes a regression, **add a row here** so the next analyst catches it.

Format each entry as:

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

If your workspace uses VS Code Copilot, additional repo-specific notes may live in `/memories/repo/` (per-machine, per-workspace). The agent will consult both this file and the memory store. If a recurring pitfall is recorded only in `/memories/repo/`, consider promoting it here so the whole team benefits.
