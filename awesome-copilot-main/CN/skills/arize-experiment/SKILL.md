---
name: arize-experiment
description: Creates, runs, and analyzes Arize experiments for evaluating and comparing model performance. Covers experiment CRUD, exporting runs, comparing results, and evaluation workflows using the ax CLI. Use when the user mentions create experiment, run experiment, compare models, model performance, evaluate AI, experiment results, benchmark, A/B test models, or measure accuracy.
metadata:
  author: arize
  version: "1.0"
compatibility: Requires the ax CLI and a configured Arize profile.
---
#掌握实验技能

> **`SPACE`** -所有`--space`标志和`ARIZE_SPACE`env变量接受一个空间**名称**（例如，`my-workspace`）或一个base64空间**ID**（例如，`U3BhY2U6...`）。用`ax spaces list`找到你的。

# #的概念

- **实验** =针对特定数据集版本运行的命名评估，每个示例包含一次运行
- **实验运行** =处理一个数据集示例的结果——包括模型输出、可选评估和可选元数据
- **Dataset** =一个版本的示例集合；每个实验都与一个数据集和一个特定的数据集版本相关联
- **评价** =一个命名的指标附加到运行（例如，`correctness`,`relevance`），可选的标签，分数，和解释

典型的流程：导出数据集→处理每个示例→收集输出和评估→创建运行的实验。

# #先决条件直接执行任务—运行所需的`ax`命令。不要预先检查版本、环境变量或配置文件。如果执行`ax`命令失败，请根据提示信息进行处理。
-`command not found`或版本错误→参见references/ax-setup.md-`401 Unauthorized`/缺少API密钥→运行`ax profiles show`检查当前配置文件。如果配置文件丢失或API密钥错误，请按照references/ax-profiles.md到create/update。如果用户没有他们的密钥，将他们引导到https://app.arize.com/admin> API Keys
-空间未知→运行`ax spaces list`按名称选择，或询问用户
-项目不明确→询问用户，或运行`ax projects list -o json --limit 100`并呈现为可选选项
- **安全：**永远不要读取`.env`文件或搜索文件系统的凭据。使用`ax profiles`作为alize凭证，使用`ax ai-integrations`作为LLM提供程序密钥。如果凭据无法通过这些渠道获得，请询问用户。
- **关键-绝不捏造输出：**运行实验时，您必须调用用户指定的每个数据集示例的真实模型API。永远不要捏造、模拟或硬编码建模输出、延迟或评估分数。如果无法调用API（缺少SDK、缺少凭据、网络错误），请停止并在继续之前告诉用户需要什么。实验列表：`ax experiments list`浏览实验，可选择通过数据集过滤。输出到标准输出。```bash
ax experiments list
ax experiments list --dataset DATASET_NAME --space SPACE --limit 20   # DATASET_NAME: name or ID (name preferred)
ax experiments list --cursor CURSOR_TOKEN
ax experiments list -o json
```
# # #旗帜

|标志位|类型|默认值|描述||------|------|---------|-------------|
|`--dataset`| string |无|按数据集|过滤
|`--limit, -l`| int | 15 |最大结果（1-100）|
|`--cursor`| string | none |先前响应|的分页游标
|`-o, --output`| string | table |输出格式：table、json、csv、parquet或文件路径|
|`-p, --profile`| string |默认|配置文件|

##获取实验：`ax experiments get`快速元数据查找——返回实验名称、链接的dataset/version和时间戳。```bash
ax experiments get NAME_OR_ID
ax experiments get NAME_OR_ID -o json
ax experiments get NAME_OR_ID --dataset DATASET_NAME --space SPACE   # required when using experiment name instead of ID
```
# # #旗帜

|标志位|类型|默认值|描述||------|------|---------|-------------|
|`NAME_OR_ID`| string | required |实验名称或ID（位置）|
|`--dataset`| string |无|数据集名称或ID（如果使用实验名称而不是ID需要）|
|`--space`| string |无|空间名称或ID（如果使用数据集名称而不是ID需要）|
|`-o, --output`| string | table |输出格式|
|`-p, --profile`| string |默认|配置文件|

###响应字段

|字段|类型|描述||-------|------|-------------|
|`id`| string |实验ID |
|`name`| string |实验名称|
|`dataset_id`| string |链接数据集ID |
|`dataset_version_id`| string |指定使用的数据集版本|
|`experiment_traces_project_id`| string |实验轨迹存储项目|
|`created_at`| datetime |实验创建时间|
|`updated_at`| datetime |最后修改时间|

##导出实验：`ax experiments export`将所有运行下载到一个文件。默认使用REST API；通过`--all`使用Arrow Flight进行批量转移。```bash
# EXPERIMENT_NAME, DATASET_NAME: name or ID (name preferred)
ax experiments export EXPERIMENT_NAME --dataset DATASET_NAME --space SPACE
# -> experiment_abc123_20260305_141500/runs.json

ax experiments export EXPERIMENT_NAME --dataset DATASET_NAME --space SPACE --all
ax experiments export EXPERIMENT_NAME --dataset DATASET_NAME --space SPACE --output-dir ./results
ax experiments export EXPERIMENT_NAME --dataset DATASET_NAME --space SPACE --stdout
ax experiments export EXPERIMENT_NAME --dataset DATASET_NAME --space SPACE --stdout | jq '.[0]'
```
# # #旗帜

|标志位|类型|默认值|描述||------|------|---------|-------------|
|`NAME_OR_ID`| string | required |实验名称或ID（位置）|
|`--dataset`| string |无|数据集名称或ID（如果使用实验名称而不是ID需要）|
|`--space`| string |无|空间名称或ID（如果使用数据集名称而不是ID需要）|
|`--all`| bool | false |使用Arrow Flight进行批量出口（见下文）|
|`--output-dir`| string |`.`|输出目录|
|`--stdout`| bool | false |打印JSON到stdout而不是文件|
|`-p, --profile`| string |默认|配置文件|

### REST与飞行（`--all`）

- **REST**（默认）：低摩擦-没有Arrow/Flight依赖，标准HTTPS端口，通过任何公司代理或防火墙工作。每页限制500次。
- **飞行** (`--all`)：需要500次以上的实验。在单独的host/port（`flight.arize.com:443`）上使用gRPC+TLS，某些公司网络可能会阻止。**代理自动升级规则：**如果REST导出返回正好500次运行，结果可能被截断。重新运行`--all`以获得完整的数据集。

输出是一个JSON数组的运行对象：```json
[
  {
    "id": "run_001",
    "example_id": "ex_001",
    "output": "The answer is 4.",
    "evaluations": {
      "correctness": { "label": "correct", "score": 1.0 },
      "relevance": { "score": 0.95, "explanation": "Directly answers the question" }
    },
    "metadata": { "model": "gpt-4o", "latency_ms": 1234 }
  }
]
```
##创建实验：`ax experiments create`创建一个从数据文件运行的新实验。```bash
ax experiments create --name "gpt-4o-baseline" --dataset DATASET_NAME --space SPACE --file runs.json
ax experiments create --name "claude-test" --dataset DATASET_NAME --space SPACE --file runs.csv
```
# # #旗帜

|标志|类型|必选|描述||------|------|----------|-------------|
|`--name, -n`| string |是|实验名称|
|`--dataset`| string | yes |针对|运行实验的数据集
|`--space, -s`| string | no |空间名称或ID（如果使用数据集名称而不是ID则需要）|
|`--file, -f`| path | yes |数据文件类型：CSV、JSON、JSONL、Parquet |
|`-o, --output`| string | no |输出格式|
|`-p, --profile`| string |否|配置文件|

通过stdin传递数据

使用`--file -`直接管道数据-不需要临时文件：```bash
echo '[{"example_id": "ex_001", "output": "Paris"}]' | ax experiments create --name "my-experiment" --dataset DATASET_NAME --space SPACE --file -

# Or with a heredoc
ax experiments create --name "my-experiment" --dataset DATASET_NAME --space SPACE --file - << 'EOF'
[{"example_id": "ex_001", "output": "Paris"}]
EOF
```
运行文件中必需的列

|字段|类型|必选|描述||--------|------|----------|-------------|
|`example_id`| string | yes |本次运行的数据集示例的ID对应|
|`output`| string | yes |本例中的model/system输出为|

其他列在运行时作为`additionalProperties`传递。

##删除实验：`ax experiments delete````bash
ax experiments delete NAME_OR_ID
ax experiments delete NAME_OR_ID --dataset DATASET_NAME --space SPACE   # required when using experiment name instead of ID
ax experiments delete NAME_OR_ID --force   # skip confirmation prompt
```
# # #旗帜

|标志位|类型|默认值|描述||------|------|---------|-------------|
|`NAME_OR_ID`| string | required |实验名称或ID（位置）|
|`--dataset`| string |无|数据集名称或ID（如果使用实验名称而不是ID需要）|
|`--space`| string |无|空间名称或ID（如果使用数据集名称而不是ID需要）|
|`--force, -f`| bool | false |跳过确认提示符|
|`-p, --profile`| string |默认|配置文件|

##实验运行模式

每次运行对应一个数据集示例：```json
{
  "example_id": "required -- links to dataset example",
  "output": "required -- the model/system output for this example",
  "evaluations": {
    "metric_name": {
      "label": "optional string label (e.g., 'correct', 'incorrect')",
      "score": "optional numeric score (e.g., 0.95)",
      "explanation": "optional freeform text"
    }
  },
  "metadata": {
    "model": "gpt-4o",
    "temperature": 0.7,
    "latency_ms": 1234
  }
}
```
评估字段

|字段|类型|必选|描述||-------|------|----------|-------------|
|`label`| string | no |类别分类（如`correct`、`incorrect`、`partial`） |
|`score`| number | no |数字质量评分（例如，0.0 - 1.0）|
|`explanation`| string | no |求值|的自由形式推理

每次求值应该至少出现`label`、`score`或`explanation`中的一个。

# #工作流程

###对数据集运行一个实验

1. 查找或创建数据集：   ```bash
   ax datasets list --space SPACE
   ax datasets export DATASET_NAME --space SPACE --stdout | jq 'length'
   ```
2. 导出数据集示例：   ```bash
   ax datasets export DATASET_NAME --space SPACE
   ```
3. 为每个示例调用实际的模型API并收集输出。使用`ax datasets export --stdout`将示例直接管道到推理脚本中：   ```bash
   ax datasets export DATASET_NAME --space SPACE --stdout | python3 infer.py > runs.json
   ```
写入`infer.py`以从标准输入读取示例，调用目标模型，并将运行的JSON写入标准输出。下面的脚本是一个模板-首先检查导出的数据集JSON以找到正确的输入字段名，然后取消用户想要的提供者块的注释：   ```python
   import json, sys, time

   examples = json.load(sys.stdin)
   runs = []

   for ex in examples:
       # Inspect the exported JSON to find the right field (e.g. "input", "question", "prompt")
       user_input = ex.get("input") or ex.get("question") or ex.get("prompt") or str(ex)

       start = time.time()

       # === CALL THE REAL MODEL API HERE — never fabricate or simulate ===
       # Uncomment and adapt the provider block the user requested:
       #
       # OpenAI (pip install openai  — uses OPENAI_API_KEY env var):
       #   from openai import OpenAI
       #   resp = OpenAI().chat.completions.create(
       #       model="gpt-4o",
       #       messages=[{"role": "user", "content": user_input}]
       #   )
       #   output_text = resp.choices[0].message.content
       #
       # Anthropic (pip install anthropic  — uses ANTHROPIC_API_KEY env var):
       #   import anthropic
       #   resp = anthropic.Anthropic().messages.create(
       #       model="claude-sonnet-4-6", max_tokens=1024,
       #       messages=[{"role": "user", "content": user_input}]
       #   )
       #   output_text = resp.content[0].text
       #
       # Google Gemini (pip install google-genai  — uses GOOGLE_API_KEY env var):
       #   from google import genai
       #   resp = genai.Client().models.generate_content(
       #       model="gemini-2.5-pro", contents=user_input
       #   )
       #   output_text = resp.text
       #
       # Custom / OpenAI-compatible proxy (pip install openai — uses CUSTOM_BASE_URL + CUSTOM_API_KEY env vars):
       # Use this for Azure OpenAI, NVIDIA NIM, local Ollama, or any OpenAI-compatible endpoint,
       # including a test integration proxy. Matches the `custom` provider in `ax ai-integrations create`.
       #   import os
       #   from openai import OpenAI
       #   resp = OpenAI(
       #       base_url=os.environ["CUSTOM_BASE_URL"],          # e.g. https://my-proxy.example.com/v1
       #       api_key=os.environ.get("CUSTOM_API_KEY", "none"),
       #   ).chat.completions.create(
       #       model=os.environ.get("CUSTOM_MODEL", "default"),
       #       messages=[{"role": "user", "content": user_input}]
       #   )
       #   output_text = resp.choices[0].message.content

       latency_ms = round((time.time() - start) * 1000)
       runs.append({
           "example_id": ex["id"],
           "output": output_text,
           "metadata": {"model": "MODEL_NAME", "latency_ms": latency_ms}
       })
       print(f"  {ex['id']}: {latency_ms}ms", file=sys.stderr)

   json.dump(runs, sys.stdout, indent=2)
   ```
**运行前：**安装提供程序SDK (`pip install openai`/`anthropic`/`google-genai`)，并确保API密钥设置为shell中的环境变量。如果无法访问API，请停止并告诉用户需要什么。

4. 验证运行文件：   ```bash
   python3 -c "import json; runs=json.load(open('runs.json')); print(f'{len(runs)} runs'); print(json.dumps(runs[0], indent=2))"
   ```
每次运行必须有`example_id`和`output`。可选字段：`evaluations`，`metadata`。
5. 创建实验：   ```bash
   ax experiments create --name "gpt-4o-baseline" --dataset DATASET_NAME --space SPACE --file runs.json
   ```
6. 验证:`ax experiments get "gpt-4o-baseline" --dataset DATASET_NAME --space SPACE`比较两个实验

1. 导出两个实验：   ```bash
   ax experiments export "experiment-a" --dataset DATASET_NAME --space SPACE --stdout > a.json
   ax experiments export "experiment-b" --dataset DATASET_NAME --space SPACE --stdout > b.json
   ```
2. 用`example_id`比较评价分数：   ```bash
   # Average correctness score for experiment A
   jq '[.[] | .evaluations.correctness.score] | add / length' a.json

   # Same for experiment B
   jq '[.[] | .evaluations.correctness.score] | add / length' b.json
   ```
3. 找出结果不同的例子：   ```bash
   jq -s '.[0] as $a | .[1][] | . as $run |
     {
       example_id: $run.example_id,
       b_score: $run.evaluations.correctness.score,
       a_score: ($a[] | select(.example_id == $run.example_id) | .evaluations.correctness.score)
     }' a.json b.json
   ```
4. 每个评估者的分数分布（pass/fail/partial计数）：   ```bash
   # Count by label for experiment A
   jq '[.[] | .evaluations.correctness.label] | group_by(.) | map({label: .[0], count: length})' a.json
   ```
5. 找到回归（在A中通过但在B中失败的例子）：   ```bash
   jq -s '
     [.[0][] | select(.evaluations.correctness.label == "correct")] as $passed_a |
     [.[1][] | select(.evaluations.correctness.label != "correct") |
       select(.example_id as $id | $passed_a | any(.example_id == $id))
     ]
   ' a.json b.json
   ```
**统计显著性说明：**评分比较最可靠，每个评估者≥30个样本。在样本较少的情况下，只将delta视为方向- n=10时5%的差异可能是噪声。报告样本大小和分数：`jq 'length' a.json`。

下载实验结果进行分析

1.`ax experiments list --dataset DATASET_NAME --space SPACE`——找到实验
2.`ax experiments export EXPERIMENT_NAME --dataset DATASET_NAME --space SPACE`——下载到文件
3. 解析:`jq '.[] | {example_id, score: .evaluations.correctness.score}' experiment_*/runs.json`管道导出到其他工具```bash
# Count runs
ax experiments export EXPERIMENT_NAME --dataset DATASET_NAME --space SPACE --stdout | jq 'length'

# Extract all outputs
ax experiments export EXPERIMENT_NAME --dataset DATASET_NAME --space SPACE --stdout | jq '.[].output'

# Get runs with low scores
ax experiments export EXPERIMENT_NAME --dataset DATASET_NAME --space SPACE --stdout | jq '[.[] | select(.evaluations.correctness.score < 0.5)]'

# Convert to CSV
ax experiments export EXPERIMENT_NAME --dataset DATASET_NAME --space SPACE --stdout | jq -r '.[] | [.example_id, .output, .evaluations.correctness.score] | @csv'
```
相关技能

- ** alize -dataset**：创建或导出此实验运行的数据集→先使用`arize-dataset`- ** ize-prompt-optimization**：利用实验结果改进提示→下一步是`arize-prompt-optimization`- ** size -trace**：检查单个跨度跟踪失败的实验运行→使用`arize-trace`- ** ize-link**：生成可点击的UI链接，以跟踪从实验运行→使用`arize-link`# #故障排除

|解决方案||---------|----------|
|`ax: command not found`|参见references/ax-setup.md|
|`401 Unauthorized`| API密钥错误，过期或无法访问此空间。使用references/ax-profiles.md.|修复配置文件
|`No profile found`|未配置profile。请参见references/ax-profiles.md创建一个。|
|`Experiment not found`|用`ax experiments list --space SPACE`|验证实验名称
|`Invalid runs file`|每次运行必须有`example_id`和`output`字段|
|`example_id mismatch`|确保`example_id`值与数据集中的id匹配（导出数据集进行验证）|
|`No runs found`|导出返回空-验证实验已通过`ax experiments get`|运行
|`Dataset not found`|链接数据集可能已被删除；检查`ax datasets list`|

##保存凭据以备将来使用

参见references/ax-profiles.md§保存凭据以备将来使用。