# Power BI中的关系

##关系属性

# # #基数
|类型|用例|备注||------|----------|-------|
|一对多（*:1）|维度到事实|最常见，首选|
|多对一（1:*）|事实到维度|同上，方向相反|
|一对一（1:1）|维度扩展|谨慎使用|
|多对多（*:*）|桥接表，复杂场景|需要精心设计|

交叉过滤方向
|设置|行为|何时使用||---------|----------|-------------|
| Single |过滤器从“one”流到“many”|默认值，最佳性能|
|两个|两个方向的过滤器|必要时才使用|

最佳实践

# # # 1。偏好一对多关系```
Customer (1) --> (*) Sales
Product  (1) --> (*) Sales
Date     (1) --> (*) Sales
```
# # # 2。使用单向交叉滤波
双向过滤:
-对工作表现有负面影响
-可以创建模糊的过滤路径
-可能产生意想不到的结果

**仅当：**时使用双向
-通过事实表进行维度到维度分析
—特定的RLS要求

**更好的选择：**在DAX测量中使用CROSSFILTER：```dax
Countries Sold = 
CALCULATE(
    DISTINCTCOUNT(Customer[Country]),
    CROSSFILTER(Customer[CustomerKey], Sales[CustomerKey], BOTH)
)
```
# # # 3。表之间有一条活动路径
—任意两个表之间只有一个活动关系
-使用用户关系的角色扮演维度：```dax
Sales by Ship Date = 
CALCULATE(
    [Total Sales],
    USERELATIONSHIP(Sales[ShipDate], Date[Date])
)
```
# # # 4。避免歧义路径
循环引用会导致错误。解决方案:
—取消一个关系
-重组模型
-在度量中使用userrelationship

关系模式

标准星型模式```
     [Date]
       |
[Product]--[Sales]--[Customer]
       |
   [Store]
```
角色扮演维度```
[Date] --(active)-- [Sales.OrderDate]
   |
   +--(inactive)-- [Sales.ShipDate]
```
桥接表（多对多）```
[Customer]--(*)--[CustomerAccount]--(*)--[Account]
```
Factless事实表```
[Product]--[ProductPromotion]--[Promotion]
```
用于捕捉没有度量的关系。

##通过MCP创建关系

###列出当前关系```
relationship_operations(operation: "List")
```
创建新关系```
relationship_operations(
  operation: "Create",
  definitions: [{
    fromTable: "Sales",
    fromColumn: "ProductKey",
    toTable: "Product", 
    toColumn: "ProductKey",
    crossFilteringBehavior: "OneDirection",
    isActive: true
  }]
)
```
###取消关系```
relationship_operations(
  operation: "Deactivate",
  references: [{ name: "relationship-guid-here" }]
)
```
# #故障排除

“模糊路径”错误
表之间存在多个活动路径。
-检查：多个事实表共享维度
—解决方法：取消冗余关系

不允许双向
将创建循环引用。
-解决方案：重组或使用DAX CROSSFILTER

未检测到关系
列可以有不同的数据类型。
-确保两列具有相同的类型
-检查文本键中的尾随空格

验证检查表

-[]所有关系都是一对多的
—[]交叉过滤默认为单向过滤
-[]任意两个表之间只有一条活动路径
-[]角色扮演维度使用非活动关系
-[]没有圆形参考路径
—[]关键列的数据类型匹配