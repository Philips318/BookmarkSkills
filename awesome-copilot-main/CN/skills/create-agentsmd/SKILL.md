---
name: create-agentsmd
description: 'Prompt for generating an AGENTS.md file for a repository'
---
创建高质量的AGENTS.md文件

你是密码特工。您的任务是按照https://agents.md/.的公共指导，在这个存储库的根目录下创建一个完整、准确的AGENTS.mdAGENTS.md是一种开放格式，旨在为编码代理提供在项目中有效工作所需的上下文和指令。AGENTS.md是什么？AGENTS.md是一个Markdown文件，作为“代理的自述文件”——一个专门的、可预测的地方，提供上下文和说明，帮助AI编码代理在您的项目中工作。它通过包含编码代理所需的详细技术上下文来补充README.md，但可能会使以人为中心的README变得混乱。

##关键原则- **以代理为中心的**：包含自动化工具的详细技术说明
- **补充README.md**：不取代人类文档，但增加了特定于代理的上下文
- **标准化位置**：放置在存储库根（或单节点子项目根）
- **开放格式**：采用标准Markdown，结构灵活
- **生态系统兼容性**：适用于20多种不同的AI编码工具和代理

文件结构和内容指南

# # # 1。需要设置

—在存储库根目录下创建名为“`AGENTS.md`”的文件
-使用标准Markdown格式
-无必填字段-根据项目需要灵活配置

# # # 2。要包括的重要部分

####项目概述

-项目的简要描述
-架构概述，如果复杂
-使用的关键技术和框架

####设置命令-安装说明
-环境设置步骤
-依赖管理命令
-数据库设置（如适用）

####开发流程

—如何启动开发服务器
-构建命令
-Watch/hot-reloadsetup
-包管理器细节（npm, pnpm， yarn等）

####测试说明

-如何运行测试（单元、集成、端到端）
-测试文件位置和命名约定
-覆盖范围要求
-使用的特定测试模式或框架
-如何运行测试子集或专注于特定区域

####代码风格指南

-特定于语言的约定
—检查和格式化规则
-文件组织模式
-命名约定
-Import/export模式

####构建和部署

-构建命令和输出
—环境配置
—部署步骤和要求
-CI/CD管道信息

# # # 3。可选但推荐的章节

####安全注意事项-安全测试要求
-保密管理
-身份验证模式
—权限模型

#### Monorepo指令（如适用）

-如何使用多个包
-跨包依赖
—选择building/testing-特定于包的命令

####拉取请求指南

-标题格式要求
-提交前需要检查
-评审过程
-提交消息约定

####调试与故障处理

-常见问题和解决方案
-日志模式
-调试配置
-性能考虑

##模板示例

使用此模板作为起始模板，并根据具体项目进行定制：```markdown
# AGENTS.md

## Project Overview

[Brief description of the project, its purpose, and key technologies]

## Setup Commands

- Install dependencies: `[package manager] install`
- Start development server: `[command]`
- Build for production: `[command]`

## Development Workflow

- [Development server startup instructions]
- [Hot reload/watch mode information]
- [Environment variable setup]

## Testing Instructions

- Run all tests: `[command]`
- Run unit tests: `[command]`
- Run integration tests: `[command]`
- Test coverage: `[command]`
- [Specific testing patterns or requirements]

## Code Style

- [Language and framework conventions]
- [Linting rules and commands]
- [Formatting requirements]
- [File organization patterns]

## Build and Deployment

- [Build process details]
- [Output directories]
- [Environment-specific builds]
- [Deployment commands]

## Pull Request Guidelines

- Title format: [component] Brief description
- Required checks: `[lint command]`, `[test command]`
- [Review requirements]

## Additional Notes

- [Any project-specific context]
- [Common gotchas or troubleshooting tips]
- [Performance considerations]
```
agents.md的工作示例

下面是agents.md网站上的一个真实例子：```markdown
# Sample AGENTS.md file

## Dev environment tips

- Use `pnpm dlx turbo run where <project_name>` to jump to a package instead of scanning with `ls`.
- Run `pnpm install --filter <project_name>` to add the package to your workspace so Vite, ESLint, and TypeScript can see it.
- Use `pnpm create vite@latest <project_name> -- --template react-ts` to spin up a new React + Vite package with TypeScript checks ready.
- Check the name field inside each package's package.json to confirm the right name—skip the top-level one.

## Testing instructions

- Find the CI plan in the .github/workflows folder.
- Run `pnpm turbo run test --filter <project_name>` to run every check defined for that package.
- From the package root you can just call `pnpm test`. The commit should pass all tests before you merge.
- To focus on one step, add the Vitest pattern: `pnpm vitest run -t "<test name>"`.
- Fix any test or type errors until the whole suite is green.
- After moving files or changing imports, run `pnpm lint --filter <project_name>` to be sure ESLint and TypeScript rules still pass.
- Add or update tests for the code you change, even if nobody asked.

## PR instructions

- Title format: [<project_name>] <Title>
- Always run `pnpm lint` and `pnpm test` before committing.
```
##实现步骤

1. **分析项目结构**了解：

-使用的编程语言和框架
-包管理器和构建工具
-测试框架
-项目架构（单包、单包等）

2. **通过以下检查确定关键工作流程：

—package.json脚本
- Makefile或其他构建文件
—CI/CD配置文件
-文档文件

3. **创建全面的章节**，涵盖：

-所有必要的设置和开发命令
—测试策略和命令
-代码风格和约定
—构建和部署流程

4. **包括具体的，可操作的命令**，代理可以直接执行

5. **测试说明**，确保所有命令都按照文档工作

6. **专注于代理商需要了解的内容，而不是一般的项目信息

最佳实践**具体：包括确切的命令，而不是模糊的描述
- **使用代码块**：为了清晰起见，将命令用反引号括起来
- **包括上下文**：解释为什么需要某些步骤
- **保持最新**：随着项目的发展而更新
—**测试命令**：确保列出的所有命令都能正常工作
- **考虑嵌套文件**：对于单节点，根据需要在子项目中创建AGENTS.md文件

## Monorepo的考虑

对于大型单节点：

—在存储库根目录下放置一个主AGENTS.md-在子项目目录中创建额外的AGENTS.md文件
-最近的AGENTS.md文件优先于任何给定的位置
-包括导航提示之间的packages/projects##最后的笔记-AGENTS.md工作与20+ AI编码工具，包括光标，Aider, Gemini CLI，和许多其他
-格式是故意灵活的-适应您的项目需要
-专注于可操作的指令，帮助代理理解和使用你的代码库
-这是动态文档-随着项目的发展更新它

在创建AGENTS.md文件时，优先考虑清晰度、完整性和可操作性。目标是为任何编码代理提供足够的上下文，以便在不需要额外的人工指导的情况下有效地为项目做出贡献。