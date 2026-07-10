---
name: issue-fields-migration
description: 'Bulk-migrate metadata to GitHub issue fields from two sources: repo labels (e.g. priority labels to a Priority field) and Project V2 fields. Use when users say "migrate my labels to issue fields", "migrate project fields to issue fields", "convert labels to issue fields", "copy project field values to issue fields", or ask about adopting issue fields. Issue fields are org-level typed metadata (single select, text, number, date) that replace label-based workarounds with structured, searchable, cross-repo fields.'
---
#问题字段迁移

[问题字段]（https://github.blog/changelog/2026-03-12-issue-fields-structured-issue-metadata-is-in-public-preview/）是组织级别类型的元数据（单个选择、文本、数字、日期），它用结构化的、可搜索的、跨报告的字段取代了基于标签的变通方法。每个组织都预先配置了`Priority`、`Effort`、`Start date`和`Target date`，支持多达25个自定义字段。

此技能将现有元数据从两个来源批量迁移到问题字段：

- **Repo标签**：将标签如`p0`，`p1`，`priority/high`转换为结构化问题字段值（例如优先级字段）。支持一次迁移多个标签，并在迁移后可选择删除它们。
- **项目V2字段**：从GitHub项目复制字段值（单个选择，文本，数字，日期，迭代）到等效的组织级问题字段。

##何时使用-用户添加了与现有项目字段重叠的组织级问题字段
—用户希望在删除旧项目字段之前将项目字段的值复制到发布字段
-用户询问“迁移”、“转移”或“复制”项目字段数据到issue字段
-用户希望将repo标签（例如p0， p1, p2, p3）转换为issue字段值（例如Priority字段）
—用户询问是否将标签替换为问题字段或采用问题字段后是否清理标签

# #先决条件

-目标组织必须启用问题字段
-问题字段必须已经存在于组织级别
—对于项目字段迁移：问题字段必须添加到项目中
—对于标签迁移：标签必须存在于目标版本中。
-用户必须具有对repos（和项目，如果迁移项目字段）的写访问权限
-`gh`CLI必须使用合适的作用域进行认证##可用工具

MCP工具（读取操作）

|工具|用途||------|---------|
|`mcp__github__projects_list`|列出项目字段（`list_project_fields`），列出项目项值（`list_project_items`） |
|`mcp__github__projects_get`|获取特定项目字段或项|的详细信息

### cli / rest API

|操作|命令||-----------|---------|
|列出组织问题字段|`gh api /orgs/{org}/issue-fields -H "X-GitHub-Api-Version: 2026-03-10"`|
|读取问题字段值|`gh api /repos/{owner}/{repo}/issues/{number}/issue-field-values -H "X-GitHub-Api-Version: 2026-03-10"`|
|写入问题字段值|`gh api /repositories/{repo_id}/issues/{number}/issue-field-values -X POST -H "X-GitHub-Api-Version: 2026-03-10" --input -`|
|获取存储库ID |`gh api /repos/{owner}/{repo} --jq .id`|
|列出回购标签|`gh label list -R {owner}/{repo} --limit 1000 --json name,color,description`|
|按标签|`gh issue list -R {owner}/{repo} --label "{name}" --state all --json number,title,labels --limit 1000`|列出问题
|从问题|`gh api /repos/{owner}/{repo}/issues/{number}/labels/{label_name} -X DELETE`|中删除标签

请参阅[references/issue-fields-api.md](references/issue-fields-api.md), [references/projects-api.md]（references/projects-api.md）和[references/labels-api.md]（references/labels-api.md）了解完整的API详细信息。

# #工作流程

###步骤0：迁移源

询问用户他们正在迁移什么：

1. “您是在迁移标签还是项目字段？”**
- **标签**：继续下面的[标签迁移流]（# Label - Migration - Flow）。
- **项目字段**：继续下面的[项目字段迁移流]（# Project - Field - Migration - Flow）。

2. 如果用户输入**labels**：
-问：“哪些组织和仓库包含标签？”
-问：“你想迁移哪些标签？”（他们可以说出它们的名字，或者说“先给我看看标签”）3. 如果用户说**项目字段**：
-问：“你能分享你的项目链接或告诉我组织名称和项目编号吗？”
-问：“你想迁移到哪个领域？”

---

标签迁移流程

当用户希望将回购标签转换为问题字段值时，使用此流。标签只能映射到`single_select`问题字段（每个标签名称映射到一个选项值）。

####阶段L1：输入和标签发现

1. 要求用户输入：**org名称**和**repo(s)**以进行迁移。
2. 从每个repo获取标签：```bash
gh label list -R {owner}/{repo} --limit 1000 --json name,color,description
```
3. 获取org issue字段：```bash
gh api /orgs/{org}/issue-fields \
  -H "X-GitHub-Api-Version: 2026-03-10" \
  --jq '.[] | {id, name, content_type, options: [.options[]?.name]}'
```
4. **过滤**（对于有许多标签的repo）：如果repo有50多个标签，按共同的前缀（例如，`priority-*`,`team-*`,`type-*`）或颜色分组。让用户在映射前使用“显示匹配优先级的标签”或“显示蓝色标签”进行过滤。永远不要一次性丢弃100多个标签。

5. 询问用户哪些标签映射到哪个问题字段和选项。支持以下模式：
- **单标签到单字段**：例如，标签“bug”→类型字段，“bug”选项
—**多个标签到一个字段**（散装）：例如，标签p0， p1, p2， p3→优先级字段和匹配选项
- **多个标签到多个字段**：例如，p1→Priority + frontend→Team。作为单独的映射组处理。6. **自动建议映射**：对于每个标签，尝试使用以下模式（按顺序）匹配问题字段选项：
- **精确匹配**（不区分大小写）：标签`Bug`→选项`Bug`- **前缀-数字** (`{prefix}-{n}`→`{P}{n}`)：标签`priority-1`→选项`P1`- **条带分隔符**（连字符、下划线、空格）：标签`good_first_issue`→选项`Good First Issue`- **子字符串包含**：标签`type: bug`→选项`Bug`同时呈现所有建议，供用户确认、更正或跳过。

* *输出示例:* *```
Labels in github/my-repo (showing relevant ones):
  p0, p1, p2, p3, bug, enhancement, frontend, backend

Org issue fields (single_select):
  Priority: Critical, P0, P1, P2, P3
  Type: Bug, Feature, Task
  Team: Frontend, Backend, Design

Suggested mappings:
  Label "p0" → Priority "P0"
  Label "p1" → Priority "P1"
  Label "p2" → Priority "P2"
  Label "p3" → Priority "P3"
  Label "bug" → Type "Bug"
  Label "frontend" → Team "Frontend"
  Label "backend" → Team "Backend"
  Label "enhancement" → (no auto-match; skip or map manually)

Confirm, adjust, or add more mappings?
```
####阶段L2：冲突检测

在完成标签到选项的映射之后，检查冲突。当一个问题有多个标签映射到同一个问题字段时，就会发生冲突（因为single_select字段只能保存一个值）。

1. 按目标问题字段对标签映射进行分组。
2. 对于具有多个标签源的每个字段，请注意潜在的冲突。
3. 向用户询问冲突解决策略：
- **第一个匹配**：使用找到的第一个匹配标签（按标签映射列表的顺序）
—**跳过**：跳过标签冲突的问题并报告
- **Manual**：呈现每个冲突供用户决定

* *的例子:* *```
Potential conflict: labels "p0" and "p1" both map to the Priority field.
If an issue has both labels, which value should win?

Options:
  1. First match (use "p0" since it appears first in the mapping)
  2. Skip conflicting issues
  3. I'll decide case by case
```
#### L3阶段：飞行前检查和数据扫描

1. 对于每个repo，验证写访问并缓存`repository_id`：```bash
gh api /repos/{owner}/{repo} --jq '{full_name, id, permissions: .permissions}'
```
2. 对于映射中的每个标签，获取匹配问题：```bash
gh issue list -R {owner}/{repo} --label "{label_name}" --state all \
  --json number,title,labels,type --limit 1000
```
**警告**:`--limit 1000`静默截断结果。如果您预计一个标签可能有超过1000个问题，请手动分页或首先验证总数（例如，`gh issue list --label "X" --state all --json number | jq length`）。

**PR过滤**:`gh issue list`返回issue和PR。如果用户只想迁移问题，则在`--json`输出和`type == "Issue"`的过滤器中包含`type`。

3. 如果**所有选择的标签返回0问题，停止并告诉用户。建议：尝试不同的标签，检查拼写，或者尝试不同的存储库。不要进行空迁移。

4. 对于多仓库迁移，在所有指定的仓库中重复。5. 对于发现的每个问题：
-检查问题是否已经有目标问题字段的值（如果设置跳过）。
—检测多标签冲突（issue对同一个字段有两个标签）。
-应用在阶段L2中选择的冲突解决策略。
—分类：**迁移**、**跳过（已设置）**、**跳过（冲突）**或**跳过（无匹配标签）**。

####阶段L4：预览/试运行

在任何写作之前提交一份摘要。

* *例预览:* *```
Label Migration Preview

Source: labels in github/my-repo
Target fields: Priority, Type, Team

| Category                | Count |
|-------------------------|-------|
| Issues to migrate       |   156 |
| Already set (skip)      |    12 |
| Conflicting labels (skip)|    3 |
| Total issues with labels|   171 |

Label breakdown:
  "p1" → Priority "P1": 42 issues
  "p2" → Priority "P2": 67 issues
  "p3" → Priority "P3": 38 issues
  "bug" → Type "Bug": 9 issues

Sample changes (first 5):
  github/my-repo#101: Priority → "P1"
  github/my-repo#203: Priority → "P2", Type → "Bug"
  github/my-repo#44:  Priority → "P3"
  github/my-repo#310: Priority → "P1"
  github/my-repo#7:   Type → "Bug"

After migration, do you also want to remove the migrated labels from issues? (optional)

Estimated time: ~24s (156 API calls at 0.15s each)

Proceed?
```
####阶段L5：执行

1. 对于每个要迁移的问题，写下问题字段值（与项目字段迁移相同的端点）：```bash
echo '{"issue_field_values": [{"field_id": FIELD_ID, "value": "OPTION_NAME"}]}' | \
  gh api /repositories/{repo_id}/issues/{number}/issue-field-values \
    -X POST \
    -H "X-GitHub-Api-Version: 2026-03-10" \
    --input -
```
将`FIELD_ID`替换为整数字段ID（例如，`1`），并将`OPTION_NAME`替换为选项名称字符串。

2. 如果用户选择删除标签，则在成功写入字段后删除每个迁移的标签：```bash
gh api /repos/{owner}/{repo}/issues/{number}/labels/{label_name} -X DELETE
```
包含空格或特殊字符的url编码标签名称。

3. **起搏**：呼叫间延迟100ms。HTTP 429上的指数回退（1s, 2s, 4s，最多30s）。
4. 进度：每25项报告一次（例如，“迁移75/156问题……”）。
5. **错误处理**：日志失败，但继续。单独包括标签移除失败。
6. * * * *最终总结:```
Label Migration Complete

| Result                | Count |
|-----------------------|-------|
| Fields set            |   153 |
| Labels removed        |   153 |
| Skipped               |    15 |
| Failed (field write)  |     2 |
| Failed (label remove) |     1 |

Failed items:
  github/my-repo#501: 403 Forbidden (insufficient permissions)
  github/my-repo#88:  422 Validation failed (field not available on repo)
  github/my-repo#120: label removal failed (404, label already removed)
```
---

项目字段迁移流程

当用户想要将值从GitHub Project V2字段复制到相应的组织级问题字段时，使用此流。

按顺序遵循这六个阶段。总是在执行前预览。

####阶段1：输入和发现

1. 向用户询问：**org名称**和**项目编号**（或项目URL）。
2. 获取项目字段：```bash
# Use MCP tool
mcp__github__projects_list(owner: "{org}", project_number: {n}, method: "list_project_fields")
```
3. 获取org issue字段：```bash
gh api /orgs/{org}/issue-fields \
  -H "X-GitHub-Api-Version: 2026-03-10" \
  --jq '.[] | {id, name, content_type, options: [.options[]?.name]}'
```
4. **过滤掉代理字段**：在项目上启用问题字段后，一些项目字段显示为“代理”条目，单个选择类型为空`options: []`。这些字段反映了真正的问题字段，应该被忽略。只匹配具有实际选项值的项目字段。

5. 根据名称（不区分大小写）与兼容类型自动匹配字段：

|项目字段类型|问题字段类型|兼容？||-------------------|-----------------|-------------|
| TEXT | TEXT |是，直接复制|
| SINGLE_SELECT | SINGLE_SELECT |是，需要|的选项映射
| NUMBER | NUMBER |是，直接复制|
| DATE | DATE |是，直接复制|
| ITERATION | (none) |无对应；跳过并警告|

6. 将建议的字段映射显示为一个表。让用户确认、调整或跳过字段。

* *输出示例:* *```
Found 3 potential field mappings:

| # | Project Field      | Type          | Issue Field        | Status     |
|---|-------------------|---------------|--------------------|------------|
| 1 | Priority (renamed) | SINGLE_SELECT | Priority           | Auto-match |
| 2 | Due Date           | DATE          | Due Date           | Auto-match |
| 3 | Sprint             | ITERATION     | (no equivalent)    | Skipped    |

Proceed with fields 1 and 2? You can also add manual mappings.
```
#### P2阶段：选项映射（仅限单个选择字段）

对于每个匹配的单选择对：

1. 比较项目字段和问题字段之间的选项名称（不区分大小写）。
2. 具有相同名称的自动匹配选项。
3. 对于任何未映射的项目字段选项，在单个摘要中呈现** ** *未映射的选项，并要求用户一次提供所有选项的映射。不要逐一提示；将它们批量放入单个交换器中。
4. 显示最终的选项映射表以供确认。

* *输出示例:* *```
Option mapping for "Release - Target":

Auto-matched (case-insensitive):
  "GA" → "GA"
  "Private Preview" → "Private Preview"
  "Public Preview" → "Public Preview"

Unmapped project options (need your input):
  1. "Internal Only" → which issue field option? (or skip)
  2. "Retired" → which issue field option? (or skip)
  3. "Beta" → which issue field option? (or skip)
  4. "Deprecated" → which issue field option? (or skip)

Available issue field options not yet mapped: "Internal", "Sunset", "Beta Testing", "End of Life"

Please provide mappings for all 4 options above (e.g., "1→Internal, 2→Sunset, 3→Beta Testing, 4→skip").
```
#### P3阶段：飞行前检查

在扫描项之前，请验证对可能被触及的每个存储库的写访问权限：

1. 从项目项（第一页）中，收集唯一的`{owner}/{repo}`值集。
2. 对于每个唯一的repo，验证被认证的用户具有Issues write权限：```bash
gh api /repos/{owner}/{repo} --jq '{full_name, permissions: .permissions}'
```
3. 如果任何repo显示`push: false`或`triage: false`，在继续之前警告用户。这些repos中的项将在写入时失败。
4. 现在缓存每个回购的`repository_id`（整数）；你会在第六阶段用到它：```bash
gh api /repos/{owner}/{repo} --jq .id
```
####阶段P4：数据扫描

1. 使用MCP获取所有项目项。**重要**：对于超过200项的项目，`gh api graphql --paginate`是不可靠的（它没有适当的分隔符连接JSON响应，并且可能超时）。使用内部处理分页的MCP工具，或使用显式的基于游标的分页：```bash
# Preferred: use MCP tool (handles pagination automatically)
mcp__github__projects_list(owner: "{org}", project_number: {n}, method: "list_project_items")

# Fallback for large projects: manual cursor-based pagination
# Fetch 100 items per page, advancing the cursor each time.
# Process each page before fetching the next to avoid memory issues.
# Save progress (page number or last cursor) so you can resume if interrupted.
```
2. 对于每个项目：
-如果它是一个草案项目（不是一个真正的问题）跳过。
—提取源项目字段值。
—如果source值为空，则跳过。
-检查问题是否已经有目标问题字段的值：```bash
gh api /repos/{owner}/{repo}/issues/{number}/issue-field-values \
  -H "X-GitHub-Api-Version: 2026-03-10"
```
—如果issue字段已经有值，跳过它（保留现有数据）。

3. 将每个项目分为以下几类：
—**Migrate**：有源值，没有目标值
- **跳过（已设置）**：目标问题字段已经有值
- **跳过（无来源）**：该项目的项目字段为空
- **跳过（草案）**：项目是一个草案，不是一个真正的问题
- **跳过（未映射选项）**：单个选择值未映射

####第五阶段：预演/试运行

在任何写作之前提交一份摘要。

**如果用户要求干运行**：显示完整的详细报告（每个问题，其当前值，建议的新值和跳过原因）并停止。不要执行。

**否则（预览模式）**：显示汇总计数和更改示例，然后要求确认。

* *例预览:* *```
Migration Preview for Project #42

Fields to migrate: Priority, Due Date

| Category               | Count |
|------------------------|-------|
| Items to migrate       |   847 |
| Already set (skip)     |    23 |
| No source value (skip) |   130 |
| Draft items (skip)     |    12 |
| Total project items    | 1,012 |

Sample changes (first 5):
  github/repo-a#101: Priority → "High"
  github/repo-a#203: Priority → "Medium", Due Date → "2025-03-15"
  github/repo-b#44:  Priority → "Low"
  github/repo-a#310: Due Date → "2025-04-01"
  github/repo-c#7:   Priority → "Critical"

Estimated time: ~127s (847 API calls at 0.15s each)

Proceed with migration? This will update 847 issues across 3 repositories.
```
####阶段P6：执行

1. 使用在阶段3中缓存的`repository_id`值。

2. 对于要迁移的每一项，写出issue字段值：```bash
echo '{"issue_field_values": [{"field_id": FIELD_ID, "value": "VALUE"}]}' | \
  gh api /repositories/{repo_id}/issues/{number}/issue-field-values \
    -X POST \
    -H "X-GitHub-Api-Version: 2026-03-10" \
    --input -
```
将`FIELD_ID`替换为整数字段ID（例如，`1`），并将`VALUE`替换为值字符串。

3. **踱步**：在API调用之间增加100ms的延迟。在HTTP 429响应中，使用指数回退（1秒、2秒、4秒，最多30秒）。
4. 进度：每25项报告一次状态（例如，“迁移了75/847项…”）。
5. **错误处理**：日志失败，但继续处理剩余项目。
6. * * * *最终总结:```
Migration Complete

| Result  | Count |
|---------|-------|
| Success |   842 |
| Skipped |   165 |
| Failed  |     5 |

Failed items:
  github/repo-a#501: 403 Forbidden (insufficient permissions)
  github/repo-b#88:  422 Validation failed (field not available on repo)
  ...
```
##重要事项- **写入端点quirk**：写入问题字段值的REST API使用`repository_id`（整数），而不是`owner/repo`。总是先用`gh api /repos/{owner}/{repo} --jq .id`查找回购ID。
- **单选择值**:REST API接受选项**名**作为字符串（不是选项id）。这使得项目字段和标签的映射都很简单。
- **回读值**：当从API响应中读取问题字段值时，使用`.single_select_option.name`作为人类可读的值。`.value`属性返回内部选项ID（像`1201`这样的整数），而不是显示名称。
- **API版本头**：所有问题字段端点要求`X-GitHub-Api-Version: 2026-03-10`。
- **Cross-repo items**：一个项目可以包含来自多个仓库的问题。缓存每个存储库的repo ID，以避免冗余查找。
- **保留现有值**：永远不要覆盖已经设置的问题字段值。跳过这些项目。
—**迭代字段**：没有i问题域等价。总是警告用户并跳过。
- **草案项**：与实际问题无关的项目项不能有问题字段值。带着便条跳过。
- **标签是repo范围内的：不像项目字段，标签存在于每个repo。相同的标签名称可能存在于多个repos中；迁移分别应用于每一个。
- **标签冲突**：一个问题可以有多个标签映射到同一个single_select字段。始终在执行之前检测并解决这些问题。
—**标签移除可选**：迁移后，用户可能需要保留标签作为备份，也可能需要移除标签。移除之前一定要问清楚。
—** url编码标签名称**：包含空格或特殊字符的标签在REST API路径中使用时必须是url编码的（例如，`good%20first%20issue`）。
**为规模生成脚本**：对于100+问题的迁移，生成一个独立的shell脚本，而不是每次执行一个API调用我通过代理。这更快、可恢复，并避免了代理超时问题。
- **幂等迁移**：重新运行迁移是安全的。已经设置了目标字段值的问题将被跳过。这意味着您可以安全地恢复部分迁移，而无需重复工作。
- **`--limit 1000`截断**:`gh issue list --limit 1000`在1000个结果时静默停止。对于问题较多的标签，使用`--jq`和基于光标的分页或运行多个过滤查询（例如，按日期范围）进行分页。
- **macOS bash版本**:macOS自带bash 3。不支持`declare -A`（关联数组）。生成的脚本应该使用posix兼容的结构，或者注意不兼容性并建议使用`brew install bash`。
- **问题vs PRs**:`gh issue list`返回问题和拉请求。如果迁移应该只针对问题，那么在`--json`输出中包括`type`，并为`type == "Issue"`添加过滤器。# #的例子

例1：完全迁移

**用户**：“我需要将优先级值从我们的项目迁移到新的组织优先级问题字段”

**行动**：遵循P1-P6阶段。发现字段，地图选项，检查权限，扫描项目，预览，执行。

例2：仅干跑

**用户**：“告诉我如果我从项目#42迁移字段会发生什么，但实际上不要这样做”

**动作**：只遵循P1-P5阶段。提交完整的试运行报告，列出每个项目。不要执行。

例3：多个字段

**用户**：“将优先级和截止日期从项目#15迁移到问题字段”

**动作**：相同的工作流程，但处理两个字段在一个单一的通过。在数据扫描期间，收集每个项目的所有映射字段的值。在每个问题的单个API调用中写入所有字段值。

例4：单个标签到发布字段**用户**：“我想将‘bug’标签迁移到类型问题字段”

**动作**:Route to Label Migration Flow。查询org/repo，列出标签，确认映射：标签“bug”→类型字段“bug”选项。用那个标签扫描问题，预览，执行。询问迁移后是否需要拆除标签。

示例5：多个标签到一个字段（散装）

**用户**：“我们有p0， p1, p2， p3标签，并希望将它们转换为优先级问题字段”

**动作**:Route to Label Migration Flow。将所有四个标签映射到优先级字段选项（p0→p0, p1→p1, p2→p2, p3→p3）。检查冲突（多个优先级标签的问题）。在一个摘要中预览所有更改。一次执行。可选地从迁移问题中删除所有四个标签。

示例6：带标签移除的跨repo标签迁移**用户**：“将‘前端’和‘后端’标签迁移到跨github/issues，github/memex和github/mobile的团队问题字段，然后删除旧标签”

**动作**:Route to Label Migration Flow。确认repos和标签映射：“前端”→团队“前端”，“后端”→团队“后端”。扫描所有三个仓库，查找这些标签的问题。检测冲突（两个标签的问题）。跨仓库预览。执行字段写操作，然后从迁移的问题中删除标签。报告每个回购的统计信息。