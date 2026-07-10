# DAX度量和命名约定

命名约定

###一般规则
-使用人类可读的名字（允许空格）
—描述性：`Total Sales Amount`，而不是`TSA`-避免使用缩写，除非大家都能理解
-使用一致的大写字母（建议使用标题大小写）
—避免使用除空格外的特殊字符

表命名
|类型|约定|示例||------|------------|---------|
|单数名词|客户、产品、日期|
|事实|业务流程|销售、订单、库存|
|桥|组合名称|客户帐户，产品类别|
|下划线前缀| _Measures， _kpi

###列命名
|类型|约定|示例||------|------------|---------|
|带有“Key”或“ID”的后缀| CustomerKey， ProductID |
|日期|带“日期”后缀| OrderDate， ShipDate |
|金额|描述与单位提示|销售金额，数量售出|
|标记|前缀“是”或“有”| IsActive, HasDiscount |

###衡量命名
|类型|约定|示例||------|------------|---------|
|聚合|动词+名词|总销售额，订单数量|
|比率| X / Y或X Rate |每个客户的销售额，转化率|
|时间情报|时期+指标|年初至今销售额，全年总销售额|
|比较|指标+与+基线|销售与预算，增长与日元|

显式措施vs隐式措施

###总是创建明确的措施：
1. 用户将查询的关键业务指标
2. 复杂的计算与过滤器操作
3. MDX中使用的度量（Excel数据透视表）
4. 控制聚合（防止平均值之和）

隐式度量（列聚合）
-可接受的简单探索
—设置正确的SummarizeBy属性：
-金额：
—Keys/IDs: None （Do Not Summarize）
—Rates/Prices：无或平均

##衡量模式

基本聚合```dax
Total Sales = SUM(Sales[SalesAmount])
Order Count = COUNTROWS(Sales)
Average Order Value = DIVIDE([Total Sales], [Order Count])
Distinct Customers = DISTINCTCOUNT(Sales[CustomerKey])
```
###时间智能（需要日期表）```dax
YTD Sales = TOTALYTD([Total Sales], 'Date'[Date])
MTD Sales = TOTALMTD([Total Sales], 'Date'[Date])
PY Sales = CALCULATE([Total Sales], SAMEPERIODLASTYEAR('Date'[Date]))
YoY Growth = DIVIDE([Total Sales] - [PY Sales], [PY Sales])
```
百分比计算```dax
Sales % of Total = 
DIVIDE(
    [Total Sales],
    CALCULATE([Total Sales], REMOVEFILTERS(Product))
)

Margin % = DIVIDE([Gross Profit], [Total Sales])
```
###运行总数```dax
Running Total = 
CALCULATE(
    [Total Sales],
    FILTER(
        ALL('Date'),
        'Date'[Date] <= MAX('Date'[Date])
    )
)
```
##列引用

最佳实践：始终限定列名```dax
// GOOD - Fully qualified
Sales Amount = SUM(Sales[SalesAmount])

// BAD - Unqualified (can cause ambiguity)
Sales Amount = SUM([SalesAmount])
```
度量参考：永远不要限定```dax
// GOOD - Unqualified measure
YTD Sales = TOTALYTD([Total Sales], 'Date'[Date])

// BAD - Qualified measure (breaks if home table changes)
YTD Sales = TOTALYTD(Sales[Total Sales], 'Date'[Date])
```
# #文档

度量描述
总是添加描述来解释：
-度量计算的内容
—业务context/usage-任何重要的假设```
measure_operations(
  operation: "Update",
  definitions: [{
    name: "Total Sales",
    tableName: "Sales",
    description: "Sum of all completed sales transactions. Excludes returns and cancelled orders."
  }]
)
```
###格式化字符串
|数据类型|格式字符串|输出示例||-----------|---------------|----------------|
| $#,##0.00 | $1,234.56 |
|百分比| 0.0% | 12.3% |
|整数| #，##0 | 1,234 |
|十进制| #，##0.00 | 1,234.56 |

##显示文件夹

将措施组织成逻辑组：```
measure_operations(
  operation: "Update",
  definitions: [{
    name: "YTD Sales",
    tableName: "_Measures",
    displayFolder: "Time Intelligence\\Year"
  }]
)
```
常用文件夹结构：```
_Measures
├── Sales
│   ├── Total Sales
│   └── Average Sale
├── Time Intelligence
│   ├── Year
│   │   ├── YTD Sales
│   │   └── PY Sales
│   └── Month
│       └── MTD Sales
└── Ratios
    ├── Margin %
    └── Conversion Rate
```
##性能变量

使用变量来：
—避免重复计算相同的表达式
-提高可读性
—打开调试开关```dax
Gross Margin % = 
VAR TotalSales = [Total Sales]
VAR TotalCost = [Total Cost]
VAR GrossProfit = TotalSales - TotalCost
RETURN
    DIVIDE(GrossProfit, TotalSales)
```
验证检查表

-[]所有关键业务指标都有明确的衡量标准
—[]度量具有清晰、描述性的名称
—[]措施有描述
-[]应用合适的格式字符串
-[]显示文件夹，组织相关措施
—[]列引用是完全限定的
-[]测量引用不合格
-[]用于复杂计算的变量