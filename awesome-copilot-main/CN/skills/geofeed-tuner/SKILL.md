---
name: geofeed-tuner
description: >
  Use this skill whenever the user mentions IP geolocation feeds, RFC 8805, geofeeds, or wants help creating, tuning, validating, or publishing a
  self-published IP geolocation feed in CSV format. Intended user audience is a network
  operator, ISP, mobile carrier, cloud provider, hosting company, IXP, or satellite provider
  asking about IP geolocation accuracy, or geofeed authoring best practices.
  Helps create, refine, and improve CSV-format IP geolocation feeds with opinionated
  recommendations beyond RFC 8805 compliance. Do NOT use for private or internal IP address
  management — applies only to publicly routable IP addresses.
license: Apache-2.0
metadata:
  author: Sid Mathur <support@getfastah.com>
  version: "0.0.9"
compatibility: Requires Python 3
---
# Geofeed调谐器-创建更好的IP地理位置源

此技能可帮助您创建和改进IP地理位置提要在CSV格式：
-确保您的CSV格式良好且一致
-检查与[RFC 8805](references/rfc8805.txt)（行业标准）的一致性
-应用从实际部署中学到的固执己见的最佳实践
-提出准确性、完整性和隐私方面的改进建议

何时使用此技能—当用户请求帮助**创建、改进或发布** CSV格式的IP地理位置提要文件时，使用此技能。
-使用它来调整和排除CSV地理位置提要-捕获错误，提出改进建议，并确保超出RFC合规的实际可用性。
- **目标受众：**
—网络运营商、管理员和工程师负责公网可路由的IP地址空间
-互联网服务提供商、移动运营商、云提供商、托管和托管公司、互联网交换运营商和卫星互联网提供商等组织
- **不要使用**此技能用于私人或内部IP地址管理；它只适用于可公开路由的IP地址。

# #先决条件

- **需要Python 3**。

目录结构和文件管理该技能在**发行文件**（只读）和**工作文件**（在运行时生成）之间使用了明确的分离。

只读目录（不能修改）

以下目录包含静态发行版资产。**禁止创建、修改或删除以下目录下的文件：**

|目录|用途||----------------|------------------------------------------------------------|
|`assets/`|静态数据文件(ISO代码、样例
|`references/`| RFC规范和代码片段供参考|
|`scripts/`|报告的可执行代码和HTML模板文件|

###工作目录（生成内容）

所有生成的、临时的和输出的文件都放在这些目录中：

|目录|用途||-----------------|------------------------------------------------------|
|`run/`|所有代理生成内容|的工作目录
|`run/data/`|从远程url下载CSV文件|
|生成HTML调优报告|

###文件管理规则1. **不要写`assets/`，`references/`，或`scripts/`** -这些是技能分布的一部分，必须保持不变。
2. **所有下载的输入文件**（远程url）必须保存到`./run/data/`。
3. **所有生成的HTML报告**必须保存到`./run/report/`。
4. **所有生成的Python脚本**必须保存到`./run/`。
5.`run/`目录可能在会话之间被清除；不要在那里存储永久数据。
6. **执行工作目录：**在`./run/`中生成的所有脚本必须以**skill根目录** （`SKILL.md`所在的目录）作为当前工作目录执行，以便`assets/iso3166-1.json`、`./run/data/report-data.json`等相对路径能够正确解析。在运行脚本之前，不要`cd`进入`./run/`。


处理管道：顺序阶段执行从第一阶段到第六阶段，所有阶段必须按顺序执行。每个阶段都依赖于前一个阶段的成功完成。例如，**结构检查**必须在**质量分析**运行之前完成。

这些阶段总结如下。代理必须遵循每个阶段部分中进一步概述的详细步骤。

|阶段|名称|描述||-------|----------------------------|-----------------------------------------------------------------------------------|
了解标准|查看RFC 8805对自发布IP地理位置提要的关键要求|
| 2 | Collect Input |从本地文件或远程url中收集IP子网数据|
| 3 |检查与建议|验证CSV结构、IP前缀分析、数据质量检查|
调优数据查找|使用Fastah的MCP工具检索调优数据以提高地理定位精度|
| 5 |生成调优报告|创建HTML报告，汇总分析和建议|
| 6 |最终审核|验证报表数据的一致性和完整性|

**不要跳过阶段。**每个阶段提供后续阶段所需的关键检查或数据转换。


执行计划规则在执行每个阶段之前，代理必须生成一个可见的TODO清单。

该计划必须：
-出现在阶段的最开始
-按顺序列出每一步
—使用复选框格式
-在步骤完成时实时更新


阶段1：理解标准

RFC 8805中该技能强制执行的关键需求总结如下。**用这份摘要作为你的工作参考。**对于边缘情况、模棱两可的情况或用户询问此处未涉及的标准问题，仅参考完整的[RFC 8805文本]（references/rfc8805.txt）。

#### RFC 8805关键事实

**目的：**一个自发布的IP地理位置提要允许网络运营商以简单的CSV格式发布其IP地址空间的权威位置数据，允许地理位置提供商合并运营商提供的更正。

**CSV列顺序（2.1.1.1-2.1.1.5节）：**|列|字段|必选|备注||--------|---------------|----------|------------------------------------------------------------|
| 1 |`ip_prefix`|是| CIDR表示法；IPv4或IPv6；必须是网络地址b|
| 2 |`alpha2code`| No | ISO 3166-1 alpha-2国家代码；empty或"ZZ" = do-not-geolocate |
| |`region`|否| ISO 3166-2细分代码（如`US-CA`） |
| 4 |`city`|无|自由文本城市名称；没有权威验证集|
| 5 |`postal_code`|否| **已弃用** -必须为空或不存在|结构规则:* * * *
文件可以包含以`#`开头的注释行（如果有的话，包括头）。
-标题行是可选的；如果出现，则以`#`开头的注释被视为注释。
—文件必须使用UTF-8编码。
—子网主机位不能设置（即`192.168.1.1/24`无效，使用`192.168.1.0/24`）。
—仅适用于**全局可路由的**单播地址，不适用于私网空间、环回空间、链路本地空间和组播空间。

** do -not- geolocation:**带有空`alpha2code`或不区分大小写的`ZZ`（无论region/city的值如何）的条目是一个明确的信号，表示操作符不希望对该前缀应用地理定位。

**邮政编码已弃用（第2.1.1.5节）：**第五列不能包含邮政编码或邮政编码。它们对于ip范围映射来说过于细粒度，并且会引起隐私问题。


阶段2：收集输入-如果用户还没有提供IP子网或范围列表（有时称为`inetnum`或`inet6num`），提示他们提供。可接受的输入格式：
-文字粘贴到聊天
—本地CSV文件
—远程URL，指向CSV文件

—如果输入为“**远程URL**”：
—处理前尝试下载CSV文件到`./run/data/`。
-在HTTP错误（4xx, 5xx，超时，或重定向循环），**立即停止**并报告给用户：    `Feed URL is not reachable: HTTP {status_code}. Please verify the URL is publicly accessible.`
-不要在下载不完整或空白的情况下进入第3阶段。

—如果输入的是**本地文件**，直接处理，不需要下载。

- **编码检测和归一化：**
1. 尝试先以UTF-8格式读取文件。
2. 如果引发了`UnicodeDecodeError`，请尝试`utf-8-sig`（带BOM的UTF-8），然后再尝试`latin-1`。
3. 一旦成功解码，重新编码并将工作副本写入UTF-8。
4. 如果没有编码成功，停止并报告：`Unable to decode input file. Please save it as UTF-8 and try again.`阶段3：检查和建议

####执行规则
—为该阶段生成**脚本**。
不要把这个阶段和其他阶段结合在一起。
—不预先计算未来阶段的数据。
—将输出存储为JSON文件：[`./run/data/report-data.json`]（./run/data/report-data.json）

####模式定义下面的JSON结构在阶段3是**IMMUTABLE**。阶段4稍后将向`Entries`中的每个对象添加一个`TunedEntry`对象—这是唯一允许的模式扩展，并在单独的阶段中进行。

JSON键直接映射到模板占位符，如`{{.CountryCode}}`、`{{.HasError}}`等。```json
{
  "InputFile": "",
  "Timestamp": 0,

  "TotalEntries": 0,
  "IpV4Entries": 0,
  "IpV6Entries": 0,
  "InvalidEntries": 0,

  "Errors": 0,
  "Warnings": 0,
  "OK": 0,
  "Suggestions": 0,

  "CityLevelAccuracy": 0,
  "RegionLevelAccuracy": 0,
  "CountryLevelAccuracy": 0,
  "DoNotGeolocate": 0,

  "Entries": [
    {
      "Line": 0,
      "IPPrefix": "",
      "CountryCode": "",
      "RegionCode": "",
      "City": "",

      "Status": "",
      "IPVersion": "",

      "Messages": [
        {
          "ID": "",
          "Type": "",
          "Text": "",
          "Checked": false
        }
      ],

      "HasError": false,
      "HasWarning": false,
      "HasSuggestion": false,
      "DoNotGeolocate": false,
      "GeocodingHint": "",
      "Tunable": false
    }
  ]
}
```
字段定义:顶级元数据:* * * *
—`InputFile`：原始输入源，可以是本地文件名，也可以是远程URL。
—`Timestamp`：从执行调优的Unix纪元开始的毫秒数。
—`TotalEntries`：处理的数据行总数（不包括注释和空行）。
—`IpV4Entries`: IPv4子网的表项计数。
—`IpV6Entries`: IPv6子网表项计数。
—`InvalidEntries`：前缀解析和CSV解析失败的表项计数。
—`Errors`：“`Status`”为“`ERROR`”的表项总数。
—`Warnings`：“`Status`”为“`WARNING`”的表项总数。
—`OK`：“`Status`”为“`OK`”的条目总数。
—`Suggestions`：“`Status`”为“`SUGGESTION`”的条目总数。
—`CityLevelAccuracy`:`City`不为空的有效条目计数。
—`RegionLevelAccuracy`:`RegionCode`为非空且`City`为空的有效条目计数。
—`CountryLevelAccuracy`:`CountryCode`为非空、`RegionCode`为空、`City`为空的有效条目计数泰。
—`DoNotGeolocate`（元数据）：`CountryCode`、`RegionCode`和`City`全部为空的有效条目计数。* *输入字段:* *
-`Entries`：对象数组，每行一个，每个条目字段如下：
—`Line`：原始CSV文件中以1为基数的行号（包括注释和空格在内的所有行）。
—`IPPrefix`：以CIDR斜杠表示的归一化IP前缀。
—`CountryCode`: ISO 3166-1 alpha-2国家代码，或空字符串。
—`RegionCode`: ISO 3166-2区域代码（例如，`US-CA`），或空字符串。
—`City`：城市名称，或空字符串。
-`Status`：最高级别：`ERROR`>`WARNING`>`SUGGESTION`>`OK`。
—`IPVersion`：根据解析后的IP前缀选择`"IPv4"`或`"IPv6"`。
-`Messages`：消息对象的数组，每个具有：    - `ID`: String identifier from the **Validation Rules Reference** table below (e.g., `"1101"`, `"3301"`).
    - `Type`: The severity type: `"ERROR"`, `"WARNING"`, or `"SUGGESTION"`.
    - `Text`: The human-readable validation message string.
    - `Checked`: `true` if the validation rule is auto-tunable (`Tunable: true` in the reference table), `false` otherwise. Controls whether the checkbox in the report is `checked` or `disabled`.
-`HasError`:`true`，如果任何消息有`Type``"ERROR"`。
-`HasWarning`:`true`如果任何消息有`Type``"WARNING"`。
-`HasSuggestion`:`true`如果任何消息有`Type``"SUGGESTION"`。
—`DoNotGeolocate`(entry):`true`如果`CountryCode`为空或`"ZZ"`-该条目是一个显式的不定位信号。
—`GeocodingHint`：在阶段3总是空字符串`""`。保留以备将来使用。
-`Tunable`:`true`如果**any**消息中有`Checked: true`。作为跨所有消息的`Checked`值的逻辑或计算。这个标志驱动“Tune”按钮在报告中的可见性。

####验证规则参考

在向条目添加消息时，使用该表中的`ID`、`Type`、`Text`和`Checked`值。

| ID |类型|文本|已检查|条件参考||--------|--------------|------------------------------------------------------------------------------------------------|---------|----------------------------------------|
|`1101`|`ERROR`| IP前缀为空|`false`| IP前缀分析：空|
|`1102`|`ERROR`|无效的IP前缀：无法解析为IPv4或IPv6网络|`false`| IP前缀分析：语法无效|
|`1103`|`ERROR`| rfc8805馈电不允许非公网IP范围|`false`| IP前缀分析：非公网|
|`3101`|`SUGGESTION`| IPv4前缀异常大，可能表示输入错误|`false`| IP前缀分析：IPv4 < /22 |
|`3102`|`SUGGESTION`| IPv6前缀异常大，可能表示输入错误|`false`| IP前缀分析：IPv6 < /64 |
|`1201`|`ERROR`|国家代码无效：不是一个有效的ISO 3166-1 alpha-2值|`true`|国家代码分析：无效|
|`1301`|`ERROR`|无效区域格式；预期的国家细分（例如，US-CA） |`true`|区域代码分析：格式不良|
|`1302`|`ERROR`|无效的区域代码：不是一个有效的ISO 3166-2细分|`true`|区域代码分析：未知代码|
|`1303`|`ERROR`|地区代码与指定的国家代码不匹配|`true`|地区代码分析：不匹配|
|`1401`|`ERROR`|无效的城市名称：不允许占位符值|`false`|城市名称分析：占位符|
|`1402`|`ERROR`|无效城市名称：检测到的缩写或代码值|`true`|城市名称分析：缩写|
|`2401`|`WARNING`|城市名称格式不一致；考虑规范化|`true`|城市名称分析：格式化|
|`1501`|`ERROR`|邮政编码已被RFC 8805弃用，出于隐私原因必须删除|`true`|邮政编码检查|
|`3301`|`SUGGESTION`|区域通常是不需要的小领土；考虑移除区域值|`true`|调优：小区域|
|`3402`|`SUGGESTION`|对于小区域通常不需要城市级粒度；考虑移除城市值|`true`|调整：小领土城市|
|`3303`|`SUGGESTION`|推荐使用地区代码当指定城市时终止；从下拉菜单中选择一个区域|`true`|调优：缺少城市|的区域
|`3104`|`SUGGESTION`|确认该子网是否被故意标记为不定位或丢失位置数据|`true`|调优：未指定地理位置|####填充消息

当验证检查匹配时，使用引用表中的值向条目的`Messages`数组添加一条消息：```python
entry["Messages"].append({
    "ID": "1201",      # From the table
    "Type": "ERROR",   # From the table
    "Text": "Invalid country code: not a valid ISO 3166-1 alpha-2 value",  # From the table
    "Checked": True    # From the table (True = tunable)
})
```
在为条目填充所有消息后，派生入门级标志：```python
entry["HasError"] = any(m["Type"] == "ERROR" for m in entry["Messages"])
entry["HasWarning"] = any(m["Type"] == "WARNING" for m in entry["Messages"])
entry["HasSuggestion"] = any(m["Type"] == "SUGGESTION" for m in entry["Messages"])
entry["Tunable"] = any(m["Checked"] for m in entry["Messages"])
```
####精度等级计数规则

准确度等级是相互排斥的。根据最细粒度的非空geo字段，将每个有效（非error，非无效）的条目分配到一个桶中：

|条件|桶||--------------------------------------------------------------|-----------------------------|
|`City`为非空|`CityLevelAccuracy`|
|`RegionCode`非空且`City`为空|`RegionLevelAccuracy`|
|`CountryCode`非空，`RegionCode`和`City`空|`CountryLevelAccuracy`|
|`DoNotGeolocate`（入口）是`true`|`DoNotGeolocate`（元数据）|

**不要在任何精度桶中计算`HasError: true`或`InvalidEntries`的**项。

代理人不得：
-重命名字段
—添加或删除字段
-更改数据类型
-重新排序键
-改变嵌套
-包装对象
—分裂成多个文件

如果一个值是未知的，**保留它为空** -永远不要发明数据。

####结构和格式检查

此阶段验证提要格式良好且可解析。在调谐器分析地理定位质量之前，必须解决关键的结构误差。

##### CSV结构本小节定义用于IP地理位置提要的csv格式输入文件的规则。
目标是确保文件可以可靠地解析并规范化为一致的内部表示。

- **CSV结构检查**
—如果已有“`pandas`”，使用“`pandas`”进行CSV解析。
—否则，退回到Python内置的`csv`模块。

—确保CSV包含的逻辑列**恰好为4或5列**。
-允许注释行。
-标题行**可以存在，也可以不存在。
—如果不存在标题行，则假定隐式列顺序：    ```
    ip_prefix, alpha2code, region, city, postal code (deprecated)
    ```
—参考示例输入文件：    [`assets/example/01-user-input-rfc8805-feed.csv`](assets/example/01-user-input-rfc8805-feed.csv)
- **CSV清理和规范化**
-使用Python逻辑清理和规范化CSV，相当于以下操作：    - Select only the **first five columns**, dropping any columns beyond the fifth.
    - Write the output file with a **UTF-8 BOM**.
——* *评论* *    - Remove comment rows where the **first column begins with `#`**.
    - This also removes a header row if it begins with `#`.
    - Create a map of comments using the **1-based line number** as the key and the full original line as the value. Also store blank lines.
    - Store this map in a JSON file at: [`./run/data/comments.json`](./run/data/comments.json)
    - Example: `{ "4": "# It's OK for small city states to leave state ISO2 code unspecified" }`
- * * * *
—两个实现路径（`pandas`和内置的`csv`）都必须使用    the `utf-8-sig` encoding to ensure a **UTF-8 BOM** is present.
#### IP前缀分析
—检查每个表项的`IPPrefix`字段是否存在且不为空。
-检查跨条目的重复`IPPrefix`值。
—如果发现重复，则停止该技能，并向用户报告：`Duplicate IP prefix detected: {ip_prefix_value} appears on lines {line_numbers}`—如果没有发现重复项，继续分析。

* * - * *检查    - Each subnet must parse cleanly as either an **IPv4 or IPv6 network** using the code snippets in the `references/` folder.
    - Subnets must be normalized and displayed in **CIDR slash notation**.
      - Single-host IPv4 subnets must be represented as **`/32`**.
      - Single-host IPv6 subnets must be represented as **`/128`**.
* * - * *错误    - Report the following conditions as **ERROR**:

    - **Invalid subnet syntax**
      - Message ID: `1102`

    - **Non-public address space**
      - Applies to subnets that are **private, loopback, link-local, multicast, or otherwise non-public**
        - In Python, detect non-public ranges using `is_private` and related address properties as shown in `./references`.
      - Message ID: `1103`
- * * * *的建议    - Report the following conditions as **SUGGESTION**:

    - **Overly large IPv6 subnets**
      - Prefixes shorter than `/64`
      - Message ID: `3102`

    - **Overly large IPv4 subnets**
      - Prefixes shorter than `/22`
      - Message ID: `3101`
####地理位置质量检查

分析地理定位数据的准确性和一致性；
-国家代码
-地区代码
-城市名称
-弃用字段

此阶段在结构检查通过后运行。

#####国家代码分析
—使用本地可用的数据表[`ISO3166-1`]（assets/iso3166-1.json）进行检查。    - JSON array of countries and territories with ISO codes
    - Each object includes:
      - `alpha_2`: two-letter country code
      - `name`: short country name
      - `flag`: flag emoji
    - This file represents the **superset of valid `CountryCode` values** for an RFC 8805 CSV.
-根据`alpha_2`属性检查条目的`CountryCode`（RFC 8805 Section 2.1.1.2，列`alpha2code`）。
—样例代码在`references/`目录下。

-如果在[`assets/small-territories.json`]（assets/small-territories.json）中发现一个国家，在内部将该条目标记为一个小领土。该标志在以后的检查和建议中使用，但**不存储在输出JSON**中（它是临时验证状态）。

- **注：**`small-territories.json`包含一些historic/disputed代码（`AN`,`CS`,`XK`），这些代码在`iso3166-1.json`中不存在。使用其中一个作为其`CountryCode`的条目将无法通过国家代码验证（ERROR），即使它作为一个小领土进行匹配。国家代码ERROR优先-不要基于小领土标志来抑制它。

* * - * *错误    - Report the following conditions as **ERROR**:
    - **Invalid country code**
      - Condition: `CountryCode` is present but not found in the `alpha_2` set
      - Message ID: `1201`
- * * * *的建议    - Report the following conditions as **SUGGESTION**:

    - **Unspecified geolocation for subnet**
      - Condition: All geographical fields (`CountryCode`, `RegionCode`, `City`) are empty for a subnet.
      - Action: 
        - Set `DoNotGeolocate = true` for the entry.
        - Set `CountryCode` to `ZZ` for the entry.
      - Message ID: `3104`
#####区域代码分析
—使用本地可用的数据表[`ISO3166-2`]（assets/iso3166-2.json）进行检查。    - JSON array of country subdivisions with ISO-assigned codes
    - Each object includes:
      - `code`: subdivision code prefixed with country code (e.g., `US-CA`)
      - `name`: short subdivision name
    - This file represents the **superset of valid `RegionCode` values** for an RFC 8805 CSV.
—如果提供了`RegionCode`值（RFC 8805 Section 2.1.1.3）：    - Check that the format matches `{COUNTRY}-{SUBDIVISION}` (e.g., `US-CA`, `AU-NSW`).
    - Check the value against the `code` attribute (already prefixed with the country code).
- **小领土例外：**如果条目是小领土**和**`RegionCode`值等于条目的`CountryCode`（例如，`SG`既是新加坡的国家和地区），将该区域视为可接受的-跳过该条目的所有区域验证检查。小领土实际上是城邦，没有意义的ISO 3166-2行政区划。

* * - * *错误    - Report the following conditions as **ERROR**:
    - **Invalid region format**
      - Condition: `RegionCode` does not match `{COUNTRY}-{SUBDIVISION}` **and** the small-territory exception does not apply
      - Message ID: `1301`
    - **Unknown region code**
      - Condition: `RegionCode` value is not found in the `code` set **and** the small-territory exception does not apply
      - Message ID: `1302`
    - **Country–region mismatch**
      - Condition: Country portion of `RegionCode` does not match `CountryCode`
      - Message ID: `1303`
#####城市名称分析

-城市名称仅使用启发式检查进行验证。
-目前**没有权威数据集**可用于验证城市名称。

* * - * *错误    - Report the following conditions as **ERROR**:
    - **Placeholder or non-meaningful values**
      - Condition: Placeholder or non-meaningful values including but not limited to:
        - `undefined`
        - `Please select`
        - `null`
        - `N/A`
        - `TBD`
        - `unknown`
      - Message ID: `1401`

    - **Truncated names, abbreviations, or airport codes**
      - Condition: Truncated names, abbreviations, or airport codes that do not represent valid city names:
        - `LA`
        - `Frft`
        - `sin01`
        - `LHR`
        - `SIN`
        - `MAA`
      - Message ID: `1402`
* * - * *警告    - Report the following conditions as **WARNING**:
    - **Inconsistent casing or formatting**
      - Condition: City names with inconsistent casing, spacing, or formatting that may reduce data quality, for example:
        - `HongKong` vs `Hong Kong`
        - Mixed casing or unexpected script usage
      - Message ID: `2401`
#####邮编查询
- RFC 8805节2.1.1.5明确**反对邮政或邮政编码**。
邮政编码可以代表非常小的人口，并且在映射IP地址范围时不被认为是隐私安全的，这本质上是统计性的。

* * - * *错误    - Report the following conditions as **ERROR**:
    - **Postal code present**
      - Condition: A non-empty value is present in the postal/ZIP code field.
      - Message ID: `1501`
####调优与建议

这一阶段应用了RFC 8805之外的建议，这些建议是从现实世界的地理feed部署中学到的，可以提高准确性和可用性。

- * * * *的建议
—将以下情况以“**建议**”的形式报告：

- **小领土指定的地区或城市**    - Condition:
      - Entry is a small territory
      - `RegionCode` is non-empty **OR**
      - `City` is non-empty.
    - Message IDs: `3301` (for region), `3402` (for city)
- **当城市输入**时，缺少地区代码    - Condition:
      - `City` is non-empty
      - `RegionCode` is empty
      - Entry is **not** a small territory
    - Message ID: `3303`
阶段4：调优数据查找

# # # #的目标
使用Fastah的`rfc8805-row-place-search`工具查找所有`Entries`。

####执行规则
—生成一个新的**脚本** _only_用于有效负载生成（读取数据集并写入一个或多个有效负载JSON文件，不从该脚本调用MCP）。
-服务器只接受每个请求1000个条目，所以如果有超过1000个条目，拆分为多个请求。
—代理必须读取生成的有效负载文件，从中构造请求，并将这些请求批量发送到MCP服务器，每个请求不超过1000个条目。
- ** MCP失败：**如果MCP服务器不可达，返回错误，或任何批处理都没有返回结果，记录警告并继续阶段5。为所有受影响的条目设置`TunedEntry: {}`。不要阻止报表生成。明确通知用户：`Tuning data lookup unavailable; the report will show validation results only.`-建议是**咨询** *永远不会自动填充**他们。####步骤1：使用重复数据删除构建查找有效负载

从[./run/data/report-data.json]（./run/data/report-data.json）加载数据集
—读取“`Entries`”数组。每个条目将用于构建MCP查找有效负载。

通过重复删除相同条目来减少服务器请求：
—对于`Entries`中的每个条目，计算一个内容哈希值（`CountryCode`+`RegionCode`+`City`）。
—创建重复数据删除映射：`{ contentHash -> { rowKey, payload, entryIndices: [] } }`。rowKey是一个UUID，它将被发送到MCP服务器以匹配响应。
—如果条目的哈希已经存在，则将其在`Entries`中基于**0的数组索引**追加到该重复数据删除条目的`entryIndices`数组。
—如果是新的hash，生成一个“**UUID (rowKey)**”，并创建新的重复数据删除表项。构建请求批次：
—从映射中提取唯一的重复数据删除项，保持重复数据删除的顺序。
-构建请求批次最多1000个项目。
-对于每个批处理，保持一个内存结构，如`[{ rowKey, payload, entryIndices }, ...]`，以匹配rowKey返回的响应。
-当写入MCP有效载荷文件时，包括每个有效载荷对象的`rowKey`字段：```json
[
    {"rowKey": "550e8400-e29b-41d4-a716-446655440000", "countryCode":"CA","regionCode":"CA-ON","cityName":"Toronto"},
    {"rowKey": "6ba7b810-9dad-11d1-80b4-00c04fd430c8", "countryCode":"IN","regionCode":"IN-KA","cityName":"Bangalore"},
    {"rowKey": "6ba7b811-9dad-11d1-80b4-00c04fd430c8", "countryCode":"IN","regionCode":"IN-KA"}
]
```
—读取响应时，将每个响应的`rowKey`字段与对应的重复数据删除表项进行匹配，获取所有关联的`entryIndices`。

规则:
写入负载到：[./run/data/mcp-server-payload.json]（./run/data/mcp-server-payload.json）
—写入负载后退出脚本。

####步骤2：调用Fastah MCP Tool

—Fastah MCP服务器`mcp.json`风格配置举例如下：```json
    "fastah-ip-geofeed": {
      "type": "http",
      "url": "https://mcp.fastah.ai/mcp"
    }
```
—服务器端：`https://mcp.fastah.ai/mcp`—工具及其模式：在第一个`tools/call`之前，代理必须发送一个`tools/list`请求来读取**`rfc8805-row-place-search`**的输入和输出模式。
使用发现的模式作为字段名称、类型和约束的权威源。
-以下仅为说明性示例；总是遵从`tools/list`返回的模式：  ```json
  [
      {"rowKey": "550e8400-...", "countryCode":"CA", ...},
      {"rowKey": "690e9301-...", "countryCode":"ZZ", ...}
  ]
- Open [./run/data/mcp-server-payload.json](./run/data/mcp-server-payload.json) and send all deduplicated entries with their rowKeys.
- If there are more than 1000 deduplicated entries after deduplication, split into multiple requests of 1000 entries each.
- The server will respond with the same `rowKey` field in each response for mapping back.
- Do NOT use local data.

#### Step 3: Attach Tuned Data to Entries

- Generate a new **script** for attaching tuned data.
- Load both [./run/data/report-data.json](./run/data/report-data.json) and the deduplication map (held in memory from Step 1, or re-derived from the payload file).
- For each response from the MCP server:
  - Extract the `rowKey` from the response.
  - Look up the `entryIndices` array associated with that `rowKey` from the deduplication map.
  - For each index in `entryIndices`, attach the best match to `Entries[index]`.
- Use the **first (best) match** from the response when available.

Create the field on each affected entry if it does not exist. Remap the MCP API response keys to Go struct field names:

```json
" TunedEntry ": {
“名称”:“”,
“CountryCode”:“”,
“RegionCode”:“”,
“讲师”:“”,
“H3Cells”:[],
“边界框(“大小):[]
}```

The `TunedEntry` field is a **single object** (not an array). It holds the best match from the MCP server.

**MCP response key → JSON key mapping**:
| MCP API response key | JSON key                   |
|----------------------|----------------------------|
| `placeName`          | `Name`                     |
| `countryCode`        | `CountryCode`              |
| `stateCode`          | `RegionCode`               |
| `placeType`          | `PlaceType`                |
| `h3Cells`            | `H3Cells`                  |
| `boundingBox`        | `BoundingBox`              |

Entries with no UUID match (i.e. the MCP server returned no response for their UUID) must receive an empty `TunedEntry: {}` object — never leave the field absent.

- Write the dataset back to: [./run/data/report-data.json](./run/data/report-data.json)
- Rules:
  - Maintain all existing validation flags.
  - Do NOT create additional intermediate files.


### Phase 5: Generate Tuning Report

Generate a **self-contained HTML report** by rendering the template at `./scripts/templates/index.html` with data from `./run/data/report-data.json` and `./run/data/comments.json`.

Write the completed report to `./run/report/geofeed-report.html`. After generating, attempt to open it in the system's default browser (e.g., `webbrowser.open()`). If running in a headless environment, CI pipeline, or remote container where no browser is available, skip the browser step and instead present the file path to the user so they can open or download it.

**The template uses Go `html/template` syntax** (`{{.Field}}`, `{{range}}`, `{{if eq}}`, etc.). Write a Python script that reads the template, builds a rendering context from the JSON data files, and processes the template placeholders to produce final HTML. Do not modify the template file itself — all processing happens in the Python script at render time.

#### Step 1: Replace Metadata Placeholders

Replace each `{{.Metadata.X}}` placeholder in the template with the corresponding value from `report-data.json`. Since JSON keys match the template placeholder, the mapping is direct — `{{.Metadata.InputFile}}` maps to the `InputFile` JSON key, etc.

| Template placeholder                   | JSON key (`report-data.json`)     |
|----------------------------------------|-----------------------------------|
| `{{.Metadata.InputFile}}`              | `InputFile`                       |
| `{{.Metadata.Timestamp}}`              | `Timestamp`                       |
| `{{.Metadata.TotalEntries}}`           | `TotalEntries`                    |
| `{{.Metadata.IpV4Entries}}`            | `IpV4Entries`                     |
| `{{.Metadata.IpV6Entries}}`            | `IpV6Entries`                     |
| `{{.Metadata.InvalidEntries}}`         | `InvalidEntries`                  |
| `{{.Metadata.Errors}}`                 | `Errors`                          |
| `{{.Metadata.Warnings}}`               | `Warnings`                        |
| `{{.Metadata.Suggestions}}`            | `Suggestions`                     |
| `{{.Metadata.OK}}`                     | `OK`                              |
| `{{.Metadata.CityLevelAccuracy}}`      | `CityLevelAccuracy`               |
| `{{.Metadata.RegionLevelAccuracy}}`    | `RegionLevelAccuracy`             |
| `{{.Metadata.CountryLevelAccuracy}}`   | `CountryLevelAccuracy`            |
| `{{.Metadata.DoNotGeolocate}}`         | `DoNotGeolocate` (metadata)       |

**Note on `{{.Metadata.Timestamp}}`:** This placeholder appears inside a JavaScript `new Date(...)` call. Replace it with the raw integer value (no HTML escaping needed for a numeric literal inside `<script>`). All other metadata values should be HTML-escaped since they appear inside HTML element text.

#### Step 2: Replace the Comment Map Placeholder

Locate this pattern in the template:
```javascript
const commentMap = {{.Comments}}；```

Replace `{{.Comments}}` with the serialized JSON object from `./run/data/comments.json`. The JSON is embedded directly as a JavaScript object literal (not inside a string), so no extra escaping is needed:

```python
Comments_json = json.dumps（comments）
Template = Template .replace("{{；评论}}”,comments_json)```

#### Step 3: Expand the Entries Range Block

The template contains a `{{range .Entries}}...{{end}}` block inside `<tbody id="entriesTableBody">`. Process it as follows:

1. **Extract** the range block body using regex. **Critical:** The block contains nested `{{end}}` tags (from `{{if eq .Status ...}}`, `{{if .Checked}}`, and `{{range .Messages}}`). A naive non-greedy match like `\{\{range \.Entries\}\}(.*?)\{\{end\}\}` will match the **first** inner `{{end}}`, truncating the block. Instead, anchor the outer `{{end}}` to the `</tbody>` that follows it:
    ```python
    m = re.search(
        r'\{\{range \.Entries\}\}(.*?)\{\{end\}\}\s*</tbody>',
        template,
        re.DOTALL,
    )
    entry_body = m.group(1)  # template text for one entry iteration
    ```
    This ensures you capture the full block body including all three `<tr>` rows and the nested `{{range .Messages}}...{{end}}`.
2. **Iterate** over each entry in `report-data.json`'s `Entries` array.
3. **Expand** the block body for each entry using the processing order below.
4. **Replace** the entire match (from `{{range .Entries}}` through `</tbody>`) with the concatenated expanded HTML followed by `</tbody>`.

**Processing order for each entry** (innermost constructs first to avoid `{{end}}` confusion):
1. Evaluate `{{if eq .Status ...}}...{{end}}` conditionals (status badge class and icon).
2. Evaluate `{{if .Checked}}...{{end}}` conditional (message checkbox).
3. Expand `{{range .Messages}}...{{end}}` inner range.
4. Replace simple `{{.Field}}` placeholders.

##### Entry Field Mapping

Within the range block body, replace these placeholders for each entry. Since JSON keys match the template placeholder, the template placeholder `{{.X}}` maps directly to JSON key `X`:

| Template placeholder           | JSON key (`Entries[]`)       | Notes                                                        |
|--------------------------------|------------------------------|--------------------------------------------------------------|
| `{{.Line}}`                    | `Line`                       | Direct integer value                                         |
| `{{.IPPrefix}}`                | `IPPrefix`                   | HTML-escaped                                                 |
| `{{.CountryCode}}`             | `CountryCode`                | HTML-escaped                                                 |
| `{{.RegionCode}}`              | `RegionCode`                 | HTML-escaped                                                 |
| `{{.City}}`                    | `City`                       | HTML-escaped                                                 |
| `{{.Status}}`                  | `Status`                     | HTML-escaped                                                 |
| `{{.HasError}}`                | `HasError`                   | Lowercase string: `"true"` or `"false"`                      |
| `{{.HasWarning}}`              | `HasWarning`                 | Lowercase string: `"true"` or `"false"`                      |
| `{{.HasSuggestion}}`           | `HasSuggestion`              | Lowercase string: `"true"` or `"false"`                      |
| `{{.GeocodingHint}}`           | `GeocodingHint`              | Empty string `""`                                            |
| `{{.DoNotGeolocate}}`          | `DoNotGeolocate`             | `"true"` or `"false"`                                        |
| `{{.Tunable}}`                 | `Tunable`                    | `"true"` or `"false"`                                        |
| `{{.TunedEntry.CountryCode}}`  | `TunedEntry.CountryCode`     | `""` if `TunedEntry` is empty `{}`                           |
| `{{.TunedEntry.RegionCode}}`   | `TunedEntry.RegionCode`      | `""` if `TunedEntry` is empty `{}`                           |
| `{{.TunedEntry.Name}}`         | `TunedEntry.Name`            | `""` if `TunedEntry` is empty `{}`                           |
| `{{.TunedEntry.H3Cells}}`      | `TunedEntry.H3Cells`         | Bracket-wrapped space-separated; `"[]"` if empty (see format below) |
| `{{.TunedEntry.BoundingBox}}`  | `TunedEntry.BoundingBox`     | Bracket-wrapped space-separated; `"[]"` if empty (see format below) |

**`data-h3-cells` and `data-bounding-box` format:** These are **NOT JSON arrays**. They are bracket-wrapped, space-separated values. Do **not** use JSON serialization (no quotes around string elements, no commas between numbers). Examples:
- `[836752fffffffff 836755fffffffff]` — correct
- `["836752fffffffff","836755fffffffff"]` — **WRONG**, quotes will break parsing
- `[-71.70 10.73 -71.52 10.55]` — correct
- `[]` — correct for empty

##### Evaluating Status Conditionals

**Process these BEFORE replacing simple `{{.Field}}` placeholders** — otherwise the `{{end}}` markers get consumed and the regex won't match.

The template uses `{{if eq .Status "..."}}` conditionals for the status badge CSS class and icon. Evaluate these by checking the entry's `status` value and keeping only the matching branch text.

The status badge line contains **two** `{{if eq .Status ...}}...{{end}}` blocks on a single line — one for the CSS class, one for the icon. Use `re.sub` with a callback to resolve all occurrences:

```python
STATUS_CSS = {"ERROR": "ERROR", "WARNING": "WARNING", "SUGGESTION": "SUGGESTION", "OK": "OK"}
Status_icon = {    "ERROR": "bi-x-circle-fill",
    "WARNING": "bi-exclamation-triangle-fill",
    "SUGGESTION": "bi-lightbulb-fill",
    "OK": "bi-check-circle-fill",
}

Def resolve_status_if(match_obj, status)：    """Pick the branch matching `status` from a {{if eq .Status ...}}...{{end}} block."""
    block = match_obj.group(0)
    # Try each branch: {{if eq .Status "X"}}val{{else if ...}}val{{else}}val{{end}}
    for st, val in [("ERROR",), ("WARNING",), ("SUGGESTION",)]:
        # not needed to parse generically — just map from the known patterns
    ...
```

A simpler approach: since there are exactly two known patterns, replace them as literal strings:
```python
css_class = STATUS_CSS。(地位、“ok”)
icon_class = STATUS_ICON。get(地位、“bi-check-circle-fill”)
Body = Body .replace（）    '{{if eq .Status "ERROR"}}error{{else if eq .Status "WARNING"}}warning{{else if eq .Status "SUGGESTION"}}suggestion{{else}}ok{{end}}',
    css_class,
）
Body = Body .replace（）    '{{if eq .Status "ERROR"}}bi-x-circle-fill{{else if eq .Status "WARNING"}}bi-exclamation-triangle-fill{{else if eq .Status "SUGGESTION"}}bi-lightbulb-fill{{else}}bi-check-circle-fill{{end}}',
    icon_class,
)
```
This avoids regex entirely and is safe because these exact strings appear verbatim in the template.

#### Step 4: Expand the Nested Messages Range

The `{{range .Messages}}...{{end}}` block contains a **nested** `{{if .Checked}} checked{{else}} disabled{{end}}` conditional, so its inner `{{end}}` would cause a simple non-greedy regex to match too early. Anchor the regex to `</td>` (the tag immediately after the messages range closing `{{end}}`) to capture the full block body:

```python
Msg_match = re.search(    r'\{\{range \.Messages\}\}(.*?)\{\{end\}\}\s*(?=</td>)',
    body, re.DOTALL
)
```

The lookahead `(?=</td>)` ensures the regex skips past the checkbox conditional's `{{end}}` (which is followed by `>`, not `</td>`) and matches only the range-closing `{{end}}` (which is followed by whitespace then `</td>`).

For each message in the entry's `Messages` array, clone the captured block body and expand it:

1. **Resolve the checkbox conditional** per message (must happen before simple placeholder replacement to remove the nested `{{end}}`):
   ```python
如果msg.get(“检查”):       msg_body = msg_body.replace(
           '{{if .Checked}} checked{{else}} disabled{{end}}', ' checked'
       )
其他:       msg_body = msg_body.replace(
           '{{if .Checked}} checked{{else}} disabled{{end}}', ' disabled'
       )
   ```

2. **Replace message field placeholders**:

   | Template placeholder | Source                            | Notes                          |
   |--------------------------|-----------------------------------|--------------------------------|
   | `{{.ID}}`                | `Messages[i].ID`                  | Direct string value from JSON  |
   | `{{.Text}}`              | `Messages[i].Text`                | HTML-escaped                   |

3. **Concatenate** all expanded message blocks and replace the original `{{range .Messages}}...{{end}}` match (`msg_match.group(0)`) with the result:
   ```python
Body = Body [:msg_match]。Start()] + “”。Join (expanded_msgs) + body[msg_match.end():]   ```

If `Messages` is empty, replace the entire matched region with an empty string (no message divs — only the issues header remains).

#### Output Guarantees

- The report must be readable in any modern browser without extra network dependencies beyond the CDN links already in the template (`leaflet`, `h3-js`, `bootstrap-icons`, Raleway font).
- All values embedded in HTML must be **HTML-escaped** (`<`, `>`, `&`, `"`) to prevent rendering issues.
- `commentMap` is embedded as a direct JavaScript object literal (not inside a string), so no JS string escaping is needed — just emit valid JSON.
- All values must be derived **only from analysis output**, not recomputed heuristically.


### Phase 6: Final Review

Perform a final verification pass using concrete, checkable assertions before presenting results to the user.

**Check 1 — Entry count integrity**
- Count non-comment, non-blank data rows in the original input CSV.
- Assert: `len(entries) in report-data.json == data_row_count`
- On failure: `Row count mismatch: input has {N} data rows but report contains {M} entries.`

**Check 2 — Summary counter integrity**
- These counters use **mutual exclusion** based on the boolean flags, which mirrors the highest-severity `Status` field. An entry with both `HasError: true` and `HasWarning: true` is counted only in `Errors`, never in `Warnings`. This is equivalent to counting by the entry's `Status` field.
- Assert all of the following; correct any that fail before generating the report:
  - `Errors == sum(1 for e in Entries if e['HasError'])`
  - `Warnings == sum(1 for e in Entries if e['HasWarning'] and not e['HasError'])`
  - `Suggestions == sum(1 for e in Entries if e['HasSuggestion'] and not e['HasError'] and not e['HasWarning'])`
  - `OK == sum(1 for e in Entries if not e['HasError'] and not e['HasWarning'] and not e['HasSuggestion'])`
  - `Errors + Warnings + Suggestions + OK == TotalEntries - InvalidEntries`

**Check 3 — Accuracy bucket integrity**
- Assert: `CityLevelAccuracy + RegionLevelAccuracy + CountryLevelAccuracy + DoNotGeolocate == TotalEntries - InvalidEntries`
- **Note:** The accuracy buckets defined in Phase 3 say "Do not count entries with `HasError: true`", but the Check 3 formula above uses `TotalEntries - InvalidEntries` (which still includes ERROR entries). This means ERROR entries (those that parsed as valid IPs but failed validation) **are** counted in accuracy buckets by their geo-field presence. Only `InvalidEntries` (unparsable IP prefixes) are excluded. Follow the Check 3 formula as the authoritative rule.
- On failure, trace and fix the bucketing logic before proceeding.

**Check 4 — No duplicate line numbers**
- Assert: all `Line` values in `Entries` are unique.
- On failure, report the duplicated line numbers to the user.

**Check 5 — TunedEntry completeness**
- Assert: every object in `Entries` has a `TunedEntry` key (even if its value is `{}`).
- On failure, add `"TunedEntry": {}` to any entry missing the key, then re-save `report-data.json`.

**Check 6 — Report file is present and non-empty**
- Confirm `./run/report/geofeed-report.html` was written and has a file size greater than zero bytes.
- On failure, regenerate the report before presenting to the user.
