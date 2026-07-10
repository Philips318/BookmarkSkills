# @WebMvcTest

使用集中切片测试测试Spring MVC控制器。

##基本结构```java
@WebMvcTest(OrderController.class)
class OrderControllerTest {
  
  @Autowired
  private MockMvcTester mvc;
  
  @MockitoBean
  private OrderService orderService;
  
  @MockitoBean
  private UserService userService;
}
```
##什么得到加载

-指定的控制器
Spring MVC基础架构（HandlerMapping, HandlerAdapter）
- Jackson ObjectMapper（用于JSON）
-异常处理程序（@ControllerAdvice）
Spring安全过滤器（如果在类路径上）
-验证（如果在类路径上）

测试GET端点```java
@Test
void shouldReturnOrder() {
  var order = new Order(1L, "PENDING", BigDecimal.valueOf(99.99));
  given(orderService.findById(1L)).willReturn(order);
  
  assertThat(mvc.get().uri("/orders/1"))
    .hasStatusOk()
    .hasContentType(MediaType.APPLICATION_JSON)
    .bodyJson()
    .extractingPath("$.status")
    .isEqualTo("PENDING");
}
```
##测试POST请求正文

使用文本块（Java 25）```java
@Test
void shouldCreateOrder() {
  given(orderService.create(any(OrderRequest.class))).willReturn(1L);
  
  var json = """
    {
      "product": "Product A",
      "quantity": 2
    }
    """;
  
  assertThat(mvc.post().uri("/orders")
    .contentType(MediaType.APPLICATION_JSON)
    .content(json))
    .hasStatus(HttpStatus.CREATED)
    .hasHeader("Location", "/orders/1");
}
```
###使用记录```java
record OrderRequest(String product, int quantity) {}

@Test
void shouldCreateOrderWithRecord() {
  var request = new OrderRequest("Product A", 2);
  given(orderService.create(any())).willReturn(1L);
  
  assertThat(mvc.post().uri("/orders")
    .contentType(MediaType.APPLICATION_JSON)
    .content(json.write(request).getJson()))
    .hasStatus(HttpStatus.CREATED);
}
```
##测试验证错误```java
@Test
void shouldRejectInvalidOrder() {
  var invalidJson = """
    {
      "product": "",
      "quantity": -1
    }
    """;
  
  assertThat(mvc.post().uri("/orders")
    .contentType(MediaType.APPLICATION_JSON)
    .content(invalidJson))
    .hasStatus(HttpStatus.BAD_REQUEST)
    .bodyJson()
    .hasPath("$.errors");
}
```
##测试查询参数```java
@Test
void shouldFilterOrdersByStatus() {
  assertThat(mvc.get().uri("/orders?status=PENDING"))
    .hasStatusOk();
  
  verify(orderService).findByStatus(OrderStatus.PENDING);
}
```
##测试路径变量```java
@Test
void shouldCancelOrder() {
  assertThat(mvc.put().uri("/orders/123/cancel"))
    .hasStatusOk();
  
  verify(orderService).cancel(123L);
}
```
安全性测试```java
@Test
@WithMockUser(roles = "ADMIN")
void adminShouldDeleteOrder() {
  assertThat(mvc.delete().uri("/orders/1"))
    .hasStatus(HttpStatus.NO_CONTENT);
}

@Test
void anonymousUserShouldBeForbidden() {
  assertThat(mvc.delete().uri("/orders/1"))
    .hasStatus(HttpStatus.UNAUTHORIZED);
}
```
##多控制器```java
@WebMvcTest({OrderController.class, ProductController.class})
class WebLayerTest {
  // Tests multiple controllers in one slice
}
```
##排除自动配置```java
@WebMvcTest(OrderController.class)
@AutoConfigureMockMvc(addFilters = false) // Skip security filters
class OrderControllerWithoutSecurityTest {
  // Tests without security filters
}
```
##要点

1. 总是用@MockitoBean模拟服务
2. 对assertj风格的断言使用MockMvcTester
3. 测试HTTP语义（状态、标头、内容类型）
4. 当副作用很重要时，验证服务方法调用
5. 不要在这里测试业务逻辑——那是用于单元测试的
6. 为JSON有效负载利用Java 25文本块