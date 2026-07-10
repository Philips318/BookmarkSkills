# @DataJpaTest

使用独立的数据层切片测试JPA存储库。

##基本结构```java
@DataJpaTest
@AutoConfigureTestDatabase(replace = AutoConfigureTestDatabase.Replace.NONE)
@Testcontainers
class OrderRepositoryTest {
  
  @Container
  @ServiceConnection
  static PostgreSQLContainer<?> postgres = new PostgreSQLContainer<>("postgres:18");
  
  @Autowired
  private OrderRepository orderRepository;
  
  @Autowired
  private TestEntityManager entityManager;
}
```
##什么得到加载

-存储库bean
—EntityManager / TestEntityManager
——数据源
-事务管理器
-没有web层，没有服务，没有控制器

测试自定义查询```java
@Test
void shouldFindOrdersByStatus() {
  // Given - Using var for cleaner code
  var pending = new Order("PENDING");
  var completed = new Order("COMPLETED");
  entityManager.persist(pending);
  entityManager.persist(completed);
  entityManager.flush();
  
  // When
  var pendingOrders = orderRepository.findByStatus("PENDING");
  
  // Then - Using sequenced collection methods
  assertThat(pendingOrders).hasSize(1);
  assertThat(pendingOrders.getFirst().getStatus()).isEqualTo("PENDING");
}
```
测试本机查询```java
@Test
void shouldExecuteNativeQuery() {
  entityManager.persist(new Order("PENDING", BigDecimal.valueOf(100)));
  entityManager.persist(new Order("PENDING", BigDecimal.valueOf(200)));
  entityManager.flush();
  
  var total = orderRepository.calculatePendingTotal();
  
  assertThat(total).isEqualTo(new BigDecimal("300.00"));
}
```
##测试分页```java
@Test
void shouldReturnPagedResults() {
  // Insert 20 orders using IntStream
  IntStream.range(0, 20).forEach(i -> {
    entityManager.persist(new Order("PENDING"));
  });
  entityManager.flush();
  
  var page = orderRepository.findByStatus("PENDING", PageRequest.of(0, 10));
  
  assertThat(page.getContent()).hasSize(10);
  assertThat(page.getTotalElements()).isEqualTo(20);
  assertThat(page.getContent().getFirst().getStatus()).isEqualTo("PENDING");
}
```
测试延迟加载```java
@Test
void shouldLazyLoadOrderItems() {
  var order = new Order("PENDING");
  order.addItem(new OrderItem("Product", 2));
  entityManager.persist(order);
  entityManager.flush();
  entityManager.clear(); // Detach from persistence context
  
  var found = orderRepository.findById(order.getId());
  
  assertThat(found).isPresent();
  // This will trigger lazy loading
  assertThat(found.get().getItems()).hasSize(1);
  assertThat(found.get().getItems().getFirst().getProduct()).isEqualTo("Product");
}
```
##测试级联```java
@Test
void shouldCascadeDelete() {
  var order = new Order("PENDING");
  order.addItem(new OrderItem("Product", 2));
  entityManager.persist(order);
  entityManager.flush();
  
  orderRepository.delete(order);
  entityManager.flush();
  
  assertThat(entityManager.find(OrderItem.class, order.getItems().getFirst().getId()))
    .isNull();
}
```
##测试@查询方法```java
@Query("SELECT o FROM Order o WHERE o.createdAt > :date AND o.status = :status")
List<Order> findRecentByStatus(@Param("date") LocalDateTime date, 
                               @Param("status") String status);

@Test
void shouldFindRecentOrders() {
  var old = new Order("PENDING");
  old.setCreatedAt(LocalDateTime.now().minusDays(10));
  var recent = new Order("PENDING");
  recent.setCreatedAt(LocalDateTime.now().minusHours(1));
  
  entityManager.persist(old);
  entityManager.persist(recent);
  entityManager.flush();
  
  var recentOrders = orderRepository.findRecentByStatus(
    LocalDateTime.now().minusDays(1), "PENDING");
  
  assertThat(recentOrders).hasSize(1);
  assertThat(recentOrders.getFirst().getId()).isEqualTo(recent.getId());
}
```
##使用H2 vs真实数据库

### H2（默认-不建议用于生产奇偶校验）```java
@DataJpaTest // Uses embedded H2 by default
class OrderRepositoryH2Test {
  // Fast but may miss DB-specific issues
}
```
Testcontainers（推荐）```java
@DataJpaTest
@AutoConfigureTestDatabase(replace = AutoConfigureTestDatabase.Replace.NONE)
@Testcontainers
class OrderRepositoryPostgresTest {
  @Container
  @ServiceConnection
  static PostgreSQLContainer<?> postgres = new PostgreSQLContainer<>("postgres:18");
}
```
##事务行为

默认情况下，测试是@事务性的，并在每次测试后回滚。```java
@Test
@Rollback(false) // Don't roll back (rarely needed)
void shouldPersistData() {
  orderRepository.save(new Order("PENDING"));
  // Data will remain in database after test
}
```
##要点

1. 使用TestEntityManager获取设置数据
2. 始终在persist（）之后执行flush（）以触发SQL
3. 清除（）实体管理器以测试延迟加载
4. 使用真实的数据库（Testcontainers）获得准确的结果
5. 测试成功案例和失败案例
6. 利用Java 25 var关键字进行更清晰的变量声明
7. 使用顺序收集方法(getFirst(), getLast（), reversed()）