#问题字段REST API引用

问题字段是问题的组织级自定义元数据。所有端点都需要API版本头：```
-H "X-GitHub-Api-Version: 2026-03-10"
```
##列出组织问题字段```bash
gh api /orgs/{org}/issue-fields \
  -H "X-GitHub-Api-Version: 2026-03-10"
```
返回一个字段对象数组：```json
[
  {
    "id": "IF_abc123",
    "name": "Priority",
    "content_type": "single_select",
    "options": [
      { "id": "OPT_1", "name": "Critical" },
      { "id": "OPT_2", "name": "High" },
      { "id": "OPT_3", "name": "Medium" },
      { "id": "OPT_4", "name": "Low" }
    ]
  },
  {
    "id": "IF_def456",
    "name": "Due Date",
    "content_type": "date",
    "options": null
  }
]
```
**字段类型：`text`，`single_select`,`number`,`date`**有用的jq过滤器**：```bash
gh api /orgs/{org}/issue-fields \
  -H "X-GitHub-Api-Version: 2026-03-10" \
  --jq '.[] | {id, name, content_type, options: [.options[]?.name]}'
```
读取Issue字段值```bash
gh api /repos/{owner}/{repo}/issues/{number}/issue-field-values \
  -H "X-GitHub-Api-Version: 2026-03-10"
```
返回问题的当前字段值。使用它在写入前检查值是否已经存在。

##写入问题字段值（POST, additive）

向问题添加值，但不删除其他字段的现有值。

**重要**：使用`repository_id`（整数），而不是`owner/repo`。```bash
# First, get the repository ID:
REPO_ID=$(gh api /repos/{owner}/{repo} --jq .id)

# Then write the value:
echo '[
  {
    "field_id": "IF_abc123",
    "value": "High"
  }
]' | gh api /repositories/$REPO_ID/issues/{number}/issue-field-values \
  -X POST \
  -H "X-GitHub-Api-Version: 2026-03-10" \
  --input -
```
###按字段类型格式化值

|字段类型|取值格式|示例||-----------|-------------|---------|
| text | String |`"value": "Some text"`|
| single_select |选项名称（string） |`"value": "High"`|
| number | number |`"value": 42`|
| date | ISO 8601日期字符串|`"value": "2025-03-15"`|

**键**：对于`single_select`， REST API接受选项**name**作为字符串。您不需要查找选项id。

###一次写入多个字段

在数组中传递多个对象以在单个调用中设置多个字段：```bash
echo '[
  {"field_id": "IF_abc123", "value": "High"},
  {"field_id": "IF_def456", "value": "2025-06-01"}
]' | gh api /repositories/$REPO_ID/issues/{number}/issue-field-values \
  -X POST \
  -H "X-GitHub-Api-Version: 2026-03-10" \
  --input -
```
##写入问题字段值（PUT，替换所有）

替换问题上的所有字段值。请谨慎使用。```bash
echo '[{"field_id": "IF_abc123", "value": "Low"}]' | \
  gh api /repositories/$REPO_ID/issues/{number}/issue-field-values \
    -X PUT \
    -H "X-GitHub-Api-Version: 2026-03-10" \
    --input -
```
**警告**:PUT删除请求体中不包含的任何字段值。始终使用POST进行迁移，以保留其他字段值。

# #权限

- **存储库**:"Issues"read/write- **组织**：“Issue Fields”read/write##速率限制

-适用标准速率限制（认证用户5000requests/hour）
—二级速率限制可能触发快速顺序写
-建议：呼叫间延迟100ms， 429时指数回退