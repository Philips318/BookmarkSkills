# CodeScene Refactoring Patterns

Proven refactoring strategies for each CodeScene code smell category.

---

## Pattern 1: Guard Clauses (for Complex Method / Nested Complexity)

**Problem**: Deep nesting and high cyclomatic complexity from cascading if/else.

**Solution**: Invert conditions and return early.

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

## Pattern 2: Extract Method (for Bumpy Road / Complex Method)

**Problem**: Function has multiple logical chunks — bumpy road.

**Solution**: Each chunk becomes a named method.

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

## Pattern 3: Introduce Value Objects (for Primitive Obsession / String Heavy)

**Problem**: Functions take many primitives of the same type — easy to mix up, no domain language.

**Solution**: Create thin wrapper types.

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
- `record struct` — small, immutable, no inheritance needed (IDs, measurements)
- `sealed record` — needs validation in constructor or reference semantics
- `class` — mutable state or complex behavior

---

## Pattern 4: Strategy Pattern (for Complex Method with switch/if chains)

**Problem**: Large switch or if-else chain dispatching on type/enum.

**Solution**: Replace conditional with polymorphism.

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

**Problem**: A God Function that is large + complex + deeply nested + called by many.

**Solution**: Three-step decomposition.

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

## Pattern 6: Table-Driven Logic (for Complex Conditionals)

**Problem**: Multiple if/else with similar structure mapping conditions to actions.

**Solution**: Replace with dictionary or lookup.

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

## Pattern 7: Split God Class (for Brain Class / Low Cohesion)

**Problem**: One class with too many responsibilities, high LCOM4.

**Solution**: Identify responsibility clusters and extract focused classes.

```
Step 1: Group methods by which fields they access
Step 2: Each group = one responsibility = one class
Step 3: Original class delegates to the new focused classes
```

---

## Decision Table: Which Pattern to Apply

| Code Smell | Primary Pattern | Secondary Pattern |
|---|---|---|
| Complex Method (CC > 9) | Guard Clauses | Extract Method |
| Bumpy Road (> 1 block) | Extract Method | — |
| Nested Complexity (depth > 2) | Guard Clauses | Extract Method |
| Primitive Obsession | Value Objects | Parameter Object |
| String Heavy Arguments | Value Objects | — |
| Brain Method | Decompose Brain | Strategy Pattern |
| Brain Class | Split God Class | — |
| Complex Conditional | Table-Driven | Named Boolean Method |
| DRY Violations | Extract Method | Base Class / Shared Helper |
| Overall Complexity (mean > 4) | Fix top 3-5 methods | — |
