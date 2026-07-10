---
name: mvvm-toolkit-di
description: 'Wire CommunityToolkit.Mvvm ViewModels into Microsoft.Extensions.DependencyInjection. Covers the .NET Generic Host composition root, constructor injection, service lifetimes (Singleton / Transient / Scoped), IMessenger registration, resolving ViewModels in Views, keyed services, testing seams, and the legacy Ioc.Default escape hatch. Use across WPF, WinUI 3, .NET MAUI, Uno, and Avalonia.'
---
# CommunityToolkit。Mvvm +`Microsoft.Extensions.DependencyInjection`MVVM工具箱故意提供没有DI容器——它与之组合`Microsoft.Extensions.DependencyInjection`，相同的容器ASP。网
核心服务、Worker服务和。. NET通用主机使用。

> * * TL,博士。**在启动时一次性构建服务提供商（首选）
>`Host.CreateDefaultBuilder()`)。注册服务和视图模型。
>通过构造函数注入。避免`Ioc.Default.GetService<T>()`用户代码中的>。

---

何时使用此技能

-为一个新的XAML应用程序(WPF, WinUI 3，
毛伊岛、乌诺岛、阿瓦洛尼亚)
-选择service/VM寿命
-连接`IMessenger`一次，注入`ObservableRecipient`视图模型
解析页面的ViewModel而不耦合到服务定位器
-诊断“无法解析类型X的服务，同时试图。
激活Y”

有关源代码生成器和ViewModel模式，请参阅**`mvvm-toolkit`**
技能。信使pub/sub见**`mvvm-toolkit-messenger`**。

---推荐组合根（通用主机）```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using CommunityToolkit.Mvvm.Messaging;

public partial class App : Application
{
    public IHost Host { get; }

    public App()
    {
        Host = Microsoft.Extensions.Hosting.Host
            .CreateDefaultBuilder()
            .ConfigureServices((_, services) =>
            {
                services.AddSingleton<IFilesService, FilesService>();
                services.AddSingleton<ISettingsService, SettingsService>();
                services.AddSingleton<IMessenger>(WeakReferenceMessenger.Default);

                services.AddSingleton<ShellViewModel>();
                services.AddTransient<ContactViewModel>();
                services.AddTransient<EditorViewModel>();
            })
            .Build();
    }

    public static T GetService<T>() where T : class =>
        ((App)Current).Host.Services.GetRequiredService<T>();
}
```
通用主机的优点：

-通过`Microsoft.Extensions.Configuration`绑定`appsettings.json`-登录通过`Microsoft.Extensions.Logging`—托管服务（`IHostedService`）用于后台工作
-开发构建中的范围验证

WPF和Windows Forms必须将主机生命周期与应用程序集成
> lifetime -看
>[使用。]. NET通用主机在WPF应用程序中]（https://learn.microsoft.com/en-us/dotnet/desktop/wpf/app-development/how-to-use-host-builder）。

###没有通用主机

当你只需要一个服务容器，并且不需要额外的依赖时：```csharp
var services = new ServiceCollection();
services.AddSingleton<IFilesService, FilesService>();
services.AddTransient<ContactViewModel>();
ServiceProvider provider = services.BuildServiceProvider();
```
---

构造函数注入

通过构造函数注入服务和子视图模型：```csharp
public sealed partial class ContactViewModel(
    IFilesService files,
    IMessenger messenger,
    ILogger<ContactViewModel> logger)
    : ObservableRecipient(messenger)
{
    [ObservableProperty]
    private string? name;

    [RelayCommand]
    private async Task SaveAsync()
    {
        logger.LogInformation("Saving {Name}", Name);
        await files.SaveAsync(Name!);
    }
}
```
为什么构造函数注入胜过服务定位器：

-依赖关系在调用站点是显式的和可见的
—单元测试直接注入fakes/mocksDI容器在启动时验证依赖关系图
-丢失的注册立即扔掉，而不是第一次使用

---

# #一生

|寿命|方法|典型用于XAML应用||----------|--------|--------------------------|
|单例|`AddSingleton<T>`|Shell/main-window虚拟机、设置、file/HTTP服务、共享`IMessenger`、全应用缓存|
|暂态|`AddTransient<T>`|每个页面或每个文档的视图模型（每次解析一个新的实例）|
|作用域|`AddScoped<T>`|在客户端应用中很少需要；可用于显式`IServiceScope`（例如，每个窗口范围）|```csharp
services.AddSingleton<ShellViewModel>();   // 1 instance for app lifetime
services.AddTransient<NoteViewModel>();    // new instance per resolve
services.AddScoped<DialogService>();       // 1 per scope (rare)
```
---

在视图中解析

在代码隐藏中解析页面的根ViewModel，然后让它拉出它的
自己的依赖关系:```csharp
public sealed partial class ContactPage : Page
{
    public ContactViewModel ViewModel { get; }

    public ContactPage()
    {
        ViewModel = App.GetService<ContactViewModel>();
        InitializeComponent();
    }
}
```
在XAML中使用`{x:Bind ViewModel.Xxx}`（编译绑定）或`{Binding Xxx}`对`DataContext`。

对于导航框架(WinUI 3`Frame.Navigate`, MAUI Shell, Prism，
MVVMCross)，让框架解析页面，页面解析它的
来自DI的ViewModel。不要手动`new`ViewModels。

---

##`IMessenger`注册

注册你想要的信使一次，注入`IMessenger`无处不在：```csharp
services.AddSingleton<IMessenger>(WeakReferenceMessenger.Default);
// or
services.AddSingleton<IMessenger>(StrongReferenceMessenger.Default);
```
然后:```csharp
public sealed partial class MyViewModel(IMessenger messenger)
    : ObservableRecipient(messenger) { }
```
对于每个窗口的信使，使用关键服务或作为作用域进行注册
实例并注入到每个窗口的viewmodel中。

有关信使表面积，请参见**`mvvm-toolkit-messenger`**技能。

---

关键服务（.）净8 +)

按键解析同一接口的不同实现：```csharp
services.AddKeyedSingleton<IExporter, CsvExporter>("csv");
services.AddKeyedSingleton<IExporter, JsonExporter>("json");

public sealed partial class ExportViewModel(
    [FromKeyedServices("csv")] IExporter csvExporter,
    [FromKeyedServices("json")] IExporter jsonExporter)
    : ObservableObject { /* ... */ }
```
---

##测试接缝

在测试中交换构造器注入的依赖关系是微不足道的。与`Moq`:```csharp
[Fact]
public async Task Save_calls_files_service()
{
    var files = new Mock<IFilesService>();
    var messenger = new WeakReferenceMessenger();
    var logger = NullLogger<ContactViewModel>.Instance;

    var vm = new ContactViewModel(files.Object, messenger, logger)
    {
        Name = "Ada"
    };

    await vm.SaveCommand.ExecuteAsync(null);

    files.Verify(f => f.SaveAsync("Ada"), Times.Once);
}
```
如果您正在模拟`Ioc.Default`或静态状态，则ViewModel正在使用
服务定位器——重构为构造函数注入。

---

## Legacy:`Ioc.Default``CommunityToolkit.Mvvm.DependencyInjection.Ioc`是一个逃生口
不可能注入构造函数的情况- xaml实例化的vm
对于设计时数据，`ValueConverter`s，控制模板。```csharp
Ioc.Default.ConfigureServices(
    new ServiceCollection()
        .AddSingleton<IFilesService, FilesService>()
        .AddTransient<ContactViewModel>()
        .BuildServiceProvider());

var files = Ioc.Default.GetRequiredService<IFilesService>();
```
把它当作最后的手段。在ViewModels， services和任何类中
DI容器可以构造，更倾向于构造函数注入。

---

##常见陷阱1. **`Ioc.Default.GetService<T>()`内部的虚拟机构造器。**隐藏
依赖，破坏单元测试，阻止启动图验证。
2. * *`Singleton`一切。**注册为单例的“按文档”虚拟机
成为所有文档之间的共享状态——微妙的数据损坏。
对于每个实例的虚拟机，使用`AddTransient`。
3. **多个`BuildServiceProvider()`调用。**每个呼叫都是一个新的
容器——单例不被共享。在启动时构建一次。
4. **捕获长寿命对象中的`IServiceProvider`。**表示a
然后服务模式。注入您需要的特定依赖项。
5. **在开发过程中没有范围验证。**使用`Host.CreateDefaultBuilder()`（将`ValidateScopes`和`ValidateOnBuild`设置为开发状态）
注册错误在启动时失败，而不是在第一次使用时失败。
6. **从根提供商解析作用域服务。* *他们
有效地提升为单例生命周期-警告是沉默的
没有范围验证。要么改变生活时间或解决
显式的`IServiceScope`。---

# #引用

|主题|文件||-------|------|
| [`references/dependency-injection.md`](references/dependency-injection.md) |全面深入（通用主机设置，生命周期，关键服务，测试模式，遗留Ioc）

外部:

—DI概述：<https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection>—DI使用率：<https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection-usage>—MVVM Toolkit Ioc页面：<https://learn.microsoft.com/en-us/dotnet/communitytoolkit/mvvm/ioc>—通用主机：<https://learn.microsoft.com/en-us/dotnet/core/extensions/generic-host>