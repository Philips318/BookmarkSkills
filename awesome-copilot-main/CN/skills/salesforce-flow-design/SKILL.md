---
name: salesforce-flow-design
description: 'Salesforce Flow architecture decisions, flow type selection, bulk safety validation, and fault handling standards. Use this skill when designing or reviewing Record-Triggered, Screen, Autolaunched, Scheduled, or Platform Event flows to ensure correct type selection, no DML/Get Records in loops, proper fault connectors on all data-changing elements, and appropriate automation density checks before deployment.'
---
# Salesforce流程设计和验证

将这些检查应用于您设计、构建或审查的每个流程。

##步骤1 -确认流程是正确的工具

在设计Flow之前，请验证轻量级声明性选项不能解决问题：

|需求|最佳工具||---|---|
|计算一个没有副作用的字段值|公式字段|
|使用用户消息防止坏记录保存|验证规则|
|对父字段|汇总汇总字段|上的子记录求和或计数
|复杂的多对象逻辑，callouts，或高容量| Apex（可排队/批处理）-不是Flow |
|其他一切|流动✓|

如果您正在构建一个可以被公式字段或验证规则取代的流，请用户确认需求确实更复杂。

##步骤2 -选择正确的流类型

|用例|流类型|键约束||---|---|---|
|保存前更新同一条记录的字段|保存前记录触发|不能发送邮件、进行标注、更改相关记录|
|Create/update相关记录、邮件、标注|保存后记录触发|提交后运行-避免递归陷阱|
|引导用户完成多步骤的UI流程|屏幕流程|不能被记录事件自动触发|
|从另一个流调用的可重用后台逻辑|自动启动（子流）|Input/output变量定义合约|
|从Apex调用的逻辑`@InvocableMethod`|自动启动（可调用）|必须声明input/output变量|
|基于时间的批处理|计划流|在批上下文中运行-尊重调控器限制|
|响应事件（平台事件/ CDC） |平台事件触发|异步运行-最终一致性|**决策规则**：当您只需要更改触发记录自己的字段时，选择before-save。当您需要触摸相关记录、发送电子邮件或进行标注时，移动到后保存。

##步骤3 -散装安全检查表

这些模式是大规模的调控器限制故障。在流被激活之前检查所有这些。

循环中的DML -自动失败```
Loop element
  └── Create Records / Update Records / Delete Records  ← ❌ DML inside loop
```
修复：收集循环内的记录到一个集合变量，然后运行DML元素**外**的循环。

在循环中获取记录-自动失败```
Loop element
  └── Get Records  ← ❌ SOQL inside loop
```
修复：在循环之前执行Get Records查询**，然后循环收集变量。

正确的批量模式```
Get Records — collect all records in one query
└── Loop over the collection variable
    └── Decision / Assignment (no DML, no Get Records)
└── After the loop: Create/Update/Delete Records — one DML operation
```
变换vs循环
当目标是重塑集合（例如将字段值从一个对象映射到另一个对象）时，使用**Transform**元素代替循环+赋值模式。Transform在设计上是批量安全的，并产生更清晰的流程图。

##步骤4 -故障路径要求

每个可能在运行时失败的元素都必须有一个故障连接器。没有故障路径的流程将原始系统错误暴露给用户。

需要故障连接器的元素
—创建记录
-更新记录
—删除记录
-获取记录（当访问可能不存在的所需记录时）
-发送电子邮件
—HTTP Callout / External Service动作
-顶点行动（可调用）
-子流程（如果子流程可以抛出错误）

故障处理程序模式```
Fault connector → Log Error (Create Records on a logging object or fire a Platform Event)
               → Screen element with user-friendly message (Screen Flows)
               → Stop / End element (Record-Triggered Flows)
```
永远不要将故障路径连接回发生故障的相同元素-这会创建一个无限循环。

##步骤5 -自动化密度检查

在部署之前，请验证在同一对象和触发事件上没有重叠的自动化：

—同一`Object`+`When to Run`组合上的其他活动记录触发流
-遗留流程生成器规则仍然在同一对象上活动
-工作流规则在同一领域的变化
- Apex触发器也运行在相同的`before insert`/`after update`上下文中

重叠的自动化可能导致意外的排序、递归和调控器限制失败。在激活之前记录对象的自动化清单。

##步骤6 -屏幕流程UX指南-通过Screen Flow的每个路径必须到达**End**元素-没有孤立分支。
-在多步骤流上提供一个**返回**导航选项，除非返回导航会损坏数据。
-所有用户输入都使用`lightning-input`和兼容slds的组件-不使用HTML表单元素。
-在用户可以提前之前，在屏幕上验证所需的输入-在屏幕上使用流验证规则。
-处理**Pause**元素，如果流可能需要等待用户跨会话的操作。

##步骤7 -部署安全```
Deploy as Draft    →   Test with 1 record   →   Test with 200+ records   →   Activate
```
-始终以**Draft**形式部署，并在激活前进行彻底测试。
-对于记录触发流：使用准确的入口条件进行测试（例如`ISCHANGED(Status)`-确保测试数据实际上触发了条件）。
-对于计划流程：在生产中启用之前，在沙箱中进行小批量测试。
-检查对象的自动化密度得分-单个对象上超过3个活动自动化会增加执行顺序风险。

##快速参考-流反模式总结

|反模式|风险|修复||---|---|---|
| DML元素在循环内|调控器限制异常|将DML移出循环|
|获取循环内的记录| SOQL调控器限制异常|循环前的查询|
|DML/email/callout元素|上无故障连接器|用户|未处理异常添加故障路径到每个这样的元素|
|在保存后流中更新无递归保护的触发记录|无限触发循环|添加条目条件或递归保护变量|
|直接在`$Record`集合上循环|在规模上的错误行为|首先分配给一个集合变量，然后循环|
| Process Builder仍然与新的Flow |一起激活，双执行，意外排序|在激活Flow |之前停用Process Builder
|屏幕流没有结束元素在所有分支|运行时错误或卡住用户|确保每个分支解析到一个结束元素|