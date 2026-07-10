---
name: aws-cost-optimize
description: 'Analyze AWS resources used in the app (IaC files and/or resources in a target account/region) and optimize costs - creating GitHub issues for identified optimizations.'
---
# AWS成本优化

此工作流分析基础设施即代码（IaC）文件和AWS资源，以生成成本优化建议。它为每个优化机会创建单独的GitHub问题，加上一个EPIC问题来协调实施，从而能够有效地跟踪和执行成本节约计划。

# #先决条件
—AWS CLI configured and authenticated （`aws sts get-caller-identity`succeeded）
- GitHub MCP服务器配置和认证
-目标GitHub存储库标识
-部署AWS资源（IaC文件可选，但很有用）

##工作流程步骤1：获取AWS成本优化最佳实践
**措施**：在分析之前检索成本优化最佳实践
**工具**:`fetch`检索AWS文档
* * * *过程:
1. **加载最佳实践**：
—获取“`https://docs.aws.amazon.com/cost-management/latest/userguide/cost-optimization-best-practices.html`”
-获取AWS良好架构成本优化支柱摘要
-使用这些实践为后续分析和建议提供信息

###步骤2：发现AWS基础设施
**动作**：动态发现和分析AWS资源和配置
**Tools**: AWS CLI +本地文件系统访问
* * * *过程:
1. **发现账户和区域**：
—执行`aws sts get-caller-identity`命令确认帐户
—执行`aws configure get region`命令确定默认区域2. **资源发现**（每个地区）：
—EC2实例：`aws ec2 describe-instances --query 'Reservations[].Instances[].[InstanceId,InstanceType,State.Name,Tags]'`—RDS实例：`aws rds describe-db-instances --query 'DBInstances[].[DBInstanceIdentifier,DBInstanceClass,Engine,MultiAZ]'`—Lambda函数：`aws lambda list-functions --query 'Functions[].[FunctionName,Runtime,MemorySize,Architectures]'`- ECSclusters/services:`aws ecs list-clusters`然后`aws ecs describe-services`—S3桶：`aws s3api list-buckets --query 'Buckets[].Name'`—ElastiCache集群：`aws elasticache describe-cache-clusters`—NAT网关：`aws ec2 describe-nat-gateways`—负载均衡器：`aws elbv2 describe-load-balancers`3. * * * * IaC检测:
-扫描IaC文件：`**/*.tf`，`**/*.yaml`(CloudFormation/SAM),`**/*.json`(CloudFormation),`**/cdk.json`,`lib/**/*.ts`（CDK）
-解析资源定义以理解预期的配置
-不要使用应用程序代码文件-只有IaC文件作为真相的来源
—如果没有找到IaC文件：停止并向用户报告

步骤3：收集使用指标并验证当前成本
**行动**：收集利用数据，核实实际资源成本
**工具**:AWS CLI （CloudWatch、成本管理器）
* * * *过程:
1. **CloudWatch指标**（过去7天）：   ```bash
   # EC2 CPU utilization
   aws cloudwatch get-metric-statistics \
     --namespace AWS/EC2 --metric-name CPUUtilization \
     --dimensions Name=InstanceId,Value=<id> \
     --start-time $(date -u -d '7 days ago' +%Y-%m-%dT%H:%M:%SZ) \
     --end-time $(date -u +%Y-%m-%dT%H:%M:%SZ) \
     --period 3600 --statistics Average

   # Lambda duration
   aws cloudwatch get-metric-statistics \
     --namespace AWS/Lambda --metric-name Duration \
     --dimensions Name=FunctionName,Value=<name> \
     --start-time $(date -u -d '7 days ago' +%Y-%m-%dT%H:%M:%SZ) \
     --end-time $(date -u +%Y-%m-%dT%H:%M:%SZ) \
     --period 86400 --statistics Average,Maximum
   ```
2. **AWS成本管理器**：   ```bash
   aws ce get-cost-and-usage \
     --time-period Start=$(date -u -d '30 days ago' +%Y-%m-%d),End=$(date -u +%Y-%m-%d) \
     --granularity MONTHLY --metrics BlendedCost \
     --group-by Type=DIMENSION,Key=SERVICE
   ```
3. **计算基线指标**:CPU/Memory平均值，Lambda调用率，数据传输模式，以及现实的当前每月总数。

步骤4：生成成本优化建议
**行动**：分析资源，识别优化机会
* * * *过程:
1. **应用优化模式**：

* *计算* *:
- EC2：基于CPU/memory的合适尺寸（<20%平均值→缩小尺寸），将按需转换为储蓄计划，迁移到Graviton/ARM（最多便宜40%）
- Lambda：减少空闲函数的内存，切换到`arm64`（便宜20%）
-ECS/EKS：为dev/batch工作负载使用Fargate Spot

数据库* * * *:
- RDS：适当大小的实例类，将单az转换为dev，使用Aurora Serverless v2进行可变负载
—DynamoDB: Switch provisioning→On-Demand，以应对不可预测的流量
—ElastiCache：根据内存利用率选择合适大小的节点类型* *存储* *:
—S3：生命周期策略（30d后标准→标准- ia→90d后冰川），启用智能分级
- EBS：删除未连接的卷，将gp2转换为gp3（性能相同，便宜20%）

* *网* *:
—非生产环境下合并NAT网关
—“S3/DynamoDB”使用VPC端点，避免NAT网关费用

2. **计算优先级评分**：   ```
   Priority Score = (Value Score × Monthly Savings) / (Risk Score × Implementation Days)
   High: Score > 20 | Medium: Score 5-20 | Low: Score < 5
   ```
###步骤5：用户确认
**动作**：在创建GitHub问题之前提交总结并获得批准```
🎯 AWS Cost Optimization Summary

📊 Analysis Results:
• Total Resources Analyzed: X
• Current Monthly Cost: $X
• Potential Monthly Savings: $Y
• Optimization Opportunities: Z
• High Priority Items: N

🏆 Recommendations:
1. [Resource]: [Current] → [Target] = $X/month savings - [Risk] | [Effort]
...

💡 This will create Y individual GitHub issues + 1 EPIC issue.

❓ Proceed with creating GitHub issues? (y/n)
```
等待用户确认后再继续。

步骤6：创建单独的优化问题
**行动**：为每个优化创建单独的GitHub问题。标签上标有“成本优化”（绿色）和“aws”（橙色）。

* *标题* *:`[COST-OPT] [Resource Type] - [Brief Description] - $X/month savings`* *身体* *:```markdown
## 💰 Cost Optimization: [Brief Title]

**Monthly Savings**: $X | **Risk Level**: [Low/Medium/High] | **Effort**: X days

### 📋 Description
[Clear explanation of the optimization and why it's needed]

### 🔧 Implementation

**IaC Files Detected**: [Yes/No]

```bash
# IaC修改（首选）或AWS CLI回退```

### 📊 Evidence
- Current Configuration: [details]
- Usage Pattern: [evidence from CloudWatch]
- Cost Impact: $X/month → $Y/month

### ✅ Validation Steps
- [ ] Test in non-production environment
- [ ] Verify no performance degradation via CloudWatch
- [ ] Confirm cost reduction in AWS Cost Explorer

### ⚠️ Risks & Considerations
- [Risk and mitigation]

**Priority Score**: X | **Value**: X/10 | **Risk**: X/10
```
###步骤7：创建EPIC协调问题
**动作**：创建主跟踪问题。标上“成本优化”（绿色）、“aws”（橙色）、“史诗”（紫色）。

* *标题* *:`[EPIC] AWS Cost Optimization Initiative - $X/month potential savings`**正文**：带有account/region细节的执行摘要，当前资源的美人鱼架构图，连接所有单个问题的优先级清单（高→中→低），进度跟踪和成功标准（>实现了估计节省的80%，没有性能下降）。

##错误处理
—**AWS鉴权失败**：通过`aws configure`—**未找到资源**：创建有关AWS资源部署的信息问题
—**权限不足**：列出所需的IAM只读权限
- **GitHub创建失败**：输出格式化的建议控制台
—**成本资源管理器未启用**：引导用户在AWS控制台中启用##成功标准
-✅根据实际配置和AWS定价验证所有成本估算
-✅为每个优化创建的单个GitHub问题
-✅EPIC问题提供全面的协调和跟踪
-✅所有建议都包含特定的AWS CLI或IaC命令
-✅创建问题前获取用户确认信息