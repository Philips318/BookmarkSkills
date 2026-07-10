---
title: 'Understanding Copilot Context'
description: 'Learn how GitHub Copilot uses context from your code, workspace, and conversation to generate relevant suggestions.'
authors:
  - GitHub Copilot Learning Hub Team
lastUpdated: 2025-11-28
estimatedReadingTime: '8 minutes'
tags:
  - context
  - fundamentals
  - how-it-works
relatedArticles:
  - ./what-are-agents-skills-instructions.md
---
上下文是GitHub Copilot生成相关、准确建议的基础。了解Copilot“看到”了什么，以及它如何使用这些信息，可以帮助你编写更好的提示，获得更高质量的完成，并在人工智能的帮助下更有效地工作。本文解释了Copilot使用的上下文类型，以及如何优化您的开发环境以获得更好的结果。

##副驾驶看到的

当GitHub Copilot生成建议或响应聊天消息时，它会分析来自开发环境的多个信息源：

**打开文件**:Copilot可以访问当前在编辑器中打开的文件的内容。让相关文件可见，可以让Copilot了解你的代码库结构、命名约定和编码模式。**当前光标位置**：光标的确切位置很重要。副驾驶会考虑周围的代码，包括之前和之后的代码，以了解您当前的意图，并生成适合上下文的建议。

**相关文件**：通过导入、引用和依赖项，Copilot识别与您当前工作相关的文件。例如，如果您正在编辑一个导入实用程序函数的组件，Copilot可能会引用该实用程序文件来了解可用的功能。

**聊天记录**：在GitHub Copilot聊天中，之前的对话信息为后续问题提供了上下文。这允许自然的、迭代的解决问题，其中每个响应都建立在先前的交换基础上。**工作区结构：项目的组织——目录结构、配置文件和模式——有助于Copilot理解你正在进行的项目类型，并遵循适当的惯例。

##上下文类型GitHub Copilot利用四种不同类型的上下文来提供建议：

编辑上下文

编辑器上下文包括编辑器中显示的活动文件和屏幕上可见的特定代码。当你在选项卡或分割视图中打开多个文件时，Copilot可以引用所有这些文件来提供更明智的建议。

**示例**：如果你正在编写一个函数，从另一个打开的文件中定义的类调用方法，Copilot可以通过引用该类定义来建议正确的方法名称和参数类型。

语义上下文语义上下文超越了原始文本，可以理解代码中的含义和关系。这包括函数签名、类型定义、接口契约、类层次结构和解释复杂逻辑的内联注释。

**示例**：当您实现接口时，Copilot使用接口定义作为语义上下文来建议具有适当参数类型和返回值的正确方法签名。

###对话上下文

在GitHub CopilotChat中，对话上下文包括当前聊天会话中以前的所有消息、问题和响应。这使得你可以在问“错误处理怎么样？”时进行上下文跟踪，而Copilot就会理解你指的是前面讨论过的代码。**示例**：在要求Copilot生成数据库查询功能后，您可以在后续操作中添加“添加错误处理和日志记录”，而无需重复完整的上下文—Copilot会记住之前的交流。

###工作区上下文

工作区上下文包括项目级信息，如目录结构、配置文件（`.gitignore`、`package.json`、`tsconfig.json`）和整个存储库组织。这有助于Copilot理解您的项目类型、依赖关系和约定。

**示例**：如果你的工作区包含一个带有TypeScript和React依赖的`package.json`， Copilot会识别出这是一个TypeScript React项目，并使用适当的模式和类型生成建议。

背景如何影响建议

上下文直接影响GitHub Copilot建议的相关性、准确性和有用性。更多的背景通常会带来更好的建议。示例：带有上下文的代码完成

**没有上下文**（只打开当前文件）：```typescript
// user.ts
function getUserById(id: string) {
  // Copilot might suggest generic database code
  const user = db.query('SELECT * FROM users WHERE id = ?', [id]);
  return user;
}
```
**与上下文**（数据库实用程序文件也打开）：```typescript
// database.ts (open in another tab)
export async function queryOne<T>(sql: string, params: any[]): Promise<T | null> {
  // ... implementation
}

// user.ts (current file)
function getUserById(id: string) {
  // Copilot now suggests using the existing utility
  return queryOne<User>('SELECT * FROM users WHERE id = ?', [id]);
}
```
通过打开`database.ts`文件，Copilot可以识别现有的实用程序函数，并建议使用它而不是生成通用数据库代码。

示例：使用文件引用进行聊天

* *没有@-mention * *:```
You: How do I handle validation?

Copilot: Here's a general approach to validation...
[provides generic validation code]
```
与#提到* * * *:```
You: How do I handle validation in #user-service.ts?

Copilot: Based on your UserService class, you can add validation like this...
[provides code specific to your UserService implementation]
```
使用`#`来引用特定的文件可以让Copilot精确地了解您询问的代码。

令牌限制和上下文优先级GitHub Copilot有一次可以处理多少上下文的最大令牌限制。当你打开了很多文件或有很长的聊天记录时，Copilot会优先处理：

1. **最接近**：代码立即围绕您的光标
2. **显式引用文件**：你在聊天中@-提到的文件对于CLI，和#-提到的ide （VS Code, Visual Studio， JetBrains等）
3. **最近修改的文件**：最近编辑过的文件
4. **直接依赖：由当前文件导入的文件

了解这种优先级可以帮助您优化哪些文件保持打开状态，以及何时使用显式引用。

上下文最佳实践

通过提供清晰、相关的上下文，最大化GitHub Copilot的有效性：**保持相关文件打开**：如果你正在处理一个组件，保持它的测试文件、相关的实用程序和类型定义在选项卡或分割视图中打开。

**使用描述性名称**：选择清晰的变量名、函数名和类名，以传达意图。`getUserProfile()`提供了比`getData()`更多的上下文。

**添加澄清注释**：对于复杂的算法或业务逻辑，编写注释来解释代码背后的“为什么”。副驾驶用这些来理解你的意图。

**逻辑地组织您的工作空间**：在反映应用程序架构的有意义的目录中组织文件。清晰的结构有助于副驾驶理解组件之间的关系。

**在聊天中使用#-mention **：在提出问题时，明确地使用`#filename`引用文件，以确保Copilot分析您正在讨论的确切代码。**在提示中提供示例**：当要求Copilot生成代码时，包括您现有模式和约定的示例。

##常见问题

问：Copilot能看到我的整个存储库吗？**

答：不，Copilot不会自动分析存储库中的所有文件。它侧重于打开的文件、最近修改的文件和当前工作直接引用的文件。对于大型代码库，这种选择性方法可确保快速响应时间，同时仍然提供相关的上下文。

**问：我如何知道副驾驶正在使用什么上下文？**

答：在GitHub Copilot聊天中，您可以看到响应中引用了哪些文件。当Copilot生成建议时，它主要使用你当前打开的文件和你光标周围的代码。在chat中使用`#codebase`显式地搜索整个存储库。

**问：我可以控制包含哪些上下文吗？**A：是的，你有几种方法来控制上下文：
-Open/close文件改变什么是可用的副驾驶
-使用`#`来明确引用特定的文件、符号或函数
—配置`.gitignore`将文件从工作空间上下文中排除
-使用说明和技能为特定场景提供持久的上下文

**Q：关闭文件是否会将其从上下文中删除？**

答：是的，关闭文件可以将其从Copilot的活动上下文中删除。但是，您最近处理过的文件可能仍然会对建议产生短暂的影响。要进行干净的上下文重置，您可以重新启动编辑器或启动新的聊天会话。

##下一步

现在您已经了解了上下文在GitHub Copilot中的工作原理，接下来探索以下相关主题：- **[什么是代理，技能和说明](../what-are-agents-skills-instructions/)** -了解提供持久上下文的自定义类型
- **[Copilot Configuration Basics](../copilot-configuration-basics/)** -配置设置以优化上下文使用
- **[创造有效的技能](../creating-effective-skills/)** -在你的技能中有效地使用上下文
- **常见陷阱和解决方案** _（即将推出）_ -避免与上下文相关的错误