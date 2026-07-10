---
name: az-cost-optimize
description: 'Analyze Azure resources used in the app (IaC files and/or resources in a target rg) and optimize costs - creating GitHub issues for identified optimizations.'
---
# Azure成本优化

此工作流分析基础架构即代码（IaC）文件和Azure资源，以生成成本优化建议。它为每个优化机会创建单独的GitHub问题，加上一个EPIC问题来协调实施，从而能够有效地跟踪和执行成本节约计划。

# #先决条件
-配置并验证Azure MCP服务器
- GitHub MCP服务器配置和认证
-目标GitHub存储库标识
-部署Azure资源（IaC文件可选但很有用）
-在可用的情况下，使用Azure MCP工具（`azmcp-*`）而不是直接使用Azure CLI

##工作流程###步骤1：获取Azure最佳实践
**措施**：在分析之前检索成本优化最佳实践
**工具**:Azure MCP最佳实践工具
* * * *过程:
1. **加载最佳实践**：
-执行`azmcp-bestpractices-get`以获得一些最新的Azure优化指南。这可能不会涵盖所有场景，但提供了一个基础。
-尽可能使用这些实践为后续分析和建议提供信息
-在优化建议中引用最佳实践，可以来自MCP工具输出或一般Azure文档###步骤2：发现Azure基础架构
**动作**：动态发现和分析Azure资源和配置
**工具**:Azure MCP工具+ Azure CLI回退+本地文件系统访问
* * * *过程:
1. * *资源发现* *:
—执行`azmcp-subscription-list`查找可用的订阅
—执行命令`azmcp-group-list --subscription <subscription-id>`，查找资源组
-获取相关组内所有资源的列表；     - Use `az resource list --subscription <id> --resource-group <name>`
—对于每种资源类型，尽可能先使用MCP工具，然后再使用CLI回退：     - `azmcp-cosmos-account-list --subscription <id>` - Cosmos DB accounts
     - `azmcp-storage-account-list --subscription <id>` - Storage accounts  
     - `azmcp-monitor-workspace-list --subscription <id>` - Log Analytics workspaces
     - `azmcp-keyvault-key-list` - Key Vaults
     - `az webapp list` - Web Apps (fallback - no MCP tool available)
     - `az appservice plan list` - App Service Plans (fallback)
     - `az functionapp list` - Function Apps (fallback)
     - `az sql server list` - SQL Servers (fallback)
     - `az redis list` - Redis Cache (fallback)
     - ... and so on for other resource types
2. * * * * IaC检测:
—使用`file_search`扫描IaC文件：“**/*。二头肌”、“* * / *。“**/main.json”， “**/*template*.json”
-解析资源定义以理解预期的配置
-与已发现的资源进行比较，找出差异
-注意IaC文件的存在，以便以后提出实施建议
-不要使用存储库中的任何其他文件，只能使用IaC文件。使用其他文件是不允许的，因为它不是真相的来源。
—如果没有找到IaC文件，则停止并向用户报告没有找到IaC文件。

3. 配置分析* * * *:
—提取每个资源的当前sku、层级和设置
-识别资源关系和依赖关系
-映射可用的资源利用模式步骤3：收集使用指标并验证当前成本
**行动**：收集利用数据并验证实际资源成本
**工具**:Azure MCP监控工具+ Azure CLI
* * * *过程:
1. **查找监测源**：
-使用`azmcp-monitor-workspace-list --subscription <id>`来查找日志分析工作区
—使用“`azmcp-monitor-table-list --subscription <id> --workspace <name> --table-type "CustomLog"`”发现可用数据

2. **执行用法查询**：
-使用`azmcp-monitor-log-query`与这些预定义查询：     - Query: "recent" for recent activity patterns
     - Query: "errors" for error-level logs indicating issues
-对于自定义分析，使用KQL查询：   ```kql
   // CPU utilization for App Services
   AppServiceAppLogs
   | where TimeGenerated > ago(7d)
   | summarize avg(CpuTime) by Resource, bin(TimeGenerated, 1h)
   
   // Cosmos DB RU consumption  
   AzureDiagnostics
   | where ResourceProvider == "MICROSOFT.DOCUMENTDB"
   | where TimeGenerated > ago(7d)
   | summarize avg(RequestCharge) by Resource
   
   // Storage account access patterns
   StorageBlobLogs
   | where TimeGenerated > ago(7d)
   | summarize RequestCount=count() by AccountName, bin(TimeGenerated, 1d)
   ```
3. **计算基准指标**：
-CPU/Memory平均利用率
-数据库吞吐量模式
-存储访问频率
-功能执行速率

4. **验证当前成本**：
—使用步骤2中发现的SKU/tier配置
-在https://azure.microsoft.com/pricing/上查找当前Azure定价或使用`az billing`命令
—文档：资源→当前SKU→每月预估成本
-在提出建议之前，计算当前每月的实际总额步骤4：生成成本优化建议
**行动**：分析资源，识别优化机会
**工具**：使用收集的数据进行本地分析
* * * *过程:
1. **根据找到的资源类型应用优化模式**：

* * * *计算优化:
-应用程序服务计划：根据CPU/memory的使用情况适当大小
-功能应用：高级→低使用率消费计划
—虚拟机：缩小超大的实例

数据库优化* * * *:
- Cosmos DB：     - Provisioned → Serverless for variable workloads
     - Right-size RU/s based on actual usage
—SQL Database：根据DTU的使用情况，选择合适大小的服务层

* * * *存储优化:
-实现生命周期策略（热→冷→存档）
—整合冗余存储帐户
—根据访问模式选择合适的存储层大小

* * * *:基础设施优化
—移除unused/redundant资源
-在有利的地方实现自动缩放
—规划非生产环境

2. **计算基于证据的节省**：
-当前验证成本→目标成本=节约成本
-记录当前和目标配置的定价来源

3. **计算每个推荐的优先级评分**：   ```
   Priority Score = (Value Score × Monthly Savings) / (Risk Score × Implementation Days)
   
   High Priority: Score > 20
   Medium Priority: Score 5-20
   Low Priority: Score < 5
   ```
4. * * * *验证建议:
—确保Azure CLI命令的准确性
-验证估计的节省计算
—评估实施风险和前提条件
-确保所有节约计算都有证据支持

###步骤5：用户确认
**动作**：在创建GitHub问题之前提交总结并获得批准
* * * *过程:
1. **显示优化总结**：   ```
   🎯 Azure Cost Optimization Summary
   
   📊 Analysis Results:
   • Total Resources Analyzed: X
   • Current Monthly Cost: $X 
   • Potential Monthly Savings: $Y 
   • Optimization Opportunities: Z
   • High Priority Items: N
   
   🏆 Recommendations:
   1. [Resource]: [Current SKU] → [Target SKU] = $X/month savings - [Risk Level] | [Implementation Effort]
   2. [Resource]: [Current Config] → [Target Config] = $Y/month savings - [Risk Level] | [Implementation Effort]
   3. [Resource]: [Current Config] → [Target Config] = $Z/month savings - [Risk Level] | [Implementation Effort]
   ... and so on
   
   💡 This will create:
   • Y individual GitHub issues (one per optimization)
   • 1 EPIC issue to coordinate implementation
   
   ❓ Proceed with creating GitHub issues? (y/n)
   ```
2. **等待用户确认**：只有用户确认后才能继续

步骤6：创建单独的优化问题
**行动**：为每个优化机会创建单独的GitHub问题。用“成本优化”（绿色）、“蔚蓝”（蓝色）标记它们。
**MCP工具所需**:`create_issue`为每个推荐
* * * *过程:
1. **使用此模板创建单个问题**：

**标题格式**:`[COST-OPT] [Resource Type] - [Brief Description] - $X/month savings`* * * *的身体模板:   ```markdown
   ## 💰 Cost Optimization: [Brief Title]
   
   **Monthly Savings**: $X | **Risk Level**: [Low/Medium/High] | **Implementation Effort**: X days
   
   ### 📋 Description
   [Clear explanation of the optimization and why it's needed]
   
   ### 🔧 Implementation
   
   **IaC Files Detected**: [Yes/No - based on file_search results]
   
   ```bash
#如果发现IaC文件：显示IaC修改+部署
#文件：infrastructure/bicep/modules/app-service.bicep#更改sku.name: ‘S3’→‘B2’
可用分区部署组create——resource-group [rg]——template-fileinfrastructure/bicep/main.bicep#如果没有IaC文件：直接使用Azure CLI命令+警告
#⚠️没有找到IaC文件。如果它们存在于其他地方，则修改它们。
B2 . az appservice plan update——name [plan]——sku   ```
   
   ### 📊 Evidence
   - Current Configuration: [details]
   - Usage Pattern: [evidence from monitoring data]
   - Cost Impact: $X/month → $Y/month
   - Best Practice Alignment: [reference to Azure best practices if applicable]
   
   ### ✅ Validation Steps
   - [ ] Test in non-production environment
   - [ ] Verify no performance degradation
   - [ ] Confirm cost reduction in Azure Cost Management
   - [ ] Update monitoring and alerts if needed
   
   ### ⚠️ Risks & Considerations
   - [Risk 1 and mitigation]
   - [Risk 2 and mitigation]
   
   **Priority Score**: X | **Value**: X/10 | **Risk**: X/10
   ```
###步骤7：创建EPIC协调问题
**行动**：创建主问题以跟踪所有优化工作。用“成本优化”（绿色）、“蔚蓝”（蓝色）和“史诗”（紫色）来标记它。
**所需MCP工具**:EPIC为`create_issue`**关于美人鱼图的注意事项**：确保您验证美人鱼语法是正确的，并在创建图表时考虑到可访问性准则（样式，颜色等）。
* * * *过程:
1. **创建EPIC问题**：

* *标题* *:`[EPIC] Azure Cost Optimization Initiative - $X/month potential savings`* * * *的身体模板:   ```markdown
   # 🎯 Azure Cost Optimization EPIC
   
   **Total Potential Savings**: $X/month | **Implementation Timeline**: X weeks
   
   ## 📊 Executive Summary
   - **Resources Analyzed**: X
   - **Optimization Opportunities**: Y  
   - **Total Monthly Savings Potential**: $X
   - **High Priority Items**: N
   
   ## 🏗️ Current Architecture Overview
   
   ```mermaid
图结核病       subgraph "Resource Group: [name]"
           [Generated architecture diagram showing current resources and costs]
       end
   ```
   
   ## 📋 Implementation Tracking
   
   ### 🚀 High Priority (Implement First)
   - [ ] #[issue-number]: [Title] - $X/month savings
   - [ ] #[issue-number]: [Title] - $X/month savings
   
   ### ⚡ Medium Priority 
   - [ ] #[issue-number]: [Title] - $X/month savings
   - [ ] #[issue-number]: [Title] - $X/month savings
   
   ### 🔄 Low Priority (Nice to Have)
   - [ ] #[issue-number]: [Title] - $X/month savings
   
   ## 📈 Progress Tracking
   - **Completed**: 0 of Y optimizations
   - **Savings Realized**: $0 of $X/month
   - **Implementation Status**: Not Started
   
   ## 🎯 Success Criteria
   - [ ] All high-priority optimizations implemented
   - [ ] >80% of estimated savings realized
   - [ ] No performance degradation observed
   - [ ] Cost monitoring dashboard updated
   
   ## 📝 Notes
   - Review and update this EPIC as issues are completed
   - Monitor actual vs. estimated savings
   - Consider scheduling regular cost optimization reviews
   ```
##错误处理
- **成本验证**：如果节省估计缺乏支持证据或似乎与Azure定价不一致，请在继续之前重新验证配置和定价来源
- **Azure身份验证失败**：提供手动Azure CLI设置步骤
- **没有找到资源**：创建关于Azure资源部署的信息问题
- **GitHub创建失败**：输出格式化的建议控制台
- **使用数据不足**：仅注意限制并提供基于配置的建议##成功标准
-✅根据实际资源配置和Azure定价验证所有成本估算
-✅为每个优化创建的单独问题（可跟踪和可分配）
-✅EPIC问题提供全面的协调和跟踪
-✅所有建议都包括具体的、可执行的Azure CLI命令
-✅优先级评分使投资回报率为重点的实现
-✅架构图准确反映当前状态
-✅用户确认防止不必要的问题产生