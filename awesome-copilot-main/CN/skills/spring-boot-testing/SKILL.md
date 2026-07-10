---
name: spring-boot-testing
description: Expert Spring Boot 4 testing specialist that selects the best Spring Boot testing techniques for your situation with Junit 6 and AssertJ.
---
#春季启动测试

该技能为使用现代模式和最佳实践测试Spring Boot 4应用程序提供了专家指南。

##核心原则

1. **测试金字塔**：单元（快速）>切片（聚焦）>集成（完整）
2. **正确的工具**：使用最窄的切片，给你信心
3. **AssertJ风格**：流畅、可读的断言优于冗长的匹配器
4. **现代api **：首选MockMvcTester和RestTestClient而不是遗留的替代方案

哪个测试片？

|场景|注释|参考||----------|------------|-----------|
|控制器+ HTTP语义|`@WebMvcTest`| [references/webmvctest.md](references/webmvctest.md) |
|库+ JPA查询|`@DataJpaTest`| [references/datajpatest.md](references/datajpatest.md) |
| REST客户端+外部接口|`@RestClientTest`| [references/restclienttest.md](references/restclienttest.md) |
| JSON （de）序列化|`@JsonTest`| [references/test-slices-overview.md](references/test-slices-overview.md) |
|全面应用|`@SpringBootTest`| [references/test-slices-overview.md](references/test-slices-overview.md) |

测试片引用

- [references/test-slices-overview.md](references/test-slices-overview.md) -决策矩阵和比较
- [references/webmvctest.md](references/webmvctest.md) - Web层与MockMvc
- [references/datajpatest.md](references/datajpatest.md) -带有Testcontainers的数据层
- [references/restclienttest.md](references/restclienttest.md) - REST客户端测试

测试工具参考

- [references/mockmvc-tester.md](references/mockmvc-tester.md) - assertj风格的MockMvc （3.2+）
- [references/mockmvc-classic.md](references/mockmvc-classic.md) -传统的MockMvc （pre-3.2）
—[references/resttestclient.md]（references/resttestclient.md）—Spring Boot 4+ REST客户端
- [references/mockitobean.md](references/mockitobean.md) -模拟依赖关系

断言库- [references/assertj-basics.md](references/assertj-basics.md) -标量，字符串，布尔值，日期
- [references/assertj-collections.md](references/assertj-collections.md) -列表，集合，映射，数组

# # Testcontainers

- [references/testcontainers-jdbc.md](references/testcontainers-jdbc.md) - PostgreSQL， MySQL等

测试数据生成

- [references/instancio.md](references/instancio.md) -生成复杂的测试对象（3+属性）

性能和迁移

- [references/context-caching.md](references/context-caching.md) -加速测试套件
- [references/sb4-migration.md](references/sb4-migration.md) - Spring Boot 4.0的变化

快速决策树```
Testing a controller endpoint?
  Yes → @WebMvcTest with MockMvcTester

Testing repository queries?
  Yes → @DataJpaTest with Testcontainers (real DB)

Testing business logic in service?
  Yes → Plain JUnit + Mockito (no Spring context)

Testing external API client?
  Yes → @RestClientTest with MockRestServiceServer

Testing JSON mapping?
  Yes → @JsonTest

Need full integration test?
  Yes → @SpringBootTest with minimal context config
```
##春季启动4亮点

- **RestTestClient**: TestRestTemplate的现代替代品
- **@MockitoBean**：取代@MockBean（已弃用）
- **MockMvcTester**: assertj风格的断言web测试
-模块化启动器**：特定于技术的测试启动器
- **上下文暂停**：自动暂停缓存的上下文（Spring Framework 7）

测试最佳实践

代码复杂性评估

当一个方法或类太复杂而无法有效测试时：

1. **分析复杂性**——如果你需要超过5-7个测试用例来覆盖一个方法，那么它可能太复杂了
2. **建议重构**——建议将代码分解成更小、更集中的函数
3. **用户决策** -如果用户同意重构，帮助确定提取点
4. **如果需要继续进行** -如果用户决定继续使用复杂的代码，尽管困难重重，也要执行测试**重构建议示例：**```java
// Before: Complex method hard to test
public Order processOrder(OrderRequest request) {
  // Validation, discount calculation, payment, inventory, notification...
  // 50+ lines of mixed concerns
}

// After: Refactored into testable units
public Order processOrder(OrderRequest request) {
  validateOrder(request);
  var order = createOrder(request);
  applyDiscount(order);
  processPayment(order);
  updateInventory(order);
  sendNotification(order);
  return order;
}
```
避免代码冗余

为常用对象和模拟设置创建助手方法，以增强可读性和可维护性。

###使用@DisplayName测试组织

使用描述性的显示名称来澄清测试意图：```java
@Test
@DisplayName("Should calculate discount for VIP customer")
void shouldCalculateDiscountForVip() { }

@Test
@DisplayName("Should reject order when customer has insufficient credit")
void shouldRejectOrderForInsufficientCredit() { }
```
测试覆盖顺序

始终按照以下顺序构建测试：

1. **主场景** -理想路径，最常见的用例
2. **其他路径** -可选择的有效场景，边缘情况
3. **Exceptions/Errors** -无效输入，错误条件，故障模式

测试生产场景

编写测试时要牢记真实的生产场景。这使得测试更具相关性，并有助于理解实际生产用例中的代码行为。

测试覆盖目标

将80%的代码覆盖率作为质量和努力之间的实际平衡。更高的覆盖率是有益的，但不是唯一的目标。

使用Jacoco maven插件进行覆盖率报告和跟踪。


* *覆盖规则:* *
-至少80%以上的覆盖率
-关注有意义的断言，而不仅仅是执行**优先级：**
1. 业务关键路径（支付处理、订单验证）
2. 复杂算法（定价、折扣计算）
3. 错误处理（异常、边缘情况）
4. 集成点（外部api、数据库）

##依赖（Spring Boot 4）```xml
<dependency>
  <groupId>org.springframework.boot</groupId>
  <artifactId>spring-boot-starter-test</artifactId>
  <scope>test</scope>
</dependency>

<!-- For WebMvc tests -->
<dependency>
  <groupId>org.springframework.boot</groupId>
  <artifactId>spring-boot-starter-webmvc-test</artifactId>
  <scope>test</scope>
</dependency>

<!-- For Testcontainers -->
<dependency>
  <groupId>org.springframework.boot</groupId>
  <artifactId>spring-boot-testcontainers</artifactId>
  <scope>test</scope>
</dependency>
```
