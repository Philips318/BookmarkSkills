# CodeScene Default Thresholds

CodeScene Code Health analysis（C#）使用的默认 threshold values reference。

## Function-Level Thresholds

| 指标 | 阈值 | 严重度 |
|---|---|---|
| Cyclomatic Complexity (per function) | ≤ 9 | Complex Method |
| Function Lines of Code | language-dependent (~50-70) | Large Method |
| 嵌套深度 | ≤ 2 | Nested Complexity |
| Bumpy Road blocks per function | ≤ 1 | Bumpy Road Ahead |
| Primitive type arguments ratio | ≤ 30% | 基本类型偏执 |
| String arguments ratio | ≤ 39% | String Heavy Function Arguments |
| Complex Conditional operators | ≤ 2 AND/OR per branch | 复杂条件 |

## Module-Level Thresholds

| 指标 | 阈值 | 严重度 |
|---|---|---|
| Mean Cyclomatic Complexity | ≤ 4.0 | Overall Code Complexity |
| Lines of Code (file) | language-dependent (~500) | Lines of Code |
| LCOM4 (cohesion) | implementation-specific | Low Cohesion |
| 每文件函数数 | combined with size + Brain Method | ????Brain Class? |

## Code Health Score Scale

| 分数 | 类别 | 含义 |
|---|---|---|
| 8.0 – 10.0 | **Green** | 健康，易于维护和演进 |
| 4.0 – 7.9 | **Yellow** | 复杂，存在维护问题，缺陷风险增加 |
| 1.0 – 3.9 | **Red** | 严重维护问题，交付风险高 |

## Alert Triggers

| Alert | 条件 |
|---|---|
| Declining Code Health | 与过去一年相比，score drops by ≥ 0.1 |
| Predicted Decline | 明显下滑，但尚未严重 |
| File climbs hotspot ranking | 活动增加 + health 下降 |

## Customization

Thresholds 可通过 `.codescene/code-health-rules.json` 覆盖：

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
- `1.0` = full impact（default）
- `0.5` = half impact（down-prioritized）
- `0.0` = disabled（rule not calculated）

### Path Scoping
- `test/**` — 仅应用于 test code
- `**/*.cs` — 仅应用于 C# files
- 可以包含多个 rule sets，并使用不同 paths

## Local Suppression

```csharp
// @codescene(disable:"Complex Method") Legacy code, refactoring planned Q3
void LegacyProcess() { ... }

// @codescene(disable:"Bumpy Road Ahead", disable:"Complex Method")
void AnotherLegacyFunction() { ... }
```

Rules:
- Directive 适用于紧随其后的 function
- String 必须与 code smell name 完全匹配
- 始终记录 rationale
- 不要在新代码中使用 `disable-all`
