---
name: kotlin-springboot
description: 'Get best practices for developing applications with Spring Boot and Kotlin.'
---
# Spring Boot with Kotlin最佳实践

您的目标是帮助我使用Kotlin编写高质量的、惯用的Spring Boot应用程序。

##项目设置和结构

**使用Maven （`pom.xml`）或Gradle （`build.gradle`）与Kotlin插件（`kotlin-maven-plugin`或`org.jetbrains.kotlin.jvm`）。
- **Kotlin插件：**对于JPA，启用`kotlin-jpa`插件自动生成实体类`open`没有样板。
—**启动器：**使用Spring Boot启动器（如`spring-boot-starter-web`，`spring-boot-starter-data-jpa`）。
- **包结构：**按feature/domain组织代码（例如，`com.example.app.order`,`com.example.app.user`）而不是按层组织。

依赖注入和组件- **主构造函数：**总是使用主构造函数进行所需的依赖注入。这是Kotlin中最地道、最简洁的方法。
**在主构造函数中声明依赖关系为`private val`。在任何地方使用`val`而不是`var`来提高不变性。
**组件原型：**使用`@Service`，`@Repository`和`@RestController`注释，就像你在Java中一样。

# #配置

- **外部化配置：**使用`application.yml`的可读性和层次结构。
**使用`@ConfigurationProperties`和`data class`来创建不可变的，类型安全的配置对象。
—**配置文件：**使用Spring配置文件（`application-dev.yml`,`application-prod.yml`）来管理特定于环境的配置。
- **秘密管理：**永远不要硬编码秘密。使用环境变量或专用的秘密管理工具，如HashiCorp Vault或AWS秘密管理器。Web层（控制器）

- **RESTful api:**设计清晰一致的RESTful端点。
- ** dto的数据类：**使用Kotlin`data class`为所有dto。这免费提供了`equals()`、`hashCode()`、`toString()`和`copy()`，并提高了不变性。
- **验证：**使用Java Bean验证（JSR 380）与注释（`@Valid`,`@NotNull`,`@Size`）在你的DTO数据类。
- **错误处理：**实现一个全局异常处理程序使用`@ControllerAdvice`和`@ExceptionHandler`一致的错误响应。

##服务层

**业务逻辑：**将业务逻辑封装在`@Service`类中。
—无状态：**服务应该是无状态的。
- **事务管理：**服务方法使用`@Transactional`。在Kotlin中，这可以应用于类或函数级别。

数据层（存储库）- **JPA实体：**将实体定义为类。记住它们一定是`open`。强烈建议使用`kotlin-jpa`编译器插件来自动处理这个问题。
- **Null Safety:**利用Kotlin的Null - Safety （`?`）来明确定义哪些实体字段在类型级别是可选的或必需的。
- **Spring Data JPA:**通过扩展`JpaRepository`或`CrudRepository`使用Spring Data JPA存储库。
-协程：对于响应式应用程序，在数据层利用Spring Boot对Kotlin协程的支持。

# #日志

- **伴侣对象记录器：**声明记录器的惯用方法是在伴侣对象中。  ```kotlin
  companion object {
      private val logger = LoggerFactory.getLogger(MyClass::class.java)
  }
  ```
- **参数化日志：**使用参数化消息（`logger.info("Processing user {}...", userId)`）的性能和清晰度。

# #测试

- **JUnit 5:** JUnit 5是默认的，与Kotlin无缝工作。
-惯用测试库：要获得更流畅和惯用的测试，请考虑使用**Kotest**进行断言，使用**MockK**进行mock。它们是为Kotlin设计的，提供了更具表现力的语法。
**测试片：**使用测试片注释，如`@WebMvcTest`或`@DataJpaTest`来测试应用程序的特定部分。
**Testcontainers:**使用testcontainer与真实的数据库、消息代理等进行可靠的集成测试。

协程和异步编程**`suspend`函数：**对于非阻塞异步代码，在控制器和服务中使用`suspend`函数。Spring Boot对协程有很好的支持。
**使用`coroutineScope`或`supervisorScope`来管理协程的生命周期。