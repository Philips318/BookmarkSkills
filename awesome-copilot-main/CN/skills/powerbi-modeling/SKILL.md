---
name: powerbi-modeling
description: 'Power BI semantic modeling assistant for building optimized data models. Use when working with Power BI semantic models, creating measures, designing star schemas, configuring relationships, implementing RLS, or optimizing model performance. Triggers on queries about DAX calculations, table relationships, dimension/fact table design, naming conventions, model documentation, cardinality, cross-filter direction, calculation groups, and data model best practices. Always connects to the active model first using power-bi-modeling MCP tools to understand the data structure before providing guidance.'
---
# Power BI语义建模

指导用户按照微软最佳实践构建优化的、文档完备的Power BI语义模型。

何时使用此技能

当用户询问以下问题时使用此技能：
-创建或优化Power BI语义模型
-设计星型模式（dimension/fact表）
-编写DAX测量或计算列
-配置表关系（基数，交叉过滤）
-实现行级安全（RLS）
-表、列、度量的命名约定
-为模型添加描述和文档
-性能调优
—计算组和字段参数
-模型验证和最佳实践检查

触发短语：“创建度量”、“添加关系”、“星型模式”、“优化模型”、“DAX公式”、“RLS”、“命名约定”、“模型文档”、“基数”、“交叉过滤”

# #先决条件必备工具
- **Power BI Modeling MCP Server**：用于连接和修改语义模型
-启用：connection_operations， table_operations, measure_operations， relationship_operations等。
-必须配置和运行与模型交互

###可选依赖项
- **Microsoft Learn MCP Server**：推荐用于研究最新的最佳实践
—启用：microsoft_docs_search， microsoft_docs_fetch
—用于复杂场景、新特性和正式文档

# #工作流程

# # # 1。先联系并分析

在提供任何建模指导之前，始终检查当前的模型状态：```
1. List connections: connection_operations(operation: "ListConnections")
2. If no connection, check for local instances: connection_operations(operation: "ListLocalInstances")
3. Connect to the model (Desktop or Fabric)
4. Get model overview: model_operations(operation: "Get")
5. List tables: table_operations(operation: "List")
6. List relationships: relationship_operations(operation: "List")
7. List measures: measure_operations(operation: "List")
```
# # # 2。评估模型运行状况

连接之后，根据最佳实践评估模型：

**星型模式**：表是否被正确地分类为维度或事实？
- **关系**：正确的基数？最小的双向过滤器？
- **命名**：人类可读，一致的命名约定？
**文档**：表、列、度量是否有描述？
- **措施**：关键计算的明确措施？
- **隐藏字段**：是否从报表视图隐藏技术列？

# # # 3。提供有针对性的指导

在分析的基础上，利用参考文献指导改进：
星型架构设计：参见[STAR-SCHEMA.md]（references/STAR-SCHEMA.md）
-关系配置：参见[RELATIONSHIPS.md]（references/RELATIONSHIPS.md）
- DAX测量和命名：见[MEASURES-DAX.md]（references/MEASURES-DAX.md）
-性能优化：见[PERFORMANCE.md]（references/PERFORMANCE.md）
-行级安全：参见[RLS.md]（references/RLS.md）

快速参考：模型质量检查表

区域|最佳实践||------|--------------|
|表|明确维度与事实分类|
|命名|人类可读：`Customer Name`而不是`CUST_NM`|
|记录的所有表、列、度量|
|度量|业务度量|的显式DAX度量
|关系|从维度到事实的一对多|
|交叉滤波|单一方向，除非特别需要|
|隐藏字段|隐藏技术关键字，id从报表视图|
|日期表|专用标记日期表|

## MCP工具参考

使用这些Power BI Modeling MCP操作：

|操作类别|关键操作|-------------------|----------------|
|`connection_operations`|连接，ListConnections, ListLocalInstances, ConnectFabric |
|`model_operations`|获取，GetStats, ExportTMDL |
|`table_operations`|列表，获取，创建，更新，GetSchema |
|`column_operations`|列表，获取，创建，更新(描述，隐藏，格式
|`measure_operations`|列表，获取，创建，更新，移动|
|`relationship_operations`| |的列表、获取、创建、更新、激活、去激活
|`dax_query_operations`|执行，验证|
|`calculation_group_operations`|列表、创建、更新|
|`security_role_operations`|列表、创建、更新、获取有效权限|

##常见任务

###添加测量与描述```
measure_operations(
  operation: "Create",
  definitions: [{
    name: "Total Sales",
    tableName: "Sales",
    expression: "SUM(Sales[Amount])",
    formatString: "$#,##0",
    description: "Sum of all sales amounts"
  }]
)
```
###更新列描述```
column_operations(
  operation: "Update",
  definitions: [{
    tableName: "Customer",
    name: "CustomerKey",
    description: "Unique identifier for customer dimension",
    isHidden: true
  }]
)
```
###创建关系```
relationship_operations(
  operation: "Create",
  definitions: [{
    fromTable: "Sales",
    fromColumn: "CustomerKey",
    toTable: "Customer",
    toColumn: "CustomerKey",
    crossFilteringBehavior: "OneDirection"
  }]
)
```
##何时使用微软学习MCP

研究当前使用`microsoft_docs_search`的最佳实践：
-最新的DAX函数文档
-新的Power BI特性和功能
-复杂的建模场景（SCD类型2，多对多）
-性能优化技术
-安全实现模式