# CodeQL SARIF输出参考

CodeQL分析产生的SARIF v2.1.0输出的详细参考。在解释或处理CodeQL扫描结果时使用它。

关于SARIF

SARIF（静态分析结果交换格式）是一种用于表示静态分析工具输出的标准化JSON格式。CodeQL生成SARIF v2.1.0（规范：`sarifv2.1.0`）。

-规格：[OASIS SARIF v2.1.0]（https://docs.oasis-open.org/sarif/sarif/v2.1.0/sarif-v2.1.0.html）
-模式：[sarif-schema-2.1.0.json]（https://docs.oasis-open.org/sarif/sarif/v2.1.0/errata01/os/schemas/sarif-schema-2.1.0.json）
-格式类型：`sarifv2.1.0`（传递给`--format`标志）

顶层结构`sarifLog`对象

|属性|总是生成|描述||---|:---:|---|
|`$schema`|✅| SARIF模式|的链接
|`version`|✅| SARIF规范版本（`"2.1.0"`） |
|`runs`|✅|数组，每种语言包含一个`run`对象|

###`run`对象

|属性|总是生成|描述||---|:---:|---|
|`tool`|✅|工具信息（`toolComponent`） |
|`artifacts`|✅|结果|中引用的每个文件的工件对象数组
|`results`|✅|`result`对象数组|
|`newLineSequences`|✅|换行字符序列|
|`columnKind`|✅|列计数方法|
|`properties`|✅|包含`semmle.formatSpecifier`，表示格式|

##工具信息`tool`对象

包含单个`driver`属性。`toolComponent`对象（驱动）

|属性|总是生成|描述||---|:---:|---|
|`name`|✅|`"CodeQL command-line toolchain"`|
|`organization`|✅|`"GitHub"`|
|`version`|✅| CodeQL发布版本（如`"2.19.0"`） |
|`rules`|✅|`reportingDescriptor`对象数组，用于available/run规则|

# #规则

对象（规则）

|属性|总是生成|描述||---|:---:|---|
|`id`|✅|`@id`查询属性的规则标识符（例如，`cpp/unsafe-format-string`）。如果定义，则使用`@opaqueid`。|
|`name`|✅|与查询|中的`@id`属性相同
|`shortDescription`|✅|从`@name`查询属性|
|`fullDescription`|✅|从`@description`查询属性|
|`defaultConfiguration`|❌|`reportingConfiguration`与`enabled`（true/false）和基于`@severity`的`level`。如果没有指定`@severity`，则省略。|

###严重性映射

CodeQL`@severity`| SARIF`level`||---|---|
|`error`|`error`|
|`warning`|`warning`|
|`recommendation`|`note`|

# #结果`result`对象

默认情况下，结果按唯一消息格式字符串和主要位置分组。在同一位置，具有相同消息的两个结果显示为单个结果。禁用使用`--ungroup-results`进行分组。

|属性|总是生成|描述||---|:---:|---|
|`ruleId`|✅|规则标识符（匹配`reportingDescriptor.id`） |
|`ruleIndex`|✅|索引到`rules`数组|
|`message`|✅|问题描述。可能包含SARIF “消息与占位符”链接到`relatedLocations`。|
|`locations`|✅|包含单个`location`对象|的数组
|`partialFingerprints`|✅|重复数据删除至少为`primaryLocationLineHash`的字典|
|`codeFlows`|❌|为带有一个或多个`codeFlow`对象的`@kind path-problem`查询填充|
|`relatedLocations`|❌|当消息有占位符选项时填充；每个独特的位置包含一次|
|`suppressions`|❌|如果被抑制：单个`suppression`对象与`@kind: IN_SOURCE`。如果不抑制，但其他结果是：空数组。否则：不设置。|

# # #指纹`partialFingerprints`包含:
-`primaryLocationLineHash`-指纹基于上下文的主要位置

GitHub用于跨提交跟踪警报并避免重复通知。

# #位置`location`对象|属性|总是生成|描述||---|:---:|---|
|`physicalLocation`|✅|物理文件位置|
|`id`|❌|存在于`relatedLocations`阵列|
|`message`|❌|存在于`relatedLocations`和`threadFlowLocation.location`|`physicalLocation`对象

|属性|总是生成|描述||---|:---:|---|
|`artifactLocation`|✅|文件引用|
|`region`|❌|提供文本文件位置|
|`contextRegion`|❌|当location有关联的片段|时出现

###`region`对象

可能产生两种类型的区域：

**Line/Column偏移区域：**

|属性|总是生成|描述||---|:---:|---|
|`startLine`|✅|起始行号|
|`startColumn`|❌|如果等于默认值1|则省略
|`endLine`|❌|如果与`startLine`|相同，则省略
|`endColumn`|✅|结束列号|
|`snippet`|❌|源代码片段|

**字符偏移区域：**

|属性|总是生成|描述||---|:---:|---|
|`charOffset`|✅|从文件|开始的字符偏移量
|`charLength`|✅|字符长度|
|`snippet`|❌|源代码片段|

>消费者应该健壮地处理这两种区域类型。

# #工件

###`artifact`对象

|属性|总是生成|描述||---|:---:|---|
|`location`|✅|`artifactLocation`对象|
|`index`|✅|工件|的索引
|`contents`|❌|使用`--sarif-add-file-contents`|时填充`artifactContent``artifactLocation`对象

|属性|总是生成|描述||---|:---:|---|
|`uri`|✅|文件路径（相对或绝对）|
|`index`|✅|索引参考|
|`uriBaseId`|❌|当文件相对于已知的抽象位置（例如，源根）|时设置

代码流（路径问题）

对于`@kind path-problem`的查询，结果包括显示数据流路径的代码流信息。

###`codeFlow`对象

|属性|总是生成|描述||---|:---:|---|
|`threadFlows`|✅|`threadFlow`对象数组|

###`threadFlow`对象

|属性|总是生成|描述||---|:---:|---|
|`locations`|✅|`threadFlowLocation`对象数组|

###`threadFlowLocation`对象

|属性|总是生成|描述||---|:---:|---|
|`location`|✅|流|中此步骤的`location`对象

##自动化细节

来自`github/codeql-action/analyze`的`category`值在SARIF输出中显示为`<run>.automationDetails.id`。

例子:```json
{
  "automationDetails": {
    "id": "/language:javascript-typescript"
  }
}
```
SARIF的关键CLI标志

b|标志|效果||---|---|
|`--format=sarif-latest`|产生SARIF v2.1.0输出|
|`--sarif-category=<cat>`|设置`automationDetails.id`作为结果分类|
|`--sarif-add-file-contents`|在`artifact.contents`|中包含源文件内容
|`--ungroup-results`|分别报告每次发生的重复数据删除（不按位置+消息）|
|`--output=<file>`|向指定文件|写入SARIF

第三方SARIF支持

当从非codeql工具上传SARIF时，确保在GitHub上填充这些属性以获得最佳效果。

推荐`reportingDescriptor`属性

|属性|必选|描述||---|:---:|---|
|`id`|✅|唯一规则标识符|
|`name`|❌|规则名称（最大255个字符）|
|`shortDescription.text`|✅|简要描述（最多1024个字符）|
|`fullDescription.text`|✅|完整描述（最大1024字符）|
|`defaultConfiguration.level`|❌|默认级别：`note`，`warning`,`error`|
|`help.text`|✅|文本格式的文档|
|`help.markdown`|❌| Markdown中的文档（可用时显示）|
|`properties.tags[]`|❌|过滤标签（例如，`security`） |
|`properties.precision`|❌|`very-high`,`high`,`medium`，`low`-影响显示顺序|
|`properties.problem.severity`|❌|非安全级别：`error`、`warning`、`recommendation`|
|`properties.security-severity`|❌|安全查询得分为0.0-10.0。映射到：| 9.0=临界，7.0-8.9 =高，4.0-6.9 =中等，0.1-3.9 =低|

源文件位置要求—尽可能使用相对路径（相对于存储库根目录）
—使用源根将绝对uri转换为相对uri
-源根可以通过以下方式设置：
—`checkout_path`输入为`github/codeql-action/analyze`—`checkout_uri`参数为SARIF上传API
—SARIF文件中的`invocations[0].workingDirectory.uri`—为了保证指纹的稳定性，需要保证每次运行时文件路径一致
—符号链接文件必须使用已解析（非符号链接）的uri

指纹要求

-`partialFingerprints`与`primaryLocationLineHash`防止跨提交的重复警报
—CodeQL SARIF自动包含指纹
—第三方指纹识别：`upload-sarif`动作在指纹缺失时计算指纹
—没有指纹的API上传可能会产生重复的警报

##上传限制

文件大小
-最大：** 10mb ** （gzip压缩）
—过大：缩小查询范围，删除`--sarif-add-file-contents`，或者拆分多次上传

对象计数限制

|对象|最大|值|---|---|
|按文件运行| 20 |
|每次运行结果| 25,000 |
|每次运行规则| 25,000 |
|每次运行的工具扩展| 100 |
|每个结果的线程流位置| 10,000 |
|每个结果的位置| 1,000 |
|每个规则的标签| 20 |

超过这些限制的文件将被拒绝。跨多个具有不同`--sarif-category`值的SARIF上传拆分分析。

# # #验证

在上传之前使用[Microsoft SARIF验证器]（https://sarifweb.azurewebsites.net/）验证SARIF文件。

##向后兼容性

标记为“always generated”的字段在以后的版本中不会被移除
-不总是生成的字段可能会改变它们出现的环境
-可以添加新的字段而不破坏更改
消费者应该对可选字段的存在和不存在都很健壮