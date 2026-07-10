---
name: CodeScene Code Health
description: '在创建或修改由 CodeScene 监控的 C# 文件时使用。执行函数复杂度限制，避免 primitive obsession，防止 bumpy road 模式，并保持健康的模块内聚性。'
applyTo: "**/*.cs"
---

# CodeScene Code Health 标准

## 范围

- 优先采用能够解决已报告 code health 问题的最小重构。
- 默认必须修复：任何会导致文件 Code Health 分数降到 Green（≥ 8.0）以下的问题。
- Code Health 下降告警是高优先级事项，绝不能让现有代码变差。
- PR 质量门禁失败必须在合并前解决。

## 函数级约束

- 每个函数的圈复杂度保持 ≤ 9。
- 函数体保持简短。优先 ≤ 20 条逻辑语句；没有正当理由时不得超过 50 行。
- 避免 Primitive Obsession：如果函数接收超过 2 个来自同一逻辑领域的 primitive 参数，请将其封装为 value object、record 或 DTO。
- 避免 String Heavy Arguments：当多个原始字符串表示不同领域概念时，不要向函数传入超过 2 个原始字符串。引入类型化包装。
- 函数必须保持单一抽象层级。不要把高层编排与低层细节混在一起。
- 不要创建 Brain Methods：函数不应同时具备高复杂度、大体量、深嵌套和中心耦合。

## 实现约束

- 避免 Bumpy Road：每个函数最多只包含一个嵌套条件逻辑块（嵌套 ≥ 2）。将其他块提取为独立方法。
- 将嵌套深度限制为 2 层。使用 guard clauses（early return）压平深层嵌套代码。
- 保持复杂条件简单：如果分支条件使用超过 2 个逻辑运算符（&&/||），请将其提取为有名称的 boolean 方法或变量。
- 不要在测试中重复 assertion 块。将共享 assertion 提取为 helper 方法。

## 模块级约束

- 保持模块内所有函数的平均圈复杂度 ≤ 4.0。
- 注意 Low Cohesion：类应具备一个清晰职责。如果 LCOM4 很高，请考虑拆分。
- 避免 God Class：一个文件不应组合大量函数、大量 LoC 和 Brain Methods。
- 保持文件大小合理。超过 500 行的文件值得审查是否需要拆分。

## 重构模式

- 针对 Primitive Obsession / String Heavy：引入 `record struct` value object 或专用 parameter object。
- 针对 Bumpy Road / Nested Complexity：使用 Extract Method 隔离每个逻辑块。
- 针对 Complex Method：应用 guard clauses、strategy pattern 或 polymorphism，替换大型 switch/if 链。
- 针对 Brain Class：应用 Single Responsibility Principle，拆分为职责聚焦的协作类。

## CodeScene 指令

- 只有在有理由且暂时无法重构 legacy code 时，才使用 `// @codescene(disable:"...")`。
- 使用指令时，始终在同一行记录理由。
- 不要在新代码中使用 `disable-all`。

## 验证

- 编辑后，验证变更没有增加圈复杂度或嵌套深度。
- 明确指出仍需要 CodeScene PR review 或完整 analysis rerun 的事项。
