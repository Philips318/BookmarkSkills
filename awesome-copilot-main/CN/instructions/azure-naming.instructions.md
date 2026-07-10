---
applyTo: '**/*.bicep,**/*.tf,**/*.tfvars,**/*.bicepparam,**/infra/**,**/infrastructure/**'
description: 'Azure resource naming conventions based on Microsoft CAF (Cloud Adoption Framework). Use when creating, reviewing, or suggesting names for Azure resources.'
---
Azure资源命名约定（CAF）

来源：[定义您的命名约定](https://learn.microsoft.com/en-us/azure/cloud-adoption-framework/ready/azure-best-practices/resource-naming) |[缩写](https://learn.microsoft.com/en-us/azure/cloud-adoption-framework/ready/azure-best-practices/resource-abbreviations) |[名称规则]（https://learn.microsoft.com/en-us/azure/azure-resource-manager/management/resource-name-rules）

在创建、建议或查看Azure资源名称时，始终遵循这些规则。

---

##通用模式```
<resource-type-abbr>-<workload>-<environment>-<region>-<instance>
```
* *组件规则:* *
- **资源类型** -使用下表中的官方缩写，放在首位
- **工作量/应用/项目** -简短的描述性名称（例如，`navigator`,`payments`）
- **环境** -`prod`、`dev`、`qa`、`stage`、`test`- **区域** -使用Azure区域短名称：`westus`、`eastus2`、`westeurope`、`northeurope`、`uksouth`、`southeastasia`、`australiaeast`等。
- **实例** -补零数字：`001`，`002`>某些资源类型偏离此模式（例如，不允许使用连字符）。参见[官方缩写和命名规则]（# Official -缩写-and- Naming - Rules）了解每个资源的模式和约束。**一般字符规则：**
—小写字母和连字符（`-`）。没有空格，没有下划线，除非资源类型需要它。
-有些资源**不允许使用连字符** -请使用连接的小写字母数字代替（见表）。
—请勿使用：`#`、`<`、`>`、`%`、`&`、`\`、`?`、`/`或控制字符。
—请勿在名称中编码敏感数据（订阅ID、租户ID）。
-大多数名称在Azure中是不区分大小写的-总是比较不区分大小写。
—公网资源不能包含保留字和商标。

---

##命名范围

|作用域|含义||-------|---------|
| **全局** |在所有Azure（具有公共端点的PaaS）中唯一|
| **资源组** |在资源组|中唯一
| **资源** |在父资源|中唯一

---

官方缩写和命名规则

管理和治理

|资源|缩写|范围|长度|有效字符|示例||----------|------|-------|--------|-----------------|---------|
|管理组|`mg`|租户| 1 ~ 90 |字母数字、连字符、下划线、句号、圆括号|`mg-platform-prod`|
|资源组|`rg`|订阅| 1-90 |下划线、连字符、句号、圆括号、字母、数字|`rg-navigator-prod`|
|日志分析工作区|`log`|资源组| 4-63 |字母数字和连字符|`log-navigator-prod-001`|
|应用洞察|`appi`|资源组| 1-260 |无法使用：`%&\?/`|`appi-navigator-prod-001`|
|自动化帐户|`aa`|资源组+地区| 6 ~ 50 |字母数字和连字符，以字母|`aa-navigator-prod-001`|开头

# # #网络

|资源|缩写|范围|长度|有效字符|示例||----------|------|-------|--------|-----------------|---------|
|虚拟网络|`vnet`|资源组| 2-64 |字母数字、下划线、句号、连字符|`vnet-shared-eastus2-001`|
|子网|`snet`|虚拟网络| 1 ~ 80 |字母数字、下划线、句号、连字符|`snet-shared-eastus2-001`|
|网络安全组|`nsg`|资源组| 1 ~ 80 |字母数字、下划线、句号、连字符|`nsg-weballow-001`|
|应用安全组|`asg`|资源组| 1 ~ 80 |字母数字、下划线、句号、连字符|`asg-navigator-prod-001`|
|网络接口|`nic`|资源组| 1 ~ 80 |字母数字、下划线、句号、连字符|`nic-01-vmnavigator-prod-001`|
|公网IP地址|`pip`|资源组| 1 ~ 80 |字母数字、下划线、句号、连字符|`pip-navigator-prod-westus-001`|
|负载均衡器（内部）|`lbi`|资源组| 1 ~ 80 |字母数字、下划线、句号、连字符|`lbi-navigator-prod-001`|
|负载均衡器（外部）|`lbe`|资源组| 1 ~ 80 |字母、下划线、“。Ods，连字符|`lbe-navigator-prod-001`|
|应用网关|`agw`|资源组| 1 ~ 80 |字母数字、下划线、句号、连字符|`agw-navigator-prod-001`|
|防火墙|`afw`|资源组| 1 ~ 80 |字母数字、下划线、句号、连字符|`afw-navigator-prod-001`|
|防火墙策略|`afwp`|资源组| 1 ~ 80 |字母数字、下划线、句号、连字符|`afwp-navigator-prod-001`|
|路由表|`rt`|资源组| 1 ~ 80 |字母数字、下划线、句号、连字符|`rt-navigator-prod-001`|
|虚拟网络网关|`vgw`|资源组| 1 ~ 80 |字母数字、下划线、句号、连字符|`vgw-shared-eastus2-001`|
| VPN网关|`vpng`|资源组| 1 ~ 80 |字母数字、下划线、句号、连字符|`vpng-navigator-prod-001`|
| Azure Bastion |`bas`|资源组| 1-80 |字母数字、下划线、句号、连字符|`bas-navigator-prod-001`|
|私有端点|`pep`|资源组| 2-64 |字母数字、下划线、周期Ods，连字符|`pep-navigator-prod-001`|
|流量管理器配置文件|`traf`| global | 1-63 |字母数字和连字符（不含句号）|`traf-navigator-prod`|
| ExpressRoute电路|`erc`|资源组| 1 ~ 80 |字母数字、下划线、句号、连字符|`erc-navigator-prod-001`|
| CDN配置文件|`cdnp`|资源组| 1 ~ 260 |字母数字和连字符|`cdnp-navigator-prod-001`|
|前门配置文件|`afd`|资源组| 5-64 |字母数字和连字符|`afd-navigator-prod`|###计算和Web

|资源|缩写|范围|长度|有效字符|示例||----------|------|-------|--------|-----------------|---------|
|虚拟机|`vm`|资源组| 1 ~ 15 (Windows) / 1 ~ 64 (Linux) |无空格或：`~ ! @ # $ % ^ & * ( ) = + _ [ ] { } \| ; : . ' " , < > / ?`|`vm-sql-test-001`|
|虚拟机规模集|`vmss`|资源组| 1 ~ 15 (Windows) / 1 ~ 64 (Linux) |同虚拟机|`vmss-navigator-prod-001`|
|可用性集|`avail`|资源组| 1 ~ 80 |字母数字、下划线、句号、连字符|`avail-navigator-prod-001`|
|应用服务规划|`asp`|资源组| 1-60 |字母数字、连字符、Unicode |`asp-navigator-prod-001`|
| Web应用|`app`| global | 2-60 |字母数字，连字符，Unicode。不能用连字符start/end。|`app-navigator-prod-001`|
|功能应用|`func`| global | 2-60 |字母数字，连字符，Unicode。不能用连字符start/end。|`func-navigator-prod-001`|
|静态web应用|`stapp`|资源组| - | - |`stapp-navigator-prod-001`|
|应用服务环境|`ase`|资源组| - | - |`ase-navigator-prod-001`|

# # #容器

|资源|缩写|范围|长度|有效字符|示例||----------|------|-------|--------|-----------------|---------|
| AKS集群|`aks`|资源组| 1 ~ 63 |字母数字、下划线、连字符|`aks-navigator-prod-001`|
| AKS系统节点池|`npsystem`|托管集群| 1-12 (Linux) / 1-6 (Windows) |小写字母和数字，不能以数字|`npsystem`|开头
| AKS用户节点池|`np`|托管集群| 1-12 (Linux) / 1-6 (Windows) |小写字母和数字，不能以数字|`npusers`|开头
|容器应用|`ca`|资源组| 2-32 |小写字母、数字、连字符。以字母开头，以字母数字结尾。|`ca-navigator-prod-001`|
|容器应用环境|`cae`|资源组| - | - |`cae-navigator-prod-001`|
|容器实例|`ci`|资源组| 1 ~ 63 |小写字母、数字、连字符。不能用连字符start/end。|`ci-navigator-prod-001`|
|容器注册表|`cr`| global | 5-50 | **仅含字母数字-无连字符** |`crnavigatorprod001`|

# # #数据库|资源|缩写|范围|长度|有效字符|示例||----------|------|-------|--------|-----------------|---------|
| Azure SQL server |`sql`| global | 1-63 |小写字母、数字、连字符。不能用连字符start/end。|`sql-navigator-prod-001`|
| Azure SQL数据库|`sqldb`| SQL服务器| 1-128 |不能使用：`<>*%&:\/?`|`sqldb-navigator-prod`|
| SQL管理实例|`sqlmi`| global | 1-63 |小写字母、数字、连字符。不能用连字符start/end。|`sqlmi-navigator-prod-001`|
| Azure Cosmos DB |`cosmos`| global | 3-44 |小写字母、数字、连字符。以小写字母或数字开头。|`cosmos-navigator-prod`|
| Azure Managed Redis |`amr`| global | 1-63 |字母数字和连字符。Start/end带字母数字。|`amr-navigator-prod-001`|
| MySQL服务器|`mysql`| global | 3-63 |小写字母、连字符、数字。不能用连字符start/end。|`mysql-navigator-prod-001`|
| PostgreSQL server |`psql`| global | 3-63 |小写字母、连字符、数字。不能用连字符start/end。|`psql-navigator-prod-001`|

# # #存储|资源|缩写|范围|长度|有效字符|示例||----------|------|-------|--------|-----------------|---------|
|存储帐户|`st`| global | 3-24 | **只能使用小写字母和数字-不能使用连字符** |`stnavigatorprod001`|
|备份vault |`bvault`|资源组| 2 ~ 50 |字母数字和连字符。从一封信开始。|`bvault-navigator-prod-001`|

# # #安全

|资源|缩写|范围|长度|有效字符|示例||----------|------|-------|--------|-----------------|---------|
|密钥库|`kv`| global | 3-24 |字母数字和连字符。以字母开头，以字母或数字结尾。没有连续的连字符。|`kv-navigator-prod-001`|
|被管理身份|`id`|资源组| 3-128 |字母、数字、“-”、“_”。从字母或数字开始。|`id-navigator-prod-001`|

# # #集成

|资源|缩写|范围|长度|有效字符|示例||----------|------|-------|--------|-----------------|---------|
| API Management |`apim`| global | 1-50 |字母数字和连字符。以字母开头，以字母数字结尾。|`apim-navigator-prod`|
|服务总线命名空间|`sbns`| global | 6-50 |字母数字和连字符。以字母开头，以字母或数字结尾。|`sbns-navigator-prod`|
|业务总线队列|`sbq`|业务总线| 1-260 |字母数字、点、连字符、下划线、斜线|`sbq-navigator`|
|服务总线主题|`sbt`|服务总线| 1-260 |字母数字、点、连字符、下划线、斜线|`sbt-navigator`|
|事件中心命名空间|`evhns`| global | 6-50 |字母数字和连字符。以字母开头，以字母或数字结尾。|`evhns-navigator-prod`|
|事件集线器|`evh`|事件集线器命名空间| 1-256 |字母数字、句号、连字符、下划线|`evh-navigator`|
|逻辑应用|`logic`|资源组| 1-43 |字母数字、连字符、下划线、句号|`logic-navigator-prod-001`|

人工智能和机器学习|资源|缩写|范围|长度|有效字符|示例||----------|------|-------|--------|-----------------|---------|
| Azure OpenAI服务|`oai`|资源组| 2-64 |字母数字和连字符|`oai-navigator-prod`|
| AI搜索|`srch`|全球| - | - |`srch-navigator-prod`|
| Azure ML工作空间|`mlw`|资源组| 3-33 |字母数字、连字符、下划线|`mlw-navigator-prod`|
|铸造中心|`hub`|资源组| 3-33 |字母数字、连字符、下划线|`hub-navigator-prod`|
|铸造中心项目|`proj`|铸造中心| 3-33 |字母数字，连字符，下划线|`proj-navigator-prod`|
|铸造厂帐号|`aif`|资源组| 2-64 |字母数字和连字符|`aif-navigator-prod`|
|铸造账户项目|`proj`|铸造账户| - | - |`proj-navigator-prod`|
| Foundry Tools（多业务）|`ais`|资源组| 2-64 |字母数字和连字符|`ais-navigator-prod`|

分析和物联网

|资源|缩写|范围|长度|有效字符|示例||----------|------|-------|--------|-----------------|---------|
| Azure Data Factory |`adf`| global | 3-63 |字母数字和连字符。Start/end带字母数字。|`adf-navigator-prod`|
| Azure Databricks工作区|`dbw`|资源组| 3-64 |字母数字、下划线、连字符|`dbw-navigator-prod-001`|
| Azure Data Explorer集群|`dec`| global | 4-22 |小写字母和数字。从一封信开始。|`decnavigatorprod`|
| Azure Synapse工作区|`synw`| global | 1-50 |小写字母、连字符、数字。Start/end用字母或数字。|`synw-navigator-prod`|
| IoT hub |`iot`| global | 3-50 |字母数字和连字符。不能以连字符结尾。|`iot-navigator-prod`|
|事件网格主题|`evgt`|区域| 3-50 |字母数字和连字符|`evgt-navigator-prod`|

开发者工具

|资源|缩写|范围|长度|有效字符|示例||----------|------|-------|--------|-----------------|---------|
| App Configuration store |`appcs`| global | 5-50 |字母数字和连字符。不超过两个连续的连字符。|`appcs-navigator-prod`|
| SignalR |`sigr`| global | 3-63 |字母数字和连字符。以字母开头，以字母或数字结尾。|`sigr-navigator-prod`|

---

不允许连字符的资源

这些资源需要小写字母数字连接（没有分隔符）：

|资源|缩写|模式||----------|------|---------|
|存储帐户|`st`|`st{workload}{env}{instance}`→`stnavigatorprod001`|
|容器注册表|`cr`|`cr{workload}{env}{instance}`→`crnavigatorprod001`|
| Azure Data Explorer集群|`dec`|`dec{workload}{env}`→`decnavigatorprod`|

---

##示例（CAF）```
# Management
rg-navigator-prod
rg-webapp-database-dev

# Networking
vnet-shared-eastus2-001
snet-shared-eastus2-001
nsg-weballow-001
pip-dc1-shared-eastus2-001
lbe-navigator-prod-001

# Compute
vm-sql-test-001
vm-sharepoint-dev-001
vmss-navigator-prod-001
asp-navigator-prod-001
app-navigator-prod-001
func-navigator-prod-001

# Containers
aks-navigator-prod-001
ca-navigator-prod-001
cae-navigator-prod-001
crnavigatorprod001        # no hyphens!

# Databases
sql-navigator-prod-001
sqldb-navigator-prod
cosmos-navigator-prod
psql-navigator-prod-001

# Storage / Security
stnavigatorprod001        # no hyphens!
kv-navigator-prod-001
id-navigator-prod-001

# Integration
apim-navigator-prod
sbns-navigator-prod
evhns-navigator-prod

# Monitoring
log-navigator-prod-001
appi-navigator-prod-001

# AI
oai-navigator-prod
srch-navigator-prod
```
---

##不做

—除非资源类型要求，否则不要使用下划线。—使用连字符。
-不要拼写完整的资源类型单词（例如，`storageaccount-myapp`→使用`stmyapp001`）。
—不使用大写字母（资源不区分大小写，小写是惯例）。
—名称中不能包含敏感数据（订阅ID、租户ID、密码）。
—即使是生产环境，也不要跳过环境部分。
-不要使用`#`-它会破坏Azure资源管理器中的URL解析。
—对于带有公共端点的资源，不允许在名称中使用保留字或商标。
—连字符不能超过两个（例如：`app--prod`无效）。