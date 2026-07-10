---
name: create-llms
description: 'Create an llms.txt file from scratch based on repository structure following the llms.txt specification at https://llmstxt.org/'
---
#从存储库结构创建LLMs.txt文件

按照官方的llms.txt规范（https://llmstxt.org/.）在存储库的根目录中从零开始创建一个新的`llms.txt`文件。该文件为大型语言模型（llm）提供了高级指导，告诉它们在哪里可以找到相关的内容，从而理解存储库的目的和规范。

##主要指令

创建一个全面的`llms.txt`文件，作为llm有效地理解和导航存储库的入口点。该文件必须符合llms.txt规范，并针对LLM使用进行了优化，同时保持人类可读。

分析和计划阶段

在创建`llms.txt`文件之前，必须完成一个彻底的分析：

步骤1：审查llms.txt规范-审查官方规范在https://llmstxt.org/，以确保完全符合
-理解所需的格式结构和指南
-注意具体的降价结构要求

步骤2：存储库结构分析

-使用适当的工具检查完整的存储库结构
-确定存储库的主要目的和范围
-对所有重要目录及其用途进行编目
-列出对LLM理解有价值的关键文件

步骤3：内容发现

-识别自述文件及其位置
-查找文档文件（`.md`文件在`/docs/`，`/spec/`等）
-定位规范文件及其用途
—发现配置文件及其相关性
-查找示例文件和代码示例
-识别任何现有的文档结构

步骤4：创建实施计划根据你的分析，创建一个结构化的计划，包括：

-储存库用途和范围摘要
-按优先级排序的LLM基本文件列表
-提供附加上下文的辅助文件
—llms.txt文件的组织结构

##实现要求

格式遵从性

根据规范，`llms.txt`文件必须遵循这个精确的结构：

1. **H1标题**：单行repository/project名称（必选）
2. **Blockquote Summary**: Blockquote格式的简短描述（可选但推荐）
3. **附加细节**：零个或多个没有上下文标题的降价节
4. **文件列表节**：零或多个H2节包含链接的标记列表

内容要求

####- **项目名称**：清晰，描述性标题为H1
- **Summary**：简洁的blockquote，解释了存储库的目的
- **关键文件**：按类别组织的基本文件（H2节）

####文件链接格式

每个文件链接必须遵循：`[descriptive-name](relative-url): optional description`####部门组织

将文件组织成逻辑的H2部分，例如：

—**Documentation**：核心文档文件
- **规格**：技术规格和要求
—**Examples**：代码样例和使用示例
—**Configuration**：安装和配置文件
- **可选**：次要文件（特殊含义-可以跳过较短的上下文）

###内容指南

####语言与风格

使用简洁、清晰、没有歧义的语言
避免没有解释的术语
-为人类和法学硕士读者写作
-在描述中要具体和翔实

####文件选择标准包括以下文件：
-解释存储库的目的和范围
-提供必要的技术文件
-展示用法示例和模式
—定义接口和规范
—包含配置和安装说明

排除下列文件：
-纯粹是实现细节
—包含冗余信息
是构建工件或生成的内容
-与理解项目无关

##执行步骤

步骤1：存储库分析

1. 彻底检查存储库结构
2. 主要阅读README.md了解项目
3. 识别所有文档目录和文件
4. 编目规范文件及其用途
5. 查找示例文件和配置文件

步骤2：内容规划1. 确定主要目的陈述
2. 为blockquote写一个简明的摘要
3. 将识别的文件分组到逻辑类别中
4. 根据文件的重要性对LLM的理解进行优先排序
5. 为每个文件链接创建描述

###步骤3：文件创建

1. 在存储库根目录下创建`llms.txt`文件
2. 遵循准确的格式规范
3. 包括所有必需的部分
4. 使用适当的降价格式
5. 确保所有链接都是有效的相对路径

###步骤4：验证
1. 验证符合https://llmstxt.org/规范
2. 检查所有链接是否有效和可访问
3. 确保文件作为一个有效的LLM导航工具
4. 确认该文件是人类和机器可读的

##质量保证

格式验证-✅H1标头与项目名称
-✅Blockquote摘要（如果包含）
-✅H2部分的文件列表
-✅正确的降价链接格式
-✅没有损坏或无效的链接
-✅始终保持格式一致

###内容验证

-✅语言清晰，毫不含糊
-✅全面覆盖重要文件
-✅内容的逻辑组织
-✅适当的文件描述
-✅作为有效的LLM导航工具

规范遵从性

-✅完全遵循https://llmstxt.org/格式
-✅使用所需的降价结构
-✅适当地实现可选部分
-✅文件位于存储库根（`/llms.txt`）

##结构模板示例```txt
# [Repository Name]

> [Concise description of the repository's purpose and scope]

[Optional additional context paragraphs without headings]

## Documentation

- [Main README](README.md): Primary project documentation and getting started guide
- [Contributing Guide](CONTRIBUTING.md): Guidelines for contributing to the project
- [Code of Conduct](CODE_OF_CONDUCT.md): Community guidelines and expectations

## Specifications

- [Technical Specification](spec/technical-spec.md): Detailed technical requirements and constraints
- [API Specification](spec/api-spec.md): Interface definitions and data contracts

## Examples

- [Basic Example](examples/basic-usage.md): Simple usage demonstration
- [Advanced Example](examples/advanced-usage.md): Complex implementation patterns

## Configuration

- [Setup Guide](docs/setup.md): Installation and configuration instructions
- [Deployment Guide](docs/deployment.md): Production deployment guidelines

## Optional

- [Architecture Documentation](docs/architecture.md): Detailed system architecture
- [Design Decisions](docs/decisions.md): Historical design decision records
```
##成功标准

创建的`llms.txt`文件应该：
1. 使llm能够快速理解存储库的目的
2. 提供重要文档的清晰导航
3. 完全遵循官方llms.txt规范
4. 全面而简洁
5. 有效地为人类和机器读者服务
6. 包括项目理解的所有关键文件
7. 自始至终使用清晰、明确的语言
8. 逻辑地组织内容以方便使用