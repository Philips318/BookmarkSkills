# @MockitoBean

在Spring Boot测试中模拟依赖（取代Spring Boot 4+中已弃用的@MockBean）。

# #概述`@MockitoBean`取代Spring Boot 4.0+中已弃用的`@MockBean`注释。它创建一个Mockito模拟并将其注册到Spring上下文中，替换任何相同类型的现有bean。

##基本用法```java
@WebMvcTest(OrderController.class)
class OrderControllerTest {
  
  @MockitoBean
  private OrderService orderService;
  
  @MockitoBean
  private UserService userService;
}
```
支持测试片

-`@WebMvcTest`-模拟service/repository依赖项
-`@WebFluxTest`-模拟响应式服务依赖
-`@SpringBootTest`-用模拟代替真正的bean

## stub方法

基本存根```java
@Test
void shouldReturnOrder() {
  Order order = new Order(1L, "PENDING");
  given(orderService.findById(1L)).willReturn(order);
  
  // Test code
}
```
多重回报```java
given(orderService.findById(anyLong()))
  .willReturn(new Order(1L, "PENDING"))
  .willReturn(new Order(2L, "COMPLETED"));
```
抛出异常```java
given(orderService.findById(999L))
  .willThrow(new OrderNotFoundException(999L));
```
参数匹配```java
given(orderService.create(argThat(req -> req.getQuantity() > 0)))
  .willReturn(1L);

given(orderService.findByStatus(eq("PENDING")))
  .willReturn(List.of(new Order()));
```
验证交互

验证被调用的方法```java
verify(orderService).findById(1L);
```
###验证从未调用```java
verify(orderService, never()).delete(any());
```
###验证计数```java
verify(orderService, times(2)).findById(anyLong());
verify(orderService, atLeastOnce()).findByStatus(anyString());
```
###验证订单```java
InOrder inOrder = inOrder(orderService, userService);
inOrder.verify(orderService).findById(1L);
inOrder.verify(userService).getUser(any());
```
##重置模拟

mock在测试之间自动重置。在测试中重置：```java
Mockito.reset(orderService);
```
## @MockitoSpyBean部分模拟

使用`@MockitoSpyBean`用Mockito包装一个真正的bean。```java
@SpringBootTest
class OrderServiceIntegrationTest {
  
  @MockitoSpyBean
  private PaymentGatewayClient paymentClient;
  
  @Test
  void shouldProcessOrder() {
    doReturn(true).when(paymentClient).processPayment(any());
    
    // Test with real service but mocked payment client
  }
}
```
@TestBean用于自定义测试bean

在测试上下文中注册一个定制bean实例：```java
@SpringBootTest
class OrderServiceTest {
  
  @TestBean
  private PaymentGatewayClient paymentClient() {
    return new FakePaymentClient();
  }
}
```
作用域：单例vs原型

Spring Framework 7+ (Spring Boot 4+)支持模拟非单例bean：```java
@Component
@Scope("prototype")
public class OrderProcessor {
  public String process() { return "real"; }
}

@SpringBootTest
class OrderServiceTest {
  @MockitoBean
  private OrderProcessor orderProcessor;
  
  @Test
  void shouldWorkWithPrototype() {
    given(orderProcessor.process()).willReturn("mocked");
    // Test code
  }
}
```
##常见模式

服务测试中的mock存储库```java
@SpringBootTest
class OrderServiceTest {
  @MockitoBean
  private OrderRepository orderRepository;
  
  @Autowired
  private OrderService orderService;
  
  @Test
  void shouldCreateOrder() {
    given(orderRepository.save(any())).willReturn(new Order(1L));
    
    Long id = orderService.createOrder(new OrderRequest());
    
    assertThat(id).isEqualTo(1L);
    verify(orderRepository).save(any(Order.class));
  }
}
```
相同类型的多个mock

使用bean名称：```java
@MockitoBean(name = "primaryDataSource")
private DataSource primaryDataSource;

@MockitoBean(name = "secondaryDataSource")
private DataSource secondaryDataSource;
```
##从@MockBean迁移

以前（已弃用）```java
@MockBean
private OrderService orderService;
```
### After （Spring Boot 4+）```java
@MockitoBean
private OrderService orderService;
```
与Mockito的关键区别

|功能| @MockitoBean | @Mock || ------- | ------------ | ----- |
|上下文集成|是|否|
| Spring生命周期|参与|无|
|与@Autowired |是|否|
|测试片支持|是|有限的|

最佳实践

1. 仅在涉及Spring上下文时使用`@MockitoBean`2. 对于纯单元测试，使用Mockito的`@Mock`或`Mockito.mock()`3. 总是验证有副作用的交互
4. 不要验证简单的查询（存根就足够了）
5. 如果test修改了共享模拟状态，则重置模拟