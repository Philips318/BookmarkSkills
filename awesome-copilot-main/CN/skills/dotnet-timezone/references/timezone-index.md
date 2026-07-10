#。. NET时区参考索引

Windows到IANA映射

此文件用于Windows时区id和IANA时区id之间的常见映射。

亚太地区

|显示名称| Windows ID | IANA ID | UTC偏移量|夏令时？|| --- | --- | --- | --- | --- |
|斯里兰卡标准时间|斯里兰卡标准时间|Asia/Colombo| +05:30 |不|
|印度标准时间|印度标准时间|Asia/Calcutta| +05:30 |不|
|巴基斯坦标准时间|巴基斯坦标准时间|Asia/Karachi| +05:00 |不|
|孟加拉国标准时间|孟加拉国标准时间|Asia/Dhaka| +06:00 |不|
|尼泊尔标准时间|尼泊尔标准时间|Asia/Katmandu| +05:45 |不|
|东南亚标准时间|东南亚标准时间|Asia/Bangkok| +07:00 |不|
|新加坡标准时间|新加坡标准时间|Asia/Singapore| +08:00 |不|
|中国标准时间|中国标准时间|Asia/Shanghai| +08:00 |不|
|东京标准时间|东京标准时间|Asia/Tokyo| +09:00 |不|
|韩国标准时间|韩国标准时间|Asia/Seoul| +09:00 |不|
|澳大利亚东部标准时间|澳大利亚东部标准时间|Australia/Sydney| +10:00/ 11:00 |是|
|新西兰标准时间|新西兰标准时间|Pacific/Auckland| +12：00/+13:00 |是|
|阿拉伯标准时间|阿拉伯标准时间|Asia/Dubai| +04:00 |不|
|阿拉伯标准时间|阿拉伯标准时间|Asia/Riyadh| +03:00 |不|
|以色列标准时间|以色列标准时间|Asia/Jerusalem| +02:00/+03:00 |是|
|土耳其标准时间|土耳其标准时间|Europe/Istanbul| +03:00 |不|# # #欧洲

|显示名称| Windows ID | IANA ID | UTC偏移量|夏令时？|| --- | --- | --- | --- | --- |
| UTC | UTC |Etc/UTC| +00:00 |否|
| GMT标准时间| GMT标准时间|Europe/London| +00:00/+01:00 |是|
| W.欧洲标准时间| W.欧洲标准时间|Europe/Berlin| +01:00/+02:00 |是|
|中欧标准时间|中欧标准时间|Europe/Budapest| +01:00/+02:00 |是|
|浪漫标准时间|浪漫标准时间|Europe/Paris| +01:00/+02:00 |是|
E.欧洲标准时间| E.欧洲标准时间|Asia/Nicosia| +02:00/+03:00 |是|
| GTB标准时间| GTB标准时间|Europe/Bucharest| +02:00/+03:00 |是|
|俄罗斯标准时间|俄罗斯标准时间|Europe/Moscow| +03:00 |不|

# # #美洲

|显示名称| Windows ID | IANA ID | UTC偏移量|夏令时？|| --- | --- | --- | --- | --- |
|东部标准时间|东部标准时间|America/New_York| -05:00/-04:00 |是|
|中央标准时间|中央标准时间|America/Chicago| -06:00/-05:00 |是|
|山区标准时间|山区标准时间|America/Denver| -07:00/-06:00 |是|
|太平洋标准时间|太平洋标准时间|America/Los_Angeles| -08:00/-07:00 |是|
|阿拉斯加标准时间|阿拉斯加标准时间|America/Anchorage| -09:00/-08:00 |是|
|夏威夷标准时间|夏威夷标准时间|Pacific/Honolulu| -10:00 |不|
|加拿大中部标准时间|加拿大中部标准时间|America/Regina| -06:00 |不|
| SA东部标准时间| SA东部标准时间|America/Cayenne| -03:00 |否|
E.南美标准时间| E.南美标准时间|America/Sao_Paulo| -03:00/-02:00 |是|

# # #非洲

|显示名称| Windows ID | IANA ID | UTC偏移量|夏令时？|| --- | --- | --- | --- | --- |
|南非标准时间|南非标准时间|Africa/Johannesburg| +02:00 |不|
|埃及标准时间|埃及标准时间|Africa/Cairo| +02:00 |否|
|非洲东部标准时间|非洲东部标准时间|Africa/Nairobi| +03:00 |不|
|西非中部标准时间|西非中部标准时间|Africa/Lagos| +01:00 |不|
|摩洛哥标准时间|摩洛哥标准时间|Africa/Casablanca| +00:00/+01:00 |是|

## nodeatime provider```csharp
DateTimeZoneProviders.Tzdb["Asia/Colombo"]
DateTimeZoneProviders.Bcl["Sri Lanka Standard Time"]
```
TimeZoneConverter示例```csharp
string ianaId = TZConvert.WindowsToIana("Sri Lanka Standard Time");
string windowsId = TZConvert.IanaToWindows("Asia/Colombo");
TimeZoneInfo tz = TZConvert.GetTimeZoneInfo("Asia/Colombo");
```
##程序化发现```csharp
foreach (var tz in TimeZoneInfo.GetSystemTimeZones())
{
    Console.WriteLine($"ID: {tz.Id} | Display: {tz.DisplayName}");
}
```
