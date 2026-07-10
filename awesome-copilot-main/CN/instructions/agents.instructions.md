---
description: 'Guidelines for creating custom agent files for GitHub Copilot'
applyTo: '**/*.agent.md'
---
#自定义代理文件指南

创建有效且可维护的自定义代理文件的说明，这些文件为GitHub Copilot中的特定开发任务提供专门的专业知识。

##项目背景

目标受众：为GitHub Copilot创建自定义代理的开发人员
-文件格式：用YAML字体标记
-文件命名约定：小写带连字符（例如，`test-specialist.agent.md`）
-位置：`.github/agents/`目录（存储库级）或`agents/`目录（organization/enterprise-level）
-目的：为特定任务定义具有量身定制的专业知识，工具和指示的专门代理
—官方文档：https://docs.github.com/en/copilot/how-tos/use-copilot-agents/coding-agent/create-custom-agents##必需的前置事项

每个代理文件必须包含以下字段的YAML标题：```yaml
---
description: 'Brief description of the agent purpose and capabilities'
name: 'Agent Display Name'
tools: ['read', 'edit', 'search']
model: 'Claude Sonnet 4.5'
target: 'vscode'
---
```
Core Frontmatter属性

#### **描述**（必选）
-单引号字符串，清楚地说明代理的目的和领域专长
-应该简洁（50-150字）和可操作的
—例如：`'Focuses on test coverage, quality, and testing best practices'`#### **name**（可选）
—在界面中显示座席的名称
-如果省略，默认为filename（不带`.md`或`.agent.md`）
-使用标题大小写，并具有描述性
—例如：`'Testing Specialist'`#### **tools**（可选）
-代理可以使用的工具名称或别名列表
—支持以逗号分隔的字符串或YAML数组格式
—如果省略，代表可以访问所有可用的工具
—详细信息请参见下面的“工具配置”部分

#### **型号**（强烈推荐）
-指定代理应该使用哪个AI模型
支持VS Code， JetBrains ide， Eclipse和Xcode
—示例：`'Claude Sonnet 4.5'`、`'gpt-4'`、`'gpt-4o'`—根据座席的复杂程度和所需的能力进行选择#### **target**（可选）
—目标环境：`'vscode'`或`'github-copilot'`—若省略，则表示两种环境均存在agent
—当代理具有特定于环境的特性时使用

#### **用户可调用**（可选）
-布尔值，控制座席是否出现在聊天的座席下拉列表中
—默认为`true`（省略）
—设置为`false`，创建只能作为子代理访问或以编程方式访问的代理

#### **disable-model-invocation**（可选）
- Boolean控制代理是否可以作为子代理被其他代理调用
—默认值：`false`（省略）
—设置为`true`，以防止子代理调用，同时保持子代理在选择器中可用

#### **元数据**（可选，仅限GitHub.com）
-具有名称-值对的对象，用于代理注释
—例如：`metadata: { category: 'testing', version: '1.0' }`-VS Code不支持#### **mcp-servers**（可选，仅限Organization/Enterprise）
—配置MCP服务器只对该代理可用
-仅支持organization/enterprise级别的代理
-请参见下面的“MCP服务器配置”一节

#### **切换**（可选，仅限VS Code）
-启用引导的顺序工作流，在代理之间转换并建议下一步
-切换配置列表，每个配置指定一个目标代理和可选提示
-聊天响应完成后，切换按钮出现，允许用户移动到下一个座席
仅支持VS Code（1.106+版本）
-详细信息请参见下面的“切换配置”部分

##切换配置

切换使您能够创建在自定义代理之间无缝转换的引导顺序工作流。这对于编排多步骤开发工作流非常有用，用户可以在移动到下一个步骤之前检查和批准每个步骤。常见的切换模式

- **计划→实施**：在计划代理中生成计划，然后交给实施代理开始编码
- **实现→审查**：完成实施，然后切换到代码审查代理来检查质量和安全问题
- **编写失败测试→编写通过测试**：生成失败测试，然后移交实现使这些测试通过的代码
- **研究→文档**：研究一个主题，然后过渡到文档代理编写指南

切换前体结构

使用`handoffs`字段在代理文件的YAML前端定义切换：```yaml
---
description: 'Brief description of the agent'
name: 'Agent Name'
tools: ['search', 'read']
handoffs:
  - label: Start Implementation
    agent: implementation
    prompt: 'Now implement the plan outlined above.'
    send: false
  - label: Code Review
    agent: code-review
    prompt: 'Please review the implementation for quality and security issues.'
    send: false
---
```
###切换属性

列表中的每个移交必须包括以下属性：

|属性|类型|必选|描述||----------|------|----------|-------------|
|`label`| string |是|聊天界面|切换按钮上显示的显示文本
|`agent`| string |是|要切换到（name或filename，不带`.agent.md`） |的目标代理标识符
|`prompt`| string |否|预填写目标座席聊天输入|的提示文本
|`send`| boolean |否|如果是`true`，则自动将提示提交给目标代理（默认为`false`） |

切换行为- **按钮显示**：切换按钮在聊天响应完成后显示为交互式建议
—**上下文保存**：当用户选择切换按钮时，他们切换到目标座席，并保持会话上下文
—**预填写提示符**：如果指定了`prompt`，则在目标座席的聊天输入中显示预填写
- **手动vs自动**：当`send: false`时，用户必须审核并手动发送预填的提示；当输入`send: true`时，提示符将自动提交

切换配置指南

####何时使用交接

- **多步骤工作流**：跨专门代理分解复杂任务
- **质量门**：确保在实施阶段之间的审查步骤
- **引导过程**：通过结构化的开发过程指导用户
- **技能转换**：从planning/design移动到implementation/testing专家

####最佳实践—**清除标签**：使用以操作为导向的标签，明确指示下一步操作
-✅好：“开始实现”，“安全审查”，“编写测试”
-❌避免使用：“Next”，“Go to agent”，“Do something”

- **相关提示**：提供上下文感知提示，引用已完成的工作
-✅好：`'Now implement the plan outlined above.'`-❌避免：没有上下文的通用提示

- **选择性使用**：不要创建移交给每一个可能的代理；关注逻辑工作流转换
-每个代理限制2-3个最相关的后续步骤
-仅为工作流程中自然遵循的代理添加切换

- **代理依赖关系**：在创建切换之前确保目标代理存在
-切换到不存在的座席会被静默忽略
-测试交接，以验证它们按预期工作- **提示内容**：保持提示简洁、可操作
-参考当前代理的工作，不重复内容
-提供目标代理可能需要的任何必要上下文

示例：完整的工作流

下面是三个代理的例子，通过切换创建一个完整的工作流：

**规划代理** (`planner.agent.md`)：```yaml
---
description: 'Generate an implementation plan for new features or refactoring'
name: 'Planner'
tools: ['search', 'read']
handoffs:
  - label: Implement Plan
    agent: implementer
    prompt: 'Implement the plan outlined above.'
    send: false
---
# Planner Agent
You are a planning specialist. Your task is to:
1. Analyze the requirements
2. Break down the work into logical steps
3. Generate a detailed implementation plan
4. Identify testing requirements

Do not write any code - focus only on planning.
```
**实施代理** (`implementer.agent.md`)：```yaml
---
description: 'Implement code based on a plan or specification'
name: 'Implementer'
tools: ['read', 'edit', 'search', 'execute']
handoffs:
  - label: Review Implementation
    agent: reviewer
    prompt: 'Please review this implementation for code quality, security, and adherence to best practices.'
    send: false
---
# Implementer Agent
You are an implementation specialist. Your task is to:
1. Follow the provided plan or specification
2. Write clean, maintainable code
3. Include appropriate comments and documentation
4. Follow project coding standards

Implement the solution completely and thoroughly.
```
**评审代理** (`reviewer.agent.md`)：```yaml
---
description: 'Review code for quality, security, and best practices'
name: 'Reviewer'
tools: ['read', 'search']
handoffs:
  - label: Back to Planning
    agent: planner
    prompt: 'Review the feedback above and determine if a new plan is needed.'
    send: false
---
# Code Review Agent
You are a code review specialist. Your task is to:
1. Check code quality and maintainability
2. Identify security issues and vulnerabilities
3. Verify adherence to project standards
4. Suggest improvements

Provide constructive feedback on the implementation.
```
这个工作流允许开发人员：
1. 从Planner代理开始创建详细的计划
2. 交给实现者代理根据计划编写代码
3. 移交给Reviewer代理来检查实现
4. 如果发现了重大问题，可以选择切换回计划

版本兼容性

—**VS Code**:VS Code1.106及以后版本支持切换
- **GitHub.com**：目前不支持；代理转换工作流使用不同的机制
- **其他ide **：有限或不支持；关注VS Code实现以获得最大的兼容性

##工具配置

工具规范策略

**启用所有工具**（默认）：```yaml
# Omit tools property entirely, or use:
tools: ['*']
```
**启用特定工具**：```yaml
tools: ['read', 'edit', 'search', 'execute']
```
**启用MCP服务器工具**：```yaml
tools: ['read', 'edit', 'github/*', 'playwright/navigate']
```
**禁用所有工具**：```yaml
tools: []
```
标准工具别名

所有别名不区分大小写：

|别名|备选名称|类别|描述||-------|------------------|----------|-------------|
|`execute`| shell, Bash， powershell | shell执行|在合适的shell执行命令|
|`read`| Read, NotebookRead， view |文件读取|读取文件内容|
|`edit`|编辑，MultiEdit, Write， notebookkedit |文件编辑|编辑和修改文件|
|`search`| Grep、Glob、search | Code search |查找文件或文件中的文本|
|`agent`|自定义代理，任务|代理调用|调用其他自定义代理|
|`web`| WebSearch、WebFetch | Web访问|获取Web内容和搜索|
|`todo`| TodoWrite |任务管理|创建和管理任务列表（仅限VS Code） |

内置MCP服务器工具

**GitHub MCP服务器**：```yaml
tools: ['github/*']  # All GitHub tools
tools: ['github/get_file_contents', 'github/search_repositories']  # Specific tools
```
—所有只读工具默认可用
-令牌范围为源存储库

**剧作家MCP服务器**：```yaml
tools: ['playwright/*']  # All Playwright tools
tools: ['playwright/navigate', 'playwright/screenshot']  # Specific tools
```
—配置为只访问localhost
-用于浏览器自动化和测试

工具选择最佳实践

—**最小权限原则**：只启用座席所需的工具
- **安全**：限制`execute`访问，除非明确要求
- **Focus**：更少的工具=更清晰的代理目的和更好的性能
—**文档**：说明复杂配置需要使用特定工具的原因

子代理调用（代理编排）

代理可以使用代理调用工具** （`agent`工具）调用其他代理，以编排多步骤工作流。推荐的方法是基于提示的编排：
-编排者用自然语言定义一步一步的工作流程。
—每一步都委托给专门的代理。
-编排器只传递基本上下文（例如，基本路径，标识符），并要求每个子代理为tools/constraints.读取自己的`.agent.md`规范

###如何工作

1)通过在编排器的工具列表中包含`agent`来启用代理调用：```yaml
tools: ['read', 'edit', 'search', 'agent']
```
2)对于每一步，通过以下方式调用子代理：
- **代理名称**（标识用户select/invoke）
- **代理规格路径** （`.agent.md`文件读取并遵循）
**最小共享上下文**（例如，`basePath`,`projectName`,`logFile`）

提示模式（推荐）

为每一步使用一致的“包装提示”，以便子代理的行为可预测：```text
This phase must be performed as the agent "<AGENT_NAME>" defined in "<AGENT_SPEC_PATH>".

IMPORTANT:
- Read and apply the entire .agent.md spec (tools, constraints, quality standards).
- Work on "<WORK_UNIT_NAME>" with base path: "<BASE_PATH>".
- Perform the necessary reads/writes under this base path.
- Return a clear summary (actions taken + files produced/modified + issues).
```
可选：如果你需要一个轻量级的，结构化的包装器来跟踪，在提示符中嵌入一个小的JSON块（仍然是人类可读的和工具无关的）：```text
{
  "step": "<STEP_ID>",
  "agent": "<AGENT_NAME>",
  "spec": "<AGENT_SPEC_PATH>",
  "basePath": "<BASE_PATH>"
}
```
编排器结构（保持通用性）

对于可维护的编排器，记录这些结构元素：

- **动态参数**：从用户中提取哪些值（例如，`projectName`,`fileName`,`basePath`）。
- **子代理注册表**：一个list/table，每一步映射到`agentName`+`agentSpecPath`。
- **步骤排序**：显式顺序（步骤1→步骤N）。
- **触发条件**（可选但推荐）：定义一个步骤何时运行或跳过。
- **日志策略**（可选但推荐）：每个步骤后更新一个log/report文件。

避免在编排器提示符中嵌入编排“代码”（JavaScript， Python等）；更喜欢确定性的、工具驱动的协调。

基本模式

构造每个步骤调用：1. **步骤说明**：明确一行目的（用于日志和可追溯性）
2. **代理标识**:`agentName`+`agentSpecPath`3. **上下文**：一个小的，显式的变量集（路径，id，环境名称）
4. **预期输出**：文件到create/update和他们应该写在哪里
5. **返回摘要**：要求子代理返回一个简短的、结构化的摘要

示例：多步骤处理```text
Step 1: Transform raw input data
Agent: data-processor
Spec: .github/agents/data-processor.agent.md
Context: projectName=${projectName}, basePath=${basePath}
Input: ${basePath}/raw/
Output: ${basePath}/processed/
Expected: write ${basePath}/processed/summary.md

Step 2: Analyze processed data (depends on Step 1 output)
Agent: data-analyst
Spec: .github/agents/data-analyst.agent.md
Context: projectName=${projectName}, basePath=${basePath}
Input: ${basePath}/processed/
Output: ${basePath}/analysis/
Expected: write ${basePath}/analysis/report.md
```
###要点

—**在提示符中传递变量**：所有动态值使用`${variableName}`- **保持提示集中**：为每个子代理明确，具体的任务
- **返回摘要**：每个子代理应报告其完成的内容
—**顺序执行**：当outputs/inputs之间存在依赖关系时，按顺序执行步骤
- **错误处理**：在执行相关步骤之前检查结果

###⚠️工具可用性要求

**关键**：如果子代理需要特定的工具（例如，`edit`,`execute`,`search`），编排器必须将这些工具包含在自己的`tools`列表中。子代理无法访问其父协调器不可用的工具。

* * * *例子:```yaml
# If your sub-agents need to edit files, execute commands, or search code
tools: ['read', 'edit', 'search', 'execute', 'agent']
```
编排器的工具权限充当所有被调用子代理的上限。仔细规划工具列表，确保所有子代理都拥有所需的工具。

###⚠️重要限制

**子代理编排不适合大规模数据处理。**在以下情况下避免使用多步骤子代理管道：
-处理数百或数千个文件
-处理大型数据集
-在大型代码库上执行批量转换
-编排超过5-10个连续步骤

每个子代理调用都会增加延迟和上下文开销。对于大容量处理，直接在单个代理中实现逻辑。仅将编排用于协调集中的、可管理的数据集上的专门任务。

代理提示结构

标题下面的降价内容定义了代理的行为、专业知识和指示。结构良好的提示通常包括：1. **代理身份和角色：代理是谁及其主要角色
2. **核心职责**：座席执行哪些具体任务
3. **方法和方法论**：代理如何工作来完成任务
4. **指南和约束**:do/avoid和质量标准
5. **输出期望**：期望的输出格式和质量

提示最佳实践

具体而直接：使用祈使句语气（“分析”、“生成”）；避免使用模糊的术语
- **定义边界**：清楚地说明范围限制和约束
- **包括上下文**：解释领域专业知识和参考相关框架
- **关注行为**：描述代理应该如何思考和工作
- **使用结构化格式**：标题，项目符号和列表使提示可扫描

变量定义和提取代理可以定义动态参数，从用户输入中提取值，并在代理的行为和子代理通信中使用它们。这使灵活的、上下文感知的代理能够适应用户提供的数据。

何时使用变量

**使用变量：
—座席的行为取决于用户的输入
—需要向子代理传递动态值
-希望使代理在不同的上下文中可重用
-需要参数化的工作流
-需要跟踪或引用用户提供的上下文

* * * *例子:
-从用户提示符中提取项目名称
-捕获管道处理的认证名称
—识别文件路径或目录
-提取配置选项
-解析特性名称或模块标识符

变量声明模式

在代理提示符的早期定义变量部分记录期望的参数：```markdown
# Agent Name

## Dynamic Parameters

- **Parameter Name**: Description and usage
- **Another Parameter**: How it's extracted and used

## Your Mission

Process [PARAMETER_NAME] to accomplish [task].
```
变量提取方法

# # # # 1。**显式用户输入**
如果提示中没有检测到变量，则要求用户提供该变量：```markdown
## Your Mission

Process the project by analyzing your codebase.

### Step 1: Identify Project
If no project name is provided, **ASK THE USER** for:
- Project name or identifier
- Base path or directory location
- Configuration type (if applicable)

Use this information to contextualize all subsequent tasks.
```
# # # # 2。**隐含提取提示符**
自动从用户的自然语言输入中提取变量：```javascript
// Example: Extract certification name from user input
const userInput = "Process My Certification";

// Extract key information
const certificationName = extractCertificationName(userInput);
// Result: "My Certification"

const basePath = `certifications/${certificationName}`;
// Result: "certifications/My Certification"
```
# # # # 3。**上下文变量解析**
使用文件上下文或工作空间信息派生变量：```markdown
## Variable Resolution Strategy

1. **From User Prompt**: First, look for explicit mentions in user input
2. **From File Context**: Check current file name or path
3. **From Workspace**: Use workspace folder or active project
4. **From Settings**: Reference configuration files
5. **Ask User**: If all else fails, request missing information
```
###在代理提示中使用变量

####指令中的变量替换

在代理提示符中使用模板变量使其动态：```markdown
# Agent Name

## Dynamic Parameters
- **Project Name**: ${projectName}
- **Base Path**: ${basePath}
- **Output Directory**: ${outputDir}

## Your Mission

Process the **${projectName}** project located at `${basePath}`.

## Process Steps

1. Read input from: `${basePath}/input/`
2. Process files according to project configuration
3. Write results to: `${outputDir}/`
4. Generate summary report

## Quality Standards

- Maintain project-specific coding standards for **${projectName}**
- Follow directory structure: `${basePath}/[structure]`
```
####将变量传递给子代理

在调用子代理时，通过提示符中的替换变量传递所有上下文。首选传递**路径和标识符**，而不是整个文件内容。

示例（提示模板）：```text
This phase must be performed as the agent "documentation-writer" defined in ".github/agents/documentation-writer.agent.md".

IMPORTANT:
- Read and apply the entire .agent.md spec.
- Project: "${projectName}"
- Base path: "projects/${projectName}"
- Input: "projects/${projectName}/src/"
- Output: "projects/${projectName}/docs/"

Task:
1. Read source files under the input path.
2. Generate documentation.
3. Write outputs under the output path.
4. Return a concise summary (files created/updated, key decisions, issues).
```
子代理接收嵌入在提示符中的所有必要上下文。变量在发送提示之前被解析，因此子代理处理具体的路径和值，而不是变量占位符。

现实世界的例子：代码审查编排器

通过多个专门代理验证代码的简单编排器示例：

1)确定共享上下文：
-`repositoryName`,`prNumber`-`basePath`（例如，`projects/${repositoryName}/pr-${prNumber}`）

2)顺序调用专门的代理（每个代理读取自己的`.agent.md`规范）：```text
Step 1: Security Review
Agent: security-reviewer
Spec: .github/agents/security-reviewer.agent.md
Context: repositoryName=${repositoryName}, prNumber=${prNumber}, basePath=projects/${repositoryName}/pr-${prNumber}
Output: projects/${repositoryName}/pr-${prNumber}/security-review.md

Step 2: Test Coverage
Agent: test-coverage
Spec: .github/agents/test-coverage.agent.md
Context: repositoryName=${repositoryName}, prNumber=${prNumber}, basePath=projects/${repositoryName}/pr-${prNumber}
Output: projects/${repositoryName}/pr-${prNumber}/coverage-report.md

Step 3: Aggregate
Agent: review-aggregator
Spec: .github/agents/review-aggregator.agent.md
Context: repositoryName=${repositoryName}, prNumber=${prNumber}, basePath=projects/${repositoryName}/pr-${prNumber}
Output: projects/${repositoryName}/pr-${prNumber}/final-review.md
```
####示例：条件步骤编排（代码审查）

这个例子展示了一个更完整的业务流程，包括飞行前检查、有条件步骤和必需与可选行为。

**动态参数（输入）：**
-`repositoryName`,`prNumber`-`basePath`（例如，`projects/${repositoryName}/pr-${prNumber}`）
-`logFile`（例如，`${basePath}/.review-log.md`）

**飞行前检查（推荐）：**
-验证期望的folders/files是否存在（例如，`${basePath}/changes/`,`${basePath}/reports/`）。
-检测影响步骤触发器的高级特征（例如，repo语言、`package.json`、`pom.xml`、`requirements.txt`、测试文件夹）。
-在开始时记录一次发现。

**步进触发条件：**

|步骤|状态|触发条件|开启故障||------|--------|-------------------|-----------|
| 1：安全审查| **必需** |始终运行|停止管道|
| 2：依赖审计|可选|如果存在依赖清单（`package.json`，`pom.xml`等）|继续|
| 3：测试覆盖检查|可选|如果测试projects/files存在|继续|
| 4：性能检查|可选|如果性能敏感代码改变或存在性能配置|继续|
| **要求** |如果步骤1完成|总是运行，停止管道|**执行流程（自然语言）：**
1. 初始化`basePath`和create/update`logFile`。
2. 进行飞行前检查并记录。
3. 依次执行步骤1→N。
4. 对于每一步：
—如果触发条件为假：标记为**已跳过**并继续。
—否则：使用包装器提示调用子代理并捕获其摘要。
-标记为**SUCCESS**或**FAILED**。
—步骤为“**Required**”且失败：停止管道并写失败总结。
5. 以最后的总结部分结束（总体状态、工件、下一步操作）。

**子代理调用提示符（示例）：**```text
This phase must be performed as the agent "security-reviewer" defined in ".github/agents/security-reviewer.agent.md".

IMPORTANT:
- Read and apply the entire .agent.md spec.
- Work on repository "${repositoryName}" PR "${prNumber}".
- Base path: "${basePath}".

Task:
1. Review the changes under "${basePath}/changes/".
2. Write findings to "${basePath}/reports/security-review.md".
3. Return a short summary with: critical findings, recommended fixes, files created/modified.
```
**日志格式（示例）：**```markdown
## Step 2: Dependency Audit
**Status:** ✅ SUCCESS / ⚠️ SKIPPED / ❌ FAILED
**Trigger:** package.json present
**Started:** 2026-01-16T10:30:15Z
**Completed:** 2026-01-16T10:31:05Z
**Duration:** 00:00:50
**Artifacts:** reports/dependency-audit.md
**Summary:** [brief agent summary]
```
此模式适用于任何编排场景：提取变量，调用具有明确上下文的子代理，等待结果。


可变的最佳实践

# # # # 1。* * * *清晰的文档
总是记录期望的变量：```markdown
## Required Variables
- **projectName**: The name of the project (string, required)
- **basePath**: Root directory for project files (path, required)

## Optional Variables
- **mode**: Processing mode - quick/standard/detailed (enum, default: standard)
- **outputFormat**: Output format - markdown/json/html (enum, default: markdown)

## Derived Variables
- **outputDir**: Automatically set to ${basePath}/output
- **logFile**: Automatically set to ${basePath}/.log.md
```
# # # # 2。* *一致的命名* *
使用一致的变量命名约定：```javascript
// Good: Clear, descriptive naming
const variables = {
  projectName,          // What project to work on
  basePath,            // Where project files are located
  outputDirectory,     // Where to save results
  processingMode,      // How to process (detail level)
  configurationPath    // Where config files are
};

// Avoid: Ambiguous or inconsistent
const bad_variables = {
  name,     // Too generic
  path,     // Unclear which path
  mode,     // Too short
  config    // Too vague
};
```
# # # # 3。**验证和约束
记录有效值和约束：```markdown
## Variable Constraints

**projectName**:
- Type: string (alphanumeric, hyphens, underscores allowed)
- Length: 1-100 characters
- Required: yes
- Pattern: `/^[a-zA-Z0-9_-]+$/`

**processingMode**:
- Type: enum
- Valid values: "quick" (< 5min), "standard" (5-15min), "detailed" (15+ min)
- Default: "standard"
- Required: no
```
MCP服务器配置（Organization/EnterpriseOnly）

MCP服务器使用其他工具扩展代理功能。仅支持组织和企业级代理。

###配置格式```yaml
---
name: my-custom-agent
description: 'Agent with MCP integration'
tools: ['read', 'edit', 'custom-mcp/tool-1']
mcp-servers:
  custom-mcp:
    type: 'local'
    command: 'some-command'
    args: ['--arg1', '--arg2']
    tools: ["*"]
    env:
      ENV_VAR_NAME: ${{ secrets.API_KEY }}
---
```
MCP服务器属性

—**type**：服务器类型（`'local'`或`'stdio'`）
—**command**：启动MCP服务器的命令
—**args**：命令参数数组
**tools**：从该服务器启用的工具（`["*"]`for all）
—**env**：环境变量（支持secret）

环境变量和秘密

秘密必须在“副驾驶”环境下的存储库设置中配置。

* * * *支持语法:```yaml
env:
  # Environment variable only
  VAR_NAME: COPILOT_MCP_ENV_VAR_VALUE

  # Variable with header
  VAR_NAME: $COPILOT_MCP_ENV_VAR_VALUE
  VAR_NAME: ${COPILOT_MCP_ENV_VAR_VALUE}

  # GitHub Actions-style (YAML only)
  VAR_NAME: ${{ secrets.COPILOT_MCP_ENV_VAR_VALUE }}
  VAR_NAME: ${{ var.COPILOT_MCP_ENV_VAR_VALUE }}
```
文件组织和命名

存储库级代理
—位置：`.github/agents/`—范围：仅在指定的存储库中可用
—Access：使用存储库配置的MCP服务器Organization/Enterprise-Level代理
-位置：`.github-private/agents/`（然后移动到`agents/`root）
-范围：适用于org/enterprise中的所有存储库
—Access：可以配置专用的MCP服务器

命名约定
—小写+连字符：`test-specialist.agent.md`-名称应反映代理的目的
-文件名成为默认代理名称（如果未指定`name`）
—允许字符：`.`、`-`、`_`、`a-z`、`A-Z`、`0-9`代理处理和行为

# # #版本控制
—基于Git提交代理文件的sha
—为不同版本的代理创建branches/tags-实例化使用最新版本的repository/branchPR交互使用相同的代理版本以保持一致性名称冲突
优先级（从高到低）：
1. 储存库层次代理
2. 企业级的代理
3. 企业级代理

较低级别的配置将覆盖具有相同名称的较高级别的配置。

工具加工
-`tools`列表过滤器可用的工具（内置和MCP）
—未指定tools =已启用所有tools
—空list (`[]`) =禁用所有工具
—特定列表=仅启用这些工具
-忽略无法识别的工具名称（允许特定于环境的工具）

MCP服务器处理订单
1. 现成的MCP服务器（例如，GitHub MCP）
2. 自定义代理MCP配置（仅限org/enterprise）
3. 存储库级别的MCP配置

每个关卡都可以覆盖之前关卡的设置。

##代理创建检查表# # # Frontmatter
- []`description`字段显示和描述（50-150个字符）
- []`description`用单引号括起来
-[]指定`name`（可选但推荐）
- []`tools`配置适当（或故意省略）
- []`model`为最佳性能设置
- []`target`set if environmental -specific
-[]使用`user-invocable: false`隐藏选择器，同时允许子代理调用
-[]使用`disable-model-invocation: true`来防止子代理调用，同时保持选择器的可见性


提示内容
-[]明确代理身份和角色定义
-[]明确列出的核心职责
-[]解释方法和方法
-[]指定的指导方针和约束条件
-[]输出预期记录
-[]提供有用的示例
-[]指令明确且可执行
-[]明确界定范围和边界
-[]总内容不超过30,000个字符文件结构
-[]文件名遵循小写连字符约定
-[]文件放置在正确的目录（`.github/agents/`或`agents/`）
-[]文件名只允许使用字符
—[]文件扩展名为`.agent.md`质量保证
- [] Agent的用途是唯一的，不可重复的
—[]工具最少，是必须的
-[]说明清晰、明确
[] Agent有代表性的任务测试
-[]文档引用是最新的
-[]解决了安全问题（如适用）

##通用代理模式

测试专家
**目的**：关注测试覆盖率和质量
**工具**：所有工具（用于全面测试创建）
方法：分析，识别差距，编写测试，避免产品代码更改###执行策划人
**目的**：制定详细的技术方案和规格
**工具**：仅限于`['read', 'search', 'edit']`方法：分析需求，创建文档，避免实施

代码审查员
**目的**：审查代码质量并提供反馈
**Tools**：仅支持`['read', 'search']`**方法**：分析，提出改进建议，不直接修改

重构专家
**目的**：改善代码结构和可维护性
* * * *工具:`['read', 'search', 'edit']`方法：分析模式，提出重构建议，安全实现

安全审计员
**目的**：识别安全问题和漏洞
* * * *工具:`['read', 'search', 'web']`方法**：扫描代码，对照OWASP检查，报告发现

要避免的常见错误Frontmatter错误
-❌缺少`description`字段
-❌描述没有用引号括起来
-❌未检查文档的工具名称无效
-❌错误的YAML语法（缩进，引号）

工具配置问题
-❌不必要地给予过多的工具访问
-❌缺少代理所需的工具
-❌不一致使用工具别名
-❌忘记MCP服务器命名空间（`server-name/tool`）

提示内容问题
-❌模糊，模棱两可的指示
-❌相互冲突或矛盾的指导方针
-❌缺乏明确的范围定义
-❌输出预期缺失
-❌过于冗长的指令（超过字符限制）
-❌没有复杂任务的示例或上下文

组织问题
-❌文件名不反映代理的目的
-❌错误的目录（混淆repo和org级别）
-❌文件名中使用空格或特殊字符
-❌代理名称重复导致冲突测试和验证

手动测试
1. 创建具有适当标题的代理文件
2. 重新加载VS Code或刷新GitHub.com
3. 从副驾驶聊天的下拉菜单中选择座席
4. 测试具有代表性的用户查询
5. 验证工具访问是否按预期工作
6. 确认输出符合预期

集成测试
-范围内不同文件类型的测试代理
-验证MCP服务器连接（如果配置）
-检查缺少上下文的座席行为
-测试错误处理和边缘情况
-验证代理切换和切换

质量检查
—执行座席创建检查表
-根据常见错误列表进行检查
—与存储库中的示例代理进行比较
-对复杂的代理进行同行评审
-记录任何特殊的配置需求

##其他资源官方文件
-[创建自定义代理]（https://docs.github.com/en/copilot/how-tos/use-copilot-agents/coding-agent/create-custom-agents）
-[自定义座席配置]（https://docs.github.com/en/copilot/reference/custom-agents-configuration）
[VS Code的海关代理]（https://code.visualstudio.com/docs/copilot/customization/custom-agents）
- [MCP集成]（https://docs.github.com/en/copilot/how-tos/use-copilot-agents/coding-agent/extend-coding-agent-with-mcp）

社区资源
[超级棒的副驾驶特工集]（https://github.com/github/awesome-copilot/tree/main/agents）
—[自定义库示例]（https://docs.github.com/en/copilot/tutorials/customization-library/custom-agents）
-你的第一个自定义代理教程（https://docs.github.com/en/copilot/tutorials/customization-library/custom-agents/your-first-custom-agent）

相关文件
—[提示文件指南]（./prompt.instructions.md）—用于创建提示文件
-[指令指南](./instructions.instructions.md) -用于创建指令文件

版本兼容性说明

GitHub.com（编码代理）
-✅完全支持所有标准的frontmatter属性
-✅存储库和org/enterprise级代理
-✅MCP服务器配置（org/enterprise）
-❌不支持`model`、`argument-hint`、`handoffs`属性###VS Code/ JetBrains / Eclipse / Xcode
-✅支持AI模型选择的`model`属性
-✅支持`argument-hint`和`handoffs`属性
-✅用户配置文件和工作空间级代理
-❌无法在存储库级别配置MCP服务器
-⚠️某些属性可能表现不同

在为多个环境创建代理时，关注公共属性并在所有目标环境中进行测试。必要时使用`target`属性创建特定于环境的代理。