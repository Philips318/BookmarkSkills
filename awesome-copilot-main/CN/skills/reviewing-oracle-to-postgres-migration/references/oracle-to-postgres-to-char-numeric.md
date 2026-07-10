# Oracle到PostgreSQL: TO_CHAR（）数字转换

# #内容

——问题
-根本原因
-解决方案模式- CAST，格式字符串，连接
-迁移检查清单
-应用程式代码评审
-测试建议
-常用地点
-要注意的错误信息

# #的问题

Oracle允许`TO_CHAR()`在没有格式说明符的情况下将数字类型转换为字符串：```sql
-- Oracle: Works fine
SELECT TO_CHAR(vessel_id) FROM vessels;
SELECT TO_CHAR(fiscal_year) FROM certificates;
```
当对数字类型使用`TO_CHAR()`时，PostgreSQL需要一个格式字符串，否则会引发：```
42883: function to_char(numeric) does not exist
```
根本原因

- **Oracle**：不带格式掩码的`TO_CHAR(number)`使用默认格式隐式地将数字转换为字符串
- **PostgreSQL**:`TO_CHAR()`总是需要一个显式的格式字符串为数字类型（例如，`'999999'`,`'FM999999'`）

解决方案模式

模式1：使用CAST（推荐）

最干净的迁移方法是将`TO_CHAR(numeric_column)`替换为`CAST(numeric_column AS TEXT)`：```sql
-- Oracle
SELECT TO_CHAR(vessel_id) AS vessel_item FROM vessels;

-- PostgreSQL (preferred)
SELECT CAST(vessel_id AS TEXT) AS vessel_item FROM vessels;
```
* *优势:* *

-在PostgreSQL中更习惯
-更明确的意图
-不需要格式字符串

模式2：提供格式字符串

如果需要特定的数字格式，请使用显式格式掩码：```sql
-- PostgreSQL with format
SELECT TO_CHAR(vessel_id, 'FM999999') AS vessel_item FROM vessels;
SELECT TO_CHAR(amount, 'FM999999.00') AS amount_text FROM payments;
```
* *格式面具:* *

-`'FM999999'`：固定宽度的整数（FM =填充模式，删除前导空格）
-`'FM999999.00'`: 2位小数
-`'999,999.00'`：带千个分隔符

模式3：字符串连接

对于隐式数字转换的简单连接：```sql
-- Oracle
WHERE TO_CHAR(fiscal_year) = '2024'

-- PostgreSQL (using concatenation)
WHERE fiscal_year::TEXT = '2024'
-- or
WHERE CAST(fiscal_year AS TEXT) = '2024'
```
##迁移清单

迁移包含`TO_CHAR()`的SQL时：

1. **识别所有TO_CHAR（）调用**：在SQL字符串、存储过程和应用程序查询中搜索`TO_CHAR\(`2. **检查参数类型**：
**DATE/TIMESTAMP**：保留`TO_CHAR()`格式字符串（例如，`TO_CHAR(date_col, 'YYYY-MM-DD')`）
—**NUMERIC/INTEGER**：替换为`CAST(... AS TEXT)`或添加格式字符串
3. **测试输出**：验证字符串表示是否符合预期（没有意外的空格，小数等）
4. **更新比较逻辑**：如果比较数字和字符串，确保两边的类型一致

应用程序代码审查

### c#示例```csharp
// Before (Oracle)
var sql = "SELECT TO_CHAR(id) AS id_text FROM entities WHERE TO_CHAR(status) = @status";

// After (PostgreSQL)
var sql = "SELECT CAST(id AS TEXT) AS id_text FROM entities WHERE CAST(status AS TEXT) = @status";
```
##测试建议

1. **单元测试**：验证数字到字符串的转换返回预期的值   ```csharp
   [Fact]
   public void GetVesselNumbers_ReturnsVesselIdsAsStrings()
   {
       var results = dal.GetVesselNumbers(certificateType);
       Assert.All(results, item => Assert.True(int.TryParse(item.DISPLAY_MEMBER, out _)));
   }
   ```
2. **集成测试**：确保使用`CAST()`执行查询没有错误
3. **比较测试**：验证带有数字到字符串比较过滤器的WHERE子句是否正确

##常见位置

搜索`TO_CHAR`：

-✅存储过程和函数（DDL脚本）
-✅应用数据访问层（DAL类）
-✅动态SQL生成器
-✅报告查询
-✅ORM/Entity框架原始SQL

##要注意的错误信息```
Npgsql.PostgresException: 42883: function to_char(numeric) does not exist
Npgsql.PostgresException: 42883: function to_char(integer) does not exist
Npgsql.PostgresException: 42883: function to_char(bigint) does not exist
```
##参见Also

- [oracle-to-postgres-type-coercion.md](oracle-to-postgres-type-coercion.md) -相关类型转换问题
PostgreSQL文档：[数据类型格式化函数]（https://www.postgresql.org/docs/current/functions-formatting.html）