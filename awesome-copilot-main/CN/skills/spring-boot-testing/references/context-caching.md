#上下文缓存

通过上下文缓存优化Spring Boot测试套件的性能。

上下文缓存是如何工作的

Spring的TestContext框架根据应用的配置“键”缓存应用的上下文。具有相同配置的测试重用相同的上下文。

什么影响缓存键

——@ContextConfiguration
——@TestPropertySource
——@ActiveProfiles
——@WebAppConfiguration
- @MockitoBean定义
—@TestConfiguration导入

##缓存关键字示例

相同的密钥（上下文重用）```java
@WebMvcTest(OrderController.class)
class OrderControllerTest1 {
  @MockitoBean private OrderService orderService;
}

@WebMvcTest(OrderController.class)
class OrderControllerTest2 {
  @MockitoBean private OrderService orderService;
}
// Same context reused
```
不同的键（新上下文）```java
@WebMvcTest(OrderController.class)
@ActiveProfiles("test")
class OrderControllerTest1 { }

@WebMvcTest(OrderController.class)
@ActiveProfiles("integration")
class OrderControllerTest2 { }
// Different contexts loaded
```
查看Cache统计信息

弹簧启动执行器```yaml
management:
  endpoints:
    web:
      exposure:
        include: metrics
```
访问:`GET /actuator/metrics/spring.test.context.cache`调试日志```properties
logging.level.org.springframework.test.context.cache=DEBUG
```
优化缓存命中率

按配置对测试进行分组```
 tests/
   unit/           # No context
   web/            # @WebMvcTest
   repository/     # @DataJpaTest  
   integration/    # @SpringBootTest
```
最小化@TestPropertySource变量

**Bad（多上下文）：**```java
@TestPropertySource(properties = "app.feature-x=true")
class FeatureXTest { }

@TestPropertySource(properties = "app.feature-y=true")
class FeatureYTest { }
```
* *更好(分组):* *```java
@TestPropertySource(properties = {"app.feature-x=true", "app.feature-y=true"})
class FeaturesTest { }
```
###谨慎使用@DirtiesContext

只有当上下文状态真正改变时：```java
@Test
@DirtiesContext // Forces context rebuild after test
void testThatModifiesBeanDefinitions() { }
```
最佳实践

1. **按配置分组** -将具有相同配置的测试放在一起
2. **限制属性变化** -在单个属性上使用配置文件
3. **避免@DirtiesContext** -首选测试数据清理
4. **使用窄片** - @WebMvcTest vs @SpringBootTest
5. **监控缓存命中** -偶尔启用调试日志