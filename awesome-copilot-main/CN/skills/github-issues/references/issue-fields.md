# Issue Fields

问题字段是在组织级别定义的自定义元数据（日期、文本、数字、单选），并为每个问题设置。它们与标签、里程碑和受让人是分开的。常见的例子：开始日期、目标日期、优先级、影响、努力。

**优先选择问题字段而不是项目字段。**当您需要设置元数据，如日期，优先级，或一个问题的状态，使用问题字段（它存在于问题本身），而不是项目字段（它存在于一个项目项）。问题字段与问题一起跨项目和视图传播，而项目字段则限定在单个项目范围内。只有当问题字段不可用或者字段是特定于项目的（例如，sprint迭代）时才使用项目字段。

## REST API（推荐）

REST API是发现字段和设置值的最简单方法。

发现可用字段```bash
gh api orgs/{org}/issue-fields --jq '.[] | {id, name, options: [.options[]? | {id, name}]}'
```
读取问题上的字段值```bash
gh api repos/{owner}/{repo}/issues/{number}/issue-field-values
```
设置字段值```bash
gh api repos/{owner}/{repo}/issues/{number}/issue-field-values \
  -X POST \
  --input - <<'EOF'
{"issue_field_values": [{"field_id": 1, "value": "P1"}]}
EOF
```
**重要：**负载必须是带有`issue_field_values`数组的JSON对象。每个条目有：
—`field_id`(integer)：从org字段列表中获取的字段的数字ID
-`value`（字符串）：单个选择字段的**选项名称**（例如，`"P1"`,`"High"`），或text/number/date字段的文字值

要避免的常见错误：
-传递选项ID而不是选项名称，如`value`（API期望显示名称）
-发送`field_id`和`value`作为顶级键，而不封装在`issue_field_values`数组中
-使用`-f`标志，而不是`--input`JSON体

###示例：设置优先级为P1```bash
# 1. Find the Priority field ID and option names
gh api orgs/{org}/issue-fields --jq '.[] | select(.name == "Priority")'

# 2. Set it (use the option NAME, not ID)
gh api repos/{owner}/{repo}/issues/{number}/issue-field-values \
  -X POST \
  --input - <<'EOF'
{"issue_field_values": [{"field_id": 1, "value": "P1"}]}
EOF
```
###示例：一次设置多个字段```bash
gh api repos/{owner}/{repo}/issues/{number}/issue-field-values \
  -X POST \
  --input - <<'EOF'
{"issue_field_values": [
  {"field_id": 1, "value": "P1"},
  {"field_id": 5, "value": "2026-06-01"},
  {"field_id": 7, "value": "High"}
]}
EOF
```
设置字段的工作流（REST）

1. **发现字段** -`gh api orgs/{org}/issue-fields`获取字段id和选项名称
2. **设置值** - POST到JSON主体的`repos/{owner}/{repo}/issues/{number}/issue-field-values`3. **批处理时尽可能** -多个字段可以设置在一个单一的请求

## GraphQL API（可选）

GraphQL API需要`GraphQL-Features: issue_fields`HTTP头。没有它，字段在模式中是不可见的。

发现可用字段（GraphQL）```graphql
# Header: GraphQL-Features: issue_fields
{
  organization(login: "OWNER") {
    issueFields(first: 30) {
      nodes {
        __typename
        ... on IssueFieldDate { id name }
        ... on IssueFieldText { id name }
        ... on IssueFieldNumber { id name }
        ... on IssueFieldSingleSelect { id name options { id name color } }
      }
    }
  }
}
```
字段类型：`IssueFieldDate`、`IssueFieldText`、`IssueFieldNumber`、`IssueFieldSingleSelect`。

读取字段值（GraphQL）```graphql
# Header: GraphQL-Features: issue_fields
{
  repository(owner: "OWNER", name: "REPO") {
    issue(number: 123) {
      issueFieldValues(first: 20) {
        nodes {
          __typename
          ... on IssueFieldDateValue {
            value
            field { ... on IssueFieldDate { id name } }
          }
          ... on IssueFieldTextValue {
            value
            field { ... on IssueFieldText { id name } }
          }
          ... on IssueFieldNumberValue {
            value
            field { ... on IssueFieldNumber { id name } }
          }
          ... on IssueFieldSingleSelectValue {
            name
            color
            field { ... on IssueFieldSingleSelect { id name } }
          }
        }
      }
    }
  }
}
```
设置字段值（GraphQL）

使用`setIssueFieldValue`一次设置一个或多个字段。您需要问题的节点ID和上面发现查询中的字段ID。```graphql
# Header: GraphQL-Features: issue_fields
mutation {
  setIssueFieldValue(input: {
    issueId: "ISSUE_NODE_ID"
    issueFields: [
      { fieldId: "IFD_xxx", dateValue: "2026-04-15" }
      { fieldId: "IFT_xxx", textValue: "some text" }
      { fieldId: "IFN_xxx", numberValue: 3.0 }
      { fieldId: "IFSS_xxx", singleSelectOptionId: "OPTION_ID" }
    ]
  }) {
    issue { id title }
  }
}
```
`issueFields`中的每个条目都接受一个`fieldId`加上一个值参数：

|字段类型|值参数|格式||-----------|----------------|--------|
|日期|`dateValue`| ISO 8601日期字符串，例如`"2026-04-15"`|
|文本|`textValue`|字符串|
|数字|`numberValue`|浮动|
|单个选择|`singleSelectOptionId`|从字段的`options`列表|节点ID

要清除字段值，请设置为`delete: true`，而不是value参数。

##按字段值搜索

### GraphQL批量查询（推荐）

通过字段值查找问题的最可靠方法是通过GraphQL获取问题，并通过`issueFieldValues`进行过滤。搜索限定符语法（`field.name:value`）在所有环境中还不是可靠的。```bash
# Find all open P1 issues in a repo
gh api graphql -H "GraphQL-Features: issue_fields" -f query='
{
  repository(owner: "OWNER", name: "REPO") {
    issues(first: 100, states: OPEN) {
      nodes {
        number
        title
        updatedAt
        assignees(first: 3) { nodes { login } }
        issueFieldValues(first: 10) {
          nodes {
            __typename
            ... on IssueFieldSingleSelectValue {
              name
              field { ... on IssueFieldSingleSelect { name } }
            }
          }
        }
      }
    }
  }
}' --jq '
  [.data.repository.issues.nodes[] |
    select(.issueFieldValues.nodes[] |
      select(.field.name == "Priority" and .name == "P1")
    ) |
    {number, title, updatedAt, assignees: [.assignees.nodes[].login]}
  ]'
```
**`IssueFieldSingleSelectValue`:**的模式说明
-所选选项的显示文本是`.name`（不是`.value`）
-还提供：`.color`，`.description`,`.id`-父字段引用在`.field`（使用内联片段获取字段名）

###搜索限定符语法（实验性）

Issue字段也可以在搜索查询中使用点符号进行搜索。这需要REST上的`advanced_search=true`或GraphQL上的`ISSUE_ADVANCED`搜索类型，但是结果不一致，即使存在匹配问题也可能返回0结果。```
field.priority:P0                  # Single-select equals value
field.target-date:>=2026-04-01     # Date comparison
has:field.priority                 # Has any value set
no:field.priority                  # Has no value set
```
字段名使用**号**（小写，连字符为空格）。例如，“目标日期”变成`target-date`。```bash
# REST API (may not return results in all environments)
gh api "search/issues?q=repo:owner/repo+field.priority:P0+is:open&advanced_search=true" \
  --jq '.items[] | "#\(.number): \(.title)"'
```
b> **警告：**冒号符号（`field:Priority:P1`）被静默忽略。如果使用搜索限定符，请始终使用点表示法（`field.priority:P1`）。然而，上面的GraphQL批量查询方法更加可靠。请参阅[search.md]（search.md）获取完整的搜索指南。