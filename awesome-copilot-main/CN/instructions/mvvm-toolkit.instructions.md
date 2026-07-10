---
description: 'CommunityToolkit.Mvvm (MVVM Toolkit) coding conventions for ViewModels, commands, messaging, validation, and DI across WPF, WinUI 3, .NET MAUI, Uno Platform, and Avalonia.'
applyTo: '**/*.cs, **/*.xaml, **/*.csproj'
---
# CommunityToolkit。Mvvm （Mvvm Toolkit）

每当项目引用`CommunityToolkit.Mvvm`时，这些规则都适用。
要获得深入的参考和端到端示例，请加载`mvvm-toolkit`技能。

##包&语言

—参考`CommunityToolkit.Mvvm`8。`.csproj`中的X（或更新版本）。不
为新项目安装旧的`Microsoft.Toolkit.Mvvm`（7.x）。
c#`LangVersion`必须支持源代码生成器（在现代sdk中默认）。

ViewModel基类

-默认从`ObservableObject`继承ViewModels。
-仅在ViewModel需要时使用`ObservableValidator``INotifyDataErrorInfo`（表单、设置、输入验证）。
-仅当ViewModel发送或接收时使用`ObservableRecipient``IMessenger`消息。
-永远不要手动实现`INotifyPropertyChanged`当一个工具包
可以使用基类。如果类型不能从工具箱基继承
（例如，自定义控件），应用类级别`[ObservableObject]`或`[INotifyPropertyChanged]`属性。

# #属性-将所有使用`[ObservableProperty]`的类型声明为`partial`（和）
每个封闭类型（如果嵌套）。
—将`[ObservableProperty]`应用于名为`name`、`_name`或的私有字段`m_name`-绝不是PascalCase。让生成器发出公共属性。
—请勿编写手动`SetProperty(ref field, value)`样板文件
字段符合`[ObservableProperty]`。
—使用`[NotifyPropertyChangedFor(nameof(Derived))]`提高更改derived/computed属性的通知。
—使用`[NotifyCanExecuteChangedFor(nameof(XxxCommand))]`so命令
当它们的输入改变时，重新计算`CanExecute`。
实现`OnXxxChanging`/`OnXxxChanged`的部分方法钩子
财产变更的副作用——不要订阅你自己的`PropertyChanged`事件。
-使用`[property: SomeAttribute]`来转发属性(例如：`[JsonIgnore]`,`[JsonPropertyName(...)]`)到生成的属性上。

# #命令-在实例方法上使用`[RelayCommand]`，而不是手动构造`RelayCommand`/`AsyncRelayCommand`实例。
—`[RelayCommand]`方法必须返回`void`或`Task`（或`Task<T>`）。
永远不要使用`async void`——异常将无法被观察到。
—对于可取消的异步工作，声明一个`CancellationToken`参数和
可选地设置`IncludeCancelCommand = true`以公开一对`XxxCancelCommand`。
使用`CanExecute = nameof(...)`+`[NotifyCanExecuteChangedFor]`保持按钮enable/disable状态同步的输入。
—“`AllowConcurrentExecutions`”默认为“`false`”。只设置`true`，当重叠调用显式地安全且有意时。
—默认的错误策略是等待并重新抛出。只设置`FlowExceptionsToTaskScheduler = true`当UI绑定到`ExecutionTask`呈现错误状态。

# #消息—默认为`WeakReferenceMessenger.Default`。只需要切换到`StrongReferenceMessenger.Default`分析时显示的信使是
热，并记录终身保证。
使用`(recipient, message)`lambda表单注册处理程序`static`修饰符-永远不要在lambda中捕获`this`。
—优选`ObservableRecipient`上的`IRecipient<TMessage>`接口
ViewModels所以`RegisterAll(this)`自动连接一切时`IsActive = true`。
—激活时设置`IsActive = true`（例如，`OnNavigatedTo`）和`IsActive = false`失活（例如，`OnNavigatedFrom`）。
—传递消息时不考虑继承—分别注册
明确的具体消息类型。
-使用通道令牌（`int`/`string`/`Guid`重载）来作用域
消息发送到子系统或窗口，当多个消费者需要时
否则发生碰撞。

依赖注入—service和ViewModel使用`Microsoft.Extensions.DependencyInjection`登记。更喜欢。. NET通用主机
（`Host.CreateDefaultBuilder()`）：配置、日志记录和范围
验证是自动连接的。
-在组合根目录中注册服务和视图模型(通常是`App.xaml.cs`)。从页面中的DI解析页面的根ViewModel
构造函数或通过导航框架。
-通过构造函数注入服务和子视图模型。不要打电话`Ioc.Default.GetService<T>()`从视图模型，服务，或任何
DI容器可以构造的类型。
寿命:
—`AddSingleton<T>()`—shell/main-window虚拟机、设置、file/HTTP    services, the shared `IMessenger`.
—`AddTransient<T>()`—按页或按文档虚拟机。
-`AddScoped<T>()`-仅显式使用`IServiceScope`；很少    needed in client apps.
—注册一次`IMessenger`(`services.AddSingleton<IMessenger>(WeakReferenceMessenger.Default)`)
并通过`ObservableRecipient(messenger)`构造函数注入它。

# #验证

—使用`ObservableValidator`+`[NotifyDataErrorInfo]`和DataAnnotation
属性(`[Required]`,`[Range]`,`[EmailAddress]`,`[MinLength]`，`[MaxLength]``[CustomValidation]`)。
-在提交表单之前调用`ValidateAllProperties()`；检查`HasErrors`，然后跳出`true`。
—提交成功后，使用`ClearAllErrors()`复位错误状态
重置表单时。
—对于跨属性规则，调用`ValidateProperty(value, nameof(Other))`从更改后的属性的`OnXxxChanged`钩子。

# # XAML

-对于WinUI 3 / UWP，更喜欢`{x:Bind}`（编译绑定）`{Binding}`。显式设置`Mode=OneWay`或`Mode=TwoWay`-`{x:Bind}`默认为`OneTime`。
—将`Command="{x:Bind ViewModel.SaveCommand}"`直接绑定到
生成命令属性。
—绑定async-command status (`IsRunning`,`ExecutionTask.Status`，`ExecutionTask.Exception`)到表面progress/errors而不是
阻塞UI线程。

要避免的事情-`[ObservableProperty] private string Name;`- PascalCase字段碰撞
使用生成的属性；使用lowerCamel。
—手动`RaisePropertyChanged(nameof(X))`呼叫旁边`[ObservableProperty]`—产生重复的通知。
-`Ioc.Default.GetService<T>()`从ViewModel构造函数内部-
隐藏依赖关系，破坏单元测试。
-不带`OnDeactivated`/`UnregisterAll`的`StrongReferenceMessenger`别针收件人和泄漏他们。
-捕获信使lambda中的`this`-闭包分配和
一生的困惑。总是使用`(r, m) => r.OnX(m)`和`static`。
-`[RelayCommand]`方法上的`async void`-返回`Task`-改变由`[ObservableProperty]`字段持有的相同引用-
相等比较器返回`true`，并且不触发更改通知。
替换实例。
-继承自`ObservableValidator`和`ObservableRecipient`-
不可能的;使用组合（注入`IMessenger`或实现）
手动验证)。