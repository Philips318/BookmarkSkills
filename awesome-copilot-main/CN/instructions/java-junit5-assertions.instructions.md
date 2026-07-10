---
description: "Standardizes JUnit 5 (Jupiter) assertions with best practices for performance, readability, and modern features (5.8+). Covers Supplier messages, assertAll, assertThrowsExactly, and performance-critical timeouts."
applyTo: "**/*Test.java, **/*IT.java, **/*Steps.java, **/*StepDefs.java"
---
# JUnit 5断言最佳实践

在使用JUnit Jupiter （JUnit 5）编写、审查或重构Java测试代码时，请遵循这些最佳实践。这些规则侧重于测试准确性、性能（惰性评估）和利用现代木星特性。

# # 1。进口

建议对断言进行静态导入，以减少样板文件。除非您的团队约定另有规定，否则更倾向于显式导入，而不是通配符（`*`）导入。```java
// ❌ BAD — verbose and clutters the test method
Assertions.assertEquals(expected, actual);

// ❌ BAD — wildcard import (unless standard in your team)
import static org.junit.jupiter.api.Assertions.*;

// ✅ GOOD — explicit static import
import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertTrue;

assertEquals(expected, actual);
```
> **最适合**：提高可读性并保持测试方法专注于逻辑。总是从`org.junit.jupiter.api.Assertions`导入。

# # 2。asserequals -期望值优先`expected`总是**第一个**参数，`actual`总是**第二个**参数。```java
// ❌ BAD — swapped; failure message is misleading
assertEquals(calculator.add(1, 1), 2);

// ✅ GOOD
assertEquals(2, calculator.add(1, 1));

// ✅ GOOD — floating point: always provide a delta
assertEquals(0.3, 0.1 + 0.2, 1e-9);
```
> **最适合**：确保故障日志正确报告“预期的[X]但却是[Y]”。

# # 3。失败消息-供应商与字符串

当消息构造开销较大（例如，字符串格式化或复杂对象检查）时，将失败消息作为`Supplier<String>`传递。```java
// ❌ BAD — expensive message constructed even when the assertion passes
assertEquals(expected, actual, "Expected %s but got %s".formatted(expected, actual));

// ✅ GOOD — evaluated only on failure (Lazy evaluation)
assertEquals(expected, actual,
    () -> "Expected %s but got %s".formatted(expected, actual));

// ✅ GOOD — simple, constant string literal (zero overhead)
assertTrue(isActive, "User account must be active");
```
> **最适合**：性能关键型测试套件和复杂的诊断消息。

# # 4。assertAll -组相关断言

当检查相同结果的多个属性时，使用`assertAll`。即使先前的断言失败，所有断言也会运行。```java
// ❌ BAD — stops at first failure; other properties go unchecked
assertEquals("Jane", person.firstName());
assertEquals("Doe",  person.lastName());

// ✅ GOOD
assertAll("person",
    () -> assertEquals("Jane", person.firstName()),
    () -> assertEquals("Doe",  person.lastName()),
    () -> assertEquals(30,     person.age())
);
```
b> **最适合**：全面的对象状态验证和避免“部分失败”歧义。

# # 5。异常测试——assertThrows vs assertThrowsExactly`assertThrows`返回用于进一步验证的异常。使用`assertThrowsExactly`进行严格的类型匹配。```java
// ✅ assertThrows — passes if thrown type IS-A expected type (subclasses accepted)
ArithmeticException ex = assertThrows(
    ArithmeticException.class,
    () -> calculator.divide(1, 0)
);
assertEquals("/ by zero", ex.getMessage());

// ✅ assertThrowsExactly — passes ONLY if type matches EXACTLY (JUnit 5.8+)
assertThrowsExactly(IllegalArgumentException.class, () -> {
    throw new IllegalArgumentException("invalid");
});
```
> **最适合**:`assertThrows`用于一般层次测试；`assertThrowsExactly`，当精确实现类是API契约的一部分时。

# # 6。assertDoesNotThrow

当没有异常是正在测试的显式契约时使用。```java
// ✅ GOOD — captures and returns the result for further assertions
int result = assertDoesNotThrow(() -> service.calculate(data));
assertEquals(100, result);
```
> **最适合**：显式地记录特定的边缘情况不应该触发错误。

# # 7。性能和截止日期- assertTimeout

使用`assertTimeout`确保在限制范围内完成执行。仅在需要硬流产时使用`assertTimeoutPreemptively`。```java
// ✅ assertTimeout — waits for completion, then checks duration
assertTimeout(Duration.ofSeconds(1), () -> service.heavyTask());

// ⚠️ assertTimeoutPreemptively — hard-aborts at deadline (Separate thread)
// Warning: ThreadLocal state (@Transactional) does NOT propagate.
assertTimeoutPreemptively(Duration.ofMillis(500), () -> service.fastTask());
```
b> **最适合**:SLA验证和防止CI/CD管道中的挂起测试。

# # 8。类型安全- assertInstanceOf

选择`assertInstanceOf`（JUnit 5.8+）而不是`assertTrue`+`instanceof`来获得自动转换。```java
// ❌ BAD — requires manual cast after assertion
assertTrue(result instanceof SuccessResponse);

// ✅ GOOD — returns the casted object
SuccessResponse resp = assertInstanceOf(SuccessResponse.class, result);
assertEquals(200, resp.statusCode());
```
**最适合**：测试多态结果和减少样板铸造。

# # 9。集合和数组

使用专用断言进行深入比较和信息差异。```java
// ✅ assertIterableEquals — element-by-element deep diff on failure
assertIterableEquals(expectedList, actualList);

// ✅ assertArrayEquals — deep comparison for arrays
assertArrayEquals(expectedArray, actualArray);
```
> **最适合**：验证列表顺序和复杂的数据结构内容。

# # 10。反模式

- **错误使用`assertTrue`表示相等：**不要使用`assertTrue(result == 42)`。使用`assertEquals(42, result)`查看日志中的两个值。
- **用`assertNotNull`代替实际检查：**如果可以检查值，不要只检查null。`assertEquals(expected, result)`总是比`assertNotNull(result)`好。
- **抑制失败：**永远不要捕捉`AssertionError`来隐藏失败。
- **遗留导入：**不要将`org.junit.Assert`（JUnit 4）与JUnit 5测试混合。