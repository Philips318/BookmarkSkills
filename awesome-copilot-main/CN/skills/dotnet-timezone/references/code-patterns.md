#。. NET时区代码模式

模式1：基本TimeZoneInfo

只有当应用程序仅支持Windows并且可以接受Windows时区id时，才使用此方法。```csharp
DateTime utcNow = DateTime.UtcNow;
TimeZoneInfo sriLankaTz = TimeZoneInfo.FindSystemTimeZoneById("Sri Lanka Standard Time");
DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, sriLankaTz);

DateTime backToUtc = TimeZoneInfo.ConvertTimeToUtc(localTime, sriLankaTz);

TimeZoneInfo tokyoTz = TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time");
DateTime tokyoTime = TimeZoneInfo.ConvertTime(localTime, sriLankaTz, tokyoTz);
```
对于Linux、容器或混合环境，请使用`TimeZoneConverter`或`NodaTime`。

模式2：跨平台使用TimeZoneConverter

大多数情况下推荐默认设置。NET应用程序可以在Windows和Linux上运行。```xml
<PackageReference Include="TimeZoneConverter" Version="6.*" />
```

```csharp
using TimeZoneConverter;

TimeZoneInfo tz = TZConvert.GetTimeZoneInfo("Asia/Colombo");
DateTime converted = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tz);
```
这也接受Windows id：```csharp
TimeZoneInfo tz = TZConvert.GetTimeZoneInfo("Sri Lanka Standard Time");
```
模式3:nodeatime

对于严格的时区算术、重复调度或DST边缘情况，在这些情况下，正确性比最小依赖关系更重要。```xml
<PackageReference Include="NodaTime" Version="3.*" />
```

```csharp
using NodaTime;

DateTimeZone colomboZone = DateTimeZoneProviders.Tzdb["Asia/Colombo"];
Instant now = SystemClock.Instance.GetCurrentInstant();
ZonedDateTime colomboTime = now.InZone(colomboZone);

DateTimeZone tokyoZone = DateTimeZoneProviders.Tzdb["Asia/Tokyo"];
ZonedDateTime tokyoTime = colomboTime.WithZone(tokyoZone);

LocalDateTime localDt = new LocalDateTime(2024, 6, 15, 14, 30, 0);
ZonedDateTime zoned = colomboZone.AtStrictly(localDt);
Instant utcInstant = zoned.ToInstant();
```
模式4:api的DateTimeOffset

对于跨越服务或进程边界的值，建议使用`DateTimeOffset`。```csharp
using TimeZoneConverter;

DateTimeOffset utcNow = DateTimeOffset.UtcNow;
TimeZoneInfo tz = TZConvert.GetTimeZoneInfo("Asia/Colombo");
DateTimeOffset colomboTime = TimeZoneInfo.ConvertTime(utcNow, tz);
```
模式5:ASP。. NET核心持久性和表示

存储UTC，在边缘转换。```csharp
using TimeZoneConverter;

entity.CreatedAtUtc = DateTime.UtcNow;

public DateTimeOffset ToUserTime(DateTime utc, string userIanaTimezone)
{
    var tz = TZConvert.GetTimeZoneInfo(userIanaTimezone);
    return TimeZoneInfo.ConvertTimeFromUtc(utc, tz);
}
```
模式6：调度和循环作业

在调度之前将面向用户的本地时间转换为UTC。```csharp
using TimeZoneConverter;

TimeZoneInfo tz = TZConvert.GetTimeZoneInfo("Asia/Colombo");
DateTime scheduledLocal = new DateTime(2024, 12, 1, 9, 0, 0, DateTimeKind.Unspecified);
DateTime scheduledUtc = TimeZoneInfo.ConvertTimeToUtc(scheduledLocal, tz);
```
延迟发射:```csharp
RecurringJob.AddOrUpdate(
    "morning-job",
    () => DoWork(),
    "0 9 * * *",
    new RecurringJobOptions { TimeZone = tz });
```
模式7：模糊和无效的DST时间

检查时区遵守夏令时时重复或跳过的本地时间戳。```csharp
using TimeZoneConverter;

TimeZoneInfo tz = TZConvert.GetTimeZoneInfo("America/New_York");
DateTime localTime = new DateTime(2024, 11, 3, 1, 30, 0);

if (tz.IsAmbiguousTime(localTime))
{
    var offsets = tz.GetAmbiguousTimeOffsets(localTime);
    var standardOffset = offsets.Min();
    var dto = new DateTimeOffset(localTime, standardOffset);
}

if (tz.IsInvalidTime(localTime))
{
    localTime = localTime.AddHours(1);
}
```
常见错误

|错|更好|| --- | --- |
|`DateTime.UtcNow`|中的|`DateTime.Now`|在数据库中存储本地时间戳|存储UTC并转换为显示|
|硬编码偏移量，如`+05:30`|使用时区id |
|使用`TZConvert.GetTimeZoneInfo("Asia/Colombo")`|
|比较不同区域的本地`DateTime`值|比较UTC或使用`DateTimeOffset`|
|使用`Utc`、`Local`或故意使用`Unspecified`|

##决策指南

—`TimeZoneInfo`仅用于标识为Windows操作系统的代码。
-大多数跨平台应用程序使用`TimeZoneConverter`。
-当DST算术或日历精度是中心时使用`NodaTime`。
—api和序列化时间戳使用`DateTimeOffset`。