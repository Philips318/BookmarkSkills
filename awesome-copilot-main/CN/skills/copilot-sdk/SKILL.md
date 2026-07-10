---
name: copilot-sdk
description: Build agentic applications with GitHub Copilot SDK. Use when embedding AI agents in apps, creating custom tools, implementing streaming responses, managing sessions, connecting to MCP servers, or creating custom agents. Triggers on Copilot SDK, GitHub SDK, agentic app, embed Copilot, programmable agent, MCP server, custom agent.
---
#GitHub CopilotSDK

在使用Python、TypeScript、Go或. net的任何应用程序中嵌入Copilot的代理工作流。

# #概述GitHub CopilotSDK公开了Copilot CLI背后的相同引擎：一个经过生产测试的代理运行时，您可以通过编程方式调用它。无需构建自己的编排——您定义代理行为，Copilot处理规划、工具调用、文件编辑等。

# #先决条件

1. **GitHub CopilotCLI**已安装并验证（[安装指南](https://docs.github.com/en/copilot/how-tos/set-up/install-copilot-cli)）
2. **语言运行时：Node.js18+， Python 3.8+, Go 1.21+，或。NET 8.0 +

验证命令行：`copilot --version`# #安装### Node.js/TypeScript
```bash
mkdir copilot-demo && cd copilot-demo
npm init -y --init-type module
npm install @github/copilot-sdk tsx
```
# # # Python```bash
pip install github-copilot-sdk
```
# # #去```bash
mkdir copilot-demo && cd copilot-demo
go mod init copilot-demo
go get github.com/github/copilot-sdk/go
```
# # #。网```bash
dotnet new console -n CopilotDemo && cd CopilotDemo
dotnet add package GitHub.Copilot.SDK
```
##快速入门

# # #打印稿```typescript
import { CopilotClient, approveAll } from "@github/copilot-sdk";

const client = new CopilotClient();
const session = await client.createSession({
    onPermissionRequest: approveAll,
    model: "gpt-4.1",
});

const response = await session.sendAndWait({ prompt: "What is 2 + 2?" });
console.log(response?.data.content);

await client.stop();
process.exit(0);
```
运行:`npx tsx index.ts`# # # Python```python
import asyncio
from copilot import CopilotClient, PermissionHandler

async def main():
    client = CopilotClient()
    await client.start()

    session = await client.create_session({
        "on_permission_request": PermissionHandler.approve_all,
        "model": "gpt-4.1",
    })
    response = await session.send_and_wait({"prompt": "What is 2 + 2?"})

    print(response.data.content)
    await client.stop()

asyncio.run(main())
```
# # #去```go
package main

import (
    "fmt"
    "log"
    "os"
    copilot "github.com/github/copilot-sdk/go"
)

func main() {
    client := copilot.NewClient(nil)
    if err := client.Start(); err != nil {
        log.Fatal(err)
    }
    defer client.Stop()

    session, err := client.CreateSession(&copilot.SessionConfig{
        OnPermissionRequest: copilot.PermissionHandler.ApproveAll,
        Model:               "gpt-4.1",
    })
    if err != nil {
        log.Fatal(err)
    }

    response, err := session.SendAndWait(copilot.MessageOptions{Prompt: "What is 2 + 2?"}, 0)
    if err != nil {
        log.Fatal(err)
    }

    fmt.Println(*response.Data.Content)
    os.Exit(0)
}
```
# # #。净(c#)```csharp
using GitHub.Copilot.SDK;

await using var client = new CopilotClient();
await using var session = await client.CreateSessionAsync(new SessionConfig
{
    OnPermissionRequest = PermissionHandler.ApproveAll,
    Model = "gpt-4.1",
});

var response = await session.SendAndWaitAsync(new MessageOptions { Prompt = "What is 2 + 2?" });
Console.WriteLine(response?.Data.Content);
```
运行:`dotnet run`##流式响应

启用实时输出以获得更好的用户体验：

# # #打印稿```typescript
import { CopilotClient, approveAll, SessionEvent } from "@github/copilot-sdk";

const client = new CopilotClient();
const session = await client.createSession({
    onPermissionRequest: approveAll,
    model: "gpt-4.1",
    streaming: true,
});

session.on((event: SessionEvent) => {
    if (event.type === "assistant.message_delta") {
        process.stdout.write(event.data.deltaContent);
    }
    if (event.type === "session.idle") {
        console.log(); // New line when done
    }
});

await session.sendAndWait({ prompt: "Tell me a short joke" });

await client.stop();
process.exit(0);
```
# # # Python```python
import asyncio
import sys
from copilot import CopilotClient, PermissionHandler
from copilot.generated.session_events import SessionEventType

async def main():
    client = CopilotClient()
    await client.start()

    session = await client.create_session({
        "on_permission_request": PermissionHandler.approve_all,
        "model": "gpt-4.1",
        "streaming": True,
    })

    def handle_event(event):
        if event.type == SessionEventType.ASSISTANT_MESSAGE_DELTA:
            sys.stdout.write(event.data.delta_content)
            sys.stdout.flush()
        if event.type == SessionEventType.SESSION_IDLE:
            print()

    session.on(handle_event)
    await session.send_and_wait({"prompt": "Tell me a short joke"})
    await client.stop()

asyncio.run(main())
```
# # #去```go
session, err := client.CreateSession(&copilot.SessionConfig{
	OnPermissionRequest: copilot.PermissionHandler.ApproveAll,
    Model:     "gpt-4.1",
    Streaming: true,
})

session.On(func(event copilot.SessionEvent) {
    if event.Type == "assistant.message_delta" {
        fmt.Print(*event.Data.DeltaContent)
    }
    if event.Type == "session.idle" {
        fmt.Println()
    }
})

_, err = session.SendAndWait(copilot.MessageOptions{Prompt: "Tell me a short joke"}, 0)
```
# # #。网```csharp
await using var session = await client.CreateSessionAsync(new SessionConfig
{
    OnPermissionRequest = PermissionHandler.ApproveAll,
    Model = "gpt-4.1",
    Streaming = true,
});

session.On(ev =>
{
    if (ev is AssistantMessageDeltaEvent deltaEvent)
        Console.Write(deltaEvent.Data.DeltaContent);
    if (ev is SessionIdleEvent)
        Console.WriteLine();
});

await session.SendAndWaitAsync(new MessageOptions { Prompt = "Tell me a short joke" });
```
##自定义工具

定义Copilot在推理过程中可以调用的工具。当你定义一个工具时，你告诉副驾驶：
1. **工具的功能**（描述）
2. **需要哪些参数** （schema）
3. **运行什么代码** （handler）

### TypeScript （JSON模式）```typescript
import { CopilotClient, approveAll, defineTool, SessionEvent } from "@github/copilot-sdk";

const getWeather = defineTool("get_weather", {
    description: "Get the current weather for a city",
    parameters: {
        type: "object",
        properties: {
            city: { type: "string", description: "The city name" },
        },
        required: ["city"],
    },
    handler: async (args: { city: string }) => {
        const { city } = args;
        // In a real app, call a weather API here
        const conditions = ["sunny", "cloudy", "rainy", "partly cloudy"];
        const temp = Math.floor(Math.random() * 30) + 50;
        const condition = conditions[Math.floor(Math.random() * conditions.length)];
        return { city, temperature: `${temp}°F`, condition };
    },
});

const client = new CopilotClient();
const session = await client.createSession({
    onPermissionRequest: approveAll,
    model: "gpt-4.1",
    streaming: true,
    tools: [getWeather],
});

session.on((event: SessionEvent) => {
    if (event.type === "assistant.message_delta") {
        process.stdout.write(event.data.deltaContent);
    }
});

await session.sendAndWait({
    prompt: "What's the weather like in Seattle and Tokyo?",
});

await client.stop();
process.exit(0);
```
Python （Pydantic）```python
import asyncio
import random
import sys
from copilot import CopilotClient, PermissionHandler
from copilot.tools import define_tool
from copilot.generated.session_events import SessionEventType
from pydantic import BaseModel, Field

class GetWeatherParams(BaseModel):
    city: str = Field(description="The name of the city to get weather for")

@define_tool(description="Get the current weather for a city")
async def get_weather(params: GetWeatherParams) -> dict:
    city = params.city
    conditions = ["sunny", "cloudy", "rainy", "partly cloudy"]
    temp = random.randint(50, 80)
    condition = random.choice(conditions)
    return {"city": city, "temperature": f"{temp}°F", "condition": condition}

async def main():
    client = CopilotClient()
    await client.start()

    session = await client.create_session({
        "on_permission_request": PermissionHandler.approve_all,
        "model": "gpt-4.1",
        "streaming": True,
        "tools": [get_weather],
    })

    def handle_event(event):
        if event.type == SessionEventType.ASSISTANT_MESSAGE_DELTA:
            sys.stdout.write(event.data.delta_content)
            sys.stdout.flush()

    session.on(handle_event)

    await session.send_and_wait({
        "prompt": "What's the weather like in Seattle and Tokyo?"
    })

    await client.stop()

asyncio.run(main())
```
# # #去```go
type WeatherParams struct {
    City string `json:"city" jsonschema:"The city name"`
}

type WeatherResult struct {
    City        string `json:"city"`
    Temperature string `json:"temperature"`
    Condition   string `json:"condition"`
}

getWeather := copilot.DefineTool(
    "get_weather",
    "Get the current weather for a city",
    func(params WeatherParams, inv copilot.ToolInvocation) (WeatherResult, error) {
        conditions := []string{"sunny", "cloudy", "rainy", "partly cloudy"}
        temp := rand.Intn(30) + 50
        condition := conditions[rand.Intn(len(conditions))]
        return WeatherResult{
            City:        params.City,
            Temperature: fmt.Sprintf("%d°F", temp),
            Condition:   condition,
        }, nil
    },
)

session, _ := client.CreateSession(&copilot.SessionConfig{
	OnPermissionRequest: copilot.PermissionHandler.ApproveAll,
    Model:     "gpt-4.1",
    Streaming: true,
    Tools:     []copilot.Tool{getWeather},
})
```
# # #。净(Microsoft.Extensions.AI)```csharp
using GitHub.Copilot.SDK;
using Microsoft.Extensions.AI;
using System.ComponentModel;

var getWeather = AIFunctionFactory.Create(
    ([Description("The city name")] string city) =>
    {
        var conditions = new[] { "sunny", "cloudy", "rainy", "partly cloudy" };
        var temp = Random.Shared.Next(50, 80);
        var condition = conditions[Random.Shared.Next(conditions.Length)];
        return new { city, temperature = $"{temp}°F", condition };
    },
    "get_weather",
    "Get the current weather for a city"
);

await using var session = await client.CreateSessionAsync(new SessionConfig
{
    OnPermissionRequest = PermissionHandler.ApproveAll,
    Model = "gpt-4.1",
    Streaming = true,
    Tools = [getWeather],
});
```
##工具如何工作

当副驾驶决定调用你的工具时：
1. 副驾驶发送带有参数的工具调用请求
2. SDK运行处理程序函数
3. 结果被发送回副驾驶
4. 副驾驶将结果纳入其响应中

Copilot根据用户的问题和工具的描述来决定何时调用你的工具。

交互式CLI助手

建立一个完整的互动助手：

# # #打印稿```typescript
import { CopilotClient, approveAll, defineTool, SessionEvent } from "@github/copilot-sdk";
import * as readline from "readline";

const getWeather = defineTool("get_weather", {
    description: "Get the current weather for a city",
    parameters: {
        type: "object",
        properties: {
            city: { type: "string", description: "The city name" },
        },
        required: ["city"],
    },
    handler: async ({ city }) => {
        const conditions = ["sunny", "cloudy", "rainy", "partly cloudy"];
        const temp = Math.floor(Math.random() * 30) + 50;
        const condition = conditions[Math.floor(Math.random() * conditions.length)];
        return { city, temperature: `${temp}°F`, condition };
    },
});

const client = new CopilotClient();
const session = await client.createSession({
    onPermissionRequest: approveAll,
    model: "gpt-4.1",
    streaming: true,
    tools: [getWeather],
});

session.on((event: SessionEvent) => {
    if (event.type === "assistant.message_delta") {
        process.stdout.write(event.data.deltaContent);
    }
});

const rl = readline.createInterface({
    input: process.stdin,
    output: process.stdout,
});

console.log("Weather Assistant (type 'exit' to quit)");
console.log("Try: 'What's the weather in Paris?'\n");

const prompt = () => {
    rl.question("You: ", async (input) => {
        if (input.toLowerCase() === "exit") {
            await client.stop();
            rl.close();
            return;
        }

        process.stdout.write("Assistant: ");
        await session.sendAndWait({ prompt: input });
        console.log("\n");
        prompt();
    });
};

prompt();
```
# # # Python```python
import asyncio
import random
import sys
from copilot import CopilotClient, PermissionHandler
from copilot.tools import define_tool
from copilot.generated.session_events import SessionEventType
from pydantic import BaseModel, Field

class GetWeatherParams(BaseModel):
    city: str = Field(description="The name of the city to get weather for")

@define_tool(description="Get the current weather for a city")
async def get_weather(params: GetWeatherParams) -> dict:
    conditions = ["sunny", "cloudy", "rainy", "partly cloudy"]
    temp = random.randint(50, 80)
    condition = random.choice(conditions)
    return {"city": params.city, "temperature": f"{temp}°F", "condition": condition}

async def main():
    client = CopilotClient()
    await client.start()

    session = await client.create_session({
        "on_permission_request": PermissionHandler.approve_all,
        "model": "gpt-4.1",
        "streaming": True,
        "tools": [get_weather],
    })

    def handle_event(event):
        if event.type == SessionEventType.ASSISTANT_MESSAGE_DELTA:
            sys.stdout.write(event.data.delta_content)
            sys.stdout.flush()

    session.on(handle_event)

    print("Weather Assistant (type 'exit' to quit)")
    print("Try: 'What's the weather in Paris?'\n")

    while True:
        try:
            user_input = input("You: ")
        except EOFError:
            break

        if user_input.lower() == "exit":
            break

        sys.stdout.write("Assistant: ")
        await session.send_and_wait({"prompt": user_input})
        print("\n")

    await client.stop()

asyncio.run(main())
```
MCP服务器集成

连接到MCP（模型上下文协议）服务器，用于预构建工具。连接到GitHub的MCP服务器，用于存储库，issue和PR访问：

# # #打印稿```typescript
const session = await client.createSession({
    onPermissionRequest: approveAll,
    model: "gpt-4.1",
    mcpServers: {
        github: {
            type: "http",
            url: "https://api.githubcopilot.com/mcp/",
        },
    },
});
```
# # # Python```python
session = await client.create_session({
    "on_permission_request": PermissionHandler.approve_all,
    "model": "gpt-4.1",
    "mcp_servers": {
        "github": {
            "type": "http",
            "url": "https://api.githubcopilot.com/mcp/",
        },
    },
})
```
# # #去```go
session, _ := client.CreateSession(&copilot.SessionConfig{
	OnPermissionRequest: copilot.PermissionHandler.ApproveAll,
    Model: "gpt-4.1",
    MCPServers: map[string]copilot.MCPServerConfig{
        "github": {
            "type": "http",
            "url": "https://api.githubcopilot.com/mcp/",
        },
    },
})
```
# # #。网```csharp
await using var session = await client.CreateSessionAsync(new SessionConfig
{
    OnPermissionRequest = PermissionHandler.ApproveAll,
    Model = "gpt-4.1",
    McpServers = new Dictionary<string, McpServerConfig>
    {
        ["github"] = new McpServerConfig
        {
            Type = "http",
            Url = "https://api.githubcopilot.com/mcp/",
        },
    },
});
```
自定义代理

为特定任务定义专门的AI角色：

# # #打印稿```typescript
const session = await client.createSession({
    onPermissionRequest: approveAll,
    model: "gpt-4.1",
    customAgents: [{
        name: "pr-reviewer",
        displayName: "PR Reviewer",
        description: "Reviews pull requests for best practices",
        prompt: "You are an expert code reviewer. Focus on security, performance, and maintainability.",
    }],
});
```
# # # Python```python
session = await client.create_session({
    "on_permission_request": PermissionHandler.approve_all,
    "model": "gpt-4.1",
    "custom_agents": [{
        "name": "pr-reviewer",
        "display_name": "PR Reviewer",
        "description": "Reviews pull requests for best practices",
        "prompt": "You are an expert code reviewer. Focus on security, performance, and maintainability.",
    }],
})
```
##系统信息

定制AI的行为和个性：

# # #打印稿```typescript
const session = await client.createSession({
    onPermissionRequest: approveAll,
    model: "gpt-4.1",
    systemMessage: {
        content: "You are a helpful assistant for our engineering team. Always be concise.",
    },
});
```
# # # Python```python
session = await client.create_session({
    "on_permission_request": PermissionHandler.approve_all,
    "model": "gpt-4.1",
    "system_message": {
        "content": "You are a helpful assistant for our engineering team. Always be concise.",
    },
})
```
外部CLI服务器

在服务器模式下单独运行CLI，并连接SDK。用于调试、资源共享或自定义环境。

###在服务器模式下启动CLI```bash
copilot --server --port 4321
```
###连接SDK到外部服务器

# # # #打印稿```typescript
const client = new CopilotClient({
    cliUrl: "localhost:4321"
});

const session = await client.createSession({
    onPermissionRequest: approveAll,
    model: "gpt-4.1",
});
```
# # # # Python```python
client = CopilotClient({
    "cli_url": "localhost:4321"
})
await client.start()

session = await client.create_session({
    "on_permission_request": PermissionHandler.approve_all,
    "model": "gpt-4.1",
})
```
# # # #```go
client := copilot.NewClient(&copilot.ClientOptions{
    CLIUrl: "localhost:4321",
})

if err := client.Start(); err != nil {
    log.Fatal(err)
}

session, _ := client.CreateSession(&copilot.SessionConfig{
	OnPermissionRequest: copilot.PermissionHandler.ApproveAll,
	Model:               "gpt-4.1",
})
```
# # # #。网```csharp
using var client = new CopilotClient(new CopilotClientOptions
{
    CliUrl = "localhost:4321"
});

await using var session = await client.CreateSessionAsync(new SessionConfig
{
    OnPermissionRequest = PermissionHandler.ApproveAll,
    Model = "gpt-4.1",
});
```
**注意：**当提供`cliUrl`时，SDK不会生成或管理CLI进程-它只连接到现有的服务器。

##事件类型

|事件|事件描述||-------|-------------|
|`user.message`|新增用户输入|
|`assistant.message`|完整模型响应|
|`assistant.message_delta`|流响应块|
|`assistant.reasoning`|模型推理（模型依赖）|
|`assistant.reasoning_delta`|流推理块|
|`tool.execution_start`|工具调用开始|
|`tool.execution_complete`|工具执行完成|
|`session.idle`|无活动处理|
|`session.error`| |发生错误

##客户端配置

|选项|描述|默认||--------|-------------|---------|
|`cliPath`|副驾驶命令行可执行文件路径|系统路径|
|`cliUrl`|连接到现有服务器（例如，“localhost:4321”）|无|
|`port`|服务器通信端口|随机|
|`useStdio`|使用演播室传输代替TCP | true |
|`logLevel`|日志详细信息| "info" |
|`autoStart`|自动启动服务器| true |
|`autoRestart`|崩溃重启| true |
|`cwd`| CLI进程工作目录|继承|

##会话配置

|选项|描述||--------|-------------|
|`model`| LLM使用（“gpt-4.1”，“claude-sonnet-4.5”等）|
|`sessionId`|自定义会话标识符|
|`tools`|自定义工具定义|
|`mcpServers`| MCP服务器连接|
|`customAgents`|自定义代理角色|
|`systemMessage`|覆盖默认系统提示符|
|`streaming`|启用增量响应块|
|`availableTools`|允许使用的工具白名单|
|`excludedTools`|禁用工具黑名单|

##会话持久性

保存和恢复跨重启的对话：

###创建自定义ID```typescript
const session = await client.createSession({
    onPermissionRequest: approveAll,
    sessionId: "user-123-conversation",
    model: "gpt-4.1"
});
```
###恢复会话```typescript
const session = await client.resumeSession("user-123-conversation", { onPermissionRequest: approveAll });
await session.send({ prompt: "What did we discuss earlier?" });
```
###列出和删除会话```typescript
const sessions = await client.listSessions();
await client.deleteSession("old-session-id");
```
##错误处理```typescript
try {
    const client = new CopilotClient();
    const session = await client.createSession({
        onPermissionRequest: approveAll,
        model: "gpt-4.1",
    });
    const response = await session.sendAndWait(
        { prompt: "Hello!" },
        30000 // timeout in ms
    );
} catch (error) {
    if (error.code === "ENOENT") {
        console.error("Copilot CLI not installed");
    } else if (error.code === "ECONNREFUSED") {
        console.error("Cannot connect to Copilot server");
    } else {
        console.error("Error:", error.message);
    }
} finally {
    await client.stop();
}
```
##安全关机```typescript
process.on("SIGINT", async () => {
    console.log("Shutting down...");
    await client.stop();
    process.exit(0);
});
```
##常见模式

多回合对话```typescript
const session = await client.createSession({
    onPermissionRequest: approveAll,
    model: "gpt-4.1",
});

await session.sendAndWait({ prompt: "My name is Alice" });
await session.sendAndWait({ prompt: "What's my name?" });
// Response: "Your name is Alice"
```
###文件附件```typescript
await session.send({
    prompt: "Analyze this file",
    attachments: [{
        type: "file",
        path: "./data.csv",
        displayName: "Sales Data"
    }]
});
```
终止长操作```typescript
const timeoutId = setTimeout(() => {
    session.abort();
}, 60000);

session.on((event) => {
    if (event.type === "session.idle") {
        clearTimeout(timeoutId);
    }
});
```
##可用型号

在运行时查询可用模型：```typescript
const models = await client.getModels();
// Returns: ["gpt-4.1", "gpt-4o", "claude-sonnet-4.5", ...]
```
最佳实践

1. **总是清理**：使用`try-finally`或`defer`来确保`client.stop()`被调用
2. **设置超时时间**：使用`sendAndWait`与超时长操作
3. **处理事件**：订阅错误事件以进行稳健的错误处理
4. **使用流媒体**：启用流媒体以获得更好的长响应用户体验
5. **持久会话**：使用自定义会话id的多回合对话
6. **定义清晰的工具**：编写描述性的工具名称和描述

# #架构```
Your Application
       |
  SDK Client
       | JSON-RPC
  Copilot CLI (server mode)
       |
  GitHub (models, auth)
```
SDK自动管理CLI进程生命周期。所有通信都是通过studio或TCP上的JSON-RPC进行的。

# #资源

- **GitHub存储库**:https://github.com/github/copilot-sdk- **入门教程**:https://github.com/github/copilot-sdk/blob/main/docs/tutorials/first-app.md- **GitHub MCP服务器**:https://github.com/github/github-mcp-server—**MCP服务器目录**:https://github.com/modelcontextprotocol/servers- **食谱**:https://github.com/github/copilot-sdk/tree/main/cookbook- **样例**:https://github.com/github/copilot-sdk/tree/main/samples# #状态

这个SDK是在**技术预览**和可能有突破性的变化。不推荐用于生产环境。