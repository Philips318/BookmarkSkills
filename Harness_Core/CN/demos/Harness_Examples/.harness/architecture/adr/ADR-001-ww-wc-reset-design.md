# ADR-001：WW/WC Reset to Default — 设计决策

**日期：** 2026-07-02
**状态：** Accepted
**决策者：** [sw-architect, user (Gate 1 approval 2026-07-02)]
**取代：** N/A

## 背景

CT 图像显示应用需要一个 "Reset Window/Level to Default" 功能（FR-01 到 FR-07），用于为活动视口恢复 DICOM 默认的 Window Width (0028,1051) 和 Window Center (0028,1050) 值。该应用是遵循 MVVM 架构的 WPF 桌面客户端，并被分类为 IEC 62304 Class B，因为不正确的 WW/WC 渲染可能遮蔽有临床意义的特征，导致漏诊或延迟诊断。

这是一个 **greenfield** 项目 — 目前还不存在 `Src/` 树。本 ADR 定义 `@developer` 必须遵循的项目布局、分层架构、组件职责、接口契约和绑定约束。

推动该决策的关键因素：

- **MVVM 纯度：** WPF views 必须零 code-behind；所有逻辑都通过 ViewModel commands 流转。
- **DICOM 正确性：** WW 必须始终 > 0；fallback 值必须可配置且无需 rebuild。
- **IEC 62304 Class B 可追溯性：** 所有 DICOM tag 访问必须通过专用抽象；不得有 magic numbers。
- **Undo 要求（FR-06）：** reset 动作必须可撤销；这需要 undo infrastructure。
- **诊断工作流：** reset 必须在 200 ms 内完成（NFR-01），并提供非阻塞用户反馈。

## 决策

我们将使用 **分层 MVVM 架构** 实现 WW/WC Reset 功能，并清晰分离接口契约（`ExtInf/`）和实现（`Src/`），通过构造函数注入的依赖连接。reset 动作在 ViewModel 上暴露为 `IRelayCommand`，DICOM defaults 位于 model 层，fallback 值来自配置服务，用户反馈通过状态栏通知服务交付。

## 理由

### 1. 命令绑定模式 — `RelayCommand` (IRelayCommand)

**选择：** 实现 `IRelayCommand` 的 `RelayCommand`（来自 CommunityToolkit.Mvvm 或等价的自包含实现）。

**原因：** `RelayCommand` 是惯用的 WPF MVVM command pattern。它包装 `Execute` delegate 和 `CanExecute` predicate，支持 `ICommand.CanExecuteChanged` 以自动启用/禁用按钮，并且除 MVVM toolkit 外不需要其他框架。使用 `IRelayCommand`（在 `ICommand` 基础上扩展 `NotifyCanExecuteChanged()`）允许 ViewModel 在 loaded series 变化时显式重新评估按钮启用状态。

**否决的替代方案：**
- `DelegateCommand` (Prism) — 功能等价，但会把项目耦合到 Prism framework，而本项目并不需要它。
- 原始 `ICommand` 实现 — 对标准 command pattern 来说样板过多。

### 2. 默认值存储 — Model 持有、按需访问

**选择：** `DicomImageModel` 在 series 加载时读取 DICOM tags (0028,1050/1051)，并通过 `DefaultWindowLevel` 属性（可空 `WindowLevel` value object）暴露。ViewModel 从 model 属性获取 defaults — 不另行缓存。

**原因：** model 是 DICOM data 的单一事实来源。在 ViewModel 中缓存 defaults 会复制状态，并在 loaded series 变化时产生同步风险。按需模式保持单向数据流：Model → ViewModel → View。

### 3. Fallback 策略 — `IWindowLevelDefaults` 配置服务

**选择：** 定义 `IWindowLevelDefaults` 接口，含单一属性 `FallbackWindowLevel`，返回可配置 fallback 对（默认：WW = 400，WC = 40）。实现从应用设置（如 `appsettings.json` 或 `.config` 文件）读取 — 值可无需 rebuild 修改（FR-04）。

**原因：** 将 fallback configuration 分离到独立服务符合 Single Responsibility Principle，且可独立测试。ViewModel 逻辑变为：“若 model defaults 有效则使用；否则使用 fallback service defaults。” fallback WW 在 service 层验证（> 0）。

### 4. 确认消息 — 通过 `IStatusNotifier` 发送到状态栏

**选择：** 通过 `IStatusNotifier` 接口交付状态栏通知。实现（`StatusBarNotifier`）设置绑定的 `StatusMessage` 属性，并启动 `DispatcherTimer`，在配置的显示时长后自动消失（按 FR-05 最少 2 秒）。

**原因：** 状态栏消息是非阻塞的，不会遮挡临床图像内容（R-04），也是医学影像应用中低严重度确认的标准反馈机制。`IStatusNotifier` 抽象保持 MVVM 分离 — ViewModel 调用 `ShowTransientMessage(string, TimeSpan)`，无需知道 UI 实现。Toast/InfoBar overlay 被否决，因为它们有遮挡诊断图像区域的风险（R-04）。

### 5. Undo 支持 — 轻量 `IUndoService`

**选择：** 一个 scoped `IUndoService`，提供 `Push(IUndoableAction)` 和 `Undo()` / `Redo()` 方法。reset command 推入一个 `WindowLevelChangeAction`（实现 `IUndoableAction`），捕获 reset 前的 WW/WC 值。`Undo()` 恢复这些值。undo service 注入到 `ImageDisplayViewModel`。

**原因：** FR-06 强制要求 undo。按 OQ-02，我们假设不存在既有 undo stack（greenfield）。`IUndoService` / `IUndoableAction` pattern 最小、可扩展到后续其他显示动作，并将 undo 关注点从 command 自身移出（SRP）。undo stack 深度有界（可配置，默认 20），防止内存无界增长。

### 6. WW 有效性守卫 — 集中在 `WindowLevel` Value Object 中

**选择：** 一个 `WindowLevel` record/struct，带工厂方法 `WindowLevel.Create(double ww, double wc)`；若 WW ≤ 0，则抛出 `ArgumentOutOfRangeException`。所有产生 `WindowLevel` 的代码路径都经过该工厂。来自 DICOM 或 undo stack 的无效值会在构造时被捕获（FR-07）。

**原因：** 将 WW > 0 不变量集中在 value object 中，意味着无论调用点在哪（model、undo、fallback），验证都不会被绕过。这是一个 safety guard — 按领域知识，WW ≤ 0 会产生黑图或反相图像。

### 7. DICOM Tag 访问 — 集中 `DicomDisplayTags` Constants

**选择：** 在 `Src/ImageDisplay/Constants/` 中放置静态类 `DicomDisplayTags`，为 tags (0028,1050)、(0028,1051) 和 (0028,1055) 保存具名 constants。每个 constant 都有 XML doc comment 引用 DICOM PS3.3 §C.7.6.3.1.5（NFR-05）。`DicomImageModel` 使用这些 constants — 其他地方没有 magic-number tag literals。

## 架构边界（给 @developer 的绑定约束）

### 项目结构（greenfield）

```
ExtInf/
  ImageDisplay/
    IImageDisplayViewModel.cs      — ViewModel contract
    IDicomImageModel.cs            — Model contract (DICOM data)
    IStatusNotifier.cs             — Transient message delivery
    IUndoService.cs                — Undo/Redo stack
    IUndoableAction.cs             — Single undoable action
    IWindowLevelDefaults.cs        — Fallback configuration
    WindowLevel.cs                 — Value object (shared between ExtInf and Src)

Src/
  ImageDisplay/
    ImageDisplayViewModel.cs       — ViewModel implementation
    Commands/
      ResetWindowLevelCommand.cs   — IRelayCommand for the reset action
    Models/
      DicomImageModel.cs           — IDicomImageModel implementation
      WindowLevelChangeAction.cs   — IUndoableAction for WW/WC undo
    Services/
      StatusBarNotifier.cs         — IStatusNotifier implementation
      WindowLevelUndoService.cs    — IUndoService implementation
      WindowLevelDefaults.cs       — IWindowLevelDefaults implementation
    Constants/
      DicomDisplayTags.cs          — DICOM tag constants (0028,1050/1051/1055)
    Resources/
      ImageDisplayStrings.resx     — Localised UI strings
    Views/
      ImageDisplayView.xaml        — WPF View (XAML only, zero code-behind)
    Test/
      ImageDisplayViewModelTests.cs
      ResetWindowLevelCommandTests.cs
      DicomImageModelTests.cs
      WindowLevelTests.cs
      WindowLevelUndoServiceTests.cs
      WindowLevelDefaultsTests.cs

  ModuleTests/
    WwWcReset/
      Features/
        WwWcReset.feature          — BDD Gherkin scenarios
      Steps/
        WwWcResetSteps.cs          — Reqnroll step definitions
```

### Namespace

所有类均位于 `Philips.CT.Host.ImageDisplay` 下 — 针对 `.Commands`、`.Models`、`.Services`、`.Constants`、`.Views` 使用子命名空间。

### 必需类型和成员

| 类型 | Location | 关键成员 |
|------|----------|-------------|
| `WindowLevel` | `ExtInf/ImageDisplay/` | `double Width` (> 0), `double Center`, `string? PresetName`, static `Create(double ww, double wc, string? name = null)` |
| `IImageDisplayViewModel` | `ExtInf/ImageDisplay/` | `double WindowWidth { get; set; }`, `double WindowCenter { get; set; }`, `IRelayCommand ResetWindowLevelCommand { get; }`, `bool IsSeriesLoaded { get; }` |
| `IDicomImageModel` | `ExtInf/ImageDisplay/` | `WindowLevel? DefaultWindowLevel { get; }`, `bool HasValidDefaultWindowLevel { get; }` |
| `IStatusNotifier` | `ExtInf/ImageDisplay/` | `void ShowTransientMessage(string message, TimeSpan duration)` |
| `IUndoService` | `ExtInf/ImageDisplay/` | `void Push(IUndoableAction action)`, `void Undo()`, `void Redo()`, `bool CanUndo { get; }`, `bool CanRedo { get; }` |
| `IUndoableAction` | `ExtInf/ImageDisplay/` | `void Execute()`, `void Undo()`, `string Description { get; }` |
| `IWindowLevelDefaults` | `ExtInf/ImageDisplay/` | `WindowLevel FallbackWindowLevel { get; }` |
| `ImageDisplayViewModel` | `Src/ImageDisplay/` | Implements `IImageDisplayViewModel`, `INotifyPropertyChanged`. Constructor-injected: `IDicomImageModel`, `IStatusNotifier`, `IUndoService`, `IWindowLevelDefaults` |
| `ResetWindowLevelCommand` | `Src/ImageDisplay/Commands/` | `IRelayCommand`. `CanExecute` → true only when `IsSeriesLoaded`. `Execute` → reads default from model (or fallback), validates WW > 0, pushes undo action, updates ViewModel properties, fires status notification. |
| `DicomDisplayTags` | `Src/ImageDisplay/Constants/` | `static readonly` fields for tags `WindowCenter` (0028,1050), `WindowWidth` (0028,1051), `WindowCenterWidthExplanation` (0028,1055). XML doc referencing PS3.3 §C.7.6.3.1.5. |

### 设计规则

1. `ImageDisplayView.xaml` 中 **零 code-behind** — 所有按钮点击处理通过 `{Binding ResetWindowLevelCommand}`。
2. 所有 ViewModel 依赖使用 **构造函数注入** — 不使用 service locator，不在 ViewModel 内部 `new`。
3. **WW > 0 不变量** 由 `WindowLevel.Create()` 强制 — 违规时抛出 `ArgumentOutOfRangeException`。
4. DICOM tags 多值时使用 **第一个 preset（index 0）**（FR-03）。
5. **所有 UI 字符串来自 `ImageDisplayStrings.resx`** — 按钮标签、tooltip、确认消息模板。
6. **键盘快捷键 `Ctrl+Shift+W`** 通过 XAML 中的 `InputBinding` / `KeyBinding` 绑定（NFR-02，需 OQ-05 确认）。
7. **Logging：** fallback 激活（FR-04）和无效 WW 拒绝（FR-07）记录 WARNING-level log。日志中无 PHI（NFR-03）。
8. **无未处理异常** — `ResetWindowLevelCommand.Execute` 方法用 try/catch 包裹主体；错误以 WARNING 记录，通过 `IStatusNotifier` 通知用户，当前 WW/WC 保持不变（NFR-04）。

### 每个任务的设计说明（用于 @product-owner backlog embedding）

| Task Area | `design_note` |
|-----------|--------------|
| **View (XAML)** | 添加带 `Command="{Binding ResetWindowLevelCommand}"` 的 `Button`，`ToolTip` 来自 resource，并为 Ctrl+Shift+W 添加 `KeyBinding`。包含 `AutomationProperties.AutomationId="ResetWindowLevelButton"` 以支持 FlaUI testing。状态栏 `TextBlock` 绑定到 `StatusMessage` 属性。 |
| **ViewModel** | `ImageDisplayViewModel` : `IImageDisplayViewModel`, `INotifyPropertyChanged`。通过 4 个注入依赖构造。暴露 `WindowWidth`、`WindowCenter`、`ResetWindowLevelCommand`、`IsSeriesLoaded`、`StatusMessage`。 |
| **Command** | `ResetWindowLevelCommand` : `IRelayCommand`。逻辑：读取 `IDicomImageModel.DefaultWindowLevel`；若 null/invalid → 使用 `IWindowLevelDefaults.FallbackWindowLevel`；通过 `WindowLevel.Create()` 验证 WW > 0；推入 undo action；设置 ViewModel props；触发 `IStatusNotifier`。 |
| **Model** | `DicomImageModel` : `IDicomImageModel`。在 series load 时用 `DicomDisplayTags` constants 解析 tags (0028,1050/1051/1055)。暴露 `DefaultWindowLevel`（nullable）。 |
| **Undo** | `WindowLevelChangeAction` 捕获 reset 前 WW/WC，并在 `Undo()` 时恢复。`WindowLevelUndoService` 管理有界 stack（默认深度 20）。 |
| **Fallback** | `WindowLevelDefaults` 从配置文件读取 WW=400、WC=40。加载时验证 WW > 0。 |
| **Status** | `StatusBarNotifier` 设置绑定字符串属性并启动 `DispatcherTimer`（最少 2 s）以自动消失。 |

## 后果

### 正面

- **MVVM-clean：** View 完全不知道 DICOM、undo 或 notification 逻辑 — 全部由 ViewModel 通过 interface contracts 中介。
- **可测试：** 每个组件都通过接口注入，可以使用 NSubstitute mocks 做 unit-test。`WindowLevel` value object 可独立测试 WW > 0 不变量。
- **Safety guard 是结构性的：** WW > 0 约束在类型层级强制（`WindowLevel.Create()`），使无效值无法在没有显式异常的情况下传播。
- **可扩展 undo：** `IUndoService` / `IUndoableAction` pattern 可自然扩展到未来其他显示动作（zoom、pan、rotation）。
- **DICOM 可追溯性：** 所有 tag access 集中在 `DicomDisplayTags` 并带标准引用，满足 NFR-05 并简化审计。
- **IEC 62304 Class B 合规：** 清晰的关注点分离、文档化接口和可追溯设计决策支持所需生命周期文档。

### 负面 / 取舍

- **新的 undo infrastructure：** FR-06 需要尚不存在的 undo stack。这会增加范围（约 2 个类 + interface），但边界清楚。
- **接口数量增加：** 单个功能在 `ExtInf/` 中新增 6 个接口可能显得偏重，但这与项目的分层架构模式一致，并支持独立测试。
- **用于状态消失的 DispatcherTimer：** 会把 `StatusBarNotifier` 绑定到 WPF 的 `Dispatcher`。这是可接受的，因为它明确是 UI-layer 组件 — 接口（`IStatusNotifier`）仍然与 UI framework 无关。

## 考虑过的替代方案

| Alternative | 描述 | Reason Rejected |
|-------------|-------------|-----------------|
| **DelegateCommand (Prism)** | Prism 的 command 实现，支持自动 CanExecute 重新评估 | 引入对 Prism framework 的依赖，而项目并不需要它。`RelayCommand` 在无框架耦合的情况下提供等价功能。 |
| **在 ViewModel 中缓存 defaults** | 首次读取后在 ViewModel 中保存一份 DICOM 默认 WW/WC | 在 Model 与 ViewModel 之间复制状态，在 loaded series 变化时产生同步风险。Model 应保持单一事实来源。 |
| **Toast overlay for confirmation** | 在图像区域上方显示浮动 toast 通知 | 有遮挡诊断图像内容的风险（R-04）。状态栏位置避免该风险，并且是医学影像应用中非关键确认的既有模式。 |
| **No undo (defer FR-06)** | 实现无 undo 的 reset；将 FR-06 推迟到未来版本 | FR-06 是明确需求。推迟会使功能不完整，并在以后添加 undo 时需要返工 command architecture。 |
| **ViewModel stores fallback constants** | 在 ViewModel 中硬编码 WW=400、WC=40 | 违反 FR-04（无需 rebuild 即可配置），并把配置关注点混入 ViewModel。 |

## IEC 62304 Safety Class 影响

**Safety Class：** B（按需求文档分类）

该架构通过以下方式支持 Class B 合规：

1. **隔离：** WW/WC reset 逻辑限制在 ImageDisplay module 内，并有定义良好的接口。它不影响 acquisition、reconstruction 或 DICOM storage pipelines。
2. **验证守卫：** `WindowLevel` value object 在构造时强制 WW > 0，防止最关键的安全危害（WW ≤ 0 导致黑图/反相图像）。
3. **优雅降级：** NFR-04 要求无未处理异常。command 的 try/catch + fallback pattern 确保显示永远不会处于不确定状态。
4. **可追溯性：** `ExtInf/` 中每个接口都映射到一个功能需求。`Src/` 中每个实现都可通过接口追溯回需求。
5. **可测试性：** 所有组件都通过接口注入并可独立测试。单元测试覆盖率 gates（≥ 80%）按项目质量门禁配置应用。

**SAD Reference：** SAD §4.5 — Software Design, Image Display Sub-System（作为 SSDS 更新的一部分编写）。

## 本 ADR 已解决的开放问题

| OQ | Resolution |
|----|-----------|
| OQ-02 | 假设不存在既有 undo stack（greenfield）。ADR 定义了一个 scoped 到此 module 的新 `IUndoService`。 |
| OQ-03 | DICOM tag access 通过 `DicomDisplayTags` constants + model abstraction（`IDicomImageModel`）。tag abstraction layer 由本 ADR 定义。 |
| OQ-04 | WW/WC viewport state 由 `ImageDisplayViewModel` 持有（属性 `WindowWidth`、`WindowCenter`）。model 持有 DICOM defaults；ViewModel 持有活动 display state。 |

## 剩余开放问题

| OQ | Status |
|----|--------|
| OQ-01 | **未解决。** Class B vs Class C 判定等待 Product Owner / Regulatory 输入。如果确认 Class C，需要额外生命周期活动（FMEA、formal verification），但架构不变。 |
| OQ-05 | **未解决。** 键盘快捷键 Ctrl+Shift+W 需要 Product Owner 确认。XAML `KeyBinding` 可轻易修改。 |