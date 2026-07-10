# Common TICS Rules

此 reference 记录了此 repository 中已经出现过的 TICS patterns；编辑 C# code 时应优先检查这些规则。

## Philips C# Priority Policy

- Source of truth：CSViewer 中的 Philips C# Coding Standard 5.33，过滤条件为 `Status = CHECKED`。
- 自动应用 instruction 的 must-fix 范围：checked levels 1 through 5。
- Skill（pre-delivery）的 must-fix 范围：checked levels 1 through 7。
- Best-effort 范围：checked level 8，当 issue 局部于 touched slice 且修复成本低。
- Low-priority 范围：checked levels 9 and 10，除非 TICS 或 CI 已针对 changed code 明确报告它们。
- Reference summary：[philips-csharp-checked-level1-7.md](./philips-csharp-checked-level1-7.md)

## 20@480 Seal Methods That Satisfy Private Interfaces

- 不要让 overridable method 成为 private 或 internal interface contract 的直接实现面。
- 优先采用以下修复之一：
  - 用 non-virtual wrapper 替换 overridable entry point，并将可扩展性移入 protected core hook。
  - 显式实现 interface member。
  - 如果不需要 extensibility，seal declaring type。
- 避免 `internal virtual` method 直接满足 private interface member 这样的模式。

## 7@105 Define a Protected Constructor on Abstract Base Classes

- 每个 abstract base class 都应声明显式 `protected` constructor。
- 如果无需初始化，保持 constructor 为空。

## 7@107 Keep One Top-Level Type Per File

- 优先每个 source file 只放一个 top-level type。
- 如果 helper type 与一个 owner 强耦合且不在其他地方复用，优先使用 nested type。
- 否则将 helper 移到自己的文件中。

## 3@109 Use a Consistent Namespace Pattern

- 复用 owning project 的现有 root namespace。
- 跟随最近的 sibling namespace，而不是发明新的 namespace branch。
- 除非任务明确要求，否则避免 broad namespace renames。

## Exception Handling

- 不要静默吞掉 exceptions。
- 在 `catch` 中，要么记录足够上下文用于诊断失败，要么补充上下文并重新抛出。
- 如果某个 catch 有意设为 non-fatal，log message 要具体说明失败的 operation。

## Repository Mirror Rule

- 当某个 behavior 存在于 mirrored code paths 中时，在同一变更中更新两份副本。
- 此 repository 中常见 mirrored pairs 包括：
  - `MIA_Dev` and `MIA_Splot`
  - `MIPPP` and `MIPPP_Dev` for mirrored config or resource changes
- 验证变更触及的所有 mirrored files。

## Validation Pattern

- 第一次 substantive edit 后，对 touched files 运行可用的最窄验证。
- 优先 file-scoped errors，然后 narrow build 或 test；如果规则纯结构性，再使用 structural grep。
- 在称变更完全 compliant 前，仍需要 TICS rerun。

## 4@101 File Header (Copyright Block)

- 每个 `.cs` file 都必须以 Philips copyright header 开头。
- 创建新文件时使用当前年份。不要修改 existing headers 中的年份。
- Template:

```csharp
/*Copyright <YEAR> Philips Healthcare
* All rights are reserved. Reproduction or dissemination
* in whole or in part is prohibited without the prior written
* consent of the copyright holder.
*/
```

- 编辑缺少 header 的文件时，将 header 添加到最顶部，位于任何 `using` 或 `namespace` 之前。

## XML Documentation Comments

- 每个 `public` 或 `internal` class、struct、interface 或 enum 都必须有 `<summary>` XML doc comment。
- `public`/`internal` 类型上的每个 `public` 或 `internal` method、property 或 event 都必须有 `<summary>` XML doc comment。
- 创建新 type 或 member 时，立即包含 comment。
- 编辑缺少 comments 的 existing type 或 member 时，补充它们。
- 除非明确要求，不要向 `private` 或 `protected` members 添加 XML doc comments。
