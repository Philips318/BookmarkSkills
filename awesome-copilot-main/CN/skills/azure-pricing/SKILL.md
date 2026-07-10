---
name: azure-pricing
description: 'Fetches real-time Azure retail pricing using the Azure Retail Prices API (prices.azure.com) and estimates Copilot Studio agent credit consumption. Use when the user asks about the cost of any Azure service, wants to compare SKU prices, needs pricing data for a cost estimate, mentions Azure pricing, Azure costs, Azure billing, or asks about Copilot Studio pricing, Copilot Credits, or agent usage estimation. Covers compute, storage, networking, databases, AI, Copilot Studio, and all other Azure service families.'
compatibility: Requires internet access to prices.azure.com and learn.microsoft.com. No authentication needed.
metadata:
  author: anthonychu
  version: "1.2"
---
Azure定价技能

使用此技能可以从公共Azure零售价格API检索实时Azure零售价格数据。不需要身份验证。

何时使用此技能

-用户询问Azure服务的成本（例如，“D4s v5虚拟机的成本是多少？”）
-用户希望比较不同地区或sku之间的价格
-用户需要对工作负载或架构进行成本估算
-用户提到Azure定价、Azure成本或Azure账单
-用户询问保留实例与随用随付的定价
-用户希望了解储蓄计划或现货定价

## API端点```
GET https://prices.azure.com/api/retail/prices?api-version=2023-01-01-preview
```
使用OData过滤器语法追加`$filter`作为查询参数。始终使用`api-version=2023-01-01-preview`来确保包含储蓄计划数据。

##分步说明

如果用户的请求有任何不清楚的地方，请在调用API之前询问澄清问题，以确定正确的过滤器字段和值。1. **从用户请求中识别过滤字段**（服务名称、地区、SKU、价格类型）。
2. **解析区域**:API要求`armRegionName`值是小写的，没有空格(例如：“美国东部”→`eastus`，“西欧”→`westeurope`，“东南亚”→`southeastasia`)。参见[references/REGIONS.md]（references/REGIONS.md）获得完整的列表。
3. **构建过滤器字符串**使用下面的字段和获取URL。
4. **解析JSON响应中的`Items`数组**。每个项目包含价格和元数据。
5. **如果您需要超过前1000个结果（很少需要），请通过`NextPageLink`跟踪分页**。
6. **计算成本估算**使用[references/COST-ESTIMATOR.md]（references/COST-ESTIMATOR.md）中的公式生成monthly/annual估算。
7. **以清晰的汇总表形式显示结果**，包括服务、SKU、地区、单价和monthly/annual估算。

##可过滤字段

|字段|类型|示例||---|---|---|
|`serviceName`| string（精确，区分大小写）|`'Functions'`,`'Virtual Machines'`,`'Storage'`|
|`serviceFamily`| string（精确，区分大小写）|`'Compute'`,`'Storage'`,`'Databases'`,`'AI + Machine Learning'`|
|`armRegionName`|字符串（精确，小写）|`'eastus'`,`'westeurope'`,`'southeastasia'`|
|`armSkuName`|字符串（精确）|`'Standard_D4s_v5'`,`'Standard_LRS'`|
|`skuName`| string（包含支持的）|`'D4s v5'`|
|`priceType`| string |`'Consumption'`,`'Reservation'`,`'DevTestConsumption'`|
|`meterName`| string（包含支持的）|`'Spot'`|

使用`eq`表示相等，使用`and`表示组合，使用`contains(field, 'value')`表示部分匹配。

##示例过滤字符串```
# All consumption prices for Functions in East US
serviceName eq 'Functions' and armRegionName eq 'eastus' and priceType eq 'Consumption'

# D4s v5 VMs in West Europe (consumption only)
armSkuName eq 'Standard_D4s_v5' and armRegionName eq 'westeurope' and priceType eq 'Consumption'

# All storage prices in a region
serviceName eq 'Storage' and armRegionName eq 'eastus'

# Spot pricing for a specific SKU
armSkuName eq 'Standard_D4s_v5' and contains(meterName, 'Spot') and armRegionName eq 'eastus'

# 1-year reservation pricing
serviceName eq 'Virtual Machines' and priceType eq 'Reservation' and armRegionName eq 'eastus'

# Azure AI / OpenAI pricing (now under Foundry Models)
serviceName eq 'Foundry Models' and armRegionName eq 'eastus' and priceType eq 'Consumption'

# Azure Cosmos DB pricing
serviceName eq 'Azure Cosmos DB' and armRegionName eq 'eastus' and priceType eq 'Consumption'
```
获取URL```
https://prices.azure.com/api/retail/prices?api-version=2023-01-01-preview&$filter=serviceName eq 'Functions' and armRegionName eq 'eastus' and priceType eq 'Consumption'
```
在构造URL时，将URL编码为`%20`的空格和`%27`的引号。

关键响应字段```json
{
  "Items": [
    {
      "retailPrice": 0.000016,
      "unitPrice": 0.000016,
      "currencyCode": "USD",
      "unitOfMeasure": "1 Execution",
      "serviceName": "Functions",
      "skuName": "Premium",
      "armRegionName": "eastus",
      "meterName": "vCPU Duration",
      "productName": "Functions",
      "priceType": "Consumption",
      "isPrimaryMeterRegion": true,
      "savingsPlan": [
        { "unitPrice": 0.000012, "term": "1 Year" },
        { "unitPrice": 0.000010, "term": "3 Years" }
      ]
    }
  ],
  "NextPageLink": null,
  "Count": 1
}
```
只使用`isPrimaryMeterRegion`为`true`的项，除非用户特别要求使用非主计量表。

支持的serviceFamily值`Analytics`,`Compute`,`Containers`,`Data`,`Databases`,`Developer Tools`,`Integration`,`Management and Governance`,`Networking`,`Security`,`Storage`,`Web`,`AI + Machine Learning`# #提示—`serviceName`值区分大小写。如果不确定，首先按`serviceFamily`进行筛选，以在结果中发现有效的`serviceName`值。
-如果结果为空，尝试扩大过滤器（例如，首先删除`priceType`或区域约束）。
-价格总是以美元为单位，除非在请求中指定了`currencyCode`。
-对于储蓄计划价格，请查找每个项目上的`savingsPlan`数组（仅在`2023-01-01-preview`中）。
—请参见[references/SERVICE-NAMES.md]（references/SERVICE-NAMES.md）获取常用服务名称的目录及其正确的大小写。
-成本估算公式和模式见[references/COST-ESTIMATOR.md]（references/COST-ESTIMATOR.md）。
-参见[references/COPILOT-STUDIO-RATES.md]（references/COPILOT-STUDIO-RATES.md）的副驾驶工作室计费费率和估计公式。

# #故障排除

|问题|解决方案||-------|----------|
|拓宽过滤器-先删除`priceType`或`armRegionName`|
|服务名称错误|使用`serviceFamily`过滤器发现有效的`serviceName`值|
|储蓄计划数据丢失|确保`api-version=2023-01-01-preview`在URL |中
|检查URL编码-空格为`%20`，引号为`%27`|
|添加更多筛选字段（region， SKU, priceType）来缩小|的范围

---

# Copilot Studio代理使用估计

当用户询问Copilot Studio定价、Copilot Credits或代理使用成本时，请使用此部分。

##何时使用本节

-用户询问Copilot Studio的定价或成本
-用户询问副驾驶积分或代理积分消耗情况
-用户要估计每月的费用为一个副驾驶工作室代理
-用户提到代理使用估计或Copilot Studio估计器
-用户询问代理运行的成本

##关键事实- **1副驾驶积分= $0.01 USD**
-积分汇集在整个租户中
-拥有M365 Copilot许可用户的面向员工的代理以零成本获得经典答案，生成答案和租户图接地
-超额执行触发为预付容量的125%

##逐步估算1. **收集用户输入**：代理类型（employee/customer）、用户数、interactions/month、知识%、租户图%、每次会话使用工具。
2. **获取实时计费费率** -使用内置的web获取工具从下面列出的源url下载最新费率。这确保估算总是使用最新的微软定价。
3. **解析获取的内容**以提取当前计费费率表（每个功能类型的积分）。
4. **使用获取的内容中的比率和公式计算估算值**：
——`total_sessions = users × interactions_per_month`—知识积分：应用租户图接地率、生成答案率和经典答案率
—座席工具积分：每个工具调用应用座席动作率
-代理流量积分：每100个动作应用流量率
-提示修改积分：每10个回复应用basic/standard/premium费率
5. **以清晰的表格显示结果**，并按cat分类学分，总学分和估计的美元成本。##获取源url

在回答Copilot Studio定价问题时，从这些url获取最新内容以用作上下文：

|网址|内容||---|---|
|https://learn.microsoft.com/en-us/microsoft-copilot-studio/requirements-messages-management|计费费率表、计费示例、超限强制规则|
|https://learn.microsoft.com/en-us/microsoft-copilot-studio/billing-licensing|许可选项，M365副驾驶包含，预付费与即用即付|

在计算之前至少获取第一个URL（计费费率）。第二个URL为许可问题提供了补充上下文。

请参阅[references/COPILOT-STUDIO-RATES.md]（references/COPILOT-STUDIO-RATES.md）获取费率、公式和计费示例的缓存快照（如果web获取不可用，则用作回退）。