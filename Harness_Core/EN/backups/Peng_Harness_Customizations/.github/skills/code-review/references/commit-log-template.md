# Commit Log Template

Templates and conventions for generating commit messages during code review.

---

## Format

```
<type>(<scope>): <subject>

<body>

<footer>
```

## Type Prefixes

| Type | When to Use |
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

The scope identifies the affected module or component. Use the project or folder name:

- `iWorkflow`, `Simulator`, `ThreeDSurview`, `VolumeEngine`
- `DirectResultPipeline`, `CT_SW_Common`, `MIPPP`
- `MIC`, `MIA`, `MIF`, `PPT`
- Use nested scopes for precision: `iWorkflow/Simulator`

## Subject Line Rules

- **Maximum 72 characters** (type + scope + colon + space + subject)
- **Imperative mood**: "fix crash" not "fixed crash" or "fixes crash"
- **Lowercase first letter**
- **No period at the end**
- **Be specific**: "fix null reference in MPR slice calculation" not "fix bug"

## Body Rules

- Separate from subject with a blank line
- Explain **what** changed and **why**, not **how**
- Wrap lines at 80 characters
- Use bullet points for multiple changes
- Reference the root cause for bug fixes
- Reference the design intent for features

## Footer Rules

- Reference issue/ticket IDs: `Fixes: #1234` or `Ref: JIRA-5678`
- Mark breaking changes: `BREAKING CHANGE: <description>`
- Co-authors: `Co-authored-by: Name <email>`

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

If the change is large, consider splitting into focused commits:

1. **Refactoring commit**: structural changes with no behavior change
2. **Fix/feature commit**: the actual logic change
3. **Test commit**: new or updated tests (can be combined with fix/feat if small)

Each commit should build and pass tests independently.
