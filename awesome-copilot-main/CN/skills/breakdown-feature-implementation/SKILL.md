---
name: breakdown-feature-implementation
description: 'Prompt for creating detailed feature implementation plans, following Epoch monorepo structure.'
---
#功能实施计划提示

# #目标

作为行业资深的软件工程师，负责为大型SaaS公司制作高触感功能。擅长基于特性PRD为特性创建详细的技术实现计划。
审查所提供的上下文，并输出一个彻底、全面的实施计划。
**注意：**不要在输出中编写代码，除非它是用于技术情况的伪代码。

##输出格式

输出应该是Markdown格式的完整实施计划，保存为`/docs/ways-of-work/plan/{epic-name}/{feature-name}/implementation-plan.md`。

###文件系统

前端和后端存储库的文件夹和文件结构遵循Epoch的单线程结构：```
apps/
  [app-name]/
services/
  [service-name]/
packages/
  [package-name]/
```
###实施计划

对于每个特性：

# # # #的目标

描述特征目标（3-5句话）

# # # #要求

-详细的功能需求（项目列表）
-实施计划详情

####技术考虑

#####系统架构简介

使用Mermaid创建一个全面的系统架构图，显示该特性如何集成到整个系统中。该图表应包括：

—**前端层**：用户界面组件、状态管理、客户端逻辑
- **API层**:tRPC端点、认证中间件、输入验证和请求路由
业务逻辑层：服务类、业务规则、工作流编排和事件处理
- **数据层**：数据库交互、缓存机制和外部API集成
—**基础设施层**：包括Docker容器、后台服务、部署组件使用子图清晰地组织这些层。用标记箭头显示层之间的数据流，箭头指示request/response模式、数据转换和事件流。包含此实现特有的任何特定于功能的组件、服务或数据结构。

- **技术堆栈选择**：每一层的文档选择原理```

- **Technology Stack Selection**: Document choice rationale for each layer
- **Integration Points**: Define clear boundaries and communication protocols
- **Deployment Architecture**: Docker containerization strategy
- **Scalability Considerations**: Horizontal and vertical scaling approaches

##### Database Schema Design

Create an entity-relationship diagram using Mermaid showing the feature's data model:

- **Table Specifications**: Detailed field definitions with types and constraints
- **Indexing Strategy**: Performance-critical indexes and their rationale
- **Foreign Key Relationships**: Data integrity and referential constraints
- **Database Migration Strategy**: Version control and deployment approach

##### API Design

- Endpoints with full specifications
- Request/response formats with TypeScript types
- Authentication and authorization with Stack Auth
- Error handling strategies and status codes
- Rate limiting and caching strategies

##### Frontend Architecture

###### Component Hierarchy Documentation

The component structure will leverage the `shadcn/ui` library for a consistent and accessible foundation.

**Layout Structure:**

```
食谱库页面
├──Header Section （shadcn: Card）
│├──├─Title （shadcn：版式`h1`）
│├──Add Recipe Button（带下拉菜单的按钮）
││├──rammstein
││├──rammstein （rammstein）
││├──从PDF文件中导入
│├──├──││├──││
├──主内容区（伸缩容器）
│├──滤镜侧边栏（旁边）
││├──滤镜标题（shadcn：版式`h4`）
││├──分类过滤器（shadcn：复选框组）
│├──├─rammstein （shadcn：复选框组）
│││难度滤镜（shadcn: RadioGroup）
│├──├─├─├
│├──├─├——cat （www.cats.org）
│├──rammstein （sound效果器）
│├──├─rammstein （www.catsound.org）
│├──rammstein （www.catmstein）
││快速动作（shadcn: Button - View, Edit）```

- **State Flow Diagram**: Component state management using Mermaid
- Reusable component library specifications
- State management patterns with Zustand/React Query
- TypeScript interfaces and types

##### Security Performance

- Authentication/authorization requirements
- Data validation and sanitization
- Performance optimization strategies
- Caching mechanisms

## Context Template

- **Feature PRD:** [The content of the Feature PRD markdown file]
