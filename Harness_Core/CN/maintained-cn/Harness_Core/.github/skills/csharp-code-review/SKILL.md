---
name: csharp-code-review
description: 审查 C# 代码的正确性、安全性（OWASP Top 10）、命名、复杂度、线程安全、错误处理和 IEC 62304 合规性。评估代码提交或执行白盒代码审查时使用。
---

# C# Code Review Skill

> **同时加载 `ct-coding-standards` skill** — 它定义了下面检查清单所执行的规范命名、编码、events/delegates 和 logging 规则。

## OWASP Top 10（C# 上下文）

| 风险 | 检查内容 |
|------|--------------|
| Injection | SQL/command 构建中没有字符串拼接 — 使用 parameterised queries 或 structured commands |
| Broken auth | 没有硬编码 credentials；secrets 来自 configuration 或 secret store，绝不来自 source code |
| Sensitive data | DEBUG 级别不记录 PII 或 device calibration data；exception messages 中不包含 sensitive data |
| XML/JSON parsing | 使用经过验证的 deserialisation；绝不在没有验证的情况下 `XmlDocument.LoadXml(userInput)` |
| Security misconfiguration | 没有 debug endpoints、没有 `CORS *`、没有向调用方暴露 verbose error detail |
| Vulnerable components | NuGet packages 位于当前 minor version，且没有已知 CVEs |
| Integrity failures | 不从用户可控路径加载 unsigned assemblies |
| Logging failures | 错误带完整上下文记录；没有静默吞掉异常的 `catch` blocks |
| SSRF | 不把用户可控字符串传给 `HttpClient` base URLs |

## 结构审查

- [ ] 类具有单一职责 — 类名准确描述它做什么
- [ ] 没有未在 `ExtInf/` interface 上声明的 public methods
- [ ] 方法 ≤ 20 行；超过则抽取 helpers
- [ ] Cyclomatic complexity ≤ 6；标记深层嵌套（> 3 层）或分支过多的方法
- [ ] 没有 dead code、不可达分支或从未被调用的方法
- [ ] 构建产生零 warnings — 没有无 documented justification 的 `#pragma warning disable`

## C# 惯用法

- [ ] 拥有 unmanaged resources 时正确实现 `IDisposable`
- [ ] 所有 `IDisposable` consumers 使用 `using` declarations（C# 8+）或 `using` statements
- [ ] 除 event handlers 外没有 `async void`；所有 async methods 返回 `Task` 或 `Task<T>`
- [ ] 所有 public async methods 接受并传播 `CancellationToken`
- [ ] 不对 tasks 使用 `.Result` 或 `.Wait()` — STA/dispatcher threads 上有 deadlock 风险
- [ ] Public method parameters 使用 `ArgumentNullException.ThrowIfNull()` 进行 null checks
- [ ] 没有 mutable public fields — 使用 properties
- [ ] Thread safety：共享可变状态由 `lock`、`SemaphoreSlim` 或 `Interlocked` 保护

## 文档和风格

- [ ] 所有 public 和 protected members 都有 XML doc comments（`<summary>`、`<param>`、`<returns>`、`<exception>`）
- [ ] 没有注释掉的代码块
- [ ] 代码中没有 `TODO`、`FIXME` 或 `HACK`
- [ ] 没有 magic numbers — 使用 named constants 或 configuration
- [ ] 除 loop counters 外没有单字母变量
- [ ] 名称中没有 abbreviations
- [ ] 没有 unused `using` statements
- [ ] 非 primitive locals 使用 `var`；primitive types（`int`、`string` 等）使用 predefined names
- [ ] 字符串相等使用 `CompareOrdinal`；反复修改字符串时使用 `StringBuilder`
- [ ] 适用时变量名包含单位：`positionInMm`、`timeoutInMs`

## 架构（项目特定）

- [ ] Production classes 实现 `ExtInf/` interfaces — 没有绕过
- [ ] 类体内部不对 service dependencies 使用 `new` — 只使用 constructor injection
- [ ] `Src/` production code 不引用 test assemblies
- [ ] 遵守分层：UI → Application → Domain → Infrastructure（没有向上引用）
- [ ] 新 public types 记录其架构层和安全级别（IEC 62304）

## Events 和 Threading

> 规则定义在 `ct-coding-standards` skill 中。

- [ ] 所有注册的 events 都会在 dispose 时取消注册
- [ ] 触发 events 前进行 null-check：`handler?.Invoke(this, args)`
- [ ] 不用 `Thread.Sleep()` 做同步
- [ ] 共享可变状态由 `lock`、`SemaphoreSlim` 或 `Interlocked` 保护

## Logging

> 规则定义在 `ct-coding-standards` skill 中。

- [ ] DEBUG 用于 trace/diagnostics，EVENT 用于 user actions，ERROR 用于 recoverable，CRITICAL 只在 top level 用于 unrecoverable
- [ ] 任何级别都不记录 ePHI 或 device calibration secrets
- [ ] tight loops 内没有 log statements
- [ ] 错误带完整上下文记录 — `catch` blocks 中没有 silent swallowing

## Quality Gate Reports

发出 review verdict 前，确认自动化质量门禁通过：

- [ ] **TICS**：所有被测特征 TQI ≥ 8.0 — 没有红色特征
- [ ] **Coverity**：零个新增 High defects；任何新增 Medium defects 都有引用 finding ID 的 justification comment
- [ ] **ReSharper**：审查已发布报告 — 标记任何 developer 应处理的 errors 或 warnings，即使它们非阻塞
- [ ] 没有质量门禁在 PR description 中缺少 documented waiver 的情况下被 suppress 或跳过

如果门禁结果不可用（pipeline 未运行、工具失败），review verdict 为 **BLOCKED**，直到门禁被确认。
