---
description: 'WinUI 3 and Windows App SDK coding guidelines. Prevents common UWP API misuse, enforces correct XAML namespaces, threading, windowing, and MVVM patterns for desktop Windows apps.'
applyTo: '**/*.xaml, **/*.cs, **/*.csproj'
---
# WinUI 3 / Windows App SDK

关键规则-永远不要使用旧的UWP api

这些UWP模式对于WinUI 3桌面应用来说是错误的。始终使用Windows应用程序SDK等效。- **永远不要**使用`Windows.UI.Popups.MessageDialog`。使用`ContentDialog`和`XamlRoot`集合。
- **永远不要**显示一个不先设置`dialog.XamlRoot = this.Content.XamlRoot`的`ContentDialog`。
- **永远不要**使用`CoreDispatcher.RunAsync`或`Dispatcher.RunAsync`。使用`DispatcherQueue.TryEnqueue`。
- **永远不要**使用`Window.Current`。通过静态`App.MainWindow`属性跟踪主窗口。
- **永远不要**使用`Windows.UI.Xaml.*`命名空间。使用`Microsoft.UI.Xaml.*`。
- **永远不要**使用`Windows.UI.Composition`。使用`Microsoft.UI.Composition`。
- **永远不要**使用`Windows.UI.Colors`。使用`Microsoft.UI.Colors`。
- **永远不要**使用`ApplicationView`或`CoreWindow`进行窗口管理。使用`Microsoft.UI.Windowing.AppWindow`。
- **永远不要**使用`CoreApplicationViewTitleBar`。使用`AppWindowTitleBar`。
**永远不要**使用`GetForCurrentView()`模式（例如，`UIViewSettings.GetForCurrentView()`）。这些在桌面WinUI 3中不存在。使用`AppWindow`api代替。
- **永远不要**直接使用UWP`PrintManager`。使用`IPrintManagerInterop`与窗口句柄。
- **永远不要**直接使用`DataTransferManager`进行共享。使用带有窗口句柄的`IDataTransferManagerInterop`。
- **永远不要**使用UWP`IBackgroundTask`。使用`Microsoft.Windows.AppLifecycle`激活。
- **永远不要**使用`WebAuthenticationBroker`。使用`OAuth2Manager`（Windows App SDK 1.7+）。

XAML模式—默认的XAML命名空间映射为`Microsoft.UI.Xaml`，而不是`Windows.UI.Xaml`。
对于编译的、类型安全的、性能更高的绑定，`{x:Bind}`优于`{Binding}`。
-当使用`{x:Bind}`时，在`DataTemplate`元素上设置`x:DataType`-这是模板中编译绑定所必需的。在Page/UserControl上，`x:DataType`启用编译时绑定验证，但如果DataContext没有更改，则不需要严格执行。
—将`Mode=OneWay`用于动态值，`Mode=OneTime`用于静态值，`Mode=TwoWay`仅用于可编辑输入。
-不要绑定静态常量-直接在XAML中设置它们。

# #线程

-使用`DispatcherQueue.TryEnqueue(() => { ... })`从后台线程更新UI。
-`TryEnqueue`返回`bool`，而不是`Task`-它是即发即弃。
—调度前使用`DispatcherQueue.HasThreadAccess`检查线程访问。
—WinUI 3使用标准STA（不是ASTA）。没有内置的可重入保护——小心使用抽取消息的异步代码。

# #窗口-通过`WindowNative.GetWindowHandle`→`Win32Interop.GetWindowIdFromWindow`→`AppWindow.GetFromWindowId`从WinUI 3中获得`AppWindow`。
—使用`AppWindow`调整大小，移动，标题和演示器操作。
—自定义标题栏：使用`AppWindow.TitleBar`属性，而不是`CoreApplicationViewTitleBar`。
-跟踪主窗口为`App.MainWindow`（在`OnLaunched`中设置的静态属性）。

对话框和选择器

- **ContentDialog**：总是在调用`ShowAsync()`之前设置`dialog.XamlRoot = this.Content.XamlRoot`。
**File/FolderPickers**：初始化`WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd)`，其中`hwnd`来自`WindowNative.GetWindowHandle(App.MainWindow)`。
- **Share/Print**：使用带有窗口句柄的COM互操作接口（`IDataTransferManagerInterop`,`IPrintManagerInterop`）。

MVVM和数据绑定

—MVVM基础架构优先选择`CommunityToolkit.Mvvm`（`[ObservableProperty]`,`[RelayCommand]`）。
—使用`Microsoft.Extensions.DependencyInjection`进行服务注册和注入。
保持UI（视图）专注于布局和绑定；将逻辑保存在ViewModels和services中。
-使用`async`/`await`用于I/O和长时间运行的工作，以保持UI响应。

##项目设置-目标`net10.0-windows10.0.22621.0`（或项目目标SDK的适当TFM）。
—在工程文件中设置“`<UseWinUI>true</UseWinUI>`”。
—参考最新的稳定版`Microsoft.WindowsAppSDK`NuGet包。
-使用`System.Text.Json`与JSON序列化源生成器。

c#代码风格

-使用文件作用域的命名空间。
—启用可空引用类型。使用`is null`/`is not null`代替`== null`。
-优先选择模式匹配超过`as`/`is`与空检查。
类型，方法，属性的PascalCase。camelCase用于私人领域。
- Allman大括号样式（在自己的行上开大括号）。
-内置类型首选显式类型；只有当类型很明显时才使用`var`。

# #可访问性

-在所有交互控件上设置`AutomationProperties.Name`。
-在section头上使用`AutomationProperties.HeadingLevel`。
-隐藏装饰元素`AutomationProperties.AccessibilityView="Raw"`。
—确保全键盘导航（Tab、Enter、空格、方向键）。
-满足WCAG颜色对比度要求。

# #性能-首选`{x:Bind}`（编译）而不是`{Binding}`（基于反射）。
- **NativeAOT:**在NativeAOT编译下，`{Binding}`（基于反射）根本不起作用。只支持`{x:Bind}`（编译绑定）。如果项目使用NativeAOT，则只使用`{x:Bind}`。
-使用`x:Load`或`x:DeferLoadStrategy`的UI元素，不是立即需要。
-使用`ItemsRepeater`与虚拟化大列表。
-避免深度布局嵌套-首选`Grid`而不是嵌套的`StackPanel`链。
-所有I/O使用`async`/`await`；永远不要阻塞UI线程。

应用程序设置（已打包与未打包）

- **打包应用**:`ApplicationData.Current.LocalSettings`工作如预期。
- **未打包的应用**：使用自定义设置文件（例如，JSON在`Environment.GetFolderPath(SpecialFolder.LocalApplicationData)`）。
-不要假设`ApplicationData`总是可用的-首先检查包装状态。

# #排版**总是**使用内置的TextBlock样式（`CaptionTextBlockStyle`,`BodyTextBlockStyle`,`BodyStrongTextBlockStyle`,`SubtitleTextBlockStyle`,`TitleTextBlockStyle`,`TitleLargeTextBlockStyle`,`DisplayTextBlockStyle`）。
-更喜欢使用内置的TextBlock风格硬编码`FontSize`，`FontWeight`，或`FontFamily`。
-字体：Segoe UI变量是默认的-不要改变它。
-对所有UI文本使用句子大小写。


主题和颜色

- **总是**使用`{ThemeResource}`笔刷和颜色自动支持光，暗，高对比度主题。
- **永远不要**硬编码UI元素的颜色值（`#FFFFFF`，`Colors.White`等）。使用主题资源，如`TextFillColorPrimaryBrush`，`CardBackgroundFillColorDefaultBrush`,`CardStrokeColorDefaultBrush`。
-使用`SystemAccentColor`（和`Light1`-`Light3`，`Dark1`-`Dark3`变体）为用户的口音调色板。
—边框：使用`CardStrokeColorDefaultBrush`或`ControlStrokeColorDefaultBrush`。

##间距和布局-使用**4px网格系统**：所有边距，填充和间距值必须是4px的倍数。
-标准间距：4（紧凑），8（控制），12（小槽），16（内容填充），24（大槽）。
-在性能方面，`Grid`优于深度嵌套的`StackPanel`链。
-使用`Auto`内容大小的rows/columns，`*`比例大小。避免固定像素大小。
-使用`VisualStateManager`与`AdaptiveTrigger`响应布局在断点（640px, 1008px）。
-使用`ControlCornerRadius`（4px）的小控件和`OverlayCornerRadius`（8px）的卡片，对话框，弹出框。

##材料和立面-使用**Mica** (`MicaBackdrop`)为应用程序窗口背景。需要上面的透明层来显示。
-使用**亚克力**仅用于瞬态表面（飞屏，菜单，导航窗格）。
-使用`LayerFillColorDefaultBrush`的内容层之上的云母。
-使用`ThemeShadow`与z轴`Translation`标高。卡片：4-8像素，弹出框：32像素，对话框：128像素。

##运动和过渡

-使用内置的主题过渡（`EntranceThemeTransition`,`RepositionThemeTransition`,`ContentThemeTransition`,`AddDeleteThemeTransition`）。
-当存在内置过渡时，避免自定义故事板动画。

##控制选择-使用`NavigationView`为主应用程序导航（不是自定义侧边栏）。
-使用`InfoBar`持久的应用内通知（不是自定义横幅）。
-使用`TeachingTip`上下文指导（不是自定义弹出窗口）。
-使用`NumberBox`用于数字输入（不是手动验证的文本框）。
—on/off设置使用`ToggleSwitch`（而不是CheckBox）。
-使用`ItemsView`作为现代收集控件，用于显示具有内置选择，虚拟化和布局灵活性的数据。
-使用`ListView`/`GridView`用于标准的虚拟化列表和网格，特别是当需要内置选择支持时。
-`ItemsRepeater`仅用于完全自定义的虚拟化布局，您需要完全控制渲染，不需要内置选择或交互处理。
-使用`Expander`可折叠部分（不自定义可视性切换）。

##错误处理-总是将`async void`事件处理程序包装在try/catch中，以防止未处理的崩溃。
—使用`InfoBar`（与`Severity = Error`一起）处理面向用户的错误消息，而不是使用`ContentDialog`处理常规错误。
—处理`App.UnhandledException`进行日志记录和安全恢复。

# #测试

- **永远不要**使用普通的MSTest或xUnit项目进行实例化WinUI 3 XAML类型的测试。使用一个**Unit Test App (WinUI in Desktop)**项目，它提供了Xaml运行时和UI线程。
-使用`[TestMethod]`进行纯逻辑测试。对于创建`[UITestMethod]`类型（控件、页面、用户控件）或与之交互的任何测试，都使用`[UITestMethod]`。
-将可测试的业务逻辑放置在**Class Library (WinUI in Desktop)**项目中，与主应用程序分开。
-在运行测试之前构建解决方案，以启用Visual Studio测试发现。

资源和本地化-在`Resources.resw`文件中存储面向用户的字符串，而不是代码或XAML文字。
-在XAML中使用`x:Uid`进行本地化文本绑定。
-使用dpi合格的图像资产（`logo.scale-200.png`）；不带刻度限定符的引用（`ms-appx:///Assets/logo.png`）。