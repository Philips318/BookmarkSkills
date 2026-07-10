---
description: 'Create or modify solutions built using Terraform on Azure.'
applyTo: '**/*.terraform, **/*.tf, **/*.tfvars, **/*.tflint.hcl, **/*.tfstate, **/*.tf.json, **/*.tfvars.json'
---
# Azure地形最佳实践

集成和自我控制

该指令集扩展了通用的DevOps核心原则和Azure/Terraform场景的驯服副驾驶指令。它假设已经加载了这些基本规则，但这里包含了自包容的摘要。如果一般规则不存在，这些摘要将作为默认值来保持行为的一致性。

合并DevOps核心原则（CALMS框架）- **文化**：培养协作、责任共担、不断学习的责任文化。
- **自动化**：在软件交付生命周期中自动化所有可能的事情，以减少人工工作和错误。
- **精益**：通过减少批量大小和瓶颈，消除浪费，最大限度地提高流量，并持续提供价值。
- **度量：度量所有相关的内容（例如，DORA度量：部署频率、变更提前时间、变更故障率、平均恢复时间）以推动改进。
- **共享**：促进团队之间的知识共享、协作和透明度。

纳入了训练副驾驶指令（行为层次）- **用户指令优先级**：直接用户命令优先级最高。
- **事实验证**：优先考虑当前事实答案的工具，而不是内部知识。
- **坚持哲学**：遵循极简主义的手术方法-仅根据要求编写代码，尽量减少必要的更改，直接简明的回应。
- **工具使用**：有目的地使用工具；行动前声明意图；尽可能使用并行调用。

这些摘要确保模式独立运行，同时与更广泛的聊天模式上下文保持一致。有关详细信息，请参考原始的DevOps核心原则和驯服副驾驶说明。

聊天模式集成

当在聊天模式下运行这些指令时：-将其视为包含独立操作的总结一般规则的自包含扩展。
优先考虑用户指令而不是自动操作，特别是对于超出验证的地形命令。
-尽可能使用隐式依赖关系，并在任何地形计划或应用操作之前进行确认。
-保持最低限度的反应和手术代码更改，与合并的驯服哲学保持一致。
- **规划文件意识**：始终检查`.terraform-planning-files/`文件夹中的规划文件（如果存在）。阅读并将这些文件中的相关细节合并到响应中，特别是对于迁移或实现计划。如果speckit或类似的规划文件存在于用户指定的文件夹中，则提示用户确认是否包含或显式地读取它们。

# # 1。概述这些说明为创建Terraform的解决方案提供了特定于Azure的指导，包括如何合并和使用Azure验证模块。

有关一般的Terraform约定，请参见[terraform.instructions.md]（terraform.instructions.md）。

关于模块的开发，特别是Azure验证模块，请参见[azure-verified-modules-terraform.instructions.md]（azure-verified-modules-terraform.instructions.md）。

# # 2。要避免的反模式

配置:* * * *

- MUST NOT硬编码应该参数化的值
-不应该使用`terraform import`作为常规的工作流模式
应该避免复杂的条件逻辑，使代码难以理解
-除非绝对必要，否则绝对不要使用`local-exec`提供程序

* *安全:* *

-绝对不能在地形文件或状态中存储秘密
-必须避免过于宽松的IAM角色或网络规则
-绝对不能为了方便而禁用安全功能
-绝对不能使用默认密码或密钥

* *操作:* *-绝对不能在没有测试的情况下直接将Terraform更改应用到生产中
-必须避免手动更改地形管理的资源
-绝对不能忽略地形状态文件的损坏或不一致
-绝对不能在本地机器上运行Terraform
-必须只使用Terraform状态文件（`**/*.tfstate`）进行只读操作，所有更改必须通过Terraform CLI或HCL进行。
-必须只使用`**/.terraform/**`（获取的模块和提供程序）的内容进行只读操作。

这些都是建立在为安全、可操作的实践而合并的驯服副驾驶指令的基础上的。

---

# # 3。清晰地组织代码

用逻辑文件分离构造地形配置：—资源使用“`main.tf`”
—输入使用`variables.tf`—输出使用“`outputs.tf`”
—使用`terraform.tf`进行提供程序配置
-使用`locals.tf`抽象复杂的表达式和更好的可读性
-遵循一致的命名约定和格式（`terraform fmt`）
-如果主。f或变量。Tf文件太大，按资源类型或函数将它们分割成多个文件（例如，`main.networking.tf`，`main.storage.tf`-将等效变量移动到`variables.networking.tf`等）

使用`snake_casing`作为变量和模块名。

# # 4。使用Azure验证模块（AVM）

如果可用，任何重要资源都应该使用AVM。avm被设计成与良好架构框架保持一致，由微软支持和维护，以帮助减少需要维护的代码量。有关如何发现这些模块的信息，请参见[Azure验证模块用于Terraform]（azure-verified-modules-terraform.instructions.md）。如果资源中没有Azure验证模块，建议创建一个“以”AVM的风格的模块，以便与现有的工作保持一致，并提供一个为上游社区做出贡献的机会。

此指令的例外情况是，如果用户已被指示使用内部私有注册表，或明确表示他们不希望使用Azure Verified Modules。

通过利用预先验证的、社区维护的模块，这与合并的DevOps自动化原则保持一致。

# # 5。变量和代码风格标准

在解决方案代码中遵循与avm一致的编码标准以保持一致性；- **变量命名**：使用snake_case所有变量名（每个TFNFR4和TFNFR16）。描述和一致的命名约定。
—**变量定义**：所有变量必须有明确的类型声明（符合TFNFR18）和全面的描述（符合TFNFR17）。避免集合值的默认值为空（根据TFNFR20），除非有特殊需要。
- **敏感变量**：适当标记敏感变量，避免显式设置`sensitive = false`（根据TFNFR22）。正确处理敏感的默认值（根据TFNFR23）。
- **动态块**：在适当的地方为可选的嵌套对象使用动态块（根据TFNFR12），并利用`coalesce`或`try`函数作为默认值（根据TFNFR13）。
- **代码组织**：考虑专门为本地值使用`locals.tf`（根据TFNFR31），并确保本地值的精确输入（根据TFNFR33）。

# # 6。秘密最好的秘密是不需要储存的秘密。例如，使用受管理的身份而不是密码或密钥。

在支持（Terraform v1.11+）的情况下，使用`ephemeral`秘密和只写参数，以避免在状态文件中存储秘密。请查阅模块文档了解可用性。

在需要秘密的地方，除非指示使用不同的服务，否则存储在密钥库中。

永远不要将秘密写入本地文件系统或提交到git。

适当地标记敏感值，将它们与其他属性隔离开来，除非绝对必要，否则避免输出敏感数据。遵循TFNFR19、TFNFR22和TFNFR23。

# # 7。输出

- **避免不必要的输出**，仅用于暴露其他配置需要的信息。
—包含秘密的输出使用“`sensitive = true`”
-为所有输出提供清晰的描述```hcl
output "resource_group_name" {
  description = "Name of the created resource group"
  value       = azurerm_resource_group.example.name
}

output "virtual_network_id" {
  description = "ID of the virtual network"
  value       = azurerm_virtual_network.example.id
}
```
# # 8。本地值

—对计算值和复杂表达式使用局部变量
—通过提取重复表达式提高可读性
—将相关值组合成结构化的局部变量```hcl
locals {
  common_tags = {
    Environment = var.environment
    Project     = var.project_name
    Owner       = var.owner
    CreatedBy   = "terraform"
  }
  
  resource_name_prefix = "${var.project_name}-${var.environment}"
  location_short       = substr(var.location, 0, 3)
}
```
# # 9。遵循推荐的Terraform实践

- **冗余的depends_on检测**：搜索并删除`depends_on`，其中依赖的资源已经在同一资源块中隐式引用。仅在显式需要的地方保留`depends_on`。永远不要依赖模块输出。

- **迭代**:0-1个资源使用`count`，多个资源使用`for_each`。对于稳定的资源地址，首选映射。与TFNFR7保持一致。

- **数据源**：在根模块中可以接受，但避免在可重用模块中使用。首选显式模块参数而不是数据源查找。

- **参数化**：使用具有显式`type`声明（TFNFR18），全面描述（TFNFR17）和不可空默认值（TFNFR20）的强类型变量。利用avm公开的变量。- **版本控制**：目标最新稳定的Terraform和Azure提供程序版本。在代码中指定版本并保持更新（TFFR3）。

# # 10。文件夹结构

对Terraform配置使用一致的文件夹结构。

使用tfvar修改环境差异。一般来说，目标是保持环境相似，同时对非生产环境进行成本优化。

反模式——每个环境有分支、每个环境有存储库、每个环境有文件夹——或者类似的布局，使得很难测试环境之间的根文件夹逻辑。

注意像Terragrunt这样的工具可能会影响这种设计。

**建议的**结构是：```text
my-azure-app/
├── infra/                          # Terraform root module (AZD compatible)
│   ├── main.tf                     # Core resources
│   ├── variables.tf                # Input variables
│   ├── outputs.tf                  # Outputs
│   ├── terraform.tf                # Provider configuration
│   ├── locals.tf                   # Local values
│   └── environments/               # Environment-specific configurations
│       ├── dev.tfvars              # Development environment
│       ├── test.tfvars             # Test environment
│       └── prod.tfvars             # Production environment
├── .github/workflows/              # CI/CD pipelines (if using github)
├── .azdo/                          # CI/CD pipelines (suggested if using Azure DevOps)
└── README.md                       # Documentation
```
未经用户直接同意，切勿更改文件夹结构。

遵循AVM规范TFNFR1、TFNFR2、TFNFR3和TFNFR4，以获得一致的文件命名和结构。

azure特定的最佳实践

资源命名和标记

-遵循[Azure命名约定]（https://learn.microsoft.com/en-us/azure/cloud-adoption-framework/ready/azure-best-practices/resource-naming）
—多区域部署时，使用一致的区域命名和变量
—实现标签一致性。

资源组策略

—使用已有的资源组
—创建新的资源组时，需要经过确认
—使用说明目的和环境的描述性名称

网络方面的考虑-在创建新的网络资源之前，验证现有的VNet/subnetid（例如，此解决方案是否部署到现有的hub & spoke着陆区）
—合理使用nsg和asg
—需要时，实现PaaS服务的私有端点，不需要时，使用资源防火墙限制公网访问。在需要公共端点的地方注释异常。

安全性和合规性

-使用受管理的身份而不是服务主体
-在适当的RBAC下实施密钥库。
—开启审计跟踪诊断设置
—遵循最小权限原则

成本管理

-确认昂贵资源的预算批准
-使用适合环境的大小（dev vs prod）
-要求成本限制，如果没有指定

##状态管理-使用状态锁定的远程后端（Azure Storage）
-永远不要将状态文件提交到源代码控制系统
—启用静态和传输中的加密

# #验证

-对现有资源进行盘点，并主动移除未使用的资源块。
—执行`terraform validate`命令检查语法
—运行`terraform plan`前请先询问。Terraform计划将需要一个订阅ID，这应该来自ARM_SUBSCRIPTION_ID环境变量，*NOT*在provider块中编码。
—首先在非生产环境中测试配置
-确保幂等性（多次应用产生相同的结果）

##后退行为

如果没有加载一般规则，则默认为：极简代码生成，对超出validate的任何地形命令显式同意，并在所有建议中遵守CALMS原则。