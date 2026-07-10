# Common CodeScene Issues

此 repository 中经常遇到的问题，以及标准 resolution patterns。

## Primitive Obsession

**Rule**: ≥ 30% of all function arguments are primitive types → code lacks domain language.

**Typical finding**:
```
New issue: Primitive Obsession — 91.7% of all function arguments are primitive types, threshold = 30.0%
```

**Standard fix**：引入 value objects 或 parameter objects。

```csharp
// BEFORE — primitive obsession
public void SetupScan(string patientId, string studyId, int sliceCount, double thickness, string protocol)

// AFTER — domain types
public void SetupScan(PatientId patientId, StudyId studyId, SliceParameters slices, Protocol protocol)

// Value objects
public readonly record struct PatientId(string Value);
public readonly record struct StudyId(string Value);
public readonly record struct Protocol(string Name);
public sealed class SliceParameters
{
    public int Count { get; init; }
    public double Thickness { get; init; }
}
```

**Guideline**：将总是一起传递的相关 primitives 组合成单一类型。

---

## String Heavy Function Arguments

**Rule**: ≥ 39% of function arguments are strings → missing domain abstractions.

**Typical finding**:
```
New issue: String Heavy Function Arguments — 76.9% of all arguments to its 14 functions are strings. Threshold = 39.0%
```

**Standard fix**：用携带 validation 和 semantics 的 typed wrappers 替换 raw strings。

```csharp
// BEFORE
public void LoadDicom(string filePath, string seriesUid, string sopInstanceUid)

// AFTER
public void LoadDicom(FilePath path, SeriesUid series, SopInstanceUid instance)
```

---

## Complex Method

**Rule**: Cyclomatic complexity per function ≤ 9.

**Typical finding**:
```
Getting worse: Complex Method — SeriesUiClick increases in cyclomatic complexity from 15 to 16, threshold = 9
```

**Standard fixes**:
1. **Guard clauses** — early returns flatten nested conditions.
2. **Extract Method** — each logical block becomes its own method.
3. **Strategy / polymorphism** — replace large switch/if chains.
4. **Table-driven logic** — replace repeated conditions with dictionary lookup.

```csharp
// BEFORE — CC = 16
void SeriesUiClick(object sender, EventArgs e)
{
    if (condition1) {
        if (condition2) { ... }
        else if (condition3) { ... }
    }
    // more branches...
}

// AFTER — CC ≤ 5 per method
void SeriesUiClick(object sender, EventArgs e)
{
    if (!IsValidClick(sender)) return;
    var action = DetermineAction(sender);
    action.Execute();
}
```

---

## Bumpy Road Ahead

**Rule**: ≤ 1 block of nested conditional logic (nesting ≥ 2) per function.

**Typical finding**:
```
New issue: Bumpy Road Ahead — Convert has 6 blocks with nested conditional logic. Threshold is one single, nested block per function.
```

**Standard fix**：对每个 logical chunk 使用 Extract Method。

```csharp
// BEFORE — 6 bumps
void Convert(Data data)
{
    // bump 1
    if (a) { if (b) { ... } }
    // bump 2
    if (c) { if (d) { ... } }
    // ... 4 more bumps
}

// AFTER — 1 bump max per method
void Convert(Data data)
{
    ConvertHeader(data);
    ConvertBody(data);
    ConvertFooter(data);
    ValidateOutput(data);
    ApplyTransform(data);
    FinalizeConversion(data);
}
```

---

## Overall Code Complexity

**Rule**: Mean cyclomatic complexity across all functions in a module ≤ 4.0.

**Typical finding**:
```
Overall Code Complexity — mean cyclomatic complexity of 4.38 across 34 functions. Threshold = 4
```

**Standard fix**：聚焦最复杂的前 3-5 个 functions。降低它们的 CC 会把 mean 拉回 threshold 以下。使用与 Complex Method 相同的 refactoring patterns。

---

## Brain Method (God Function)

**Rule**：一个 function 同时大、复杂、深嵌套且中心耦合。

**Standard fix**：拆分为 orchestration + focused helpers。orchestrating method 应读起来像目录。

---

## Nested Complexity

**Rule**: Nesting depth ≤ 2 levels.

**Standard fix**：Guard clauses + Extract Method。

```csharp
// BEFORE — 4 levels deep
void Process(Order order)
{
    if (order != null)
    {
        if (order.Items.Any())
        {
            foreach (var item in order.Items)
            {
                if (item.IsValid) { ... }
            }
        }
    }
}

// AFTER — max 2 levels
void Process(Order order)
{
    if (order?.Items is not { Count: > 0 } items) return;
    foreach (var item in items)
    {
        ProcessItem(item);
    }
}
```

---

## Low Cohesion (LCOM4)

**Rule**：class 应有一个清晰职责。

**Standard fix**：使用 SRP 拆分为聚焦的协作类。

---

## DRY Violations

**Rule**：duplicated logic 会以可预测模式一起变化。

**Standard fix**：将共享逻辑提取到 common method 或 base class。
