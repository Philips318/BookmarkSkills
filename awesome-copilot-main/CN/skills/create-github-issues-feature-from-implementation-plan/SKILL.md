---
name: create-github-issues-feature-from-implementation-plan
description: 'Create GitHub Issues from implementation plan phases using feature_request.yml or chore_request.yml templates.'
---
#创建GitHub问题从实施计划

在`${file}`上为实现计划创建GitHub Issues。

# #过程

1. 分析计划文件以确定阶段
2. 使用`search_issues`检查现有问题
3. 使用`create_issue`在每个阶段创建新问题或使用`update_issue`更新现有问题
4. 使用`feature_request.yml`或`chore_request.yml`模板（回退到默认值）

# #要求

-每个实施阶段一个问题
-清晰、有条理的标题和描述
-只包括计划所需的更改
-在创建之前对存在的问题进行验证

##发布内容

-标题：实施计划中的阶段名称
—描述：阶段细节、需求和上下文
-标签：适用于发行类型（feature/chore）