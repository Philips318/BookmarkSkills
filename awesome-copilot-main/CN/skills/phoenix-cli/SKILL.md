---
name: phoenix-cli
description: Debug LLM applications using the Phoenix CLI. Fetch traces, analyze errors, structure trace review with open coding and axial coding, inspect datasets, review experiments, query annotation configs, and use the GraphQL API. Use whenever the user is analyzing traces or spans, investigating LLM/agent failures, deciding what to do after instrumenting an app, building failure taxonomies, choosing what evals to write, or asking "what's going wrong", "what kinds of mistakes", or "where do I focus" — even without naming a technique.
license: Apache-2.0
compatibility: Requires Node.js (for npx) or global install of @arizeai/phoenix-cli. Optionally requires jq for JSON processing.
metadata:
  author: arize-ai
  version: "3.3.0"
---
#凤凰命令行

# #调用```bash
px <resource> <action>                          # if installed globally
npx @arizeai/phoenix-cli <resource> <action>    # no install required
```
CLI使用单一资源命令和子命令，如`list`和`get`：```bash
px trace list
px trace get <trace-id>
px trace annotate <trace-id>
px trace add-note <trace-id>
px trace-annotations delete
px span list
px span annotate <span-id>
px span add-note <span-id>
px span-annotations delete
px session list
px session get <session-id>
px session annotate <session-id>
px session add-note <session-id>
px session-annotations delete
px dataset list
px dataset get <name>
px project list
px project get <name>
px annotation-config list
px auth status
px profile list
px profile show [name]
px profile create <name>
px profile use <name>
px profile edit <name>
px profile delete <name>
```
# #设置```bash
export PHOENIX_HOST=http://localhost:6006
export PHOENIX_PROJECT=my-project
export PHOENIX_API_KEY=your-api-key  # if auth is enabled
```
当管道连接到`jq`时，总是使用`--format raw --no-progress`。

##快速参考

|任务|文件|| ---- | ----- |
| [references/open-coding](references/open-coding.md) |查看采样的轨迹、跨度或会话，并记录出具体的错误（尚未分类）
| [references/axial-coding](references/axial-coding.md) |这两个阶段用一个共享的编码注释标识符（描述性形状，例如`coding-run:chatbot-context-loss-2026-05-06`）标记每个工件，因此运行是可查询的，可逆的，并且可以作为一个单元查看。在每个`px`调用上显式地传递`--identifier <value>`-跨代理线段的shell继承是不可靠的。开放编码通过`px ... add-note`写笔记，并在`.px/coding/<sanitized-identifier>.jsonl`记录一个小的本地JSONL sidecar；轴向编码将侧车读取为确定性切换，并在`.px/coding/<sanitized-identifier>-axial.jsonl`中记录标签。每次运行选择标识符一次（参见[references/open-coding.md](references/open-coding.md# code -annotation-identifier- Pick -this-first)），然后分享总结部分中的Phoenix UI链接。Revert是可选的，只有在明确的用户确认之后才运行三个标识符绑定的delete。b> **工作流术语与服务器注释名称。技能散文将这个值称为编码注释标识符（shell变量提示：`CODING_ANNOTATION_IDENTIFIER`）。用于UI过滤器的服务器端注释NAME保持不变—`coding_session_id`—以便与以前运行已经写入的行保持数据兼容性。不要试图重命名服务器端注释；将不对称视为承重。

# #工作流程

**“测试后我该做什么？”/“我该把注意力集中在哪里？”/“出什么事了？”**
[开放编码]（references/open-coding.md）→[轴向编码]（references/axial-coding.md）→为顶级类别构建评估。

##参考类别

|前缀|描述信息|| ------ | ----------- |
|`references/open-coding`|针对采样轨迹、跨度或会话的自由形式注释-每当用户想要了解LLM流量但还没有故障类别时，就可以使用它。包括一个分析单元诊断，因此工作流在故障模式实际存在的级别上运行（无状态单次调用的跟踪，多回合代理的会话，mechanical/in-isolation故障的范围）。|
|`references/axial-coding`|将笔记归纳为带有计数的MECE分类法-每当用户有观察和需要分类或评估目标时就可以使用|

# #身份验证```bash
px auth status                                # check connection and authentication
px auth status --endpoint http://other:6006   # check a specific endpoint
px auth status --profile staging              # check a named profile's connection
```
# #概要文件

命名的配置文件允许您在多个Phoenix实例（本地、暂存、云）之间切换，而无需处理环境变量。配置文件存储在`~/.px/settings.json`（或`$XDG_CONFIG_HOME/px/settings.json`）中。

配置优先级（从高到低）：命令行标志> env vars > active profile >内置默认值。```bash
px profile list                              # list all profiles (shows active profile)
px profile show                              # show the active profile's settings
px profile show staging                      # show a named profile's settings
px profile create prod --endpoint https://app.phoenix.arize.com --api-key <key> --activate
px profile create local --endpoint http://localhost:6006 --project my-app
px profile use prod                          # switch the active profile
px profile edit prod                         # open profile JSON in $EDITOR (validates on save)
px profile delete prod --yes                 # delete a profile (--yes skips confirmation)
```
在任何命令上使用`--profile <name>`来针对特定的配置文件，而不更改活动配置文件：```bash
px trace list --profile staging --limit 10 --format raw --no-progress | jq .
px auth status --profile prod
```
`px profile create`选项：`--endpoint <url>`，`--project <name>`,`--api-key <key>`,`--header <key=value>`（可重复），`--activate`# #项目```bash
px project list                                            # list all projects (table view)
px project list --format raw --no-progress | jq '.[].name' # project names as JSON
px project get my-project --format raw --no-progress       # single record by exact name
px project get my-project --format raw --no-progress | jq -r '.id'  # extract project id
```
`project get`在名称缺失时以`ExitCode.FAILURE`(1)退出，并将`StructuredError``{error, code: "FAILURE", hint}`写入到`--format json|raw`中的stderr。

# #痕迹```bash
px trace list --limit 20 --format raw --no-progress | jq .
px trace list --last-n-minutes 60 --limit 20 --format raw --no-progress | jq '.[] | select(.status == "ERROR")'
px trace list --since 2025-01-15T00:00:00Z --limit 50 --format raw --no-progress | jq .
px trace list --format raw --no-progress | jq 'sort_by(-.duration) | .[0:5]'
px trace list --include-notes --format raw --no-progress | jq '.[].notes'
px trace get <trace-id> --format raw | jq .
px trace get <trace-id> --format raw | jq '.spans[] | select(.status_code != "OK")'
px trace get <trace-id> --include-notes --format raw | jq '.notes'
px trace annotate <trace-id> --name reviewer --label pass
px trace annotate <trace-id> --name reviewer --score 0.9 --format raw --no-progress
px trace annotate <trace-id> --name reviewer --label pass --identifier "<coding-annotation-id>"  # tag with a coding annotation identifier
px trace add-note <trace-id> --text "needs follow-up"
px trace add-note <trace-id> --text "needs follow-up" --identifier "<coding-annotation-id>"  # tag + upsert on identifier
px trace-annotations delete --identifier "<coding-annotation-id>" --all -y            # nuke every annotation tied to this coding annotation identifier
```
`px <entity>-annotations delete`需要`--all`或`--start-time`和`--end-time`，并在成功时发出`{deleted: true, target, filter}`。

跟踪JSON形状```
Trace
  traceId, status ("OK"|"ERROR"), duration (ms), startTime, endTime
  annotations[] (with --include-annotations, excludes note)
    name, result { score, label, explanation }
  notes[] (with --include-notes)
    name="note", result { explanation }
  rootSpan  — top-level span (parent_id: null)
  spans[]
    name, span_kind ("LLM"|"CHAIN"|"TOOL"|"RETRIEVER"|"EMBEDDING"|"AGENT"|"RERANKER"|"GUARDRAIL"|"EVALUATOR"|"UNKNOWN")
    status_code ("OK"|"ERROR"|"UNSET"), parent_id, context.span_id
    notes[] (with --include-notes)
      name="note", result { explanation }
    attributes
      input.value, output.value          — raw input/output
      llm.model_name, llm.provider
      llm.token_count.prompt/completion/total
      llm.token_count.prompt_details.cache_read
      llm.token_count.completion_details.reasoning
      llm.input_messages.{N}.message.role/content
      llm.output_messages.{N}.message.role/content
      llm.invocation_parameters          — JSON string (temperature, etc.)
      exception.message                  — set if span errored
```
# #跨越```bash
px span list --limit 20                                    # recent spans (table view)
px span list --last-n-minutes 60 --limit 50                # spans from last hour
px span list --since 2025-01-15T00:00:00Z --limit 50       # spans since a timestamp
px span list --span-kind LLM --limit 10                    # only LLM spans
px span list --status-code ERROR --limit 20                # only errored spans
px span list --name chat_completion --limit 10             # filter by span name
px span list --trace-id <id> --format raw --no-progress | jq .   # all spans for a trace
px span list --parent-id null --limit 10                   # only root spans
px span list --parent-id <span-id> --limit 10              # only children of a span
px span list --include-annotations --limit 10              # include annotation scores
px span list --include-notes --limit 10                    # include span notes
px span list --attribute llm.model_name:gpt-4 --limit 10  # filter by string attribute
px span list --attribute llm.token_count.total:500 --limit 10  # filter by numeric attribute
px span list --attribute 'user.id:"12345"' --limit 10     # force string match for numeric-looking value
px span list --attribute session.id:sess:abc:123 --limit 20  # colon in value OK (split on first colon only)
px span list --attribute llm.model_name:gpt-4 --attribute session.id:abc --limit 10  # AND multiple filters
px span list output.json --limit 100                       # save to JSON file
px span list --format raw --no-progress | jq '.[] | select(.status_code == "ERROR")'
px span annotate <span-id> --name reviewer --label pass
px span annotate <span-id> --name checker --score 1 --annotator-kind CODE
px span annotate <span-id> --name reviewer --label pass --identifier "<coding-annotation-id>"  # tag with a coding annotation identifier
px span add-note <span-id> --text "verified by agent"
px span add-note <span-id> --text "verified by agent" --identifier "<coding-annotation-id>"  # tag + upsert on identifier
px span-annotations delete --identifier "<coding-annotation-id>" --all -y           # nuke every annotation tied to this coding annotation identifier
```
### Span JSON形状```
Span
  name, span_kind ("LLM"|"CHAIN"|"TOOL"|"RETRIEVER"|"EMBEDDING"|"AGENT"|"RERANKER"|"GUARDRAIL"|"EVALUATOR"|"UNKNOWN")
  status_code ("OK"|"ERROR"|"UNSET"), status_message
  context.span_id, context.trace_id, parent_id
  start_time, end_time
  attributes
    input.value, output.value          — raw input/output
    llm.model_name, llm.provider
    llm.token_count.prompt/completion/total
    llm.input_messages.{N}.message.role/content
    llm.output_messages.{N}.message.role/content
    llm.invocation_parameters          — JSON string (temperature, etc.)
    exception.message                  — set if span errored
  annotations[] (with --include-annotations, excludes note)
    name, result { score, label, explanation }
  notes[] (with --include-notes)
    name="note", result { explanation }
```
# #会话```bash
px session list --limit 10 --format raw --no-progress | jq .
px session list --order asc --format raw --no-progress | jq '.[].session_id'
px session list --include-annotations --include-notes --format raw --no-progress | jq '.[].notes'
px session get <session-id> --format raw | jq .
px session get <session-id> --include-annotations --format raw | jq '.session.annotations'
px session get <session-id> --include-notes --format raw | jq '.session.notes'
px session annotate <session-id> --name reviewer --label pass
px session annotate <session-id> --name reviewer --score 0.9 --format raw --no-progress
px session annotate <session-id> --name reviewer --label pass --identifier "<coding-annotation-id>"  # tag with a coding annotation identifier
px session add-note <session-id> --text "verified by agent"
px session add-note <session-id> --text "verified by agent" --identifier "<coding-annotation-id>"  # tag + upsert on identifier
px session-annotations delete --identifier "<coding-annotation-id>" --all -y              # nuke every annotation tied to this coding annotation identifier
```
会话JSON形状```
SessionData
  id, session_id, project_id
  start_time, end_time
  token_count_prompt, token_count_completion, token_count_total  — cumulative across all LLM spans in the session (int, default 0)
  annotations[] (with --include-annotations, excludes note)
    name, result { score, label, explanation }
  notes[] (with --include-notes)
    name="note", result { explanation }
  traces[]
    id, trace_id, start_time, end_time
```
##数据集/实验/提示```bash
px dataset list --format raw --no-progress | jq '.[].name'
px dataset get <name> --format raw | jq '.examples[] | {input, output: .expected_output}'
px dataset get <name> --split train --format raw | jq .    # filter by split
px dataset get <name> --version <version-id> --format raw | jq .
px experiment list --dataset <name> --format raw --no-progress | jq '.[] | {id, name, failed_run_count}'
px experiment get <id> --format raw --no-progress | jq '.[] | select(.error != null) | {input, error}'
px prompt list --format raw --no-progress | jq '.[].name'
px prompt get <name> --format text --no-progress   # plain text, ideal for piping to AI
```
##注释配置```bash
px annotation-config list                                           # list all configs (table view)
px annotation-config list --format raw --no-progress | jq '.[].name' # config names as JSON
```
# # GraphQL

对于上述命令未涵盖的临时查询。输出为`{"data": {...}}`。```bash
px api graphql '{ projectCount datasetCount promptCount evaluatorCount }'
px api graphql '{ projects { edges { node { name traceCount tokenCountTotal } } } }' | jq '.data.projects.edges[].node'
px api graphql '{ datasets { edges { node { name exampleCount experimentCount } } } }' | jq '.data.datasets.edges[].node'
px api graphql '{ evaluators { edges { node { name kind } } } }' | jq '.data.evaluators.edges[].node'

# Introspect any type
px api graphql '{ __type(name: "Project") { fields { name type { name } } } }' | jq '.data.__type.fields[]'
```
关键根字段：`projects`、`datasets`、`prompts`、`evaluators`、`projectCount`、`datasetCount`、`promptCount`、`evaluatorCount`、`viewer`。

# #文档

下载Phoenix文档标记，供编码代理在本地使用。```bash
px docs fetch                                # fetch default workflow docs to .px/docs
px docs fetch --workflow tracing             # fetch only tracing docs
px docs fetch --workflow tracing --workflow evaluation
px docs fetch --dry-run                      # preview what would be downloaded
px docs fetch --refresh                      # clear .px/docs and re-download
px docs fetch --output-dir ./my-docs         # custom output directory
```
关键选项：`--workflow`（可重复，值：`tracing`，`evaluation`,`datasets`,`prompts`,`integrations`,`sdk`,`self-hosting`,`all`），`--dry-run`,`--refresh`,`--output-dir`（默认`.px/docs`），`--workers`（默认10）。