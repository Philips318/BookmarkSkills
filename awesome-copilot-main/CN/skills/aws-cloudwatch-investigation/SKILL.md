---
name: aws-cloudwatch-investigation
description: >
  Reusable investigation patterns for AWS CloudWatch: Logs Insights query templates,
  alarm-to-deployment correlation, blast-radius narrowing decision tree, and
  PromQL-style metric query patterns for structured incident triage.
---
# AWS CloudWatch调查技能

用于使用CloudWatch日志、度量和警报调查生产事件的可重用模式。这些模式被设计成在事件分类期间组合在一起。

---

模式1：日志洞察查询模板

错误尖峰检测

在一个时间窗口中，按错误类型分组查找最严重的错误：```
fields @timestamp, @message, @logStream
| filter @message like /(?i)(error|exception|fatal|critical)/
| stats count(*) as errorCount by bin(5m), @logStream
| sort errorCount desc
| limit 20
```
### P99按操作分解延迟

确定哪些操作导致了延迟峰值：```
fields @timestamp, @duration, operation
| filter ispresent(@duration)
| stats avg(@duration) as avgMs,
        pct(@duration, 50) as p50Ms,
        pct(@duration, 95) as p95Ms,
        pct(@duration, 99) as p99Ms,
        count(*) as invocations
  by operation
| sort p99Ms desc
| limit 15
```
Lambda冷启动检测

量化事故中冷启动的影响：```
fields @timestamp, @duration, @initDuration, @memorySize, @maxMemoryUsed
| filter ispresent(@initDuration)
| stats count(*) as coldStarts,
        avg(@initDuration) as avgInitMs,
        max(@initDuration) as maxInitMs,
        avg(@duration) as avgDurationMs
  by bin(5m)
| sort @timestamp desc
```
内存不足（OOM）检测

查找被内存压力杀死的Lambda函数或容器：```
fields @timestamp, @message, @logStream, @memorySize, @maxMemoryUsed
| filter @message like /Runtime exited|out of memory|OOMKilled|Cannot allocate memory|MemoryError/
| stats count(*) as oomEvents by @logStream, bin(10m)
| sort oomEvents desc
| limit 10
```
OOM之前的内存使用趋势：```
fields @timestamp, @maxMemoryUsed, @memorySize
| filter ispresent(@maxMemoryUsed)
| stats max(@maxMemoryUsed / @memorySize * 100) as peakMemPct,
        avg(@maxMemoryUsed / @memorySize * 100) as avgMemPct
  by bin(5m)
| sort @timestamp desc
```
###超时检测

查找达到配置超时的调用：```
fields @timestamp, @duration, @logStream, @requestId
| filter @message like /Task timed out/ or @duration > 28000
| stats count(*) as timeouts by @logStream, bin(5m)
| sort timeouts desc
```
---

模式2：告警历史到部署-事件关联

# # #过程

1. **获取告警转换时间** -记录告警进入alarm状态的准确时间戳。
2. **在[alarm_time - 30min， alarm_time]窗口内查询CloudTrail**中的部署相关事件：```
# CloudTrail Lake query for deployment events
SELECT eventTime, eventName, userIdentity.arn, requestParameters
FROM <event-data-store-id>
WHERE eventTime > '<alarm_time_minus_30m>'
  AND eventTime < '<alarm_time>'
  AND eventName IN (
    'UpdateFunctionCode', 'UpdateFunctionConfiguration',
    'UpdateService', 'CreateDeployment', 'RegisterTaskDefinition',
    'CreateChangeSet', 'ExecuteChangeSet',
    'StartPipelineExecution', 'PutImage'
  )
ORDER BY eventTime DESC
```
3. **关联标准** -如果：
—与告警对象相同的service/resource—告警切换前15分钟内完成
-部署者身份匹配CI/CD角色（不是应用热修复程序的人）

4. **加强相关性：**
—检查在前一个部署周期中是否存在相同的健康告警
—在同一窗口中验证没有其他环境更改（缩放事件，配置更改）
—查找同时启动的canary/synthetic监视器故障

输出格式```
Deploy Correlation:
  Event: UpdateFunctionCode
  Time: 2024-03-15T14:23:07Z (12 min before alarm)
  Actor: arn:aws:sts::123456789012:assumed-role/github-actions-deploy/session
  Resource: arn:aws:lambda:us-east-1:123456789012:function:payment-processor
  Correlation: STRONG — same resource, CI/CD actor, alarm was OK prior cycle
```
---

模式3：缩小爆炸半径决策树

使用这棵树系统地从最广泛到最具体地确定事件范围：```
START
  |
  v
[1] ACCOUNT — Which account(s) show the alarm?
  |  - Check: Are alarms firing in multiple accounts?
  |  - If yes → suspect shared service (SSO, networking, shared deployment pipeline)
  |  - If no → proceed to Region
  v
[2] REGION — Which region(s) are affected?
  |  - Check: Same alarm in other regions?
  |  - If multi-region → suspect global service (IAM, Route53, S3 global)
  |  - If single-region → proceed to Service
  v
[3] SERVICE — Which service namespace shows degradation?
  |  - Check CloudWatch namespace: AWS/Lambda, AWS/ECS, AWS/ApiGateway, etc.
  |  - If multiple services → suspect shared dependency (VPC, NAT, DNS, IAM)
  |  - If single service → proceed to Operation
  v
[4] OPERATION — Which API action or function is failing?
  |  - For Lambda: which function name?
  |  - For ECS: which service/task definition?
  |  - For API GW: which stage/resource/method?
  |  - If all operations → suspect service-level issue (throttling, quota)
  |  - If specific operation → proceed to Resource
  v
[5] RESOURCE — Which specific resource instance?
     - Function ARN, Task ID, DB instance identifier
     - This is your investigation target
     - Proceed to log and trace analysis scoped to this resource
```
共享依赖调查

当爆炸半径跨越多个服务时，按以下顺序进行调查：

1. **VPC/Networking** - NAT Gateway ErrorPortAllocation，丢包，DNS解析失败
2. **IAM/STS** - ThrottlingException on假设，令牌售卖延迟
3. **下游依赖-共享数据库，缓存或外部API
4. **部署管道** -从同一管道运行的跨服务同时部署
5. **AWS服务事件** -检查该区域的AWS运行状况指示板和服务运行状况

---

模式4:promql风格的度量查询模式

这些模式使用CloudWatch度量数学和GetMetricData来构建复合信号。将它们表示为仪表板或编程检索的度量查询。

错误率为百分比```
MetricDataQueries:
  - Id: errors
    MetricStat:
      Metric:
        Namespace: AWS/Lambda
        MetricName: Errors
        Dimensions: [{Name: FunctionName, Value: TARGET}]
      Period: 60
      Stat: Sum
  - Id: invocations
    MetricStat:
      Metric:
        Namespace: AWS/Lambda
        MetricName: Invocations
        Dimensions: [{Name: FunctionName, Value: TARGET}]
      Period: 60
      Stat: Sum
  - Id: error_rate
    Expression: "errors / invocations * 100"
    Label: "Error Rate %"
```
延迟异常检测（与基线相比）```
MetricDataQueries:
  - Id: current_p99
    MetricStat:
      Metric:
        Namespace: AWS/Lambda
        MetricName: Duration
        Dimensions: [{Name: FunctionName, Value: TARGET}]
      Period: 300
      Stat: p99
  - Id: baseline_p99
    MetricStat:
      Metric:
        Namespace: AWS/Lambda
        MetricName: Duration
        Dimensions: [{Name: FunctionName, Value: TARGET}]
      Period: 300
      Stat: p99
    # Use StartTime/EndTime set to same window last week
  - Id: anomaly_ratio
    Expression: "current_p99 / baseline_p99"
    Label: "Latency vs Baseline (ratio > 2 = anomaly)"
```
节流压力评分

将多个节流信号组合成单个压力指标：```
MetricDataQueries:
  - Id: lambda_throttles
    MetricStat:
      Metric: {Namespace: AWS/Lambda, MetricName: Throttles}
      Period: 60
      Stat: Sum
  - Id: api_gw_429s
    MetricStat:
      Metric: {Namespace: AWS/ApiGateway, MetricName: 4XXError, Dimensions: [{Name: ApiName, Value: TARGET}]}
      Period: 60
      Stat: Sum
  - Id: dynamo_throttles
    MetricStat:
      Metric: {Namespace: AWS/DynamoDB, MetricName: ThrottledRequests, Dimensions: [{Name: TableName, Value: TARGET}]}
      Period: 60
      Stat: Sum
  - Id: throttle_pressure
    Expression: "lambda_throttles + api_gw_429s + dynamo_throttles"
    Label: "Combined Throttle Pressure"
```
并发执行空间```
MetricDataQueries:
  - Id: concurrent
    MetricStat:
      Metric: {Namespace: AWS/Lambda, MetricName: ConcurrentExecutions}
      Period: 60
      Stat: Maximum
  - Id: headroom
    Expression: "1000 - concurrent"
    Label: "Remaining Concurrency (account limit 1000)"
```
---

模式5：事件时间线重建

# # #过程

通过合并来自多个来源的数据重建精确的时间线：

1. * *收集时间戳:* *

|源|查询|输出||--------|-------|--------|
| CloudWatch告警|告警历史API |状态转换次数|
| CloudWatch Metrics | GetMetricData 1分钟周期|第一个异常点|
| CloudWatch日志|日志洞察与`earliest(@timestamp)`|第一次错误发生|
| CloudTrail |按时间过滤的lookupeevents |Deployment/changeevents |
| AWS运行状况|描述事件| AWS端事件|

2. **建立时间线：**```
fields @timestamp, @message
| filter @message like /ERROR|WARN|timeout|refused|denied/
| stats earliest(@timestamp) as firstSeen, latest(@timestamp) as lastSeen, count(*) as occurrences
  by @message
| sort firstSeen asc
| limit 20
```
3. **识别序列：**```
Timeline:
  T-15m: CloudTrail — UpdateFunctionCode by CI/CD role
  T-12m: Logs — first error "Connection refused to payments-api.internal"
  T-10m: Metrics — Error count crosses 5/min threshold
  T-8m:  Alarm — PaymentProcessorErrors enters ALARM
  T-5m:  Metrics — p99 latency spikes to 28s (timeout)
  T-0:   Current — error rate at 45%, alarm still firing
```
4. **确定根事件** -所有症状之前最早的更改。从第一个症状回溯到最近的突变（部署、配置更改、扩展事件或外部依赖转移）。

# # #陷阱

—CloudWatch度量时间戳是周期结束。14:05的1分钟数据点涵盖14:04-14:05。
- CloudTrail事件最多可以有15分钟的交付延迟。使用`eventTime`，而不是摄入时间。
—日志组时间戳取决于agent/SDK刷新间隔。允许30-60秒的时钟偏差。
—告警状态变化具有内置的评估延迟（周期×评估周期）。实际的异常开始得更早。