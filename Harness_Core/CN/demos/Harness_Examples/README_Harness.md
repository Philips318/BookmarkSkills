# Harness_Enhance — 统一端到端 AI 交付 Harness（含完整示例）

这是一个可直接使用的 **AI 开发 harness**，面向 CT 医疗器械软件（IEC 62304 / ISO 14971 / ISO 13485），
适用于 VS Code + GitHub Copilot。它把两种互补方法整合为一个自主、可信且
合规的端到端交付系统：

| 部分 | 优势 |
|------|------|
| **Execution Engine** | 自主长时间运行循环、上下文隔离的生成器+评估器、BDD + 黑盒 UI 测试、活体 QMS、3 个人工门禁、并行执行 |
| **Domain & Compliance Intelligence** | DFMEA / IFU / impact / product-defect 分析，CT·DICOM·Spectral 知识，TICS & CodeScene 修复，自包含 HTML 报告 |

> 此包 **包含一个完整示例** — `WW/WC Reset` 功能 — 让你能精确看到一次完整
> 端到端运行会产出什么。若要从干净状态开始自己的工作，请参见下方 **"Reset to a clean slate"**。

## 包含内容

```
Harness_Enhance/
├─ .github/                      # The harness (18 agents · 31 skills · 12 prompts · 5 instructions)
│  ├─ copilot-instructions.md    #   unified always-loaded core
│  ├─ ASSET_CATALOG.md           #   asset → pipeline-stage map
│  ├─ agents/ skills/ prompts/ instructions/ scripts/
├─ Build/                        # Batch quality-gate scripts (auto-resolve *Impl.sln)
├─ .harness/                     # State store — POPULATED by the example run:
│  │                             #   backlogs/ww-wc-reset.json, requirements/, architecture/adr+diagrams/,
│  │                             #   specs/ (Gherkin), eval_feedback/, tool_outputs/, demo_evidence/, progress.md
├─ docs/
│  ├─ harness/                   # Project-map reference
│  ├─ qms/                       # Living IEC 62304 QMS docs — POPULATED by the example
│  └─ qms-templates/skeletons/   # Clean skeletons (use these to reset qms/)
├─ ExtInf/  Src/                 # The worked EXAMPLE: WW/WC Reset production code, tests, WPF host
└─ .vscode/                      # Subagent invocation settings
```

资产总数：**18 agents · 31 skills · 12 prompts · 5 instructions。**

### 完整示例 — WW/WC Reset

一个全新的 WPF/MVVM CT 图像显示功能（将 Window Width/Center 复位到 DICOM 默认值，IEC 62304 Class B），
由 harness 跨 4 个任务 / 17 个 BDD 场景端到端构建，最终达到 **85 个单元测试 + 17 个模块测试全绿**。
可通过以下内容了解流程：

- 需求：`.harness/requirements/ww-wc-reset-requirements.md`
- 架构：`.harness/architecture/adr/ADR-001-ww-wc-reset-design.md`（+ diagrams）
- Backlog + specs：`.harness/backlogs/ww-wc-reset.json`、`.harness/specs/ww-wc-reset/`
- 评估器结论：`.harness/eval_feedback/`
- QMS：`docs/qms/`（SwRS … MVReport）
- 会话叙事（每个阶段 + gate）：`.harness/progress.md`
- 代码 + 测试 + WPF host：`ExtInf/ImageDisplay/`、`Src/`

## 快速开始（你自己的功能）

1. 在启用 GitHub Copilot（agent mode）的 VS Code 中打开此文件夹。
2. 打开 Copilot Chat，并用你的功能 / bug / 改进请求调用 **`@orchestrator`**。
3. 在 **3 个人工 gates** 批准：(1) requirements + architecture + backlog + risk，(2) design + test design，(3) feature demo。
4. harness 会自主实现、评估（隔离）、自愈质量问题、生成风险/合规文档，并
   演示每个任务；只有当某个任务失败 3 次时才需要你再次介入。

一次性任务可使用 slash prompts（例如 `/full-pipeline`、`/dfmea-analysis`、`/code-quality`、`/tics-preflight`）。

## Reset to a clean slate

若要移除示例并重新开始：

```powershell
# from the harness root
Remove-Item Src, ExtInf -Recurse -Force
Get-ChildItem .harness\requirements, .harness\architecture\adr, .harness\architecture\diagrams, `
  .harness\specs, .harness\eval_feedback, .harness\tool_outputs, .harness\demo_evidence -Recurse -File |
  Where-Object { $_.Name -ne 'README.md' } | Remove-Item -Force
Remove-Item .harness\backlogs\ww-wc-reset.json -Force -EA SilentlyContinue
Get-ChildItem docs\qms-templates\skeletons\*.skeleton.md | ForEach-Object {
  Copy-Item $_.FullName ("docs\qms\" + ($_.Name -replace '\.skeleton\.md$','.md')) -Force }
```

（然后清理 `.harness/progress.md` 中的会话条目，保留 Templates 部分。）
另有一个不含示例的预清理模板 **`Harness_Enhance_Dist`**。

## 统一流水线

```
Analyse → Requirements Review + Safety Class → Architect (ADR) + Impact Analysis → Plan (Backlog + Gherkin)
  → ⏸ GATE 1
  → Develop (BDD) ∥ Test-Design (black-box, isolated)
  → ⏸ GATE 2
  → Evaluate (isolated) + Quality Self-Heal (TICS/CodeScene)
  → Risk & Compliance (DFMEA / IFU / QMS) → Documentation (HTML + md→docx)
  → Demonstrate → ⏸ GATE 3 → Done
```

完整的逐阶段资产映射请参见 [`.github/ASSET_CATALOG.md`](./.github/ASSET_CATALOG.md)。

## 部署到项目

harness 位于它要操作的仓库 **根目录**。可以直接打开此文件夹，或将
`.github/`、`.harness/`、`Build/`、`docs/` 和 `.vscode/` 复制到目标仓库根目录（它会操作该
仓库的 `Src/`）。然后 **Reload Window**，让 Copilot 重新扫描 customizations。

## 自定义

- 确保你的 solution 遵循 `Src\{repo}Impl.sln` 命名约定（`Build/` gate 脚本会自动解析它）。
- 如果不是 C#/.NET，请更新 `.github/copilot-instructions.md` 中的技术栈，以及 `source-code` / `build-ci` instructions。
- 将项目特定事实记录在 `.harness/` 或 `/memories/repo/` 中，并保持 skills 可在团队间移植。

## 前置条件（用于完整端到端运行）

.NET SDK / `dotnet` CLI · ReSharper CLI (`jb`) · `dotnet dotcover`（覆盖率）·（可选）`ffmpeg`（demo 视频）·
Python（QMS md→docx 导出）。