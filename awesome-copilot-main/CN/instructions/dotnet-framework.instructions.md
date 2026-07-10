---
description: 'Guidance for working with .NET Framework projects. Includes project structure, C# language version, NuGet management, and best practices.'
applyTo: '**/*.csproj, **/*.cs'
---
#。. NET框架开发

构建和编译需求
-始终使用`msbuild /t:rebuild`来构建解决方案或项目，而不是`dotnet build`##项目文件管理

遗留和sdk风格的项目结构
很多。. NET框架项目使用遗留的非sdk项目格式，这与现代sdk风格的项目有很大不同。但是，sdk风格的项目文件也可以瞄准。. NET框架，如`net48`或`net472`。在应用项目文件指导之前，请检查`.csproj`格式：

- **遗留的非sdk项目**：所有新的源文件**必须**使用`<Compile>`元素显式添加到项目文件（`.csproj`）
-遗留的非sdk项目不会像sdk风格的项目那样自动包含目录中的文件
—例如：`<Compile Include="Path\To\NewFile.cs" />`- ** sdk风格的项目**：如果项目文件有一个`Sdk`属性，使用sdk风格的约定，即使它的目标。微软网络框架
—例如：`<Project Sdk="Microsoft.NET.Sdk">`—使用`<TargetFramework>`，而不是`<TargetFrameworkVersion>`—例如：`<TargetFramework>net48</TargetFramework>`- **遗留项目中没有隐式导入**：与sdk风格的项目不同，遗留的非sdk项目不会自动导入公共命名空间或程序集

- **在遗留项目中构建配置**：包含用于Debug/Release配置的显式`<PropertyGroup>`部分

- **遗留项目中的输出路径**：显式`<OutputPath>`和`<IntermediateOutputPath>`定义

- **遗留项目中的目标框架**：使用`<TargetFrameworkVersion>`而不是`<TargetFramework>`—例如：`<TargetFrameworkVersion>v4.7.2</TargetFrameworkVersion>`NuGet包管理
-安装和更新NuGet包。. NET框架项目是一项复杂的任务，需要对多个文件进行协调更改。因此，**不尝试在本项目中安装或更新NuGet包**。
—相反，如果需要更改NuGet引用，请要求用户使用Visual Studio NuGet包管理器或Visual Studio包管理器控制台安装或更新NuGet包。
—推荐NuGet包时，请确保NuGet包兼容。. NET框架或。. NET标准2.0(不只是。. NET Core或。净5 +)。

c#语言版本为7.3
这个项目仅限于c# 7.3的特性。请避免使用：c# 8.0+特性（不支持）：
-使用声明（`using var stream = ...`）
-等待using语句（`await using var resource = ...`）
—Switch表达式（`variable switch { ... }`）
-空合并赋值（`??=`）
-范围和索引操作符（`array[1..^1]`,`array[^1]`）
-默认接口方法
-结构中的只读成员
—静态局部函数
-可空引用类型（`string?`,`#nullable enable`）

c# 9.0+特性（不支持）：
-记录（`public record Person(string Name)`）
-初始化属性（`{ get; init; }`）
-顶层程序（没有Main方法的程序）
-模式匹配增强
-目标类型的new表达式（`List<string> list = new()`）

c# 10+特性（不支持）：
-全局using语句
-文件作用域命名空间
-记录结构
-所需成员###改用（c# 7.3兼容）：
-传统使用带大括号的语句
—Switch语句，而不是Switch表达式
-显式空检查，而不是空合并赋值
-手动索引的数组切片
-抽象类或接口代替默认的接口方法

环境注意事项（Windows环境）
-使用带有反斜杠的windows样式路径（例如，`C:\path\to\file.cs`）
—建议终端操作时，使用适合windows操作系统的命令
-在处理文件系统操作时考虑windows特有的行为

##普通。。NET框架陷阱和最佳实践Async/Await模式
- **ConfigureAwait(false)**：在库代码中始终使用`ConfigureAwait(false)`来避免死锁：  ```csharp
  var result = await SomeAsyncMethod().ConfigureAwait(false);
  ```
**避免sync-over-async**：不要使用`.Result`或`.Wait()`或`.GetAwaiter().GetResult()`。这些同步高于异步的模式可能导致死锁和较差的性能。对于异步调用总是使用`await`。

DateTime处理
- **使用DateTimeOffset时间戳**：对于绝对时间点，`DateTimeOffset`优于`DateTime`- **指定日期时间类型**：当使用`DateTime`时，总是指定`DateTimeKind.Utc`或`DateTimeKind.Local`- **文化感知格式**：使用`CultureInfo.InvariantCulture`代替serialization/parsing字符串操作
- **StringBuilder用于连接**：使用`StringBuilder`用于多个字符串连接
- **StringComparison**：对于字符串操作总是指定`StringComparison`：  ```csharp
  string.Equals(other, StringComparison.OrdinalIgnoreCase)
  ```
内存管理
—**处置模式**：对非托管资源正确实现`IDisposable`- **Using语句**：总是在Using语句中包装`IDisposable`对象
- **避免大对象堆**：将对象保持在85KB以下，以避免LOH分配

# # #配置
—**使用ConfigurationManager**：通过`ConfigurationManager.AppSettings`访问应用程序设置
- **连接字符串**：存储在`<connectionStrings>`节，而不是`<appSettings>`- **转换**：使用web.config/app.config转换环境特定的设置

异常处理
—**特定异常**：捕获特定的异常类型，而不是通用的`Exception`- **不要吞下异常**：总是记录或适当地重新抛出异常
- **使用使用一次性资源**：确保正确的清理，即使发生异常性能考虑
- **避免装箱**：注意值类型和泛型中的boxing/unboxing- **字符串实习**：使用`string.Intern()`明智的经常使用的字符串
- **延迟初始化**：使用`Lazy<T>`为昂贵的对象创建
- **避免在热路径反射**：缓存`MethodInfo`，`PropertyInfo`对象时，可能的