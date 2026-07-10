# Issue Types （Advanced GraphQL）

问题类型（Bug、Feature、Task、Epic等）在组织级别定义，并由存储库继承。他们对问题进行分类，而不是标签。

对于基本用法，MCP工具可以本地处理问题类型。调用`mcp__github__list_issue_types`来发现类型，并将`type: "Bug"`传递给`mcp__github__create_issue`或`mcp__github__update_issue`。本参考资料涵盖了高级GraphQL操作。

## GraphQL特性头

所有GraphQL问题类型操作都需要`GraphQL-Features: issue_types`HTTP标头。

列表类型（org或repo级别）```graphql
# Header: GraphQL-Features: issue_types
{
  organization(login: "OWNER") {
    issueTypes(first: 20) {
      nodes { id name color description isEnabled }
    }
  }
}
```
还可以通过`repository.issueTypes`列出每个repo的类型，或者通过`repository.issueType(name: "Bug")`按名称查找类型。

读取问题的类型```graphql
# Header: GraphQL-Features: issue_types
{
  repository(owner: "OWNER", name: "REPO") {
    issue(number: 123) {
      issueType { id name color }
    }
  }
}
```
##在现有问题上设置类型```graphql
# Header: GraphQL-Features: issue_types
mutation {
  updateIssueIssueType(input: {
    issueId: "ISSUE_NODE_ID"
    issueTypeId: "IT_xxx"
  }) {
    issue { id issueType { name } }
  }
}
```
创建类型问题```graphql
# Header: GraphQL-Features: issue_types
mutation {
  createIssue(input: {
    repositoryId: "REPO_NODE_ID"
    title: "Fix login bug"
    issueTypeId: "IT_xxx"
  }) {
    issue { id number issueType { name } }
  }
}
```
若要清除该类型，请将“`issueTypeId`”设置为“`null`”。

##可选颜色`GRAY`,`BLUE`,`GREEN`,`YELLOW`,`ORANGE`,`RED`,`PINK`,`PURPLE`