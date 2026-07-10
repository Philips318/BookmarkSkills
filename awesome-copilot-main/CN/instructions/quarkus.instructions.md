---
applyTo: '*'
description: 'Quarkus development standards and instructions'
---
-使用Java 17或更高版本的高质量Quarkus应用程序的说明。

##项目背景

-最新夸克版本：3.x
—Java版本：17及以上
-使用Maven或Gradle进行构建管理。
-关注干净的架构、可维护性和性能。

##开发标准

-为每个类，方法和复杂逻辑编写清晰简洁的注释。
-使用Javadoc的公共api和方法，以确保清晰的消费者。
-在整个项目中保持一致的编码风格，遵守Java惯例。
-遵守Quarkus编码标准和最佳实践，以获得最佳性能和可维护性。
-遵循雅加达EE和MicroProfile惯例，确保包装组织清晰。
-在适当的地方使用Java 17或更高版本的特性，例如记录和密封类。命名约定
-类名使用PascalCase（例如，`ProductService`,`ProductResource`）。
-方法和变量名使用驼峰大小写（例如，`findProductById`,`isProductAvailable`）。
-常量使用ALL_CAPS（例如，`DEFAULT_PAGE_SIZE`）。

# # Quarkus
-利用Quarkus开发模式加快开发周期。
-使用Quarkus扩展和最佳实践实现构建时优化。
-使用GraalVM配置原生构建以获得最佳性能（例如，使用quarkus-maven-plugin）。
-使用quarkus日志功能（JBoss， SL4J或JUL）进行一致的日志记录实践。

夸克特有模式
-对单例bean使用`@ApplicationScoped`，而不是`@Singleton`-使用`@Inject`进行依赖注入
-首选Panache存储库而不是传统的JPA存储库
—修改数据的业务方法使用`@Transactional`—使用描述性REST端点路径来应用`@Path`—REST资源使用“`@Consumes(MediaType.APPLICATION_JSON)`”和“`@Produces(MediaType.APPLICATION_JSON)`”REST资源
-始终使用JAX-RS注释（`@Path`,`@GET`，`@POST`等）
-返回正确的HTTP状态码（200,201,400,404,500）
-使用`Response`类的复杂响应
-包括适当的错误处理与try-catch块
-使用Bean Validation注释验证输入参数
—对公共端点进行速率限制

###数据访问
-首选Panache实体（扩展`PanacheEntity`）而不是传统的JPA
-使用Panache存储库（`PanacheRepository<T>`）进行复杂查询
—修改数据时始终使用`@Transactional`—对复杂的数据库操作使用命名查询
-为列表端点实现适当的分页


# # #配置
—使用“`application.properties`”或“`application.yaml`”进行简单配置
-使用`@ConfigProperty`作为类型安全的配置类
—敏感数据首选环境变量
-为不同的环境（开发，测试，生产）使用配置文件# # #测试
-使用`@QuarkusTest`进行集成测试
-使用JUnit 5进行单元测试
-使用`@QuarkusIntegrationTest`进行本机构建测试
-使用`@QuarkusTestResource`模拟外部依赖
使用RestAssured进行REST端点测试（`@QuarkusTestResource`）
—修改数据库的测试使用`@Transactional`-使用测试容器进行数据库集成测试

不要使用这些模式：
-不要在测试中使用字段注入（使用构造函数注入）
—不要硬编码配置值
—不要忽略异常


##开发流程

###创建新功能时：
1. 创建具有适当验证的实体
2. 创建带有自定义查询的存储库
3. 创建具有业务逻辑的服务
4. 创建具有适当端点的REST资源
5. 编写全面的测试
6. 添加适当的错误处理
7. 更新文档

##安全考虑###实现安全性时：
—使用Quarkus Security扩展（如`quarkus-smallrye-jwt`、`quarkus-oidc`）。
-使用MicroProfile JWT或OIDC实现基于角色的访问控制（RBAC）。
—验证所有输入参数