# Spring Boot 4.0迁移

从Spring Boot 3迁移时的关键测试更改。X到4.0。

依赖性改变

模块化测试启动器

Spring Boot 4.0引入模块化测试启动器：

(3. x)前* *:* *```xml
<dependency>
  <groupId>org.springframework.boot</groupId>
  <artifactId>spring-boot-starter-test</artifactId>
  <scope>test</scope>
</dependency>
```
**(4.0)后- WebMvc测试：**```xml
<dependency>
  <groupId>org.springframework.boot</groupId>
  <artifactId>spring-boot-starter-webmvc-test</artifactId>
  <scope>test</scope>
</dependency>
```
**后(4.0)- REST客户端测试：**```xml
<dependency>
  <groupId>org.springframework.boot</groupId>
  <artifactId>spring-boot-starter-restclient-test</artifactId>
  <scope>test</scope>
</dependency>
```
##注释迁移

### @MockBean→@MockitoBean

* *弃用(3. x): * *```java
@MockBean
private OrderService orderService;
```
新(4.0):* * * *```java
@MockitoBean
private OrderService orderService;
```
### @SpyBean→@MockitoSpyBean

* *弃用(3. x): * *```java
@SpyBean
private PaymentGatewayClient paymentClient;
```
新(4.0):* * * *```java
@MockitoSpyBean
private PaymentGatewayClient paymentClient;
```
新的测试功能

# # # RestTestClient

取代TestRestTemplate（已弃用）：```java
@SpringBootTest(webEnvironment = WebEnvironment.RANDOM_PORT)
@AutoConfigureRestTestClient
class OrderIntegrationTest {
  
  @Autowired
  private RestTestClient restClient;
  
  @Test
  void shouldCreateOrder() {
    restClient
      .post()
      .uri("/orders")
      .body(new OrderRequest("Product", 2))
      .exchange()
      .expectStatus()
      .isCreated()
      .expectHeader()
      .location("/orders/1");
  }
}
```
支持JUnit 6

Spring Boot 4.0默认使用JUnit 6：

JUnit 4已弃用（暂时使用JUnit Vintage）
-所有JUnit 5的功能仍然工作
-删除JUnit 4依赖项以进行干净迁移

Testcontainers 2.0

模块命名更改：

(1. x)前* *:* *```xml
<artifactId>postgresql</artifactId>
```
(2.0)后* *:* *```xml
<artifactId>testcontainers-postgresql</artifactId>
```
非单例Bean嘲弄

Spring Framework 7允许模拟原型作用域的bean：```java
@Component
@Scope("prototype")
public class OrderProcessor { }

@SpringBootTest
class OrderServiceTest {
  @MockitoBean
  private OrderProcessor orderProcessor; // Now works!
}
```
## SpringExtension上下文改变

扩展上下文现在默认为测试方法范围。

如果使用@Nested类测试失败：```java
@SpringExtensionConfig(useTestClassScopedExtensionContext = true)
@SpringBootTest
class OrderTest {
  // Use old behavior
}
```
##迁移清单

-[]将@MockBean替换为@MockitoBean
-将@SpyBean替换为@MockitoSpyBean
-[]更新Testcontainers依赖到2.0命名
-[]根据需要添加模块化测试启动器
-[]迁移TestRestTemplate到RestTestClient
-[]移除JUnit 4的依赖
-[]更新自定义TestExecutionListener实现
-[]测试@嵌套类行为

##向后兼容

使用“经典”启动器进行逐步迁移：```xml
<dependency>
  <groupId>org.springframework.boot</groupId>
  <artifactId>spring-boot-starter-test-classic</artifactId>
  <scope>test</scope>
</dependency>
```
这在增量迁移时提供了旧的行为。