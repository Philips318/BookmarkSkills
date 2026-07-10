---
name: dotnet-timezone
description: '.NET timezone handling guidance for C# applications. Use when working with TimeZoneInfo, DateTimeOffset, NodaTime, UTC conversion, daylight saving time, scheduling across timezones, cross-platform Windows/IANA timezone IDs, or when a .NET user needs the timezone for a city, address, region, or country and copy-paste-ready C# code.'
---
#。净时区

为解决时区问题。NET和c#代码，具有生产安全的指导和可复制粘贴的代码片段。

##从正确的道路开始

首先确定请求类型：

-地址或位置查找
—时区ID查询
—UTC/local转换
—跨平台时区兼容性
—调度或夏令时处理
- API或持久性设计

如果库不清楚，则跨平台工作默认为`TimeZoneConverter`。如果场景中有重复的时间表或严格的夏令时规则，请选择`NodaTime`。

解析地址和位置

如果用户提供了地址、城市、地区、国家或包含地名的文档：1. 从输入中提取每个位置。
2. 阅读`references/timezone-index.md`了解常见的Windows和IANA映射。
3. 如果没有列出确切的位置，则从地理位置推断正确的IANA区域，然后将其映射到Windows ID。
4. 返回id和一个可用的c#示例。

对于每个已解析的位置，提供：```text
Location: <resolved place>
Windows ID: <windows id>
IANA ID: <iana id>
UTC offset: <standard offset and DST offset when relevant>
DST: <yes/no>
```
然后包含如下的跨平台代码片段：```csharp
using TimeZoneConverter;

TimeZoneInfo tz = TZConvert.GetTimeZoneInfo("Asia/Colombo");
DateTime local = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tz);
```
如果存在多个位置，则每个位置包含一个块，然后包含一个组合的多时区片段。

如果位置不明确，则列出可能匹配的时区，并要求用户选择正确的。

查找时区id

Windows到IANA的映射使用`references/timezone-index.md`。

始终提供两种格式：

-`TimeZoneInfo.FindSystemTimeZoneById()`在Windows上的Windows ID
—Linux、容器、`NodaTime`、`TimeZoneConverter`的IANA ID

##生成代码

使用`references/code-patterns.md`，选择最小的图案适合：

—模式1:`TimeZoneInfo`（仅用于windows代码）
-模式二：跨平台转换`TimeZoneConverter`—模式3:`NodaTime`，用于严格的时区算法和夏令时敏感调度
-模式4:`DateTimeOffset`用于api和数据传输
模式5:ASP。. NET Core持久性和表示
-模式6：循环作业和调度器
—模式7：不明确且无效的DST时间戳在推荐第三方库时始终包含包指导。

##警告常见陷阱

在适用的情况下提到相关的警告：

—`TimeZoneInfo.FindSystemTimeZoneById()`是特定于平台的时区id。
-避免在数据库中存储`DateTime.Now`；而是存储UTC。
-将`DateTimeKind.Unspecified`视为bug风险，除非它是故意输入的。
—夏令时转换可以跳过或重复本地时间。
- Azure Windows和Azure Linux环境可能期望不同的时区ID格式。

##响应形状

地址及地点查询：

1. 返回每个位置解析的时区块。
2. 用一句话陈述推荐的实施方案。
3. 包括一个可以复制粘贴的c#代码片段。

对于代码和架构请求：1. 用一句话陈述推荐的方法。
2. 如果相关，请提供时区id。
3. 包括最小的工作代码片段。
4. 如果需要的话，提到包的需求。
5. 如果重要的话，添加一个陷阱警告。

保持回复简洁和代码优先。

# #引用

—`references/timezone-index.md`：普通Windows和IANA时区映射
-`references/code-patterns.md`：即用型。. NET时区模式