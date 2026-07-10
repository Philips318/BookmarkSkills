---
name: code-exemplars-blueprint-generator
description: 'Technology-agnostic prompt generator that creates customizable AI prompts for scanning codebases and identifying high-quality code exemplars. Supports multiple programming languages (.NET, Java, JavaScript, TypeScript, React, Angular, Python) with configurable analysis depth, categorization methods, and documentation formats to establish coding standards and maintain consistency across development teams.'
---
代码范例蓝图生成器

##配置变量
$ {PROJECT_TYPE = "自动检测|。净JavaScript Java | | | | |反应手稿角Python | |其他“}< !——初级技术——b>
$ {SCAN_DEPTH = "基本标准| |全面”}< !——分析代码库的深度——>
$ {INCLUDE_CODE_SNIPPETS = true |假}< !——包括实际的代码片段，除了文件引用——>
${CATEGORIZATION=“模式类型|架构层|文件类型”}<！——如何组织范例——b>
$ {MAX_EXAMPLES_PER_CATEGORY = 3} < !-每个类别的最大示例数->
$ {INCLUDE_COMMENTS = true |假}< !-包括每个范例的解释性评论->

##生成提示

扫描此代码库并生成一个exemplars.md文件，该文件识别高质量的代表性代码示例。范例应该演示我们的编码标准和模式，以帮助保持一致性。使用以下方法：# # # 1。代码库分析阶段
- ${PROJECT_TYPE == “自动检测” ？通过扫描文件扩展名和配置文件自动检测主要编程语言和框架
识别具有高质量实现，良好文档和清晰结构的文件
寻找常用的模式、架构组件和结构良好的实现
-优先考虑展示我们技术堆栈最佳实践的文件
-只引用代码库中存在的实际文件-没有假设的示例# # # 2。范例识别准则
-结构良好，具有清晰命名约定的可读代码
-全面的评论和文档
-正确的错误处理和验证
-遵循设计模式和架构原则
-关注点分离和单一责任原则
-高效的实现，没有代码异味
-代表我们的标准方法

# # # 3。核心模式类别${project_type == "。. NET“ || PROJECT_TYPE == ”自动检测" ？“# # # #。NET Exemplars（如果检测到）
- **领域模型**：找到正确实现封装和领域逻辑的实体
- **存储库实现**：我们数据访问方法的示例
-服务层组件**：结构良好的业务逻辑实现
- **控制器模式**：使用正确的验证和响应清理API控制器
- **依赖注入用法**:DI配置和使用的好例子
- **中间件组件**：自定义中间件实现
- **单元测试模式**：结构良好的测试，有适当的安排和断言':""}${(PROJECT_TYPE == "JavaScript" || PROJECT_TYPE == "TypeScript" || PROJECT_TYPE == "React" || PROJECT_TYPE == "Angular" || PROJECT_TYPE == "Auto-detect") ？' ####前端范例（如果检测到）
- **组件结构**：干净、结构良好的组件
- **状态管理**：状态处理的好例子
**API集成**：实现良好的服务调用和数据处理
- **表单处理**：验证和提交模式
—**路由实现**：导航和路由配置
**UI组件：可重用的、结构良好的UI元素
- **单元测试示例**：组件和服务测试':""}${PROJECT_TYPE == "Java" || PROJECT_TYPE == "Auto-detect" ？' #### Java Exemplars（如果检测到）
- **实体类**：设计良好的JPA实体或领域模型
- **服务实现**：清理服务层组件
- **Repository Patterns**：数据访问实现
**Controller/Resource类**:API端点实现
—**Configuration Classes**：应用配置
- **单元测试**：结构良好的JUnit测试':""}

${PROJECT_TYPE == “Python“ || PROJECT_TYPE == ”自动检测” ？' #### Python范例（如果检测到）
- **类定义**：结构良好的类和适当的文档
**APIRoutes/Views**：干净的API实现
—**Data Models**: ORM模型定义
—**Service Functions**：业务逻辑实现
- **实用程序模块**:Helper和实用程序函数
- **测试用例**：结构良好的单元测试':""}

# # # 4。架构层范例- **表示层**：
-用户界面组件
-Controllers/API端点
—查看“models/DTOs”

**业务逻辑层**：
-服务实现
-业务逻辑组件
-工作流编排

- **数据访问层**：
-存储库实现
-数据模型
-查询模式

- **交叉关注**：
-日志实现
-错误处理  - Authentication/authorization
——验证

# # # 5。范例文档格式

对于每个确定的范例，文件：
—文件路径（相对于存储库根目录）
-简要描述是什么让它成为典范
-它所代表的模式或组件类型
$ {INCLUDE_COMMENTS吗?-演示的关键实现细节和编码原则“:”"}
$ {INCLUDE_CODE_SNIPPETS吗?-小的，有代表性的代码片段（如果适用）“:”"}

${SCAN_DEPTH == "Comprehensive" ？“# # # 6。附加的文档

- **一致性模式**：注意在代码库中观察到的一致性模式
**架构观察**：记录代码中明显的架构模式
- **实现约定：确定命名和结构约定
**避免反模式**：注意代码库偏离最佳实践的任何区域':""}

### ${SCAN_DEPTH == "Comprehensive" ？7: 6}。输出格式创建exemplars.md1. 引言，说明本文档的目的
2. 目录与链接到类别
3. 基于${CATEGORIZATION}组织章节
4. 每个类别最多${MAX_EXAMPLES_PER_CATEGORY}示例
5. 结论和维护代码质量的建议

对于需要指导如何实现与现有模式一致的新特性的开发人员，该文档应该是可操作的。

重要：只包含代码库中的实际文件。验证所有文件路径是否存在。不要包括占位符或假设性的例子。
”

##预期输出
运行此提示符后，GitHub Copilot将扫描您的代码库，并生成一个exemplars.md文件，其中包含对存储库中高质量代码示例的实际引用，并根据您选择的参数进行组织。