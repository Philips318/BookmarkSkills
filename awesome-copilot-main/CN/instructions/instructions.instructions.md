---
description: 'Guidelines for creating high-quality custom instruction files for GitHub Copilot'
applyTo: '**/*.instructions.md'
---
#自定义说明文件指南

创建有效且可维护的自定义指令文件的说明，这些文件指导GitHub Copilot生成特定于领域的代码并遵循项目约定。

##项目背景

-目标受众：使用领域特定代码的开发人员和GitHub Copilot-文件格式：用YAML字体标记
-文件命名约定：小写带连字符（例如，`react-best-practices.instructions.md`）
—位置：`.github/instructions/`目录
-目的：为代码生成，审查和文档提供上下文感知指导

##必需的前置事项

每个指令文件必须包含以下字段的YAML frontmatter：```yaml
---
description: 'Brief description of the instruction purpose and scope'
applyTo: 'glob pattern for target files (e.g., **/*.ts, **/*.py)'
---
```
### Frontmatter指南

—**description**：单引号字符串，1 ~ 500个字符，明确说明目的
- **applyTo**：指定这些指令应用于哪些文件的全局模式
—单模式：`'**/*.ts'`—多种模式：`'**/*.ts, **/*.tsx, **/*.js'`—具体文件：`'src/**/*.py'`. xml
—所有文件：`'**'`##文件结构

一个结构良好的指令文件应该包括以下部分：

# # # 1。标题和概述

-清晰，描述性标题使用`#`标题
-简要介绍，说明目的和范围
-可选：包含关键技术和版本的项目上下文部分

# # # 2。核心部分

将内容组织成基于域的逻辑部分：- **总则**：高级指导方针和原则
**最佳实践**：推荐的模式和方法
- **代码标准**：命名约定、格式、样式规则
- **Architecture/Structure**：项目组织设计模式
- **公共模式**：经常使用的实现
- **安全**：安全考虑（如适用）
- **性能**：优化指南（如适用）
- **测试**：测试标准和方法（如适用）

# # # 3。示例和代码片段

提供明确标签的具体例子：```markdown
### Good Example
\`\`\`language
// Recommended approach
code example here
\`\`\`

### Bad Example
\`\`\`language
// Avoid this pattern
code example here
\`\`\`
```
# # # 4。确认和验证（可选但推荐）

-构建命令来验证代码
-检查和格式化工具
-测试要求
-验证步骤

##内容指南

写作风格

-使用清晰、简洁的语言
-用祈使句（“使用”，“实施”，“避免”）
-具体和可操作
避免使用“应该”、“可能”、“可能”等模棱两可的词语
-使用项目符号和列表来提高可读性
-保持各部分的重点和可浏览性

最佳实践- **具体**：提供具体的例子而不是抽象的概念
- **显示原因**：当建议增加价值时，解释其背后的原因
- **使用表**：用于比较选项，列出规则，或显示模式
- **包括示例**：真实的代码片段比描述更有效
- **保持最新**：参考当前版本和最佳实践
- **链接资源**：包括官方文件和权威来源

###指令高度（金发姑娘区）

-从完全定义预期结果的最小规则集开始
-在观察到的失败后添加约束，而不是假设的边缘情况
-比起详尽的决策表，更喜欢高信号的例子

|海拔|故障模式|结果|| --- | --- | --- |
|指定过多|脆弱的if-else语句|在未列出的情况下中断|
|未指定|假设共享上下文|一般输出|
|正确高度|启发式+例子|稳定，可推广的质量|

要包含的常见模式

1. 命名约定：如何命名变量、函数、类、文件
2. **代码组织**：文件结构、模块组织、导入顺序
3. **错误处理**：首选的错误处理模式
4. **依赖性**：如何管理和记录依赖性
5. **注释和文档：何时以及如何编写代码文档
6. **版本信息**：目标language/framework版本

##要遵循的模式

###项目符号和列表```markdown
## Security Best Practices

- Always validate user input before processing
- Use parameterized queries to prevent SQL injection
- Store secrets in environment variables, never in code
- Implement proper authentication and authorization
- Enable HTTPS for all production endpoints
```
结构化信息表```markdown
## Common Issues

| Issue            | Solution            | Example                       |
| ---------------- | ------------------- | ----------------------------- |
| Magic numbers    | Use named constants | `const MAX_RETRIES = 3`       |
| Deep nesting     | Extract functions   | Refactor nested if statements |
| Hardcoded values | Use configuration   | Store API URLs in config      |
```
代码比较```markdown
### Good Example - Using TypeScript interfaces
\`\`\`typescript
interface User {
  id: string;
  name: string;
  email: string;
}

function getUser(id: string): User {
  // Implementation
}
\`\`\`

### Bad Example - Using any type
\`\`\`typescript
function getUser(id: any): any {
  // Loses type safety
}
\`\`\`
```
条件指导```markdown
## Framework Selection

- **For small projects**: Use Minimal API approach
- **For large projects**: Use controller-based architecture with clear separation
- **For microservices**: Consider domain-driven design patterns
```
要避免的模式

- **过于冗长的解释**：保持简洁和可浏览
- **过时信息**：始终引用当前版本和实践
- **模棱两可的指导方针**：具体说明该做什么或避免什么
- **缺少示例**：没有具体代码示例的抽象规则
- **矛盾建议**：确保整个文件的一致性
- **从文档中复制粘贴**：通过提取和上下文化来增加价值
- **假设规则膨胀**：不要为未发生的失败添加规则

##测试你的说明

指令文件定稿前：

1. **测试副驾驶**：在VS Code中尝试使用实际提示的说明
2. **验证示例**：确保代码示例正确且运行无错误
3. **检查全局模式**：确认`applyTo`模式与预期文件匹配

##示例结构下面是一个新指令文件的最小结构示例：```markdown
---
description: 'Brief description of purpose'
applyTo: '**/*.ext'
---

# Technology Name Development

Brief introduction and context.

## General Instructions

- High-level guideline 1
- High-level guideline 2

## Best Practices

- Specific practice 1
- Specific practice 2

## Code Standards

### Naming Conventions
- Rule 1
- Rule 2

### File Organization
- Structure 1
- Structure 2

## Common Patterns

### Pattern 1
Description and example

\`\`\`language
code example
\`\`\`

### Pattern 2
Description and example

## Validation

- Build command: `command to verify`
- Linting: `command to lint`
- Testing: `command to test`
```
# #维护

-当依赖项或框架更新时审查说明
-更新示例以反映当前的最佳实践
-删除过时的模式或弃用的功能
-在社区中出现新模式时添加新模式
随着项目结构的发展，保持全局模式的准确性

##其他资源

-[自定义说明文档]（https://code.visualstudio.com/docs/copilot/customization/custom-instructions）
-[超级副驾驶指南]（https://github.com/github/awesome-copilot/tree/main/instructions）
-[系统提示高度- AI代理的有效上下文工程]（https://www.anthropic.com/engineering/effective-context-engineering-for-ai-agents#the-anatomy-of-effective-context）