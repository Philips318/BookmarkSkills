---
name: impact-analysis
description: 'Analyze the impact of a requirement change on existing modules, interfaces, and data flows. Reads codebase to identify affected components, upstream/downstream dependencies, and regression risk. Produces an impact analysis report for Architecture and Review stages.'
argument-hint: 'Provide a requirement description or change request, and the target module/project name'
user-invocable: true
---

# Impact Analysis

Use this skill to assess the ripple effect of a new requirement or a change to an existing feature before implementation begins.

## When to Use

- After requirements are structured — before architecture design.
- When modifying an existing feature — "what else could break?"
- For IEC 62304 Class B/C — impact analysis is mandatory.
- Before Three Amigos — to bring dependency awareness into the discussion.

## Preferred Inputs

- **Requirement or change description** — what is being added/modified/removed.
- **Target module** — e.g., "MIA Tissue Management", "DirectResultPipeline".
- **Safety class** — A/B/C (Class B/C requires more thorough analysis).

## Analysis Process

### Step 1: Identify Target Scope

Read the requirement and determine:
- Which class(es) / interface(s) will be created or modified?
- Which namespace / assembly is affected?

### Step 2: Dependency Discovery

Search the codebase to find:

| Dependency Type | How to Find |
|----------------|-------------|
| **Direct callers** | Find usages of the target class/method |
| **Interface implementors** | Find all implementations of the modified interface |
| **DLL consumers** | Check HintPath references in .csproj files |
| **Event subscribers** | Find `+=` handlers for modified events |
| **Configuration** | Check .config files referencing the module |
| **DICOM tags** | Check if DICOM tag handling is affected |

### Step 3: Categorize Impact

For each affected component, classify:

| Impact Level | Criteria |
|-------------|---------|
| **Direct** | Code must change to compile/work |
| **Indirect** | Behavior may change, needs verification |
| **Potential** | Risk of subtle regression, needs review |

### Step 4: Risk Assessment

| Risk Factor | Low | Medium | High |
|------------|-----|--------|------|
| # of affected modules | 1-2 | 3-5 | 6+ |
| Cross-assembly impact | No | Same solution | Cross-solution |
| Patient data path | No | Indirect | Direct |
| UI impact | No | Display only | Input/interaction |
| Shared DLL (MIPPP\Bin) | No | Read only | Write/rebuild |

## Output Structure

```markdown
# Impact Analysis — [Feature/Change Name]

## Change Summary
- **What**: [brief description of change]
- **Where**: [target module / namespace]
- **Safety Class**: [A|B|C]

## Affected Components

| # | Component | File/Namespace | Impact Level | Change Required |
|---|-----------|---------------|-------------|-----------------|
| 1 | [ClassName] | [path] | Direct | [what needs to change] |
| 2 | [ClassName] | [path] | Indirect | [verify behavior] |
| 3 | [ClassName] | [path] | Potential | [review for regression] |

## Dependency Map

[Mermaid diagram showing affected modules and their relationships]

## Risk Summary

| Dimension | Level | Notes |
|-----------|-------|-------|
| Scope breadth | Low/Med/High | N modules affected |
| Patient safety | Low/Med/High | [reasoning] |
| Regression risk | Low/Med/High | [reasoning] |
| Build impact | Low/Med/High | [DLL/solution rebuild scope] |

**Overall Risk: LOW / MEDIUM / HIGH**

## Recommendations

1. [Specific action to mitigate risk]
2. [Tests to add]
3. [Modules to monitor during review]
```

## Codebase Search Strategy

See [dependency-discovery.md](./references/dependency-discovery.md) for the full search-pattern cookbook (source / build-graph / config / cross-solution / dynamic).

The condensed checklist:

1. **Source-code references** — class name, method calls, interface implementations, event handlers, typeof/nameof, DI registrations
2. **Build-graph references** — `ProjectReference`, `PackageReference`, `HintPath`, `packages.config`
3. **Configuration** — `*.config`, `appsettings*.json`, feature flags
4. **Cross-solution** — search published artifact names across all workspace solutions, check for mirrored/forked projects
5. **Dynamic dependencies** — reflection, serialization, resource lookups, DICOM tag handling, T4 / code generators

## Past Pitfalls (repository-specific)

See [repo-specific-pitfalls.md](./references/repo-specific-pitfalls.md). That file is a **placeholder** containing generic example pitfalls — each team should customize it with their own hard-won lessons after a regression.

Additionally, if your VS Code Copilot workspace has `/memories/repo/` notes recording past incidents, consult them as a second source.

## Quality Rules

See [impact-analysis-process.md § Output Quality Checklist](./references/impact-analysis-process.md#output-quality-checklist) for the full list. Key rules:

- Every "Direct" impact component must have a specific change description.
- Cross-assembly impacts must flag DLL rebuild requirements.
- Patient data path impacts must be escalated regardless of scope.
- Overall Risk uses the **max** dimension, not average.
- The analysis must be updated if requirements change after initial analysis.

## Portability Note

This skill is designed to be **team-portable**. The 4-step process, dependency-discovery patterns, and risk dimensions are generic and apply to any .NET / C# codebase. Repository-specific facts (which DLL lives where, which module owns which dataset, which build configuration publishes which output) **must not** be hardcoded into this skill. Record them in:

- `references/repo-specific-pitfalls.md` — versioned in git, visible to the whole team
- `/memories/repo/` — VS Code Copilot per-workspace memory, per-machine

When sharing this skill with another team, both of the above files will be replaced by their team's own content — the skill itself remains valid.
