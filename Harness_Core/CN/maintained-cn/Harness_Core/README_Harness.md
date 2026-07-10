# Harness_Enhance — 统一端到端 AI 交付 Harness（可分发）

这是一个面向 CT 医疗器械软件（IEC 62304 / ISO 14971 / ISO 13485）的即用型 **AI 开发 harness**，
适用于 VS Code + GitHub Copilot。它将两种互补方法合并为一个自主、可信且
合规的端到端交付系统：

| 部分 | 优势 |
|------|----------|
| **执行引擎** | 自主长运行循环、上下文隔离的 generator+evaluator、BDD + 黑盒 UI 测试、living QMS、3 个 human gates、并行执行 |
| **领域与合规智能** | DFMEA / IFU / impact / product-defect analysis、CT·DICOM·Spectral knowledge、TICS & CodeScene remediation、自包含 HTML 报告 |

> 这是一个**干净的可分发模板** — 它只包含 harness assets，不包含示例项目。
> 已包含全新的 `.harness/` state store 和空的 QMS skeletons，因此首次运行会从干净状态开始。

## 包含内容

```
Harness_Enhance/
├─ .github/
│  ├─ copilot-instructions.md   # Unified always-loaded core
│  ├─ ASSET_CATALOG.md          # Full asset → pipeline-stage map
│  ├─ agents/       (18)         # 12 execution + 6 domain specialists
│  ├─ skills/       (31)         # engineering/test/UI + domain/risk/compliance
│  ├─ prompts/      (12)         # slash-command entry points
│  ├─ instructions/ (5)          # path-triggered + always-on TICS/CodeScene
│  └─ scripts/                   # QMS md→docx export (Python)
├─ Build/                        # Batch quality-gate scripts (auto-resolve *Impl.sln)
├─ .harness/                     # State store (empty: backlogs schema + template, empty stage dirs, fresh progress.md)
├─ docs/
│  ├─ harness/                   # Project-map reference
│  ├─ qms/                       # Empty IEC 62304 QMS skeletons (SwRS, SSDS, SDD, MVP, MVProcedure, MVReport)
│  └─ qms-templates/             # PDLM templates + generated skeletons
└─ .vscode/                      # Subagent invocation settings
```

资产总计：**18 agents · 31 skills · 12 prompts · 5 instructions。**

## 快速开始

1. 在启用 GitHub Copilot（agent mode）的 VS Code 中打开此文件夹。
2. 打开 Copilot Chat，并带着你的功能 / 缺陷 / 改进请求调用 **`@orchestrator`**。
3. 在 **3 human gates** 批准：(1) requirements + architecture + backlog + risk，(2) design + test design，(3) feature demo。
4. harness 会自主实现、评估（隔离）、自愈质量、生成风险/合规文档，并
   演示每个任务 — 只有当某个任务失败 3× 时，你才需要再次介入。

对于一次性任务，请使用 slash prompts（例如 `/full-pipeline`、`/dfmea-analysis`、`/code-quality`、`/tics-preflight`）。

## 统一 pipeline

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

harness 位于它所操作仓库的**根目录**。有两种使用方式：

- **独立试用：** 直接在 VS Code 中打开此文件夹 — 所有内容已经位于根目录。
- **应用到目标 repo：** 将 `.github/`、`.harness/`、`Build/`、`docs/` 和 `.vscode/` 复制到目标
  repository 的根目录（harness 会操作该 repo 的 `Src/`）。

然后执行 **Reload Window**，让 Copilot 重新扫描 customizations。在 Chat 中，`@` 应列出 agents，`/`
应列出 prompts。

## 自定义

- 确保你的 solution 遵循 `Src\{repo}Impl.sln` 命名约定（`Build/` gate scripts 会自动解析）。
- 如果不是 C#/.NET，请更新 `.github/copilot-instructions.md` 中的 technology stack，以及
  `source-code` / `build-ci` instructions。
- 将项目特定事实记录在 `.harness/` 或 `/memories/repo/` 中 — 保持 skills 可在团队间移植
  （不要把项目特定内容硬编码到 skills 中）。

## 先决条件（用于完整端到端运行）

.NET SDK / `dotnet` CLI · 用于检查的 ReSharper CLI (`jb`) · 用于覆盖率的 `dotnet dotcover` ·
（可选）用于 demo video 的 `ffmpeg` · 用于 QMS md→docx export 的 Python。

## 设计原则

1. **上下文隔离是关键机制**（盲测设计、隔离评估）— domain skills 以只读评判标准注入，不得破坏它。
2. **将生成与评估分离。** 3. **每个会话一个任务；orchestrator 拥有状态。**
4. **构建前先验证。** 5. **遵循 ADR；只处理新增/修改代码。** 6. **安全优先**（记录 IEC 62304 class 及理由）。
7. **定义变更向下级联，绝不向上。** 8. **双轨文档** — QMS（审计）+ HTML reports（管理）。
