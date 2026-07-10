---
name: create-github-issues-for-unmet-specification-requirements
description: 'Create GitHub Issues for unimplemented requirements from specification files using feature_request.yml template.'
---
#创建未满足规范要求的GitHub问题

在`${file}`的规范中为未实现的需求创建GitHub Issues。

# #过程

1. 分析规格文件，提取所有需求
2. 检查每个需求的代码库实现状态
3. 使用`search_issues`搜索现有问题，以避免重复
4. 使用`create_issue`为每个未实现的需求创建新问题
5. 使用`feature_request.yml`模板（回退到默认值）

# #要求

-规范中每个未实现的需求都有一个问题
—清除需求ID和描述映射
-包括实施指南和验收标准
-在创建之前对存在的问题进行验证

##发布内容

-标题：需求ID和简要描述
-描述：详细的需求、实现方法和上下文
-标签：特性、增强（视情况而定）

##实现检查-在代码库中搜索相关的代码模式
—查看`/spec/`目录下的相关规格文件
-验证需求没有部分实现