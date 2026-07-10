#故障排除`CommunityToolkit.Mvvm`8.x的常见错误、诊断和陷阱。

---

##源生成器诊断（`MVVMTK0xxx`）

生成器发出编号的诊断。最常见的是：

|代码|含义|修复||------|---------|-----|
|`MVVMTK0008`|包含类型（或封闭类型）不是`partial`|将`partial`添加到类声明**和**每个封闭类型|
|`MVVMTK0016`|`[NotifyCanExecuteChangedFor]`目标不是可访问的`IRelayCommand`属性|确保目标是相同类型|上的`[RelayCommand]`生成的命令(或手动声明的`IRelayCommand`属性
|`MVVMTK0017`|`[NotifyDataErrorInfo]`在`ObservableValidator`外部使用|继承`ObservableValidator`或删除属性|
|`MVVMTK0018`|`[NotifyPropertyChangedRecipients]`在`ObservableRecipient`外部使用|从`ObservableRecipient`继承或删除属性|
|从`ObservableObject`继承或将`[INotifyPropertyChanged]`/`[ObservableObject]`应用到类型|
|`MVVMTK0042`|`[ObservableProperty]`字段属于泛型类型，没有适当的`partial`声明|与`MVVMTK0008`相同的修复（添加`partial`） |

搜索完整的表格：<https://learn.microsoft.com/en-us/dotnet/communitytoolkit/mvvm/generators/errors/>
---

## “属性名与字段名冲突”```text
'SampleViewModel' already contains a definition for 'Name'
```
您使用PascalCase命名该字段：```csharp
[ObservableProperty]
private string Name;   // ❌ collides with generated property
```
使用lowerCamel（或前缀）代替：```csharp
[ObservableProperty]
private string? name;   // ✅ generates Name
```
---

## “Setter从不抛出`PropertyChanged`”

可能的原因:

1. **相同的引用。**生成器使用`EqualityComparer<T>.Default.Equals`检测更改。供参考
类型，比较器将返回`true`通知被跳过。替换实例而不是改变。
2. 属性设置为相同的值。**相同值→不通知由
设计。
3. **需要定制比较器。**对于默认相等的值类型
错，手写财产，然后打电话`SetProperty(ref field, value, comparer)`。

---

## "ContentDialog抛出`InvalidOperationException`" （WinUI 3）

不是工具包问题，但通常会受到`[RelayCommand]`异步方法的影响。
在调用`ShowAsync()`之前先设置`XamlRoot`。看到`winui3-migration-guide`技能详细说明。

---

## Async`[RelayCommand]`吞下异常默认行为：等待包装任务，异常为
在同步上下文上重新抛出。如果你的方法是`async void`，
生成器将其包装为同步`RelayCommand`，异常变为
没注意到。**总是从`[RelayCommand]`方法返回`Task`如果UI绑定到`ExecutionTask.Exception`以呈现错误，请选择加入`FlowExceptionsToTaskScheduler = true`:```csharp
[RelayCommand(FlowExceptionsToTaskScheduler = true)]
private async Task LoadAsync(CancellationToken token) { /* ... */ }
```
---

##取消似乎没有任何作用

—确保包装方法声明了一个`CancellationToken`参数。
-将令牌传递到等待的api (`HttpClient.GetAsync(url, token)`，`Task.Delay(ms, token)`,等等)。
-捕获`OperationCanceledException`，这样UI就不会看到错误。

---

信使处理程序永远不会触发

检查表:

1. 收件人注册为确切的消息类型，而不是基
类型。不考虑继承。
2. 同一个`IMessenger`实例用于发送和注册
（`WeakReferenceMessenger.Default`vs注入的每窗口信使）。
3. 发送方和接收方之间的令牌（通道）匹配。
4. 对于`WeakReferenceMessenger`，收件人可能已经是
垃圾收集。在某处保存一个强引用（通常是DI）
容器为单例虚拟机做这个)。
5. 对于`ObservableRecipient`，`IsActive`必须是`true`-`OnActivated`是注册`IRecipient<T>`处理程序的。

---

##`OnActivated`从不运行当`IsActive`从`false`到`true`。如果您从未设置`IsActive = true`，则没有处理程序注册。
常见的模式:```csharp
protected override void OnNavigatedTo(NavigationEventArgs e)
{
    base.OnNavigatedTo(e);
    ViewModel.IsActive = true;
}

protected override void OnNavigatedFrom(NavigationEventArgs e)
{
    base.OnNavigatedFrom(e);
    ViewModel.IsActive = false;
}
```
---`StrongReferenceMessenger`内存泄漏

在调用`Unregister`之前，强ref接收器将固定。:

-继承自`ObservableRecipient`（自动注销`OnDeactivated`）。
—切换到“`WeakReferenceMessenger.Default`”。
-在您的处置/拆除路径中调用`messenger.UnregisterAll(this)`。

---

“不能继承`ObservableValidator`和`ObservableRecipient`”

c#单继承-选一个。如果两者都需要：

—继承自`ObservableRecipient`（或`ObservableValidator`）。
-注入`IMessenger`（或实施验证）在侧面通过
组成。

或者使用类级别`[INotifyPropertyChanged]`/`[ObservableObject]`属性设置在包装这两个部分的自定义基类型上。

---

DI容器不能构造ViewModel

症状：`InvalidOperationException`提示“无法解析服务”
在尝试激活“MyViewModel”时，输入“X”。

原因:没有注册构造函数参数类型。添加`services.AddX(...)`。
-多个模棱两可的构造函数——容器选择最长的构造函数
其依赖项都已注册。如果两个构造器符合条件，则一个
抛出异常。将其中一个标记为规范构造函数，或者删除
歧义。
-将有作用域的服务注入到单例中（在dev模式下）
验证)。要么改变寿命要么注入`IServiceScopeFactory`从作用域解析。

---

XAML无法解析命名空间```text
The type 'local:ContactViewModel' was not found.
```
XAML名称空间映射需要引用程序集和
要匹配的名称空间。如果VM位于类库中，则映射需要
程序集名称：```xml
xmlns:vm="using:MyApp.Shared.ViewModels;assembly=MyApp.Shared"
```
（WPF语法略有不同：`xmlns:vm="clr-namespace:...;assembly=..."`。）

---

##“设计时数据没有显示任何内容”

设计时XAML编辑器在没有DI容器的情况下实例化页面。
:

-提供一个无参数构造函数来引导设计时VM。
—使用`d:DataContext="{d:DesignInstance Type=vm:ContactViewModel, IsDesignTimeCreatable=True}"`。
-使用单独的设计时视图模型类和硬编码的示例数据。

---

# #更多

—所有`MVVMTK0xxx`错误：  <https://learn.microsoft.com/en-us/dotnet/communitytoolkit/mvvm/generators/errors/>
-来源：<https://github.com/CommunityToolkit/dotnet>-示例应用程序：<https://aka.ms/mvvmtoolkit/samples>