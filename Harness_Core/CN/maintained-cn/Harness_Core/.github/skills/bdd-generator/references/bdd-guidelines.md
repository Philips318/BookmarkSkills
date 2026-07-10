## Gherkin Writing Rules

### Language
- Gherkin keywords 使用 **English**（Given/When/Then）。
- Scenario descriptions 可用 **Chinese or English** — 匹配团队约定。
- Step text 保持简洁 — 每行少于 80 characters。

### Given (Arrange)
- 描述 initial state，而不是 setup actions。
- ✅ `Given the patient has an active scan session`
- ❌ `Given I click the Start button and wait for the scan to initialize`

### When (Act)
- 精确描述 ONE action 或 event。
- ✅ `When the user adjusts WW to 400`
- ❌ `When the user adjusts WW to 400 and WC to 40 and clicks Apply`

### Then (Assert)
- 描述 expected outcome，必须可验证、可测量。
- ✅ `Then the image contrast should update within 200ms`
- ❌ `Then the image should look correct`

### And / But
- 用于 additional preconditions 或 assertions。
- ✅ `And the rendering mode is set to MIP`
- ✅ `But the original image data should remain unchanged`

## Scenario Outline Best Practices

- 当相同行为需要用不同数据测试时使用。
- Examples table 至少 3 行（happy、boundary、error）。
- 如果存在多个 tables，请命名 Examples table。

```gherkin
Scenario Outline: Window width boundary validation
  Given the image viewer is displaying a CT scan
  When the user sets WW to <ww_value>
  Then the system should <behavior>

  Examples: Valid range
    | ww_value | behavior                    |
    | 1        | accept and apply the value  |
    | 4096     | accept and apply the value  |

  Examples: Invalid range
    | ww_value | behavior                          |
    | 0        | reject with validation error      |
    | -1       | reject with validation error      |
    | 4097     | clamp to maximum value 4096       |
```

## Tagging Convention

| 标签 | 用途 | 示例 |
|-----|---------|---------|
| `@AC-n` | Link to acceptance criterion | `@AC-1` |
| `@module-xxx` | Module scope | `@module-tissue` |
| `@class-B` | IEC 62304 safety class | `@class-B` |
| `@smoke` | Smoke test suite | `@smoke` |
| `@regression` | Regression suite | `@regression` |
| `@wip` | Work in progress | `@wip` |

## Anti-Patterns to Avoid

1. **Testing implementation, not behavior** — scenarios 应能经受 refactoring。
2. **Too many steps** — 如果 scenario 超过 8 steps，请拆分。
3. **Shared mutable state** — 每个 scenario 必须从 clean state 开始。
4. **Incidental details** — 只包含与被测 behavior 相关的 details。
5. **Missing negative tests** — 始终测试不应发生的事情。
