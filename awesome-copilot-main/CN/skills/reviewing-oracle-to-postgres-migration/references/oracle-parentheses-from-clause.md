# Oracle to PostgreSQL: FROM子句中有括号

# #内容

——问题
-根本原因
-解决方案模式
——示例
-迁移检查清单
-常用地点
-应用程序代码示例
-要注意的错误信息
-测试建议

# #的问题

Oracle允许在FROM子句中为表名加上括号：```sql
-- Oracle: Both are valid
SELECT * FROM (TABLE_NAME) WHERE id = 1;
SELECT * FROM TABLE_NAME WHERE id = 1;
```
PostgreSQL不允许在FROM子句中为单个表名加上额外的括号，除非它是派生表或子查询。尝试使用这种模式会导致：```
Npgsql.PostgresException: 42601: syntax error at or near ")"
```
根本原因

- **Oracle**：将`FROM(TABLE_NAME)`视为等同于`FROM TABLE_NAME`- **PostgreSQL**: FROM子句中的括号只在以下情况下有效：
—子查询：`FROM (SELECT * FROM table)`-显式表引用是连接语法的一部分
-通用表表达式（cte）
如果没有一个有效的SELECT或join上下文，PostgreSQL会抛出语法错误

解决方案模式

删除表名周围不必要的括号：```sql
-- Oracle (problematic in PostgreSQL)
SELECT col1, col2
FROM (TABLE_NAME)
WHERE id = 1;

-- PostgreSQL (correct)
SELECT col1, col2
FROM TABLE_NAME
WHERE id = 1;
```
# #的例子

例1：简单表引用```sql
-- Oracle
SELECT employee_id, employee_name
FROM (EMPLOYEES)
WHERE department_id = 10;

-- PostgreSQL (fixed)
SELECT employee_id, employee_name
FROM EMPLOYEES
WHERE department_id = 10;
```
例2：用圆括号连接```sql
-- Oracle (problematic)
SELECT e.employee_id, d.department_name
FROM (EMPLOYEES) e
JOIN (DEPARTMENTS) d ON e.department_id = d.department_id;

-- PostgreSQL (fixed)
SELECT e.employee_id, d.department_name
FROM EMPLOYEES e
JOIN DEPARTMENTS d ON e.department_id = d.department_id;
```
示例3：有效的子查询括号（适用于两者）```sql
-- Both Oracle and PostgreSQL
SELECT *
FROM (SELECT employee_id, employee_name FROM EMPLOYEES WHERE department_id = 10) sub;
```
##迁移清单

修复此问题时，请验证：

1. **找出所有有问题的FROM子句**：
—在SQL中搜索`FROM (`模式
-确认在`FROM`后面紧跟着一个表名的左括号
-确认它不是子查询（里面没有SELECT关键字）

2. **区分有效括号**：
-✅`FROM (SELECT ...)`-有效的子查询
-✅`FROM (table_name`后接一个join -检查是否接join关键字
-❌`FROM (TABLE_NAME)`-无效，删除括号

3. **应用修复**：
-去掉表名周围的圆括号
-为合法的子查询保留括号

4. 彻底的* * * *测试:
—在PostgreSQL中执行
-验证结果集是否匹配原始Oracle查询
-包括在集成测试中

##常见位置

搜索`FROM (`：-✅存储过程和函数（DDL脚本）
-✅应用数据访问层（DAL类）
-✅动态SQL生成器
-✅报告查询
-✅视图和物化视图
-✅具有多个连接的复杂查询

应用程序代码示例

# # # VB。网```vb
' Before (Oracle)
StrSQL = "SELECT employee_id, NAME " _
       & "FROM (EMPLOYEES) e " _
       & "WHERE e.department_id = 10"

' After (PostgreSQL)
StrSQL = "SELECT employee_id, NAME " _
       & "FROM EMPLOYEES e " _
       & "WHERE e.department_id = 10"
```
### c#```csharp
// Before (Oracle)
var sql = "SELECT id, name FROM (USERS) WHERE status = @status";

// After (PostgreSQL)
var sql = "SELECT id, name FROM USERS WHERE status = @status";
```
##要注意的错误信息```
Npgsql.PostgresException: 42601: syntax error at or near ")"
ERROR: syntax error at or near ")"
LINE 1: SELECT * FROM (TABLE_NAME) WHERE ...
                      ^
```
##测试建议

1. **语法验证**：解析所有迁移的查询，以确保它们运行时没有语法错误   ```csharp
   [Fact]
   public void GetEmployees_ExecutesWithoutSyntaxError()
   {
       // Should not throw PostgresException with error code 42601
       var employees = dal.GetEmployees(departmentId: 10);
       Assert.NotEmpty(employees);
   }
   ```
2. **结果比较**：检查迁移前后的结果集是否一致
3. **基于正则表达式的搜索**：使用模式`FROM\s*\(\s*[A-Za-z_][A-Za-z0-9_]*\s*\)`来识别候选对象

##相关文件

—参考：[oracle-to-postgres-type-coercion.md]（oracle-to-postgres-type-coercion.md）—其他语法差异
- PostgreSQL文档：[SELECT语句]（https://www.postgresql.org/docs/current/sql-select.html）

##迁移说明

-这是一个简单的语法修复，没有语义含义
—无需进行数据转换
-安全应用自动查找和替换，但手动验证复杂的查询
-更新集成测试以执行迁移的查询