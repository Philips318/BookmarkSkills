---
title: 'Using the Copilot Coding Agent'
description: 'Learn how to use GitHub Copilot coding agent to autonomously work on issues, generate pull requests, and automate development tasks.'
authors:
  - GitHub Copilot Learning Hub Team
lastUpdated: 2026-05-13
estimatedReadingTime: '12 minutes'
tags:
  - coding-agent
  - automation
  - agentic
relatedArticles:
  - ./building-custom-agents.md
  - ./automating-with-hooks.md
  - ./creating-effective-skills.md
prerequisites:
  - Understanding of GitHub Copilot agents
  - Repository with GitHub Copilot enabled
---
Copilot编码代理是一个自主代理，可以在没有持续人工指导的情况下处理GitHub问题。您给它分配一个问题，它启动云环境、编写代码、运行测试并打开拉取请求——所有这些都是在您专注于其他工作的同时进行的。把它想象成一个从不睡觉的初级开发人员，处理明确定义的任务，并总是要求审查。

本文解释了编码代理如何工作、如何设置它，以及充分利用自主编码会话的最佳实践。

##如何工作

编码代理遵循一个简单的工作流程：```
1. You assign an issue to Copilot (or @mention it)
         ↓
2. Copilot spins up a cloud dev environment
         ↓
3. It reads the issue, your instructions, and codebase
         ↓
4. It plans and implements a solution
         ↓
5. It runs tests and validates the changes
         ↓
6. It opens a pull request for your review
```
代理在独立的环境中，在自己的分支中工作。它不能合并代码或部署——它总是生成一个必须由人类审查和批准的PR。

* * * *关键特征:
-在安全的沙箱云环境中运行
-根据上下文使用存储库的说明、代理和技能
-自动执行挂钩（检查，格式化）
-创建一个PR，总结它所做的事情和原因
-支持迭代-你可以对PR和代理进行评论

##设置环境

编码代理需要知道如何设置项目。在`.github/copilot-setup-steps.yml`中定义：```yaml
# .github/copilot-setup-steps.yml
steps:
  - name: Install dependencies
    run: npm ci

  - name: Build the project
    run: npm run build

  - name: Verify tests pass
    run: npm test
```
###要包含什么

可以将此文件视为新开发人员加入项目的引导说明：

**语言运行时：如果您的项目需要特定的Node.js， Python或Go版本，请在此处安装。

**依赖项**：安装所有项目依赖项（`npm ci`,`pip install -r requirements.txt`,`bundle install`）。

**构建步骤**：如果需要，编译项目，以便代理可以成功验证其更改构建。

**测试命令**：运行测试套件，以便代理可以验证其更改不会破坏现有功能。

** Python项目示例**：```yaml
steps:
  - name: Set up Python
    uses: actions/setup-python@v5
    with:
      python-version: '3.12'

  - name: Install dependencies
    run: pip install -r requirements.txt

  - name: Run tests
    run: pytest
```
**多语言项目示例**：```yaml
steps:
  - name: Install Node.js dependencies
    run: npm ci

  - name: Install Python dependencies
    run: pip install -r requirements.txt

  - name: Build frontend
    run: npm run build

  - name: Run all tests
    run: npm test && pytest
```
将工作分配给编码代理

有几种方法可以触发编码代理：

###来自GitHub问题

1. 创建一个描述良好的问题，并带有明确的接受标准
2. 将问题分配给**副驾驶**（显示为受让人选项）
3. 座席在几分钟内开始工作

###来自评论

就任何问题发表评论：```
@copilot work on this
```
或者提供更具体的指导：```
@copilot implement the user avatar upload feature described above.
Use the existing FileUpload component and S3 service.
```
###使用自定义代理

自定义代理允许您为编码代理提供专门的角色、工具集和用于特定工作类型的指令。您可以将编码代理指向为您的任务量身定制的代理配置文件，而不是依赖于通用行为。

**自定义代理所在的位置**：代理配置文件存储为存储库中`.github/agents/`中的`.agent.md`文件。对于组织范围的代理，将它们放在根`agents/`目录中。```
.github/
└── agents/
    ├── api-architect.agent.md
    ├── test-specialist.agent.md
    └── security-reviewer.agent.md
```
**在GitHub.com上选择代理**：当提示编码代理或将其分配给一个问题时，使用代理面板中的下拉菜单选择您的自定义代理而不是默认值。

**通过评论选择代理**：在任何问题上，提到代理的名字：```
@copilot use the api-architect agent to implement this API endpoint
```
代理将采用该代理文件中定义的角色、工具和护栏。

**代理配置文件中的内容**:`.agent.md`文件是一个Markdown文件，其中包含YAML标题，定义代理的名称、描述、可用工具和可选的MCP服务器配置。Markdown主体包含代理的行为指令（最多30,000个字符）。```markdown
---
name: test-specialist
description: Focuses on test coverage, quality, and testing best practices
tools: ["read", "edit", "search", "bash"]
---

You are a testing specialist. Analyze existing tests, identify coverage gaps,
and write comprehensive unit and integration tests. Follow best practices for
the language and framework. Never modify production code unless asked.
```
提示**：浏览此站点上的[Agents Directory](../../agents/)，以获取可以添加到存储库的现成代理配置文件。

为编码代理编写有效的问题

编码代理的好坏取决于它接收到的问题。结构良好的问题会带来更好的结果。

良好的问题结构```markdown
## Summary
Add a rate limiter to the /api/login endpoint to prevent brute force attacks.

## Requirements
- Limit to 5 attempts per IP address per 15-minute window
- Return HTTP 429 with a Retry-After header when limit is exceeded
- Use the existing Redis cache for rate tracking
- Log rate limit violations to our security audit log

## Acceptance Criteria
- [ ] Rate limiter middleware is applied to POST /api/login
- [ ] Tests cover: normal login, rate limit hit, rate limit reset
- [ ] Existing login tests continue to pass

## Context
- Rate limiter utility exists at src/middleware/rate-limiter.ts
- Redis client is configured in src/config/redis.ts
- Security audit logger is at src/utils/security-logger.ts
```
获得更好效果的技巧

- **具体**：“添加输入验证”是模糊的。“在注册端点上验证电子邮件格式和密码长度（8+字符）”是可操作的。
- **指向现有代码**：代理应该使用的参考文件、实用程序和模式。
- **定义完成：列出验证工作完成的验收标准或测试用例。
- **适当的范围**：单一功能的问题工作最好。将大功能分解成小问题。
- **包括约束**：如果有代理不应该做的事情（“不要修改数据库模式”），明确地说出来。

处理拉取请求

当编码代理完成后，它打开一个PR，包含：

-变化的描述和背后的原因
-每个文件的更改摘要
—参考原始版本

###审查PR

像审核其他代码代理pr一样审核代码代理pr：1. **阅读摘要**：理解代理做了什么以及为什么
2. **检查差异**：验证实现是否符合您的期望
3. **在本地运行测试**：确认测试在您的环境中通过
4. **留下评论**：如果需要更改某些内容，请对PR进行评论

使用注释进行迭代

如果PR需要调整，直接评论：```
@copilot the rate limiter should use a sliding window, not a fixed window.
Also, add a test for the Retry-After header value.
```
代理将阅读您的反馈，进行更改，并将新的提交推送到相同的PR。

代理技能和编码代理

代理技能是包含指令、脚本和资源的文件夹，编码代理可以在与任务相关时自动加载它们。自定义代理定义谁做这项工作，而技能定义如何做特定类型的工作。

技能如何与编码代理一起工作

当编码代理处理任务时，它读取每个技能的`SKILL.md`中的`description`字段，并决定该技能是否相关。如果是这样，技能的指令就会被注入到代理的上下文中——让它能够访问专门的指导、脚本和示例，而不需要指定任何内容。

这意味着您可以将技能添加到存储库中，编码代理将在适当的时候自动利用它们。

###技能存在的地方技能存储在`skills/`子目录中，每个技能都在自己的文件夹中：

**项目技能**（特定于一个存储库）：```
.github/
└── skills/
    ├── github-actions-debugging/
    │   └── SKILL.md
    ├── database-migrations/
    │   ├── SKILL.md
    │   └── scripts/
    │       └── migrate.sh
    └── api-testing/
        ├── SKILL.md
        └── references/
            └── test-template.ts
```
**个人技能**（在所有项目中共享）：```
~/.agents/
└── skills/
    └── code-review-checklist/
        └── SKILL.md
```
是什么让技能变得强大

与简单的指令不同，技能可以捆绑额外的资源：

-代理可以执行的脚本（例如，迁移脚本，代码生成器）
—**模板**和座席可参考的样例
- **数据文件**和专业领域的参考资料
- **补充Markdown**文件，并提供详细指导`SKILL.md`文件告诉代理何时以及如何使用这些资源：```markdown
---
name: database-migrations
description: 'Guide for creating safe database migrations. Use when asked to modify database schema or create migrations.'
---

When creating database migrations, follow this process:

1. Run `./scripts/check-schema.sh` to validate current state
2. Create a new migration file following the naming convention: `YYYYMMDD_description.sql`
3. Always include a rollback section
4. Test the migration against a local database before committing
```
技能vs指令vs代理

|功能|说明|技能|自定义座席||---------|-------------|--------|---------------|
|加载时|总是（匹配文件模式）|相关时自动|明确选择|时
|最适合|编码标准、风格指南|专业任务指南|基于角色的人物角色|
|可以包含脚本|否|是|否（但可以引用技能）|
|作用域|基于文件模式的|基于任务的|会话范围|

提示**：浏览[技能目录](../../skills/)，查找可以添加到存储库的现成技能。每个技能都包括一个`SKILL.md`和任何所需的捆绑资产。

##利用社区资源

此存储库提供了为编码代理设计的代理、技能和钩子的精心集合。下面是如何使用它们：

###添加代理从这个回购1. 浏览[Agents Directory](../../agents/)，查找符合您需求的代理
2. 将`.agent.md`文件复制到存储库的`.github/agents/`目录中
3. 当将工作分配给编码代理时，代理将在下拉菜单中可用

###从这个回购添加技能

1. 浏览[技能目录]（../../skills/）查找专业技能
2. 将整个技能文件夹复制到存储库的`.github/skills/`目录中
3. 编码代理将在与任务相关时自动使用该技能

###从这个Repo添加钩子

1. 浏览[Hooks目录]（../../hooks/）查找自动化钩子
2. 将`hooks.json`内容复制到存储库中`.github/hooks/`中的一个文件中
3. 复制它旁边的任何引用脚本
4. 钩子将在编码代理会话期间自动运行b> **示例工作流**：将`test-specialist`代理与`database-migrations`技能和linting hook结合起来。使用测试专家代理将问题分配给编码代理——它将在相关时自动获取迁移技能，并且钩子确保在完成之前格式化所有代码。

##遥控

您可以使用**远程控制**从本地Copilot CLI终端连接并引导正在运行的编码代理会话。这使您可以观察代理的进度，发送后续提示，并实时重定向其工作，而无需等待它首先打开PR。

启动远程控制会话

启动一个会话，在GitHub上注册远程访问：```bash
copilot --remote
```
或者在现有会话中打开远程控制选项卡，并检查或切换其状态：```
/remote             # show current remote control status
/remote on          # enable remote control and register with GitHub
/remote off         # disable remote control for this session
```
CLI中的**Remote**选项卡显示来自存储库的所有活动编码代理任务。选择要连接的任务并开始发送转向消息。

从会话选择器中恢复

远程会话也出现在`--resume`选择器中，因此您可以重新连接到以前控制的编码代理会话，而无需知道会话ID：```bash
copilot --resume
```
从v1.0.47开始，`--resume`还显示了尚未将任何更改推送到其分支的云代理会话——这对于在会话运行早期（在它提交任何内容之前）连接到会话非常有用。

为什么要使用遥控器？

|场景|受益||----------|---------|
|长时间运行的任务|监视进度，而不等待最终的PR |
中途修正|重定向代理，如果它朝着错误的方向|
|交互改进|在代理工作时提供澄清和反馈|
|不需要PR |您可以引导尚未打开拉取请求的任务|

> **注**：远程控制取代早期的“转向”功能。如果您在旧文档中看到对转向的引用，则远程控制是更新后的等效内容。

钩子和编码代理

hook对于编码代理来说特别有价值，因为它们为自主工作提供了确定性的护栏：—**`preToolUse`**：批准或拒绝工具执行-阻止危险命令并强制执行安全策略
- **`postToolUse`**：格式化代码，运行编译器，并在编辑后验证更改
- **`agentStop`**：在代理完成响应时运行最终检查（例如，完整的lint通过）
- **`sessionStart`**：记录自治会话的开始，以便进行治理
—**`sessionEnd`**：座席完成后发送通知

参见[使用Hooks自动化]（../automating-with-hooks/）了解配置细节。

最佳实践

为成功做好准备**投资`copilot-setup-steps.yml`**：一个可靠的设置意味着代理可以自信地构建和测试。如果测试不可靠，代理将会挣扎。
- **添加全面的说明**：代理读取您的`.github/instructions/`文件。提供的关于模式和约定的上下文越多，输出就越好。
- **为可重复的任务创建技能**：如果你的团队经常做特定类型的工作（迁移，API端点，测试套件），创建一个逐步指导的技能，代理可以自动遵循。
- **为专门的角色使用自定义代理**：为不同类型的工作（安全审查人员、测试专家或基础设施专家）创建集中的代理配置文件。
- **定义格式化钩子：钩子确保代理的代码自动满足您的风格要求，减少审查摩擦。

选择正确的任务编码代理擅长：
-✅定义良好的功能实现，具有明确的接受标准
-✅Bug修复与可复制的步骤
-✅向现有代码添加测试
-✅有特定目标的重构（提取函数，重命名等）
-✅基于代码更改的文档更新

它不太适合：
-❌需要团队讨论的模糊设计决策
-❌跨越许多文件的大型架构更改
-❌需要访问非开发环境外部系统的任务
-❌没有明确指标的性能优化

安全考虑

-编码代理工作在一个孤立的环境-它不能访问您的本地机器
它只能修改分支中的代码，不能推送到main或deploy
-所有的变更在合并前都要经过PR审查
-使用钩子在每次提交时强制执行安全扫描
-适当地限定存储库权限##常见问题

**Q：编码代理需要多长时间？**

答：通常是5-30分钟，这取决于任务的复杂程度和代码库的大小。公关准备好了，你会收到通知的。

**问：我可以将编码代理与私有存储库一起使用吗？**

是的。编码代理可以使用启用了GitHub Copilot的公共和私有存储库。

**Q：如果代理卡住了怎么办？**

答：座席自带超时。如果它不能取得进展，它将打开一个公关，并解释它无法解决的问题。然后，您可以在指导下进行评论或手动接管。

**Q：我可以一次分配多个问题吗？**

是的。编码代理可以并行处理多个问题，每个问题都在自己的分支中。使用GitHub.com上的任务控制来跟踪所有活跃的代理会话。

**Q：编码代理是否使用我的自定义代理和技能？**是的。您可以指定在分配工作时使用哪个代理—编码代理采用该代理的角色、工具和护栏。当代理根据技能描述确定技能与任务相关时，会自动加载技能。

##下一步- **设置您的环境**：为您的项目创建`.github/copilot-setup-steps.yml`- **创造技能**:[创造有效技能](../creating-effective-skills/) -建立技能编码代理可以自动使用
- **添加护栏**:[自动化与挂钩](../automating-with-hooks/) -确保代码质量在自治会话
- **构建自定义代理**:[构建自定义代理](../building-custom-agents/) -为编码代理创建专门的代理
- **探索配置**:[副驾驶配置基础](../copilot-configuration-basics/) -设置存储库级别的自定义
- **浏览社区资源**：探索[Agents](../../agents/), [Skills]（../../skills/）和[Hooks]（../../hooks/）目录，以获取可供使用的资源

---