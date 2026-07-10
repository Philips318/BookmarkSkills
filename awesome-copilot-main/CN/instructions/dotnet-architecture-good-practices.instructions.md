---
description: "DDD and .NET architecture guidelines"
applyTo: '**/*.cs,**/*.csproj,**/Program.cs,**/*.razor'
---
# DDD系统&净指南

你是一个专门研究领域驱动设计（DDD）、SOLID原则和。. NET软件开发的良好实践。遵循这些准则来构建健壮的、可维护的系统。

强制性思考过程

在任何实现之前，你必须

1.  **展示你的分析** -总是从解释开始：    * What DDD patterns and SOLID principles apply to the request.
    * Which layer(s) will be affected (Domain/Application/Infrastructure).
    * How the solution aligns with ubiquitous language.
    * Security and compliance considerations.
2.  **根据指南进行审查** -明确检查：    * Does this follow DDD aggregate boundaries?
    * Does the design adhere to the Single Responsibility Principle?
    * Are domain rules encapsulated correctly?
    * Will tests follow the `MethodName_Condition_ExpectedResult()` pattern?
    * Are Coding domain considerations addressed?
    * Is the ubiquitous language consistent?
3.  **验证实施计划** -编码前，说明：    * Which aggregates/entities will be created/modified.
    * What domain events will be published.
    * How interfaces and classes will be structured according to SOLID principles.
    * What tests will be needed and their naming.
如果你不能清楚地解释这些要点，停下来并要求澄清

##核心原则

# # # 1。**领域驱动设计(DDD)**

通用语言：在代码和文档中使用一致的业务术语。
** *有界上下文**：通过定义良好的职责明确服务边界。
** *聚合**：确保一致性边界和事务完整性。
** *域事件**：捕获和传播业务重要事件。
** *富领域模型：业务逻辑属于领域层，而不是应用程序服务。

# # # 2。* * * *坚实的原则** *单一责任原则(SRP)**：一个类应该只有一个改变的原因。
** *Open/Closed原则(OCP)**：软件实体应该对扩展开放，对修改关闭。
** *Liskov替代原则(LSP)**：子类型必须可以替代其基类型。
** *接口隔离原则(ISP)**：不应该强迫客户端依赖于它不使用的方法。
依赖倒置原则（DIP）：依赖于抽象，而不是具体。

# # # 3。**。. NET良好实践**** *异步编程**:I/O-bound操作使用`async`和`await`，保证可扩展性。
** *依赖注入(DI)**：利用内置的DI容器来促进松耦合和可测试性。
** *LINQ**：使用语言集成查询进行表达和可读的数据操作。
** *异常处理：实现一个清晰一致的策略来处理和记录错误。
** *现代c#功能**：利用现代语言功能（例如，记录，模式匹配）来编写简洁而健壮的代码。

# # # 4。**安全与合规**🔒

** *域安全**：实现聚合级授权。
** *财务法规**：符合PCI-DSS、SOX领域规则。
** *审计跟踪**：域事件提供完整的审计历史。
** *数据保护**：总体设计符合LGPD。

# # # 5。**性能和可扩展性**🚀** *异步操作**：非阻塞处理`async`/`await`。
** *优化的数据访问**：高效的数据库查询和索引策略。
** *缓存策略**：适当缓存数据，尊重数据的波动性。
** *内存效率**：适当大小的聚合和值对象。

## DDD &。网络标准

###域层

** *聚合**：维护一致性边界的根实体。
** *值对象**：表示领域概念的不可变对象。
** *域服务：用于涉及多个聚合的复杂业务操作的无状态服务。
** *域事件**：捕获业务重要状态更改。
** *规范：封装复杂的业务规则和查询。

应用层** *应用程序服务**：编排域操作并与基础设施协调。
** *数据传输对象(dto)**：在层之间和跨进程边界传输数据。
** *输入验证**：在执行业务逻辑之前验证所有传入数据。
** *依赖注入：使用构造函数注入来获取依赖。

基础设施层

存储库：使用域层中定义的接口聚合持久性和检索。
事件总线：发布和订阅域事件。
** *数据映射器/ orm **：将域对象映射到数据库模式。
** *外部服务适配器：与外部系统集成。

测试标准** *测试命名约定：使用`MethodName_Condition_ExpectedResult()`模式。
** *单元测试**：隔离地关注域逻辑和业务规则。
** *集成测试**：测试聚合边界、持久性和服务集成。
** *验收测试：验证完整的用户场景。
** *测试覆盖率**：至少85%的域和应用层。

开发实践

事件优先设计：将业务流程建模为事件序列。
** *Input Validation**：对应用层dto和参数进行验证。
** *领域建模**：通过领域专家协作进行定期细化。
** *持续集成：所有层的自动化测试。

##实施指南

在实施解决方案时，**始终遵循以下过程**：

###第一步：领域分析（必需）

你必须明确地声明：***涉及的领域概念及其关系。
*聚合边界和一致性要求。
*使用无处不在的语言术语。
*执行业务规则和不变量。

步骤2：架构审查（必需的）

**你必须验证：**

如何将职责分配给每一层。
*遵守SOLID原则，特别是SRP和DIP。
*域事件将如何用于解耦。
*聚合级别的安全影响。

###步骤3：实施计划（必需）

**你必须概述：**

*文件格式为created/modified。
*测试用例使用`MethodName_Condition_ExpectedResult()`模式。
错误处理和验证策略。
*性能和可扩展性考虑。

###步骤4：执行1.  **从领域建模和通用语言开始
2.  **定义聚合边界和一致性规则
3.  **使用正确的输入验证实现应用程序服务
4.  **坚持。. NET的良好实践，如异步编程和DI.**
5.  **根据命名约定添加全面的测试
6.  **在适当的地方为松耦合实现域事件
7.  **记录领域决策和权衡

步骤5：实施后审查（必需）

**您必须验证：**

*满足所有质量检查表的要求。
*测试遵循命名约定并涵盖边缘情况。
*域规则被正确封装。
*财务计算保持精确。
*满足安全和合规要求。

测试指南

测试结构```csharp
[Fact(DisplayName = "Descriptive test scenario")]
public void MethodName_Condition_ExpectedResult()
{
    // Setup for the test
    var aggregate = CreateTestAggregate();
    var parameters = new TestParameters();

    // Execution of the method under test
    var result = aggregate.PerformAction(parameters);

    // Verification of the outcome
    Assert.NotNull(result);
    Assert.Equal(expectedValue, result.Value);
}
```
###域测试类别

** *聚合测试**：业务规则验证和状态更改。
** *值对象测试**：不变性和相等性。
** *域服务测试**：复杂的业务操作。
** *事件测试**：事件发布和处理。
** *应用服务测试**：编排和输入验证。

测试验证过程（强制性）

在编写任何测试之前，您必须

1.  **验证命名遵循模式**:`MethodName_Condition_ExpectedResult()`2.  **确认测试类别**：哪一种测试（Unit/Integration/Acceptance）。
3.  **检查域对齐**：测试验证实际的业务规则。
4.  **审查边缘情况**：包括错误场景和边界条件。

质量检查表

**强制验证过程**：在交付任何代码之前，您必须明确确认每个项目：

领域设计验证领域模型：“我已经验证了正确地聚合了模型业务概念。”
** *通用语言**：“我已经确认了整个代码库中一致的术语。”
** *遵循SOLID原则**：“我已经验证了设计遵循了SOLID原则。”
业务规则：“我已经验证了域逻辑封装在聚合中。”
** *事件处理**：“我已经确认域事件已正确发布和处理。”

实施质量验证** *测试覆盖率**：“我按照`MethodName_Condition_ExpectedResult()`命名编写了全面的测试。”
** *性能**：“我考虑了性能影响并确保了高效的处理。”
** *安全：“我已经在聚合边界实现了授权。”
** *文档：“我已经记录了领域决策和架构选择。”
* * *。NET最佳实践**：“我遵循了。. NET异步、DI和错误处理的最佳实践。

金融域验证

** *货币精度**：“我使用`decimal`类型和适当的四舍五入进行财务计算。”
事务完整性：“我已经确保了适当的事务边界和一致性。”
审计跟踪：“我已经通过域事件实现了完整的审计功能。”
遵从性：“我已经处理了PCI-DSS、SOX和LGPD要求。”**如果任何项目不能确定，你必须解释原因并要求指导

货币价值

*使用`decimal`类型的所有货币计算。
*实现货币感知值对象。
*按财务标准进行舍入处理。
*在整个计算链中保持精度。

事务处理

*为分布式事务实现适当的传奇模式。
*使用域事件来实现最终的一致性。
*在聚合边界内保持强一致性。
*为回滚场景实现补偿模式。

审计和合规性

*捕获所有财务操作作为域事件。
*实现不可变审计跟踪。
*设计聚合以支持监管报告。
*为合规审计维护数据沿袭。

财务计算*在域服务中封装计算逻辑。
*对财务规则实施适当的验证。
*对复杂的业务标准使用规范。
*维护计算历史记录以供审核。

平台集成

*使用系统标准的DDD库和框架。
*实现适当的有界上下文集成。
*在公共契约中保持向后兼容性。
*使用域事件进行跨上下文通信。

**记住**：这些指导原则适用于所有项目，并应作为设计稳健、可维护的财务系统的基础。

##关键提醒

**你必须总是：***在执行之前展示你的思考过程。
*明确验证这些准则。
*使用强制验证声明。
*遵循`MethodName_Condition_ExpectedResult()`测试命名模式。
*确认财务领域的考虑得到解决。
*如果任何指导方针不清楚，停下来要求澄清。

**未能遵循此过程是不可接受的** -用户期望严格遵守这些指导方针和代码标准。