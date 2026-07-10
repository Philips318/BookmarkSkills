# MockMvc经典

用于Spring MVC控制器测试的传统MockMvc API （pre-Spring Boot 3.2或遗留代码库）。

何时使用此引用

-项目使用Spring Boot < 3.2（无`MockMvcTester`可用）
—现有的测试使用`mvc.perform(...)`，您正在维护或扩展它们
-需要将经典的MockMvc测试迁移到`MockMvcTester`（参见下面的迁移部分）
-用户明确询问`ResultActions`、`andExpect()`或hamcrest风格的web断言

对于Spring Boot 3.2+上的新测试，请选择[mockmvc-tester.md]（mockmvc-tester.md）。

# #设置```java
@WebMvcTest(OrderController.class)
class OrderControllerTest {

  @Autowired
  private MockMvc mvc;

  @MockBean
  private OrderService orderService;
}
```
基本GET请求```java
@Test
void shouldReturnOrder() throws Exception {
  given(orderService.findById(1L)).willReturn(new Order(1L, "PENDING", 99.99));

  mvc.perform(get("/orders/1"))
    .andExpect(status().isOk())
    .andExpect(content().contentType(MediaType.APPLICATION_JSON))
    .andExpect(jsonPath("$.id").value(1))
    .andExpect(jsonPath("$.status").value("PENDING"))
    .andExpect(jsonPath("$.totalToPay").value(99.99));
}
```
## POST请求正文```java
@Test
void shouldCreateOrder() throws Exception {
  given(orderService.create(any(OrderRequest.class))).willReturn(1L);

  mvc.perform(post("/orders")
      .contentType(MediaType.APPLICATION_JSON)
      .content("{\"product\": \"Laptop\", \"quantity\": 2}"))
    .andExpect(status().isCreated())
    .andExpect(header().string("Location", "/orders/1"));
}
```
## PUT请求```java
@Test
void shouldUpdateOrder() throws Exception {
  mvc.perform(put("/orders/1")
      .contentType(MediaType.APPLICATION_JSON)
      .content("{\"status\": \"COMPLETED\"}"))
    .andExpect(status().isOk());
}
```
##删除请求```java
@Test
void shouldDeleteOrder() throws Exception {
  mvc.perform(delete("/orders/1"))
    .andExpect(status().isNoContent());
}
```
##状态匹配器```java
.andExpect(status().isOk())           // 200
.andExpect(status().isCreated())      // 201
.andExpect(status().isNoContent())    // 204
.andExpect(status().isBadRequest())   // 400
.andExpect(status().isUnauthorized()) // 401
.andExpect(status().isForbidden())    // 403
.andExpect(status().isNotFound())     // 404
.andExpect(status().is(422))          // arbitrary code
```
JSON路径断言```java
// Exact value
.andExpect(jsonPath("$.status").value("PENDING"))

// Existence
.andExpect(jsonPath("$.id").exists())
.andExpect(jsonPath("$.deletedAt").doesNotExist())

// Array size
.andExpect(jsonPath("$.items").isArray())
.andExpect(jsonPath("$.items", hasSize(3)))

// Nested field
.andExpect(jsonPath("$.customer.name").value("John Doe"))
.andExpect(jsonPath("$.customer.address.city").value("Berlin"))

// With Hamcrest matchers
.andExpect(jsonPath("$.total", greaterThan(0.0)))
.andExpect(jsonPath("$.description", containsString("order")))
```
##内容断言```java
.andExpect(content().contentType(MediaType.APPLICATION_JSON))
.andExpect(content().contentTypeCompatibleWith(MediaType.APPLICATION_JSON))
.andExpect(content().string(containsString("PENDING")))
.andExpect(content().json("{\"status\":\"PENDING\"}"))
```
##头断言```java
.andExpect(header().string("Location", "/orders/1"))
.andExpect(header().string("Content-Type", containsString("application/json")))
.andExpect(header().exists("X-Request-Id"))
.andExpect(header().doesNotExist("X-Deprecated"))
```
##请求参数和报头```java
// Query parameters
mvc.perform(get("/orders").param("status", "PENDING").param("page", "0"))
  .andExpect(status().isOk());

// Path variables
mvc.perform(get("/orders/{id}", 1L))
  .andExpect(status().isOk());

// Request headers
mvc.perform(get("/orders/1").header("X-Api-Key", "secret"))
  .andExpect(status().isOk());
```
捕获响应```java
@Test
void shouldReturnCreatedId() throws Exception {
  given(orderService.create(any())).willReturn(42L);

  MvcResult result = mvc.perform(post("/orders")
      .contentType(MediaType.APPLICATION_JSON)
      .content("{\"product\": \"Laptop\", \"quantity\": 1}"))
    .andExpect(status().isCreated())
    .andReturn();

  String location = result.getResponse().getHeader("Location");
  assertThat(location).isEqualTo("/orders/42");
}
```
##与andDo链接```java
mvc.perform(get("/orders/1"))
  .andDo(print())              // prints request/response to console (debug)
  .andExpect(status().isOk());
```
##静态导入```java
import org.springframework.boot.test.mock.mockito.MockBean;
import static org.springframework.test.web.servlet.request.MockMvcRequestBuilders.*;
import static org.springframework.test.web.servlet.result.MockMvcResultMatchers.*;
import static org.springframework.test.web.servlet.result.MockMvcResultHandlers.*;
import static org.hamcrest.Matchers.*;
```
迁移到MockMvcTester

|经典MockMvc | MockMvcTester（推荐）|| --- | --- |
|`@Autowired MockMvc mvc`|`@Autowired MockMvcTester mvc`|
|`mvc.perform(get("/orders/1"))`|`mvc.get().uri("/orders/1")`|
|`.andExpect(status().isOk())`|`.hasStatusOk()`|
|`.andExpect(jsonPath("$.status").value("X"))`|`.bodyJson().convertTo(T.class)`+ AssertJ |
|`throws Exception`每个方法|没有检查异常|
| Hamcrest匹配器| AssertJ流畅断言|

参见[mockmvc-tester.md]（mockmvc-tester.md）获得完整的现代API。

##要点

1. **每个测试方法必须声明`throws Exception`** -`perform()`抛出检查异常
2. **在调试期间使用`andDo(print())`** -在提交之前删除
3. **首选`jsonPath()`而不是`content().string()`** -更精确的字段级断言
4. **需要静态导入** - IDE可以自动添加它们
5. **升级到Spring Boot 3.2+时迁移到MockMvcTester**以获得更好的可读性