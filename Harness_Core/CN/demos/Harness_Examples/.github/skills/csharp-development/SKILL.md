---
name: csharp-development
description: 编写新的 C# 生产代码：命名约定、XML documentation、async/await patterns、分层架构规则和 interface-first design。在 `Src/` 目录中实现 features 或 classes 时使用。
---

# C# Development Skill

> **同时加载 `ct-coding-standards` skill** — 它定义了适用于所有 CT C# 代码的命名约定、核心编码规则、events/delegates 和 logging levels。

## 命名约定

> 见 `ct-coding-standards` skill。

## XML Documentation

所有 public members 都必须有 XML doc comments：

```csharp
/// <summary>
/// Returns the current device measurement value in millimetres.
/// </summary>
/// <param name="token">Cancellation token.</param>
/// <returns>Position in mm, or <c>null</c> if device is not connected.</returns>
/// <exception cref="DeviceCommunicationException">Thrown when the device returns a malformed response.</exception>
public async Task<double?> GetPositionAsync(CancellationToken token = default) { ... }
```

## 编码规则

> 基础规则见 `ct-coding-standards` skill。额外 implementation patterns：

- 对拥有 unmanaged resources 的类实现 `IDisposable`；consumers 使用 `using` declarations
- 不写没有重新抛出或带完整上下文记录日志的 `catch (Exception)`

## Async Patterns

- Async methods 返回 `Task` 或 `Task<T>` — 绝不使用 `async void`（event handlers 除外）
- Public async methods 始终接受并传播 `CancellationToken`
- 绝不使用 `.Result` 或 `.Wait()` — 使用 `await`（STA threads 上有 deadlock 风险，例如 WPF dispatcher）
- 不要在 constructors 中 `await` — 使用 factory methods 或 lazy initialisation

## Events and Delegates

> 见 `ct-coding-standards` skill。

## Logging

> 见 `ct-coding-standards` skill。

## 架构规则（项目特定）

- Production classes 实现来自 `ExtInf/` 的 interfaces — 绝不绕过
- 类体内部不对 service dependencies 使用 `new` — 使用 constructor injection
- `Src/` 中的新 classes 必须位于与其文件夹路径匹配的 namespace 中
- 生产代码不要引用 test assemblies
- Dependencies 只能向下流动：UI → Application → Domain → Infrastructure
- 跨仓库依赖必须只引用 `{repo}Inf.pkg` Interface NuGet — 绝不引用 `{repo}Impl.pkg`
- 所有 dependency assemblies 都从 `Dependencies/Ref/` 引用 — 绝不直接从 Artifactory 或本地 NuGet cache 引用
- Unit tests 与其组件共置在 `Src/{ProjectDir}/Test/`
