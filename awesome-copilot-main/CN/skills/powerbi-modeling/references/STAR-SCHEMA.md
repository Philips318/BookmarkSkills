Power BI的星型模式设计

# #概述

星型模式是Power BI语义模型的最佳设计模式。它将数据组织为：
- **维度表**：启用过滤和分组（“one”面）
- **事实表**：启用总结（“多”方面）

##表分类

维度表
—包含filtering/slicing的描述性属性
-具有唯一的键列（每个实体一行）
-示例：客户、产品、日期、地理、员工
-命名约定：单数名词（`Customer`,`Product`）

事实表
—包含可测量的定量数据
-具有维度的外键
-以一致的粒度存储数据（每transaction/event一行）
-例子：销售，订单，库存，网站访问
命名约定：业务流程名词（`Sales`、`Orders`）

设计原则

# # # 1。从事实中分离维度```
BAD:  Single denormalized "Sales" table with customer details
GOOD: "Sales" fact table + "Customer" dimension table
```
# # # 2。一致的粮食
事实表中的每一行都表示相同的东西：
-订单级别（最常见）
-每日汇总
-月度总结

不要在一张桌子里混合谷物。

# # # 3。代理键
当源缺乏唯一标识符时添加代理键：```m
// Power Query: Add index column
= Table.AddIndexColumn(Source, "CustomerKey", 1, 1)
```
# # # 4。日期维度
总是创建一个专用的日期表：
在Power BI中标记为日期表
-如有需要，包括财政期间
-添加相对日期列（IsCurrentMonth, IsPreviousYear）```dax
Date = 
ADDCOLUMNS(
    CALENDAR(DATE(2020,1,1), DATE(2030,12,31)),
    "Year", YEAR([Date]),
    "Month", FORMAT([Date], "MMMM"),
    "MonthNum", MONTH([Date]),
    "Quarter", "Q" & FORMAT([Date], "Q"),
    "WeekDay", FORMAT([Date], "dddd")
)
```
特殊维度类型

角色扮演维度
多次使用相同的维度（例如，OrderDate的Date， ShipDate）：
-选项1：复制表（OrderDate， ShipDate表）
-选项2：在DAX中与userrelationship使用非活动关系

缓慢变化的尺寸（类型2）
使用版本列跟踪历史更改：
—“开始日期”、“结束日期”列
- IsCurrent标志
-需要在数据仓库中进行预处理

垃圾尺寸
将低基数标志组合到一个表中：```
OrderFlags dimension: IsRush, IsGift, IsOnline
```
简并维度
在事实表中保留事务标识符（OrderNumber, InvoiceID）。

要避免的反模式

|反模式|问题|解决方案||--------------|---------|----------|
|性能差，难以维护|拆分为星型模式|
|雪花（标准化dim） |额外的连接影响性能|平坦尺寸|
|多对多无桥|歧义结果|添加bridge/junction表|
|混合的谷物事实|不正确的聚合|每个谷物单独的表|

验证检查表

-[]每个表都是明确的维度或事实
-[]事实表具有所有相关维度的外键
—[]维度有唯一的键列
—[]已存在日期表，并已标记
-[]没有循环关系路径
-[]一致的命名约定