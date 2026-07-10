---
name: arize-prompt-optimization
description: Optimizes, improves, and debugs LLM prompts using production trace data, evaluations, and annotations. Extracts prompts from spans, gathers performance signal, and runs a data-driven optimization loop using the ax CLI. Use when the user mentions optimize prompt, improve prompt, make AI respond better, improve output quality, prompt engineering, prompt tuning, or system prompt improvement.
metadata:
  author: arize
  version: "1.0"
compatibility: Requires the ax CLI and a configured Arize profile.
---
#熟悉提示优化技能

> **`SPACE`** -所有`--space`标志和`ARIZE_SPACE`env变量接受一个空间**名称**（例如，`my-workspace`）或一个base64空间**ID**（例如，`U3BhY2U6...`）。用`ax spaces list`找到你的。

# #的概念

提示符在跟踪数据中的位置

LLM应用程序发出的跨度遵循OpenInference语义约定。根据跨度类型和工具的不同，提示符存储在不同的跨度属性中：

|列|包含什么|何时使用||--------|-----------------|-------------|
|`attributes.llm.input_messages`|基于角色格式的结构化聊天消息（系统、用户、助手、工具）| **基于聊天的LLM提示|的主要来源**
|`attributes.llm.input_messages.roles`|角色数组：`system`、`user`、`assistant`、`tool`|提取单个消息角色|
|`attributes.llm.input_messages.contents`|消息内容字符串数组|提取消息文本|
|`attributes.input.value`|序列化提示或用户问题（通用的，所有跨度类型）|当结构化消息不可用时的回退|
|`attributes.llm.prompt_template.template`|模板与`{variable}`占位符（例如，`"Answer {question} using {context}"`） |当应用程序使用提示模板|
|`attributes.llm.prompt_template.variables`|模板变量值（JSON对象）|查看哪些值被替换到模板|中
|`attributes.output.value`|模型响应文本|看看LLM产生了什么|
|`attributes.llm.output_messages`|结构化模型输出（包括工具调用）|检查工具调用响应|

###根据Span类型查找提示- **LLM span** (`attributes.openinference.span.kind = 'LLM'`)：检查`attributes.llm.input_messages`结构化聊天消息，或`attributes.input.value`序列化提示。查看“`attributes.llm.prompt_template.template`”是否为模板。
—**Chain/Agentspan**:`attributes.input.value`包含用户的问题。实际的LLM提示符位于**子LLM跨越**——沿着跟踪树导航。
- **刀具跨度**:`attributes.input.value`有刀具输入，`attributes.output.value`有刀具结果。通常不会在提示符所在的地方。

###性能信号列

这些列携带用于优化的反馈数据：

|列模式|源|它告诉你的||---------------|--------|-------------------|
|分类等级（如`correct`，`incorrect`,`partial`） |
|`annotation.<name>.score`|人类评审员|数字质量评分（例如0.0 - 1.0）|
|`annotation.<name>.text`|人工审稿人|对等级|的自由解释
|`eval.<name>.label`| LLM-as-judge评价|自动分类评估|
|`eval.<name>.score`| LLM-as-judge评价|自动数字评分|
|`eval.<name>.explanation`| LLM-as-judge evals |为什么eval给出了这个分数——**最有价值的优化** |
|`attributes.input.value`|跟踪数据|进入LLM的|
|`attributes.output.value`|跟踪数据| LLM产生的|
|`{experiment_name}.output`|实验运行|特定实验|的输出

# #先决条件

直接执行任务—运行所需的`ax`命令。不要预先检查版本、环境变量或配置文件。如果执行`ax`命令失败，请根据提示信息进行处理：
-`command not found`或版本错误→参见references/ax-setup.md-`401 Unauthorized`/缺少API密钥→运行`ax profiles show`检查当前配置文件。如果配置文件丢失或API密钥错误，请按照references/ax-profiles.md到create/update。如果用户没有他们的密钥，将他们引导到https://app.arize.com/admin> API Keys
-空间未知→运行`ax spaces list`按名称选择，或询问用户
-项目不清楚→询问用户，或运行`ax projects list -o json --limit 100`并呈现为可选选项
LLM提供程序调用失败（缺少OPENAI_API_KEY / ANTHROPIC_API_KEY）→运行`ax ai-integrations list --space SPACE`检查平台管理的凭据。如果不存在，请要求用户提供密钥或通过alize -ai-provider-integration技能创建集成
- **安全：**永远不要读取`.env`文件或搜索文件系统的凭据。使用`ax profiles`作为alize凭证，使用`ax ai-integrations`作为LLM提供程序密钥。如果credential不能通过这些渠道获得，请询问用户。阶段1：提取当前提示符

查找包含提示符的LLM span```bash
# Sample LLM spans (where prompts live)
ax spans export PROJECT --filter "attributes.openinference.span.kind = 'LLM'" -l 10 --stdout

# Filter by model
ax spans export PROJECT --filter "attributes.llm.model_name = 'gpt-4o'" -l 10 --stdout

# Filter by span name (e.g., a specific LLM call)
ax spans export PROJECT --filter "name = 'ChatCompletion'" -l 10 --stdout
```
导出跟踪以检查提示结构```bash
# Export all spans in a trace
ax spans export PROJECT --trace-id TRACE_ID

# Export a single span
ax spans export PROJECT --span-id SPAN_ID
```
从导出的JSON中提取提示```bash
# Extract structured chat messages (system + user + assistant)
jq '.[0] | {
  messages: .attributes.llm.input_messages,
  model: .attributes.llm.model_name
}' trace_*/spans.json

# Extract the system prompt specifically
jq '[.[] | select(.attributes.llm.input_messages.roles[]? == "system")] | .[0].attributes.llm.input_messages' trace_*/spans.json

# Extract prompt template and variables
jq '.[0].attributes.llm.prompt_template' trace_*/spans.json

# Extract from input.value (fallback for non-structured prompts)
jq '.[0].attributes.input.value' trace_*/spans.json
```
将提示重构为消息

有了span数据后，将提示符重构为消息数组：```json
[
  {"role": "system", "content": "You are a helpful assistant that..."},
  {"role": "user", "content": "Given {input}, answer the question: {question}"}
]
```
如果span具有`attributes.llm.prompt_template.template`，则提示符使用变量。保留这些占位符（`{variable}`或`{{variable}}`）——它们在运行时被替换。

阶段2：收集性能数据

From traces（生产反馈）```bash
# Find error spans -- these indicate prompt failures
ax spans export PROJECT \
  --filter "status_code = 'ERROR' AND attributes.openinference.span.kind = 'LLM'" \
  -l 20 --stdout

# Find spans with low eval scores
ax spans export PROJECT \
  --filter "annotation.correctness.label = 'incorrect'" \
  -l 20 --stdout

# Find spans with high latency (may indicate overly complex prompts)
ax spans export PROJECT \
  --filter "attributes.openinference.span.kind = 'LLM' AND latency_ms > 10000" \
  -l 20 --stdout

# Export error traces for detailed inspection
ax spans export PROJECT --trace-id TRACE_ID
```
从数据集和实验中```bash
# Export a dataset (ground truth examples)
ax datasets export DATASET_NAME --space SPACE
# -> dataset_*/examples.json

# Export experiment results (what the LLM produced)
ax experiments export EXPERIMENT_NAME --dataset DATASET_NAME --space SPACE
# -> experiment_*/runs.json
```
合并数据集+实验进行分析

通过`example_id`连接两个文件，以查看输出和评估旁边的输入：```bash
# Count examples and runs
jq 'length' dataset_*/examples.json
jq 'length' experiment_*/runs.json

# View a single joined record
jq -s '
  .[0] as $dataset |
  .[1][0] as $run |
  ($dataset[] | select(.id == $run.example_id)) as $example |
  {
    input: $example,
    output: $run.output,
    evaluations: $run.evaluations
  }
' dataset_*/examples.json experiment_*/runs.json

# Find failed examples (where eval score < threshold)
jq '[.[] | select(.evaluations.correctness.score < 0.5)]' experiment_*/runs.json
```
确定要优化的内容

在失败中寻找模式：

1. **将输出与真实值进行比较**:LLM输出与预期的差异在哪里？
2. **读取eval解释**:`eval.*.explanation`告诉你为什么失败
3. **检查注释文本**：人工反馈描述具体问题
4. **寻找冗长不匹配**：如果输出太long/short与接地真理
5. **检查格式遵从性**：输出是否符合预期格式？

阶段3：优化提示

优化元提示符

使用此模板生成改进版本的提示符。填写三个占位符并将其发送给您的LLM （gpt - 40， Claude等）：````
You are an expert in prompt optimization. Given the original baseline prompt
and the associated performance data (inputs, outputs, evaluation labels, and
explanations), generate a revised version that improves results.

ORIGINAL BASELINE PROMPT
========================

{PASTE_ORIGINAL_PROMPT_HERE}

========================

PERFORMANCE DATA
================

The following records show how the current prompt performed. Each record
includes the input, the LLM output, and evaluation feedback:

{PASTE_RECORDS_HERE}

================

HOW TO USE THIS DATA

1. Compare outputs: Look at what the LLM generated vs what was expected
2. Review eval scores: Check which examples scored poorly and why
3. Examine annotations: Human feedback shows what worked and what didn't
4. Identify patterns: Look for common issues across multiple examples
5. Focus on failures: The rows where the output DIFFERS from the expected
   value are the ones that need fixing

ALIGNMENT STRATEGY

- If outputs have extra text or reasoning not present in the ground truth,
  remove instructions that encourage explanation or verbose reasoning
- If outputs are missing information, add instructions to include it
- If outputs are in the wrong format, add explicit format instructions
- Focus on the rows where the output differs from the target -- these are
  the failures to fix

RULES

Maintain Structure:
- Use the same template variables as the current prompt ({var} or {{var}})
- Don't change sections that are already working
- Preserve the exact return format instructions from the original prompt

Avoid Overfitting:
- DO NOT copy examples verbatim into the prompt
- DO NOT quote specific test data outputs exactly
- INSTEAD: Extract the ESSENCE of what makes good vs bad outputs
- INSTEAD: Add general guidelines and principles
- INSTEAD: If adding few-shot examples, create SYNTHETIC examples that
  demonstrate the principle, not real data from above

Goal: Create a prompt that generalizes well to new inputs, not one that
memorizes the test data.

OUTPUT FORMAT

Return the revised prompt as a JSON array of messages:

[
  {"role": "system", "content": "..."},
  {"role": "user", "content": "..."}
]

Also provide a brief reasoning section (bulleted list) explaining:
- What problems you found
- How the revised prompt addresses each one
````
准备性能数据

在粘贴到模板之前，将记录格式化为JSON数组：```bash
# From dataset + experiment: join and select relevant columns
jq -s '
  .[0] as $ds |
  [.[1][] | . as $run |
    ($ds[] | select(.id == $run.example_id)) as $ex |
    {
      input: $ex.input,
      expected: $ex.expected_output,
      actual_output: $run.output,
      eval_score: $run.evaluations.correctness.score,
      eval_label: $run.evaluations.correctness.label,
      eval_explanation: $run.evaluations.correctness.explanation
    }
  ]
' dataset_*/examples.json experiment_*/runs.json

# From exported spans: extract input/output pairs with annotations
jq '[.[] | select(.attributes.openinference.span.kind == "LLM") | {
  input: .attributes.input.value,
  output: .attributes.output.value,
  status: .status_code,
  model: .attributes.llm.model_name
}]' trace_*/spans.json
```
应用修改后的提示符

LLM返回修改后的消息数组后：

1. 将原提示和修改后的提示并排比较
2. 验证所有模板变量都被保留
3. 检查格式说明是否完整
4. 在完全部署之前对几个示例进行测试

阶段4：迭代

优化循环```
1. Extract prompt    -> Phase 1 (once)
2. Run experiment    -> ax experiments create ...
3. Export results    -> ax experiments export EXPERIMENT_NAME --dataset DATASET_NAME --space SPACE
4. Analyze failures  -> jq to find low scores
5. Run meta-prompt   -> Phase 3 with new failure data
6. Apply revised prompt
7. Repeat from step 2
```
###衡量改进```bash
# Compare scores across experiments
# Experiment A (baseline)
jq '[.[] | .evaluations.correctness.score] | add / length' experiment_a/runs.json

# Experiment B (optimized)
jq '[.[] | .evaluations.correctness.score] | add / length' experiment_b/runs.json

# Find examples that flipped from fail to pass
jq -s '
  [.[0][] | select(.evaluations.correctness.label == "incorrect")] as $fails |
  [.[1][] | select(.evaluations.correctness.label == "correct") |
    select(.example_id as $id | $fails | any(.example_id == $id))
  ] | length
' experiment_a/runs.json experiment_b/runs.json
```
比较两个提示符

1. 针对同一数据集创建两个实验，每个实验使用不同的提示版本
2. 导出两个：`ax experiments export EXP_A`和`ax experiments export EXP_B`3. 比较平均分数、失败率和具体的投掷例子
4. 检查回归——通过提示A但通过提示B的例子

提示工程最佳实践

在写作或修改提示时应用这些方法：

|技术|何时应用|示例||-----------|--------------|---------|
|清晰，详细的说明|输出模糊或偏离主题|“将情绪分为：积极，消极，中性”|
|开头指令|模型忽略后面的指令|将任务描述放在示例前面|
|分步分解|复杂的多步骤流程|“首先提取实体，然后对每个实体进行分类，然后进行总结”|
|特定角色|需要一致的style/tone|“你是一名高级金融分析师，为机构投资者写作”|
|使用`---`、`###`或XML标记将输入与指令|分开
|输出格式需要澄清|显示2-3个合成input/output对|
|输出长度规格|响应太长或太短|“以2-3句话准确响应”|
推理指导|准确性至关重要|“在回答之前一步一步地思考”|“我不知道”指导原则|产生幻觉是有风险的|“如果答案不在提供的上下文中，就说‘我没有足够的信息’”|变量保存

在优化使用模板变量的提示符时：

- **单括号** (`{variable}`): Python f-string / Jinja风格。在阿拉斯加州最常见。
- **双括号** (`{{variable}}`)：小胡子风格。当框架需要时使用。
-永远不要在优化过程中添加或删除变量占位符
-永远不要重命名变量——运行时替换依赖于确切的名称
-如果添加几个镜头的例子，使用文字值，而不是变量占位符

# #工作流程

从失败跟踪中优化提示

1. 查找失败的踪迹：   ```bash
   ax traces list PROJECT --filter "status_code = 'ERROR'" --limit 5
   ```
2. 导出跟踪：   ```bash
   ax spans export PROJECT --trace-id TRACE_ID
   ```
3. 从LLM跨度中提取提示符：   ```bash
   jq '[.[] | select(.attributes.openinference.span.kind == "LLM")][0] | {
     messages: .attributes.llm.input_messages,
     template: .attributes.llm.prompt_template,
     output: .attributes.output.value,
     error: .attributes.exception.message
   }' trace_*/spans.json
   ```
4. 从错误消息或输出中确定失败的内容
5. 用提示和错误上下文填充优化元提示（阶段3）
6. 应用修改后的提示

###使用数据集和实验进行优化

1. 找到数据集并进行实验：   ```bash
   ax datasets list --space SPACE
   ax experiments list --dataset DATASET_NAME --space SPACE
   ```
2. 出口两个:   ```bash
   ax datasets export DATASET_NAME --space SPACE
   ax experiments export EXPERIMENT_NAME --dataset DATASET_NAME --space SPACE
   ```
3. 为元提示符准备联接数据
4. 运行优化元提示符
5. 用修改后的提示创建一个新的实验来衡量改进情况

调试产生错误格式的提示符

1. 导出跨越输出格式错误的地方：   ```bash
   ax spans export PROJECT \
     --filter "attributes.openinference.span.kind = 'LLM' AND annotation.format.label = 'incorrect'" \
     -l 10 --stdout > bad_format.json
   ```
2. 看看法学硕士产生了什么和预期的是什么
3. 向提示符添加明确的格式说明（JSON模式、示例、分隔符）
4. 常见修复：添加几个示例，显示所需的输出格式

减少在RAG提示中的幻觉

1. 找到模型产生幻觉的痕迹：   ```bash
   ax spans export PROJECT \
     --filter "annotation.faithfulness.label = 'unfaithful'" \
     -l 20 --stdout
   ```
2. 导出和检查猎犬+ LLM跨度一起：   ```bash
   ax spans export PROJECT --trace-id TRACE_ID
   jq '[.[] | {kind: .attributes.openinference.span.kind, name, input: .attributes.input.value, output: .attributes.output.value}]' trace_*/spans.json
   ```
3. 检查检索的上下文是否实际包含答案
4. 在系统提示中添加接地说明：“仅使用提供的上下文中的信息。”如果上下文没有给出答案，就直接说出来。”

# #故障排除

|解决方案||---------|----------|
|`ax: command not found`|参见references/ax-setup.md|
|`No profile found`|未配置配置文件。请参见references/ax-profiles.md创建一个。|`input_messages`on span |检查span类型——Chain/Agentspan在子LLM span上存储提示，而不是自己|
并不是所有的仪器都发出`prompt_template`。使用`input_messages`或`input.value`代替|
验证修改后的提示保留了原始|中的所有`{var}`占位符
|检查是否过拟合——元提示符可能记住了测试数据。确保少数镜头的例子是合成|
|首先运行评估（通过alize UI或SDK），然后重新导出|
|实验输出列未找到|列名为`{experiment_name}.output`——通过`ax experiments get`|检查确切的实验名称
|`jq`错误在span JSON |确保你的目标是正确的文件路径（例如，`trace_*/spans.json`） |