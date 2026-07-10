---
title: 'Creating Effective Skills'
description: 'Master the art of writing reusable, shareable skill folders that deliver consistent results across your team.'
authors:
  - GitHub Copilot Learning Hub Team
lastUpdated: 2026-06-30
estimatedReadingTime: '9 minutes'
tags:
  - skills
  - customization
  - fundamentals
relatedArticles:
  - ./what-are-agents-skills-instructions.md
  - ./defining-custom-instructions.md
prerequisites:
  - Basic understanding of GitHub Copilot chat
---
技能是自包含的文件夹，它将可重用功能（指令、参考文件、模板和脚本）打包到一个单元中，代理可以自动发现该单元，用户可以通过斜杠命令调用该单元。它们使团队能够标准化常见的工作流，如生成测试、审查代码或创建文档，确保所有团队成员之间一致的高质量结果。

本文向您展示了如何设计、构建和优化解决实际开发挑战的技能。

什么是技能？

技能是包含`SKILL.md`文件和可选捆绑资产的文件夹。`SKILL.md`定义：- **Name**：一个kebab-case标识符，相当于用户调用的`/command`的两倍（例如，`/generate-tests`）
- **描述**：什么技能完成，什么时候应该触发
- **说明**：详细的工作流程副驾驶执行
—**资产引用**：绑定模板、脚本、模式和参考文档的链接

**较旧的提示文件格式的主要优点**：
** -代理可以自动查找和调用技能，而提示需要手动斜杠命令调用
技能可以捆绑额外的文件（参考文档，模板，脚本）和他们的指令，给AI更丰富的背景
通过开放的[代理技能规范](https://agentskills.io/home)，技能在编码代理系统中更加规范化
技能仍然支持**斜杠命令调用**，就像提示一样技能与其他自定义的区别

**技能与说明**：
-明确调用技能（由代理或用户）；说明自动生效
-技能驱动特定任务与捆绑资源；指令提供持续的上下文
-使用技能为您触发的工作流程的需求；使用始终适用的标准说明

**技能vs座席**：
-技能是专注于任务的能力；代理是专门的角色
-技能工作与标准副驾驶工具和捆绑自己的资产；代理可能需要MCP服务器或自定义集成
-运用技能完成可重复的任务；对需要持久状态的复杂多步骤工作流使用代理

##剖析技能

每一个有效的技能都有两个部分：一个包含标题和说明的`SKILL.md`文件，以及可选的捆绑资产。SKILL.md结构

**示例-简单技能** (`skills/generate-tests/SKILL.md`)：```markdown
---
name: generate-tests
description: 'Generate comprehensive unit tests for the selected code, covering happy path, edge cases, and error conditions'
---

# generate-tests

Generate comprehensive unit tests for the selected code.

## When to Use This Skill

Use this skill when you need to create or expand test coverage for existing code.

## Requirements

- Cover happy path, edge cases, and error conditions
- Use the testing framework already present in the codebase
- Follow existing test file naming conventions
- Include descriptive test names explaining what is being tested
- Add assertions for all expected behaviors
```
**为何有效**：
—清除`name`字段提供斜杠命令标识符
-丰富的`description`告诉座席何时调用此技能
-结构化指令提供具体的、可操作的指导
-通用性足以跨不同的项目工作

添加捆绑资产

技能可以包括参考文件、模板和脚本，以丰富AI的背景：```
skills/
└── generate-tests/
    ├── SKILL.md
    ├── references/
    │   └── testing-patterns.md      # Common testing patterns
    ├── templates/
    │   └── test-template.ts         # Starter test file
    └── scripts/
        └── setup-test-env.sh        # Environment setup
```
在SKILL.md指令中引用这些资产：```markdown
## Testing Patterns

Follow the patterns documented in [references/testing-patterns.md](references/testing-patterns.md).

Use [templates/test-template.ts](templates/test-template.ts) as a starting structure.
```
## Frontmatter配置

YAML前端内容控制着Copilot如何发现和执行你的技能。

必填字段

**name**：与文件夹名称匹配的串大小写标识符```yaml
name: generate-tests
```
**description**：该技能的作用和使用时间的简要总结（10-1024个字符，用单引号括起来）```yaml
description: 'Generate comprehensive unit tests for a component, covering happy path, edge cases, and error conditions'
```
最佳实践描述`description`字段对于代理发现至关重要。编写它，以便代理理解何时调用该技能：

✅**好**:`'Generate conventional commit messages by analyzing staged git changes and applying the Conventional Commits specification'`❌**可怜**:`'Commit helper'`包括触发关键字和上下文线索，帮助代理将技能与用户意图相匹配。

###可选字段

**argument-hint** *(v1.0.64+)*：出现在斜杠命令输入占位符中的短标签，用于指导用户提供什么参数。例如，`generate-tests`技能可以设置：```yaml
argument-hint: 'Enter function or file to test'
```
当用户在VS CodeChat中输入`/generate-tests`时，提示将作为占位符文本出现在输入框中，从而使预期的输入立即变得明显。```yaml
---
name: generate-tests
description: 'Generate comprehensive unit tests for the selected code, covering happy path, edge cases, and error conditions'
argument-hint: 'Enter function, class, or file to test'
---
```
##来自存储库的真实示例

awesome-copilot存储库包括演示生产模式的技能文件夹。

常规提交

参见[`skills/conventional-commit/SKILL.md`]（https://github.com/github/awesome-copilot/tree/main/skills/conventional-commit）自动提交消息：```markdown
---
name: conventional-commit
description: 'Generate conventional commit messages from staged changes following the Conventional Commits specification'
---

# conventional-commit

## Workflow

Follow these steps:

1. Run `git status` to review changed files
2. Run `git diff --cached` to inspect changes
3. Construct commit message using Conventional Commits format
4. Execute commit command automatically

## Commit Message Structure

<type>(scope): description

Types: feat|fix|docs|style|refactor|perf|test|build|ci|chore

## Examples

- feat(parser): add ability to parse arrays
- fix(ui): correct button alignment
- docs: update README with usage instructions
```
该技能使用代理可以自动发现和调用的经过验证的模板自动执行重复性任务（编写提交消息）。

使用捆绑资产生成图表

参见[`skills/excalidraw-diagram-generator/`]（https://github.com/github/awesome-copilot/tree/main/skills/excalidraw-diagram-generator）获得具有丰富捆绑资源的技能：```
excalidraw-diagram-generator/
├── SKILL.md
├── references/
│   ├── excalidraw-schema.md
│   └── element-types.md
├── templates/
│   ├── flowchart-template.json
│   └── relationship-template.json
└── scripts/
    └── split-excalidraw-library.py
```
该技能将模式文档、初学者模板和实用程序脚本打包，因此AI拥有生成有效图表所需的一切，而不会对格式产生幻觉。

编写有效的技能说明

构建你的技能

* * 1。从明确的目标开始：```markdown
# skill-name

Your goal is to [specific task] for [specific target].
```
* * 2。添加“何时使用”指导**（有助于发现代理）：```markdown
## When to Use This Skill

Use this skill when:
- A user asks to [trigger phrase 1]
- You need to [trigger phrase 2]
- Keywords: [keyword1], [keyword2], [keyword3]
```
* * 3。明确定义需求**：```markdown
## Requirements

- Must follow [standard/pattern]
- Should include [specific element]
- Avoid [anti-pattern]
```
* * 4。参考捆绑资产**：```markdown
## References

- Follow patterns in [references/patterns.md](references/patterns.md)
- Use template from [templates/starter.json](templates/starter.json)
```
* * 5。提供例子* *:```markdown
### Good Example
[Show desired output]

### What to Avoid
[Show problematic patterns]
```
最佳实践

- **每个技能一个目的**：专注于单一任务或工作流程
- **为发现而写**：带有触发关键字的工艺描述，以便代理找到正确的技能
- **捆绑重要的东西**：包括模板，模式和参考文档，减少幻觉
- **通用**：写下在不同项目中都适用的技能
- **明确**：避免模棱两可的语言；明确要求
- **名称描述性**：使用清晰，面向操作的名称：`generate-tests`，而不是`helper`- **保持资产精简**：捆绑的文件应该在5mb以下
**彻底测试**：验证不同输入和代码库的技能工作

写作风格指南

**使用祈使语气**：
-✅“为选定的功能生成单元测试”
-❌“你应该生成一些测试”**具体要求**：
-✅“使用Jest与React测试库”
-❌“使用任何测试框架”

* * * *提供防护:
-✅"不要修改现有的测试文件；创造新的。”
-❌“根据需要更新测试”

**结构复杂技能**：```markdown
## Step 1: Analysis
[Analyze requirements]

## Step 2: Generation
[Generate code]

## Step 3: Validation
[Check output]
```
##常见模式

多步骤工作流程与参考```markdown
---
name: scaffold-feature
description: 'Scaffold a new feature with implementation, tests, and documentation following project conventions'
---

# scaffold-feature

Create a complete feature implementation:

1. **Analyze**: Review existing patterns in codebase
2. **Generate**: Create implementation files following project structure
3. **Test**: Generate comprehensive test coverage using [references/test-patterns.md](references/test-patterns.md)
4. **Document**: Add inline comments and update relevant docs
5. **Validate**: Check for common issues and anti-patterns

Use the existing code style and conventions found in the codebase.
```
快速分析技能```markdown
---
name: explain-architecture
description: 'Analyze and explain code architecture, design patterns, and data flow for selected code'
---

# explain-architecture

Analyze the selected code and explain:

1. Overall architecture and design patterns used
2. Key components and their responsibilities
3. Data flow and dependencies
4. Potential improvements or concerns

Keep explanations concise and developer-focused.
```
技能与脚本资产```markdown
---
name: run-test-suite
description: 'Execute the project test suite, parse failures, and suggest fixes for failing tests'
---

# run-test-suite

Execute the project's test suite:

1. Identify the test command from package.json or build files
2. Run tests in the integrated terminal
3. Parse test output for failures
4. Summarize failed tests with relevant file locations
5. Suggest potential fixes based on error messages

Use [scripts/parse-test-output.sh](scripts/parse-test-output.sh) to extract structured failure data.
```
##常见问题

**问：我如何调用技能？**

答：技能可以通过几种方式调用：
- **斜杠命令**：在您的消息中键入技能名称的任何地方（例如，`/generate-tests fix the failing tests`）。从v1.0.44开始，斜杠命令可以在输入过程中出现-您不必从它们开始。
- **一条消息中包含多个技能**：您可以在一条消息中调用多个技能（例如，`/generate-tests and then /conventional-commit`）。这两个技能将按顺序执行。
- **代理发现**：代理也可以根据技能的`description`和用户的意图自动发现和调用技能-不需要斜杠命令。

**问：如何在CLI下管理技能？**

答：`copilot skill`子命令（v1.0.65+）允许您直接从终端列出，添加和删除技能，而无需手动编辑配置文件。```bash
copilot skill list                      # list all currently loaded skills
copilot skill add ./my-skill/           # add a skill from a local directory
copilot skill add https://example.com/skill.zip  # add a skill from a URL
copilot skill remove my-skill           # remove an installed skill by name
```
您还可以在交互式会话中运行`/skill`（或现有的`/skills`），以查看加载的内容。`copilot skill`子命令是安装未打包在插件中的技能的推荐方法。

**Q：技能和提示有什么不同？**

答：技能替换旧的提示文件（`*.prompt.md`）格式。技能提供了代理发现（提示是手动的）、捆绑资产（提示是单个文件），以及通过代理技能规范的跨平台可移植性。如果您有现有的提示，请考虑将它们迁移到技能中。

**Q：技能可以包含多个文件吗？**

答:是的!技能是文件夹，而不是单个文件。您可以捆绑参考文档、模板、脚本和AI所需的任何其他资源。将单个资产保持在5 MB以下。

**问：我如何与团队分享技能？**答：将技能文件夹存储在存储库的`.github/skills/`目录中。当在该存储库中工作时，所有具有Copilot访问权限的团队成员都可以自动使用它们。

**Q：我可以在一条消息中调用多个技能吗？**

A：是的，从1.0.44版本开始。您可以在单个消息中包含多个斜杠命令（例如，`/generate-tests and then /conventional-commit`）， CLI将按顺序执行每个技能。代理还可以根据用户的意图在会话期间发现和链接多个技能。每个技能调用都是独立的，但是代理维护跨调用的对话上下文。

**Q：技能应该包括代码示例吗？**

A：是的，为了清楚起见。展示所需输出格式、要遵循的模式或要避免的反模式的示例。对于复杂的模式或格式，考虑将它们捆绑为参考文件，而不是内联示例。

**问：我如何审查代理提出的技能更改？**答：在v1.0.66+中，代理可以在会话中发现可重用模式时提出草稿技能添加或改进。与以下人员互动审阅每一份草稿：```
/chronicle skills review
```
这打开了一个审查流程，您可以接受，拒绝或推迟每个建议的更改-让您完全控制您的技能库如何发展。在您批准之前，不会应用任何更改。

要避免的常见陷阱

-❌**模糊描述**：“代码助手”不会帮助代理发现技能
✅**代替**：用触发器关键字写描述：“生成涵盖正常路径、边缘情况和错误条件的全面单元测试”

-❌**缺少捆绑的资源**：期望AI知道你的测试模式或模式
✅**代替**：在技能文件夹中捆绑引用文档和模板

-❌**责任太多**：生成、测试、文档和部署的技能
✅**代替**：为每个关注点创建重点技能-❌**硬编码路径**：在技能说明中引用具体的项目文件路径
✅**代替**：编写跨项目工作的通用指令

-❌**没有例子**：抽象的需求，没有具体的指导
✅**代替**：包括“好例子”和“避免什么”部分，或包模板

##下一步

既然你了解了有效的技能，你就可以：

- **探索存储库示例**：浏览[技能目录]（../../skills/）的生产技能涵盖不同的工作流程
- **了解代理**:[建立自定义代理](../building-custom-agents/) -何时从技能升级到完整的代理
- **理解指令**:[定义自定义指令](../defining-custom-instructions/) -补充技能与自动上下文
- **决策框架**：选择正确的自定义_（即将推出）_ -何时使用技能与其他类型**建议阅读顺序**：
1. 这篇文章（创建有效的技能）
2. [建立海关代理](../building-custom-agents/) -更复杂的工作流程
3. 选择正确的定制_（即将推出）_ -决策指导

---