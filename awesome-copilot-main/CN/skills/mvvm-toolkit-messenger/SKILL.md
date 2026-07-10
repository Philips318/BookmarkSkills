---
name: mvvm-toolkit-messenger
description: 'CommunityToolkit.Mvvm Messenger pub/sub for decoupled communication between ViewModels (or any objects). Covers WeakReferenceMessenger vs StrongReferenceMessenger, IRecipient<TMessage>, RequestMessage<T> / AsyncRequestMessage<T> / CollectionRequestMessage<T>, ValueChangedMessage<T>, channels (tokens), and the ObservableRecipient activation lifecycle. Use across WPF, WinUI 3, .NET MAUI, Uno, and Avalonia.'
---
# CommunityToolkit。Mvvm信使Pub/sub消息传递ViewModels（或任何对象），而不强制共享
参考图。`CommunityToolkit.Mvvm`8 .x的一部分。

> * * TL,博士。**默认为`WeakReferenceMessenger.Default`。注册处理程序
>与`(recipient, message)`lambda和`static`修饰符，所以你
>永远不要捕捉`this`。继承自`ObservableRecipient`和toggle
>`IsActive`在activation/deactivation得到自动register/unregister.---

何时使用此技能

两个或更多的viewmodel需要对一个事件(登录，主题更改，
保存（导航），而不保留彼此的引用
- ViewModel需要向另一个VM请求一个值（request/reply）
-您正在使用通道令牌将事件限定到子系统或窗口
-诊断“我的处理程序从未触发”或弱引用接收者生命周期
问题

有关源代码生成器、基类和命令，请参阅**`mvvm-toolkit`**
技能。关于DI连接（注册一个`IMessenger`实例），请参见
* *`mvvm-toolkit-di`* *。

---选择一个实现

|当|时，输入||------|------|
|`WeakReferenceMessenger.Default`| **默认值。**即使已注册，收件人仍不具备GC资格。内部修整运行在全gc；不需要手动`Cleanup()`。|
|`StrongReferenceMessenger.Default`|分析器显示信使很热，分配很重要。收件人被固定，直到你`Unregister`。忘记取消注册会泄露它们。|
自定义`IMessenger`实例|Per-window/per-scope（例如，每个应用程序窗口一个信使）。直接构建，通过DI注入。|`ObservableRecipient`的无参数构造函数使用`WeakReferenceMessenger.Default`。将不同的`IMessenger`传递给它的
要重写的构造函数。

---

定义消息

该工具包提供基类；任何类都可以。```csharp
using CommunityToolkit.Mvvm.Messaging.Messages;

// Single-payload broadcast
public sealed class LoggedInUserChangedMessage(User user)
    : ValueChangedMessage<User>(user);

// Custom shape (records are great for this)
public sealed record ThemeChangedMessage(AppTheme NewTheme);

// Empty signal
public sealed record RefreshRequestedMessage;
```
---

##注册收件人

Lambda样式（推荐）```csharp
WeakReferenceMessenger.Default.Register<MyViewModel, ThemeChangedMessage>(
    this,
    static (recipient, message) => recipient.OnThemeChanged(message.NewTheme));
```
`static`修饰符防止意外的闭包分配并保持
lambda中的`this`-使用`recipient`参数代替。`IRecipient<TMessage>`接口样式```csharp
public sealed class MyViewModel : ObservableRecipient,
    IRecipient<ThemeChangedMessage>,
    IRecipient<RefreshRequestedMessage>
{
    public void Receive(ThemeChangedMessage message) { /* ... */ }
    public void Receive(RefreshRequestedMessage message) { /* ... */ }
}
```
`ObservableRecipient.OnActivated()`调用`Messenger.RegisterAll(this)`，
它订阅由该类型实现的每个`IRecipient<T>`接口。
如果你不使用`ObservableRecipient`，手动注册：```csharp
WeakReferenceMessenger.Default.RegisterAll(this);
```
---

##发送消息```csharp
WeakReferenceMessenger.Default.Send(new ThemeChangedMessage(AppTheme.Dark));

// Empty payloads use the parameterless overload:
WeakReferenceMessenger.Default.Send<RefreshRequestedMessage>();
```
---

##通道（令牌）

将消息作用域到带有令牌（任意相等）的子系统或窗口
取值（`int`,`string`,`Guid`）：```csharp
const int LeftPaneChannel = 1;

WeakReferenceMessenger.Default.Register<MyViewModel, RefreshRequestedMessage, int>(
    this, LeftPaneChannel,
    static (r, _) => r.RefreshLeft());

WeakReferenceMessenger.Default.Send(new RefreshRequestedMessage(), LeftPaneChannel);
```
在没有令牌的情况下发送的消息使用默认的共享通道——它们是
**不**传递给通道范围的接收者。

---

##请求/回复

对于请求式场景，其中收件人将值返回给
发送者，使用`RequestMessage<T>`家族。

同步请求```csharp
public sealed class CurrentUserRequest : RequestMessage<User> { }

WeakReferenceMessenger.Default.Register<UserService, CurrentUserRequest>(
    this,
    static (r, m) => m.Reply(r.CurrentUser));

User user = WeakReferenceMessenger.Default.Send<CurrentUserRequest>();
```
如果不是，则从`CurrentUserRequest`到`User`的隐式转换会抛出
收件人名为`Reply`。首先捕获要检查的消息：```csharp
var request = WeakReferenceMessenger.Default.Send<CurrentUserRequest>();
if (request.HasReceivedResponse)
    User user = request.Response;
```
异步请求```csharp
public sealed class CurrentUserRequest : AsyncRequestMessage<User> { }

WeakReferenceMessenger.Default.Register<UserService, CurrentUserRequest>(
    this,
    static (r, m) => m.Reply(r.GetCurrentUserAsync()));

User user = await WeakReferenceMessenger.Default.Send<CurrentUserRequest>();
```
收集请求（fan-in）`CollectionRequestMessage<T>`和`AsyncCollectionRequestMessage<T>`集合
每个应答者都发出一个`Reply`：```csharp
public sealed class OpenDocumentsRequest : CollectionRequestMessage<Document> { }

var docs = WeakReferenceMessenger.Default.Send<OpenDocumentsRequest>();
foreach (Document doc in docs) { /* ... */ }
```
---

# #生命周期

即使使用`WeakReferenceMessenger`，也要在接收人时显式取消注册
正在被拆除-它修剪死条目并提高性能；```csharp
WeakReferenceMessenger.Default.Unregister<ThemeChangedMessage>(this);
WeakReferenceMessenger.Default.Unregister<ThemeChangedMessage, int>(this, LeftPaneChannel);
WeakReferenceMessenger.Default.UnregisterAll(this);
```
`ObservableRecipient.OnDeactivated()`自动执行此操作`IsActive`变成`false`。从你的激活钩子设置它：```csharp
protected override void OnNavigatedTo(NavigationEventArgs e)
{
    base.OnNavigatedTo(e);
    ViewModel.IsActive = true;
}

protected override void OnNavigatedFrom(NavigationEventArgs e)
{
    ViewModel.IsActive = false;
    base.OnNavigatedFrom(e);
}
```
---

##常见陷阱1. **捕获lambda中的`this`。**`(r, m) => OnX(m)`隐式
捕捉`this`;分配闭包并混淆生命周期。总是使用`(r, m) => r.OnX(m)`和`static`。
2. **强ref接收器没有`Unregister`。* *和`StrongReferenceMessenger`，接收者（及其整个对象图）
永远呆在原地。要么继承`ObservableRecipient`（`OnDeactivated`中的自动注销）或调用`UnregisterAll(this)`。
3. **继承消息类型。**为`BaseMessage`注册的处理程序是`DerivedMessage : BaseMessage`不调用**。登记每一个
具体的类型。
4. 错误的信使实例。**通过`WeakReferenceMessenger.Default`发送
通过注入的每窗口信使注册意味着消息
永远不会到来。在任何地方使用相同的`IMessenger`(通常是inject
它通过`ObservableRecipient(messenger)`)。
5. **`OnActivated`从不运行。**`ObservableRecipient`只注册
当`IsActive`从`false`翻转到`true`时的处理程序。
6. * *跨线程更新时。**信使是线程不可知论者。如果一个
handler更新UI，手动封送
（`DispatcherQueue.TryEnqueue`/`Dispatcher.BeginInvoke`）。---

多个信使（每个窗口作用域）```csharp
services.AddSingleton<IMessenger>(WeakReferenceMessenger.Default); // app-wide
services.AddScoped<WindowScopedMessenger>();                       // per-window
```
注入适当的`IMessenger`到ViewModel构造函数中：```csharp
public sealed partial class WindowViewModel(IMessenger messenger)
    : ObservableRecipient(messenger) { }
```
这将广播隔离到单个窗口-对于多窗口非常有用
桌面应用程序（WinUI 3， WPF， MAUI桌面，Avalonia）。

---

# #引用

|主题|文件||-------|------|
| [`references/messenger-patterns.md`](references/messenger-patterns.md) |

外部:

—Messenger文档：<https://learn.microsoft.com/en-us/dotnet/communitytoolkit/mvvm/messenger>-`WeakReferenceMessenger`API:<https://learn.microsoft.com/en-us/dotnet/api/communitytoolkit.mvvm.messaging.weakreferencemessenger>-来源：<https://github.com/CommunityToolkit/dotnet>