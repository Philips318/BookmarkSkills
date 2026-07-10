---
name: create-spring-boot-java-project
description: 'Create Spring Boot Java Project Skeleton'
---
创建Spring Boot Java项目提示符

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
  -d artifactId=${input:projectName:demo-java} \
  -d bootVersion=3.4.5 \
  -d dependencies=lombok,configuration-processor,web,data-jpa,postgresql,data-redis,data-mongodb,validation,cache,testcontainers \
  -d javaVersion=21 \
  -d packageName=com.example \
  -d packaging=jar \
  -d type=maven-project \
  -o starter.zip
```
##解压缩下载的文件

—在终端中执行如下命令解压缩下载的文件```shell
unzip starter.zip -d ./${input:projectName:demo-java}
```
##删除下载的zip文件

—在终端中执行如下命令，删除下载的zip文件```shell
rm -f starter.zip
```
将目录更改为项目根目录

—在终端中执行如下命令，将目录修改为工程根目录```shell
cd ${input:projectName:demo-java}
```
添加额外的依赖项

—在`pom.xml`文件中插入`springdoc-openapi-starter-webmvc-ui`和`archunit-junit5`依赖项```xml
<dependency>
  <groupId>org.springdoc</groupId>
  <artifactId>springdoc-openapi-starter-webmvc-ui</artifactId>
  <version>2.8.6</version>
</dependency>
<dependency>
  <groupId>com.tngtech.archunit</groupId>
  <artifactId>archunit-junit5</artifactId>
  <version>1.2.1</version>
  <scope>test</scope>
</dependency>
```
添加SpringDoc， Redis， JPA和MongoDB的配置

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
—在`application.properties`文件中插入JPA配置```properties
# JPA configurations
spring.datasource.driver-class-name=org.postgresql.Driver
spring.datasource.url=jdbc:postgresql://localhost:5432/postgres
spring.datasource.username=postgres
spring.datasource.password=rootroot
spring.jpa.hibernate.ddl-auto=update
spring.jpa.show-sql=true
spring.jpa.properties.hibernate.format_sql=true
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
##添加`docker-compose.yaml`与Redis， PostgreSQL和MongoDB服务

—在项目根目录下创建`docker-compose.yaml`，并添加以下服务：`redis:6`、`postgresql:17`和`mongo:8`。

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
添加`.gitignore`文件

—在“`.gitignore`”文件中插入“`redis_data`”、“`postgres_data`”和“`mongo_data`”目录

##运行Maven test命令

—执行maven clean test命令，检查项目是否正常运行```shell
./mvnw clean test
```
##运行Maven运行命令（可选）

—（可选）`docker-compose up -d`：启动服务，`./mvnw spring-boot:run`：运行Spring Boot工程，`docker-compose rm -sf`：停止服务。

让我们一步一步地做这件事