# Oracle to PostgreSQL：在客户端应用程序中Refcursor Handling

核心区别

Oracle的驱动程序自动展开`SYS_REFCURSOR`输出参数，直接在数据阅读器中公开结果集。PostgreSQL的Npgsql驱动程序返回一个**游标名称**（例如，`"<unnamed portal 1>"`）。客户机必须发出单独的`FETCH ALL FROM "<cursor_name>"`命令来检索实际的行。

不明原因的：未能解释这一原因的：```
System.IndexOutOfRangeException: Field not found in row: <column_name>
```
阅读器只包含游标名称参数，而不包含预期的结果列。

> **事务要求：** PostgreSQL refcursors的作用域是一个事务。过程调用和`FETCH`都必须在同一个显式事务中执行，否则在自动提交完成取操作之前，游标可能会被关闭。

解决方案：显式Refcursor unwrapped （c#）```csharp
public IEnumerable<User> GetUsers(int departmentId)
{
    var users = new List<User>();
    using var connection = new NpgsqlConnection(connectionString);
    connection.Open();

    // Refcursors are transaction-scoped — wrap both the call and FETCH in one transaction.
    using var tx = connection.BeginTransaction();

    using var command = new NpgsqlCommand("get_users", connection, tx)
    {
        CommandType = CommandType.StoredProcedure
    };
    command.Parameters.AddWithValue("p_department_id", departmentId);
    var refcursorParam = new NpgsqlParameter("cur_result", NpgsqlDbType.Refcursor)
    {
        Direction = ParameterDirection.Output
    };
    command.Parameters.Add(refcursorParam);

    // Execute the procedure to open the cursor.
    command.ExecuteNonQuery();

    // Retrieve the cursor name, then fetch the actual data.
    string cursorName = (string)refcursorParam.Value;
    using var fetchCommand = new NpgsqlCommand($"FETCH ALL FROM \"{cursorName}\"", connection, tx);
    using var reader = fetchCommand.ExecuteReader();
    while (reader.Read())
    {
        users.Add(new User
        {
            UserId   = reader.GetInt32(reader.GetOrdinal("user_id")),
            UserName = reader.GetString(reader.GetOrdinal("user_name")),
            Email    = reader.GetString(reader.GetOrdinal("email"))
        });
    }

    tx.Commit();
    return users;
}
```
可重用的助手

从helper返回一个活动的`NpgsqlDataReader`会使底层的`NpgsqlCommand`未被处理，并创建不明确的所有权。更喜欢在helper中实现结果：```csharp
public static class PostgresHelpers
{
    public static List<T> ExecuteRefcursorProcedure<T>(
        NpgsqlConnection connection,
        NpgsqlTransaction transaction,
        string procedureName,
        Dictionary<string, object> parameters,
        string refcursorParameterName,
        Func<NpgsqlDataReader, T> map)
    {
        using var command = new NpgsqlCommand(procedureName, connection, transaction)
        {
            CommandType = CommandType.StoredProcedure
        };
        foreach (var (key, value) in parameters)
            command.Parameters.AddWithValue(key, value);

        var refcursorParam = new NpgsqlParameter(refcursorParameterName, NpgsqlDbType.Refcursor)
        {
            Direction = ParameterDirection.Output
        };
        command.Parameters.Add(refcursorParam);
        command.ExecuteNonQuery();

        string cursorName = (string)refcursorParam.Value;
        if (string.IsNullOrEmpty(cursorName))
            return new List<T>();

        // fetchCommand is disposed here; results are fully materialized before returning.
        using var fetchCommand = new NpgsqlCommand($"FETCH ALL FROM \"{cursorName}\"", connection, transaction);
        using var reader = fetchCommand.ExecuteReader();

        var results = new List<T>();
        while (reader.Read())
            results.Add(map(reader));
        return results;
    }
}

// Usage:
using var connection = new NpgsqlConnection(connectionString);
connection.Open();
using var tx = connection.BeginTransaction();

var users = PostgresHelpers.ExecuteRefcursorProcedure(
    connection, tx,
    "get_users",
    new Dictionary<string, object> { { "p_department_id", departmentId } },
    "cur_result",
    r => new User
    {
        UserId   = r.GetInt32(r.GetOrdinal("user_id")),
        UserName = r.GetString(r.GetOrdinal("user_name")),
        Email    = r.GetString(r.GetOrdinal("email"))
    });

tx.Commit();
```
Oracle vs. PostgreSQL总结

| Aspect | Oracle （ODP）PostgreSQL (Npgsql) | . NET|--------|------------------|---------------------|
| **游标返回** |结果集直接暴露在数据阅读器|游标名称字符串在输出参数|
| **数据访问** |`ExecuteReader()`立即返回行|`ExecuteNonQuery()`→获取游标名称→`FETCH ALL FROM`|
| **事务** |透明| CALL和FETCH必须共享同一个事务|
| **多个游标** |自动|每个需要单独的`FETCH`命令|
| **资源生存期** |驱动程序管理的|游标打开，直到获取或事务结束|

##迁移清单

[]识别所有返回`SYS_REFCURSOR`(Oracle) /`refcursor`（PostgreSQL）的过程
-[]将`ExecuteReader()`替换为`ExecuteNonQuery()`→光标名称→`FETCH ALL FROM`-[]在显式事务中包装每个调用和提取对
[]确保命令和读取器被丢弃（更倾向于在helper中物化结果）
-[]更新单元测试和集成测试

# #引用- [PostgreSQL文档：cursor]（https://www.postgresql.org/docs/current/plpgsql-cursors.html）
- [PostgreSQL FETCH命令]（https://www.postgresql.org/docs/current/sql-fetch.html）
- [Npgsql Refcursor支持]（https://github.com/npgsql/npgsql/issues/1887）