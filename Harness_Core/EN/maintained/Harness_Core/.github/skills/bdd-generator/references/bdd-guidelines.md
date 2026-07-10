## Gherkin Writing Rules

### Language
- Use **English** for Gherkin keywords (Given/When/Then).
- Use **Chinese or English** for scenario descriptions — match team convention.
- Keep step text concise — under 80 characters per line.

### Given (Arrange)
- Describe the initial state, NOT the setup actions.
- ✅ `Given the patient has an active scan session`
- ❌ `Given I click the Start button and wait for the scan to initialize`

### When (Act)
- Describe exactly ONE action or event.
- ✅ `When the user adjusts WW to 400`
- ❌ `When the user adjusts WW to 400 and WC to 40 and clicks Apply`

### Then (Assert)
- Describe the expected outcome, verifiable and measurable.
- ✅ `Then the image contrast should update within 200ms`
- ❌ `Then the image should look correct`

### And / But
- Use for additional preconditions or assertions.
- ✅ `And the rendering mode is set to MIP`
- ✅ `But the original image data should remain unchanged`

## Scenario Outline Best Practices

- Use when the same behavior is tested with different data.
- Minimum 3 rows in Examples table (happy, boundary, error).
- Name the Examples table if multiple tables exist.

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

| Tag | Purpose | Example |
|-----|---------|---------|
| `@AC-n` | Link to acceptance criterion | `@AC-1` |
| `@module-xxx` | Module scope | `@module-tissue` |
| `@class-B` | IEC 62304 safety class | `@class-B` |
| `@smoke` | Smoke test suite | `@smoke` |
| `@regression` | Regression suite | `@regression` |
| `@wip` | Work in progress | `@wip` |

## Anti-Patterns to Avoid

1. **Testing implementation, not behavior** — scenarios should survive refactoring.
2. **Too many steps** — if a scenario has > 8 steps, split it.
3. **Shared mutable state** — each scenario must start from a clean state.
4. **Incidental details** — only include details relevant to the behavior being tested.
5. **Missing negative tests** — always test what should NOT happen.
