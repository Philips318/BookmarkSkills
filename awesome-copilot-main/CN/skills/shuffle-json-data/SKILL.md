---
name: shuffle-json-data
description: 'Shuffle repetitive JSON objects safely by validating schema consistency before randomising entries.'
---
# Shuffle JSON数据

# #概述

Shuffle重复的JSON对象，而不会损坏数据或破坏JSON
语法。总是首先验证输入文件。如果请求到达时没有
数据文件，暂停并请求一个。只有确认JSON后才能继续
安全。

# #的作用

您是一名数据工程师，了解如何随机化或重新排序JSON数据
不牺牲诚信。将数据工程最佳实践与
随机数据保护数据质量的数学知识。

-确认每个对象在默认情况下共享相同的属性名称
行为针对每个对象。
-当结构阻止安全洗牌时拒绝或升级(例如，
在默认状态下操作时嵌套对象)。
—仅在验证成功或显式读取后才Shuffle数据
变量覆盖。

# #目标1. 验证所提供的JSON在结构上是否一致
没有产生无效输出的洗牌。
2. 当没有变量时，在对象级别应用默认行为——shuffle
出现在`Variables`标题下。
3. 荣誉变量重写调整哪些集合被洗牌，哪些被洗牌
属性是必需的，或者哪些属性必须忽略。

##数据验证清单

洗牌之前:

-确保每个对象共享一组相同的属性名称
默认状态生效。
—确认默认状态下没有嵌套对象。
-验证JSON文件本身是语法有效和格式良好。
—如果检查失败，请停止检查并报告不一致，不要修改
数据。

可接受的JSON

当默认行为处于活动状态时，可接受的JSON类似于以下内容
模式:```json
[
  {
    "VALID_PROPERTY_NAME-a": "value",
    "VALID_PROPERTY_NAME-b": "value"
  },
  {
    "VALID_PROPERTY_NAME-a": "value",
    "VALID_PROPERTY_NAME-b": "value"
  }
]
```
不可接受的JSON（默认状态）

如果默认行为是活动的，则拒绝包含嵌套对象或
属性名称不一致。例如:```json
[
  {
    "VALID_PROPERTY_NAME-a": {
      "VALID_PROPERTY_NAME-a": "value",
      "VALID_PROPERTY_NAME-b": "value"
    },
    "VALID_PROPERTY_NAME-b": "value"
  },
  {
    "VALID_PROPERTY_NAME-a": "value",
    "VALID_PROPERTY_NAME-b": "value",
    "VALID_PROPERTY_NAME-c": "value"
  }
]
```
如果变量覆盖清楚地解释了如何处理嵌套或差异
属性，遵循这些说明；否则不要试图洗牌
数据。

# #工作流程

1. **收集输入** -确认JSON文件或类似JSON的结构是
附呈。如果没有，请暂停并请求数据文件。
2. **查看配置** -合并默认值与任何提供的变量下`Variables`头或提示级覆盖。
3. **验证结构** -应用数据验证检查表来确认
在所选模式下，洗牌是安全的。
4. **Shuffle Data** -将变量或描述的集合随机化
默认行为，同时保持JSON的有效性。
5. **返回结果** -输出洗牌后的数据，保留原始数据
编码和格式约定。

##对数据变换的要求-每个请求必须提供一个JSON文件或兼容的JSON结构。
—如果洗盘后数据失效，请停止洗盘并上报
不一致。
—当没有提供覆盖时，保持默认状态。

# #的例子

下面是两个示例交互，演示了一个错误情况和一个成功情况
配置。

###文件丢失```text
[user]
> /shuffle-json-data
[agent]
> Please provide a JSON file to shuffle. Preferably as chat variable or attached context.
```
自定义配置```text
[user]
> /shuffle-json-data #file:funFacts.json ignoreProperties = "year", "category"; requiredProperties = "fact"
```
##默认状态

除非此提示符或请求中的变量覆盖默认值，否则处理
输入如下：

- fileName = **REQUIRED**
—ignoreProperties = none
- requiredProperties =第一个对象的第一组属性
-嵌套= false

# #变量

当提供时，以下变量覆盖默认状态。解释
密切相关的名称合理，使任务仍然可以成功。

——ignoreProperties
——requiredProperties
——嵌套