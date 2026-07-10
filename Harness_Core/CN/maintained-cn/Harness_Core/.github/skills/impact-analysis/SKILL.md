---
name: impact-analysis
description: '分析 requirement change 对 existing modules、interfaces 和 data flows 的影响。读取 codebase 以识别 affected components、upstream/downstream dependencies 和 regression risk。为 Architecture 和 Review stages 产出 impact analysis report。'
argument-hint: 'Provide a requirement description or change request, and the target module/project name'
user-invocable: true
---

# Impact Analysis

使用此 skill 在 implementation 开始前评估 new requirement 或 existing feature change 的 ripple effect。

## When to Use

- requirements 结构化后 — architecture design 前。
- 修改 existing feature 时 — "what else could break?"
- 对 IEC 62304 Class B/C — impact analysis 是 mandatory。
- Three Amigos 前 — 把 dependency awareness 带入讨论。

## Preferred Inputs

- **Requirement or change description** — 正在添加/修改/删除什么。
- **Target module** — 例如 "MIA Tissue Management"、"DirectResultPipeline"。
- **Safety class** — A/B/C（Class B/C 需要更 thorough analysis）。

## Analysis Process

### Step 1: Identify Target Scope

读取 requirement 并确定：
- 哪些 class(es) / interface(s) 会被创建或修改？
- 哪个 namespace / assembly 受影响？

### Step 2: Dependency Discovery

搜索 codebase 以查找：

| Dependency Type | How to Find |
|----------------|-------------|
| **Direct callers** | Find usages of the target class/method |
| **Interface implementors** | Find all implementations of the modified interface |
| **DLL consumers** | Check HintPath references in .csproj files |
| **Event subscribers** | Find `+=` handlers for modified events |
| **Configuration** | Check .config files referencing the module |
| **DICOM tags** | Check if DICOM tag handling is affected |

### Step 3: Categorize Impact

对每个 affected component 分类：

| Impact Level | 准则 |
|-------------|---------|
| **Direct** | Code must change to compile/work |
| **Indirect** | Behavior may change, needs verification |
| **Potential** | Risk of subtle regression, needs review |

### Step 4: Risk Assessment

| Risk Factor | 低 | 中 | 高 |
|------------|-----|--------|------|
| # of affected modules | 1-2 | 3-5 | 6+ |
| Cross-assembly impact | 否 | Same solution | Cross-solution |
| 患者数据路径 | 否 | 间接 | 直接 |
| UI impact | 否 | Display only | Input/interaction |
| Shared DLL (MIPPP\Bin) | 否 | Read only | Write/rebuild |

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

完整 search-pattern cookbook（source / build-graph / config / cross-solution / dynamic）见 [dependency-discovery.md](./references/dependency-discovery.md)。

Condensed checklist：

1. **Source-code references** — class name、method calls、interface implementations、event handlers、typeof/nameof、DI registrations
2. **Build-graph references** — `ProjectReference`、`PackageReference`、`HintPath`、`packages.config`
3. **Configuration** — `*.config`、`appsettings*.json`、feature flags
4. **Cross-solution** — 在所有 workspace solutions 中搜索 published artifact names，检查 mirrored/forked projects
5. **Dynamic dependencies** — reflection、serialization、resource lookups、DICOM tag handling、T4 / code generators

## Past Pitfalls (repository-specific)

见 [repo-specific-pitfalls.md](./references/repo-specific-pitfalls.md)。该文件是一个包含 generic example pitfalls 的 **placeholder** — 每个 team 应在 regression 后用自己的 hard-won lessons 自定义它。

此外，如果你的 VS Code Copilot workspace 有记录 past incidents 的 `/memories/repo/` notes，也应把它们作为第二来源查阅。

## Quality Rules

完整列表见 [impact-analysis-process.md § Output Quality Checklist](./references/impact-analysis-process.md#output-quality-checklist)。关键规则：

- 每个 "Direct" impact component 都必须有具体 change description。
- Cross-assembly impacts 必须标出 DLL rebuild requirements。
- Patient data path impacts 无论 scope 大小都必须 escalate。
- Overall Risk 使用 **max** dimension，而不是 average。
- 如果 requirements 在 initial analysis 后发生变化，analysis 必须更新。

## Portability Note

此 skill 设计为 **team-portable**。4-step process、dependency-discovery patterns 和 risk dimensions 都是 generic，适用于任何 .NET / C# codebase。Repository-specific facts（哪个 DLL 位于哪里、哪个 module 拥有哪些 dataset、哪个 build configuration 发布哪个 output）**不得**硬编码到此 skill 中。请记录在：

- `references/repo-specific-pitfalls.md` — versioned in git，whole team 可见
- `/memories/repo/` — VS Code Copilot per-workspace memory，per-machine

当与另一个 team 共享此 skill 时，上面两个文件都会替换为该 team 的内容 — skill 本身仍然有效。
