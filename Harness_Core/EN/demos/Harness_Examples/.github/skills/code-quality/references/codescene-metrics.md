# CodeScene Metrics Reference

Quick reference for CodeScene Code Health metrics used in the quality assessment.

## Function-Level Metrics

| Metric | Description | Threshold | Collection Method |
|--------|-------------|-----------|-------------------|
| Cyclomatic Complexity | Independent execution paths | ≤ 9 | Count if/else/switch/for/while/catch/&&/\|\| |
| Nesting Depth | Max nested block depth | ≤ 2 | Count nested { } levels |
| Function LoC | Lines in function body | ≤ 50 | Count non-blank, non-comment lines |
| Primitive Obsession | Ratio of primitive params | ≤ 30% | Count int/string/bool/double params vs total |
| String Heavy Args | Ratio of string params | ≤ 39% | Count string params vs total |
| Bumpy Road | Nested conditional blocks | ≤ 1 per function | Count blocks with nesting ≥ 2 |
| Brain Method | Combined: high CC + large + deep + coupled | No threshold — composite | Flags when CC > 9 AND LoC > 50 AND nesting > 2 |

## Module-Level Metrics

| Metric | Description | Threshold | Collection Method |
|--------|-------------|-----------|-------------------|
| Mean CC | Average CC across all functions | ≤ 4.0 | Sum CC / function count |
| File LoC | Total lines in file | ≤ 500 | Count all lines |
| LCOM4 | Lack of Cohesion (connected components) | ≤ 1 ideal | Analyze field-method usage graph |
| Function Count | Functions per file | Context-dependent | Count methods + properties with logic |

## How to Collect Without CodeScene

For local assessment without the CodeScene server:

1. **CC**: Count branching keywords per function
2. **Nesting**: Track `{` depth inside functions
3. **LoC**: Exclude blank lines and single `//` comment lines
4. **Primitive ratio**: Parse method signatures
5. **Mean CC**: Sum individual CC / total function count

## Score Mapping

Map CodeScene-style findings to the D1 dimension score using the violation density table in [scoring-rubric.md](./scoring-rubric.md).
