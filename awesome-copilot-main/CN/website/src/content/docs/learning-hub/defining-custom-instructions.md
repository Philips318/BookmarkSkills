---
title: 'Defining Custom Instructions'
description: 'Learn how to create persistent, context-aware instructions that guide GitHub Copilot automatically across your codebase.'
authors:
  - GitHub Copilot Learning Hub Team
lastUpdated: 2026-07-01
estimatedReadingTime: '8 minutes'
tags:
  - instructions
  - customization
  - fundamentals
relatedArticles:
  - ./what-are-agents-skills-instructions.md
  - ./creating-effective-skills.md
  - ./copilot-configuration-basics.md
prerequisites:
  - Basic understanding of GitHub Copilot features
---
自定义指令是持久的配置文件，当使用代码库中的特定文件或目录时，它会自动指导GitHub Copilot的行为。与需要显式调用（由用户或代理）的技能不同，指令在后台默默地工作，确保Copilot始终遵循团队的标准、惯例和架构决策。

本文解释了如何创建有效的自定义指令，何时使用它们，以及它们如何与您的开发工作流集成。

什么是自定义说明？

自定义指令是标记文件（`.instructions.md`），其中包含：- **编码标准**：命名约定，格式规则，风格指南
- **框架特定指导**：技术堆栈的最佳实践
架构决策：项目结构、设计模式、约定
- **合规性要求**：安全策略、法规约束

* * * *要点:
-当副驾驶在匹配文件上工作时，说明自动应用
-它们在所有聊天会话和内联补全中持续存在
-它们可以使用全局模式对全局、每种语言或每个目录进行作用域
-它们可以帮助Copilot在没有手动提示的情况下理解您的代码库的独特上下文

指令与其他自定义有何不同**说明与技能**：
-指令总是激活匹配文件；技能需要显式调用（由用户或代理）
-指令提供被动语境；技能驱动特定的任务与捆绑的资源
-重复使用的标准使用说明；使用按需操作的技能

**说明与代理**：
-指令是轻量级上下文；代理是具有工具访问权限的专门角色
-指示与任何副驾驶互动；代理人需要明确的选择
-编码标准的使用说明；对需要工具的复杂工作流使用代理

创建你的第一个自定义指令

自定义说明遵循一个简单的结构，带有YAML标题和标记内容。

* * * *例子:````markdown
---
description: 'TypeScript coding standards for React components'
applyTo: '**/*.tsx, **/*.ts'
---

# TypeScript React Development

Use functional components with TypeScript interfaces for all props.

## Naming Conventions

- Component files: PascalCase (e.g., `UserProfile.tsx`)
- Hook files: camelCase with `use` prefix (e.g., `useAuth.ts`)
- Type files: PascalCase with descriptive names (e.g., `UserTypes.ts`)

## Component Structure

Always define prop interfaces explicitly:

```typescript
接口UserProfileProps
用户标识:字符串;
onUpdate: (user: user) => void；
}

导出函数UserProfile({userId, onUpdate}: UserProfileProps) {
/ /实现
}```
````
最佳实践

—单独导出类型，以便跨组件重用
-使用React。只有在需要子类型输入时才使用FC
-首选命名导出，而不是默认导出

**为何有效**：
—`applyTo`glob模式专门针对TypeScript/TSX文件
-每当生成或建议匹配文件的代码时，副驾驶都会读取这些说明
标准的执行是一致的，开发人员不需要记住每条规则
-新团队成员自动受益于机构知识

有效地限定指令的范围`applyTo`字段确定哪些文件接收指令的指导。它接受**逗号分隔的字符串**或**全局模式数组** -两种格式都可以：```yaml
# String format (comma-separated)
applyTo: '**/*.ts, **/*.tsx'

# Array format
applyTo:
  - '**/*.ts'
  - '**/*.tsx'
```
###通用范围模式

**所有TypeScript文件：```yaml
applyTo: '**/*.ts, **/*.tsx'
```
* *特定目录* *:```yaml
applyTo: 'src/components/**/*.tsx'
```
**仅限测试文件**：```yaml
applyTo: '**/*.test.ts, **/*.spec.ts'
```
* * * *单一的技术:```yaml
applyTo: '**/*.py'
```
* *整个项目* *:```yaml
applyTo: '**'
```
* * * *预期的结果:
当你处理与模式匹配的文件时，Copilot会自动将该指令的上下文整合到建议和聊天响应中。

使用@ style导入组合指令

*(v1.0.66+)* Copilot CLI支持**@ style导入**在指令文件，AGENTS.md，和CLAUDE.md。使用一个裸`@path/to/file.md`引用来嵌入另一个文件的内容：```markdown
---
description: 'Full TypeScript standards for this project'
applyTo: '**/*.ts, **/*.tsx'
---

@shared/base-coding-standards.md
@shared/security-rules.md

## TypeScript-Specific Rules

- Prefer `interface` over `type` for object shapes
- Always provide return types for public functions
```
当指令文件被加载时，被引用的文件被读取，它们的内容在被发送到模型之前被内联替换。

**为什么这很重要**：
- **DRY原则**：维护一个共享的`security-rules.md`，并将其导入到每条指令中，而不是复制它
- **模块化组成**：从小的，集中的文件构建复杂的指令
- **简单更新**：编辑共享文件一次；每个导入它的指令都会自动获取更改

* * * *的小贴士:
-路径解析相对于指令文件的位置
-引用的文件不需要是指令文件本身-普通Markdown文件工作
-导入可以嵌套（你导入的文件可以自己导入其他文件）



awesome-copilot-hub存储库包含120多个演示真实世界模式的指令文件。

安全标准综合安全指导见[security-and-owasp.instructions.md](https://github.com/github/awesome-copilot/blob/main/instructions/security-and-owasp.instructions.md)：```markdown
---
description: 'OWASP Top 10 security practices for all code'
applyTo: '**'
---

# Security and OWASP Best Practices

Always validate and sanitize user input before processing.

## Input Validation

- Whitelist acceptable input patterns
- Reject unexpected formats early
- Never trust client-side validation alone
- Use parameterized queries for database operations
```
此说明适用于所有文件（`applyTo: '**'`），确保每个建议都具有安全意识。

特定于框架的指导

参见[reactjs.instructions.md]（https://github.com/github/awesome-copilot/blob/main/instructions/reactjs.instructions.md）了解特定于react的模式：```markdown
---
description: 'React development best practices and patterns'
applyTo: '**/*.jsx, **/*.tsx'
---

# React Development Guidelines

Use functional components with hooks for all new components.

## State Management

- Use `useState` for local component state
- Use `useContext` for shared state across components
- Consider Redux only for complex global state
- Avoid prop drilling beyond 2-3 levels
```
该指令仅针对React组件文件，提供特定于上下文的指导。

测试标准

参见[playwright-typescript.instructions.md]（https://github.com/github/awesome-copilot/blob/main/instructions/playwright-typescript.instructions.md）了解测试自动化模式：````markdown
---
description: 'Playwright test automation with TypeScript'
applyTo: '**/*.spec.ts, **/tests/**/*.ts'
---

# Playwright Testing Standards

Write descriptive test names that explain the expected behavior.

## Test Structure

```typescript
Test(‘当登录失败时应该显示错误消息’，async ({page}) => {
等待page.goto(' /登录');
等待page.fill('#username', 'invalid')；
等待page.fill('#password', 'invalid')；
等待page.click(#提交);

等待期待(page.locator (' . error ')) .toBeVisible ();
});```
````
此指令仅适用于测试文件，以确保特定于测试的上下文。

构建指令内容

有效的组织

一个结构良好的指令文件包括：

1. **清晰的标题和概述**：本说明书涵盖的内容
2. **具体的指导方针**：可操作的规则，而不是模糊的建议
3. 代码示例**：显示正确模式的工作片段
4. **解释**：为什么某些方法是首选的

写作风格最佳实践

- **具体**：“使用PascalCase组件名”而不是“命名组件好”
- **显示示例**：包括演示模式的工作代码片段
- **解释推理**：简短的上下文有助于副驾驶理解意图
- **保持简洁：专注于最重要的事情；避免详尽的文档

**例子-模糊vs具体**：

❌**模糊**：“正确处理错误”

✅特定* * * *:````markdown
## Error Handling

Wrap async operations in try-catch blocks and log errors:

```typescript
尝试{
const data = await fetchUser(userId)；
返回数据;
} catch (error) {
记录器。错误（“获取用户失败”，{userId，错误}）；
抛出新的UserNotFoundError(userId)；
}```
````
##常见问题

**Q：我应该创建多少个指令？**

答：从3-5个核心说明开始，涵盖你最重要的标准（命名、结构、安全性）。随着模式的出现，添加更多。对于一个中等规模的项目来说，拥有10-20个指令是合理的。令人敬畏的副驾驶存储库包含超过120来展示可能性的范围。

**Q：指示会使副驾驶减速吗？**

答:不是。作为副驾驶上下文窗口的一部分，指令被有效地处理。保持单个文件的重点（500行以下）以获得最佳结果，并确保它们的范围适当。

**Q：指令之间会有矛盾吗？**答：如果多个指令适用于同一个文件，副驾驶会考虑所有指令。通过保持指令集中和使用特定的`applyTo`模式来避免矛盾。更具体的模式优先考虑，但最好设计补充指令。

**问：我如何知道我的指示是否有效？**

答：通过要求副驾驶生成与您的模式匹配的代码进行测试。如果它在没有明确提示的情况下遵循你的标准，说明是有效的。你也可以在chat中显式地引用该指令：“按照我的指令中的TypeScript标准，创建一个用户组件。”

**问：我应该把所有的东西都记录在说明书里吗？**

答:不是。指令用于重复应用的持久标准。在代码注释中记录一次性决定。使用指示模式，您希望副驾驶自动遵循。

最佳实践- **每个文件一个目的**：为不同的关注点（安全，测试，样式）创建单独的指令
- **使用清晰的命名**：文件名描述：`react-component-standards.instructions.md`，而不是`rules.instructions.md`- **包括示例**：每个指南应该至少有一个代码示例
- **保持最新**：当依赖项或框架更新时检查说明
- **测试您的指令**：生成代码并验证副驾驶遵循模式
- **文档链接**：详细说明请参考官方文档
- **使用表格的规则**：表格格式工作良好的命名约定和比较

要避免的常见陷阱

-❌**太笼统了：“写干净的代码”并没有给副驾驶可操作的指导
✅**代替**：提供特定的模式：“将超过20行的函数提取为更小的命名函数”-❌**太冗长**：包括整个文档页压倒上下文窗口
✅**代替**：提取关键模式并链接到完整文档

-❌**矛盾的规则**：不同的说明建议相反的方法
✅**代替**：设计具有明确范围的补充说明

-❌**过时的模式**：引用已弃用的api或旧版本的指令
✅**代替**：当依赖项更改时，检查和更新说明

-❌**缺少作用域**：使用`applyTo: '**'`作为特定于语言的指南
✅**代替**:Scope到相关文件：`applyTo: '**/*.py'`对于特定于python的规则

##下一步

既然您了解了自定义指令，您可以：- **探索存储库示例**：浏览[说明目录](../../instructions/) -超过120个真实世界的例子，涵盖框架，语言和领域
- **学习技能**:[创造有效技能](../creating-effective-skills/) -发现何时使用技能而不是说明
- **了解代理**:[构建自定义代理](../building-custom-agents/) -了解代理如何补充复杂工作流程的说明
- **配置基础知识**:[副驾驶配置基础知识](../copilot-configuration-basics/) -学习如何组织和管理您的自定义

**建议阅读顺序**：
1. 本文（定义自定义指令）
2. [创造有效技能](../creating-effective-skills/) -学习互补定制类型
3. [构建自定义代理](../building-custom-agents/) -何时使用每种类型的决策框架