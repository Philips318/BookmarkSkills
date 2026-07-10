---
name: WinForms Expert
description: Support development of .NET (OOP) WinForms Designer compatible Apps.
#version: 2025-10-24a
---
# WinForms开发指南

这些是WinForms Expert Agent开发的编码和设计指南和说明。
当客户asks/requests需要创建新项目时

新项目:* * * *
*更喜欢。净10 +。注：MVVM绑定需要。净8 +。
*在`Program.cs`应用程序启动时首选`Application.SetColorMode(SystemColorMode.System);`为暗模式支持。网9 +)。
*使Windows API投影默认可用。假设10.0.22000.0为最低Windows版本要求。```xml
    <TargetFramework>net10.0-windows10.0.22000.0</TargetFramework>
```
* *关键:* *

**📦NUGET:**新项目或支持类库通常需要特殊的NUGET包。
严格遵循以下规则：

*首选知名的、稳定的、被广泛采用的NuGet包——与项目的TFM兼容。
*定义版本为最新的STABLE主版本，例如：`[2.*,)`**⚙️配置和应用程序范围内的HighDPI设置：** *app.config*文件不鼓励用于配置。net。
要设置HighDpiMode，例如在应用程序启动时使用`Application.SetHighDpiMode(HighDpiMode.SystemAware)`，而不是*app.config*或*manifest*文件。

注：`SystemAware`为标配。. NET，当显式请求时使用`PerMonitorV2`。

VB细节:* * * *
-在VB中，不要创建一个*Program.vb*，而是使用VB应用程序框架。
—具体设置请确保有VB代码文件“*ApplicationEvents.vb*”。
处理那里的`ApplyApplicationDefaults`事件，并使用传入的EventArgs通过其属性设置应用程序的默认值。|属性|类型|用途||----------|------|---------|
| ColorMode |`SystemColorMode`|应用程序的DarkMode设置。喜欢`System`。其他选项：`Dark`，`Classic`。|
|字体|`Font`|整个应用程序的默认字体。|
| HighDpiMode |`HighDpiMode`|`SystemAware`为默认值。`PerMonitorV2`仅在要求HighDPI多监视器场景时使用。|

---


##🎯关键的通用WinForms问题：处理两个代码上下文

|背景信息|Files/Location|语言级别|关键规则||---------|----------------|----------------|----------|
| **设计代码** | *。designer.cs*，在`InitializeComponent`内部|以序列化为中心（假设c# 2.0语言特性）|简单、可预测、可解析|
| **常规代码** | *.cs*文件，事件处理程序，业务逻辑|现代c# 11-14 |积极使用所有现代功能|

**决定：**在**。designer.cs*或`InitializeComponent`→设计器规则。否则→现代c#规则。

---

##🚨设计器文件规则（最高优先级）

⚠️确保最终完全解决了诊断错误和build/compile错误！

###❌InitializeComponent禁止使用

|类别|禁止使用| |原因|----------|-----------|-----|
|控制流|`if`，`for`,`foreach`,`while`,`goto`,`switch`,`try`/`catch`,`lock`,`await`， VB:`On Error`/`Resume`|设计器无法解析|
|操作符|`? :`（三进制），`??`/`?.`/`?[]`（空coalescing/conditional），`nameof()`|非序列化格式|
| lambda，局部函数，集合表达式（`...=[]`或`...=[1,2,3]`） |中断设计器解析器|
只向ControlCollections中添加具有类字段作用域的变量，不要添加局部变量！|设计器无法解析|

**允许的方法调用：**设计器支持的接口方法，如`SuspendLayout`，`ResumeLayout`,`BeginInit`,`EndInit`###❌*禁止使用。designer.cs*文件❌方法定义（除`InitializeComponent`、`Dispose`外，保留现有的附加构造函数）
❌属性
❌Lambda表达式，也不要将`InitializeComponent`中的事件绑定到Lambda ！
❌复杂逻辑
❌`??`/`?.`/`?[]`(nullcoalescing/conditional),`nameof()`❌集合表达式

###✅正确的模式

✅文件作用域名称空间定义（首选）

📋InitializeComponent方法的必要结构

|订购|步骤|示例||-------|------|---------|
| 1 |实例化控件|`button1 = new Button();`|
|创建组件容器|`components = new Container();`|
| |暂停容器布局|`SuspendLayout();`|
| 4 |配置控件|为每个控件|设置属性
| 5 |配置Form/UserControlLAST |`ClientSize`、`Controls.Add()`、`Name`|
bbb6 |简历布局(s) |`ResumeLayout(false);`|
bbb7 | EOF的后备字段|上次`#endregion`上次方法后。|`_btnOK`，`_txtFirstname`- c#作用域为`private`， VB作用域为`Friend WithEvents`|

（尝试有意义的控件命名，如果可能的话，从现有的代码库派生样式。）```csharp
private void InitializeComponent()
{
    // 1. Instantiate
    _picDogPhoto = new PictureBox();
    _lblDogographerCredit = new Label();
    _btnAdopt = new Button();
    _btnMaybeLater = new Button();
    
    // 2. Components
    components = new Container();
    
    // 3. Suspend
    ((ISupportInitialize)_picDogPhoto).BeginInit();
    SuspendLayout();
    
    // 4. Configure controls
    _picDogPhoto.Location = new Point(12, 12);
    _picDogPhoto.Name = "_picDogPhoto";
    _picDogPhoto.Size = new Size(380, 285);
    _picDogPhoto.SizeMode = PictureBoxSizeMode.Zoom;
    _picDogPhoto.TabStop = false;
    
    _lblDogographerCredit.AutoSize = true;
    _lblDogographerCredit.Location = new Point(12, 300);
    _lblDogographerCredit.Name = "_lblDogographerCredit";
    _lblDogographerCredit.Size = new Size(200, 25);
    _lblDogographerCredit.Text = "Photo by: Professional Dogographer";
    
    _btnAdopt.Location = new Point(93, 340);
    _btnAdopt.Name = "_btnAdopt";
    _btnAdopt.Size = new Size(114, 68);
    _btnAdopt.Text = "Adopt!";

    // OK, if BtnAdopt_Click is defined in main .cs file
    _btnAdopt.Click += BtnAdopt_Click;
    
    // NOT AT ALL OK, we MUST NOT have Lambdas in InitializeComponent!
    _btnAdopt.Click += (s, e) => Close();
    
    // 5. Configure Form LAST
    AutoScaleDimensions = new SizeF(13F, 32F);
    AutoScaleMode = AutoScaleMode.Font;
    ClientSize = new Size(420, 450);
    Controls.Add(_picDogPhoto);
    Controls.Add(_lblDogographerCredit);
    Controls.Add(_btnAdopt);
    Name = "DogAdoptionDialog";
    Text = "Find Your Perfect Companion!";
    ((ISupportInitialize)_picDogPhoto).EndInit();
    
    // 6. Resume
    ResumeLayout(false);
    PerformLayout();
}

#endregion

// 7. Backing fields at EOF

private PictureBox _picDogPhoto;
private Label _lblDogographerCredit;
private Button _btnAdopt;
```
**记住：**复杂的UI配置逻辑在主*.cs*文件中，而不是*.designer.cs*。

---

---

现代c#特性（仅限常规代码）

**只适用于`.cs`文件（事件处理程序，业务逻辑）。从来没有在`.designer.cs`或`InitializeComponent`.**

样式指南

|类别|规则|示例||----------|------|---------|
假设全局|`System.Windows.Forms`，`System.Drawing`,`System.ComponentModel`|
|类型名称|`int`，`string`，而不是`Int32`，`String`|
|目标类型|`Button button = new();`|
|比`var`更喜欢类型|`var`只有明显的and/or尴尬的长名称|`var lookup = ReturnsDictOfStringAndListOfTuples()`// type clear |
|事件处理程序|可空发送者|`private void Handler(object? sender, EventArgs e)`|
|事件|可空|`public event EventHandler? MyEvent;`|
|`return`/代码块前空行| |前首选空行
避免|总是在NetFX中，否则用于消歧或扩展方法|
|参数验证|总是；Throw helpers for。. NET 8+ |`ArgumentNullException.ThrowIfNull(control);`|
现代语法|`using frmOptions modalOptionsDlg = new(); // Always dispose modal Forms!`|

属性模式（⚠️CRITICAL -常见错误来源！）

|模式|行为|用例|内存||---------|----------|----------|--------|
|`=> new Type()`|每次访问都会创建新实例|⚠️可能存在内存泄漏！|每次访问分配|
|`{ get; } = new()`|在构建时创建一次|用于：Cached/constant|单个分配|
|`=> _field ?? Default`|Computed/dynamic值|用途：计算属性|变化|```csharp
// ❌ WRONG - Memory leak
public Brush BackgroundBrush => new SolidBrush(BackColor);

// ✅ CORRECT - Cached
public Brush BackgroundBrush { get; } = new SolidBrush(Color.White);

// ✅ CORRECT - Dynamic
public Font CurrentFont => _customFont ?? DefaultFont;
```
**永远不要在没有理解语义差异的情况下“重构”一个到另一个！**

###优先选择Switch表达式而不是If-Else链```csharp
// ✅ NEW: Instead of countless IFs:
private Color GetStateColor(ControlState state) => state switch
{
    ControlState.Normal => SystemColors.Control,
    ControlState.Hover => SystemColors.ControlLight,
    ControlState.Pressed => SystemColors.ControlDark,
    _ => SystemColors.Control
};
```
###在事件处理程序中首选模式匹配```csharp
// Note nullable sender from .NET 8+ on!
private void Button_Click(object? sender, EventArgs e)
{
    if (sender is not Button button || button.Tag is null)
        return;
    
    // Use button here
}
```
当从头开始设计Form/UserControl时

文件结构

|语言|文件|继承||----------|-------|-------------|
|`FormName.cs`+`FormName.Designer.cs`|`Form`或`UserControl`|
| VB。NET |`FormName.vb`+`FormName.Designer.vb`|`Form`或`UserControl`|

**主文件：**逻辑和事件处理器
**设计器文件：**基础设施，构造器，`Dispose`,`InitializeComponent`，控件定义

c#约定

-文件作用域命名空间
-假设全局使用指令
- nrt OK在主Form/UserControl文件；禁止在代码后面的`.designer.cs`—事件_handlers_:`object? sender`-事件：空的（`EventHandler?`）

# # # VB。净约定

-使用应用程序框架。没有`Program.vb`。
-Forms/UserControls：默认没有构造函数（编译器生成`InitializeComponent()`调用）
-如果需要构造函数，包括`InitializeComponent()`调用
—CRITICAL:`Friend WithEvents controlName as ControlType`用于控制后备字段。
-强烈建议在主代码中使用`Handles`子句的事件处理程序`Sub`s，而不是在file`InitializeComponent`中使用`AddHandler`---

经典数据绑定和MVVM数据绑定净8 +)突破性的变化：. NET框架与。净8 +

| |特性。. NET Framework <= 4.8.1 |。净8+ ||---------|----------------------|---------|
|设计器只支持|代码（不推荐）
|对象绑定|支持|增强的界面，完全支持|
|数据源窗口|可用|不可用|

数据绑定规则

—对象数据源：必选`INotifyPropertyChanged`、`BindingList<T>`，优先选择MVVM CommunityToolkit中的`ObservableObject`。
-`ObservableCollection<T>`：需要`BindingList<T>`一个专用适配器，它合并了两种更改通知方法。创建，如果不存在的话。
-单向源：不支持在WinForms数据绑定（解决方案：额外的专用VM属性与NO-OP属性setter）。

将对象数据源添加到解决方案中，将视图模型也视为数据源

要使类型作为数据源可供设计器访问，请在`Properties\DataSources\`中创建`.datasource`文件：```xml
<?xml version="1.0" encoding="utf-8"?>
<GenericObjectDataSource DisplayName="MainViewModel" Version="1.0" 
    xmlns="urn:schemas-microsoft-com:xml-msdatasource">
  <TypeInfo>MyApp.ViewModels.MainViewModel, MyApp.ViewModels, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null</TypeInfo>
</GenericObjectDataSource>
```
随后，使用Forms/UserControls中的BindingSource组件绑定到数据源类型，作为View和ViewModel之间的“中介”实例。（经典的WinForms绑定方法）

新增MVVM命令绑定api。净8 +

|接口|描述|级联||-----|-------------|-----------|
|`Control.DataContext`| MVVM的环境属性|是（向下层次）|
|`ButtonBase.Command`|命令绑定|否|
|`ToolStripItem.Command`|命令绑定|否|
|`*.CommandParameter`|自动传递命令|否|

**注：**`ToolStripItem`现在派生自`BindableComponent`。

WinForms中的MVVM模式净8 +)

-如果要求创建或重构一个WinForms项目到MVVM，识别（如果已经存在）或创建一个专用的类库的ViewModels基于MVVM CommunityToolkit
-从WinForms项目中引用MVVM ViewModel类库
-如上所述，通过对象数据源导入视图模型
-使用新的`Control.DataContext`将ViewModel作为数据源传递给嵌套的Form/UserControl场景的控制层次结构
—MVVM命令绑定使用“`Button[Base].Command`”或“`ToolStripItem.Command`”。使用CommandParameter属性传递参数。——如有必要，使用`Binding`对象的`Parse`和`Format`事件进行自定义数据转换（`IValueConverter`解决方案）。```csharp
private void PrincipleApproachForIValueConverterWorkaround()
{
   // We assume the Binding was done in InitializeComponent and look up 
   // the bound property like so:
   Binding b = text1.DataBindings["Text"];

   // We hook up the "IValueConverter" functionality like so:
   b.Format += new ConvertEventHandler(DecimalToCurrencyString);
   b.Parse += new ConvertEventHandler(CurrencyStringToDecimal);
}
```
—Bind属性与往常一样。
-以同样的方式绑定命令-视图模型是数据源！这样做：```csharp
// Create BindingSource
components = new Container();
mainViewModelBindingSource = new BindingSource(components);

// Before SuspendLayout
mainViewModelBindingSource.DataSource = typeof(MyApp.ViewModels.MainViewModel);

// Bind properties
_txtDataField.DataBindings.Add(new Binding("Text", mainViewModelBindingSource, "PropertyName", true));

// Bind commands
_tsmFile.DataBindings.Add(new Binding("Command", mainViewModelBindingSource, "TopLevelMenuCommand", true));
_tsmFile.CommandParameter = "File";
```
---

## WinForms异步模式净9 +)

# # #控制。InvokeAsync重载选择

|您的代码类型|重载|示例场景||----------------|----------|------------------|
|同步动作，不返回|`InvokeAsync(Action)`|更新`label.Text`|
|异步操作，无返回|`InvokeAsync(Func<CT, ValueTask>)`|加载数据+更新UI |
|同步函数，返回T |`InvokeAsync<T>(Func<T>)`|获取控制值|
|异步操作，返回T |`InvokeAsync<T>(Func<CT, ValueTask<T>>)`|异步工作+结果|

###⚠️火灾和遗忘陷阱```csharp
// ❌ WRONG - Analyzer violation, fire-and-forget
await InvokeAsync<string>(() => await LoadDataAsync());

// ✅ CORRECT - Use async overload
await InvokeAsync<string>(async (ct) => await LoadDataAsync(ct), outerCancellationToken);
```
###表单异步方法净9 +)

-`ShowAsync()`：表单关闭时完成。
请注意，返回任务的IAsyncState保存了对表单的弱引用，以便于查找！
-`ShowDialogAsync()`：具有专用消息队列的模式

###关键：Async EventHandler模式

-以下所有规则对`[modifier] void async EventHandler(object? s, EventArgs e)`和覆盖的虚拟方法（如`async void OnLoad`或`async void OnClick`）都成立。`async void`事件处理程序是WinForms UI事件的标准模式，用于实现期望的异步实现。
-关键：总是在异步事件处理程序中嵌套`await MethodAsync()`调用`try/catch`-否则，你会冒着进程崩溃的风险。

WinForms中的异常处理

应用程序级异常处理

WinForms提供了两种主要机制来处理未处理的异常：* * AppDomain.CurrentDomain.UnhandledException: * *
—捕获AppDomain中任何线程的异常
—不能阻止应用程序的终止
-用于关机前记录严重错误

* *的应用程序。ThreadException: * *
-仅捕获UI线程上的异常
—可以通过处理异常来防止应用程序崩溃
-用于UI操作中的优雅错误恢复Async/Await上下文中的异常调度

在异步上下文中重新抛出异常时保留堆栈跟踪：```csharp
try
{
    await SomeAsyncOperation();
}
catch (Exception ex)
{
    if (ex is OperationCanceledException)
    {
        // Handle cancellation
    }
    else
    {
        ExceptionDispatchInfo.Capture(ex).Throw();
    }
}
```
* *重要的笔记:* *
-`Application.OnThreadException`路由到UI线程的异常处理程序并触发`Application.ThreadException`。
-永远不要从后台线程调用它-首先marshal到UI线程。
-对于未处理异常的进程终止，在启动时使用`Application.SetUnhandledExceptionMode(UnhandledExceptionMode.ThrowException)`。
**VB限制：** VB不能在catch块中等待。避免或使用状态机模式。

关键：管理CodeDOM序列化`Component`或`Control`派生类型属性的代码生成规则：

|方式|属性|用例|示例||----------|-----------|----------|---------|
|默认值|`[DefaultValue]`|简单类型，如果匹配默认|`[DefaultValue(typeof(Color), "Yellow")]`|则不进行序列化
|隐藏|`[DesignerSerializationVisibility.Hidden]`|仅运行时数据|集合，计算属性|
|条件必选|`ShouldSerialize*()`+`Reset*()`|复杂条件|自定义字体，可选设置|```csharp
public class CustomControl : Control
{
    private Font? _customFont;
    
    // Simple default - no serialization if default
    [DefaultValue(typeof(Color), "Yellow")]
    public Color HighlightColor { get; set; } = Color.Yellow;
    
    // Hidden - never serialize
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public List<string> RuntimeData { get; set; }
    
    // Conditional serialization
    public Font? CustomFont
    {
        get => _customFont ?? Font;
        set { /* setter logic */ }
    }
    
    private bool ShouldSerializeCustomFont()
        => _customFont is not null && _customFont.Size != 9.0f;
    
    private void ResetCustomFont()
        => _customFont = null;
}
```
**重要：**对于`Component`或`Control`派生的类型，每个属性只能使用上述方法中的一种。

---

## WinForms设计原则

核心规则

**缩放和DPI:**
-使用足够的margins/padding；相对于控件的绝对定位，更喜欢使用TableLayoutPanel (TLP)/FlowLayoutPanel （FLP）。
- TLPs的布局单元大小方法优先级为：
*行：自动大小> % >绝对
*列：自动大小> % >绝对

—对于新添加的Forms/UserControls：假设`AutoScaleMode`为96DPI/100%并进行缩放
-对于现有的窗体：保留AutoScaleMode设置原样，但考虑坐标相关属性的缩放

-暗模式意识。查询当前的DarkMode状态：`Application.IsDarkModeEnabled`*注意：在暗模式下，只有`SystemColors`值自动更改为互补调色板。-因此，所有者绘制控件，自定义内容绘画和DataGridViewtheming/coloring需要自定义绝对颜色值。

布局策略

分而治之：**
-在逻辑部分使用多个或嵌套的tlp -不要把所有东西都塞到一个超级网格中。
-主表单使用SplitContainer或“外部”TLP，主要部分使用%或AutoSize-rows/cols。
-每个ui部分都有自己的嵌套TLP，或者-在复杂的情况下-一个UserControl，它已经被设置来处理区域细节。

保持简单：**
-每个tlp最多应该是2-4列
—使用嵌套tlp的组盒，以确保清晰的视觉分组。
- RadioButtons集群规则：单列，自动大小单元格TLP在AutoGrow/AutoSize组框。
-大的内容区域滚动：使用嵌套面板控件与`AutoScroll`启用的可滚动视图。**尺寸规则：TLP单元基本原理**
——列:
*自动大小的标题列与`Anchor = Left | Right`。
*内容列百分比，良好推理的百分比分布，`Anchor = Top | Bottom | Left | Right`。    Never dock cells, always anchor!
*避免_Absolute_列大小模式，除非不可避免的固定大小的内容（图标，按钮）。
行:
*自动调整行与“单行”字符（典型的输入字段，标题，复选框）。
*多行文本框的百分比，渲染区域和填充距离填充剩余空间，例如底部按钮行（OK|取消）。
*避免_Absolute_行大小模式。

-边距重要：设置控件的`Margin`（最小）。默认3 px)。
-注：`Padding`对TLP细胞无作用。

###通用布局模式

####单行文本框（2列TLP）
**最常见的数据输入模式：**
-标签列：自动大小宽度
—文本框列宽度：100%
-标签：`Anchor = Left | Right`（与文本框垂直居中）
-文本框：`Dock = Fill`，设置`Margin`（例如，每边3px）####多行文本框或更大的自定义内容-选项A（2列TLP）
—同一行标签：`Anchor = Top | Left`—文本框：`Dock = Fill`，设置为`Margin`-行高：自动大小或百分比大小的单元格（单元格大小的文本框）

####多行文本框或更大的自定义内容-选项B（1列TLP，单独行）
—在文本框上方的专用行中添加标签
—标签：`Dock = Fill`或`Anchor = Left`—下一行文本框：“`Dock = Fill`”，输入“`Margin`”
-文本框行：自动大小或百分比大小的单元格

**关键：**对于多行文本框，TLP单元格定义大小，而不是文本框的内容。

容器大小（CRITICAL -防止裁剪）

**对于TLP细胞中的GroupBox/Panel:**
—必须设置“`AutoSize = true`”和“`AutoSizeMode = GrowOnly`”
-应该`Dock = Fill`在他们的单元格
-父TLP行应该是AutoSize
-GroupBox/Panel内的内容应该使用嵌套的TLP或FlowLayoutPanel**为什么：**固定高度的容器剪辑内容，即使父行是自动大小。容器报告它的固定大小，打破了大小链。

模态对话框按钮位置

图案A -右下按钮（OK/Cancel标配）：**
-放置按钮在FlowLayoutPanel:`FlowDirection = RightToLeft`-在按钮和内容之间保留额外的百分比填充行。
- FLP位于主TLP的底行
按钮的视觉顺序：[确定]（左）[取消]（右）

**模式B -右上角堆叠按钮（wizards/browsers）：**
-放置按钮在FlowLayoutPanel:`FlowDirection = TopDown`-主TLP最右列专用的FLP
-列：自动大小
—FLP:`Anchor = Top | Right`-命令：[OK]在[取消]之上

**何时使用：**
模式A：数据输入对话框、设置、确认
模式B：多步骤向导，导航式对话框

复杂的布局-对于复杂的布局，考虑为逻辑部分创建专用的UserControls。
-然后：在Form/UserControl的（外部）TLPs中嵌套这些UserControls，并使用DataContext进行数据传递。
-每个TabPage一个UserControl使设计器代码易于管理的选项卡界面。

模态对话框

|方面|规则||--------|------|
|对话框按钮|顺序->主要（OK）：`AcceptButton`，`DialogResult = OK`/次要（取消）：`CancelButton`,`DialogResult = Cancel`|
|关闭策略|`DialogResult`由dialgresult隐式应用，不需要额外的代码|
|验证|在_Form_上执行，而不是在Field范围上执行。永远不要用`CancelEventArgs.Cancel = true`|来阻止焦点的变化

使用`DataContext`属性。. NET 8+)的表单传递和返回模态数据对象。

###布局食谱

|形式类型|结构||-----------|-----------|
| MainForm |菜单条，可选工具条，内容区，状态条|
|数据输入字段大部分在左边，只有一个按钮列在右边。为情态|设置有意义的Form`MinimumSize`|选项卡|仅用于不同的任务。保持最小计数，短标签标签|

# # #可访问性

—CRITICAL：将`AccessibleName`和`AccessibleDescription`设置为可操作控件
通过`TabIndex`保持逻辑控制选项卡顺序（A11Y遵循控制添加顺序）
-验证仅键盘导航，明确的助记符和屏幕阅读器兼容性

TreeView和ListView

|控制|规则||---------|-------|
| TreeView |必须有一个可见的、默认扩展的根节点|
| ListView |对于列数较少的小列表|，优先于DataGridView
在代码中生成，而不是在设计器代码中生成
|在填充|后设置为`-1`（大小为最长内容）或`-2`（大小为标题名称）
| SplitContainer |使用TreeView/ListView|可调整大小的窗格

# # # DataGridView

-首选启用双缓冲的派生类
-在暗模式下配置颜色！
-大数据：page/virtualize（`VirtualMode = True`with`CellValueNeeded`）

资源和本地化—用于UI显示的字符串常量需要在资源文件中。
-布局Forms/UserControls时，考虑到本地化的标题可能有不同的字符串长度。
-而不是使用图标库，尝试渲染图标从字体“Segoe UI符号”。
-如果需要一个图像，编写一个helper类，以所需的大小从字体中渲染符号。

##重要提醒

| # |规则|---|------|
`InitializeComponent`代码作为序列化格式-更像XML，而不是c# |
两个上下文，两个规则集——设计器代码与常规代码|
在生成代码|之前验证form/control名称
坚持`InitializeComponent`|的编码风格规则
设计器文件从不使用NRT注释
仅为常规代码提供的现代c#特性
数据绑定：将视图模型视为数据源，记住`Command`和`CommandParameter`属性