# @RestClientTest

使用MockRestServiceServer单独测试REST客户端。

# #概述`@RestClientTest`自动配置:

-RestTemplate/RestClient与模拟服务器支持
——Jackson ObjectMapper
——MockRestServiceServer

##基本设置```java
@RestClientTest(WeatherService.class)
class WeatherServiceTest {
  
  @Autowired
  private WeatherService weatherService;
  
  @Autowired
  private MockRestServiceServer server;
}
```
##测试RestTemplate```java
@RestClientTest(WeatherService.class)
class WeatherServiceTest {
  
  @Autowired
  private WeatherService weatherService;
  
  @Autowired
  private MockRestServiceServer server;
  
  @Test
  void shouldFetchWeather() {
    // Given
    server.expect(requestTo("https://api.weather.com/v1/current"))
      .andExpect(method(HttpMethod.GET))
      .andExpect(queryParam("city", "Berlin"))
      .andRespond(withSuccess()
        .contentType(MediaType.APPLICATION_JSON)
        .body("{\"temperature\": 22, \"condition\": \"Sunny\"}"));
    
    // When
    Weather weather = weatherService.getCurrentWeather("Berlin");
    
    // Then
    assertThat(weather.getTemperature()).isEqualTo(22);
    assertThat(weather.getCondition()).isEqualTo("Sunny");
  }
}
```
测试RestClient （Spring 6.1+）```java
@RestClientTest(WeatherService.class)
class WeatherServiceTest {
  
  @Autowired
  private WeatherService weatherService;
  
  @Autowired
  private MockRestServiceServer server;
  
  @Test
  void shouldFetchWeatherWithRestClient() {
    server.expect(requestTo("https://api.weather.com/v1/current"))
      .andRespond(withSuccess()
        .body("{\"temperature\": 22}"));
    
    Weather weather = weatherService.getCurrentWeather("Berlin");
    
    assertThat(weather.getTemperature()).isEqualTo(22);
  }
}
```
##请求匹配

准确的URL```java
server.expect(requestTo("https://api.example.com/users/1"))
  .andRespond(withSuccess());
```
URL模式```java
server.expect(requestTo(matchesPattern("https://api.example.com/users/\\d+")))
  .andRespond(withSuccess());
```
HTTP方法```java
server.expect(ExpectedCount.once(), 
  requestTo("https://api.example.com/users"))
  .andExpect(method(HttpMethod.POST))
  .andRespond(withCreatedEntity(URI.create("/users/1")));
```
请求正文```java
server.expect(requestTo("https://api.example.com/users"))
  .andExpect(content().contentType(MediaType.APPLICATION_JSON))
  .andExpect(content().json("{\"name\": \"John\"}"))
  .andRespond(withSuccess());
```
# # #标题```java
server.expect(requestTo("https://api.example.com/users"))
  .andExpect(header("Authorization", "Bearer token123"))
  .andExpect(header("X-Api-Key", "secret"))
  .andRespond(withSuccess());
```
##响应类型

###成功与身体```java
server.expect(requestTo("/users/1"))
  .andRespond(withSuccess()
    .contentType(MediaType.APPLICATION_JSON)
    .body("{\"id\": 1, \"name\": \"John\"}"));
```
###从资源成功```java
server.expect(requestTo("/users/1"))
  .andRespond(withSuccess()
    .body(new ClassPathResource("user-response.json")));
```
# # #创建```java
server.expect(requestTo("/users"))
  .andExpect(method(HttpMethod.POST))
  .andRespond(withCreatedEntity(URI.create("/users/1")));
```
错误响应```java
server.expect(requestTo("/users/999"))
  .andRespond(withResourceNotFound());

server.expect(requestTo("/users"))
  .andRespond(withServerError()
    .body("Internal Server Error"));

server.expect(requestTo("/users"))
  .andRespond(withStatus(HttpStatus.BAD_REQUEST)
    .body("{\"error\": \"Invalid input\"}"));
```
##验证请求```java
@Test
void shouldCallApi() {
  server.expect(ExpectedCount.once(), 
    requestTo("https://api.example.com/data"))
    .andRespond(withSuccess());
  
  service.fetchData();
  
  server.verify(); // Verify all expectations met
}
```
忽略额外的请求```java
@Test
void shouldHandleMultipleCalls() {
  server.expect(ExpectedCount.manyTimes(),
    requestTo(matchesPattern("/api/.*")))
    .andRespond(withSuccess());
  
  // Multiple calls allowed
  service.callApi();
  service.callApi();
  service.callApi();
}
```
测试间重置```java
@BeforeEach
void setUp() {
  server.reset();
}
```
##测试超时```java
server.expect(requestTo("/slow-endpoint"))
  .andRespond(withSuccess()
    .body("{\"data\": \"test\"}")
    .delay(100, TimeUnit.MILLISECONDS));

// Test timeout handling
```
最佳实践

1. 始终在测试结束时验证`server.verify()`2. 对于大型JSON响应使用资源文件
3. 匹配最小请求属性集
4. 重置@BeforeEach中的服务器
5. 测试错误响应，而不仅仅是成功
6. 验证POST/PUT调用的请求体