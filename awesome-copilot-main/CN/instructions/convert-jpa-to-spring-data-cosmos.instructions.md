---
description: 'Step-by-step guide for converting Spring Boot JPA applications to use Azure Cosmos DB with Spring Data Cosmos'
applyTo: '**/*.java,**/pom.xml,**/build.gradle,**/application*.properties'
---
将Spring JPA项目转换为Spring Data Cosmos

这个通用指南适用于任何JPA到Spring Data Cosmos DB的转换项目。

##高层次计划

1. 交换构建依赖（删除JPA，添加Cosmos + Identity）。
2. 添加`cosmos`配置文件和属性。
3. 添加具有正确Azure身份验证的Cosmos配置。
4. 转换实体（id→`String`，添加`@Container`和`@PartitionKey`，删除JPA映射，调整关系）。
5. 转换存储库（`JpaRepository`→`CosmosRepository`）。
6. **为关系管理和模板兼容性创建服务层**。
7. **CRITICAL**：更新所有测试文件以使用String id和Cosmos存储库。
8. 通过`CommandLineRunner`播种数据。
9. **CRITICAL**：测试运行时功能并修复模板兼容性问题。

# #循序渐进

###步骤1 -构建依赖关系- **Maven** (`pom.xml`)：
-删除依赖`spring-boot-starter-data-jpa`-删除数据库特定的依赖关系（H2, MySQL, PostgreSQL），除非需要在其他地方
-添加`com.azure:azure-spring-data-cosmos:5.17.0`（或最新兼容版本）
添加`com.azure:azure-identity:1.15.4`（DefaultAzureCredential需要）
- **Gradle**：对Gradle语法应用相同的依赖项更改
删除测试容器和特定于jpa的测试依赖项

###步骤2 -属性和配置

-创建`src/main/resources/application-cosmos.properties`：  ```properties
  azure.cosmos.uri=${COSMOS_URI:https://localhost:8081}
  azure.cosmos.database=${COSMOS_DATABASE:petclinic}
  azure.cosmos.populate-query-metrics=false
  azure.cosmos.enable-multiple-write-locations=false
  ```
-更新`src/main/resources/application.properties`：  ```properties
  spring.profiles.active=cosmos
  ```
###步骤3 -配置类与Azure身份

—创建`src/main/java/<rootpkg>/config/CosmosConfiguration.java`：  ```java
  @Configuration
  @EnableCosmosRepositories(basePackages = "<rootpkg>")
  public class CosmosConfiguration extends AbstractCosmosConfiguration {

    @Value("${azure.cosmos.uri}")
    private String uri;

    @Value("${azure.cosmos.database}")
    private String dbName;

    @Bean
    public CosmosClientBuilder getCosmosClientBuilder() {
      return new CosmosClientBuilder().endpoint(uri).credential(new DefaultAzureCredentialBuilder().build());
    }

    @Override
    protected String getDatabaseName() {
      return dbName;
    }

    @Bean
    public CosmosConfig cosmosConfig() {
      return CosmosConfig.builder().enableQueryMetrics(false).build();
    }
  }

  ```
—**重要**：为了保证生产安全，请使用`DefaultAzureCredentialBuilder().build()`代替密钥认证

###步骤4 -实体转换-针对所有带有JPA注释的类（`@Entity`,`@MappedSuperclass`,`@Embeddable`）
- **基础实体变更**：
—将`id`字段类型从`Integer`更改为`String`-添加`@Id`和`@GeneratedValue`注释
-添加`@PartitionKey`字段（通常是`String partitionKey`）
—删除所有导入的`jakarta.persistence`文件
- **CRITICAL - Cosmos DB序列化要求**：
- **从需要持久化到Cosmos DB的字段中删除所有`@JsonIgnore`注释**
- **认证实体（User, Authority）必须是完全可序列化的** -密码，权限或其他持久化字段不需要`@JsonIgnore`- **使用`@JsonProperty`而不是`@JsonIgnore`**当你需要控制JSON字段名，但仍然保持数据
- **常见的认证序列化错误**:`Cannot pass null or empty values to constructor`通常表示`@JsonIgnore`阻塞了必需字段的序列化
- **实体特定变化**：
—将`@Entity`替换为`@Container(containerName = "<plural-entity-name>")`-删除`@Table`，`@Column`，`@JoinColumn`等．
-删除关系注释（`@OneToMany`,`@ManyToOne`,`@ManyToMany`）
-对于人际关系：    - Embed collections for one-to-many (e.g., `List<Pet> pets` in Owner)
    - Use reference IDs for many-to-one (e.g., `String ownerId` in Pet)
    - **For complex relationships**: Store IDs but add transient properties for templates
—添加构造函数设置分区键：`setPartitionKey("entityType")`- **CRITICAL -认证实体模式**：
- **对于Spring安全的用户实体**：将权限存储为`Set<String>`而不是`Set<Authority>`对象
- **用户实体转换示例**：    ```java
    @Container(containerName = "users")
    public class User {

      @Id
      private String id;

      @PartitionKey
      private String partitionKey = "user";

      private String login;
      private String password; // NO @JsonIgnore - must be serializable

      @JsonProperty("authorities") // Use @JsonProperty, not @JsonIgnore
      private Set<String> authorities = new HashSet<>(); // Store as strings

      // Add transient property for Spring Security compatibility if needed
      // @JsonIgnore - ONLY for transient properties not persisted to Cosmos
      private Set<Authority> authorityObjects = new HashSet<>();

      // Conversion methods between string authorities and Authority objects
      public void setAuthorityObjects(Set<Authority> authorities) {
        this.authorityObjects = authorities;
        this.authorities = authorities.stream().map(Authority::getName).collect(Collectors.toSet());
      }
    }

    ```
- **关键-模板兼容性的关系变化**：
- **将关系转换为ID引用时，保留模板访问**
- **示例**：如果实体有`List<Specialty> specialties`→转换为：    - Storage: `List<String> specialtyIds` (persisted to Cosmos)
    - Template: `@JsonIgnore private List<Specialty> specialties = new ArrayList<>()` (transient)
    - Add getters/setters for both properties
- **更新实体方法逻辑**:`getNrOfSpecialties()`应该使用瞬态列表
- **关键-Thymeleaf/JSP应用程序的模板兼容性**：
- **识别模板属性访问**：在`.html`文件中搜索`${entity.relationshipProperty}`- **对于在模板中访问的每个关系属性**：    - **Storage**: Keep ID-based storage (e.g., `List<String> specialtyIds`)
    - **Template Access**: Add transient property with `@JsonIgnore` (e.g., `private List<Specialty> specialties = new ArrayList<>()`)
    - **Example**:

      ```java
      // Stored in Cosmos (persisted)
      private List<String> specialtyIds = new ArrayList<>();

      // For template access (transient)
      @JsonIgnore
      private List<Specialty> specialties = new ArrayList<>();

      // Getters/setters for both properties
      public List<String> getSpecialtyIds() {
        return specialtyIds;
      }

      public List<Specialty> getSpecialties() {
        return specialties;
      }

      ```

    - **Update count methods**: `getNrOfSpecialties()` should use transient list, not ID list
- **CRITICAL -方法签名冲突**：
—**当ID类型从Integer转换为String时，检查方法签名是否冲突**
- **常见冲突**:`getPet(String name)`与`getPet(String id)`-两者具有相同的签名
- **解决方案**：将方法重命名为具体的：    - `getPet(String id)` for ID-based lookup
    - `getPetByName(String name)` for name-based lookup
    - `getPetByName(String name, boolean ignoreNew)` for conditional name-based lookup
**更新控制器和测试中重命名方法的所有调用者**
- **实体的方法更新**：
—将`addVisit(Integer petId, Visit visit)`更新为`addVisit(String petId, Visit visit)`—确保所有ID比较逻辑使用`.equals()`，而不是`==`###步骤5 -存储库转换

—修改所有存储库接口：
-出自：`extends JpaRepository<Entity, Integer>`—To:`extends CosmosRepository<Entity, String>`- **查询方法更新**：
-从自定义查询中删除分页参数
—将“`Page<Entity> findByX(String param, Pageable pageable)`”修改为“`List<Entity> findByX(String param)`”
-更新`@Query`注释使用Cosmos SQL语法
- **替换自定义方法名**:`findPetTypes()`→`findAllOrderByName()`- **更新所有引用**到控制器和格式化器中更改的方法名

###步骤6 - **创建服务层**用于关系管理和模板兼容性- **CRITICAL**：创建服务类，将Cosmos文档存储与现有模板期望连接起来
- **用途**：处理关系填充和维护模板兼容性
- **具有关系**的每个实体的服务模式：  ```java
  @Service
  public class EntityService {

    private final EntityRepository entityRepository;
    private final RelatedRepository relatedRepository;

    public EntityService(EntityRepository entityRepository, RelatedRepository relatedRepository) {
      this.entityRepository = entityRepository;
      this.relatedRepository = relatedRepository;
    }

    public List<Entity> findAll() {
      List<Entity> entities = entityRepository.findAll();
      entities.forEach(this::populateRelationships);
      return entities;
    }

    public Optional<Entity> findById(String id) {
      Optional<Entity> entityOpt = entityRepository.findById(id);
      if (entityOpt.isPresent()) {
        Entity entity = entityOpt.get();
        populateRelationships(entity);
        return Optional.of(entity);
      }
      return Optional.empty();
    }

    private void populateRelationships(Entity entity) {
      if (entity.getRelatedIds() != null && !entity.getRelatedIds().isEmpty()) {
        List<Related> related = entity
          .getRelatedIds()
          .stream()
          .map(relatedRepository::findById)
          .filter(Optional::isPresent)
          .map(Optional::get)
          .collect(Collectors.toList());
        // Set transient property for template access
        entity.setRelated(related);
      }
    }
  }

  ```
###步骤6.5 - **Spring安全集成**（对认证至关重要）

- **UserDetailsService集成模式**：  ```java
  @Service
  @Transactional
  public class DomainUserDetailsService implements UserDetailsService {

    private final UserRepository userRepository;
    private final AuthorityRepository authorityRepository;

    @Override
    public UserDetails loadUserByUsername(String login) {
      log.debug("Authenticating user: {}", login);

      return userRepository
        .findOneByLogin(login)
        .map(user -> createSpringSecurityUser(login, user))
        .orElseThrow(() -> new UsernameNotFoundException("User " + login + " was not found"));
    }

    private org.springframework.security.core.userdetails.User createSpringSecurityUser(String lowercaseLogin, User user) {
      if (!user.isActivated()) {
        throw new UserNotActivatedException("User " + lowercaseLogin + " was not activated");
      }

      // Convert string authorities back to GrantedAuthority objects
      List<GrantedAuthority> grantedAuthorities = user
        .getAuthorities()
        .stream()
        .map(SimpleGrantedAuthority::new)
        .collect(Collectors.toList());

      return new org.springframework.security.core.userdetails.User(user.getLogin(), user.getPassword(), grantedAuthorities);
    }
  }

  ```
- **密钥认证要求**：
-用户实体必须是完全可序列化的（password/authorities上没有`@JsonIgnore`）
为了兼容Cosmos DB，将权限存储为`Set<String>`在UserDetailsService中转换字符串权限和`GrantedAuthority`对象
—增加综合调试日志，跟踪认证流程
-适当处理activated/deactivated用户状态

#### **模板关系人口模式**

每个为模板渲染返回实体的服务方法必须填充瞬态属性：```java
private void populateRelationships(Entity entity) {
  // For each relationship used in templates
  if (entity.getRelatedIds() != null && !entity.getRelatedIds().isEmpty()) {
    List<Related> relatedObjects = entity
      .getRelatedIds()
      .stream()
      .map(relatedRepository::findById)
      .filter(Optional::isPresent)
      .map(Optional::get)
      .collect(Collectors.toList());
    entity.setRelated(relatedObjects); // Set transient property
  }
}

```
#### **控制器关键业务占用率**

—**将所有直接存储库调用**替换为控制器中的服务调用
- **不要在没有关系填充的情况下直接从存储库返回实体**到模板
- **更新控制器**以直接使用服务层而不是存储库
- **控制器模式更改**：  ```java
  // OLD: Direct repository usage
  @Autowired
  private EntityRepository entityRepository;

  // NEW: Service layer usage
  @Autowired
  private EntityService entityService;
  // Update method calls
  // OLD: entityRepository.findAll()
  // NEW: entityService.findAll()

  ```
###步骤7 -数据播种

—创建`@Component`实现`CommandLineRunner`：  ```java
  @Component
  public class DataSeeder implements CommandLineRunner {

    @Override
    public void run(String... args) throws Exception {
      if (ownerRepository.count() > 0) {
        return; // Data already exists
      }
      // Seed comprehensive test data with String IDs
      // Use meaningful ID patterns: "owner-1", "pet-1", "pettype-1", etc.
    }
  }

  ```
- **CRITICAL - JDK 17+的BigDecimal反射问题：
—**如果使用BigDecimal字段**，可能会在播种时出现反射错误
-错误模式**:`Unable to make field private final java.math.BigInteger java.math.BigDecimal.intVal accessible`- * * * *解决方案:    1. Use `Double` or `String` instead of `BigDecimal` for monetary values
    2. Add JVM argument: `--add-opens java.base/java.math=ALL-UNNAMED`
    3. Wrap BigDecimal operations in try-catch and handle gracefully
- **即使播种失败，应用程序也会成功启动** -检查播种错误日志

###步骤8 -测试文件转换（临界部分）

**这一步经常被忽视，但对成功转换至关重要**

#### a . **编译检查策略**

- **每次重大变更后，执行`mvn test-compile`命令，尽早发现问题**
- **在继续**之前，系统地修复编译错误
- **不要依赖IDE - Maven编译会揭示所有问题**

#### B. **系统地搜索和更新所有测试文件**

**使用搜索工具查找和更新每个事件：**

—搜索：`int.*TEST.*ID`→替换为：`String.*TEST.*ID = "test-xyz-1"`—搜索：`setId\(\d+\)`→替换为：`setId("test-id-X")`—搜索：`findById\(\d+\)`→替换为：`findById("test-id-X")`—搜索：`\.findPetTypes\(\)`→替换为：`.findAllOrderByName()`—搜索：`\.findByLastNameStartingWith\(.*,.*Pageable`→删除分页参数

####更新测试注释和导入-将`@DataJpaTest`替换为`@SpringBootTest`或适当的切片测试
-删除`@AutoConfigureTestDatabase`注释
-从测试中删除`@Transactional`（除非是单分区操作）
—删除`org.springframework.orm`包中的导入

####修复所有测试文件中实体ID的使用

**必须更新的关键文件（搜索整个测试目录）：**

-`*ControllerTests.java`-路径变量，实体创建，模拟设置
-`*ServiceTests.java`-存储库交互，实体id
-`EntityUtils.java`- ID处理的实用方法
-`*FormatterTests.java`-存储库方法调用
-`*ValidatorTests.java`-使用字符串id创建实体
-集成测试类-测试数据设置

#### E. **修复受存储库更改影响的控制器和服务类**- **更新调用具有更改签名的存储库方法的控制器**
- **更新使用存储库方法**的formatters/converters- **常用检查文件**：
-`PetTypeFormatter.java`-经常调用`findPetTypes()`方法
-`*Controller.java`-可能有分页逻辑要删除
—使用存储库方法的服务类

#### F.在测试中更新存储库模拟

-从存储库模拟中删除分页：
——`given(repository.findByX(param, pageable)).willReturn(pageResult)`-→`given(repository.findByX(param)).willReturn(listResult)`-更新模拟中的方法名：
——`given(petTypeRepository.findPetTypes()).willReturn(types)`-→`given(petTypeRepository.findAllOrderByName()).willReturn(types)`修复测试使用的实用工具类

-更新`EntityUtils.java`或类似版本：
-删除jpa特定的异常导入（`ObjectRetrievalFailureException`）
—将方法签名从`int id`修改为`String id`—更新ID比较逻辑：`entity.getId() == entityId`→`entity.getId().equals(entityId)`-用标准异常替换JPA异常（`IllegalArgumentException`）

####更新字符串id的断言-更改ID断言：
-`assertThat(entity.getId()).isNotZero()`→`assertThat(entity.getId()).isNotEmpty()`-`assertThat(entity.getId()).isEqualTo(1)`→`assertThat(entity.getId()).isEqualTo("test-id-1")`- JSON路径断言：`jsonPath("$.id").value(1)`→`jsonPath("$.id").value("test-id-1")`###步骤8 -测试文件转换（临界部分）

**这一步经常被忽视，但对成功转换至关重要**

#### a . **编译检查策略**

- **每次重大变更后，运行`mvn test-compile`，尽早发现问题**
- **在继续**之前，系统地修复编译错误
- **不要依赖IDE - Maven编译会揭示所有问题**

#### B. **系统地搜索和更新所有测试文件**

**使用搜索工具查找和更新每个事件：**

—搜索：`setId\(\d+\)`→替换为：`setId("test-id-X")`—搜索：`findById\(\d+\)`→替换为：`findById("test-id-X")`—搜索：`\.findPetTypes\(\)`→替换为：`.findAllOrderByName()`—搜索：`\.findByLastNameStartingWith\(.*,.*Pageable`→移除分页参数

####更新测试注释和导入-将`@DataJpaTest`替换为`@SpringBootTest`或适当的切片测试
-删除`@AutoConfigureTestDatabase`注释
-从测试中删除`@Transactional`（除非是单分区操作）
—删除`org.springframework.orm`包中的导入

####修复所有测试文件中实体ID的使用

**必须更新的关键文件（搜索整个测试目录）：**

-`*ControllerTests.java`-路径变量，实体创建，模拟设置
-`*ServiceTests.java`-存储库交互，实体id
-`EntityUtils.java`- ID处理的实用方法
-`*FormatterTests.java`-存储库方法调用
—`*ValidatorTests.java`—使用字符串id创建实体
-集成测试类-测试数据设置

#### E. **修复受存储库更改影响的控制器和服务类**- **更新调用具有更改签名的存储库方法的控制器**
- **更新使用存储库方法**的formatters/converters- **常用检查文件**：
-`PetTypeFormatter.java`-经常调用`findPetTypes()`方法
-`*Controller.java`-可能有分页逻辑要删除
—使用存储库方法的服务类

#### F.在测试中更新存储库模拟

-从存储库模拟中删除分页：
——`given(repository.findByX(param, pageable)).willReturn(pageResult)`-→`given(repository.findByX(param)).willReturn(listResult)`-更新模拟中的方法名：
——`given(petTypeRepository.findPetTypes()).willReturn(types)`-→`given(petTypeRepository.findAllOrderByName()).willReturn(types)`修复测试使用的实用工具类

-更新`EntityUtils.java`或类似版本：
-删除jpa特定的异常导入（`ObjectRetrievalFailureException`）
—将方法签名从`int id`修改为`String id`-更新ID比较逻辑：`entity.getId() == entityId`→`entity.getId().equals(entityId)`-用标准异常替换JPA异常（`IllegalArgumentException`）

####更新字符串id的断言-更改ID断言：
-`assertThat(entity.getId()).isNotZero()`→`assertThat(entity.getId()).isNotEmpty()`-`assertThat(entity.getId()).isEqualTo(1)`→`assertThat(entity.getId()).isEqualTo("test-id-1")`- JSON路径断言：`jsonPath("$.id").value(1)`→`jsonPath("$.id").value("test-id-1")`###步骤9 - **运行时测试和模板兼容性**

#### **CRITICAL**：编译成功后测试正在运行的应用程序

- **启动应用程序**:`mvn spring-boot:run`- **浏览web界面中的所有页面**，以识别运行时错误
- **转换后常见的运行时问题**：
-模板试图访问不再存在的属性（例如，`vet.specialties`）
-服务层不填充瞬态关系属性
—不使用服务层加载关系的控制器

#### **模板兼容性修复**：如果模板访问关系属性**（例如，`entity.relatedObjects`）：
-确保具有适当getters/setters的实体存在瞬态属性
-验证服务层填充这些瞬态属性
-更新`getNrOfXXX()`方法使用瞬态列表而不是ID列表
- **检查日志中的SpEL （Spring Expression Language）错误**：
-`Property or field 'xxx' cannot be found`→添加缺失的瞬态属性
-`EL1008E`错误→服务层未填充关系

#### **业务层验证**：

—**确保所有控制器使用业务层**，而不是直接访问存储库
在返回实体之前验证服务方法填充关系
- **测试所有CRUD操作**通过web界面

###步骤9.5 - **模板运行时验证**（关键）

#### **系统模板测试流程**

编译成功并启动应用程序后：1. **系统地导航到应用程序中的每个页面**
2. **测试显示实体数据的每个模板：
-列出页面（如：`/vets`、`/owners`）
-详细页面（如：`/owners/{id}`、`/vets/{id}`）
-表单和编辑页面
3. **查找特定模板错误**：
——`Property or field 'relationshipName' cannot be found on object of type 'EntityName'`-`EL1008E`Spring表达式语言错误
-应该显示关系的数据为空或缺失

#### **模板错误解决清单**

遇到模板错误时：

-[] **从错误信息中识别缺失的属性**
-[] **检查属性是否在实体中作为瞬态字段**存在
-[] **在返回实体之前验证服务层是否填充了属性**
-[] **确保控制器使用业务层，而不是直接访问存储库
-[] **修复后再次测试特定页面

#### **常见模板错误模式**-`Property or field 'specialties' cannot be found`→添加`@JsonIgnore private List<Specialty> specialties`到Vet实体
-`Property or field 'pets' cannot be found`→添加`@JsonIgnore private List<Pet> pets`到Owner实体
—显示空关系数据→服务不填充瞬态属性

###步骤10 - **系统错误解决过程**

####编译失败时：

1. **先运行`mvn compile`** -在测试前修复主要源代码问题
2. **运行`mvn test-compile`** -系统修复每个测试编译错误
3. **关注最常见的错误模式**：
-`int cannot be converted to String`→更改测试常量和实体设置
-`method X cannot be applied to given types`→删除分页参数
-`cannot find symbol: method Y()`→更新到新的存储库方法名称
—方法签名冲突→重命名冲突的方法

###步骤10 - **系统错误解决过程**

####编译失败时：1. **先运行`mvn compile`** -在测试前修复主要源代码问题
2. **运行`mvn test-compile`** -系统修复每个测试编译错误
3. **关注最常见的错误模式**：
-`int cannot be converted to String`→更改测试常量和实体设置
-`method X cannot be applied to given types`→删除分页参数
-`cannot find symbol: method Y()`→更新到新的存储库方法名称
—方法签名冲突→重命名冲突的方法
####运行失败时：

1. **检查应用程序日志**查看特定的错误信息
2. **查找template/SpEL错误**：
-`Property or field 'xxx' cannot be found`→为实体添加瞬态属性
-缺失关系数据→服务层没有填充关系
3. **验证控制器中服务层的使用情况
4. **测试所有应用页面的导航**

####常见错误模式及解决方法：- **`method findByLastNameStartingWith cannot be applied`**→删除`Pageable`参数
- **`cannot find symbol: method findPetTypes()`**→修改为`findAllOrderByName()`- **`incompatible types: int cannot be converted to String`**→更新测试ID常量
- **`method getPet(String) is already defined`**→重命名一个方法（例如，`getPetByName`）
- **`cannot find symbol: method isNotZero()`**→字符串id改为`isNotEmpty()`- **`Property or field 'specialties' cannot be found`**→添加暂态属性并填充在服务中
- **`ClassCastException: reactor.core.publisher.BlockingIterable cannot be cast to java.util.List`**→修复库`findAllWithEagerRelationships()`方法使用StreamSupport
- **`Unable to make field...BigDecimal.intVal accessible`**→将BigDecimal替换为Double
- **健康检查数据库失败**→从健康检查准备配置中删除‘db’

#### **特定于模板的运行时错误**

- * *`Property or field 'XXX' cannot be found on object of type 'YYY'`* *:

—根本原因：模板访问关系属性转换为ID存储
—解决方案：在实体中添加瞬态属性，在业务层填充
—预防：在转换关系之前，始终检查模板的使用情况

- **`EL1008E`Spring表达式语言错误**：—根本原因：业务层未填充瞬态属性
—解决方法：验证`populateRelationships()`方法是否被调用并正常工作
—预防：业务层实现后，测试所有模板导航

- **模板中的Empty/null关系数据**：
—根本原因：控制器绕过业务层或服务不填充关系
—解决方案：确保所有控制器方法都使用服务层进行实体检索
—预防：永远不要将存储库结果直接返回给模板

###步骤11 -验证检查表

转换后，验证：-[] **主应用编译**:`mvn compile`成功
-[] **所有测试文件编译**:`mvn test-compile`成功
-[] **无编译错误**：解决每个编译错误
-[] **应用程序启动成功**:`mvn spring-boot:run`无错误
-[] **所有网页加载**：浏览所有应用程序页面没有运行时错误
-[] **服务层填充关系**：正确设置瞬态属性
-[] **所有模板页面呈现无错误**：浏览整个应用程序
-[] **关系数据正确显示**：列表、计数和相关对象正确显示
-[] **日志中无SpEL模板错误**：导航时查看应用日志
-[] **暂态属性是@JsonIgnore注释**：防止JSON序列化问题
-[] **一致使用的服务层**：不直接访问控制器中的存储库以进行模板渲染—[]没有剩余的`jakarta.persistence`导入
—[]所有实体id一致为`String`类型
—[]所有存储库接口都扩展为`CosmosRepository<Entity, String>`—[]配置使用`DefaultAzureCredential`鉴权
-[]数据播种组件存在并工作
—[]测试文件一致使用String id
[]更新了Cosmos方法的存储库模拟
-[]实体类中没有方法签名冲突
-[] **在调用者（控制器、测试、格式化器）中更新了所有重命名的方法**要避免的常见陷阱1. **不经常检查编译** -在每次重大更改后运行`mvn test-compile`2. **方法签名冲突** -转换ID类型时方法重载问题
3. **忘记更新方法调用者** -当重命名方法时，更新所有调用者
4. **缺少存储库方法重命名** -自定义存储库方法必须在任何调用处更新
5. **使用基于密钥的身份验证** -使用`DefaultAzureCredential`代替
6. **混合整数和字符串id ** -在任何地方保持字符串id一致，特别是在测试中
7. **不更新控制器分页逻辑** -当存储库更改时从控制器中删除分页
8. **留下特定于jpa的测试注释** -替换为兼容cosmos的替代品
9. **不完整的测试文件更新** -搜索整个测试目录，而不仅仅是明显的文件
10. **跳过运行时测试** -始终测试正在运行的应用程序，而不仅仅是比较ilation
11. **缺少服务层** -不要直接从控制器访问存储库
12. **忘记瞬态属性** -模板可能需要访问关系数据
13. **不测试模板导航** -编译成功并不意味着模板工作
14. **模板缺少瞬态属性** -模板需要对象访问，而不仅仅是id
15. **绕过服务层** -控制器必须使用服务，绝不直接访问存储库
16. **不完全关系填充** -服务方法必须填充模板使用的所有瞬态属性
17. **在瞬态属性上忘记@JsonIgnore ** -防止序列化问题
18. **@JsonIgnore对持久化字段** - **CRITICAL**：永远不要对需要存储在Cosmos DB中的字段使用`@JsonIgnore`19. **认证序列化错误** -User/Authority实体必须是完全可序列化的，没有`@JsonIgnore`阻塞必填字段
20.。**BigDecimal反射问题** -使用替代数据类型或JVM参数来兼容JDK 17+
21. **存储库响应式类型转换** -不要直接将`findAll()`转换为`List`，使用`StreamSupport.stream().collect(Collectors.toList())`22. **健康检查数据库引用** -删除JPA后，从Spring Boot健康检查中删除数据库依赖项
23. **集合类型不匹配** -更新服务方法以一致地处理字符串和对象集合系统地调试编译问题

如果转换后编译失败：

1. **从主编译开始**:`mvn compile`-首先修复实体和控制器问题
2. **然后测试编译**:`mvn test-compile`-系统修复每个错误
3. **检查整个代码库中剩余的`jakarta.persistence`导入**
4. **验证所有的测试常数使用字符串id ** -搜索`int.*TEST.*ID`5. **确保存储库方法签名匹配**新的Cosmos接口
6. **在实体关系和测试中检查混合Integer/StringID使用**
7. **验证所有mock使用正确的方法名** （`findAllOrderByName()`而不是`findPetTypes()`）
8. **查找方法签名冲突** -通过重命名冲突的方法解决
9. **验证断言方法工作与字符串id ** （`isNotEmpty()`不是`isNotZero()`）

系统地调试运行时问题

如果编译成功后运行时失败：1. **检查应用程序启动日志**是否有初始化错误
2. **浏览所有页面**以识别template/controller问题
3. **在日志中查找拼写模板错误**：
-`Property or field 'xxx' cannot be found`→缺少瞬态属性`EL1008E`→服务层不填充关系
4. **验证正在使用服务层**而不是直接访问存储库
5. **检查服务方法中是否填充了瞬态属性
6. **测试所有CRUD操作**通过web界面
7. **验证数据播种工作正确**和关系维护
8. * * Authentication-specific调试* *:
-`Cannot pass null or empty values to constructor`→检查所需字段的`@JsonIgnore`-`BadCredentialsException`→验证用户实体序列化和密码字段可访问性
—查看“DomainUserDetailsService”调试输出日志，跟踪鉴权流程

### **成功秘诀****不要让错误累积
- **使用全局搜索和替换** -查找所有出现的模式进行更新
- **系统** -在移动到下一个文件之前修复所有文件中的一种错误
- **仔细测试方法重命名** -确保所有调用者都更新了
- **使用有意义的字符串id ** -“owner-1”，“pet-1”代替随机字符串
- **检查控制器类** -它们经常调用更改签名的存储库方法
- **始终测试运行时** -编译成功并不能保证功能模板
-服务层是关键** -文档存储和模板期望之间的桥梁

### **认证故障排除指南**（紧急）

#### **常见认证序列化错误**：

1. * *`Cannot pass null or empty values to constructor`* *:- **根本原因**:`@JsonIgnore`阻止必要的字段序列化到Cosmos DB
**解决方案**：从所有持久化字段（密码，权限等）中删除`@JsonIgnore`—**校验**：检查用户实体存储字段中没有`@JsonIgnore`2. **`BadCredentialsException`登录时**：

—**根本原因**：鉴权时密码字段不可访问
- **解决方案**：确保密码字段在UserDetailsService中可序列化和访问
—**验证**：以`loadUserByUsername`方式添加调试日志

3. **权限无法正确加载**：

- **根本原因**：权限对象存储为复杂实体而不是字符串
- **解决方案**：存储权限为`Set<String>`，并在UserDetailsService中转换为`GrantedAuthority`* * - * *模式:     ```java
     // In User entity - stored in Cosmos
     @JsonProperty("authorities")
     private Set<String> authorities = new HashSet<>();

     // In UserDetailsService - convert for Spring Security
     List<GrantedAuthority> grantedAuthorities = user
       .getAuthorities()
       .stream()
       .map(SimpleGrantedAuthority::new)
       .collect(Collectors.toList());

     ```
4. **认证时找不到用户实体**：
—**根本原因**：存储库查询方法不支持String id
- **解决方案**：更新存储库`findOneByLogin`方法以与Cosmos DB一起工作
- **验证**：独立测试存储库方法

#### **认证调试清单**：

-[]用户实体完全可序列化（没有`@JsonIgnore`对持久化字段）
-[]密码字段可访问且不为空
-[]权限存储为`Set<String>`—[]UserDetailsService将字符串权限转换为`GrantedAuthority`—[]存储库方法使用String id
-[]启用鉴权服务调试日志
-[]用户激活状态检查是否正确
-[]测试登录已知凭据（admin/admin）

### **常见运行时问题和解决方案**

#### **问题1：存储库响应式类型转换错误**

* *误差* *:`ClassCastException: reactor.core.publisher.BlockingIterable cannot be cast to java.util.List`**根本原因**:Cosmos存储库返回响应类型（`Iterable`），但遗留JPA代码期望`List`**解决方案**：在存储库方法中正确转换响应类型：```java
// WRONG - Direct casting fails
default List<Entity> customFindMethod() {
    return (List<Entity>) this.findAll(); // ClassCastException!
}

// CORRECT - Convert Iterable to List
default List<Entity> customFindMethod() {
    return StreamSupport.stream(this.findAll().spliterator(), false)
            .collect(Collectors.toList());
}
```
**要检查的文件**：

—所有具有自定义默认方法的存储库接口
-任何从Cosmos存储库调用返回`List<Entity>`的方法
—导入“`java.util.stream.StreamSupport`”和“`java.util.stream.Collectors`”

#### **问题2:Java 17+中的BigDecimal反射问题

* *误差* *:`Unable to make field private final java.math.BigInteger java.math.BigDecimal.intVal accessible`根本原因：Java 17+模块系统在序列化过程中限制了对BigDecimal内部字段的反射访问

* * * *解决方案:

1. **用Double代替简单的情况**：   ```java
   // Before: BigDecimal fields
   private BigDecimal amount;

   // After: Double fields (if precision requirements allow)
   private Double amount;

   ```
2. **使用字符串的高精度要求**：   ```java
   // Store as String, convert as needed
   private String amount; // Store "1500.00"

   public BigDecimal getAmountAsBigDecimal() {
     return new BigDecimal(amount);
   }

   ```
3. **添加JVM参数**（如果必须保留BigDecimal）：   ```
   --add-opens java.base/java.math=ALL-UNNAMED
   ```
#### **问题3：健康检查数据库依赖项**

**错误**：应用程序无法通过健康检查查找已删除的数据库组件

**根本原因**：删除后，Spring Boot健康检查仍然引用JPA/database依赖项

解决方案**：更新健康检查配置：```yaml
# In application.yml - Remove database from health checks
management:
  health:
    readiness:
      include: 'ping,diskSpace' # Remove 'db' if present
```
**要检查的文件**：

—所有`application*.yml`配置文件
-删除任何特定于数据库的运行状况指标
—检查执行器端点配置

#### **问题4：服务中的集合类型不匹配**

**错误**：将实体关系转换为基于字符串的存储时出现类型不匹配错误

**根本原因**：服务方法在实体转换后期望不同的集合类型

解决方案**：更新服务方法来处理新的实体结构：```java
// Before: Entity relationships
public Set<RelatedEntity> getRelatedEntities() {
    return entity.getRelatedEntities(); // Direct entity references
}

// After: String-based relationships with conversion
public Set<RelatedEntity> getRelatedEntities() {
    return entity.getRelatedEntityIds()
        .stream()
        .map(relatedRepository::findById)
        .filter(Optional::isPresent)
        .map(Optional::get)
        .collect(Collectors.toSet());
}

### **Enhanced Error Resolution Process**

#### **Common Error Patterns and Solutions**:

1. **Reactive Type Casting Errors**:
   - **Pattern**: `cannot be cast to java.util.List`
   - **Fix**: Use `StreamSupport.stream().collect(Collectors.toList())`
   - **Files**: Repository interfaces with custom default methods

2. **BigDecimal Serialization Errors**:
   - **Pattern**: `Unable to make field...BigDecimal.intVal accessible`
   - **Fix**: Replace with Double, String, or add JVM module opens
   - **Files**: Entity classes, DTOs, data initialization classes

3. **Health Check Database Errors**:
   - **Pattern**: Health check fails looking for database
   - **Fix**: Remove database references from health check configuration
   - **Files**: application.yml configuration files

4. **Collection Type Conversion Errors**:
   - **Pattern**: Type mismatch in entity relationship handling
   - **Fix**: Update service methods to handle String-based entity references
   - **Files**: Service classes, DTOs, entity relationship methods

#### **Enhanced Validation Checklist**:
- [ ] **Repository reactive casting handled**: No ClassCastException on collection returns
- [ ] **BigDecimal compatibility resolved**: Java 17+ serialization works
- [ ] **Health checks updated**: No database dependencies in health configuration
- [ ] **Service layer collection handling**: String-based entity references work correctly
- [ ] **Data seeding completes**: "Data seeding completed" message appears in logs
- [ ] **Application starts fully**: Both frontend and backend accessible
- [ ] **Authentication works**: Can sign in without serialization errors
- [ ] **CRUD operations functional**: All entity operations work through UI

## **Quick Reference: Common Post-Migration Fixes**

### **Top Runtime Issues to Check**

1. **Repository Collection Casting**:
   ```java
//修复任何返回集合的存储库方法：
默认List<Entity>customFindMethod() {       return StreamSupport.stream(this.findAll().spliterator(), false)
               .collect(Collectors.toList());
}

2. **BigDecimal兼容性(Java 17+)**：   ```java
   // Replace BigDecimal fields with alternatives:
   private Double amount; // Or String for high precision

   ```
3. **健康检查配置**：   ```yaml
   # Remove database dependencies from health checks:
   management:
     health:
       readiness:
         include: 'ping,diskSpace'
   ```
### **认证转换模式

- **从需要Cosmos DB持久化的字段中删除`@JsonIgnore`**将复杂对象存储为简单类型**（例如，将权威存储为`Set<String>`）
- **在service/repository层之间转换简单和复杂类型**Template/UI兼容模式**

- **添加瞬态属性**与`@JsonIgnore`UI访问相关数据
- **在渲染前使用服务层**填充瞬态关系
- **在没有关系填充的情况下，永远不要将存储库结果直接**返回给模板