---
name: arize-evaluator
description: Handles LLM-as-judge evaluation workflows on Arize including creating/updating evaluators, running evaluations on spans or experiments, managing tasks, trigger-run operations, column mapping, and continuous monitoring. Use when the user mentions create evaluator, LLM judge, hallucination, faithfulness, correctness, relevance, run eval, score spans, score experiment, trigger-run, column mapping, continuous monitoring, or improve evaluator prompt.
metadata:
  author: arize
  version: "1.0"
compatibility: Requires the ax CLI and a configured Arize profile with an AI integration.
---
#掌握评估员技能

> **`SPACE`** -所有`--space`标志和`ARIZE_SPACE`env变量接受一个空间**名称**（例如，`my-workspace`）或一个base64空间**ID**（例如，`U3BhY2U6...`）。用`ax spaces list`找到你的。

该技能涵盖设计，创建和运行**法学硕士作为法官评估**在alize。评价者定义了法官；任务是你如何根据真实数据运行它。

---

# #先决条件

直接执行任务—运行所需的`ax`命令。不要预先检查版本、环境变量或配置文件。当执行`ax`命令失败时，请根据提示信息进行处理。
-`command not found`或版本错误→参见references/ax-setup.md-`401 Unauthorized`/缺少API密钥→运行`ax profiles show`检查当前配置文件。如果配置文件丢失或API密钥错误，请按照references/ax-profiles.md到create/update。如果用户没有他们的密钥，将他们引导到https://app.arize.com/admin> API Keys
-空间未知→运行`ax spaces list`按名称选择，或询问用户
LLM提供商调用失败（缺少OPENAI_API_KEY / ANTHROPIC_API_KEY）→运行`ax ai-integrations list --space SPACE`检查平台管理的凭据。如果不存在，请要求用户提供密钥或通过alize -ai-provider-integration技能创建集成
- **安全：**永远不要读取`.env`文件或搜索文件系统的凭据。使用`ax profiles`作为alize凭证，使用`ax ai-integrations`作为LLM提供程序密钥。如果凭据无法通过这些渠道获得，请询问用户。
- **关键-绝不捏造评估结果：**如果评估任务失败，被取消或没有得分，请清楚地报告失败并解释问题所在。不要执行“手动评估”，发明质量分数，估计百分比，或呈现任何代理生成的分析，就好像它来自Arize评估系统一样。相反，建议：(1)修复已识别的问题并重试，(2)尝试从alize UI运行，(3)使用`ax ai-integrations list`验证集成凭证，(4)在https://arize.com/support联系支持---

# #的概念

什么是评估器？

“评估者”是法学硕士作为法官的定义。它包含:

|字段|描述||-------|-------------|
| **模板** |法官提示符。使用在运行时通过任务的列映射填充的`{variable}`占位符（例如`{input}`，`{output}`,`{context}`）。|
|允许输出标签的集合（例如`factual`/`hallucinated`）。二进制是默认的，也是最常见的。每个选项都可以选择带有数字分数。|
| **AI集成** |存储的LLM提供商凭证（OpenAI, Anthropic， Bedrock等），评估器使用它来调用判断模型。|
| **Model** |具体判断模型（如`gpt-4o`、`claude-sonnet-4-5`）。|
| **调用参数** |模型设置的可选JSON，如`{"temperature": 0}`。为了重现性，建议低温。|
| **优化方向** |分数越高越好（`maximize`）还是越差（`minimize`）。设置UI如何呈现趋势。|
| **数据粒度** |求值器是否运行在**span**、**trace**或**session**级别。金属氧化物半导体T求值器在跨度级别上运行。|求值器是受版本控制的——每次提示或模型更改都会创建一个新的不可变版本。最新的版本是有效的。

什么是任务？

一个任务是你如何对真实数据运行一个或多个评估器。任务附加到**项目** (livetraces/spans)或**数据集**（实验运行）。任务包含：

|字段|描述||-------|-------------|
| **Evaluators** |要运行的评估器列表。你可以在一个任务中运行多个任务。|
|将每个求值器的模板变量映射到span或实验运行的实际字段路径（例如`"input" → "attributes.input.value"`）。这就是使评估器在项目和实验中可移植的原因。|
| **查询过滤器** | sql风格的表达式来选择哪个spans/runs评估（例如`"span_kind = 'LLM'"`）。可选，但对精度很重要。|
| **连续** |对于项目任务：是否在新跨度到达时自动评分。|
| **采样率** |对于连续的项目任务：要评估的新跨度的分数（0-1）。|

---

##数据粒度`--data-granularity`标志控制评估器评分的数据单元。它默认为`span`，仅适用于**项目任务**（不是dataset/experiment任务-那些直接评估实验运行）。

|级别|评估|用于|结果列前缀||-------|-------------------|---------|---------------------|
|`span`（默认）|个人跨度|问答正确性、幻觉、相关性|`eval.{name}.label`/`.score`/`.explanation`|
|`trace_eval.{name}.label`/`.score`/`.explanation`|所有跨越在一个跟踪中，按`context.trace_id`|代理轨迹，任务正确性-任何需要完整调用链的|`trace_eval.{name}.label`/`.score`/`.explanation`|分组
|`session_eval.{name}.label`/`.score`/`.explanation`|会话中的所有痕迹，由`attributes.session.id`分组并按开始时间排序|多轮一致性，整体音调，会话质量|`session_eval.{name}.label`/`.score`/`.explanation`|

跟踪和会话聚合如何工作

对于**trace**粒度，共享相同`context.trace_id`的跨被分组在一起。在传递给判断模型之前，计算器模板使用的列值被逗号连接到单个字符串中（每个值被截断为100K个字符）。对于**会话**粒度，首先进行相同的跟踪级别分组，然后按`start_time`排序跟踪，并按`attributes.session.id`分组。会话级别的值上限为100K个字符。`{conversation}`模板变量

在会话粒度上，`{conversation}`是一个特殊的模板变量，它在会话中的所有跟踪中呈现为`{input, output}`的JSON数组，由`attributes.input.value`/`attributes.llm.input_messages`（输入端）和`attributes.output.value`/`attributes.llm.output_messages`（输出端）构建。

在跨度或跟踪粒度上，`{conversation}`被视为常规模板变量，并像其他变量一样通过列映射进行解析。

多评估器任务一个任务可以包含不同粒度的评估器。在运行时，系统使用**最高**粒度（session > trace > span）来获取数据，并自动**拆分为每个评估器**的一个子运行。任务的评估器JSON中的Per-evaluator`query_filter`进一步缩小了所包含的范围（例如，在会话中仅包含工具调用范围）。

---

基本CRUD

AI集成

AI集成存储评估器使用的LLM提供者凭证。对于完整的CRUD -列表，为所有提供商（OpenAI, Anthropic, Azure, Bedrock, Vertex, Gemini, NVIDIA NIM, custom）创建，更新和删除-使用** alize -ai-provider-integration**技能。

常见案例的快速参考（OpenAI）：```bash
# Check for an existing integration first
ax ai-integrations list --space SPACE

# Create if none exists
ax ai-integrations create \
  --name "My OpenAI Integration" \
  --provider openAI \
  --api-key $OPENAI_API_KEY
```
复制返回的集成ID—`ax evaluators create --ai-integration-id`需要这个ID。

# # #评价者```bash
# List / Get
ax evaluators list --space SPACE
ax evaluators get ID                    # accepts name or ID
ax evaluators get NAME --space SPACE   # required when using name instead of ID
ax evaluators list-versions NAME_OR_ID
ax evaluators get-version VERSION_ID

# Create (creates the evaluator and its first version)
ax evaluators create \
  --name "Answer Correctness" \
  --space SPACE \
  --description "Judges if the model answer is correct" \
  --template-name "correctness" \
  --commit-message "Initial version" \
  --ai-integration-id INT_ID \
  --model-name "gpt-4o" \
  --include-explanations \
  --use-function-calling \
  --classification-choices '{"correct": 1, "incorrect": 0}' \
  --template 'You are an evaluator. Given the user question and the model response, decide if the response correctly answers the question.

User question: {input}

Model response: {output}

Respond with exactly one of these labels: correct, incorrect'

# Create a new version (for prompt or model changes — versions are immutable)
ax evaluators create-version NAME_OR_ID \
  --commit-message "Added context grounding" \
  --template-name "correctness" \
  --ai-integration-id INT_ID \
  --model-name "gpt-4o" \
  --include-explanations \
  --classification-choices '{"correct": 1, "incorrect": 0}' \
  --template 'Updated prompt...

{input} / {output} / {context}'

# Update metadata only (name, description — not prompt)
ax evaluators update NAME_OR_ID \
  --name "New Name" \
  --description "Updated description"

# Delete (permanent — removes all versions)
ax evaluators delete NAME_OR_ID
```
**`create`的关键标志：**

|标志|必选|描述||------|----------|-------------|
|`--name`|是|评估器名称（在空间中唯一）|
|`--space`|是|要在|中创建的空间名称或ID
|`--template-name`|是| Eval列名-字母数字，空格，连字符，下划线|
|`--commit-message`| yes | |版本描述
|`--ai-integration-id`|是| AI集成ID（从上到下）|
|`--model-name`| yes |判断模型（例如`gpt-4o`） |
|`--template`|是|提示符`{variable}`占位符（在bash中单引号）|
|`--classification-choices`|是| JSON对象映射选择标签到数字分数，例如`'{"correct": 1, "incorrect": 0}'`|
|`--description`| no |人类可读的描述|
|`--include-explanations`| no |在标签|旁边包含推理
|`--use-function-calling`| no |首选结构化函数调用输出|
|`--invocation-params`| no |模型参数的JSON，例如`'{"temperature": 0}'`|
|`--data-granularity`| no |`span`（默认值）、`trace`或`session`。仅与项目任务相关，而与dataset/experiment任务无关。请参阅数据粒度部分。|
|`--direction`| no |直接优化离子：`maximize`或`minimize`。设置UI如何呈现趋势。|
|`--provider-params`| no |提供程序特定参数的JSON对象|# # #任务

>`PROJECT_NAME`、`DATASET_NAME`和`evaluator_id`都接受名称或base64 ID。```bash
# List / Get
ax tasks list --space SPACE
ax tasks list --project PROJECT_NAME
ax tasks list --dataset DATASET_NAME --space SPACE
ax tasks get TASK_ID

# Create (project — continuous)
ax tasks create \
  --name "Correctness Monitor" \
  --task-type template_evaluation \
  --project PROJECT_NAME \
  --evaluators '[{"evaluator_id": "EVAL_ID", "column_mappings": {"input": "attributes.input.value", "output": "attributes.output.value"}}]' \
  --is-continuous \
  --sampling-rate 0.1

# Create (project — one-time / backfill)
ax tasks create \
  --name "Correctness Backfill" \
  --task-type template_evaluation \
  --project PROJECT_NAME \
  --evaluators '[{"evaluator_id": "EVAL_ID", "column_mappings": {"input": "attributes.input.value", "output": "attributes.output.value"}}]' \
  --no-continuous

# Create (experiment / dataset)
ax tasks create \
  --name "Experiment Scoring" \
  --task-type template_evaluation \
  --dataset DATASET_NAME --space SPACE \
  --experiment-ids "EXP_ID_1,EXP_ID_2" \   # base64 IDs from `ax experiments list --space SPACE -o json`
  --evaluators '[{"evaluator_id": "EVAL_ID", "column_mappings": {"output": "output"}}]' \
  --no-continuous

# Trigger a run (project task — use data window)
ax tasks trigger-run TASK_ID \
  --data-start-time "2026-03-20T00:00:00" \
  --data-end-time "2026-03-21T23:59:59" \
  --wait

# Trigger a run (experiment task — use experiment IDs)
ax tasks trigger-run TASK_ID \
  --experiment-ids "EXP_ID_1" \   # base64 ID from `ax experiments list --space SPACE -o json`
  --wait

# Monitor
ax tasks list-runs TASK_ID
ax tasks get-run RUN_ID
ax tasks wait-for-run RUN_ID --timeout 300
ax tasks cancel-run RUN_ID --force
```
**触发运行时间格式：**`2026-03-21T09:00:00`-不带尾`Z`。

**其他触发运行标志：**

|标志位|描述||------|-------------|
|`--max-spans`|已处理的跨度（默认为10,000）|
|`--override-evaluations`|重新评分已经有标签|的跨度
|`--wait`/`-w`|阻塞直到运行完成|
|`--timeout`|使用`--wait`等待的时间（默认为600）|
|`--poll-interval`|等待时轮询间隔（默认5秒）|

**运行状态指南：**

|状态|含义||--------|---------|
|`completed`， 0跨度| eval索引滞后1-2小时-最近摄入的跨度可能还没有索引。将窗口切换到至少2小时前的数据，或扩大时间范围以覆盖更多的历史数据。|
|`cancelled`~1s |集成凭证无效|
|`cancelled`~3min |找到了span，但LLM调用失败-检查模型名称或密钥|
|`completed`, N > 0 |成功-在UI中检查分数|

---

工作流程A：为一个项目创建一个评估者

当用户说“为我的Playground Traces项目创建一个评估器”时，可以使用这个。

###步骤1：确认项目名称`ax spans export`直接接受项目名称—不需要查找ID。如果你不知道项目名称，列出可用的项目：```bash
ax projects list --space SPACE -o json
```
查找与`"name"`匹配的条目（不区分大小写），并在后续命令中使用该名称作为`PROJECT`。如果稍后在名称上遇到验证错误，请转而使用项目的`"id"`（base64字符串）。

步骤2：了解要评估的内容

如果用户指定了评估器类型（幻觉、正确性、相关性等）→跳到步骤3。

如果没有，选取最近的跨度，使评估者基于实际数据：```bash
ax spans export PROJECT --space SPACE -l 10 --days 30 --stdout
```
检查`attributes.input`、`attributes.output`、span类型和任何现有的注释。确定失败模式（例如，幻觉事实，偏离主题的答案，缺少上下文）并提出** 1-3个具体的评估者想法**。让用户自己选择。

每个建议必须包括：评估器名称（粗体），对其判断内容的一句话描述，以及括号中的二进制标签对。格式如下：

1. **名称** -被评判对象的描述。（`label_a`/`label_b`）

例子:
1. **回应的正确性** -座席的回应是否正确地解决了用户的财务问题？（`correct`/`incorrect`）
2. **幻觉** -反应是否捏造了没有基于检索上下文的事实？（`factual`/`hallucinated`）

步骤3：确认或创建AI集成```bash
ax ai-integrations list --space SPACE -o json
```
如果存在合适的集成，请注意其ID。如果没有，使用“实现-ai-提供者-集成”技能创建一个。询问用户他们想要哪个provider/model作为判断。

步骤4：创建评估器

使用下面的模板设计最佳实践。保持评估器名称和变量**通用** -任务（步骤6）通过`column_mappings`处理特定于项目的连接。```bash
ax evaluators create \
  --name "Hallucination" \
  --space SPACE \
  --template-name "hallucination" \
  --commit-message "Initial version" \
  --ai-integration-id INT_ID \
  --model-name "gpt-4o" \
  --include-explanations \
  --use-function-calling \
  --classification-choices '{"factual": 1, "hallucinated": 0}' \
  --template 'You are an evaluator. Given the user question and the model response, decide if the response is factual or contains unsupported claims.

User question: {input}

Model response: {output}

Respond with exactly one of these labels: hallucinated, factual'
```
###步骤5：询问—回填，连续，还是两者都有？

**建议的方法：**总是从一个小的回填（~100个历史跨度）开始，在开启连续监控之前验证评估器。这使您可以在对所有未来的生产范围进行评分之前捕获列映射错误、错误的跨度类型和已知数据上的模板问题。只有在回填确认正确得分后才启用连续。

在创建任务之前，询问：

“你想要：
> (a)对历史跨度（一次性）进行**回填** ？
> (b)对未来的新跨度进行持续评估？
> (c) ** ** -先回填验证，然后继续自动得分新跨度？(推荐)”

###步骤6：从实际的span数据确定列映射

不要猜测路径。抽取一个样本并检查实际存在的字段：```bash
ax spans export PROJECT --space SPACE -l 5 --days 7 --stdout
```
对于每个模板变量（`{input}`、`{output}`、`{context}`），找到匹配的JSON路径。常见的出发点- **总是在使用**之前验证您的实际数据：

|模板var | LLM跨度|链跨度||---|---|---|
|`input`|`attributes.input.value`|`attributes.input.value`|
|`output`|`attributes.llm.output_messages.0.message.content`|`attributes.output.value`|
|`context`|`attributes.retrieval.documents.contents`| - |
|`tool_output`|`attributes.input.value`（后退）|`attributes.output.value`|

**验证跨度类型对齐：**如果评估器提示假设LLM最终文本，但任务目标是CHAIN跨度（反之亦然），运行可能会取消或对错误的文本进行评分。确保任务上的`query_filter`与您映射的跨度类型匹配。

**`query_filter`仅适用于索引属性：**在求值器JSON中的`query_filter`是根据eval索引求值的，而不是原始跨度存储。`attributes.metadata.*`或自定义键下的属性可能不会被索引，并且将静默地不匹配任何内容。使用众所周知的索引属性，如`span_kind`或`attributes.llm.model_name`进行过滤。如果一个过滤器在存在数据的情况下返回0跨度，请尝试删除该过滤器作为诊断步骤。

**完整示例`--evaluators`JSON:**```json
[
  {
    "evaluator_id": "EVAL_ID",
    "query_filter": "span_kind = 'LLM'",
    "column_mappings": {
      "input": "attributes.input.value",
      "output": "attributes.llm.output_messages.0.message.content",
      "context": "attributes.retrieval.documents.contents"
    }
  }
]
```
为模板引用的每个变量都包含一个映射。遗漏一个会导致运行不产生有效分数。

###步骤7：创建任务

**只能回填(a):**```bash
ax tasks create \
  --name "Hallucination Backfill" \
  --task-type template_evaluation \
  --project PROJECT \
  --evaluators '[{"evaluator_id": "EVAL_ID", "column_mappings": {"input": "attributes.input.value", "output": "attributes.output.value"}}]' \
  --no-continuous
```
**仅连续(b):**```bash
ax tasks create \
  --name "Hallucination Monitor" \
  --task-type template_evaluation \
  --project PROJECT \
  --evaluators '[{"evaluator_id": "EVAL_ID", "column_mappings": {"input": "attributes.input.value", "output": "attributes.output.value"}}]' \
  --is-continuous \
  --sampling-rate 0.1
```
**两个(c):**在创建时使用`--is-continuous`，然后在步骤8中触发回填运行。

###步骤8：触发回填运行（如果请求）

> **Eval索引滞后：** Eval索引是从主跟踪存储异步构建的，可能滞后** 1-2小时**。对于您的第一次测试运行，使用至少在过去2小时结束的时间窗口。如果在最后一小时内摄取的跨度上将`--data-end-time`设置为“now”，则运行将成功完成，但跨度得分为0。

首先找出有数据的时间范围：```bash
ax spans export PROJECT --space SPACE -l 100 --days 1 --stdout   # try last 24h first
ax spans export PROJECT --space SPACE -l 100 --days 7 --stdout   # widen if empty
```
使用来自real span的`start_time`/`end_time`字段来设置窗口。对于第一次验证运行，将`--max-spans`设置为~100，以获得快速反馈：```bash
ax tasks trigger-run TASK_ID \
  --data-start-time "2026-03-20T00:00:00" \
  --data-end-time "2026-03-21T23:59:59" \
  --max-spans 100 \
  --wait
```
在扩展到完全回填或启用连续之前，检查分数和解释。

---

工作流程B：为实验创建一个评估器

当用户说“为我的实验创建一个评估器”或“评估我的数据集运行”时，使用这个。

**如果用户说“数据集”但没有实验：**任务必须针对实验（而不是裸数据集）。问:
>“评估任务根据实验运行，而不是直接根据数据集运行。你愿意先在这个数据集上做一个实验吗？”

如果是，使用**缩放-实验**技能创建一个，然后返回这里。

###第一步：找到数据集和实验名称```bash
ax datasets list --space SPACE
ax experiments list --dataset DATASET_NAME --space SPACE -o json
```
请注意数据集名称和实验名称以进行评分。它们接受后续命令中的名称或id—首选名称。

步骤2：了解要评估的内容

如果用户指定了评估器类型→跳转到步骤3。

如果没有，检查最近运行的实验，以实际数据为基础的评估器：```bash
ax experiments export EXPERIMENT_NAME --dataset DATASET_NAME --space SPACE --stdout | python3 -c "import sys,json; runs=json.load(sys.stdin); print(json.dumps(runs[0], indent=2))"
```
查看`output`、`input`、`evaluations`和`metadata`字段。找出差距（用户关心但还没有的指标），并提出1-3个评估人员的想法。每个建议必须包括：评估者名称（粗体）、一句话描述和括号中的二进制标签对——与工作流a步骤2的格式相同。

步骤3：确认或创建AI集成

与工作流程A步骤3相同。

步骤4：创建评估器

与工作流程A步骤4相同。保持变量的通用性。

###步骤5：根据实际运行数据确定列映射

运行数据的形状不同于跨度数据。检查:```bash
ax experiments export EXPERIMENT_NAME --dataset DATASET_NAME --space SPACE --stdout | python3 -c "import sys,json; runs=json.load(sys.stdin); print(json.dumps(runs[0], indent=2))"
```
实验运行的常见映射：
-`output`→`"output"`（每次运行时的顶级字段）
-`input`→检查它是在运行中还是嵌入在链接数据集示例中

如果`input`不在运行JSON中，导出数据集示例查找路径：```bash
ax datasets export DATASET_NAME --space SPACE --stdout | python3 -c "import sys,json; ex=json.load(sys.stdin); print(json.dumps(ex[0], indent=2))"
```
步骤6：创建任务```bash
ax tasks create \
  --name "Experiment Correctness" \
  --task-type template_evaluation \
  --dataset DATASET_NAME --space SPACE \
  --experiment-ids "EXP_ID" \   # base64 ID from `ax experiments list --space SPACE -o json`
  --evaluators '[{"evaluator_id": "EVAL_ID", "column_mappings": {"output": "output"}}]' \
  --no-continuous
```
###步骤7：触发和监控```bash
ax tasks trigger-run TASK_ID \
  --experiment-ids "EXP_ID" \   # base64 ID from `ax experiments list --space SPACE -o json`
  --wait

ax tasks list-runs TASK_ID
ax tasks get-run RUN_ID
```
---

模板设计的最佳实践

# # # 1。使用通用的、可移植的变量名

使用`{input}`、`{output}`和`{context}`——不要使用与特定项目或跨度属性相关的名称（例如，不要使用`{attributes_input_value}`）。求值器本身保持抽象；**任务的`column_mappings`**是将其连接到特定项目或实验中的实际字段的地方。这使得同一个评估者可以在不修改的情况下运行多个项目和实验。

# # # 2。默认为二进制标签

使用两个明确的字符串标签（例如`hallucinated`/`factual`，`correct`/`incorrect`,`pass`/`fail`）。二元标签是：
-最容易的法官模型生产一致
-最常见的行业
-在仪表板中最容易解释

如果用户坚持使用两个以上的选择，那没问题——但建议先使用二进制，并解释利弊（更多的标签→更多的歧义→更低的评级间可靠性）。# # # 3。明确说明模型必须返回的内容

模板必须告诉判断模型只使用标签字符串来响应，而不是其他字符串。提示符中的标签字符串必须与`--classification-choices`中的标签完全匹配（相同的拼写，相同的大小写）。

好:```
Respond with exactly one of these labels: hallucinated, factual
```
糟糕（太开放式）：```
Is this hallucinated? Answer yes or no.
```
# # # 4。保持低温

通过`--invocation-params '{"temperature": 0}'`获得可重复的评分。较高的温度会在评估结果中引入噪声。

# # # 5。使用`--include-explanations`进行调试

在初始设置期间，始终包含解释，以便在大规模信任标签之前验证裁判的推理是正确的。

# # # 6。在bash中用单引号传递模板

单引号防止shell插入`{variable}`占位符。双引号会引起问题：```bash
# Correct
--template 'Judge this: {input} → {output}'

# Wrong — shell may interpret { } or fail
--template "Judge this: {input} → {output}"
```
# # # 7。始终设置`--classification-choices`以匹配模板标签`--classification-choices`中的标签必须与`--template`中引用的标签完全匹配（相同的拼写，相同的大小写）。忽略`--classification-choices`会导致任务运行失败，因为“缺少轨道和分类选择”。

---

# #故障排除

|解决方案||---------|----------|
|`ax: command not found`|参见references/ax-setup.md|
|`401 Unauthorized`| API密钥可能无法访问此空间。在https://app.arize.com/admin> API Keys |处验证
|`Evaluator not found`|`ax evaluators list --space SPACE`|
|`Integration not found`|`ax ai-integrations list --space SPACE`|
|`Task not found`|`ax tasks list --space SPACE`|
|`project and dataset-id are mutually exclusive`|创建任务|时只能使用一个
|`experiment-ids required for dataset tasks`|将`--experiment-ids`添加到`create`和`trigger-run`|
|`sampling-rate only valid for project tasks`|从数据集任务|中删除`--sampling-rate`|`ax spans export`验证错误|项目名称通常工作；如果仍然出现验证错误，请通过`ax projects list --space SPACE -o json`查找base64项目ID，并使用`id`字段代替|
在bash中使用单引号`--template '...'`；单括号`{var}`，而不是双括号`{{var}}`|
|运行卡在`pending`|`ax tasks get-run RUN_ID`；然后`ax tasks cancel-run RUN_ID`|
|运行`cancelled`~1s |集成凭据无效-检查AI集成|
|运行`cancelled`~3min |找到了span，但LLM调用失败-模型名称错误或密钥错误|
|运行`completed`， 0跨越|拓宽时间窗口；Eval索引可能没有T覆盖旧数据|
修复`column_mappings`以匹配spans/runs|上的真实路径
添加`--include-explanations`并检查几个样本上的判断推理|
|匹配`query_filter`和`column_mappings`到LLM vs CHAIN跨度|
|`trigger-run`|时间格式错误使用`2026-03-21T09:00:00`-不带尾`Z`|
|添加`--classification-choices '{"label_a": 1, "label_b": 0}'`到`ax evaluators create`-标签必须匹配模板|
|运行`completed`，所有跨度跳过|查询过滤器匹配的跨度，但列映射错误或模板变量无法解析-导出一个样本跨度并验证路径|
|`query_filter`set but 0 span scores | filter属性可能不会在eval索引中被索引。`attributes.metadata.*`和自定义属性通常没有索引。使用`span_kind`或`attributes.llm.model_name`代替，或者删除过滤器以确认窗口中存在跨度。|诊断取消的运行

当任务运行被取消时（状态`cancelled`），按顺序执行以下检查表：

* * 1。检查集成凭证**```bash
ax ai-integrations list --space SPACE -o json
```
验证评估器使用的集成ID是否存在，并且具有有效的凭据。如果集成被删除或API密钥过期，运行将在1秒内取消。

* * 2。验证模型名称**```bash
ax evaluators get EVALUATOR_NAME --space SPACE -o json
```
检查`model_name`字段。输入错误或弃用的模型会导致LLM调用失败，并在约3分钟后取消运行。

* * 3。导出样例span/run，并将路径与column_mappings**进行比较

对于项目任务：```bash
ax spans export PROJECT --space SPACE -l 1 --days 7 --stdout | python3 -m json.tool
```
对于实验任务：```bash
ax experiments export EXPERIMENT_NAME --dataset DATASET_NAME --space SPACE --stdout | python3 -c "import sys,json; runs=json.load(sys.stdin); print(json.dumps(runs[0], indent=2)) if runs else print('No runs')"
```
将导出的JSON路径与任务的`column_mappings`进行比较。对于每个模板变量，确认映射的路径确实存在。常见的不匹配:
-在实验运行时映射`output`到`attributes.output.value`（应该只是`output`）
—当实际路径为`attributes.llm.input_messages`时，将`input`映射为`attributes.input.value`—将`context`映射到正在过滤的跨度类型上不存在的路径

* * 4。检查`data_start_time`不是epoch**

如果`trigger-run`使用的起始时间为`0`、`1970-01-01`或空字符串，则时间窗口无效。总是从实跨度时间戳派生：```bash
ax spans export PROJECT --space SPACE -l 5 --days 30 --stdout | python3 -c "
import sys, json
spans = json.load(sys.stdin)
for s in spans:
    print(s.get('start_time', 'N/A'), s.get('end_time', 'N/A'))
"
```
* * 5。验证跨度类型匹配求值器范围**

如果用`--data-granularity trace`创建了求值器，但任务的`query_filter`是`span_kind = 'LLM'`，则运行可能找不到符合条件的数据并取消。确保粒度和过滤器是一致的。

* * 6。检查所有模板变量是否解析**

求值器模板中的每个`{variable}`都必须有一个对应的`column_mappings`项，该项解析为一个非空值。针对实际范围测试分辨率：```bash
ax spans export PROJECT --space SPACE -l 3 --days 7 --stdout | python3 -c "
import sys, json
spans = json.load(sys.stdin)
# Replace these paths with your actual column_mappings values
mappings = {'input': 'attributes.input.value', 'output': 'attributes.output.value'}
for i, span in enumerate(spans):
    print(f'--- Span {i} ---')
    for var, path in mappings.items():
        parts = path.split('.')
        val = span
        for p in parts:
            val = val.get(p) if isinstance(val, dict) else None
        status = 'FOUND' if val else 'MISSING'
        print(f'  {var} ({path}): {status} — {str(val)[:80] if val else \"null\"}')
"
```
如果任何变量在所有跨度上显示MISSING，则修复列映射或调整`query_filter`以针对不同的跨度类型。

---

相关技能

**: LLM提供商集成的完整CRUD（创建，更新，删除凭据）
—** ize-trace**：导出跨度以发现列路径和时间范围
- ** ize-experiment**：创建实验并为实验列映射导出运行
- ** ize-dataset**：导出数据集示例，以便在运行时忽略输入字段
- ** alize -link**：到alize UI中的评估器和任务的深度链接

---

##保存凭据以备将来使用

参见references/ax-profiles.md§保存凭据以备将来使用。