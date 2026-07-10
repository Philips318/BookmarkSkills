---
name: sql-server-table-reconciliation
description: "Use when: comparing SQL Server tables across instances, data migration validation, ETL verification, row mismatch detection, schema drift, reconciliation report, production vs staging comparison. Uses mssql-python driver with Apache Arrow for fast columnar data transfer and comparison."
---
SQL Server表协调

使用Python与`mssql-python`驱动程序和Apache Arrow在两个SQL Server实例中比较相同的表。检测缺失行、列不匹配、模式漂移，并生成核对报告。

# #工作流程

1. 收集源和目标的连接详细信息
2. 识别主键/组合键
3. 检测模式差异
4. 提取数据通过箭头有效柱状转移
5. 比较行和列
6. 生成对账报告

##收集输入

| |必选参数|描述||-----------|----------|-------------|
|源服务器|是|源SQL服务器（如`prod-server.database.windows.net`） |
|源数据库|是|源数据库名称|
|目标服务器|是|目标SQL server（如`staging-server.database.windows.net`） |
|目标数据库|是|目标数据库名称|
|表|是|逗号分隔的`schema.table`名称，或`schema.*`通配符（例如`dbo.Orders,dbo.Items`或`dbo.*`） |
|授权模式|是|`sql`（user/password）或`entra`(AzureAD/token) |
|主键|自动检测|形成行标识的列。如果未提供，则从元数据自动检测。|
|列比较|所有|列的子集，或|所有非pk列
|块大小|`100000`|大表的每批行|
|输出格式为|`console`|`console`、`csv`、`parquet`、`json`|

##绑定脚本

协调逻辑作为独立脚本在`scripts/reconcile.py`提供。使用基于用户输入的适当参数调用它：```bash
python scripts/reconcile.py \
    --source-server <source_server> \
    --source-database <source_database> \
    --target-server <target_server> \
    --target-database <target_database> \
    --tables "<table_spec>" \
    --auth <sql|entra> \
    --chunk-size <chunk_size> \
    --output <console|csv|json>
```
可选参数

|参数|描述||----------|-------------|
|`--primary-key`|逗号分隔的PK列。省略以自动检测。|
|`--columns`|逗号分隔的列进行比较。省略比较所有非pk列。|

示例调用

使用SQL认证的单表：```bash
python scripts/reconcile.py \
    --source-server prod-server.database.windows.net \
    --source-database ProdDB \
    --target-server staging-server.database.windows.net \
    --target-database StagingDB \
    --tables "dbo.Orders" \
    --auth sql \
    --output console
```
带有Entra授权和CSV输出的通配符：```bash
python scripts/reconcile.py \
    --source-server prod-server.database.windows.net \
    --source-database ProdDB \
    --target-server staging-server.database.windows.net \
    --target-database StagingDB \
    --tables "dbo.*" \
    --auth entra \
    --output csv
```
# # #先决条件

运行前安装所需的软件包：```bash
pip install mssql-python pyarrow pandas
```
比较规则

- **在比较**之前对类型进行规范化：将小数转换为相同的精度，修剪字符串，将日期时间规范化为UTC
- **NULL处理**:`NULL == NULL`被认为是匹配的（两边缺失=没有差异）
- **忽略行顺序**：总是通过PK连接进行比较，而不是位置
- **大型表**：块提取与`OFFSET/FETCH`或`ROW_NUMBER()`分区

基于哈希的优化（适用于大型表）

当表有100万行时，生成哈希预检查：```sql
SELECT {pk_cols},
       HASHBYTES('SHA2_256', CONCAT_WS('|', col1, col2, ...)) AS row_hash
FROM {table}
```
首先比较哈希值；只获取不匹配的哈希的完整行。这大大减少了数据传输。

##报表格式```
Reconciling dbo.EMPLOYEES...
Reconciling dbo.DEPARTMENTS...
Reconciling dbo.JOBS...

--- dbo.EMPLOYEES ---
  Source: 107  Target: 107
  Missing: 0  Extra: 0  Mismatches: 0
  Result: ✓ IDENTICAL

--- dbo.DEPARTMENTS ---
  Source: 27  Target: 27
  Missing: 0  Extra: 0  Mismatches: 3
  Result: ✗ DIFFERENCES FOUND

--- dbo.JOBS ---
  Source: 19  Target: 19
  Missing: 0  Extra: 0  Mismatches: 0
  Result: ✓ IDENTICAL

=== Summary: 2 passed, 1 failed, 0 skipped / 3 tables ===
```
当提供单个表时，包括完整的细节（模式漂移、示例行、不匹配）。当有多个表时，使用上面的紧凑的逐表格式，仅对具有`FAIL`状态的表使用完整的详细信息。

性能考虑

|场景|策略||----------|----------|
| < 100K行|单箭头取，内存中的熊猫比较|
|分块提取（100K批），流式比较|
| > 1M行|哈希预检查→只取不匹配的行|
|宽表（100+ cols） |首先比较PK + hash，钻取不匹配的特定列|
|使用箭头柱状格式（比逐行小10-50倍）|

# #约束-始终使用`mssql-python`驱动程序（而不是pyodbc， pymssql）
-始终使用Apache Arrow通过光标（`cursor.arrow()`）进行数据提取
连接必须使用连接字符串格式，而不是关键字参数（如`encrypt=True`抛出错误）
-如果自动检测失败，永远不要在没有确定PK的情况下进行比较
—通过重试逻辑优雅地处理连接失败
- **永远不要硬编码凭证**在生成的脚本-使用`os.environ`/`getpass`（环境变量：`MSSQL_USER`，`MSSQL_PASSWORD`）
—不要在输出或日志中打印凭据
-使用参数化查询（`?`占位符）进行元数据查找-永远不要将用户输入插入到SQL中