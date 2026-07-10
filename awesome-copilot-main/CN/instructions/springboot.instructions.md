---
description: 'Guidelines for building Spring Boot base applications'
applyTo: '**/*.java, **/*.kt'
---
# Spring Boot开发

##一般使用说明

-在审查代码更改时只提出高可信度的建议。
编写具有良好可维护性的代码，包括对做出某些设计决策的原因的注释。
—处理边缘情况，编写清晰的异常处理。
-对于库或外部依赖，在注释中提到它们的用途和目的。

##春季启动说明

依赖注入

-对所有必需的依赖使用构造函数注入。
—声明依赖字段为`private final`。

# # #配置

—使用YAML文件（`application.yml`）进行外部配置。
-环境配置文件：为不同的环境（开发、测试、生产）使用Spring配置文件
配置属性：使用@ConfigurationProperties进行类型安全的配置绑定
-秘密管理：使用环境变量或秘密管理系统将秘密外部化代码组织

-包结构：按feature/domain组织，而不是按层组织
关注点分离：保持控制器精简，服务集中，存储库简单
-实用程序类：使实用程序类最终与私有构造函数

###服务层

-将业务逻辑放在`@Service`注释的类中。
服务应该是无状态且可测试的。
-通过构造函数注入存储库。
服务方法签名应该使用域id或dto，除非必要，否则不要直接暴露存储库实体。

# # #日志

-使用SLF4J进行所有日志记录（`private static final Logger logger = LoggerFactory.getLogger(MyClass.class);`）。
—不要直接使用具体实现（Logback, Log4j2）或`System.out.println()`。
—使用参数化日志：`logger.info("User {} logged in", userId);`。

安全性和输入处理—使用参数化查询|始终使用Spring Data JPA或`NamedParameterJdbcTemplate`，以防止SQL注入。
-使用JSR-380 （`@NotNull`，`@Size`等）注释和`BindingResult`验证请求主体和参数

构建和验证

-在添加或修改代码后，验证项目是否继续成功构建。
—如果项目使用Maven，执行命令`mvn clean package`。
—如果项目使用Gradle，执行命令`./gradlew build`（Windows为`gradlew.bat build`）。
-确保所有测试作为构建的一部分通过。

##实用命令

| Gradle命令| Maven命令|描述|:--------------------------|:----------------------------------|:----------------------------------------------|
|`./gradlew bootRun`|`./mvnw spring-boot:run`|运行应用。|
|`./gradlew build`|`./mvnw package`|构建应用程序。|
|`./gradlew test`|`./mvnw test`|运行测试。|
将应用程序打包为JAR。|
|`./gradlew bootBuildImage`|`./mvnw spring-boot:build-image`|将应用程序打包为容器映像。|