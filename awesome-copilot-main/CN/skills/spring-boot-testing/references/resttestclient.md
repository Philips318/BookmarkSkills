# RestTestClient

现代REST客户端测试与Spring Boot 4+（取代TestRestTemplate）。

# #概述

在Spring Boot 4.0+中，RestTestClient是teststtemplate的现代替代品。它为测试REST端点提供了流畅的反应性API。

# #设置

依赖（Spring Boot 4+）```xml
<dependency>
  <groupId>org.springframework.boot</groupId>
  <artifactId>spring-boot-starter-restclient-test</artifactId>
  <scope>test</scope>
</dependency>
```
###基本配置```java
@SpringBootTest(webEnvironment = WebEnvironment.RANDOM_PORT)
@AutoConfigureRestTestClient
class OrderIntegrationTest {
  
  @Autowired
  private RestTestClient restClient;
}
```
## HTTP方法

### GET请求```java
@Test
void shouldGetOrder() {
  restClient
    .get()
    .uri("/orders/1")
    .exchange()
    .expectStatus()
    .isOk()
    .expectBody(Order.class)
    .value(order -> {
      assertThat(order.getId()).isEqualTo(1L);
      assertThat(order.getStatus()).isEqualTo("PENDING");
    });
}
```
### POST请求```java
@Test
void shouldCreateOrder() {
  OrderRequest request = new OrderRequest("Laptop", 2);
  
  restClient
    .post()
    .uri("/orders")
    .contentType(MediaType.APPLICATION_JSON)
    .body(request)
    .exchange()
    .expectStatus()
    .isCreated()
    .expectHeader()
    .location("/orders/1")
    .expectBody(Long.class)
    .isEqualTo(1L);
}
```
### PUT请求```java
@Test
void shouldUpdateOrder() {
  restClient
    .put()
    .uri("/orders/1")
    .body(new OrderUpdate("COMPLETED"))
    .exchange()
    .expectStatus()
    .isOk();
}
```
###删除请求```java
@Test
void shouldDeleteOrder() {
  restClient
    .delete()
    .uri("/orders/1")
    .exchange()
    .expectStatus()
    .isNoContent();
}
```
##响应断言

###状态码```java
restClient
  .get()
  .uri("/orders/1")
  .exchange()
  .expectStatus()
  .isOk()           // 200
  .isCreated()      // 201
  .isNoContent()    // 204
  .isBadRequest()   // 400
  .isNotFound()     // 404
  .is5xxServerError() // 5xx
  .isEqualTo(200);  // Specific code
```
# # #标题```java
restClient
  .post()
  .uri("/orders")
  .exchange()
  .expectHeader()
  .location("/orders/1")
  .contentType(MediaType.APPLICATION_JSON)
  .exists("X-Request-Id")
  .valueEquals("X-Api-Version", "v1");
```
###主体断言```java
restClient
  .get()
  .uri("/orders/1")
  .exchange()
  .expectBody(Order.class)
  .value(order -> assertThat(order.getId()).isEqualTo(1L))
  .returnResult();
```
JSON路径```java
restClient
  .get()
  .uri("/orders")
  .exchange()
  .expectBody()
  .jsonPath("$.content[0].id").isEqualTo(1)
  .jsonPath("$.content[0].status").isEqualTo("PENDING")
  .jsonPath("$.totalElements").isNumber();
```
##请求配置

# # #标题```java
restClient
  .get()
  .uri("/orders/1")
  .header("Authorization", "Bearer token")
  .header("X-Api-Key", "secret")
  .exchange();
```
查询参数```java
restClient
  .get()
  .uri(uriBuilder -> uriBuilder
    .path("/orders")
    .queryParam("status", "PENDING")
    .queryParam("page", 0)
    .queryParam("size", 10)
    .build())
  .exchange();
```
###路径变量```java
restClient
  .get()
  .uri("/orders/{id}", 1L)
  .exchange();
```
##使用MockMvc

RestTestClient也可以与MockMvc一起工作（没有服务器启动）：```java
@SpringBootTest
@AutoConfigureMockMvc
@AutoConfigureRestTestClient
class OrderMockMvcTest {
  
  @Autowired
  private RestTestClient restClient;
  
  @Test
  void shouldWorkWithMockMvc() {
    // Uses MockMvc under the hood - no server startup
    restClient
      .get()
      .uri("/orders/1")
      .exchange()
      .expectStatus()
      .isOk();
  }
}
```
比较：RestTestClient和TestRestTemplate

|特性| RestTestClient | TestRestTemplate || ------- | -------------- | ---------------- |
|风格|Fluent/reactive|命令式|
| Spring Boot | 4.0+ |所有版本（在4中已弃用）|
|断言|内置|手动|
| MockMvc支持|是|否|
|异步|本地|需要额外处理|

从TestRestTemplate迁移

以前（已弃用）```java
@Autowired
private TestRestTemplate restTemplate;

@Test
void shouldGetOrder() {
  ResponseEntity<Order> response = restTemplate
    .getForEntity("/orders/1", Order.class);
  
  assertThat(response.getStatusCode()).isEqualTo(HttpStatus.OK);
  assertThat(response.getBody().getId()).isEqualTo(1L);
}
```
### After （RestTestClient）```java
@Autowired
private RestTestClient restClient;

@Test
void shouldGetOrder() {
  restClient
    .get()
    .uri("/orders/1")
    .exchange()
    .expectStatus()
    .isOk()
    .expectBody(Order.class)
    .value(order -> assertThat(order.getId()).isEqualTo(1L));
}
```
最佳实践

1. 使用@SpringBootTest（WebEnvironment）。RANDOM_PORT)用于真正的HTTP
2. 与@AutoConfigureMockMvc一起使用，可以在没有服务器的情况下进行更快的测试
3. 利用流畅的断言来提高可读性
4. 测试成功和错误场景
5. 验证security/API版本的标头