#高级用法：输出，查询和参数

##目录
-[输出格式]（# Output - Formats）
- [JMESPath查询]（# JMESPath - Queries）
-[高级JMESPath查询]（# Advanced - JMESPath - Queries）
-[全局参数]（# Global - Arguments）
-[常用参数]（# Common - Parameters）
- [Git别名]（# Git -alias）
-[获取帮助]（# Getting - Help）

---

##输出格式

所有命令支持多种输出格式：```bash
# Table format (human-readable)
az pipelines list --output table

# JSON format (default, machine-readable)
az pipelines list --output json

# JSONC (colored JSON)
az pipelines list --output jsonc

# YAML format
az pipelines list --output yaml

# YAMLC (colored YAML)
az pipelines list --output yamlc

# TSV format (tab-separated values)
az pipelines list --output tsv

# None (no output)
az pipelines list --output none
```
## JMESPath查询

过滤和转换输出：```bash
# Filter by name
az pipelines list --query "[?name=='myPipeline']"

# Get specific fields
az pipelines list --query "[].{Name:name, ID:id}"

# Chain queries
az pipelines list --query "[?name.contains('CI')].{Name:name, ID:id}" --output table

# Get first result
az pipelines list --query "[0]"

# Get top N
az pipelines list --query "[0:5]"
```
##高级JMESPath查询

过滤和排序```bash
# Filter by multiple conditions
az pipelines list --query "[?name.contains('CI') && enabled==true]"

# Filter by status and result
az pipelines runs list --query "[?status=='completed' && result=='succeeded']"

# Sort by date (descending)
az pipelines runs list --query "sort_by([?status=='completed'], &finishTime | reverse(@))"

# Get top N items after filtering
az pipelines runs list --query "[?result=='succeeded'] | [0:5]"
```
嵌套查询```bash
# Extract nested properties
az pipelines show --id $PIPELINE_ID --query "{Name:name, Repo:repository.{Name:name, Type:type}, Folder:folder}"

# Query build details
az pipelines build show --id $BUILD_ID --query "{ID:id, Number:buildNumber, Status:status, Result:result, Requested:requestedFor.displayName}"
```
复杂过滤```bash
# Find pipelines with specific YAML path
az pipelines list --query "[?process.type.name=='yaml' && process.yamlFilename=='azure-pipelines.yml']"

# Find PRs from specific reviewer
az repos pr list --query "[?contains(reviewers[?displayName=='John Doe'].displayName, 'John Doe')]"

# Find work items with specific iteration and state
az boards work-item show --id $WI_ID --query "{Title:fields['System.Title'], State:fields['System.State'], Iteration:fields['System.IterationPath']}"
```
# # #聚合```bash
# Count items by status
az pipelines runs list --query "groupBy([?status=='completed'], &[result]) | {Succeeded: [?key=='succeeded'][0].count, Failed: [?key=='failed'][0].count}"

# Get unique reviewers
az repos pr list --query "unique_by(reviewers[], &displayName)"

# Sum values
az pipelines runs list --query "[?result=='succeeded'] | [].{Duration:duration} | [0].Duration"
```
条件变换```bash
# Format dates
az pipelines runs list --query "[].{ID:id, Date:createdDate, Formatted:createdDate | format_datetime(@, 'yyyy-MM-dd HH:mm')}"

# Conditional output
az pipelines list --query "[].{Name:name, Status:(enabled ? 'Enabled' : 'Disabled')}"

# Extract with defaults
az pipelines show --id $PIPELINE_ID --query "{Name:name, Folder:folder || 'Root', Description:description || 'No description'}"
```
复杂的工作流程```bash
# Find longest running builds
az pipelines build list --query "sort_by([?result=='succeeded'], &queueTime) | reverse(@) | [0:3].{ID:id, Number:buildNumber, Duration:duration}"

# Get PR statistics per reviewer
az repos pr list --query "groupBy([], &reviewers[].displayName) | [].{Reviewer:@.key, Count:length(@)}"

# Find work items with multiple child items
az boards work-item relation list --id $PARENT_ID --query "[?rel=='System.LinkTypes.Hierarchy-Forward'] | [].{ChildID:url | split('/', @) | [-1]}"
```
##全局参数

可用于所有命令：

| |参数说明||---|---|
|`--help`/`-h`|显示命令帮助|
|`--output`/`-o`|输出格式（json、jsonc、none、table、tsv、yaml、yamlc） |
|`--query`|过滤输出的JMESPath查询字符串|
|`--verbose`|增加日志记录的篇幅|
|`--debug`|显示所有调试日志|
|`--only-show-errors`|只显示错误，抑制警告|
|`--subscription`|订阅|的名称或ID
|`--yes`/`-y`|跳过确认提示|

##常用参数

| |参数说明||---|---|
|`--org`/`--organization`| Azure DevOps组织URL（例如，`https://dev.azure.com/{org}`） |
|`--project`/`-p`|项目名称或ID |
|`--detect`|自动检测组织从git配置|
|`--yes`/`-y`|跳过确认提示|
|`--open`|打开web浏览器中的资源|
|`--subscription`| Azure订阅（Azure资源）|

## Git别名

启用git别名后：```bash
# Enable Git aliases
az devops configure --use-git-aliases true

# Use Git commands for DevOps operations
git pr create --target-branch main
git pr list
git pr checkout 123
```
##寻求帮助```bash
# General help
az devops --help

# Help for specific command group
az pipelines --help
az repos pr --help

# Help for specific command
az repos pr create --help

# Search for examples
az find "az repos pr create"
```
