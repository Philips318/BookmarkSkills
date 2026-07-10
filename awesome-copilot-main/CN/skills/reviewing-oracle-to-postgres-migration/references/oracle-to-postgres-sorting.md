# Oracle到PostgreSQL排序迁移指南

目的：在将查询移动到PostgreSQL时保留类似oracle的排序语义。

##要点
Oracle通常将普通的`ORDER BY`视为binary/byte-wise，为ASCII提供不区分大小写的排序。
- PostgreSQL默认值不同；为了匹配Oracle的行为，在排序表达式上使用`COLLATE "C"`。

标准`SELECT … ORDER BY`**目标：**保持oracle式的排序。

* *模式:* *```sql
SELECT col1
FROM your_table
ORDER BY col1 COLLATE "C";
```
* *注:* *
-对每个必须模仿Oracle的排序表达式应用`COLLATE "C"`。
-工作与ascending/descending和多列排序，如`ORDER BY col1 COLLATE "C", col2 COLLATE "C" DESC`。`SELECT DISTINCT … ORDER BY`**问题：** PostgreSQL强制`ORDER BY`表达式出现在`SELECT`列表中的`DISTINCT`，引发：`Npgsql.PostgresException: 42P10: for SELECT DISTINCT, ORDER BY expressions must appear in select list`**Oracle差异：** Oracle允许在使用`DISTINCT`时按非投影表达式排序。

**推荐模式（包装和排序）：**```sql
SELECT *
FROM (
  SELECT DISTINCT col1, col2
  FROM your_table
) AS distinct_results
ORDER BY col2 COLLATE "C";
```
* *原因:* *
—内部查询执行`DISTINCT`投影。
外部查询安全地对结果集排序，并添加`COLLATE "C"`以与Oracle排序保持一致。

* *小贴士:* *
-确保在外部`ORDER BY`中使用的任何列都包含在内部投影中。
—对于多列排序，整理每个相关表达式：`ORDER BY col2 COLLATE "C", col3 COLLATE "C" DESC`。

验证检查表
[]添加`COLLATE "C"`到每一个应该遵循Oracle排序规则的`ORDER BY`-[]对于`DISTINCT`查询，包装投影并在外部查询中排序。
-[]内部投影中存在已确认的有序列。
-[]重新运行测试或代表性查询以验证排序是否与Oracle输出匹配。