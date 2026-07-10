#错误处理模式

在您的Copilot SDK应用程序中优雅地处理错误。

> **可运行示例：** [recipe/error-handling.cs]（recipe/error-handling.cs）
>
>“bash
运行recipe/error-handling.cs> ' ' '

示例场景

您需要处理各种错误情况，如连接失败、超时和无效响应。

##基本的试接```csharp
using GitHub.Copilot;

var client = new CopilotClient();

try
{
    await client.StartAsync();
    var session = await client.CreateSessionAsync(new SessionConfig
    {
        Model = "gpt-5",
        OnPermissionRequest = PermissionHandler.ApproveAll
    });

    var done = new TaskCompletionSource<string>();
    session.On(evt =>
    {
        if (evt is AssistantMessageEvent msg)
        {
            done.SetResult(msg.Data.Content);
        }
    });

    await session.SendAsync(new MessageOptions { Prompt = "Hello!" });
    var response = await done.Task;
    Console.WriteLine(response);

    await session.DisposeAsync();
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}
finally
{
    await client.StopAsync();
}
```
处理特定的错误类型```csharp
try
{
    await client.StartAsync();
}
catch (FileNotFoundException)
{
    Console.WriteLine("Copilot CLI not found. Please install it first.");
}
catch (HttpRequestException ex) when (ex.Message.Contains("connection"))
{
    Console.WriteLine("Could not connect to Copilot CLI server.");
}
catch (Exception ex)
{
    Console.WriteLine($"Unexpected error: {ex.Message}");
}
```
##超时处理```csharp
var session = await client.CreateSessionAsync(new SessionConfig
{
    Model = "gpt-5",
    OnPermissionRequest = PermissionHandler.ApproveAll
});

try
{
    var done = new TaskCompletionSource<string>();
    session.On(evt =>
    {
        if (evt is AssistantMessageEvent msg)
        {
            done.SetResult(msg.Data.Content);
        }
    });

    await session.SendAsync(new MessageOptions { Prompt = "Complex question..." });

    // Wait with timeout (30 seconds)
    using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
    var response = await done.Task.WaitAsync(cts.Token);

    Console.WriteLine(response);
}
catch (OperationCanceledException)
{
    Console.WriteLine("Request timed out");
}
```
##终止请求```csharp
var session = await client.CreateSessionAsync(new SessionConfig
{
    Model = "gpt-5",
    OnPermissionRequest = PermissionHandler.ApproveAll
});

// Start a request
await session.SendAsync(new MessageOptions { Prompt = "Write a very long story..." });

// Abort it after some condition
await Task.Delay(5000);
await session.AbortAsync();
Console.WriteLine("Request aborted");
```
##安全关机```csharp
Console.CancelKeyPress += async (sender, e) =>
{
    e.Cancel = true;
    Console.WriteLine("Shutting down...");

    try
    {
        await client.StopAsync();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Cleanup error: {ex.Message}");
    }

    Environment.Exit(0);
};
```
在1.0中，如果在清理过程中遇到错误，`StopAsync()`会抛出，而不是返回
>清理错误列表，因此将其包装在try/catch中以记录失败，而不是让它们发生
b>崩溃关闭。如果优雅的停止时间太长，则使用`ForceStopAsync()`。

##使用await进行自动处理```csharp
await using var client = new CopilotClient();
await client.StartAsync();

var session = await client.CreateSessionAsync(new SessionConfig
{
    Model = "gpt-5",
    OnPermissionRequest = PermissionHandler.ApproveAll
});

// ... do work ...

// client.StopAsync() is automatically called when exiting scope
```
最佳实践

权限处理是可选择的。如果会话可能需要工具、文件或系统访问，则在创建会话时显式设置`OnPermissionRequest`。

1. **总是清理**：使用try-finally或`await using`来确保`StopAsync()`被调用
2. **处理连接错误**:CLI可能未安装或未运行
3. **设置适当的超时时间**：对于长时间运行的请求使用`CancellationToken`4. **日志错误**：捕获错误细节以便调试