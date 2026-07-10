#错误处理模式

在您的Copilot SDK应用程序中优雅地处理错误。

> **可运行示例：** [recipe/ErrorHandling.java]（recipe/ErrorHandling.java）
>
>“bash
> jbangrecipe/ErrorHandling.java> ' ' '

示例场景

您需要处理各种错误情况，如连接失败、超时和无效响应。

##基本的资源尝试

Java的`try-with-resources`确保始终清理客户机，即使在发生异常时也是如此。```java
//DEPS com.github:copilot-sdk-java:0.2.1-java.1

import com.github.copilot.sdk.*;
import com.github.copilot.sdk.json.*;

public class BasicErrorHandling {
    public static void main(String[] args) {
        try (var client = new CopilotClient()) {
            client.start().get();
            var session = client.createSession(
                new SessionConfig()
                    .setOnPermissionRequest(PermissionHandler.APPROVE_ALL)
                    .setModel("gpt-5")).get();

            var response = session.sendAndWait(
                new MessageOptions().setPrompt("Hello!")).get();
            System.out.println(response.getData().content());

            session.close();
        } catch (Exception ex) {
            System.err.println("Error: " + ex.getMessage());
        }
    }
}
```
处理特定的错误类型

每个`CompletableFuture.get()`调用都将失败封装在`ExecutionException`中。展开原因以检查真正的错误。```java
import java.io.IOException;
import java.util.concurrent.ExecutionException;

try (var client = new CopilotClient()) {
    client.start().get();
} catch (ExecutionException ex) {
    var cause = ex.getCause();
    if (cause instanceof IOException) {
        System.err.println("Copilot CLI not found or could not connect: " + cause.getMessage());
    } else {
        System.err.println("Unexpected error: " + cause.getMessage());
    }
} catch (InterruptedException ex) {
    Thread.currentThread().interrupt();
    System.err.println("Interrupted while starting client.");
}
```
##超时处理

在`CompletableFuture`上使用重载的`get(timeout, unit)`来强制执行时间限制。```java
import java.util.concurrent.TimeUnit;
import java.util.concurrent.TimeoutException;

var session = client.createSession(
    new SessionConfig()
        .setOnPermissionRequest(PermissionHandler.APPROVE_ALL)
        .setModel("gpt-5")).get();

try {
    var response = session.sendAndWait(
        new MessageOptions().setPrompt("Complex question..."))
        .get(30, TimeUnit.SECONDS);

    System.out.println(response.getData().content());
} catch (TimeoutException ex) {
    System.err.println("Request timed out after 30 seconds.");
    session.abort().get();
}
```
##终止请求

通过调用`session.abort()`取消正在运行的请求。```java
var session = client.createSession(
    new SessionConfig()
        .setOnPermissionRequest(PermissionHandler.APPROVE_ALL)
        .setModel("gpt-5")).get();

// Start a request without waiting
session.send(new MessageOptions().setPrompt("Write a very long story..."));

// Abort after some condition
Thread.sleep(5000);
session.abort().get();
System.out.println("Request aborted.");
```
##安全关机

使用JVM关闭钩子在进程中断时进行清理。```java
var client = new CopilotClient();
client.start().get();

Runtime.getRuntime().addShutdownHook(new Thread(() -> {
    System.out.println("Shutting down...");
    try {
        client.close();
    } catch (Exception ex) {
        System.err.println("Cleanup error: " + ex.getMessage());
    }
}));
```
Try-with-resources（嵌套）

当使用多个会话时，嵌套`try-with-resources`块以确保每个资源都已关闭。```java
try (var client = new CopilotClient()) {
    client.start().get();

    try (var session = client.createSession(
            new SessionConfig()
                .setOnPermissionRequest(PermissionHandler.APPROVE_ALL)
                .setModel("gpt-5")).get()) {

        session.sendAndWait(
            new MessageOptions().setPrompt("Hello!")).get();
    } // session is closed here

} // client is closed here
```
处理工具错误

在定义工具时，将错误字符串返回给模型以表示失败，而不是抛出。```java
import com.github.copilot.sdk.json.ToolDefinition;
import java.util.concurrent.CompletableFuture;

var readFileTool = ToolDefinition.create(
    "read_file",
    "Read a file from disk",
    Map.of(
        "type", "object",
        "properties", Map.of(
            "path", Map.of("type", "string", "description", "File path")
        ),
        "required", List.of("path")
    ),
    invocation -> {
        try {
            var path = (String) invocation.getArguments().get("path");
            var content = java.nio.file.Files.readString(
                java.nio.file.Path.of(path));
            return CompletableFuture.completedFuture(content);
        } catch (java.io.IOException ex) {
            return CompletableFuture.completedFuture(
                "Error: Failed to read file: " + ex.getMessage());
        }
    }
);

// Register tools when creating the session
var session = client.createSession(
    new SessionConfig()
        .setOnPermissionRequest(PermissionHandler.APPROVE_ALL)
        .setModel("gpt-5")
        .setTools(List.of(readFileTool))
).get();
```
最佳实践

1. **使用try-with-resources**：始终将`CopilotClient`（和会话，如果`AutoCloseable`）包装在try-with-resources中，以保证清理。
2. **打开`ExecutionException`**：调用`getCause()`来检查真正的错误-外部的`ExecutionException`只是一个`CompletableFuture`包装器。
3. **恢复中断标志**：当捕捉到`InterruptedException`时，调用`Thread.currentThread().interrupt()`保持中断状态。
4. **设置超时：对于任何可能无限期阻塞的调用，使用`get(timeout, TimeUnit)`而不是`get()`。
5. **返回工具错误，不要抛出**：从`CompletableFuture`返回错误字符串，以便模型可以优雅地恢复。
6. **日志错误**：捕获错误细节用于调试——考虑使用SLF4J这样的日志框架用于生产应用程序。