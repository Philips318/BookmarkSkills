---
name: readme-blueprint-generator
description: 'Intelligent README.md generation prompt that analyzes project documentation structure and creates comprehensive repository documentation. Scans .github/copilot directory files and copilot-instructions.md to extract project information, technology stack, architecture, development workflow, coding standards, and testing approaches while generating well-structured markdown documentation with proper formatting, cross-references, and developer-focused content.'
---
#自述生成器提示符

通过分析.github/copilot目录和copilot-instructions.md文件中的文档文件，为这个存储库生成一个全面的README.md。遵循以下步骤：

1. 扫描.github/copilot文件夹下的所有文件，如：
——体系结构
——Code_Exemplars
——Coding_Standards
——Project_Folder_Structure
——Technology_Stack
——unit_test
——Workflow_Analysis

2. 中的copilot-instructions.md文件。github文件夹

3. 用以下部分创建一个README.md：

项目名称和描述
-从文档中提取项目名称和主要用途
-包括对项目内容的简明描述

##技术栈
-列出使用的主要技术、语言和框架
-包括可用的版本信息
-主要从Technology_Stack文件中获取此信息##项目架构
-提供架构的高级概述
-如果在文档中有描述，考虑包括一个简单的图表
-来自架构文件的源代码

##开始
—包括基于技术栈的安装说明
-添加安装和配置步骤
-包括任何先决条件

项目结构
-文件夹组织的简要概述
-来自Project_Folder_Structure文件

##主要特性
-列出项目的主要功能和特点
-从各种文档文件中提取

##开发流程
-总结开发过程
-包括分支策略的信息
-来自Workflow_Analysis文件

编码标准
-总结关键编码标准和约定
-源代码来自Coding_Standards文件# #测试
-解释测试方法和工具
-来自Unit_Tests文件

# #贡献
-为项目作出贡献的指引
-参考任何代码范例作为指导
-源代码来自Code_Exemplars和copilot-instructions

# #许可证
-包括可用的许可信息

用适当的Markdown格式化README，包括：
-明确标题和副标题
-适当的代码块
-列表，更好的可读性
—其他文档文件的链接
-如果信息可用，构建状态，版本等的徽章

保持README的简洁和信息量，重点关注新开发人员或用户需要了解的项目。