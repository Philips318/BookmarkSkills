# Projects V2 API参考（用于迁移）

本参考涵盖了字段迁移所需的Projects V2 API的子集：发现项目字段和读取项值。

列出项目字段

通过MCP工具```
mcp__github__projects_list(
  owner: "{org}",
  project_number: {n},
  method: "list_project_fields"
)
```
###通过GraphQL```bash
gh api graphql -f query='
  query {
    organization(login: "ORG") {
      projectV2(number: N) {
        fields(first: 30) {
          pageInfo { hasNextPage endCursor }
          nodes {
            ... on ProjectV2Field {
              id
              name
              dataType
            }
            ... on ProjectV2SingleSelectField {
              id
              name
              dataType
              options { id name }
            }
            ... on ProjectV2IterationField {
              id
              name
              dataType
            }
          }
        }
      }
    }
  }'
```
###字段数据类型

| dataType |描述|迁移到||----------|-------------|-------------|
| TEXT |自由格式文本|`text`问题字段|
| SINGLE_SELECT |下拉选项|`single_select`问题字段|
| NUMBER |数值|`number`问题字段|
| DATE |日期值|`date`问题字段|
|迭代|Sprint/iteration循环|无等效（跳过）|

##列出项目项（带字段值）

通过MCP工具```
mcp__github__projects_list(
  owner: "{org}",
  project_number: {n},
  method: "list_project_items"
)
```
返回分页结果。每个项目包括：
-项目类型（ISSUE, DRAFT_ISSUE, PULL_REQUEST）
-内容参考（回购所有者、回购名称、发行号）
-所有项目字段的字段值

###通过GraphQL```bash
gh api graphql -f query='
  query($cursor: String) {
    organization(login: "ORG") {
      projectV2(number: N) {
        items(first: 100, after: $cursor) {
          pageInfo { hasNextPage endCursor }
          nodes {
            type
            content {
              ... on Issue {
                number
                repository { nameWithOwner }
              }
            }
            fieldValues(first: 20) {
              pageInfo { hasNextPage endCursor }
              nodes {
                ... on ProjectV2ItemFieldTextValue { text field { ... on ProjectV2Field { name } } }
                ... on ProjectV2ItemFieldSingleSelectValue { name field { ... on ProjectV2SingleSelectField { name } } }
                ... on ProjectV2ItemFieldNumberValue { number field { ... on ProjectV2Field { name } } }
                ... on ProjectV2ItemFieldDateValue { date field { ... on ProjectV2Field { name } } }
              }
            }
          }
        }
      }
    }
  }' -f cursor="$CURSOR"
```
迁移的重要注意事项

- **分页**：项目可以有多达10,000项。始终使用`pageInfo.hasNextPage`和`pageInfo.endCursor`分页。
- **草案项目**：带有`type: DRAFT_ISSUE`的项目没有实际问题。在迁移过程中跳过这些。
- **拉请求**：项目与`type: PULL_REQUEST`是pr，而不是问题。问题字段仅适用于问题。跳过这些。
- **Cross-repo**：单个项目可以包含来自多个存储库的问题。按回购进行分组，以批处理回购ID查找。
—**字段值访问**：每个字段值节点类型不同（`ProjectV2ItemFieldTextValue`、`ProjectV2ItemFieldSingleSelectValue`等）。处理每种类型。