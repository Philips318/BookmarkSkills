---
name: winmd-api-search
description: 'Find and explore Windows desktop APIs. Use when building features that need platform capabilities — camera, file access, notifications, UI controls, AI/ML, sensors, networking, etc. Discovers the right API for a task and retrieves full type details (methods, properties, events, enumeration values).'
license: Complete terms in LICENSE.txt
---
# WinMD API搜索

这项技能可以帮助您找到适合任何功能的Windows API，并获得其全部细节。它搜索所有WinMD元数据的本地缓存：

- **Windows平台SDK** -所有`Windows.*`WinRT api（始终可用，无需恢复）
- **WinAppSDK / WinUI** -捆绑作为缓存生成器的基线（始终可用，不需要恢复）
- **NuGet包** -恢复项目中包含`.winmd`文件的任何附加包
-类库（c++ /WinRT, c#）生成`.winmd`作为构建输出

即使在没有还原或构建的新克隆上，你仍然可以获得完整的平台SDK + WinAppSDK覆盖。

何时使用此技能-用户想要构建一个功能，你需要找到哪个API提供该功能
-用户询问“我如何做X?”，其中X涉及平台功能（摄像头，文件，通知，传感器，人工智能等）
—在编写代码之前，您需要确切的方法、属性、事件或类型的枚举值
-你不确定使用哪个控件、类或接口来完成UI或系统任务

# #先决条件

- * *。. NET SDK 8.0或更高版本** -需要构建缓存生成器。如果不可用，请从[dotnet.microsoft.com]（https://dotnet.microsoft.com/download）安装。

##设置缓存（第一次使用前需要）

所有查询和搜索命令都从本地JSON缓存读取。**必须在运行任何查询之前生成缓存```powershell
# All projects in the repo (recommended for first run)
.\.github\skills\winmd-api-search\scripts\Update-WinMdCache.ps1

# Single project
.\.github\skills\winmd-api-search\scripts\Update-WinMdCache.ps1 -ProjectDir <project-folder>
```
基线覆盖不需要项目恢复或构建（平台SDK + WinAppSDK）。对于其他NuGet包，项目需要`dotnet restore`（生成`project.assets.json`）或`packages.config`文件。

缓存存储在`Generated Files\winmd-cache\`，每个包+版本重复数据删除。

###什么被索引

|源|可用时||--------|----------------|
| Windows平台SDK |总是（从本地SDK安装读取）|
| WinAppSDK（最新）|总是（绑定为缓存生成器的基线）|
| WinAppSDK Runtime |安装在系统上时（通过`Get-AppxPackage`检测）|
|项目NuGet包|`dotnet restore`之后或与`packages.config`|
|项目输出`.winmd`|项目构建后（生成WinMD的类库）|

**注意：**这个缓存目录应该在`.gitignore`中-它是生成的，而不是源目录。

##如何使用

选择符合情况的路径：

---

### Discover -“我不知道该用哪个API”

用户用自己的语言描述功能。您需要找到正确的API。

* * 0。确保缓存存在**

如果还没有生成缓存，请先运行`Update-WinMdCache.ps1`—参见上面的[cache Setup]（#cache- Setup -required-before-first-use）。

* * 1。翻译用户语言→搜索关键词**将用户的日常语言映射为编程术语。尝试多种变化：

|用户说|搜索关键字尝试（按顺序）||-----------|-----------------------------------|
|“拍照”|`camera`,`capture`,`photo`,`MediaCapture`|
| "load from disk" |`file open`,`picker`,`FileOpen`,`StorageFile`|
| “描述里面有什么” |`image description`,`Vision`,`Recognition`|
| “显示一个弹出窗口” |`dialog`,`flyout`,`popup`,`ContentDialog`|
|“拖放”|`drag`,`drop`,`DragDrop`|
|“保存设置”|`settings`,`ApplicationData`,`LocalSettings`|

从简单的日常词汇开始。如果结果很弱或不相关，尝试更技术性的变化。

* * 2。运行搜索* *```powershell
.\.github\skills\winmd-api-search\scripts\Invoke-WinMdQuery.ps1 -Action search -Query "<keyword>"
```
这将返回具有顶级匹配类型的排序名称空间和JSON文件路径。

如果结果得分低（低于60分）或不相关，请退回到搜索在线文档。

1. 使用网络搜索在Microsoft Learn上找到合适的API，例如：
—`Windows.*`api为`site:learn.microsoft.com/uwp/api <capability keywords>`-`site:learn.microsoft.com/windows/windows-app-sdk/api/winrt <capability keywords>`用于`Microsoft.*`WinAppSDK api
2. 阅读文档页面以确定哪种类型符合用户的需求。
3. 一旦知道了类型名，再回来使用`-Action members`或`-Action enums`来获得确切的本地签名。

* * 3。读取JSON以选择正确的API**

从顶部结果的路径读取文件。JSON具有该命名空间中的所有类型——完整成员、签名、参数、返回类型、枚举值。

阅读并决定哪些类型和成员符合用户的要求。

* * 4。查阅官方文档了解上下文**缓存只包含签名——没有描述或使用指南。有关解释、示例和注释，请在Microsoft Learn上查找类型：

|命名空间前缀|文档基URL ||-----------------|----------------------|
|`Windows.*`|`https://learn.microsoft.com/uwp/api/{fully.qualified.typename}`|
|`Microsoft.*`(WinAppSDK) |`https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/{fully.qualified.typename}`|

例如，`Microsoft.UI.Xaml.Controls.NavigationView`映射到：`https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.controls.navigationview`
* * 5。使用API知识回答或编写代码**

---

###查找-“我知道API，告诉我细节”

您已经知道（或怀疑）类型或名称空间的名称。直接:```powershell
# Get all members of a known type
.\.github\skills\winmd-api-search\scripts\Invoke-WinMdQuery.ps1 -Action members -TypeName "Microsoft.UI.Xaml.Controls.NavigationView"

# Get enum values
.\.github\skills\winmd-api-search\scripts\Invoke-WinMdQuery.ps1 -Action enums -TypeName "Microsoft.UI.Xaml.Visibility"

# List all types in a namespace
.\.github\skills\winmd-api-search\scripts\Invoke-WinMdQuery.ps1 -Action types -Namespace "Microsoft.UI.Xaml.Controls"

# Browse namespaces
.\.github\skills\winmd-api-search\scripts\Invoke-WinMdQuery.ps1 -Action namespaces -Filter "Microsoft.UI"
```
如果需要`-Action members`显示的内容之外的完整细节，请使用`-Action search`获取JSON文件路径，然后直接读取JSON文件。

---

###其他命令```powershell
# List cached projects
.\.github\skills\winmd-api-search\scripts\Invoke-WinMdQuery.ps1 -Action projects

# List packages for a project
.\.github\skills\winmd-api-search\scripts\Invoke-WinMdQuery.ps1 -Action packages

# Show stats
.\.github\skills\winmd-api-search\scripts\Invoke-WinMdQuery.ps1 -Action stats
```
>如果只缓存一个项目，则自动选择`-Project`。
>如果存在多个项目，添加`-Project <name>`（使用`-Action projects`查看可用名称）。
>在扫描模式下，清单名称包含一个短哈希后缀，以避免冲突；如果没有歧义，则可以传递不带后缀的基本项目名称。

##搜索评分

搜索将根据查询对类型名称和成员名称进行排序：

|评分|匹配类型|示例||-------|-----------|---------|
|确切名称|`Button`→`Button`|
| 80 | |开头`Navigation`→`NavigationView`|
| 60 |包含|`Dialog`→`ContentDialog`|
| 50 | PascalCase首字母|`ASB`→`AutoSuggestBox`|
| 40 |多关键字AND |`navigation item`→`NavigationViewItem`|
|模糊字符匹配|`NavVw`→`NavigationView`|

结果按名称空间分组。得分较高的命名空间首先出现。

# #故障排除

|问题|修复||-------|-----|
| "Cache not found" |运行`Update-WinMdCache.ps1`|
| “多个项目缓存” |添加`-Project <name>`|
| "Namespace not found" |使用`-Action namespaces`列出可用的|
|使用完全限定名称（例如，`Microsoft.UI.Xaml.Controls.Button`） |
| NuGet更新后失效|重新运行`Update-WinMdCache.ps1`|
|添加`Generated Files/`到`.gitignore`|

# #引用

- [Windows平台SDK API参考](https://learn.microsoft.com/uwp/api/) -`Windows.*`命名空间的文档
- [Windows AppSDK API参考](https://learn.microsoft.com/windows/windows-app-sdk/api/winrt/) -`Microsoft.*`WinAppSDK命名空间的文档