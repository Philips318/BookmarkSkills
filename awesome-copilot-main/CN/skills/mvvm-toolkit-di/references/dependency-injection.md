#依赖注入

MVVM工具箱故意不提供它自己的DI容器
与`Microsoft.Extensions.DependencyInjection`集成，相同
ASP使用的容器。. NET核心、Worker服务和。. NET通用主机。

> **默认为构造函数注入。**解决服务和子节点
b> ViewModels通过需要它们的类型的构造函数。避免
用户代码中的>服务定位器模式（`Ioc.Default.GetService<T>()`）。

---

推荐组合根（通用主机）```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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

-通过`Microsoft.Extensions.Configuration`绑定`appsettings.json`配置
-内置日志通过`Microsoft.Extensions.Logging`—托管服务（`IHostedService`）用于后台工作
-开发构建中的范围验证

>在WPF和Windows窗体上，将主机生命周期与
>应用程序生命周期-参见
>[使用。]. NET通用主机在WPF应用程序中]（https://learn.microsoft.com/en-us/dotnet/desktop/wpf/app-development/how-to-use-host-builder）。

---

##组合根（没有通用主机）

当您不需要configuration/logging/hosting时，构建提供程序
直接:```csharp
public partial class App : Application
{
    public IServiceProvider Services { get; }

    public App()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IFilesService, FilesService>();
        services.AddTransient<ContactViewModel>();
        Services = services.BuildServiceProvider();
    }

    public static T GetService<T>() where T : class =>
        ((App)Current).Services.GetRequiredService<T>();
}
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

-依赖关系在调用站点是显式的和可见的。
-单元测试注入fakes/mocks而不诉诸运行时技巧。
DI容器在启动时验证依赖关系图
（在dev中使用`BuildServiceProvider(validateScopes: true)`）。
-没有隐藏的运行时失败-丢失注册立即抛出。

---

# #一生

|寿命|方法|典型用于XAML应用||----------|--------|--------------------------|
|单例|`AddSingleton<T>`|Shell/main-window虚拟机、设置、file/HTTP服务、共享`IMessenger`、全应用缓存|
|暂态|`AddTransient<T>`|每个页面或每个文档的视图模型（每次解析一个新的实例）|
|作用域|`AddScoped<T>`|在客户端应用中很少需要；当您创建显式的`IServiceScope`s（每个窗口作用域，嵌入式HTTP的每个请求作用域）|时非常有用```csharp
services.AddSingleton<ShellViewModel>();      // 1 instance for app lifetime
services.AddTransient<NoteViewModel>();        // new instance per resolve
services.AddScoped<DialogService>();           // 1 per scope (rare)
```
---

在视图中解析

在代码隐藏中解析页面的根ViewModel，然后让它拉
它自己的依赖项：```csharp
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
来自DI的ViewModel。避免手动创建ViewModels。

---

##`IMessenger`注册

该工具包提供了两个实现。注册一次你想要的，
然后到处注射`IMessenger`：```csharp
services.AddSingleton<IMessenger>(WeakReferenceMessenger.Default);
// or
services.AddSingleton<IMessenger>(StrongReferenceMessenger.Default);
```
然后:```csharp
public sealed partial class MyViewModel(IMessenger messenger)
    : ObservableRecipient(messenger) { }
```
多个信使（例如，每个窗口一个）也是有效的-注册它们
使用关键服务或作为作用域实例。

---

关键服务（.）净8 +)

当您有相同接口的多个实现时非常有用
想要按键选择一个：```csharp
services.AddKeyedSingleton<IExporter, CsvExporter>("csv");
services.AddKeyedSingleton<IExporter, JsonExporter>("json");

public sealed partial class ExportViewModel(
    [FromKeyedServices("csv")] IExporter csvExporter,
    [FromKeyedServices("json")] IExporter jsonExporter)
    : ObservableObject
{ /* ... */ }
```
---

##测试接缝

在测试中交换构造器注入的依赖关系是微不足道的。与`Moq`（或`NSubstitute`/`FakeItEasy`）：```csharp
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
如果您发现自己需要模拟`Ioc.Default`或静态状态，那么
ViewModel正在使用服务定位器——重构为构造函数注入
代替。

---

## Legacy:`Ioc.Default`该工具包为案例提供了`CommunityToolkit.Mvvm.DependencyInjection.Ioc`其中构造函数注入是不可能的(例如，一个xml实例化
用于设计时数据的ViewModel，`ValueConverter`，控件模板)。

设置:```csharp
Ioc.Default.ConfigureServices(
    new ServiceCollection()
        .AddSingleton<IFilesService, FilesService>()
        .AddTransient<ContactViewModel>()
        .BuildServiceProvider());
```
解决:```csharp
var files = Ioc.Default.GetRequiredService<IFilesService>();
```
就把这当成逃生口吧。在ViewModels， services和任何
类可以通过DI传递，更倾向于构造函数注入。

---

常见错误1. **通过`Ioc`.**从ViewModel构造函数内部解析子节点
隐藏依赖项。类注入子VM（或工厂）
构造函数。
2. **将所有东西注册为单例。**“每个文档”的ViewModel
注册为单例成为跨所有文档的共享状态——a
微妙的数据损坏bug。对每个实例的虚拟机使用`AddTransient`。
3. **构建多个`ServiceProvider`实例。* *每个`BuildServiceProvider()`是一个新的容器-单例不是
共享。在启动时构建一次，然后重用。
4. **在长寿命对象中捕获`IServiceProvider`本身
指示服务定位器模式。注入特定的依赖项
你所需要的。
5. **忘记在开发中连接范围验证。* *使用`Host.CreateDefaultBuilder()`(设置`ValidateScopes`和`ValidateOnBuild`在开发中)所以注册错误失败
启动，而不是第一次使用。