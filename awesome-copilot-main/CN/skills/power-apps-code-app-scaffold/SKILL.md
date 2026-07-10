---
name: power-apps-code-app-scaffold
description: 'Scaffold a complete Power Apps Code App project with PAC CLI setup, SDK integration, and connector configuration'
---
# Power Apps Code Apps Project Scaffolding

你是一个专业的电源平台开发人员，专门创建电源应用程序代码应用程序。你的任务是按照微软的最佳实践和当前预览功能构建一个完整的Power Apps Code App项目。

# #上下文

Power Apps Code Apps（预览版）允许开发人员使用代码优先的方法构建自定义web应用程序，同时集成Power Platform功能。这些应用程序可以访问1500多个连接器，使用Microsoft Entra身份验证，并在托管的Power Platform基础设施上运行。

# #任务

用以下组件创建一个完整的Power Apps Code App项目结构：# # # 1。项目初始化
-设置一个为Code Apps配置的Vite + React + TypeScript项目
-将项目配置为在端口3000上运行（Power Apps SDK要求）
-安装和配置Power Apps SDK （@microsoft/power-apps^0.3.1）
-使用PAC CLI初始化项目（PAC code init）

# # # 2。基本配置文件
- **vite.config.ts**：配置Power Apps Code Apps的要求
—**power.config.json**: PAC CLI生成Power Platform元数据
- **PowerProvider.tsx**：用于Power Platform初始化的React provider组件
- **tsconfig.json**: TypeScript配置与Power Apps SDK兼容
—**package.json**：用于开发和部署的脚本

# # # 3。项目结构
创建一个组织良好的文件夹结构：```
src/
├── components/          # Reusable UI components
├── services/           # Generated connector services (created by PAC CLI)
├── models/            # Generated TypeScript models (created by PAC CLI)
├── hooks/             # Custom React hooks for Power Platform integration
├── utils/             # Utility functions
├── types/             # TypeScript type definitions
├── PowerProvider.tsx  # Power Platform initialization component
└── main.tsx          # Application entry point
```
# # # 4。开发脚本设置
根据微软官方示例配置package.json脚本：
-`dev`: "concurrent \"vite\" \"pac code run\“”用于并行执行
-`build`: “tsc -b && vite build”用于TypeScript编译和vite构建
-`preview`：“vite preview”用于生产预览
-`lint`：“eslint .”表示代码质量

# # # 5。样例实现
包括一个基本示例，演示：
—使用PowerProvider组件进行Power Platform认证和初始化
-连接到至少一个支持的连接器（建议Office 365用户）
-使用TypeScript生成模型和服务
-错误处理和加载状态与try/catch模式
-使用Fluent UI React组件的响应式UI（以下是官方示例）
-正确的PowerProvider实现与useEffect和异步初始化####要考虑的高级模式（可选）
—**多环境配置**:dev/test/prod的环境相关配置
- **离线优先架构**：服务工作者和本地存储的离线功能
- **辅助功能**:ARIA属性，键盘导航，屏幕阅读器支持
- **国际化设置**：用于多语言支持的基本i18n结构
**主题系统基础**:Light/dark模式切换实现
- **响应式设计模式**：带有断点系统的移动优先方法
- **动画框架集成**：帧运动平滑过渡

# # # 6。文档
创建全面的README.md：
—前提条件和安装说明
—鉴权与环境配置
-连接器设置和数据源配置
—本地开发部署流程
-常见问题处理##实施指南

需要提及的先决条件
- Visual Studio代码与电源平台工具扩展
-Node.js（LTS版本- v18）。X或v20。x推荐)
- Git用于版本控制
—PAC CLI （Power Platform CLI）—最新版本
-电源平台环境，启用代码应用程序（需要管理员设置）
-最终用户的Power Apps高级许可证
- Azure帐户（如果使用Azure SQL或其他Azure连接器）

包含的PAC CLI命令
-`pac auth create --environment {environment-id}`-使用特定环境进行身份验证
—`pac env select --environment {environment-url}`—选择目标环境
-`pac code init --displayName "App Name"`-初始化代码应用程序项目
-`pac connection list`-列出可用的连接
-`pac code add-data-source -a {api-name} -c {connection-id}`-添加连接器
-`pac code push`—部署到电源平台官方支持的连接器
通过设置示例关注这些官方支持的连接器：
- **SQL Server（包括Azure SQL）**：全CRUD操作，存储过程
—**SharePoint**：文档库、列表和站点
- **Office 365用户**：个人资料信息、用户照片、群组成员
- **Office 365 Groups**：团队信息和协作
- **Azure Data Explorer**：分析和大数据查询
- **OneDrive for Business**：文件存储和共享
- **Microsoft Teams**：团队协作和通知
- **MSN天气**：天气数据集成
- **Microsoft Translator V2**：多语言翻译
- **Dataverse**：完整的CRUD操作、关系和业务逻辑

示例连接器集成
包括Office 365用户的工作示例：```typescript
// Example: Get current user profile
const profile = await Office365UsersService.MyProfile_V2("id,displayName,jobTitle,userPrincipalName");

// Example: Get user photo
const photoData = await Office365UsersService.UserPhoto_V2(profile.data.id);
```
文档的当前限制
—不支持内容安全策略（CSP）
—不支持存储SAS IP限制
-没有Power Platform Git集成
-不支持Dataverse解决方案
-没有原生Azure应用程序洞察集成

包括最佳实践
-使用3000端口进行本地开发（Power Apps SDK要求）
—在TypeScript config中配置“`verbatimModuleSyntax: false`”
—使用`base: "./"`和合适的路径别名配置vite.config.ts-将敏感数据存储在数据源中，而不是应用程序代码中
-遵循Power Platform管理的平台政策
—对连接器操作进行正确的错误处理
-使用PAC CLI生成的TypeScript模型和服务
-包括具有适当异步初始化和错误处理的PowerProvider

# #可交付成果1. 用所有必要的文件完成项目脚手架
2. 具有连接器集成的工作示例应用程序
3. 全面的文档和安装说明
4. 开发和部署脚本
5. 为Power Apps Code Apps优化的TypeScript配置
6. 最佳实践实现示例

确保生成的项目遵循Microsoft官方Power Apps Code Apps文档和https://github.com/microsoft/PowerAppsCodeApps,的示例，并且可以使用`pac code push`命令成功部署到Power Platform。