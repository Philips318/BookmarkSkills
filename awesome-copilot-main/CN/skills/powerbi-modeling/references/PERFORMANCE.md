Power BI模型的性能优化

数据简化技术

# # # 1。删除不需要的列
—只导入报表所需的列
-删除审计列（CreatedBy, ModifiedDate），除非需要
—删除duplicate/redundant列```
column_operations(operation: "List", filter: { tableNames: ["Sales"] })
// Review and remove unneeded columns
```
# # # 2。删除不必要的行
—将历史数据筛选到相关时间段
-排除不需要的cancelled/void事务
-在电源查询中应用过滤器（不在DAX中）

# # # 3。减少基数
高基数（许多独特的值）影响：
-模型大小
-刷新时间
—查询性能

* *解决方案:* *
|柱型|还原技术|-------------|---------------------|
| DateTime |分为日期和时间两列|
|十进制精度|四舍五入到所需精度|
|带有模式的文本|提取通用prefix/suffix|
|高精度id |使用代理整数键|

# # # 4。优化数据类型
|从|到|受益||------|-----|---------|
| DateTime |日期（如果不需要时间）| 8 ~ 4字节|
|十进制|固定十进制|更好的压缩|
|带数字的文本|整数|更好的压缩|
|长文|短文|减少存储|

# # # 5。分组总结
不需要详细信息时预聚合数据：
-每天而不是事务性的
-每月一次而不是每天一次
-考虑DirectQuery的聚合表

##列优化

###优先选择功率查询列而不是计算列
何时使用||----------|-------------|
|功率查询(M) |可以在源处计算，静态值|
|计算列（DAX） |需要模型关系，动态逻辑|

功率查询列：
-加载速度更快
-更好地压缩
使用更少的内存

避免在关系键上计算列
关系中的DAX计算列：
-不能使用索引
—为DirectQuery生成复杂的SQL
-显著伤害性能

**对于多列关系使用组合值：**```dax
// If you must use calculated column for composite key
CompositeKey = COMBINEVALUES(",", [Country], [City])
```
###设置适当的摘要
防止非相加列的意外聚合；```
column_operations(
  operation: "Update",
  definitions: [{
    tableName: "Product",
    name: "UnitPrice",
    summarizeBy: "None"
  }]
)
```
关系优化

# # # 1。尽量减少双向关系
各双向关系：
增加查询复杂度
-可以创建模糊路径
-降低性能

# # # 2。尽可能避免多对多
多对多的关系:
-生成更复杂的查询
-需要更多内存
-能产生意想不到的结果

# # # 3。减少关系基数
保持关系列的基数低：
-在文本上使用整数键
-考虑更高层次的关系

DAX优化

# # # 1。使用变量```dax
// GOOD - Calculate once, use twice
Sales Growth = 
VAR CurrentSales = [Total Sales]
VAR PriorSales = [PY Sales]
RETURN DIVIDE(CurrentSales - PriorSales, PriorSales)

// BAD - Recalculates [Total Sales] and [PY Sales]
Sales Growth = 
DIVIDE([Total Sales] - [PY Sales], [PY Sales])
```
# # # 2。避免对整个表使用FILTER```dax
// BAD - Iterates entire table
Sales High Value = 
CALCULATE([Total Sales], FILTER(Sales, Sales[Amount] > 1000))

// GOOD - Uses column reference
Sales High Value = 
CALCULATE([Total Sales], Sales[Amount] > 1000)
```
# # # 3。适当地使用KEEPFILTERS```dax
// Respects existing filters
Sales with Filter = 
CALCULATE([Total Sales], KEEPFILTERS(Product[Category] = "Bikes"))
```
# # # 4。除法运算符优于除法运算符```dax
// GOOD - Handles divide by zero
Margin % = DIVIDE([Profit], [Sales])

// BAD - Errors on zero
Margin % = [Profit] / [Sales]
```
## DirectQuery优化

# # # 1。最小化列和表
DirectQuery模型:
-查询来源的每一个可视化
—性能取决于源
-最小化数据检索

# # # 2。避免复杂的功率查询转换
—转换成为子查询
-本机查询更快
-尽可能从源头实现

# # # 3。一开始保持措施简单
复杂的DAX生成复杂的SQL：
-从基本聚合开始
-逐渐增加复杂性
—监控查询性能

# # # 4。禁用自动Date/Time对于DirectQuery模型，禁用autodate/time：
—创建隐藏的计算表
增加模型的复杂性
-使用显式日期表代替

# #聚合

###自定义聚合
预聚合事实表：
-非常大的模型（数十亿行）
—混合DirectQuery/Import-常见查询模式```
table_operations(
  operation: "Create",
  definitions: [{
    name: "SalesAgg",
    mode: "Import",
    mExpression: "..."
  }]
)
```
性能测试

使用性能分析器
1. 在Power BI Desktop中启用
2. 开始记录
3. 与视觉互动
4. 查看DAX查询次数

与DAX Studio监视器
外部工具用于：
—查询定时
-服务器计时
—执行计划

验证检查表

-[]删除不必要的列
-[]使用合适的数据类型
-[]处理高基数列
-[]最小化双向关系
—[]DAX对重复表达式使用变量
-[]在整个表上没有FILTER
-[]用除法代替除法运算符
- [] Autodate/timedisabled for DirectQuery
-[]性能测试具有代表性的数据