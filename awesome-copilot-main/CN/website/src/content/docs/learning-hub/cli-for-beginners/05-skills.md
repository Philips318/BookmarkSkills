---
title: '05 · Automate Repetitive Tasks'
description: 'Mirror the source chapter on skills that load automatically for repeated GitHub Copilot CLI workflows.'
authors:
  - GitHub Copilot Learning Hub Team
lastUpdated: 2026-07-03
---
！[第五章：技能系统]（/images/learning-hub/copilot-cli-for-beginners/05/chapter-header.png）

如果Copilot可以自动应用你的团队的最佳实践，而不需要你每次都解释它们，那会怎么样？**

在本章中，你将学习代理技能：当与你的任务相关时，副驾驶会自动加载指令文件夹。虽然智能体改变了副驾驶的思维方式，但技能教会了副驾驶完成任务的具体方式。您将创建一个安全审计技能，当您询问安全性时，Copilot应用该技能，构建团队标准审查标准，确保一致的代码质量，并了解技能如何跨Copilot CLI，VS Code和Copilot编码代理工作。


##🎯学习目标

在本章结束时，你将能够：-了解座席技能如何工作以及何时使用
-创建自定义技能与SKILL.md文件
-使用来自共享存储库的社区技能
-知道何时使用技能、座席和MCP

>⏱️**预计时间**:~55分钟（20分钟阅读+ 35分钟动手）

---

##🧩现实世界的类比：电动工具

一个通用的钻头是有用的，但专门的附件使它更强大。<img src="/images/learning-hub/copilot-cli-for-beginners/05/power-tools-analogy.png" alt="Power Tools - Skills Extend Copilot's Capabilities" width="800"/>
技能也是如此。就像为不同的工作更换钻头一样，你可以为不同的任务添加技能：

|技能附件|用途||------------|---------|
|`commit`|生成一致性提交消息|
|`security-audit`|检查OWASP漏洞|
|`generate-tests`|创建综合pytest测试|
|`code-checklist`|应用团队代码质量标准|



技能是专门的附件，扩展副驾驶可以做什么

---

#技能是如何工作的<img src="/images/learning-hub/copilot-cli-for-beginners/05/how-skills-work.png" alt="Glowing RPG-style skill icons connected by light trails on a starfield background representing Copilot skills" width="800"/>
了解什么是技能，为什么它们很重要，以及它们与代理和MCP有何不同。

---

新技能？*从这里开始！

1. **查看已有的技能：**   ```bash
   copilot
   > /skills list
   ```
这显示了Copilot可以在您的项目和个人文件夹中找到的所有技能。

2. **查看我们提供的[code-checklistSKILL.md]（https://github.com/github/copilot-cli-for-beginners/blob/main/.github/skills/code-checklist/SKILL.md）来查看模式。它只是YAML标题加上降价说明。

3. **理解核心概念：**技能是任务特定的指令，当你的提示匹配技能的描述时，副驾驶会自动加载。你不需要激活他们，只要自然地问。


##理解技能

Agent Skills是包含指令、脚本和资源的文件夹，当与您的任务相关时，Copilot **会自动加载。副驾驶会阅读您的提示，检查是否有任何技能匹配，并自动执行相关指令。```bash
copilot

> Check books.py against our quality checklist
# Copilot detects this matches your "code-checklist" skill
# and automatically applies its Python quality checklist

> Generate tests for the BookCollection class
# Copilot loads your "pytest-gen" skill
# and applies your preferred test structure

> What are the code quality issues in this file?
# Copilot loads your "code-checklist" skill
# and checks against your team's standards
```
>💡**关键洞察**：技能**自动触发**基于您的提示匹配技能的描述。只要自然地问，副驾驶就会在幕后运用相关技能。您也可以直接调用技能，下面将介绍这些技能。

>🧰**随时可用的模板**：查看[.github/skills]（https://github.com/github/copilot-cli-for-beginners/tree/main/.github/skills/）文件夹中的简单复制粘贴技巧，您可以尝试一下。

直接斜杠命令调用

虽然自动触发是技能工作的主要方式，你也可以**直接调用技能**使用他们的名字作为斜杠命令：```bash
> /generate-tests Create tests for the user authentication module

> /code-checklist Check books.py for code quality issues

> /security-audit Check the API endpoints for vulnerabilities
```
当您想要确保使用特定技能时，这为您提供了明确的控制。

####在一条消息中结合多种技能

您可以在单个消息中调用多个技能，并且skill斜杠命令可以出现在提示符中的任何地方-而不仅仅是在开头。当你想一次完成两个不同的检查时，这很方便：```bash
> Check @samples/book-app-project/book_app.py with /code-checklist and also run /generate-tests for it

> Review the auth module /security-audit then /code-checklist the result
```
副驾驶将在相同的回复中应用每个指定的技能，从而节省您发送多个单独的消息。

>💡**提示**：将技能斜杠命令放在句子中最自然的地方。你可以把它们放在邮件的开头、中间或结尾。

>📝**技能与座席调用**：不要混淆技能调用与座席调用：
> - **技能**:`/skill-name <prompt>`，例如`/code-checklist Check this file`**代理**:`/agent`（从列表中选择）或`copilot --agent <name>`（命令行）
>
b>如果技能和代理都有相同的名称（例如，“code-reviewer”），输入`/code-reviewer`将调用**技能**，而不是代理。

我怎么知道一个技能被使用了？

你可以直接问副驾驶：```bash
> What skills did you use for that response?

> What skills do you have available for security reviews?
```
技能vs代理vs MCP

技能只是GitHub Copilot可扩展性模型的一部分。下面是它们与代理和MCP服务器的比较。

不要太担心MCP。我们将在[第06章]（../06-mcp-servers/）中讨论它。它包含在这里，这样你就可以看到技能是如何融入整体画面的<img src="/images/learning-hub/copilot-cli-for-beginners/05/skills-agents-mcp-comparison.png" alt="Comparison diagram showing the differences between Agents, Skills, and MCP Servers and how they combine into your workflow" width="800"/>
|功能|做什么|何时使用||---------|--------------|-------------|
b| **代理** |改变人工智能的思考方式|在许多任务中需要专门的专业知识|
| **Skills** |提供特定于任务的指令|特定的、可重复的任务，并提供详细的步骤|
| **MCP** |连接外部服务|需要api的实时数据|

将代理用于广泛的专业知识，将技能用于特定的任务说明，将MCP用于外部数据。座席可以在会话中使用一种或多种技能。例如，当您要求代理检查您的代码时，它可能会自动应用`security-audit`技能和`code-checklist`技能。

>📚**了解更多**：请参阅官方[关于座席技能]（https://docs.github.com/copilot/concepts/agents/about-agent-skills）文档，以获得有关技能格式和最佳实践的完整参考。

---

##从手动提示到自动鉴定

在深入研究如何创造技能之前，让我们看看它们“为什么”值得学习。一旦你看到一致性的提高，“如何”就更有意义了。之前的技能：不一致的评论

每次代码审查，你可能会忘记一些东西：```bash
copilot

> Review this code for issues
# Generic review - might miss your team's specific concerns
```
或者你每次都写一个很长的提示：```bash
> Review this code checking for bare except clauses, missing type hints,
> mutable default arguments, missing context managers for file I/O,
> functions over 50 lines, print statements in production code...
```
时间：**30+秒**打字。一致性：**因内存而异**。

后技能：自动最佳实践

安装了`code-checklist`技能后，只需自然地问：```bash
copilot

> Check the book collection code for quality issues
```
**幕后发生了什么**：
1. Copilot在你的提示中看到“代码质量”和“问题”
2. 检查技能描述，找到你的`code-checklist`技能匹配
3. 自动加载团队的质量检查表
4. 应用所有的检查，而不需要列出它们<img src="/images/learning-hub/copilot-cli-for-beginners/05/skill-auto-discovery-flow.png" alt="How Skills Auto-Trigger - 4-step flow showing how Copilot automatically matches your prompt to the right skill" width="800"/>
自然地问就好。副驾驶匹配您的提示正确的技能，并自动应用它

* *输出* *:```
## Code Checklist: books.py

### Code Quality
- [PASS] All functions have type hints
- [PASS] No bare except clauses
- [PASS] No mutable default arguments
- [PASS] Context managers used for file I/O
- [PASS] Functions are under 50 lines
- [PASS] Variable and function names follow PEP 8

### Input Validation
- [FAIL] User input is not validated - add_book() accepts any year value
- [FAIL] Edge cases not fully handled - empty strings accepted for title/author
- [PASS] Error messages are clear and helpful

### Testing
- [FAIL] No corresponding pytest tests found

### Summary
3 items need attention before merge
```
**区别**：您的团队的标准是自动应用的，每次都不需要输入它们。

---<details>
<summary>🎬 See it in action!</summary>
！[技能触发演示]（/images/learning-hub/copilot-cli-for-beginners/05/skill-trigger-demo.gif）

*Demo输出不同。您的模型、工具和响应将与此处显示的有所不同</details>
---

大规模一致性：团队PR审查技能

假设你的团队有一个10点的PR清单。如果没有技能，每个开发者都必须记住所有10个要点，而有些人总是会忘记其中的一个。使用`pr-review`技能，整个团队获得一致的评审：```bash
copilot

> Can you review this PR?
```
副驾驶自动加载你的团队的`pr-review`技能，并检查所有10点：```
PR Review: feature/user-auth

## Security ✅
- No hardcoded secrets
- Input validation present
- No bare except clauses

## Code Quality ⚠️
- [WARN] print statement on line 45 - remove before merge
- [WARN] TODO on line 78 missing issue reference
- [WARN] Missing type hints on public functions

## Testing ✅
- New tests added
- Edge cases covered

## Documentation ❌
- [FAIL] Breaking change not documented in CHANGELOG
- [FAIL] API changes need OpenAPI spec update
```
**权力**：每个团队成员自动应用相同的标准。新员工不需要记住清单，因为他们的技能可以处理它。

---

创建自定义技能<img src="/images/learning-hub/copilot-cli-for-beginners/05/creating-managing-skills.png" alt="Human and robotic hands building a wall of glowing LEGO-like blocks representing skill creation and management" width="800"/>
从SKILL.md文件构建自己的技能。

---

##技能位置

技能存储在`.github/skills/`（特定于项目）或`~/.copilot/skills/`（用户级别）中。

###副驾驶如何找到技能

副驾驶会自动扫描这些位置的技能：

|位置|范围||----------|-------|
|`.github/skills/`|项目特定的（通过git与团队共享）|
|`~/.copilot/skills/`|用户特定的（您的个人技能）|

技能结构

每个技能都在自己的文件夹中，并带有`SKILL.md`文件。您可以选择包含脚本，示例或其他资源：```
.github/skills/
└── my-skill/
    ├── SKILL.md           # Required: Skill definition and instructions
    ├── examples/          # Optional: Example files Copilot can reference
    │   └── sample.py
    └── scripts/           # Optional: Scripts the skill can use
        └── validate.sh
```
>💡**提示**：目录名应该匹配SKILL.md标题中的`name`（小写带连字符）。SKILL.md格式

技能使用简单的标记格式与YAML frontmatter：```markdown
---
name: code-checklist
description: Comprehensive code quality checklist with security, performance, and maintainability checks
license: MIT
---

# Code Checklist

When checking code, look for:

## Security
- SQL injection vulnerabilities
- XSS vulnerabilities
- Authentication/authorization issues
- Sensitive data exposure

## Performance
- N+1 query problems (running one query per item instead of one query for all items)
- Unnecessary loops or computations
- Memory leaks
- Blocking operations

## Maintainability
- Function length (flag functions > 50 lines)
- Code duplication
- Missing error handling
- Unclear naming

## Output Format
Provide issues as a numbered list with severity:
- [CRITICAL] - Must fix before merge
- [HIGH] - Should fix before merge
- [MEDIUM] - Should address soon
- [LOW] - Nice to have
```
* * YAML属性:* *

|属性|必选|描述||----------|----------|-------------|
|`name`| **是** |唯一标识符（小写，空格为连字符）|
|`description`| **是** |这个技能做什么，什么时候副驾驶应该使用它|
|`license`|否|适用于该技能的License。|
|`argument-hint`| No |向用户显示描述技能期望的参数的简短提示（例如，`"file path or code snippet"`） |

>💡**什么是`argument-hint`？**当用户直接调用技能时（例如，`/security-audit`），`argument-hint`文本作为占位符出现，显示下一步输入的内容-就像一个迷你帮助提示符。例如，设置`argument-hint: "file path to review"`告诉用户在技能名之后提供一个文件路径。

>📖**官方文档**:[关于代理技能]（https://docs.github.com/copilot/concepts/agents/about-agent-skills）

创造你的第一个技能

让我们构建一个安全审计技能来检查OWASP十大漏洞：```bash
# Create skill directory
mkdir -p .github/skills/security-audit

# Create the SKILL.md file
cat > .github/skills/security-audit/SKILL.md << 'EOF'
---
name: security-audit
description: Security-focused code review checking OWASP (Open Web Application Security Project) Top 10 vulnerabilities
---

# Security Audit

Perform a security audit checking for:

## Injection Vulnerabilities
- SQL injection (string concatenation in queries)
- Command injection (unsanitized shell commands)
- LDAP injection
- XPath injection

## Authentication Issues
- Hardcoded credentials
- Weak password requirements
- Missing rate limiting
- Session management flaws

## Sensitive Data
- Plaintext passwords
- API keys in code
- Logging sensitive information
- Missing encryption

## Access Control
- Missing authorization checks
- Insecure direct object references
- Path traversal vulnerabilities

## Output
For each issue found, provide:
1. File and line number
2. Vulnerability type
3. Severity (CRITICAL/HIGH/MEDIUM/LOW)
4. Recommended fix
EOF

# Test your skill (skills load automatically based on your prompt)
copilot

> @samples/book-app-project/ Check this code for security vulnerabilities
# Copilot detects "security vulnerabilities" matches your skill
# and automatically applies its OWASP checklist
```
**预期输出**（结果会有所不同）：```
Security Audit: book-app-project

[HIGH] Hardcoded file path (book_app.py, line 12)
  File path is hardcoded rather than configurable
  Fix: Use environment variable or config file

[MEDIUM] No input validation (book_app.py, line 34)
  User input passed directly to function without sanitization
  Fix: Add input validation before processing

✅ No SQL injection found
✅ No hardcoded credentials found
```
---

写好技能描述SKILL.md中的`description`字段是至关重要的！这是副驾驶决定是否加载你的技能的方式：```markdown
---
name: security-audit
description: Use for security reviews, vulnerability scanning,
  checking for SQL injection, XSS, authentication issues,
  OWASP Top 10 vulnerabilities, and security best practices
---
```
>💡**提示**：包含与您自然提问方式相匹配的关键字。如果您说“安全审查”，请在描述中包括“安全审查”。

###结合技能与代理

技能和代理一起工作。代理提供专业知识，技能提供具体指令：```bash
# Start with a code-reviewer agent
copilot --agent code-reviewer

> Check the book app for quality issues
# code-reviewer agent's expertise combines
# with your code-checklist skill's checklist
```
---

#管理和分享技能

发现已安装的技能，找到社区技能，并分享您自己的技能。<img src="/images/learning-hub/copilot-cli-for-beginners/05/managing-sharing-skills.png" alt="Managing and Sharing Skills - showing the discover, use, create, and share cycle for CLI skills" width="800" />
---

##管理技能与`copilot skill`命令和`/skills`Copilot CLI提供了两种管理技能的方法。您可以在启动副驾驶之前直接从终端进行操作，也可以在副驾驶会话中进行操作。

选项1:`copilot skill`（终端命令）`copilot skill`子命令允许您直接从终端管理技能，而无需打开交互式Copilot会话。这对于编写脚本、快速检查或在开始工作之前添加技能非常方便。```bash
# See all installed skills
copilot skill list

# Add a skill from a local file, URL, or directory
copilot skill add .github/skills/my-skill/SKILL.md
copilot skill add https://example.com/skills/security-audit/SKILL.md

# Remove a skill by name
copilot skill remove security-audit
```
###选项2:`/skills`（内部副驾驶会话）

一旦你在交互式副驾驶会话，使用`/skills`（或其快捷方式`/skill`）来管理技能无需离开：

|命令|功能||---------|--------------|
|`/skills list`|显示所有已安装的技能|
|`/skills info <name>`|获取特定技能的详细信息|
|`/skills add <name>`|启用一项技能（来自存储库或市场）|
|`/skills remove <name>`|禁用或卸载技能|
编辑SKILL.md文件后重新加载技能|

>💡**`/skill`快捷方式**：您可以键入`/skill`而不是`/skills`-它们是可互换的。例如，`/skill list`的工作原理与`/skills list`相同。

>💡**记住**：您不需要为每个提示“激活”技能。一旦安装，技能是**自动触发**当你的提示符合他们的描述。这些命令用于管理可用的技能，而不是用于使用它们。

示例：查看你的技能```bash
# From the terminal (no interactive session needed):
copilot skill list

Available skills:
- security-audit: Security-focused code review checking OWASP Top 10
- generate-tests: Generate comprehensive unit tests with edge cases
- code-checklist: Team code quality checklist
...

# Or from inside a Copilot session:
copilot

> /skills list

Available skills:
- security-audit: Security-focused code review checking OWASP Top 10
- generate-tests: Generate comprehensive unit tests with edge cases
- code-checklist: Team code quality checklist
...

> /skills info security-audit

Skill: security-audit
Source: Project
Location: .github/skills/security-audit/SKILL.md
Description: Security-focused code review checking OWASP Top 10 vulnerabilities
```

---

<details>
<summary>See it in action!</summary>
！[列表技能演示]（/images/learning-hub/copilot-cli-for-beginners/05/list-skills-demo.gif）

*Demo输出不同。您的模型、工具和响应将与此处显示的有所不同</details>
---

何时使用`/skills reload`在创建或编辑技能的SKILL.md文件后，运行`/skills reload`来获取更改，而无需重新启动Copilot：```bash
# Edit your skill file
# Then in Copilot:
> /skills reload
Skills reloaded successfully.
```
>💡**很高兴知道**：技能仍然有效，即使使用`/compact`来总结你的谈话历史。压实后无需重新装填。

---

发现并使用社区技能

###使用插件安装技能

>💡**什么是插件？**插件是可安装的软件包，可以将技能、代理和MCP服务器配置捆绑在一起。可以把它们看作是Copilot CLI的“应用商店”扩展。`/plugin`命令允许您浏览和安装这些软件包：```bash
copilot

> /plugin list
# Shows installed plugins

> /plugin marketplace
# Browse available plugins

> /plugin install <plugin-name>
# Install a plugin from the marketplace
```
插件可以将多个功能捆绑在一起——一个插件可能包括相关的技能、代理和协同工作的MCP服务器配置。

社区技能库

预制技能也可以从社区知识库中获得：

- **[超级副驾驶](https://github.com/github/awesome-copilot)** -官方GitHub Copilot资源，包括技能文档和示例

手动安装社区技能

如果你在GitHub仓库中找到一个技能，将其文件夹复制到你的技能目录中：```bash
# Clone the awesome-copilot repository
git clone https://github.com/github/awesome-copilot.git /tmp/awesome-copilot

# Copy a specific skill to your project
cp -r /tmp/awesome-copilot/skills/code-checklist .github/skills/

# Or for personal use across all projects
cp -r /tmp/awesome-copilot/skills/code-checklist ~/.copilot/skills/
```
>⚠️**安装前检查**：总是阅读技能的`SKILL.md`之前复制到您的项目。技能控制着副驾驶的行为，恶意技能可能会指示它运行有害命令或以意想不到的方式修改代码。

---

#实践<img src="/images/learning-hub/copilot-cli-for-beginners/05/practice.png" alt="Warm desk setup with monitor showing code, lamp, coffee cup, and headphones ready for hands-on practice" width="800"/>
通过建立和测试自己的技能来应用你所学到的知识。

---

##▶️自己试试

建立更多的技能

下面是另外两种表现出不同模式的技能。按照上面“创建您的第一项技能”中的相同`mkdir`+`cat`工作流程，或复制并粘贴技能到适当的位置。更多的例子可以在[.github/skills]（https://github.com/github/copilot-cli-for-beginners/tree/main/.github/skills）中找到。

pytest测试生成技能

确保整个代码库中pytest结构一致的技能：```bash
mkdir -p .github/skills/pytest-gen

cat > .github/skills/pytest-gen/SKILL.md << 'EOF'
---
name: pytest-gen
description: Generate comprehensive pytest tests with fixtures and edge cases
---

# pytest Test Generation

Generate pytest tests that include:

## Test Structure
- Use pytest conventions (test_ prefix)
- One assertion per test when possible
- Clear test names describing expected behavior
- Use fixtures for setup/teardown

## Coverage
- Happy path scenarios
- Edge cases: None, empty strings, empty lists
- Boundary values
- Error scenarios with pytest.raises()

## Fixtures
- Use @pytest.fixture for reusable test data
- Use tmpdir/tmp_path for file operations
- Mock external dependencies with pytest-mock

## Output
Provide complete, runnable test file with proper imports.
EOF
```
团队PR审查技能

在整个团队中执行一致的PR审查标准的技能：```bash
mkdir -p .github/skills/pr-review

cat > .github/skills/pr-review/SKILL.md << 'EOF'
---
name: pr-review
description: Team-standard PR review checklist
---

# PR Review

Review code changes against team standards:

## Security Checklist
- [ ] No hardcoded secrets or API keys
- [ ] Input validation on all user data
- [ ] No bare except clauses
- [ ] No sensitive data in logs

## Code Quality
- [ ] Functions under 50 lines
- [ ] No print statements in production code
- [ ] Type hints on public functions
- [ ] Context managers for file I/O
- [ ] No TODOs without issue references

## Testing
- [ ] New code has tests
- [ ] Edge cases covered
- [ ] No skipped tests without explanation

## Documentation
- [ ] API changes documented
- [ ] Breaking changes noted
- [ ] README updated if needed

## Output Format
Provide results as:
- ✅ PASS: Items that look good
- ⚠️ WARN: Items that could be improved
- ❌ FAIL: Items that must be fixed before merge
EOF
```
###更进一步

1. **技能创造挑战**：创建一个`quick-review`技能，做一个3点清单：
-条款除外
-缺少类型提示
变量名称不清晰

测试方法是：“快速回顾一下books.py”。

2. **技能比较**：手动编写详细的安全审查提示。然后只需询问“检查此文件中的安全问题”，并让您的安全审计技能自动加载。这个技能节省了多少时间？

3. **团队技能挑战**：想想你团队的代码审查清单。你能把它编码成一种技能吗？写下该技能应该经常检查的3件事。

**自检**：当你能解释为什么`description`字段很重要时，你就理解了技能（这是副驾驶决定是否加载你的技能的方式）。

---

##📝作业

主要挑战：建立一个书籍总结技能上面的示例创建了`pytest-gen`和`pr-review`技能。现在练习创建一种完全不同的技能：用于从数据生成格式化输出的技能。

1. 列出你目前的技能：运行副驾驶并通过`/skills list`。您还可以使用`ls .github/skills/`查看项目技能，或者使用`ls ~/.copilot/skills/`查看个人技能。
2. 在`.github/skills/book-summary/SKILL.md`上创建一个`book-summary`技能，该技能生成图书集合的格式化降价摘要
3. 你的技能应该具备：
-清晰的名称和描述（描述是匹配的关键！）
-特定的格式规则（例如，带有标题，作者，年份，阅读状态的标记表）
-输出约定（例如，使用✅/❌读取状态，按年份排序）
4. 测试技能：`@samples/book-app-project/data.json Summarize the books in this collection`5. 通过检查`/skills list`来验证技能自动触发
6. 尝试用`/book-summary Summarize the books in this collection`直接调用它**成功标准**：你有一个工作的`book-summary`技能，当你询问图书收藏时，Copilot会自动应用。<details>
<summary>💡 Hints (click to expand)</summary>
**启动模板**：创建`.github/skills/book-summary/SKILL.md`：```markdown
---
name: book-summary
description: Generate a formatted markdown summary of a book collection
---

# Book Summary Generator

Generate a summary of the book collection following these rules:

1. Output a markdown table with columns: Title, Author, Year, Status
2. Use ✅ for read books and ❌ for unread books
3. Sort by year (oldest first)
4. Include a total count at the bottom
5. Flag any data issues (missing authors, invalid years)

Example:
| Title | Author | Year | Status |
|-------|--------|------|--------|
| 1984 | George Orwell | 1949 | ✅ |
| Dune | Frank Herbert | 1965 | ❌ |

**Total: 2 books (1 read, 1 unread)**
```
测试:* * * *```bash
copilot
> @samples/book-app-project/data.json Summarize the books in this collection
# The skill should auto-trigger based on the description match
```
**如果不触发：**尝试`/skills reload`然后再问一次。</details>
奖励挑战：提交消息技能

1. 创建一个`commit-message`技能，以一致的格式生成常规提交消息
2. 测试它的方法是进行一个变更，并询问：“为我的阶段性变更生成一个提交消息”。
3. 记录你的技能，并在GitHub上与`copilot-skill`主题分享

---<details>
<summary>🔧 <strong>Common Mistakes & Troubleshooting</strong> (click to expand)</summary>
常见错误

|错误|发生了什么|修复||---------|--------------|-----|
|将文件命名为`SKILL.md`|以外的东西将无法识别技能|文件必须准确命名为`SKILL.md`|
|模糊`description`字段|技能永远不会自动加载|描述是PRIMARY发现机制。使用特定的触发词|
|在frontmatter |中缺少`name`或`description`技能加载|失败添加YAML frontmatter |中的两个字段
|文件夹位置错误|未找到技能|使用`.github/skills/skill-name/`（项目）或`~/.copilot/skills/skill-name/`（个人）|

# # #故障排除

**技能未被使用** -如果副驾驶没有在预期时使用你的技能：

1. **检查描述**：它是否符合你的要求？   ```markdown
   # Bad: Too vague
   description: Reviews code

   # Good: Includes trigger words
   description: Use for code reviews, checking code quality,
     finding bugs, security issues, and best practice violations
   ```
2. **验证文件位置**：   ```bash
   # Project skills
   ls .github/skills/

   # User skills
   ls ~/.copilot/skills/
   ```
3. **检查SKILL.md格式**：需要Frontmatter：   ```markdown
   ---
   name: skill-name
   description: What the skill does and when to use it
   ---

   # Instructions here
   ```
**技能未出现** -验证文件夹结构：```
.github/skills/
└── my-skill/           # Folder name
    └── SKILL.md        # Must be exactly SKILL.md (case-sensitive)
```
在创建或编辑技能之后，运行`/skills reload`，以确保能够拾取更改。

**测试是否有技能加载** -直接询问副驾驶：```bash
> What skills do you have available for checking code quality?
# Copilot will describe relevant skills it found
```
**我怎么知道我的技能是否有效？**

1. **检查输出格式**：如果您的技能指定输出格式（如`[CRITICAL]`标签），请在响应中查找
2. **直接问：在得到回应后，问“你用了什么技能吗？”
3. **比较with/without**：用`--no-custom-instructions`尝试相同的提示来查看差异：   ```bash
   # With skills
   copilot --allow-all -p "Review @file.py for security issues"

   # Without skills (baseline comparison)
   copilot --allow-all -p "Review @file.py for security issues" --no-custom-instructions
   ```
4. **检查特定检查**：如果你的技能包含特定检查（如“函数超过50行”），看看这些是否出现在输出中</details>
---

#总结

##🔑关键要点

1. **技能是自动的：当你的提示匹配技能的描述时，副驾驶会加载它们
2. **直接调用**：您也可以使用`/skill-name`作为斜杠命令直接调用技能
3. **SKILL.md格式**:YAML标题（名称，描述，可选许可，参数提示）加上标记说明
4. **位置问题**:`.github/skills/`供project/team共享使用，`~/.copilot/skills/`供个人使用
5. **描述是关键**：写与你自然提问方式相符的描述
6. **管理技能的两种方式：在终端使用`copilot skill`或在会话中使用`/skills`（快捷方式：`/skill`）

>📋**快速参考**：请参阅[GitHub CopilotCLI命令参考]（https://docs.github.com/en/copilot/reference/cli-command-reference）获得完整的命令和快捷方式列表。

---

##➡️下一步是什么技能扩展了副驾驶可以做的自动加载指令。但是如何连接到外部服务呢？这就是MCP的用武之地。

在**[第06章：MCP服务器](../06-mcp-servers/)**中，你将学到：

-什么是MCP （Model Context Protocol）
-连接到GitHub，文件系统和文档服务
—配置MCP服务器
—多服务器工作流

---