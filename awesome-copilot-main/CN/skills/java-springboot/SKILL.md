---
name: java-springboot
description: 'Get best practices for developing applications with Spring Boot.'
---
# Spring Boot最佳实践

您的目标是通过遵循已建立的最佳实践来帮助我编写高质量的Spring Boot应用程序。

##项目设置和结构

- **构建工具：**使用Maven （`pom.xml`）或Gradle （`build.gradle`）进行依赖管理。
- **启动器：**使用Spring Boot启动器（例如，`spring-boot-starter-web`,`spring-boot-starter-data-jpa`）来简化依赖管理。
**包结构：**按feature/domain组织代码（例如，`com.example.app.order`,`com.example.app.user`）而不是按层（例如，`com.example.app.controller`,`com.example.app.service`）。

依赖注入和组件

- **构造函数注入：**总是对需要的依赖使用基于构造函数的注入。这使得组件更容易测试，依赖关系更明确。
**声明依赖字段为`private final`。
**组件原型：**使用`@Component`、`@Service`、`@Repository`和`@Controller`/`@RestController`注解来定义bean。

# #配置- **外化配置：**使用`application.yml`（或`application.properties`）进行配置。YAML通常因其可读性和分层结构而更受青睐。
**使用`@ConfigurationProperties`将配置绑定到强类型Java对象。
—**配置文件：**使用Spring配置文件（`application-dev.yml`,`application-prod.yml`）来管理特定于环境的配置。
- **机密管理：**不要对机密进行硬编码。使用环境变量，或专用的秘密管理工具，如HashiCorp Vault或AWS秘密管理器。

Web层（控制器）- **RESTful api:**设计清晰一致的RESTful端点。
- ** dto（数据传输对象）：**使用dto在API层公开和消费数据。不要直接向客户端公开JPA实体。
- **验证：**在dto上使用Java Bean验证（JSR 380）和注释（`@Valid`,`@NotNull`,`@Size`）来验证请求有效负载。
- **错误处理：**实现一个全局异常处理程序使用`@ControllerAdvice`和`@ExceptionHandler`提供一致的错误响应。

##服务层

**业务逻辑：**将所有业务逻辑封装在`@Service`类中。
—无状态：**服务应该是无状态的。
- **事务管理：**在服务方法上使用`@Transactional`来声明式地管理数据库事务。在必要的最细粒度级别上应用它。

数据层（存储库）- **Spring Data JPA:**通过扩展`JpaRepository`或`CrudRepository`来使用Spring Data JPA存储库进行标准数据库操作。
- **自定义查询：**对于复杂的查询，使用`@Query`或JPA Criteria API。
- **投影：**使用DTO投影只从数据库中获取必要的数据。

# #日志

- **SLF4J:**使用SLF4J API进行日志记录。
- **记录器声明：**`private static final Logger logger = LoggerFactory.getLogger(MyClass.class);`- **参数化日志：**使用参数化消息（`logger.info("Processing user {}...", userId);`）代替字符串连接来提高性能。

# #测试- **单元测试：**使用JUnit 5和mock框架（如Mockito）为服务和组件编写单元测试。
- **集成测试：**使用`@SpringBootTest`进行加载Spring应用程序上下文的集成测试。
**测试片：**使用`@WebMvcTest`（用于控制器）或`@DataJpaTest`（用于存储库）这样的测试片注释来隔离测试应用程序的特定部分。
Testcontainers：考虑使用testcontainer与真实的数据库、消息代理等进行可靠的集成测试。

# #安全

—**Spring Security:**使用Spring Security进行鉴权授权。
- **密码编码：**始终使用BCrypt等强哈希算法编码密码。
- **输入消毒：**使用Spring Data JPA或参数化查询防止SQL注入。通过正确编码输出来防止跨站点脚本（XSS）。