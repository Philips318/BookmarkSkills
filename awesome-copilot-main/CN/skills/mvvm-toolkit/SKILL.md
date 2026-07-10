---
name: mvvm-toolkit
description: 'CommunityToolkit.Mvvm (the MVVM Toolkit) core: source generators ([ObservableProperty], [RelayCommand], [NotifyPropertyChangedFor], [NotifyCanExecuteChangedFor], [NotifyDataErrorInfo]), base classes (ObservableObject / ObservableValidator / ObservableRecipient), commands (RelayCommand / AsyncRelayCommand), and validation. Companion skills: mvvm-toolkit-messenger for pub/sub, mvvm-toolkit-di for Microsoft.Extensions.DependencyInjection wiring. Works across WPF, WinUI 3, MAUI, Uno, and Avalonia.'
---
# CommunityToolkit。Mvvm(核心)

在创作或审查ViewModels， properties，
命令，或在使用`CommunityToolkit.Mvvm`8.x的应用程序中验证。

> **同伴技能。**加载**`mvvm-toolkit-messenger`**为`IMessenger`>pub/sub模式。加载**`mvvm-toolkit-di`**为
>`Microsoft.Extensions.DependencyInjection`积分。

> **快速回顾。**`[ObservableProperty]`在`partial`的私有字段
>类;`[RelayCommand]`对实例方法；继承
>`ObservableObject`(或`ObservableValidator`输入表单，
>`ObservableRecipient`当使用`IMessenger`)。

---

##包&设置```xml
<ItemGroup>
  <PackageReference Include="CommunityToolkit.Mvvm" Version="8.*" />
</ItemGroup>
```
目标：`netstandard2.0`，`netstandard2.1`,`net6.0`+。继续工作。网。网
框架,Mono。源生成器在同一个NuGet中发布-没有额外的
需要分析仪参考资料。

名称空间:```csharp
using CommunityToolkit.Mvvm.ComponentModel;   // ObservableObject, [ObservableProperty]
using CommunityToolkit.Mvvm.Input;             // [RelayCommand], RelayCommand, AsyncRelayCommand
```
> **通用规则。**所有使用`[ObservableProperty]`或
>`[RelayCommand]`-以及每个封闭类型（如果嵌套的话）必须是
>声明`partial`。没有它，发电机就会发光
>`MVVMTK0008`/`MVVMTK0042`。

---

##源生成器小抄

|属性|应用于|产生||-----------|-----------|-----------|
|`[ObservableProperty]`|私有字段|公共`INotifyPropertyChanged`属性+`OnXxxChanging`/`OnXxxChanged`部分方法钩子|
|`[NotifyPropertyChangedFor(nameof(Other))]`|可观察字段|也为列出的属性|引发`PropertyChanged`|`[NotifyCanExecuteChangedFor(nameof(MyCommand))]`|可观察字段|在更改|时调用`MyCommand.NotifyCanExecuteChanged()`|`[NotifyDataErrorInfo]`|`ObservableValidator`|从setter |调用`ValidateProperty(value)`|`[NotifyPropertyChangedRecipients]`|在`ObservableRecipient`|`Broadcast(old, new)`改变|后的观测场
|懒惰`RelayCommand`/`AsyncRelayCommand`暴露为`IRelayCommand`/`IAsyncRelayCommand`|
|`[RelayCommand(CanExecute = nameof(CanX))]`|实例方法|将`CanExecute`连接到方法或属性|
|`[RelayCommand(IncludeCancelCommand = true)]`|与`CancellationToken`|的异步方法也生成`XxxCancelCommand`|
|`[RelayCommand(AllowConcurrentExecutions = true)]`|异步方法|允许queued/parallel调用（运行时默认禁用）|
|`[RelayCommand(FlowExceptionsToTaskScheduler = true)]`|异步方法|通过`ExecutionTask`显示异常，而不是等待并重新抛出|
|`[property: SomeAttr]`|可观察字段或`[RelayCommand]`方法|将`SomeAttr`转发到生成的属性(例如，`[JsonIgnore]`) |* *命名。**字段`name`/`_name`/`m_name`→`Name`。方法`LoadAsync`→`LoadCommand`(去掉`Async`后缀；也去掉前导`On`剥夺了)。

参见[`references/source-generators.md`]（references/source-generators.md）
包含生成代码示例的完整属性引用。

---

## ViewModel模式

###简单的可观察属性```csharp
public partial class ContactViewModel : ObservableObject
{
    [ObservableProperty]
    private string? name;
}
```
###挂钩：`OnXxxChanging`/`OnXxxChanged````csharp
[ObservableProperty]
private string? name;

partial void OnNameChanged(string? value) =>
    Logger.LogInformation("Name changed to {Name}", value);
```
单参数`(value)`和双参数`(oldValue, newValue)`都重载
是可用的。只实现你需要的；未实现的钩子是
编译器省略（零运行时成本）。

依赖属性+依赖命令```csharp
[ObservableProperty]
[NotifyPropertyChangedFor(nameof(FullName))]
[NotifyCanExecuteChangedFor(nameof(SaveCommand))]
private string? firstName;

[ObservableProperty]
[NotifyPropertyChangedFor(nameof(FullName))]
[NotifyCanExecuteChangedFor(nameof(SaveCommand))]
private string? lastName;

public string FullName => $"{FirstName} {LastName}".Trim();
```
包装一个不可观察的模型```csharp
public sealed class ObservableUser(User user) : ObservableObject
{
    public string Name
    {
        get => user.Name;
        set => SetProperty(user.Name, value, user, (u, n) => u.Name = n);
    }
}
```
传递一个静态lambda（没有捕获状态）以保持调用分配自由。

---

# #命令```csharp
[RelayCommand]
private void Refresh() => Items.Reset();

[RelayCommand]
private async Task LoadAsync()
{
    foreach (var item in await service.GetItemsAsync())
        Items.Add(item);
}

[RelayCommand(IncludeCancelCommand = true)]
private async Task DownloadAsync(CancellationToken token)
{
    await using var stream = await http.GetStreamAsync(url, token);
    // ...
}

[RelayCommand(CanExecute = nameof(CanSave))]
private Task SaveAsync() => repo.SaveAsync(Name!);

private bool CanSave() => !string.IsNullOrWhiteSpace(Name);
```
只能获取手动`RelayCommand`/`AsyncRelayCommand`构造函数
当您必须显式拥有命令的生命周期或从
非平凡的来源。属性样式覆盖了大约95%的情况。

看到[`references/relaycommand-cookbook.md`] (references/relaycommand-cookbook.md)
用于同步/异步/可取消/并发/错误处理食谱。

---

基类选择

|基类|当|时使用|------------|---------|
|`ObservableObject`|默认值。`INotifyPropertyChanged`+`INotifyPropertyChanging`+`SetProperty`超载+`SetPropertyAndNotifyOnCompletion``Task`属性|
|`ObservableValidator`|虚拟机需要`INotifyDataErrorInfo`(forms, settings input) |
|`ObservableRecipient`|虚拟机发送或接收`IMessenger`消息—参见**`mvvm-toolkit-messenger`**技能|

c#是单继承的：`ObservableValidator`和`ObservableRecipient`两者都扩展了`ObservableObject`，因此组合它们需要组合
（例如，将`IMessenger`注入到`ObservableValidator`中）。

---

# #验证```csharp
using System.ComponentModel.DataAnnotations;

public sealed partial class RegistrationViewModel : ObservableValidator
{
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required, MinLength(2), MaxLength(100)]
    private string? name;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required, EmailAddress]
    private string? email;

    [RelayCommand]
    private void Submit()
    {
        ValidateAllProperties();
        if (HasErrors) return;
        // submit...
    }
}
```
其他入口点：`TrySetProperty`、`ValidateProperty(value, name)`、`ClearAllErrors()``GetErrors(propertyName)`。自定义规则支持`[CustomValidation]`方法和自定义`ValidationAttribute`子类。

参见[`references/validation.md`]（references/validation.md）的完整
验证器表面积。

---

##主要陷阱1. * *忘记`partial`。**类（和所有封闭类型）必须是`partial`。编译错误`MVVMTK0008`/`MVVMTK0042`。
2. **PascalCase字段名。* *`[ObservableProperty] private string Name;`与生成的属性碰撞。使用`name`、`_name`或`m_name`。
3. **`async void`对`[RelayCommand]`。**生成器只封装`Task`返回方法为`IAsyncRelayCommand`。`async void`变得
同步`RelayCommand`和异常未被观察到。总是返回`Task`。
4. * *忘记`[NotifyCanExecuteChangedFor]`。**保存按钮保持不变
禁用，即使`CanSave()`现在将返回`true`。
5. **改变由`[ObservableProperty]`持有的相同引用
字段。**`EqualityComparer<T>.Default`返回`true`，无通知
火灾。替换实例而不是改变它。

有关完整诊断表（`MVVMTK0xxx`）和更多缺陷，请参见
[`references/troubleshooting.md`] (references/troubleshooting.md)。

---

端到端迷你演练

一个双面板Notes应用程序演示生成器+命令+`[NotifyCanExecuteChangedFor]`:```csharp
public sealed partial class NoteViewModel(INotesService notes,
    IMessenger messenger) : ObservableRecipient(messenger)
{
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    [NotifyCanExecuteChangedFor(nameof(DeleteCommand))]
    private string? filename;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    private string? text;

    [RelayCommand(CanExecute = nameof(CanSave))]
    private Task SaveAsync()
    {
        Messenger.Send(new NoteSavedMessage(Filename!));
        return notes.SaveAsync(Filename!, Text!);
    }

    [RelayCommand(CanExecute = nameof(CanDelete))]
    private Task DeleteAsync() => notes.DeleteAsync(Filename!);

    private bool CanSave() =>
        !string.IsNullOrWhiteSpace(Filename) && !string.IsNullOrEmpty(Text);
    private bool CanDelete() => !string.IsNullOrWhiteSpace(Filename);
}
```
有关完整示例（DI连接、查看代码隐藏、XAML、单元测试），请参见
[`references/end-to-end-walkthrough.md`] (references/end-to-end-walkthrough.md)。

---

推荐信和同伴技能

|主题|地点||-------|-------|
|源生成器属性引用| [`references/source-generators.md`](references/source-generators.md) |
| RelayCommand recipes | [`references/relaycommand-cookbook.md`](references/relaycommand-cookbook.md) |
|验证深潜| [`references/validation.md`](references/validation.md) |
| [`references/end-to-end-walkthrough.md`](references/end-to-end-walkthrough.md) |
| [`references/troubleshooting.md`](references/troubleshooting.md) |
| **信使pub/sub** |同伴技能：**`mvvm-toolkit-messenger`** |
b| **`Microsoft.Extensions.DependencyInjection`布线** |同伴技能：**`mvvm-toolkit-di`** |

外部来源:

—Toolkit概述：<https://learn.microsoft.com/en-us/dotnet/communitytoolkit/mvvm/>- WinUI MVVM工具包教程：<https://learn.microsoft.com/en-us/windows/apps/tutorials/winui-mvvm-toolkit/intro>-来源：<https://github.com/CommunityToolkit/dotnet>—示例：<https://github.com/CommunityToolkit/MVVM-Samples>