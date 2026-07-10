# Oracle to PostgreSQL: CURRENT_TIMESTAMP and NOW() Timezone Handling

# #内容

——问题
-行为比较
—PostgreSQL时区优先级
-常见错误现象
Npgsql配置，DateTime规范化，存储过程，会话时区，应用程序代码
-集成测试模式
——检查表

# #的问题

Oracle的`CURRENT_TIMESTAMP`返回**会话时区**中的值，并将其存储在列声明的精度中。什么时候。NET通过ODP读取该值。NET时，它显示为`DateTime`和`Kind=Local`，反映客户机的操作系统时区。

PostgreSQL的`CURRENT_TIMESTAMP`和`NOW()`都返回一个锚定到**UTC**的`timestamptz`（带时区的时间戳），与会话时区设置无关。Npgsql如何显示此值取决于驱动程序版本和配置：- **Npgsql < 6 / legacy mode (`EnableLegacyTimestampBehavior = true`):**`timestamptz`列返回为`DateTime`与`Kind=Unspecified`。这就是从Oracle迁移时静默时区错误的来源。
**Npgsql 6+禁用遗留模式（新的默认值）：**`timestamptz`列返回为`DateTime`和`Kind=Utc`，写入`Kind=Unspecified`值会在插入时抛出异常。

尚未升级到Npgsql 6+的项目，或者明确选择返回到遗留模式的项目，仍然容易受到`Kind=Unspecified`问题的影响。这种不匹配—以及意外地重新启用遗留模式的便利性—会导致静默数据损坏、不正确的比较和异常n小时的错误，这些错误非常难以跟踪。

---

##行为比较

| Aspect | Oracle | PostgreSQL ||---|---|---|
|`CURRENT_TIMESTAMP`类型|`TIMESTAMP WITH LOCAL TIME ZONE`|`timestamptz`（utc标准化）|
|客户端`DateTime.Kind`通过驱动|`Local`|`Unspecified`（Npgsql < 6 / legacy模式）；`Utc`（Npgsql 6+默认）|
|会话时区影响|是-影响stored/returned值|只影响*显示*；内部存储的UTC |
| NOW（）等效|`SYSDATE`/`CURRENT_TIMESTAMP`|`NOW()`=`CURRENT_TIMESTAMP`（都返回`timestamptz`） |
| Oracle应用会话TZ偏移| PostgreSQL比较UTC；session TZ为display-only |

---

PostgreSQL时区优先级

PostgreSQL使用以下层次结构解析有效会话时区（优先级最高）：

|级别|设置方式||---|---|
| **会话** |`SET TimeZone = 'UTC'`在连接打开|时发送
| **角色** |`ALTER ROLE app_user SET TimeZone = 'UTC'`|
| **数据库** |`ALTER DATABASE mydb SET TimeZone = 'UTC'`|
| **服务器** |`postgresql.conf`→`TimeZone = 'America/New_York'`|

会话时区不影响`timestamptz`列存储的UTC值——它只控制`SHOW timezone`和`::text`如何强制转换格式以显示值。如果服务器的默认时区不是UTC，依赖于`DateTime.Kind`或比较没有显式时区的时间戳的应用程序代码可能会产生不正确的结果。

---

常见错误症状从PostgreSQL读取的时间戳有`Kind=Unspecified`；与`DateTime.UtcNow`或`DateTime.Now`比较会产生不正确的结果。
—日期范围查询返回的行数太少或太多，因为WHERE子句比较是在与存储的UTC值不同的时区中进行计算的。
—集成测试在开发人员机器（UTC操作系统时区）上通过，但在CI或生产（非UTC时区）中失败。
—携带时间戳的存储过程输出参数与服务器应用的会话偏移量一起到达，但随后与应用程序中的UTC值进行比较。

---

##迁移操作

# # # 1。通过连接字符串或AppContext为UTC配置NpgsqlNpgsql 6+附带的`EnableLegacyTimestampBehavior`默认设置为`false`，这导致`timestamptz`值返回为`DateTime`和`Kind=Utc`。仍然建议在启动时显式设置开关，以防止意外选择进入遗留模式（例如，通过配置文件或传递依赖项），并使意图对未来的维护者可见：```csharp
// Program.cs / Startup.cs — apply once at application start
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", false);
```
禁用此开关后，如果尝试将带有`Kind=Unspecified`的`DateTime`写入`timestamptz`列，Npgsql就会抛出异常，这会在插入时而不是在查询时使时区错误变得明显和可检测。

# # # 2。在持久化之前规范化DateTime值

用`DateTime.UtcNow`替换整个迁移代码库中的任何`DateTime.Now`。对于来自外部输入的值（例如，从JSON反序列化的用户提供的日期），确保在保存之前将它们转换为UTC：```csharp
// Before (Oracle-era code — relied on session/OS timezone)
var timestamp = DateTime.Now;

// After (PostgreSQL-compatible)
var timestamp = DateTime.UtcNow;

// For externally-supplied values
var utcTimestamp = dateTimeInput.Kind == DateTimeKind.Utc
    ? dateTimeInput
    : dateTimeInput.ToUniversalTime();
```
# # # 3。修复使用CURRENT_TIMESTAMP / NOW（）的存储过程

必须检查将`CURRENT_TIMESTAMP`或`NOW()`分配给`timestamp without time zone`（`timestamp`）列的存储过程。首选`timestamptz`列或显式强制转换：```sql
-- Ambiguous: server timezone influences interpretation
INSERT INTO audit_log (created_at) VALUES (NOW()::timestamp);

-- Safe: always UTC
INSERT INTO audit_log (created_at) VALUES (NOW() AT TIME ZONE 'UTC');

-- Or: use timestamptz column type and let PostgreSQL store UTC natively
INSERT INTO audit_log (created_at) VALUES (CURRENT_TIMESTAMP);
```
# # # 4。强制连接打开会话时区（纵深防御）

无论角色或数据库默认值如何，在打开连接时显式设置会话时区。这保证了独立于服务器配置的一致行为：```csharp
// Npgsql connection string approach
var connString = "Host=localhost;Database=mydb;Username=app;Password=...;Timezone=UTC";

// Or: apply via NpgsqlDataSourceBuilder
var dataSource = new NpgsqlDataSourceBuilder(connString)
    .Build();

// Or: execute on every new connection
await using var conn = new NpgsqlConnection(connString);
await conn.OpenAsync();
await using var cmd = new NpgsqlCommand("SET TimeZone = 'UTC'", conn);
await cmd.ExecuteNonQueryAsync();
```
# # # 5。应用程序代码-避免DateTime。类型=未指明的

审计所有读取时间戳列的存储库和数据访问代码。当Npgsql返回`Unspecified`时，要么全局配置数据源（上面的选项1），要么封装读：```csharp
// Safe reader helper — convert Unspecified to Utc at the boundary
DateTime ReadUtcDateTime(NpgsqlDataReader reader, int ordinal)
{
    var dt = reader.GetDateTime(ordinal);
    return dt.Kind == DateTimeKind.Unspecified
        ? DateTime.SpecifyKind(dt, DateTimeKind.Utc)
        : dt.ToUniversalTime();
}
```
---

集成测试模式

### Test：验证时间戳是否存在并返回UTC```csharp
[Fact]
public async Task InsertedTimestamp_ShouldRoundTripAsUtc()
{
    var before = DateTime.UtcNow;

    await repository.InsertAuditEntryAsync(/* ... */);

    var retrieved = await repository.GetLatestAuditEntryAsync();

    Assert.Equal(DateTimeKind.Utc, retrieved.CreatedAt.Kind);
    Assert.True(retrieved.CreatedAt >= before,
        "Persisted CreatedAt should not be earlier than the pre-insert UTC timestamp.");
}
```
验证Oracle和PostgreSQL基线之间的时间戳比较```csharp
[Fact]
public async Task TimestampComparison_ShouldReturnSameRowsAsOracle()
{
    var cutoff = DateTime.UtcNow.AddDays(-1);

    var oracleResults = await oracleRepository.GetEntriesAfter(cutoff);
    var postgresResults = await postgresRepository.GetEntriesAfter(cutoff);

    Assert.Equal(oracleResults.Count, postgresResults.Count);
}
```
---

# #检查表

- []`AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", false)`在应用启动时应用。
-[]数据访问代码中所有`DateTime.Now`的用法替换为`DateTime.UtcNow`。
-[]连接串或连接开钩集`Timezone=UTC`/`SET TimeZone = 'UTC'`。
-[]查看使用`CURRENT_TIMESTAMP`或`NOW()`的存储过程；`timestamp without time zone`列显式转换或替换为`timestamptz`。
-[]集成测试在检索的时间戳值上断言`DateTime.Kind == Utc`。
-[]测试涵盖日期范围查询，以确认行数是否符合Oracle基线。