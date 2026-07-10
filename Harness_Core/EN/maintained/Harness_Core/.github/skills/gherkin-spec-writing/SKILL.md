---
name: gherkin-spec-writing
description: Writing Gherkin feature files: Given/When/Then step quality, scenario independence, vertical slice rule, demo scenario requirements, prohibited patterns (compound Whens, omniscient Thens). Use when creating or reviewing .feature files.
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

- Never share state between scenarios via static fields or global state
- Use `Background:` only for preconditions that ALL scenarios in the file require
- Each scenario must be fully self-contained via its `Given` steps

## Demo Scenario (First Scenario Rule)

The **first `Scenario:`** in every `type: feature` feature file is the demo scenario — `@feature-demonstrator` will run it against the live application. Requirements for this scenario:
- Every `Then` step references a UI element by `AutomationId`
- Values are deterministic (reproducible with `--simulator --seed=demo`)
- The full scenario can be executed in under 2 minutes
- No steps require human intervention

## Step Reuse Guidelines

Steps are reusable across feature files when they describe domain concepts:
- ✅ `Given the CT device is connected` — domain concept, reusable
- ❌ `Given the CtDeviceController has _channel set to a SimulatedDeviceChannel` — implementation detail, not reusable

## Scenario Count

- 3–7 scenarios per feature file is optimal
- Structure: 1 happy path + 2–4 error/edge cases
- If you need > 7 scenarios, the feature is probably too large — split it

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

| Pattern | Why Prohibited |
|---------|---------------|
| `Then it works` | Not verifiable — specify the observable outcome |
| `Then no error occurs` | Too vague — specify the expected positive state |
| `When I call method X` | Leaks implementation detail — use domain language |
| `Given I set the mock to return 42` | Exposes test infrastructure — use domain setup language |
| Scenario depending on order | Causes non-deterministic failures in parallel runs |
