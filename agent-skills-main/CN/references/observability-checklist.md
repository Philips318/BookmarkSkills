# Observability Checklist

为生产代码添加 instrumentation 的快速参考。与 `observability-and-instrumentation` skill 搭配使用。

## Table of Contents

- [On-Call Questions (Start Here)](#on-call-questions-start-here)
- [Structured Logging](#structured-logging)
- [Metrics](#metrics)
- [Distributed Tracing](#distributed-tracing)
- [Alerting](#alerting)
- [Dashboards](#dashboards)
- [Verify the Telemetry](#verify-the-telemetry)
- [Pre-Launch Gate](#pre-launch-gate)

## On-Call Questions (Start Here)

没有问题的 telemetry 就是噪音。在为任何东西添加 instrumentation 前：

- [ ] 已写下 on-call engineer 会询问该 feature 的 2–4 个问题
- [ ] 下方每个 signal 都映射到这些问题之一
- [ ] 每个问题都匹配到正确 signal type：metrics 说明有东西出了问题，traces 说明问题在**哪里**，logs 说明**为什么**

## Structured Logging

- [ ] Logs 是结构化的（JSON），并使用稳定 event names — 不是自由格式字符串
- [ ] 每条 log line 都携带 correlation/request ID，在系统边界生成或接受
- [ ] Correlation ID 在每个 outbound call 和 async boundary 上传播（HTTP headers、queue metadata）
- [ ] Log levels 一致：`error` = invariant broken，可能需要有人行动；`warn` = degraded but handled；`info` = significant business event；`debug` = 生产中关闭
- [ ] 任何 log line 中都没有 secrets、tokens、passwords 或未脱敏 PII（来自 `security-and-hardening` 的硬规则）
- [ ] Fields 使用 allowlist — 不记录完整 request/response bodies，不记录 auth headers
- [ ] External service calls 只记录 metadata：endpoint、status、latency、attempt count、sanitized identifiers
- [ ] 实际 log output 已 spot-check：是 structured fields，不是 `[object Object]`

## Metrics

- [ ] 每个 endpoint 和每个 external dependency 都有 **RED** instrumentation：Rate、Errors、Duration
- [ ] 每个 resource（queues、pools、hosts）都有 **USE** instrumentation：Utilization、Saturation、Errors
- [ ] Latency 是 histogram；p50/p95/p99 可查询 — 绝不是 average
- [ ] 所有 labels 都来自小而固定的集合（route template、status class、provider name）
- [ ] 没有无界 label values：没有 user IDs、tenant IDs、emails、raw URLs、request IDs 或 error message text
- [ ] Status codes 按 class 分组（`5xx`，不是 `503`）
- [ ] 每个 worker/queue 都跟踪 queue depth 和 processing duration

## Distributed Tracing

- [ ] OpenTelemetry（或等价工具）在 service startup 初始化，早于其他 imports
- [ ] 为 HTTP、gRPC 和 DB clients 启用 auto-instrumentation
- [ ] 每个 outbound call 都传播 trace context（W3C `traceparent`/`tracestate`），并从每个 inbound request 中提取
- [ ] Context 能跨 async boundaries 保留 — queue messages 携带 trace metadata
- [ ] Manual spans 只围绕有意义的内部工作单元，并带上 on-call 会用于过滤的 attributes
- [ ] 没有 secrets 或 PII 作为 span attributes
- [ ] 默认低比例 head-based sampling；如果可用 tail sampling，则保留 100% errors

## Alerting

- [ ] 每个 alert 都基于 symptom（error rate、p99 latency、queue age）— causes（CPU、disk、restarts）放到 dashboards，而不是 pagers
- [ ] 每个 alert 都可行动；“ignore it, it self-heals” alerts 应删除
- [ ] 每个 alert 都链接到 runbook — 至少三行：它意味着什么、第一条要运行的 query、escalation path
- [ ] Thresholds 和 durations 基于 SLO 或历史数据，而不是猜测
- [ ] 只有两种 severities：**page**（user-facing，立即行动）和 **ticket**（degradation，本周处理）
- [ ] 每个新 alert 都 test-fired 一次：它到达正确 channel，且 runbook link 可用
- [ ] 没有每天触发并被确认却不采取行动的 alerts

## Dashboards

- [ ] 存在 service health dashboard：error rate、latency p99、traffic、saturation
- [ ] Dependency health panel 显示每个 service 的 error rates 和 latency
- [ ] Dashboard 能回答 checklist 顶部的 on-call questions — 不是“除了答案之外什么都有”
- [ ] 默认 time range 合理（1h–6h，而不是 30d）

## Verify the Telemetry

Instrumentation 是代码；它也可能出错：

- [ ] 在 staging 中强制制造 error → 能通过 correlation ID 在 logs 中找到
- [ ] 发送 test traffic → metric series 以预期 labels 和合理 values 出现
- [ ] 在 tracing UI 中端到端跟踪一个 request → 没有 broken spans
- [ ] 一次 induced failure 仅凭 telemetry 被诊断出来，无需读取 source

## Pre-Launch Gate

在 feature ship 到 production 前，以下各项都为真：

- [ ] Structured logs 正在流入 log aggregator
- [ ] 每个新 endpoint 和 dependency 的 RED metrics 都可在 dashboards 中看到
- [ ] 至少配置一个 symptom-based alert，带 runbook，且已 test-fired
- [ ] 一个 request 可跨它触达的每个 service 被 trace
- [ ] On-call 知道 runbooks 在哪里

Launch-day monitoring sequence 和 rollback triggers 见 `shipping-and-launch` skill。
