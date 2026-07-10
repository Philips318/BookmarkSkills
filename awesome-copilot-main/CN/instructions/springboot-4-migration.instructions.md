---
description: "Comprehensive guide for migrating Spring Boot applications from 3.x to 4.0, focusing on Gradle Kotlin DSL and version catalogs"
applyTo: "**/*.java, **/*.kt, **/build.gradle.kts, **/build.gradle, **/settings.gradle.kts, **/gradle/libs.versions.toml, **/*.properties, **/*.yml, **/*.yaml"
---
# Spring Boot 3x到4.0迁移指南

##项目背景

本指南为从版本3升级Spring Boot项目提供了全面的GitHub Copilot说明。x到4.0，重点是Gradle Kotlin DSL、版本目录（`libs.versions.toml`）和Kotlin特定的注意事项。

** Spring Boot 4.0的主要架构变化：**
-模块化依赖结构与集中，较小的模块
—Spring Frameworkx所需
- Jakarta EE 11 （Servlet 6.1基线）
——杰克逊X迁移（包名称空间更改）
- Kotlin 2.2+要求
-全面的物业重组

##系统要求

最小版本**Java**: 17+（最好是最新的LTS: Java 21或25）
—**Kotlin**: 2.2.0及以上版本
—**Spring Framework**: 7。x（由Spring Boot 4.0管理）
- **Jakarta EE**: 11 （Servlet 6.1基线）
- **GraalVM**（本机图像）：25+
Gradle: 8.5+（用于Kotlin DSL和版本目录支持）
- Gradle CycloneDX插件**:3.0.0+

验证兼容性```bash
# Check current versions
./gradlew --version
./gradlew dependencies --configuration runtimeClasspath
```
迁移前步骤

# # # 1。升级到Latest Spring Boot 3.5.x

在迁移到4.0之前，请升级到最新的3.5。x版本:```kotlin
// libs.versions.toml
[versions]
springBoot = "3.5.6" # Latest 3.x before migrating to 4.0
```
# # # 2。清理弃用

从Spring Boot 3.x中删除所有弃用的API。这些将是4.0中的编译错误：```bash
# Build and review warnings
./gradlew clean build --warning-mode all
```
# # # 3。检查依赖项更改

比较你的依赖：
—[Spring Boot 3.5.][https://docs.spring.io/spring-boot/3.5/appendix/dependency-versions/coordinates.html]
- [Spring Boot 4.0.]x依赖版本]（https://docs.spring.io/spring-boot/4.0/appendix/dependency-versions/coordinates.html）

模块重组和启动器更改

关键：模块化架构

Spring Boot 4.0引入了**更小、更集中的模块**，取代了大型的单体jar。这需要在大多数项目中更新依赖项。

**对库作者很重要：**由于模块化的努力和包重组，**强烈建议在同一个工件中同时支持Spring Boot 3和Spring Boot 4 **。库作者应该为每个主要版本发布单独的工件，以避免运行时冲突，并确保干净的依赖关系管理。

迁移策略：选择一种方法

####选项1：特定于技术的启动器（推荐用于生产）Spring Boot涵盖的大多数技术现在都有专门的测试启动器伙伴。这提供了细粒度的控制。

**完整的Starter参考：**对于所有可用的Starter （Core, Web, Database, Spring Data, Messaging, Security, template， Production-Ready等）及其测试伙伴的综合表，请参阅[官方Spring Boot 4.0迁移指南]（https://github.com/spring-projects/spring-boot/wiki/Spring-Boot-4.0-Migration-Guide#starters）。

* *libs.versions.toml: * *```toml
[versions]
springBoot = "4.0.0"

[libraries]
# Core starters with dedicated test modules
spring-boot-starter-web = { module = "org.springframework.boot:spring-boot-starter-webmvc", version.ref = "springBoot" }
spring-boot-starter-webmvc-test = { module = "org.springframework.boot:spring-boot-starter-webmvc-test", version.ref = "springBoot" }

spring-boot-starter-data-jpa = { module = "org.springframework.boot:spring-boot-starter-data-jpa", version.ref = "springBoot" }
spring-boot-starter-data-jpa-test = { module = "org.springframework.boot:spring-boot-starter-data-jpa-test", version.ref = "springBoot" }

spring-boot-starter-security = { module = "org.springframework.boot:spring-boot-starter-security", version.ref = "springBoot" }
spring-boot-starter-security-test = { module = "org.springframework.boot:spring-boot-starter-security-test", version.ref = "springBoot" }
```
* * build.gradle.kts: * *```kotlin
dependencies {
    implementation(libs.spring.boot.starter.webmvc)
    implementation(libs.spring.boot.starter.data.jpa)
    implementation(libs.spring.boot.starter.security)

    testImplementation(libs.spring.boot.starter.webmvc.test)
    testImplementation(libs.spring.boot.starter.data.jpa.test)
    testImplementation(libs.spring.boot.starter.security.test)
}
```
####选项2：经典启动器（快速迁移，已弃用）

为了快速迁移，使用经典的启动器，捆绑所有的自动配置（如Spring Boot 3.x）：

* *libs.versions.toml: * *```toml
[libraries]
spring-boot-starter-classic = { module = "org.springframework.boot:spring-boot-starter-classic", version.ref = "springBoot" }
spring-boot-starter-test-classic = { module = "org.springframework.boot:spring-boot-starter-test-classic", version.ref = "springBoot" }
```
* * build.gradle.kts: * *```kotlin
dependencies {
    implementation(libs.spring.boot.starter.classic)
    testImplementation(libs.spring.boot.starter.test.classic)
}
```
**警告**：经典启动器已被**弃用**，并将在未来的版本中删除。计划迁移到特定于技术的启动器。

####选项3：直接模块依赖关系（高级）

对于传递依赖项的显式控制：

* *libs.versions.toml: * *```toml
[libraries]
spring-boot-webmvc = { module = "org.springframework.boot:spring-boot-webmvc", version.ref = "springBoot" }
spring-boot-webmvc-test = { module = "org.springframework.boot:spring-boot-webmvc-test", version.ref = "springBoot" }
```
###重命名启动器（突破性更改）

在`libs.versions.toml`中更新这些启动器名称：

| Spring Bootx | Spring Boot 4.0 | Notes ||----------------|-----------------|-------|
|`spring-boot-starter-web`|`spring-boot-starter-webmvc`| |显式命名
|`spring-boot-starter-web-services`|`spring-boot-starter-webservices`|去掉连字符|
|`spring-boot-starter-aop`|`spring-boot-starter-aspectj`|只需要如果使用`org.aspectj.lang.annotation`|
|`spring-boot-starter-oauth2-authorization-server`|`spring-boot-starter-security-oauth2-authorization-server`|安全命名空间|
|`spring-boot-starter-oauth2-client`|`spring-boot-starter-security-oauth2-client`|安全命名空间|
|`spring-boot-starter-oauth2-resource-server`|`spring-boot-starter-security-oauth2-resource-server`|安全命名空间|

**迁移示例（libs.versions.toml）：**```toml
[libraries]
# Old (Spring Boot 3.x)
# spring-boot-starter-web = { module = "org.springframework.boot:spring-boot-starter-web", version.ref = "springBoot" }
# spring-boot-starter-oauth2-client = { module = "org.springframework.boot:spring-boot-starter-oauth2-client", version.ref = "springBoot" }

# New (Spring Boot 4.0)
spring-boot-starter-webmvc = { module = "org.springframework.boot:spring-boot-starter-webmvc", version.ref = "springBoot" }
spring-boot-starter-security-oauth2-client = { module = "org.springframework.boot:spring-boot-starter-security-oauth2-client", version.ref = "springBoot" }
```
AspectJ Starter澄清

只包含`spring-boot-starter-aspectj`，如果你**实际使用AspectJ注释**：```kotlin
// Only needed if code uses org.aspectj.lang.annotation package
import org.aspectj.lang.annotation.Aspect
import org.aspectj.lang.annotation.Before

@Aspect
class MyAspect {
    @Before("execution(* com.example..*(..))")
    fun beforeAdvice() { }
}
```
如果不使用AspectJ，请删除依赖项。

删除的功能和替代方案

嵌入式服务器

####潜流清除

**Undertow被完全移除** -与Servlet 6.1基线不兼容。

迁移:* * * *
—使用**Tomcat**（默认）或**Jetty**
—不要** **将Spring Boot 4.0应用部署到非servlet 6.1的容器中

* *libs.versions.toml: * *```toml
[libraries]
# Remove Undertow
# spring-boot-starter-undertow = { module = "org.springframework.boot:spring-boot-starter-undertow", version.ref = "springBoot" }

# Use Tomcat (default) or Jetty
spring-boot-starter-jetty = { module = "org.springframework.boot:spring-boot-starter-jetty", version.ref = "springBoot" }
```
* * build.gradle.kts: * *```kotlin
dependencies {
    implementation(libs.spring.boot.starter.webmvc) {
        exclude(group = "org.springframework.boot", module = "spring-boot-starter-tomcat")
    }
    implementation(libs.spring.boot.starter.jetty) // Alternative to Tomcat
}
```
###会话管理

####春季会话删除了Hazelcast和MongoDB

**由各自的团队维护**，不再在Spring Boot依赖管理。

迁移(libs.versions.toml): * * * *```toml
[versions]
hazelcast-spring-session = "3.x.x" # Check Hazelcast documentation
mongodb-spring-session = "4.x.x"   # Check MongoDB documentation

[libraries]
# Explicit versions required
spring-session-hazelcast = { module = "com.hazelcast:spring-session-hazelcast", version.ref = "hazelcast-spring-session" }
spring-session-mongodb = { module = "org.springframework.session:spring-session-data-mongodb", version.ref = "mongodb-spring-session" }
```
响应式消息传递

####脉冲星反应移除

Spring脉冲星放弃了反应器支持- reactive脉冲星客户端被移除。

迁移:* * * *
-使用命令式脉冲星客户端
-或者迁移到其他响应式消息（Kafka, RabbitMQ）

# # #测试

####移除Spock框架

**Spock还不支持Groovy 5** （Spring Boot 4.0需要）。

迁移:* * * *
-使用junit5与Kotlin
-或者等待Spock Groovy 5兼容性

###构建功能

####可执行Jar启动脚本删除

删除了用于“完全可执行”jar的嵌入式启动脚本（特定于unix，使用有限）。

* * build.gradle。康泰斯(删除):* *```kotlin
// Remove this configuration
tasks.bootJar {
    launchScript() // No longer supported
}
```
* *的替代品:* *
—直接使用`java -jar app.jar`使用Gradle Application Plugin作为本地启动器
—使用systemd服务文件

####经典的Uber-Jar加载器删除

经典的uber-jar加载器已被移除。从构建中删除任何加载器实现配置。

**Maven (pom.xml) -删除：**```xml
<build>
    <plugins>
        <plugin>
            <groupId>org.springframework.boot</groupId>
            <artifactId>spring-boot-maven-plugin</artifactId>
            <configuration>
                <loaderImplementation>CLASSIC</loaderImplementation> <!-- REMOVE THIS -->
            </configuration>
        </plugin>
    </plugins>
</build>
```
* * Gradle (build.gradle。删除：**```kotlin
tasks.bootJar {
    loaderImplementation = org.springframework.boot.loader.tools.LoaderImplementation.CLASSIC // REMOVE THIS
}
```
##杰克逊3移民

重大变更：包命名空间

Jackson 3更改了**组ID和包名**：

|组件| Old (Jackson 2) | New (Jackson 3) ||-----------|----------------|-----------------|
|组ID |`com.fasterxml.jackson`|`tools.jackson`|
|包|`com.fasterxml.jackson.*`|`tools.jackson.*`|
|例外|`jackson-annotations`|仍然使用`com.fasterxml.jackson.core`组|

* *libs.versions.toml: * *```toml
[versions]
jackson = "3.0.1" # Managed by Spring Boot 4.0

[libraries]
# Jackson 3 uses new group ID
jackson-databind = { module = "tools.jackson.core:jackson-databind", version.ref = "jackson" }
jackson-module-kotlin = { module = "tools.jackson.module:jackson-module-kotlin", version.ref = "jackson" }

# Exception: annotations still use old group
jackson-annotations = { module = "com.fasterxml.jackson.core:jackson-annotations", version.ref = "jackson" }
```
类和注释重命名

更新导入和注释：

| Spring Bootx | Spring Boot 4.0 ||----------------|-----------------|
|`Jackson2ObjectMapperBuilderCustomizer`|`JsonMapperBuilderCustomizer`|
|`JsonObjectSerializer`|`ObjectValueSerializer`|
|`JsonValueDeserializer`|`ObjectValueDeserializer`|
|`@JsonComponent`|`@JacksonComponent`|
|`@JsonMixin`|`@JacksonMixin`|

* *迁移的例子:* *```kotlin
// Old (Spring Boot 3.x)
import com.fasterxml.jackson.databind.ObjectMapper
import org.springframework.boot.autoconfigure.jackson.Jackson2ObjectMapperBuilderCustomizer
import org.springframework.boot.jackson.JsonComponent

@JsonComponent
class CustomSerializer : JsonSerializer<MyType>() { }

@Configuration
class JacksonConfig {
    @Bean
    fun customizer(): Jackson2ObjectMapperBuilderCustomizer {
        return Jackson2ObjectMapperBuilderCustomizer { builder ->
            builder.simpleDateFormat("yyyy-MM-dd")
        }
    }
}

// New (Spring Boot 4.0)
import tools.jackson.databind.ObjectMapper
import org.springframework.boot.autoconfigure.jackson.JsonMapperBuilderCustomizer
import org.springframework.boot.jackson.JacksonComponent

@JacksonComponent
class CustomSerializer : JsonSerializer<MyType>() { }

@Configuration
class JacksonConfig {
    @Bean
    fun customizer(): JsonMapperBuilderCustomizer {
        return JsonMapperBuilderCustomizer { builder ->
            builder.simpleDateFormat("yyyy-MM-dd")
        }
    }
}
```
配置属性更改

* *application.yml迁移:* *```yaml
# Old (Spring Boot 3.x)
spring:
  jackson:
    read:
      enums-using-to-string: true
    write:
      dates-as-timestamps: false

# New (Spring Boot 4.0)
spring:
  jackson:
    json:
      read:
        enums-using-to-string: true
      write:
        dates-as-timestamps: false
```
###杰克逊2兼容模块（临时）

对于逐步迁移，请使用**临时兼容性模块**（已弃用，将被删除）：

* *libs.versions.toml: * *```toml
[libraries]
spring-boot-jackson2 = { module = "org.springframework.boot:spring-boot-jackson2", version.ref = "springBoot" }
```
* * build.gradle.kts: * *```kotlin
dependencies {
    implementation(libs.spring.boot.jackson2)
}
```
* *application.yml: * *```yaml
spring:
  jackson:
    use-jackson2-defaults: true # Use Jackson 2 behavior
```
**使用兼容性模块时，`spring.jackson2.*`命名空间**下的属性。

**计划从这个模块迁移** -它将在未来的版本中被删除。

##核心框架改变

注释：JSpecify

Spring Boot 4.0在整个代码库中添加了JSpecify空性注释。

* *影响:* *
- Kotlin null-safety可能会标记新的warnings/errors- Null检查器（SpotBugs, NullAway）可能会报告新的问题
- **像`body()`这样的RestClient方法现在显式标记为可空** -始终检查是否为空或使用`Objects.requireNonNull()`** Kotlin迁移：**```kotlin
// Explicit nullable types may be required
fun processUser(id: String?): User? {
    return userRepository.findById(id) // May now be explicitly nullable
}

// RestClient body() can return null
val body: String? = restClient.get()
    .uri("https://api.example.com/data")
    .retrieve()
    .body(String::class.java) // Nullable - handle appropriately

if (body != null) {
    println(body.length)
}
```
**执行器端点参数：**
—不能使用`javax.annotations.NonNull`或`org.springframework.lang.Nullable`—请使用`org.jspecify.annotations.Nullable`* *libs.versions.toml: * *```toml
[libraries]
jspecify = { module = "org.jspecify:jspecify", version = "1.0.0" }
```
###包重新定位

# # # # BootstrapRegistry

* *老进口:* *```kotlin
import org.springframework.boot.BootstrapRegistry
```
* *新导入:* *```kotlin
import org.springframework.boot.bootstrap.BootstrapRegistry
```
# # # # EnvironmentPostProcessor

* *老进口:* *```kotlin
import org.springframework.boot.env.EnvironmentPostProcessor
```
* *新导入:* *```kotlin
import org.springframework.boot.EnvironmentPostProcessor
```
* *更新`META-INF/spring.factories`: * *```properties
# Old
org.springframework.boot.env.EnvironmentPostProcessor=com.example.MyPostProcessor

# New
org.springframework.boot.EnvironmentPostProcessor=com.example.MyPostProcessor
```
**注意：**已弃用的表单暂时仍然可用，但将被删除。

####实体扫描

* *老进口:* *```kotlin
import org.springframework.boot.autoconfigure.domain.EntityScan
```
* *新导入:* *```kotlin
import org.springframework.boot.persistence.autoconfigure.EntityScan
```
记录更改

#### Logback默认字符集

日志文件现在默认为**UTF-8**（与Log4j2一致）：

**logback-spring.xml（显式配置）：**```xml
<configuration>
    <appender name="FILE" class="ch.qos.logback.core.FileAppender">
        <file>app.log</file>
        <encoder>
            <charset>UTF-8</charset> <!-- Now default -->
            <pattern>%d{yyyy-MM-dd HH:mm:ss} - %msg%n</pattern>
        </encoder>
    </appender>
</configuration>
```
**控制台日志记录：**使用`Console#charset()`，如果可用（Java 17+），否则退回到UTF-8。这在保持编码一致的同时提供了更好的平台兼容性。

### DevTools变更

####默认禁用实时加载

* *application.yml: * *```yaml
spring:
  devtools:
    livereload:
      enabled: true # Must explicitly enable in 4.0
```
* *libs.versions.toml: * *```toml
[libraries]
spring-boot-devtools = { module = "org.springframework.boot:spring-boot-devtools", version.ref = "springBoot" }
```
* * build.gradle.kts: * *```kotlin
dependencies {
    developmentOnly(libs.spring.boot.devtools)
}
```
PropertyMapper API行为改变

**突破性的改变：**不再调用adapter/predicate方法默认源是`null`。

* *迁移模式:* *```kotlin
// Old behavior (Spring Boot 3.x)
map.from(source::method).to(destination::method)
// Calls destination.method(null) if source returns null

// New behavior (Spring Boot 4.0)
map.from(source::method).to(destination::method)
// Skips call if source returns null

// Explicit null handling (new)
map.from(source::method).always().to(destination::method)
// Always calls destination.method(value), even if null
```
**删除方法：**`alwaysApplyingNotNull()`-使用`always()`代替。

**迁移示例：**查看[Spring Boot commit 239f384ac0](https://github.com/spring-projects/spring-boot/commit/239f384ac0893d151b89f204886874c6adb00001)，看看Spring Boot本身如何适应新的API。

依赖和构建变更

Gradle插件更新

* * build.gradle.kts: * *```kotlin
plugins {
    kotlin("jvm") version "2.2.0" // Minimum 2.2.0
    kotlin("plugin.spring") version "2.2.0"
    id("org.springframework.boot") version "4.0.0"
    id("io.spring.dependency-management") version "1.1.7"
    id("org.cyclonedx.bom") version "3.0.0" // Minimum 3.0.0
}
```
Gradle中的可选依赖项

默认情况下，可选依赖项**不再包含在uber jar中。

* * build.gradle。KTS（显式地包括可选项）：**```kotlin
tasks.bootJar {
    includeOptional = true // If needed
}
```
Spring Retry→Spring Framework Core Retry

Spring Boot 4.0删除了对Spring Retry的依赖管理（将投资组合迁移到Spring Framework 7.0核心重试）。

迁移选项1：使用Spring Framework Core Retry（推荐）**```kotlin
// Use built-in Spring Framework retry
import org.springframework.core.retry.RetryTemplate
import org.springframework.core.retry.support.RetryTemplateBuilder

@Configuration
class RetryConfig {
    @Bean
    fun retryTemplate(): RetryTemplate {
        return RetryTemplateBuilder()
            .maxAttempts(3)
            .fixedBackoff(1000)
            .build()
    }
}
```
迁移选项2：显式Spring重试版本（临时）**

* *libs.versions.toml: * *```toml
[versions]
spring-retry = "2.0.5" # Explicit version required

[libraries]
spring-retry = { module = "org.springframework.retry:spring-retry", version.ref = "spring-retry" }
```
**计划迁移到Spring Framework核心重试

Spring授权服务器

现在Spring安全的一部分——明确的版本管理被移除。

**libs.versions.toml（之前- Spring Boot 3.x）：**```toml
[versions]
spring-authorization-server = "1.3.0" # No longer works

[libraries]
spring-security-oauth2-authorization-server = { module = "org.springframework.security:spring-security-oauth2-authorization-server", version.ref = "spring-authorization-server" }
```
**迁移（Spring Boot 4.0）：**```toml
[versions]
spring-security = "7.0.0" # Use Spring Security version instead

[libraries]
# Managed by spring-security.version property, not separate
spring-security-oauth2-authorization-server = { module = "org.springframework.security:spring-security-oauth2-authorization-server", version.ref = "spring-security" }
```
或者依赖Spring Boot依赖管理（推荐）：```kotlin
dependencies {
    implementation("org.springframework.security:spring-security-oauth2-authorization-server")
    // Version managed by Spring Boot 4.0
}
```
Elasticsearch客户端变更

####底层客户端替换

**已弃用低级`RestClient`→新`Rest5Client`:**

**注：**高级客户端（`ElasticsearchClient`和Spring Data的`ReactiveElasticsearchClient`） **保持不变，并已在内部更新以使用新的低级客户端。

进口* *:* *```kotlin
// Old (Spring Boot 3.x)
import org.elasticsearch.client.RestClient
import org.elasticsearch.client.RestClientBuilder
import org.springframework.boot.autoconfigure.elasticsearch.RestClientBuilderCustomizer

// New (Spring Boot 4.0)
import co.elastic.clients.transport.rest_client.Rest5Client
import co.elastic.clients.transport.rest_client.Rest5ClientBuilder
import org.springframework.boot.autoconfigure.elasticsearch.Rest5ClientBuilderCustomizer
```
配置:* * * *```kotlin
@Configuration
class ElasticsearchConfig {

    // Old
    // @Bean
    // fun restClientCustomizer(): RestClientBuilderCustomizer {
    //     return RestClientBuilderCustomizer { builder ->
    //         builder.setRequestConfigCallback { config ->
    //             config.setConnectTimeout(5000)
    //         }
    //     }
    // }

    // New
    @Bean
    fun rest5ClientCustomizer(): Rest5ClientBuilderCustomizer {
        return Rest5ClientBuilderCustomizer { builder ->
            builder.setRequestConfigCallback { config ->
                config.setConnectTimeout(5000)
            }
        }
    }
}
```
* *依赖整合:* *

嗅探器现在包含在`co.elastic.clients:elasticsearch-java`模块中。

* *libs.versions.toml: * *```toml
[libraries]
# Remove these - no longer managed
# elasticsearch-rest-client = { module = "org.elasticsearch.client:elasticsearch-rest-client", version = "..." }
# elasticsearch-rest-client-sniffer = { module = "org.elasticsearch.client:elasticsearch-rest-client-sniffer", version = "..." }

# Use single dependency (includes sniffer)
elasticsearch-java = { module = "co.elastic.clients:elasticsearch-java", version = "8.x.x" }
```
Hibernate依赖改变

* *libs.versions.toml: * *```toml
[libraries]
# Renamed module (hibernate-jpamodelgen replaced by hibernate-processor)
hibernate-processor = { module = "org.hibernate.orm:hibernate-processor", version.ref = "hibernate" }

# These artifacts are NO LONGER PUBLISHED by Hibernate:
# hibernate-proxool - discontinued by Hibernate project
# hibernate-vibur - discontinued by Hibernate project
# Remove any dependencies on these modules
```
**注意：**`hibernate-jpamodelgen`工件仍然存在，但已弃用。继续使用`hibernate-processor`。

##配置属性更改

MongoDB属性重组

**主要重组：**非spring Data属性移至`spring.mongodb.*`；

* *application.yml迁移:* *```yaml
# Old (Spring Boot 3.x)
spring:
  data:
    mongodb:
      uri: mongodb://localhost:27017/mydb
      database: mydb
      host: localhost
      port: 27017
      username: user
      password: pass
      authentication-database: admin
      replica-set-name: rs0
      additional-hosts:
        - host1:27017
        - host2:27017
      ssl:
        enabled: true
        bundle: my-bundle
      representation:
        uuid: STANDARD

management:
  health:
    mongo:
      enabled: true
  metrics:
    mongo:
      command:
        enabled: true
      connectionpool:
        enabled: true

# New (Spring Boot 4.0)
spring:
  mongodb:
    uri: mongodb://localhost:27017/mydb
    database: mydb
    host: localhost
    port: 27017
    username: user
    password: pass
    authentication-database: admin
    replica-set-name: rs0
    additional-hosts:
      - host1:27017
      - host2:27017
    ssl:
      enabled: true
      bundle: my-bundle
    representation:
      uuid: STANDARD # Explicit configuration now required

  data:
    mongodb:
      # Spring Data-specific properties remain here
      auto-index-creation: true
      field-naming-strategy: org.springframework.data.mapping.model.SnakeCaseFieldNamingStrategy
      gridfs:
        bucket: fs
        database: gridfs-db
      repositories:
        type: auto
      representation:
        big-decimal: DECIMAL128 # Explicit configuration now required

management:
  health:
    mongodb: # Renamed from "mongo"
      enabled: true
  metrics:
    mongodb: # Renamed from "mongo"
      command:
        enabled: true
      connectionpool:
        enabled: true
```
* *主要变化:* *
- **UUID表示**:**必选** -无默认提供，必须显式配置`spring.mongodb.representation.uuid`（例如，`STANDARD`,`JAVA_LEGACY`,`PYTHON_LEGACY`,`C_SHARP_LEGACY`）
- **BigDecimal表示**:**必选** -不提供默认值，必须显式配置`spring.data.mongodb.representation.big-decimal`（例如，`DECIMAL128`,`STRING`）
- **管理属性**:`mongo`→`mongodb`- **配置失败将导致在持久化UUID或BigDecimal值时出现运行错误**

Spring会话属性重命名

* *application.yml迁移:* *```yaml
# Old (Spring Boot 3.x)
spring:
  session:
    redis:
      namespace: myapp:session
      flush-mode: on-save
    mongodb:
      collection-name: sessions

# New (Spring Boot 4.0)
spring:
  session:
    data:
      redis:
        namespace: myapp:session
        flush-mode: on-save
      mongodb:
        collection-name: sessions
```
持久性模块属性更改

* *application.yml迁移:* *```yaml
# Old (Spring Boot 3.x)
spring:
  dao:
    exceptiontranslation:
      enabled: true

# New (Spring Boot 4.0)
spring:
  persistence:
    exceptiontranslation:
      enabled: true
```
## Web框架改变

静态资源位置`PathRequest#toStaticResources()`现在默认包含`/fonts/**`。

**安全配置（必要时排除字体）：**```kotlin
import org.springframework.boot.autoconfigure.security.servlet.PathRequest
import org.springframework.boot.autoconfigure.security.StaticResourceLocation

@Configuration
@EnableWebSecurity
class SecurityConfig {

    @Bean
    fun securityFilterChain(http: HttpSecurity): SecurityFilterChain {
        http {
            authorizeHttpRequests {
                // Exclude fonts if needed
                authorize(PathRequest.toStaticResources()
                    .atCommonLocations()
                    .excluding(StaticResourceLocation.FONTS), permitAll)
                authorize(anyRequest, authenticated)
            }
        }
        return http.build()
    }
}
```
HttpMessageConverters已弃用

由于框架改进，`HttpMessageConverters`已弃用（合并的client/server转换器）。

迁移:* * * *```kotlin
// Old (Spring Boot 3.x)
import org.springframework.boot.autoconfigure.http.HttpMessageConverters
import org.springframework.context.annotation.Bean

@Configuration
class WebConfig {
    @Bean
    fun customConverters(): HttpMessageConverters {
        return HttpMessageConverters(MyCustomConverter())
    }
}

// New (Spring Boot 4.0)
import org.springframework.boot.autoconfigure.http.client.ClientHttpMessageConvertersCustomizer
import org.springframework.boot.autoconfigure.http.server.ServerHttpMessageConvertersCustomizer

@Configuration
class WebConfig {

    // Separate client and server converters
    @Bean
    fun clientConvertersCustomizer(): ClientHttpMessageConvertersCustomizer {
        return ClientHttpMessageConvertersCustomizer { converters ->
            converters.add(MyCustomClientConverter())
        }
    }

    @Bean
    fun serverConvertersCustomizer(): ServerHttpMessageConvertersCustomizer {
        return ServerHttpMessageConvertersCustomizer { converters ->
            converters.add(MyCustomServerConverter())
        }
    }
}
```
删除了尾斜杠URL匹配`PathMatchConfigurer#setUseTrailingSlashMatch(true)`在Spring Framework 7 / Spring Boot 4中被**删除**。
没有替换配置旋钮-`/foo`和`/foo/`不再被视为相同的路由。

**迁移：**注册Spring框架的`UrlHandlerFilter`为`FilterRegistrationBean`运行在安全链的前面。`wrapRequest()`使其透明地向前(不
重定向)，端到端保留旧的行为：```java
import org.springframework.boot.web.servlet.FilterRegistrationBean;
import org.springframework.web.filter.UrlHandlerFilter;

@Configuration
class WebConfig {

    // After ForwardedHeaderFilter, before ServletRequestPathFilter and security filters.
    private static final int BEFORE_SECURITY_FILTER_ORDER = -101;

    @Bean
    FilterRegistrationBean<UrlHandlerFilter> trailingSlashHandlerFilter() {
        UrlHandlerFilter filter = UrlHandlerFilter.trailingSlashHandler("/**").wrapRequest().build();
        FilterRegistrationBean<UrlHandlerFilter> registration = new FilterRegistrationBean<>(filter);
        registration.setOrder(BEFORE_SECURITY_FILTER_ORDER);
        return registration;
    }
}
```
球衣和杰克逊3不兼容

**Jersey 4.0限制：** Spring Boot 4.0支持Jersey 4.0，其中**尚不支持Jackson 3**。

**解决方案：**使用`spring-boot-jackson2`兼容模块**在**`spring-boot-jackson`的位置或旁边：

* *libs.versions.toml: * *```toml
[libraries]
spring-boot-starter-jersey = { module = "org.springframework.boot:spring-boot-starter-jersey", version.ref = "springBoot" }
spring-boot-jackson2 = { module = "org.springframework.boot:spring-boot-jackson2", version.ref = "springBoot" }
# Optional: Keep Jackson 3 for non-Jersey parts of application
spring-boot-jackson = { module = "org.springframework.boot:spring-boot-jackson", version.ref = "springBoot" }
```
* * build.gradle.kts: * *```kotlin
dependencies {
    implementation(libs.spring.boot.starter.jersey)
    implementation(libs.spring.boot.jackson2) // Required for Jersey JSON processing
    // Optional: Use Jackson 3 elsewhere in application
    // implementation(libs.spring.boot.jackson)
}
```
**注意：**如果在您的应用程序中只使用Jersey，您可以将Jackson 3完全替换为Jackson 2兼容性模块。

消息传递框架更改

Kafka流定制器的替换

**已弃用`StreamBuilderFactoryBeanCustomizer`→`StreamsBuilderFactoryBeanConfigurer`:**```kotlin
// Old (Spring Boot 3.x)
import org.springframework.boot.autoconfigure.kafka.StreamsBuilderFactoryBeanCustomizer

@Configuration
class KafkaStreamsConfig {
    @Bean
    fun streamsCustomizer(): StreamBuilderFactoryBeanCustomizer {
        return StreamBuilderFactoryBeanCustomizer { factoryBean ->
            factoryBean.setKafkaStreamsCustomizer { streams ->
                // Custom config
            }
        }
    }
}

// New (Spring Boot 4.0)
import org.springframework.kafka.config.StreamsBuilderFactoryBeanConfigurer

@Configuration
class KafkaStreamsConfig {
    @Bean
    fun streamsConfigurer(): StreamsBuilderFactoryBeanConfigurer {
        return StreamsBuilderFactoryBeanConfigurer { factoryBean ->
            factoryBean.setKafkaStreamsCustomizer { streams ->
                // Custom config
            }
        }
    }
}
```
**注：**新配置器使用默认值`0`实现`Ordered`。

### Kafka重试属性更改

* *application.yml迁移:* *```yaml
# Old (Spring Boot 3.x)
spring:
  kafka:
    retry:
      topic:
        backoff:
          random: true

# New (Spring Boot 4.0)
spring:
  kafka:
    retry:
      topic:
        backoff:
          jitter: 0.5 # More flexible than boolean
```
RabbitMQ重试Customizer分裂

**Spring AMQP从Spring重试移动到Spring框架核心重试**，自定义器分裂：```kotlin
// Old (Spring Boot 3.x)
import org.springframework.boot.autoconfigure.amqp.RabbitRetryTemplateCustomizer

@Configuration
class RabbitConfig {
    @Bean
    fun retryCustomizer(): RabbitRetryTemplateCustomizer {
        return RabbitRetryTemplateCustomizer { template ->
            // Applies to both RabbitTemplate and listeners
        }
    }
}

// New (Spring Boot 4.0)
import org.springframework.boot.autoconfigure.amqp.RabbitTemplateRetrySettingsCustomizer
import org.springframework.boot.autoconfigure.amqp.RabbitListenerRetrySettingsCustomizer

@Configuration
class RabbitConfig {

    // For RabbitTemplate operations
    @Bean
    fun templateRetryCustomizer(): RabbitTemplateRetrySettingsCustomizer {
        return RabbitTemplateRetrySettingsCustomizer { settings ->
            settings.maxAttempts = 5
        }
    }

    // For message listeners
    @Bean
    fun listenerRetryCustomizer(): RabbitListenerRetrySettingsCustomizer {
        return RabbitListenerRetrySettingsCustomizer { settings ->
            settings.maxAttempts = 3
        }
    }
}
```
测试框架变更

移除Mockito集成`MockitoTestExecutionListener`已删除（在3.4中已弃用）。

**迁移到MockitoExtension:**```kotlin
// Old (Spring Boot 3.x)
import org.springframework.boot.test.context.SpringBootTest
import org.mockito.Mock
import org.mockito.Captor

@SpringBootTest
class MyServiceTest {
    @Mock
    private lateinit var repository: MyRepository

    @Captor
    private lateinit var captor: ArgumentCaptor<String>
}

// New (Spring Boot 4.0)
import org.springframework.boot.test.context.SpringBootTest
import org.mockito.Mock
import org.mockito.Captor
import org.mockito.junit.jupiter.MockitoExtension
import org.junit.jupiter.api.extension.ExtendWith

@SpringBootTest
@ExtendWith(MockitoExtension::class) // Explicit extension required
class MyServiceTest {
    @Mock
    private lateinit var repository: MyRepository

    @Captor
    private lateinit var captor: ArgumentCaptor<String>
}
```
### @SpringBootTest Changes`@SpringBootTest`不再自动提供**MockMVC**、**WebTestClient**或**TestRestTemplate**。

#### MockMVC配置```kotlin
// Old (Spring Boot 3.x)
@SpringBootTest(webEnvironment = SpringBootTest.WebEnvironment.RANDOM_PORT)
class ControllerTest {
    @Autowired
    private lateinit var mockMvc: MockMvc // Available automatically
}

// New (Spring Boot 4.0)
import org.springframework.boot.test.autoconfigure.web.servlet.AutoConfigureMockMvc
import org.springframework.boot.test.autoconfigure.web.servlet.HtmlUnit

@SpringBootTest(webEnvironment = SpringBootTest.WebEnvironment.RANDOM_PORT)
@AutoConfigureMockMvc // Explicit annotation required
class ControllerTest {
    @Autowired
    private lateinit var mockMvc: MockMvc
}

// HtmlUnit configuration moved to annotation attribute
@AutoConfigureMockMvc(
    htmlUnit = HtmlUnit(webClient = false, webDriver = false)
)
```
#### WebTestClient配置```kotlin
// Old (Spring Boot 3.x)
@SpringBootTest(webEnvironment = SpringBootTest.WebEnvironment.RANDOM_PORT)
class WebFluxTest {
    @Autowired
    private lateinit var webTestClient: WebTestClient // Available automatically
}

// New (Spring Boot 4.0)
import org.springframework.boot.test.autoconfigure.web.reactive.AutoConfigureWebTestClient

@SpringBootTest(webEnvironment = SpringBootTest.WebEnvironment.RANDOM_PORT)
@AutoConfigureWebTestClient // Explicit annotation required
class WebFluxTest {
    @Autowired
    private lateinit var webTestClient: WebTestClient
}
```
#### TestRestTemplate→RestTestClient（推荐）

**Spring Boot 4.0引入`RestTestClient`**作为`TestRestTemplate`的现代替代品。```kotlin
// Old approach (still works with annotation)
import org.springframework.boot.test.autoconfigure.web.client.AutoConfigureTestRestTemplate
import org.springframework.boot.test.web.client.TestRestTemplate

@SpringBootTest(webEnvironment = SpringBootTest.WebEnvironment.RANDOM_PORT)
@AutoConfigureTestRestTemplate // Required in 4.0
class RestApiTest {
    @Autowired
    private lateinit var testRestTemplate: TestRestTemplate
}

// New recommended approach
import org.springframework.boot.test.autoconfigure.web.client.AutoConfigureRestTestClient
import org.springframework.boot.resttestclient.RestTestClient

@SpringBootTest(webEnvironment = SpringBootTest.WebEnvironment.RANDOM_PORT)
@AutoConfigureRestTestClient // New annotation
class RestApiTest {
    @Autowired
    private lateinit var restTestClient: RestTestClient

    @Test
    fun testEndpoint() {
        val response = restTestClient.get()
            .uri("/api/users")
            .retrieve()
            .toEntity<List<User>>()

        assertThat(response.statusCode).isEqualTo(HttpStatus.OK)
    }
}
```
**TestRestTemplate包更改（如果仍在使用）：**

**重要：**如果继续使用`TestRestTemplate`，您必须：
1. 添加`spring-boot-resttestclient`测试依赖项
2. **更新包导入**（类移动到新包）

* *libs.versions.toml: * *```toml
[libraries]
spring-boot-resttestclient = { module = "org.springframework.boot:spring-boot-resttestclient", version.ref = "springBoot" }
```
* * build.gradle.kts: * *```kotlin
dependencies {
    testImplementation(libs.spring.boot.resttestclient)
}
```
**更新包导入（必选）：**```kotlin
// Old package import - will cause compilation failure
// import org.springframework.boot.test.web.client.TestRestTemplate

// New package import - required in Spring Boot 4.0
import org.springframework.boot.resttestclient.TestRestTemplate
```
### @PropertyMapping注释重定位```kotlin
// Old (Spring Boot 3.x)
import org.springframework.boot.test.autoconfigure.properties.PropertyMapping
import org.springframework.boot.test.autoconfigure.properties.Skip

// New (Spring Boot 4.0)
import org.springframework.boot.test.context.PropertyMapping
import org.springframework.boot.test.context.PropertyMapping.Skip
```
生产就绪的功能和模块

运行状况、度量和可观察性模块

Spring Boot 4.0将生产就绪的功能模块化为集中的模块：

* *libs.versions.toml: * *```toml
[libraries]
# Health monitoring
spring-boot-health = { module = "org.springframework.boot:spring-boot-health", version.ref = "springBoot" }

# Micrometer metrics
spring-boot-micrometer-metrics = { module = "org.springframework.boot:spring-boot-micrometer-metrics", version.ref = "springBoot" }
spring-boot-micrometer-metrics-test = { module = "org.springframework.boot:spring-boot-micrometer-metrics-test", version.ref = "springBoot" }

# Micrometer observation
spring-boot-micrometer-observation = { module = "org.springframework.boot:spring-boot-micrometer-observation", version.ref = "springBoot" }

# Distributed tracing
spring-boot-micrometer-tracing = { module = "org.springframework.boot:spring-boot-micrometer-tracing", version.ref = "springBoot" }
spring-boot-micrometer-tracing-test = { module = "org.springframework.boot:spring-boot-micrometer-tracing-test", version.ref = "springBoot" }
spring-boot-micrometer-tracing-brave = { module = "org.springframework.boot:spring-boot-micrometer-tracing-brave", version.ref = "springBoot" }
spring-boot-micrometer-tracing-opentelemetry = { module = "org.springframework.boot:spring-boot-micrometer-tracing-opentelemetry", version.ref = "springBoot" }

# OpenTelemetry integration
spring-boot-opentelemetry = { module = "org.springframework.boot:spring-boot-opentelemetry", version.ref = "springBoot" }

# Zipkin reporter
spring-boot-zipkin = { module = "org.springframework.boot:spring-boot-zipkin", version.ref = "springBoot" }
```
* * build.gradle。KTS（示例可观察性堆栈）：**```kotlin
dependencies {
    // Actuator with metrics and tracing
    implementation(libs.spring.boot.starter.actuator)
    implementation(libs.spring.boot.micrometer.observation)
    implementation(libs.spring.boot.micrometer.tracing.opentelemetry)
    implementation(libs.spring.boot.opentelemetry)

    // Test support
    testImplementation(libs.spring.boot.micrometer.metrics.test)
    testImplementation(libs.spring.boot.micrometer.tracing.test)
}
```
**注意：**大多数使用启动器的应用程序（例如，`spring-boot-starter-actuator`）不需要直接声明这些模块。使用直接模块依赖关系进行细粒度控制。

##执行器改变

###默认启用健康探测

激活和准备探针现在**默认启用**。

**application.yml（如果需要禁用）：**```yaml
management:
  endpoint:
    health:
      probes:
        enabled: false # Disable if not using Kubernetes probes
```
* *自动曝光:* *
——`/actuator/health/liveness`——`/actuator/health/readiness`##构建配置

Kotlin编译器配置

* * build.gradle.kts: * *```kotlin
import org.jetbrains.kotlin.gradle.tasks.KotlinCompile

plugins {
    kotlin("jvm") version "2.2.0" // Minimum 2.2.0
    kotlin("plugin.spring") version "2.2.0"
    kotlin("plugin.jpa") version "2.2.0"
    id("org.springframework.boot") version "4.0.0"
    id("io.spring.dependency-management") version "1.1.7"
}

java {
    toolchain {
        languageVersion = JavaLanguageVersion.of(21) // Or 17, 25
    }
}

kotlin {
    compilerOptions {
        freeCompilerArgs.addAll(
            "-Xjsr305=strict", // Strict null-safety
            "-Xemit-jvm-type-annotations" // Emit type annotations
        )
    }
}

tasks.withType<KotlinCompile> {
    kotlinOptions {
        jvmTarget = "21" // Match Java toolchain
    }
}

tasks.withType<Test> {
    useJUnitPlatform()
}
```
Java预览功能（如果使用Java 25）

* * build.gradle.kts: * *```kotlin
tasks.withType<JavaCompile> {
    options.compilerArgs.add("--enable-preview")
}

tasks.withType<Test> {
    jvmArgs("--enable-preview")
}

tasks.withType<JavaExec> {
    jvmArgs("--enable-preview")
}
```
##迁移清单

# # #预迁移

-[]升级到最新的Spring Boot 3.5.x
-检查并修复所有弃用警告
-[]记录当前依赖版本
-[]运行完整的测试套件并验证绿色构建
- [] Review [Spring Boot 3.5.]（https://docs.spring.io/spring-boot/4.0/appendix/dependency-versions/coordinates.html）

核心迁移

-[]使用Spring Boot 4.0.0更新`libs.versions.toml`-[]更新Kotlin版本到2.2.0+
-[]重新命名启动器：`spring-boot-starter-web`→`spring-boot-starter-webmvc`等。
[]添加特定技术的测试启动器（或暂时使用经典启动器）
[]移除对暗流的依赖（切换到Tomcat/Jetty）
-[]删除`spring-session-hazelcast`/`spring-session-mongodb`或添加显式版本

###杰克逊3移民-[]更新导入：`com.fasterxml.jackson`→`tools.jackson`—[]更新例外：`jackson-annotations`仍然使用`com.fasterxml.jackson.core`-[]重命名：`@JsonComponent`→`@JacksonComponent`-[]重命名：`Jackson2ObjectMapperBuilderCustomizer`→`JsonMapperBuilderCustomizer`-[]更新属性：`spring.jackson.read.*`→`spring.jackson.json.read.*`-[]如果需要，考虑临时`spring-boot-jackson2`模块

###属性更新

- [] MongoDB:`spring.data.mongodb.*`→`spring.mongodb.*`（非spring Data属性）
-[]会话：`spring.session.redis.*`→`spring.session.data.redis.*`-[]持久性：`spring.dao.exceptiontranslation`→`spring.persistence.exceptiontranslation`- [] Kafka retry:`backoff.random`→`backoff.jitter`###代码更新

-[]更新包：`BootstrapRegistry`→`org.springframework.boot.bootstrap.BootstrapRegistry`-[]更新包：`EnvironmentPostProcessor`→`org.springframework.boot.EnvironmentPostProcessor`-[]更新包：`EntityScan`→`org.springframework.boot.persistence.autoconfigure.EntityScan`[]更新：`RestClient`→`Rest5Client`（Elasticsearch）
-[]更新：`StreamBuilderFactoryBeanCustomizer`→`StreamsBuilderFactoryBeanConfigurer`（Kafka）
-[]分裂：`RabbitRetryTemplateCustomizer`→`RabbitTemplateRetrySettingsCustomizer`/`RabbitListenerRetrySettingsCustomizer`-[]替换：`HttpMessageConverters`→`ClientHttpMessageConvertersCustomizer`/`ServerHttpMessageConvertersCustomizer`-[]更新：`PropertyMapper`使用`.always()`如果null处理需要

测试更新-[]添加`@ExtendWith(MockitoExtension::class)`测试使用`@Mock`/`@Captor`-[]使用`MockMvc`添加`@AutoConfigureMockMvc`到测试
-[]添加`@AutoConfigureWebTestClient`测试使用`WebTestClient`-[]迁移`TestRestTemplate`→`RestTestClient`（或添加`@AutoConfigureTestRestTemplate`）
-[]更新：`@PropertyMapping`imports→`org.springframework.boot.test.context`###构建配置

- [] Gradle升级到8.5+
-更新Gradle CycloneDX插件到3.0.0+
-[]查看可选的依赖包括在优步jar
-[]删除`loaderImplementation = CLASSIC`如果存在
-[]删除`launchScript()`配置

# # #验证

-[]执行命令`./gradlew clean build`-[]运行完整的测试套件
-[]使用testcontainer验证集成测试
-[]检查新的Kotlin空安全警告
-[]测试弹簧启动执行器端点
-验证健康探针（`/actuator/health/liveness`,`/actuator/health/readiness`）
-[]使用新的默认值进行性能测试

# # #迁移后-[]查看Spring Boot 4.0版本说明了解更多功能
-[]考虑采用Spring Framework 7.0的新特性
[]计划从经典启动器迁移（如果使用）
[]从`spring-boot-jackson2`模块迁移（如果使用）
[]更新CI/CD管道以满足Java 17+的要求
[]更新部署清单（Servlet 6.1容器）

##常见陷阱1. **经典启动器**：记住这些都是不推荐的-计划迁移到特定于技术的启动器
2. **Undertow**：完全删除，没有解决方案-必须使用Tomcat或Jetty
3. **Jackson 3包**：容易丢失`jackson-annotations`仍然使用旧的组ID
4. **MongoDB属性**：许多移到`spring.mongodb.*`，但有些留在`spring.data.mongodb.*`5. **测试配置**:`@SpringBootTest`不再自动配置MockMVC/WebTestClient/TestRestTemplate6. **Kotlin 2.2**：最低要求-旧版本将无法工作
7. **Null-safety**: JSpecify注释可能会在Kotlin中出现新的警告
8. **PropertyMapper**：行为改变与空处理-审查使用
9. **Jersey + Jackson 3**：不兼容-使用`spring-boot-jackson2`模块
10. **健康探测**：现在默认启用-可能影响非kubernetes部署

性能考虑- **模块化启动器**：更小的jar和更快的启动技术特定的启动器
- **Spring Framework 7**：核心框架的性能改进
- **Jackson 3**：改进JSON处理性能
- **虚拟线程**：考虑启用Java 21+ （`spring.threads.virtual.enabled=true`）

# #资源

- [Spring Boot 4.0迁移指南]（https://github.com/spring-projects/spring-boot/wiki/Spring-Boot-4.0-Migration-Guide）
- [Spring Boot 4.0版本说明]（https://github.com/spring-projects/spring-boot/releases）
- [Spring Framework 7.0文档]（https://docs.spring.io/spring-framework/reference/）
- [Jackson 3迁移指南]（https://github.com/FasterXML/jackson/blob/main/jackson3/MIGRATING_TO_JACKSON_3.md）
- [Kotlin 2.2版本说明]（https://kotlinlang.org/docs/whatsnew22.html）

---