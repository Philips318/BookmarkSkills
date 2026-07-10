---
name: technology-stack-blueprint-generator
description: 'Comprehensive technology stack blueprint generator that analyzes codebases to create detailed architectural documentation. Automatically detects technology stacks, programming languages, and implementation patterns across multiple platforms (.NET, Java, JavaScript, React, Python). Generates configurable blueprints with version information, licensing details, usage patterns, coding conventions, and visual diagrams. Provides implementation-ready templates and maintains architectural consistency for guided development.'
---
综合技术堆栈蓝图生成器

##配置变量
$ {PROJECT_TYPE = "自动检测|。. NET|Java|JavaScript|React.js|React Native|Angular|Python|其他"}<！——初级技术——b>
$ {DEPTH_LEVEL = "基本全面标准| | | Implementation-Ready”}< !——分析深度——>
$ {INCLUDE_VERSIONS = true |假}< !——包含版本信息——>
$ {INCLUDE_LICENSES = true |假}< !——包括许可信息——>
$ {INCLUDE_DIAGRAMS = true |假}< !——生成架构图——>
$ {INCLUDE_USAGE_PATTERNS = true |假}< !——包括代码使用模式——>
$ {INCLUDE_CONVENTIONS = true |假}< !——文档编码规范——>
$ {OUTPUT_FORMAT = "减价JSON | | YAML | HTML”}< !——选择输出格式——>
${CATEGORIZATION=“技术类型|层|用途”}<！——组织方法——>

##生成提示“分析代码库并生成${DEPTH_LEVEL}技术堆栈蓝图，该蓝图将彻底记录技术和实现模式，以促进一致的代码生成。使用以下方法：

# # # 1。技术鉴定阶段
- ${PROJECT_TYPE == “自动检测” ？“扫描项目文件、配置文件和依赖关系的代码库，以确定正在使用的所有技术堆栈”：“关注${PROJECT_TYPE}技术”}
-通过检查文件扩展名和内容来识别所有编程语言
—分析配置文件(package.json，。Csproj，pom.xml等)来提取依赖项
检查构建脚本和管道定义以获取工具信息
- ${include_versions ？“从包文件和配置中提取精确的版本信息”：“跳过版本细节”}
- ${include_licenses ？“记录所有依赖项的许可信息”：“”}# # # 2。核心技术分析

${project_type == "。. NET“ || PROJECT_TYPE == ”自动检测" ？“# # # #。. NET堆栈分析（如果检测到）
-目标框架和语言版本（从项目文件中检测）
-所有NuGet包引用，包括版本和目的注释
-项目结构和组织模式
-配置方法（appsettings.json， IOptions等）
-认证机制（Identity， JWT等）
- API设计模式（REST, GraphQL， minimal API等）
-数据访问方法（EF Core， Dapper等）
-依赖注入模式
-中间件管道组件“:”"}${PROJECT_TYPE == "Java" || PROJECT_TYPE == "Auto-detect" ？"#### Java堆栈分析（如果检测到）
—JDK版本和核心框架
-所有的Maven/Gradle依赖与版本和目的
-包结构组织
—Spring Boot的使用和配置
-注释模式
-依赖注入方法
-数据访问技术（JPA， JDBC等）
- API设计（Spring MVC， JAX-RS等）“:”"}

${PROJECT_TYPE == “JavaScript“ || PROJECT_TYPE == ”自动检测” ？"#### JavaScript堆栈分析（如果检测到）
- ECMAScript版本和转译器设置
-按用途分类的所有npm依赖项
模块系统（ESM, CommonJS）
-构建工具（webpack， Vite等）与配置
- TypeScript的使用和配置
-测试框架和模式“:”"}${PROJECT_TYPE == “React.js“ || PROJECT_TYPE == ”自动检测” ？"####反应分析（如果检测到）
- React版本和关键模式（钩子vs类组件）
-状态管理方法（Context, Redux， Zustand等）
-组件库使用（Material-UI， Chakra等）
-路由实现
-表格处理策略
- API集成模式
-组件“:”"}的测试方法

${PROJECT_TYPE == “Python“ || PROJECT_TYPE == ”自动检测” ？"#### Python分析（如果检测到）
- Python版本和使用的关键语言特性
-软件包依赖项和虚拟环境设置
- Web框架细节（Django, Flask, FastAPI）
- ORM使用模式
-项目结构组织
- API设计模式“:”"}

# # # 3。实现模式和约定
$ {INCLUDE_CONVENTIONS吗?
每个技术领域的文档编码约定和模式：####命名约定
-Class/type命名模式
-Method/function命名模式
-变量命名约定
-文件命名和组织规范
-Interface/abstract类模式

####代码组织
-文件结构和组织
-文件夹层次结构模式
-Component/module边界
-代码分离和职责模式

####常用模式
-错误处理方法
-日志模式
—配置访问
-Authentication/authorization实现
-验证策略
-测试模式“:”"}

# # # 4。用法示例
$ {INCLUDE_USAGE_PATTERNS吗?
“提取显示标准实现模式的代表性代码示例：

#### API实现示例
-标准controller/endpoint实现
-请求DTO模式
-回复格式
-验证方法
-错误处理####数据访问示例
-存储库模式实现
-Entity/model定义
-查询模式
-事务处理

####服务层示例
-服务类实现
-业务逻辑组织
横切关注集成
-依赖注入的使用

#### UI组件示例（如果适用）
-组件结构
-状态管理模式
-事件处理
- API集成模式“:”"}

# # # 5。技术堆栈图
${DEPTH_LEVEL == "Comprehensive" || DEPTH_LEVEL == "Implementation-Ready" ？
“创建一个全面的技术地图，包括：

####核心框架使用
-主要框架及其在项目中的具体使用
-特定于框架的配置和自定义
—扩展点和自定义####集成点
-不同的技术组件如何整合
—组件间的认证流程
—前端与后端之间的数据流
—第三方服务集成模式

####开发工具
- IDE设置和约定
-代码分析工具
-配置文件和格式化程序
-构建和部署管道
-测试框架和方法

# # # #的基础设施
-部署环境详细信息
-容器技术
-使用的云服务
-监控和日志基础设施“:”"}

# # # 6。特定于技术的实现细节${project_type == "。. NET“ || PROJECT_TYPE == ”自动检测" ？
“# # # #。NET实现细节（如果检测到）
- **依赖注入模式**：
-服务注册方法（Scoped/Singleton/Transient模式）
-配置绑定模式

- **控制器模式**：
-基本控制器使用率
-操作结果类型和模式
-路由属性约定
-过滤器使用（授权、验证等）

- **数据访问模式**：
—ORM的配置和使用
-实体配置方法
-关系定义
-查询模式和优化方法

- **API设计模式**（如果使用）：
-端点组织
-参数绑定方法
-响应类型处理

- **使用的语言特性**：
-从代码中检测特定的语言特性
-识别常见的模式和习语
-注意任何特定的版本依赖特性“:”"}${PROJECT_TYPE == “React.js“ || PROJECT_TYPE == ”自动检测” ？
"#### React实现细节（如果检测到）
- **组件结构**：
-函数与类组件
- Props接口定义
-组件组成模式

- **钩子使用模式**：
-自定义钩子实现风格
- useState模式
-使用效果清理方法
-上下文使用模式

- **状态管理**：
-局部与全局状态决策
—状态管理库模式
—存储配置
-选择模式

- **造型方法**：
- CSS方法（CSS模块，样式组件等）
-主题实现
-响应式设计模式“:”"}

# # # 7。新代码实现蓝图
${DEPTH_LEVEL == "Implementation-Ready" ？
在分析的基础上，提供一个实现新功能的详细蓝图：- **File/Class模板**：通用组件类型的标准结构
- **代码片段**：常见操作的现成代码模式
- **实现清单**：实现端到端功能的标准步骤
- **集成点**：如何将新代码与现有系统连接
- **测试要求**：不同组件类型的标准测试模式
- **文档要求**：新特性的标准文档模式“:”"}

$ {INCLUDE_DIAGRAMS吗?
“# # # 8。技术关系图
- **堆栈图**：完整技术堆栈的可视化表示
- **依赖流**：不同的技术如何相互作用
- **组件关系**：主要组件之间的依赖关系
- **数据流**：数据如何流经技术栈“:”"}### ${include_diagrams ？9: 8}。技术决策背景
-记录技术选择的明显原因
-注意任何遗留或弃用的技术标记为替换
-识别技术限制和边界
—记录技术升级路径和兼容性注意事项

将输出格式设置为${OUTPUT_FORMAT}，并按${CATEGORIZATION}对技术进行分类。

将输出保存为“Technology_Stack_Blueprint”。${OUTPUT_FORMAT == "Markdown" ？“md”： OUTPUT_FORMAT.toLowerCase()}'
”