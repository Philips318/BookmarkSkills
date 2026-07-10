## CT Software Department Architecture Principles

### Layering Rules

```
┌─────────────────────────────┐
│  Presentation (Views/XAML)  │  ← Only UI rendering, no logic
├─────────────────────────────┤
│  ViewModel                  │  ← Commands, bindings, UI state
├─────────────────────────────┤
│  Application Services       │  ← Use cases, workflow orchestration
├─────────────────────────────┤
│  Domain Models              │  ← Business entities, value objects
├─────────────────────────────┤
│  Infrastructure             │  ← Repositories, DICOM, file I/O
└─────────────────────────────┘
```

**Dependency Rule:** 每一层只能依赖其正下方的一层。绝不向上依赖。绝不跳层。

### Forbidden Dependencies

| From | To | 原因 |
|------|----|-----|
| View | Service / Repository | 绕过 ViewModel |
| ViewModel | Repository (direct) | 必须经过 Service |
| 领域 | Infrastructure | Domain 是纯的，无 I/O |
| Any layer | Concrete class in another layer | 使用 interfaces |

### MVVM Rules for WPF Projects

1. **View (XAML)**
   - 仅使用 data binding — 不要有带 business logic 的 `Click` handlers。
   - 对 user actions 使用 `Command` binding。
   - Code-behind 限于：visual-only logic（animations、focus management）。

2. **ViewModel**
   - 实现 `INotifyPropertyChanged`。
   - 为 user actions 暴露 `ICommand`。
   - 不直接引用 UI types（`Window`、`Control`、`Brush`）。
   - 通过 constructor 注入 dependencies。

3. **Model / Service**
   - Business logic 位于这里。
   - 不知道 UI 或 ViewModel。
   - 可隔离测试（无 UI thread dependency）。

### Dependency Injection

- **New projects (.NET 8):** 使用内置 `Microsoft.Extensions.DependencyInjection`。
- **Legacy projects (.NET 4.8):** 使用 Autofac（已在使用）。
- 注册方式为 interface → implementation。
- 除非需要 state sharing，否则优先使用 `Transient`。
- `Singleton` 仅用于：configuration、logging、shared caches。

### Shared Component Guidelines

| 组件 | Package Type | Consumers |
|-----------|-------------|-----------|
| PPTCommon | Project reference / NuGet | All projects |
| CT_SW_Common | Project reference | MIA, MIC, VolumeEngine |
| DirectResultPipeline | NuGet | Service layer |

**Rules for shared components:**
- Breaking API changes require ADR + version bump + migration guide.
- All public APIs must have XML doc comments.
- Minimum 70% UT coverage for shared components.

### DICOM Considerations

- 将 DICOM protocol 包装在 `IDicomService` interface 后面。
- ViewModel 或 Service layer 中不要直接访问 raw DICOM tag。
- DICOM parsing errors 必须被捕获并记录，绝不能让 UI crash。
- Patient data 绝不能出现在 log files 或 AI prompts 中。

### Design Pattern Preferences

| 场景 | Preferred Pattern | Avoid |
|----------|------------------|-------|
| Multiple algorithms | 策略 | Switch/case blocks |
| Object creation | Factory / Builder | `new` in business logic |
| Event communication | Observer / EventAggregator | Direct coupling |
| State machines | State pattern | Nested if/else |
| Cross-cutting concerns | Decorator / Middleware | Base class inheritance |
| 配置 | Options pattern | Static config access |
