---
applyTo: '**/*.java, **/pom.xml'
description: 'This file provides guidance on building Java applications using GitHub Copilot SDK for Java.'
name: 'GitHub Copilot SDK Java Instructions'
---
##核心原则

- SDK是在公开预览和可能有突破性的变化
-需要Java 17或更高版本的基线SDK使用。一些示例使用较新的JDK特性，因此需要JDK 21或更高版本（例如，通过`Executors.newVirtualThreadPerTaskExecutor()`和`switch`模式匹配的虚拟线程）。**强烈推荐Java 25或更高版本**。
-需要安装GitHub CopilotCLI，并放在PATH中
—所有异步操作都使用`CompletableFuture`实现`AutoCloseable`用于资源清理（try-with-resources）
-配置类上的getter返回`Optional<T>`（或`OptionalInt`/`OptionalDouble`），以区分“未设置”和显式值；setter接受原始类型并返回用于链接的`this`。如果需要，使用`clear`方法来取消设置值。

# #安装

# # # Maven```xml
<dependency>
    <groupId>com.github</groupId>
    <artifactId>copilot-sdk-java</artifactId>
    <version>${copilot-sdk-java.version}</version>
</dependency>
```
# # # Gradle```groovy
implementation "com.github:copilot-sdk-java:${copilotSdkJavaVersion}"
```
##客户端初始化

基本客户端设置```java
try (var client = new CopilotClient()) {
    client.start().get();
    // Use client...
}
```
虚拟线程（JDK 25+）

在JDK 21中引入了虚拟线程，但是直到JDK 25才修复了重要的性能错误，使得JDK 25成为虚拟线程在生产环境中使用的最低推荐版本。在JDK 25+中，使用虚拟线程执行器可以显著提高可伸缩性。SDK的异步操作将在虚拟线程上运行，而不是默认的`ForkJoinPool`：```java
var options = new CopilotClientOptions()
    .setExecutor(Executors.newVirtualThreadPerTaskExecutor());

try (var client = new CopilotClient(options)) {
    client.start().get();
    // Use client...
}
```
客户端配置选项

当创建一个CopilotClient时，使用`CopilotClientOptions`：-`cliPath`- CLI可执行文件的路径（默认为Path中的“copilot”）
-`cliArgs`-附加在sdk管理标志之前的参数
-`cliUrl`-已存在的CLI服务器的URL（例如localhost:8080）。当提供时，客户端不会生成进程
-`port`-服务器端口（默认：0随机，只有当`useStdio`为false时）
-`useStdio`-使用stdio传输而不是TCP（默认值：true）
-`logLevel`-日志级别：error， warn, info, debug, trace（默认为info）
-`autoStart`-第一次请求自动启动服务器（默认：true）
-`autoRestart`-崩溃时自动重启（默认为true）
—`cwd`—CLI进程的工作目录
—`environment`—CLI进程环境变量
-`gitHubToken`- GitHub令牌用于身份验证
-`useLoggedInUser`-使用登录的`gh`CLI授权（默认：true，除非提供令牌）
-`onListModels`- BYOK场景的自定义模型列表处理程序
-`remote`-启用任务控制/ cloud会话集成（默认：false）
-`telemetry`-`TelemetryConfig`用于OpenTelemetry导出（从1.2.0开始）
-`sessionIdleTimeoutSeconds`-会话自动关闭前的空闲超时（从1.3.0开始）
-`executor`-自定义`Executor`异步操作（默认：ForkJoinPool）
-`tcpConnectionToken`- TCP传输认证的安全令牌```java
var options = new CopilotClientOptions()
    .setCliPath("/path/to/copilot")
    .setLogLevel("debug")
    .setAutoStart(true)
    .setAutoRestart(true)
    .setGitHubToken(System.getenv("GITHUB_TOKEN"));

try (var client = new CopilotClient(options)) {
    client.start().get();
    // Use client...
}
```
手动服务器控制

对于显式控制：```java
var client = new CopilotClient(new CopilotClientOptions().setAutoStart(false));
client.start().get();
// Use client...
client.stop().get();
```
当`stop()`耗时太长时，请使用`forceStop()`。

##会话管理

创建会话

使用`SessionConfig`进行配置。权限处理程序是**必需的：```java
var session = client.createSession(new SessionConfig()
    .setModel("gpt-5")
    .setStreaming(true)
    .setTools(List.of(...))
    .setSystemMessage(new SystemMessageConfig()
        .setMode(SystemMessageMode.APPEND)
        .setContent("Custom instructions"))
    .setAvailableTools(List.of("tool1", "tool2"))
    .setExcludedTools(List.of("tool3"))
    .setProvider(new ProviderConfig().setType("openai"))
    .setOnPermissionRequest(PermissionHandler.APPROVE_ALL)
).get();
```
会话配置选项-`sessionId`—自定义会话ID
-`clientName`-应用名称
-`model`-模型名称（"gpt-5", “claude-sonnet-4.5”等）
-`reasoningEffort`- "low", "medium", "high", “xhigh”
-`tools`-向CLI公开的自定义工具
-`systemMessage`-系统消息定制
-`availableTools`允许工具名称列表
-`excludedTools`-工具名称黑名单`provider`自定义API提供程序配置（BYOK）
-`streaming`-启用流响应块（默认：false）
—`workingDirectory`—会话工作目录
—`mcpServers`—MCP服务器配置
-`customAgents`-自定义座席配置
-`agent`-按名称预选代理
-`infiniteSessions`-无限会话配置
-`skillDirectories`- SkillSKILL.md目录
-`disabledSkills`-禁用的技能
—`configDir`—配置目录路径
-`hooks`-会话生命周期钩子
-`onPermissionRequest`- **REQUIRED**权限处理程序
-`onUserInputRequest`-用户输入处理程序
-`onEvent`-之前注册的事件处理程序重新创建会话对于方法链接，所有setter返回`SessionConfig`。

###恢复会话```java
var session = client.resumeSession(sessionId, new ResumeSessionConfig()
    .setOnPermissionRequest(PermissionHandler.APPROVE_ALL)
).get();
```
会话操作

-`session.getSessionId()`获取会话标识符
-`session.send(prompt)`/`session.send(MessageOptions)`-发送消息，返回`CompletableFuture<String>`（消息ID，用于关联）
-`session.sendAndWait(prompt)`/`session.sendAndWait(MessageOptions)`-发送并等待响应（60s超时）
-`session.sendAndWait(options, timeoutMs)`-发送并等待自定义超时
-`session.abort()`-中止当前处理
-`session.getMessages()`-得到所有events/messages-`session.setModel(modelId)`-切换到其他型号
-`session.setModel(modelId, reasoningEffort)`-带推理力切换模型（"low", "medium", "high", "xhigh"）
-`session.setModel(modelId, reasoningEffort, modelCapabilities)`-与`ModelCapabilitiesOverride`切换模型（从1.3.0开始）
-`session.log(message)`/`session.log(message, "warning", false)`/`session.log(message, "error", false)`-日志到会话时间线级别`"info"`，`"warning"`，或`"error"`-`session.log(message, level, ephemeral, url)`-日志与可点击的URL链接
-`session.close()`-清理资源

##事件处理

事件订阅模式

使用`CompletableFuture`来等待会话事件：```java
var done = new CompletableFuture<Void>();

session.on(event -> {
    if (event instanceof AssistantMessageEvent msg) {
        System.out.println(msg.getData().content());
    } else if (event instanceof SessionIdleEvent) {
        done.complete(null);
    }
});

session.send(new MessageOptions().setPrompt("Hello"));
done.get();
```
类型安全的事件处理

使用类型化的`on()`重载来保证编译时的安全：```java
var done = new java.util.concurrent.CompletableFuture<Void>();

session.on(AssistantMessageEvent.class, msg -> {
    System.out.println(msg.getData().content());
});

session.on(SessionIdleEvent.class, idle -> {
    done.complete(null);
});
```
###取消订阅事件

方法返回一个`Closeable`：```java
var subscription = session.on(event -> { /* handler */ });
// Later...
subscription.close();
```
事件类型

使用模式匹配（Java 17+）进行事件处理：```java
session.on(event -> {
    if (event instanceof UserMessageEvent userMsg) {
        // Handle user message
    } else if (event instanceof AssistantMessageEvent assistantMsg) {
        System.out.println(assistantMsg.getData().content());
    } else if (event instanceof AssistantMessageDeltaEvent delta) {
        System.out.print(delta.getData().deltaContent());
    } else if (event instanceof ToolExecutionStartEvent toolStart) {
        // Tool execution started
    } else if (event instanceof ToolExecutionCompleteEvent toolComplete) {
        // Tool execution completed
    } else if (event instanceof SessionStartEvent start) {
        // Session started
    } else if (event instanceof SessionIdleEvent idle) {
        // Session is idle (processing complete)
    } else if (event instanceof SessionErrorEvent error) {
        System.err.println("Error: " + error.getData().message());
    }
});
```
###事件错误处理

控制如何处理事件处理程序中的错误：```java
// Set a custom error handler
session.setEventErrorHandler(ex -> {
    logger.error("Event handler error", ex);
});

// Or set the error propagation policy
session.setEventErrorPolicy(EventErrorPolicy.SUPPRESS_AND_LOG_ERRORS);
```
`EventErrorPolicy`值:
-`PROPAGATE_AND_LOG_ERRORS`-在错误时停止事件调度（默认）
-`SUPPRESS_AND_LOG_ERRORS`-继续调度，记录错误

##流式响应

###启用流

在SessionConfig中设置`streaming(true)`：```java
var session = client.createSession(new SessionConfig()
    .setModel("gpt-5")
    .setStreaming(true)
    .setOnPermissionRequest(PermissionHandler.APPROVE_ALL)
).get();
```
处理流事件

处理增量事件和最终事件：

要求Java 21+，但最好是25。```java
var done = new CompletableFuture<Void>();

session.on(event -> {
    switch (event) {
        case AssistantMessageDeltaEvent delta ->
            // Incremental text chunk
            System.out.print(delta.getData().deltaContent());
        case AssistantReasoningDeltaEvent reasoningDelta ->
            // Incremental reasoning chunk (model-dependent)
            System.out.print(reasoningDelta.getData().deltaContent());
        case AssistantMessageEvent msg ->
            // Final complete message
            System.out.println("\n--- Final ---\n" + msg.getData().content());
        case AssistantReasoningEvent reasoning ->
            // Final reasoning content
            System.out.println("--- Reasoning ---\n" + reasoning.getData().content());
        case SessionIdleEvent idle ->
            done.complete(null);
        default -> { }
    }
});

session.send(new MessageOptions().setPrompt("Tell me a story"));
done.get();
```
注意：无论流设置如何，最终事件（`AssistantMessageEvent`,`AssistantReasoningEvent`）总是被发送。

##自定义工具

定义工具

使用`ToolDefinition.create()`与JSON模式参数和`ToolHandler`：```java
var tool = ToolDefinition.create(
    "get_weather",
    "Get weather for a location",
    Map.of(
        "type", "object",
        "properties", Map.of(
            "location", Map.of("type", "string", "description", "City name")
        ),
        "required", List.of("location")
    ),
    invocation -> {
        String location = (String) invocation.getArguments().get("location");
        return CompletableFuture.completedFuture("Sunny in " + location);
    }
);

var session = client.createSession(new SessionConfig()
    .setModel("gpt-5")
    .setTools(List.of(tool))
    .setOnPermissionRequest(PermissionHandler.APPROVE_ALL)
).get();
```
类型安全的工具参数

使用`getArgumentsAs()`反序列化到键入的记录或类：```java
record WeatherArgs(String location, String unit) {}

var tool = ToolDefinition.create(
    "get_weather",
    "Get weather for a location",
    Map.of(
        "type", "object",
        "properties", Map.of(
            "location", Map.of("type", "string"),
            "unit", Map.of("type", "string", "enum", List.of("celsius", "fahrenheit"))
        ),
        "required", List.of("location")
    ),
    invocation -> {
        var args = invocation.getArgumentsAs(WeatherArgs.class);
        return CompletableFuture.completedFuture(
            Map.of("temp", 72, "unit", args.unit(), "location", args.location())
        );
    }
);
```
重写内置工具```java
var override = ToolDefinition.createOverride(
    "built_in_tool_name",
    "Custom description",
    Map.of("type", "object", "properties", Map.of(...)),
    invocation -> CompletableFuture.completedFuture("custom result")
);
```
跳过权限检查（从1.2.0开始）

使用`createSkipPermission()`定义一个绕过CLI权限请求流的工具：```java
var tool = ToolDefinition.createSkipPermission(
    "safe_read_only_tool",
    "A tool that needs no permission confirmation",
    Map.of("type", "object", "properties", Map.of(...)),
    invocation -> CompletableFuture.completedFuture("result")
);
```
工具返回类型

-返回任何json可序列化的值（字符串，映射，列表，记录，POJO）
—SDK自动将返回值进行序列化，并返回给CLI

工具执行流

当Copilot调用一个工具时，客户端会自动：
1. 对论点进行反序列化
2. 运行处理程序函数
3. 序列化返回值
4. 响应CLI

##权限处理

需要的权限处理程序

在创建或恢复会话时，权限处理程序是**强制性的：```java
// Approve all requests (for development/testing)
new SessionConfig()
    .setOnPermissionRequest(PermissionHandler.APPROVE_ALL)

// Custom permission logic
new SessionConfig()
    .setOnPermissionRequest((request, invocation) -> {
        if ("dangerous-action".equals(request.getKind())) {
            return CompletableFuture.completedFuture(
                new PermissionRequestResult().setKind(PermissionRequestResultKind.DENIED)
            );
        }
        return CompletableFuture.completedFuture(
            new PermissionRequestResult().setKind(PermissionRequestResultKind.APPROVED)
        );
    })
```
##用户输入处理

处理来自代理的用户输入请求：```java
new SessionConfig()
    .setOnUserInputRequest((request, invocation) -> {
        System.out.println("Agent asks: " + request.getQuestion());
        String answer = scanner.nextLine();
        return CompletableFuture.completedFuture(
            new UserInputResponse()
                .setAnswer(answer)
                .setWasFreeform(true)
        );
    })
```
系统消息定制

附加模式（默认-保留护栏）```java
var session = client.createSession(new SessionConfig()
    .setModel("gpt-5")
    .setSystemMessage(new SystemMessageConfig()
        .setMode(SystemMessageMode.APPEND)
        .setContent("""
            <workflow_rules>
            - Always check for security vulnerabilities
            - Suggest performance improvements when applicable
            </workflow_rules>
            """))
    .setOnPermissionRequest(PermissionHandler.APPROVE_ALL)
).get();
```
替换模式（完全控制-移除护栏）```java
var session = client.createSession(new SessionConfig()
    .setModel("gpt-5")
    .setSystemMessage(new SystemMessageConfig()
        .setMode(SystemMessageMode.REPLACE)
        .setContent("You are a helpful assistant."))
    .setOnPermissionRequest(PermissionHandler.APPROVE_ALL)
).get();
```
##文件附件

使用`Attachment`将文件附加到消息：```java
session.send(new MessageOptions()
    .setPrompt("Analyze this file")
    .setAttachments(List.of(
        new Attachment("file", "/path/to/file.java", "My File")
    ))
);
```
##消息传递模式

在`MessageOptions`中使用`mode`属性：

-`"enqueue"`-用于处理的队列消息（默认）
-`"immediate"`-立即处理消息```java
session.send(new MessageOptions()
    .setPrompt("...")
    .setMode("enqueue")
);
```
方便：发送和等待

使用`sendAndWait()`发送消息并阻塞，直到助手响应：```java
// With default 60-second timeout
AssistantMessageEvent response = session.sendAndWait("What is 2+2?").get();
System.out.println(response.getData().content());

// With custom timeout
AssistantMessageEvent response = session.sendAndWait(
    new MessageOptions().setPrompt("Write a long story"),
    120_000  // 120 seconds
).get();
```
##多会话

会话是独立的，可以并发运行：```java
var session1 = client.createSession(new SessionConfig()
    .setModel("gpt-5")
    .setOnPermissionRequest(PermissionHandler.APPROVE_ALL)
).get();

var session2 = client.createSession(new SessionConfig()
    .setModel("claude-sonnet-4.5")
    .setOnPermissionRequest(PermissionHandler.APPROVE_ALL)
).get();

session1.send(new MessageOptions().setPrompt("Hello from session 1"));
session2.send(new MessageOptions().setPrompt("Hello from session 2"));
```
自带钥匙（BYOK）

通过`ProviderConfig`使用自定义API提供商：```java
// OpenAI
var session = client.createSession(new SessionConfig()
    .setProvider(new ProviderConfig()
        .setType("openai")
        .setBaseUrl("https://api.openai.com/v1")
        .setApiKey("sk-..."))
    .setOnPermissionRequest(PermissionHandler.APPROVE_ALL)
).get();

// Azure OpenAI
var session = client.createSession(new SessionConfig()
    .setProvider(new ProviderConfig()
        .setType("azure")
        .setAzure(new AzureOptions()
            .setEndpoint("https://my-resource.openai.azure.com")
            .setDeployment("gpt-4"))
        .setBearerToken("..."))
    .setOnPermissionRequest(PermissionHandler.APPROVE_ALL)
).get();
```
会话生命周期管理

列出会话```java
var sessions = client.listSessions().get();
for (var metadata : sessions) {
    System.out.println("Session: " + metadata.getSessionId());
}
```
过滤会话

使用`SessionListFilter`可以根据工作目录、git根目录、仓库或分支来缩小结果范围：```java
var filter = new SessionListFilter()
    .setRepository("owner/repo")
    .setBranch("main");

var sessions = client.listSessions(filter).get();
```
删除会话```java
client.deleteSession(sessionId).get();
```
检查连接状态```java
var state = client.getState();
```
生命周期事件订阅```java
AutoCloseable subscription = client.onLifecycle(event -> {
    System.out.println("Lifecycle event: " + event);
});
// Later...
subscription.close();
```
过滤生命周期事件

订阅特定的生命周期事件类型：```java
AutoCloseable subscription = client.onLifecycle("session.created", event -> {
    System.out.println("New session created");
});
```
##错误处理

标准异常处理```java
try {
    var session = client.createSession(new SessionConfig()
        .setOnPermissionRequest(PermissionHandler.APPROVE_ALL)
    ).get();
    session.sendAndWait("Hello").get();
} catch (ExecutionException ex) {
    Throwable cause = ex.getCause();
    System.err.println("Error: " + cause.getMessage());
} catch (Exception ex) {
    System.err.println("Error: " + ex.getMessage());
}
```
会话错误事件

监视`SessionErrorEvent`的运行时错误：```java
session.on(SessionErrorEvent.class, error -> {
    System.err.println("Session Error: " + error.getData().message());
});
```
##连接测试

使用`ping()`验证服务器连通性：```java
var response = client.ping("test message").get();
```
##状态和认证```java
// Get CLI version and protocol info
var status = client.getStatus().get();

// Check authentication status
var authStatus = client.getAuthStatus().get();

// List available models
var models = client.listModels().get();
```
##资源清理

使用try-with-resources自动清理

总是使用try-with-resources自动处理：```java
try (var client = new CopilotClient()) {
    client.start().get();
    try (var session = client.createSession(new SessionConfig()
            .setOnPermissionRequest(PermissionHandler.APPROVE_ALL)).get()) {
        // Use session...
    }
}
// Resources automatically cleaned up
```
###手动清理

如果不使用try-with-resources：```java
var client = new CopilotClient();
try {
    client.start().get();
    // Use client...
} finally {
    client.stop().get();
}
```
最佳实践

1. **对于`CopilotClient`和`CopilotSession`总是使用try-with-resources**
2. **始终提供权限处理程序** -`createSession`和`resumeSession`都需要
3. **正确使用`CompletableFuture`** -调用`.get()`来阻塞，或与`.thenApply()`/`.thenCompose()`连接
4. **使用`sendAndWait()`**用于简单的请求-响应模式，而不是手动事件处理
5. **处理`SessionErrorEvent`**健壮的错误处理
6. **使用模式匹配**（与密封类型切换）进行事件处理
7. **在交互场景中启用流**以获得更好的用户体验
8. **关闭事件订阅** (`Closeable`)当不再需要
9. **使用`SystemMessageMode.APPEND`**保护安全护栏
10. **提供描述性工具名称和描述，以便更好地理解模型
11. **处理增量和最终事件**时，流是启用的
12. **使用`getArgumentsAs()`**进行类型安全的工具参数反序列化

##常见模式简单查询-响应```java
try (var client = new CopilotClient()) {
    client.start().get();

    try (var session = client.createSession(new SessionConfig()
            .setModel("gpt-5")
            .setOnPermissionRequest(PermissionHandler.APPROVE_ALL)).get()) {

        var response = session.sendAndWait("What is 2+2?").get();
        System.out.println(response.getData().content());
    }
}
```
事件驱动的对话```java
try (var client = new CopilotClient()) {
    client.start().get();

    try (var session = client.createSession(new SessionConfig()
            .setModel("gpt-5")
            .setOnPermissionRequest(PermissionHandler.APPROVE_ALL)).get()) {

        var done = new CompletableFuture<Void>();

        session.on(AssistantMessageEvent.class, msg ->
            System.out.println(msg.getData().content()));

        session.on(SessionIdleEvent.class, idle ->
            done.complete(null));

        session.send(new MessageOptions().setPrompt("What is 2+2?"));
        done.get();
    }
}
```
多回合对话```java
try (var session = client.createSession(new SessionConfig()
        .setModel("gpt-5")
        .setOnPermissionRequest(PermissionHandler.APPROVE_ALL)).get()) {

    var response1 = session.sendAndWait("What is the capital of France?").get();
    System.out.println(response1.getData().content());

    var response2 = session.sendAndWait("What is its population?").get();
    System.out.println(response2.getData().content());
}
```
复杂返回类型的工具```java
record UserInfo(String id, String name, String email, String role) {}

var tool = ToolDefinition.create(
    "get_user",
    "Retrieve user information",
    Map.of(
        "type", "object",
        "properties", Map.of(
            "userId", Map.of("type", "string", "description", "User ID")
        ),
        "required", List.of("userId")
    ),
    invocation -> {
        String userId = (String) invocation.getArguments().get("userId");
        return CompletableFuture.completedFuture(
            new UserInfo(userId, "John Doe", "john@example.com", "Developer")
        );
    }
);
```
会话钩子```java
var session = client.createSession(new SessionConfig()
    .setModel("gpt-5")
    .setOnPermissionRequest(PermissionHandler.APPROVE_ALL)
    .setHooks(new SessionHooks()
        .setOnPreToolUse((input, invocation) -> {
            System.out.println("About to execute tool: " + input.getToolName());
            // Use static factory methods on PreToolUseHookOutput:
            // PreToolUseHookOutput.allow()
            // PreToolUseHookOutput.deny()
            // PreToolUseHookOutput.deny("reason")
            // PreToolUseHookOutput.ask()
            return CompletableFuture.completedFuture(PreToolUseHookOutput.allow());
        })
        .setOnPostToolUse((output, invocation) -> {
            System.out.println("Tool execution complete: " + output);
            return CompletableFuture.completedFuture(null);
        })
        .setOnUserPromptSubmitted((prompt, invocation) -> {
            // Intercept user prompts before processing
            return CompletableFuture.completedFuture(null);
        })
        .setOnSessionStart((event, invocation) -> {
            return CompletableFuture.completedFuture(null);
        })
        .setOnSessionEnd((event, invocation) -> {
            return CompletableFuture.completedFuture(null);
        }))
).get();
```
MCP服务器配置

通过`SessionConfig.setMcpServers()`配置模型上下文协议服务器：

基于stdio的MCP服务器```java
var mcpServers = Map.of(
    "my-server", new McpStdioServerConfig()
        .setCommand("node")
        .setArgs(List.of("path/to/server.js"))
);

var session = client.createSession(new SessionConfig()
    .setModel("gpt-5")
    .setMcpServers(mcpServers)
    .setOnPermissionRequest(PermissionHandler.APPROVE_ALL)
).get();
```
###HTTP/SSEMCP服务器```java
var mcpServers = Map.of(
    "remote-server", new McpHttpServerConfig()
        .setUrl("https://my-mcp-server.example.com/sse")
);

var session = client.createSession(new SessionConfig()
    .setModel("gpt-5")
    .setMcpServers(mcpServers)
    .setOnPermissionRequest(PermissionHandler.APPROVE_ALL)
).get();
```
模型功能覆盖（从1.3.0开始）

覆盖BYOK或自定义提供程序的模型功能：```java
var capabilities = new ModelCapabilitiesOverride()
    .setSupports(new ModelCapabilitiesOverride.Supports()
        .setVision(true)
        .setReasoningEffort(true))
    .setLimits(new ModelCapabilitiesOverride.Limits()
        .setMaxPromptTokens(128000));

var session = client.createSession(new SessionConfig()
    .setModel("custom-model")
    .setModelCapabilities(capabilities)
    .setOnPermissionRequest(PermissionHandler.APPROVE_ALL)
).get();
```
