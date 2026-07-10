---
name: ct-coding-standards
description: 所有 skills 共享的规范 CT 项目 C# 编码标准：命名约定、核心编码规则、events/delegates 和 logging levels。只要在 CT 项目中编写或审查 C# 代码，就加载此 skill。
---

# CT Coding Standards

所有 CT C# 代码的共享参考。由 `csharp-development` 和 `csharp-code-review` skills 引用。

## 命名约定

| 元素 | 约定 | 示例 |
|---------|-----------|----------|
| Class / Struct | PascalCase | `CtDeviceController` |
| Public method | PascalCase | `GetPositionAsync()` |
| Private field | `_camelCase` | `_deviceChannel` |
| Property | PascalCase | `CurrentPosition` |
| 接口 | `I` + PascalCase | `IDeviceChannel` |
| Constant | PascalCase | `MaxPositionMm` |
| Local variable | camelCase | `positionValue` |
| Async method | suffix `Async` | `GetPositionAsync()` |

- 在变量名中包含单位：`positionInMm`、`timeoutInMs`、`angleInDeg`
- `bool` members 和 methods 以前缀 `is`、`has` 或 `can` 开头：`isConnected`、`hasData`
- 除 loop counters（`i`、`j`、`k`）外，不使用单字符名称
- 不使用 abbreviations — 使用完整描述性词语
- 文件名必须匹配 class name（PascalCase）

## 核心编码规则

- 局部变量声明使用 `var`；例外：primitive types（`int`、`string`、`double` 等）— 使用 predefined type name
- 没有 unused `using` statements
- 没有 magic numbers — 使用 named constants 或 configuration values
- 方法 ≤ 20 行；类 ≤ 200 行
- 新代码中每个方法的 cyclomatic complexity ≤ 6
- 没有 public mutable fields — 使用 properties
- 除非显式设计为可继承，否则将 classes 标记为 `sealed`
- Constructors 只做构造 — 不放 business logic
- 字符串相等比较使用 `CompareOrdinal`；字符串被反复修改时使用 `StringBuilder`
- 所有 UI strings 来自 resource files — 不硬编码 strings 或 paths
- 避免 WPF code-behind；严格使用 MVVM
- 构建必须产生零 warnings；warnings 即 errors
- 代码中没有注释掉的代码；没有 `TODO`、`FIXME` 或 `HACK`

## Events and Delegates

- 所有注册的 events 都必须在 subscriber disposed 时取消注册
- 触发 event 前进行 null-check：`handler?.Invoke(this, args)`
- Event handlers 不能是 public — 不允许外部直接调用
- 如果 event 来自非 UI thread 且会更新 UI，请通过 `Dispatcher.Invoke` marshal
- 不用 `Thread.Sleep()` 做同步 — 使用 events 或 semaphores

## Logging

- DEBUG：method entry/exit、internal state traces
- EVENT：user-initiated actions
- ERROR：recoverable errors — 由处理它们的类记录
- CRITICAL：unrecoverable errors — 只在 top level 记录一次
- 任何级别都绝不记录 ePHI（patient data）或 device calibration secrets
- tight loops 内没有 log statements
