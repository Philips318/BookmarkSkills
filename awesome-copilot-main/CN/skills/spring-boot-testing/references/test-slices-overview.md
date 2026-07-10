# Test Slices概述

选择正确的Spring Boot测试片的快速参考。

##决策矩阵

|注释|在|加载|时使用|| ---------- | -------- | ----- | ----- |
| **无**（纯JUnit） |测试纯业务逻辑|无|最快|
|`@WebMvcTest`|控制器+ HTTP层|控制器，MVC, Jackson | Fast |
|`@DataJpaTest`| Repository查询| Repositories， JPA, DataSource | Fast |
|`@RestClientTest`| REST客户端代码|RestTemplate/RestClient， Jackson | Fast |
|`@JsonTest`| JSON序列化| ObjectMapper只|最快切片|
|`@WebFluxTest`|响应式控制器|控制器，WebFlux |快速|
|`@DataJdbcTest`| JDBC存储库|存储库，JDBC | Fast |
|`@DataMongoTest`| MongoDB知识库|知识库，MongoDB | Fast |
|`@DataRedisTest`| Redis repositories | repositories, Redis | Fast |
|`@SpringBootTest`|完全集成|整个应用|慢|

##选择指南

###不使用注释（普通单元测试）```java
class PriceCalculatorTest {
  private PriceCalculator calculator = new PriceCalculator();
  
  @Test
  void shouldApplyDiscount() {
    var result = calculator.applyDiscount(100, 0.1);
    assertThat(result).isEqualTo(new BigDecimal("90.00"));
  }
}
```
**When**：纯业务逻辑，没有依赖关系或通过构造函数注入可模拟的简单依赖关系。

###使用@WebMvcTest```java
@WebMvcTest(OrderController.class)
class OrderControllerTest {
  @Autowired private MockMvcTester mvc;
  @MockitoBean private OrderService orderService;
}
```
**When**：测试请求映射、验证、JSON映射、安全性、过滤器。

**你得到的**:MockMvc， ObjectMapper, Spring Security（如果存在），异常处理程序。

###使用@DataJpaTest```java
@DataJpaTest
@AutoConfigureTestDatabase(replace = AutoConfigureTestDatabase.Replace.NONE)
@Testcontainers
class OrderRepositoryTest {
  @Container
  static PostgreSQLContainer<?> postgres = new PostgreSQLContainer<>("postgres:18");
}
```
**When**：测试自定义JPA查询，实体映射，事务行为，级联操作。

**你得到了什么**：仓库bean， EntityManager, TestEntityManager，事务支持。

###使用@RestClientTest```java
@RestClientTest(WeatherService.class)
class WeatherServiceTest {
  @Autowired private WeatherService weatherService;
  @Autowired private MockRestServiceServer server;
}
```
**当**：测试调用外部api的REST客户端。

**你得到什么**:MockRestServiceServer存根HTTP响应。

###使用@JsonTest```java
@JsonTest
class OrderJsonTest {
  @Autowired private JacksonTester<Order> json;
}
```
**当**：测试自定义serializers/deserializers，复杂的JSON映射。

###使用@SpringBootTest```java
@SpringBootTest(webEnvironment = WebEnvironment.RANDOM_PORT)
@AutoConfigureRestTestClient
class OrderIntegrationTest {
  @Autowired private RestTestClient restClient;
}
```
**当**：测试完整的请求流，安全过滤器，数据库交互。

**您得到的**：完整的应用程序上下文，嵌入式服务器（可选），真正的bean。

常见错误

1. **使用@SpringBootTest ** -不必要地减慢您的测试套件
2. **@WebMvcTest不带mock服务** -导致上下文加载失败
3. **带@MockBean的@DataJpaTest **——违背了目的（你想要真正的存储库）
4. **多个片在一个测试** -每个片是一个单独的测试类

Java 25在测试中的特性

测试数据记录```java
record OrderRequest(String product, int quantity) {}
record OrderResponse(Long id, String status, BigDecimal total) {}
```
测试中的模式匹配```java
@Test
void shouldHandleDifferentOrderTypes() {
  var order = orderService.create(new OrderRequest("Product", 2));
  
  switch (order) {
    case PhysicalOrder po -> assertThat(po.getShippingAddress()).isNotNull();
    case DigitalOrder do_ -> assertThat(do_.getDownloadLink()).isNotNull();
    default -> throw new IllegalStateException("Unknown order type");
  }
}
```
用于JSON的文本块```java
@Test
void shouldParseComplexJson() {
  var json = """
    {
      "id": 1,
      "status": "PENDING",
      "items": [
        {"product": "Laptop", "price": 999.99},
        {"product": "Mouse", "price": 29.99}
      ]
    }
    """;
  
  assertThat(mvc.post().uri("/orders")
    .contentType(APPLICATION_JSON)
    .content(json))
    .hasStatus(CREATED);
}
```
###序列集合```java
@Test
void shouldReturnOrdersInSequence() {
  var orders = orderRepository.findAll();
  
  assertThat(orders.getFirst().getStatus()).isEqualTo("NEW");
  assertThat(orders.getLast().getStatus()).isEqualTo("COMPLETED");
  assertThat(orders.reversed().getFirst().getStatus()).isEqualTo("COMPLETED");
}
```
##依赖于Slice```xml
<!-- WebMvcTest -->
<dependency>
  <groupId>org.springframework.boot</groupId>
  <artifactId>spring-boot-starter-webmvc-test</artifactId>
  <scope>test</scope>
</dependency>

<!-- DataJpaTest -->
<dependency>
  <groupId>org.springframework.boot</groupId>
  <artifactId>spring-boot-starter-data-jpa</artifactId>
</dependency>

<!-- RestClientTest -->
<dependency>
  <groupId>org.springframework.boot</groupId>
  <artifactId>spring-boot-starter-restclient-test</artifactId>
  <scope>test</scope>
</dependency>

<!-- Testcontainers -->
<dependency>
  <groupId>org.springframework.boot</groupId>
  <artifactId>spring-boot-testcontainers</artifactId>
  <scope>test</scope>
</dependency>
```
