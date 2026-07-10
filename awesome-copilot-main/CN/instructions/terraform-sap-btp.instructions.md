---
description: 'Terraform conventions and guidelines for SAP Business Technology Platform (SAP BTP).'
applyTo: '**/*.tf, **/*.tfvars, **/*.tflint.hcl, **/*.tf.json, **/*.tfvars.json'
---
在SAP BTP上进行Terraform -最佳实践和惯例

##核心原则

保持Terraform代码最小化、模块化、可重复、安全和可审计。
始终对Terraform HCL进行版本控制，从不对生成状态进行版本控制。

# #安全

强制性的:
-使用最新稳定的Terraform CLI和提供商版本；主动升级，获取安全补丁。
-不要提交秘密，凭据，证书，地形状态，或计划输出工件。
—将所有秘密变量和输出标记为`sensitive = true`。
-更喜欢临时/只写的提供者身份验证（Terraform >= 1.11），这样秘密就不会在状态中持续存在。
-尽量减少敏感输出；只排放下游自动化真正需要的东西。
-在CI中连续扫描`tfsec`，`trivy`,`checkov`（至少选择一个）。
-定期检查提供者凭据，旋转密钥，并在支持的情况下启用MFA。

# #模块化结构清晰和速度：
-按逻辑域划分（例如，授权、服务实例），而不是按环境划分。
-仅将模块用于可重用的多资源模式；避免使用单资源包装器模块。
保持模块层次较浅；避免深度嵌套和循环依赖。
-仅通过`outputs`暴露必要的跨模块数据（必要时标记为敏感）。

# #可维护性目标是显式b>隐式。
-评论“为什么”，而不是“什么”；避免重复明显的资源属性。
-参数化（变量）而不是硬编码；只在合理的情况下提供默认值。
-首选外部现有基础设施的数据来源；永远不要对在同一根目录下创建的资源使用输出。
-避免在通用可重用模块中使用数据源；而是要求输入。
-删除未使用/缓慢的数据源；它们降低了计划时间。
—对于派生表达式或重复表达式，使用`locals`来集中逻辑。

##样式和格式

# # #一般
-资源、变量、输出的描述性、一致性名称。
-变量和局部变量的snake_case。
- 2个空格缩进；`terraform fmt -recursive`运行。

布局和文件

推荐的结构:```text
my-sap-btp-app/
├── infra/                      # Root module
│   ├── main.tf                 # Core resources (split by domain when large)
│   ├── variables.tf            # Inputs
│   ├── outputs.tf              # Outputs
│   ├── provider.tf             # Provider config(s)
│   ├── locals.tf               # Local/derived values
│   └── environments/           # Environment var files only
│       ├── dev.tfvars
│       ├── test.tfvars
│       └── prod.tfvars
├── .github/workflows/          # CI/CD (if GitHub)
└── README.md                   # Documentation
```
规则:
-不要为每个环境（反模式）创建单独的branches/repos/folders。
-保持环境漂移最小化；在*中编码差异。只支持Tfvars文件。
-将超大的`main.tf`/`variables.tf`拆分为逻辑命名的片段（例如，`main_services.tf`,`variables_services.tf`）。
保持命名一致。

资源块组织

顺序（上→下）：可选`depends_on`，然后是`count`/`for_each`，然后是属性，最后是`lifecycle`。
-仅当Terraform无法推断依赖关系（例如，数据源需要授权）时使用`depends_on`。
-使用`count`作为可选的单个资源；`for_each`用于由映射键控的多个实例，用于稳定地址。
-组属性：首先是必需的，然后是可选的；逻辑部分之间的空白行。
-按字母顺序排列在一个部分内，以便更快地扫描。# # #变量
-每个变量：显式`type`，非空`description`。
-优选具体类型（`object`，`map(string)`等），而不是`any`。
避免集合的默认值为null；使用空的lists/maps代替。

# # #当地人
—集中计算或重复的表达式。
-将相关值分组到对象局部变量中以实现内聚。

# # #输出
-只暴露下游modules/automation消耗的内容。
—标记secrets`sensitive = true`。
-总是给出一个清晰的`description`。

###格式和线条
—执行`terraform fmt -recursive`命令（CI需要执行）。
-在预提交/ CI中强制执行`tflint`（以及可选的`terraform validate`）。

# #文档

强制性的:
-`description`+`type`对所有变量和输出。
一个简洁的根`README.md`：目的，先决条件，授权模型，使用（init/plan/apply），测试，回滚。
-生成模块文档与`terraform-docs`（添加到CI如果可能的话）。
-仅在澄清非明显决策或约束的地方注释。##状态管理
-使用支持锁定的远程后端（例如，Terraform Cloud, AWS S3， GCS, Azure Storage）。避免SAP BTP对象存储（可靠锁定和安全性的能力不足）。
-永远不要提交`*.tfstate`或备份。
-静态和传输中的加密状态；根据最小权限原则限制访问。

# #验证
—在提交前运行`terraform validate`（语法和内部检查）。
-在`terraform plan`（需要认证和全局帐户子域）之前与用户确认。通过envars或tvars提供授权；永远不要在提供程序块中内联秘密。
-首先在非产品中进行测试；确保幂等适用。

# #测试
-使用Terraform测试框架（`*.tftest.hcl`）测试模块逻辑和不变量。
-覆盖成功和失败路径；保持测试stateless/idempotent.-在可行的情况下更喜欢模拟外部数据源。

SAP BTP提供商细节指南:
—使用`data "btp_subaccount_service_plan"`解析业务计划id，并从该数据源引用`serviceplan_id`。

例子:```terraform
data "btp_subaccount_service_plan" "example" {
  subaccount_id = var.subaccount_id
  service_name  = "your_service_name"
  plan_name     = "your_plan_name"
}

resource "btp_subaccount_service_instance" "example" {
  subaccount_id  = var.subaccount_id
  serviceplan_id = data.btp_subaccount_service_plan.example.id
  name           = "my-example-instance"
}
```
显式依赖（提供程序不能推断）：```terraform
resource "btp_subaccount_entitlement" "example" {
  subaccount_id = var.subaccount_id
  service_name  = "your_service_name"
  plan_name     = "your_plan_name"
}

data "btp_subaccount_service_plan" "example" {
  subaccount_id = var.subaccount_id
  service_name  = "your_service_name"
  plan_name     = "your_plan_name"
  depends_on    = [btp_subaccount_entitlement.example]
}
```
认购也取决于应享权利；当提供程序不能通过属性推断链接时添加`depends_on`（匹配`service_name`/`plan_name`）。

##工具集成

### HashiCorp Terraform MCP Server
使用Terraform MCP Server进行交互式模式查找、资源块起草和验证。
1. 安装并运行服务器（参见https://github.com/mcp/hashicorp/terraform-mcp-server）。
2. 将其作为工具添加到您的Copilot / MCP客户端配置中。
3. 在创作之前查询提供程序模式（例如，列表资源、数据源）。
4. 生成草稿资源块，然后手动改进命名和标记标准。
5. 验证计划总结（不包括秘密）；在`apply`之前与审稿人确认差异。

Terraform注册表
参考SAP BTP提供程序文档：https://registry.terraform.io/providers/SAP/btp/latest/docs获取权威资源和数据源字段。如果不确定，请与注册表文档交叉检查MCP响应。

##反模式（避免）配置:
硬编码特定于环境的值（使用变量和tfvars）。
—例程使用`terraform import`（仅用于迁移）。
-深层/不透明的条件逻辑和降低清晰度的动态块。
-`local-exec`提供程序，除了不可避免的集成缺口。
-将SAP BTP提供商与Cloud Foundry提供商混合在同一根中，除非明确证明（拆分模块）。

安全:
—在HCL、state或VCS中存储秘密。
—禁用加密、验证或速度扫描。
-使用默认的passwords/keys或跨环境重用凭据。操作:
-直接生产无需事先进行非产品验证。
- terrraform外的手动漂移改变。
-忽略状态不一致/损坏症状。
-运行生产适用于不受控制的本地笔记本电脑（使用CI/CD或批准的运行器）。
-从原始`*.tfstate`读取业务数据，而不是从输出/数据源读取。

所有的更改必须通过Terraform CLI + HCL -永远不要手动改变状态。