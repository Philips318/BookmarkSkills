---
name: csharp-async
description: 'Get best practices for C# async programming'
---
# c#异步编程最佳实践

你的目标是帮助我遵循c#异步编程的最佳实践。

命名约定

-对所有Async方法使用‘Async’后缀
-在适用的情况下，将方法名与其同步对应的方法名相匹配（例如，`GetData()`对应`GetDataAsync()`）

##返回类型

—当方法返回值时，返回`Task<T>`-当方法不返回值时返回`Task`-考虑`ValueTask<T>`用于高性能场景，以减少分配
避免返回`void`的异步方法，除了事件处理程序

##异常处理

-在await表达式周围使用try/catch块
避免在异步方法中吞下异常
-在适当的时候使用`ConfigureAwait(false)`来防止库代码中的死锁
-用`Task.FromException()`传播异常，而不是抛出异步任务返回方法

# #性能-使用`Task.WhenAll()`并行执行多个任务
—使用`Task.WhenAny()`实现超时或执行第一个完成的任务
-在简单传递任务结果时避免不必要的async/await-考虑长时间运行的操作的取消令牌

##常见陷阱

-不要在异步代码中使用`.Wait()`、`.Result`或`.GetAwaiter().GetResult()`-避免混合阻塞和异步代码
不要创建async void方法（除了事件处理程序）
-总是等待任务返回方法

##实现模式

—对于长时间运行的操作，采用async命令模式
-使用异步流（IAsyncEnumerable<T>）异步处理序列
—考虑公共api的基于任务的异步模式（TAP）

在检查我的c#代码时，找出这些问题，并根据这些最佳实践提出改进建议。