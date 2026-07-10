#标签API参考

标签迁移流程中使用的GitHub标签REST API端点参考。

列出存储库中的标签```
GET /repos/{owner}/{repo}/labels
```
返回存储库上定义的所有标签。分页（每页最多100页）。

* * CLI快捷方式:* *```bash
gh label list -R {owner}/{repo} --limit 1000 --json name,color,description
```
**响应字段：**`id`、`node_id`、`url`、`name`、`description`、`color`、`default`。

##按标签列出问题```
GET /repos/{owner}/{repo}/issues?labels={label_name}&state=all&per_page=100
```
返回与标签匹配的问题（和拉取请求）。通过检查“`pull_request`”字段是否存在来过滤pr。

* * CLI快捷方式:* *```bash
gh issue list -R {owner}/{repo} --label "{label_name}" --state all \
  --json number,title,labels --limit 1000
```
`gh issue list`命令会自动排除pr。

**分页：**在CLI中使用`--limit`或在REST中使用`page`查询参数。对于具有bbb1000匹配问题的repos，可以通过Link头使用基于光标的分页。

##从Issue中移除标签```
DELETE /repos/{owner}/{repo}/issues/{issue_number}/labels/{label_name}
```
从问题中删除单个标签。返回`200 OK`和问题上的剩余标签。

**重要：** url编码标签名称与空格或特殊字符：
-`good first issue`→`good%20first%20issue`-`bug/critical`→`bug%2Fcritical`* * CLI快捷方式:* *```bash
gh api /repos/{owner}/{repo}/issues/{number}/labels/{label_name} -X DELETE
```
为Issue添加标签```
POST /repos/{owner}/{repo}/issues/{issue_number}/labels
```
身体:`{"labels": ["label1", "label2"]}`迁移通常不需要，但对于回滚场景很有用。

# #笔记

-标签是重定义作用域的。相同的标签名称可以独立存在于不同的仓库中。
-没有MCP工具列出回购标签。使用`gh label list`或REST API。
—MCP工具`mcp__github__list_issues`支持`labels`过滤器，用于按标签获取问题。
-标签名称是不区分大小写的匹配目的，但API保留原来的大小写。
-每期最多标签数：没有硬性限制，但实际上有几十个。