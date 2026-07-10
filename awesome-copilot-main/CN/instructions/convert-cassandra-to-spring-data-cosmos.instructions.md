---
description: 'Step-by-step guide for converting Spring Boot Cassandra applications to use Azure Cosmos DB with Spring Data Cosmos'
applyTo: '**/*.java,**/pom.xml,**/build.gradle,**/application*.properties,**/application*.yml,**/application*.conf'
---
#综合指南：使用Spring Data Cosmos （Spring - Data - Cosmos）将Spring Boot Cassandra应用程序转换为Azure Cosmos DB

# #适用性

本指南适用于：
-✅Spring BootX - 3。X应用程序（反应性和非反应性）
-✅基于Maven和gradle的项目
-✅使用Spring Data Cassandra、Cassandra dao或DataStax驱动的应用
-✅有或没有Lombok的项目
-✅基于uuid或基于字符串的实体标识符
-✅同步和响应式（Spring WebFlux）应用

本指南不包括：
-❌非spring框架（Jakarta EE, Micronaut, Quarkus, plain Java）
-❌复杂的Cassandra功能（物化视图，udt，计数器，自定义类型）
-❌批量数据迁移（仅支持代码转换，数据必须单独迁移）
-❌cassandra特有的功能，如轻量级事务（LWT）或跨分区的批处理操作# #概述

本指南提供了使用Spring Data Cosmos将响应式Spring Boot应用程序从Apache Cassandra转换到Azure Cosmos DB的分步说明。它涵盖了所有遇到的主要问题及其解决方案，基于现实世界的转换经验。

# #先决条件

- Java 11或更高版本（Spring Boot 3.x需要Java 17+）
-安装并验证Azure CLI （`az login`）用于本地开发
-在Azure Portal中创建的Azure Cosmos数据库帐户
- Maven 3.6+或Gradle 6+（取决于你的项目）
-适用于Spring Boot 3的Gradle项目。确保JAVA_HOME环境变量指向Java 17+
-对应用程序的数据模型和查询模式有基本的了解

为Azure Cosmos DB设置数据库

**CRITICAL**：在运行应用程序之前，请确保数据库存在于Cosmos DB帐户中。选项1：手动创建数据库（建议首次运行）
1. 转到Azure Portal→您的Cosmos DB帐户
2. 导航到“数据浏览器”
3. 点击“新建数据库”
4. 输入与应用程序配置匹配的数据库名称（检查`application.properties`或`application.yml`以获取配置的数据库名称）
5. 选择吞吐量设置（根据需要手动或自动缩放）
-从手动400RU/s开始development/testing-对具有可变流量的生产工作负载使用Autoscale
6. 单击OK

选项2：自动创建
Spring Data Cosmos可以在第一次连接时自动创建数据库，但这需要：
适当的RBAC权限（Cosmos DB内置数据贡献者角色）
—如果权限不足，可能会失败容器（集合）创建
当应用程序启动时，Spring Data Cosmos将使用实体中的`@Container`注释设置自动创建容器。除非您想配置特定的吞吐量或索引策略，否则不需要手动创建容器。

使用Azure Cosmos数据库进行身份验证

使用DefaultAzureCredential（推荐）
对于开发和生产，推荐使用`DefaultAzureCredential`认证方法：

**工作原理**：
1. 按顺序尝试多个凭据源：
—环境变量
-工作负荷识别（适用于AKS）
-托管身份（用于AzureVMs/App服务）
- Azure命令行（`az login`）
- Azure PowerShell
- Azure开发者命令行

**本地开发设置**：```bash
# Login via Azure CLI
az login

# The application will automatically use your CLI credentials
```
**配置**（无需钥匙）：```java
@Bean
public CosmosClientBuilder getCosmosClientBuilder() {
    return new CosmosClientBuilder()
        .endpoint(uri)
        .credential(new DefaultAzureCredentialBuilder().build());
}
```
**属性文件** (application-cosmos。属性或application.properties)：```properties
azure.cosmos.uri=https://<your-cosmos-account-name>.documents.azure.com:443/
azure.cosmos.database=<your-database-name>
# No key property needed when using DefaultAzureCredential
azure.cosmos.populate-query-metrics=false
```
**注**：用实际值替换`<your-cosmos-account-name>`和`<your-database-name>`。

RBAC权限要求
当使用DefaultAzureCredential时，你的Azure身份需要适当的RBAC权限：

**常见启动错误**：```
Request blocked by Auth: Request for Read DatabaseAccount is blocked because principal
[xxx] does not have required RBAC permissions to perform action
[Microsoft.DocumentDB/databaseAccounts/sqlDatabases/write] on any scope.
```
解决方案**：分配“Cosmos DB内置数据贡献者”角色：```bash
# Get your user's object ID
PRINCIPAL_ID=$(az ad signed-in-user show --query id -o tsv)

# Assign the role (replace <resource-group> with your actual resource group)
az cosmosdb sql role assignment create \
  --account-name your-cosmos-account \
  --resource-group <resource-group> \
  --scope "/" \
  --principal-id $PRINCIPAL_ID \
  --role-definition-name "Cosmos DB Built-in Data Contributor"
```
**替代方案**：如果您使用`az login`登录，如果您是Cosmos DB帐户的owner/contributor，则您的帐户应该已经具有权限。

基于密钥的身份验证（仅限本地仿真器）
仅在本地模拟器开发中使用基于密钥的身份验证：```java
@Bean
public CosmosClientBuilder getCosmosClientBuilder() {
    // Only for local emulator
    if (key != null && !key.isEmpty()) {
        return new CosmosClientBuilder()
            .endpoint(uri)
            .key(key);
    }
    // Production: use DefaultAzureCredential
    return new CosmosClientBuilder()
        .endpoint(uri)
        .credential(new DefaultAzureCredentialBuilder().build());
}
```
##重要的经验教训

Java版本要求（Spring Boot 3.x）
问题：Spring Boot 3.0+需要Java 17或更高版本。使用Java 11会导致构建失败。
* * * *错误:```
No matching variant of org.springframework.boot:spring-boot-gradle-plugin:3.0.5 was found.
Incompatible because this component declares a component compatible with Java 17
and the consumer needed a component compatible with Java 11
```
* * * *解决方案:```bash
# Check Java version
java -version

# Set JAVA_HOME to Java 17+
export JAVA_HOME=/usr/lib/jvm/java-17-openjdk-amd64  # Linux
# or
export JAVA_HOME=/Library/Java/JavaVirtualMachines/jdk-17.jdk/Contents/Home  # macOS

# Verify
echo $JAVA_HOME
```
对于Gradle项目**，始终使用正确的JAVA_HOME运行：```bash
export JAVA_HOME=/path/to/java-17
./gradlew clean build
./gradlew bootRun
```
gradle特有的问题

####问题1：旧配置文件冲突
**问题**：重命名或替换Cassandra配置文件时，旧文件可能仍然存在，导致编译错误：```
error: class CosmosConfiguration is public, should be declared in a file named CosmosConfiguration.java
```
解决方案**：显式删除旧的Cassandra配置文件：```bash
# Find and remove old Cassandra config files
find src/main/java -name "*CassandraConfig*.java" -o -name "*CassandraConfiguration*.java"
# Review the output, then delete if appropriate
rm src/main/java/<path-to-old-config>/CassandraConfig.java
```
####问题2:Repository findAllById返回Iterable
问题：CosmosRepository的`findAllById()`返回`Iterable<Entity>`，而不是`List<Entity>`。直接调用`.stream()`失败：```
error: cannot find symbol
  symbol:   method stream()
  location: interface Iterable<YourEntity>
```
**解决方案**：正确处理Iterable：```java
// WRONG - Iterable doesn't have stream() method
var entities = repository.findAllById(ids).stream()...

// CORRECT - Option 1: Use forEach to populate a collection
Iterable<YourEntity> entitiesIterable = repository.findAllById(ids);
Map<String, YourEntity> entityMap = new HashMap<>();
entitiesIterable.forEach(entity -> entityMap.put(entity.getId(), entity));

// CORRECT - Option 2: Convert to List first
List<YourEntity> entities = new ArrayList<>();
repository.findAllById(ids).forEach(entities::add);

// CORRECT - Option 3: Use StreamSupport (Java 8+)
List<YourEntity> entities = StreamSupport.stream(
    repository.findAllById(ids).spliterator(), false)
    .collect(Collectors.toList());
```
###package-info.javajavax注释的问题
**问题**:`package-info.java`使用`javax.annotation.ParametersAreNonnullByDefault`导致Java 11+编译错误：```
error: cannot find symbol
import javax.annotation.ParametersAreNonnullByDefault;
```
解决方案**：删除或简化package-info.java文件：```java
// Simple version - just package declaration
package com.your.package;
```
实体构造函数问题
**问题**：使用带有手动构造函数的Lombok`@NoArgsConstructor`会导致重复构造函数编译错误。
解决方案**：选择一种方法：
-选项1：删除`@NoArgsConstructor`并保留手动构造器
-选项2：删除手动构造函数并依赖Lombok注释
- **最佳实践**：对于具有初始化逻辑（如设置分区键）的Cosmos实体，删除`@NoArgsConstructor`并仅使用手动构造函数。

删除业务对象构造函数
**问题**：从实体类中删除`@AllArgsConstructor`或自定义构造函数会破坏使用这些构造函数的现有代码。
**影响**：映射工具、数据种子器和测试文件将无法编译。
* * * *解决方案:
-删除或修改构造函数后，搜索所有文件以查找对这些实体的构造函数调用
-用默认构造函数+ setter模式替换：  ```java
  // Before - using all-args constructor
  MyEntity entity = new MyEntity(id, field1, field2, field3);

  // After - using default constructor + setters
  MyEntity entity = new MyEntity();
  entity.setId(id);
  entity.setField1(field1);
  entity.setField2(field2);
  entity.setField3(field3);
  ```
数据种子器构造函数调用
**问题**：数据播种或初始化代码使用实体构造函数，在实体转换为Cosmos注释后可能不存在。
**解决方案**：更新数据播种组件中的所有实体实例以使用setter：```java
// Before - constructor-based initialization
MyEntity entity1 = new MyEntity("entity-1", "value1", "value2");

// After - setter-based initialization
MyEntity entity1 = new MyEntity();
entity1.setId("entity-1");
entity1.setField1("value1");
entity1.setField2("value2");
```
**要检查的常见文件**:DataSeeder， DatabaseInitializer, TestDataLoader，或任何实现`CommandLineRunner`的`@Component````java
OwnerEntity owner1 = new OwnerEntity();
owner1.setId("owner-1");
```
需要更新测试文件
**问题**：测试文件引用旧的Cassandra dao并使用UUID构造器。
**需要更新的关键文件**：
1. 删除`MockReactiveResultSet.java`（特定于cassandra）
2. 更新`*ReactiveServicesTest.java`-用Cosmos存储库替换DAO引用
3. 更新`*ReactiveControllerTest.java`-用Cosmos存储库替换DAO引用
4. 将所有`UUID.fromString()`替换为String id
5. 用setter模式替换构造函数调用：`new Owner(UUID.fromString(...))`应用程序启动和DefaultAzureCredential行为
**重要**:DefaultAzureCredential依次尝试多种身份验证方法，这是正常和预期的。

**期望的启动日志模式**：```
INFO c.azure.identity.ChainedTokenCredential : Azure Identity => Attempted credential EnvironmentCredential is unavailable.
INFO c.azure.identity.ChainedTokenCredential : Azure Identity => Attempted credential WorkloadIdentityCredential is unavailable.
INFO c.azure.identity.ChainedTokenCredential : Azure Identity => Attempted credential ManagedIdentityCredential is unavailable.
INFO c.azure.identity.ChainedTokenCredential : Azure Identity => Attempted credential SharedTokenCacheCredential is unavailable.
INFO c.azure.identity.ChainedTokenCredential : Azure Identity => Attempted credential IntelliJCredential is unavailable.
INFO c.azure.identity.ChainedTokenCredential : Azure Identity => Attempted credential AzureCliCredential is unavailable.
INFO c.azure.identity.ChainedTokenCredential : Azure Identity => Attempted credential AzurePowerShellCredential is unavailable.
INFO c.azure.identity.ChainedTokenCredential : Azure Identity => Attempted credential AzureDeveloperCliCredential returns a token
```
* * * *要点:
-“不可用”消息是**正常** -它正在按顺序尝试每个凭据源
-一旦它找到一个工作（例如，AzureCliCredential或AzureDeveloperCliCredential），它将使用它
- **不要中断启动过程** -在凭据源之间循环需要10-15秒
—应用程序通常需要30-60秒才能完全启动并连接到Cosmos DB

* * * *成功指标:```
INFO c.a.c.i.RxDocumentClientImpl : Initializing DocumentClient [1] with serviceEndpoint [https://your-account.documents.azure.com:443/]
INFO c.a.c.i.GlobalEndpointManager : db account retrieved {...}
INFO c.a.c.implementation.SessionContainer : Registering a new collection resourceId [...]
INFO o.s.b.w.embedded.tomcat.TomcatWebServer : Tomcat started on port(s): 8944 (http)
INFO com.your.app.Application : Started Application in X.XXX seconds
```
**排除启动失败**：

1. **如果所有凭据都“不可用”**：   ```bash
   # Re-authenticate with Azure CLI
   az login

   # Verify login
   az account show
   ```
2. **如果看到权限错误**：   ```
   Request blocked by Auth: principal [xxx] does not have required RBAC permissions
   ```
-确保Cosmos DB帐户中存在数据库（参见数据库设置部分）
-验证RBAC权限（参见身份验证部分）
-检查您是否已登录到正确的Azure订阅

3. **已使用的端口**：   ```bash
   # Find and kill the process
   lsof -ti:8944 | xargs kill -9

   # Or change the port in application.properties
   server.port=8945
   ```
###应用启动耐心
**问题**：应用程序需要30-60秒才能完全启动（编译+ Spring Boot + Cosmos DB连接）。
* * * *解决方案:
-对于Gradle:`./gradlew bootRun`（默认在前台运行）
—对于Maven:`mvn spring-boot:run`—如果需要，使用后台执行：`nohup ./gradlew bootRun > app.log 2>&1 &`- **CRITICAL**：不要中断启动过程，特别是在凭据认证期间（10-15秒）
—监控日志：`tail -f app.log`或检查“已启动的应用程序”消息
—等待Tomcat启动并显示端口号后，再测试端点

###端口配置
**问题**：应用程序可能无法在默认端口8080上运行。
* * * *解决方案:
—实际端口：`ss -tlnp | grep java`—测试连通性：`curl http://localhost:<port>/petclinic/api/owners`—普通端口：8080、9966、9967

系统编译错误解决方案

在转换过程中，我们遇到了100多个编译错误。下面是解决这些问题的系统方法：###步骤1：识别残留的Cassandra文件
**问题**：旧的cassandra特定文件在删除依赖项后导致编译错误。
**解决方案**：系统删除所有cassandra专用文件：```bash
# Identify and delete old DAOs
find . -name "*Dao.java" -o -name "*DAO.java"
# Delete: OwnerReactiveDao, PetReactiveDao, VetReactiveDao, VisitReactiveDao

# Identify and delete Cassandra mappers
find . -name "*Mapper.java" -o -name "*EntityToOwnerMapper.java"
# Delete: EntityToOwnerMapper, EntityToPetMapper, EntityToVetMapper, EntityToVisitMapper

# Identify and delete old configuration
find . -name "*CassandraConfig.java" -o -name "CassandraConfiguration.java"
# Delete: CassandraConfiguration.java

# Identify test utilities for Cassandra
find . -name "MockReactiveResultSet.java"
# Delete: MockReactiveResultSet.java (Cassandra-specific test utility)
```
###步骤2：运行增量编译检查
**方法**：在每次重大更改后，进行编译以确定遗留问题：```bash
# After deleting old files
mvn compile 2>&1 | grep -E "(ERROR|error)" | wc -l
# Expected: Number decreases with each fix

# After updating entity constructors
mvn compile 2>&1 | grep "constructor"
# Identify constructor-related compilation errors

# After fixing business object constructors
mvn compile 2>&1 | grep -E "(new Owner|new Pet|new Vet|new Visit)"
# Identify remaining constructor calls that need fixing
```
步骤3：系统地修复与构造函数相关的错误
**模式**：搜索特定文件类型中的所有构造函数调用：```bash
# Find all constructor calls in MappingUtils
grep -n "new Owner\|new Pet\|new Vet\|new Visit" src/main/java/**/MappingUtils.java

# Find all constructor calls in DataSeeder
grep -n "new OwnerEntity\|new PetEntity\|new VetEntity\|new VisitEntity" src/main/java/**/DataSeeder.java

# Find all constructor calls in test files
grep -rn "new Owner\|new Pet\|new Vet\|new Visit" src/test/java/
```
###步骤4：更新最后一次测试
**原理**：在测试代码之前修复应用代码，以便清楚地看到所有问题；

1. 第一：更新测试存储库模拟（DAO→Cosmos存储库）
2. 第二：修复测试数据中的UUID→字符串转换
3. 第三：更新测试设置中的构造函数调用
4. 最后：运行测试验证：`mvn test`步骤5：验证零编译错误
* *最后检查* *:```bash
# Clean and full compile
mvn clean compile

# Should see: BUILD SUCCESS
# Should NOT see any ERROR messages

# Verify test compilation
mvn test-compile

# Run tests
mvn test
```
* * * *成功指标:
-`mvn compile`: BUILD SUCCESS
-`mvn test`：所有测试都通过（即使有些测试被跳过）
—无ERROR信息输出
-没有“无法找到符号”错误
-没有“构造函数不能被应用”错误

##转换步骤

# # # 1。更新Maven依赖项

####删除Cassandra依赖项```xml
<!-- REMOVE these Cassandra dependencies -->
<dependency>
    <groupId>com.datastax.oss</groupId>
    <artifactId>java-driver-core</artifactId>
</dependency>
<dependency>
    <groupId>com.datastax.oss</groupId>
    <artifactId>java-driver-query-builder</artifactId>
</dependency>
```
####添加Azure Cosmos依赖项```xml
<!-- Azure Spring Data Cosmos (Java 11 compatible) -->
<dependency>
    <groupId>com.azure</groupId>
    <artifactId>azure-spring-data-cosmos</artifactId>
    <version>3.46.0</version>
</dependency>

<!-- Azure Identity for DefaultAzureCredential authentication -->
<dependency>
    <groupId>com.azure</groupId>
    <artifactId>azure-identity</artifactId>
    <version>1.11.4</version>
</dependency>
```
####紧急：添加兼容性版本管理
弹簧引导2.3。x与Azure库的版本冲突。将此添加到`<dependencyManagement>`部分：```xml
<dependencyManagement>
    <dependencies>
        <!-- Override reactor-netty version to fix compatibility with azure-spring-data-cosmos -->
        <dependency>
            <groupId>io.projectreactor.netty</groupId>
            <artifactId>reactor-netty</artifactId>
            <version>1.0.40</version>
        </dependency>
        <dependency>
            <groupId>io.projectreactor.netty</groupId>
            <artifactId>reactor-netty-http</artifactId>
            <version>1.0.40</version>
        </dependency>
        <dependency>
            <groupId>io.projectreactor.netty</groupId>
            <artifactId>reactor-netty-core</artifactId>
            <version>1.0.40</version>
        </dependency>

        <!-- Override reactor-core version to support Sinks API required by azure-identity -->
        <dependency>
            <groupId>io.projectreactor</groupId>
            <artifactId>reactor-core</artifactId>
            <version>3.4.32</version>
        </dependency>

        <!-- Override Netty versions to fix compatibility with Azure Cosmos Client -->
        <dependency>
            <groupId>io.netty</groupId>
            <artifactId>netty-bom</artifactId>
            <version>4.1.101.Final</version>
            <type>pom</type>
            <scope>import</scope>
        </dependency>

        <!-- Override netty-tcnative to match Netty version -->
        <dependency>
            <groupId>io.netty</groupId>
            <artifactId>netty-tcnative-boringssl-static</artifactId>
            <version>2.0.62.Final</version>
        </dependency>
    </dependencies>
</dependencyManagement>
```
# # # 2。配置设置

####创建Cosmos配置类
将您的Cassandra配置替换为：```java
@Configuration
@EnableCosmosRepositories  // Required for non-reactive repositories
@EnableReactiveCosmosRepositories  // CRITICAL: Required for reactive repositories
public class CosmosConfiguration extends AbstractCosmosConfiguration {

    @Value("${azure.cosmos.uri}")
    private String uri;

    @Value("${azure.cosmos.database}")
    private String database;

    @Bean
    public CosmosClientBuilder getCosmosClientBuilder() {
        return new CosmosClientBuilder()
            .endpoint(uri)
            .credential(new DefaultAzureCredential());
    }

    @Bean
    public CosmosAsyncClient cosmosAsyncClient(CosmosClientBuilder cosmosClientBuilder) {
        return cosmosClientBuilder.buildAsyncClient();
    }

    @Bean
    public CosmosClientBuilderFactory cosmosFactory(CosmosAsyncClient cosmosAsyncClient) {
        return new CosmosClientBuilderFactory(cosmosAsyncClient);
    }

    @Bean
    public ReactiveCosmosTemplate reactiveCosmosTemplate(CosmosClientBuilderFactory cosmosClientBuilderFactory) {
        return new ReactiveCosmosTemplate(cosmosClientBuilderFactory, database);
    }

    @Override
    protected String getDatabaseName() {
        return database;
    }
}
```
* *重要指出:* *
- **需要两个注释**:@EnableCosmosRepositories和@EnableReactiveCosmosRepositories
缺少@EnableReactiveCosmosRepositories将导致响应式存储库出现“No qualifying bean”错误

####应用属性
添加cosmos配置文件配置：```properties
# application-cosmos.properties
azure.cosmos.uri=https://your-cosmos-account.documents.azure.com:443/
azure.cosmos.database=your-database-name
```
# # # 3。实体转换

####从Cassandra转换到Cosmos注释

* *前(Cassandra): * *```java
@Table(value = "entity_table")
public class EntityName {
    @PartitionKey
    private UUID id;

    @ClusteringColumn
    private String fieldName;

    @Column("column_name")
    private String anotherField;
}
```
(宇宙):后* * * *```java
@Container(containerName = "entities")
public class EntityName {
    @Id
    private String id;  // Changed from UUID to String

    @PartitionKey
    private String fieldName;  // Choose appropriate partition key

    private String anotherField;

    // Generate String IDs
    public EntityName() {
        this.id = UUID.randomUUID().toString();
    }
}
```
####主要变化：
—将`@Table`替换为`@Container(containerName = "...")`—修改`@PartitionKey`为Cosmos分区键策略
—将所有id从`UUID`转换为`String`-删除`@Column`注释（Cosmos使用字段名）
-删除`@ClusteringColumn`（不适用宇宙）

# # # 4。库转换

####用Cosmos repository代替Cassandra Data Access Layer

**如果您的应用程序使用dao或自定义数据访问类

**前（卡桑德拉DAO图案）：**```java
@Repository
public class EntityReactiveDao {
    // Custom Cassandra query methods
}
```
**后（Cosmos Repository）：**```java
@Repository
public interface EntityCosmosRepository extends ReactiveCosmosRepository<EntityName, String> {

    @Query("SELECT * FROM entities e WHERE e.fieldName = @fieldName")
    Flux<EntityName> findByFieldName(@Param("fieldName") String fieldName);

    @Query("SELECT * FROM entities e WHERE e.id = @id")
    Mono<EntityName> findEntityById(@Param("id") String id);
}
```
**如果您的应用程序使用Spring Data Cassandra存储库

* *: * *```java
@Repository
public interface EntityCassandraRepository extends ReactiveCassandraRepository<EntityName, UUID> {
    // Cassandra-specific methods
}
```
* *: * *```java
@Repository
public interface EntityCosmosRepository extends ReactiveCosmosRepository<EntityName, String> {
    // Convert existing methods to Cosmos queries
}
```
**如果您的应用程序使用直接CqlSession或Cassandra驱动程序：**
-用存储库模式替换直接驱动程序调用
—将CQL查询转换为Cosmos SQL语法
—实现如上所示的存储库接口####
- **临界**：使用`ReactiveCosmosRepository<Entity, String>`响应式编程（不是CosmosRepository）
-使用`CosmosRepository<Entity, String>`用于非响应式应用程序
—**存储库接口更改**：如果从现有的Cassandrarepositories/DAOs转换，请确保所有存储库接口都扩展ReactiveCosmosRepository
- **常见错误**：“没有合格的bean类型ReactiveCosmosRepository”=缺少@EnableReactiveCosmosRepositories
- **如果使用自定义数据访问类**：转换为存储库模式，以便更好地集成
- **如果已经使用Spring Data**：将接口扩展名从ReactiveCassandraRepository更改为ReactiveCosmosRepository
使用类似sql的语法（不是CQL）实现带有`@Query`注释的自定义查询
—所有查询参数必须使用`@Param`标注

# # # 5。服务层更新

####更新响应式编程的服务类（如果适用）**如果你的应用程序有一个服务层

**CRITICAL：服务方法必须返回Flux/Mono，而不是Iterable/Optional```java
@Service
public class EntityReactiveServices {
    private final EntityCosmosRepository repository;

    public EntityReactiveServices(EntityCosmosRepository repository) {
        this.repository = repository;
    }

    // CORRECT: Returns Flux<EntityName>
    public Flux<EntityName> findAll() {
        return repository.findAll();
    }

    // CORRECT: Returns Mono<EntityName>
    public Mono<EntityName> findById(String id) {
        return repository.findById(id);
    }

    // CORRECT: Returns Mono<EntityName>
    public Mono<EntityName> save(EntityName entity) {
        return repository.save(entity);
    }

    // Custom queries - MUST return Flux/Mono
    public Flux<EntityName> findByFieldName(String fieldName) {
        return repository.findByFieldName(fieldName);
    }

    // WRONG PATTERNS TO AVOID:
    // public Iterable<EntityName> findAll() - Will cause compilation errors
    // public Optional<EntityName> findById() - Will cause compilation errors
    // repository.findAll().collectList() - Unnecessary blocking
}
```
**如果你的应用在控制器中使用直接存储库注入
考虑添加一个服务层，以更好地分离关注点
-更新控制器依赖以使用新的Cosmos存储库
-确保在整个调用链中正确处理响应式类型

共同问题:* * * *
- **编译错误**:“Cannot resolve method”使用Iterable返回类型时
- **运行时错误**：尝试调用。collectList（）阻止不必要的()
**性能**：阻塞响应式流违背了响应式编程的目的

# # # 6。控制器更新（如适用）

####更新String id的REST控制器

**如果你的应用程序有REST控制器

* *: * *```java
@GetMapping("/entities/{entityId}")
public Mono<EntityDto> getEntity(@PathVariable UUID entityId) {
    return entityService.findById(entityId);
}
```
* *: * *```java
@GetMapping("/entities/{entityId}")
public Mono<EntityDto> getEntity(@PathVariable String entityId) {
    return entityService.findById(entityId);
}
```
**如果你的应用程序不使用控制器
—在数据访问层应用相同的UUID→字符串转换原则
—更新accept/return实体id的所有外部api或接口

# # # 7。数据映射工具（如适用）

####更新域对象与实体的对应关系

**如果您的应用程序使用映射工具或转换器```java
public class MappingUtils {

    // Convert domain object to entity
    public static EntityName toEntity(DomainObject domain) {
        EntityName entity = new EntityName();
        entity.setId(domain.getId()); // Now String instead of UUID
        entity.setFieldName(domain.getFieldName());
        entity.setAnotherField(domain.getAnotherField());
        // ... other fields
        return entity;
    }

    // Convert entity to domain object
    public static DomainObject toDomain(EntityName entity) {
        DomainObject domain = new DomainObject();
        domain.setId(entity.getId());
        domain.setFieldName(entity.getFieldName());
        domain.setAnotherField(entity.getAnotherField());
        // ... other fields
        return domain;
    }
}
```
**如果你的应用程序不使用显式映射
确保在整个代码库中使用一致的ID类型
-更新任何对象构造或复制逻辑来处理字符串id

# # # 8。测试更新

####更新测试类

**关键**：所有测试文件必须更新以使用String id和Cosmos存储库。```java
**If your application has unit tests:**

```java
@ExtendWith (MockitoExtension.class)
类EntityReactiveServicesTest {    @Mock
    private EntityCosmosRepository entityRepository; // Updated to Cosmos repository

    @InjectMocks
    private EntityReactiveServices entityService;

    @Test
    void testFindById() {
        String entityId = "test-entity-id"; // Changed from UUID to String
        EntityName mockEntity = new EntityName();
        mockEntity.setId(entityId);

        when(entityRepository.findById(entityId)).thenReturn(Mono.just(mockEntity));

        StepVerifier.create(entityService.findById(entityId))
            .expectNext(mockEntity)
            .verifyComplete();
    }
}
```

**If your application has integration tests:**
- Update test data setup to use String IDs
- Replace Cassandra test containers with Cosmos DB emulator (if available)
- Update test queries to use Cosmos SQL syntax instead of CQL

**If your application doesn't have tests:**
- Consider adding basic tests to verify the conversion works correctly
- Focus on testing ID conversion and basic CRUD operations
```
# # # 9。常见问题及解决方案

####问题1:NoClassDefFoundError with reactor.core.publisher. sink
**问题**:Azure Identity库需要更新的Reactor Core版本
* *误差* *:`java.lang.NoClassDefFoundError: reactor/core/publisher/Sinks`**根本原因**:Spring Boot 2.3。x使用旧的反应堆堆芯，没有下沉API
**解决方案**：在dependencyManagement中添加reactor-core版本覆盖（参见步骤1）

####问题2:Netty Epoll方法的NoSuchMethodError
问题：Spring Boot Netty和Azure Cosmos要求之间的版本不匹配
* *误差* *:`java.lang.NoSuchMethodError: 'boolean io.netty.channel.epoll.Epoll.isTcpFastOpenClientSideAvailable()'`**根本原因**:Spring Boot 2.3。x使用Netty 4.1.51。最后，Azure需要更新的方法
**解决方案**：添加netty-bom版本覆盖（参见步骤1）####问题3：带SSL上下文的NoSuchMethodError
问题：Netty TLS原生库版本不匹配
* *误差* *:`java.lang.NoSuchMethodError: 'boolean io.netty.internal.tcnative.SSLContext.setCurvesList(long, java.lang.String[])'`**根本原因**:Netty替代版本与升级后的Netty不兼容
解决方案**：添加netty- tnative -boringssl-static版本覆盖（参见步骤1）

####问题4：未创建ReactiveCosmosRepository bean
**问题**：缺少@EnableReactiveCosmosRepositories注释
* *误差* *:`No qualifying bean of type 'ReactiveCosmosRepository' available`**根本原因**：只有@EnableCosmosRepositories不会创建响应式存储库bean
**解决方案**：将@EnableCosmosRepositories和@EnableReactiveCosmosRepositories添加到配置中####问题5：存储库接口编译错误
**问题**：使用CosmosRepository而不是ReactiveCosmosRepository
* *误差* *:`Cannot resolve method 'findAll()' in 'CosmosRepository'`**根本原因：CosmosRepository返回Iterable，而不是Flux
**解决方案**：更改所有存储库接口以扩展ReactiveCosmosRepository<Entity, String>

####问题6：服务层响应式类型不匹配
**问题**：服务方法返回Iterable/Optional而不是Flux/Mono* *误差* *:`Required type: Flux<Entity> Provided: Iterable<Entity>`**根本原因：存储库方法返回响应类型，服务必须匹配
解决方案**：更新所有服务方法签名返回Flux/Mono####问题7：使用DefaultAzureCredential进行身份验证失败
**问题**:DefaultAzureCredential找不到凭据
**错误**:`All credentials in the chain are unavailable`或特定凭据不可用消息
**根本原因**：没有可用的有效Azure凭据源* * * *解决方案:
1. **本地开发**：确保Azure CLI登录   ```bash
   az login
   # Verify login
   az account show
   ```
2. **对于azure托管的应用程序**：确保启用了Managed Identity并具有适当的RBAC权限

3. **检查凭证链顺序**:DefaultAzureCredential按此顺序尝试：
-环境变量→工作负载标识→托管标识→Azure CLI→PowerShell→Developer CLI

####问题8：数据库未发现错误
**问题**：应用程序启动失败，数据库未找到错误
**错误：`Database 'your-database-name' not found`或`Resource Not Found`**根本原因**:Cosmos DB帐户中不存在数据库

**解决方案**：在第一次运行之前创建数据库（参见数据库设置部分）：```bash
# Via Azure CLI
az cosmosdb sql database create \
  --account-name your-cosmos-account \
  --name your-database-name \
  --resource-group your-resource-group

# Or via Azure Portal (recommended for first-time setup)
# Portal → Cosmos DB → Data Explorer → New Database
```
**注**：容器（集合）将从实体`@Container`注释自动创建，但数据库本身可能需要首先存在，这取决于您的RBAC权限。

####问题9:RBAC权限错误
**问题**：应用程序失败，出现权限拒绝错误
* * * *错误:```
Request blocked by Auth: principal [xxx] does not have required RBAC permissions
to perform action [Microsoft.DocumentDB/databaseAccounts/sqlDatabases/write]
```
**根本原因**：您的Azure身份缺乏所需的Cosmos DB权限

**解决方案**：分配“Cosmos DB内置数据贡献者”角色：```bash
# Get resource group
RESOURCE_GROUP=$(az cosmosdb show --name your-cosmos-account --query resourceGroup -o tsv 2>/dev/null)

# If the above fails, list all Cosmos accounts to find it
az cosmosdb list --query "[?name=='your-cosmos-account'].{name:name, resourceGroup:resourceGroup}" -o table

# Assign role
az cosmosdb sql role assignment create \
  --account-name your-cosmos-account \
  --resource-group $RESOURCE_GROUP \
  --scope "/" \
  --principal-id $(az ad signed-in-user show --query id -o tsv) \
  --role-definition-name "Cosmos DB Built-in Data Contributor"
```
**备选方案**：门户→Cosmos DB→访问控制（IAM）→添加角色分配→“Cosmos DB内置数据贡献者”

####问题10：分区键策略差异
问题：Cassandra集群键不直接映射到Cosmos分区键
**错误**：跨分区查询或性能差
**根本原因**：不同的数据分发策略
**解决方案**：根据查询模式选择合适的分区键，通常是查询最频繁的字段

####问题10:UUID到字符串的转换问题
问题：测试文件和控制器仍然使用UUID类型
**错误**:`Cannot convert UUID to String`或类型不匹配错误
**根本原因**：并非所有UUID的出现都被转换为字符串
**解决方案**：系统地搜索并替换所有UUID引用String

# # # 10。数据播种（如适用）

####实现数据填充

**如果你的应用需要初始数据：**```java
@Component
public class DataSeeder implements CommandLineRunner {

    private final EntityCosmosRepository entityRepository;

    @Override
    public void run(String... args) throws Exception {
        if (entityRepository.count().block() == 0) {
            // Seed initial data
            EntityName entity = new EntityName();
            entity.setFieldName("Sample Value");
            entity.setAnotherField("Sample Data");

            entityRepository.save(entity).block();
        }
    }
}
```
**如果您的应用程序已有数据迁移需求：**
—创建迁移脚本，从Cassandra导出并导入到Cosmos DB
-考虑数据转换需求（UUID到字符串的转换）
-计划Cassandra和Cosmos数据模型之间的任何模式差异

**如果您的应用程序不需要数据播种
—跳过此步骤，继续验证

# # # 11。应用程序配置文件

####为Cosmos配置文件更新application.yml```yaml
spring:
  profiles:
    active: cosmos

---
spring:
  profiles: cosmos

azure:
  cosmos:
    uri: ${COSMOS_URI:https://your-account.documents.azure.com:443/}
    database: ${COSMOS_DATABASE:your-database}
```
##验证步骤

1. **编译检查**:`mvn compile`应该成功，没有错误
2. **测试检查**:`mvn test`应该通过更新的测试用例
3. **运行时检查**：应用程序启动时应该没有版本冲突
4. **连接检查**：应用程序应该连接到Cosmos DB成功
5. **数据检查**:CRUD操作应该通过API工作
6. **UI检查**：前端应该显示来自Cosmos DB的数据

最佳实践1. **ID策略**：始终使用字符串ID而不是uuid为Cosmos DB
2. **分区键**：根据查询模式和数据分布选择分区键
3. **查询设计**：对自定义查询使用@Query注释，而不是方法命名约定
4. 响应式编程：在整个服务层坚持使用Flux/Mono模式
5. **版本管理**：总是包含Spring Boot 2的依赖版本覆盖。x项目
6. **Testing**：更新所有测试文件以使用String id和模拟Cosmos存储库
7. **认证**：使用DefaultAzureCredential进行生产就绪认证

##故障处理命令```bash
# Check dependencies and version conflicts
mvn dependency:tree | grep -E "(reactor|netty|cosmos)"

# Verify specific problematic dependencies
mvn dependency:tree | grep "reactor-core"
mvn dependency:tree | grep "reactor-netty"
mvn dependency:tree | grep "netty-tcnative"

# Test connection
curl http://localhost:8080/api/entities

# Check Azure login status
az account show

# Clean and rebuild (often fixes dependency issues)
mvn clean compile

# Run with debug logging for dependency resolution
mvn dependency:resolve -X

# Check for compilation errors specifically
mvn compile 2>&1 | grep -E "(ERROR|error)"

# Run with debug for runtime issues
mvn spring-boot:run -Dspring-boot.run.jvmArguments="-Xdebug -Xrunjdwp:transport=dt_socket,server=y,suspend=n,address=5005"

# Check application logs for version conflicts
grep -E "(NoSuchMethodError|NoClassDefFoundError|reactor|netty)" application.log
```
典型错误序列和解决方法

根据实际的转换经验，您可能会遇到以下错误：

阶段1：编译错误
1. **缺少依赖项**→添加azure-spring-data-cosmos和azure-identity
2. **配置类错误**→创建CosmosConfiguration（如果不存在）
3. **实体标注错误**→将@Table转换为@Container等。
4. **存储库接口错误**→更改为ReactiveCosmosRepository（如果使用存储库模式）

阶段2:Bean创建错误
5. **“没有ReactiveCosmosRepository类型的合格bean”**→添加@EnableReactiveCosmosRepositories
6. **服务层类型不匹配**→将Iterable更改为Flux，可选更改为Mono（如果使用服务层）阶段3：运行时版本冲突（最复杂）
7. * * NoClassDefFoundError: reactor.core.publisher。水槽**→添加反应堆堆芯3.4.32覆盖
8. * * NoSuchMethodError: Epoll。isTcpFastOpenClientSideAvailable**→新增netty-bom 4.1.101。最终覆盖
9. * * NoSuchMethodError: SSLContext。setCurvesList**→添加netty- tnative -boringssl-static 2.0.62。最终覆盖

### **阶段4：认证和连接
10. **ManagedIdentityCredential鉴权不可用**→执行命令`az login --use-device-code`11. **应用程序启动成功**→连接Cosmos数据库！

**紧急**：按顺序解决这些问题。不要跳过——每个阶段都必须在下一个阶段出现之前解决。

性能考虑1. **分区策略**：设计分区键均匀分配负载
2. **查询优化**：尽可能使用索引并避免跨分区查询
3. **连接池**:Cosmos客户端自动管理连接
4. **请求单元**：监控RU消耗并根据需要调整吞吐量
5. **批量操作**：对多个文档更新使用批量操作

本指南涵盖了从Cassandra转换到Cosmos DB的所有主要方面，包括在实际场景中遇到的所有版本冲突和身份验证问题。