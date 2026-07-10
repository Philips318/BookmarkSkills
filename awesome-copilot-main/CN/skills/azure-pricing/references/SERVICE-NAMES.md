# Azure服务名称引用

Azure零售价格API中的`serviceName`字段是区分大小写的。使用此引用查找要在过滤器中使用的确切服务名称。

# #计算

|服务|`serviceName`值||---------|-------------------|
|虚拟机|`Virtual Machines`|
| Azure函数|`Functions`|
| Azure应用服务|`Azure App Service`|
| Azure容器应用|`Azure Container Apps`|
| Azure容器实例|`Container Instances`|
| Azure Kubernetes服务|`Azure Kubernetes Service`|
| Azure Batch |`Azure Batch`|
| Azure Spring Apps |`Azure Spring Apps`|
| Azure VMware Solution |`Azure VMware Solution`|

# #存储

|服务|`serviceName`值||---------|-------------------|
| Azure存储（Blob，文件，队列，表）|`Storage`|
| Azure NetApp文件|`Azure NetApp Files`|
| Azure备份|`Backup`|
| Azure数据盒|`Data Box`|

> **备注**:Blob Storage、Files、Disk Storage、Data Lake Storage都在单个服务名称`Storage`下。使用`meterName`或`productName`来区分它们（例如，`contains(meterName, 'Blob')`）。

# #数据库

|服务|`serviceName`值||---------|-------------------|
Azure Cosmos DB |`Azure Cosmos DB`|
Azure SQL数据库|`SQL Database`|
| Azure SQL管理实例|`SQL Managed Instance`|
| Azure Database for PostgreSQL |`Azure Database for PostgreSQL`|
| Azure MySQL数据库|`Azure Database for MySQL`|
| Azure Redis缓存|`Redis Cache`|

人工智能+机器学习

|服务|`serviceName`值||---------|-------------------|
| Azure AI铸造厂模型（包括OpenAI） |`Foundry Models`|
| Azure AI铸造工具|`Foundry Tools`|
Azure机器学习|`Azure Machine Learning`|
| Azure认知搜索（AI搜索）|`Azure Cognitive Search`|
| Azure Bot服务|`Azure Bot Service`|

**注**:Azure OpenAI定价现在低于`Foundry Models`。使用`contains(productName, 'OpenAI')`或`contains(meterName, 'GPT')`来过滤openai特定的模型。

# #网络

|服务|`serviceName`值||---------|-------------------|
| Azure负载均衡器|`Load Balancer`|
| Azure应用网关|`Application Gateway`|
| Azure前门|`Azure Front Door Service`|
| Azure CDN |`Azure CDN`|
| Azure DNS |`Azure DNS`|
| Azure虚拟网络|`Virtual Network`|
| Azure VPN网关|`VPN Gateway`|
| Azure ExpressRoute |`ExpressRoute`|
| Azure防火墙|`Azure Firewall`|

# #分析

|服务|`serviceName`值||---------|-------------------|
| Azure Synapse Analytics |`Azure Synapse Analytics`|
| Azure数据工厂|`Azure Data Factory v2`|
| Azure流分析|`Azure Stream Analytics`|
| Azure数据库|`Azure Databricks`|
| Azure事件中心|`Event Hubs`|

# #集成

|服务|`serviceName`值||---------|-------------------|
| Azure服务总线|`Service Bus`|
| Azure逻辑应用|`Logic Apps`|
| Azure API管理|`API Management`|
| Azure事件网格|`Event Grid`|

##管理和监控

|业务|`serviceName`值||---------|-------------------|
| Azure Monitor |`Azure Monitor`|
| Azure日志分析|`Log Analytics`|
| Azure密钥库|`Key Vault`|
| Azure备份|`Backup`|

# #网络

|业务|`serviceName`值||---------|-------------------|
| Azure静态Web应用|`Azure Static Web Apps`|
| Azure SignalR |`Azure SignalR Service`|

# #提示

-如果您不确定服务名称，**首先通过`serviceFamily`过滤**以在响应中发现有效的`serviceName`值。
—示例：`serviceFamily eq 'Databases' and armRegionName eq 'eastus'`返回所有数据库服务名称。
-一些服务有多个`serviceName`条目，用于不同的层或代。