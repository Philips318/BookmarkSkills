---
name: observability-and-instrumentation
description: 对代码进行仪表化，使生产行为可见且可诊断。用于添加日志、指标、追踪或告警。用于发布任何将在生产中运行、并且你需要证据证明它有效的功能。用于报告生产问题但现有数据无法说明发生了什么时。
---

# 可观测性与仪表化

## 概述

无法观测的代码，就是无法运维的代码。可观测性是从外部用代码发出的遥测数据回答“系统正在做什么，为什么？”的能力。仪表化不是发布后的附加项，它要和功能一起编写，就像测试一样。如果功能发布时没有 telemetry，第一个用户报告的 bug 就会变成考古，而不是一次查询。

## 何时使用

- 构建任何会在生产中运行的功能
- 添加新的 service、endpoint、background job 或外部集成
- 生产事故诊断时间过长（“we couldn't tell what happened”）
- 设置或审查告警规则
- 审查添加 I/O、重试、队列或跨服务调用的 PR

**不用于：**
- 诊断正在发生的故障：使用 `debugging-and-error-recovery` 技能（可观测性会让该技能下次更快）
- 剖析和优化已测量到的缓慢：使用 `performance-optimization` 技能
- 发布日监控清单和回滚触发器：见 `shipping-and-launch` 技能；本技能覆盖供它们使用的仪表化

## 流程

### 1. 在仪表化前定义“正常工作”

没有问题的 telemetry 是噪音。在添加任何仪表化之前，写下 on-call 工程师会针对这个功能提出的 2-4 个问题：

```
FEATURE: checkout payment retry
QUESTIONS ON-CALL WILL ASK:
1. What fraction of payments succeed on first attempt vs after retry?
2. When a payment fails permanently, why? (provider error? timeout? validation?)
3. Is the payment provider slower than usual?
→ Every signal below must help answer one of these.
```

如果你说不出这些问题，就还没准备好做仪表化；你会记录一切，却什么也学不到。

### 2. 为每个问题选择正确的信号

| 信号 | 回答什么 | 成本特征 | 示例 |
|---|---|---|---|
| **结构化日志** | “这个具体案例发生了什么？” | 按事件计费；随流量增长 | 带 provider error code 的 `payment_failed` |
| **指标** | “整体上多频繁/多快？” | 每个 series 固定；查询便宜 | provider 调用的 p99 latency |
| **追踪** | “跨服务时间花在哪里？” | 按请求；通常采样 | 一个慢 checkout，按 hop 拆解 |

经验法则：metrics 告诉你**有**问题，traces 告诉你问题在**哪里**，logs 告诉你**为什么**。

### 3. 结构化日志

记录事件，而不是散文。每条日志都是带稳定 event 名称和机器可读字段的 JSON 对象：

```typescript
// BAD: string interpolation — unqueryable, inconsistent
logger.info(`Payment ${id} failed for user ${userId} after ${n} retries`);

// GOOD: stable event name + structured fields
logger.warn({
  event: 'payment_failed',
  paymentId: id,
  provider: 'stripe',
  errorCode: err.code,
  attempt: n,
}, 'payment failed');
```

**日志级别：一致使用它们：**

| 级别 | 含义 | On-call 动作 |
|---|---|---|
| `error` | 不变量被破坏；可能需要有人行动 | 调查 |
| `warn` | 降级但已处理（重试成功、使用 fallback） | 观察趋势 |
| `info` | 重要业务事件（下单、job 完成） | 无 |
| `debug` | 诊断细节 | 生产默认关闭 |

**Correlation IDs 是强制的。** 在系统边界生成（或接受）request ID，并把它附加到每条日志、span 和 outbound call。没有它，你无法从交错日志中重建单个请求：

```typescript
// Express: child logger per request, ID propagated downstream
app.use((req, res, next) => {
  req.id = req.headers['x-request-id'] ?? crypto.randomUUID();
  req.log = logger.child({ requestId: req.id });
  res.setHeader('x-request-id', req.id);
  next();
});
```

**永远不要记录 secrets、tokens、passwords 或完整 PII。** 这是 `security-and-hardening` 技能中的硬规则：telemetry pipelines 是典型的数据泄漏路径。使用 allowlist 字段，不要记录整个 request body。

### 4. 指标

对于 request-driven services，在每个 endpoint 和每个外部依赖上仪表化 **RED**：**R**ate（requests/sec）、**E**rrors（failure rate）、**D**uration（latency histogram，不是 average）。对于资源（queues、pools、hosts），使用 **USE**：**U**tilization、**S**aturation、**E**rrors。

与 tracing 一样，vendor-neutral 路径是 OpenTelemetry metrics API（与步骤 5 使用相同 SDK 和 context）。下面示例使用 Prometheus 的 `prom-client`，这是常见 backend 选择之一，不是唯一选择；RED/USE 和 cardinality 规则完全相同。

```typescript
import { Histogram } from 'prom-client';

const httpDuration = new Histogram({
  name: 'http_request_duration_seconds',
  help: 'HTTP request duration',
  labelNames: ['method', 'route', 'status_class'],  // '2xx', not '200'
  buckets: [0.05, 0.1, 0.25, 0.5, 1, 2.5, 5],
});
```

**Cardinality 是失败模式。** 每一种唯一 label 组合都是单独的 time series。Labels 必须来自小而固定的集合（route template、status class、provider name）。永远不要把 user IDs、raw URLs、error messages 或其他无界值用作 labels，这些属于 logs 和 traces。

```
OK as label:    route="/api/tasks/:id"   status_class="5xx"   provider="stripe"
NEVER a label:  user_id, email, request_id, full URL, error message text
```

永远不要跟踪平均值，始终跟踪百分位：average 会隐藏那 1% 体验很差的用户。使用 histograms 并读取 p50/p95/p99。

### 5. 分布式追踪

使用 OpenTelemetry。它是 vendor-neutral 标准，auto-instrumentation 以接近零代码覆盖 HTTP、gRPC 和常见 DB clients：

```typescript
// tracing.ts — must be imported before anything else
import { NodeSDK } from '@opentelemetry/sdk-node';
import { getNodeAutoInstrumentations } from '@opentelemetry/auto-instrumentations-node';

const sdk = new NodeSDK({
  serviceName: 'checkout-service',
  instrumentations: [getNodeAutoInstrumentations()],
});
sdk.start();
```

只在有意义的内部工作单元周围添加 manual spans（例如 `applyDiscounts`、`chargeProvider`），并附加 on-call 会用来过滤的 attributes。跨每个 async boundary 传播 context，包括 HTTP headers、queue message metadata，否则 trace 会在断点处死亡。默认使用低比例 head-based sampling；如果 backend 支持 tail sampling，则保留 100% errors。

### 6. 告警

对**用户能感受到的症状**告警，而不是对原因告警：

```
SYMPTOM (page-worthy):           CAUSE (dashboard, not a page):
error rate > 1% for 5 min        CPU at 85%
p99 latency > 2s                 one pod restarted
queue age > 10 min               disk at 70%
```

基于原因的告警会在没有问题时触发，并错过你没预测到的失败。基于症状的告警会在用户受伤时准确触发，无论原因是什么。

你创建的每个告警都必须满足：

1. **必须可行动。** 如果响应是“忽略它，它会自愈”，删除这个告警。
2. **链接到 runbook**，哪怕只有三行：它是什么意思、先运行哪个查询、升级路径。
3. **有阈值和持续时间**，由 SLO 或历史数据证明，而不是猜测。
4. 只使用两个严重级别：**page**（面向用户，立刻行动）和 **ticket**（降级，本周处理）。第三层会变成噪音，训练人们忽略所有东西。

### 7. 验证 telemetry 本身

仪表化也是代码，可能是错的。在宣布完成前，触发路径并查看实际输出：

- 在 staging 强制一个错误 → 用 `requestId` 在日志中找到它，确认字段是结构化的（不是 `[object Object]`）
- 发送测试流量 → 确认 metric series 以预期 labels 和合理值出现
- 在 tracing UI 中跟踪一个跨服务请求 → 没有断裂 spans
- 触发每个新告警一次（临时降低阈值）→ 确认它到达正确渠道，且 runbook 链接有效

## 常见合理化借口

| 合理化借口 | 现实 |
|---|---|
| “等它工作了我再加 logging” | “之后”会变成“第一次事故之后”，那是发现自己失明的最昂贵时刻。边构建边仪表化。 |
| “更多日志 = 更多可观测性” | 非结构化噪音会让事故更慢，而不是更快。三条可查询事件胜过三百行散文。 |
| “console.log 现在够用” | 非结构化输出无法过滤、关联或告警。结构化 logger 一次只多花五分钟。 |
| “出问题时看 dashboard 就行” | 没有从明确问题出发的 dashboard，会展示答案以外的一切。从 on-call 问题开始。 |
| “给所有重要东西加告警，之后再调” | 嘈杂 pager 会训练人忽略它。调优永远不会发生；真正的 page 会被错过。 |
| “把 User ID 作为 metric label 更好调试” | 它也会让 metrics backend 崩掉。高基数查找属于 logs 和 traces。 |
| “两个服务用 tracing 太夸张” | 两个服务就已经有跨服务 latency 问题是 logs 无法回答的。Auto-instrumentation 让成本微不足道。 |

## 危险信号

- 带 retries、queues 或 external calls 的 feature PR 没有任何新 telemetry
- 用字符串插值构建日志行，而不是结构化字段
- 没有 correlation/request ID，每条日志都是孤儿
- Metrics 使用 user IDs、raw URLs 或 error message text 作为 labels（cardinality bomb）
- 用 average 跟踪 latency，没有 percentiles
- 告警每天触发并被确认但没有行动
- 用户侧 error rate 未监控，却对 causes（CPU、memory）给人类 page
- Secrets、tokens 或完整 request bodies 出现在日志中
- “It works on my machine” 是生产功能健康的唯一证据

## 验证

仪表化一个功能后，确认：

- [ ] 已写下该功能的 on-call 问题，并且每个 signal 都映射到其中一个问题
- [ ] 所有 log output 都是结构化的（JSON），每条都有稳定 event name 和 correlation ID
- [ ] 任意日志行中都没有 secrets、tokens 或未脱敏 PII（抽查实际输出）
- [ ] 每个新 endpoint 和每个外部依赖都有 RED metrics，并使用有界 label sets
- [ ] Latency 是 histogram；p95/p99 可查询
- [ ] 在 tracing UI 中可以端到端跟踪单个请求，没有断裂 spans
- [ ] 每个新告警都是 symptom-based，有 runbook 链接，并已测试触发一次
- [ ] 在 staging 中诱发的故障，只通过 telemetry 就能定位，无需阅读源码

有关这份清单的速览版本，包括预发布 instrumentation gate，请参阅 `references/observability-checklist.md`。
