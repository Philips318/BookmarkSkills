# AzureRM Set-Type属性引用

本文档介绍`azurerm_set_attributes.json`的概述和维护。

> **最后更新：2026年1月28日

# #概述`azurerm_set_attributes.json`是AzureRM提供程序中作为set类型处理的属性的定义文件。`analyze_plan.py`脚本读取该JSON以识别Terraform计划中的“假阳性差异”。

什么是Set-Type属性？

Terraform的Set类型是一个不保证顺序的集合。
因此，在添加或删除元素时，未更改的元素可能会显示为“已更改”。
这被称为“假阳性差”。

JSON文件结构

基本格式```json
{
  "resources": {
    "azurerm_resource_type": {
      "attribute_name": "key_attribute"
    }
  }
}
```
- **key_attribute**：唯一标识Set元素的属性（例如，`name`,`id`）
- **null**：当没有键属性时（比较整个元素）

嵌套格式

当一个Set属性包含另一个Set属性时：```json
{
  "rewrite_rule_set": {
    "_key": "name",
    "rewrite_rule": {
      "_key": "name",
      "condition": "variable",
      "request_header_configuration": "header_name"
    }
  }
}
```
**`_key`**：该关卡的Set元素的关键属性
- **其他键**：嵌套Set属性的定义

示例：azurerm_application_gateway```json
"azurerm_application_gateway": {
  "backend_address_pool": "name",           // Simple Set (key is name)
  "rewrite_rule_set": {                     // Nested Set
    "_key": "name",
    "rewrite_rule": {
      "_key": "name",
      "condition": "variable"
    }
  }
}
```
# #维护

添加新属性

1. **查看官方文档**
-在[Terraform Registry]（https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs）中搜索资源
-验证属性列表为“Set of…”
一些资源如`azurerm_application_gateway`有明确的Set属性

2. **检查源代码（更可靠）**
-在[AzureRM Provider GitHub]（https://github.com/hashicorp/terraform-provider-azurerm）中搜索资源
—确认模式定义中的`Type: pluginsdk.TypeSet`-在Set的`Schema`中识别可以用作`_key`的属性

3. **添加到JSON**   ```json
   "azurerm_new_resource": {
     "set_attribute": "key_attribute"
   }
   ```
4. * * * *测试   ```bash
   # Verify with an actual plan
   python3 scripts/analyze_plan.py your_plan.json
   ```
识别关键属性

|常用关键属性|用途| . ||---------------------|-------|
|`name`|命名块（最常见）|
|`id`|资源ID引用|
|`location`|地理位置|
|`address`|网络地址|
|`host_name`|主机名|
|`null`|当没有键存在时（比较整个元素）|

##相关工具### analyze_plan.py
分析Terraform计划JSON以识别假阳性差异。```bash
# Basic usage
terraform show -json plan.tfplan | python3 scripts/analyze_plan.py

# Read from file
python3 scripts/analyze_plan.py plan.json

# Use custom attribute file
python3 scripts/analyze_plan.py plan.json --attributes /path/to/custom.json
```
支持的资源

当前支持的资源请直接参考`azurerm_set_attributes.json`：```bash
# List resources
jq '.resources | keys' azurerm_set_attributes.json
```
关键资源:
-`azurerm_application_gateway`-后端池，监听器，规则等
—`azurerm_firewall_policy_rule_collection_group`—规则集合
-`azurerm_frontdoor`-后端池，路由
—`azurerm_network_security_group`—安全规则
—`azurerm_virtual_network_gateway`—IP配置、VPN客户端配置

# #笔记

—属性行为可能因Provider/API版本而异
-新的资源和属性需要添加，因为他们变得可用
-定义所有层次的深度嵌套结构可以提高准确性