# MockMvcTester

assertj风格的Spring MVC控制器测试（Spring Boot 3.2+）。

# #概述

MockMvcTester为web层测试提供了流畅的assertj风格的断言。比传统的MockMvc更具可读性和类型安全性。

**推荐模式**：将JSON转换为真实对象并使用AssertJ断言：```java
assertThat(mvc.get().uri("/orders/1"))
  .hasStatus(HttpStatus.OK)
  .bodyJson()
  .convertTo(OrderResponse.class)
  .satisfies(response -> {
    assertThat(response.getTotalToPay()).isEqualTo(expectedAmount);
    assertThat(response.getItems()).isNotEmpty();
  });
```
##基本用法```java
@WebMvcTest(OrderController.class)
class OrderControllerTest {
  
  @Autowired
  private MockMvcTester mvc;
  
  @MockitoBean
  private OrderService orderService;
}
```
建议：对象转换模式

单对象响应```java
@Test
void shouldGetOrder() {
  given(orderService.findById(1L)).willReturn(new Order(1L, "PENDING", 99.99));
  
  assertThat(mvc.get().uri("/orders/1"))
    .hasStatus(HttpStatus.OK)
    .bodyJson()
    .convertTo(OrderResponse.class)
    .satisfies(response -> {
      assertThat(response.getId()).isEqualTo(1L);
      assertThat(response.getStatus()).isEqualTo("PENDING");
      assertThat(response.getTotalToPay()).isEqualTo(new BigDecimal("99.99"));
    });
}
```
###列表响应```java
@Test
void shouldGetAllOrders() {
  given(orderService.findAll()).willReturn(Arrays.asList(
    new Order(1L, "PENDING"),
    new Order(2L, "COMPLETED")
  ));
  
  assertThat(mvc.get().uri("/orders"))
    .hasStatus(HttpStatus.OK)
    .bodyJson()
    .convertTo(new TypeReference<List<OrderResponse>>() {})
    .satisfies(orders -> {
      assertThat(orders).hasSize(2);
      assertThat(orders.get(0).getStatus()).isEqualTo("PENDING");
      assertThat(orders.get(1).getStatus()).isEqualTo("COMPLETED");
    });
}
```
嵌套对象```java
@Test
void shouldGetOrderWithCustomer() {
  assertThat(mvc.get().uri("/orders/1"))
    .hasStatus(HttpStatus.OK)
    .bodyJson()
    .convertTo(OrderResponse.class)
    .satisfies(response -> {
      assertThat(response.getCustomer()).isNotNull();
      assertThat(response.getCustomer().getName()).isEqualTo("John Doe");
      assertThat(response.getCustomer().getAddress().getCity()).isEqualTo("Berlin");
    });
}
```
复杂的断言```java
@Test
void shouldCalculateOrderTotal() {
  assertThat(mvc.get().uri("/orders/1/calculate"))
    .hasStatus(HttpStatus.OK)
    .bodyJson()
    .convertTo(CalculationResponse.class)
    .satisfies(calc -> {
      assertThat(calc.getSubtotal()).isEqualTo(new BigDecimal("100.00"));
      assertThat(calc.getTax()).isEqualTo(new BigDecimal("19.00"));
      assertThat(calc.getTotalToPay()).isEqualTo(new BigDecimal("119.00"));
      assertThat(calc.getItems()).allMatch(item -> item.getPrice().compareTo(BigDecimal.ZERO) > 0);
    });
}
```
## HTTP方法

### POST请求体```java
@Test
void shouldCreateOrder() {
  given(orderService.create(any())).willReturn(1L);
  
  assertThat(mvc.post().uri("/orders")
    .contentType(MediaType.APPLICATION_JSON)
    .content("{\"product\": \"Laptop\", \"quantity\": 2}"))
    .hasStatus(HttpStatus.CREATED)
    .hasHeader("Location", "/orders/1");
}
```
### PUT请求```java
@Test
void shouldUpdateOrder() {
  assertThat(mvc.put().uri("/orders/1")
    .contentType(MediaType.APPLICATION_JSON)
    .content("{\"status\": \"COMPLETED\"}"))
    .hasStatus(HttpStatus.OK);
}
```
###删除请求```java
@Test
void shouldDeleteOrder() {
  assertThat(mvc.delete().uri("/orders/1"))
    .hasStatus(HttpStatus.NO_CONTENT);
}
```
## Status断言```java
assertThat(mvc.get().uri("/orders/1"))
  .hasStatusOk()                    // 200
  .hasStatus(HttpStatus.OK)         // 200
  .hasStatus2xxSuccessful()         // 2xx
  .hasStatusBadRequest()            // 400
  .hasStatusNotFound()              // 404
  .hasStatusUnauthorized()          // 401
  .hasStatusForbidden()             // 403
  .hasStatus(HttpStatus.CREATED);   // 201
```
内容类型断言```java
assertThat(mvc.get().uri("/orders/1"))
  .hasContentType(MediaType.APPLICATION_JSON)
  .hasContentTypeCompatibleWith(MediaType.APPLICATION_JSON);
```
##头断言```java
assertThat(mvc.post().uri("/orders"))
  .hasHeader("Location", "/orders/123")
  .hasHeader("X-Request-Id", matchesPattern("[a-z0-9-]+"));
```
选择：JSON路径（谨慎使用）

仅在无法转换为类型化对象时使用：```java
assertThat(mvc.get().uri("/orders/1"))
  .hasStatusOk()
  .bodyJson()
  .extractingPath("$.customer.address.city")
  .asString()
  .isEqualTo("Berlin");
```
##请求参数```java
// Query parameters
assertThat(mvc.get().uri("/orders?status=PENDING&page=0"))
  .hasStatusOk();

// Path parameters
assertThat(mvc.get().uri("/orders/{id}", 1L))
  .hasStatusOk();

// Headers
assertThat(mvc.get().uri("/orders/1")
  .header("X-Api-Key", "secret"))
  .hasStatusOk();
```
##使用JacksonTester请求Body```java
@Autowired
private JacksonTester<OrderRequest> json;

@Test
void shouldCreateOrder() {
  OrderRequest request = new OrderRequest("Laptop", 2);
  
  assertThat(mvc.post().uri("/orders")
    .contentType(MediaType.APPLICATION_JSON)
    .content(json.write(request).getJson()))
    .hasStatus(HttpStatus.CREATED);
}
```
##错误响应```java
@Test
void shouldReturnValidationErrors() {
  given(orderService.findById(999L))
    .willThrow(new OrderNotFoundException(999L));
  
  assertThat(mvc.get().uri("/orders/999"))
    .hasStatus(HttpStatus.NOT_FOUND)
    .bodyJson()
    .convertTo(ErrorResponse.class)
    .satisfies(error -> {
      assertThat(error.getMessage()).isEqualTo("Order 999 not found");
      assertThat(error.getCode()).isEqualTo("ORDER_NOT_FOUND");
    });
}
```
验证错误测试```java
@Test
void shouldRejectInvalidOrder() {
  OrderRequest invalidRequest = new OrderRequest("", -1);
  
  assertThat(mvc.post().uri("/orders")
    .contentType(MediaType.APPLICATION_JSON)
    .content(json.write(invalidRequest).getJson()))
    .hasStatus(HttpStatus.BAD_REQUEST)
    .bodyJson()
    .convertTo(ValidationErrorResponse.class)
    .satisfies(errors -> {
      assertThat(errors.getFieldErrors()).hasSize(2);
      assertThat(errors.getFieldErrors())
        .extracting("field")
        .contains("product", "quantity");
    });
}
```
比较：MockMvcTester与经典的MockMvc

|功能| MockMvcTester |经典MockMvc || ------- | ------------- | --------------- |
|风格| AssertJ流利| MockMvc匹配器|
|可读性|高|中|
|类型安全|更好|更少|
| IDE支持|优秀|良好|
|对象转换类型|本地|手动|

从经典的MockMvc迁移

###之前（经典）```java
mvc.perform(get("/orders/1"))
  .andExpect(status().isOk())
  .andExpect(jsonPath("$.status").value("PENDING"))
  .andExpect(jsonPath("$.totalToPay").value(99.99));
```
（带有对象转换的测试器）```java
assertThat(mvc.get().uri("/orders/1"))
  .hasStatus(HttpStatus.OK)
  .bodyJson()
  .convertTo(OrderResponse.class)
  .satisfies(response -> {
    assertThat(response.getStatus()).isEqualTo("PENDING");
    assertThat(response.getTotalToPay()).isEqualTo(new BigDecimal("99.99"));
  });
```
##要点

1. **首选`convertTo()`而不是`extractingPath()`** -类型安全，可重构
2. **对多个断言使用`satisfies()`** -保持测试的可读性
3. **导入静态`org.assertj.core.api.Assertions.assertThat`**
4. **通过`TypeReference`泛型工作** -用于`List<T>`响应
5. **IDE重构友好** -重命名字段，IDE更新测试