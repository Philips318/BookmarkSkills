---
name: arize-annotation
description: Creates and manages annotation configs (categorical, continuous, freeform label schemas) and annotation queues (human review workflows) on Arize. Applies human annotations to project spans via the Python SDK. Use when the user mentions annotation config, annotation queue, label schema, human feedback, bulk annotate spans, update_annotations, labeling queue, annotate record, or human review.
metadata:
  author: arize
  version: "1.0"
compatibility: Requires the ax CLI and a configured Arize profile.
---
#掌握注释技能

> **`SPACE`** -所有`--space`标志和`ARIZE_SPACE`env变量接受一个空间**名称**（例如，`my-workspace`）或一个base64空间**ID**（例如，`U3BhY2U6...`）。用`ax spaces list`找到你的。

该技能涵盖**注释配置**（标签模式）和**注释队列**（人工审核工作流），以及通过Python SDK以编程方式注释项目跨度。

**方向：**在Arize人工标签附加由配置定义的值到**跨度**，**数据集示例**，**实验相关的记录**，和**队列项**在产品UI。该技能包括：`ax annotation-configs`、`ax annotation-queues`以及使用`ArizeClient.spans.update_annotations`进行大跨度更新。

---

# #先决条件

直接执行任务—运行所需的`ax`命令。不要预先检查版本、环境变量或配置文件。当执行`ax`命令失败时，请根据提示信息进行处理。
-`command not found`或版本错误→参见references/ax-setup.md-`401 Unauthorized`/缺少API密钥→运行`ax profiles show`检查当前配置文件。如果配置文件丢失或API密钥错误，请按照references/ax-profiles.md到create/update。如果用户没有他们的密钥，将他们引导到https://app.arize.com/admin> API Keys
-空间未知→运行`ax spaces list`按名称选择，或询问用户
- **安全：**永远不要读取`.env`文件或搜索文件系统的凭据。使用`ax profiles`作为alize凭证，使用`ax ai-integrations`作为LLM提供程序密钥。如果凭据无法通过这些渠道获得，请询问用户。

---

# #的概念

什么是注释配置？

注释配置**为单一类型的人工反馈标签定义模式。在任何人可以对跨度、数据集记录、实验输出或队列项进行注释之前，必须在空间中存在该标签的配置。|字段|描述||-------|-------------|
|描述标识符（如`Correctness`，`Helpfulness`）。在空间中必须是唯一的。|
| **输入** |`categorical`（从列表中选择），`continuous`（数字范围）或`freeform`（自由文本）。|
| **值** |对于分类：数组的`{"label": str, "score": number}`对。|
| **Min/Max分数** |连续：数值界限。|
| **优化方向** |分数越高越好（`maximize`）还是越差（`minimize`）。用于在UI中呈现趋势。|

标签应用的位置（表面）

|表面|典型路径||---------|----------------|
| **项目跨越** | Python SDK`spans.update_annotations`（如下）and/or的Arize UI |
| **数据集示例** | Arize UI（人工标记流）；配置必须在|空间中存在
| **实验输出** |通常在UI中与数据集或轨迹一起进行审查-参见ize- Experiment， ize-dataset |
| **注释队列项** |`ax annotation-queues`CLI（下图）and/orArize UI；配置必须存在|

在期望标签持久化之前，始终确保在空格中存在相关的注释配置。

---

基本CRUD：注释配置

# # #列表```bash
ax annotation-configs list --space SPACE
ax annotation-configs list --space SPACE -o json
ax annotation-configs list --space SPACE --limit 20
```
###创建-分类

分类配置提供了一组固定的标签供审阅者选择。```bash
ax annotation-configs create \
  --name "Correctness" \
  --space SPACE \
  --type categorical \
  --value correct \
  --value incorrect \
  --optimization-direction maximize
```
常见的二进制标签对：
—`correct`/`incorrect`-`helpful`/`unhelpful`-`safe`/`unsafe`-`relevant`/`irrelevant`-`pass`/`fail`###创建-连续

连续配置允许审阅者在定义的范围内输入数字分数。```bash
ax annotation-configs create \
  --name "Quality Score" \
  --space SPACE \
  --type continuous \
  --min-score 0 \
  --max-score 10 \
  --optimization-direction maximize
```
###创建-自由格式

自由格式配置收集开放式文本反馈。除了名称、空间和类型之外，不需要其他标志。```bash
ax annotation-configs create \
  --name "Reviewer Notes" \
  --space SPACE \
  --type freeform
```
# # #得到```bash
ax annotation-configs get NAME_OR_ID
ax annotation-configs get NAME_OR_ID -o json
ax annotation-configs get NAME_OR_ID --space SPACE   # required when using name instead of ID
```
# # #删除```bash
ax annotation-configs delete NAME_OR_ID
ax annotation-configs delete NAME_OR_ID --space SPACE   # required when using name instead of ID
ax annotation-configs delete NAME_OR_ID --force   # skip confirmation
```
**注：**删除不可逆。与此配置的任何注释队列关联也会在产品中删除（队列可能保留；如果需要，可以在Arize UI中修复关联）。

---

##注释队列：`ax annotation-queues`注释队列将记录（跨度、数据集示例、实验运行）路由给人工审阅者。每个队列都链接到一个或多个注释配置，这些注释配置定义了审查者可以应用哪些标签。

###列表/获取```bash
ax annotation-queues list --space SPACE
ax annotation-queues list --space SPACE -o json

ax annotation-queues get NAME_OR_ID --space SPACE
ax annotation-queues get NAME_OR_ID --space SPACE -o json
```
# # #创建

至少需要一个`--annotation-config-id`。```bash
ax annotation-queues create \
  --name "Correctness Review" \
  --space SPACE \
  --annotation-config-id CONFIG_ID \
  --annotator-email reviewer@example.com \
  --instructions "Label each response as correct or incorrect." \
  --assignment-method all   # or: random
```
重复`--annotation-config-id`和`--annotator-email`以附加多个配置或检查器。

# # #更新

列表标志(`--annotation-config-id`,`--annotator-email`) **在提供时完全替换**现有值-传递所有所需值，而不仅仅是新值。```bash
ax annotation-queues update NAME_OR_ID --space SPACE --name "New Name"
ax annotation-queues update NAME_OR_ID --space SPACE --instructions "Updated instructions"
ax annotation-queues update NAME_OR_ID --space SPACE \
  --annotation-config-id CONFIG_ID_A \
  --annotation-config-id CONFIG_ID_B
```
# # #删除```bash
ax annotation-queues delete NAME_OR_ID --space SPACE
ax annotation-queues delete NAME_OR_ID --space SPACE --force   # skip confirmation
```
###列表记录```bash
ax annotation-queues list-records NAME_OR_ID --space SPACE
ax annotation-queues list-records NAME_OR_ID --space SPACE --limit 50 -o json
```
为记录提交注释

注释通过配置名称来替换，每个注释配置调用一次。至少提供`--score`、`--label`或`--text`中的一个。```bash
ax annotation-queues annotate-record NAME_OR_ID RECORD_ID \
  --annotation-name "Correctness" \
  --label "correct" \
  --space SPACE

ax annotation-queues annotate-record NAME_OR_ID RECORD_ID \
  --annotation-name "Quality Score" \
  --score 8.5 \
  --text "Response was accurate but slightly verbose." \
  --space SPACE
```
###分配记录

指定用户审查特定的记录；```bash
ax annotation-queues assign-record NAME_OR_ID RECORD_ID --space SPACE
```
###删除记录```bash
ax annotation-queues delete-records NAME_OR_ID --space SPACE
```
---

##为span应用注解（Python SDK）

当你已经有了标签（例如，从评审导出或外部标签工具）时，使用Python SDK批量应用注释到项目范围。```python
import pandas as pd
from arize import ArizeClient

import os

client = ArizeClient(api_key=os.environ["ARIZE_API_KEY"])

# Build a DataFrame with annotation columns
# Required: context.span_id + at least one annotation.<name>.label or annotation.<name>.score
annotations_df = pd.DataFrame([
    {
        "context.span_id": "span_001",
        "annotation.Correctness.label": "correct",
        "annotation.Correctness.updated_by": "reviewer@example.com",
    },
    {
        "context.span_id": "span_002",
        "annotation.Correctness.label": "incorrect",
        "annotation.Correctness.updated_by": "reviewer@example.com",
    },
])

response = client.spans.update_annotations(
    space_id=os.environ["ARIZE_SPACE"],
    project_name="your-project",
    dataframe=annotations_df,
    validate=True,
)
```
**DataFrame列模式：**

|字段|必选|描述||--------|----------|-------------|
|`context.span_id`| yes | |需要标注的跨度
|`annotation.<name>.label`| |之一分类或自由标签|
|`annotation.<name>.score`| |的一个数字分数|
|`annotation.<name>.updated_by`| no |注释器标识符（电子邮件或姓名）|
|`annotation.<name>.updated_at`| no |从epoch |开始的时间戳，以毫秒为单位
|`annotation.notes`| no |自由形式的音符在跨度|

**限制：**注释仅适用于提交前31天内的跨度。

---

# #故障排除

|解决方案||---------|----------|
|`ax: command not found`|参见references/ax-setup.md|
|`401 Unauthorized`| API密钥可能无法访问此空间。在https://app.arize.com/admin> API Keys |处验证
|`Annotation config not found`|`ax annotation-configs list --space SPACE`（或使用`ax annotation-configs get NAME_OR_ID --space SPACE`） |
|`409 Conflict on create`|名称在空间中已经存在。使用不同的名称或获取现有的配置ID。|
|没有找到|`ax annotation-queues list --space SPACE`；验证队列名称或ID |
|记录没有出现在队列|确保链接到队列的注释配置存在；检查`ax annotation-configs list --space SPACE`|
|确认`project_name`、`space_id`和Span id；使用ize-trace导出跨|

---

相关技能—** ize-trace**：导出跨度以查找跨度id和时间范围
—** ize-dataset**：查找数据集id和样例id
- ** alize -evaluator**：自动法学硕士作为法官与人类注释
- ** ize-experiment**：与数据集和评估工作流相关的实验
- ** ize-link**：到alize UI中的注释配置和队列的深度链接

---

##保存凭据以备将来使用

参见references/ax-profiles.md§保存凭据以备将来使用。