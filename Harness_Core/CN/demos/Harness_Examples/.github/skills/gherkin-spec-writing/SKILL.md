---
name: gherkin-spec-writing
description: 编写 Gherkin feature files：Given/When/Then step 质量、scenario independence、vertical slice rule、demo scenario requirements、prohibited patterns（compound Whens、omniscient Thens）。创建或审查 .feature files 时使用。
---

# Gherkin Spec Writing Skill

## Declarative vs Imperative

```gherkin
# WRONG — imperative (describes HOW, leaks implementation)
When I call GetMeasurementAsync() on the CtDeviceController
Then the return value is 42.5

# CORRECT — declarative (describes WHAT the system does)
When the operator requests the current CT device measurement
Then the position display shows "42.5 mm"
```

## Scenario Independence

- 绝不通过 static fields 或 global state 在 scenarios 之间共享状态
- 仅当文件中所有 scenarios 都需要某些 preconditions 时，才使用 `Background:`
- 每个 scenario 都必须通过自己的 `Given` steps 完全自包含

## Demo Scenario（First Scenario Rule）

每个 `type: feature` feature file 中的**第一个 `Scenario:`** 是 demo scenario — `@feature-demonstrator` 会针对 live application 运行它。此 scenario 的要求：
- 每个 `Then` step 都通过 `AutomationId` 引用 UI element
- Values 是确定性的（可通过 `--simulator --seed=demo` 复现）
- 完整 scenario 可在 2 分钟内执行
- 没有 steps 需要人工介入

## Step Reuse Guidelines

当 steps 描述 domain concepts 时，可跨 feature files 复用：
- ✅ `Given the CT device is connected` — domain concept，可复用
- ❌ `Given the CtDeviceController has _channel set to a SimulatedDeviceChannel` — implementation detail，不可复用

## Scenario Count

- 每个 feature file 3–7 个 scenarios 最佳
- 结构：1 条 happy path + 2–4 条 error/edge cases
- 如果需要 > 7 个 scenarios，feature 可能太大 — 拆分它

## Scenario Outline for Boundary Testing

```gherkin
Scenario Outline: Position display rounds to one decimal place
  Given the device reports a raw position of <raw_mm>
  Then the PositionDisplay shows "<displayed>"

  Examples:
    | raw_mm | displayed |
    | 42.55  | 42.6 mm   |
    | 42.54  | 42.5 mm   |
    | 0.001  | 0.0 mm    |
```

## Prohibited Patterns

| 模式 | 禁止原因 |
|---------|---------------|
| `Then it works` | 不可验证 — 指定可观察结果 |
| `Then no error occurs` | 过于模糊 — 指定预期的正向状态 |
| `When I call method X` | 泄露 implementation detail — 使用 domain language |
| `Given I set the mock to return 42` | 暴露 test infrastructure — 使用 domain setup language |
| Scenario depending on order | 会导致并行运行中的非确定性失败 |
