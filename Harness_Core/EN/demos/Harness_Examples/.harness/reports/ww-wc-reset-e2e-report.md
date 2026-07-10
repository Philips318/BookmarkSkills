# Harness End-to-End 实验报告

**主题（Feature）**：WW/WC Reset to Default（CT 图像窗宽/窗位一键复位）
**仓库**：`Harness_Enhance`　**日期**：2026-07-02　**安全等级**：IEC 62304 Class B
**报告类型**：AI Dev Harness 端到端实验结果（用于团队反馈）

---

## 0. 一句话结论

从"空 harness + 一句话需求"到"可运行、可演示、通过三道门禁、并带黑盒 UI 验收测试"的 CT 医疗软件功能，**端到端主流程全部跑通**。全程 **3 道 Gate 全部通过**；期间 **Gate 2 抓到 2 个"测试全绿却仍存在"的缺陷、Gate 3 现场演示又抓到 1 个所有自动化测试都漏掉的 UI 缺陷**——这三处正是本实验最有价值的证据，印证了"生成/评估分离 + 独立黑盒测试 + 人工现场演示"这套机制的作用。

| Gate | 类型 | 结果 |
|------|------|------|
| Gate 1 组合评审（需求/架构/Backlog/Gherkin） | 人工 | ✅ APPROVED |
| Gate 2 逐任务质量门禁（dev-evaluator，隔离上下文） | 自动+独立 | ✅ PASS（T01、T04 各 FAIL 一次后修复） |
| Gate 3 功能演示（用户为裁判） | 人工 | ✅ APPROVED |

---

## 1. 完整流程与步骤

```
Phase 0 Orientation ─▶ Phase 1 需求接入 ─▶ Phase 2 规划(analyst/architect/product-owner)
   ─▶ [GATE 1] ─▶ Phase 3 执行环(developer × 4 任务, evaluator 逐任务) ─▶ [GATE 2]
   ─▶ Phase 4 Backlog 完成 ─▶ Phase 5 WPF 宿主 + Demo(feature-demonstrator) ─▶ [GATE 3]
   ─▶ Phase 6 交付后加固(test-designer FlaUI 验收 + 工具链 + 报告)
```

| # | 步骤 | 执行角色 | 交付物 | 结果 |
|---|------|----------|--------|------|
| 0 | Orientation | orchestrator | 确认 harness 为空（无 `Src/`、Backlog 仅模板、baseline 无法运行） | 诚实报告"无任务" |
| 1 | 需求接入 | user → @orchestrator | 一句话需求 | — |
| 2a | 需求分析 | @analyst | `requirements/ww-wc-reset-requirements.md`（FR-01~07, NFR-01~05, Class B, 风险, OQ）；`docs/qms/SwRS.md` | ✅ |
| 2b | 架构 | @sw-architect | `architecture/adr/ADR-001`（7 决策）+ 组件图 + 时序图；`docs/qms/SSDS.md` | ✅ |
| 2c | 计划 | @product-owner | `backlogs/ww-wc-reset.json`（4 任务）+ 4 个 Gherkin feature（17 scenarios） | ✅ |
| **G1** | **组合评审** | user | 批准；裁定 OQ-01=Class B、OQ-02=per-viewport | ✅ APPROVED |
| 3.1 | T01 基础设施 | @developer → @dev-evaluator | 全新 `ExtInf/`(7) + `Src/ImageDisplay/` + 测试工程 + `.sln` | evaluator FAIL×1 → 修复 → complete |
| 3.2 | T02 复位主流程 | @developer | ViewModel/Command/Model/Notifier + 单测 + BDD | baseline 绿 → complete |
| 3.3 | T03 回退 | @developer | `WindowLevelDefaults` + 回退分支 + 测试 | baseline 绿 → complete |
| 3.4 | T04 撤销 | @developer → @dev-evaluator | `WindowLevelChangeAction` + `WindowLevelUndoService`(深度20) + Undo/Redo | evaluator FAIL×1(3 findings) → 修复 → PASS |
| **G2** | **质量门禁** | @dev-evaluator（隔离） | 85 单测 + 17 模块，0 fail/skip | ✅ PASS |
| 4 | Backlog 完成 | @orchestrator | 4/4 complete | ✅ |
| 5 | WPF 宿主 + Demo | @orchestrator + @feature-demonstrator | `Src/ImageDisplayApp/`(7 文件) + 18 张演示截图 | Demo 发现 Undo 缺陷 → 修复 → 复演通过 |
| **G3** | **功能演示** | user | 审阅证据后批准 | ✅ APPROVED |
| 6 | 交付后加固 | @test-designer + @orchestrator | `Src/VerificationTests/`（12 FlaUI 验收测试）+ dotCover/ReSharper 工具链 | ✅ 12/12 pass |

---

## 2. 各阶段交付物与成果（明细）

### 2.1 需求（@analyst）
- **FR-01~07**：按钮存在/复位到 DICOM 默认(index 0)/多预设名/无标签回退(WW 400,WC 40)/瞬态确认消息/撤销/WW≤0 守卫。
- **NFR-01~05**：≤200ms 响应、键盘可达+tooltip、无 PHI 泄漏、降级不崩溃、DICOM 标准可追溯(PS3.3 §C.7.6.3.1.5)。
- **IEC 62304 Class B**（主诊断工作站，OQ-01 确认）。

### 2.2 架构（@sw-architect）—— ADR-001 七项决策
RelayCommand 命令绑定 · 模型持有默认值 · `IWindowLevelDefaults` 回退服务 · 状态栏 `IStatusNotifier` 非阻塞确认 · `IUndoService`/`IUndoableAction` 撤销(深度20, per-viewport) · `WindowLevel.Create()` 集中 WW>0 守卫 · `DicomDisplayTags` 集中标签常量。分层：`ExtInf`(契约) → `Src`(实现)，命名空间 `Philips.CT.Host.ImageDisplay.*`。

### 2.3 Backlog + Gherkin（@product-owner）
4 任务（1 基础 + 3 特性），17 个 scenario，每个特性任务 `demo_required=true`。

### 2.4 实现（@developer × 4）
16 个生产 `.cs`（ExtInf 7 + Src 9），8 个单测文件，4 个 Reqnroll step 类。

### 2.5 WPF 宿主（交付后补充）
`Src/ImageDisplayApp/`：`App`(组合根，解析 `--no-dicom-ww`)、`ImageDisplayView`(WW/WC 滑块 + Reset/Undo/Redo + 亮度随 WC 变化的视口 + 状态栏)、`DispatcherStatusNotifier`(WPF 版 `IStatusNotifier` + `INotifyPropertyChanged`)、`WindowCenterToBrushConverter`。复用已测库，未改被测代码。

### 2.6 黑盒 UI 验收（@test-designer，交付后加固）
`Src/VerificationTests/`：Task2(5) + Task3(3) + Task4(4) = **12 个 FlaUI `[Category("Verification")]` 测试**，page-object 模式、retry 等待、从 feature 文件 + UI 契约独立编写（未读生产代码）。**含关键回归测试 `ResetButton_Verify_UndoButtonBecomesEnabled`**——若此套件早于 demo 存在，可自动拦截 Undo 缺陷。

---

## 3. 三处关键缺陷（本实验最有价值的证据）

| # | 阶段 | 缺陷 | 为何测试没抓到 | 修复 |
|---|------|------|----------------|------|
| A | Gate 2 (T01) | `DicomDisplayTags` 各字段缺 DICOM 标准引用（违反 NFR-05） | 属文档/合规规则，功能测试不覆盖 | 补 PS3.3 §C.7.6.3.1.5 引用 |
| B | Gate 2 (T04) | `IsSeriesLoaded` 由 `HasValidDefaultWindowLevel` 派生 → **无 DICOM 标签的序列被误判"未加载"，复位按钮被禁用**，与 FR-01/04/07 冲突 | 单测/BDD 直接调 `CanExecute()`，未覆盖"加载但无标签"这条真实一致性路径 | 新增显式 `IsSeriesLoaded` 契约 + 2 回归测试 |
| C | **Gate 3 现场演示** | 复位后 **Undo 按钮不激活**（自定义 `DelegateCommand` 在 UI 无关库中无法挂 WPF `CommandManager.RequerySuggested`，复位路径未触发 `CanExecuteChanged`） | **单测/BDD 全绿**——它们直接调 `CanExecute(null)`，绕过了 WPF 事件驱动的重查询语义 | 从 WW/WC setter 统一 `RefreshUndoRedoState()` + 2 回归测试；FlaUI 回归测试兜底 |

> 结论：**缺陷 C 是所有既有自动化测试都漏掉、只有"人工现场 demo"才暴露的**。这直接证明了 harness 中 Gate 3（现场演示）与随后补的 FlaUI 黑盒验收层不可或缺。

---

## 4. 度量数据

| 指标 | 值 |
|------|-----|
| 需求 | 7 FR + 5 NFR，Class B |
| ADR / 图 | 1 ADR（7 决策）+ 2 图 |
| Backlog | 4 任务 / 17 Gherkin scenarios |
| 生产代码 | 16 个 `.cs`（ExtInf 7 + Src 9） |
| WPF 宿主 | 7 文件 |
| **自动化测试总数** | **114**：单测 **85** + 模块 BDD **17** + FlaUI 验收 **12** |
| 测试结果 | 0 失败 / 0 跳过 / 0 警告（TreatWarningsAsErrors） |
| 生产代码覆盖率（coverlet, 85 单测） | **90.6% 行 · 85% 分支**（Impl 90% / Inf 100%，超 80% 门槛） |
| Gate 2 拒绝次数 | 2（T01×1、T04×1，均修复后 PASS） |
| 现场 demo 抓到的额外缺陷 | 1（Undo 按钮激活，已修复 + 回归测试兜底） |
| Demo 证据 | 18 张截图 |

---

## 5. 需要改进的 Point（团队反馈重点）

| # | 问题 | 影响 | 建议 |
|---|------|------|------|
| 1 | **UI 集成缺陷漏到 demo 才发现** | 命令可用性/按钮激活类 bug 单测测不到 | ✅ 已补 FlaUI 黑盒验收层；**建议将其纳入 Gate 2**（developer∥test-designer 并行，不再滞后到 demo） |
| 2 | **Gate 2 测试盲区**：直接调 `CanExecute()` 绕过 WPF requery | 事件驱动可用性变更测不到 | 增加"`CanExecuteChanged` 通知契约"单测（本次已加 2 个） |
| 3 | **质量门禁脚本与 dotCover 打包不匹配** | `Run-QualityGate.cmd` 调 `dotnet dotcover test`，但 `JetBrains.dotCover.CommandLineTools` 暴露的是独立 `dotCover cover/report`（2026.1），非 `dotnet dotcover` 集成 | 调和 `Build/*.cmd` 与 CommandLineTools 包（或安装 `dotnet-dotcover` 集成变体） |
| 4 | **Reqnroll 陷阱耗时** | 步骤文本裸 `/` 被当 Cucumber 择一算符 → "undefined" 静默跳过；提交生成的 `*.feature.cs` 致场景双重编译 | 仓库加 `.gitignore *.feature.cs` + step 文本 lint（禁裸 `/` 或强制 `ExpressionType.RegularExpression`） |
| 5 | **Orchestrator 直接改代码修复** | T01/T04/demo 修复由 orchestrator 应用后用单测+demo 验证，略偏"生成/评估分离"纯粹性 | 关键修复回抛 @developer + 重跑 @dev-evaluator |
| 6 | **环境工具缺失（部分已补）** | 曾无 dotCover/ReSharper（覆盖率仅定性）、无 ffmpeg（无 demo 视频） | ✅ 已装 dotCover + ReSharper；ffmpeg 待装以录像 |
| 7 | **QMS 审计轨未完全闭环** | SDD/MVP/MVProcedure 部分章节 evaluator 曾标记未填 | 补齐或显式标注 N/A 至 QMS checker 全绿 |
| 8 | **规划期 demo_entry_point 失真** | 原 Backlog 指向类库里不存在的 `.exe` | 规划期即决定是否需要可运行宿主 |

---

## 6. 亮点（机制有效性验证）

1. **生成/评估分离有效**：隔离的 @dev-evaluator 读代码抓到缺陷 B（功能一致性）。
2. **人工现场 demo 不可替代**：Gate 3 抓到缺陷 C——所有自动化测试全绿仍存在。
3. **黑盒验收层闭环**：@test-designer 从规格独立编写 12 个 FlaUI 测试，含针对缺陷 C 的回归兜底。
4. **根因纪律**：Reqnroll Cucumber-expression 问题被彻底根因定位并写入长期记忆，而非绕过。
5. **全程可追溯**：FR → ADR → Backlog → Gherkin → 单测/BDD/FlaUI → Demo 证据链完整；baseline 全程绿色。

---

## 7. 交付物索引

- 需求：`.harness/requirements/ww-wc-reset-requirements.md`
- 架构：`.harness/architecture/adr/ADR-001-ww-wc-reset-design.md` + `diagrams/`
- Backlog：`.harness/backlogs/ww-wc-reset.json`
- 规格：`.harness/specs/ww-wc-reset/*.feature`（4）
- 生产代码：`ExtInf/ImageDisplay/`、`Src/ImageDisplay/`
- WPF 宿主：`Src/ImageDisplayApp/`（`--simulator --seed=demo [--no-dicom-ww]`）
- 单测/BDD：`Src/ImageDisplay/Test/`、`Src/ModuleTests/WwWcReset/`
- FlaUI 验收：`Src/VerificationTests/`（12 tests）
- Demo 证据：`.harness/demo_evidence/ww-wc-reset/`（18 张）
- 进度日志：`.harness/progress.md`（单一事实来源）
- 本报告：`.harness/reports/ww-wc-reset-e2e-report.md` + `.html`
