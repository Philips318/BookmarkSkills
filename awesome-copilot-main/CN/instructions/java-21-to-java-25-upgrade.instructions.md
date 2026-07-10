---
applyTo: ['*']
description: "Comprehensive best practices for adopting new Java 25 features since the release of Java 21."
---
# Java 21到Java 25升级指南

这些说明帮助GitHub Copilot帮助开发人员将Java项目从JDK 21升级到JDK 25，重点关注新的语言特性、API更改和最佳实践。

JDK 22-25中的语言特性和API变化

模式匹配增强（JEP455/488- 23年预览版）

** pattern， instanceof和switch中的基本类型**

使用模式匹配时：
-建议在switch表达式和instanceof检查中使用基本类型模式
—从传统交换机升级举例：```java
// Old approach (Java 21)
switch (x.getStatus()) {
    case 0 -> "okay";
    case 1 -> "warning"; 
    case 2 -> "error";
    default -> "unknown status: " + x.getStatus();
}

// New approach (Java 25 Preview)
switch (x.getStatus()) {
    case 0 -> "okay";
    case 1 -> "warning";
    case 2 -> "error"; 
    case int i -> "unknown status: " + i;
}
```
-启用预览功能与`--enable-preview`标志
-建议更复杂情况下的防护模式：```java
switch (x.getYearlyFlights()) {
    case 0 -> ...;
    case int i when i >= 100 -> issueGoldCard();
    case int i -> ... // handle 1-99 range
}
```
类文件API （JEP466/484-第二次预览将于2009年发布，标准版将于25年发布）

**用标准API替换ASM **

当检测字节码操作或类文件处理时：
-建议从ASM库迁移到标准的Class-File API
—使用`java.lang.classfile`包代替`org.objectweb.asm`—迁移模式示例：```java
// Old ASM approach
ClassReader reader = new ClassReader(classBytes);
ClassWriter writer = new ClassWriter(reader, 0);
// ... ASM manipulation

// New Class-File API approach
ClassModel classModel = ClassFile.of().parse(classBytes);
byte[] newBytes = ClassFile.of().transform(classModel, 
    ClassTransform.transformingMethods(methodTransform));
```
Markdown文档注释（JEP 467 -标准版23）

* * JavaDoc现代化* *

使用JavaDoc注释时：
-建议将html重JavaDoc转换为Markdown语法
使用`///`作为Markdown文档注释
—转换示例：```java
// Old HTML JavaDoc
/**
 * Returns the <b>absolute</b> value of an {@code int} value.
 * <p>
 * If the argument is not negative, return the argument.
 * If the argument is negative, return the negation of the argument.
 * 
 * @param a the argument whose absolute value is to be determined
 * @return the absolute value of the argument
 */

// New Markdown JavaDoc  
/// Returns the **absolute** value of an `int` value.
///
/// If the argument is not negative, return the argument.
/// If the argument is negative, return the negation of the argument.
/// 
/// @param a the argument whose absolute value is to be determined
/// @return the absolute value of the argument
```
派生记录创建（JEP 468 -预览版23）

增强* * * *记录

处理记录时：
—建议使用`with`表达式创建派生记录
-为派生记录创建启用预览功能
-示例模式：```java
// Instead of manual record copying
public record Person(String name, int age, String email) {
    public Person withAge(int newAge) {
        return new Person(name, newAge, email);
    }
}

// Use derived record creation (Preview)
Person updated = person with { age = 30; };
```
流采集器（JEP473/485-第二次预览在23年，标准在25年）

**增强流处理**

当处理复杂的流操作时：
-建议使用`Stream.gather()`自定义中间操作
—内置采集器导入`java.util.stream.Gatherers`—示例用法：```java
// Custom windowing operations
List<List<String>> windows = stream
    .gather(Gatherers.windowSliding(3))
    .toList();

// Custom filtering with state
List<Integer> filtered = numbers.stream()
    .gather(Gatherers.fold(0, (state, element) -> {
        // Custom stateful logic
        return state + element > threshold ? element : null;
    }))
    .filter(Objects::nonNull)
    .toList();
```
迁移警告和弃用

不安全的内存访问方法（JEP 471 -在23中已弃用）

当检测到`sun.misc.Unsafe`使用情况时：
—警告已弃用的内存访问方法
-建议迁移到标准替代方案；```java
// Deprecated: sun.misc.Unsafe memory access
Unsafe unsafe = Unsafe.getUnsafe();
unsafe.getInt(object, offset);

// Preferred: VarHandle API
VarHandle vh = MethodHandles.lookup()
    .findVarHandle(MyClass.class, "fieldName", int.class);
int value = (int) vh.get(object);

// Or for off-heap: Foreign Function & Memory API
MemorySegment segment = MemorySegment.ofArray(new int[10]);
int value = segment.get(ValueLayout.JAVA_INT, offset);
```
JNI使用警告（JEP 472 - warning in 24）

当检测到JNI使用情况时：
-警告即将到来的JNI使用限制
-建议使用JNI的应用程序添加`--enable-native-access`标志
-建议在可能的情况下迁移到外部函数和内存API
-为本机访问添加module-info.java项：```java
module com.example.app {
    requires jdk.unsupported; // for remaining JNI usage
}
```
##垃圾收集更新

ZGC世代模式（JEP 474 -默认在23）

配置垃圾收集时：
默认ZGC现在使用分代模式
-如果显式使用非分代ZGC，则更新JVM标志：```bash
# Explicit non-generational mode (will show deprecation warning)
-XX:+UseZGC -XX:-ZGenerational

# Default generational mode
-XX:+UseZGC
```
G1改进（JEP 475 - 24年实现）

使用G1GC时：
-无需更改代码-内部JVM优化
-可以看到改进的编译性能与C2编译器

矢量API （JEP 469 - 25年第8个孵化器）

处理数值计算时：
-建议SIMD操作的矢量API（仍在酝酿中）
—添加`--add-modules jdk.incubator.vector`—示例用法：```java
import jdk.incubator.vector.*;

// Traditional scalar computation
for (int i = 0; i < a.length; i++) {
    c[i] = a[i] + b[i];
}

// Vectorized computation
var species = IntVector.SPECIES_PREFERRED;
for (int i = 0; i < a.length; i += species.length()) {
    var va = IntVector.fromArray(species, a, i);
    var vb = IntVector.fromArray(species, b, i);
    var vc = va.add(vb);
    vc.intoArray(c, i);
}
```
编译和构建配置

预览功能

对于使用预览功能的项目：
-将`--enable-preview`添加到编译器参数中
-将`--enable-preview`添加到运行时参数中
- Maven配置：```xml
<plugin>
    <groupId>org.apache.maven.plugins</groupId>
    <artifactId>maven-compiler-plugin</artifactId>
    <configuration>
        <release>25</release>
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
        languageVersion = JavaLanguageVersion.of(25)
    }
}

tasks.withType<JavaCompile> {
    options.compilerArgs.add("--enable-preview")
}

tasks.withType<Test> {
    jvmArgs("--enable-preview")
}
```
##迁移策略

###逐步升级过程

1. **更新构建工具**：确保Maven/Gradle支持JDK 25
2. **Update Dependencies**：检查JDK 25的兼容性
3. **处理警告**：解决JEPs471/472的弃用警告
4. **启用预览功能**：如果使用模式匹配或其他预览功能
5. **彻底测试**：特别是对于使用JNI或sun.misc.Unsafe的应用程序
6. **性能测试**：用新的ZGC默认值验证GC行为

代码审查检查表在审查Java 25升级的代码时：
-[]用类文件API代替ASM用法
-[]转换复杂的HTML JavaDoc Markdown
-[]在switch表达式中使用基本模式
-用VarHandle或FFM API替换sun.misc.Unsafe
-[]为JNI使用添加本机访问权限
-[]使用流采集器进行复杂的流操作
-[]更新构建配置预览功能

测试注意事项

-测试与`--enable-preview`标志预览功能
-验证JNI应用程序工作与本机访问警告
新ZGC分代模式下的性能测试
-使用Markdown注释验证JavaDoc生成

##常见陷阱1. **预览功能依赖**：不要在没有明确文档的情况下在库代码中使用预览功能
2. **本机访问：直接或间接使用JNI的应用程序可能需要`--enable-native-access`配置
3. **不安全迁移**：不要延迟从sun.misc.不安全的迁移-弃用警告表示将来删除
4. **模式匹配范围**：基本模式适用于所有基本类型，而不仅仅是int
5. **记录增强**：派生的记录创建需要Java 23中的预览标志

性能考虑

- ZGC分代模式可以提高大多数工作负载的性能
-类文件API减少asm相关开销
—流采集器为复杂的流操作提供更好的性能
G1GC改进减少了JIT编译开销

记住，在将Java 25升级部署到生产系统之前，要在登台环境中进行彻底的测试。