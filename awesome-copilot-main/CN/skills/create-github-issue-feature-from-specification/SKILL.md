---
name: create-github-issue-feature-from-specification
description: 'Create GitHub Issue for feature request from specification file using feature_request.yml template.'
---
#从规范中创建GitHub问题

在`${file}`为规范创建GitHub Issue。

# #过程

1. 分析规格文件，提取需求
2. 使用`search_issues`检查现有问题
3. 使用`create_issue`创建新问题或使用`update_issue`更新现有问题
4. 使用`feature_request.yml`模板（回退到默认值）

# #要求

-完整规格的单一问题
-清晰的标题识别规格
-仅包括规范要求的更改
-在创建之前对存在的问题进行验证

##发布内容

-标题：来自规范的功能名称
-描述：问题陈述、建议的解决方案和背景
-标签：特性、增强（视情况而定）