---
name: github-issues
description: 'Create, update, and manage GitHub issues using MCP tools. Use this skill when users want to create bug reports, feature requests, or task issues, update existing issues, add labels/assignees/milestones, set issue fields (dates, priority, custom fields), set issue types, manage issue workflows, link issues, add dependencies, or track blocked-by/blocking relationships. Triggers on requests like "create an issue", "file a bug", "request a feature", "update issue X", "set the priority", "set the start date", "link issues", "add dependency", "blocked by", "blocking", or any GitHub issue management task.'
---
# GitHub问题

使用`@modelcontextprotocol/server-github`MCP服务器管理GitHub问题。

##可用工具

MCP工具（读取操作）

|工具|用途||------|---------|
|`mcp__github__issue_read`|读取问题细节，子问题，评论，标签（方法：get， get_comments, get_sub_issues, get_labels） |
|`mcp__github__list_issues`|按状态、标签、日期列出和过滤存储库问题|
|`mcp__github__search_issues`|使用GitHub搜索语法搜索跨repos问题|
|`mcp__github__projects_list`|列出项目，项目字段，项目项，状态更新|
|`mcp__github__projects_get`|获取项目、字段、项或状态更新|的详细信息
|`mcp__github__projects_write`|Add/update/delete项目项，创建状态更新|

### CLI / REST API（写操作）

MCP服务器目前不支持创建、更新或评论问题。使用`gh api`进行这些操作。

|操作|命令||-----------|---------|
|创建issue |`gh api repos/{owner}/{repo}/issues -X POST -f title=... -f body=...`|
|更新问题|`gh api repos/{owner}/{repo}/issues/{number} -X PATCH -f title=... -f state=...`|
|添加评论|`gh api repos/{owner}/{repo}/issues/{number}/comments -X POST -f body=...`|
|关闭|`gh api repos/{owner}/{repo}/issues/{number} -X PATCH -f state=closed`|
|设置问题类型|在create调用中包含`-f type=Bug`（仅限REST API，`gh issue create`CLI不支持）|

**注意：**`gh issue create`用于基本问题创建，但**不**支持`--type`标志。当需要设置问题类型时，使用`gh api`。

# #工作流程

1. **确定操作**：创建、更新还是查询？
2. **收集上下文**：获取回购信息，现有标签，里程碑（如果需要）
3. **结构内容**：使用合适的模板从[references/templates.md]（references/templates.md）
4. **Execute**：读用MCP工具，写用`gh api`工具
5. **确认**：向用户报告问题URL

##制造问题

使用`gh api`创建问题。这支持包括问题类型在内的所有参数。```bash
gh api repos/{owner}/{repo}/issues \
  -X POST \
  -f title="Issue title" \
  -f body="Issue body in markdown" \
  -f type="Bug" \
  --jq '{number, html_url}'
```
###可选参数

将这些标志添加到`gh api`调用中：```
-f type="Bug"                    # Issue type (Bug, Feature, Task, Epic, etc.)
-f labels[]="bug"                # Labels (repeat for multiple)
-f assignees[]="username"        # Assignees (repeat for multiple)
-f milestone=1                   # Milestone number
```
问题类型**是组织级元数据。要发现可用的类型，使用：```bash
gh api graphql -f query='{ organization(login: "ORG") { issueTypes(first: 10) { nodes { name } } } }' --jq '.data.organization.issueTypes.nodes[].name'
```
**选择问题类型而不是标签进行分类。**当问题类型可用时（例如，Bug, Feature, Task），使用`type`参数而不是应用等价的标签，如`bug`或`enhancement`。问题类型是GitHub上对问题进行分类的规范方法。只有当组织没有配置问题类型时，才返回到标签。

标题指南

-具体和可操作
—长度不超过72个字符
-当问题类型设置时，不要添加多余的前缀，如`[Bug]`例子:
-`Login fails with SSO enabled`（与类型=Bug）
-`Add dark mode support`（with type=Feature）
-`Add unit tests for auth module`（with type=Task）

###身体结构

始终使用[references/templates.md]（references/templates.md）中的模板。根据问题类型选择：

|用户请求|模板||--------------|----------|
|错误报告|
|功能，增强，增加，新的|功能请求|
|任务、杂务、重构、更新|任务|

##更新问题

使用`gh api`和PATCH：```bash
gh api repos/{owner}/{repo}/issues/{number} \
  -X PATCH \
  -f state=closed \
  -f title="Updated title" \
  --jq '{number, html_url}'
```
只包含您想要更改的字段。可用字段：`title`、`body`、`state`（open/closed）、`labels`、`assignees`、`milestone`。

# #的例子

例1:Bug报告

**用户**：“创建一个bug问题-使用SSO时登录页面崩溃”

* *行动* *:```bash
gh api repos/github/awesome-copilot/issues \
  -X POST \
  -f title="Login page crashes when using SSO" \
  -f type="Bug" \
  -f body="## Description
The login page crashes when users attempt to authenticate using SSO.

## Steps to Reproduce
1. Navigate to login page
2. Click 'Sign in with SSO'
3. Page crashes

## Expected Behavior
SSO authentication should complete and redirect to dashboard.

## Actual Behavior
Page becomes unresponsive and displays error." \
  --jq '{number, html_url}'
```
示例2：功能请求

**用户**：“为高优先级的暗模式创建一个功能请求”

* *行动* *:```bash
gh api repos/github/awesome-copilot/issues \
  -X POST \
  -f title="Add dark mode support" \
  -f type="Feature" \
  -f labels[]="high-priority" \
  -f body="## Summary
Add dark mode theme option for improved user experience and accessibility.

## Motivation
- Reduces eye strain in low-light environments
- Increasingly expected by users

## Proposed Solution
Implement theme toggle with system preference detection.

## Acceptance Criteria
- [ ] Toggle switch in settings
- [ ] Persists user preference
- [ ] Respects system preference by default" \
  --jq '{number, html_url}'
```
##常用标签

适用时使用以下标准标签：

|标签|用于||-------|---------|
|`bug`|有些东西不工作|
|`enhancement`| |新增特性或改进
|`documentation`|文档更新|
|`good first issue`|新手好|
需要额外的注意
|`question`| |请求更多信息
|`wontfix`|不会被寻址|
|`duplicate`|已经存在|
|`high-priority`|紧急事项|

# #提示

在创建问题之前，始终确认存储库上下文
-询问遗漏的关键信息，而不是猜测
-链接相关问题时，已知：`Related to #123`-对于更新，首先获取当前问题以保留未更改的字段

##扩展功能

以下功能需要基本MCP工具之外的REST或GraphQL api。每个都在自己的参考文件中记录，因此代理只加载它需要的知识。

| Capability |何时使用|参考||------------|-------------|-----------|
| [references/search.md](references/search.md) |高级搜索|复杂查询布尔逻辑，日期范围，交叉回购搜索，问题字段过滤器(`field.name:value`| [references/sub-issues.md](references/sub-issues.md) |
|跟踪被阻塞/阻塞关系| [references/dependencies.md](references/dependencies.md) |
|问题类型（高级）|超越MCP的GraphQL操作`list_issue_types`/`type`param | [references/issue-types.md](references/issue-types.md) |
|项目板、进度报告、现场管理| [references/projects.md](references/projects.md) |
|自定义元数据：日期，优先级，文本，数字（私人预览）| [references/issue-fields.md](references/issue-fields.md) |
| [references/images.md](references/images.md) |