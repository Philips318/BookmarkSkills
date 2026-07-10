---
name: arize-link
description: Generates deep links to the Arize UI for traces, spans, sessions, datasets, labeling queues, evaluators, and annotation configs. Produces clickable URLs for sharing Arize resources with team members. Use when the user wants to link to or open a trace, span, session, dataset, evaluator, or annotation config in the Arize UI.
metadata:
  author: arize
  version: "1.0"
---
# alize Link

为跟踪、跨度、会话、数据集、标记队列、评估器和注释配置生成到alize UI的深度链接。

##何时使用

-用户希望链接到跟踪、跨度、会话、数据集、标签队列、评估器或注释配置
—从导出的数据或日志中获取id，需要链接回UI
-用户要求在Arize中“打开”或“查看”上述任何内容

##所需输入

从用户或上下文收集（导出的跟踪数据，解析的url）：

|始终需要|特定于资源的||---|---|
|`org_id`(base64) |`project_id`+`trace_id`[+`span_id`] -trace/span|
|`space_id`(base64) |`project_id`+`session_id`- session |
| |`dataset_id`- dataset |
| |`queue_id`特定的队列（省略列表）|
| |`evaluator_id`[+`version`] -求值器|

**所有路径id必须为base64编码的**（字符：`A-Za-z0-9+/=`）。原始数字ID生成一个看起来有效的URL，即404。如果用户提供了一个数字，请他们直接从他们的浏览器URL （`https://app.arize.com/organizations/{org_id}/spaces/{space_id}/…`）复制ID。如果你有一个原始的内部ID（例如`Organization:1:abC1`），在插入到URL之前对它进行base64编码。

## URL模板

基本URL:`https://app.arize.com`（覆盖内部部署）

**跟踪**（添加`&selectedSpanId={span_id}`以突出显示特定的跨度）：```
{base_url}/organizations/{org_id}/spaces/{space_id}/projects/{project_id}?selectedTraceId={trace_id}&queryFilterA=&selectedTab=llmTracing&timeZoneA=America%2FLos_Angeles&startA={start_ms}&endA={end_ms}&envA=tracing&modelType=generative_llm
```
会话:* * * *```
{base_url}/organizations/{org_id}/spaces/{space_id}/projects/{project_id}?selectedSessionId={session_id}&queryFilterA=&selectedTab=llmTracing&timeZoneA=America%2FLos_Angeles&startA={start_ms}&endA={end_ms}&envA=tracing&modelType=generative_llm
```
**Dataset** (`selectedTab`:`examples`or`experiments`)：```
{base_url}/organizations/{org_id}/spaces/{space_id}/datasets/{dataset_id}?selectedTab=examples
```
**队列列表/特定队列：**```
{base_url}/organizations/{org_id}/spaces/{space_id}/queues
{base_url}/organizations/{org_id}/spaces/{space_id}/queues/{queue_id}
```
**求值器**（省略`?version=…`）：```
{base_url}/organizations/{org_id}/spaces/{space_id}/evaluators/{evaluator_id}
{base_url}/organizations/{org_id}/spaces/{space_id}/evaluators/{evaluator_id}?version={version_url_encoded}
```
`version`值必须是url编码的（例如，`=`→`%3D`）。

* *注释配置:* *```
{base_url}/organizations/{org_id}/spaces/{space_id}/annotation-configs
```
时间范围

关键：`startA`和`endA`（epoch毫秒）对于trace/span/session链接是**必需的** -省略它们默认为最近7天，如果跟踪落在该窗口之外将显示“没有最近的数据”。

* *优先顺序:* *
1. **用户提供的URL** -直接提取和重用`startA`/`endA`。
2. **跨度`start_time`** - pad±1天（或±1小时，较紧的窗口）。
3. **后退** -最后90天（`now - 90d`到`now`）。

喜欢紧窗；90天的Windows加载缓慢。

# #指令

1. 从用户、导出数据或URL上下文中收集id。
2. 验证所有路径id都是base64编码的。
3. 使用上面的优先顺序确定`startA`/`endA`。
4. 将其替换为适当的模板，并作为可单击的降价链接呈现。

# #故障排除

|解决方案||---|---|
|“无数据”/空视图|时间窗口外的跟踪-加宽`startA`/`endA`（±1h→±1d→90d）。|
| 404 | ID错误或非base64。从浏览器URL重新检查`org_id`、`space_id`、`project_id`。|
| Span未突出显示|`span_id`可能属于不同的跟踪。根据导出的跨度数据进行验证。|
|`ax`CLI没有公开它。要求用户从`https://app.arize.com/organizations/{org_id}/spaces/{space_id}/…`复制。|

相关技能

- ** ize-trace**：导出范围以获取`trace_id`、`span_id`和`start_time`。

# #的例子

请参阅references/EXAMPLES.md获取每种链接类型的完整的具体url集。