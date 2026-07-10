---
applyTo: "Build/**,*.yml"
---

# Build & CI Instructions

When working with build scripts, CI pipelines, or packaging:

## CI/CD

- Pipelines are Azure DevOps YAML
- PR pipeline: `Build/CI/PR_Pipeline.yml`
- Integration pipeline: `Build/CI/Integration_Pipeline.yml`
- Variables are factored out into `Variables_*.yml` files
- Pipeline templates come from the shared `CT_CICD` repository

## Compile Scripts

- Compile scripts live in `Build/Compile/` — owned by the dev team
- Do not mix CI orchestration logic into compile scripts

## Packaging

- NuGet specs: `Build/Pkg/Nuget/`
- MSI packaging: `Build/Pkg/MSI/` (WiX-based)
- Version management: `Build/Pkg/Version.Config`, `Build/Version/Main.Config`
- Each repo produces 4 packages: `{repo}Inf.pkg`, `{repo}Impl.pkg`, `{repo}PostActions.pkg`, `{repo}Tests.pkg`
- An MSI is assembled from the 3 production packages: `{repo}Inf.pkg` + `{repo}Impl.pkg` + `{repo}PostActions.pkg`

## Post-Install Actions

- `Build/Actions/Install/` — scripts run after MSI installation
- `Build/Actions/Uninstall/` — scripts run before MSI uninstallation
- `Build/Actions/Test/` — scripts to set up the development/test environment

## Output Folders (post-build copy targets)

- `Output/OutInf/` — ExtInf project binaries
- `Output/OutImpl/` — Src implementation binaries
- `Output/OutCfg/` — runtime configuration files
- `Output/OutRes/` — runtime resource files (images, databases)
- `Output/OutTests/` — unit test and module test binaries

## Versioning

Format: `{MAJOR}.{MINOR}.{PATCH}.{BUILD}`

- **MAJOR** — breaking interface change; must be updated manually
- **MINOR** — additive interface change (backward compatible); must be updated manually
- **PATCH** — implementation change; auto-incremented per nightly build when source changes
- **BUILD** — auto-incremented on every build; reset to 0 when PATCH changes
- MAJOR.MINOR are shared between Inf and Impl versions
- All interfaces in a repo carry the same version
- Interface version must be updated whenever any interface source changes; build fails if out of sync
- Implementation MAJOR.MINOR must always match the Interface MAJOR.MINOR
- Version config files: `Build/Version/Main.Config` (MAJOR/MINOR), `Build/Pkg/Version.Config`
- `CommonVersion.cs` is auto-updated by nightly build: `ExtInf/CommonVersion/CommonVersion.cs` and `Src/CommonVersion/CommonVersion.cs`

## Quality Gates

Quality gates run in two phases: **locally before submitting** (developer responsibility) and **in the CI pipeline** (automated enforcement).

### Canonical local gate scripts (`Build/`)

Agents and developers invoke these tested wrappers instead of hand-assembling `dotnet`/`jb`/`dotcover` command lines. Each auto-resolves the `*Impl.sln` under `Src/`, bakes in the dotCover filters (`+:Philips.CT.*;-:*.Test*`) and report types, stores all raw tool outputs under `-OutputDir`, and returns a non-zero exit on any failure (meaning: fix the code/tests, not the command):

| Script | Purpose | Used by |
|---|---|---|
| `Build\Verify-Baseline.cmd` | `dotnet build` + `dotnet test --no-build` on the impl solution | developer baseline |
| `Build\Run-QualityGate.cmd -OutputDir <dir>` | build + instrumented test run (`tests.trx`, `coverage.xml`) + ReSharper (`resharper.xml`) | developer full gate |
| `Build\Build-VerificationTests.cmd -OutputDir <dir>` | build + ReSharper on `VerificationTests.sln` | test-designer |
| `Build\Run-CombinedCoverage.cmd -OutputDir <dir>` | combined dev + verification coverage (`coverage-combined.xml`) | evaluator |

The gate scripts are self-contained Windows batch (`.cmd`) files — no PowerShell, so there is no execution-policy or publisher-trust friction at runtime. They share `Build/_GateCommon.cmd` (solution resolution, the canonical dotCover filter, output-dir creation) and are the single source of truth for the canonical commands. The raw commands below are documented for reference and for the CI pipeline.

### Local pre-submit (developer runs before submitting for review)

| Tool | Command | Pass criterion |
|---|---|---|
| **ReSharper CLI** | `jb inspectcode Src\{repo}Impl.sln --output=report.xml` | Zero errors; warnings documented |
| **dotCover** | `dotnet dotcover test Src\{repo}Impl.sln --dcOutput=coverage.html --dcReportType=HTML --dcFilters="+:Philips.CT.*;-:*.Test*"` | See coverage thresholds below |

ReSharper CLI and dotCover are installed as dotnet tools:
```
dotnet tool install JetBrains.ReSharper.GlobalTools
dotnet tool install JetBrains.dotCover.CommandLineTools
```

### Coverage Thresholds (per test type)

| Test Type | Filter | Minimum Coverage | Command |
|-----------|--------|-----------------|----------|
| **Unit** | `--filter Category=Unit` | 80% statement | `dotnet dotcover test --dcFilters="+:Philips.CT.*;-:*.Test*" --filter Category=Unit` |
| **Module (BDD)** | `--filter Category=Module` | 70% statement | `dotnet dotcover test --dcFilters="+:Philips.CT.*;-:*.Test*" --filter Category=Module` |
| **Integration** | `--filter Category=Integration` | 60% statement | `dotnet dotcover test --dcFilters="+:Philips.CT.*;-:*.Test*" --filter Category=Integration` |
| **Verification** | `--filter Category=Verification` | Not a separate gate | Run as part of combined coverage |
| **Combined** | All tests (dev + verification) | 80% statement | `dotnet dotcover test Src\{repo}Impl.sln Src\VerificationTests\VerificationTests.sln --dcFilters="+:Philips.CT.*;-:*.Test*"` |

Coverage reports are generated in HTML and XML. The XML report (`--dcReportType=DetailedXML`) is used by `@dev-evaluator` for per-class analysis. Combined coverage includes both developer tests (`Src/{repo}Impl.sln`) and evaluator verification tests (`Src/VerificationTests/VerificationTests.sln`).

### CI pipeline gates (automatic on submission, blocking merge)

| Tool | Purpose | Blocking threshold |
|---|---|---|
| **TICS (TIOBE)** | Coding standards, maintainability, test coverage | TQI < 8.0 on any measured characteristic blocks approval |
| **Coverity** | Static security and safety defect analysis | Any new **High** defect blocks approval; new **Medium** requires documented justification |
| **ReSharper CLI** (`jb inspectcode`) | Code style, naming, and idiom violations | Report published to pipeline; does **not** block approval (informational) |

### Pipeline step order

1. `dotnet build` — zero warnings
2. `dotnet test --filter Category=Unit` — all unit tests pass
3. `dotnet dotcover test` — coverage gate (combined ≥ 80%)
4. TICS analysis — TQI gate
5. Coverity scan — defect gate
6. ReSharper CLI — report only
7. Package and publish artifacts

### Rules

- TICS and Coverity results are **evidence artefacts** — the `@dev-evaluator` must confirm both gates passed before issuing a PASS verdict
- A suppressed or skipped quality gate is treated as a gate failure — document any waiver in the PR description and obtain team lead approval
- ReSharper suppressions (`// ReSharper disable`) in production code require an inline justification comment
- Coverity false-positive annotations (`/* coverity[...] */`) require a justification comment referencing the finding ID

## Conventions

- Do not modify pipeline templates — those live in the shared CT_CICD repo
- Version changes go through `Build/Pkg/Version.Config` or `Build/Version/Main.Config`
- PowerShell scripts must pass PSScriptAnalyzer
- Batch scripts must handle error codes properly
