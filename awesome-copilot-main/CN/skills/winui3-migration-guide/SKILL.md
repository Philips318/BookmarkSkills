---
name: winui3-migration-guide
description: 'UWP-to-WinUI 3 migration reference. Maps legacy UWP APIs to correct Windows App SDK equivalents with before/after code snippets. Covers namespace changes, threading (CoreDispatcher to DispatcherQueue), windowing (CoreWindow to AppWindow), dialogs, pickers, sharing, printing, background tasks, and the most common Copilot code generation mistakes.'
---
# WinUI 3迁移指南

在将UWP应用程序迁移到WinUI 3 / Windows应用程序SDK时，或者在验证生成的代码使用正确的WinUI 3 api而不是传统的UWP模式时，使用此技能。

---

##命名空间更改

所有`Windows.UI.Xaml.*`名称空间移动到`Microsoft.UI.Xaml.*`：

| UWP命名空间| WinUI 3命名空间||--------------|-------------------|
|`Windows.UI.Xaml`|`Microsoft.UI.Xaml`|
|`Windows.UI.Xaml.Controls`|`Microsoft.UI.Xaml.Controls`|
|`Windows.UI.Xaml.Media`|`Microsoft.UI.Xaml.Media`|
|`Windows.UI.Xaml.Input`|`Microsoft.UI.Xaml.Input`|
|`Windows.UI.Xaml.Data`|`Microsoft.UI.Xaml.Data`|
|`Windows.UI.Xaml.Navigation`|`Microsoft.UI.Xaml.Navigation`|
|`Windows.UI.Xaml.Shapes`|`Microsoft.UI.Xaml.Shapes`|
|`Windows.UI.Composition`|`Microsoft.UI.Composition`|
|`Windows.UI.Input`|`Microsoft.UI.Input`|
|`Windows.UI.Colors`|`Microsoft.UI.Colors`|
|`Windows.UI.Text`|`Microsoft.UI.Text`|
|`Windows.UI.Core`|`Microsoft.UI.Dispatching`（调度器）|

---

##副驾驶最常见的3个错误

# # # 1。没有XamlRoot的ContentDialog```csharp
// ❌ WRONG — Throws InvalidOperationException in WinUI 3
var dialog = new ContentDialog
{
    Title = "Error",
    Content = "Something went wrong.",
    CloseButtonText = "OK"
};
await dialog.ShowAsync();
```

```csharp
// ✅ CORRECT — Set XamlRoot before showing
var dialog = new ContentDialog
{
    Title = "Error",
    Content = "Something went wrong.",
    CloseButtonText = "OK",
    XamlRoot = this.Content.XamlRoot  // Required in WinUI 3
};
await dialog.ShowAsync();
```
# # # 2。MessageDialog而不是ContentDialog```csharp
// ❌ WRONG — UWP API, not available in WinUI 3 desktop
var dialog = new Windows.UI.Popups.MessageDialog("Are you sure?", "Confirm");
await dialog.ShowAsync();
```

```csharp
// ✅ CORRECT — Use ContentDialog
var dialog = new ContentDialog
{
    Title = "Confirm",
    Content = "Are you sure?",
    PrimaryButtonText = "Yes",
    CloseButtonText = "No",
    XamlRoot = this.Content.XamlRoot
};
var result = await dialog.ShowAsync();
if (result == ContentDialogResult.Primary)
{
    // User confirmed
}
```
# # # 3。CoreDispatcher代替DispatcherQueue```csharp
// ❌ WRONG — CoreDispatcher does not exist in WinUI 3
await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
{
    StatusText.Text = "Done";
});
```

```csharp
// ✅ CORRECT — Use DispatcherQueue
DispatcherQueue.TryEnqueue(() =>
{
    StatusText.Text = "Done";
});

// With priority:
DispatcherQueue.TryEnqueue(DispatcherQueuePriority.High, () =>
{
    ProgressBar.Value = 100;
});
```
---

##窗口迁移

窗口引用```csharp
// ❌ WRONG — Window.Current does not exist in WinUI 3
var currentWindow = Window.Current;
```

```csharp
// ✅ CORRECT — Use a static property in App
public partial class App : Application
{
    public static Window MainWindow { get; private set; }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        MainWindow = new MainWindow();
        MainWindow.Activate();
    }
}
// Access anywhere: App.MainWindow
```
窗口管理

| UWP API | WinUI 3 API ||---------|-------------|
|`ApplicationView.TryResizeView()`|`AppWindow.Resize()`|
|`AppWindow.TryCreateAsync()`|`AppWindow.Create()`|
|`AppWindow.TryShowAsync()`|`AppWindow.Show()`|
|`AppWindow.TryConsolidateAsync()`|`AppWindow.Destroy()`|
|`AppWindow.RequestMoveXxx()`|`AppWindow.Move()`|
|`AppWindow.GetPlacement()`|`AppWindow.Position`属性|
|`AppWindow.RequestPresentation()`|`AppWindow.SetPresenter()`|

标题栏

| UWP API | WinUI 3 API ||---------|-------------|
|`CoreApplicationViewTitleBar`|`AppWindowTitleBar`|
|`CoreApplicationView.TitleBar.ExtendViewIntoTitleBar`|`AppWindow.TitleBar.ExtendsContentIntoTitleBar`|

---

对话框和选择器迁移

###File/Folder```csharp
// ❌ WRONG — UWP style, no window handle
var picker = new FileOpenPicker();
picker.FileTypeFilter.Add(".txt");
var file = await picker.PickSingleFileAsync();
```

```csharp
// ✅ CORRECT — Initialize with window handle
var picker = new FileOpenPicker();
var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow);
WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);
picker.FileTypeFilter.Add(".txt");
var file = await picker.PickSingleFileAsync();
```
##线程迁移

| UWP模式| WinUI 3等效||-------------|-------------------|
|`CoreDispatcher.RunAsync(priority, callback)`|`DispatcherQueue.TryEnqueue(priority, callback)`|
|`Dispatcher.HasThreadAccess`|`DispatcherQueue.HasThreadAccess`|
|`CoreDispatcher.ProcessEvents()`|无等效-重构异步代码|
|`CoreWindow.GetForCurrentThread()`|不可用-使用`DispatcherQueue.GetForCurrentThread()`|

**关键区别**:UWP使用内置重入阻止的ASTA （Application STA）。WinUI 3使用没有此保护的标准STA。注意异步代码抽取消息时的可重入性问题。

---

后台任务迁移```csharp
// ❌ WRONG — UWP IBackgroundTask
public sealed class MyTask : IBackgroundTask
{
    public void Run(IBackgroundTaskInstance taskInstance) { }
}
```

```csharp
// ✅ CORRECT — Windows App SDK AppLifecycle
using Microsoft.Windows.AppLifecycle;

// Register for activation
var args = AppInstance.GetCurrent().GetActivatedEventArgs();
if (args.Kind == ExtendedActivationKind.AppNotification)
{
    // Handle background activation
}
```
---

## App Settings Migration

|场景|打包应用|未打包应用||----------|-------------|----------------|
|简单设置|`ApplicationData.Current.LocalSettings`| JSON文件在`LocalApplicationData`|
|本地文件存储|`ApplicationData.Current.LocalFolder`|`Environment.GetFolderPath(SpecialFolder.LocalApplicationData)`|

---

## GetForCurrentView（）替换

所有`GetForCurrentView()`模式在WinUI 3桌面应用程序中不可用：

| UWP API | WinUI 3替换||---------|-------------------|
|`UIViewSettings.GetForCurrentView()`|使用`AppWindow`properties |
|`ApplicationView.GetForCurrentView()`|`AppWindow.GetFromWindowId(windowId)`|
|`DisplayInformation.GetForCurrentView()`| Win32`GetDpiForWindow()`或`XamlRoot.RasterizationScale`|
|`CoreApplication.GetCurrentView()`|不可用-手动跟踪窗口|
|`SystemNavigationManager.GetForCurrentView()`|直接处理`NavigationView`的回导航|

---

##测试迁移

UWP单元测试项目不能在WinUI 3中工作。您必须迁移到WinUI 3测试项目模板。

| UWP | WinUI 3 ||-----|---------|
|单元测试应用程序（通用Windows） | **单元测试应用程序（WinUI桌面）** |
|必须使用WinUI测试应用程序Xaml运行时|
|`[TestMethod]`用于所有测试|`[TestMethod]`用于逻辑，`[UITestMethod]`用于XAML/UI测试|
|类库（通用Windows） | **类库（WinUI在桌面）** |```csharp
// ✅ WinUI 3 unit test — use [UITestMethod] for any XAML interaction
[UITestMethod]
public void TestMyControl()
{
    var control = new MyLibrary.MyUserControl();
    Assert.AreEqual(expected, control.MyProperty);
}
```
关键字：**`[UITestMethod]`属性告诉测试运行器在XAML UI线程上执行测试，这是实例化任何`Microsoft.UI.Xaml`类型所必需的。

---

##迁移清单1. []用`Microsoft.UI.Xaml.*`替换所有使用指令的`Windows.UI.Xaml.*`2. []将`Windows.UI.Colors`替换为`Microsoft.UI.Colors`3. []将`CoreDispatcher.RunAsync`替换为`DispatcherQueue.TryEnqueue`4. []将`Window.Current`替换为`App.MainWindow`静态属性
5. []将`XamlRoot`添加到所有`ContentDialog`实例中
6. []用`InitializeWithWindow.Initialize(picker, hwnd)`初始化所有的拾取器
7. []用`ContentDialog`代替`MessageDialog`8. []将`ApplicationView`/`CoreWindow`替换为`AppWindow`9. []用`AppWindowTitleBar`代替`CoreApplicationViewTitleBar`10. []将所有`GetForCurrentView()`调用替换为等同的`AppWindow`11. []更新共享和打印管理器的互操作
12. []将`IBackgroundTask`替换为`AppLifecycle`激活
13. []更新项目文件：TFM到`net10.0-windows10.0.22621.0`，添加`<UseWinUI>true</UseWinUI>`14. []将单元测试迁移到**单元测试App (WinUI in Desktop)**项目；使用`[UITestMethod]`进行XAML测试
15. []测试打包和未打包的配置