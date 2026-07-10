# Testcontainers JDBC

使用Testcontainers用真实数据库测试JPA存储库。

# #概述

Testcontainers在Docker容器中为集成测试提供了真实的数据库实例。在生产平价方面比H2更可靠。

安装PostgreSQL

# # #依赖性```xml
<dependency>
  <groupId>org.springframework.boot</groupId>
  <artifactId>spring-boot-testcontainers</artifactId>
  <scope>test</scope>
</dependency>
<dependency>
  <groupId>org.testcontainers</groupId>
  <artifactId>testcontainers-postgresql</artifactId>
  <scope>test</scope>
</dependency>
```
基本测试```java
@DataJpaTest
@AutoConfigureTestDatabase(replace = AutoConfigureTestDatabase.Replace.NONE)
@Testcontainers
class OrderRepositoryPostgresTest {
  
  @Container
  @ServiceConnection
  static PostgreSQLContainer<?> postgres = new PostgreSQLContainer<>("postgres:18");
  
  @Autowired
  private OrderRepository orderRepository;
  
  @Autowired
  private TestEntityManager entityManager;
}
```
## MySQL设置```xml
<dependency>
  <groupId>org.testcontainers</groupId>
  <artifactId>testcontainers-mysql</artifactId>
  <scope>test</scope>
</dependency>
```

```java
@Container
@ServiceConnection
static MySQLContainer<?> mysql = new MySQLContainer<>("mysql:8.4");
```
##多数据库```java
@DataJpaTest
@AutoConfigureTestDatabase(replace = AutoConfigureTestDatabase.Replace.NONE)
@Testcontainers
class MultiDatabaseTest {
  
  @Container
  @ServiceConnection(name = "primary")
  static PostgreSQLContainer<?> primaryDb = new PostgreSQLContainer<>("postgres:18");
  
  @Container
  @ServiceConnection(name = "analytics")
  static PostgreSQLContainer<?> analyticsDb = new PostgreSQLContainer<>("postgres:18");
}
```
容器重用（速度优化）

添加到`~/.testcontainers.properties`：```properties
testcontainers.reuse.enable=true
```
然后在代码中启用重用：```java
@Container
@ServiceConnection
static PostgreSQLContainer<?> postgres = new PostgreSQLContainer<>("postgres:18")
  .withReuse(true);
```
##数据库初始化

使用SQL脚本```java
@Container
@ServiceConnection
static PostgreSQLContainer<?> postgres = new PostgreSQLContainer<>("postgres:18")
  .withInitScript("schema.sql");
```
###与Flyway```java
@SpringBootTest
@Testcontainers
@AutoConfigureTestDatabase(replace = AutoConfigureTestDatabase.Replace.NONE)
class MigrationTest {
  
  @Container
  @ServiceConnection
  static PostgreSQLContainer<?> postgres = new PostgreSQLContainer<>("postgres:18");
  
  @Autowired
  private Flyway flyway;
  
  @Test
  void shouldApplyMigrations() {
    flyway.migrate();
    // Test code
  }
}
```
##高级配置

自定义Database/Schema```java
@Container
@ServiceConnection
static PostgreSQLContainer<?> postgres = new PostgreSQLContainer<>("postgres:18")
  .withDatabaseName("testdb")
  .withUsername("testuser")
  .withPassword("testpass")
  .withInitScript("init-schema.sql");
```
等待策略```java
@Container
static PostgreSQLContainer<?> postgres = new PostgreSQLContainer<>("postgres:18")
  .waitingFor(Wait.forLogMessage(".*database system is ready.*", 1));
```
##测试示例```java
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
  
  @Test
  void shouldFindOrdersByStatus() {
    // Given
    entityManager.persist(new Order("PENDING"));
    entityManager.persist(new Order("COMPLETED"));
    entityManager.flush();
    
    // When
    List<Order> pending = orderRepository.findByStatus("PENDING");
    
    // Then
    assertThat(pending).hasSize(1);
    assertThat(pending.get(0).getStatus()).isEqualTo("PENDING");
  }
  
  @Test
  void shouldSupportPostgresSpecificFeatures() {
    // Can use Postgres-specific features like:
    // - JSONB columns
    // - Array types
    // - Full-text search
  }
}
```
## @DynamicPropertySource替代

如果不使用@ServiceConnection：```java
@SpringBootTest
@Testcontainers
class OrderServiceTest {
  
  @Container
  static PostgreSQLContainer<?> postgres = new PostgreSQLContainer<>("postgres:18");
  
  @DynamicPropertySource
  static void configureProperties(DynamicPropertyRegistry registry) {
    registry.add("spring.datasource.url", postgres::getJdbcUrl);
    registry.add("spring.datasource.username", postgres::getUsername);
    registry.add("spring.datasource.password", postgres::getPassword);
  }
}
```
支持的数据库

|数据库|容器类| Maven工件|| -------- | --------------- | -------------- |
| PostgreSQL | PostgreSQLContainer | testcontainers-postgresql |
MySQLContainer | testcontainers-mysql |
| MariaDB | MariaDBContainer | testcontainer - MariaDB |
| SQLServer | MSSQLServerContainer | testcontainers-mssqlserver |
| Oracle | oracleccontainer | testcontainer - Oracle -free |
| MongoDB | MongoDBContainer | testcontainers-mongodb |

最佳实践

1. 尽可能使用@ServiceConnection （Spring Boot 3.1+）
2. 启用容器重用以实现更快的本地构建
3. 使用特定版本（postgres:18）而不是最新版本
4. 保持容器配置在静态字段
5. 使用@DataJpaTest与AutoConfigureTestDatabase.Replace.NONE