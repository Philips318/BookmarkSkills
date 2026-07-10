---
name: terraform-azurerm-set-diff-analyzer
description: Analyze Terraform plan JSON output for AzureRM Provider to distinguish between false-positive diffs (order-only changes in Set-type attributes) and actual resource changes. Use when reviewing terraform plan output for Azure resources like Application Gateway, Load Balancer, Firewall, Front Door, NSG, and other resources with Set-type attributes that cause spurious diffs due to internal ordering changes.
license: MIT
---
# Terraform AzureRM设置Diff分析器

识别由AzureRM Provider的Set-type属性引起的Terraform计划中的“假阳性差异”并将其与实际更改区分开来的技能。

##何时使用

-`terraform plan`显示了许多变化，但您只能added/removed单个元素
应用网关、负载均衡器、NSG等显示“所有元素都改变了”
—您希望自动过滤CI/CD中的假阳性差异

# #背景

Terraform的Set类型通过位置而不是键进行比较，所以当添加或删除元素时，所有元素都显示为“已更改”。这是一个常见的Terraform问题，但对于大量使用set类型属性（如Application Gateway、Load Balancer和NSG）的AzureRM资源来说，这个问题尤其明显。

这些“假阳性差异”实际上并不影响资源，但它们使审查地形计划输出变得困难。

# #先决条件- Python 3.8+

如果Python不可用，通过包管理器（例如，`apt install python3`,`brew install python3`）或从[python.org]（https://www.python.org/downloads/）安装。

##基本用法```bash
# 1. Generate plan JSON output
terraform plan -out=plan.tfplan
terraform show -json plan.tfplan > plan.json

# 2. Analyze
python scripts/analyze_plan.py plan.json
```
# #故障排除

- **`python: command not found`**：使用`python3`代替，或者安装Python
- **`ModuleNotFoundError`**：脚本只使用标准库；确保Python 3.8+

##详细文档

- [scripts/README.md](scripts/README.md) -所有选项，输出格式，退出代码，CI/CD示例
—[references/azurerm_set_attributes.md]（references/azurerm_set_attributes.md）—支持的资源和属性