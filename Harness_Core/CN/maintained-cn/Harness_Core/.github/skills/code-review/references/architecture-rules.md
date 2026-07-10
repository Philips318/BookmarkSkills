# Architecture Rules

用于在 code review 期间验证 architectural conformance 的规则。

---

## Layering Model

典型 project layering（top → bottom）：

```
┌─────────────────────────┐
│   UI / Views / XAML      │  ← Presentation layer
├─────────────────────────┤
│   ViewModels / Commands  │  ← Presentation logic
├─────────────────────────┤
│   Services / Workflows   │  ← Application logic
├─────────────────────────┤
│   Domain / Models        │  ← Business domain
├─────────────────────────┤
│   Infrastructure / IO    │  ← Data access, file IO, network
├─────────────────────────┤
│   Common / Utilities     │  ← Cross-cutting shared code
└─────────────────────────┘
```

**Rule**：Dependencies 必须向下流动。Upper layers 可以依赖 lower layers，反向依赖不允许。

---

## Dependency Rules

### DR-1: No Upward Dependencies
Lower-layer module 不得引用 higher-layer module。例如 Service 不应引用 ViewModel。

### DR-2: No Circular Dependencies
禁止 Project A → Project B → Project A。如果检测到，引入 shared lower layer 中的 abstraction（interface）。

### DR-3: Depend on Abstractions
High-level modules 应依赖 interfaces 或 abstract classes，而不是 concrete implementations。跨项目边界尤其重要。

### DR-4: Shared Library Stability
Shared libraries（`CT_SW_Common`、`DirectResultPipeline`、`MIPPP`）必须保持 backward-compatible public APIs。Breaking changes 需要明确协调。

### DR-5: Project Boundary Enforcement
特定于一个 application 的 types（例如 `iWorkflow`、`ThreeDSurview`）不得泄漏到 shared libraries。

---

## Design Rules

### DES-1: Separation of Concerns
- View/XAML code-behind 中不要有 business logic。
- Service 或 Domain layers 中不要有 UI framework types（例如 `Dispatcher`、`Brush`、`Visibility`）。
- ViewModel constructors 中不要有 file I/O 或 network calls。

### DES-2: Dependency Injection
- 优先使用 constructor injection，而不是 Service Locator / `Resolve<T>()` patterns。
- 在 composition root 中注册 new services，不要散落在整个 codebase 中。
- 避免注入 DI container 本身。

### DES-3: Interface Segregation
- New interfaces 应小而聚焦于单一 role。
- 如果方法用途不同，不要向 existing interfaces 添加方法 — 创建新 interface。

### DES-4: Event and Messaging
- 优先使用 typed events 或 message bus，而不是 modules 间 tight coupling。
- 始终将 event subscribe 与 unsubscribe 配对。
- 除非没有替代方案，否则不要使用 static events 做 cross-module communication。

### DES-5: Configuration Management
- 不要 hard-code connection strings、paths 或 magic values。
- Configuration 应从 config files 或 DI 流入，而不是嵌入 business logic。

### DES-6: Error Handling Strategy
- Domain exceptions 应在 layer boundaries 被捕获并转换。
- UI layer 应展示 user-friendly messages；service layer 应记录 technical details。
- 不要让 infrastructure exceptions（例如 `SqlException`）传播到 UI。

---

## Cross-Project Rules

### CP-1: Mirrored Files
当 behavior 存在于 mirrored folders（例如 `_Dev` / `_Plot` pairs）中时，在同一 commit 中更新两份副本。

### CP-2: Shared DLL Output
将 DLL 发布到 shared output folders（例如 `MIPPP\Bin`）的 projects，必须在验证 dependent projects 前使用正确 configuration 重新构建。

### CP-3: NuGet Package Consistency
在一个 project 中更新 NuGet package 时，检查 sibling projects 是否也使用同一 package，并对齐版本以避免 runtime conflicts。

---

## Review Verdicts

| 结论 | 准则 |
|---------|----------|
| **Pass** | 没有 architecture rule violation |
| **Warning** | 不破坏 architecture 的轻微偏离（例如 convenience shortcut） |
| **Fail** | 明确违反 layering、dependency direction 或 project boundary rules |
