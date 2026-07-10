#子问题和父问题

子问题让你把工作分解成分层的任务。每个父问题可以有多达100个子问题，嵌套深度可达8层。子问题可以跨越同一所有者的存储库。

推荐工作流程

创建子问题最简单的方法是两步：创建问题，然后链接它。```bash
# Step 1: Create the issue and capture its numeric ID
ISSUE_ID=$(gh api repos/{owner}/{repo}/issues \
  -X POST \
  -f title="Sub-task title" \
  -f body="Description" \
  --jq '.id')

# Step 2: Link it as a sub-issue of the parent
# IMPORTANT: sub_issue_id must be an integer. Use --input (not -f) to send JSON.
echo "{\"sub_issue_id\": $ISSUE_ID}" | gh api repos/{owner}/{repo}/issues/{parent_number}/sub_issues -X POST --input -
```
**为什么是`--input`而不是`-f`？**`gh api -f`标志将所有值作为字符串发送，但API要求`sub_issue_id`作为整数。使用`-f sub_issue_id=12345`将返回一个422错误。

或者，使用GraphQL`createIssue`和`parentIssueId`一步完成（请参阅下面的GraphQL部分）。

##使用MCP工具

* *列表sub-issues: * *
用`method: "get_sub_issues"`、`owner`、`repo`和`issue_number`调用`mcp__github__issue_read`。

**创建一个问题作为子问题：**
没有用于直接创建子问题的MCP工具。使用上面的工作流或GraphQL。

##使用REST API

* *列表sub-issues: * *```bash
gh api repos/{owner}/{repo}/issues/{issue_number}/sub_issues
```
**获取父问题：**```bash
gh api repos/{owner}/{repo}/issues/{issue_number}/parent
```
**添加一个现有问题作为子问题：**```bash
# sub_issue_id is the numeric issue ID (not the issue number)
# Get it from the .id field when creating or fetching an issue
echo '{"sub_issue_id": 12345}' | gh api repos/{owner}/{repo}/issues/{parent_number}/sub_issues -X POST --input -
```
要移动已经有父节点的子问题，请将`"replace_parent": true`添加到JSON主体中。

**删除子问题：**```bash
echo '{"sub_issue_id": 12345}' | gh api repos/{owner}/{repo}/issues/{parent_number}/sub_issue -X DELETE --input -
```
**重新确定子问题的优先级：**```bash
echo '{"sub_issue_id": 6, "after_id": 5}' | gh api repos/{owner}/{repo}/issues/{parent_number}/sub_issues/priority -X PATCH --input -
```
使用`after_id`或`before_id`来定位子问题相对于另一个子问题的位置。

##使用GraphQL

**读取父问题和子问题：**```graphql
{
  repository(owner: "OWNER", name: "REPO") {
    issue(number: 123) {
      parent { number title }
      subIssues(first: 50) {
        nodes { number title state }
      }
      subIssuesSummary { total completed percentCompleted }
    }
  }
}
```
**增加一个子问题：**```graphql
mutation {
  addSubIssue(input: {
    issueId: "PARENT_NODE_ID"
    subIssueId: "CHILD_NODE_ID"
  }) {
    issue { id }
    subIssue { id number title }
  }
}
```
您还可以使用`subIssueUrl`而不是`subIssueId`（传递问题的HTML URL）。添加`replaceParent: true`将子问题从另一个父问题移开。

**直接创建一个问题作为子问题：**```graphql
mutation {
  createIssue(input: {
    repositoryId: "REPO_NODE_ID"
    title: "Implement login validation"
    parentIssueId: "PARENT_NODE_ID"
  }) {
    issue { id number }
  }
}
```
**删除子问题：**```graphql
mutation {
  removeSubIssue(input: {
    issueId: "PARENT_NODE_ID"
    subIssueId: "CHILD_NODE_ID"
  }) {
    issue { id }
  }
}
```
**重新确定子问题的优先级：**```graphql
mutation {
  reprioritizeSubIssue(input: {
    issueId: "PARENT_NODE_ID"
    subIssueId: "CHILD_NODE_ID"
    afterId: "OTHER_CHILD_NODE_ID"
  }) {
    issue { id }
  }
}
```
使用`afterId`或`beforeId`相对于另一子问题定位。