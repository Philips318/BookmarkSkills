---
name: aws-resource-health-diagnose
description: 'Analyze AWS resource health, diagnose issues from CloudWatch logs and metrics, and create a remediation plan for identified problems.'
---
# AWS资源运行状况和问题诊断

此工作流程分析特定AWS资源，以评估其运行状况，使用CloudWatch日志和指标诊断潜在问题，并针对发现的任何问题制定全面的补救计划。

# #先决条件
—配置并认证AWS CLI
确定目标AWS资源（名称、类型和可选的region/account）
-在目标资源上启用CloudWatch日志记录和度量

##工作流程

###步骤1：获取AWS诊断最佳实践
获取`https://docs.aws.amazon.com/AmazonCloudWatch/latest/monitoring/`以获取监控和故障排除指南，从而为诊断方法提供信息。

步骤2：资源发现和识别
使用适合其类型的AWS CLI命令定位目标资源：```bash
# EC2
aws ec2 describe-instances --filters "Name=tag:Name,Values=<name>"
# Lambda
aws lambda get-function --function-name <name>
# RDS
aws rds describe-db-instances --db-instance-identifier <name>
# ECS
aws ecs describe-services --cluster <cluster> --services <name>
# ALB
aws elbv2 describe-load-balancers --names <name>
# DynamoDB
aws dynamodb describe-table --table-name <name>
# SQS
aws sqs get-queue-attributes --queue-url <url> --attribute-names All
# API Gateway
aws apigatewayv2 get-apis
```
如果找到多个匹配项，则提示用户指定region/account.第三步：健康状况评估
运行特定于服务的运行状况检查：```bash
# EC2
aws ec2 describe-instance-status --instance-ids <id>

# RDS
aws rds describe-db-instances --db-instance-identifier <name> \
  --query 'DBInstances[0].DBInstanceStatus'

# Lambda - error rate over 24h
aws cloudwatch get-metric-statistics --namespace AWS/Lambda \
  --metric-name Errors --dimensions Name=FunctionName,Value=<name> \
  --start-time $(date -u -d '24 hours ago' +%Y-%m-%dT%H:%M:%SZ) \
  --end-time $(date -u +%Y-%m-%dT%H:%M:%SZ) \
  --period 3600 --statistics Sum

# ECS
aws ecs describe-services --cluster <cluster> --services <name> \
  --query 'services[0].[status,runningCount,desiredCount,pendingCount]'
```
按服务类型划分的主要运行状况指标：
- **Lambda**：错误率，节流率，持续时间P99，并发执行
—**RDS**: CPU利用率、FreeStorageSpace、DatabaseConnections、ReadLatency/WriteLatency- **ECS**：运行vs期望的任务数，任务停止原因
- **ALB**: TargetResponseTime, HTTPCode_ELB_5XX_Count, UnHealthyHostCount
- **SQS**: approximate enumberofmessagesnotvisible, ApproximateAgeOfOldestMessage
—**DynamoDB**: ConsumedReadCapacityUnits, ThrottledRequests, SuccessfulRequestLatency

步骤4：日志和指标分析
查找日志组并运行CloudWatch Logs Insights查询：```bash
# Find log groups
aws logs describe-log-groups --log-group-name-prefix /aws/<service>/<name>

# Start a query (last 24h errors)
aws logs start-query \
  --log-group-name /aws/lambda/<name> \
  --start-time $(date -u -d '24 hours ago' +%s) \
  --end-time $(date -u +%s) \
  --query-string 'filter @message like /ERROR/ | stats count(*) as errorCount by bin(1h)'

# Get results
aws logs get-query-results --query-id <id>

# Lambda cold starts
aws logs start-query \
  --log-group-name /aws/lambda/<name> \
  --start-time $(date -u -d '24 hours ago' +%s) \
  --end-time $(date -u +%s) \
  --query-string 'filter @type = "REPORT" | filter @initDuration > 0 | stats count() as coldStarts by bin(1h)'

# RDS Performance Insights (if enabled)
aws pi get-resource-metrics \
  --service-type RDS --identifier db:<identifier> \
  --metric-queries '[{"Metric":"db.load.avg"}]' \
  --start-time $(date -u -d '24 hours ago' +%Y-%m-%dT%H:%M:%SZ) \
  --end-time $(date -u +%Y-%m-%dT%H:%M:%SZ) \
  --period-in-seconds 3600
```
识别：重复出现的错误模式、与部署的相关性（CloudTrail）、性能趋势、依赖失败。

步骤5：问题分类和根本原因分析
* * * *严重程度:
—**紧急**：业务不可用、数据丢失、安全事件
- **高**：性能下降，错误率bbb50 %，间歇性故障
- **中等**：警告，次优配置，轻微性能问题
- **低**：信息警报，优化机会**根本原因分类**：
—配置问题：设置错误，缺少envars， IAM权限拒绝
—资源约束：CPU/memory/disk限制，Lambda节流，RDS连接耗尽
—网络问题：安全组规则、VPC路由、DNS、acl
-应用程序问题：代码错误，内存泄漏，未处理的异常，缓慢的查询
-依赖问题：下游超时，SQS/SNS失败，外部API限制
—安全问题：KMS密钥问题、证书过期

###步骤6：生成补救计划

**立即行动**（紧急）：```bash
# Lambda throttling — increase reserved concurrency
aws lambda put-reserved-concurrency \
  --function-name <name> --reserved-concurrent-executions 100

# RDS connection exhaustion — reboot to reset connections
aws rds reboot-db-instance --db-instance-identifier <name>
```
**短期修复** (High/Medium)：配置调整，正确的大小，CloudWatch报警改进，IAM更正。

**长期改进**：针对弹性、预防性监控的架构更改，通过EventBridge启用AWS运行状况仪表板通知。

步骤7：报告和用户确认

目前发现:```
🏥 AWS Resource Health Assessment

📊 Resource Overview:
• Resource: [Name] ([Type])
• Status: [Healthy/Warning/Critical]
• Region: [Region] | Account: [Account ID]

🚨 Issues Identified:
• Critical: X | High: Y | Medium: Z | Low: N

🔍 Top Issues:
1. [Issue]: [Description] — Impact: [High/Medium/Low]
2. [Issue]: [Description] — Impact: [High/Medium/Low]

🛠️ Remediation: X immediate, Y short-term, Z long-term actions

❓ Proceed with detailed remediation plan? (y/n)
```
然后生成完整的降价报告，内容包括：运行状况指标、根本原因分析问题、使用AWS CLI命令分阶段修复步骤、CloudWatch警报建议和验证清单。

##错误处理
—**资源未找到**：请用户澄清name/region- **认证问题**:`aws configure`指南
—**权限不足**：列出需要的IAM操作（`logs:*`,`cloudwatch:*`,`pi:*`）
—**No Logs Available**：建议该资源类型开启CloudWatch日志
—**查询超时时间**：使用更短的时间窗口##成功标准
-✅准确评估了所有关键指标的资源运行状况
-✅所有已识别并按严重程度分类的重大问题
-✅主要问题的根本原因分析完成
-✅使用AWS CLI命令的可操作修复计划
-✅CloudWatch监控建议包括
-✅实现步骤包括验证和回滚过程