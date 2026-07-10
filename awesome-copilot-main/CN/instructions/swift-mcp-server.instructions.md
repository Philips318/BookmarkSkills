---
description: 'Best practices and patterns for building Model Context Protocol (MCP) servers in Swift using the official MCP Swift SDK package.'
applyTo: "**/*.swift, **/Package.swift, **/Package.resolved"
---
# Swift MCP服务器开发指南

在Swift中构建MCP服务器时，请使用官方Swift SDK遵循这些最佳实践和模式。

##服务器设置

使用具有以下功能的`Server`类创建MCP服务器：```swift
import MCP

let server = Server(
    name: "MyServer",
    version: "1.0.0",
    capabilities: .init(
        prompts: .init(listChanged: true),
        resources: .init(subscribe: true, listChanged: true),
        tools: .init(listChanged: true)
    )
)
```
##添加工具

使用`withMethodHandler`注册工具处理程序：```swift
// Register tool list handler
await server.withMethodHandler(ListTools.self) { _ in
    let tools = [
        Tool(
            name: "search",
            description: "Search for information",
            inputSchema: .object([
                "properties": .object([
                    "query": .string("Search query"),
                    "limit": .number("Maximum results")
                ]),
                "required": .array([.string("query")])
            ])
        )
    ]
    return .init(tools: tools)
}

// Register tool call handler
await server.withMethodHandler(CallTool.self) { params in
    switch params.name {
    case "search":
        let query = params.arguments?["query"]?.stringValue ?? ""
        let limit = params.arguments?["limit"]?.intValue ?? 10
        
        // Perform search
        let results = performSearch(query: query, limit: limit)
        
        return .init(
            content: [.text("Found \(results.count) results")],
            isError: false
        )
        
    default:
        return .init(
            content: [.text("Unknown tool")],
            isError: true
        )
    }
}
```
##添加资源

实现数据访问的资源处理程序：```swift
// Register resource list handler
await server.withMethodHandler(ListResources.self) { params in
    let resources = [
        Resource(
            name: "Data File",
            uri: "resource://data/example.txt",
            description: "Example data file",
            mimeType: "text/plain"
        )
    ]
    return .init(resources: resources, nextCursor: nil)
}

// Register resource read handler
await server.withMethodHandler(ReadResource.self) { params in
    switch params.uri {
    case "resource://data/example.txt":
        let content = loadResourceContent(uri: params.uri)
        return .init(contents: [
            Resource.Content.text(
                content,
                uri: params.uri,
                mimeType: "text/plain"
            )
        ])
        
    default:
        throw MCPError.invalidParams("Unknown resource URI: \(params.uri)")
    }
}

// Register resource subscribe handler
await server.withMethodHandler(ResourceSubscribe.self) { params in
    // Track subscription for notifications
    subscriptions.insert(params.uri)
    print("Client subscribed to \(params.uri)")
    return .init()
}
```
##添加提示

为模板化对话实现提示处理程序：```swift
// Register prompt list handler
await server.withMethodHandler(ListPrompts.self) { params in
    let prompts = [
        Prompt(
            name: "analyze",
            description: "Analyze a topic",
            arguments: [
                .init(name: "topic", description: "Topic to analyze", required: true),
                .init(name: "depth", description: "Analysis depth", required: false)
            ]
        )
    ]
    return .init(prompts: prompts, nextCursor: nil)
}

// Register prompt get handler
await server.withMethodHandler(GetPrompt.self) { params in
    switch params.name {
    case "analyze":
        let topic = params.arguments?["topic"]?.stringValue ?? "general"
        let depth = params.arguments?["depth"]?.stringValue ?? "basic"
        
        let description = "Analysis of \(topic) at \(depth) level"
        let messages: [Prompt.Message] = [
            .user("Please analyze this topic: \(topic)"),
            .assistant("I'll provide a \(depth) analysis of \(topic)")
        ]
        
        return .init(description: description, messages: messages)
        
    default:
        throw MCPError.invalidParams("Unknown prompt: \(params.name)")
    }
}
```
##传输配置

###演播室传输

对于本地子进程通信：```swift
import MCP
import Logging

let logger = Logger(label: "com.example.mcp-server")
let transport = StdioTransport(logger: logger)

try await server.start(transport: transport)
```
HTTP传输（客户端）

对于远程服务器连接：```swift
let transport = HTTPClientTransport(
    endpoint: URL(string: "http://localhost:8080")!,
    streaming: true  // Enable Server-Sent Events
)

try await client.connect(transport: transport)
```
并发性和actor

服务器是一个参与者，确保线程安全访问：```swift
actor ServerState {
    private var subscriptions: Set<String> = []
    private var cache: [String: Any] = [:]
    
    func addSubscription(_ uri: String) {
        subscriptions.insert(uri)
    }
    
    func getSubscriptions() -> Set<String> {
        return subscriptions
    }
}

let state = ServerState()

await server.withMethodHandler(ResourceSubscribe.self) { params in
    await state.addSubscription(params.uri)
    return .init()
}
```
##错误处理

使用Swift的错误处理`MCPError`：```swift
await server.withMethodHandler(CallTool.self) { params in
    do {
        guard let query = params.arguments?["query"]?.stringValue else {
            throw MCPError.invalidParams("Missing query parameter")
        }
        
        let result = try performOperation(query: query)
        
        return .init(
            content: [.text(result)],
            isError: false
        )
    } catch let error as MCPError {
        return .init(
            content: [.text(error.localizedDescription)],
            isError: true
        )
    } catch {
        return .init(
            content: [.text("Unexpected error: \(error.localizedDescription)")],
            isError: true
        )
    }
}
```
带值类型的JSON模式

对JSON模式使用`Value`类型：```swift
let schema = Value.object([
    "type": .string("object"),
    "properties": .object([
        "name": .object([
            "type": .string("string"),
            "description": .string("User's name")
        ]),
        "age": .object([
            "type": .string("integer"),
            "minimum": .number(0),
            "maximum": .number(150)
        ]),
        "email": .object([
            "type": .string("string"),
            "format": .string("email")
        ])
    ]),
    "required": .array([.string("name")])
])
```
Swift包管理器安装

创建`Package.swift`：```swift
// swift-tools-version: 6.0
import PackageDescription

let package = Package(
    name: "MyMCPServer",
    platforms: [
        .macOS(.v13),
        .iOS(.v16)
    ],
    dependencies: [
        .package(
            url: "https://github.com/modelcontextprotocol/swift-sdk.git",
            from: "0.10.0"
        ),
        .package(
            url: "https://github.com/apple/swift-log.git",
            from: "1.5.0"
        )
    ],
    targets: [
        .executableTarget(
            name: "MyMCPServer",
            dependencies: [
                .product(name: "MCP", package: "swift-sdk"),
                .product(name: "Logging", package: "swift-log")
            ]
        )
    ]
)
```
ServiceLifecycle的安全关机

使用Swift服务生命周期来正确关闭：```swift
import MCP
import ServiceLifecycle
import Logging

struct MCPService: Service {
    let server: Server
    let transport: Transport
    
    func run() async throws {
        try await server.start(transport: transport)
        try await Task.sleep(for: .days(365 * 100))
    }
    
    func shutdown() async throws {
        await server.stop()
    }
}

let logger = Logger(label: "com.example.mcp-server")
let transport = StdioTransport(logger: logger)
let mcpService = MCPService(server: server, transport: transport)

let serviceGroup = ServiceGroup(
    services: [mcpService],
    configuration: .init(
        gracefulShutdownSignals: [.sigterm, .sigint]
    ),
    logger: logger
)

try await serviceGroup.run()
```
##Async/Await模式

所有服务器操作都使用Swift并发：```swift
await server.withMethodHandler(CallTool.self) { params in
    async let result1 = fetchData1()
    async let result2 = fetchData2()
    
    let combined = await "\(result1) and \(result2)"
    
    return .init(
        content: [.text(combined)],
        isError: false
    )
}
```
# #日志

使用swift-log进行结构化日志记录：```swift
import Logging

let logger = Logger(label: "com.example.mcp-server")

await server.withMethodHandler(CallTool.self) { params in
    logger.info("Tool called", metadata: [
        "name": .string(params.name),
        "args": .string("\(params.arguments ?? [:])")
    ])
    
    // Process tool call
    
    logger.debug("Tool completed successfully")
    
    return .init(content: [.text("Result")], isError: false)
}
```
# #测试

使用async/await测试您的服务器：```swift
import XCTest
@testable import MyMCPServer

final class ServerTests: XCTestCase {
    func testToolCall() async throws {
        let server = createTestServer()
        
        // Test tool call logic
        let params = CallTool.Params(
            name: "search",
            arguments: ["query": .string("test")]
        )
        
        // Verify behavior
        XCTAssertNoThrow(try await processToolCall(params))
    }
}
```
##初始化钩子

使用initialize钩子验证客户端连接：```swift
try await server.start(transport: transport) { clientInfo, clientCapabilities in
    // Validate client
    guard clientInfo.name != "BlockedClient" else {
        throw MCPError.invalidRequest("Client not allowed")
    }
    
    // Check capabilities
    if clientCapabilities.sampling == nil {
        logger.warning("Client doesn't support sampling")
    }
    
    logger.info("Client connected", metadata: [
        "name": .string(clientInfo.name),
        "version": .string(clientInfo.version)
    ])
}
```
##常见模式

###内容类型

处理不同的内容类型：```swift
return .init(
    content: [
        .text("Plain text response"),
        .image(imageData, mimeType: "image/png", metadata: [
            "width": 1024,
            "height": 768
        ]),
        .resource(
            uri: "resource://data",
            mimeType: "application/json",
            text: jsonString
        )
    ],
    isError: false
)
```
###严格配置

使用严格模式在缺少功能时快速失败：```swift
let client = Client(
    name: "StrictClient",
    version: "1.0.0",
    configuration: .strict
)

// Will throw immediately if capability not available
try await client.listTools()
```
请求批处理

有效地发送多个请求：```swift
var tasks: [Task<CallTool.Result, Error>] = []

try await client.withBatch { batch in
    for i in 0..<10 {
        tasks.append(
            try await batch.addRequest(
                CallTool.request(.init(
                    name: "process",
                    arguments: ["id": .number(Double(i))]
                ))
            )
        )
    }
}

for (index, task) in tasks.enumerated() {
    let result = try await task.value
    print("\(index): \(result.content)")
}
```
