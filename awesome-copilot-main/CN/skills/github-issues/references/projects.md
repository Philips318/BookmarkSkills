# Projects V2

GitHub项目V2是通过GraphQL管理的。MCP服务器提供了三个包装GraphQL API的工具，因此通常不需要原始的GraphQL。

使用MCP工具（优先）

* *项目:列表* *
用`method: "list_projects"`、`owner`和`owner_type`（“用户”或“组织”）调用`mcp__github__projects_list`。

**列出项目字段：**
用`method: "list_project_fields"`和`project_number`调用`mcp__github__projects_list`。

**列出项目项：**
用`method: "list_project_items"`和`project_number`调用`mcp__github__projects_list`。

**添加issue/PR到项目：**
用`method: "add_project_item"`、`project_id`（节点ID）和`content_id`（issue/PR节点ID）调用`mcp__github__projects_write`。

**更新项目项字段值：**
用`method: "update_project_item"`、`project_id`、`item_id`、`field_id`和`value`调用`mcp__github__projects_write`（对象有：`text`、`number`、`date`、`singleSelectOptionId`、`iterationId`）。

**删除项目项：**
用`method: "delete_project_item"`、`project_id`和`item_id`调用`mcp__github__projects_write`。

项目操作的工作流程1. **查找项目** -参见下面的[按名称查找项目]（# Finding -a-project-by-name）
2. **发现字段** -使用`projects_list`和`list_project_fields`来获取字段id和选项id
3. **查找项目** -使用`projects_list`和`list_project_items`来获取项目id
4. **突变** -使用`projects_write`来添加、更新或删除项目

##按名称查找项目

b> **⚠️已知问题：**`projectsV2(query: "…")`进行关键字搜索，而不是精确的名称匹配，并返回按最近排序的结果。像“问题”或“bug”这样的常见单词会返回数百个误报。实际的项目可能被埋在几十页深。

使用以下优先顺序：

# # # 1。直接查询（如果您知道号码）```bash
gh api graphql -f query='{
  organization(login: "ORG") {
    projectV2(number: 42) { id title }
  }
}' --jq '.data.organization.projectV2'
```
# # # 2。从已知问题进行反向查找（最可靠）
如果用户提到项目中的问题，史诗或里程碑，查询该问题的`projectItems`以发现项目：```bash
gh api graphql -f query='{
  repository(owner: "OWNER", name: "REPO") {
    issue(number: 123) {
      projectItems(first: 10) {
        nodes {
          id
          project { number title id }
        }
      }
    }
  }
}' --jq '.data.repository.issue.projectItems.nodes[] | {number: .project.number, title: .project.title, id: .project.id}'
```
对于名称搜索失败的大型组织，这是最可靠的方法。

# # # 3。使用客户端过滤的GraphQL名称搜索（备用）
查询一个大的页面和过滤客户端为一个确切的标题匹配：```bash
gh api graphql -f query='{
  organization(login: "ORG") {
    projectsV2(first: 100, query: "search term") {
      nodes { number title id }
    }
  }
}' --jq '.data.organization.projectsV2.nodes[] | select(.title | test("(?i)^exact name$"))'
```
如果没有返回任何结果，则使用`after`游标进行分页或扩展正则表达式。结果按近期排序，因此旧项目需要分页。

# # # 4。MCP工具（仅限小型组织）
用`method: "list_projects"`调用`mcp__github__projects_list`。这对于项目少于50个但没有名称过滤器的组织很有效，因此您必须扫描所有结果。

进度报告的项目发现

当用户要求更新项目进度时（例如，“给我一个项目X的进度更新”），遵循以下工作流程：

1. **查找项目** -使用上面的[查找项目]（#finding-a-project-by-name）策略。如果名称搜索失败，请用户提供已知的问题号。

2. **发现字段** -使用`list_project_fields`调用`projects_list`来查找Status字段（它的选项告诉您工作流阶段）和任何迭代字段（范围到当前sprint）。3. **获得所有项目** -调用`projects_list`与`list_project_items`。对于大型项目（100+项），对所有页面进行分页。每个项包括它的字段值（状态、迭代、分配）。

4. **构建报告** -按状态字段值分组项目并计数。对于基于迭代的项目，首先筛选到当前迭代。给出如下分类：   ```
   Project: Issue Fields (Iteration 42, Mar 2-8)
   15 actionable items:
     🎉 Done:        4 (27%)
     In Review:      3
     In Progress:    3
     Ready:          2
     Blocked:        2
   ```
5. **添加上下文** -如果项目有子问题，包括`subIssuesSummary`计数。如果项有依赖关系，记录被阻塞的项以及是什么阻塞了它们。

## OAuth范围要求

|操作|所需范围||-----------|---------------|
|读取项目、字段、项|`read:project`|
|Add/update/delete项，更改字段值|`project`|

**常见缺陷：**默认的`gh auth`令牌通常只有`read:project`。`INSUFFICIENT_SCOPES`的突变将失败。添加写作用域。```bash
gh auth refresh -h github.com -s project
```
这会触发一个基于浏览器的OAuth流。你必须在变异起作用之前完成它。

查找Issue的项目项目ID

当您知道问题，但需要其项目项ID（例如，更新其状态）时，从问题方查询：```bash
gh api graphql -f query='
{
  repository(owner: "OWNER", name: "REPO") {
    issue(number: 123) {
      projectItems(first: 5) {
        nodes {
          id
          project { title number }
          fieldValues(first: 10) {
            nodes {
              ... on ProjectV2ItemFieldSingleSelectValue {
                name
                field { ... on ProjectV2SingleSelectField { name } }
              }
            }
          }
        }
      }
    }
  }
}' --jq '.data.repository.issue.projectItems.nodes'
```
这将在一个查询中返回项目ID、项目信息和当前字段值。

##通过gh api使用GraphQL（推荐）

使用`gh api graphql`运行GraphQL查询和修改。对于写操作，这比MCP工具更可靠。

**查找项目及其状态字段选项：**```bash
gh api graphql -f query='
{
  organization(login: "ORG") {
    projectV2(number: 5) {
      id
      title
      field(name: "Status") {
        ... on ProjectV2SingleSelectField {
          id
          options { id name }
        }
      }
    }
  }
}' --jq '.data.organization.projectV2'
```
**列出所有字段（包括迭代）：**```bash
gh api graphql -f query='
{
  node(id: "PROJECT_ID") {
    ... on ProjectV2 {
      fields(first: 20) {
        nodes {
          ... on ProjectV2Field { id name }
          ... on ProjectV2SingleSelectField { id name options { id name } }
          ... on ProjectV2IterationField { id name configuration { iterations { id startDate } } }
        }
      }
    }
  }
}' --jq '.data.node.fields.nodes'
```
**更新字段值（例如，将Status设置为“In Progress”）：**```bash
gh api graphql -f query='
mutation {
  updateProjectV2ItemFieldValue(input: {
    projectId: "PROJECT_ID"
    itemId: "ITEM_ID"
    fieldId: "FIELD_ID"
    value: { singleSelectOptionId: "OPTION_ID" }
  }) {
    projectV2Item { id }
  }
}'
```
取值范围：`text`、`number`、`date`、`singleSelectOptionId`、`iterationId`。

**添加一个项目：**```bash
gh api graphql -f query='
mutation {
  addProjectV2ItemById(input: {
    projectId: "PROJECT_ID"
    contentId: "ISSUE_OR_PR_NODE_ID"
  }) {
    item { id }
  }
}'
```
**删除项：**```bash
gh api graphql -f query='
mutation {
  deleteProjectV2Item(input: {
    projectId: "PROJECT_ID"
    itemId: "ITEM_ID"
  }) {
    deletedItemId
  }
}'
```
端到端示例：将Issue状态设置为“正在进行中”```bash
# 1. Get the issue's project item ID, project ID, and current status
gh api graphql -f query='{
  repository(owner: "github", name: "planning-tracking") {
    issue(number: 2574) {
      projectItems(first: 1) {
        nodes { id project { id title } }
      }
    }
  }
}' --jq '.data.repository.issue.projectItems.nodes[0]'

# 2. Get the Status field ID and "In Progress" option ID
gh api graphql -f query='{
  node(id: "PROJECT_ID") {
    ... on ProjectV2 {
      field(name: "Status") {
        ... on ProjectV2SingleSelectField { id options { id name } }
      }
    }
  }
}' --jq '.data.node.field'

# 3. Update the status
gh api graphql -f query='mutation {
  updateProjectV2ItemFieldValue(input: {
    projectId: "PROJECT_ID"
    itemId: "ITEM_ID"
    fieldId: "FIELD_ID"
    value: { singleSelectOptionId: "IN_PROGRESS_OPTION_ID" }
  }) { projectV2Item { id } }
}'
```
```
