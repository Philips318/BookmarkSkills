---
applyTo: ['*']
description: "Comprehensive best practices for adopting new Java 21 features since the release of Java 17."
---
# Java 17到Java 21升级指南

这些说明帮助GitHub Copilot帮助开发人员将Java项目从JDK 17升级到JDK 21，重点关注新的语言特性、API更改和最佳实践。

JDK 18-21的主要语言特性

模式匹配开关（JEP 441 - 21标准）

**增强的交换机表达式和语句**

当使用开关结构时：
-建议在适当情况下将传统开关转换为模式匹配
-使用模式匹配进行类型检查和解构
—升级模式示例：```java
// Old approach (Java 17)
public String processObject(Object obj) {
    if (obj instanceof String) {
        String s = (String) obj;
        return s.toUpperCase();
    } else if (obj instanceof Integer) {
        Integer i = (Integer) obj;
        return i.toString();
    }
    return "unknown";
}

// New approach (Java 21)
public String processObject(Object obj) {
    return switch (obj) {
        case String s -> s.toUpperCase();
        case Integer i -> i.toString();
        case null -> "null";
        default -> "unknown";
    };
}
```
-支持保护模式：```java
switch (obj) {
    case String s when s.length() > 10 -> "Long string: " + s;
    case String s -> "Short string: " + s;
    case Integer i when i > 100 -> "Large number: " + i;
    case Integer i -> "Small number: " + i;
    default -> "Other";
}
```
记录模式（JEP 440 - 21年标准）

**解构模式匹配中的记录**

处理记录时：
-建议使用记录模式进行解构
—结合switch表达式，实现强大的数据处理
—示例用法：```java
public record Point(int x, int y) {}
public record ColoredPoint(Point point, Color color) {}

// Destructuring in switch
public String describe(Object obj) {
    return switch (obj) {
        case Point(var x, var y) -> "Point at (" + x + ", " + y + ")";
        case ColoredPoint(Point(var x, var y), var color) -> 
            "Colored point at (" + x + ", " + y + ") in " + color;
        default -> "Unknown shape";
    };
}
```
-用于复杂的模式匹配：```java
// Nested record patterns
switch (shape) {
    case Rectangle(ColoredPoint(Point(var x1, var y1), var c1), 
                   ColoredPoint(Point(var x2, var y2), var c2)) 
        when c1 == c2 -> "Monochrome rectangle";
    case Rectangle r -> "Multi-colored rectangle";
}
```
虚拟线程（JEP 444 - 21的标准）

* *轻量级并发* *

当使用并发时：
-建议在高吞吐量、并发应用中使用虚拟线程
—使用“`Thread.ofVirtual()`”创建虚拟线程
—迁移模式示例：```java
// Old platform thread approach
ExecutorService executor = Executors.newFixedThreadPool(100);
executor.submit(() -> {
    // blocking I/O operation
    httpClient.send(request);
});

// New virtual thread approach
try (var executor = Executors.newVirtualThreadPerTaskExecutor()) {
    executor.submit(() -> {
        // blocking I/O operation - now scales to millions
        httpClient.send(request);
    });
}
```
-使用结构化并发模式：```java
// Structured concurrency (Preview)
try (var scope = new StructuredTaskScope.ShutdownOnFailure()) {
    Future<String> user = scope.fork(() -> fetchUser(userId));
    Future<String> order = scope.fork(() -> fetchOrder(orderId));
    
    scope.join();           // Join all subtasks
    scope.throwIfFailed();  // Propagate errors
    
    return processResults(user.resultNow(), order.resultNow());
}
```
字符串模板（JEP 430 - 21版预览）

**安全字符串插值**

处理字符串格式时：
-建议字符串模板的安全字符串插值（预览功能）
-启用预览功能与`--enable-preview`—示例用法：```java
// Traditional concatenation
String message = "Hello, " + name + "! You have " + count + " messages.";

// String Templates (Preview)
String message = STR."Hello, \{name}! You have \{count} messages.";

// Safe HTML generation
String html = HTML."<p>User: \{username}</p>";

// Safe SQL queries  
PreparedStatement stmt = SQL."SELECT * FROM users WHERE id = \{userId}";
```
序列集合（JEP 431 - 21版标准）

**增强的收集接口**

使用集合时：
—使用新的`SequencedCollection`、`SequencedSet`、`SequencedMap`接口
-跨集合类型统一访问first/last元素
—示例用法：```java
// New methods available on Lists, Deques, LinkedHashSet, etc.
List<String> list = List.of("first", "middle", "last");
String first = list.getFirst();  // "first"
String last = list.getLast();    // "last"
List<String> reversed = list.reversed(); // ["last", "middle", "first"]

// Works with any SequencedCollection
SequencedSet<String> set = new LinkedHashSet<>();
set.addFirst("start");
set.addLast("end");
String firstElement = set.getFirst();
```
未命名模式和变量（JEP 443 - 21版预览）

**简化模式匹配**

使用模式匹配时：
-对不需要的值使用未命名模式`_`—简化开关表达式和记录模式
—示例用法：```java
// Ignore unused variables
switch (ball) {
    case RedBall(_) -> "Red ball";     // Don't care about size
    case BlueBall(var size) -> "Blue ball size " + size;
}

// Ignore parts of records
switch (point) {
    case Point(var x, _) -> "X coordinate: " + x; // Ignore Y
    case ColoredPoint(Point(_, var y), _) -> "Y coordinate: " + y;
}

// Exception handling with unnamed variables
try {
    riskyOperation();
} catch (IOException | SQLException _) {
    // Don't need exception details
    handleError();
}
```
范围值（JEP 446 - 21版预览）

**改进上下文传播**

当使用线程本地数据时：
-考虑将作用域值作为ThreadLocal的现代替代品
为虚拟线程提供更好的性能和更清晰的语义
—示例用法：```java
// Define scoped value
private static final ScopedValue<String> USER_ID = ScopedValue.newInstance();

// Set and use scoped value
ScopedValue.where(USER_ID, "user123")
    .run(() -> {
        processRequest(); // Can access USER_ID.get() anywhere in call chain
    });

// In nested method
public void processRequest() {
    String userId = USER_ID.get(); // "user123"
    // Process with user context
}
```
API增强和新功能

默认为UTF-8 （JEP 400 - Standard in 18）

当使用文件I/O时：
- UTF-8现在是所有平台的默认字符集
-删除明确的字符集规格，其中UTF-8的意图
-简化示例：```java
// Old explicit UTF-8 specification
Files.readString(path, StandardCharsets.UTF_8);
Files.writeString(path, content, StandardCharsets.UTF_8);

// New default behavior (Java 18+)
Files.readString(path);  // Uses UTF-8 by default
Files.writeString(path, content);  // Uses UTF-8 by default
```
简单Web服务器（JEP 408 - 18年标准）

当需要基本HTTP服务器时：
-使用内置的`jwebserver`命令或`com.sun.net.httpserver`增强功能
-非常适合测试和开发
—示例用法：```java
// Command line
$ jwebserver -p 8080 -d /path/to/files

// Programmatic usage
HttpServer server = HttpServer.create(new InetSocketAddress(8080), 0);
server.createContext("/", new SimpleFileHandler(Path.of("/tmp")));
server.start();
```
internet地址解析SPI （JEP 418 - 19年标准）

使用自定义DNS解析时：
—自定义地址解析实现`InetAddressResolverProvider`-用于服务发现和测试场景

密钥封装机制API （JEP 452 - 21版标准）

使用后量子密码学时：
—密钥封装机制使用KEM API
—示例用法：```java
KeyPairGenerator kpg = KeyPairGenerator.getInstance("ML-KEM");
KeyPair kp = kpg.generateKeyPair();

KEM kem = KEM.getInstance("ML-KEM");
KEM.Encapsulator encapsulator = kem.newEncapsulator(kp.getPublic());
KEM.Encapsulated encapsulated = encapsulator.encapsulate();
```
弃用和警告

终结弃用（JEP 421 - 18年弃用）

遇到`finalize()`方法时：
-删除finalize方法并使用替代方法
-建议使用Cleaner API或试用资源
—迁移示例：```java
// Deprecated finalize approach
@Override
protected void finalize() throws Throwable {
    cleanup();
}

// Modern approach with Cleaner
private static final Cleaner CLEANER = Cleaner.create();

public MyResource() {
    cleaner.register(this, new CleanupTask(nativeResource));
}

private static class CleanupTask implements Runnable {
    private final long nativeResource;
    
    CleanupTask(long nativeResource) {
        this.nativeResource = nativeResource;
    }
    
    public void run() {
        cleanup(nativeResource);
    }
}
```
动态代理加载（JEP 451 - 21中的警告）

当使用代理或仪器时：
—根据需要添加`-XX:+EnableDynamicAgentLoading`来抑制警告
-考虑在启动时加载代理，而不是动态加载
-更新工具以使用启动代理加载

构建配置更新

预览功能

对于使用预览功能的项目：
-将`--enable-preview`添加到编译器和运行时
- Maven配置：```xml
<plugin>
    <groupId>org.apache.maven.plugins</groupId>
    <artifactId>maven-compiler-plugin</artifactId>
    <configuration>
        <release>21</release>
        <compilerArgs>
            <arg>--enable-preview</arg>
        </compilerArgs>
    </configuration>
</plugin>

<plugin>
    <groupId>org.apache.maven.plugins</groupId>
    <artifactId>maven-surefire-plugin</artifactId>
    <configuration>
        <argLine>--enable-preview</argLine>
    </configuration>
</plugin>
```
Gradle配置：```kotlin
java {
    toolchain {
        languageVersion = JavaLanguageVersion.of(21)
    }
}

tasks.withType<JavaCompile> {
    options.compilerArgs.add("--enable-preview")
}

tasks.withType<Test> {
    jvmArgs("--enable-preview")
}
```
虚拟线程配置

对于使用虚拟线程的应用程序：
-不需要特殊的JVM标志（21的标准特性）
-考虑这些系统属性进行调试：```bash
-Djdk.virtualThreadScheduler.parallelism=N  # Set carrier thread count
-Djdk.virtualThreadScheduler.maxPoolSize=N  # Set max pool size
```
运行时和GC改进

世代ZGC （JEP 439 - 21年发售）

配置垃圾收集时：
-尝试世代ZGC以获得更好的性能
—启用方式：`-XX:+UseZGC -XX:+ZGenerational`-监控分配模式和GC行为

##迁移策略

###逐步升级过程

1. **更新构建工具**：确保Maven/Gradle支持JDK 21
2. **语言特性采用**：
-从开关的模式匹配开始（标准）
-在有利的地方添加记录模式
-考虑为I/O重型应用程序使用虚拟线程
3. **预览功能**：仅在特定用例需要时启用
4. **测试**：全面测试，特别是并发性更改
5. **性能**：新的GC选项的基准测试

代码审查检查表在审查Java 21升级的代码时：
-[]将适当的instanceof链转换为switch表达式
-[]使用记录模式进行数据解构
-[]将ThreadLocal替换为ScopedValues
-[]考虑在高并发场景下使用虚拟线程
-[]删除明确的UTF-8字符集规范
-[]用Cleaner或try-with-resources替换finalize（）方法
-[]对first/last访问模式使用SequencedCollection方法
-[]为正在使用的预览特性添加预览标志

常见迁移模式

1. * *开关增强* *:   ```java
   // From instanceof chains to switch expressions
   if (obj instanceof String s) return processString(s);
   else if (obj instanceof Integer i) return processInt(i);
   // becomes:
   return switch (obj) {
       case String s -> processString(s);
       case Integer i -> processInt(i);
       default -> processDefault(obj);
   };
   ```
2. **虚拟线程采用**：   ```java
   // From platform threads to virtual threads
   Executors.newFixedThreadPool(200)
   // becomes:
   Executors.newVirtualThreadPerTaskExecutor()
   ```
3. **记录模式用法**：   ```java
   // From manual destructuring to record patterns
   if (point instanceof Point p) {
       int x = p.x();
       int y = p.y();
   }
   // becomes:
   if (point instanceof Point(var x, var y)) {
       // use x and y directly
   }
   ```
性能考虑

-虚拟线程擅长阻塞I/O，但可能不利于cpu密集型任务
分代ZGC可以减少大多数应用程序的GC开销
- switch中的模式匹配通常比instanceof链更有效
SequencedCollection方法提供对first/last元素的O(1)访问
—对于虚拟线程，作用域值的开销比ThreadLocal低

##测试建议

-在高并发下测试虚拟线程应用程序
-验证模式匹配涵盖所有预期的情况
-性能测试与代ZGC与其他收集器
-跨不同平台验证UTF-8默认行为
-在生产使用前彻底测试预览功能

记住，只有在特别需要时才启用预览功能，并在部署到生产环境之前在登台环境中进行彻底测试。