# Commit Log Template

code review 期间生成 commit messages 的 templates 和 conventions。

---

## Format

```
<type>(<scope>): <subject>

<body>

<footer>
```

## Type Prefixes

| 类型 | 何时使用 |
|------|-------------|
| `fix` | A bug fix |
| `feat` | A new feature |
| `refactor` | Code restructuring without behavior change |
| `chore` | Build, tooling, config, or dependency updates |
| `docs` | Documentation-only changes |
| `test` | Adding or updating tests |
| `style` | Formatting, whitespace, or code style (no logic change) |
| `perf` | Performance improvement |

## Scope

Scope 标识受影响的 module 或 component。使用 project 或 folder name：

- `iWorkflow`, `Simulator`, `ThreeDSurview`, `VolumeEngine`
- `DirectResultPipeline`, `CT_SW_Common`, `MIPPP`
- `MIC`, `MIA`, `MIF`, `PPT`
- 使用 nested scopes 提高精度：`iWorkflow/Simulator`

## Subject Line Rules

- **Maximum 72 characters**（type + scope + colon + space + subject）
- **Imperative mood**："fix crash"，不要写 "fixed crash" 或 "fixes crash"
- **Lowercase first letter**
- **No period at the end**
- **Be specific**：写 "fix null reference in MPR slice calculation"，不要写 "fix bug"

## Body Rules

- 与 subject 用 blank line 分隔
- 解释**what** changed 和 **why**，而不是 **how**
- 每行 wrap at 80 characters
- 多项变更使用 bullet points
- bug fixes 引用 root cause
- features 引用 design intent

## Footer Rules

- 引用 issue/ticket IDs：`Fixes: #1234` 或 `Ref: JIRA-5678`
- 标记 breaking changes：`BREAKING CHANGE: <description>`
- Co-authors：`Co-authored-by: Name <email>`

---

## Examples

### Bug Fix

```
fix(Simulator): prevent null reference when data source is empty

The SimulatorDataManagement threw NullReferenceException when
initializing with an empty data source collection. Added null
check before accessing the first element.

Root cause: DataSources list was not validated before indexing.

Fixes: #4521
```

### Feature

```
feat(iWorkflow): add MPR orientation reset button

Added a reset button to the MPR toolbar that restores the default
orientation for all three planes. The button is enabled only when
the current orientation differs from the default.

- Added ResetOrientationCommand to MprViewModel
- Added toolbar button with binding in MprView.xaml
- Added unit test for orientation reset logic

Ref: FEAT-892
```

### Refactor

```
refactor(DirectResultPipeline): extract stage validation into dedicated class

Moved validation logic from PipelineRunner.ExecuteStages() into
a new StageValidator class to reduce cyclomatic complexity and
improve testability.

No behavioral change.
```

### Multi-file Change

```
fix(MIC/VolumeEngine): correct voxel spacing in oblique reconstruction

Updated the spacing calculation for oblique slices to account for
non-isotropic voxel dimensions. Previously, the calculation assumed
isotropic spacing, causing distortion in oblique MPR views.

- Fixed spacing formula in VolumeReconstructor.cs
- Updated unit tests with non-isotropic test data
- Rebuilt shared DLL to MIPPP\Bin

Fixes: #3087
```

---

## Multi-Commit Guidance

如果变更很大，考虑拆分为聚焦 commits：

1. **Refactoring commit**：structural changes with no behavior change
2. **Fix/feature commit**：实际 logic change
3. **Test commit**：new 或 updated tests（如果较小，可与 fix/feat 合并）

每个 commit 都应能独立 build 并 pass tests。
