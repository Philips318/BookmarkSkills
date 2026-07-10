# Terraform AzureRM设置Diff分析器脚本

一个Python脚本，用于分析Terraform计划JSON并识别AzureRM set类型属性中的“假阳性差异”。

# #概述

AzureRM提供程序的set类型属性（如`backend_address_pool`、`security_rule`等）不保证顺序，因此在添加或删除元素时，所有元素都显示为“已更改”。该脚本将这种“假阳性差异”与实际变化区分开来。

用例

-作为**座席技能**（推荐）
—作为**CLI工具**手动执行
-用于**CI/CD管道中的自动分析**

# #先决条件

- Python 3.8或更高版本
-不需要额外的软件包（仅使用标准库）

# #使用

###基本用法```bash
# Read from file
python analyze_plan.py plan.json

# Read from stdin
terraform show -json plan.tfplan | python analyze_plan.py
```
# # #选项

|选项|短|描述|默认值||--------|-------|-------------|---------|
|`--format`|`-f`|输出格式（markdown/json/summary） | markdown |
|`--exit-code`|`-e`|根据修改返回退出码| false |
|`--quiet`|`-q`|抑制警告| false |
|`--verbose`|`-v`|显示详细警告| false |
|`--ignore-case`| - |不区分大小写比较| false |
|`--attributes`| - |自定义属性定义文件|（内置）|的路径
|`--include`| - |过滤资源分析（可指定多个）| (all) |
|`--exclude`| - |过滤资源，排除（可指定多个）| (none) |

###退出码（带`--exit-code`）

|代码|含义||------|---------|
| 0 |没有更改，或者只更改|
| 1 |实际设置属性更改|
| 2 |资源替换（删除+创建）|
| 3 | |错误

##输出格式

### Markdown（默认）

PR评论和报告的人类可读格式。```bash
python analyze_plan.py plan.json --format markdown
```
# # # JSON

用于程序化处理的结构化数据。```bash
python analyze_plan.py plan.json --format json
```
示例输出:```json
{
  "summary": {
    "order_only_count": 3,
    "actual_set_changes_count": 1,
    "replace_count": 0
  },
  "has_real_changes": true,
  "resources": [...],
  "warnings": []
}
```
# # #总结CI/CD日志的一行摘要。```bash
python analyze_plan.py plan.json --format summary
```
示例输出:```
🟢 3 order-only | 🟡 1 set changes
```
##CI/CD管道使用### GitHub Actions

```yaml
name: Terraform Plan Analysis

on:
  pull_request:
    paths:
      - '**.tf'

jobs:
  analyze:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      
      - name: Setup Terraform
        uses: hashicorp/setup-terraform@v3
        
      - name: Terraform Init & Plan
        run: |
          terraform init
          terraform plan -out=plan.tfplan
          terraform show -json plan.tfplan > plan.json
          
      - name: Analyze Set Diff
        run: |
          python path/to/analyze_plan.py plan.json --format markdown > analysis.md
          
      - name: Comment PR
        uses: marocchino/sticky-pull-request-comment@v2
        with:
          path: analysis.md
```
###GitHub Actions（有出口码的门）```yaml
      - name: Analyze and Gate
        run: |
          python path/to/analyze_plan.py plan.json --exit-code --format summary
        # Fail on exit code 2 (resource replacement)
        continue-on-error: false
```
Azure管道```yaml
- task: TerraformCLI@0
  inputs:
    command: 'plan'
    commandOptions: '-out=plan.tfplan'

- script: |
    terraform show -json plan.tfplan > plan.json
    python scripts/analyze_plan.py plan.json --format markdown > $(Build.ArtifactStagingDirectory)/analysis.md
  displayName: 'Analyze Plan'

- task: PublishBuildArtifacts@1
  inputs:
    pathToPublish: '$(Build.ArtifactStagingDirectory)/analysis.md'
    artifactName: 'plan-analysis'
```
过滤示例

只分析特定的资源：```bash
python analyze_plan.py plan.json --include application_gateway --include load_balancer
```
排除特定资源：```bash
python analyze_plan.py plan.json --exclude virtual_network
```
##解释结果

|类别|含义|建议动作||----------|---------|-------------------|
|🟢仅订单|假阳性差异，没有实际更改|安全忽略|
|🟡实际更改|设置元素added/removed/modified|查看内容，通常就地更新|
|🔴资源替换| delete + create |检查停机对|的影响

自定义属性定义

默认情况下，使用`references/azurerm_set_attributes.json`，但你可以指定一个自定义文件：```bash
python analyze_plan.py plan.json --attributes /path/to/custom_attributes.json
```
有关定义文件格式，请参见`references/azurerm_set_attributes.md`。

# #的局限性

—只支持AzureRM资源（`azurerm_*`）
—某些resources/attributes可能不支持
-包含`after_unknown`的属性的比较可能是不完整的（应用后确定的值）
敏感属性的比较可能是不完整的（它们被屏蔽了）

##相关文档

- [SKILL.md](../SKILL.md) -作为座席技能使用
—[azurerm_set_attributes.md]（../references/azurerm_set_attributes.md）—属性定义引用