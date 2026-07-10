# AWS云开发插件

全面的AWS云开发工具，包括基础设施即代码、无服务器功能、架构模式和用于构建可扩展云应用程序的成本优化。

# #安装```bash
# Using Copilot CLI
copilot plugin install aws-cloud-development@awesome-copilot
```
包含的内容

###命令（斜杠命令）

|命令|描述||---------|-------------|
|`/aws-cloud-development:aws-cost-optimize`|分析应用程序中使用的AWS资源（目标account/region中的IaC文件and/or资源）并优化成本-为确定的优化创建GitHub问题。|
|`/aws-cloud-development:aws-resource-health-diagnose`|分析AWS资源运行状况，从CloudWatch日志和指标中诊断问题，并为已识别的问题创建修复计划。|
|`/aws-cloud-development:aws-resource-query`|使用自然语言查询任意AWS资源（EC2、S3、RDS、Lambda、VPC、IAM、Secrets Manager等）。严格只读-没有写或删除。|
|`/aws-cloud-development:aws-well-architected-review`|对当前工作负载IaC和架构执行AWS良好架构框架审查，生成结果和GitHub问题以进行改进。|

# # #代理

|代理|描述||-------|-------------|
|`aws-principal-architect`|使用AWS良好架构框架原则和AWS最佳实践提供专业的AWS首席架构师指导。|
|`aws-serverless-architect`|提供专注于事件驱动架构、Lambda、API Gateway和无服务器最佳实践的专业AWS无服务器架构师指导。|
|`terraform-aws-planning`|作为AWS平台基础设施即代码任务的实施计划者。|
|`terraform-aws-implement`|作为AWS Terraform基础设施和代码编码专家，为AWS资源创建和审查Terraform。|

# #源

这个插件是[Awesome Copilot]（https://github.com/github/awesome-copilot）的一部分，这是一个社区驱动的GitHub Copilot扩展集合。

# #许可证

麻省理工学院