# Issue Dependencies （Blocked By / Blocking）

依赖项允许您标记一个问题被另一个问题阻塞。这创建了一个正式的依赖关系，在UI中可见，并可通过API跟踪。不存在用于依赖的MCP工具；直接使用REST或GraphQL。

##使用REST API

**列出阻止此问题的问题：**```
GET /repos/{owner}/{repo}/issues/{issue_number}/dependencies/blocked_by
```
**添加一个阻塞依赖：**```
POST /repos/{owner}/{repo}/issues/{issue_number}/dependencies/blocked_by
Body: { "issue_id": 12345 }
```
`issue_id`是数字issue **ID**（不是issue号）。

**移除阻塞依赖：**```
DELETE /repos/{owner}/{repo}/issues/{issue_number}/dependencies/blocked_by/{issue_id}
```
##使用GraphQL

* *读依赖性:* *```graphql
{
  repository(owner: "OWNER", name: "REPO") {
    issue(number: 123) {
      blockedBy(first: 10) { nodes { number title state } }
      blocking(first: 10) { nodes { number title state } }
      issueDependenciesSummary { blockedBy blocking totalBlockedBy totalBlocking }
    }
  }
}
```
**添加一个依赖：**```graphql
mutation {
  addBlockedBy(input: {
    issueId: "BLOCKED_ISSUE_NODE_ID"
    blockingIssueId: "BLOCKING_ISSUE_NODE_ID"
  }) {
    blockingIssue { number title }
  }
}
```
**移除依赖项：**```graphql
mutation {
  removeBlockedBy(input: {
    issueId: "BLOCKED_ISSUE_NODE_ID"
    blockingIssueId: "BLOCKING_ISSUE_NODE_ID"
  }) {
    blockingIssue { number title }
  }
}
```
跟踪问题（只读）

任务列表跟踪关系可以通过GraphQL作为只读字段使用：

-`trackedIssues(first: N)`-在这个问题的任务列表中跟踪的问题
-`trackedInIssues(first: N)`-任务列表引用此问题的issues

在任务列表中引用问题时自动设置这些参数（`- [ ] #123`）。没有突变来控制它们。