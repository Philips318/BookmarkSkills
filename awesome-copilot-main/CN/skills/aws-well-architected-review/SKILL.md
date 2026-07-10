---
name: aws-well-architected-review
description: 'Perform an AWS Well-Architected Framework review of the current workload IaC and architecture, generating findings and GitHub issues for improvements.'
---
# AWS架构良好的审查

此工作流针对工作负载的IaC文件和部署的基础架构执行结构化的AWS良好架构框架（WAF）审查。它识别所有6个WAF支柱的风险，并创建GitHub问题来跟踪补救措施。

# #先决条件
—配置并认证AWS CLI
-存储库中存在的IaC文件（Terraform、CloudFormation、CDK或SAM）
- GitHub MCP服务器配置和认证

##工作流程

步骤1：加载架构良好的框架参考
获取当前AWS WAF最佳实践：- `https://docs.aws.amazon.com/wellarchitected/latest/framework/welcome.html`
-与工作负载类型相关的特定于支柱的镜头（无服务器、SaaS等）

###步骤2：发现IaC和建筑
扫描存储库中的IaC文件：
—地形：`**/*.tf`-CloudFormation/SAM:`**/*.yaml`,`**/*.json`（CFn模板）
- CDK:`lib/**/*.ts`,`bin/**/*.ts`,`cdk.json`确定正在使用的关键AWS服务（计算、数据、网络、安全、可观察性），并生成一个Mermaid架构图。

步骤3：逐个检查

####第一支柱：卓越运营
[]所有基础设施都定义为IaC（不需要手动修改控制台）
-[]所有资源采用一致的标签策略
—[]关键指标定义的CloudWatch告警
-[]自动部署管道（没有手动部署）
-[]启用CloudTrail审计日志
-[]有操作手册或操作文件####第二支柱：安全
—[]IAM角色使用最小特权策略（没有正当理由的`*`操作）
-[]在IaC或代码中没有硬编码凭据
-[]通过秘密管理器或SSM参数存储管理的秘密
—[]S3桶阻断了公共访问，启用了服务器端加密
—[]私有子网内的敏感资源
—[]安全组将入站限制在最小要求的ports/CIDRs-对敏感数据存储（RDS、EBS、S3、SQS、DynamoDB）启用KMS加密
- []SSL/TLS在所有终端强制执行（`enforceSSL: true`）
- [] GuardDuty enabled （`aws guardduty list-detectors`）
-[]在面向公众的api和CloudFront发行版上配置的AWS WAF
—[]S3关键桶启用MFA删除功能####支柱3：可靠性
-生产数据库的多az部署（RDS Multi-AZ, DynamoDB Global Tables）
—[]自动缩放为EC2/ECS配置了合适的策略
—[]S3版本和生命周期策略配置
—[]启用RDS自动备份，并保留适当的保留期
- [] DynamoDB启用PITR （Point-in-Time Recovery）
- [] Lambda、SQS、SNS配置的DLQ （Dead Letter Queues）
- [] Route 53配置DNS故障切换健康检查
- [] Lambda保留并发设置，以防止噪音邻居节流####支柱4：性能效率
[]正确大小的实例类型（Lambda内存，EC2类型，RDS类）
-使用Graviton/ARM实例（Lambda`arm64`, EC2 Graviton）
[]缓存实现（ElastiCache， DAX, CloudFront， API Gateway缓存）
- [] CloudFront用于全局静态内容分发
- [] Aurora Serverless或DynamoDB On-Demand可变负载模式
- [] Lambda Provisioned Concurrency用于延迟严重的同步路径####支柱5：成本优化
—[]EC2为稳定工作负载预留实例或节省计划
—[]S3生命周期策略将数据转移到更便宜的存储层
-采用Lambda`arm64`架构（成本降低20%）
- [] VPCS3/DynamoDB的端点，避免NAT网关费用
[]将gp2的EBS卷迁移到gp3（性能相同，价格便宜20%）
- []Development/test环境有自动关机计划
—[]已配置AWS预算和成本异常检测
—[]识别出未挂载的EBS卷和空闲的EC2实例

####支柱6：可持续性
-[]选择Graviton/ARM实例
- []Serverless/managed服务优先于永远在线的EC2
—[]S3生命周期策略减少不必要的长期数据存储
-[]自动伸缩配置，避免过度分配
-[]区域选择考虑AWS可再生能源承诺第四步：风险分类
对于每一个发现，分类如下：
- **高风险**：安全漏洞，单点故障，无backup/recovery- **中等风险**：可靠性次优，成本低，性能问题
- **低风险**：最佳实践偏差，小优化机会

###步骤5：用户确认```
🏗️ AWS Well-Architected Review Summary

📊 Review Results:
• IaC Files Analyzed: X
• AWS Services Identified: Y
• Total Findings: Z
  • High Risk: A (immediate action required)
  • Medium Risk: B (should address soon)
  • Low Risk: C (nice to have)

🔴 Top High Risk Findings:
1. [Pillar]: [Finding] — [Why it matters]
2. [Pillar]: [Finding] — [Why it matters]

💡 This will create Z individual GitHub issues + 1 EPIC issue.

❓ Proceed with creating GitHub issues? (y/n)
```
###步骤6：创建单个查找问题
贴上“架构良好”和支柱名称的标签（例如，“安全性”、“可靠性”）。

* *标题* *:`[WAF-<PILLAR>] [Brief Finding] — [Risk Level]`* *身体* *:```markdown
## 🏗️ Well-Architected Finding: [Brief Title]

**Pillar**: [Name] | **Risk Level**: [High/Medium/Low] | **Effort**: [Low/Medium/High]

### 📋 Description
[Clear explanation of the finding and why it matters]

### 🔧 Remediation

**IaC Fix** (preferred):
```hcl
# Terraform的例子
资源“aws_s3_bucket_server_side_encryption_configuration” example" {
Bucket = aws_s3_bucket.example.id
规则{    apply_server_side_encryption_by_default {
      sse_algorithm = "aws:kms"
    }
  }
}
```

**AWS CLI fallback**:
```bash
Aws s3api - put-bucket-encryption——bucket<name>\
——server-side-encryption-configuration{“规则”:[{“ApplyServerSideEncryptionByDefault”:{“SSEAlgorithm”:“aws:公里”}}]}”```

### 📚 AWS Reference
- [WAF Best Practice Link]
- [AWS Documentation Link]

### ✅ Validation
- [ ] Change implemented in IaC and deployed
- [ ] AWS Config rule passes (if applicable)
- [ ] Security Hub finding resolved (if applicable)

**Well-Architected Question**: [WAF question this maps to]
```
###步骤7：创建EPIC跟踪问题
贴上“架构良好”和“史诗”的标签。

* *标题* *:`[EPIC] AWS Well-Architected Review — X findings across 6 pillars`**主体**：执行摘要，包含支柱细分表（按支柱和风险级别查找计数）、美人鱼架构图、链接所有单独问题的优先级清单（高→中→低），以及成功标准：
-所有高风险发现均已解决
-中等调查结果已接受缓解计划
-在现有的CloudWatch告警或配置规则中没有回归

##错误处理
- **未找到IaC文件**：限制通过AWS CLI实时资源发现的审查，并注意差距
—** AWS权限不足**：列出审核所需的只读权限
- **GitHub创建失败**：将所有发现输出为格式化markdown到控制台##成功标准
-✅所有6个WAF支柱根据IaC和现场基础设施进行了审查
-✅所有调查结果按风险等级和支柱分类
-✅针对每个发现的可操作的补救步骤和IaC示例
-✅为团队跟踪创建的GitHub问题
-✅为EPIC上下文生成的架构图
-✅包括AWS文档参考