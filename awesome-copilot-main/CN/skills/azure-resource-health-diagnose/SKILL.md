---
name: azure-resource-health-diagnose
description: 'Analyze Azure resource health, diagnose issues from logs and telemetry, and create a remediation plan for identified problems.'
---
# Azure资源运行状况和问题诊断

此工作流分析特定Azure资源以评估其运行状况，使用日志和遥测数据诊断潜在问题，并针对发现的任何问题制定全面的补救计划。

# #先决条件
-配置并验证Azure MCP服务器
-确定目标Azure资源（名称和可选资源group/subscription）
—生成logs/telemetry时，资源必须已部署并运行
-使用Azure MCP工具（`azmcp-*`），而不是直接使用Azure CLI

##工作流程###步骤1：获取Azure最佳实践
**操作**：检索诊断和故障排除最佳实践
**工具**:Azure MCP最佳实践工具
* * * *过程:
1. **加载最佳实践**：
-执行Azure最佳实践工具以获取诊断指南
—关注运行状况监视、日志分析和问题解决模式
-使用这些实践为诊断方法和补救建议提供信息

步骤2：资源发现和识别
**操作**：定位并识别目标Azure资源
**工具**:Azure MCP工具+ Azure CLI回退
* * * *过程:
1. * * * *资源查找:
-如果只提供资源名称：使用`azmcp-subscription-list`搜索订阅
—使用`az resource list --name <resource-name>`查找匹配的资源
—如果匹配到多个，提示用户指定subscription/resource组
-收集详细的资源信息：     - Resource type and current status
     - Location, tags, and configuration
     - Associated services and dependencies
2. **资源类型检测**
-识别资源类型以确定适当的诊断方法；     - **Web Apps/Function Apps**: Application logs, performance metrics, dependency tracking
     - **Virtual Machines**: System logs, performance counters, boot diagnostics
     - **Cosmos DB**: Request metrics, throttling, partition statistics
     - **Storage Accounts**: Access logs, performance metrics, availability
     - **SQL Database**: Query performance, connection logs, resource utilization
     - **Application Insights**: Application telemetry, exceptions, dependencies
     - **Key Vault**: Access logs, certificate status, secret usage
     - **Service Bus**: Message metrics, dead letter queues, throughput
第三步：健康状况评估
**措施**：评估当前资源运行状况和可用性
**工具**:Azure MCP监控工具+ Azure CLI
* * * *过程:
1. **基本健康检查**：
—查看资源发放状态和运行状态
-验证服务的可用性和响应能力
-检查最近的部署或配置更改
-评估当前资源利用率（CPU，内存，存储等）

2. **特定服务的健康指标**：
- **Web应用**:HTTP响应代码，响应时间，正常运行时间
—**数据库**：连接成功率、查询性能、死锁
—**存储**：可用性百分比、请求成功率、延迟
- ** vm **：启动诊断，来宾操作系统指标，网络连接
- **功能**：执行成功率、持续时间、出错频率步骤4：日志和遥测分析
**行动**：分析日志和遥测以识别问题和模式
**工具**：用于日志分析查询的Azure MCP监控工具
* * * *过程:
1. **查找监测源**：
—使用`azmcp-monitor-workspace-list`标识Log Analytics工作空间
-定位与资源关联的Application Insights实例
-使用`azmcp-monitor-table-list`识别相关日志表

2. **执行诊断查询**：
根据资源类型使用`azmcp-monitor-log-query`和有针对性的KQL查询：

**一般误差分析**：   ```kql
   // Recent errors and exceptions
   union isfuzzy=true 
       AzureDiagnostics,
       AppServiceHTTPLogs,
       AppServiceAppLogs,
       AzureActivity
   | where TimeGenerated > ago(24h)
   | where Level == "Error" or ResultType != "Success"
   | summarize ErrorCount=count() by Resource, ResultType, bin(TimeGenerated, 1h)
   | order by TimeGenerated desc
   ```
* * * *性能分析:   ```kql
   // Performance degradation patterns
   Perf
   | where TimeGenerated > ago(7d)
   | where ObjectName == "Processor" and CounterName == "% Processor Time"
   | summarize avg(CounterValue) by Computer, bin(TimeGenerated, 1h)
   | where avg_CounterValue > 80
   ```
* * * *特定于应用程序的查询:   ```kql
   // Application Insights - Failed requests
   requests
   | where timestamp > ago(24h)
   | where success == false
   | summarize FailureCount=count() by resultCode, bin(timestamp, 1h)
   | order by timestamp desc
   
   // Database - Connection failures
   AzureDiagnostics
   | where ResourceProvider == "MICROSOFT.SQL"
   | where Category == "SQLSecurityAuditEvents"
   | where action_name_s == "CONNECTION_FAILED"
   | summarize ConnectionFailures=count() by bin(TimeGenerated, 1h)
   ```
3. * *模式识别* *:
—识别重复出现的错误模式或异常
—将错误与部署时间或配置更改关联起来
—分析性能趋势和退化模式
-查找依赖失败或外部服务问题

步骤5：问题分类和根本原因分析
**措施**：对已发现的问题进行分类并确定根本原因
* * * *过程:
1. * * * *问题分类:
- **紧急**：服务不可用，数据丢失，安全漏洞
- **高**：性能下降，间歇性故障，高错误率
- **中等**：警告，次优配置，轻微性能问题
- **低**：信息警报，优化机会2. **根本原因分析**
- **配置问题**：不正确的设置，缺少依赖项
- **资源约束**:CPU/memory/disk限制，throttling
- **网络问题**：连接问题，DNS解析，防火墙规则
- **应用程序问题**：代码错误，内存泄漏，低效的查询
—**外部依赖**：第三方服务失败，API限制
- **安全问题**：认证失败、证书过期

3. * * * *影响评估:
-确定业务影响和受影响的users/systems-评估数据完整性和安全影响
-评估恢复时间目标和优先级###步骤6：生成补救计划
**行动**：制定一个全面的计划来解决已发现的问题
* * * *过程:
1. **立即行动**（关键问题）：
-紧急修复以恢复服务可用性
-临时解决方案，以减轻影响
-复杂问题的升级程序

2. **短期修复** （High/Medium问题）：
—配置调整和资源扩展
—应用程序更新和补丁
-监控和警报改进

3. **长期改进**（所有问题）：
-架构更改以提高弹性
-预防措施和加强监测
-文件和流程改进

4. * * * *的实现步骤:
-使用特定Azure CLI命令对操作项进行优先级排序
-测试和验证程序
—每次变更的回滚计划
-监控以验证问题的解决步骤7：用户确认和报告生成
**行动**：提出调查结果并获得补救措施的批准
* * * *过程:
1. **显示健康评估摘要**：   ```
   🏥 Azure Resource Health Assessment
   
   📊 Resource Overview:
   • Resource: [Name] ([Type])
   • Status: [Healthy/Warning/Critical]
   • Location: [Region]
   • Last Analyzed: [Timestamp]
   
   🚨 Issues Identified:
   • Critical: X issues requiring immediate attention
   • High: Y issues affecting performance/reliability  
   • Medium: Z issues for optimization
   • Low: N informational items
   
   🔍 Top Issues:
   1. [Issue Type]: [Description] - Impact: [High/Medium/Low]
   2. [Issue Type]: [Description] - Impact: [High/Medium/Low]
   3. [Issue Type]: [Description] - Impact: [High/Medium/Low]
   
   🛠️ Remediation Plan:
   • Immediate Actions: X items
   • Short-term Fixes: Y items  
   • Long-term Improvements: Z items
   • Estimated Resolution Time: [Timeline]
   
   ❓ Proceed with detailed remediation plan? (y/n)
   ```
2. **生成详细报告**：   ```markdown
   # Azure Resource Health Report: [Resource Name]
   
   **Generated**: [Timestamp]  
   **Resource**: [Full Resource ID]  
   **Overall Health**: [Status with color indicator]
   
   ## 🔍 Executive Summary
   [Brief overview of health status and key findings]
   
   ## 📊 Health Metrics
   - **Availability**: X% over last 24h
   - **Performance**: [Average response time/throughput]
   - **Error Rate**: X% over last 24h
   - **Resource Utilization**: [CPU/Memory/Storage percentages]
   
   ## 🚨 Issues Identified
   
   ### Critical Issues
   - **[Issue 1]**: [Description]
     - **Root Cause**: [Analysis]
     - **Impact**: [Business impact]
     - **Immediate Action**: [Required steps]
   
   ### High Priority Issues  
   - **[Issue 2]**: [Description]
     - **Root Cause**: [Analysis]
     - **Impact**: [Performance/reliability impact]
     - **Recommended Fix**: [Solution steps]
   
   ## 🛠️ Remediation Plan
   
   ### Phase 1: Immediate Actions (0-2 hours)
   ```bash
#关键修复恢复服务
[Azure CLI命令和解释]   ```
   
   ### Phase 2: Short-term Fixes (2-24 hours)
   ```bash
性能和可靠性改进
[Azure CLI命令和解释]   ```
   
   ### Phase 3: Long-term Improvements (1-4 weeks)
   ```bash
#建筑和预防措施
[Azure CLI命令和配置更改]   ```
   
   ## 📈 Monitoring Recommendations
   - **Alerts to Configure**: [List of recommended alerts]
   - **Dashboards to Create**: [Monitoring dashboard suggestions]
   - **Regular Health Checks**: [Recommended frequency and scope]
   
   ## ✅ Validation Steps
   - [ ] Verify issue resolution through logs
   - [ ] Confirm performance improvements
   - [ ] Test application functionality
   - [ ] Update monitoring and alerting
   - [ ] Document lessons learned
   
   ## 📝 Prevention Measures
   - [Recommendations to prevent similar issues]
   - [Process improvements]
   - [Monitoring enhancements]
   ```
##错误处理
- **未找到资源**：对资源name/location规格提供指导
- **身份验证问题**：指导用户完成Azure身份验证设置
—**权限不足**：列出访问资源所需的RBAC角色
—**无日志可用**：建议开启诊断设置，等待数据
—**查询超时时间**：将分析分解为更小的时间窗口
- **特定于服务的问题**：提供一般的健康评估，并指出局限性

##成功标准
-✅准确评估资源健康状态
-✅所有重大问题的识别和分类
-✅主要问题的根本原因分析完成
-✅提供具体步骤的可行补救计划
-✅包括监测和预防建议
-✅根据业务影响明确问题的优先级
-✅实现步骤包括验证和回滚过程