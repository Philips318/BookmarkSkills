---
description: 'Best practices and patterns for building Model Context Protocol (MCP) servers in Java using the official MCP Java SDK with reactive streams and Spring integration.'
applyTo: "**/*.java, **/pom.xml, **/build.gradle, **/build.gradle.kts"
---
# Java MCP服务器开发指南

在Java中构建MCP服务器时，请使用官方Java SDK遵循这些最佳实践和模式。

# #依赖性

将MCP Java SDK添加到Maven项目中：```xml
<dependencies>
    <dependency>
        <groupId>io.modelcontextprotocol.sdk</groupId>
        <artifactId>mcp</artifactId>
        <version>0.14.1</version>
    </dependency>
</dependencies>
```
或者对于Gradle：```kotlin
dependencies {
    implementation("io.modelcontextprotocol.sdk:mcp:0.14.1")
}
```
##服务器设置

使用构建器模式创建MCP服务器：```java
import io.mcp.server.McpServer;
import io.mcp.server.McpServerBuilder;
import io.mcp.server.transport.StdioServerTransport;

McpServer server = McpServerBuilder.builder()
    .serverInfo("my-server", "1.0.0")
    .capabilities(capabilities -> capabilities
        .tools(true)
        .resources(true)
        .prompts(true))
    .build();

// Start with stdio transport
StdioServerTransport transport = new StdioServerTransport();
server.start(transport).subscribe();
```
##添加工具

向服务器注册工具处理程序：```java
import io.mcp.server.tool.Tool;
import io.mcp.server.tool.ToolHandler;
import reactor.core.publisher.Mono;

// Define a tool
Tool searchTool = Tool.builder()
    .name("search")
    .description("Search for information")
    .inputSchema(JsonSchema.object()
        .property("query", JsonSchema.string()
            .description("Search query")
            .required(true))
        .property("limit", JsonSchema.integer()
            .description("Maximum results")
            .defaultValue(10)))
    .build();

// Register tool handler
server.addToolHandler("search", (arguments) -> {
    String query = arguments.get("query").asText();
    int limit = arguments.has("limit") 
        ? arguments.get("limit").asInt() 
        : 10;
    
    // Perform search
    List<String> results = performSearch(query, limit);
    
    return Mono.just(ToolResponse.success()
        .addTextContent("Found " + results.size() + " results")
        .build());
});
```
##添加资源

实现数据访问的资源处理程序：```java
import io.mcp.server.resource.Resource;
import io.mcp.server.resource.ResourceHandler;

// Register resource list handler
server.addResourceListHandler(() -> {
    List<Resource> resources = List.of(
        Resource.builder()
            .name("Data File")
            .uri("resource://data/example.txt")
            .description("Example data file")
            .mimeType("text/plain")
            .build()
    );
    return Mono.just(resources);
});

// Register resource read handler
server.addResourceReadHandler((uri) -> {
    if (uri.equals("resource://data/example.txt")) {
        String content = loadResourceContent(uri);
        return Mono.just(ResourceContent.text(content, uri));
    }
    throw new ResourceNotFoundException(uri);
});

// Register resource subscribe handler
server.addResourceSubscribeHandler((uri) -> {
    subscriptions.add(uri);
    log.info("Client subscribed to {}", uri);
    return Mono.empty();
});
```
##添加提示

为模板化对话实现提示处理程序：```java
import io.mcp.server.prompt.Prompt;
import io.mcp.server.prompt.PromptMessage;
import io.mcp.server.prompt.PromptArgument;

// Register prompt list handler
server.addPromptListHandler(() -> {
    List<Prompt> prompts = List.of(
        Prompt.builder()
            .name("analyze")
            .description("Analyze a topic")
            .argument(PromptArgument.builder()
                .name("topic")
                .description("Topic to analyze")
                .required(true)
                .build())
            .argument(PromptArgument.builder()
                .name("depth")
                .description("Analysis depth")
                .required(false)
                .build())
            .build()
    );
    return Mono.just(prompts);
});

// Register prompt get handler
server.addPromptGetHandler((name, arguments) -> {
    if (name.equals("analyze")) {
        String topic = arguments.getOrDefault("topic", "general");
        String depth = arguments.getOrDefault("depth", "basic");
        
        List<PromptMessage> messages = List.of(
            PromptMessage.user("Please analyze this topic: " + topic),
            PromptMessage.assistant("I'll provide a " + depth + " analysis of " + topic)
        );
        
        return Mono.just(PromptResult.builder()
            .description("Analysis of " + topic + " at " + depth + " level")
            .messages(messages)
            .build());
    }
    throw new PromptNotFoundException(name);
});
```
响应式流模式

Java SDK使用响应式流（Project Reactor）进行异步处理：```java
// Return Mono for single results
server.addToolHandler("process", (args) -> {
    return Mono.fromCallable(() -> {
        String result = expensiveOperation(args);
        return ToolResponse.success()
            .addTextContent(result)
            .build();
    }).subscribeOn(Schedulers.boundedElastic());
});

// Return Flux for streaming results
server.addResourceListHandler(() -> {
    return Flux.fromIterable(getResources())
        .map(r -> Resource.builder()
            .uri(r.getUri())
            .name(r.getName())
            .build())
        .collectList();
});
```
##同步Facade

对于阻塞用例，使用同步API：```java
import io.mcp.server.McpSyncServer;

McpSyncServer syncServer = server.toSyncServer();

// Blocking tool handler
syncServer.addToolHandler("greet", (args) -> {
    String name = args.get("name").asText();
    return ToolResponse.success()
        .addTextContent("Hello, " + name + "!")
        .build();
});
```
##传输配置

###演播室传输

对于本地子进程通信：```java
import io.mcp.server.transport.StdioServerTransport;

StdioServerTransport transport = new StdioServerTransport();
server.start(transport).block();
```
HTTP传输（Servlet）

对于基于http的服务器：```java
import io.mcp.server.transport.ServletServerTransport;
import jakarta.servlet.http.HttpServlet;

public class McpServlet extends HttpServlet {
    private final McpServer server;
    private final ServletServerTransport transport;
    
    public McpServlet() {
        this.server = createMcpServer();
        this.transport = new ServletServerTransport();
    }
    
    @Override
    protected void doPost(HttpServletRequest req, HttpServletResponse resp) {
        transport.handleRequest(server, req, resp).block();
    }
}
```
Spring Boot集成

使用Spring Boot启动器实现无缝集成：```xml
<dependency>
    <groupId>io.modelcontextprotocol.sdk</groupId>
    <artifactId>mcp-spring-boot-starter</artifactId>
    <version>0.14.1</version>
</dependency>
```
使用Spring配置服务器：```java
import org.springframework.context.annotation.Configuration;
import io.mcp.spring.McpServerConfigurer;

@Configuration
public class McpConfiguration {
    
    @Bean
    public McpServerConfigurer mcpServerConfigurer() {
        return server -> server
            .serverInfo("spring-server", "1.0.0")
            .capabilities(cap -> cap
                .tools(true)
                .resources(true)
                .prompts(true));
    }
}
```
将处理程序注册为Spring bean：```java
import org.springframework.stereotype.Component;
import io.mcp.spring.ToolHandler;

@Component
public class SearchToolHandler implements ToolHandler {
    
    @Override
    public String getName() {
        return "search";
    }
    
    @Override
    public Tool getTool() {
        return Tool.builder()
            .name("search")
            .description("Search for information")
            .inputSchema(JsonSchema.object()
                .property("query", JsonSchema.string().required(true)))
            .build();
    }
    
    @Override
    public Mono<ToolResponse> handle(JsonNode arguments) {
        String query = arguments.get("query").asText();
        return Mono.just(ToolResponse.success()
            .addTextContent("Search results for: " + query)
            .build());
    }
}
```
##错误处理

使用正确的错误处理MCP异常：```java
server.addToolHandler("risky", (args) -> {
    return Mono.fromCallable(() -> {
        try {
            String result = riskyOperation(args);
            return ToolResponse.success()
                .addTextContent(result)
                .build();
        } catch (ValidationException e) {
            return ToolResponse.error()
                .message("Invalid input: " + e.getMessage())
                .build();
        } catch (Exception e) {
            log.error("Unexpected error", e);
            return ToolResponse.error()
                .message("Internal error occurred")
                .build();
        }
    });
});
```
JSON模式构建

使用fluent架构构建器：```java
import io.mcp.json.JsonSchema;

JsonSchema schema = JsonSchema.object()
    .property("name", JsonSchema.string()
        .description("User's name")
        .minLength(1)
        .maxLength(100)
        .required(true))
    .property("age", JsonSchema.integer()
        .description("User's age")
        .minimum(0)
        .maximum(150))
    .property("email", JsonSchema.string()
        .description("Email address")
        .format("email")
        .required(true))
    .property("tags", JsonSchema.array()
        .items(JsonSchema.string())
        .uniqueItems(true))
    .additionalProperties(false)
    .build();
```
日志记录和可观察性

使用SLF4J进行日志：```java
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

private static final Logger log = LoggerFactory.getLogger(MyMcpServer.class);

server.addToolHandler("process", (args) -> {
    log.info("Tool called: process, args: {}", args);
    
    return Mono.fromCallable(() -> {
        String result = process(args);
        log.debug("Processing completed successfully");
        return ToolResponse.success()
            .addTextContent(result)
            .build();
    }).doOnError(error -> {
        log.error("Processing failed", error);
    });
});
```
用反应器传播上下文：```java
import reactor.util.context.Context;

server.addToolHandler("traced", (args) -> {
    return Mono.deferContextual(ctx -> {
        String traceId = ctx.get("traceId");
        log.info("Processing with traceId: {}", traceId);
        
        return Mono.just(ToolResponse.success()
            .addTextContent("Processed")
            .build());
    });
});
```
# #测试

使用同步API编写测试：```java
import org.junit.jupiter.api.Test;
import static org.assertj.core.Assertions.assertThat;

class McpServerTest {
    
    @Test
    void testToolHandler() {
        McpServer server = createTestServer();
        McpSyncServer syncServer = server.toSyncServer();
        
        JsonNode args = objectMapper.createObjectNode()
            .put("query", "test");
        
        ToolResponse response = syncServer.callTool("search", args);
        
        assertThat(response.isError()).isFalse();
        assertThat(response.getContent()).hasSize(1);
    }
}
```
##杰克逊一体化

SDK使用Jackson进行JSON序列化。根据需要定制：```java
import com.fasterxml.jackson.databind.ObjectMapper;
import com.fasterxml.jackson.datatype.jsr310.JavaTimeModule;

ObjectMapper mapper = new ObjectMapper();
mapper.registerModule(new JavaTimeModule());

// Use custom mapper with server
McpServer server = McpServerBuilder.builder()
    .objectMapper(mapper)
    .build();
```
##内容类型

在响应中支持多种内容类型：```java
import io.mcp.server.content.Content;

server.addToolHandler("multi", (args) -> {
    return Mono.just(ToolResponse.success()
        .addTextContent("Plain text response")
        .addImageContent(imageBytes, "image/png")
        .addResourceContent("resource://data", "application/json", jsonData)
        .build());
});
```
##服务器生命周期

正确管理服务器生命周期：```java
import reactor.core.Disposable;

Disposable serverDisposable = server.start(transport).subscribe();

// Graceful shutdown
Runtime.getRuntime().addShutdownHook(new Thread(() -> {
    log.info("Shutting down MCP server");
    serverDisposable.dispose();
    server.stop().block();
}));
```
##常见模式

请求验证```java
server.addToolHandler("validate", (args) -> {
    if (!args.has("required_field")) {
        return Mono.just(ToolResponse.error()
            .message("Missing required_field")
            .build());
    }
    
    return processRequest(args);
});
```
异步操作```java
server.addToolHandler("async", (args) -> {
    return Mono.fromCallable(() -> callExternalApi(args))
        .timeout(Duration.ofSeconds(30))
        .onErrorResume(TimeoutException.class, e -> 
            Mono.just(ToolResponse.error()
                .message("Operation timed out")
                .build()))
        .subscribeOn(Schedulers.boundedElastic());
});
```
资源缓存```java
private final Map<String, String> cache = new ConcurrentHashMap<>();

server.addResourceReadHandler((uri) -> {
    return Mono.fromCallable(() -> 
        cache.computeIfAbsent(uri, this::loadResource))
        .map(content -> ResourceContent.text(content, uri));
});
```
最佳实践

1. **使用响应式流**异步操作和反压
2. **利用Spring Boot** starter为企业应用程序
3. **对特定的错误信息实施适当的错误处理**
4. **使用SLF4J**记录日志，而不是System.out
5. **验证工具和提示处理器中的输入**
6. **支持正常关机**和适当的资源清理
7. **使用有界弹性调度程序**阻塞操作
8. 在反应链中传播可观察性的上下文
9. **测试与同步API**为简单
10. **遵循Java命名约定** （camelCase表示方法，PascalCase表示类）