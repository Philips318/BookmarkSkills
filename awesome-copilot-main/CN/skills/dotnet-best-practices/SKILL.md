---
name: dotnet-best-practices
description: 'Ensure .NET/C# code meets best practices for the solution/project.'
---
最佳实践

你的任务是确保${selection}中的.NET/C#代码符合特定于这个solution/project.的最佳实践，这包括：

##文档和结构

-为所有公共类、接口、方法和属性创建全面的XML文档注释
—在XML注释中包含参数说明和返回值说明
—按照已建立的命名空间结构：{Core|Console|App|Service} {Feature}

##设计模式和架构

-使用主构造函数语法进行依赖注入（例如，`public class MyClass(IDependency dependency)`）
-使用泛型基类实现命令处理程序模式（例如，`CommandHandler<TOptions>`）
-使用接口隔离和明确的命名约定（用‘I’作为接口的前缀）
-遵循工厂模式创建复杂对象。

依赖注入和服务-通过ArgumentNullException使用构造函数依赖注入进行null检查
-注册具有适当生命周期的服务（单例、限定范围、瞬态）
-使用Microsoft.Extensions.DependencyInjection模式
—实现可测试性的服务接口

##资源管理和本地化

—使用ResourceManager处理本地化消息和错误字符串
—分离LogMessages和ErrorMessages资源文件
—通过`_resourceManager.GetString("MessageKey")`访问资源

##Async/Await模式

—对于所有I/O操作和长时间运行的任务使用async/await—从异步方法返回Task或Task<T>-在适当的地方使用ConfigureAwait（false）
—正确处理异步异常

##测试标准-对断言使用MSTest框架和FluentAssertions
-遵循AAA模式（安排，行动，主张）
-使用Moq模拟依赖关系
-测试成功和失败的场景
-包括空参数验证测试

##配置和设置

-使用带有数据注释的强类型配置类
-实现验证属性（Required, NotEmptyOrWhitespace）
—使用“配置绑定”进行设置
—支持appsettings.json配置文件

语义内核和AI集成

—使用微软软件。用于AI操作的SemanticKernel
-实现正确的内核配置和服务注册
-处理AI模型设置（聊天完成，嵌入等）
-为可靠的AI响应使用结构化的输出模式

##错误处理和日志-使用结构化日志与Microsoft.Extensions.Logging
-包括有意义的上下文的范围日志记录
-抛出带有描述性消息的特定异常
-在预期的故障场景中使用try-catch块

性能和安全性

-使用c# 12+的特性和。NET 8优化（如适用）
-实施适当的输入验证和处理
—对数据库操作使用参数化查询
-AI/ML操作遵循安全编码实践

代码质量

-确保遵守SOLID原则
-通过基类和实用程序避免代码重复
—使用反映域概念的有意义的名称
-保持方法的重点和凝聚力
——实行合理的资源配置模式