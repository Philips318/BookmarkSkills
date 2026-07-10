# CodeScene Default Thresholds

Reference for the default threshold values used by CodeScene Code Health analysis (C#).

## Function-Level Thresholds

| Metric | Threshold | Severity |
|---|---|---|
| Cyclomatic Complexity (per function) | ≤ 9 | Complex Method |
| Function Lines of Code | language-dependent (~50-70) | Large Method |
| Nesting Depth | ≤ 2 | Nested Complexity |
| Bumpy Road blocks per function | ≤ 1 | Bumpy Road Ahead |
| Primitive type arguments ratio | ≤ 30% | Primitive Obsession |
| String arguments ratio | ≤ 39% | String Heavy Function Arguments |
| Complex Conditional operators | ≤ 2 AND/OR per branch | Complex Conditional |

## Module-Level Thresholds

| Metric | Threshold | Severity |
|---|---|---|
| Mean Cyclomatic Complexity | ≤ 4.0 | Overall Code Complexity |
| Lines of Code (file) | language-dependent (~500) | Lines of Code |
| LCOM4 (cohesion) | implementation-specific | Low Cohesion |
| Functions per file | combined with size + Brain Method | Brain Class |

## Code Health Score Scale

| Score | Category | Meaning |
|---|---|---|
| 8.0 – 10.0 | **Green** | Healthy, easy to maintain and evolve |
| 4.0 – 7.9 | **Yellow** | Complex, maintenance issues, increased defect risk |
| 1.0 – 3.9 | **Red** | Severe maintenance issues, high delivery risk |

## Alert Triggers

| Alert | Condition |
|---|---|
| Declining Code Health | Score drops by ≥ 0.1 compared to past year |
| Predicted Decline | Noticeable downward slide, not yet severe |
| File climbs hotspot ranking | Increasing activity + declining health |

## Customization

Thresholds can be overridden via `.codescene/code-health-rules.json`:

```json
{
  "rule_sets": [{
    "matching_content_path": "**/*.cs",
    "rules": [
      { "name": "Primitive Obsession", "weight": 0.5 },
      { "name": "Large Method", "weight": 0.0 }
    ],
    "thresholds": [
      { "name": "function_cyclomatic_complexity_warning", "value": 12 },
      { "name": "function_nesting_depth_warning", "value": 3 }
    ]
  }]
}
```

### Weight Values
- `1.0` = full impact (default)
- `0.5` = half impact (down-prioritized)
- `0.0` = disabled (rule not calculated)

### Path Scoping
- `test/**` — apply only to test code
- `**/*.cs` — apply only to C# files
- Can have multiple rule sets with different paths

## Local Suppression

```csharp
// @codescene(disable:"Complex Method") Legacy code, refactoring planned Q3
void LegacyProcess() { ... }

// @codescene(disable:"Bumpy Road Ahead", disable:"Complex Method")
void AnotherLegacyFunction() { ... }
```

Rules:
- Directive applies to the immediately following function
- String must exactly match the code smell name
- Always document rationale
- Never use `disable-all` in new code
