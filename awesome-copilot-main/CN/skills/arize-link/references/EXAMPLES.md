# Arize链接示例

占位符：
-`{org_id}`- base64编码的组织ID
-`{space_id}`- base64编码的空间ID
-`{project_id}`- base64编码的项目ID
-`{start_ms}`/`{end_ms}`- epoch毫秒（例如1741305600000 / 1741392000000）

---

# #跟踪```
https://app.arize.com/organizations/{org_id}/spaces/{space_id}/projects/{project_id}?selectedTraceId={trace_id}&queryFilterA=&selectedTab=llmTracing&timeZoneA=America%2FLos_Angeles&startA={start_ms}&endA={end_ms}&envA=tracing&modelType=generative_llm
```
## Span （trace + Span突出显示）```
https://app.arize.com/organizations/{org_id}/spaces/{space_id}/projects/{project_id}?selectedTraceId={trace_id}&selectedSpanId={span_id}&queryFilterA=&selectedTab=llmTracing&timeZoneA=America%2FLos_Angeles&startA={start_ms}&endA={end_ms}&envA=tracing&modelType=generative_llm
```
# #会话```
https://app.arize.com/organizations/{org_id}/spaces/{space_id}/projects/{project_id}?selectedSessionId={session_id}&queryFilterA=&selectedTab=llmTracing&timeZoneA=America%2FLos_Angeles&startA={start_ms}&endA={end_ms}&envA=tracing&modelType=generative_llm
```
## Dataset （examples选项卡）```
https://app.arize.com/organizations/{org_id}/spaces/{space_id}/datasets/{dataset_id}?selectedTab=examples
```
##数据集（实验选项卡）```
https://app.arize.com/organizations/{org_id}/spaces/{space_id}/datasets/{dataset_id}?selectedTab=experiments
```
##标签队列列表```
https://app.arize.com/organizations/{org_id}/spaces/{space_id}/queues
```
标签队列（特定）```
https://app.arize.com/organizations/{org_id}/spaces/{space_id}/queues/{queue_id}
```
##评估器（最新版本）```
https://app.arize.com/organizations/{org_id}/spaces/{space_id}/evaluators/{evaluator_id}
```
##评估器（特定版本）```
https://app.arize.com/organizations/{org_id}/spaces/{space_id}/evaluators/{evaluator_id}?version={version_url_encoded}
```
##注释配置```
https://app.arize.com/organizations/{org_id}/spaces/{space_id}/annotation-configs
```
