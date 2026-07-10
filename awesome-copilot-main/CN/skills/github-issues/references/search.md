#高级问题搜索`search_issues`MCP工具使用GitHub的问题搜索查询格式进行交叉回购搜索，支持隐式and查询、日期范围和元数据过滤器（但不支持显式OR/NOT操作符）。

##何时使用搜索、列表和高级搜索

有三种方法可以找到问题，每种方法都有不同的功能：

|功能|`list_issues`(MCP) |`search_issues`(MCP) |高级搜索（`gh api`） ||-----------|---------------------|----------------------|---------------------------|
| **范围** |单一回购仅|交叉回购，跨机构|交叉回购，跨机构|
| **发布字段过滤器** (`field.priority:P0`) |否|否| **是**（点表示法）|
| **发布类型过滤器** (`type:Bug`) |否|是|是|
| **布尔逻辑** （AND/OR/NOT，嵌套）|否|是（隐式且仅）| **是**（显式AND/OR/NOT） |
| **Label/state/date过滤器** |是|是|是|
| **Assignee/author/mentions** |否|是|是|
| **否定** (`-label:x`,`no:label`) |否|是|是|
| **文本搜索** (title/body/comments) |否|是|是|
| **`since`filter** |是|否|否|
| **结果限制** |无上限（分页全部）| 1,000 max | 1,000 max |
| **如何直接调用** | MCP工具| MCP工具直接|`gh api`与`advanced_search=true`|* *决策指南:* *
- **单一的repo，简单的过滤器（状态，标签，最近更新）：**使用`list_issues`- **交叉回购，文本搜索，author/assignee，问题类型：**使用`search_issues`- **问题字段值（优先级，日期，自定义字段）或复杂的布尔逻辑：**使用`gh api`与`advanced_search=true`##查询语法`query`参数是一个搜索词和限定符的字符串。项之间的空格是隐含的AND。

# # #范围```
repo:owner/repo       # Single repo (auto-added if you pass owner+repo params)
org:github            # All repos in an org
user:octocat          # All repos owned by user
in:title              # Search only in title
in:body               # Search only in body
in:comments           # Search only in comments
```
###状态和关闭原因```
is:open               # Open issues (auto-added: is:issue)
is:closed             # Closed issues
reason:completed      # Closed as completed
reason:"not planned"  # Closed as not planned
```
# # #人```
author:username       # Created by
assignee:username     # Assigned to
mentions:username     # Mentions user
commenter:username    # Has comment from
involves:username     # Author OR assignee OR mentioned OR commenter
author:@me            # Current authenticated user
team:org/team         # Team mentioned
```
标签、里程碑、项目、类型```
label:"bug"                 # Has label (quote multi-word labels)
label:bug label:priority    # Has BOTH labels (AND)
label:bug,enhancement       # Has EITHER label (OR)
-label:wontfix              # Does NOT have label
milestone:"v2.0"            # In milestone
project:github/57           # In project board
type:"Bug"                  # Issue type
```
缺少元数据```
no:label              # No labels assigned
no:milestone          # No milestone
no:assignee           # Unassigned
no:project            # Not in any project
```
# # #日期

所有日期限定符都支持ISO 8601格式的`>`、`<`、`>=`、`<=`和range （`..`）操作符：```
created:>2026-01-01              # Created after Jan 1
updated:>=2026-03-01             # Updated since Mar 1
closed:2026-01-01..2026-02-01   # Closed in January
created:<2026-01-01              # Created before Jan 1
```
链接内容```
linked:pr             # Issue has a linked PR
-linked:pr            # Issues not yet linked to any PR
linked:issue          # PR is linked to an issue
```
数字过滤器```
comments:>10          # More than 10 comments
comments:0            # No comments
interactions:>100     # Reactions + comments > 100
reactions:>50         # More than 50 reactions
```
布尔逻辑和嵌套

使用`AND`，`OR`和括号（最多5层深度，最多5个操作符）：```
label:bug AND assignee:octocat
assignee:octocat OR assignee:hubot
(type:"Bug" AND label:P1) OR (type:"Feature" AND label:P1)
-author:app/dependabot          # Exclude bot issues
```
没有显式操作符的项之间的空格被视为AND。

##通用查询模式

* *未赋值的错误:* *```
repo:owner/repo type:"Bug" no:assignee is:open
```
**本周结束的事项：**```
repo:owner/repo is:closed closed:>=2026-03-01
```
**陈旧的开放问题（90天内没有更新）：**```
repo:owner/repo is:open updated:<2026-01-01
```
**没有链接PR的开放问题（需要工作）：**```
repo:owner/repo is:open -linked:pr
```
**我在组织中涉及的问题：**```
org:github involves:@me is:open
```
* *个问题:* *```
repo:owner/repo is:open comments:>20
```
**问题按类型和优先级标签：**```
repo:owner/repo type:"Epic" label:P1 is:open
```
##问题字段搜索

可靠性警告：**`field.name:value`搜索限定符语法是实验性的，即使存在匹配问题也可能返回0结果。要按字段值进行可靠的过滤，请使用[issue-fields.md]中记录的GraphQL批量查询方法（issue-fields.md#按字段值搜索）。

问题字段理论上可以使用高级搜索模式通过`field.name:value`限定符进行搜索。这在web UI中是有效的，但是来自API的结果是不一致的。

rest API

添加`advanced_search=true`作为查询参数：```bash
gh api "search/issues?q=org:github+field.priority:P0+type:Epic+is:open&advanced_search=true" \
  --jq '.items[] | "#\(.number): \(.title)"'
```
# # # GraphQL

使用`type: ISSUE_ADVANCED`代替`type: ISSUE`：```graphql
{
  search(query: "org:github field.priority:P0 type:Epic is:open", type: ISSUE_ADVANCED, first: 10) {
    issueCount
    nodes {
      ... on Issue { number title }
    }
  }
}
```
发布字段限定符

语法使用**点表示法**字段的名称（小写，连字符为空格）：```
field.priority:P0                  # Single-select field equals value
field.priority:P1                  # Different option value
field.target-date:>=2026-04-01     # Date comparison
has:field.priority                 # Has any value set
no:field.priority                  # Has no value set
```
**MCP限制：**`search_issues`MCP工具不通过`advanced_search=true`。您必须直接使用`gh api`进行问题字段搜索。

通用字段搜索模式

**跨组织的史诗：**```
org:github field.priority:P0 type:Epic is:open
```
**本季度目标日期的问题：**```
org:github field.target-date:>=2026-04-01 field.target-date:<=2026-06-30 is:open
```
**打开丢失优先级的bug:**```
org:github no:field.priority type:Bug is:open
```
# #的局限性

—查询文本：最大**256个字符**（不包括operators/qualifiers）
—布尔运算符：max **5**AND/OR/NOT每个查询
-结果：最大**1,000**总数（使用`list_issues`，如果你需要所有问题）
—Repo扫描：搜索多达**4,000**匹配的存储库
—速率限制：**30requests/minute**
-问题字段搜索需要`advanced_search=true`（REST）或`ISSUE_ADVANCED`(GraphQL)；不能通过MCP`search_issues`