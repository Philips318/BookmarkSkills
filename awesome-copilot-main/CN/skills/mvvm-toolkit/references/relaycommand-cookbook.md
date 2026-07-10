# RelayCommand食谱`RelayCommand`/`AsyncRelayCommand`和`[RelayCommand]`的配方
发电机。默认为生成器属性样式；手动构造函数
高级案例的模式在底部列出。

---

##同步命令```csharp
[RelayCommand]
private void IncrementCounter() => Counter++;
```

```xml
<Button Command="{x:Bind ViewModel.IncrementCounterCommand}" Content="+1"/>
```
##同步命令与参数```csharp
[RelayCommand]
private void RemoveItem(Item item) => Items.Remove(item);
```

```xml
<Button Command="{x:Bind ViewModel.RemoveItemCommand}"
        CommandParameter="{x:Bind Item}" Content="Remove"/>
```
生成器根据参数类型选择`IRelayCommand<Item>`。

##与`CanExecute`同步命令```csharp
[ObservableProperty]
[NotifyCanExecuteChangedFor(nameof(SubmitCommand))]
private string? text;

[RelayCommand(CanExecute = nameof(CanSubmit))]
private void Submit() => service.Submit(Text!);

private bool CanSubmit() => !string.IsNullOrWhiteSpace(Text);
```
`[NotifyCanExecuteChangedFor]`自动引发`CanExecuteChanged`无论何时`Text`发生变化，如果没有它，该按钮将保持禁用状态
用户类型。

---

##异步命令```csharp
[RelayCommand]
private async Task LoadAsync()
{
    Items.Clear();
    foreach (var item in await service.GetItemsAsync())
        Items.Add(item);
}
```
将UI绑定到`LoadCommand.IsRunning`以显示转轮：```xml
<ProgressRing IsActive="{x:Bind ViewModel.LoadCommand.IsRunning, Mode=OneWay}"/>
```
带取消的Async命令```csharp
[RelayCommand(IncludeCancelCommand = true)]
private async Task DownloadAsync(CancellationToken token)
{
    try
    {
        await using var stream = await http.GetStreamAsync(url, token);
        // ...
    }
    catch (OperationCanceledException)
    {
        // Expected — user cancelled.
    }
}
```

```xml
<Button Command="{x:Bind ViewModel.DownloadCommand}" Content="Download"/>
<Button Command="{x:Bind ViewModel.DownloadCancelCommand}" Content="Cancel"/>
```
`DownloadCancelCommand.CanExecute`自动连接到`DownloadCommand.IsRunning`。

##异步命令与并发```csharp
[RelayCommand(AllowConcurrentExecutions = true)]
private async Task PingAsync(string host)
{
    await pingService.PingAsync(host);
}
```
Default （`AllowConcurrentExecutions = false`）将命令报告为
在前一个执行未执行时禁用。设置为`true`即发即弃模式，其中重叠调用是安全的。

Async命令向UI显示错误```csharp
[RelayCommand(FlowExceptionsToTaskScheduler = true)]
private async Task SyncAsync(CancellationToken token)
{
    await syncService.SyncAsync(token);
}
```

```xml
<TextBlock Text="{x:Bind ViewModel.SyncCommand.ExecutionTask.Exception, Mode=OneWay}"/>
```
如果没有`FlowExceptionsToTaskScheduler = true`，则在`SyncAsync`将使应用程序崩溃（镜像同步命令）。有了它，
异常通过`ExecutionTask`和气泡浮出水面`TaskScheduler.UnobservedTaskException`。

显示async命令状态```xml
<StackPanel>
    <ProgressRing IsActive="{x:Bind ViewModel.SyncCommand.IsRunning, Mode=OneWay}"/>
    <TextBlock Text="{x:Bind ViewModel.SyncCommand.ExecutionTask.Status, Mode=OneWay}"/>
</StackPanel>
```
`IAsyncRelayCommand`上的有用属性：

|属性|类型|用途||----------|------|---------|
|`ExecutionTask`|`Task?`|当前运行（或最后完成）的任务|
|`IsRunning`|`bool`|`true`当一个任务在飞行|
|`CanBeCanceled`|`bool`|`true`如果包装方法接受`CancellationToken`|
|`IsCancellationRequested`|`bool`|`true`之后，`Cancel()`被调用用于飞行任务|

方法:

|方法|目的||--------|---------|
|`Cancel()`|主`CancellationToken`|`NotifyCanExecuteChanged()`|重新求值`CanExecute`并引发`CanExecuteChanged`|

---

##转发属性到生成的命令属性```csharp
[RelayCommand]
[property: JsonIgnore]
[property: Description("Saves the current document")]
private Task SaveAsync() => repo.SaveAsync(Text!);
```
生成器发出`SaveCommand`、`[JsonIgnore]`和`[Description]`应用-当虚拟机被序列化时有用。

---

##手动`RelayCommand`/`AsyncRelayCommand`当您需要时，请使用手动构造函数：

-由多个方法组成或动态重建的命令
-从外部可观察对象构建的`CanExecute`谓词
-在字段中保存的iccommand实例（罕见的；生成器的lazy属性）
几乎对所有情况都足够了)```csharp
public sealed class CounterViewModel : ObservableObject
{
    public CounterViewModel()
    {
        IncrementCommand = new RelayCommand(() => Counter++);
        DecrementCommand = new RelayCommand(() => Counter--, () => Counter > 0);
    }

    [ObservableProperty]
    private int counter;

    public IRelayCommand IncrementCommand { get; }
    public IRelayCommand DecrementCommand { get; }
}
```

```csharp
public sealed class DownloadViewModel : ObservableObject
{
    public DownloadViewModel()
    {
        DownloadCommand = new AsyncRelayCommand(DownloadAsync, () => CanDownload);
    }

    [ObservableProperty]
    private bool canDownload = true;

    public IAsyncRelayCommand DownloadCommand { get; }

    private async Task DownloadAsync()
    {
        CanDownload = false;
        try { await http.DownloadAsync(); }
        finally { CanDownload = true; }
    }
}
```
手动触发`CanExecute`重新评估`SomeCommand.NotifyCanExecuteChanged()`。

---

##`Task.WhenAll`从单个命令```csharp
[RelayCommand]
private async Task SyncAllAsync(CancellationToken token)
{
    var tasks = providers.Select(p => p.SyncAsync(token));
    await Task.WhenAll(tasks);
}
```
如果希望对每个提供者进行单独的进度跟踪，则公开一个命令
而是每个提供者。

---

常见错误

1. **`async void`而不是`async Task`。**生成器只封装`Task`-返回方法为`IAsyncRelayCommand`。`async void`变成了a
同步`RelayCommand`和异常是未观察到的。
2. * *忘记`[NotifyCanExecuteChangedFor]`。**该按钮保持禁用状态
尽管`CanX()`现在会返回`true`。
3. **在不可取消的命令上调用`Cancel()`。**只命令
包装方法接受`CancellationToken`荣誉`Cancel()`。
4. **捕捉`OperationCanceledException`和重新投掷作为不同的
类型。**失去取消语义；`ExecutionTask.IsCanceled`是`false`。让`OperationCanceledException`传播（或返回）。
5. **等待`IAsyncRelayCommand.ExecuteAsync()`从另一个内部`[RelayCommand]`。**建议直接调用底层方法
避免双重包装cancellation/concurrency语义。