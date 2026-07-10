# Oracle to PostgreSQL并发事务处理

# #内容

——概述
-核心区别
-常见错误现象
—问题场景
-解决方案-实体化结果，分离连接，单一查询
-侦测策略
-要注意的错误信息
-对照表
-最佳实践
-迁移检查清单

# #概述

当从Oracle迁移到PostgreSQL时，一个关键的区别在于如何处理单个数据库连接上的并发操作。甲骨文的ODP。. NET驱动允许在同一连接上同时使用多个活动命令和结果集，而PostgreSQL的Npgsql驱动强制执行严格的“每个连接一个活动命令”规则。如果并发操作共享一个连接，在Oracle中无缝工作的代码在PostgreSQL中会抛出运行时异常。

核心区别

Oracle行为:* * * *—单个连接可以同时执行多个活动命令
—允许在打开另一个`DataReader`时打开第二个`DataReader`-同一连接上的嵌套或重叠数据库调用透明地工作

* * PostgreSQL行为:* *

—一个连接一次只支持**一条活动命令**
当`DataReader`打开时，尝试执行第二个命令会抛出异常
-延迟加载的导航属性或在同一连接上触发额外查询的回调驱动读取将失败

常见错误症状

当迁移Oracle代码而不考虑这种差异时：```
System.InvalidOperationException: An operation is already in progress.
```

```
Npgsql.NpgsqlOperationInProgressException: A command is already in progress: <SQL text>
```
当应用程序代码试图在已经有活动`DataReader`或未提交命令的连接上执行新命令时，就会发生这种情况。

---

##问题场景

场景1：在执行另一个命令的同时迭代一个DataReader```csharp
using (var reader = command1.ExecuteReader())
{
    while (reader.Read())
    {
        // PROBLEM: executing a second command on the same connection
        // while the reader is still open
        using (var command2 = new NpgsqlCommand("SELECT ...", connection))
        {
            var value = command2.ExecuteScalar(); // FAILS
        }
    }
}
```
场景2：数据访问层的延迟加载/延迟执行```csharp
// Oracle: works because ODP.NET supports concurrent readers
var items = repository.GetItems(); // returns IEnumerable backed by open DataReader
foreach (var item in items)
{
    // PROBLEM: triggers a second query on the same connection
    var details = repository.GetDetails(item.Id); // FAILS on PostgreSQL
}
```
场景3：通过应用程序代码嵌套存储过程调用```csharp
// Oracle: ODP.NET handles multiple active commands
command1.ExecuteNonQuery(); // starts a long-running operation
command2.ExecuteScalar();   // FAILS on PostgreSQL — command1 still in progress
```
---

# #的解决方案

解决方案1：在发布新命令之前实现结果（推荐）

在同一连接上执行后续命令之前，通过将第一个结果集加载到内存中来关闭它。```csharp
// Load all results into a list first
var items = new List<Item>();
using (var reader = command1.ExecuteReader())
{
    while (reader.Read())
    {
        items.Add(MapItem(reader));
    }
} // reader is closed and disposed here

// Now safe to execute another command on the same connection
foreach (var item in items)
{
    using (var command2 = new NpgsqlCommand("SELECT ...", connection))
    {
        command2.Parameters.AddWithValue("id", item.Id);
        var value = command2.ExecuteScalar(); // Works
    }
}
```
对于LINQ / EF Core场景，使用`.ToList()`强制物化：```csharp
// Before (fails on PostgreSQL — deferred execution keeps connection busy)
var items = dbContext.Items.Where(i => i.Active);
foreach (var item in items)
{
    var details = dbContext.Details.FirstOrDefault(d => d.ItemId == item.Id);
}

// After (materializes first query before issuing second)
var items = dbContext.Items.Where(i => i.Active).ToList();
foreach (var item in items)
{
    var details = dbContext.Details.FirstOrDefault(d => d.ItemId == item.Id);
}
```
解决方案2：为并发操作使用单独的连接

当操作确实需要并发运行时，为每个操作打开一个专用连接。```csharp
using (var reader = command1.ExecuteReader())
{
    while (reader.Read())
    {
        // Use a separate connection for the nested query
        using (var connection2 = new NpgsqlConnection(connectionString))
        {
            connection2.Open();
            using (var command2 = new NpgsqlCommand("SELECT ...", connection2))
            {
                var value = command2.ExecuteScalar(); // Works — different connection
            }
        }
    }
}
```
解决方案3：重构为单个查询

在可能的情况下，使用join或子查询将嵌套查找组合到单个查询中，以完全消除对并发命令的需求。```csharp
// Before: two sequential queries on the same connection
var order = GetOrder(orderId);          // query 1
var details = GetOrderDetails(orderId); // query 2 (fails if query 1 reader still open)

// After: single query with JOIN
using (var command = new NpgsqlCommand(
    "SELECT o.*, d.* FROM orders o JOIN order_details d ON o.id = d.order_id WHERE o.id = @id",
    connection))
{
    command.Parameters.AddWithValue("id", orderId);
    using (var reader = command.ExecuteReader())
    {
        // Process combined result set
    }
}
```
---

##检测策略

代码审查检查表

-[]搜索打开`DataReader`的方法，并在关闭它之前调用其他数据库方法
-[]查找延迟执行的数据访问方法的`IEnumerable`返回类型（表示打开的读取器）
-[]识别没有`.ToList()`/`.ToArray()`的EF Core查询，在发出进一步查询时迭代
-[]检查共享连接的应用程序代码中嵌套的存储过程调用

###常见的搜索位置

—数据访问层和存储库类
-协调多个存储库调用的服务方法
-迭代查询结果并按行执行查找的代码路径
-在数据迭代期间触发的事件处理程序或回调

搜索模式```regex
ExecuteReader\(.*\)[\s\S]*?Execute(Scalar|NonQuery|Reader)\(
```

```regex
\.Where\(.*\)[\s\S]*?foreach[\s\S]*?dbContext\.
```
---

##要注意的错误信息

|错误信息|可能原因||---------------|--------------|
|`An operation is already in progress`|在同一连接|上打开`DataReader`时执行的第二个命令
|`A command is already in progress: <SQL>`| Npgsql检测到在单个连接上执行重叠命令|
|`The connection is already in state 'Executing'`|并发使用导致的连接状态冲突|

---

对比表：Oracle和PostgreSQL

| Aspect | Oracle （ODP）PostgreSQL (Npgsql) | . NET|--------|------------------|---------------------|
| **并发命令** |每个连接多个激活命令|每个连接一个激活命令|
| **多个开放的datareader ** |支持|不支持-必须先close/materialize|
| **在迭代期间嵌套DB调用** |透明|抛出`InvalidOperationException`|
| **延迟执行安全** |安全迭代和查询|必须在发出新的查询|之前实现（`.ToList()`）
| **连接池影响** |连接需求较低|如果使用方案2 |，可能需要更多的连接池

---

最佳实践

1. **实现早期** -在迭代和发出进一步的数据库调用之前，对查询结果调用`.ToList()`或`.ToArray()`。这是最简单、最可靠的修复方法。

2. **审计数据访问模式** -检查所有存储库和数据访问方法的延迟执行返回类型（`IEnumerable`,`IQueryable`），调用者在发出额外查询时迭代。3. **首选单个查询** -在可行的情况下，将嵌套查找组合到join或子查询中，以完全消除并发命令模式。

4. **必要时隔离连接** -如果确实需要并发操作，请使用单独的连接而不是尝试共享一个连接。

5. **测试迭代工作流** -集成测试应该涵盖代码迭代结果集和执行每行附加数据库操作的场景，因为这些是最常见的故障点。

##迁移清单-[]识别在单个连接上同时执行多个命令的所有代码路径
-[]查找受`IEnumerable`支持的数据访问方法，这些方法在打开读器时延迟执行
-[]添加`.ToList()`/`.ToArray()`物化，延迟的结果与进一步的查询一起迭代
-[]重构嵌套的数据库调用，在适当的地方使用单独的连接或组合查询
-[]验证EF Core导航属性和延迟加载不会触发并发连接使用
-[]更新集成测试以涵盖迭代数据访问模式
-[]如果广泛使用方案2（单独连接），则负载测试连接池大小

# #引用

- [Npgsql文档：基本用法]（https://www.npgsql.org/doc/basic-usage.html）
- PostgreSQL文档：并发控制[https://www.postgresql.org/docs/current/mvcc.html]
- [Npgsql GitHub：多活动结果集讨论]（https://github.com/npgsql/npgsql/issues/462）