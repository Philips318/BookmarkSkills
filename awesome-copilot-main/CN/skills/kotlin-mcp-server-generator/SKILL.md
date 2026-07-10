---
name: kotlin-mcp-server-generator
description: 'Generate a complete Kotlin MCP server project with proper structure, dependencies, and implementation using the official io.modelcontextprotocol:kotlin-sdk library.'
---
# Kotlin MCP服务器项目生成器

在Kotlin中生成一个完整的、生产就绪的模型上下文协议（Model Context Protocol， MCP）服务器项目。

##项目需求

您将使用以下内容创建Kotlin MCP服务器：

1. **项目结构：基于gradle的Kotlin项目布局
2. **依赖项**：官方MCP SDK， Ktor和kotlinx库
3. **服务器设置**：配置MCP服务器与传输
4. **工具**：至少2-3个有用的工具，输入inputs/outputs5. **错误处理**：正确的异常处理和验证
6. **文档**:README与安装和使用说明
7. **Testing**：带有协程的基本测试结构

模板结构```
myserver/
├── build.gradle.kts
├── settings.gradle.kts
├── gradle.properties
├── src/
│   ├── main/
│   │   └── kotlin/
│   │       └── com/example/myserver/
│   │           ├── Main.kt
│   │           ├── Server.kt
│   │           ├── config/
│   │           │   └── Config.kt
│   │           └── tools/
│   │               ├── Tool1.kt
│   │               └── Tool2.kt
│   └── test/
│       └── kotlin/
│           └── com/example/myserver/
│               └── ServerTest.kt
└── README.md
```
build.gradle.kts模板```kotlin
plugins {
    kotlin("jvm") version "2.1.0"
    kotlin("plugin.serialization") version "2.1.0"
    application
}

group = "com.example"
version = "1.0.0"

repositories {
    mavenCentral()
}

dependencies {
    implementation("io.modelcontextprotocol:kotlin-sdk:0.7.2")
    
    // Ktor for transports
    implementation("io.ktor:ktor-server-netty:3.0.0")
    implementation("io.ktor:ktor-client-cio:3.0.0")
    
    // Serialization
    implementation("org.jetbrains.kotlinx:kotlinx-serialization-json:1.7.3")
    
    // Coroutines
    implementation("org.jetbrains.kotlinx:kotlinx-coroutines-core:1.9.0")
    
    // Logging
    implementation("io.github.oshai:kotlin-logging-jvm:7.0.0")
    implementation("ch.qos.logback:logback-classic:1.5.12")
    
    // Testing
    testImplementation(kotlin("test"))
    testImplementation("org.jetbrains.kotlinx:kotlinx-coroutines-test:1.9.0")
}

application {
    mainClass.set("com.example.myserver.MainKt")
}

tasks.test {
    useJUnitPlatform()
}

kotlin {
    jvmToolchain(17)
}
```
## settings.gradle.kts模板```kotlin
rootProject.name = "{{PROJECT_NAME}}"
```
# #主要。kt模板```kotlin
package com.example.myserver

import io.modelcontextprotocol.kotlin.sdk.server.StdioServerTransport
import kotlinx.coroutines.runBlocking
import io.github.oshai.kotlinlogging.KotlinLogging

private val logger = KotlinLogging.logger {}

fun main() = runBlocking {
    logger.info { "Starting MCP server..." }
    
    val config = loadConfig()
    val server = createServer(config)
    
    // Use stdio transport
    val transport = StdioServerTransport()
    
    logger.info { "Server '${config.name}' v${config.version} ready" }
    server.connect(transport)
}
```
# #服务器。kt模板```kotlin
package com.example.myserver

import io.modelcontextprotocol.kotlin.sdk.server.Server
import io.modelcontextprotocol.kotlin.sdk.server.ServerOptions
import io.modelcontextprotocol.kotlin.sdk.Implementation
import io.modelcontextprotocol.kotlin.sdk.ServerCapabilities
import com.example.myserver.tools.registerTools

fun createServer(config: Config): Server {
    val server = Server(
        serverInfo = Implementation(
            name = config.name,
            version = config.version
        ),
        options = ServerOptions(
            capabilities = ServerCapabilities(
                tools = ServerCapabilities.Tools(),
                resources = ServerCapabilities.Resources(
                    subscribe = true,
                    listChanged = true
                ),
                prompts = ServerCapabilities.Prompts(listChanged = true)
            )
        )
    ) {
        config.description
    }
    
    // Register all tools
    server.registerTools()
    
    return server
}
```
# #配置。kt模板```kotlin
package com.example.myserver.config

import kotlinx.serialization.Serializable

@Serializable
data class Config(
    val name: String = "{{PROJECT_NAME}}",
    val version: String = "1.0.0",
    val description: String = "{{PROJECT_DESCRIPTION}}"
)

fun loadConfig(): Config {
    return Config(
        name = System.getenv("SERVER_NAME") ?: "{{PROJECT_NAME}}",
        version = System.getenv("VERSION") ?: "1.0.0",
        description = System.getenv("DESCRIPTION") ?: "{{PROJECT_DESCRIPTION}}"
    )
}
```
# # Tool1。kt模板```kotlin
package com.example.myserver.tools

import io.modelcontextprotocol.kotlin.sdk.server.Server
import io.modelcontextprotocol.kotlin.sdk.CallToolRequest
import io.modelcontextprotocol.kotlin.sdk.CallToolResult
import io.modelcontextprotocol.kotlin.sdk.TextContent
import kotlinx.serialization.json.buildJsonObject
import kotlinx.serialization.json.put
import kotlinx.serialization.json.putJsonObject
import kotlinx.serialization.json.putJsonArray

fun Server.registerTool1() {
    addTool(
        name = "tool1",
        description = "Description of what tool1 does",
        inputSchema = buildJsonObject {
            put("type", "object")
            putJsonObject("properties") {
                putJsonObject("param1") {
                    put("type", "string")
                    put("description", "First parameter")
                }
                putJsonObject("param2") {
                    put("type", "integer")
                    put("description", "Optional second parameter")
                }
            }
            putJsonArray("required") {
                add("param1")
            }
        }
    ) { request: CallToolRequest ->
        // Extract and validate parameters
        val param1 = request.params.arguments["param1"] as? String
            ?: throw IllegalArgumentException("param1 is required")
        val param2 = (request.params.arguments["param2"] as? Number)?.toInt() ?: 0
        
        // Perform tool logic
        val result = performTool1Logic(param1, param2)
        
        CallToolResult(
            content = listOf(
                TextContent(text = result)
            )
        )
    }
}

private fun performTool1Logic(param1: String, param2: Int): String {
    // Implement tool logic here
    return "Processed: $param1 with value $param2"
}
```
##tools/ToolRegistry.kt模板```kotlin
package com.example.myserver.tools

import io.modelcontextprotocol.kotlin.sdk.server.Server

fun Server.registerTools() {
    registerTool1()
    registerTool2()
    // Register additional tools here
}
```
# # ServerTest。kt模板```kotlin
package com.example.myserver

import kotlinx.coroutines.test.runTest
import kotlin.test.Test
import kotlin.test.assertEquals
import kotlin.test.assertFalse

class ServerTest {
    
    @Test
    fun `test server creation`() = runTest {
        val config = Config(
            name = "test-server",
            version = "1.0.0",
            description = "Test server"
        )
        
        val server = createServer(config)
        
        assertEquals("test-server", server.serverInfo.name)
        assertEquals("1.0.0", server.serverInfo.version)
    }
    
    @Test
    fun `test tool1 execution`() = runTest {
        val config = Config()
        val server = createServer(config)
        
        // Test tool execution
        // Note: You'll need to implement proper testing utilities
        // for calling tools in the server
    }
}
```
##README.md模板```markdown
# {{PROJECT_NAME}}

A Model Context Protocol (MCP) server built with Kotlin.

## Description

{{PROJECT_DESCRIPTION}}

## Requirements

- Java 17 or higher
- Kotlin 2.1.0

## Installation

Build the project:

\`\`\`bash
./gradlew build
\`\`\`

## Usage

Run the server with stdio transport:

\`\`\`bash
./gradlew run
\`\`\`

Or build and run the jar:

\`\`\`bash
./gradlew installDist
./build/install/{{PROJECT_NAME}}/bin/{{PROJECT_NAME}}
\`\`\`

## Configuration

Configure via environment variables:

- `SERVER_NAME`: Server name (default: "{{PROJECT_NAME}}")
- `VERSION`: Server version (default: "1.0.0")
- `DESCRIPTION`: Server description

## Available Tools

### tool1
{{TOOL1_DESCRIPTION}}

**Input:**
- `param1` (string, required): First parameter
- `param2` (integer, optional): Second parameter

**Output:**
- Text result of the operation

## Development

Run tests:

\`\`\`bash
./gradlew test
\`\`\`

Build:

\`\`\`bash
./gradlew build
\`\`\`

Run with auto-reload (development):

\`\`\`bash
./gradlew run --continuous
\`\`\`

## Multiplatform

This project uses Kotlin Multiplatform and can target JVM, Wasm, and iOS.
See `build.gradle.kts` for platform configuration.

## License

MIT
```
##生成指令

在生成Kotlin MCP服务器时：

1. **Gradle安装**：用所有依赖项创建正确的`build.gradle.kts`2. **包结构**：遵循Kotlin包约定
3. **类型安全：使用数据类和kotlinx.序列化
4. 协程：所有的操作都应该是挂起函数
5. **错误处理**：使用Kotlin异常和验证
6. **JSON模式**：使用`buildJsonObject`作为工具模式
7. **测试**：包括协程测试工具
8. **Logging**：使用kotlin-logging进行结构化日志记录
9. **配置**：使用数据类和环境变量
10. **文档**：公共api的KDoc注释

最佳实践—对所有异步操作使用挂起函数
-利用Kotlin的null安全和类型系统
-对结构化数据使用数据类
—应用kotlinx。序列化JSON处理
-对结果类型使用密封类
-实现正确的错误处理Result/Either模式
-使用kotlinx-coroutines-test编写测试
-使用依赖注入实现可测试性
-遵循Kotlin编码约定
-使用有意义的名称和KDoc注释

##传输选项

###演播室传输```kotlin
val transport = StdioServerTransport()
server.connect(transport)
```
SSE Transport （Ktor）```kotlin
embeddedServer(Netty, port = 8080) {
    mcp {
        Server(/*...*/) { "Description" }
    }
}.start(wait = true)
```
##多平台配置

对于多平台项目，添加到`build.gradle.kts`：```kotlin
kotlin {
    jvm()
    js(IR) { nodejs() }
    wasmJs()
    
    sourceSets {
        commonMain.dependencies {
            implementation("io.modelcontextprotocol:kotlin-sdk:0.7.2")
        }
    }
}
```
