---
name: arize-dataset
description: Creates, manages, and queries Arize datasets and examples. Covers dataset CRUD, appending examples, exporting data, and file-based dataset creation using the ax CLI. Use when the user needs test data, evaluation examples, or mentions create dataset, list datasets, export dataset, append examples, dataset version, golden dataset, or test set.
metadata:
  author: arize
  version: "1.0"
compatibility: Requires the ax CLI and a configured Arize profile.
---
# Dataset技能

> **`SPACE`** -所有`--space`标志和`ARIZE_SPACE`env变量接受一个空间**名称**（例如，`my-workspace`）或一个base64空间**ID**（例如，`U3BhY2U6...`）。用`ax spaces list`找到你的。

# #的概念

- **Dataset** =用于评估和实验的示例的版本集合
—**数据集版本** =数据集在某个时间点的快照；更新可以就地进行，也可以创建新版本
- **示例** =数据集中具有任意用户自定义字段的单个记录（例如，`question`,`answer`,`context`）
- **空间** =组织容器；数据集属于一个空间

示例中的系统管理字段（`id`、`created_at`、`updated_at`）是由服务器自动生成的——永远不要在创建或追加有效负载中包含它们。

# #先决条件

直接执行任务—运行所需的`ax`命令。不要预先检查版本、环境变量或配置文件。如果执行`ax`命令失败，请根据提示信息进行处理：
-`command not found`或版本错误→参见references/ax-setup.md-`401 Unauthorized`/缺少API密钥→运行`ax profiles show`检查当前配置文件。如果配置文件丢失或API密钥错误，请按照references/ax-profiles.md到create/update。如果用户没有他们的密钥，将他们引导到https://app.arize.com/admin> API Keys
-空间未知→运行`ax spaces list`按名称选择，或询问用户
-项目不清楚→询问用户，或运行`ax projects list -o json --limit 100`并呈现为可选选项
- **安全：**永远不要读取`.env`文件或搜索文件系统的凭据。使用`ax profiles`作为alize凭证，使用`ax ai-integrations`作为LLM提供程序密钥。如果凭据无法通过这些渠道获得，请询问用户。

##列表数据集：`ax datasets list`浏览空间中的数据集。输出到标准输出。```bash
ax datasets list
ax datasets list --space SPACE --limit 20
ax datasets list --cursor CURSOR_TOKEN
ax datasets list -o json
```
# # #旗帜

|标志位|类型|默认值|描述||------|------|---------|-------------|
|`--space`| string |从配置文件|通过空格|过滤
|`--limit, -l`| int | 15 |最大结果（1-100）|
|`--cursor`| string | none |先前响应|的分页游标
|`-o, --output`| string | table |输出格式：table、json、csv、parquet或文件路径|
|`-p, --profile`| string |默认|配置文件|

##获取数据集：`ax datasets get`快速元数据查找——返回数据集名称、空间、时间戳和版本列表。```bash
ax datasets get NAME_OR_ID
ax datasets get NAME_OR_ID -o json
ax datasets get NAME_OR_ID --space SPACE   # required when using dataset name instead of ID
```
# # #旗帜

|标志位|类型|默认值|描述||------|------|---------|-------------|
|`NAME_OR_ID`| string | required |数据集名称或ID(位置
|`--space`| string |无|空间名称或ID（如果使用数据集名称而不是ID需要）|
|`-o, --output`| string | table |输出格式|
|`-p, --profile`| string |默认|配置文件|

###响应字段

|字段|类型|描述||-------|------|-------------|
|`id`| string |数据集ID |
|`name`| string |数据集名称|
|`space_id`| string |空间该数据集属于|
|`created_at`| datetime |数据集创建时间|
|`updated_at`| datetime |最后修改时间|
|`versions`| array |数据集版本列表（id, name, dataset_id, created_at, updated_at） |

##导出数据集：`ax datasets export`将所有示例下载到一个文件中。对于大于500个示例的数据集使用`--all`（无限制批量导出）。```bash
ax datasets export NAME_OR_ID
# -> dataset_abc123_20260305_141500/examples.json

ax datasets export NAME_OR_ID --all
ax datasets export NAME_OR_ID --version-id VERSION_ID
ax datasets export NAME_OR_ID --output-dir ./data
ax datasets export NAME_OR_ID --stdout
ax datasets export NAME_OR_ID --stdout | jq '.[0]'
ax datasets export NAME_OR_ID --space SPACE   # required when using dataset name instead of ID
```
# # #旗帜

|标志位|类型|默认值|描述||------|------|---------|-------------|
|`NAME_OR_ID`| string | required |数据集名称或ID(位置
|`--space`| string |无|空间名称或ID（如果使用数据集名称而不是ID需要）|
|`--version-id`| string |最新|导出指定数据集版本|
|`--all`| bool | false |无限制批量导出（用于数据集> 500个示例）|
|`--output-dir`| string |`.`|输出目录|
|`--stdout`| bool | false |打印JSON到stdout而不是文件|
|`-p, --profile`| string |默认|配置文件|

**代理自动升级规则：**如果导出返回恰好500个示例，则结果可能被截断-使用`--all`重新运行以获得完整的数据集。

**导出完整性验证：**导出后，确认行数与服务器报告的相符：```bash
# Get the server-reported count from dataset metadata
ax datasets get DATASET_NAME --space SPACE -o json | jq '.versions[-1] | {version: .id, examples: .example_count}'

# Compare to what was exported
jq 'length' dataset_*/examples.json

# If counts differ, re-export with --all
```
输出是一个示例对象的JSON数组。每个示例都有系统字段（`id`,`created_at`,`updated_at`）以及所有用户定义字段：```json
[
  {
    "id": "ex_001",
    "created_at": "2026-01-15T10:00:00Z",
    "updated_at": "2026-01-15T10:00:00Z",
    "question": "What is 2+2?",
    "answer": "4",
    "topic": "math"
  }
]
```
创建数据集：`ax datasets create`从数据文件创建新数据集。```bash
ax datasets create --name "My Dataset" --space SPACE --file data.csv
ax datasets create --name "My Dataset" --space SPACE --file data.json
ax datasets create --name "My Dataset" --space SPACE --file data.jsonl
ax datasets create --name "My Dataset" --space SPACE --file data.parquet
```
# # #旗帜

|标志|类型|必选|描述||------|------|----------|-------------|
|`--name, -n`| string |是|数据集名称|
|`--space`| string | yes | |中创建数据集的空间
|`--file, -f`|路径|是|数据文件：CSV、JSON、JSONL或Parquet |
|`-o, --output`| string | no |返回的数据集元数据输出格式|
|`-p, --profile`| string | no |配置文件|

通过stdin传递数据

使用`--file -`直接管道数据-不需要临时文件：```bash
echo '[{"question": "What is 2+2?", "answer": "4"}]' | ax datasets create --name "my-dataset" --space SPACE --file -

# Or with a heredoc
ax datasets create --name "my-dataset" --space SPACE --file - << 'EOF'
[{"question": "What is 2+2?", "answer": "4"}]
EOF
```
要向现有数据集添加行，请使用`ax datasets append --json '[...]'`—不需要文件。

支持的文件格式

|格式|扩展|注释||--------|-----------|-------|
| CSV |`.csv`|列头变为字段名|
| JSON |`.json`|对象数组|
| JSON行|`.jsonl`|每行一个对象（不是JSON数组）|
| Parquet |`.parquet`|列名变成字段名；保留|类型

* *格式问题:* *
—**CSV**：丢失类型信息—日期变成字符串，`null`变成空字符串。使用JSON/Parquet保存类型。
—**JSONL**：每行是一个单独的JSON对象。`.jsonl`文件中的JSON数组（`[{...}, {...}]`）将失败-使用`.json`扩展名代替。
—**Parquet**：保留列类型。要求`pandas`/`pyarrow`本地读取：`pd.read_parquet("examples.parquet")`。

示例：`ax datasets append`向现有数据集添加示例。两种输入模式-使用适合的。

内联JSON（代理友好）

直接生成有效负载——不需要临时文件：```bash
ax datasets append DATASET_NAME --space SPACE --json '[{"question": "What is 2+2?", "answer": "4"}]'

ax datasets append DATASET_NAME --space SPACE --json '[
  {"question": "What is gravity?", "answer": "A fundamental force..."},
  {"question": "What is light?", "answer": "Electromagnetic radiation..."}
]'
```
###从文件```bash
ax datasets append DATASET_NAME --space SPACE --file new_examples.csv
ax datasets append DATASET_NAME --space SPACE --file additions.json
```
到特定的版本```bash
ax datasets append DATASET_NAME --space SPACE --json '[{"q": "..."}]' --version-id VERSION_ID
```
# # #旗帜

|标志|类型|必选|描述||------|------|----------|-------------|
|`NAME_OR_ID`| string | yes |数据集名称或ID（位置）；在使用名称|时添加`--space`|`--space`| string | no |空间名称或ID（如果使用数据集名称而不是ID则需要）|
|`--json`| string |互斥体|示例对象的JSON数组|
|`--file, -f`|路径|互斥锁|数据文件（CSV、JSON、JSONL、Parquet） | . | . |路径|互斥锁|
|`--version-id`| string | no |附加到指定版本后（默认为latest） |
|`-o, --output`| string | no |返回的数据集元数据输出格式|
|`-p, --profile`| string |否|配置文件|

只需要`--json`或`--file`中的一个。

# # #验证

—每个示例必须是一个JSON对象，至少有一个用户定义字段
—每个请求最多100,000个示例

**如果数据集已经有示例，在附加之前检查它的模式，以避免沉默字段不匹配。```bash
# Check existing field names in the dataset
ax datasets export DATASET_NAME --space SPACE --stdout | jq '.[0] | keys'

# Verify your new data has matching field names
echo '[{"question": "..."}]' | jq '.[0] | keys'

# Both outputs should show the same user-defined fields
```
字段是自由形式的：在新示例中添加额外的字段，而缺失的字段变为空。但是，字段名中的拼写错误（例如，`queston`vs`question`）会静默地创建新列——在附加之前检查拼写。

删除数据集：`ax datasets delete````bash
ax datasets delete NAME_OR_ID
ax datasets delete NAME_OR_ID --space SPACE   # required when using dataset name instead of ID
ax datasets delete NAME_OR_ID --force   # skip confirmation prompt
```
# # #旗帜

|标志位|类型|默认值|描述||------|------|---------|-------------|
|`NAME_OR_ID`| string | required |数据集名称或ID(位置
|`--space`| string |无|空间名称或ID（如果使用数据集名称而不是ID需要）|
|`--force, -f`| bool | false |跳过确认提示符|
|`-p, --profile`| string |默认|配置文件|

# #工作流程

###根据名称查找数据集

所有数据集命令都直接接受名称或ID。你可以传递一个数据集名称作为位置参数（在不使用ID时添加`--space SPACE`）：```bash
# Use name directly
ax datasets get "eval-set-v1" --space SPACE
ax datasets export "eval-set-v1" --space SPACE

# Or resolve name to ID via list if you need the base64 ID
ax datasets list -o json | jq '.[] | select(.name == "eval-set-v1") | .id'
```
###从文件中创建数据集用于评估

1. 准备一个包含评估列的CSV/JSON/Parquet文件（例如，`input`,`expected_output`）
-如果生成内联数据，通过stdin使用`--file -`管道（参见创建数据集部分）
2.`ax datasets create --name "eval-set-v1" --space SPACE --file eval_data.csv`3. 验证:`ax datasets get DATASET_NAME --space SPACE`4. 使用数据集名称来运行实验

###向现有数据集添加示例```bash
# Find the dataset
ax datasets list --space SPACE

# Append inline or from a file using the dataset name (see Append Examples section for full syntax)
ax datasets append DATASET_NAME --space SPACE --json '[{"question": "...", "answer": "..."}]'
ax datasets append DATASET_NAME --space SPACE --file additional_examples.csv
```
下载数据集用于离线分析

1.`ax datasets list --space SPACE`——查找数据集名称
2.`ax datasets export DATASET_NAME --space SPACE`——下载到文件
3. 解析JSON:`jq '.[] | .question' dataset_*/examples.json`导出指定版本```bash
# List versions
ax datasets get DATASET_NAME --space SPACE -o json | jq '.versions'

# Export that version
ax datasets export DATASET_NAME --space SPACE --version-id VERSION_ID
```
对数据集进行迭代

1. 导出当前版本：`ax datasets export DATASET_NAME --space SPACE`2. 在本地修改示例
3. 添加新行：`ax datasets append DATASET_NAME --space SPACE --file new_rows.csv`4. 或者创建一个新的版本：`ax datasets create --name "eval-set-v2" --space SPACE --file updated_data.json`管道导出到其他工具```bash
# Count examples
ax datasets export DATASET_NAME --space SPACE --stdout | jq 'length'

# Extract a single field
ax datasets export DATASET_NAME --space SPACE --stdout | jq '.[].question'

# Convert to CSV with jq
ax datasets export DATASET_NAME --space SPACE --stdout | jq -r '.[] | [.question, .answer] | @csv'
```
##数据集示例模式

示例是自由格式的JSON对象。没有固定的模式——列是您提供的任何字段。系统管理的字段由服务器添加：

|字段|类型|由|管理Notes ||-------|------|-----------|-------|
|`id`| string | server |自动生成的UUID。更新时必需，在create/append|上禁止
|`created_at`| datetime | server |不可变创建时间戳|
|`updated_at`| datetime | server |修改|自动更新
| *（任意用户字段）* |任意JSON类型|用户|字符串，数字，布尔值，空，嵌套对象，数组|


相关技能

- ** ize-trace**：导出生产跨度，以了解在数据集中放置什么数据→使用`arize-trace`- ** ize-experiment**：对这个数据集运行评估→下一步是`arize-experiment`- ** ize-prompt-optimization**：使用数据集+实验结果改进提示→使用`arize-prompt-optimization`# #故障排除

|解决方案||---------|----------|
|`ax: command not found`|参见references/ax-setup.md|
|`401 Unauthorized`| API密钥错误，过期或无法访问此空间。使用references/ax-profiles.md.|修复配置文件
|`No profile found`|未配置profile。请参见references/ax-profiles.md创建一个。|
|`Dataset not found`|用`ax datasets list`|验证数据集ID
|`File format error`|支持：CSV， JSON， JSONL, Parquet。使用`--file -`从stdin中读取。|
从create/append有效载荷中移除`id`，`created_at`,`updated_at`|
移除`time`，`count`，或任何`source_record_*`字段|
|`Provide either --json or --file`| Append只需要一个输入源|
确保JSON数组或文件至少包含一个示例|
|`not a JSON object`|`--json`数组中的每个元素必须是`{...}`对象，而不是字符串或数字

##保存凭据以备将来使用

参见references/ax-profiles.md§保存凭据以备将来使用。