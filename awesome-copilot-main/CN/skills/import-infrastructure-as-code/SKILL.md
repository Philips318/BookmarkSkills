---
name: import-infrastructure-as-code
description: 'Import existing Azure resources into Terraform using Azure CLI discovery and Azure Verified Modules (AVM). Use when asked to reverse-engineer live Azure infrastructure, generate Infrastructure as Code from existing subscriptions/resource groups/resource IDs, map dependencies, derive exact import addresses from downloaded module source, prevent configuration drift, and produce AVM-based Terraform files ready for validation and planning across any Azure resource type.'
---
#将基础设施导入为代码（Azure -> Terraform with AVM）

使用发现数据和Azure验证模块将现有Azure基础架构转换为可维护的Terraform代码。

何时使用此技能

当用户请求时使用此技能：

-导入现有的Azure资源到Terraform
-从Azure环境中生成IaC
-处理AVM支持的任何Azure资源类型（以及文档证明的非AVM回退）
—从订阅或资源组中重新创建基础架构
-映射发现的Azure资源之间的依赖关系
-使用AVM模块代替手写的`azurerm_*`资源

# #先决条件

安装并验证Azure CLI （`az login`）
—访问目标订阅或资源组
—已安装Terraform CLI
-网络访问Terraform注册表和AVM索引源

# #输入

| |必选参数|默认值|描述||---|---|---|---|
|`subscription-id`|否|活动CLI上下文|用于订阅范围发现和上下文设置的Azure订阅|
|`resource-group-name`|否|无|用于发现资源组范围的Azure资源组|
|`resource-id`|否|无|用于特定资源范围发现的一个或多个Azure ARM资源id |

需要`subscription-id`、`resource-group-name`或`resource-id`中的至少一个。

##分步工作流程

### 1)收集所需范围（强制性）

在运行发现命令之前请求这些作用域之一：

—订阅范围：`<subscription-id>`—资源组范围：`<resource-group-name>`—特定资源范围：一个或多个`<resource-id>`值

作用域处理规则：-将Azure ARM资源id（例如`/subscriptions/.../providers/...`）视为云资源标识符，而不是本地文件系统路径。
—资源id只能与Azure CLI的`--ids`参数一起使用（例如`az resource show --ids <resource-id>`）。
-永远不要将资源id传递给文件读取命令（`cat`,`ls`,`read_file`， glob搜索），除非用户明确表示它们是本地文件路径。
如果用户已经提供了一个有效的作用域，不要要求额外的作用域输入，除非一个失败的命令需要。
-不要问可以从已经提供的范围值回答的后续问题。

如果缺少作用域，明确地请求它并停止。

### 2)认证和设置上下文

只运行所选范围所需的命令。

订阅范围：```bash
az login
az account set --subscription <subscription-id>
az account show --query "{subscriptionId:id, name:name, tenantId:tenantId}" -o json
```
期望输出：具有`subscriptionId`、`name`和`tenantId`的JSON对象。

对于资源组或特定的资源范围，`az login`仍然是必需的，但是如果活动上下文已经正确，`az account set`是可选的。

当使用特定的资源作用域时，首先选择直接基于`--ids`的命令，并避免对订阅或资源组进行额外的发现提示，除非具体命令需要。

### 3)运行发现命令

使用所选范围发现资源。确保获取所有必要的信息，以准确地生成地形。```bash
# Subscription scope
az resource list --subscription <subscription-id> -o json

# Resource group scope
az resource list --resource-group <resource-group-name> -o json

# Specific resource scope
az resource show --ids <resource-id-1> <resource-id-2> ... -o json
```
期望输出：包含Azure资源元数据的JSON对象或数组（`id`,`type`,`name`,`location`,`tags`,`properties`）。

### 4)在代码生成之前解决依赖关系

解析导出的JSON和map：

—亲子关系（例如：网卡—>子网—> VNet）
—`properties`中的跨资源引用
-订购地形创建

重要：生成以下文档并将其保存到项目根目录下的docs文件夹中。
-`exported-resources.json`，包含所有发现的资源及其元数据，包括依赖项和引用。
-`EXPORTED-ARCHITECTURE.MD`文件，其中包含基于发现的资源及其关系的人类可读架构概述。

### 5)选择Azure验证模块（必选）

对每种资源类型使用最新的AVM版本。

Terraform注册表—搜索“avm +资源名”
-通过“合作伙伴”标签筛选以查找官方AVM模块
—示例：搜索“avm存储帐户”→按合作伙伴过滤

官方AVM指数

> **注意：**以下链接总是指向主分支上最新版本的CSV文件。正如预期的那样，这意味着文件可能会随着时间的推移而更改。如果您需要某个时间点的版本，请考虑在URL中使用特定的发布标签。

- **地形资源模块**:`https://raw.githubusercontent.com/Azure/Azure-Verified-Modules/refs/heads/main/docs/static/module-indexes/TerraformResourceModules.csv`- **地形模式模块**:`https://raw.githubusercontent.com/Azure/Azure-Verified-Modules/refs/heads/main/docs/static/module-indexes/TerraformPatternModules.csv`- **Terraform实用模块**:`https://raw.githubusercontent.com/Azure/Azure-Verified-Modules/refs/heads/main/docs/static/module-indexes/TerraformUtilityModules.csv`单个模块信息

如果`.terraform`文件夹中没有本地可用的模块信息，请使用`web`工具或其他合适的MCP方法获取模块信息。

使用AVM来源：

—注册表：`https://registry.terraform.io/modules/Azure/<module>/azurerm/latest`- GitHub:`https://github.com/Azure/terraform-azurerm-avm-res-<service>-<resource>`当存在AVM模块时，首选AVM模块而不是手写的`azurerm_*`资源。当从GitHub存储库中获取模块信息时，存储库根目录中的README.md文件通常包含有关模块的所有详细信息，例如：https://raw.githubusercontent.com/Azure/terraform-azurerm-avm-res-<service>-<resource>/refs/heads/main/README.md### 5a)在编写任何代码之前阅读模块README（强制）

**该步骤不可选。**在为模块编写一行HCL之前，获取和
请阅读该模块的完整README。不依赖原始`azurerm`提供程序的知识
或有其他AVM模块的经验。

对于每个选定的AVM模块，获取其README：```text
https://raw.githubusercontent.com/Azure/terraform-azurerm-avm-res-<service>-<resource>/refs/heads/main/README.md
```
或者如果模块在`terraform init`之后已经下载：```bash
cat .terraform/modules/<module_key>/README.md
```
在编写代码之前，从README中提取并记录**：

1. **所需输入** -模块所需的每个输入。这里列出的任何子资源	 (NICs, extensions, subnets, public IPs) is managed **inside** the module. Do **not**
	 create standalone module blocks for those resources.
2. **可选输入** -确切的Terraform变量名称及其声明的`type`。	 Do not assume they match the raw `azurerm` provider argument names or block shapes.
3. **使用示例** -检查使用了什么资源组标识符（`parent_id`vs . xqz）	 `resource_group_name`), how child resources are expressed (inline map vs separate module),
	 and what syntax each input expects.
####将模块规则应用为模式，而不是假设

使用下面的教训作为经常导致导入失败的不匹配的“类型”的示例。
不要假设这些确切的名称适用于每个AVM模块。始终验证每个选择的模块
README和`variables.tf`。

**`avm-res-compute-virtualmachine`（任意版本）**

—`network_interfaces`为**必选输入。网卡属于虚拟机模块。从来没有	create standalone `avm-res-network-networkinterface` modules alongside a VM module —
	define every NIC inline under `network_interfaces`.
—TrustedLaunch通过顶层布尔值`secure_boot_enabled = true`表示	and `vtpm_enabled = true`. The `security_type` argument exists only under `os_disk` for
	Confidential VM disk encryption and must not be used for TrustedLaunch.
—`boot_diagnostics`是`bool`，而不是对象。使用`boot_diagnostics = true`;使用	separate `boot_diagnostics_storage_account_uri` variable if a storage URI is needed.
-扩展通过`extensions`映射在模块内进行管理。不要创造	standalone extension resources.
**`avm-res-network-virtualnetwork`（任何版本）**

—该模块由AzAPI提供程序支持，而不是`azurerm`。使用`parent_id`(全	resource group resource ID string) to specify the resource group, not `resource_group_name`.
- README中的每个示例都显示`parent_id`；无show`resource_group_name`。

所有AVM模块的一般化结论：

-在创建兄弟模块之前，从**Required Inputs**确定子资源的所有权。
-从**可选输入**和`variables.tf`中确定可接受的变量名称和类型。
-从README使用示例中确定标识符样式和输入形状。
-不要从原始的`azurerm_*`资源中推断参数名称。

### 6)生成地形文件

###在写导入块之前-检查模块源（必选）

在`terraform init`下载模块之后，检查每个模块的源文件来确定
在写入任何`import {}`块之前，获取确切的Terraform资源地址。从来没有写
从内存中导入地址。

####步骤A—识别提供者和资源标签```bash
grep "^resource" .terraform/modules/<module_key>/main*.tf
```
这将显示模块是使用`azurerm_*`还是`azapi_resource`标签。例如,`avm-res-network-virtualnetwork`暴露了`azapi_resource "vnet"`，而不是`azurerm_virtual_network "this"`。

####步骤B -确定子模块和嵌套路径```bash
grep "^module" .terraform/modules/<module_key>/main*.tf
```
如果子资源是在子模块（子网、扩展等）中管理的，则导入
Address必须包含每个中间模块的标签：```text
module.<root_module_key>.module.<child_module_key>["<map_key>"].<resource_type>.<label>[<index>]
```
####步骤C -检查`count`vs`for_each````bash
grep -n "count\|for_each" .terraform/modules/<module_key>/main*.tf
```
任何使用`count`的资源都需要在导入地址中添加索引。当`count = 1`(例如，
条件Linux vs Windows选择)，地址必须以`[0]`结尾。资源的使用`for_each`使用字符串键，而不是数字索引。

####已知的导入地址模式（来自经验教训的示例）

这些只是例子。使用它们作为推理的模板，然后推导出确切的地址
从当前导入中模块的下载源代码中获取。

|资源|正确导入`to`地址模式||---|---|
| azapi支持的VNet |`module.<vnet_key>.azapi_resource.vnet`|
|子网（嵌套，基于计数）|`module.<vnet_key>.module.subnet["<subnet_name>"].azapi_resource.subnet[0]`|
| Linux虚拟机（计数）|`module.<vm_key>.azurerm_linux_virtual_machine.this[0]`|
|虚拟机网卡|`module.<vm_key>.azurerm_network_interface.virtualmachine_network_interfaces["<nic_key>"]`|
|虚拟机扩展（默认部署序列=5）|`module.<vm_key>.module.extension["<ext_name>"].azurerm_virtual_machine_extension.this`|
|虚拟机扩展（deploy_sequence= 1-4） |`module.<vm_key>.module.extension_<n>["<ext_name>"].azurerm_virtual_machine_extension.this`|
| nsg -网卡关联|`module.<vm_key>.azurerm_network_interface_security_group_association.this["<nic_key>-<nsg_key>"]`|

生产:

-`providers.tf`与`azurerm`提供程序和所需的版本约束
-`main.tf`与AVM模块块和显式依赖
-`variables.tf`为特定于环境的值
—`outputs.tf`表示密钥id和端点
-`terraform.tfvars.example`与占位符值

###根据模块默认值设置Live属性（必选）

在编写初始配置之后，比较每个发现的非零属性
与相应AVM模块中声明的默认值进行比较`variables.tf`。任何活动值与模块默认值不同的属性都必须是
在Terraform配置中显式设置。请特别注意以下属性类别，它们是常见的来源
无声配置漂移：

- **超时值**（例如，公网IP`idle_timeout_in_minutes`默认为`4`； live）	deployments often use `30`)
- **网络策略标志**(例如，子网`private_endpoint_network_policies`默认为	`"Enabled"`; existing subnets often have `"Disabled"`)
- **SKU和分配**（例如，公共IP`sku`，`allocation_method`）
—**可用分区**（如虚拟机分区、公网IP分区）
—**存储和数据库资源的冗余和复制**设置

使用显式的`az`命令检索完整的活动属性，例如：```bash
az network public-ip show --ids <resource_id> --query "{idleTimeout:idleTimeoutInMinutes, sku:sku.name, zones:zones}" -o json
az network vnet subnet show --ids <resource_id> --query "{privateEndpointPolicies:privateEndpointNetworkPolicies, delegation:delegations}" -o json
```
不要仅仅依赖`az resource list`输出，这可能会忽略嵌套属性或计算属性。

显式引脚模块版本：```hcl
module "example" {
	source  = "Azure/<module>/azurerm"
	version = "<latest-compatible-version>"
}
```
验证生成的代码

运行:```bash
terraform init
terraform fmt -recursive
terraform validate
terraform plan
```
预期输出：没有语法错误，没有验证错误，并且计划与发现的基础结构意图相匹配。

# #故障排除

|问题|可能原因|行动||---|---|---|
|`az`命令失败，授权错误|tenant/subscription错误或缺少RBAC角色|重新运行`az login`，验证订阅上下文，确认所需权限|
|发现输出为空|作用域不正确或没有资源|重新检查作用域输入，再次执行scopedlist/show命令|
|找不到资源类型|资源类型尚未被AVM覆盖|使用该类型的本机`azurerm_*`资源并记录差距|
|`terraform validate`失败|缺少变量或未解析的依赖|添加所需变量和显式依赖，然后重新运行验证|
|模块|中未找到未知参数或变量AVM变量名称与`azurerm`提供程序参数名称不同|阅读模块README`variables.tf`或可选输入部分以获得正确的名称|
|提供程序标签错误（`azurerm_`vs`azapi_`），缺少子模块运行`grep "^resource" .terraform/modules/<key>/main*.tf`和`grep "^module"`以找到确切的地址|
|`terraform plan`在导入资源上显示意外的`~ update`| Live值与AVM模块默认值不同|使用`az <resource> show`获取Live属性，与模块默认值比较，添加显式值|
|子资源声明为独立模块，即使父模块拥有它们|检查README中的必需输入，删除不正确的独立模块，并使用父模块的文档输入结构|建模子资源
|缺少中间模块路径，错误的映射键，或者缺少索引|检查模块块和`count`/`for_each`源；构建完整的嵌套导入地址，包括所有模块段和所需的key/index|
|工具试图读取ARM资源ID为文件pa|资源ID未被视为`--ids`输入，或者代理不信任已提供的作用域|严格将ARM ID视为云标识符，使用`az ... --ids ...`，一旦存在一个有效作用域就停止重新提示|##响应合同

当返回结果时，提供：

1. 使用的范围（订阅、资源组或资源id）
2. 创建的发现文件
3. 检测到的资源类型
4. AVM模块选择与版本
5. 生成或更新地形文件
6. 验证命令结果
7. 打开需要用户输入的空白（如果有的话）

代理的执行规则

-如果范围丢失，请不要继续。
在没有列出发现的文件和验证输出之前，不要声称导入成功。
-在生成Terraform之前不要跳过依赖映射。
-优先考虑AVM模块；明确地证明每个非avm回退。
- **在编写代码之前，请阅读每个AVM模块的README。**所需输入识别	which child resources the module owns. Optional Inputs document exact variable names and
	types. Usage examples show provider-specific conventions (`parent_id` vs
	`resource_group_name`). Skipping the README is the single most common cause of
	code errors in AVM-based imports.
—**不要假设网卡、扩展或公网IP资源是独立的。* *的	any AVM module, treat child resources as parent-owned unless the README explicitly indicates
	a separate module is required. Check Required Inputs before creating sibling modules.
- **不要从内存中写入导入地址。**`terraform init`后，执行grep命令	module source to discover the actual provider (`azurerm` vs `azapi`), resource labels,
	sub-module nesting, and `count` vs `for_each` usage before writing any `import {}` block.
- **永远不要把ARM资源id当作文件路径。**资源id属于Azure CLI`--ids`	arguments and API queries, not file IO tools. Only read local files when a real workspace
	path is provided.
- **当范围已知时尽量减少提示。**如果是订阅、资源组或	specific resource IDs are already provided, proceed with commands directly and only ask a
	follow-up when a command fails due to missing required context.
- **不声明导入完成，直到`terraform plan`显示0销毁和0	unwanted changes.** Telemetry `+ create` resources are acceptable. Any `~ update` or
	`- destroy` on real infrastructure resources must be resolved.
# #引用

- [Azure验证模块索引（Terraform）]（https://github.com/Azure/Azure-Verified-Modules/tree/main/docs/static/module-indexes）
- [Terraform AVM注册表名称空间]（https://registry.terraform.io/namespaces/Azure）