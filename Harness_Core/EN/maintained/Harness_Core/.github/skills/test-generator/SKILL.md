---
name: test-generator
description: 'Generate unit tests (xUnit/NUnit/MSTest) for C# classes. Produces test classes with AAA pattern, covers happy path, boundary values, error handling, and edge cases. Enforces CT software department testing standards.'
argument-hint: 'Provide a class name, method name, or file path to generate tests for'
user-invocable: true
---

# Test Generator

Use this skill to generate comprehensive unit tests for C# code in the CT software department's .NET projects.

## When to Use

- After implementing a new class or method — generate UT to meet the ≥60% coverage target.
- When adding tests to existing untested code — systematic test generation.
- Before a PR — ensure new code has adequate test coverage.
- When reviewing test quality — check if existing tests follow AAA pattern and cover edge cases.

## Preferred Inputs

Provide one or more of the following:

- **Class or method to test** — file path, class name, or code snippet.
- **Test framework** — xUnit (default for .NET 8), NUnit, or MSTest (for .NET 4.8 projects).
- **Acceptance criteria / BDD scenarios** — if available, align UT with AC.
- **Verification Methods table** — from `artifacts/<feature>/01-requirements.md`; this is the source of truth for what to test (see [Deriving Tests from Verification Methods](#deriving-tests-from-verification-methods) below).
- **Specific focus** — e.g., "boundary values only" or "error handling only".

If no framework is specified, detect from the project:
- `.NET 8` projects → **xUnit** + FluentAssertions
- `.NET 4.8` projects → **MSTest** or **NUnit** (match existing test project)

## Deriving Tests from Verification Methods

The RequirementsAnalyst produces **requirement-level** Verification Methods (one per AC, high-level). The Tester is responsible for expanding each row into **implementation-level** test cases. Use this derivation pattern:

| Verification Method element | Derivation rule |
|------------------------------|----------------|
| Precondition | → `Arrange` section: build the input state |
| Steps (numbered list) | → `Act` section: invoke the SUT in the same order |
| Expected Result | → `Assert` section: at least one assertion per observable outcome |
| AC ID (row header) | → Test attribute `[Trait("AC", "AC-1")]` (xUnit) or `[TestCategory("AC-1")]` (MSTest) |

For each Verification Method row, generate **at least these test cases**:

1. **Happy path** — exact scenario from Verification Method (1 test)
2. **Boundary values** — min, max, just-below-min, just-above-max from the input domain (2–4 tests)
3. **Error paths** — invalid input, null, missing precondition (1+ tests per documented error)
4. **State variants** — if the SUT is stateful, one test per state where the operation is valid/invalid

This turns 5 Verification Method rows into roughly 25–40 test methods, which is the right density for Class B/C features.

## Domain-Anchored Boundary Values

When the SUT operates on CT/DICOM/Spectral data, **pull boundary values from `domain-knowledge`** — do not invent numbers. Common anchors:

| Domain area | Reference | Boundary values |
|-------------|-----------|-----------------|
| Spectral MonoE keV | `spectral-knowledge.md` | 39 (just below min), 40 (min), 200 (max), 201 (just above max) |
| Iodine accuracy threshold | `spectral-knowledge.md` | 5 mg/ml (the documented accuracy boundary) |
| Concurrent spectral results | `spectral-knowledge.md` | 1, 4, 5 (typical, max, over-limit → reject) |
| Window Width | `ct-glossary.md` | 0, 1, 4096, 4097 |
| Patient position (IOP) | `ct-glossary.md` | 8 documented positions → 8 Theory inline cases |
| Slice thickness | `dicom-patterns.md` | 0.625, 1.0, 5.0 mm (common production values) |
| Transfer syntax | `dicom-patterns.md` | ImplicitVR / ExplicitVR / JPEGLossless / JPEG2000 → 4 Theory cases |
| EFOV spectral limit | `spectral-knowledge.md` | 499 (accept), 500 (boundary), 501 (reject) |
| Photometric Interpretation | `dicom-patterns.md` | MONOCHROME1, MONOCHROME2, RGB |

**Rule:** Tests using made-up boundary values (e.g., `WW = 100` as a "max" boundary) are a code-review failure. The boundary must trace back to a domain constant.

## Coverage Targets by Safety Class

Coverage intensity is driven by the Safety Classification in `artifacts/<feature>/01-requirements.md`:

| Safety Class | Required Coverage |
|--------------|-------------------|
| **Class A** | P0 categories (happy + null + at least one error) |
| **Class B** | P0 + P1 categories (boundary min/max, out-of-range, empty input) + state-dependency tests for any stateful SUT |
| **Class C** | P0 + P1 + P2 categories (concurrency, dispose) + **every** documented error path from `safety-rules.md` that applies (e.g., lossy compression rejection, keV out-of-range rejection, contraindicated-workflow blocking) + negative authorization tests where applicable |

The test report (`artifacts/<feature>/04-test-report.md`) must state which Safety Class drove the coverage decisions.

## Output Structure

### 1. Test Class

```csharp
/// <summary>
/// Unit tests for <see cref="ClassName"/>.
/// </summary>
public class ClassNameTests
{
    // System Under Test
    private readonly ClassName _sut;

    public ClassNameTests()
    {
        // Common setup
        _sut = new ClassName();
    }

    [Fact] // or [Test] for NUnit, [TestMethod] for MSTest
    public void MethodName_WhenCondition_ShouldExpectedResult()
    {
        // Arrange
        var input = ...;

        // Act
        var result = _sut.MethodName(input);

        // Assert
        result.Should().Be(expected);
    }
}
```

### 2. Test Naming Convention

Follow the pattern: `MethodName_WhenCondition_ShouldExpectedResult`

- `MethodName` — the method being tested.
- `WhenCondition` — the specific scenario or input condition.
- `ShouldExpectedResult` — the expected behavior.

Examples:
- `SetWindowWidth_WhenValueInRange_ShouldUpdateProperty`
- `SetWindowWidth_WhenValueBelowMinimum_ShouldThrowArgumentOutOfRange`
- `LoadImage_WhenFileNotFound_ShouldReturnNull`

### 3. Coverage Matrix

For each public method, generate tests across:

| Category | Test | Priority |
|----------|------|----------|
| Happy path | Normal input → expected output | P0 |
| Null input | null parameter → ArgumentNullException | P0 |
| Empty input | empty string/collection → defined behavior | P1 |
| Boundary (min) | minimum valid value | P1 |
| Boundary (max) | maximum valid value | P1 |
| Out of range | value beyond limits → exception or clamp | P1 |
| State dependency | method called in wrong state | P2 |
| Concurrency | parallel calls (if applicable) | P2 |
| Dispose / cleanup | resource release verification | P2 |

See [test-patterns.md](./references/test-patterns.md) for detailed patterns.

### Resource Hygiene Checklist (CA2000)

When the SUT (or any helper it calls) acquires an `IDisposable`, the test suite **must** include at least one test that fails if the resource is leaked. Common leak sources to scan for in the production code under test:

| API | Leak risk | Required test pattern |
|-----|-----------|------------------------|
| `Process.GetCurrentProcess()` / `Process.Start(...)` | Native handle until GC | Assert wrapping `using` exists (source-text scan) OR observe handle count stable across N invocations |
| `FileStream` / `StreamReader` / `StreamWriter` | File handle until GC | Same as above, plus a test that opens the same file twice in succession |
| `HttpClient` (per-call) | Socket exhaustion | Assert single shared instance OR `using` per call |
| `SqlConnection` / `DbContext` | Connection pool starvation | `using` block enforced |
| `RegistryKey`, `Mutex`, `Semaphore`, `EventWaitHandle` | OS handle until GC | `using` block enforced |
| `Bitmap`, `Graphics`, `Font`, `Brush` (GDI+) | Unmanaged GDI handle until GC | `using` block enforced |

**Detection hint:** before declaring a test class complete, grep the production source for the APIs above. If found without a `using`/`Dispose`, add a guard test (reflection or source scan) **and** report a Should-Fix to the Reviewer — do not silently let it through.

This section was added after the PipelineEnvironmentInfoLogger dogfood: a `Process.GetCurrentProcess().Id` call was shipped without `using`, the test suite missed it, and the Reviewer caught it. The lesson is that resource-hygiene must be a **test obligation**, not a Reviewer-only obligation.

## Test Quality Rules

1. **AAA Pattern** — every test must have clearly separated Arrange, Act, Assert sections.
2. **One assert per test** — test exactly one behavior. Multiple related asserts on the same result are acceptable.
3. **No test interdependence** — tests must pass in any order.
4. **No real I/O** — mock file system, network, database. Use interfaces and dependency injection.
5. **Deterministic** — no random values, no DateTime.Now. Use fixed test data.
6. **Fast** — each test should complete in < 200ms.
7. **Readable** — test name + arrange section should explain the scenario without comments.

## Mocking Strategy

- Use **Moq** or **NSubstitute** for interface mocking.
- Mock at the boundary — external dependencies only.
- Do NOT mock the System Under Test.
- Verify interactions only when the behavior IS the interaction (e.g., event raised, service called).

```csharp
// Example: mocking a dependency
var mockRepository = new Mock<IPatientRepository>();
mockRepository.Setup(r => r.GetById(patientId)).Returns(expectedPatient);
var sut = new PatientService(mockRepository.Object);
```

## Framework-Specific Conventions

### xUnit (.NET 8 projects)
- `[Fact]` for single-case tests
- `[Theory]` + `[InlineData]` for parameterized tests
- Constructor for setup, `IDisposable` for teardown
- `IClassFixture<T>` for shared expensive setup

### MSTest (.NET 4.8 projects)
- `[TestMethod]` for tests
- `[DataRow]` for parameterized tests
- `[TestInitialize]` / `[TestCleanup]` for setup/teardown
- `[TestClass]` on the class

### NUnit (.NET 4.8 projects)
- `[Test]` for single-case tests
- `[TestCase]` for parameterized tests
- `[SetUp]` / `[TearDown]` for lifecycle

## CI Quality Gates

Generated tests must meet these thresholds to pass CI:
- **Coverage ≥ 60%** for new code (line coverage).
- **0 test failures** — all tests must pass.
- **0 compiler warnings** in test project.
- **No skipped tests** without a documented reason (`[Skip("reason")]`).

## Portability Note

This skill is designed to be **team-portable**. The AAA pattern, framework conventions (xUnit/MSTest/NUnit), mocking strategy, and quality rules are generic and apply to any .NET / C# codebase. Repository-specific facts (which mocking library is preferred, which test base classes exist, which folders host test projects) **must not** be hardcoded here. They belong in:

- A team-versioned `references/test-patterns.md` companion file (commit your team's conventions there)
- `/memories/repo/` for VS Code Copilot per-workspace conventions

The Domain-Anchored Boundary Values section above relies on `domain-knowledge` which itself is a separate, replaceable skill — another team's domain skill will supply different anchors automatically.
