---
name: javax-to-jakarta-migration
description: "Migrate Java code from javax.* to jakarta.* namespace. Use when upgrading to Tomcat 11, Jakarta EE 10, or when javax imports are detected in the codebase."
argument-hint: "File, package, or module to migrate"
---
# javax→雅加达迁移技能

##何时使用
升级到Tomcat 11 / Jakarta EE 10+
—代码审查检测`javax.*`导入
—将现有项目迁移到jakarta命名空间

# #过程

###步骤1 -扫描javax使用
在代码库中搜索所有需要迁移的`javax.*`导入：```
javax.servlet.*      → jakarta.servlet.*
javax.persistence.*  → jakarta.persistence.*
javax.validation.*   → jakarta.validation.*
javax.annotation.*   → jakarta.annotation.*
javax.inject.*       → jakarta.inject.*
javax.enterprise.*   → jakarta.enterprise.*
javax.faces.*        → jakarta.faces.*
javax.ws.rs.*        → jakarta.ws.rs.*
javax.el.*           → jakarta.el.*
javax.json.*         → jakarta.json.*
javax.mail.*         → jakarta.mail.*
javax.websocket.*    → jakarta.websocket.*
```
**不要迁移**这些（它们仍然在`javax.*`）：
—`javax.sql.*`—JDK的一部分`javax.naming.*`- JDK的一部分（JNDI）
—`javax.crypto.*`—JDK的一部分
-`javax.net.*`- JDK的一部分
—`javax.security.auth.*`—JDK的一部分
—`javax.swing.*`、`javax.xml.parsers.*`—JDK包

###步骤2 -更新pom.xml替换依赖坐标：

|旧|新||-----|-----|
|`javax.servlet:javax.servlet-api`|`jakarta.servlet:jakarta.servlet-api:6.0.0`|
|`javax.persistence:javax.persistence-api`|`jakarta.persistence:jakarta.persistence-api:3.1.0`|
|`javax.validation:validation-api`|`jakarta.validation:jakarta.validation-api:3.0.2`|
|`javax.annotation:javax.annotation-api`|`jakarta.annotation:jakarta.annotation-api:2.1.1`|

###步骤3 -更新web.xml（如果存在）```xml
<!-- Old namespace -->
<web-app xmlns="http://xmlns.jcp.org/xml/ns/javaee" version="4.0">

<!-- New namespace -->
<web-app xmlns="https://jakarta.ee/xml/ns/jakartaee" version="6.0">
```
###步骤4 -更新Java源文件
将所有`javax.`导入替换为`.java`文件中的`jakarta.`等量物。

###步骤5 -验证
1. 运行`mvn clean compile`或`gradlew build`-修复任何编译错误
2. 运行`mvn test`或`gradlew test`—确保所有测试都通过
3. 搜索任何剩余的`javax.*`导入（不包括JDK包）

# # #输出
提供迁移摘要，列出所有更改的文件、替换的导入以及所需的任何手动步骤。