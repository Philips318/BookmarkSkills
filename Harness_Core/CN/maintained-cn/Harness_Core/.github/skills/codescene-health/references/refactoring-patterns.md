# CodeScene Refactoring Patterns

针对每个 CodeScene code smell category 的已验证 refactoring strategies。

---

## Pattern 1: Guard Clauses（用于 Complex Method / Nested Complexity）

**Problem**：级联 if/else 导致深嵌套和高 cyclomatic complexity。

**Solution**：反转条件并 early return。

```csharp
// BEFORE — 4 levels, CC = 8
void ProcessOrder(Order order)
{
    if (order != null)
    {
        if (order.IsValid)
        {
            if (order.Items.Count > 0)
            {
                foreach (var item in order.Items)
                {
                    if (item.InStock)
                    {
                        Ship(item);
                    }
                }
            }
        }
    }
}

// AFTER — 1 level, CC = 4
void ProcessOrder(Order order)
{
    if (order is not { IsValid: true, Items.Count: > 0 }) return;

    foreach (var item in order.Items.Where(i => i.InStock))
    {
        Ship(item);
    }
}
```

---

## Pattern 2: Extract Method（用于 Bumpy Road / Complex Method）

**Problem**：Function 有多个 logical chunks — bumpy road。

**Solution**：每个 chunk 变成一个命名 method。

```csharp
// BEFORE — 4 bumps
void ImportData(RawData raw)
{
    // validate
    if (raw.Header == null) { if (strict) throw ...; else log...; }
    // parse
    if (raw.Format == "A") { ... } else if (raw.Format == "B") { ... }
    // transform
    if (needsConversion) { if (legacy) { ... } }
    // store
    if (db.IsConnected) { if (batch) { ... } else { ... } }
}

// AFTER — 1 bump per method max
void ImportData(RawData raw)
{
    ValidateHeader(raw);
    var parsed = Parse(raw);
    var transformed = Transform(parsed);
    Store(transformed);
}
```

---

## Pattern 3: Introduce Value Objects（用于 Primitive Obsession / String Heavy）

**Problem**：Functions 接收许多相同类型 primitives — 易混淆，且缺少 domain language。

**Solution**：创建 thin wrapper types。

```csharp
// Step 1: Identify clusters of related primitives
// (patientId, studyId) always travel together → PatientContext
// (sliceCount, thickness) are always paired → SliceConfig

// Step 2: Create value objects
public readonly record struct PatientId(string Value)
{
    public static PatientId Parse(string raw) =>
        string.IsNullOrWhiteSpace(raw)
            ? throw new ArgumentException("Patient ID cannot be empty")
            : new(raw.Trim());
}

public sealed record SliceConfig(int Count, double Thickness)
{
    public SliceConfig
    {
        if (Count <= 0) throw new ArgumentOutOfRangeException(nameof(Count));
        if (Thickness <= 0) throw new ArgumentOutOfRangeException(nameof(Thickness));
    }
}

// Step 3: Refactor function signatures
// BEFORE: void Setup(string patientId, int sliceCount, double thickness)
// AFTER:  void Setup(PatientId patient, SliceConfig slices)
```

**When to use record struct vs class**:
- `record struct` — 小型、immutable、不需要 inheritance（IDs、measurements）
- `sealed record` — constructor 中需要 validation 或 reference semantics
- `class` — mutable state 或 complex behavior

---

## Pattern 4: Strategy Pattern（用于带 switch/if chains 的 Complex Method）

**Problem**：大型 switch 或 if-else chain 基于 type/enum dispatch。

**Solution**：用 polymorphism 替换 conditional。

```csharp
// BEFORE — CC = 12
void Process(Message msg)
{
    switch (msg.Type)
    {
        case "A": /* 10 lines */ break;
        case "B": /* 15 lines */ break;
        case "C": /* 8 lines */ break;
        // ... more cases
    }
}

// AFTER — CC = 2 per handler
interface IMessageHandler { bool CanHandle(Message msg); void Handle(Message msg); }

void Process(Message msg)
{
    var handler = _handlers.FirstOrDefault(h => h.CanHandle(msg))
        ?? throw new InvalidOperationException($"No handler for {msg.Type}");
    handler.Handle(msg);
}
```

---

## Pattern 5: Decompose Brain Method

**Problem**：God Function 又大又复杂，深嵌套且被大量调用。

**Solution**：三步拆解。

```
Step 1: Identify the "sections" (often separated by blank lines or comments)
Step 2: Extract each section into a well-named private method
Step 3: The original becomes a "table of contents" orchestrator
```

```csharp
// AFTER decomposition — the orchestrator
void ExecuteWorkflow(WorkflowContext ctx)
{
    var input = ValidateAndNormalizeInput(ctx);
    var plan = BuildExecutionPlan(input);
    var results = ExecutePlan(plan);
    PublishResults(results);
    UpdateAuditTrail(ctx, results);
}
```

---

## Pattern 6: Table-Driven Logic（用于 Complex Conditionals）

**Problem**：多个 if/else 以相似结构将 conditions 映射到 actions。

**Solution**：替换为 dictionary 或 lookup。

```csharp
// BEFORE — CC = 8
string GetStatusLabel(int code)
{
    if (code == 0) return "Idle";
    else if (code == 1) return "Running";
    else if (code == 2) return "Paused";
    else if (code == 3) return "Error";
    else if (code == 4) return "Complete";
    else if (code == 5) return "Cancelled";
    else return "Unknown";
}

// AFTER — CC = 1
private static readonly Dictionary<int, string> StatusLabels = new()
{
    [0] = "Idle", [1] = "Running", [2] = "Paused",
    [3] = "Error", [4] = "Complete", [5] = "Cancelled"
};

string GetStatusLabel(int code) =>
    StatusLabels.GetValueOrDefault(code, "Unknown");
```

---

## Pattern 7: Split God Class（用于 Brain Class / Low Cohesion）

**Problem**：一个 class 承担过多 responsibilities，LCOM4 高。

**Solution**：识别 responsibility clusters 并提取 focused classes。

```
Step 1: Group methods by which fields they access
Step 2: Each group = one responsibility = one class
Step 3: Original class delegates to the new focused classes
```

---

## Decision Table: Which Pattern to Apply

| Code Smell | Primary Pattern | Secondary Pattern |
|---|---|---|
| Complex Method (CC > 9) | 守卫子句 | 提取方法 |
| Bumpy Road (> 1 block) | 提取方法 | — |
| Nested Complexity (depth > 2) | 守卫子句 | 提取方法 |
| 基本类型偏执 | 值对象 | Parameter Object |
| 字符串参数过多 | 值对象 | — |
| ?????Brain Method? | Decompose Brain | Strategy Pattern |
| ????Brain Class? | Split God Class | — |
| 复杂条件 | Table-Driven | Named Boolean Method |
| DRY Violations | 提取方法 | Base Class / Shared Helper |
| Overall Complexity (mean > 4) | Fix top 3-5 methods | — |
