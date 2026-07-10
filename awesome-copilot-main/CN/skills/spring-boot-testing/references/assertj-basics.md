# AssertJ Basics

流畅的断言，可读，可维护的测试。

##基本断言

对象相等```java
assertThat(order.getStatus()).isEqualTo("PENDING");
assertThat(order.getId()).isNotEqualTo(0);
assertThat(order).isEqualTo(expectedOrder);
assertThat(order).isNotNull();
assertThat(nullOrder).isNull();
```
字符串断言```java
assertThat(order.getDescription())
  .isEqualTo("Test Order")
  .startsWith("Test")
  .endsWith("Order")
  .contains("Test")
  .hasSize(10)
  .matches("[A-Za-z ]+");
```
###数量断言```java
assertThat(order.getAmount())
  .isEqualTo(99.99)
  .isGreaterThan(50)
  .isLessThan(100)
  .isBetween(50, 100)
  .isPositive()
  .isNotZero();
```
布尔断言```java
assertThat(order.isActive()).isTrue();
assertThat(order.isDeleted()).isFalse();
```
Date/Time断言```java
assertThat(order.getCreatedAt())
  .isEqualTo(LocalDateTime.of(2024, 1, 15, 10, 30))
  .isBefore(LocalDateTime.now())
  .isAfter(LocalDateTime.of(2024, 1, 1))
  .isCloseTo(LocalDateTime.now(), within(5, ChronoUnit.SECONDS));
```
##可选断言```java
Optional<Order> maybeOrder = orderService.findById(1L);

assertThat(maybeOrder)
  .isPresent()
  .hasValueSatisfying(order -> {
    assertThat(order.getId()).isEqualTo(1L);
  });

assertThat(orderService.findById(999L)).isEmpty();
```
##异常断言

JUnit 5异常处理```java
@Test
void shouldThrowException() {
  OrderService service = new OrderService();
  
  assertThatThrownBy(() -> service.findById(999L))
    .isInstanceOf(OrderNotFoundException.class)
    .hasMessage("Order 999 not found")
    .hasMessageContaining("999");
}
```
### AssertJ异常处理```java
@Test
void shouldThrowExceptionWithCause() {
  assertThatExceptionOfType(OrderProcessingException.class)
    .isThrownBy(() -> service.processOrder(invalidOrder))
    .withCauseInstanceOf(ValidationException.class);
}
```
自定义断言

为可重用的测试代码创建特定于领域的断言：```java
public class OrderAssert extends AbstractAssert<OrderAssert, Order> {
  
  public static OrderAssert assertThat(Order actual) {
    return new OrderAssert(actual);
  }
  
  private OrderAssert(Order actual) {
    super(actual, OrderAssert.class);
  }
  
  public OrderAssert isPending() {
    isNotNull();
    if (!"PENDING".equals(actual.getStatus())) {
      failWithMessage("Expected order status to be PENDING but was %s", actual.getStatus());
    }
    return this;
  }
  
  public OrderAssert hasTotal(BigDecimal expected) {
    isNotNull();
    if (!expected.equals(actual.getTotal())) {
      failWithMessage("Expected total %s but was %s", expected, actual.getTotal());
    }
    return this;
  }
}
```
用法:```java
OrderAssert.assertThat(order)
  .isPending()
  .hasTotal(new BigDecimal("99.99"));
```
##软断言

失败前收集多次失败：```java
@Test
void shouldValidateOrder() {
  Order order = orderService.findById(1L);
  
  SoftAssertions.assertSoftly(softly -> {
    softly.assertThat(order.getId()).isEqualTo(1L);
    softly.assertThat(order.getStatus()).isEqualTo("PENDING");
    softly.assertThat(order.getItems()).isNotEmpty();
  });
}
```
##满足模式```java
assertThat(order)
  .satisfies(o -> {
    assertThat(o.getId()).isPositive();
    assertThat(o.getStatus()).isNotBlank();
    assertThat(o.getCreatedAt()).isNotNull();
  });
```
##使用Spring```java
import static org.assertj.core.api.Assertions.assertThat;

@SpringBootTest
class OrderServiceTest {
  
  @Autowired
  private OrderService orderService;
  
  @Test
  void shouldCreateOrder() {
    Order order = orderService.create(new OrderRequest("Product", 2));
    
    assertThat(order)
      .isNotNull()
      .extracting(Order::getId, Order::getStatus)
      .containsExactly(1L, "PENDING");
  }
}
```
##静态导入

对于干净的断言总是使用静态导入：```java
import static org.assertj.core.api.Assertions.assertThat;
import static org.assertj.core.api.Assertions.assertThatThrownBy;
import static org.assertj.core.api.Assertions.catchThrowable;
```
##主要优势

1. **可读**：句子式结构
2. **类型安全**:IDE自动完成工作
3. **丰富的API**：许多内置断言
4. **可扩展**：为您的域自定义断言
5. **更好的错误**：清除失败消息