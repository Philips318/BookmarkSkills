---
applyTo: ["*"]
description: "Comprehensive best practices for adopting new Java 17 features since the release of Java 11."
---
# Java 11到Java 17升级指南

##项目背景

本指南为将Java项目从JDK 11升级到JDK 17提供了全面的GitHub Copilot说明，涵盖了主要的语言特性、API更改和基于这些版本之间集成的47个jep的迁移模式。

语言特性和API变化

JEP 395：记录（Java 16）

**迁移模式**：将数据类转换为记录```java
// Old: Traditional data class
public class Person {
    private final String name;
    private final int age;

    public Person(String name, int age) {
        this.name = name;
        this.age = age;
    }

    public String name() { return name; }
    public int age() { return age; }

    @Override
    public boolean equals(Object obj) { /* boilerplate */ }
    @Override
    public int hashCode() { /* boilerplate */ }
    @Override
    public String toString() { /* boilerplate */ }
}

// New: Record (Java 16+)
public record Person(String name, int age) {
    // Compact constructor for validation
    public Person {
        if (age < 0) throw new IllegalArgumentException("Age cannot be negative");
    }

    // Custom methods can be added
    public boolean isAdult() {
        return age >= 18;
    }
}
```
JEP 409：密封类（Java 17）

**迁移模式**：使用密封类进行受限继承```java
// New: Sealed class hierarchy
public sealed class Shape
    permits Circle, Rectangle, Triangle {

    public abstract double area();
}

public final class Circle extends Shape {
    private final double radius;

    public Circle(double radius) {
        this.radius = radius;
    }

    @Override
    public double area() {
        return Math.PI * radius * radius;
    }
}

public final class Rectangle extends Shape {
    private final double width, height;

    public Rectangle(double width, double height) {
        this.width = width;
        this.height = height;
    }

    @Override
    public double area() {
        return width * height;
    }
}

public non-sealed class Triangle extends Shape {
    // Non-sealed allows further inheritance
    private final double base, height;

    public Triangle(double base, double height) {
        this.base = base;
        this.height = height;
    }

    @Override
    public double area() {
        return 0.5 * base * height;
    }
}
```
### JEP 394: instanceof模式匹配（Java 16）

迁移模式：简化instanceof检查```java
// Old: Traditional instanceof with casting
public String processObject(Object obj) {
    if (obj instanceof String) {
        String str = (String) obj;
        return str.toUpperCase();
    } else if (obj instanceof Integer) {
        Integer num = (Integer) obj;
        return "Number: " + num;
    } else if (obj instanceof List<?>) {
        List<?> list = (List<?>) obj;
        return "List with " + list.size() + " elements";
    }
    return "Unknown type";
}

// New: Pattern matching for instanceof (Java 16+)
public String processObject(Object obj) {
    if (obj instanceof String str) {
        return str.toUpperCase();
    } else if (obj instanceof Integer num) {
        return "Number: " + num;
    } else if (obj instanceof List<?> list) {
        return "List with " + list.size() + " elements";
    }
    return "Unknown type";
}

// Works great with sealed classes
public String describeShape(Shape shape) {
    if (shape instanceof Circle circle) {
        return "Circle with radius " + circle.radius();
    } else if (shape instanceof Rectangle rect) {
        return "Rectangle " + rect.width() + "x" + rect.height();
    } else if (shape instanceof Triangle triangle) {
        return "Triangle with base " + triangle.base();
    }
    return "Unknown shape";
}
```
JEP 361: Switch Expressions （Java 14）

**迁移模式**：将switch语句转换为表达式```java
// Old: Traditional switch statement
public String getDayType(DayOfWeek day) {
    String result;
    switch (day) {
        case MONDAY:
        case TUESDAY:
        case WEDNESDAY:
        case THURSDAY:
        case FRIDAY:
            result = "Workday";
            break;
        case SATURDAY:
        case SUNDAY:
            result = "Weekend";
            break;
        default:
            throw new IllegalArgumentException("Unknown day: " + day);
    }
    return result;
}

// New: Switch expression (Java 14+)
public String getDayType(DayOfWeek day) {
    return switch (day) {
        case MONDAY, TUESDAY, WEDNESDAY, THURSDAY, FRIDAY -> "Workday";
        case SATURDAY, SUNDAY -> "Weekend";
    };
}

// With yield for complex logic
public int calculateScore(Grade grade) {
    return switch (grade) {
        case A -> 100;
        case B -> 85;
        case C -> 70;
        case D -> {
            System.out.println("Consider improvement");
            yield 55;
        }
        case F -> {
            System.out.println("Needs retake");
            yield 0;
        }
    };
}
```
JEP 406：模式匹配开关（Java 17预览版）

**迁移模式**：增强开关模式（预览功能）```java
// Requires --enable-preview flag
public String formatValue(Object obj) {
    return switch (obj) {
        case String s -> "String: " + s;
        case Integer i -> "Integer: " + i;
        case null -> "null value";
        case default -> "Unknown: " + obj.getClass().getSimpleName();
    };
}

// With guarded patterns
public String categorizeNumber(Object obj) {
    return switch (obj) {
        case Integer i when i < 0 -> "Negative integer";
        case Integer i when i == 0 -> "Zero";
        case Integer i when i > 0 -> "Positive integer";
        case Double d when d.isNaN() -> "Not a number";
        case Number n -> "Other number: " + n;
        case null -> "null";
        case default -> "Not a number";
    };
}
```
JEP 378：文本块（Java 15）

**迁移模式**：对多行字符串使用文本块```java
// Old: Concatenated strings
String html = "<html>\n" +
              "  <body>\n" +
              "    <h1>Hello World</h1>\n" +
              "    <p>Welcome to Java 17!</p>\n" +
              "  </body>\n" +
              "</html>";

String sql = "SELECT p.id, p.name, p.email, " +
             "       a.street, a.city, a.state " +
             "FROM person p " +
             "JOIN address a ON p.address_id = a.id " +
             "WHERE p.active = true " +
             "ORDER BY p.name";

// New: Text blocks (Java 15+)
String html = """
              <html>
                <body>
                  <h1>Hello World</h1>
                  <p>Welcome to Java 17!</p>
                </body>
              </html>
              """;

String sql = """
             SELECT p.id, p.name, p.email,
                    a.street, a.city, a.state
             FROM person p
             JOIN address a ON p.address_id = a.id
             WHERE p.active = true
             ORDER BY p.name
             """;

// With string interpolation methods
String json = """
              {
                "name": "%s",
                "age": %d,
                "city": "%s"
              }
              """.formatted(name, age, city);
```
### JEP 358：有用的NullPointerExceptions （Java 14）

**迁移指南**：更好的NPE调试（在Java 17中默认启用）```java
// Old NPE message: "Exception in thread 'main' java.lang.NullPointerException"
// New NPE message shows exactly what was null:
// "Cannot invoke 'String.length()' because the return value of 'Person.getName()' is null"

public class PersonProcessor {
    public void processPersons(List<Person> persons) {
        // This will show exactly which person.getName() returned null
        persons.stream()
            .mapToInt(person -> person.getName().length())  // Clear NPE if getName() returns null
            .sum();
    }

    // Better error messages help with complex expressions
    public void complexExample(Map<String, List<Person>> groups) {
        // NPE will show exactly which part of the chain is null
        int totalNameLength = groups.get("admins")
                                  .get(0)
                                  .getName()
                                  .length();
    }
}
```
JEP 371：隐藏类（Java 15）

迁移模式：用于框架和代理生成```java
// For frameworks creating dynamic proxies
public class DynamicProxyExample {
    public static <T> T createProxy(Class<T> interfaceClass, InvocationHandler handler) {
        // Hidden classes provide better encapsulation for dynamically generated classes
        MethodHandles.Lookup lookup = MethodHandles.lookup();

        // Framework code would use hidden classes for better isolation
        // This is typically handled by frameworks, not application code
        return interfaceClass.cast(
            Proxy.newProxyInstance(
                interfaceClass.getClassLoader(),
                new Class<?>[]{interfaceClass},
                handler
            )
        );
    }
}
```
JEP 334: JVM常量API （Java 12）

迁移模式：用于编译时常量```java
import java.lang.constant.*;

// For advanced metaprogramming and tooling
public class ConstantExample {
    // Use dynamic constants for computed values
    public static final DynamicConstantDesc<String> COMPUTED_CONSTANT =
        DynamicConstantDesc.of(
            ConstantDescs.BSM_INVOKE,
            "computeValue",
            ConstantDescs.CD_String
        );

    // Primarily used by compiler and framework developers
    public static String computeValue() {
        return "Computed at runtime, cached as constant";
    }
}
```
JEP 415：上下文特定的反序列化过滤器（Java 17）

**迁移模式**：增强对象反序列化的安全性```java
import java.io.*;

public class SecureDeserialization {
    // Set up deserialization filters for security
    public static void setupSerializationFilters() {
        // Global filter
        ObjectInputFilter globalFilter = ObjectInputFilter.Config.createFilter(
            "java.base/*;java.util.*;!*"
        );
        ObjectInputFilter.Config.setSerialFilter(globalFilter);
    }

    public <T> T deserializeSecurely(byte[] data, Class<T> expectedType) throws IOException, ClassNotFoundException {
        try (ByteArrayInputStream bis = new ByteArrayInputStream(data);
             ObjectInputStream ois = new ObjectInputStream(bis)) {

            // Context-specific filter
            ObjectInputFilter contextFilter = ObjectInputFilter.Config.createFilter(
                expectedType.getName() + ";java.lang.*;!*"
            );
            ois.setObjectInputFilter(contextFilter);

            return expectedType.cast(ois.readObject());
        }
    }
}
```
JEP 356：增强的伪随机数生成器（Java 17）

**迁移模式**：使用新的随机生成器接口```java
import java.util.random.*;

// Old: Limited Random class
Random oldRandom = new Random();
int oldValue = oldRandom.nextInt(100);

// New: Enhanced random generators (Java 17+)
RandomGenerator generator = RandomGeneratorFactory
    .of("Xoshiro256PlusPlus")
    .create(System.nanoTime());

RandomGenerator.SplittableGenerator splittableGenerator =
    RandomGeneratorFactory.of("L64X128MixRandom").create();

// Better for parallel processing
splittableGenerator.splits(4)
    .parallel()
    .mapToInt(rng -> rng.nextInt(1000))
    .forEach(System.out::println);

// Streamable random values
generator.ints(10, 1, 101)
    .forEach(System.out::println);
```
##I/O和网络改进

JEP 380: Unix-Domain Socket Channels （Java 16）

**迁移模式**：本地IPC使用Unix域套接字```java
import java.net.UnixDomainSocketAddress;
import java.nio.channels.*;

// Old: TCP sockets for local communication
// ServerSocketChannel server = ServerSocketChannel.open();
// server.bind(new InetSocketAddress("localhost", 8080));

// New: Unix domain sockets (Java 16+)
public class UnixSocketExample {
    public void createUnixDomainServer() throws IOException {
        Path socketPath = Path.of("/tmp/my-app.socket");
        UnixDomainSocketAddress address = UnixDomainSocketAddress.of(socketPath);

        try (ServerSocketChannel server = ServerSocketChannel.open(StandardProtocolFamily.UNIX)) {
            server.bind(address);

            while (true) {
                try (SocketChannel client = server.accept()) {
                    // Handle client connection
                    handleClient(client);
                }
            }
        }
    }

    public void connectToUnixSocket() throws IOException {
        Path socketPath = Path.of("/tmp/my-app.socket");
        UnixDomainSocketAddress address = UnixDomainSocketAddress.of(socketPath);

        try (SocketChannel client = SocketChannel.open(address)) {
            // Communicate with server
            ByteBuffer buffer = ByteBuffer.allocate(1024);
            client.read(buffer);
        }
    }

    private void handleClient(SocketChannel client) throws IOException {
        ByteBuffer buffer = ByteBuffer.allocate(1024);
        int bytesRead = client.read(buffer);
        // Process client data
    }
}
```
JEP 352：非易失性映射字节缓冲区（Java 14）

**迁移模式**：用于持久内存操作```java
import java.nio.MappedByteBuffer;
import java.nio.channels.FileChannel;
import java.nio.file.StandardOpenOption;

public class PersistentMemoryExample {
    public void usePersistentMemory() throws IOException {
        Path nvmFile = Path.of("/mnt/pmem/data.bin");

        try (FileChannel channel = FileChannel.open(nvmFile,
                StandardOpenOption.READ,
                StandardOpenOption.WRITE,
                StandardOpenOption.CREATE)) {

            // Map as persistent memory
            MappedByteBuffer buffer = channel.map(
                FileChannel.MapMode.READ_WRITE, 0, 1024,
                ExtendedMapMode.READ_WRITE_SYNC
            );

            // Write data that persists across crashes
            buffer.putLong(0, System.currentTimeMillis());
            buffer.putInt(8, 12345);

            // Force write to persistent storage
            buffer.force();
        }
    }
}
```
##构建系统配置

Maven配置```xml
<properties>
    <maven.compiler.source>17</maven.compiler.source>
    <maven.compiler.target>17</maven.compiler.target>
    <maven.compiler.release>17</maven.compiler.release>
</properties>

<build>
    <plugins>
        <plugin>
            <groupId>org.apache.maven.plugins</groupId>
            <artifactId>maven-compiler-plugin</artifactId>
            <version>3.11.0</version>
            <configuration>
                <release>17</release>
                <!-- Enable preview features if using JEP 406 -->
                <compilerArgs>
                    <arg>--enable-preview</arg>
                </compilerArgs>
            </configuration>
        </plugin>

        <!-- For running tests with preview features -->
        <plugin>
            <groupId>org.apache.maven.plugins</groupId>
            <artifactId>maven-surefire-plugin</artifactId>
            <version>3.0.0</version>
            <configuration>
                <argLine>--enable-preview</argLine>
            </configuration>
        </plugin>
    </plugins>
</build>
```
Gradle配置```kotlin
java {
    toolchain {
        languageVersion = JavaLanguageVersion.of(17)
    }
}

tasks.withType<JavaCompile> {
    options.release.set(17)
    // Enable preview features if needed
    options.compilerArgs.addAll(listOf("--enable-preview"))
}

tasks.withType<Test> {
    useJUnitPlatform()
    // Enable preview features for tests
    jvmArgs("--enable-preview")
}
```
弃用和删除

### JEP 411：删除安全管理器

**迁移模式**：删除安全管理器依赖项```java
// Old: Using Security Manager
SecurityManager sm = System.getSecurityManager();
if (sm != null) {
    sm.checkPermission(new RuntimePermission("shutdownHooks"));
}

// New: Alternative security approaches
// Use application-level security, containers, or process isolation
// Most applications don't need Security Manager functionality
```
### JEP 398：移除Applet API

**迁移模式**：从applet迁移到现代web技术```java
// Old: Java Applet (deprecated)
public class MyApplet extends Applet {
    @Override
    public void start() {
        // Applet code
    }
}

// New: Modern alternatives
// 1. Convert to standalone Java application
public class MyApplication extends JFrame {
    public MyApplication() {
        setTitle("My Application");
        setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
        // Application code
    }

    public static void main(String[] args) {
        SwingUtilities.invokeLater(() -> {
            new MyApplication().setVisible(true);
        });
    }
}

// 2. Use Java Web Start alternative (jlink)
// 3. Convert to web application using modern frameworks
```
### JEP 372：删除Nashorn JavaScript引擎

**迁移模式**：使用替代的JavaScript引擎```java
// Old: Nashorn (removed in Java 17)
// ScriptEngine engine = new ScriptEngineManager().getEngineByName("nashorn");

// New: Alternative approaches
// 1. Use GraalVM JavaScript engine
ScriptEngine engine = new ScriptEngineManager().getEngineByName("graal.js");

// 2. Use external JavaScript execution
ProcessBuilder pb = new ProcessBuilder("node", "script.js");
Process process = pb.start();

// 3. Use web-based approach or embedded browser
```
JVM和性能改进

JEP 377: ZGC—可伸缩的低延迟垃圾收集器（Java 15）

**迁移模式**：为低延迟应用程序启用ZGC```bash
# Enable ZGC
-XX:+UseZGC
-XX:+UnlockExperimentalVMOptions  # Not needed in Java 17

# Monitor ZGC performance
-XX:+LogVMOutput
-XX:LogFile=gc.log
```
### JEP 379: Shenandoah -一个低暂停时间的垃圾收集器（Java 15）

**迁移模式**：启用Shenandoah以获得一致的延迟```bash
# Enable Shenandoah
-XX:+UseShenandoahGC
-XX:+UnlockExperimentalVMOptions  # Not needed in Java 17

# Shenandoah tuning
-XX:ShenandoahGCHeuristics=adaptive
```
JEP 341：默认的CDS档案（Java 12）和JEP 350：动态的CDS档案（Java 13）

**迁移模式**：提高启动性能```bash
# CDS is enabled by default, but you can create custom archives
# Create custom CDS archive
java -XX:DumpLoadedClassList=classes.lst -cp myapp.jar com.example.Main
java -Xshare:dump -XX:SharedClassListFile=classes.lst -XX:SharedArchiveFile=myapp.jsa -cp myapp.jar

# Use custom CDS archive
java -XX:SharedArchiveFile=myapp.jsa -cp myapp.jar com.example.Main
```
测试和迁移策略

第一阶段：打底（1-2周）

1. **更新构建系统**

—修改Java 17的Maven/Gradle配置
-更新CI/CD管道
-验证依赖项兼容性

2. **地址删除和弃用
-删除JavaScript引擎使用
-替换不支持的Applet api
-更新安全管理器的使用情况

阶段2：语言特性（第3-4周）

1. * * * *实施记录

—将数据类转换为记录
-在紧凑构造函数中添加验证
—测试序列化兼容性

2. **添加模式匹配**
-转换instanceof链
-实现类型安全的类型转换模式

阶段3：高级功能（5-6周）

1. * * * *转换表达式

—将switch语句转换为表达式
-使用新的箭头语法
-实现复杂的yield逻辑2. * * * *的文本块
—替换连接的多行字符串
—更新SQL和HTML的生成
-使用格式化方法

第四阶段：密封课程（第7-8周）

1. 设计密封的层次结构

-确定继承限制
-实现密封类模式
—与模式匹配结合

2. **测试和验证**
-全面的测试覆盖率
-性能基准测试
-兼容性验证

性能考虑

记录vs传统类

—记录更节省内存
—更快的创建和相等性检查
—支持自动序列化
—考虑数据传输对象

模式匹配性能

-消除冗余的类型检查
减少施法开销
更好的JVM优化机会
-与密封类一起使用，以达到耗尽性

###切换表达式优化-更高效的字节码生成
-更好的恒定折叠
改进分支预测
-用于复杂的条件逻辑

最佳实践

1. **使用数据类记录**

-不可变数据容器
—API数据传输对象
—配置对象

2. **策略性地应用模式匹配**

-替换instanceof链
与密封类一起使用
—与交换机表达式结合使用

3. **多行内容采用文本块**

- SQL查询
- JSON模板
- HTML内容
—配置文件

4. **采用密封类设计**

-领域建模
-状态机
—代数数据类型
- API演化控制

5. **利用增强的随机生成器**
-并行处理场景
-高质量随机数
-统计应用
-游戏和模拟这个全面的指南使GitHub Copilot能够在将Java 11项目升级到Java 17时提供上下文合适的建议，重点关注语言增强、API改进和现代Java开发实践。