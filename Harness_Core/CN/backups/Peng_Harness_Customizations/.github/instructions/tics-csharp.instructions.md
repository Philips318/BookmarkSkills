---
name: TICS CSharp Standards
description: '在创建或修改将由 TICS 或 TIOBE 验证的 C# 文件时使用。应用团队关于版权头、XML doc comments、抽象基类构造函数、每个文件一个顶级类型、namespace 一致性、异常日志记录和镜像文件同步更新的规则。'
applyTo: "**/*.cs"
---

# TICS CSharp 标准

## 范围

- 优先采用能够解决已报告规则且保持行为不变的最小修复。
- 对自动生成或已编辑 C# 代码的默认 must-fix 目标：Philips C# Coding Standard 5.33 checked levels 1 through 5。
- 将 checked levels 6 and 7 视为 best-effort：当问题局部且修复成本低时进行修复。完整的 level 1-7 交付前检查请使用 `tics-standard` skill。
- 当 level 8 问题局部且修复成本低时，也按 best-effort 处理。
- 除非 changed code 明确报告了 checked levels 9 and 10，否则将其视为低优先级。
- 如果 TICS 或 CI 已经在 touched code 上报告低优先级规则，也请一并修复。

## File Header (4@101)

- 每个新的 `.cs` 文件都必须以 Philips copyright header block 开头。
- 在 copyright 行使用当前年份。模板：

```csharp
/*Copyright <YEAR> Philips Healthcare
* All rights are reserved. Reproduction or dissemination
* in whole or in part is prohibited without the prior written
* consent of the copyright holder.
*/
```

- 编辑已有且已包含 copyright header 的文件时，不要修改年份或 header。
- 编辑缺少 header 的已有文件时，将 header 添加到文件顶部。

## XML Documentation Comments

- 每个 `public` 或 `internal` class、struct、interface 或 enum 都必须有 `<summary>` XML doc comment。
- `public` 或 `internal` 类型上的每个 `public` 或 `internal` method、property 或 event 都必须有 `<summary>` XML doc comment。
- 添加新的 `public`/`internal` 类型或成员时，立即包含 XML doc comment。
- 编辑缺少 XML doc comments 的已有 `public`/`internal` 类型或成员时，补充它们。
- 除非明确要求，不要向 `private` 或 `protected` 成员添加 XML doc comments。

## 生成约束

- 生成 null-safe 代码。只有当前控制流证明值非 null 时，才解引用该值。
- 不要在源代码中 suppress compiler warnings。
- 不要静默吞掉异常。每个 `catch` 都必须记录足够上下文以诊断失败，或带着补充上下文重新抛出。
- 避免从意外位置抛出异常，例如 property accessors、event callbacks、finalizers 或类似 utility members，除非 API contract 明确要求。
- 保持 `switch` 语句 exhaustive。
- 不要在 `for` 循环体内修改循环变量。
- 不要 lock 公开可访问对象、`this`、`Type` 实例或字符串。
- 字段默认保持 `private`。
- 新类型默认使用 `internal`，除非已有 API 要求更宽可见性。
- 每个文件只保留一个顶级类型，并按主类型命名文件。
- 不要使用 `new` 隐藏继承成员。改为 override、rename 或 refactor。
- 不要在嵌套作用域中重新声明可见名称。
- 每个 abstract base class 必须声明显式 `protected` constructor。
- 不要让 overridable method 成为 private 或 internal interface member 的直接实现面。优先使用 non-virtual wrapper 加 protected core hook、explicit interface implementation，或 sealed declaring type。
- 如果 override `Equals`，也要 override `GetHashCode`。
- 如果实现 `==`，保持其与 `Equals` 一致。
- 如果实现任何 relational operator，实现完整的 relational set。
- 如果实现 `IComparable`，保持 comparison operators 与其一致。
- 不要在 overloaded operator 实现中修改 operands。
- 不要在 class types 上 overload modifying operators。
- 当类型拥有 disposables、unmanaged resources 或 event subscriptions 时，实现 `IDisposable`。
- 避免 finalizers。如果存在 finalizer，不要在其中访问 reference-type members。
- 不要在 `using` 作用域变量超出作用域后使用它。
- 每个 event subscription 都要配对对应的 unsubscribe。
- 触发 event 前对 event delegate 做 null-check。不要在 events 中使用 callback return values。
- 不要用 `==`、`!=` 或 `Equals` 比较 floating-point values。使用 tolerance-based comparison。
- 优先使用 named constants 而不是 magic numbers，除非明显字面量已被本地代码风格接受。
- 对 read-only collections 返回稳定接口。
- 当 pattern matching 能提升安全性和清晰度时，优先使用它而不是 `as`。
- 当代码已经依赖相关能力时，添加 generic constraints。
- 遵循已有项目 namespace pattern。复用最近的已建立 namespace branch，不要发明新的。

## Repository Rules

- 当相同行为存在于 mirrored folders 中时，在同一变更中同步更新两份副本。

## 验证

- 编辑后，对 touched files 运行聚焦验证。
- 明确指出仍需要 TICS 或 CI rerun 的事项。
