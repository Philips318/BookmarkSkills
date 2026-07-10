---
name: update-llms
description: 'Update the llms.txt file in the root folder to reflect changes in documentation or specifications following the llms.txt specification at https://llmstxt.org/'
---
#更新LLMs.txt文件

更新存储库根目录中的现有`llms.txt`文件，以反映文档、规范或存储库结构中的更改。该文件为大型语言模型（llm）提供了高级指导，告诉它们在哪里可以找到相关的内容，以理解存储库的目的和规范。

##主要指令

更新现有的`llms.txt`文件，以保持llms.txt规范的准确性和遵从性，同时反映当前的存储库结构和内容。该文件必须保持为LLM消费而优化，同时保持人类可读。

分析和计划阶段

在更新`llms.txt`文件之前，必须完成一个彻底的分析：步骤1：审查当前文件和规范
-读取现有的`llms.txt`文件，以了解当前的结构
-审查官方规范在https://llmstxt.org/，以确保持续合规
-根据存储库更改确定可能需要更新的区域

步骤2：存储库结构分析
-使用适当的工具检查当前的存储库结构
-比较当前的结构与现有的`llms.txt`文件
-确定应该包含的新目录、文件或文档
-注意任何需要更新的删除或重新定位的文件步骤3：内容发现和变更检测
-识别新的自述文件及其位置
-查找新的文档文件（`.md`文件在`/docs/`，`/spec/`等）
定位新的规范文件及其用途
—发现新的配置文件及其相关性
-查找新的示例文件和代码示例
识别现有文档结构的任何更改

步骤4：创建更新计划
根据你的分析，创建一个结构化的计划，包括：
-保持准确性所需的更改
—添加到“llms.txt. zip”文件的新文件
-过时的引用将被删除或更新
-组织改进以保持清晰度

##实现要求

格式遵从性
根据规范，更新后的`llms.txt`文件必须保持这个精确的结构：1. **H1标题**：单行repository/project名称（必选）
2. **Blockquote Summary**: Blockquote格式的简短描述（可选但推荐）
3. **附加细节**：零个或多个没有上下文标题的降价节
4. **文件列表节**：零或多个H2节包含链接的标记列表

内容要求

####
- **项目名称**：清晰，描述性标题为H1
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

####文件选择标准
包括以下文件：
-解释存储库的目的和范围
-提供必要的技术文件
-展示用法示例和模式
—定义接口和规范
—包含配置和安装说明排除下列文件：
-纯粹是实现细节
—包含冗余信息
是构建工件或生成的内容
-与理解项目无关

##执行步骤

步骤1：当前状态分析
1. 彻底阅读现有的`llms.txt`文件
2. 彻底检查当前的存储库结构
3. 将现有文件引用与实际存储库内容进行比较
4. 找出过时的、缺失的或不正确的参考资料
5. 注意当前文件的任何结构问题

步骤2：内容规划
1. 确定主目的语句是否需要更新
2. 如有需要，检查并更新汇总报价
3. 计划添加新文件和目录
4. 计划删除过时或已移动的内容
5. 如果需要的话，重新组织章节，使其更清晰###步骤3：文件更新
1. 更新存储库根目录中现有的`llms.txt`文件
2. 保持与精确格式规范的遵从性
3. 添加带有适当描述的新文件引用
4. 删除或更新过时的引用
5. 确保所有链接都是有效的相对路径

###步骤4：验证
1. 验证https://llmstxt.org/规范的持续遵从性
2. 检查所有链接是否有效和可访问
3. 确保该文件仍然是一个有效的LLM导航工具
4. 确认文件仍然是人类和机器可读的

##质量保证

格式验证
-✅H1标头与项目名称
-✅Blockquote摘要（如果包含）
-✅H2部分的文件列表
-✅正确的降价链接格式
-✅没有损坏或无效的链接
-✅始终保持格式一致###内容验证
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

##更新策略

添加过程
添加新内容时：
1. 为新文件确定适当的部分
2. 为链接创建清晰、描述性的名称
3. 写简洁但信息丰富的描述
4. 在各部分中保持字母顺序或逻辑顺序
5. 考虑新的内容类型是否需要新的部分移除过程
删除过时内容时：
1. 验证文件实际上已被删除或重新定位
2. 检查重新定位的文件是否应该更新而不是删除
3. 如果它们变成空的，删除整个部分
4. 如果需要，更新交叉引用

重组过程
重组内容时：
1. 维护从一般到特定的逻辑流
2. 将必要的文档放在主要部分
3. 如果合适的话，将次要内容移到“Optional”部分
4. 确保新组织改进LLM导航`llms.txt`的结构示例：```txt
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

更新后的`llms.txt`文件应该：
1. 准确地反映当前的存储库结构和内容
2. 保持与llms.txt规范的遵从性
3. 提供重要文档的清晰导航
4. 删除过时或不正确的引用
5. 包括新的重要文件和文档
6. 维护逻辑组织以方便LLM的使用
7. 自始至终使用清晰、明确的语言
8. 继续有效地为人类和机器读者服务