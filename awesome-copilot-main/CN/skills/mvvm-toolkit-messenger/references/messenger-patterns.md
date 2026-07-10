# Messenger模式`CommunityToolkit.Mvvm.Messaging`提供了解耦的pub/sub视图模型（或任何对象），而不强制共享引用图。

##选择实现

|输入|何时使用||------|------------|
|`WeakReferenceMessenger.Default`| **默认值。**即使仍然注册，收件人仍有弱资格获得GC。内部修整运行在全gc期间。不需要手动`Cleanup()`。|
|`StrongReferenceMessenger.Default`|当分析显示信使是热的并且分配很重要时使用。收件人被固定，直到你`Unregister`。忘记取消注册会泄露它们。|
自定义`IMessenger`实例|Per-window/per-scope信使（例如，每个应用程序窗口一个）。直接构建，通过DI注入。|`ObservableRecipient`的无参数构造函数使用`WeakReferenceMessenger.Default`。将不同的`IMessenger`传递给它的
要重写的构造函数。

---

##定义消息

该工具包提供了一些可以继承的基类，但是任何类都可以
作品。

###普通有效载荷```csharp
public sealed record ThemeChangedMessage(AppTheme NewTheme);
```
# # #`ValueChangedMessage<T>````csharp
using CommunityToolkit.Mvvm.Messaging.Messages;

public sealed class LoggedInUserChangedMessage(User user)
    : ValueChangedMessage<User>(user);
```
通过`.Value`访问有效负载。

###空信号```csharp
public sealed record RefreshRequestedMessage;
```
有用的“现在重新加载”或“现在保存”广播，没有有效载荷。

---

##注册收件人

Lambda样式（推荐）```csharp
WeakReferenceMessenger.Default.Register<MyViewModel, ThemeChangedMessage>(
    this,
    static (recipient, message) => recipient.OnThemeChanged(message.NewTheme));
```
`static`修饰符确保lambda不会捕获`this`(或任何
局部变量)，保持它不需要分配，并防止意外的强
通过闭包捕获返回到接收者的引用。`IRecipient<TMessage>`接口样式```csharp
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

// Empty payloads can use the parameterless overload:
WeakReferenceMessenger.Default.Send<RefreshRequestedMessage>();
```
---

##通道（令牌）Send/receive通过一个指定通道将消息作用到子系统。这个
令牌是任何等价值（通常是`int`、`string`或`Guid`）。```csharp
const int LeftPaneChannel = 1;
const int RightPaneChannel = 2;

WeakReferenceMessenger.Default.Register<MyViewModel, RefreshRequestedMessage, int>(
    this, LeftPaneChannel,
    static (r, _) => r.RefreshLeft());

WeakReferenceMessenger.Default.Send(new RefreshRequestedMessage(), LeftPaneChannel);
```
没有令牌发送的消息使用默认的共享通道，并且是
**不**传递给通道范围的接收者。

---

##请求/回复

对于请求式场景，收件人应提供返回的值
发送者，使用`RequestMessage<T>`族。

同步请求```csharp
public sealed class CurrentUserRequest : RequestMessage<User> { }

// Recipient
WeakReferenceMessenger.Default.Register<UserService, CurrentUserRequest>(
    this,
    static (r, m) => m.Reply(r.CurrentUser));

// Caller
User user = WeakReferenceMessenger.Default.Send<CurrentUserRequest>();
```
如果不是，则从`CurrentUserRequest`到`User`的隐式转换会抛出
收件人名为`Reply`。要检查，首先捕获消息：```csharp
var request = WeakReferenceMessenger.Default.Send<CurrentUserRequest>();
if (request.HasReceivedResponse)
{
    User user = request.Response;
}
```
异步请求```csharp
public sealed class CurrentUserRequest : AsyncRequestMessage<User> { }

WeakReferenceMessenger.Default.Register<UserService, CurrentUserRequest>(
    this,
    static (r, m) => m.Reply(r.GetCurrentUserAsync()));

User user = await WeakReferenceMessenger.Default.Send<CurrentUserRequest>();
```
收集请求（fan-in）`CollectionRequestMessage<T>`和`AsyncCollectionRequestMessage<T>`集合
来自每个处理消息的接收者的`Reply`：```csharp
public sealed class OpenDocumentsRequest : CollectionRequestMessage<Document> { }

var responses = WeakReferenceMessenger.Default.Send<OpenDocumentsRequest>();
foreach (Document doc in responses) { /* ... */ }
```
---

# #取消注册

当收件人的生命周期结束时，始终取消注册。与`WeakReferenceMessenger`，这是为了提高性能（删除无效条目）；
对于`StrongReferenceMessenger`，需要避免泄漏。```csharp
WeakReferenceMessenger.Default.Unregister<ThemeChangedMessage>(this);
WeakReferenceMessenger.Default.Unregister<ThemeChangedMessage, int>(this, LeftPaneChannel);
WeakReferenceMessenger.Default.UnregisterAll(this);
```
`ObservableRecipient.OnDeactivated()`为您注销所有内容`IsActive`翻转到`false`-在激活流中设置`IsActive = true`（例如，页`OnNavigatedTo`）和`IsActive = false`在拆卸。

---

##终身陷阱1. * * Closure-captured`this`。**避免`(r, m) => OnX(m)`lambdas
隐式捕获封闭的`this`。使用`(r, m) => r.OnX(m)`而是传入收件人。
2. **长寿的强裁判。**带`StrongReferenceMessenger`，
忘记`UnregisterAll`会保留接收方（及其整个对象）
图)永远活着。
3. **继承消息类型。**为`BaseMessage`注册的处理程序是`DerivedMessage : BaseMessage`没有被**调用。登记每一个
要处理的具体类型。
4. **多个`ObservableRecipient`激活。**设置`IsActive = true`两次没有中间停用抛出保护开关。
5. * * ui线程编组。**信使是线程不可知论者。如果一个
handler更新UI，手动封送
（`DispatcherQueue.TryEnqueue`/`Dispatcher.BeginInvoke`）

---

##多个信使

常见的架构是每个窗口或每个作用域一个信使：```csharp
services.AddSingleton<IMessenger>(WeakReferenceMessenger.Default);  // app-wide
services.AddScoped<WindowScopedMessenger>();                        // per-window
```
注入适当的`IMessenger`到ViewModel构造函数中：```csharp
public sealed partial class WindowViewModel(IMessenger messenger)
    : ObservableRecipient(messenger) { /* ... */ }
```
这将广播隔离到单个窗口-对于多窗口非常有用
桌面应用程序（WinUI 3， WPF， MAUI桌面，Avalonia）。