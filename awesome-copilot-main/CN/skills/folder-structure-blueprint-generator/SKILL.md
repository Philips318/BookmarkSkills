---
name: folder-structure-blueprint-generator
description: 'Comprehensive technology-agnostic prompt for analyzing and documenting project folder structures. Auto-detects project types (.NET, Java, React, Angular, Python, Node.js, Flutter), generates detailed blueprints with visualization options, naming conventions, file placement patterns, and extension templates for maintaining consistent code organization across diverse technology stacks.'
---
#项目文件夹结构蓝图生成器

##配置变量

$ {PROJECT_TYPE = "自动检测|。净Java | | |角反应Python | |Node.js| |其他“摆动}<!-- Select primary technology -->
$ {INCLUDES_MICROSERVICES =“自动检测|真|假”}<!-- Is this a microservices architecture? -->
$ {INCLUDES_FRONTEND =“自动检测|真|假”}<!-- Does project include frontend components? -->
$ {IS_MONOREPO =“自动检测|真|假”}<!-- Is this a monorepo with multiple projects? -->
$ {VISUALIZATION_STYLE = " ASCII |减价列表|表”}<!-- How to visualize the structure -->
$ {DEPTH_LEVEL = 1 - 5}<!-- How many levels of folders to document in detail -->
$ {INCLUDE_FILE_COUNTS = true |假}<!-- Include file count statistics -->
$ {INCLUDE_GENERATED_FOLDERS = true |假}<!-- Include auto-generated folders -->
$ {INCLUDE_FILE_PATTERNS = true |假}<!-- Document file naming/location patterns -->
$ {INCLUDE_TEMPLATES = true |假}<!-- Include file/folder templates for new features -->
##生成提示

“分析项目的文件夹结构，并创建一个全面的‘Project_Folders_Structure_Blueprint.md’文档，作为维护一致的代码组织的明确指南。使用以下方法：

初始自动检测阶段${PROJECT_TYPE == “自动检测” ？
首先扫描文件夹结构，寻找识别项目类型的关键文件：
-查找solution/project文件。sln,。csproj。fsproj。Vbproj)来识别。网项目
-检查构建文件(pom.xml, build。gradle,设置。gradle)用于Java项目
-识别package.json与JavaScript/TypeScript项目的依赖关系
-查找特定的框架文件（angular.json， react-scripts条目，next.config.js）
-检查Python项目标识符（requirements.txt,setup.py,pyproject.toml）
-检查移动应用程序标识符（pubspec.yaml，android/ios文件夹）
-注明所有找到的技术签名及其版本”；
“重点分析${PROJECT_TYPE}项目结构”}${IS_MONOREPO == “自动检测” ？
“通过查找以下内容来确定这是否是一个单orepo：
-多个不同的项目有自己的配置文件
—工作空间配置文件（lerna.json、nx.json、turborepo.json等）
-跨项目引用和共享依赖模式
-根级业务流程脚本和配置“:”"}

${incles_microservices == “自动检测” ？
检查微服务架构指标：
—多个服务目录，结构为similar/repeated—与服务相关的Dockerfiles或部署配置
-服务间通信模式（api、消息代理）
—服务注册中心或发现配置
—API网关配置文件
-跨服务共享库或实用程序“:”"}${incles_frontend == “自动检测” ？
通过查找以下内容来识别前端组件：
- Web资产目录（wwwroot, public, dist, static）
UI框架文件（组件、模块、页面）
-前端构建配置（webpack, vite， rollup等）
-样式表组织（CSS， SCSS，样式组件）
-静态资产组织（图像，字体，图标）“:”"}

# # # 1。结构概述

提供${PROJECT_TYPE == “自动检测”的高级概述？“检测到的项目类型”：PROJECT_TYPE}项目的组织原则和文件夹结构：

-记录文件夹结构中反映的整体架构方法
-确定主要的组织原则（按特性、按层、按领域等）
-注意所有在代码库中重复的结构模式
-在可以推断的地方记录结构背后的基本原理${IS_MONOREPO == “自动检测” ？
“如果被发现是单一组织，解释单一组织是如何组织的以及项目之间的关系。”：
IS_MONOREPO吗?“解释公司是如何组织的，以及项目之间的关系。”: "}

${incles_microservices == “自动检测” ？
“如果检测到微服务，描述它们是如何构建和组织的。”：
INCLUDES_MICROSERVICES吗?“描述微服务的结构和组织方式。”: "}

# # # 2。目录可视化

${visualization_style == " ascii " ？
“创建文件夹层次结构的ASCII树表示到深度级别${DEPTH_LEVEL}。”: "}

${VISUALIZATION_STYLE == “降价列表” ？
“使用嵌套标记列表来表示文件夹层次结构到深度级别${DEPTH_LEVEL}。”: "}${VISUALIZATION_STYLE == “表” ？
“创建一个包含路径、目的、内容类型和约定等列的表。”: "}

$ {INCLUDE_GENERATED_FOLDERS吗?
“包括所有文件夹，包括生成的文件夹。”：
“排除自动生成的文件夹，如bin/、obj/、node_modules/等”}

# # # 3。关键目录分析

记录每个重要目录的目的、内容和模式：

${PROJECT_TYPE == “自动检测” ？
对于每种检测到的技术，根据观察到的使用模式分析目录结构：“:” "}

${(project_type == "。. NET" || PROJECT_TYPE == "Auto-detect") ？
“# # # #。. NET项目结构（如果检测到）

- **解决方案组织**：
-项目如何分组和关联
-解决方案文件夹组织模式
-多目标项目模式- **项目组织**：
-内部文件夹结构模式
-源代码组织方法
-资源组织
-项目依赖项和引用

- **Domain/Feature机构**：
-如何分离业务领域或功能
-领域边界执行模式

- **层组织**：
-关注点分离（控制器、服务、存储库等）
-层交互和依赖模式

- **配置管理**：
-配置文件的位置和用途
—特定于环境的配置
-保密管理方法

- **测试项目组织**：
-测试项目结构和命名
-测试类别和组织
-测试数据和模拟位置“:”"}

${(PROJECT_TYPE == "React" || PROJECT_TYPE == "Angular" || PROJECT_TYPE == "Auto-detect") ？
#### UI项目结构（如果检测到）- **组成机构**：
-组件文件夹结构模式
-分组策略（按特性、类型等）
-共享组件vs.特定功能组件

- **状态管理**：
—与国家相关的文件组织
-全局状态的存储结构
—本地状态管理模式

- **路由组织**：
-路由定义位置
-Page/view组件组织
-路由参数处理

- **API集成**：
- API客户端组织
—业务层结构
-数据获取模式

- **资产管理**：
—静态资源组织
—Image/media文件结构
-字体和图标组织

- **组织风格**：
—CSS/SCSS文件结构
-主题组织
-样式模块模式“:”"}

# # # 4。文件放置模式$ {INCLUDE_FILE_PATTERNS吗?
记录确定不同类型的文件应该放在哪里的模式：- **配置文件**
—不同类型配置对应的位置
—特定于环境的配置模式

**Model/Entity定义**：
-定义领域模型的位置
—DTO （Data transfer object）位置
-模式定义位置

**业务逻辑**：
-服务实现位置
—业务规则组织
-实用程序和辅助函数的放置

**接口定义**：
-定义接口和抽象的地方
-如何对接口进行分组和组织

- **测试文件**：
-单元测试位置模式
-集成测试放置
-测试实用程序和模拟位置

- **文档文件**：
- API文档放置
-内部文件组织
-“自述文件分发”：
“项目中密钥文件类型所在的文档。”}# # # 5。命名和组织惯例
记录整个项目中观察到的命名和组织惯例：

- **文件命名模式**：
-大小写约定（PascalCase, camelCase, kebab-case）
-前缀和后缀模式
—在文件名中键入指标

- **文件夹命名模式**：
-不同文件夹类型的命名约定
-分层命名模式
-分组和分类约定

- **Namespace/Module图案**：
-namespaces/modules如何映射到文件夹结构
-Import/using语句组织
内部API与公共API的分离

- **组织模式**：
-代码共置策略
-特性封装方法
-横切关注点组织

# # # 6。导航和开发工作流
为导航和使用代码库结构提供指导：- **入口**：
-主要应用程序入口点
—关键配置起始点
-了解项目的初始文件

- **常见开发任务**：
-在哪里添加新功能
-如何扩展现有的功能
-在哪里放置新的测试
-配置修改位置

**依赖模式**：
-依赖关系如何在文件夹之间流动
-Import/reference模式
-依赖注入注册位置

$ {INCLUDE_FILE_COUNTS吗?
- **内容统计**：
-每个目录的文件分析
-代码分发指标
-复杂性集中区域“:”"}

# # # 7。构建和输出组织
记录构建过程和输出组织：- **构建配置**：
-构建脚本位置和目的
-建立管道组织
-构建任务定义

- **输出结构：
-Compiled/built输出位置
-产出组织模式
-分发包结构

- **特定于环境的构建**：
-开发与生产差异
—环境配置策略
-构建变型组织

# # # 8。特定于技术的组织

${(project_type == "。. NET" || PROJECT_TYPE == "Auto-detect") ？
“# # # #。net特定结构模式（如果检测到）- **项目档案整理**：
-项目文件结构和模式
-目标框架配置
-物业集团组织
-项目组模式

- **装配机构**：
程序集命名模式
-多装配体系结构
-程序集引用模式

- **资源组织**：
-嵌入式资源模式
-本地化文件结构
—静态web资产组织

- **包管理**：
—NuGet配置位置
-包参考组织
-包版本管理“:”"}

${（PROJECT_TYPE == “Java“ || PROJECT_TYPE == ”自动检测”）？
####特定于java的结构模式（如果检测到）- **包结构**：
-包命名和嵌套约定
-领域vs.技术包
-可见性和访问模式

- **构建工具组织**：
-Maven/Gradle结构模式
-模块组织
-插件配置模式

- **资源组织**：
-资源文件夹结构
-环境资源
-属性文件组织“:”"}

${(PROJECT_TYPE == "Node.js" || PROJECT_TYPE == "Auto-detect") ？
"####Node.js-特定结构模式（如果检测到）- **模块组织**：
- CommonJS vs. ESM组织
-内部模块模式
-第三方依赖管理

- **脚本组织**：
-npm/yarn脚本定义模式
-实用程序脚本位置
-开发工具脚本

- **配置管理**：
-配置文件位置
-环境变量管理
-保密管理办法“:”"}

# # # 9。扩展和进化
记录如何设计项目结构以进行扩展：- **扩展点**：
-如何添加新的modules/features，同时保持惯例
-Plugin/extension文件夹模式
自定义目录结构

- **可扩展性模式**：
-结构如何适应更大的特征
-分解大模块的方法
-代码分割策略

**重构模式**：
-常见的重构方法
-如何管理结构性变化
-增量重组模式

$ {INCLUDE_TEMPLATES吗?
“# # # 10。结构模板

为创建遵循项目约定的新组件提供模板：- **新功能模板**：
—用于添加完整特性的文件夹结构
—所需的文件类型及其位置
-要遵循的命名模式

- **新组件模板**：
—典型组件的目录结构
-必须包含的文件
-与现有结构的集成点

- **新的服务模板**：
—添加新业务的结构
-接口和实现位置
-配置和注册模式

- **新的测试结构**：
-测试projects/files的文件夹结构
—测试文件组织模板
-测试资源组织“:”"}

### ${include_templates ？“11”： “10”}。结构执行

记录项目结构是如何维护和执行的：- **结构验证**：
-Tools/scripts强制结构
-检查结构合规性
—与结构相关的检查规则

- **文件编制惯例**：
-如何记录结构变化
-记录架构决策的地方
-结构演化史

在最后包含关于维护此蓝图以及上次更新时间的部分。
”