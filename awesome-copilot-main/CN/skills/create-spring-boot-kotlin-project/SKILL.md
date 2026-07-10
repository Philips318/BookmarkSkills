---
name: create-spring-boot-kotlin-project
description: 'Create Spring Boot Kotlin Project Skeleton'
---
创建Spring Boot Kotlin项目提示符

-请确保你的系统已安装下列软件：

- Java 21
——码头工人
- Docker撰写

-如果您需要自定义项目名称，请更改[download-spring-boot-project-template]中的`artifactId`和`packageName`（#download-spring-boot-project-template）

-如果需要更新Spring Boot版本，请在[download-spring-boot-project-template]中更改`bootVersion`（#download-spring-boot-project-template）

检查Java版本

—在终端执行如下命令，查看Java版本```shell
java -version
```
下载Spring Boot项目模板

—在终端中执行如下命令下载Spring Boot工程模板```shell
curl https://start.spring.io/starter.zip \
  -d artifactId=${input:projectName:demo-kotlin} \
  -d bootVersion=3.4.5 \
  -d dependencies=configuration-processor,webflux,data-r2dbc,postgresql,data-redis-reactive,data-mongodb-reactive,validation,cache,testcontainers \
  -d javaVersion=21 \
  -d language=kotlin \
  -d packageName=com.example \
  -d packaging=jar \
  -d type=gradle-project-kotlin \
  -o starter.zip
```
##解压缩下载的文件

—在终端中执行如下命令解压缩下载的文件```shell
unzip starter.zip -d ./${input:projectName:demo-kotlin}
```
##删除下载的zip文件

—在终端中执行如下命令，删除下载的zip文件```shell
rm -f starter.zip
```
##解压缩下载的文件

—在终端中执行如下命令解压缩下载的文件```shell
unzip starter.zip -d ./${input:projectName:demo-kotlin}
```
添加额外的依赖项

—在`build.gradle.kts`文件中插入`springdoc-openapi-starter-webmvc-ui`和`archunit-junit5`依赖项```gradle.kts
dependencies {
  implementation("org.springdoc:springdoc-openapi-starter-webflux-ui:2.8.6")
  testImplementation("com.tngtech.archunit:archunit-junit5:1.2.1")
}
```
—将SpringDoc配置信息插入到`application.properties`文件中```properties
# SpringDoc configurations
springdoc.swagger-ui.doc-expansion=none
springdoc.swagger-ui.operations-sorter=alpha
springdoc.swagger-ui.tags-sorter=alpha
```
—将Redis配置信息插入到`application.properties`文件中```properties
# Redis configurations
spring.data.redis.host=localhost
spring.data.redis.port=6379
spring.data.redis.password=rootroot
```
—在“`application.properties`”文件中插入R2DBC配置```properties
# R2DBC configurations
spring.r2dbc.url=r2dbc:postgresql://localhost:5432/postgres
spring.r2dbc.username=postgres
spring.r2dbc.password=rootroot

spring.sql.init.mode=always
spring.sql.init.platform=postgres
spring.sql.init.continue-on-error=true
```
—将MongoDB的配置信息插入到`application.properties`文件中```properties
# MongoDB configurations
spring.data.mongodb.host=localhost
spring.data.mongodb.port=27017
spring.data.mongodb.authentication-database=admin
spring.data.mongodb.username=root
spring.data.mongodb.password=rootroot
spring.data.mongodb.database=test
```
—在项目根目录下创建“`docker-compose.yaml`”，并添加以下服务：`redis:6`、`postgresql:17`和`mongo:8`。

- redis服务应该有    - password `rootroot`
    - mapping port 6379 to 6379
    - mounting volume `./redis_data` to `/data`
- postgresql服务应该有    - password `rootroot`
    - mapping port 5432 to 5432
    - mounting volume `./postgres_data` to `/var/lib/postgresql/data`
-蒙古服务应该有    - initdb root username `root`
    - initdb root password `rootroot`
    - mapping port 27017 to 27017
    - mounting volume `./mongo_data` to `/data/db`
—在“`.gitignore`”文件中插入“`redis_data`”、“`postgres_data`”和“`mongo_data`”目录

—执行gradle clean test命令检查项目是否正常运行```shell
./gradlew clean test
```
—（可选）`docker-compose up -d`：启动服务，`./gradlew spring-boot:run`：运行Spring Boot工程，`docker-compose rm -sf`：停止服务。

让我们一步一步来做。