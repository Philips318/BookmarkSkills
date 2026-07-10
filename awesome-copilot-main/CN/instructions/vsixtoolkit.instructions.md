---
description: 'Guidelines for Visual Studio extension (VSIX) development using Community.VisualStudio.Toolkit'
applyTo: '**/*.cs, **/*.vsct, **/*.xaml, **/source.extension.vsixmanifest'
---
使用Community.VisualStudio.Toolkit开发VisualStudio扩展

# #范围

**这些说明仅适用于使用`Community.VisualStudio.Toolkit`.**的Visual Studio扩展

通过检查以下项目来验证项目是否使用了工具包：
-`Community.VisualStudio.Toolkit.*`NuGet包参考
-`ToolkitPackage`基类（不是原始的`AsyncPackage`）
-`BaseCommand<T>`模式的命令

**如果项目使用原始VSSDK （`AsyncPackage`直接）或新的`VisualStudio.Extensibility`模型，请不要使用这些说明

# #目标

-生成异步优先，线程安全的扩展代码
-使用工具箱抽象（`VS.*`helper,`BaseCommand<T>`,`BaseOptionModel<T>`）
-确保所有的UI尊重Visual Studio主题
—遵循VSSDK和VSTHRD分析器规则
生成可测试、可维护的扩展代码
- **坚持`.editorconfig`设置**时，存在于存储库

代码样式（.editorconfig）

**如果存储库中存在`.editorconfig`文件，所有生成和修改的代码必须遵循其规则这包括但不限于：
-缩进样式（制表符vs空格）和大小
-行结束符和最后换行符要求
命名约定（字段、属性、方法等）
-代码风格偏好（`var`用法、表达式体、大括号等）
-分析器的严重程度和抑制

在生成代码之前，在存储库根目录中检查`.editorconfig`，并应用它的设置。如果有疑问，请匹配正在编辑的文件中周围代码的样式。

# #。. NET框架和c#语言约束

**Visual Studio扩展目标。. NET Framework 4.8**，但可以使用现代c#语法（直到c# 14）。. NET框架运行时。###✅支持现代c#功能
-主建造者
-文件作用域命名空间
-全局使用
-模式匹配（所有形式）
-记录（有限制）
-`init`访问器
—目标类型`new`-可空引用类型（仅限注释）
-原始字符串字面量
—集合表达式

###❌不支持。. NET框架限制)
-`Span<T>`,`ReadOnlySpan<T>`,`Memory<T>`（不支持运行时）
-`IAsyncEnumerable<T>`（不含填充包）
-默认接口实现
-`Index`和`Range`类型（运行时不支持`^`和`..`操作符）
-结构体上的`init`-only设置（运行时限制）
-`System.Text.Json`的一些特性

最佳实践
编写代码时，更倾向于使用。. NET Framework 4.8。如果需要现代API，请检查是否存在polyfill NuGet包（例如，`IAsyncEnumerable<T>`的`Microsoft.Bcl.AsyncInterfaces`）。

提示行为示例###✅好建议
“创建一个命令，使用`BaseCommand<T>`打开当前文件所在的文件夹”
-“添加一个布尔选项页面，使用`BaseOptionModel<T>`”
“为c#文件编写标记器提供程序，突出显示TODO注释”
-“在处理文件时显示状态栏进度指示器”

###❌避免
-建议原始`AsyncPackage`而不是`ToolkitPackage`-直接使用`OleMenuCommandService`而不是`BaseCommand<T>`-创建WPF元素而不首先切换到UI线程
—使用`.Result`、`.Wait()`或`Task.Run`进行UI工作
-硬编码颜色，而不是使用VS主题颜色

项目结构```
src/
├── Commands/           # Command handlers (menu items, toolbar buttons)
├── Options/            # Settings/options pages
├── Services/           # Business logic and services
├── Tagging/            # ITagger implementations (syntax highlighting, outlining)
├── Adornments/         # Editor adornments (IntraTextAdornment, margins)
├── QuickInfo/          # QuickInfo/tooltip providers
├── SuggestedActions/   # Light bulb actions
├── Handlers/           # Event handlers (format document, paste, etc.)
├── Resources/          # Images, icons, license files
├── source.extension.vsixmanifest  # Extension manifest
├── VSCommandTable.vsct            # Command definitions (menus, buttons)
├── VSCommandTable.cs              # Auto-generated command IDs
└── *Package.cs                    # Main package class
```
Community.VisualStudio.Toolkit模式

全局使用

使用工具包的扩展应该在Package文件中有这些全局用法：```csharp
global using System;
global using Community.VisualStudio.Toolkit;
global using Microsoft.VisualStudio.Shell;
global using Task = System.Threading.Tasks.Task;
```
包类```csharp
[PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
[InstalledProductRegistration(Vsix.Name, Vsix.Description, Vsix.Version)]
[ProvideMenuResource("Menus.ctmenu", 1)]
[Guid(PackageGuids.YourExtensionString)]
[ProvideOptionPage(typeof(OptionsProvider.GeneralOptions), Vsix.Name, "General", 0, 0, true, SupportsProfiles = true)]
public sealed class YourPackage : ToolkitPackage
{
    protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
    {
        await this.RegisterCommandsAsync();
    }
}
```
# # #命令

命令使用`[Command]`属性并继承`BaseCommand<T>`：```csharp
[Command(PackageIds.YourCommandId)]
internal sealed class YourCommand : BaseCommand<YourCommand>
{
    protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
    {
        // Command implementation
    }

    // Optional: Control command state (enabled, checked, visible)
    protected override void BeforeQueryStatus(EventArgs e)
    {
        Command.Checked = someCondition;
        Command.Enabled = anotherCondition;
    }
}
```
###选项页面```csharp
internal partial class OptionsProvider
{
    [ComVisible(true)]
    public class GeneralOptions : BaseOptionPage<General> { }
}

public class General : BaseOptionModel<General>
{
    [Category("Category Name")]
    [DisplayName("Setting Name")]
    [Description("Description of the setting.")]
    [DefaultValue(true)]
    public bool MySetting { get; set; } = true;
}
```
## MEF组件

标签提供者

使用`[Export]`和适当的`[ContentType]`属性：```csharp
[Export(typeof(IViewTaggerProvider))]
[ContentType("CSharp")]
[ContentType("Basic")]
[TagType(typeof(IntraTextAdornmentTag))]
[TextViewRole(PredefinedTextViewRoles.Document)]
internal sealed class YourTaggerProvider : IViewTaggerProvider
{
    [Import]
    internal IOutliningManagerService OutliningManagerService { get; set; }

    public ITagger<T> CreateTagger<T>(ITextView textView, ITextBuffer buffer) where T : ITag
    {
        if (textView == null || !(textView is IWpfTextView wpfTextView))
            return null;

        if (textView.TextBuffer != buffer)
            return null;

        return wpfTextView.Properties.GetOrCreateSingletonProperty(
            () => new YourTagger(wpfTextView)) as ITagger<T>;
    }
}
```
### QuickInfo资源```csharp
[Export(typeof(IAsyncQuickInfoSourceProvider))]
[Name("YourQuickInfo")]
[ContentType("code")]
[Order(Before = "Default Quick Info Presenter")]
internal sealed class YourQuickInfoSourceProvider : IAsyncQuickInfoSourceProvider
{
    public IAsyncQuickInfoSource TryCreateQuickInfoSource(ITextBuffer textBuffer)
    {
        return textBuffer.Properties.GetOrCreateSingletonProperty(
            () => new YourQuickInfoSource(textBuffer));
    }
}
```
建议行动（灯泡）```csharp
[Export(typeof(ISuggestedActionsSourceProvider))]
[Name("Your Suggested Actions")]
[ContentType("text")]
internal sealed class YourSuggestedActionsSourceProvider : ISuggestedActionsSourceProvider
{
    public ISuggestedActionsSource CreateSuggestedActionsSource(ITextView textView, ITextBuffer textBuffer)
    {
        return new YourSuggestedActionsSource(textView, textBuffer);
    }
}
```
##线程指南

WPF操作总是切换到UI线程```csharp
await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
// Now safe to create/modify WPF elements
```
背景工作```csharp
ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
{
    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
    await VS.Commands.ExecuteAsync("View.TaskList");
});
```
VSSDK和线程分析器规则

扩展应该强制执行这些分析器规则。添加到`.editorconfig`：```ini
dotnet_diagnostic.VSSDK*.severity = error
dotnet_diagnostic.VSTHRD*.severity = error
```
性能规则
| ID |规则|修复||----|------|-----|
| **VSSDK001** |源自`AsyncPackage`|使用`ToolkitPackage`（源自AsyncPackage） |
| **VSSDK002** |`AllowsBackgroundLoading = true`|添加到`[PackageRegistration]`|

线程规则（VSTHRD）
| ID |规则|修复||----|------|-----|
| **VSTHRD001** |避免`.Wait()`|使用`await`|
| **VSTHRD002** |避免`JoinableTaskFactory.Run`|使用`RunAsync`或`await`|
| **VSTHRD010** | COM调用需要UI线程|`await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync()`|
| **VSTHRD100** |否`async void`|使用`async Task`|
| **VSTHRD110** |观察同步结果|`await task;`或用pragma |抑制

Visual Studio主题

**所有UI必须尊重VS主题（浅色，深色，蓝色，高对比度）**

WPF主题与环境颜色```xml
<!-- MyControl.xaml -->
<UserControl x:Class="MyExt.MyControl"
             xmlns:vsui="clr-namespace:Microsoft.VisualStudio.PlatformUI;assembly=Microsoft.VisualStudio.Shell.15.0">
    <Grid Background="{DynamicResource {x:Static vsui:EnvironmentColors.ToolWindowBackgroundBrushKey}}">
        <TextBlock Foreground="{DynamicResource {x:Static vsui:EnvironmentColors.ToolWindowTextBrushKey}}"
                   Text="Hello, themed world!" />
    </Grid>
</UserControl>
```
### Toolkit自动主题化（推荐）

该工具包为WPF用户控件提供了自动主题化：```xml
<UserControl x:Class="MyExt.MyUserControl"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:toolkit="clr-namespace:Community.VisualStudio.Toolkit;assembly=Community.VisualStudio.Toolkit"
             toolkit:Themes.UseVsTheme="True">
    <!-- Controls automatically get VS styling -->
</UserControl>
```
对于对话框窗口，使用`DialogWindow`：```xml
<platform:DialogWindow
    x:Class="MyExt.MyDialog"
    xmlns:platform="clr-namespace:Microsoft.VisualStudio.PlatformUI;assembly=Microsoft.VisualStudio.Shell.15.0"
    xmlns:toolkit="clr-namespace:Community.VisualStudio.Toolkit;assembly=Community.VisualStudio.Toolkit"
    toolkit:Themes.UseVsTheme="True">
</platform:DialogWindow>
```
###常见主题颜色标记

|类别|令牌|用途||----------|-------|-------|
| **背景** |`EnvironmentColors.ToolWindowBackgroundBrushKey`|Window/panel背景|
| **前台** |`EnvironmentColors.ToolWindowTextBrushKey`|文本|
| **命令栏** |`EnvironmentColors.CommandBarTextActiveBrushKey`|菜单项|
| **链接** |`EnvironmentColors.ControlLinkTextBrushKey`|超链接|

具有主题意识的图标

使用VS Image Catalog中的`KnownMonikers`作为主题感知图标：```csharp
public ImageMoniker IconMoniker => KnownMonikers.Settings;
```
在VSCT:```xml
<Icon guid="ImageCatalogGuid" id="Settings"/>
<CommandFlag>IconIsMoniker</CommandFlag>
```
##通用VS SDK api

### VS助手方法（Community.VisualStudio.Toolkit）```csharp
// Status bar
await VS.StatusBar.ShowMessageAsync("Message");
await VS.StatusBar.ShowProgressAsync("Working...", currentStep, totalSteps);

// Solution/Projects
Solution solution = await VS.Solutions.GetCurrentSolutionAsync();
IEnumerable<SolutionItem> items = await VS.Solutions.GetActiveItemsAsync();
bool isOpen = await VS.Solutions.IsOpenAsync();

// Documents
DocumentView docView = await VS.Documents.GetActiveDocumentViewAsync();
string text = docView?.TextBuffer?.CurrentSnapshot.GetText();
await VS.Documents.OpenAsync(fileName);
await VS.Documents.OpenInPreviewTabAsync(fileName);

// Commands
await VS.Commands.ExecuteAsync("View.TaskList");

// Settings
await VS.Settings.OpenAsync<OptionsProvider.GeneralOptions>();

// Messages
await VS.MessageBox.ShowAsync("Title", "Message");
await VS.MessageBox.ShowErrorAsync("Extension Name", ex.ToString());

// Events
VS.Events.SolutionEvents.OnAfterOpenProject += OnAfterOpenProject;
VS.Events.DocumentEvents.Saved += OnDocumentSaved;
```
###使用设置```csharp
// Read settings synchronously
var value = General.Instance.MyOption;

// Read settings asynchronously
var general = await General.GetLiveInstanceAsync();
var value = general.MyOption;

// Write settings
General.Instance.MyOption = newValue;
General.Instance.Save();

// Or async
general.MyOption = newValue;
await general.SaveAsync();

// Listen for settings changes
General.Saved += OnSettingsSaved;
```
文本缓冲区操作```csharp
// Get snapshot
ITextSnapshot snapshot = textBuffer.CurrentSnapshot;

// Get line
ITextSnapshotLine line = snapshot.GetLineFromLineNumber(lineNumber);
string lineText = line.GetText();

// Create tracking span
ITrackingSpan trackingSpan = snapshot.CreateTrackingSpan(span, SpanTrackingMode.EdgeInclusive);

// Edit buffer
using (ITextEdit edit = textBuffer.CreateEdit())
{
    edit.Replace(span, newText);
    edit.Apply();
}

// Insert at caret position
DocumentView docView = await VS.Documents.GetActiveDocumentViewAsync();
if (docView?.TextView != null)
{
    SnapshotPoint position = docView.TextView.Caret.Position.BufferPosition;
    docView.TextBuffer?.Insert(position, "text to insert");
}
```
VSCT命令表Menu/Command结构```xml
<Commands package="YourPackage">
  <Menus>
    <Menu guid="YourPackage" id="SubMenu" type="Menu">
      <Parent guid="YourPackage" id="MenuGroup"/>
      <Strings>
        <ButtonText>Menu Name</ButtonText>
        <CommandName>Menu Name</CommandName>
        <CanonicalName>.YourExtension.MenuName</CanonicalName>
      </Strings>
    </Menu>
  </Menus>

  <Groups>
    <Group guid="YourPackage" id="MenuGroup" priority="0x0600">
      <Parent guid="guidSHLMainMenu" id="IDM_VS_CTXT_CODEWIN"/>
    </Group>
  </Groups>

  <Buttons>
    <Button guid="YourPackage" id="CommandId" type="Button">
      <Parent guid="YourPackage" id="MenuGroup"/>
      <Icon guid="ImageCatalogGuid" id="Settings"/>
      <CommandFlag>IconIsMoniker</CommandFlag>
      <CommandFlag>DynamicVisibility</CommandFlag>
      <Strings>
        <ButtonText>Command Name</ButtonText>
        <CanonicalName>.YourExtension.CommandName</CanonicalName>
      </Strings>
    </Button>
  </Buttons>
</Commands>

<Symbols>
  <GuidSymbol name="YourPackage" value="{guid-here}">
    <IDSymbol name="MenuGroup" value="0x0001"/>
    <IDSymbol name="CommandId" value="0x0100"/>
  </GuidSymbol>
</Symbols>
```
最佳实践

# # # 1。表演

—处理大文档前检查file/buffer大小
—使用`NormalizedSnapshotSpanCollection`进行高效的跨操作
-尽可能缓存解析结果
—在库代码中使用`ConfigureAwait(false)````csharp
// Skip large files
if (buffer.CurrentSnapshot.Length > 150000)
    return null;
```
# # # 2。错误处理

-在try-catch中包装外部操作
-正确记录错误
-永远不要让异常崩溃VS```csharp
try
{
    // Operation
}
catch (Exception ex)
{
    await ex.LogAsync();
}
```
# # # 3。可支配的资源

-在标记器和其他长寿对象上实现`IDisposable`-取消订阅处置中的事件```csharp
public void Dispose()
{
    if (!_isDisposed)
    {
        _buffer.Changed -= OnBufferChanged;
        _isDisposed = true;
    }
}
```
# # # 4。内容类型`[ContentType]`属性的常见内容类型：
-`"text"`-所有文本文件
-`"code"`-所有代码文件`"CSharp"`- c#文件
-`"Basic"`- VB。网络文件
-`"CSS"`,`"LESS"`，`"SCSS"`-样式文件
—`"TypeScript"`、`"JavaScript"`—脚本文件
-`"HTML"`、`"HTMLX"`- HTML文件
—`"XML"`—XML文件
—`"JSON"`—JSON文件

# # # 5。图像和图标

使用VS Image Catalog中的`KnownMonikers`：```csharp
public ImageMoniker IconMoniker => KnownMonikers.Settings;
```
在VSCT:```xml
<Icon guid="ImageCatalogGuid" id="Settings"/>
<CommandFlag>IconIsMoniker</CommandFlag>
```
# #测试

-对于需要VS上下文的测试使用`[VsTestMethod]`-尽可能模拟VS服务
-测试业务逻辑从VS集成分开

##常见陷阱

陷阱|解决方案||---------|----------|
|总是使用`async`/`await`|
|首先调用`SwitchToMainThreadAsync()`|
|忽略取消令牌|通过异步链|传递它们
|VSCommandTable.csmismatch | VSCT改变|后再生
|使用`PackageGuids`和`PackageIds`常量|
|吞食异常|日志`await ex.LogAsync()`|
|缺少动态可见性|需要`BeforeQueryStatus`工作|
|使用`.Result`，`.Wait()`|导致死锁；总是`await`|
使用VS主题颜色（`EnvironmentColors`） |
|使用`async Task`代替|

# #验证

构建并验证扩展：```bash
msbuild /t:rebuild
```
确保分析器在`.editorconfig`中启用：```ini
dotnet_diagnostic.VSSDK*.severity = error
dotnet_diagnostic.VSTHRD*.severity = error
```
发布前在VS实验实例中测试。

NuGet包

|包装|用途||---------|---------|
|`Community.VisualStudio.Toolkit.17`|简化VS扩展开发|
|`Microsoft.VisualStudio.SDK`| Core VS SDK |
|`Microsoft.VSSDK.BuildTools`| VSIX |的构建工具
|`Microsoft.VisualStudio.Threading.Analyzers`|线程分析器|
| VSSDK分析仪|

# #资源

——[Community.VisualStudio.Toolkit] (https://github.com/VsixCommunity/Community.VisualStudio.Toolkit)
- [VS扩展性文档]（https://learn.microsoft.com/en-us/visualstudio/extensibility/）
- [VSIX社区样本]（https://github.com/VsixCommunity/Samples）

## README和市场演示

一个好的自述文件可以在GitHub和VS Marketplace上使用。市场使用README.md作为扩展的描述页面。

自述结构```markdown
[marketplace]: https://marketplace.visualstudio.com/items?itemName=Publisher.ExtensionName
[repo]: https://github.com/user/repo

# Extension Name

[![Build](https://github.com/user/repo/actions/workflows/build.yaml/badge.svg)](...)
[![Visual Studio Marketplace Version](https://img.shields.io/visual-studio-marketplace/v/Publisher.ExtensionName)][marketplace]
[![Visual Studio Marketplace Downloads](https://img.shields.io/visual-studio-marketplace/d/Publisher.ExtensionName)][marketplace]

Download this extension from the [Visual Studio Marketplace][marketplace]
or get the [CI build](http://vsixgallery.com/extension/ExtensionId/).

--------------------------------------

**Hook line that sells the extension in one sentence.**

![Screenshot](art/screenshot.png)

## Features

### Feature 1
Description with screenshot...

## How to Use
...

## License
[Apache 2.0](LICENSE)
```
###自述最佳实践

|元素|指南||---------|-----------|
| **Title** |使用与vsixmanifest |中的`DisplayName`相同的名称
| **勾线** |粗体，在徽章|后紧接一句话价值主张
| **截图** |放置在`/art`文件夹中，使用相对路径（`art/image.png`） |
| **图像大小** |保持在1MB， 800-1200px宽，清晰度|
| **徽章** |版本，下载，评级，构建状态|
| **功能部分** |使用H3 （`###`）与每个主要功能|的截图
| **键盘快捷键** |格式为**Ctrl+M， Ctrl+C**（粗体）|
| **表** |非常适合比较选项或列出功能|
| **链接** |在顶部使用参考风格的链接，以便更清晰地降价|

VSIX Manifest （source.extension.vsixmanifest）```xml
<Metadata>
  <Identity Id="ExtensionName.guid-here" Version="1.0.0" Language="en-US" Publisher="Your Name" />
  <DisplayName>Extension Name</DisplayName>
  <Description xml:space="preserve">Short, compelling description under 200 chars. This appears in search results and the extension tile.</Description>
  <MoreInfo>https://github.com/user/repo</MoreInfo>
  <License>Resources\LICENSE.txt</License>
  <Icon>Resources\Icon.png</Icon>
  <PreviewImage>Resources\Preview.png</PreviewImage>
  <Tags>keyword1, keyword2, keyword3</Tags>
</Metadata>
```
展示最佳实践

|元素|指南||---------|-----------|
| **DisplayName** | 3-5个字，没有“for Visual Studio”（隐含）|
| **描述** | 200字以下，重点是价值而不是功能。出现在搜索tile |中
| **标签** | 5-10个相关关键词，逗号分隔，有助于|的可发现性
| **图标** | 128x128或256x256 PNG，简单的设计可见的小尺寸|
| **PreviewImage** | 200x200 PNG，可以与图标或功能截图|相同
| **MoreInfo** |链接到GitHub回购的文档和问题|

###写作技巧1. **以好处为主，而不是功能** -“不要纠结于XML注释”胜过“XML注释格式化器”
2. **展示，不要告诉**——截图比描述更有说服力
3. **使用一致的术语** -在README， manifest和UI之间匹配术语
4. **保持描述的可扫描性** -简短的段落，项目符号，表格
5. **包括键盘快捷键** -用户喜欢生产力提示
6. **添加“为什么”部分** -在解决方案之前解释问题