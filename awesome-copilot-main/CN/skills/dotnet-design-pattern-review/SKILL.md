---
name: dotnet-design-pattern-review
description: 'Review the C#/.NET code for design pattern implementation and suggest improvements.'
---
设计模式回顾

回顾c# /。. NET代码在${selection}中进行设计模式实现并提出改进建议，对于solution/project.不做任何修改，只提供一个回顾。

需要的设计模式

- **命令模式**：泛型基类（`CommandHandler<TOptions>`），`ICommandHandler<TOptions>`接口，`CommandHandlerOptions`继承，静态`SetupCommand(IHost host)`方法
- **工厂模式**：复杂对象创建服务提供商集成
- **依赖注入**：主构造函数语法，`ArgumentNullException`空检查，接口抽象，适当的服务生命周期
- **存储库模式**：连接的异步数据访问接口提供者抽象
- **提供者模式**：外部服务抽象（数据库、人工智能）、清晰契约、配置处理
—**资源模式**:ResourceManager用于本地化消息，独立。resx文件（LogMessages, ErrorMessages）

##检查清单- **设计模式**：确定使用的模式。命令处理程序、工厂、提供程序和存储库模式是否正确实现？缺少有益的模式？
- **架构**：遵循命名空间约定（`{Core|Console|App|Service}.{Feature}`）？Core/Console项目之间的适当分离？模块化和可读？
- * *。. NET最佳实践**：主构造函数，带任务返回的async/await， ResourceManager的使用，结构化日志记录，强类型配置？
- **GoF模式**：命令，工厂，模板方法，策略模式是否正确实现？
- **SOLID原则**：单一责任，Open/Closed， Liskov替换，接口隔离，依赖反转违反？
**性能**：适当的async/await，资源处置，ConfigureAwait(false)，并行处理的机会？
- **可维护性**：清晰的关注点分离，一致的错误处理，正确的配置使用？
- * * Testabi**：通过接口抽象的依赖，可模拟的组件，异步可测试性，AAA模式兼容性？
- **安全**：输入验证，安全凭据处理，参数化查询，安全异常处理？
**文档**：用于公共api的XML文档，parameter/return描述，资源文件组织？
- **代码清晰度**：有意义的名称反映领域的概念，通过模式明确的意图，自我解释的结构？
- **干净的代码**：一致的风格，适当的method/class大小，最小的复杂性，消除重复？改进重点领域

- **命令处理程序**：基类验证，一致的错误处理，适当的资源管理
- **工厂**：依赖配置，服务提供商集成，处理模式
- **提供商**：连接管理，异步模式，异常处理和日志
- **配置**：数据注释，验证属性，安全敏感值处理
-AI/ML集成：语义内核模式，结构化输出处理，模型配置

提供具体的、可操作的建议，以便根据项目的架构和需求进行改进。NET最佳实践。