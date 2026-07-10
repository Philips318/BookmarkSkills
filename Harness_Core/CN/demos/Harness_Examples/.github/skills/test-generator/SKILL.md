---
name: test-generator
description: '为 C# classes 生成 unit tests（xUnit/NUnit/MSTest）。产出遵循 AAA pattern 的 test classes，覆盖 happy path、boundary values、error handling 和 edge cases。强制执行 CT software department testing standards。'
argument-hint: 'Provide a class name, method name, or file path to generate tests for'
user-invocable: true
---

# Test Generator

使用此 skill 为 CT software department 的 .NET projects 中的 C# code 生成 comprehensive unit tests。

## When to Use

- 实现 new class 或 method 后 — 生成 UT 以满足 ≥60% coverage target。
- 为 existing untested code 添加 tests 时 — systematic test generation。
- PR 前 — 确保 new code 有 adequate test coverage。
- 审查 test quality 时 — 检查 existing tests 是否遵循 AAA pattern 并覆盖 edge cases。

## Preferred Inputs

提供以下一项或多项：

- **Class or method to test** — file path、class name 或 code snippet。
- **Test framework** — xUnit（.NET 8 默认）、NUnit 或 MSTest（用于 .NET 4.8 projects）。
- **Acceptance criteria / BDD scenarios** — 如可用，将 UT 与 AC 对齐。
- **Verification Methods table** — 来自 `artifacts/<feature>/01-requirements.md`；这是要测试内容的 source of truth（见下方 [Deriving Tests from Verification Methods](#deriving-tests-from-verification-methods)）。
- **Specific focus** — 例如 "boundary values only" 或 "error handling only"。

如果未指定 framework，则从 project 检测：
- `.NET 8` projects → **xUnit** + FluentAssertions
- `.NET 4.8` projects → **MSTest** 或 **NUnit**（匹配 existing test project）

## Deriving Tests from Verification Methods

RequirementsAnalyst 会产出 **requirement-level** Verification Methods（每个 AC 一条，high-level）。Tester 负责把每一行扩展为 **implementation-level** test cases。使用此 derivation pattern：

| Verification Method element | Derivation rule |
|------------------------------|----------------|
| 前置条件 | → `Arrange` section: build the input state |
| Steps (numbered list) | → `Act` section: invoke the SUT in the same order |
| 预期结果 | → `Assert` section: at least one assertion per observable outcome |
| AC ID (row header) | → Test attribute `[Trait("AC", "AC-1")]` (xUnit) or `[TestCategory("AC-1")]` (MSTest) |

对每一行 Verification Method，至少生成这些 test cases：

1. **Happy path** — Verification Method 中的 exact scenario（1 个 test）
2. **Boundary values** — input domain 的 min、max、just-below-min、just-above-max（2–4 个 tests）
3. **Error paths** — invalid input、null、missing precondition（每个 documented error 1+ 个 tests）
4. **State variants** — 如果 SUT 是 stateful，则对 operation valid/invalid 的每个 state 各生成一个 test

这会把 5 行 Verification Method 转换为约 25–40 个 test methods，这是 Class B/C features 合适的密度。

## Domain-Anchored Boundary Values

当 SUT 操作 CT/DICOM/Spectral data 时，**从 `domain-knowledge` 拉取 boundary values** — 不要编造数字。常见 anchors：

| 领域区域 | Reference | Boundary values |
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

**Rule:** 使用编造 boundary values 的 tests（例如把 `WW = 100` 当作 "max" boundary）属于 code-review failure。boundary 必须能追溯到 domain constant。

## Coverage Targets by Safety Class

Coverage intensity 由 `artifacts/<feature>/01-requirements.md` 中的 Safety Classification 驱动：

| 安全等级 | 必需覆盖范围 |
|--------------|-------------------|
| **Class A** | P0 categories（happy + null + 至少一个 error） |
| **Class B** | P0 + P1 categories（boundary min/max、out-of-range、empty input）+ 对任何 stateful SUT 的 state-dependency tests |
| **Class C** | P0 + P1 + P2 categories（concurrency、dispose）+ 来自 `safety-rules.md` 的**每个**适用 documented error path（例如 lossy compression rejection、keV out-of-range rejection、contraindicated-workflow blocking）+ 适用时 negative authorization tests |

Test report（`artifacts/<feature>/04-test-report.md`）必须说明哪个 Safety Class 驱动了 coverage decisions。

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

遵循 pattern：`MethodName_WhenCondition_ShouldExpectedResult`

- `MethodName` — 被测试的方法。
- `WhenCondition` — specific scenario 或 input condition。
- `ShouldExpectedResult` — expected behavior。

Examples:
- `SetWindowWidth_WhenValueInRange_ShouldUpdateProperty`
- `SetWindowWidth_WhenValueBelowMinimum_ShouldThrowArgumentOutOfRange`
- `LoadImage_WhenFileNotFound_ShouldReturnNull`

### 3. Coverage Matrix

对每个 public method，按以下 categories 生成 tests：

| 类别 | Test | 优先级 |
|----------|------|----------|
| 正常路径 | Normal input → expected output | P0 |
| Null input | null parameter → ArgumentNullException | P0 |
| Empty input | empty string/collection → defined behavior | P1 |
| 边界（最小） | minimum valid value | P1 |
| 边界（最大） | maximum valid value | P1 |
| Out of range | value beyond limits → exception or clamp | P1 |
| State dependency | method called in wrong state | P2 |
| 并发 | parallel calls (if applicable) | P2 |
| Dispose / cleanup | resource release verification | P2 |

详细 patterns 见 [test-patterns.md](./references/test-patterns.md)。

### Resource Hygiene Checklist (CA2000)

当 SUT（或它调用的任何 helper）获取 `IDisposable` 时，test suite **必须**至少包含一个能在 resource leaked 时失败的 test。production code under test 中常见 leak sources：

| API | Leak risk | Required test pattern |
|-----|-----------|------------------------|
| `Process.GetCurrentProcess()` / `Process.Start(...)` | Native handle until GC | Assert wrapping `using` exists (source-text scan) OR observe handle count stable across N invocations |
| `FileStream` / `StreamReader` / `StreamWriter` | File handle until GC | Same as above, plus a test that opens the same file twice in succession |
| `HttpClient` (per-call) | Socket exhaustion | Assert single shared instance OR `using` per call |
| `SqlConnection` / `DbContext` | Connection pool starvation | `using` block enforced |
| `RegistryKey`, `Mutex`, `Semaphore`, `EventWaitHandle` | OS handle until GC | `using` block enforced |
| `Bitmap`, `Graphics`, `Font`, `Brush` (GDI+) | Unmanaged GDI handle until GC | `using` block enforced |

**Detection hint:** 声明 test class complete 前，grep production source 是否使用上述 API。如果发现但没有 `using`/`Dispose`，添加 guard test（reflection 或 source scan），并向 Reviewer 报告 Should-Fix — 不要静默放过。

这一节是在 PipelineEnvironmentInfoLogger dogfood 后加入的：一次 `Process.GetCurrentProcess().Id` 调用未使用 `using` 就被提交，test suite 没抓到，Reviewer 抓到了。lesson 是 resource-hygiene 必须是 **test obligation**，而不是只靠 Reviewer。

## Test Quality Rules

1. **AAA Pattern** — 每个 test 都必须有清楚分离的 Arrange、Act、Assert sections。
2. **One assert per test** — 每个 test 只测试一个 behavior。对同一 result 的多个相关 asserts 可接受。
3. **No test interdependence** — tests 必须能以任意顺序 pass。
4. **No real I/O** — mock file system、network、database。使用 interfaces 和 dependency injection。
5. **Deterministic** — 不使用 random values，不使用 DateTime.Now。使用 fixed test data。
6. **Fast** — 每个 test 应在 < 200ms 内完成。
7. **Readable** — test name + arrange section 应在无需 comments 的情况下说明 scenario。

## Mocking Strategy

- 对 interface mocking 使用 **Moq** 或 **NSubstitute**。
- Mock at the boundary — 只 mock external dependencies。
- 不要 mock System Under Test。
- 只有当 behavior 本身就是 interaction 时才 verify interactions（例如 event raised、service called）。

```csharp
// Example: mocking a dependency
var mockRepository = new Mock<IPatientRepository>();
mockRepository.Setup(r => r.GetById(patientId)).Returns(expectedPatient);
var sut = new PatientService(mockRepository.Object);
```

## Framework-Specific Conventions

### xUnit (.NET 8 projects)
- 单 case tests 使用 `[Fact]`
- parameterized tests 使用 `[Theory]` + `[InlineData]`
- constructor 用于 setup，`IDisposable` 用于 teardown
- shared expensive setup 使用 `IClassFixture<T>`

### MSTest (.NET 4.8 projects)
- tests 使用 `[TestMethod]`
- parameterized tests 使用 `[DataRow]`
- setup/teardown 使用 `[TestInitialize]` / `[TestCleanup]`
- class 上标注 `[TestClass]`

### NUnit (.NET 4.8 projects)
- single-case tests 使用 `[Test]`
- parameterized tests 使用 `[TestCase]`
- lifecycle 使用 `[SetUp]` / `[TearDown]`

## CI Quality Gates

Generated tests 必须满足这些 thresholds 才能通过 CI：
- new code **Coverage ≥ 60%**（line coverage）。
- **0 test failures** — 所有 tests 必须 pass。
- test project 中 **0 compiler warnings**。
- 没有 undocumented reason 的 skipped tests（`[Skip("reason")]`）。

## Portability Note

此 skill 设计为 **team-portable**。AAA pattern、framework conventions（xUnit/MSTest/NUnit）、mocking strategy 和 quality rules 都是 generic，适用于任何 .NET / C# codebase。Repository-specific facts（偏好的 mocking library、已有 test base classes、test projects 所在 folders）**不得**硬编码在这里。它们应放在：

- team-versioned `references/test-patterns.md` companion file（在其中提交 team conventions）
- `/memories/repo/` 用于 VS Code Copilot per-workspace conventions

上面的 Domain-Anchored Boundary Values section 依赖 `domain-knowledge`，它本身是单独、可替换的 skill — 另一个 team 的 domain skill 会自动提供不同 anchors。
