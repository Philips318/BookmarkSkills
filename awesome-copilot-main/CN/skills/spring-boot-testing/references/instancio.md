# Instancio

自动生成复杂的测试对象。当entities/DTOs有3+属性时使用。

##何时使用

-具有**3个或更多属性的对象**
-为存储库设置测试数据
—为控制器测试创建dto
-避免重复的builder/setter调用

# #的依赖```xml
<dependency>
  <groupId>org.instancio</groupId>
  <artifactId>instancio-junit</artifactId>
  <version>5.5.1</version>
  <scope>test</scope>
</dependency>
```
##基本用法

简单对象```java
final var order = Instancio.create(Order.class);
// All fields populated with random data
```
对象列表```java
final var orders = Instancio.ofList(Order.class).size(5).create();
// 5 orders with random data
```
##自定义值

###设置特定字段```java
final var order = Instancio.of(Order.class)
  .set(field(Order::getStatus), "PENDING")
  .set(field(Order::getTotal), new BigDecimal("99.99"))
  .create();
```
###提供生成值```java
final var order = Instancio.of(Order.class)
  .supply(field(Order::getEmail), () -> "user" + UUID.randomUUID() + "@test.com")
  .create();
```
###忽略字段```java
final var order = Instancio.of(Order.class)
  .ignore(field(Order::getId)) // Let DB generate
  .create();
```
##复杂对象

嵌套对象```java
final var order = Instancio.of(Order.class)
  .set(field(Order::getCustomer), Instancio.create(Customer.class))
  .set(field(Order::getItems), Instancio.ofList(OrderItem.class).size(3).create())
  .create();
```
###所有字段随机```java
// When you need fully random but valid data
final var randomOrder = Instancio.create(Order.class);
// Customer, items, addresses - all populated
```
Spring Boot集成

存储库测试设置```java
@DataJpaTest
@AutoConfigureTestDatabase
@Testcontainers
class OrderRepositoryTest {
  
  @Container
  @ServiceConnection
  static PostgreSQLContainer<?> postgres = new PostgreSQLContainer<>("postgres:18");
  
  @Autowired
  private OrderRepository orderRepository;
  
  @Test
  void shouldFindOrdersByStatus() {
    // Given: Create 10 random orders with PENDING status
    final var orders = Instancio.ofList(Order.class)
      .size(10)
      .set(field(Order::getStatus), "PENDING")
      .create();
    
    orderRepository.saveAll(orders);
    
    // When
    final var found = orderRepository.findByStatus("PENDING");
    
    // Then
    assertThat(found).hasSize(10);
  }
}
```
控制器测试设置```java
@WebMvcTest(OrderController.class)
class OrderControllerTest {
  
  @Autowired
  private MockMvcTester mvc;
  
  @MockitoBean
  private OrderService orderService;
  
  @Test
  void shouldReturnOrder() {
    // Given: Random order with specific ID
    Order order = Instancio.of(Order.class)
      .set(field(Order::getId), 1L)
      .create();
    
    given(orderService.findById(1L)).willReturn(order);
    
    // When/Then
    assertThat(mvc.get().uri("/orders/1"))
      .hasStatus(HttpStatus.OK)
      .bodyJson()
      .convertTo(OrderResponse.class)
      .satisfies(response -> {
        assertThat(response.getId()).isEqualTo(1L);
      });
  }
}
```
# #模式

构建器模式替代```java
// Instead of:
Order order = Order.builder()
  .id(1L)
  .status("PENDING")
  .customer(Customer.builder().name("John").build())
  .items(List.of(
    OrderItem.builder().product("A").price(10).build(),
    OrderItem.builder().product("B").price(20).build()
  ))
  .build();

// Use:
Order order = Instancio.of(Order.class)
  .set(field(Order::getId), 1L)
  .set(field(Order::getStatus), "PENDING")
  .create();
// Customer and items auto-generated
```
种子数据```java
// Consistent "random" data for reproducible tests
Order order = Instancio.of(Order.class)
  .withSeed(12345L)
  .create();
// Same data every test run with seed 12345
```
##常见模式

### Email Generation```java
String email = Instancio.gen().net().email();
```
日期生成```java
LocalDateTime createdAt = Instancio.gen().temporal()
  .localDateTime()
  .past()
  .create();
```
字符串模式```java
String phone = Instancio.gen().text().pattern("+1-###-###-####");
```
# #比较

方法|代码行|可维护性|| -------- | ------------- | --------------- |
|手动设置| 10-20 |低|
|构建模式| 5-10 |中等|
| **实例** | 2-5 | **高** |

最佳实践

1. **用于3+属性对象** -不值得用于简单对象
2. **只设置相关的内容** -让实例填充其余内容
3. **与Testcontainers一起使用** -非常适合数据库播种
4. **显式设置id ** -测试特定场景时
5. **忽略自动生成的字段** -像createdAt， updatedAt

# #链接

- [Instancio文档]（https://www.instancio.org/）
- [JUnit 5扩展]（https://www.instancio.org/user-guide/#junit-integration）