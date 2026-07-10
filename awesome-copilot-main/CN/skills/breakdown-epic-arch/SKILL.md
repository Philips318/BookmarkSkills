---
name: breakdown-epic-arch
description: 'Prompt for creating the high-level technical architecture for an Epic, based on a Product Requirements Document.'
---
# Epic架构规范提示符

# #目标

担任高级软件架构师。您的任务是使用Epic PRD并创建高级技术体系结构规范。本文档将指导史诗的开发，概述所需的主要组件、特性和技术支持。

背景考虑

-产品经理提供的Epic PRD。
-面向模块化、可扩展应用的领域驱动架构模式。
- **自托管和SaaS部署**要求。
-所有服务的Docker容器化。
**TypeScript/Next.js**栈与应用路由器。
- **Turborepo monorepo**模式。
**tRPC**用于类型安全api。
- **Stack Auth**用于认证。

**注意：**不要在输出中编写代码，除非它是用于技术情况的伪代码。

##输出格式输出应该是Markdown格式的完整Epic Architecture Specification，保存到`/docs/ways-of-work/plan/{epic-name}/arch.md`。

规格结构

# # # # 1。史诗建筑概述

-简要总结一下史诗的技术方法。

# # # # 2。系统架构图

创建一个全面的Mermaid图，以说明此史诗的完整系统架构。该图表应包括：- **用户层**：显示不同的用户类型（web浏览器，移动应用程序，管理界面）如何与系统交互
- **应用层**：描述负载均衡器、应用实例和认证服务（Stack Auth）
- **服务层**：包括tRPC api，后台服务，工作流引擎（n8n），以及任何史诗特定的服务
- **数据层**：显示数据库（PostgreSQL），矢量数据库（Qdrant），缓存层（Redis）和外部API集成
- **基础设施层**：代表Docker的容器化和部署架构

使用清晰的子图来组织这些层，为不同的组件类型应用一致的颜色编码，并显示组件之间的数据流。包括与史诗相关的同步请求路径和异步处理流。

# # # # 3。高级功能和技术支持-要构建的高级功能列表。
-支持这些特性所需的技术支持因素列表（例如，新服务、库、基础设施）。

# # # # 4。技术堆栈

-要使用的关键技术、框架和库的列表。

# # # # 5。技术价值

-评估技术价值（例如，高、中、低）并给出简短的理由。

# # # # 6。t恤尺寸估算

-为史诗提供一个高层次的t恤尺寸估算（例如，S， M， L， XL）。

##背景模板

- **Epic PRD:** [Epic PRD降价文件的内容]