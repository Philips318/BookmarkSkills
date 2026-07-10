#源生成器参考`CommunityToolkit.Mvvm`8的完整属性参考。x源
生成器，包含每个生成器生成的代码。

> **通用规则。**使用这些属性之一的所有类型- and
>每个封闭类型（如果嵌套）必须声明为`partial`。这个
>生成器发出兄弟分部类声明；没有`partial`,
b>编译器报告`MVVMTK0008`/`MVVMTK0042`。

---

# #`[ObservableProperty]`从私有字段生成一个可观察属性。```csharp
using CommunityToolkit.Mvvm.ComponentModel;

public partial class SampleViewModel : ObservableObject
{
    [ObservableProperty]
    private string? name;
}
```
生成(简体):```csharp
public string? Name
{
    get => name;
    set
    {
        if (!EqualityComparer<string?>.Default.Equals(name, value))
        {
            string? oldValue = name;
            OnNameChanging(value);
            OnNameChanging(oldValue, value);
            OnPropertyChanging();
            name = value;
            OnNameChanged(value);
            OnNameChanged(oldValue, value);
            OnPropertyChanged();
        }
    }
}

partial void OnNameChanging(string? value);
partial void OnNameChanging(string? oldValue, string? newValue);
partial void OnNameChanged(string? value);
partial void OnNameChanged(string? oldValue, string? newValue);
```
# # #命名

-字段`name`→属性`Name`-字段`_name`→属性`Name`-字段`m_name`→属性`Name`-字段`Name`（PascalCase）→**错误**（与生成的属性冲突）

# # #钩子

实现部分方法的任意子集。未实现的钩子是
编译器省略-零运行时成本。```csharp
[ObservableProperty]
private ChildViewModel? selectedItem;

partial void OnSelectedItemChanging(ChildViewModel? oldValue, ChildViewModel? newValue)
{
    if (oldValue is not null) oldValue.IsSelected = false;
    if (newValue is not null) newValue.IsSelected = true;
}
```
钩子方法是`partial`，没有主体声明——你不能添加
显式可访问性（没有`public`/`private`）。

---

# #`[NotifyPropertyChangedFor(nameof(Other))]`当此字段更改时，引发`PropertyChanged`以获取其他属性。
为多个目标堆叠多个属性。```csharp
[ObservableProperty]
[NotifyPropertyChangedFor(nameof(FullName))]
[NotifyPropertyChangedFor(nameof(Initials))]
private string? firstName;
```
将其用于derived/computed属性：```csharp
public string FullName => $"{FirstName} {LastName}";
public string Initials => $"{FirstName?[0]}{LastName?[0]}";
```
---

# #`[NotifyCanExecuteChangedFor(nameof(MyCommand))]`当此字段更改时调用`MyCommand.NotifyCanExecuteChanged()`。这个
目标必须是`IRelayCommand`（或`IAsyncRelayCommand`）属性。```csharp
[ObservableProperty]
[NotifyCanExecuteChangedFor(nameof(SaveCommand))]
[NotifyCanExecuteChangedFor(nameof(SubmitCommand))]
private string? name;

[RelayCommand(CanExecute = nameof(CanSave))]
private Task SaveAsync() => repo.SaveAsync(Name!);

private bool CanSave() => !string.IsNullOrWhiteSpace(Name);
```
如果目标不是可访问的，则引发> **`MVVMTK0016`**
>`IRelayCommand`属性在同一类型。

---

# #`[NotifyDataErrorInfo]`仅对继承自`ObservableValidator`的类型有效。添加一个`ValidateProperty(value)`在生成的setter内调用，所以是DataAnnotation
验证器在每个任务上运行。```csharp
using System.ComponentModel.DataAnnotations;

public partial class RegistrationViewModel : ObservableValidator
{
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required, MinLength(2), MaxLength(100)]
    private string? name;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required, EmailAddress]
    private string? email;
}
```
只有来自`ValidationAttribute`的属性才会被转发到
生成的属性。其他属性将被忽略，除非使用`[property: ]`（见下文）。

---

# #`[NotifyPropertyChangedRecipients]`仅对继承自`ObservableRecipient`的类型有效。添加一个`Broadcast(oldValue, newValue)`呼叫设置成功后，发送a`PropertyChangedMessage<T>`发送给活动`IMessenger`的所有接收者。```csharp
public partial class SelectionViewModel : ObservableRecipient
{
    [ObservableProperty]
    [NotifyPropertyChangedRecipients]
    private Item? selectedItem;
}
```
订阅者可以收听：```csharp
WeakReferenceMessenger.Default.Register<SelectionViewModel, PropertyChangedMessage<Item>>(
    this,
    static (r, m) =>
    {
        if (m.PropertyName == nameof(SelectionViewModel.SelectedItem))
            r.Handle(m.NewValue);
    });
```
---

# #`[RelayCommand]`从实例生成惰性`RelayCommand`/`AsyncRelayCommand`方法。通过`IRelayCommand`/`IAsyncRelayCommand`接口公开它。```csharp
[RelayCommand]
private void Refresh() => Items.Reset();
```

```csharp
private RelayCommand? refreshCommand;
public IRelayCommand RefreshCommand =>
    refreshCommand ??= new RelayCommand(Refresh);
```
# # #命名

-`Refresh`→`RefreshCommand`-`OnRefresh`→`RefreshCommand`（前导`On`剥离）
-`LoadAsync`→`LoadCommand`（去掉后面的`Async`）
-`OnLoadAsync`→`LoadCommand`（两个剥离）

###同步参数```csharp
[RelayCommand]
private void GreetUser(User user) => Console.WriteLine($"Hello {user.Name}");
```
生成`IRelayCommand<User> GreetUserCommand`（键入的命令）。

Async不取消```csharp
[RelayCommand]
private async Task GreetUserAsync()
{
    var user = await users.GetCurrentAsync();
    Console.WriteLine($"Hello {user.Name}");
}
```
生成支持的`IAsyncRelayCommand GreetUserCommand``AsyncRelayCommand`。

###异步取消```csharp
[RelayCommand]
private async Task GreetUserAsync(CancellationToken token)
{
    try
    {
        var user = await users.GetCurrentAsync(token);
        Console.WriteLine($"Hello {user.Name}");
    }
    catch (OperationCanceledException) { /* expected */ }
}
```
工具包将`CancellationToken`传播到包装的方法。调用`GreetUserCommand.Cancel()`表示它。

# # #`IncludeCancelCommand = true`生成配对的`XxxCancelCommand`，其`CanExecute`连接到
底层async命令的`IsRunning`状态-绑定到一个取消按钮：```csharp
[RelayCommand(IncludeCancelCommand = true)]
private async Task DownloadAsync(CancellationToken token) { /* ... */ }
```

```xml
<Button Command="{x:Bind ViewModel.DownloadCommand}" Content="Download"/>
<Button Command="{x:Bind ViewModel.DownloadCancelCommand}" Content="Cancel"/>
```
# # #`CanExecute = nameof(MethodOrProperty)````csharp
[RelayCommand(CanExecute = nameof(CanGreetUser))]
private void GreetUser(User? user) => Console.WriteLine($"Hello {user!.Name}");

private bool CanGreetUser(User? user) => user is not null;
```
在绑定命令时初始调用`CanExecute`成员，并且
同样，每次运行命令的`NotifyCanExecuteChanged`时(使用`[NotifyCanExecuteChangedFor]`为连线时自动绑定状态
更改)。

# # #`AllowConcurrentExecutions = true`默认值是`false`：当调用挂起时，命令报告
本身是不可执行的。设置`true`允许queued/parallel调用。```csharp
[RelayCommand(AllowConcurrentExecutions = true)]
private async Task PingAsync() { /* fire-and-keep-going */ }
```
当包装的方法采用`CancellationToken`并并发执行时
是否允许**请求一个新的执行，而一个正在等待取消
首先是先前的令牌。

# # #`FlowExceptionsToTaskScheduler = true`默认是等待并重新抛出（异常会使应用程序崩溃，镜像同步）
命令)。设置`true`会通过`ExecutionTask`和路由异常`TaskScheduler.UnobservedTaskException`——在UI绑定时很有用
到`ExecutionTask.Status`以呈现错误状态。```csharp
[RelayCommand(FlowExceptionsToTaskScheduler = true)]
private async Task LoadAsync(CancellationToken token) { /* ... */ }
```
---

# #`[property: SomeAttribute(...)]`将属性转发到生成的属性上(用于`[ObservableProperty]`字段或`[RelayCommand]`方法)。```csharp
[ObservableProperty]
[property: JsonRequired]
[property: JsonPropertyName("name")]
private string? username;

[RelayCommand]
[property: JsonIgnore]
private void GreetUser(User user) { /* ... */ }
```
将此用于序列化属性(`[JsonIgnore]`，`[JsonPropertyName]`,`[XmlElement]`)，数据属性（`[Display(Name=...)]`），
或任何其他需要驻留在property/command而不是
在field/method.上

---

##`[INotifyPropertyChanged]`（类级）

仅在不能继承`ObservableObject`时使用(例如，类型
已经从不同的基础继承)。生成
类型本身上的`INotifyPropertyChanged`管道。```csharp
using CommunityToolkit.Mvvm.ComponentModel;

[INotifyPropertyChanged]
public partial class MyControl : UserControl
{
    [ObservableProperty]
    private string? caption;
}
```
首选`ObservableObject`（或`ObservableValidator`/）`ObservableRecipient`)继承。类级别的
属性主要用于继承锁定的场景，例如
自定义控件和平台基础类型。

还有`[ObservableObject]`（类级别）用于相同的目的
您希望在该类型上生成完整的`SetProperty<T>`API表面
没有继承。