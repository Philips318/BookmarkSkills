# CodeScene Metrics Reference

质量评估中使用的 CodeScene Code Health metrics 快速 reference。

## Function-Level Metrics

| 指标 | 描述 | 阈值 | 采集方法 |
|--------|-------------|-----------|-------------------|
| 圈复杂度 | Independent execution paths | ≤ 9 | 统计 if/else/switch/for/while/catch/&&/\|\| |
| 嵌套深度 | Max nested block depth | ≤ 2 | 统计 nested { } levels |
| Function LoC | Lines in function body | ≤ 50 | 统计 non-blank、non-comment lines |
| 基本类型偏执 | Ratio of primitive params | ≤ 30% | 统计 int/string/bool/double params vs total |
| 字符串参数过多 | Ratio of string params | ≤ 39% | 统计 string params vs total |
| ?????Bumpy Road? | Nested conditional blocks | ≤ 1 per function | 统计 nesting ≥ 2 的 blocks |
| ?????Brain Method? | Combined: high CC + large + deep + coupled | No threshold — composite | 当 CC > 9 AND LoC > 50 AND nesting > 2 时标记 |

## Module-Level Metrics

| 指标 | 描述 | 阈值 | 采集方法 |
|--------|-------------|-----------|-------------------|
| 平均圈复杂度 | Average CC across all functions | ≤ 4.0 | Sum CC / function count |
| 文件代码行数 | Total lines in file | ≤ 500 | Count all lines |
| LCOM4 | Lack of Cohesion (connected components) | ≤ 1 ideal | Analyze field-method usage graph |
| Function Count | 每文件函数数 | Context-dependent | Count methods + properties with logic |

## How to Collect Without CodeScene

本地评估时如果没有 CodeScene server：

1. **CC**：按 function 统计 branching keywords
2. **Nesting**：跟踪 functions 内的 `{` depth
3. **LoC**：排除 blank lines 和单行 `//` comment lines
4. **Primitive ratio**：解析 method signatures
5. **Mean CC**：Sum individual CC / total function count

## Score Mapping

使用 [scoring-rubric.md](./scoring-rubric.md) 中的 violation density table，将 CodeScene-style findings 映射到 D1 dimension score。
